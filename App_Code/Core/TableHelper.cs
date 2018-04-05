using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.Reflection;
using Prem.PTC.Members;

/// <summary>
/// Summary description for BaseTableObjectHelper
/// </summary>
public static class TableHelper
{
    /// <exception cref="DbException" />
    public static bool RowExists(string tableName, Dictionary<string, object> where)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            return bridge.Instance.Count(tableName, where) > 0;
        }
    }

    /// <exception cref="DbException" />
    public static bool RowExists(string tableName, string columnName, object columnValue)
    {
        return RowExists(tableName, MakeDictionary(columnName, columnValue));
    }

    /// <exception cref="DbException" />
    public static void DeleteRows(string tableName, string columnName, object columnValue)
    {
        DeleteRows(tableName, MakeDictionary(columnName, columnValue));
    }

    /// <exception cref="DbException" />
    public static void DeleteRows(string tableName, Dictionary<string, object> where)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            bridge.Instance.Delete(tableName, where);
        }
    }

    /// <exception cref="DbException" />
    public static void DeleteRows<T>(Dictionary<string, object> where) where T : BaseTableObject
    {
        string tableName = GetTableName<T>();

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            bridge.Instance.Delete(tableName, where);
        }
    }

    /// <exception cref="DbException" />
    public static void DeleteRows<T>(string column, object value) where T : BaseTableObject
    {
        DeleteRows<T>(TableHelper.MakeDictionary(column, value));
    }

    /// <exception cref="DbException" />
    public static void DeleteAllRows<T>() where T : BaseTableObject
    {
        DeleteRows<T>(null);
    }


    /// <exception cref="DbException" />
    public static Dictionary<string, object> MakeDictionary(string column, object value)
    {
        var dictionary = new Dictionary<string, object>();
        dictionary[column] = value;

        return dictionary;
    }

    /// <exception cref="DbException" />
    public static List<T> SelectRows<T>(Dictionary<string, object> where) where T : BaseTableObject
    {
        string tableName = GetTableName<T>();

        return SelectRows<T>(tableName, where);
    }

    /// <exception cref="DbException" />
    public static List<T> SelectAllRows<T>() where T : BaseTableObject
    {
        return SelectRows<T>(null);
    }

    /// <summary>
    /// Returns table name basing on static property TableName that BaseTableObject should have
    /// </summary>
    private static string GetTableName<T>() where T : BaseTableObject
    {
        const string PROPERTY_NAME = "TableName";

        PropertyInfo property = typeof(T).GetProperty(PROPERTY_NAME, BindingFlags.Public | BindingFlags.Static);

        return property.GetValue(null, null) as string;
    }

    private static List<T> SelectRows<T>(string tableName, Dictionary<string, object> where) where T : BaseTableObject
    {
        DataTable allObjectsTable;
        Database ToBeUsed = Database.Client;

        if (tableName == "AuthenticationPairs" || tableName == "PowerPacks")
            ToBeUsed = Database.Service;

        using (var bridge = ParserPool.Acquire(ToBeUsed))
        {
            allObjectsTable = bridge.Instance.Select(tableName, where);
        }

        List<T> list = new List<T>(allObjectsTable.Rows.Count);
        var TType = typeof(T);

        foreach (DataRow row in allObjectsTable.Rows)
            list.Add(Activator.CreateInstance(TType, row, true) as T);

        return list;
    }

    public static List<T> GetListFromDataTable<T>(DataTable allObjectsTable, int maxCount, bool noMaxCount = false) where T : BaseTableObject
    {
        int i = 0;
        List<T> list = new List<T>(allObjectsTable.Rows.Count);
        var TType = typeof(T);

        foreach (DataRow row in allObjectsTable.Rows)
        {
            if (i == maxCount && !noMaxCount)
                break;
            list.Add(Activator.CreateInstance(TType, row, true) as T);
            i++;
        }

        return list;
    }

    public static List<int> GetListFromDataTable(DataTable allObjectsTable, int maxCount, bool noMaxCount = false)
    {
        int i = 0;
        List<int> list = new List<int>(allObjectsTable.Rows.Count);

        foreach (DataRow row in allObjectsTable.Rows)
        {
            if (i == maxCount && !noMaxCount)
                break;
            list.Add(int.Parse(row.ItemArray[0].ToString()));
            i++;
        }

        return list;
    }

    public static List<string> GetStringListFromDataTable(DataTable allObjectsTable, int maxCount, bool noMaxCount = false)
    {
        int i = 0;
        List<string> list = new List<string>(allObjectsTable.Rows.Count);

        foreach (DataRow row in allObjectsTable.Rows)
        {
            if (i == maxCount && !noMaxCount)
                break;
            list.Add(row.ItemArray[0].ToString());
            i++;
        }

        return list;
    }

    public static List<T> GetListFromDataTable<T>(DataTable allObjectsTable) where T : BaseTableObject
    {
        return GetListFromDataTable<T>(allObjectsTable, 100, true);
    }

    public static List<int> GetListFromDataTable(DataTable allObjectsTable)
    {
        return GetListFromDataTable(allObjectsTable, 100, true);
    }

    public static List<string> GetStringListFromDataTable(DataTable allObjectsTable)
    {
        return GetStringListFromDataTable(allObjectsTable, 100, true);
    }

    /// <summary>
    /// Gets the list from the SQL query. The beginning is already written: "SELECT * FROM [TableName] " (SPACE is here!)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="whereQuery">The where query.</param>
    /// <returns></returns>
    public static List<T> GetListFromQuery<T>(string whereQuery) where T : BaseTableObject
    {
        return GetListFromRawQuery<T>("SELECT * FROM " + GetTableName<T>() + " " + whereQuery);
    }

    public static List<T> GetListFromRawQuery<T>(string query) where T : BaseTableObject
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            var result = bridge.Instance.ExecuteRawCommandToDataTable(query);
            return TableHelper.GetListFromDataTable<T>(result);
        }
    }

    public static DataTable GetDataTableFromRawQuery(string query)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            return bridge.Instance.ExecuteRawCommandToDataTable(query);
        }
    }

    public static List<int> GetListFromRawQuery(string query)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            var result = bridge.Instance.ExecuteRawCommandToDataTable(query);
            return TableHelper.GetListFromDataTable(result);
        }
    }

    public static List<string> GetStringListFromRawQuery(string query)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            var result = bridge.Instance.ExecuteRawCommandToDataTable(query);
            return TableHelper.GetStringListFromDataTable(result);
        }
    }

    public static int GetNextId(string tableName)
    {
        string query = String.Format("SELECT IDENT_CURRENT('{0}') + IDENT_INCR('{0}')", tableName);
        object result = SelectScalar(query);

        return Convert.ToInt32( result);
    }

    public static void UpdateRows(string tableName, Dictionary<string, object> what, Dictionary<string, object> where)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            bridge.Instance.Update(tableName, what, where);
        }
    }

    /// <exception cref="DbException" />
    public static int CountOf<T>(Dictionary<string, object> where) where T : BaseTableObject
    {
        string tableName = GetTableName<T>();

        return CountOf<T>(tableName, where);
    }

    /// <exception cref="DbException" />
    public static int CountOf<T>() where T : BaseTableObject
    {
        return CountOf<T>(null);
    }

    private static int CountOf<T>(string tableName, Dictionary<string, object> where) where T : BaseTableObject
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            return bridge.Instance.Count(tableName, where);
        }
    }


    /// <summary>
    /// Returns List made of DbList formatted as el1#el2#el3. El1, el2, el3 etc.
    /// </summary>
    public static List<Money> GetMoneyListFromString(string collection)
    {
        string[] elements = collection.Split('#');

        return (from element in elements
                select Money.Parse(element)).ToList();
    }

    /// <summary>
    /// Returns List made of DbList formatted as el1#el2#el3. El1, el2, el3 etc.
    /// Empty collection contains only "-1"
    /// </summary>
    public static List<int> GetIntListFromString(string collection)
    {
        string[] elements = collection.Split('#');

        return (from element in elements
                select Int32.Parse(element)).ToList();
    }

    /// <summary>
    /// Returns string made of List<int> formatted as el1#el2#el3. El1, el2, el3 etc.
    /// Empty string is "-1"
    /// </summary>
    public static string GetStringFromIntList(List<int> collection)
    {

        var elements = new StringBuilder(""); int i = 0;

        foreach (int el in collection)
        {
            elements.Append("#");
            elements.Append(el.ToString());
            if (i == 0) elements = new StringBuilder(el.ToString());
            ++i;
        }

        return elements.ToString();
    }

    /// <summary>
    /// Returns string made of List<Money> formatted as el1#el2#el3. El1, el2, el3 etc.
    /// </summary>
    public static string GetStringFromMoneyList(List<Money> collection)
    {
        var elements = new StringBuilder("");
        int i = 0;

        foreach (Money el in collection)
        {
            elements.Append("#");
            elements.Append(el.ToClearString());
            if (i == 0) elements = new StringBuilder(el.ToClearString());
            ++i;
        }

        return elements.ToString();
    }

    /// <summary>
    /// Recalculates the list : 0->1 1->2 ... 14=DELETED (if list had 14 elements)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="?"></param>
    /// <returns></returns>
    public static List<T> RecalculateList<T>(List<T> list, T zerovalue)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            list[i] = list[i - 1];
        }
        list[0] = zerovalue;

        return list;

    }

    /// <summary>
    /// Recalculates the list : 0->1 1->2 ... 14=DELETED (if list had 14 elements)
    /// UBER FAST
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    public static string FastRecalculate(string list)
    {
        bool IsMoney = false;
        //check if we have money
        if (list.Contains("."))
            IsMoney = true;

        StringBuilder sb;

        if (IsMoney)
            sb = new StringBuilder("0.000#");
        else
            sb = new StringBuilder("0#");

        sb.Append(list.Remove(list.LastIndexOf('#')));
        return sb.ToString();
    }

    /// <summary>
    /// DateParameter must have @DATE name
    /// </summary>
    /// <param name="command"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static SqlCommand GetSqlCommand(string command, DateTime parameter)
    {
        SqlCommand fCommand = new SqlCommand();
        fCommand.CommandText = command;
        fCommand.Parameters.Add("@DATE", SqlDbType.DateTime);
        fCommand.Parameters["@DATE"].Value = parameter;

        return fCommand;
    }

    public static string GetRawRecalculateCommand(string TableName, string ColumnName, bool IsMoney)
    {
        // We want to create this SQL query:
        //"UPDATE Users SET StatsClicks = SUBSTRING(StatsClicks, 1, DATALENGTH(StatsClicks)-CHARINDEX('#', REVERSE(cast(StatsClicks as varchar(max))))); UPDATE Users SET StatsClicks = '0#' + cast(StatsClicks as varchar(max));"

        string zero = "0";
        if (IsMoney)
            zero = "0.000";

        StringBuilder sb = new StringBuilder();
        sb.Append("UPDATE ")
          .Append(TableName)
          .Append(" SET ")
          .Append(ColumnName)
          .Append(" = SUBSTRING(")
          .Append(ColumnName)
          .Append(", 1, DATALENGTH(")
          .Append(ColumnName)
          .Append(")-CHARINDEX('#', REVERSE(cast(")
          .Append(ColumnName)
          .Append(" as varchar(max))))); UPDATE ")
          .Append(TableName)
          .Append(" SET ")
          .Append(ColumnName)
          .Append(" = '")
          .Append(zero)
          .Append("#' + cast(")
          .Append(ColumnName)
          .Append(" as varchar(max));");

        return sb.ToString();
    }
    public static object SelectScalar(string query)
    {
        object returnObject;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            returnObject = (bridge.Instance.ExecuteRawCommandScalar(query));
        }
        return returnObject;
    }
    public static void ExecuteRawCommandNonQuery(string query)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            bridge.Instance.ExecuteRawCommandNonQuery(query);
        }
    }
}