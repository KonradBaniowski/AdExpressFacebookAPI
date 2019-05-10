using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.Identity;
using Km.AdExpressClientWeb.Models;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Date.DAL;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Web;
using TNS.AdExpress.Domain.Translation;
using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Helpers;
using Newtonsoft.Json;
using NLog;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.DataAccess;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;
using System.Net.Http;
using TNS.AdExpress.Web.Utilities.Exceptions;


namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
    public class AccountController : Controller
    {
        private IApplicationUserManager _userManager;
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public AccountController(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, int siteLanguage = -1, string err = "")
        {
            if (siteLanguage == -1) siteLanguage = WebApplicationParameters.DefaultLanguage;

            ViewBag.LoginProviders = _userManager.GetExternalAuthenticationTypes();
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(Convert.ToInt32(siteLanguage));
            ViewBag.SiteLanguageCode = siteLanguage;

            if (WebApplicationParameters.EnableGdpr)
            {
                ViewBag.ForceCookieReInit = false;

                switch (WebApplicationParameters.CountryCode)
                {
                    case CountryCode.FRANCE:
                        ViewBag.PolicyUpdateDate = System.Configuration.ConfigurationManager.AppSettings["PolicyUpdateDateFr"];
                        break;
                    case CountryCode.POLAND:
                        ViewBag.PolicyUpdateDate = System.Configuration.ConfigurationManager.AppSettings["PolicyUpdateDatePl"];
                        break;
                    case CountryCode.SLOVAKIA:
                        ViewBag.PolicyUpdateDate = System.Configuration.ConfigurationManager.AppSettings["PolicyUpdateDateSk"];
                        break;
                    case CountryCode.FINLAND:
                        ViewBag.PolicyUpdateDate = System.Configuration.ConfigurationManager.AppSettings["PolicyUpdateDateFi"];
                        break;
                    case CountryCode.TURKEY:
                        ViewBag.PolicyUpdateDate = System.Configuration.ConfigurationManager.AppSettings["PolicyUpdateDateTr"];
                        break;
                    default:
                        throw new Exception("Get PolicyUpdateDate : Country Code not handled !!!");
                }
            }

            LoginViewModel model = new LoginViewModel
            {
                ErrorMessage = GestionWeb.GetWebWord(880, Convert.ToInt32(siteLanguage)),
                Labels = LabelsHelper.LoadPageLabels(Convert.ToInt32(siteLanguage)),
                SiteLanguage = Convert.ToInt32(siteLanguage)
            };

            if (!String.IsNullOrEmpty(err))
            {
                _userManager.SignOut();

                if (err.Equals("Error while initializing session !!!"))
                {
                    ModelState.AddModelError("", "Error while initializing session !!!");
                    model.ErrorMessage = "Error while initializing session !!!";
                }
                else
                {
                    ModelState.AddModelError("", GestionWeb.GetWebWord(3110, Convert.ToInt32(siteLanguage)));
                    model.ErrorMessage = GestionWeb.GetWebWord(3110, Convert.ToInt32(siteLanguage));
                }
            }

            return View(model);
        }
        //
        // GET: /Account/ChangeLanguage
        [AllowAnonymous]
        public JsonResult ChangeLanguage(string returnUrl, int siteLanguage = -1)
        {
            if (siteLanguage == -1) siteLanguage = WebApplicationParameters.DefaultLanguage;

            ViewBag.LoginProviders = _userManager.GetExternalAuthenticationTypes();
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(Convert.ToInt32(siteLanguage));
            LoginViewModel model = new LoginViewModel
            {
                ErrorMessage = GestionWeb.GetWebWord(880, Convert.ToInt32(siteLanguage)),
                Labels = LabelsHelper.LoadPageLabels(Convert.ToInt32(siteLanguage)),
                SiteLanguage = Convert.ToInt32(siteLanguage),
                RedirectUrl = string.Format("{0}?siteLanguage={1}", returnUrl, siteLanguage)

            };
            JsonResult jsonModel = Json(model, JsonRequestBehavior.AllowGet);
            return jsonModel;
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //TO DO
            ViewBag.LoginProviders = _userManager.GetExternalAuthenticationTypes();
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(model.SiteLanguage);
            ViewBag.SiteLanguageCode = model.SiteLanguage;

            //model.ReturnUrl = returnUrl;
            if (!ModelState.IsValid || String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", GestionWeb.GetWebWord(880, Convert.ToInt32(model.SiteLanguage)));
                model.Labels = LabelsHelper.LoadPageLabels(Convert.ToInt32(model.SiteLanguage));
                return View(model);
            }

            if (WebApplicationParameters.EnableGdpr)
            {
                string[] cookiesKeys = {};

                try
                {
                    cookiesKeys = Request.Cookies.AllKeys;
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", GestionWeb.GetWebWord(3273, Convert.ToInt32(model.SiteLanguage)));
                    model.Labels = LabelsHelper.LoadPageLabels(Convert.ToInt32(model.SiteLanguage));
                    ViewBag.ForceCookieReInit = true;
                    return View(model);
                }

                List<string> rgpdCookies = new List<string>();

                foreach (var key in cookiesKeys)
                {
                    if(key.StartsWith("cookieControlPrefs"))
                        rgpdCookies.Add(key);
                }

                if (rgpdCookies.Count == 0)
                {
                    ModelState.AddModelError("", GestionWeb.GetWebWord(3273, Convert.ToInt32(model.SiteLanguage)));
                    model.Labels = LabelsHelper.LoadPageLabels(Convert.ToInt32(model.SiteLanguage));
                    return View(model);
                }

                bool cookieExist = false;
                foreach (var cookie in rgpdCookies)
                {
                    var cookieValue = Request.Cookies[cookie];

                    if (cookieValue != null)
                    {
                        var prefs = JsonConvert.DeserializeObject<GdprCookie>(cookieValue.Value);

                        if (string.IsNullOrEmpty(prefs.guid))
                        {
                            cookieExist = true;
                            break;
                        }

                        if (Helpers.SecurityHelper.Decrypt(prefs.guid, Helpers.SecurityHelper.CryptKey).ToLower() == model.Email.ToLower())
                        {
                            cookieExist = true;
                        }
                    }
                }

                if (!cookieExist)
                {
                    ModelState.AddModelError("", GestionWeb.GetWebWord(3273, Convert.ToInt32(model.SiteLanguage)));
                    model.Labels = LabelsHelper.LoadPageLabels(Convert.ToInt32(model.SiteLanguage));
                    ViewBag.ForceCookieReInit = true;
                    return View(model);
                }
            }

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            //MOCK List<>MODULE  

            var result = await _userManager.PasswordSignIn(model.Email, model.Password, false, shouldLockout: false);
            string message = string.Empty;
            switch (result)
            {
                case SignInStatus.Success:
                    //message = string.Format("The user {0} has been sucessfully logged with the following password {1}.", model.Email, model.Password);
                    //Logger.Log(LogLevel.Info, message);
                    return RedirectToAction("webSession", new { SiteLanguage = model.SiteLanguage });
                case SignInStatus.LockedOut:
                    //message = string.Format("The user {0} has been locked out with the following password {1}.", model.Email, model.Password);
                    //Logger.Log(LogLevel.Info, message);
                    return View("Lockout");
                case SignInStatus.RequiresTwoFactorAuthentication:
                    //message = string.Format("The user {0} has sucessfully tried to login with the following password {1}. A verification code will be sent shortly.", model.Email, model.Password);
                    //Logger.Log(LogLevel.Info, message);
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                default:
                    ModelState.AddModelError("", GestionWeb.GetWebWord(880, Convert.ToInt32(model.SiteLanguage)));
                    model.Labels = LabelsHelper.LoadPageLabels(Convert.ToInt32(model.SiteLanguage));
                    //message = string.Format("The user {0} has sucessfully tried to login with the following password {1}. The login page will be reloaded.", model.Email, model.Password);
                    //Logger.Log(LogLevel.Info, message);
                    return View(model);
            }
        }


        [Authorize(Roles = Role.ADEXPRESS)]
        public ActionResult WebSession(int siteLanguage = -1)
        {
            WebSession _webSession = null;
            int _siteLanguage = WebApplicationParameters.DefaultLanguage;

            try
            {
                if (siteLanguage == -1) siteLanguage = WebApplicationParameters.DefaultLanguage;

                var cla = new ClaimsPrincipal(User.Identity);
                var idLogin = cla.Claims.Where(e => e.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                var login = cla.Claims.Where(e => e.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
                var password = cla.Claims.Where(e => e.Type == ClaimTypes.Hash).Select(c => c.Value).SingleOrDefault();
                var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                
                if (_siteLanguage != siteLanguage) _siteLanguage = siteLanguage;
                var right = new TNS.AdExpress.Right(long.Parse(idLogin), login, password, _siteLanguage);
                if (right != null && right.CanAccessToAdExpress())
                {
                    right.SetModuleRights();
                    right.SetFlagsRights();
                    right.SetRights();
                    if (WebApplicationParameters.VehiclesFormatInformation.Use)
                        right.SetBannersAssignement();

                    //newRight.HasModuleAssignmentAlertsAdExpress();
                    if (_webSession == null) _webSession = new WebSession(right);
                    _webSession.IdSession = idWS;
                    _webSession.SiteLanguage = _siteLanguage;

                    if (WebApplicationParameters.EnableGdpr)
                    {
                        HttpCookie cookieControlPrefs = null;
                        string cookieName = "cookieControlPrefs-" + idLogin;
                        var cookiesKeys = Request.Cookies.AllKeys;
                        var found = cookiesKeys.FirstOrDefault(n => n == "cookieControlPrefs");

                        if (!string.IsNullOrEmpty(found))
                        {
                            cookieControlPrefs = Request.Cookies["cookieControlPrefs"];
                        }
                        else
                        {
                            foreach (var key in cookiesKeys)
                            {
                                if (key.StartsWith("cookieControlPrefs"))
                                {
                                    var id = key.Split('-')[1];
                                    if (Convert.ToInt64(id) == Convert.ToInt64(idLogin))
                                    {
                                        cookieControlPrefs = Request.Cookies[key];
                                        break;
                                    }
                                }
                            }
                        }

                        if (cookieControlPrefs != null)
                        {
                            var cookies = JsonConvert.DeserializeObject<GdprCookie>(cookieControlPrefs.Value);

                            var enableTracking = cookies.prefs.FirstOrDefault(s => s.Contains("Statistics"));
                            var enableTroubleshooting = cookies.prefs.FirstOrDefault(s => s.Contains("Diagnostic"));
                            int enableTrackingDb = 0;
                            int enableTroubleshootingDb = 0;

                            if (enableTracking != null)
                            {
                                _webSession.EnableTracking = true;
                                enableTrackingDb = 1;
                            }
                            else
                                _webSession.EnableTracking = false;

                            if (enableTroubleshooting != null)
                            {
                                _webSession.EnableTroubleshooting = true;
                                enableTroubleshootingDb = 1;
                            }
                            else
                                _webSession.EnableTroubleshooting = false;

                            if (!cookies.storedInDb)
                            {
                                IDataSource Source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.rights);
                                var expDateCookie = DateTime.Now.AddDays(395);
                                RightDAL.SetAllPrivacySettings(Source, Convert.ToInt32(idLogin), enableTrackingDb, enableTroubleshootingDb, expDateCookie);

                                cookies.storedInDb = true;
                                cookies.creationDate = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                                cookies.expDate = expDateCookie.ToString("yyyy-MM-dd-HH-mm-ss");
                                cookies.guid = Helpers.SecurityHelper.Encrypt(login.ToLower(), Helpers.SecurityHelper.CryptKey);
                                cookieControlPrefs.Name = cookieName;
                                cookieControlPrefs.Value = JsonConvert.SerializeObject(cookies);
                                cookieControlPrefs.Expires = expDateCookie;
                                Response.Cookies.Add(cookieControlPrefs);
                                var cookieTmp = Response.Cookies["cookieControlPrefs"];
                                if (cookieTmp != null)
                                    cookieTmp.Expires = DateTime.Now.AddDays(-1);
                            }
                            else
                            {
                                IDataSource Source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.rights);
                                bool allowTracking = false;
                                bool allowTroubleshooting = false;
                                DateTime expDate = new DateTime(2000, 1, 1);
                                RightDAL.GetPrivacySettings(Source, Convert.ToInt32(idLogin), out allowTracking, out allowTroubleshooting, out expDate);

                                cookies.prefs = new List<string>();

                                if (allowTracking)
                                {
                                    cookies.prefs.Add("Statistics");
                                    _webSession.EnableTracking = true;
                                }
                                else
                                    _webSession.EnableTracking = false;

                                if (allowTroubleshooting)
                                {
                                    cookies.prefs.Add("Diagnostic");
                                    _webSession.EnableTroubleshooting = true;
                                }
                                else
                                    _webSession.EnableTroubleshooting = false;

                                cookies.creationDate = expDate.AddDays(-395).ToString("yyyy-MM-dd-HH-mm-ss");
                                cookies.expDate = expDate.ToString("yyyy-MM-dd-HH-mm-ss");
                                cookieControlPrefs.Expires = expDate;
                                cookieControlPrefs.Value = JsonConvert.SerializeObject(cookies);
                                Response.Cookies.Add(cookieControlPrefs);
                            }
                        }
                    }
                    else
                    {
                        _webSession.EnableTracking = true;
                        _webSession.EnableTroubleshooting = true;
                    }

                    // Année courante pour les recaps                    
                    TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                    if (cl == null) throw (new NullReferenceException("Core layer is null for the Date DAL"));
                    IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);

                    //TODO : a modifier
                    // if(!WebApplicationParameters.CountryCode.Equals(CountryCode.TURKEY))
                    _webSession.DownLoadDate = dateDAL.GetLastLoadedYear();

                    // On met à jour IDataSource à partir de la session elle même.
                    _webSession.Source = right.Source;
                    //Sauvegarder la session
                    _webSession.Save();
                    // Tracking (NewConnection)
                    // On obtient l'adresse IP: 
                    _webSession.OnNewConnection(this.Request.UserHostAddress);
                }

                ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);

                var navigationSession = (WebSession) TNS.AdExpress.Web.Core.Sessions.WebSession.Load(idWS);
            }
            catch (Exception ex)
            {
                if (_webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(this.HttpContext, ex.Message, ex.StackTrace, _webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                return RedirectToAction("Login", new {returnUrl = "", siteLanguage = _siteLanguage, err = "Error while initializing session !!!"});
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await _userManager.HasBeenVerified())
            {
                return View("Error");
            }
            var id = await _userManager.GetVerifiedUserIdAsync();
            var user = await _userManager.FindByIdAsync(id.Value);
            if (user != null)
            {
                // To exercise the flow without actually sending codes, uncomment the following line
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await _userManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userManager.TwoFactorSignIn(model.Provider, model.Code, false, model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, Request.Url == null ? "" : Request.Url.Scheme);
                    await _userManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int? userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(userId.Value, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Url == null ? "" : Request.Url.Scheme);
                await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }), _userManager);
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await _userManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(userId.Value);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            // Generate the token and send it
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await _userManager.SendTwoFactorCode(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _userManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await _userManager.ExternalSignIn(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _userManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await _userManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _userManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public int SetPrivacySettings(string prefs)
        {
            var cookies = JsonConvert.DeserializeObject<List<string>>(prefs);

            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            TNS.AdExpress.Web.Core.Sessions.WebSession session = (TNS.AdExpress.Web.Core.Sessions.WebSession)TNS.AdExpress.Web.Core.Sessions.WebSession.Load(idWS);

            var enableTracking = cookies.FirstOrDefault(s => s.Contains("Statistics"));
            var enableTroubleshooting = cookies.FirstOrDefault(s => s.Contains("Diagnostic"));
            int enableTrackingDb = 0;
            int enableTroubleshootingDb = 0;

            if (enableTracking != null)
            {
                session.EnableTracking = true;
                enableTrackingDb = 1;
            }
            else
                session.EnableTracking = false;

            if (enableTroubleshooting != null)
            {
                session.EnableTroubleshooting = true;
                enableTroubleshootingDb = 1;
            }
            else
                session.EnableTroubleshooting = false;

            session.Save();

            IDataSource Source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.rights);
            RightDAL.SetPrivacySettings(Source, session.CustomerLogin.IdLogin, enableTrackingDb, enableTroubleshootingDb);

            return 1;
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private void AddErrors(ApplicationIdentityResult result)
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

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri, IApplicationUserManager userManager)
                : this(provider, redirectUri, null, userManager)
            {
            }

            public ChallengeResult(string provider, string redirectUri, int? userId, IApplicationUserManager userManager)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
                UserManager = userManager;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public int? UserId { get; set; }
            public IApplicationUserManager UserManager { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                UserManager.Challenge(RedirectUri, XsrfKey, UserId, LoginProvider);
            }
        }
        #endregion

    }
}