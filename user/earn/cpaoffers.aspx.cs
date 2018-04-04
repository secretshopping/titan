using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using System.Data;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Contests;
using Prem.PTC.Offers;
using Resources;
using System.Text.RegularExpressions;
using Titan;

public partial class About : System.Web.UI.Page
{
    #region Data

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (TitanFeatures.IsJ5WalterFreebiesFromHome)
            Page.MasterPageFile = "/J5Walter.master";
    }

    public bool IsDailyCategoryList
    {
        get
        {
            if (ViewState["IsDailyCategoryList"] == null)
                ViewState["IsDailyCategoryList"] = DailyCategoriesButton.CssClass == "ViewSelected" ? true : false;

            return (bool)ViewState["IsDailyCategoryList"];
        }
        set
        {
            ViewState["IsDailyCategoryList"] = value;
        }
    }

    public DeviceType OffersDeviceType
    {
        get
        {
            if (ViewState["OffersDeviceType"] == null)
                ViewState["OffersDeviceType"] = DesktopDeviceTypeButton.CssClass == "ViewSelected" ? (int)DeviceType.Desktop : (int)DeviceType.Mobile;

            return (DeviceType)Convert.ToInt32(ViewState["OffersDeviceType"]);
        }
        set
        {
            ViewState["OffersDeviceType"] = (int)value;
        }
    }

    public int CurrentPageNumber
    {
        get
        {
            if (ViewState["CurrentPageNumber"] == null)
                return 0;

            return Convert.ToInt32(ViewState["CurrentPageNumber"]);
        }
        set
        {
            ViewState["CurrentPageNumber"] = (int)value;
        }
    }

    public int LastPageNumber
    {
        get
        {
            if (ViewState["LastPageNumber"] == null)
                return 2000000000;

            return Convert.ToInt32(ViewState["LastPageNumber"]);
        }
        set
        {
            ViewState["LastPageNumber"] = (int)value;
        }
    }

    private OffersManager _om;
    private Member member;

    public OffersManager OM
    {
        get
        {
            if (_om == null)
                _om = new OffersManager(CurrentUser);
            _om.isDailyOfers = IsDailyCategoryList;
            _om.SelectedDeviceType = OffersDeviceType;
            return _om;
        }
    }

    public Member CurrentUser
    {
        get
        {
            if (member == null)
                member = Member.Current;
            return member;
        }
    }

    #endregion Data

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(Member.IsLogged && AppSettings.TitanFeatures.EarnersRoleEnabled
            && AppSettings.TitanFeatures.EarnCPAGPTEnabled && Member.CurrentInCache.IsEarner);

        FrequencySelectionPlaceHolder.Visible = AppSettings.CPAGPT.DailyNotDailyButtonsEnabled;
        PreviousPageButton.Visible = CurrentPageNumber > 0;

        #region Handlers

        if (Request.QueryString["ignore"] != null)
        {
            //ACTION 1
            //Proceed with Ignore

            int OfferId = Convert.ToInt32(Request.QueryString["ignore"]);
            var Offer = new CPAOffer(OfferId);
            OfferRegisterEntry.AddNew(Offer, Member.CurrentName, OfferStatus.Ignored);

            RemoveElementFromSessionList(OfferId);

            NotificationManager.Refresh(NotificationType.NewCPAOffers);
            this.DataBind();
            Response.Redirect("cpaoffers.aspx?keeporder=1");
        }
        else if (Request.QueryString["report"] != null)
        {
            //ACTION 2
            //Proceed with Report

            int OfferId = Convert.ToInt32(Request.QueryString["report"]);
            var Offer = new CPAOffer(OfferId);

            string ReportText = "none";

            foreach (var t in Request.Form.AllKeys)
                if (t.EndsWith("ReportMessage" + OfferId))
                {
                    ReportText = Request.Form[t];
                    break;
                }

            //Validate
            string regex = @"[a-zA-Z0-9\.\-\,\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,600}";
            var match = Regex.Match(ReportText, regex, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // does match
                var report = new CPAReport();
                report.DateReported = DateTime.Now;
                report.ReportText = ReportText;
                report.ReportingUsername = Member.CurrentName;
                report.OfferId = OfferId;
                report.Save();

                OfferRegisterEntry.AddNew(Offer, Member.CurrentName, OfferStatus.Reported, report.Id.ToString());

                if (!AppSettings.CPAGPT.ReadOnlyModeEnabled)
                    RemoveElementFromSessionList(OfferId);

                NotificationManager.Refresh(NotificationType.NewCPAOffers);
                Response.Redirect("cpaoffers.aspx?keeporder=1");
            }
            else
                WriteError(L1.BADERRREP);
        }
        else if (Request.QueryString["submit"] != null)
        {
            //ACTION 3
            //Proceed with Submit

            int OfferId = Convert.ToInt32(Request.QueryString["submit"]);
            string LoginID = "";
            string EmailID = "";

            foreach (var t in Request.Form.AllKeys)
                if (t.EndsWith("LoginID" + OfferId))
                {
                    LoginID = Request.Form[t];
                    break;
                }

            foreach (var t in Request.Form.AllKeys)
                if (t.EndsWith("EmailID" + OfferId))
                {
                    EmailID = Request.Form[t];
                    break;
                }

            //Validate

            CPAOffer ThisOffer = new CPAOffer(OfferId);

            bool isOk1 = true;
            bool isOk2 = true;

            if (ThisOffer.LoginBoxRequired)
            {
                string regex1 = @"[a-zA-Z0-9\.\-\,\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,80}";
                var match1 = Regex.Match(LoginID, regex1, RegexOptions.IgnoreCase);
                isOk1 = match1.Success;
            }

            if (ThisOffer.EmailBoxRequired)
            {
                string regex2 = @"[a-zA-Z0-9\.\-\,\!\?\&\:\$\@\(\)\+\=\%\*|\\\t\\\n|\\\r|\\\s]{1,400}";
                var match2 = Regex.Match(EmailID, regex2, RegexOptions.IgnoreCase);
                isOk2 = match2.Success;
            }

            if (isOk1 && isOk2)
            {
                //Does match
                //Check for duplicates first
                try
                {
                    OfferRegisterEntry.CheckDuplicateAndStatus(Member.CurrentName, ThisOffer);

                    if (ThisOffer.GlobalDailySubmitsRestricted)
                    {
                        int submissionsGloballyToday = OfferRegisterEntry.GetAllTodaysSubmissionsCountForOffer(ThisOffer.Id);

                        if (submissionsGloballyToday >= ThisOffer.MaxGlobalDailySubmits)
                            throw new MsgException(U3900.DAILYBUTABOVETHELIMIT.Replace("%n1%", submissionsGloballyToday.ToString())
                                                .Replace("%n2%", ThisOffer.MaxGlobalDailySubmits.ToString()));
                    }

                    var entry = OfferRegisterEntry.AddNew(ThisOffer, Member.CurrentName, OfferStatus.Pending, LoginID, EmailID);

                    if (AppSettings.CPAGPT.AutoApprovalEnabled)
                    {
                        CPAManager.AcceptEntryManually(entry, Member.Current);
                    }
                    else
                    {
                        ThisOffer.PerformStatusControlCheck();
                    }

                    RemoveElementFromSessionList(OfferId);
                    NotificationManager.RefreshWithMember(NotificationType.NewCPAOffers, Member.Current);
                    Response.Redirect("cpaoffers.aspx?keeporder=1");
                }
                catch (MsgException ex)
                {
                    WriteError(ex.Message);
                }
            }
            else
                WriteError(L1.ONEFIELDSABAD);
        }
        else if (Request.QueryString["undo"] != null)
        {
            if (Request.QueryString["sender"] != null && Request.QueryString["sender"] != "undefined")
            {
                int offerId = Convert.ToInt32(Request.QueryString["undo"]);

                var where = TableHelper.MakeDictionary("OfferId", offerId);
                where.Add("Username", Member.CurrentName);
                if (Request.QueryString["sender"] == "ignored")
                    where.Add("OfferStatus", (int)OfferStatus.Ignored);

                else if (Request.QueryString["sender"] == "pending")
                    where.Add("OfferStatus", (int)OfferStatus.Pending);

                else if (Request.QueryString["sender"] == "reported")
                    where.Add("OfferStatus", (int)OfferStatus.Reported);

                TableHelper.DeleteRows<OfferRegisterEntry>(where);

                var offer = new CPAOffer(offerId);

                offer.PerformStatusControlCheck();
            }

            NotificationManager.Refresh(NotificationType.NewCPAOffers);
            Response.Redirect("cpaoffers.aspx?keeporder=0");
        }

        #endregion Handlers

        if (!Page.IsPostBack)
        {
            PreviousPageButton.Text = "← " + U4000.PREVIOUSPAGE;
            NextPageButton.Text = U4000.NEXTPAGE + " →";

            if (AppSettings.CPAGPT.ReadOnlyModeEnabled)
            {
                NavigationPlaceHolder.Visible = false;
            }

            if (AppSettings.CPAGPT.ReadOnlyModeEnabled || AppSettings.CPAGPT.AutoApprovalEnabled)
            {
                SortBy1.Items.Remove(SortBy1.Items.FindByValue("ARating"));
                SortBy1.Items.Remove(SortBy1.Items.FindByValue("ALast"));
                SortBy1.Items.Remove(SortBy1.Items.FindByValue("ATimes"));
                SortBy1.Items.Remove(SortBy1.Items.FindByValue("DRating"));
                SortBy1.Items.Remove(SortBy1.Items.FindByValue("DLast"));
                SortBy1.Items.Remove(SortBy1.Items.FindByValue("DTimes"));
            }

            if (TitanFeatures.IsJ5Walter)
                InformationPlaceHolder.Visible = false;

            if (TitanFeatures.IsJ5WalterFreebiesFromHome)
            {
                CustomPageHeaderPlaceHolder.Visible = true;
                StandardPageHeaderPlaceHolder.Visible = false;
                SearchBox1.CssClass = "form-control";
                SearchButton1.Attributes["style"] = "padding: 6px 12px";
                SortBy1.Items.Remove(SortBy1.Items.FindByValue("AMoney"));
                SortBy1.Items.Remove(SortBy1.Items.FindByValue("DMoney"));
            }

            if (TitanFeatures.IsTami9191)
                IsDailyCategoryList = true;

            if (AppSettings.CPAGPT.AutoApprovalEnabled) // Do not show Pending/Under review/Denied tabs
                Button4.Visible = Button6.Visible = Button7.Visible = false;

            if (Request.QueryString["o"] != null)
            {
                //Show particular offer
                try
                {
                    int offerId = Convert.ToInt32(Request.QueryString["o"]);
                    CPAOffer offer = new CPAOffer(offerId);
                    var list = OM.AllActiveOffersForMember;
                    bool exists = false;

                    foreach (var elem in list)
                        if (elem.Id == offerId)
                            exists = true;

                    OffersPanel0.Controls.Clear();

                    if (exists)
                        OffersPanel0.Controls.Add(offer.ToPanel(CurrentUser.Membership.CPAProfitPercent));
                    else
                    {
                        Literal temp = new Literal();
                        temp.Text = U4000.OFFERNOTAVAIL;
                        OffersPanel0.Controls.Add(temp);
                    }

                    MakeCategoriesList();
                    MakeSortingList();
                    makeButtons();

                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
            }
            else
            {
                //Preset the desired device (Desktop is preselected as default)
                if (AppSettings.CPAGPT.DeviceTypeDistinctionEnabled && Device.Current == DeviceType.Mobile)
                {
                    OffersDeviceType = DeviceType.Mobile;
                    MobileDeviceTypeButton.CssClass = "ViewSelected";
                    DesktopDeviceTypeButton.CssClass = "";
                }

                KeepOrder = (Request.QueryString["keeporder"] != null && Request.QueryString["keeporder"] == "1");
                DecideAndUpdate();

                //ACTIVE
                //Categories List and active boxes
                MakeCategoriesList();
                MakeSortingList();
                makeButtons();

                //Recreate if needed
                //Only for ACTIVE
                if (KeepOrder && MenuMultiView.ActiveViewIndex == 0)
                {
                    SearchBox1.Text = SearchBoxInput;
                    CategoriesList.SelectedIndex = CategoryIdInput;
                    SortBy1.SelectedIndex = SortIdInput;
                }

            }
        }

        //Disabling Daily/Other groups for specific client
        if (TitanFeatures.IsBobbyDonev)
        {
            FrequencySelectionPlaceHolder.Visible = !AppSettings.TitanFeatures.OfferLevelsEnabled;
            GroupOfferLevelsPlaceHolder.Visible = AppSettings.TitanFeatures.OfferLevelsEnabled;
        }
        else
            GroupOfferLevelsPlaceHolder.Visible = false;


        if (IsDailyCategoryList)
        {
            DailyCategoriesButton.CssClass = "ViewSelected";
            OtherCategoriesButton.CssClass = "";
        }
    }

    private void RemoveElementFromSessionList(int OfferID)
    {
        try
        {
            var offer = new CPAOffer(OfferID);
            var templist = ActiveRememberedList;

            foreach (var elem in templist)
            {
                if (elem == OfferID)
                {
                    templist.Remove(elem);
                    break;
                }
            }

            ActiveRememberedList = templist;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    #region Helpers

    public int ActiveDailyOffersCount { get; set; }
    public int ActiveNonDailyOffersCount { get; set; }
    public int ActiveDesktopOffersCount { get; set; }
    public int ActiveMobileOffersCount { get; set; }

    public List<CPAOffer> AdjustActiveOffers(List<CPAOffer> IncomeList, bool IncludeCategory = false)
    {
        ActiveDailyOffersCount = ActiveNonDailyOffersCount = ActiveDesktopOffersCount = ActiveMobileOffersCount = 0;

        var resultList = new List<CPAOffer>();

        //1. Seachbox
        if (!string.IsNullOrWhiteSpace(SearchBox1.Text))
        {
            string patttern = SearchBox1.Text.ToLower();
            foreach (var elem in IncomeList)
            {
                if (elem.Title.ToLower().Contains(patttern) || elem.Description.ToLower().Contains(patttern))
                    resultList.Add(elem);
            }
        }
        else
            resultList = IncomeList;

        //2. Category 
        if (CategoriesList.SelectedIndex != -1 && IncludeCategory && CategoriesList.SelectedValue != "ALL")
        {
            int selected = Convert.ToInt32(CategoriesList.SelectedValue);
            CPACategory cat = new CPACategory(selected);
            var newlist = new List<CPAOffer>();
            foreach (var elem in resultList)
            {
                if (elem.Category.Id == cat.Id)
                    newlist.Add(elem);
            }
            resultList = newlist;
        }

        if (TitanFeatures.IsBobbyDonev)
            resultList.Sort(LevelComparison);

        //3. Sorting
        if (SortBy1.SelectedIndex != -1 && SortBy1.SelectedValue != "NONE")
        {
            if (SortBy1.SelectedValue == "ARating")
                resultList.Sort(RatingComparison);
            else if (SortBy1.SelectedValue == "ALast")
                resultList.Sort(LastCreditedComparison);
            else if (SortBy1.SelectedValue == "ADate")
                resultList.Sort(DateAddedComparison);
            else if (SortBy1.SelectedValue == "ATimes")
                resultList.Sort(CompletedTimesComparison);
            else if (SortBy1.SelectedValue == "AMoney")
                resultList.Sort(MoneyEarnedComparison);

            else if (SortBy1.SelectedValue == "DRating")
                resultList.Sort(DRatingComparison);
            else if (SortBy1.SelectedValue == "DLast")
                resultList.Sort(DLastCreditedComparison);
            else if (SortBy1.SelectedValue == "DDate")
                resultList.Sort(DDateAddedComparison);
            else if (SortBy1.SelectedValue == "DTimes")
                resultList.Sort(DCompletedTimesComparison);
            else if (SortBy1.SelectedValue == "DMoney")
                resultList.Sort(DMoneyEarnedComparison);
        }

        //4. Device Type
        if (AppSettings.CPAGPT.DeviceTypeDistinctionEnabled)
        {
            var temporaryList1 = new List<CPAOffer>();
            foreach (var elem in resultList)
            {
                if (elem.DeviceType == OffersDeviceType)
                    temporaryList1.Add(elem);

                if (elem.DeviceType == DeviceType.Desktop)
                    ActiveDesktopOffersCount++;
                else if (elem.DeviceType == DeviceType.Mobile)
                    ActiveMobileOffersCount++;
            }
            resultList = temporaryList1;
        }

        //5. Daily/Not-daily
        foreach (var elem in resultList)
        {
            if (elem.IsDaily)
                ActiveDailyOffersCount++;
            else
                ActiveNonDailyOffersCount++;
        }

        if (AppSettings.CPAGPT.DailyNotDailyButtonsEnabled)
        {
            var temporaryList2 = new List<CPAOffer>();
            foreach (var elem in resultList)
            {
                if (TitanFeatures.IsBobbyDonev)
                    temporaryList2.Add(elem);
                else if (IsDailyCategoryList && elem.IsDaily || !IsDailyCategoryList && !elem.IsDaily)
                    temporaryList2.Add(elem);                
            }
            resultList = temporaryList2;
        }        

        //Calculate active categories count (not filtering in any way)
        CategoriesCount = new Dictionary<int, int>();
        foreach (var offer in resultList)
        {
            if (CategoriesCount.ContainsKey(offer.IntCategory))
                CategoriesCount[offer.IntCategory]++;
            else
                CategoriesCount.Add(offer.IntCategory, 1);
        }

        //6. Paging
        int numberOfObjectsPerPage = Convert.ToInt32(PagingDropDownList.SelectedValue);
        int pageNumber = CurrentPageNumber;
        LastPageNumber = ((resultList.Count - 1) / numberOfObjectsPerPage);
        NextPageButton.Visible = CurrentPageNumber < LastPageNumber;

        return resultList
                .Skip(numberOfObjectsPerPage * pageNumber)
                .Take(numberOfObjectsPerPage)
                .ToList();

    }

    #region Sorting Comparers

    public int MoneyEarnedComparison(CPAOffer x, CPAOffer y)
    {
        if (x.BaseValue > y.BaseValue)
            return 1;
        else if (x.BaseValue == y.BaseValue)
            return 0;
        else
            return -1;
    }

    public int CompletedTimesComparison(CPAOffer x, CPAOffer y)
    {
        if (x.CompletedTimes > y.CompletedTimes)
            return 1;
        else if (x.CompletedTimes == y.CompletedTimes)
            return 0;
        else
            return -1;
    }

    public int DateAddedComparison(CPAOffer x, CPAOffer y)
    {
        if (x.DateAdded > y.DateAdded)
            return 1;
        else if (x.DateAdded == y.DateAdded)
            return 0;
        else
            return -1;
    }

    public int LastCreditedComparison(CPAOffer x, CPAOffer y)
    {
        if (x.LastCredited > y.LastCredited)
            return 1;
        else if (x.LastCredited == y.LastCredited)
            return 0;
        else
            return -1;
    }

    public int RatingComparison(CPAOffer x, CPAOffer y)
    {
        if (x.Rating > y.Rating)
            return 1;
        else if (x.Rating == y.Rating)
            return 0;
        else
            return -1;
    }

    public int LevelComparison(CPAOffer x, CPAOffer y)
    {
        if (Int32.Parse(x.OfferLevel) > Int32.Parse(y.OfferLevel))
            return 1;
        else if (x.OfferLevel == y.OfferLevel)
            return 1;
        else
            return -1;
    }

    public int DMoneyEarnedComparison(CPAOffer x, CPAOffer y)
    {
        if (x.BaseValue < y.BaseValue)
            return 1;
        else if (x.BaseValue == y.BaseValue)
            return 0;
        else
            return -1;
    }

    public int DCompletedTimesComparison(CPAOffer x, CPAOffer y)
    {
        if (x.CompletedTimes < y.CompletedTimes)
            return 1;
        else if (x.CompletedTimes == y.CompletedTimes)
            return 0;
        else
            return -1;
    }

    public int DDateAddedComparison(CPAOffer x, CPAOffer y)
    {
        if (x.DateAdded < y.DateAdded)
            return 1;
        else if (x.DateAdded == y.DateAdded)
            return 0;
        else
            return -1;
    }

    public int DLastCreditedComparison(CPAOffer x, CPAOffer y)
    {
        if (x.LastCredited < y.LastCredited)
            return 1;
        else if (x.LastCredited == y.LastCredited)
            return 0;
        else
            return -1;
    }

    public int DRatingComparison(CPAOffer x, CPAOffer y)
    {
        if (x.Rating < y.Rating)
            return 1;
        else if (x.Rating == y.Rating)
            return 0;
        else
            return -1;
    }

    #endregion Sorting Comparers

    public void UpperMenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        IsDailyCategoryList = Int32.Parse(TheButton.CommandArgument) == 1 ? true : false;

        //Change button style
        foreach (Button b in UpperMenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

        HidePanels();
        DecideAndUpdate();
        MakeCategoriesList();
        makeButtons();
    }

    public void DeviceTypeMenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        OffersDeviceType = (DeviceType)Int32.Parse(TheButton.CommandArgument);

        //Change button style
        foreach (Button b in DeviceTypeMenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

        HidePanels();
        DecideAndUpdate();
        MakeCategoriesList();
        makeButtons();
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
        {
            KeepOrder = false;
            Response.Redirect("cpaoffers.aspx?keeporder=0");
        }

        MenuMultiView.ActiveViewIndex = viewIndex;

        DecideAndUpdate(viewIndex);

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

        HidePanels();
    }

    public void HidePanels()
    {
        SuccMessagePanel.Visible = false;
        ErrorMessagePanel.Visible = false;
    }

    public void WriteSuccess(string message)
    {
        HidePanels();
        SuccMessagePanel.Visible = true;
        SuccMessage.Text = message;
    }

    public void WriteError(string message)
    {
        HidePanels();
        ErrorMessagePanel.Visible = true;
        ErrorMessage.Text = message;
    }

    private List<CPACategory> _accountStatusesNetwork;
    public List<CPACategory> AccountStatusesNetwork
    {
        get
        {
            if (_accountStatusesNetwork == null)
            {
                _accountStatusesNetwork = CPACategory.AllCategories;
            }
            return _accountStatusesNetwork;
        }
    }

    private Dictionary<int, int> CategoriesCount = new Dictionary<int, int>();
    
    public void MakeCategoriesList()
    {
        var list = new Dictionary<string, string>();
        var values = AccountStatusesNetwork;
        int howmanyall = 0;

        //Add 'All' at the beginning
        list.Add("ALL", "text");

        foreach (var elem in values)
        {
            int offersCount = CategoriesCount.ContainsKey(elem.Id) ? CategoriesCount[elem.Id] : 0;
            howmanyall += offersCount;
            list.Add((elem.Id).ToString(), CPAType.GetText(elem) + " [" + offersCount + "]");
        }
        list["ALL"] = "All [" + howmanyall + "]";

        CategoriesList.DataSource = list;
        CategoriesList.DataTextField = "Value";
        CategoriesList.DataValueField = "Key";
        CategoriesList.DataBind();
    }

    public void MakeSortingList()
    {
        SortBy1.Items[0].Text = L1.NONE;

        for (int i = 0; i < SortBy1.Items.Count; i++)
        {
            string value = SortBy1.Items[i].Value;
            string text = L1.NONE;

            if (value == "ARating")
                text = string.Format("{0}: {1}", L1.ASC, L1.OFFERRATING);
            else if (value == "ALast")
                text = string.Format("{0}: {1}", L1.ASC, L1.LASTCREDITED);
            else if (value == "ADate")
                text = string.Format("{0}: {1}", L1.ASC, L1.DATEADDED);
            else if (value == "ATimes")
                text = string.Format("{0}: {1}", L1.ASC, L1.COMPLETEDXTIMES.Replace("%n%", ""));
            else if (value == "AMoney")
                text = string.Format("{0}: {1}", L1.ASC, L1.AMOUNT);
            else if (value == "DRating")
                text = string.Format("{0}: {1}", L1.DESCR, L1.OFFERRATING);
            else if (value == "DLast")
                text = string.Format("{0}: {1}", L1.DESCR, L1.LASTCREDITED);
            else if (value == "DDate")
                text = string.Format("{0}: {1}", L1.DESCR, L1.DATEADDED);
            else if (value == "DTimes")
                text = string.Format("{0}: {1}", L1.DESCR, L1.COMPLETEDXTIMES.Replace("%n%", ""));
            else if (value == "DMoney")
                text = string.Format("{0}: {1}", L1.DESCR, L1.AMOUNT);

            SortBy1.Items[i].Text = text;
        }
    }

    protected void CategoriesList_SelectedIndexChanged(object sender, EventArgs e)
    {
        KeepOrder = false;
        DecideAndUpdate();
        MakeCategoriesList();
        makeButtons();
    }

    protected void SearchButton1_Click(object sender, EventArgs e)
    {
        KeepOrder = false;
        DecideAndUpdate();
        MakeCategoriesList();
        makeButtons();
    }

    protected void SortBy1_SelectedIndexChanged(object sender, EventArgs e)
    {
        KeepOrder = false;
        DecideAndUpdate();
        MakeCategoriesList();
        makeButtons();
    }

    protected void PagingDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        KeepOrder = false;
        DecideAndUpdate();
        MakeCategoriesList();
        makeButtons();
    }

    public void PreviousPageButton_Click(object sender, EventArgs e)
    {
        CurrentPageNumber--;
        KeepOrder = false;
        CurrentPageLiteral.Text = (CurrentPageNumber + 1).ToString();
        DecideAndUpdate();
    }

    public void NextPageButton_Click(object sender, EventArgs e)
    {
        CurrentPageNumber++;
        KeepOrder = false;
        CurrentPageLiteral.Text = (CurrentPageNumber + 1).ToString();
        DecideAndUpdate();
    }

    /// <summary>
    /// List of active offers, with their order and everything remembered
    /// </summary>
    private List<int> ActiveRememberedList
    {
        get
        {
            if (Session[GetSessionName()] == null)
            {
                var list = IsDailyCategoryList ? OM.ActiveDailyOffersForMember : OM.ActiveNotDailyOffersForMember;
                Session[GetSessionName()] = ParseCPAList(AdjustActiveOffers(list, true));
            }

            return (List<int>)Session[GetSessionName()];
        }
        set
        {
            Session[GetSessionName()] = value;
        }
    }

    private string GetSessionName()
    {
        return String.Format("ADRLFromCPAOff{0}", Convert.ToInt32(IsDailyCategoryList));
    }

    private List<int> ParseCPAList(List<CPAOffer> Input)
    {
        var list = new List<int>();
        for (int i = 0; i < Input.Count; ++i)
            list.Add(Input[i].Id);
        return list;
    }

    private List<CPAOffer> ParseIntList(List<int> Input)
    {
        var list = new List<CPAOffer>();
        for (int i = 0; i < Input.Count; ++i)
            list.Add(new CPAOffer(Input[i]));
        return list;
    }

    private string SearchBoxInput
    {
        get
        {
            if (Session["ARLFromCPAOff1"] == null)
                Session["ARLFromCPAOff1"] = "";

            return (string)Session["ARLFromCPAOff1"];
        }
        set
        {
            Session["ARLFromCPAOff1"] = value;
        }
    }

    private int CategoryIdInput
    {
        get
        {
            if (Session["ARLFromCPAOff12"] == null)
                Session["ARLFromCPAOff12"] = -1;

            return (int)Session["ARLFromCPAOff12"];
        }
        set
        {
            Session["ARLFromCPAOff12"] = value;
        }
    }

    private int SortIdInput
    {
        get
        {
            if (Session["ARLFromCPAOff123"] == null)
                Session["ARLFromCPAOff123"] = -1;

            return (int)Session["ARLFromCPAOff123"];
        }
        set
        {
            Session["ARLFromCPAOff123"] = value;
        }
    }

    private bool KeepOrder
    {
        get
        {
            if (Session["keeporder"] != null && (bool)Session["keeporder"])
                return true;
            return false;
        }
        set
        {
            Session["keeporder"] = value;
        }
    }

    private void DecideAndUpdate(int forceRefreshIndex = -1)
    {
        PreviousPageButton.Visible = CurrentPageNumber > 0;

        // 0 = Active, 1 = completed, 2 = denied, 3 = ignored, 4 = pending, 5 = reported, 6 = under
        List<CPAOffer> list = new List<CPAOffer>();
        int data = MenuMultiView.ActiveViewIndex;

        if (forceRefreshIndex != -1)
            data = forceRefreshIndex;

        switch (data)
        {
            case 0:
                if (KeepOrder)
                {
                    //We are back from 'Submit' 'Ignore' or 'Report'
                    //Lets keep the list the way it was
                    //Additional record is gone
                    list = ParseIntList(ActiveRememberedList);
                }
                else
                {

                    list = AdjustActiveOffers(OM.AllActiveOffersForMember, true);
                    ActiveRememberedList = ParseCPAList(list);
                    //Save Other
                    SearchBoxInput = SearchBox1.Text;
                    CategoryIdInput = CategoriesList.SelectedIndex;
                    SortIdInput = SortBy1.SelectedIndex;
                }

                OffersPanel0.Controls.Clear();
                foreach (var elem in list)
                    OffersPanel0.Controls.Add(elem.ToPanel(CurrentUser.Membership.CPAProfitPercent));
                break;

            case 1:
                foreach (var sub in OM.SubmissionsCompleted)
                {
                    CPAOffer off = sub.Offer;
                    off.Temp_Time = sub.DateCompleted;
                    off.Temp = sub.Id;

                    list.Add(off);
                }
                list = AdjustActiveOffers(list);
                OffersPanel1.Controls.Clear();
                foreach (var elem in list)
                    OffersPanel1.Controls.Add(elem.ToCompletedPanel(elem.Temp_Time, CurrentUser.Membership.CPAProfitPercent, elem.Temp));
                break;

            case 2:
                foreach (var sub in OM.SubmissionsDenied)
                {
                    CPAOffer off = sub.Offer;
                    off.Temp = sub.Id;

                    list.Add(off);
                }
                list = AdjustActiveOffers(list);
                OffersPanel2.Controls.Clear();
                foreach (var elem in list)
                    OffersPanel2.Controls.Add(elem.ToDeniedPanel(CurrentUser.Membership.CPAProfitPercent, elem.Temp));
                break;

            case 3:
                foreach (var sub in OM.SubmissionsIgnored)
                {
                    CPAOffer off = sub.Offer;
                    off.Temp = sub.Id;

                    list.Add(off);
                }
                list = AdjustActiveOffers(list);
                OffersPanel3.Controls.Clear();
                foreach (var elem in list)
                    OffersPanel3.Controls.Add(elem.ToIgnoredPanel(CurrentUser.Membership.CPAProfitPercent, elem.Temp, "ignored"));
                break;

            case 4:
                foreach (var sub in OM.SubmissionsPending)
                {
                    CPAOffer off = sub.Offer;
                    off.Temp_LoginID = sub.LoginID;
                    off.Temp_EmailID = sub.EmailID;
                    off.Temp = sub.Id;

                    list.Add(off);
                }
                list = AdjustActiveOffers(list);
                OffersPanel4.Controls.Clear();
                foreach (var elem in list)
                    OffersPanel4.Controls.Add(elem.ToPendingPanel(CurrentUser.Membership.CPAProfitPercent, elem.Temp_LoginID, elem.Temp_EmailID, elem.Temp, "pending"));
                break;

            case 5:
                foreach (var sub in OM.SubmissionsReported)
                {
                    CPAOffer off = sub.Offer;

                    var report = new CPAReport(Convert.ToInt32(sub.LoginID));

                    off.Temp_LoginID = report.ReportText;
                    off.Temp = sub.Id;

                    list.Add(off);
                }
                list = AdjustActiveOffers(list);
                OffersPanel5.Controls.Clear();
                foreach (var elem in list)
                    OffersPanel5.Controls.Add(elem.ToReportedPanel(CurrentUser.Membership.CPAProfitPercent, elem.Temp_LoginID, elem.Temp, "reported"));
                OffersPanel5.DataBind();
                break;

            case 6:
                foreach (var sub in OM.SubmissionsUnderReview)
                {
                    CPAOffer off = sub.Offer;
                    off.Temp = sub.Id;

                    list.Add(off);
                }
                list = AdjustActiveOffers(list);
                OffersPanel6.Controls.Clear();
                foreach (var elem in list)
                    OffersPanel6.Controls.Add(elem.ToUnderReviewPanel(CurrentUser.Membership.CPAProfitPercent, elem.Temp));
                break;
        }
    }

    private void makeButtons()
    {
        DeviceTypeSelectionPlaceHolder.Visible = AppSettings.CPAGPT.DeviceTypeDistinctionEnabled;

        DailyCategoriesButton.Text = string.Format("{0} ({1})", L1.DAILY, ActiveDailyOffersCount);
        OtherCategoriesButton.Text = string.Format("{0} ({1})", U6008.OTHER, ActiveNonDailyOffersCount);
        DesktopDeviceTypeButton.Text = string.Format("{0} ({1})", U6011.DESKTOP, ActiveDesktopOffersCount);
        MobileDeviceTypeButton.Text = string.Format("{0} ({1})", U6011.MOBILE, ActiveMobileOffersCount);

        if (TitanFeatures.IsBobbyDonev && AppSettings.TitanFeatures.OfferLevelsEnabled)
            Button1.Text = string.Format("{0} ({1})", L1.ACTIVE, ActiveDailyOffersCount + ActiveNonDailyOffersCount);
        else
        {
            int count = 0;
            if (AppSettings.CPAGPT.DailyNotDailyButtonsEnabled)
                count = IsDailyCategoryList ? ActiveDailyOffersCount : ActiveNonDailyOffersCount;
            else
                count = ActiveDailyOffersCount + ActiveNonDailyOffersCount;

            Button1.Text = string.Format("{0} ({1})", L1.ACTIVE, count);
        }

        Button2.Text = string.Format("{0} ({1})", L1.COMPLETED, OM.SubmissionsCompleted.Count);
        Button6.Text = string.Format("{0} ({1})", L1.DENIED, OM.SubmissionsDenied.Count);
        Button5.Text = string.Format("{0} ({1})", L1.IGNORED, OM.SubmissionsIgnored.Count);
        Button4.Text = string.Format("{0} ({1})", L1.PENDING, OM.SubmissionsPending.Count);
        Button3.Text = string.Format("{0} ({1})", L1.REPORTED, OM.SubmissionsReported.Count);
        Button7.Text = string.Format("{0} ({1})", L1.UNDERREVIEW, OM.SubmissionsUnderReview.Count);
    }
    #endregion Helpers
}