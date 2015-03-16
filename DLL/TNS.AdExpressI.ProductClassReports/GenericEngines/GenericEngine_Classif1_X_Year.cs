using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstPersonalized = TNS.AdExpress.Constantes.Web.AdvertiserPersonalisation.Type;
using FctUtilities = TNS.AdExpress.Web.Functions;

using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;
//using FctWeb = TNS.AdExpress.Web.

namespace TNS.AdExpressI.ProductClassReports.GenericEngines
{
    /// <summary>
    /// Implement an engine to build a report presented as Classif1 X Year
    /// </summary>
    public class GenericEngine_Classif1_X_Year : GenericEngine
    {

        #region Constant
        protected const Int32 ID_LEVEL = -1;
        protected const Int32 ID_N = -2;
        protected const Int32 ID_PDMV_N = -3;
        protected const Int32 ID_N1 = -4;
        protected const Int32 ID_PDMV_N1 = -5;
        protected const Int32 ID_EVOL = -6;
        #endregion

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
        /// Build Table Classif1 X Year
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override ResultTable ComputeData(DataSet data)
        {

            ProductClassResultTable tab = null;

            #region Data
            DataTable dtData = data.Tables[0];
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

            #region first data column
            int FIRST_DATA_INDEX = 0;
            List<Int32> DATA_CLASSIF_INDEXES = new List<Int32>();
            for (int i = 0; i < dtData.Columns.Count; i = i + 2)
            {
                if (dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0 || dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0)
                {
                    DATA_CLASSIF_INDEXES.Add(i);
                }
                else
                {
                    FIRST_DATA_INDEX = i;
                    break;
                }
            }
            #endregion

            // delete useless lines
            this.CleanDataTable(dtData, FIRST_DATA_INDEX);

            #region result columns
            Int32 RES_N_INDEX = 2;
			Int32 RES_PDMV_N_INDEX = ((_session.PDM && _tableType == CstFormat.PreformatedTables.media_X_Year.GetHashCode())|| (_session.PDV && _tableType == CstFormat.PreformatedTables.product_X_Year.GetHashCode())) ? RES_N_INDEX + 1 : -1;
            Int32 RES_N1_INDEX = (_session.ComparativeStudy)? Math.Max(RES_N_INDEX, RES_PDMV_N_INDEX) + 1 : -1;
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
            headers.Root.Add(new Header(true, FctUtilities.Dates.getPeriodLabel(_session, CstWeb.CustomerSessions.Period.Type.currentYear), ID_N));
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
                headers.Root.Add(new Header(true, FctUtilities.Dates.getPeriodLabel(_session, CstWeb.CustomerSessions.Period.Type.previousYear), ID_N1));
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

            #region Total
            List<CellLevel> levels = new List<CellLevel>();
            List<LineType> RES_LINE_TYPES = new List<LineType>();
            List<Int64> keys = new List<Int64>();
            Int32 cLine = -1;
            CellUnitFactory cellFactory = _session.GetCellUnitFactory(); 
            CellLevel cellTotal = null;
            if (_vehicle != CstDBClassif.Vehicles.names.plurimedia && _vehicle != CstDBClassif.Vehicles.names.PlurimediaWithoutMms 
                && _tableType != CstFormat.PreformatedTables.product_X_Year.GetHashCode())
            {
                RES_LINE_TYPES.Add(LineType.total);
            }
            else
            {
                keys.Add(-1);
                cellTotal = new CellLevel(-1, "Total", null, 0, 0);
                cLine = tab.AddNewLine(LineType.total, keys, cellTotal);
                tab[cLine, RES_N_INDEX] = cellFactory.Get(0.0);
                if (RES_PDMV_N_INDEX > -1)
                {
                    tab[cLine, RES_PDMV_N_INDEX] = new CellPDM(0.0, null);
                    ((CellUnit)tab[cLine, RES_PDMV_N_INDEX]).StringFormat = "{0:percentage}";

                }
                if (RES_N1_INDEX > -1)
                {
                    tab[cLine, RES_N1_INDEX] = cellFactory.Get(0.0);
                }
                if (RES_PDMV_N1_INDEX > -1)
                {
                    tab[cLine, RES_PDMV_N1_INDEX] = new CellPDM(0.0, null);
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
            Double dYearN = 0.0;
            Double dYearN1 = 0.0;
            List<DetailLevelItemInformation> details = null;
            if(_tableType != CstFormat.PreformatedTables.product_X_Year.GetHashCode()){
                details = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
            }
            else{
                details = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
            }
            foreach (DataRow row in dtData.Rows)
            {
                //Init level
                for (int i = 0; i < DATA_CLASSIF_INDEXES.Count; i++)
                {
                    cId = Convert.ToInt64(row[DATA_CLASSIF_INDEXES[i]]);
                    if (levels.Count <= i || levels[i].Id != cId)
                    {
                        for (int j = levels.Count-1; j >= i && j >= 0; j--)
                        {
                            levels.RemoveAt(levels.Count-1);
                            keys.RemoveAt(keys.Count-1);
                        }
                        keys.Add(cId);
                        if (i > 0){
                            levels.Add(new CellLevel(cId, row[DATA_CLASSIF_INDEXES[i]+1].ToString(), levels[i-1], i, cLine));
                        }
                        else
                        {
                            levels.Add(new CellLevel(cId, row[DATA_CLASSIF_INDEXES[i]+1].ToString(), cellTotal, i, cLine));
                        }
                        cLine = tab.AddNewLine(RES_LINE_TYPES[i], keys, levels[i]);
                        tab[cLine, RES_N_INDEX] = cellFactory.Get(0.0);
                        if (RES_PDMV_N_INDEX > -1)
                        {
                            tab[cLine, RES_PDMV_N_INDEX] = new CellPDM(0.0, ((i > 0) ? (CellUnit)tab[levels[i - 1].LineIndexInResultTable, RES_N_INDEX] : ((cellTotal != null) ? (CellUnit)tab[cellTotal.LineIndexInResultTable, RES_N_INDEX] : null)));
                            ((CellUnit)tab[cLine, RES_PDMV_N_INDEX]).StringFormat = "{0:percentage}";
                        }
                        if (RES_N1_INDEX > -1)
                        {
                            tab[cLine, RES_N1_INDEX] = cellFactory.Get(0.0);
                        }
                        if (RES_PDMV_N1_INDEX > -1)
                        {
                            tab[cLine, RES_PDMV_N1_INDEX] = new CellPDM(0.0, ((i > 0) ? (CellUnit)tab[levels[i - 1].LineIndexInResultTable, RES_N1_INDEX] : ((cellTotal != null) ? (CellUnit)tab[cellTotal.LineIndexInResultTable, RES_N1_INDEX] : null)));
                            ((CellUnit)tab[cLine, RES_PDMV_N1_INDEX]).StringFormat = "{0:percentage}";
                        }
                        if (RES_EVOL_INDEX > -1)
                        {
                            tab[cLine, RES_EVOL_INDEX] = new CellEvol(tab[cLine, RES_N_INDEX], tab[cLine, RES_N1_INDEX]);
                            ((CellUnit)tab[cLine, RES_EVOL_INDEX]).StringFormat = "{0:percentage}";
                        }
                    }
                }

                //Add Value
                dYearN = Convert.ToDouble(row[FIRST_DATA_INDEX]);
                tab.AffectValueAndAddToHierarchy(1, cLine, RES_N_INDEX, dYearN);
                if (RES_PDMV_N_INDEX > -1)
                {
                    tab.AffectValueAndAddToHierarchy(1, cLine, RES_PDMV_N_INDEX, dYearN);
                }
                if (RES_N1_INDEX > -1)
                {
                    dYearN1 = Convert.ToDouble(row[FIRST_DATA_INDEX+1]);
                    tab.AffectValueAndAddToHierarchy(1, cLine, RES_N1_INDEX, dYearN1);
                    if (RES_PDMV_N1_INDEX > -1)
                    {
                        tab.AffectValueAndAddToHierarchy(1, cLine, RES_PDMV_N1_INDEX, dYearN1);
                    }
                }

                #region Advertisers univers
                if (_isPersonalized > 0)
                {
                    for (int i = levels.Count - 1; i >= 0; i--)
                    {
                        SetPersoAdvertiser(tab, levels[i].LineIndexInResultTable, row, details[i].Id);
                    }
                    if (cellTotal != null)
                    {
                        SetPersoAdvertiser(tab, cellTotal.LineIndexInResultTable, row, DetailLevelItemInformation.Levels.vehicle);
                    }
                }
                #endregion


            }
            #endregion

            #region Hide lines if required
            if (_session.PersonalizedElementsOnly && _isPersonalized > 0)
            {
                for (int i = 0; i < tab.LinesNumber; i++)
                {
                    ProductClassLineStart ls = (ProductClassLineStart)tab[i, 0];
                    if (ls.LineUnivers == UniversType.neutral && ChildrenAreNeutral(tab,i))
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
