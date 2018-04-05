using System.Data;

namespace Prem.PTC.Members
{
    public class GeolocationDictionary : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return 0; } protected set { } }

        [Column("Country")]
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        [Column("Count")]
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private string _country;
        private int _count;

        public GeolocationDictionary(): base() { }

        public GeolocationDictionary(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
    }
}