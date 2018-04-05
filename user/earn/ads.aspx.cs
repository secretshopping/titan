using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;
using Prem.PTC.Advertising;
using System.Reflection;
using Resources;

public partial class About : System.Web.UI.Page
{
    List<int> CreatedCustomCategories;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && AppSettings.TitanFeatures.EarnAdsEnabled
            && (!Member.IsLogged || Member.CurrentInCache.IsEarner));
        
        ExternalIFrameLabel.Text = AppSettings.PtcAdverts.IsExternalIFrameEnabled.ToString();
        
        searchPlaceHolder.Visible = AppSettings.PtcAdverts.CashLinkViewEnabled != AppSettings.PTCViewMode.CashLink;
        SortPlaceHolder.Visible = AppSettings.PtcAdverts.StarredAdsEnabled && AppSettings.PtcAdverts.FavoriteAdsEnabled && AppSettings.PtcAdverts.CashLinkViewEnabled != AppSettings.PTCViewMode.CashLink;
        favouriteAnhor.Visible = AppSettings.PtcAdverts.FavoriteAdsEnabled;

        if (TitanFeatures.IsRofriqueWorkMines)
        {
            GotoMineBTCPanel.Visible = true;
            ResetTimeLeftPlaceHolder.Visible = false;
            FilterSection.Visible = false;

            if (Member.CurrentInCache.AdsViewedCount >= Member.CurrentInCache.Membership.MaxDailyPtcClicks)
            {
                AdsLiteral.Visible = false;
                SmallAdsLiteral.Visible = false;

                AdditionalAdsInfoLabel.Text = "You have already reached you daily mining session limit. Buy more cloud processing power to continue mining or comeback tommorow.";
                AdditionalAdsInfoPlaceHolder.Visible = true;
            }
        }

        ExposureCategoriesPlaceHolder.Visible = AppSettings.PtcAdverts.ExposureCategoriesEnabled;

        if (Member.IsLogged && AppSettings.Points.LevelMembershipPolicyEnabled)
            BindToClicksToKeepLevel();
    }

    private void BindToClicksToKeepLevel()
    {
        Member user = Member.CurrentInCache;
        ClicksToKeepLevelPlaceHolder.Visible = true;
        string coloredAdsViewed;
        int adsViewedThisMonth = user.PtcSurfClicksThisMonth;
        int monthlyRequiredClicks = user.Membership.MinAdsWatchedMonthlyToKeepYourLevel;

        if (adsViewedThisMonth < monthlyRequiredClicks)
            coloredAdsViewed = string.Format("<span style={0}>{1}</span>", "'color: #cb6f1b; font-weight: bold;'", adsViewedThisMonth);
        else
            coloredAdsViewed = string.Format("<span style={0}>{1}</span>", "'color: #87a131; font-weight: bold;'", adsViewedThisMonth);

        ClicksToKeepLevelLiteral.Text = string.Format(U5007.CLICKSTOKEEPTHISLEVEL, coloredAdsViewed, monthlyRequiredClicks);
    }

    protected void AdRefreshUpdatePanel_Load(object sender, EventArgs e)
    {
        AdPacksRequiredPlaceHolder.Visible = false;

        if (AppSettings.PtcAdverts.CashLinkViewEnabled == AppSettings.PTCViewMode.PTC)  
            SmallAdsLiteral.Controls.Clear();           
        else if(AppSettings.PtcAdverts.CashLinkViewEnabled == AppSettings.PTCViewMode.CashLink)
            FilterSection.Visible = false;

        if (Member.IsLogged)
            if (AppSettings.RevShare.RequireAPToViewCashLinks && !Member.CurrentInCache.HasEverBoughtAdPacks && AppSettings.PtcAdverts.CashLinkCrediterEnabled)
            {
                AdPacksRequiredPlaceHolder.Visible = true;
                AdsLiteral.Visible = false;
                return;
            }

        AdsLiteral.Controls.Clear();
        LoadAds();
    }

    protected UserControl GetAdHTML(PtcAdvert ad, Member user, string color = "")
    {
        bool IsActive = true;

        if (Member.IsLogged)
            IsActive = !user.AdsViewed.Contains(ad.Id);

        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/PtcAdvert.ascx");
        var parsedControl = objControl as IPtcAdvertObjectControl;
        parsedControl.Object = ad;

        PropertyInfo myProp = parsedControl.GetType().GetProperty("IsActive");
        myProp.SetValue(parsedControl, IsActive, null);

        parsedControl.DataBind();

        return objControl;
    }

    protected void LoadAds()
    {
        Form.Action = Request.RawUrl;

        List<PtcAdvert> AvailableAdList;
        CreatedCustomCategories = new List<int>();

        Member user = null;

        if (Member.IsLogged)
        {
            user = Member.Current;
            int temporaryAdvertId = 0;
            if (AppSettings.PtcAdverts.BlockPtcAdvertsAfterMissingPointer
                && Request.Params.Get("__EVENTARGUMENT5") != null && Int32.TryParse(Request.Params.Get("__EVENTARGUMENT5"), out temporaryAdvertId))
            {
                // User missed the anti-robot button 3 times
                // Blocking this particualr ad for him
                List<int> av = user.AdsViewed;
                av.Add(temporaryAdvertId); // Blocking = adding to watched
                user.AdsViewed = av;
                user.Save();

                NotificationManager.RefreshWithMember(NotificationType.NewAds, user);
            }

            AvailableAdList = PtcAdvert.GetActiveAdsForUser(user);
        }
        else
            AvailableAdList = PtcAdvert.GetAllActiveAds();

        if (AvailableAdList.Count == 0)
        {
            MenuMultiView.Visible = false;
            NoPTCpanelWrapper.Visible = true;
            NoPTCpanel.Visible = true;
            NoPTCpanel.Text = U6003.NOPTC;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(Request.Params.Get("__EVENTARGUMENT")))
            {
                string agrument = Request.Params.Get("__EVENTARGUMENT");
                if (PTCSercurityManager.IsPTCAdvertFound(agrument, AvailableAdList))
                    PTCSercurityManager.Release();
            }

            //Sort by favorite?
            if (Member.IsLogged)
            {
                if (SortBy1.SelectedValue == "Favorite")
                {
                    var favorites = FavoriteAd.GetUserFavorites(Member.CurrentId, FavoriteAdType.PTC);
                    AvailableAdList = AvailableAdList.OrderByDescending(i => favorites.Contains(i.Id)).ToList();
                }
                else if (SortBy1.SelectedValue == "Voted")
                {
                    var favorites = FavoriteAd.GetUserFavorites(Member.CurrentId, FavoriteAdType.PTC);
                    AvailableAdList = AvailableAdList.OrderByDescending(i => i.CaptchaYesAnswers - i.CaptchaNoAnswers).ToList();
                }
            }

            //Display proper ads to the Member
            try
            {
                //Lets get the proper ads to the member
                foreach (PtcAdvert Ad in AvailableAdList)
                {
                    bool IsSearch = !string.IsNullOrWhiteSpace(SearchTextBox.Text);

                    if (!IsSearch || (IsSearch && IsSearchRequirementsMeet(Ad, SearchTextBox.Text)))
                    {
                        if (AppSettings.PtcAdverts.PTCCategoryPolicy == AppSettings.PTCCategoryPolicy.Custom)
                        {
                            //Display Custom Categories
                            CustomCategoriesPlaceHolder.Visible = true;
                            if (!CreatedCustomCategories.Contains(Ad.CategoryId))
                                CreatedCustomCategories.Add(Ad.CategoryId);

                            CustomCategoriesPlaceHolder.Controls.Add(GetAdHTML(Ad, user));
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(Ad.Description) && Ad.IsStarredAd == false)
                                SmallAdsLiteral.Controls.Add(GetAdHTML(Ad, user, ""));
                            else
                                AdsLiteral.Controls.Add(GetAdHTML(Ad, user, ""));
                        }
                    }
                }
                if (AppSettings.PtcAdverts.PTCCategoryPolicy == AppSettings.PTCCategoryPolicy.Custom)                
                    foreach (var categoryId in CreatedCustomCategories)                    
                        LoadCustomCategoryButton(categoryId);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }

    protected UserControl GetCashLink(PtcAdvert ad)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/PtcAdvert.ascx");
        var parsedControl = objControl as IPtcAdvertObjectControl;
        parsedControl.Object = ad;
        parsedControl.DataBind();

        return objControl;
    }

    private void LoadCustomCategoryButton(int categoryId)
    {
        PtcAdvertCategory category = new PtcAdvertCategory(categoryId);
        LinkButton btn = new LinkButton();
        btn.ID = "CustomCategoryButton" + categoryId;
        btn.CssClass = "btn btn-default btn-xs";
        btn.Attributes.Add("data-option-value", ".gallery-group-" + categoryId);
        btn.Text = category.Name;
        btn.ClientIDMode = ClientIDMode.Static;
        btn.OnClientClick = "return false;";
        CategoryButtonsPlaceHolder.Controls.Add(btn);
    }

    protected Panel GetCustomCategoryPanel(int categoryId, bool isFromFilter = false)
    {
        if (CreatedCustomCategories.Contains(categoryId))
        {
            return (Panel)CustomCategoriesPlaceHolder.FindControl("CustomCategoriesAdsPlanel" + categoryId);
        }
        else if (!isFromFilter)
        {
            CreatedCustomCategories.Add(categoryId);
            PtcAdvertCategory category = new PtcAdvertCategory(categoryId);
            Panel panel = new Panel();
            panel.ID = "CustomCategoriesAdsPlanel" + categoryId;
            panel.CssClass = "gallery row";
            panel.ClientIDMode = ClientIDMode.Static;
            Literal literal = new Literal();
            PlaceHolder adsplaceholder = new PlaceHolder();
            adsplaceholder.ID = "CustomCategoriesAdsPlaceHolder" + categoryId;
            literal.Text = "<h3>" + category.Name + "</h3><br/>";
            panel.Attributes.Add("style", "overflow:auto");
            panel.Controls.Add(literal);
            panel.Controls.Add(adsplaceholder);
            CustomCategoriesPlaceHolder.Controls.Add(panel);
            return panel;
        }
        return null;
    }

    protected void SortBy1_Init(object sender, EventArgs e)
    {
        if (AppSettings.PtcAdverts.StarredAdsEnabled)
            SortBy1.Items.Add(new ListItem(U5006.SORTBY1, "Starred"));

        if (AppSettings.PtcAdverts.FavoriteAdsEnabled)
            SortBy1.Items.Add(new ListItem(U5006.SORTBY2, "Favorite"));

        if (AppSettings.PtcAdverts.FeedbackCaptchaEnabled)
            SortBy1.Items.Add(new ListItem(U5008.MOSTVOTED, "Voted"));

        if (SortBy1.Items.Count == 0)
            sortbydiv.Visible = false;

    }


    protected void ExpiredFavoriteGridView_DataSource_Init(object sender, EventArgs e)
    {
        string favoriteIds = FavoriteAd.GetUserFavoritesCommaDelimited(Member.CurrentId, FavoriteAdType.PTC);
        ExpiredFavoriteGridView_DataSource.SelectCommand = string.Format("SELECT PtcAdvertId, Title, Description, TargetURL FROM PtcAdverts " +
            "WHERE Status != " + (int)AdvertStatus.Active + " AND PtcAdvertId IN ({0})", favoriteIds);
    }

    protected void ExpiredFavoriteGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Text = String.Format("<a href='{0}' target='_blank'>{0}</a>", e.Row.Cells[2].Text);
        }
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {

    }

    protected bool IsSearchRequirementsMeet(PtcAdvert ad, string text)
    {
        return ad.Title.ToLower().Contains(text.ToLower()) || (!String.IsNullOrEmpty(ad.Description) && ad.Description.ToLower().Contains("#" + text.ToLower()));
    }
}
