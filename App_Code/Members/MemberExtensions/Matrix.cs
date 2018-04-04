using Microsoft.SqlServer.Types;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Titan.Matrix;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        [Column("MatrixId")]
        protected string MatrixIdString { get { return _MatrixIdString; } set { _MatrixIdString = value; SetUpToDateAsFalse(); } }

        [Column("MatrixBonusMoneyFromLeftLeg")]
        public Money MatrixBonusMoneyFromLeftLeg { get { return _MatrixBonusMoneyFromLeftLeg; } set { _MatrixBonusMoneyFromLeftLeg = value; SetUpToDateAsFalse(); } }

        [Column("MatrixBonusMoneyFromRightLeg")]
        public Money MatrixBonusMoneyFromRightLeg { get { return _MatrixBonusMoneyFromRightLeg; } set { _MatrixBonusMoneyFromRightLeg = value; SetUpToDateAsFalse(); } }

        [Column("MatrixBonusMoneyCyclesToday")]
        public int MatrixBonusMoneyCyclesToday { get { return _MatrixBonusMoneyCyclesToday; } set { _MatrixBonusMoneyCyclesToday = value; SetUpToDateAsFalse(); } }

        [Column("MatrixBonusMoneyIncome")]
        public Money MatrixBonusMoneyIncome { get { return _MatrixBonusMoneyIncome; } set { _MatrixBonusMoneyIncome = value; SetUpToDateAsFalse(); } }

        public SqlHierarchyId MatrixId
        {
            get { return SqlHierarchyId.Parse(MatrixIdString); }
            set { MatrixIdString = value.ToString(); }
        }

        string _MatrixIdString;
        [NonSerialized]
        SqlHierarchyId _MatrixId;
        bool _EligibleForMatrix;
        Money _MatrixBonusMoneyFromLeftLeg, _MatrixBonusMoneyFromRightLeg, _MatrixBonusMoneyIncome;
        int _MatrixBonusMoneyCyclesToday;

        private static Member GetByMatrixId(SqlHierarchyId matrixId)
        {
            string query = string.Format(@"SELECT TOP 1 UserId FROM Users WHERE MatrixId = '{0}'",
                matrixId.ToString());

            object userIdObject = TableHelper.SelectScalar(query);

            if (userIdObject is DBNull)
                return null;

            return new Member((int)userIdObject);
        }

        public Member GetAncestor(int level)
        {
            if (MatrixId.GetAncestor(level).IsNull)
                return null;

            return GetByMatrixId(MatrixId.GetAncestor(level));
        }

        public Member GetAncestor()
        {
            return GetByMatrixId(MatrixId.GetAncestor(1));
        }

        public List<Member> GetDescendants()
        {
            if (string.IsNullOrEmpty(this.MatrixIdString))
                return new List<Member>();

            var CurrentMemberLevel = (int)MatrixId.GetLevel();

            string query = string.Format(@"SELECT * FROM Users WHERE MatrixId IS NOT NULL AND CAST(MatrixId AS HierarchyId).IsDescendantOf('{0}') = 1 
                    AND CAST(MatrixId AS HierarchyId).GetLevel() BETWEEN {1} AND {2} ORDER BY CAST(MatrixId AS hierarchyid).GetLevel() ASC, CAST(MatrixId AS hierarchyid) ASC", this.MatrixIdString, CurrentMemberLevel, 
                    CurrentMemberLevel + AppSettings.Matrix.MatrixMaxCreditedLevels);

            return TableHelper.GetListFromRawQuery<Member>(query);
        }

        public List<Member> GetDescendantsLevel(int level)
        {
            if (string.IsNullOrEmpty(this.MatrixIdString))
                return new List<Member>();

            var CurrentMemberLevel = (int)MatrixId.GetLevel();

            string query = string.Format(@"SELECT * FROM Users WHERE MatrixId IS NOT NULL AND CAST(MatrixId AS HierarchyId).IsDescendantOf('{0}') = 1 
                    AND CAST(MatrixId AS HierarchyId).GetLevel() = {1} ORDER BY CAST(MatrixId AS hierarchyid).GetLevel() ASC, CAST(MatrixId AS hierarchyid) ASC", this.MatrixIdString, CurrentMemberLevel + level);

            return TableHelper.GetListFromRawQuery<Member>(query);
        }

        public List<Member> GetDirectDescendants()
        {
            return GetDescendantsLevel(1);
        }

        public SqlHierarchyId GetMatrixFirstEmptyLeft()
        {
            List<SqlHierarchyId> descendants = this.GetDirectDescendants().Select(x => x.MatrixId).ToList();
            List<SqlHierarchyId> ancestors = new List<SqlHierarchyId>() { this.MatrixId };
            
            SqlHierarchyId result = SqlHierarchyId.Parse(this.MatrixId.ToString() + "1/"); ;

            if (descendants.Contains(result))
            {
                Member rootLeft = Member.GetByMatrixId(descendants.FirstOrDefault(x => (bool)(x == result)));
            
                descendants = rootLeft.GetDirectDescendants().Select(x => x.MatrixId).ToList();
                ancestors = new List<SqlHierarchyId>() { rootLeft.MatrixId };

                int level = 1;
                int nodes = AppSettings.Matrix.MaxChildrenInMatrix;

                while (descendants.Count == nodes)
                {
                    level++;
                    nodes *= AppSettings.Matrix.MaxChildrenInMatrix;
                    ancestors = descendants.Clone();
                    descendants = rootLeft.GetDescendantsLevel(level).Select(x => x.MatrixId).ToList();
                }

                for (int a = 0; a < ancestors.Count; a++)
                {
                    for (int i = 0; i < AppSettings.Matrix.MaxChildrenInMatrix; i++)
                    {
                        result = SqlHierarchyId.Parse(ancestors[a].ToString() + ((i % AppSettings.Matrix.MaxChildrenInMatrix) + 1) + "/");

                        if (!descendants.Contains(result))
                            return result;
                    }
                }
            }

            return result;
        }

        public SqlHierarchyId GetMatrixFirstEmptyRight()
        {
            List<SqlHierarchyId> descendants = this.GetDirectDescendants().Select(x => x.MatrixId).ToList();
            List<SqlHierarchyId> ancestors = new List<SqlHierarchyId>() { this.MatrixId };

            SqlHierarchyId result = SqlHierarchyId.Parse(this.MatrixId.ToString() + "2/"); ;

            if (descendants.Contains(result))
            {
                Member rootRight = Member.GetByMatrixId(descendants.FirstOrDefault(x => (bool)(x == result)));

                descendants = rootRight.GetDirectDescendants().Select(x => x.MatrixId).ToList();
                ancestors = new List<SqlHierarchyId>() { rootRight.MatrixId };

                int level = 1;
                int nodes = AppSettings.Matrix.MaxChildrenInMatrix;

                while (descendants.Count == nodes)
                {
                    level++;
                    nodes *= AppSettings.Matrix.MaxChildrenInMatrix;
                    ancestors = descendants.Clone();
                    descendants = rootRight.GetDescendantsLevel(level).Select(x => x.MatrixId).ToList();
                }

                for (int a = 0; a < ancestors.Count; a++)
                {
                    for (int i = 0; i < AppSettings.Matrix.MaxChildrenInMatrix; i++)
                    {
                        result = SqlHierarchyId.Parse(ancestors[a].ToString() + ((i % AppSettings.Matrix.MaxChildrenInMatrix) + 1) + "/");

                        if (!descendants.Contains(result))
                            return result;
                    }
                }
            }

            return result;
        }

        #region Binary

        public List<Member> GetUnassignedMatrixMembers()
        {
            string query = string.Format(@"SELECT UserId, Username FROM Users WHERE ReferrerId = {0} AND MatrixId IS NULL ORDER BY Username",
                         this.Id);
            return TableHelper.GetListFromRawQuery<Member>(query);
        }

        public int GetUnassignedMatrixMembersCount()
        {
            string query = string.Format(@"SELECT COUNT(UserId) FROM Users WHERE ReferrerId = {0} AND MatrixId IS NULL",
                         this.Id);
            return (int)TableHelper.SelectScalar(query);
        }

        #endregion

        #region Helpers

        public void SaveMatrixId()
        {
            PropertyInfo[] propertiesToSave = BuildMatrixId();
            
            SavePartially(IsUpToDate, propertiesToSave);
        }

        public void ReloadMatrixId()
        {
            PropertyInfo[] propertiesToReload = BuildMatrixId();

            ReloadPartially(IsUpToDate, propertiesToReload);
        }

        private PropertyInfo[] BuildMatrixId()
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();

             builder.Append(x => x.MatrixIdString)
                    .Append(x => x.MatrixBonusMoneyFromLeftLeg)
                    .Append(x => x.MatrixBonusMoneyFromRightLeg)
                    .Append(x => x.MatrixBonusMoneyCyclesToday)
                    .Append(x => x.MatrixBonusMoneyIncome)
                    ;

            return builder.Build();
        }

        #endregion
    }

}