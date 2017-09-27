using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bwarrickblog.Models;
using Microsoft.AspNet.Identity;

namespace bwarrickblog
{
    [RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        //public ActionResult Index()
        //{
        //    var comments = db.Comments.Include(c => c.BlogPost);
        //    return View(comments.ToList());
        //}

        // GET: Comments/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Comment comment = db.Comments.Find(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(comment);
        //}

        // GET: Comments/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BlogPostId,AuthorId,Body,Created,Updated,UpdateReason")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                comment.Created = DateTime.Now;
                comment.AuthorId = user.Id;
                db.Comments.Add(comment);
                db.SaveChanges();

                Post post = db.Posts.Find(comment.BlogPostId);
                return RedirectToAction("Details", "Posts", new { slug = post.Slug });
            }

            return View(comment);
        }

        // GET: Comments/Edit/5
        //[Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Comment comment = db.Comments.Find(id);
            if ((User.IsInRole("Admin") || User.IsInRole("Moderator")) || comment.AuthorId == user.Id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BlogPostId,AuthorId,Body,Created,Updated,UpdateReason")] Comment comment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if ((User.IsInRole("Admin") || User.IsInRole("Moderator")) || comment.AuthorId == user.Id)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(comment).State = EntityState.Modified;
                    comment.Updated = DateTime.Now;
                    db.SaveChanges();
                    Post post = db.Posts.Find(comment.BlogPostId);
                    return RedirectToAction("Details", "Posts", new { slug = post.Slug });
                }
                //ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title", comment.BlogPostId);
                return View(comment);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: Comments/Delete/5
        //[Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Comment comment = db.Comments.Find(id);
            if ((User.IsInRole("Admin") || User.IsInRole("Moderator")) || comment.AuthorId == user.Id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        // POST: Comments/Delete/5
        //[Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Comment comment = db.Comments.Find(id);
            if ((User.IsInRole("Admin") || User.IsInRole("Moderator")) || comment.AuthorId == user.Id) { 
              
            db.Comments.Remove(comment);
            db.SaveChanges();
            Post post = db.Posts.Find(comment.BlogPostId);
            return RedirectToAction("Details", "Posts", new { slug = post.Slug });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
