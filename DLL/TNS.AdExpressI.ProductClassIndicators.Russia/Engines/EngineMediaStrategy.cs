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
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstMediaStrategy = TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy;
using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;
using CstDbClassif = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstInvestmentType = TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.InvestmentType;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using System.Collections;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Constantes;

namespace TNS.AdExpressI.ProductClassIndicators.Russia.Engines
{
    /// <summary>
    /// Engine to build a Top report or to provide computed data for top report or indicator
    /// </summary>
    public class EngineMediaStrategy : TNS.AdExpressI.ProductClassIndicators.Engines.EngineMediaStrategy
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineMediaStrategy(WebSession session, IProductClassIndicatorsDAL dalLayer)
            : base(session, dalLayer)
        {
        }
        #endregion

        #region GetResult
        /// <summary>
        /// Crée le code HTML pour afficher le tableau de la répartition média sur le total de la période 
        /// contenant les éléments ci-après :
        /// en ligne :
        /// -le total famille (en option uniquement en sélection de groupe/variétés) ou le
        /// total marché (en option)
        /// -les éléments de références
        /// -les éléments concurrents 
        /// en colonne :
        /// -Les investissements de la période N
        /// -une PDM (part de marché ) exprimant la répartition media en %
        /// -une évolution N vs N-1 en % (uniquement dans le cas d'une étude comparative)
        /// -le 1er annonceur en Keuros uniquement  pour les lignes total produits éventuels
        /// -la 1ere référence en Keuros uniquement  pour les lignes total produits éventuels
        /// Sur la dimension support le tableau est décliné de la façon suivante :
        /// -si plusieurs media, le tableua sera décliné par media
        /// -si un seul media, le tableau sera décliné par media, catégorie et supports
        /// </summary>				
        /// <param name="page">Page qui affiche les statégies média</param>
        /// <param name="tab">tableau des résultats</param>	
        /// <param name="webSession">Session du client</param>
        /// <param name="excel">booléen pour sortie html ou excel</param>	
        /// <returns>Code HTML</returns>		
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
        public override ResultTable GetResultTable()
        {

            ResultTable resultTable = null;
            List<DetailLevelItemInformation> levels = null;
            bool showProduct = false;

            #region GetData
            DataSet ds = _dalLayer.GetMediaStrategyData();

            #endregion

            #region Pas de données à afficher
            if (ds == null || ds.Tables == null || ds.Tables.Count == 0)
            {
                return null;
            }
            #endregion

            #region Get Right Product
            showProduct = _session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            #endregion

            #region Initialize Datatable
            levels = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
            #endregion

            #region Headers
            Headers headers = new Headers();

            //Header Media
            int indexCol = 1;
            Header currentHeader = new Header(string.Empty, indexCol);
            headers.Root.Add(currentHeader);


            //Header Type line
            indexCol++;
            currentHeader = new Header(string.Empty, indexCol);
            headers.Root.Add(currentHeader);

            //Header Investment
            indexCol++;
            currentHeader = new Header(GestionWeb.GetWebWord(1246, _session.SiteLanguage) + " " + _periodEnd.Year, true, indexCol);
            headers.Root.Add(currentHeader);

            //Header PDM
            indexCol++;
            currentHeader = new Header(GestionWeb.GetWebWord(806, _session.SiteLanguage), indexCol);
            headers.Root.Add(currentHeader);

            //Header Evol
            if (_session.ComparativeStudy)
            {
                indexCol++;
                currentHeader = new Header(GestionWeb.GetWebWord(1168, _session.SiteLanguage) + " " + _periodEnd.Year + "/" + (_periodEnd.Year - 1), indexCol);
                headers.Root.Add(currentHeader);
            }

            //Header Advertiser
            indexCol++;
            currentHeader = new Header(GestionWeb.GetWebWord(1154, _session.SiteLanguage), true, indexCol);
            headers.Root.Add(currentHeader);

            //Header Advertiser Investment
            indexCol++;
            currentHeader = new Header(GestionWeb.GetWebWord(1246, _session.SiteLanguage) + " " + _periodEnd.Year, indexCol);
            headers.Root.Add(currentHeader);

            if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
            {

                //Header Product
                indexCol++;
                currentHeader = new Header(GestionWeb.GetWebWord(1155, _session.SiteLanguage), true, indexCol);
                headers.Root.Add(currentHeader);

                //Header Product Investment
                indexCol++;
                currentHeader = new Header(GestionWeb.GetWebWord(1246, _session.SiteLanguage) + " " + _periodEnd.Year, indexCol);
                headers.Root.Add(currentHeader);
            }
            #endregion

            #region Initialize ResultTable
            int nbLine = 0;
            for (int i = 1; i <= levels.Count; i++)
            {
                nbLine += ds.Tables["LEVEL " + i].Rows.Count;
                if (showProduct && ds.Tables.Contains("LEVEL " + i + " REF")) nbLine += ds.Tables["LEVEL " + i + " REF"].Rows.Count;
            }
            resultTable = new ResultTable(nbLine, headers);
            #endregion

            #region Build table
            DataTable dtL1Distinct = null;
            DataRow[] currentLevelDataRowList;
            DataRow[] currentLevelRefDataRowList;
            DataRow currentLevelDataRow;
            int currentTotalType = 0;
            LineType currentLineType = LineType.total;
            Int64 currentLevel1Id = long.MinValue;
            Int64 currentLevel2Id = long.MinValue;
            Int64 currentLevel3Id = long.MinValue;

            object oLabel = null;
            object oTotal = null;
            object oPDM = null;
            object oEvol = null;
            object oAdvLabel = null;
            object oAdvInvest = null;
            object oRefLabel = null;
            object oRefInvest = null;

            #region Get All ID Distinct for the Level 1
            System.Data.DataView Dv = ds.Tables["LEVEL 1"].DefaultView;
            dtL1Distinct = Dv.ToTable(true, "ID_LEVEL1");
            #endregion

            //For chaque id level1 distinct du Level1
            foreach (DataRow currentRow in dtL1Distinct.Rows)
            {
                currentLevel2Id = long.MinValue;

                #region Level 1
                currentLevel1Id = Int64.Parse(currentRow["ID_LEVEL1"].ToString());
                currentLevelDataRowList = ds.Tables["LEVEL 1"].Select("ID_LEVEL1=" + currentLevel1Id);
                for (int k = 0; k < currentLevelDataRowList.GetLength(0); k++)
                {

                    currentLevelDataRow = currentLevelDataRowList[k];

                    #region GetTypeLine
                    currentTotalType = Int32.Parse(currentLevelDataRow["TOTAL_TYPE"].ToString());
                    #endregion

                    #region Get Information for the current Level type
                    switch (currentTotalType)
                    {
                        case 0:

                            #region Total market
                            oLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                            #endregion

                            break;
                        case 1:

                            #region Total Category
                            oLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                            #endregion

                            break;
                        case 2:

                            #region Total Univers
                            oLabel = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                            #endregion

                            break;
                        default:
                            throw new ProductClassIndicatorsException("Current Total Type is not defined");
                    }
                    #endregion

                    #region Get Data Value
                    oTotal = currentLevelDataRow["UNIT_N"];
                    oPDM = currentLevelDataRow["MARKET_SHARE"];
                    if (_session.ComparativeStudy) oEvol = currentLevelDataRow["CHANGE"];
                    else oEvol = null;
                    oAdvLabel = currentLevelDataRow["TOP_ADVERTISER"];
                    oAdvInvest = currentLevelDataRow["TOP_ADV_UNIT"];
                    if (showProduct)
                    {
                        oRefLabel = currentLevelDataRow["TOP_PRODUCT"];
                        oRefInvest = currentLevelDataRow["TOP_PROD_UNIT"];
                    }
                    else
                    {
                        oRefLabel = null;
                        oRefInvest = null;
                    }
                    #endregion

                    AppendLineResultTable(LineType.total, resultTable, (k == 0) ? currentLevelDataRow["LEVEL1"] : null, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);
                }

                #region Reference and competitor elements
                if (showProduct && ds.Tables.Contains("LEVEL 1 REF") && ds.Tables["LEVEL 1 REF"] != null)
                {

                    currentLevelRefDataRowList = ds.Tables["LEVEL 1 REF"].Select("ID_LEVEL1=" + currentLevel1Id);
                    foreach (DataRow currentDataRow in currentLevelRefDataRowList)
                    {

                        #region Reference or competitor?
                        Int64 idAdv = Convert.ToInt64(currentDataRow["ID_ADVERTISER"]);
                        if (_referenceIDS.Contains(idAdv))
                        {
                            currentLineType = LineType.level1;
                        }
                        else if (_competitorIDS.Contains(idAdv))
                        {
                            currentLineType = LineType.level2;
                        }
                        #endregion

                        #region Get Data Value
                        oLabel = currentDataRow["ADVERTISER"];
                        oTotal = currentDataRow["UNIT_N"];
                        oPDM = currentDataRow["MARKET_SHARE"];
                        if (_session.ComparativeStudy) oEvol = currentDataRow["CHANGE"];
                        else oEvol = null;
                        #endregion

                        AppendLineResultTable(currentLineType, resultTable, null, oLabel, oTotal, oPDM, oEvol, null, null, null, null);
                    }

                }
                #endregion

                #endregion

                #region Level 2 & 3
                if (ds.Tables.Contains("LEVEL 2") && levels.Count > 1)
                {
                    ds.Tables["LEVEL 2"].DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id;
                    ds.Tables["LEVEL 2"].DefaultView.Sort = "ID_LEVEL2";

                    foreach (DataRowView LevelDataRowView in ds.Tables["LEVEL 2"].DefaultView)
                    {
                        currentLevel3Id = long.MinValue;

                        if (currentLevel2Id != Int64.Parse(LevelDataRowView["ID_LEVEL2"].ToString()))
                        {
                            currentLevel2Id = Int64.Parse(LevelDataRowView["ID_LEVEL2"].ToString());

                            DataTable currentLevel2List = ds.Tables["LEVEL 2"].DefaultView.ToTable("LEVEL_2");
                            currentLevel2List.DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " and ID_LEVEL2=" + currentLevel2Id;
                            currentLevel2List.DefaultView.Sort = "TOTAL_TYPE";
                            Boolean First = true;
                            foreach (DataRowView currentLevelDataRowView in currentLevel2List.DefaultView)
                            {

                                currentLevelDataRow = currentLevelDataRowView.Row;

                                #region GetTypeLine
                                currentTotalType = Int32.Parse(currentLevelDataRow["TOTAL_TYPE"].ToString());
                                #endregion

                                #region Get Information for the current Level type
                                switch (currentTotalType)
                                {
                                    case 0:

                                        #region Total market
                                        oLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                                        #endregion

                                        break;
                                    case 1:

                                        #region Total Category
                                        oLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                                        #endregion

                                        break;
                                    case 2:

                                        #region Total Univers
                                        oLabel = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                                        #endregion

                                        break;
                                    default:
                                        throw new ProductClassIndicatorsException("Current Total Type is not defined");
                                }
                                #endregion

                                #region Get Data Value
                                oTotal = currentLevelDataRow["UNIT_N"];
                                oPDM = currentLevelDataRow["MARKET_SHARE"];
                                if (_session.ComparativeStudy) oEvol = currentLevelDataRow["CHANGE"];
                                else oEvol = null;
                                oAdvLabel = currentLevelDataRow["TOP_ADVERTISER"];
                                oAdvInvest = currentLevelDataRow["TOP_ADV_UNIT"];
                                if (showProduct)
                                {
                                    oRefLabel = currentLevelDataRow["TOP_PRODUCT"];
                                    oRefInvest = currentLevelDataRow["TOP_PROD_UNIT"];
                                }
                                else
                                {
                                    oRefLabel = null;
                                    oRefInvest = null;
                                }
                                #endregion

                                AppendLineResultTable(LineType.subTotal1, resultTable, First ? currentLevelDataRow["LEVEL2"] : null, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);
                                First = false;
                            }

                            #region Reference and competitor elements
                            if (showProduct && ds.Tables.Contains("LEVEL 2 REF"))
                            {

                                ds.Tables["LEVEL 2 REF"].DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " and ID_LEVEL2=" + currentLevel2Id;
                                foreach (DataRowView currentDataRowView in ds.Tables["LEVEL 2 REF"].DefaultView)
                                {
                                    DataRow currentDataRow = currentDataRowView.Row;
                                    currentLineType = LineType.subTotal1;

                                    #region Reference or competitor?
                                    Int64 idAdv = Convert.ToInt64(currentDataRow["ID_ADVERTISER"]);
                                    if (_referenceIDS.Contains(idAdv))
                                    {
                                        currentLineType = LineType.level1;
                                    }
                                    else if (_competitorIDS.Contains(idAdv))
                                    {
                                        currentLineType = LineType.level2;
                                    }
                                    #endregion

                                    #region Get Data Value
                                    oLabel = currentDataRow["ADVERTISER"];
                                    oTotal = currentDataRow["UNIT_N"];
                                    oPDM = currentDataRow["MARKET_SHARE"];
                                    if (_session.ComparativeStudy) oEvol = currentDataRow["CHANGE"];
                                    else oEvol = null;
                                    #endregion

                                    AppendLineResultTable(currentLineType, resultTable, null, oLabel, oTotal, oPDM, oEvol, null, null, null, null);
                                }
                            }
                      
                            #endregion

                        #region Level 3
                        if (ds.Tables.Contains("LEVEL 3") && levels.Count > 2)
                        {
                            ds.Tables["LEVEL 3"].DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " AND ID_LEVEL2=" + currentLevel2Id;
                            ds.Tables["LEVEL 3"].DefaultView.Sort = "ID_LEVEL3";

                            foreach (DataRowView Level3DataRowView in ds.Tables["LEVEL 3"].DefaultView)
                            {
                                if (currentLevel3Id != Int64.Parse(Level3DataRowView["ID_LEVEL3"].ToString()))
                                {
                                    currentLevel3Id = Int64.Parse(Level3DataRowView["ID_LEVEL3"].ToString());

                                    DataTable currentLevel3List = ds.Tables["LEVEL 3"].DefaultView.ToTable("LEVEL_3");
                                    currentLevel3List.DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " and ID_LEVEL2=" + currentLevel2Id + " and ID_LEVEL3=" + currentLevel3Id;
                                    currentLevel3List.DefaultView.Sort = "TOTAL_TYPE";
                                    First = true;
                                    foreach (DataRowView currentLevelDataRowView in currentLevel3List.DefaultView)
                                    {

                                        currentLevelDataRow = currentLevelDataRowView.Row;
                                        
                                        #region GetTypeLine
                                        currentTotalType = Int32.Parse(currentLevelDataRow["TOTAL_TYPE"].ToString());
                                        #endregion

                                        #region Get Information for the current Level type
                                        switch (currentTotalType)
                                        {
                                            case 0:

                                                #region Total market
                                                oLabel = GestionWeb.GetWebWord(1190, _session.SiteLanguage);
                                                #endregion

                                                break;
                                            case 1:

                                                #region Total Category
                                                oLabel = GestionWeb.GetWebWord(1189, _session.SiteLanguage);
                                                #endregion

                                                break;
                                            case 2:

                                                #region Total Univers
                                                oLabel = GestionWeb.GetWebWord(1188, _session.SiteLanguage);
                                                #endregion

                                                break;
                                            default:
                                                throw new ProductClassIndicatorsException("Current Total Type is not defined");
                                        }
                                        #endregion

                                        #region Get Data Value
                                        oTotal = currentLevelDataRow["UNIT_N"];
                                        oPDM = currentLevelDataRow["MARKET_SHARE"];
                                        if (_session.ComparativeStudy) oEvol = currentLevelDataRow["CHANGE"];
                                        else oEvol = null;
                                        oAdvLabel = currentLevelDataRow["TOP_ADVERTISER"];
                                        oAdvInvest = currentLevelDataRow["TOP_ADV_UNIT"];
                                        if (showProduct)
                                        {
                                            oRefLabel = currentLevelDataRow["TOP_PRODUCT"];
                                            oRefInvest = currentLevelDataRow["TOP_PROD_UNIT"];
                                        }
                                        else
                                        {
                                            oRefLabel = null;
                                            oRefInvest = null;
                                        }
                                        #endregion

                                        AppendLineResultTable(LineType.subTotal2, resultTable, First ? currentLevelDataRow["LEVEL3"] : null, oLabel, oTotal, oPDM, oEvol, oAdvLabel, oAdvInvest, oRefLabel, oRefInvest);
                                        First = false;
                                    }

                                    #region Reference and competitor elements
                                    if (showProduct && ds.Tables.Contains("LEVEL 3 REF"))
                                    {
                                        ds.Tables["LEVEL 3 REF"].DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " and ID_LEVEL2=" + currentLevel2Id + " and ID_LEVEL3=" + currentLevel3Id;
                                        foreach (DataRowView currentDataRowView in ds.Tables["LEVEL 3 REF"].DefaultView)
                                        {
                                            DataRow currentDataRow = currentDataRowView.Row;
                                     
                                            currentLineType = LineType.subTotal2;

                                            #region Reference or competitor?
                                            Int64 idAdv = Convert.ToInt64(currentDataRow["ID_ADVERTISER"]);
                                            if (_referenceIDS.Contains(idAdv))
                                            {
                                                currentLineType = LineType.level1;
                                            }
                                            else if (_competitorIDS.Contains(idAdv))
                                            {
                                                currentLineType = LineType.level2;
                                            }
                                            #endregion

                                            #region Get Data Value
                                            oLabel = currentDataRow["ADVERTISER"];
                                            oTotal = currentDataRow["UNIT_N"];
                                            oPDM = currentDataRow["MARKET_SHARE"];
                                            if (_session.ComparativeStudy) oEvol = currentDataRow["CHANGE"];
                                            else oEvol = null;
                                            #endregion

                                            AppendLineResultTable(currentLineType, resultTable, null, oLabel, oTotal, oPDM, oEvol, null, null, null, null);
                                        }

                                    }
                                    #endregion

                                }
                            }
                        }
                        #endregion
                        }

                    }

                }
                #endregion

            }
            #endregion

            return resultTable;

        }
        #endregion

        #region GetChartData
        /// <summary>
        /// La répartition média sur le total de la période dans un tableau
        /// contenant les éléments ci-après :
        /// en ligne :
        /// -le total famille (en option uniquement en sélection de groupe/variétés) ou le
        /// total marché (en option)
        /// -les éléments de références
        /// -les éléments concurrents 
        /// en colonne :
        /// -Les investissements de la période N		
        /// Sur la dimension support le tableau est décliné de la façon suivante :
        /// -si plusieurs media, le tableau sera décliné par media
        /// -si un seul media, le tableau sera décliné par media, catégorie et supports
        /// </summary>
        /// <remarks>Cette méthode est utilisée pour la présentation graphique des résultats.</remarks>
        /// <returns>tableau de résultats</returns>
        public override object[,] GetChartData()
        {
            object[,] tabResult = null;
            List<DetailLevelItemInformation> levels = null;
            bool showProduct = false;

            #region GetData
            DataSet ds = _dalLayer.GetMediaStrategyData();
            #endregion

            #region Pas de données à afficher
            if (ds == null || ds.Tables == null || ds.Tables.Count == 0)
            {
                return null;
            }
            #endregion

            #region Get Right Product
            showProduct = _session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            #endregion

            #region Initialize Datatable
            levels = DetailLevelItemsInformation.Translate(_session.PreformatedMediaDetail);
            #endregion

            #region Initialize ResultTable
            int nbLine = 0;
            for (int i = 1; i <= levels.Count; i++)
            {
                nbLine += ds.Tables["Level " + i].Rows.Count;
                if (showProduct && ds.Tables.Contains("Level " + i + " REF")) nbLine += ds.Tables["Level " + i + " REF"].Rows.Count;
            }
            tabResult = new object[nbLine, NB_CHART_MAX_COLUMNS];
            #endregion

            #region Build table
            DataTable dtL1Distinct = null;
            DataRow[] currentLevelDataRowList;
            DataRow[] currentLevelRefDataRowList;
            DataRow currentLevelDataRow;
            int currentTotalType = 0;
            Int64 currentLevel1Id = long.MinValue;
            Int64 currentLevel2Id = long.MinValue;
            Int64 currentLevel3Id = long.MinValue;
            string currentLevel1Label = string.Empty;
            string currentLevel2Label = string.Empty;
            string currentLevel3Label = string.Empty;

            #region Get All ID Distinct for the Level 1
            System.Data.DataView Dv = ds.Tables["Level 1"].DefaultView;
            dtL1Distinct = Dv.ToTable(true, "ID_LEVEL1");
            #endregion

            //For chaque id level1 distinct du Level1
            int currentLine = 0;
            foreach (DataRow currentRow in dtL1Distinct.Rows)
            {
                currentLevel2Id = long.MinValue;

                #region Level 1
                currentLevel1Id = Int64.Parse(currentRow["ID_LEVEL1"].ToString());
                currentLevelDataRowList = ds.Tables["Level 1"].Select("ID_LEVEL1=" + currentLevel1Id);
                for (int k = 0; k < currentLevelDataRowList.GetLength(0); k++)
                {

                    currentLevelDataRow = currentLevelDataRowList[k];
                    currentLevel1Label = currentLevelDataRow["LEVEL1"].ToString();

                    #region GetTypeLine
                    currentTotalType = Int32.Parse(currentLevelDataRow["TOTAL_TYPE"].ToString());
                    #endregion

                    #region Get Information for the current Level type
                    switch (currentTotalType)
                    {
                        case 0:

                            #region Total market
                            tabResult[currentLine, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = null;
                            tabResult[currentLine, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = null;
                            tabResult[currentLine, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                            #endregion

                            break;
                        case 1:

                            #region Total Category
                            tabResult[currentLine, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = null;
                            tabResult[currentLine, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                            tabResult[currentLine, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = null;
                            #endregion

                            break;
                        case 2:

                            #region Total Univers
                            tabResult[currentLine, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                            tabResult[currentLine, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = null;
                            tabResult[currentLine, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = null;
                            #endregion

                            break;
                        default:
                            throw new ProductClassIndicatorsException("Current Total Type is not defined");
                    }
                    #endregion

                    #region Get Data Value
                    tabResult[currentLine, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, ID_VEHICLE_COLUMN_INDEX] = currentLevel1Id;
                    tabResult[currentLine, LABEL_VEHICLE_COLUMN_INDEX] = currentLevel1Label;
                    tabResult[currentLine, ID_CATEGORY_COLUMN_INDEX] = null;
                    tabResult[currentLine, LABEL_CATEGORY_COLUMN_INDEX] = null;
                    tabResult[currentLine, ID_MEDIA_COLUMN_INDEX] = null;
                    tabResult[currentLine, LABEL_MEDIA_COLUMN_INDEX] = null;
                    tabResult[currentLine, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = null;
                    tabResult[currentLine, LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_UNIV_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_SECTOR_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_MARKET_INVEST_COLUMN_INDEX] = null;
                    tabResult[currentLine, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                    currentLine++;
                    #endregion
                }

                #region Reference and competitor elements
                if (showProduct && ds.Tables.Contains("Level 1 REF") && ds.Tables["Level 1 REF"] != null)
                {

                    currentLevelRefDataRowList = ds.Tables["Level 1 REF"].Select("ID_LEVEL1=" + currentLevel1Id);
                    foreach (DataRow currentDataRow in currentLevelRefDataRowList)
                    {

                        tabResult[currentLine, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = currentDataRow["UNIT_N"];
                        tabResult[currentLine, ID_VEHICLE_COLUMN_INDEX] = currentLevel1Id;
                        tabResult[currentLine, LABEL_VEHICLE_COLUMN_INDEX] = currentLevel1Label;
                        tabResult[currentLine, ID_CATEGORY_COLUMN_INDEX] = null;
                        tabResult[currentLine, LABEL_CATEGORY_COLUMN_INDEX] = null;
                        tabResult[currentLine, ID_MEDIA_COLUMN_INDEX] = null;
                        tabResult[currentLine, LABEL_MEDIA_COLUMN_INDEX] = null;
                        tabResult[currentLine, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = Convert.ToInt64(currentDataRow["ID_ADVERTISER"]);
                        tabResult[currentLine, LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = currentDataRow["ADVERTISER"];
                        tabResult[currentLine, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_UNIV_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_SECTOR_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_MARKET_INVEST_COLUMN_INDEX] = null;
                        tabResult[currentLine, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                        currentLine++;
                    }

                }
                #endregion

                #endregion

                #region Level 2 & 3
                if (ds.Tables.Contains("Level 2") && levels.Count > 1)
                {
                    ds.Tables["LEVEL 2"].DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id;
                    ds.Tables["LEVEL 2"].DefaultView.Sort = "ID_LEVEL2";

                    foreach (DataRowView LevelDataRowView in ds.Tables["LEVEL 2"].DefaultView)
                    {
                        currentLevel3Id = long.MinValue;

                        if (currentLevel2Id != Int64.Parse(LevelDataRowView["ID_LEVEL2"].ToString()))
                        {
                            currentLevel2Id = Int64.Parse(LevelDataRowView["ID_LEVEL2"].ToString());
                            currentLevel2Label = LevelDataRowView["LEVEL2"].ToString();

                            DataTable currentLevel2List = ds.Tables["LEVEL 2"].DefaultView.ToTable("LEVEL_2");
                            currentLevel2List.DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " and ID_LEVEL2=" + currentLevel2Id;
                            currentLevel2List.DefaultView.Sort = "TOTAL_TYPE";
                            foreach (DataRowView currentLevelDataRowView in currentLevel2List.DefaultView)
                            {

                                currentLevelDataRow = currentLevelDataRowView.Row;

                                #region GetTypeLine
                                currentTotalType = Int32.Parse(currentLevelDataRow["TOTAL_TYPE"].ToString());
                                #endregion

                                #region Get Information for the current Level type
                                switch (currentTotalType)
                                {
                                    case 0:

                                        #region Total market
                                        tabResult[currentLine, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                        tabResult[currentLine, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                        tabResult[currentLine, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                                        #endregion

                                        break;
                                    case 1:

                                        #region Total Category
                                        tabResult[currentLine, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                        tabResult[currentLine, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                                        tabResult[currentLine, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                        #endregion

                                        break;
                                    case 2:

                                        #region Total Univers
                                        tabResult[currentLine, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                                        tabResult[currentLine, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                        tabResult[currentLine, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                        #endregion

                                        break;
                                    default:
                                        throw new ProductClassIndicatorsException("Current Total Type is not defined");
                                }
                                #endregion

                                #region Get values
                                tabResult[currentLine, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, ID_VEHICLE_COLUMN_INDEX] = null;
                                tabResult[currentLine, LABEL_VEHICLE_COLUMN_INDEX] = null;
                                tabResult[currentLine, ID_CATEGORY_COLUMN_INDEX] = currentLevel2Id;
                                tabResult[currentLine, LABEL_CATEGORY_COLUMN_INDEX] = currentLevel2Label;
                                tabResult[currentLine, ID_MEDIA_COLUMN_INDEX] = null;
                                tabResult[currentLine, LABEL_MEDIA_COLUMN_INDEX] = null;
                                tabResult[currentLine, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = null;
                                tabResult[currentLine, LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_UNIV_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_SECTOR_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_MARKET_INVEST_COLUMN_INDEX] = null;
                                tabResult[currentLine, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                                currentLine++;
                                #endregion
                            }

                            #region Reference and competitor elements
                            if (showProduct && ds.Tables.Contains("Level 2 REF") && ds.Tables["Level 2 REF"] != null)
                            {
                                ds.Tables["LEVEL 2 REF"].DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " and ID_LEVEL2=" + currentLevel2Id;
                                foreach (DataRowView currentDataRowView in ds.Tables["LEVEL 2 REF"].DefaultView)
                                {
                                    DataRow currentDataRow = currentDataRowView.Row;

                                    tabResult[currentLine, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = currentDataRow["UNIT_N"];
                                    tabResult[currentLine, ID_VEHICLE_COLUMN_INDEX] = currentLevel1Id;
                                    tabResult[currentLine, LABEL_VEHICLE_COLUMN_INDEX] = currentLevel1Label;
                                    tabResult[currentLine, ID_CATEGORY_COLUMN_INDEX] = currentLevel2Id;
                                    tabResult[currentLine, LABEL_CATEGORY_COLUMN_INDEX] = currentLevel2Label;
                                    tabResult[currentLine, ID_MEDIA_COLUMN_INDEX] = null;
                                    tabResult[currentLine, LABEL_MEDIA_COLUMN_INDEX] = null;
                                    tabResult[currentLine, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = Convert.ToInt64(currentDataRow["ID_ADVERTISER"]);
                                    tabResult[currentLine, LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = currentDataRow["ADVERTISER"];
                                    tabResult[currentLine, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_UNIV_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_SECTOR_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_MARKET_INVEST_COLUMN_INDEX] = null;
                                    tabResult[currentLine, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                                    currentLine++;
                                }

                            }
                            #endregion

                            #region Level 3
                            if (ds.Tables.Contains("Level 3") && levels.Count > 2)
                            {

                                ds.Tables["LEVEL 3"].DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " AND ID_LEVEL2=" + currentLevel2Id;
                                ds.Tables["LEVEL 3"].DefaultView.Sort = "ID_LEVEL3";

                                foreach (DataRowView Level3DataRowView in ds.Tables["LEVEL 3"].DefaultView)
                                {
                                    if (currentLevel3Id != Int64.Parse(Level3DataRowView["ID_LEVEL3"].ToString()))
                                    {
                                        currentLevel3Id = Int64.Parse(Level3DataRowView["ID_LEVEL3"].ToString());
                                        currentLevel3Label = Level3DataRowView["LEVEL3"].ToString();

                                        DataTable currentLevel3List = ds.Tables["LEVEL 3"].DefaultView.ToTable("LEVEL_3");
                                        currentLevel3List.DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " and ID_LEVEL2=" + currentLevel2Id + " and ID_LEVEL3=" + currentLevel3Id;
                                        currentLevel3List.DefaultView.Sort = "TOTAL_TYPE";
                                        foreach (DataRowView currentLevelDataRowView in currentLevel3List.DefaultView)
                                        {

                                            currentLevelDataRow = currentLevelDataRowView.Row;

                                            #region GetTypeLine
                                            currentTotalType = Int32.Parse(currentLevelDataRow["TOTAL_TYPE"].ToString());
                                            #endregion

                                            #region Get Information for the current Level type
                                            switch (currentTotalType)
                                            {
                                                case 0:

                                                    #region Total market
                                                    tabResult[currentLine, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                    tabResult[currentLine, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                    tabResult[currentLine, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                                                    #endregion

                                                    break;
                                                case 1:

                                                    #region Total Category
                                                    tabResult[currentLine, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                    tabResult[currentLine, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                                                    tabResult[currentLine, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                    #endregion

                                                    break;
                                                case 2:

                                                    #region Total Univers
                                                    tabResult[currentLine, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = currentLevelDataRow["UNIT_N"];
                                                    tabResult[currentLine, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                    tabResult[currentLine, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                    #endregion

                                                    break;
                                                default:
                                                    throw new ProductClassIndicatorsException("Current Total Type is not defined");
                                            }
                                            #endregion

                                            #region Get Data values
                                            tabResult[currentLine, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, ID_VEHICLE_COLUMN_INDEX] = null;
                                            tabResult[currentLine, LABEL_VEHICLE_COLUMN_INDEX] = null;
                                            tabResult[currentLine, ID_CATEGORY_COLUMN_INDEX] = null;
                                            tabResult[currentLine, LABEL_CATEGORY_COLUMN_INDEX] = null;
                                            tabResult[currentLine, ID_MEDIA_COLUMN_INDEX] = currentLevel3Id;
                                            tabResult[currentLine, LABEL_MEDIA_COLUMN_INDEX] = currentLevel3Label;
                                            tabResult[currentLine, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = null;
                                            tabResult[currentLine, LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_UNIV_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_SECTOR_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_MARKET_INVEST_COLUMN_INDEX] = null;
                                            tabResult[currentLine, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                                            currentLine++;
                                            #endregion

                                        }

                                        #region Reference and competitor elements
                                        if (showProduct && ds.Tables.Contains("Level 3 REF") && ds.Tables["Level 3 REF"] != null)
                                        {

                                            ds.Tables["LEVEL 3 REF"].DefaultView.RowFilter = "ID_LEVEL1=" + currentLevel1Id + " and ID_LEVEL2=" + currentLevel2Id + " and ID_LEVEL3=" + currentLevel3Id;
                                            foreach (DataRowView currentDataRowView in ds.Tables["LEVEL 3 REF"].DefaultView)
                                            {
                                                DataRow currentDataRow = currentDataRowView.Row;

                                                tabResult[currentLine, REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = currentDataRow["UNIT_N"];
                                                tabResult[currentLine, ID_VEHICLE_COLUMN_INDEX] = currentLevel1Id;
                                                tabResult[currentLine, LABEL_VEHICLE_COLUMN_INDEX] = currentLevel1Label;
                                                tabResult[currentLine, ID_CATEGORY_COLUMN_INDEX] = currentLevel2Id;
                                                tabResult[currentLine, LABEL_CATEGORY_COLUMN_INDEX] = currentLevel2Label;
                                                tabResult[currentLine, ID_MEDIA_COLUMN_INDEX] = currentLevel3Id;
                                                tabResult[currentLine, LABEL_MEDIA_COLUMN_INDEX] = currentLevel3Label;
                                                tabResult[currentLine, ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = Convert.ToInt64(currentDataRow["ID_ADVERTISER"]);
                                                tabResult[currentLine, LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] = currentDataRow["ADVERTISER"];
                                                tabResult[currentLine, TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_UNIV_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_SECTOR_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_MARKET_INVEST_COLUMN_INDEX] = null;
                                                tabResult[currentLine, TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] = null;
                                                currentLine++;

                                            }

                                        }
                                        #endregion

                                    }
                                }
                            #endregion


                            }
                        }
                    }
                #endregion
                }
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