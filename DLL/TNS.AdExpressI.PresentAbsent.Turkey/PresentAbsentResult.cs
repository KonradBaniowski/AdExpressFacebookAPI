using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.PresentAbsent.DAL;
using TNS.Classification.Universe;
using TNS.FrameWork.WebResultUI;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.PresentAbsent.Turkey
{
    public class PresentAbsentResult : PresentAbsent.PresentAbsentResult
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public PresentAbsentResult(WebSession session) : base(session) { }
        #endregion

        #region Synthesis
        /// <summary>
        /// Get table with synthesis about numbers of Commons, Exclusives and Missings products
        /// </summary>
        /// <returns>Result Table</returns>
		public override ResultTable GetSynthesisData()
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
            IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName,
                _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
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


                     
                    }

                    #endregion

                }
                else return null;
            }
            else return null;


            return resultTable;
        }
        #endregion

        /// <summary>
        /// Init default values such as levels, Adresses...
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="row">Data container</param>
        /// <param name="level">Current level</param>
        /// <param name="parent">Parent level</param>
        /// <returns>Index of current line</returns>
        protected override Int32 InitLine(ResultTable tab, DataRow row, Int32 level, CellLevel parent)
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
           
            return cLine;

        }
    }
}
