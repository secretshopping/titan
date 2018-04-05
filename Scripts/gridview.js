function hideIfUnchecked(selectedPanelId) {

    var inputs = document.getElementsByTagName("input"); //or document.forms[0].elements;  
    var cbs = []; //will contain all checkboxes  
    var checked = []; //will contain all checked checkboxes  
    var rows = $("table.gridtable tr");

    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].type == "checkbox" && inputs[i].name.indexOf("chkSelect") !== -1) {
            cbs.push(inputs[i]);
            if (inputs[i].checked && inputs[i].parentElement.parentElement.style.display != "none") {
                checked.push(inputs[i]);
            }
        }
    }

    var nbCbs = cbs.length; //number of checkboxes  
    var nbChecked = checked.length; //number of checked checkboxes  
    if (nbChecked == 0)
        document.getElementById(selectedPanelId).style.display = 'none';
    else
        document.getElementById(selectedPanelId).style.display = 'block';
}


function sliderChanged(value) {
    //1. Get the values
    var value1;
    value1 = parseFloat(value);
    var val2 = parseFloat(value.substring(value.indexOf(";") + 1));

    //2. foreach all rows
    var cells = $("table.gridtable tr td.AVG");

    for (var i = 0; i < cells.length; i++) {
        var val3 = parseFloat(cells[i].innerHTML);

        if (val3 >= value1 && val3 <= val2)
            cells[i].parentElement.style.display = "";
        else
            cells[i].parentElement.style.display = "none";
    }

    //3. Uncheck invisible
    uncheckInvisible();

}

function uncheckInvisible()
{

    var inputs = document.getElementsByTagName("input"); //or document.forms[0].elements;  

    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].type == "checkbox" && inputs[i].name.indexOf("chkSelect") !== -1) {
            if (inputs[i].checked && inputs[i].parentElement.parentElement.style.display == "none") {
                inputs[i].checked = false;
            }
        }
    }
}