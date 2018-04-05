using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Prem.PTC.Utils
{
    /// <summary>
    /// Transforms given property into equivalent columm name.
    /// </summary>
    /// <example>
    /// Suppose we have class Member with property MainBalance declared as:
    /// <c>
    /// //...
    /// [Column("Balance1")]
    /// public Money MainBalance {get; set;}
    /// //...
    /// </c>
    /// And we want to get column name ("Balance1") for some purposes.
    /// In order to achieve this we use ColumnHelper:
    /// <c>
    /// string mainBalanceColumn = ColumnHelper<Member>.GetName(x => x.MainBalance);
    /// // now mainBalanceColumn == "Balance1"
    /// </c>
    /// </example>
    /// <exception cref="ArgumentException">When given property is not column</exception>
    public static class ColumnHelper<T> where T : BaseTableObject
    {
        public static string GetName<TValue>(Expression<Func<T, TValue>> item)
        {
            PropertyHelper<T> _helper = new PropertyHelper<T>();
            PropertyInfo property = _helper.GetProperty(item);
            return Column.Get(property).Name;
        }
    }
}