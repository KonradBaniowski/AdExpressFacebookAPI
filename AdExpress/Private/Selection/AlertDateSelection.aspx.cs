#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//	19/12/2004 A. Obermeyer Intégration de WebPage
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
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Customer;
using AdExpressWebControles=TNS.AdExpress.Web.Controls;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Domain.Web.Navigation;
using DateDll = TNS.FrameWork.Date;
using AdExpressException=AdExpress.Exceptions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de sélection de dates par jour
	/// Cette Page est utilisée pour les alertes
	/// </summary>
	public partial class AlertDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Variables MMI
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
		/// <summary>texte</summary>
		/// <summary>texte</summary>
		/// <summary></summary>
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
		/// Contrôle choix N derniers jours
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Formulaire principale
		/// </summary>
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
		/// Main Menu
		/// </summary>
		/// <summary>
		/// Text option jour précédente
		/// </summary>
		public string previousDay;		
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public AlertDateSelection():base(){
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
//						//if(_webSession.isDatesSelected())linkToShow.Add(5);
//						recallWebControl.LinkToShow=linkToShow;
//						if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isCompetitorMediaSelected())recallWebControl.CanGoToResult=true;
//						break;
//					default:
//						throw(new AdExpress.Exceptions.AdExpressException("Le Module n'est pas géré"));
//				}
			
				#endregion				

				#region Textes et langage du site
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				validateButton1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				validateButton2.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton2.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				previousMonth = GestionWeb.GetWebWord(788,_webSession.SiteLanguage);
				previousWeek = GestionWeb.GetWebWord(789,_webSession.SiteLanguage);
				previousDay = GestionWeb.GetWebWord(1975,_webSession.SiteLanguage);
				// Gestion des Calendrier
				this.dayCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.dayCalendarEndWebControl.Language=_webSession.SiteLanguage;

				#endregion
		
				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"AlertDateSelectionHelp.aspx";
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
			if (this.IsPostBack){
				#region Url Suivante
				
				if(_nextUrlOk){
					if(selectedIndex==6 ||selectedIndex==7) validateButton1_Click(this, null);
					else validateButton2_Click(this, null);
				}
				#endregion
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
		override protected void OnInit(EventArgs e)
		{
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
		/// DeterminePostBackMode
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
//			recallWebControl.CustomerWebSession=_webSession;
			weekDateList.WebSession=_webSession;
			monthDateList.WebSession=_webSession;
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
			try{
				DateDll.AtomicPeriodWeek week;
				switch(selectedIndex){
					case 1:
						_webSession.PeriodType=CstPeriodType.nLastMonth;
						_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);
						_webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");;
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
				Response.Write("<script language=\"JavaScript\">alert(\""+ex.Message+"\");</script>");
			}
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
				_webSession.Save();
			}
			else{
				throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(886,_webSession.SiteLanguage)));
			}
		}
		#endregion

		#endregion

		#region Implémentation méthodes abstraites
		/// <summary>
		/// Event launch to fire validation of the page
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.Page_PreRender(sender,e);
		}
		/// <summary>
		/// Retrieve next Url from the menu
		/// </summary>
		/// <returns>Next Url</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

	}
}
