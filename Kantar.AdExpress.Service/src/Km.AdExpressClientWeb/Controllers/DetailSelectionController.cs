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

            AutoMapper.Mapper.CreateMap<DetailSelectionResponse, DetailSelectionWSModel>();
            vm.DetailSelectionWSModel = AutoMapper.Mapper.Map<DetailSelectionWSModel>(result);
            vm.Labels = LabelsHelper.LoadPageLabels(vm.DetailSelectionWSModel.SiteLanguage);

            return PartialView(vm);
        }
    }
}