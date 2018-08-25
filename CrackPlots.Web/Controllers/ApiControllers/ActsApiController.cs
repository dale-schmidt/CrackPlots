using ForeSight.Web.Domain;
using ForeSight.Web.Models.Requests;
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
    [RoutePrefix("api/acts")]
    public class ActsApiController : ApiController
    {
        [Route, HttpPost]
        public HttpResponseMessage Insert(ActAddRequest model)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = ActService.Insert(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(ActUpdateRequest model)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ActService.Update(model);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            ActService.Delete(id);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage SelectById(int id)
        {
            ItemResponse<Act> response = new ItemResponse<Act>();
            response.Item = ActService.SelectById(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("project/{projectId:int}"), HttpGet]
        public HttpResponseMessage SelectByProjectId(int projectId)
        {
            ItemsResponse<Act> response = new ItemsResponse<Act>();
            response.Items = ActService.SelectByProjectId(projectId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
