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


public class ReferralsBanner : BaseTableObject
{
    #region Columns


    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "ReferralsBanners"; } }
    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public const string Id = "Id";
        public const string BannerUrl = "BannerUrl";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.BannerUrl)]
    public string BannerUrl { get { return _BannerUrl; } set { _BannerUrl = value; SetUpToDateAsFalse(); } }

    private int _Id;
    private string _BannerUrl;

    public ReferralsBanner()
        : base() { }

    public ReferralsBanner(int id) : base(id) { }

    public ReferralsBanner(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    #endregion Columns

}