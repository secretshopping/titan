using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UniqueAd
/// </summary>
public class UniqueAd : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "UniqueAds"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    //not logged in = -1
    [Column("CreatorUserId")]
    public int CreatorUserId { get { return _CreatorUserId; } set { _CreatorUserId = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

    [Column("TargetUrl")]
    public string TargetUrl { get { return UrlCreator.ParseUrl(_TargetUrl); } set { _TargetUrl = value; SetUpToDateAsFalse(); } }

    [Column("RewardPerView")]
    public Money RewardPerView { get { return _RewardPerView; } set { _RewardPerView = value; SetUpToDateAsFalse(); } }

    [Column("ViewsBought")]
    public int ViewsBought { get { return _ViewsBought; } set { _ViewsBought = value; SetUpToDateAsFalse(); } }

    [Column("ViewsReceived")]
    public int ViewsReceived { get { return _ViewsReceived; } set { _ViewsReceived = value; SetUpToDateAsFalse(); } }

    private int _Id, _Status, _CreatorUserId, _ViewsBought, _ViewsReceived;
    private string _TargetUrl;
    Money _RewardPerView;

    public AdvertStatus Status
    {
        get { return (AdvertStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    #endregion

    public bool ShouldBeFinished { get { return ViewsReceived >= ViewsBought; } }

    #region Constructors

    public UniqueAd()
            : base()
        {

    }
    public UniqueAd(int id)
            : base(id)
        {

    }
    public UniqueAd(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {

    }


    #endregion
}