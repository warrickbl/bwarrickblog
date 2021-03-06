﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bwarrickblog.Models;
using bwarrickblog.Helpers;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace bwarrickblog
{
    [RequireHttps]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index(int? page)           
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            if(Request.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(db.Posts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
            }

            return View(db.Posts.Where(p => p.Published == true).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

        // POST: Posts
        [HttpPost]
        public ActionResult Index(string searchStr, int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.Search = searchStr;
            SearchHeper search = new SearchHeper();
            var blogList = search.IndexSearch(searchStr);

            if(Request.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View(blogList.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
            }
            return View(blogList.Where(p => p.Published == true).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }


        // GET: Posts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Slug == Slug);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }


        // GET: Posts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaUrl,Published")] Post post, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format");
            }
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var filePath = "/Assets/Aurora/Site/Parallax/assets/images/";
                    var absPath = Server.MapPath("~" + filePath);
                    post.MediaUrl = filePath + image.FileName;
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }
                var Slug = StringUtilities.URLFriendly(post.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(post);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(post);
                }

                post.Slug = Slug;
                post.Created = DateTime.Now;
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }


        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Created, Updated, MediaUrl,Published,Slug")] Post post, string MediaUrl, HttpPostedFileBase image)
        {
            post.Updated = System.DateTime.Now;
           
            if (image != null && image.ContentLength > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format");
            }
            if (ModelState.IsValid)
            {

                
                if (image != null)
                {
                    var filePath = "/Assets/Aurora/Site/Parallax/assets/images/";
                    var absPath = Server.MapPath("~" + filePath);
                    post.MediaUrl = filePath + image.FileName;
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }
                else
                {
                    post.MediaUrl = MediaUrl;
                }
                //In order to keep the slug, we need to use AsNoTracking
                var title = db.Posts.AsNoTracking().FirstOrDefault(p => p.Id == post.Id).Title;
                if (post.Title != title) { 
                var Slug = StringUtilities.URLFriendly(post.Title);
                    
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(post);
                }
                    post.Slug = Slug;
                }
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
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
