using System.Data;
using Prem.PTC;
using System.Collections.Generic;
using Prem.PTC.Offers;
using Prem.PTC.Members;
using Resources;
using Prem.PTC.Advertising;
using System;
using System.Linq;
using MarchewkaOne.Titan.Balances;

namespace Titan.CustomFeatures
{
    //CREATE TABLE[dbo].S4DSAuthenticationInformation(
    //[Id] INT PRIMARY KEY IDENTITY,
    //[UserId] INT NOT NULL,
    //[personId] VARCHAR(100) NOT NULL,
    //[token] VARCHAR(100) NOT NULL,
    //);

    public class S4DSAuthenticationInformation : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "S4DSAuthenticationInformation"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

        [Column("personId")]
        public string personId { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

        [Column("token")]
        public string token { get { return _Description; } set { _Description = value; SetUpToDateAsFalse(); } }

        private int _Id, _UserId;
        private string _Title, _Description;

        public S4DSAuthenticationInformation()
            : base()
        { }

        public S4DSAuthenticationInformation(int id) : base(id) { }

        public S4DSAuthenticationInformation(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate)
        { }

        #endregion Columns

        public static S4DSAuthenticationInformation Get(int userId)
        {
            return TableHelper.GetListFromRawQuery<S4DSAuthenticationInformation>
                ("SELECT * FROM S4DSAuthenticationInformation WHERE UserId = " + userId)[0];
        }

        public static void AddOrUpdate(int userId, S4DSAuthInfo info)
        {
            S4DSAuthenticationInformation Target = null;

            var result =  TableHelper.GetListFromRawQuery<S4DSAuthenticationInformation>
                ("SELECT * FROM S4DSAuthenticationInformation WHERE UserId = " + userId);

            if (result.Count > 0)
                Target = result[0];
            else
            {
                Target = new S4DSAuthenticationInformation();
                Target.UserId = userId;
            }

            Target.token = info.token;
            Target.personId = info.personId;
            Target.Save();
        }
    }
    
}