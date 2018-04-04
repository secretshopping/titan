using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public class JackpotPvpStage : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "JackpotPvpStages"; } }
    protected override string dbTable { get { return TableName; } }


    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("Name")]
    public String Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

    [Column("Cost")]
    public Money Cost { get { return _Cost; } set { _Cost = value; SetUpToDateAsFalse(); } }

    [Column("WinPercent")]
    public int WinPercent { get { return _WinPercent; } set { _WinPercent = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    private int StatusInt { get { return (int)_Status; } set { _Status = (UniversalStatus)value; SetUpToDateAsFalse(); } }

    public UniversalStatus Status { get { return (UniversalStatus)StatusInt; } set { StatusInt = (int)value; } }

    private int _id, _WinPercent;
    private String _Name;
    private Money _Cost;
    private UniversalStatus _Status;


    #endregion

    public JackpotPvpStage() : base() { }

    public JackpotPvpStage(int id) : base(id) { }

    public JackpotPvpStage(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

    #region Helpers

    public static bool CheckIfStageWithThisNameExists(String stageName)
    {
        int objectsFound = Convert.ToInt32(TableHelper.SelectScalar(String.Format("SELECT COUNT(*) FROM JackpotPvpStages WHERE Name='{0}'", stageName)));
        if (objectsFound > 0)
            return true;

        return false;
    }

    #endregion

}
