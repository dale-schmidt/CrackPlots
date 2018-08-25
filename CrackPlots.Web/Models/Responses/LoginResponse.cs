using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Responses
{
    public class LoginResponse : BaseResponse
    {
        public string Message { get; set; }
        //public DateTime dateTokenCreated { get; set; }
        public bool HasError { get; set; }
    }
}