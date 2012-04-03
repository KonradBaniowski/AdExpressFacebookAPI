#region Information
// Author: D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpressI.Portofolio.Engines {
	/// <summary>
	/// Compute portofolio synthesis' results
	/// </summary>
	public class SynthesisEngine : Engine {

        #region Const
        protected const string UNIT_FORMAT = "{0:max0}";
        #endregion

        #region Variables
        /// <summary>
        /// Dal portofolio Synthesis
        /// </summary>
        protected IPortofolioDAL _portofolioDAL = null;
        #endregion

        #region Constructor
        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {

            #region Load DAL Layer
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            _portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            #endregion

        }

		#endregion
		
		#region Public methods

		#region Abstract methods implementation
		/// <summary>
		/// Get Result Table for portofolio synthesis
		/// </summary>
		/// <returns>ResultTable</returns>
		protected override ResultTable ComputeResultTable() {

            #region Constantes
            /// <summary>
            /// Header column index
            /// </summary>
            const int HEADER_COLUMN_INDEX = 0;
            /// <summary>
            /// First column index
            /// </summary>
            const int FIRST_COLUMN_INDEX = 1;
            /// <summary>
            /// Second column index
            /// </summary>
            const int SECOND_COLUMN_INDEX = 2;

            #endregion

            #region Variables
            ResultTable resultTable = null;
            List<ICell> data = null;
            LineType lineType = LineType.level1;
            #endregion

            #region GetDataList
            data = GetDataCellList();
            #endregion

            #region No Result
            if (data == null) return null;
            #endregion

            #region Building Result Table

            #region headers
            Headers headers = new Headers();
            TNS.FrameWork.WebResultUI.Header header = new TNS.FrameWork.WebResultUI.Header(GetDataMedia().ToString(), HEADER_COLUMN_INDEX, "SynthesisH1");
            header.Add(new TNS.FrameWork.WebResultUI.Header("", FIRST_COLUMN_INDEX, "SynthesisH2"));
            header.Add(new TNS.FrameWork.WebResultUI.Header("", SECOND_COLUMN_INDEX, "SynthesisH2"));
            headers.Root.Add(header);
            resultTable = new ResultTable(data.Count / 2, headers);
            #endregion

            #region Data table
            int lineIndex = 0;
            for (int i = 0; i < data.Count; i += 2) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = data[i];
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = data[i+1];
                ChangeLineType(ref lineType);
            }
            #endregion

            #endregion

            return resultTable;

		}
		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override string BuildHtmlResult() {
			throw new PortofolioException("The method or operation is not implemented.");
        }
        #endregion

        #endregion

        #region Protected methods

        #region Change line type
        /// <summary>
		/// Change line type
		/// </summary>
		/// <param name="lineType">Line Type</param>
		/// <returns>Line type</returns>
		protected virtual void ChangeLineType(ref LineType lineType) {

			if (lineType == LineType.level1)
				lineType = LineType.level2;
			else
				lineType = LineType.level1;

		}
		#endregion		

        #region GetPeriod
        /// <summary>
        /// GetPeriod
        /// </summary>
        protected virtual void GetPeriod(bool isAlertModule, string firstDate, string lastDate, out DateTime dtFirstDate, out DateTime dtLastDate) {

            dtFirstDate = DateTime.Today;
            dtLastDate = DateTime.Today;

            if (isAlertModule && (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing 
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.czinternet
                )) {
                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.indoor
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.instore) {
                    if (firstDate.Length > 0) {
                        dtFirstDate = Convert.ToDateTime(firstDate);
                        dtFirstDate = dtFirstDate.Date;
                    }
                    if (lastDate.Length > 0) {
                        dtLastDate = Convert.ToDateTime(lastDate);
                        dtLastDate = dtLastDate.Date;
                    }
                }
                else {
                    if (firstDate.Length > 0) {
                        dtFirstDate = new DateTime(int.Parse(firstDate.Substring(0, 4)), int.Parse(firstDate.Substring(4, 2)), int.Parse(firstDate.Substring(6, 2)));
                    }

                    if (lastDate.Length > 0) {
                        dtLastDate = new DateTime(int.Parse(lastDate.Substring(0, 4)), int.Parse(lastDate.Substring(4, 2)), int.Parse(lastDate.Substring(6, 2)));
                    }
                }
            }
            else {
                dtFirstDate = WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType);
                dtLastDate = WebFunctions.Dates.getPeriodEndDate(_periodEnd, _webSession.PeriodType);
            }
        }
        #endregion		

        #region GetData

        #region GetDataMedia
        /// <summary>
        /// GetDataMedia
        /// </summary>
        protected virtual string GetDataMedia() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.media);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
                return (dt.Rows[0]["media"].ToString());
            return string.Empty;
        }
        #endregion

        #region GetDataCategory
        /// <summary>
        /// GetDataCategory
        /// </summary>
        protected virtual string GetDataCategory() {
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category)) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.category);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                    return (dt.Rows[0]["category"].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataMediaSeller
        /// <summary>
        /// GetDataMediaSeller
        /// </summary>
        protected virtual string GetDataMediaSeller() {
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
                    && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile
                    && _vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.mediaSeller)) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.mediaSeller);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                    return (dt.Rows[0]["media_seller"].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataInterestCenter
        /// <summary>
        /// GetDataInterestCenter
        /// </summary>
        protected virtual string GetDataInterestCenter() {
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.interestCenter)) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.interestCenter);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                    return (dt.Rows[0]["interest_center"].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataPeriodicity
        /// <summary>
        /// GetDataPeriodicity
        /// </summary>
        protected virtual string GetDataPeriodicity() {
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                ) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.periodicity);
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["periodicity"].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataPeriodSelected
        /// <summary>
        /// GetDataPeriodSelected
        /// </summary>
        protected virtual void GetDataPeriodSelected(bool isAlertModule, out string firstDate, out string lastDate) {
            firstDate = string.Empty;
            lastDate = string.Empty;
            if (isAlertModule
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.czinternet
                ) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.periodSelected);
                DataTable dt = ds.Tables[0];
                if (dt.Columns.Contains("first_date")) firstDate = dt.Rows[0]["first_date"].ToString();
                if (dt.Columns.Contains("last_date")) lastDate = dt.Rows[0]["last_date"].ToString();
            }
        }
        #endregion

        #region GetDataAdNumberIncludingInsets
        /// <summary>
        /// GetDataAdNumberIncludingInsets
        /// </summary>
        protected virtual string GetDataAdNumberIncludingInsets() {
            if (WebApplicationParameters.AllowInsetOption
                && _vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages
                ) && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine

                )) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.adNumberIncludingInsets);
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString()].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataAdNumberExcludingInsets
        /// <summary>
        /// GetDataAdNumberExcludingInsets
        /// </summary>
        protected virtual string GetDataAdNumberExcludingInsets() {
            if (WebApplicationParameters.AllowInsetOption
                && _vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages
                ) && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                )) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.adNumberExcludingInsets);
                DataTable dt = ds.Tables[0];
                 return (dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString()].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataPageNumber
        /// <summary>
        /// GetDataPageNumber
        /// </summary>
        protected virtual string GetDataPageNumber() {
            string pageNumber = string.Empty;
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages)
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.czinternet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.pageNumber);
                DataTable dt = ds.Tables[0];
                pageNumber = dt.Rows[0]["page"].ToString();
                if (pageNumber.Length == 0)
                    pageNumber = "0";
            }
            return pageNumber;
        }
        #endregion

        #region GetDataProductNumber
        /// <summary>
        /// GetDataProductNumber
        /// </summary>
        protected virtual string GetDataProductNumber() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberProduct);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["nbLines"].ToString());
            }
            else return null;
        }
        #endregion

        #region GetDataProductNumberInTracking
        /// <summary>
        /// GetDataProductNumberInTracking
        /// </summary>
        protected virtual string GetDataProductNumberInTracking() {
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.instore
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.indoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.czinternet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.cinema)
            {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberNewProductInTracking);
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["nbLines"].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataProductNumberInVehicle
        /// <summary>
        /// GetDataProductNumberInVehicle
        /// </summary>
        protected virtual string GetDataProductNumberInVehicle() {
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.instore
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.indoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.czinternet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.cinema)
            {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberNewProductInVehicle);
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["nbLines"].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataAdvertiserNumber
        /// <summary>
        /// GetDataAdvertiserNumber
        /// </summary>
        protected virtual string GetDataAdvertiserNumber() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberAdvertiser);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["nbLines"].ToString());
            }
            else return null;
        }
        #endregion

        #region GetDataBannerNumber (Evaliant)
        /// <summary>
        /// GetDataBannerNumber (Evaliant)
        /// </summary>
        protected virtual string GetDataBannerNumber() {
            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                ) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberBanners);
                DataTable dt = ds.Tables[0];
                CellIdsNumber cell = new CellIdsNumber();
                foreach (DataRow row in dt.Rows) {
                    cell.Add(row["hashcode"].ToString().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
               return (cell.Value.ToString());
            }
            return string.Empty;
        }
        #endregion

        #region GetDataTypeSale
        /// <summary>
        /// GetDataTypeSale
        /// </summary>
        protected virtual DataTable GetDataTypeSale() {
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor
                ||_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.indoor
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.instore) {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.typeSale);
                return (ds.Tables[0]);
            }
            return null;
        }
        #endregion

        #endregion

        #region ComputeData

        #region ComputeDataPeriodIssue
        protected virtual List<ICell> ComputeDataPeriodIssue(bool isAlertModule) {

            #region Variables
            string firstDate = string.Empty;
            string lastDate = string.Empty;
            DateTime dtFirstDate = DateTime.Today;
            DateTime dtLastDate = DateTime.Today;
            List<ICell> data = null;
            #endregion

            #region Get Data
            GetDataPeriodSelected(isAlertModule, out firstDate, out lastDate);
            GetPeriod(isAlertModule, firstDate, lastDate, out dtFirstDate, out dtLastDate);
            #endregion

            #region Compute data
            if (!isAlertModule
                || (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.directMarketing && _webSession.CustomerPeriodSelected.IsSliding4M)
                || (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internet && _webSession.CustomerPeriodSelected.IsSliding4M)
                || (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.czinternet && _webSession.CustomerPeriodSelected.IsSliding4M)
                || (firstDate.Length > 0 && lastDate.Length > 0 && isAlertModule)) {

                // Date begin and date end for outdooor
                if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.indoor
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.instore) 
                    && isAlertModule) {
                    data = new List<ICell>(4);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1607, _webSession.SiteLanguage)));
                    data.Add(new CellLabel(Dates.DateToString(dtFirstDate, _webSession.SiteLanguage)));

                    data.Add(new CellLabel(GestionWeb.GetWebWord(1608, _webSession.SiteLanguage)));
                    data.Add(new CellLabel(Dates.DateToString(dtLastDate, _webSession.SiteLanguage)));
                }
                // Period selected
                else {
                    //if (firstDate.Length > 0 || !isAlertModule) {
                    data = new List<ICell>(2);
                    if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) && isAlertModule) {
                        data.Add(new CellLabel(GestionWeb.GetWebWord(1381, _webSession.SiteLanguage)));
                    }
                    else {
                        data.Add(new CellLabel(GestionWeb.GetWebWord(1541, _webSession.SiteLanguage)));
                    }
                    if ((firstDate != null && firstDate.Length > 0 && lastDate != null && lastDate.Length > 0 && firstDate.Equals(lastDate) && isAlertModule)
                        || (dtLastDate.CompareTo(dtFirstDate) == 0 && !isAlertModule)) {
                        data.Add(new CellLabel(Dates.DateToString(dtFirstDate, _webSession.SiteLanguage)));
                    }
                    else {
                        data.Add(new CellLabel(GestionWeb.GetWebWord(896, _webSession.SiteLanguage) + " " + Dates.DateToString(dtFirstDate, _webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(1730, _webSession.SiteLanguage) + " " + Dates.DateToString(dtLastDate, _webSession.SiteLanguage)));
                    }
                    //}
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataPeriodicity
        protected virtual List<ICell> ComputeDataPeriodicity() {

            #region Variables
            string periodicity = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Get Data
            periodicity = GetDataPeriodicity();
            #endregion

            #region Compute data
            if (periodicity.Length > 0) {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1450, _webSession.SiteLanguage)));
                data.Add(new CellLabel(periodicity));
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataCategory
        protected virtual List<ICell> ComputeDataCategory() {

            #region Variables
            string category = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Get Data
            category = GetDataCategory();
            #endregion

            #region Compute data
            if (category.Length > 0) {
                data = new List<ICell>(2); 
                data.Add(new CellLabel(GestionWeb.GetWebWord(1416, _webSession.SiteLanguage)));
                data.Add(new CellLabel(category));
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataMediaSeller
        protected virtual List<ICell> ComputeDataMediaSeller() {

            #region Variables
            string regie = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing) {

                #region Get Data
                regie = GetDataMediaSeller();
                #endregion

                if (regie.Length > 0) {
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1417, _webSession.SiteLanguage)));
                    data.Add(new CellLabel(regie));
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataVolumeForMarketingDirect
        protected virtual List<ICell> ComputeDataVolumeForMarketingDirect(DataUnit dataUnit) {

            #region Variables
            string volume = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Compute data
            if (dataUnit!=null && _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.directMarketing &&
                _webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_VOLUME_MARKETING_DIRECT)) {

                #region Get Data
                volume = dataUnit.GetVolume();
                #endregion

                if (volume != null && volume.Length > 0) {
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(2216, _webSession.SiteLanguage)));
                    CellVolume cV = new CellVolume(double.Parse(volume));
                    cV.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.volume).StringFormat;
                    data.Add(cV);
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataInterestCenter
        protected virtual List<ICell> ComputeDataInterestCenter() {

            #region Variables
            string interestCenter = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing) {

                #region Get Data
                interestCenter = GetDataInterestCenter();
                #endregion

                if (interestCenter.Length > 0) {
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1411, _webSession.SiteLanguage)));
                    data.Add(new CellLabel(interestCenter));
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataBannersNumber
        protected virtual List<ICell> ComputeDataBannersNumber() {

            #region Variables
            string numberBanner = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Compute data
            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)) {

                #region Get Data
                numberBanner = GetDataBannerNumber();
                #endregion

                if (numberBanner.Length > 0) {
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(2479, _webSession.SiteLanguage)));
                    CellNumber cN = new CellNumber(double.Parse(numberBanner.ToString()));
                    cN.StringFormat = UNIT_FORMAT;
                    data.Add(cN);
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataNumberBoard
        protected virtual List<ICell> ComputeDataNumberBoard(DataUnit dataUnit) {

            #region Variables
            string numberBoard = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Compute data
            if (dataUnit!=null 
                && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.indoor
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.instore)) {

                #region Get Data
                numberBoard = dataUnit.GetNumberBoard();
                #endregion

                //number board
                if (numberBoard != null && numberBoard.Length > 0) {
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1604, _webSession.SiteLanguage)));
                    CellNumber cN1 = new CellNumber(double.Parse(numberBoard.ToString()));
                    cN1.StringFormat = UNIT_FORMAT;
                    data.Add(cN1);
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataNetworkType
        protected virtual List<ICell> ComputeDataNetworkType(bool isAlertModule) {

            #region Variables
            DataTable dtTypeSale = null;
            List<ICell> data = null;
            string typeReseauStr = string.Empty;
            #endregion

            #region Compute data
            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.indoor
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.instore)
                && isAlertModule) {

                #region Get Data
                dtTypeSale = GetDataTypeSale();
                #endregion

                if (dtTypeSale != null && dtTypeSale.Rows.Count > 0) {
                    //Type sale
                    int count = 0;
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1609, _webSession.SiteLanguage)));
                    if (dtTypeSale.Rows.Count == 0) typeReseauStr = "&nbsp;";
                    else {
                        foreach (DataRow row in dtTypeSale.Rows) {
                            if (count > 0) {
                                typeReseauStr += "<BR>";
                            }
                            typeReseauStr += SQLGenerator.SaleTypeOutdoor(row["type_sale"].ToString(), _webSession.SiteLanguage);
                            count++;
                        }
                    }
                    data.Add(new CellLabel(typeReseauStr));
                }

            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeData for Vehicle Press

        #region ComputeDataPageNumber
        protected virtual List<ICell> ComputeDataPageNumber() {

            #region Variables
            List<ICell> data = null;
            string pageNumber = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {

                #region Get Data
                pageNumber = GetDataPageNumber();
                #endregion

                if (pageNumber != null && pageNumber.Length > 0) {
                    // Nombre de page
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1385, _webSession.SiteLanguage)));
                    CellNumber cN2 = new CellNumber(double.Parse(pageNumber));
                    cN2.StringFormat = UNIT_FORMAT;
                    data.Add(cN2);
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAdNumber
        protected virtual List<ICell> ComputeDataAdNumber(DataUnit dataUnit) {

            #region Variables
            List<ICell> data = null;
            string adNumber = string.Empty;
            #endregion

            #region Compute data
            if (dataUnit!=null && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress)) {

                #region Get Data
                adNumber = dataUnit.GetAdNumber();
                #endregion

                if (adNumber != null && adNumber.Length > 0) {
                    // Nb de page pub		
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1386, _webSession.SiteLanguage)));
                    CellPage cP = new CellPage(double.Parse(adNumber));
                    cP.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat;
                    data.Add(cP);
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataPageRatio
        protected virtual List<ICell> ComputeDataPageRatio(double pageNumber, double adNumber) {

            #region Variables
            List<ICell> data = null;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {

                // Ratio
                if (adNumber >= 0) {
                    data = new List<ICell>(2);
                    if (pageNumber >= 0) {
                        data.Add(new CellLabel(GestionWeb.GetWebWord(1387, _webSession.SiteLanguage)));
                        data.Add(new CellPercent(((adNumber / pageNumber * 100) / (double)1000)));
                    }
                    else {
                        data.Add(new CellLabel(GestionWeb.GetWebWord(1387, _webSession.SiteLanguage)));
                        data.Add(new CellLabel("&nbsp;&nbsp;&nbsp;&nbsp;"));
                    }
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAdNumberExcludingInsets
        protected virtual List<ICell> ComputeDataAdNumberExcludingInsets(bool isAlertModule) {

            #region Variables
            List<ICell> data = null;
            string adNumberExcludingInsets = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                if (isAlertModule && WebApplicationParameters.AllowInsetOption == true) {

                    #region Get Data
                    adNumberExcludingInsets = GetDataAdNumberExcludingInsets();
                    #endregion

                    if (adNumberExcludingInsets != null && adNumberExcludingInsets.Length > 0) {
                        // Nombre de page de pub hors encarts
                        if (adNumberExcludingInsets.Length == 0)
                            adNumberExcludingInsets = "0";
                        data = new List<ICell>(2);
                        data.Add(new CellLabel(GestionWeb.GetWebWord(1388, _webSession.SiteLanguage)));
                        CellPage cP1 = new CellPage(double.Parse(adNumberExcludingInsets));
                        cP1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat;
                        data.Add(cP1);
                    }
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAdNumberIncludingInsets
        protected virtual List<ICell> ComputeDataAdNumberIncludingInsets(bool isAlertModule) {

            #region Variables
            List<ICell> data = null;
            string adNumberIncludingInsets = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                if (isAlertModule && WebApplicationParameters.AllowInsetOption == true) {

                    #region Get Data
                    adNumberIncludingInsets = GetDataAdNumberIncludingInsets();
                    #endregion

                    if (adNumberIncludingInsets != null && adNumberIncludingInsets.Length > 0) {
                        // Nombre de page de pub encarts
                        if (adNumberIncludingInsets.Length == 0) {
                            adNumberIncludingInsets = "0";
                        }
                        data = new List<ICell>(2);
                        data.Add(new CellLabel(GestionWeb.GetWebWord(1389, _webSession.SiteLanguage)));
                        CellPage cP2 = new CellPage(double.Parse(adNumberIncludingInsets));
                        cP2.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat;
                        data.Add(cP2);
                    }
                }
            }
            #endregion

            return data;

        }
        #endregion

        #endregion

        #region ComputeData for Vehicle TV, Radio

        #region ComputeDataSpotNumber
        protected virtual List<ICell> ComputeDataSpotNumber(DataUnit dataUnit) {

            #region Variables
            List<ICell> data = null;
            string nbrSpot = string.Empty;
            #endregion

            #region Compute data
            if (dataUnit!=null && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)) {

                #region Get Data
                nbrSpot = dataUnit.GetSpotNumber();
                #endregion

                //Nombre de spot
                if (nbrSpot.Length == 0) {
                    nbrSpot = "0";
                }
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1404, _webSession.SiteLanguage)));
                CellNumber cN3 = new CellNumber(double.Parse(nbrSpot));
                cN3.StringFormat = UNIT_FORMAT;
                data.Add(cN3);
            }
            #endregion

            return data;

        }
       
        #endregion

        #region ComputeDataEcranNumber
        protected virtual List<ICell> ComputeDataEcranNumber(DataEcran dataEcran) {

            #region Variables
            List<ICell> data = null;
            string nbrEcran = string.Empty;
            #endregion

            #region Compute data
            if (dataEcran!=null &&
                (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)) {
                if (dataEcran.IsAlertModule) {

                    #region Get Data
                    nbrEcran = dataEcran.GetNumber();
                    #endregion

                    // Nombre d'ecran
                    if (nbrEcran.Length == 0) {
                        nbrEcran = "0";
                    }
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1412, _webSession.SiteLanguage)));
                    CellNumber cN4 = new CellNumber(double.Parse(nbrEcran));
                    cN4.StringFormat = UNIT_FORMAT;
                    data.Add(cN4);
                }
            }
            #endregion

            return data;

        }
       
        #endregion

        #region ComputeDataTotalDuration
        protected virtual List<ICell> ComputeDataTotalDuration(DataUnit dataUnit) {

            #region Variables
            List<ICell> data = null;
            string totalDuration = string.Empty;
            #endregion

            #region Compute data
            if (dataUnit!= null && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)) {

                #region Get Data
                totalDuration = dataUnit.GetTotalDuration();
                #endregion

                if (totalDuration.Length == 0) {
                    totalDuration = "0";
                }
                // total durée
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1413, _webSession.SiteLanguage)));
                CellDuration cD = new CellDuration(double.Parse(totalDuration));
                cD.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.duration).StringFormat;
                data.Add(cD);
            }
            #endregion

            return data;

        }
       
        #endregion

        #region ComputeDataEvaliantInsertionNumber
        protected virtual List<ICell> ComputeDataEvaliantInsertionNumber(DataUnit dataUnit) {

            #region Variables
            List<ICell> data = null;
            string nbrSpot = string.Empty;
            #endregion

            #region Compute data
            if (dataUnit!=null && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.others
                && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)) {

                #region Get Data
                nbrSpot = dataUnit.GetSpotNumber();
                #endregion

                // Number of insertion (occurrences) Evaliant
                if (nbrSpot.Length == 0) {
                    nbrSpot = "0";
                }
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1398, _webSession.SiteLanguage)));
                CellNumber cN5 = new CellNumber(double.Parse(nbrSpot));
                cN5.StringFormat = UNIT_FORMAT;
                data.Add(cN5);
            }
            #endregion

            return data;

        }
        #endregion

        #endregion

        #region ComputeDataInvestissementsTotal
        /// <summary>
        /// Compute Data Investissements Total
        /// </summary>
        /// <param name="dataUnit">data Unit</param>
        /// <returns>List of cells</returns>
        protected virtual List<ICell> ComputeDataInvestissementsTotal(DataUnit dataUnit) {

            #region Variables
            List<ICell> data = null;
            string investment = string.Empty;
            UnitInformation defaultCurrency = UnitsInformation.List[UnitsInformation.DefaultCurrency];
            #endregion

            #region Get Data
            investment = dataUnit.GetInvestment();
            #endregion

            #region Compute data
            if (dataUnit!=null && investment != null && investment.Length > 0 && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile) {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2787, _webSession.SiteLanguage) + " (" + defaultCurrency.GetUnitWebText(_webSession.SiteLanguage) + ")"));
                CellEuro cE = new CellEuro(double.Parse(investment));
                cE.StringFormat = UnitsInformation.Get(UnitsInformation.DefaultCurrency).StringFormat;
                data.Add(cE);
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataProductNumber
        protected virtual List<ICell> ComputeDataProductNumber() {

            #region Variables
            List<ICell> data = null;
            string numberProduct = string.Empty;
            #endregion

            #region Get Data
            numberProduct = GetDataProductNumber();
            #endregion

            #region Compute data
            if (numberProduct != null)
            {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1393, _webSession.SiteLanguage)));
                CellNumber cN6 = new CellNumber(double.Parse(numberProduct));
                cN6.StringFormat = UNIT_FORMAT;
                data.Add(cN6);
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataProductNumberInTracking
        protected virtual List<ICell> ComputeDataProductNumberInTracking(bool isAlertModule) {

            #region Variables
            List<ICell> data = null;
            string numberNewProductInTracking = string.Empty;
            #endregion

            #region Compute data
            if ((_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.instore
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.indoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.czinternet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.cinema)
                && isAlertModule) {

                #region Get Data
                numberNewProductInTracking = GetDataProductNumberInTracking();
                #endregion

                //Nombre de nouveaux produits dans la pige
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1394, _webSession.SiteLanguage)));
                CellNumber cN7 = new CellNumber(double.Parse(numberNewProductInTracking));
                cN7.StringFormat = UNIT_FORMAT;
                data.Add(cN7);
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataProductNumberInVehicle
        protected virtual List<ICell> ComputeDataProductNumberInVehicle(bool isAlertModule) {

            #region Variables
            List<ICell> data = null;
            string numberNewProductInVehicle = string.Empty;
            #endregion

            #region Compute data
            if ((_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.instore
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.indoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.czinternet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.cinema)
                && isAlertModule) {

                #region Get Data
                numberNewProductInVehicle = GetDataProductNumberInVehicle();
                #endregion

                //Nombre de nouveaux produits dans le support
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1395, _webSession.SiteLanguage)));
                CellNumber cN8 = new CellNumber(double.Parse(numberNewProductInVehicle));
                cN8.StringFormat = UNIT_FORMAT;
                data.Add(cN8);
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAdvertiserNumber
        protected virtual List<ICell> ComputeDataAdvertiserNumber() {

            #region Variables
            List<ICell> data = null;
            string numberAdvertiser = string.Empty;
            #endregion

            #region Get Data
            numberAdvertiser = GetDataAdvertiserNumber();
            #endregion

            #region Compute data
            if (numberAdvertiser != null)
            {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1396, _webSession.SiteLanguage)));
                CellNumber cN9 = new CellNumber(double.Parse(numberAdvertiser));
                cN9.StringFormat = UNIT_FORMAT;
                data.Add(cN9);
            }
            #endregion

            return data;

        }
        #endregion

        #region Cas tv, radio, others

        #region ComputeDataAverageDurationEcran
        protected virtual List<ICell> ComputeDataAverageDurationEcran(DataEcran dataEcran) {

            #region Variables
            List<ICell> data = null;
            decimal averageDurationEcran = 0;
            string nbrEcran = null;
            #endregion

            #region Get Data
            if (dataEcran != null) {
                nbrEcran = dataEcran.GetNumber();
                if (nbrEcran.Length > 0) {
                    averageDurationEcran = dataEcran.GetAverageDuration();
                }
            }
            #endregion

            #region Compute data
            if (nbrEcran != null && nbrEcran.Length > 0
                && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
                && dataEcran.IsAlertModule) {

                // Durée moyenne d'un écran

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1414, _webSession.SiteLanguage)));
                CellDuration cD1 = new CellDuration(Convert.ToDouble(((long)averageDurationEcran).ToString()));
                cD1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.duration).StringFormat;
                data.Add(cD1);

            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataSpotNumberByEcran
        protected virtual List<ICell> ComputeDataSpotNumberByEcran(DataEcran dataEcran) {

            #region Variables
            List<ICell> data = null;
            decimal nbrSpotByEcran = 0;
            string nbrEcran = null;
            #endregion

            #region Get Data
            if (dataEcran != null) {
                nbrEcran = dataEcran.GetNumber();
                if (nbrEcran.Length > 0) {
                    nbrSpotByEcran = dataEcran.GetSpotNumberByEcran();
                }
            }
            #endregion

            #region Compute data
            if (nbrEcran != null && nbrEcran.Length > 0
                && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
                && dataEcran.IsAlertModule) {

                // Nombre moyen de spots par écran
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1415, _webSession.SiteLanguage)));
                data.Add(new CellLabel(nbrSpotByEcran.ToString("0.00")));
            }
            #endregion

            return data;

        }
        #endregion

        #endregion

        #endregion

        #region GetDataCells
        /// <summary>
        /// Get Data Cell List
        /// </summary>
        /// <returns>Data Cell List</returns>
        protected virtual List<ICell> GetDataCellList() {

            #region Variables
            List<ICell> dataTemp = null;
            List<ICell> data = null;
            List<ICell> dataInvestisment = null;
            List<ICell> dataProductNumber = null;
            List<ICell> dataAdvertiserNumber = null;
            DataEcran dataEcran = null;
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;
            #endregion

            #region AlertModule
            bool isAlertModule = _webSession.CustomerPeriodSelected.IsSliding4M;
            if (isAlertModule == false) {
                DateTime DateBegin = WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType);
                if (DateBegin > DateTime.Now)
                    isAlertModule = true;
            }
            #endregion

            #region Building Data Result

            DataUnit dataUnit = new DataUnit(_portofolioDAL, _vehicleInformation);

            #region Get Priority Data
            dataInvestisment = ComputeDataInvestissementsTotal(dataUnit);
            dataProductNumber = ComputeDataProductNumber();
            dataAdvertiserNumber = ComputeDataAdvertiserNumber();

            #region No Data
            if ((dataInvestisment == null || dataInvestisment.Count != 2 || !(dataInvestisment[1] is CellNumber) || ((CellNumber)dataInvestisment[1]).Value <= 0)
                && (dataProductNumber == null || dataProductNumber.Count != 2 || !(dataProductNumber[1] is CellNumber) || ((CellNumber)dataProductNumber[1]).Value <= 0)
                && (dataAdvertiserNumber == null || dataAdvertiserNumber.Count != 2 || !(dataAdvertiserNumber[1] is CellNumber) || ((CellNumber)dataAdvertiserNumber[1]).Value <= 0))
                return null;
            #endregion

            if(_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others){
                 dataEcran = new DataEcran(_portofolioDAL, _vehicleInformation, isAlertModule);
            }

            #endregion

            #region Compute Period Issue
            data = ComputeDataPeriodIssue(isAlertModule);
            #endregion

            #region Periodicity
            dataTemp = ComputeDataPeriodicity();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Category
            dataTemp = ComputeDataCategory();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Media Seller
            dataTemp = ComputeDataMediaSeller();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Volume for Marketing Direct
            dataTemp = ComputeDataVolumeForMarketingDirect(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Interest center
            dataTemp = ComputeDataInterestCenter();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Number of banners
            dataTemp = ComputeDataBannersNumber();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region number board and newtwork type
            dataTemp = ComputeDataNumberBoard(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);

            dataTemp = ComputeDataNetworkType(isAlertModule);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Case vehicle press
            List<ICell> dataPageNumber = ComputeDataPageNumber();
            if (dataPageNumber != null) data.AddRange(dataPageNumber);

            List<ICell> dataAdNumber = ComputeDataAdNumber(dataUnit);
            if (dataAdNumber != null) data.AddRange(dataAdNumber);

            if (dataAdNumber != null && dataAdNumber.Count == 2 && dataAdNumber[1] is CellUnit
                && dataPageNumber != null && dataPageNumber.Count == 2 && dataPageNumber[1] is CellUnit)
                dataTemp = ComputeDataPageRatio(((CellUnit)dataPageNumber[1]).Value, ((CellUnit)dataAdNumber[1]).Value);
            else if (dataAdNumber != null && dataAdNumber.Count == 2 && dataAdNumber[1] is CellUnit)
                dataTemp = ComputeDataPageRatio(-1, ((CellUnit)dataAdNumber[1]).Value);
            else
                dataTemp = ComputeDataPageRatio(-1, -1);
            if (dataTemp != null) data.AddRange(dataTemp);

            dataTemp = ComputeDataAdNumberExcludingInsets(isAlertModule);
            if (dataTemp != null) data.AddRange(dataTemp);

            dataTemp = ComputeDataAdNumberIncludingInsets(isAlertModule);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Cas tv, radio
            dataTemp = ComputeDataSpotNumber(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);

            dataTemp = ComputeDataEcranNumber(dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);

            dataTemp = ComputeDataTotalDuration(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);

            dataTemp = ComputeDataEvaliantInsertionNumber(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Total investissements
            if (dataInvestisment != null) data.AddRange(dataInvestisment);
            #endregion

            #region Nombre de produits
            if (dataProductNumber != null) data.AddRange(dataProductNumber);

            dataTemp = ComputeDataProductNumberInTracking(isAlertModule);
            if (dataTemp != null) data.AddRange(dataTemp);

            dataTemp = ComputeDataProductNumberInVehicle(isAlertModule);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Nombre d'annonceurs
            if (dataAdvertiserNumber != null) data.AddRange(dataAdvertiserNumber);
            #endregion

            #region Cas tv, radio, others
            dataTemp = ComputeDataAverageDurationEcran(dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);

            dataTemp = ComputeDataSpotNumberByEcran(dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #endregion

            return data;

        }
        #endregion

        #endregion
    }


    #region Class Data Unit
    /// <summary>
    /// Data Unit
    /// </summary>
    public class DataUnit {

        #region Variables
        /// <summary>
        /// Data Table
        /// </summary>
        protected DataTable _dt = null;
        /// <summary>
        /// Dal portofolio Synthesis
        /// </summary>
        protected IPortofolioDAL _portofolioDAL = null;
        /// <summary>
        /// Vehicle
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DataUnit(IPortofolioDAL portofolioDAL, VehicleInformation vehicleInformation) {
            _portofolioDAL = portofolioDAL;
            _vehicleInformation = vehicleInformation;
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.investment);
            if(ds!=null && ds.Tables!=null && ds.Tables.Count>0)
                _dt = ds.Tables[0];
        }
        #endregion

        #region Public Methods

        #region GetVolume
        /// <summary>
        /// GetVolume
        /// </summary>
        public virtual string GetVolume() {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.volume) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.volume].Id.ToString())) {
                if (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.volume].Id.ToString()].ToString().Length > 0) {
                    return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.volume].Id.ToString()].ToString());
                }
                else return ("0");
            }
            return string.Empty;
        }
        #endregion

        #region GetNumberBoard
        /// <summary>
        /// GetNumberBoard
        /// </summary>
        public virtual string GetNumberBoard() {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.numberBoard) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.numberBoard].Id.ToString()))
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.numberBoard].Id.ToString()].ToString());
            return string.Empty;
        }
        #endregion

        #region GetAdNumber
        /// <summary>
        /// GetVolume
        /// </summary>
        public virtual string GetAdNumber() {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString()))
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString()].ToString());
            return string.Empty;
        }
        #endregion

        #region GetSpotNumber
        /// <summary>
        /// GetSpotNumber
        /// </summary>
        public virtual string GetSpotNumber() {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.insertion) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString())
                && _dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString().Length > 0)
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
            else if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.spot) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString()))
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString()].ToString());
            else if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.occurence) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString()))
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString()].ToString());
            else
                return string.Empty;
        }
        #endregion

        #region GetTotalDuration
        /// <summary>
        /// GetTotalDuration
        /// </summary>
        public virtual string GetTotalDuration() {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.duration) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()))
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()].ToString());
            return string.Empty;
        }
        #endregion

        #region GetInvestment
        /// <summary>
        /// GetInvestment
        /// </summary>
        public virtual string GetInvestment() {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.euro) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()) && _dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()].ToString().Length > 0)
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()].ToString());
            else
                return ("0");
        }
        #endregion

        #endregion
    }
    #endregion

    #region Class Data Ecran
    /// <summary>
    /// Data Ecran
    /// </summary>
    public class DataEcran {

        #region Variables
        /// <summary>
        /// Data Table
        /// </summary>
        protected DataTable _dt = null;
        /// <summary>
        /// Dal portofolio Synthesis
        /// </summary>
        protected IPortofolioDAL _portofolioDAL = null;
        /// <summary>
        /// Vehicle
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// is Alert Module
        /// </summary>
        protected bool _isAlertModule;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DataEcran(IPortofolioDAL portofolioDAL, VehicleInformation vehicleInformation, bool isAlertModule) {
            _portofolioDAL = portofolioDAL;
            _vehicleInformation = vehicleInformation;
            _isAlertModule = isAlertModule;
            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
                && _isAlertModule) {
                _dt = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberAdBreaks).Tables[0];
            }
            else _dt = null;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Is Alert Module or not
        /// </summary>
        public bool IsAlertModule {
            get { return _isAlertModule; }
        }
        #endregion

        #region Public Methods

        #region GetNumber
        /// <summary>
        /// Get Number of Ecran
        /// </summary>
        public virtual string GetNumber() {
            if (_dt != null) {
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetAverageDuration
        /// <summary>
        /// Get Average Duration of a spot
        /// </summary>
        public virtual decimal GetAverageDuration() {
            if (_dt != null && _dt.Rows[0]["ecran_duration"] != System.DBNull.Value) {
                return (decimal.Parse(_dt.Rows[0]["ecran_duration"].ToString()) / decimal.Parse(_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString()));
            }
            else return 0;
        }
        #endregion

        #region GetSpotNumberByEcran
        /// <summary>
        /// Get Spot Number By Ecran
        /// </summary>
        public virtual decimal GetSpotNumberByEcran() {

            decimal nbrSpotByEcran = 0;
            string nbrEcran = GetNumber();
            if (nbrEcran.Length > 0) {
                if (_dt != null && _dt.Rows[0]["nbre_spot"] != System.DBNull.Value) {
                    nbrSpotByEcran = (decimal.Parse(_dt.Rows[0]["nbre_spot"].ToString()) / decimal.Parse(_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString()));
                }
                else nbrSpotByEcran = 0;
            }
            return nbrSpotByEcran;
        }
        #endregion

        #endregion
    }
    #endregion


}
