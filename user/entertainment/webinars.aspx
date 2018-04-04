<%@ Page Language="C#" AutoEventWireup="true" CodeFile="webinars.aspx.cs" Inherits="user_webinars" MasterPageFile="~/User.master" %>

<asp:Content runat="server" ContentPlaceHolderID="PageMainContent">

    <h1 class="page-header"><%= U6003.WEBINARS%></h1>
    <div class="row">
        <div class="col-md-12">
            <p class="lead"><%=U6003.WEBINARSDESC %></p>
        </div>
    </div>
    <div class="TitanViewElement">
        <div class="row">
            <div class="col-md-12">
                <asp:GridView runat="server" ID="WebinarsGridView" AllowPaging="true" AllowSorting="true"
                    DataSourceID="WebinarsGridViewDataSource"
                    OnPreRender="BaseGridView_PreRender" PageSize="20" OnDataBound="WebinarsGridView_DataBound">
                    <Columns>
                        <asp:TemplateField SortExpression="Title">
                            <ItemTemplate>
                                <%# "<a href='" + Eval("Url") + "'>"+Eval("Title")+"</a>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                        <asp:BoundField DataField="Language" HeaderText="Language" SortExpression="Language" />
                        <asp:TemplateField HeaderText="Time" SortExpression="DateTime">
                            <ItemTemplate>
                                <%# ((DateTime)Eval("DateTime")).ToString() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource runat="server" ID="WebinarsGridViewDataSource"
                    OnInit="WebinarsGridViewDataSource_Init" ConnectionString="<%$ ConnectionStrings:ClientDbString %>"></asp:SqlDataSource>
            </div>
        </div>
    </div>
</asp:Content>
