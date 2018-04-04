using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.EBooks;

public partial class Controls_Misc_EBook : System.Web.UI.UserControl, IEBookObjectControl
{
    public EBook Object { get; set; }

    public string ImageUrl
    {
        get { return Object.ImgUrl; }
    }

    public string EBookUrl
    {
        get { return Object.EBookUrl; }
    }

    public string Title
    {
        get { return Object.Title; }
    }

    public string Description
    {
        get { return Object.Description; }
    }
}