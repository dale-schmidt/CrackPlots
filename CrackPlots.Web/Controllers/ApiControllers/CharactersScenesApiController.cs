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
    [RoutePrefix("api/charactersscenes")]
    public class CharactersScenesApiController : ApiController
    {
        [Route, HttpPost]
        public HttpResponseMessage Insert(CharacterSceneRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            CharacterSceneService.Insert(model);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route, HttpPut]
        public HttpResponseMessage Update(CharacterSceneRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            CharacterSceneService.Update(model);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{characterId:int}/{sceneId:int}"), HttpDelete]
        public HttpResponseMessage Delete(int characterId, int sceneId)
        {
            CharacterSceneService.Delete(characterId, sceneId);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("scene/{sceneId:int}"), HttpGet]
        public HttpResponseMessage SelectBySceneId(int sceneId)
        {
            ItemsResponse<CharacterScene> response = new ItemsResponse<CharacterScene>();
            response.Items = CharacterSceneService.SelectBySceneId(sceneId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("exits"), HttpGet]
        public HttpResponseMessage SelectAllCharacterSceneExitTypes()
        {
            ItemsResponse<CharacterSceneExitTypes> response = new ItemsResponse<CharacterSceneExitTypes>();
            response.Items = CharacterSceneService.SelectAllExitTypes();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
