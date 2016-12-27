using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    public class CreativeMediaScheduleController : Controller
    {
        private IWebSessionService _webSessionService;
        private IApplicationUserManager _userManager;
        private int _siteLanguage = WebApplicationParameters.DefaultLanguage;
        private IMediaScheduleService _mediaSchedule;
        private const string _controller = "CreativeMediaSchedule";

        public CreativeMediaScheduleController( IWebSessionService webSessionService, IApplicationUserManager userManager, IMediaScheduleService mediaSchedule)
        {
            _webSessionService = webSessionService;
            _userManager = userManager;
            _mediaSchedule = mediaSchedule;
        }

        // GET: CreativeMediaSchedule
        //public async Task<ActionResult> Index(string siteLanguage, string mediaTypeIds, string beginDate, string endDate, string productIds, string creativeIds)
        public async Task<ActionResult> Index(string k,string l, string m, string b, string e, string p, string c)
        {
            string siteLanguage = string.Empty, mediaTypeIds = string.Empty, beginDate = string.Empty, endDate = string.Empty, productIds = string.Empty, creativeIds = string.Empty;
            const string LOGIN = WebConstantes.CreativeMSAccount.LOGIN;
            const string PASSWORD = WebConstantes.CreativeMSAccount.PASSWORD;

            ViewBag.SiteLanguage = Convert.ToInt32(WebConstantes.CountryCode.FRANCE);

            var authenticate = await _userManager.PasswordSignIn(LOGIN, PASSWORD, false, shouldLockout: false);
            
            switch (authenticate)
            {
                case SignInStatus.Success:
                    var pageHelper = new Helpers.PageHelper();
                    var model = new Models.MediaSchedule.CreativeMediaScheduleResultsViewModel();


                    model.Labels = pageHelper.LoadPageLabels(_siteLanguage, _controller);
                    model.ErrorMessages = new List<string>();
                    try
                    {
                        string key = SecurityHelper.Decrypt(k, SecurityHelper.CryptKey);

                        if (key != "adscope-" + DateTime.Now.ToString("yyyyMMdd"))
                            model.ErrorMessages.Add("Erreur : Le paramètre k est invalide");
                    }
                    catch (Exception ex)
                    {
                        model.ErrorMessages.Add("Erreur lors du traitement du paramètre k : " + ex.Message);
                    }

                    if (string.IsNullOrEmpty(l))
                        model.ErrorMessages.Add("Erreur : Le paramètre l est invalide");
                    else
                    {
                        try
                        {
                            siteLanguage = SecurityHelper.Decrypt(l, SecurityHelper.CryptKey);
                            int siteLang = Convert.ToInt32(siteLanguage);
                            if (siteLang != WebApplicationParameters.DefaultLanguage)
                                model.ErrorMessages.Add("Erreur : Le paramètre l est invalide");
                        }
                        catch(Exception ex)
                        {
                            model.ErrorMessages.Add("Erreur lors du traitement du paramètre l : " + ex.Message);
                        }
                    }

                    if (string.IsNullOrEmpty(m))
                        model.ErrorMessages.Add("Erreur : Le paramètre m est invalide");
                    else
                    {
                        try
                        {
                            mediaTypeIds = SecurityHelper.Decrypt(m, SecurityHelper.CryptKey);
                        }
                        catch (Exception ex)
                        {
                            model.ErrorMessages.Add("Erreur lors du traitement du paramètre m : " + ex.Message);
                        }
                    }

                    if (string.IsNullOrEmpty(p))
                        model.ErrorMessages.Add("Erreur : Le  paramètre p est invalide");
                    else
                    {
                        try
                        {
                            productIds = SecurityHelper.Decrypt(p, SecurityHelper.CryptKey);
                        }
                        catch (Exception ex)
                        {
                            model.ErrorMessages.Add("Erreur lors du traitement du paramètre p : " + ex.Message);
                        }
                    }

                    if (string.IsNullOrEmpty(b))
                        model.ErrorMessages.Add("Erreur : Le champ b est invalide");
                    else
                    {
                        try
                        {
                            beginDate = SecurityHelper.Decrypt(b, SecurityHelper.CryptKey);
                            if (beginDate.Length != 8)
                                model.ErrorMessages.Add("Erreur : Le champ b est invalide");
                        }
                        catch (Exception ex)
                        {
                            model.ErrorMessages.Add("Erreur lors du traitement du paramètre b : " + ex.Message);
                        }
                    }

                    if (string.IsNullOrEmpty(e))
                        model.ErrorMessages.Add("Erreur : Le champ e est invalide");
                    else
                    {
                        try
                        {
                            endDate = SecurityHelper.Decrypt(e, SecurityHelper.CryptKey);
                            if (endDate.Length != 8)
                                model.ErrorMessages.Add("Erreur : Le champ e est invalide");
                        }
                        catch (Exception ex)
                        {
                            model.ErrorMessages.Add("Erreur lors du traitement du paramètre e : " + ex.Message);
                        }
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(c))
                            creativeIds = SecurityHelper.Decrypt(c, SecurityHelper.CryptKey);
                    }
                    catch (Exception ex)
                    {
                        model.ErrorMessages.Add("Erreur lors du traitement du paramètre c : " + ex.Message);
                    }

                    if (model.ErrorMessages.Count > 0)
                        return View(model);

                    
                    model.SiteLanguage = l;
                    model.EndDate = e;
                    model.BeginDate = b;
                    model.ProductIds = p;
                    model.CreativeIds = c;
                    model.MediaTypeIds = m;

                    return View(model);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View("Home");
            }
        }

        [Authorize(Roles = WebConstantes.Role.ADSCOPE)]
        public JsonResult Results(string l,string m,string b, string e ,string p,string c)
        {
            GridResultResponse creativeMediaScheduleResponse;
            GridResult gridResult;
            JsonResult jsonModel;
            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(i => i.Type == ClaimTypes.UserData).Select(s => s.Value).SingleOrDefault();

            int siteLanguage = Int32.Parse(SecurityHelper.Decrypt(l, SecurityHelper.CryptKey));
            string mediaTypeIds = SecurityHelper.Decrypt(m, SecurityHelper.CryptKey);
            int beginDate = Int32.Parse(SecurityHelper.Decrypt(b, SecurityHelper.CryptKey));
            int endDate = Int32.Parse(SecurityHelper.Decrypt(e, SecurityHelper.CryptKey));
            string productIds = SecurityHelper.Decrypt(p, SecurityHelper.CryptKey);
            string creativeIds = string.Empty;

            if(!string.IsNullOrEmpty(c))
                creativeIds = SecurityHelper.Decrypt(c, SecurityHelper.CryptKey);

            CreativeMediaScheduleRequest request = new CreativeMediaScheduleRequest { IdWebSession = idWS, SiteLanguage = siteLanguage, MediaTypeIds = mediaTypeIds, BeginDate = beginDate, EndDate = endDate, ProductIds = productIds, CreativeIds = creativeIds };

            try
            {
                creativeMediaScheduleResponse = _mediaSchedule.GetGridResult(request);
            }
            catch(Exception ex)
            {
                var response = new { success = false, errorMessage = ex.Message };
                jsonModel = Json(response, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }

            if (creativeMediaScheduleResponse.Success)
            {
                gridResult = creativeMediaScheduleResponse.GridResult;

                if (!gridResult.HasData)
                    return null;

                if (gridResult.HasMoreThanMaxRowsAllowed)
                {
                    var response = new { success = true, hasMoreThanMaxRowsAllowed = true };
                    jsonModel = Json(response, JsonRequestBehavior.AllowGet);
                    jsonModel.MaxJsonLength = Int32.MaxValue;

                    return jsonModel;
                }

                string jsonData = JsonConvert.SerializeObject(gridResult.Data);

                var obj = new { success = true, datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, columnsNotAllowedSorting = gridResult.ColumnsNotAllowedSorting, needfixedcolumns = gridResult.NeedFixedColumns, hasMSCreatives = gridResult.HasMSCreatives, unit = gridResult.Unit };
                jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;


                return jsonModel;
            }
            else
            {
                var response = new { success = false, errorMessage = creativeMediaScheduleResponse.Message };
                jsonModel = Json(response, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }
        }
    }
}