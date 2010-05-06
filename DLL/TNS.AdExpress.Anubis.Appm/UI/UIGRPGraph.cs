#region Information
/*
 * Author : D. V. Mussuma
 * Creation 26/09/2005
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
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using TNS.AdExpress.Constantes.Customer;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using System.Collections.Generic;
using TNS.FrameWork.WebTheme;


namespace TNS.AdExpress.Anubis.Appm.UI
{
	/// <summary>
	/// UIGRPGraph : Methods to build graphs in the family,PDV,Periodicity plan
	/// </summary>
	public class UIGRPGraph : Chart
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
		/// Data PDV
		/// </summary>
		private DataTable _dtPdvData = null;
		/// <summary>
		/// Data total PDV
		/// </summary>
		private DataTable _dtTotalPdvData = null;
		/// <summary>
		/// Data Interest Family
		/// </summary>
		private DataTable _dtFamilyData = null;
		/// <summary>
		/// Data Periodicity
		/// </summary>
		private DataTable _dtPeriodicityData = null;
        /// <summary>
        /// Style
        /// </summary>
        private TNS.FrameWork.WebTheme.Style _style = null;
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
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">customer session</param>
		/// <param name="dataSource">date source</param>
		/// <param name="config">config parameters</param>
		/// <param name="dsData">tables de données</param>
		public UIGRPGraph(WebSession webSession,IDataSource dataSource, AppmConfig config, DataSet dsData,TNS.FrameWork.WebTheme.Style style):base() {
			_webSession = webSession;
			_dataSource = dataSource;
			_config = config;
			_dtPdvData = dsData.Tables["Pdv"];
			_dtTotalPdvData = dsData.Tables["TotalPdv"];
			_dtFamilyData = dsData.Tables["Family"];
			_dtPeriodicityData = dsData.Tables["Periodicity"];
            _style = style;

            _pieColors = ((Colors)_style.GetTag("GrpGraphPieColors")).ColorList;
            _barColors = ((Colors)_style.GetTag("GrpGraphBarColors")).ColorList;
		}
		#endregion

		#region GRP
		/// <summary>
		/// Graphiques PDV,Familles d'intérêts,périodicity plan par GRP
		/// </summary>
		internal void BuildGRP(){
		
			#region variable			
			ChartArea cAreaPDV = null;
			ChartArea cAreaFamily = null;
			ChartArea cAreaPeriodicity = null;
			Series sPDVData = null;
			Series sFamilyData = null;
			Series sPeriodicityData = null;
			//Séries de valeurs pour chaque tranche du graphique
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			double maxScale=0;
			#endregion

            //Base target
			string targets = "'" + _webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CstRights.type.aepmTargetAccess) + "'";
			//Wave
			string idWave = ((LevelInformation)_webSession.SelectionUniversAEPMWave.Nodes[0].Tag).ID.ToString();
			DataSet ds = TargetListDataAccess.GetAEPMTargetListFromIDSDataAccess(idWave, targets, _webSession.Source);			

			try {

				if(_dtPdvData!=null && _dtPdvData.Rows.Count>0 && _dtFamilyData!=null && _dtFamilyData.Rows.Count>0 && _dtPeriodicityData!=null && _dtPeriodicityData.Rows.Count>0){

                    _style.GetTag("GrpGraphSize").SetStyleDundas(this);
					//Graph Properties
					this.BackGradientType = GradientType.TopBottom;
                    _style.GetTag("GrpGraphLineEnCircle").SetStyleDundas(this);

					cAreaPDV  = new ChartArea();
					cAreaFamily = new ChartArea();
					cAreaPeriodicity = new ChartArea();

					this.ChartAreas.Add(cAreaPDV);
					this.ChartAreas.Add(cAreaFamily);
					this.ChartAreas.Add(cAreaPeriodicity);

					cAreaPDV.Area3DStyle.Enable3D = cAreaFamily.Area3DStyle.Enable3D =  cAreaPeriodicity.Area3DStyle.Enable3D= true;

					//names
					cAreaPDV.Name = GestionWeb.GetWebWord(1775,_webSession.SiteLanguage)						
						+ " \n (" + ds.Tables[0].Rows[0]["target"]+"). ";

					cAreaFamily.Name = GestionWeb.GetWebWord(1736,_webSession.SiteLanguage)
						+ " "
						+ " GRP "
						+ " ("+_dtFamilyData.Rows[0]["additionalTarget"]+") \n "
						+ GestionWeb.GetWebWord(1737,_webSession.SiteLanguage);
					
					cAreaPeriodicity.Name = GestionWeb.GetWebWord(1736,_webSession.SiteLanguage)
						+ " GRP "
						+ " (" + _dtPeriodicityData.Rows[0]["additionalTarget"]+") \n "
						+ GestionWeb.GetWebWord(1738,_webSession.SiteLanguage);

					//positions
					cAreaPDV.Position.X = 1;
					cAreaFamily.Position.X = 34;
					cAreaPeriodicity.Position.X = 67;
					cAreaPDV.Position.Y = cAreaFamily.Position.Y = cAreaPeriodicity.Position.Y = 1;
					cAreaPDV.Position.Height = cAreaFamily.Position.Height = cAreaPeriodicity.Position.Height = 98;
					cAreaPDV.Position.Width = cAreaFamily.Position.Width = cAreaPeriodicity.Position.Width = 32;
					

					//Series creation
					sPDVData = new Series();
					sFamilyData = new Series();
					sPeriodicityData = new Series();

					sPDVData.ChartArea = cAreaPDV.Name;
					sFamilyData.ChartArea = cAreaFamily.Name;
					sPeriodicityData.ChartArea = cAreaPeriodicity.Name;

					sPDVData.Type = sFamilyData.Type = sPeriodicityData.Type = SeriesChartType.Pie;
					sPDVData.XValueType = sFamilyData.XValueType = sPeriodicityData.XValueType = ChartValueTypes.String;
					sPeriodicityData.YValueType = ChartValueTypes.Double;								
					sPDVData.Enabled = sFamilyData.Enabled = sPeriodicityData.Enabled = true;
					
					#region data series

					//pdv data series					
					yUnitValues=new double[_dtPdvData.Rows.Count+1];
					xUnitValues=new string[_dtPdvData.Rows.Count+1];				
					GetSeriesData(_webSession,_dtPdvData,_dtTotalPdvData,xUnitValues,yUnitValues,"GRP");					
					sPDVData.YValueType=ChartValueTypes.Long;								
					sPDVData.Points.DataBindXY(xUnitValues,yUnitValues);

					//Interst family data series 
					yUnitValues=new double[_dtFamilyData.Rows.Count+1];
					xUnitValues=new string[_dtFamilyData.Rows.Count+1];					
					GetFamilySeriesDataAdditional(_dtFamilyData,ref xUnitValues,ref yUnitValues);										
					sFamilyData.YValueType=ChartValueTypes.Long;								
					sFamilyData.Points.DataBindXY(xUnitValues,yUnitValues);

					//Periodicity data series
					GetSeriesDataAdditional(_dtPeriodicityData,ref xUnitValues,ref yUnitValues);
					sPeriodicityData.Points.DataBindXY(xUnitValues, yUnitValues);

					#endregion
					
					#region Points
					
					//PDV points
					for(int k=0 ; k<sPDVData.Points.Count ; k++){
						if(k < _pieColors.Count){
                            sPDVData.Points[k].Color = _pieColors[k];
						}
						sPDVData.Points[k]["Exploded"] = "true";
					}
					//Interst family point
					for(int k=0 ; k<sFamilyData.Points.Count ; k++){
                        if (k < _pieColors.Count) {
                            sFamilyData.Points[k].Color = _pieColors[k];
						}
						sFamilyData.Points[k]["Exploded"] = "true";
					}
					//Periodicity points
					for(int k=0 ; k < _dtPeriodicityData.Rows.Count ; k++){
                        if (k < _pieColors.Count) {
                            sPeriodicityData.Points[k].Color = _pieColors[k];
						}
						sPeriodicityData.Points[k]["Exploded"] = "true";
					}
					
					#endregion

					#region Labels and Tooltips
					sPeriodicityData["LabelStyle"] = sFamilyData["LabelStyle"] =  sPDVData["LabelStyle"] = "Outside";
					sPeriodicityData["PieLineColor"] = sFamilyData["PieLineColor"]= sPDVData["PieLineColor"] = "Black";					
					sFamilyData.Label= sPDVData.Label = sPeriodicityData.Label = "#PERCENT" ;
					#endregion

					//common legend
					//sPDVData.ShowInLegend = sPeriodicityData.ShowInLegend = sFamilyData.ShowInLegend = false;
					Legend lPeriodicity = new Legend(cAreaPeriodicity.Name);
					Legend lPDV = new Legend(cAreaPDV.Name);				
					Legend lFamily = new Legend(cAreaFamily.Name);

					lPeriodicity.Enabled=lPDV.Enabled=lFamily.Enabled=true;


                    _style.GetTag("GrpGraphDefaultFont").SetStyleDundas(lPeriodicity);
                    _style.GetTag("GrpGraphDefaultFont").SetStyleDundas(lFamily);
                    _style.GetTag("GrpGraphDefaultFont").SetStyleDundas(lPDV);

					//Docking
					lPeriodicity.Docking = lPDV.Docking = lFamily.Docking = LegendDocking.Bottom;
					

//					lFamily.Alignment = lPeriodicity.Alignment = lPDV.Alignment = StringAlignment.Center;
//					lFamily.LegendStyle = lPeriodicity.LegendStyle = lPDV.LegendStyle = LegendStyle.Row;
					
				
					sPDVData.Legend = lPDV.Name;
					sPDVData.LegendText="#PERCENT : #VALX";									
					sFamilyData.Legend = lFamily.Name;
					sFamilyData.LegendText="#PERCENT : #VALX";
					sPeriodicityData.Legend = lPeriodicity.Name;
					sPeriodicityData.LegendText="#PERCENT : #VALX";
					
					
					//Series Font
					sFamilyData.Font = sPeriodicityData.Font = sPDVData.Font = lPDV.Font;
					sFamilyData.FontColor = sPeriodicityData.FontColor = sPDVData.FontColor = lPDV.FontColor;
					this.Legends.Add(lPDV);
					this.Legends.Add(lFamily);
					this.Legends.Add(lPeriodicity);
					
					// Dock the default legend inside the first chart area
					this.Legends[cAreaPDV.Name].DockInsideChartArea = true;
					this.Legends[cAreaPDV.Name].DockToChartArea = cAreaPDV.Name;

					this.Legends[cAreaFamily.Name].DockInsideChartArea = true;
					this.Legends[cAreaFamily.Name].DockToChartArea = cAreaFamily.Name;
					
					this.Legends[cAreaPeriodicity.Name].DockInsideChartArea = true;
					this.Legends[cAreaPeriodicity.Name].DockToChartArea = cAreaPeriodicity.Name;


					//titles
					this.Titles.Add(cAreaPDV.Name);
					this.Titles.Add(cAreaFamily.Name);
					this.Titles.Add(cAreaPeriodicity.Name);

					this.Titles[0].DockToChartArea = cAreaPDV.Name;
					this.Titles[1].DockToChartArea = cAreaFamily.Name;
					this.Titles[2].DockToChartArea = cAreaPeriodicity.Name;


					this.Titles[2].DockInsideChartArea = this.Titles[1].DockInsideChartArea = this.Titles[0].DockInsideChartArea = true;
                    _style.GetTag("GrpGraphDefaultTitleFont").SetStyleDundas(this.Titles[2]);
                    _style.GetTag("GrpGraphDefaultTitleFont").SetStyleDundas(this.Titles[1]);
                    _style.GetTag("GrpGraphDefaultTitleFont").SetStyleDundas(this.Titles[0]);

					//Add Series
					this.Series.Add(sPDVData);
					this.Series.Add(sFamilyData);
					this.Series.Add(sPeriodicityData);

				}
			}
			catch(System.Exception err){				
				throw(new UIPeriodicityChartException("Unable to build pies for GRP.",err));
			}
		}
		#endregion

		#region CGRP
		/// <summary>
		/// Graphiques PDV,Familles d'intérêts,périodicity plan par CGRP 
		/// </summary>
		internal void BuildCGRP(){
			
		
				#region variable
				ChartArea cAreaFamily = null;
				ChartArea cAreaPeriodicity = null;
				Series sDataFamily = null;
				Series sTargetDataFamily = null;
				Series sDataPeriodicity = null;
				Series sTargetDataPeriodicity = null;
				//Séries de valeurs pour chaque tranche du graphique
				double[]  yUnitValues = null;
				string[]  xUnitValues  = null;
				string[]  xUnitValuesTarget = null;
				double[]  yUnitValuesTarget  = null;
				double maxScale=0;
				#endregion

			try {

				if(_dtFamilyData!=null && _dtFamilyData.Rows.Count>0 && _dtPeriodicityData!=null && _dtPeriodicityData.Rows.Count>0){

                    _style.GetTag("CgrpGraphSize").SetStyleDundas(this);
                    //Graph Properties
                    this.BackGradientType = GradientType.TopBottom;
                    _style.GetTag("CgrpGraphLineEnCircle").SetStyleDundas(this);

					cAreaFamily = new ChartArea();
					cAreaPeriodicity = new ChartArea();

					this.ChartAreas.Add(cAreaFamily);
					this.ChartAreas.Add(cAreaPeriodicity);

					cAreaFamily.Area3DStyle.Enable3D = cAreaPeriodicity.Area3DStyle.Enable3D = false;
					cAreaFamily.BackColor = cAreaPeriodicity.BackColor = Color.FromArgb(222,207,231);
                    _style.GetTag("CgrpGraphDefaultFont").SetStyleDundas(cAreaFamily.AxisY.LabelStyle);
                    _style.GetTag("CgrpGraphDefaultFont").SetStyleDundas(cAreaFamily.AxisX.LabelStyle);
                    _style.GetTag("CgrpGraphDefaultFont").SetStyleDundas(cAreaPeriodicity.AxisY.LabelStyle);
                    _style.GetTag("CgrpGraphDefaultFont").SetStyleDundas(cAreaPeriodicity.AxisX.LabelStyle);

					//name
					cAreaFamily.Name = GestionWeb.GetWebWord(1685,_webSession.SiteLanguage)
						+ " " + GestionWeb.GetWebWord(1737,_webSession.SiteLanguage) + " :"
						+ _dtFamilyData.Rows[0]["baseTarget"]
						+ " " + GestionWeb.GetWebWord(1739,_webSession.SiteLanguage) + " "
						+ _dtFamilyData.Rows[0]["additionalTarget"]
						;
					
					cAreaPeriodicity.Name = GestionWeb.GetWebWord(1685,_webSession.SiteLanguage)
						+ " " + GestionWeb.GetWebWord(1738,_webSession.SiteLanguage) + " :"
						+ _dtPeriodicityData.Rows[0]["baseTarget"]
						+ " " + GestionWeb.GetWebWord(1739,_webSession.SiteLanguage) + " "
						+ _dtPeriodicityData.Rows[0]["additionalTarget"]
						;

					//positions
					cAreaFamily.Position.X = 1;
					cAreaFamily.Position.Y = 10;
					cAreaPeriodicity.Position.X = 50;
					cAreaPeriodicity.Position.Y = 10;
					cAreaFamily.Position.Height = cAreaPeriodicity.Position.Height = 80;
					cAreaFamily.Position.Width = cAreaPeriodicity.Position.Width  = 48;

					//Series creation
					sDataFamily = new Series();
					sTargetDataFamily = new Series();
					sDataPeriodicity = new Series();
					sTargetDataPeriodicity = new Series();


					sDataFamily.ChartArea = cAreaFamily.Name;
					sTargetDataFamily.ChartArea = cAreaFamily.Name;
					sDataPeriodicity.ChartArea = cAreaPeriodicity.Name;
					sTargetDataPeriodicity.ChartArea = cAreaPeriodicity.Name;


					sTargetDataFamily.Type = sDataFamily.Type = sTargetDataPeriodicity.Type = sDataPeriodicity.Type = SeriesChartType.Bar;
					sTargetDataFamily.XValueType = sDataFamily.XValueType = sTargetDataPeriodicity.XValueType = sDataPeriodicity.XValueType = ChartValueTypes.String;
					sTargetDataFamily.YValueType = sDataFamily.YValueType = sTargetDataPeriodicity.YValueType = sDataPeriodicity.YValueType =  ChartValueTypes.Double;								
					sTargetDataFamily.Enabled = sDataFamily.Enabled=sTargetDataPeriodicity.Enabled = sDataPeriodicity.Enabled=true;
					
					//family series
					GetSeriesDataFamilyCgrp(_dtFamilyData,ref xUnitValues,ref yUnitValues,ref xUnitValuesTarget,ref yUnitValuesTarget,ref maxScale);
					sDataFamily.Points.DataBindXY(xUnitValues, yUnitValues);
					sTargetDataFamily.Points.DataBindXY(xUnitValuesTarget, yUnitValuesTarget);
					cAreaFamily.AxisY.Maximum= maxScale+1000;

					for(int k=0 ; k < xUnitValues.Length ; k++){
						sDataFamily.Points[k].Color = _barColors[0];
						sTargetDataFamily.Points[k].Color = _barColors[1];
					}
					sTargetDataFamily["LabelStyle"] = sDataFamily["LabelStyle"] = "Outside";
					sTargetDataFamily["PointWidth"] = sDataFamily["PointWidth"] = "1.0";
					sTargetDataFamily.Label = sDataFamily.Label="#VALY";
					cAreaFamily.AxisX.Interval=1;
					cAreaFamily.AxisX.Margin=true;
					cAreaFamily.AxisX.LabelStyle.ShowEndLabels = true;
					
					//Periodicity series
					maxScale=0;
					GetSeriesDataCgrp(_dtPeriodicityData,ref xUnitValues,ref yUnitValues,ref xUnitValuesTarget,ref yUnitValuesTarget,ref maxScale);
					sDataPeriodicity.Points.DataBindXY(xUnitValues, yUnitValues);
					sTargetDataPeriodicity.Points.DataBindXY(xUnitValuesTarget, yUnitValuesTarget);
					cAreaPeriodicity.AxisY.Maximum= maxScale+1000;

					for(int k=0 ; k < xUnitValues.Length ; k++){
						sDataPeriodicity.Points[k].Color = _barColors[0];
						sTargetDataPeriodicity.Points[k].Color = _barColors[1];
					}

					//common legend
					sTargetDataFamily.ShowInLegend = false;
					sDataFamily.ShowInLegend = false;
					sTargetDataPeriodicity.ShowInLegend = false;
					sDataPeriodicity.ShowInLegend = false;

					Legend l = new Legend(cAreaFamily.Name);
					l.Enabled=true;
					l.Font = cAreaFamily.AxisY.LabelStyle.Font;
					l.FontColor = cAreaFamily.AxisY.LabelStyle.FontColor;
					l.Docking = LegendDocking.Bottom;
					l.Alignment = StringAlignment.Center;
					l.LegendStyle = LegendStyle.Row;
					l.CustomItems.Add(_barColors[0],GestionWeb.GetWebWord(1685,_webSession.SiteLanguage) + " ("+_dtFamilyData.Rows[0]["baseTarget"]+") ");
					l.CustomItems.Add(_barColors[1],GestionWeb.GetWebWord(1685,_webSession.SiteLanguage) + " ("+_dtFamilyData.Rows[0]["additionalTarget"]+") ");


					this.Legends.Add(l);

					//Series Font
					sTargetDataFamily.Font = sDataFamily.Font = sTargetDataPeriodicity.Font = sDataPeriodicity.Font = l.Font;
					sTargetDataFamily.FontColor = sDataFamily.FontColor = 	sTargetDataPeriodicity.FontColor = sDataPeriodicity.FontColor = l.FontColor;

					//titles
					this.Titles.Add(cAreaFamily.Name);
					this.Titles.Add(cAreaPeriodicity.Name);

					this.Titles[0].DockInsideChartArea = false;
					this.Titles[1].DockInsideChartArea = false;
					//test
					this.Titles[0].Position.Auto = false;
					this.Titles[0].Position.X = 26;
					this.Titles[0].Position.Y = 8;
					this.Titles[1].Position.Auto = false;
					this.Titles[1].Position.X = 76;
					this.Titles[1].Position.Y = 8;

//					this.Titles[0].Docking = Docking.Top;
//					this.Titles[1].Docking = Docking.Top;
					
					this.Titles[0].DockToChartArea = cAreaFamily.Name;
					this.Titles[1].DockToChartArea = cAreaPeriodicity.Name;

                    _style.GetTag("CgrpGraphDefaultTitleFont").SetStyleDundas(this.Titles[0]);
                    _style.GetTag("CgrpGraphDefaultTitleFont").SetStyleDundas(this.Titles[1]);
					

					//Add Series
					this.Series.Add(sDataFamily);
					this.Series.Add(sTargetDataFamily);
					this.Series.Add(sDataPeriodicity);
					this.Series.Add(sTargetDataPeriodicity);

				}
			}
			catch(System.Exception err){				
				throw(new UIPeriodicityChartException("Unable to build bars for CGRP.",err));
			}
		}
		#endregion

		#region Get Series Data for periodicity
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

		#region Gets data for interest family

		/// <summary>
		/// Obtient les séries de valeurs des unités pour la cible sélectionnée à afficher graphiquement
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="xValues">libellés du graphique</param>
		/// <param name="yValues">valeurs pour graphique</param>
		private static void GetFamilySeriesDataAdditional(DataTable dt,ref string[] xValues,ref double[] yValues){
	
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
		private static void GetSeriesDataFamilyCgrp(DataTable dt,ref string[] xValuesBase,ref double[] yValuesBase,ref string[] xValuesSelected,ref double[] yValuesSelected,ref double maxScale){
	
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

		#region Gets data for the Series for PDV
		/// <summary>
		/// This method populates the xValues and Yvalues arrays which are then used to construct the series
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="graphicsTable">Table that contains the data for reference univers</param>
		/// <param name="totalTable">Table that contains the data for the total univers</param>
		/// <param name="xValues">An array that carries XValues for the series</param>
		/// <param name="yValues">An array that carries YValues for the series</param>
		/// <param name="colunm">colunm whose value is required for example euros, pages etc</param>
		private void GetSeriesData(WebSession webSession,DataTable graphicsTable,DataTable totalTable,string[] xValues,double[] yValues,string colunm) {
			
			int x=1;
			int y=1;
			
			//values for the rest of the chart series  
			double rest=Convert.ToDouble(totalTable.Rows[0][colunm])-Convert.ToDouble(totalTable.Rows[1][colunm]);
			if(_webSession.CompetitorUniversAdvertiser!=null){
				if(_webSession.CompetitorUniversAdvertiser.Count>1){
					xValues[0]=GestionWeb.GetWebWord(1678,webSession.SiteLanguage);;
				}else if(webSession.CompetitorUniversAdvertiser.Count==1)	xValues[0]=GestionWeb.GetWebWord(1668,webSession.SiteLanguage);
			}else xValues[0]="";
			yValues[0]=rest;
			//Values for the series
			foreach(DataRow dr in graphicsTable.Rows){
				xValues[x] = dr["elements"].ToString();                         //+" ["+dr["elementType"]+"] ";
				yValues[y] = double.Parse(dr[colunm].ToString());
				x++;
				y++;
			}
				
		}
		#endregion



		
	}
}
