#region Informations
// Auteur: D. V. MUSSUMA
// Date de création: 18/07/2008
// Date de modification: 

#endregion
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Drawing;
using System.Web.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;

using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Constantes.FrameWork.Results;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;

using WebConstantes = TNS.AdExpress.Constantes.Web;
using Portofolio = TNS.AdExpressI.Portofolio;
using Domain = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;
using System.Reflection;

using Dundas.Charting.WebControl;

namespace TNS.AdExpress.Web.Controls.Results {
	/// <summary>
	/// Generate chart of poertofolio results
	/// </summary>
	[ToolboxData("<{0}:PortofolioChartWebControl runat=server></{0}:PortofolioChartWebControl>")]
	public class PortofolioChartWebControl : Chart {
		
		#region Variables
		/// <summary>
		/// Pie colors list
		/// </summary>
		private string _pieColors = string.Empty;
		/// <summary>
		/// Pie line color
		/// </summary>
		private string _pieLineColor = string.Empty;
		/// <summary>
		/// Default border line color
		/// </summary>
		private string _defaultBorderLineColor = string.Empty;
		/// <summary>
		/// Chart border line color
		/// </summary>
		private string _chartBorderLineColor = string.Empty;
		/// <summary>
		/// Title color
		/// </summary>
		private string _titleColor = string.Empty;
		/// <summary>
		/// Session client
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Graph type
		/// </summary>
		private bool _typeFlash = false;
		#endregion

		#region Accessors
		/// <summary>
		/// Get or Set Pie colors list
		/// </summary>
		public string PieColors {
			get { return _pieColors; }
			set { _pieColors = value; }
		}
		/// <summary>
		/// Get or Set Pie line color
		/// </summary>
		public string PieLineColor {
			get { return _pieLineColor; }
			set { _pieLineColor = value; }
		}
		/// <summary>
		/// Get or Set Default border line color
		/// </summary>
		public string DefaultBorderLineColor {
			get { return _defaultBorderLineColor; }
			set { _defaultBorderLineColor = value; }
		}
		/// <summary>
		/// Get or Set Chart border line color
		/// </summary>
		public string ChartBorderLineColor {
			get { return _chartBorderLineColor; }
			set { _chartBorderLineColor = value; }
		}
		/// <summary>
		/// Get or Set Title color
		/// </summary>
		public string TitleColor {
			get { return _titleColor; }
			set { _titleColor = value; }
		}
		/// <summary>
		/// Get or Set client web session 
		/// </summary>
		public WebSession CustomerWebSession {
			get { return _webSession; }
			set { _webSession = value; }
		}

		/// <summary>
		/// Get or Set graph type
		/// </summary>
		public bool TypeFlash {
			get { return _typeFlash; }
			set { _typeFlash = value; }
		}
		#endregion				

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PortofolioChartWebControl() {
            this.ImageUrl = WebApplicationParameters.DundasConfiguration.ImageURL;
        }
        #endregion

        #region SetDesignMode
        /// <summary>
		/// Design graphic
		/// </summary>
		protected virtual void SetDesignMode() {
			
			#region Variables
			ChartArea chartArea = null;
			Series series = null;
			double[] yValues = null;
			string[] xValues = null;
			Dictionary<long, int> valuesListSize = new Dictionary<long, int>();
			long oldUnitId = -1;
			int nbItem = 0;
			bool start = true;
			int currentLine =0, indexTitle=0;
			string oldUnitLabel = "";
			
			#endregion

			#region Graphics slice colors
			string[] pieColorsList = _pieColors.Split(',');
			Color[] pieColors = new Color[12];
			int indexPieColors = 0;
			ColorConverter ColorConverter = new ColorConverter();
			DataTable dt = null;
			float posY = (_typeFlash) ? 4 : 7;
			foreach (string pieColorsString in pieColorsList) {
				pieColors.SetValue((Color)ColorConverter.ConvertFrom(pieColorsString), indexPieColors++);
			}
			#endregion

			#region Set Graph values

			Domain.Web.Navigation.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
			if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
			object[] parameters = new object[1];
			parameters[0] = _webSession;
			Portofolio.IPortofolioResults portofolioResult = (Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			dt = portofolioResult.GetStructureChartData();

			string idVehicle = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);					
					if (dt == null || dt.Rows.Count == 0) {
						//No data to show
						this.Visible = false;
					}
					else {
						foreach (DataRow dr in dt.Rows) {
							if (oldUnitId != long.Parse(dr["idUnit"].ToString()) && !start) {
								valuesListSize.Add(oldUnitId, nbItem);
								nbItem = 0;
							}
							nbItem++;
							start = false;
							oldUnitId = long.Parse(dr["idUnit"].ToString());
						}
						valuesListSize.Add(oldUnitId, nbItem);
						
						//Set chart values
						start = true;
						oldUnitId = -1;
						foreach (DataRow dr in dt.Rows) {
							if (oldUnitId != long.Parse(dr["idUnit"].ToString()) && !start) {								
								if (xValues != null && xValues.Length > 0 && yValues != null && yValues.Length > 0) {
									SetSeries(chartArea, series, xValues, yValues, pieColors, oldUnitLabel, _typeFlash, _pieLineColor);
									InitializeComponent(chartArea, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor, indexTitle, nbItem,ref posY);
									this.Series.Add(series);
									this.ChartAreas.Add(chartArea);
									indexTitle++;
								}
							}
							if (oldUnitId != long.Parse(dr["idUnit"].ToString())) {
								chartArea = new ChartArea();
								series = new Series();
								xValues = new string[valuesListSize[long.Parse(dr["idUnit"].ToString())]];
								yValues = new double[valuesListSize[long.Parse(dr["idUnit"].ToString())]];
								currentLine = 0;
							}
						
							xValues[currentLine] = dr["chartDataLabel"].ToString();
							yValues[currentLine] = double.Parse(dr["chartDataValue"].ToString());
							oldUnitLabel = dr["unitLabel"].ToString();
							oldUnitId = long.Parse(dr["idUnit"].ToString());
							currentLine++;							
							start = false;
						}
						if (!start) {
							if (xValues != null && xValues.Length > 0 && yValues != null && yValues.Length > 0) {
								SetSeries(chartArea, series, xValues, yValues, pieColors, oldUnitLabel, _typeFlash, _pieLineColor);
								InitializeComponent(chartArea, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor, indexTitle, nbItem,ref posY);
								if (!_typeFlash) {
									//CopyRight
									Title title = new Title("" + GestionWeb.GetWebWord(2266, _webSession.SiteLanguage) + "");
									title.Font = new Font("Arial", (float)8);
									title.DockInsideChartArea = false;
									title.Docking = Docking.Bottom;
									title.Position.Y = 98;
									title.Position.X = 50;
									this.Titles.Add(title);
								}
								this.Series.Add(series);
								this.ChartAreas.Add(chartArea);
							}
						}
					}
								
			#endregion

		}
		#endregion

		#region Set series 
		/// <summary>
		/// Crétion du graphique euros,spot ou  durée pour média radio et télé
		/// </summary>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">séries de valeurs</param>
		/// <param name="xValues">séries de libellés</param>
		/// <param name="yValues">séries de valeurs</param>
		/// <param name="pieColors">couleurs du graphique</param>
		/// <param name="typeFlash">sortie flash</param>
		/// <param name="chartAreaName">nom du conteneur de l'image</param>
		/// <param name="pieLineColor">Pie line color</param>
		/// <returns>séries de valeurs</returns>
		private static void SetSeries(ChartArea chartArea, Dundas.Charting.WebControl.Series series, string[] xValues, double[] yValues, Color[] pieColors, string chartAreaName, bool typeFlash, string pieLineColor) {
			#region  Création graphique
			if (xValues != null && yValues != null) {

				#region Creation and definition of the chart

				//Chart type 
				series.Type = SeriesChartType.Pie;

				series.XValueType = ChartValueTypes.String;
				series.YValueType = ChartValueTypes.Double;
				series.Enabled = true;

				chartArea.Area3DStyle.Enable3D = true;
				chartArea.Name = chartAreaName;
				series.ChartArea = chartArea.Name;
				series.Points.DataBindXY(xValues, yValues);

				#region Chart Color definition
				//Chart colors
				for (int k = 0; (pieColors != null && pieColors.Length > 0 && k < series.Points.Count && k < pieColors.Length - 1); k++) {
					series.Points[k].Color = pieColors[k];
				}
				#endregion

				#region Legend
				series["LabelStyle"] = "Outside";
				series.LegendToolTip = "#PERCENT";
				series.ToolTip = "#PERCENT : #VALX ";
				series["PieLineColor"] = pieLineColor;
				#endregion

				series.Label = "#PERCENT : #VALX";
				series["3DLabelLineSize"] = "30";
				series.Font = new Font("Arial", (float)7);


				#endregion

			}
			#endregion

		}
		#endregion

		#region Initialize component 
		
		
		/// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>		
		/// <param name="typeFlash">sortie flash</param>
		/// <param name="webSession">Session client</param>
		/// <param name="defaultBorderLineColor">Default chart border color</param>
		/// <param name="borderLineColor">Chart border color</param>
		/// <param name="indexTitle">index Title </param>
		/// <param name="chartArea">Chart area</param>
		/// <param name="titleColor">Titke color</param>
		private void InitializeComponent(ChartArea chartArea, bool typeFlash, WebSession webSession, string defaultBorderLineColor, string borderLineColor, string titleColor, int indexTitle, int nbItem ,ref float posY) {
			#region Variables
			float imgHeight = 21;			
			float spacer = 4;
			#endregion

			#region Animation Flash
			//Animation flash
			if (typeFlash) {
				this.ImageType = ChartImageType.Flash;
				this.AnimationTheme = AnimationTheme.MovingFromTop;
				this.AnimationDuration = 3;
				this.RepeatAnimation = false;
			}
			else {
				this.ImageType = ChartImageType.Jpeg;
				this.BackImage = "/App_Themes/" + WebApplicationParameters.Themes[webSession.SiteLanguage].Name + WebConstantes.Images.LOGO_TNS_2;
				this.BackImageAlign = ChartImageAlign.TopLeft;
				this.BackImageMode = ChartImageWrapMode.Unscaled;
			}
			#endregion

			#region Color Converter
			ColorConverter ColorConverter = new ColorConverter();
			#endregion

			#region Chart
			if (nbItem > 3) {
				this.Width = new Unit("800px");
				this.Height = new Unit("1000px");
			}
			else {
				this.Width = new Unit("700px");
				this.Height = new Unit("800px");
			}
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = (Color)ColorConverter.ConvertFrom(defaultBorderLineColor);
			this.BorderStyle = ChartDashStyle.Solid;
			this.BorderLineColor = (Color)ColorConverter.ConvertFrom(borderLineColor);
			this.BorderLineWidth = 2;
			this.Legend.Enabled = false;
			#endregion

			#region Titles
			//title euros
			this.Titles.Add(chartArea.Name);
			this.Titles[indexTitle].DockInsideChartArea = true;
			this.Titles[indexTitle].Position.Auto = false;
			this.Titles[indexTitle].Position.X = 50;
			this.Titles[indexTitle].Font = new Font("Arial", (float)12);
			this.Titles[indexTitle].Color = (Color)ColorConverter.ConvertFrom(titleColor);
			this.Titles[indexTitle].DockToChartArea = chartArea.Name;
			chartArea.AlignOrientation = AreaAlignOrientation.Vertical;
			
			chartArea.Position.X = 22;
			chartArea.Position.Width = 60;
			chartArea.Position.Height = imgHeight;
				
			this.Titles[indexTitle].Position.Y = posY - 2;
			chartArea.Position.Y = posY;
			posY = posY + imgHeight + spacer;
			
			#endregion

		}
		#endregion		

		#region Render
		/// <summary>
		/// Overrided to add "param" tags for contextual menu managment
		/// </summary>
		/// <param name="writer">Writer</param>
		protected override void Render(HtmlTextWriter writer) {
			SetDesignMode();

			HtmlTextWriter txt = new HtmlTextWriter(new StringWriter());
			base.Render(txt);
			int i = -1;
			if ((i = txt.InnerWriter.ToString().IndexOf("<PARAM name=\"movie\"")) > -1) {
				writer.Write(txt.InnerWriter.ToString().Insert(i, "\r\n<PARAM name=\"wmode\" value=\"transparent\">\r\n<PARAM name=\"menu\" value=\"false\">\r\n"));
			}
			else {
				writer.Write(txt.InnerWriter.ToString());
			}
		}		
		#endregion
	}
}
