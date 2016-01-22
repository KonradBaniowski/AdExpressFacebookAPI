#region Information
//Author : Y. Rkaina 
//Creation : 19/07/2006
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;

using TNS.AdExpress.Constantes.Web;
using CstUI = TNS.AdExpress.Constantes.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

using TNS.AdExpress.Anubis.Hotep.Common;
using TNS.AdExpress.Anubis.Hotep.Exceptions;

using Dundas.Charting.WinControl;
using TNS.FrameWork.DB.Common;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork;
using TblFormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using TNS.FrameWork.WebTheme;

namespace TNS.AdExpress.Anubis.Hotep.UI
{
	/// <summary>
	/// Description résumée de UIMediaStrategyGraph.
	/// </summary>
	public class UIMediaStrategyGraph :  Chart{ 
	
		#region Attributes
		/// <summary>
		/// User Session
		/// </summary>
		protected WebSession _webSession = null;
		/// <summary>
		/// Data Source
		/// </summary>
        protected IDataSource _dataSource = null;
		/// <summary>
		/// Hotep configuration
		/// </summary>
        protected HotepConfig _config = null;
        /// <summary>
        /// Style
        /// </summary>
        protected TNS.FrameWork.WebTheme.Style _style = null;
        /// <summary>
        /// Pie ColorS
        /// </summary>
        protected List<Color> _pieColors = null;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, Series> _listSeriesMedia = null;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<int, string> _listSeriesName = null;
		#endregion
		
		#region Constructeur
        public UIMediaStrategyGraph(WebSession webSession, 
            IDataSource dataSource, 
            HotepConfig config, 
            TNS.FrameWork.WebTheme.Style style,
            Dictionary<string, Series> listSeriesMedia,
            Dictionary<int, string> listSeriesName)
            : base() {
            _webSession = webSession;
            _dataSource = dataSource;
            _config = config;
            _style = style;
            _pieColors = ((Colors)_style.GetTag("MediaStrategyGraphPieColors")).ColorList;
            _listSeriesMedia = listSeriesMedia;
            _listSeriesName = listSeriesName;
        }
		#endregion
		
		#region MediaStrategy
		/// <summary>
		/// Graphiques Media Strategy
		/// </summary>
		public virtual void BuildMediaStrategy(){

            #region Constantes
            /// <summary>
            /// Hauteur d'un graphique stratégie média
            /// </summary>
            const int MEDIA_STRATEGY_HEIGHT_GRAPHIC = 300;
            #endregion

            #region Chart
            ChartArea chartArea = new ChartArea();
            this.ChartAreas.Add(chartArea);
            _style.GetTag("MediaStrategyGraphSize").SetStyleDundas(this);
            this.BackGradientType = GradientType.TopBottom;
            _style.GetTag("MediaStrategyGraphLineEnCircle").SetStyleDundas(this);
            this.Legend.Enabled = false;
            #endregion
			
			#region Parcours de tab

			float yPosition=0.0F;

			#region Affichage des graphiques
			int iterator=0;
            foreach (int j in _listSeriesName.Keys) {
				if(((Series)_listSeriesMedia[(string)_listSeriesName[j]]).Points.Count>0){
										
					#region Type de Graphique
					((Series)_listSeriesMedia[(string)_listSeriesName[j]]).Type= SeriesChartType.Pie;
					#endregion
				
					#region Définition des couleurs
                    for (int k = 0; k < _pieColors .Count && k < ((Series)_listSeriesMedia[(string)_listSeriesName[j]]).Points.Count; k++) {
						((Series)_listSeriesMedia[(string)_listSeriesName[j]]).Points[k].Color=_pieColors[k];
					}
					#endregion
				
					#region Légende
					((Series)_listSeriesMedia[(string)_listSeriesName[j]])["LabelStyle"]="Outside";
					((Series)_listSeriesMedia[(string)_listSeriesName[j]]).LegendToolTip = "#PERCENT";
					((Series)_listSeriesMedia[(string)_listSeriesName[j]]).ToolTip = " "+(string)_listSeriesName[j]+" \n #VALX : #PERCENT";
					((Series)_listSeriesMedia[(string)_listSeriesName[j]])["PieLineColor"]="Black";
					#endregion

					#region Création et définition du graphique
					ChartArea chartArea2=new ChartArea();
					this.ChartAreas.Add(chartArea2);
					chartArea2.Area3DStyle.Enable3D = true; 
					chartArea2.Name=(string)_listSeriesName[j];
					((Series)_listSeriesMedia[(string)_listSeriesName[j]]).ChartArea=chartArea2.Name;
					#endregion

					#region Titre
					this.Titles.Add(chartArea2.Name);
                    this.Titles[iterator].DockInsideChartArea = true;
                    this.Titles[iterator].Position.Auto = false;
                    this.Titles[iterator].Position.X = 45;
                    this.Titles[iterator].Position.Y = 3 + ((96 / _listSeriesMedia.Count) * iterator);
                    _style.GetTag("MediaStrategyGraphTitleFont").SetStyleDundas(this.Titles[iterator]);
                    this.Titles[iterator].DockToChartArea = chartArea2.Name;
					#endregion

					#region Type image
					((Series)_listSeriesMedia[(string)_listSeriesName[j]]).Label="#PERCENT : #VALX";
					((Series)_listSeriesMedia[(string)_listSeriesName[j]])["3DLabelLineSize"]="50";
					#endregion
				
					#region Positionnement du graphique
					chartArea2.Position.Width = 80;
                    chartArea2.Position.Y = 3 + (((96 / _listSeriesMedia.Count) * iterator) + 1);
					chartArea2.Position.Height = (96/_listSeriesMedia.Count)-1;
					chartArea2.Position.X=4;
					#endregion

                    iterator++;				

					#region Ajout des dans la série
					this.Series.Add(((Series)_listSeriesMedia[(string)_listSeriesName[j]]));	
					#endregion

					yPosition+=chartArea2.Position.Height;
				}
			}

			#region Dimensionnement de l'image
			// Taille d'un graphique * Nombre de graphique
			double imgLength=(MEDIA_STRATEGY_HEIGHT_GRAPHIC*_listSeriesMedia.Count);
			#endregion

			#endregion
	
			#endregion			

		}
		#endregion

    }
}
