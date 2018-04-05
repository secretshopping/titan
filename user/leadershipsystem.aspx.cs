using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Text;
using System.Web.UI;
using Titan.Leadership;

public partial class user_leadershipsystem : System.Web.UI.Page
{
    private Member User;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.LeaderShipSystemEnabled);

        if (!Page.IsPostBack)
        {
            User = Member.CurrentInCache;


            var userRank = User.GetRank() != null ? User.GetRank().Rank : 0;
            var rankList = LeadershipRank.RankList;

            try
            {
                var rank = rankList.Find(x => x.Rank == userRank);
                CurrentRankControl.Rank = rank;
                CurrentRankControl.DataBind();
                CurrentRankNameLabel.Text = string.Format("{0}: <b>{1}</b>", U6012.NEXTRANK, userRank.ToString());
            }
            catch(Exception ex)
            {
                CurrentRankControl.Visible = false;
            }            

            for(int i = 0; i < rankList.Count; i++)
            {
                var objControl = (UserControl)Page.LoadControl("~/Controls/LeadershipSytemRow.ascx");
                var parsedControl = objControl as ILeadershipSystemRowControl;

                parsedControl.Rank = rankList[i];
                parsedControl.DataBind();

                RanksPanel.Controls.Add(objControl);
            }

            InitAccountDetails();
        }
    }

    private void InitAccountDetails()
    {
        var sb = new StringBuilder();

        //ActiveAdPacks = 0,
        if (LeadershipSystem.IsRestrictionOfThisKindExist(RestrictionKind.ActiveAdPacks))        
            sb.Append(string.Format("{0} {1}: <b>{2}</b><br />",
                L1.ACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural, User.ActiveAdPacks.ToString()));        

        //ActiveAdPacksPrice = 1,
        if (LeadershipSystem.IsRestrictionOfThisKindExist(RestrictionKind.ActiveAdPacksPrice))        
            sb.Append(string.Format("{0} {1} {2}: <b>{3}</b><br />",
                L1.ACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural, L1.PRICE, ((Money)LeadershipRankRequirements.GetRequirmentProgress(User.Id, RestrictionKind.ActiveAdPacksPrice)).ToString()));

        //DirectReferralsCount = 2,
        if (LeadershipSystem.IsRestrictionOfThisKindExist(RestrictionKind.DirectReferralsCount))
            sb.Append(string.Format("{0}: <b>{1}</b><br />",
                U6005.DIRECTREFERRALSCOUNT, LeadershipRankRequirements.GetRequirmentProgress(User.Id, RestrictionKind.DirectReferralsCount)));
            
        //DirectReferralsActiveAdPacks = 3,
        if (LeadershipSystem.IsRestrictionOfThisKindExist(RestrictionKind.DirectReferralsActiveAdPacks))        
            sb.Append(string.Format("{0} {1} {2}: <b>{3}</b><br />",
                U6005.DIRECTREFERRALSACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural, U6005.COUNT, LeadershipRankRequirements.GetRequirmentProgress(User.Id, RestrictionKind.DirectReferralsActiveAdPacks)));

        //DirectReferralsActiveAdPacksPrice = 4,
        if (LeadershipSystem.IsRestrictionOfThisKindExist(RestrictionKind.DirectReferralsActiveAdPacksPrice))
            sb.Append(string.Format("{0} {1} {2}: <b>{3}</b><br />",
                U6005.DIRECTREFERRALSACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural, L1.AMOUNT, ((Money)LeadershipRankRequirements.GetRequirmentProgress(User.Id, RestrictionKind.DirectReferralsActiveAdPacksPrice)).ToString()));

        //Points = 5,
        if (LeadershipSystem.IsRestrictionOfThisKindExist(RestrictionKind.Points))
            sb.Append(string.Format("{0}: <b>{1}</b><br />",
                AppSettings.PointsName, LeadershipRankRequirements.GetRequirmentProgress(User.Id, RestrictionKind.Points)));

        //DirectReferralsTotalPaidIn = 7,
        if (LeadershipSystem.IsRestrictionOfThisKindExist(RestrictionKind.DirectReferralsTotalPaidIn))
            sb.Append(string.Format("{0}: <b>{1}</b><br />",
                U6006.DIRECTREFERRALSTOTALPAIDIN, ((Money)LeadershipRankRequirements.GetRequirmentProgress(User.Id, RestrictionKind.DirectReferralsTotalPaidIn)).ToString()));

        AccountDetailsLiteral.Text = sb.ToString();
    }
}