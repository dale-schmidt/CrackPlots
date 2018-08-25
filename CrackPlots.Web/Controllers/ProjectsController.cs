using ForeSight.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForeSight.Web.Controllers
{
    [RoutePrefix("projects")]
    public class ProjectsController : BaseController
    {
        [Route("index")]
        public ActionResult Index()
        {
            BaseViewModel model = new BaseViewModel();
            return View(model);
        }
        [Route("tv/{id:int}")]
        public ActionResult TV(int id)
        {
            ItemViewModel<int> model = new ItemViewModel<int>();
            model.Item = id;
            return View(model);
        }
        [Route("episode/{id:int}")]
        public ActionResult Episode(int id)
        {
            ItemViewModel<int> model = new ItemViewModel<int>();
            model.Item = id;
            return View(model);
        }
        [Route("edit/{id:int}")]
        public ActionResult Edit(int id)
        {
            ItemViewModel<int> model = new ItemViewModel<int>();
            model.Item = id;
            return View(model);
        }
        [Route("acts/{id:int}")]
        public ActionResult Acts(int id)
        {
            ItemViewModel<int> model = new ItemViewModel<int>();
            model.Item = id;
            return View(model);
        }
        [Route("print/{id:int}")]
        public ActionResult Print(int id)
        {
            ItemViewModel<int> model = new ItemViewModel<int>();
            model.Item = id;
            return View(model);
        }
    }
}