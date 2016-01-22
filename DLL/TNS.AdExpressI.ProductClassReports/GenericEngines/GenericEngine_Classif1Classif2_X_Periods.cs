#region Information
/*
 * Author : G Ragneau
 * Created on : 12/01/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion


using System;
using System.Globalization;
using System.Collections.Generic;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;

using CstClassif = TNS.AdExpress.Constantes.Classification;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;

namespace TNS.AdExpressI.ProductClassReports.GenericEngines
{

    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Classif2 X Period (monthly or yearly)
    /// </summary>
    public abstract class GenericEngine_Classif1Classif2_X_Periods : GenericEngine
    {

        #region Constants
        protected const Int32 ID_PRODUCT = 0;
        protected const Int32 ID_YEAR_N = -1;
        protected const Int32 ID_PDV_YEAR_N = -2;
        protected const Int32 ID_PDM_YEAR_N = -3;
        protected const Int32 ID_YEAR_N1 = -4;
        protected const Int32 ID_PDV_YEAR_N1 = -5;
        protected const Int32 ID_PDM_YEAR_N1 = -6;
        protected const Int32 ID_EVOL = -7;
        #endregion

        #region Attributes
        /// <summary>
        /// Determine if the table must be computed with a monthly or a yearly detail
        /// </summary>
        protected bool _monthlyExtended;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set the period breakdown (monthly or yearly)
        /// </summary>
        public bool MonthlyExtended
        {
            get
            {
                return _monthlyExtended;
            }
            set
            {
                _monthlyExtended = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public GenericEngine_Classif1Classif2_X_Periods(WebSession session, int result) : base(session, result) { }
        #endregion

        #region ComputeData
        protected override ResultTable ComputeData(DataSet data)
        {

            ProductClassResultTable tab = null;

            #region Data Checking
            DataTable dtData = data.Tables[0];
            if (dtData.Rows.Count <= 0) return null;
            #endregion

            #region Data Indexes

            #region First numerical data and classification indexes
            Int32 DATA_FIRST_PRODUCT_INDEX = -1;
            Int32 DATA_FIRST_MEDIA_INDEX = -1;
            Int32 DATA_FIRST_DATA_COLUMN = -1;
            for (Int32 i = 0; i < dtData.Columns.Count; i = i + 2)
            {

                if (dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0 || dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0)
                {

                    if (dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0 && DATA_FIRST_MEDIA_INDEX < 0)
                    {
                        DATA_FIRST_MEDIA_INDEX = i;
                    }
                    else if (dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0 && DATA_FIRST_PRODUCT_INDEX < 0)
                    {
                        DATA_FIRST_PRODUCT_INDEX = i;
                    }
                }
                else
                {
                    DATA_FIRST_DATA_COLUMN = i;
                    break;
                }
            }
            #endregion

            #region Identify classification hierarchy
            Int32 DATA_C1_INDEX;
            Int32 DATA_C2_INDEX;
            CstClassif.Branch.type C1_TYPE;
            CstClassif.Branch.type C2_TYPE;
            List<DetailLevelItemInformation> MAIN_LEVELS = null;
            List<DetailLevelItemInformation> SECOND_LEVELS = null;

            if (DATA_FIRST_MEDIA_INDEX < DATA_FIRST_PRODUCT_INDEX)
            {
                //Main classification  = media
                DATA_C1_INDEX = DATA_FIRST_MEDIA_INDEX;
                C1_TYPE = CstClassif.Branch.type.media;
                //Secondary classification = produit
                DATA_C2_INDEX = DATA_FIRST_PRODUCT_INDEX;
                C2_TYPE = CstClassif.Branch.type.product;
                //levels
                MAIN_LEVELS = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
                SECOND_LEVELS = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
                //Fake level to replace total line
                if (_vehicle == Vehicles.names.plurimedia || _vehicle == Vehicles.names.PlurimediaWithoutMms)
                {
                    MAIN_LEVELS.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.slogan));
                }
            }
            else
            {
                //Main classification  = produit
                DATA_C1_INDEX = DATA_FIRST_PRODUCT_INDEX;
                C1_TYPE = CstClassif.Branch.type.product;
                //Secondary classification = media
                DATA_C2_INDEX = DATA_FIRST_MEDIA_INDEX;
                C2_TYPE = CstClassif.Branch.type.media;
                //levels
                MAIN_LEVELS = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
                SECOND_LEVELS = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
                //Fake level to replace total line
                MAIN_LEVELS.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.slogan));
            }
            //Classification levels indexes
            List<Int32> DATA_C1_INDEXES = new List<Int32>();
            List<Int32> DATA_C2_INDEXES = new List<Int32>();
            List<long> C1_IDS = new List<long>();
            List<long> C2_IDS = new List<long>();
            List<CellLevel> C1_LEVELS = new List<CellLevel>();
            List<CellLevel> C2_LEVELS = new List<CellLevel>();
            for (int i = DATA_C1_INDEX; i < DATA_C2_INDEX; i = i + 2)
            {
                DATA_C1_INDEXES.Add(i);
                C1_IDS.Add(-1);
            }
            for (int i = DATA_C2_INDEX; i < DATA_FIRST_DATA_COLUMN; i = i + 2)
            {
                DATA_C2_INDEXES.Add(i);
                C2_IDS.Add(-1);
            }
            #endregion

            #region Personal advertisers
            if (dtData.Columns.Contains("inref"))
            {
                _isPersonalized = 3;
            }
            #endregion

            // Delete useless lines
            CleanDataTable(dtData, DATA_FIRST_DATA_COLUMN);

            #endregion

            #region Periods
            DateTime begin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);

            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            string periodEnd = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);

            int yearN = Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4));
            int yearN1 = _session.ComparativeStudy ? yearN - 1 : -1;
            Int32 DATA_FIRST_MONTH_COLUMN = -1;
            Int32 DATA_LAST_MONTH_COLUMN = -1;
            int firstMonth = Convert.ToInt32(_session.PeriodBeginningDate.Substring(4, 2));
            int lastMonth = Convert.ToInt32(periodEnd.Substring(4, 2));
            Int32 nbMonthes = (lastMonth - firstMonth) + 1;
            if (_monthlyExtended)
            {
                DATA_FIRST_MONTH_COLUMN = DATA_FIRST_DATA_COLUMN + 1;
                if (yearN1 > -1)
                {
                    DATA_FIRST_MONTH_COLUMN++;
                }
                DATA_LAST_MONTH_COLUMN = DATA_FIRST_MONTH_COLUMN + (lastMonth - firstMonth);
            }
            #endregion

            #region Headers
            Headers headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), ID_PRODUCT));
            headers.Root.Add(new Header(true, yearN.ToString(), ID_YEAR_N));
            if (_session.PDV)
            {
                headers.Root.Add(new Header(true, string.Format("{0} {1}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), yearN), ID_PDV_YEAR_N));
            }
            //PDM
            if (_session.PDM)
            {
                headers.Root.Add(new Header(true, string.Format("{0} {1}", GestionWeb.GetWebWord(806, _session.SiteLanguage), yearN), ID_PDM_YEAR_N));
            }
            //N-1
            if (_session.ComparativeStudy)
            {
                headers.Root.Add(new Header(true, yearN1.ToString(), ID_YEAR_N1));
                if (_session.PDV)
                {
                    headers.Root.Add(new Header(true, string.Format("{0} {1}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), yearN1), ID_PDV_YEAR_N1));
                }
                //PDM
                if (_session.PDM)
                {
                    headers.Root.Add(new Header(true, string.Format("{0} {1}", GestionWeb.GetWebWord(806, _session.SiteLanguage), yearN1), ID_PDM_YEAR_N1));
                }
                //Evol
                if (_session.Evolution)
                {
                    headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1168, _session.SiteLanguage), ID_EVOL));
                }
            }
            //Months
            if (_monthlyExtended)
            {
                CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
                DateTime cMonth = begin;
                for (int j = 0; j < nbMonthes; j++)
                {
                    headers.Root.Add(new Header(true, cMonth.ToString("MMMM", cInfo), cMonth.Month));
                    cMonth = cMonth.AddMonths(1);
                }
            }
            tab = new ProductClassResultTable(Convert.ToInt32(dtData.Rows.Count * 1.5), headers);
            Int32 RES_YEAR_N_INDEX = tab.GetHeadersIndexInResultTable(ID_YEAR_N.ToString());
            Int32 RES_PDV_YEAR_N_INDEX = tab.GetHeadersIndexInResultTable(ID_PDV_YEAR_N.ToString());
            Int32 RES_PDM_YEAR_N_INDEX = tab.GetHeadersIndexInResultTable(ID_PDM_YEAR_N.ToString());
            Int32 RES_YEAR_N1_INDEX = tab.GetHeadersIndexInResultTable(ID_YEAR_N1.ToString());
            Int32 RES_PDV_YEAR_N1_INDEX = tab.GetHeadersIndexInResultTable(ID_PDV_YEAR_N1.ToString());
            Int32 RES_PDM_YEAR_N1_INDEX = tab.GetHeadersIndexInResultTable(ID_PDM_YEAR_N1.ToString());
            Int32 RES_EVOL_INDEX = tab.GetHeadersIndexInResultTable(ID_EVOL.ToString());
            Int32 RES_FIRSTMONTH_INDEX = tab.GetHeadersIndexInResultTable(begin.Month.ToString());
            #endregion

            #region LineTypes
            List<LineType> c1LineTypes = new List<LineType>();
            List<LineType> c2SubTotalLineTypes = new List<LineType>();
            List<LineType> c2LineTypes = new List<LineType>();
            c1LineTypes.Add(LineType.total);
            c1LineTypes.Add(LineType.level1);
            c1LineTypes.Add(LineType.level2);
            c1LineTypes.Add(LineType.level3);
            c1LineTypes.Add(LineType.level4);
            c2SubTotalLineTypes.Add(LineType.subTotal1);
            c2SubTotalLineTypes.Add(LineType.subTotal2);
            c2SubTotalLineTypes.Add(LineType.subTotal3);
            c2SubTotalLineTypes.Add(LineType.subTotal4);
            c2LineTypes.Add(LineType.level5);
            c2LineTypes.Add(LineType.level6);
            c2LineTypes.Add(LineType.level7);
            c2LineTypes.Add(LineType.level8);
            #endregion

            #region Build Table
            long cId = -1;
            Int32 cLine = -1;
            List<Int64> mainLevelIds = new List<Int64>();
            List<Int64> scdLevelIds = new List<Int64>();
            Double dYearN = 0.0;
            Double dYearN1 = 0.0;
            List<Double> dMonthes = new List<Double>();
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();

            foreach (DataRow row in dtData.Rows)
            {

                dYearN = Convert.ToDouble(row[DATA_FIRST_DATA_COLUMN]);
                if (RES_YEAR_N1_INDEX > -1)
                {
                    dYearN1 = Convert.ToDouble(row[DATA_FIRST_DATA_COLUMN + 1]);
                }
                if (dYearN + dYearN1 > 0)
                {

                    #region Main classification init
                    for (int i = 0; i < C1_IDS.Count; i++)
                    {
                        cId = Convert.ToInt64(row[DATA_C1_INDEXES[i]]);
                        if (cId != C1_IDS[i])
                        {
                            C1_IDS[i] = cId;
                            for (int j = mainLevelIds.Count - 1; j >= i && mainLevelIds.Count > 0; j--)
                            {
                                mainLevelIds.RemoveAt(j);
                                C1_LEVELS.RemoveAt(j);
                            }
                            mainLevelIds.Add(cId);
                            //Init sublevels
                            for (int j = i + 1; j < C1_IDS.Count; j++)
                            {
                                C1_IDS[j] = -1;

                            }
                            //Init Scnd Classification Sub Levels
                            scdLevelIds = new List<Int64>();
                            for (int j = 0; j < C2_IDS.Count; j++)
                            {
                                C2_IDS[j] = -1;

                            }

                            //Init current line
                            C1_LEVELS.Add(new CellLevel(cId, row[DATA_C1_INDEXES[i] + 1].ToString(), i, cLine));
                            cLine = tab.AddNewLine(c1LineTypes[i], mainLevelIds, C1_LEVELS[i]);
                            //year N
                            tab[cLine, 2] = cellFactory.Get(0.0);
                            //PDV
                            if (RES_PDV_YEAR_N_INDEX > -1)
                            {
                                if (C1_TYPE == CstClassif.Branch.type.product && i > 0)
                                {
                                    tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(0.0, (CellUnit)tab[C1_LEVELS[i - 1].LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                    ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                }
                                else
                                {
                                    tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(0.0, null);
                                    ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                }
                            }
                            //PDM
                            if (RES_PDM_YEAR_N_INDEX > -1)
                            {
                                if (C1_TYPE == CstClassif.Branch.type.media && i > 0)
                                {
                                    tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(0.0, (CellUnit)tab[C1_LEVELS[i - 1].LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                    ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                }
                                else
                                {
                                    tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(0.0, null);
                                    ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                }
                            }
                            //Year N1
                            if (yearN1 > -1)
                            {
                                tab[cLine, RES_YEAR_N1_INDEX] = cellFactory.Get(0.0);
                                //PDV
                                if (RES_PDV_YEAR_N1_INDEX > -1)
                                {
                                    if (C1_TYPE == CstClassif.Branch.type.product && i > 0)
                                    {
                                        tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(0.0, (CellUnit)tab[C1_LEVELS[i - 1].LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                        ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                    }
                                    else
                                    {
                                        tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(0.0, null);
                                        ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                    }
                                }
                                //PDM
                                if (RES_PDM_YEAR_N1_INDEX > -1)
                                {
                                    if (C1_TYPE == CstClassif.Branch.type.media && i > 0)
                                    {
                                        tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(0.0, (CellUnit)tab[C1_LEVELS[i - 1].LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                        ((CellUnit)tab[cLine, RES_PDM_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                    }
                                    else
                                    {
                                        tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(0.0, null);
                                        ((CellUnit)tab[cLine, RES_PDM_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                    }
                                }
                                //Evol
                                if (RES_EVOL_INDEX > -1)
                                {
                                    tab[cLine, RES_EVOL_INDEX] = new CellEvol(tab[C1_LEVELS[i].LineIndexInResultTable, RES_YEAR_N_INDEX], tab[C1_LEVELS[i].LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                    ((CellUnit)tab[cLine, RES_EVOL_INDEX]).StringFormat = "{0:percentage}";
                                }

                            }
                            //Monthes of year N
                            if (_monthlyExtended)
                            {
                                for (int j = 0; j < nbMonthes; j++)
                                {
                                    tab[cLine, RES_FIRSTMONTH_INDEX + j] = cellFactory.Get(0.0);
                                }
                            }

                        }
                        cLine = C1_LEVELS[i].LineIndexInResultTable;
                        //Advertisers
                        if (_isPersonalized > 0)
                        {                            
                            SetPersoAdvertiser(tab, cLine, row, MAIN_LEVELS[i].Id, C1_TYPE);

                        }
                    }
                    #endregion

                    #region Second classification treatment
                    for (int i = 0; i < C2_IDS.Count; i++)
                    {
                        cId = Convert.ToInt64(row[DATA_C2_INDEXES[i]]);
                        if (cId != C2_IDS[i])
                        {
                            C2_IDS[i] = cId;
                            for (int j = scdLevelIds.Count - 1; j >= i && scdLevelIds.Count > 0; j--)
                            {
                                scdLevelIds.RemoveAt(j);
                                C2_LEVELS.RemoveAt(j);
                            }
                            scdLevelIds.Add(cId);
                            //Init sublevels
                            for (int j = i + 1; j < C2_IDS.Count; j++)
                            {
                                C2_IDS[j] = -1;

                            }
                            //Check if parent and current main level contain this second level
                            for (int j = 0; j < mainLevelIds.Count; j++)
                            {
                                List<Int64> parentKeys = mainLevelIds.GetRange(0, j + 1);
                                if ((cLine = tab.Contains(parentKeys, scdLevelIds)) < 0)
                                {
                                    CellLevel p = tab.GetLevel(parentKeys, scdLevelIds.GetRange(0, scdLevelIds.Count - 1));
                                    CellLevel c = new CellLevel(cId, row[DATA_C2_INDEXES[i] + 1].ToString(), p, j, cLine);
                                    C2_LEVELS.Add(c);
                                    cLine = tab.InsertNewLine((j == 0) ? c2SubTotalLineTypes[i] : c2LineTypes[i], parentKeys, scdLevelIds, c);
                                    //year N
                                    tab[cLine, 2] = cellFactory.Get(0.0);
                                    //PDV
                                    if (RES_PDV_YEAR_N_INDEX > -1)
                                    {
                                        if (C1_TYPE == CstClassif.Branch.type.product)
                                        {
                                            if (j > 0)
                                            {
                                                tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(0.0, (CellUnit)tab[tab.GetLevel(mainLevelIds.GetRange(0, j), scdLevelIds).LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                                ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                            else
                                            {
                                                tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(0.0, null);
                                                ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                        }
                                        else
                                        {
                                            tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(0.0, (CellUnit)tab[p.LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                            ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                        }
                                    }
                                    //PDM
                                    if (RES_PDM_YEAR_N_INDEX > -1)
                                    {
                                        if (C1_TYPE == CstClassif.Branch.type.media)
                                        {
                                            if (j > 0)
                                            {
                                                tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(0.0, (CellUnit)tab[tab.GetLevel(mainLevelIds.GetRange(0, 1), scdLevelIds).LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                                ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                            else
                                            {
                                                tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(0.0, null);
                                                ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                        }
                                        else
                                        {
                                            tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(0.0, (CellUnit)tab[p.LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                            ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                        }
                                    }
                                    //Year N1
                                    if (yearN1 > -1)
                                    {
                                        tab[cLine, RES_YEAR_N1_INDEX] = cellFactory.Get(0.0);
                                        //PDV
                                        if (RES_PDV_YEAR_N1_INDEX > -1)
                                        {
                                            if (C1_TYPE == CstClassif.Branch.type.product)
                                            {
                                                if (j > 0)
                                                {
                                                    tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(0.0, (CellUnit)tab[tab.GetLevel(mainLevelIds.GetRange(0, j), scdLevelIds).LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                                    ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                                }
                                                else
                                                {
                                                    tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(0.0, null);
                                                    ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                                }
                                            }
                                            else
                                            {
                                                tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(0.0, (CellUnit)tab[p.LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                                ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                        }
                                        //PDM
                                        if (RES_PDM_YEAR_N1_INDEX > -1)
                                        {
                                            if (C1_TYPE == CstClassif.Branch.type.media)
                                            {
                                                if (j > 0)
                                                {
                                                    tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(0.0, (CellUnit)tab[tab.GetLevel(mainLevelIds.GetRange(0, 1), scdLevelIds).LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                                    ((CellUnit)tab[cLine, RES_PDM_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                                }
                                                else
                                                {
                                                    tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(0.0, null);
                                                    ((CellUnit)tab[cLine, RES_PDM_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                                }
                                            }
                                            else
                                            {
                                                tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(0.0, (CellUnit)tab[p.LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                                ((CellUnit)tab[cLine, RES_PDM_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                        }
                                        //Evol
                                        if (RES_EVOL_INDEX > -1)
                                        {
                                            tab[cLine, RES_EVOL_INDEX] = new CellEvol(tab[cLine, RES_YEAR_N_INDEX], tab[cLine, RES_YEAR_N1_INDEX]);
                                            ((CellUnit)tab[cLine, RES_EVOL_INDEX]).StringFormat = "{0:percentage}";
                                        }
                                    }
                                    //Monthes of year N
                                    if (_monthlyExtended)
                                    {
                                        for (int m = 0; m < nbMonthes; m++)
                                        {
                                            tab[cLine, RES_FIRSTMONTH_INDEX + m] = cellFactory.Get(0.0);
                                        }
                                    }

                                }
                            }

                            //Added
                            if (_isPersonalized > 0)
                            {
                                for (int j = 0; j < mainLevelIds.Count; j++)
                                {
                                    List<Int64> parentKeys = mainLevelIds.GetRange(0, j + 1);
                                    cLine = tab.Contains(parentKeys, scdLevelIds);
                                    //Advertisers                                    
                                    SetPersoAdvertiser(tab, cLine, row, SECOND_LEVELS[i].Id, C2_TYPE);
                                }
                            }
                        }
                    }
                    //                        if (_isPersonalized > 0)
                    //                        {
                    //                            for (int j = 0; j < mainLevelIds.Count; j++)
                    //                            {
                    //                                List<Int64> parentKeys = mainLevelIds.GetRange(0, j + 1);
                    //                                cLine = tab.Contains(parentKeys, scdLevelIds);
                    //                                //Advertisers
                    //#region OLD

                    //                                //ProductClassLineStart ls = (ProductClassLineStart)tab[cLine, 0];

                    //                                //if (Convert.ToInt32(row["inref"]) > 0)
                    //                                //{
                    //                                //    ls.SetUniversType(UniversType.reference);
                    //                                //}
                    //                                //if (Convert.ToInt32(row["incomp"]) > 0)
                    //                                //{
                    //                                //    ls.SetUniversType(UniversType.concurrent);
                    //                                //}
                    //                                //if (Convert.ToInt32(row["inneutral"]) > 0)
                    //                                //{
                    //                                //    ls.SetUniversType(UniversType.neutral);
                    //                                //}
                    //                                //if (C2_TYPE == CstClassif.Branch.type.product)
                    //                                //{
                    //                                //    switch (SECOND_LEVELS[i].Id)
                    //                                //    {
                    //                                //        case DetailLevelItemInformation.Levels.advertiser:
                    //                                //        case DetailLevelItemInformation.Levels.brand:
                    //                                //        case DetailLevelItemInformation.Levels.product:
                    //                                //            DisplayPerso(ls,row, SECOND_LEVELS[i].Id);
                    //                                //            break;
                    //                                //        default:
                    //                                //            break;
                    //                                //    }
                    //                                //}
                    //#endregion
                    //                                SetPersoAdvertiser(tab, cLine, row, SECOND_LEVELS[i].Id, C2_TYPE);
                    //                            }
                    //                        }
                    //                    }
                    #endregion

                    #region Affect values
                    dMonthes.Clear();
                    //Get values
                    if (_monthlyExtended)
                    {
                        for (int i = 0; i < nbMonthes; i++)
                        {
                            dMonthes.Add(Convert.ToDouble(row[DATA_FIRST_MONTH_COLUMN + i]));
                        }
                    }

                    //Affect value to each required cell
                    for (int i = 1; i <= mainLevelIds.Count; i++)
                    {
                        cLine = tab.GetLevel(mainLevelIds.GetRange(0, i), scdLevelIds).LineIndexInResultTable;
                        tab.AffectValueAndAddToHierarchy(1, cLine, RES_YEAR_N_INDEX, dYearN);
                        if (RES_PDM_YEAR_N_INDEX > -1)
                        {
                            tab.AffectValueAndAddToHierarchy(1, cLine, RES_PDM_YEAR_N_INDEX, dYearN);
                        }
                        if (RES_PDV_YEAR_N_INDEX > -1)
                        {
                            tab.AffectValueAndAddToHierarchy(1, cLine, RES_PDV_YEAR_N_INDEX, dYearN);
                        }
                        if (yearN1 > 0)
                        {
                            tab.AffectValueAndAddToHierarchy(1, cLine, RES_YEAR_N1_INDEX, dYearN1);
                            if (RES_PDM_YEAR_N1_INDEX > -1)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, RES_PDM_YEAR_N1_INDEX, dYearN1);
                            }
                            if (RES_PDV_YEAR_N1_INDEX > -1)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, RES_PDV_YEAR_N1_INDEX, dYearN1);
                            }
                        }
                        if (_monthlyExtended)
                        {
                            for (int j = 0; j < nbMonthes; j++)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, RES_FIRSTMONTH_INDEX + j, dMonthes[j]);
                            }
                        }

                    }
                    #endregion

                }

            }
            #endregion

            #region Hide lines if required
            if (_session.PersonalizedElementsOnly && _isPersonalized > 0)
            {
                for (int i = 0; i < tab.LinesNumber; i++)
                {
                    ProductClassLineStart ls = (ProductClassLineStart)tab[i, 0];
                    if (ls.LineUnivers == UniversType.neutral && ChildrenAreNeutral(tab, i))
                    {
                        tab.SetLineStart(new LineHide(ls.LineType), i);
                    }
                }
            }
            #endregion

            return tab;

        }
        #endregion
                
    }
}
