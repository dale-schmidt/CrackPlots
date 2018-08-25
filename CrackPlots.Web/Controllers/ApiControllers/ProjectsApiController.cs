using ForeSight.Web.Domain;
using ForeSight.Web.Models.Requests;
using ForeSight.Web.Models.Responses;
using ForeSight.Web.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace ForeSight.Web.Controllers.ApiControllers
{
    [RoutePrefix("api/projects")]
    public class ProjectsApiController : ApiController
    {
        [Route, HttpPost]
        public HttpResponseMessage Insert(ProjectAddRequest model)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<int> response = new ItemResponse<int>();
            if (model.AutoStructured)
            {
                response.Item = ProjectService.InsertStructured(model);
            }
            else
            {
                response.Item = ProjectService.Insert(model);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(ProjectUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ProjectService.Update(model);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            ItemResponse<Project> response = new ItemResponse<Project>();
            response.Item = ProjectService.SelectById(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("episodes/{id:int}"), HttpGet]
        public HttpResponseMessage GetEpisodeById(int id)
        {
            ItemResponse<Project> response = new ItemResponse<Project>();
            response.Item = ProjectService.SelectEpisodeById(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("print/{id:int}")]
        public HttpResponseMessage GetByIdPrint(int id)
        {
            ItemResponse<Project> response = new ItemResponse<Project>();
            response.Item = ProjectService.SelectByIdPrint(id);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route, HttpGet]
        public HttpResponseMessage GetByPersonId()
        {
            string aspNetUserId = HttpContext.Current.User.Identity.GetUserId();
            ItemsResponse<Project> response = new ItemsResponse<Project>();
            response.Items = ProjectService.SelectByPersonId(aspNetUserId);
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            ProjectService.Delete(id);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
