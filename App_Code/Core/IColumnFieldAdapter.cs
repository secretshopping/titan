using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC
{
    public interface IColumnFieldAdapter<T> where T : ColumnFieldData
    {
        void BindToColumns(T valuesToFill);
    }

    public class ColumnFieldData
    {
    }
}