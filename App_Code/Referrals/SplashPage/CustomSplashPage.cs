using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Prem.PTC.Utils.NVP;
using MarchewkaOne.Titan.Balances;
using System.Web.Security;
using Titan;
using ExtensionMethods;
using System.Web.Caching;
using Prem.PTC;


public class CustomSplashPage : BaseTableObject
{
    #region Columns


    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "CustomSplashPages"; } }
    protected override string dbTable { get { return TableName; } }


    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("BackgroundImage")]
    public string BackgroundImage { get { return _BackgroundImage; } set { _BackgroundImage = value; SetUpToDateAsFalse(); } }

    [Column("Text")]
    public string Text { get { return _Text; } set { _Text = value; SetUpToDateAsFalse(); } }

    private int _Id, _UserId;
    private string _BackgroundImage, _Text;

    public CustomSplashPage()
        : base() { }

    public CustomSplashPage(int id) : base(id) { }

    public CustomSplashPage(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    #endregion Columns


    public static CustomSplashPage Get(int userId)
    {
        try
        {
            return TableHelper.GetListFromQuery<CustomSplashPage>("WHERE UserId = " + userId)[0];
        }
        catch(Exception ex)
        {
            return null;
        }
    }
}