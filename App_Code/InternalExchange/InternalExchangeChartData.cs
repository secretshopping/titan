using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ExtensionMethods;

namespace Titan.ICO
{
    //NOT NORMAL TABLE
    [Serializable]
    public class InternalExchangeChartData : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "TableDontExists"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "TmpId ";
            public const string GraphGroupId = "GraphGroupId";
            public const string OpenValue = "OpenValue";
            public const string CloseValue = "CloseValue";
            public const string MinValue = "MinValue";
            public const string MaxValue = "MaxValue";
            public const string Volume = "Volume";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.GraphGroupId)]
        public float GraphGroupId { get { return _graphGroupId; } set { _graphGroupId = float.Parse(value.ToString().Replace(',','.')); SetUpToDateAsFalse(); } }

        [Column(Columns.OpenValue)]
        public decimal OpenValue { get { return _openValue; } set { _openValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CloseValue)]
        public decimal CloseValue { get { return _closeValue; } set { _closeValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MinValue)]
        public decimal MinValue { get { return _minValue; } set { _minValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MaxValue)]
        public decimal MaxValue { get { return _maxValue; } set { _maxValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Volume)]
        public decimal Volume { get { return _volume; } set { _volume = value; SetUpToDateAsFalse(); } }

        private int _id;
        private float _graphGroupId;
        private decimal _openValue, _closeValue, _minValue, _maxValue, _volume;

        public InternalExchangeChartData() : base() { }
        public InternalExchangeChartData(int id) : base(id) { }
        public InternalExchangeChartData(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public static String GetChartData()
        {
            var newList = new List<InternalExchangeChartData>();

            string SQL_COMMAND = String.Format(
                @"SELECT
	                ROW_NUMBER() OVER(ORDER BY GraphGroupId DESC) AS TmpId
	                , GraphGroupId
                    , LAG(CloseValue, 1, OpenValue) OVER(ORDER BY GraphGroupId ASC) AS OpenValue
	                , CloseValue
	                , MinValue
	                , MaxValue
	                , Volume

                FROM
                (
                SELECT DISTINCT
	                GraphGroupId,
	                FIRST_VALUE(TransactionValue) OVER(PARTITION BY GraphGroupId ORDER BY GraphGroupId DESC) AS OpenValue,
	                LAST_VALUE(TransactionValue) OVER(PARTITION BY GraphGroupId ORDER BY GraphGroupId DESC) AS CloseValue,
	                MIN(TransactionValue) OVER(PARTITION BY GraphGroupId ORDER BY GraphGroupId DESC) AS MinValue,
	                MAX(TransactionValue) OVER(PARTITION BY GraphGroupId ORDER BY GraphGroupId DESC) AS MaxValue,
	                SUM(TransactionAmount) OVER(PARTITION BY GraphGroupId ORDER BY GraphGroupId DESC) AS Volume
                FROM 
	                InternalExchangeTransactions
                WHERE 
	                TransactionDate > '{0}'
                ) allData
                ORDER BY
	                GraphGroupId DESC", AppSettings.ServerTime.AddHours(-24).ToDBString());


            DataTable dt;

            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                dt = bridge.Instance.ExecuteRawCommandToDataTable(SQL_COMMAND);
            }

            String StringChartData = "Date,Open,High,Low,Close,Volume\n\r";

            // #0
            //BASE DATA INIT
            int GroupsAmount = 99;
            //check year
            DateTime OpenDate = AppSettings.ServerTime.AddHours(-24);

            // #1
            //LoadFastGrups
            List<string> ExistingGroups = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                ExistingGroups.Add(ReturnDateFromChartDataRow(((double)row.ItemArray[1]).ToString()));
            }

            int     StartDateYear   = Int32.Parse(OpenDate.ToString("yy"));
            int     StartDateFullYear = Int32.Parse(OpenDate.ToString("yyyy"));
            String  StartDateMonth  = OpenDate.ToString("MMM");
            int     StartDateIntMonth = OpenDate.Month;
            int     StartDateDay    = Int32.Parse(OpenDate.ToString("dd"));
            int     StartDateHour   = OpenDate.Hour;
            int     DaysInCurrentMonth = DateTime.DaysInMonth(OpenDate.Year, OpenDate.Month);

            decimal LastStockValue = InternalExchangeTransaction.GetLastStockValueBefore24h();
            bool    existsCustomRow = false;

            for (int groupNr=0; groupNr<GroupsAmount; groupNr++)
            {
                //UP HOUR
                if (groupNr != 0 && groupNr % 4 == 0) StartDateHour++;

                String formattedDate = String.Format("{0}-{1}-{2}T{3}:{4}", String.Format("{0}{1}", StartDateDay < 10 ? "0" : "", StartDateDay), 
                                                                            StartDateMonth,
                                                                            String.Format("{0}{1}", StartDateYear < 10 ? "0" : "", StartDateYear),
                                                                            String.Format("{0}{1}", StartDateHour < 10 ? "0" : "", StartDateHour), 
                                                                            (groupNr%4*15).ToString("00"));
                if (ExistingGroups.Contains(formattedDate))
                {
                    DataRow row = GetDataRow(dt.Rows, String.Format("{0}{1}{2}{3}{4}",
                                                                                        StartDateFullYear,
                                                                                        String.Format("{0}{1}", StartDateIntMonth<10? "0" : "", StartDateIntMonth),
                                                                                        String.Format("{0}{1}", StartDateDay < 10 ? "0" : "", StartDateDay),
                                                                                        String.Format("{0}{1}", StartDateHour < 10 ? "0" : "", StartDateHour),
                                                                                        groupNr%4==0 ? "" : String.Format(".0{0}", groupNr%4) ));
                    //[1]-date, [2]-Open, [3]-Close, [4]-Low, [5]-High, [6]-Volume
                    StringChartData += String.Format("\n{0},{1},{2},{3},{4},{5}",
                                                        ReturnDateFromChartDataRow(((double)row.ItemArray[1]).ToString()),
                                                        existsCustomRow ? ((Decimal)row.ItemArray[2]).ToString("G0") : LastStockValue.ToString("G0"),
                                                        ((Decimal)row.ItemArray[5]).ToString("G0"),
                                                        ((Decimal)row.ItemArray[4]).ToString("G0"),
                                                        ((Decimal)row.ItemArray[3]).ToString("G0"),
                                                        ((Decimal)row.ItemArray[6]).ToString("G0"));

                    LastStockValue = (Decimal)row.ItemArray[3];
                    existsCustomRow = true;
                }
                else
                    StringChartData += String.Format("\n{0},{1},{2},{3},{4},{5}", formattedDate, LastStockValue, LastStockValue, LastStockValue, LastStockValue, 0);

                if(StartDateHour == 24)
                {
                    StartDateHour = 0;
                    //If need to up month
                    if (StartDateDay >= DaysInCurrentMonth)
                    {
                        //If December, add yes
                        if (OpenDate.Month == 12)
                        {
                            StartDateYear++;
                            StartDateFullYear++;
                        }
                            
                        StartDateMonth = OpenDate.AddMonths(1).ToString("MMM");
                        StartDateIntMonth++;
                        if (StartDateIntMonth == 13) StartDateIntMonth = 1;

                        StartDateDay = 1;
                    }
                    else
                        StartDateDay++;
                }  
            }

            return StringChartData + ";";
        }

        private static DataRow GetDataRow(DataRowCollection collection, String group)
        {
            foreach(DataRow row in collection)
            {
                if (row.ItemArray[1].ToString() == group)
                    return row;
            }
            throw new MsgException("Unexpected group");
        }

        private static String ReturnDateFromChartDataRow(String dataCell)
        {
            String[] dateEncode = dataCell.Split('.');

            String rawDate = dateEncode[0];
            String minute = dateEncode.Length > 1 ? dateEncode[1] : "00";
            String hour = rawDate.Substring(rawDate.Length-2);

            minute = (Int32.Parse(minute) * 15).ToString("00");

            DateTime refactoredDate = DateTime.ParseExact(rawDate.Remove(rawDate.Length - 2), "yyyyMMdd", new System.Globalization.CultureInfo("en-US"));
            string result = String.Format("{0}T{1}:{2}", refactoredDate.ToString("dd-MMM-yy"), hour, minute);

            return result;
        }
    }
}
