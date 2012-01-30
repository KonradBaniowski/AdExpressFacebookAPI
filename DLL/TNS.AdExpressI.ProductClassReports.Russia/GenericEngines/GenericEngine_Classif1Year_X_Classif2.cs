using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;

using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstClassif = TNS.AdExpress.Constantes.Classification;
using FctUtilities = TNS.AdExpress.Web.Functions;

using TNS.Classification.Universe;
using TNS.AdExpressI.ProductClassReports.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.ProductClassReports.GenericEngines;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Domain.Level;
using System.Reflection;
using TNS.AdExpress.Constantes;

namespace TNS.AdExpressI.ProductClassReports.Russia.GenericEngines
{

    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Year X Classif2
    /// </summary>
    public class GenericEngine_Classif1Year_X_Classif2 : TNS.AdExpressI.ProductClassReports.GenericEngines.GenericEngine_Classif1Year_X_Classif2
    {

        #region Attributes
        #endregion

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public GenericEngine_Classif1Year_X_Classif2(WebSession session, int result) : base(session, result) { }
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
            _isPersonalized = 0;
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
            int vehicleId = Convert.ToInt32(dtData.Rows[0]["ID_M1"]);
            switch (_vehicle)
            {
                case CstDBClassif.Vehicles.names.plurimedia:
                    headers.Root.Add(new Header(true, GestionWeb.GetWebWord(210, _session.SiteLanguage).ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.press:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.magazine:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.newspaper:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.radio:
                case CstDBClassif.Vehicles.names.radioGeneral:
                case CstDBClassif.Vehicles.names.radioSponsorship:
                case CstDBClassif.Vehicles.names.radioMusic:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.tv:
                case CstDBClassif.Vehicles.names.tvGeneral:
                case CstDBClassif.Vehicles.names.tvSponsorship:
                case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                case CstDBClassif.Vehicles.names.tvAnnounces:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.mediasTactics:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.internet:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.outdoor:
                case CstDBClassif.Vehicles.names.indoor:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.cinema:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
                case CstDBClassif.Vehicles.names.emailing:
                    headers.Root.Add(new Header(true, vehicleLabel.ToUpper(), ID_TOTAL));
                    break;
            }
            
            Dictionary<Int32, Header> RES_MEDIA_HEADERS = new Dictionary<Int32, Header>();
            Dictionary<Int32, HeaderBase> RES_MEDIA_SUBTOTAL = new Dictionary<Int32, HeaderBase>();
            List<DetailLevelItemInformation> mediaLevels = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
            HeaderBase[] MEDIA_IDS = new HeaderBase[mediaLevels.Count];
            List<HeaderBase> SUB_TOTALS = new List<HeaderBase>();
            Int32 cId = -1;
            HeaderBase pHeaderBase = headers.Root;
            string C2_ID_NAME = "ID_M", C2_IDS_KEY = string.Empty;
            string C2_LABEL_NAME = "M";
            int mediaLevelsCount = 0;
            int mediaLevelsIndex = 0;
            DataTable dtHeader = dsData.Tables["TOTAL_" + mediaLevels.Count];
            foreach (DataRow row in dtHeader.Rows)
            {

                if (_vehicle == CstDBClassif.Vehicles.names.plurimedia)
                {
                    mediaLevelsCount = mediaLevels.Count;
                    mediaLevelsIndex = 1;
                }
                else
                {
                    mediaLevelsCount = mediaLevels.Count - 1;
                    mediaLevelsIndex = 2;
                }

                for (int i = 0; i < mediaLevelsCount; i++)
                {
                    cId = Convert.ToInt32(row[C2_ID_NAME + mediaLevelsIndex]);
                    if (MEDIA_IDS[i] == null || cId != MEDIA_IDS[i].Id)
                    {
                        if (i < mediaLevelsCount - 1)
                        {
                            MEDIA_IDS[i] = new HeaderGroup(row[C2_LABEL_NAME + mediaLevelsIndex].ToString(), cId);
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
                            MEDIA_IDS[i] = new Header(true, row[C2_LABEL_NAME + mediaLevelsIndex].ToString(), cId);
                            RES_MEDIA_HEADERS.Add(cId, (Header)MEDIA_IDS[i]);
                            pHeaderBase.Add(MEDIA_IDS[i]);
                            if (pHeaderBase is HeaderGroup)
                            {
                                RES_MEDIA_SUBTOTAL.Add(cId, SUB_TOTALS[SUB_TOTALS.Count - 1]);
                            }
                        }
                        
                    }
                    mediaLevelsIndex++;
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
            List<DetailLevelItemInformation> productLevels = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
            productLevels.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sector));
            CellLevel[] parents = new CellLevel[productLevels.Count];
            List<Int64> keys = new List<Int64>();
            Double investN = 0, investN1 = 0;
            string[] tmp;

            Dictionary<string, Dictionary<string, DataRow>> globalDictionary = CommonFunctions.InitDictionariesData(dsData, productLevels.Count - 1, mediaLevels.Count, CstClassif.Branch.type.product, CstClassif.Branch.type.media);

            DataRow currentTotalRow = globalDictionary["TOTAL"]["TOTAL"];

            keys.Add(ID_TOTAL);

            #region Init Total
            //Total
            int cLine = 0;
            cLine = tab.AddNewLine(LineType.total, keys, parents[0] = new CellLevel(ID_TOTAL, GestionWeb.GetWebWord(1401, _session.SiteLanguage), 0, cLine, textWrap.NbChar, textWrap.Offset));
            tab[cLine, 2] = cellHiddenFactory.Get(0.0);
            //YearN
            tab.AddNewLine(LineType.subTotal1, keys, keyYearN, new CellLevel(ID_YEAR_N, labelN, (CellLevel)tab[cLine, 1], 0, cLine + RES_YEAR_N_OFFSET, textWrap.NbChar, textWrap.Offset));
            investN = Convert.ToDouble(currentTotalRow["N"]);
            tab[cLine + RES_YEAR_N_OFFSET, 2] = cellFactory.Get(investN);
            //YearN1
            if (RES_YEAR_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyYearN1, new CellLevel(ID_YEAR_N1, labelN1, 0, cLine + RES_YEAR_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                investN1 = Convert.ToDouble(currentTotalRow["N1"]);
                tab[cLine + RES_YEAR_N1_OFFSET, 2] = cellFactory.Get(investN1);
            }
            //Evol
            if (RES_EVOL_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyEvol, new CellLevel(ID_EVOL, labelEvol, 0, cLine + RES_EVOL_OFFSET, textWrap.NbChar, textWrap.Offset));
                tab[cLine + RES_EVOL_OFFSET, 2] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, 2], tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                ((CellEvol)tab[cLine + RES_EVOL_OFFSET, 2]).StringFormat = "{0:percentage}";
            }
            //PDV N
            if (RES_PDV_N_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdvYearN, new CellLevel(ID_PDV_N, labelPDVN, 0, cLine + RES_PDV_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                tab[cLine + RES_PDV_N_OFFSET, 2] = new CellPDM(investN, null);
                ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, 2]).StringFormat = "{0:percentage}";
            }
            //PDV N1
            if (RES_PDV_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdvYearN1, new CellLevel(ID_PDV_N1, labelPDVN1, 0, cLine + RES_PDV_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                tab[cLine + RES_PDV_N1_OFFSET, 2] = new CellPDM(investN1, null);
                ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
            }
            for (int i = 3; i <= tab.DataColumnsNumber; i++)
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
                tab.AddNewLine(LineType.subTotal1, keys, keyPdmYearN, new CellLevel(ID_PDM_N, labelPDMN, 0, cLine + RES_PDM_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                tab[cLine + RES_PDM_N_OFFSET, 2] = new CellPDM(investN, null);
                ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, 2]).StringFormat = "{0:percentage}";
            }
            //PDM N1
            if (RES_PDM_N1_OFFSET > 0)
            {
                tab.AddNewLine(LineType.subTotal1, keys, keyPdmYearN1, new CellLevel(ID_PDM_N1, labelPDMN1, 0, cLine + RES_PDM_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                tab[cLine + RES_PDM_N1_OFFSET, 2] = new CellPDM(investN1, null);
                ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
            }
            #endregion

            #region Init sub total
            DataRow currentSubTotalRow;
            //Init sub total
            foreach (HeaderBase h in SUB_TOTALS)
            {
                string keyString = string.Empty;
                tmp = h.Key.Split('-');
                if (_vehicle == CstDBClassif.Vehicles.names.plurimedia)
                {
                    mediaLevelsIndex = 1;
                    currentSubTotalRow = globalDictionary["TOTAL_" + mediaLevelsIndex][tmp[0]];
                }
                else
                {
                    mediaLevelsIndex = 2;
                    keyString = vehicleId.ToString() + "_" + tmp[0];
                    currentSubTotalRow = globalDictionary["TOTAL_" + mediaLevelsIndex][keyString];
                }
                
                //YearN
                investN = Convert.ToDouble(currentSubTotalRow["N"]);
                tab[cLine + RES_YEAR_N_OFFSET, h.IndexInResultTable] = cellFactory.Get(investN);
                //YearN1
                if (RES_YEAR_N1_OFFSET > 0)
                {
                    investN1 = Convert.ToDouble(currentSubTotalRow["N1"]);
                    tab[cLine + RES_YEAR_N1_OFFSET, h.IndexInResultTable] = cellFactory.Get(investN1);
                }
                //Evol
                if (RES_EVOL_OFFSET > 0)
                {
                    tab[cLine + RES_EVOL_OFFSET, h.IndexInResultTable] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, h.IndexInResultTable], tab[cLine + RES_YEAR_N1_OFFSET, h.IndexInResultTable]);
                    ((CellEvol)tab[cLine + RES_EVOL_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDV N
                if (RES_PDV_N_OFFSET > 0)
                {
                    tab[cLine + RES_PDV_N_OFFSET, h.IndexInResultTable] = new CellPDM(investN, null);
                    ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDV N1
                if (RES_PDV_N1_OFFSET > 0)
                {
                    tab[cLine + RES_PDV_N1_OFFSET, h.IndexInResultTable] = new CellPDM(investN1, null);
                    ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDM N
                if (RES_PDM_N_OFFSET > 0)
                {
                    tab[cLine + RES_PDM_N_OFFSET, h.IndexInResultTable] = new CellPDM(investN, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, 2]);
                    ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDM N1
                if (RES_PDM_N1_OFFSET > 0)
                {
                    tab[cLine + RES_PDM_N1_OFFSET, h.IndexInResultTable] = new CellPDM(investN1, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                    ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                }

            }
            #endregion

            #region Init Media
            DataRow currentMediaRow;
            //Init Media
            foreach (KeyValuePair<Int32, Header> record in RES_MEDIA_HEADERS)
            {
                string keyString = string.Empty;
                tmp = record.Value.Key.Split('-');
                if (_vehicle == CstDBClassif.Vehicles.names.plurimedia)
                {
                    if (mediaLevels.Count == 1)
                        keyString = tmp[0];
                    else
                        keyString = tmp[0] + "_" + tmp[tmp.Length - 1];
                }
                else
                {
                    if (mediaLevels.Count == 2)
                    {
                        keyString = vehicleId.ToString() + "_" + tmp[0];
                    }
                    else
                    {
                        keyString = vehicleId.ToString() + "_" + tmp[0] + "_" + tmp[tmp.Length - 1];
                    }
                }

                currentMediaRow = globalDictionary["TOTAL_" + mediaLevels.Count][keyString];

                //YearN
                investN = Convert.ToDouble(currentMediaRow["N"]);
                tab[cLine + RES_YEAR_N_OFFSET, record.Value.IndexInResultTable] = cellFactory.Get(investN);
                //YearN1
                if (RES_YEAR_N1_OFFSET > 0)
                {
                    investN1 = Convert.ToDouble(currentMediaRow["N1"]);
                    tab[cLine + RES_YEAR_N1_OFFSET, record.Value.IndexInResultTable] = cellFactory.Get(investN1);
                }
                //Evol
                if (RES_EVOL_OFFSET > 0)
                {
                    tab[cLine + RES_EVOL_OFFSET, record.Value.IndexInResultTable] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, record.Value.IndexInResultTable], tab[cLine + RES_YEAR_N1_OFFSET, record.Value.IndexInResultTable]);
                    ((CellEvol)tab[cLine + RES_EVOL_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDV N
                if (RES_PDV_N_OFFSET > 0)
                {
                    tab[cLine + RES_PDV_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN, null);
                    ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDV N1
                if (RES_PDV_N1_OFFSET > 0)
                {
                     tab[cLine + RES_PDV_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN1, null);
                    ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDM N
                if (RES_PDM_N_OFFSET > 0)
                {
                    if (RES_MEDIA_SUBTOTAL.ContainsKey(record.Key))
                    {
                        tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, RES_MEDIA_SUBTOTAL[record.Key].IndexInResultTable]);
                    }
                    else
                    {
                        tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, 2]);
                    }
                    ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                }
                //PDM N1
                if (RES_PDM_N1_OFFSET > 0)
                {
                    if (RES_MEDIA_SUBTOTAL.ContainsKey(record.Key))
                    {
                        tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN1, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, RES_MEDIA_SUBTOTAL[record.Key].IndexInResultTable]);
                    }
                    else
                    {
                        tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN1, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                    }
                    ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                }

            }
            #endregion

            #endregion

            #region Fill table
            LineType[] lTypes = new LineType[4] { LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
            LineType[] lSubTypes = new LineType[4] { LineType.level5, LineType.level6, LineType.level7, LineType.level8 };
            Double valueN = 0;
            Double valueN1 = 0;
            Int32 subTotalIndex = -1;
            string C1_ID_NAME = "ID_P", C1_IDS_KEY = string.Empty;
            string C1_LABEL_NAME = "P";
            List<DetailLevelItemInformation> levels = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
            int productLevelsCount = levels.Count;
            levels.Insert(0, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sector));

            DataRow currentProductRow;

            foreach (DataRow row in dtData.Rows)
            {
                for (int i = 0; i < productLevelsCount; i++)
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

                        currentProductRow = globalDictionary["CLASSIF1_" + (i + 1)][C1_IDS_KEY];

                        #region Fill cells

                        #region Init Total
                        //Total
                        cLine = tab.AddNewLine(lTypes[i], keys, parents[i + 1] = new CellLevel(cId, row[C1_LABEL_NAME + (i + 1)].ToString(), (CellLevel)tab[parents[i].LineIndexInResultTable + RES_YEAR_N_OFFSET, 1], i + 1, cLine, textWrap.NbChar, textWrap.Offset));
                        tab[cLine, 2] = cellHiddenFactory.Get(0.0);
                        //YearN
                        tab.AddNewLine(lSubTypes[i], keys, keyYearN, new CellLevel(ID_YEAR_N, labelN, (CellLevel)tab[cLine, 1], i + 1, cLine + RES_YEAR_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                        investN = Convert.ToDouble(currentProductRow["N"]);
                        tab[cLine + RES_YEAR_N_OFFSET, 2] = cellFactory.Get(investN);
                        //YearN1
                        if (RES_YEAR_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyYearN1, new CellLevel(ID_YEAR_N1, labelN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_YEAR_N1_OFFSET, 1], i + 1, cLine + RES_YEAR_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                            investN1 = Convert.ToDouble(currentProductRow["N1"]);
                            tab[cLine + RES_YEAR_N1_OFFSET, 2] = cellFactory.Get(investN1);
                        }
                        //Evol
                        if (RES_EVOL_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyEvol, new CellLevel(ID_EVOL, labelEvol, i + 1, cLine + RES_EVOL_OFFSET, textWrap.NbChar, textWrap.Offset));
                            tab[cLine + RES_EVOL_OFFSET, 2] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, 2], tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                            ((CellEvol)tab[cLine + RES_EVOL_OFFSET, 2]).StringFormat = "{0:percentage}";
                        }
                        //PDV N
                        if (RES_PDV_N_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdvYearN, new CellLevel(ID_PDV_N, labelPDVN, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDV_N_OFFSET, 1], i + 1, cLine + RES_PDV_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                            tab[cLine + RES_PDV_N_OFFSET, 2] = new CellPDM(investN, null);
                            ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, 2]).StringFormat = "{0:percentage}";
                        }
                        //PDV N1
                        if (RES_PDV_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdvYearN1, new CellLevel(ID_PDV_N1, labelPDVN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDV_N1_OFFSET, 1], i + 1, cLine + RES_PDV_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                            tab[cLine + RES_PDV_N1_OFFSET, 2] = new CellPDM(investN1, null);
                            ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
                        }
                        for (int k = 3; k <= tab.DataColumnsNumber; k++)
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
                            tab.AddNewLine(lSubTypes[i], keys, keyPdmYearN, new CellLevel(ID_PDM_N, labelPDMN, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDM_N_OFFSET, 1], i + 1, cLine + RES_PDM_N_OFFSET, textWrap.NbChar, textWrap.Offset));
                            tab[cLine + RES_PDM_N_OFFSET, 2] = new CellPDM(investN, null);
                            ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, 2]).StringFormat = "{0:percentage}";
                        }
                        //PDM N1
                        if (RES_PDM_N1_OFFSET > 0)
                        {
                            tab.AddNewLine(lSubTypes[i], keys, keyPdmYearN1, new CellLevel(ID_PDM_N1, labelPDMN1, (CellLevel)tab[parents[i].LineIndexInResultTable + RES_PDM_N1_OFFSET, 1], i + 1, cLine + RES_PDM_N1_OFFSET, textWrap.NbChar, textWrap.Offset));
                            tab[cLine + RES_PDM_N1_OFFSET, 2] = new CellPDM(investN1, null);
                            ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, 2]).StringFormat = "{0:percentage}";
                        }
                        #endregion

                        #region Init sub total
                        //Init sub total
                        foreach (HeaderBase h in SUB_TOTALS)
                        {
                            string keyString = string.Empty;
                            tmp = h.Key.Split('-');
                            if (_vehicle == CstDBClassif.Vehicles.names.plurimedia)
                            {
                                mediaLevelsIndex = 1;
                                if (globalDictionary["CLASSIF2_" + (i + 1) + "_" + mediaLevelsIndex].ContainsKey(C1_IDS_KEY + "_" + tmp[0]))
                                    currentSubTotalRow = globalDictionary["CLASSIF2_" + (i + 1) + "_" + mediaLevelsIndex][C1_IDS_KEY + "_" + tmp[0]];
                                else
                                    currentSubTotalRow = null;
                            }
                            else
                            {
                                mediaLevelsIndex = 2;
                                keyString = vehicleId.ToString() + "_" + tmp[0];
                                if (globalDictionary["CLASSIF2_" + (i + 1) + "_" + mediaLevelsIndex].ContainsKey(C1_IDS_KEY + "_" + keyString))
                                    currentSubTotalRow = globalDictionary["CLASSIF2_" + (i + 1) + "_" + mediaLevelsIndex][C1_IDS_KEY + "_" + keyString];
                                else
                                    currentSubTotalRow = null;
                            }

                            //YearN
                            if (currentSubTotalRow != null)
                                investN = Convert.ToDouble(currentSubTotalRow["N"]);
                            else
                                investN = 0;
                            tab[cLine + RES_YEAR_N_OFFSET, h.IndexInResultTable] = cellFactory.Get(investN);
                            //YearN1
                            if (RES_YEAR_N1_OFFSET > 0)
                            {
                                if (currentSubTotalRow != null)
                                    investN1 = Convert.ToDouble(currentSubTotalRow["N1"]);
                                else
                                    investN1 = 0;
                                tab[cLine + RES_YEAR_N1_OFFSET, h.IndexInResultTable] = cellFactory.Get(investN1);
                            }
                            //Evol
                            if (RES_EVOL_OFFSET > 0)
                            {
                                tab[cLine + RES_EVOL_OFFSET, h.IndexInResultTable] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, h.IndexInResultTable], tab[cLine + RES_YEAR_N1_OFFSET, h.IndexInResultTable]);
                                ((CellEvol)tab[cLine + RES_EVOL_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDV N
                            if (RES_PDV_N_OFFSET > 0)
                            {
                                tab[cLine + RES_PDV_N_OFFSET, h.IndexInResultTable] = new CellPDM(investN, null);
                                ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDV N1
                            if (RES_PDV_N1_OFFSET > 0)
                            {
                                tab[cLine + RES_PDV_N1_OFFSET, h.IndexInResultTable] = new CellPDM(investN1, null);
                                ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDM N
                            if (RES_PDM_N_OFFSET > 0)
                            {
                                tab[cLine + RES_PDM_N_OFFSET, h.IndexInResultTable] = new CellPDM(investN, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, 2]);
                                ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDM N1
                            if (RES_PDM_N1_OFFSET > 0)
                            {
                                tab[cLine + RES_PDM_N1_OFFSET, h.IndexInResultTable] = new CellPDM(investN1, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                                ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, h.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }

                        }
                        #endregion

                        #region Init Media
                        //Init Media
                        foreach (KeyValuePair<Int32, Header> record in RES_MEDIA_HEADERS)
                        {
                            string keyString = string.Empty;
                            tmp = record.Value.Key.Split('-');
                            if (_vehicle == CstDBClassif.Vehicles.names.plurimedia)
                            {
                                if (mediaLevels.Count == 1)
                                    keyString = tmp[0];
                                else
                                    keyString = tmp[0] + "_" + tmp[tmp.Length - 1];
                            }
                            else
                            {
                                if (mediaLevels.Count == 2)
                                {
                                    keyString = vehicleId.ToString() + "_" + tmp[0];
                                }
                                else
                                {
                                    keyString = vehicleId.ToString() + "_" + tmp[0] + "_" + tmp[tmp.Length - 1];
                                }
                            }

                            if (globalDictionary["CLASSIF2_" + (i + 1) + "_" + mediaLevels.Count].ContainsKey(C1_IDS_KEY + "_" + keyString))
                                currentMediaRow = globalDictionary["CLASSIF2_" + (i + 1) + "_" + mediaLevels.Count][C1_IDS_KEY + "_" + keyString];
                            else
                                currentMediaRow = null;

                            //YearN
                            if (currentMediaRow != null)
                                investN = Convert.ToDouble(currentMediaRow["N"]);
                            else
                                investN = 0;
                            tab[cLine + RES_YEAR_N_OFFSET, record.Value.IndexInResultTable] = cellFactory.Get(investN);
                            //YearN1
                            if (RES_YEAR_N1_OFFSET > 0)
                            {
                                if (currentMediaRow != null)
                                    investN1 = Convert.ToDouble(currentMediaRow["N1"]);
                                else
                                    investN1 = 0;
                                tab[cLine + RES_YEAR_N1_OFFSET, record.Value.IndexInResultTable] = cellFactory.Get(investN1);
                            }
                            //Evol
                            if (RES_EVOL_OFFSET > 0)
                            {
                                tab[cLine + RES_EVOL_OFFSET, record.Value.IndexInResultTable] = new CellEvol(tab[cLine + RES_YEAR_N_OFFSET, record.Value.IndexInResultTable], tab[cLine + RES_YEAR_N1_OFFSET, record.Value.IndexInResultTable]);
                                ((CellEvol)tab[cLine + RES_EVOL_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDV N
                            if (RES_PDV_N_OFFSET > 0)
                            {
                                tab[cLine + RES_PDV_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN, null);
                                ((CellPDM)tab[cLine + RES_PDV_N_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDV N1
                            if (RES_PDV_N1_OFFSET > 0)
                            {
                                tab[cLine + RES_PDV_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN1, null);
                                ((CellPDM)tab[cLine + RES_PDV_N1_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDM N
                            if (RES_PDM_N_OFFSET > 0)
                            {
                                if (RES_MEDIA_SUBTOTAL.ContainsKey(record.Key))
                                {
                                    tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, RES_MEDIA_SUBTOTAL[record.Key].IndexInResultTable]);
                                }
                                else
                                {
                                    tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN, (CellUnit)tab[cLine + RES_YEAR_N_OFFSET, 2]);
                                }
                                ((CellPDM)tab[cLine + RES_PDM_N_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }
                            //PDM N1
                            if (RES_PDM_N1_OFFSET > 0)
                            {
                                if (RES_MEDIA_SUBTOTAL.ContainsKey(record.Key))
                                {
                                    tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN1, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, RES_MEDIA_SUBTOTAL[record.Key].IndexInResultTable]);
                                }
                                else
                                {
                                    tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable] = new CellPDM(investN1, (CellUnit)tab[cLine + RES_YEAR_N1_OFFSET, 2]);
                                }
                                ((CellPDM)tab[cLine + RES_PDM_N1_OFFSET, record.Value.IndexInResultTable]).StringFormat = "{0:percentage}";
                            }

                        }
                        #endregion

                        #endregion

                    }
                }

                #region Advertisers univers
                int classifIndex = levels.Count - 1;
                DataRow currentPersonalizedRow;
                if (_isPersonalized > 0)
                {
                    classifIndex = levels.Count - 1;
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
                            currentPersonalizedRow = globalDictionary["CLASSIF1_" + classifIndex][C1_IDS_KEY];
                            classifIndex--;
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
                }
                #endregion

            }
            #endregion

			return tab;
        }
        #endregion

    }

}
