<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RepresentativeCountry.ascx.cs" Inherits="Controls_Countries" %>

<div class="panel panel-default">
    <div class="panel-heading">
        <h4 class="panel-title">
            <a data-toggle="collapse" data-parent="#accordion" <%="href='#accordion-" + Id +"'" %>>                
                <h5 class="m-0">
                    <asp:Image CssClass="UpperFlag" ID="flagImage" runat="server" />
                    <asp:Label ID="CountryName" runat="server" />
                </h5>
                <p id="RepresentativeCount" runat="server" class="m-0 text-muted"></p>
            </a>
        </h4>
    </div>
    <div id="accordion-<%=Id %>" class="panel-collapse collapse">
        <div class="panel-body">
            <asp:PlaceHolder ID="RepresentativesPlaceHolder" runat="server"></asp:PlaceHolder>    
        </div>
    </div>
</div>
