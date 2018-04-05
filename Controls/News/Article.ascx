<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Article.ascx.cs" Inherits="Controls_News_Article" %>


<asp:PlaceHolder ID="HeadlinerPlaceHolder" runat="server" Visible="false">
    <div class="post-detail section-container">
        <div class="row">
            <div class="col-md-6">
                <!-- begin post-image -->
                <div class="post-image">
                    <a href="<%=Object.GetLink() %>">
                        <img src="<%=Object.GetCovertImageURL() %>" alt="">
                    </a>
                </div>
            </div>
            <div class="col-md-6">
                <h4 class="post-title">
                    <a href="<%=Object.GetLink() %>"><%=Object.Title %></a>
                </h4>
                <div class="post-by">
                    <%=U6013.POSTEDBY %> <a href="sites/profile.aspx?u=<%=Author.Name %>"><%=Author.FirstName %> <%=Author.SecondName %> (<%=Author.Name %>)</a> <span class="divider">|</span> <%=Object.CreatedDate.ToShortDateString() %> <span class="divider">|</span> <a href="#"><%=Category.Text %></a> 
                </div>
                <div class="post-desc">
                    <p class="ellipsis">
                        <%=Object.GetRawText() %>
                    </p>
                </div>
            </div>
        </div>
        <div class="read-btn-container">
            <a href="<%=Object.GetLink() %>"><%=U6013.READMORE %> <i class="fa fa-angle-double-right"></i></a>
        </div>
    </div>
</asp:PlaceHolder>



<asp:PlaceHolder ID="MainPlaceHolder" runat="server" Visible="false">


    <li>
        <!-- begin post-left-info -->
        <div class="post-left-info">
            <div class="post-date">
                <span class="day"><%=Object.CreatedDate.Day.ToString() %></span>
                <span class="month"><%=Object.CreatedDate.ToString("MMM") %></span>
            </div>
        </div>
        <!-- end post-left-info -->
        <!-- begin post-content -->
        <div class="post-content">
            <!-- begin post-image -->
            <div class="post-image">
                <a href="<%=Object.GetLink() %>">
                    <img src="<%=Object.GetCovertImageURL() %>" alt=""></a>
            </div>
            <!-- end post-image -->
            <!-- begin post-info -->
            <div class="post-info">
                <h4 class="post-title">
                    <a href="<%=Object.GetLink() %>"><%=Object.Title %></a>
                </h4>
                <div class="post-by">
                    <%=U6013.POSTEDBY %> <a href="sites/profile.aspx?u=<%=Author.Name %>"><%=Author.FirstName %> <%=Author.SecondName %> (<%=Author.Name %>)</a> <span class="divider">|</span> <a href="#"><%=Category.Text %></a>
                                   
                </div>
                <div class="post-desc">
                    <%=Object.ShortDescription %> [...]
                                   
                </div>
            </div>
            <!-- end post-info -->
            <!-- begin read-btn-container -->
            <div class="read-btn-container">
                <a href="<%=Object.GetLink() %>" class="read-btn"><%=U6013.READMORE %> <i class="fa fa-angle-double-right"></i></a>
            </div>
            <!-- end read-btn-container -->
        </div>
        <!-- end post-content -->
    </li>
</asp:PlaceHolder>



<asp:PlaceHolder ID="CategoryPlaceHolder" runat="server" Visible="false">
    <li>
        <div class="post-image small">
            <img src="<%=Object.GetCovertImageURL() %>" alt="">
        </div>
        <div class="info">
            <h4 class="title"><a href="<%=Object.GetLink() %>"><%=Object.Title %></a></h4>
            <div class="date"><%=Object.CreatedDate.ToString("d MMMM yyyy") %></div>
        </div>
    </li>
</asp:PlaceHolder>



<asp:PlaceHolder ID="WorldNewsPlaceHolder" runat="server" Visible="false">
    <li>
        <div class="info">
            <h4 class="title"><a href="<%=Object.GetLink() %>"><%=Object.Title %></a></h4>
            <div class="date"><%=Object.CreatedDate.ToString("d MMMM yyyy") %></div>
        </div>
    </li>
</asp:PlaceHolder>
