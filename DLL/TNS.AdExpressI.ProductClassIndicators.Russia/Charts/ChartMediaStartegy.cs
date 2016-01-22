using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Russia.Engines;
using System.Drawing;

using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstDbClassif = TNS.AdExpress.Constantes.Classification.DB;

using TNS.AdExpress.Domain.Translation;
using Dundas.Charting.WebControl;
using System.Data;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

using System.Web.UI.WebControls;

namespace TNS.AdExpressI.ProductClassIndicators.Russia.Charts
{

    [ToolboxData("<{0}:ChartMediaStartegy runat=server></{0}:ChartMediaStartegy>")]
    public class ChartMediaStartegy : TNS.AdExpressI.ProductClassIndicators.Charts.ChartMediaStartegy
    {

		#region Constructor
		/// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public ChartMediaStartegy(WebSession session, IProductClassIndicatorsDAL dalLayer)
            : base(session, dalLayer)
        {
        }
        #endregion

        #region Init Engine
        /// <summary>
        /// Init Engine
        /// </summary>
        override protected void InitEngine()
        {
            _engine = new EngineMediaStrategy(this._session, this._dalLayer);
        }
        #endregion

        protected override int GetMediaLevelNumber()
        {
            switch (_session.PreformatedMediaDetail)
            {
                case CstPreformatedDetail.PreformatedMediaDetails.vehicle:
                case CstPreformatedDetail.PreformatedMediaDetails.region:
                case CstPreformatedDetail.PreformatedMediaDetails.Media:
                    return 1;                
                case CstPreformatedDetail.PreformatedMediaDetails.vehicleRegion:
                case CstPreformatedDetail.PreformatedMediaDetails.regionVehicle:
                    return 2;
                default:
                    return 3;
            }
        }

        protected override void ComputeTotals(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, ref double totalUniversValue, ref double totalSectorValue, ref double totalMarketValue, int MEDIA_LEVEL_NUMBER)
        {
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;

            if (MEDIA_LEVEL_NUMBER >0)
            {
                for (int i = 0; i < tab.GetLongLength(0); i++)
                {
                    for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                    {
                        switch (j)
                        {

                            #region support
                            // Univers Total
                            case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX],fp);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            #endregion

                            #region Advertisers
                            case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                                if (!IsNull(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX])
                                    && !IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])                                
                                   )
                                {
                                    listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            #endregion

                            #region Category
                            // Univers Total	
                            case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:

                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX], fp);
                                }

                                break;
                            #endregion

                            #region PluriMedia
                            // Univers Total	
                            case EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:

                                if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX], fp);
                                }

                                break;
                            #endregion

                            default:
                                break;
                        }
                    }
                }
            }
          

       
        }

        protected override void FillTable(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, Dictionary<string, DataTable> listTableRefCompetitor, DataTable tableUnivers, DataTable tableSectorMarket, ref double totalUniversValue, ref double totalSectorValue, ref double totalMarketValue, int MEDIA_LEVEL_NUMBER, bool withPluriByCategory)
        {
            #region Table
            double elementValue;
           
            tableUnivers.Columns.Add("Name");
            tableUnivers.Columns.Add("Position", typeof(double));
            tableSectorMarket.Columns.Add("Name");
            tableSectorMarket.Columns.Add("Position", typeof(double));
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;

            for (int i = 0; i < tab.GetLongLength(0); i++)
            {
                for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                {
                    switch (j)
                    {

                        #region Media
                        case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX],fp) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                                j = j + 6;
                            }

                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX], fp) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 5;
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX], fp) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 4;
                            }
                            break;
                        #endregion

                        #region Advertisers
                        case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] )
                                && !IsNull(tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] )
                                && !IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] )
                                && MEDIA_LEVEL_NUMBER == 3)
                            {

                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX], fp) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }

                                j = j + 12;
                            }
                            else if (!IsNull(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX])
                                && !IsNull(tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX])
                                && IsNull(tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX])
                                && !IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])
                                && MEDIA_LEVEL_NUMBER == 2)
                            {

                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX], fp) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            else if (!IsNull(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX])
                             && !IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])
                             && !IsNull(tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX])
                             && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX], fp) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            break;
                        #endregion

                        #region Categorie
                        case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] ) && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX], fp) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX])&& MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX], fp) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX], fp) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        #endregion

                        #region PluriMedia
                        case EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] ) && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX], fp) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX], fp) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:
                            if (!IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] ) && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX], fp) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        #endregion

                        default:
                            break;
                    }
                }
            }
            #endregion
        }

        #region  Init Series
        protected override void InitSeries(DataTable tableUnivers, DataTable tableSectorMarket, double[] yValues, string[] xValues, double[] yValuesSectorMarket, string[] xValuesSectorMarket, Dictionary<string, Series> listSeriesMedia, Dictionary<string, DataTable> listTableRefCompetitor, Dictionary<int, string> listSeriesName, int MEDIA_LEVEL_NUMBER)
        {
            #region Init Series
            string strSort = "Position  DESC";
            DataRow[] foundRows = null;
            foundRows = tableUnivers.Select("", strSort);
            DataRow[] foundRowsSectorMarket = null;
            foundRowsSectorMarket = tableSectorMarket.Select("", strSort);          
            double otherUniversValue = 0;
            double otherSectorMarketValue = 0;
            int index = 0;
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;


            for (int i = 0; i < foundRows.Length; i++)
            {
                xValues[i] = foundRows[i]["Name"].ToString();
                yValues[i] = Convert.ToDouble(foundRows[i]["Position"], fp);
                otherUniversValue += Convert.ToDouble(foundRows[i]["Position"], fp);
            }

            for (int i = 0; i < foundRowsSectorMarket.Length; i++)
            {
                xValuesSectorMarket[i] = foundRowsSectorMarket[i]["Name"].ToString();
                yValuesSectorMarket[i] = Convert.ToDouble(foundRowsSectorMarket[i]["Position"], fp);
                otherSectorMarketValue += Convert.ToDouble(foundRowsSectorMarket[i]["Position"], fp);
            }
           

            double[] yVal = new double[foundRows.Length];
            string[] xVal = new string[foundRows.Length];
           // double otherCompetitorRefValue = 0;
            int k = 2;

            foreach (string name in listSeriesMedia.Keys)
            {

                if (name == GestionWeb.GetWebWord(1780, _session.SiteLanguage))
                {
                    if (xValues != null && xValues.Length > 0 && xValues[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1780, _session.SiteLanguage)].Points.DataBindXY(xValues, yValues);
                }
                else if (_session.ComparaisonCriterion == CstComparisonCriterion.sectorTotal && name == GestionWeb.GetWebWord(1189, _session.SiteLanguage))
                {
                    if (xValuesSectorMarket != null && xValuesSectorMarket.Length > 0 && xValuesSectorMarket[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1189, _session.SiteLanguage)].Points.DataBindXY(xValuesSectorMarket, yValuesSectorMarket);
                }
                else if (name == GestionWeb.GetWebWord(1316, _session.SiteLanguage))
                {
                    if (xValuesSectorMarket != null && xValuesSectorMarket.Length > 0 && xValuesSectorMarket[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1316, _session.SiteLanguage)].Points.DataBindXY(xValuesSectorMarket, yValuesSectorMarket);
                }
                else
                {
                    DataRow[] foundRowsCompetitorRef = null;
                    foundRowsCompetitorRef = ((DataTable)listTableRefCompetitor[name]).Select("", strSort);
                    //otherCompetitorRefValue = 0;

                    yVal = new double[foundRowsCompetitorRef.Length];
                    xVal = new string[foundRowsCompetitorRef.Length];

                    for (int i = 0; i < foundRowsCompetitorRef.Length; i++)
                    {
                        xVal[i] = foundRowsCompetitorRef[i]["Name"].ToString();
                        yVal[i] = Convert.ToDouble(foundRowsCompetitorRef[i]["Position"], fp);

                    }
                    
                    if (xVal.Length > 0 && xVal[0] != null)
                        listSeriesMedia[name].Points.DataBindXY(xVal, yVal);


                    listSeriesName.Add(k, name);
                    k++;
                }

            }
            #endregion
        }
        #endregion

        protected override void CreatesSeries(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, Dictionary<string, DataTable> listTableRefCompetitor, Dictionary<string, Series> listSeriesMedia)
        {
            // Create series (one per media)
            for (int i = 0; i < tab.GetLongLength(0); i++)
            {

                //	Dictionary with advertiser label as key and total as value
                if (!IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]))
                {
                    if (!listSeriesMediaRefCompetitor.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        listSeriesMediaRefCompetitor.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), new double());
                    }

                    if (!listTableRefCompetitor.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        DataTable tableCompetitorRef = new DataTable();
                        tableCompetitorRef.Columns.Add("Name");
                        tableCompetitorRef.Columns.Add("Position", typeof(double));
                        listTableRefCompetitor.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), tableCompetitorRef);

                    }

                    if (!listSeriesMedia.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        listSeriesMedia.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), new Series());
                    }
                }
            }
        }

        protected virtual bool IsNull(object obj){
            return  (obj==null || obj==System.DBNull.Value);            
        }

    }

}
