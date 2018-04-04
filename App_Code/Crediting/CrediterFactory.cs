using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;

namespace Titan
{
    /// <summary>
    /// Creates /solid class for flexible and secure members crediting/ instances
    /// Just CREDITING + ACHIEVEMENTS. 
    /// </summary>
    public static class CrediterFactory
    {
        public static Crediter Acquire(string Username, CreditType Type)
        {
            Member User = new Member(Username);
            return Acquire(User, Type);
        }

        public static Crediter Acquire(Member User, CreditType Type)
        {
            switch (Type)
            {

                case CreditType.PTC:
                    return new PtcCrediter(User);

                case CreditType.FacebookLike:
                    return new FacebookCrediter(User);

                case CreditType.Contest:
                    return new ContestCrediter(User);

                case CreditType.CPAGPTOffer:
                    return new CPAGPTCrediter(User);

                case CreditType.TrafficGrid:
                    return new TrafficGridCrediter(User);

                case CreditType.TrafficExchange:
                    return new TrafficExchangeCrediter(User);

                case CreditType.Upgrade:
                    return new UpgradeCrediter(User);

                case CreditType.AdPackCrediter:
                    return new AdPackCrediter(User);

                case CreditType.CashBalanceDeposit:
                    return new CashBalanceCrediter(User);

                case CreditType.AccountActivationFee:
                    return new AccountActivationFeeCrediter(User);

                default:
                    return new CPAGPTCrediter(User);

            }
        }
    }
}