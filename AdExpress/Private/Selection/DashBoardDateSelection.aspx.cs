#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 24/02/2005
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
	/// Description r�sum�e de DashBoardDateSelection.
	/// </summary>
	public partial class DashBoardDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{
		
		#region variables MMI
		/// <summary>
		/// Contr�le titre du module
		/// </summary>
		/// <summary>
		/// Contr�le ent�te page
		/// </summary>
		/// <summary>
		/// Contr�le texte titre
		/// </summary>
		/// <summary>
		/// Contr�le texte �tude coparative
		/// </summary>
		/// <summary>
		/// Contr�le case � cocher �tude coparative
		/// </summary>
		/// <summary>
		/// Contr�le bouton validation
		/// </summary>
		/// <summary>
		/// Contr�le texte titre 
		/// </summary>
		/// <summary>
		/// Contr�le rdiio bouton ann�e courante
		/// </summary>
		/// <summary>
		/// Contr�le texte ann�e courante
		/// </summary>
		/// <summary>
		/// Contr�le annn�e pr�c�dente
		/// </summary>
		/// <summary>
		/// Contr�le texte ann�e pr�c�dente
		/// </summary>
		/// <summary>
		/// Contr�le bouton radio ann�e avant ann�e pr�c�dente
		/// </summary>
		/// <summary>
		/// Contr�le texte ann�e avant ann�e pr�c�dente
		/// </summary>
		/// <summary>
		/// Contr�le texte �tude comparative
		/// </summary>
		/// <summary>
		/// Contr�le case � cocher �tude comparative
		/// </summary>
		/// <summary>
		/// Contr�le choix d�but p�riode
		/// </summary>
		/// <summary>
		/// Contr�le choix fin p�riode
		/// </summary>
		/// <summary>
		/// Contr�le bouton valider
		/// </summary>
		/// <summary>
		/// Contr�le derniere semaine charg�e
		/// </summary>
		/// <summary>
		/// Contr�le dernier mois charg�
		/// </summary>
		/// <summary>
		/// Texte derniere semaine charg�e
		/// </summary>
		/// <summary>
		/// Texte dernier mois charg�
		/// </summary>
		#endregion

		#region variables
		/// <summary>
		/// Index s�lectionn�
		/// </summary>
		int selectedIndex=-1;
		/// <summary>
		/// Etude de comparative ?
		/// </summary>
		int selectedComparativeStudy=-1;
		/// <summary>
		/// Ann�e de chargement des donn�es
		/// </summary>
		public int downloadDate=0;
		/// <summary>
		/// Date de d�but de chargemebt des donn�es
		/// </summary>
		private string downloadBeginningDate="";
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Date de fin de chargement des donn�es
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
		
		#region Ev�nements
		
		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try{

				#region Options pour la p�riode s�lectionn�e
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);
				#endregion

				#region Rappel des diff�rentes s�lections
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

				#region D�finition de la page d'aide
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

		#region Pr�Rendu de la page

		/// <summary>
		/// Ev�nement de Pr�Rendu
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){						
		}
		#endregion

		#region Validation des calendriers
		/// <summary>
		/// Ev�nement de validation des calendriers
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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
		/// Ev�nement de validation des dates avec sauvegarde
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void validateButton2_Click(object sender, System.EventArgs e) {
			//DateDll.AtomicPeriodWeek week;
			if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);				
			try {
				DateTime downloadDate=new DateTime(_webSession.DownLoadDate,12,31);
				
				switch(selectedIndex) {
						//Choix  dernier mois charg�
					case 1:
							_webSession.PeriodType=CstPeriodType.LastLoadedMonth;
							
							WebFunctions.Dates.LastLoadedMonth(ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.nLastMonth);								
							_webSession.PeriodBeginningDate=downloadBeginningDate;						
							_webSession.PeriodEndDate=downloadEndDate;							
							_webSession.PeriodLength=int.Parse(downloadBeginningDate.Substring(4,2));	
							_webSession.DetailPeriod = CstPeriodDetail.monthly;
							//Activation de l'option etude comparative si selectionn�
							if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
							else _webSession.ComparativeStudy=false;
							_webSession.Save();
						break;
						//Choix derniere semaine charg�e
					case 9 :

							WebFunctions.Dates.LastLoadedWeek(ref downloadBeginningDate,ref downloadEndDate);								
							_webSession.PeriodBeginningDate=downloadBeginningDate;						
							_webSession.PeriodEndDate=downloadEndDate;
							_webSession.PeriodType=CstPeriodType.LastLoadedWeek;

							_webSession.PeriodLength=int.Parse(downloadBeginningDate.Substring(4,2));		
							_webSession.DetailPeriod =  CstPeriodDetail.weekly;
							//Activation de l'option etude comparative si selectionn�
							if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
							else _webSession.ComparativeStudy=false;
							_webSession.Save();											
						break;
						//Choix ann�e courante
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
						//Activation de l'option etude comparative si selectionn�
						if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						break;
						//Choix ann�e pr�cedente
					case 3:
						_webSession.PeriodType=CstPeriodType.previousYear;
						_webSession.PeriodLength=1;					
						
						WebFunctions.Dates.DownloadDates(_webSession,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.previousYear);								
						_webSession.PeriodBeginningDate=downloadBeginningDate;						
						_webSession.PeriodEndDate=downloadEndDate;
						
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						//Activation de l'option etude comparative si selectionn�
						if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						
						break;
						//Choix ann�e N-2
					case 4:
						if(isComparativeStudy(selectedComparativeStudy))throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						_webSession.PeriodType=CstPeriodType.nextToLastYear;
						_webSession.PeriodLength=1;						
						// Cas o� l'ann�e de chargement est inf�rieur � l'ann�e en cours
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

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
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

		#region M�thodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation() {
			// On v�rifie que 2 dates ont �t� s�lectionn�es
			if( monthWeekCalendarBeginWebControl.isDateSelected() && monthWeekCalendarEndWebControl.isDateSelected()) {		
				// On teste que les deux dates sont du m�me type
				if(monthWeekCalendarBeginWebControl.SelectedDateType!=monthWeekCalendarEndWebControl.SelectedDateType)
					throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(1854,_webSession.SiteLanguage)));
				// On teste que la date de d�but est inf�rieur � la date de fin
				if(monthWeekCalendarBeginWebControl.SelectedDate> monthWeekCalendarEndWebControl.SelectedDate)
					throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1855,_webSession.SiteLanguage)));
				//on teste que l'�tude s'effectue sur une ann�e civile
				if(monthWeekCalendarBeginWebControl.SelectedYear != monthWeekCalendarEndWebControl.SelectedYear)
					throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1856,_webSession.SiteLanguage)));
				//On autorise une �tude comparative que pour les ann�es N et N-1
//				if(DateTime.Now.Year>_webSession.DownLoadDate){
//					if(isComparativeStudy(selectedComparativeStudy) && (monthWeekCalendarBeginWebControl.SelectedYear==DateTime.Now.Year-3 || monthWeekCalendarEndWebControl.SelectedYear==DateTime.Now.Year-3) )
//						throw(new AdExpressException.SectorDateSelectionException("Il est n�cessaire de s�lectionner une p�riode sup�rieure � N-2 pour r�aliser une �tude comparative."));
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
						throw(new AdExpressException.AnalyseDateSelectionException("Le choix de type de s�lection de p�riode est incorrect"));
				}
				// On sauvegarde les donn�es
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
		/// Verifie si c'est une �tude comparative
		/// </summary>
		/// <param name="idStudy">Etude � traiter</param>
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

		#region Impl�mentation m�thodes abstraites
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
