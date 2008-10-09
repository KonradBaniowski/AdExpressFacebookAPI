#region Information
/*
 * Author : G Ragneau
 * Creation : 15/04/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion


#region Using
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

using TNS.AdExpress;
using CstCustom = TNS.AdExpress.Constantes.Customer;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassif = TNS.AdExpress.DataAccess.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using Navigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using FctWeb = TNS.AdExpress.Web.Functions;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpressI.LostWon.Exceptions;
using TNS.AdExpressI.LostWon.DAL;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Classification;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using System.Globalization;
using System.Text;
using TNS.AdExpress.DataAccess.Classification.MediaBranch;
using TNS.AdExpress.DataAccess.Classification;
using TNS.FrameWork.Collections;

#endregion

namespace TNS.AdExpressI.LostWon {

    /// <summary>
    /// Default Dynamic reports
    /// </summary>
    public abstract class LostWonResult : ILostWonResult {

        #region Constantes
        /// <summary>
        /// Id of media univers on year N
        /// </summary>
        public const int N_UNIVERSE_ID = -1;
        /// <summary>
        /// Id of media univers on year N-1
        /// </summary>
        public const int N1_UNIVERSE_ID = -2;
        /// <summary>
        /// Id of media univers about evol
        /// </summary>
        public const int EVOL_UNIVERSE_ID = -3;
        /// <summary>
        /// Id of subtotal columns
        /// </summary>
        public const int SUBTOTAL_ID = -5;
        /// <summary>
        /// Id of label columns
        /// </summary>
        public const int LEVEL_ID = -6;
        /// <summary>
        /// Id of Media Schedule Column
        /// </summary>
        public const int MEDIA_SCHEDULE_ID = -7;
        /// <summary>
        /// Id of column loyal
        /// </summary>
        public const int LOYAL_HEADER_ID = -8;
        /// <summary>
        /// Id of column loyal sliding
        /// </summary>
        public const int LOYAL_DECLINE_HEADER_ID = -9;
        /// <summary>
        /// Id of column loyal rising
        /// </summary>
        public const int LOYAL_RISE_HEADER_ID = -10;
        /// <summary>
        /// Id of column Won
        /// </summary>
        public const int WON_HEADER_ID = -11;
        /// <summary>
        /// Id of column Lost
        /// </summary>
        public const int LOST_HEADER_ID = -12;
        /// <summary>
        /// Id of column Item Number
        /// </summary>
        public const int ITEM_NUMBER_HEADER_ID = -13;
        /// <summary>
        /// Id of column Unit
        /// </summary>
        public const int UNIT_HEADER_ID = -14;

        #endregion

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Result Type
        /// </summary>
        protected int _result;
        /// <summary>
        /// Current vehicle univers
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// Current Module
        /// </summary>
        protected Navigation.Module _module;
        /// <summary>
        /// Specify if media schedule columns must be shown
        /// </summary>
        protected bool _showMediaSchedule = false;
        #endregion

        #region Accessors
        /// <summary>
        /// Get User session
        /// </summary>
        public WebSession Session {
            get { return _session; }
        }
        /// <summary>
        /// Get Result Type
        /// </summary>
        public int ResultType {
            get { return _result; }
        }
        /// <summary>
        /// Get Current Vehicle
        /// </summary>
        public VehicleInformation VehicleInformationObject {
            get { return _vehicleInformation; }
        }
        /// <summary>
        /// Get Current Module
        /// </summary>
        public Navigation.Module CurrentModule {
            get { return _module; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public LostWonResult(WebSession session) {
            _session = session;
            _module = Navigation.ModulesList.GetModule(session.CurrentModule);

            #region S�lection du vehicle
            string vehicleSelection = session.GetSelection(session.SelectionUniversMedia, CstCustom.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new LostWonException("Uncorrect Media Selection"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion

        }
        #endregion

        #region ILostWon Imp
        /// <summary>
        /// Compute portefolio report
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetPortofolio() {
            return GetResult(DynamicAnalysis.PORTEFEUILLE);
        }
        /// <summary>
        /// Compute Loyal Report
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetLoyal() {
            return GetResult(DynamicAnalysis.LOYAL);
        }
        /// <summary>
        /// Compute "Loyal In Decline" Report
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetLoyalDecline() {
            return GetResult(DynamicAnalysis.LOYAL_DECLINE);
        }
        /// <summary>
        /// Compute "Loyal In Progress" Report
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetLoyalRise() {
            return GetResult(DynamicAnalysis.LOYAL_RISE);
        }
        /// <summary>
        /// Compute "Won" Report
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetWon() {
            return GetResult(DynamicAnalysis.WON);
        }
        /// <summary>
        /// Compute "Lost" Report
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetLost() {
            return GetResult(DynamicAnalysis.LOST);
        }
        /// <summary>
        /// Compute "Synthesis" Report
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetSynthesis() {
            return GetResult(DynamicAnalysis.SYNTHESIS);
        }
        /// <summary>
        /// Compute result specified in user session
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetResult() {
            return this.GetResult((int)_session.CurrentTab);
        }
        /// <summary>
        /// Compute specified result
        /// </summary>
        /// <param name="result">Type of result (DynamicAnalysis)</param>
        /// <returns>Computed data</returns>
        public ResultTable GetResult(int result) {
            switch (result) {
                case DynamicAnalysis.LOST:
                case DynamicAnalysis.LOYAL:
                case DynamicAnalysis.LOYAL_DECLINE:
                case DynamicAnalysis.LOYAL_RISE:
                case DynamicAnalysis.PORTEFEUILLE:
                case DynamicAnalysis.WON:
                    this._result = result;
                    return this.GetData();
                case DynamicAnalysis.SYNTHESIS:
                    return this.GetSynthesisData();
                default: return null;
            }
        }
        #endregion

        #region GetData
        /// <summary>
        /// Compute Result Data
        /// </summary>
        /// <returns>Computed Result Data</returns>
        protected ResultTable GetData()
        {
            ResultTable tabData = GetRawTable();
            ResultTable tabResult = null;

            #region No data
            if (tabData == null)
            {
                return null;
            }
            #endregion

            #region Data Filtering
            switch (this._result)
            {
                case DynamicAnalysis.LOST:
                    Filter(tabData, new PredicateDelegate(PredicateLost));
                    break;
                case DynamicAnalysis.WON:
                    Filter(tabData, new PredicateDelegate(PredicateWon));
                    break;
                case DynamicAnalysis.LOYAL:
                    Filter(tabData, new PredicateDelegate(PredicateLoyal));
                    break;
                case DynamicAnalysis.LOYAL_DECLINE:
                    Filter(tabData, new PredicateDelegate(PredicateLoyalDecline));
                    break;
                case DynamicAnalysis.LOYAL_RISE:
                    Filter(tabData, new PredicateDelegate(PredicateLoyalRising));
                    break;
                default:
                    break;
            }
            #endregion

            tabResult = GetFinalTable(tabData);

            return tabResult;

        }
        #endregion

        #region GetSynthesisData
        /// <summary>
        /// Get synthesis report about number of products matching Loyal, Loayl sliding, Loyal rising, Won, lost
        /// </summary>
        /// <returns>Result Table</returns>
        protected ResultTable GetSynthesisData() {

            #region variables
            long nbLine;
            List<string> advertisers = null;
            List<string> products = null;
            List<string> brands = null;
            List<string> sectors = null;
            List<string> subsectors = null;
            List<string> groups = null;
            List<string> agencyGroups = null;
            List<string> agency = null;
            Int64 advertiserLineIndex = 0;
            Int64 brandLineIndex = 0;
            Int64 productLineIndex = 0;
            Int64 sectorLineIndex = 0;
            Int64 subsectorLineIndex = 0;
            Int64 groupLineIndex = 0;
            Int64 agencyGroupLineIndex = 0;
            Int64 agencyLineIndex = 0;

            string filterN = "";
            string filterN1 = "";
            DataTable dt = null;
            string beginningPeriodDA = _session.PeriodBeginningDate;
            string endPeriodDA = _session.PeriodEndDate;
            CellNumber c = new CellNumber(0.0);
            c.StringFormat = "max0";
            CellUnitFactory numberFactory = new CellUnitFactory(c); ;
            #endregion

            #region Calcul des p�riodes
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

            string beginningPeriodN1DA = customerPeriod.ComparativeStartDate;
            string endPeriodN1DA = customerPeriod.ComparativeEndDate;
            DateTime PeriodBeginningDate = new DateTime(int.Parse(customerPeriod.StartDate.Substring(0, 4)), int.Parse(customerPeriod.StartDate.Substring(4, 2)), int.Parse(customerPeriod.StartDate.Substring(6, 2)));
            DateTime PeriodEndDate = new DateTime(int.Parse(customerPeriod.EndDate.Substring(0, 4)), int.Parse(customerPeriod.EndDate.Substring(4, 2)), int.Parse(customerPeriod.EndDate.Substring(6, 2))); ;

            DateTime PeriodBeginningDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeStartDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeStartDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeStartDate.Substring(6, 2)));
            DateTime PeriodEndDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeEndDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeEndDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeEndDate.Substring(6, 2))); ;
            CultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            
			//string PeriodDateN = string.Format(cInfo, "{0:d}-{1:d}", PeriodBeginningDate, PeriodEndDate);
			//string PeriodDateN1 = string.Format(cInfo, "{0:d}-{1:d}", PeriodBeginningDateN1DA, PeriodEndDateN1DA);
			string PeriodDateN = FctUtilities.Dates.DateToString(PeriodBeginningDate,_session.SiteLanguage) + "-" + FctUtilities.Dates.DateToString(PeriodEndDate,_session.SiteLanguage);
			string PeriodDateN1 = FctUtilities.Dates.DateToString(PeriodBeginningDateN1DA, _session.SiteLanguage) + "-" + FctUtilities.Dates.DateToString(PeriodEndDateN1DA, _session.SiteLanguage);

            #endregion

            #region Aucune donn�es (par rapport aux dates)
            if (PeriodBeginningDate > DateTime.Now) {
                return null;
            }
            #endregion

            #region Chargement des donn�es � partir de la base

            try {
                if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the lost won result"));
                object[] parameters = new object[1];
                parameters[0] = _session;
                ILostWonResultDAL lostwonDAL = (ILostWonResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
                dt = lostwonDAL.GetSynthesisData();

                //dt = DynamicDataAccess.GetGenericSynthesisData(_session, vehicleName); 
            }
            catch (System.Exception err) {
                throw (new LostWonException("Unable to load data for synthesis report.", err));
            }
            #endregion

            #region Aucune donn�es (par rapport aux donn�es)
            if (dt == null || dt.Rows == null || dt.Rows.Count < 1) {
                return null;
            }
            #endregion

            #region Identifiant du texte des unit�s
            Int64 unitId = _session.GetUnitLabelId();
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            GetProductActivity getProductActivity;
            string expression = string.Empty;
            if (cellUnitFactory.Get(0.0) is CellIdsNumber)
            {
                expression = _session.GetSelectedUnit().Id.ToString();
                getProductActivity = new GetProductActivity(GetListProductActivity);
            }
            else{
                expression = FctWeb.SQLGenerator.GetUnitAliasSum(_session);
                getProductActivity = new GetProductActivity(GetDoubleProductActivity);
            }
            #endregion

            #region Cr�ation des headers
            nbLine = 4;
			if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) nbLine++;
            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) nbLine++;
            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MEDIA_AGENCY)) {
                if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                    nbLine++;
                if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                    nbLine++;
            }

            // Ajout de la colonne Produit
            Headers headers = new Headers();
            headers.Root.Add(new Header(GestionWeb.GetWebWord(1164, _session.SiteLanguage), LEVEL_ID));

            #region Fid�le
            HeaderGroup fidele = new HeaderGroup(GestionWeb.GetWebWord(1241, _session.SiteLanguage), LOYAL_HEADER_ID);
            fidele.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitFidele = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitFidele.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID));
            unitFidele.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID));
            fidele.Add(unitFidele);
            headers.Root.Add(fidele);
            #endregion

            #region Fid�le en baisse
            HeaderGroup fideleDecline = new HeaderGroup(GestionWeb.GetWebWord(1242, _session.SiteLanguage), LOYAL_DECLINE_HEADER_ID);
            fideleDecline.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitFideleDecline = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitFideleDecline.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID));
            unitFideleDecline.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID));
            fideleDecline.Add(unitFideleDecline);
            headers.Root.Add(fideleDecline);
            #endregion

            #region Fid�le en hausse
            HeaderGroup fideleRise = new HeaderGroup(GestionWeb.GetWebWord(1243, _session.SiteLanguage), LOYAL_RISE_HEADER_ID);
            fideleRise.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitFideleRise = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitFideleRise.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID));
            unitFideleRise.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID));
            fideleRise.Add(unitFideleRise);
            headers.Root.Add(fideleRise);
            #endregion

            #region Gagn�s
            HeaderGroup won = new HeaderGroup(GestionWeb.GetWebWord(1244, _session.SiteLanguage), WON_HEADER_ID);
            won.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitWon = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitWon.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID));
            unitWon.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID));
            won.Add(unitWon);
            headers.Root.Add(won);
            #endregion

            #region Perdus
            HeaderGroup lost = new HeaderGroup(GestionWeb.GetWebWord(1245, _session.SiteLanguage), LOST_HEADER_ID);
            lost.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitLost = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitLost.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID));
            unitLost.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID));
            lost.Add(unitLost);
            headers.Root.Add(lost);
            #endregion

            #endregion

            ResultTable resultTable = new ResultTable(nbLine, headers);
            Int64 nbCol = resultTable.ColumnsNumber - 2;

            advertisers = new List<string>();
            products = new List<string>();
            brands = new List<string>();
            sectors = new List<string>();
            subsectors = new List<string>();
            groups = new List<string>();
            agencyGroups = new List<string>();
            agency = new List<string>();

            #region Initialisation des lignes
            Int64 levelLabelColIndex = resultTable.GetHeadersIndexInResultTable(LEVEL_ID.ToString());
            advertiserLineIndex = resultTable.AddNewLine(LineType.level1);
            resultTable[advertiserLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1146, _session.SiteLanguage));
            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) {
                brandLineIndex = resultTable.AddNewLine(LineType.level1);
                resultTable[brandLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1149, _session.SiteLanguage));
            }
			if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) {
				productLineIndex = resultTable.AddNewLine(LineType.level1);
				resultTable[productLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1164, _session.SiteLanguage));
			}
			sectorLineIndex = resultTable.AddNewLine(LineType.level1);
            resultTable[sectorLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1847, _session.SiteLanguage));
            subsectorLineIndex = resultTable.AddNewLine(LineType.level1);
            resultTable[subsectorLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1848, _session.SiteLanguage));
            groupLineIndex = resultTable.AddNewLine(LineType.level1);
            resultTable[groupLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1849, _session.SiteLanguage));
            // Groupe d'Agence && Agence
            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MEDIA_AGENCY)) {
                if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency)) {
                    agencyGroupLineIndex = resultTable.AddNewLine(LineType.level1);
                    resultTable[agencyGroupLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1850, _session.SiteLanguage));
                }
                if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency)) {
                    agencyLineIndex = resultTable.AddNewLine(LineType.level1);
                    resultTable[agencyLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1851, _session.SiteLanguage));
                }
            }
            #endregion

            #region Initialisation des lignes
            Int64 _loyalNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 _loyalDeclineNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_DECLINE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 _loyalRiseNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_RISE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 _wonNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(WON_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 _lostNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(LOST_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            #region Initialisation des Nombres
            for (int i = 0; i < nbLine; i++) {
                resultTable[i, _loyalNumberColonneIndex] = numberFactory.Get(0.0);
                resultTable[i, _loyalDeclineNumberColonneIndex] = numberFactory.Get(0.0);
                resultTable[i, _loyalRiseNumberColonneIndex] = numberFactory.Get(0.0);
                resultTable[i, _wonNumberColonneIndex] = numberFactory.Get(0.0);
                resultTable[i, _lostNumberColonneIndex] = numberFactory.Get(0.0);
            }
            for (long i = 0; i < nbLine; i++) {
                for (long j = _loyalNumberColonneIndex + 1; j < _loyalDeclineNumberColonneIndex; j++) {
                    resultTable[i, j] = cellUnitFactory.Get(0.0);
                }
                for (long j = _loyalDeclineNumberColonneIndex + 1; j < _loyalRiseNumberColonneIndex; j++) {
                    resultTable[i, j] = cellUnitFactory.Get(0.0);
                }
                for (long j = _loyalRiseNumberColonneIndex + 1; j < _wonNumberColonneIndex; j++) {
                    resultTable[i, j] = cellUnitFactory.Get(0.0);
                }
                for (long j = _wonNumberColonneIndex + 1; j < _lostNumberColonneIndex; j++) {
                    resultTable[i, j] = cellUnitFactory.Get(0.0);
                }
                for (long j = _lostNumberColonneIndex + 1; j <= nbCol; j++) {
                    resultTable[i, j] = cellUnitFactory.Get(0.0);
                }
            }
            #endregion

            #endregion

            #region Traitement des donn�es
            foreach (DataRow currentRow in dt.Rows) {

                //Activit� publicitaire Annonceurs
                if (currentRow["id_advertiser"] != null && currentRow["id_advertiser"] != System.DBNull.Value && !advertisers.Contains(currentRow["id_advertiser"].ToString())) {
                    filterN = "id_advertiser=" + currentRow["id_advertiser"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                    filterN1 = "id_advertiser=" + currentRow["id_advertiser"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    getProductActivity(resultTable, dt, advertiserLineIndex, expression, filterN, filterN1);
                    advertisers.Add(currentRow["id_advertiser"].ToString());
                }

                if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) {//droits marques
                    //Activit� publicitaire marques
                    if (currentRow["id_brand"] != null && currentRow["id_brand"] != System.DBNull.Value && !brands.Contains(currentRow["id_brand"].ToString())) {
                        filterN = "id_brand=" + currentRow["id_brand"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                        filterN1 = "id_brand=" + currentRow["id_brand"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                        getProductActivity(resultTable, dt, brandLineIndex, expression, filterN, filterN1);
                        brands.Add(currentRow["id_brand"].ToString());
                    }
                }

                //Activit� publicitaire produits
                if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) && currentRow["id_product"] != null && currentRow["id_product"] != System.DBNull.Value && !products.Contains(currentRow["id_product"].ToString())) {
                    filterN = "id_product=" + currentRow["id_product"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                    filterN1 = "id_product=" + currentRow["id_product"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    getProductActivity(resultTable, dt, productLineIndex, expression, filterN, filterN1);
                    products.Add(currentRow["id_product"].ToString());
                }

                //Activit� publicitaire Famille
                if (currentRow["id_sector"] != null && currentRow["id_sector"] != System.DBNull.Value && !sectors.Contains(currentRow["id_sector"].ToString())) {
                    filterN = "id_sector=" + currentRow["id_sector"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                    filterN1 = "id_sector=" + currentRow["id_sector"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    getProductActivity(resultTable, dt, sectorLineIndex, expression, filterN, filterN1);
                    sectors.Add(currentRow["id_sector"].ToString());
                }
                //Activit� publicitaire Classe
                if (currentRow["id_subsector"] != null && currentRow["id_subsector"] != System.DBNull.Value && !subsectors.Contains(currentRow["id_subsector"].ToString())) {
                    filterN = "id_subsector=" + currentRow["id_subsector"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))"; ;
                    filterN1 = "id_subsector=" + currentRow["id_subsector"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    getProductActivity(resultTable, dt, subsectorLineIndex, expression, filterN, filterN1);
                    subsectors.Add(currentRow["id_subsector"].ToString());
                }
                //Activit� publicitaire Groupes
                if (currentRow["id_group_"] != null && currentRow["id_group_"] != System.DBNull.Value && !groups.Contains(currentRow["id_group_"].ToString())) {
                    filterN = "id_group_=" + currentRow["id_group_"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                    filterN1 = "id_group_=" + currentRow["id_group_"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    getProductActivity(resultTable, dt, groupLineIndex, expression, filterN, filterN1);
                    groups.Add(currentRow["id_group_"].ToString());
                }

                if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MEDIA_AGENCY)) {//droits agences m�dia					
                    //activit� publicitaire Groupes d'agences
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency) && currentRow["ID_GROUP_ADVERTISING_AGENCY"] != null && currentRow["ID_GROUP_ADVERTISING_AGENCY"] != System.DBNull.Value && !agencyGroups.Contains(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString())) {
                        filterN = "ID_GROUP_ADVERTISING_AGENCY=" + currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                        filterN1 = "ID_GROUP_ADVERTISING_AGENCY=" + currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                        getProductActivity(resultTable, dt, agencyGroupLineIndex, expression, filterN, filterN1);
                        agencyGroups.Add(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString());
                    }

                    //activit� publicitaire agence
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency) && currentRow["ID_ADVERTISING_AGENCY"] != null && currentRow["ID_ADVERTISING_AGENCY"] != System.DBNull.Value && !agency.Contains(currentRow["ID_ADVERTISING_AGENCY"].ToString())) {
                        filterN = "ID_ADVERTISING_AGENCY=" + currentRow["ID_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                        filterN1 = "ID_ADVERTISING_AGENCY=" + currentRow["ID_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                        getProductActivity(resultTable, dt, agencyLineIndex, expression, filterN, filterN1);
                        agency.Add(currentRow["ID_ADVERTISING_AGENCY"].ToString());
                    }
                }
            }
            #endregion

            return (resultTable);
        }
        #endregion

        #region Internal methods

        #region Raw table
        /// <summary>
        /// Get Table with data without any filtering on required result
        /// </summary>
        /// <returns>Data</returns>
        protected ResultTable GetRawTable()
        {
            #region Date
            int dateBegin = int.Parse(_session.PeriodBeginningDate);
            int dateEnd = int.Parse(_session.PeriodEndDate);
            DateTime startDate = new DateTime(dateBegin / 10000, (dateBegin-(10000*(dateBegin/10000))) / 100, (dateBegin-(100*(dateBegin/100))));
            if (startDate > DateTime.Now)
            {
                return null;
            }
            #endregion

            #region Load data from data access layer
            DataTable dt = null;
            DataSet dsMedia = null;
            Navigation.Module currentModuleDescription = Navigation.ModulesList.GetModule(_session.CurrentModule);
            try
            {

                if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the lost won result"));
                object[] parameters = new object[1];
                parameters[0] = _session;
                ILostWonResultDAL lostwonDAL = (ILostWonResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
                dt = lostwonDAL.GetData();
                dsMedia = lostwonDAL.GetMediaDetails();

            }
            catch (System.Exception err)
            {
                throw (new LostWonException("Unable to load dynamic report data.", err));
            }
            DataTable dtMedia = dsMedia.Tables[0];

            if (dt != null && dt.Rows.Count == 0)
            {
                return null;
            }
            #endregion

            #region GetHeaders
            Headers headers = GetHeaders(dtMedia);
            #endregion

            #region Init Table
            Int64 nbline = GetNbLine(dt);
            ResultTable tabData = new ResultTable(nbline, headers);
            #endregion

            #region Fill result table
            int levelNb = _session.GenericProductDetailLevel.GetNbLevels;
            Int64[] oldIds = new Int64[levelNb];
            Int64[] cIds = new Int64[levelNb];
            CellLevel[] levels = new CellLevel[nbline];
            Int64 cLine = 0;
            for (int i = 0; i < levelNb; i++) { oldIds[i] = cIds[i] = -1; }
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();
            SetLineDelegate setLine;
            switch (_session.Unit)
            {
                case CstWeb.CustomerSessions.Unit.versionNb:
                    setLine = new SetLineDelegate(SetListLine);
                    break;
                default:
                    setLine = new SetLineDelegate(SetDoubleLine);
                    break;
            }
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < levelNb; i++)
                {
                    cIds[i] = _session.GenericProductDetailLevel.GetIdValue(row, i + 1);
                    if (cIds[i] >= 0 && cIds[i] != oldIds[i])
                    {
                        oldIds[i] = cIds[i];
                        for (int ii = i + 1; ii < levelNb; ii++) { oldIds[ii] = -1; }
                        cLine = InitLine(tabData, row, cellFactory, i + 1, (i > 0) ? levels[i - 1] : null);
                        levels[i] = (CellLevel)tabData[cLine, 1];
                    }
                }
                setLine(tabData, cLine, row, cellFactory, dateBegin, dateEnd);
            }
            #endregion

            return tabData;
        }

        #region InitLine
        /// <summary>
        /// Delegate to init lines
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory</param>
        /// <param name="level">Current level</param>
        /// <param name="parent">Parent level</param>
        /// <returns>Index of current line</returns>
        protected Int64 InitLine(ResultTable tab, DataRow row, CellUnitFactory cellFactory, int level, CellLevel parent)
        {

            Int64 cLine = -1;
            CellLevel cell;
            switch (level)
            {
                case 1:
                    cLine = tab.AddNewLine(LineType.level1);
                    break;
                case 2:
                    cLine = tab.AddNewLine(LineType.level2);
                    break;
                case 3:
                    cLine = tab.AddNewLine(LineType.level3);
                    break;
                default:
                    throw new ArgumentException(string.Format("Level {0} is not supported.", level));
            }
            tab[cLine, 1] = cell = new CellLevel(
                _session.GenericProductDetailLevel.GetIdValue(row, level)
                , _session.GenericProductDetailLevel.GetLabelValue(row, level)
                , parent
                , level
                , cLine);
            if (_session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == level)
            {
                if (row["id_address"] != DBNull.Value)
                {
                    cell.AddressId = Convert.ToInt64(row["id_address"]);
                }

            }

            for (int i = 2; i <= tab.DataColumnsNumber; i++)
            {
                tab[cLine, i] = cellFactory.Get(0.0);
            }
            return cLine;

        }
        #endregion

        #region SetLineDelegate
        /// <summary>
        /// Delegate to affect values to the table
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="cLine">Current line</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory</param>
        /// <param name="periodBegin">Period Begin</param>
        /// <param name="periodEnd">Period End</param>
        /// <returns>Current line</returns>
        protected delegate Int64 SetLineDelegate(ResultTable tab, Int64 cLine, DataRow row, CellUnitFactory cellFactory, Int64 periodBegin, Int64 periodEnd);
        /// <summary>
        /// Delegate to affect double values to the table
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="cLine">Current line</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory for double cells</param>
        /// <param name="periodBegin">Period Begin</param>
        /// <param name="periodEnd">Period End</param>
        /// <returns>Current line</returns>
        protected Int64 SetDoubleLine(ResultTable tab, Int64 cLine, DataRow row, CellUnitFactory cellFactory, Int64 periodBegin, Int64 periodEnd)
        {

            Int64 idElement = Convert.ToInt64(row["columnDetailLevel"]);
            string idCol = string.Empty;
            string idSubTotal = string.Empty;
            if (IsComparativeDateLine(Int64.Parse(row["date_num"].ToString()), periodBegin, periodEnd))
            {
                idCol = string.Format("{0}-{1}", N1_UNIVERSE_ID, idElement);
                idSubTotal = string.Format("{0}-{1}", N1_UNIVERSE_ID, SUBTOTAL_ID);
            }
            else
            {
                idCol = string.Format("{0}-{1}", N_UNIVERSE_ID, idElement);
                idSubTotal = string.Format("{0}-{1}", N_UNIVERSE_ID, SUBTOTAL_ID);
            }

            Double value = Convert.ToDouble(row[_session.GetSelectedUnit().Id.ToString()]);
            tab.AffectValueAndAddToHierarchy(1, cLine, tab.GetHeadersIndexInResultTable(idCol), value);
            if (tab.HeadersIndexInResultTable.ContainsKey(idSubTotal))
            {
                tab.AffectValueAndAddToHierarchy(1, cLine, tab.GetHeadersIndexInResultTable(idSubTotal), value);
            }
            return cLine;

        }
        /// <summary>
        /// Delegate to affect list values to the table
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="cLine">Current line</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory for list cells</param>
        /// <param name="periodBegin">Period Begin</param>
        /// <param name="periodEnd">Period End</param>
        /// <returns>Current line</returns>
        protected Int64 SetListLine(ResultTable tab, Int64 cLine, DataRow row, CellUnitFactory cellFactory, Int64 periodBegin, Int64 periodEnd)
        {

            Int64 idElement = Convert.ToInt64(row["columnDetailLevel"]);
            string idCol = string.Empty;
            string idSubTotal = string.Empty;
            if (IsComparativeDateLine(Int64.Parse(row["date_num"].ToString()), periodBegin, periodEnd))
            {
                idCol = string.Format("{0}-{1}", N1_UNIVERSE_ID, idElement);
                idSubTotal = string.Format("{0}-{1}", N1_UNIVERSE_ID, SUBTOTAL_ID);
            }
            else
            {
                idCol = string.Format("{0}-{1}", N_UNIVERSE_ID, idElement);
                idSubTotal = string.Format("{0}-{1}", N_UNIVERSE_ID, SUBTOTAL_ID);
            }

            Int64 iSubTotal = -1;
            Int64 iCol = tab.GetHeadersIndexInResultTable(idCol);
            if (tab.HeadersIndexInResultTable.ContainsKey(idSubTotal))
            {
                iSubTotal = tab.GetHeadersIndexInResultTable(idSubTotal);
            }
           
            string[] value = row[_session.GetSelectedUnit().Id.ToString()].ToString().Split(',');
            Int64 v = 0;
            foreach (string s in value)
            {
                v = Convert.ToInt64(s);
                tab.AffectValueAndAddToHierarchy(1, cLine, iCol, v);
                // SubTotal if required (univers contains more than one element)
                if (iSubTotal > -1)
                {
                    tab.AffectValueAndAddToHierarchy(1, cLine, iSubTotal, v);
                }
            }

            return cLine;

        }
        #endregion

        #endregion

        #region IsComparativeDateLine
        /// <summary>
        /// Specify whereas a date belongs to the comparative period or not.
        /// </summary>
        /// <param name="dateToCompare">Date to tests</param>
        /// <param name="beginningDate">Beginning of comparative period</param>
        /// <param name="endDate">End of comparative period</param>
        /// <returns>True if dateToCompare belongs to the comparative period, false neither</returns>
        protected bool IsComparativeDateLine(Int64 dateToCompare, Int64 beginningDate, Int64 endDate) {

            if (dateToCompare.ToString().Length == 6) {
                beginningDate = beginningDate / 100;
                endDate = endDate / 100;
            }

            if (dateToCompare >= beginningDate && dateToCompare <= endDate) return (false);
            return (true);
        }
        #endregion

        #region Init Indexes
        /// <summary>
        /// Build headers
        /// </summary>
        /// <param name="dtMedia">List of column levels</param>
        /// <returns>Headers of the final table</returns>
        protected Headers GetHeaders(DataTable dtMedia)
        {

            #region Dates
            DateTime periodBeginN = new DateTime(int.Parse(_session.CustomerPeriodSelected.StartDate.Substring(0, 4)), int.Parse(_session.CustomerPeriodSelected.StartDate.Substring(4, 2)), int.Parse(_session.CustomerPeriodSelected.StartDate.Substring(6, 2)));
            DateTime periodEndN = new DateTime(int.Parse(_session.CustomerPeriodSelected.EndDate.Substring(0, 4)), int.Parse(_session.CustomerPeriodSelected.EndDate.Substring(4, 2)), int.Parse(_session.CustomerPeriodSelected.EndDate.Substring(6, 2))); ;

            DateTime periodBeginN1 = new DateTime(int.Parse(_session.CustomerPeriodSelected.ComparativeStartDate.Substring(0, 4)), int.Parse(_session.CustomerPeriodSelected.ComparativeStartDate.Substring(4, 2)), int.Parse(_session.CustomerPeriodSelected.ComparativeStartDate.Substring(6, 2)));
            DateTime periodEndN1 = new DateTime(int.Parse(_session.CustomerPeriodSelected.ComparativeEndDate.Substring(0, 4)), int.Parse(_session.CustomerPeriodSelected.ComparativeEndDate.Substring(4, 2)), int.Parse(_session.CustomerPeriodSelected.ComparativeEndDate.Substring(6, 2))); ;

            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
			//string periodLabelN = string.Format("{0}-{1}", string.Format(cInfo, "{0:d}", periodBeginN), string.Format(cInfo, "{0:d}", periodEndN));
			//string periodLabelN1 = string.Format("{0}-{1}", string.Format(cInfo, "{0:d}", periodBeginN1), string.Format(cInfo, "{0:d}", periodEndN1));
			string periodLabelN = FctUtilities.Dates.DateToString(periodBeginN, _session.SiteLanguage) + "-" + FctUtilities.Dates.DateToString(periodEndN, _session.SiteLanguage);
			string periodLabelN1 = FctUtilities.Dates.DateToString(periodBeginN1, _session.SiteLanguage) + "-" + FctUtilities.Dates.DateToString(periodEndN1, _session.SiteLanguage);

            #endregion

            #region Extract Columns Elements
            List<Int64> lIds = new List<Int64>();
            Int64 id = -1;
            StringBuilder sIds = new StringBuilder();
            foreach (DataRow row in dtMedia.Rows)
            {
                id = Convert.ToInt64(row["columnDetailLevel"]);
                if (!lIds.Contains(id))
                {
                    lIds.Add(id);
                    sIds.AppendFormat("{0},", id);
                }
            }
            if (sIds.Length > 0) sIds.Length -= 1;
            #endregion

            #region Load elements labels
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            ClassificationLevelListDataAccess levels = null;

            switch (columnDetailLevel.Id)
            {

                case DetailLevelItemInformation.Levels.media:
                    levels = new PartialMediaListDataAccess(sIds.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.category:
                    levels = new PartialCategoryListDataAccess(sIds.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.mediaSeller:
                    levels = new PartialMediaSellerListDataAccess(sIds.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.title:
                    levels = new PartialTitleListDataAccess(sIds.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.interestCenter:
                    levels = new PartialInterestCenterListDataAccess(sIds.ToString(), _session.DataLanguage, _session.Source);
                    break;

            }
            #endregion

            #region Build headers

            #region Current Columns
            // Product column
            Headers headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), LEVEL_ID));
            // Add Media Schedule column
            if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.holdingCompany) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.sector) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subSector) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.group)
                )
            {
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(150, _session.SiteLanguage), MEDIA_SCHEDULE_ID));
                _showMediaSchedule = true;
            }
            #endregion

            #region Years and evol
            Int64 eltNb = lIds.Count;
            HeaderGroup hGpYearN = new HeaderGroup(periodLabelN, true, N_UNIVERSE_ID);
            HeaderGroup hGpYearN1 = new HeaderGroup(periodLabelN1, true, N1_UNIVERSE_ID);
            HeaderGroup hGpEvol = new HeaderGroup(GestionWeb.GetWebWord(1212, _session.SiteLanguage), true, EVOL_UNIVERSE_ID);
            headers.Root.Add(hGpYearN);
            headers.Root.Add(hGpYearN1);
            headers.Root.Add(hGpEvol);

            if (eltNb > 1)
            {
                hGpYearN.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUBTOTAL_ID);
                hGpYearN1.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUBTOTAL_ID);
                hGpEvol.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUBTOTAL_ID);
            }

            foreach (Int64 i in levels.IdListOrderByClassificationItem)
            {
                hGpYearN.Add(new Header(true,levels[i], i));
                hGpYearN1.Add(new Header(true, levels[i], i));
                hGpEvol.Add(new Header(true, levels[i], i));
            }
            #endregion

            #endregion

            return headers;

        }
        #endregion

        #region Formattage d'un tableau de r�sultat
        /// <summary>
        /// Create ResultTable
        /// </summary>
        /// <param name="tabData">Raw Data table</param>
        /// <returns>Final Data</returns>
        protected ResultTable GetFinalTable(ResultTable tabData)
        {

            #region Lines number
            int nbLine = 0;
            for (int i = 0; i < tabData.LinesNumber; i++)
            {
                if (!(tabData.GetLineStart(i) is LineHide))
                {
                    nbLine++;
                }
            }
            #endregion

            #region No data
            if (nbLine == 0)
            {
                return null;
            }
            #endregion

            bool computePDM = _session.Percentage;

            #region Parutions
            Dictionary<string, double> resNbParution = null;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.media && (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id))
            {
                resNbParution = GetNbParutionsByMedia();
                if (resNbParution != null && resNbParution.Count > 0)
                    nbLine++;
            }
            #endregion

            #region Table init
            //Total line
            nbLine++;
            ResultTable tabResult = new ResultTable(nbLine, tabData.NewHeaders);
            Int64 cLine = 0;
            Int64 NIndex = tabResult.GetHeadersIndexInResultTable(N_UNIVERSE_ID.ToString());
            Int64 N1Index = tabResult.GetHeadersIndexInResultTable(N1_UNIVERSE_ID.ToString());
            Int64 levelIndex = tabResult.GetHeadersIndexInResultTable(LEVEL_ID.ToString());
            Int64 msIndex = tabResult.GetHeadersIndexInResultTable(MEDIA_SCHEDULE_ID.ToString());
            Int64 EvolIndex = tabResult.GetHeadersIndexInResultTable(EVOL_UNIVERSE_ID.ToString());
            Int64 dataIndex = (_showMediaSchedule) ? msIndex + 1 : levelIndex + 1;
            Int64 nbLevel = _session.GenericProductDetailLevel.GetNbLevels;
            CellLevel[] levels = new CellLevel[nbLevel + 1];
            #endregion

            #region Unit selection
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            InitFinalLineValuesDelegate initValues = null;
            SetFinalLineDelegate setValues = null;
            switch (_session.Unit)
            {
                case CstWeb.CustomerSessions.Unit.versionNb:
                    initValues = new InitFinalLineValuesDelegate(InitFinalListValuesLine);
                    setValues = new SetFinalLineDelegate(SetFinalListLine);
                    break;
                default:
                    initValues = new InitFinalLineValuesDelegate(InitFinalDoubleValuesLine);
                    setValues = new SetFinalLineDelegate(SetFinalDoubleLine);
                    break;
            }
            #endregion

            #region Ligne du Total
            cLine = tabResult.AddNewLine(LineType.total);
            //Total label
            levels[0] = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), 0, cLine);
            tabResult[cLine, levelIndex] = levels[0];
            if (_showMediaSchedule) tabResult[cLine, msIndex] = new CellMediaScheduleLink(levels[0], _session);
            initValues(tabResult, cLine, cellUnitFactory, computePDM, NIndex, N1Index, EvolIndex);
            #endregion

            #region Nombre parutions by media
            CellNumber cNb = new CellNumber(0.0);
            cNb.StringFormat = "max0";
            CellUnitFactory nbFactory = new CellUnitFactory(cNb);
            CellEvol cEvol;
            if (resNbParution != null && resNbParution.Count > 0)
            {
                cLine = tabResult.AddNewLine(TNS.FrameWork.WebResultUI.LineType.nbParution);
                //Label
                CellLevel cellParution = new CellLevel(-1, GestionWeb.GetWebWord(2460, _session.SiteLanguage), 0, cLine);
                tabResult[cLine, levelIndex] = cellParution;
                if (_showMediaSchedule) tabResult[cLine, msIndex] = new CellMediaScheduleLink(cellParution, _session);
                //Year N
                tabResult[cLine, NIndex] = nbFactory.Get(0.0);
                for (Int64 k = NIndex + 1; k < N1Index; k++)
                {
                    tabResult[cLine, k] = nbFactory.Get(0.0);
                }
                //Year N1
                tabResult[cLine, N1Index] = nbFactory.Get(0.0);
                for (Int64 k = N1Index + 1; k < EvolIndex; k++)
                {
                    tabResult[cLine, k] = nbFactory.Get(0.0);
                }
                //Evol
                cEvol = new CellEvol(tabResult[cLine, NIndex], tabResult[cLine, N1Index]);
                cEvol.StringFormat = "percentage";
                tabResult[cLine, EvolIndex] = cEvol;
                for (Int64 k = EvolIndex + 1; k <= tabResult.DataColumnsNumber; k++)
                {
                    cEvol = new CellEvol(tabResult[cLine, NIndex + (k - EvolIndex)], tabResult[cLine, N1Index + (k - EvolIndex)]);
                    cEvol.StringFormat = "percentage";
                    tabResult[cLine, k] = cEvol;
                }

                //Parution numbers for N and N1
                Int64 z;
                foreach (KeyValuePair<string, double> kpv in resNbParution)
                {
                    z = tabResult.GetHeadersIndexInResultTable(kpv.Key);
                    if (z > -1)
                    {
                        tabResult[cLine, z] = nbFactory.Get(kpv.Value);
                    }
                }
            }
            #endregion

            #region Fill final table
            CellLevel cLevel = null;
            Int64 NTotalIndex = tabResult.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N_UNIVERSE_ID, SUBTOTAL_ID));
            Int64 N1TotalIndex = tabResult.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N1_UNIVERSE_ID, SUBTOTAL_ID));
            for (int i = 0; i < tabData.LinesNumber; i++)
            {

                if (tabData.GetLineStart(i) is LineHide)
                    continue;

                #region Init Line
                cLine = InitFinalLine(tabData, tabResult, i, levels[((CellLevel)tabData[i, 1]).Level-1], msIndex);
                initValues(tabResult, cLine, cellUnitFactory, computePDM, NIndex, N1Index, EvolIndex);
                cLevel = (CellLevel)tabResult[cLine, 1];
                #endregion

                if (cLevel.Level < nbLevel)
                {
                    levels[cLevel.Level] = cLevel;
                }
                else{
                    setValues(tabData, tabResult, i, cLine, NIndex, N1Index, EvolIndex, NTotalIndex, N1TotalIndex);
                }

            }
            #endregion

            return (tabResult);
        }

        #region InitFinalLineValuesDelegate
        protected delegate Int64 InitFinalLineValuesDelegate(ResultTable toTab, Int64 toLine, CellUnitFactory cellFactory, bool isPDM, Int64 NIndex, Int64 N1Index, Int64 EvolIndex);
        protected Int64 InitFinalDoubleValuesLine(ResultTable toTab, Int64 toLine, CellUnitFactory cellFactory, bool isPDM, Int64 NIndex, Int64 N1Index, Int64 EvolIndex)
        {

            // Units
            if (isPDM)
            {
                toTab[toLine, NIndex] = new CellPDM(0.0, null);
            }
            else
            {
                toTab[toLine, NIndex] = cellFactory.Get(0.0);
            }
            //year N
            for (Int64 k = NIndex + 1; k < N1Index; k++)
            {
                if (isPDM)
                {
                    toTab[toLine, k] = new CellPDM(0.0, (CellUnit)toTab[toLine, NIndex]);
                }
                else
                {
                    toTab[toLine, k] = cellFactory.Get(0.0);
                }
            }
            //year N1
            if (isPDM)
            {
                toTab[toLine, N1Index] = new CellPDM(0.0, null);
            }
            else
            {
                toTab[toLine, N1Index] = cellFactory.Get(0.0);
            }
            for (Int64 k = N1Index + 1; k < EvolIndex; k++)
            {
                if (isPDM)
                {
                    toTab[toLine, k] = new CellPDM(0.0, (CellUnit)toTab[toLine, N1Index]);
                }
                else
                {
                    toTab[toLine, k] = cellFactory.Get(0.0);
                }
            }
            //Evol
            CellEvol cEvol = new CellEvol(toTab[toLine, NIndex], toTab[toLine, N1Index]);
            cEvol.StringFormat = "percentage";
            toTab[toLine, EvolIndex] = cEvol;
            for (Int64 k = EvolIndex + 1; k <= toTab.DataColumnsNumber; k++)
            {
                cEvol = new CellEvol(toTab[toLine, NIndex + (k - EvolIndex)], toTab[toLine, N1Index + (k - EvolIndex)]);
                cEvol.StringFormat = "percentage";
                toTab[toLine, k] = cEvol;
            }

            return toLine;

        }
        protected Int64 InitFinalListValuesLine(ResultTable toTab, Int64 toLine, CellUnitFactory cellFactory, bool isPDM, Int64 NIndex, Int64 N1Index, Int64 EvolIndex)
        {

            // Units
            if (isPDM)
            {
                toTab[toLine, NIndex] = new CellVersionNbPDM(null);
            }
            else
            {
                toTab[toLine, NIndex] = cellFactory.Get(0.0);
            }
            //year N
            for (Int64 k = NIndex + 1; k < N1Index; k++)
            {
                if (isPDM)
                {
                    toTab[toLine, k] = new CellVersionNbPDM((CellVersionNbPDM)toTab[toLine, NIndex]);
                }
                else
                {
                    toTab[toLine, k] = cellFactory.Get(0.0);
                }
            }
            //year N1
            if (isPDM)
            {
                toTab[toLine, N1Index] = new CellVersionNbPDM(null);
            }
            else
            {
                toTab[toLine, N1Index] = cellFactory.Get(0.0);
            }
            for (Int64 k = N1Index + 1; k < EvolIndex; k++)
            {
                if (isPDM)
                {
                    toTab[toLine, k] = new CellVersionNbPDM((CellVersionNbPDM)toTab[toLine, N1Index]);
                }
                else
                {
                    toTab[toLine, k] = cellFactory.Get(0.0);
                }
            }
            //Evol
            CellEvol cEvol = new CellEvol(toTab[toLine, NIndex], toTab[toLine, N1Index]);
            cEvol.StringFormat = "percentage";
            toTab[toLine, EvolIndex] = cEvol;
            for (Int64 k = EvolIndex + 1; k <= toTab.DataColumnsNumber; k++)
            {
                cEvol = new CellEvol(toTab[toLine, NIndex + (k - EvolIndex)], toTab[toLine, N1Index + (k - EvolIndex)]);
                cEvol.StringFormat = "percentage";
                toTab[toLine, k] = cEvol;
            }

            return toLine;

        }
        protected Int64 InitFinalLine(ResultTable fromTab, ResultTable toTab, Int64 fromLine, CellLevel parent, Int64 msIndex)
        {
            CellLevel cFromLevel = (CellLevel)fromTab[fromLine, 1];
            Int64 cLine = toTab.AddNewLine(fromTab.GetLineStart(fromLine).LineType);
            AdExpressCellLevel cell = new AdExpressCellLevel(cFromLevel.Id, cFromLevel.Label, parent, cFromLevel.Level, cLine, _session);
            toTab[cLine, 1] = cell;

            //Links
            if (_showMediaSchedule) toTab[cLine, msIndex] = new CellMediaScheduleLink(cell, _session);

            //Gad
            if (_session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == cell.Level)
            {
                cell.AddressId = cFromLevel.AddressId;
            }
            return cLine;

        }
        #endregion

        #region SetLineDelegate
        protected delegate Int64 SetFinalLineDelegate(ResultTable fromTab, ResultTable toTab, Int64 fromLine, Int64 toLine, Int64 NIndex, Int64 N1Index, Int64 EvolIndex, Int64 NTotalIndex, Int64 N1TotalIndex);
        protected Int64 SetFinalDoubleLine(ResultTable fromTab, ResultTable toTab, Int64 fromLine, Int64 toLine, Int64 NIndex, Int64 N1Index, Int64 EvolIndex, Int64 NTotalIndex, Int64 N1TotalIndex)
        {
            Double v = 0;
            //year N
            if (NTotalIndex < 0)
            {
                toTab.AffectValueAndAddToHierarchy(1, toLine, NIndex, ((CellUnit)fromTab[fromLine, NIndex]).Get_value());
            }
            for (Int64 k = NIndex + 1; k < N1Index; k++)
            {
                v = ((CellUnit)fromTab[fromLine, k]).Get_value();
                toTab.AffectValueAndAddToHierarchy(1, toLine, k, v);
                if (NTotalIndex > -1)
                {
                    toTab.AffectValueAndAddToHierarchy(1, toLine, NTotalIndex, v);
                }
            }
            //year N1
            if (N1TotalIndex < 0)
            {
                toTab.AffectValueAndAddToHierarchy(1, toLine, N1Index, ((CellUnit)fromTab[fromLine, N1Index]).Get_value());
            }
            for (Int64 k = N1Index + 1; k < EvolIndex; k++)
            {
                v = ((CellUnit)fromTab[fromLine, k]).Get_value();
                toTab.AffectValueAndAddToHierarchy(1, toLine, k, v);
                if (N1TotalIndex > -1)
                {
                    toTab.AffectValueAndAddToHierarchy(1, toLine, N1TotalIndex, v);
                }
            }

            return toLine;

        }
        protected Int64 SetFinalListLine(ResultTable fromTab, ResultTable toTab, Int64 fromLine, Int64 toLine, Int64 NIndex, Int64 N1Index, Int64 EvolIndex, Int64 NTotalIndex, Int64 N1TotalIndex)
        {

            HybridList value = null;
            Int64 l = 0;
            Int64 v = 0;
            //year N
            if (NTotalIndex < 0)
            {
                value = ((CellIdsNumber)fromTab[fromLine, NIndex]).List;
                l = value.length;
                for (int i = 0; i < l; i++)
                {
                    v = value.removeHead().UniqueID;
                    toTab.AffectValueAndAddToHierarchy(1, toLine, NIndex, v);
                }
            }
            for (Int64 k = NIndex + 1; k < N1Index; k++)
            {
                value = ((CellIdsNumber)fromTab[fromLine, k]).List;
                l = value.length;
                for (int i = 0; i < l; i++)
                {
                    v = value.removeHead().UniqueID;
                    toTab.AffectValueAndAddToHierarchy(1, toLine, k, v);
                    if (NTotalIndex > -1)
                    {
                        toTab.AffectValueAndAddToHierarchy(1, toLine, NTotalIndex, v);
                    }
                }
            }
            //year N1
            if (N1TotalIndex < 0)
            {
                value = ((CellIdsNumber)fromTab[fromLine, N1Index]).List;
                l = value.length;
                for (int i = 0; i < l; i++)
                {
                    v = value.removeHead().UniqueID;
                    toTab.AffectValueAndAddToHierarchy(1, toLine, N1Index, v);
                }
            }
            for (Int64 k = N1Index + 1; k < EvolIndex; k++)
            {
                value = ((CellIdsNumber)fromTab[fromLine, k]).List;
                l = value.length;
                for (int i = 0; i < l; i++)
                {
                    v = value.removeHead().UniqueID;
                    toTab.AffectValueAndAddToHierarchy(1, toLine, k, v);
                    if (N1TotalIndex > -1)
                    {
                        toTab.AffectValueAndAddToHierarchy(1, toLine, N1TotalIndex, v);
                    }
                }
            }

            return toLine;

        }
        #endregion


        #endregion

        #region Compute line numbers in result table from preformated data table
        /// <summary>
        /// Get the number of line from the database data
        /// </summary>
        /// <param name="tabData">Data</param>
        /// <returns>Number of lines</returns>
        protected long GetNbLine(DataTable dt)
        {

            long nbLine = 0;
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            Int64 cIdL1 = -1;
            Int64 cIdL2 = -1;
            Int64 cIdL3 = -1;

            foreach (DataRow row in dt.Rows)
            {
                cIdL1 = _session.GenericProductDetailLevel.GetIdValue(row, 1);
                cIdL2 = _session.GenericProductDetailLevel.GetIdValue(row, 2);
                cIdL3 = _session.GenericProductDetailLevel.GetIdValue(row, 3);
                if (cIdL1 >= 0 && cIdL1 != oldIdL1)
                {
                    oldIdL1 = cIdL1;
                    oldIdL2 = oldIdL3 = -1;
                    nbLine++;
                }
                if (cIdL2 >= 0 && cIdL2 != oldIdL2)
                {
                    oldIdL2 = cIdL2;
                    oldIdL3 = -1;
                    nbLine++;
                }
                if (cIdL3 >= 0 && cIdL3 != oldIdL3)
                {
                    oldIdL3 = cIdL3;
                    nbLine++;
                }
            }
            return nbLine;
        }
        #endregion

        #region Data Filtering
        /// <summary>
        /// Filter data to keep a result with only won data or lost data.
        /// </summary>
        /// <param name="getWon">Specify if the result must be in Won mode or in lost mode</param>
        /// <param name="tabData">Data Table</param>
        /// <param name="nbLineInTabResult">(out) Number of lines in final result</param>
        /// <param name="nbCol">Nb of column in Preformatted table</param>
        /// <returns>Table with either won data or lost data</returns>
        protected void Filter(ResultTable tabData, PredicateDelegate predicate)
        {

            int nbLevel = _session.GenericProductDetailLevel.GetNbLevels;

            CellLevel[] levels = new CellLevel[nbLevel];
            bool[] display = new bool[nbLevel + 1];
            for (int i = 0; i <= nbLevel; i++) { display[i] = false; }
            CellLevel cLevel = null;
            Int64 yearNIndex = tabData.GetHeadersIndexInResultTable(N_UNIVERSE_ID.ToString());
            Int64 yearN1Index = tabData.GetHeadersIndexInResultTable(N1_UNIVERSE_ID.ToString());
            for (int i = 0; i < tabData.LinesNumber; i++)
            {
                cLevel = (CellLevel)tabData[i, 1];
                //Init parents
                if (cLevel.Level < nbLevel)
                {
                    //Check previous parents
                    if (levels[cLevel.Level] != null)
                    {
                        if (!display[cLevel.Level])
                        {
                            tabData.SetLineStart(new LineHide(tabData.GetLineStart(levels[cLevel.Level].LineIndexInResultTable).LineType), levels[cLevel.Level].LineIndexInResultTable);
                        }
                    }
                    //Init current parents
                    levels[cLevel.Level] = cLevel;
                    display[cLevel.Level] = false;
                    continue;
                }

                //filter data
                display[cLevel.Level] = predicate(tabData, i, yearNIndex, yearN1Index);

                //Check result
                if (!display[cLevel.Level])
                {
                    tabData.SetLineStart(new LineHide(tabData.GetLineStart(i).LineType), i);
                }
                else
                {
                    for (int j = nbLevel - 1; j >= 0; j--) { display[j] = true; }
                }
            }
            for (int i = 1; i < nbLevel; i++)
            {
                if (levels[i] != null)
                {
                    if (!display[levels[i].Level])
                    {
                        tabData.SetLineStart(new LineHide(tabData.GetLineStart(levels[i].LineIndexInResultTable).LineType), levels[i].LineIndexInResultTable);
                    }
                }
            }

        }

        #region Predicates
        /// <summary>
        /// Define contract for predicates chackings (most, won, loayls, risings, declines)
        /// </summary>
        /// <param name="tabData">Data container</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="yearN1Index">N1 year column index</param>
        /// <param name="yearNIndex">N year column index</param>
        /// <returns>True if predicate is respected, false either</returns>
        protected delegate bool PredicateDelegate(ResultTable tabData, Int64 cLine, Int64 yearNIndex, Int64 yearN1Index);
        /// <summary>
        /// Check line match "won" predicate
        /// </summary>
        /// <param name="tabData">Data container</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="yearN1Index">N1 year column index</param>
        /// <param name="yearNIndex">N year column index</param>
        /// <returns>True line is there is data in year N but not in year N1, false neither</returns>
        protected bool PredicateWon(ResultTable tabData, Int64 cLine, Int64 yearNIndex, Int64 yearN1Index)
        {
            return ((CellUnit)tabData[cLine, yearNIndex]).Value != 0.0 && ((CellUnit)tabData[cLine, yearN1Index]).Value == 0.0;
        }
        /// <summary>
        /// Check line match "lost" predicate
        /// </summary>
        /// <param name="tabData">Data container</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="yearN1Index">N1 year column index</param>
        /// <param name="yearNIndex">N year column index</param>
        /// <returns>True line is there is no data in year N but some in year N1, false neither</returns>
        protected bool PredicateLost(ResultTable tabData, Int64 cLine, Int64 yearNIndex, Int64 yearN1Index)
        {
            return ((CellUnit)tabData[cLine, yearNIndex]).Value == 0.0 && ((CellUnit)tabData[cLine, yearN1Index]).Value != 0.0;
        }
        /// <summary>
        /// Check line match "loyal" predicate
        /// </summary>
        /// <param name="tabData">Data container</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="yearN1Index">N1 year column index</param>
        /// <param name="yearNIndex">N year column index</param>
        /// <returns>True line is there is data both in N and N1, false neither</returns>
        protected bool PredicateLoyal(ResultTable tabData, Int64 cLine, Int64 yearNIndex, Int64 yearN1Index)
        {
            return ((CellUnit)tabData[cLine, yearNIndex]).Value != 0.0 && ((CellUnit)tabData[cLine, yearN1Index]).Value != 0.0;
        }
        /// <summary>
        /// Check line match "loyal rising" predicate
        /// </summary>
        /// <param name="tabData">Data container</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="yearN1Index">N1 year column index</param>
        /// <param name="yearNIndex">N year column index</param>
        /// <returns>True line is there is data both in N and N1 and N > N1, false neither</returns>
        protected bool PredicateLoyalRising(ResultTable tabData, Int64 cLine, Int64 yearNIndex, Int64 yearN1Index)
        {
            double v = ((CellUnit)tabData[cLine, yearN1Index]).Value;
            return ((CellUnit)tabData[cLine, yearNIndex]).Value > v && v > 0;
        }
        /// <summary>
        /// Check line match "loyal decline" predicate
        /// </summary>
        /// <param name="tabData">Data container</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="yearN1Index">N1 year column index</param>
        /// <param name="yearNIndex">N year column index</param>
        /// <returns>True line is there is data both in N and N1 and N lower than N1, false neither</returns>
        protected bool PredicateLoyalDecline(ResultTable tabData, Int64 cLine, Int64 yearNIndex, Int64 yearN1Index)
        {
            double v = ((CellUnit)tabData[cLine, yearNIndex]).Value;
            return ((CellUnit)tabData[cLine, yearN1Index]).Value > v && v > 0;
        }
        #endregion

        #endregion

        #region Get Product activity
        /// <summary>
        /// Get Advertising activity of a product
        /// </summary>
        /// <param name="tabResult">Result Table</param>
        /// <param name="dt">Data Table</param>
        /// <param name="indexLineProduct">Index of product line</param>
        /// <param name="expression">Calcul expression</param>
        /// <param name="filterN">Year N Filter</param>
        /// <param name="filterN1">Year N-1 filter</param>
        protected delegate void GetProductActivity(ResultTable tabResult, DataTable dt, long indexLineProduct, string expression, string filterN, string filterN1);
        /// <summary>
        /// Get Advertising activity of a product
        /// </summary>
        /// <param name="tabResult">Result Table</param>
        /// <param name="dt">Data Table</param>
        /// <param name="indexLineProduct">Index of product line</param>
        /// <param name="expression">Calcul expression</param>
        /// <param name="filterN">Year N Filter</param>
        /// <param name="filterN1">Year N-1 filter</param>
        protected void GetDoubleProductActivity(ResultTable tabResult, DataTable dt, long indexLineProduct, string expression, string filterN, string filterN1)
        {
            Double unitValueN = 0;
            Double unitValueN1 = 0;
            Int64 loyalNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 loyalDeclineNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_DECLINE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 loyalRiseNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_RISE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 wonNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(WON_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 lostNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOST_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            object o = dt.Compute(expression, filterN);
            unitValueN = (o != DBNull.Value) ? Convert.ToDouble(o) : 0;
            o = dt.Compute(expression, filterN1);
            unitValueN1 = (o != DBNull.Value) ? Convert.ToDouble(o) : 0;

            #region Loyal
            if (unitValueN != 0 && unitValueN1 != 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalNumberColonneIndex]).Value += 1;
                //Unit� N
                ((CellUnit)tabResult[indexLineProduct, loyalNumberColonneIndex + 1]).Value += unitValueN;
                //Unit� N-1
                ((CellUnit)tabResult[indexLineProduct, loyalNumberColonneIndex + 2]).Value += unitValueN1;

            }
            #endregion

            #region Fid�le en baisse
            if (unitValueN > 0 && unitValueN1 > unitValueN)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex]).Value += 1;
                //Unit� N
                ((CellUnit)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex + 1]).Value += unitValueN;
                //Unit� N-1
                ((CellUnit)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex + 2]).Value += unitValueN1;

            }
            #endregion

            #region Fid�le en  d�veloppement
            if (unitValueN1 > 0 && unitValueN > unitValueN1)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalRiseNumberColonneIndex]).Value += 1;
                //Unit� N
                ((CellUnit)tabResult[indexLineProduct, loyalRiseNumberColonneIndex + 1]).Value += unitValueN;
                //Unit� N-1
                ((CellUnit)tabResult[indexLineProduct, loyalRiseNumberColonneIndex + 2]).Value += unitValueN1;

            }
            #endregion

            #region Gagn�s
            if (unitValueN > 0 && unitValueN1 <= 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, wonNumberColonneIndex]).Value += 1;
                //Unit� N
                ((CellUnit)tabResult[indexLineProduct, wonNumberColonneIndex + 1]).Value += unitValueN;
            }
            #endregion

            #region Perdus
            if (unitValueN1 > 0 && unitValueN <= 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, lostNumberColonneIndex]).Value += 1;
                //Unit� N-1
                ((CellUnit)tabResult[indexLineProduct, lostNumberColonneIndex + 2]).Value += unitValueN1;
            }
            #endregion

        }
        /// <summary>
        /// Get Advertising activity of a product
        /// </summary>
        /// <param name="tabResult">Result Table</param>
        /// <param name="dt">Data Table</param>
        /// <param name="indexLineProduct">Index of product line</param>
        /// <param name="expression">Column name of the field to treat</param>
        /// <param name="filterN">Year N Filter</param>
        /// <param name="filterN1">Year N-1 filter</param>
        protected void GetListProductActivity(ResultTable tabResult, DataTable dt, long indexLineProduct, string expression, string filterN, string filterN1)
        {
            CellIdsNumber unitValueN = (CellIdsNumber)_session.GetCellUnitFactory().Get(0.0);
            CellIdsNumber unitValueN1 = (CellIdsNumber)_session.GetCellUnitFactory().Get(0.0);
            Int64 loyalNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 loyalDeclineNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_DECLINE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 loyalRiseNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_RISE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 wonNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(WON_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 lostNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOST_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            #region Extract data
            //Year N
            DataRow[] rows = dt.Select(filterN);
            string[] ids;            
            foreach (DataRow row in rows)
            {
                ids = row[expression].ToString().Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    unitValueN.Add(Convert.ToDouble(ids[i]));
                }
            }
            //Year N1
            rows = dt.Select(filterN1);
            foreach (DataRow row in rows)
            {
                ids = row[expression].ToString().Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    unitValueN1.Add(Convert.ToDouble(ids[i]));
                }
            }
            #endregion

            #region data dispatch
            Double NValue = unitValueN.Value;
            Double N1Value = unitValueN1.Value;
            bool loyal = NValue > 0 && N1Value > 0;
            bool loyalDecline = NValue > 0  && N1Value > NValue;
            bool loyalRising = NValue > N1Value && N1Value > 0;
            bool won = NValue > 0 && N1Value <= 0;
            bool lost = NValue <= 0 && N1Value > 0;

            #region Get List of Ids
            List<Int64> NIds = new List<Int64>();
            List<Int64> N1Ids = new List<Int64>();
            Int64 l = unitValueN.List.length;
            for (int i = 0; i < l; i++)
            {
                NIds.Add(unitValueN.List.removeHead().UniqueID);
            }
            l = unitValueN1.List.length;
            for (int i = 0; i < l; i++)
            {
                N1Ids.Add(unitValueN1.List.removeHead().UniqueID);
            }
            #endregion

            #region Loyal
            if (loyal)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalNumberColonneIndex]).Value += 1;
                //Unit� N
                ((CellIdsNumber)tabResult[indexLineProduct, loyalNumberColonneIndex + 1]).Add(NIds);
                //Unit� N-1
                ((CellIdsNumber)tabResult[indexLineProduct, loyalNumberColonneIndex + 2]).Add(N1Ids);

            }
            #endregion

            #region Loyals Decline
            if (loyalDecline)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex]).Value += 1;
                //Unit� N
                ((CellIdsNumber)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex + 1]).Add(NIds);
                //Unit� N-1
                ((CellIdsNumber)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex + 2]).Add(N1Ids);

            }
            #endregion

            #region Loyals rising
            if (loyalRising)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalRiseNumberColonneIndex]).Value += 1;
                //Unit� N
                ((CellIdsNumber)tabResult[indexLineProduct, loyalRiseNumberColonneIndex + 1]).Add(NIds);
                //Unit� N-1
                ((CellIdsNumber)tabResult[indexLineProduct, loyalRiseNumberColonneIndex + 2]).Add(N1Ids);

            }
            #endregion

            #region Won
            if (won)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, wonNumberColonneIndex]).Value += 1;
                //Unit� N
                ((CellIdsNumber)tabResult[indexLineProduct, wonNumberColonneIndex + 1]).Add(NIds);
            }
            #endregion

            #region Lost
            if (lost)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, lostNumberColonneIndex]).Value += 1;
                //Unit� N-1
                ((CellIdsNumber)tabResult[indexLineProduct, lostNumberColonneIndex + 2]).Add(N1Ids);
            }
            #endregion

            #endregion
        }
        #endregion

        #region GetNbParutionsByMedia
        /// <summary>
        /// Get Number of parution by media data
        /// </summary>
        /// <returns>Number of parution by media data</returns>
        protected Dictionary<string, double> GetNbParutionsByMedia() {

            #region Variables
            Dictionary<string, double> res = new Dictionary<string, double>();
            double nbParutionsCounter = 0;
            bool start = true;
            string oldKey = "";
            DataTable dt = null;
            #endregion

            #region Chargement des donn�es � partir de la base
            DataSet ds;
            try {
                if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the lost won result"));
                object[] parameters = new object[1];
                parameters[0] = _session;
                ILostWonResultDAL lostwonDAL = (ILostWonResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
                ds = lostwonDAL.GetNbParutionData();
                if (ds != null) dt = ds.Tables[0];
            }
            catch (System.Exception err) {
                throw (new LostWonException("Unable to load data for synthesis report.", err));
            }
            #endregion

            string cKey;
            if (dt != null && dt.Rows.Count > 0) {

                foreach (DataRow dr in dt.Rows) {
                    cKey = string.Format("-{0}-{1}", dr["yearParution"], dr["id_media"]);
                    if (!oldKey.Equals(cKey) && !start) {
                        res.Add(oldKey, nbParutionsCounter);
                        nbParutionsCounter = 0;
                    }
                    nbParutionsCounter += double.Parse(dr["NbParution"].ToString());
                    start = false;
                    oldKey = cKey;
                }
                res.Add(oldKey, nbParutionsCounter);

            }

            return res;

        }

        #endregion

        #endregion

    }

}