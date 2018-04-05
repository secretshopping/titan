using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;


namespace Prem.PTC.Utils
{
    /// <summary>
    /// Summary description for PropertyBuilder
    /// </summary>
    public class PropertyBuilder<T>
    {
        private List<PropertyInfo> _elements;
        private PropertyHelper<T> _helper;

        public PropertyBuilder()
        {
            _elements = new List<PropertyInfo>();
            _helper = new PropertyHelper<T>();
        }

        public PropertyBuilder<T> Append<TValue>(Expression<Func<T, TValue>> item)
        {
            _elements.Add(_helper.GetProperty(item));
            return this;
        }

        public PropertyInfo[] Build()
        {
            return _elements.ToArray();
        }
    }


    class PropertyHelper<T>
    {
        public PropertyInfo GetProperty<TValue>(Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }

        public PropertyInfo[] GetProperties<TValue>(params Expression<Func<T, TValue>>[] selectors)
        {
            return (from selector in selectors select GetProperty(selector)).ToArray();
        }
    }
}