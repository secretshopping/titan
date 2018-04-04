using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Resources;
using Prem.PTC.Referrals;
using Prem.PTC.Statistics;
using Newtonsoft.Json.Linq;
using Titan.Leadership;

public partial class IndirectReferrals : System.Web.UI.Page
{
    public JArray Nodes;
    public JArray Edges;
    public JArray UsersToAssign;
    List<string> CurrentNodes;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ReferralsIndirectEnabled);

        if (!Page.IsPostBack)
        {
            InitializeData();
        }
    }

    protected void InitializeData()
    {
        Member User = Member.CurrentInCache;
        IndirectReferralsHelper IRH = new IndirectReferralsHelper(User);

        int DirectReferralsCount = User.GetDirectReferralsCount();
        int IndirectReferralsCount = IRH.GetIndirectReferralsCountForMember();

        Nodes = new JArray();
        Edges = new JArray();
        UsersToAssign = new JArray();
        CurrentNodes = new List<string>();

        if (DirectReferralsCount + IndirectReferralsCount == 0)
        {
            ReferralsAvailablePlaceHolder.Visible = false;
            MatrixJavascriptPlaceHolder.Visible = false;
            NoReferralsAvailablePlaceHolder.Visible = true;
        }
        else
        {
            RefCount2.Text = IndirectReferralsCount.ToString();
            RefCount.Text = DirectReferralsCount.ToString();

            var UsersInMatrix = IRH.GetAllIndirectReferralsForGraph();

            //Generate Matrix

            foreach (var UserInMatrix in UsersInMatrix)
            {
                TryAddNode(UserInMatrix.Key);
                TryAddNode(UserInMatrix.Value);

                JObject edge = new JObject(
                    new JProperty("data",
                        new JObject(
                            new JProperty("source", UserInMatrix.Key.Name),
                            new JProperty("target", UserInMatrix.Value.Name))
                ));

                Edges.Add(edge);
            }
        }
    }

    protected void TryAddNode(Member member)
    {
        if (!CurrentNodes.Contains(member.Name))
        {
            string rankName = LeadershipRank.GetRankName(member.GetRankId());

            JObject properties = new JObject();

            properties.Add(new JProperty("id", member.Name));
            properties.Add(new JProperty("rank", rankName));
            properties.Add(new JProperty("image", Page.ResolveUrl(member.AvatarUrl)));
            properties.Add(new JProperty("flag", Page.ResolveUrl(@"~/Images/Flags/" + member.CountryCode.ToLower() + ".png")));
            properties.Add(new JProperty("countryName", member.Country));
            properties.Add(new JProperty("name", member.Name));
            properties.Add(new JProperty("status", "occupied"));


            JObject node = new JObject(
                new JProperty("data", properties));

            Nodes.Add(node);

            CurrentNodes.Add(member.Name);
        }
    }

}
