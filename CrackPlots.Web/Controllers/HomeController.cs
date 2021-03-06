﻿using ForeSight.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForeSight.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            BaseViewModel model = new BaseViewModel();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FAQ()
        {
            BaseViewModel model = new BaseViewModel();
            return View(model);
        }

        public ActionResult HowTo()
        {
            return View();
        }
    }
}