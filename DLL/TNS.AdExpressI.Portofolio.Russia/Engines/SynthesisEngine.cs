#region Information
// Author: D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Text;
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
using TNS.AdExpressI.Portofolio.Engines;
using TNS.AdExpress.Web.Core.Result;

namespace TNS.AdExpressI.Portofolio.Russia.Engines {
	/// <summary>
	/// Compute portofolio synthesis' results
	/// </summary>
    public class SynthesisEngine : TNS.AdExpressI.Portofolio.Engines.SynthesisEngine {

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
		}
		#endregion

        #region Compute Data

        #region ComputeDataPeriodIssue
        protected override List<ICell> ComputeDataPeriodIssue(bool isAlertModule)
        {

            #region Variables
            string firstDate = string.Empty;
            string lastDate = string.Empty;
            DateTime dtFirstDate = DateTime.Today;
            DateTime dtLastDate = DateTime.Today;
            List<ICell> data = null;
            #endregion

            #region Get Data
            GetDataPeriodSelected(isAlertModule, out firstDate, out lastDate);
            #endregion

            #region Compute data
            dtFirstDate = new DateTime(Convert.ToInt32(firstDate.Substring(0, 4)), Convert.ToInt32(firstDate.Substring(4, 2)), Convert.ToInt32(firstDate.Substring(6, 2)));
            dtLastDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
            data = new List<ICell>(2);
            data.Add(new CellLabel(GestionWeb.GetWebWord(1541, _webSession.SiteLanguage)));
            if (firstDate != null && firstDate.Length > 0 && lastDate != null && lastDate.Length > 0 && firstDate.Equals(lastDate)){
                data.Add(new CellLabel(Dates.DateToString(dtFirstDate, _webSession.SiteLanguage)));
            }
            else{
                data.Add(new CellLabel(GestionWeb.GetWebWord(896, _webSession.SiteLanguage) + " " + Dates.DateToString(dtFirstDate, _webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(1730, _webSession.SiteLanguage) + " " + Dates.DateToString(dtLastDate, _webSession.SiteLanguage)));
            }
            #endregion

            return data;

        }
        #endregion

        #region GetDataPeriodSelected
        /// <summary>
        /// GetDataPeriodSelected
        /// </summary>
        protected override void GetDataPeriodSelected(bool isAlertModule, out string firstDate, out string lastDate)
        {
            firstDate = string.Empty;
            lastDate = string.Empty;
            if (isAlertModule)
            {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.periodSelected);
                DataTable dt = ds.Tables[0];
                if (dt.Columns.Contains("first_date")) firstDate = dt.Rows[0]["first_date"].ToString();
                if (dt.Columns.Contains("last_date")) lastDate = dt.Rows[0]["last_date"].ToString();
            }
            else
            {
                firstDate = TNS.FrameWork.Date.DateString.DateTimeToYYYYMMDD(WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType));
                lastDate = TNS.FrameWork.Date.DateString.DateTimeToYYYYMMDD(WebFunctions.Dates.getPeriodEndDate(_periodEnd, _webSession.PeriodType));

            }
        }
        #endregion

        #region ComputeDataInvestissementsTotal
        /// <summary>
        /// Compute Data Investissements Total
        /// </summary>
        /// <param name="dataUnit">data Unit</param>
        /// <returns>List of cells</returns>
        protected override List<ICell> ComputeDataInvestissementsTotal(TNS.AdExpressI.Portofolio.Engines.DataUnit dataUnit) {

            #region Variables
            List<ICell> data = null;
            string investmentUSD = string.Empty;
            string investmentRubled = string.Empty;
            #endregion

            if (dataUnit!= null && dataUnit is DataUnit)
            {

                #region Get Data
                investmentUSD = ((DataUnit)dataUnit).GetInvestmentUSD();
                investmentRubled = ((DataUnit)dataUnit).GetInvestmentRubled();
                #endregion

                #region Compute data
                if (investmentUSD != null && investmentUSD.Length > 0) {
                    if (data == null) data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(2749, _webSession.SiteLanguage)));
                    CellEuro cE = new CellEuro(double.Parse(investmentUSD));
                    cE.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.usd).StringFormat;
                    data.Add(cE);
                }

                if (investmentRubled != null && investmentRubled.Length > 0) {
                    if (data == null) data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(2748, _webSession.SiteLanguage)));
                    CellEuro cE = new CellEuro(double.Parse(investmentRubled));
                    cE.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.rubles).StringFormat;
                    data.Add(cE);
                }
                #endregion

            }

            return data;

        }
        #endregion

        #region ComputeDataSponsorshipNumber
        /// <summary>
        /// Compute Data Sponsorship Number
        /// </summary>
        /// <returns></returns>
        protected virtual List<ICell> ComputeDataSponsorshipNumber() {

            #region Variables
            List<ICell> data = null;
            string numberSponsorship = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                )
            {
                #region Get Data
                numberSponsorship = GetDataSponsorshipNumber();
                #endregion
                
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2745, _webSession.SiteLanguage)));
                CellNumber cN6 = new CellNumber(double.Parse(numberSponsorship));
                cN6.StringFormat = UNIT_FORMAT;
                data.Add(cN6);

            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataPageNumberWithSelfPromotion
        protected virtual List<ICell> ComputeDataPageNumberWithSelfPromotion()
        {

            #region Variables
            List<ICell> data = null;
            string PageNumberWithSelfPromotion = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press)
            {

                #region Get Data
                PageNumberWithSelfPromotion = GetDataPageNumberWithSelfPromotion();
                #endregion

                if (PageNumberWithSelfPromotion != null && PageNumberWithSelfPromotion.Length > 0)
                {
                    // Nombre de page
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(2762, _webSession.SiteLanguage)));
                    CellRussiaPage cN2 = new CellRussiaPage(double.Parse(PageNumberWithSelfPromotion));
                    cN2.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat; 
                    
                    data.Add(cN2);
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataMarket
        /// <summary>
        /// Compute Data Market
        /// </summary>
        /// <returns></returns>
        protected virtual List<ICell> ComputeDataMarket() {

            #region Variables
            List<ICell> data = null;
            string market = string.Empty;
            #endregion

            #region Get Data
            market = GetDataMarket();
            #endregion

            #region Compute data
            data = new List<ICell>(2);
            data.Add(new CellLabel(GestionWeb.GetWebWord(2746, _webSession.SiteLanguage)));
            data.Add(new CellLabel(market));
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataHolding
        /// <summary>
        /// Compute Data Holding
        /// </summary>
        /// <returns></returns>
        protected virtual List<ICell> ComputeDataHolding() {

            #region Variables
            List<ICell> data = null;
            string holding = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship)
            {

                // Get Data
                holding = GetDataHolding();

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2747, _webSession.SiteLanguage)));
                data.Add(new CellLabel(holding));
            }
            #endregion

            return data;

        }
        #endregion       

        #region ComputeDataPublishingHouse
        /// <summary>
        /// Compute Data Publishing House
        /// </summary>
        /// <returns>ICells list</returns>
        protected virtual List<ICell> ComputeDataPublishingHouse(){

            #region Variables
            List<ICell> data = null;
            string publishingHouse = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press){

                // Get Data
                publishingHouse = GetDataPublishingHouse();

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2656, _webSession.SiteLanguage)));
                data.Add(new CellLabel(publishingHouse));
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataSyndicate
        /// <summary>
        /// Compute Data Syndicate
        /// </summary>
        /// <returns>ICells list</returns>
        protected virtual List<ICell> ComputeDataSyndicate(){

            #region Variables
            List<ICell> data = null;
            string syndicate = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press){

                // Get Data
                syndicate = GetDataSyndicate();

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2657, _webSession.SiteLanguage)));
                data.Add(new CellLabel(syndicate));
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataEditionType
        /// <summary>
        /// Compute Data Edition Type
        /// </summary>
        /// <returns>ICells list</returns>
        protected virtual List<ICell> ComputeDataEditionType(){

            #region Variables
            List<ICell> data = null;
            string editionType = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press){

                // Get Data
                editionType = GetDataEditionType();

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2659, _webSession.SiteLanguage)));
                data.Add(new CellLabel(editionType));
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataIssueFormat
        /// <summary>
        /// Compute Data Issue Format
        /// </summary>
        /// <returns>ICells list</returns>
        protected virtual List<ICell> ComputeDataIssueFormat(){

            #region Variables
            List<ICell> data = null;
            string issueFormat = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press){

                // Get Data
                issueFormat = GetDataIssueFormat();

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2761, _webSession.SiteLanguage)));
                data.Add(new CellLabel(issueFormat));
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataBrandNumber
        /// <summary>
        /// Compute Data Brand Number
        /// </summary>
        /// <returns></returns>
        protected virtual List<ICell> ComputeDataBrandNumber() {

            #region Variables
            List<ICell> data = null;
            string numberBrand = string.Empty;
            #endregion

            #region Get Data
            numberBrand = GetDataBrandNumber();
            #endregion

            #region Compute data
            data = new List<ICell>(2);
            data.Add(new CellLabel(GestionWeb.GetWebWord(2742, _webSession.SiteLanguage)));
            CellNumber cN6 = new CellNumber(double.Parse(numberBrand));
            cN6.StringFormat = UNIT_FORMAT;
            data.Add(cN6);
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataSubBrandNumber
        /// <summary>
        /// Compute Data Sub Brand Number
        /// </summary>
        /// <returns></returns>
        protected virtual List<ICell> ComputeDataSubBrandNumber() {

            #region Variables
            List<ICell> data = null;
            string numberSubBrand = string.Empty;
            #endregion

            #region Get Data
            numberSubBrand = GetDataSubBrandNumber();
            #endregion

            #region Compute data
            data = new List<ICell>(2);
            data.Add(new CellLabel(GestionWeb.GetWebWord(2743, _webSession.SiteLanguage)));
            CellNumber cN6 = new CellNumber(double.Parse(numberSubBrand));
            cN6.StringFormat = UNIT_FORMAT;
            data.Add(cN6);
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataSpotNumberWithSelfPromotion
        /// <summary>
        /// Compute Data Spot Number With Self Promotion
        /// </summary>
        /// <param name="dataUnit">dataUnit object</param>
        /// <returns>List of Data Spot Number With Self Promotion</returns>
        protected virtual List<ICell> ComputeDataSpotNumberWithSelfPromotion(DataUnit dataUnit) {

            #region Variables
            List<ICell> data = null;
            string nbrSpot = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others) {

                #region Get Data
                nbrSpot = dataUnit.GetSpotNumberWithSelfPromotion();
                #endregion

                //Nombre de spot
                if (nbrSpot.Length == 0) {
                    nbrSpot = "0";
                }
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2744, _webSession.SiteLanguage)));
                CellNumber cN3 = new CellNumber(double.Parse(nbrSpot));
                cN3.StringFormat = UNIT_FORMAT;
                data.Add(cN3);
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAverageDurationEcran
        protected override List<ICell> ComputeDataAverageDurationEcran(TNS.AdExpressI.Portofolio.Engines.DataEcran dataEcran)
        {

            #region Variables
            List<ICell> data = null;
            decimal averageDurationEcran = 0;
            string nbrEcran = null;
            #endregion

            #region Get Data
            if (dataEcran != null)
            {
                nbrEcran = dataEcran.GetNumber();
                if (nbrEcran.Length > 0)
                {
                    averageDurationEcran = dataEcran.GetAverageDuration();
                }
            }
            #endregion

            #region Compute data
            if (nbrEcran != null && nbrEcran.Length > 0
                && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship)
                && dataEcran.IsAlertModule)
            {
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

        #region ComputeDataRegion
        /// <summary>
        /// Compute Data Region
        /// </summary>
        /// <returns>ICells list</returns>
        protected virtual List<ICell> ComputeDataRegion()
        {

            #region Variables
            List<ICell> data = null;
            string region = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor)
            {

                // Get Data
                region = GetDataRegion();

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2652, _webSession.SiteLanguage)));
                data.Add(new CellLabel(region));
            }
            #endregion

            return data;

        }
        #endregion      

        #region ComputeDataNetwork
        /// <summary>
        /// Compute Data Network
        /// </summary>
        /// <returns>ICells list</returns>
        protected virtual List<ICell> ComputeDataNetwork()
        {

            #region Variables
            List<ICell> data = null;
            string network = string.Empty;
            #endregion

            #region Compute data
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor)
            {

                // Get Data
                network = GetDataNetwork();

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2651, _webSession.SiteLanguage)));
                data.Add(new CellLabel(network));
            }
            #endregion

            return data;

        }
        #endregion

        #endregion

        #region Get Data

        #region GetDataSponsorshipNumber
        /// <summary>
        /// Get Data Sponsorship Number
        /// </summary>
        protected virtual string GetDataSponsorshipNumber() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberSponsorship);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["nbLines"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataPageNumberWithSelfPromotion
        /// <summary>
        /// Data Page Number With Self Promotion
        /// </summary>
        protected virtual string GetDataPageNumberWithSelfPromotion()
        {
            string pageNumberSelfPromo = string.Empty;
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages))
            {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.pageSelfPromo);
                DataTable dt = ds.Tables[0];
                pageNumberSelfPromo = dt.Rows[0]["nbLines"].ToString();
                if (pageNumberSelfPromo.Length == 0)
                    pageNumberSelfPromo = "0";
            }
            return pageNumberSelfPromo;
        }
        #endregion

        #region GetDataMarket
        /// <summary>
        /// Get Data Market
        /// </summary>
        protected virtual string GetDataMarket() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.market);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["market"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataHolding
        /// <summary>
        /// Get Data Holding
        /// </summary>
        protected virtual string GetDataHolding() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.holding);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["holding"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataRegion
        /// <summary>
        /// Get Data Region
        /// </summary>
        protected virtual string GetDataRegion() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.region);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["region"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataPublishingHouse
        /// <summary>
        /// Get Data Publishing House
        /// </summary>
        protected virtual string GetDataPublishingHouse(){
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.publishingHouse);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["publishingHouse"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataSyndicate
        /// <summary>
        /// Get Data Syndicate
        /// </summary>
        protected virtual string GetDataSyndicate(){
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.syndicate);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["syndicate"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataEditionType
        /// <summary>
        /// Get Data Edition Type
        /// </summary>
        protected virtual string GetDataEditionType(){
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.editionType);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["editionType"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataIssueFormat
        /// <summary>
        /// Get Data Issue Format
        /// </summary>
        protected virtual string GetDataIssueFormat() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.issueFormat);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["issueFormat"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataBrandNumber
        /// <summary>
        /// GetDataProductNumber
        /// </summary>
        protected virtual string GetDataBrandNumber() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberBrand);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["nbLines"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetDataSubBrandNumber
        /// <summary>
        /// GetDataProductNumber
        /// </summary>
        protected virtual string GetDataSubBrandNumber() {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberSubBrand);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["nbLines"].ToString());
            }
            else return string.Empty;
        }
        #endregion      

        #region GetDataNetwork
        /// <summary>
        /// Get Data Network
        /// </summary>
        protected virtual string GetDataNetwork(){
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.network);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0) {
                DataTable dt = ds.Tables[0];
                return (dt.Rows[0]["network"].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region ComputeDataBoardNumber
        protected virtual List<ICell> ComputeDataBoardNumber(DataUnit dataUnit)
        {

            #region Variables
            List<ICell> data = null;
            string boardNumber = string.Empty;
            #endregion

            #region Compute data
            if (dataUnit!=null && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor)) {

                #region Get Data
                boardNumber = dataUnit.GetNumberBoard();
                #endregion

                if (boardNumber != null && boardNumber.Length > 0)
                {
                    // Number of boards		
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1604, _webSession.SiteLanguage)));
                    CellInsertion cP = new CellInsertion(double.Parse(boardNumber));                    
                    cP.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.numberBoard).StringFormat;
                    data.Add(cP);
                }
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAdNumber
        protected override List<ICell> ComputeDataAdNumber(TNS.AdExpressI.Portofolio.Engines.DataUnit dataUnit)
        {

            #region Variables
            List<ICell> data = null;
            string adNumber = string.Empty;
            #endregion

            #region Compute data
            if (dataUnit != null && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress))
            {

                #region Get Data
                adNumber = dataUnit.GetAdNumber();
                #endregion

                if (adNumber != null && adNumber.Length > 0)
                {
                    // Nb de page pub		
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(GestionWeb.GetWebWord(1386, _webSession.SiteLanguage)));
                    CellRussiaPage cP = new CellRussiaPage(double.Parse(adNumber));
                    cP.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat;
                    data.Add(cP);
                }
            }
            #endregion

            return data;

        }
        #endregion

        #endregion

        #region GetDataCells
        /// <summary>
        /// Get Data Cell List
        /// </summary>
        /// <returns>Data Cell List</returns>
        protected override List<ICell> GetDataCellList() {

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

            #region Get Priority Data

            DataUnit dataUnit = new DataUnit(_portofolioDAL, _vehicleInformation);

            dataInvestisment = ComputeDataInvestissementsTotal(dataUnit);
            dataProductNumber = ComputeDataProductNumber();
            dataAdvertiserNumber = ComputeDataAdvertiserNumber();

            #region No Data
            if ((dataInvestisment == null || dataInvestisment.Count<=0 || (dataInvestisment.Count%2)!=0 || !(dataInvestisment[1] is CellNumber) || ((CellNumber)dataInvestisment[1]).Value <= 0)
                && (dataProductNumber == null || dataProductNumber.Count != 2 || !(dataProductNumber[1] is CellNumber) || ((CellNumber)dataProductNumber[1]).Value <= 0)
                && (dataAdvertiserNumber == null || dataAdvertiserNumber.Count != 2 || !(dataAdvertiserNumber[1] is CellNumber) || ((CellNumber)dataAdvertiserNumber[1]).Value <= 0))
                return null;
            #endregion

            if (_vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioMusic
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvSponsorship
                ) {
                dataEcran = new DataEcran(_portofolioDAL, _vehicleInformation, isAlertModule);
            }

            #endregion

            #region Period Selected
            data = ComputeDataPeriodIssue(isAlertModule);
            #endregion

            #region Market
            dataTemp = ComputeDataMarket();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Holding
            dataTemp = ComputeDataHolding();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Region
            dataTemp = ComputeDataRegion();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Publishing house
            dataTemp = ComputeDataPublishingHouse();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Syndicate
            dataTemp = ComputeDataSyndicate();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Edition Type
            dataTemp = ComputeDataEditionType();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Issue format
            dataTemp = ComputeDataIssueFormat();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Number of pages
            dataTemp = ComputeDataPageNumber();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Number of pages of advertisements
            dataTemp = ComputeDataAdNumber(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Number of pages with self promo
            dataTemp = ComputeDataPageNumberWithSelfPromotion();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region No of Spots without Self Promotion
            dataTemp = ComputeDataSpotNumber(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Number of SponsorShip
            dataTemp = ComputeDataSponsorshipNumber();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region No. of Spots with Self Promotion
            dataTemp = ComputeDataSpotNumberWithSelfPromotion(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion         

            #region Network
            dataTemp = ComputeDataNetwork();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Number of boards
            dataTemp = ComputeDataBoardNumber(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Total duration
            dataTemp = ComputeDataTotalDuration(dataUnit);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Total Ad Spends in Rub & USD
            if (dataInvestisment != null) data.AddRange(dataInvestisment);
            #endregion

            #region Number of AdBreak
            dataTemp = ComputeDataEcranNumber(dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Number of Sub Brands
            dataTemp = ComputeDataSubBrandNumber();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Number of Brands
            dataTemp = ComputeDataBrandNumber();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Nombre de produits
            if (dataProductNumber != null) data.AddRange(dataProductNumber);
            #endregion

            #region Nombre d'annonceurs
            if (dataAdvertiserNumber != null) data.AddRange(dataAdvertiserNumber);
            #endregion

            #region Average Duration of a spot
            dataTemp = ComputeDataAverageDurationEcran(dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion         

            return data;

        }
        #endregion

        //#region HTML for vehicle view
        ///// <summary>
        ///// Get view of the vehicle (HTML)
        ///// </summary>
        ///// <param name="excel">True for excel result</param>
        ///// <param name="resultType">Result Type (Synthesis, MediaDetail)</param>
        ///// <returns>HTML code</returns>
        //public override string GetVehicleViewHtml(bool excel, int resultType)
        //{
        //    return Engines.CommonFunction.GetVehicleViewHtml(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, excel, resultType);
        //}
        //#endregion

    }

    #region Class Data Unit
    /// <summary>
    /// Data Unit
    /// </summary>
    public class DataUnit:TNS.AdExpressI.Portofolio.Engines.DataUnit {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DataUnit(IPortofolioDAL portofolioDAL, VehicleInformation vehicleInformation)
            : base(portofolioDAL, vehicleInformation) {
        }
        #endregion

        #region Public Methods

        #region GetVolume
        /// <summary>
        /// GetVolume
        /// </summary>
        public override string GetVolume() {
            throw new NotImplementedException("This method is not implemented");
        }
        #endregion

        #region GetSpotNumber
        /// <summary>
        /// Get Spot Number Without self promotion
        /// </summary>
        public override string GetSpotNumber() {
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

        #region GetSpotNumberWithSelfPromotion
        /// <summary>
        /// Get Spot Number With Self promotion
        /// </summary>
        public virtual string GetSpotNumberWithSelfPromotion() {

            string withSelfPromotion = "_WITH_SP";

            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.insertion) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString() + withSelfPromotion)
                && _dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString() + withSelfPromotion].ToString().Length > 0)
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString() + withSelfPromotion].ToString());
            else if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.spot) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString() + withSelfPromotion))
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString() + withSelfPromotion].ToString());
            else if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.occurence) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString() + withSelfPromotion))
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString() + withSelfPromotion].ToString());
            else
                return string.Empty;
        }
        #endregion

        #region GetTotalDuration
        /// <summary>
        /// Get Total Duration
        /// </summary>
        public override string GetTotalDuration() {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.duration) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()))
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()].ToString());
            return string.Empty;
        }
        #endregion

        #region GetInvestment
        /// <summary>
        /// GetInvestment
        /// </summary>
        public override string GetInvestment() {
            throw new NotImplementedException("This method is not implemented");
        }
        #endregion

        #region GetInvestmentRubled
        /// <summary>
        /// Get Investment USD
        /// </summary>
        public virtual string GetInvestmentRubled() {

            string data = string.Empty;
            if (_dt != null && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile) {
                if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.rubles) && _dt.Columns.Contains(WebCst.CustomerSessions.Unit.rubles.ToString())) {
                    data = _dt.Rows[0][WebCst.CustomerSessions.Unit.rubles.ToString()].ToString();
                }
            }
            else data = null;

            return data;
            
        }      
        #endregion

        #region GetInvestmentUSD
        /// <summary>
        /// Get Investment USD
        /// </summary>
        public virtual string GetInvestmentUSD() {

            string data = string.Empty;
            if (_dt != null && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile) {

                if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.usd) && _dt.Columns.Contains(WebCst.CustomerSessions.Unit.usd.ToString())) {
                    data = _dt.Rows[0][WebCst.CustomerSessions.Unit.usd.ToString()].ToString();
                }

            }
            else data = null;


            return data;

        }
        #endregion

        #endregion

    }
    #endregion

    #region Class Data Ecran
    /// <summary>
    /// Data Ecran
    /// </summary>
    public class DataEcran:TNS.AdExpressI.Portofolio.Engines.DataEcran {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DataEcran(IPortofolioDAL portofolioDAL, VehicleInformation vehicleInformation, bool isAlertModule):base(portofolioDAL, vehicleInformation, isAlertModule) {

        }
        #endregion

        #region Public Methods

        #region GetNumber
        /// <summary>
        /// Get Number of Ecran
        /// </summary>
        public override string GetNumber() {
            if (_dt != null)
            {
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetAverageDuration
        /// <summary>
        /// Get Average Duration of a spot
        /// </summary>
        public override decimal GetAverageDuration()
        {
            if (_dt != null && _dt.Rows[0]["ecran_duration"] != System.DBNull.Value)
            {
                return (decimal.Parse(_dt.Rows[0]["ecran_duration"].ToString()) / decimal.Parse(_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString()));
            }
            else return 0;
        }
        #endregion


        #region GetSpotNumberByEcran
        /// <summary>
        /// Get Average number of spots per ad break
        /// </summary>
        public override decimal GetSpotNumberByEcran() {
            throw new NotImplementedException("This method is not implemented");
        }
        #endregion

        #endregion

    }
    #endregion

}
