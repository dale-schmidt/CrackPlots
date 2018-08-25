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
    [RoutePrefix("api/characters")]
    public class CharactersApiController : ApiController
    {
        [Route, HttpPost]
        public HttpResponseMessage Insert(CharacterAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = CharacterService.Insert(model);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(CharacterUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            CharacterService.Update(model);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            CharacterService.Delete(id);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage SelectById(int id)
        {
            ItemResponse<Character> response = new ItemResponse<Character>();
            response.Item = CharacterService.SelectById(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("project/{projectId:int}"), HttpGet]
        public HttpResponseMessage SelectByProjectId(int projectId)
        {
            ItemsResponse<Character> response = new ItemsResponse<Character>();
            response.Items = CharacterService.SelectByProjectId(projectId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("scene/{sceneId:int}"), HttpGet]
        public HttpResponseMessage SelectBySceneId(int sceneId)
        {
            ItemsResponse<Character> response = new ItemsResponse<Character>();
            response.Items = CharacterService.SelectBySceneId(sceneId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
