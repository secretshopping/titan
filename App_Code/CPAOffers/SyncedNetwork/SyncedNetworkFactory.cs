using Titan;
using Prem.PTC.Offers;

public class SyncedNetworkFactory
{
	public static SyncedNetwork Acquire(AffiliateNetwork Network, bool ThrowExceptions = false)
	{
        switch (Network.AffiliateNetworkSoftwareType)
        {
            case AffiliateNetworkSoftwareType.CPALead:
                return new CPALeadSyncedNetwork(Network, ThrowExceptions);

            case AffiliateNetworkSoftwareType.Performa:
                return new PerformaManager(Network, ThrowExceptions);

            case AffiliateNetworkSoftwareType.AdGateMedia:
                return new AdGateSyncedNetwork(Network, ThrowExceptions);

            case AffiliateNetworkSoftwareType.ProLeadsMedia:
                return new ProLeadsMediaSyncedNetwork(Network, ThrowExceptions);

            case AffiliateNetworkSoftwareType.PointClickTrack:
                return new PointClickTrackSyncedNetwork(Network, ThrowExceptions);

            case AffiliateNetworkSoftwareType.HasOffers:
                return new HasOffersSyncedNetwork(Network, ThrowExceptions);

            case AffiliateNetworkSoftwareType.RedFireNetwork:
                return new RedFireNetworkSyncedNetwork(Network, ThrowExceptions);
        }
        return new PerformaManager(Network, ThrowExceptions);
	}

    public static void SynchronizeAll(bool ThrowExceptions = false)
    {
        PerformaManager.SynchronizeAll(ThrowExceptions);
        CPALeadSyncedNetwork.SynchronizeAll(ThrowExceptions);
        AdGateSyncedNetwork.SynchronizeAll(ThrowExceptions);
        ProLeadsMediaSyncedNetwork.SynchronizeAll(ThrowExceptions);
        PointClickTrackSyncedNetwork.SynchronizeAll(ThrowExceptions);
        HasOffersSyncedNetwork.SynchronizeAll(ThrowExceptions);
        RedFireNetworkSyncedNetwork.SynchronizeAll(ThrowExceptions);
    }
}