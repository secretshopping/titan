using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;
using Prem.PTC.Advertising;
using Resources;
using Titan;

public partial class About : System.Web.UI.Page
{
    public string DisplayNoneIfEmpty = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && AppSettings.TitanFeatures.EarnOfferwallsEnabled && Member.CurrentInCache.IsEarner);

        var ActiveOfferwalls = OfferwallManager.GetOfferwallsForMember(Member.CurrentInCache);

        if (Request.QueryString["wall"] != null)
        {
            try
            {
                for (int i = 0; i < ActiveOfferwalls.Count; i++)
                {
                    string offname = HttpUtility.UrlDecode(Request.QueryString["wall"].Trim());

                    if (offname == ActiveOfferwalls[i].DisplayName.Trim())
                        MenuMultiView.ActiveViewIndex = i;

                    Button butt = FindProperButton(offname);
                    foreach (Button b in MenuButtonPlaceHolder.Controls)
                        b.CssClass = "";

                    butt.CssClass = "ViewSelected";
                }
            }
            catch (Exception ex)
            { }
        }
        else if (ActiveOfferwalls.Count > 0)
        {
            MenuMultiView.ActiveViewIndex = 0;
        }
        else
            DisplayNoneIfEmpty = "display:none;";
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        Member User = Member.CurrentInCache;

        //Generate Offerwalls
        var ActiveOfferwalls = OfferwallManager.GetOfferwallsForMember(User);

        for (int i = 0; i < ActiveOfferwalls.Count; ++i)
        {
            var offer = ActiveOfferwalls[i];
            MenuButtonPlaceHolder.Controls.AddAt(i, GetMenuButton(offer.DisplayName, i));
            MenuMultiView.Controls.AddAt(i, GetViewContent(offer, User));
        }
    }

    private Button GetMenuButton(string name, int counter)
    {
        Button button = new Button();
        button.Click += new EventHandler(MenuButton_Click);
        button.CommandArgument = counter.ToString();             
        button.Text = name;

        if (counter == 0)
            button.CssClass = "ViewSelected";

        return button;
    }

    private Button FindProperButton(string offerwallname)
    {
        foreach (var control in MenuButtonPlaceHolder.Controls)
        {
            if (control is Button && ((Button)control).Text == offerwallname)
                return (Button)control;
        }
        return null;
    }

    private View GetViewContent(Offerwall offer, Member User)
    {
        View view = new View();
        Literal pre = new Literal();
        Literal post = new Literal();
        Literal content = new Literal();

        pre.Text = "<div class=\"TitanViewElement NewTitanViewElement\"><br /><br />";
        post.Text = "</div>";
        content.Text = offer.ToClientHTML(User);

        view.Controls.AddAt(0, pre);
        view.Controls.AddAt(1, content);
        view.Controls.AddAt(2, post);

        return view;
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("offers.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
            b.CssClass = "";

        TheButton.CssClass = "ViewSelected";
    }
}
