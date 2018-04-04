using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Prem.PTC;

public abstract class DictionaryStatisticBase<K, V> : CacheBase
{
    protected override string Name { get { return CacheName; } }
    protected abstract string CacheName { get; }

    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromHours(1); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.Normal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }

    public abstract string GetSqlQuery { get; }
    public abstract string KeyColumnName { get; }
    public abstract string ValueColumnName { get; }

    private const int topRows = 10;

    protected override object GetDataFromSource()
    {
        return GetDictionary(true);
    }

    /// <summary>
    /// Returns List with top x predicate
    /// </summary>
    /// <param name="top">How many top rows to return</param>
    /// <returns></returns>
    public List<KeyValuePair<K, V>> GetList(bool limitRows = false, int top = topRows)
    {
        return GetList(GetDataTable(limitRows, top));
    }

    protected virtual List<KeyValuePair<K, V>> GetList(DataTable data)
    {
        return GetDictionary(data).ToList();
    }

    /// <summary>
    /// Returns Dictionary with top x predicate
    /// </summary>
    /// <param name="top">How many top rows to return</param>
    /// <returns></returns>
    public Dictionary<K, V> GetDictionary(bool limitRows = false, int top = topRows)
    {
        return GetDictionary(GetDataTable(limitRows, top));
    }

    protected virtual Dictionary<K, V> GetDictionary(DataTable data)
    {
        Dictionary<K, V> list = new Dictionary<K, V>();

        foreach (DataRow row in data.Rows)
        {
            list.Add(
                (K)Convert.ChangeType(row[KeyColumnName], typeof(K)),
                (V)Convert.ChangeType(row[ValueColumnName], typeof(V)));
        }

        return list;
    }
    
    public DataTable GetDataTable()
    {
        return GetDataTable(String.Format(GetSqlQuery, String.Empty));
    }

    /// <summary>
    /// Returns Datatable with top x predicate
    /// </summary>
    /// <param name="top">How many top rows to return</param>
    /// <returns></returns>
    public DataTable GetDataTable(bool limitRows = false, int top = topRows)
    {
        if(limitRows)
            return GetDataTable(String.Format(GetSqlQuery, "TOP " + top.ToString()));
        else
            return GetDataTable(String.Format(GetSqlQuery, String.Empty));
    }

    protected DataTable GetDataTable(string query)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            return bridge.Instance.ExecuteRawCommandToDataTable(query);
        }
    }    
}