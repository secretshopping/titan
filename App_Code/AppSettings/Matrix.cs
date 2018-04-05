using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public static class Matrix
        {
            public static int MaxChildrenInMatrix
            {
                get { return appSettings.MaxChildrenInMatrix; }
                set { appSettings.MaxChildrenInMatrix = value; }
            }

            public static int DaysBetweenMatrixRebuild
            {
                get { return appSettings.DaysBetweenMatrixRebuild; }
                set { appSettings.DaysBetweenMatrixRebuild = value; }
            }

            public static DateTime LastMatrixRebuild
            {
                get { return appSettings.LastMatrixRebuild; }
                set { appSettings.LastMatrixRebuild = value; }
            }

            public static bool MatrixCheckPtc
            {
                get { return appSettings.MatrixCheckPtc; }
                set { appSettings.MatrixCheckPtc = value; }
            }

            public static bool MatrixCheckBanner
            {
                get { return appSettings.MatrixCheckBanner; }
                set { appSettings.MatrixCheckBanner = value; }
            }

            public static bool MatrixCheckFacebook
            {
                get { return appSettings.MatrixCheckFacebook; }
                set { appSettings.MatrixCheckFacebook = value; }
            }

            public static bool MatrixCheckCpa
            {
                get { return appSettings.MatrixCheckCpa; }
                set { appSettings.MatrixCheckCpa = value; }
            }

            public static bool MatrixCheckLogin
            {
                get { return appSettings.MatrixCheckLogin; }
                set { appSettings.MatrixCheckLogin = value; }
            }

            public static bool MatrixCheckTrafficGrid
            {
                get { return appSettings.MatrixCheckTrafficGrid; }
                set { appSettings.MatrixCheckTrafficGrid = value; }
            }

            public static bool MatrixCheckAdPack
            {
                get { return appSettings.MatrixCheckAdPack; }
                set { appSettings.MatrixCheckAdPack = value; }
            }

            public static bool MatrixCheckExternalBanner
            {
                get { return appSettings.MatrixCheckExternalBanner; }
                set { appSettings.MatrixCheckExternalBanner = value; }
            }

            public static bool MatrixCheckInText
            {
                get { return appSettings.MatrixCheckInText; }
                set { appSettings.MatrixCheckInText = value; }
            }

            public static bool MatrixCheckPtcOfferWall
            {
                get { return appSettings.MatrixCheckPtcOfferWall; }
                set { appSettings.MatrixCheckPtcOfferWall = value; }
            }

            public static bool MatrixCheckInvestmentPlatform
            {
                get { return appSettings.MatrixCheckInvestmentPlatform; }
                set { appSettings.MatrixCheckInvestmentPlatform = value; }
            }

            public static int MatrixMaxCreditedLevels
            {
                get { return appSettings.MatrixMaxCreditedLevels; }
                set { appSettings.MatrixMaxCreditedLevels = value; }
            }

            public static MatrixType Type
            {
                get { return (MatrixType)appSettings.MatrixTypeInt; }
                set { appSettings.MatrixTypeInt = (int)value; }
            }

            public static MatrixCrediter Crediter
            {
                get { return (MatrixCrediter)appSettings.MatrixCrediterInt; }
                set { appSettings.MatrixCrediterInt = (int)value; }
            }

            public static bool AutolocateMembersInBinaryMatrix
            {
                get { return appSettings.AutolocateMembersInBinaryMatrix; }
                set { appSettings.AutolocateMembersInBinaryMatrix = value; }
            }

            public static bool MatrixCyclesFromRank
            {
                get { return appSettings.MatrixCyclesFromRank; }
                set { appSettings.MatrixCyclesFromRank = value; }
            }

            public static int MatrixCyclesPerDay
            {
                get { return appSettings.MatrixCyclesPerDay; }
                set { appSettings.MatrixCyclesPerDay = value; }
            }

            public static Money MatrixCyclesBonusMoneyFromLeg
            {
                get { return appSettings.MatrixCyclesBonusMoneyFromLeg; }
                set { appSettings.MatrixCyclesBonusMoneyFromLeg = value; }
            }

            public static Money MatrixCyclesPrizeMoney
            {
                get { return appSettings.MatrixCyclesPrizeMoney; }
                set { appSettings.MatrixCyclesPrizeMoney = value; }
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.ReloadMatrix();
            }

            public static void Save()
            {
                appSettings.SaveMatrix();
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {

            [Column("MaxChildrenInMatrix")]
            internal int MaxChildrenInMatrix
            {
                get { return _MaxChildrenInMatrix; }
                set
                {
                    if (value < 1)
                        throw new MsgException("Max Direct Children in Matrix must be greater than 0.");
                    _MaxChildrenInMatrix = value; SetUpToDateAsFalse();
                }
            }

            [Column("DaysBetweenMatrixRebuild")]
            internal int DaysBetweenMatrixRebuild
            {
                get { return _DaysBetweenMatrixRebuild; }
                set
                {
                    if (value < 1)
                        throw new MsgException("Minimum number of Days Between Matrix Rebuild is 1");
                    _DaysBetweenMatrixRebuild = value; SetUpToDateAsFalse();
                }
            }

            [Column("LastMatrixRebuild")]
            internal DateTime LastMatrixRebuild
            {
                get { return _LastMatrixRebuild; }
                set { _LastMatrixRebuild = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckPtc")]
            internal bool MatrixCheckPtc
            {
                get { return _MatrixCheckPtc; }
                set { _MatrixCheckPtc = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckBanner")]
            internal bool MatrixCheckBanner
            {
                get { return _MatrixCheckBanner; }
                set { _MatrixCheckBanner = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckFacebook")]
            internal bool MatrixCheckFacebook
            {
                get { return _MatrixCheckFacebook; }
                set { _MatrixCheckFacebook = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckCpa")]
            internal bool MatrixCheckCpa
            {
                get { return _MatrixCheckCpa; }
                set { _MatrixCheckCpa = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckLogin")]
            internal bool MatrixCheckLogin
            {
                get { return _MatrixCheckLogin; }
                set { _MatrixCheckLogin = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckTrafficGrid")]
            internal bool MatrixCheckTrafficGrid
            {
                get { return _MatrixCheckTrafficGrid; }
                set { _MatrixCheckTrafficGrid = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckAdPack")]
            internal bool MatrixCheckAdPack
            {
                get { return _MatrixCheckAdPack; }
                set { _MatrixCheckAdPack = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckExternalBanner")]
            internal bool MatrixCheckExternalBanner
            {
                get { return _MatrixCheckExternalBanner; }
                set { _MatrixCheckExternalBanner = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckInText")]
            internal bool MatrixCheckInText
            {
                get { return _MatrixCheckInText; }
                set { _MatrixCheckInText = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckPtcOfferWall")]
            internal bool MatrixCheckPtcOfferWall
            {
                get { return _MatrixCheckPtcOfferWall; }
                set { _MatrixCheckPtcOfferWall = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCheckInvestmentPlatform")]
            internal bool MatrixCheckInvestmentPlatform
            {
                get { return _MatrixCheckInvestmentPlatform; }
                set { _MatrixCheckInvestmentPlatform = value; SetUpToDateAsFalse(); }
            }

            [Column("AutolocateMembersInBinaryMatrix")]
            internal bool AutolocateMembersInBinaryMatrix
            {
                get { return _AutolocateMembersInBinaryMatrix; }
                set { _AutolocateMembersInBinaryMatrix = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixMaxCreditedLevels")]
            internal int MatrixMaxCreditedLevels
            {
                get { return _MatrixMaxCreditedLevels; }
                set
                {
                    if (value < 1)
                        throw new MsgException("Max Credited Levels in Matrix must be greater than 0.");
                    _MatrixMaxCreditedLevels = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixTypeInt")]
            internal int MatrixTypeInt
            {
                get { return _MatrixType; }
                set{ _MatrixType = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCrediterInt")]
            internal int MatrixCrediterInt
            {
                get { return _MatrixCrediter; }
                set { _MatrixCrediter = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCyclesFromRank")]
            internal bool MatrixCyclesFromRank
            {
                get { return _MatrixCyclesFromRank; }
                set { _MatrixCyclesFromRank = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCyclesPerDay")]
            internal int MatrixCyclesPerDay
            {
                get { return _MatrixCyclesPerDay; }
                set { _MatrixCyclesPerDay = value; SetUpToDateAsFalse(); }
            }

           [Column("MatrixCyclesBonusMoneyFromLeg")]
            internal Money MatrixCyclesBonusMoneyFromLeg
            {
                get { return _MatrixCyclesBonusMoneyFromLeg; }
                set { _MatrixCyclesBonusMoneyFromLeg = value; SetUpToDateAsFalse(); }
            }

            [Column("MatrixCyclesPrizeMoney")]
            internal Money MatrixCyclesPrizeMoney
            {
                get { return _MatrixCyclesPrizeMoney; }
                set { _MatrixCyclesPrizeMoney = value; SetUpToDateAsFalse(); }
            }

            int _MaxChildrenInMatrix, _DaysBetweenMatrixRebuild, _MatrixMaxCreditedLevels, _MatrixType, _MatrixCrediter, _MatrixCyclesPerDay;
            DateTime _LastMatrixRebuild;
            bool _MatrixCheckPtc, _MatrixCheckBanner, _MatrixCheckFacebook, _MatrixCheckCpa, _MatrixCheckLogin, _MatrixCheckInvestmentPlatform,
                _MatrixCheckTrafficGrid, _MatrixCheckAdPack, _MatrixCheckExternalBanner, _MatrixCheckInText, _MatrixCheckPtcOfferWall,
                _AutolocateMembersInBinaryMatrix, _MatrixCyclesFromRank;

            Money _MatrixCyclesBonusMoneyFromLeg, _MatrixCyclesPrizeMoney;

            internal void ReloadMatrix()
            {
                ReloadPartially(IsUpToDate, buildMatrixProperties());
            }

            internal void SaveMatrix()
            {
                SavePartially(IsUpToDate, buildMatrixProperties());
            }

            private PropertyInfo[] buildMatrixProperties()
            {
                var MatrixProperties = new PropertyBuilder<AppSettingsTable>();
                MatrixProperties
                    .Append(x => x.MaxChildrenInMatrix)
                    .Append(x => x.DaysBetweenMatrixRebuild)
                    .Append(x => x.LastMatrixRebuild)
                    .Append(x => x.MatrixCheckPtc)
                    .Append(x => x.MatrixCheckBanner)
                    .Append(x => x.MatrixCheckFacebook)
                    .Append(x => x.MatrixCheckCpa)
                    .Append(x => x.MatrixCheckLogin)
                    .Append(x => x.MatrixCheckTrafficGrid)
                    .Append(x => x.MatrixCheckAdPack)
                    .Append(x => x.MatrixCheckExternalBanner)
                    .Append(x => x.MatrixCheckInText)
                    .Append(x => x.MatrixCheckPtcOfferWall)
                    .Append(x => x.MatrixTypeInt)
                    .Append(x => x.MatrixCrediterInt)
                    .Append(x => x.MatrixCheckInvestmentPlatform)
                    .Append(x => x.MatrixMaxCreditedLevels)
                    .Append(x => x.AutolocateMembersInBinaryMatrix)
                    .Append(x => x.MatrixCyclesFromRank)
                    .Append(x => x.MatrixCyclesPerDay)
                    .Append(x => x.MatrixCyclesBonusMoneyFromLeg)
                    .Append(x => x.MatrixCyclesPrizeMoney);
                
                return MatrixProperties.Build();
            }
        }
    }
}