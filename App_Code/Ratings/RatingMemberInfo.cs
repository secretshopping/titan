using System;

[Serializable]
public class RatingMemberInfo
{
    public int TotalActions { get; set; }

    public int TotalPositiveActions { get; set; }

    public RatingMemberInfo(int totalActions, int totalPositiveActions)
    {
        this.TotalActions = totalActions;
        this.TotalPositiveActions = totalPositiveActions;
    }
}