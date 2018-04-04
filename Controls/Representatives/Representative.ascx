<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Representative.ascx.cs" Inherits="Controls_Representative" %>

<div class="controls m-b-20">
    <div class="row">
        <div class="col-md-4">
            <h4><span id="Namelabel" runat="server"></span></h4>
            <p>
                <asp:Literal ID="UserAvatar" runat="server" />
                <br />
                <b><%=U5001.LASTACTIVITY %></b>: <span id="LastActivitySpan" runat="server"></span>
                <br />
                <b><%=L1.LANGUAGE %></b>: <span id="Languageslabel" runat="server"></span>
                <br />
                <span id="Citylabel" runat="server"></span>, 
                <span id="Countrylabel" runat="server"></span>
            </p>
        </div>
        <div class="col-md-4">
            <h4>
                <br />
            </h4>
            <p>
                <%=U4200.PHONE %>: <span id="PhoneNumberlabel" runat="server"></span>
                <br />
                <a id="Emaillabel" runat="server"></a>
                <br />
                <a runat="server" id="skypeAnhor">
                    <i class="fa fa-skype fa-2x" aria-hidden="true"></i>
                </a>
                &nbsp;
                <a id="FacebookAnhor" runat="server" target="_blank">
                    <i class="fa fa-facebook fa-2x" aria-hidden="true"></i>
                </a>
            </p>
        </div>

        <div class="col-md-4 btn-group-sm">
            <asp:LinkButton ID="JoinLinkButton" runat="server" CssClass="btn btn-success btn-block btn-sm" />
            <br />
            <a href="/user/transfer.aspx?button=7" runat="server" class="btn btn-success btn-block btn-sm" id="DepositButton" visible="false">
            <%=U6010.DEPOSITVIAREPRESENTATIVE %></a>
            <br />
            <a href="/user/cashout.aspx?b=WithdrawViaRepresentative" id="WithdrawalButton" runat="server" class="btn btn-success btn-block btn-sm" visible="false" >
            <%=U6010.WITHDRAWVIAREPRESENTATIVE %></a>
        </div>
    </div>
</div>

