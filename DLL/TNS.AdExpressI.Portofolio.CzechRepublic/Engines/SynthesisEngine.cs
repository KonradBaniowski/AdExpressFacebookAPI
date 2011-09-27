#region Information
// Author: A. Rousseau
// Creation date: 06/07/2009
// Modification date:
#endregion

#region Using
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using AbstractResult = TNS.AdExpressI.Portofolio.Engines;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Translation;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
#endregion

namespace TNS.AdExpressI.Portofolio.CzechRepublic.Engines {
    /// <summary>
    /// CzechRepublic synthesis engine
    /// </summary>
    public class SynthesisEngine : AbstractResult.SynthesisEngine {

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

        #region Protected Methods Override
        /// <summary>
        /// Compute Data product Number in Tracking
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataProductNumberInTracking(bool isAlertModule) {
            return null;
        }
        /// <summary>
        /// Compute Data product Number in Vehicle
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataProductNumberInVehicle(bool isAlertModule) {
            return null;
        }
        /// <summary>
        /// Compute Data Average Duration Ecran (not display because the field cobranding is always at 0 for tv and radio)
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataAverageDurationEcran(bool isAlertModule) {
            return null;
        }
        /// <summary>
        /// Compute Data Spot Number By Ecran (not display because the field cobranding is always at 0 for tv and radio)
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataSpotNumberByEcran(bool isAlertModule) {
            return null;
        }
        //_displayTypeSale
        protected override List<ICell> ComputeDataNetworkType(bool isAlertModule) {
            return null;
        }

        #region ComputeDataInvestissementsTotal
        /// <summary>
        /// Compute Data investment Total (override for evaliant SK)
        /// </summary>
        /// <returns></returns>
        protected override List<ICell> ComputeDataInvestissementsTotal() {

            #region Variables
            List<ICell> data = null;
            string investment = string.Empty;
            UnitInformation defaultCurrency = UnitsInformation.List[UnitsInformation.DefaultCurrency];
            #endregion

            #region Get Data
            investment = GetInvestment(GetDataInvestment());
            #endregion

            #region Compute data
            if (investment != null && investment.Length > 0 && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile) {
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

        #region SpotNumberGetData
        /// <summary>
        /// Spot Number Get Data
        /// </summary>
        /// <returns>Spot Number</returns>
        protected override string SpotNumberGetData(){
            return GetSpotNumber(GetDataInvestment());
        }
        #endregion

        #region EcranNumberGetData
        /// <summary>
        /// Ecran Number Get Data
        /// </summary>
        /// <param name="isAlertModule">Is Alert Module</param>
        /// <returns>Ecran Number</returns>
        protected override string EcranNumberGetData(bool isAlertModule) {
            return GetEcranNumber(GetDataEncartData(isAlertModule));
        }
        #endregion

        #region TotalDurationGetData
        /// <summary>
        /// Total Duration Get Data
        /// </summary>
        /// <returns>Total Duration</returns>
        protected override string TotalDurationGetData() {
            return GetTotalDuration(GetDataInvestment());
        }
        #endregion

        #region GetSpotNumber
        /// <summary>
        /// GetSpotNumber
        /// </summary>
        protected override string GetSpotNumber(DataTable dt) {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.insertion) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString())
                && dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString().Length > 0)
                return (dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
            else if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.spot) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString()))
                return (dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString()].ToString());
            else if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.occurence) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString()))
                return (dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString()].ToString());
            else
                return string.Empty;
        }
        #endregion

        #region GetEcranNumber
        /// <summary>
        /// GetEcranNumber
        /// </summary>
        protected override string GetEcranNumber(DataTable dt) {
            if (dt != null) {
                return (dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
            }
            else return string.Empty;
        }
        #endregion

        #region GetTotalDuration
        /// <summary>
        /// GetTotalDuration
        /// </summary>
        protected override string GetTotalDuration(DataTable dt) {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.duration) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()))
                return (dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()].ToString());
            return string.Empty;
        }
        #endregion

        #endregion

    }
}
