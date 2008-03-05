#region Informations
// Auteur: D. Mussuma
// Date de création: 29/01/2007
// Date de modification: 
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

namespace AdExpress.Private.Selection {
	/// <summary>
	/// Page de sélection de dates par jour ou mois ou semaines
	/// Cette Page est utilisée pour les justrificatifs Presse
	/// </summary>
	public partial class ProofDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{
	
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
		/// Text option année précédente
		/// </summary>
		public string previousYear;	
		/// <summary>
		/// Text option année courante
		/// </summary>
		public string currentYear;	
		/// <summary>
		/// Index de l'élément date sélectioné
		/// </summary>
		int selectedIndex=-1;
		
					
		#endregion
		
		#region Variables MMI
		/// <summary>
		/// Entete
		/// </summary>
		/// <summary>
		/// Titre et description de la page
		/// </summary>
		/// <summary>
		/// Information
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
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText Adexpresstext1;
		/// <summary>
		/// Menu clic droit
		/// </summary>
		#endregion


		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public ProofDateSelection():base(){
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try{

				#region Option de période sélectionnée
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				#endregion															

				#region Textes et langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				validateButton1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				validateButton2.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton2.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				previousMonth = GestionWeb.GetWebWord(788,_webSession.SiteLanguage);
				previousWeek = GestionWeb.GetWebWord(789,_webSession.SiteLanguage);
				previousYear = GestionWeb.GetWebWord(787,_webSession.SiteLanguage);
				currentYear = GestionWeb.GetWebWord(1119,_webSession.SiteLanguage);
				// Gestion des Calendrier
				this.dayCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.dayCalendarEndWebControl.Language=_webSession.SiteLanguage;

				#endregion				

				//Annuler l'univers de version 
				_webSession.IdSlogans = new ArrayList();
				_webSession.SloganIdZoom=-1;
				_webSession.Save();
				

			}
			catch(System.Exception exc) {
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
		private void InitializeComponent() {
           
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
			weekDateList.WebSession=_webSession;
			monthDateList.WebSession=_webSession;
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
				DBFunctions.closeDataBase(_webSession);
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
						//N derniers mois
					case 1:
						_webSession.PeriodType=CstPeriodType.nLastMonth;
						_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);
						_webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");;
						_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						break;
						//N dernieres semaines
					case 2:
						_webSession.PeriodType=CstPeriodType.nLastWeek;
						_webSession.PeriodLength=int.Parse(weekDateList.SelectedValue);
						week = new DateDll.AtomicPeriodWeek(DateTime.Now);
						_webSession.PeriodEndDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
						week.SubWeek(_webSession.PeriodLength-1);
						_webSession.PeriodBeginningDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
						_webSession.DetailPeriod = CstPeriodDetail.weekly;
						break;
						//Mois précédent
					case 4:
						_webSession.PeriodType=CstPeriodType.previousMonth;
						_webSession.PeriodLength=1;
						_webSession.PeriodEndDate=_webSession.PeriodBeginningDate=DateTime.Now.AddMonths(-1).ToString("yyyyMM");
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						break;
						//Semaine précédente
					case 5:
						_webSession.PeriodType=CstPeriodType.previousWeek;
						_webSession.PeriodLength=1;
						week = new DateDll.AtomicPeriodWeek(DateTime.Now.AddDays(-7));
						_webSession.PeriodBeginningDate=_webSession.PeriodEndDate=week.Year.ToString() + ((week.Week>9)?"":"0")+week.Week.ToString();
						_webSession.DetailPeriod = CstPeriodDetail.weekly;
						break;
						//Année précédente
					case 3:
						_webSession.PeriodType=CstPeriodType.previousYear;
						_webSession.PeriodLength=1;
						_webSession.PeriodBeginningDate=DateTime.Now.AddYears(-1).ToString("yyyy01");
						_webSession.PeriodEndDate=DateTime.Now.AddYears(-1).ToString("yyyy12");
						_webSession.DetailPeriod = CstPeriodDetail.monthly;					
						break;
						//Année courante
					case 8:
						_webSession.PeriodType=CstPeriodType.currentYear;
						_webSession.PeriodLength=1;
						_webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
						_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
						_webSession.DetailPeriod = CstPeriodDetail.monthly;	
						break;
					
					default:
						throw(new AdExpressException.SponsorshipDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				_webSession.Save();
				DBFunctions.closeDataBase(_webSession);
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
					throw(new AdExpressException.SponsorshipDateSelectionException("La date de début doit être inférieure à la date de fin"));
				// On sauvegarde les données
				_webSession.DetailPeriod =TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly;
				_webSession.PeriodType=TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
				_webSession.PeriodBeginningDate = dayCalendarBeginWebControl.SelectedDate.ToString();
				_webSession.PeriodEndDate = dayCalendarEndWebControl.SelectedDate.ToString();
				_webSession.Save();
			}
			else{
				throw(new AdExpressException.SponsorshipDateSelectionException(GestionWeb.GetWebWord(886,_webSession.SiteLanguage)));
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

