using System;

namespace Prem.PTC.Advertising
{
    /// <summary>
    /// Summary description for PtcAdvertPackIdProxy
    /// </summary>
    public class TrafficGridAdvertPackIdProxy : BaseIdProxyObject<ITrafficGridAdvertPack>, ITrafficGridAdvertPack
    {
        #region Columns

        public TimeSpan DisplayTime
        {
            get
            {
                SetInstanceIfNeeded();
                return proxyObject.DisplayTime;
            }
            set
            {
                SetInstanceIfNeeded();
                proxyObject.DisplayTime = value;
            }
        }

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

        public TrafficGridAdvertPackIdProxy(int id) : base(x => new TrafficGridAdvertPack(id), id) { }

        #endregion Constructors
    }
}