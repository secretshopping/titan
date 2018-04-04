using Resources;
using System;

namespace BitcoinValidator
{
    public static class BitcoinValidator
    {

        public static bool ValidateBitcoinAddress(string address)
        {
            try
            {
                if (string.IsNullOrEmpty(address))
                    return true;
                //Coinbase email validation
                if (address.Contains("@"))
                    return true;
                NBitcoin.BitcoinAddress.Create(address);
                return true;
            }
            catch
            {
                throw new Exception(U6000.INVALIDBITCOINADDRESSINFO);
            }
        }
    }
}