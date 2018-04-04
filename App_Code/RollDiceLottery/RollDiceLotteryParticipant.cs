using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;

namespace Titan.RollDiceLottery
{
    public enum ParticipantStatus
    {
        Active = 1,
        Recorded = 2
    }

    public class RollDiceLotteryParticipant : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "RollDiceLotteryParticipants"; } }

        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _userId; } set { _userId = value; SetUpToDateAsFalse(); } }

        [Column("DateOccured")]
        public DateTime DateOccured { get { return _dateOccured; } set { _dateOccured = value; SetUpToDateAsFalse(); } }

        [Column("Score")]
        public int Score { get { return _score; } set { _score = value; SetUpToDateAsFalse(); } }

        [Column("GameTime")]
        public int GameTime { get { return _gameTime; } set { _gameTime = value; SetUpToDateAsFalse(); } }

        [Column("NumberOfRolls")]
        public int NumberOfRolls { get { return _numberOfRolls; } set { _numberOfRolls = value; SetUpToDateAsFalse(); } }

        [Column("StatusInt")]
        protected int StatusInt { get { return _statusInt; } set { _statusInt = value; SetUpToDateAsFalse(); } }

        public ParticipantStatus Status { get { return (ParticipantStatus)StatusInt; } set { StatusInt = (int)value; } }

        private int _id, _userId, _score, _gameTime, _numberOfRolls, _statusInt;
        private DateTime _dateOccured;

        public RollDiceLotteryParticipant() : base() { }

        public RollDiceLotteryParticipant(int id) : base(id) { }

        public RollDiceLotteryParticipant(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public static List<RollDiceLotteryParticipant> GetAllActiveParticipants()
        {
            var query = string.Format("SELECT * FROM {0} WHERE [StatusInt] = {1}", TableName, (int)ParticipantStatus.Active);
            return TableHelper.GetListFromRawQuery<RollDiceLotteryParticipant>(query);
        }
    }
}