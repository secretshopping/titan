using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// This class simplifies adding Language resources to various controls
/// </summary>
public static class LangAdder
{

    /// <summary>
    /// Adds .ErrorMessage text to TargetValidator validator
    /// (supports all validators)
    /// </summary>
    public static void Add(BaseValidator TargetValidator, string Text, bool overrideText = false)
    {
        TargetValidator.ErrorMessage = Text;

        if (overrideText)
            TargetValidator.Text = Text;
    }

    /// <summary>
    /// Adds .Text text to TargetButton
    /// </summary>
    public static void Add(Button TargetButton, string Text)
    {
        TargetButton.Text = Text;
    }

    /// <summary>
    /// Adds .Value text to ListItem
    /// </summary>
    public static void Add(ListItem TargetItem, string Text)
    {
        TargetItem.Value = Text;
    }

    /// <summary>
    /// Adds .Text text to Label
    /// </summary>
    public static void Add(Label TargetItem, string Text)
    {
        TargetItem.Text = Text;
    }

    /// <summary>
    /// Adds .Text text to checkbox
    /// </summary>
    public static void Add(CheckBox TargetItem, string Text)
    {
        TargetItem.Text = Text;
    }

    public static void Add(Literal TargetItem, string Text)
    {
        TargetItem.Text = Text;
    }

    /// <summary>
    /// Add translations to first UpgradeGridView column
    /// </summary>
    /// <param name="rowindex"></param>
    /// <param name="text"></param>
    public static void AddUpgrade(GridViewRow row, int rowindex, string text)
    {
        if (row.RowIndex == rowindex)        {
            row.Cells[0].Text = text;
        }
    }
}