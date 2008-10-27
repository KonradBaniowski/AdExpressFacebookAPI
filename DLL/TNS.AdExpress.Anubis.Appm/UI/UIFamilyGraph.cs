#region Information
/*
 * Author : G. Ragneau
 * Creation : 30/08/2005
 * Modification:
 *		
 * */
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

//using TNS.AdExpress.Common;

using TNS.AdExpress.Constantes.Web;
using CstUI = TNS.AdExpress.Constantes.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

using TNS.AdExpress.Anubis.Appm.Common;
using TNS.AdExpress.Anubis.Appm.Exceptions;

using Dundas.Charting.WinControl;
using TNS.FrameWork.DB.Common;
using System.Collections.Generic;


namespace TNS.AdExpress.Anubis.Appm.UI {
	/// <summary>
	/// UIFamilyGraph : Methods to build graphs in the family plan
	/// </summary>
	public class UIFamilyGraph : Chart {

		#region Attributes
		/// <summary>
		/// User Session
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Data Source
		/// </summary>
		private IDataSource _dataSource = null;
		/// <summary>
		/// APPM configuration
		/// </summary>
		private AppmConfig _config = null;
		/// <summary>
		/// Data reserved to the calcul of PDVs
		/// </summary>
		private DataTable _dtGraphicsData = null;
        /// <summary>
        /// Style
        /// </summary>
        private TNS.AdExpress.Domain.Theme.Style _style = null;
        /// <summary>
        /// Pie ColorS
        /// </summary>
        private List<Color> _pieColors = null;
        /// <summary>
        /// Bar Colors
        /// </summary>
        private List<Color> _barColors = null;
		#endregion

		#region Constructeur
        public UIFamilyGraph(WebSession webSession, IDataSource dataSource, AppmConfig config, DataTable dtGraphicsData, TNS.AdExpress.Domain.Theme.Style style)
            : base() {
			_webSession = webSession;
			_dataSource = dataSource;
			_config = config;
			_dtGraphicsData = dtGraphicsData;
            _style = style;

            _pieColors = ((TNS.AdExpress.Domain.Theme.Colors)_style.GetTag("FamilyGraphPieColors")).ColorList;
            _barColors = ((TNS.AdExpress.Domain.Theme.Colors)_style.GetTag("FamilyGraphBarColors")).ColorList;
		}
		#endregion

		#region GRP
		/// <summary>
		/// Graphiques Family plan GRP for the base target
		/// </summary>
		/// <param name="cible">Target to process</param>
		internal void BuildGRP(string cible){

			string[] xUnitValues = null;
			double[] yUnitValues = null;
			ChartArea cArea = null;
			Series sData = null;

			try{

				if(_dtGraphicsData.Rows.Count>0){

					#region Graph Properties
                    _style.GetTag("FamilyGraphSize").SetStyleDundas(this);
					this.BackGradientType = GradientType.TopBottom;
                    _style.GetTag("FamilyGraphLineEnCircle").SetStyleDundas(this);

					cArea = new ChartArea();
					cArea.Name = GestionWeb.GetWebWord(1736,_webSession.SiteLanguage)
						+ " "
						+ " GRP "
						+ " ("+_dtGraphicsData.Rows[0][cible]+") "
						+ GestionWeb.GetWebWord(1737,_webSession.SiteLanguage);
					this.ChartAreas.Add(cArea);
					cArea.Area3DStyle.Enable3D = true;


					//positions
					cArea.Position.X = 1;
					cArea.Position.Y = 30;
					cArea.Position.Height = 98;
					cArea.Position.Width = 98;
					#endregion

					#region series for GRP
					sData = new Series();	
					sData.Enabled = true;
					sData.Type=SeriesChartType.Pie;
					sData.ChartArea=cArea.Name;
					#endregion

					#region data
					yUnitValues=new double[_dtGraphicsData.Rows.Count+1];
					xUnitValues=new string[_dtGraphicsData.Rows.Count+1];
					if (cible == "baseTarget"){
						getSeriesDataBase(_dtGraphicsData,ref xUnitValues,ref yUnitValues);
					}
					else{
						getSeriesDataAdditional(_dtGraphicsData,ref xUnitValues,ref yUnitValues);
					}
					sData.XValueType=ChartValueTypes.String;
					sData.YValueType=ChartValueTypes.Long;								
					sData.Points.DataBindXY(xUnitValues,yUnitValues);
					#endregion

					#region Defining Colour
					for(int k=0 ; k<sData.Points.Count ; k++){
						if(k < _pieColors.Count){
							sData.Points[k].Color = _pieColors[k];
						}
						sData.Points[k]["Exploded"] = "true";
					}
					#endregion

					#region Labels and Tooltips
					sData["LabelStyle"]="Outside";
					sData.Label="#PERCENT #VALX" ;
					sData["PieLineColor"]="Black";
					sData.ShowInLegend = false;
                    _style.GetTag("FamilyGraphDefaultFont").SetStyleDundas(sData);
					this.Titles.Add(cArea.Name);
                    _style.GetTag("FamilyGraphDefaultTitleFont").SetStyleDundas(this.Titles[0]);

					#endregion


					this.Series.Add(sData);
				}

			}
			catch(System.Exception err){				
				throw(new UIPDVPlanChartException("Unable to build pies for Family Plan.",err));
			}

		}
		#endregion


		#region CGRP (Non Utilisé)
        /*
		internal void BuildBars(){
		
			#region variable
			ChartArea cArea = null;
			Series sData = null;
			Series sTargetData = null;
			//Séries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			string[]  xUnitValuesTarget = null;
			double[]  yUnitValuesTarget  = null;
			double maxScale=0;
			#endregion

			try {

				if(_dtGraphicsData!=null && _dtGraphicsData.Rows.Count>0){

					this.Size = new Size(600,700);
					//Graph Properties
					this.BackGradientType = GradientType.TopBottom;
					this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);											
					this.BorderStyle=ChartDashStyle.Solid;
					this.BorderLineColor=Color.FromArgb(99,73,132);
					this.BorderLineWidth=1;

					cArea = new ChartArea();

					this.ChartAreas.Add(cArea);

					cArea.Area3DStyle.Enable3D = false;
					cArea.BackColor =Color.FromArgb(222,207,231);
                    _style.GetTag("defaultFont").SetStyleDundas(cArea.AxisY.LabelStyle);
					
					//name
					cArea.Name = GestionWeb.GetWebWord(1685,_webSession.SiteLanguage)
						+ " " + GestionWeb.GetWebWord(1738,_webSession.SiteLanguage) + " :"
						+ _dtGraphicsData.Rows[0]["baseTarget"]
						+ " " + GestionWeb.GetWebWord(1739,_webSession.SiteLanguage) + " "
						+ _dtGraphicsData.Rows[0]["additionalTarget"]
						;

					//positions
					cArea.Position.X = 1;
					cArea.Position.Y = 10;
					cArea.Position.Height = 80;
					cArea.Position.Width = 80;

					//Series creation
					sData = new Series();
					sTargetData = new Series();

					sData.ChartArea = cArea.Name;
					sTargetData.ChartArea = cArea.Name;

					sTargetData.Type = sData.Type = SeriesChartType.Bar;
					sTargetData.XValueType = sData.XValueType = ChartValueTypes.String;
					sTargetData.YValueType = sData.YValueType = ChartValueTypes.Double;								
					sTargetData.Enabled = sData.Enabled=true;
					
					getSeriesDataCgrp(_dtGraphicsData,ref xUnitValues,ref yUnitValues,ref xUnitValuesTarget,ref yUnitValuesTarget,ref maxScale);
					sData.Points.DataBindXY(xUnitValues, yUnitValues);
					sTargetData.Points.DataBindXY(xUnitValuesTarget, yUnitValuesTarget);
					cArea.AxisY.Maximum= maxScale+1000;

					for(int k=0 ; k < xUnitValues.Length ; k++){
						sData.Points[k].Color = _barColors[0];
						sTargetData.Points[k].Color = _barColors[1];
					}
					sTargetData["LabelStyle"] = sData["LabelStyle"] = "Outside";
					sTargetData["PointWidth"] = sData["PointWidth"] = "1.0";
					sTargetData.Label = sData.Label="#VALY";
					cArea.AxisX.Interval=1;
					cArea.AxisX.Margin=true;
					cArea.AxisX.LabelStyle.ShowEndLabels = true;
	


					//common legend
					sTargetData.ShowInLegend = false;
					sData.ShowInLegend = false;

					Legend l = new Legend(cArea.Name);
					l.Enabled=true;
					l.Font = cArea.AxisY.LabelStyle.Font;
					l.FontColor = cArea.AxisY.LabelStyle.FontColor;
					l.Docking = LegendDocking.Bottom;
					l.Alignment = StringAlignment.Center;
					l.LegendStyle = LegendStyle.Row;
					l.CustomItems.Add(_barColors[0],GestionWeb.GetWebWord(1685,_webSession.SiteLanguage) + " ("+_dtGraphicsData.Rows[0]["baseTarget"]+") ");
					l.CustomItems.Add(_barColors[1],GestionWeb.GetWebWord(1685,_webSession.SiteLanguage) + " ("+_dtGraphicsData.Rows[0]["additionalTarget"]+") ");


					this.Legends.Add(l);

					//Series Font
					sTargetData.Font = sData.Font = l.Font;
					sTargetData.FontColor = sData.FontColor = l.FontColor;

					//titles
					this.Titles.Add(cArea.Name);
                    _style.GetTag("defaultTitleFont").SetStyleDundas(this.Titles[0]);

					//Add Series
					this.Series.Add(sData);
					this.Series.Add(sTargetData);

				}
			}
			catch(System.Exception err){				
				throw(new UIPeriodicityChartException("Unable to build bars for periodicity.",err));
			}
		}*/
		#endregion
        
		#region méthodes privées
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible de base à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private static void getSeriesDataBase(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			//string ventilationType="distributionBase";
			int x=0; 
			int y=0;
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];
			string[] zValues = new string[dt.Rows.Count];				
			for(int i=1; i<dt.Rows.Count-1;i++){
				xValues[x]=dt.Rows[i]["InterestFamily"].ToString();
				yValues[y]=double.Parse(dt.Rows[i]["distributionBase"].ToString());
				x++;
				y++;
			}
			#endregion
		}

		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private static void getSeriesDataAdditional(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			//string ventilationType="distributionSelected";
			int x=0;
			int y=0;
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];			
			for(int i=1; i<dt.Rows.Count-1;i++){
				xValues[x]=dt.Rows[i]["InterestFamily"].ToString();
				yValues[y]=double.Parse(dt.Rows[i]["distributionSelected"].ToString());
				x++;
				y++;
			}
			#endregion
		}

		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValuesBase">libellés du graphique</param>
		/// <param name="yValuesBase">valeurs pour graphique</param>
		/// <param name="xValuesSelected">Liebllés sélectionnée</param>
		/// <param name="yValuesSelected">Valeur sélectionnée</param>
		/// <param name="maxScale">Echelle de l'axe X du graphe</param>
		private static void getSeriesDataCgrp(DataTable dt,ref string[] xValuesBase,ref double[] yValuesBase,ref string[] xValuesSelected,ref double[] yValuesSelected,ref double maxScale){
	
			#region Variable
			//string ventilationTypeBase="cgrpBase";
			//string ventilationTypeSelected="cgrpSelected";
			int x=0;
			int y=0;
			#endregion
			
			#region Les séries  cgrp
			xValuesBase = new string[dt.Rows.Count-2];				
			yValuesBase = new double[dt.Rows.Count-2];
			xValuesSelected = new string[dt.Rows.Count-2];				
			yValuesSelected= new double[dt.Rows.Count-2];
	
			for(int i=0; i<dt.Rows.Count-1;i++){
				if(dt.Rows[i]["cgrpBase"].ToString()==""){
					continue;
				}else{
					xValuesBase[x]=dt.Rows[i]["InterestFamily"].ToString();
					//yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpDistributionBase"].ToString());
					yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpBase"].ToString());
					xValuesSelected[x]=dt.Rows[i]["InterestFamily"].ToString();
					//yValuesSelected[y]=double.Parse(dt.Rows[i]["cgrpDistributionSelected"].ToString());
					yValuesSelected[y]=double.Parse(dt.Rows[i]["cgrpSelected"].ToString());
					x++;
					y++;
				}
			}
			for(int i=0;i<yValuesBase.Length;i++){					
				if(maxScale<yValuesBase[i])
					maxScale=yValuesBase[i];
			}

			for(int i=0;i<yValuesSelected.Length;i++){					
				if(maxScale<yValuesSelected[i])
					maxScale=yValuesSelected[i];
			}
			#endregion
		}
		#endregion
	}
}
