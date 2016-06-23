using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.Models;
using TNS.AdExpress.Domain.Results;
using Newtonsoft.Json;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class SocialMediaController : Controller
    {
        private IWebSessionService _webSessionService;
        private int _siteLanguage = 33;

        public SocialMediaController(IWebSessionService webSessionService)
        {
            _webSessionService = webSessionService;
        }

        //GET: SocialMedia
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
            var model = new Models.SocialMedia.ResultsViewModel
            {
                NavigationBar = navigationHelper.LoadNavBar(idSession, result.ControllerDetails.Name, _siteLanguage, 4),
                Presentation = navigationHelper.LoadPresentationBar(result.WebSession.SiteLanguage, result.ControllerDetails),
                Labels = navigationHelper.LoadPageLabels(_siteLanguage, result.ControllerDetails.Name)
            };

            return View(model);

        }


        public JsonResult SocialMediaResult()
        {
            //var gridResult = ServiceRomain();

            /************************** MOCK ***********************************/
            var gridResult = new GridResult();

            object[,] gridData = new object[11, 13]; 
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();

            //Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            columns.Add(new { headerText = "profilPage", key = "profilPage", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "profilPage" });

            columns.Add(new { headerText = "", key = "Title", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "Title" });
            columns.Add(new { headerText = "Lien vers les Post", key = "10", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "10" });
            columns.Add(new { headerText = "URL Page", key = "20", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "20" });
            columns.Add(new { headerText = "Page", key = "30", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "30" });
            columns.Add(new { headerText = "Fan", key = "40", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "40" });
            columns.Add(new { headerText = "Post", key = "50", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "50" });
            columns.Add(new { headerText = "Like", key = "60", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "60" });
            columns.Add(new { headerText = "Comment", key = "70", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "70" });
            columns.Add(new { headerText = "Share", key = "80", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "80" });
            columns.Add(new { headerText = "Brand exposure", key = "90", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "90" });

            gridData[0, 0] = 1;
            gridData[0, 1] = -1;
            gridData[0, 2] = 0;
            gridData[0, 3] = "Référents";
            gridData[0, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
            gridData[0, 5] = "";
            gridData[0, 6] = "2";
            gridData[0, 7] = "100";
            gridData[0, 8] = "100";
            gridData[0, 9] = "100";
            gridData[0, 10] = "300";
            gridData[0, 11] = "20";
            gridData[0, 12] = "4500";

                gridData[1, 0] = 2;
                gridData[1, 1] = 1;
                gridData[1, 2] = 0;
                gridData[1, 3] = "BMW";
                gridData[1, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                gridData[1, 5] = "";
                gridData[1, 6] = "2";
                gridData[1, 7] = "100";
                gridData[1, 8] = "100";
                gridData[1, 9] = "100";
                gridData[1, 10] = "300";
                gridData[1, 11] = "20";
                gridData[1, 12] = "4500";

                    gridData[2, 0] = 3;
                    gridData[2, 1] = 2;
                    gridData[2, 2] = 11;
                    gridData[2, 3] = "WooowWw";
                    gridData[2, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[2, 5] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[2, 6] = "1";
                    gridData[2, 7] = "50";
                    gridData[2, 8] = "50";
                    gridData[2, 9] = "50";
                    gridData[2, 10] = "150";
                    gridData[2, 11] = "10";
                    gridData[2, 12] = "";

                    gridData[3, 0] = 4;
                    gridData[3, 1] = 2;
                    gridData[3, 2] = 22;
                    gridData[3, 3] = "WooowWw";
                    gridData[3, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[3, 5] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[3, 6] = "1";
                    gridData[3, 7] = "50";
                    gridData[3, 8] = "50";
                    gridData[3, 9] = "50";
                    gridData[3, 10] = "150";
                    gridData[3, 11] = "10";
                    gridData[3, 12] = "";

            gridData[4, 0] = 5;
            gridData[4, 1] = -1;
            gridData[4, 2] = 0;
            gridData[4, 3] = "Concurrents";
            gridData[4, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
            gridData[4, 5] = "";
            gridData[4, 6] = "4";
            gridData[4, 7] = "300";
            gridData[4, 8] = "300";
            gridData[4, 9] = "300";
            gridData[4, 10] = "300";
            gridData[4, 11] = "60";
            gridData[4, 12] = "9000";

                gridData[5, 0] = 6;
                gridData[5, 1] = 5;
                gridData[5, 2] = 0;
                gridData[5, 3] = "AUDI";
                gridData[5, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                gridData[5, 5] = "";
                gridData[5, 6] = "3";
                gridData[5, 7] = "150";
                gridData[5, 8] = "150";
                gridData[5, 9] = "150";
                gridData[5, 10] = "400";
                gridData[5, 11] = "30";
                gridData[5, 12] = "4500";

                    gridData[6, 0] = 7;
                    gridData[6, 1] = 6;
                    gridData[6, 2] = 33;
                    gridData[6, 3] = "Yeahhhh";
                    gridData[6, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[6, 5] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[6, 6] = "1";
                    gridData[6, 7] = "50";
                    gridData[6, 8] = "50";
                    gridData[6, 9] = "50";
                    gridData[6, 10] = "150";
                    gridData[6, 11] = "10";
                    gridData[6, 12] = "";

                    gridData[7, 0] = 8;
                    gridData[7, 1] = 6;
                    gridData[7, 2] = 44;
                    gridData[7, 3] = "Yeahhhh";
                    gridData[7, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[7, 5] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[7, 6] = "1";
                    gridData[7, 7] = "50";
                    gridData[7, 8] = "50";
                    gridData[7, 9] = "50";
                    gridData[7, 10] = "150";
                    gridData[7, 11] = "10";
                    gridData[7, 12] = "";

                    gridData[8, 0] = 9;
                    gridData[8, 1] = 6;
                    gridData[8, 2] = 55;
                    gridData[8, 3] = "Yeahhhh";
                    gridData[8, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[8, 5] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[8, 6] = "1";
                    gridData[8, 7] = "50";
                    gridData[8, 8] = "50";
                    gridData[8, 9] = "50";
                    gridData[8, 10] = "100";
                    gridData[8, 11] = "10";
                    gridData[8, 12] = "";

                gridData[9, 0] = 10;
                gridData[9, 1] = 5;
                gridData[9, 2] = 0;
                gridData[9, 3] = "VOLKSWAGEN";
                gridData[9, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                gridData[9, 5] = "";
                gridData[9, 6] = "2";
                gridData[9, 7] = "150";
                gridData[9, 8] = "150";
                gridData[9, 9] = "150";
                gridData[9, 10] = "400";
                gridData[9, 11] = "30";
                gridData[9, 12] = "4500";

                    gridData[10, 0] = 11;
                    gridData[10, 1] = 10;
                    gridData[10, 2] = 66;
                    gridData[10, 3] = "ChoooOou";
                    gridData[10, 4] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[10, 5] = "<center><a href='#'><span class='fa fa-search-plus'></a></center>";
                    gridData[10, 6] = "1";
                    gridData[10, 7] = "50";
                    gridData[10, 8] = "50";
                    gridData[10, 9] = "50";
                    gridData[10, 10] = "150";
                    gridData[10, 11] = "10";
                    gridData[10, 12] = "";


            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            gridResult.ColumnsFixed = columnsFixed;
            gridResult.NeedFixedColumns = false;
            gridResult.Data = gridData;

            /************************ FIN MOCK ***********************************/

            try
            {
                if (!gridResult.HasData)
                    return null;

                string jsonData = JsonConvert.SerializeObject(gridResult.Data);
                var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, needfixedcolumns = gridResult.NeedFixedColumns, isonecolumnline = gridResult.isOneColumnLine };
                JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}