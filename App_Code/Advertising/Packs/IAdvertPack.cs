using System;

namespace Prem.PTC.Advertising
{
    public interface IAdvertPack : ITableObject
    {
        global::Prem.PTC.Utils.End Ends { get; set; }
        bool IsVisibleByMembers { get; set; }
        [Obsolete]
        string Name { get; set; }
        global::Prem.PTC.Money Price { get; set; }
    }

    public interface IBannerAdvertPack : IAdvertPack { }

    public interface IPtcAdvertPack : IAdvertPack
    {
        TimeSpan DisplayTime { get; set; }
        Money MinUserBalance { get; set; }
        Money MaxUserBalance { get; set; }
    }

    public interface ITrafficGridAdvertPack : IAdvertPack
    {
        TimeSpan DisplayTime { get; set; }
    }

    public interface IFacebookAdvertPack : IAdvertPack { }

}