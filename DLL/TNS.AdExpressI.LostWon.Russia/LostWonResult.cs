#region Information
/*
 * Author : D Mussuma
 * Creation : 09/07/2010
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
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.LostWon;
using TNS.AdExpressI.LostWon.DAL;
using TNS.AdExpressI.LostWon.Exceptions;
using TNS.FrameWork.WebResultUI;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using FctWeb = TNS.AdExpress.Web.Functions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;

using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Classification.DAL;
using TNS.FrameWork.Collections;
using TNS.AdExpress.Web.Core;
using System.Globalization;
using System.Collections;
using TNS.AdExpress.Constantes;

#endregion

namespace TNS.AdExpressI.LostWon.Russia
{
    /// <summary>
    /// Russia Lost\Won  reports
    /// </summary>
    public class LostWonResult : LostWon.LostWonResult
    {
       
        #region constantes
        /// <summary>
        /// Reference period ID
        /// </summary>
        protected const long PRINCIPAL_PERIOD_ID = 1;
        /// <summary>
        /// Competing period ID
        /// </summary>
        protected const long COMPARATIVE_PERIOD_ID = 2;
        /// <summary>
        ///LOYAL result database ID
        /// </summary>
        protected const long LOYAL_RESULT_DB_ID = 1;
        /// <summary>
        ///LOYAL DECLINE result database ID
        /// </summary>
        protected const long LOYAL_DECLINE_RESULT_DB_ID = 2;
        /// <summary>
        ///LOYAL RISE result database ID
        /// </summary>
        protected const long LOYAL_RISE_RESULT_DB_ID = 3;
        /// <summary>
        ///WON  result database ID
        /// </summary>
        protected const long WON_RESULT_DB_ID = 4;
        /// <summary>
        ///LOST result database ID
        /// </summary>
        protected const long LOST_RESULT_DB_ID = 5;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public LostWonResult(WebSession session) : base(session) { }
        #endregion

        #region GetData
        /// <summary>
        /// Compute Result Data
        /// </summary>
        /// <remarks>Pay Attention that Data Filtering should be done in Russia's DAL</remarks>
        /// <returns>Computed Result Data</returns>
        protected override ResultTable GetData()
        {
            ResultTable tabData = GetRawTable();
            ResultTable tabResult = null;

            #region No data
            if (tabData == null)
            {
                return null;
            }
            #endregion

            tabResult = GetFinalTable(tabData);

            return tabResult;

        }
        #endregion

        #region Raw table
        /// <summary>
        /// Get Table with data without any filtering on required result
        /// </summary>
        /// <returns>Data</returns>
        protected override ResultTable GetRawTable()
        {
            #region Variables
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            DataSet dsMedia = null, ds = null;
            DataTable dtTotal = null, dtLevel1 = null, dtLevel2 = null, dtLevel3 = null;
            Dictionary<Int64, List<int>> level1ChildsIndex = null;
            Dictionary<string, List<int>> level2ChildsIndex = null;
            DataRow rowL2 = null, rowL3 = null;
            bool isNewL1 = false, isNewL2 = false;
            long idL2 = long.MinValue, oldIdL2 = long.MinValue, idL3 = long.MinValue, oldIdL3 = long.MinValue, tempId = long.MinValue, tempIdL2 = long.MinValue;
            List<int> tempList = null;
            Int32 levelNb = _session.GenericProductDetailLevel.GetNbLevels;
            int i = 0;
            Int64 idElement = long.MinValue;
            DetailLevelItemInformation columnDetailLevel = null;
            #endregion

            #region Date
            Int32 dateBegin = Int32.Parse(_session.PeriodBeginningDate);
            Int32 dateEnd = Int32.Parse(_session.PeriodEndDate);
            DateTime startDate = new DateTime(dateBegin / 10000, (dateBegin - (10000 * (dateBegin / 10000))) / 100, (dateBegin - (100 * (dateBegin / 100))));
            if (startDate > DateTime.Now)
            {
                return null;
            }
            #endregion

            #region Load data from data access layer
            DataTable dt = null;

            TNS.AdExpress.Domain.Web.Navigation.Module currentModuleDescription = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
            try
            {

                if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the lost won result"));
                object[] parameters = new object[1];
                parameters[0] = _session;
                ILostWonResultDAL lostwonDAL = (ILostWonResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
                ds = lostwonDAL.GetData();
                dsMedia = lostwonDAL.GetColumnDetails();

            }
            catch (System.Exception err)
            {
                throw (new LostWonException("Unable to load dynamic report data.", err));
            }
            DataTable dtMedia = dsMedia.Tables[0];

            if (ds == null || ds.Tables.Count == 0 || ds.Tables["total"].Rows.Count == 0)
            {
                return null;
            }
            #endregion

            #region Get Data tables indexes
            //Get  Data tables hierachical indexes

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
            #endregion

            #region GetHeaders
            Headers headers = GetHeaders(dtMedia);
            #endregion

            #region Init Table
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
            switch (_session.Unit)
            {
                case CstWeb.CustomerSessions.Unit.versionNb :
                    //Set total line      
                    cLine = tabData.AddNewLine(LineType.total);
                    for (i = 2; i <= tabData.DataColumnsNumber; i++)
                    {
                        tabData[cLine, i] = cellFactory.Get(null);
                    }
                    tabData[cLine, 1] = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), null, 0, cLine, textWrap.NbChar,textWrap.Offset);
                    columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

                    foreach (DataRow row in dtTotal.Rows)
                    {
                        idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
                        string idCol = string.Empty;
                        string idSubTotal = string.Empty;
                        if (Int64.Parse(row["date_num"].ToString()) == COMPARATIVE_PERIOD_ID)
                        {
                            idCol = string.Format("{0}-{1}", N1_UNIVERSE_ID, idElement);
                            idSubTotal = string.Format("{0}-{1}", N1_UNIVERSE_ID, SUBTOTAL_ID);
                        }
                        else
                        {
                            idCol = string.Format("{0}-{1}", N_UNIVERSE_ID, idElement);
                            idSubTotal = string.Format("{0}-{1}", N_UNIVERSE_ID, SUBTOTAL_ID);
                        }

                        Int32 iSubTotal = -1;
                        Int32 iCol = tabData.GetHeadersIndexInResultTable(idCol);
                        if (tabData.HeadersIndexInResultTable.ContainsKey(idSubTotal))
                        {
                            iSubTotal = tabData.GetHeadersIndexInResultTable(idSubTotal);
                        }

                        string[] value = row[_session.GetSelectedUnit().Id.ToString()].ToString().Split(',');
                        Int64 v = 0;
                        foreach (string s in value)
                        {
                            v = Convert.ToInt64(s);
                            tabData.AffectValueAndAddToHierarchy(1, cLine, iCol, v);
                            // SubTotal if required (univers contains more than one element)
                            if (iSubTotal > -1)
                            {
                                tabData.AffectValueAndAddToHierarchy(1, cLine, iSubTotal, v);
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
                    tabData[cLine, 1] = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), null, 0, cLine, textWrap.NbChar, textWrap.Offset);
                    columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
                    foreach (DataRow row in dtTotal.Rows)
                    {
                        idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
                        string idCol = string.Empty;
                        string idSubTotal = string.Empty;
                        //Is comparative line
                        if (Int64.Parse(row["date_num"].ToString())==COMPARATIVE_PERIOD_ID)
                        {
                            idCol = string.Format("{0}-{1}", N1_UNIVERSE_ID, idElement);
                            idSubTotal = string.Format("{0}-{1}", N1_UNIVERSE_ID, SUBTOTAL_ID);
                        }
                        else
                        {   
                            idCol = string.Format("{0}-{1}", N_UNIVERSE_ID, idElement);
                            idSubTotal = string.Format("{0}-{1}", N_UNIVERSE_ID, SUBTOTAL_ID);
                        }
                        if (row[_session.GetSelectedUnit().Id.ToString()] != System.DBNull.Value)
                        {
                            Double value = Convert.ToDouble(row[_session.GetSelectedUnit().Id.ToString()]);

                            //Add value to current media in tota line
                            tabData.AffectValueAndAddToHierarchy(1, cLine, tabData.GetHeadersIndexInResultTable(idCol), value);

                            //Add value to sub total
                            if (tabData.HeadersIndexInResultTable.ContainsKey(idSubTotal))
                            {
                                tabData.AffectValueAndAddToHierarchy(1, cLine, tabData.GetHeadersIndexInResultTable(idSubTotal), value);
                            }
                        }
                    }
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
                        cLine = InitLine(tabData, row, cellFactory, i + 1, null);
                        levels[i] = (CellLevel)tabData[cLine, 1];
                        isNewL1 = true;
                    }
                    setLine(tabData, cLine, row, cellFactory, dateBegin, dateEnd);
                    oldIdL2 = long.MinValue;
                    idL2 = long.MinValue;

                    #region Set Level 2
                      //Set Level 2
                    if (levelNb > 1 && isNewL1)
                    {
                        //Get L1 children indexes (L2 indexes)
                        List<int> tempL1ChildsIndex = level1ChildsIndex[Convert.ToInt64(cIds[i])];

                        //For each row L2
                        for (int j = 0; j < tempL1ChildsIndex.Count; j++)
                        {
                            rowL2 = dtLevel2.Rows[tempL1ChildsIndex[j]];
                            idL2 = _session.GenericProductDetailLevel.GetIdValue(rowL2, 2);
                            if (oldIdL2 != idL2)
                            {
                                cLine2 = InitLine(tabData, rowL2, cellFactory, 2, null);
                                isNewL2 = true;
                            }

                            setLine(tabData, cLine2, rowL2, cellFactory, dateBegin, dateEnd);
                            oldIdL2 = idL2;
                            oldIdL3 = long.MinValue;
                            idL3 = long.MinValue;

                             #region Set Level 3
                            //Set Level 3
                            if (levelNb > 2 && isNewL2)
                            {
                                //Get L2 children indexes (L3 indexes)
                                List<int> tempL2ChildsIndex = level2ChildsIndex[cIds[i].ToString() + "-" + idL2.ToString()];

                                //For each row L3
                                for (int k = 0; k < tempL2ChildsIndex.Count; k++)
                                {
                                    rowL3 = dtLevel3.Rows[tempL2ChildsIndex[k]];
                                    idL3 = _session.GenericProductDetailLevel.GetIdValue(rowL3, 3);
                                    if (oldIdL3 != idL3)
                                        cLine3 = InitLine(tabData, rowL3, cellFactory, 3, null);
                                    setLine(tabData, cLine3, rowL3, cellFactory, dateBegin, dateEnd);
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

        #region GetHeaders
        /// <summary>
        /// Build headers
        /// </summary>
        /// <param name="dtMedia">List of column levels</param>
        /// <returns>Headers of the final table</returns>
        protected override Headers GetHeaders(DataTable dtMedia)
        {

            #region Dates
            DateTime periodBeginN = new DateTime(Int32.Parse(_session.CustomerPeriodSelected.StartDate.Substring(0, 4)), Int32.Parse(_session.CustomerPeriodSelected.StartDate.Substring(4, 2)), Int32.Parse(_session.CustomerPeriodSelected.StartDate.Substring(6, 2)));
            DateTime periodEndN = new DateTime(Int32.Parse(_session.CustomerPeriodSelected.EndDate.Substring(0, 4)), Int32.Parse(_session.CustomerPeriodSelected.EndDate.Substring(4, 2)), Int32.Parse(_session.CustomerPeriodSelected.EndDate.Substring(6, 2))); ;

            DateTime periodBeginN1 = new DateTime(Int32.Parse(_session.CustomerPeriodSelected.ComparativeStartDate.Substring(0, 4)), Int32.Parse(_session.CustomerPeriodSelected.ComparativeStartDate.Substring(4, 2)), Int32.Parse(_session.CustomerPeriodSelected.ComparativeStartDate.Substring(6, 2)));
            DateTime periodEndN1 = new DateTime(Int32.Parse(_session.CustomerPeriodSelected.ComparativeEndDate.Substring(0, 4)), Int32.Parse(_session.CustomerPeriodSelected.ComparativeEndDate.Substring(4, 2)), Int32.Parse(_session.CustomerPeriodSelected.ComparativeEndDate.Substring(6, 2))); ;
            
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            string periodLabelN = FctUtilities.Dates.DateToString(periodBeginN, _session.SiteLanguage) + "-" + FctUtilities.Dates.DateToString(periodEndN, _session.SiteLanguage);
            string periodLabelN1 = FctUtilities.Dates.DateToString(periodBeginN1, _session.SiteLanguage) + "-" + FctUtilities.Dates.DateToString(periodEndN1, _session.SiteLanguage);
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = _session.CustomerDataFilters.DataSource;
            param[1] = _session.DataLanguage;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL levels = null;
            #endregion

            #region Extract Columns Elements
            List<Int64> lIds = new List<Int64>();
            Int64 id = long.MinValue;
            StringBuilder sIds = new StringBuilder();
            foreach (DataRow row in dtMedia.Rows)
            {
                id = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
                if (!lIds.Contains(id))
                {
                    lIds.Add(id);
                    sIds.AppendFormat("{0},", id);
                }
            }
            if (sIds.Length > 0) sIds.Length -= 1;
            #endregion

            #region Load elements labels
            levels = factoryLevels.CreateClassificationLevelListDAL(columnDetailLevel, sIds.ToString());
            #endregion

            #region Build headers

            #region Current Columns
            // Product column
            Headers headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(67, _session.SiteLanguage), LEVEL_ID,  textWrap.NbChar,textWrap.Offset));

            // Add Media Schedule column
            if (_session.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
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
                hGpYearN.Add(new Header(true, levels[i], i, textWrap.NbChar, textWrap.Offset));
                hGpYearN1.Add(new Header(true, levels[i], i, textWrap.NbChar, textWrap.Offset));
                hGpEvol.Add(new Header(true, levels[i], i, textWrap.NbChar, textWrap.Offset));
            }
            #endregion

            #endregion

            return headers;
        }
        #endregion

        #region Formattage d'un tableau de résultat
        /// <summary>
        /// Create ResultTable
        /// </summary>
        /// <param name="tabData">Raw Data table</param>
        /// <returns>Final Data</returns>
        protected override ResultTable GetFinalTable(ResultTable tabData)
        {
            TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            #region Lines number
            Int32 nbLine = 0;
            for (Int32 i = 0; i < tabData.LinesNumber; i++)
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

            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.media &&
                (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id
                || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id
                || CstDBClassif.Vehicles.names.newspaper == _vehicleInformation.Id
                || CstDBClassif.Vehicles.names.magazine == _vehicleInformation.Id
                ))
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
            Int32 cLine = 0;
            Int32 NIndex = tabResult.GetHeadersIndexInResultTable(N_UNIVERSE_ID.ToString());
            Int32 N1Index = tabResult.GetHeadersIndexInResultTable(N1_UNIVERSE_ID.ToString());
            Int32 levelIndex = tabResult.GetHeadersIndexInResultTable(LEVEL_ID.ToString());
            Int32 msIndex = tabResult.GetHeadersIndexInResultTable(MEDIA_SCHEDULE_ID.ToString());
            Int32 EvolIndex = tabResult.GetHeadersIndexInResultTable(EVOL_UNIVERSE_ID.ToString());
            Int32 dataIndex = (_showMediaSchedule) ? msIndex + 1 : levelIndex + 1;
            Int32 nbLevel = _session.GenericProductDetailLevel.GetNbLevels;
            Int32 NTotalIndex = tabResult.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N_UNIVERSE_ID, SUBTOTAL_ID));
            Int32 N1TotalIndex = tabResult.GetHeadersIndexInResultTable(string.Format("{0}-{1}", N1_UNIVERSE_ID, SUBTOTAL_ID));
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

            #region Number of publications by media
            CellNumber cNb = new CellNumber();
            cNb.StringFormat = "{0:max0}";
            CellUnitFactory nbFactory = new CellUnitFactory(cNb);
            CellRussiaEvol cEvol;
            if (resNbParution != null && resNbParution.Count > 0)
            {
                cLine = tabResult.AddNewLine(TNS.FrameWork.WebResultUI.LineType.nbParution);
                //Label
                CellLevel cellParution = new CellLevel(-1, GestionWeb.GetWebWord(2460, _session.SiteLanguage), 0, cLine, textWrap.NbChar, textWrap.Offset);
                tabResult[cLine, levelIndex] = cellParution;
                if (_showMediaSchedule) tabResult[cLine, msIndex] = new CellMediaScheduleLink(cellParution, _session);
                //Year N
                tabResult[cLine, NIndex] = nbFactory.Get(null);
                for (Int32 k = NIndex + 1; k < N1Index; k++)
                {
                    tabResult[cLine, k] = nbFactory.Get(null);
                }
                //Year N1
                tabResult[cLine, N1Index] = nbFactory.Get(null);
                for (Int32 k = N1Index + 1; k < EvolIndex; k++)
                {
                    tabResult[cLine, k] = nbFactory.Get(null);
                }
                //Evol
                cEvol = new CellRussiaEvol(tabResult[cLine, NIndex], tabResult[cLine, N1Index]);
                cEvol.StringFormat = "{0:percentage}";
                tabResult[cLine, EvolIndex] = cEvol;
                for (Int32 k = EvolIndex + 1; k <= tabResult.DataColumnsNumber; k++)
                {
                    cEvol = new CellRussiaEvol(tabResult[cLine, NIndex + (k - EvolIndex)], tabResult[cLine, N1Index + (k - EvolIndex)]);
                    cEvol.StringFormat = "{0:percentage}";
                    tabResult[cLine, k] = cEvol;
                }

                //Publications numbers for N and N1
                Int32 z;
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

            #region Total Line
            cLine = tabResult.AddNewLine(LineType.total);
            //Total label
            levels[0] = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), 0, cLine, textWrap.NbChar, textWrap.Offset);
            tabResult[cLine, levelIndex] = levels[0];
            if (_showMediaSchedule) tabResult[cLine, msIndex] = new CellMediaScheduleLink(levels[0], _session);
            initValues(tabResult, cLine, cellUnitFactory, computePDM, NIndex, N1Index, EvolIndex);

            //Set total line values
            setValues(tabData, tabResult, 0, cLine, NIndex, N1Index, EvolIndex, NTotalIndex, N1TotalIndex);
            #endregion

            

            #region Fill final table
            CellLevel cLevel = null;
           
            for (Int32 i = 0; i < tabData.LinesNumber; i++)
            {

                if (tabData.GetLineStart(i) is LineHide)
                    continue;

                #region Init Line
                if (tabData.GetLineStart(i).LineType != TNS.FrameWork.WebResultUI.LineType.nbParution
                    && tabData.GetLineStart(i).LineType != TNS.FrameWork.WebResultUI.LineType.total)
                {
                    cLine = InitFinalLine(tabData, tabResult, i, null, msIndex);
                    initValues(tabResult, cLine, cellUnitFactory, computePDM, NIndex, N1Index, EvolIndex);
                    cLevel = (CellLevel)tabResult[cLine, 1];

                    setValues(tabData, tabResult, i, cLine, NIndex, N1Index, EvolIndex, NTotalIndex, N1TotalIndex);

                }
                #endregion          

            }
            #endregion

            return (tabResult);
        }

        #region InitFinalLineValuesDelegate
        protected override Int32 InitFinalDoubleValuesLine(ResultTable toTab, Int32 toLine, CellUnitFactory cellFactory, bool isPDM, Int32 NIndex, Int32 N1Index, Int32 EvolIndex)
        {

            // Units
            if (isPDM)
            {
                toTab[toLine, NIndex] = new CellRussiaPDM(null, null);
                ((CellRussiaPDM)toTab[toLine, NIndex]).StringFormat = "{0:percentWOSign}";
            }
            else
            {
                toTab[toLine, NIndex] = cellFactory.Get(null);
            }
            //year N
            for (Int32 k = NIndex + 1; k < N1Index; k++)
            {
                if (isPDM)
                {
                    toTab[toLine, k] = new CellRussiaPDM(null, (CellUnit)toTab[toLine, NIndex]);
                    ((CellRussiaPDM)toTab[toLine, k]).StringFormat = "{0:percentWOSign}";
                }
                else
                {
                    toTab[toLine, k] = cellFactory.Get(null);
                }
            }
            //year N1
            if (isPDM)
            {
                toTab[toLine, N1Index] = new CellRussiaPDM(null, null);
                ((CellRussiaPDM)toTab[toLine, N1Index]).StringFormat = "{0:percentWOSign}";
            }
            else
            {
                toTab[toLine, N1Index] = cellFactory.Get(null);
            }
            for (Int32 k = N1Index + 1; k < EvolIndex; k++)
            {
                if (isPDM)
                {
                    toTab[toLine, k] = new CellRussiaPDM(null, (CellUnit)toTab[toLine, N1Index]);
                    ((CellRussiaPDM)toTab[toLine, k]).StringFormat = "{0:percentWOSign}";
                }
                else
                {
                    toTab[toLine, k] = cellFactory.Get(null);
                }
            }
            //Evol
            CellRussiaEvol cEvol = new CellRussiaEvol(toTab[toLine, NIndex], toTab[toLine, N1Index]);
            cEvol.StringFormat = "{0:percentage}";
            toTab[toLine, EvolIndex] = cEvol;
            for (Int32 k = EvolIndex + 1; k <= toTab.DataColumnsNumber; k++)
            {
                cEvol = new CellRussiaEvol(toTab[toLine, NIndex + (k - EvolIndex)], toTab[toLine, N1Index + (k - EvolIndex)]);
                cEvol.StringFormat = "{0:percentage}";
                toTab[toLine, k] = cEvol;
            }

            return toLine;

        }
        protected override Int32 InitFinalListValuesLine(ResultTable toTab, Int32 toLine, CellUnitFactory cellFactory, bool isPDM, Int32 NIndex, Int32 N1Index, Int32 EvolIndex)
        {

            // Units
            if (isPDM)
            {
                toTab[toLine, NIndex] = new CellVersionNbPDM(null);
                ((CellVersionNbPDM)toTab[toLine, NIndex]).StringFormat = "{0:percentWOSign}";
            }
            else
            {
                toTab[toLine, NIndex] = cellFactory.Get(null);
            }
            //year N
            for (Int32 k = NIndex + 1; k < N1Index; k++)
            {
                if (isPDM)
                {
                    toTab[toLine, k] = new CellVersionNbPDM((CellVersionNbPDM)toTab[toLine, NIndex]);
                    ((CellVersionNbPDM)toTab[toLine, k]).StringFormat = "{0:percentWOSign}";
                }
                else
                {
                    toTab[toLine, k] = cellFactory.Get(null);
                }
            }
            //year N1
            if (isPDM)
            {
                toTab[toLine, N1Index] = new CellVersionNbPDM(null);
                ((CellVersionNbPDM)toTab[toLine, N1Index]).StringFormat = "{0:percentWOSign}";
            }
            else
            {
                toTab[toLine, N1Index] = cellFactory.Get(null);
            }
            for (Int32 k = N1Index + 1; k < EvolIndex; k++)
            {
                if (isPDM)
                {
                    toTab[toLine, k] = new CellVersionNbPDM((CellVersionNbPDM)toTab[toLine, N1Index]);
                    ((CellVersionNbPDM)toTab[toLine, k]).StringFormat = "{0:percentWOSign}";
                }
                else
                {
                    toTab[toLine, k] = cellFactory.Get(null);
                }
            }
            //Evol
            CellRussiaEvol cEvol = new CellRussiaEvol(toTab[toLine, NIndex], toTab[toLine, N1Index]);
            cEvol.StringFormat = "{0:percentage}";
            toTab[toLine, EvolIndex] = cEvol;
            for (Int32 k = EvolIndex + 1; k <= toTab.DataColumnsNumber; k++)
            {
                cEvol = new CellRussiaEvol(toTab[toLine, NIndex + (k - EvolIndex)], toTab[toLine, N1Index + (k - EvolIndex)]);
                cEvol.StringFormat = "{0:percentage}";
                toTab[toLine, k] = cEvol;
            }

            return toLine;

        }
        protected override Int32 InitFinalLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, CellLevel parent, Int32 msIndex)
        {
            TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            CellLevel cFromLevel = (CellLevel)fromTab[fromLine, 1];
            Int32 cLine = toTab.AddNewLine(fromTab.GetLineStart(fromLine).LineType);
            AdExpressCellLevel cell = new AdExpressCellLevel(cFromLevel.Id, cFromLevel.Label, parent, cFromLevel.Level, cLine, _session, textWrap.NbChar, textWrap.Offset);
            toTab[cLine, 1] = cell;

            //Links
            if (_showMediaSchedule) toTab[cLine, msIndex] = new CellMediaScheduleLink(cell, _session);
           
            return cLine;

        }
        #endregion

        #region SetLineDelegate
        protected override Int32 SetFinalDoubleLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, Int32 toLine, Int32 NIndex, Int32 N1Index, Int32 EvolIndex, Int32 NTotalIndex, Int32 N1TotalIndex)
        {
            //Double v = 0;
            double? v = 0.0;
            //year N
            if (NTotalIndex < 0)
            {
                v = ((CellUnit)fromTab[fromLine, NIndex]).GetNullableValue();
                if(v.HasValue)
                ((CellUnit)toTab[toLine, NIndex]).Add(v.Value);
            }
            for (Int32 k = NIndex + 1; k < N1Index; k++)
            {
                v = ((CellUnit)fromTab[fromLine, k]).GetNullableValue();
                if (v.HasValue)
                {
                    ((CellUnit)toTab[toLine, k]).Add(v.Value);
                   
                    if (NTotalIndex > -1)
                    {
                        ((CellUnit)toTab[toLine, NTotalIndex]).Add(v.Value);
                    }
                }
            }
            //year N1
            if (N1TotalIndex < 0)
            {
                v = ((CellUnit)fromTab[fromLine,N1Index]).GetNullableValue();
                if (v.HasValue)
                    ((CellUnit)toTab[toLine, N1Index]).Add(v.Value);
            }
            for (Int32 k = N1Index + 1; k < EvolIndex; k++)
            {
                v = ((CellUnit)fromTab[fromLine, k]).GetNullableValue();
                if (v.HasValue)
                {
                    ((CellUnit)toTab[toLine, k]).Add(v.Value);

                    if (N1TotalIndex > -1)
                    {
                        ((CellUnit)toTab[toLine, N1TotalIndex]).Add(v.Value);
                    }
                }
            }

            return toLine;

        }
        protected override Int32 SetFinalListLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, Int32 toLine, Int32 NIndex, Int32 N1Index, Int32 EvolIndex, Int32 NTotalIndex, Int32 N1TotalIndex)
        {

            HybridList value = null;
            Int32 l = 0;
            Int64 v = 0;
            //year N
            if (NTotalIndex < 0)
            {
                value = ((CellIdsNumber)fromTab[fromLine, NIndex]).List;
                l = Convert.ToInt32(value.length);
                for (Int32 i = 0; i < l; i++)
                {
                    v = value.removeHead().UniqueID;
                    toTab.AffectValueAndAddToHierarchy(1, toLine, NIndex, v);
                }
            }
            for (Int32 k = NIndex + 1; k < N1Index; k++)
            {
                value = ((CellIdsNumber)fromTab[fromLine, k]).List;
                l = Convert.ToInt32(value.length);
                for (Int32 i = 0; i < l; i++)
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
                l = Convert.ToInt32(value.length);
                for (Int32 i = 0; i < l; i++)
                {
                    v = value.removeHead().UniqueID;
                    toTab.AffectValueAndAddToHierarchy(1, toLine, N1Index, v);
                }
            }
            for (Int32 k = N1Index + 1; k < EvolIndex; k++)
            {
                value = ((CellIdsNumber)fromTab[fromLine, k]).List;
                l = Convert.ToInt32(value.length);
                for (Int32 i = 0; i < l; i++)
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
        protected override Int32 GetNbLine(DataTable dt)
        {

            Int32 nbLine = 0;
            Int64 oldIdL1 = long.MinValue;
            Int64 oldIdL2 = long.MinValue;
            Int64 oldIdL3 = long.MinValue;
            Int64 cIdL1 = long.MinValue;
            Int64 cIdL2 = long.MinValue;
            Int64 cIdL3 = long.MinValue;

            if (dt == null || dt.Rows.Count <= 0)
                return 0;

            int nbLevel = _session.GenericProductDetailLevel.GetNbLevels;
            foreach (DataRow row in dt.Rows)
            {
                cIdL1 = (1>nbLevel) ?  long.MinValue : _session.GenericProductDetailLevel.GetIdValue(row, 1);
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

        #region SetDoubleLine
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
        protected override Int64 SetDoubleLine(ResultTable tab, Int32 cLine, DataRow row, CellUnitFactory cellFactory, Int64 periodBegin, Int64 periodEnd)
        {
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

            Int64 idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
            string idCol = string.Empty;
            string idSubTotal = string.Empty;
            if (Int64.Parse(row["date_num"].ToString())==COMPARATIVE_PERIOD_ID)
            {
                idCol = string.Format("{0}-{1}", N1_UNIVERSE_ID, idElement);
                idSubTotal = string.Format("{0}-{1}", N1_UNIVERSE_ID, SUBTOTAL_ID);
            }
            else
            {
                idCol = string.Format("{0}-{1}", N_UNIVERSE_ID, idElement);
                idSubTotal = string.Format("{0}-{1}", N_UNIVERSE_ID, SUBTOTAL_ID);
            }

            //Add Value o current media classification item
            if (row[_session.GetSelectedUnit().Id.ToString()] != System.DBNull.Value)
            {
                Double value = Convert.ToDouble(row[_session.GetSelectedUnit().Id.ToString()]);
                tab.AffectValueAndAddToHierarchy(1, cLine, tab.GetHeadersIndexInResultTable(idCol), value);

                //Add Value to sub total
                if (tab.HeadersIndexInResultTable.ContainsKey(idSubTotal))
                {
                    tab.AffectValueAndAddToHierarchy(1, cLine, tab.GetHeadersIndexInResultTable(idSubTotal), value);
                }
            }
            return cLine;

        }
        #endregion

        #region SetListLine
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
        protected override Int64 SetListLine(ResultTable tab, Int32 cLine, DataRow row, CellUnitFactory cellFactory, Int64 periodBegin, Int64 periodEnd)
        {

            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

            Int64 idElement = Convert.ToInt64(row[columnDetailLevel.DataBaseIdField]);
            string idCol = string.Empty;
            string idSubTotal = string.Empty;
            if (Int64.Parse(row["date_num"].ToString()) == COMPARATIVE_PERIOD_ID)
            {
                idCol = string.Format("{0}-{1}", N1_UNIVERSE_ID, idElement);
                idSubTotal = string.Format("{0}-{1}", N1_UNIVERSE_ID, SUBTOTAL_ID);
            }
            else
            {
                idCol = string.Format("{0}-{1}", N_UNIVERSE_ID, idElement);
                idSubTotal = string.Format("{0}-{1}", N_UNIVERSE_ID, SUBTOTAL_ID);
            }

            Int32 iSubTotal = -1;
            Int32 iCol = tab.GetHeadersIndexInResultTable(idCol);
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
        protected override Int32 InitLine(ResultTable tab, DataRow row, CellUnitFactory cellFactory, Int32 level, CellLevel parent)
        {
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
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
                , cLine, textWrap.NbChar, textWrap.Offset);            

            for (Int32 i = 2; i <= tab.DataColumnsNumber; i++)
            {
                tab[cLine, i] = cellFactory.Get(null);
            }
            return cLine;

        }
        #endregion

        #region GetSynthesisData
        /// <summary>
        /// Get synthesis report about number of products matching Loyal, Loayl sliding, Loyal rising, Won, lost
        /// </summary>
        /// <returns>Result Table</returns>
        protected override ResultTable GetSynthesisData()
        {

            #region variables
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            Int32 nbLine;
            Int32 currentLineIndex = 0;              
            DataTable dt = null;
            string beginningPeriodDA = _session.PeriodBeginningDate;
            string endPeriodDA = _session.PeriodEndDate;
            CellNumber c = new CellNumber();
            c.StringFormat = "{0:max0}";
            string sort = "rep_id asc";
            string unitFormat = "{0:max0}";
            CellUnitFactory numberFactory = new CellUnitFactory(c);
           
            AddValue addValueDelegate = new AddValue(AddDoubleValue);                    
            #endregion

            #region Computing periods
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

            string beginningPeriodN1DA = customerPeriod.ComparativeStartDate;
            string endPeriodN1DA = customerPeriod.ComparativeEndDate;
            DateTime PeriodBeginningDate = new DateTime(Int32.Parse(customerPeriod.StartDate.Substring(0, 4)), Int32.Parse(customerPeriod.StartDate.Substring(4, 2)), Int32.Parse(customerPeriod.StartDate.Substring(6, 2)));
            DateTime PeriodEndDate = new DateTime(Int32.Parse(customerPeriod.EndDate.Substring(0, 4)), Int32.Parse(customerPeriod.EndDate.Substring(4, 2)), Int32.Parse(customerPeriod.EndDate.Substring(6, 2))); ;

            DateTime PeriodBeginningDateN1DA = new DateTime(Int32.Parse(customerPeriod.ComparativeStartDate.Substring(0, 4)), Int32.Parse(customerPeriod.ComparativeStartDate.Substring(4, 2)), Int32.Parse(customerPeriod.ComparativeStartDate.Substring(6, 2)));
            DateTime PeriodEndDateN1DA = new DateTime(Int32.Parse(customerPeriod.ComparativeEndDate.Substring(0, 4)), Int32.Parse(customerPeriod.ComparativeEndDate.Substring(4, 2)), Int32.Parse(customerPeriod.ComparativeEndDate.Substring(6, 2))); ;
            CultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
          
            string PeriodDateN = FctUtilities.Dates.DateToString(PeriodBeginningDate, _session.SiteLanguage) + "-" + FctUtilities.Dates.DateToString(PeriodEndDate, _session.SiteLanguage);
            string PeriodDateN1 = FctUtilities.Dates.DateToString(PeriodBeginningDateN1DA, _session.SiteLanguage) + "-" + FctUtilities.Dates.DateToString(PeriodEndDateN1DA, _session.SiteLanguage);

            #endregion

            #region No Data (due to dates)
            if (PeriodBeginningDate > DateTime.Now)
            {
                return null;
            }
            #endregion

            #region Data loading

            try
            {
                if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the lost won result"));
                object[] parameters = new object[1];
                parameters[0] = _session;
                ILostWonResultDAL lostwonDAL = (ILostWonResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
                DataSet ds = lostwonDAL.GetSynthesisData();
                if (ds != null && ds.Tables != null && ds.Tables[0] != null)
                    dt = ds.Tables[0];

            }
            catch (System.Exception err)
            {
                throw (new LostWonException("Unable to load data for synthesis report.", err));
            }
            #endregion

            #region Aucune données (par rapport aux données)
            if (dt == null || dt.Rows == null || dt.Rows.Count < 1)
            {
                return null;
            }
            #endregion

            #region Identifiant du texte des unités
            Int64 unitId = _session.GetUnitLabelId();
            CellUnitFactory cellUnitFactory = _session.GetCellUnitFactory();
            //GetProductActivity getProductActivity;
            //string expression = string.Empty;
            //if (cellUnitFactory.Get(0.0) is CellIdsNumber)
            //{
            //    expression = _session.GetSelectedUnit().Id.ToString();
            //    //getProductActivity = new GetProductActivity(GetListProductActivity);
            //}
            //else
            //{
            //    expression = FctWeb.SQLGenerator.GetUnitAliasSum(_session);
            //    //getProductActivity = new GetProductActivity(GetDoubleProductActivity);
            //}
            #endregion

            #region Création des headers
            GenericDetailLevel levels = GetSummaryLevels();
            nbLine = levels.GetNbLevels;              
           
            // Ajout de la colonne Produit
            Headers headers = new Headers();
            headers.Root.Add(new Header(GestionWeb.GetWebWord(67, _session.SiteLanguage), LEVEL_ID, textWrap.NbCharHeader, textWrap.Offset));

            #region Fidèle
            HeaderGroup fidele = new HeaderGroup(GestionWeb.GetWebWord(1241, _session.SiteLanguage), LOYAL_HEADER_ID);
            fidele.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset));
            Header unitFidele = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset);
            unitFidele.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            unitFidele.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            fidele.Add(unitFidele);
            headers.Root.Add(fidele);
            #endregion

            #region Fidèle en baisse
            HeaderGroup fideleDecline = new HeaderGroup(GestionWeb.GetWebWord(1242, _session.SiteLanguage), LOYAL_DECLINE_HEADER_ID);
            fideleDecline.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset));
            Header unitFideleDecline = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset);
            unitFideleDecline.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            unitFideleDecline.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            fideleDecline.Add(unitFideleDecline);
            headers.Root.Add(fideleDecline);
            #endregion

            #region Fidèle en hausse
            HeaderGroup fideleRise = new HeaderGroup(GestionWeb.GetWebWord(1243, _session.SiteLanguage), LOYAL_RISE_HEADER_ID);
            fideleRise.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset));
            Header unitFideleRise = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset);
            unitFideleRise.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            unitFideleRise.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            fideleRise.Add(unitFideleRise);
            headers.Root.Add(fideleRise);
            #endregion

            #region Gagnés
            HeaderGroup won = new HeaderGroup(GestionWeb.GetWebWord(1244, _session.SiteLanguage), WON_HEADER_ID);
            won.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset));
            Header unitWon = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset);
            unitWon.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            unitWon.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            won.Add(unitWon);
            headers.Root.Add(won);
            #endregion

            #region Perdus
            HeaderGroup lost = new HeaderGroup(GestionWeb.GetWebWord(1245, _session.SiteLanguage), LOST_HEADER_ID);
            lost.Add(new Header(true, GestionWeb.GetWebWord(1852, _session.SiteLanguage), ITEM_NUMBER_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset));
            Header unitLost = new Header(true, GestionWeb.GetWebWord(unitId, _session.SiteLanguage), UNIT_HEADER_ID, textWrap.NbCharHeader, textWrap.Offset);
            unitLost.Add(new Header(true, PeriodDateN, N_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            unitLost.Add(new Header(true, PeriodDateN1, N1_UNIVERSE_ID, textWrap.NbCharHeader, textWrap.Offset));
            lost.Add(unitLost);
            headers.Root.Add(lost);
            #endregion

            #endregion

            ResultTable resultTable = new ResultTable(nbLine, headers);
            Int32 nbCol = resultTable.ColumnsNumber - 2;
            string filterExpression = "";
            DataRow[] foundRows = null;
         
            #region Initialisation des lignes

         
            #endregion

            #region Initialisation des lignes
            Int32 levelLabelColIndex = resultTable.GetHeadersIndexInResultTable(LEVEL_ID.ToString());
           
            //Number columns indexes
            Int32 _loyalNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 _loyalDeclineNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_DECLINE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 _loyalRiseNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_RISE_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 _wonNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(WON_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);
            Int32 _lostNumberColonneIndex = resultTable.GetHeadersIndexInResultTable(LOST_HEADER_ID + "-" + ITEM_NUMBER_HEADER_ID);

            //period N columns indexes
            Int32 _loyalPeriodNColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N_UNIVERSE_ID);
            Int32 _loyalDeclinePeriodNColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_DECLINE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N_UNIVERSE_ID);
            Int32 _loyalRisePeriodNColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_RISE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N_UNIVERSE_ID);
            Int32 _wonPeriodNColonneIndex = resultTable.GetHeadersIndexInResultTable(WON_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N_UNIVERSE_ID);
            Int32 _lostPeriodNColonneIndex = resultTable.GetHeadersIndexInResultTable(LOST_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N_UNIVERSE_ID);

            //period N1 columns indexes
            Int32 _loyalPeriodN1ColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N1_UNIVERSE_ID);
            Int32 _loyalDeclinePeriodN1ColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_DECLINE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N1_UNIVERSE_ID);
            Int32 _loyalRisePeriodN1ColonneIndex = resultTable.GetHeadersIndexInResultTable(LOYAL_RISE_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N1_UNIVERSE_ID);
            Int32 _wonPeriodN1ColonneIndex = resultTable.GetHeadersIndexInResultTable(WON_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N1_UNIVERSE_ID);
            Int32 _lostPeriodN1ColonneIndex = resultTable.GetHeadersIndexInResultTable(LOST_HEADER_ID + "-" + UNIT_HEADER_ID + "-" + N1_UNIVERSE_ID);
            #endregion

            if (dt != null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0)
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
                            resultTable[currentLineIndex, levelLabelColIndex] = new CellLabel(GetLevelLabels(level), textWrap.NbChar, textWrap.Offset);

                            //Initilaize number
                            CellNumber cN = new CellNumber();
                            cN.StringFormat = unitFormat;
                            resultTable[currentLineIndex, _loyalNumberColonneIndex] = cN;
                            CellNumber cN1 = new CellNumber();
                            cN1.StringFormat = unitFormat;
                            resultTable[currentLineIndex, _loyalDeclineNumberColonneIndex] = cN1;
                            CellNumber cN2 = new CellNumber();
                            cN2.StringFormat = unitFormat;
                            resultTable[currentLineIndex, _loyalRiseNumberColonneIndex] = cN2;
                            CellNumber cN3 = new CellNumber();
                            cN3.StringFormat = unitFormat;
                            resultTable[currentLineIndex, _wonNumberColonneIndex] = cN3;
                            CellNumber cN4 = new CellNumber();
                            cN4.StringFormat = unitFormat;
                            resultTable[currentLineIndex, _lostNumberColonneIndex] = cN4;

                            //Unit initialization                          
                            for (Int32 j = _loyalNumberColonneIndex + 1; j < _loyalDeclineNumberColonneIndex; j++)
                            {
                                resultTable[i, j] = cellUnitFactory.Get(null);
                            }
                            for (Int32 j = _loyalDeclineNumberColonneIndex + 1; j < _loyalRiseNumberColonneIndex; j++)
                            {
                                resultTable[i, j] = cellUnitFactory.Get(null);
                            }
                            for (Int32 j = _loyalRiseNumberColonneIndex + 1; j < _wonNumberColonneIndex; j++)
                            {
                                resultTable[i, j] = cellUnitFactory.Get(null);
                            }
                            for (Int32 j = _wonNumberColonneIndex + 1; j < _lostNumberColonneIndex; j++)
                            {
                                resultTable[i, j] = cellUnitFactory.Get(null);
                            }
                            for (Int32 j = _lostNumberColonneIndex + 1; j <= nbCol; j++)
                            {
                                resultTable[i, j] = cellUnitFactory.Get(null);
                            }

                            foreach (DataRow currentRow in foundRows)
                            {
                                #region Loyal
                                if (currentRow["rep_id"] != System.DBNull.Value && Convert.ToInt64(currentRow["rep_id"].ToString()) == LOYAL_RESULT_DB_ID)
                                {    //Number 
                                    if (currentRow["num"] != System.DBNull.Value)
                                        ((CellUnit)resultTable[currentLineIndex, _loyalNumberColonneIndex]).Value = Convert.ToDouble(currentRow["num"].ToString());

                                    //References Period
                                    if (currentRow["stat1"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _loyalPeriodNColonneIndex]), Convert.ToDouble(currentRow["stat1"].ToString()));

                                    //Competing Period
                                    if (currentRow["stat2"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _loyalPeriodN1ColonneIndex]), Convert.ToDouble(currentRow["stat2"].ToString()));
                                }
                                #endregion

                                #region Loyal decline
                                if (currentRow["rep_id"] != System.DBNull.Value && Convert.ToInt64(currentRow["rep_id"].ToString()) == LOYAL_DECLINE_RESULT_DB_ID)
                                {    //Number 
                                    if (currentRow["num"] != System.DBNull.Value)
                                        ((CellUnit)resultTable[currentLineIndex, _loyalDeclineNumberColonneIndex]).Value = Convert.ToDouble(currentRow["num"].ToString());

                                    //References Period
                                    if (currentRow["stat1"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _loyalDeclinePeriodNColonneIndex]), Convert.ToDouble(currentRow["stat1"].ToString()));

                                    //Competing Period
                                    if (currentRow["stat2"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _loyalDeclinePeriodN1ColonneIndex]), Convert.ToDouble(currentRow["stat2"].ToString()));
                                }
                                #endregion

                                #region Loyal rise
                                if (currentRow["rep_id"] != System.DBNull.Value && Convert.ToInt64(currentRow["rep_id"].ToString()) == LOYAL_RISE_RESULT_DB_ID)
                                {    //Number 
                                    if (currentRow["num"] != System.DBNull.Value)
                                        ((CellUnit)resultTable[currentLineIndex, _loyalRiseNumberColonneIndex]).Value = Convert.ToDouble(currentRow["num"].ToString());

                                    //References Period
                                    if (currentRow["stat1"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _loyalRisePeriodNColonneIndex]), Convert.ToDouble(currentRow["stat1"].ToString()));

                                    //Competing Period
                                    if (currentRow["stat2"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _loyalRisePeriodN1ColonneIndex]), Convert.ToDouble(currentRow["stat2"].ToString()));
                                }
                                #endregion

                                #region Won
                                if (currentRow["rep_id"] != System.DBNull.Value && Convert.ToInt64(currentRow["rep_id"].ToString()) == WON_RESULT_DB_ID)
                                {    //Number 
                                    if (currentRow["num"] != System.DBNull.Value)
                                        ((CellUnit)resultTable[currentLineIndex, _wonNumberColonneIndex]).Value = Convert.ToDouble(currentRow["num"].ToString());

                                    //References Period
                                    if (currentRow["stat1"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _wonPeriodNColonneIndex]), Convert.ToDouble(currentRow["stat1"].ToString()));

                                    //Competing Period
                                    if (currentRow["stat2"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _wonPeriodN1ColonneIndex]), Convert.ToDouble(currentRow["stat2"].ToString()));
                                }
                                #endregion

                                #region Lost
                                if (currentRow["rep_id"] != System.DBNull.Value && Convert.ToInt64(currentRow["rep_id"].ToString()) == LOST_RESULT_DB_ID)
                                {    //Number 
                                    if (currentRow["num"] != System.DBNull.Value)
                                        ((CellUnit)resultTable[currentLineIndex, _lostNumberColonneIndex]).Value = Convert.ToDouble(currentRow["num"].ToString());

                                    //References Period
                                    if (currentRow["stat1"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _lostPeriodNColonneIndex]), Convert.ToDouble(currentRow["stat1"].ToString()));

                                    //Competing Period
                                    if (currentRow["stat2"] != System.DBNull.Value)
                                        addValueDelegate(((CellUnit)resultTable[currentLineIndex, _lostPeriodN1ColonneIndex]), Convert.ToDouble(currentRow["stat2"].ToString()));
                                }
                                #endregion
                            }
                        }
                    }
                }



                #endregion
            }
            else return null;

            return (resultTable);
        }
        #endregion

        #region delegates
        protected delegate void AddValue(CellUnit cell, object value);
        #endregion

        #region AddValue(CellUnit cell, object value)
        protected virtual void AddDoubleValue(CellUnit cell, object value)
        {

            cell.Add(Convert.ToDouble(value));

        }
        #endregion

        #region GetSummaryLevels
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
            //levelsIds.Add(14);
            levelsIds.Add(10);
            levelsIds.Add(9);
            levelsIds.Add(28);
            levelsIds.Add(8);
            GenericDetailLevel levels = new GenericDetailLevel(levelsIds);
            return levels;
        }
        #endregion

        #region CanShowLevel
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
        #endregion

        #region GetLevelLabels
        protected virtual string GetLevelLabels(DetailLevelItemInformation currentLevel)
        {
            switch (currentLevel.Id)
            {
                case DetailLevelItemInformation.Levels.advertiser:
                    return GestionWeb.GetWebWord(1146, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.brand:
                    return GestionWeb.GetWebWord(1149, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.group:
                    return GestionWeb.GetWebWord(1849, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.product:
                    return GestionWeb.GetWebWord(1164, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.sector:
                    return GestionWeb.GetWebWord(1847, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.segment:
                    return GestionWeb.GetWebWord(2661, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.subBrand:
                    return GestionWeb.GetWebWord(2662, _session.SiteLanguage);
                case DetailLevelItemInformation.Levels.subSector:
                    return GestionWeb.GetWebWord(1848, _session.SiteLanguage);
                default: return "!";
            }
        }
        #endregion
    }
}
