using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tweeter.Models;

namespace Tweeter.Controllers
{
    public class TweetsController : Controller
    {
        private TweeterContext db = new TweeterContext();

        [Authorize]
       
        // GET: Tweets
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                List<Tweet> tweets = db.People.Include(c => c.Following).ToList().Where(x => x.user_id.ToUpper() == Session["User"].ToString().ToUpper()).First().Following.ToList().SelectMany(x => x.Tweets).ToList();
                List<Tweet> personTweets = db.Tweets.Include(c => c.Person).ToList().Where(x => x.user_id.ToUpper() == Session["User"].ToString().ToUpper()).OrderByDescending(x => x.Created).ToList();
                Session["Tweets"] = personTweets.Count;
                List<Tweet> finalTweets = (List<Tweet>)tweets.Concat(personTweets).OrderByDescending(x => x.Created).ToList();
                return View(finalTweets);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View();
        }

       
        // GET: Tweets/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.user_id = Session["User"].ToString();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View();
        }

        // POST: Tweets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Message")]Tweet tweet)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tweet.user_id = Session["User"].ToString();
                    tweet.Created = DateTime.Now;
                    db.Tweets.Add(tweet);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.user_id = Session["User"].ToString();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(tweet);
        }

        // GET: Tweets/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tweet tweet = null;
            try
            {
                tweet = db.Tweets.Find(id);
                if (tweet == null)
                {
                    return HttpNotFound();
                }
                ViewBag.user_id = Session["User"].ToString();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(tweet);
        }

        // POST: Tweets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit( Tweet tweet)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tweet.Created = DateTime.Now;
                    tweet.user_id = Session["User"].ToString();
                    db.Entry(tweet).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.user_id = Session["User"].ToString();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(tweet);
        }

        // GET: Tweets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tweet tweet = null;
            try
            {
                tweet = db.Tweets.Find(id);
                if (tweet == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(tweet);
        }

        // POST: Tweets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Tweet tweet = db.Tweets.Find(id);
                db.Tweets.Remove(tweet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View();
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
