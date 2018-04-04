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
    public class PtcAdvertDimensions : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PtcAdvertDimensions"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("Width")]
        public int Width { get { return _Width; } set { _Width = value; SetUpToDateAsFalse(); } }

        [Column("Height")]
        public int Height { get { return _Height; } set { _Height = value; SetUpToDateAsFalse(); } }

        [Column("Price")]
        public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        public int StatusInt { get { return _status; } set { _status = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        private int _Id, _Width, _Height, _status;
        private Money _Price;

        #endregion

        #region Constructors

        public PtcAdvertDimensions()
            : base()
        {

        }
        public PtcAdvertDimensions(int id)
            : base(id)
        {

        }
        public PtcAdvertDimensions(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {

        }


        #endregion

        public static List<PtcAdvertDimensions> GetActiveDimensions()
        {
            var whereDict = TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active);
            return TableHelper.SelectRows<PtcAdvertDimensions>(whereDict);
        }


    }
}