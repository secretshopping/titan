using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Referrals;
using MarchewkaOne.Titan.Contests;

namespace Prem.PTC.Contests
{

    public class Contest : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "Contests"; } }
        protected override string dbTable { get { return TableName; } }


        public static class Columns
        {
            public static string Id = "Id";
            public static string Type = "Type";
            public static string Name = "Name";
            public static string DateStart = "DateStart";
            public static string DateEnd = "DateEnd";
            public static string Status = "Status";
            public static string PrizeType = "PrizeType";
            public static string MinMembersDeposit = "MinMembersDeposit";
        }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Type")]
        protected int IntType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("Name")]
        public string Name { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("DateStart")]
        public DateTime DateStart { get { return _DateStart; } set { _DateStart = value; SetUpToDateAsFalse(); } }

        [Column("DateEnd")]
        public DateTime DateEnd { get { return _DateEnd; } set { _DateEnd = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int IntStatus { get { return stype; } set { stype = value; SetUpToDateAsFalse(); } }

        [Column("PrizeType")]
        protected int IntPrize { get { return ptype; } set { ptype = value; SetUpToDateAsFalse(); } }

        [Column("Prize1Value")]
        public Money Prize1Value { get { return _Prize1Value; } set { _Prize1Value = value; SetUpToDateAsFalse(); } }

        [Column("Prize2Value")]
        public Money Prize2Value { get { return _Prize2Value; } set { _Prize2Value = value; SetUpToDateAsFalse(); } }

        [Column("Prize3Value")]
        public Money Prize3Value { get { return _Prize3Value; } set { _Prize3Value = value; SetUpToDateAsFalse(); } }

        [Column("ClicksRestriction")]
        public int ClicksRestriction { get { return r1; } set { r1 = value; SetUpToDateAsFalse(); } }


        [Column("ClicksReferallsRestriction")]
        public int ClicksReferallsRestriction { get { return _ClicksReferallsRestriction; } set { _ClicksReferallsRestriction = value; SetUpToDateAsFalse(); } }

        [Column("RegisteredDaysRestriction")]
        public int RegisteredDaysRestriction { get { return r2; } set { r2 = value; SetUpToDateAsFalse(); } }

        [Column("DirectRefRestriction")]
        public int DirectRefRestriction { get { return r3; } set { r3 = value; SetUpToDateAsFalse(); } }

        [Column("RentedRefRestriction")]
        public int RentedRefRestriction { get { return r4; } set { r4 = value; SetUpToDateAsFalse(); } }

        [Column("Autoparticipate")]
        public bool Autoparticipate { get { return _Autoparticipate; } set { _Autoparticipate = value; SetUpToDateAsFalse(); } }

        [Column("Prize1Url")]
        public string Prize1Url { get { return _Prize1Url; } set { _Prize1Url = value; SetUpToDateAsFalse(); } }

        [Column("Prize2Url")]
        public string Prize2Url { get { return _Prize2Url; } set { _Prize2Url = value; SetUpToDateAsFalse(); } }

        [Column("Prize3Url")]
        public string Prize3Url { get { return _Prize3Url; } set { _Prize3Url = value; SetUpToDateAsFalse(); } }

        [Column("AdPackReferallsPurchaseRestriction")]
        public int AdPackReferrallsPurchaseRestriction
        {
            get { return _AdPackReferrallsPurchaseRestriction; }
            set { _AdPackReferrallsPurchaseRestriction = value; SetUpToDateAsFalse(); }
        }

        [Column("MinMembersDeposit")]
        public Money MinMembersDeposit { get { return _MinMembersDeposit; } set { _MinMembersDeposit = value; SetUpToDateAsFalse(); } }

        private int _id, quantity, type, imageid, _Points, stype, ptype, r1, r2, r3, r4, _ClicksReferallsRestriction, _AdPackReferrallsPurchaseRestriction;
        private string name, _Prize1Url, _Prize2Url, _Prize3Url;
        private bool _Autoparticipate;
        private DateTime _DateStart, _DateEnd;
        private Money _Prize3Value, _Prize2Value, _Prize1Value, _MinMembersDeposit;

        #endregion Columns

        public Contest()
            : base()
        {
            Autoparticipate = false;
        }

        public Contest(int id) : base(id) { }

        public Contest(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        public ContestType Type
        {
            get
            {
                return (ContestType)IntType;
            }

            set
            {
                IntType = (int)value;
            }
        }

        public PrizeType RewardType
        {
            get
            {
                return (PrizeType)IntPrize;
            }

            set
            {
                IntPrize = (int)value;
            }
        }

        public ContestStatus Status
        {
            get
            {
                return (ContestStatus)IntStatus;
            }

            set
            {
                IntStatus = (int)value;
            }
        }


        public bool IsMemberParticipating(string Username, bool NotRecurrent = false)
        {

            var where = TableHelper.MakeDictionary("ContestId", this.Id);
            where.Add("Username", Username);

            var result = TableHelper.SelectRows<ContestParticipant>(where);

            if (result.Count > 0)
                return true;

            if (NotRecurrent)
                return false;

            if (this.Autoparticipate)
            {
                //Maybe we can add him?
                Member User = new Member(Username);
                if (this.CanMemberParticipate(User))
                {
                    this.Participate(User);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finishes the contest, no need to save
        /// </summary>
        public void Finish()
        {
            try
            {
                this.Status = ContestStatus.Finished;

                //Pick up the winners
                //var AllParticipants = TableHelper.SelectRows<ContestParticipant>
                //   (TableHelper.MakeDictionary("ContestId", this.Id));
                var AllParticipants = GetAllMembersParticipating();

                AllContestLatestWinners HistoryContestData = new AllContestLatestWinners();
                HistoryContestData.ContestId = this.Id;
                HistoryContestData.NumberOfParticipants = AllParticipants.Count;

                AllParticipants.Sort(Comparison);

                string winner1 = "-";
                string winner2 = "-";
                string winner3 = "-";

                if (AllParticipants.Count > 0)
                    winner1 = "OK";
                if (AllParticipants.Count > 1)
                    winner2 = "OK";
                if (AllParticipants.Count > 2)
                    winner3 = "OK";

                Money ZeroMoney = new Money(0);

                if (winner1 == "OK")
                {
                    if (AllParticipants[0].Points == ZeroMoney)
                        winner1 = "-";
                    else
                    {
                        Member Winner1 = new Member(AllParticipants[0].Username);
                        ContestManager.AwardMember(Winner1, this.RewardType, this.Prize1Value, 1);
                        winner1 = Winner1.Name + ": " + ContestManager.GetPrizeProperObject(this.RewardType, this.Prize1Value).ToString() + " [%" + (int)this.RewardType + "%]";
                    }
                }
                if (winner2 == "OK")
                {
                    if (AllParticipants[1].Points == ZeroMoney)
                        winner2 = "-";
                    else
                    {
                        Member Winner2 = new Member(AllParticipants[1].Username);
                        ContestManager.AwardMember(Winner2, this.RewardType, this.Prize2Value, 2);
                        winner2 = Winner2.Name + ": " + ContestManager.GetPrizeProperObject(this.RewardType, this.Prize2Value).ToString() + " [%" + (int)this.RewardType + "%]";
                    }
                }
                if (winner3 == "OK")
                {
                    if (AllParticipants[2].Points == ZeroMoney)
                        winner3 = "-";
                    else
                    {
                        Member Winner3 = new Member(AllParticipants[2].Username);
                        ContestManager.AwardMember(Winner3, this.RewardType, this.Prize3Value, 3);
                        winner3 = Winner3.Name + ": " + ContestManager.GetPrizeProperObject(this.RewardType, this.Prize3Value).ToString() + " [%" + (int)this.RewardType + "%]";
                    }
                }

                //Save contest history data
                foreach (var elem in AllParticipants)
                {
                    HistoryContestData.Participants += elem.Username + "(" + elem.Points.GetRealTotals().ToString() + "), ";
                }
                HistoryContestData.Winner1 = winner1;
                HistoryContestData.Winner2 = winner2;
                HistoryContestData.Winner3 = winner3;
                HistoryContestData.Save();

                ContestLatestWinners.AddAndClean(this.Type, winner1, winner2, winner3);
                this.Save();

                //Clean the contest data (ContestParticipants)
                TableHelper.DeleteRows<ContestParticipant>(TableHelper.MakeDictionary("ContestId", this.Id));
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }


        /// <summary>
        /// Gets 7 top members list for current contest
        /// 1st = [0]
        /// 2nd = [1]
        /// 3rd = [2]
        /// etc.
        /// </summary>
        public List<string> GetTopMembersList(int HowMany = 7)
        {
            List<string> result = new List<string>();

            //var AllParticipants = TableHelper.SelectRows<ContestParticipant>
            //    (TableHelper.MakeDictionary("ContestId", this.Id));

            var AllParticipants = GetAllMembersParticipating();

            AllParticipants.Sort(Comparison);

            for (int i = 0; i < HowMany; ++i)
            {
                if (AllParticipants.Count > i && AllParticipants[i].Points.GetRealTotals() > 0)
                {
                    result.Add(AllParticipants[i].Username + " <span style=\"font-size:8px\">(" +
                        ContestManager.GetActionProperObject(Type, AllParticipants[i].Points).ToString() + ") "
                        + AllParticipants[i].LatestAction.ToShortDateString() + "</span>");
                }
                else
                    result.Add("-");
            }

            return result;
        }

        public List<ContestParticipant> GetAllMembersParticipating()
        {
            var where = TableHelper.MakeDictionary("ContestId", this.Id);
            var AllParticipants = TableHelper.GetListFromQuery<ContestParticipant>(@"INNER JOIN Users ON ContestParticipants.Username = Users.Username
                WHERE ContestParticipants.ContestId = " + this.Id + @" AND (Users.AccountStatusInt IN (1,9,10));");

            if (this.Type == ContestType.Direct && (this.ClicksReferallsRestriction > 0 || this.AdPackReferrallsPurchaseRestriction > 0))
            {
                Dictionary<string, int> Referals = new Dictionary<string, int>();
                var ListUsers = TableHelper.GetListFromQuery<Member>(@"INNER JOIN ContestParticipants On ContestParticipants.Username = Users.Referer WHERE IsRented = 0 
                    AND RegisterDate >='" + this.DateStart + @"' AND ContestId =" + this.Id + " AND TotalClicks >=" + this.ClicksReferallsRestriction + ";");

                foreach (var user in ListUsers)
                {
                    if ((user.Status == MemberStatus.Active || user.Status == MemberStatus.AwaitingSMSPIN ||
                         user.Status == MemberStatus.VacationMode) &&
                        user.ExpiredAdPacks + user.ActiveAdPacks >= this.AdPackReferrallsPurchaseRestriction)
                    {
                        if (Referals.ContainsKey(user.Referer))
                        {
                            Referals[user.Referer] += 1;
                        }
                        else
                            Referals.Add(user.Referer, 1);
                    }
                }

                var resultParticipants = new List<ContestParticipant>();

                foreach (var participant in AllParticipants)
                {
                    if (Referals.ContainsKey(participant.Username))
                    {
                        participant.Points = new Money(Referals[participant.Username]);
                        resultParticipants.Add(participant);
                    }
                }

                return resultParticipants;
            }
            else if (this.Type == ContestType.Forum)
            {
                AllParticipants = ForumContestHelper.GetForumParticipants(this);
            }
            return AllParticipants;
        }

        private int Comparison(ContestParticipant x, ContestParticipant y)
        {
            // Return -1 = X is greater then Y (Put x above Y)
            // Return 1 = Y is greater then X (Put y above x) 

            if (x.Points < y.Points)
                return 1;
            else if (x.Points == y.Points)
            {
                // Compare the Last Activity Date to see who got to the points value first.
                DateTime xLastActvity = x.LatestAction;
                DateTime yLastActivity = y.LatestAction;
                int result = DateTime.Compare(xLastActvity, yLastActivity);

                if (result < 0) //X Time is earlier then Y Time
                {
                    return -1;
                }
                else if (result == 0) //X Time is the same as Y Time (Leave X on Top)
                {
                    return 1;
                }
                else //X Time is later then Y Time
                {
                    return 1;
                }
            }
            else
                return -1;
        }

        /// <summary>
        /// 1st = [0]
        /// 2nd = [1]
        /// 3rd = [2]
        /// </summary>
        public List<string> GetPrizesList()
        {
            List<string> result = new List<string>();

            if (RewardType != PrizeType.CustomPrize)
            {
                result.Add(GeneratePrizeCode(Prize1Value));
                result.Add(GeneratePrizeCode(Prize2Value));
                result.Add(GeneratePrizeCode(Prize3Value));
            }
            else
            {
                result.Add(GeneratePrizeCode(Prize1Url));
                result.Add(GeneratePrizeCode(Prize2Url));
                result.Add(GeneratePrizeCode(Prize3Url));
            }

            return result;
        }

        private string GeneratePrizeCode(Money Value)
        {
            return " <b>" + ContestManager.GetPrizeProperObject(RewardType, Value).ToString() + "</b> ["
                + ContestManager.GetPrizeName(RewardType) + "]";
        }

        private string GeneratePrizeCode(string imageUrl)
        {
            imageUrl = imageUrl.Remove(0, 2);
            return " <img style='max-width: 100px; max-heigth: 100px' src =" + imageUrl + ">";
        }

        public bool CanMemberParticipate(Member User, bool obeyParticipateCheck = false)
        {
            int numberOfReferrals = 0;
            //1. check if he is not already participating
            
            if (obeyParticipateCheck == false && IsMemberParticipating(User.Name, true))
                return false;

            //2. Check requirements
            if (User.TotalClicks < this.ClicksRestriction)
                return false;

            if (User.Registered > DateTime.Now.AddDays(-this.RegisteredDaysRestriction))
                return false;

            if (this.DirectRefRestriction > 0 && User.GetDirectReferralsCount() < this.DirectRefRestriction)
                return false;

            if (this.RentedRefRestriction > 0)
            {
                var rrm = new RentReferralsSystem(User.Name, User.Membership);
                if (rrm.GetUserRentedReferralsCount() < this.RentedRefRestriction)
                    return false;
            }

            //3. Check blacklist
            var BlockedContestsForMember = TableHelper.SelectRows<ContestsBlocked>(TableHelper.MakeDictionary("UserId", User.Id));
            foreach (var elem in BlockedContestsForMember)
                if (elem.ContestId == this.Id)
                    return false;

            if (UsersBannedFromContestsType.IsBannedFromContestType(this.Id, this.Type))
                return false;
            
            return true;
        }

        public void Participate(Member User)
        {
            if (CanMemberParticipate(User))
            {
                var cp = new ContestParticipant();
                cp.ContestId = this.Id;
                cp.LatestAction = DateTime.Now;
                cp.Points = new Money(0);
                cp.Username = User.Name;
                cp.Save();
            }
        }
    }
}