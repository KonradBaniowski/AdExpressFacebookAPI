#region Information
// Author: A. Rousseau
// Creation date: 06/07/2009
// Modification date:
#endregion

#region Using
using System;
using System.Collections.Generic;

using AbstractResult = TNS.AdExpressI.Portofolio.Engines;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpressI.Portofolio.DAL;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Translation;

using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.Portofolio.Engines;
#endregion

namespace TNS.AdExpressI.Portofolio.CzechRepublic.Engines
{
    /// <summary>
    /// CzechRepublic synthesis engine
    /// </summary>
    public class SynthesisEngine : AbstractResult.SynthesisEngine
    {

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
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
        }
        #endregion

        #region Protected Methods Override
        /// <summary>
        /// Compute Data product Number in Tracking
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataProductNumberInTracking(bool isAlertModule)
        {
            return null;
        }
        /// <summary>
        /// Compute Data product Number in Vehicle
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataProductNumberInVehicle(bool isAlertModule)
        {
            return null;
        }
        /// <summary>
        /// Compute Data Average Duration Ecran (not display because the field cobranding is always at 0 for tv and radio)
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataAverageDurationEcran(DataEcran dataEcran) {
            return null;
        }
        /// <summary>
        /// Compute Data Spot Number By Ecran (not display because the field cobranding is always at 0 for tv and radio)
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataSpotNumberByEcran(DataEcran dataEcran) {
            return null;
        }
        //_displayTypeSale
        protected override List<ICell> ComputeDataNetworkType(bool isAlertModule)
        {
            return null;
        }

        #region ComputeDataInvestissementsTotal
        /// <summary>
        /// Compute Data investment Total (override for evaliant SK)
        /// </summary>
        /// <returns></returns>
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
            if (investment != null && investment.Length > 0 && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile)
            {
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

        #endregion

    }
    #region Class Data Unit
    /// <summary>
    /// Data Unit
    /// </summary>
    public class DataUnit : AbstractResult.DataUnit
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DataUnit(IPortofolioDAL portofolioDAL, VehicleInformation vehicleInformation)
            : base(portofolioDAL, vehicleInformation)
        {
        }
        #endregion
    }
    #endregion
}
