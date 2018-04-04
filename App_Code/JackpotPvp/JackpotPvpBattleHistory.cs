using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public class JackpotPvpBattleHistory : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "JackpotPvpBattleHistory"; } }
    protected override string dbTable { get { return TableName; } }


    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("StageId")]
    public int StageId { get { return _StageId; } set { _StageId = value; SetUpToDateAsFalse(); } }

    [Column("WinnerId")]
    public int WinnerId { get { return _WinnerId; } set { _WinnerId = value; SetUpToDateAsFalse(); } }

    [Column("LoserId")]
    public int LoserId { get { return _LoserId; } set { _LoserId = value; SetUpToDateAsFalse(); } }

    private int _id, _StageId, _WinnerId, _LoserId;

    #endregion

    #region Constructors

    public JackpotPvpBattleHistory() : base() { }

    public JackpotPvpBattleHistory(int id) : base(id) { }

    public JackpotPvpBattleHistory(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

    #endregion

    #region Helpers

    public static int GetAmountOfWonBattles(int userId, int stageId)
    {
        String Query = String.Format("SELECT COUNT(*) FROM JackpotPvpBattleHistory WHERE StageId={0} AND WinnerId={1}", stageId, userId);
        return Convert.ToInt32(TableHelper.SelectScalar(Query));
    }

    public static int GetAmountOfLostBattles(int userId, int stageId)
    {
        String Query = String.Format("SELECT COUNT(*) FROM JackpotPvpBattleHistory WHERE StageId={0} AND LoserId={1}", stageId, userId);
        return Convert.ToInt32(TableHelper.SelectScalar(Query));
    }

    public static void AddNewHistoryBattle(int stageId, int winnerId, int loserId)
    {
        var NewBattleToHistory = new JackpotPvpBattleHistory();
        NewBattleToHistory.StageId = stageId;
        NewBattleToHistory.WinnerId = winnerId;
        NewBattleToHistory.LoserId = loserId;
        NewBattleToHistory.Save();
    }

    #endregion

}
