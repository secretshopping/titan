using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Webinars
{
    [Serializable]
    public class Webinar : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "Webinars"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("Title")]
        public string Title { get { return _Title; } protected set { _Title = value; SetUpToDateAsFalse(); } }

        [Column("Description")]
        public string Description { get { return _Description; } protected set { _Description = value; SetUpToDateAsFalse(); } }

        [Column("Language")]
        public string Language { get { return _Language; } protected set { _Language = value; SetUpToDateAsFalse(); } }

        [Column("Url")]
        public string Url { get { return _Url; } protected set { _Url = value; SetUpToDateAsFalse(); } }

        [Column("DateTime")]
        public DateTime DateTime { get { return _DateTime; } protected set { _DateTime = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get
            {
                return (UniversalStatus)StatusInt;
            }
            set
            {
                StatusInt = (int)value;
            }
        }

        int _Id, _StatusInt;
        string _Language, _Url, _Title, _Description;
        DateTime _DateTime;

        #endregion


        public Webinar(int id) : base(id) { }

        public Webinar(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private Webinar(string title, string description, string language, string url, DateTime dateTime)
        {
            Title = title;
            Description = description;
            Language = language;
            Url = url;
            DateTime = dateTime;
            Status = UniversalStatus.Paused;
        }

        public static void Create(string title, string description, string language, string url, DateTime dateTime)
        {
            var webinar = new Webinar(title, description, language, url, dateTime);
            webinar.Save();
        }

        public void Activate()
        {
            Status = UniversalStatus.Active;
            Save();
        }

        public void Pause()
        {
            Status = UniversalStatus.Paused;
            Save();
        }

        public override void Delete()
        {
            Status = UniversalStatus.Deleted;
            Save();
        }

        public void Update(string title, string description, string language, string url, DateTime dateTime)
        {
            Title = title;
            Description = description;
            Language = language;
            Url = url;
            DateTime = dateTime;
            Save();
        }
    }
}