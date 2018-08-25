﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.Exceptions
{
    public class IdentityResultException : Exception
    {
        public IdentityResultException(IdentityResult result)
        {
            this.Result = result;
        }

        public IdentityResult Result { get; set; }

    }
}