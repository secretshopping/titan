using System;

namespace Prem.PTC.Advertising
{
    /// <summary>
    /// Proxy class for AdvertPack class containing 
    /// </summary>
    /// <seealso cref="http://en.wikipedia.org/wiki/Proxy_pattern"/>
    public class AdvertPackIdProxy : BaseIdProxyObject<IAdvertPack>, IAdvertPack
    {
        #region Columns
        public Utils.End Ends
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.Ends;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.Ends = value;
            }
        }

        public bool IsVisibleByMembers
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.IsVisibleByMembers;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.IsVisibleByMembers = value;
            }
        }

        public string Name
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.Name;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.Name = value;
            }
        }

        public Money Price
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.Price;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.Price = value;
            }
        }
        #endregion Columns


        #region Constructors
        public AdvertPackIdProxy(Func<int, IAdvertPack> iAdvertPackGetter, int id)
            : base(iAdvertPackGetter, id) { }
        #endregion Constructors
    }
}