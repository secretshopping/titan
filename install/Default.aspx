<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="install_Default" ValidateRequest="false" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TITAN Installer</title>
    <link rel="stylesheet" href="css/style.css?eq=1" type="text/css" />
    <link rel="shortcut icon" type="image/ico" href="images/favicon.png" />
   
    <script type="text/javascript">
        function pageLoad() {
            // $('input[type=submit]').prop('disabled', 'enabled').prop('class', '');
        }
    </script>

</head>
<body style="background: url(images/background.jpeg);">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="mainWindow">

            <asp:Wizard ID="InstallWizard" runat="server" EnableTheming="true" CssClass="wizard" OnNextButtonClick="InstallWizard_NextButtonClick"
                NavigationStyle-CssClass="navigation" SideBarStyle-CssClass="sidebar" StepStyle-CssClass="step" DisplaySideBar="false"
                FinishCompleteButtonStyle-CssClass="displaynone">
                <StepNavigationTemplate>
                    <asp:Button ID="SkipButton" runat="server" CssClass="rbutton" Style="margin-right: 20px" Text="Do this later" OnClick="SkipButton_Click" Visible="false" />
                    <asp:Button ID="RefreshButton" runat="server" CssClass="rbutton" Style="margin-right: 20px" Text="Refresh" OnClick="RefreshButton_Click" />
                    <asp:Button ID="StepPreviousButton" runat="server" CommandName="MovePrevious" Text="< Previous Step" />
                    <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Next Step >" />
                </StepNavigationTemplate>
                <FinishNavigationTemplate>
                </FinishNavigationTemplate>
                <WizardSteps>
                    <asp:WizardStep ID="WizardStep1" runat="server" Title="Start">
                        <h2>Start</h2>
                        <br />
                        We are going to install TITAN on this server together. Please take your time to complete every step. It's very important
                        to read instructions carefully. The installation process should take about 30 minutes.
                        
                        <div style="margin-top: 20px; text-align: center;">
                            <img src="images/robot.png" />
                        </div>

                    </asp:WizardStep>


                    <asp:WizardStep ID="WizardStep2" runat="server" Title="Requirements & files" OnActivate="WizardStep2_Activate">
                        <h2>Requirements</h2>
                        <br />
                        Let's check if your server meet the requirements:
                        <br />
                        <br />
                        <ul class="reqlist">
                            <li><b>ASP.NET version:</b>
                                <asp:Label ID="ASPVersion" runat="server"></asp:Label>
                                <asp:Label ID="ASPStatus" runat="server"></asp:Label>
                                <asp:Label ID="ASPStatus2" runat="server"></asp:Label></li>
                            <li><b>Trust Level:</b>
                                <asp:Label ID="TrustLevel" runat="server"></asp:Label>
                                <asp:Label ID="TrustStatus" runat="server"></asp:Label>
                                <asp:Label ID="TrustStatus2" runat="server"></asp:Label></li>
                        </ul>
                        <br />
                        <br />
                        <h2>Files</h2>
                        <br />
                        Did you upload ALL files to the server?
                                                <br />
                        <br />
                        <ul class="reqlist">
                            <li><b>Titan files: </b>
                                <asp:Label ID="FilesStatus" runat="server"></asp:Label>
                                <asp:Label ID="FilesStatus2" runat="server"></asp:Label>
                            </li>
                            <li><b>YAF.NET Forum files: </b>
                                <asp:Label ID="FFilesStatus" runat="server"></asp:Label>
                                <asp:Label ID="FFilesStatus2" runat="server"></asp:Label>
                            </li>
                        </ul>
                        <br />
                    </asp:WizardStep>

                    <asp:WizardStep ID="WizardStep10" runat="server" Title="Machine Key" OnActivate="WizardStep10_Activate">


                        <h2>Machine Key</h2>
                        <br />

                        Go to <a href="http://www.developerfusion.com/tools/generatemachinekey/">http://www.developerfusion.com/tools/generatemachinekey/</a> and click "Generate key". 

                        <br />
                        <br />
                        Next, copy "ASP.NET <b>2.0</b> Machine Key" and open ~/Web.config file, which is located on your hosting account. Find &lt;!--MACHINE KEY--&gt; and 
                           replace it with the "ASP.NET <b>2.0</b> Machine Key" generated above. Save the file and click the button below.
                        <br />
                        <br />
                        <br />
                        <asp:PlaceHolder ID="MKEYHolder" runat="server">
                            <a href="<%=BaseUrl %>install/Default.aspx?mc=1" class="rbutton" style="color: white">&nbsp;&nbsp;I completed this step&nbsp;&nbsp;</a>
                        </asp:PlaceHolder>
                        <br />
                        <br />
                    </asp:WizardStep>


                    <asp:WizardStep ID="WizardStep5" runat="server" Title="Permissions" OnActivate="WizardStep5_Activate">


                        <h2>Permissions</h2>
                        <br />
                        You need to grant "Full Control" (read, write, modify = 777) permissions 
                           for the application to the following folders/files/subfolders:
                        <br />
                        <br />
                        <ul class="reqlist">
                            <li><%=BaseUrl %><b></b> (main directory)
                                <asp:Label ID="File0Status" runat="server" Font-Bold="true"></asp:Label></li>
                            <li><%=BaseUrl %><b>Images/b_ads/</b>
                                <asp:Label ID="File1Status" runat="server" Font-Bold="true"></asp:Label></li>
                            <li><%=BaseUrl %><b>Logs/</b>  and subfolders
                                <asp:Label ID="File2Status" runat="server" Font-Bold="true"></asp:Label></li>
                            <li><%=BaseUrl %><b>App_Data/</b>  and subfolders
                                <asp:Label ID="File3Status" runat="server" Font-Bold="true"></asp:Label></li>
                            <li><%=BaseUrl %><b>Handlers/</b> and subfolders
                                <asp:Label ID="File4Status" runat="server" Font-Bold="true"></asp:Label></li>
                            <li><%=BaseUrl %><b>Handlers/RENAME_ME_SCHEDULED_TASK.ashx</b>
                                <asp:Label ID="File5Status" runat="server" Font-Bold="true"></asp:Label></li>
                        </ul>
                        <br />
                        <br />
                    </asp:WizardStep>


                    <asp:WizardStep ID="WizardStep3" runat="server" Title="Databases" OnActivate="WizardStep3_Activate1">
                        <h2>Databases</h2>

                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="MultiViewUpdatePanel">
                            <ProgressTemplate>
                                <div class="blackRefresh" id="blackOverlayLoading">
                                    <img class="refreshBlack" alt="" src="images/loading.gif" />
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:UpdatePanel ID="MultiViewUpdatePanel" runat="server">

                            <ContentTemplate>

                                <asp:Panel ID="SuccMessagePanel" runat="server" Visible="false" CssClass="greenbox">
                                    <asp:Literal ID="SuccMessage" runat="server"></asp:Literal>
                                </asp:Panel>

                                <br />
                                <asp:Panel ID="ErrorMessagePanel" runat="server" Visible="false" CssClass="ui-state-error" ForeColor="White">
                                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                                </asp:Panel>
                                <br />


                                Now let's connect to the data.
                        <br />
                                <br />
                                Go to your HOSTING Control Panel and create <b>2</b> Microsoft SQL (MSSQL)* databases. <b>1</b> for the website, and <b>1</b> for the forum. 
                        <br />

                                Afterwards, fill in the details below. 
                        <br />
                                <br />
                                <br />
                                <center> Database server address:
                            
                                    <asp:TextBox ID="web_server" runat="server" CssClass="rtextbox" Width="220px"></asp:TextBox>
                          </center>
                                <br />
                                <br />

                                <div style="width: 45%; display: inline; float: left;">
                                    <h3>Website</h3>
                                    <table style="">
                                        <tr>
                                            <td>Database name:&nbsp;&nbsp;&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="web_name" runat="server" CssClass="rtextbox"></asp:TextBox>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Database user:</td>
                                            <td>
                                                <asp:TextBox ID="web_username" runat="server" CssClass="rtextbox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>Password:</td>
                                            <td>
                                                <asp:TextBox ID="web_password" runat="server" CssClass="rtextbox" TextMode="Password"></asp:TextBox></td>
                                        </tr>

                                    </table>
                                </div>
                                <div style="width: 45%; display: inline; float: left;">
                                    <h3>Forum</h3>
                                    <table style="">
                                        <tr>
                                            <td>Database name:&nbsp;&nbsp;&nbsp;</td>
                                            <td>
                                                <asp:TextBox ID="forum_name" runat="server" CssClass="rtextbox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>Database user:</td>
                                            <td>
                                                <asp:TextBox ID="forum_username" runat="server" CssClass="rtextbox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>Password:</td>
                                            <td>
                                                <asp:TextBox ID="forum_password" runat="server" CssClass="rtextbox" TextMode="Password"></asp:TextBox></td>
                                        </tr>

                                    </table>
                                </div>

                                <div style="clear: both;"></div>
                                <br />
                                <br />
                                <span style="font-size: smaller">*MSSQL versions supported: 2012, 2014 & 2016</span>

                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </asp:WizardStep>


                    <asp:WizardStep ID="WizardStep4" runat="server" Title="Email" OnActivate="WizardStep4_Activate">
                        <h2>Email</h2>
                        <br />
                        <asp:Panel ID="SuccPanel2" runat="server" Visible="false" CssClass="greenbox">
                            <asp:Literal ID="SuccMess2" runat="server"></asp:Literal>
                        </asp:Panel>

                        <br />
                        <asp:Panel ID="ErrorPanel2" runat="server" Visible="false" CssClass="ui-state-error" ForeColor="White">
                            <asp:Literal ID="ErrorMess2" runat="server"></asp:Literal>
                        </asp:Panel>
                        <br />
                        Now let's connect to your email server. Go to your HOSTING Control Panel and create one email account: 
                        <br />

                        <asp:Label ID="settings_ForwardEmail" Font-Size="Larger" Font-Bold="true" runat="server" />
                        <br />
                        <br />

                        Next, type in your SMTP (outgoing email) settings below:
                        <br />
                        <br />
                        <table class="vtable">
                            <tr>
                                <td>SMTP host:</td>
                                <td>
                                    <asp:TextBox ID="settings_Host" CssClass="rtextbox"
                                        ValidationGroup="settings_Group" Width="120" runat="server" />

                                </td>
                            </tr>
                            <tr>
                                <td>SMTP username:</td>
                                <td>
                                    <asp:TextBox ID="settings_Username" CssClass="rtextbox"
                                        ValidationGroup="settings_Group" Width="120" runat="server" />

                                </td>
                            </tr>
                            <tr>
                                <td>SMTP password:</td>
                                <td>
                                    <asp:TextBox ID="settings_Password" CssClass="rtextbox" TextMode="Password"
                                        ValidationGroup="settings_Group" Width="120" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                <td>SMTP port:</td>
                                <td>
                                    <asp:TextBox ID="settings_Port" MaxLength="5" Width="40px"
                                        ValidationGroup="settings_Group" runat="server" CssClass="rtextbox" />
                                    (default: 25, GoDaddy: 3535)
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>

                                    <asp:CheckBox ID="settings_Secure" runat="server" />
                                    SSL/TLS


                                </td>
                            </tr>
                        </table>
                        <br />
                        <br />
                    </asp:WizardStep>





                    <asp:WizardStep ID="WizardStep6" runat="server" Title="CRON" OnActivate="WizardStep6_Activate">
                        <h2>CRON</h2>
                        <br />
                        <asp:Panel ID="ErrorPanel3" runat="server" Visible="false" CssClass="ui-state-error" ForeColor="White">
                            <asp:Literal ID="ErrorMess3" runat="server"></asp:Literal>
                        </asp:Panel>
                        <br />
                        You need to run a particular URL every 24h at <b>23:30 server time</b>. It's necessary for Titan to work. This file
                        handles all daily jobs. Titan won't work properly without this file trigerred.

                        <br />
                        <br />
                        <br />

                        URL to run every 24h: 
                         <asp:Label ID="CRONFile" Font-Size="Larger" Font-Bold="true" runat="server" />

                        <br />
                        <br />
                        <br />
                        Run your CRON job/Scheduled Task now. If it's setup preperly, it should trigger URL above. After it's successfully trigerred,
                        go to the next step. 
                        <br />
                        <br />
                        <br />
                    </asp:WizardStep>





                    <asp:WizardStep ID="WizardStep7" runat="server" Title="Password" OnActivate="WizardStep7_Activate">

                        <h2>Admin password</h2>
                        <br />
                        <asp:Panel ID="ErrorPanel4" runat="server" Visible="false" CssClass="ui-state-error" ForeColor="White">
                            <asp:Literal ID="ErrorMess4" runat="server"></asp:Literal>
                        </asp:Panel>
                        <br />
                        You access your Titan Admin Panel (located on <a href="https://admin.usetitan.com">https://admin.usetitan.com</a>) using your UseTitan login details. 
                        <br />
                        However you also need
                        'admin' account on your website. It can be used e.g. for forum administration or other activities on your website.
                        <br />
                        <br />
                        Your login details are as follows:
                        <br />
                        <table style="border-spacing: 10px; border-collapse: separate;">
                            <tr>
                                <td>Username:</td>
                                <td>
                                    <b>admin</b>

                                </td>
                            </tr>
                            <tr>
                                <td>Password:</td>
                                <td>
                                    <asp:TextBox ID="adminpassword" CssClass="rtextbox" TextMode="Password"
                                        Width="120" runat="server" />

                                </td>
                            </tr>
                            <tr>
                                <td>PIN:</td>
                                <td>
                                    <b>1234</b>
                                </td>
                            </tr>
                        </table>

                        <br />
                        <br />

                    </asp:WizardStep>


                    <asp:WizardStep ID="WizardStep8" runat="server" Title="Add the license" OnActivate="WizardStep8_Activate">
                        <h2>Add the license</h2>
                        <br />
                        The installation is almost finished. Now you need to add your domain to your license plan in UseTitan system.<br />
                        <br />
                        Go to Your UseTitan DashBoard (<a href="https://usetitan.com/account.aspx">https://usetitan.com/account.aspx</a>) and click
                        "Add new domain".

                         The correct setup details are:
                        <br />
                        <br />
                        <table style="border-spacing: 10px; border-collapse: separate;">
                            <tr>
                                <td>Domain name:</td>
                                <td>http://<b><%=DomainName %></b>

                                </td>
                            </tr>
                            <tr>
                                <td>Website Connection String:</td>
                                <td>
                                    <asp:TextBox ID="FinalCS" runat="server" Width="500px" Font-Size="Smaller"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Forum Connection String:</td>
                                <td>
                                    <asp:TextBox ID="FinalFCS" runat="server" Width="500px" Font-Size="Smaller"></asp:TextBox>
                                </td>
                            </tr>
                        </table>

                        <br />
                        After it's done, go to the next step. 
                           <br />
                        <br />
                    </asp:WizardStep>


                    <asp:WizardStep ID="WizardStep9" runat="server" Title="Finished!" OnActivate="WizardStep9_Activate">
                        <h2>Finished!</h2>
                        <br />
                        Congratulations. Titan has been successfully installed on your hosting account.
     
                        You can now log into your Admin Panel located on <a href="https://admin.usetitan.com">https://admin.usetitan.com</a>. 

                        <br />
                        <br />
                        <div style="color: red">
                            <h2>Important notice</h2>
                            Make sure to delete <%=BaseUrl %>install/ folder from your server <b>NOW</b>. It's important for security.
                        <br />
                        </div>
                        <br />
                        <br />
                        <a href="<%=BaseUrl %>" class="rbutton" style="color: white">Go now to <%=DomainName %></a>
                        <br />
                        <br />
                    </asp:WizardStep>


                </WizardSteps>

            </asp:Wizard>

        </div>
        <div class="footer">
            &copy; UseTitan 2013-<%=DateTime.Now.Year %>. All rights reserved.<br />
            Need help? Check <a href="http://wiki.usetitan.com/">TITAN Wiki</a>
        </div>
    </form>
</body>
</html>
