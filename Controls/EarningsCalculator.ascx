<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EarningsCalculator.ascx.cs" Inherits="Controls_EarningsCalculator" %>

<script type="text/javascript">
    function calculate() {
 

        //Get all data
        var e1 = document.getElementById("<%=MembershipsDropDownList.ClientID%>");
        var strMembershipId = e1.options[e1.selectedIndex].value;

        var e2 = document.getElementById("<%=DropDownList1.ClientID%>");
        var strClicks = e2.options[e2.selectedIndex].value;

        var e3 = document.getElementById("<%=DropDownList2.ClientID%>");
        var strRefClicks = e3.options[e3.selectedIndex].value;

        var e4 = document.getElementById("<%=DropDownList3.ClientID%>");
        var strFacebookLikes = e4.options[e4.selectedIndex].value;

        var clicks = parseInt(strClicks);
        var refClicks = parseInt(strRefClicks);
        var likes = parseInt(strFacebookLikes);
 
        var pricePerClick = parseFloat(document.getElementById("perclick_" + strMembershipId).innerHTML);
        var pricePerRefClick = parseFloat(document.getElementById("perrefclick_" + strMembershipId).innerHTML);
        var amountPerLike = parseInt(document.getElementById("pointsperlike").innerHTML);
   
        var dailyEarnings = clicks * pricePerClick + (clicks * refClicks * pricePerRefClick);
        dailyEarnings = Math.round(dailyEarnings * <%=Money.Zero.GetMultiplier()%>) / <%=Money.Zero.GetMultiplier()%>;

        if (clicks == 0)
            dailyEarnings = 0.0;

        var monthlyEarnings = dailyEarnings * 30;
        monthlyEarnings = Math.round(monthlyEarnings * <%=Money.Zero.GetMultiplier()%>) / <%=Money.Zero.GetMultiplier()%>;

        var yearlyEarnings = dailyEarnings * 365;
        yearlyEarnings = Math.round(yearlyEarnings * <%=Money.Zero.GetMultiplier()%>) / <%=Money.Zero.GetMultiplier()%>;
        
        var decimalPlaces = <%=CoreSettings.GetMaxDecimalPlaces()%>;
        var sign = <%=AppSettings.Site.CurrencySign%>;

        document.getElementById("daily").innerHTML = sign + dailyEarnings.toFixed(decimalPlaces);
        document.getElementById("monthly").innerHTML = sign + monthlyEarnings.toFixed(decimalPlaces);
        document.getElementById("yearly").innerHTML = sign + yearlyEarnings.toFixed(decimalPlaces);
   
        if (likes > 0) {
            var points = amountPerLike * likes;

            document.getElementById("daily").innerHTML += " +" + points + "p";
            document.getElementById("monthly").innerHTML += " +" + points * 30 + "p";
            document.getElementById("yearly").innerHTML += " +" + points * 365 + "p";
        }

    }
</script>


<asp:Panel ID="MainPanel" runat="server">

    <asp:Literal ID="ConstantsLiteral" runat="server"></asp:Literal>

    <div class="box" style="width: 300px;">

        <h2>Earnings calculator</h2>

        <table>
            <tr>
                <td><%=Resources.L1.MEMBERSHIP %>:</td>
                <td style="padding: 2px; padding-left: 10px;">
                    <asp:DropDownList ID="MembershipsDropDownList" runat="server" onchange="calculate()"></asp:DropDownList></td>
            </tr>

            <tr>
                <td>Clicks per day:</td>
                <td style="padding: 2px; padding-left: 10px;">
                    <asp:DropDownList ID="DropDownList1" runat="server" OnInit="DropDownList_Init" onchange="calculate()"></asp:DropDownList></td>
            </tr>

            <tr>
                <td><%=Resources.L1.REFERRALS %>:</td>
                <td style="padding: 2px; padding-left: 10px;">
                    <asp:DropDownList ID="DropDownList2" runat="server" OnInit="DropDownList_Init" onchange="calculate()"></asp:DropDownList></td>
            </tr>

            <tr>
                <td>Facebook likes per day:</td>
                <td style="padding: 2px; padding-left: 10px;">
                    <asp:DropDownList ID="DropDownList3" runat="server" OnInit="DropDownList_Init" onchange="calculate()"></asp:DropDownList></td>
            </tr>
        </table>
        <br />
        <h4>Daily profit: <span style="color: green" id="daily"></span>
            <br />
            Monthly profit: <span style="color: green" id="monthly"></span>
            <br />
            Yearly profit: <span style="color: green" id="yearly"></span>
        </h4>
    </div>

    <script type="text/javascript">
        calculate();
    </script>

</asp:Panel>

