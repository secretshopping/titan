using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Titan.API;
using Titan;
using Prem.PTC.Members;
using Prem.PTC;
using Newtonsoft.Json.Linq;

public abstract class BaseApiController : ApiController
{
    public HttpResponseMessage Post(object args)
    {
        try
        {
            return Request.CreateResponse(HttpStatusCode.OK, HandleRequest(args));
        }
        catch (MsgException ex)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new ApiResultMessage { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            return Request.CreateResponse(HttpStatusCode.OK, new ApiResultMessage { success = false, message = "Unknown error." });
        }
    }

    protected abstract ApiResultMessage HandleRequest(object args);
}
