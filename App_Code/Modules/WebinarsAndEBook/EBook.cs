using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;

namespace Titan.EBooks
{
    [Serializable]
    public class EBook : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "EBooks"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Title = "Title";
            public const string Description = "Description";
            public const string ImgUrl = "ImgUrl";
            public const string EBookUrl = "EBookUrl";
            public const string FileName = "FileName";
            public const string Status = "Status";            
        }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Title)]
        public string Title { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Description)]
        public string Description { get { return _Description; } set { _Description = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ImgUrl)]
        public string ImgUrl { get { return _ImgUrl; } set { _ImgUrl = value; SetUpToDateAsFalse(); } }

        [Column(Columns.EBookUrl)]
        public string EBookUrl { get { return _EBookUrl; } set { _EBookUrl = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FileName)]
        public string FileName { get { return _FileName; } set { _FileName = value;  SetUpToDateAsFalse(); } }

        [Column(Columns.Status)]
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

        private int _Id, _StatusInt;
        private string  _ImgUrl, _Title, _Description, _EBookUrl, _FileName;

        #endregion


        public EBook(int id) : base(id) { }

        public EBook(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private EBook(string title, string description, string imgUrl, string eBookUrl, string fileName)
        {
            Title = title;
            Description = description;
            ImgUrl = imgUrl;
            EBookUrl = eBookUrl;
            FileName = fileName;
            Status = UniversalStatus.Paused;
        }

        public static void Create(string title, string description, string imgUrl, string eBookUrl, string fileName)
        {
            var ebook = new EBook(title, description, imgUrl, eBookUrl, fileName);
            ebook.Save();
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

        public void Update(string title, string description, string imgUrl, string eBookUrl, string fileName)
        {
            Title = title;
            Description = description;
            if(!string.IsNullOrEmpty(imgUrl))
                ImgUrl = imgUrl;
            if(!string.IsNullOrEmpty(eBookUrl))
                EBookUrl = eBookUrl;
            FileName = fileName;
            Save();
        }

        public static List<EBook> GetAllActiveEBooks()
        {
            return TableHelper.GetListFromRawQuery<EBook>(string.Format(@"SELECT * FROM EBooks WHERE [Status] = {0} ORDER BY [Title] DESC", (int)UniversalStatus.Active));
        }
    }
}