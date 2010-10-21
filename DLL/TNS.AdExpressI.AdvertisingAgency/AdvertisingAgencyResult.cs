﻿using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.AdvertisingAgency.DAL;
using System.Reflection;
using System.Data;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.AdvertisingAgency.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;


namespace TNS.AdExpressI.AdvertisingAgency
{
    /// <summary>
    /// Default Advertising Agency Reports
    /// </summary>
    public abstract class AdvertisingAgencyResult : IAdvertisingAgencyResult
    {

        #region Constants
        private const Int32 ID_PRODUCT = -1;
        private const Int32 ID_TOTAL = -2;
        private const Int32 ID_YEAR_N = -3;
        private const Int32 ID_YEAR_N1 = -4;
        private const Int32 ID_EVOL = -5;
        private const Int32 ID_PDV_N = -6;
        private const Int32 ID_PDV_N1 = -7;
        private const Int32 ID_PDM_N = -8;
        private const Int32 ID_PDM_N1 = -9;
        #endregion

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current Module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
        /// <summary>
        /// Media Schedule Period Management regarding to global date selection
        /// </summary>
        protected MediaSchedulePeriod _period = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        public AdvertisingAgencyResult(WebSession session)
        {
            DateTime begin;
            DateTime end;

            _session = session;
            _module = ModulesList.GetModule(session.CurrentModule);
            begin = WebFunctions.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            end = WebFunctions.Dates.getPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
            if (_session.DetailPeriod == CstPeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
            {
                _session.DetailPeriod = CstPeriod.DisplayLevel.monthly;
            }
            _period = new MediaSchedulePeriod(begin, end, _session.DetailPeriod);
        }
        #endregion

        #region GetResult
        /// <summary>
        /// Compute specified result
        /// </summary>
        /// <param name="result">Type of result to compute</param>
        /// <returns>Computed data</returns>
        public ResultTable GetResult()
        {
            
            #region Data
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the advertising agency result"));
            object[] parameters = new object[2];
            parameters[0] = _session;
            parameters[1] = _period;
            IAdvertisingAgencyDAL advertisingAgencyDAL = (IAdvertisingAgencyDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            DataSet ds = advertisingAgencyDAL.GetData();
            DataTable dtData = ds.Tables[0];
            if (dtData.Rows.Count <= 0) return null;
            #endregion

            #region Indexes

            #region Compute indexes : first numerical column, product indexes, media indexes
            int FIRST_DATA_INDEX = 0;
            List<Int32> DATA_PRODUCT_INDEXES = new List<int>();
            List<Int32> DATA_MEDIA_INDEXES = new List<int>();
            bool firstMedia = true;
            for (int i = 0; i < dtData.Columns.Count; i = i + 2)
            {
                if (dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0)
                {
                        DATA_MEDIA_INDEXES.Add(i);
                }
                else if (dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0)
                {
                    DATA_PRODUCT_INDEXES.Add(i);
                }
                else
                {
                    FIRST_DATA_INDEX = i;
                    break;
                }
            }
            #endregion

            #region Periods
            MediaSchedulePeriod comparativePeriod = new MediaSchedulePeriod(_period.Begin.AddMonths(-12), _period.End.AddMonths(-12), _session.DetailPeriod);
            DateTime begin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            DateTime periodEndDate = FctUtilities.Dates.getPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
            int yearN = Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4));
            int yearN1 = _session.ComparativeStudy ? yearN - 1 : -1;
            Int32 DATA_YEAR_N = FIRST_DATA_INDEX;
            Int32 DATA_YEAR_N1 = (yearN1 > 0) ? FIRST_DATA_INDEX + 1 : -1;
            Int32 RES_YEAR_N_OFFSET = 1;
            List<Int64> keyYearN = new List<Int64>(); keyYearN.Add(ID_YEAR_N);
            Int32 RES_YEAR_N1_OFFSET = (yearN1 > 0) ? 2 : -1;
            List<Int64> keyYearN1 = new List<Int64>(); keyYearN1.Add(ID_YEAR_N1);
            Int32 RES_EVOL_OFFSET = (_session.Evolution && RES_YEAR_N1_OFFSET > 0) ? RES_YEAR_N1_OFFSET + 1 : -1;
            List<Int64> keyEvol = new List<Int64>(); keyEvol.Add(ID_EVOL);
            Int32 RES_PDV_N_OFFSET = (_session.PDV) ? Math.Max(RES_YEAR_N_OFFSET, Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET)) + 1 : -1;
            List<Int64> keyPdvYearN = new List<Int64>(); keyPdvYearN.Add(ID_PDV_N);
            Int32 RES_PDV_N1_OFFSET = (yearN1 > 0 && _session.PDV) ? RES_PDV_N_OFFSET + 1 : -1;
            List<Int64> keyPdvYearN1 = new List<Int64>(); keyPdvYearN1.Add(ID_PDV_N1);
            Int32 RES_PDM_N_OFFSET = (_session.PDM) ? Math.Max(RES_PDV_N1_OFFSET, Math.Max(Math.Max(RES_YEAR_N_OFFSET, RES_PDV_N_OFFSET), Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET))) + 1 : -1;
            List<Int64> keyPdmYearN = new List<Int64>(); keyPdmYearN.Add(ID_PDM_N);
            Int32 RES_PDM_N1_OFFSET = (yearN1 > 0 && _session.PDM) ? RES_PDM_N_OFFSET + 1 : -1;
            List<Int64> keyPdmYearN1 = new List<Int64>(); keyPdmYearN1.Add(ID_PDM_N1);
            string labelN = FctUtilities.Dates.getPeriodLabel(_session, _session.PeriodBeginningDate, _session.PeriodEndDate);
            string labelN1 = FctUtilities.Dates.getPeriodLabel(_session, comparativePeriod.Begin.ToString("yyyyMMdd"), comparativePeriod.End.ToString("yyyyMMdd"));
            string labelEvol = GestionWeb.GetWebWord(1168, _session.SiteLanguage);
            string labelPDMN = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), labelN);
            string labelPDMN1 = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), labelN1);
            string labelPDVN = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), labelN);
            string labelPDVN1 = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), labelN1);
            #endregion

            #endregion

            #region Build headers
            Headers headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), ID_PRODUCT));
            string vehicleLabel = dtData.Rows[0]["M1"].ToString();
            string[] listVehicles = null;
            int optionsIndex = 2;
            listVehicles = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess).Split(new char[] { ',' });

            if (listVehicles.Length > 1)
            {
                headers.Root.Add(new Header(true, GestionWeb.GetWebWord(210, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                headers.Root[1].Add(new Header(true, labelN, 1));
                if(_session.ComparativeStudy)
                    headers.Root[1].Add(new Header(true, labelN1, optionsIndex++));
                if (_session.Evolution)
                    headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(1207, _session.SiteLanguage), optionsIndex++));
                if (_session.PDM)
                {
                    headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                    if(_session.ComparativeStudy)
                        headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex++));
                }
                if (_session.PDV)
                {
                    headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                    if(_session.ComparativeStudy)
                        headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex));
                }
            }
                
            //Go threw data to extract media levels
            string sortStr = "";
            switch (_session.PreformatedMediaDetail)
            {
                case CstFormat.PreformatedMediaDetails.vehicle:
                case CstFormat.PreformatedMediaDetails.category:
                case CstFormat.PreformatedMediaDetails.Media:
                case CstFormat.PreformatedMediaDetails.mediaSeller:
                    sortStr = "M1,ID_M1";
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategory:
                case CstFormat.PreformatedMediaDetails.vehicleMedia:
                case CstFormat.PreformatedMediaDetails.vehicleMediaSeller:
                case CstFormat.PreformatedMediaDetails.mediaSellerVehicle:
                case CstFormat.PreformatedMediaDetails.mediaSellerMedia:
                case CstFormat.PreformatedMediaDetails.mediaSellerCategory:
                    sortStr = "M1,ID_M1,M2,ID_M2";
                    break;
                default:
                    throw new AdvertisingAgencyException("Detail format " + _session.PreformatedMediaDetail.ToString() + " unvalid.");
            }
            DataRow[] dtMedias = dtData.Select("", sortStr);

            Dictionary<string, Header> RES_MEDIA_HEADERS = new Dictionary<string, Header>();
            Dictionary<string, HeaderBase> RES_MEDIA_SUBTOTAL = new Dictionary<string, HeaderBase>();
            HeaderBase[] MEDIA_IDS = new HeaderBase[DATA_MEDIA_INDEXES.Count];
            List<HeaderBase> SUB_TOTALS = new List<HeaderBase>();
            int tableIndex = 0;
            List<long> cId = new List<long>();
            for (int i = 0; i < DATA_MEDIA_INDEXES.Count; i++)
                cId.Add(0);
            long oldcId = -1;
            List<string> labelsList = new List<string>();

            HeaderBase pHeaderBase = headers.Root;
            foreach (DataRow row in dtMedias)
            {

                for (int i = 0; i < DATA_MEDIA_INDEXES.Count; i++)
                {
                    if(i == 0) oldcId = cId[0];
                    cId[i] = Convert.ToInt64(row[DATA_MEDIA_INDEXES[i]]);


                    if (MEDIA_IDS[i] == null || (cId[i] != MEDIA_IDS[i].Id || (i>0 && oldcId != cId[0])))
                    {
                        if (i < DATA_MEDIA_INDEXES.Count - 1)
                        {
                            MEDIA_IDS[i] = new HeaderGroup(row[DATA_MEDIA_INDEXES[i] + 1].ToString(), cId[i]);
                            SUB_TOTALS.Add(((HeaderGroup)MEDIA_IDS[i]).AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), ID_TOTAL));
                            MEDIA_IDS[i][0].Add(new Header(true, labelN, 1));
                            optionsIndex = 2;
                            if(_session.ComparativeStudy)
                                MEDIA_IDS[i][0].Add(new Header(true, labelN1, optionsIndex++));
                            if (_session.Evolution)
                                MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(1207, _session.SiteLanguage), optionsIndex++));
                            if (_session.PDM)
                            {
                                MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                                if(_session.ComparativeStudy)
                                    MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex++));
                            }
                            if (_session.PDV)
                            {
                                MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                                if(_session.ComparativeStudy)
                                    MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex));
                            }

                            if (i > 0)
                            {
                                MEDIA_IDS[i - 1].Add(MEDIA_IDS[i]);
                            }
                            else
                            {
                                headers.Root.Add(MEDIA_IDS[i]);
                            }
                            pHeaderBase = MEDIA_IDS[i];
                        }
                        else
                        {
                            labelsList = new List<string>();
                            if (i == 0) tableIndex = i;
                            else tableIndex = i - 1;
                            MEDIA_IDS[i] = new Header(row[DATA_MEDIA_INDEXES[i] + 1].ToString(), cId[i]);
                            MEDIA_IDS[i].Add(new Header(true, labelN, 1));
                            RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_Period1", (Header)MEDIA_IDS[i][0]);
                            labelsList.Add("Period1");
                            optionsIndex = 2;
                            if (_session.ComparativeStudy)
                            {
                                MEDIA_IDS[i].Add(new Header(true, labelN1, optionsIndex));
                                RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_Period2", (Header)MEDIA_IDS[i][optionsIndex-1]);
                                optionsIndex++;
                                labelsList.Add("Period2");
                            }
                            if (_session.Evolution)
                            {
                                MEDIA_IDS[i].Add(new Header(true, GestionWeb.GetWebWord(1207, _session.SiteLanguage), optionsIndex));
                                RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_Evolution", (Header)MEDIA_IDS[i][optionsIndex-1]);
                                optionsIndex++;
                                labelsList.Add("Evolution");
                            }
                            if (_session.PDM)
                            {
                                MEDIA_IDS[i].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex));
                                RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_PDM", (Header)MEDIA_IDS[i][optionsIndex-1]);
                                optionsIndex++;
                                labelsList.Add("PDM");
                                if (_session.ComparativeStudy)
                                {
                                    MEDIA_IDS[i].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex));
                                    RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_PDM1", (Header)MEDIA_IDS[i][optionsIndex - 1]);
                                    optionsIndex++;
                                    labelsList.Add("PDM1");
                                }
                            }
                            if (_session.PDV)
                            {
                                MEDIA_IDS[i].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex));
                                RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_PDV", (Header)MEDIA_IDS[i][optionsIndex-1]);
                                optionsIndex++;
                                labelsList.Add("PDV");
                                if (_session.ComparativeStudy) {
                                    MEDIA_IDS[i].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex));
                                    RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_PDV1", (Header)MEDIA_IDS[i][optionsIndex - 1]);
                                    labelsList.Add("PDV1");
                                }
                            }

                            //RES_MEDIA_HEADERS.Add(cId, (Header)MEDIA_IDS[i]);
                            pHeaderBase.Add(MEDIA_IDS[i]);
                            if (pHeaderBase is HeaderGroup)
                            {
                                RES_MEDIA_SUBTOTAL.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i], SUB_TOTALS[SUB_TOTALS.Count - 1]);
                                //for(int mediaIndex=0; mediaIndex < MEDIA_IDS[i].Count; mediaIndex++)
                                    //RES_MEDIA_SUBTOTAL.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_" + labelsList[mediaIndex], SUB_TOTALS[SUB_TOTALS.Count - 1]);
                            }
                        }
                    }
                }

            }
            #endregion

            #region Init Result Table
            ResultTable tab = new ResultTable(dtData.Rows.Count * 2, headers);
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();
            CellUnit cell = cellFactory.Get(0.0);
            cell.DisplayContent = false;
            CellUnitFactory cellHiddenFactory = new CellUnitFactory(cell);
            CellLevel[] parents = new CellLevel[DATA_PRODUCT_INDEXES.Count + 1];
            #endregion

            //tab.AddNewLine(LineType.total);
            //for (int i = 1; i <= tab.ColumnsNumber - 2; i++)
            //    tab[0, i] = new CellLabel("jlkkjljkl");

            #region Total line
            List<Int64> keys = new List<Int64>();
            keys.Add(ID_TOTAL);
            //Total
            int cLine = 0;
            int columnIndex = 1;


            cLine = tab.AddNewLine(LineType.total);
            //Total
            tab[cLine, columnIndex] = parents[0] = new CellLevel(ID_TOTAL, GestionWeb.GetWebWord(1401, _session.SiteLanguage), 0, cLine);
            for (int k = columnIndex; k <= tab.DataColumnsNumber - labelsList.Count; k += labelsList.Count)
            {
                //YearN
                tab[cLine, k + RES_YEAR_N_OFFSET] = cellFactory.Get(0.0);
                //YearN1
                if (RES_YEAR_N1_OFFSET > 0)
                {
                    tab[cLine, k + RES_YEAR_N1_OFFSET] = cellFactory.Get(0.0);
                }
                //Evol
                if (RES_EVOL_OFFSET > 0)
                {
                    tab[cLine, k + RES_EVOL_OFFSET] = new CellEvol(tab[cLine, k + RES_YEAR_N_OFFSET], tab[cLine, k + RES_YEAR_N1_OFFSET]);
                    ((CellEvol)tab[cLine, k + RES_EVOL_OFFSET]).StringFormat = "{0:percentage}";
                }
                //PDV N
                if (RES_PDV_N_OFFSET > 0)
                {
                    tab[cLine, k + RES_PDV_N_OFFSET] = new CellPDM(0.0, null);
                    ((CellPDM)tab[cLine, k + RES_PDV_N_OFFSET]).StringFormat = "{0:percentage}";
                }
                //PDV N1
                if (RES_PDV_N1_OFFSET > 0)
                {
                    tab[cLine, k + RES_PDV_N1_OFFSET] = new CellPDM(0.0, null);
                    ((CellPDM)tab[cLine, k + RES_PDV_N1_OFFSET]).StringFormat = "{0:percentage}";
                }
            }
            #endregion

            #region Fill table
            LineType[] lTypes = new LineType[4] { LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
            LineType[] lSubTypes = new LineType[4] { LineType.level5, LineType.level6, LineType.level7, LineType.level8 };
            Double valueN = 0;
            Double valueN1 = 0;
            Int32 subTotalIndex = -1;
            Int64 columnId = 0;
            string columnKey = string.Empty;
            List<DetailLevelItemInformation> levels = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
            levels.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sector));

            foreach (DataRow row in dtData.Rows)
            {
                for (int i = 0; i < DATA_PRODUCT_INDEXES.Count; i++)
                {
                    columnId = Convert.ToInt64(row[DATA_PRODUCT_INDEXES[i]]);
                    if (parents[i + 1] == null || columnId != parents[i + 1].Id)
                    {
                        for (int j = parents.Length - 1; j > i; j--)
                        {
                            parents[j] = null;
                            if (keys.Count > j)
                            {
                                keys.RemoveAt(j);
                            }

                        }
                        keys.Add(columnId);

                        #region Init cells
                        //Total
                        cLine = tab.AddNewLine(lTypes[i]);
                        tab[cLine, 1] = new CellLevel(columnId, row[DATA_PRODUCT_INDEXES[i] + 1].ToString(), (CellLevel)tab[parents[0].LineIndexInResultTable, 1], i + 1, cLine);
                        for (int k = columnIndex; k <= tab.DataColumnsNumber - labelsList.Count; k += labelsList.Count)
                        {
                            //YearN
                            tab[cLine, k + RES_YEAR_N_OFFSET] = cellFactory.Get(0.0);
                            //YearN1
                            if (RES_YEAR_N1_OFFSET > 0)
                            {
                                tab[cLine, k + RES_YEAR_N1_OFFSET] = cellFactory.Get(0.0);
                            }
                            //Evol
                            if (RES_EVOL_OFFSET > 0)
                            {
                                tab[cLine, k + RES_EVOL_OFFSET] = new CellEvol(tab[cLine, k + RES_YEAR_N_OFFSET], tab[cLine, k + RES_YEAR_N1_OFFSET]);
                                ((CellEvol)tab[cLine, k + RES_EVOL_OFFSET]).StringFormat = "{0:percentage}";
                            }
                            //PDV N
                            if (RES_PDV_N_OFFSET > 0)
                            {
                                tab[cLine, k + RES_PDV_N_OFFSET] = new CellPDM(0.0, (CellUnit)tab[parents[0].LineIndexInResultTable, k + RES_PDV_N_OFFSET]);
                                ((CellPDM)tab[cLine, k + RES_PDV_N_OFFSET]).StringFormat = "{0:percentage}";
                            }
                            //PDV N1
                            if (RES_PDV_N1_OFFSET > 0)
                            {
                                tab[cLine, k + RES_PDV_N1_OFFSET] = new CellPDM(0.0, (CellUnit)tab[parents[0].LineIndexInResultTable, k + RES_PDV_N1_OFFSET]);
                                ((CellPDM)tab[cLine, k + RES_PDV_N1_OFFSET]).StringFormat = "{0:percentage}";
                            }
                        }
                        #endregion

                    }

                    #region Add Values
                    columnId = -1;
                    if (DATA_MEDIA_INDEXES.Count > 0)
                    {
                        columnId = Convert.ToInt32(row[DATA_MEDIA_INDEXES[DATA_MEDIA_INDEXES.Count - 1]]);
                        columnKey = Convert.ToInt32(row[DATA_MEDIA_INDEXES[0]]) + "_" + columnId;
                    }
                    subTotalIndex = (RES_MEDIA_SUBTOTAL.ContainsKey(columnKey)) ? RES_MEDIA_SUBTOTAL[columnKey].IndexInResultTable : -1;
                    valueN = Convert.ToDouble(row[DATA_YEAR_N]);
                    if (DATA_YEAR_N1 > -1)
                    {
                        valueN1 = Convert.ToDouble(row[DATA_YEAR_N1]);
                    }
                    //N
                    if (columnId > -1)
                        tab.AffectValueAndAddToHierarchy(1, cLine, RES_MEDIA_HEADERS[columnKey + "_Period1"].IndexInResultTable, valueN);
                    tab.AffectValueAndAddToHierarchy(1, cLine, RES_YEAR_N_OFFSET + 1, valueN);
                    if (subTotalIndex > -1)
                    {
                        tab.AffectValueAndAddToHierarchy(1, cLine, subTotalIndex, valueN);
                    }
                    //N1
                    if (RES_YEAR_N1_OFFSET > -1)
                    {
                        if (columnId > -1)
                            tab.AffectValueAndAddToHierarchy(1, cLine, RES_MEDIA_HEADERS[columnKey + "_Period2"].IndexInResultTable, valueN1);
                        tab.AffectValueAndAddToHierarchy(1, cLine, RES_YEAR_N1_OFFSET + 1, valueN1);
                        if (subTotalIndex > -1)
                        {
                            tab.AffectValueAndAddToHierarchy(1, cLine, subTotalIndex + RES_YEAR_N1_OFFSET - 1, valueN1);
                        }
                    }
                    //PDV N
                    if (RES_PDV_N_OFFSET > -1)
                    {
                        if (columnId > -1)
                            tab.AffectValueAndAddToHierarchy(1, cLine, RES_MEDIA_HEADERS[columnKey + "_PDV"].IndexInResultTable, valueN);
                        tab.AffectValueAndAddToHierarchy(1, cLine, RES_PDV_N_OFFSET + 1, valueN);
                        if (subTotalIndex > -1)
                        {
                            tab.AffectValueAndAddToHierarchy(1, cLine, subTotalIndex + RES_PDV_N_OFFSET - 1, valueN);
                        }
                    }
                    //PDV N1
                    if (RES_PDV_N1_OFFSET > -1)
                    {
                        if (columnId > -1)
                            tab.AffectValueAndAddToHierarchy(1, cLine, RES_MEDIA_HEADERS[columnKey + "_PDV1"].IndexInResultTable, valueN1);
                        tab.AffectValueAndAddToHierarchy(1, cLine, RES_PDV_N1_OFFSET + 1, valueN1);
                        if (subTotalIndex > -1)
                        {
                            tab.AffectValueAndAddToHierarchy(1, cLine, subTotalIndex + RES_PDV_N1_OFFSET - 1, valueN1);
                        }
                    }
                    #endregion

                }

            }
            #endregion

            return tab;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Check condition
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="cId">Table of identifiant</param>
        /// <param name="headerBaseList">HeaderBase list</param>
        /// <returns>True or False</returns>
        private bool CheckCondition(int index, List<long> cId, HeaderBase[] headerBaseList)
        {
            if (index == 0 && cId[index] != headerBaseList[index].Id)
                return true;
            else if (index > 0)
            {
                if (cId[index] != headerBaseList[index].Id || cId[index - 1] != headerBaseList[index - 1].Id)
                    return true;
            }
            return false;
        }
        #endregion

    }
}