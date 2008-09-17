#region Information
//Author : Y. Rkaina 
//Creation : 19/07/2006
#endregion

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections;

using TNS.AdExpress.Common;

using TNS.AdExpress.Constantes.Web;
using CstUI = TNS.AdExpress.Constantes.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;

using TNS.AdExpress.Anubis.Hotep.Common;
using TNS.AdExpress.Anubis.Hotep.Exceptions;

using Dundas.Charting.WinControl;
using TNS.FrameWork.DB.Common;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork;
using TblFormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

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
		private WebSession _webSession = null;
		/// <summary>
		/// Data Source
		/// </summary>
		private IDataSource _dataSource = null;
		/// <summary>
		/// Hotep configuration
		/// </summary>
		private HotepConfig _config = null;
		/// <summary>
		/// Tableau d'objets qui contient les résultats
		/// </summary>
		private object[,] _tab=null;
		
		#endregion
		
		#region Constructeur
		public UIMediaStrategyGraph(WebSession webSession,IDataSource dataSource, HotepConfig config,object[,] tab):base(){
		_webSession = webSession;
		_dataSource = dataSource;
		_config = config;
		_tab = tab;
		}
		#endregion
		
		#region MediaStrategy
		/// <summary>
		/// Graphiques Media Strategy
		/// </summary>
		internal void BuildMediaStrategy(){
			
			#region Constantes
			Color[] pieColors={
								  Color.FromArgb(100,72,131),
								  Color.FromArgb(177,163,193),
								  Color.FromArgb(208,200,218),
								  Color.FromArgb(225,224,218),
								  Color.FromArgb(255,215,215),
								  Color.FromArgb(255,240,240),
								  Color.FromArgb(202,255,202)};

			const int NBRE_MEDIA=5;
			
			/// <summary>
			/// Hauteur d'un graphique stratégie média
			/// </summary>
			const int MEDIA_STRATEGY_HEIGHT_GRAPHIC=300;
			#endregion
					
			#region Variables
			ChartArea chartArea=new ChartArea();
			// Il y a au moins un élément
			//bool oneProductExist=false;
			#endregion
			
			this.ChartAreas.Add(chartArea);
			
			#region Niveau de détail
			int MEDIA_LEVEL_NUMBER;
			switch(_webSession.PreformatedMediaDetail){
				case TblFormatCst.PreformatedMediaDetails.vehicle:
					MEDIA_LEVEL_NUMBER = 1;
					break;
				case TblFormatCst.PreformatedMediaDetails.vehicleCategory:
					MEDIA_LEVEL_NUMBER = 2;
					break;
				default:
					MEDIA_LEVEL_NUMBER = 3;
					break;
			}
			#endregion

			#region Chart
			this.Size = new Size(800,500);
			this.BackGradientType = GradientType.TopBottom;
			this.BorderLineColor = Color.FromKnownColor(KnownColor.LightGray);
			//	this.ChartAreas[strChartArea].BackColor=Color.FromArgb(222,207,231);			
			this.BorderStyle=ChartDashStyle.Solid;
			this.BorderLineColor=Color.FromArgb(99,73,132);
			this.BorderLineWidth=2;
			this.Legend.Enabled=false;
			#endregion		

			#region Parcours de tab
			
			#region Variables
			double totalUniversValue=0;
			double totalSectorValue=0;
			double totalMarketValue=0;
			double oldValueRefCompetitor=0;
			double elementValue;
			int i,j=0;
			Hashtable listSeriesMedia=new Hashtable();
			Hashtable listSeriesName=new Hashtable();
			Hashtable listSeriesMediaRefCompetitor=new Hashtable();
			Hashtable listTableRefCompetitor=new Hashtable();		
			DataTable tableUnivers=new DataTable();
			DataTable tableSectorMarket=new DataTable();
			#endregion

			// Définition des colonnes
			tableUnivers.Columns.Add("Name");
			tableUnivers.Columns.Add("Position",typeof(double));
			tableSectorMarket.Columns.Add("Name");
			tableSectorMarket.Columns.Add("Position",typeof(double));

			
			// Serie Univers
			listSeriesMedia.Add(GestionWeb.GetWebWord(1780,_webSession.SiteLanguage),new Series());
			listSeriesName.Add(0,GestionWeb.GetWebWord(1780,_webSession.SiteLanguage));
			// Serie Famille
			if((_webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal)
				||(_webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)){
				listSeriesMedia.Add(GestionWeb.GetWebWord(1189,_webSession.SiteLanguage),new Series());
				listSeriesName.Add(1,GestionWeb.GetWebWord(1189,_webSession.SiteLanguage));
				// Serie Marché
			}
			else{
				listSeriesMedia.Add(GestionWeb.GetWebWord(1316,_webSession.SiteLanguage),new Series());
				listSeriesName.Add(1,GestionWeb.GetWebWord(1316,_webSession.SiteLanguage));
			}


			// Création des séries (une série par média) que l'on place dans la hashTable listSeriesMedia
			for(i=1;i<_tab.GetLongLength(0);i++){			

				//	HashTable avec comme clé le libéllé de l'annonceur référence ou concurrent et comme valeur le total
				if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null){
					if(listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]==null){
						listSeriesMediaRefCompetitor.Add(_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(),new double());
					}

					if(listTableRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]==null){
						DataTable tableCompetitorRef=new DataTable();
						tableCompetitorRef.Columns.Add("Name");
						tableCompetitorRef.Columns.Add("Position",typeof(double));
						listTableRefCompetitor.Add(_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(),tableCompetitorRef);

					}

					if(listSeriesMedia[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]==null){
						listSeriesMedia.Add(_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(),new Series());
					}
				}
			}	
	

			#region Totaux
			// Création des totaux
			if(MEDIA_LEVEL_NUMBER ==2 || MEDIA_LEVEL_NUMBER==3){
				for(i=1;i<_tab.GetLongLength(0);i++){			
					for( j=0;j<FrameWorkConstantes.Results.MediaStrategy.NB_MAX_COLUMNS;j++){
						switch(j){

								#region support
								// Total Univers
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){									
									totalUniversValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX].ToString());
								}							
								break;
								// Total Famille
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){
									totalSectorValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								// Total Marché
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){
									totalMarketValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								#endregion

							case FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
									&& _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]!=null
									&&  _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
									&& MEDIA_LEVEL_NUMBER==3){
									oldValueRefCompetitor=(double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()];
									listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]=oldValueRefCompetitor+double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString());
								}
								else if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
									&& _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX]!=null
									&&  _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
									&& _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]==null
									&& MEDIA_LEVEL_NUMBER==2){
									oldValueRefCompetitor=(double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()];
									listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]=oldValueRefCompetitor+double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString());
								}
								break;


								#region Category

								// Total Univers	
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
									totalUniversValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								// Total Famille
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
									totalSectorValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								// Total Marché	
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
						
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
									totalMarketValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX].ToString());
								}

								break;
							


								#endregion														


							default:
								break;
						}
					}
				}
			}

			#region PluriMedia
			if(MEDIA_LEVEL_NUMBER==1){
				for(i=0;i<_tab.GetLongLength(0);i++){			
					for( j=0;j<FrameWorkConstantes.Results.MediaStrategy.NB_MAX_COLUMNS;j++){
						switch(j){			
						
								// Total Univers
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX]!=null ){
									totalUniversValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX].ToString());
								}							
								break;
								// Total Famille
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]!=null){
									totalSectorValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX].ToString());
								}
								break;
								// Total Marché
							case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]!=null){
									totalMarketValue+=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX].ToString());
								}
								break;

							case FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
								if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
									&&  _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
									&&  _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX]!=null){
									oldValueRefCompetitor=(double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()];
									listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]=oldValueRefCompetitor+double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString());
								}
								
								break;
							
							default:
								break;
						}
					}
				}
			}
			#endregion

			#endregion
			
			// Parcours de l'arbre pour la création des histogrammes
			for( i=1;i<_tab.GetLongLength(0);i++){			
				for( j=0;j<FrameWorkConstantes.Results.MediaStrategy.NB_MAX_COLUMNS;j++){
					switch(j){

							#region Media
							//Univers niveau Media 
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){								
								
								if(totalUniversValue!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX].ToString())/totalUniversValue*100;
									DataRow row=tableUnivers.NewRow();
									row["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString();
									row["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableUnivers.Rows.Add(row);
								}
									
								j=j+6;
							}
														
							break;
							// Famille niveau media
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){
								
								if(totalSectorValue!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX].ToString())/totalSectorValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);

								}
								j=j+5;
							}							
							break;

							// Marché niveau Media
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==3){
									
									
								if(totalMarketValue!=0){							
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX].ToString())/totalMarketValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);
								}
								j=j+4;
							}
							break;
							#endregion

						case FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
								&& _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]!=null
								&& _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
								&& MEDIA_LEVEL_NUMBER==3){
								
								if((double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString())/(double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]*100;
									DataRow row1=((DataTable)listTableRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									((DataTable)listTableRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).Rows.Add(row1);
								}
								
								j=j+12;
							}
							else	if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
								&& _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX]!=null
								&& _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]==null
								//&& tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX]==null
								&&  _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
								&& MEDIA_LEVEL_NUMBER==2){
								
								if((double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString())/(double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]*100;
									DataRow row1=((DataTable)listTableRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									((DataTable)listTableRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).Rows.Add(row1);
								}
								
								j=j+12;
							}
							else if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null 
								&&  _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null
								&&  _tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX]!=null
								&& MEDIA_LEVEL_NUMBER==1){
							
								if((double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString())/(double)listSeriesMediaRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]*100;
									DataRow row1=((DataTable)listTableRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									((DataTable)listTableRefCompetitor[_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()]).Rows.Add(row1);

								}
							
								j=j+12;							
							}
							break;							
						
							#region Categorie 
							// Univers niveau categorie
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){								
								
								if(totalUniversValue!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX].ToString())/totalUniversValue*100;
									DataRow row=tableUnivers.NewRow();
									row["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString();
									row["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableUnivers.Rows.Add(row);
								}
								
							}							
							break;
							// Famille niveau category
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
								if(totalSectorValue!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX].ToString())/totalSectorValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);
								}
								
							}							
							break;
						
							// Marché niveau categorie
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]!=null && MEDIA_LEVEL_NUMBER==2){
								
								if(totalMarketValue!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX].ToString())/totalMarketValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);
								}
							}				

							break;
						
							#endregion

							#region PluriMedia
							//Univers  
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]!=null && i>1){								
								if(totalUniversValue!=0){	
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX].ToString())/totalUniversValue*100;
									DataRow row=tableUnivers.NewRow();
									row["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString();
									row["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableUnivers.Rows.Add(row);	
								}
							}														
							break;
							// Famille 
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]!=null && i>1){
								if(totalSectorValue!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX].ToString())/totalSectorValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);								
								}
							}							
							break;

							// Marché 
						case FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:
							if(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]!=null && i>1){
								if(totalMarketValue!=0){
									elementValue=double.Parse(_tab[i,FrameWorkConstantes.Results.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX].ToString())/totalMarketValue*100;
									DataRow row1=tableSectorMarket.NewRow();
									row1["Name"]=_tab[i,FrameWorkConstantes.Results.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString();
									row1["Position"]=double.Parse(elementValue.ToString("0.00"));
									tableSectorMarket.Rows.Add(row1);
								}
							}
							break;

							#endregion

						default:
							break;
					}
				}
			}
			string strSort = "Position  DESC";
			DataRow[] foundRows=null;
			foundRows=tableUnivers.Select("",strSort);
		
		

			DataRow[] foundRowsSectorMarket=null;
			foundRowsSectorMarket=tableSectorMarket.Select("",strSort);
			double[]  yValues=new double[foundRows.Length];
			string[]    xValues=new string[foundRows.Length];
			#region ancienne version (modifié par Dédé)
			//			double[] yValuesSectorMarket=new double[foundRows.Length];
			//			string[]  xValuesSectorMarket=new string[foundRows.Length];
			#endregion
			double[] yValuesSectorMarket=new double[foundRowsSectorMarket.Length];
			string[]  xValuesSectorMarket=new string[foundRowsSectorMarket.Length];
			double otherUniversValue=0;
			double otherSectorMarketValue=0;

			if( MEDIA_LEVEL_NUMBER!=1 ){
				for(i=0;i<5 && i<foundRows.Length;i++){
					xValues[i]=(string)foundRows[i]["Name"];
					yValues[i]=double.Parse(foundRows[i]["Position"].ToString());
					otherUniversValue+=double.Parse(foundRows[i]["Position"].ToString());
				}
				if(foundRows.Length>NBRE_MEDIA){				
					xValues[i]=GestionWeb.GetWebWord(647,_webSession.SiteLanguage);
					yValues[i]=100-otherUniversValue;			
				}

				for(i=0;i<5 && i<foundRowsSectorMarket.Length ;i++){
					xValuesSectorMarket[i]=(string)foundRowsSectorMarket[i]["Name"];
					yValuesSectorMarket[i]=double.Parse(foundRowsSectorMarket[i]["Position"].ToString());
					otherSectorMarketValue+=double.Parse(foundRowsSectorMarket[i]["Position"].ToString());
				}
				#region ancienne version (modifié par Dédé)
				//				if(foundRows.Length>NBRE_MEDIA){
				#endregion
				if(foundRowsSectorMarket.Length>NBRE_MEDIA){
					xValuesSectorMarket[i]=GestionWeb.GetWebWord(647,_webSession.SiteLanguage);
					yValuesSectorMarket[i]=100-otherSectorMarketValue;			
				}
			}
				// Cas PluriMedia
			else{
				for(i=0; i<foundRows.Length;i++){
					xValues[i]=(string)foundRows[i]["Name"];
					yValues[i]=double.Parse(foundRows[i]["Position"].ToString());
					otherUniversValue+=double.Parse(foundRows[i]["Position"].ToString());
				}				

				for(i=0; i<foundRowsSectorMarket.Length ;i++){
					xValuesSectorMarket[i]=(string)foundRowsSectorMarket[i]["Name"];
					yValuesSectorMarket[i]=double.Parse(foundRowsSectorMarket[i]["Position"].ToString());
					otherSectorMarketValue+=double.Parse(foundRowsSectorMarket[i]["Position"].ToString());
				}			
			}
				
			double[]  yVal=new double[foundRows.Length];
			string[]    xVal=new string[foundRows.Length];
			double otherCompetitorRefValue=0;
			j=2;

			foreach(string name in listSeriesMedia.Keys){
				
				if(name==GestionWeb.GetWebWord(1780,_webSession.SiteLanguage)){ 
					if(xValues!=null && xValues.Length>0 && xValues[0]!=null)
						((Series)listSeriesMedia[GestionWeb.GetWebWord(1780,_webSession.SiteLanguage)]).Points.DataBindXY(xValues,yValues);
				}
				else if(((_webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal)||(_webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)) && name==GestionWeb.GetWebWord(1189,_webSession.SiteLanguage)){
					if(xValuesSectorMarket!=null && xValuesSectorMarket.Length>0 && xValuesSectorMarket[0]!=null)
						((Series)listSeriesMedia[GestionWeb.GetWebWord(1189,_webSession.SiteLanguage)]).Points.DataBindXY(xValuesSectorMarket,yValuesSectorMarket);
				}
				else if(name==GestionWeb.GetWebWord(1316,_webSession.SiteLanguage)){
					if(xValuesSectorMarket!=null  && xValuesSectorMarket.Length>0 && xValuesSectorMarket[0]!=null)
						((Series)listSeriesMedia[GestionWeb.GetWebWord(1316,_webSession.SiteLanguage)]).Points.DataBindXY(xValuesSectorMarket,yValuesSectorMarket);
				}
				else{
					DataRow[] foundRowsCompetitorRef=null;
					foundRowsCompetitorRef=((DataTable)listTableRefCompetitor[name]).Select("",strSort);
					otherCompetitorRefValue=0;
					
					yVal=new double[foundRowsCompetitorRef.Length];
					xVal=new string[foundRowsCompetitorRef.Length];
					if( MEDIA_LEVEL_NUMBER!=1 ){
						for(i=0;i<foundRowsCompetitorRef.Length && i<NBRE_MEDIA ;i++){
							
							
							xVal[i]=(string)foundRowsCompetitorRef[i]["Name"];
							yVal[i]=double.Parse(foundRowsCompetitorRef[i]["Position"].ToString());
							
							otherCompetitorRefValue+=double.Parse(foundRowsCompetitorRef[i]["Position"].ToString());
				
						}
						if(foundRowsCompetitorRef.Length>NBRE_MEDIA){				
							xVal[i]="Autres";
							yVal[i]=100-otherCompetitorRefValue;			
						}
					}
						// PluriMedia
					else{
						for(i=0;i<foundRowsCompetitorRef.Length ;i++){
							xVal[i]=(string)foundRowsCompetitorRef[i]["Name"];
							yVal[i]=double.Parse(foundRowsCompetitorRef[i]["Position"].ToString());
							
						}
					}
					if(xVal.Length>0 && xVal[0]!=null)
						((Series)listSeriesMedia[name]).Points.DataBindXY(xVal,yVal);


					listSeriesName.Add(j,name);
					j++;
				}
			
			}
			float yPosition=0.0F;
			#region Affichage des graphiques
			i=0;
			for(j=0;j<listSeriesMedia.Count;j++){
				if(((Series)listSeriesMedia[(string)listSeriesName[j]]).Points.Count>0){
					
										
					#region Type de Graphique
					((Series)listSeriesMedia[(string)listSeriesName[j]]).Type= SeriesChartType.Pie;
					#endregion
				
					#region Définition des couleurs
					for(int k=0;k<6&&k<((Series)listSeriesMedia[(string)listSeriesName[j]]).Points.Count;k++){
						((Series)listSeriesMedia[(string)listSeriesName[j]]).Points[k].Color=pieColors[k];
					}
					#endregion
				
					#region Légende
					((Series)listSeriesMedia[(string)listSeriesName[j]])["LabelStyle"]="Outside";
					((Series)listSeriesMedia[(string)listSeriesName[j]]).LegendToolTip = "#PERCENT";
					((Series)listSeriesMedia[(string)listSeriesName[j]]).ToolTip = " "+(string)listSeriesName[j]+" \n #VALX : #PERCENT";
					((Series)listSeriesMedia[(string)listSeriesName[j]])["PieLineColor"]="Black";
					#endregion

					#region Création et définition du graphique
					ChartArea chartArea2=new ChartArea();
					this.ChartAreas.Add(chartArea2);
					chartArea2.Area3DStyle.Enable3D = true; 
					chartArea2.Name=(string)listSeriesName[j];
					//					TextAnnotation ta=new TextAnnotation();
					//					ta.AllowTextEditing=false;
					//					ta.Text=chartArea2.Name;
					//					ta.X=10;
					//					ta.Y=yPosition;
					//					this.Annotations.Add(ta);
					((Series)listSeriesMedia[(string)listSeriesName[j]]).ChartArea=chartArea2.Name;
					#endregion

					#region Titre
					this.Titles.Add(chartArea2.Name);
					this.Titles[i].DockInsideChartArea=true;
					this.Titles[i].Position.Auto = false;
					this.Titles[i].Position.X=45;
					this.Titles[i].Position.Y=3+((96/listSeriesMedia.Count)*i);
					this.Titles[i].Font=new Font("Arial", (float)13);
					this.Titles[i].Color=Color.FromArgb(100,72,131);
					this.Titles[i].DockToChartArea=chartArea2.Name;
					#endregion

					#region Type image
					//if(!typeFlash){
					((Series)listSeriesMedia[(string)listSeriesName[j]]).Label="#PERCENT : #VALX";
					((Series)listSeriesMedia[(string)listSeriesName[j]])["3DLabelLineSize"]="50";
					//}
					#endregion
				
					#region Positionnement du graphique
					chartArea2.Position.Width = 80; 
					chartArea2.Position.Y=3+(((96/listSeriesMedia.Count)*i)+1);
					chartArea2.Position.Height = (96/listSeriesMedia.Count)-1;
					chartArea2.Position.X=4;
					#endregion
					
					i++;				

					#region Ajout des dans la série
					this.Series.Add(((Series)listSeriesMedia[(string)listSeriesName[j]]));	
					#endregion

					yPosition+=chartArea2.Position.Height;
				}
			}

			#region Dimensionnement de l'image
			// Taille d'un graphique * Nombre de graphique
			double imgLength=(MEDIA_STRATEGY_HEIGHT_GRAPHIC*listSeriesMedia.Count);
			//chartArea.Position.Height=new Unit(imgLength);
			#endregion
			#endregion
	
			#endregion			
			

		}
		#endregion

	}
}
