using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstPersonalized = TNS.AdExpress.Constantes.Web.AdvertiserPersonalisation.Type;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.ProductClassReports.GenericEngines;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Constantes;
//using FctWeb = TNS.AdExpress.Web.

namespace TNS.AdExpressI.ProductClassReports.Russia.GenericEngines
{
    /// <summary>
    /// Implement an engine to build a report presented as Classif1 X Year
    /// </summary>
    public class GenericEngine_Classif1_X_Year : TNS.AdExpressI.ProductClassReports.GenericEngines.GenericEngine_Classif1_X_Year
    {

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public GenericEngine_Classif1_X_Year(WebSession session, int result) : base(session, result) { }
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

            #region Data
            if (dsData == null || dsData.Tables.Count == 0) return null;
            DataTable dtData = dsData.Tables[dsData.Tables.Count - 1];
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

            #region result columns
            Int32 RES_N_INDEX = 2;
            Int32 RES_PDMV_N_INDEX = ((_session.PDM && _tableType == CstFormat.PreformatedTables.media_X_Year.GetHashCode()) || (_session.PDV && _tableType == CstFormat.PreformatedTables.product_X_Year.GetHashCode())) ? RES_N_INDEX + 1 : -1;
            Int32 RES_N1_INDEX = (_session.ComparativeStudy) ? Math.Max(RES_N_INDEX, RES_PDMV_N_INDEX) + 1 : -1;
            Int32 RES_PDMV_N1_INDEX = (((_session.PDM && _tableType == CstFormat.PreformatedTables.media_X_Year.GetHashCode()) || (_session.PDV && _tableType == CstFormat.PreformatedTables.product_X_Year.GetHashCode())) && _session.ComparativeStudy) ? RES_N1_INDEX + 1 : -1;
            Int32 RES_EVOL_INDEX = (_session.Evolution && _session.ComparativeStudy) ? Math.Max(RES_N1_INDEX, RES_PDMV_N1_INDEX) + 1 : -1;
            #endregion

            #endregion

            #region Init headers
            Headers headers = new Headers();
            if (_tableType != CstFormat.PreformatedTables.media_X_Year.GetHashCode())
            {
                headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, _session.SiteLanguage), ID_LEVEL));
            }
            else
            {
                headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1357, _session.SiteLanguage), ID_LEVEL));
            }
            headers.Root.Add(new Header(true, TNS.AdExpress.Web.Functions.Dates.getPeriodLabel(_session, CstWeb.CustomerSessions.Period.Type.currentYear), ID_N));
            //PDM
            if (_session.PDM && _tableType == CstFormat.PreformatedTables.media_X_Year.GetHashCode())
            {
                headers.Root.Add(new Header(true, string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), _session.PeriodBeginningDate.Substring(0, 4)), ID_PDMV_N));
            }
            //PDV
            if (_session.PDV && _tableType == CstFormat.PreformatedTables.product_X_Year.GetHashCode())
            {
                headers.Root.Add(new Header(true, string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), _session.PeriodBeginningDate.Substring(0, 4)), ID_PDMV_N));
            }
            if (_session.ComparativeStudy)
            {
                headers.Root.Add(new Header(true, TNS.AdExpress.Web.Functions.Dates.getPeriodLabel(_session, CstWeb.CustomerSessions.Period.Type.previousYear), ID_N1));
                //PDM
                if (_session.PDM && _tableType == CstFormat.PreformatedTables.media_X_Year.GetHashCode())
                {
                    headers.Root.Add(new Header(true, string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4)) - 1), ID_PDMV_N1));
                }
                //PDV
                if (_session.PDV && _tableType == CstFormat.PreformatedTables.product_X_Year.GetHashCode())
                {
                    headers.Root.Add(new Header(true, string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4)) - 1), ID_PDMV_N1));
                }
                if (RES_EVOL_INDEX > -1)
                {
                    headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1168, _session.SiteLanguage), ID_EVOL));
                }
            }
            #endregion

            tab = new ProductClassResultTable(Convert.ToInt32(dtData.Rows.Count * 1.4), headers);

            #region Levels
            string C1_ID_NAME = string.Empty, C1_IDS_KEY = string.Empty;
            string C1_LABEL_NAME = string.Empty;
            List<DetailLevelItemInformation> details = null;

            if (_tableType != CstFormat.PreformatedTables.product_X_Year.GetHashCode())
            {
                details = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
                C1_ID_NAME = "ID_M";
                C1_LABEL_NAME = "M";
            }
            else
            {
                details = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
                C1_ID_NAME = "ID_P";
                C1_LABEL_NAME = "P";
            }
            #endregion

            Dictionary<string, Dictionary<string, DataRow>> globalDictionary = CommonFunctions.InitDictionariesData(dsData, details.Count, _tableType);

            #region Total
            List<CellLevel> levels = new List<CellLevel>();
            List<LineType> RES_LINE_TYPES = new List<LineType>();
            List<Int64> keys = new List<Int64>();
            Int32 cLine = -1;
            CellUnitFactory cellFactory = _session.GetCellUnitFactory();
            CellLevel cellTotal = null;
            DataRow currentRow = null;
            Double invest = 0;

            if (_vehicle != CstDBClassif.Vehicles.names.plurimedia && _tableType != CstFormat.PreformatedTables.product_X_Year.GetHashCode())
            {
                RES_LINE_TYPES.Add(LineType.total);
            }
            else
            {
                keys.Add(-1);
                currentRow = globalDictionary["TOTAL"]["TOTAL"];
                cellTotal = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), null, 0, 0, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap.NbChar, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap.Offset);
                cLine = tab.AddNewLine(LineType.total, keys, cellTotal);
                invest = Convert.ToDouble(currentRow["N"]);
                tab[cLine, RES_N_INDEX] = cellFactory.Get(invest);
                if (RES_PDMV_N_INDEX > -1)
                {
                    tab[cLine, RES_PDMV_N_INDEX] = new CellPDM(invest, null);
                    ((CellUnit)tab[cLine, RES_PDMV_N_INDEX]).StringFormat = "{0:percentage}";

                }
                invest = Convert.ToDouble(currentRow["N1"]);
                if (RES_N1_INDEX > -1)
                {
                    tab[cLine, RES_N1_INDEX] = cellFactory.Get(invest);
                }
                if (RES_PDMV_N1_INDEX > -1)
                {
                    tab[cLine, RES_PDMV_N1_INDEX] = new CellPDM(invest, null);
                    ((CellUnit)tab[cLine, RES_PDMV_N1_INDEX]).StringFormat = "{0:percentage}";
                }
                if (RES_EVOL_INDEX > -1)
                {
                    tab[cLine, RES_EVOL_INDEX] = new CellEvol(tab[cLine, RES_N_INDEX], tab[cLine, RES_N1_INDEX]);
                    ((CellUnit)tab[cLine, RES_EVOL_INDEX]).StringFormat = "{0:percentage}";
                }
            }
            RES_LINE_TYPES.Add(LineType.level1);
            RES_LINE_TYPES.Add(LineType.level2);
            RES_LINE_TYPES.Add(LineType.level3);
            #endregion

            #region Fill Table
            Int64 cId = -1;
 
            foreach (DataRow row in dtData.Rows)
            {
                //Init level
                for (int i = 0; i < details.Count; i++)
                {

                    cId = Convert.ToInt64(row[C1_ID_NAME + (i + 1)]);

                    if (levels.Count <= i || levels[i].Id != cId)
                    {
                        for (int j = levels.Count - 1; j >= i && j >= 0; j--)
                        {
                            levels.RemoveAt(levels.Count - 1);
                            keys.RemoveAt(keys.Count - 1);
                        }
                        keys.Add(cId);

                        C1_IDS_KEY = string.Empty;
                        for (int j = 0; j < keys.Count; j++)
                            if (keys[j] != -1)
                                C1_IDS_KEY += keys[j] + "_";
                        if (C1_IDS_KEY.Length > 0)
                            C1_IDS_KEY = C1_IDS_KEY.Substring(0, C1_IDS_KEY.Length - 1);

                       currentRow = globalDictionary["CLASSIF1_" + (i + 1)][C1_IDS_KEY];

                        if (i > 0)
                        {
                            levels.Add(new CellLevel(cId, currentRow[C1_LABEL_NAME + (i + 1)].ToString(), levels[i - 1], i, cLine, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap.NbChar, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap.Offset));
                        }
                        else
                        {
                            levels.Add(new CellLevel(cId, currentRow[C1_LABEL_NAME + (i + 1)].ToString(), cellTotal, i, cLine, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap.NbChar, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap.Offset));
                        }

                        cLine = tab.AddNewLine(RES_LINE_TYPES[i], keys, levels[i]);
                        invest = Convert.ToDouble(currentRow["N"]);
                        tab[cLine, RES_N_INDEX] = cellFactory.Get(invest);
                        if (RES_PDMV_N_INDEX > -1)
                        {
                            tab[cLine, RES_PDMV_N_INDEX] = new CellPDM(invest, ((i > 0) ? (CellUnit)tab[levels[i - 1].LineIndexInResultTable, RES_N_INDEX] : ((cellTotal != null) ? (CellUnit)tab[cellTotal.LineIndexInResultTable, RES_N_INDEX] : null)));
                            ((CellUnit)tab[cLine, RES_PDMV_N_INDEX]).StringFormat = "{0:percentage}";
                        }
                        invest = Convert.ToDouble(currentRow["N1"]);
                        if (RES_N1_INDEX > -1)
                        {
                            tab[cLine, RES_N1_INDEX] = cellFactory.Get(invest);
                        }
                        if (RES_PDMV_N1_INDEX > -1)
                        {
                            tab[cLine, RES_PDMV_N1_INDEX] = new CellPDM(invest, ((i > 0) ? (CellUnit)tab[levels[i - 1].LineIndexInResultTable, RES_N1_INDEX] : ((cellTotal != null) ? (CellUnit)tab[cellTotal.LineIndexInResultTable, RES_N1_INDEX] : null)));
                            ((CellUnit)tab[cLine, RES_PDMV_N1_INDEX]).StringFormat = "{0:percentage}";
                        }
                        if (RES_EVOL_INDEX > -1)
                        {
                            tab[cLine, RES_EVOL_INDEX] = new CellEvol(tab[cLine, RES_N_INDEX], tab[cLine, RES_N1_INDEX]);
                            ((CellUnit)tab[cLine, RES_EVOL_INDEX]).StringFormat = "{0:percentage}";
                        }
                    }
                }

                #region Advertisers univers
                if (_isPersonalized > 0)
                {
                    for (int i = levels.Count - 1; i >= 0; i--)
                    {
                        SetPersoAdvertiser(tab, levels[i].LineIndexInResultTable, currentRow, details[i].Id);
                    }
                    if (cellTotal != null)
                    {
                        SetPersoAdvertiser(tab, cellTotal.LineIndexInResultTable, currentRow, DetailLevelItemInformation.Levels.vehicle);
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
