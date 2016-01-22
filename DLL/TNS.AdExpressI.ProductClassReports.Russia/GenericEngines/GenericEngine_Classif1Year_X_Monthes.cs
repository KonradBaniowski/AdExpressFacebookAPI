#region Information
/*
 * Author : G Ragneau
 * Created on : 24/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.ProductClassReports.GenericEngines;
using CstTblFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables;
using TNS.AdExpressI.ProductClassReports.Exceptions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;
using TNS.AdExpress.Constantes;

namespace TNS.AdExpressI.ProductClassReports.Russia.GenericEngines
{
    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Year X Monthes
    /// </summary>
    public class GenericEngine_Classif1Year_X_Monthes : TNS.AdExpressI.ProductClassReports.GenericEngines.GenericEngine_Classif1Year_X_Monthes
    {

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public GenericEngine_Classif1Year_X_Monthes(WebSession session, int result) : base(session, result) { }
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        /// <param name="plusMonthesCumul">Determine if should compute total periods</param>
        public GenericEngine_Classif1Year_X_Monthes(WebSession session, int result, bool plusMonthesCumul)
            : base(session, result,plusMonthesCumul){
        }
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
            #region Data
            DataTable dtData;
            if (dsData == null || dsData.Tables.Count == 0) return null;
            dtData = dsData.Tables[dsData.Tables.Count - 1];
            if (dtData.Rows.Count <= 0) return null;
            #endregion

            #region Indexes

            #region Personal advertisers
            if (dtData.Columns.Contains("inref"))
            {
                _isPersonalized = 3;
            }
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
            int firstMonth = Convert.ToInt32(_session.PeriodBeginningDate.Substring(4, 2));
            int lastMonth = Convert.ToInt32(periodEnd.Substring(4, 2));
            Int32 nbMonthes = (lastMonth - firstMonth) + 1;
            int monthColumnIndex = 1; 
            int monthIndexStart = _plusMonthesCumul ? 3 : 2;
            int monthIndexStop = _plusMonthesCumul ? nbMonthes+2 : nbMonthes + 1;
            Int32 RES_YEAR_N_OFFSET = 1;
            List<Int64> keyYearN = new List<Int64>(); keyYearN.Add(ID_YEAR_N);
            Int32 RES_YEAR_N1_OFFSET = (yearN1 > 0) ? 2 : -1;
            List<Int64> keyYearN1 = new List<Int64>(); keyYearN1.Add(ID_YEAR_N1);
            Int32 RES_EVOL_OFFSET = (_session.Evolution && RES_YEAR_N1_OFFSET > 0) ? RES_YEAR_N1_OFFSET + 1 : -1;
            List<Int64> keyEvol = new List<Int64>(); keyEvol.Add(ID_EVOL);
            Int32 RES_PDV_N_OFFSET = -1;
            List<Int64> keyPdvYearN = new List<Int64>(); keyPdvYearN.Add(ID_PDV_N);
            Int32 RES_PDV_N1_OFFSET = -1;
            List<Int64> keyPdvYearN1 = new List<Int64>(); keyPdvYearN1.Add(ID_PDV_N1);
            Int32 RES_PDM_N_OFFSET = -1;
            List<Int64> keyPdmYearN = new List<Int64>(); keyPdmYearN.Add(ID_PDM_N);
            Int32 RES_PDM_N1_OFFSET = -1;
            List<Int64> keyPdmYearN1 = new List<Int64>(); keyPdmYearN1.Add(ID_PDM_N1);
            switch (_tableType)
            {
                case (int)CstTblFormat.productYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Mensual:
                    RES_PDV_N_OFFSET = (_session.PDV) ? Math.Max(RES_YEAR_N_OFFSET, Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET)) + 1 : -1;
                    RES_PDV_N1_OFFSET = (yearN1 > 0 && _session.PDV) ? RES_PDV_N_OFFSET + 1 : -1;
                    break;
                case (int)CstTblFormat.mediaYear_X_Mensual:
                case (int)CstTblFormat.mediaYear_X_Cumul:
                    RES_PDM_N_OFFSET = (_session.PDM) ? Math.Max(RES_YEAR_N_OFFSET, Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET)) + 1 : -1;
                    RES_PDM_N1_OFFSET = (yearN1 > 0 && _session.PDM) ? RES_PDM_N_OFFSET + 1 : -1;
                    break;
                default:
                    throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
            }
            string labelEvol = GestionWeb.GetWebWord(1168, _session.SiteLanguage);
            string labelPDMN = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN);
            string labelPDMN1 = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN1);
            string labelPDVN = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN);
            string labelPDVN1 = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN1);
            #endregion

            #endregion

            #region Build headers
            Headers headers = new Headers();
            switch (_tableType)
            {
                case (int)CstTblFormat.productYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Mensual:
                    headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), ID_PRODUCT));
                    break;
                case (int)CstTblFormat.mediaYear_X_Mensual:
                case (int)CstTblFormat.mediaYear_X_Cumul:
                    headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1357, _session.SiteLanguage), ID_PRODUCT));
                    break;
                default:
                    throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
            }
            //TOTAL
            if (_plusMonthesCumul)
                headers.Root.Add(new Header(true, GestionWeb.GetWebWord(805, _session.SiteLanguage).ToUpper(), ID_TOTAL));
            //Months			
            CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            DateTime cMonth = begin;
            //Dictionary<Int32, Header> RES_MONTHES_HEADERS = new Dictionary<Int32, Header>();
            Header[] MONTHES_HEADERS_IDS = new Header[nbMonthes];
            for (int j = 0; j < nbMonthes; j++)
            {
                MONTHES_HEADERS_IDS[j] = new Header(true, cMonth.ToString("MMMM", cInfo), cMonth.Month);
                headers.Root.Add((Header)MONTHES_HEADERS_IDS[j]);
                cMonth = cMonth.AddMonths(1);
            }
            #endregion

            #region Init Result Table
            tab = new ProductClassResultTable(dtData.Rows.Count * 2, headers);
            tab.SortSubLevels = false;
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();
            CellUnit cell = cellFactory.Get(0.0);
            cell.DisplayContent = false;
            CellUnitFactory cellHiddenFactory = new CellUnitFactory(cell);
            string C1_ID_NAME = string.Empty, C1_IDS_KEY = string.Empty;
            string C1_LABEL_NAME = string.Empty;
            int levelsCount = 0;
            List<DetailLevelItemInformation> levels = null;
			
			switch (_tableType) {
				case (int)CstTblFormat.productYear_X_Cumul:
				case (int)CstTblFormat.productYear_X_Mensual:
					levels = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
					levels.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sector));
                    C1_ID_NAME = "ID_P";
                    C1_LABEL_NAME = "P";
                    levelsCount = levels.Count - 1;
					break;
				case (int)CstTblFormat.mediaYear_X_Mensual:
				case (int)CstTblFormat.mediaYear_X_Cumul:
					levels = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
                    C1_ID_NAME = "ID_M";
                    C1_LABEL_NAME = "M";
                    levelsCount = levels.Count;
					break;
				default:
					throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
			}
            #endregion

            Dictionary<string, Dictionary<string, DataRow>> globalDictionary = CommonFunctions.InitDictionariesData(dsData, levelsCount, _tableType);

            #region Total line
            CellLevel[] parents = new CellLevel[levelsCount + 1];
            List<Int64> keys = new List<Int64>();
            Double investN = 0, investN1 = 0;
            DataRow currentRow = globalDictionary["TOTAL"]["TOTAL"];
            keys.Add(ID_TOTAL);
            //Total
            int cLine = 0;
            cLine = tab.AddNewLine(LineType.total, keys, parents[0] = new CellLevel(ID_TOTAL, GestionWeb.GetWebWord(1401, _session.SiteLanguage), 0, cLine, textWrap.NbChar, textWrap.Offset));
            //YearN
            tab.AddNewLine(LineType.subTotal1, keys, keyYearN, new CellLevel(ID_YEAR_N, yearN.ToString(), (CellLevel)tab[cLine, 1], 0, cLine + RES_YEAR_N_OFFSET, textWrap.NbChar, textWrap.Offset));
            if (_plusMonthesCumul)
            {
                //Total
                tab[cLine, 2] = cellHiddenFactory.Get(0.0);
                //YearN
                investN = Convert.ToDouble(currentRow["N"]);
                tab[cLine + RES_YEAR_N_OFFSET, 2] = cellFactory.Get(investN);
            }
            //YearN1
            if (RES_YEAR_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyYearN1, new CellLevel(ID_YEAR_N1, yearN1.ToString(), 0, cLine + RES_YEAR_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                if (_plusMonthesCumul)
                {
                    //YearN1
                    investN1 = Convert.ToDouble(currentRow["N1"]);
                    tab[cLine + RES_YEAR_N1_OFFSET, 2] = cellFactory.Get(investN1);
                }
            }
            //Evol
            if (RES_EVOL_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyEvol, new CellLevel(ID_EVOL, labelEvol, 0, cLine + RES_EVOL_OFFSET, textWrap.NbChar, textWrap.Offset));
                if (_plusMonthesCumul)
                {
                    tab[cLine + RES_EVOL_OFFSET, 2] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, 2], tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                    ((CellEvol)tab[cLine + RES_EVOL_OFFSET, 2]).StringFormat = "{0:percentage}";
                }
            }
            //PDV N
            if (RES_PDV_N_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdvYearN, new CellLevel(ID_PDV_N, labelPDVN, 0, cLine + RES_PDV_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                if (_plusMonthesCumul)
                {
                    tab[cLine + RES_PDV_N_OFFSET, 2] = new CellPDM(investN, null);
                    ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, 2]).StringFormat = "{0:percentage}";
                }
            }
            //PDV N1
            if (RES_PDV_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdvYearN1, new CellLevel(ID_PDV_N1, labelPDVN1, 0, cLine + RES_PDV_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                if (_plusMonthesCumul)
                {
                    tab[cLine + RES_PDV_N1_OFFSET, 2] = new CellPDM(investN1, null);
                    ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
                }
            }
            //PDM N
            if (RES_PDM_N_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdmYearN, new CellLevel(ID_PDM_N, labelPDMN, 0, cLine + RES_PDM_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                if (_plusMonthesCumul)
                {
                    tab[cLine + RES_PDM_N_OFFSET, 2] = new CellPDM(investN, null);
                    ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, 2]).StringFormat = "{0:percentage}";
                }
            }
            //PDM N1
            if (RES_PDM_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdmYearN1, new CellLevel(ID_PDM_N1, labelPDMN1, 0, cLine + RES_PDM_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                if (_plusMonthesCumul)
                {
                    tab[cLine + RES_PDM_N1_OFFSET, 2] = new CellPDM(investN1, null);
                    ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
                }
            }
            monthColumnIndex = 1;
            for (int i = monthIndexStart; i <= monthIndexStop; i++)
            {
                //Total
                tab[cLine, i] = cellHiddenFactory.Get(0.0);
                investN = Convert.ToDouble(currentRow["N_MO_" + monthColumnIndex]);
                //YearN
                tab[cLine + RES_YEAR_N_OFFSET, i] = cellFactory.Get(investN);
                //YearN1
                if (RES_YEAR_N1_OFFSET > 0)
                {
                    investN1 = Convert.ToDouble(currentRow["N1_MO_" + monthColumnIndex]);
                    tab[cLine + RES_YEAR_N1_OFFSET, i] = cellFactory.Get(investN1);
                }
                //Evol
                if (RES_EVOL_OFFSET > 0)
                {
                    tab[cLine + RES_EVOL_OFFSET, i] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, i], tab[cLine + RES_YEAR_N1_OFFSET, i]);
                    ((CellEvol)tab[cLine + RES_EVOL_OFFSET, i]).StringFormat = "{0:percentage}";
                }
                //PDV N
                if (RES_PDV_N_OFFSET > 0)
                {
                    tab[cLine + RES_PDV_N_OFFSET, i] = new CellPDM(investN, null);
                    ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, i]).StringFormat = "{0:percentage}";
                }
                //PDV N1
                if (RES_PDV_N1_OFFSET > 0)
                {
                    tab[cLine + RES_PDV_N1_OFFSET, i] = new CellPDM(investN1, null);
                    ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, i]).StringFormat = "{0:percentage}";
                }
                //PDM N
                if (RES_PDM_N_OFFSET > 0)
                {
                    tab[cLine + RES_PDM_N_OFFSET, i] = new CellPDM(investN, null);
                    ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, i]).StringFormat = "{0:percentage}";
                }
                //PDM N1
                if (RES_PDM_N1_OFFSET > 0)
                {
                    tab[cLine + RES_PDM_N1_OFFSET, i] = new CellPDM(investN1, null);
                    ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, i]).StringFormat = "{0:percentage}";
                }
                monthColumnIndex++;
            }
            #endregion

            #region Fill table
            Int64 cId = -1;
            LineType[] lTypes = new LineType[4] { LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
            LineType[] lSubTypes = new LineType[4] { LineType.level5, LineType.level6, LineType.level7, LineType.level8 };
            Double valueN = 0;
            Double valueN1 = 0;

            foreach (DataRow row in dtData.Rows)
            {
                for (int i = 0; i < levelsCount; i++)
                {
                    cId = Convert.ToInt32(row[C1_ID_NAME + (i+1)]);
                    if (parents[i + 1] == null || cId != parents[i + 1].Id)
                    {
                        for (int j = parents.Length - 1; j > i; j--)
                        {
                            parents[j] = null;
                            if (keys.Count > j)
                            {
                                keys.RemoveAt(j);
                            }
                        }
                        keys.Add(cId);

                        C1_IDS_KEY = string.Empty;
                        for (int j = 0; j < keys.Count; j++)
                            if ((keys[j] != -1) && (keys[j] != -2))
                                C1_IDS_KEY += keys[j] + "_";
                        if (C1_IDS_KEY.Length > 0)
                            C1_IDS_KEY = C1_IDS_KEY.Substring(0, C1_IDS_KEY.Length - 1);

                        currentRow = globalDictionary["CLASSIF1_" + (i + 1)][C1_IDS_KEY];

                        #region Cells
                        //Total
                        cLine = tab.AddNewLine(lTypes[i], keys, parents[i + 1] = new CellLevel(cId, currentRow[C1_LABEL_NAME + (i + 1)].ToString(), (CellLevel)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, 1], i + 1, cLine, textWrap.NbChar, textWrap.Offset));
                        //YearN
                        tab.AddNewLine(lSubTypes[i], keys, keyYearN, new CellLevel(ID_YEAR_N, yearN.ToString(), (CellLevel)tab[cLine, 1], i + 1, cLine + RES_YEAR_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                        if (_plusMonthesCumul)
                        {
                            //Total
                            tab[cLine, 2] = cellHiddenFactory.Get(0.0);
                            investN = Convert.ToDouble(currentRow["N"]);
                            tab[cLine + RES_YEAR_N_OFFSET, 2] = cellFactory.Get(investN);
                        }
                        //YearN1
                        if (RES_YEAR_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyYearN1, new CellLevel(ID_YEAR_N1, yearN1.ToString(), (CellLevel)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, 1], i + 1, cLine + RES_YEAR_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                            if (_plusMonthesCumul)
                            {
                                investN1 = Convert.ToDouble(currentRow["N1"]);
                                tab[cLine + RES_YEAR_N1_OFFSET, 2] = cellFactory.Get(investN1);
                            }
                        }
                        //Evol
                        if (RES_EVOL_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyEvol, new CellLevel(ID_EVOL, labelEvol, i + 1, cLine + RES_EVOL_OFFSET, textWrap.NbChar, textWrap.Offset));
                            if (_plusMonthesCumul)
                            {
                                tab[cLine + RES_EVOL_OFFSET, 2] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, 2], tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                                ((CellEvol)tab[cLine + RES_EVOL_OFFSET, 2]).StringFormat = "{0:percentage}";
                            }
                        }
                        //PDV N
                        if (RES_PDV_N_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdvYearN, new CellLevel(ID_PDV_N, labelPDVN, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDV_N_OFFSET, 1], i + 1, cLine + RES_PDV_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                            if (_plusMonthesCumul)
                            {
                                tab[cLine + RES_PDV_N_OFFSET, 2] = new CellPDM(investN, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, 2]);
                                ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, 2]).StringFormat = "{0:percentage}";
                            }
                        }
                        //PDV N1
                        if (RES_PDV_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdvYearN1, new CellLevel(ID_PDV_N1, labelPDVN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDV_N1_OFFSET, 1], i + 1, cLine + RES_PDV_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                            if (_plusMonthesCumul)
                            {
                                tab[cLine + RES_PDV_N1_OFFSET, 2] = new CellPDM(investN1, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, 2]);
                                ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
                            }
                        }
                        //PDM N
                        if (RES_PDM_N_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdmYearN, new CellLevel(ID_PDM_N, labelPDMN, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDM_N_OFFSET, 1], i + 1, cLine + RES_PDM_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                            if (_plusMonthesCumul)
                            {
                                tab[cLine + RES_PDM_N_OFFSET, 2] = new CellPDM(investN, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, 2]);
                                ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, 2]).StringFormat = "{0:percentage}";
                            }
                        }
                        //PDM N1
                        if (RES_PDM_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdmYearN1, new CellLevel(ID_PDM_N1, labelPDMN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDM_N1_OFFSET, 1], i + 1, cLine + RES_PDM_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                            if (_plusMonthesCumul)
                            {
                                tab[cLine + RES_PDM_N1_OFFSET, 2] = new CellPDM(investN1, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, 2]);
                                ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
                            }
                        }
                        monthColumnIndex = 1;
                        for (int k = monthIndexStart; k <= monthIndexStop; k++)
                        {
                            //Total
                            tab[cLine, k] = cellHiddenFactory.Get(0.0);
                            //YearN
                            investN = Convert.ToDouble(currentRow["N_MO_" + monthColumnIndex]);
                            tab[cLine + RES_YEAR_N_OFFSET, k] = cellFactory.Get(investN);
                            //YearN1
                            if (RES_YEAR_N1_OFFSET > 0)
                            {
                                investN1 = Convert.ToDouble(currentRow["N1_MO_" + monthColumnIndex]);
                                tab[cLine + RES_YEAR_N1_OFFSET, k] = cellFactory.Get(investN1);
                            }
                            //Evol
                            if (RES_EVOL_OFFSET > 0)
                            {
                                tab[cLine + RES_EVOL_OFFSET, k] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, k], tab[cLine + RES_YEAR_N1_OFFSET, k]);
                                ((CellEvol)tab[cLine + RES_EVOL_OFFSET, k]).StringFormat = "{0:percentage}";
                            }
                            //PDV N
                            if (RES_PDV_N_OFFSET > 0)
                            {
                                tab[cLine + RES_PDV_N_OFFSET, k] = new CellPDM(investN, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, k]);
                                ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, k]).StringFormat = "{0:percentage}";
                            }
                            //PDV N1
                            if (RES_PDV_N1_OFFSET > 0)
                            {
                                tab[cLine + RES_PDV_N1_OFFSET, k] = new CellPDM(investN1, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, k]);
                                ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, k]).StringFormat = "{0:percentage}";
                            }
                            //PDM N
                            if (RES_PDM_N_OFFSET > 0)
                            {
                                tab[cLine + RES_PDM_N_OFFSET, k] = new CellPDM(investN, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, k]);
                                ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, k]).StringFormat = "{0:percentage}";
                            }
                            //PDM N1
                            if (RES_PDM_N1_OFFSET > 0)
                            {
                                tab[cLine + RES_PDM_N1_OFFSET, k] = new CellPDM(investN1, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, k]);
                                ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, k]).StringFormat = "{0:percentage}";
                            }
                            monthColumnIndex++;
                        }
                        #endregion

                    }
                }

                #region Advertisers univers
                int classifIndex = levels.Count -1;
                if (_isPersonalized > 0)
                {
                    classifIndex = levelsCount;
                    for (int i = parents.Length - 1; i >= 0; i--)
                    {
                        C1_IDS_KEY = string.Empty;
                        for (int j = 0; j <= i; j++)
                            if ((keys[j] != -1) && (keys[j] != -2))
                                C1_IDS_KEY += keys[j] + "_";
                        if (C1_IDS_KEY.Length > 0)
                            C1_IDS_KEY = C1_IDS_KEY.Substring(0, C1_IDS_KEY.Length - 1);

                        if (C1_IDS_KEY.Length > 0)
                        {
                            currentRow = globalDictionary["CLASSIF1_" + classifIndex][C1_IDS_KEY];
                            classifIndex--;
                            SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable, currentRow, levels[i].Id);
                            SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, currentRow, levels[i].Id);
                            //YearN1
                            if (RES_YEAR_N1_OFFSET > 0)
                            {
                                SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, currentRow, levels[i].Id);
                            }
                            //Evol
                            if (RES_EVOL_OFFSET > 0)
                            {
                                SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_EVOL_OFFSET, currentRow, levels[i].Id);
                            }
                            //PDV N
                            if (RES_PDV_N_OFFSET > 0)
                            {
                                SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDV_N_OFFSET, currentRow, levels[i].Id);
                            }
                            //PDV N1
                            if (RES_PDV_N1_OFFSET > 0)
                            {
                                SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDV_N1_OFFSET, currentRow, levels[i].Id);
                            }
                            //PDM N
                            if (RES_PDM_N_OFFSET > 0)
                            {
                                SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDM_N_OFFSET, currentRow, levels[i].Id);
                            }
                            //PDM N1
                            if (RES_PDM_N1_OFFSET > 0)
                            {
                                SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDM_N1_OFFSET, currentRow, levels[i].Id);
                            }
                        }
                    }
                }
                #endregion

            }
            #endregion

            return tab;
        }
        #endregion

    }
}
