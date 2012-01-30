#region Informations
// Auteur: D. V. Mussuma
// Date de création: 20/09/2004
// Date de modification: 20/09/2004
//	30/12/2004  D. Mussuma Intégration de WebPage
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
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Constantes.Customer;
using AdExpressWebControles=TNS.AdExpress.Web.Controls;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Domain.Web.Navigation;
using DateDll = TNS.FrameWork.Date;
using AdExpressException=AdExpress.Exceptions;
using TNS.AdExpress.Web.Exceptions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using System.Text;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Domain.Web;
using System.Reflection;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Exceptions;
#endregion

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de sélection de dates par mois ou par années
	/// Cette Page est utilisée pour le choix des périodes dans l'analyse sectorielle
	/// </summary>
	public partial class SectorDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage{		
		
		#region Variables MMI
		/// <summary>
		/// Entete
		/// </summary>
		/// <summary>
		/// Titre et description de la page
		/// </summary>
		/// <summary>
		/// Titre sélection sans sauvegarde
		/// </summary>
		/// <summary>
		/// Texte étude comparative période glissante
		/// </summary>
		/// <summary>
		/// choix étude comparative période glissante
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Texte titre 
		/// </summary>
		/// <summary>
		/// Texte commentaire
		/// </summary>
		/// <summary>
		/// Liste des mois
		/// </summary>
		/// <summary>
		/// Texte du mois sélectionnable
		/// </summary>
		/// <summary>
		/// Texte de la dernière période sélectionnée
		/// </summary>
		/// <summary>
		/// année courante (N)
		/// </summary>
		/// <summary>
		/// Texte année courante
		/// </summary>
		/// <summary>
		/// choix année précédente (N-1)
		/// </summary>
		/// <summary>
		/// Texte année précédente (N-1)
		/// </summary>
		/// <summary>
		/// choix année avant année précédente (N-2)
		/// </summary>
		/// <summary>
		/// Texte année avant année précédente (N-2)
		/// </summary>
		/// <summary>
		/// Texte étude comparative
		/// </summary>
		/// <summary>
		/// Choix étude comparative
		/// </summary>
		/// <summary>
		/// Bouton valider 2
		/// </summary>
		/// <summary>
		/// Calendrier de sélection de date de début
		/// </summary>
		/// <summary>
		/// Calendrier de sélection de date de fin
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Index sélectionné
		/// </summary>
		int selectedIndex=-1;
		/// <summary>
		/// Etude de comparative ?
		/// </summary>
		int selectedComparativeStudy=-1;
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Année de chargement des données
		/// </summary>
		public int downloadDate=0;
		#endregion
		
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public SectorDateSelection():base(){			
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

				#region Options pour la période sélectionnée
				if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
				if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);
				#endregion
			
				//ATTaquer une table
				downloadDate=_webSession.DownLoadDate;				

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress		
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;								
				// Gestion des Calendrier
				this.monthCalendarBeginWebControl.Language=_webSession.SiteLanguage;
				this.monthCalendarEndWebControl.Language=_webSession.SiteLanguage;
				#endregion

                if (!this.Page.ClientScript.IsClientScriptBlockRegistered("RecapDatesJavaScriptFunctions"))
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RecapDatesJavaScriptFunctions", RecapDatesJavaScriptFunctions());

                #region Initialize Calendar
                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                
                monthCalendarEndWebControl.IsRestricted = monthCalendarBeginWebControl.IsRestricted = ModulesList.GetModule(_webSession.CurrentModule).DisplayIncompleteDateInCalendar;
                List<Int64> selectedVehicleList = new List<Int64>();
                if (monthCalendarBeginWebControl.IsRestricted) {
                    monthCalendarEndWebControl.IsRestricted = monthCalendarBeginWebControl.IsRestricted;
                    string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                    if (vehicleSelection == null) throw (new VehicleException("Selection of media type is not correct"));
                    selectedVehicleList = new List<Int64>((new List<string>(vehicleSelection.Split(','))).ConvertAll<Int64>(ConvertStringToInt64));

                    if (IsPostBack) {
                        if (this.ViewState["FirstDayNotEnabledVS"] != null)
                            monthCalendarEndWebControl.FirstDayNotEnable =  monthCalendarBeginWebControl.FirstDayNotEnable = (DateTime)this.ViewState["FirstDayNotEnabledVS"];
                    }
                    else {
                        monthCalendarEndWebControl.FirstDayNotEnable = monthCalendarBeginWebControl.FirstDayNotEnable = dateDAL.GetFirstDayNotEnabled(selectedVehicleList, dateDAL.GetCalendarStartDate());
                        ViewState.Add("FirstDayNotEnabledVS", monthCalendarBeginWebControl.FirstDayNotEnable);
                    }
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
		protected void Page_PreRender(object sender, System.EventArgs e) 
		{
			if (this.IsPostBack)
			{
				#region Url Suivante
				
				if(_nextUrlOk)
				{
					if(selectedIndex==6 ||selectedIndex==7 ||selectedIndex==8) validateButton1_Click(this, null);
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
		protected void validateButton1_Click(object sender, System.EventArgs e) 
		{			
			if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);	
			try
			{
				calendarValidation();
				if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
				else _webSession.ComparativeStudy=false;
				_webSession.Save();
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch (TNS.AdExpress.Domain.Exceptions.NoDataException) {
				_webSession.ComparativeStudy=false;
				Response.Write("<script language=\"JavaScript\">alert(\""+GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)+"\");</script>");
			}
			catch(System.Exception ex)
			{	_webSession.ComparativeStudy=false;
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
		protected void validateButton2_Click(object sender, System.EventArgs e)
		{
			if(Request.Form.GetValues("selectedItemIndex")!=null)selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			if(Request.Form.GetValues("selectedComparativeStudy")!=null)selectedComparativeStudy = int.Parse(Request.Form.GetValues("selectedComparativeStudy")[0]);	

			try
			{
                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

				DateTime downloadDate=new DateTime(_webSession.DownLoadDate,12,31);				
				string absolutEndPeriod ="";
				switch(selectedIndex)
				{
						//Choix N derniers mois
					case 1:
						if(int.Parse(monthDateList.SelectedValue)!=0)
						{
							_webSession.PeriodType=CstPeriodType.nLastMonth;
							_webSession.PeriodLength=int.Parse(monthDateList.SelectedValue);						

							// Cas où l'année de chargement des données est inférieur à l'année actuelle
							if(DateTime.Now.Year>_webSession.DownLoadDate){
								_webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");
								_webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
							
							}
							else{
								_webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");
								_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
							}

							absolutEndPeriod = dateDAL.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
							if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4,2).Equals("00")))
								throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)));

							if(int.Parse(absolutEndPeriod)<int.Parse(_webSession.PeriodEndDate))
								_webSession.PeriodEndDate=absolutEndPeriod;

							_webSession.DetailPeriod = CstPeriodDetail.dayly;
							//Activation de l'option etude comparative si selectionné
							if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
							else _webSession.ComparativeStudy=false;
							_webSession.Save();
						}
						else
						{
							throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
						}
						break;
						//Choix année courante
					case 2:
						_webSession.PeriodType=CstPeriodType.currentYear;
						_webSession.PeriodLength=1;						
						// Cas où l'année de chargement est inférieur à l'année en cours
						if(DateTime.Now.Year>_webSession.DownLoadDate){
							_webSession.PeriodBeginningDate=downloadDate.ToString("yyyy01");						
							_webSession.PeriodEndDate=downloadDate.ToString("yyyyMM");
						}
						else{
							_webSession.PeriodBeginningDate=DateTime.Now.ToString("yyyy01");						
							_webSession.PeriodEndDate=DateTime.Now.ToString("yyyyMM");
						}

						//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
						//du dernier mois dispo en BDD
						//traitement de la notion de fréquence
                        absolutEndPeriod = dateDAL.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
						if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4,2).Equals("00")) )
							throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)));
												
						_webSession.PeriodEndDate=absolutEndPeriod;

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
						
						// Cas où l'année de chargement est inférieur à l'année en cours
						if(DateTime.Now.Year>_webSession.DownLoadDate){
							_webSession.PeriodBeginningDate=downloadDate.AddYears(-1).ToString("yyyy01");
							_webSession.PeriodEndDate=downloadDate.AddYears(-1).ToString("yyyy12");
						}
						else{
							_webSession.PeriodBeginningDate=DateTime.Now.AddYears(-1).ToString("yyyy01");
							_webSession.PeriodEndDate=DateTime.Now.AddYears(-1).ToString("yyyy12");
						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
						//Activation de l'option etude comparative si selectionné
						if(isComparativeStudy(selectedComparativeStudy))_webSession.ComparativeStudy=true;
						else _webSession.ComparativeStudy=false;
						_webSession.Save();
						
						break;

						//Choix année N-2
					case 4:
                        if (isComparativeStudy(selectedComparativeStudy) && (WebApplicationParameters.DataNumberOfYear <= 3)) throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
						_webSession.PeriodType=CstPeriodType.nextToLastYear;
						_webSession.PeriodLength=1;						
						// Cas où l'année de chargement est inférieur à l'année en cours
						if(DateTime.Now.Year>_webSession.DownLoadDate){
							_webSession.PeriodBeginningDate=downloadDate.AddYears(-2).ToString("yyyy01");
							_webSession.PeriodEndDate=downloadDate.AddYears(-2).ToString("yyyy12");
						}
						else{
							_webSession.PeriodBeginningDate=DateTime.Now.AddYears(-2).ToString("yyyy01");
							_webSession.PeriodEndDate=DateTime.Now.AddYears(-2).ToString("yyyy12");
						}
						_webSession.DetailPeriod = CstPeriodDetail.monthly;
                        if (isComparativeStudy(selectedComparativeStudy)) _webSession.ComparativeStudy = true;
                        else 
						_webSession.ComparativeStudy=false;
						_webSession.Save();
						break;
					default :
						throw(new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885,_webSession.SiteLanguage)));
				}
				_webSession.Source.Close();
				Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
			}
			catch (TNS.AdExpress.Domain.Exceptions.NoDataException) {
				Response.Write("<script language=\"JavaScript\">alert(\""+GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)+"\");</script>");
			}
			catch(System.Exception ex)
			{
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

		#region DeterminePostBack
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			try
			{
			monthDateList.WebSession=_webSession;
			monthCalendarBeginWebControl.WebSession=_webSession;
			monthCalendarEndWebControl.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
			return tmp;
		}
		#endregion

		#region Méthodes internes

		#region Validation des calendriers
		/// <summary>
		/// Traitement des dates d'un calendrier
		/// </summary>
		public void calendarValidation()
		{
			// On vérifie que 2 dates ont été sélectionnées
			if( monthCalendarBeginWebControl.isDateSelected() && monthCalendarEndWebControl.isDateSelected())
			{		
				// On teste que la date de début est inférieur à la date de fin
				if(monthCalendarBeginWebControl.SelectedDate> monthCalendarEndWebControl.SelectedDate)
				throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1855,_siteLanguage)));
				
				if(monthCalendarBeginWebControl.SelectedYear != monthCalendarEndWebControl.SelectedYear)
					throw (new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(2573, _siteLanguage)));
				
				if(DateTime.Now.Year>_webSession.DownLoadDate){
                    if (isComparativeStudy(selectedComparativeStudy) && ((monthCalendarBeginWebControl.SelectedYear == DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear || monthCalendarEndWebControl.SelectedYear == DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear)))
                        throw (new AdExpressException.SectorDateSelectionException(string.Format(GestionWeb.GetWebWord(2807, _siteLanguage), (WebApplicationParameters.DataNumberOfYear - 1))));
				
				}else{
                    if (isComparativeStudy(selectedComparativeStudy) && ((monthCalendarBeginWebControl.SelectedYear == DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear - 1) || monthCalendarEndWebControl.SelectedYear == DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear - 1))))
                        throw (new AdExpressException.SectorDateSelectionException(string.Format(GestionWeb.GetWebWord(2807, _siteLanguage), (WebApplicationParameters.DataNumberOfYear - 1))));
				}


				_webSession.PeriodType =monthCalendarBeginWebControl.SelectedDateType;
				try
				{					
					_webSession.DetailPeriod = CstPeriodDetail.monthly;
				}
				catch(Exception e){
					_webSession.ComparativeStudy=false;
					Response.Write("<script language=\"JavaScript\">alert(\""+e.Message+"\");</script>");
				}
				

				_webSession.PeriodBeginningDate = monthCalendarBeginWebControl.SelectedDate.ToString();
				_webSession.PeriodEndDate = monthCalendarEndWebControl.SelectedDate.ToString();
	
				// On sauvegarde les données
				//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
				//du dernier mois dispo en BDD
				//traitement de la notion de fréquence	
				if(int.Parse(monthCalendarEndWebControl.SelectedDate.ToString().Substring(0,4))==DateTime.Now.Year && int.Parse(monthCalendarBeginWebControl.SelectedDate.ToString().Substring(0,4))==DateTime.Now.Year){
                    
                    CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                    object[] param = new object[1];
                    param[0] = _webSession;
                    IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

                    string absolutEndPeriod = dateDAL.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
					if (int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate))
						throw(new AdExpressException.SectorDateSelectionException(GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)));

					if(int.Parse(absolutEndPeriod)<int.Parse(_webSession.PeriodEndDate))
						_webSession.PeriodEndDate=absolutEndPeriod;
				}
			}
			else
			{	_webSession.ComparativeStudy=false;
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
		private bool isComparativeStudy(Int64 idStudy)
		{			
			switch(idStudy)
			{
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

        #region ConvertStringToInt64
        /// <summary>
        /// Convert String To Int64
        /// </summary>
        /// <param name="p">String parameter</param>
        /// <returns>Int64 Result</returns>
        private Int64 ConvertStringToInt64(string p) {
            return Int64.Parse(p);
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
            script.Append("\r\n }");

            //function selectedCheck
            script.Append("\r\n function selectedCheck(i,formName)");
            script.Append("\r\n {");
            script.Append("\r\n\t switch(i)");
            script.Append("\r\n\t {");
           
            script.Append("\r\n\t\t case 2 :");
            script.Append("\r\n\t\t\t formName.monthDateList.options[0].selected=true;");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t\t case 3 :");
            script.Append("\r\n\t\t\t formName.monthDateList.options[0].selected=true;");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t\t case 4 :");
            if (WebApplicationParameters.DataNumberOfYear <= 3)//If start year is N-2 
                script.Append("\r\n\t\t\t formName.CompetitorSudy2Ckbx.checked=false;");
            script.Append("\r\n\t\t\t formName.CompetitorSudy1Ckbx.checked=false;");
            script.Append("\r\n\t\t\t formName.monthDateList.options[0].selected=true;");
            script.Append("\r\n\t\t\t formName.selectedItemIndex.value=i;");
            script.Append("\r\n\t\t\t break;");

            script.Append("\r\n\t\t case 5 :");
            if (WebApplicationParameters.DataNumberOfYear <= 3)//If start year is N-2 
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
            script.Append("\r\n\t\t\t formName.monthDateList.options[0].selected=true;");
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
