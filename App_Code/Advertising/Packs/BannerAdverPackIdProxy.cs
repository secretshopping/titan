using System;

namespace Prem.PTC.Advertising
{
    /// <summary>
    /// Summary description for BannerAdverPackIdProxy
    /// </summary>
    public class BannerAdverPackIdProxy : BaseIdProxyObject<IBannerAdvertPack>, IBannerAdvertPack
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


        #region Construcors
        public BannerAdverPackIdProxy(int id)
            : base(x => new BannerAdvertPack(id), id){ }
        #endregion Constructors
    }
}