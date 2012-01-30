using System;
using System.Drawing;

using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpressI.ProductClassIndicators.Russia.Engines;
using FctIndicators = TNS.AdExpressI.ProductClassIndicators.Russia.Functions;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using System.Globalization;

namespace TNS.AdExpressI.ProductClassIndicators.Russia.Charts
{
    public class ChartTop : TNS.AdExpressI.ProductClassIndicators.Charts.ChartTop
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">Data Access layer</param>
        /// <param name="classifLevel">Classification Detail (product or advertiser)</param>
        public ChartTop(WebSession session, IProductClassIndicatorsDAL dalLayer, CstResult.MotherRecap.ElementType classifLevel)
            : base(session, dalLayer, classifLevel)
        {
        }
        #endregion

        #region Init Engine
        /// <summary>
        /// Init Engine
        /// </summary>
        override protected void InitEngine()
        {
            _engine = new EngineTop(this._session, this._dalLayer);
        }
        #endregion

        #region HasData
        /// <summary>
        /// Check if table has data
        /// </summary>
        /// <param name="tab">table</param>
        /// <returns>True if has data</returns>
        protected virtual bool HasData(object[,] tab)
        {
            if (FctIndicators.IsNull(tab) || tab.GetLongLength(0) == 1)
            {

                return false;
            }
            return true;
        }
        #endregion

        #region DataBuilding
        /// <summary>
        /// Dat building
        /// </summary>
        /// <param name="tab">table</param>
        /// <param name="series">series</param>
        /// <param name="fp">IFormatProvider</param>
        /// <param name="oneProductExist">test if one Product Exist</param>
        /// <param name="mixedElement">test if mixe dElement</param>
        /// <param name="competitorElement">test if competitor Element</param>
        /// <param name="referenceElement">test if reference Element</param>
        protected override void DataBuilding(object[,] tab, Series series, AdExpressCultureInfo fp, ref bool oneProductExist, ref bool mixedElement, ref bool competitorElement, ref bool referenceElement)
        {

            for (int i = 1; i < tab.GetLongLength(0) && i < 11; i++)
            {
                if (!FctIndicators.IsNull(tab[i, EngineTop.TOTAL_N]))
                {
                    //double d = Convert.ToDouble(tab[i, EngineTop.TOTAL_N], fp);
                    string u = FctIndicators.ConvertUnitValueToString(tab[i, EngineTop.TOTAL_N], _session.Unit, fp);
                    if (!string.IsNullOrEmpty(u))
                    {
                        oneProductExist = true;

                        series.Points.AddXY(tab[i, EngineTop.PRODUCT], FctIndicators.ConvertToDouble(u.Trim(), fp));

                        series.Points[i - 1].ShowInLegend = true;

                        if (!FctIndicators.IsNull(tab[i, EngineTop.COMPETITOR]))
                        {
                            // Mixed in yellow
                            if ((int)tab[i, EngineTop.COMPETITOR] == 3)
                            {
                                series.Points[i - 1].Color = (Color)_colorConverter.ConvertFrom(_mixedSerieColor);
                                mixedElement = true;
                            }
                            // Competitor in red
                            else if ((int)tab[i, EngineTop.COMPETITOR] == 2)
                            {
                                series.Points[i - 1].Color = (Color)_colorConverter.ConvertFrom(_competitorSerieColor);
                                competitorElement = true;
                            }
                            // Reference in green
                            else if ((int)tab[i, EngineTop.COMPETITOR] == 1)
                            {
                                series.Points[i - 1].Color = (Color)_colorConverter.ConvertFrom(_referenceSerieColor);
                                referenceElement = true;
                            }
                        }

                    }
                }
            }
            if (!oneProductExist)
                this.Visible = false;

        }
        #endregion

        #region SetAxeY
        /// <summary>
        /// Set Axe Y
        /// </summary>
        /// <param name="tab">table</param>
        /// <param name="selectedCurrency">selected Currency</param>
        /// <param name="strChartArea">string Chart Area</param>
        protected override void SetAxeY(object[,] tab, UnitInformation selectedCurrency, string strChartArea, IFormatProvider fp)
        {

            this.ChartAreas[strChartArea].AxisY.Enabled = AxisEnabled.True;
            this.ChartAreas[strChartArea].AxisY.LabelStyle.Enabled = true;
            this.ChartAreas[strChartArea].AxisY.LabelsAutoFit = false;

            this.ChartAreas[strChartArea].AxisY.LabelStyle.Font = new Font("Arial", (float)10);
            this.ChartAreas[strChartArea].AxisY.Title = "" + GestionWeb.GetWebWord(1246, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.DataLanguage) + ")";
            this.ChartAreas[strChartArea].AxisY.TitleFont = new Font("Arial", (float)10);

            if (!FctIndicators.IsNull(tab[0, EngineTop.TOTAL_N]))
            {
                string u = FctIndicators.ConvertUnitValueToString(tab[0, EngineTop.TOTAL_N], _session.Unit, fp);
                if (!string.IsNullOrEmpty(u))
                {

                    double uu = Convert.ToDouble(u.Trim(), fp);
                    if (uu > 0)
                    {
                        this.ChartAreas[strChartArea].AxisY.Maximum = uu;
                    }
                    else
                    {
                        this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
                    }
                }
                else this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;
            }
            else this.ChartAreas[strChartArea].AxisY.Maximum = (double)0.0;

            this.ChartAreas[strChartArea].AxisY.MajorGrid.LineWidth = 0;

        }
        #endregion

        #region GetUnit
        /// <summary>
        /// Get unit
        /// </summary>
        /// <returns>unit</returns>
        protected override UnitInformation GetUnit()
        {
            return _session.GetSelectedUnit();

        }
        #endregion
        


    }
}
