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

            #region Media Type
            dataTemp = ComputeDataMediaType();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Media Owner
            dataTemp = ComputeDataMediaOwner();
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Total Data Spot Number By Ecran
            dataTemp = ComputeTotalDataSpotNumberByEcran(_dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Total Duration spot by Ecran
            dataTemp = ComputeDataTotalDurationEcran(_dataEcran);
            if (dataTemp != null) data.AddRange(dataTemp);
            #endregion

            #region Total Duration spot by Ecran
            dataTemp = ComputeDataAverageDuration(_dataUnit, _dataEcran);
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
        protected List<ICell> ComputeDataMediaType()
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
        protected List<ICell> ComputeDataMediaOwner()
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
            string nbrEcran = null;
            #endregion

            #region Get Data
            if (dataEcran != null)
            {
                nbrEcran = dataEcran.GetNumber();
            }
            #endregion

            #region Compute data
            if (!string.IsNullOrEmpty(nbrEcran)
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
                && dataEcran.IsAlertModule)
            {

                // Nombre moyen de spots par écran
                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(3216, _webSession.SiteLanguage)));

                CellNumber cell = new CellNumber(Convert.ToDouble(nbrEcran));
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
            string nbrEcran = null;
            #endregion

            #region Get Data
            if (dataEcran != null)
            {
                nbrEcran = dataEcran.GetNumber();
            }
            #endregion

            #region Compute data
            if (!string.IsNullOrEmpty(nbrEcran)
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
                && dataEcran.IsAlertModule)
            {

                // Durée moyenne d'un écran

                data = new List<ICell>(2);
                data.Add(new CellLabel(GestionWeb.GetWebWord(3217, _webSession.SiteLanguage)));
                CellDuration cD1 = new CellDuration(Convert.ToDouble(nbrEcran));
                cD1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.duration).StringFormat;
                cD1.CssClass = "left";
                data.Add(cD1);

            }
            #endregion

            return data;

        }
        #endregion

        #region ComputeDataAverageDuration
        protected virtual List<ICell> ComputeDataAverageDuration(AbstractResult.DataUnit dataUnit, AbstractResult.DataEcran dataEcran)
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
                && dataEcran.IsAlertModule)
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

        #endregion

        #region Average duration of a spot Label
        protected override string GetAverageDurationOfSpostLabel()
        {
            return GestionWeb.GetWebWord(3215, _webSession.SiteLanguage);
        }
        #endregion
    }
}
