using Kantar.AdExpress.Service.Core.BusinessService;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Models.Alert;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models.PortfolioAlert;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Alert.Domain;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    public class PortfolioAlertController : Controller
    {
        private IPortfolioAlertService _portfolioAlertService;

        public PortfolioAlertController(IPortfolioAlertService portfolioAlertService)
        {
            _portfolioAlertService = portfolioAlertService;
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

    }
}