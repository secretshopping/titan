using System;
using Prem.PTC;
using System.Data;
using Titan.Registration;

namespace Titan
{
    public class CustomRegistrationField : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CustomRegistrationFields"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("StringID")]
        public string StringID { get { return name; } set { name = value; SetUpToDateAsFalse(); } }
        
        [Column("FieldType")]
        private int FieldType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("IsRequired")]
        public bool IsRequired { get { return i1; } set { i1 = value; SetUpToDateAsFalse(); } }

        [Column("IsHidden")]
        public bool IsHidden { get { return i2; } set { i2 = value; SetUpToDateAsFalse(); } }

        [Column("Label")]
        public string Label { get { return name2; } set { name2 = value; SetUpToDateAsFalse(); } }
        
        private int _id, type;
        private string name, name2;
        private DateTime d1, d2;
        private bool i1, i2;

        public RegistrationFieldType Type
        {
            get
            {
                return (RegistrationFieldType)FieldType;
            }
            set
            {
                FieldType = (int)value;
            }
        }

        #endregion Columns

        public CustomRegistrationField()
            : base() { }

        public CustomRegistrationField(int id) : base(id) { }

        public CustomRegistrationField(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public override void Save(bool forceSave = false)
        {
            //Check if ID is unique
            var elemsWithTheSameId = TableHelper.SelectRows<CustomRegistrationField>(TableHelper.MakeDictionary("StringID", this.StringID));
            foreach (var elem in elemsWithTheSameId)
                if (elem.Id != this.Id)
                    throw new MsgException("The StringID must be unique! (There is already a field with this StringID)");

            base.Save(forceSave);
        }

    }
}