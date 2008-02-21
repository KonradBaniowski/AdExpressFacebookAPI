#region Informations
// Auteur: D. Mussuma 
// Date de création: 28/07/2006
// Date de modification:
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Constantes.Customer; 
using WebConstantes = TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.DB.Common;
using Dundas.Charting.WebControl;

namespace TNS.AdExpress.Web.Controls.Results.Appm
{
	/// <summary>
	/// Sert de classe  pour des contrôles graphiques d'analyse des parts de voix de l'APPM.
	/// </summary>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:PdvPlanAppmChartWebControl runat=server></{0}:PdvPlanAppmChartWebControl>")]
	public class PdvPlanAppmChartWebControl : BaseAppmChartWebControl {
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">Source de données</param>
		/// <param name="appmImageType">Type de l'image Appm (jpg, flash...)</param>
        public PdvPlanAppmChartWebControl(WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, ChartImageType appmImageType)
            : base(webSession, dataSource, appmImageType)
        {
		}
		#endregion

		#region Implémentation des méthodes abstraites 
		/// <summary>		
		/// Définit les données au moment du design du contrôle. 	
		/// </summary>
		public override void SetDesignMode(){

			#region variables
			double[]  yUnitValues = null;
			string[]  xUnitValues  = null;
			string chartAreaName=string.Empty;
			#endregion

			#region Chargement des données
			DataTable PDVPlanData=PDVPlanRules.GetData(this._customerWebSession,this._dataSource,this._dateBegin,this._dateEnd,this._idBaseTarget,this._idAdditionalTarget,true);
			DataTable PDVGraphicsData=PDVPlanRules.GetGraphicsData(this._customerWebSession,this._dataSource,this._dateBegin,this._dateEnd,this._idAdditionalTarget);
			#endregion

			#region Couleurs
			//colors of the pie chart
			Color[] pieColors={
								  Color.FromArgb(100,72,131),
								  Color.FromArgb(177,163,193),
								  Color.FromArgb(208,200,218),
								  Color.FromArgb(225,224,218),
								  Color.FromArgb(255,215,215),
								  Color.FromArgb(255,240,240),
								  Color.FromArgb(202,255,202),
								  Color.FromArgb(255,5,182),
								  Color.FromArgb(157,152,133),
								  Color.FromArgb(241,241,241),
								  Color.FromArgb(77,150,75),
								  Color.FromArgb(0,0,0)
							  };
			
			#endregion

			#region Construction du  graphique 

			if(PDVPlanData!=null && PDVPlanData.Rows.Count>0 && PDVGraphicsData!=null && PDVGraphicsData.Rows.Count>0) {
				yUnitValues=new double[PDVGraphicsData.Rows.Count+1];
				xUnitValues=new string[PDVGraphicsData.Rows.Count+1];

				#region euros
				//Creates chart area and series for euros
				GetSeriesData(PDVGraphicsData,PDVPlanData,xUnitValues,yUnitValues,"euros");
				chartAreaName=GestionWeb.GetWebWord(1423,this._customerWebSession.SiteLanguage);
				ChartArea chartAreaEuro=new ChartArea();
				Series euroSeries=new Series();								
				//Allignments of the chart
				chartAreaEuro.AlignOrientation = AreaAlignOrientation.Vertical;
				chartAreaEuro.Position.X=2;
				chartAreaEuro.Position.Y=2;
				chartAreaEuro.Position.Width=90;
				chartAreaEuro.Position.Height=23;	
				this.ChartAreas.Add(chartAreaEuro);
				SeriesPDVPlan(PDVGraphicsData,chartAreaEuro,euroSeries,xUnitValues,yUnitValues,WebConstantes.UI.UI.newPieColors,chartAreaName);									
				#endregion

				#region pages
				//Creates chart area and series for pages
				GetSeriesData(PDVGraphicsData,PDVPlanData,xUnitValues,yUnitValues,"pages");
				chartAreaName=GestionWeb.GetWebWord(943,this._customerWebSession.SiteLanguage);
				ChartArea chartAreaPages=new ChartArea();
				Series pageSeries=new Series();	
				//Allignments of the chart
				chartAreaPages.AlignOrientation = AreaAlignOrientation.Vertical;
				chartAreaPages.Position.X=2;
				chartAreaPages.Position.Y=27;
				chartAreaPages.Position.Width=90;
				chartAreaPages.Position.Height=23;	
				this.ChartAreas.Add(chartAreaPages);
				SeriesPDVPlan(PDVGraphicsData,chartAreaPages,pageSeries,xUnitValues,yUnitValues,WebConstantes.UI.UI.newPieColors,chartAreaName);												
				#endregion

				#region insertions
				//Creates chart area and series for insertions
				GetSeriesData(PDVGraphicsData,PDVPlanData,xUnitValues,yUnitValues,"insertions");
				chartAreaName=GestionWeb.GetWebWord(940,this._customerWebSession.SiteLanguage);
				ChartArea chartAreaInsertions=new ChartArea();
				Series insertionSeries=new Series();
				//Allignments of the chart
				chartAreaInsertions.AlignOrientation = AreaAlignOrientation.Vertical;
				chartAreaInsertions.Position.X=2;
				chartAreaInsertions.Position.Y=52;
				chartAreaInsertions.Position.Width=90;
				chartAreaInsertions.Position.Height=23;	
				this.ChartAreas.Add(chartAreaInsertions);
				SeriesPDVPlan(PDVGraphicsData,chartAreaInsertions,insertionSeries,xUnitValues,yUnitValues,WebConstantes.UI.UI.newPieColors,chartAreaName);												
				#endregion

				#region GRP	
				//Creates chart area and series for GRP
				GetSeriesData(PDVGraphicsData,PDVPlanData,xUnitValues,yUnitValues,"GRP");
				chartAreaName=GestionWeb.GetWebWord(1679,this._customerWebSession.SiteLanguage);
				ChartArea chartAreaGRP=new ChartArea();
				Series grpSeries=new Series();	
				//Allignments of the chart
				chartAreaGRP.AlignOrientation = AreaAlignOrientation.Vertical;
				chartAreaGRP.Position.X=2;
				chartAreaGRP.Position.Y=77;
				chartAreaGRP.Position.Width=90;
				chartAreaGRP.Position.Height=22;								
				this.ChartAreas.Add(chartAreaGRP);
				SeriesPDVPlan(PDVGraphicsData,chartAreaGRP,grpSeries,xUnitValues,yUnitValues,WebConstantes.UI.UI.newPieColors,chartAreaName);												
				#endregion

				#region Initializing chart and adding series to it
				InitializeComponents(this,chartAreaEuro,chartAreaPages,chartAreaInsertions,chartAreaGRP,this._imageType);
				this.Series.Add(euroSeries);
				this.Series.Add(pageSeries);
				this.Series.Add(insertionSeries);
				this.Series.Add(grpSeries);
				#endregion

				AddCopyRight(this._customerWebSession, (_imageType == ChartImageType.Jpeg));
					
			}
			else{
				this.Titles.Add(GestionWeb.GetWebWord(2106,this._customerWebSession.SiteLanguage) );
				this.Titles[0].Font=new Font("Arial", (float)8,System.Drawing.FontStyle.Bold);
				this.Titles[0].Color=Color.FromArgb(100,72,131);
				this.Width=250;
				this.Height=20;
			}
			#endregion

		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){			
			base.Render(output);
		}
		#endregion

		#region Méthodes privées 
		
		#region Construction des Series de valeurs
		/// <summary>
		/// Construct series for the PDV Plan Charts 
		/// </summary>
		/// <param name="dt">DataTable containing the data for constructing chart</param>
		/// <param name="chartArea">contenant objet graphique</param>
		/// <param name="series">Reference to the series object</param>
		/// <param name="xValues">xValues for the series</param>
		/// <param name="yValues">yValues for the series</param>
		/// <param name="pieColors">colors of the pie chart</param>
		/// <param name="chartAreaName">Name of the chart Area</param>
		private static  void SeriesPDVPlan(DataTable dt ,ChartArea chartArea,Series series,string[] xValues,double[] yValues,Color[] pieColors,string chartAreaName) {
			
			if(xValues!=null && yValues!=null){								
				#region Creation and definitions of the series, labels , legends etc
				//Graphics Type
				series.Type=SeriesChartType.Pie;
				series.SmartLabels.Enabled = true;				
				series.XValueType=ChartValueTypes.String;
				series.YValueType=ChartValueTypes.Long;								
				series.Enabled=true;
				chartArea.Area3DStyle.Enable3D = true; 
				chartArea.Name=chartAreaName;
				series.ChartArea=chartArea.Name;
				//Binding series values
				series.Points.DataBindXY(xValues,yValues);

				#region Defining Colour
				for(int k=0;k<dt.Rows.Count+1 && k<11;k++){
					series.Points[k].Color=pieColors[k];
				}
				#endregion				

				#region Labels and Tooltips
				series["LabelStyle"]="Outside";
				series.Label = "#VALX \n #PERCENT ";
				series.ToolTip = "#PERCENT #VALX";
				series.Font = new Font("Arial", (float)7);
									
				#endregion

				#endregion	
			}
			

			
		}
		#endregion

		#region Initialize Chart
		/// <summary>
		/// This method is used to set the styles for the chart
		/// </summary>
		/// <param name="appmChart">Reference to the chart Object</param>
		/// <param name="chartAreaEuros">Chart Area for the Euro Pie</param>
		/// <param name="chartAreaPages">Chart Area for the Pages Pie</param>
		/// <param name="chartAreaInsertions">Chart Area for the Insertions Pie</param>
		/// <param name="chartAreaGRP">Chart Area for the GRP Pie</param>
		/// <param name="appmImageType">type de l'image</param>
		private static void InitializeComponents(BaseAppmChartWebControl appmChart,ChartArea chartAreaEuros,ChartArea chartAreaPages,ChartArea chartAreaInsertions,ChartArea chartAreaGRP,ChartImageType appmImageType) {			
			#region type image

			appmChart.ImageType=appmImageType;
			if(appmImageType==ChartImageType.Flash){
				appmChart.AnimationTheme = AnimationTheme.MovingFromTop;
				appmChart.AnimationDuration = 3;
				appmChart.RepeatAnimation = false;
			}

			#endregion

			#region Chart
			//setting dimensions and properties of the chart
			appmChart.Width=new Unit("850");
			appmChart.Height=new Unit("1000");
			appmChart.BackGradientType = GradientType.TopBottom;
			appmChart.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);											
			appmChart.BorderStyle=ChartDashStyle.Solid;
			appmChart.BorderLineColor=Color.FromArgb(99,73,132);
			appmChart.BorderLineWidth=2;
			appmChart.Legend.Enabled=false;
			#endregion	

			#region Titles 
			//Euros			
			appmChart.Titles.Add(chartAreaEuros.Name);
			appmChart.Titles[0].DockInsideChartArea=true;
			appmChart.Titles[0].DockInsideChartArea=true;
			appmChart.Titles[0].Position.Auto = false;
			appmChart.Titles[0].Position.X = 47;
			appmChart.Titles[0].Position.Y = 2;
			appmChart.Titles[0].Font=new Font("Arial", (float)12);
			appmChart.Titles[0].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[0].DockToChartArea=chartAreaEuros.Name;	
			//pages
			appmChart.Titles.Add(chartAreaPages.Name);
			appmChart.Titles[1].DockInsideChartArea=true;
			appmChart.Titles[1].DockInsideChartArea=true;
			appmChart.Titles[1].Position.Auto = false;
			appmChart.Titles[1].Position.X = 47;
			appmChart.Titles[1].Position.Y = 26;
			appmChart.Titles[1].Font=new Font("Arial", (float)12);
			appmChart.Titles[1].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[1].DockToChartArea=chartAreaPages.Name;
			//Insertions
			appmChart.Titles.Add(chartAreaInsertions.Name);
			appmChart.Titles[2].DockInsideChartArea=true;
			appmChart.Titles[2].DockInsideChartArea=true;
			appmChart.Titles[2].Position.Auto = false;
			appmChart.Titles[2].Position.X = 47;
			appmChart.Titles[2].Position.Y = 51;
			appmChart.Titles[2].Font=new Font("Arial", (float)12);
			appmChart.Titles[2].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[2].DockToChartArea=chartAreaInsertions.Name;
			//GRP
			appmChart.Titles.Add(chartAreaGRP.Name);
			appmChart.Titles[3].DockInsideChartArea=true;
			appmChart.Titles[3].DockInsideChartArea=true;
			appmChart.Titles[3].Position.Auto = false;
			appmChart.Titles[3].Position.X = 47;
			appmChart.Titles[3].Position.Y = 76;
			appmChart.Titles[3].Font=new Font("Arial", (float)12);
			appmChart.Titles[3].Color=Color.FromArgb(100,72,131);
			appmChart.Titles[3].DockToChartArea=chartAreaGRP.Name;			
			#endregion
		}
		#endregion

		#region Gets data for the Series
		/// <summary>
		/// This method populates the xValues and Yvalues arrays which are then used to construct the series
		/// </summary>
		/// <param name="graphicsTable">Table that contains the data for reference univers</param>
		/// <param name="totalTable">Table that contains the data for the total univers</param>
		/// <param name="xValues">An array that carries XValues for the series</param>
		/// <param name="yValues">An array that carries YValues for the series</param>
		/// <param name="colunm">colunm whose value is required for example euros, pages etc</param>
		private static void GetSeriesData(DataTable graphicsTable,DataTable totalTable,string[] xValues,double[] yValues,string colunm) {
			int x=1;
			int y=1;
			
			//values for the rest of the chart series  
			double rest=Convert.ToDouble(totalTable.Rows[0][colunm])-Convert.ToDouble(totalTable.Rows[1][colunm]);
			xValues[0]="";
			yValues[0]=rest;
			//Values for the series
			foreach(DataRow dr in graphicsTable.Rows){
				xValues[x] = dr["elements"].ToString();  //":"+                       //+" ["+dr["elementType"]+"] ";
				yValues[y]=double.Parse(dr[colunm].ToString());
				x++;
				y++;
			}
				
		}
		#endregion

		#endregion
	}
}
