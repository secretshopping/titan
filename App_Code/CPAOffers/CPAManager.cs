using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Offers;
using Prem.PTC.Members;
using System.Data;
using Prem.PTC.Memberships;
using Prem.PTC.Advertising;

namespace Titan
{
    public class CPAManager
    {
        #region Accept
        public static void AcceptEntry(OfferRegisterEntry Entry, Member User)
        {
            Entry.Status = OfferStatus.Completed;
            Entry.DateCompleted = DateTime.Now;
            Entry.Save();

            Entry.Offer.LastCredited = DateTime.Now;
            Entry.Offer.Save();

            User.TotalCPACompleted += 1;
        }

        //From U-A
        //From AP
        //If you made any changes here, also change TryToCreditAllOffers() metod
        public static void AcceptEntryManually(OfferRegisterEntry entry, Member user)
        {
            CPAManager.AcceptEntry(entry, user);

            CPAGPTCrediter Crediter = (CPAGPTCrediter)CrediterFactory.Acquire(user, CreditType.CPAGPTOffer);
            Crediter.CreditManual(entry);

            entry.Offer.PerformStatusControlCheck();
        }

        //From Postback
        public static Money AcceptEntryFromPostback(OfferRegisterEntry entry, Money balance, CreditAs creditAs, int offerId,
            string networkName, string offerTitle, bool requiresConversion, out bool isLocked)
        {
            Money Calculated = new Money(0);
            Member User = new Member(entry.Username);

            CPAManager.AcceptEntry(entry, User);
            CPAGPTCrediter Crediter = (CPAGPTCrediter)CrediterFactory.Acquire(User.Name, CreditType.CPAGPTOffer);
            Calculated = Crediter.CreditFromPostback(balance, creditAs, networkName, offerId, offerTitle, entry._OfferId ,requiresConversion);

            //isLocked = Crediter.isLocked;
            isLocked = false; //TODO

            return Calculated;
        }
        #endregion

        #region Deny
        //From U-A
        //From AP
        public static void DenyEntry(OfferRegisterEntry Entry, Member User, string offerTitle)
        {
            Entry.Status = OfferStatus.Denied;
            Entry.Save();

            var offer = Entry.Offer;

            //History
            History.AddCPAOfferDeniedUnder(User.Name, offerTitle, offer.IsFromAutomaticNetwork ? offer.AdvertiserUsername : "");
        }

        //From Postback
        public static Money DenyEntryFromPostback(OfferRegisterEntry entry, Money balance, CreditAs creditAs, int offerId,
            string networkName, string offerTitle, bool requiresConversion)
        {
            Money Calculated = new Money(0);
            Member User = new Member(entry.Username);

            CPAManager.DenyEntry(entry, User, offerTitle);

            User.TotalCPACompleted -= 1;

            CPAGPTCrediter Crediter = (CPAGPTCrediter)CrediterFactory.Acquire(User.Name, CreditType.CPAGPTOffer);
            Calculated = Crediter.ReverseCreditFromPostback(balance, creditAs, networkName, offerId, offerTitle, requiresConversion);

            return Calculated;
        }

        public static void DenyEntryManually(OfferRegisterEntry entry, Member user)
        {
            CPAManager.DenyEntry(entry, user, entry.Offer.Title);
            entry.Offer.PerformStatusControlCheck();
        }

        //From U-A
        public static void PutEntryUnderReview(OfferRegisterEntry Entry, Member User)
        {
            Entry.Status = OfferStatus.UnderReview;
            Entry.Save();

            History.AddCPAOfferDeniedUnder(User.Name, Entry.Offer.Title);
            Entry.Offer.PerformStatusControlCheck();
        }

        //From AP
        public static void ReverseEntry(OfferRegisterEntry Entry, Member User)
        {
            CPAManager.DenyEntry(Entry, User, Entry.Offer.Title);

            User.TotalCPACompleted -= 1;

            //Now return the money back
            CPAGPTCrediter Crediter = (CPAGPTCrediter)CrediterFactory.Acquire(User, CreditType.CPAGPTOffer);
            Crediter.ReverseManual(Entry);
        }
        #endregion

        #region Popup
        public static List<string> GetPopUpNotifications()
        {
            List<string> result = new List<string>();

            string SQL_COMMAND = "SELECT * FROM History WHERE AssignedUsername = '" + Member.CurrentName +
           "' AND Type = 10 AND IsRead = 'false' ORDER BY Date DESC";

            if (Member.IsLogged)
            {
                var where = TableHelper.MakeDictionary("Username", Member.CurrentName);
                DataTable dt;

                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    dt = bridge.Instance.ExecuteRawCommandToDataTable(SQL_COMMAND);
                }
                var results = TableHelper.GetListFromDataTable<History>(dt, 100, true);
                foreach (var elem in results)
                    result.Add(elem.GetFullText());

            }
            return result;
        }

        public static void ClearAllPopUps()
        {
            string SQL_COMMAND = "UPDATE History SET IsRead = 'true' WHERE [AssignedUsername] = '" + Member.CurrentName +
          "' AND [Type] = 10 AND [IsRead] = 'false'";

            if (Member.IsLogged)
            {
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    bridge.Instance.ExecuteRawCommandNonQuery(SQL_COMMAND);
                }
            }
        }
        #endregion

        #region Misc

        public static void TryToCreditAllOffers()
        {
            //Get all nonLocked Entries
            var entriesQuery = string.Format(@"SELECT * FROM OfferRegisterEntries WHERE OfferStatus = {0} OR OfferStatus = {1} AND HasBeenLocked = 0",
                                      (int)OfferStatus.Pending, (int)OfferStatus.UnderReview);
            var entriesList = TableHelper.GetListFromRawQuery<OfferRegisterEntry>(entriesQuery).OrderBy(x => x.Username);

            Member user = null;
            //Creadit all Entries
            foreach (var entry in entriesList)
            {
                if(user == null || (user == null && user.Name != entry.Username))
                    user = new Member(entry.Username);

                CPAGPTCrediter Crediter = (CPAGPTCrediter)CrediterFactory.Acquire(user, CreditType.CPAGPTOffer);
                Crediter.CreditManual(entry);
            }
            
            //Change status of credited offers
            var changeCPAOffersStatusQuery = string.Format(@"
                        WITH CTE AS 
                            (SELECT OfferId, COUNT(*) As CounfOfDone FROM OfferRegisterEntries WHERE OfferStatus IN ({0}, {1}, {2}) GROUP BY OfferId),
                        CTE2 AS
                            (SELECT OfferId, COUNT(*) As CounfOfCompleted FROM OfferRegisterEntries WHERE OfferStatus = {0} GROUP BY OfferId)
                        UPDATE CPAOffers 
                            SET [Status] = (SELECT CASE 
                                WHEN(Id IN (SELECT OfferId FROM CTE) AND CreditsBought > (SELECT CounfOfDone FROM CTE WHERE OfferId = Id) and [Status] = {4})
                                    THEN {3}
                                WHEN(Id IN (SELECT OfferId FROM CTE) AND CreditsBought <= (SELECT CounfOfDone FROM CTE WHERE OfferId = Id) and [Status] = {3})
                                    THEN {4}
                                WHEN(Id IN (SELECT OfferId FROM CTE) AND CreditsBought <= (SELECT CounfOfCompleted FROM CTE2 WHERE OfferId = Id) and [Status] != {5})
                                    THEN {5}
                                ELSE [Status]
                                END) ",
                        (int)OfferStatus.Completed, (int)OfferStatus.Pending, (int)OfferStatus.UnderReview,
                        (int)AdvertStatus.Active, (int)AdvertStatus.Stopped, (int)AdvertStatus.Finished);

            //Increase Users TotalCPACompleted
            var increaseUsersTotalCPACompletedQuery = string.Format(@"
                        WITH CTE AS 
                            (SELECT Username, COUNT(*) AS CountOf FROM OfferRegisterEntries WHERE OfferStatus = {0} OR OfferStatus = {1} GROUP BY username)
                        UPDATE Users 
                            SET TotalCPACompleted += (SELECT CountOf FROM CTE WHERE Username = Users.Username)
                            WHERE Username IN 
                                (SELECT Username from CTE)",
                        (int)OfferStatus.Pending, (int)OfferStatus.UnderReview);

            //Update status of completed entries
            var updateEntriesQuery = string.Format(@"
                        UPDATE OfferRegisterEntries 
                            SET OfferStatus = {0}, CompletedDate = GETDATE() WHERE OfferStatus = {1} OR OfferStatus = {2} AND HasBeenLocked = 0",
                        (int)OfferStatus.Completed, (int)OfferStatus.Pending, (int)OfferStatus.UnderReview);

            //Executing Queries in the correct order
            TableHelper.ExecuteRawCommandNonQuery(changeCPAOffersStatusQuery);
            TableHelper.ExecuteRawCommandNonQuery(increaseUsersTotalCPACompletedQuery);
            TableHelper.ExecuteRawCommandNonQuery(updateEntriesQuery);
        }

        public static void UpdateTableAfterDeletingMembership(int membershipId)
        {
            var querry = string.Format(@"UPDATE CPAOffers SET RequiredMembership = {0} WHERE RequiredMembership = {1}", Membership.Standard.Id, membershipId);
            TableHelper.ExecuteRawCommandNonQuery(querry);
        }

        #endregion
    }
}