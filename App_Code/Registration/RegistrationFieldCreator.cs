using System.Web.UI.WebControls;
using Titan;
using Prem.PTC.Members;
using Prem.PTC.Utils.NVP;
using Prem.PTC;
using Titan.Registration;

public class RegistrationFieldCreator
{
    private const string VALIDATION_GROUP = "RegisterUserValidationGroup";
    private const string ERROR_COLOR = "#b70d00";
    private const string TEXTBOX_CSS_CLASS = "form-control";
    private const string CHECKBOX_CSS_CLASS = "pull-left";

    private const string TEXTBOX_BACKGROUND_COLOR = "#f8f8f7";

    private const string VALIADTION_REGEX = "[^';\"]{0,300}";
    private const string customFieldPrefix = "CustomField";
    public static Panel Generate(bool isFromSurvey = false)
    {
        var Fields = TableHelper.SelectRows<CustomRegistrationField>(TableHelper.MakeDictionary("IsHidden", false));
        Panel Container = new Panel();

        if (!AppSettings.Authentication.CustomFieldsAsSurvey || isFromSurvey)
        {
            foreach (var Field in Fields)
            {
                Literal Wrap1 = new Literal();
                Wrap1.Text = "<div class=\"form-group\">";
                Container.Controls.Add(Wrap1);
                Literal L1 = new Literal();
                L1.Text = "<label class=\"control-label\">" + Field.Label + "</label>";
                
                if (Field.Type == RegistrationFieldType.TextBox)
                {
                    Container.Controls.Add(L1);
                    TextBox TB = new TextBox();
                    TB.ID = customFieldPrefix + Field.StringID;
                    TB.CssClass = TEXTBOX_CSS_CLASS;
                    TB.Attributes.Add("placeholder", Field.Label);
                    TB.Text = "";



                    Container.Controls.Add(TB);

                    RegularExpressionValidator RegX = new RegularExpressionValidator();
                    RegX.ValidationExpression = VALIADTION_REGEX;
                    RegX.ErrorMessage = Resources.L1.DETAILEDNOSPECIAL + " " + Field.Label;
                    RegX.ControlToValidate = customFieldPrefix + Field.StringID;
                    RegX.ForeColor = System.Drawing.ColorTranslator.FromHtml(ERROR_COLOR);
                    RegX.ValidationGroup = VALIDATION_GROUP;
                    RegX.Display = ValidatorDisplay.Dynamic;
                    RegX.Text = "*";

                    Container.Controls.Add(RegX);

                    if (Field.IsRequired)
                    {
                        RequiredFieldValidator ReqX = new RequiredFieldValidator();
                        ReqX.ErrorMessage = Field.Label + " " + Resources.U3900.FIELDISREQUIRED;
                        ReqX.ControlToValidate = customFieldPrefix + Field.StringID;
                        ReqX.ForeColor = System.Drawing.ColorTranslator.FromHtml(ERROR_COLOR);
                        ReqX.ValidationGroup = VALIDATION_GROUP;
                        RegX.Display = ValidatorDisplay.Dynamic;
                        ReqX.Text = "*";

                        Container.Controls.Add(ReqX);
                    }

                    
                }
                else if (Field.Type == RegistrationFieldType.CheckBox)
                {
                    Literal Wrap3 = new Literal();
                    Wrap3.Text = "<div class=\"checkbox\">";
                    Container.Controls.Add(Wrap3);

                    L1.Text = "<label class=\"control-label\">" + Field.Label;
                    Container.Controls.Add(L1);
                    CheckBox CB = new CheckBox();
                    CB.ID = customFieldPrefix + Field.StringID;
                    CB.CssClass = CHECKBOX_CSS_CLASS;
                    Container.Controls.Add(CB);   
                    Literal Wrap4 = new Literal();
                    Wrap4.Text = "</label></div>";
                    Container.Controls.Add(Wrap4);
                }

                Literal Wrap2 = new Literal();
                Wrap2.Text = "</div>";
                Container.Controls.Add(Wrap2);
            }

            
        }
        return Container;
    }

    public static void Save(Member User, Panel page, bool isFromSurvey = false)
    {
        if (!AppSettings.Authentication.CustomFieldsAsSurvey || isFromSurvey)
        {
            var Fields = TableHelper.SelectRows<Titan.CustomRegistrationField>(TableHelper.MakeDictionary("IsHidden", false));
            NotNullNameValuePairs nvp = new NotNullNameValuePairs();

            foreach (var Field in Fields)
            {

                if (Field.Type == RegistrationFieldType.TextBox)
                    nvp.Add(Field.StringID, ((TextBox)page.FindControl(customFieldPrefix + Field.StringID)).Text);
                else if (Field.Type == RegistrationFieldType.CheckBox)
                    nvp.Add(Field.StringID, ((CheckBox)page.FindControl(customFieldPrefix + Field.StringID)).Checked.ToString());
            }
            User.Custom = nvp;
        }
    }

}