﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Prem.PTC.Utils.NVP
{
    /// <summary>
    /// Summary description for NameValuePairs
    /// </summary>
    public class NameValuePairs : IDictionary<string, string>
    {
        protected IDictionary<string, string> nameValuePairs;

        public NameValuePairs()
        {
            nameValuePairs = new Dictionary<string, string>();
        }

        public NameValuePairs(int capacity)
        {
            nameValuePairs = new Dictionary<string, string>(capacity);
        }

        public static NameValuePairs Parse(string nvps)
        {
            return parse(nvps, x => HttpUtility.UrlDecode(x));
        }

        public static NameValuePairs ParseDecoded(string nvps)
        {
            return parse(nvps, x => x);
        }

        private static NameValuePairs parse(string nvps, Func<string, string> decodingFunc)
        {
            NameValuePairs list = new NameValuePairs();

            var nameBuilder = new StringBuilder();
            var valueBuilder = new StringBuilder();

            for (int i = -1; i < nvps.Length; )
            {
                for (++i; i < nvps.Length && nvps[i] != '='; ++i) nameBuilder.Append(nvps[i]);
                for (++i; i < nvps.Length && nvps[i] != '&'; ++i) valueBuilder.Append(nvps[i]);

                string name = decodingFunc.Invoke(nameBuilder.ToString());
                string value = decodingFunc.Invoke(valueBuilder.ToString());

                list[name] = value;

                nameBuilder.Clear();
                valueBuilder.Clear();
            }

            return list;
        }

        public override string ToString()
        {
            var stringBuilder = new NVPStringBuilder();

            foreach (var nvp in this) stringBuilder.Append(nvp);

            return stringBuilder.Build();
        }


        #region IDictionary interface implementation

        public void Add(string key, string value)
        {
            nameValuePairs.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return nameValuePairs.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return nameValuePairs.Keys; }
        }

        public bool Remove(string key)
        {
            return nameValuePairs.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return nameValuePairs.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return nameValuePairs.Values; }
        }

        public string this[string key]
        {
            get { return nameValuePairs[key]; }
            set { nameValuePairs[key] = value; }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            nameValuePairs.Add(item);
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return nameValuePairs.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            nameValuePairs.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return nameValuePairs.Remove(item);
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return (nameValuePairs as IEnumerable<KeyValuePair<string, string>>).GetEnumerator();
        }

        public void Clear()
        {
            nameValuePairs.Clear();
        }

        public int Count
        {
            get { return nameValuePairs.Count; }
        }

        public bool IsReadOnly
        {
            get { return nameValuePairs.IsReadOnly; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return nameValuePairs.GetEnumerator();
        }

        #endregion IDictionary interface implementation
    }

    public class NotNullNameValuePairs : NameValuePairs
    {
        public NotNullNameValuePairs()
            : base()
        {
        }

        public string this[string key]
        {
            get
            {
                try
                {
                    return base.nameValuePairs[key];
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
            set { base.nameValuePairs[key] = value; }
        }

        public static NotNullNameValuePairs Parse(string nvps)
        {
            return parse(nvps, x => HttpUtility.UrlDecode(x));
        }

        public static NotNullNameValuePairs ParseDecoded(string nvps)
        {
            return parse(nvps, x => x);
        }

        private static NotNullNameValuePairs parse(string nvps, Func<string, string> decodingFunc)
        {

            NotNullNameValuePairs list = new NotNullNameValuePairs();

            if (nvps != null)
            {
                var nameBuilder = new StringBuilder();
                var valueBuilder = new StringBuilder();

                for (int i = -1; i < nvps.Length; )
                {
                    for (++i; i < nvps.Length && nvps[i] != '='; ++i) nameBuilder.Append(nvps[i]);
                    for (++i; i < nvps.Length && nvps[i] != '&'; ++i) valueBuilder.Append(nvps[i]);

                    string name = decodingFunc.Invoke(nameBuilder.ToString());
                    string value = decodingFunc.Invoke(valueBuilder.ToString());

                    list[name] = value;

                    nameBuilder.Clear();
                    valueBuilder.Clear();
                }
            }
            return list;
        }
    }
}