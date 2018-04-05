<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<!--[if IE 8]> <html lang="<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName %>" class="ie8"> <![endif]-->
<!--[if !IE]><!-->
<html lang="<%=System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName %>">
<!--<![endif]-->
<head id="YafHead">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" name="viewport" />
    <link rel="shortcut icon" type="image/ico" <%=string.Format("href='{0}'",AppSettings.Site.FaviconImageURL) %>>
    <title><%=AppSettings.Site.Name %> | <%=AppSettings.Site.Slogan %></title>

    <link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/default/assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Scripts/home/assets/css/animate.min.css" rel="stylesheet" />
    <link href="Scripts/home/assets/css/style.min.css" rel="stylesheet" />
    <link href="Scripts/home/assets/css/style-responsive.min.css" rel="stylesheet" />
    <link href="Scripts/home/assets/css/theme/default.css" id="theme" rel="stylesheet" />
    <link href="Scripts/home/assets/css/custom.css" rel="stylesheet" />
    <script src="Scripts/default/assets/plugins/jquery/jquery-1.9.1.min.js"></script>
    <script src="Scripts/default/assets/plugins/jquery/jquery-migrate-1.1.0.min.js"></script>
    <asp:Literal ID="ScriptLiteral" runat="server"></asp:Literal>

    <!-- Homepage popup start -->
    <!---
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/animate.css/3.5.2/animate.min.css" />
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/limonte-sweetalert2/7.0.3/sweetalert2.min.css" />
        <script src="https://cdnjs.cloudflare.com/ajax/libs/limonte-sweetalert2/7.0.3/sweetalert2.min.js"></script>
    
        <script>
            $(function () {            
                swal({
                    title: 'Title',
                    html: 'Main <br /> content',
                    animation: false,
                    showCloseButton: true,
                    showCancelButton: false,
                    customClass: 'animated tada',
                    type: 'warning',
                    width: '50%',
                    confirmButtonText: 'Ok',
                    buttonsStyling: false,
                    confirmButtonClass: 'btn btn-success'
                });
            });
        </script>
        --->
    <!-- Homepage popup end -->

    <!-- Facebook wall link handlers-->
    <titan:FacebookOGraphInfo runat="server" />
    <titan:CustomHeader runat="server" />
</head>
<body class="" data-spy="scroll" data-target="#header-navbar" data-offset="51">

    <!-- begin #page-loader -->
    <div id="page-loader" class="fade in"><span class="spinner"></span></div>
    <!-- end #page-loader -->
    <form runat="server">
        <titan:UseTitanDemoHeader runat="server" />
        <titan:Countdown runat="server" />
        <!-- begin #page-container -->
        <div id="page-container" class="fade">
            <!-- begin #header -->

            <div id="header" class="header navbar navbar-transparent navbar-fixed-top">
                <!-- begin container -->
                <div class="container">
                    <!-- begin navbar-header -->
                    <div class="navbar-header">
                        <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#header-navbar">
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                        </button>
                        <a href="default.aspx" class="navbar-brand">
                            <span>
                                <img src="<%=AppSettings.Site.LogoImageURL %>" class="" style="height: 30px;" /></span>
                            <% if (AppSettings.Site.ShowSiteName)
                                { %>
                            <span><%=AppSettings.Site.Name %></span>
                            <% } %>
                        </a>
                    </div>
                    <!-- end navbar-header -->
                    <!-- begin navbar-collapse -->

                    <div class="collapse navbar-collapse" id="header-navbar">

                        <titan:MainMenu runat="server" />
                    </div>

                    <!-- end navbar-collapse -->
                </div>
                <!-- end container -->
            </div>
            <!-- end #header -->


            <!-- begin #home -->
            <div id="home" class="content has-bg home">
                <!-- begin content-bg -->
                <div class="content-bg">
                    <img src="Images/Home/titan-home-bg-6.jpg" alt="Home" />
                </div>
                <!-- end content-bg -->
                <!-- begin container -->
                <div class="container home-content">
                    <h1>Welcome to <%=AppSettings.Site.Name %></h1>
                    <h3><%=AppSettings.Site.Slogan %></h3>
                    <p>
                        <%=AppSettings.Site.Description %>
                    </p>
                    <a href="register.aspx" class="btn btn-theme"><%=L1.REGISTER %></a> <a href="sites/login.aspx" class="btn btn-outline"><%=L1.LOGIN %></a><br />
                    <br />
                    Or see our local <a href="/sites/representatives.aspx">Representatives</a> for more information.
                </div>
                <!-- end container -->
            </div>
            <!-- end #home -->

            <!-- begin #about -->
            <div id="about" class="content" data-scrollview="true">
                <!-- begin container -->
                <div class="container" data-animation="true" data-animation-type="fadeInDown">
                    <h2 class="content-title">About Us</h2>
                    <p class="content-desc">
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum consectetur eros dolor,<br />
                        sed bibendum turpis luctus eget
               
                    </p>
                    <!-- begin row -->
                    <div class="row">
                        <!-- begin col-4 -->
                        <div class="col-md-4 col-sm-6">
                            <!-- begin about -->
                            <div class="about">
                                <h3>Our Story</h3>
                                <p>
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                                Vestibulum posuere augue eget ante porttitor fringilla. 
                                Aliquam laoreet, sem eu dapibus congue, velit justo ullamcorper urna, 
                                non rutrum dolor risus non sapien. Vivamus vel tincidunt quam. 
                                Donec ultrices nisl ipsum, sed elementum ex dictum nec. 
                           
                                </p>
                                <p>
                                    In non libero at orci rutrum viverra at ac felis. 
                                Curabitur a efficitur libero, eu finibus quam. 
                                Pellentesque pretium ante vitae est molestie, ut faucibus tortor commodo. 
                                Donec gravida, eros ac pretium cursus, est erat dapibus quam, 
                                sit amet dapibus nisl magna sit amet orci. 
                           
                                </p>
                            </div>
                            <!-- end about -->
                        </div>
                        <!-- end col-4 -->
                        <!-- begin col-4 -->
                        <div class="col-md-4 col-sm-6">
                            <h3>Our Philosophy</h3>
                            <!-- begin about-author -->
                            <div class="about-author">
                                <div class="quote bg-silver">
                                    <i class="fa fa-quote-left"></i>
                                    <h3>We work harder,<br />
                                        <span>to let our user keep simple</span></h3>
                                    <i class="fa fa-quote-right"></i>
                                </div>
                                <div class="author">
                                    <div class="image">
                                        <img src="Images/Home/user-1.jpg" alt="Sean Ngu" />
                                    </div>
                                    <div class="info">
                                        Sean Ngu
                                   
                                    <small>Front End Developer</small>
                                    </div>
                                </div>
                            </div>
                            <!-- end about-author -->
                        </div>
                        <!-- end col-4 -->
                        <!-- begin col-4 -->
                        <div class="col-md-4 col-sm-12">
                            <h3>Our Experience</h3>
                            <!-- begin skills -->
                            <div class="skills">
                                <div class="skills-name">Front End</div>
                                <div class="progress progress-striped">
                                    <div class="progress-bar progress-bar-success" style="width: 95%">
                                        <span class="progress-number">95%</span>
                                    </div>
                                </div>
                                <div class="skills-name">Programming</div>
                                <div class="progress progress-striped">
                                    <div class="progress-bar progress-bar-success" style="width: 90%">
                                        <span class="progress-number">90%</span>
                                    </div>
                                </div>
                                <div class="skills-name">Database Design</div>
                                <div class="progress progress-striped">
                                    <div class="progress-bar progress-bar-success" style="width: 85%">
                                        <span class="progress-number">85%</span>
                                    </div>
                                </div>
                                <div class="skills-name">Wordpress</div>
                                <div class="progress progress-striped">
                                    <div class="progress-bar progress-bar-success" style="width: 80%">
                                        <span class="progress-number">80%</span>
                                    </div>
                                </div>
                            </div>
                            <!-- end skills -->
                        </div>
                        <!-- end col-4 -->
                    </div>
                    <!-- end row -->
                </div>
                <!-- end container -->
            </div>
            <!-- end #about -->

            <!-- begin #milestone -->
            <div id="milestone" class="content bg-black-darker has-bg" data-scrollview="true">
                <!-- begin content-bg -->
                <div class="content-bg">
                    <img src="Images/Home/home-bg-2.jpeg" alt="Milestone" />
                </div>
                <!-- end content-bg -->
                <!-- begin container -->
                <div class="container">
                    <!-- begin row -->
                    <div class="row">
                        <!-- begin col-3 -->
                        <div class="col-md-3 col-sm-3 milestone-col">
                            <div class="milestone">
                                <div class="number" data-animation="true" data-animation-type="number" data-final-number="<%=AppSettings.TotalMembers %>"></div>
                                <div class="title"><%=DEFAULT.TOTALMEMBERS %></div>
                            </div>
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-md-3 col-sm-3 milestone-col">
                            <div class="milestone">
                                <% if (AppSettings.Site.IsCurrencySignBefore) %>
                                <% { %>
                                <div class="number" data-animation="true" data-prefix="<%=AppSettings.Site.CurrencySign %>" data-animation-type="number" data-final-number="<%=AppSettings.TotalEarned.ToClearString() %>"></div>
                                <% }
                                    else
                                    { %>
                                <div class="number" data-animation="true" data-suffix="<%=AppSettings.Site.CurrencySign %>" data-animation-type="number" data-final-number="<%=AppSettings.TotalEarned.ToClearString() %>"></div>
                                <% } %>
                                <div class="title"><%=DEFAULT.TOTALEARNED %></div>

                            </div>
                        </div>

                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-md-3 col-sm-3 milestone-col">
                            <div class="milestone">
                                <div class="number" data-animation="true" data-animation-type="number" data-final-number="<%=AppSettings.OnlineUsers %>"></div>
                                <div class="title"><%=DEFAULT.MEMBERSONLINE %></div>
                            </div>
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-md-3 col-sm-3 milestone-col">
                            <div class="milestone">
                                <% if (AppSettings.Site.IsCurrencySignBefore) %>
                                <% { %>
                                <div class="number" data-animation="true" data-prefix="<%=AppSettings.Site.CurrencySign %>" data-animation-type="number" data-final-number="<%=AppSettings.TotalCashout.ToClearString() %>"></div>
                                <% }
                                    else
                                    { %>
                                <div class="number" data-animation="true" data-suffix="<%=AppSettings.Site.CurrencySign %>" data-animation-type="number" data-final-number="<%=AppSettings.TotalCashout.ToClearString() %>"></div>
                                <% } %>
                                <div class="title"><%=DEFAULT.TOTALCASHOUT %></div>
                            </div>
                        </div>
                        <!-- end col-3 -->
                    </div>
                    <!-- end row -->
                </div>
                <!-- end container -->
            </div>
            <!-- end #milestone -->

            <!-- begin #team -->
            <div id="team" class="content" data-scrollview="true">
                <!-- begin container -->
                <div class="container">
                    <h2 class="content-title">Our Team</h2>
                    <p class="content-desc">
                        Phasellus suscipit nisi hendrerit metus pharetra dignissim. Nullam nunc ante, viverra quis<br />
                        ex non, porttitor iaculis nisi.
               
                    </p>
                    <!-- begin row -->
                    <div class="row">
                        <!-- begin col-4 -->
                        <div class="col-md-4 col-sm-4">
                            <!-- begin team -->
                            <div class="team">
                                <div class="image" data-animation="true" data-animation-type="flipInX">
                                    <img src="Images/Home/user-1.jpg" alt="Ryan Teller" />
                                </div>
                                <div class="info">
                                    <h3 class="name">Ryan Teller</h3>
                                    <div class="title text-theme">FOUNDER</div>
                                    <p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor.</p>
                                    <div class="social">
                                        <a href="#"><i class="fa fa-facebook fa-lg fa-fw"></i></a>
                                        <a href="#"><i class="fa fa-twitter fa-lg fa-fw"></i></a>
                                        <a href="#"><i class="fa fa-google-plus fa-lg fa-fw"></i></a>
                                    </div>
                                </div>
                            </div>
                            <!-- end team -->
                        </div>
                        <!-- end col-4 -->
                        <!-- begin col-4 -->
                        <div class="col-md-4 col-sm-4">
                            <!-- begin team -->
                            <div class="team">
                                <div class="image" data-animation="true" data-animation-type="flipInX">
                                    <img src="Images/Home/user-2.jpg" alt="Jonny Cash" />
                                </div>
                                <div class="info">
                                    <h3 class="name">Johnny Cash</h3>
                                    <div class="title text-theme">WEB DEVELOPER</div>
                                    <p>Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.</p>
                                    <div class="social">
                                        <a href="#"><i class="fa fa-facebook fa-lg fa-fw"></i></a>
                                        <a href="#"><i class="fa fa-twitter fa-lg fa-fw"></i></a>
                                        <a href="#"><i class="fa fa-google-plus fa-lg fa-fw"></i></a>
                                    </div>
                                </div>
                            </div>
                            <!-- end team -->
                        </div>
                        <!-- end col-4 -->
                        <!-- begin col-4 -->
                        <div class="col-md-4 col-sm-4">
                            <!-- begin team -->
                            <div class="team">
                                <div class="image" data-animation="true" data-animation-type="flipInX">
                                    <img src="Images/Home/user-3.jpg" alt="Mia Donovan" />
                                </div>
                                <div class="info">
                                    <h3 class="name">Mia Donovan</h3>
                                    <div class="title text-theme">WEB DESIGNER</div>
                                    <p>Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. </p>
                                    <div class="social">
                                        <a href="#"><i class="fa fa-facebook fa-lg fa-fw"></i></a>
                                        <a href="#"><i class="fa fa-twitter fa-lg fa-fw"></i></a>
                                        <a href="#"><i class="fa fa-google-plus fa-lg fa-fw"></i></a>
                                    </div>
                                </div>
                            </div>
                            <!-- end team -->
                        </div>
                        <!-- end col-4 -->
                    </div>
                    <!-- end row -->
                </div>
                <!-- end container -->
            </div>
            <!-- end #team -->

            <!-- begin #quote -->
            <div id="quote" class="content bg-black-darker has-bg" data-scrollview="true">
                <!-- begin content-bg -->
                <div class="content-bg">
                    <img src="Images/Home/quote-bg.jpg" alt="Quote" />
                </div>
                <!-- end content-bg -->
                <!-- begin container -->
                <div class="container" data-animation="true" data-animation-type="fadeInLeft">
                    <!-- begin row -->
                    <div class="row">
                        <!-- begin col-12 -->
                        <div class="col-md-12 quote">
                            <i class="fa fa-quote-left"></i>Passion leads to design, design leads to performance,
                        <br />
                            performance leads to <span class="text-theme">success</span>!  
                       
                        <i class="fa fa-quote-right"></i>
                            <small>Sean Themes, Developer Teams in Malaysia</small>
                        </div>
                        <!-- end col-12 -->
                    </div>
                    <!-- end row -->
                </div>
                <!-- end container -->
            </div>
            <!-- end #quote -->

            <!-- beign #service -->
            <div id="service" class="content" data-scrollview="true">



                <!-- begin container -->
                <div class="container">
                    <h2 class="content-title">Our Services</h2>
                    <p class="content-desc">
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum consectetur eros dolor,<br />
                        sed bibendum turpis luctus eget
               
                    </p>
                    <!-- begin row -->
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-line-chart"></i></div>
                                <div class="info">
                                    <h4 class="title">Revenue Sharing</h4>
                                    <p class="desc">Full Revenue Sharing System, easy administration, customizable revenue return speed, process + full statistics. True power of GPT+PTC+TE+REVSHARE connection.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-table"></i></div>
                                <div class="info">
                                    <h4 class="title">All Offer Walls</h4>
                                    <p class="desc">Titan is the first script to introduce global support for all Offer Walls, Automatic postback, Credit as Points/Money/Convert, Advanced logging & Reversal System.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-globe"></i></div>
                                <div class="info">
                                    <h4 class="title">ALL CPA/GPT Networks</h4>
                                    <p class="desc">The most advanced CPA/GPT system: Instant crediting and approval, Submissions control, reversal handlers, Fraud protection. Add offers manually or automatically.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-lock"></i></div>
                                <div class="info">
                                    <h4 class="title">Anti-Cheat System</h4>
                                    <p class="desc">Runs live all the time. Analiyzes member IPs, browser & PC info, referral connections, Anti-Proxy API, etc. 100% automatic.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-video-camera"></i></div>
                                <div class="info">
                                    <h4 class="title">GPT Video</h4>
                                    <p class="desc">Connect with DailyMotion and earn when your member watch videos. Different Points rates per different countries, Restrictions, Anti-Cheat System..</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-refresh"></i></div>
                                <div class="info">
                                    <h4 class="title">Traffic Exchange</h4>
                                    <p class="desc">Change your Titan to Traffic Exchange script and offer massive traffic: Anonymous/Direct traffic types. Subpages support. Autosurf link available, Stats & graphs.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-lock"></i></div>
                                <div class="info">
                                    <h4 class="title">Points locking</h4>
                                    <p class="desc">Lock Points from CPA/GPT offers and Offerwalls that exceed some limit to protect against fraud submissions.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-shopping-basket"></i></div>
                                <div class="info">
                                    <h4 class="title">Gift Card System</h4>
                                    <p class="desc">Add any product with custom title, image & description. Codes can be preapproved & sent automatically via email. Geotargeting, different prices per different countries.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-trophy"></i></div>
                                <div class="info">
                                    <h4 class="title">Famobi games</h4>
                                    <p class="desc">Add Famobi.com games with one click. Earn on game ads & attract your members to stay on your website more time. </p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-search"></i></div>
                                <div class="info">
                                    <h4 class="title">GPT Search</h4>
                                    <p class="desc">Earn by your member searches. Sign a contract with Yahoo with our help. Different Points rates per different countries, Restrictions, Anti-Cheat System.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-envelope-o"></i></div>
                                <div class="info">
                                    <h4 class="title">SMS verification</h4>
                                    <p class="desc">Select your Phone Verification policy: Full, Per Each Payout, Per Each Login, Per Each Registration. ProxStop API supported.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-puzzle-piece"></i></div>
                                <div class="info">
                                    <h4 class="title">SolveMedia & reCaptcha</h4>
                                    <p class="desc">Earn on security captchas. Titan supports SolveMedia, Google Captcha (reCAPTCHA) and Titan Captcha.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-language"></i></div>
                                <div class="info">
                                    <h4 class="title">Multi-language support</h4>
                                    <p class="desc">ALL languages supported. English, Polish, German, Spanish, Russian & Indonesian are preinstalled. Easily translate images & texts.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-download"></i></div>
                                <div class="info">
                                    <h4 class="title">Automatic offer import</h4>
                                    <p class="desc">Automatically download & synchornize offers from CPALead, AdGateMedia and Performa-based networks. </p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-facebook-f"></i></div>
                                <div class="info">
                                    <h4 class="title">Earn with Facebook</h4>
                                    <p class="desc">Advertise fanpages & sell likes. All made automatic, instant crediting. Available filters: age, friends, etc. "Earn with one click" + API integration supported.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-object-group"></i></div>
                                <div class="info">
                                    <h4 class="title">100% OOP</h4>
                                    <p class="desc">Titan is written in 100% with Object-Oriented Programming paradigm. It guarantees good scalability, security and development.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-cog"></i></div>
                                <div class="info">
                                    <h4 class="title">PTC mastered</h4>
                                    <p class="desc">Paid-To-Click made perfect: Geo-Targeting, Custom packages for days/clicks, Inside & outside advertising, Advanced settings & stats.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-list"></i></div>
                                <div class="info">
                                    <h4 class="title">Ads queue</h4>
                                    <p class="desc">Queue ads you watch. Available for Traffic Exchange and AdPacks ads. See title, time and basic details of each ad in the queue.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-users"></i></div>
                                <div class="info">
                                    <h4 class="title">Revenue Groups</h4>
                                    <p class="desc">Setup Automatic (more AdPacks you have, better acceleration) or Custom (invite your friends and earn more) Groups to attract more members.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-pie-chart"></i></div>
                                <div class="info">
                                    <h4 class="title">Pool System</h4>
                                    <p class="desc">Divide your income into various poools and have 100% control over the money you spend. Make your Revenue Sharing System stable and secure forever.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-line-chart"></i></div>
                                <div class="info">
                                    <h4 class="title">Profit acceleration</h4>
                                    <p class="desc">Accelerate your revenue return profit, depending of active AdPacks number or your current group. Full control over the acceleration process.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-money"></i></div>
                                <div class="info">
                                    <h4 class="title">Many payment processors</h4>
                                    <p class="desc">Full IPN and API Integration: PayPal, Payza, Perfect Money, SolidTrustPay & Payeer. Instant order crediting, live balances, 1-click payout 100% automatic.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-comment-o"></i></div>
                                <div class="info">
                                    <h4 class="title">YAF.NET Forum</h4>
                                    <p class="desc">The most powerful ASP.NET forum integrated 100% into Titan. Connected with Titan login system, User statistics, achievements and all information.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-share"></i></div>
                                <div class="info">
                                    <h4 class="title">RefBack</h4>
                                    <p class="desc">Want to get more referrals on your other sites? 1-click participate, List of members in system, Easy to manage RefBack system.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-smile-o"></i></div>
                                <div class="info">
                                    <h4 class="title">MLM upto 10 levels</h4>
                                    <p class="desc">Indirect Referrals system: 1-10 referral levels supported, All earning ways included in the system (PTC, CPA/GPT, Offerwalls, TE, PTSU, etc.)</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-dashboard"></i></div>
                                <div class="info">
                                    <h4 class="title">Advanced Bot system</h4>
                                    <p class="desc">Titan provides custom & innovative solution for Bot System: Totally invisible (bots bound to humans), Easy to configure (Titan BQI), doesn't require CRON setup.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-trophy"></i></div>
                                <div class="info">
                                    <h4 class="title">Achievement System</h4>
                                    <p class="desc">Let your users earn more than just a money: Custom achievements/Trophies, Adjust icons & rewards, Multiple gaining possibilites.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-dollar"></i></div>
                                <div class="info">
                                    <h4 class="title">Multiple Currencies</h4>
                                    <p class="desc">Become localized brand: ALL currencies supported, Switch in 1-click in Admin Panel, Orders, statistics, payouts, etc.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-bitcoin"></i></div>
                                <div class="info">
                                    <h4 class="title">BitCoins (BTC)</h4>
                                    <p class="desc">Cryptocurrencies supported. BitCoin Balance, spend and trasfer BTC in the system. BlockChain & Block.io API automatic support.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-power-off"></i></div>
                                <div class="info">
                                    <h4 class="title">Turn features ON/OFF</h4>
                                    <p class="desc">Customize Titan to fit your needs easily. Enable/disable all Titan features with one-click.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-cubes"></i></div>
                                <div class="info">
                                    <h4 class="title">AdPack Ads</h4>
                                    <p class="desc">Create your own AdPack Ads. Start Pages, Full statistics, administration, unique product name(s), colors, groups, revenue system included. </p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-bookmark"></i></div>
                                <div class="info">
                                    <h4 class="title">Login Ads</h4>
                                    <p class="desc">Force members to view ads after each login. Additional pure profit for the administrator, statistics, geolocation, administration.</p>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4 col-sm-4">
                            <div class="service">
                                <div class="icon bg-theme" data-animation="true" data-animation-type="bounceIn"><i class="fa fa-comments-o"></i></div>
                                <div class="info">
                                    <h4 class="title">Live Chat</h4>
                                    <p class="desc">Tawk Live Chat software fully integrated into the script. Just register on Tawk and take advantage of Live Chat on your website.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- end container -->
            </div>
            <!-- end #about -->

            <!-- beign #action-box -->
            <div id="action-box" class="content has-bg" data-scrollview="true">
                <!-- begin content-bg -->
                <div class="content-bg">
                    <img src="Images/Home/action-bg.jpg" alt="Action" />
                </div>
                <!-- end content-bg -->
                <!-- begin container -->
                <div class="container" data-animation="true" data-animation-type="fadeInRight">
                    <!-- begin row -->
                    <div class="row action-box">
                        <!-- begin col-9 -->
                        <div class="col-md-9 col-sm-9">
                            <div class="icon-large text-theme">
                                <i class="fa fa-dollar"></i>
                            </div>
                            <h3>Payment Proofs</h3>

                            <p class="marquee" style="overflow: hidden;">
                                <asp:Literal runat="server" ID="LatestPayoutsLiteral" />
                            </p>

                        </div>
                        <!-- end col-9 -->
                        <!-- begin col-3 -->
                        <div class="col-md-3 col-sm-3">
                            <a href="/sites/proofs.aspx" class="btn btn-outline btn-block">Proofs</a>
                        </div>
                        <!-- end col-3 -->
                    </div>
                    <!-- end row -->
                </div>
                <!-- end container -->
            </div>
            <!-- end #action-box -->

            <!-- begin #work -->
            <div id="work" class="content" data-scrollview="true">
                <!-- begin container -->
                <div class="container" data-animation="true" data-animation-type="fadeInDown">
                    <h2 class="content-title">Our Partners</h2>
                    <p class="content-desc">
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum consectetur eros dolor,<br />
                        sed bibendum turpis luctus eget
               
                    </p>
                    <!-- begin row -->
                    <div class="row row-space-10">
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="http://paypal.com">
                                        <img class="logo" src="Images/Global/banner3.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="http://perfectmoney.is">
                                        <img class="logo" src="Images/Global/banner4.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="http://payza.com">
                                        <img class="logo" src="Images/Global/banner2.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="https://www.solidtrustpay.com/">
                                        <img class="logo" src="Images/Global/banner1.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="https://payeer.com/">
                                        <img class="logo" src="Images/Global/banner5.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="https://www.okpay.com/">
                                        <img class="logo" src="Images/Global/okpay.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="https://www.neteller.com/">
                                        <img class="logo" src="Images/Global/neteller.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="http://www.advcash.com/">
                                        <img class="logo" src="Images/Global/advcash.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="https://www.coinbase.com/">
                                        <img class="logo" src="Images/Global/coinbase.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="https://www.coinpayments.net/">
                                        <img class="logo" src="Images/Global/coinpayments.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="https://bitcoin.org">
                                        <img class="logo" src="Images/Global/btc.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                        <!-- begin col-3 -->
                        <div class="col-lg-2 col-md-3 col-sm-6">
                            <!-- begin work -->
                            <div class="work">
                                <div class="image text-center">
                                    <a href="https://www.blockchain.com/">
                                        <img class="logo" src="Images/Global/blockchain.png" alt="support" />
                                    </a>
                                </div>
                            </div>
                            <!-- end work -->
                        </div>
                        <!-- end col-3 -->
                    </div>
                    <!-- end row -->
                </div>
                <!-- end container -->
            </div>
            <!-- end #work -->

            <!-- begin #client -->
            <div id="client" class="content has-bg bg-green" data-scrollview="true">
                <!-- begin content-bg -->
                <div class="content-bg">
                    <img src="Images/Home/client-bg.jpg" alt="Client" />
                </div>
                <!-- end content-bg -->
                <!-- begin container -->
                <div class="container" data-animation="true" data-animation-type="fadeInUp" data-fix="height">
                    <h2 class="content-title">Our Client Testimonials</h2>
                    <asp:Literal ID="TestimonialsLiteral" runat="server"></asp:Literal>
                </div>
                <!-- end containter -->
            </div>
            <!-- end #client -->

            <!-- begin #pricing -->
            <div id="pricing" class="content" data-scrollview="true">
                <!-- begin container -->
                <div class="container">
                    <h2 class="content-title">Our Price</h2>
                    <p class="content-desc">
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum consectetur eros dolor,<br />
                        sed bibendum turpis luctus eget
               
                    </p>
                    <!-- begin pricing-table -->
                    <ul class="pricing-table col-4">
                        <li data-animation="true" data-animation-type="fadeInUp">
                            <div class="pricing-container">
                                <h3>Starter</h3>
                                <div class="price">
                                    <div class="price-figure">
                                        <span class="price-number">FREE</span>
                                    </div>
                                </div>
                                <ul class="features">
                                    <li>1GB Storage</li>
                                    <li>2 Clients</li>
                                    <li>5 Active Projects</li>
                                    <li>5 Colors</li>
                                    <li>Free Goodies</li>
                                    <li>24/7 Email support</li>
                                </ul>
                                <div class="footer">
                                    <a href="#" class="btn btn-inverse btn-block">Buy Now</a>
                                </div>
                            </div>
                        </li>
                        <li data-animation="true" data-animation-type="fadeInUp">
                            <div class="pricing-container">
                                <h3>Basic</h3>
                                <div class="price">
                                    <div class="price-figure">
                                        <span class="price-number">$9.99</span>
                                        <span class="price-tenure">per month</span>
                                    </div>
                                </div>
                                <ul class="features">
                                    <li>2GB Storage</li>
                                    <li>5 Clients</li>
                                    <li>10 Active Projects</li>
                                    <li>10 Colors</li>
                                    <li>Free Goodies</li>
                                    <li>24/7 Email support</li>
                                </ul>
                                <div class="footer">
                                    <a href="#" class="btn btn-inverse btn-block">Buy Now</a>
                                </div>
                            </div>
                        </li>
                        <li class="highlight" data-animation="true" data-animation-type="fadeInUp">
                            <div class="pricing-container">
                                <h3>Premium</h3>
                                <div class="price">
                                    <div class="price-figure">
                                        <span class="price-number">$19.99</span>
                                        <span class="price-tenure">per month</span>
                                    </div>
                                </div>
                                <ul class="features">
                                    <li>5GB Storage</li>
                                    <li>10 Clients</li>
                                    <li>20 Active Projects</li>
                                    <li>20 Colors</li>
                                    <li>Free Goodies</li>
                                    <li>24/7 Email support</li>
                                </ul>
                                <div class="footer">
                                    <a href="#" class="btn btn-theme btn-block">Buy Now</a>
                                </div>
                            </div>
                        </li>
                        <li data-animation="true" data-animation-type="fadeInUp">
                            <div class="pricing-container">
                                <h3>Lifetime</h3>
                                <div class="price">
                                    <div class="price-figure">
                                        <span class="price-number">$999</span>
                                    </div>
                                </div>
                                <ul class="features">
                                    <li>Unlimited Storage</li>
                                    <li>Unlimited Clients</li>
                                    <li>Unlimited Projects</li>
                                    <li>Unlimited Colors</li>
                                    <li>Free Goodies</li>
                                    <li>24/7 Email support</li>
                                </ul>
                                <div class="footer">
                                    <a href="#" class="btn btn-inverse btn-block">Buy Now</a>
                                </div>
                            </div>
                        </li>
                    </ul>
                </div>
                <!-- end container -->
            </div>
            <!-- end #pricing -->

            <!-- begin #contact -->
            <div id="contact" class="content bg-silver-lighter" data-scrollview="true">
                <!-- begin container -->
                <div class="container">
                    <h2 class="content-title">Contact Us</h2>
                    <p class="content-desc">
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum consectetur eros dolor,<br />
                        sed bibendum turpis luctus eget
               
                    </p>
                    <!-- begin row -->
                    <div class="row">
                        <!-- begin col-6 -->
                        <div class="col-md-6" data-animation="true" data-animation-type="fadeInLeft">
                            <h3>If you have a project you would like to discuss, get in touch with us.</h3>
                            <p>
                                Morbi interdum mollis sapien. Sed ac risus. Phasellus lacinia, magna a ullamcorper laoreet, lectus arcu pulvinar risus, vitae facilisis libero dolor a purus.
                       
                            </p>
                            <p>
                                <strong>SeanTheme Studio, Inc</strong><br />
                                795 Folsom Ave, Suite 600<br />
                                San Francisco, CA 94107<br />
                                P: (123) 456-7890<br />
                            </p>
                            <p>
                                <span class="phone">+11 (0) 123 456 78</span><br />
                                <a href="mailto:<%=AppSettings.Email.Username %>"><%=AppSettings.Email.Username %></a>
                            </p>
                        </div>
                        <!-- end col-6 -->
                    </div>
                    <!-- end row -->
                </div>
                <!-- end container -->
            </div>
            <!-- end #contact -->

            <!-- begin #footer -->
            <div id="footer" class="footer">
                <div class="container">
                    <p class="m-b-30">
                        <a class="m-5" href="sites/tos.aspx"><%=L1.TERMSOFSERVICE %></a>
                        <a class="m-5" href="sites/privacy.aspx"><%=L1.PRIVACYPOLICY %></a>
                        <a class="m-5" id="PaymentProofsLink" href="sites/proofs.aspx" runat="server"><%=L1.PAYMENTPROOFS %></a>
                        <a class="m-5" id="FooterNewsLink" href="/sites/news.aspx" runat="server"><%=U6002.NEWS %></a>
                    </p>
                    <div class="footer-brand">
                        <img src="<%=AppSettings.Site.LogoImageURL %>" />
                    </div>
                    <p>
                        &copy; Copyright <%=DateTime.Now.Year %> <%=AppSettings.Site.Name %>
                    </p>
                    <titan:TitanFooter runat="server" />
                    <titan:SocialListFooter runat="server" />
                </div>
            </div>
            <!-- end #footer -->

        </div>
        <!-- end #page-container -->
    </form>

    <script src="Scripts/default/assets/plugins/jquery/jquery-1.9.1.min.js"></script>
    <script src="Scripts/default/assets/plugins/jquery/jquery-migrate-1.1.0.min.js"></script>
    <script src="Scripts/default/assets/plugins/bootstrap/js/bootstrap.min.js"></script>
    <script data-pace-options='{"ajax": false}' src="Scripts/assets/js/pace.min.js"></script>
    <script src="Scripts/default/assets/plugins/scrollMonitor/scrollMonitor.js"></script>
    <script src="Scripts/default/assets/plugins/marquee/jquery.marquee.min.js"></script>
    <script src="Scripts/home/assets/js/apps.min.js"></script>

    <script>
        $(document).ready(function () {
            App.init();
            $('.marquee').marquee({
                duration: 7000,
                gap: 50,
                delayBeforeStart: 0,
                direction: 'left',
                duplicated: true
            });
        });
    </script>

    <asp:PlaceHolder ID="TawkChatPlaceHolder" Visible="false" runat="server">

        <!--Start of Tawk.to Script-->
        <script type="text/javascript">
            var Tawk_API = Tawk_API || {}, Tawk_LoadStart = new Date();
            (function () {
                var s1 = document.createElement("script"), s0 = document.getElementsByTagName("script")[0];
                s1.async = true;
                s1.src = 'https://embed.tawk.to/<%=TawkSourceID%>/default';
                s1.charset = 'UTF-8';
                s1.setAttribute('crossorigin', '*');
                s0.parentNode.insertBefore(s1, s0);
            })();
        </script>
        <!--End of Tawk.to Script-->

    </asp:PlaceHolder>
    <titan:CustomFooter runat="server" />
</body>
</html>
