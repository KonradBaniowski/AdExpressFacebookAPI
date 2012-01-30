using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;

using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctUtilities = TNS.AdExpress.Web.Functions;

using TNS.Classification.Universe;
using TNS.AdExpressI.ProductClassReports.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;

namespace TNS.AdExpressI.ProductClassReports.GenericEngines
{

    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Year X Classif2
    /// </summary>
    public class GenericEngine_Classif1Year_X_Classif2 : GenericEngine
    {

        #region Constants
        protected const Int32 ID_PRODUCT = -1;
        protected const Int32 ID_TOTAL = -2;
        protected const Int32 ID_YEAR_N = -3;
        protected const Int32 ID_YEAR_N1 = -4;
        protected const Int32 ID_EVOL = -5;
        protected const Int32 ID_PDV_N = -6;
        protected const Int32 ID_PDV_N1 = -7;
        protected const Int32 ID_PDM_N = -8;
        protected const Int32 ID_PDM_N1 = -9;
        #endregion

        private Int32 LEVEL_OFFSET = 2;

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public GenericEngine_Classif1Year_X_Classif2(WebSession session, int result) : base(session, result) { }
        #endregion

        #region Implementation
        protected override ResultTable ComputeData(DataSet data)
        {
            ProductClassResultTable tab = null;

            #region Data
            DataTable dtData = data.Tables[0];
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
                    if (_vehicle == CstDBClassif.Vehicles.names.plurimedia || (!firstMedia))//|| _session.PreformatedMediaDetail == CstFormat.PreformatedMediaDetails.vehicle
                    //if (!firstMedia)
                    {
                        DATA_MEDIA_INDEXES.Add(i);
                    }
                    firstMedia = false;
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

            #region Personal advertisers
            _isPersonalized = 0;
            if (dtData.Columns.Contains("inref"))
            {
                _isPersonalized = 3;
            }
            #endregion

            //delete useless lines
            CleanDataTable(dtData, FIRST_DATA_INDEX);
            if (dtData.Rows.Count <= 0) return null;

            #region Periods
            DateTime begin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);

            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            string periodEnd = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);
            int yearN = Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4));
            int yearN1 = _session.ComparativeStudy ? yearN - 1 : -1;
            Int32 DATA_YEAR_N = FIRST_DATA_INDEX;
            Int32 DATA_YEAR_N1 = (yearN1 > 0) ? FIRST_DATA_INDEX + 1 : -1;
            Int32 RES_YEAR_N_OFFSET = 1;
            List<Int64> keyYearN = new List<Int64>(); keyYearN.Add(ID_YEAR_N);
            Int32 RES_YEAR_N1_OFFSET = -1;
            if (yearN1 > 0)
            {
                RES_YEAR_N1_OFFSET = 2;
                LEVEL_OFFSET++;
            }
            List<Int64> keyYearN1 = new List<Int64>(); keyYearN1.Add(ID_YEAR_N1);
            Int32 RES_EVOL_OFFSET = -1;
            if (_session.Evolution && RES_YEAR_N1_OFFSET > 0)
            {
                RES_EVOL_OFFSET = RES_YEAR_N1_OFFSET + 1;
                LEVEL_OFFSET++;
            }
            List<Int64> keyEvol = new List<Int64>(); keyEvol.Add(ID_EVOL);
            Int32 RES_PDV_N_OFFSET = -1;
            if (_session.PDV)
            {
                RES_PDV_N_OFFSET = Math.Max(RES_YEAR_N_OFFSET, Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET)) + 1;
                LEVEL_OFFSET++;
            }
            List<Int64> keyPdvYearN = new List<Int64>(); keyPdvYearN.Add(ID_PDV_N);
            Int32 RES_PDV_N1_OFFSET = -1;
            if (yearN1 > 0 && _session.PDV)
            {
                RES_PDV_N1_OFFSET = RES_PDV_N_OFFSET + 1;
                LEVEL_OFFSET++;
            }
            List<Int64> keyPdvYearN1 = new List<Int64>(); keyPdvYearN1.Add(ID_PDV_N1);
            Int32 RES_PDM_N_OFFSET = -1;
            if (_session.PDM)
            {
                RES_PDM_N_OFFSET = Math.Max(RES_PDV_N1_OFFSET, Math.Max(Math.Max(RES_YEAR_N_OFFSET, RES_PDV_N_OFFSET), Math.Max(RES_YEAR_N1_OFFSET, RES_EVOL_OFFSET))) + 1;
                LEVEL_OFFSET++;
            }
            List<Int64> keyPdmYearN = new List<Int64>(); keyPdmYearN.Add(ID_PDM_N);
            Int32 RES_PDM_N1_OFFSET = -1;
            if (yearN1 > 0 && _session.PDM)
            {
                RES_PDM_N1_OFFSET = RES_PDM_N_OFFSET + 1;
                LEVEL_OFFSET++;
            }
            List<Int64> keyPdmYearN1 = new List<Int64>(); keyPdmYearN1.Add(ID_PDM_N1);
            string labelN = FctUtilities.Dates.getPeriodLabel(_session, CstPeriod.Type.currentYear);
            string labelN1 = FctUtilities.Dates.getPeriodLabel(_session, CstPeriod.Type.previousYear);
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
            switch (_vehicle)
            {
                case CstDBClassif.Vehicles.names.plurimedia:
                    headers.Root.Add(new Header(true, GestionWeb.GetWebWord(210, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.press:
                    //headers.Root.Add(new Header(true, GestionWeb.GetWebWord(204, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.magazine:
                    //headers.Root.Add(new Header(true, GestionWeb.GetWebWord(204, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.newspaper:
                    //headers.Root.Add(new Header(true, GestionWeb.GetWebWord(204, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.radio:
                case CstDBClassif.Vehicles.names.radioGeneral:
                case CstDBClassif.Vehicles.names.radioSponsorship:
                case CstDBClassif.Vehicles.names.radioMusic:
					//headers.Root.Add(new Header(true, GestionWeb.GetWebWord(205, _session.SiteLanguage).ToUpper(), ID_TOTAL));
					headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.tv:
                case CstDBClassif.Vehicles.names.tvGeneral:
                case CstDBClassif.Vehicles.names.tvSponsorship:
                case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                case CstDBClassif.Vehicles.names.tvAnnounces:
                    //headers.Root.Add(new Header(true, GestionWeb.GetWebWord(206, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.mediasTactics:
                    //headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1304, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.internet:
                    //headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1301, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.outdoor:
                case CstDBClassif.Vehicles.names.indoor:
                case CstDBClassif.Vehicles.names.instore:
					//headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1302, _session.SiteLanguage).ToUpper(), ID_TOTAL));
					headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.cinema:
                    //headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1303, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.emailing:
                    //headers.Root.Add(new Header(true, "E mailing".ToUpper(), ID_TOTAL));
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
            }
            //Go threw data to extract media levels
            string sortStr = "";
            switch (_session.PreformatedMediaDetail)
            {
                case CstFormat.PreformatedMediaDetails.vehicle:
                    sortStr = "M1,ID_M1";
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategory:
                case CstFormat.PreformatedMediaDetails.vehicleMedia:
                    if (_vehicle != CstDBClassif.Vehicles.names.plurimedia)
                    {
                        sortStr = "M2,ID_M2";
                    }
                    else
                    {
                        sortStr = "M1,ID_M1,M2,ID_M2";
                    }
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
                    sortStr = "M2,ID_M2,M3,ID_M3";
                    break;
                default:
                    throw new ProductClassReportsException("Detail format " + _session.PreformatedMediaDetail.ToString() + " unvalid.");
            }
            DataRow[] dtMedias = dtData.Select("", sortStr);

            Dictionary<long, Header> RES_MEDIA_HEADERS = new Dictionary<long, Header>();
            Dictionary<long, HeaderBase> RES_MEDIA_SUBTOTAL = new Dictionary<long, HeaderBase>();
            HeaderBase[] MEDIA_IDS = new HeaderBase[DATA_MEDIA_INDEXES.Count];
            List<HeaderBase> SUB_TOTALS = new List<HeaderBase>();
            long cId = -1;
            HeaderBase pHeaderBase = headers.Root;
            foreach (DataRow row in dtMedias)
            {

                for (int i = 0; i < DATA_MEDIA_INDEXES.Count; i++)
                {
                    cId = Convert.ToInt64(row[DATA_MEDIA_INDEXES[i]]);
                    if (MEDIA_IDS[i] == null || cId != MEDIA_IDS[i].Id)
                    {
                        if (i < DATA_MEDIA_INDEXES.Count - 1)
                        {
                            MEDIA_IDS[i] = new HeaderGroup(row[DATA_MEDIA_INDEXES[i] + 1].ToString(), cId);
                            SUB_TOTALS.Add(((HeaderGroup)MEDIA_IDS[i]).AddSubTotal(true, GestionWeb.GetWebWord(1102, _session.SiteLanguage), ID_TOTAL));
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
                            MEDIA_IDS[i] = new Header(true, row[DATA_MEDIA_INDEXES[i] + 1].ToString(), cId);
                            RES_MEDIA_HEADERS.Add(cId, (Header)MEDIA_IDS[i]);
                            pHeaderBase.Add(MEDIA_IDS[i]);
                            if (pHeaderBase is HeaderGroup)
                            {
                                RES_MEDIA_SUBTOTAL.Add(cId, SUB_TOTALS[SUB_TOTALS.Count - 1]);
                            }
                        }
                    }
                }

            }
            #endregion

            #region Init Result Table
            tab = new ProductClassResultTable(dtData.Rows.Count * 2, headers);
            tab.SortSubLevels = false;
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();
            CellUnit cell = cellFactory.Get(0.0);
            cell.DisplayContent = false;
            CellUnitFactory cellHiddenFactory = new CellUnitFactory(cell);
            #endregion

            #region Total line
            CellLevel[] parents = new CellLevel[DATA_PRODUCT_INDEXES.Count + 1];
            List<Int64> keys = new List<Int64>();
            keys.Add(ID_TOTAL);
            //Total
            int cLine = 0;
            cLine = tab.AddNewLine(LineType.total, keys, parents[0] = new CellLevel(ID_TOTAL, GestionWeb.GetWebWord(1401, _session.SiteLanguage), 0, cLine));
            //YearN
            tab.AddNewLine(LineType.subTotal1, keys, keyYearN, new CellLevel(ID_YEAR_N, labelN, (CellLevel)tab[cLine, 1], 0, cLine + RES_YEAR_N_OFFSET));
            //YearN1
            if (RES_YEAR_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyYearN1, new CellLevel(ID_YEAR_N1, labelN1, 0, cLine + RES_YEAR_N1_OFFSET));
            }
            //Evol
            if (RES_EVOL_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyEvol, new CellLevel(ID_EVOL, labelEvol, 0, cLine + RES_EVOL_OFFSET));
            }
            //PDV N
            if (RES_PDV_N_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdvYearN, new CellLevel(ID_PDV_N, labelPDVN, 0, cLine + RES_PDV_N_OFFSET));
            }
            //PDV N1
            if (RES_PDV_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdvYearN1, new CellLevel(ID_PDV_N1, labelPDVN1, 0, cLine + RES_PDV_N1_OFFSET));
            }
            for (int i = 2; i <= tab.DataColumnsNumber; i++)
            {
                //Total
                tab[cLine, i] = cellHiddenFactory.Get(0.0);
                //YearN
                tab[cLine + RES_YEAR_N_OFFSET, i] = cellFactory.Get(0.0);
                //YearN1
                if (RES_YEAR_N1_OFFSET > 0)
                {
                    tab[cLine + RES_YEAR_N1_OFFSET, i] = cellFactory.Get(0.0);
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
                    tab[cLine + RES_PDV_N_OFFSET, i] = new CellPDM(0.0, null);
                    ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, i]).StringFormat = "{0:percentage}";
                }
                //PDV N1
                if (RES_PDV_N1_OFFSET > 0)
                {
                    tab[cLine + RES_PDV_N1_OFFSET, i] = new CellPDM(0.0, null);
                    ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, i]).StringFormat = "{0:percentage}";
                }
            }
            //PDM N
            if (RES_PDM_N_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdmYearN, new CellLevel(ID_PDM_N, labelPDMN, 0, cLine + RES_PDM_N_OFFSET));
                tab[cLine + RES_PDM_N_OFFSET, 2] = new CellPDM(0.0, null);
                ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, 2]).StringFormat = "{0:percentage}";
            }
            //PDM N1
            if (RES_PDM_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdmYearN1, new CellLevel(ID_PDM_N1, labelPDMN1, 0, cLine + RES_PDM_N1_OFFSET));
                tab[cLine + RES_PDM_N1_OFFSET, 2] = new CellPDM(0.0, null);
                ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
            }
            //Init sub total PDM
            foreach (HeaderBase h in SUB_TOTALS)
            {
                //PDM N
                if (RES_PDM_N_OFFSET > 0)
                {
                    tab[cLine + RES_PDM_N_OFFSET, h.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, 2]);
                    ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDM N1
                if (RES_PDM_N1_OFFSET > 0)
                {
                    tab[cLine + RES_PDM_N1_OFFSET, h.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                    ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                }

            }
            //Init Media PDM
            foreach (KeyValuePair<long, Header> record in RES_MEDIA_HEADERS)
            {
                //PDM N
                if (RES_PDM_N_OFFSET > 0)
                {
                    if (RES_MEDIA_SUBTOTAL.ContainsKey(record.Key))
                    {
                        tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, RES_MEDIA_SUBTOTAL[record.Key].IndexInResultTable]);
                    }
                    else
                    {
                        tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, 2]);
                    }
                    ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDM N1
                if (RES_PDM_N1_OFFSET > 0)
                {
                    if (RES_MEDIA_SUBTOTAL.ContainsKey(record.Key))
                    {
                        tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, RES_MEDIA_SUBTOTAL[record.Key].IndexInResultTable]);
                    }
                    else
                    {
                        tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                    }
                    ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                }

            }
            #endregion

            #region Fill table          
            List<LineType> lTypes = new List<LineType> { LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
            List<LineType> lSubTypes = new List<LineType> { LineType.level5, LineType.level6, LineType.level7, LineType.level8 };
            Double valueN = 0;
            Double valueN1 = 0;
            Int32 subTotalIndex = -1;
            List<DetailLevelItemInformation> levels = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
            levels.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sector));

            foreach (DataRow row in dtData.Rows)
            {
                for (int i = 0; i < DATA_PRODUCT_INDEXES.Count; i++)
                {
                    cId = Convert.ToInt64(row[DATA_PRODUCT_INDEXES[i]]);
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

                        #region Init cells
                        //Total
                        cLine = tab.AddNewLine(lTypes[i], keys, parents[i + 1] = new CellLevel(cId, row[DATA_PRODUCT_INDEXES[i] + 1].ToString(), (CellLevel)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, 1], i + 1, cLine));
                        //YearN
                        tab.AddNewLine(lSubTypes[i], keys, keyYearN, new CellLevel(ID_YEAR_N, labelN, (CellLevel)tab[cLine, 1], i + 1, cLine + RES_YEAR_N_OFFSET));
                        //YearN1
                        if (RES_YEAR_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyYearN1, new CellLevel(ID_YEAR_N1, labelN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, 1], i + 1, cLine + RES_YEAR_N1_OFFSET));
                        }
                        //Evol
                        if (RES_EVOL_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyEvol, new CellLevel(ID_EVOL, labelEvol, i + 1, cLine + RES_EVOL_OFFSET));
                        }
                        //PDV N
                        if (RES_PDV_N_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdvYearN, new CellLevel(ID_PDV_N, labelPDVN, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDV_N_OFFSET, 1], i + 1, cLine + RES_PDV_N_OFFSET));
                        }
                        //PDV N1
                        if (RES_PDV_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdvYearN1, new CellLevel(ID_PDV_N1, labelPDVN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDV_N1_OFFSET, 1], i + 1, cLine + RES_PDV_N1_OFFSET));
                        }
                        for (int k = 2; k <= tab.DataColumnsNumber; k++)
                        {
                            //Total
                            tab[cLine, k] = cellHiddenFactory.Get(0.0);
                            //YearN
                            tab[cLine + RES_YEAR_N_OFFSET, k] = cellFactory.Get(0.0);
                            //YearN1
                            if (RES_YEAR_N1_OFFSET > 0)
                            {
                                tab[cLine + RES_YEAR_N1_OFFSET, k] = cellFactory.Get(0.0);
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
                                tab[cLine + RES_PDV_N_OFFSET, k] = new CellPDM(0.0, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, k]);
                                ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, k]).StringFormat = "{0:percentage}";
                            }
                            //PDV N1
                            if (RES_PDV_N1_OFFSET > 0)
                            {
                                tab[cLine + RES_PDV_N1_OFFSET, k] = new CellPDM(0.0, (CellUnit)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, k]);
                                ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, k]).StringFormat = "{0:percentage}";
                            }
                        }
                        //PDM N
                        if (RES_PDM_N_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdmYearN, new CellLevel(ID_PDM_N, labelPDMN, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDM_N_OFFSET, 1], i + 1, cLine + RES_PDM_N_OFFSET));
                            tab[cLine + RES_PDM_N_OFFSET, 2] = new CellPDM(0.0, null);
                            ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, 2]).StringFormat = "{0:percentage}";
                        }
                        //PDM N1
                        if (RES_PDM_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdmYearN1, new CellLevel(ID_PDM_N1, labelPDMN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDM_N1_OFFSET, 1], i + 1, cLine + RES_PDM_N1_OFFSET));
                            tab[cLine + RES_PDM_N1_OFFSET, 2] = new CellPDM(0.0, null);
                            ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
                        }
                        //Init sub total PDM
                        foreach (HeaderBase h in SUB_TOTALS)
                        {
                            //PDM N
                            if (RES_PDM_N_OFFSET > 0)
                            {
                                tab[cLine + RES_PDM_N_OFFSET, h.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, 2]);
                                ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDM N1
                            if (RES_PDM_N1_OFFSET > 0)
                            {
                                tab[cLine + RES_PDM_N1_OFFSET, h.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                                ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }

                        }
                        //Init Media PDM
                        foreach (KeyValuePair<long, Header> record in RES_MEDIA_HEADERS)
                        {
                            //PDM N
                            if (RES_PDM_N_OFFSET > 0)
                            {
                                if (RES_MEDIA_SUBTOTAL.ContainsKey(record.Key))
                                {
                                    tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, RES_MEDIA_SUBTOTAL[record.Key].IndexInResultTable]);
                                }
                                else
                                {
                                    tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, 2]);
                                }
                                ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDM N1
                            if (RES_PDM_N1_OFFSET > 0)
                            {
                                if (RES_MEDIA_SUBTOTAL.ContainsKey(record.Key))
                                {
                                    tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, RES_MEDIA_SUBTOTAL[record.Key].IndexInResultTable]);
                                }
                                else
                                {
                                    tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(0.0, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                                }
                                ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }

                        }
                        #endregion

                    }
                }

                #region Add Values
                cId = -1;
                if (DATA_MEDIA_INDEXES.Count > 0)
                    cId = Convert.ToInt32(row[DATA_MEDIA_INDEXES[DATA_MEDIA_INDEXES.Count - 1]]);
                subTotalIndex = (RES_MEDIA_SUBTOTAL.ContainsKey(cId)) ? RES_MEDIA_SUBTOTAL[cId].IndexInResultTable : -1;
                valueN = Convert.ToDouble(row[DATA_YEAR_N]);
                if (DATA_YEAR_N1 > -1)
                {
                    valueN1 = Convert.ToDouble(row[DATA_YEAR_N1]);
                }
                //N
                if (cId > -1)
                    tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N_OFFSET, RES_MEDIA_HEADERS[cId].IndexInResultTable, valueN);
                tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N_OFFSET, 2, valueN);
                if (subTotalIndex > -1)
                {
                    tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N_OFFSET, subTotalIndex, valueN);
                }
                //N1
                if (RES_YEAR_N1_OFFSET > -1)
                {
                    if (cId > -1)
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N1_OFFSET, RES_MEDIA_HEADERS[cId].IndexInResultTable, valueN1);
                    tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N1_OFFSET, 2, valueN1);
                    if (subTotalIndex > -1)
                    {
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_YEAR_N1_OFFSET, subTotalIndex, valueN1);
                    }
                }
                //PDV N
                if (RES_PDV_N_OFFSET > -1)
                {
                    if (cId > -1)
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N_OFFSET, RES_MEDIA_HEADERS[cId].IndexInResultTable, valueN);
                    tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N_OFFSET, 2, valueN);
                    if (subTotalIndex > -1)
                    {
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N_OFFSET, subTotalIndex, valueN);
                    }
                }
                //PDV N1
                if (RES_PDV_N1_OFFSET > -1)
                {
                    if (cId > -1)
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N1_OFFSET, RES_MEDIA_HEADERS[cId].IndexInResultTable, valueN1);
                    tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N1_OFFSET, 2, valueN1);
                    if (subTotalIndex > -1)
                    {
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDV_N1_OFFSET, subTotalIndex, valueN1);
                    }
                }
                //PDM N
                if (RES_PDM_N_OFFSET > -1)
                {
                    if (cId > -1)
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N_OFFSET, RES_MEDIA_HEADERS[cId].IndexInResultTable, valueN);
                    tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N_OFFSET, 2, valueN);
                    if (subTotalIndex > -1)
                    {
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N_OFFSET, subTotalIndex, valueN);
                    }
                }
                //PDM N1
                if (RES_PDM_N1_OFFSET > -1)
                {
                    if (cId > -1)
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N1_OFFSET, RES_MEDIA_HEADERS[cId].IndexInResultTable, valueN1);
                    tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N1_OFFSET, 2, valueN1);
                    if (subTotalIndex > -1)
                    {
                        tab.AffectValueAndAddToHierarchy(1, cLine + RES_PDM_N1_OFFSET, subTotalIndex, valueN1);
                    }
                }
                #endregion

                #region Advertisers univers
                if (_isPersonalized > 0)
                {
                    for (int i = parents.Length - 1; i >= 0; i--)
                    {
                        SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable, row, levels[i].Id);
                        SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, row, levels[i].Id);
                        //YearN1
                        if (RES_YEAR_N1_OFFSET > 0)
                        {
                            SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, row, levels[i].Id);
                        }
                        //Evol
                        if (RES_EVOL_OFFSET > 0)
                        {
                            SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_EVOL_OFFSET, row, levels[i].Id);
                        }
                        //PDV N
                        if (RES_PDV_N_OFFSET > 0)
                        {
                            SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDV_N_OFFSET, row, levels[i].Id);
                        }
                        //PDV N1
                        if (RES_PDV_N1_OFFSET > 0)
                        {
                            SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDV_N1_OFFSET, row, levels[i].Id);
                        }
                        //PDM N
                        if (RES_PDM_N_OFFSET > 0)
                        {
                            SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDM_N_OFFSET, row, levels[i].Id);
                        }
                        //PDM N1
                        if (RES_PDM_N1_OFFSET > 0)
                        {
                            SetPersoAdvertiser(tab, parents[i].LineIndexInResultTable + RES_PDM_N1_OFFSET, row, levels[i].Id);
                        }
                    }
                }
                #endregion

            }
            #endregion

            #region Hide lines if required
            if (_session.PersonalizedElementsOnly && _isPersonalized > 0)
            {
                HideNonCustomisedLines(tab, lTypes, lSubTypes);
            }
            #endregion


            return tab;
        }

        



        #endregion




    }

}
