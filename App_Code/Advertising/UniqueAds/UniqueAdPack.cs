using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UniqueAdPack
/// </summary>
public class UniqueAdPack : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "UniqueAdPacks"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

    [Column("Price")]
    public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

    [Column("RewardPerView")]
    public Money RewardPerView { get { return _RewardPerView; } set { _RewardPerView = value; SetUpToDateAsFalse(); } }

    [Column("Views")]
    public int Views { get { return _Views; } set { _Views = value; SetUpToDateAsFalse(); } }

    int _Id, _Status, _Views;

    Money _Price, _RewardPerView;

    public UniversalStatus Status
    {
        get { return (UniversalStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    #endregion

    #region Constructors

    public UniqueAdPack()
            : base()
        {

    }
    public UniqueAdPack(int id)
            : base(id)
        {

    }
    public UniqueAdPack(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {

    }


    #endregion
}