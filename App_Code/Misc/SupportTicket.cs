using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Prem.PTC
{
    /// <summary>
    /// Represents single instance of Support ticket.
    /// 
    /// </summary>
    public class SupportTicket : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return AppSettings.TableNames.SupportTickets; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns

        public static class Columns
        {
            public const string Id = "SupportTicketId";
            public const string From = "FromUsername";
            public const string Subject = "Subject";
            public const string Body = "Body";
            public const string Date = "Date";
            public const string IsRead = "IsRead";
            public const string IsReplied = "IsReplied";
            public const string IsSolved = "IsSolved";
            public const string BrowserAndSystem = "BrowserAndSystem";
            public const string ReplySubject = "ReplySubject";
            public const string ReplyBody = "ReplyBody";
            public const string ReplyDate = "ReplyDate";
            public const string FullName = "FullName";
            public const string PhoneNumber = "PhoneNumber";
            public const string TicketDepartmentId = "TicketDepartmentId";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.From)]
        private string FromUsername { get { return _fromUsername; } set { _fromUsername = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Subject)]
        public string Subject { get { return _subject; } set { _subject = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Body)]
        public string Body { get { return _body; } set { _body = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Date)]
        public DateTime Date { get { return _date; } set { _date = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsRead)]
        public bool IsRead { get { return _isRead; }  set { _isRead = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsReplied)]
        public bool IsReplied { get { return _isReplied; } private set { _isReplied = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsSolved)]
        public bool IsSolved { get { return _isSolved; } set { _isSolved = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BrowserAndSystem)]
        public string BrowserAndSystem { get { return _browserAndSystem; } set { _browserAndSystem = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ReplySubject)]
        public string ReplySubject { get { return _replySubject; } private set { _replySubject = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ReplyBody)]
        public string ReplyBody { get { return _replyBody; } private set { _replyBody = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ReplyDate)]
        public DateTime? ReplyDate { get { return _replyDate; } private set { _replyDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FullName)]
        public string FullName { get { return _fullName; } private set { _fullName = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PhoneNumber)]
        public string PhoneNumber { get { return _phoneNumber; } private set { _phoneNumber = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TicketDepartmentId)]
        public int TicketDepartmentId { get { return _ticketDepartmentId; } set { _ticketDepartmentId = value; SetUpToDateAsFalse(); } }

        private int _id, _ticketDepartmentId;
        private bool _isRead, _isReplied, _isSolved;
        private string _fromUsername, _subject, _body, _browserAndSystem, _replySubject, _replyBody, _fullName, _phoneNumber;
        private DateTime _date;
        private DateTime? _replyDate;
        #endregion

        private Member _from = null;
        public Member From
        {
            get
            {
                if (_from == null || FromUsername != _from.Name)
                    if (Member.Exists(FromUsername))
                        _from = new Member(FromUsername);

                return _from;
            }
        }

        /// <summary>
        /// Creates support ticket basing on id
        /// </summary>
        /// <param name="id">Id of support ticket</param>
        public SupportTicket(int id) : base(id) { }

        /// <summary>
        /// Creates support ticket basing on delivered DataRow
        /// </summary>
        /// <param name="row">Row of db table :)</param>
        /// <param name="isUpToDate">True if row contains the same data
        /// which is in database</param>
        public SupportTicket(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public SupportTicket(Member from, string browserAndSystem = "")
            : base()
        {
            _from = from;
            FromUsername = from.Name;
            Date = AppSettings.ServerTime;
            IsRead = IsReplied = IsSolved = false;
            BrowserAndSystem = browserAndSystem;
        }

        //public SupportTicket(Member from, string subject, string body, string browserAndSystem = "", string name = "", string phoneNumber = "")
        //    : this(from, browserAndSystem)
        //{
        //    Subject = subject;
        //    Body = body;
        //    FullName = name;
        //    PhoneNumber = phoneNumber;
        //}

        public SupportTicket(Member from, string subject, string body, int ticketDepartmentId = SupportTicketDepartment.DefaultDepartament, string browserAndSystem = "", string name = "", string phoneNumber = "")
            : this(from, browserAndSystem)
        {
            Subject = subject;
            Body = body;
            FullName = name;
            PhoneNumber = phoneNumber;
            TicketDepartmentId = ticketDepartmentId;
        }

        public void ReplyFromAdmin(string body, string adminName, bool sendEmail = false)
        {
            var supportReply = new SupportTicketReply();
            supportReply.Message = body;
            supportReply.TicketId = this.Id;
            supportReply.ReplyDate = AppSettings.ServerTime;
            supportReply.IsMember = false;
            supportReply.AdminName = adminName;
            supportReply.Save();

            this.IsRead = false;
            this.Save();

            if (sendEmail)
                sendEmailOnReply();
        }

        public void ReplyFromMember(string body)
        {
            var supportReply = new SupportTicketReply();
            supportReply.Message = body;
            supportReply.TicketId = this.Id;
            supportReply.ReplyDate = AppSettings.ServerTime;
            supportReply.IsMember = true;

            supportReply.Save();
        }

        private void sendEmailOnReply()
        {
            string userEmail = string.Empty;
            string ticketBody = "\n\n" + FromUsername + ":\n" + Body;
            ticketBody += "\n\n-------------------------------------\n\n";

            using (var br = ParserPool.Acquire(Database.Client))
            {
                var where = TableHelper.MakeDictionary(Member.Columns.Username, FromUsername);
                userEmail = br.Instance.Select(Member.TableName, Member.Columns.Email, where) as string;

                var querry = string.Format(@"SELECT * FROM {0} WHERE {1} = {2} ORDER BY {3} ASC", SupportTicketReply.TableName, SupportTicketReply.Columns.TicketId, Id, SupportTicketReply.Columns.ReplyDate);
                var tickets = TableHelper.GetListFromRawQuery<SupportTicketReply>(querry);

                for(int i = 0; i < tickets.Count-1; i++)
                {
                    ticketBody += AddTicketResponseToEmailBody(tickets[i]);
                }
                ticketBody += AddTicketResponseToEmailBody(tickets.Last(), false);
            }

            string subject = "Ticket response";

            Mailer.SendEmailToUser(userEmail, subject, ticketBody);
        }

        private string AddTicketResponseToEmailBody(SupportTicketReply ticket, bool endTag = true)
        {
            var body = string.Empty;

            if (ticket.IsMember)
                body += FromUsername + ":\n";
            else
                body += AppSettings.Site.Name + ":\n";
            body += ticket.Message;

            if (endTag)
                body += "\n\n-------------------------------------\n\n";
            else
                body += "\n\n";

            return body;
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                var isUpToDate = IsUpToDate;

                IsRead = true;
                PropertyBuilder<SupportTicket> builder = new PropertyBuilder<SupportTicket>();
                builder.Append(x => x.IsRead);

                SavePartially(isUpToDate, builder.Build());
            }
        }

        public static IList<SupportTicket> GetSupportTickets(Member member)
        {
            return GetSupportTickets(member.Name);
        }

        /// <remarks>Hardcoded column name!</remarks>
        public static IList<SupportTicket> GetSupportTickets(string memberUsername)
        {
            var whereUsername = TableHelper.MakeDictionary("FromUsername", memberUsername);

            var suportTickets = TableHelper.SelectRows<SupportTicket>(whereUsername);

            return (from supportTicket in suportTickets
                    orderby supportTicket.Date descending
                    select supportTicket).ToList();
        }
    }
}