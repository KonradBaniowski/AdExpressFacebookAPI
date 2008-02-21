#region Information
/*
 * auteur : D. V. MUSSUMA
 * créé le : 30/09/2004
 * modifié le : 
 *	30/12/2004  D. Mussuma Intégration de WebPage
 * */
#endregion

#region Namespace
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.Core.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using FrameWorkConstantes= TNS.AdExpress.Constantes.FrameWork.Results;
using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
#endregion

namespace AdExpress.Private.Results {
	
	/// <summary>
	///Résultas des différentes planches indicateurs :
	///- Saisonnalité : répartition des investissements par média et par annonceurs ou références.
	///- Palmares : le top 10 des annonceurs et références ayant le plus investi sur la période sélectionnée
	///en fonction du média.
	///- Nouveautés : nouveaux annonceurs ou références sur la période sélectionnée.
	///- Evolution : annonceurs et références dont l'investissement a le plus augmenté entre la période N-1 et N.
	///- Stratégie Média : répartion des investissements par média et par annonceurs et références.
	/// </summary>
	public partial class IndicatorSeasonalityResults :  TNS.AdExpress.Web.UI.ResultWebPage {

		#region variables
		/// <summary>
		/// choix affichage total marché
		/// </summary>
		public bool market;
		/// <summary>
		/// choix affichage total marché famille
		/// </summary>
		public bool sector;		
		/// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result ;
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// planche palmares
		/// </summary>
		public bool palmares=true;
		/// <summary>
		/// Taille de l'image dans un plus gd format
		/// </summary>
		public bool bigFormat=false;
		/// <summary>
		/// Commentaire Présentation graphique
		/// </summary>
		public string chartTitle="";
		/// <summary>
		/// Graphique pour les annonceurs
		/// </summary>
		/// <summary>
		/// Graphique pour les références
		/// </summary>
		/// <summary>
		/// Commentaire Présentation tableau
		/// </summary>
		public string tableTitle="";
		/// <summary>
		/// Commentaire Agrandissement de l'image
		/// </summary>
		public string zoomTitle="";
		/// <summary>
		/// Affiche les graphiques
		/// </summary>
		public bool displayChart=false;
		/// <summary>
		/// Affiche le controle pour les graphiques de Stratégie Média
		/// </summary>
		public bool mediaStrategyChart=false;
		/// <summary>
		/// Type de l'élément à trier
		/// </summary>
		string itemType="";
						
		/// <summary>
		/// Vrai si choix du type de résultat (tableau ou graphique)
		/// </summary>
		public bool isShowResultOption=false;
		#endregion

		#region variables MMI
		/// <summary>
		/// Contrôle Titre du module
		/// </summary>
		/// <summary>
		/// Contrôle Options des résultats
		/// </summary>
		/// <summary>
		/// Bouton de validation
		/// </summary>
		/// <summary>
		/// Contrôle passerelle vers les autres modules
		/// </summary>
//		/// <summary>
//		/// Contrôle export des résultats
//		/// </summary>
//		protected TNS.AdExpress.Web.Controls.Headers.ExportWebControl ExportWebControl1;
		/// <summary>
		/// Contrôle menu d'entête 
		/// </summary>
		/// <summary>
		/// Choix du type d'univers d'étude (famille,univers,marché)
		/// </summary>
//		/// <summary>
//		/// Choix affichage des résultas ous forme graphique ou tableau
//		/// </summary>
//		protected System.Web.UI.WebControls.RadioButtonList graphRadioButtonList;
//		/// <summary>
//		/// Choix affichage graphique
//		/// </summary>		
//		protected System.Web.UI.WebControls.RadioButton graphRadioButton;
//		/// <summary>
//		/// Choix affichage tableau
//		/// </summary>
//		protected System.Web.UI.WebControls.RadioButton tableRadioButton;
		/// <summary>
		/// Zoom sur le graphique affiché
		/// </summary>		
		/// <summary>
		/// annule la personnalisation des éléments de références ou concurrents
		/// </summary>
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public IndicatorSeasonalityResults():base(){
			// Chargement de la Session			
			_webSession.CurrentModule = WebConstantes.Module.Name.INDICATEUR;
			_webSession.Unit = WebConstantes.CustomerSessions.Unit.kEuro;
							
		}
		#endregion		

		#region Evènements

		#region chargement de la page
		/// <summary>
		/// Chargement de la page
		/// Suivant l'indicateur choisi une méthode contenue dans UI est appelé
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e) {
			
			try{				
				#region Gestion du flash d'attente
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=MenuWebControl2.ID){
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
					}
				}
				else{
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
				}
				#endregion

				#region Url Suivante
				//				_nextUrl=this.recallWebControl.NextUrl;

				if(_nextUrl.Length!=0){
					DBFunctions.closeDataBase(_webSession);
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
				#endregion
			
				#region Textes et Langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				//				 chartTitle=GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);	
				ResultsOptionsWebControl1.ChartTitle=GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
				//				tableTitle=GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);
				ResultsOptionsWebControl1.TableTitle=GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);
				zoomTitle=GestionWeb.GetWebWord(1235,_webSession.SiteLanguage);
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion

				#region Définition de la page d'aide
				//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"IndicatorSeasonalityResultsHelp.aspx";
				#endregion

				if(!IsPostBack){
					#region Création de palmaresRadioButtonList
					palmaresRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1188,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()));
					palmaresRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1189,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()));
					palmaresRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1190,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal.GetHashCode().ToString()));
					palmaresRadioButtonList.Items[0].Selected=true;
					palmaresRadioButtonList.CssClass="txtNoir11";
					#endregion
					//					graphRadioButton.Checked=_webSession.Graphics;
					ResultsOptionsWebControl1.GraphRadioButton.Checked=_webSession.Graphics;
					//					tableRadioButton.Checked=!_webSession.Graphics;
					ResultsOptionsWebControl1.TableRadioButton.Checked=!_webSession.Graphics;
				}else{
					//					_webSession.Graphics=graphRadioButton.Checked;
					if( _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS
						&& (!ResultsOptionsWebControl1.GraphRadioButton.Checked && !ResultsOptionsWebControl1.TableRadioButton.Checked)
						){
						ResultsOptionsWebControl1.GraphRadioButton.Checked=_webSession.Graphics;
						ResultsOptionsWebControl1.TableRadioButton.Checked=!_webSession.Graphics;
					}
					else _webSession.Graphics = ResultsOptionsWebControl1.GraphRadioButton.Checked;
					_webSession.Save();
				}				

				#region Suppression du total univers
				if((_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY)
					&& _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS
					){
					palmaresRadioButtonList.Items.Remove(palmaresRadioButtonList.Items.FindByValue("2"));
				}
				#endregion

				#region Ajout du total univers
				if((_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.NOVELTY)
					&& _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS){
					try{
						if(palmaresRadioButtonList.Items.FindByValue("2").Value!=null){
					
						}
					}catch(Exception){
						palmaresRadioButtonList.Items.Insert(0,new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1188,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()));
					
					}
				}
				#endregion
			
				#region radioButton sélectionné
				if( _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS){
					try{
						palmaresRadioButtonList.Items.FindByValue(palmaresRadioButtonList.SelectedItem.Value).Selected=true;
					}
					catch(Exception){
						if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.NOVELTY){
							palmaresRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()).Selected=true;
						}
						else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY){
							palmaresRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()).Selected=true;
						}				
					}
				}
				#endregion
		
				#region ComparaisonCriterion	
				if(_webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION && _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS){
					if(palmaresRadioButtonList.Items.FindByValue(palmaresRadioButtonList.SelectedItem.Value).Value==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()){
						_webSession.ComparaisonCriterion=TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;
					}
					else if(palmaresRadioButtonList.Items.FindByValue(palmaresRadioButtonList.SelectedItem.Value).Value==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()){
						_webSession.ComparaisonCriterion=TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
						//Pour saisonnalité
						this.sector=true;
						this.market=false;
					}
					else if(palmaresRadioButtonList.Items.FindByValue(palmaresRadioButtonList.SelectedItem.Value).Value==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal.GetHashCode().ToString()){
						_webSession.ComparaisonCriterion=TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal;
						//Pour saisonnalité
						this.sector=false;
						this.market=true;
					}
				}
				_webSession.Save();
 
				#endregion
				
				#region Résultat
				//Code html des résultats				
				advertiserChart.Visible=false;
				referenceChart.Visible=false;						
				
				ResultsOptionsWebControl1.ResultFormat=true;
				InitializeProductWebControl1.Visible=true;
				//				if(graphRadioButton.Checked){
				if(ResultsOptionsWebControl1.GraphRadioButton.Checked){
					displayChart=true;
				}
				try{
					//Résultats Synthèse
					if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS){
						palmares=false;										
						//TODO désactiver les ofmats jpeg dans les tableaux d'indicateurs
						//						ExportWebControl1.JpegFormatFromWebPage=false;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						advertiserChart.Visible=false;
						referenceChart.Visible=false;						
						ResultsOptionsWebControl1.ResultFormat=false;	
						InitializeProductWebControl1.Visible=false;
						result = TNS.AdExpress.Web.UI.Results.IndicatorSynthesisUI.GetIndicatorSynthesisHtmlUI(_webSession,false,false);
					}
						//Résulats SAISONNALITE
						//					if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY && tableRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Seasonality.SEASONALITY && ResultsOptionsWebControl1.TableRadioButton.Checked){
						palmares=true;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						//						ExportWebControl1.JpegFormatFromWebPage=false;
						result=TNS.AdExpress.Web.UI.Results.IndicatorSeasonalityUI.GetIndicatorSeasonalityHtmlUI(this,IndicatorSeasonalityRules.GetFormattedTable(_webSession),_webSession,false);
					}
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY && graphRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Seasonality.SEASONALITY && ResultsOptionsWebControl1.GraphRadioButton.Checked){
						palmares=true;	
						ResultsOptionsWebControl1.MediaDetailOption=false;
						//						ExportWebControl1.JpegFormatFromWebPage=true;
						bigFormat=true;
						advertiserChart.Visible=true;
						advertiserChart.SeasonalityLine(_webSession,bigFormatCheckBox.Checked,true);
						if(advertiserChart.Visible==false)
							result=noResult("");
					}
					
						//Résulats PALMARES
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES && tableRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES && ResultsOptionsWebControl1.TableRadioButton.Checked){
						palmares=true;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						//						ExportWebControl1.JpegFormatFromWebPage=false;
						result=TNS.AdExpress.Web.UI.Results.IndicatorPalmaresUI.GetAllPalmaresIndicatorUI(_webSession,itemType,false);
					}
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES && graphRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES && ResultsOptionsWebControl1.GraphRadioButton.Checked){
						palmares=true;
						//						ExportWebControl1.JpegFormatFromWebPage=true;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						advertiserChart.Visible=true;
						referenceChart.Visible=true;
						advertiserChart.PalmaresBar(_webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear,FrameWorkConstantes.PalmaresRecap.ElementType.advertiser,true);
						referenceChart.PalmaresBar(_webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear,FrameWorkConstantes.PalmaresRecap.ElementType.product,true);
						if(advertiserChart.Visible==false)
							result=noResult("");
					}
						
						//Résulats NOUVEAUTES
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.NOVELTY && tableRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Novelty.NOVELTY && ResultsOptionsWebControl1.TableRadioButton.Checked){
						palmares= true;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						string resultAdvertiser="";
						result="<center>"+TNS.AdExpress.Web.UI.Results.IndicatorNoveltyUI.GetIndicatorNoveltyHtmlUI(this,IndicatorNoveltyRules.GetFormattedTable(_webSession,ConstResults.Novelty.ElementType.product),_webSession,ConstResults.Novelty.ElementType.product,false);			
						if(result.Length<30){
							result="<center>"+noResult(result);
						}
						resultAdvertiser="<br>"+TNS.AdExpress.Web.UI.Results.IndicatorNoveltyUI.GetIndicatorNoveltyHtmlUI(this,IndicatorNoveltyRules.GetFormattedTable(_webSession,ConstResults.Novelty.ElementType.advertiser),_webSession,ConstResults.Novelty.ElementType.advertiser,false)+"</center>";
						if(resultAdvertiser.Length<50){
							result+="<br>"+noResult(resultAdvertiser)+"</center>";
						}else{
							result+=resultAdvertiser;
						}				
					}
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.NOVELTY && graphRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Novelty.NOVELTY && ResultsOptionsWebControl1.GraphRadioButton.Checked){	
						palmares=false;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						string resultAdvertiser=""; 
						result=TNS.AdExpress.Web.UI.Results.IndicatorNoveltyUI.GetIndicatorNoveltyGraphicHtmlUI(IndicatorNoveltyRules.GetFormattedTable(_webSession,ConstResults.Novelty.ElementType.product),_webSession,ConstResults.Novelty.ElementType.product);
						if(result.Length<30){
							result="<center>"+noResult(result);
						}
						resultAdvertiser+="<br>"+TNS.AdExpress.Web.UI.Results.IndicatorNoveltyUI.GetIndicatorNoveltyGraphicHtmlUI(IndicatorNoveltyRules.GetFormattedTable(_webSession,ConstResults.Novelty.ElementType.advertiser),_webSession,ConstResults.Novelty.ElementType.advertiser);		
						if(resultAdvertiser.Length<50){
							result+=noResult(resultAdvertiser)+"</center>";
						}
						else{
							result+=resultAdvertiser;
						}
				
					}

						//Résulats EVOLUTION
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION && tableRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION && ResultsOptionsWebControl1.TableRadioButton.Checked){					
						palmares=false;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						//						ExportWebControl1.JpegFormatFromWebPage=false;
						result=TNS.AdExpress.Web.UI.Results.IndicatorEvolutionUI.GetAllEvolutionIndicatorUI(_webSession,false);
					}
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION && graphRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION && ResultsOptionsWebControl1.GraphRadioButton.Checked){	 
					
						palmares=false;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						//						ExportWebControl1.JpegFormatFromWebPage=true;
						//Cas année N-2
						DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
						if((PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-2) && DateTime.Now.Year<=_webSession.DownLoadDate)
							|| (PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-3) && DateTime.Now.Year>_webSession.DownLoadDate)
							) {
							result=TNS.AdExpress.Web.UI.Results.IndicatorEvolutionUI.GetAllEvolutionIndicatorUI(_webSession,false);
							advertiserChart.Visible=false;
							referenceChart.Visible=false;
						}else{
	

							advertiserChart.Visible=true;
							referenceChart.Visible=true;
							advertiserChart.EvolutionBar(_webSession,FrameWorkConstantes.EvolutionRecap.ElementType.advertiser,true);
							referenceChart.EvolutionBar(_webSession,FrameWorkConstantes.EvolutionRecap.ElementType.product,true);
							if(advertiserChart.Visible==false)
								result=noResult("");
					
						}
					}
						
						//Résulats STRATEGIE MEDIA
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY && tableRadioButton.Checked){
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.MEDIA_STRATEGY && ResultsOptionsWebControl1.TableRadioButton.Checked){
						palmares=true;	
						//						ExportWebControl1.JpegFormatFromWebPage=false;
						result = TNS.AdExpress.Web.UI.Results.IndicatorMediaStrategyUI.GetIndicatorMediaStrategyHtmlUI(Page,TNS.AdExpress.Web.Rules.Results.IndicatorMediaStrategyRules.GetFormattedTable(_webSession,_webSession.ComparaisonCriterion),_webSession,false);											
						
					}
						// Partie graphique
						//					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY && graphRadioButton.Checked){					
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.MEDIA_STRATEGY && ResultsOptionsWebControl1.GraphRadioButton.Checked){					
						// si WebSession est au niveau media on se met au niveau categorie/media
						if(_webSession.PreformatedMediaDetail==TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle && (ClassificationCst.DB.Vehicles.names)((LevelInformation) _webSession.SelectionUniversMedia.FirstNode.Tag).ID!=ClassificationCst.DB.Vehicles.names.plurimedia){
							_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
							_webSession.Save();
						}
						palmares=true;
						//						ExportWebControl1.JpegFormatFromWebPage=true;
						advertiserChart.Visible=true;
						referenceChart.Visible=false;
						advertiserChart.MediaStrategyBar(_webSession,true);					
					}	
					
				}catch(System.Exception){
					advertiserChart.Visible=false;
					referenceChart.Visible=false;
					result=noResult("");				
				}
				#endregion								
			
				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.Save();
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ModuleBridgeWebControl1.CustomerWebSession=_webSession;
//			ExportWebControl1.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;		
//			recallWebControl.CustomerWebSession=_webSession;
			InitializeProductWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
//			bool fromSearchSession=false;
//			if(Request.UrlReferrer!=null && Request.UrlReferrer.AbsolutePath.IndexOf("SearchSession")>0)
//				fromSearchSession = true;
//
//			if(!Page.IsPostBack && !fromSearchSession)
//				_webSession.CurrentTab = FrameWorkConstantes.SynthesisRecap.SYNTHESIS;//Règle : planche synthèse par défaut dans le module indicateurs
			return tmp;
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){
           
		}
		#endregion

		#region Aucun Résultat
		/// <summary>
		/// Affichage d'un message d'erreur
		/// </summary>
		/// <returns></returns>
		private string noResult(string message){
			System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
			t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
			t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
			t.Append(GestionWeb.GetWebWord(177,_webSession.SiteLanguage)+" "+message);
			t.Append("</td></tr></table>");
			return t.ToString();
		} 
		#endregion

		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output){
			// Suppime les items inutiles de la list "niveau media" pour le graphe de strategie Media
//			if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY && graphRadioButton.Checked){
				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY && ResultsOptionsWebControl1.GraphRadioButton.Checked){
				switch((ClassificationCst.DB.Vehicles.names)((LevelInformation) _webSession.SelectionUniversMedia.FirstNode.Tag).ID){
					case ClassificationCst.DB.Vehicles.names.tv:
					case ClassificationCst.DB.Vehicles.names.radio:
					case ClassificationCst.DB.Vehicles.names.outdoor:
					case ClassificationCst.DB.Vehicles.names.mediasTactics:
						ResultsOptionsWebControl1.mediaDetail.Items.Remove(ResultsOptionsWebControl1.mediaDetail.Items.FindByText(GestionWeb.GetWebWord(1141, _webSession.SiteLanguage)));
						break;
					case ClassificationCst.DB.Vehicles.names.press:
					case ClassificationCst.DB.Vehicles.names.internet:
						ResultsOptionsWebControl1.mediaDetail.Items.Remove(ResultsOptionsWebControl1.mediaDetail.Items.FindByText(GestionWeb.GetWebWord(1141, _webSession.SiteLanguage)));
						break;
					case ClassificationCst.DB.Vehicles.names.cinema:
						result=noResult("");
						advertiserChart.Visible=false;

						break;
					default:						
						ResultsOptionsWebControl1.mediaDetail.Enabled=false;
						break;
				}				
			}
			base.Render(output);
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Get next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return this.MenuWebControl2.NextUrl;
		}
		#endregion

	}
}
