using System;
using System.Web.UI.WebControls;

public class PageScriptGenerator
{
    public static string GetGridViewCode(GridView control)
    {

        if (control.Rows.Count > 0)
        {
            var PaginationScript = String.Format(@"
                var paginationList = $('.table').find('.pgr');
                if (paginationList.length) {{
                    paginationList.each(function () {{
                        var paginationTable = $(this).find('table');
                        var paginationParent = paginationTable.closest('.table');
                        var paginationParentId = paginationParent.attr('id');  
                        paginationParent.parent().append('<div data-parent =\'' + paginationParentId + '\' class=\'pagination\'>' + paginationTable.html() + '</div>');                                                                                           
                        $(this).remove();
                    }});
                }}
            ");

            return String.Format(@"
            {0}
 
            $('#{1}').DataTable({{
                responsive: true,
                paginate: false,
                info: false,
                searching: false,
                ordering: false, 
                retrieve: true
            }});", PaginationScript, control.ClientID);
        }
            

        return String.Empty;

    }
}