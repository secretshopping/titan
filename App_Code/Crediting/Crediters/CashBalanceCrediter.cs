﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Advertising;
using Prem.PTC.Contests;
using Prem.PTC.Referrals;
using MarchewkaOne.Titan.Balances;

namespace Titan
{
    public class CashBalanceCrediter : Crediter
    {
        public CashBalanceCrediter(Member User) : base(User)
        {
        }

        public void TryCreditReferer(Money cash)
        {
            if (AppSettings.Payments.CashBalanceEnabled)
            {
                base.CreditReferersMainBalance(cash, "Cash Balance deposit /ref/" + User.Name, BalanceLogType.CashBalanceDepositComission);
            }
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.CashBalanceDepositPercent);
        }
    }
}