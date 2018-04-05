using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Publisher.Security
{
    public class ExternalBannerActionTracker : ActionTracker
    {
        public override Database Database
        {
            get
            {
                return Database.Client;
            }
        }
        public static new string TableName { get { return "ExternalBannerActionTracker"; } }
        protected override string dbTable
        {
            get
            {
                return TableName;
            }
        }

        public ExternalBannerActionTracker(int id) : base(id) { }
        public ExternalBannerActionTracker(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

        public ExternalBannerActionTracker(int publishersWebsiteId, string ip, int advertId) : base(publishersWebsiteId, ip, advertId) { }
    }
}