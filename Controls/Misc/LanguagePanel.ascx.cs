using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using System.Globalization;

public partial class Controls_LanguagePanel : System.Web.UI.UserControl
{
    string[] languages;
    public int Height
    {
        get
        {
            return Convert.ToInt32(20.8 * languages.Count<string>());
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        languages = AppSettings.Site.ChoosedLanguages.Split('#');

        LangPanel.Controls.Add(GetButton(System.Threading.Thread.CurrentThread.CurrentCulture.Name));

        for (int i = 0; i < languages.Length; i++)        
            if (languages[i] != System.Threading.Thread.CurrentThread.CurrentCulture.Name)            
                LangPanel.Controls.Add(GetButton(languages[i]));        

        if (!Page.IsPostBack)
        {
            if (AppSettings.Site.IsMulticurrencyPricingEnabled)
            {
                MulticurrencyPlaceHolder.Visible = true;
                CurrencyDropDownList.Items.Clear();
                CurrencyDropDownList.Items.AddRange(MulticurrencyHelper.AllCurrenciesListItem);
                CurrencyDropDownList.SelectedValue = MulticurrencyHelper.GetRegionInfo().Name;
            }
        }
    }

    protected void CurrencyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        RegionInfo regionInfo = new RegionInfo(CurrencyDropDownList.SelectedValue);
        MulticurrencyHelper.SetCurrency(regionInfo.Name);
    }

    protected void changelang_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "changelang")
        {
            string selectedLanguage = e.CommandArgument.ToString();
            try
            {
                CultureInfo.CreateSpecificCulture(selectedLanguage);
                HttpCookie cookie = new HttpCookie("CultureInfo")
                {
                    Value = selectedLanguage
                };
                Response.Cookies.Add(cookie);

                Image img = (Image)sender;

                Response.Redirect(Request.Url.ToString());
            }
            catch (CultureNotFoundException)
            {
                throw new Exception("ERROR: Invalid culture string");
            }
        }
    }

    protected ImageButton GetButton(string cultureCode)
    {
        ImageButton button = new ImageButton
        {
            ImageUrl = "~/Images/Flags/" + cultureCode.Substring(3).ToLower() + ".png",
            CommandArgument = cultureCode,
            CommandName = "changelang",
            AlternateText = cultureCode
        };
        button.Command += changelang_Command;

        return button;
    }
}
