using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Publish.PTCOfferWalls
{
    public class PTCOfferWallUserUrl : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PTCOfferWallsUserUrls"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("PTCOfferWallId")]
        public int PTCOfferWallId { get { return _PTCOfferWallId; } protected set { _PTCOfferWallId = value; SetUpToDateAsFalse(); } }

        [Column("UserUrlId")]
        public int UserUrlId { get { return _UserUrlId; } protected set { _UserUrlId = value; SetUpToDateAsFalse(); } }

        #endregion Columns



        int _Id, _PTCOfferWallId, _UserUrlId;

        public PTCOfferWallUserUrl(int id) : base(id) { }

        public PTCOfferWallUserUrl(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private PTCOfferWallUserUrl(int ptcOfferWallId, int userUrlId)
        {
            PTCOfferWallId = ptcOfferWallId;
            UserUrlId = userUrlId;
        }

        public static void CreateMapping(int ptcOfferWallId, int userUrlId)
        {
            if (!RecordExists(ptcOfferWallId, userUrlId))
            {
                var item = new PTCOfferWallUserUrl(ptcOfferWallId, userUrlId);
                item.Save();
            }
        }

        private static bool RecordExists(int ptcOfferWallId, int userUrlId)
        {
            var whereDict = new Dictionary<string, object>();
            whereDict["PTCOfferWallId"] = ptcOfferWallId;
            whereDict["UserUrlId"] = userUrlId;
            return TableHelper.RowExists(PTCOfferWallUserUrl.TableName, whereDict);
        }
    }
}