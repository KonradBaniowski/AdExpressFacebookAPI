﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Selection;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using WeBCst = TNS.AdExpress.Constantes.Web;
using CustomCst = TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.Insertions;
using TNS.AdExpressI.Insertions.Cells;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Helper.DataAccess;
using TNS.AdExpress.Web.Helper.Functions;
using TNS.AdExpress.Web.Helper.Exceptions;

namespace TNS.AdExpress.Web.Helper.MediaPlanVersions
{
    /// <summary>
    /// VersionsVehicleUI provide methods to get html code to display a set of version all from the same vehicle
    /// </summary>
    public class VersionsVehicleUI
    {

        #region Variables
        /// <summary>
        /// WebControl title
        /// </summary>
        private string _title = string.Empty;
        ///<summary>List of Versions</summary>
        /// <author>gragneau</author>
        /// <since>jeudi 13 juillet 2006</since>
        private Dictionary<Int64, VersionItem> _versions = new Dictionary<Int64, VersionItem>();
        /// <summary>
        /// Media Classification considered in the web control
        /// </summary>
        ///<directed>True</directed>
        private DBCst.Vehicles.names _vehicle;
        /// <summary>
        /// Customer web session
        /// </summary>
        private WebSession _webSession = null;
        /// <summary>
        /// Number of versions on a line
        /// </summary>
        private int _nb_column = 10;
        /// <summary>
        /// Object genberating html code
        /// </summary>
        private ArrayList _versionsUIs;
        /// <summary>
        /// Période utilisée
        /// </summary>
        private MediaSchedulePeriod _period = null;
        /// <summary>
        /// Zoom date
        /// </summary>
        private string _zoomDate = string.Empty;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Customer web session
        /// </summary>
        public WebSession Session
        {
            get { return _webSession; }
            set { _webSession = value; }
        }
        /// <summary>
        /// Get / Set Considered Vehicle
        /// </summary>
        public DBCst.Vehicles.names Vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }
        /// <summary>
        /// Get / Set Web Control title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// Get / Set Columns number
        /// </summary>
        public int Nb_Columns
        {
            get { return _nb_column; }
            set { _nb_column = value; }
        }
        ///<summary>Get / Set versions</summary>
        /// <author>gragneau</author>
        /// <since>jeudi 13 juillet 2006</since>
        public Dictionary<Int64, VersionItem> versions
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
        ///<summary>Get / Set Zoom date</summary>
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
        /// <param name="vehicle">Vehicle considered</param>
        public VersionsVehicleUI(WebSession webSession, Dictionary<Int64, VersionItem> versions, DBCst.Vehicles.names vehicle)
        {
            this._webSession = webSession;
            this._versions = versions;
            this._vehicle = vehicle;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="versions">List of verions details indexed by their Id</param>
        /// <param name="vehicle">Vehicle considered</param>
        /// <param name="period">Période utilisée</param>
        public VersionsVehicleUI(WebSession webSession, Dictionary<Int64, VersionItem> versions, DBCst.Vehicles.names vehicle, MediaSchedulePeriod period)
        {
            this._webSession = webSession;
            this._versions = versions;
            this._vehicle = vehicle;
            this._period = period;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicle">Vehicle considered</param>
        /// <param name="period">Période utilisée</param>
        /// <param name="zoomDate">Zoom date</param>
        public VersionsVehicleUI(WebSession webSession, DBCst.Vehicles.names vehicle, MediaSchedulePeriod period, string zoomDate)
        {
            this._webSession = webSession;
            this._vehicle = vehicle;
            this._period = period;
            this._zoomDate = zoomDate;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Build Html code to display the set of version
        /// </summary>
        /// <returns>Html Code</returns>
        public string GetHtml()
        {
            SetUp();
            StringBuilder htmlBld = new StringBuilder(10000);
            BuildHtml(htmlBld);
            return htmlBld.ToString();
        }
        /// <summary>
        /// Build Html code to display the set of version
        /// </summary>
        /// <returns>Html Code</returns>
        public string GetMSCreativesHtml()
        {

            #region MSCreatives
            object[] paramMSCraetives = new object[2];
            paramMSCraetives[0] = _webSession;
            paramMSCraetives[1] = _webSession.CurrentModule;

            CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[Constantes.Web.Layers.Id.insertions];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions rules"));
            var resultMSCreatives = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance
                | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, paramMSCraetives, null, null);

            ResultTable data = null;
            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CustomCst.Right.type.vehicleAccess).Split(',');
            string filters = string.Empty;
            int fromDate = Convert.ToInt32(_period.Begin.ToString("yyyyMMdd"));
            int toDate = Convert.ToInt32(_period.End.ToString("yyyyMMdd"));
            #endregion

            data = resultMSCreatives.GetMSCreatives(VehiclesInformation.Get(Int64.Parse(vehicles[0])), fromDate, toDate, filters, -1, _zoomDate);

            var htmlBld = new StringBuilder(10000);
            BuildMSCreativesHtml(htmlBld, data);
            return htmlBld.ToString();
        }
        #endregion

        #region Protected Methods

        #region SetUp
        /// <summary>
        /// Initialise all webcontrols
        /// </summary>
        protected void SetUp()
        {

            #region Get Data from persistent layer
            //TODO Get Data from database
            DataSet dtSet = null;
            DataSet dtVersionAllMedia = null;
            Dictionary<Int64, List<Int64>> mediasByVersionId = null;
            switch (this._vehicle)
            {
                case DBCst.Vehicles.names.press:
                    bool isBefore2015 = true;
                    if (_webSession.CurrentModule == Module.Name.BILAN_CAMPAGNE)
                    {
                        dtSet = VersionDataAccess.GetAPPMVersions(_versions.Keys, _webSession);
                        dtVersionAllMedia = VersionDataAccess.GetAPPMVersionsAllMedia(_versions.Keys, _webSession);
                        mediasByVersionId = GetMediasByVersionId(dtVersionAllMedia);
                        isBefore2015 = Functions.Rights.ParutionDateBefore2015(_webSession.PeriodBeginningDate)
                     || Rights.ParutionDateBefore2015(_webSession.PeriodEndDate);
                    }
                    else
                    {
                        dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.press, _period);
                        dtVersionAllMedia = VersionDataAccess.GetVersionsAllMedia(_versions.Keys, _webSession, DBCst.Vehicles.names.press, _period);
                        mediasByVersionId = GetMediasByVersionId(dtVersionAllMedia);
                    }
                    break;

                case DBCst.Vehicles.names.internationalPress:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.internationalPress, _period);
                    break;

                case DBCst.Vehicles.names.radio:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.radio, _period);
                    break;
                case DBCst.Vehicles.names.radioGeneral:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.radioGeneral, _period);
                    break;
                case DBCst.Vehicles.names.radioSponsorship:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.radioSponsorship, _period);
                    break;
                case DBCst.Vehicles.names.radioMusic:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.radioMusic, _period);
                    break;
                case DBCst.Vehicles.names.tv:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.tv, _period);
                    break;
                case DBCst.Vehicles.names.tvAnnounces:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.tvAnnounces, _period);
                    break;
                case DBCst.Vehicles.names.tvGeneral:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.tvGeneral, _period);
                    break;
                case DBCst.Vehicles.names.tvSponsorship:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.tvSponsorship, _period);
                    break;
                case DBCst.Vehicles.names.tvNonTerrestrials:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.tvNonTerrestrials, _period);
                    break;
                case DBCst.Vehicles.names.directMarketing:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.directMarketing, _period);
                    break;

                case DBCst.Vehicles.names.outdoor:
                case DBCst.Vehicles.names.instore:
                case DBCst.Vehicles.names.indoor:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, _vehicle, _period);
                    break;
                default:
                    throw new VersionUIException("Non authorized vehicle level : " + _vehicle.ToString());
            }
            #endregion

            #region Build Set of VersionControl
            //Create each webcontrol
            string path = string.Empty;
            string[] pathes = null;
            string dirPath = string.Empty;
            VersionItem item = null;
            VersionDetailUI versionUi = null;

            if (dtSet != null && dtSet.Tables[0].Rows != null && dtSet.Tables[0].Rows.Count > 0)
            {
                this._versionsUIs = new ArrayList();
                foreach (DataRow row in dtSet.Tables[0].Rows)
                {
                    if (row["visual"] != DBNull.Value)
                    {

                        //build different pathes
                        switch (this._vehicle)
                        {

                            case DBCst.Vehicles.names.press:
                            case DBCst.Vehicles.names.internationalPress:
                                pathes = row["visual"].ToString().Split(',');
                                path = string.Empty;
                                bool hasCopyright = HasPressCopyright(row, mediasByVersionId);
                                foreach (string str in pathes)
                                {
                                    path += Functions.Creatives.GetCreativePath(Int64.Parse(row["idMedia"].ToString()), Int64.Parse(row["dateKiosque"].ToString()), Int64.Parse(row["dateCover"].ToString()), str, true, true, hasCopyright) + ",";
                                }
                                break;

                            case DBCst.Vehicles.names.directMarketing:
                                pathes = row["visual"].ToString().Split(',');
                                path = string.Empty;
                                dirPath = this.BuildVersionPath(row["id"].ToString(), WeBCst.CreationServerPathes.IMAGES_MD);
                                foreach (string str in pathes)
                                {
                                    path += dirPath + "/" + str + ",";
                                }
                                break;

                            case DBCst.Vehicles.names.outdoor:
                            case DBCst.Vehicles.names.indoor:
                                pathes = row["visual"].ToString().Split(',');
                                path = string.Empty;
                                dirPath = this.BuildVersionPath(row["id"].ToString(), WeBCst.CreationServerPathes.IMAGES_OUTDOOR);
                                foreach (string str in pathes)
                                {
                                    path += dirPath + "/" + str + ",";
                                }
                                break;
                            case DBCst.Vehicles.names.instore:
                                pathes = row["visual"].ToString().Split(',');
                                path = string.Empty;
                                dirPath = this.BuildVersionPath(row["id"].ToString(), WeBCst.CreationServerPathes.IMAGES_INSTORE);
                                foreach (string str in pathes)
                                {
                                    path += dirPath + "/" + str + ",";
                                }
                                break;

                            case DBCst.Vehicles.names.radio:
                            case DBCst.Vehicles.names.tv:
                                path = row["visual"].ToString();
                                break;

                            default:
                                break;
                        }

                        //fill version path
                        item = ((VersionItem)_versions[(Int64)row["id"]]);
                        if (item == null)
                        {
                            continue;
                        }


                        //build control
                        switch (this._vehicle)
                        {
                            case DBCst.Vehicles.names.press:
                                if (path.Length > 0)
                                {
                                    item.Path = path.Substring(0, path.Length - 1);
                                }
                                if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE)
                                {
                                    if (row["dateKiosque"] != System.DBNull.Value) item.Parution = row["dateKiosque"].ToString();
                                    else item.Parution = "";
                                    versionUi = new VersionPressAPPMUI(_webSession, item);
                                }
                                else
                                {
                                    versionUi = new VersionPressUI(_webSession, item);
                                }
                                break;

                            case DBCst.Vehicles.names.internationalPress:
                                if (path.Length > 0)
                                {
                                    item.Path = path.Substring(0, path.Length - 1);
                                }
                                versionUi = new VersionPressUI(_webSession, item) { };
                                break;

                            case DBCst.Vehicles.names.directMarketing:
                            case DBCst.Vehicles.names.outdoor:
                            case DBCst.Vehicles.names.instore:
                            case DBCst.Vehicles.names.indoor:
                                if (path.Length > 0)
                                {
                                    item.Path = path.Substring(0, path.Length - 1);
                                }
                                versionUi = new VersionPressUI(_webSession, item);
                                break;

                            case DBCst.Vehicles.names.radio:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionRadioUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.radio));
                                break;
                            case DBCst.Vehicles.names.radioGeneral:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionRadioUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.radioGeneral));
                                break;
                            case DBCst.Vehicles.names.radioSponsorship:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionRadioUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.radioSponsorship));
                                break;
                            case DBCst.Vehicles.names.radioMusic:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionRadioUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.radioSponsorship));
                                break;
                            case DBCst.Vehicles.names.tv:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionTvUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.tv));
                                break;
                            case DBCst.Vehicles.names.tvGeneral:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionTvUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.tvGeneral));
                                break;
                            case DBCst.Vehicles.names.tvSponsorship:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionTvUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.tvSponsorship));
                                break;
                            case DBCst.Vehicles.names.tvAnnounces:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionTvUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.tvAnnounces));
                                break;
                            case DBCst.Vehicles.names.tvNonTerrestrials:
                                if (path.Length > 0)
                                {
                                    item.Path = path;
                                }
                                versionUi = new VersionTvUI(_webSession, item, VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.tvNonTerrestrials));
                                break;

                            default:
                                throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
                        }
                        this._versionsUIs.Add(versionUi);
                    }
                }
            }
            #endregion

        }
        #endregion

        #region BuildHtml
        /// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
		protected void BuildHtml(StringBuilder output)
        {

            if (this._versionsUIs != null)
            {
                output.Append("<table align=\"left\" border=\"0\" class=\"violetBackGroundV3 txtBlanc12Bold\" style=\"text-align:left\">");
                output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
                if (_title == string.Empty)
                {
                    switch (this._vehicle)
                    {
                        case DBCst.Vehicles.names.press:
                            _title = GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage);
                            break;

                        case DBCst.Vehicles.names.internationalPress:
                            _title = GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage);
                            break;

                        case DBCst.Vehicles.names.tv:
                            _title = GestionWeb.GetWebWord(2012, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.tvGeneral:
                            _title = GestionWeb.GetWebWord(2640, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.tvSponsorship:
                            _title = GestionWeb.GetWebWord(2641, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.tvNonTerrestrials:
                            _title = GestionWeb.GetWebWord(2643, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.tvAnnounces:
                            _title = GestionWeb.GetWebWord(2642, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.radio:
                            _title = GestionWeb.GetWebWord(2011, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.radioGeneral:
                            _title = GestionWeb.GetWebWord(2637, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.radioSponsorship:
                            _title = GestionWeb.GetWebWord(2638, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.radioMusic:
                            _title = GestionWeb.GetWebWord(2639, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.directMarketing:
                            _title = GestionWeb.GetWebWord(2217, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.outdoor:
                            _title = GestionWeb.GetWebWord(2255, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.indoor:
                            _title = GestionWeb.GetWebWord(2993, this._webSession.SiteLanguage);
                            break;
                        case DBCst.Vehicles.names.instore:
                            _title = GestionWeb.GetWebWord(2667, this._webSession.SiteLanguage);
                            break;
                        default:
                            _title = "?";
                            break;
                    }
                }
                output.Append(_title);
                output.Append("</td></tr>");

                int columnIndex = 0;
                foreach (VersionDetailUI item in this._versionsUIs)
                {

                    if ((columnIndex % Nb_Columns) == 0)
                    {
                        if (columnIndex > 0)
                        {
                            output.Append("</tr>");
                        }
                        output.Append("<tr>");

                    }
                    output.Append("<td>");
                    item.GetHtml(output);
                    output.Append("</td>");
                    columnIndex++;
                }
                output.Append("</tr>");
                output.Append("</table>");
            }
        }
        #endregion

        #region BuildMSCreativesHtml
        /// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
        protected void BuildMSCreativesHtml(StringBuilder output, ResultTable resultTable)
        {

            output.Append("<table align=\"left\" border=\"0\" class=\"violetBackGroundV3 txtBlanc12Bold\">");
            output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
            if (_title == string.Empty)
            {
                switch (this._vehicle)
                {
                    case DBCst.Vehicles.names.press:
                        _title = GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.internationalPress:
                        _title = GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.tv:
                        _title = GestionWeb.GetWebWord(2012, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.tvGeneral:
                        _title = GestionWeb.GetWebWord(2633, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.tvAnnounces:
                        _title = GestionWeb.GetWebWord(2635, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.tvSponsorship:
                        _title = GestionWeb.GetWebWord(2634, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.tvNonTerrestrials:
                        _title = GestionWeb.GetWebWord(2636, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.radio:
                        _title = GestionWeb.GetWebWord(2011, this._webSession.SiteLanguage);
                        break;
                    case DBCst.Vehicles.names.radioGeneral:
                        _title = GestionWeb.GetWebWord(2637, this._webSession.SiteLanguage);
                        break;
                    case DBCst.Vehicles.names.radioSponsorship:
                        _title = GestionWeb.GetWebWord(2638, this._webSession.SiteLanguage);
                        break;
                    case DBCst.Vehicles.names.radioMusic:
                        _title = GestionWeb.GetWebWord(2639, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.directMarketing:
                        _title = GestionWeb.GetWebWord(2217, this._webSession.SiteLanguage);
                        break;

                    case DBCst.Vehicles.names.outdoor:
                        _title = GestionWeb.GetWebWord(2255, this._webSession.SiteLanguage);
                        break;
                    case DBCst.Vehicles.names.indoor:
                        _title = GestionWeb.GetWebWord(2993, this._webSession.SiteLanguage);
                        break;
                    case DBCst.Vehicles.names.instore:
                        _title = GestionWeb.GetWebWord(2667, this._webSession.SiteLanguage);
                        break;
                    default:
                        _title = "?";
                        break;
                }
            }
            output.Append(_title);
            output.Append("</td></tr>");

            int columnIndex = 0;
            for (int i = 0; i < resultTable.LinesNumber; i++)
            {
                if ((columnIndex % Nb_Columns) == 0)
                {
                    if (columnIndex > 0)
                    {
                        output.Append("</tr>");
                    }
                    output.Append("<tr>");

                }
                output.Append("<td>");
                output.Append(((CellCreativesInformation)resultTable[i, 1]).RenderThumbnails());
                output.Append("</td>");
                columnIndex++;
            }
            output.Append("</tr>");
            output.Append("</table>");

        }
        #endregion



        #region Internal Methods
        /// <summary>
        /// Build visual access path depending on the vehicle
        /// </summary>
        /// <param name="date">date to format YYYYMMDD</param>
        /// <param name="idMedia">Media Id</param>
        /// <returns>Full path to access an image</returns>
        private string BuildVersionPath(string idMedia, string date, string folderPath)
        {
            string path = string.Empty;
            switch (this._vehicle)
            {
                case DBCst.Vehicles.names.press:
                case DBCst.Vehicles.names.internationalPress:
                    path = folderPath + "/" + idMedia + "/" + date + "/imagette";
                    break;
                default:
                    throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
            }
            return path;
        }
        /// <summary>
        /// Build visual access path for Marketing Direct & Outdoor
        /// </summary>
        /// <param name="idSlogan">Slogan ID</param>
        /// <returns>Full path to access an image</returns>
        private string BuildVersionPath(string idSlogan, string folderPath)
        {
            string path = string.Empty;
            path = folderPath;
            string dir1 = idSlogan.Substring(idSlogan.Length - 1, 1);
            path = string.Format(@"{0}/{1}", path, dir1);
            string dir2 = idSlogan.Substring(idSlogan.Length - 2, 1);
            path = string.Format(@"{0}/{1}", path, dir2);
            string dir3 = idSlogan.Substring(idSlogan.Length - 3, 1);
            path = string.Format(@"{0}/{1}/imagette", path, dir3);
            return path;
        }

        #region HasPressCopyright
        ///// <summary>
        ///// Has Press Copyright
        ///// </summary>
        ///// <param name="row">Row</param>
        ///// <param name="mediasByVersionId">Medias By Version Id</param>
        ///// <returns>True if has copyright press</returns>
        //protected bool HasPressCopyright(DataRow row, Dictionary<Int64, List<Int64>> mediasByVersionId) {

        //    string ids = Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaExcludedForCopyright);
        //    if (!string.IsNullOrEmpty(ids))
        //    {
        //        if (row.Table.Columns.Contains("id") && row["id"] != DBNull.Value) {
        //            long idSlogan = Convert.ToInt64(row["id"]);

        //            var notAllowedMediaIds = ids.Split(',').Select(p => Convert.ToInt64(p)).ToList();
        //            foreach (Int64 idMedia in mediasByVersionId[idSlogan]) {
        //                if (!notAllowedMediaIds.Contains(idMedia))
        //                    return true;
        //            }
        //        }

        //        return false;
        //    }
        //    return true;
        //}
        #endregion
        /// <summary>
        /// Has Press Copyright
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="mediasByVersionId">Medias By Version Id</param>
        /// <returns>True if has copyright press</returns>
        protected bool HasPressCopyright(DataRow row, Dictionary<Int64, List<Int64>> mediasByVersionId)
        {
            string ids = Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaExcludedForCopyright);
            if (!string.IsNullOrEmpty(ids))
            {
                bool isBefore2015;
                if (_period != null && _webSession.CurrentModule != Module.Name.BILAN_CAMPAGNE)
                    isBefore2015 = Rights.ParutionDateBefore2015(_period.Begin.ToString("yyyyMMdd")) || Rights.ParutionDateBefore2015(_period.End.ToString("yyyyMMdd"));
                else
                    isBefore2015 = Rights.ParutionDateBefore2015(_webSession.PeriodBeginningDate) || Rights.ParutionDateBefore2015(_webSession.PeriodEndDate);

                if (!isBefore2015 && row.Table.Columns.Contains("id") && row["id"] != DBNull.Value)
                {
                    long idSlogan = Convert.ToInt64(row["id"]);

                    var notAllowedMediaIds = ids.Split(',').Select(p => Convert.ToInt64(p)).ToList();
                    return mediasByVersionId[idSlogan].Any(idMedia => !notAllowedMediaIds.Contains(idMedia));
                }

            }
            return true;
        }
        #endregion

        #region GetMediasByVersionId
        /// <summary>
        /// Get Medias By Version Id
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>Media list by version Id</returns>
        private Dictionary<Int64, List<Int64>> GetMediasByVersionId(DataSet ds)
        {

            Dictionary<Int64, List<Int64>> mediasByVersionId = new Dictionary<Int64, List<Int64>>();
            List<Int64> medias = new List<Int64>();
            Int64 idSloganOld = -1;
            Int64 idSlogan = -1;

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow currentRow in ds.Tables[0].Rows)
                {
                    if (currentRow["id_slogan"] != null && currentRow["id_slogan"] != System.DBNull.Value && Int64.Parse(currentRow["id_slogan"].ToString()) != 0)
                    {

                        idSlogan = Int64.Parse(currentRow["id_slogan"].ToString());

                        if (idSlogan != idSloganOld)
                        {
                            medias = new List<Int64>();
                            medias.Add(Int64.Parse(currentRow["id_media"].ToString()));
                            mediasByVersionId.Add(idSlogan, medias);
                        }
                        else
                        {
                            mediasByVersionId[idSlogan].Add(Int64.Parse(currentRow["id_media"].ToString()));
                        }

                        idSloganOld = idSlogan;

                    }
                }
            }

            return mediasByVersionId;
        }
        #endregion

        #endregion

    }
}