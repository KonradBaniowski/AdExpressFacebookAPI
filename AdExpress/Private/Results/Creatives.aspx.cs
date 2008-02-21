using System;
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

namespace Private.Results {

    /// <summary>
    /// Creatives WebPage
    /// </summary>
    public partial class Creatives : TNS.AdExpress.Web.UI.WebPage {

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

            try {
                //Paramètres postés
                idsString = Page.Request.QueryString.Get("ids");
                ids = idsString.Split(',');
                zoomDate = Page.Request.QueryString.Get("zoomDate");
                string idUnivers = Page.Request.QueryString.Get("idUnivers");
                idModule = Int64.Parse(Page.Request.QueryString.Get("moduleId"));
                idVehicle = Page.Request.QueryString.Get("vehicleId");
                string page = Page.Request.QueryString.Get("page");

                //session
                this.CreativesWebControl1.WebSession = _webSession;

                //Period
                this.CreativesWebControl1.ZoomDate = (zoomDate != null) ? zoomDate : string.Empty;
                //Selection Ids
                this.CreativesWebControl1.IdsFilter = idsString;
                //Univers Id
                this.CreativesWebControl1.IdUnivers = (idUnivers != null && idUnivers.Length > 0) ? Convert.ToInt32(idUnivers) : -1;
                //Module Id
                this.CreativesWebControl1.IdModule = idModule;
                //Current vehicle
                this.CreativesWebControl1.IdVehicle = (idVehicle != null && idVehicle.Length > 0) ? Convert.ToInt32(idVehicle) : -1;
                //Current page
                this.CreativesWebControl1.PageIndex = (page != null && page.Length > 0) ? Convert.ToInt32(page) : 1;
                

                List<int> sizes = new List<int>();
                sizes.Add(10);
                sizes.Add(15);
                sizes.Add(20);
                this.CreativesWebControl1.PageSizeOption = sizes;
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
