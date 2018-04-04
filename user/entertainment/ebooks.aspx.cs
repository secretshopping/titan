using Prem.PTC;
using Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.EBooks;

public partial class user_ebooks : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EBooksEnabled);
            LoadEBooks();
        }
    }

    private void LoadEBooks()
    {
        List<EBook> AvailableEBookList = EBook.GetAllActiveEBooks();

        if (AvailableEBookList.Count == 0)
        {
            NoEBooksPanelWrapper.Visible = true;
            NoEBookspanel.Visible = true;
            NoEBookspanel.Text = U6004.NOEBOOKS;
        }
        else
        {
            try
            {
                foreach (EBook eBook in AvailableEBookList)
                {
                    EBooksLiteral.Controls.Add(GetAdHTML(eBook));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }

    protected UserControl GetAdHTML(EBook eBook)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Misc/EBook.ascx");
        var parsedControl = objControl as IEBookObjectControl;
        parsedControl.Object = eBook;

        parsedControl.DataBind();

        return (UserControl)parsedControl;
    }
}