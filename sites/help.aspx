<%@ Page Language="C#" AutoEventWireup="true" CodeFile="help.aspx.cs" Inherits="sites_help" MasterPageFile="~/Sites.master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <div class="content" data-scrollview="true">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U4000.SUPPORT %></h2>
            <p class="text-center"><%=L1.HELPINFO %></p>
            <div class="row p-t-30">
                <!-- begin col-3 -->
                <div class="col-md-4 col-sm-6" runat="server" id="faqDiv">
                    <div class="service service-vertical help-box">
                        <a href="sites/faq.aspx">
                            <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-question"></i></div>
                            <div class="info">
                                <h4 class="title">FAQ</h4>
                                <p class="desc"><%=L1.HELP1 %></p>
                            </div>
                        </a>
                    </div>
                </div>
                <!-- end col-3 -->
                <!-- begin col-3 -->
                <div class="col-md-4 col-sm-6" runat="server" id="forumDiv">
                    <div class="service service-vertical help-box">
                        <a href="forum/">
                            <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-comments-o"></i></div>
                            <div class="info">
                                <h4 class="title"><%=L1.FORUM %></h4>
                                <p class="desc"><%=L1.HELP2 %></p>
                            </div>
                        </a>
                    </div>
                </div>
                <!-- end col-3 -->
                <!-- begin col-3 -->
                <div class="col-md-4 col-sm-6" runat="server" id="contactDiv">
                    <div class="service service-vertical help-box">
                        <a href="sites/contact.aspx">
                            <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-envelope-o"></i></div>
                            <div class="info">
                                <h4 class="title"><%=L1.CONTACT %></h4>
                                <p class="desc"><%=L1.HELP3 %></p>
                            </div>
                        </a>
                    </div>
                </div>
                <!-- end col-3 -->
            </div>
        </div>
    </div>

    <div id="TestimonialsPlaceHolder" class="content bg-silver-lighter" data-scrollview="true" runat="server">
        <div class="container" data-animation="true" data-animation-type="fadeInDown">
            <h2 class="content-title"><%=U5008.TESTIMONIALS%></h2>
            <p class="text-center"><a href="user/testimonials.aspx"><%=U5008.LEAVEFEEDBACK %></a></p>
        </div>
    </div>
</asp:Content>
