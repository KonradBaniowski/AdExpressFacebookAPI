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

		#region SetDesgin Old
		///// <summary>
		///// Design graphic
		///// </summary>
		//protected void SetDesignModeOld() {
		//    #region variables
		//    object[,] tab = null;
		//    DataTable dtFormat, dtColor, dtInsert, dtLocation;			
		//    //Séries de valeurs pour chaque tranche du graphique
		//    double[] yFormatValues = null;
		//    string[] xFormatValues = null;
		//    double[] yColorValues = null;
		//    string[] xColorValues = null;
		//    double[] yInsertValues = null;
		//    string[] xInsertValues = null;
		//    double[] yLocationValues = null;
		//    string[] xLocationValues = null;

		//    double[] yEurosValues = null;
		//    string[] xEurosValues = null;
		//    double[] ySpotValues = null;
		//    string[] xSpotValues = null;
		//    double[] yDurationValues = null;
		//    string[] xDurationValues = null;
		//    #endregion

		//    #region Graphics slice colors
		//    string[] pieColorsList = _pieColors.Split(',');
		//    Color[] pieColors = new Color[12];
		//    int indexPieColors = 0;
		//    ColorConverter ColorConverter = new ColorConverter();
		//    DataTable dt = null;

		//    foreach (string pieColorsString in pieColorsList) {
		//        pieColors.SetValue((Color)ColorConverter.ConvertFrom(pieColorsString), indexPieColors++);
		//    }			
		//    #endregion

		//    #region Set Graph values
		//    Domain.Web.Navigation.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
		//    if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
		//    object[] parameters = new object[1];
		//    parameters[0] = _webSession;
		//    Portofolio.IPortofolioResults portofolioResult = (Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
		//    dt = portofolioResult.GetStructureChartData();

		//    string idVehicle = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
		//    switch ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())) {
		//        case DBClassificationConstantes.Vehicles.names.press:
		//        case DBClassificationConstantes.Vehicles.names.internationalPress:
		//            //Data for press
		//            //portofolioResult.GetStructPressResult(out dtFormat, out dtColor, out dtLocation, out dtInsert);

		//            //if (dtFormat == null && dtColor == null && dtInsert == null && dtLocation == null) {
		//            if (dt==null || dt.Rows.Count==0) {
		//                //Pas de données à afficher
		//                this.Visible = false;
		//            }
		//            else {
		//                //Récupère les séries de valeurs pour le graphique
		//                //format
		//                if (dtFormat != null && dtFormat.Rows.Count > 0)
		//                    GetPressSeriesData(dtFormat, ref xFormatValues, ref yFormatValues, PortofolioStructure.Ventilation.format);
		//                //couleur
		//                if (dtColor != null && dtColor.Rows.Count > 0)
		//                    GetPressSeriesData(dtColor, ref xColorValues, ref yColorValues, PortofolioStructure.Ventilation.color);
		//                //Emplacements
		//                if (dtLocation != null && dtLocation.Rows.Count > 0)
		//                    GetPressSeriesData(dtLocation, ref xLocationValues, ref yLocationValues, PortofolioStructure.Ventilation.location);
		//                //Encarts
		//                if (dtInsert != null && dtInsert.Rows.Count > 0)
		//                    GetPressSeriesData(dtInsert, ref xInsertValues, ref yInsertValues, PortofolioStructure.Ventilation.insert);

		//                #region Création et définition du graphique
		//                //Création du graphique	
		//                ChartArea chartAreaFormat = null;
		//                Series serieFormat = new Series();
		//                if (dtFormat != null && dtFormat.Rows.Count > 0) {
		//                    //Conteneur graphique pour format
		//                    chartAreaFormat = new ChartArea();
		//                    //Alignement
		//                    chartAreaFormat.AlignOrientation = AreaAlignOrientation.Vertical;
		//                    chartAreaFormat.Position.X = 7;
		//                    chartAreaFormat.Position.Width = 80;

		//                    if (dtInsert != null && dtInsert.Rows.Count > 0) {
		//                        chartAreaFormat.Position.Y = (_typeFlash) ? 3 : 8;
		//                        chartAreaFormat.Position.Height = 22;
		//                    }
		//                    else {
		//                        chartAreaFormat.Position.Y = (_typeFlash) ? 3 : 8;
		//                        chartAreaFormat.Position.Height = 31;
		//                    }

		//                    this.ChartAreas.Add(chartAreaFormat);
		//                    //Charger les séries de valeurs pour format
		//                    serieFormat = SetSeriesForPress(dtFormat, chartAreaFormat, serieFormat, xFormatValues, yFormatValues, pieColors, GestionWeb.GetWebWord(1420, _webSession.SiteLanguage), _typeFlash, _pieLineColor);
		//                }
		//                //Conteneur graphique pour couleur
		//                ChartArea chartAreaColor = null;
		//                Series serieColor = new Series();
		//                if (dtColor != null && dtColor.Rows.Count > 0) {
		//                    chartAreaColor = new ChartArea();
		//                    //Alignement
		//                    chartAreaColor.AlignOrientation = AreaAlignOrientation.Vertical;
		//                    chartAreaColor.Position.X = 7;
		//                    chartAreaColor.Position.Width = 80;
		//                    if (dtInsert != null && dtInsert.Rows.Count > 0) {
		//                        chartAreaColor.Position.Y = (_typeFlash) ? 27 : 32;
		//                        chartAreaColor.Position.Height = 23;
		//                    }
		//                    else {
		//                        chartAreaColor.Position.Y = (_typeFlash) ? 36 : 41;
		//                        chartAreaColor.Position.Height = 31;
		//                    }
		//                    this.ChartAreas.Add(chartAreaColor);
		//                    //Charger les séries de valeurs pour couleur
		//                    serieColor = SetSeriesForPress(dtColor, chartAreaColor, serieColor, xColorValues, yColorValues, pieColors, GestionWeb.GetWebWord(1438, _webSession.SiteLanguage), _typeFlash, _pieLineColor);
		//                }
		//                //Conteneur graphique pour emplacements
		//                ChartArea chartAreaLocation = null;
		//                Series serieLocation = new Series();
		//                if (dtLocation != null && dtLocation.Rows.Count > 0) {
		//                    chartAreaLocation = new ChartArea();
		//                    //Alignement
		//                    chartAreaLocation.AlignOrientation = AreaAlignOrientation.Vertical;
		//                    chartAreaLocation.Position.X = 1;
		//                    chartAreaLocation.Position.Width = 95;
		//                    if (dtInsert != null && dtInsert.Rows.Count > 0) {
		//                        chartAreaLocation.Position.Y = (_typeFlash) ? 52 : 57;
		//                        chartAreaLocation.Position.Height = 23;
		//                    }
		//                    else {
		//                        chartAreaLocation.Position.Y = (_typeFlash) ? 69 : 74;
		//                        chartAreaLocation.Position.Height = 30;
		//                    }
		//                    this.ChartAreas.Add(chartAreaLocation);
		//                    //Charger les séries de valeurs pour emplacements
		//                    serieLocation = SetSeriesForPress(dtLocation, chartAreaLocation, serieLocation, xLocationValues, yLocationValues, pieColors, GestionWeb.GetWebWord(1439, _webSession.SiteLanguage), _typeFlash, _pieLineColor);

		//                }

		//                //Conteneur graphique pour encarts
		//                ChartArea chartAreaInsert = null;
		//                Series serieInsert = new Series();
		//                if (dtInsert != null && dtInsert.Rows.Count > 0) {
		//                    chartAreaInsert = new ChartArea();
		//                    //Alignement
		//                    chartAreaInsert.AlignOrientation = AreaAlignOrientation.Vertical;
		//                    chartAreaInsert.Position.X = 7;
		//                    chartAreaInsert.Position.Y = (_typeFlash) ? 77 : 78;
		//                    chartAreaInsert.Position.Width = 80;
		//                    chartAreaInsert.Position.Height = 22;
		//                    this.ChartAreas.Add(chartAreaInsert);
		//                    //Charger les séries de valeurs pour encarts
		//                    serieInsert = SetSeriesForPress(dtInsert, chartAreaInsert, serieInsert, xInsertValues, yInsertValues, pieColors, GestionWeb.GetWebWord(1440, _webSession.SiteLanguage), _typeFlash, _pieLineColor);
		//                }
		//                //initialisation du control
		//                InitializeComponentForPress(chartAreaFormat, chartAreaColor, chartAreaLocation, chartAreaInsert, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor);

		//                //ajout des séries de valeurs
		//                if (dtFormat != null && dtFormat.Rows.Count > 0)
		//                    this.Series.Add(serieFormat);
		//                if (dtColor != null && dtColor.Rows.Count > 0)
		//                    this.Series.Add(serieColor);
		//                if (dtLocation != null && dtLocation.Rows.Count > 0)
		//                    this.Series.Add(serieLocation);
		//                if (dtInsert != null && dtInsert.Rows.Count > 0)
		//                    this.Series.Add(serieInsert);

		//                #endregion
		//            }
		//            break;
		//        case DBClassificationConstantes.Vehicles.names.radio:
		//            //données pour radio
		//            tab = portofolioResult.GetStructRadio();
		//            if (tab != null && tab.GetLength(0) > 0) {
		//                GetEuroSeriesData(tab, ref xEurosValues, ref yEurosValues);
		//                GetSpotSeriesData(tab, ref xSpotValues, ref ySpotValues);
		//                GetDurationSeriesData(tab, ref xDurationValues, ref yDurationValues);
		//                #region  Création du graphique
		//                if (xEurosValues != null && yEurosValues != null && xSpotValues != null && ySpotValues != null && xDurationValues != null && yDurationValues != null) {
		//                    #region Création et définition du graphique
		//                    //Création du graphique																								
		//                    //Conteneur graphique pour Euros
		//                    ChartArea chartAreaEuros = new ChartArea();
		//                    Series serieEuros = new Series();
		//                    this.ChartAreas.Add(chartAreaEuros);
		//                    //Conteneur graphique pour Spot
		//                    ChartArea chartAreaSpot = new ChartArea();
		//                    Series serieSpot = new Series();
		//                    this.ChartAreas.Add(chartAreaSpot);
		//                    //Conteneur graphique pour durée
		//                    ChartArea chartAreaDuration = new ChartArea();
		//                    Series serieDuration = new Series();
		//                    this.ChartAreas.Add(chartAreaDuration);


		//                    //Charger les séries de valeurs pour euros 
		//                    serieEuros = SetSeries(tab, chartAreaEuros, serieEuros, xEurosValues, yEurosValues, pieColors, GestionWeb.GetWebWord(1423, _webSession.SiteLanguage), _typeFlash, _pieLineColor);

		//                    //Charger les séries de valeurs pour spot
		//                    serieSpot = SetSeries(tab, chartAreaSpot, serieSpot, xSpotValues, ySpotValues, pieColors, GestionWeb.GetWebWord(869, _webSession.SiteLanguage), _typeFlash, _pieLineColor);

		//                    //Charger les séries de valeurs pour durée
		//                    serieDuration = SetSeries(tab, chartAreaDuration, serieDuration, xDurationValues, yDurationValues, pieColors, GestionWeb.GetWebWord(280, _webSession.SiteLanguage), _typeFlash, _pieLineColor);
		//                    //initialisation du control
		//                    InitializeComponent(chartAreaEuros, chartAreaSpot, chartAreaDuration, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor);

		//                    //ajout des séries de valeurs
		//                    this.Series.Add(serieEuros);
		//                    this.Series.Add(serieSpot);
		//                    this.Series.Add(serieDuration);

		//                    #endregion
		//                }
		//                #endregion
		//            }
		//            else {
		//                this.Visible = false;
		//            }
		//            break;
		//        case DBClassificationConstantes.Vehicles.names.tv:
		//        case DBClassificationConstantes.Vehicles.names.others:
		//            //données pour tv
		//            tab = portofolioResult.GetStructTV();
		//            if (tab != null && tab.GetLength(0) > 0) {
		//                GetEuroSeriesData(tab, ref xEurosValues, ref yEurosValues);
		//                GetSpotSeriesData(tab, ref xSpotValues, ref ySpotValues);
		//                GetDurationSeriesData(tab, ref xDurationValues, ref yDurationValues);
		//                #region  Création du graphique
		//                if (xEurosValues != null && yEurosValues != null && xSpotValues != null && ySpotValues != null && xDurationValues != null && yDurationValues != null) {
		//                    #region Création et définition du graphique
		//                    //Création du graphique																								
		//                    //Conteneur graphique pour Euros
		//                    ChartArea chartAreaEuros = new ChartArea();
		//                    Series serieEuros = new Series();
		//                    this.ChartAreas.Add(chartAreaEuros);
		//                    //Conteneur graphique pour Spot
		//                    ChartArea chartAreaSpot = new ChartArea();
		//                    Series serieSpot = new Series();
		//                    this.ChartAreas.Add(chartAreaSpot);
		//                    //Conteneur graphique pour durée
		//                    ChartArea chartAreaDuration = new ChartArea();
		//                    Series serieDuration = new Series();
		//                    this.ChartAreas.Add(chartAreaDuration);

		//                    //Charger les séries de valeurs pour euros 
		//                    serieEuros = SetSeries(tab, chartAreaEuros, serieEuros, xEurosValues, yEurosValues, pieColors, GestionWeb.GetWebWord(1423, _webSession.SiteLanguage), _typeFlash, _pieLineColor);

		//                    //Charger les séries de valeurs pour spot
		//                    serieSpot = SetSeries(tab, chartAreaSpot, serieSpot, xSpotValues, ySpotValues, pieColors, GestionWeb.GetWebWord(869, _webSession.SiteLanguage), _typeFlash, _pieLineColor);

		//                    //Charger les séries de valeurs pour durée
		//                    serieDuration = SetSeries(tab, chartAreaDuration, serieDuration, xDurationValues, yDurationValues, pieColors, GestionWeb.GetWebWord(280, _webSession.SiteLanguage), _typeFlash, _pieLineColor);

		//                    //initialisation du control
		//                    InitializeComponent(chartAreaEuros, chartAreaSpot, chartAreaDuration, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor);

		//                    //ajout des séries de valeurs
		//                    this.Series.Add(serieEuros);
		//                    this.Series.Add(serieSpot);
		//                    this.Series.Add(serieDuration);

		//                    #endregion
		//                }
		//                #endregion
		//            }
		//            else {
		//                this.Visible = false;
		//            }
		//            break;
		//        default:
		//            throw new Exceptions.PortofolioChartWebControlException("Vehicle unknown.");
		//    }

		//    #endregion

		//}
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
			
			switch ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())) {
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:						
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
				case DBClassificationConstantes.Vehicles.names.radio: 
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
									if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.press
										|| (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.internationalPress)

                                        // BUG A DEDE
                                        InitializeComponent(chartArea, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor, indexTitle);
										//InitializeComponentForPress(chartArea, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor, indexTitle);
									else
										InitializeComponent(chartArea, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor, indexTitle);
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
								if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.press
										|| (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.internationalPress)
                                    // BUG A DEDE
                                    InitializeComponent(chartArea, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor, indexTitle);//TODO chek for all vehicle
                                    //InitializeComponentForPress(chartArea, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor, indexTitle);
								else InitializeComponent(chartArea, _typeFlash, _webSession, _defaultBorderLineColor, _chartBorderLineColor, _titleColor, indexTitle);//TODO chek for all vehicle
								if (!_typeFlash) {
									//CopyRight
									Title title = new Title("" + GestionWeb.GetWebWord(2266, _webSession.SiteLanguage) + "");
									title.Font = new Font("Arial", (float)8);
									title.DockInsideChartArea = false;
									title.Docking = Docking.Bottom;
									this.Titles.Add(title);
								}
								this.Series.Add(series);
								this.ChartAreas.Add(chartArea);
							}
						}
					}
					
					break;
				default:
					throw new Exceptions.PortofolioChartWebControlException("Vehicle unknown.");
			}

			#endregion

		}
		#endregion

		#region Set series (New version)
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
				//Création du graphique							

				//Chart type 
				series.Type = SeriesChartType.Pie;

				series.XValueType = ChartValueTypes.String;
				series.YValueType = ChartValueTypes.Double;
				series.Enabled = true;


				chartArea.Area3DStyle.Enable3D = true;
				chartArea.Name = chartAreaName;
				series.ChartArea = chartArea.Name;
				series.Points.DataBindXY(xValues, yValues);

				#region Définition des couleurs
				//Chart colors
				for (int k = 0; (pieColors!=null && pieColors.Length>0 && k < xValues.Length - 1 && k < pieColors.Length - 1); k++) {
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

				#endregion

			}
			#endregion

			//return series;
		}
		#endregion

		#region Initialize component (New version)
		/// <summary>
		/// Initialise les styles du webcontrol pour média radio et télé
		/// </summary>		
		/// <param name="typeFlash">sortie flash</param>
		/// <param name="webSession">Session client</param>
		/// <param name="defaultBorderLineColor">Default chart border color</param>
		/// <param name="borderLineColor">Chart border color</param>
		/// <param name="title">Title color</param>
		private void InitializeComponent(ChartArea chartArea, bool typeFlash, WebSession webSession, string defaultBorderLineColor, string borderLineColor, string titleColor,int indexTitle) {

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
				this.BackImage = WebConstantes.Images.LOGO_TNS_2;
				this.BackImageAlign = ChartImageAlign.TopLeft;
				this.BackImageMode = ChartImageWrapMode.Unscaled;
			}
			#endregion

			#region Color Converter
			ColorConverter ColorConverter = new ColorConverter();
			#endregion

			#region Chart
			this.Width = new Unit("700px");
			this.Height = new Unit("800px");
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = (Color)ColorConverter.ConvertFrom(defaultBorderLineColor);
			this.BorderStyle = ChartDashStyle.Solid;
			this.BorderLineColor = (Color)ColorConverter.ConvertFrom(borderLineColor);
			this.BorderLineWidth = 2;
			this.Legend.Enabled = false;
			#endregion

			#region Titre
			//title euros
			this.Titles.Add(chartArea.Name);
			this.Titles[indexTitle].DockInsideChartArea = true;
			this.Titles[indexTitle].Position.Auto = false;
			this.Titles[indexTitle].Position.X = 50;
			this.Titles[indexTitle].Font = new Font("Arial", (float)13);
			this.Titles[indexTitle].Color = (Color)ColorConverter.ConvertFrom(titleColor);
			this.Titles[indexTitle].DockToChartArea = chartArea.Name;

			if (indexTitle == 0) {
				this.Titles[indexTitle].Position.Y = (typeFlash) ? 2 : 7;								
				if (!typeFlash) {
					chartArea.Position.X = 22;
					chartArea.Position.Width = 60;
					chartArea.Position.Y = 8;
					chartArea.Position.Height = 22;
				}
			}
			//title spot	
			if (indexTitle == 1) {
				this.Titles[indexTitle].Position.Y = (typeFlash) ? 34 : 38;				
				if (!typeFlash) {
					chartArea.Position.X = 22;
					chartArea.Position.Width = 60;
					chartArea.Position.Y = 40;
					chartArea.Position.Height = 22;
				}
			}
			//title duration
			if (indexTitle == 2) {
				this.Titles[indexTitle].Position.Y = 66;				
				if (!typeFlash) {
					chartArea.Position.X = 22;
					chartArea.Position.Width = 60;
					chartArea.Position.Y = 68;
					chartArea.Position.Height = 22;
				}
			}
			#endregion

			//if (!typeFlash) {
			//    //CopyRight
			//    Title title = new Title("" + GestionWeb.GetWebWord(2266, webSession.SiteLanguage) + "");
			//    title.Font = new Font("Arial", (float)8);
			//    title.DockInsideChartArea = false;
			//    title.Docking = Docking.Bottom;
			//    this.Titles.Add(title);
			//}

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

		/// <summary>
		/// Overrided to add "param" tags for contextual menu managment
		/// </summary>
		public string GetHtmlRender() {
			SetDesignMode();

			HtmlTextWriter txt = new HtmlTextWriter(new StringWriter());
			base.Render(txt);
			int i = -1;
			if ((i = txt.InnerWriter.ToString().IndexOf("<PARAM name=\"movie\"")) > -1) {
				return txt.InnerWriter.ToString().Insert(i, "\r\n<PARAM name=\"wmode\" value=\"transparent\">\r\n<PARAM name=\"menu\" value=\"false\">\r\n");
			}
			else {
				return txt.InnerWriter.ToString();
			}
		}
		#endregion
	}
}
