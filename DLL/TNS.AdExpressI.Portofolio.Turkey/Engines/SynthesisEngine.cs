using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.FrameWork.WebResultUI;
using AbstractResult = TNS.AdExpressI.Portofolio.Engines;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.Turkey.Engines
{
    public class SynthesisEngine : AbstractResult.SynthesisEngine
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">VehicleInformation</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
        }
        #endregion

        #region GetDataCells
        /// <summary>
        /// Get Data Cell List
        /// </summary>
        /// <returns>Data Cell List</returns>
        protected override List<ICell> GetDataCellList()
        {
            List<ICell> data = base.GetDataCellList();
            List<ICell> dataTemp = null;

            #region Total Data Spot Number By Ecran
            dataTemp = ComputeTotalDataSpotNumberByEcran(_dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Total Duration spot by Ecran
            dataTemp = ComputeDataTotalDurationEcran(_dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            

            return data;
        }
        #endregion

        #region ComputeDataCategory
        protected override List<ICell> ComputeDataCategory()
        {

            #region Variables
            string category = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Get Data
            category = GetDataCategory();
            #endregion

            #region Compute data
            if (category.Length > 0)
            {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(1382, _webSession.SiteLanguage)));
                data.Add(new CellLabel(category));
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataMediaType
        protected override List<ICell> ComputeDataMediaType()
        {

            #region Variables
            string subMedia = string.Empty;
            List<ICell> data = null;

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv)
                subMedia = GestionWeb.GetWebWord(1838, _webSession.SiteLanguage);
            #endregion
            
            #region Compute data
            if (subMedia.Length > 0)
            {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(3214, _webSession.SiteLanguage)));
                data.Add(new CellLabel(subMedia));
            }
            #endregion

            return data;
        }
        #endregion

        #region ComputeDataMediaOwner
        protected override List<ICell> ComputeDataMediaOwner()
        {

            #region Variables
            string mediaOwner = string.Empty;
            List<ICell> data = null;
            #endregion

            #region Get Data
            mediaOwner = GetDataMediaOwner();
            #endregion

            #region Compute data
            if (mediaOwner.Length > 0)
            {
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(3145, _webSession.SiteLanguage)));
                data.Add(new CellLabel(mediaOwner));
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataInvestissementsTotal
        /// <summary>
        /// Compute Data Investissements Total
        /// </summary>
        /// <param name="dataUnit">data Unit</param>
        /// <returns>List of cells</returns>
        protected override List<ICell> ComputeDataInvestissementsTotal(AbstractResult.DataUnit dataUnit)
        {

            #region Variables
            List<ICell> data = null;
            List<ICell> allData = new List<ICell>();
            string investment = string.Empty;
            UnitInformation defaultCurrency = UnitsInformation.List[UnitsInformation.DefaultCurrency];
            List<WebCst.CustomerSessions.Unit> units = new List<WebCst.CustomerSessions.Unit> { WebCst.CustomerSessions.Unit.tl, WebCst.CustomerSessions.Unit.euro, WebCst.CustomerSessions.Unit.usd };
            #endregion

            foreach (var unit in units)
            {
                #region Get Data
                investment = dataUnit.GetInvestmentByCurrency(unit);
                #endregion

                #region Compute data

                if (dataUnit != null && investment != null && investment.Length > 0)
                {
                    data = new List<ICell>(2);
                    data.Add(new CellLabel(string.Format("{0} ({1})",
                        GestionWeb.GetWebWord(2787, _webSession.SiteLanguage),
                        UnitsInformation.Get(unit).GetUnitWebText(_webSession.SiteLanguage))));
                    CellEuro cE = new CellEuro(double.Parse(investment));
                    cE.StringFormat = UnitsInformation.Get(unit).StringFormat;
                    cE.CssClass = "left";
                    data.Add(cE);
                    allData.AddRange(data);
                }

                #endregion
            }

            return allData;

        }
        #endregion

        #region ComputeCommercialItemNumber
        protected override List<ICell> ComputeCommercialItemNumber()
        {

            #region Variables
            int commercialItemNumber = 0;
            List<ICell> data = null;
            #endregion

            #region Get Data
            commercialItemNumber = GetCommercialItemNumber();
            #endregion

            #region Compute data
            data = new List<ICell>(2);
            data.Add(new CellLabel(GestionWeb.GetWebWord(3187, _webSession.SiteLanguage)));
            data.Add(new CellNumber(commercialItemNumber));
            #endregion

            return data;

        }
        #endregion

        #region ComputeCustomEcranData
        protected List<ICell> ComputeCustomEcranData()
        {

            #region Variables
            DataRow row;
            List<ICell> data = null;
            #endregion

            #region Get Data
            row = GetCustomEcranData();
            #endregion

            #region Compute data
            //if (row != null)
            //{
            //    data = new List<ICell>(2);
            //    data.Add(new CellLabel(GestionWeb.GetWebWord(3145, _webSession.SiteLanguage)));
            //    data.Add(new CellLabel(mediaOwner));
            //}
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataSpotNumber
        protected override List<ICell> ComputeDataSpotNumber(AbstractResult.DataUnit dataUnit)
        {

            #region Variables
            List<ICell> data = null;
            string nbrSpot = string.Empty;
            #endregion

            #region Compute data
            if (dataUnit != null && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNicheChannels
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others))
            {

                #region Get Data
                nbrSpot = dataUnit.GetSpotNumber();
                #endregion

                //Nombre de spot
                if (nbrSpot.Length == 0)
                {
                    nbrSpot = "0";
                }
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(2253, _webSession.SiteLanguage)));
                CellNumber cN3 = new CellNumber(double.Parse(nbrSpot));
                cN3.StringFormat = UNIT_FORMAT;
                cN3.AsposeFormat = 3;
                cN3.CssClass = "left";
                data.Add(cN3);
            }
            #endregion

            return data;

        }

        #endregion

        #region ComputeTotalDataSpotNumberByEcran
        protected List<ICell> ComputeTotalDataSpotNumberByEcran(AbstractResult.DataEcran dataEcran)
        {

            #region Variables
            List<ICell> data = null;
            decimal spotNumber = 0;
            #endregion

            #region Get Data
            if (dataEcran != null)
            {
                spotNumber = dataEcran.GetSpotNumber();
            }
            #endregion

            #region Compute data
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
                && (dataEcran!=null && dataEcran.IsAlertModule))
            {

                // Nombre moyen de spots par écran
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(3216, _webSession.SiteLanguage)));

                CellNumber cell = new CellNumber(Convert.ToDouble(spotNumber));
                cell.StringFormat = "{0:max2}";
                cell.AsposeFormat = 4;
                cell.CssClass = "left";
                data.Add(cell);
            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataTotalDurationEcran
        protected List<ICell> ComputeDataTotalDurationEcran(AbstractResult.DataEcran dataEcran)
        {

            #region Variables
            List<ICell> data = null;
            decimal duration = 0;
            #endregion

            #region Get Data
            if (dataEcran != null)
            {
                duration = dataEcran.GetDuration();
            }
            #endregion

            #region Compute data
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
                && (dataEcran != null && dataEcran.IsAlertModule))
            {

                // Durée moyenne d'un écran

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(3217, _webSession.SiteLanguage)));
                CellDuration cD1 = new CellDuration(Convert.ToDouble(duration));
                cD1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.duration).StringFormat;
                cD1.CssClass = "left";
                data.Add(cD1);

            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAverageDuration
        protected override List<ICell> ComputeDataAverageDuration(AbstractResult.DataUnit dataUnit, AbstractResult.DataEcran dataEcran)
        {

            #region Variables
            List<ICell> data = null;
            decimal averageDuration = 0;
            string nbrSpots = string.Empty;
            #endregion

            #region Get Data
            if (dataUnit != null)
            {
                nbrSpots = dataUnit.GetSpotNumber();
                if (nbrSpots.Length > 0)
                {
                    averageDuration = dataUnit.GetAverageDuration();
                }
            }
            #endregion

            #region Compute data
            if (!string.IsNullOrEmpty(nbrSpots)
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
                && (dataEcran!=null && dataEcran.IsAlertModule))
            {

                // Durée moyenne d'un écran

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(3218, _webSession.SiteLanguage)));
                CellDuration cD1 = new CellDuration(Convert.ToDouble(averageDuration));
                cD1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.duration).StringFormat;
                cD1.CssClass = "left";
                data.Add(cD1);

            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAverageDurationEcran
        protected override List<ICell> ComputeDataAverageDurationEcran(AbstractResult.DataEcran dataEcran)
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
                    averageDurationEcran = dataEcran.GetAverageDurationInBreak();
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
                && (dataEcran != null && dataEcran.IsAlertModule))
            {

                // Durée moyenne d'un écran

                data = new List<ICell>(2);
                data.Add(new CellLabel(GetAverageDurationOfSpostLabel()));
                CellDuration cD1 = new CellDuration(Convert.ToDouble(((long)averageDurationEcran).ToString()));
                cD1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.duration).StringFormat;
                cD1.CssClass = "left";
                data.Add(cD1);

            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataVolumeForMarketingDirect
        protected override List<ICell> ComputeDataVolumeForMarketingDirect(AbstractResult.DataUnit dataUnit)
        {
            return null;
        }
        #endregion

        #region ComputeDataBannersNumber
        protected override List<ICell> ComputeDataBannersNumber()
        {
            return null;
        }
        #endregion

        #region ComputeDataNumberBoard
        protected override List<ICell> ComputeDataNumberBoard(AbstractResult.DataUnit dataUnit)
        {
            return null;
        }
        #endregion

        #region ComputeDataNetworkType
        protected override List<ICell> ComputeDataNetworkType(bool isAlertModule)
        {
            return null;
        }
        #endregion

        #region ComputeDataPageNumber
        protected override List<ICell> ComputeDataPageNumber()
        {
            return null;
        }
        #endregion

        #region ComputeDataAdNumber
        protected override List<ICell> ComputeDataAdNumber(AbstractResult.DataUnit dataUnit)
        {
            return null;
        }
        #endregion

        #region ComputeDataPageRatio
        protected override List<ICell> ComputeDataPageRatio(double pageNumber, double adNumber)
        {
            return null;
        }
        #endregion

        #region ComputeDataAdNumberExcludingInsets
        protected override List<ICell> ComputeDataAdNumberExcludingInsets(bool isAlertModule)
        {
            return null;
        }
        #endregion

        #region ComputeDataAdNumberIncludingInsets
        protected override List<ICell> ComputeDataAdNumberIncludingInsets(bool isAlertModule)
        {
            return null;
        }
        #endregion

        #region ComputeDataEvaliantInsertionNumber
        protected override List<ICell> ComputeDataEvaliantInsertionNumber(AbstractResult.DataUnit dataUnit)
        {
            return null;
        }
        #endregion

        #region ComputeDataMmsInsertionNumber
        protected override List<ICell> ComputeDataMmsInsertionNumber(AbstractResult.DataUnit dataUnit)
        {
            return null;
        }
        #endregion

        #region ComputeDataProductNumberInTracking
        protected override List<ICell> ComputeDataProductNumberInTracking(bool isAlertModule)
        {
            return null;
        }
        #endregion

        #region ComputeDataProductNumberInVehicle
        protected override List<ICell> ComputeDataProductNumberInVehicle(bool isAlertModule)
        {
            return null;
        }
        #endregion

        #region Get Data
        
        #region GetDataMediaOwner
        /// <summary>
        /// GetDataCategory
        /// </summary>
        protected string GetDataMediaOwner()
        {
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.mediaOwner))
            {
                DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.mediaOwner);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                    return (dt.Rows[0]["media_owner"].ToString());
            }
            return string.Empty;
        }
        #endregion

        #region Get Custom Ecran Data
        /// <summary>
        /// Get Custom Ecran Data
        /// </summary>
        protected DataRow GetCustomEcranData()
        {
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.mediaOwner))
            {
                DataSet ds = _portofolioDAL.GetCustomEcranData();
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                    return (dt.Rows[0]);
            }
            return null;
        }
        #endregion

        #region GetCommercialItemNumber
        /// <summary>
        /// GetDataCategory
        /// </summary>
        protected int GetCommercialItemNumber()
        {
            DataSet ds = _portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.commercialItemNumber);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                var itemsNb = dt.Rows[0]["COM_ITEM_NB"].ToString();
                if (string.IsNullOrEmpty(itemsNb))
                    return 0;
                else
                    return (Convert.ToInt32(itemsNb));
            }
            return 0;
        }
        #endregion

        #endregion

        #region Average duration of a spot Label
        protected override string GetAverageDurationOfSpostLabel()
        {
            return GestionWeb.GetWebWord(3215, _webSession.SiteLanguage);
        }
        #endregion
    }
}
