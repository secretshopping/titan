using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Members;

namespace Prem.PTC.Contests
{
    public class ContestParticipant : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ContestParticipants"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("ContestId")]
        public int ContestId { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("LatestAction")]
        public DateTime LatestAction { get { return _DateStart; } set { _DateStart = value; SetUpToDateAsFalse(); } }

        [Column("Points")]
        public Money Points { get { return r4; } set { r4 = value; SetUpToDateAsFalse(); } }

        private int _id, quantity, type;
        private string name;
        private Money r4;
        private DateTime _DateStart;

        #endregion Columns

        public ContestParticipant()
            : base() { }

        public ContestParticipant(int id) : base(id) { }

        public ContestParticipant(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        /// <summary>
        /// WARNING: Participant must exists!
        /// </summary>
        /// <param name="contest"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        public static ContestParticipant GetParticipant(Contest contest, string Username)
        {
            Member User = new Member(Username);
            var where = TableHelper.MakeDictionary("ContestId", contest.Id);

            if (User.Status == MemberStatus.Active || User.Status == MemberStatus.VacationMode || User.Status == MemberStatus.AwaitingSMSPIN)
                where.Add("Username", Username);

            var result = TableHelper.SelectRows<ContestParticipant>(where);

            return result[0];
        }

        /// <summary>
        /// Also updates last activity date
        /// </summary>
        /// <param name="Amount"></param>
        private void Credit(int Amount)
        {
            Points += new Money(Amount);
            LatestAction = DateTime.Now;
        }

        /// <summary>
        /// Also updates last activity date
        /// </summary>
        /// <param name="Amount"></param>
        private void Credit(Money Amount)
        {
            Points += Amount;
            LatestAction = DateTime.Now;
        }

        /// <summary>
        /// Credits the participant and update LastAction time. Needs to be saved
        /// </summary>
        /// <param name="IntAmount"></param>
        /// <param name="MoneyAmount"></param>
        public void Credit(int IntAmount, Money MoneyAmount)
        {
            if (MoneyAmount != null)
                Credit(MoneyAmount);
            else
                Credit(IntAmount);
        }
    }
}