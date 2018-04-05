using System;
using System.Collections.Generic;
using System.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using Newtonsoft.Json.Linq;
using Microsoft.SqlServer.Types;
using System.Web.Services;
using Titan.Leadership;
using MemberExtentionMethods;
using SocialNetwork;

public partial class Matrix : System.Web.UI.Page
{
    public JArray Nodes;
    public JArray Edges;
    public JArray UsersToAssign;

    public bool ShowPoints = false;
    public bool PointsOnEdges = true;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ReferralMatrixEnabled);
        AccessManager.RedirectIfDisabled(AppSettings.Matrix.Type == MatrixType.Referral);

        MatrixBase matrix = MatrixFactory.GetMatrix();
        ShowPoints = matrix != null ? matrix.Crediter is BinaryReferralMatrixCrediter : false;
        PointsOnEdges = !TitanFeatures.IsTrafficThunder;

        if (!Page.IsPostBack)
        {
            InitializeData();
        }

        //Listening for AJAX request "Assign the member"
        ListenForAJAX();
    }

    protected void ListenForAJAX()
    {
        if (!String.IsNullOrEmpty(Request.Params["nodeId"]) && !String.IsNullOrEmpty(Request.Params["user"]))
        {
            Member User = Member.CurrentInCache;
            Member DesiredReferral = new Member(Request.Params["user"]);

            string NodeId = Request.Params["nodeId"].Replace('_', '/').Replace("PARENT-", "");

            DesiredReferral.MatrixId = SqlHierarchyId.Parse(NodeId);
            DesiredReferral.Save();

            //Response.Redirect("~/user/refs/matrix.aspx");
        }
    }

    protected void InitializeData()
    {
        Member User = Member.CurrentInCache;

        Nodes = new JArray();
        Edges = new JArray();
        
        List<Member> UnassignedReferrals = User.GetUnassignedMatrixMembers();

        bool userInMatrix = (User.MatrixId != SqlHierarchyId.Null).IsTrue;
        bool IsUnassignedReferrals = !AppSettings.Matrix.AutolocateMembersInBinaryMatrix && UnassignedReferrals.Count > 0;

        AllocateReferralScriptsPlaceHolder.Visible = IsUnassignedReferrals && userInMatrix;
        AssignButtonsPlaceHolder.Visible = IsUnassignedReferrals;
        UnassignedReferralsPlaceHolder.Visible = IsUnassignedReferrals;

        AssignLeftDropList.DataSource = UnassignedReferrals;
        AssignRightDropList.DataSource = UnassignedReferrals;

        AssignLeftDropList.DataBind();
        AssignRightDropList.DataBind();

        UnassignedMatrixReferralsLiteral.Text = String.Format(U6007.UNASSIGNEDMATRIXMEMBERS,
            "<b>" + UnassignedReferrals.Count + "</b>");

        var UsersInMatrix = User.GetDescendants();

        Nodes = new JArray();
        Edges = new JArray();

       

        MatrixTreePlaceHolder.Visible = userInMatrix;
        MatrixTreeScriptPlaceHolder.Visible = userInMatrix;
        NotInMatrixPlaceHolder.Visible = !userInMatrix;

        //Generate Matrix
        if (userInMatrix)
        {
            AddNode(User);
            AddChildrens(User);
        }

        UsersToAssign = new JArray(UnassignedReferrals.Select(item => item.Name).ToArray());
        
    }

    private void AddChildrens(Member member)
    {
        var children = member.GetDirectDescendants();

        for (int i = 0; i < AppSettings.Matrix.MaxChildrenInMatrix; i++)  //Matrix size = 2 (binary)
        {
            SqlHierarchyId result = SqlHierarchyId.Parse(member.MatrixId.ToString() + ((i % AppSettings.Matrix.MaxChildrenInMatrix) + 1) + "/");

            if (children.Any(x => (bool)(x.MatrixId == result)))
            {
                Member child = children.First(x => (bool)(x.MatrixId == result));

                AddNode(child);
                AddEdge(member, child);

                AddChildrens(child);
            }
            else
            {
                AddEmptyNode(member, i);
            }
        }
    }

    private void AddNode(Member member)
    {
        string rankName = LeadershipRank.GetRankName(member.GetRankId());

        JObject properties = new JObject();

        properties.Add(new JProperty("id", member.MatrixId.ToString()));
        properties.Add(new JProperty("userId", member.Id));
        properties.Add(new JProperty("rank", rankName));
        properties.Add(new JProperty("image", Page.ResolveUrl(member.AvatarUrl)));
        properties.Add(new JProperty("flag", Page.ResolveUrl(@"~/Images/Flags/" + member.CountryCode.ToLower() + ".png")));
        properties.Add(new JProperty("countryName", member.Country));
        properties.Add(new JProperty("name", member.Name));
        properties.Add(new JProperty("status", "occupied"));

        string sponsor = Member.GetMemberUsername(member.ReferrerId);
        properties.Add(new JProperty("sponsor", string.IsNullOrWhiteSpace(sponsor) ? "-" : sponsor));

        properties.Add(new JProperty("leftPoints", member.MatrixBonusMoneyFromLeftLeg.ToClearString()));
        properties.Add(new JProperty("rightPoints", member.MatrixBonusMoneyFromRightLeg.ToClearString()));

        FriendshipRequest friendRequest = FriendshipRequest.Get(Member.CurrentId, member.Id);
        bool areFriends =
            Member.CurrentInCache.IsFriendsWith(member)
            || (friendRequest != null && friendRequest.Status != FriendshipRequestStatus.Rejected);

        properties.Add(new JProperty("areNotFriends", AppSettings.TitanFeatures.SocialNetworkEnabled && !areFriends));

        JObject node = new JObject(
            new JProperty("data", properties));

        Nodes.Add(node);
    }

    private void AddEdge(Member source, Member target)
    {
        string matrixId = target.MatrixId.ToString();
        string side = matrixId.Substring(matrixId.Length - 2, 1);
        string edgeLabel = string.Empty;

        if (side == "1") side = "L";
        else if (side == AppSettings.Matrix.MaxChildrenInMatrix.ToString()) side = "R";

        if (ShowPoints && PointsOnEdges)
        {
            switch (side)
            {
                case "L":
                    edgeLabel = source.MatrixBonusMoneyFromLeftLeg.ToShortString();
                    break;
                case "R":
                    edgeLabel = source.MatrixBonusMoneyFromRightLeg.ToShortString();
                    break;
            }
        }

        JObject edge = new JObject(
            new JProperty("data",
                new JObject(
                    new JProperty("source", source.MatrixId.ToString()),
                    new JProperty("label", edgeLabel),
                    new JProperty("target", target.MatrixId.ToString()),
                    new JProperty("side", side))
        ));

        Edges.Add(edge);
    }

    private void AddEmptyNode(Member ancestor, int element)
    {
        string ElementID = "PARENT-" + ancestor.MatrixId.ToString().Replace('/', '_') + (element + 1).ToString() + "_";
        string edgeLabel = String.Empty;
        if (ShowPoints && PointsOnEdges)
        {
            edgeLabel = element == 0 ?
                        ancestor.MatrixBonusMoneyFromLeftLeg.ToShortString()
                        : ancestor.MatrixBonusMoneyFromRightLeg.ToShortString();
        }

        JObject properties = new JObject();

        properties.Add(new JProperty("id", ElementID));
        properties.Add(new JProperty("userId", -1));    
        properties.Add(new JProperty("rank", "-"));
        properties.Add(new JProperty("image", "/Images/Misc/defaultavatar.png"));
        properties.Add(new JProperty("flag", "~/Images/Flags/-.png"));
        properties.Add(new JProperty("countryName", "None"));
        properties.Add(new JProperty("name", ""));
        properties.Add(new JProperty("status", "free"));

        properties.Add(new JProperty("sponsor", String.Empty));

        properties.Add(new JProperty("leftPoints", ancestor.MatrixBonusMoneyFromLeftLeg.ToClearString()));
        properties.Add(new JProperty("rightPoints", ancestor.MatrixBonusMoneyFromRightLeg.ToClearString()));

        properties.Add(new JProperty("areNotFriends", true));


        JObject node = new JObject(
        new JProperty("data", properties));

        Nodes.Add(node);

        JObject edge = new JObject(
            new JProperty("data",
                new JObject(
                    new JProperty("source", ancestor.MatrixId.ToString()),
                    new JProperty("label", edgeLabel),
                    new JProperty("target", ElementID),
                    new JProperty("side", element == 0 ? "L" : "R"))
            ));

        Edges.Add(edge);
    }

    [WebMethod]
    public static void AssignUser(string nodeId, string user)
    {
        Member User = Member.CurrentInCache;
        Member DesiredReferral = new Member(user);

        string NodeId = nodeId.Replace('_', '/').Replace("PARENT-", "");

        DesiredReferral.MatrixId = SqlHierarchyId.Parse(NodeId);
        DesiredReferral.Save();
    }

    protected void AssignFirstLeft_Click(object sender, EventArgs e)
    {
        SqlHierarchyId firstLeft = Member.Current.GetMatrixFirstEmptyLeft();

        Member user = new Member(Int32.Parse(AssignLeftDropList.SelectedValue));

        user.MatrixId = firstLeft;
        user.SaveMatrixId();

        InitializeData();
    }

    protected void AssignFirstRight_Click(object sender, EventArgs e)
    {
        SqlHierarchyId firstRight = Member.Current.GetMatrixFirstEmptyRight();

        Member user = new Member(Int32.Parse(AssignRightDropList.SelectedValue));

        user.MatrixId = firstRight;
        user.SaveMatrixId();

        InitializeData();
    }
}
