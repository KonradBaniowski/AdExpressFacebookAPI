#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//	31/12/2004 A. Obermeyer Intégration de WebPage
//	01/08/2006 Modification FindNextUrl
#endregion

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
using Oracle.DataAccess.Client;
using System.Globalization;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.UI.Selections.Periods;
using AdExpressWebControles=TNS.AdExpress.Web.Controls;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Domain.Web.Navigation;
using DateDll = TNS.FrameWork.Date;
using AdExpressException=AdExpress.Exceptions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.Selection {
	/// <summary>
	/// Sélection des dates pour l'alerte portefeuille
	/// </summary>
	public partial class PortofolioDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Variables MMI
		/// <summary>
		/// Formulaire principale
		/// </summary>
		/// <summary>
		/// Entete
		/// </summary>
		/// <summary>
		/// Titre et description de la page
		/// </summary>
		/// <summary>
		/// Bouton de validation de la sélection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Bouton de validation de la sélection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Titre sélection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Titre de la partie sélection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Description de la partie avec sauvegarde
		/// </summary>
		/// <summary>Texte</summary>
		/// <summary>Texte</summary>
		/// <summary>Texte</summary>
		/// <summary>
		/// Période nLastMonth
		/// </summary>
		/// <summary>
		/// Période nLastWeek
		/// </summary>
		/// <summary>
		/// Période calendaire de début
		/// </summary>
		/// <summary>
		/// Période Calendaire finale
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Controle N derniers jours
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Text option année précédente
		/// </summary>
		public string previousMonth;
		/// <summary>
		/// Text option semaine précédente
		/// </summary>
		public string previousWeek;		
		/// <summary>
		/// Index de l'élément date sélectioné
		/// </summary>
		int selectedIndex=-1;
		/// <summary>
		/// Liste des médias
		/// </summary>
		public string listMedia="";
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Text option jour précédente
		/// </summary>
		public string previousDay;		
		#endregion
	
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PortofolioDateSelection():base(){			
		}
		#endregion

		#region Evènements

		#region Chargement de la page

		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void Page_Load(object sender, System.EventArgs e){

			try{

				#region Option de période sélectionnée
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				#endregion

				#region Next URL
//				_nextUrl=this.recallWebControl.NextUrl;
//				if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
//				else{
//					_nextUrlOk=true;
//				}
				#endregion

				if(Request.Form.Get("__EVENTTARGET")=="link"){
					_webSession.PeriodType=WebConstantes.CustomerSessions.Period.Type.dateToDate;
					_webSession.Save();
					validateLink(Request.Form.Get("__EVENTARGUMENT"));			
				}

				#region Rappel des différentes sélections
//				ArrayList linkToShow=new ArrayList();
//				switch(int.Parse(this._webSession.CurrentModule.ToString())){
//					case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA:
//					case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
//						//linkToShow.Add(1);
//						if(_webSession.isAdvertisersSelected() || _webSession.isCompetitorAdvertiserSelected())linkToShow.Add(2);
//						if(_webSession.isMediaSelected())linkToShow.Add(3);
//						//if(_webSession.isDatesSelected())linkToShow.Add(4);
//						recallWebControl.LinkToShow=linkToShow;
//						if(_webSession.LastReachedResultUrl.Length>0 && (_webSession.isAdvertisersSelected()||_webSession.isCompetitorAdvertiserSelected()) && _webSession.isMediaSelected())recallWebControl.CanGoToResult=true;
//						break;
//					case WebConstantes.Module.Name.ALERTE_DYNAMIQUE:
//					case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
//					case WebConstantes.Module.Name.ALERTE_POTENTIELS :										 
//						if(_webSession.isMediaSelected())linkToShow.Add(3);						
//						recallWebControl.LinkToShow=linkToShow;
//						if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isCompetitorMediaSelected())recallWebControl.CanGoToResult=true;
//						break;
//						case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE :
//							if(_webSession.isMediaSelected())linkToShow.Add(3);						
//							recallWebControl.LinkToShow=linkToShow;
//							if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isMediaSelected())recallWebControl.CanGoToResult=true;
//						break;
//					default:
//						throw(new AdExpress.Exceptions.AdExpressException("Le Module n'est pas géré"));
//				}
				#endregion				

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress			
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				//validateButton1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validateButton1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				//validateButton2.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validateButton2.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				previousMonth = GestionWeb.GetWebWord(788,_webSession.SiteLanguage);
				previousWeek = GestionWeb.GetWebWord(789,_webSession.SiteLanguage);
				previousDay = GestionWeb.GetWebWord(1975,_webSession.SiteLanguage);
				// Gestion des Calendrier
				this.dayCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.dayCalendarEndWebControl.Language=_webSession.SiteLanguage;
				#endregion
		
				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"PortofolioDateSelectionHelp.aspx";
				#endregion

				listMedia=PortofolioDateUI.ListMedia(_webSession,((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID,((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID,DateString.dateTimeToYYYYMMDD_HH24_MI_SS(_webSession.BeginningDate.AddMonths(-4)).Substring(0,8),DateString.dateTimeToYYYYMMDD_HH24_MI_SS(_webSession.BeginningDate).Substring(0,8));

				#region Script
				// Ouverture des div
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}

				#endregion
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region PréRendu de la page

		/// <summary>
		/// Evènement de PréRendu
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void Page_PreRender(object sender, System.EventArgs e) {
			try{
				if (this.IsPostBack){
					#region Url Suivante
				
					if(_nextUrlOk){
						if(selectedIndex==6 ||selectedIndex==7) validateButton1_Click(this, null);
						else validateButton2_Click(this, null);
					}
					#endregion
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void Page_UnLoad(object sender, System.EventArgs e) {
		}
		#endregion

		#region Initialisation

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
            
            this.Unload += new System.EventHandler(this.Page_UnLoad);
          
		}
		#endregion

		#endregion

		#region DeterminePostBack
		/// <summary>
		/// DeterminePostBack
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;
			monthDateList.WebSession=_webSession;
			weekDateList.WebSession=_webSession;
			dayDateList.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		#region Validation des calendriers
		/// <summary>
		/// Evènement de validation des calendriers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void validateButton1_Click(object sender, System.EventArgs e) {
			try{
				calendarValidation();
				_webSession.Save();
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(System.Exception ex){
				Response.Write("<script language=\"JavaScript\">alert(\""+ex.Message+"\");</script>");
			}
		
		}
		#endregion

		#region Validation des dates avec sauvegarde
		/// <summary>
		/// Evènement de validation des dates avec sauvegarde
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void validateButton2_Click(object sender, System.EventArgs e) {
			string endDate;
			try{
				DateDll.AtomicPeriodWeek week;				
				switch(selectedIndex){
					case 1:
						_webSession.PeriodType=CstPeriodType.nLastMonth;
						_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);
						_webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");;
						endDate=TNS.AdExpress.Web.DataAccess.Selections.Periods.PortofolioDateDataAccess.LastPublication(_webSession,long.Parse(_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess)),long.Parse(_webSession.GetSelection(_webSession.ReferenceUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess)));
						
						if(endDate!=null && endDate.Length>0)
							_webSession.PeriodEndDate = endDate.Substring(0,endDate.Length-2);
						else
							_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");

						_webSession.DetailPeriod = CstPeriodDetail.dayly;
						break;
					case 2:
						_webSession.PeriodType=CstPeriodType.nLastWeek;
						_webSession.PeriodLength=int.Parse(weekDateList.SelectedValue);
						week = new DateDll.AtomicPeriodWeek(DateTime.Now);
						_webSession.PeriodEndDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
						week.SubWeek(_webSession.PeriodLength-1);
						_webSession.PeriodBeginningDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
						_webSession.DetailPeriod = CstPeriodDetail.dayly;
						break;
					case 4:
						_webSession.PeriodType=CstPeriodType.previousMonth;
						_webSession.PeriodLength=1;
						_webSession.PeriodEndDate=_webSession.PeriodBeginningDate=DateTime.Now.AddMonths(-1).ToString("yyyyMM");
						_webSession.DetailPeriod = CstPeriodDetail.dayly;
						break;
					case 5:
						_webSession.PeriodType=CstPeriodType.previousWeek;
						_webSession.PeriodLength=1;
						week = new DateDll.AtomicPeriodWeek(DateTime.Now.AddDays(-7));
						_webSession.PeriodBeginningDate=_webSession.PeriodEndDate=week.Year.ToString() + ((week.Week>9)?"":"0")+week.Week.ToString();
						_webSession.DetailPeriod = CstPeriodDetail.dayly;
						break;

						//N derniers jours
					case 8 :
						_webSession.PeriodType=CstPeriodType.nLastDays;
						_webSession.PeriodLength=int.Parse(dayDateList.SelectedValue);
						_webSession.PeriodBeginningDate = DateTime.Now.AddDays(1 - _webSession.PeriodLength).ToString("yyyyMMdd");;
						_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");
						_webSession.DetailPeriod = CstPeriodDetail.dayly;
						break;

						//Jour précédent
					case 9 :
						_webSession.PeriodType=CstPeriodType.previousDay;
						_webSession.PeriodLength=2;						
						_webSession.PeriodBeginningDate = _webSession.PeriodEndDate =DateTime.Now.AddDays(1 - _webSession.PeriodLength).ToString("yyyyMMdd");;
						_webSession.DetailPeriod = CstPeriodDetail.dayly;
						break;
					default:
						throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				_webSession.Save();
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(System.Exception ex){ 
				Response.Write("<script language=\"JavaScript\">alert(\""+ex.Message.Replace("\r\n"," ")+"\");</script>");
			}
		}
		#endregion

		#region validation des dates avec les liens
		/// <summary>
		/// Cas d'un click sur un lien
		/// </summary>
		/// <param name="date"></param>
		private void validateLink(string date){
			_webSession.DetailPeriod=CstPeriodDetail.dayly;
			_webSession.PeriodBeginningDate=date;
			_webSession.PeriodEndDate=date;
			_webSession.Save();
			_webSession.Source.Close();
			Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);		
		}
		#endregion

		#endregion

		#region Méthodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation(){
			// On vérifie que 2 dates ont été sélectionnées
			if(dayCalendarBeginWebControl.isDateSelected() && dayCalendarEndWebControl.isDateSelected()){
				// On teste que la date de début est inférieur à la date de fin
				if(dayCalendarBeginWebControl.SelectedDate>dayCalendarEndWebControl.SelectedDate)
					throw(new AdExpressException.AnalyseDateSelectionException("La date de début doit être inférieure à la date de fin"));
				// On sauvegarde les données
				_webSession.DetailPeriod =TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
				_webSession.PeriodType=TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
				_webSession.PeriodBeginningDate = dayCalendarBeginWebControl.SelectedDate.ToString();
				_webSession.PeriodEndDate = dayCalendarEndWebControl.SelectedDate.ToString();	
			}
			else{
				throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(886,_webSession.SiteLanguage)));
			}
		}
		#endregion

		#endregion

		#region Implémentation méthodes abstraites
		/// <summary>
		/// Validation de la sélection de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.Page_PreRender(sender,e);
		}

		/// <summary>
		/// Obtient l'url suivante
		/// </summary>
		/// <returns>Url suivante</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

	}
}
