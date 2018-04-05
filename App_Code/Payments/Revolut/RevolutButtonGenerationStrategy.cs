using System;

namespace Prem.PTC.Payments
{
    public class RevolutButtonGenerationStrategy : ButtonGenerationStrategy
    {
        protected RevolutAccountDetails account;

        public RevolutButtonGenerationStrategy(RevolutAccountDetails revolut)
        {
            account = revolut;
        }

        protected override string generate(string itemName, Money amount, string command, object[] args)
        {
            

            string url = String.Empty;

            return url;
        }

        public override string ToString()
        {
            return "Revolut";
        }
    }
}