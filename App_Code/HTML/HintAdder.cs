using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

    /// <summary>
    /// Class is used to add hints (over TextBoxes or Labels) automatically
    /// Adding hints = JUST adding 'title' attribute to control
    /// Rest of teh work is done by Tipsy plugin (Plgins/Tipsy) via jQuery/CSS
    /// </summary>
    public static class HintAdder
    {
        /// <summary>
        /// Adds 'title' attribute (hint to parse) to TextBox
        /// </summary>
        public static void Add(TextBox TargetControl, string Hint)
        {
            TargetControl.Attributes.Add("title", Hint);
        }

        /// <summary>
        /// Adds 'title' attribute (hint to parse) to Label
        /// </summary>
        public static void Add(Label TargetControl, string Hint)
        {
            TargetControl.Attributes.Add("title", Hint);
        }


        /// <summary>
        /// Finds TextBox by TargetControlId in ControlToSearch control e.g. RegisterUserWizardStep.ContentTemplateContainer
        /// Next adds 'title' attribute (hint to parse) to this TextBox
        /// TRUE = OK, FALSE = Control Not Found
        /// </summary>
        public static bool FindAndAdd(Control ControlToSearch, string TargetControlId, string Hint)
        {
            TextBox TargetTextBox;
            try
            {
                TargetTextBox = (TextBox)ControlToSearch.FindControl(TargetControlId);
            }
            catch (Exception ex)
            {
                return false;
            }
            Add(TargetTextBox, Hint);
            return true;
        }
    }
