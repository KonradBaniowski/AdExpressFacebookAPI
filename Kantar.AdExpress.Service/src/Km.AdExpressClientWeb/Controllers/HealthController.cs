using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Constantes.Web;
using System.Data;
using System.Text;
using Km.AdExpressClientWeb.Models.Health;
using Newtonsoft.Json;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;


namespace Km.AdExpressClientWeb.Controllers
{
    public class HealthController : Controller
    {
        private IWebSessionService _webSessionService;
        private int _siteLanguage = WebApplicationParameters.DefaultLanguage;

        private const string _controller = "Health";

        public HealthController(IWebSessionService webSessionService)
        {
            _webSessionService = webSessionService;
        }

        // GET: Health
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Results()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _webSessionService.GetWebSession(idSession);
            _siteLanguage = result.WebSession.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            var resultNode = new NavigationNode { Position = 4 };
            var navigationHelper = new Helpers.PageHelper();
            var model = new Models.Shared.ResultsViewModel
            {
                NavigationBar = navigationHelper.LoadNavBar(idSession, _controller, _siteLanguage, 4),
                Presentation = navigationHelper.LoadPresentationBar(result.WebSession.SiteLanguage, result.ControllerDetails),
                Labels = LabelsHelper.LoadPageLabels(result.WebSession.SiteLanguage),
                ExportTypeViewModels = PageHelper.GetExportTypes(WebApplicationParameters.CountryCode, Module.Name.HEALTH, _siteLanguage)
            };

            return View(model);
        }

        public JsonResult HealthResult()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _webSessionService.GetWebSession(idWebSession);

            DataSet ds = new DataSet();
            JsonResult jsonModel;
            StringBuilder sql = new StringBuilder();
            List<DataCost> DataList = new List<DataCost>();

            string listVehicles = result.WebSession.GetSelection(result.WebSession.SelectionUniversMedia, CstRight.type.vehicleAccess);

            #region Query Building
            sql.Append(GetMasterQuery(result.WebSession.PeriodBeginningDate, result.WebSession.PeriodEndDate, listVehicles, result.WebSession));
            sql.AppendFormat("{0}", " ");

            #endregion

            #region Execution de la requête
            try
            {
                ds = result.WebSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new Exception("Unable to load Media Schedule Data : " + sql, err));
            }
            #endregion


            foreach (DataRow theRow in ds.Tables[0].Rows)
            {

                string date = string.Format("{0}/{1}/{2}", theRow["DATE_CANAL"].ToString().Substring(3, 2),
                 theRow["DATE_CANAL"].ToString().Substring(0, 2)
                , theRow["DATE_CANAL"].ToString().Substring(6, 4));
                DataList.Add(new DataCost
                {
                    IdCanal = Int64.Parse(theRow["ID_CANAL"].ToString()),
                    Canal = theRow["CANAL"].ToString(),
                    IdCategory = Int64.Parse(theRow["ID_CATEGORY"].ToString()),
                    Category = theRow["CATEGORY"].ToString(),
                    IdSpecialist = Int64.Parse(theRow["ID_MEDECIN"].ToString()),
                    Specialist = theRow["MEDECIN"].ToString(),
                    IdGrpPharma = Int64.Parse(theRow["ID_GRP_PHARMA"].ToString()),
                    GrpPharma = theRow["GRP_PHARMA"].ToString(),
                    IdLabratory = Int64.Parse(theRow["ID_LABORATOIRE"].ToString()),
                    Laboratory = theRow["LABORATOIRE"].ToString(),
                    IdProduct = Int64.Parse(theRow["ID_PRODUCT"].ToString()),
                    Product = theRow["PRODUCT"].ToString(),
                    IdFormat = Int64.Parse(theRow["ID_CONDITIONNEMENT"].ToString()),
                    Format = theRow["CONDITIONNEMENT"].ToString(),
                    Date = date,
                    Euro = Convert.ToDouble(theRow["euro"]),

                });

            }

            var data = JsonConvert.SerializeObject(DataList);

            var obj = new { datapivotgrid = data };
            jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;

            return jsonModel;
        }


        private string GetMasterQuery(string beginDate, string endDate, string medias, TNS.AdExpress.Web.Core.Sessions.WebSession webSession)
        {

            StringBuilder query = new StringBuilder();
            string dataTablePrefixe = "d";

            query.Append(@"select ca.ID_CANAL,CANAL, d.ID_CATEGORY
			, CATEGORY,me.ID_MEDECIN ,MEDECIN, gp.ID_GRP_PHARMA ,GRP_PHARMA,
              la.ID_LABORATOIRE,LABORATOIRE,
              po.ID_PRODUCT, PRODUCT
              ,cd.ID_CONDITIONNEMENT ,CONDITIONNEMENT
              ,DATE_CANAL,sum(EXPENDITURE) as euro


            from     KHEALTH01.CANAL ca, KHEALTH01.CATEGORY ct,
				            KHEALTH01.MEDECIN me, KHEALTH01.GRP_PHARMA  gp,
				            KHEALTH01.LABORATOIRE   la, KHEALTH01.PRODUCT po
				            ,KHEALTH01.DATA_COST d  ,KHEALTH01.CONDITIONNEMENT    cd

                            where
                            d.ID_CANAL  = ca.ID_CANAL
				            and d.ID_CATEGORY  = ct.ID_CATEGORY
				            and d.ID_MEDECIN = me.ID_MEDECIN
				            and d.ID_GRP_PHARMA = gp.ID_GRP_PHARMA
				            and d.ID_LABORATOIRE = la.ID_LABORATOIRE
				            and d.ID_PRODUCT = po.ID_PRODUCT	 
	                  and d.ID_CONDITIONNEMENT    =  cd.ID_CONDITIONNEMENT ");

            query.AppendFormat("and d.ID_CANAL in ({0}) and DATE_CANAL >= to_date({1},'YYYYMMDD') and DATE_CANAL <= to_date({2},'YYYYMMDD')  ", medias, beginDate + "01", endDate + "01");

            string doctors = webSession.PrincipalMediaUniverses[0].GetSqlConditions(dataTablePrefixe, true);
            query.Append(doctors);

            query.Append(@"group by  ca.ID_CANAL,CANAL, d.ID_CATEGORY , CATEGORY, me.ID_MEDECIN
					            ,MEDECIN, gp.ID_GRP_PHARMA ,GRP_PHARMA,
                                  la.ID_LABORATOIRE,LABORATOIRE, po.ID_PRODUCT, PRODUCT ,DATE_CANAL,
                                  cd.ID_CONDITIONNEMENT ,CONDITIONNEMENT
            ");


            return query.ToString();
        }

    }
}