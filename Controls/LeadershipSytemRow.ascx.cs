using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titan.Leadership;

public partial class Controls_LeadershipSystemRow : System.Web.UI.UserControl, ILeadershipSystemRowControl
{
    public LeadershipRank Rank { get; set; }

    public List<LeadershipRankRequirements> RanksRequirements
    {
        get
        {
            var requirementsList = LeadershipRankRequirements.RequirementsList;
            var _RanksRequirements = requirementsList.Where(x => x.Rank == Rank.Id).ToList();

            return _RanksRequirements;
        }
    }

    public bool IsRankAcquired
    {
        get
        {
            var rankMax = Member.Current.GetMaxRank();
            return rankMax != null && Rank.Rank <= rankMax.Rank;
        }
    }

    public bool IsCurrentRank
    {
        get
        {
            var rank = Member.Current.GetRank();
            return rank != null && Rank.Rank == rank.Rank;
        }
    }

    public override void DataBind()
    {
        base.DataBind();
        CreateRequirements();
        InitPrize();
    }

    private void CreateRequirements()
    {
        var list = RanksRequirements;
        var sb = new StringBuilder();
        int acquiredReqs = 0;

        for (int i = 0; i < list.Count; i++)
        {
            var change = ChangeColor();
            var inProgress = false;
            ProgressBarLiteral.Visible = false;

            if (change)
                sb.Append(GetColor(list[i], out inProgress));

            sb.Append(string.Format("{0}: <b>", LeadershipSystem.ReturnName(list[i].RestrictionKind)));

            if (list[i].RestrictionKind == RestrictionKind.RequiredMembership)
                sb.Append(Membership.SelectName(list[i].RestrictionValue));
            else if (LeadershipSystem.IsMoneyRestriction(list[i].RestrictionKind))
                sb.Append((Money)list[i].RestrictionValue);
            else
                sb.Append(list[i].RestrictionValue);

            if (inProgress)
            {
                acquiredReqs++;
                sb.Append(GetProgress(list[i].RestrictionValue, list[i].RestrictionKind));
            }
            if (change)
                sb.Append("</b></span><br />");
            else
                sb.Append("</b></span><br />");
        }

        RanksRequirementsLiteral.Text = sb.ToString();

        ProgressBarLiteral.Visible = true;

        if (IsRankAcquired)
            ProgressBarLiteral.Text = string.Format("<br /><div class='progress'><div class='progress-bar progress-bar-success' style='width: {0}%'>{1}</div></div>", 100, "100%");
        else if (!IsRankAcquired && list.Count > 1)
        {
            var progress = 100 - (acquiredReqs != 0 ? Convert.ToDecimal(acquiredReqs) / Convert.ToDecimal(list.Count) * 100 : 100);
            if (progress > 100)
                progress = 100;
            ProgressBarLiteral.Text = string.Format("<br /><div class='progress'><div class='progress-bar' style='width: {0}%'>{1:1.##}</div></div>", progress, string.Format("{0}%", progress));
        }
        else
        {
            var progress = 100 - GerPercentageProgress(list[0]);
            if (progress > 100)
                progress = 100;
            ProgressBarLiteral.Text = string.Format("<br /><div class='progress'><div class='progress-bar' style='width: {0}%'>{0:0.##}%</div></div>", progress);
        }
    }

    private bool ChangeColor()
    {
        var rank = Member.Current.GetRank();
        return (rank == null && Rank.Rank == 1) || (rank != null && rank.Rank + 1 == Rank.Rank);
    }

    private string GetColor(LeadershipRankRequirements requirment, out bool inProgress)
    {
        inProgress = false;
        if (requirment.CheckRequirement(Member.CurrentInCache.Id))
            return "<span class='text-success'><i class='fa fa-check m-r-10'></i>";

        inProgress = true;
        return "<span class='text-danger'><i class='fa fa-times m-r-10'></i>";
    }

    private decimal GerPercentageProgress(LeadershipRankRequirements rankReq)
    {
        if (rankReq.RestrictionKind == RestrictionKind.RequiredMembership)
            return 0;

        object value;
        try
        {
            value = rankReq.RestrictionValue - (int)LeadershipRankRequirements.GetRequirmentProgress(Member.CurrentId, rankReq.RestrictionKind);
            return Convert.ToDecimal(value) / rankReq.RestrictionValue * 100;
        }
        catch (Exception ex)
        {
            value = (Money)rankReq.RestrictionValue - (Money)LeadershipRankRequirements.GetRequirmentProgress(Member.CurrentId, rankReq.RestrictionKind);
            return Convert.ToDecimal(((Money)value).ToDecimal()) / rankReq.RestrictionValue * 100;
        }
    }

    private string GetProgress(int max, RestrictionKind restriction)
    {
        if (restriction == RestrictionKind.RequiredMembership)
            return "";

        StringBuilder result = new StringBuilder();
        result.Append(string.Format(" ({0}: ",U6006.REMAINING));

        try
        {
            result.Append(max - (int)LeadershipRankRequirements.GetRequirmentProgress(Member.CurrentId, restriction));
        }
        catch (Exception ex)
        {
            result.Append((Money)max - (Money)LeadershipRankRequirements.GetRequirmentProgress(Member.CurrentId, restriction));
        }

        result.Append(")");
        return result.ToString();
    }

    private void InitPrize()
    {
        if (Rank.PrizeKind == PrizeKind.CustomPrize)
        {
            CustomPrizePlaceHolder.Visible = true;
            PrizeImage.ImageUrl = Rank.PrizeValue;
        }
        else
        {
            PrizePlaceHolder.Visible = true;
            if (Rank.PrizeKind == PrizeKind.MainBalance)
                PrizeLiteral.Text = string.Format("<b>{0}</b>", new Money(int.Parse(Rank.PrizeValue)).ToString());
            else
                PrizeLiteral.Text = string.Format("<b>{0}</b>", Rank.PrizeValue);
        }

        if (TitanFeatures.IsTrafficThunder)
        {
            PrizePlaceHolder.Visible = true;
            PrizeLiteral.Text = string.Format("</td></tr><tr><td>{0}</td><td style='min-width:80px'><b>{1}</b></td>", "Binary Cap",
                (Rank.MatrixCyclesPerDay * new Money(1)).ToString());
        }
    }
}