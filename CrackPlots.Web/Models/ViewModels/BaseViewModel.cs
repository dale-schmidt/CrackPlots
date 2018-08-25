using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForeSight.Web.Models.ViewModels
{
    public class BaseViewModel
    {
        public bool IsLoggedIn { get; set; }
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string Name { get; set; }
    }
}