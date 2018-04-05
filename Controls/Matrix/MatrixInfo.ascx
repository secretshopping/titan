<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MatrixInfo.ascx.cs" Inherits="Controls_Matrix_MatrixInfo" %>

<div class="alert note note-info">
    <span class="close" data-dismiss="alert">×</span>
    <h4><asp:Label runat="server"><%=string.Format(U6003.MATRIXWILLSHUFFLE, Prem.PTC.Utils.TimeSpanExtensions.ToFriendlyDisplay(timeToShuffleMatrix, 3)) %></asp:Label></h4>
    <br />
    <asp:PlaceHolder runat="server" ID="MatrixRestrictionPlaceHolder" Visible="false">
        <asp:Literal runat="server" ID="MatrixRestrictionLiteral"></asp:Literal>
        <br />
    </asp:PlaceHolder>
    <br />
    <%=U6003.LEVELSINMATRIX %>: <%=maxLevelWithChildrenCount.Key %>
    <br />
    <%=U6003.SLOTSTAKENINLASTLEVEL %>: <%=maxLevelWithChildrenCount.Value %> / <%=Math.Pow(AppSettings.Matrix.MaxChildrenInMatrix, maxLevelWithChildrenCount.Key).ToString() %>
    <br />
    <asp:PlaceHolder runat="server" ID="MyMatrixLevelPlaceHolder" Visible ="false">
        <asp:Literal runat="server" ID="MyMatrixLevelLiteral"></asp:Literal>
    </asp:PlaceHolder>
</div>
