﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using DBClassifCst = TNS.AdExpress.Constantes.Classification;
using System.Text;

namespace Private.Results {


    /// <summary>
    /// Insertions
    /// </summary>
    public partial class Insertions : TNS.AdExpress.Web.UI.PrivateWebPage {

        #region Variables
        /// <summary>
        /// Code html de fermeture du flash d'attente
        /// </summary>
        public string divClose = LoadingSystem.GetHtmlCloseDiv();
        /// <summary>
        /// Liste des paramètres postés par le navigateur
        /// </summary>				
        private string[] ids = null;
        /// <summary>
        /// Liste des paramètres postés par le navigateur
        /// </summary>	
        private string idsString = "";
        /// <summary>
        /// Zoom Date
        /// </summary>
        private string zoomDate = "";
        /// <summary>
        /// Identifiant du média courant
        /// </summary>
        private string idVehicle = "-1";
        /// <summary>
        /// Identifiant du module courant
        /// </summary>
        private Int64 idModule = -1;
        private string idUnivers = "";
        #endregion

        #region Page_Load
        /// <summary>
        /// Loading Handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            try {

                #region Flash d'attente
                Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                Page.Response.Flush();
                #endregion

                MenuWebControl2.ForceHelp = WebCst.Links.HELP_FILE_PATH+"MediaInsertionsCreationsResultsHelp.aspx";
                MenuWebControl2.ForcePrint = string.Format("/Private/Results/Excel/Insertions.aspx?idSession={0}&zoomDate={1}&ids={2}&vehicleId={3}&idUnivers={4}&moduleId={5}",
                    _webSession.IdSession,
                    zoomDate,
                    idsString,
                    idVehicle,
                    idUnivers,
                    idModule);

                MenuWebControl2.DisplayHtmlPrint = true;


            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }

            }
        }
        #endregion

        #region Determine PostBack
        /// <summary>
        /// Determine PostBack Handling
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {

            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            MenuWebControl2.CustomerWebSession = _webSession;
            MenuWebControl2.ForbidHelpPages = true;


            string setParameters = string.Empty;

            try {
                //Paramètres postés
                idsString = Page.Request.QueryString.Get("ids");
                ids = idsString.Split(',');
                zoomDate = Page.Request.QueryString.Get("zoomDate");
                idUnivers = Page.Request.QueryString.Get("idUnivers");
                idModule = Int64.Parse(Page.Request.QueryString.Get("moduleId"));
                idVehicle = Page.Request.QueryString.Get("vehicleId");
                string page = Page.Request.QueryString.Get("page");

                #region Parameters
                if (Page.Request.Form.GetValues("zoomParam") != null && Page.Request.Form.GetValues("zoomParam")[0].Length > 0)
                {
                    zoomDate = Page.Request.Form.GetValues("zoomParam")[0];
                }
                zoomParam.Value = zoomDate;
                if (Page.Request.Form.GetValues("vehicleParam") != null && Page.Request.Form.GetValues("vehicleParam")[0].Length > 0)
                {
                    idVehicle = Page.Request.Form.GetValues("vehicleParam")[0];
                }
                vehicleParam.Value = idVehicle;

                StringBuilder js = new StringBuilder();
                js.Append("\r\n<script type=\"text/javascript\">");
                js.Append("\r\nfunction SetParameters(){");
                js.Append("\r\n\t\tdocument.getElementById(\"zoomParam\").value = resultParameters.Zoom;");
                js.Append("\r\n\t\tdocument.getElementById(\"vehicleParam\").value = resultParameters.IdVehicle;");
                js.Append("\r\n\t\t__doPostBack('', '');");
                js.Append("\r\n}");
                js.Append("\r\n</script>");
                setParameters = js.ToString();
                if (!this.ClientScript.IsClientScriptBlockRegistered("SetParameters")) this.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetParameters", setParameters);
                #endregion



                //session
                this.InsertionsWebControl1.WebSession = _webSession;
                this.InsertionsWebControl1.JavaScriptRefresh = "SetParameters";
                //Period
                this.InsertionsWebControl1.ZoomDate = (zoomDate != null) ? zoomDate : string.Empty;
                //Selection Ids
                this.InsertionsWebControl1.IdsFilter = idsString;
                //Univers Id
                this.InsertionsWebControl1.IdUnivers = (idUnivers != null && idUnivers.Length > 0) ? Convert.ToInt32(idUnivers) : -1;
                //Module Id
                this.InsertionsWebControl1.IdModule = idModule;
                //Current vehicle
                this.InsertionsWebControl1.IdVehicle = (idVehicle != null && idVehicle.Length > 0) ? Convert.ToInt32(idVehicle) : -1;
                //Current page
                this.InsertionsWebControl1.PageIndex = (page != null && page.Length > 0) ? Convert.ToInt32(page) : 1;


                string sizes = "10,15,20";
                this.InsertionsWebControl1.PageSizeOptions = sizes;
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
            return (tmp);
        }
        #endregion

    }

}