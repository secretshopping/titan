<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EBook.ascx.cs" Inherits="Controls_Misc_EBook" %>
<%@ Import Namespace="Prem.PTC.Utils" %>

<asp:PlaceHolder ID="EBookPlaceHolder" runat="server">
    <div class="col-xs-12 col-sm-6 col-md-4 text-center p-40">
        
            <a href='<%# UrlUtils.ConvertTildePathToImgPath(EBookUrl) %>' target="_blank" style="text-decoration:none;">
                <h5 class="title f-w-700"><%=Title %></h5>
                <div class="m-t-20 m-b-20" style="background:url(<%=UrlUtils.ConvertTildePathToImgPath(ImageUrl) %>);background-size:contain;background-repeat:no-repeat;background-position:center;height:200px;width:100%;">
                    <%--<img src="<%=UrlUtils.ConvertTildePathToImgPath(ImageUrl) %>" />--%>
                </div>
            </a>
            
            
            <div class="desc text-left">
                <%=Description %>
            </div>
       
    </div>
</asp:PlaceHolder>
