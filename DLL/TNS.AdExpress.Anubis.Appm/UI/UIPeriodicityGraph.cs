#region Information
/*
 * Author : G. Ragneau
 * Creation 29/08/2005
 * Modification:
 *		
 * */
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

using TNS.AdExpress.Common;

using TNS.AdExpress.Constantes.Web;
using CstUI = TNS.AdExpress.Constantes.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;

using TNS.AdExpress.Anubis.Appm.Common;
using TNS.AdExpress.Anubis.Appm.Exceptions;

using TNS.FrameWork.DB.Common;


using Dundas.Charting.WinControl;

namespace TNS.AdExpress.Anubis.Appm.UI
{
	/// <summary>
	/// UIPeriodicityGraph : Methods to build graphs in the periodicity plan
	/// </summary>
	public class UIPeriodicityGraph:Chart
	{
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
		/// Data
		/// </summary>
		private DataTable _dtData = null;

		#endregion

		#region Constructeur
		public UIPeriodicityGraph(WebSession webSession,IDataSource dataSource, AppmConfig config, DataTable dtData):base()
		{
			_webSession = webSession;
			_dataSource = dataSource;
			_config = config;
			_dtData = dtData;
		}
		#endregion

		#region GRP  
		/// <summary>
		/// Graphiques périodicity plan GRP
		/// </summary>
		internal void BuildGRP(){
		
			#region variable
			ChartArea cArea = null;
			ChartArea cAreaTarget = null;
			Series sData = null;
			Series sTargetData = null;
			//Séries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			double maxScale=0;
			#endregion

			try {

				if(_dtData!=null && _dtData.Rows.Count>0){

					this.Size = new Size(1000,500);
					//Graph Properties
					this.BackGradientType = GradientType.TopBottom;
					this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);											
					this.BorderStyle=ChartDashStyle.Solid;
					this.BorderLineColor=Color.FromArgb(99,73,132);
					this.BorderLineWidth=1;

					cArea = new ChartArea();
					cAreaTarget = new ChartArea();

					this.ChartAreas.Add(cArea);
					this.ChartAreas.Add(cAreaTarget);

					cAreaTarget.Area3DStyle.Enable3D = cArea.Area3DStyle.Enable3D = true;

					//names
					cArea.Name = GestionWeb.GetWebWord(1736,_webSession.SiteLanguage)
						+ " GRP "
						+ " (" + _dtData.Rows[0]["baseTarget"]+") "
						+ GestionWeb.GetWebWord(1738,_webSession.SiteLanguage);
					cAreaTarget.Name = GestionWeb.GetWebWord(1736,_webSession.SiteLanguage)
						+ " GRP "
						+ " (" + _dtData.Rows[0]["additionalTarget"]+") "
						+ GestionWeb.GetWebWord(1738,_webSession.SiteLanguage);

					//positions
					cArea.Position.X = 1;
					cAreaTarget.Position.X = 51;
					cAreaTarget.Position.Y = cArea.Position.Y = 1;
					cAreaTarget.Position.Height = cArea.Position.Height = 98;
					cAreaTarget.Position.Width = cArea.Position.Width = 48;

					//Series creation
					sData = new Series();
					sTargetData = new Series();

					sData.ChartArea = cArea.Name;
					sTargetData.ChartArea = cAreaTarget.Name;

					sTargetData.Type = sData.Type = SeriesChartType.Pie;
					sTargetData.XValueType = sData.XValueType = ChartValueTypes.String;
					sTargetData.YValueType = sData.YValueType = ChartValueTypes.Double;								
					sTargetData.Enabled = sData.Enabled=true;
					
					GetSeriesDataBase(_dtData,ref xUnitValues,ref yUnitValues);
					sData.Points.DataBindXY(xUnitValues, yUnitValues);
					GetSeriesDataAdditional(_dtData,ref xUnitValues,ref yUnitValues);
					sTargetData.Points.DataBindXY(xUnitValues, yUnitValues);

					for(int k=0 ; k < _dtData.Rows.Count ; k++){
						if (k < CstUI.UI.pieColors.Length){
							sTargetData.Points[k].Color = sData.Points[k].Color = CstUI.UI.pieColors[k];
						}
						sTargetData.Points[k]["Exploded"] = sData.Points[k]["Exploded"] = "true";
					}
					sTargetData["LabelStyle"] = sData["LabelStyle"] = "Outside";
					sTargetData["PieLineColor"] = sData["PieLineColor"]="Black";
					sTargetData.Label = sData.Label="#PERCENT";

					//common legend
					sTargetData.ShowInLegend = false;
					Legend l = new Legend(cArea.Name);
					l.Enabled=true;
					l.Font = new Font(_config.DefaultFont.Name,8,
						((_config.DefaultFont.Bold)?FontStyle.Bold:0)
						|((_config.DefaultFont.Underline)?FontStyle.Underline:0)
						|((_config.DefaultFont.Italic)?FontStyle.Italic:0)
						);
					l.FontColor = _config.DefaultFontColor;
					l.Docking = LegendDocking.Bottom;
					l.Alignment = StringAlignment.Center;
					l.LegendStyle = LegendStyle.Row;
					sData.Legend = l.Name;
					sData.LegendText="#VALX";

					//Series Font
					sTargetData.Font = sData.Font = l.Font;
					sTargetData.FontColor = sData.FontColor = l.FontColor;
					this.Legends.Add(l);

					//titles
					this.Titles.Add(cArea.Name);
					this.Titles.Add(cAreaTarget.Name);

					this.Titles[0].DockToChartArea = cArea.Name;
					this.Titles[1].DockToChartArea = cAreaTarget.Name;

					this.Titles[1].DockInsideChartArea = this.Titles[0].DockInsideChartArea = true;
					this.Titles[1].Font = this.Titles[0].Font = new Font(_config.TitleFont.Name,(float)_config.DefaultFont.Size,
						((_config.TitleFont.Bold)?FontStyle.Bold:0)
						|((_config.TitleFont.Underline)?FontStyle.Underline:0)
						|((_config.TitleFont.Italic)?FontStyle.Italic:0)
						);
					this.Titles[1].Color = this.Titles[0].Color = _config.DefaultFontColor;

					//Add Series
					this.Series.Add(sData);
					this.Series.Add(sTargetData);

				}
			}
			catch(System.Exception err){				
				throw(new UIPeriodicityChartException("Unable to build pies for periodicity.",err));
			}
		}
		#endregion	

		#region periodicity
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

				if(_dtData!=null && _dtData.Rows.Count>0){

					this.Size = new Size(600,500);
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
					cArea.AxisY.LabelStyle.Font = cArea.AxisX.LabelStyle.Font = new Font(_config.DefaultFont.Name,8,
						((_config.DefaultFont.Bold)?FontStyle.Bold:0)
						|((_config.DefaultFont.Underline)?FontStyle.Underline:0)
						|((_config.DefaultFont.Italic)?FontStyle.Italic:0)
						);
					cArea.AxisY.LabelStyle.FontColor = cArea.AxisX.LabelStyle.FontColor = _config.DefaultFontColor;

					//name
					cArea.Name = GestionWeb.GetWebWord(1685,_webSession.SiteLanguage)
						+ " " + GestionWeb.GetWebWord(1738,_webSession.SiteLanguage) + " :"
						+ _dtData.Rows[0]["baseTarget"]
						+ " " + GestionWeb.GetWebWord(1739,_webSession.SiteLanguage) + " "
						+ _dtData.Rows[0]["additionalTarget"]
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
					
					GetSeriesDataCgrp(_dtData,ref xUnitValues,ref yUnitValues,ref xUnitValuesTarget,ref yUnitValuesTarget,ref maxScale);
					sData.Points.DataBindXY(xUnitValues, yUnitValues);
					sTargetData.Points.DataBindXY(xUnitValuesTarget, yUnitValuesTarget);
					cArea.AxisY.Maximum= maxScale+1000;

					for(int k=0 ; k < xUnitValues.Length ; k++){
						sData.Points[k].Color = CstUI.UI.barColors[0];
						sTargetData.Points[k].Color = CstUI.UI.barColors[1];
					}
					sTargetData["LabelStyle"] = sData["LabelStyle"] = "Outside";
					sTargetData.Label = sData.Label="#VALY";

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
					l.CustomItems.Add(CstUI.UI.barColors[0],GestionWeb.GetWebWord(1685,_webSession.SiteLanguage) + " ("+_dtData.Rows[0]["baseTarget"]+") ");
					l.CustomItems.Add(CstUI.UI.barColors[1],GestionWeb.GetWebWord(1685,_webSession.SiteLanguage) + " ("+_dtData.Rows[0]["additionalTarget"]+") ");


					this.Legends.Add(l);

					//Series Font
					sTargetData.Font = sData.Font = l.Font;
					sTargetData.FontColor = sData.FontColor = l.FontColor;

					//titles
					this.Titles.Add(cArea.Name);
					//this.Titles[0].DockToChartArea = cArea.Name;
					//this.Titles[0].DockInsideChartArea = true;
					this.Titles[0].Font = new Font(_config.TitleFont.Name,(float)_config.DefaultFont.Size,
						((_config.TitleFont.Bold)?FontStyle.Bold:0)
						|((_config.TitleFont.Underline)?FontStyle.Underline:0)
						|((_config.TitleFont.Italic)?FontStyle.Italic:0)
						);
					this.Titles[0].Color = _config.DefaultFontColor;

					//Add Series
					this.Series.Add(sData);
					this.Series.Add(sTargetData);

				}
			}
			catch(System.Exception err){				
				throw(new UIPeriodicityChartException("Unable to build bars for periodicity.",err));
			}
		}
		#endregion

		#region GetSeriesDataAdditional
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private void GetSeriesDataAdditional(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			string ventilationType="periodicity";
			int x=0;
			int y=0;
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];			
			for(int i=1; i<dt.Rows.Count-1;i++){
				xValues[x]=dt.Rows[i][ventilationType].ToString();
				yValues[y]=double.Parse(dt.Rows[i]["distributionSelected"].ToString());
				x++;
				y++;
			}
			#endregion
		}
		#endregion

		#region GetSeriesDataBase
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible de base à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private void GetSeriesDataBase(DataTable dt,ref string[] xValues,ref double[] yValues){
	
			#region Variable
			string ventilationType="periodicity";
			int x=0; 
			int y=0;
			#endregion
			
			#region Les séries  d'unité à afficher graphiquement
			xValues = new string[dt.Rows.Count];				
			yValues = new double[dt.Rows.Count];			
			for(int i=1; i<dt.Rows.Count-1;i++){
				xValues[x]=dt.Rows[i][ventilationType].ToString();
				yValues[y]=double.Parse(dt.Rows[i]["distributionBase"].ToString());
				x++;
				y++;
			}
			#endregion
		}

		#endregion

		#region GetSeriesDataCGRP
		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValuesBase">libellés du graphique</param>
		/// <param name="yValuesBase">valeurs pour graphique</param>
		/// <param name="xValuesSelected">Liebllés sélectionnée</param>
		/// <param name="yValuesSelected">Valeur sélectionnée</param>
		/// <param name="maxScale">Echel maximum</param>
		private void GetSeriesDataCgrp(DataTable dt,ref string[] xValuesBase,ref double[] yValuesBase,ref string[] xValuesSelected,ref double[] yValuesSelected,ref double maxScale){
	
			#region Variable
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
					xValuesBase[x]=dt.Rows[i]["periodicity"].ToString();
					//yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpDistributionBase"].ToString());
					yValuesBase[y]=double.Parse(dt.Rows[i]["cgrpBase"].ToString());
					xValuesSelected[x]=dt.Rows[i]["periodicity"].ToString();
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

		#region SetSeriesPeriodicityBar
		/// <summary>
		/// Crétion du graphique unité Cgrp (histogramme)
		/// </summary>
		/// <param name="dt">tableau de résultats</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">séries de valeurs</param>
		/// <param name="xValues">séries de libellés</param>
		/// <param name="yValues">séries de valeurs</param>
		///<param name="barColor">couleurs du graphique</param>
		/// <param name="chartAreaName">Nom du conteneur de l'image</param>
		/// <param name="maxScale">echelle maximum</param>
		/// <returns>séries de valeurs</returns>
		private Series SetSeriesBarPeriodicity(DataTable dt ,ChartArea chartArea,Series series,string[] xValues,double[] yValues,Color barColor,string chartAreaName,double maxScale){

			#region  Création graphique
			if(xValues!=null && yValues!=null){
								
				#region Création et définition du graphique
				//Création du graphique							
				
				//Type de graphique
				series.Type= SeriesChartType.Bar;
				series.XValueType=ChartValueTypes.String;
				series.YValueType=ChartValueTypes.Double;								
				series.Enabled=true;
																
				chartArea.Area3DStyle.Enable3D = false;
				chartArea.BackColor =Color.FromArgb(222,207,231);

				chartArea.Name=chartAreaName;
				series.ChartArea=chartArea.Name;
				chartArea.AxisY.Maximum= maxScale+1000;
				//chartArea.AxisX.Maximum= dt.Rows.Count+1;
				series.Points.DataBindXY(xValues,yValues);
				chartArea.AxisX.LabelStyle.Font = new Font("Arial", 8);
				chartArea.AxisY.LabelStyle.Font = new Font("Arial", 8);
				
				#region Définition des couleurs
				//couleur du graphique
				series.Color= barColor;
				#endregion
 
				#region Légende
				series["LabelStyle"]="Outside";
				series.LegendToolTip = "#PERCENT";
				series.ToolTip = "#VALX : #VALY";
				series["PieLineColor"]="Black";
				#endregion

				series.Label="#VALY";

				#endregion	
			}
			#endregion 

			return series;
		}

		#endregion

	}
}
