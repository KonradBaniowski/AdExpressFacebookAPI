#region Informations
// Auteur: K. Shehzad && D. V. Mussuma
// Date de création: 04/07/2004
//	01/08/2006 Modification FindNextUrl
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
using SelectionsGrp=TNS.AdExpress.Web.BusinessFacade.Selections.Grp;
#endregion

namespace AdExpress.Private.Selection{
	/// <summary>
	///Page de sélection des dates pour le module APPM.
	/// </summary>
	public partial class APPMDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

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
		/// Titre sélection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Titre de la partie sélection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Description de la partie avec sauvegarde
		/// </summary>
		/// <summary>
		/// Contrôle txt n dernieres annees
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText yearAdExpressText;
		/// <summary>
		/// Contrôle txt n derniers mois
		/// </summary>
		/// <summary>
		/// Contrôle txt n dernieres semaines
		/// </summary>
		/// <summary>
		/// Contrôle txt "ou bien sur"
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText lastPeriodAdexpresstext;
//		/// <summary>
//		/// nombre d'année à prendre en compte dans les cas N dernières années
//		/// </summary>
//		protected TNS.AdExpress.Web.Controls.Selections.DateListWebControl yearDateList;
		/// <summary>
		/// nombre de mois à prendre en compte dans les cas N dernièrs mois
		/// </summary>
		/// <summary>
		/// nombre de semaine à prendre en compte dans les cas N dernières semaines
		/// </summary>
		/// <summary>
		///  Contrôle calendrier date de début de période
		/// </summary>
		/// <summary>
		///  Contrôle calendrier date de fin de période
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		#endregion
		
		#region Variables
		/// <summary>
		/// Option de période sélectionné
		/// </summary>
		int selectedIndex=-1;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public  APPMDateSelection():base(){
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
//				else
//				{
//					_nextUrlOk=true;
//				}
				#endregion

				#region Rappel des différentes sélections
//				ArrayList linkToShow=new ArrayList();				
//				if(_webSession.isAdvertisersSelected() || _webSession.isCompetitorAdvertiserSelected())linkToShow.Add(2);
//				recallWebControl.LinkToShow=linkToShow;				
				#endregion

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;

				validateButton1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				validateButton2.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton2.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
////			previousYear = GestionWeb.GetWebWord(787,_webSession.SiteLanguage);
//				previousMonth = GestionWeb.GetWebWord(788,_webSession.SiteLanguage);
//				previousWeek = GestionWeb.GetWebWord(789,_webSession.SiteLanguage);

				// Gestion des Calendrier
				this.lastMonthWeekCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.lastMonthWeekCalendarEndWebControl.Language=_webSession.SiteLanguage;		
				#endregion

				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"APPMDateSelectionHelp.aspx";
				#endregion

				//Annuler l'univers de version car les cibles changent en fonction de la période sélectionnée				
					_webSession.IdSlogans = new ArrayList();
					_webSession.SloganIdZoom=-1;
					_webSession.Save();
				
				if(_webSession.IsTargetSelected()){
					ArrayList forbidForbidOptionPagesList = new ArrayList();
					forbidForbidOptionPagesList.Add(2); //Pas accès direct à la page choix des produits lorsqu'on change de date, pour éviter de passer directement à la page de résultat via cette dernière.
					MenuWebControl2.ForbidOptionPagesList = forbidForbidOptionPagesList;
				}
				


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
		protected void Page_PreRender(object sender, System.EventArgs e){
			try{
				if (this.IsPostBack){
					if(lastMonthWeekCalendarBeginWebControl.isDateSelected()){
						switch(lastMonthWeekCalendarBeginWebControl.SelectedDateType){
							case CstPeriodType.dateToDateMonth:
								lastMonthWeekCalendarEndWebControl.DisplayType=AdExpressWebControles.Selections.LastMonthWeekCalendarWebControl.Display.month;
								break;
							case CstPeriodType.dateToDateWeek:
								lastMonthWeekCalendarEndWebControl.DisplayType=AdExpressWebControles.Selections.LastMonthWeekCalendarWebControl.Display.week;
								break;
							default:
								lastMonthWeekCalendarBeginWebControl.DisplayType=AdExpressWebControles.Selections.LastMonthWeekCalendarWebControl.Display.all;
								break;
						}
					}
					#region Url Suivante	
					if(_nextUrlOk){
						if(selectedIndex==6 ||selectedIndex==7) validateButton1_Click(this, null);
						else validateButton2_Click(this, null);
					}
					#endregion
				}
			}
			catch(System.Exception exc) {
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
			if(_webSession.CustomerLogin.Connection!=null){
				if(_webSession.CustomerLogin.Connection.State==System.Data.ConnectionState.Open)_webSession.CustomerLogin.Connection.Close();
				_webSession.CustomerLogin.Connection.Dispose();
			}
			_webSession.Save();
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// On l'utilise pour l'initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;
			//			yearDateList.WebSession=_webSession;
			monthDateList.WebSession=_webSession;
			weekDateList.WebSession=_webSession;
			monthDateList.ModuleType = weekDateList.ModuleType =  WebConstantes.Module.Type.chronoPress;
			lastMonthWeekCalendarBeginWebControl.CustomerWebSession = _webSession;
			lastMonthWeekCalendarEndWebControl.CustomerWebSession=_webSession;
			lastMonthWeekCalendarBeginWebControl.NumberOfLastMonth = 16;
			lastMonthWeekCalendarEndWebControl.NumberOfLastMonth = 16;
			MenuWebControl2.CustomerWebSession = _webSession;
			MenuWebControl2.ForbidResultIcon = true; // Interdit de passer directement à la page de résultats depuis cette page car choix des vagues et cibles foncitons de la période sélectionnée.
			
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
				_webSession.SelectionUniversAEPMWave.Nodes.Clear();
				_webSession.SelectionUniversAEPMWave.Nodes.Add(SelectionsGrp.WavesSystem.GetWaves(_webSession,this._dataSource));
				if(_webSession.GetSelection(_webSession.SelectionUniversAEPMWave,Right.type.aepmWaveAccess).Length==0)
					throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(1765,_webSession.SiteLanguage)));
				_webSession.Save();
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
					case 1:
						if(int.Parse(monthDateList.SelectedValue)!=0){
							_webSession.PeriodType=CstPeriodType.nLastMonth;
							_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);
							_webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");;
							_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
							_webSession.DetailPeriod = CstPeriodDetail.monthly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;
					case 2:
						if(int.Parse(weekDateList.SelectedValue)!=0){
							_webSession.PeriodType=CstPeriodType.nLastWeek;
							_webSession.PeriodLength=int.Parse(weekDateList.SelectedValue);
							week = new DateDll.AtomicPeriodWeek(DateTime.Now);
							_webSession.PeriodEndDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
							week.SubWeek(_webSession.PeriodLength-1);
							_webSession.PeriodBeginningDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
							_webSession.DetailPeriod = CstPeriodDetail.weekly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;					
					default:
						throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				_webSession.SelectionUniversAEPMWave.Nodes.Clear();
				_webSession.SelectionUniversAEPMWave.Nodes.Add(SelectionsGrp.WavesSystem.GetWaves(_webSession,this._dataSource));
			if(_webSession.GetSelection(_webSession.SelectionUniversAEPMWave,Right.type.aepmWaveAccess).Length==0)
				throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(1765,_webSession.SiteLanguage)));
				_webSession.Save();
				DBFunctions.closeDataBase(_webSession);
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(AdExpressException.AnalyseDateSelectionException ex){
				Response.Write("<script language=\"JavaScript\">alert('"+ex.Message+"');</script>");
			}
		}
		#endregion

		#endregion
		
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Argument</param>
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
		private void InitializeComponent()
		{
            
            this.Unload += new System.EventHandler(this.Page_UnLoad);
          
		}
		#endregion
		
		#region Méthodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation(){
			// On vérifie que 2 dates ont été sélectionnées
			if(lastMonthWeekCalendarBeginWebControl.isDateSelected() && lastMonthWeekCalendarEndWebControl.isDateSelected()){
				// On teste que les deux dates sont du même type
				if(lastMonthWeekCalendarBeginWebControl.SelectedDateType!=lastMonthWeekCalendarEndWebControl.SelectedDateType)
					throw(new AdExpressException.AnalyseDateSelectionException("Les dates sélectionnées ne sont pas du même type"));
				// On teste que la date de début est inférieur à la date de fin
				if(lastMonthWeekCalendarBeginWebControl.SelectedDate>lastMonthWeekCalendarEndWebControl.SelectedDate)
					throw(new AdExpressException.AnalyseDateSelectionException("La date de début doit être inférieure à la date de fin"));
				_webSession.PeriodType =lastMonthWeekCalendarBeginWebControl.SelectedDateType;
				switch(lastMonthWeekCalendarBeginWebControl.SelectedDateType){
					case CstPeriodType.dateToDateMonth:
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						break;
					case CstPeriodType.dateToDateWeek:
						_webSession.DetailPeriod = CstPeriodDetail.weekly;
						break;
					default:
						throw(new AdExpressException.AnalyseDateSelectionException("Le choix de type de sélection de période est incorrect"));
				}
				// On sauvegarde les données
				_webSession.PeriodBeginningDate = lastMonthWeekCalendarBeginWebControl.SelectedDate.ToString();
				_webSession.PeriodEndDate = lastMonthWeekCalendarEndWebControl.SelectedDate.ToString();	
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
