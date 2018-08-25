using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ForeSight.Web.Classes.Exceptions
{
    [Serializable]
    public class WebApiMissingRouteException : Exception
    {
        public WebApiMissingRouteException()
        {
        }
        public WebApiMissingRouteException(string message)
            : base(message)
        {
        }
        public WebApiMissingRouteException(string message, Exception inner)
            : base(message, inner)
        {
        }
        protected WebApiMissingRouteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}