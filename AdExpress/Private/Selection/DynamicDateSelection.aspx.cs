#region Informations
//Auteur : A.Obermeyer
//Date de cr�ation : 29/12/2004
//31/12/2004 A. Obermeyer Int�gration de WebPage
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
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
namespace AdExpress.Private.Selection{

	/// <summary>
	/// Description r�sum�e de DynamicDateSelection.
	/// </summary>
	public partial class DynamicDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Variables MMI


		#endregion

		#region Variables
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
		/// Bouton de validation de la s�lection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Bouton de validation de la s�lection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Titre s�lection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Calandrier de s�lection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Titre de la partie s�lection avec sauvegarde
		/// </summary>
		/// <summary>
		/// Description de la partie avec sauvegarde
		/// </summary>
		/// <summary>
		/// Contr�le txt n dernieres annees
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText yearAdExpressText;
		/// <summary>
		/// Contr�le txt n derniers mois
		/// </summary>
		/// <summary>
		/// Contr�le txt n dernieres semaines
		/// </summary>
		/// <summary>
		/// Contr�le txt "ou bien sur"
		/// </summary>
		/// <summary>
		/// nombre de mois � prendre en compte dans les cas N derni�rs mois
		/// </summary>
		/// <summary>
		/// nombre de semaine � prendre en compte dans les cas N derni�res semaines
		/// </summary>
//		/// <summary>
//		/// Text Option ann�e courante
//		/// </summary>
//		public string currentYear;
//		/// <summary>
//		/// Text Option ann�e pr�c�dent
//		/// </summary>
//		public string previousYear;
//		/// <summary>
//		/// Text Option mois pr�c�dent
//		/// </summary>
//		public string previousMonth;
//		/// <summary>
//		/// Text Option semaine pr�c�dente
//		/// </summary>
//		public string txtPreviousWeek; //previousWeek;
		/// <summary>
		/// Contr�le calendrier date de fin de p�riode
		/// </summary>
		/// <summary>
		/// Option de p�riode s�lectionn�
		/// </summary>
		int selectedIndex=-1;
		/// <summary>
		/// Date de d�but de chargemebt des donn�es
		/// </summary>
		private string downloadBeginningDate="";
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Informations
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Date de fin de chargement des donn�es
		/// </summary>
		private string downloadEndDate="";
		/// <summary>
		/// Choix semaine pr�c�dente
		/// </summary>
		
		/// <summary>
		/// Choix mois pr�c�dent
		/// </summary>
	
		/// <summary>
		/// Choix ann�e pr�c�dente
		/// </summary>

		/// <summary>
		/// Choix ann�e courante
		/// </summary>


		/// <summary>
		/// Dernier mois (YYYYMM) dont les donn�es sont compl�tement disponibles
		/// </summary>
		protected string _lastCompleteMonth = null;
		
		/// <summary>
		/// Media s�lectionn�
		/// </summary>
		protected long _selectedVehicle = -1;
		#endregion
	
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public DynamicDateSelection():base(){
	}
		#endregion

		#region Ev�nements

		#region Chargement de la page

		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region D�finition de la date d�but Analyse dynamique
				if(_webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DYNAMIQUE){
					this.monthWeekCalendarBeginWebControl.StartYear=DateTime.Now.Year-1;
					this.monthWeekCalendarEndWebControl.StartYear=DateTime.Now.Year-1;
				}
				#endregion
			
				#region Option de p�riode s�lectionn�e
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				#endregion
				
				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				
				validateButton1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				validateButton2.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				validateButton2.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
//				previousYear = GestionWeb.GetWebWord(787,_webSession.SiteLanguage);
//				previousMonth = GestionWeb.GetWebWord(788,_webSession.SiteLanguage);
				
//				currentYear=GestionWeb.GetWebWord(1119,_webSession.SiteLanguage);
			
				// Gestion des Calendrier
				this.monthWeekCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.monthWeekCalendarEndWebControl.Language=_webSession.SiteLanguage;	
//				txtPreviousWeek = GestionWeb.GetWebWord(789,_webSession.SiteLanguage);

	
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

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void Page_UnLoad(object sender, System.EventArgs e) {			
		}
		#endregion

		#region Initialisation
		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Ev�nement d'initialisation
		/// </summary>
		/// <param name="e">Argument</param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){
           
            this.Unload += new System.EventHandler(this.Page_UnLoad);
           

		}
		#endregion

		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// On l'utilise pour l'initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;	
			monthDateList.WebSession=_webSession;
			weekDateList.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;

			this.monthWeekCalendarBeginWebControl.IncompleteDatesInRed =  true;			
			this.monthWeekCalendarEndWebControl.IncompleteDatesInRed = true;

				
			previousWeekCheckBox.Language = previousMonthCheckbox.Language = 
			previousYearCheckbox.Language = currentYearCheckbox.Language =_webSession.SiteLanguage;						
			 

			//Les calendriers affiche uniquement des mois pour le m�dia Internet
			if(_webSession.SelectionUniversMedia!=null && _webSession.SelectionUniversMedia.FirstNode !=null){
				 _selectedVehicle = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
				if((DBClassificationConstantes.Vehicles.names)_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet){
					
					_lastCompleteMonth = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(_webSession,WebConstantes.CustomerSessions.Period.DisplayLevel.monthly,WebConstantes.Module.Type.analysis,_selectedVehicle);
					
#if DEBUG
					_lastCompleteMonth = "200703";
#endif
					if(_lastCompleteMonth!=null && _lastCompleteMonth.Length>0)
						this.monthWeekCalendarBeginWebControl.LastCompleteMonth = _lastCompleteMonth;
					else this.monthWeekCalendarBeginWebControl.AllDatesInRed = true;
					this.monthWeekCalendarBeginWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.month;
					
					if(_lastCompleteMonth!=null && _lastCompleteMonth.Length>0)
						this.monthWeekCalendarEndWebControl.LastCompleteMonth = _lastCompleteMonth;
					else this.monthWeekCalendarEndWebControl.AllDatesInRed = true;
					this.monthWeekCalendarEndWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.month;
					
					//Rendre invisible les options de s�lection des semaines pour Intrenet
					weekDateList.Visible = false;
					weekAdExpressText.Visible = false;
					previousWeekCheckBox.Visible = false;
				}
				
			}

			return tmp;
		}
		#endregion

		#region Validation des calendriers
		/// <summary>
		/// Ev�nement de validation des calendriers
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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
		/// Ev�nement de validation des dates avec sauvegarde
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Argument</param>
		protected void validateButton2_Click(object sender, System.EventArgs e) {
			

			try{
				DateDll.AtomicPeriodWeek week;
				DateTime monthPeriod;

				switch(selectedIndex){
						//Ann�e courante
					case 0:						
						_webSession.PeriodType=CstPeriodType.currentYear;
						_webSession.PeriodLength=1;
						
						//Dates de chargement des donn�es pour Internet
						if(_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){
							if( _lastCompleteMonth !=null && _lastCompleteMonth.Length>0 && int.Parse(_lastCompleteMonth.Substring(0,4))==DateTime.Now.Year){
								_webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");						
								_webSession.PeriodEndDate = _lastCompleteMonth;
							}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,_webSession.SiteLanguage));
						}else{
							//Dates de chargement des donn�es pour les autres m�dias
							if(DateTime.Now.Month==1){
								throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(1612,_webSession.SiteLanguage));														
							}else {
								WebFunctions.Dates.DownloadDates(_webSession,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.currentYear);								
								_webSession.PeriodBeginningDate=downloadBeginningDate;						
								_webSession.PeriodEndDate=downloadEndDate;
							}
						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;						
						break;

						//N derniers mois
					case 1:
						if(int.Parse(monthDateList.SelectedValue)!=0){
							_webSession.PeriodType=CstPeriodType.nLastMonth;
							_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);
						
							//Dates de chargement des donn�es pour Internet
							if(_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){
								if(_lastCompleteMonth !=null && _lastCompleteMonth.Length>0){
									_webSession.PeriodEndDate = _lastCompleteMonth;
									monthPeriod = new DateTime(int.Parse(_lastCompleteMonth.Substring(0,4)),int.Parse(_lastCompleteMonth.Substring(4,2)),01);						
									_webSession.PeriodBeginningDate = monthPeriod.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");	
								}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,_webSession.SiteLanguage));
							}else{
								//Dates de chargement des donn�es pour les autres m�dias
								WebFunctions.Dates.LastLoadedMonth(ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.nLastMonth);								
								monthPeriod = new DateTime(int.Parse(downloadBeginningDate.Substring(0,4)),int.Parse(downloadBeginningDate.Substring(4,2)),01);						
								_webSession.PeriodBeginningDate = monthPeriod.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");							
								_webSession.PeriodEndDate = downloadEndDate;
							}
							
							_webSession.DetailPeriod = CstPeriodDetail.monthly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;

						//N derni�res semaines
					case 2:
						if(int.Parse(weekDateList.SelectedValue)!=0){
							//derni�re semaine charg�e
							WebFunctions.Dates.LastLoadedWeek(ref downloadBeginningDate,ref downloadEndDate);
							_webSession.PeriodType=CstPeriodType.nLastWeek;
							_webSession.PeriodLength=int.Parse(weekDateList.SelectedValue);							
							week = new DateDll.AtomicPeriodWeek(int.Parse(downloadEndDate.Substring(0,4)),int.Parse(downloadEndDate.Substring(4,2)));
							_webSession.PeriodEndDate = downloadEndDate;
							week.SubWeek(_webSession.PeriodLength-1);
							_webSession.PeriodBeginningDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
							_webSession.DetailPeriod = CstPeriodDetail.weekly;
						}
						else{
							throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;
					
						//Ann�e pr�c�dente
					case 3:
						_webSession.PeriodType=CstPeriodType.previousYear;
						_webSession.PeriodLength=1;

						//Dates de chargement des donn�es pour Internet
						if(_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){						
							if(_lastCompleteMonth !=null && _lastCompleteMonth.Length>0 && int.Parse(_lastCompleteMonth.Substring(0,4))>DateTime.Now.AddYears(-2).Year){
								_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
								_webSession.PeriodEndDate = (int.Parse(_lastCompleteMonth.Substring(0,4))==DateTime.Now.Year)? DateTime.Now.AddYears(-1).ToString("yyyy12") : _lastCompleteMonth;
							}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,_webSession.SiteLanguage));
						}else{
							//Dates de chargement des donn�es pour les autres m�dias
							WebFunctions.Dates.DownloadDates(_webSession,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.previousYear);								
							_webSession.PeriodBeginningDate=downloadBeginningDate;						
							_webSession.PeriodEndDate=downloadEndDate;
						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;					
						break;

						//Mois pr�c�dent
					case 4:
						//Dates de chargement des donn�es pour Internet
						if(_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()){						
							if(_lastCompleteMonth !=null && _lastCompleteMonth.Length>0 && int.Parse(_lastCompleteMonth) >= int.Parse(DateTime.Now.AddMonths(-1).ToString("yyyyMM")))
								_webSession.PeriodEndDate = _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
							else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,_webSession.SiteLanguage));
						}else{
							//Dates de chargement des donn�es pour les autres m�dias
							WebFunctions.Modules.ActivePreviousAtomicPeriod(CstPeriodType.previousMonth,_webSession);												
							_webSession.PeriodEndDate = _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
							
						}
						_webSession.PeriodType=CstPeriodType.previousMonth;	
						_webSession.PeriodLength=1;
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						break;

						//Semaine pr�c�dente
					case 5:
						WebFunctions.Modules.ActivePreviousAtomicPeriod(CstPeriodType.previousWeek,_webSession);
						_webSession.PeriodType=CstPeriodType.previousWeek;
						_webSession.PeriodLength=1;
						week = new DateDll.AtomicPeriodWeek(DateTime.Now.AddDays(-7));
						_webSession.PeriodBeginningDate=_webSession.PeriodEndDate=week.Year.ToString() + ((week.Week>9)?"":"0")+week.Week.ToString();
						_webSession.DetailPeriod = CstPeriodDetail.weekly;
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

		#region M�thodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation(){
			// On v�rifie que 2 dates ont �t� s�lectionn�es
			if(monthWeekCalendarBeginWebControl.isDateSelected() && monthWeekCalendarEndWebControl.isDateSelected()){
				// On teste que les deux dates sont du m�me type
				if(monthWeekCalendarBeginWebControl.SelectedDateType!=monthWeekCalendarEndWebControl.SelectedDateType)
					throw(new AdExpressException.AnalyseDateSelectionException("Les dates s�lectionn�es ne sont pas du m�me type"));
				// On teste que la date de d�but est inf�rieur � la date de fin
				if(monthWeekCalendarBeginWebControl.SelectedDate>monthWeekCalendarEndWebControl.SelectedDate)
					throw(new AdExpressException.AnalyseDateSelectionException("La date de d�but doit �tre inf�rieure � la date de fin"));
				_webSession.PeriodType =monthWeekCalendarBeginWebControl.SelectedDateType;
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
			else{
				throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(886,_webSession.SiteLanguage)));
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
