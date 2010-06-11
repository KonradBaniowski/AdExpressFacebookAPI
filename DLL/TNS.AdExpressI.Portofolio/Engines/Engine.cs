#region Information
// Author: D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Globalization;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Web.Core.Utilities;

using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultsConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;


namespace TNS.AdExpressI.Portofolio.Engines {
	/// <summary>
	/// Define default behaviours of portofolio result like global process, lengendaries, properties...
	/// </summary>
	public abstract class Engine {

		#region Variables
		/// <summary>
		/// Customer session
		/// </summary>
		protected WebSession _webSession;
		/// <summary>
		/// Vehicle
		/// </summary>
		protected VehicleInformation _vehicleInformation;
		/// <summary>
		/// Media Id
		/// </summary>
		protected Int64 _idMedia;
		/// <summary>
		/// Date begin
		/// </summary>
		protected string _periodBeginning;
		/// <summary>
		/// Date end
		/// </summary>
		protected string _periodEnd;
		/// <summary>
		/// Current Module
		/// </summary>
		protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
		/// <summary>
		/// List of media to test for creative acces (press specific)
		/// </summary>
		protected List<long> _mediaList = null;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public Engine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd) {
			if (webSession == null) throw (new ArgumentNullException("Customer session is null"));
			_webSession = webSession;
			try {
				// Set Vehicle
				_vehicleInformation = vehicleInformation;
				//Set Media Id
				_idMedia = idMedia;
				// Period
				_periodBeginning = periodBeginning;
				_periodEnd = periodEnd;
				// Module
				_module = ModulesList.GetModule(webSession.CurrentModule);
			}
			catch (System.Exception err) {
				throw (new PortofolioException("Impossible to set parameters", err));
			}
		}

		#endregion

		#region Virtual Public methods
		/// <summary>
		/// Get result table
		/// </summary>
		/// <returns></returns>
		public virtual ResultTable GetResultTable() {
			return ComputeResultTable();
		}
		/// <summary>
		/// Get Html result
		/// </summary>
		/// <returns></returns>
		public virtual string GetHtmlResult() {
			return BuildHtmlResult();
        }

        #region Data for vehicle view
        /// <summary>
        /// Get data for vehicle view
        /// </summary>
        /// <param name="dtVisuel">Visuel information</param>
        /// <param name="htValue">investment values</param>
        /// <returns>Media name</returns>
        public string GetVehicleViewData(out DataTable dtVisuel, out Hashtable htValue) {

            string media = "";
            DataSet dsVisuel;

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            dsVisuel = portofolioDAL.GetListDate(true, DBCst.TableType.Type.dataVehicle);
            dtVisuel = dsVisuel.Tables[0];
            if (dtVisuel.Rows.Count > 0)
                media = dtVisuel.Rows[0]["media"].ToString();

            htValue = portofolioDAL.GetInvestmentByMedia();

            return media;
        }
        #endregion

		#region Get All Period Insertions
		/// <summary>
		/// Show the link for access to all insetion detail.
		/// </summary>
		/// <param name="t">String builder</param>
		/// <param name="linkText">Link text</param>
		/// <param name="webSession">session client</param>
		/// <param name="idMedia">Id media</param>
		public virtual void GetAllPeriodInsertions(StringBuilder t, string linkText) {
			string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");
			t.Append("<TR height=10><TD ><a class=\"roll03\" href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','','');\" ");
			t.Append(" onmouseover=\"detailSpotButton.src='/App_Themes/" + themeName + "/Images/Common/detailSpot_down.gif';\" onmouseout=\"detailSpotButton.src='/App_Themes/" + themeName + "/Images/Common/detailSpot_up.gif';\" ");
			t.Append("><IMG NAME=\"detailSpotButton\" src=\"/App_Themes/" + themeName + "/Images/Common/detailSpot_up.gif\" BORDER=0 align=absmiddle alt=\"" + linkText + "\">");
			t.Append("&nbsp;" + linkText);
			t.Append("</a></TD></TR>");
			t.Append("</table>");
			t.Append("<br><br>");
		}
		#endregion

		#endregion

		#region Internal methods
		/// <summary>
		/// Get result table
		/// </summary>
		/// <returns></returns>
		protected abstract ResultTable ComputeResultTable();

		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected abstract string BuildHtmlResult();
		#endregion

		#region Protected methods
		/// <summary>
		/// No data message
		/// </summary>
		/// <returns></returns>
		protected virtual string GetNoDataMessageHtml() {
			#region No data
			return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, _webSession.SiteLanguage)
				+ "</div>");
			#endregion
		}

		#endregion

	}
}
