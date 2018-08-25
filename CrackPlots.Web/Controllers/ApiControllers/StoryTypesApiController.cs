using ForeSight.Web.Domain;
using ForeSight.Web.Models.Responses;
using ForeSight.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ForeSight.Web.Controllers.ApiControllers
{
    [RoutePrefix("api/storytypes")]
    public class StoryTypesApiController : ApiController
    {
        [Route, HttpGet]
        public HttpResponseMessage GetAll()
        {
            ItemsResponse<StoryType> response = new ItemsResponse<StoryType>();
            response.Items = StoryTypeService.SelectAll();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
