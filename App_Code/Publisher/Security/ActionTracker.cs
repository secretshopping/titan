using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Resources;
using ExtensionMethods;

namespace Titan.Publisher.Security
{
    public abstract class ActionTracker : BaseTableObject
    {
        #region Columns

        public static class Columns
        {
            public const string Id = "Id";
            public const string PublishersWebsiteId = "PublishersWebsiteId";
            public const string Ip = "Ip";
            public const string CreditedActions = "CreditedActions";
            public const string AllActions = "AllActions";
            public const string LastCreditedActionTime = "LastCreditedActionTime";
            public const string AdvertId = "AdvertId";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PublishersWebsiteId)]
        public int PublishersWebsiteId { get { return _PublishersWebsiteId; } set { _PublishersWebsiteId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Ip)]
        public string Ip { get { return _Ip; } set { _Ip = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreditedActions)]
        public int CreditedActions { get { return _CreditedActions; } set { _CreditedActions = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AllActions)]
        public int AllActions { get { return _AllActions; } set { _AllActions = value; SetUpToDateAsFalse(); } }

        [Column(Columns.LastCreditedActionTime)]
        public DateTime LastCreditedActionTime { get { return _LastCreditedActionTime; } set { _LastCreditedActionTime = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdvertId)]
        public int AdvertId { get { return _AdvertId; } set { _AdvertId = value; SetUpToDateAsFalse(); } }

        int _Id, _PublishersWebsiteId, _CreditedActions, _AllActions, _AdvertId;
        DateTime _LastCreditedActionTime;
        string _Ip;
        #endregion Columns

        public ActionTracker(int id) : base(id) { }

        public ActionTracker(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

        public ActionTracker(int publishersWebsiteId, string ip, int advertId)
        {
            PublishersWebsiteId = publishersWebsiteId;
            Ip = ip;
            CreditedActions = 0;
            AllActions = 0;
            LastCreditedActionTime = new DateTime().Zero();
            AdvertId = advertId;
        }
        
        public virtual void HandleAction(Action onSuccess)
        {
            //if (LastCreditedActionTime < AppSettings.ServerTime.AddMinutes(-AppSettings.AffiliateNetwork.MinutesBetweenExternalBannerClicksPerIp))
            //{
            if (CreditedActions == 0)
            {
                CreditedActions++;
                LastCreditedActionTime = AppSettings.ServerTime;
                
                onSuccess();
            }
            AllActions++;
            Save();
        }
    }
}

