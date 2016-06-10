using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using CustomCst = TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Classification;
using DomainResults = TNS.AdExpress.Domain.Results;
using TNS.AdExpressI.Insertions;
using TNS.AdExpressI.Insertions.Cells;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Helper.MediaPlanVersions
{
    /// <summary>
	/// VersionsPluriMediaUI provide methods to get html code to display a set of version depending on the vehicles criterions
	/// </summary>
	public class VersionsPluriMediaUI
    {

        #region Variables
        /// <summary>
        /// Customer web session
        /// </summary>
        /// <author>gragneau</author>
        /// <since>jeudi 20 juillet 2006</since>
        private WebSession _webSession = null;
        /// <summary>
        /// Versions to display
        /// </summary>
        /// <author>gragneau</author>
        /// <since>jeudi 20 juillet 2006</since>
        private Dictionary<Int64, DomainResults.VersionItem> _versions = new Dictionary<long, DomainResults.VersionItem>();
        /// <summary>
        /// Considered Vehicles
        /// </summary>
        /// <author>gragneau</author>
        /// <since>jeudi 20 juillet 2006</since>
        private ArrayList _vehicles;
        /// <summary>
        /// Specifi wether plurimedia is allowed are not
        /// </summary>
        /// <author>gragneau</author>
        /// <since>jeudi 20 juillet 2006</since>
        public const bool ALLOW_PLURI = false;
        /// <summary>
        /// Période utilisée
        /// </summary>
        private MediaSchedulePeriod _period = null;
        /// <summary>
        /// Zoom Date
        /// </summary>
        private string _zoomDate = string.Empty;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Customer web session
        /// </summary>
        /// <author>gragneau</author>
        /// <since>jeudi 20 juillet 2006</since>
        public WebSession Session
        {
            get { return _webSession; }
            set { _webSession = value; }
        }
        ///<summary>Get / Set versions</summary>
        /// <author>gragneau</author>
        /// <since>jeudi 20 juillet 2006</since>
        public Dictionary<Int64, DomainResults.VersionItem> versions
        {
            get { return (_versions); }
            set { _versions = value; }
        }
        ///<summary>Get / Set La période sélectionnée</summary>
        /// <author>yrkaina</author>
        /// <since>jeudi 24 janvier 2008</since>
        public MediaSchedulePeriod Period
        {
            get { return (_period); }
            set { _period = value; }
        }
        ///<summary>Get / Set zoom date</summary>
        public string ZoomDate
        {
            get { return (_zoomDate); }
            set { _zoomDate = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="versions">List of verions details indexed by their Id</param>
        public VersionsPluriMediaUI(WebSession webSession, Dictionary<Int64, DomainResults.VersionItem> versions)
        {
            this._webSession = webSession;
            this._versions = versions;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="versions">List of verions details indexed by their Id</param>
        /// <param name="period">Période sélectionnée</param>
        public VersionsPluriMediaUI(WebSession webSession, Dictionary<Int64, DomainResults.VersionItem> versions, MediaSchedulePeriod period)
        {
            this._webSession = webSession;
            this._versions = versions;
            this._period = period;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="period">Période sélectionnée</param>
        /// <param name="zoomDate">Zoom date</param>
        public VersionsPluriMediaUI(WebSession webSession, MediaSchedulePeriod period, string zoomDate)
        {
            this._webSession = webSession;
            this._period = period;
            this._zoomDate = zoomDate;
        }
        #endregion

        #region Public Methods

        #region GetMSCreativesHtml
        /// <summary>
        /// Build Html code to display the set of version
        /// </summary>
        /// <returns>Html Code</returns>
        public string GetMSCreativesHtml()
        {

            StringBuilder htmlBld = new StringBuilder(10000);

            //listing of vehicle to display
            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CustomCst.Right.type.vehicleAccess).Split(',');
            _vehicles = new ArrayList();
            if (vehicles != null && (ALLOW_PLURI || (!ALLOW_PLURI && vehicles.Length < 2)))
            {
                foreach (string str in vehicles)
                {
                    if (WebApplicationParameters.MsCreativesDetail.ContainsVehicle(Int64.Parse(str)) && _webSession.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str))))
                    {
                        _vehicles.Add(new VersionsVehicleUI(_webSession, VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str)), Period, ZoomDate));
                    }
                }
            }

            //build components
            htmlBld.Append("<table cellspacing=\"0\" cellpadding=\"0\"  border=\"0\">");
            foreach (VersionsVehicleUI vUi in _vehicles)
            {
                htmlBld.Append("\r\n\t<tr>\r\n\t\t<td>");
                htmlBld.Append(vUi.GetMSCreativesHtml());
                htmlBld.Append("\r\n\t\t</td>\r\n\t</tr>");
            }
            htmlBld.Append("</table>");

            return htmlBld.ToString();
        }
        #endregion

        #region GetExportMSCreativesHtml
        /// <summary>
        /// Build Html code to display the set of version
        /// </summary>
        /// <param name="creativeCells">creativeCells</param>
        /// <param name="style">Style</param>
        /// <returns>Html Code</returns>
        public string GetExportMSCreativesHtml(ref SortedDictionary<Int64, List<CellCreativesInformation>> creativeCells, Style style)
        {

            StringBuilder htmlBld = new StringBuilder(10000);

            //listing of vehicle to display
            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CustomCst.Right.type.vehicleAccess).Split(',');
            _vehicles = new ArrayList();
            if (vehicles != null && (ALLOW_PLURI || (!ALLOW_PLURI && vehicles.Length < 2)))
            {
                foreach (string str in vehicles)
                {
                    if (WebApplicationParameters.MsCreativesDetail.ContainsVehicle(Int64.Parse(str)) && _webSession.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str))))
                    {
                        _vehicles.Add(new ExportVersionsVehicleUI(_webSession, VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str)), Period, ZoomDate));
                    }
                }
            }

            //build components
            htmlBld.Append("<table cellspacing=\"0\" cellpadding=\"0\"  border=\"0\">");
            foreach (ExportVersionsVehicleUI vUi in _vehicles)
            {
                htmlBld.Append("\r\n\t<tr>\r\n\t\t<td>");
                htmlBld.Append(vUi.GetMSCreativesHtml(ref creativeCells, style));
                htmlBld.Append("\r\n\t\t</td>\r\n\t</tr>");
            }
            htmlBld.Append("</table>");

            return htmlBld.ToString();
        }
        #endregion

        #region GetHtml
        /// <summary>
		/// Build Html code to display the set of version
		/// </summary>
		/// <returns>Html Code</returns>
		public string GetHtml()
        {

            StringBuilder htmlBld = new StringBuilder(10000);

            //listing of vehicle to display
            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CustomCst.Right.type.vehicleAccess).Split(',');
            _vehicles = new ArrayList();
            if (vehicles != null && (ALLOW_PLURI || (!ALLOW_PLURI && vehicles.Length < 2)))
            {
                foreach (string str in vehicles)
                {
                    if (WebApplicationParameters.MsCreativesDetail.ContainsVehicle(Int64.Parse(str)) && _webSession.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str))))
                    {
                        if (Period != null)
                            _vehicles.Add(new VersionsVehicleUI(_webSession, _versions, VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str)), Period));
                        else
                            _vehicles.Add(new VersionsVehicleUI(_webSession, _versions, VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str))));
                    }
                }
            }

            //build components
            htmlBld.Append("<table cellspacing=\"0\" cellpadding=\"0\"  border=\"0\">");
            foreach (VersionsVehicleUI vUi in _vehicles)
            {
                htmlBld.Append("\r\n\t<tr>\r\n\t\t<td>");
                htmlBld.Append(vUi.GetHtml());
                htmlBld.Append("\r\n\t\t</td>\r\n\t</tr>");
            }
            htmlBld.Append("</table>");

            return htmlBld.ToString();
        }
        #endregion

        #region GetHtmlExport
        /// <summary>
		/// Build Html code to display the set of version for the export UI
		/// </summary>
		/// <returns>Html Code</returns>
		public string GetHtmlExport(ref ArrayList versionsUIs)
        {

            StringBuilder htmlBld = new StringBuilder(10000);

            //listing of vehicle to display
            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CustomCst.Right.type.vehicleAccess).Split(',');
            _vehicles = new ArrayList();
            if (vehicles != null && (ALLOW_PLURI || (!ALLOW_PLURI && vehicles.Length < 2)))
            {
                foreach (string str in vehicles)
                {
                    if (WebApplicationParameters.MsCreativesDetail.ContainsVehicle(Int64.Parse(str)) && _webSession.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str))))
                    {
                        _vehicles.Add(new ExportVersionsVehicleUI(_webSession, _versions, VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str))));
                    }
                }
            }

            //build components
            htmlBld.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
            foreach (ExportVersionsVehicleUI vUi in _vehicles)
            {
                htmlBld.Append("\r\n\t<tr>\r\n\t\t<td>");
                htmlBld.Append(vUi.GetHtml(ref versionsUIs));
                htmlBld.Append("\r\n\t\t</td>\r\n\t</tr>");
            }
            htmlBld.Append("</table>");

            return htmlBld.ToString();
        }
        #endregion

        #region GetAPPMHtmlExport
        /// <summary>
		/// Build Html code to display the set of version for the export UI
		/// </summary>
		/// <returns>Html Code</returns>
		public ArrayList GetAPPMHtmlExport(IDataSource dataSource, String title, ref ArrayList versionsUIs)
        {
            ArrayList partitHTMLVersion = new ArrayList();

            //listing of vehicle to display
            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CustomCst.Right.type.vehicleAccess).Split(',');
            _vehicles = new ArrayList();
            if (vehicles != null && (ALLOW_PLURI || (!ALLOW_PLURI && vehicles.Length < 2)))
            {
                foreach (string str in vehicles)
                {
                    if (WebApplicationParameters.MsCreativesDetail.ContainsVehicle(Int64.Parse(str)) && _webSession.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str))))
                    {
                        _vehicles.Add(new ExportVersionsVehicleUI(_webSession, _versions, VehiclesInformation.DatabaseIdToEnum(Int64.Parse(str))));
                    }
                }
            }

            //build components
            foreach (ExportVersionsVehicleUI vUi in _vehicles)
            {
                partitHTMLVersion = vUi.GetAPPMHtml(dataSource, title, ref versionsUIs);
            }
            return partitHTMLVersion;
        }
        #endregion

        #endregion

    }
}
