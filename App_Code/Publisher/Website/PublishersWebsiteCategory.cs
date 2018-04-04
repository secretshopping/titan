using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Resources;


public class PublishersWebsiteCategory : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "PublishersWebsiteCategories"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("Name")]
    public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    public UniversalStatus Status
    {
        get { return (UniversalStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    int _Id, _StatusInt;
    string _Name;
    #endregion Columns

    public PublishersWebsiteCategory(int id) : base(id) { }

    public PublishersWebsiteCategory(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    private PublishersWebsiteCategory(string name)
    {
        Name = name;
        Status = UniversalStatus.Paused;
    }

    public static void Create(string name)
    {
        if (TableHelper.RowExists(PublishersWebsiteCategory.TableName, "Name", name))
            throw new MsgException("Category must be unique");

        var category = new PublishersWebsiteCategory(name);
        category.Save();
    }

    public void Activate()
    {
        Status = UniversalStatus.Active;
        this.Save();
    }

    public void Pause()
    {
        Status = UniversalStatus.Paused;
        this.Save();
    }

    public override void Delete()
    {
        Status = UniversalStatus.Deleted;
        this.Save();
    }

    public static List<PublishersWebsiteCategory> GetActive()
    {
        return TableHelper.SelectRows<PublishersWebsiteCategory>(
            TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
    }

    public static bool AreAnyActive()
    {
        return TableHelper.RowExists("PublishersWebsiteCategories", TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
    }
}