using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Prem.PTC.Achievements
{
    public class Achievement : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "Achievements"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Type")]
        protected int IntType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("Quantity")]
        public int Quantity { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

        [Column("Name")]
        public string Name { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("ImageId")]
        public int ImageId { get { return imageid; } set { imageid = value; SetUpToDateAsFalse(); } }

        [Column("Points")]
        public int Points { get { return _Points; } set { _Points = value; SetUpToDateAsFalse(); } }

        [Column("AchievementStatus")]
        protected int AchievmentStatusInt { get { return _AchievmentStatus; } set { _AchievmentStatus = value; SetUpToDateAsFalse(); } }

        private int _id, quantity, type, imageid, _Points, _AchievmentStatus;
        private string name;

        #endregion Columns

        public Achievement()
            : base() { }

        public Achievement(int id) : base(id) { }

        public Achievement(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public AchievementType Type
        {
            get
            {
                return (AchievementType)IntType;
            }

            set
            {
                IntType = (int)value;
            }
        }

        public AchievmentStatus AchievmentStatus
        {
            get
            {
                return (AchievmentStatus)AchievmentStatusInt;
            }

            set
            {
                AchievmentStatusInt = (int)value;
            }
        }

        public string GetText()
        {
            return GetAssociatedResource(Type, Quantity);
        }

        private string GetAssociatedResource(AchievementType Type, int Quantity)
        {
            switch (Type)
            {

                //Resources example: 'For those who had more than %n% banner campaigns'

                case AchievementType.AfterAdvertisingBannerCampaigns:
                    return Resources.L1.ACHIEVEMENT_1.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterAdvertisingFacebookCampaigns:
                    return Resources.L1.ACHIEVEMENT_2.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterAdvertisingPtcCampaigns:
                    return Resources.L1.ACHIEVEMENT_3.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterClicks:
                    return Resources.L1.ACHIEVEMENT_4.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterEarning:
                    return Resources.L1.ACHIEVEMENT_5.Replace("%n%", new Money(Quantity).ToString());

                case AchievementType.AfterHavingDirectReferrals:
                    return Resources.L1.ACHIEVEMENT_6.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterHavingRentedReferrals:
                    return Resources.L1.ACHIEVEMENT_7.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterTransferringOnceAmount:
                    return Resources.L1.ACHIEVEMENT_8.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterAdvertisingTrafficGridCampaigns:
                    return Resources.L1.ACHIEVEMENT_9.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterWinningInTrafficGrid:
                    return Resources.L1.ACHIEVEMENT_10.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterCPAOffersCompleted:
                    return Resources.L1.ACHIEVEMENT_11.Replace("%n%", Quantity.ToString());

                case AchievementType.AfterGettingPointsInOneDay:
                    return Resources.L1.ACHIEVEMENT_12.Replace("%n%", Quantity.ToString());

                default:
                    return "No achievement resource provided";
            }
        }

        public static List<Achievement> GetProperAchievements(AchievementType Type, int UserQuantity)
        {
            return TableHelper.GetListFromQuery<Achievement>("WHERE [Type] = " + (int)Type + " AND [AchievementStatus] = "
                + (int)AchievmentStatus.Visible + " AND [Quantity] <= " + UserQuantity);
        }

        public static bool HasAnyoneThisAchievement(Achievement achievement)
        {
            return HasAnyoneThisAchievement(achievement.Id);
        }

        public static bool HasAnyoneThisAchievement(int achievementId)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                string Command = "SELECT COUNT(Username) FROM Users WHERE Achievements LIKE '%#" +
                    achievementId.ToString() + "#%' OR Achievements LIKE '%#" + achievementId.ToString() + "'";

                int count = (int)bridge.Instance.ExecuteRawCommandScalar(Command);

                return count > 0;
            }
        }

    }
}