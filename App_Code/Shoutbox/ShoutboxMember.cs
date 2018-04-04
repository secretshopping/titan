using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;

namespace Titan
{

    public class ShoutboxMember : Member
    {
        public ShoutboxMember(string Username)
            : base(Username)
        {

        }

        public bool IsBanned
        {
            get
            {
                var results = TableHelper.SelectRows<ShoutboxBannedUsernames>(TableHelper.MakeDictionary("Username", base.Name));
                foreach (var elem in results)
                {
                    if (elem.BannedUntil <= DateTime.Now)
                        elem.Delete();
                    else
                        return true;
                }
                return false;
            }
        }

        public bool IsBannedPermanently
        {
            get
            {
                if (BannedUntil > DateTime.Now.AddYears(5))
                    return true;
                return false;
            }
        }

        public DateTime BannedUntil
        {
            get
            {
                var results = TableHelper.SelectRows<ShoutboxBannedUsernames>(TableHelper.MakeDictionary("Username", base.Name));
                DateTime theWorsestBan = new DateTime(1999, 1, 1);

                foreach (var elem in results)
                {
                    if (elem.BannedUntil > theWorsestBan)
                        theWorsestBan = elem.BannedUntil;
                }
                return theWorsestBan;
            }
        }

        public void Ban(TimeSpan Time)
        {
            ShoutboxBannedUsernames NewEntry = new ShoutboxBannedUsernames();
            NewEntry.Username = base.Name;
            NewEntry.BannedSince = DateTime.Now;
            NewEntry.BannedUntil = DateTime.Now.Add(Time);
            NewEntry.Save();
        }

        public void BanPermanently()
        {
            Ban(TimeSpan.FromDays(5000));
        }

        public void Unban()
        {
            var results = TableHelper.SelectRows<ShoutboxBannedUsernames>(TableHelper.MakeDictionary("Username", base.Name));
            foreach (var elem in results)
            {
                    elem.Delete();
            }
        }
    }
}