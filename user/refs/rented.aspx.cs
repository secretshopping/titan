using System;
using System.Linq;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using Resources;
using System.Web.UI.HtmlControls;

public partial class About : System.Web.UI.Page
{
    public string jsSelectAllCode;
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ReferralsRentedEnabled);

        Member User = Member.Current;
        RentReferralsSystem rrs;

        //For Chrome compatibility
        Context.Response.AppendHeader("X-XSS-Protection", "0");

        //Lets check if user clicked the pack and want to make a purchase
        if (Request.Params.Get("__PACKID") != null && IsParameterANumber() && !AppSettings.Site.TrafficExchangeMod)
        {
            SuccMessagePanel.Visible = false;
            ErrorMessagePanel.Visible = false;

            int packID = Int32.Parse(Request.Params.Get("__PACKID"));
            rrs = new RentReferralsSystem(User.Name, User.Membership);

            try
            {
                AppSettings.DemoCheck();

                ReferralPackage pack = new ReferralPackage(packID);

                // Check if all valid
                HasValidRequirements(User, pack, rrs, true);

                //Purchase the pack and save the user
                rrs.RentReferrals(pack.RefsInPackage); //throws MsgException if no ref available

                Money totalPrice = CalculatePackCost(User, pack);
                User.SubtractFromPurchaseBalance(totalPrice, "Ref: rent");
                User.LastRentDate = DateTime.Now;

                //Check if achievement should be applied
                rrs = new RentReferralsSystem(User.Name, User.Membership);
                User.TryToAddAchievements(
                        Prem.PTC.Achievements.Achievement.GetProperAchievements(
                        Prem.PTC.Achievements.AchievementType.AfterHavingRentedReferrals, rrs.GetUserRentedReferralsCount()));

                User.Save();

                // 4. Write success info
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = U3501.REFSUCC;

                //Ad history entry
                History.AddPurchase(User.Name, totalPrice, pack.RefsInPackage.ToString() + " rented referrals");

            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
        }

        //Lang
        DirectRefsGridView.EmptyDataText = L1.NORENTEDREFERRALS;
        LangAdder.Add(Button1, L1.MANAGE);
        LangAdder.Add(Button2, L1.RENTNEW);
        UserName.Text = Member.CurrentName;
        RemoveButton.Text = L1.REMOVE;
        RecycleButton.Text = string.Format("{0} ({1} {2})", L1.RECYCLE, User.Membership.RentedReferralRecycleCost, U6011.PERONE);
        AutopayAllOnButton.Text = AutopayOnButton.Text = U6004.AUTOPAYON;
        AutopayAllOffButton.Text = AutopayOffButton.Text = U6004.AUTOPAYOFF;
        RenewButton.Text = L1.RENEW;
        //Data
        rrs = new RentReferralsSystem(User.Name, User.Membership);
        RefLimit.Text = User.Membership.RentedReferralsLimit.ToString();
        MaxRefPack.Text = User.Membership.MaxRefPackageCount.ToString();
        RefCount.Text = rrs.GetUserRentedReferralsCount().ToString();
        RentalTimeLabel.Text = AppSettings.RentedReferrals.CanBeRentedFor.Days.ToString();
        RefGauranteeLabel.Text = (AppSettings.RentedReferrals.MinLastClickingActivity == TimeSpan.MinValue) ?
            L1.NONE : L1.CLICKEDINLAST + " " + AppSettings.RentedReferrals.MinLastClickingActivity.Days.ToString() + " " + L1.DAYS;
        MinClicksLabel.Text = AppSettings.Referrals.MinTotalClicksToRentRefs.ToString();
        LastRentedLiteral.Text = (User.LastRentDate == null) ? L1.NEVER : User.LastRentDate.ToString();
        MinRentInterval.Text = User.Membership.MinRentingInterval.Days.ToString();
        TotalClicksLiteral.Text = User.TotalClicks.ToString();

        //Autopay available? 
        AutopayOnButton.Visible = AutopayOffButton.Visible =
            User.Membership.CanAutoPay && AppSettings.Referrals.RentedRefAutopayPolicy == AppSettings.Referrals.AutopayPolicy.UserChooses;

        AutopayAllPlaceHolder.Visible =
            User.Membership.CanAutoPay && AppSettings.Referrals.RentedRefAutopayPolicy == AppSettings.Referrals.AutopayPolicy.AllReferrals;

        //Generate pack images
        RefBoxesLiteral.Text = string.Empty;
        var packList = TableHelper.SelectAllRows<ReferralPackage>();
        packList.Sort(Comparison);

        for (int i = 0; i < packList.Count; ++i)
        {
            bool isActive = HasValidRequirements(User, packList[i], rrs, false);
            Money packPrice = CalculatePackCost(User, packList[i]);

            RefBoxesLiteral.Text += HtmlCreator.GenerateRentRefBox(packList[i].RefsInPackage, packPrice, isActive, packList[i].Id);
        }

        if (AppSettings.Referrals.Renting == AppSettings.Referrals.RentingOption.DirectReferrals)
        {
            InfoPlaceHolder.Visible = false;
            Button1.Visible = false;
            Button2.CssClass = "ViewSelected";
            MenuMultiView.ActiveViewIndex = 1;
        }

        //Warning displays
        if (!User.HasClickedEnoughToProfitFromReferrals() && DirectRefsGridView.Rows.Count != 0)
        {
            WarningPanel.Visible = true;
            WarningLiteral.Text = L1.REFNOPROFITS;
            WarningLiteral.Text = WarningLiteral.Text.Replace("%n%", AppSettings.Referrals.MinDailyClicksToEarnFromRefs.ToString());
        }

        RemoveButton.Visible = AppSettings.RentedReferrals.IsDeletingEnabled;
        RecycleButton.Visible = AppSettings.RentedReferrals.IsRecyclingEnabled;

        RegisterStartupGridViewCode(DirectRefsGridView, SelectedPanel, ref jsSelectAllCode);
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
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("rented.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

    }

    private bool HasValidRequirements(Member User, ReferralPackage pack, RentReferralsSystem rrs, bool throwsMsgException)
    {
        bool isValid = true;
        try
        {
            //Limit
            if (AppSettings.Referrals.Renting != AppSettings.Referrals.RentingOption.DirectReferrals &&
                (pack.RefsInPackage + rrs.GetUserRentedReferralsCount() > User.Membership.RentedReferralsLimit))
                throw new MsgException(L1.ER_RENT_LIMIT);

            if (AppSettings.Referrals.Renting == AppSettings.Referrals.RentingOption.DirectReferrals &&
                (pack.RefsInPackage + User.GetDirectReferralsCount() > User.Membership.DirectReferralsLimit))
                throw new MsgException(L1.ER_RENT_LIMIT);

            //Min clicks
            if (AppSettings.Referrals.MinTotalClicksToRentRefs > User.TotalClicks)
                throw new MsgException(L1.ER_RENT_CLICKS);

            //Renting interval
            if (User.LastRentDate != null && ((DateTime)User.LastRentDate).AddDays(User.Membership.MinRentingInterval.Days) > DateTime.Now)
                throw new MsgException(L1.ER_RENT_INTERVAL);

            //Lets validate the money
            Money totalCost = CalculatePackCost(User, pack);
            if (totalCost > User.PurchaseBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            //Max ref package
            if (pack.RefsInPackage > User.Membership.MaxRefPackageCount)
                throw new MsgException(U2502.ER_EXCEEDPACKAGE);
        }
        catch (MsgException ex)
        {
            isValid = false;
            if (throwsMsgException)
                throw ex;
        }
        return isValid;
    }

    public int Comparison(ReferralPackage x, ReferralPackage y)
    {
        if (x.RefsInPackage > y.RefsInPackage)
            return 1;
        else if (x.RefsInPackage == y.RefsInPackage)
            return 0;
        else
            return -1;
    }

    private Money CalculatePackCost(Member user, ReferralPackage pack)
    {
        Money membershipSingleRefPrice = user.Membership.ReferralRentCost;
        int currentRefs = new RentReferralsSystem(user.Name, user.Membership).GetUserRentedReferralsCount();

        Money singleRefPrice = RentedReferralRangePrice.GetPriceForSingleRef(membershipSingleRefPrice, currentRefs + pack.RefsInPackage);

        return Money.MultiplyPercent(singleRefPrice * pack.RefsInPackage, pack.PercentValue);
    }

    private Money CalcRenewCost(int one, Money two)
    {
        return Money.MultiplyPercent(two, 100 - one);
    }

    private bool IsParameterANumber()
    {
        bool isValid = true;
        try
        {
            Int32.Parse(Request.Params.Get("__PACKID"));
        }
        catch (Exception ex)
        {
            isValid = false;
        }
        return isValid;
    }

    protected void DirectRefsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var chbox = ((HtmlInputCheckBox)e.Row.FindControl("chkSelect"));
            chbox.Attributes.Add("onclick", SelectedPanel.ClientID + ".style.display = 'block';hideIfUnchecked('" + SelectedPanel.ClientID + "');");

            //Expires[4]
            DateTime expires = Convert.ToDateTime(e.Row.Cells[4].Text);
            int daysLeft = expires.Subtract(DateTime.Now).Days;
            e.Row.Cells[4].Text = daysLeft.ToString() + " " + L1.DAYS;

            //Last click[5]
            string output;
            if (string.IsNullOrEmpty(e.Row.Cells[5].Text) || e.Row.Cells[5].Text == "&nbsp;")
                output = "<i>" + L1.NEVER + "</i>";
            else
            {
                DateTime lastActivity = Convert.ToDateTime(e.Row.Cells[5].Text);
                int days = (int)Math.Ceiling(DateTime.Now.Subtract(lastActivity).TotalDays);
                switch (days)
                {
                    case 1:
                        output = L1.TODAY;
                        break;
                    case 2:
                        output = L1.YESTERDAY;
                        break;
                    default:
                        output = days.ToString() + " " + L1.DAYSAGO;
                        break;
                }
            }
            e.Row.Cells[5].Text = output;

            //6 AutoPay check image
            var check = (CheckBox)e.Row.Cells[7].Controls[0];
            if (check.Checked)
                e.Row.Cells[7].Text = HtmlCreator.GetCheckboxCheckedImage();
            else
                e.Row.Cells[7].Text = HtmlCreator.GetCheckboxUncheckedImage();
        }
    }

    private void ClearPanels()
    {
        SuccMessagePanel2.Visible = false;
        ErrorMessagePanel2.Visible = false;
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ClearPanels();
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            for (int i = 0; i < DirectRefsGridView.Rows.Count; i++)
            {
                if (((HtmlInputCheckBox)DirectRefsGridView.Rows[i].FindControl("chkSelect")).Checked)
                {
                    string refId = (DirectRefsGridView.Rows[i].Cells[1].Text.Trim());
                    RentReferralsSystem.DeleteReferral(Int32.Parse(refId), bridge.Instance);
                }
            }
        }
        Response.Redirect("rented.aspx");
    }

    protected void RecycleButton_Click(object sender, EventArgs e)
    {
        ClearPanels();
        //Recycle
        try
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                Member User = Member.Current;
                Money SingleOperationCost = User.Membership.RentedReferralRecycleCost;
                int referralsDone = 0;
                for (int i = 0; i < DirectRefsGridView.Rows.Count; i++)
                {
                    if (((HtmlInputCheckBox)DirectRefsGridView.Rows[i].FindControl("chkSelect")).Checked)
                    {
                        if (SingleOperationCost > User.PurchaseBalance)
                            throw new MsgException(L1.OPPREFPARTIAL.ToString().Replace("%n%", referralsDone.ToString()));

                        string refId = (DirectRefsGridView.Rows[i].Cells[1].Text.Trim());
                        RentReferralsSystem.RecycleReferral(Int32.Parse(refId), bridge.Instance);
                        User.SubtractFromPurchaseBalance(SingleOperationCost, "Ref: recycle");
                        User.SaveBalances();
                        referralsDone++;
                    }
                }

                RentReferralsSystem.TryForceAutopay(User.Name);

                SuccMessagePanel2.Visible = true;
                SuccMessage2.Text = U3501.REFACTIONSUCC;
            }
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel2.Visible = true;
            ErrorMessage2.Text = ex.Message;
        }

        DirectRefsGridView.DataBind();
    }
    protected void AutopayAllOnButton_Click(object sender, EventArgs e)
    {
        ClearPanels();
        //AutoPay
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            int referralsDone = 0;
            for (int i = 0; i < DirectRefsGridView.Rows.Count; i++)
            {
                int ExpireDays = Int32.Parse(DirectRefsGridView.Rows[i].Cells[4].Text.Replace(" " + L1.DAYS, ""));
                string refId = (DirectRefsGridView.Rows[i].Cells[1].Text.Trim());
                int RefId = Int32.Parse(refId);

                referralsDone++;
                RentReferralsSystem.SetAutopayOnReferral(RefId, bridge.Instance);
            }
            SuccMessagePanel2.Visible = true;
            SuccMessage2.Text = U6011.REFACTIONSUCC;

            DirectRefsGridView.DataBind();
        }
    }

    protected void AutopayAllOffButton_Click(object sender, EventArgs e)
    {
        ClearPanels();
        //AutoPay
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            int referralsDone = 0;
            for (int i = 0; i < DirectRefsGridView.Rows.Count; i++)
            {
                int ExpireDays = Int32.Parse(DirectRefsGridView.Rows[i].Cells[4].Text.Replace(" " + L1.DAYS, ""));
                string refId = (DirectRefsGridView.Rows[i].Cells[1].Text.Trim());
                int RefId = Int32.Parse(refId);

                referralsDone++;
                RentReferralsSystem.SetAutopayOffReferral(RefId, bridge.Instance);
            }
            SuccMessagePanel2.Visible = true;
            SuccMessage2.Text = U6011.REFACTIONSUCC;

            DirectRefsGridView.DataBind();
        }
    }

    protected void AutopayOnButton_Click(object sender, EventArgs e)
    {
        ClearPanels();
        bool hasErrors = false;
        //AutoPay
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            int referralsDone = 0;
            for (int i = 0; i < DirectRefsGridView.Rows.Count; i++)
            {
                if (((HtmlInputCheckBox)DirectRefsGridView.Rows[i].FindControl("chkSelect")).Checked)
                {
                    int ExpireDays = Int32.Parse(DirectRefsGridView.Rows[i].Cells[4].Text.Replace(" " + L1.DAYS, ""));
                    string refId = (DirectRefsGridView.Rows[i].Cells[1].Text.Trim());
                    int RefId = Int32.Parse(refId);

                    //Should we turn on or off

                    if (ExpireDays < AppSettings.RentedReferrals.MinDaysToStartAutoPay)
                        hasErrors = true;
                    else
                    {
                        referralsDone++;
                        RentReferralsSystem.SetAutopayOnReferral(RefId, bridge.Instance);
                    }
                }
            }
            if (!hasErrors) //so bad, exceptions [*]
            {
                SuccMessagePanel2.Visible = true;
                SuccMessage2.Text = U3501.REFACTIONSUCC2;
            }
            else
            {
                ErrorMessagePanel2.Visible = true;
                ErrorMessage2.Text = L1.OPPREFAPARTIAL + " " + L1.OPREFAP;
                ErrorMessage2.Text = ErrorMessage2.Text.Replace("%n%", referralsDone.ToString());
                ErrorMessage2.Text = ErrorMessage2.Text.Replace("%n2%", AppSettings.RentedReferrals.MinDaysToStartAutoPay.ToString());
            }
            DirectRefsGridView.DataBind();
        }
    }

    protected void AutopayOffButton_Click(object sender, EventArgs e)
    {
        ClearPanels();
        bool hasErrors = false;
        //AutoPay
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            int referralsDone = 0;
            for (int i = 0; i < DirectRefsGridView.Rows.Count; i++)
            {
                if (((HtmlInputCheckBox)DirectRefsGridView.Rows[i].FindControl("chkSelect")).Checked)
                {
                    int ExpireDays = Int32.Parse(DirectRefsGridView.Rows[i].Cells[4].Text.Replace(" " + L1.DAYS, ""));
                    string refId = (DirectRefsGridView.Rows[i].Cells[1].Text.Trim());
                    int RefId = Int32.Parse(refId);

                    //Should we turn on or off

                    if (ExpireDays < AppSettings.RentedReferrals.MinDaysToStartAutoPay)
                        hasErrors = true;
                    else
                    {
                        referralsDone++;
                        RentReferralsSystem.SetAutopayOffReferral(RefId, bridge.Instance);
                    }
                }
            }
            if (!hasErrors) //so bad, exceptions [*]
            {
                SuccMessagePanel2.Visible = true;
                SuccMessage2.Text = U3501.REFACTIONSUCC2;
            }
            else
            {
                ErrorMessagePanel2.Visible = true;
                ErrorMessage2.Text = L1.OPPREFAPARTIAL + " " + L1.OPREFAP;
                ErrorMessage2.Text = ErrorMessage2.Text.Replace("%n%", referralsDone.ToString());
                ErrorMessage2.Text = ErrorMessage2.Text.Replace("%n2%", AppSettings.RentedReferrals.MinDaysToStartAutoPay.ToString());
            }
            DirectRefsGridView.DataBind();
        }
    }

    protected void DirectRefsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[4] { "comDelete", "comRenew", "comRecycle", "comAutopay" };

        if (commands.Contains(e.CommandName))
        {
            int index = e.GetSelectedRowIndex();

            GridViewRow row = DirectRefsGridView.Rows[index];
            ClearPanels();

            try
            {
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    if (e.CommandName == "comDelete")
                    {
                        string refId = (row.Cells[1].Text.Trim());
                        RentReferralsSystem.DeleteReferral(Int32.Parse(refId), bridge.Instance);
                        Response.Redirect("rented.aspx");
                    }
                    else if (e.CommandName == "comRenew")
                    {
                        Member User = Member.Logged(Context);
                        Money SingleOperationCost = CalcRenewCost(User.Membership.RenewalDiscount, User.Membership.ReferralRentCost);

                        if (User.PurchaseBalance < SingleOperationCost)
                            throw new MsgException(L1.NOTENOUGHFUNDS);

                        User.SubtractFromPurchaseBalance(SingleOperationCost, "Ref: renew");
                        User.SaveBalances();
                        string refId = (row.Cells[1].Text.Trim());
                        RentReferralsSystem.RenewReferral(Int32.Parse(refId), bridge.Instance);
                    }
                    else if (e.CommandName == "comRecycle")
                    {
                        Member User = Member.Logged(Context);
                        Money SingleOperationCost = User.Membership.RentedReferralRecycleCost;

                        if (User.PurchaseBalance < SingleOperationCost)
                            throw new MsgException(L1.NOTENOUGHFUNDS);

                        string refId = (row.Cells[1].Text.Trim());
                        RentReferralsSystem.RecycleReferral(Int32.Parse(refId), bridge.Instance);
                        User.SubtractFromPurchaseBalance(SingleOperationCost, "Ref: recycle");
                        User.SaveBalances();
                    }
                    else if (e.CommandName == "comAutopay")
                    {
                        bool setOn = true;
                        string refId = (row.Cells[1].Text.Trim());

                        if (row.Cells[7].Text.Contains("img")) // I know its bad, very bad 
                            setOn = false;

                        if (setOn)
                        {
                            int ExpireDays = Int32.Parse(row.Cells[4].Text.Replace(" " + L1.DAYS, ""));
                            if (ExpireDays < AppSettings.RentedReferrals.MinDaysToStartAutoPay)
                                throw new MsgException((L1.OPREFAP).ToString().Replace("%n2%", AppSettings.RentedReferrals.MinDaysToStartAutoPay.ToString()));

                            RentReferralsSystem.SetAutopayOnReferral(Int32.Parse(refId), bridge.Instance);
                        }
                        else
                            RentReferralsSystem.SetAutopayOffReferral(Int32.Parse(refId), bridge.Instance);
                    }

                    DirectRefsGridView.DataBind();
                    SuccMessagePanel2.Visible = true;
                    SuccMessage2.Text = L1.OP_SUCCESS;
                }
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel2.Visible = true;
                ErrorMessage2.Text = ex.Message;
            }
        }
    }

    protected void RenewDropDownList_Init(object sender, EventArgs e)
    {
        RenewDropDownList.Items.Clear();
        var discounts = RentedReferralRenewalDiscount.GetAll();
        var membership = Member.CurrentInCache.Membership;

        if (!discounts.Any(d => d.Days == 30))
            discounts.Add(RentedReferralRenewalDiscount.GetDefaultObject());

        discounts = discounts.OrderBy(x => x.Days).ToList();

        foreach (var d in discounts)
        {
            RenewDropDownList.Items.Add(new ListItem(string.Format("{0} {1}: {2}", d.Days, L1.DAYS, RentedReferralRenewalDiscount.GetRenewalPrice(membership, d)), d.Id.ToString()));
        }
    }

    protected void RenewButton_Click(object sender, EventArgs e)
    {
        ClearPanels();
        //Renew
        try
        {
            Member user = Member.Current;

            int referralsDone = 0;
            for (int i = 0; i < DirectRefsGridView.Rows.Count; i++)
            {
                if (((HtmlInputCheckBox)DirectRefsGridView.Rows[i].FindControl("chkSelect")).Checked)
                {
                    RentedReferralRenewalDiscount discount;
                    var discountId = Convert.ToInt32(RenewDropDownList.SelectedValue);

                    if (discountId == -1)
                        discount = RentedReferralRenewalDiscount.GetDefaultObject();
                    else
                        discount = new RentedReferralRenewalDiscount(discountId);

                    var price = RentedReferralRenewalDiscount.GetRenewalPrice(user.Membership, discount);
                    if (price > user.PurchaseBalance)
                        throw new MsgException(L1.OPPREFPARTIAL.ToString().Replace("%n%", referralsDone.ToString()));

                    user.SubtractFromPurchaseBalance(price, "Ref: renew");
                    user.SaveBalances();
                    referralsDone++;
                    string refId = (DirectRefsGridView.Rows[i].Cells[1].Text.Trim());
                    RentReferralsSystem.RenewReferral(Int32.Parse(refId), discount.Days);
                }
            }
            SuccMessagePanel2.Visible = true;
            SuccMessage2.Text = U3501.REFACTIONSUCC;
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel2.Visible = true;
            ErrorMessage2.Text = ex.Message;
        }

        DirectRefsGridView.DataBind();
    }
}
