using System.Linq;
using System.Security.Claims;
using Kantar.AdExpress.Service.Core.BusinessService;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using System.Threading.Tasks;
using System.Web.Mvc;
using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models.PortfolioAlert;

namespace Km.AdExpressClientWeb.Controllers
{
    public class PortfolioAlertController : Controller
    {
        private IPortfolioAlertService _portfolioAlertService;
        private IGadService _gadService;

        public PortfolioAlertController(IPortfolioAlertService portfolioAlertService, IGadService gadService)
        {
            _portfolioAlertService = portfolioAlertService;
            _gadService = gadService;
        }

        public async Task<ActionResult> Index(long alertId, long alertTypeId, string dateMediaNum, int idLanguage = 33)
        {

            PortfolioAlertViewModel model = new PortfolioAlertViewModel();

            Domain.PortfolioAlertResultResponse portfolioAlert = _portfolioAlertService.GetPortfolioAlertResult(alertId, alertTypeId, dateMediaNum, idLanguage);

            model.IdLanguage = idLanguage;
            model.AlertDatas = portfolioAlert;
            model.Labels  = LabelsHelper.LoadPageLabels(idLanguage);

            return View(model);
        }

        public ActionResult DocInfos(string ida,string aa)
        {
            //ida = idAdress
            //aa = advertiser
            Domain.Gad gadInfos = _gadService.GetGadInfos(ida, aa, this.HttpContext);

            return View(gadInfos);
        }

    }
}