#region Info
/*
 * Author :     G Ragneau
 * Created on : 29/07/2008
 * History:
 *      Date - Author - Description
 *      29/07/2008 - G Ragneau - Moved from TNS.AdExpress.Web
 * 
 * 
 * Auteur: A. Obermeyer 
 * Date de création : 21/10/2004 
 * Date de modification : 21/10/2004 
 *      12/08/2005		G. Facon		Nom de fonction et suppression des propriétés
 *      24/10/2005	D. V. Mussuma	Intégration unité Keuros
 * */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;

using Dundas.Charting.WebControl;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using System.IO;
using Navigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.ProductClassIndicators.DAL;

namespace TNS.AdExpressI.ProductClassIndicators.Charts
{

    /// <summary>
    /// Chart for Product Class Indicator
    /// </summary>
    [ToolboxData("<{0}:ChartProductClassIndicator runat=server></{0}:ChartProductClassIndicator>")]
    public abstract class ChartProductClassIndicator : Chart
    {

        #region Variables
        /// <summary>
        /// Dal layer (should be ProductClassIndicatorsDAL)
        /// </summary>
        protected IProductClassIndicatorsDAL _dalLayer = null;
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session = null;
        /// <summary>
        /// Chart Image Type
        /// </summary>
        protected ChartImageType _chartType = ChartImageType.Flash;
        /// <summary>
        /// Classification level
        /// </summary>
        protected CstResult.MotherRecap.ElementType _classifLevel = CstResult.MotherRecap.ElementType.advertiser;
        /// <summary>
        /// Chart areas back color
        /// </summary>
        protected string _chartAreasBackColor = string.Empty;
        /// <summary>
        /// Border line color
        /// </summary>
        protected string _chartBorderLineColor = string.Empty;
        /// <summary>
        /// Series color
        /// </summary>
        protected string _seriesColor = string.Empty;
        /// <summary>
        /// Competitor serie color
        /// </summary>
        protected string _competitorSerieColor = string.Empty;
        /// <summary>
        /// Reference serie color
        /// </summary>
        protected string _referenceSerieColor = string.Empty;
		/// <summary>
		/// Mixed (reference and competiror) serie color
		/// </summary>
		protected string _mixedSerieColor = string.Empty;
        /// <summary>
        /// Legend item competitor color
        /// </summary>
        protected string _legendItemCompetitorColor = string.Empty;
        /// <summary>
        /// Legend item reference color
        /// </summary>
        protected string _legendItemReferenceColor = string.Empty;
		/// <summary>
		/// Legend item Mixed (reference and competiror) color
		/// </summary>
		protected string _legendItemMixedColor = string.Empty;
        /// <summary>
        /// Pie colors list
        /// </summary>
        protected string _pieColors = string.Empty;
        /// <summary>
        /// Pie line color
        /// </summary>
        protected string _pieLineColor = string.Empty;
        /// <summary>
        /// Title color
        /// </summary>
        protected string _titleColor = string.Empty;
        /// <summary>
        /// Tool to convert string as a color
        /// </summary>
        protected static ColorConverter _colorConverter = new ColorConverter();
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set User session
        /// </summary>
        public WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        /// <summary>
        /// Get / Set Chart rendering mode (flash, image...)
        /// </summary>
        public ChartImageType ChartType
        {
            get { return _chartType; }
            set { _chartType = value;  }
        }
        /// <summary>
        /// Get / Set Classification level to study (advertiser or product)
        /// </summary>
        public CstResult.MotherRecap.ElementType ClassifLevel
        {
            get { return _classifLevel; }
            set { _classifLevel = value; }
        }       /// <summary>
        /// Get / Set Chart areas back color
        /// </summary>
        public string ChartAreasBackColor
        {
            get { return _chartAreasBackColor; }
            set { _chartAreasBackColor = value; }
        }
        /// <summary>
        /// Get / Set Border line color
        /// </summary>
        public string ChartBorderLineColor
        {
            get { return _chartBorderLineColor; }
            set { _chartBorderLineColor = value; }
        }
        /// <summary>
        /// Get / Set Series color
        /// </summary>
        public string SeriesColor
        {
            get { return _seriesColor; }
            set { _seriesColor = value; }
        }
        /// <summary>
        /// Get / Set Competitor serie color
        /// </summary>
        public string CompetitorSerieColor
        {
            get { return _competitorSerieColor; }
            set { _competitorSerieColor = value; }
        }
        /// <summary>
        /// Get / Set Reference serie color
        /// </summary>
        public string ReferenceSerieColor
        {
            get { return _referenceSerieColor; }
            set { _referenceSerieColor = value; }
        }
		/// <summary>
		/// Get / Set mixed (reference and competiror) serie color
		/// </summary>
		public string MixedSerieColor {
			get { return _mixedSerieColor; }
			set { _mixedSerieColor = value; }
		}
        /// <summary>
        /// Get / Set Legend item competitor color
        /// </summary>
        public string LegendItemCompetitorColor
        {
            get { return _legendItemCompetitorColor; }
            set { _legendItemCompetitorColor = value; }
        }
        /// <summary>
        /// Get / Set Legend item reference color
        /// </summary>
        public string LegendItemReferenceColor
        {
            get { return _legendItemReferenceColor; }
            set { _legendItemReferenceColor = value; }
        }
		/// <summary>
		/// Get / Set Legend item  mixed (reference and competiror) color
		/// </summary>
		public string LegendItemMixedColor {
			get { return _legendItemMixedColor; }
			set { _legendItemMixedColor = value; }
		}
        /// <summary>
        /// Get / Set Pie colors list
        /// </summary>
        public string PieColors
        {
            get { return _pieColors; }
            set { _pieColors = value; }
        }
        /// <summary>
        /// Get / Set Pie line color
        /// </summary>
        public string PieLineColor
        {
            get { return _pieLineColor; }
            set { _pieLineColor = value; }
        }
        /// <summary>
        /// Get / Set Title color
        /// </summary>
        public string TitleColor
        {
            get { return _titleColor; }
            set { _titleColor = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User sessions</param>
        /// <param name="dalLayer">DAL Layer</param>
        public ChartProductClassIndicator(WebSession session, IProductClassIndicatorsDAL dalLayer)
        {
            this._session = session;
            this._dalLayer = dalLayer;
            this.ImageUrl = WebApplicationParameters.DundasConfiguration.ImageURL;
        }
        #endregion

        #region PreRender
        /// <summary>
        /// PreRender control
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {

            base.OnPreRender(e);

            this.AddHeader();

            #region Animation Params
            if (_chartType != ChartImageType.Flash)
            {
                this.ImageType = _chartType;
            }
            else
            {
                this.ImageType = ChartImageType.Jpeg;
                this.ImageType = ChartImageType.Flash;
                this.AnimationTheme = AnimationTheme.GrowingTogether;
                this.AnimationDuration = 5;
                this.RepeatAnimation = false;
            }
            #endregion

            #region Rendering Params
            this.BackGradientType = GradientType.TopBottom;
			this.BorderStyle=ChartDashStyle.Solid;
            this.BorderLineColor = (Color)_colorConverter.ConvertFrom(_chartBorderLineColor);
			this.BorderLineWidth=2;            
            #endregion

        }
        #endregion

        #region Render
        /// <summary>
        /// Overrided to add "param" tags for contextual menu managment
        /// </summary>
        /// <param name="writer">Writer</param>
        protected override void Render(HtmlTextWriter writer)
        {

            HtmlTextWriter txt = new HtmlTextWriter(new StringWriter());
            base.Render(txt);
            int i = -1;
            if ((i = txt.InnerWriter.ToString().IndexOf("<PARAM name=\"movie\"")) > -1)
            {
                writer.Write(txt.InnerWriter.ToString().Insert(i, "\r\n<PARAM name=\"wmode\" value=\"transparent\">\r\n<PARAM name=\"menu\" value=\"false\">\r\n"));
            }
            else
            {
                writer.Write(txt.InnerWriter.ToString());
            }
            writer.Write("<br/><br/>");

        }
        #endregion

        #region Add Header
        /// <summary>
        /// Add Header
        /// </summary>
        protected void AddHeader()
        {
            
            if (_chartType != ChartImageType.Flash)
            {
                Title title = new Title(GestionWeb.GetWebWord(2266, _session.SiteLanguage));
                title.Font = new Font("Arial", (float)8);
                title.DockInsideChartArea = false;
                title.Docking = Docking.Bottom;
                this.Titles.Add(title);

                this.BackImage = string.Format("/App_themes/{0}{1}", WebApplicationParameters.Themes[_session.SiteLanguage].Name, CstWeb.Images.LOGO_TNS_2);
                this.BackImageAlign = ChartImageAlign.TopLeft;
                this.BackImageMode = ChartImageWrapMode.Unscaled;

            }
        }
        #endregion


    }

}
