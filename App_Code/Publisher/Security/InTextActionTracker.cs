using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Publisher.Security
{
    public class InTextActionTracker : ActionTracker
    {
        public override Database Database
        {
            get
            {
                return Database.Client;
            }
        }
        public static new string TableName { get { return "InTextActionTracker"; } }
        protected override string dbTable
        {
            get
            {
                return TableName;
            }
        }

        public InTextActionTracker(int id) : base(id) { }
        public InTextActionTracker(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

        public InTextActionTracker(int publishersWebsiteId, string ip, int advertId) : base(publishersWebsiteId, ip, advertId) { }

    }
}