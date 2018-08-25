using ForeSight.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Filters;

namespace ForeSight.Web.Classes.Exceptions
{
    public class WebApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            ExceptionLoggingService.Insert(actionExecutedContext.Exception);
            actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}