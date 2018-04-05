using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Resources;

public partial class Controls_Countries : System.Web.UI.UserControl
{
    public List<Representative> Representatives { get; set; }
    public string Country { get; set; }
    public int Id { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Country = Representatives[0].Country;
            var countryCode = CountryManager.GetCountryCode(Country);
            flagImage.ImageUrl = string.Format("~/Images/Flags/{0}.png", countryCode.ToLower());

            CountryName.Text = Country;

            int noOfDisplayedCRepresentatives = 0;

            int noOfRepresentatives = AppSettings.Representatives.NoOfRepresentatives;
            Id = Representatives[0].Id;
            
            for (int i = 0; i < Representatives.Count && i < noOfRepresentatives; i++)
            {
                noOfDisplayedCRepresentatives++;

                UserControl representativeControl = (UserControl)Page.LoadControl("~/Controls/Representatives/Representative.ascx");

                PropertyInfo ctrl = representativeControl.GetType().GetProperty("Representative");
                ctrl.SetValue(representativeControl, Representatives[i], null);

                representativeControl.DataBind();
                RepresentativesPlaceHolder.Controls.Add(representativeControl);
            }

            RepresentativeCount.InnerText = noOfDisplayedCRepresentatives.ToString() + " " + U6002.REPRESENTATIVES;
        }
    }       
}