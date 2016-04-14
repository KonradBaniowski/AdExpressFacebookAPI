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

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class UniverseController : Controller
    {
        private IUniverseService _universeService;
        private IMyAdExpressService _myAdExpressService;

        public UniverseController(IUniverseService universeService, IMyAdExpressService myAdExpressService)
        {
            _universeService = universeService;
            _myAdExpressService = myAdExpressService;
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
            var model = _universeService.GetItems(universeId, keyWord, idSession, dimension, idMedias, out totalItems);
            return Json(new { data = model, total = totalItems } , JsonRequestBehavior.AllowGet);
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
            var model = _universeService.GetItems(levelId, selectedClassification,selectedLevelId, idSession, dimension, idMedias, out totalItems);
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
                var data = _universeService.GetUserSavedUniversGroups(webSessionId, dimension);
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
            var result = _universeService.GetTreesByUserUnivers(id, idWebSession, dimension);
            return Json(result);
        }

        [HttpGet]
        public PartialViewResult SaveUserUnivers(Dimension dimension)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _universeService.GetUserUniversGroups(webSessionId, dimension);
            SaveUserUniversViewModel model = new SaveUserUniversViewModel
            {
                Title = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, data.SiteLanguage),
                SelectUniversGroup = GestionWeb.GetWebWord(LanguageConstantes.SelectUniversGroup, data.SiteLanguage),
                SelectUnivers = GestionWeb.GetWebWord(LanguageConstantes.SelectUnivers, data.SiteLanguage),
                UniversLabel = GestionWeb.GetWebWord(LanguageConstantes.UniversLabel, data.SiteLanguage),
                UserGroups = new List<SelectListItem>(),
                UserUnivers = new List<SelectListItem>()
            };
            if (data.UniversGroups.Any())
            {
                var items = data.UniversGroups.Select(p => new SelectListItem()
                {
                    Value = p.Id.ToString(),
                    Text = p.Description
                }).ToList();
                items.FirstOrDefault().Selected = true;
                model.UserGroups = items;
                if (data.UniversGroups.FirstOrDefault().UserUnivers.Any())
                {
                    model.UserUnivers = data.UniversGroups.FirstOrDefault().UserUnivers.Select(m => new SelectListItem()
                    {
                        Value = m.Id.ToString(),
                        Text = m.Description
                    }).ToList();
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
                var data = _universeService.GetUserUniversGroups(webSessionId, dimension, id);
                univers = data.UniversGroups.FirstOrDefault().UserUnivers.Select(m => new SelectListItem()
                {
                    Value = m.Id.ToString(),
                    Text = m.Description
                }).ToList();
            }
            return Json(new SelectList(univers, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string SaveUserUnivers(List<Tree> trees, string groupId, string universId, string name, Dimension dimension, List<long> media = null)
        {
            string error = "";
            if (trees.Any() && trees.Where(p => p.UniversLevels != null).Any() && !String.IsNullOrEmpty(groupId) && (!String.IsNullOrEmpty(universId) || !String.IsNullOrEmpty(name)))
            {
                var claim = new ClaimsPrincipal(User.Identity);
                string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                List<Tree> validTrees = trees.Where(p => p.UniversLevels.Where(x => x.UniversItems != null).Any()).ToList();
                var data = Mapper.Map<List<Kantar.AdExpress.Service.Core.Domain.Tree>>(validTrees);
                Domain.UniversGroupSaveRequest request = new Domain.UniversGroupSaveRequest
                {
                    Dimension = dimension,
                    Name = name,
                    UniversGroupId = long.Parse(groupId),
                    UserUniversId = long.Parse(universId),
                    WebSessionId = webSessionId,
                    Trees = Mapper.Map<List<Domain.Tree>>(validTrees),
                    IdUniverseClientDescription = 16,
                    MediaIds = (media != null) ? media : new List<long>()
                };
                var result = _universeService.SaveUserUnivers(request);
                error = result.ErrorMessage;
            }
            else
            {
                error = "Invalid Selection";
            }
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
                Message=string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            result = _myAdExpressService.RenameSession(name, universId, webSessionId);
            return Json(result);
        }

        public JsonResult MoveSession(string idOldDirectory, string idNewDirectory,string id)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            result = _myAdExpressService.MoveSession(id,idOldDirectory, idNewDirectory, webSessionId);
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
            result = _myAdExpressService.RenameUnivers(name, universId, webSessionId);
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
            result = _myAdExpressService.MoveUnivers(id, idOldDirectory, idNewDirectory, webSessionId);
            return Json(result);
        }

        public JsonResult DeleteUnivers( string universId)
        {
            Domain.AdExpressResponse result = new Domain.AdExpressResponse
            {
                Message = string.Empty
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if(!String.IsNullOrEmpty(webSessionId))
                result = _myAdExpressService.DeleteUnivers(universId, webSessionId);
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
                result = _myAdExpressService.DeleteSession(universId, webSessionId);
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
                result = _myAdExpressService.CreateDirectory(directoryName, universType, webSessionId);
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
                result = _myAdExpressService.RenameDirectory(directoryName, universType, idDirectory, webSessionId);
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
                result = _myAdExpressService.DropDirectory(idDirectory, universType, webSessionId);
            return Json(result);
        }
    }
}