using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Responses
{
    public class ItemResponse<T> : SuccessResponse
    {

        public T Item { get; set; }

    }
}