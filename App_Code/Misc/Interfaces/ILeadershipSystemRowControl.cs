using Titan.Leadership;

public interface ILeadershipSystemRowControl
{
    LeadershipRank Rank { get; set; }
    void DataBind();
}