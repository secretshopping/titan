using System;
using System.Collections.Generic;
using System.Web.UI;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;

public partial class Controls_Shoutbox : System.Web.UI.UserControl
{
    public string Width { get; set; }
    public string Height { get; set; }
    public int MessagesInFeed { get; set; }
    public bool OnlyEventsOnHomePage { get; set; }

    private const int REFRESH_EVERY_SECS = 45;

    private const string DATE_FORMAT = "MM/dd HH:mm:ss";
    private const string SESSION_CONTENT = "SB.SessionContent";
    private const string SESSION_LAST_UPDATED = "SB.LastUpdated";

    private const string ANONYMOUS_NAME = "Anonymous";

    protected void UpdatePanel1_Load(object sender, EventArgs e)
    {
        UpdateTimer.Interval = REFRESH_EVERY_SECS * 1000;
        if (this.Visible && !Page.IsPostBack)
        {
            if (OnlyEventsOnHomePage)
            {
                Setup();
            }
            else if (!Member.IsLogged || AppSettings.Shoutbox.DisplayMode == ShoutboxDisplayMode.Disabled)
                ShoutboxPanel.Visible = false;
            else
            {
                if (!Member.IsLogged)
                    ShoutboxInputPanel.Visible = false;

                Setup(); 
            }
        }
    }

    protected void TryUpdate(bool ForceUpdate = false)
    {
        var cache = new ShoutboxCache();
        cache.MessagesInFeed = this.MessagesInFeed;
        cache.ForceUpdate = ForceUpdate;

        var results = (List<IShoutboxContent>)cache.Get();

        if (results.Count > 0)
        {
            ContentLiteral.Text = GetParsedHTML(results);
        }
        else
        {
            EmptyPanel.Visible = true;
            EmptyLabel.Text = U3500.NOPOSTS;
        }
    }

    protected void Setup(bool ForceUpdate = false)
    {
        TitleLiteral.Text = U3500.SHOUTBOX;

        //Show button+textbox after update
        ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "ShoutboxEndRequestHandler", "ShoutboxEndRequestHandler();", true);

        TryUpdate(ForceUpdate);

        if (OnlyEventsOnHomePage)
        {
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "shoutboxInitJS", "changeCart(1);", true);
            CartPlaceholder.Visible = false;
            ShoutboxInputPanel.Visible = false;
            PanelButtonsPlaceHolder.Visible = false;
            TitleLiteral.Text = U4000.EVENTS;
        }
        else if (AppSettings.Shoutbox.DisplayContent == ShoutboxDisplayContent.ChatAndEventsInSeparateTabs)
        {
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "shoutboxInitJS", "changeCart(0);", true);
            CartPlaceholder.Visible = true;
        }        
    }

    protected void UpdateTimer_Tick(object sender, EventArgs e)
    {
        TryUpdate();
    }

    protected void SendMessageButton_Click(object sender, EventArgs e)
    {
        try
        {
            ErrorPanelCtrl.Visible = false;
            SuccessPanelCtrl.Visible = false;

            ShoutboxMessage Mess = new ShoutboxMessage();
            Mess.Message = Message2.Text;
            Mess.Username = Member.CurrentName;
            Mess.TrySend(Message.MaxLength);

            Setup(true);
        }
        catch (MsgException ex)
        {
            ErrorPanelCtrl.Visible = true;
            ErrorPanelCtrl.ErrorText = ex.Message;
            ErrorPanelCtrl.DataBind();
        }
        catch (SuccessMsgException ex)
        {
            SuccessPanelCtrl.Visible = true;
            SuccessPanelCtrl.SuccessText = ex.Message;
            SuccessPanelCtrl.DataBind();
        }
    }

    protected string GetParsedHTML(List<IShoutboxContent> List)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < List.Count; ++i)
        {
            sb.Append(GetShoutboxMessageHTML(List[i]));
        }

        return sb.ToString();
    }

    protected string GetShoutboxMessageHTML(IShoutboxContent elem)
    {
        var userInfo = ShoutboxMemberDictionary.Get(elem.Username);
        string Avatar = userInfo.Avatar.StartsWith("~") ? this.ResolveUrl(userInfo.Avatar) : userInfo.Avatar;

        bool ShowCountryFlag = AppSettings.Shoutbox.ShowMemberCountryFlag;

        if (userInfo.IsForumAdministrator)
            ShowCountryFlag = AppSettings.Shoutbox.ShowAdminCountryFlag;

        if (elem.IsEvent)
        {
            if (userInfo.ShoutboxPrivacyPermission == ShoutboxPermission.DoNotPublish)
                return "";
            else if (userInfo.ShoutboxPrivacyPermission == ShoutboxPermission.HideUsername)
            {
                return ShoutboxHTMLCreator.GetShoutboxMessageHTML(ANONYMOUS_NAME, GetDefaultAvatarUrl(), userInfo.CountryCode.ToLower(), elem.SentDate.ToString(DATE_FORMAT), elem.Message,
                    elem.IsEvent, AppSettings.Shoutbox.ShoutboxIconsEnabled, ShowCountryFlag);
            }
        }

        return ShoutboxHTMLCreator.GetShoutboxMessageHTML(userInfo.FormattedName, Avatar, userInfo.CountryCode.ToLower(), elem.SentDate.ToString(DATE_FORMAT),
            elem.Message, elem.IsEvent, AppSettings.Shoutbox.ShoutboxIconsEnabled, ShowCountryFlag);
    }

    private string GetDefaultAvatarUrl()
    {
        if (AppSettings.Misc.DefaultAvatarUrl.StartsWith("~"))
            return Server.MapPath(AppSettings.Misc.DefaultAvatarUrl);
        else
            return AppSettings.Misc.DefaultAvatarUrl;
    }
}
