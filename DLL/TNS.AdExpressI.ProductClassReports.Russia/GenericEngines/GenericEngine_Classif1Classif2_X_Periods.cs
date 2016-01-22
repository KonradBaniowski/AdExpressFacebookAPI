using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;

using CstClassif = TNS.AdExpress.Constantes.Classification;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Translation;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.Date;
using TNS.AdExpressI.ProductClassReports.GenericEngines;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;
using TNS.AdExpress.Constantes;

namespace TNS.AdExpressI.ProductClassReports.Russia.GenericEngines
{

    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Classif2 X Period (monthly or yearly)
    /// </summary>
    public class GenericEngine_Classif1Classif2_X_Periods : TNS.AdExpressI.ProductClassReports.GenericEngines.GenericEngine_Classif1Classif2_X_Periods
    {

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public GenericEngine_Classif1Classif2_X_Periods(WebSession session, int result) : base(session, result) { }
        #endregion

        #region Abstract methods implementation
        /// <summary>
        /// Compute data got from the DAL layer before to design the report
        /// Build a table of two types based on the user parameters in session:
        /// Type 1 : (media) X (N [PDM, N-1, EVOL])
        /// Type 2 : (produit) X (N [PDV, N-1, EVOL])
        /// Steps:
        ///		- Check data
        ///		- Build constraints:
        ///			- reference and competitors params
        ///			- index of first column with numerical data
        ///			- indexes table with (column index in dstatable, line of the level in the result table, level id) for each classification level
        /// </summary>
        /// <param name="data">DAL data</param>
        /// <returns>data computed from DAL result</returns>
        protected override ResultTable ComputeData(DataSet dsData)
        {

            ProductClassResultTable tab = null;
            TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            #region Data Checking
            if (dsData == null || dsData.Tables.Count ==0) return null;
            DataTable dtData = dsData.Tables[dsData.Tables.Count-1];
            if (dtData.Rows.Count <= 0) return null;
            #endregion
            
            #region Data Indexes

            #region First numerical data and classification indexes
            Int32 DATA_FIRST_PRODUCT_INDEX = -1;
            Int32 DATA_FIRST_MEDIA_INDEX = -1;
            Int32 DATA_FIRST_DATA_COLUMN = -1;
            for (Int32 i = 0; i < dtData.Columns.Count; i ++){

                if (dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0 || dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0){

                    if (dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0 && DATA_FIRST_MEDIA_INDEX < 0){
                        DATA_FIRST_MEDIA_INDEX = i;
                    }
                    else if (dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0 && DATA_FIRST_PRODUCT_INDEX < 0){
                        DATA_FIRST_PRODUCT_INDEX = i;
                    }
                }
            }
            #endregion

            #region Identify classification hierarchy
            CstClassif.Branch.type C1_TYPE;
            CstClassif.Branch.type C2_TYPE;
            List<DetailLevelItemInformation> MAIN_LEVELS = null;
            List<DetailLevelItemInformation> SECOND_LEVELS = null;
            int mainLevelsCount = 0, secondlevelsCount = 0;

            if (DATA_FIRST_MEDIA_INDEX < DATA_FIRST_PRODUCT_INDEX){

                //Main classification  = media
                C1_TYPE = CstClassif.Branch.type.media;
                //Secondary classification = produit
                C2_TYPE = CstClassif.Branch.type.product;
                //levels
                MAIN_LEVELS = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
                SECOND_LEVELS = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
                mainLevelsCount = MAIN_LEVELS.Count;
                secondlevelsCount = SECOND_LEVELS.Count;
                //Fake level to replace total line
                if (_vehicle == Vehicles.names.plurimedia){
                    MAIN_LEVELS.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.slogan));
                }
            }
            else{

                //Main classification  = produit
                C1_TYPE = CstClassif.Branch.type.product;
                //Secondary classification = media
                C2_TYPE = CstClassif.Branch.type.media;
                //levels
                MAIN_LEVELS = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
                SECOND_LEVELS = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
                mainLevelsCount = MAIN_LEVELS.Count;
                secondlevelsCount = SECOND_LEVELS.Count;
                //Fake level to replace total line
                MAIN_LEVELS.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.slogan));
            }
            //Classification levels indexes
            List<Int32> C1_IDS = new List<Int32>();
            List<Int32> C2_IDS = new List<Int32>();
            List<CellLevel> C1_LEVELS = new List<CellLevel>();
            List<CellLevel> C2_LEVELS = new List<CellLevel>();
            for (int i = 0; i < MAIN_LEVELS.Count; i++)
                C1_IDS.Add(-1);
            for (int i = 0; i < SECOND_LEVELS.Count; i++)
                C2_IDS.Add(-1);

            Dictionary<string, Dictionary<string, DataRow>> globalDictionary = CommonFunctions.InitDictionariesData(dsData, mainLevelsCount, secondlevelsCount, C1_TYPE, C2_TYPE);
            #endregion

            #region Personal advertisers
            if ((C1_TYPE == CstClassif.Branch.type.product && dsData.Tables["CLASSIF1_1"].Columns.Contains("inref"))
                || (C2_TYPE == CstClassif.Branch.type.product && dsData.Tables["CLASSIF2_1_1"].Columns.Contains("inref"))){
                _isPersonalized = 3;
            }
            #endregion

            #endregion

            #region Periods
            DateTime begin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);

            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            string periodEnd = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);

            int yearN = Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4));
            int yearN1 = _session.ComparativeStudy ? yearN - 1 : -1;
            Int32 DATA_FIRST_MONTH_COLUMN = -1;
            Int32 DATA_LAST_MONTH_COLUMN = -1;
            int firstMonth = Convert.ToInt32(_session.PeriodBeginningDate.Substring(4, 2));
            int lastMonth = Convert.ToInt32(periodEnd.Substring(4, 2));
            Int32 nbMonthes = (lastMonth - firstMonth) + 1;
            if (_monthlyExtended){
                DATA_FIRST_MONTH_COLUMN = DATA_FIRST_DATA_COLUMN + 1;
                if (yearN1 > -1){
                    DATA_FIRST_MONTH_COLUMN++;
                }
                DATA_LAST_MONTH_COLUMN = DATA_FIRST_MONTH_COLUMN + (lastMonth - firstMonth);
            }
            #endregion

            #region Headers
            Headers headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), ID_PRODUCT));
            headers.Root.Add(new Header(true, yearN.ToString(), ID_YEAR_N));
            if (_session.PDV){
                headers.Root.Add(new Header(true, string.Format("{0} {1}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), yearN), ID_PDV_YEAR_N));
            }
            //PDM
            if (_session.PDM){
                headers.Root.Add(new Header(true, string.Format("{0} {1}", GestionWeb.GetWebWord(806, _session.SiteLanguage), yearN), ID_PDM_YEAR_N));
            }
            //N-1
            if (_session.ComparativeStudy){
                headers.Root.Add(new Header(true, yearN1.ToString(), ID_YEAR_N1));
                if (_session.PDV){
                    headers.Root.Add(new Header(true, string.Format("{0} {1}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), yearN1), ID_PDV_YEAR_N1));
                }
                //PDM
                if (_session.PDM){
                    headers.Root.Add(new Header(true, string.Format("{0} {1}", GestionWeb.GetWebWord(806, _session.SiteLanguage), yearN1), ID_PDM_YEAR_N1));
                }
                //Evol
                if (_session.Evolution){
                    headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1168, _session.SiteLanguage), ID_EVOL));
                }
            }
            //Months
            if (_monthlyExtended){
                CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
                DateTime cMonth = begin;
                for (int j = 0; j < nbMonthes; j++){
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
            Int32 cId = -1;
            Int32 cLine = -1;
            List<Int64> mainLevelIds = new List<Int64>();
            List<Int64> scdLevelIds = new List<Int64>();
            
            string C1_ID_NAME = (C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.media) ? "ID_M" : "ID_P";
            string C2_ID_NAME = (C2_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product) ? "ID_P" : "ID_M";
            string C1_LABEL_NAME = (C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.media) ? "M" : "P";
            string C2_LABEL_NAME = (C2_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product) ? "P" : "M";
            string C1_IDS_KEY = string.Empty;
            string C2_IDS_KEY = string.Empty;
            List<Double> dMonthes = new List<Double>();
            double invest = 0;
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();
            DataRow currentRow = null;
            dtData = dsData.Tables["CLASSIF2_"+mainLevelsCount+"_"+secondlevelsCount];

            foreach (DataRow row in dtData.Rows){

               

                    #region Main classification init
                    for (int i = 0; i < C1_IDS.Count; i++)
                    {
                        if ((_vehicle == CstDBClassif.Vehicles.names.plurimedia
                                || C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product)
                                && i==0)
                            cId = 0;
                        else if (_vehicle == CstDBClassif.Vehicles.names.plurimedia
                                || C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product)
                            cId = Convert.ToInt32(row[C1_ID_NAME+(i)]);
                        else
                            cId = Convert.ToInt32(row[C1_ID_NAME + (i + 1)]);

                        if (cId != C1_IDS[i]){

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

                            C1_IDS_KEY = string.Empty;
                            for (int j = 0; j < C1_IDS.Count; j++)
                                if ((C1_IDS[j] != -1) && (C1_IDS[j] != 0))
                                    C1_IDS_KEY += C1_IDS[j] + "_";
                            if(C1_IDS_KEY.Length > 0)
                                C1_IDS_KEY = C1_IDS_KEY.Substring(0, C1_IDS_KEY.Length - 1);

                            if (((_vehicle == CstDBClassif.Vehicles.names.plurimedia && c1LineTypes[i] == LineType.total)
                                || C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product)
                                && i==0)
                                currentRow = globalDictionary["TOTAL"]["TOTAL"];
                            else if (_vehicle == CstDBClassif.Vehicles.names.plurimedia
                                || C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product)
                                currentRow = globalDictionary["CLASSIF1_" + i][C1_IDS_KEY];
                            else
                                currentRow = globalDictionary["CLASSIF1_" + (i+1)][C1_IDS_KEY];

                            //Init current line
                            if (((_vehicle == CstDBClassif.Vehicles.names.plurimedia && c1LineTypes[i] == LineType.total)
                                || C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product)
                                && i == 0)
                                C1_LEVELS.Add(new CellLevel(cId, GestionWeb.GetWebWord(805, _session.SiteLanguage), i, cLine, textWrap.NbChar, textWrap.Offset));
                            else if (_vehicle == CstDBClassif.Vehicles.names.plurimedia
                                || C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product)
                                C1_LEVELS.Add(new CellLevel(cId, currentRow[C1_LABEL_NAME + i].ToString(), i, cLine, textWrap.NbChar, textWrap.Offset));
                            else
                                C1_LEVELS.Add(new CellLevel(cId, currentRow[C1_LABEL_NAME + (i + 1)].ToString(), i, cLine, textWrap.NbChar, textWrap.Offset));

                            cLine = tab.AddNewLine(c1LineTypes[i], mainLevelIds, C1_LEVELS[i]);
                            //year N
                            invest = Convert.ToDouble(currentRow["N"]);
                            tab[cLine, 2] = cellFactory.Get(invest);
                            //PDV
                            if (RES_PDV_YEAR_N_INDEX > -1)
                            {
                                if (C1_TYPE == CstClassif.Branch.type.product && i > 0)
                                {
                                    tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(invest, (CellUnit)tab[C1_LEVELS[i - 1].LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                    ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                }
                                else
                                {
                                    tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(invest, null);
                                    ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                }
                            }
                            //PDM
                            if (RES_PDM_YEAR_N_INDEX > -1)
                            {
                                if (C1_TYPE == CstClassif.Branch.type.media && i > 0)
                                {
                                    tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(invest, (CellUnit)tab[C1_LEVELS[i - 1].LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                    ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                }
                                else
                                {
                                    tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(invest, null);
                                    ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                }
                            }
                            //Year N1
                            if (yearN1 > -1)
                            {
                                invest = Convert.ToDouble(currentRow["N1"]);
                                tab[cLine, RES_YEAR_N1_INDEX] = cellFactory.Get(invest);
                                //PDV
                                if (RES_PDV_YEAR_N1_INDEX > -1)
                                {
                                    if (C1_TYPE == CstClassif.Branch.type.product && i > 0)
                                    {
                                        tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(invest, (CellUnit)tab[C1_LEVELS[i - 1].LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                        ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                    }
                                    else
                                    {
                                        tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(invest, null);
                                        ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                    }
                                }
                                //PDM
                                if (RES_PDM_YEAR_N1_INDEX > -1)
                                {
                                    if (C1_TYPE == CstClassif.Branch.type.media && i > 0)
                                    {
                                        tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(invest, (CellUnit)tab[C1_LEVELS[i - 1].LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                        ((CellUnit)tab[cLine, RES_PDM_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                    }
                                    else
                                    {
                                        tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(invest, null);
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
                                    tab[cLine, RES_FIRSTMONTH_INDEX + j] = cellFactory.Get(Convert.ToDouble(currentRow["N_MO_"+ (j+1)]));
                                }
                            }
                        }
                        cLine = C1_LEVELS[i].LineIndexInResultTable;
                        //Advertisers
                        if (_isPersonalized > 0 && currentRow.Table.Columns.Contains("inref"))
                        {
                            ProductClassLineStart ls = (ProductClassLineStart)tab[cLine, 0];
                            if (Convert.ToInt32(currentRow["inref"]) > 0)
                            {
                                ls.SetUniversType(UniversType.reference);
                            }
                            if (Convert.ToInt32(currentRow["incomp"]) > 0)
                            {
                                ls.SetUniversType(UniversType.concurrent);
                            }
                            if (Convert.ToInt32(currentRow["inneutral"]) > 0)
                            {
                                ls.SetUniversType(UniversType.neutral);
                            }
                            if (C1_TYPE == CstClassif.Branch.type.product)
                            {
                                switch (MAIN_LEVELS[i].Id)
                                {
                                    case DetailLevelItemInformation.Levels.advertiser:
                                    case DetailLevelItemInformation.Levels.brand:
                                    case DetailLevelItemInformation.Levels.product:
                                        ls.DisplayPerso = true;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    #endregion

                    #region Second classification treatment
                    for (int i = 0; i < C2_IDS.Count; i++)
                    {
                        cId = Convert.ToInt32(row[C2_ID_NAME + (i + 1)]);

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

                                C1_IDS_KEY = string.Empty;
                                for (int l = 0; l <= j; l++)
                                    if ((mainLevelIds[l] != -1) && (mainLevelIds[l] != 0))
                                        C1_IDS_KEY += mainLevelIds[l] + "_";

                                C2_IDS_KEY = string.Empty;
                                for (int l = 0; l < C2_IDS.Count; l++)
                                    if (C2_IDS[l] != -1)
                                        C2_IDS_KEY += C2_IDS[l] + "_";
                                C2_IDS_KEY = C2_IDS_KEY.Substring(0, C2_IDS_KEY.Length - 1);

                                if (j == 0)
                                    currentRow = globalDictionary["TOTAL_" + (i + 1)][C2_IDS_KEY];
                                else
                                {
                                    if(C1_TYPE== TNS.AdExpress.Constantes.Classification.Branch.type.product
                                        || _vehicle == CstDBClassif.Vehicles.names.plurimedia)
                                    currentRow = globalDictionary["CLASSIF2_" + j + "_" + (i + 1)][C1_IDS_KEY + C2_IDS_KEY];
                                    else currentRow = globalDictionary["CLASSIF2_" + (j +1)+ "_" + (i + 1)][C1_IDS_KEY + C2_IDS_KEY];
                                }

                                List<Int64> parentKeys = mainLevelIds.GetRange(0, j + 1);
                                if ((cLine = tab.Contains(parentKeys, scdLevelIds)) < 0)
                                {
                                    CellLevel p = tab.GetLevel(parentKeys, scdLevelIds.GetRange(0, scdLevelIds.Count - 1));
                                    CellLevel c = new CellLevel(cId, currentRow[C2_LABEL_NAME + (i + 1)].ToString(), p, j, cLine, textWrap.NbChar, textWrap.Offset);
                                    C2_LEVELS.Add(c);
                                    cLine = tab.InsertNewLine((j == 0) ? c2SubTotalLineTypes[i] : c2LineTypes[i], parentKeys, scdLevelIds, c);
                                    //year N
                                    invest = Convert.ToDouble(currentRow["N"]);
                                    tab[cLine, 2] = cellFactory.Get(invest);
                                    //PDV
                                    if (RES_PDV_YEAR_N_INDEX > -1)
                                    {
                                        if (C1_TYPE == CstClassif.Branch.type.product)
                                        {
                                            if (j > 0)
                                            {
                                                tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(invest, (CellUnit)tab[tab.GetLevel(mainLevelIds.GetRange(0, j), scdLevelIds).LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                                ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                            else
                                            {
                                                tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(invest, null);
                                                ((CellUnit)tab[cLine, RES_PDV_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                        }
                                        else
                                        {
                                            tab[cLine, RES_PDV_YEAR_N_INDEX] = new CellPDM(invest, (CellUnit)tab[p.LineIndexInResultTable, RES_YEAR_N_INDEX]);
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
                                                tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(invest, (CellUnit)tab[tab.GetLevel(mainLevelIds.GetRange(0, 1), scdLevelIds).LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                                ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                            else
                                            {
                                                tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(invest, null);
                                                ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                            }
                                        }
                                        else
                                        {
                                            tab[cLine, RES_PDM_YEAR_N_INDEX] = new CellPDM(invest, (CellUnit)tab[p.LineIndexInResultTable, RES_YEAR_N_INDEX]);
                                            ((CellUnit)tab[cLine, RES_PDM_YEAR_N_INDEX]).StringFormat = "{0:percentage}";
                                        }
                                    }
                                    //Year N1
                                    if (yearN1 > -1)
                                    {
                                        invest = Convert.ToDouble(currentRow["N1"]);
                                        tab[cLine, RES_YEAR_N1_INDEX] = cellFactory.Get(invest);
                                        //PDV
                                        if (RES_PDV_YEAR_N1_INDEX > -1)
                                        {
                                            if (C1_TYPE == CstClassif.Branch.type.product)
                                            {
                                                if (j > 0)
                                                {
                                                    tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(invest, (CellUnit)tab[tab.GetLevel(mainLevelIds.GetRange(0, j), scdLevelIds).LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                                    ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                                }
                                                else
                                                {
                                                    tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(invest, null);
                                                    ((CellUnit)tab[cLine, RES_PDV_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                                }
                                            }
                                            else
                                            {
                                                tab[cLine, RES_PDV_YEAR_N1_INDEX] = new CellPDM(invest, (CellUnit)tab[p.LineIndexInResultTable, RES_YEAR_N1_INDEX]);
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
                                                    tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(invest, (CellUnit)tab[tab.GetLevel(mainLevelIds.GetRange(0, 1), scdLevelIds).LineIndexInResultTable, RES_YEAR_N1_INDEX]);
                                                    ((CellUnit)tab[cLine, RES_PDM_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                                }
                                                else
                                                {
                                                    tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(invest, null);
                                                    ((CellUnit)tab[cLine, RES_PDM_YEAR_N1_INDEX]).StringFormat = "{0:percentage}";
                                                }
                                            }
                                            else
                                            {
                                                tab[cLine, RES_PDM_YEAR_N1_INDEX] = new CellPDM(invest, (CellUnit)tab[p.LineIndexInResultTable, RES_YEAR_N1_INDEX]);
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
                                            tab[cLine, RES_FIRSTMONTH_INDEX + m] = cellFactory.Get(Convert.ToDouble(currentRow["N_MO_" + (m + 1)]));
                                        }
                                    }

                                }
                            }
                        }
                        if (_isPersonalized > 0 && currentRow.Table.Columns.Contains("inref"))
                        {
                            for (int j = 0; j < mainLevelIds.Count; j++)
                            {
                                List<Int64> parentKeys = mainLevelIds.GetRange(0, j + 1);
                                cLine = tab.Contains(parentKeys, scdLevelIds);
                                //Advertisers
                                ProductClassLineStart ls = (ProductClassLineStart)tab[cLine, 0];
                                if (Convert.ToInt32(currentRow["inref"]) > 0)
                                {
                                    ls.SetUniversType(UniversType.reference);
                                }
                                if (Convert.ToInt32(currentRow["incomp"]) > 0)
                                {
                                    ls.SetUniversType(UniversType.concurrent);
                                }
                                if (Convert.ToInt32(currentRow["inneutral"]) > 0)
                                {
                                    ls.SetUniversType(UniversType.neutral);
                                }
                                if (C2_TYPE == CstClassif.Branch.type.product)
                                {
                                    switch (SECOND_LEVELS[i].Id)
                                    {
                                        case DetailLevelItemInformation.Levels.advertiser:
                                        case DetailLevelItemInformation.Levels.brand:
                                        case DetailLevelItemInformation.Levels.product:
                                            ls.DisplayPerso = true;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    
                    dMonthes.Clear();
            
                

            }
            #endregion

            return tab;
        }
        #endregion

    }
}
