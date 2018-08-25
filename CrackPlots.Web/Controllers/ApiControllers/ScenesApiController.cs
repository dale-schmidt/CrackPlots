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
    [RoutePrefix("api/scenes")]
    public class ScenesApiController : ApiController
    {
        [Route, HttpPost]
        public HttpResponseMessage Insert(SceneAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = SceneService.Insert(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(SceneUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            SceneService.Update(model);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            SceneService.Delete(id);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage SelectById(int id)
        {
            ItemResponse<Scene> response = new ItemResponse<Scene>();
            response.Item = SceneService.SelectById(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("act/{sequenceId:int}"), HttpGet]
        public HttpResponseMessage SelectByProjectId(int sequenceId)
        {
            ItemsResponse<Scene> response = new ItemsResponse<Scene>();
            response.Items = SceneService.SelectBySequenceId(sequenceId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
