using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public interface ICustomObjectControl
{
    int ObjectID { get; set; }
    void DataBind();
}