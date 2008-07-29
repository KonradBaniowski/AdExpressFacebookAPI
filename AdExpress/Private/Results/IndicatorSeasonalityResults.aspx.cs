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
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
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
	///- totalChoice : le top 10 des annonceurs et références ayant le plus investi sur la période sélectionnée
	///en fonction du média.
	///- Nouveautés : nouveaux annonceurs ou références sur la période sélectionnée.
	///- Evolution : annonceurs et références dont l'investissement a le plus augmenté entre la période N-1 et N.
	///- Stratégie Média : répartion des investissements par média et par annonceurs et références.
	/// </summary>
	public partial class IndicatorSeasonalityResults :  TNS.AdExpress.Web.UI.ResultWebPage {

		#region variables
		/// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result ;
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Choix du type de total (bas sélection, famille, marché)
		/// </summary>
        public bool totalChoice = true;
		/// <summary>
		/// Taille de l'image dans un plus gd format
		/// </summary>
		public bool bigFormat=false;
		/// <summary>
		/// Commentaire Agrandissement de l'image
		/// </summary>
		public string zoomTitle="";
		/// <summary>
		/// Affiche les graphiques
		/// </summary>
		public bool displayChart=false;

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
				if(_nextUrl.Length!=0){
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
				#endregion
			
				#region Textes et Langage du site
                for (int i = 0; i < this.Controls.Count; i++) {
                    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                }
				ResultsOptionsWebControl1.ChartTitle=GestionWeb.GetWebWord(1191,_webSession.SiteLanguage);
				ResultsOptionsWebControl1.TableTitle=GestionWeb.GetWebWord(1192,_webSession.SiteLanguage);
				zoomTitle=GestionWeb.GetWebWord(1235,_webSession.SiteLanguage);
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion

				if(!IsPostBack){
					#region Création de totalRadioButtonList
					totalRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1188,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()));
					totalRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1189,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()));
					totalRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1190,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal.GetHashCode().ToString()));
					totalRadioButtonList.Items[0].Selected=true;
					totalRadioButtonList.CssClass="txtNoir11";
					#endregion
					ResultsOptionsWebControl1.GraphRadioButton.Checked=_webSession.Graphics;
					ResultsOptionsWebControl1.TableRadioButton.Checked=!_webSession.Graphics;
				}else{
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
					totalRadioButtonList.Items.Remove(totalRadioButtonList.Items.FindByValue("2"));
				}
				#endregion

				#region Ajout du total univers
				if((_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.NOVELTY)
					&& _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS){
					try{
						if(totalRadioButtonList.Items.FindByValue("2").Value!=null){
					
						}
					}catch(Exception){
						totalRadioButtonList.Items.Insert(0,new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1188,_webSession.SiteLanguage),TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()));
					
					}
				}
				#endregion
			
				#region radioButton sélectionné
				if( _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS){
					try{
						totalRadioButtonList.Items.FindByValue(totalRadioButtonList.SelectedItem.Value).Selected=true;
					}
					catch(Exception){
						if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.NOVELTY){
							totalRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()).Selected=true;
						}
						else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY){
							totalRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()).Selected=true;
						}				
					}
				}
				#endregion
		
				#region ComparaisonCriterion	
				if(_webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION && _webSession.CurrentTab!=TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS){
					if(totalRadioButtonList.Items.FindByValue(totalRadioButtonList.SelectedItem.Value).Value==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()){
						_webSession.ComparaisonCriterion=TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;
					}
					else if(totalRadioButtonList.Items.FindByValue(totalRadioButtonList.SelectedItem.Value).Value==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()){
						_webSession.ComparaisonCriterion=TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
					}
					else if(totalRadioButtonList.Items.FindByValue(totalRadioButtonList.SelectedItem.Value).Value==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal.GetHashCode().ToString()){
						_webSession.ComparaisonCriterion=TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal;
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
				if(ResultsOptionsWebControl1.GraphRadioButton.Checked){
					displayChart=true;
				}
				try{
					//Résultats Synthèse
					if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS){
                        totalChoice = false;										
						//TODO désactiver les ofmats jpeg dans les tableaux d'indicateurs
						ResultsOptionsWebControl1.MediaDetailOption=false;
						advertiserChart.Visible=false;
						referenceChart.Visible=false;						
						ResultsOptionsWebControl1.ResultFormat=false;	
						InitializeProductWebControl1.Visible=false;
						result = TNS.AdExpress.Web.UI.Results.IndicatorSynthesisUI.GetIndicatorSynthesisHtmlUI(_webSession,false,false);
					}
						//Résulats SAISONNALITE
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Seasonality.SEASONALITY && ResultsOptionsWebControl1.TableRadioButton.Checked){
						totalChoice=true;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						result=TNS.AdExpress.Web.UI.Results.IndicatorSeasonalityUI.GetIndicatorSeasonalityHtmlUI(this,IndicatorSeasonalityRules.GetFormattedTable(_webSession),_webSession,false);
					}
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Seasonality.SEASONALITY && ResultsOptionsWebControl1.GraphRadioButton.Checked){
						totalChoice=true;	
						ResultsOptionsWebControl1.MediaDetailOption=false;
						bigFormat=true;
						advertiserChart.Visible=true;
						advertiserChart.SeasonalityLine(_webSession,bigFormatCheckBox.Checked,true);
						if(advertiserChart.Visible==false)
							result=noResult("");
					}
					
						//Résulats totalChoice
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES && ResultsOptionsWebControl1.TableRadioButton.Checked){
						totalChoice=true;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						result=TNS.AdExpress.Web.UI.Results.IndicatorPalmaresUI.GetAllPalmaresIndicatorUI(_webSession,string.Empty,false);
					}
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES && ResultsOptionsWebControl1.GraphRadioButton.Checked){
						totalChoice=true;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						advertiserChart.Visible=true;
						referenceChart.Visible=true;
						advertiserChart.PalmaresBar(_webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear,FrameWorkConstantes.PalmaresRecap.ElementType.advertiser,true);
						referenceChart.PalmaresBar(_webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear,FrameWorkConstantes.PalmaresRecap.ElementType.product,true);
						if(advertiserChart.Visible==false)
							result=noResult("");
					}
						
						//Résulats NOUVEAUTES
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Novelty.NOVELTY && ResultsOptionsWebControl1.TableRadioButton.Checked){
						totalChoice= true;
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
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Novelty.NOVELTY && ResultsOptionsWebControl1.GraphRadioButton.Checked){	
						totalChoice=false;
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
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION && ResultsOptionsWebControl1.TableRadioButton.Checked){					
						totalChoice=false;
						ResultsOptionsWebControl1.MediaDetailOption=false;
						result=TNS.AdExpress.Web.UI.Results.IndicatorEvolutionUI.GetAllEvolutionIndicatorUI(_webSession,false);
					}
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION && ResultsOptionsWebControl1.GraphRadioButton.Checked){	 
					
						totalChoice=false;
						ResultsOptionsWebControl1.MediaDetailOption=false;
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
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.MEDIA_STRATEGY && ResultsOptionsWebControl1.TableRadioButton.Checked){
						totalChoice=true;	
						result = TNS.AdExpress.Web.UI.Results.IndicatorMediaStrategyUI.GetIndicatorMediaStrategyHtmlUI(Page,TNS.AdExpress.Web.Rules.Results.IndicatorMediaStrategyRules.GetFormattedTable(_webSession,_webSession.ComparaisonCriterion),_webSession,false);											
						
					}
						// Partie graphique
					else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.MediaStrategy.MEDIA_STRATEGY && ResultsOptionsWebControl1.GraphRadioButton.Checked){					
						// si WebSession est au niveau media on se met au niveau categorie/media
						if(_webSession.PreformatedMediaDetail==TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle && (ClassificationCst.DB.Vehicles.names)((LevelInformation) _webSession.SelectionUniversMedia.FirstNode.Tag).ID!=ClassificationCst.DB.Vehicles.names.plurimedia){
							_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
							_webSession.Save();
						}
						totalChoice=true;
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
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;		
			InitializeProductWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
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
				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY && ResultsOptionsWebControl1.GraphRadioButton.Checked){
				switch((ClassificationCst.DB.Vehicles.names)((LevelInformation) _webSession.SelectionUniversMedia.FirstNode.Tag).ID){
					case ClassificationCst.DB.Vehicles.names.tv:
					case ClassificationCst.DB.Vehicles.names.radio:
					case ClassificationCst.DB.Vehicles.names.outdoor:
					case ClassificationCst.DB.Vehicles.names.mediasTactics:
					case ClassificationCst.DB.Vehicles.names.mobileTelephony:
					case ClassificationCst.DB.Vehicles.names.emailing:
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
