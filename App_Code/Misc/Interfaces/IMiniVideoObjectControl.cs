using Titan.MiniVideos;

public interface IMiniVideoObjectControl
{
    MiniVideoCampaign Object { get; set; }
    void DataBind();
}