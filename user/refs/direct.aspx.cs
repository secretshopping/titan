using System;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using Prem.PTC.Referrals;
using ExtensionMethods;
using System.Text;
using Prem.PTC.Memberships;
using Titan.Cryptocurrencies;
using System.Web.UI.HtmlControls;

public partial class DirectReferrals : System.Web.UI.Page
{
    public string jsSelectAllCode;
    Member User;
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ReferralsDirectEnabled);

        if (!Page.IsPostBack)
        {
            DataBind();
            if (TitanFeatures.IsTrafficThunder)
            {
                AdditionalInfoPlaceHolder.Visible = false;
                DirectRefsGridView.AllowSorting = false;
            }    
        }

        //For Chrome compatibility
        Context.Response.AppendHeader("X-XSS-Protection", "0");
    }

    public override void DataBind()
    {
        User = Member.CurrentInCache;
        //Lang
        DirectRefsGridView.EmptyDataText = L1.NODIRECTREFERRALS;
        Button1.Text = L1.STATISTICS;
        Button2.Visible = AppSettings.DirectReferrals.DirectReferralBuyingEnabled;
        Button2.Text = L1.BUY;
        UserName.Text = User.Name;
        BuyDirectRefBackButton.Text = L1.BUY;
        ReferrerInfoLiteral.Text = string.Format("{0}: {1}", U6010.YOURREFFERER, User.ReferrerId != -1 ? HtmlCreator.CreateAvatarPlusUsername(new Member(User.ReferrerId)) : "-");
        RegisterStartupGridViewCode(DirectRefsGridView, SelectedPanel, ref jsSelectAllCode);

        //Check if delete is enabled
        if (!AppSettings.DirectReferrals.IsDeletingEnabled)
            SelectedPanel.Visible = false;

        //Hide if no data
        if (DirectRefsGridView.Rows.Count == 0 || !AppSettings.TitanFeatures.EarnAdsEnabled)
            ChangeAVGPanel.Visible = false;

        //Update notifications
        if (NotificationManager.Get(NotificationType.NewDirectReferrals) != 0)
        {
            NotificationManager.SpotAllDirectReferrals();
            NotificationManager.Refresh(NotificationType.NewDirectReferrals);
        }

        //Warning display
        if (!User.HasClickedEnoughToProfitFromReferrals() && DirectRefsGridView.Rows.Count != 0 && !AppSettings.Site.PureGPTMode && AppSettings.TitanFeatures.EarnAdsEnabled)
        {
            WarningPanel.Visible = true;
            WarningLiteral.Text = L1.REFNOPROFITS;
            WarningLiteral.Text = WarningLiteral.Text.Replace("%n%", AppSettings.Referrals.MinDailyClicksToEarnFromRefs.ToString());
        }

        base.DataBind();
    }

    public static void RegisterStartupGridViewCode(GridView theView, Panel selPanel, ref string jsCode)
    {
        // Dont display selected if no rows
        if (theView.Rows.Count == 0)
            selPanel.Visible = false;

        //Register checkbox JS events
        for (int i = 0; i < theView.Rows.Count; i++)
        {
            GridViewRow row = theView.Rows[i];
            var chbox = ((HtmlInputCheckBox)row.FindControl("chkSelect"));
            jsCode += "getElementById('" + chbox.ClientID + "').checked = getElementById('checkAll').checked;";
        }
        jsCode += "hideIfUnchecked('" + selPanel.ClientID + "');uncheckInvisible();";
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("~/user/refs/direct.aspx");

        if (viewIndex == 1)
            AccessManager.RedirectIfDisabled(AppSettings.DirectReferrals.DirectReferralBuyingEnabled);

        MenuMultiView.ActiveViewIndex = viewIndex;
        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

        EPanel.Visible = SPanel.Visible = false;
        EText.Text = SText.Text = string.Empty;
    }
    protected void DirectRefsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            if (TitanFeatures.IsTrafficThunder)
            {
                e.Row.Cells[17].Text = "Comission";
                e.Row.Cells[20].Visible = false;

                for (int i = 0; i < 27; i++)
                    e.Row.Cells[i].Style.Add("color", "#f15f79");
            }              
            else
                e.Row.Cells[17].Text = U4000.MONEY;

            if (!AppSettings.TitanFeatures.UpgradeEnabled)
                e.Row.Cells[21].Visible = false;

            if(!AppSettings.Ethereum.ERC20TokenEnabled)
                e.Row.Cells[18].Visible = false;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Text = "";

            var chbox = ((HtmlInputCheckBox)e.Row.FindControl("chkSelect"));
            chbox.Attributes.Add("onclick", SelectedPanel.ClientID + ".style.display = 'block';hideIfUnchecked('" + SelectedPanel.ClientID + "');");

            //Create an instance of the datarow
            var userName = e.Row.Cells[19].Text;
            Member User = new Member(userName);

            //Lets generate avatar + colored login OR encrypt 
            if (AppSettings.DirectReferrals.AreUsernamesEncrypted)            
                e.Row.Cells[2].Text = MemberAuthenticationService.ComputeHash(userName).Substring(0, 16);            
            else            
                e.Row.Cells[2].Text = HtmlCreator.CreateAvatarPlusUsername(User);            

            if (TitanFeatures.isAri)            
                e.Row.Cells[6].Text = User.ActiveAdPacks.ToString();            

            //Count the AVG [] Clicks[] RegDate[]
            int clicks = Int32.Parse(e.Row.Cells[10].Text);
            DateTime registered = Convert.ToDateTime(e.Row.Cells[7].Text);

            if (registered < AppSettings.ServerTime.AddYears(-100))
                e.Row.Cells[7].Text = Member.CurrentInCache.Registered.ToShortDateString();

            e.Row.Cells[11].Text = HtmlCreator.GetColoredAVGValue(RentReferralsSystem.GetAVG(clicks, registered));

            //Last activity[5]
            string lastActivityString;
            if (string.IsNullOrEmpty(e.Row.Cells[8].Text) || e.Row.Cells[8].Text == "&nbsp;" || Convert.ToDateTime(e.Row.Cells[8].Text) < DateTime.Now.AddYears(-100))
                lastActivityString = "<i>" + L1.NEVER + "</i>";
            else
            {
                DateTime lastActivity = Convert.ToDateTime(e.Row.Cells[8].Text);
                int days = (int)Math.Ceiling(DateTime.Now.Subtract(lastActivity).TotalDays);
                switch (days)
                {
                    case 1:
                        lastActivityString = L1.TODAY;
                        break;
                    case 2:
                        lastActivityString = L1.YESTERDAY;
                        break;
                    default:
                        lastActivityString = days.ToString() + " " + L1.DAYSAGO;
                        break;
                }
            }

            e.Row.Cells[8].Text = lastActivityString;
            e.Row.Cells[15].Text = new Money(Convert.ToDecimal(e.Row.Cells[15].Text)).ToString();
            e.Row.Cells[17].Text = Money.Parse(e.Row.Cells[17].Text).ToString();

            if (AppSettings.Ethereum.ERC20TokenEnabled)
                e.Row.Cells[18].Text = (new CryptocurrencyMoney(CryptocurrencyType.ERC20Token, Decimal.Parse(e.Row.Cells[18].Text))).ToString();
            else
                e.Row.Cells[18].Visible = false;

            if (AppSettings.DirectReferrals.DirectReferralBuyingEnabled && User.ReferrerExpirationDate.HasValue)
            {
                e.Row.Cells[20].Text = GetTimeToExpiration(User.ReferrerExpirationDate.Value);
            }
            else if (AppSettings.DirectReferrals.DirectReferralExpirationEnabled)
            {
                var expirationDate = Convert.ToDateTime(e.Row.Cells[20].Text).AddDays(AppSettings.DirectReferrals.DirectReferralExpiration);
                e.Row.Cells[20].Text = GetTimeToExpiration(expirationDate);
            }
            else
            {
                e.Row.Cells[20].Text = "-";
            }

            e.Row.Cells[21].Text = new Membership(Convert.ToInt32(e.Row.Cells[21].Text)).Name;

            if (!AppSettings.TitanFeatures.UpgradeEnabled)
                e.Row.Cells[21].Visible = false;

            if(TitanFeatures.IsTrafficThunder)
                e.Row.Cells[20].Visible = false;

            if (TitanFeatures.IsSardaryify)
            {
                // Get id of current user 
                // Id is in the last column
                var id = Convert.ToInt32(e.Row.Cells[26].Text);

                e.Row.Cells[23].Text = AdPackManager.GetNumberOfUsersAdPacks(id, true).ToString();

                e.Row.Cells[24].Text = Member.GetDirectReferralsCount(id).ToString();

                var amount = TableHelper.SelectScalar(string.Format(@"
                SELECT SUM(Price)
                FROM Adpacks
                AS A
                JOIN AdPackTypes
                AS B
                ON A.AdPackTypeId = B.Id
                WHERE A.UserId = {0}
                AND BalanceBoughtType = {1}
                ", id, (int)PurchaseBalances.Cash)).ToString();

                Money amountInMoney;

                if (!string.IsNullOrWhiteSpace(amount))
                    amountInMoney = Money.Parse(amount);
                else
                    amountInMoney = new Money(0);

                e.Row.Cells[25].Text = amountInMoney.ToString();
            }
        }
    }

    private string GetTimeToExpiration(DateTime expirationDate)
    {
        var actualExp = (AppSettings.ServerTime + AppSettings.Site.TimeToNextCRONRun).AddDays((expirationDate.Date - AppSettings.ServerTime.Date).TotalDays);
        var hoursToExpiration = (actualExp - AppSettings.ServerTime).TotalHours;

        if (hoursToExpiration <= 72 && hoursToExpiration > 0)
            return Math.Round(hoursToExpiration) + "h";
        else if (hoursToExpiration == 0)
            return Math.Round((actualExp - AppSettings.ServerTime).TotalMinutes) + "min";
        else
            return actualExp.ToString("d");
    }

    private void HideAppropriateColumns()
    {    
        if (!AppSettings.DirectReferrals.IsDeletingEnabled)
        {
            DirectRefsGridView.Columns[1].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[1].ItemStyle.CssClass = "displaynone";
        }
        if (!AppSettings.Authentication.DetailedRegisterFields || !AppSettings.DirectReferrals.ShowDirectReferralsFullName)
        {
            DirectRefsGridView.Columns[3].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[3].ItemStyle.CssClass = "displaynone";
        }
        if (!AppSettings.DirectReferrals.ShowDirectReferralsPhoneNumber)
        {
            DirectRefsGridView.Columns[4].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[4].ItemStyle.CssClass = "displaynone";
        }
        if (!AppSettings.TitanFeatures.EarnAdsEnabled)
        {
            DirectRefsGridView.Columns[10].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[10].ItemStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[11].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[11].ItemStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[12].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[12].ItemStyle.CssClass = "displaynone";
        }
        if (!AppSettings.TitanFeatures.AdvertAdPacksEnabled)
        {
            DirectRefsGridView.Columns[15].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[15].ItemStyle.CssClass = "displaynone";
        }
        if (!AppSettings.Points.PointsEnabled)
        {
            DirectRefsGridView.Columns[16].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[16].ItemStyle.CssClass = "displaynone";
        }
        if (!AppSettings.DirectReferrals.ShowDirectReferralEmail)
        {
            DirectRefsGridView.Columns[5].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[5].ItemStyle.CssClass = "displaynone";
        }
        if (TitanFeatures.isAri)
        {
            DirectRefsGridView.Columns[15].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[15].ItemStyle.CssClass = "displaynone";
        }
        if (!AppSettings.DirectReferrals.ShowDirectReferralsStatus)
        {
            DirectRefsGridView.Columns[22].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[22].ItemStyle.CssClass = "displaynone";
        }
        if (TitanFeatures.IsSardaryify)
        {
            DirectRefsGridView.Columns[23].HeaderStyle.CssClass = "";
            DirectRefsGridView.Columns[23].ItemStyle.CssClass = "";

            DirectRefsGridView.Columns[24].HeaderStyle.CssClass = "";
            DirectRefsGridView.Columns[24].ItemStyle.CssClass = "";

            DirectRefsGridView.Columns[25].HeaderStyle.CssClass = "";
            DirectRefsGridView.Columns[25].ItemStyle.CssClass = "";
        }
    }

    protected void DirectRefsGridView_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        //Disable the currently open row
        if (DirectRefsGridView.SelectedIndex != -1)
        {
            GridViewRow row = DirectRefsGridView.SelectedRow;
            row.Attributes.Remove("height");
            DirectRefsGridView.Columns[6].HeaderStyle.CssClass = "";
            DirectRefsGridView.Columns[7].HeaderStyle.CssClass = "";
            DirectRefsGridView.Columns[8].HeaderStyle.CssClass = "";
            DirectRefsGridView.Columns[6].ItemStyle.CssClass = "";
            DirectRefsGridView.Columns[7].ItemStyle.CssClass = "";
            DirectRefsGridView.Columns[8].ItemStyle.CssClass = "";

            DirectRefsGridView.Columns[9].HeaderStyle.CssClass = "displaynone";
            DirectRefsGridView.Columns[9].ItemStyle.CssClass = "displaynone";
        }

        //New row (color and change height)
        GridViewRow newRow = DirectRefsGridView.Rows[e.NewSelectedIndex];
        newRow.Attributes.Add("height", "150px");

        //Lets delete bad columns and make visible proper ones [2][3][4] - visible false
        DirectRefsGridView.Columns[6].HeaderStyle.CssClass = "displaynone";
        DirectRefsGridView.Columns[7].HeaderStyle.CssClass = "displaynone";
        DirectRefsGridView.Columns[8].HeaderStyle.CssClass = "displaynone";
        DirectRefsGridView.Columns[6].ItemStyle.CssClass = "displaynone";
        DirectRefsGridView.Columns[7].ItemStyle.CssClass = "displaynone";
        DirectRefsGridView.Columns[8].ItemStyle.CssClass = "displaynone";

        DirectRefsGridView.Columns[9].HeaderStyle.CssClass = "";
        DirectRefsGridView.Columns[9].ItemStyle.CssClass = "";
    }

    protected void ImageButton1_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < DirectRefsGridView.Rows.Count; i++)
        {
            GridViewRow row = DirectRefsGridView.Rows[i];
            var chb = ((HtmlInputCheckBox)row.FindControl("chkSelect"));
            bool isChecked = chb.Checked;

            if (isChecked)
            {
                string userName = row.Cells[19].Text.Trim();
                Member User = new Member(userName);
                User.RemoveReferer();
                User.Save();
            }
        }
        Response.Redirect("direct.aspx");
    }


    protected void DirectRefsGridView_DataBound(object sender, EventArgs e)
    {
        if (TitanFeatures.isAri)
        {
            DirectRefsGridView.Columns[6].HeaderText = "Active AdPacks";
        }

        HideAppropriateColumns();
        ChangeColumnNames();
    }

    protected void ChangeColumnNames()
    {
        DirectRefsGridView.Columns[3].HeaderText = L1.NAME;
        DirectRefsGridView.Columns[4].HeaderText = U4200.PHONE;

        DirectRefsGridView.Columns[10].HeaderText = U6003.PTC + " " + L1.CLICKS;
        DirectRefsGridView.Columns[11].HeaderText = U6003.PTC + " AVG";

        DirectRefsGridView.Columns[15].HeaderText = AppSettings.RevShare.AdPack.AdPackNamePlural;
        DirectRefsGridView.Columns[16].HeaderText = AppSettings.PointsName;

        if (AppSettings.Points.LevelMembershipPolicyEnabled)
            DirectRefsGridView.Columns[21].HeaderText = U5006.LEVEL;

        DirectRefsGridView.Columns[22].HeaderText = L1.STATUS;

        DirectRefsGridView.Columns[23].HeaderText = string.Format("{0} {1}", L1.ACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural);
        DirectRefsGridView.Columns[24].HeaderText = L1.REFERRALS;
        DirectRefsGridView.Columns[25].HeaderText = L1.AMOUNT;
    }

    protected void DirectRefsGridView_DataSource_Init(object sender, EventArgs e)
    {
        DirectRefsGridView_DataSource.SelectCommand = string.Format("SELECT * FROM Users u WHERE ReferrerId = '{0}'", Member.CurrentId);
    }

    protected void BuyDirectRefBackButton_Click(object sender, EventArgs e)
    {
        try
        {
            EPanel.Visible = SPanel.Visible = false;
            EText.Text = SText.Text = string.Empty;

            DirectReferralPack pack = new DirectReferralPack(Convert.ToInt32(DirectRefPackDDL.SelectedValue));

            DirectReferralPackManager.BuyPack(pack, Member.Current, TargetBalanceRadioButtonList.TargetBalance);
            SPanel.Visible = true;
            SText.Text = string.Format(U5007.BUYDIRECTSUCCESS, pack.NumberOfRefs, pack.Days);
            View2_Activate(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                EPanel.Visible = true;
                EText.Text = ex.Message;
            }
        }
    }

    protected void View2_Activate(object sender, EventArgs e)
    {
        var packs = DirectReferralPackManager.GetAvailablePacks();

        DirectRefPackDDL.Items.Clear();

        for (int i = 0; i < packs.Count; i++)
        {
            string itemValue = packs[i].Id.ToString();
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(U5007.REFPACKDDL, packs[i].NumberOfRefs, AppSettings.ServerTime.AddDays(packs[i].Days).ToShortDateDBString(), packs[i].Price);
            if (AppSettings.DirectReferrals.DirectReferralMembershipPacksEnabled)
            {
                sb.Append(" (" + L1.MEMBERSHIP + ": ");
                if (packs[i].MembershipId == 0)
                    sb.Append(U4200.ALL + ")");
                else
                    sb.Append(Membership.SelectName(packs[i].MembershipId) + ")");
            }
                
                            
            string itemString = sb.ToString();

            ListItem item = new ListItem(itemString, itemValue);
            DirectRefPackDDL.Items.Insert(i, item);
            DirectRefPackDDL.SelectedIndex = 0;

        }

        if (packs.Count <= 0)
        {
            BuyDirectRefPackPlaceHolder.Visible = false;
            NoDirectRefPacksPlaceHolder.Visible = true;
            NoDirectRefPacksLiteral.Text = U5007.REFPACKSUNAVAILABLE;
        }
        ReferralsCount2.DataBind();
    }
}
