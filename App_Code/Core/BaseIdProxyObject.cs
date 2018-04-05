using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC
{
    /// <summary>
    /// Summary description for BaseIdProxyObject
    /// </summary>
    public abstract class BaseIdProxyObject<T> : ITableObject where T : ITableObject
    {
        #region Columns
        public int Id { get { return _id; } }

        public Database Database
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.Database;
            }
        }

        public bool IsInDatabase
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.IsInDatabase;
            }
        }

        public bool IsUpToDate
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.IsUpToDate;
            }
        }
        #endregion Columns


        private int _id;
        private Func<int, T> _iTableObjectGetter;

        protected T proxyObject = default(T);


        #region Constructors

        public BaseIdProxyObject(Func<int, T> iTableObjectGetter, int id)
        {
            _id = id;
            _iTableObjectGetter = iTableObjectGetter;
        }

        #endregion Constructors


        public void Save(bool forceSave = false)
        {
            SetInstanceIfNeeded();
            proxyObject.Save(forceSave);
        }

        public void Reload()
        {
            SetInstanceIfNeeded();
            proxyObject.Reload();
        }

        public void Delete()
        {
            SetInstanceIfNeeded();
            proxyObject.Delete();
        }

        protected void SetInstanceIfNeeded()
        {
            if (proxyObject == null)
                proxyObject = _iTableObjectGetter(_id);
        }
    }
}