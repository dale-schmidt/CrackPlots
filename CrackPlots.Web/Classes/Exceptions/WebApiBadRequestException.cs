using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ForeSight.Web.Classes.Exceptions
{
    [Serializable]
    public class WebApiBadRequestException : Exception
    {
        public WebApiBadRequestException()
        {
        }
        public WebApiBadRequestException(string message)
            : base(message)
        {
        }
        public WebApiBadRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
        protected WebApiBadRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}