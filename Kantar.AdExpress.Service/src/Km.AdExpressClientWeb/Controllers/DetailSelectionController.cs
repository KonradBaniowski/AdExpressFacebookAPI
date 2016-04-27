using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain;
using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models.DetailSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Domain = Kantar.AdExpress.Service.Core.Domain;

namespace Km.AdExpressClientWeb.Controllers
{
    public class DetailSelectionController : Controller
    {
        private IDetailSelectionService _detailSelectionService;
        public DetailSelectionController(IDetailSelectionService detailSelectionService)
        {
            _detailSelectionService = detailSelectionService;
        }

        public ActionResult GetDetailSelection()
        {
            var vm = new DetailSelectionViewModel();
            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _detailSelectionService.GetDetailSelection(idWS);
            vm.DetailSelectionWSModel = AutoMapper.Mapper.Map<DetailSelectionWSModel>(result);
            vm.Labels = LabelsHelper.LoadPageLabels(vm.DetailSelectionWSModel.SiteLanguage);

            return PartialView(vm);
        }

        public ActionResult LoadDetails(string id, string type)
        {
            var vm = new DetailSelectionViewModel();
            if (!String.IsNullOrEmpty(id))
            {
                DetailSelectionResponse response = new DetailSelectionResponse();
                var cp = new ClaimsPrincipal(User.Identity);
                var idWebSession = cp.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                var requestType  = (Domain.UniversType)Enum.Parse(typeof(Domain.UniversType), type);
                switch (requestType)
                {
                    case Domain.UniversType.Result:
                        response = _detailSelectionService.LoadSessionDetails(id, idWebSession);
                        break;
                    case Domain.UniversType.Univers:
                        response = _detailSelectionService.LoadUniversDetails(id, idWebSession);
                        break;
                    case Domain.UniversType.Alert:
                        response = _detailSelectionService.LoadAlertDetails(id, idWebSession);
                        break;
                    default:
                        vm.Message = "Not avaiable";
                        break;
                }
                
                vm.DetailSelectionWSModel = AutoMapper.Mapper.Map<DetailSelectionWSModel>(response);
                vm.Labels = LabelsHelper.LoadPageLabels(vm.DetailSelectionWSModel.SiteLanguage);
            }
            else
                vm.Message = "Invalid selection";
            return PartialView("GetDetailSelection", vm);
        }
    }
}