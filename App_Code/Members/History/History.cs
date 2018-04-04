using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Titan;
using Prem.PTC.Contests;
using Resources;

namespace Prem.PTC.Members
{
    /// <summary>
    /// Summary description for History
    /// </summary>
    public partial class History : BaseTableObject, IShoutboxContent
    {
        public static new string SELECT_SQL_COMMAND { get { return "SELECT TOP 50 * FROM History WHERE Type IN ({0}) ORDER BY Date DESC"; } }

        #region Columns
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "History"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("AssignedUsername")]
        public string AssignedUsername { get { return username; } set { username = value; SetUpToDateAsFalse(); } }

        [Column("Type")]
        protected int IntType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("Date")]
        public DateTime Date { get { return _sentDate; } set { _sentDate = value; SetUpToDateAsFalse(); } }

        [Column("IsRead")]
        public bool IsRead { get { return _isRead; } set { _isRead = value; SetUpToDateAsFalse(); } }

        [Column("Text")]
        public string Text { get { return text; } set { text = parseLength(value, 299); ; SetUpToDateAsFalse(); } }

        private int _id, type;
        private string username, text;
        private DateTime _sentDate;
        private bool _isRead;

        private string parseLength(string Input, int Allowed)
        {
            if (Input == null)
                return Input;

            if (Input.Length > Allowed)
                return Input.Substring(0, Allowed - 1);
            return Input;
        }

        #endregion Columns

        public DateTime SentDate { get { return _sentDate; } set { _sentDate = value; SetUpToDateAsFalse(); } }
        public string Username { get { return username; } set { username = value; SetUpToDateAsFalse(); } }

        public History()
            : base()
        { }

        public History(int id) : base(id) { }

        public History(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        public HistoryType Type
        {
            get
            {
                return (HistoryType)IntType;
            }

            set
            {
                IntType = (int)value;
            }
        }

        public string Message
        {
            get
            {
                return GetFullText();
            }
            set
            {
                //Only for IShoutBoxContent compatibility
            }
        }

        public string GetFullText()
        {
            if (Type == HistoryType.Contest)
            {
                try
                {
                    return ContestManager.ReparseId(Text);
                }
                catch (Exception ex) { }
            }

            if (Type != HistoryType.None)
                return GetAssociatedResource(Type) + " <i>" + Text + "</i>";

            //From 2502

            try
            {
                string[] texts = Text.Split('^');
                return texts[0] + " <i>" + texts[1] + "</i>";
            }
            catch (Exception ex)
            {
                return Text;
            }
        }


        private string GetAssociatedResource(HistoryType Type)
        {
            switch (Type)
            {
                case HistoryType.Cashout:
                    return Resources.L1.HISTORY_1;

                case HistoryType.Purchase:
                    return Resources.L1.HISTORY_2;

                case HistoryType.Registration:
                    return Resources.L1.HISTORY_3;

                case HistoryType.Transfer:
                    return Resources.L1.HISTORY_4;

                case HistoryType.UpgradeExpiration:
                    return Resources.L1.HISTORY_5;

                case HistoryType.LevelExpiration:
                        return Resources.U5008.LEVELDOWNGRADED;

                case HistoryType.UpgradePurchase:
                    if (AppSettings.Points.LevelMembershipPolicyEnabled)
                        return U5008.UPGRADED;
                    return Resources.L1.HISTORY_6;

                case HistoryType.Offerwalls:
                    return Resources.L1.HISTORY_7;

                case HistoryType.OfferwallRevers:
                    return Resources.U4100.OFFERWALL_REV;

                case HistoryType.TrafficGrid:
                    return Resources.L1.HISTORY_8;

                case HistoryType.Contest:
                    return Resources.L1.HISTORY_9;

                case HistoryType.CPAOffer:
                    return Resources.L1.HISTORY_10;

                case HistoryType.CPAOfferDenied:
                    return Resources.L1.HISTORY_11;

                case HistoryType.CashoutRejection:
                    return Resources.U2502.REJECTEDCR;

                case HistoryType.MoneyReturned:
                    return Resources.U2502.RETURNEDBACK;

                case HistoryType.LockedFunds:
                    return Resources.U4001.FUNDSLOCKED;

                //CLP

                case HistoryType.EliteStatus:
                    return "You have reveived Elite status";

                case HistoryType.PrestigeMode:
                    return "You are now in Prestige Mode";

                case HistoryType.JackpotWin:
                    return U5003.YOUWONJACKPOT;

                case HistoryType.BurnCardWin:
                    return "You won the BurnCard: ";

                //4000

                case HistoryType.BidPlaced:
                    return Resources.U4000.BIDPLACED + ": ";

                case HistoryType.AuctionWon:
                    return Resources.U4000.YOUWONAUCTION + " ";

                case HistoryType.Edit:
                    return U5008.EDITED;

                case HistoryType.SlotChancesWon:
                    return "You got Slot Machine chances: ";

                case HistoryType.SlotMachinePayout:
                    return "You won " + AppSettings.PointsName + " on Slot Machine: ";

                default:
                    return "No history resource provided";

                //InvestmentPlatform

                case HistoryType.InvestmentPlatformDailyCredit:
                    return "";

                case HistoryType.InvestmentPlatformBonus:
                    return "";

                case HistoryType.InvestmentPlatformReferralCommission:
                    return "";

                case HistoryType.InvestmentPlatformSpeedUpBonus:
                    return "";
            }
        }

        public static void AddEntry(string username, HistoryType Type, string text)
        {
            History Entry = new History();
            Entry.Type = Type;
            Entry.Date = DateTime.Now;
            Entry.IsRead = false;
            Entry.AssignedUsername = username;
            Entry.Text = text;
            Entry.Save();
        }

        public static void AddJackpotWin(string username, string description, string ticketId, string jackpotName)
        {
            AddEntry(username, HistoryType.JackpotWin, "#" + ticketId + " (" + jackpotName + ": " + description + ")");
        }

        public static void AddPurchase(string username, Money money, string what)
        {
            AddEntry(username, HistoryType.Purchase, what + " (" + money.ToString() + ")");
        }

        public static void AddPurchase(string username, int pointsValue, string what)
        {
            AddEntry(username, HistoryType.Purchase, what + " (" + pointsValue + ")");
        }

        public static void AddEdit(string username, Money money, string what)
        {
            AddEntry(username, HistoryType.Edit, what + " (" + money.ToString() + ")");
        }

        public static void AddCashout(string username, Money money)
        {
            AddEntry(username, HistoryType.Cashout, money.ToString());
        }

        public static void AddRegistration(string username)
        {
            AddEntry(username, HistoryType.Registration, username + "!");
        }

        public static void AddTransfer(string username, Money money, string from, string to, bool IsPointsTransfer = false)
        {
            if (IsPointsTransfer)
                AddEntry(username, HistoryType.Transfer, money.GetRealTotals().ToString() + " " + AppSettings.PointsName + " " + Resources.L1.FROM + " " + from + " " + Resources.L1.TO + "  " + to);
            else
                AddEntry(username, HistoryType.Transfer, money.ToString() + " " + Resources.L1.FROM + " " + from + " " + Resources.L1.TO + "  " + to);
        }

        public static void AddUpgradePurchase(string username, string upgradename, Money money)
        {
            AddEntry(username, HistoryType.UpgradePurchase, "(" + upgradename + ") " + Resources.L1.FORPURCHASED + " " + money.ToString());
        }

        public static void AddLevelUp(string username, string upgradedTo)
        {
            AddEntry(username, HistoryType.UpgradePurchase, upgradedTo);
        }

        public static void AddUpgradeExpiration(string username, string upgradename)
        {
            AddEntry(username, HistoryType.UpgradeExpiration, "(" + upgradename + ")");
        }

        public static void AddLevelExpiration(string username, string upgradename)
        {
            AddEntry(username, HistoryType.LevelExpiration, upgradename);
        }

        public static void AddOfferwalCompleted(string username, string offerwallname, Money value, CreditAs As)
        {
            string amount = value.ToString();
            if (As == CreditAs.Points)
                amount = value.GetRealTotals().ToString() + " " + AppSettings.PointsName;


            AddEntry(username, HistoryType.Offerwalls, " " + amount + " (" + offerwallname + ")");
        }

        public static void AddOfferwalRevereseCompleted(string username, string offerwallname, Money value, CreditAs As)
        {
            string amount = value.ToString();
            if (As == CreditAs.Points)
                amount = value.GetRealTotals().ToString() + " " + AppSettings.PointsName;


            AddEntry(username, HistoryType.OfferwallRevers, " " + amount + " (" + offerwallname + ")");
        }

        public static void AddOfferLocked(string username, string networkName, string offerName, Money value, CreditAs As)
        {
            string amount = value.ToString();
            if (As == CreditAs.Points)
                amount = value.GetRealTotals().ToString() + " " + AppSettings.PointsName;


            if (!string.IsNullOrWhiteSpace(offerName))
                networkName = networkName + ": ";

            AddEntry(username, HistoryType.LockedFunds, " (" + networkName + offerName + "), " + amount);
        }

        public static void AddTrafficGridWin(string username, string what)
        {
            AddEntry(username, HistoryType.TrafficGrid, " (" + what + ")");
        }

        public static void AddContestWin(string username, int place, string what)
        {
            AddEntry(username, HistoryType.Contest, " (" + place + ": " + what + ")");
        }

        public static void AddCPAOfferCompleted(string username, string title, string money)
        {
            AddEntry(username, HistoryType.CPAOffer, " (" + title + ": " + money + ")");
        }

        public static void AddCPAOfferCompleted(string username, string title, string money, string network, int offerid)
        {
            if (!string.IsNullOrWhiteSpace(network))
                network = network + ": ";
            var text = string.Empty;

            if (AppSettings.Shoutbox.ShowCPANetworkAndLink)
                text = " (" + network + title + " for " + money +
                "[<a href=\"" + AppSettings.Site.Url + "user/earn/cpaoffers.aspx?o=" + offerid + "\">"
                + Resources.U4000.CLICKTOCOMPLETE + "</a>])";
            else
                text = " (" + title + " for " + money + ")";

            AddEntry(username, HistoryType.CPAOffer, text);
        }

        public static void AddCPAOfferDeniedUnder(string username, string title, string network = "")
        {
            if (!string.IsNullOrWhiteSpace(network))
                network = network + ": ";

            AddEntry(username, HistoryType.CPAOfferDenied, " (" + network + title + ")");
        }

        public static void Add(string username, string unbold, string bold)
        {
            AddEntry(username, HistoryType.None, unbold + "^" + bold);
        }

        public static void Add(string username, string text)
        {
            Add(username, text, "");
        }

        public static void AddCashoutRejection(string username, string money)
        {
            AddEntry(username, HistoryType.CashoutRejection, money);
            AddEntry(username, HistoryType.MoneyReturned, money);
        }

        public static void AddBidPlaced(string username, string money)
        {
            AddEntry(username, HistoryType.BidPlaced, money);
        }

        public static void AddAuctionWon(string username, string auctionTime)
        {
            AddEntry(username, HistoryType.AuctionWon, auctionTime);
        }

        public static void AddInvestmentLevelCashout(string username, Money money)
        {
            AddEntry(username, HistoryType.InvestmentLevelCashout, money.ToString());
        }

        public static List<History> GetLatestRecords(int count)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var parser = bridge.Instance;
                var types = "4, 7, 8 , 9, 10, 17, 25, 26";

                if (AppSettings.Shoutbox.ShowLastRegistrations)
                    types += ", 1";

                DataTable dt = parser.ExecuteRawCommandToDataTable(string.Format(SELECT_SQL_COMMAND, types));

                return TableHelper.GetListFromDataTable<History>(dt, count);
            }
        }

        public bool IsEvent
        {
            get
            {
                return true;
            }
        }
    }
}