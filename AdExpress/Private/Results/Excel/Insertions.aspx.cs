using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Private.Results.Excel
{
    public partial class Insertions : TNS.AdExpress.Web.UI.ExcelWebPage
    {
        #region Chargement
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                Response.ContentType = "application/vnd.ms-excel";

            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }

        }
        #endregion

        #region DeterminePostBack
        /// <summary>
		/// Evaluation de l'évènement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns>PostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
		

            try
            {
                ResultWebControl1.CustomerWebSession = _webSession;

                //Paramètres postés
                string idsString = Page.Request.QueryString.Get("ids");
                string[] ids = idsString.Split(',');
                string zoomDate = Page.Request.QueryString.Get("zoomDate");
                string idUnivers = Page.Request.QueryString.Get("idUnivers");
                Int64 idModule = Int64.Parse(Page.Request.QueryString.Get("moduleId"));
                string idVehicle = Page.Request.QueryString.Get("vehicleId");

                //Period
                this.ResultWebControl1.ZoomDate = (zoomDate != null) ? zoomDate : string.Empty;
                //Selection Ids
                this.ResultWebControl1.IdsFilter = idsString;
                //Univers Id
                this.ResultWebControl1.IdUnivers = (idUnivers != null && idUnivers.Length > 0) ? Convert.ToInt32(idUnivers) : -1;
                //Module Id
                this.ResultWebControl1.IdModule = idModule;
                //Current vehicle
                this.ResultWebControl1.IdVehicle = (idVehicle != null && idVehicle.Length > 0) ? Convert.ToInt32(idVehicle) : -1;

            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }

            return tmp;
		}
		#endregion

    }
}