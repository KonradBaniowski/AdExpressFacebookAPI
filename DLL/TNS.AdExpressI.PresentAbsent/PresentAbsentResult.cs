#region Information
/*
 * Author : G Ragneau
 * Creation : 18/03/2008
 * Updates :
 *      Date        Author      Description
 *      11/08/2008  G. Facon    Use Vehicle XML to show creatives & insertions or not
 */
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using DALClassif = TNS.AdExpress.DataAccess.Classification;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Core.Utilities;
using Navigation = TNS.AdExpress.Domain.Web.Navigation;

using TNS.AdExpress;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpressI.PresentAbsent.DAL;
using TNS.AdExpressI.PresentAbsent.Exceptions;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Collections;
using TNS.AdExpress.Domain.Web;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Units;

#endregion

namespace TNS.AdExpressI.PresentAbsent
{
    /// <summary>
    /// Default Present/Absent reports
    /// </summary>
    public abstract class PresentAbsentResult : IPresentAbsentResult
    {

        #region Constantes
        /// <summary>
        /// First Media Index
        /// </summary>
        protected const int FIRST_MEDIA_INDEX = 8;
        /// <summary>
        /// Index of level N Id
        /// </summary>
        protected const int IDL1_INDEX = 0;
        /// <summary>
        /// Index of level N label
        /// </summary>
        protected const int LABELL1_INDEX = 1;
        /// <summary>
        /// Index of level N-1 Id
        /// </summary>
        protected const int IDL2_INDEX = 2;
        /// <summary>
        /// Index of level N-1 Label
        /// </summary>
        protected const int LABELL2_INDEX = 3;
        /// <summary>
        /// Index of level N-2 Id
        /// </summary>
        protected const int IDL3_INDEX = 4;
        /// <summary>
        /// Index of level N-3 Label
        /// </summary>
        protected const int LABELL3_INDEX = 5;
        /// <summary>
        /// Index of Adresse Id Colunm
        /// </summary>
        protected const int ADDRESS_COLUMN_INDEX = 6;
        /// <summary>
        /// Index of total
        /// </summary>
        protected const int TOTAL_INDEX = 7;
        /// <summary>
        /// Level Column Id (product)
        /// </summary>
        protected const int LEVEL_HEADER_ID = 0;
        /// <summary>
        /// Creatives Column ID
        /// </summary>
        protected const int CREATIVE_HEADER_ID = 1;
        /// <summary>
        /// Inserts Column ID
        /// </summary>
        protected const int INSERTION_HEADER_ID = 2;
        /// <summary>
        /// Media Schedule Column ID
        /// </summary>
        protected const int MEDIA_SCHEDULE_HEADER_ID = 3;
        /// <summary>
        /// Univers Total Column ID
        /// </summary>
        protected const int TOTAL_HEADER_ID = -4;
        /// <summary>
        /// Univers SUb totals ID
        /// </summary>
        public const int SUB_TOTAL_HEADER_ID = -5;
        /// <summary>
        /// Group ID Beginning
        /// </summary>
        public const int START_ID_GROUP = 13;
        /// <summary>
        /// Present Column ID
        /// </summary>
        public const int PRESENT_HEADER_ID = 6;
        /// <summary>
        /// Missing Column ID
        /// </summary>
        public const int ABSENT_HEADER_ID = 7;
        /// <summary>
        /// Exclusives Column ID
        /// </summary>
        public const int EXCLUSIVE_HEADER_ID = 8;
        /// <summary>
        /// Number Column ID
        /// </summary>
        public const int ITEM_NUMBER_HEADER_ID = 9;
        /// <summary>
        /// Unit Column ID
        /// </summary>
        public const int UNIT_HEADER_ID = 10;
        /// <summary>
        /// Reference Media Column ID
        /// </summary>
        public const int REFERENCE_MEDIA_HEADER_ID = 11;
        /// <summary>
        /// Competitor Column ID
        /// </summary>
        public const int COMPETITOR_MEDIA_HEADER_ID = 12;
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
        /// Show creative column
        /// </summary>
        protected bool _showCreative = false;
        /// <summary>
        /// Show insertion column
        /// </summary>
        protected bool _showInsertions = false;
        /// <summary>
        /// Show Media Schedule Column
        /// </summary>
        protected bool _showMediaSchedule = false;
        /// <summary>
        /// Show Total Column
        /// </summary>
        protected bool _showTotal = false;
        /// <summary>
        /// Is acces to pickanews?
        /// </summary>
        protected bool _allowPickaNews = false;
        #endregion

        #region Accessors
        /// <summary>
        /// Get User session
        /// </summary>
        public WebSession Session
        {
            get { return _session; }
        }
        /// <summary>
        /// Get Result Type
        /// </summary>
        public int ResultType
        {
            get { return _result; }
        }
        /// <summary>
        /// Get Current Vehicle
        /// </summary>
        public VehicleInformation VehicleInformationObject
        {
            get { return _vehicleInformation; }
        }
        /// <summary>
        /// Get Current Module
        /// </summary>
        public Navigation.Module CurrentModule
        {
            get { return _module; }
        }
        /// <summary>
        /// Get / Set autorisation to access to pickanews
        /// </summary>
        public bool AllowPickaNews
        {
            get { return _allowPickaNews; }
            set { _allowPickaNews = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public PresentAbsentResult(WebSession session)
        {
            _session = session;
            _module = Navigation.ModulesList.GetModule(session.CurrentModule);

            #region Sélection du vehicle
            string vehicleSelection = session.GetSelection(session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new PresentAbsentException("Uncorrect Media Selection"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));

            #endregion

        }
        #endregion

        #region IResults Membres
        /// <summary>
        /// Compute result "Summary"
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetSummary()
        {
            return GetResult(CompetitorMarketShare.SYNTHESIS);
        }

        /// <summary>
        /// Compute result "Portofolio"
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetPortefolio()
        {
            return GetResult(CompetitorMarketShare.PORTEFEUILLE);
        }

        /// <summary>
        /// Compute result for the study "Present in more than one vehicle"
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetCommons()
        {
            return GetResult(CompetitorMarketShare.COMMON);
        }

        /// <summary>
        /// Compute result "Missings"
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetMissings()
        {
            return GetResult(CompetitorMarketShare.ABSENT);
        }

        /// <summary>
        /// Compute result "Exclusives"
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetExclusives()
        {
            return GetResult(CompetitorMarketShare.EXCLUSIF);
        }

        /// <summary>
        /// Compute result "Strengths"
        /// </summary>
        /// <returns>Computed data</returns>
        public virtual ResultTable GetStrengths()
        {
            this._result = CompetitorMarketShare.FORCES;
            return GetData();
        }

        /// <summary>
        /// Compute result "Prospects"
        /// </summary>
        /// <returns>Computed data</returns>
        public virtual ResultTable GetProspects()
        {
            return GetResult(CompetitorMarketShare.POTENTIELS);
        }

        /// <summary>
        /// Compute specified result
        /// </summary>
        /// <param name="result">Type of result to compute</param>
        /// <returns>Computed data</returns>
        public virtual ResultTable GetResult(int result)
        {
            switch (result)
            {
                case CompetitorMarketShare.ABSENT:
                case CompetitorMarketShare.COMMON:
                case CompetitorMarketShare.EXCLUSIF:
                case CompetitorMarketShare.FORCES:
                case CompetitorMarketShare.PORTEFEUILLE:
                case CompetitorMarketShare.POTENTIELS:
                    this._result = result;
                    return GetData();
                case CompetitorMarketShare.SYNTHESIS:
                    this._result = CompetitorMarketShare.SYNTHESIS;
                    return GetSynthesisData();
                default: return null;
            }
        }

        /// <summary>
        /// Compute result specified in user session
        /// </summary>
        /// <returns>Computed data</returns>
        public virtual ResultTable GetResult()
        {
            return GetResult((int)_session.CurrentTab);
        }
        #endregion

        #region Result Computing Methods

        #region Get Data
        protected virtual ResultTable GetData()
        {
            Dictionary<Int64, HeaderBase> universesSubTotal = null;
            Dictionary<string, HeaderBase> elementsHeader = null;
            Dictionary<string, HeaderBase> elementsSubTotal = null;
            ResultTable tabData = this.GetGrossTable(out universesSubTotal, out elementsHeader, out elementsSubTotal);
            ResultTable tabResult = null;
            Int32 nbLine = 0;

            if (tabData == null)
            {
                return null;
            }

            #region Traitement des données
            switch (_session.CurrentTab)
            {

                #region Portefolio, strength, potentials
                case CompetitorMarketShare.FORCES:
                case CompetitorMarketShare.POTENTIELS:
                case CompetitorMarketShare.PORTEFEUILLE:
                    nbLine = tabData.LinesNumber;
                    break;
                #endregion

                #region Absent / Exclusif / Common
                default:
                    int nbLevel = _session.GenericProductDetailLevel.GetNbLevels;
                    Int64 iUnivers = 0;
                    bool competitor = false;

                    CellLevel[] levels = new CellLevel[nbLevel];
                    bool[] display = new bool[nbLevel + 1];
                    for (int i = 0; i <= nbLevel; i++) { display[i] = false; }
                    CellLevel cLevel = null;
                    for (int i = 0; i < tabData.LinesNumber; i++)
                    {
                        cLevel = (CellLevel)tabData[i, 1];

                        if (cLevel.Level < nbLevel)
                        {
                            if (levels[cLevel.Level] != null)
                            {
                                if (!display[cLevel.Level])
                                {
                                    tabData.SetLineStart(new LineHide(tabData.GetLineStart(levels[cLevel.Level].LineIndexInResultTable).LineType), levels[cLevel.Level].LineIndexInResultTable);
                                }
                                else
                                {
                                    nbLine++;
                                }
                            }
                            levels[cLevel.Level] = cLevel;
                            display[cLevel.Level] = false;
                            continue;
                        }
                        iUnivers = 0;

                        #region Result specific treatment
                        display[cLevel.Level] = false;
                        switch (_session.CurrentTab)
                        {
                            case CompetitorMarketShare.ABSENT:
                                // We look for lines with first subTotal
                                if (((ICell)tabData[i, universesSubTotal[iUnivers].IndexInResultTable]).CompareTo(0.0) == 0)
                                {
                                    iUnivers++;
                                    while (!display[cLevel.Level] && universesSubTotal.ContainsKey(iUnivers))
                                    {
                                        if (((ICell)tabData[i, universesSubTotal[iUnivers].IndexInResultTable]).CompareTo(0.0) != 0) display[cLevel.Level] = true;
                                        iUnivers++;
                                    }
                                }
                                break;
                            case CompetitorMarketShare.EXCLUSIF:
                                competitor = false;
                                // We look for subtotals with value in first subtotal and 0 in others
                                if (((ICell)tabData[i, universesSubTotal[iUnivers].IndexInResultTable]).CompareTo(0.0) != 0)
                                {
                                    iUnivers++;
                                    while (!competitor && universesSubTotal.ContainsKey(iUnivers))
                                    {
                                        if (((ICell)tabData[i, universesSubTotal[iUnivers].IndexInResultTable]).CompareTo(0.0) != 0) competitor = true;
                                        iUnivers++;
                                    }
                                    if (!competitor)
                                    {
                                        display[cLevel.Level] = true;
                                    }
                                }
                                break;
                            case CompetitorMarketShare.COMMON:
                                competitor = true;
                                // We look for lines with all subtotals > 0
                                while (competitor && universesSubTotal.ContainsKey(iUnivers))
                                {
                                    if (((ICell)tabData[i, universesSubTotal[iUnivers].IndexInResultTable]).CompareTo(0.0) == 0) competitor = false;
                                    iUnivers++;
                                }
                                if (competitor)
                                {
                                    display[cLevel.Level] = true;
                                }
                                break;
                        }
                        #endregion

                        if (!display[cLevel.Level])
                        {
                            tabData.SetLineStart(new LineHide(tabData.GetLineStart(i).LineType), i);
                        }
                        else
                        {
                            for (int j = nbLevel - 1; j >= 0; j--) { display[j] = true; }
                            nbLine++;
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
                            else
                            {
                                nbLine++;
                            }
                        }
                    }
                    break;
                    #endregion

            }
            #endregion

            #region Build Final Table (only required lines + total and parution numbers)
            if (nbLine > 0)
            {
                tabResult = GetResultTable(tabData, nbLine, universesSubTotal, elementsHeader, elementsSubTotal);
            }
            else
            {
                return null;
            }
            #endregion

            #region Strength and prospects filters
            if (_result == CompetitorMarketShare.FORCES)
                FilterTable(tabResult, true, universesSubTotal);
            if (_result == CompetitorMarketShare.POTENTIELS)
                FilterTable(tabResult, false, universesSubTotal);
            #endregion

            return tabResult;
        }
        #endregion

        #region Synthesis
        /// <summary>
        /// Get table with synthesis about numbers of Commons, Exclusives and Missings products
        /// </summary>
        /// <returns>Result Table</returns>
		public virtual ResultTable GetSynthesisData()
        {

            #region Variables
            int positionUnivers = 0; //1 oLD
            Int32 nbLine = 8;
            Int32 advertiserLineIndex = 0;
            Int32 brandLineIndex = 0;
            Int32 productLineIndex = 0;
            Int32 sectorLineIndex = 0;
            Int32 subsectorLineIndex = 0;
            Int32 groupLineIndex = 0;
            Int32 agencyGroupLineIndex = 0;
            Int32 agencyLineIndex = 0;
            List<string> advertisers = null;
            List<string> products = null;
            List<string> brands = null;
            List<string> sectors = null;
            List<string> subsectors = null;
            List<string> groups = null;
            List<string> agencyGroups = null;
            List<string> agency = null;
            DataTable dt = null;
            List<string> referenceUniversMedia = null;
            List<string> competitorUniversMedia = null;
            string mediaList = "";
            string expression = "";
            string sort = "id_media asc";
            string unitFormat = "{0:max0}";
            int asposeFormat = 37;
            bool showProduct = _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            #endregion

            #region Init delegates
            AddValue addValueDelegate;
            InitValue initValueDelegate;
            SetSynthesisTable setSynthesisTableDelegate;

            switch (_session.Unit)
            {
                case CstWeb.CustomerSessions.Unit.versionNb:
                    addValueDelegate = new AddValue(AddListValue);
                    initValueDelegate = new InitValue(InitListValue);
                    setSynthesisTableDelegate = new SetSynthesisTable(SetListSynthesisTable);
                    break;
                default:
                    addValueDelegate = new AddValue(AddDoubleValue);
                    initValueDelegate = new InitValue(InitDoubleValue);
                    setSynthesisTableDelegate = new SetSynthesisTable(SetDoubleSynthesisTable);
                    break;
            }
            #endregion

            #region Chargement des données
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
            object[] parameters = new object[1];
            parameters[0] = _session;
            IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            dt = presentAbsentDAL.GetSynthesisData().Tables[0];
            //dt = CompetitorDataAccess.GetGenericSynthesisData(webSession, vehicleName);
            #endregion

            #region Identifiant du texte des unités
            Int64 unitId = _session.GetUnitLabelId();
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            #endregion

            #region Création des headers
            nbLine = 4;
            if (showProduct) nbLine++;
            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) nbLine++;
            //if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MEDIA_AGENCY)) {
            if (_session.CustomerLogin.CustomerMediaAgencyFlagAccess(_vehicleInformation.DatabaseId))
            {
                if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency)) nbLine++;
                if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency)) nbLine++;
            }

            // Ajout de la colonne Produit
            Headers headers = new Headers();
            headers.Root.Add(new Header(GestionWeb.GetWebWord(1164, _session.SiteLanguage), LEVEL_HEADER_ID));

            #region Communs
            HeaderGroup present = new HeaderGroup(GestionWeb.GetWebWord(1127, _session.SiteLanguage), PRESENT_HEADER_ID);
            present.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitPresent = new Header(GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitPresent.Add(new Header(true, GestionWeb.GetWebWord(1365, _session.SiteLanguage), REFERENCE_MEDIA_HEADER_ID));
            unitPresent.Add(new Header(true, GestionWeb.GetWebWord(1366, _session.SiteLanguage), COMPETITOR_MEDIA_HEADER_ID));
            present.Add(unitPresent);
            headers.Root.Add(present);
            #endregion

            #region Absents
            HeaderGroup absent = new HeaderGroup(GestionWeb.GetWebWord(1126, _session.SiteLanguage), ABSENT_HEADER_ID);
            absent.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitAbsent = new Header(GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitAbsent.Add(new Header(true, GestionWeb.GetWebWord(1365, _session.SiteLanguage), REFERENCE_MEDIA_HEADER_ID));
            unitAbsent.Add(new Header(true, GestionWeb.GetWebWord(1366, _session.SiteLanguage), COMPETITOR_MEDIA_HEADER_ID));
            absent.Add(unitAbsent);
            headers.Root.Add(absent);
            #endregion

            #region Exclusifs
            HeaderGroup exclusive = new HeaderGroup(GestionWeb.GetWebWord(1128, _session.SiteLanguage), EXCLUSIVE_HEADER_ID);
            exclusive.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID));
            Header unitExclusive = new Header(GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID);
            unitExclusive.Add(new Header(true, GestionWeb.GetWebWord(1365, _session.SiteLanguage), REFERENCE_MEDIA_HEADER_ID));
            unitExclusive.Add(new Header(true, GestionWeb.GetWebWord(1366, _session.SiteLanguage), COMPETITOR_MEDIA_HEADER_ID));
            exclusive.Add(unitExclusive);
            headers.Root.Add(exclusive);
            #endregion

            #endregion

            #region Création du tableau
            ResultTable resultTable = new ResultTable(nbLine, headers);
            Int32 nbCol = resultTable.ColumnsNumber - 2;
            #endregion

            #region Initialisation des lignes
            Int32 levelLabelColIndex = resultTable.GetHeadersIndexInResultTable(LEVEL_HEADER_ID.ToString());
            advertiserLineIndex = resultTable.AddNewLine(LineType.level1);
            resultTable[advertiserLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1146, _session.SiteLanguage));
            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
            {
                brandLineIndex = resultTable.AddNewLine(LineType.level1);
                resultTable[brandLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1149, _session.SiteLanguage));
            }
            if (showProduct)
            {
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
            //if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MEDIA_AGENCY))
            if (_session.CustomerLogin.CustomerMediaAgencyFlagAccess(_vehicleInformation.DatabaseId))
            {
                if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                {
                    agencyGroupLineIndex = resultTable.AddNewLine(LineType.level1);
                    resultTable[agencyGroupLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1850, _session.SiteLanguage));
                }
                if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                {
                    agencyLineIndex = resultTable.AddNewLine(LineType.level1);
                    resultTable[agencyLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1851, _session.SiteLanguage));
                }
            }

            Int32 presentNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 absentNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 exclusiveNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            #region Initialisation des Nombres
            for (int i = 0; i < nbLine; i++)
            {
                CellNumber cN = new CellNumber();
                cN.StringFormat = unitFormat;
                cN.AsposeFormat = asposeFormat;
                resultTable[i, presentNumberColumnIndex] = cN;
                CellNumber cN1 = new CellNumber();
                cN1.StringFormat = unitFormat;
                cN1.AsposeFormat = asposeFormat;
                resultTable[i, absentNumberColumnIndex] = cN1;
                CellNumber cN2 = new CellNumber();
                cN2.StringFormat = unitFormat;
                cN2.AsposeFormat = asposeFormat;
                resultTable[i, exclusiveNumberColumnIndex] = cN2;
            }
            for (Int32 i = 0; i < nbLine; i++)
            {
                for (Int32 j = presentNumberColumnIndex + 1; j < absentNumberColumnIndex; j++)
                {
                    resultTable[i, j] = cellUnitFactory.Get(null);
                }
                for (Int32 j = absentNumberColumnIndex + 1; j < exclusiveNumberColumnIndex; j++)
                {
                    resultTable[i, j] = cellUnitFactory.Get(null);
                }
                for (Int32 j = exclusiveNumberColumnIndex + 1; j <= nbCol; j++)
                {
                    resultTable[i, j] = cellUnitFactory.Get(null);
                }
            }
            #endregion

            #endregion


            if (dt != null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0)
            {

                #region Sélection de Médias
                //Liste des supports de référence
                if (_session.PrincipalMediaUniverses != null && _session.PrincipalMediaUniverses.Count > 0)
                {
                    List<long> mediaIds = _session.PrincipalMediaUniverses[positionUnivers].GetLevelValue(TNSClassificationLevels.MEDIA, AccessType.includes);
                    if (mediaIds != null && mediaIds.Count > 0)
                    {
                        referenceUniversMedia = mediaIds.ConvertAll(Convert.ToString);
                        positionUnivers++;
                    }
                }
                //Liste des supports concurrents
                if (referenceUniversMedia != null && referenceUniversMedia.Count > 0)
                {
                    competitorUniversMedia = GetCompetitormedias(positionUnivers);
                }
                else return null;

                #endregion

                if (referenceUniversMedia != null && referenceUniversMedia.Count > 0 && competitorUniversMedia != null && competitorUniversMedia.Count > 0)
                {

                    advertisers = new List<string>();
                    products = new List<string>();
                    brands = new List<string>();
                    sectors = new List<string>();
                    subsectors = new List<string>();
                    groups = new List<string>();
                    agencyGroups = new List<string>();
                    agency = new List<string>();


                    #region Traitement des données
                    //Activités publicitaire Annonceurs,marques,produits
                    foreach (DataRow currentRow in dt.Rows)
                    {

                        //Activité publicitaire Annonceurs
                        if (!advertisers.Contains(currentRow["id_advertiser"].ToString()))
                        {
                            expression = string.Format("id_advertiser={0}", currentRow["id_advertiser"]);
                            GetProductActivity(resultTable, dt, advertiserLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia, addValueDelegate, setSynthesisTableDelegate, initValueDelegate);
                            advertisers.Add(currentRow["id_advertiser"].ToString());
                        }
                        if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
                        {
                            //Activité publicitaire marques
                            if (currentRow["id_brand"] != null && currentRow["id_brand"] != System.DBNull.Value && !brands.Contains(currentRow["id_brand"].ToString()))
                            {
                                expression = string.Format("id_brand={0}", currentRow["id_brand"]);
                                GetProductActivity(resultTable, dt, brandLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia, addValueDelegate, setSynthesisTableDelegate, initValueDelegate);
                                brands.Add(currentRow["id_brand"].ToString());
                            }
                        }

                        //Activité publicitaire produits
                        if (showProduct && currentRow["id_product"] != null && currentRow["id_product"] != System.DBNull.Value && !products.Contains(currentRow["id_product"].ToString()))
                        {
                            expression = string.Format("id_product={0}", currentRow["id_product"]);
                            GetProductActivity(resultTable, dt, productLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia, addValueDelegate, setSynthesisTableDelegate, initValueDelegate);
                            products.Add(currentRow["id_product"].ToString());
                        }

                        //Activité publicitaire Famille
                        if (currentRow["id_sector"] != null && currentRow["id_sector"] != System.DBNull.Value && !sectors.Contains(currentRow["id_sector"].ToString()))
                        {
                            expression = string.Format("id_sector={0}", currentRow["id_sector"]);
                            GetProductActivity(resultTable, dt, sectorLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia, addValueDelegate, setSynthesisTableDelegate, initValueDelegate);
                            sectors.Add(currentRow["id_sector"].ToString());
                        }
                        //Activité publicitaire Classe
                        if (currentRow["id_subsector"] != null && currentRow["id_subsector"] != System.DBNull.Value && !subsectors.Contains(currentRow["id_subsector"].ToString()))
                        {
                            expression = string.Format("id_subsector={0}", currentRow["id_subsector"]);
                            GetProductActivity(resultTable, dt, subsectorLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia, addValueDelegate, setSynthesisTableDelegate, initValueDelegate);
                            subsectors.Add(currentRow["id_subsector"].ToString());
                        }
                        //Activité publicitaire Groupes
                        if (currentRow["id_group_"] != null && currentRow["id_group_"] != System.DBNull.Value && !groups.Contains(currentRow["id_group_"].ToString()))
                        {
                            expression = string.Format("id_group_={0}", currentRow["id_group_"]);
                            GetProductActivity(resultTable, dt, groupLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia, addValueDelegate, setSynthesisTableDelegate, initValueDelegate);
                            groups.Add(currentRow["id_group_"].ToString());
                        }


                        //if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MEDIA_AGENCY)){
                        if (_session.CustomerLogin.CustomerMediaAgencyFlagAccess(_vehicleInformation.DatabaseId))
                        {
                            //activité publicitaire Groupes d'agences
                            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency) && currentRow["ID_GROUP_ADVERTISING_AGENCY"] != null && currentRow["ID_GROUP_ADVERTISING_AGENCY"] != System.DBNull.Value && !agencyGroups.Contains(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString()))
                            {
                                expression = string.Format("ID_GROUP_ADVERTISING_AGENCY={0}", currentRow["ID_GROUP_ADVERTISING_AGENCY"]);
                                GetProductActivity(resultTable, dt, agencyGroupLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia, addValueDelegate, setSynthesisTableDelegate, initValueDelegate);
                                agencyGroups.Add(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString());
                            }

                            //activité publicitaire agence
                            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency) && currentRow["ID_ADVERTISING_AGENCY"] != null && currentRow["ID_ADVERTISING_AGENCY"] != System.DBNull.Value && !agency.Contains(currentRow["ID_ADVERTISING_AGENCY"].ToString()))
                            {
                                expression = string.Format("ID_ADVERTISING_AGENCY={0}", currentRow["ID_ADVERTISING_AGENCY"]);
                                GetProductActivity(resultTable, dt, agencyLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia, addValueDelegate, setSynthesisTableDelegate, initValueDelegate);
                                agency.Add(currentRow["ID_ADVERTISING_AGENCY"].ToString());
                            }
                        }
                    }

                    #endregion

                }
                else return null;
            }
            else return null;


            return resultTable;
        }
        #endregion

        #endregion

        #region Data Filtering
        protected virtual void FilterTable(ResultTable data, bool computeStrenghs, Dictionary<Int64, HeaderBase> universesSubTotal)
        {

            if (data == null)
            {
                return;
            }

            #region Variables
            Int32 i;
            #endregion

            #region Indexes de comparaison
            Int32 comparaisonIndexInTabResult = universesSubTotal[0].IndexInResultTable;//1
            Int32 levelLabelColIndex = data.GetHeadersIndexInResultTable(LEVEL_HEADER_ID.ToString());
            #endregion

            #region Détermine le niveau de détail Produit
            int NbLevels = _session.GenericProductDetailLevel.GetNbLevels;
            #endregion

            #region Traitement des données
            try
            {
                // Supprime les lignes qui ont un total support de références média inférieur
                // au total univers des supports de référence.
                long[] nbLevelToShow = { 0, 0, 0, 0 };
                CellLevel curLevel = null;
                for (i = data.LinesNumber - 1; i >= 0; i--)
                {
                    curLevel = (CellLevel)data[i, levelLabelColIndex];
                    if (curLevel.Level == NbLevels)
                    {
                        if (data[i, comparaisonIndexInTabResult] != null && ((CellUnit)data[i, comparaisonIndexInTabResult]).Value != double.NaN &&
                            ((computeStrenghs && ((CellUnit)data[i, comparaisonIndexInTabResult]).Value < ((CellUnit)data[0, comparaisonIndexInTabResult]).Value) ||
                            (!computeStrenghs && ((CellUnit)data[i, comparaisonIndexInTabResult]).Value > ((CellUnit)data[0, comparaisonIndexInTabResult]).Value)))
                        {
                            data.SetLineStart(new LineHide(data.GetLineStart(i).LineType), i);
                        }
                        else
                        {
                            nbLevelToShow[NbLevels]++;
                        }
                    }
                    else
                    {
                        if (nbLevelToShow[curLevel.Level + 1] > 0)
                        {
                            nbLevelToShow[curLevel.Level]++;
                        }
                        else
                        {
                            data.SetLineStart(new LineHide(data.GetLineStart(i).LineType), i);
                        }
                        if (!(data[i, 0] is LineStart && ((LineStart)data[i, 0]).LineType == LineType.nbParution))
                        {
                            for (int j = curLevel.Level + 1; j < nbLevelToShow.Length; j++)
                            {
                                nbLevelToShow[j] = 0;
                            }
                        }
                    }
                }
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentException("Unable to build filter table (stringth and prospects): ", err));
            }
            #endregion

        }
        #endregion

        #region Internal methods

        #region Tableau Préformaté
        /// <summary>
        /// Compute data
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="groupMediaTotalIndex">List of indexes of groups selection</param>
        /// <param name="subGroupMediaTotalIndex">List of indexes of sub groups selections</param>
        /// <param name="mediaIndex">Media indexes</param>
        /// <param name="nbCol">Column number in result table</param>
        /// <param name="nbLineInNewTable">(out) Line number in table result</param>
        /// <param name="nbUnivers">(out) Univers number</param>
        /// <param name="mediaListForLabelSearch">(out)Media Ids list</param>
        /// <returns>Data table</returns>
        protected virtual ResultTable GetGrossTable(out Dictionary<Int64, HeaderBase> universesSubTotal, out Dictionary<string, HeaderBase> elementsHeader, out Dictionary<string, HeaderBase> elementsSubTotal)
        {

            #region Load data from data layer
            DataTable dt = null;
            DataSet dsMedia = null;

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
            object[] parameters = new object[1];
            parameters[0] = _session;
            IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            dt = presentAbsentDAL.GetData().Tables[0];
            dsMedia = presentAbsentDAL.GetColumnDetails();

            DataTable dtMedia = dsMedia.Tables[0];

            if (dt == null || dt.Rows.Count == 0)
            {
                universesSubTotal = null;
                elementsHeader = null;
                elementsSubTotal = null;
                return null;
            }
            #endregion

            #region Get Headers
            Dictionary<Int64, Int64> mediaToUnivers = null;
            Headers headers = GetHeaders(dtMedia, out elementsHeader, out elementsSubTotal, out universesSubTotal, out mediaToUnivers);
            #endregion

            #region Init ResultTable
            Int32 nbline = GetNbLine(dt);
            ResultTable tabData = new ResultTable(nbline, headers);
            #endregion

            #region Fill result table
            int levelNb = _session.GenericProductDetailLevel.GetNbLevels;
            Int64[] oldIds = new Int64[levelNb];
            Int64[] cIds = new Int64[levelNb];
            CellLevel[] levels = new CellLevel[nbline];
            Int32 cLine = 0;
            for (int i = 0; i < levelNb; i++) { oldIds[i] = cIds[i] = long.MinValue; }
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
                        for (int ii = i + 1; ii < levelNb; ii++) { oldIds[ii] = long.MinValue; }
                        cLine = InitDoubleLine(tabData, row, cellFactory, i + 1, (i > 0) ? levels[i - 1] : null);
                        levels[i] = (CellLevel)tabData[cLine, 1];
                    }
                }
                setLine(tabData, elementsHeader, elementsSubTotal, cLine, row, cellFactory, mediaToUnivers);
            }
            #endregion

            return tabData;

        }

        #region InitLineDelegate
        /// <summary>
        /// Delegate to init lines of type double
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory</param>
        /// <param name="level">Current level</param>
        /// <param name="parent">Parent level</param>
        /// <returns>Index of current line</returns>
        protected virtual Int32 InitDoubleLine(ResultTable tab, DataRow row, CellUnitFactory cellFactory, Int32 level, CellLevel parent)
        {

            Int32 cLine = InitLine(tab, row, level, parent);
            for (int i = 2; i <= tab.DataColumnsNumber; i++)
            {
                tab[cLine, i] = cellFactory.Get(null);
            }
            return cLine;

        }
        /// <summary>
        /// Init default values such as levels, Adresses...
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="row">Data container</param>
        /// <param name="level">Current level</param>
        /// <param name="parent">Parent level</param>
        /// <returns>Index of current line</returns>
        protected virtual Int32 InitLine(ResultTable tab, DataRow row, Int32 level, CellLevel parent)
        {

            Int32 cLine = -1;
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
            return cLine;

        }
        #endregion

        #region SetLineDelegate
        /// <summary>
        /// Delegate to affect values to the table
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="elementsHeader">Headers by element ids (media, interst centers...)</param>
        /// <param name="cLine">Current line</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory</param>
        /// <returns>Current line</returns>
        protected delegate Int32 SetLineDelegate(ResultTable tab, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal, Int32 cLine, DataRow row, CellUnitFactory cellFactory, Dictionary<Int64, Int64> mediaToUnivers);
        /// <summary>
        /// Delegate to affect double values to the table
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="elementsHeader">Headers by element ids (media, interst centers...)</param>
        /// <param name="cLine">Current line</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory for double cells</param>
        /// <returns>Current line</returns>
        protected virtual Int32 SetDoubleLine(ResultTable tab, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal, Int32 cLine, DataRow row, CellUnitFactory cellFactory, Dictionary<Int64, Int64> mediaToUnivers)
        {
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

            Int64 idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
            Int64 idMedia = Convert.ToInt64(row["id_media"]);
            Double value = Convert.ToDouble(row[_session.GetSelectedUnit().Id.ToString()]);
            string sIdElement = string.Format("{0}-{1}", mediaToUnivers[idMedia], idElement);
            tab.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[sIdElement].IndexInResultTable, value);
            // SubTotal if required (univers contains more than one element)
            if (elementsHeader[sIdElement] != elementsSubTotal[sIdElement])
            {
                tab.AffectValueAndAddToHierarchy(1, cLine, elementsSubTotal[sIdElement].IndexInResultTable, value);
            }
            // Total if required
            if (elementsHeader.ContainsKey(TOTAL_HEADER_ID.ToString()) && elementsSubTotal[sIdElement] != elementsHeader[TOTAL_HEADER_ID.ToString()])
            {
                tab.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[TOTAL_HEADER_ID.ToString()].IndexInResultTable, value);
            }
            return cLine;

        }
        /// <summary>
        /// Delegate to affect list values to the table
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="elementsHeader">Headers by element ids (media, interst centers...)</param>
        /// <param name="cLine">Current line</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory for list cells</param>
        /// <returns>Current line</returns>
        protected virtual Int32 SetListLine(ResultTable tab, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal, Int32 cLine, DataRow row, CellUnitFactory cellFactory, Dictionary<Int64, Int64> mediaToUnivers)
        {
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

            Int64 idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
            Int64 idMedia = Convert.ToInt64(row["id_media"]);
            string[] value = row[_session.GetSelectedUnit().Id.ToString()].ToString().Split(',');
            string sIdElement = string.Format("{0}-{1}", mediaToUnivers[idMedia], idElement);
            bool afectTotal = elementsHeader.ContainsKey(TOTAL_HEADER_ID.ToString()) && elementsSubTotal[sIdElement] != elementsHeader[TOTAL_HEADER_ID.ToString()];
            bool afectSubTotal = elementsHeader[sIdElement] != elementsSubTotal[sIdElement];
            Int64 v = 0;
            foreach (string s in value)
            {
                v = Convert.ToInt64(s);
                tab.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[sIdElement].IndexInResultTable, v);
                // SubTotal if required (univers contains more than one element)
                if (afectSubTotal)
                {
                    tab.AffectValueAndAddToHierarchy(1, cLine, elementsSubTotal[sIdElement].IndexInResultTable, v);
                }
                // Total if required
                if (afectTotal)
                {
                    tab.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[TOTAL_HEADER_ID.ToString()].IndexInResultTable, v);
                }
            }

            return cLine;

        }
        #endregion

        #endregion

        #region Initialisation des indexes
        /// <summary>
        /// Init headers
        /// </summary>
        /// <param name="elementsHeaders">(ou) Header for each level element</param>
        /// <param name="dtMedia">List of medias with the detail level matching
        protected virtual Headers GetHeaders(DataTable dtMedia, out Dictionary<string, HeaderBase> elementsHeader, out Dictionary<string, HeaderBase> elementsSubTotal, out Dictionary<Int64, HeaderBase> universesSubTotal, out Dictionary<Int64, Int64> idMediaToIdUnivers)
        {

            #region Extract Media lists
            string tmp = string.Empty;
            Int64[] tIds = null;
            int iUnivers = 1;
            //Elements by univers
            Dictionary<Int64, List<Int64>> idsByUnivers = new Dictionary<Int64, List<Int64>>();
            //Media ids ==> id univers mapping
            idMediaToIdUnivers = new Dictionary<Int64, Int64>();
            //vehicle-level detail in column
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

            //Init media univers mapping

            #region Old
            //while (_session.CompetitorUniversMedia[iUnivers] != null)
            //{
            //    idsByUnivers.Add(iUnivers, new List<Int64>());
            //    // Load media ids
            //    tmp = _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[iUnivers], CstCustomer.Right.type.mediaAccess);
            //    tIds = Array.ConvertAll<string, Int64>(tmp.Split(','), (Converter<string, long>)delegate (string s) { return Convert.ToInt64(s); });
            //    //Init Media ids X univers
            //    foreach (Int64 l in tIds)
            //    {
            //        if (!idMediaToIdUnivers.ContainsKey(l))
            //        {
            //            idMediaToIdUnivers.Add(l, iUnivers);
            //        }
            //    }
            //    iUnivers++;
            //}
            //iUnivers--;
            #endregion

            for (int p = 0; p < _session.PrincipalMediaUniverses.Count; p++)
            {
                idsByUnivers.Add(p, new List<Int64>());
                var includedItems = _session.PrincipalMediaUniverses[p].GetLevelValue(TNSClassificationLevels.MEDIA, AccessType.includes);

                //Init Media ids X univers
                foreach (Int64 l in includedItems)
                {
                    if (!idMediaToIdUnivers.ContainsKey(l))
                    {
                        idMediaToIdUnivers.Add(l, p);
                    }
                }
            }

            //Dispatch elements in current univers
            List<Int64> idElements = new List<Int64>();
            StringBuilder sIdElments = new StringBuilder();
            Int64 idElement = long.MinValue;
            Int64 idMedia = long.MinValue;
            foreach (DataRow row in dtMedia.Rows)
            {
                idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
                idMedia = Convert.ToInt64(row["id_media"]);
                if (!idElements.Contains(idElement))
                {
                    idElements.Add(idElement);
                    sIdElments.AppendFormat("{0},", idElement);
                }
                if (!idsByUnivers[idMediaToIdUnivers[idMedia]].Contains(idElement))
                {
                    idsByUnivers[idMediaToIdUnivers[idMedia]].Add(idElement);
                }

            }
            if (sIdElments.Length > 0) sIdElments.Length -= 1;
            #endregion

            #region Load elements labels
            DALClassif.ClassificationLevelListDataAccess levels = null;

            switch (columnDetailLevel.Id)
            {

                case DetailLevelItemInformation.Levels.media:
                    levels = new DALClassif.MediaBranch.PartialMediaListDataAccess(sIdElments.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.category:
                    levels = new DALClassif.MediaBranch.PartialCategoryListDataAccess(sIdElments.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.mediaSeller:
                    levels = new DALClassif.MediaBranch.PartialMediaSellerListDataAccess(sIdElments.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.title:
                    levels = new DALClassif.MediaBranch.PartialTitleListDataAccess(sIdElments.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.interestCenter:
                    levels = new DALClassif.MediaBranch.PartialInterestCenterListDataAccess(sIdElments.ToString(), _session.DataLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.basicMedia:
                    levels = new DALClassif.MediaBranch.PartialBasicMediaListDataAccess(sIdElments.ToString(), _session.DataLanguage, _session.Source);
                    break;

            }
            #endregion

            #region Build headers

            #region Current Columns
            // Product column
            Headers headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), LEVEL_HEADER_ID));


            // Add Creative coumn
            if (_vehicleInformation.ShowCreations &&
                _session.CustomerLogin.ShowCreatives(_vehicleInformation.Id) &&
                (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)))
            {
                headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(1994, _session.SiteLanguage), CREATIVE_HEADER_ID));
                _showCreative = true;
            }

            // Add insertion colmun
            if (_vehicleInformation.ShowInsertions &&
                (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)))
            {
                headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(2245, _session.SiteLanguage), INSERTION_HEADER_ID));
                _showInsertions = true;
            }

            // Add Media Schedule column
            if ((_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.holdingCompany) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.sector) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subSector) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.group) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.segment) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subBrand)
                )
                && _session.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null
                )
            {
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(150, _session.SiteLanguage), MEDIA_SCHEDULE_HEADER_ID));
                _showMediaSchedule = true;
            }
            #endregion

            #region Total column
            Header headerTmp = null;
            Header headerTotal = null;
            elementsHeader = new Dictionary<string, HeaderBase>();
            if ((_session.PrincipalMediaUniverses != null && _session.PrincipalMediaUniverses.Count > 1) || idElements.Count > 1)
            {
                headerTotal = new Header(true, GestionWeb.GetWebWord(805, _session.SiteLanguage), TOTAL_HEADER_ID);
                elementsHeader.Add(TOTAL_HEADER_ID.ToString(), headerTotal);
                headers.Root.Add(headerTotal);
                _showTotal = true;
            }
            #endregion

            #region Elements groups
            HeaderGroup headerGroupTmp = null;
            Header headerGroupSubTotal = null;
            iUnivers = 1;
            elementsSubTotal = new Dictionary<string, HeaderBase>();
            universesSubTotal = new Dictionary<Int64, HeaderBase>();

            #region Old
            //while (_session.CompetitorUniversMedia[iUnivers] != null)
            //{
            //    //Group init
            //    if (iUnivers != 1)
            //    {
            //        headerGroupTmp = new HeaderGroup(string.Format("{0} {1}", GestionWeb.GetWebWord(1366, _session.SiteLanguage), iUnivers - 1), true, START_ID_GROUP + iUnivers);
            //    }
            //    else
            //    {
            //        headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(1365, _session.SiteLanguage), true, START_ID_GROUP + iUnivers);
            //    }
            //    if (idsByUnivers[iUnivers].Count > 1 && _session.CompetitorUniversMedia.Count > 1)
            //    {
            //        headerGroupSubTotal = headerGroupTmp.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUB_TOTAL_HEADER_ID);
            //        universesSubTotal.Add(iUnivers, headerGroupSubTotal);
            //    }
            //    List<Header> heads = new List<Header>();
            //    foreach (Int64 id in idsByUnivers[iUnivers])
            //    {
            //        headerTmp = new Header(true, levels[id], id);
            //        heads.Add(headerTmp);
            //        elementsHeader.Add(string.Format("{0}-{1}", iUnivers, id), headerTmp);
            //        if (!headerGroupTmp.ContainSubTotal)
            //        {
            //            if (!universesSubTotal.ContainsKey(iUnivers))
            //            {
            //                if (iUnivers == 1 && idsByUnivers.Count < 2 && idsByUnivers[1].Count > 1)
            //                {
            //                    universesSubTotal.Add(iUnivers, headerTotal);
            //                }
            //                else
            //                {
            //                    universesSubTotal.Add(iUnivers, headerTmp);
            //                }
            //            }
            //            elementsSubTotal.Add(string.Format("{0}-{1}", iUnivers, id), headerTmp);
            //        }
            //        else
            //        {
            //            elementsSubTotal.Add(string.Format("{0}-{1}", iUnivers, id), headerGroupSubTotal);
            //        }
            //    }
            //    heads.Sort(delegate (Header h1, Header h2) { return h1.Label.CompareTo(h2.Label); });
            //    foreach (Header h in heads)
            //    {
            //        headerGroupTmp.Add(h);
            //    }
            //    headers.Root.Add(headerGroupTmp);
            //    iUnivers++;
            //}

            #endregion

            for (int p = 0; p < _session.PrincipalMediaUniverses.Count; p++)
            {
                var includedItems = _session.PrincipalMediaUniverses[p].GetLevelValue(TNSClassificationLevels.MEDIA, AccessType.includes);

                //Group init
                if (p > 0)
                {
                    headerGroupTmp = new HeaderGroup(string.Format("{0} {1}", GestionWeb.GetWebWord(1366, _session.SiteLanguage), p), true, START_ID_GROUP + p);
                }
                else
                {
                    headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(1365, _session.SiteLanguage), true, START_ID_GROUP + p);
                }
                if (idsByUnivers[p].Count > 1 && _session.PrincipalMediaUniverses.Count > 1)
                {
                    headerGroupSubTotal = headerGroupTmp.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUB_TOTAL_HEADER_ID);
                    universesSubTotal.Add(p, headerGroupSubTotal);
                }
                List<Header> heads = new List<Header>();
                foreach (Int64 id in idsByUnivers[p])
                {
                    headerTmp = new Header(true, levels[id], id);
                    heads.Add(headerTmp);
                    elementsHeader.Add(string.Format("{0}-{1}", p, id), headerTmp);
                    if (!headerGroupTmp.ContainSubTotal)
                    {
                        if (!universesSubTotal.ContainsKey(p))
                        {
                            if (p == 0 && idsByUnivers.Count < 2 && idsByUnivers[p].Count > 1)
                            {
                                universesSubTotal.Add(p, headerTotal);
                            }
                            else
                            {
                                universesSubTotal.Add(p, headerTmp);
                            }
                        }
                        elementsSubTotal.Add(string.Format("{0}-{1}", p, id), headerTmp);
                    }
                    else
                    {
                        elementsSubTotal.Add(string.Format("{0}-{1}", p, id), headerGroupSubTotal);
                    }
                }
                heads.Sort(delegate (Header h1, Header h2) { return h1.Label.CompareTo(h2.Label); });
                foreach (Header h in heads)
                {
                    headerGroupTmp.Add(h);
                }
                headers.Root.Add(headerGroupTmp);
            }

            #endregion

            #endregion

            return headers;

        }
        #endregion

        #region Format ResultTable
        protected virtual ResultTable GetResultTable(ResultTable grossTable, Int32 nbLine, Dictionary<Int64, HeaderBase> universesSubTotal, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal)
        {

            #region Line Number
            //Total Line
            nbLine++;
            //Parution Number
            Dictionary<Int64, double> resNbParution = null;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            if (!_session.Percentage && columnDetailLevel.Id == DetailLevelItemInformation.Levels.media && (_vehicleInformation.Id == CstDBClassif.Vehicles.names.press || _vehicleInformation.Id == CstDBClassif.Vehicles.names.internationalPress
                || _vehicleInformation.Id == CstDBClassif.Vehicles.names.newspaper
                || _vehicleInformation.Id == CstDBClassif.Vehicles.names.magazine))
            {
                resNbParution = GetNbParutionsByMedia();
                if (resNbParution != null && resNbParution.Count > 0)
                    nbLine++;
            }
            #endregion

            #region Init indexes and tables
            ResultTable tab = new ResultTable(nbLine, grossTable.NewHeaders);
            bool computePDM = (_session.Percentage) ? true : false;
            Int32 cLine = 0;
            Int32 creaIndex = tab.GetHeadersIndexInResultTable(CREATIVE_HEADER_ID.ToString());
            Int32 msIndex = tab.GetHeadersIndexInResultTable(MEDIA_SCHEDULE_HEADER_ID.ToString());
            Int32 insertIndex = tab.GetHeadersIndexInResultTable(INSERTION_HEADER_ID.ToString());
            Int32 totalIndex = tab.GetHeadersIndexInResultTable(TOTAL_HEADER_ID.ToString());
            Int32 iFirstDataIndex = 2;
            if (_showCreative) iFirstDataIndex++;
            if (_showInsertions) iFirstDataIndex++;
            if (_showMediaSchedule) iFirstDataIndex++;
            Int64 nbLevel = _session.GenericProductDetailLevel.GetNbLevels;
            CellLevel[] parents = new CellLevel[nbLevel + 1];
            #endregion

            #region Init Units dimensions
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();
            InitFinalLineValuesDelegate initLine;
            SetFinalLineDelegate setLine;
            switch (_session.Unit)
            {
                case CstWeb.CustomerSessions.Unit.versionNb:
                    initLine = new InitFinalLineValuesDelegate(InitFinalListValuesLine);
                    setLine = new SetFinalLineDelegate(SetFinalListLine);
                    break;
                default:
                    initLine = new InitFinalLineValuesDelegate(InitFinalDoubleValuesLine);
                    setLine = new SetFinalLineDelegate(SetFinalDoubleLine);
                    break;
            }
            #endregion

            #region Total
            CellLevel cLevelTotal = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), 0, cLine);
            parents[0] = null;
            parents[1] = cLevelTotal;
            cLine = tab.AddNewLine(LineType.total);
            //Total label
            tab[cLine, 1] = cLevelTotal;
            if (_showCreative) tab[cLine, creaIndex] = new CellOneLevelCreativesLink(cLevelTotal, _session, _session.GenericProductDetailLevel);
            if (_showInsertions) tab[cLine, insertIndex] = new CellOneLevelInsertionsLink(cLevelTotal, _session, _session.GenericProductDetailLevel);
            if (_showMediaSchedule) tab[cLine, msIndex] = new CellMediaScheduleLink(cLevelTotal, _session);
            initLine(iFirstDataIndex, tab, cLine, cellFactory, computePDM);
            #endregion

            #region Nombre parutions by media
            if (resNbParution != null && resNbParution.Count > 0)
            {
                CellNumber cNb = new CellNumber();
                cNb.StringFormat = "{0:max0}";
                CellUnitFactory nbFactory = new CellUnitFactory(cNb);
                cLine = tab.AddNewLine(LineType.nbParution);
                CellLevel cLevelParution = new CellLevel(-1, GestionWeb.GetWebWord(2460, _session.SiteLanguage), 0, cLine);
                tab[cLine, 1] = cLevelParution;
                if (_showCreative) tab[cLine, creaIndex] = new CellOneLevelCreativesLink(cLevelParution, _session, _session.GenericProductDetailLevel);
                if (_showInsertions) tab[cLine, insertIndex] = new CellOneLevelInsertionsLink(cLevelParution, _session, _session.GenericProductDetailLevel);
                if (_showMediaSchedule) tab[cLine, msIndex] = new CellMediaScheduleLink(cLevelParution, _session);
                if (_showTotal) { tab[cLine, totalIndex] = new CellNumber(); ((CellNumber)tab[cLine, totalIndex]).StringFormat = "{0:max0}"; ((CellNumber)tab[cLine, totalIndex]).AsposeFormat = 3; }
                for (Int32 i = iFirstDataIndex; i <= tab.DataColumnsNumber; i++)
                {
                    tab[cLine, i] = nbFactory.Get(null);
                }

                //Insert numbers of parutions
                foreach (HeaderBase h in elementsHeader.Values)
                {
                    if (resNbParution.ContainsKey(h.Id))
                    {
                        tab[cLine, h.IndexInResultTable] = nbFactory.Get(resNbParution[h.Id]);
                    }
                }

            }
            #endregion

            #region Fill final table
            CellLevel cLevel = null;
            for (int i = 0; i < grossTable.LinesNumber; i++)
            {

                if (grossTable.GetLineStart(i) is LineHide)
                    continue;

                #region Init Line
                cLine = InitFinalLine(grossTable, tab, i, parents[((CellLevel)grossTable[i, 1]).Level], creaIndex, insertIndex, msIndex);
                initLine(iFirstDataIndex, tab, cLine, cellFactory, computePDM);
                cLevel = (CellLevel)tab[cLine, 1];
                #endregion

                if (cLevel.Level < nbLevel)
                {
                    parents[cLevel.Level + 1] = cLevel;
                }
                else
                {
                    setLine(grossTable, tab, i, cLine, elementsHeader, elementsSubTotal);
                }

            }
            #endregion

            return tab;
        }

        #region InitFinalLineValuesDelegate
        protected delegate Int32 InitFinalLineValuesDelegate(Int32 iFirstDataIndex, ResultTable toTab, Int32 toLine, CellUnitFactory cellFactory, bool isPDM);
        protected virtual Int32 InitFinalDoubleValuesLine(Int32 iFirstDataIndex, ResultTable toTab, Int32 toLine, CellUnitFactory cellFactory, bool isPDM)
        {

            Int32 t = toTab.GetHeadersIndexInResultTable(TOTAL_HEADER_ID.ToString());
            CellUnit cell = null;
            if (t > -1)
            {
                if (!isPDM)
                {
                    toTab[toLine, t] = cellFactory.Get(null);
                }
                else
                {
                    toTab[toLine, t] = cell = GetCellPDM(null);
                    ((CellUnit)toTab[toLine, t]).StringFormat = "{0:percentWOSign}";
                }
                t++;
            }
            else
            {
                t = iFirstDataIndex;
            }

            for (Int32 i = t; i <= toTab.DataColumnsNumber; i++)
            {
                if (!isPDM)
                {
                    toTab[toLine, i] = cellFactory.Get(null);
                }
                else
                {
                    toTab[toLine, i] = GetCellPDM(cell);
                    ((CellUnit)toTab[toLine, i]).StringFormat = "{0:percentWOSign}";
                }
            }
            return toLine;

        }
        /// <summary>
        /// Get Cell PDM
        /// </summary>
        /// <param name="cell">Reference Cell</param>
        /// <returns>CellPDM</returns>
        protected virtual CellUnit GetCellPDM(CellUnit cell)
        {

            CellPDM cellPDM = new CellPDM(null, cell);
            return cellPDM;

        }
        protected Int32 InitFinalListValuesLine(Int32 iFirstDataIndex, ResultTable toTab, Int32 toLine, CellUnitFactory cellFactory, bool isPDM)
        {

            Int32 t = toTab.GetHeadersIndexInResultTable(TOTAL_HEADER_ID.ToString());
            CellVersionNbPDM cell = null;
            if (t > -1)
            {
                if (!isPDM)
                {
                    toTab[toLine, t] = cellFactory.Get(null);
                }
                else
                {
                    toTab[toLine, t] = cell = new CellVersionNbPDM(null);
                    ((CellVersionNbPDM)toTab[toLine, t]).StringFormat = "{0:percentWOSign}";
                }
                t++;
            }
            else
            {
                t = iFirstDataIndex;
            }

            for (Int32 i = t; i <= toTab.DataColumnsNumber; i++)
            {
                if (!isPDM)
                {
                    toTab[toLine, i] = cellFactory.Get(null);
                }
                else
                {
                    toTab[toLine, i] = new CellVersionNbPDM(cell);
                    ((CellVersionNbPDM)toTab[toLine, i]).StringFormat = "{0:percentWOSign}";
                }
            }
            return toLine;

        }
        protected virtual Int32 InitFinalLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, CellLevel parent, Int32 creaIndex, Int32 insertIndex, Int32 msIndex)
        {
            CellLevel cFromLevel = (CellLevel)fromTab[fromLine, 1];
            Int32 cLine = toTab.AddNewLine(fromTab.GetLineStart(fromLine).LineType);
            AdExpressCellLevel cell = new AdExpressCellLevel(cFromLevel.Id, cFromLevel.Label, parent, cFromLevel.Level, cLine, _session);
            toTab[cLine, 1] = cell;

            //Links
            if (_showCreative) toTab[cLine, creaIndex] = new CellOneLevelCreativesLink(cell, _session, _session.GenericProductDetailLevel);
            if (_showInsertions) toTab[cLine, insertIndex] = new CellOneLevelInsertionsLink(cell, _session, _session.GenericProductDetailLevel);
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
        protected delegate Int64 SetFinalLineDelegate(ResultTable fromTab, ResultTable toTab, Int32 fromLine, Int32 toLine, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal);
        protected virtual Int64 SetFinalDoubleLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, Int32 toLine, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal)
        {

            CellLevel level = (CellLevel)fromTab[fromLine, 1];
            HeaderBase hTotal = (elementsHeader.ContainsKey(TOTAL_HEADER_ID.ToString())) ? elementsHeader[TOTAL_HEADER_ID.ToString()] : null;

            //elements
            HeaderBase cHeader = null;
            HeaderBase subTotalHeader = null;
            double? value = 0.0;
            foreach (string s in elementsHeader.Keys)
            {
                cHeader = elementsHeader[s];
                if (cHeader != hTotal)
                {
                    value = ((CellUnit)fromTab[fromLine, cHeader.IndexInResultTable]).GetNullableValue();
                    if (value.HasValue)
                    {
                        toTab.AffectValueAndAddToHierarchy(1, toLine, cHeader.IndexInResultTable, value.Value);
                        //univers sub total
                        subTotalHeader = elementsSubTotal[s];
                        if (subTotalHeader != cHeader && subTotalHeader != hTotal)
                        {
                            toTab.AffectValueAndAddToHierarchy(1, toLine, subTotalHeader.IndexInResultTable, value.Value);
                        }
                        //line total
                        if (hTotal != null)
                        {
                            toTab.AffectValueAndAddToHierarchy(1, toLine, hTotal.IndexInResultTable, value.Value);
                        }
                    }

                }

            }

            return toLine;

        }
        protected virtual Int64 SetFinalListLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, Int32 toLine, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal)
        {

            CellLevel level = (CellLevel)fromTab[fromLine, 1];
            HeaderBase hTotal = (elementsHeader.ContainsKey(TOTAL_HEADER_ID.ToString())) ? elementsHeader[TOTAL_HEADER_ID.ToString()] : null;

            //elements
            HeaderBase cHeader = null;
            HeaderBase subTotalHeader = null;
            HybridList value = null;
            Int64 l = long.MinValue;
            Int64 len = 0;
            bool affectTotal = hTotal != null;
            foreach (string s in elementsHeader.Keys)
            {
                cHeader = elementsHeader[s];
                if (cHeader != hTotal)
                {
                    value = ((CellIdsNumber)fromTab[fromLine, cHeader.IndexInResultTable]).List;
                    len = value.length;
                    for (int i = 0; i < len; i++)
                    {
                        l = value.removeHead().UniqueID;
                        toTab.AffectValueAndAddToHierarchy(1, toLine, cHeader.IndexInResultTable, l);
                        //univers sub total
                        subTotalHeader = elementsSubTotal[s];
                        if (subTotalHeader != cHeader && subTotalHeader != hTotal)
                        {
                            toTab.AffectValueAndAddToHierarchy(1, toLine, subTotalHeader.IndexInResultTable, l);
                        }
                        //line total
                        if (affectTotal)
                        {
                            toTab.AffectValueAndAddToHierarchy(1, toLine, hTotal.IndexInResultTable, l);
                        }
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
        protected virtual Int32 GetNbLine(DataTable dt)
        {

            Int32 nbLine = 0;
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
                if (cIdL1 > -1 && cIdL1 != oldIdL1)
                {
                    oldIdL1 = cIdL1;
                    oldIdL2 = oldIdL3 = -1;
                    nbLine++;
                }
                if (cIdL2 > -1 && cIdL2 != oldIdL2)
                {
                    oldIdL2 = cIdL2;
                    oldIdL3 = -1;
                    nbLine++;
                }
                if (cIdL3 > -1 && cIdL3 != oldIdL3)
                {
                    oldIdL3 = cIdL3;
                    nbLine++;
                }
            }
            return nbLine;
        }
        #endregion

        #region Obtient l'activité publicitaire d'un produit
        /// <summary>
        /// Get Advertising Product Activity
        /// </summary>
        /// <param name="tabResult">Result Table</param>
        /// <param name="dt">Data table</param>
        /// <param name="indexLineProduct">Product line index</param>
        /// <param name="filterExpression">filter</param>
        /// <param name="sort">sorting</param>
        /// <param name="referenceUniversMedia">Reference Media List</param>
        /// <param name="competitorUniversMedia">Competitor Media List</param>	
        protected virtual void GetProductActivity(ResultTable tabResult, DataTable dt, Int32 indexLineProduct, string filterExpression, string sort, List<string> referenceUniversMedia, List<string> competitorUniversMedia, AddValue addValueDelegate, SetSynthesisTable setSynthesisDataDelegate, InitValue initValueDelegate)
        {
            CellUnit unitReferenceMedia = initValueDelegate();
            CellUnit unitCompetitorMedia = initValueDelegate();
            string unitAlias = FctWeb.SQLGenerator.GetUnitAlias(_session);
            Int32 presentNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 absentNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 exclusiveNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            Int32 presentReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);
            Int32 absentReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);
            Int32 exclusiveReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);

            Int32 presentCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);
            Int32 absentCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);
            Int32 exclusiveCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);

            DataRow[] foundRows = dt.Select(filterExpression, sort);

            if (foundRows != null && !foundRows.Equals(System.DBNull.Value) && foundRows.Length > 0)
            {
                for (int i = 0; i < foundRows.Length; i++)
                {
                    //Unité supports de référence
                    if (foundRows[i]["id_media"] != null && foundRows[i]["id_media"] != System.DBNull.Value && referenceUniversMedia != null &&
                        foundRows[i][unitAlias] != null && foundRows[i][unitAlias] != System.DBNull.Value && referenceUniversMedia.Contains(foundRows[i]["id_media"].ToString()))
                    {
                        addValueDelegate(unitReferenceMedia, foundRows[i][unitAlias]);
                        //unitReferenceMedia += double.Parse(foundRows[i][unitAlias].ToString());
                    }
                    //Unité supports concurrents
                    if (foundRows[i]["id_media"] != null && foundRows[i]["id_media"] != System.DBNull.Value && competitorUniversMedia != null &&
                        foundRows[i][unitAlias] != null && foundRows[i][unitAlias] != System.DBNull.Value && competitorUniversMedia.Contains(foundRows[i]["id_media"].ToString()))
                    {
                        addValueDelegate(unitCompetitorMedia, foundRows[i][unitAlias]);
                        //unitCompetitorMedia += double.Parse(foundRows[i][unitAlias].ToString());
                    }
                }
            }

            #region Communs
            if (unitReferenceMedia.CompareTo(0) > 0 && unitCompetitorMedia.CompareTo(0) > 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, presentNumberColumnIndex]).Value += 1;

                //supports de référence communs
                setSynthesisDataDelegate(tabResult, indexLineProduct, presentReferenceColumnIndex, unitReferenceMedia);
                //((CellUnit)tabResult[indexLineProduct, presentReferenceColumnIndex]).Value += unitReferenceMedia;

                //supports concurrents communs
                setSynthesisDataDelegate(tabResult, indexLineProduct, presentCompetitorColumnIndex, unitCompetitorMedia);
                //((CellUnit)tabResult[indexLineProduct, presentCompetitorColumnIndex]).Value += unitCompetitorMedia;

            }
            #endregion

            #region Absents
            if (unitReferenceMedia.CompareTo(0) == 0 && unitCompetitorMedia.CompareTo(0) > 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, absentNumberColumnIndex]).Value += 1;

                //supports concurrents Absents
                setSynthesisDataDelegate(tabResult, indexLineProduct, absentCompetitorColumnIndex, unitCompetitorMedia);
                //((CellUnit)tabResult[indexLineProduct, absentCompetitorColumnIndex]).Value += unitCompetitorMedia;

            }
            #endregion

            #region Exclusifs
            if (unitReferenceMedia.CompareTo(0) > 0 && unitCompetitorMedia.CompareTo(0) == 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, exclusiveNumberColumnIndex]).Value += 1;

                //supports de référence exclusifs
                setSynthesisDataDelegate(tabResult, indexLineProduct, exclusiveReferenceColumnIndex, unitReferenceMedia);
                //((CellUnit)tabResult[indexLineProduct, exclusiveReferenceColumnIndex]).Value += unitReferenceMedia;
            }
            #endregion

        }

        #region delegates
        protected delegate CellUnit InitValue();
        protected delegate void AddValue(CellUnit cell, object value);
        protected delegate void SetSynthesisTable(ResultTable tab, Int32 line, Int32 column, CellUnit value);

        #region InitValue
        protected virtual CellUnit InitDoubleValue()
        {

            return new CellNumber();

        }
        protected virtual CellUnit InitListValue()
        {

            return new CellIdsNumber();

        }
        #endregion

        #region AddValue(CellUnit cell, object value)
        protected virtual void AddDoubleValue(CellUnit cell, object value)
        {

            cell.Add(Convert.ToDouble(value));

        }
        protected virtual void AddListValue(CellUnit cell, object value)
        {

            string[] ids = value.ToString().Split(',');
            for (int i = 0; i < ids.Length; i++)
            {
                cell.Add(Convert.ToDouble(ids[i]));
            }

        }
        #endregion

        #region SetSynthesisTable(ResultTable tab, Int32 line, Int32 column, CellUnit value)
        protected virtual void SetDoubleSynthesisTable(ResultTable tab, Int32 line, Int32 column, CellUnit value)
        {

            ((CellUnit)tab[line, column]).Add(((CellUnit)value).Value);

        }
        protected virtual void SetListSynthesisTable(ResultTable tab, Int32 line, Int32 column, CellUnit value)
        {
            HybridList h = ((CellIdsNumber)value).List;
            CellIdsNumber c = (CellIdsNumber)tab[line, column];
            Int64 len = h.length;
            for (int i = 0; i < len; i++)
            {
                c.Add(h.removeHead().UniqueID);
            }

        }
        #endregion

        #endregion

        #endregion

        #region GetNbParutionsByMedia
        /// <summary>
        /// Get Number of parution by media
        /// </summary>
        /// <param name="webSession"> Client Session</param>
        /// <returns>Number of parution by media data</returns>
        protected virtual Dictionary<Int64, double> GetNbParutionsByMedia()
        {

            #region Variables
            Dictionary<Int64, double> res = new Dictionary<Int64, double>();
            double nbParutionsCounter = 0;
            bool start = true;
            Int64 oldKey = long.MinValue;
            Int64 cKey = long.MinValue;
            #endregion

            #region Chargement des données à partir de la base
            DataSet ds;
            try
            {
                if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
                object[] parameters = new object[1];
                parameters[0] = _session;
                IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                ds = presentAbsentDAL.GetNbParutionData();

            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentException("Impossible de charger les données pour le nombre de parution", err));
            }
            DataTable dt = ds.Tables[0];
            #endregion

            if (dt != null && dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {

                    cKey = Convert.ToInt64(dr["id_media"]);
                    if (oldKey != cKey && !start)
                    {
                        res.Add(oldKey, nbParutionsCounter);
                        nbParutionsCounter = 0;
                    }
                    nbParutionsCounter += Convert.ToDouble(dr["NbParution"]);
                    start = false;
                    oldKey = cKey;

                }
                res.Add(oldKey, nbParutionsCounter);

            }

            return res;

        }

        public GridResult GetGridResult()
        {
            GridResult gridResult = new GridResult();
            ResultTable resultTable = GetResult();
            string mediaSchedulePath = "/MediaSchedulePopUp";
            string insertionPath = "/Insertions";
            string versionPath = "/Creative";
            string pickanewsLink = "http://www.pickanews.com";
            LineStart cLineStart = null;
            int nbLines = 0;
            _allowPickaNews = WebApplicationParameters.ShowPickaNews;

            if (resultTable == null || resultTable.DataColumnsNumber == 0)
            {
                gridResult.HasData = false;
                return gridResult;
            }


            resultTable.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
            resultTable.CultureInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            object[,] gridData = null;
            if (_session.CurrentTab != DynamicAnalysis.SYNTHESIS)
            {
                for (int i = 0; i < resultTable.LinesNumber; i++)
                {
                    cLineStart = resultTable.GetLineStart(i);
                    if (!(cLineStart is LineHide))
                        nbLines++;
                }
                if (nbLines == 0)
                {
                    gridResult.HasData = false;
                    return gridResult;
                }
                else if (nbLines > CstWeb.Core.MAX_ALLOWED_ROWS_NB)
                {
                    gridResult.HasData = true;
                    gridResult.HasMoreThanMaxRowsAllowed = true;
                    return (gridResult);
                }

                gridData = new object[nbLines, resultTable.ColumnsNumber + 1]; //+2 car ID et PID en plus  -  //_data.LinesNumber // + 1 for gad column
                if (_allowPickaNews)
                {
                    gridData = new object[nbLines, resultTable.ColumnsNumber + 1 + 1]; //+2 car ID et PID en plus  -  //_data.LinesNumber // + 1 for gad column // + 1 for pickanews
                }

            }
            else
            {
                nbLines = resultTable.LinesNumber;
                gridData = new object[nbLines, resultTable.ColumnsNumber + 1];
                if (_allowPickaNews)
                {
                    gridData = new object[nbLines, resultTable.ColumnsNumber + 1]; // + 1 for pickanews
                }

            }//+2 car ID et PID en plus  -  //_data.LinesNumber

            List<string> listStringNotAllowedSorting = new List<string> {
                GestionWeb.GetWebWord(150, _session.SiteLanguage), //Planmedia
                GestionWeb.GetWebWord(751, _session.SiteLanguage), //PlanMedia
                GestionWeb.GetWebWord(1994, _session.SiteLanguage), //Versions
                GestionWeb.GetWebWord(2245, _session.SiteLanguage), //Insertions
            };

            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();
            List<object> columnsNotAllowedSorting = new List<object>();

            //Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            if (_session.CurrentTab != DynamicAnalysis.SYNTHESIS)
            {
                columns.Add(new { headerText = "GAD", key = "GAD", dataType = "string", width = "*", hidden = true });
                schemaFields.Add(new { name = "GAD" });

            }

            List<object> groups = null;
            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            string format = string.Empty;
            List<object> subGroups = null;

            //Headers
            if (resultTable.NewHeaders != null)
            {
                for (int j = 0; j < resultTable.NewHeaders.Root.Count; j++)
                {
                    groups = null;
                    string colKey = string.Empty;
                    if (resultTable.NewHeaders.Root[j].Count > 0)
                    {
                        groups = new List<object>();

                        int nbGroupItems = resultTable.NewHeaders.Root[j].Count;
                        for (int g = 0; g < nbGroupItems; g++)
                        {
                            //Manage sub groups items (stynthesis result)
                            if (resultTable.NewHeaders.Root[j][g].Count > 0)
                            {
                                #region Sub group synthesis

                                subGroups = new List<object>();

                                int nbSubGroupitems = resultTable.NewHeaders.Root[j][g].Count;

                                for (int sg = 0; sg < nbSubGroupitems; sg++)
                                {
                                    colKey = string.Format("sg{0}", resultTable.NewHeaders.Root[j][g][sg].IndexInResultTable);
                                    //cell format sub group
                                    if (resultTable != null && resultTable.LinesNumber > 0)
                                    {
                                        var cell = resultTable[0, resultTable.NewHeaders.Root[j][g][sg].IndexInResultTable];

                                        if (cell is CellPercent || cell is CellPDM)
                                        {
                                            format = "percent";
                                            colKey += "-pdm";
                                        }
                                        else if (cell is CellEvol)
                                        {
                                            format = "percent";
                                            colKey += "-evol";
                                        }
                                        else if (cell is CellDuration)
                                        {
                                            format = "duration";
                                            colKey += "-unit";
                                        }
                                        else if (cell is CellUnit)
                                        {
                                            format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(_session.Unit).StringFormat);
                                            colKey += "-unit";
                                        }
                                    }

                                    subGroups.Add(new { headerText = resultTable.NewHeaders.Root[j][g][sg].Label, key = colKey, dataType = "number", format = format, columnCssClass = "colStyle", width = "*", allowSorting = true });
                                    schemaFields.Add(new { name = colKey });

                                }

                                colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j][g].IndexInResultTable);
                                groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = colKey, group = subGroups });
                                //schemaFields.Add(new { name = colKey });


                                #endregion
                            }
                            else
                            {
                                //No sub groups items
                                #region /No sub groups items
                                colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j][g].IndexInResultTable);
                                //cell format
                                if (resultTable != null && resultTable.LinesNumber > 0)
                                {
                                    var cell = resultTable[0, resultTable.NewHeaders.Root[j][g].IndexInResultTable];

                                    if (cell is CellPercent || cell is CellPDM)
                                    {
                                        format = "percent";
                                        colKey += "-pdm";
                                    }
                                    else if (cell is CellEvol)
                                    {
                                        format = "percent";
                                        colKey += "-evol";
                                    }
                                    else if (cell is CellDuration)
                                    {
                                        format = "duration";
                                        colKey += "-unit";
                                    }
                                    else if (cell is CellUnit)
                                    {
                                        format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(_session.Unit).StringFormat);
                                        colKey += "-unit";
                                    }
                                }

                                groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = colKey, dataType = "number", format = format, columnCssClass = "colStyle", width = "*", allowSorting = true });
                                schemaFields.Add(new { name = colKey });
                                #endregion

                            }


                        }

                        columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = "gr" + colKey, group = groups });
                        columnsFixed.Add(new { columnKey = "gr" + colKey, isFixed = false, allowFixing = false });
                    }
                    else
                    {
                        colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j].IndexInResultTable);
                        if (j == 0)
                        {
                            if (_session.CurrentTab == DynamicAnalysis.SYNTHESIS)
                            {
                                columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "350", allowSorting = true });
                            }
                            else columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "350", allowSorting = true, template = "{{if ${GAD}.length > 0}} <span class=\"gadLink\" href=\"#gadModal\" data-toggle=\"modal\" data-gad=\"[${GAD}]\">${" + colKey + "}</span> {{else}} ${" + colKey + "} {{/if}}" });
                            columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });

                            if (_allowPickaNews)
                            {
                                columns.Add(new { headerText = "<img src='/Content/img/pickanews-logo.png' als='PickaNews'>", key = "PICKANEWS", dataType = "string", width = "35", allowSorting = false });
                                schemaFields.Add(new { name = "PICKANEWS" });
                                columnsFixed.Add(new { columnKey = "PICKANEWS", isFixed = false, allowFixing = false });
                                columnsNotAllowedSorting.Add(new { columnKey = "PICKANEWS", allowSorting = false });
                            }
                        }
                        else
                        {
                            if (resultTable.NewHeaders.Root[j].Label == GestionWeb.GetWebWord(805, _session.SiteLanguage))
                            {
                                if (_session.Percentage)
                                {
                                    format = "percent";
                                    colKey += "-pdm";
                                }
                                else
                                {
                                    format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(_session.Unit).StringFormat);
                                    colKey += "-unit";
                                }

                                columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "number", format = format, columnCssClass = "colStyle", width = "*", allowSorting = true });
                            }
                            else
                            {
                                if (listStringNotAllowedSorting.Contains(resultTable.NewHeaders.Root[j].Label))
                                {
                                    columnsNotAllowedSorting.Add(new { columnKey = colKey, allowSorting = false });
                                }

                                columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "*" });
                            }

                            columnsFixed.Add(new { columnKey = colKey, isFixed = false, allowFixing = false });
                        }
                        schemaFields.Add(new { name = colKey });
                    }

                }
            }

            //table body rows
            int currentLine = 0;
            for (int i = 0; i < resultTable.LinesNumber; i++) //
            {
                cLineStart = resultTable.GetLineStart(i);
                if (cLineStart is LineHide)
                    continue;

                int pk = 0;
                if (_allowPickaNews)
                    pk++;

                gridData[currentLine, 0] = i; // Pour column ID
                gridData[currentLine, 1] = resultTable.GetSortedParentIndex(i); // Pour column PID

                for (int k = 1; k < resultTable.ColumnsNumber - 1; k++)
                {
                    var cell = resultTable[i, k];
                    var link = string.Empty;
                    if (cell is CellMediaScheduleLink)
                    {

                        var c = cell as CellMediaScheduleLink;
                        if (c != null)
                        {
                            link = c.GetLink();
                            if (!string.IsNullOrEmpty(link))
                            {
                                link = string.Format("<center><a href='{0}?{1}' target='_blank'><span class='fa fa-search-plus'></span></a></center>"
                           , mediaSchedulePath
                           , link);
                            }
                        }
                        gridData[currentLine, k + 2 + pk] = link;

                    }
                    else if (cell is CellOneLevelInsertionsLink)
                    {
                        var c = cell as CellOneLevelInsertionsLink;

                        if (c != null)
                        {
                            link = c.GetLink();
                            if (!string.IsNullOrEmpty(link))
                            {
                                link = string.Format("<center><a href='{0}?{1}' target='_blank'><span class='fa fa-search-plus'></span></a></center>"
                         , insertionPath
                         , link);
                            }

                        }
                        gridData[currentLine, k + 2 + pk] = link;
                    }
                    else if (cell is CellOneLevelCreativesLink)
                    {
                        var c = cell as CellOneLevelCreativesLink;

                        if (c != null)
                        {
                            link = c.GetLink();
                            if (!string.IsNullOrEmpty(link))
                            {
                                link = string.Format("<center><a href='{0}?{1}' target='_blank'><span class='fa fa-search-plus'></span></a></center>"
                         , versionPath
                         , link);
                            }

                        }
                        gridData[currentLine, k + 2 + pk] = link;
                    }
                    else
                    {
                        if (cell is CellPercent || cell is CellEvol || cell is CellPDM)
                        {
                            double value = ((CellUnit)cell).Value;

                            if (double.IsInfinity(value))
                                gridData[currentLine, k + 2 + pk] = "Infinity";
                            else if (double.IsNaN(value))
                                gridData[currentLine, k + 2 + pk] = null;
                            else
                                gridData[currentLine, k + 2 + pk] = value / 100;
                        }
                        else if (cell is CellUnit)
                        {
                            if (((LineStart)resultTable[i, 0]).LineType != LineType.nbParution)
                            {
                                if (_session.CurrentTab == DynamicAnalysis.SYNTHESIS)
                                {
                                    gridData[currentLine, k + 1 + pk] = FctWeb.Units.ConvertUnitValue(((CellUnit)cell).Value, _session.Unit);
                                }
                                else gridData[currentLine, k + 2 + pk] = FctWeb.Units.ConvertUnitValue(((CellUnit)cell).Value, _session.Unit);
                            }
                            else
                                gridData[currentLine, k + 2 + pk] = ((CellUnit)cell).Value;
                        }
                        else if (cell is AdExpressCellLevel)
                        {
                            string label = ((AdExpressCellLevel)cell).RawString();
                            string gadParams = ((AdExpressCellLevel)cell).GetGadParams();

                            if (gadParams.Length > 0)
                                gridData[currentLine, 2] = gadParams;
                            else
                                gridData[currentLine, 2] = "";

                            gridData[currentLine, k + 2 + pk] = label;

                            if (_allowPickaNews)
                            {
                                string url = string.Format("/find?q={0}#mon-dashboard", Uri.EscapeDataString(label));

                                gridData[currentLine, 3] = string.Format("<center><a href='{0}{1}' target='_blank' alt='pickanews link'><span class='fa fa-search-plus'></span></a></center>"
                                    , pickanewsLink
                                    , url
                                    );
                            }
                        }
                        else
                        {
                            if (_session.CurrentTab == DynamicAnalysis.SYNTHESIS)
                            {
                                gridData[currentLine, k + 1 + pk] = cell.RenderString();
                            }
                            else gridData[currentLine, k + 2 + pk] = cell.RenderString();
                        }
                    }
                }
                currentLine++;
            }
            gridResult.NeedFixedColumns = true;
            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            gridResult.ColumnsFixed = columnsFixed;
            gridResult.ColumnsNotAllowedSorting = columnsNotAllowedSorting;
            gridResult.Data = gridData;
            gridResult.Unit = _session.Unit.ToString();

            return gridResult;
        }
        #endregion

        #endregion

        private List<string> GetCompetitormedias(int position)
        {
            string mediaList = string.Empty;
            List<long> ids = new List<long>();
            for (int p = position; p < _session.PrincipalMediaUniverses.Count; p++)
            {
                ids.AddRange(_session.PrincipalMediaUniverses[p].GetLevelValue(TNSClassificationLevels.MEDIA, AccessType.includes));
            }

            return ids.ConvertAll(Convert.ToString);
        }
    }
}
