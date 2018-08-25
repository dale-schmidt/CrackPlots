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
    [RoutePrefix("api/sequences")]
    public class SequencesApiController : ApiController
    {
        [Route, HttpPost]
        public HttpResponseMessage Insert(SequenceAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = SequenceService.Insert(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(SequenceUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            SequenceService.Update(model);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("scenes/{id:int}"), HttpPut]
        public HttpResponseMessage ScenesUpdate(SequenceScenesUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = SequenceService.ScenesUpdate(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("all"), HttpPut]
        public HttpResponseMessage TitlesUpdate(SequenceTitlesUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = SequenceService.TitlesUpdate(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            SequenceService.Delete(id);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage SelectById(int id)
        {
            ItemResponse<Sequence> response = new ItemResponse<Sequence>();
            response.Item = SequenceService.SelectById(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("act/{actId:int}"), HttpGet]
        public HttpResponseMessage SelectByProjectId(int actId)
        {
            ItemsResponse<Sequence> response = new ItemsResponse<Sequence>();
            response.Items = SequenceService.SelectByActId(actId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
