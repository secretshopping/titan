using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{
    public enum GatewayCashflowDirection
    {
        Null = 0,
        FromGate = 1,
        ToGate = 2,
        Both = 3
    }

    public static class GatewayCashflowDirectionExtensions
    {
        public static string GetDescription(this GatewayCashflowDirection direction)
        {
            switch (direction)
            {
                case GatewayCashflowDirection.Null: return "N/A";
                case GatewayCashflowDirection.FromGate: return "Sending money to members";
                case GatewayCashflowDirection.ToGate: return "Receiving money from members";
                case GatewayCashflowDirection.Both: return "Sending and receiving money";
                default: throw new NotImplementedException(direction + " not considered");
            }
        }
    }
}