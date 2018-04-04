using System;
using Prem.PTC;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Resources;

namespace Titan
{
    public class ShoutboxMessage : BaseTableObject, IShoutboxContent
    {
        #region Columns
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ShoutboxMessages"; } }
        public static new string SELECT_SQL_COMMAND { get { return "SELECT TOP 50 * FROM ShoutboxMessages WHERE IsDeleted = 0 ORDER BY SentDate DESC"; } }

        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("Message")]
        public string Message { get { return Message1; } set { Message1 = value; SetUpToDateAsFalse(); } }

        [Column("SentDate")]
        public DateTime SentDate { get { return d1; } set { d1 = value; SetUpToDateAsFalse(); } }

        [Column("IsDeleted")]
        public bool IsDeleted { get { return del; } set { del = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string name, Message1;
        private DateTime d1;
        private bool del;

        public bool IsEvent
        {
            get
            {
                return false;
            }
        }

        #endregion Columns

        public ShoutboxMessage()
            : base() { }

        public ShoutboxMessage(int id) : base(id) { }

        public ShoutboxMessage(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public static List<ShoutboxMessage> GetLatestRecords(int count)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                DataTable dt = bridge.Instance.ExecuteRawCommandToDataTable(SELECT_SQL_COMMAND);
                return TableHelper.GetListFromDataTable<ShoutboxMessage>(dt, count);
            }
        }

        public void TrySend(int maxLength)
        {
            SentDate = DateTime.Now;
            ShoutboxMember User = new ShoutboxMember(Username);

            //ContainsCommand?
            if (AppSettings.Shoutbox.CommandsEnabled)            
                ShoutboxCommands.TryExecuteCommand(Message);            

            //Links disabled?
            if (!AppSettings.Shoutbox.ExternalLinksEnabled && !ShoutboxMessageRestrictions.AllowMessage(Message))
                    throw new MsgException(U4100.LINKSDISABLED);                       

            if (AppSettings.Shoutbox.DefaultBannedPolicyEnabled)
            {
                if (User.IsBanned)
                {
                    if (User.IsBannedPermanently)
                        throw new MsgException(U3500.BANNEDPERM);
                    throw new MsgException(U3500.BANNEDUNTIL.Replace("%n%", User.BannedUntil.ToString()));
                }
            }
            else if (!User.IsBanned)
                throw new MsgException(U6005.YOUCANTUSESHOUTBOX);

            //Banned words
            var allBannedWords = TableHelper.SelectAllRows<ShoutboxBannedWord>();
            foreach (var word in allBannedWords)
            {
                if (AppSettings.Shoutbox.AreBannedWordsCaseSensitive)
                    Message = Message.Replace(word.WordName, new string('*', word.WordName.Length));
                else
                    Message = Regex.Replace(Message, string.Format("\\b{0}\\b", word.WordName), new string('*', word.WordName.Length), RegexOptions.IgnoreCase);
            }
                
            Message = InputChecker.HtmlEncode(Message, maxLength, U5004.MESSAGE);
            Save();
        }
    }
}