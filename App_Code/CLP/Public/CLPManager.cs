﻿using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Members;

namespace Titan.CLP
{
    public class CLPManager
    {
        public static void CRON()
        {        
        }

        public static bool HasCLP
        {
            get
            {
                return (bool)HttpContext.Current.Application["CLP"];
            }
        }

		public static string GetChatTitle(Member user)
		{
			return "";
		}

		public static Money ProceedWithCPA(int points, Member user, bool isReverse = false)
        {
            return new Money(points);
        }

        public static Money ProceedWithCPA(Money money, Member user, bool isReverse = false)
        {
            return money;
        }

        public static Money ProceedWithOfferwall(int points, Member user)
        {
            return new Money(points);
        }

        public static Money ProceedWithOfferwall(Money money, Member user)
        {
            return money;
        }

        public static Member CheckCashoutBonus(Member user, Money cashoutAmount)
        {
            return user;
        }
    }
}