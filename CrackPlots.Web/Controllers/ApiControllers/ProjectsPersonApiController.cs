using ForeSight.Web.Domain;
using ForeSight.Web.Models.Requests;
using ForeSight.Web.Models.Responses;
using ForeSight.Web.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ForeSight.Web.Controllers.ApiControllers
{
    [RoutePrefix("api/projectsperson")]
    public class ProjectsPersonApiController : ApiController
    {
        [Route, HttpPost]
        public HttpResponseMessage Add(ProjectPersonRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<Person> response = new ItemResponse<Person>();
            try
            {
                response.Item = ProjectPersonService.Add(model);
            }
            catch (Exception ex)
            {
                if(ex.GetType() == typeof(SqlException))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [Route("{projectId:int}/{personId:int}"), HttpDelete]
        public HttpResponseMessage Delete(int projectId, int personId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ProjectPersonService.Delete(projectId, personId);
            SuccessResponse response = new SuccessResponse();
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
