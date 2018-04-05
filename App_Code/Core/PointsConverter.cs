using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Titan.Cryptocurrencies;

namespace Titan
{
    public class PointsConverter
    {
        /// <summary>
        /// Convert Money value into Points using Virtual Currency ratio
        /// </summary>
        /// <param name="input">Amount of money</param>
        /// <returns></returns>
        public static int ToPoints(Money input)
        {
            int pointRate = Points.GetPointsPer1d();

            return ToPoints(input, pointRate);
        }

        /// <summary>
        /// Convert Money value into Points using custom ratio
        /// </summary>
        /// <param name="input">Amount of money</param>
        /// <param name="pointRate">1 Money = x Points</param>
        /// <returns></returns>
        public static int ToPoints(Money input, Decimal pointRate)
        {
            Decimal Result = input.ToDecimal() * pointRate;

            return Convert.ToInt32(Result);
        }
        
        /// <summary>
        /// Convert Points into Money using Virtual Currency ratio
        /// </summary>
        /// <param name="Input">Amount of points</param>
        /// <returns></returns>
        public static Money ToMoney(int Input)
        {
            Decimal input = new Decimal(Input);
            return ToMoney(input);
        }

        /// <summary>
        /// Convert Points into Money using Virtual Currency ratio
        /// </summary>
        /// <param name="Input">Amount of points</param>
        /// <returns></returns>
        public static Money ToMoney(Decimal input)
        {
            int pointRate = Points.GetPointsPer1d();

            return ToMoney(input, pointRate);
        }

        /// <summary>
        /// Convert Points into Money using custom ratio
        /// </summary>
        /// <param name="input">Amount of points</param>
        /// <param name="pointRate">>1 Money = x Points</param>
        /// <returns></returns>
        public static Money ToMoney(Decimal input, Decimal pointRate)
        {
            Money amount = new Money(input / pointRate);

            return amount;
        }
    }
}