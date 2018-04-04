using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public class JackpotPvpStageBought : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "JackpotPvpStagesBought"; } }
    protected override string dbTable { get { return TableName; } }


    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("StageId")]
    public int StageId { get { return _StageId; } set { _StageId = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("DateOfBuy")]
    public DateTime DateOfBuy { get { return _DateOfBuy; } set { _DateOfBuy = value; SetUpToDateAsFalse(); } }

    [Column("BattlesDone")]
    public int BattlesDone { get { return _BattlesDone; } set { _BattlesDone = value; SetUpToDateAsFalse(); } }

    [Column("Active")]
    public bool IsActive { get { return _IsActive; } set { _IsActive = value; SetUpToDateAsFalse(); } }

    private int _id, _StageId, _UserId, _BattlesDone;
    private DateTime _DateOfBuy;
    private bool _IsActive;

    #endregion

    #region Constructors
    public JackpotPvpStageBought() : base() { }

    public JackpotPvpStageBought(int id) : base(id) { }

    public JackpotPvpStageBought(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

    #endregion

    #region Helpers

    public static int GetSumOfBattlesDoneForStage(int userId, int stageId)
    {
        String Query = String.Format("SELECT SUM(BattlesDone) FROM JackpotPvpStagesBought WHERE UserId={0} AND StageId={1}", userId, stageId);
        return Convert.ToInt32(TableHelper.SelectScalar(Query));
    }

    public static int GetAmountOfStagesBought(int userId, int stageId)
    {
        String Query = String.Format("SELECT COUNT(*) FROM JackpotPvpStagesBought WHERE UserId={0} AND StageId={1}", userId, stageId);
        return Convert.ToInt32(TableHelper.SelectScalar(Query));
    }

    public static void IncreaseBattlesCounterFotStage(int userId, int stageId)
    {
        String Query = String.Format("SELECT Id FROM JackpotPvpStagesBought WHERE UserId={0} AND StageId={1} AND Active=1", userId, stageId);
        var PlayedStageId = Convert.ToInt32(TableHelper.SelectScalar(Query));
        var PlayedStage = new JackpotPvpStageBought(PlayedStageId);

        PlayedStage.BattlesDone++;

        if (PlayedStage.BattlesDone >= JackpotPvpManager.BattlesAmountPerStage)
            PlayedStage.IsActive = false;

        PlayedStage.Save(); 
    }

    public static void AddNewStageBought(int stageId, int userId)
    {
        JackpotPvpStageBought NewBuy = new JackpotPvpStageBought();
        NewBuy.StageId = stageId;
        NewBuy.UserId = userId;
        NewBuy.DateOfBuy = DateTime.Now;
        NewBuy.BattlesDone = 0;
        NewBuy.IsActive = true;
        NewBuy.Save();
    }

    public static bool CheckIfThereAreBattlesLeftForThisStage(int userId, int stageId, int battlesAmountPerStage)
    {
        int TotalBattlesForSelectedStage = JackpotPvpStageBought.GetSumOfBattlesDoneForStage(userId, stageId);
        int TotalAmountOfStagesSameAsSelectedStage = JackpotPvpStageBought.GetAmountOfStagesBought(userId, stageId);

        if (TotalBattlesForSelectedStage < TotalAmountOfStagesSameAsSelectedStage * battlesAmountPerStage)
            return true;

        return false;
    }

    #endregion

}
