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
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using System.Text;

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
        public bool finland = false;

        public string currentYear = DateTime.Now.Year.ToString();

        protected VehicleInformation _vehicleInformation = null;
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

                #region Test Cedexis
                //Test Cedexis
                if (WebApplicationParameters.CountryCode == TNS.AdExpress.Constantes.Web.CountryCode.FRANCE &&
                !Page.ClientScript.IsClientScriptBlockRegistered("CedexisScript"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "CedexisScript", TNS.AdExpress.Web.Functions.Script.CedexisScript());
                }
                #endregion

                if (!this.Page.ClientScript.IsClientScriptBlockRegistered("RecapDatesJavaScriptFunctions"))
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RecapDatesJavaScriptFunctions", RecapDatesJavaScriptFunctions());

                #region Patch Finland
                finland = false;
                if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.FINLAND)) finland = true;
                
                #endregion

				#region Options pour la période sélectionnée
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);
				#endregion

                if (finland )
                {
                    this.monthWeekCalendarBeginWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.month;
                    this.monthWeekCalendarEndWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.month;
                    this.monthWeekCalendarEndWebControl.CurrentModule = _currentModule.Id;
                    this.monthWeekCalendarBeginWebControl.CurrentModule = _currentModule.Id;
                    if (_vehicleInformation == null)
                    {
                        long vehicleId = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
                        _vehicleInformation = VehiclesInformation.Get(vehicleId);
                    }
                    //Patch Finland pour le Tableau de bord PRESSE
                    if (TNS.AdExpress.Web.Core.Utilities.LastAvailableDate.LastAvailableDateList.ContainsKey(_vehicleInformation.Id))
                    {                        
                         this.monthWeekCalendarEndWebControl.VehicleInformation = _vehicleInformation;
                         this.monthWeekCalendarBeginWebControl.VehicleInformation = _vehicleInformation;
                    }
                   
                }
                else
                {
                    this.monthWeekCalendarBeginWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.all;
                    this.monthWeekCalendarEndWebControl.DisplayType = TNS.AdExpress.Web.Controls.Selections.MonthWeekCalendarWebControl.Display.all;
                }
                int nbYears = WebApplicationParameters.DataNumberOfYear - 1;
                this.monthWeekCalendarBeginWebControl.StartYear = DateTime.Now.AddYears(-nbYears).Year;
                this.monthWeekCalendarEndWebControl.StartYear = DateTime.Now.AddYears(-nbYears).Year;

			
				//ATTaquer une table
				downloadDate=_webSession.DownLoadDate;				

			

				#region Textes et langage du site
				
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
									
				// Gestion des Calendrier
				this.monthWeekCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.monthWeekCalendarEndWebControl.Language=_webSession.SiteLanguage;	
				
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
				_webSession.Source.Close();
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
			;
			if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);				
			try {
				DateTime downloadDate=new DateTime(_webSession.DownLoadDate,12,31);

                if (finland)
                {
                    if (_vehicleInformation == null)
                    {
                        long vehicleId = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
                        _vehicleInformation = VehiclesInformation.Get(vehicleId);
                    }
                    //Patch Finland pour le Tableau de bord PRESSE
                    if (TNS.AdExpress.Web.Core.Utilities.LastAvailableDate.LastAvailableDateList.ContainsKey(_vehicleInformation.Id))
                    {
                        downloadDate = TNS.AdExpress.Web.Core.Utilities.LastAvailableDate.LastAvailableDateList[_vehicleInformation.Id];                       
                    }

                }
				switch(selectedIndex) {
						//Choix  dernier mois chargé
					case 1:
							_webSession.PeriodType=CstPeriodType.LastLoadedMonth;
                            if (finland)
                            {
                                //Patch Finland pour le Tableau de bord PRESSE
                                downloadBeginningDate = _webSession.PeriodEndDate = _webSession.PeriodBeginningDate = String.Format("{0:yyyyMM}", downloadDate);
                            }
                            else
                            {
                                WebFunctions.Dates.LastLoadedMonth(ref downloadBeginningDate, ref downloadEndDate, CstPeriodType.nLastMonth);
                                _webSession.PeriodBeginningDate = downloadBeginningDate;
                                _webSession.PeriodEndDate = downloadEndDate;
                            }
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
                        if (finland)
                        {
                            //Patch Finland pour le Tableau de bord PRESSE
                            _webSession.PeriodBeginningDate = String.Format("{0:yyyy01}", downloadDate);
                            _webSession.PeriodEndDate = String.Format("{0:yyyyMM}", downloadDate);
                        }
                        else
                        {
                            if (DateTime.Now.Month == 1)
                            {
                                throw new TNS.AdExpress.Domain.Exceptions.NoDataException(GestionWeb.GetWebWord(1612, _webSession.SiteLanguage));
                            }
                            else
                            {
                                WebFunctions.Dates.DownloadDates(_webSession, ref downloadBeginningDate, ref downloadEndDate, CstPeriodType.currentYear);
                                _webSession.PeriodBeginningDate = downloadBeginningDate;
                                _webSession.PeriodEndDate = downloadEndDate;
                            }

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
                        if (finland)
                        {
                            //Patch Finland pour le Tableau de bord PRESSE
                            int yearN1 = downloadDate.Year -1;
                            _webSession.PeriodBeginningDate = yearN1.ToString() + "01";
                            _webSession.PeriodEndDate = yearN1.ToString() + "12"; 
                        }
                        else
                        {
                            WebFunctions.Dates.DownloadDates(_webSession, ref downloadBeginningDate, ref downloadEndDate, CstPeriodType.previousYear);
                            _webSession.PeriodBeginningDate = downloadBeginningDate;
                            _webSession.PeriodEndDate = downloadEndDate;
                        }
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						//Activation de l'option etude comparative si selectionné
						if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						
						break;
						//Choix année N-2
					case 4:
						if(isComparativeStudy(selectedComparativeStudy) &&  (WebApplicationParameters.DataNumberOfYear <= 3))
                            throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						_webSession.PeriodType=CstPeriodType.nextToLastYear;
						_webSession.PeriodLength=1;						
						// Cas où l'année de chargement est inférieur à l'année en cours
                        if (finland)
                        {
                            //Patch Finland pour le Tableau de bord PRESSE
                            int yearN2 = downloadDate.Year - 2;
                             _webSession.PeriodBeginningDate = yearN2.ToString() + "01";
                             _webSession.PeriodEndDate = yearN2.ToString() + "12";
                        }
                        else
                        {
                            _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy01");
                            _webSession.PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy12");
                        }
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
                        if (isComparativeStudy(selectedComparativeStudy)) _webSession.ComparativeStudy = true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						break;
					default :
						throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				_webSession.Source.Close();
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

           

            this.monthWeekCalendarBeginWebControl.BlockIncompleteDates =  true;			
			this.monthWeekCalendarEndWebControl.BlockIncompleteDates = true;
            this.monthWeekCalendarEndWebControl.CurrentModule = _currentModule.Id;
            this.monthWeekCalendarBeginWebControl.CurrentModule = _currentModule.Id;
            if (_vehicleInformation == null)
            {
                long vehicleId = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
                _vehicleInformation = VehiclesInformation.Get(vehicleId);
                this.monthWeekCalendarEndWebControl.VehicleInformation = _vehicleInformation;
                this.monthWeekCalendarBeginWebControl.VehicleInformation = _vehicleInformation;
            }
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

                if (isComparativeStudy(selectedComparativeStudy)
                    && ((monthWeekCalendarBeginWebControl.SelectedYear == DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear - 1) || monthWeekCalendarEndWebControl.SelectedYear == DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear - 1))))
                    throw (new AdExpressException.SectorDateSelectionException(string.Format(GestionWeb.GetWebWord(2807, _siteLanguage), (WebApplicationParameters.DataNumberOfYear - 1))));


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

        #region Javascript
        private string RecapDatesJavaScriptFunctions()
        {
            StringBuilder script = new StringBuilder(2000);
            script.Append("<script language=\"JavaScript\">");
            //function selectedItem
            script.Append("\r\n function selectedItem(i,formName)");
            script.Append("\r\n {");
            script.Append("\r\n\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t formName.CurrentYearRadioButton.checked=false;");
            script.Append("\r\n\t formName.PreviousYearRadioButton.checked=false;");
            script.Append("\r\n\t formName.TwoYearAgoRadioButton.checked=false;");
            script.Append("\r\n\t formName.CompetitorSudy1Ckbx.checked=false;");
            script.Append("\r\n\t formName.LastLoadedMonthRadiobutton.checked=false;");
            script.Append("\r\n\t if(formName.LastLoadedWeekRadioButton!=null)formName.LastLoadedWeekRadioButton.checked=false;");
            script.Append("\r\n }");

            //function selectedCheck
            script.Append("\r\n function selectedCheck(i,formName)");
            script.Append("\r\n {");
            script.Append("\r\n\t switch(i)");
            script.Append("\r\n\t {");
            script.Append("\r\n\t\t case 1 :");
            script.Append("\r\n\t\t\t formName.CompetitorSudy1Ckbx.checked=false;");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t\t case 2 :");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t\t case 3 :");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t\t case 4 :");
            if (WebApplicationParameters.DataNumberOfYear <= 3)//If start year is N-2 
            script.Append("\r\n\t\t\t formName.CompetitorSudy2Ckbx.checked=false;");
            script.Append("\r\n\t\t\t formName.CompetitorSudy1Ckbx.checked=false;");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t\t case 5 :");
            if(WebApplicationParameters.DataNumberOfYear<=3)//If start year is N-2 
            script.Append("\r\n\t\t\t formName.TwoYearAgoRadioButton.checked=false;");
            script.Append("\r\n\t\t\t formName.CompetitorSudy1Ckbx.checked=false;");
            script.Append("\r\n\t\t\t formName.selectedComparativeStudy.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t\t case 8 :");
            script.Append("\r\n\t\t\t formName.selectedComparativeStudy.value=i;");
            script.Append("\r\n\t\t\t formName.CompetitorSudy2Ckbx.checked=false;");
            script.Append("\r\n\t\t\t formName.CurrentYearRadioButton.checked=false;");
            script.Append("\r\n\t\t\t formName.PreviousYearRadioButton.checked=false;");
            script.Append("\r\n\t\t\t formName.TwoYearAgoRadioButton.checked=false;");
            script.Append("\r\n\t\t\t formName.LastLoadedMonthRadiobutton.checked=false;");
            script.Append("\r\n\t\t\t if(formName.LastLoadedWeekRadioButton!=null)formName.LastLoadedWeekRadioButton.checked=false;");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");


            script.Append("\r\n\t\t case 9 :");
            script.Append("\r\n\t\t\t formName.CompetitorSudy1Ckbx.checked=false;");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t }");
            script.Append("\r\n }");
            script.Append("\r\n </script>");
            return (script.ToString());
        }
        #endregion
    
    }
}
