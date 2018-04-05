<%@ Page Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeFile="offers.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="PageHeadContent">
    
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="PageMainContent">

    <input type="hidden" name="__EVENTARGUMENT5" id="__EVENTARGUMENT5" value="" />


    <h1 class="page-header"><%=U5009.OFFERWALLS %></h1>
    <div class="row">
        <div class="col-md-12">
            <asp:PlaceHolder runat="server" ID="SpacePlaceHolder" Visible="false">

            </asp:PlaceHolder>
        </div>
    </div>
    
    <div class="row">
        <div class="col-md-12">
            <div class="nav nav-tabs custom text-right">
                <div class="TitanViewPage TitanViewPageMany NewTitanViewPage" style="<%=DisplayNoneIfEmpty %>">
                    <asp:PlaceHolder ID="MenuButtonPlaceHolder" runat="server"></asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <div class="tab-content">
        <asp:MultiView ID="MenuMultiView" runat="server">
        </asp:MultiView>
    </div>

</asp:Content>
