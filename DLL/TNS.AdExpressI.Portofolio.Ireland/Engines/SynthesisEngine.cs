using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using AbstractResult = TNS.AdExpressI.Portofolio.Engines;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.Ireland.Engines {
    /// <summary>
    /// Ireland synthesis engine
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

        #region Compute Data Investissements Total
        /// <summary>
        /// Compute Data Investissements Total
        /// </summary>
        /// <param name="dataUnit"></param>
        /// <returns></returns>
        protected override List<ICell> ComputeDataInvestissementsTotal(AbstractResult.DataUnit dataUnit) {

            #region Variables
            List<ICell> data = null;
            string investment = string.Empty;
            UnitInformation defaultCurrency = UnitsInformation.List[UnitsInformation.DefaultCurrency];
            #endregion

            #region Get Data
            investment = dataUnit.GetInvestment();
            #endregion

            #region Compute data
            if (investment != null && investment.Length > 0 && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile) {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2787, _webSession.SiteLanguage) + " (" + defaultCurrency.GetUnitWebText(_webSession.SiteLanguage) + ")"));
                CellEuro cE = new CellEuro(double.Parse(investment));
                cE.StringFormat = UnitsInformation.Get(defaultCurrency.Id).StringFormat;
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

        protected override List<ICell> ComputeDataPeriodicity() {
            return null;
        }
        #endregion

    }
}
