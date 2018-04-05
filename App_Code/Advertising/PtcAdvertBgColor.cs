using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.ComponentModel;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using Prem.PTC.Offers;

namespace Prem.PTC.Advertising
{
    public class PtcAdvertBgColor : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PtcAdvertBgColors"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("Price")]
        public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        public int StatusInt { get { return _status; } set { _status = value; SetUpToDateAsFalse(); } }

        [Column("BgColor")]
        public string BgColor { get { return _BgColor; } set { _BgColor = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        private int _Id, _status;
        private Money _Price;
        private string _BgColor;

        #endregion

        #region Constructors

        public PtcAdvertBgColor()
            : base()
        {

        }
        public PtcAdvertBgColor(int id)
            : base(id)
        {

        }
        public PtcAdvertBgColor(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {

        }


        #endregion

        public static List<PtcAdvertBgColor> GetActiveBgColors()
        {
            var whereDict = TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active);
            return TableHelper.SelectRows<PtcAdvertBgColor>(whereDict);
        }


    }
}