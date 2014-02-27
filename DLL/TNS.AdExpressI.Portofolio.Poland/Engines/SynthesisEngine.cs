#region Information
// Author: Y. R'kaina
// Creation date: 25/11/2008
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

namespace TNS.AdExpressI.Portofolio.Poland.Engines {
    /// <summary>
    /// Poland synthesis engine
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

       

        #region ComputeDataInvestissementsTotal
        protected override List<ICell> ComputeDataInvestissementsTotal(AbstractResult.DataUnit dataUnit)
        {

            #region Variables
            List<ICell> data = null;
            string investment = string.Empty;
            UnitInformation defaultCurrency = UnitsInformation.List[UnitsInformation.DefaultCurrency];
            #endregion

            #region Get Data
            investment = dataUnit.GetInvestment();
            #endregion

            #region Compute data
            if (investment != null && investment.Length > 0 && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile)
            {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2787, _webSession.SiteLanguage) + " (" + defaultCurrency.GetUnitWebText(_webSession.SiteLanguage) + ")"));
                CellEuro cE = new CellEuro(double.Parse(investment));
                cE.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pln).StringFormat;
                cE.CssClass = "left";
                data.Add(cE);
            }
            #endregion

            return data;

        }
        #endregion

        //_displayNewProductNumber
        protected override List<ICell> ComputeDataProductNumberInTracking(bool isAlertModule) {
            return null;
        }

        protected override List<ICell> ComputeDataProductNumberInVehicle(bool isAlertModule) {
            return null;
        }
        //_displayTypeSale
        protected override List<ICell> ComputeDataNetworkType(bool isAlertModule) {
            return null;
        }
        //_displayPageNumber
        protected override List<ICell> ComputeDataPageNumber() {
            return null;
        }

        protected override List<ICell> ComputeDataPageRatio(double pageNumber, double adNumber) {
            return null;
        }
        #endregion

    }

     #region Class Data Unit
    /// <summary>
    /// Data Unit
    /// </summary>
    public class DataUnit: AbstractResult.DataUnit 
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DataUnit(IPortofolioDAL portofolioDAL, VehicleInformation vehicleInformation)
            :base( portofolioDAL,  vehicleInformation)
        {         
        }
        #endregion

        #region GetInvestment
        /// <summary>
        /// GetInvestment
        /// </summary>
        public override string GetInvestment()
        {
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pln) && _dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.pln].Id.ToString()) && _dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pln].Id.ToString()].ToString().Length > 0)
                return (_dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pln].Id.ToString()].ToString());
            else
                return ("0");
        }
        #endregion

    }
     #endregion
}
