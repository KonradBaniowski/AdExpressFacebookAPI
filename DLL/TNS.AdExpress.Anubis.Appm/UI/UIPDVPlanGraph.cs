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
using TNS.FrameWork.DB.Common;

using Dundas.Charting.WinControl;

namespace TNS.AdExpress.Anubis.Appm.UI {
	/// <summary>
	/// UIPDVPlanGrtaph : Methods to build graphs in the PDV plan
	/// </summary>
	public class UIPDVPlanGraph : Chart {

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
		/// Data containing total GRP (for the rest)
		/// </summary>
		private DataTable _dtTotalData = null;

		#endregion

		#region Constructeur
		public UIPDVPlanGraph(WebSession webSession,IDataSource dataSource, AppmConfig config, DataTable dtGraphicsData, DataTable dtTotalData):base() {
			_webSession = webSession;
			_dataSource = dataSource;
			_config = config;
			_dtGraphicsData = dtGraphicsData;
			_dtTotalData = dtTotalData;
		}
		#endregion

		#region GRP
		/// <summary>
		/// Graphiques PDV plan GRP
		/// </summary>
		internal void BuildGRP(){

			string[] xUnitValues = null;
			double[] yUnitValues = null;
			ChartArea cArea = null;
			Series sData = null;

			try{

				if(_dtGraphicsData.Rows.Count>0){

					#region Graph Properties
					this.Size = new Size(800,400);					
					this.BackGradientType = GradientType.TopBottom;
					this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);											
					this.BorderStyle=ChartDashStyle.Solid;
					this.BorderLineColor=Color.FromArgb(99,73,132);
					this.BorderLineWidth=1;

					cArea = new ChartArea();
					cArea.Name = GestionWeb.GetWebWord(1775, _webSession.SiteLanguage);
					this.ChartAreas.Add(cArea);
					cArea.Area3DStyle.Enable3D = true;


					//positions
					cArea.Position.X = 1;
					cArea.Position.Y = 1;				
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
					GetSeriesData(_dtGraphicsData,_dtTotalData,xUnitValues,yUnitValues,"GRP");
					sData.XValueType=ChartValueTypes.String;
					sData.YValueType=ChartValueTypes.Long;								
					sData.Points.DataBindXY(xUnitValues,yUnitValues);
					#endregion

					#region Defining Colour
					for(int k=0 ; k<sData.Points.Count ; k++){
						if(k < CstUI.UI.pieColors.Length){
							sData.Points[k].Color = CstUI.UI.pieColors[k];
						}
						sData.Points[k]["Exploded"] = "true";
					}
					#endregion

					#region Labels and Tooltips
					sData["LabelStyle"]="Outside";
					sData.Label="#PERCENT #VALX" ;
					sData["PieLineColor"]="Black";
					sData.ShowInLegend = false;
					sData.Font = new Font(_config.DefaultFont.Name,8,
						((_config.DefaultFont.Bold)?FontStyle.Bold:0)
						|((_config.DefaultFont.Underline)?FontStyle.Underline:0)
						|((_config.DefaultFont.Italic)?FontStyle.Italic:0)
						);
					sData.FontColor = _config.DefaultFontColor;
					this.Titles.Add(cArea.Name);
					this.Titles[0].DockToChartArea = cArea.Name;
					this.Titles[0].DockInsideChartArea = true;
					this.Titles[0].Font = new Font(_config.TitleFont.Name,(float)_config.DefaultFont.Size,
						((_config.TitleFont.Bold)?FontStyle.Bold:0)
						|((_config.TitleFont.Underline)?FontStyle.Underline:0)
						|((_config.TitleFont.Italic)?FontStyle.Italic:0)
						);
					this.Titles[0].Color = _config.TitleFontColor;

					#endregion

					this.Series.Add(sData);
				}

			}
			catch(System.Exception err){				
				throw(new UIPDVPlanChartException("Unable to build pies for PDV Plan.",err));
			}

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
		private void GetSeriesData(DataTable graphicsTable,DataTable totalTable,string[] xValues,double[] yValues,string colunm) {
			int x=1;
			int y=1;
			
			//values for the rest of the chart series  
			double rest=Convert.ToDouble(totalTable.Rows[0][colunm])-Convert.ToDouble(totalTable.Rows[1][colunm]);
			xValues[0]="";
			yValues[0]=rest;
			//Values for the series
			foreach(DataRow dr in graphicsTable.Rows){
				xValues[x]=":"+dr["elements"].ToString();                         //+" ["+dr["elementType"]+"] ";
				yValues[y]=double.Parse(dr[colunm].ToString());
				x++;
				y++;
			}
				
		}
		#endregion

	}
}
