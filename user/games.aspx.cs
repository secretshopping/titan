using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;

public partial class About : System.Web.UI.Page
{
    public static FamobiGame famobiGame;
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PeopleGamesEnabled);


        if (!IsPostBack)
        {
            // One or all games ?
            string GameIdString = string.IsNullOrEmpty(Request.QueryString["gameId"]) ? "" : Request.QueryString["gameId"];
            if (GameIdString != "")
            {
                GamesLiteral.Visible = false;
                OneGamePanel.Visible = true;
                try
                {
                    famobiGame = new FamobiGame(Convert.ToInt32(GameIdString));
                }
                catch (Exception ex) { }
            }
            else
            {
                var availableGames = TableHelper.GetListFromQuery<FamobiGame>(string.Format("WHERE {0} IN ({1})", FamobiGame.Columns.RequiredMembershipId, Membership.GetSqlQuerryForMembershipsIdListUnderCurrentMembership()));
                StringBuilder gamesUrl = new StringBuilder();

                foreach (var game in availableGames)
                {
                    gamesUrl.Append("<div class=\"gamesDiv\"><a href=\"user/games.aspx?gameId=" + game.Id + "\"><img src=\"" + game.ThumbUrl + "\"/></a><span>" + game.Name + "</span></div>");
                }

                GamesLiteral.Text = gamesUrl.ToString();
            }
        }
    }

    public static string GetUrl()
    {       
        return (famobiGame.Url + "/" + AppSettings.Offerwalls.FamobiGameUserId);
    }
    public static string GetTitlGame()
    {
        return famobiGame.Name;
    }
    public static string GetDescriptionGame()
    {
        return famobiGame.Description;
    }

    public static string GetAscpetRatio()
    {
        return famobiGame.AspectRatio.Replace(",",".");
    }

    public static int GetHeight()
    {
        return Convert.ToInt32(640 / Convert.ToDecimal(GetAscpetRatio())) + 1;
    }

}
