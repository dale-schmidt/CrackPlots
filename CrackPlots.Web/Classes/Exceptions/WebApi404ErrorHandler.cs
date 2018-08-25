using ForeSight.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ForeSight.Web.Classes.Exceptions
{
    public class WebApi404ErrorHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            try
            {
                if ((int)response.StatusCode == 404)
                {
                    throw new WebApiMissingRouteException("The requested API endpoint was not found");
                }
            }
            catch (Exception ex)
            {
                ExceptionLoggingService.Insert(ex);
            }
            return response;
        }
    }
}