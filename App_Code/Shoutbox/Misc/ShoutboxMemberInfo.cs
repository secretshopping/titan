using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[Serializable]
public class ShoutboxMemberInfo
{
    public string FormattedName { get; set; }
    public string Avatar { get; set; }
    public string CountryCode { get; set; }
    public ShoutboxPermission ShoutboxPrivacyPermission { get; set; }
    public bool IsForumAdministrator { get; set; }

    public ShoutboxMemberInfo(string avatar, string cc, string formatedName, ShoutboxPermission permission, bool isAdmin)
    {
        this.FormattedName = formatedName;
        this.Avatar = avatar;
        this.CountryCode = cc;
        this.ShoutboxPrivacyPermission = permission;
        this.IsForumAdministrator = isAdmin;
    }
}