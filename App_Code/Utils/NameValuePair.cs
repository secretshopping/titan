using System;
using System.Collections.Generic;

namespace Prem.PTC.Utils.NVP
{
    /// <summary>
    /// Summary description for NameValuePair
    /// </summary>
    public struct NameValuePair
    {
        private string _name, _value;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public NameValuePair(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public static implicit operator KeyValuePair<string, string>(NameValuePair pair)
        {
            return new KeyValuePair<string, string>(pair.Name, pair.Value);
        }

        public static implicit operator NameValuePair(KeyValuePair<string, string> pair)
        {
            return new NameValuePair(pair.Key, pair.Value);
        }

        public static implicit operator NameValuePair(KeyValuePair<string, object> pair)
        {
            return new NameValuePair(pair.Key, Convert.ToString(pair.Value));
        }

        public override string ToString()
        {
            return Name + '=' + Value;
        }
    }
}