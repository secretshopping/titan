using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class install_Default : System.Web.UI.Page
{
    public int ForumFilesProperCount = 3703; //3703


    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Server.ScriptTimeout = 600;

        if (!Page.IsPostBack && Request.QueryString["mc"] != null)
            InstallWizard.ActiveStepIndex = 2;
    }


    public string DomainName
    {
        get
        {
            string DomainName = HttpContext.Current.Request.Url.Host;
            return DomainName;
        }
    }

    public static string BaseUrl
    {
        get
        {
            HttpContext context = HttpContext.Current;
            string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
            return baseUrl;
        }
    }

    #region Step 2
    protected void WizardStep2_Activate(object sender, EventArgs e)
    {
        Check3Activate();
    }

    AspNetHostingPermissionLevel GetCurrentTrustLevel()
    {
        foreach (AspNetHostingPermissionLevel trustLevel in
                new AspNetHostingPermissionLevel[] {
            AspNetHostingPermissionLevel.Unrestricted,
            AspNetHostingPermissionLevel.High,
            AspNetHostingPermissionLevel.Medium,
            AspNetHostingPermissionLevel.Low,
            AspNetHostingPermissionLevel.Minimal
                })
        {
            try
            {
                new AspNetHostingPermission(trustLevel).Demand();
            }
            catch (System.Security.SecurityException)
            {
                continue;
            }

            return trustLevel;
        }

        return AspNetHostingPermissionLevel.None;
    }

    #endregion

    #region Step 3
    protected void WizardStep3_Activate(object sender, EventArgs e)
    {
        bool canGoNext = true;

        //ASP.NET version
        try
        {
            ASPVersion.Text = Environment.Version.ToString();
            if (Environment.Version.Major >= 4)
            {
                //OK
                ASPStatus.Text = "OK";
                ASPStatus.Font.Bold = true;
                ASPStatus.CssClass = "okgreen";
            }
            else
            {
                //BAD
                ASPStatus.Text = "BAD";
                ASPStatus.Font.Bold = true;
                ASPStatus.CssClass = "red";

                ASPStatus2.Text = "You need to change your ASP.NET version to >= 4.0";
                ASPStatus2.CssClass = "red small";

                canGoNext = false;
            }
        }
        catch (Exception ex)
        {
            ASPVersion.Text = "???";
            ASPVersion.ForeColor = System.Drawing.Color.Orange;
            ASPStatus.Text = "I'm unable to check it. Do it manually.";
            ASPStatus.CssClass = "orange small";
        }


        if (canGoNext == false)
        {
            //Disable the button
            var button = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton");
            button.Visible = false;
        }
    }


    #endregion

    #region Step 4

    private static IEnumerable<string> SplitSqlStatements(string sqlScript)
    {
        // Split by "GO" statements
        var statements = Regex.Split(
                sqlScript,
                @"^\s*GO\s* ($ | \-\- .*$)",
                RegexOptions.Multiline |
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.IgnoreCase);

        // Remove empties, trim, and return
        return statements
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim(' ', '\r', '\n'));
    }

    protected void InstallWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        bool IsOK = false;

        if (e.CurrentStepIndex == 4)
        {
            //Step 4 - DATABASES

            IsOK = false;
            ErrorMessagePanel.Visible = false;
            SuccMessagePanel.Visible = false;

            web_server.Text = web_server.Text.Replace(":", ",");

            string websiteCS = "Data Source=" + web_server.Text + "; Initial Catalog=" + web_name.Text +
                "; Integrated Security=false; User ID=" + web_username.Text + "; Password=" + web_password.Text + ";";

            string forumCS = "Data Source=" + web_server.Text + "; Initial Catalog=" + forum_name.Text +
                "; Integrated Security=false; User ID=" + forum_username.Text + "; Password=" + forum_password.Text + ";";

            string log = "";
            
            try
            {
                log += "Connecting to Website database...";
                //Connect to SQL locally WEBSITE
                using (SqlConnection sqlConnection = new SqlConnection(websiteCS))
                {
                    sqlConnection.Open();
                    log += "OK.<br/>";

                    log += "Executing Website SQL scripts...";

                    //Check if already done
                    bool done = true;
                    try
                    {
                        SqlCommand command = new SqlCommand("SELECT UpgradeProcessorButtonEnabled FROM ApplicationSettings", sqlConnection);
                        command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        done = false;
                    }

                    if (done)
                        log += "OK (skipped).<br/>";
                    else
                    {
                        //Execute
                        using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/install/sql/website.txt")))
                        {
                            String line = sr.ReadToEnd();
                            var scripts = SplitSqlStatements(line);

                            foreach (var splitScript in scripts)
                            {
                                try
                                {
                                    SqlCommand command = new SqlCommand(splitScript, sqlConnection);
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex) { }
                            }
                        }
                        log += "OK.<br/>";
                    }

                    sqlConnection.Close();
                }

                log += "Connecting to Forum database...";
                //Connect to SQL locally FORUM
                using (SqlConnection sqlConnection = new SqlConnection(forumCS))
                {
                    sqlConnection.Open();
                    log += "OK.<br/>";

                    log += "Executing Forum SQL scripts...";

                    //Check if already done
                    bool done = true;
                    try
                    {
                        SqlCommand command = new SqlCommand("SELECT Name FROM yaf_AccessMask", sqlConnection);
                        command.ExecuteScalar();

                    }
                    catch (Exception ex)
                    {
                        done = false;
                    }

                    if (done)
                        log += "OK (skipped).<br/>";
                    else
                    {
                        //Execute
                        using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/install/sql/forum.txt")))
                        {
                            String line = sr.ReadToEnd();
                            var scripts = SplitSqlStatements(line);

                            foreach (var splitScript in scripts)
                            {
                                try
                                {
                                    SqlCommand command = new SqlCommand(splitScript, sqlConnection);
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex) { }
                            }
                        }
                        log += "OK.<br/>";
                    }

                    sqlConnection.Close();
                }

                //Connect to SQL remotely

                log += "Checking remote connection to Website...";

                using (WebClient client = new WebClient())
                {
                    var response = client.DownloadString("https://usetitan.com/Handlers/cs.ashx?c=" + HttpUtility.UrlEncode(websiteCS));
                    if (response == "OK")
                        log += "OK.<br/>";
                    else
                        throw new Exception(response);
                }

                log += "Checking remote connection to Forum...";

                using (WebClient client = new WebClient())
                {
                    var response = client.DownloadString("https://usetitan.com/Handlers/fcs.ashx?c=" + HttpUtility.UrlEncode(forumCS));
                    if (response == "OK")
                        log += "OK.<br/>";
                    else
                        throw new Exception(response);
                }


                //Modify Web.config
                var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
                section.ConnectionStrings["ClientDbString"].ConnectionString = websiteCS;
                section.ConnectionStrings["yafnet"].ConnectionString = forumCS;
                configuration.Save();

                IsOK = true;

                SuccMessagePanel.Visible = true;
                SuccMessage.Text = log;
            }
            catch (Exception ex)
            {
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = log;

                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }

            if (!IsOK)
                e.Cancel = true;

        }
        if (e.CurrentStepIndex == 5)
        {
            //Step 5 - EMAILS            

            IsOK = false;
            SuccPanel2.Visible = false;
            ErrorPanel2.Visible = false;
            string log = "";

            try
            {
                log += "Sending test email message...";

                AppSettings.Email.Username = settings_Username.Text.Trim();
                AppSettings.Email.Password = settings_Password.Text.Trim();
                AppSettings.Email.Host = settings_Host.Text.Trim();
                AppSettings.Email.Port = Convert.ToInt32(settings_Port.Text.Trim());
                AppSettings.Email.IsSecureMail = settings_Secure.Checked;
                AppSettings.Email.Forward = AppSettings.Email.NoReply = settings_ForwardEmail.Text.Trim();
                AppSettings.Email.Save();

                Mailer.SendEmailToUser("mailservertest@usetitan.com", "Test server", "Test");

                log += "OK.<br/>";
                IsOK = true;

                ForumHelper.UpdateForumDB(ForumHelper.BDProperty.Email, AppSettings.Email.Forward);

                SuccPanel2.Visible = true;
                SuccMess2.Text = log;
            }
            catch (Exception ex)
            {
                SuccPanel2.Visible = true;
                SuccMess2.Text = log;

                ErrorPanel2.Visible = true;
                ErrorMess2.Text = ex.Message;
            }

            if (!IsOK)
                e.Cancel = true;
        }
        if (e.CurrentStepIndex == 6)
        {
            Commission.UpdateCommissionSystem();

            //CRON
            IsOK = false;
            ErrorPanel3.Visible = false;

            bool IsGood = false;
            var cronLogs = TableHelper.SelectAllRows<CronEntry>();

            foreach (var entry in cronLogs)
                if (entry.Date.Date == DateTime.Now.Date)
                    IsGood = true;

            if (IsGood)
            {
                IsOK = true;
                AppSettings.Site.Url = BaseUrl;
                AppSettings.Site.Save();

                ForumHelper.UpdateForumDB(ForumHelper.BDProperty.BaseURL, BaseUrl + "forum");
            }
            else
            {
                ErrorPanel3.Visible = true;
                ErrorMess3.Text = "The CRON file for today did not trigger successfully. Try again.";
            }


            if (!IsOK)
                e.Cancel = true;
        }
        if (e.CurrentStepIndex == 7)
        {
            //Admin password
            IsOK = false;
            ErrorPanel4.Visible = false;

            if (String.IsNullOrWhiteSpace(adminpassword.Text))
            {
                ErrorPanel4.Visible = true;
                ErrorMess4.Text = "The password cannot be blank.";
            }
            else
            {

                Member admin = new Member("admin");
                string newPassword = adminpassword.Text;
                admin.PrimaryPassword = MemberAuthenticationService.ComputeHash(newPassword);
                admin.Save();
                IsOK = true;
            }

            if (!IsOK)
                e.Cancel = true;
        }
    }

    #endregion

    #region Step 5
    protected void WizardStep4_Activate(object sender, EventArgs e)
    {
        settings_ForwardEmail.Text = "contact@" + DomainName.Replace("www.","");
        settings_ForwardEmail.Enabled = false;

        //Disable the refreshbutton
        var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
        refreshbutton.Visible = false;
        //Enable skip button
        var skipButton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("SkipButton");
        skipButton.Visible = true;
    }
    #endregion

    #region Step 7
    protected void WizardStep6_Activate(object sender, EventArgs e)
    {
        Check2Activate();
    }
    #endregion


    protected void WizardStep5_Activate(object sender, EventArgs e)
    {
        Check1Activate();
    }

    private bool FileCreateCheck(string path)
    {
        bool IsOK = false;
        try
        {
            File.Create(Server.MapPath(path + "testfile.txt")).Close();
            File.Delete(Server.MapPath(path + "testfile.txt"));
            IsOK = true;
        }
        catch (Exception ex)
        { }

        return IsOK;
    }

    private bool FileRenameCheck(string path)
    {
        bool IsOK = false;
        try
        {
            File.Move(Server.MapPath(path), Server.MapPath("~/Handlers/test"));
            File.Move(Server.MapPath("~/Handlers/test"), Server.MapPath(path));
            IsOK = true;
        }
        catch (Exception ex)
        { }

        return IsOK;
    }

    private void FileCheckHelper(Label label, bool condition, ref bool canGoNext)
    {
        if (condition)
        {
            label.Text = "OK";
            label.CssClass = "okgreen";
        }
        else
        {
            label.Text = "NO";
            label.CssClass = "red";
            canGoNext = false;
        }
    }

    protected void WizardStep8_Activate(object sender, EventArgs e)
    {
        //Disable the refreshbutton
        var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
        refreshbutton.Visible = false;

        var configuration = WebConfigurationManager.OpenWebConfiguration("~");
        var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
        FinalCS.Text = section.ConnectionStrings["ClientDbString"].ConnectionString;
        FinalFCS.Text = section.ConnectionStrings["yafnet"].ConnectionString;

        //Forum config setup
        var section2 = (AppSettingsSection)configuration.GetSection("appSettings");
        section2.Settings["YAF.ConfigPassword"].Value = HashingManager.GenerateMD5(DateTime.Now + DomainName);
        configuration.Save();
    }

    protected void WizardStep10_Activate(object sender, EventArgs e)
    {
        //Disable the refreshbutton
        var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
        refreshbutton.Visible = false;

        bool hasMachineKey = false;

        using (StreamReader sr = new StreamReader(Server.MapPath("~/Web.config")))
        {
            String line = sr.ReadToEnd();
            if (line.Contains("<machineKey"))
                hasMachineKey = true;
        }

        if (hasMachineKey)
        {
            MKEYHolder.Visible = false;
            var button = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton");
            button.Visible = true;
        }
        else
        {
            MKEYHolder.Visible = true;
            //Disable the button
            var button = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton");
            button.Visible = false;
        }
    }

    protected void RefreshButton_Click(object sender, EventArgs e)
    {
        if (InstallWizard.ActiveStepIndex == 1)
            Check3Activate();

        else if (InstallWizard.ActiveStepIndex == 3)
            Check1Activate();

        else if (InstallWizard.ActiveStepIndex == 6)
            Check2Activate();
    }

    protected void SkipButton_Click(object sender, EventArgs e)
    {
        InstallWizard.ActiveStepIndex = 6;
    }

    protected void Check1Activate()
    {
        //Disable the refreshbutton
        var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
        refreshbutton.Visible = true;

        //Permissions

        bool canGoNext = true;


        //File0
        try
        {
            //Modify Web.config
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
            section.ConnectionStrings["ClientDbString"].ConnectionString = "CHANGE_ME";
            section.ConnectionStrings["yafnet"].ConnectionString = "CHANGE_ME";
            configuration.Save();

            File0Status.Text = "OK";
            File0Status.CssClass = "okgreen";
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            File0Status.Text = "NO";
            File0Status.CssClass = "red";
            canGoNext = false;
        }


        FileCheckHelper(File1Status, FileCreateCheck("~/Images/b_ads/"), ref canGoNext);
        FileCheckHelper(File2Status, FileCreateCheck("~/Logs/"), ref canGoNext);
        FileCheckHelper(File3Status, FileCreateCheck("~/App_Data/"), ref canGoNext);
        FileCheckHelper(File4Status, FileCreateCheck("~/Handlers/CPAGPT/"), ref canGoNext);
        FileCheckHelper(File4Status, FileCreateCheck("~/Handlers/Offerwalls/"), ref canGoNext);
        FileCheckHelper(File5Status, FileRenameCheck("~/Handlers/RENAME_ME_SCHEDULED_TASK.ashx"), ref canGoNext);

        if (canGoNext == false)
        {
            //Disable the button
            var button = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton");
            button.Visible = false;
        }
        else
        {
            //Disable the button
            var button = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton");
            button.Visible = true;
        }
    }

    protected void Check2Activate()
    {
        //Disable the refreshbutton
        var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
        refreshbutton.Visible = true;
        //Disable skip button
        var skipButton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("SkipButton");
        skipButton.Visible = false;

        //CRON

        if (File.Exists(Server.MapPath("~/Handlers/RENAME_ME_SCHEDULED_TASK.ashx")))
        {
            string newlocation = HashingManager.GenerateMD5(DateTime.Now + DomainName + BaseUrl);
            File.Move(Server.MapPath("~/Handlers/RENAME_ME_SCHEDULED_TASK.ashx"), Server.MapPath("~/Handlers/cron" + newlocation + ".ashx"));
        }

        string[] files = System.IO.Directory.GetFiles(Server.MapPath("~/Handlers/"), "*.ashx");

        foreach (var file in files)
        {
            if (Path.GetFileName(file).StartsWith("cron"))
            {
                CRONFile.Text = BaseUrl + "Handlers/" + Path.GetFileName(file);
            }
        }
    }

    protected void Check3Activate()
    {
        //Disable the refreshbutton
        var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
        refreshbutton.Visible = true;

        bool canGoNext = true;

        //ASP.NET version
        try
        {
            ASPVersion.Text = Environment.Version.ToString();
            if (Environment.Version.Major >= 4)
            {
                //OK
                ASPStatus.Text = "OK";
                ASPStatus.Font.Bold = true;
                ASPStatus.CssClass = "okgreen";
            }
            else
            {
                //BAD
                ASPStatus.Text = "BAD";
                ASPStatus.Font.Bold = true;
                ASPStatus.CssClass = "red";

                ASPStatus2.Text = "You need to change your ASP.NET version to >= 4.0";
                ASPStatus2.CssClass = "red small";

                canGoNext = false;
            }
        }
        catch (Exception ex)
        {
            ASPVersion.Text = "???";
            ASPVersion.ForeColor = System.Drawing.Color.Orange;
            ASPStatus.Text = "I'm unable to check it. Do it manually.";
            ASPStatus.CssClass = "orange small";
        }


        //TRUST LEVEL
        var trustLevel = GetCurrentTrustLevel();

        TrustLevel.Text = trustLevel.ToString();

        if (trustLevel == AspNetHostingPermissionLevel.Unrestricted ||
            trustLevel == AspNetHostingPermissionLevel.High)
        {
            //OK
            TrustStatus.Text = "OK";
            TrustStatus.Font.Bold = true;
            TrustStatus.CssClass = "okgreen";
        }
        else if (trustLevel == AspNetHostingPermissionLevel.None)
        {
            TrustLevel.Text = "???";
            TrustLevel.ForeColor = System.Drawing.Color.Orange;
            TrustStatus.Text = "I'm unable to check it. Do it manually.";
            TrustStatus.CssClass = "orange small";
        }
        else
        {
            //BAD
            TrustStatus.Text = "BAD";
            TrustStatus.Font.Bold = true;
            TrustStatus.CssClass = "red";

            TrustStatus2.Text = "You need to have Full or High trust level on the server.";
            TrustStatus2.CssClass = "red small";

            canGoNext = false;
        }


        //FILE CHECKS
        int FilesProperCount = 0;

        using (StreamReader sr = new StreamReader(Server.MapPath("~/install/files_count.txt")))
        {
            String line = sr.ReadToEnd();
            FilesProperCount = Convert.ToInt32(line);
        }

        int AllFilesCount = 0;
        int FilesCount = 0;
        int ForumFilesCount = 0;

        foreach (string file in Directory.EnumerateFiles(
        Server.MapPath("~/"), "*.*", SearchOption.AllDirectories))
        {
            AllFilesCount++;
        }

        foreach (string file in Directory.EnumerateFiles(
        Server.MapPath("~/forum/"), "*.*", SearchOption.AllDirectories))
        {
            ForumFilesCount++;
        }

        FilesCount = AllFilesCount - ForumFilesCount;


        if (FilesCount >= FilesProperCount)
        {
            //All OK
            FilesStatus.Text = "OK";
            FilesStatus.Font.Bold = true;
            FilesStatus.CssClass = "okgreen";

            FilesStatus2.Text = "(" + FilesCount + "/" + FilesProperCount + ")";
        }
        else
        {
            //File not found 

            FilesStatus.Text = "NO";
            FilesStatus.Font.Bold = true;
            FilesStatus.CssClass = "red";
            FilesStatus2.Text = "(" + FilesCount + "/" + FilesProperCount + "). Make sure all files have been uploaded.";

            canGoNext = false;
        }

        if (ForumFilesCount >= ForumFilesProperCount)
        {
            //All OK
            FFilesStatus.Text = "OK";
            FFilesStatus.Font.Bold = true;
            FFilesStatus.CssClass = "okgreen";

            FFilesStatus2.Text = "(" + ForumFilesCount + "/" + ForumFilesProperCount + ")";
        }
        else
        {
            //File not found 

            FFilesStatus.Text = "NO";
            FFilesStatus.Font.Bold = true;
            FFilesStatus.CssClass = "red";
            FFilesStatus2.Text = "(" + ForumFilesCount + "/" + ForumFilesProperCount + "). Make sure all files have been uploaded.";

            canGoNext = false;
        }

        if (canGoNext == false)
        {
            //Disable the button
            var button = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton");
            button.Visible = false;
        }
        else
        {
            var button = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton");
            button.Visible = true;
        }
    }

    protected void WizardStep3_Activate1(object sender, EventArgs e)
    {
        //Disable the refreshbutton
        var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
        refreshbutton.Visible = false;
        //Disable skip button
        var skipButton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("SkipButton");
        skipButton.Visible = false;
    }

    protected void WizardStep7_Activate(object sender, EventArgs e)
    {
        //Disable the refreshbutton
        var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
        refreshbutton.Visible = false;
    }

    protected void WizardStep9_Activate(object sender, EventArgs e)
    {
        try
        {
            //Disable the refreshbutton
            var refreshbutton = (Button)InstallWizard.FindControl("StepNavigationTemplateContainerID").FindControl("RefreshButton");
            refreshbutton.Visible = false;
        }
        catch (Exception ex) { }
    }
}