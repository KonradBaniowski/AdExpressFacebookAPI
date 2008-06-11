#region Information
/*
 * Author : G Ragneau
 * Creation : 18/03/2008
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

using DALClassif = TNS.AdExpress.DataAccess.Classification;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;

using TNS.AdExpress;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.PresentAbsent.Exceptions;
using System.Windows.Forms;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Result;
using Navigation = TNS.AdExpress.Domain.Web.Navigation;

using TNS.AdExpressI.PresentAbsent.DAL;
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
        protected const int TOTAL_HEADER_ID = 4;
        /// <summary>
        /// Univers SUb totals ID
        /// </summary>
        public const int SUB_TOTAL_HEADER_ID = 5;
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
        protected CstDBClassif.Vehicles.names _vehicle;
        /// <summary>
        /// Current Module
        /// </summary>
        protected Navigation.Module _module;
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
        public CstDBClassif.Vehicles.names Vehicle
        {
            get { return _vehicle; }
        }
        /// <summary>
        /// Get Current Module
        /// </summary>
        public Navigation.Module CurrentModule
        {
            get { return _module; }
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
            _vehicle = (CstDBClassif.Vehicles.names)int.Parse(vehicleSelection);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new PresentAbsentException("Uncorrect Media Selection"));
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
        public ResultTable GetStrengths()
        {
            this._result = CompetitorMarketShare.FORCES;
            return GetData();
        }

        /// <summary>
        /// Compute result "Prospects"
        /// </summary>
        /// <returns>Computed data</returns>
        public ResultTable GetProspects()
        {
            return GetResult(CompetitorMarketShare.POTENTIELS);
        }

        /// <summary>
        /// Compute specified result
        /// </summary>
        /// <param name="result">Type of result to compute</param>
        /// <returns>Computed data</returns>
        public ResultTable GetResult(int result)
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
        public ResultTable GetResult()
        {
            return GetResult((int)_session.CurrentTab);
        }
        #endregion

        #region Result Computing Methods

        #region Get Data
        /// <summary>
        /// Compute result specified in user session
        /// </summary>
        /// <param name="session">User session</param>
        /// <returns>Computed data</returns>
        protected ResultTable GetData()
        {

            #region Variables
            int positionUnivers = 1;
            long currentLine;
            long currentColumn;
            long currentLineInTabResult;
            long nbLineInTabResult = -1;
            bool allSubTotalNotNull = true;
            bool subTotalNotNull = true;
            #endregion

            #region Tableaux d'index
            SelectionGroup[] groupMediaTotalIndex = new SelectionGroup[11];
            SelectionSubGroup[] subGroupMediaTotalIndex = new SelectionSubGroup[1000];
            int nbUnivers = 0;
            Dictionary<Int64, GroupItemForTableResult> mediaIndex = new Dictionary<Int64, GroupItemForTableResult>();
            string mediaListForLabelSearch = "";
            int maxIndex = 0;
            #endregion

            #region Chargement du tableau
            long nbLineInNewTable = 0;
            object[,] tabData = GetPreformatedTable(groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, ref maxIndex, ref nbLineInNewTable, ref nbUnivers, ref mediaListForLabelSearch);
            #endregion

            if (tabData == null)
            {
                return null;
            }

            #region Déclaration du tableau de résultat
            long nbCol = tabData.GetLength(0);
            long nbLineInFormatedTable = nbLineInNewTable;
            object[,] tabResult = new object[nbCol, nbLineInFormatedTable];
            #endregion

            #region Traitement des données
            currentLineInTabResult = -1;

            switch (_session.CurrentTab)
            {

                #region Portefeuille
                case CompetitorMarketShare.PORTEFEUILLE:
                    // Pas de traitement
                    tabResult = tabData;
                    nbLineInTabResult = nbLineInFormatedTable;
                    break;
                #endregion

                #region Absent
                case CompetitorMarketShare.ABSENT:
                    for (currentLine = 0; currentLine < nbLineInFormatedTable; currentLine++)
                    {
                        positionUnivers = 1;
                        subTotalNotNull = false;
                        // On cherche les lignes qui on des unités à 0(null) dans le premier sous total
                        if ((double)tabData[groupMediaTotalIndex[1].IndexInResultTable, currentLine] == 0.0)
                        {
                            positionUnivers++;
                            while (!subTotalNotNull && positionUnivers < nbUnivers)
                            {
                                if ((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable, currentLine] != 0.0) subTotalNotNull = true;
                                positionUnivers++;
                            }
                            //au moins un sous total de concurrent différent à 0(null)
                            if (subTotalNotNull)
                            {
                                currentLineInTabResult++;
                                for (currentColumn = 0; currentColumn < nbCol; currentColumn++)
                                {
                                    tabResult[currentColumn, currentLineInTabResult] = tabData[currentColumn, currentLine];
                                }
                            }

                        }
                    }
                    nbLineInTabResult = currentLineInTabResult + 1;
                    break;
                #endregion

                #region Exclusif
                case CompetitorMarketShare.EXCLUSIF:
                    for (currentLine = 0; currentLine < nbLineInFormatedTable; currentLine++)
                    {
                        positionUnivers = 1;
                        allSubTotalNotNull = true;
                        // On cherche les lignes qui on des unités différentes de 0(null) dans le premier sous total
                        if ((double)tabData[groupMediaTotalIndex[1].IndexInResultTable, currentLine] != 0.0)
                        {
                            positionUnivers++;
                            while (allSubTotalNotNull && positionUnivers < nbUnivers)
                            {
                                if ((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable, currentLine] != 0.0) allSubTotalNotNull = false;
                                positionUnivers++;
                            }
                            // et tous les sous totaux de concurrent à 0(null)
                            if (allSubTotalNotNull)
                            {
                                currentLineInTabResult++;
                                for (currentColumn = 0; currentColumn < nbCol; currentColumn++)
                                {
                                    tabResult[currentColumn, currentLineInTabResult] = tabData[currentColumn, currentLine];
                                }
                            }

                        }
                    }
                    nbLineInTabResult = currentLineInTabResult + 1;
                    break;
                #endregion

                #region Commun
                case CompetitorMarketShare.COMMON:
                    for (currentLine = 0; currentLine < nbLineInFormatedTable; currentLine++)
                    {
                        allSubTotalNotNull = true;
                        positionUnivers = 1;
                        while (allSubTotalNotNull && positionUnivers < nbUnivers)
                        {
                            if ((double)tabData[groupMediaTotalIndex[positionUnivers].IndexInResultTable, currentLine] == 0.0) allSubTotalNotNull = false;
                            positionUnivers++;
                        }
                        if (allSubTotalNotNull)
                        {
                            currentLineInTabResult++;
                            for (currentColumn = 0; currentColumn < nbCol; currentColumn++)
                            {
                                tabResult[currentColumn, currentLineInTabResult] = tabData[currentColumn, currentLine];
                            }
                        }
                    }
                    nbLineInTabResult = currentLineInTabResult + 1;
                    break;
                #endregion

                #region Forces ou Potentiels
                case CompetitorMarketShare.FORCES:
                case CompetitorMarketShare.POTENTIELS:
                    tabResult = tabData;
                    nbLineInTabResult = nbLineInFormatedTable;
                    break;
                #endregion

            }
            #endregion

            #region Debug: Voir le tableau
            //						int i,j;
            //						string HTML="<html><table><tr>";
            //						for(i=0;i<=currentLineInTabResult;i++){
            //							for(j=0;j<nbCol;j++){
            //								if(tabResult[j,i]!=null)HTML+="<td>"+tabResult[j,i].ToString()+"</td>";
            //								else HTML+="<td>&nbsp;</td>";
            //							}
            //							HTML+="</tr><tr>";
            //						}
            //						HTML+="</tr></table></html>";
            #endregion

            ResultTable data = GetResultTable(tabResult, nbLineInTabResult, groupMediaTotalIndex, subGroupMediaTotalIndex, mediaIndex, mediaListForLabelSearch, nbUnivers);

            #region Strength and prospects filters
            if (_result == CompetitorMarketShare.FORCES)
                FilterTable(data, true, subGroupMediaTotalIndex);
            if (_result == CompetitorMarketShare.POTENTIELS)
                FilterTable(data, false, subGroupMediaTotalIndex);
            #endregion

            return (data);

        }
        #endregion

        #region Synthesis
        /// <summary>
        /// Get table with synthesis about numbers of Commons, Exclusives and Missings products
        /// </summary>
        /// <returns>Result Table</returns>
        public ResultTable GetSynthesisData()
        {

            #region Variables
            int positionUnivers = 1;
            long nbLine = 8;
            Int64 advertiserLineIndex = 0;
            Int64 brandLineIndex = 0;
            Int64 productLineIndex = 0;
            Int64 sectorLineIndex = 0;
            Int64 subsectorLineIndex = 0;
            Int64 groupLineIndex = 0;
            Int64 agencyGroupLineIndex = 0;
            Int64 agencyLineIndex = 0;
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

            #endregion

            #region Formattage des dates
            string periodBeginning = GetDateBegin();
            string periodEnd = GetDateEnd();
            #endregion


            #region Chargement des données
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
            object[] parameters = new object[1];
            parameters[0] = _session;
            IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            dt = presentAbsentDAL.GetSynthesisData();
            //dt = CompetitorDataAccess.GetGenericSynthesisData(webSession, vehicleName);
            #endregion

            #region Identifiant du texte des unités
            Int64 unitId = _session.GetUnitLabelId();
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            #endregion

            #region Création des headers
            nbLine = 5;
            if (_session.CustomerLogin.GetFlag((long)CstDB.Flags.ID_MARQUE) != null) nbLine++;
            if (_session.CustomerLogin.GetFlag((long)CstDB.Flags.ID_MEDIA_AGENCY) != null) nbLine += 2;

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
            Int64 nbCol = resultTable.ColumnsNumber - 2;
            #endregion

            #region Initialisation des lignes
            Int64 levelLabelColIndex = resultTable.GetHeadersIndexInResultTable(LEVEL_HEADER_ID.ToString());
            advertiserLineIndex = resultTable.AddNewLine(LineType.level1);
            resultTable[advertiserLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1146, _session.SiteLanguage));
            if (_session.CustomerLogin.GetFlag((long)CstDB.Flags.ID_MARQUE) != null)
            {
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
            if (_session.CustomerLogin.GetFlag((long)CstDB.Flags.ID_MEDIA_AGENCY) != null)
            {
                agencyGroupLineIndex = resultTable.AddNewLine(LineType.level1);
                resultTable[agencyGroupLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1850, _session.SiteLanguage));
                agencyLineIndex = resultTable.AddNewLine(LineType.level1);
                resultTable[agencyLineIndex, levelLabelColIndex] = new CellLabel(GestionWeb.GetWebWord(1851, _session.SiteLanguage));
            }

            Int64 presentNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 absentNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 exclusiveNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            #region Initialisation des Nombres
            for (int i = 0; i < nbLine; i++)
            {
                resultTable[i, presentNumberColumnIndex] = new CellNumber(0.0);
                resultTable[i, absentNumberColumnIndex] = new CellNumber(0.0);
                resultTable[i, exclusiveNumberColumnIndex] = new CellNumber(0.0);
            }
            for (long i = 0; i < nbLine; i++)
            {
                for (long j = presentNumberColumnIndex + 1; j < absentNumberColumnIndex; j++)
                {
                    resultTable[i, j] = cellUnitFactory.Get(0.0);
                }
                for (long j = absentNumberColumnIndex + 1; j < exclusiveNumberColumnIndex; j++)
                {
                    resultTable[i, j] = cellUnitFactory.Get(0.0);
                }
                for (long j = exclusiveNumberColumnIndex + 1; j <= nbCol; j++)
                {
                    resultTable[i, j] = cellUnitFactory.Get(0.0);
                }
            }
            #endregion

            #endregion


            if (dt != null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0)
            {

                #region Sélection de Médias
                //Liste des supports de référence
                if (_session.CompetitorUniversMedia[positionUnivers] != null)
                {
                    mediaList = _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess);
                    if (mediaList != null && mediaList.Length > 0)
                    {
                        referenceUniversMedia = new List<string>(mediaList.Split(','));
                        positionUnivers++;
                    }
                    mediaList = "";
                }
                //Liste des supports concurrents
                if (referenceUniversMedia != null && referenceUniversMedia.Count > 0)
                {
                    while (_session.CompetitorUniversMedia[positionUnivers] != null)
                    {
                        mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
                        positionUnivers++;
                    }
                    if (mediaList.Length > 0) competitorUniversMedia = new List<string>(mediaList.Substring(0, mediaList.Length - 1).Split(','));
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
                            GetProductActivity(resultTable, dt, advertiserLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia);
                            advertisers.Add(currentRow["id_advertiser"].ToString());
                        }
                        if (_session.CustomerLogin.GetFlag((long)CstDB.Flags.ID_MARQUE) != null)
                        {
                            //Activité publicitaire marques
                            if (currentRow["id_brand"] != null && currentRow["id_brand"] != System.DBNull.Value && !brands.Contains(currentRow["id_brand"].ToString()))
                            {
                                expression = string.Format("id_brand={0}", currentRow["id_brand"]);
                                GetProductActivity(resultTable, dt, brandLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia);
                                brands.Add(currentRow["id_brand"].ToString());
                            }
                        }

                        //Activité publicitaire produits
                        if (currentRow["id_product"] != null && currentRow["id_product"] != System.DBNull.Value && !products.Contains(currentRow["id_product"].ToString()))
                        {
                            expression = string.Format("id_product={0}", currentRow["id_product"]);
                            GetProductActivity(resultTable, dt, productLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia);
                            products.Add(currentRow["id_product"].ToString());
                        }

                        //Activité publicitaire Famille
                        if (currentRow["id_sector"] != null && currentRow["id_sector"] != System.DBNull.Value && !sectors.Contains(currentRow["id_sector"].ToString()))
                        {
                            expression = string.Format("id_sector={0}", currentRow["id_sector"]);
                            GetProductActivity(resultTable, dt, sectorLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia);
                            sectors.Add(currentRow["id_sector"].ToString());
                        }
                        //Activité publicitaire Classe
                        if (currentRow["id_subsector"] != null && currentRow["id_subsector"] != System.DBNull.Value && !subsectors.Contains(currentRow["id_subsector"].ToString()))
                        {
                            expression = string.Format("id_subsector={0}", currentRow["id_subsector"]);
                            GetProductActivity(resultTable, dt, subsectorLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia);
                            subsectors.Add(currentRow["id_subsector"].ToString());
                        }
                        //Activité publicitaire Groupes
                        if (currentRow["id_group_"] != null && currentRow["id_group_"] != System.DBNull.Value && !groups.Contains(currentRow["id_group_"].ToString()))
                        {
                            expression = string.Format("id_group_={0}", currentRow["id_group_"]);
                            GetProductActivity(resultTable, dt, groupLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia);
                            groups.Add(currentRow["id_group_"].ToString());
                        }


                        if (_session.CustomerLogin.GetFlag((long)CstDB.Flags.ID_MEDIA_AGENCY) != null)
                        {
                            //activité publicitaire Groupes d'agences
                            if (currentRow["ID_GROUP_ADVERTISING_AGENCY"] != null && currentRow["ID_GROUP_ADVERTISING_AGENCY"] != System.DBNull.Value && !agencyGroups.Contains(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString()))
                            {
                                expression = string.Format("ID_GROUP_ADVERTISING_AGENCY={0}", currentRow["ID_GROUP_ADVERTISING_AGENCY"]);
                                GetProductActivity(resultTable, dt, agencyGroupLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia);
                                agencyGroups.Add(currentRow["ID_GROUP_ADVERTISING_AGENCY"].ToString());
                            }

                            //activité publicitaire agence
                            if (currentRow["ID_ADVERTISING_AGENCY"] != null && currentRow["ID_ADVERTISING_AGENCY"] != System.DBNull.Value && !agency.Contains(currentRow["ID_ADVERTISING_AGENCY"].ToString()))
                            {
                                expression = string.Format("ID_ADVERTISING_AGENCY={0}", currentRow["ID_ADVERTISING_AGENCY"]);
                                GetProductActivity(resultTable, dt, agencyLineIndex, expression, sort, referenceUniversMedia, competitorUniversMedia);
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
        protected void FilterTable(ResultTable data, bool computeStrenghs, SelectionSubGroup[] subGroupMediaTotalIndex)
        {

            if (data == null)
            {
                return;
            }

            #region Variables
            long i;
            #endregion

            #region Indexes de comparaison
            long comparaisonIndexInTabResult;
            if (data.HeadersIndexInResultTable.ContainsKey((START_ID_GROUP + 1) + "-" + SUB_TOTAL_HEADER_ID))
            {
                comparaisonIndexInTabResult = data.GetHeadersIndexInResultTable((START_ID_GROUP + 1) + "-" + SUB_TOTAL_HEADER_ID);
            }
            else
            {
                comparaisonIndexInTabResult = data.GetHeadersIndexInResultTable((START_ID_GROUP + 1) + "-" + subGroupMediaTotalIndex[2].DataBaseId.ToString());
            }
            long levelLabelColIndex = data.GetHeadersIndexInResultTable(LEVEL_HEADER_ID.ToString());
            #endregion

            #region Détermine le niveau de détail Produit
            int NbLevels = _session.GenericProductDetailLevel.GetNbLevels;
            #endregion

            #region Traitement des données
            try
            {
                // Supprime les lignes qui ont un total support de références média inférieur
                // au total univers des supports de référence.
                long[] nbLevelToShow ={ 0, 0, 0, 0 };
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
						if (!(data[i, 0] is LineStart && ((LineStart)data[i, 0]).LineType == LineType.nbParution)) {
							for (int j = curLevel.Level + 1; j < nbLevelToShow.Length; j++) {
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
        protected object[,] GetPreformatedTable(SelectionGroup[] groupMediaTotalIndex, SelectionSubGroup[] subGroupMediaTotalIndex, Dictionary<Int64, GroupItemForTableResult> mediaIndex, ref int nbCol, ref long nbLineInNewTable, ref int nbUnivers, ref string mediaListForLabelSearch)
        {
			
			#region Variables
			double unit;
			Int64 idMedia;
			long oldIdL1=-1;
			long oldIdL2=-1;
			long oldIdL3=-1;
			Int64 currentLine=-1;
			int k;
			bool changeLine=false;
			#endregion
			
			#region Formattage des dates 
			string periodBeginning = GetDateBegin();
            string periodEnd = GetDateEnd();
			#endregion

			#region Chargement des données à partir de la base	
			DataSet ds=null;
            DataSet dsMedia = null;

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
            object[] parameters = new object[1];
            parameters[0] = _session;
            IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            ds = presentAbsentDAL.GetData();
            dsMedia = presentAbsentDAL.GetMediaDetails();
            //ds = CompetitorDataAccess.GetGenericData(session, vehicleName);
            //dsMedia = CompetitorDataAccess.GetMediaColumnDetailLevelList(session);
			
			DataTable dt=ds.Tables[0];
            DataTable dtMedia = dsMedia.Tables[0];

            if (dt.Rows.Count == 0) {
                return null;
            }

            #region Tableaux d'index
            InitIndexAndValues(groupMediaTotalIndex, subGroupMediaTotalIndex, ref nbUnivers, mediaIndex, ref mediaListForLabelSearch, ref nbCol, dtMedia);
            #endregion

			#endregion
			
			#region Déclaration du tableau de résultat
			long nbline=dt.Rows.Count;
			object[,] tabResult=new object[nbCol,dt.Rows.Count];
			#endregion

			#region Tableau de résultat
			foreach(DataRow currentRow in dt.Rows){
                if (_session.GenericProductDetailLevel.GetIdValue(currentRow, 1) > 0 && _session.GenericProductDetailLevel.GetIdValue(currentRow, 1) != oldIdL1) changeLine = true;
                if (!changeLine && _session.GenericProductDetailLevel.GetIdValue(currentRow, 2) > 0 && _session.GenericProductDetailLevel.GetIdValue(currentRow, 2) != oldIdL2) changeLine = true;
                if (!changeLine && _session.GenericProductDetailLevel.GetIdValue(currentRow, 3) > 0 && _session.GenericProductDetailLevel.GetIdValue(currentRow, 3) != oldIdL3) changeLine = true;

				#region On change de ligne
				if(changeLine){
					currentLine++;
					// Ecriture de L1 ?
                    if (_session.GenericProductDetailLevel.GetIdValue(currentRow, 1) > 0)
                    {
                        oldIdL1 = _session.GenericProductDetailLevel.GetIdValue(currentRow, 1);
						tabResult[IDL1_INDEX,currentLine]=oldIdL1;
                        tabResult[LABELL1_INDEX, currentLine] = _session.GenericProductDetailLevel.GetLabelValue(currentRow, 1);
					}
					// Ecriture de L2 ?
                    if (_session.GenericProductDetailLevel.GetIdValue(currentRow, 2) > 0)
                    {
                        oldIdL2 = _session.GenericProductDetailLevel.GetIdValue(currentRow, 2);
						tabResult[IDL2_INDEX,currentLine]=oldIdL2;
                        tabResult[LABELL2_INDEX, currentLine] = _session.GenericProductDetailLevel.GetLabelValue(currentRow, 2);
					}
					// Ecriture de L3 ?
                    if (_session.GenericProductDetailLevel.GetIdValue(currentRow, 3) > 0)
                    {
                        oldIdL3 = _session.GenericProductDetailLevel.GetIdValue(currentRow, 3);
						tabResult[IDL3_INDEX,currentLine]=oldIdL3;
                        tabResult[LABELL3_INDEX, currentLine] = _session.GenericProductDetailLevel.GetLabelValue(currentRow, 3);
					}
					// Totaux, sous Totaux et médias à 0
					for(k=TOTAL_INDEX;k<nbCol;k++){
						tabResult[k,currentLine]=(double) 0.0;
					}
					
					try{
						if(currentRow["id_address"]!=null)tabResult[ADDRESS_COLUMN_INDEX,currentLine]=Int64.Parse(currentRow["id_address"].ToString());
					}catch(Exception){
					
					}

					changeLine=false;
				}
				#endregion


				
				unit=double.Parse(currentRow["unit"].ToString());
                idMedia = (Int64)currentRow["id_media"];
				// Ecriture du résultat du média
                tabResult[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].IndexInResultTable, currentLine] = (double)tabResult[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].IndexInResultTable, currentLine] + unit;
                
                // Ecriture du résultat du sous total (somme)
                if (groupMediaTotalIndex[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].ParentId].Count > 1)
                {
                    tabResult[groupMediaTotalIndex[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].ParentId].IndexInResultTable, currentLine] = (double)tabResult[groupMediaTotalIndex[subGroupMediaTotalIndex[mediaIndex[idMedia].GroupNumber].ParentId].IndexInResultTable, currentLine] + unit;
                }

                // Ecriture du résultat du total (somme)
				tabResult[TOTAL_INDEX,currentLine]=(double)tabResult[TOTAL_INDEX,currentLine]+unit;
			}
			#endregion
			
			#region Debug: Voir le tableau 
//			int i,j;
//			string HTML="<html><table><tr>";
//			for(i=0;i<=currentLine;i++){
//				for(j=0;j<nbCol;j++){
//					if(tabResult[j,i]!=null)HTML+="<td>"+tabResult[j,i].ToString()+"</td>";
//					else HTML+="<td>&nbsp;</td>";
//				}
//				HTML+="</tr><tr>";
//			}
//			HTML+="</tr></table></html>";
			#endregion

			nbLineInNewTable=currentLine+1;
			return(tabResult);
		}
		#endregion

        #region Formatage des dates
		/// <summary>
		/// Get Period Beginning
		/// </summary>
		/// <returns>Period Beginning</returns>
		protected string GetDateBegin(){
            return (_session.PeriodBeginningDate);
		}

		/// <summary>
		/// Get Period End
		/// </summary>
		/// <returns>Period End</returns>
		protected string GetDateEnd(){
            return (_session.PeriodEndDate);
		}
		#endregion

        #region Initialisation des indexes
        /// <summary>
        /// Initialise indexes tables about groups and media selections
        /// </summary>
        /// <param name="groupMediaTotalIndex">(out) Group selection indexes table</param>
        /// <param name="subGroupMediaTotalIndex">Liste des index des sous groupes de sélection</param>
        /// <param name="nbUnivers">(out)Nombre d'univers</param>
        /// <param name="mediaIndex">(out Tableau d'indexes des médias</param>
        /// <param name="mediaListForLabelSearch">(out)Liste des codes des médias</param>
        /// <param name="maxIndex">(out)Index des colonnes maximum</param>
        /// <param name="dtMedia">Liste des média avec le niveau de détail colonne correspondant</param>
        protected void InitIndexAndValues(SelectionGroup[] groupMediaTotalIndex, SelectionSubGroup[] subGroupMediaTotalIndex, ref int nbUnivers, Dictionary<Int64, GroupItemForTableResult> mediaIndex, ref string mediaListForLabelSearch, ref int maxIndex, DataTable dtMedia)
        {

            #region Variables
            string tmp = "";
            int positionUnivers = 1;
            int positionSubGroup = 2;
            int subGroupCount = 0;
            Int64[] mediaList;
            Dictionary<Int64, int> mediaSubGroupId = new Dictionary<Int64, int>();
            List<string> columnDetailLevelList;
            #endregion

            #region Initialisation des variables
            maxIndex = FIRST_MEDIA_INDEX;
            #endregion

            // On suppose que les supports sont triés en order croissant par sous groupe
            while (_session.CompetitorUniversMedia[positionUnivers] != null)
            {
                // Chargement de la liste de support (média)
                tmp = _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess);
                mediaList = Array.ConvertAll<string, Int64>(tmp.Split(','), (Converter<string,long>)delegate (string s) { return Convert.ToInt64(s); });

                columnDetailLevelList = new List<string>();

                // Chargement de la liste du niveau de détail colonne
                foreach (Int64 media in mediaList)
                {
                    foreach (DataRow row in dtMedia.Rows)
                    {
                        if (media == Convert.ToInt64(row["id_media"]))
                        {
                            if (!columnDetailLevelList.Contains(row["columnDetailLevel"].ToString()))
                            {
                                columnDetailLevelList.Add(row["columnDetailLevel"].ToString());
                                subGroupMediaTotalIndex[positionSubGroup] = new SelectionSubGroup(positionSubGroup);
                                subGroupMediaTotalIndex[positionSubGroup].DataBaseId = int.Parse(row["columnDetailLevel"].ToString());
                                subGroupMediaTotalIndex[positionSubGroup].ParentId = positionUnivers;
                                subGroupMediaTotalIndex[positionSubGroup].SetItemsNumber = 0;
                                subGroupMediaTotalIndex[positionSubGroup].IndexInResultTable = 0;
                                mediaSubGroupId[media] = positionSubGroup;
                                positionSubGroup++;
                                subGroupCount++;
                                mediaListForLabelSearch += row["columnDetailLevel"].ToString() + ",";
                            }
                            else
                            {
                                foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex)
                                    if (subGroup != null)
                                    {
                                        if (subGroup.DataBaseId == int.Parse(row["columnDetailLevel"].ToString()))
                                        {
                                            if (subGroup.Count == 0)
                                                subGroup.SetItemsNumber = 2;
                                            else
                                                subGroup.SetItemsNumber = subGroup.Count + 1;
                                            mediaSubGroupId[media] = subGroup.Id;
                                        }
                                    }
                            }
                        }
                    }
                }

                // Définition du groupe
                groupMediaTotalIndex[positionUnivers] = new SelectionGroup(positionUnivers);

                // Le groupe contient plus de 1 éléments
                if (subGroupCount > 1)
                {
                    groupMediaTotalIndex[positionUnivers].IndexInResultTable = maxIndex;
                    groupMediaTotalIndex[positionUnivers].SetItemsNumber = subGroupCount;
                    maxIndex++;
                    //nbSubTotal++;
                }
                else
                {
                    groupMediaTotalIndex[positionUnivers].IndexInResultTable = maxIndex;
                    groupMediaTotalIndex[positionUnivers].SetItemsNumber = 0;
                }

                // Pour les sous Groupes
                foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex)
                {
                    if (subGroup != null)
                    {
                        if (subGroup.IndexInResultTable == 0)
                        {
                            subGroup.IndexInResultTable = maxIndex;
                            maxIndex++;
                        }
                    }
                }

                // Indexes des média (support)
                foreach (Int64 currentMedia in mediaList)
                {
                    mediaIndex[currentMedia] = new GroupItemForTableResult(currentMedia, mediaSubGroupId[currentMedia], maxIndex);
                }

                positionUnivers++;
                subGroupCount = 0;
            }

            nbUnivers = positionUnivers--;
            mediaListForLabelSearch = mediaListForLabelSearch.Substring(0, mediaListForLabelSearch.Length - 1);
        }
        #endregion

        #region Formattage d'un tableau de résultat
        /// <summary>
        /// Formattage d'un tableau de résultat à partir d'un tableau de données
        /// </summary>
        /// <param name="session">Session du client</param>
        /// <param name="tabData">Table de données</param>
        /// <param name="nbLineInTabData">Nombre de ligne dans le tableau</param>
        /// <param name="groupMediaTotalIndex">Liste des groupes de sélection</param>
        /// <param name="subGroupMediaTotalIndex">Liste des sous groupes de sélection</param>
        /// <param name="mediaIndex">Liste des Média</param>
        /// <param name="mediaListForLabelSearch">Liste des médias</param>
        /// <param name="nbUnivers">Nombre d'univers</param>
        /// <returns>Tableau de résultat</returns>
        protected ResultTable GetResultTable(object[,] tabData, long nbLineInTabData, SelectionGroup[] groupMediaTotalIndex, SelectionSubGroup[] subGroupMediaTotalIndex, Dictionary<Int64, GroupItemForTableResult> mediaIndex, string mediaListForLabelSearch, int nbUnivers)
        {

            #region Variables
            long decal = 0;
            string[] mediaList;
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            long currentLine;
            long currentLineInTabResult;
            long k;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Aucune données
            if (nbLineInTabData == 0)
            {
                return null;
            }
            #endregion

            #region Calcul des PDM ?
            bool computePDM = (_session.Percentage) ? true : false;
            #endregion

            #region Obtention du vehicle
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new PresentAbsentException("Media Selection is not valid"));
            CstDBClassif.Vehicles.names vehicle = (CstDBClassif.Vehicles.names)int.Parse(vehicleSelection);
            #endregion

            #region Headers
            // Ajout de la colonne Produit
            Headers headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), LEVEL_HEADER_ID));
            long startDataColIndex = 1;
            long startDataColIndexInit = 1;


            // Ajout Création ?
            bool showCreative = false;
            //A vérifier Création où version
            if (_session.CustomerLogin.GetFlag(CstDB.Flags.ID_SLOGAN_ACCESS_FLAG) != null &&
                (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)))
            {
                headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(1994, _session.SiteLanguage), CREATIVE_HEADER_ID));
                showCreative = true;
                startDataColIndex++;
                startDataColIndexInit++;
            }

            // Ajout Insertions ?
            bool showInsertions = false;
            if ((_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)))
            {
                headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(2245, _session.SiteLanguage), INSERTION_HEADER_ID));
                showInsertions = true;
                startDataColIndex++;
                startDataColIndexInit++;
            }

            // Ajout plan media ?
            bool showMediaSchedule = false;
            if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.holdingCompany) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.sector) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subSector) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.group)
                )
            {
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(150, _session.SiteLanguage), MEDIA_SCHEDULE_HEADER_ID));
                showMediaSchedule = true;
                startDataColIndex++;
                startDataColIndexInit++;
            }


            //Colonne total s'il ya plusieurs groupes:
            mediaList = mediaListForLabelSearch.Split(',');
            bool showTotal = false;
            if (_session.CompetitorUniversMedia.Count > 1 || mediaList.Length > 1)
            {
                startDataColIndexInit++;
                showTotal = true;
                headers.Root.Add(new Header(true, GestionWeb.GetWebWord(805, _session.SiteLanguage), TOTAL_HEADER_ID));
            }
            // Chargement des libellés de colonnes
            DALClassif.MediaBranch.PartialMediaListDataAccess mediaLabelList = null;
            DALClassif.MediaBranch.PartialCategoryListDataAccess categoryLabelList = null;
            DALClassif.MediaBranch.PartialMediaSellerListDataAccess mediaSellerLabelList = null;
            DALClassif.MediaBranch.PartialTitleListDataAccess titleLabelList = null;
            DALClassif.MediaBranch.PartialInterestCenterListDataAccess interestCenterLabelList = null;

            switch (columnDetailLevel.Id)
            {

                case DetailLevelItemInformation.Levels.media:
                    mediaLabelList = new DALClassif.MediaBranch.PartialMediaListDataAccess(mediaListForLabelSearch, _session.SiteLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.category:
                    categoryLabelList = new DALClassif.MediaBranch.PartialCategoryListDataAccess(mediaListForLabelSearch, _session.SiteLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.mediaSeller:
                    mediaSellerLabelList = new DALClassif.MediaBranch.PartialMediaSellerListDataAccess(mediaListForLabelSearch, _session.SiteLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.title:
                    titleLabelList = new DALClassif.MediaBranch.PartialTitleListDataAccess(mediaListForLabelSearch, _session.SiteLanguage, _session.Source);
                    break;
                case DetailLevelItemInformation.Levels.interestCenter:
                    interestCenterLabelList = new DALClassif.MediaBranch.PartialInterestCenterListDataAccess(mediaListForLabelSearch, _session.SiteLanguage, _session.Source);
                    break;

            }

            HeaderGroup headerGroupTmp = null;
			Dictionary<long, string> mediaKeyIndexResultTable = new Dictionary<long, string>();
            for (int m = 1; m < groupMediaTotalIndex.GetLength(0); m++)
            {
                if (groupMediaTotalIndex[m] != null)
                {
                    //Supports de référence ou concurents 1365
                    if (m == 1) headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(1365, _session.SiteLanguage), true, START_ID_GROUP + m);
                    else headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(1366, _session.SiteLanguage) + " " + (m - 1).ToString(), true, START_ID_GROUP + m);
                    if (groupMediaTotalIndex[m].Count > 1 && _session.CompetitorUniversMedia.Count > 1) headerGroupTmp.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), SUB_TOTAL_HEADER_ID);

                    foreach (SelectionSubGroup subGroup in subGroupMediaTotalIndex)
                    {
                        if (subGroup != null)
                        {
                            if (subGroup.ParentId == m)
                                switch (columnDetailLevel.Id)
                                {

                                    case DetailLevelItemInformation.Levels.media:
                                        headerGroupTmp.Add(new Header(true, mediaLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
										mediaKeyIndexResultTable.Add(subGroup.DataBaseId, (START_ID_GROUP + m) + "-" + subGroup.DataBaseId.ToString());
										break;
                                    case DetailLevelItemInformation.Levels.category:
                                        headerGroupTmp.Add(new Header(true, categoryLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;
                                    case DetailLevelItemInformation.Levels.mediaSeller:
                                        headerGroupTmp.Add(new Header(true, mediaSellerLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;
                                    case DetailLevelItemInformation.Levels.title:
                                        headerGroupTmp.Add(new Header(true, titleLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;
                                    case DetailLevelItemInformation.Levels.interestCenter:
                                        headerGroupTmp.Add(new Header(true, interestCenterLabelList[subGroup.DataBaseId], subGroup.DataBaseId));
                                        break;

                                }
                        }
                    }

                    headers.Root.Add(headerGroupTmp);
                }
            }
            #endregion

            #region Déclaration du tableau de résultat

            long nbLine = GetNbLineFromPreformatedTableToResultTable(tabData) + 1;
			#region Add Line for  Nb parution data
			Dictionary<string, double> resNbParution = null;
			if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.media && (CstDBClassif.Vehicles.names.press == vehicle || CstDBClassif.Vehicles.names.internationalPress == vehicle)) {
				resNbParution = GetNbParutionsByMedia();
				if (resNbParution != null && resNbParution.Count>0)
					nbLine = nbLine + 1;
			}
			#endregion
            ResultTable resultTable = new ResultTable(nbLine, headers);
            long nbCol = resultTable.ColumnsNumber - 2;
            long totalColIndex = -1;
            if (showTotal) totalColIndex = resultTable.GetHeadersIndexInResultTable(TOTAL_HEADER_ID.ToString());
            long levelLabelColIndex = resultTable.GetHeadersIndexInResultTable(LEVEL_HEADER_ID.ToString());
            long mediaScheduleColIndex = resultTable.GetHeadersIndexInResultTable(MEDIA_SCHEDULE_HEADER_ID.ToString());
            long creativeColIndex = resultTable.GetHeadersIndexInResultTable(CREATIVE_HEADER_ID.ToString());
            long insertionsColIndex = resultTable.GetHeadersIndexInResultTable(INSERTION_HEADER_ID.ToString());
            #endregion

            #region Sélection de l'unité
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            #endregion

            #region Total
            long nbColInTabData = tabData.GetLength(0);
            Dictionary<Int64, bool> subTotalWithOneMediaIndexes = GetSubTotalWithOneMediaIndexes(nbColInTabData, resultTable, groupMediaTotalIndex);
            startDataColIndex++;
            startDataColIndexInit++;
            currentLineInTabResult = resultTable.AddNewLine(LineType.total);
            //Libellé du total
            resultTable[currentLineInTabResult, levelLabelColIndex] = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), 0, currentLineInTabResult);
            CellLevel currentCellLevel0 = (CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
            if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevel0, _session, _session.GenericProductDetailLevel);
            if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevel0, _session, _session.GenericProductDetailLevel);
            if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevel0, _session);
            if (showTotal && !computePDM) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
            if (showTotal && computePDM) resultTable[currentLineInTabResult, totalColIndex] = new CellPDM(0.0, null);
            for (k = startDataColIndexInit; k <= nbCol; k++)
            {
                if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
            }
            #endregion

			#region Nombre parutions by media
			if (resNbParution != null && resNbParution.Count > 0) {				
				currentLineInTabResult = resultTable.AddNewLine(LineType.nbParution);
				//Libellé du total
				resultTable[currentLineInTabResult, levelLabelColIndex] = new CellLevel(-1, GestionWeb.GetWebWord(2460, _session.SiteLanguage), 0, currentLineInTabResult);
				CellLevel currentCellLevelParution = (CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
				if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevelParution, _session, _session.GenericProductDetailLevel);
				if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevelParution, _session, _session.GenericProductDetailLevel);
				if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevelParution, _session);
				if (showTotal && !computePDM) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
				if (showTotal && computePDM) resultTable[currentLineInTabResult, totalColIndex] = new CellPDM(0.0, null);
				for (k = startDataColIndexInit; k <= nbCol; k++) {
					resultTable[currentLineInTabResult, k] = new CellNumber(0.0);
				}

				//Insertion du nombre de parution 		
				foreach (KeyValuePair<string, double> kpv in resNbParution) {
					if (mediaKeyIndexResultTable.ContainsKey(long.Parse(kpv.Key)) && resultTable.HeadersIndexInResultTable.ContainsKey(mediaKeyIndexResultTable[long.Parse(kpv.Key)])) {
						TNS.FrameWork.WebResultUI.Header header = (TNS.FrameWork.WebResultUI.Header)resultTable.HeadersIndexInResultTable[mediaKeyIndexResultTable[long.Parse(kpv.Key)]];
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

            for (currentLine = 0; currentLine < nbLineInTabData; currentLine++)
            {

                #region On change de niveau L1
                if (tabData[IDL1_INDEX, currentLine] != null && (Int64)tabData[IDL1_INDEX, currentLine] != oldIdL1)
                {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level1);

                    #region Totaux et sous Totaux à 0 et media
                    if (showTotal && !computePDM) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
                    if (showTotal && computePDM) resultTable[currentLineInTabResult, totalColIndex] = new CellPDM(0.0, null);
                    for (k = startDataColIndexInit; k <= nbCol; k++)
                    {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    #endregion

                    oldIdL1 = (Int64)tabData[IDL1_INDEX, currentLine];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel(oldIdL1, (string)tabData[LABELL1_INDEX, currentLine], currentCellLevel0, 1, currentLineInTabResult, _session);
                    currentCellLevel1 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevel1, _session, _session.GenericProductDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevel1, _session, _session.GenericProductDetailLevel);
                    if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevel1, _session);
                    currentL1Index = currentLineInTabResult;
                    oldIdL2 = oldIdL3 = -1;

                    #region GAD
                    if (_session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 1)
                    {
                        if (tabData[ADDRESS_COLUMN_INDEX, currentLine] != null)
                        {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = (Int64)tabData[ADDRESS_COLUMN_INDEX, currentLine];
                        }
                    }
                    #endregion
                }
                #endregion

                #region On change de niveau L2
                if (tabData[IDL2_INDEX, currentLine] != null && (Int64)tabData[IDL2_INDEX, currentLine] != oldIdL2)
                {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level2);

                    #region Totaux et sous Totaux à 0 et media
                    if (showTotal && !computePDM) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
                    if (showTotal && computePDM) resultTable[currentLineInTabResult, totalColIndex] = new CellPDM(0.0, null);
                    for (k = startDataColIndexInit; k <= nbCol; k++)
                    {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    #endregion

                    oldIdL2 = (Int64)tabData[IDL2_INDEX, currentLine];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel(oldIdL2, (string)tabData[LABELL2_INDEX, currentLine], currentCellLevel1, 2, currentLineInTabResult, _session);
                    currentCellLevel2 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevel2, _session, _session.GenericProductDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevel2, _session, _session.GenericProductDetailLevel);
                    if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevel2, _session);
                    currentL2Index = currentLineInTabResult;
                    oldIdL3 = -1;

                    #region GAD
                    if (_session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 2)
                    {
                        if (tabData[ADDRESS_COLUMN_INDEX, currentLine] != null)
                        {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = (Int64)tabData[ADDRESS_COLUMN_INDEX, currentLine];
                        }
                    }
                    #endregion
                }
                #endregion

                #region On change de niveau L3
                if (tabData[IDL3_INDEX, currentLine] != null && (Int64)tabData[IDL3_INDEX, currentLine] != oldIdL3)
                {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level3);

                    #region Totaux et sous Totaux à 0 et media
                    if (showTotal && !computePDM) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
                    if (showTotal && computePDM) resultTable[currentLineInTabResult, totalColIndex] = new CellPDM(0.0, null);
                    for (k = startDataColIndexInit; k <= nbCol; k++)
                    {
                        if (computePDM) resultTable[currentLineInTabResult, k] = new CellPDM(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                        else resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    }
                    #endregion

                    oldIdL3 = (Int64)tabData[IDL3_INDEX, currentLine];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel(oldIdL3, (string)tabData[LABELL3_INDEX, currentLine], currentCellLevel2, 3, currentLineInTabResult, _session);
                    currentCellLevel3 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellOneLevelCreativesLink(currentCellLevel3, _session, _session.GenericProductDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellOneLevelInsertionsLink(currentCellLevel3, _session, _session.GenericProductDetailLevel);
                    if (showMediaSchedule) resultTable[currentLineInTabResult, mediaScheduleColIndex] = new CellMediaScheduleLink(currentCellLevel3, _session);
                    currentL3Index = currentLineInTabResult;

                    #region GAD
                    if (_session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 3)
                    {
                        if (tabData[ADDRESS_COLUMN_INDEX, currentLine] != null)
                        {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = (Int64)tabData[ADDRESS_COLUMN_INDEX, currentLine];
                        }
                    }
                    #endregion
                }
                #endregion

                // On copy la ligne et on l'ajoute aux totaux
                decal = 0;
                for (k = TOTAL_INDEX; k < nbColInTabData; k++)
                {
                    //On affiche pas la cellule si:
                    // Dans data c'est un sous total et que le groupe n'a qu'un seul éléments
                    if (!subTotalWithOneMediaIndexes.ContainsKey(k))
                        resultTable.AffectValueAndAddToHierarchy(levelLabelColIndex, currentLineInTabResult, startDataColIndex - decal + k - (long.Parse((TOTAL_INDEX).ToString())), (double)tabData[k, currentLine]);
                    else
                        decal++;
                }

            }
            #endregion

            return (resultTable);
        }
        #endregion

        #region Compute line numbers in result table from preformated data table
        /// <summary>
        /// Get the number of line in ResultTable from preformated data table
        /// </summary>
        /// <param name="tabData">Preformated Table</param>
        /// <returns>Result Table data line number</returns>
        protected long GetNbLineFromPreformatedTableToResultTable(object[,] tabData)
        {

            #region Variables
            long nbLine = 0;
            long k;
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            #endregion

            for (k = 0; k < tabData.GetLength(1); k++)
            {
                // Somme des L1
                if (tabData[IDL1_INDEX, k] != null && (Int64)tabData[IDL1_INDEX, k] != oldIdL1)
                {
                    oldIdL1 = (Int64)tabData[IDL1_INDEX, k];
                    oldIdL3 = oldIdL2 = -1;
                    nbLine++;
                }

                // Somme des L2
                if (tabData[IDL2_INDEX, k] != null && (Int64)tabData[IDL2_INDEX, k] != oldIdL2)
                {
                    oldIdL2 = (Int64)tabData[IDL2_INDEX, k];
                    oldIdL3 = -1;
                    nbLine++;
                }

                // Somme des L3
                if (tabData[IDL3_INDEX, k] != null && (Int64)tabData[IDL3_INDEX, k] != oldIdL3)
                {
                    oldIdL3 = (Int64)tabData[IDL3_INDEX, k];
                    nbLine++;
                }

            }
            return (nbLine);
        }
        #endregion

        #region List of indexes of columns with only one media
        /// <summary>
        /// List of indexes of columns with only one media
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="nbColInTabData">Columns number in data table</param>
        /// <param name="resultTable">Result Table</param>
        /// <param name="groupMediaTotalIndex">List of media groups</param>
        /// <returns>Indexes of columns with only one media</returns>
        protected Dictionary<Int64, bool> GetSubTotalWithOneMediaIndexes(long nbColInTabData, ResultTable resultTable, SelectionGroup[] groupMediaTotalIndex)
        {
            Dictionary<Int64, bool> subTotalWithOneMediaIndexes = new Dictionary<Int64, bool>();
            for (long k = TOTAL_INDEX; k < nbColInTabData; k++)
            {
                for (int m = 1; m < groupMediaTotalIndex.GetLength(0); m++)
                {
                    if (groupMediaTotalIndex[m] != null)
                    {
                        if (groupMediaTotalIndex[m].IndexInResultTable == k &&
                          (groupMediaTotalIndex[m].Count == 1 || _session.CompetitorUniversMedia.Count == 1))
                            subTotalWithOneMediaIndexes.Add(k, true);
                    }
                }
            }
            return (subTotalWithOneMediaIndexes);
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
        private void GetProductActivity(ResultTable tabResult, DataTable dt, long indexLineProduct, string filterExpression, string sort, List<string> referenceUniversMedia, List<string> competitorUniversMedia)
        {
            double unitReferenceMedia = 0;
            double unitCompetitorMedia = 0;
            Int64 presentNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 absentNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int64 exclusiveNumberColumnIndex = tabResult.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            Int64 presentReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);
            Int64 absentReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);
            Int64 exclusiveReferenceColumnIndex = tabResult.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);

            Int64 presentCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);
            Int64 absentCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);
            Int64 exclusiveCompetitorColumnIndex = tabResult.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);

            DataRow[] foundRows = dt.Select(filterExpression, sort);

            if (foundRows != null && !foundRows.Equals(System.DBNull.Value) && foundRows.Length > 0)
            {
                for (int i = 0; i < foundRows.Length; i++)
                {
                    //Unité supports de référence
                    if (foundRows[i]["id_media"] != null && foundRows[i]["id_media"] != System.DBNull.Value && referenceUniversMedia != null &&
                        foundRows[i]["unit"] != null && foundRows[i]["unit"] != System.DBNull.Value && referenceUniversMedia.Contains(foundRows[i]["id_media"].ToString()))
                    {
                        unitReferenceMedia += double.Parse(foundRows[i]["unit"].ToString());
                    }
                    //Unité supports concurrents
                    if (foundRows[i]["id_media"] != null && foundRows[i]["id_media"] != System.DBNull.Value && competitorUniversMedia != null &&
                        foundRows[i]["unit"] != null && foundRows[i]["unit"] != System.DBNull.Value && competitorUniversMedia.Contains(foundRows[i]["id_media"].ToString()))
                    {
                        unitCompetitorMedia += double.Parse(foundRows[i]["unit"].ToString());
                    }
                }
            }

            #region Communs
            if (unitReferenceMedia > 0 && unitCompetitorMedia > 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, presentNumberColumnIndex]).Value += 1;

                //supports de référence communs
                ((CellUnit)tabResult[indexLineProduct, presentReferenceColumnIndex]).Value += unitReferenceMedia;

                //supports concurrents communs
                ((CellUnit)tabResult[indexLineProduct, presentCompetitorColumnIndex]).Value += unitCompetitorMedia;

            }
            #endregion

            #region Absents
            if (unitReferenceMedia == 0 && unitCompetitorMedia > 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, absentNumberColumnIndex]).Value += 1;

                //supports concurrents Absents
                ((CellUnit)tabResult[indexLineProduct, absentCompetitorColumnIndex]).Value += unitCompetitorMedia;

            }
            #endregion

            #region Exclusifs
            if (unitReferenceMedia > 0 && unitCompetitorMedia == 0)
            {
                //Nombre 
                ((CellUnit)tabResult[indexLineProduct, exclusiveNumberColumnIndex]).Value += 1;

                //supports de référence exclusifs
                ((CellUnit)tabResult[indexLineProduct, exclusiveReferenceColumnIndex]).Value += unitReferenceMedia;
            }
            #endregion

        }
        #endregion

		#region GetNbParutionsByMedia
		/// <summary>
		/// Get Number of parution by media
		/// </summary>
		/// <param name="webSession"> Client Session</param>
		/// <returns>Number of parution by media data</returns>
		protected  Dictionary<string, double> GetNbParutionsByMedia() {

			#region Variables
			Dictionary<string, double> res = new Dictionary<string, double>();
			double nbParutionsCounter = 0;
			Int64 oldColumnDetailLevel = -1;
			bool start = true;
			string oldKey = "";
			#endregion		

			#region Chargement des données à partir de la base
			DataSet ds;
			try {
				if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
				object[] parameters = new object[1];
				parameters[0] = _session;
				IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
				ds = presentAbsentDAL.GetNbParutionData();
				
			}
			catch (System.Exception err) {
				throw (new PresentAbsentException("Impossible de charger les données pour le nombre de parution", err));
			}
			DataTable dt = ds.Tables[0];
			#endregion

			if (dt != null && dt.Rows.Count > 0) {
				foreach (DataRow dr in dt.Rows) {
					if (!oldKey.Equals(dr["id_media"].ToString()) && !start) {
						res.Add(oldKey, nbParutionsCounter);
						nbParutionsCounter = 0;
					}
					nbParutionsCounter += double.Parse(dr["NbParution"].ToString());
					start = false;
					oldKey = dr["id_media"].ToString();
				}
				res.Add(oldKey, nbParutionsCounter);
			}

			return res;

		}

		#endregion

        #endregion
    }
}
