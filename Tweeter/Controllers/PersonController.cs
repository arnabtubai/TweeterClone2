using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tweeter.Models;
using System.Data.Entity;
namespace Tweeter.Controllers
{
    public class PersonController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            List<Person> model = null;
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {


                    model = dbContext.People.Include(c => c.Followers).Include(c => c.Following).ToList().Where(x => x.user_id.ToUpper() != Session["User"].ToString().ToUpper()).ToList();

                    return View(model);
                }

               
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string strSearch)
        {
            List<Person> model = null;
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {


                    model = dbContext.People.Include(c => c.Followers).Include(c => c.Following).ToList().Where(x => x.user_id.ToUpper() != Session["User"].ToString().ToUpper() && x.FullName.ToUpper().Contains(strSearch.ToUpper())).ToList();

                    return View(model);
                }


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Followers()
        {
            List<Person> model = null;
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {


                    model = dbContext.People.Include(c => c.Followers).Include(c => c.Following).ToList().Where(x => x.user_id.ToUpper() == Session["User"].ToString().ToUpper()).ToList().First().Followers;

                    return View(model);
                }


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Following()
        {
            List<Person> model = null;
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {


                    model = dbContext.People.Include(c => c.Followers).Include(c => c.Following).ToList().Where(x => x.user_id.ToUpper() == Session["User"].ToString().ToUpper()).ToList().First().Following;

                    return View(model);
                }


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Follow(string id)
        {
            Person perObj = new Person();
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {

                    perObj = (from obj in dbContext.People
                              where obj.user_id.Trim().ToUpper() == id.Trim().ToUpper()
                              select obj).ToList().First();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(perObj);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Follow(string id, Person perObj)
        {
            using (TweeterContext dbContext = new TweeterContext())
            {
                perObj = (from obj in dbContext.People
                          where obj.user_id.Trim().ToUpper() == id.Trim().ToUpper()
                          select obj).ToList().First();
                string strCurUser = Session["User"].ToString();

                Person userObj = (from obj in dbContext.People
                                        where obj.user_id.Trim().ToUpper() == strCurUser.Trim().ToUpper()
                                        select obj).ToList().First();

                
                userObj.Following.Add(perObj);
                dbContext.SaveChanges();
                Session["Following"] = userObj.Following.Count;
                Session["Followers"] = userObj.Followers.Count;
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult UnFollow(string id)
        {
            Person perObj = new Person();
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {

                    perObj = (from obj in dbContext.People
                              where obj.user_id.Trim().ToUpper() == id.Trim().ToUpper()
                              select obj).ToList().First();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(perObj);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UnFollow(string id, Person perObj)
        {
            using (TweeterContext dbContext = new TweeterContext())
            {
                perObj = (from obj in dbContext.People
                          where obj.user_id.Trim().ToUpper() == id.Trim().ToUpper()
                          select obj).ToList().First();
                string strCurUser = Session["User"].ToString();

                Person userObj = (from obj in dbContext.People
                                  where obj.user_id.Trim().ToUpper() == strCurUser.Trim().ToUpper()
                                  select obj).ToList().First();

              
                userObj.Following.Remove(perObj);
                dbContext.SaveChanges();
                Session["Following"] = userObj.Following.Count;
                Session["Followers"] = userObj.Followers.Count;
            }
            return RedirectToAction("Index");
        }

        // GET: Person/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Person/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Person/Edit/5
        public ActionResult Edit(string id)
        {
            Person perObj = new Person();
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {

                    perObj = (from obj in dbContext.People
                              where obj.user_id.Trim().ToUpper() == id.Trim().ToUpper()
                              select obj).ToList().First();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(perObj);
        }

        // POST: Person/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, [Bind(Include = "Email,Password,FullName,user_id,Joined,Active")] Person perObj)
        {
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {
                    
                    dbContext.Entry(perObj).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return RedirectToAction("Delete/"+id);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(perObj);
        }

        // GET: Person/Delete/5
        [Authorize]
        public ActionResult Delete(string id)
        {
            using (TweeterContext dbContext = new TweeterContext())
            {
                Person perObj = (from obj in dbContext.People
                                       where obj.user_id.Trim().ToUpper() == id.Trim().ToUpper()
                                       select obj).ToList().First();
                return View(perObj);
            }
        }

        // POST: Person/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(string id, Person perObj)
        {
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {
                    List<Tweet> tweetObj = (from obj in dbContext.Tweets
                                      where obj.user_id.Trim().ToUpper() == id.Trim().ToUpper()
                                      select obj).ToList();
                    perObj = (from obj in dbContext.People
                              where obj.user_id.Trim().ToUpper() == id.Trim().ToUpper()
                              select obj).ToList().First();

                    dbContext.Tweets.RemoveRange(tweetObj);
                    dbContext.People.Find(id).Followers.RemoveAll(x =>x.user_id ==x.user_id);
                    dbContext.People.Find(id).Following.RemoveAll(x => x.user_id == x.user_id);
                    dbContext.People.Remove(perObj);

                    dbContext.SaveChanges();
                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    // Sign Out.    
                    authenticationManager.SignOut();
                    Session.Clear();
                    Session.Abandon();
                    return RedirectToAction("../Account/Login");
                }
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = "Error:" + ex.Message;
            }
            return View(perObj);
        }
    }
}
