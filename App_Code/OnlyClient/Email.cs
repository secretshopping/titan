using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Data;
using System.Net.Mail;
using Prem.PTC.Members;
using Prem.PTC.Offers;


namespace Prem.PTC.Misc
{
    public class Email : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "Emails"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "EmailId";
            public const string FromName = "FromName";
            public const string FromEmail = "FromEmail";
            public const string ToName = "ToName";
            public const string ToEmail = "ToEmail";
            public const string Subject = "Subject";
            public const string Body = "Body";
            public const string SentDate = "SentDate";
            public const string IsSingleRecipient = "IsSingleRecipient";
            public const string RecipientUsername = "RecipientUsername";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FromName)]
        public string FromName { get { return _fromName; } set { _fromName = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FromEmail)]
        public string FromEmail { get { return _fromEmail; } set { _fromEmail = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ToName)]
        public string ToName { get { return _toName; } set { _toName = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ToEmail)]
        public string ToEmail { get { return _toEmail; } set { _toEmail = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Subject)]
        public string Subject { get { return _subject; } set { _subject = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Body)]
        public string Body { get { return _body; } set { _body = value; SetUpToDateAsFalse(); } }

        [Column(Columns.SentDate)]
        public DateTime? Sent { get { return _sent; } set { _sent = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsSingleRecipient)]
        public bool IsSingleRecipient { get { return _isSingleRecipient; } set { _isSingleRecipient = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RecipientUsername)]
        public string RecipientUsername { get { return _recipientUsername; } set { _recipientUsername = value; SetUpToDateAsFalse(); } }

        [Column("IsGeolocated")]
        public bool IsGeolocated { get { return _IsGeolocated; } set { _IsGeolocated = value; SetUpToDateAsFalse(); } }

        [Column("Note")]
        public string Note { get { return _Note; } set { _Note = value; SetUpToDateAsFalse(); } }

        [Column("StatusInt")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        public GeolocationUnit GeolocationUnit { get; set; }

        public EmailStatus Status
        {
            get
            {
                return (EmailStatus)StatusInt;
            }
            set
            {
                StatusInt = (int)value;
            }
        }

        private int _id, _StatusInt;
        private DateTime? _sent;
        private bool _isSingleRecipient, _IsGeolocated;
        private string _fromName, _fromEmail, _toName, _toEmail, _subject, _body, _recipientUsername, _Note;

        #endregion

        #region Constructors

        public Email() : base() { }
        public Email(int id) : base(id) { }
        public Email(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion

    }
}