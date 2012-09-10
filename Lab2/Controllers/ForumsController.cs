using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lab2.Models.Entities;
using Lab2.Models.Repositories;
using Lab2.ViewModels;
using Lab2.Models.SessionManager;
using System.Runtime.InteropServices;

namespace Lab2.Controllers
{
    public class ForumsController : Controller
    {
        //
        // GET: /Forums/

        public ActionResult Index()
        {
            List<ForumThread> threads = Repository.Instance.GetSortedThreads();

            return View(threads);
        }


        public ActionResult Thread(Guid id)
        {

            ThreadViewModel MyThread = new ThreadViewModel();
            MyThread.Posts = Repository.Instance.GetPostbyThreadId(id);
            MyThread.Thread = Repository.Instance.Get<ForumThread>(id);

            return View(MyThread);
        }

        [Authorize]
        public ActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(Post post, Guid? id )
        {
            if (ModelState.IsValid)
            {
                if (SessionManager.CurrentUser != null)
                {
                    post.CreatedByID = SessionManager.CurrentUser.ID;
                }

                if (id == null)
                {
                    ForumThread newthread = new ForumThread();

                    newthread.CreateDate = DateTime.Now;
                    newthread.Title = post.Title;

                    Repository.Instance.Save<ForumThread>(newthread);

                    post.ThreadID = newthread.ID;
                    post.CreateDate = DateTime.Now;

                    Repository.Instance.Save<Post>(post);

                    return RedirectToAction("Thread" + "/" + newthread.ID, "Forums");
                }
                else
                {
                    post.ThreadID = (Guid)id;
                    post.CreateDate = DateTime.Now;
           
                    Repository.Instance.Save<Post>(post);

                    return RedirectToAction("Thread" + "/" + id, "Forums");
                }

            }

            return View();
        }
    }
}
