using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace SocialNetwork
{
    public class Friendship : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "Friendships"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("UserIdOne")]
        public int UserIdOne { get { return _UserIdOne; } set { _UserIdOne = value; SetUpToDateAsFalse(); } }

        [Column("UserIdTwo")]
        public int UserIdTwo { get { return _UserIdTwo; } set { _UserIdTwo = value; SetUpToDateAsFalse(); } }

        int _id, _UserIdOne, _UserIdTwo;

        #endregion Columns

        public Friendship()
            : base() { }

        public Friendship(int id) : base(id) { }

        public Friendship(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        private Friendship(int userIdOne, int userIdTwo)
        {
            UserIdOne = userIdOne;
            UserIdTwo = userIdTwo;
        }
        public static Friendship Get(int userIdOne, int userIdTwo)
        {
            return TableHelper.GetListFromRawQuery<Friendship>(
                string.Format(@"SELECT TOP 1* FROM Friendships 
                WHERE (UserIdOne = {0} AND UserIdTwo = {1}) OR (UserIdOne = {1} AND UserIdTwo = {0})",
                userIdOne, userIdTwo)).SingleOrDefault();
        }

        public static void Create(int userIdOne, int userIdTwo)
        {
            if (Get(userIdOne, userIdTwo) == null)
            {
                var friendship = new Friendship(userIdOne, userIdTwo);
                friendship.Save();
            }
        }

        public static List<Friendship> GetList(int userId)
        {
            return TableHelper.GetListFromRawQuery<Friendship>(
                string.Format(@"SELECT * FROM Friendships 
                WHERE (UserIdOne = {0} OR UserIdTwo = {0})",
                userId));
        }
    }
}