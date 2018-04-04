using Prem.PTC;
using System;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Titan.InvestmentPlatform
{ 
    public enum TicketStatus
    {
        [Description("Waiting")]
        WaitingInQueue = 0,
        [Description("Depositing")]
        ReceivingMoney = 1,
        [Description("Received")]
        Finished = 2
    }

    public class InvestmentTicket : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "InvestmentTickets"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string Date = "Date";
            public const string UserId = "UserId";
            public const string UserPlanId = "UserPlanId";
            public const string Level = "Level";
            public const string TicketNumber = "TicketNumber";
            public const string StatusInt = "StatusInt";
            public const string LevelPrice = "LevelPrice";
            public const string LevelFee = "LevelFee";
            public const string LevelEarnings = "LevelEarnings";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserPlanId)]
        public int UserPlanId { get { return planId; } set { planId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Level)]
        public int Level { get { return level; } set { level = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserId)]
        public int UserId { get { return userId; } set { userId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Date)]
        public DateTime Date { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TicketNumber)]
        public int TicketNumber { get { return ticketNumber; } set { ticketNumber = value; SetUpToDateAsFalse(); } }

        [Column(Columns.StatusInt)]
        protected int StatusInt { get { return statusInt; } set { statusInt = value; SetUpToDateAsFalse(); } }

        public TicketStatus Status
        {
            get { return (TicketStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }
        
        [Column(Columns.LevelPrice)]
        public Money LevelPrice { get { return levelPrice; } set { levelPrice = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LevelFee)]
        public Money LevelFee { get { return fee; } set { fee = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LevelEarnings)]
        public Money LevelEarnings { get { return levelEarnings; } set { levelEarnings = value; SetUpToDateAsFalse(); } }

        private string name, color;
        private int id, planId, ticketNumber, statusInt, userId, level;
        private DateTime date;
        private Money levelPrice, fee, levelEarnings;

        public InvestmentTicket() : base() { }
        public InvestmentTicket(int id) : base(id) { }
        public InvestmentTicket(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public string GenerateTicketNumber()
        {
            var sb = new StringBuilder();
            var resetNumber = TicketNumber / 10000;
            var displayedTiccektNo = TicketNumber - resetNumber * 10000;

            sb.Append(string.Format("L{0}-", FormText(level, 2)));
            sb.Append(string.Format("R{0}-", FormText(resetNumber, 2)));
            sb.Append(string.Format("{0}-{1}-{2}-", FormText(Date.AddYears(-2000).Year, 2), FormText(Date.Month, 2), FormText(Date.Day, 2)));
            sb.Append(FormText(displayedTiccektNo, 4));

            return sb.ToString();
        }        

        public static InvestmentTicket GetTicket(int userId, int userPlanId)
        {
            var query = string.Format("SELECT TOP 1 * FROM {0} WHERE {1} = {2} AND {3} = {4} ORDER BY {5} DESC",
                TableName, Columns.UserPlanId, userPlanId, Columns.UserId, userId, Columns.Date);

            return TableHelper.GetListFromRawQuery<InvestmentTicket>(query)[0]; 
        }

        private string FormText(int number, int maxLength)
        {
            return number.ToString().PadLeft(maxLength, '0');
        }
    }
}