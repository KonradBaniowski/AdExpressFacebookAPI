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

using TNS.AdExpressI.LostWon.Exceptions;
using TNS.AdExpressI.LostWon.DAL;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Classification;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using System.Globalization;

#endregion

namespace TNS.AdExpressI.LostWon {
    /// <summary>
    /// Default Dynamic reports
    /// </summary>
    public abstract class LostWonResult : ILostWonResult {

        #region Constantes
        /// <summary>
        /// Match Number of univers + 1 (used in loops)
        /// </summary>
        public const int NB_UNIVERSES_TEST = 4;
        /// <summary>
        /// Id of media univers on year N
        /// </summary>
        public const int N_UNIVERSE_POSITION = 1;
        /// <summary>
        /// Id of media univers on year N-1
        /// </summary>
        public const int N1_UNIVERSE_POSITION = 2;
        /// <summary>
        /// Id of media univers about evol
        /// </summary>
        public const int EVOL_UNIVERSE_POSITION = 3;
        /// <summary>
        /// Index of Level 1 Id element
        /// </summary>
        public const int IDL1_INDEX = 0;
        /// <summary>
        /// Index of Level 1 Label element
        /// </summary>
        public const int LABELL1_INDEX = 1;
        /// <summary>
        /// Index of Level 1 Id element
        /// </summary>
        public const int IDL2_INDEX = 2;
        /// <summary>
        /// Index of Level 1 Label element
        /// </summary>
        public const int LABELL2_INDEX = 3;
        /// <summary>
        /// Index of Level 1 Id element
        /// </summary>
        public const int IDL3_INDEX = 4;
        /// <summary>
        /// Index of Level 1 Label element
        /// </summary>
        public const int LABELL3_INDEX = 5;
        /// <summary>
        /// Index of Adresse Id Column
        /// </summary>
        public const int ADDRESS_COLUMN_INDEX = 6;
        /// <summary>
        /// Index of first Media (*)
        /// </summary>
        public const int FIRST_MEDIA_INDEX = 7;
        /// <summary>
        /// Id of subtotal columns
        /// </summary>
        public const int SUBTOTAL_ID = -5;
        /// <summary>
        /// Id of label columns
        /// </summary>
        public const int LEVEL_ID = 6;
        /// <summary>
        /// Id of Media Schedule Column
        /// </summary>
        public const int MEDIA_SCHEDULE_ID = 7;
        /// <summary>
        /// Id of column loyal
        /// </summary>
        public const int LOYAL_HEADER_ID = 8;
        /// <summary>
        /// Id of column loyal sliding
        /// </summary>
        public const int LOYAL_DECLINE_HEADER_ID = 9;
        /// <summary>
        /// Id of column loyal rising
        /// </summary>
        public const int LOYAL_RISE_HEADER_ID = 10;
        /// <summary>
        /// Id of column Won
        /// </summary>
        public const int WON_HEADER_ID = 11;
        /// <summary>
        /// Id of column Lost
        /// </summary>
        public const int LOST_HEADER_ID = 12;
        /// <summary>
        /// Id of column Item Number
        /// </summary>
        public const int ITEM_NUMBER_HEADER_ID = 13;
        /// <summary>
        /// Id of column Unit
        /// </summary>
        public const int UNIT_HEADER_ID = 14;
        /// <summary>
        /// Index of first line in result table
        /// </summary>
        public const int FIRST_LINE_RESULT_INDEX = 4;

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

            #region Sélection du vehicle
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
        protected ResultTable GetData() {

            #region Variable
            string beginningPeriod = GetDateBegin();
            string endPeriod = GetDateEnd();
            #endregion

            #region Build Indexes Table
            List<SelectionGroup> groupMediaTotalIndex = new List<SelectionGroup>();
            List<SelectionSubGroup> subGroupMediaTotalIndex = new List<SelectionSubGroup>();
            Dictionary<Int64, GroupItemForTableResult> mediaIndex = new Dictionary<Int64, GroupItemForTableResult>();
            Dictionary<Int64, GroupItemForTableResult> mediaEvolIndex = new Dictionary<Int64, GroupItemForTableResult>();
            string mediaListForLabelSearch = "";
            int maxIndex = 0;
            long nbLineInNewTable = 0;
            #endregion

            #region Chargement du tableau
            object[,] tabData = GetPreformatedTable(groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref maxIndex, ref nbLineInNewTable, beginningPeriod, endPeriod, ref mediaListForLabelSearch);
            #endregion

            #region No Data
            if (tabData == null) {
                return null;
            }
            #endregion

            #region Data Filtering
            object[,] tabResult = null;
            long nbLineInTabResult = 0; //currentLineInTabResult + 1;
            long nbCol = tabData.GetLength(0);
            switch (this._result) {
                case DynamicAnalysis.PORTEFEUILLE:
                    nbLineInTabResult = nbLineInNewTable;
                    tabResult = tabData;
                    break;
                case DynamicAnalysis.LOST:
                case DynamicAnalysis.WON:
                    tabResult = FilterWonLost((this._result == DynamicAnalysis.WON), tabData, groupMediaTotalIndex, nbLineInNewTable, ref nbLineInTabResult, nbCol);
                    break;
                case DynamicAnalysis.LOYAL:
                case DynamicAnalysis.LOYAL_DECLINE:
                case DynamicAnalysis.LOYAL_RISE:
                    tabResult = FilterLoyal((this._result != DynamicAnalysis.LOYAL), (this._result == DynamicAnalysis.LOYAL_RISE), tabData, groupMediaTotalIndex, nbLineInNewTable, ref nbLineInTabResult, nbCol);
                    break;
                default:
                    break;
            }
            #endregion

            return GetResultTable(tabResult, nbLineInTabResult, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, mediaListForLabelSearch);

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

            string expression = FctWeb.SQLGenerator.GetUnitAliasSum(_session);
            string filterN = "";
            string filterN1 = "";
            DataTable dt = null;
            string beginningPeriodDA = GetDateBegin();
            string endPeriodDA = GetDateEnd();
            #endregion

            #region Calcul des périodes
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

            string beginningPeriodN1DA = customerPeriod.ComparativeStartDate;
            string endPeriodN1DA = customerPeriod.ComparativeEndDate;
            DateTime PeriodBeginningDate = new DateTime(int.Parse(customerPeriod.StartDate.Substring(0, 4)), int.Parse(customerPeriod.StartDate.Substring(4, 2)), int.Parse(customerPeriod.StartDate.Substring(6, 2)));
            DateTime PeriodEndDate = new DateTime(int.Parse(customerPeriod.EndDate.Substring(0, 4)), int.Parse(customerPeriod.EndDate.Substring(4, 2)), int.Parse(customerPeriod.EndDate.Substring(6, 2))); ;

            DateTime PeriodBeginningDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeStartDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeStartDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeStartDate.Substring(6, 2)));
            DateTime PeriodEndDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeEndDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeEndDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeEndDate.Substring(6, 2))); ;

            string PeriodDateN = DateString.dateTimeToDD_MM_YYYY(PeriodBeginningDate,_session.SiteLanguage) + "-" + DateString.dateTimeToDD_MM_YYYY(PeriodEndDate,_session.SiteLanguage);
            string PeriodDateN1 = DateString.dateTimeToDD_MM_YYYY(PeriodBeginningDateN1DA, _session.SiteLanguage) + "-" + DateString.dateTimeToDD_MM_YYYY(PeriodEndDateN1DA, _session.SiteLanguage);

            #endregion

            #region Aucune données
            if (PeriodBeginningDate > DateTime.Now) {
                return null;
            }
            #endregion

            #region Chargement des données à partir de la base

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

            #region Identifiant du texte des unités
            Int64 unitId = _session.GetUnitLabelId();
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            #endregion

            #region Création des headers
            nbLine = 5;
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

            #region Fidèle
            HeaderGroup fidele = new HeaderGroup(GestionWeb.GetWebWord(1241, _session.SiteLanguage), LOYAL_HEADER_ID);
            fidele.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitFidele = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitFidele.Add(new Header(true, PeriodDateN, N_UNIVERSE_POSITION));
            unitFidele.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_POSITION));
            fidele.Add(unitFidele);
            headers.Root.Add(fidele);
            #endregion

            #region Fidèle en baisse
            HeaderGroup fideleDecline = new HeaderGroup(GestionWeb.GetWebWord(1242, _session.SiteLanguage), LOYAL_DECLINE_HEADER_ID);
            fideleDecline.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitFideleDecline = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitFideleDecline.Add(new Header(true, PeriodDateN, N_UNIVERSE_POSITION));
            unitFideleDecline.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_POSITION));
            fideleDecline.Add(unitFideleDecline);
            headers.Root.Add(fideleDecline);
            #endregion

            #region Fidèle en hausse
            HeaderGroup fideleRise = new HeaderGroup(GestionWeb.GetWebWord(1243, _session.SiteLanguage), LOYAL_RISE_HEADER_ID);
            fideleRise.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitFideleRise = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitFideleRise.Add(new Header(true, PeriodDateN, N_UNIVERSE_POSITION));
            unitFideleRise.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_POSITION));
            fideleRise.Add(unitFideleRise);
            headers.Root.Add(fideleRise);
            #endregion

            #region Gagnés
            HeaderGroup won = new HeaderGroup(GestionWeb.GetWebWord(1244, _session.SiteLanguage), WON_HEADER_ID);
            won.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitWon = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitWon.Add(new Header(true, PeriodDateN, N_UNIVERSE_POSITION));
            unitWon.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_POSITION));
            won.Add(unitWon);
            headers.Root.Add(won);
            #endregion

            #region Perdus
            HeaderGroup lost = new HeaderGroup(GestionWeb.GetWebWord(1245, _session.SiteLanguage), LOST_HEADER_ID);
            lost.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitLost = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitLost.Add(new Header(true, PeriodDateN, N_UNIVERSE_POSITION));
            unitLost.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_POSITION));
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
            productLineIndex = resultTable.AddNewLine(LineType.level1);
            resultTable[productLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1164, _session.SiteLanguage));
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
                resultTable[i, _loyalNumberColonneIndex] = new CellNumber(0.0);
                resultTable[i, _loyalDeclineNumberColonneIndex] = new CellNumber(0.0);
                resultTable[i, _loyalRiseNumberColonneIndex] = new CellNumber(0.0);
                resultTable[i, _wonNumberColonneIndex] = new CellNumber(0.0);
                resultTable[i, _lostNumberColonneIndex] = new CellNumber(0.0);
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

            #region Traitement des données
            foreach (DataRow currentRow in dt.Rows) {

                //Activité publicitaire Annonceurs
                if (currentRow["id_advertiser"] != null && currentRow["id_advertiser"] != System.DBNull.Value && !advertisers.Contains(currentRow["id_advertiser"].ToString())) {
                    filterN = "id_advertiser=" + currentRow["id_advertiser"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                    filterN1 = "id_advertiser=" + currentRow["id_advertiser"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    GetProductActivity(resultTable, dt, advertiserLineIndex, expression, filterN, filterN1);
                    advertisers.Add(currentRow["id_advertiser"].ToString());
                }

                if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) {//droits marques
                    //Activité publicitaire marques
                    if (currentRow["id_brand"] != null && currentRow["id_brand"] != System.DBNull.Value && !brands.Contains(currentRow["id_brand"].ToString())) {
                        filterN = "id_brand=" + currentRow["id_brand"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                        filterN1 = "id_brand=" + currentRow["id_brand"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                        GetProductActivity(resultTable, dt, brandLineIndex, expression, filterN, filterN1);
                        brands.Add(currentRow["id_brand"].ToString());
                    }
                }

                //Activité publicitaire produits
                if (currentRow["id_product"] != null && currentRow["id_product"] != System.DBNull.Value && !products.Contains(currentRow["id_product"].ToString())) {
                    filterN = "id_product=" + currentRow["id_product"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                    filterN1 = "id_product=" + currentRow["id_product"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    GetProductActivity(resultTable, dt, productLineIndex, expression, filterN, filterN1);
                    products.Add(currentRow["id_product"].ToString());
                }

                //Activité publicitaire Famille
                if (currentRow["id_sector"] != null && currentRow["id_sector"] != System.DBNull.Value && !sectors.Contains(currentRow["id_sector"].ToString())) {
                    filterN = "id_sector=" + currentRow["id_sector"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                    filterN1 = "id_sector=" + currentRow["id_sector"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    GetProductActivity(resultTable, dt, sectorLineIndex, expression, filterN, filterN1);
                    sectors.Add(currentRow["id_sector"].ToString());
                }
                //Activité publicitaire Classe
                if (currentRow["id_subsector"] != null && currentRow["id_subsector"] != System.DBNull.Value && !subsectors.Contains(currentRow["id_subsector"].ToString())) {
                    filterN = "id_subsector=" + currentRow["id_subsector"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))"; ;
                    filterN1 = "id_subsector=" + currentRow["id_subsector"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    GetProductActivity(resultTable, dt, subsectorLineIndex, expression, filterN, filterN1);
                    subsectors.Add(currentRow["id_subsector"].ToString());
                }
                //Activité publicitaire Groupes
                if (currentRow["id_group_"] != null && currentRow["id_group_"] != System.DBNull.Value && !groups.Contains(currentRow["id_group_"].ToString())) {
                    filterN = "id_group_=" + currentRow["id_group_"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                    filterN1 = "id_group_=" + currentRow["id_group_"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                    GetProductActivity(resultTable, dt, groupLineIndex, expression, filterN, filterN1);
                    groups.Add(currentRow["id_group_"].ToString());
                }

                if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MEDIA_AGENCY)) {//droits agences média					
                    //activité publicitaire Groupes d'agences
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency) && currentRow["ID_GROUP_ADVERTISING_AGENCY"] != null && currentRow["ID_GROUP_ADVERTISING_AGENCY"] != System.DBNull.Value && !agencyGroups.Contains(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString())) {
                        filterN = "ID_GROUP_ADVERTISING_AGENCY=" + currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                        filterN1 = "ID_GROUP_ADVERTISING_AGENCY=" + currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                        GetProductActivity(resultTable, dt, agencyGroupLineIndex, expression, filterN, filterN1);
                        agencyGroups.Add(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString());
                    }

                    //activité publicitaire agence
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency) && currentRow["ID_ADVERTISING_AGENCY"] != null && currentRow["ID_ADVERTISING_AGENCY"] != System.DBNull.Value && !agency.Contains(currentRow["ID_ADVERTISING_AGENCY"].ToString())) {
                        filterN = "ID_ADVERTISING_AGENCY=" + currentRow["ID_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodDA + " AND date_num<=" + endPeriodDA + ") or (date_num>=" + beginningPeriodDA.Substring(0, 6) + " AND date_num<=" + endPeriodDA.Substring(0, 6) + "))";
                        filterN1 = "ID_ADVERTISING_AGENCY=" + currentRow["ID_ADVERTISING_AGENCY"].ToString() + " AND ((date_num>=" + beginningPeriodN1DA + " AND date_num<=" + endPeriodN1DA + ") or (date_num>=" + beginningPeriodN1DA.Substring(0, 6) + " AND date_num<=" + endPeriodN1DA.Substring(0, 6) + "))";
                        GetProductActivity(resultTable, dt, agencyLineIndex, expression, filterN, filterN1);
                        agency.Add(currentRow["ID_ADVERTISING_AGENCY"].ToString());
                    }
                }
            }
            #endregion

            return (resultTable);
        }
        #endregion

        #region Internal methods

        #region Formatage des dates
        /// <summary>
        /// Get Period Beginning
        /// </summary>
        /// <returns>Period Beginning</returns>
        protected string GetDateBegin() {
            return (_session.PeriodBeginningDate);
        }

        /// <summary>
        /// Get Period End
        /// </summary>
        /// <returns>Period End</returns>
        protected string GetDateEnd() {
            return (_session.PeriodEndDate);
        }
        #endregion

        #region Tableau Préformaté
        /// <summary>
        /// Get Preformated Table
        /// </summary>
        /// <param name="groupMediaTotalIndex">List of indexes about selected groups</param>
        /// <param name="subGroupMediaTotalIndex">List of indexes about selected subGroups</param>
        /// <param name="mediaIndex">List of indexes of media</param>
        /// <param name="nbCol">Number of columns in Result Table</param>
        /// <param name="nbLineInNewTable">(out) Number of lines in result table</param>
        /// <param name="beginningPeriod">Beginning of the period</param>
        /// <param name="endPeriod">End of the period</param>
        /// <param name="mediaEvolIndex">List of indexes of media evol</param>
        /// <param name="mediaListForLabelSearch">(out) Media Ids</param>
        /// <returns>Preformated result table</returns>
        protected object[,] GetPreformatedTable(List<SelectionGroup> groupMediaTotalIndex, List<SelectionSubGroup> subGroupMediaTotalIndex, Dictionary<Int64, GroupItemForTableResult> mediaIndex, Dictionary<Int64, GroupItemForTableResult> mediaEvolIndex, ref int nbCol, ref long nbLineInNewTable, string beginningPeriod, string endPeriod, ref string mediaListForLabelSearch) {

            #region Variables
            Int64 idMedia = -1;
            double unit;
            long oldIdL1 = -1;
            long oldIdL2 = -1;
            long oldIdL3 = -1;
            Int64 currentLine = -1;
            int k;
            bool changeLine = false;
            #endregion

            #region Formattage des dates

            Int64 beginningDate = Int64.Parse(beginningPeriod);
            Int64 endPeriodDate = Int64.Parse(endPeriod);
            DateTime startDate = new DateTime(int.Parse(beginningPeriod.Substring(0, 4)), int.Parse(beginningPeriod.Substring(4, 2)), int.Parse(beginningPeriod.Substring(6, 2)));

            #endregion

            #region Aucune données
            if (startDate > DateTime.Now) {
                return null;
            }
            #endregion

            #region Chargement des données à partir de la base
            DataSet ds = null;
            DataSet dsMedia = null;
            Navigation.Module currentModuleDescription = Navigation.ModulesList.GetModule(_session.CurrentModule);
            try {
                if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the lost won result"));
                object[] parameters = new object[1];
                parameters[0] = _session;
                ILostWonResultDAL lostwonDAL = (ILostWonResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
                ds = lostwonDAL.GetData();
                dsMedia = lostwonDAL.GetMediaDetails();

                //ds = DynamicDataAccess.GetGenericData(webSession, vehicleName);
                //dsMedia = DynamicDataAccess.GetMediaColumnDetailLevelList(webSession);
            }
            catch (System.Exception err) {
                throw (new LostWonException("Unable to load dynamic report data.", err));
            }
            DataTable dt = ds.Tables[0];
            DataTable dtMedia = dsMedia.Tables[0];
            #endregion

            #region Aucune données
            if (dt.Rows.Count == 0) {
                return null;
            }
            #endregion

            #region Tableaux d'index
            InitIndexAndValues(groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaEvolIndex, ref mediaListForLabelSearch, ref nbCol, dtMedia);
            #endregion

            #region Déclaration du tableau de résultat
            long nbline = dt.Rows.Count;
            object[,] tabResult = new object[nbCol, dt.Rows.Count];
            #endregion

            #region Tableau de résultat

            string unitAliasName = FctWeb.SQLGenerator.GetUnitAlias(_session);

            foreach (DataRow currentRow in dt.Rows) {
                idMedia = (Int64)currentRow["id_media"];
                if (_session.GenericProductDetailLevel.GetIdValue(currentRow, 1) >= 0 && _session.GenericProductDetailLevel.GetIdValue(currentRow, 1) != oldIdL1) changeLine = true;
                if (!changeLine && _session.GenericProductDetailLevel.GetIdValue(currentRow, 2) >= 0 && _session.GenericProductDetailLevel.GetIdValue(currentRow, 2) != oldIdL2) changeLine = true;
                if (!changeLine && _session.GenericProductDetailLevel.GetIdValue(currentRow, 3) >= 0 && _session.GenericProductDetailLevel.GetIdValue(currentRow, 3) != oldIdL3) changeLine = true;


                #region On change de ligne
                if (changeLine) {

                    currentLine++;
                    // Ecriture de L1 ?
                    if (_session.GenericProductDetailLevel.GetIdValue(currentRow, 1) >= 0) {
                        oldIdL1 = _session.GenericProductDetailLevel.GetIdValue(currentRow, 1);
                        tabResult[IDL1_INDEX, currentLine] = oldIdL1;
                        tabResult[LABELL1_INDEX, currentLine] = _session.GenericProductDetailLevel.GetLabelValue(currentRow, 1);
                    }
                    // Ecriture de L2 ?
                    if (_session.GenericProductDetailLevel.GetIdValue(currentRow, 2) >= 0) {
                        oldIdL2 = _session.GenericProductDetailLevel.GetIdValue(currentRow, 2);
                        tabResult[IDL2_INDEX, currentLine] = oldIdL2;
                        tabResult[LABELL2_INDEX, currentLine] = _session.GenericProductDetailLevel.GetLabelValue(currentRow, 2);
                    }
                    // Ecriture de L3 ?
                    if (_session.GenericProductDetailLevel.GetIdValue(currentRow, 3) >= 0) {
                        oldIdL3 = _session.GenericProductDetailLevel.GetIdValue(currentRow, 3);
                        tabResult[IDL3_INDEX, currentLine] = oldIdL3;
                        tabResult[LABELL3_INDEX, currentLine] = _session.GenericProductDetailLevel.GetLabelValue(currentRow, 3);
                    }
                    // Totaux, sous Totaux et médias à 0
                    for (k = FIRST_MEDIA_INDEX; k < nbCol; k++) {
                        tabResult[k, currentLine] = (double)0.0;
                    }

                    try {
                        if (currentRow["id_address"] != null) tabResult[ADDRESS_COLUMN_INDEX, currentLine] = Int64.Parse(currentRow["id_address"].ToString());
                    }
                    catch (Exception) {

                    }
                    changeLine = false;
                }
                #endregion

                unit = double.Parse(currentRow[unitAliasName].ToString());


                if (IsComparativeDateLine(Int64.Parse(currentRow["date_num"].ToString()), beginningDate, endPeriodDate)) {
                    idMedia = -1 * idMedia;
                }
                // Ecriture du résultat du média
                tabResult[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].IndexInResultTable, currentLine] = (double)tabResult[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].IndexInResultTable, currentLine] + unit;

                // Ecriture du résultat du sous total (somme)
                if (groupMediaTotalIndex[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].ParentId].Count > 1) {
                    tabResult[groupMediaTotalIndex[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].ParentId].IndexInResultTable, currentLine] = (double)tabResult[groupMediaTotalIndex[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].ParentId].IndexInResultTable, currentLine] + unit;
                }

            }

            #endregion

            #region Debug: voir le tableau
#if(DEBUG)
            //						int i,j;
            //						string HTML="<html><table><tr>";
            //						for(i=0;i<=currentLine;i++){
            //							for(j=0;j<nbCol;j++){
            //								if(tabResult[j,i]!=null)HTML+="<td>"+tabResult[j,i].ToString()+"</td>";
            //								else HTML+="<td>&nbsp;</td>";
            //							}
            //							HTML+="</tr><tr>";
            //						}
            //						HTML+="</tr></table></html>";
#endif
            #endregion

            nbLineInNewTable = currentLine + 1;
            return (tabResult);
        }
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

        #region Initialisation des indexes
        /// <summary>
        /// Init indexes tables
        /// </summary>
        /// <param name="groupMediaTotalIndex">(out) List of indexes of selection groups</param>
        /// <param name="subGroupMediaTotalIndex">List of indexes of selection subgroups</param>
        /// <param name="mediaIndex">(out) Media indexes</param>
        /// <param name="mediaListForLabelSearch">(out) Media Ids</param>
        /// <param name="maxIndex">(out) Index of last column</param>
        /// <param name="mediaEvolIndex">Indexes list for evol</param>
        /// <param name="dtMedia">List of media with the matching level of detail</param>
        protected void InitIndexAndValues(List<SelectionGroup> groupMediaTotalIndex, List<SelectionSubGroup> subGroupMediaTotalIndex, Dictionary<Int64, GroupItemForTableResult> mediaIndex, Dictionary<Int64, GroupItemForTableResult> mediaEvolIndex, ref string mediaListForLabelSearch, ref int maxIndex, DataTable dtMedia) {

            #region Variables
            string tmp = "";
            Int64[] mediaList;
            int positionSubGroup = 2;
            int subGroupCount = 0;
            Dictionary<Int64, int> mediaSubGroupNId = new Dictionary<Int64, int>();
            Dictionary<Int64, int> mediaSubGroupN1Id = new Dictionary<Int64, int>();
            Dictionary<Int64, int> mediaSubGroupEvolId = new Dictionary<Int64, int>();
            List<int> columnDetailLevelList;
            Int64 currentMedia;
            int currentColumnDetail;
            #endregion

            #region Initialisation des variables
            maxIndex = FIRST_MEDIA_INDEX;
            #endregion

            tmp = _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[N_UNIVERSE_POSITION], CstCustom.Right.type.mediaAccess);
            mediaList = Array.ConvertAll<string, Int64>(tmp.Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); });

            groupMediaTotalIndex.Add(null);
            subGroupMediaTotalIndex.Add(null);
            subGroupMediaTotalIndex.Add(null);

            // Chargement de la liste du niveau de détail colonne
            for (int i = 1; i <= 3; i++) {

                subGroupCount = 0;
                mediaListForLabelSearch = "";
                columnDetailLevelList = new List<int>();

                foreach (Int64 idMedia in mediaList) {
                    foreach (DataRow row in dtMedia.Rows) {
                        currentMedia = Convert.ToInt64(row["id_media"]);
                        if (idMedia == currentMedia) {
                            currentColumnDetail = Convert.ToInt32(row["columnDetailLevel"]);
                            if (!columnDetailLevelList.Contains(currentColumnDetail)) {
                                columnDetailLevelList.Add(currentColumnDetail);
                                subGroupMediaTotalIndex.Add(new SelectionSubGroup(positionSubGroup));
                                subGroupMediaTotalIndex[positionSubGroup].DataBaseId = currentColumnDetail;
                                switch (i) {
                                    case 1:
                                        subGroupMediaTotalIndex[positionSubGroup].ParentId = N_UNIVERSE_POSITION;
                                        mediaSubGroupNId.Add(idMedia, positionSubGroup);
                                        break;
                                    case 2:
                                        subGroupMediaTotalIndex[positionSubGroup].ParentId = N1_UNIVERSE_POSITION;
                                        mediaSubGroupN1Id.Add(idMedia, positionSubGroup);
                                        break;
                                    case 3:
                                        subGroupMediaTotalIndex[positionSubGroup].ParentId = EVOL_UNIVERSE_POSITION;
                                        mediaSubGroupEvolId.Add(idMedia, positionSubGroup);
                                        break;
                                }
                                subGroupMediaTotalIndex[positionSubGroup].SetItemsNumber = 0;
                                subGroupMediaTotalIndex[positionSubGroup].IndexInResultTable = 0;
                                positionSubGroup++;
                                subGroupCount++;
                                mediaListForLabelSearch += currentColumnDetail + ",";
                            }
                            else {
                                foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex)
                                    if (subGroup != null) {
                                        if (subGroup.DataBaseId == currentColumnDetail) {
                                            if (subGroup.Count == 0)
                                                subGroup.SetItemsNumber = 2;
                                            else
                                                subGroup.SetItemsNumber = subGroup.Count + 1;
                                            switch (i) {
                                                case 1:
                                                    mediaSubGroupNId[idMedia] = subGroup.Id;
                                                    break;
                                                case 2:
                                                    mediaSubGroupN1Id[idMedia] = subGroup.Id;
                                                    break;
                                                case 3:
                                                    mediaSubGroupEvolId[idMedia] = subGroup.Id;
                                                    break;
                                            }
                                        }
                                    }
                            }
                        }
                    }
                }
            }

            #region Année N
            // Définition du groupe
            groupMediaTotalIndex.Add(new SelectionGroup(N_UNIVERSE_POSITION));
            // Le groupe contient plus de 1 éléments
            if (subGroupCount > 1) {
                groupMediaTotalIndex[N_UNIVERSE_POSITION].IndexInResultTable = maxIndex;
                groupMediaTotalIndex[N_UNIVERSE_POSITION].SetItemsNumber = subGroupCount;
                // Changement pourcentage
                maxIndex++;
                //nbSubTotal++;
            }
            else {
                groupMediaTotalIndex[N_UNIVERSE_POSITION].IndexInResultTable = maxIndex;
                groupMediaTotalIndex[N_UNIVERSE_POSITION].SetItemsNumber = 0;
            }
            // Indexes des média (support)
            foreach (Int64 media in mediaList) {
                mediaIndex.Add(media, new GroupItemForTableResult(media, mediaSubGroupNId[media], maxIndex));
            }
            // Pour les sous Groupes
            foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
                if (subGroup != null) {
                    if (subGroup.IndexInResultTable == 0 && subGroup.ParentId == N_UNIVERSE_POSITION) {
                        subGroup.IndexInResultTable = maxIndex;
                        maxIndex++;
                    }
                }
            }
            #endregion

            #region Année N -1
            // Définition du groupe
            groupMediaTotalIndex.Add(new SelectionGroup(N1_UNIVERSE_POSITION));
            // Le groupe contient plus de 1 éléments
            if (subGroupCount > 1) {
                groupMediaTotalIndex[N1_UNIVERSE_POSITION].IndexInResultTable = maxIndex;
                groupMediaTotalIndex[N1_UNIVERSE_POSITION].SetItemsNumber = subGroupCount;
                maxIndex++;
                //nbSubTotal++;
            }
            else {
                groupMediaTotalIndex[N1_UNIVERSE_POSITION].IndexInResultTable = maxIndex;
                groupMediaTotalIndex[N1_UNIVERSE_POSITION].SetItemsNumber = 0;
            }
            // Indexes des média (support)
            foreach (Int64 media in mediaList) {
                mediaIndex.Add(-1 * media, new GroupItemForTableResult(-1 * media, mediaSubGroupN1Id[media], maxIndex));
            }
            // Pour les sous Groupes
            foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
                if (subGroup != null) {
                    if (subGroup.IndexInResultTable == 0 && subGroup.ParentId == N1_UNIVERSE_POSITION) {
                        subGroup.IndexInResultTable = maxIndex;
                        maxIndex++;
                    }
                }
            }
            #endregion

            #region Evol
            // Définition du groupe
            groupMediaTotalIndex.Add(new SelectionGroup(EVOL_UNIVERSE_POSITION));
            // Le groupe contient plus de 1 éléments
            if (subGroupCount > 1) {
                groupMediaTotalIndex[EVOL_UNIVERSE_POSITION].IndexInResultTable = maxIndex;
                groupMediaTotalIndex[EVOL_UNIVERSE_POSITION].SetItemsNumber = subGroupCount;
                maxIndex++;
                //nbSubTotal++;
            }
            else {
                groupMediaTotalIndex[EVOL_UNIVERSE_POSITION].IndexInResultTable = maxIndex;
                groupMediaTotalIndex[EVOL_UNIVERSE_POSITION].SetItemsNumber = 0;
            }
            // Indexes des média (support)
            foreach (Int64 media in mediaList) {
                mediaEvolIndex[media] = new GroupItemForTableResult(media, mediaSubGroupEvolId[media], maxIndex);
            }
            // Pour les sous Groupes
            foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
                if (subGroup != null) {
                    if (subGroup.IndexInResultTable == 0 && subGroup.ParentId == EVOL_UNIVERSE_POSITION) {
                        subGroup.IndexInResultTable = maxIndex;
                        maxIndex++;
                    }
                }
            }
            #endregion

            mediaListForLabelSearch = mediaListForLabelSearch.Substring(0, mediaListForLabelSearch.Length - 1);
        }

        #endregion

        #region Formattage d'un tableau de résultat
        /// <summary>
        /// Create ResultTable
        /// </summary>
        /// <param name="tabData">Data table</param>
        /// <param name="nbLineInTabData">Numùber of lines in table</param>
        /// <param name="groupMediaTotalIndex">Media Group</param>
        /// <param name="subGroupMediaTotalIndex">List of subgroups selection</param>
        /// <param name="mediaIndex">Index of medias</param>
        /// <param name="mediaEvolIndex">Index of media evols</param>
        /// <param name="mediaListForLabelSearch">List of media Ids</param>
        /// <returns>Result</returns>
        protected ResultTable GetResultTable(object[,] tabData, long nbLineInTabData, List<SelectionGroup> groupMediaTotalIndex, List<SelectionSubGroup> subGroupMediaTotalIndex, Dictionary<Int64, GroupItemForTableResult> mediaIndex, Dictionary<Int64, GroupItemForTableResult> mediaEvolIndex, string mediaListForLabelSearch) {

            #region Variables
            Int64[] mediaList;
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            long currentLine;
            long currentLineInTabResult;
            long k;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Aucune données
            if (nbLineInTabData == 0) {
                return null;
            }
            #endregion

            #region Calcul des PDM ?
            bool computePDM = false;
            if (_session.Percentage) computePDM = true;
            #endregion

            #region Affiche le Gad ?
            bool showGad = false;
            int advertiserColumnIndex = _session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser);
            if (advertiserColumnIndex > 0) {
                showGad = true;
            }
            #endregion

            // Chargement des libellés de colonnes
            DBClassif.MediaBranch.PartialMediaListDataAccess mediaLabelList = null;
            DBClassif.MediaBranch.PartialCategoryListDataAccess categoryLabelList = null;
            DBClassif.MediaBranch.PartialMediaSellerListDataAccess mediaSellerLabelList = null;
            DBClassif.MediaBranch.PartialTitleListDataAccess titleLabelList = null;
            DBClassif.MediaBranch.PartialInterestCenterListDataAccess interestCenterLabelList = null;

            switch (columnDetailLevel.Id) {

                case DetailLevelItemInformation.Levels.media:
                    mediaLabelList = new DBClassif.MediaBranch.PartialMediaListDataAccess(mediaListForLabelSearch, _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.category:
                    categoryLabelList = new DBClassif.MediaBranch.PartialCategoryListDataAccess(mediaListForLabelSearch, _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.mediaSeller:
                    mediaSellerLabelList = new DBClassif.MediaBranch.PartialMediaSellerListDataAccess(mediaListForLabelSearch, _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.title:
                    titleLabelList = new DBClassif.MediaBranch.PartialTitleListDataAccess(mediaListForLabelSearch, _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.interestCenter:
                    interestCenterLabelList = new DBClassif.MediaBranch.PartialInterestCenterListDataAccess(mediaListForLabelSearch, _session.DataLanguage, _session.Source);
                    break;

            }

            // Nombre d'éléments dans un groupe
            mediaList = Array.ConvertAll<string, Int64>(mediaListForLabelSearch.Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); });

            #region Calcul des périodes
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

            DateTime PeriodBeginningDate = new DateTime(int.Parse(customerPeriod.StartDate.Substring(0, 4)), int.Parse(customerPeriod.StartDate.Substring(4, 2)), int.Parse(customerPeriod.StartDate.Substring(6, 2)));
            DateTime PeriodEndDate = new DateTime(int.Parse(customerPeriod.EndDate.Substring(0, 4)), int.Parse(customerPeriod.EndDate.Substring(4, 2)), int.Parse(customerPeriod.EndDate.Substring(6, 2))); ;

            DateTime PeriodBeginningDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeStartDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeStartDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeStartDate.Substring(6, 2)));
            DateTime PeriodEndDateN1DA = new DateTime(int.Parse(customerPeriod.ComparativeEndDate.Substring(0, 4)), int.Parse(customerPeriod.ComparativeEndDate.Substring(4, 2)), int.Parse(customerPeriod.ComparativeEndDate.Substring(6, 2))); ;

            string PeriodDateN = DateString.dateTimeToDD_MM_YYYY(PeriodBeginningDate,_session.SiteLanguage) + "-" + DateString.dateTimeToDD_MM_YYYY(PeriodEndDate,_session.SiteLanguage);

            string PeriodDateN1 = DateString.dateTimeToDD_MM_YYYY(PeriodBeginningDateN1DA,_session.SiteLanguage) + "-" + DateString.dateTimeToDD_MM_YYYY(PeriodEndDateN1DA,_session.SiteLanguage);

            #endregion

            #region Headers
            // Ajout de la colonne Produit
            Headers headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), LEVEL_ID));
            // Ajout plan media ?
            bool showMediaSchedule = false;
            if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.holdingCompany) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.sector) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subSector) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.group)
                ) {
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(150, _session.SiteLanguage), MEDIA_SCHEDULE_ID));
                showMediaSchedule = true;
            }

            bool addSubTotal = false;
            if (mediaList.Length > 1) addSubTotal = true;

            #region Ajout Année N
            HeaderGroup yearN = new HeaderGroup(PeriodDateN, true, N_UNIVERSE_POSITION);
            // Ajout sous total
            if (addSubTotal) yearN.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUBTOTAL_ID);
            // Media
            foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
                if (subGroup != null) {
                    if (subGroup.ParentId == N_UNIVERSE_POSITION)
                        switch (columnDetailLevel.Id) {

                            case DetailLevelItemInformation.Levels.media:
                                yearN.Add(new Header(true, mediaLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.category:
                                yearN.Add(new Header(true, categoryLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.mediaSeller:
                                yearN.Add(new Header(true, mediaSellerLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.title:
                                yearN.Add(new Header(true, titleLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.interestCenter:
                                yearN.Add(new Header(true, interestCenterLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;

                        }
                }
            }
            headers.Root.Add(yearN);
            #endregion

            #region  Ajout Année N-1
            HeaderGroup yearN1 = new HeaderGroup(PeriodDateN1, true, N1_UNIVERSE_POSITION);
            // Ajout sous total
            if (addSubTotal) yearN1.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUBTOTAL_ID);
            // Media
            foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
                if (subGroup != null) {
                    if (subGroup.ParentId == N1_UNIVERSE_POSITION)
                        switch (columnDetailLevel.Id) {

                            case DetailLevelItemInformation.Levels.media:
                                yearN1.Add(new Header(true, mediaLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.category:
                                yearN1.Add(new Header(true, categoryLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.mediaSeller:
                                yearN1.Add(new Header(true, mediaSellerLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.title:
                                yearN1.Add(new Header(true, titleLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.interestCenter:
                                yearN1.Add(new Header(true, interestCenterLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;

                        }
                }
            }
            headers.Root.Add(yearN1);
            #endregion

            #region  Ajout Année Evol
            HeaderGroup evol = new HeaderGroup(GestionWeb.GetWebWord(1212, _session.SiteLanguage), true, EVOL_UNIVERSE_POSITION);
            // Ajout sous total
            if (addSubTotal) evol.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUBTOTAL_ID);
            // Media
            foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex) {
                if (subGroup != null) {
                    if (subGroup.ParentId == EVOL_UNIVERSE_POSITION)
                        switch (columnDetailLevel.Id) {

                            case DetailLevelItemInformation.Levels.media:
                                evol.Add(new Header(true, mediaLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.category:
                                evol.Add(new Header(true, categoryLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.mediaSeller:
                                evol.Add(new Header(true, mediaSellerLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.title:
                                evol.Add(new Header(true, titleLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;
                            case DetailLevelItemInformation.Levels.interestCenter:
                                evol.Add(new Header(true, interestCenterLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                break;

                        }
                }
            }
            headers.Root.Add(evol);
            #endregion

            #endregion

            #region Déclaration du tableau de résultat
            long nbLine = GetNbLineFromPreformatedTableToResultTable(tabData) + 1;
            #region Add Line for  Nb parution data
            Dictionary<string, double> resNbParution = null;
            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.media && (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id)) {
                resNbParution = GetNbParutionsByMedia();
                if (resNbParution != null && resNbParution.Count > 0)
                    nbLine = nbLine + 1;
            }
            #endregion
            ResultTable resultTable = new ResultTable(nbLine, headers);
            long nbCol = resultTable.ColumnsNumber - 2;
            long NStartColIndex = -1;
            if (addSubTotal) NStartColIndex = resultTable.GetHeadersIndexInResultTable(N_UNIVERSE_POSITION + "-" + SUBTOTAL_ID);
            else NStartColIndex = resultTable.GetHeadersIndexInResultTable(N_UNIVERSE_POSITION + "-" + mediaList[0]);
            long N1StartColIndex = -1;
            if (addSubTotal) N1StartColIndex = resultTable.GetHeadersIndexInResultTable(N1_UNIVERSE_POSITION + "-" + SUBTOTAL_ID);
            else N1StartColIndex = resultTable.GetHeadersIndexInResultTable(N1_UNIVERSE_POSITION + "-" + mediaList[0]);
            long levelLabelColIndex = resultTable.GetHeadersIndexInResultTable(LEVEL_ID.ToString());
            long mediaScheduleColIndex = resultTable.GetHeadersIndexInResultTable(MEDIA_SCHEDULE_ID.ToString());
            long EvolStartColIndex;
            if (mediaList.Length > 1)
                EvolStartColIndex = resultTable.GetHeadersIndexInResultTable(EVOL_UNIVERSE_POSITION + "-" + SUBTOTAL_ID);
            else
                EvolStartColIndex = resultTable.GetHeadersIndexInResultTable(EVOL_UNIVERSE_POSITION.ToString()) + 1;
            long startDataColIndex = levelLabelColIndex + 1;
            if (showMediaSchedule) startDataColIndex = mediaScheduleColIndex + 1;
            #endregion


            #region Sélection de l'unité
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            #endregion

            #region Ligne du Total
            currentLineInTabResult = resultTable.AddNewLine(LineType.total);
            //Libellé du total
            resultTable[currentLineInTabResult, levelLabelColIndex] = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), 0, currentLineInTabResult);
            CellLevel currentCellLevel0 = (CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
            if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevel0, _session);
            // Unité
            if (computePDM) resultTable[currentLineInTabResult, NStartColIndex] = new CellPDM(0.0, null);
            else resultTable[currentLineInTabResult, NStartColIndex] = cellUnitFactory.Get(0.0);
            for (k = NStartColIndex + 1; k < N1StartColIndex; k++) {
                if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, NStartColIndex]);
                else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
            }
            if (computePDM) resultTable[currentLineInTabResult, N1StartColIndex] = new CellPDM(0.0, null);
            else resultTable[currentLineInTabResult, N1StartColIndex] = cellUnitFactory.Get(0.0);
            for (k = N1StartColIndex + 1; k < EvolStartColIndex; k++) {
                if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, N1StartColIndex]);
                else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
            }
            #region Evol Total
            if (mediaList.Length > 1) resultTable[currentLineInTabResult, EvolStartColIndex] = new CellEvol(resultTable[currentLineInTabResult, NStartColIndex], resultTable[currentLineInTabResult, N1StartColIndex]);
            foreach (Int64 currentMedia in mediaList) {
                resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", EVOL_UNIVERSE_POSITION, currentMedia))] = new CellEvol(resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N_UNIVERSE_POSITION, currentMedia))], resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N1_UNIVERSE_POSITION, currentMedia))]);
            }
            #endregion

            #endregion

            #region Nombre parutions by media
            if (resNbParution != null && resNbParution.Count > 0) {
                currentLineInTabResult = resultTable.AddNewLine(TNS.FrameWork.WebResultUI.LineType.nbParution);
                //Libellé du Nombre parutions
                resultTable[currentLineInTabResult, levelLabelColIndex] = new CellLevel(-1, GestionWeb.GetWebWord(2460, _session.SiteLanguage), 0, currentLineInTabResult);
                CellLevel currentCellParution = (CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellParution, _session);
                resultTable[currentLineInTabResult, NStartColIndex] = new CellNumber(0.0);
                for (k = NStartColIndex + 1; k < N1StartColIndex; k++) {
                    resultTable[currentLineInTabResult, k] = new CellNumber(0.0);
                }
                resultTable[currentLineInTabResult, N1StartColIndex] = new CellNumber(0.0);
                for (k = N1StartColIndex + 1; k < EvolStartColIndex; k++) {
                    resultTable[currentLineInTabResult, k] = new CellNumber(0.0);
                }
                #region Evol Nb parutions
                if (mediaList.Length > 1) resultTable[currentLineInTabResult, EvolStartColIndex] = new CellEvol(resultTable[currentLineInTabResult, NStartColIndex], resultTable[currentLineInTabResult, N1StartColIndex]);
                foreach (Int64 currentMedia in mediaList) {
                    resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", EVOL_UNIVERSE_POSITION, currentMedia))] = new CellEvol(resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N_UNIVERSE_POSITION, currentMedia))], resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N1_UNIVERSE_POSITION, currentMedia))]);
                }
                #endregion

                //Insertion du nombre de parution pour period N et N-1			
                foreach (KeyValuePair<string, double> kpv in resNbParution) {
                    if (resultTable.HeadersIndexInResultTable.ContainsKey(kpv.Key)) {
                        TNS.FrameWork.WebResultUI.Header header = (TNS.FrameWork.WebResultUI.Header)resultTable.HeadersIndexInResultTable[kpv.Key];
                        resultTable[currentLineInTabResult, header.IndexInResultTable] = new CellNumber(resNbParution[kpv.Key]);
                    }
                }
            }
            #endregion

            #region Tableau de résultat
            oldIdL1 = -1;
            oldIdL2 = -1;
            oldIdL3 = -1;
            AdExpressCellLevel currentCellLevel1 = null;
            AdExpressCellLevel currentCellLevel2 = null;
            AdExpressCellLevel currentCellLevel3 = null;
            long currentL1Index = -1;
            long currentL2Index = -1;
            long currentL3Index = -1;
            long nbColInTabData = tabData.GetLength(0);
            currentLineInTabResult = FIRST_LINE_RESULT_INDEX - 1;
            for (currentLine = 0; currentLine < nbLineInTabData; currentLine++) {

                #region On change de niveau L1
                if (tabData[IDL1_INDEX, currentLine] != null && (Int64)tabData[IDL1_INDEX, currentLine] != oldIdL1) {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level1);


                    #region Totaux et sous Totaux à 0 et media
                    // Unité
                    // Unité
                    if (computePDM) resultTable[currentLineInTabResult, NStartColIndex] = new CellPDM(0.0, null);
                    else resultTable[currentLineInTabResult, NStartColIndex] = cellUnitFactory.Get(0.0);
                    for (k = NStartColIndex + 1; k < N1StartColIndex; k++) {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, NStartColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    if (computePDM) resultTable[currentLineInTabResult, N1StartColIndex] = new CellPDM(0.0, null);
                    else resultTable[currentLineInTabResult, N1StartColIndex] = cellUnitFactory.Get(0.0);
                    for (k = N1StartColIndex + 1; k < EvolStartColIndex; k++) {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, N1StartColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    #endregion


                    #region Evol L1
                    if (mediaList.Length > 1) resultTable[currentLineInTabResult, EvolStartColIndex] = new CellEvol(resultTable[currentLineInTabResult, NStartColIndex], resultTable[currentLineInTabResult, N1StartColIndex]);
                    foreach (Int64 currentMedia in mediaList) {
                        resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", EVOL_UNIVERSE_POSITION, currentMedia))] = new CellEvol(resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N_UNIVERSE_POSITION, currentMedia))], resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N1_UNIVERSE_POSITION, currentMedia))]);
                    }
                    #endregion

                    oldIdL1 = (Int64)tabData[IDL1_INDEX, currentLine];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel((Int64)tabData[IDL1_INDEX, currentLine], (string)tabData[LABELL1_INDEX, currentLine], currentCellLevel0, 1, currentLineInTabResult, _session);
                    currentCellLevel1 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevel1, _session);
                    currentL1Index = currentLineInTabResult;
                    oldIdL2 = oldIdL3 = -1;

                    #region GAD
                    if (showGad && _session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 1) {
                        if (tabData[ADDRESS_COLUMN_INDEX, currentLine] != null) {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = (Int64)tabData[ADDRESS_COLUMN_INDEX, currentLine];
                        }
                    }
                    #endregion
                }
                #endregion

                #region On change de niveau L2
                if (tabData[IDL2_INDEX, currentLine] != null && (Int64)tabData[IDL2_INDEX, currentLine] != oldIdL2) {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level2);

                    #region Totaux et sous Totaux à 0 et media
                    // Unité
                    if (computePDM) resultTable[currentLineInTabResult, NStartColIndex] = new CellPDM(0.0, null);
                    else resultTable[currentLineInTabResult, NStartColIndex] = cellUnitFactory.Get(0.0);
                    for (k = NStartColIndex + 1; k < N1StartColIndex; k++) {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, NStartColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    if (computePDM) resultTable[currentLineInTabResult, N1StartColIndex] = new CellPDM(0.0, null);
                    else resultTable[currentLineInTabResult, N1StartColIndex] = cellUnitFactory.Get(0.0);
                    for (k = N1StartColIndex + 1; k < EvolStartColIndex; k++) {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, N1StartColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    #endregion

                    #region Evol L2
                    if (mediaList.Length > 1) resultTable[currentLineInTabResult, EvolStartColIndex] = new CellEvol(resultTable[currentLineInTabResult, NStartColIndex], resultTable[currentLineInTabResult, N1StartColIndex]);
                    foreach (Int64 currentMedia in mediaList) {
                        resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", EVOL_UNIVERSE_POSITION, currentMedia))] = new CellEvol(resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N_UNIVERSE_POSITION, currentMedia))], resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N1_UNIVERSE_POSITION, currentMedia))]);
                    }
                    #endregion

                    oldIdL2 = (Int64)tabData[IDL2_INDEX, currentLine];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel((Int64)tabData[IDL2_INDEX, currentLine], (string)tabData[LABELL2_INDEX, currentLine], currentCellLevel1, 2, currentLineInTabResult, _session);
                    currentCellLevel2 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevel2, _session);
                    currentL2Index = currentLineInTabResult;
                    oldIdL3 = -1;

                    #region GAD
                    if (showGad && _session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 2) {
                        if (tabData[ADDRESS_COLUMN_INDEX, currentLine] != null) {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = (Int64)tabData[ADDRESS_COLUMN_INDEX, currentLine];
                        }
                    }
                    #endregion
                }
                #endregion

                #region On change de niveau L3
                if (tabData[IDL3_INDEX, currentLine] != null && (Int64)tabData[IDL3_INDEX, currentLine] != oldIdL3) {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level3);

                    #region Totaux et sous Totaux à 0 et media
                    // Unité
                    if (computePDM) resultTable[currentLineInTabResult, NStartColIndex] = new CellPDM(0.0, null);
                    else resultTable[currentLineInTabResult, NStartColIndex] = cellUnitFactory.Get(0.0);
                    for (k = NStartColIndex + 1; k < N1StartColIndex; k++) {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, NStartColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    if (computePDM) resultTable[currentLineInTabResult, N1StartColIndex] = new CellPDM(0.0, null);
                    else resultTable[currentLineInTabResult, N1StartColIndex] = cellUnitFactory.Get(0.0);
                    for (k = N1StartColIndex + 1; k < EvolStartColIndex; k++) {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, N1StartColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    #endregion

                    #region Evol L3
                    if (mediaList.Length > 1) resultTable[currentLineInTabResult, EvolStartColIndex] = new CellEvol(resultTable[currentLineInTabResult, NStartColIndex], resultTable[currentLineInTabResult, N1StartColIndex]);
                    foreach (Int64 currentMedia in mediaList) {
                        resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", EVOL_UNIVERSE_POSITION, currentMedia))] = new CellEvol(resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N_UNIVERSE_POSITION, currentMedia))], resultTable[currentLineInTabResult, resultTable.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N1_UNIVERSE_POSITION, currentMedia))]);
                    }
                    #endregion

                    oldIdL3 = (Int64)tabData[IDL3_INDEX, currentLine];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel((Int64)tabData[IDL3_INDEX, currentLine], (string)tabData[LABELL3_INDEX, currentLine], currentCellLevel2, 3, currentLineInTabResult, _session);
                    currentCellLevel3 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevel3, _session);
                    currentL3Index = currentLineInTabResult;

                    #region GAD
                    if (showGad && _session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 3) {
                        if (tabData[ADDRESS_COLUMN_INDEX, currentLine] != null) {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = (Int64)tabData[ADDRESS_COLUMN_INDEX, currentLine];
                        }
                    }
                    #endregion
                }
                #endregion

                // On copy la ligne et on l'ajoute aux totaux
                for (k = FIRST_MEDIA_INDEX; k < nbColInTabData; k++) {
                    resultTable.AffectValueAndAddToHierarchy(levelLabelColIndex, currentLineInTabResult, startDataColIndex + k - (long.Parse((FIRST_MEDIA_INDEX).ToString())), (double)tabData[k, currentLine]);

                }

            }
            #endregion

            return (resultTable);
        }
        #endregion

        #region Calcul du nombre de ligne d'un tableau préformaté
        /// <summary>
        /// Get the number of line in the final result table from the perfromatted table
        /// </summary>
        /// <param name="tabData">Preformated Table</param>
        /// <returns>Number of lines in final result table</returns>
        private long GetNbLineFromPreformatedTableToResultTable(object[,] tabData) {

            #region Variables
            long nbLine = 0;
            long k;
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            #endregion

            for (k = 0; k < tabData.GetLength(1); k++) {
                // Somme des L1
                if (tabData[IDL1_INDEX, k] != null && (Int64)tabData[IDL1_INDEX, k] != oldIdL1) {
                    oldIdL1 = (Int64)tabData[IDL1_INDEX, k];
                    nbLine++;
                    oldIdL3 = oldIdL2 = -1;
                }
                // Somme des L2
                if (tabData[IDL2_INDEX, k] != null && (Int64)tabData[IDL2_INDEX, k] != oldIdL2) {
                    oldIdL2 = (Int64)tabData[IDL2_INDEX, k];
                    nbLine++;
                    oldIdL3 = -1;
                }
                // Somme des L3
                if (tabData[IDL3_INDEX, k] != null && (Int64)tabData[IDL3_INDEX, k] != oldIdL3) {
                    oldIdL3 = (Int64)tabData[IDL3_INDEX, k];
                    nbLine++;
                }
            }
            return (nbLine);
        }
        #endregion

        #region Data Filtering

        #region WON / LOST
        /// <summary>
        /// Filter data to keep a result with only won data or lost data.
        /// </summary>
        /// <param name="getWon">Specify if the result must be in Won mode or in lost mode</param>
        /// <param name="tabData">Data Table</param>
        /// <param name="groupMediaTotalIndex">Indexes of media groups</param>
        /// <param name="nbLineInFormatedTable">Number of data in Preformated table</param>
        /// <param name="nbLineInTabResult">(out) Number of lines in final result</param>
        /// <param name="nbCol">Nb of column in Preformatted table</param>
        /// <returns>Table with either won data or lost data</returns>
        protected object[,] FilterWonLost(bool getWon, object[,] tabData, List<SelectionGroup> groupMediaTotalIndex, long nbLineInFormatedTable, ref long nbLineInTabResult, long nbCol) {
            bool subTotalNotNull = true;
            int currentLineInTabResult = -1;
            int positionUnivers = 0;
            object[,] tabResult = new object[nbCol, nbLineInFormatedTable];

            for (int currentLine = 0; currentLine < nbLineInFormatedTable; currentLine++) {
                positionUnivers = 1;
                subTotalNotNull = getWon;
                // On cherche les lignes qui on des unités à 0(null) dans le premier sous total
                if ((!getWon && (double)tabData[groupMediaTotalIndex[1].IndexInResultTable, currentLine] == 0.0)
                    || (getWon && (double)tabData[groupMediaTotalIndex[1].IndexInResultTable, currentLine] != 0.0)) {
                    positionUnivers++;
                    while (((getWon && subTotalNotNull) || (!getWon && !subTotalNotNull)) && positionUnivers < NB_UNIVERSES_TEST - 1) {
                        if ((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable, currentLine] != 0.0)
                            subTotalNotNull = (getWon) ? false : true;
                        positionUnivers++;
                    }
                    //au moins un sous total de concurrent différent à 0(null)
                    if (subTotalNotNull) {
                        currentLineInTabResult++;
                        for (int currentColumn = 0; currentColumn < nbCol; currentColumn++) {
                            tabResult[currentColumn, currentLineInTabResult] = tabData[currentColumn, currentLine];
                        }
                    }

                }
            }
            nbLineInTabResult = currentLineInTabResult + 1;

            return tabResult;
        }
        #endregion

        #endregion

        #region LOYAL (SIMPLE, RISING, SLIDING)
        /// <summary>
        /// Filter data to keep a result with only won data or lost data.
        /// </summary>
        /// <param name="getEvol">Specify if the result must filter loyal data with an evolution or not</param>
        /// <param name="getRising">without any effect if getEvol==false, else if getRising=true keep rising datafilter, else keep sliding data</param>
        /// <param name="tabData">Data Table</param>
        /// <param name="groupMediaTotalIndex">Indexes of media groups</param>
        /// <param name="nbLineInFormatedTable">Number of data in Preformated table</param>
        /// <param name="nbLineInTabResult">(out) Number of lines in final result</param>
        /// <param name="nbCol">Nb of column in Preformatted table</param>
        /// <returns>Table with loyal, loyal rising or loyal sliding data</returns>
        protected object[,] FilterLoyal(bool getEvol, bool getRising, object[,] tabData, List<SelectionGroup> groupMediaTotalIndex, long nbLineInFormatedTable, ref long nbLineInTabResult, long nbCol) {
            int currentLineInTabResult = -1;
            int positionUnivers = 0;
            object[,] tabResult = new object[nbCol, nbLineInFormatedTable];
            bool allSubTotalNotNull = true;

            for (int currentLine = 0; currentLine < nbLineInFormatedTable; currentLine++) {
                allSubTotalNotNull = true;
                positionUnivers = 1;
                while (allSubTotalNotNull && positionUnivers < NB_UNIVERSES_TEST - 1) {
                    if ((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable, currentLine] == 0.0)
                        allSubTotalNotNull = false;
                    positionUnivers++;
                }
                if (allSubTotalNotNull && (
                    !getEvol
                    || (getEvol && getRising && (double)tabData[groupMediaTotalIndex[N_UNIVERSE_POSITION].IndexInResultTable, currentLine] > (double)tabData[groupMediaTotalIndex[N1_UNIVERSE_POSITION].IndexInResultTable, currentLine])
                    || (getEvol && !getRising && (double)tabData[groupMediaTotalIndex[N_UNIVERSE_POSITION].IndexInResultTable, currentLine] < (double)tabData[groupMediaTotalIndex[N1_UNIVERSE_POSITION].IndexInResultTable, currentLine])
                    )) {
                    currentLineInTabResult++;
                    for (int currentColumn = 0; currentColumn < nbCol; currentColumn++) {
                        tabResult[currentColumn, currentLineInTabResult] = tabData[currentColumn, currentLine];
                    }
                }
            }

            nbLineInTabResult = currentLineInTabResult + 1;

            return tabResult;
        }
        #endregion

        #region Obtient l'activité publicitaire d'un produit
        /// <summary>
        /// Get Advertising activity of a product
        /// </summary>
        /// <param name="tabResult">Result Table</param>
        /// <param name="dt">Data Table</param>
        /// <param name="indexLineProduct">Index of product line</param>
        /// <param name="expression">Calcul expression</param>
        /// <param name="filterN">Year N Filter</param>
        /// <param name="filterN1">Year N-1 filter</param>
        protected void GetProductActivity(ResultTable tabResult, DataTable dt, long indexLineProduct, string expression, string filterN, string filterN1) {
            object unitValueN = System.DBNull.Value;
            object unitValueN1 = System.DBNull.Value;
            Int64 loyalNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 loyalDeclineNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_DECLINE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 loyalRiseNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOYAL_RISE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 wonNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(WON_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 lostNumberColonneIndex = tabResult.GetHeadersIndexInResultTable(LOST_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            unitValueN = dt.Compute(expression, filterN);
            unitValueN1 = dt.Compute(expression, filterN1);

            #region Fidèles

            if (unitValueN != System.DBNull.Value && !unitValueN.ToString().Equals("") && unitValueN1 != System.DBNull.Value && !unitValueN1.ToString().Equals("")) {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalNumberColonneIndex]).Value += 1;
                //Unité N
                ((CellUnit)tabResult[indexLineProduct, loyalNumberColonneIndex + 1]).Value += double.Parse(unitValueN.ToString());
                //Unité N-1
                ((CellUnit)tabResult[indexLineProduct, loyalNumberColonneIndex + 2]).Value += double.Parse(unitValueN1.ToString());

            }
            #endregion

            #region Fidèle en baisse

            if (unitValueN != System.DBNull.Value && !unitValueN.ToString().Equals("") && unitValueN1 != System.DBNull.Value && !unitValueN1.ToString().Equals("") && double.Parse(unitValueN.ToString()) < double.Parse(unitValueN1.ToString())) {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex]).Value += 1;
                //Unité N
                ((CellUnit)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex + 1]).Value += double.Parse(unitValueN.ToString());
                //Unité N-1
                ((CellUnit)tabResult[indexLineProduct, loyalDeclineNumberColonneIndex + 2]).Value += double.Parse(unitValueN1.ToString());

            }
            #endregion

            #region Fidèle en  développement
            if (unitValueN != System.DBNull.Value && !unitValueN.ToString().Equals("") && unitValueN1 != System.DBNull.Value && !unitValueN1.ToString().Equals("") && double.Parse(unitValueN.ToString()) > double.Parse(unitValueN1.ToString())) {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, loyalRiseNumberColonneIndex]).Value += 1;
                //Unité N
                ((CellUnit)tabResult[indexLineProduct, loyalRiseNumberColonneIndex + 1]).Value += double.Parse(unitValueN.ToString());
                //Unité N-1
                ((CellUnit)tabResult[indexLineProduct, loyalRiseNumberColonneIndex + 2]).Value += double.Parse(unitValueN1.ToString());

            }
            #endregion

            #region Gagnés
            if (unitValueN != System.DBNull.Value && !unitValueN.ToString().Equals("") && (unitValueN1 == System.DBNull.Value || unitValueN1.ToString().Equals(""))) {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, wonNumberColonneIndex]).Value += 1;
                //Unité N
                ((CellUnit)tabResult[indexLineProduct, wonNumberColonneIndex + 1]).Value += double.Parse(unitValueN.ToString());
            }
            #endregion

            #region Perdus
            if (unitValueN1 != System.DBNull.Value && !unitValueN1.ToString().Equals("") && (unitValueN == System.DBNull.Value || unitValueN.ToString().Equals(""))) {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, lostNumberColonneIndex]).Value += 1;
                //Unité N-1
                ((CellUnit)tabResult[indexLineProduct, lostNumberColonneIndex + 2]).Value += double.Parse(unitValueN1.ToString());
            }
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


            #region Chargement des données à partir de la base
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


            if (dt != null && dt.Rows.Count > 0) {
                foreach (DataRow dr in dt.Rows) {
                    if (!oldKey.Equals(dr["yearParution"].ToString() + "-" + dr["id_media"].ToString()) && !start) {
                        res.Add(oldKey, nbParutionsCounter);
                        nbParutionsCounter = 0;
                    }
                    nbParutionsCounter += double.Parse(dr["NbParution"].ToString());
                    start = false;
                    oldKey = dr["yearParution"].ToString() + "-" + dr["id_media"].ToString();
                }
                res.Add(oldKey, nbParutionsCounter);
            }

            return res;

        }

        #endregion

        #endregion
    }
}
