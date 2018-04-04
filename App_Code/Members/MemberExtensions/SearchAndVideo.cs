using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Prem.PTC.Utils.NVP;
using MarchewkaOne.Titan.Balances;
using System.Web.Security;
using Prem.PTC;
using Titan;
using ExtensionMethods;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        #region Columns

        [Column("TotalSearchesDone")]
        public int TotalSearchesDone { get { return _TotalSearchesDone; } set { _TotalSearchesDone = value; SetUpToDateAsFalse(); } }

        [Column("TotalVideosWatched")]
        public int TotalVideosWatched { get { return _TotalVideosWatched; } set { _TotalVideosWatched = value; SetUpToDateAsFalse(); } }

        [Column("PointsCreditedForSearchToday")]
        public int PointsCreditedTodayForSearch { get { return _PointsCreditedForSearchToday; } set { _PointsCreditedForSearchToday = value; SetUpToDateAsFalse(); } }

        [Column("PointsCreditedForVideoToday")]
        public int PointsCreditedTodayForVideo { get { return _PointsCreditedForVideoToday; } set { _PointsCreditedForVideoToday = value; SetUpToDateAsFalse(); } }

        [Column("LastCreditedSearch")]
        protected DateTime? _LastCreditedSearch { get { return _lastCreditedSearch; } set { _lastCreditedSearch = value; SetUpToDateAsFalse(); } }

        private int _TotalSearchesDone, _TotalVideosWatched, _PointsCreditedForSearchToday, _PointsCreditedForVideoToday;
        private DateTime? _lastCreditedSearch;

        #endregion

        #region Properties
        /// <summary>
        /// Returns DateTime of last successfuly credited search. If null, returns DateTime.MinValue. 
        /// </summary>
        public DateTime LastCreditedSearch
        {
            get
            {
                if (!_LastCreditedSearch.HasValue)
                    return DateTime.MinValue;

                return _LastCreditedSearch.Value;
            }
            set
            {
                _LastCreditedSearch = value;
            }
        }

        #endregion

        private PropertyInfo[] buildSearchVideo()
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();

            builder.Append(x => x.TotalSearchesDone)
                   .Append(x => x.PointsCreditedTodayForSearch)
                   .Append(x => x.PointsCreditedTodayForVideo)
                   .Append(x => x._LastCreditedSearch)
                   .Append(x => x.TotalVideosWatched);

            return builder.Build();
        }

        public void SaveSearchAndVideo()
        {
            SavePartially(IsUpToDate, buildSearchVideo());
        }
    }
}