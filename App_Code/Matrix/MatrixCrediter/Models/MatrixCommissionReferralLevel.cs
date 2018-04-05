using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Matrix
{
    [Serializable]
    public class MatrixCommissionReferralLevel : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "MatrixCommissionReferralLevels"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("DirectRefCount")]
        public int DirectRefCount { get { return _DirectRefCount; } protected set { _DirectRefCount = value; SetUpToDateAsFalse(); } }

        [Column("CommissionPercent")]
        public decimal CommissionPercent
        {
            get { return _CommissionPercent; }
            protected set
            {
                if (value < 0)
                    throw new MsgException("Commission Percent cannot be lower than 0.");
                _CommissionPercent = value; SetUpToDateAsFalse();
            }
        }

        [Column("Status")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }


        #endregion Columns
        public UniversalStatus Status
        {
            get { return (UniversalStatus)StatusInt; }
            protected set { StatusInt = (int)value; }
        }

        int _Id, _DirectRefCount, _StatusInt;
        decimal _CommissionPercent;

        public MatrixCommissionReferralLevel(int id) : base(id) { }

        public MatrixCommissionReferralLevel(DataRow row, bool isUpToDate = true)
                        : base(row, isUpToDate) { }

        private MatrixCommissionReferralLevel(int directRefCount, decimal commissionPercent, UniversalStatus status)
        {
            DirectRefCount = directRefCount;
            CommissionPercent = commissionPercent;
            Status = status;
        }

        public void Activate()
        {
            Status = UniversalStatus.Active;
            this.Save();

            SetStatusesForLowerDirectRefCount(UniversalStatus.Active, this.DirectRefCount);
        }

        public void Pause()
        {
            Status = UniversalStatus.Paused;
            this.Save();

            SetStatusesForLowerDirectRefCount(UniversalStatus.Paused, this.DirectRefCount);
        }

        public override void Delete()
        {
            string query = string.Format("DELETE FROM MatrixCommissionReferralLevels WHERE DirectRefCount > {0}",
                                        this.DirectRefCount);
            TableHelper.ExecuteRawCommandNonQuery(query);

            base.Delete();
        }

        public void Update(decimal commissionPercent)
        {
            this.CommissionPercent = commissionPercent;
            this.Save();
        }

        public static void Create(decimal commissionPercent)
        {
            MatrixCommissionReferralLevel recordWithMaxDirectRefCount = GetRecordWithMaxDirectRefCount();
            MatrixCommissionReferralLevel newRecord;

            if (recordWithMaxDirectRefCount == null)
                newRecord = new MatrixCommissionReferralLevel(1, commissionPercent, UniversalStatus.Active);
            else
                newRecord = new MatrixCommissionReferralLevel(recordWithMaxDirectRefCount.DirectRefCount + 1, commissionPercent, recordWithMaxDirectRefCount.Status);

            newRecord.Save();
        }

        private void SetStatusesForLowerDirectRefCount(UniversalStatus status, int directRefCount)
        {
            string sign;
            if (status == UniversalStatus.Active)
                sign = "<";
            else if (status == UniversalStatus.Paused)
                sign = ">";
            else
                return;

            string query = string.Format("UPDATE MatrixCommissionReferralLevels SET Status = {0} WHERE DirectRefCount {1} {2}",
                (int)status, sign, directRefCount);
            TableHelper.ExecuteRawCommandNonQuery(query);
        }

        private static MatrixCommissionReferralLevel GetRecordWithMaxDirectRefCount()
        {
            string query = "SELECT TOP 1 * FROM MatrixCommissionReferralLevels ORDER BY DirectRefCount DESC";

            return TableHelper.GetListFromRawQuery<MatrixCommissionReferralLevel>(query).FirstOrDefault();
        }

        public static List<MatrixCommissionReferralLevel> GetActive()
        {
            string query = string.Format("SELECT * FROM MatrixCommissionReferralLevels WHERE Status = {0}", (int)UniversalStatus.Active);
            return TableHelper.GetListFromRawQuery<MatrixCommissionReferralLevel>(query);
        }
    }
}