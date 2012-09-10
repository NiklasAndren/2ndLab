using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab2.Models.Entities;
using Lab2.Models.Repositories;
using Lab2.ViewModels;
using Lab2.Models.SessionManager;

namespace Lab2.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Users/

        public ActionResult Index()
        {
            List<User> users = Repository.Instance.GetSortedUsers(10, 0);
            ViewBag.Users = users;

            return View();
        }

        //
        // GET: /Details/
        public ActionResult Details(string id)
        {
            UserDetailsViewModel vm = new UserDetailsViewModel();
            vm.User = Repository.Instance.GetUserByUserName(id);
            vm.Posts = Repository.Instance.GetLatestPostForUser(vm.User.ID, 5);
            return View(vm);
        }

        //
        // GET: /Create/
        public ActionResult Create()
        {
            if (SessionManager.CurrentUser == null || 
                SessionManager.CurrentUser.Type != Models.Entities.User.UserType.Admin)
                return RedirectToAction("Index", "Users");

            return View();
        }

        //
        // POST: /Create/
        [HttpPost]
        public ActionResult Create(User user)
        {
            if (SessionManager.CurrentUser == null ||
                SessionManager.CurrentUser.Type != Models.Entities.User.UserType.Admin)
                return RedirectToAction("Index", "Users");

            if (ModelState.IsValid)
            {
                Repository.Instance.Save<User>(user);

                return RedirectToAction("Index", "Users");
            }

            return View();
        }

        public ActionResult Footer()
        {
            List<User> Admins = Repository.Instance.GetSiteAdmins();

            return View(Admins);
        }
    }
}
