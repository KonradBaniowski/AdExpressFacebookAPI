#region Informations
// Auteur: D. V. Mussuma
// Date de création: 24/02/2005
// Date de modification: 24/02/2005
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
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;

#endregion

namespace AdExpress.Private.Selection
{
	/// <summary>
	/// Description résumée de DashBoardDateSelection.
	/// </summary>
	public partial class DashBoardDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{
		
		#region variables MMI
		/// <summary>
		/// Contrôle titre du module
		/// </summary>
		/// <summary>
		/// Contrôle entête page
		/// </summary>
		/// <summary>
		/// Contrôle texte titre
		/// </summary>
		/// <summary>
		/// Contrôle texte étude coparative
		/// </summary>
		/// <summary>
		/// Contrôle case à cocher étude coparative
		/// </summary>
		/// <summary>
		/// Contrôle bouton validation
		/// </summary>
		/// <summary>
		/// Contrôle texte titre 
		/// </summary>
		/// <summary>
		/// Contrôle rdiio bouton année courante
		/// </summary>
		/// <summary>
		/// Contrôle texte année courante
		/// </summary>
		/// <summary>
		/// Contrôle annnée précédente
		/// </summary>
		/// <summary>
		/// Contrôle texte année précédente
		/// </summary>
		/// <summary>
		/// Contrôle bouton radio année avant année précédente
		/// </summary>
		/// <summary>
		/// Contrôle texte année avant année précédente
		/// </summary>
		/// <summary>
		/// Contrôle texte étude comparative
		/// </summary>
		/// <summary>
		/// Contrôle case à cocher étude comparative
		/// </summary>
		/// <summary>
		/// Contrôle choix début période
		/// </summary>
		/// <summary>
		/// Contrôle choix fin période
		/// </summary>
		/// <summary>
		/// Contrôle bouton valider
		/// </summary>
		/// <summary>
		/// Contrôle derniere semaine chargée
		/// </summary>
		/// <summary>
		/// Contrôle dernier mois chargé
		/// </summary>
		/// <summary>
		/// Texte derniere semaine chargée
		/// </summary>
		/// <summary>
		/// Texte dernier mois chargé
		/// </summary>
		#endregion

		#region variables
		/// <summary>
		/// Index sélectionné
		/// </summary>
		int selectedIndex=-1;
		/// <summary>
		/// Etude de comparative ?
		/// </summary>
		int selectedComparativeStudy=-1;
		/// <summary>
		/// Année de chargement des données
		/// </summary>
		public int downloadDate=0;
		/// <summary>
		/// Date de début de chargemebt des données
		/// </summary>
		private string downloadBeginningDate="";
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Date de fin de chargement des données
		/// </summary>
		private string downloadEndDate="";
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public DashBoardDateSelection():base(){			
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

				#region Options pour la période sélectionnée
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);
				#endregion

				#region Rappel des différentes sélections
//				ArrayList linkToShow=new ArrayList();			
//				if(_webSession.CurrentUniversProduct!=null && _webSession.CurrentUniversProduct.Nodes!=null && _webSession.CurrentUniversProduct.Nodes.Count >0)
//					linkToShow.Add(3);
//				if(_webSession.isMediaSelected())linkToShow.Add(4);
//				recallWebControl.LinkToShow=linkToShow;
//				if(_webSession.LastReachedResultUrl.Length>0 && _webSession.CurrentUniversProduct!=null && _webSession.CurrentUniversProduct.Nodes!=null && _webSession.CurrentUniversProduct.Nodes.Count >0 && _webSession.isMediaSelected())
//					recallWebControl.CanGoToResult=true;
				#endregion

				this.monthWeekCalendarBeginWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.all;
				this.monthWeekCalendarEndWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.all;
				
				this.monthWeekCalendarBeginWebControl.StartYear=DateTime.Now.AddYears(-2).Year;				
				this.monthWeekCalendarEndWebControl.StartYear=DateTime.Now.AddYears(-2).Year;

			
				//ATTaquer une table
				downloadDate=_webSession.DownLoadDate;				

				#region Next URL
//				_nextUrl=this.recallWebControl.NextUrl;
//				if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
//				else {
//					_nextUrlOk=true;
//				}
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
				// Gestion des Calendrier
				this.monthWeekCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.monthWeekCalendarEndWebControl.Language=_webSession.SiteLanguage;	
				
				#endregion

				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"DashBoardDateSelectionHelp.aspx";
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
		protected void Page_PreRender(object sender, System.EventArgs e){
			try{
				if (this.IsPostBack){
//					if(monthWeekCalendarBeginWebControl.isDateSelected()){
//						switch(monthWeekCalendarBeginWebControl.SelectedDateType){
//							case CstPeriodType.dateToDateMonth:
//								monthWeekCalendarEndWebControl.DisplayType=AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.month;
//								break;
//							case CstPeriodType.dateToDateWeek:
//								monthWeekCalendarEndWebControl.DisplayType=AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.week;
//								break;
//							default:
//								monthWeekCalendarEndWebControl.DisplayType=AdExpressWebControles.Selections.MonthWeekCalendarWebControl.Display.all;
//								break;
//						}
//					}
					#region Url Suivante	
					if(_nextUrlOk){
						if(selectedIndex==6 ||selectedIndex==7 || selectedIndex==8) validateButton1_Click(this, null);
						else 
							validateButton2_Click(this, null);
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
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){						
		}
		#endregion

		#region Validation des calendriers
		/// <summary>
		/// Evènement de validation des calendriers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void validateButton1_Click(object sender, System.EventArgs e) {			
			if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);	
			try {
				calendarValidation();
				if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
				else _webSession.ComparativeStudy=false;
				_webSession.Save();
				DBFunctions.closeDataBase(_webSession);
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(System.Exception ex) {
			 	_webSession.ComparativeStudy=false;
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
			//DateDll.AtomicPeriodWeek week;
			if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);				
			try {
				DateTime downloadDate=new DateTime(_webSession.DownLoadDate,12,31);
				
				switch(selectedIndex) {
						//Choix  dernier mois chargé
					case 1:
							_webSession.PeriodType=CstPeriodType.LastLoadedMonth;
							
							WebFunctions.Dates.LastLoadedMonth(ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.nLastMonth);								
							_webSession.PeriodBeginningDate=downloadBeginningDate;						
							_webSession.PeriodEndDate=downloadEndDate;							
							_webSession.PeriodLength=int.Parse(downloadBeginningDate.Substring(4,2));	
							_webSession.DetailPeriod = CstPeriodDetail.monthly;
							//Activation de l'option etude comparative si selectionné
							if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
							else _webSession.ComparativeStudy=false;
							_webSession.Save();
						break;
						//Choix derniere semaine chargée
					case 9 :

							WebFunctions.Dates.LastLoadedWeek(ref downloadBeginningDate,ref downloadEndDate);								
							_webSession.PeriodBeginningDate=downloadBeginningDate;						
							_webSession.PeriodEndDate=downloadEndDate;
							_webSession.PeriodType=CstPeriodType.LastLoadedWeek;

							_webSession.PeriodLength=int.Parse(downloadBeginningDate.Substring(4,2));		
							_webSession.DetailPeriod =  CstPeriodDetail.weekly;
							//Activation de l'option etude comparative si selectionné
							if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
							else _webSession.ComparativeStudy=false;
							_webSession.Save();											
						break;
						//Choix année courante
					case 2:
						_webSession.PeriodType=CstPeriodType.currentYear;
						_webSession.PeriodLength=1;					
							if(DateTime.Now.Month==1){
								throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(1612,_webSession.SiteLanguage));														
							}
							else {
								WebFunctions.Dates.DownloadDates(_webSession,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.currentYear);								
								_webSession.PeriodBeginningDate=downloadBeginningDate;						
								_webSession.PeriodEndDate=downloadEndDate;
							}
					
						
						_webSession.DetailPeriod = CstPeriodDetail.monthly;	
						//Activation de l'option etude comparative si selectionné
						if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						break;
						//Choix année précedente
					case 3:
						_webSession.PeriodType=CstPeriodType.previousYear;
						_webSession.PeriodLength=1;					
						
						WebFunctions.Dates.DownloadDates(_webSession,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.previousYear);								
						_webSession.PeriodBeginningDate=downloadBeginningDate;						
						_webSession.PeriodEndDate=downloadEndDate;
						
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						//Activation de l'option etude comparative si selectionné
						if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						
						break;
						//Choix année N-2
					case 4:
						if(isComparativeStudy(selectedComparativeStudy))throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						_webSession.PeriodType=CstPeriodType.nextToLastYear;
						_webSession.PeriodLength=1;						
						// Cas où l'année de chargement est inférieur à l'année en cours
//						if(DateTime.Now.Year>_webSession.DownLoadDate){
//							_webSession.PeriodBeginningDate=downloadDate.AddYears(-2).ToString("yyyy01");
//							_webSession.PeriodEndDate=downloadDate.AddYears(-2).ToString("yyyy12");
//						}
//						else{
							_webSession.PeriodBeginningDate=DateTime.Now.AddYears(-2).ToString("yyyy01");
							_webSession.PeriodEndDate=DateTime.Now.AddYears(-2).ToString("yyyy12");
//						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						_webSession.ComparativeStudy=false;
						_webSession.Save();
						break;
					default :
						throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				DBFunctions.closeDataBase(_webSession);
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch(System.Exception ex) {
				Response.Write("<script language=\"JavaScript\">alert(\""+ex.Message+"\");</script>");
			}		
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
		private void InitializeComponent() {
          
            this.Unload += new System.EventHandler(this.Page_UnLoad);
          
		}
		#endregion

		#region DeterminePostBack
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
//			recallWebControl.CustomerWebSession=_webSession;
//			monthDateList.WebSession=_webSession;	
//			weekDateList.WebSession = _webSession;
			this.monthWeekCalendarBeginWebControl.BlockIncompleteDates =  true;				
			this.monthWeekCalendarEndWebControl.BlockIncompleteDates = true;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		#region Méthodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation() {
			// On vérifie que 2 dates ont été sélectionnées
			if( monthWeekCalendarBeginWebControl.isDateSelected() && monthWeekCalendarEndWebControl.isDateSelected()) {		
				// On teste que les deux dates sont du même type
				if(monthWeekCalendarBeginWebControl.SelectedDateType!=monthWeekCalendarEndWebControl.SelectedDateType)
					throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(1854,_webSession.SiteLanguage)));
				// On teste que la date de début est inférieur à la date de fin
				if(monthWeekCalendarBeginWebControl.SelectedDate> monthWeekCalendarEndWebControl.SelectedDate)
					throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1855,_webSession.SiteLanguage)));
				//on teste que l'étude s'effectue sur une année civile
				if(monthWeekCalendarBeginWebControl.SelectedYear != monthWeekCalendarEndWebControl.SelectedYear)
					throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1856,_webSession.SiteLanguage)));
				//On autorise une étude comparative que pour les années N et N-1
//				if(DateTime.Now.Year>_webSession.DownLoadDate){
//					if(isComparativeStudy(selectedComparativeStudy) && (monthWeekCalendarBeginWebControl.SelectedYear==DateTime.Now.Year-3 || monthWeekCalendarEndWebControl.SelectedYear==DateTime.Now.Year-3) )
//						throw(new AdExpressException.SectorDateSelectionException("Il est nécessaire de sélectionner une période supérieure à N-2 pour réaliser une étude comparative."));
//				
//				}else{
					if(isComparativeStudy(selectedComparativeStudy) && (monthWeekCalendarBeginWebControl.SelectedYear==DateTime.Now.Year-2 || monthWeekCalendarEndWebControl.SelectedYear==DateTime.Now.Year-2) )
						throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1857,_webSession.SiteLanguage)));
//				}


				_webSession.PeriodType = monthWeekCalendarBeginWebControl.SelectedDateType;
				switch(monthWeekCalendarBeginWebControl.SelectedDateType){
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
				_webSession.PeriodBeginningDate = monthWeekCalendarBeginWebControl.SelectedDate.ToString();
				_webSession.PeriodEndDate = monthWeekCalendarEndWebControl.SelectedDate.ToString();	
			}
			else {
			 	_webSession.ComparativeStudy=false;
				throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(886,_webSession.SiteLanguage)));
			}		
		}
		#endregion
		
		#region selection etude comparative
		/// <summary>
		/// Verifie si c'est une étude comparative
		/// </summary>
		/// <param name="idStudy">Etude à traiter</param>
		/// <returns>True s'il c'en est une, false sinon</returns>
		private bool isComparativeStudy(Int64 idStudy) {			
			switch(idStudy) {
				case 5:
				case 8:				
					return(true);
				default:
					return(false);
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
