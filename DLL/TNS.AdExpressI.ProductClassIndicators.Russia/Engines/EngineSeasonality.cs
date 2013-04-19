#region Info
/*
 * Author :     G Ragneau
 * Created on : 04/08/2008
 * History:
 *      Date - Author - Description
 *      04/08/2008 - G Ragneau - Moved from TNS.AdExpress.Web
 * 
 * 
 * */
#endregion

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using DBConstantes = TNS.AdExpress.Constantes.DB;


using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using System.Collections;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes;

namespace TNS.AdExpressI.ProductClassIndicators.Russia.Engines
{
    /// <summary>
    /// Engine to build a Top report or to provide computed data for top report or indicator
    /// </summary>
    public class EngineSeasonality:TNS.AdExpressI.ProductClassIndicators.Engines.EngineSeasonality
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineSeasonality(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { 
        }
        #endregion

        #region GetResult
        /// <summary>
        /// Get Seasonality indicator as a table
        /// </summary>
        /// <returns>StringBuilder with HTML code</returns>
        public override StringBuilder GetResult()
        {
            throw new NotImplementedException("This methods is not implemented");
        }
        #endregion

        #region GetResultTable
        /// <summary>
        /// Get data to build  seasonality report as a table
        /// Give : 
        ///     -Investments on N
        ///     -Evol n vs N-1 (only if comparative study)
        ///     -Reference number
        ///     -Average of investment per reference
        ///     -First advertiser in k€ and SOV (only for eventual advertiser lines)
        ///     -First reference in k€ and SOV (only for eventual product lines)
        /// For univers, market or sector and on potential references and competitors advertisers
        /// </summary>
        /// <returns>Table with a mensual compareason N vs N-1</returns>
        /// <returns></returns>
        public override ResultTable GetResultTable() {

            #region variables
            ResultTable resultTable = null;
            bool showProduct = false;
            int currentMonth = 0;
            int nbMonths = 0;
            int currentTotalType = 0;

            string totalTypeLabel = string.Empty;
            object invest;
            object evol;
            object refNb;
            object avgInvest;
            object advLabel;
            object advInvest;
            object advSOV;
            object refLabel = null;
            object refInvest = null;
            object refSOV = null;
            DataRow[] currentDataRowList = null;
            LineType currentLineType = LineType.total;
            HeaderLine currentHeaderLine = null;
            CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            #endregion

            #region GetData
            object[] tabResult = this.GetTableData();
            DataTable dtTotals = null;
            DataTable dtAdvertiser = null;
            #endregion

            #region No data
            if (tabResult == null || tabResult.GetLength(0) == 0 || !(tabResult[0] is DataTable) 
                || (dtTotals = (DataTable)tabResult[0]) == null || dtTotals.Rows.Count == 0) {
                return null;
            }
            if (tabResult.GetLength(0) == 2 && (tabResult[1] is DataTable)) {
                dtAdvertiser = (DataTable)tabResult[1];
            }
            #endregion

            #region Get Right Product
            showProduct = _session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            #endregion

            #region Headers
            int indexColheader = 1;
            Headers headers = new Headers();

            HeaderColumnLineHeader currentHeaderColumnLineHeader = new HeaderColumnLineHeader(string.Empty);
            headers.Root.Add(currentHeaderColumnLineHeader);

            Header currentHeader = new Header(string.Empty, indexColheader);
            headers.Root.Add(currentHeader);

            indexColheader++;
            currentHeader = new Header(GestionWeb.GetWebWord(UnitsInformation.Get(_session.Unit).WebTextId, _session.SiteLanguage) +" ("+GestionWeb.GetWebWord(2782, _session.SiteLanguage) + ") " + _periodBegin.Year, indexColheader);
            headers.Root.Add(currentHeader);


            //Evol (optionnelle)
            if (_session.ComparativeStudy) {
                indexColheader++;
                currentHeader = new Header(GestionWeb.GetWebWord(1168, _session.SiteLanguage) + " " + _periodBegin.Year + "/" + _periodBegin.AddYears(-1).Year, indexColheader);
                headers.Root.Add(currentHeader);
            }

            indexColheader++;
            currentHeader = new Header(GestionWeb.GetWebWord(1152, _session.SiteLanguage), indexColheader);
            headers.Root.Add(currentHeader);

            indexColheader++;
            currentHeader = new Header(GestionWeb.GetWebWord(1153, _session.SiteLanguage), indexColheader);
            headers.Root.Add(currentHeader);

            //First advertiser (optionnels)
            indexColheader++;
            currentHeader = new Header(GestionWeb.GetWebWord(1154, _session.SiteLanguage), true, indexColheader);
            headers.Root.Add(currentHeader);

            indexColheader++;
            currentHeader = new Header(string.Empty, indexColheader);
            headers.Root.Add(currentHeader);

            indexColheader++;
            currentHeader = new Header(GestionWeb.GetWebWord(437, _session.SiteLanguage), indexColheader);
            headers.Root.Add(currentHeader);

            if (showProduct) {
                //Separator
                if (!_excel) {
                    //str.Append("<td class=\"violetBackGround columnSeparator\"><img width=1px></td>");
                }
                //1er références (optionnels)	
                indexColheader++;
                currentHeader = new Header(GestionWeb.GetWebWord(1155, _session.SiteLanguage), true, indexColheader);
                headers.Root.Add(currentHeader);

                indexColheader++;
                currentHeader = new Header(string.Empty, indexColheader);
                headers.Root.Add(currentHeader);

                indexColheader++;
                currentHeader = new Header(GestionWeb.GetWebWord(437, _session.SiteLanguage), indexColheader);
                headers.Root.Add(currentHeader);
            }
            #endregion

            if(dtAdvertiser!=null)
                resultTable = new ResultTable(dtTotals.Rows.Count + dtAdvertiser.Rows.Count, headers); 
            else
                resultTable = new ResultTable(dtTotals.Rows.Count, headers); 

            #region Get monthes
            nbMonths = _periodEnd.Month - _periodBegin.Month + 1;
            currentMonth = _periodBegin.Month;
            #endregion

            #region Monthes
            for (int m = 1; m <= nbMonths; m++, currentMonth++) {

                currentHeaderLine = new HeaderLine(MonthString.GetCharacters(currentMonth, cInfo, 0));
                currentLineType = LineType.total;

                currentDataRowList = dtTotals.Select("MONTH=" + currentMonth);
                foreach (DataRow currentRow in currentDataRowList) {

                    currentTotalType = Int32.Parse(currentRow["TOTAL_TYPE"].ToString());

                    #region Get Information for the current Level type
                    switch (currentTotalType) {
                        case 0:

                            #region Total market
                            totalTypeLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                            #endregion

                            break;
                        case 1:

                            #region Total Category
                            totalTypeLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                            #endregion

                            break;
                        case 2:

                            #region Total Univers
                            totalTypeLabel = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                            #endregion

                            break;
                        default:
                            throw new ProductClassIndicatorsException("Current Total Type is not defined");
                    }
                    #endregion

                    #region Get Data Value
                    invest = currentRow["UNIT_N"];
                    if (_session.ComparativeStudy) evol = currentRow["CHANGE"];
                    else evol = null;
                    refNb = currentRow["NUMBER_PRODUCT"];
                    avgInvest = currentRow["AVERAGE_BUDGET"];
                    advLabel = currentRow["TOP_ADVERTISER"];
                    advInvest = currentRow["TOP_ADV_UNIT"];
                    advSOV = currentRow["ADV_SOV"];
                    if (showProduct) {
                        refLabel = currentRow["TOP_PRODUCT"];
                        refInvest = currentRow["TOP_PROD_UNIT"];
                        refSOV = currentRow["PROD_SOV"];
                    }
                    else {
                        refLabel = null;
                        refInvest = null;
                        refSOV = null;
                    }
                    #endregion

                    AppendLineResultTable(currentLineType, resultTable, currentHeaderLine, totalTypeLabel, invest, evol, refNb, avgInvest, advLabel, advInvest, advSOV, refLabel, refInvest, refSOV);

                }
                
                #region Advertisers
                if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0) {
                    currentDataRowList = dtAdvertiser.Select("MONTH=" + currentMonth);

                    foreach (DataRow currentRow in currentDataRowList) {

                        #region Reference or competitor?
                        if (currentRow["ID_ADVERTISER"] != null) {
                            Int64 idAdv = Int64.Parse(currentRow["ID_ADVERTISER"].ToString());
                            if (_referenceIDS.Contains(idAdv)) {
                                currentLineType = LineType.level1;
                            }
                            else if (_competitorIDS.Contains(idAdv)) {
                                currentLineType = LineType.level2;
                            }

                            invest = currentRow["UNIT_N"];
                            if (_session.ComparativeStudy) evol = currentRow["CHANGE"];
                            else evol = null;
                            refNb = currentRow["NUMBER_PRODUCT"];
                            avgInvest = currentRow["AVERAGE_BUDGET"];
                            advLabel = null;
                            advInvest = null;
                            advSOV = null;
                            refLabel = null;
                            refInvest = null;
                            refSOV = null;

                            AppendLineResultTable(currentLineType, resultTable, currentHeaderLine, currentRow["ADVERTISER"].ToString(), invest, evol, refNb, avgInvest, advLabel, advInvest, advSOV, refLabel, refInvest, refSOV);

                        }
                        #endregion

                    }
                }
                #endregion

            }
            #endregion


            return resultTable;

        }
        #endregion

        #region GetTableData
        /// <summary>
        /// Get data to build  seasonality report as a table
        /// Give : 
        ///     -Investments on N
        ///     -Evol n vs N-1 (only if comparative study)
        ///     -Reference number
        ///     -Average of investment per reference
        ///     -First advertiser in k€ and SOV (only for eventual advertiser lines)
        ///     -First reference in k€ and SOV (only for eventual product lines)
        /// For univers, market or sector and on potential references and competitors advertisers
        /// </summary>
        /// <returns>Table with a mensual compareason N vs N-1</returns>
        /// <returns></returns>
        public override object[] GetTableData()
        {

            object[] tabResult = null;

            #region Get Data
            DataSet ds = _dalLayer.GetSeasonalityTblData(true, false);
            #endregion

            if (ds != null && ds.Tables.Count>0 && ds.Tables[0] != null) {
                tabResult = new object[ds.Tables.Count];
                for (int i = 0; i < ds.Tables.Count; i++) {
                    tabResult[i] = ds.Tables[i];
                }
            }
            else tabResult = null;

            return tabResult;

        }
        #endregion

        #region GetChartData
        /// <summary>
        /// Get investments of year N and market/sector/univers totals
        /// </summary>
        /// <remarks>Used to build seasonality graph.</remarks>
        /// <returns>Month by month investments</returns>
        public override object[,] GetChartData()
        {
            #region variables
            object[,] tabResult = null;
            object[] tabData = null;
            int currentMonth = 0;
            int nbMonths = 0;
            int currentTotalType = 0;

            string totalTypeLabel = string.Empty;
            Int64 totalTypeId = 0;
            double invest;
            string advLabel;
            DataRow[] currentDataRowList = null;
            #endregion

            #region Get Data

            #region Get Data
            DataSet ds = _dalLayer.GetSeasonalityGraphData(true, false);
            #endregion

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null) {
                tabData = new object[ds.Tables.Count];
                for (int i = 0; i < ds.Tables.Count; i++) {
                    tabData[i] = ds.Tables[i];
                }
            }
            else tabData = null;
            DataTable dtTotals = null;
            DataTable dtAdvertiser = null;
            #endregion

            #region No data
            if (tabData == null || tabData.GetLength(0) == 0 || !(tabData[0] is DataTable)
                || (dtTotals = (DataTable)tabData[0]) == null || dtTotals.Rows.Count == 0) {
                return null;
            }
            if (tabData.GetLength(0) == 2 && (tabData[1] is DataTable)) {
                dtAdvertiser = (DataTable)tabData[1];
            }
            #endregion

            if (dtAdvertiser != null)
                tabResult = new object[dtTotals.Rows.Count + dtAdvertiser.Rows.Count, 4];
            else
                tabResult = new object[dtTotals.Rows.Count, 4]; 

            #region Get monthes
            nbMonths = _periodEnd.Month - _periodBegin.Month + 1;
            currentMonth = _periodBegin.Month;
            #endregion

            #region Build Result
            if (ds != null && ds.Tables[0] != null) {

                #region Monthes
                for (int m = 1, currentLine = 0; m <= nbMonths; m++, currentMonth++) {

                    currentDataRowList = dtTotals.Select("MONTH=" + currentMonth);
                    foreach (DataRow currentRow in currentDataRowList) {

                        currentTotalType = Int32.Parse(currentRow["TOTAL_TYPE"].ToString());

                        #region Get Information for the current Level type
                        switch (currentTotalType) {
                            case 0:

                                #region Total market
                                totalTypeLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                                totalTypeId = ID_TOTAL_MARKET_COLUMN_INDEX;
                                #endregion

                                break;
                            case 1:

                                #region Total Category
                                totalTypeLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                                totalTypeId = ID_TOTAL_SECTOR_COLUMN_INDEX;
                                #endregion

                                break;
                            case 2:

                                #region Total Univers
                                totalTypeLabel = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                                totalTypeId = ID_TOTAL_UNIVERSE_COLUMN_INDEX;
                                #endregion

                                break;
                            default:
                                throw new ProductClassIndicatorsException("Current Total Type is not defined");
                        }
                        #endregion

                        #region Get Data Value
                        if (currentRow["UNIT_N"] != null && currentRow["UNIT_N"] != System.DBNull.Value) invest = Convert.ToDouble(currentRow["UNIT_N"], WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo);
                        else invest = 0;
                        #endregion

                        tabResult[currentLine, ID_MONTH_COLUMN_INDEX] = currentMonth;
                        tabResult[currentLine, ID_ELEMENT_COLUMN_INDEX] = totalTypeId;
                        tabResult[currentLine, LABEL_ELEMENT_COLUMN_INDEX] = totalTypeLabel;
                        tabResult[currentLine, INVEST_COLUMN_INDEX] = invest;
                        currentLine++;
                    }

                    #region Advertisers
                    if (dtAdvertiser != null && dtAdvertiser.Rows.Count > 0) {
                        currentDataRowList = dtAdvertiser.Select("MONTH=" + currentMonth);

                        foreach (DataRow currentRow in currentDataRowList) {

                            #region Reference or competitor?
                            if (currentRow["ID_ADVERTISER"] != null) {
                                Int64 idAdv = Int64.Parse(currentRow["ID_ADVERTISER"].ToString());
                                if (currentRow["UNIT_N"] != null && currentRow["UNIT_N"] != System.DBNull.Value) invest = Convert.ToDouble(currentRow["UNIT_N"], WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo);
                                else invest = 0;
                                if (currentRow["ADVERTISER"] != null) advLabel = currentRow["ADVERTISER"].ToString();
                                else advLabel = string.Empty;

                                tabResult[currentLine, ID_MONTH_COLUMN_INDEX] = currentMonth;
                                tabResult[currentLine, ID_ELEMENT_COLUMN_INDEX] = idAdv;
                                tabResult[currentLine, LABEL_ELEMENT_COLUMN_INDEX] = advLabel;
                                tabResult[currentLine, INVEST_COLUMN_INDEX] = invest;
                                currentLine++;

                            }
                            #endregion

                        }
                    }
                    #endregion

                }
                #endregion

            }
            #endregion

            return tabResult;
        }
        #endregion

        #region Method
        public override CellLabel GetCellLabel(String label)
        {
            return new CellLabel(label, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap.NbChar, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap.Offset);
        }
        #endregion
	}
}