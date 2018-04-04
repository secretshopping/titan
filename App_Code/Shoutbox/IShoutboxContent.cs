using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan
{
    public interface IShoutboxContent
    {
        string Username { get; set; }
        DateTime SentDate { get; set; }
        string Message { get; set; }

        bool IsEvent { get; }
    }
}