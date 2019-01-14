using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tweeter.Models;

namespace Tweeter.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(PersonViewLoginModel model, string returnUrl)
        {
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {
                    // Verification.    
                    if (ModelState.IsValid)
                    {
                        // Initialization.    
                        bool isValid = ((from obj in dbContext.People
                                         where obj.user_id.ToUpper() == model.user_id.ToUpper() && obj.Password == model.Password.Trim()
                                         select obj).ToList().Count > 0);
                        // Verification.    
                        if (isValid)
                        {
                            // Initialization.    
                            var personObj = (from obj in dbContext.People
                                                where obj.user_id.ToUpper() == model.user_id.ToUpper() && obj.Password == model.Password.Trim()
                                                select obj).ToList().First();

                           

                            // Login In.    
                            this.SignInUser(model.user_id, personObj.FullName, false);
                            // setting.    

                            this.Session["User"] = model.user_id;
                            this.Session["UserName"] = personObj.FullName;
                            this.Session["Followers"] = personObj.Followers.Count;
                            this.Session["Following"] = personObj.Following.Count;
                            this.Session["Tweets"] = personObj.Tweets.Count;
                            // Info.    
                            return this.RedirectToLocal("~/Tweets/Index");
                        }
                        else
                        {
                            // Setting.    
                            ModelState.AddModelError(string.Empty, "Invalid username or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Info    
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            // If we got this far, something failed, redisplay form    
            return this.View(model);
        }
      

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(PersonViewRegisterModel model)
        {
            try
            {
                using (TweeterContext dbContext = new TweeterContext())
                {
                    if (ModelState.IsValid)
                    {
                        bool isInValid = ((from obj in dbContext.People
                                           where obj.user_id.ToUpper() == model.user_id.ToUpper()
                                           select obj).ToList().Count > 0);
                        if (isInValid)
                        {
                            ModelState.AddModelError("", "User Id already exists");
                            return View(model);
                        }
                        Person user = new Person { user_id = model.user_id, Password = model.Password, Email = model.Email, FullName = model.FullName, Joined = DateTime.Now, Active = true };
                        var result = dbContext.People.Add(user);
                        int iResult = dbContext.SaveChanges();
                        if (iResult > 0)
                        {
                            // Login In.    
                            this.SignInUser(result.user_id, result.FullName, false);
                            // setting.    

                            this.Session["User"] = result.user_id;
                            this.Session["UserName"] = result.FullName;
                           
                            this.Session["Followers"] = result.Followers.Count;
                            this.Session["Following"] = result.Following.Count;
                            this.Session["Tweets"] = result.Tweets.Count;
                            // Info.    
                            return this.RedirectToLocal("~/Tweets/Index");


                        }
                        //AddErrors(result);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>    
        /// Sign In User method.    
        /// </summary>    
        /// <param name="username">Username parameter.</param>    
        /// <param name="role_id">Role ID parameter</param>    
        /// <param name="isPersistent">Is persistent parameter.</param>    
        private void SignInUser(string username, string fullName, bool isPersistent)
        {
            // Initialization.    
            var claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(ClaimTypes.GivenName, fullName));
                //claims.Add(new Claim(ClaimTypes.Role, role_id.ToString()));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
        }

      

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                // Setting.    
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign Out.    
                authenticationManager.SignOut();
                Session.Clear();
                Session.Abandon();
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Login", "Account");
        }

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

      
        #endregion
    }
}