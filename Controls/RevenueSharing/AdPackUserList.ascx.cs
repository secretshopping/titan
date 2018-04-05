using System;
using System.Collections.Generic;
using System.Web.UI;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;

public partial class Controls_AdPackUserList : System.Web.UI.UserControl
{
    private StringBuilder _sb;
    private int _adPackTypesCount;
    private List<AdPackType> _adPackTypes;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Visible && AppSettings.RevShare.IsRevShareEnabled && !Page.IsPostBack)
        {
            ContainerPlaceHolder.Visible = true;
            NoDataLiteral.Visible = false;

            _adPackTypes = AdPackTypeManager.GetAllActiveTypes();
            _adPackTypesCount = _adPackTypes.Count;

            _sb = new StringBuilder();

            _sb.Append("<table class='table table-striped'><thead><tr>");

            for (int i = 0; i < _adPackTypesCount; i++)
            {
                _sb.Append(string.Format("<th style='color: {0};'>{1}</th>", _adPackTypes[i].Color, _adPackTypes[i].Name));
            }

            _sb.Append("</tr></thead><tbody><tr>");

            _sb.Append(GetTitle(U4200.ALL));

            AddStringToStringBuilder(@"SELECT COUNT (Id) FROM AdPacks WHERE UserId = {0} AND AdPackTypeId = {1}");

            _sb.Append(GetTitle(L1.ACTIVE));

            AddStringToStringBuilder(
                @"SELECT COUNT (Id) FROM AdPacks WHERE UserId = {0} AND AdPackTypeId = {1} AND MoneyReturned < MoneyToReturn");

            _sb.Append(GetTitle(U5003.EXPIRED));

            AddStringToStringBuilder(
                @"SELECT COUNT (Id) FROM AdPacks WHERE UserId = {0} AND AdPackTypeId = {1} AND MoneyReturned >= MoneyToReturn");

            _sb.Append("</tr></tbody></table>");

            TotalAdPacksLiteral.Text = _sb.ToString();

        }
        else
        {
            NoDataLiteral.Text = L1.NODATA;
        }
    }

    private void AddStringToStringBuilder(string countString)
    {
        for (var i = 0; i < _adPackTypes.Count; i++)
        {
            var adPackCount = (int)TableHelper.SelectScalar(string.Format(countString, Member.CurrentId, _adPackTypes[i].Id));

            _sb.Append(string.Format("<td style='color: {0};'>{1}</td>", _adPackTypes[i].Color, adPackCount));
        }
    }

    private string GetTitle( string name)
    {
        return string.Format("</tr><td colspan='{0}' style='font-weight: bold; text-align: center'>{1}</td><tr>",
            _adPackTypesCount, name);
    }
}