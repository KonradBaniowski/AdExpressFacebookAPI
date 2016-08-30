﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.AdvertisingAgency.DAL;
using System.Reflection;
using System.Data;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.AdvertisingAgency.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Units;
using FctWeb = TNS.AdExpress.Web.Core.Utilities;

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
            begin = Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            end = Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
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
            var parameters = new object[2];
            parameters[0] = _session;
            parameters[1] = _period;
            var advertisingAgencyDAL = (IAdvertisingAgencyDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName), _module.CountryDataAccessLayer.Class,
                false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            DataSet ds = advertisingAgencyDAL.GetData();
            DataTable dtData = ds.Tables[0];
            if (dtData.Rows.Count <= 0) return null;
            #endregion

            #region Indexes

            #region Compute indexes : first numerical column, product indexes, media indexes
            int FIRST_DATA_INDEX = 0;
            var DATA_PRODUCT_INDEXES = new List<int>();
            var DATA_MEDIA_INDEXES = new List<int>();
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

            #region Periods Indexes
            var comparativePeriod = new MediaSchedulePeriod(_period.Begin.AddMonths(-12), _period.End.AddMonths(-12), _session.DetailPeriod);
            DateTime begin = Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            DateTime periodEndDate = Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
            int yearN = Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4));
            int yearN1 = _session.ComparativeStudy ? yearN - 1 : -1;
            Int32 DATA_YEAR_N = FIRST_DATA_INDEX;
            Int32 DATA_YEAR_N1 = (yearN1 > 0) ? FIRST_DATA_INDEX + 1 : -1;
            Int32 RES_YEAR_N_OFFSET = 1;
            var keyYearN = new List<Int64>(); keyYearN.Add(ID_YEAR_N);
            Int32 RES_YEAR_N1_OFFSET = (yearN1 > 0) ? 2 : -1;
            var keyYearN1 = new List<Int64>(); keyYearN1.Add(ID_YEAR_N1);
            Int32 RES_EVOL_OFFSET = (_session.Evolution && RES_YEAR_N1_OFFSET > 0) ? RES_YEAR_N1_OFFSET + 1 : -1;
            var keyEvol = new List<Int64>(); keyEvol.Add(ID_EVOL);
            Int32 RES_PDV_N_OFFSET = (_session.PDV) ? Math.Max(RES_YEAR_N_OFFSET, Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET)) + 1 : -1;
            var keyPdvYearN = new List<Int64>(); keyPdvYearN.Add(ID_PDV_N);
            Int32 RES_PDV_N1_OFFSET = (yearN1 > 0 && _session.PDV) ? RES_PDV_N_OFFSET + 1 : -1;
            var keyPdvYearN1 = new List<Int64>(); keyPdvYearN1.Add(ID_PDV_N1);
            Int32 RES_PDM_N_OFFSET = (_session.PDM) ? Math.Max(RES_PDV_N1_OFFSET, Math.Max(Math.Max(RES_YEAR_N_OFFSET, RES_PDV_N_OFFSET), Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET))) + 1 : -1;
            var keyPdmYearN = new List<Int64>(); keyPdmYearN.Add(ID_PDM_N);
            Int32 RES_PDM_N1_OFFSET = (yearN1 > 0 && _session.PDM) ? RES_PDM_N_OFFSET + 1 : -1;
            var keyPdmYearN1 = new List<Int64>(); keyPdmYearN1.Add(ID_PDM_N1);
            #endregion

            #region Periods Labels
            string labelN = FctUtilities.Dates.getPeriodLabelComparative(_session, _session.PeriodBeginningDate, _session.PeriodEndDate);
            string labelN1 = FctUtilities.Dates.getPeriodLabelComparative(_session, comparativePeriod.Begin.ToString("yyyyMMdd"), comparativePeriod.End.ToString("yyyyMMdd"));
            string labelEvol = GestionWeb.GetWebWord(1168, _session.SiteLanguage);
            string labelPDMN = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), labelN);
            string labelPDMN1 = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), labelN1);
            string labelPDVN = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), labelN);
            string labelPDVN1 = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), labelN1);
            #endregion

            #endregion

            #region Build headers
            var headers = new Headers();
            //Header Product
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), ID_PRODUCT));
            string vehicleLabel = dtData.Rows[0]["M1"].ToString();
            var itemsList = new List<long>();
            int optionsIndex = 2;

            //Header TOTAL MEDIA
            headers.Root.Add(new Header(true, GetTotalMediaText().ToUpper(), ID_TOTAL));

            //Header Period N
            headers.Root[1].Add(new Header(true, labelN, 1));

            //Header Period N1
            if (_session.ComparativeStudy)
                headers.Root[1].Add(new Header(true, labelN1, optionsIndex++));

            //Header Evol
            if (_session.Evolution)
                headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(1207, _session.SiteLanguage), optionsIndex++));

            //Heades PDM
            if (_session.PDM)
            {

                headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                if (_session.ComparativeStudy)
                    headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex++));
            }
            //Headers PDV
            if (_session.PDV)
            {
                headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                if (_session.ComparativeStudy)
                    headers.Root[1].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex));
            }

            //Go threw data to extract media levels
            string sortStr = "";
            switch (_session.PreformatedMediaDetail)
            {
                case CstFormat.PreformatedMediaDetails.vehicle:
                case CstFormat.PreformatedMediaDetails.category:
                case CstFormat.PreformatedMediaDetails.Media:
                case CstFormat.PreformatedMediaDetails.mediaSeller:
                case CstFormat.PreformatedMediaDetails.title:
                    sortStr = "M1,ID_M1";
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategory:
                case CstFormat.PreformatedMediaDetails.vehicleMedia:
                case CstFormat.PreformatedMediaDetails.vehicleMediaSeller:
                case CstFormat.PreformatedMediaDetails.vehicleTitle:
                case CstFormat.PreformatedMediaDetails.mediaSellerTitle:
                case CstFormat.PreformatedMediaDetails.mediaSellerVehicle:
                case CstFormat.PreformatedMediaDetails.mediaSellerMedia:
                case CstFormat.PreformatedMediaDetails.mediaSellerCategory:
                    sortStr = "M1,ID_M1,M2,ID_M2";
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
                    sortStr = "M1,ID_M1,M2,ID_M2,M3,ID_M3";
                    break;
                default:
                    throw new AdvertisingAgencyException("Detail format " + _session.PreformatedMediaDetail.ToString() + " unvalid.");
            }
            var dtMedias = dtData.Select("", sortStr);// advertisingAgencyDAL.GetData(true).Tables[0];//

            var RES_MEDIA_HEADERS = new Dictionary<string, Header>();
            var RES_MEDIA_SUBTOTAL = new Dictionary<string, HeaderBase>();
            var RES_MEDIA_TOTAL = new Dictionary<string, HeaderBase>();
            var MEDIA_IDS = new HeaderBase[DATA_MEDIA_INDEXES.Count];
            var MEDIA_IDS_TOTAL = new List<HeaderBase>();

            var SUB_TOTALS = new List<HeaderBase>();
            var TOTALS = new List<HeaderBase>();
            int tableIndex = 0;
            var cId = new List<long>();
            var oldcId = new List<long>();
            // long oldcId = -1;
            var subTotalsParentIds = new Dictionary<string, HeaderBase>();

            for (int i = 0; i < DATA_MEDIA_INDEXES.Count; i++)
            {
                cId.Add(-1);
                oldcId.Add(-1);
            }



            var labelsList = new List<string>();
            HeaderBase pHeaderBase = headers.Root;
            foreach (DataRow row in dtMedias)
            {

                for (int i = 0; i < DATA_MEDIA_INDEXES.Count; i++)
                {
                    //if(i == 0) oldcId = cId[0];
                    if (i == 0 && DATA_MEDIA_INDEXES.Count < 3) oldcId[0] = cId[0];
                    cId[i] = Convert.ToInt64(row[DATA_MEDIA_INDEXES[i]]);

                    if( CanAddHeader( cId,oldcId,  DATA_MEDIA_INDEXES,  MEDIA_IDS,  i))
                    {
                        //Add Media Type
                        if (cId.Count > 2 && i < DATA_MEDIA_INDEXES.Count - 2 && cId[0] != oldcId[0])
                        {
                            var headerGroup = new HeaderGroup(row[DATA_MEDIA_INDEXES[i] + 1].ToString(), cId[i]);
                            TOTALS.Add(headerGroup);

                            //Media Type group Header
                            MEDIA_IDS_TOTAL.Add(headerGroup);

                            //Header Period N
                            headerGroup.Add(new Header(true, labelN, 1));
                            labelsList.Add("Period1");
                            optionsIndex = 2;

                            //Header Period N1
                            if (_session.ComparativeStudy)
                            {
                                headerGroup.Add(new Header(true, labelN1, optionsIndex++));
                                labelsList.Add("Period2");
                            }

                            //Header Evol
                            if (_session.Evolution)
                            {
                                headerGroup.Add(new Header(true, GestionWeb.GetWebWord(1207, _session.SiteLanguage), optionsIndex++));
                                labelsList.Add("Evolution");
                            }

                            //HeaderS PDM
                            if (_session.PDM)
                            {
                                headerGroup.Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                                labelsList.Add("PDM");
                                if (_session.ComparativeStudy)
                                {
                                    headerGroup.Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex++));
                                    labelsList.Add("PDM1");
                                }
                            }

                            //HeaderS PDV
                            if (_session.PDV)
                            {
                                headerGroup.Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                                labelsList.Add("PDV");
                                if (_session.ComparativeStudy)
                                {
                                    headerGroup.Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex));
                                    labelsList.Add("PDV1");
                                }
                            }
                            headers.Root.Add(headerGroup);
                            RES_MEDIA_TOTAL.Add(MEDIA_IDS_TOTAL[i].Id + "_" + cId[i], headerGroup);

                        }

                        //Add SUB TOTAL items                      
                        else if (i < DATA_MEDIA_INDEXES.Count - 1)
                        {
                            //Media group Header
                            MEDIA_IDS[i] = new HeaderGroup(row[DATA_MEDIA_INDEXES[i] + 1].ToString(), cId[i]);

                            //Sub total
                            SUB_TOTALS.Add(((HeaderGroup)MEDIA_IDS[i]).AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), ID_TOTAL));
                            //MEDIA_IDS[i][0].Key = cId[i].ToString();

                            //Header Period N
                            MEDIA_IDS[i][0].Add(new Header(true, labelN, 1));
                            optionsIndex = 2;

                            //Header Period N1
                            if (_session.ComparativeStudy)
                                MEDIA_IDS[i][0].Add(new Header(true, labelN1, optionsIndex++));

                            //Header Evol
                            if (_session.Evolution)
                                MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(1207, _session.SiteLanguage), optionsIndex++));

                            //HeaderS PDM
                            if (_session.PDM)
                            {
                                MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                                if (_session.ComparativeStudy)
                                    MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex++));
                            }

                            //HeaderS PDV
                            if (_session.PDV)
                            {
                                MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex++));
                                if (_session.ComparativeStudy)
                                    MEDIA_IDS[i][0].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex));
                            }

                           
							
                            if (DATA_MEDIA_INDEXES.Count == 3)

                            {
                                headers.Root.Add(MEDIA_IDS[i]);

                                if (DATA_MEDIA_INDEXES.Count >= 3)
                                    subTotalsParentIds.Add(MEDIA_IDS[i].Id.ToString(), MEDIA_IDS_TOTAL.Last());
                            }
                            else
                            {
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
                        }
                        else
                        {
                            //Add Current media level items
                            labelsList = new List<string>();
                            if (i == 0) tableIndex = i;
                            else tableIndex = i - 1;
                            MEDIA_IDS[i] = new Header(row[DATA_MEDIA_INDEXES[i] + 1].ToString(), cId[i]);
                            MEDIA_IDS[i].Add(new Header(true, labelN, 1));
                            RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_Period1", (Header)MEDIA_IDS[i][0]);
                            labelsList.Add("Period1");
                            optionsIndex = 2;

                            if (!itemsList.Contains(MEDIA_IDS[tableIndex].Id))
                                itemsList.Add(MEDIA_IDS[tableIndex].Id);

                            if (_session.ComparativeStudy)
                            {
                                MEDIA_IDS[i].Add(new Header(true, labelN1, optionsIndex));
                                RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_Period2", (Header)MEDIA_IDS[i][optionsIndex - 1]);
                                optionsIndex++;
                                labelsList.Add("Period2");
                            }
                            if (_session.Evolution)
                            {
                                MEDIA_IDS[i].Add(new Header(true, GestionWeb.GetWebWord(1207, _session.SiteLanguage), optionsIndex));
                                RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_Evolution", (Header)MEDIA_IDS[i][optionsIndex - 1]);
                                optionsIndex++;
                                labelsList.Add("Evolution");
                            }
                            if (_session.PDM)
                            {
                                MEDIA_IDS[i].Add(new Header(true, GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN, optionsIndex));
                                RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_PDM", (Header)MEDIA_IDS[i][optionsIndex - 1]);
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
                                RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_PDV", (Header)MEDIA_IDS[i][optionsIndex - 1]);
                                optionsIndex++;
                                labelsList.Add("PDV");
                                if (_session.ComparativeStudy)
                                {
                                    MEDIA_IDS[i].Add(new Header(true, GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + labelN1, optionsIndex));
                                    RES_MEDIA_HEADERS.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i] + "_PDV1", (Header)MEDIA_IDS[i][optionsIndex - 1]);
                                    labelsList.Add("PDV1");
                                }
                            }

                            pHeaderBase.Add(MEDIA_IDS[i]);


                            if (DATA_MEDIA_INDEXES.Count == 3)
                            {
                                if (SUB_TOTALS.Any())
                                    RES_MEDIA_SUBTOTAL.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i], SUB_TOTALS[SUB_TOTALS.Count - 1]);
                                if (DATA_MEDIA_INDEXES.Count >= 3)
                                    RES_MEDIA_TOTAL.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i], TOTALS[TOTALS.Count - 1]);
                            }
                            else
                            {
                                if (pHeaderBase is HeaderGroup)
                                {
                                    RES_MEDIA_SUBTOTAL.Add(MEDIA_IDS[tableIndex].Id + "_" + cId[i], SUB_TOTALS[SUB_TOTALS.Count - 1]);
                                }
                                
                            }

                        
                           
                        }
                    }
                    if (DATA_MEDIA_INDEXES.Count == 3)
                    oldcId[i] = cId[i];
                }

            }

            if (itemsList.Count == 1)
                headers.Root.RemoveAt(1);
            #endregion

            #region Init Result Table
            var tab = new ResultTable(dtData.Rows.Count * 2, headers);
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();
            CellUnit cell = cellFactory.Get(0.0);
            cell.DisplayContent = false;
            var parents = new AdExpressCellLevel[DATA_PRODUCT_INDEXES.Count + 1];
            /* This variable is used to reintialize PDV and PDM total cells in the case of an empty column result
             * For the total PDV and PDM cells, we always display 100%, because we consider that the column must be filled with data
             * ther no reason to have a PDV or PDM column, if we don't have data.
             * However, for the 'Manadataires' module, we can have PDV or PDM columns with empty data
             * */
            var reInitPDVPDMVoidColumns = new Dictionary<int, bool>();
            #endregion

            #region Total line
            var keys = new List<Int64>();
            keys.Add(ID_TOTAL);
            //Total
            int cLine = 0;
            int columnIndex = 1;
            Int64 columnId = 0;
            string columnKey = string.Empty;

            cLine = tab.AddNewLine(LineType.total);
            //Total
            tab[cLine, columnIndex] = parents[0] = new AdExpressCellLevel(ID_TOTAL, GestionWeb.GetWebWord(1401, _session.SiteLanguage), 0, cLine, _session);
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
                    if (!reInitPDVPDMVoidColumns.ContainsKey(k + RES_PDV_N_OFFSET))
                        reInitPDVPDMVoidColumns.Add(k + RES_PDV_N_OFFSET, false);
                }
                //PDV N1
                if (RES_PDV_N1_OFFSET > 0)
                {
                    tab[cLine, k + RES_PDV_N1_OFFSET] = new CellPDM(0.0, null);
                    ((CellPDM)tab[cLine, k + RES_PDV_N1_OFFSET]).StringFormat = "{0:percentage}";
                    if (!reInitPDVPDMVoidColumns.ContainsKey(k + RES_PDV_N1_OFFSET))
                        reInitPDVPDMVoidColumns.Add(k + RES_PDV_N1_OFFSET, false);
                }
            }
            //PDM N
            if (RES_PDM_N_OFFSET > 0)
            {
                tab[cLine, 1 + RES_PDM_N_OFFSET] = new CellPDM(0.0, null);
                ((CellPDM)tab[cLine, 1 + RES_PDM_N_OFFSET]).StringFormat = "{0:percentage}";
                if (!reInitPDVPDMVoidColumns.ContainsKey(1 + RES_PDM_N_OFFSET))
                    reInitPDVPDMVoidColumns.Add(1 + RES_PDM_N_OFFSET, false);
            }
            //PDM N1
            if (RES_PDM_N1_OFFSET > 0)
            {
                tab[cLine, 1 + RES_PDM_N1_OFFSET] = new CellPDM(0.0, null);
                ((CellPDM)tab[cLine, 1 + RES_PDM_N1_OFFSET]).StringFormat = "{0:percentage}";
                if (!reInitPDVPDMVoidColumns.ContainsKey(1 + RES_PDM_N1_OFFSET))
                    reInitPDVPDMVoidColumns.Add(1 + RES_PDM_N1_OFFSET, false);
            }
            if (DATA_MEDIA_INDEXES.Count >= 3)
            {
                //Init Media Type
                TOTALS.ForEach(h =>
                {
                    //PDM N                       
                    if (RES_PDM_N_OFFSET > 0)
                    {

                        tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, 2]);
                        ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1]).StringFormat = "{0:percentage}";
                        if (!reInitPDVPDMVoidColumns.ContainsKey(h.IndexInResultTable + RES_PDM_N_OFFSET - 1))
                            reInitPDVPDMVoidColumns.Add(h.IndexInResultTable + RES_PDM_N_OFFSET - 1, false);
                    }
                    //PDM N1
                    if (RES_PDM_N1_OFFSET > 0)
                    {
                        tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, 3]);
                        ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1]).StringFormat = "{0:percentage}";
                        if (!reInitPDVPDMVoidColumns.ContainsKey(h.IndexInResultTable + RES_PDM_N1_OFFSET - 1))
                            reInitPDVPDMVoidColumns.Add(h.IndexInResultTable + RES_PDM_N1_OFFSET - 1, false);
                    }
                });

                //Init sub total PDM
                foreach (HeaderBase h in SUB_TOTALS)
                {
                    //PDM N
                    if (RES_PDM_N_OFFSET > 0)
                    {
                        var separator = new string[1];
                        separator[0] = "--";
                        var subkey = h.Key.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        var totalPDM = subTotalsParentIds[subkey[0]];
                        tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, totalPDM.IndexInResultTable + RES_PDM_N_OFFSET - 1]);
                        ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1]).StringFormat = "{0:percentage}";
                        if (!reInitPDVPDMVoidColumns.ContainsKey(h.IndexInResultTable + RES_PDM_N_OFFSET - 1))
                            reInitPDVPDMVoidColumns.Add(h.IndexInResultTable + RES_PDM_N_OFFSET - 1, false);
                    }
                    //PDM N1
                    if (RES_PDM_N1_OFFSET > 0)
                    {
                        var separator = new string[1];
                        separator[0] = "--";
                        var subkey = h.Key.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        var totalPDM = subTotalsParentIds[subkey[0]];
                        tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, totalPDM.IndexInResultTable + RES_PDM_N1_OFFSET - 1]);
                        ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1]).StringFormat = "{0:percentage}";
                        if (!reInitPDVPDMVoidColumns.ContainsKey(h.IndexInResultTable + RES_PDM_N1_OFFSET - 1))
                            reInitPDVPDMVoidColumns.Add(h.IndexInResultTable + RES_PDM_N1_OFFSET - 1, false);
                    }
                }
            }
            else
            {
                //Init sub total PDM
                foreach (HeaderBase h in SUB_TOTALS)
                {
                    //PDM N
                    if (RES_PDM_N_OFFSET > 0)
                    {
                        tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, 2]);
                        ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1]).StringFormat = "{0:percentage}";
                        if (!reInitPDVPDMVoidColumns.ContainsKey(h.IndexInResultTable + RES_PDM_N_OFFSET - 1))
                            reInitPDVPDMVoidColumns.Add(h.IndexInResultTable + RES_PDM_N_OFFSET - 1, false);
                    }
                    //PDM N1
                    if (RES_PDM_N1_OFFSET > 0)
                    {
                        tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, 3]);
                        ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1]).StringFormat = "{0:percentage}";
                        if (!reInitPDVPDMVoidColumns.ContainsKey(h.IndexInResultTable + RES_PDM_N1_OFFSET - 1))
                            reInitPDVPDMVoidColumns.Add(h.IndexInResultTable + RES_PDM_N1_OFFSET - 1, false);
                    }
                }
            }
            ////Init Media PDM
            foreach (KeyValuePair<string, Header> record in RES_MEDIA_HEADERS)
            {
                string[] subString = record.Key.Split('_');
                string subKey = subString[0] + "_" + subString[1];
                if (subString[2].Equals("PDM"))
                {
                    //PDM N
                    if (RES_PDM_N_OFFSET > 0)
                    {
                        if (RES_MEDIA_SUBTOTAL.ContainsKey(subKey))
                        {
                            tab[cLine, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine, RES_MEDIA_SUBTOTAL[subKey].IndexInResultTable]);
                        }
                        else
                        {
                            tab[cLine, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine, 2]);
                        }
                        ((CellPDM)tab[cLine, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                        if (!reInitPDVPDMVoidColumns.ContainsKey(record.Value.IndexInResultTable))
                            reInitPDVPDMVoidColumns.Add(record.Value.IndexInResultTable, false);
                    }
                }
                if (subString[2].Equals("PDM1"))
                {
                    //PDM N1
                    if (RES_PDM_N1_OFFSET > 0)
                    {
                        if (RES_MEDIA_SUBTOTAL.ContainsKey(subKey))
                        {
                            tab[cLine, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine, RES_MEDIA_SUBTOTAL[subKey].IndexInResultTable + 1]);
                        }
                        else
                        {
                            tab[cLine, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine, 3]);
                        }
                        ((CellPDM)tab[cLine, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                        if (!reInitPDVPDMVoidColumns.ContainsKey(record.Value.IndexInResultTable))
                            reInitPDVPDMVoidColumns.Add(record.Value.IndexInResultTable, false);
                    }
                }

            }
            #endregion

            #region Fill table
            LineType[] lTypes = new LineType[4] { LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
            LineType[] lSubTypes = new LineType[4] { LineType.level5, LineType.level6, LineType.level7, LineType.level8 };
            Double valueN = 0;
            Double valueN1 = 0;
            Int32 subTotalIndex = -1;
            Int32 vehicleTotalIndex = -1;
            GenericDetailLevel detailLevel = _session.GenericProductDetailLevel;
            int valueColumnIndex = 0;
            int totalColumnIndex = 0;
            int subTotalColumnIndex = 0;

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

                        #region Add new line and Init cells
                        //Total
                        cLine = tab.AddNewLine(lTypes[i]);
                        tab[cLine, 1] = parents[i + 1] = new AdExpressCellLevel(columnId, row[DATA_PRODUCT_INDEXES[i] + 1].ToString(), (AdExpressCellLevel)tab[parents[i].LineIndexInResultTable, 1], i + 1, cLine, _session);

                        ////Gad
                        if (_session.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == ((AdExpressCellLevel)tab[cLine, 1]).Level)
                        {
                            if (row[row.ItemArray.Length - 1].ToString().Length > 0)
                                ((AdExpressCellLevel)tab[cLine, 1]).AddressId = (Int64)row[row.ItemArray.Length - 1];
                        }

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
                                tab[cLine, k + RES_PDV_N_OFFSET] = new CellPDM(0.0, (CellUnit)tab[parents[i].LineIndexInResultTable, k + RES_PDV_N_OFFSET]);
                                ((CellPDM)tab[cLine, k + RES_PDV_N_OFFSET]).StringFormat = "{0:percentage}";
                            }
                            //PDV N1
                            if (RES_PDV_N1_OFFSET > 0)
                            {
                                tab[cLine, k + RES_PDV_N1_OFFSET] = new CellPDM(0.0, (CellUnit)tab[parents[i].LineIndexInResultTable, k + RES_PDV_N1_OFFSET]);
                                ((CellPDM)tab[cLine, k + RES_PDV_N1_OFFSET]).StringFormat = "{0:percentage}";
                            }
                        }
                        //PDM N
                        if (RES_PDM_N_OFFSET > 0)
                        {
                            tab[cLine, 1 + RES_PDM_N_OFFSET] = new CellPDM(0.0, null);
                            ((CellPDM)tab[cLine, 1 + RES_PDM_N_OFFSET]).StringFormat = "{0:percentage}";
                        }
                        //PDM N1
                        if (RES_PDM_N1_OFFSET > 0)
                        {
                            tab[cLine, 1 + RES_PDM_N1_OFFSET] = new CellPDM(0.0, null);
                            ((CellPDM)tab[cLine, 1 + RES_PDM_N1_OFFSET]).StringFormat = "{0:percentage}";
                        }
                        if (DATA_MEDIA_INDEXES.Count >= 3)
                        {
                            //Init Media Type
                            TOTALS.ForEach(h =>
                            {
                                //PDM N                       
                                if (RES_PDM_N_OFFSET > 0)
                                {

                                    tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, 2]);
                                    ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1]).StringFormat = "{0:percentage}";
                                    if (!reInitPDVPDMVoidColumns.ContainsKey(h.IndexInResultTable + RES_PDM_N_OFFSET - 1))
                                        reInitPDVPDMVoidColumns.Add(h.IndexInResultTable + RES_PDM_N_OFFSET - 1, false);
                                }
                                //PDM N1
                                if (RES_PDM_N1_OFFSET > 0)
                                {
                                    tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, 3]);
                                    ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1]).StringFormat = "{0:percentage}";
                                    if (!reInitPDVPDMVoidColumns.ContainsKey(h.IndexInResultTable + RES_PDM_N1_OFFSET - 1))
                                        reInitPDVPDMVoidColumns.Add(h.IndexInResultTable + RES_PDM_N1_OFFSET - 1, false);
                                }
                            });

                            //Init sub total PDM
                            foreach (HeaderBase h in SUB_TOTALS)
                            {
                                //PDM N
                                if (RES_PDM_N_OFFSET > 0)
                                {
                                    var separator = new string[1];
                                    separator[0] = "--";
                                    var subkey = h.Key.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                    var totalPDM = subTotalsParentIds[subkey[0]];
                                    tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, totalPDM.IndexInResultTable + RES_PDM_N_OFFSET - 1]);
                                    ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1]).StringFormat = "{0:percentage}";                                  
                                }
                                //PDM N1
                                if (RES_PDM_N1_OFFSET > 0)
                                {
                                    var separator = new string[1];
                                    separator[0] = "--";
                                    var subkey = h.Key.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                    var totalPDM = subTotalsParentIds[subkey[0]];
                                    tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, totalPDM.IndexInResultTable + RES_PDM_N1_OFFSET - 1]);
                                    ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1]).StringFormat = "{0:percentage}";                                 
                                }
                            }
                        }
                        else
                        {
                            //Init sub total PDM
                            foreach (HeaderBase h in SUB_TOTALS)
                            {
                                //PDM N
                                if (RES_PDM_N_OFFSET > 0)
                                {
                                    tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, 2]);
                                    ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N_OFFSET - 1]).StringFormat = "{0:percentage}";
                                }
                                //PDM N1
                                if (RES_PDM_N1_OFFSET > 0)
                                {
                                    tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1] = new CellPDM(0.0, (CellUnit)tab[cLine, 3]);
                                    ((CellPDM)tab[cLine, h.IndexInResultTable + RES_PDM_N1_OFFSET - 1]).StringFormat = "{0:percentage}";
                                }

                            }
                        }
                        //Init Media PDM
                        foreach (KeyValuePair<string, Header> record in RES_MEDIA_HEADERS)
                        {
                            string[] subString = record.Key.Split('_');
                            string subKey = subString[0] + "_" + subString[1];
                            if (subString[2].Equals("PDM"))
                            {
                                //PDM N
                                if (RES_PDM_N_OFFSET > 0)
                                {
                                    if (RES_MEDIA_SUBTOTAL.ContainsKey(subKey))
                                    {
                                        tab[cLine, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine, RES_MEDIA_SUBTOTAL[subKey].IndexInResultTable]);
                                    }
                                    else
                                    {
                                        tab[cLine, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine, 2]);
                                    }
                                    ((CellPDM)tab[cLine, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                                }
                            }
                            if (subString[2].Equals("PDM1"))
                            {
                                //PDM N1
                                if (RES_PDM_N1_OFFSET > 0)
                                {
                                    if (RES_MEDIA_SUBTOTAL.ContainsKey(subKey))
                                    {
                                        tab[cLine, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine, RES_MEDIA_SUBTOTAL[subKey].IndexInResultTable + 1]);
                                    }
                                    else
                                    {
                                        tab[cLine, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine, 3]);
                                    }
                                    ((CellPDM)tab[cLine, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                                }
                            }
                        }
                        #endregion

                    }

                    if (i == (DATA_PRODUCT_INDEXES.Count - 1))
                    {
                        #region Add Values
                        valueColumnIndex = 0;
                        totalColumnIndex = 0;
                        subTotalColumnIndex = 0;

                        columnId = -1;
                        if (DATA_MEDIA_INDEXES.Count > 0)
                        {
                            columnId = Convert.ToInt32(row[DATA_MEDIA_INDEXES[DATA_MEDIA_INDEXES.Count - 1]]);
                            int mediaIndex = DATA_MEDIA_INDEXES.Count == 3 ? DATA_MEDIA_INDEXES.Count - 2 : 0;
                            columnKey = Convert.ToInt32(row[DATA_MEDIA_INDEXES[mediaIndex]]) + "_" + columnId;
                        }

                        subTotalIndex = (RES_MEDIA_SUBTOTAL.ContainsKey(columnKey)) ? RES_MEDIA_SUBTOTAL[columnKey].IndexInResultTable : -1;

                        if (DATA_MEDIA_INDEXES.Count >= 3)
                        vehicleTotalIndex =  (RES_MEDIA_TOTAL.ContainsKey(columnKey)) ? RES_MEDIA_TOTAL[columnKey].IndexInResultTable : -1;

                        valueN = Convert.ToDouble(row[DATA_YEAR_N]);
                        if (DATA_YEAR_N1 > -1)
                        {
                            valueN1 = Convert.ToDouble(row[DATA_YEAR_N1]);
                        }
                        //N Column Value
                        try
                        {
                            valueColumnIndex = RES_MEDIA_HEADERS[columnKey + "_Period1"].IndexInResultTable;
                        }
                        catch (Exception e)
                        {

                            string c = columnKey;
                        }
                    
                        if (columnId > -1)
                            tab.AffectValueAndAddToHierarchy(1, cLine, valueColumnIndex, valueN);
                        // Total Column Value
                        totalColumnIndex = RES_YEAR_N_OFFSET + 1;
                        if (valueColumnIndex != totalColumnIndex)
                            tab.AffectValueAndAddToHierarchy(1, cLine, totalColumnIndex, valueN);
                        // Sub Total Column Value
                        if (subTotalIndex > -1)
                        {
                            subTotalColumnIndex = subTotalIndex;
                            if (totalColumnIndex != subTotalColumnIndex)
                                tab.AffectValueAndAddToHierarchy(1, cLine, subTotalColumnIndex, valueN);
                        }
                        // Vehicle Total Column Value
                        if (DATA_MEDIA_INDEXES.Count >= 3 && vehicleTotalIndex > -1)
                        {

                            if (totalColumnIndex != vehicleTotalIndex)
                                tab.AffectValueAndAddToHierarchy(1, cLine, vehicleTotalIndex, valueN);
                        }
                        //N1 Column Value
                        if (RES_YEAR_N1_OFFSET > -1)
                        {
                            valueColumnIndex = RES_MEDIA_HEADERS[columnKey + "_Period2"].IndexInResultTable;
                            if (columnId > -1)
                                tab.AffectValueAndAddToHierarchy(1, cLine, valueColumnIndex, valueN1);
                            // Total Column Value
                            totalColumnIndex = RES_YEAR_N1_OFFSET + 1;
                            if (valueColumnIndex != totalColumnIndex)
                                tab.AffectValueAndAddToHierarchy(1, cLine, totalColumnIndex, valueN1);
                            // Sub Total Column Value
                            if (subTotalIndex > -1)
                            {
                                subTotalColumnIndex = subTotalIndex + RES_YEAR_N1_OFFSET - 1;
                                if (totalColumnIndex != subTotalColumnIndex)
                                    tab.AffectValueAndAddToHierarchy(1, cLine, subTotalColumnIndex, valueN1);
                            }
                            // Vehicle Total Column Value
                            if (DATA_MEDIA_INDEXES.Count >= 3 && vehicleTotalIndex > -1)
                            {
                                var indx = vehicleTotalIndex + RES_YEAR_N1_OFFSET - 1;
                                if (totalColumnIndex != indx)
                                    tab.AffectValueAndAddToHierarchy(1, cLine, indx, valueN1);
                            }
                        }
                        //PDV N
                        if (RES_PDV_N_OFFSET > -1)
                        {
                            //Column Value
                            valueColumnIndex = RES_MEDIA_HEADERS[columnKey + "_PDV"].IndexInResultTable;
                            if (columnId > -1)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, valueColumnIndex, valueN);
                                if (valueN > 0)
                                    reInitPDVPDMVoidColumns[valueColumnIndex] = true;
                            }
                            // Total Column Value
                            totalColumnIndex = RES_PDV_N_OFFSET + 1;
                            if (valueColumnIndex != totalColumnIndex)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, totalColumnIndex, valueN);
                                if (valueN > 0)
                                    reInitPDVPDMVoidColumns[totalColumnIndex] = true;
                            }
                            // Sub Total Column Value
                            if (subTotalIndex > -1)
                            {
                                subTotalColumnIndex = subTotalIndex + RES_PDV_N_OFFSET - 1;
                                if (totalColumnIndex != subTotalColumnIndex)
                                {
                                    tab.AffectValueAndAddToHierarchy(1, cLine, subTotalColumnIndex, valueN);
                                    if (valueN > 0)
                                        reInitPDVPDMVoidColumns[subTotalColumnIndex] = true;
                                }
                            }
                            // Vehicle Total Column Value
                            if (DATA_MEDIA_INDEXES.Count >= 3 && vehicleTotalIndex > -1)
                            {
                                var indx = (vehicleTotalIndex + RES_PDV_N_OFFSET - 1);
                                if (totalColumnIndex != indx)
                                {
                                    tab.AffectValueAndAddToHierarchy(1, cLine, indx, valueN);
                                    if (valueN > 0)
                                        reInitPDVPDMVoidColumns[indx] = true;
                                }
                            }
                        }
                        //PDV N1
                        if (RES_PDV_N1_OFFSET > -1)
                        {
                            //Column Value
                            valueColumnIndex = RES_MEDIA_HEADERS[columnKey + "_PDV1"].IndexInResultTable;
                            if (columnId > -1)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, valueColumnIndex, valueN1);
                                if (valueN1 > 0)
                                    reInitPDVPDMVoidColumns[valueColumnIndex] = true;
                            }
                            // Total Column Value
                            totalColumnIndex = RES_PDV_N1_OFFSET + 1;
                            if (valueColumnIndex != totalColumnIndex)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, totalColumnIndex, valueN1);
                                if (valueN1 > 0)
                                    reInitPDVPDMVoidColumns[totalColumnIndex] = true;
                            }
                            // Sub Total Column Value
                            if (subTotalIndex > -1)
                            {
                                subTotalColumnIndex = subTotalIndex + RES_PDV_N1_OFFSET - 1;
                                if (totalColumnIndex != subTotalColumnIndex)
                                {
                                    tab.AffectValueAndAddToHierarchy(1, cLine, subTotalColumnIndex, valueN1);
                                    if (valueN1 > 0)
                                        reInitPDVPDMVoidColumns[subTotalColumnIndex] = true;
                                }
                            }
                            // Vehicle Total Column Value
                            if (DATA_MEDIA_INDEXES.Count >= 3 && vehicleTotalIndex > -1)
                            {
                                var indx = vehicleTotalIndex + RES_PDV_N1_OFFSET - 1;
                                if (totalColumnIndex != indx)
                                {
                                    tab.AffectValueAndAddToHierarchy(1, cLine, indx, valueN1);
                                    if (valueN1 > 0)
                                        reInitPDVPDMVoidColumns[indx] = true;
                                }
                            }
                        }
                        //PDM N
                        if (RES_PDM_N_OFFSET > -1)
                        {
                            //Column Value
                            valueColumnIndex = RES_MEDIA_HEADERS[columnKey + "_PDM"].IndexInResultTable;
                            if (columnId > -1)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, valueColumnIndex, valueN);
                                if (valueN > 0)
                                    reInitPDVPDMVoidColumns[valueColumnIndex] = true;
                            }
                            // Total Column Value
                            totalColumnIndex = RES_PDM_N_OFFSET + 1;
                            if (valueColumnIndex != totalColumnIndex)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, totalColumnIndex, valueN);
                                if (valueN > 0)
                                    reInitPDVPDMVoidColumns[totalColumnIndex] = true;
                            }
                            // Sub Total Column Value
                            if (subTotalIndex > -1)
                            {
                                subTotalColumnIndex = subTotalIndex + RES_PDM_N_OFFSET - 1;
                                if (totalColumnIndex != subTotalColumnIndex)
                                {
                                    tab.AffectValueAndAddToHierarchy(1, cLine, subTotalColumnIndex, valueN);
                                    if (valueN > 0)
                                        reInitPDVPDMVoidColumns[subTotalColumnIndex] = true;
                                }
                            }
                             // Vehicle Total Column Value                        
                            if (DATA_MEDIA_INDEXES.Count >= 3 && vehicleTotalIndex > -1)
                            {
                                var indx = vehicleTotalIndex + RES_PDM_N_OFFSET - 1;
                                if (totalColumnIndex != indx)
                                {
                                    tab.AffectValueAndAddToHierarchy(1, cLine, indx, valueN);
                                    if (valueN > 0)
                                        reInitPDVPDMVoidColumns[indx] = true;
                                }
                            }
                        }
                        //PDM N1
                        if (RES_PDM_N1_OFFSET > -1)
                        {
                            //Column Value
                            valueColumnIndex = RES_MEDIA_HEADERS[columnKey + "_PDM1"].IndexInResultTable;
                            if (columnId > -1)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, valueColumnIndex, valueN1);
                                if (valueN1 > 0)
                                    reInitPDVPDMVoidColumns[valueColumnIndex] = true;
                            }
                            // Total Column Value
                            totalColumnIndex = RES_PDM_N1_OFFSET + 1;
                            if (valueColumnIndex != totalColumnIndex)
                            {
                                tab.AffectValueAndAddToHierarchy(1, cLine, totalColumnIndex, valueN1);
                                if (valueN1 > 0)
                                    reInitPDVPDMVoidColumns[totalColumnIndex] = true;
                            }
                            // Sub Total Column Value
                            if (subTotalIndex > -1)
                            {
                                subTotalColumnIndex = subTotalIndex + RES_PDM_N1_OFFSET - 1;
                                if (totalColumnIndex != subTotalColumnIndex)
                                {
                                    tab.AffectValueAndAddToHierarchy(1, cLine, subTotalColumnIndex, valueN1);
                                    if (valueN1 > 0)
                                        reInitPDVPDMVoidColumns[subTotalColumnIndex] = true;
                                }
                            }
                            // Vehicle Total Column Value         
                            if (DATA_MEDIA_INDEXES.Count >= 3 && vehicleTotalIndex > -1)
                            {
                                var indx = vehicleTotalIndex + RES_PDM_N1_OFFSET - 1;
                                if (totalColumnIndex != indx)
                                {
                                    tab.AffectValueAndAddToHierarchy(1, cLine, indx, valueN1);
                                    if (valueN1 > 0)
                                        reInitPDVPDMVoidColumns[indx] = true;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }

            foreach (int key in reInitPDVPDMVoidColumns.Keys)
            {

                if (!reInitPDVPDMVoidColumns[key])
                {
                    tab[0, key] = new CellPercent(0.0);
                }
            }
            #endregion

            return tab;
        }
        #endregion

        public GridResult GetGridResult()
        {
            GridResult gridResult = new GridResult();
            ResultTable resultTable = GetResult();
            string mediaSchedulePath = "/MediaSchedulePopUp";
            LineStart cLineStart = null;
            int nbLines = 0;

            if (resultTable == null || resultTable.DataColumnsNumber == 0)
            {
                gridResult.HasData = false;
                return gridResult;
            }
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
            resultTable.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
            resultTable.CultureInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            object[,] gridData = new object[nbLines, resultTable.ColumnsNumber + 1]; //+2 car ID et PID en plus  -  //_data.LinesNumber // + 1 for gad column
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();

            //Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            columns.Add(new { headerText = "GAD", key = "GAD", dataType = "string", width = "*", hidden = true });
            schemaFields.Add(new { name = "GAD" });
            List<object> groups = null;
            List<object> subGroups = null;
            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            string format = string.Empty;

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
                            colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j][g].IndexInResultTable);

                            if (resultTable != null && resultTable.LinesNumber > 0)
                            {
                                var cell = resultTable[0, resultTable.NewHeaders.Root[j][g].IndexInResultTable];

                                if (cell is CellPercent || cell is CellPDM)
                                {
                                    format = "percent";
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

                            if(resultTable.NewHeaders.Root[j][g].Count > 0)
                            {
                                subGroups = new List<object>();
                                subGroups.Add(new { headerText = resultTable.NewHeaders.Root[j][g][0].Label, key = colKey, dataType = "number", format = format, columnCssClass = "colStyle", width = "*", allowSorting = true });
                                groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = "subgr" + colKey, group = subGroups });
                                schemaFields.Add(new { name = colKey });
                            }
                            else
                            {
                                groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = colKey, dataType = "number", format = format, columnCssClass = "colStyle", width = "*", allowSorting = true });
                                schemaFields.Add(new { name = colKey });
                            }
                        }
                        //colKey = string.Format("gr{0}", resultTable.NewHeaders.Root[j].IndexInResultTable);
                        columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = "gr" + colKey, group = groups });
                        columnsFixed.Add(new { columnKey = "gr" + colKey, isFixed = false, allowFixing = false });
                    }
                    else
                    {
                        colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j].IndexInResultTable);
                        if (j == 0)
                        {
                            columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "350", allowSorting = true, template = "{{if ${GAD}.length > 0}} <span class=\"gadLink\" href=\"#gadModal\" data-toggle=\"modal\" data-gad=\"[${GAD}]\">${" + colKey + "}</span> {{else}} ${" + colKey + "} {{/if}}" });
                            columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });
                        }
                        else
                        {
                            columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "*" });
                            columnsFixed.Add(new { columnKey = colKey, isFixed = false, allowFixing = false });
                        }
                        schemaFields.Add(new { name = colKey });
                    }

                }
            }

            //table body rows
            int currentLine = 0;
            for (int i = 0; i < resultTable.LinesNumber; i++) //_data.LinesNumber
            {
                cLineStart = resultTable.GetLineStart(i);
                if (cLineStart is LineHide)
                    continue;

                gridData[currentLine, 0] = i; // Pour column ID
                gridData[currentLine, 1] = resultTable.GetSortedParentIndex(i); // Pour column PID

                for (int k = 1; k < resultTable.ColumnsNumber - 1; k++)
                {
                    var cell = resultTable[currentLine, k];
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
                        gridData[currentLine, k + 2] = link;

                    }

                    else
                    {
                        if (cell is CellPercent || cell is CellEvol || cell is CellPDM)
                        {
                            double value = ((CellUnit)cell).Value;

                            if (double.IsInfinity(value))
                                gridData[currentLine, k + 2] = (value < 0) ? "-Infinity" : "+Infinity";
                            else if (double.IsNaN(value))
                                gridData[currentLine, k + 2] = null;
                            else
                                gridData[currentLine, k + 2] = value / 100;
                        }
                        else if (cell is CellUnit)
                        {
                            if (((LineStart)resultTable[i, 0]).LineType != LineType.nbParution)
                                gridData[currentLine, k + 2] = FctWeb.Units.ConvertUnitValue(((CellUnit)cell).Value, _session.Unit);
                            else
                                gridData[currentLine, k + 2] = ((CellUnit)cell).Value;
                        }
                        else if (cell is AdExpressCellLevel)
                        {
                            string label = ((AdExpressCellLevel)cell).RawString();
                            string gadParams = ((AdExpressCellLevel)cell).GetGadParams();

                            if (gadParams.Length > 0)
                                gridData[currentLine, 2] = gadParams;
                            else
                                gridData[currentLine, 2] = "";

                            gridData[currentLine, k + 2] = label;
                        }
                        else
                        {
                            gridData[currentLine, k + 2] = cell.RenderString();
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
            gridResult.Data = gridData;
            gridResult.Unit = _session.Unit.ToString();

            return gridResult;
        }

        #region CanAddHeader
        private bool CanAddHeader(List<long> cId, List<long> oldcId, List<int> DATA_MEDIA_INDEXES, HeaderBase[] MEDIA_IDS, int i)
        {
            if (DATA_MEDIA_INDEXES.Count == 3)
                return (cId[0] != oldcId[0] || (DATA_MEDIA_INDEXES.Count > 1 && cId[1] != oldcId[1]) || (DATA_MEDIA_INDEXES.Count > 2 && cId[2] != oldcId[2]));
            else return (MEDIA_IDS[i] == null || (cId[i] != MEDIA_IDS[i].Id || (i > 0 && oldcId[0] != cId[0])));
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

        /// <summary>
        /// Get Total Media Text
        /// </summary>
        /// <returns>Total media text</returns>
        protected string GetTotalMediaText()
        {
            switch (_session.PreformatedMediaDetail)
            {
                case CstFormat.PreformatedMediaDetails.vehicle:
                case CstFormat.PreformatedMediaDetails.vehicleCategory:
                case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
                case CstFormat.PreformatedMediaDetails.vehicleMedia:
                case CstFormat.PreformatedMediaDetails.vehicleMediaSeller:
                    return GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " " + GestionWeb.GetWebWord(363, _session.SiteLanguage);
                case CstFormat.PreformatedMediaDetails.category:
                    return GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " " + GestionWeb.GetWebWord(1382, _session.SiteLanguage);
                case CstFormat.PreformatedMediaDetails.mediaSeller:
                case CstFormat.PreformatedMediaDetails.mediaSellerVehicle:
                case CstFormat.PreformatedMediaDetails.mediaSellerMedia:
                case CstFormat.PreformatedMediaDetails.mediaSellerCategory:
                    return GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " " + GestionWeb.GetWebWord(1383, _session.SiteLanguage);
                case CstFormat.PreformatedMediaDetails.Media:
                    return GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " " + GestionWeb.GetWebWord(18, _session.SiteLanguage);
                default:
                    return GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " " + GestionWeb.GetWebWord(363, _session.SiteLanguage);
            }
        }
        #endregion

    }
}
