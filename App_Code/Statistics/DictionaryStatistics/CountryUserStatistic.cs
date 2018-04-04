
public class CountryUserStatistic : DictionaryStatisticBase<string, int>
{
    protected override string CacheName { get { return "CountryUserTopStatistic"; } }

    public override string GetSqlQuery
    {
        get
        {
            return
            @"SELECT {0}
                Country AS [Key]
                , COUNT(Country) AS [Value]
            FROM
                Users
            WHERE AccountStatusInt != 3
            GROUP BY Country
            ORDER BY [Value] DESC";
        }
    }

    public override string KeyColumnName{ get { return "Key"; } }
    public override string ValueColumnName { get { return "Value"; } }

    
}