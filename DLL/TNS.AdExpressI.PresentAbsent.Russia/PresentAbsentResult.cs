#region Information
/*
 * Author : D Mussuma
 * Creation : 13/03/2009
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
using System.Text;
using System.Windows.Forms;

using DALClassif = TNS.AdExpress.DataAccess.Classification;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Functions;
using Navigation = TNS.AdExpress.Domain.Web.Navigation;

using TNS.AdExpress;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpressI.PresentAbsent.DAL;
using TNS.AdExpressI.PresentAbsent.Exceptions;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Collections;
using TNS.AdExpressI.PresentAbsent;

using TNS.AdExpressI.Classification.DAL;
using System.Collections;
using TNS.AdExpress.Constantes;
using TNS.AdExpress.Domain.Web;
#endregion


namespace TNS.AdExpressI.PresentAbsent.Russia {
	/// <summary>
	/// Russia Present/Absent reports
	/// </summary>
	public class PresentAbsentResult : PresentAbsent.PresentAbsentResult
    {
        #region constantes
        /// <summary>
        ///Common result database IdID
        /// </summary>
        protected const long COMMON_RESULT_DB_ID = 1;
        /// <summary>
        ///Common result database IdID
        /// </summary>
        protected const long ABSENT_RESULT_DB_ID = 2;
        /// <summary>
        ///Common result database IdID
        /// </summary>
        protected const long EXCLUSIVE_RESULT_DB_ID = 3;
        #endregion

        #region Constructor
        /// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		public PresentAbsentResult(WebSession session)
			: base(session) {
		}
		#endregion 
       
        #region Get Data
        protected override ResultTable GetData()
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
            nbLine = tabData.LinesNumber;
                
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
           
            return tabResult;
        }
        #endregion
		
        #region Cross table
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
        protected override ResultTable GetGrossTable(out Dictionary<Int64, HeaderBase> universesSubTotal, out Dictionary<string, HeaderBase> elementsHeader, out Dictionary<string, HeaderBase> elementsSubTotal)
        {

            #region Load data from data layer
            DataTable dtTotal = null, dtLevel1 = null, dtLevel2 = null, dtLevel3 = null;
            DataSet dsMedia = null, ds = null;
            Dictionary<Int64, List<int>> level1ChildsIndex = null;
            Dictionary<string, List<int>> level2ChildsIndex = null;
            DataRow rowL2 = null, rowL3 = null;
            int i = 0;
            bool isNewL1 = false, isNewL2 = false;
            long idL2 = long.MinValue, oldIdL2 = long.MinValue, idL3 = long.MinValue, oldIdL3 = long.MinValue, tempId = long.MinValue, tempIdL2 = long.MinValue;
            int levelNb = _session.GenericProductDetailLevel.GetNbLevels;
            List<int> tempList = null;
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
            object[] parameters = new object[1];
            parameters[0] = _session;
            IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            ds = presentAbsentDAL.GetData();

            if (ds == null || ds.Tables.Count == 0 || ds.Tables["total"].Rows.Count == 0)
            {
                universesSubTotal = null;
                elementsHeader = null;
                elementsSubTotal = null;
                return null;
            }

            //Get total data
            dtTotal = ds.Tables["total"];

            //Get level 1 data
            if (levelNb > 0) dtLevel1 = ds.Tables["level1"];



            //Get level 2 data
            if (levelNb > 1)
            {
                dtLevel2 = ds.Tables["level2"];

                //Build Level 2 Indexes
                if (dtLevel2 != null && dtLevel2.Rows.Count > 0)
                {
                    level1ChildsIndex = new Dictionary<long, List<int>>();
                    for (int rowIndex = 0; rowIndex < dtLevel2.Rows.Count; rowIndex++)
                    {
                        tempId = _session.GenericProductDetailLevel.GetIdValue(dtLevel2.Rows[rowIndex], 1);
                        if (level1ChildsIndex.ContainsKey(tempId))
                        {
                            level1ChildsIndex[tempId].Add(rowIndex);
                        }
                        else
                        {
                            tempList = new List<int>();
                            tempList.Add(rowIndex);
                            level1ChildsIndex.Add(tempId, tempList);
                        }
                    }
                }
            }

            //Get level 3 data
            if (levelNb > 2)
            {
                dtLevel3 = ds.Tables["level3"];
                if (dtLevel3 != null && dtLevel3.Rows.Count > 0)
                {
                    level2ChildsIndex = new Dictionary<string, List<int>>();
                    for (int rowIndex = 0; rowIndex < dtLevel3.Rows.Count; rowIndex++)
                    {
                        tempId = _session.GenericProductDetailLevel.GetIdValue(dtLevel3.Rows[rowIndex], 1);
                        tempIdL2 = _session.GenericProductDetailLevel.GetIdValue(dtLevel3.Rows[rowIndex], 2);
                        string keyL3 = tempId.ToString() + "-" + tempIdL2.ToString();
                        if (level2ChildsIndex.ContainsKey(keyL3))
                            level2ChildsIndex[keyL3].Add(rowIndex);
                        else
                        {
                            tempList = new List<int>();
                            tempList.Add(rowIndex);
                            level2ChildsIndex.Add(keyL3, tempList);
                        }
                    }
                }
            }

            dsMedia = presentAbsentDAL.GetColumnDetails();

            DataTable dtMedia = dsMedia.Tables[0];


            #endregion

            #region Get Headers
            Dictionary<Int64, Int64> mediaToUnivers = null;
            Headers headers = GetHeaders(dtMedia, out elementsHeader, out elementsSubTotal, out universesSubTotal, out mediaToUnivers);
            #endregion

            #region Init ResultTable
            Int32 nbline = 0;
            if (levelNb == 1) nbline += GetNbLine(dtLevel1);
            if (levelNb == 2) nbline += GetNbLine(dtLevel2);
            if (levelNb == 3) nbline += GetNbLine(dtLevel3);
            ResultTable tabData = new ResultTable(nbline, headers);
            #endregion

            #region Fill result table

            Int64[] oldIds = new Int64[levelNb];
            Int64[] cIds = new Int64[levelNb];
            CellLevel[] levels = new CellLevel[nbline];
            Int32 cLine = 0, cLine2 = 0, cLine3 = 0;
            for (i = 0; i < levelNb; i++) { oldIds[i] = cIds[i] = long.MinValue; }
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

            #region Set total Line
            Double valueTot = 0;
            switch (_session.Unit)
            {
                case CstWeb.CustomerSessions.Unit.versionNb:
                    //Set total line

                    cLine = tabData.AddNewLine(LineType.total);
                    for (i = 2; i <= tabData.DataColumnsNumber; i++)
                    {
                        tabData[cLine, i] = cellFactory.Get(null);
                    }
                    tabData[cLine, 1] = new CellLevel(0, GestionWeb.GetWebWord(805, _session.SiteLanguage), null, 0, cLine);
                    foreach (DataRow row in dtTotal.Rows)
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
                            tabData.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[sIdElement].IndexInResultTable, v);
                            // SubTotal if required (univers contains more than one element)
                            if (afectSubTotal)
                            {
                                tabData.AffectValueAndAddToHierarchy(1, cLine, elementsSubTotal[sIdElement].IndexInResultTable, v);
                            }
                            // Total if required
                            if (afectTotal)
                            {
                                tabData.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[TOTAL_HEADER_ID.ToString()].IndexInResultTable, v);
                            }
                        }
                    }
                    break;
                default:
                    //Set total line                  
                    cLine = tabData.AddNewLine(LineType.total);
                    for (i = 2; i <= tabData.DataColumnsNumber; i++)
                    {
                        tabData[cLine, i] = cellFactory.Get(null);
                    }
                    tabData[cLine, 1] = new CellLevel(0, GestionWeb.GetWebWord(805, _session.SiteLanguage), null, 0, cLine);
                    foreach (DataRow row in dtTotal.Rows)
                    {
                        DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

                        Int64 idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
                        Int64 idMedia = Convert.ToInt64(row["id_media"]);
                        Double value = Convert.ToDouble(row[_session.GetSelectedUnit().Id.ToString()]);
                        string sIdElement = string.Format("{0}-{1}", mediaToUnivers[idMedia], idElement);

                        tabData.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[sIdElement].IndexInResultTable, value);
                        // SubTotal if required (univers contains more than one element)
                        if (elementsHeader[sIdElement] != elementsSubTotal[sIdElement])
                        {
                            tabData.AffectValueAndAddToHierarchy(1, cLine, elementsSubTotal[sIdElement].IndexInResultTable, value);
                        }
                        // Total if required
                        if (elementsHeader.ContainsKey(TOTAL_HEADER_ID.ToString()) && elementsSubTotal[sIdElement] != elementsHeader[TOTAL_HEADER_ID.ToString()])
                        {
                            valueTot += Convert.ToDouble(row[_session.GetSelectedUnit().Id.ToString()]);

                        }
                    }
                    if (elementsHeader != null && elementsHeader.ContainsKey(TOTAL_HEADER_ID.ToString())) tabData.AffectValueAndAddToHierarchy(1, cLine, elementsHeader[TOTAL_HEADER_ID.ToString()].IndexInResultTable, valueTot);
                    break;
            }

            #endregion

            #region Set Level 1
            //Set Level 1
            if (levelNb > 0)
            {
                foreach (DataRow row in dtLevel1.Rows)
                {
                    i = 0;
                    cIds[i] = _session.GenericProductDetailLevel.GetIdValue(row, i + 1);
                    if (cIds[i] > long.MinValue && cIds[i] != oldIds[i])
                    {
                        oldIds[i] = cIds[i];
                        for (int ii = i + 1; ii < levelNb; ii++) { oldIds[ii] = long.MinValue; }
                        cLine = InitDoubleLine(tabData, row, cellFactory, i + 1, null);
                        levels[i] = (CellLevel)tabData[cLine, 1];
                        isNewL1 = true;
                    }
                    setLine(tabData, elementsHeader, elementsSubTotal, cLine, row, cellFactory, mediaToUnivers);
                    oldIdL2 = long.MinValue;
                    idL2 = long.MinValue;

                    #region Set Level 2
                    //Set Level 2
                    if (levelNb > 1 && isNewL1)
                    {
                        List<int> tempL1ChildsIndex = level1ChildsIndex[Convert.ToInt64(cIds[i])];

                        for (int j = 0; j < tempL1ChildsIndex.Count; j++)
                        {
                            rowL2 = dtLevel2.Rows[tempL1ChildsIndex[j]];
                            idL2 = _session.GenericProductDetailLevel.GetIdValue(rowL2, 2);
                            if (oldIdL2 != idL2)
                            {
                                cLine2 = InitDoubleLine(tabData, rowL2, cellFactory, 2, null);
                                isNewL2 = true;
                            }
                            setLine(tabData, elementsHeader, elementsSubTotal, cLine2, rowL2, cellFactory, mediaToUnivers);
                            oldIdL2 = idL2;
                            oldIdL3 = long.MinValue;
                            idL3 = long.MinValue;

                            #region Set Level 3
                            //Set Level 3
                            if (levelNb > 2 && isNewL2)
                            {

                                List<int> tempL2ChildsIndex = level2ChildsIndex[cIds[i].ToString() + "-" + idL2.ToString()];

                                for (int k = 0; k < tempL2ChildsIndex.Count; k++)
                                {
                                    rowL3 = dtLevel3.Rows[tempL2ChildsIndex[k]];
                                    idL3 = _session.GenericProductDetailLevel.GetIdValue(rowL3, 3);
                                    if (oldIdL3 != idL3)
                                        cLine3 = InitDoubleLine(tabData, rowL3, cellFactory, 3, null);
                                    setLine(tabData, elementsHeader, elementsSubTotal, cLine3, rowL3, cellFactory, mediaToUnivers);
                                    oldIdL3 = idL3;
                                }
                            }
                            #endregion

                            isNewL2 = false;
                        }
                    }
                    #endregion

                    isNewL1 = false;
                    isNewL2 = false;
                }
            }
            #endregion

            #endregion

            return tabData;

        }
        #endregion

        #region Format ResultTable
        protected override ResultTable GetResultTable(ResultTable grossTable, Int32 nbLine, Dictionary<Int64, HeaderBase> universesSubTotal, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal)
        {

            #region Line Number
            //Total Line
            nbLine++;
            //Parution Number
            Dictionary<Int64, double> resNbParution = null;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.media && (_vehicleInformation.Id == CstDBClassif.Vehicles.names.press || _vehicleInformation.Id == CstDBClassif.Vehicles.names.internationalPress))
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
           

            #region Nombre parutions by media
            if (resNbParution != null && resNbParution.Count > 0)
            {
                CellNumber cNb = new CellNumber();
                cNb.StringFormat = "{0:max0}";
                CellUnitFactory nbFactory = new CellUnitFactory(cNb);
                cLine = tab.AddNewLine(LineType.nbParution);
                CellLevel cLevelParution = new CellLevel(-1, GestionWeb.GetWebWord(2460, _session.SiteLanguage), 0, cLine);
                tab[cLine, 1] = cLevelParution;
                if (_showCreative) tab[cLine, creaIndex] = new CellOneLevelRussiaCreativesLink(cLevelParution, _session, _session.GenericProductDetailLevel);
                if (_showInsertions) tab[cLine, insertIndex] = new CellOneLevelRussiaInsertionsLink(cLevelParution, _session, _session.GenericProductDetailLevel);
                if (_showMediaSchedule) tab[cLine, msIndex] = new CellMediaScheduleLink(cLevelParution, _session);
                if (_showTotal) { tab[cLine, totalIndex] = new CellNumber(); ((CellNumber)tab[cLine, totalIndex]).StringFormat = "{0:max0}"; }
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
              
                setLine(grossTable, tab, i, cLine, elementsHeader, elementsSubTotal);

            }
            #endregion

            return tab;
        }



        #endregion

        #region Synthesis
        /// <summary>
        /// Get table with synthesis about numbers of Commons, Exclusives and Missings products
        /// </summary>
        /// <returns>Result Table</returns>
        public override ResultTable GetSynthesisData()
        {

            #region Variables
            int positionUnivers = 1;
            Int32 nbLine = 8;          
            Int32 currentLineIndex = 0;          
            DataTable dt = null;
            List<string> referenceUniversMedia = null;
            List<string> competitorUniversMedia = null;
            string mediaList = "";           
            string sort = "rep_id asc";
            string unitFormat = "{0:max0}";
            #endregion

            #region Init delegates
            AddValue addValueDelegate = new AddValue(AddDoubleValue);
            InitValue initValueDelegate = new InitValue(InitDoubleValue);
            #endregion

            #region Data loading
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent Russia result"));
            object[] parameters = new object[1];
            parameters[0] = _session;
            IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            dt = presentAbsentDAL.GetSynthesisData().Tables[0];
            #endregion

            #region Identifiant du texte des unités
            Int64 unitId = _session.GetUnitLabelId();
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            #endregion

            #region Headers
            GenericDetailLevel levels = GetSummaryLevels();
            nbLine = levels.GetNbLevels;          
           
            // Ajout de la colonne Produit
            Headers headers = new Headers();
            headers.Root.Add(new Header(GestionWeb.GetWebWord(67, _session.SiteLanguage), LEVEL_HEADER_ID));

            #region Commons
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

            #region Creation of result table
            ResultTable resultTable = new ResultTable(nbLine, headers);
            Int32 nbCol = resultTable.ColumnsNumber - 2;
            string filterExpression = "";          
            DataRow[] foundRows = null;
          
            #endregion

            #region Initialisation of lines
            Int32 levelLabelColIndex = resultTable.GetHeadersIndexInResultTable(LEVEL_HEADER_ID.ToString());                     
           
            Int32 presentNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 absentNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 exclusiveNumberColumnIndex = resultTable.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            Int32 presentReferenceColumnIndex = resultTable.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);
            Int32 absentReferenceColumnIndex = resultTable.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);
            Int32 exclusiveReferenceColumnIndex = resultTable.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + REFERENCE_MEDIA_HEADER_ID);

            Int32 presentCompetitorColumnIndex = resultTable.GetHeadersIndexInResultTable(PRESENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);
            Int32 absentCompetitorColumnIndex = resultTable.GetHeadersIndexInResultTable(ABSENT_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);
            Int32 exclusiveCompetitorColumnIndex = resultTable.GetHeadersIndexInResultTable(EXCLUSIVE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + COMPETITOR_MEDIA_HEADER_ID);          
            #endregion


            if (dt != null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0)
            {

                #region Selection of vehicles
                //List of references vehicles
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
                //List of competing vehicles 
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
                    int levelsCount = levels.GetNbLevels;

                    #region Data Treatment                                         

                        DetailLevelItemInformation level;
                        for (int i = 0; i < levelsCount; i++)
                        {
                           
                           level = (DetailLevelItemInformation)levels.Levels[i];
                           if (CanShowLevel(level))
                           {
                               filterExpression = string.Format("id_level='{0}'", level.DataBaseIdField);
                               foundRows = dt.Select(filterExpression, sort);

                               if (foundRows != null && foundRows.Length > 0)
                               {
                                   currentLineIndex = resultTable.AddNewLine(LineType.level1);
                                   resultTable[currentLineIndex, levelLabelColIndex] = new CellLabel(GetLevelLabels(level));
                                  
                                   //Initilaize number
                                   CellNumber cN = new CellNumber();
                                   cN.StringFormat = unitFormat;
                                   resultTable[currentLineIndex, presentNumberColumnIndex] = cN;
                                   CellNumber cN1 = new CellNumber();
                                   cN1.StringFormat = unitFormat;
                                   resultTable[currentLineIndex, absentNumberColumnIndex] = cN1;
                                   CellNumber cN2 = new CellNumber();
                                   cN2.StringFormat = unitFormat;
                                   resultTable[currentLineIndex, exclusiveNumberColumnIndex] = cN2;

                                   //Intrilaize unit                                  
                                       for (Int32 j = presentNumberColumnIndex + 1; j < absentNumberColumnIndex; j++)
                                       {
                                           resultTable[currentLineIndex, j] = cellUnitFactory.Get(null);
                                       }
                                       for (Int32 j = absentNumberColumnIndex + 1; j < exclusiveNumberColumnIndex; j++)
                                       {
                                           resultTable[currentLineIndex, j] = cellUnitFactory.Get(null);
                                       }
                                       for (Int32 j = exclusiveNumberColumnIndex + 1; j <= nbCol; j++)
                                       {
                                           resultTable[currentLineIndex, j] = cellUnitFactory.Get(null);
                                       }
                                  

                                   foreach (DataRow currentRow in foundRows)
                                   {
                                       #region Commons
                                       if (currentRow["rep_id"] != System.DBNull.Value && Convert.ToInt64(currentRow["rep_id"].ToString()) == COMMON_RESULT_DB_ID)
                                       {    //Number 
                                           if (currentRow["num"] != System.DBNull.Value)
                                               ((CellUnit)resultTable[currentLineIndex, presentNumberColumnIndex]).Value = Convert.ToDouble(currentRow["num"].ToString());

                                           //References vehicles
                                           if (currentRow["stat1"] != System.DBNull.Value)
                                               addValueDelegate(((CellUnit)resultTable[currentLineIndex, presentReferenceColumnIndex]), Convert.ToDouble(currentRow["stat1"].ToString()));

                                           //Competing vehicles
                                           if (currentRow["stat2"] != System.DBNull.Value)
                                               addValueDelegate(((CellUnit)resultTable[currentLineIndex, presentCompetitorColumnIndex]), Convert.ToDouble(currentRow["stat2"].ToString()));
                                       }
                                       #endregion

                                       #region Absents
                                       if (currentRow["rep_id"] != System.DBNull.Value && Convert.ToInt64(currentRow["rep_id"].ToString()) == ABSENT_RESULT_DB_ID)
                                       {    //Number 
                                           if (currentRow["num"] != System.DBNull.Value)
                                               ((CellUnit)resultTable[currentLineIndex, absentNumberColumnIndex]).Value = Convert.ToDouble(currentRow["num"].ToString());

                                           //References vehicles
                                           if (currentRow["stat1"] != System.DBNull.Value)
                                               addValueDelegate(((CellUnit)resultTable[currentLineIndex, absentReferenceColumnIndex]), Convert.ToDouble(currentRow["stat1"].ToString()));

                                           //Competing vehicles
                                           if (currentRow["stat2"] != System.DBNull.Value)
                                               addValueDelegate(((CellUnit)resultTable[currentLineIndex, absentCompetitorColumnIndex]), Convert.ToDouble(currentRow["stat2"].ToString()));
                                       }
                                       #endregion

                                       #region Exclusives
                                       if (currentRow["rep_id"] != System.DBNull.Value && Convert.ToInt64(currentRow["rep_id"].ToString()) == EXCLUSIVE_RESULT_DB_ID)
                                       {    //Number 
                                           if (currentRow["num"] != System.DBNull.Value)
                                               ((CellUnit)resultTable[currentLineIndex, exclusiveNumberColumnIndex]).Value = Convert.ToDouble(currentRow["num"].ToString());

                                           //References vehicles
                                           if (currentRow["stat1"] != System.DBNull.Value)
                                               addValueDelegate(((CellUnit)resultTable[currentLineIndex, exclusiveReferenceColumnIndex]), Convert.ToDouble(currentRow["stat1"].ToString()));

                                           //Competing vehicles
                                           if (currentRow["stat2"] != System.DBNull.Value)
                                               addValueDelegate(((CellUnit)resultTable[currentLineIndex, exclusiveCompetitorColumnIndex]), Convert.ToDouble(currentRow["stat2"].ToString()));
                                       }

                                       #endregion
                                   }
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

		#region Initialisation des indexes
		/// <summary>
		/// Init headers
		/// </summary>
		/// <param name="elementsHeaders">(ou) Header for each level element</param>
		/// <param name="dtMedia">List of medias with the detail level matching
		protected override Headers GetHeaders(DataTable dtMedia, out Dictionary<string, HeaderBase> elementsHeader, out Dictionary<string, HeaderBase> elementsSubTotal, out Dictionary<Int64, HeaderBase> universesSubTotal, out Dictionary<Int64, Int64> idMediaToIdUnivers) {

			#region Extract Media lists
			string tmp = string.Empty;
			Int64[] tIds = null;
			int iUnivers = 1;
            TNS.AdExpress.Domain.Web.TextWrap textWrap = TNS.AdExpress.Domain.Web.WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
			DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
			DetailLevelItemInformation mediaDetailLevelItemInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);
			CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
			if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
			object[] param = new object[2];
            param[0] = GetDataSource();
			param[1] = _session.DataLanguage;
			TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
			TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL levels = null;
			
			//Elements by univers
			Dictionary<Int64, List<Int64>> idsByUnivers = new Dictionary<Int64, List<Int64>>();
			//Media ids ==> id univers mapping
			idMediaToIdUnivers = new Dictionary<Int64, Int64>();
			//Init media univers mapping
			while (_session.CompetitorUniversMedia[iUnivers] != null) {
				idsByUnivers.Add(iUnivers, new List<Int64>());
				// Load media ids
				tmp = _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[iUnivers], CstCustomer.Right.type.mediaAccess);
				tIds = Array.ConvertAll<string, Int64>(tmp.Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); });
				//Init Media ids X univers
				foreach (Int64 l in tIds) {
					if (!idMediaToIdUnivers.ContainsKey(l)) {
						idMediaToIdUnivers.Add(l, iUnivers);
					}
				}
				iUnivers++;
			}
			iUnivers--;

			//Dispatch elements in current univers
			List<Int64> idElements = new List<Int64>();
			StringBuilder sIdElments = new StringBuilder();
            Int64 idElement = long.MinValue;
            Int64 idMedia = long.MinValue;
			foreach (DataRow row in dtMedia.Rows) {
				idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
				idMedia = Convert.ToInt64(row[mediaDetailLevelItemInformation.DataBaseIdField]);
				if (!idElements.Contains(idElement)) {
					idElements.Add(idElement);
					sIdElments.AppendFormat("{0},", idElement);
				}
				if (!idsByUnivers[idMediaToIdUnivers[idMedia]].Contains(idElement)) {
					idsByUnivers[idMediaToIdUnivers[idMedia]].Add(idElement);
				}				

			}
			if (sIdElments.Length > 0) sIdElments.Length -= 1;
			#endregion

			#region Load elements labels

			levels = factoryLevels.CreateClassificationLevelListDAL(columnDetailLevel, sIdElments.ToString());
			
			#endregion

			#region Build headers

			#region Current Columns
			// Product column
			Headers headers = new Headers();
			headers.Root.Add(new Header(true, GestionWeb.GetWebWord(67, _session.SiteLanguage), LEVEL_HEADER_ID));


			// Add Creative column
            if (_vehicleInformation.ShowCreations &&
                _session.CustomerLogin.ShowCreatives(_vehicleInformation.Id) &&
                (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.segment) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subBrand) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)))
            {
				headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(1994, _session.SiteLanguage), CREATIVE_HEADER_ID));
				_showCreative = true;
			}

			// Add insertion colmun
			if (_vehicleInformation.ShowInsertions &&
                _session.CustomerLogin.ShowCreatives(_vehicleInformation.Id) &&
                (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.segment) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.brand) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.subBrand) ||
                _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)))
            {
				headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(2245, _session.SiteLanguage), INSERTION_HEADER_ID));
				_showInsertions = true;
			}

			// Add Media Schedule column	
			headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(150, _session.SiteLanguage), MEDIA_SCHEDULE_HEADER_ID));
			_showMediaSchedule = true;
			
			#endregion

			#region Total column
			Header headerTmp = null;
			Header headerTotal = null;
			elementsHeader = new Dictionary<string, HeaderBase>();
			if (_session.CompetitorUniversMedia.Count > 1 || idElements.Count > 1) {
                headerTotal = new Header(true, GestionWeb.GetWebWord(805, _session.SiteLanguage) + " " + GestionWeb.GetWebWord(_session.GetSelectedUnit().WebTextId, _session.SiteLanguage), TOTAL_HEADER_ID);
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
			while (_session.CompetitorUniversMedia[iUnivers] != null) {
				//Group init
				if (iUnivers != 1) {
					headerGroupTmp = new HeaderGroup(string.Format("{0} {1}", GestionWeb.GetWebWord(1366, _session.SiteLanguage), iUnivers - 1), true, START_ID_GROUP + iUnivers);
				}
				else {
					headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(1365, _session.SiteLanguage), true, START_ID_GROUP + iUnivers);
				}
				if (idsByUnivers[iUnivers].Count > 1 && _session.CompetitorUniversMedia.Count > 1) {
                    headerGroupSubTotal = headerGroupTmp.AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage) + " " + GestionWeb.GetWebWord(_session.GetSelectedUnit().WebTextId, _session.SiteLanguage), SUB_TOTAL_HEADER_ID);
					universesSubTotal.Add(iUnivers, headerGroupSubTotal);
				}
				List<Header> heads = new List<Header>();
				foreach (Int64 id in idsByUnivers[iUnivers]) {
					headerTmp = new Header(true, levels[id], id, textWrap.NbCharHeader, textWrap.Offset);
					//headerTmp = new Header(true, currentColItems[id], id);
					heads.Add(headerTmp);
					elementsHeader.Add(string.Format("{0}-{1}", iUnivers, id), headerTmp);
					if (!headerGroupTmp.ContainSubTotal) {
						if (!universesSubTotal.ContainsKey(iUnivers)) {
							if (iUnivers == 1 && idsByUnivers.Count < 2 && idsByUnivers[1].Count > 1) {
								universesSubTotal.Add(iUnivers, headerTotal);
							}
							else {
								universesSubTotal.Add(iUnivers, headerTmp);
							}
						}
						elementsSubTotal.Add(string.Format("{0}-{1}", iUnivers, id), headerTmp);
					}
					else {
						elementsSubTotal.Add(string.Format("{0}-{1}", iUnivers, id), headerGroupSubTotal);
					}
				}
				heads.Sort(delegate(Header h1, Header h2) { return h1.Label.CompareTo(h2.Label); });
				foreach (Header h in heads) {
					headerGroupTmp.Add(h);
				}
				headers.Root.Add(headerGroupTmp);
				iUnivers++;
			}
			#endregion

			#endregion

			return headers;

		}
		#endregion
		
        #region Init line version Russe
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
        #endregion

        #region InitFinalLine
        protected override Int32 InitFinalLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, CellLevel parent, Int32 creaIndex, Int32 insertIndex, Int32 msIndex)
        {
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            CellLevel cFromLevel = (CellLevel)fromTab[fromLine, 1];
            Int32 cLine = toTab.AddNewLine(fromTab.GetLineStart(fromLine).LineType);
            AdExpressCellLevel cell = new AdExpressCellLevel(cFromLevel.Id, cFromLevel.Label, parent, cFromLevel.Level, cLine, _session, textWrap.NbChar, textWrap.Offset);
            toTab[cLine, 1] = cell;

            //Links
            if (_showCreative) toTab[cLine, creaIndex] = new CellOneLevelRussiaCreativesLink(cell, _session, _session.GenericProductDetailLevel);
            if (_showInsertions) toTab[cLine, insertIndex] = new CellOneLevelRussiaInsertionsLink(cell, _session, _session.GenericProductDetailLevel);
            if (_showMediaSchedule) toTab[cLine, msIndex] = new CellMediaScheduleLink(cell, _session);
            
            return cLine;

        }
        #endregion

        #region Set double line
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
            //tab[cLine, elementsHeader[sIdElement].IndexInResultTable] = cellFactory.Get(value);
            ((CellUnit)tab[cLine, elementsHeader[sIdElement].IndexInResultTable]).Add(value);

            // SubTotal if required (univers contains more than one element)
            if (elementsHeader[sIdElement] != elementsSubTotal[sIdElement])
            {
                tab.AffectValueAndAddToHierarchy(1,cLine, elementsSubTotal[sIdElement].IndexInResultTable,  value);                
            }

            return cLine;

        }
        #endregion

        #region Set Final double line
        protected override Int64 SetFinalDoubleLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, Int32 toLine, Dictionary<string, HeaderBase> elementsHeader, Dictionary<string, HeaderBase> elementsSubTotal)
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
                        CellLevel currentLevel = (CellLevel)toTab[toLine, 1];
                        ((CellUnit)toTab[currentLevel.LineIndexInResultTable, cHeader.IndexInResultTable]).Add(value.Value);
                        //univers sub total
                        subTotalHeader = elementsSubTotal[s];
                        if (subTotalHeader != cHeader && subTotalHeader != hTotal)
                        {
                            ((CellUnit)toTab[currentLevel.LineIndexInResultTable, subTotalHeader.IndexInResultTable]).Add(value.Value);
                        }
                        //line total
                        if (hTotal != null)
                        {
                            ((CellUnit)toTab[currentLevel.LineIndexInResultTable, hTotal.IndexInResultTable]).Add(value.Value);
                        }
                    }
                }
            }

            return toLine;

        }
        #endregion

        #region GetCellPDM
        /// <summary>
        /// Get Cell PDM
        /// </summary>
        /// <param name="cell">Reference Cell</param>
        /// <returns>CellPDM</returns>
        protected override CellUnit GetCellPDM(CellUnit cell){

            CellRussiaPDM cellPDM = new CellRussiaPDM(null, cell);
            return cellPDM;

        }
        #endregion

        #region Data Filtering
        /// <summary>
        /// Filter Table
        /// </summary>
        /// <param name="data">Result Table</param>
        /// <param name="computeStrenghs">true for Strenghs result</param>
        /// <param name="universesSubTotal">Dictionary of universes sub total</param>
        protected override void FilterTable(ResultTable data, bool computeStrenghs, Dictionary<Int64, HeaderBase> universesSubTotal){}
        #endregion

        #region Compute line numbers in result table from preformated data table
        /// <summary>
        /// Get the number of line from the database data
        /// </summary>
        /// <param name="tabData">Data</param>
        /// <returns>Number of lines</returns>
        protected override Int32 GetNbLine(DataTable dt)
        {

            Int32 nbLine = 0;
            Int64 oldIdL1 = long.MinValue;
            Int64 oldIdL2 = long.MinValue;
            Int64 oldIdL3 = long.MinValue;
            Int64 cIdL1 = long.MinValue;
            Int64 cIdL2 = long.MinValue;
            Int64 cIdL3 = long.MinValue;

            int nbLevel = _session.GenericProductDetailLevel.GetNbLevels;
            foreach (DataRow row in dt.Rows)
            {
                cIdL1 = (1 > nbLevel) ? long.MinValue : _session.GenericProductDetailLevel.GetIdValue(row, 1);
                cIdL2 = (2 > nbLevel) ? long.MinValue : _session.GenericProductDetailLevel.GetIdValue(row, 2);
                cIdL3 = (3 > nbLevel) ? long.MinValue : _session.GenericProductDetailLevel.GetIdValue(row, 3);
                if (cIdL1 > long.MinValue && cIdL1 != oldIdL1)
                {
                    oldIdL1 = cIdL1;
                    oldIdL2 = oldIdL3 = long.MinValue;
                    nbLine++;
                }
                if (cIdL2 > long.MinValue && cIdL2 != oldIdL2)
                {
                    oldIdL2 = cIdL2;
                    oldIdL3 = long.MinValue;
                    nbLine++;
                }
                if (cIdL3 > long.MinValue && cIdL3 != oldIdL3)
                {
                    oldIdL3 = cIdL3;
                    nbLine++;
                }
            }
            return nbLine;
        }
        #endregion

        /// <summary>
        /// Get Data Source
        /// </summary>
        /// <returns>Data source</returns>
        protected virtual TNS.FrameWork.DB.Common.IDataSource GetDataSource()
        {            
            return _session.CustomerDataFilters.DataSource;

        }

        /// <summary>
        /// Get summary product classification levels
        /// </summary>
        /// <returns>summary product classification levels</returns>
        protected virtual GenericDetailLevel GetSummaryLevels()
        {
            //TODO : Impelements all the mechanism via configuration file

            ArrayList levelsIds = new ArrayList();
          
            levelsIds.Add(11);
            levelsIds.Add(12);
            levelsIds.Add(13);
            levelsIds.Add(14);
            levelsIds.Add(10);
            levelsIds.Add(9);
            levelsIds.Add(28);
            levelsIds.Add(8);
            GenericDetailLevel levels = new GenericDetailLevel(levelsIds);
            return levels;
        }

        protected virtual string GetLevelLabels(DetailLevelItemInformation currentLevel)
        {
            switch (currentLevel.Id)
            {
                case DetailLevelItemInformation.Levels.advertiser :
                    return GestionWeb.GetWebWord(1146, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.brand :
                    return GestionWeb.GetWebWord(1149, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.group:
                    return GestionWeb.GetWebWord(1849, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.product :
                    return GestionWeb.GetWebWord(1164, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.sector :
                    return GestionWeb.GetWebWord(1847, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.segment :
                    return GestionWeb.GetWebWord(2661, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.subBrand :
                    return GestionWeb.GetWebWord(2662, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.subSector :
                    return GestionWeb.GetWebWord(1848, _session.SiteLanguage);
                default: return "!";
            }
        }

        protected virtual bool CanShowLevel(DetailLevelItemInformation currentLevel)
        {
            bool hasRight = true;
            switch (currentLevel.Id)
            {               
                case DetailLevelItemInformation.Levels.brand:
                    return hasRight = _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE);               
                case DetailLevelItemInformation.Levels.product:
                    return hasRight = _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
                default: return hasRight;
            }         
        }

        //protected virtual string AddBreak(string label, int nbChar, int offset)
        //{
           
        //    string temp = "", res = "", curStr="";
        //    if (!string.IsNullOrEmpty(label))
        //    {
        //        #region Solution 1
        //        //while (label.Length > 0)
        //        //{
        //        //    if (label.Length > nbChar)
        //        //    {                      
        //        //        int indexOf = label.IndexOf(" ");                       
        //        //         temp = (indexOf>0 && (nbChar-indexOf)<=Math.Abs(offset)) ? 
        //        //             label.Substring(0, indexOf) : label.Substring(0, nbChar);
        //        //        label = label.Substring(temp.Length);
        //        //        res += temp + "<br/>";
        //        //    }
        //        //    else
        //        //    {
        //        //        res += label;
        //        //        label = "";
        //        //    }
        //        //}
        //        //return res;
        //        #endregion

        //        #region Solution 2
        //        int curNbChar = 0;
              
        //        for (int i = 0; i < label.Length; i++ )
        //        {
        //            curNbChar++;
        //            curStr = label.Substring(i,1);
        //            if ((curStr.Equals(" ") && Math.Abs(curNbChar - nbChar) <= offset))
        //            {
        //                curNbChar = 0;
        //                res += curStr + "<br/>";
        //            }
        //            else res += curStr;
        //        }
        //        #endregion
        //    }
        //    return label;
        //}
        
	}
}
