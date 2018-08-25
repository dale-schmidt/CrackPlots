using ForeSight.Web.Domain;
using ForeSight.Web.Models.ViewModels;
using Sabio.Web.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForeSight.Web.Controllers
{
    public class BaseController : Controller
    {
        public new ViewResult View()
        {
            BaseViewModel model = GetViewModel<BaseViewModel>();
            DecorateViewModel(model);
            return base.View(model);
        }

        public new ViewResult View(string viewString)
        {
            BaseViewModel model = GetViewModel<BaseViewModel>();
            DecorateViewModel(model);
            return base.View(viewString, model); //is this viewString supposed to be the name of the view to rendered? What's the use for this method?
        }

        public ViewResult View(BaseViewModel baseVM)

        {
            if (baseVM != null)
            {
                baseVM = DecorateViewModel<BaseViewModel>(baseVM);
            }
            return base.View(baseVM);
        }

        public ViewResult View(string viewString, BaseViewModel baseVM)
        {
            if (baseVM != null)
            {
                baseVM = DecorateViewModel<BaseViewModel>(baseVM);
            }
            return base.View(viewString, baseVM);
        }

        //Strongly Typed Layout Views
        //Sabio.layout.model to move out to layout
        protected T GetViewModel<T>() where T : BaseViewModel, new()
        {
            T model = new T();
            return DecorateViewModel(model);
        }

        protected T DecorateViewModel<T>(T model) where T : BaseViewModel
        {

            //the below method checks if the user is logged in when it gets their UserId 
            string aspNetUserId = UserService.GetCurrentUserId();
            if (!string.IsNullOrEmpty(aspNetUserId))
            {
                model.IsLoggedIn = true;

                Person person = new Person();
                person = UserService.GetPersonByAspNetUserId(aspNetUserId);

                model.UserEmail = person.Email;
                model.Id = person.Id;
                model.Name = person.Name;
            }
            return model;
        }
    }
}