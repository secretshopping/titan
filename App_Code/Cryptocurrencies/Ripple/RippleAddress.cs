using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.Cryptocurrencies
{
    public class RippleAddress
    {
        public string Address { get; set; }
        public string DestinationTag { get; set; }

        public RippleAddress(string address, string destinationTag)
        {
            Address = address;
            DestinationTag = destinationTag;
        }

        public override string ToString()
        {
            return Address + "/" + DestinationTag;
        }

        public bool HasDestinationTag()
        {
            return !String.IsNullOrWhiteSpace(DestinationTag);
        }

        public static RippleAddress FromString(string input)
        {
            var parts = input.Split('/');
            return new RippleAddress(parts[0], parts[1]);
        }

        public string ToDisplayString()
        {
            if (HasDestinationTag())
                return this.ToString();

            return Address;
        }
    }
}