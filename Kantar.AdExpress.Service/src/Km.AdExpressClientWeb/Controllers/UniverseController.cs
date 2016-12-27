using AutoMapper;
using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.Classification.Universe;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using WebCst = TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
    public class UniverseController : Controller
    {
        private IUniverseService _universeService;
        private IMyAdExpressService _myAdExpressService;
        private IDetailSelectionService _detailSelectionService;
        #region CONST
        private const string PORTFOLIO = "Portfolio";
        private const string LOSTWON = "LostWon";
        private const string PRESENTABSENT = "PresentAbsent";
        private const string MEDIASCHEDULE = "MediaSchedule";
        private const string ANALYSIS = "Analysis";
        private const string ADVERTISING_AGENCY = "AdvertisingAgency";
        private const string NEW_CREATIVES = "NewCreatives";
        private const string SOCIALMEDIA = "SocialMedia";
        private const string ANALYSE_DES_DISPOSITIFS = "CampaignAnalysis";
        private const string PROGRAM_ANALYSIS = "ProgramAnalysis";

        #endregion
        public UniverseController(IUniverseService universeService, IMyAdExpressService myAdExpressService, IDetailSelectionService detailSelectionService)
        {
            _universeService = universeService;
            _myAdExpressService = myAdExpressService;
            _detailSelectionService = detailSelectionService;
        }

        // GET: Universe
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// TEST : http://localhost:55658/Universe/GetBranches?keyWord=%27bmw%27&branchId=7
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public JsonResult GetUniverses(string keyWord, int universeId, Dimension dimension, List<int> idMedias = null)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var idSession = identity.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            int totalItems = 0;
            Domain.SearchRequest request = new Domain.SearchRequest(universeId, keyWord, idSession, dimension, idMedias);
            var model = _universeService.GetItems(request, out totalItems, this.HttpContext);
            return Json(new { data = model, total = totalItems }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// TEST : http://localhost:55658/Universe/GetUniverses?dimension=2
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public JsonResult GetModuleUniverses(Dimension dimension)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var idSession = identity.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var model = _universeService.GetUniverses(dimension, idSession, this.HttpContext);
            return Json( model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// TEST : http://localhost:55658/Universe/GetClassification?levelId=7&selectedClassification=%27295196%27&selectedLevelId=6
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="selectedClassification"></param>
        /// <param name="selectedLevelId"></param>
        /// <returns></returns>
        public JsonResult GetClassification(int levelId, string selectedClassification, int selectedLevelId, List<int> idMedias, Dimension dimension)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var idSession = identity.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            int totalItems = 0;
            var model = _universeService.GetItems(levelId, selectedClassification, selectedLevelId, idSession, dimension, idMedias, out totalItems, this.HttpContext);
            return Json(new { data = model, total = totalItems }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadUserUniversGroups(Dimension dimension)
        {
            bool showUserSavedGroups = true;
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            UserUniversGroupsModel result = new UserUniversGroupsModel
            {
                LoadUniversCode = LanguageConstantes.LoadUniversCode,
                ModuleCode = LanguageConstantes.MediaScheduleCode,
                SaveUniversCode = LanguageConstantes.SaveUniversCode,
                UserUniversGroups = new List<UserUniversGroup>(),
                UserUniversCode = LanguageConstantes.UserUniversCode,
                ErrorMsgCode = LanguageConstantes.NoSavedUniversCode,
                ModuleDecriptionCode = LanguageConstantes.MediaScheduleDescriptionCode,
                ShowUserSavedGroups = showUserSavedGroups
            };
            if (showUserSavedGroups)
            {
                var data = _universeService.GetUserSavedUniversGroups(webSessionId, dimension, this.HttpContext);
                result.SiteLanguage = data.SiteLanguage;
                result.UserUniversGroups = Mapper.Map<List<UserUniversGroup>>(data.UniversGroups);
                foreach (var group in result.UserUniversGroups)
                {
                    int count = group.Count;
                    group.FirstColumnSize = (count % 2 == 0) ? count / 2 : (count / 2) + 1;
                    group.SecondeColumnSize = count - group.FirstColumnSize;
                }
            }
            return PartialView("UserUniversGroupsContent", result);
        }

        public JsonResult GetUserUnivers(int id, Dimension dimension)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _universeService.GetTreesByUserUnivers(id, idWebSession, dimension, this.HttpContext);
            return Json(result);
        }

        [HttpGet]
        public PartialViewResult SaveUserUnivers(Dimension dimension)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _universeService.GetUnivers(webSessionId, WebCst.Universe.ALL_BRANCHES, string.Empty, this.HttpContext);
            SaveUserUniversViewModel model = new SaveUserUniversViewModel
            {
                Title = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, data.SiteLanguage),
                SelectUniversGroup = GestionWeb.GetWebWord(LanguageConstantes.SelectUniversGroup, data.SiteLanguage),
                SelectUnivers = GestionWeb.GetWebWord(LanguageConstantes.SelectUnivers, data.SiteLanguage),
                UniversLabel = GestionWeb.GetWebWord(LanguageConstantes.UniversLabel, data.SiteLanguage),
                UserGroups = new List<SelectListItem>(),
                UserUnivers = new List<SelectListItem>(),
                Submit = GestionWeb.GetWebWord(LanguageConstantes.Submit, data.SiteLanguage),
                CanSetDefaultUniverse = data.CanSetDefaultUniverse,
                DefaultUniverse = "Default Universe"
            };
            if (data.UniversGroups.Any())
            {
                List<SelectListItem> tmpUserUniverse = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "---" } };

                var items = data.UniversGroups.Select(p => new SelectListItem()
                {
                    Value = p.Id.ToString(),
                    Text = p.Description
                }).ToList();
                items.FirstOrDefault().Selected = true;
                model.UserGroups = items;
                if (data.UniversGroups.FirstOrDefault().UserUnivers.Any())
                {
                    tmpUserUniverse.AddRange(data.UniversGroups.FirstOrDefault().UserUnivers.Select(m => new SelectListItem()
                    {
                        Value = m.Id.ToString(),
                        Text = m.Description
                    }).ToList());

                    model.UserUnivers = tmpUserUniverse;
                    model.UserUnivers.FirstOrDefault().Selected = true;

                }
            }
            return PartialView(model);
        }
        [HttpGet]
        public JsonResult GetUniversByGroup(int id, Dimension dimension)
        {
            List<SelectListItem> univers = new List<SelectListItem>();
            if (id > 0)
            {
                //long groupId = long.Parse(id);
                var claim = new ClaimsPrincipal(User.Identity);
                string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                var data = _universeService.GetUserUniversGroups(webSessionId, dimension, this.HttpContext, id);
                univers = data.UniversGroups.FirstOrDefault().UserUnivers.Select(m => new SelectListItem()
                {
                    Value = m.Id.ToString(),
                    Text = m.Description
                }).ToList();
            }
            return Json(new SelectList(univers, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string SaveUserUnivers(List<Tree> trees, string groupId, string universId, string name, Dimension dimension, bool isDefaultUniverse, List<long> media = null)
        {
            string error = string.Empty;
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            List<Tree> validTrees = trees.Where(p => p.UniversLevels.Where(x => x.UniversItems != null).Any()).ToList();
            var data = Mapper.Map<List<Kantar.AdExpress.Service.Core.Domain.Tree>>(validTrees);
            Domain.UniversGroupSaveRequest request = new Domain.UniversGroupSaveRequest
            {
                Dimension = dimension,
                Name = name,
                UniversGroupId = !string.IsNullOrEmpty(groupId) ? long.Parse(groupId) : 0,
                UserUniversId = !string.IsNullOrEmpty(universId) ? long.Parse(universId) : 0,
                WebSessionId = webSessionId,
                Trees = Mapper.Map<List<Domain.Tree>>(validTrees),
                IdUniverseClientDescription = 16,
                MediaIds = (media != null) ? media : new List<long>(),
                IsDefaultUniverse =isDefaultUniverse
            };
            var result = _universeService.SaveUserUnivers(request, this.HttpContext);
            error = result.ErrorMessage;
            return error;
        }

        public JsonResult GetResultUnivers()
        {
            JsonResult result = new JsonResult();
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            return result;
        }
        [HttpPost]
        public JsonResult RenameSession(string name, string universId)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            result = _myAdExpressService.RenameSession(name, universId, webSessionId, this.HttpContext);
            return Json(result);
        }

        public JsonResult MoveSession(string idOldDirectory, string idNewDirectory, string id)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            result = _myAdExpressService.MoveSession(id, idOldDirectory, idNewDirectory, webSessionId, this.HttpContext);
            return Json(result);
        }

        public JsonResult RenameUnivers(string name, string universId)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            result = _myAdExpressService.RenameUnivers(name, universId, webSessionId, this.HttpContext);
            return Json(result);
        }

        public JsonResult MoveUnivers(string idOldDirectory, string idNewDirectory, string id)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            result = _myAdExpressService.MoveUnivers(id, idOldDirectory, idNewDirectory, webSessionId, this.HttpContext);
            return Json(result);
        }

        public JsonResult DeleteUnivers(string universId)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (!String.IsNullOrEmpty(webSessionId))
                result = _myAdExpressService.DeleteUnivers(universId, webSessionId, this.HttpContext);
            return Json(result);
        }

        public JsonResult DeleteSession(string universId)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (!String.IsNullOrEmpty(webSessionId))
                result = _myAdExpressService.DeleteSession(universId, webSessionId, this.HttpContext);
            return Json(result);
        }
        public JsonResult CreateDirectory(string directoryName, string type)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var universType = (type == "Session") ? Domain.UniversType.Result : Domain.UniversType.Univers;
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (!String.IsNullOrEmpty(webSessionId))
                result = _myAdExpressService.CreateDirectory(directoryName, universType, webSessionId, this.HttpContext);
            return Json(result);
        }

        public JsonResult RenameDirectory(string directoryName, string type, string idDirectory)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var universType = (type == "Session") ? Domain.UniversType.Result : Domain.UniversType.Univers;
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (!String.IsNullOrEmpty(webSessionId))
                result = _myAdExpressService.RenameDirectory(directoryName, universType, idDirectory, webSessionId, this.HttpContext);
            return Json(result);
        }
        public JsonResult DropDirectory(string idDirectory, string type)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var universType = (type == "Session") ? Domain.UniversType.Result : Domain.UniversType.Univers;
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (!String.IsNullOrEmpty(webSessionId))
                result = _myAdExpressService.DropDirectory(idDirectory, universType, webSessionId, this.HttpContext);
            return Json(result);
        }

        public PartialViewResult UserResult(string id)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _universeService.GetResultUnivers(webSessionId, this.HttpContext);
            SaveUserResultViewModel model = new SaveUserResultViewModel
            {
                Title = GestionWeb.GetWebWord(908, data.SiteLanguage),
                SelectFolder = GestionWeb.GetWebWord(702, data.SiteLanguage),
                SelectResult = GestionWeb.GetWebWord(2261, data.SiteLanguage),
                ResultLabel = GestionWeb.GetWebWord(2263, data.SiteLanguage),
                UserFolders = new List<SelectListItem>(),
                UserResults = new List<SelectListItem>()
            };
            if (data.UniversGroups.Any())
            {
                List<SelectListItem> tmpUserResults = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "---" } };

                var items = data.UniversGroups.Select(p => new SelectListItem()
                {
                    Value = p.Id.ToString(),
                    Text = p.Description
                }).ToList();

                if (id != "0")
                {
                    items.Where(i => i.Value == id).FirstOrDefault().Selected = true;
                    model.SelectedUserFolderId = Int32.Parse(id);
                }
                else
                    items.FirstOrDefault().Selected = true;

                model.UserFolders = items;
                if (id != "0")
                {
                    if (data.UniversGroups.Where(i => i.Id == Int64.Parse(id)).FirstOrDefault().UserUnivers.Any())
                    {
                        tmpUserResults.AddRange(data.UniversGroups.Where(i => i.Id == Int64.Parse(id)).FirstOrDefault().UserUnivers.Select(m => new SelectListItem()
                        {
                            Value = m.Id.ToString(),
                            Text = m.Description
                        }).ToList());

                        model.UserResults = tmpUserResults;

                        model.UserResults.FirstOrDefault().Selected = true;
                    }
                }
                else
                {
                    if (data.UniversGroups.FirstOrDefault().UserUnivers.Any())
                    {
                        tmpUserResults.AddRange(data.UniversGroups.FirstOrDefault().UserUnivers.Select(m => new SelectListItem()
                        {
                            Value = m.Id.ToString(),
                            Text = m.Description
                        }).ToList());

                        model.UserResults = tmpUserResults;

                        model.UserResults.FirstOrDefault().Selected = true;
                    }
                }
            }
            return PartialView(model);
        }

        public JsonResult SaveUserResult(string folderId, string saveAsResultId, string saveResult)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            string message = _universeService.SaveUserResult(webSessionId, folderId, saveAsResultId, saveResult, this.HttpContext);

            JsonResult jsonModel = new JsonResult();

            jsonModel = Json(new { Message = message });

            return jsonModel;
        }

        public JsonResult LoadSession(string idSession, string type)
        {
            var response = new Domain.AdExpressResponse
            {
                Message = string.Empty,
                RedirectUrl = string.Empty
            };
            var redirectUrl = string.Empty;
            var claim = new ClaimsPrincipal(User.Identity);
            //var requestedSessionType = (type.ToUpper() == "SESSION") ? Domain.UniversType.Result : Domain.UniversType.Alert;
            var requestType = (Domain.UniversType)Enum.Parse(typeof(Domain.UniversType), type);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (!String.IsNullOrEmpty(webSessionId))
                response = _myAdExpressService.LoadSession(idSession, requestType, webSessionId, this.HttpContext);
            if (response.Success && response.ModuleId > 0)
            {
                var controller = string.Empty;
                var action = "Results";
                switch (response.ModuleId)
                {
                    case Module.Name.ANALYSE_PLAN_MEDIA:
                        controller = MEDIASCHEDULE;
                        break;
                    case Module.Name.ANALYSE_PORTEFEUILLE:
                        controller = PORTFOLIO;
                        break;
                    case Module.Name.ANALYSE_CONCURENTIELLE:
                        controller = PRESENTABSENT;
                        break;
                    case Module.Name.ANALYSE_DYNAMIQUE:
                        controller = LOSTWON;
                        break;
                    case Module.Name.TABLEAU_DYNAMIQUE:
                        controller = ANALYSIS;
                        break;
                    case Module.Name.ANALYSE_MANDATAIRES:
                        controller = ADVERTISING_AGENCY;
                        break;
                    case Module.Name.NEW_CREATIVES:
                        controller = NEW_CREATIVES;
                        break;
                    case Module.Name.FACEBOOK:
                        controller = SOCIALMEDIA;
                        break;
                    case Module.Name.ANALYSE_DES_DISPOSITIFS:
                        controller = ANALYSE_DES_DISPOSITIFS;
                        break;
                    case Module.Name.ANALYSE_DES_PROGRAMMES:
                        controller = PROGRAM_ANALYSIS;
                        break;
                };
                response.Message = "Redirecting to the result page.";
                response.RedirectUrl = new UrlHelper(Request.RequestContext).Action(action, controller);
                //return Json(new { Url = redirectUrl });
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryItems( int idUniverse, Dimension dimension, List<int> idMedia)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var idWebSession = identity.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            int totalItems = 0;
            //List<int> id = new List<int> { idMedia };
            Domain.SearchItemsCriteria criteria = new Domain.SearchItemsCriteria(idWebSession, dimension, idUniverse, idMedia);
            var model = _universeService.GetGategoryItems(criteria, out totalItems, this.HttpContext);
            return Json(new { data = model, total = totalItems }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeMarketUniverse(long universeId)
        {   if(universeId > 0)
            {
                var identity = (ClaimsIdentity)User.Identity;
                var idWebSession = identity.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                _universeService.ChangeMarketUniverse(universeId, idWebSession, this.HttpContext);
            }          
            return new EmptyResult();
        }
    }
}