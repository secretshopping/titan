<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Sites.master"
    CodeFile="register.aspx.cs" Inherits="About" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContentPlaceHolder">


        <div class="container">
            <!-- begin #page-container -->
            <div id="page-container" class="row">
                <!-- begin login -->
                <div class="col-md-4 col-md-offset-4">
                    <div class="register-panel">
                        <asp:PlaceHolder runat="server" ID="SpamInfoPlaceHolder" Visible="false">
                            <div class="row">
                                <div class="col-md-12">
                                    <span class="text-success"><%=U6011.REGISTERSPAMNOTE %></span>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <h1 class="header text-center"><%=L1.REGISTER %></h1>
                        <!-- begin register-content -->
                        <titan:Register runat="server"></titan:Register>
                        <!-- end register-content -->                    

                    </div>
                </div>
                <!-- end login -->

            </div>
            <!-- end page container -->
        </div>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="FooterContentPlaceHolder">

</asp:Content>
