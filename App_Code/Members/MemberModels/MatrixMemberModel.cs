using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.MemerModels
{
    public class MatrixMemberModel : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return AppSettings.TableNames.Members; } }
        protected override string dbTable { get { return TableName; } }

        [Column("UserId", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("UserName")]
        public string Name { get { return _UserName; } set { _UserName = value; SetUpToDateAsFalse(); } }

        public bool EligibleForMatrix { get { return _EligibleForMatrix; } set { _EligibleForMatrix = value; SetUpToDateAsFalse(); } }

        int _Id;
        string _UserName;
        bool _EligibleForMatrix;

        public override void Save(bool forceSave = false)
        {
            //Do nothing
        }
        public override void Delete()
        {
            //Do nothing
        }

        public MatrixMemberModel(int id)
            : base(id)
        { }
        public MatrixMemberModel(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }
    }
}