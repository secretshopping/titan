using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using Prem.PTC.Texts;
using Prem.PTC.Contests;

public partial class Controls_Contest : System.Web.UI.UserControl, ICustomObjectControl
{
    public int ObjectID { get; set; }

    public Contest Contest;
    public List<string> PrizesList, TopList;

    public bool IsImageRewards
    {
        get {
            return Contest.RewardType == PrizeType.CustomPrize;
        }
    }

    public override void DataBind()
    {
        base.DataBind();
        Contest = new Contest(ObjectID);

        PrizesList = Contest.GetPrizesList();
        TopList = Contest.GetTopMembersList();

        MinTransferPlaceHolder.Visible = Contest.Type == ContestType.Transfer;
    }

    public bool IsActive
    {
        get
        {
            return Contest.IsMemberParticipating(Member.CurrentName);
        }
    }

    public string ParticipateButtonHTML
    {
        get
        {
            if (Contest.CanMemberParticipate(Member.CurrentInCache))
            {
               return "<a href=\"user/earn/contests.aspx?participate=" + Contest.Id + "\" class=\"btn btn-inverse m-15\" style=\"margin-left: 15px;\">"
                    + L1.PARTICIPATE + "</a>";
            }
            return String.Empty;
        }
    }
    public string AdditionalRequirementsHTML
    {
        get
        {
            if (Contest.Type == ContestType.Direct)
            {
                return "<li>" + L1.REFERRALS + " " + L1.CLICKSSMALL + " : " + Contest.ClicksReferallsRestriction + @" </li>";
            }
            return String.Empty;
        }
    }
 
}
