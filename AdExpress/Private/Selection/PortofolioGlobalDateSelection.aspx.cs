#region Informations
// Auteur: D. Mussuma
// Date de création: 7/080/2008
// Date de modification: 
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using AdExpressException = AdExpress.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using Portofolio = TNS.AdExpressI.Portofolio;
using Domain = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Date;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

/// <summary>
/// Page de sélection des dates (global)
/// </summary>
public partial class Private_Selection_PortofolioGlobalDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage {

	#region Variables
	/// <summary>
	/// Option de période sélectionné
	/// </summary>
	int selectedIndex = -1;
	/// <summary>
	/// Pour verifier si on a sélectionné une période
	/// </summary>
	public string testSelection = "";
	/// <summary>
	/// Renvoie vraie si on est dans le module dynamique
	/// </summary>
	public bool isDynamicModule = false;
	/// <summary>
	/// Le type de la disponibilité des données
	/// </summary>
	public WebConstantes.globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.currentDay;
	/// <summary>
	/// Type de la période comparative
	/// </summary>
	public WebConstantes.globalCalendar.comparativePeriodType comparativePeriodCalendarType = WebConstantes.globalCalendar.comparativePeriodType.dateToDate;

	#endregion

	#region Evènements

	#region Chargement de la page
	/// <summary>
	/// Evènement de chargement de la page
	/// </summary>
	/// <param name="sender">Objet qui lance l'évènement</param>
	/// <param name="e">Argument</param>
	protected void Page_Load(object sender, System.EventArgs e) {
		try {
            #region Test Cedexis
            //Test Cedexis
            if (WebApplicationParameters.CountryCode == TNS.AdExpress.Constantes.Web.CountryCode.FRANCE &&
            !Page.ClientScript.IsClientScriptBlockRegistered("CedexisScript"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "CedexisScript", TNS.AdExpress.Web.Functions.Script.CedexisScript());
            }
            #endregion

			#region Option de période sélectionnée
			if (Request.Form.GetValues("selectedItemIndex") != null) selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
			#endregion

			#region Textes et langage du site
			//Modification de la langue pour les Textes AdExpress
            //for (int i = 0; i < this.Controls.Count; i++) {
            //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
            //}
			ModuleTitleWebControl1.CustomerWebSession = _webSession;
			InformationWebControl1.Language = _webSession.SiteLanguage;

            validateButton1.ImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_up.gif";
            validateButton1.RollOverImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_down.gif";
            validateButton1.Attributes.Add("style", "cursor:pointer;");

            validateButton2.ImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_up.gif";
            validateButton2.RollOverImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_down.gif";
            validateButton2.Attributes.Add("style", "cursor:pointer;");
			#endregion

			string selectionType = "";
			string disponibilityType = "";


            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _webSession;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

			if (_webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA) {

				if (Page.Request.Form.GetValues("selectionType") != null) selectionType = Page.Request.Form.GetValues("selectionType")[0];

				if (Page.Request.Form.GetValues("disponibilityType") != null) disponibilityType = Page.Request.Form.GetValues("disponibilityType")[0];

				if (selectionType.Equals("dateWeekComparative"))
					comparativePeriodCalendarType = WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate;

				if (disponibilityType.Equals("lastPeriod"))
					periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod;

			}

			GlobalCalendarWebControl1.PeriodSelectionTitle = GestionWeb.GetWebWord(2275, _webSession.SiteLanguage);
			GlobalCalendarWebControl1.Language = _webSession.SiteLanguage;

			
		    GlobalCalendarWebControl1.PeriodRestrictedLabel = GestionWeb.GetWebWord(2284, _webSession.SiteLanguage);
            GlobalCalendarWebControl1.StartYear = dateDAL.GetCalendarStartDate();
			

            GlobalCalendarWebControl1.IsRestricted = ModulesList.GetModule(_webSession.CurrentModule).DisplayIncompleteDateInCalendar;
            List<Int64> selectedVehicleList = new List<Int64>();
            if (GlobalCalendarWebControl1.IsRestricted) {
                string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                if (vehicleSelection == null) throw (new VehicleException("Selection of media type is not correct"));
                selectedVehicleList = new List<Int64>((new List<string>(vehicleSelection.Split(','))).ConvertAll<Int64>(ConvertStringToInt64));

                if (IsPostBack) {
                    if (this.ViewState["FirstDayNotEnabledVS"] != null)
                        GlobalCalendarWebControl1.FirstDayNotEnable = (DateTime)this.ViewState["FirstDayNotEnabledVS"];
                }
                else {
                    GlobalCalendarWebControl1.FirstDayNotEnable = dateDAL.GetFirstDayNotEnabled(selectedVehicleList, GlobalCalendarWebControl1.StartYear);
                    ViewState.Add("FirstDayNotEnabledVS", GlobalCalendarWebControl1.FirstDayNotEnable);
                }
            }

			#region Script
			//Gestion de la sélection comparative
			if (!Page.ClientScript.IsClientScriptBlockRegistered("PostBack")) {
				if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PostBack", TNS.AdExpress.Web.Functions.Script.PostBack(_webSession.SiteLanguage, true, validateButton1.ID, validateButton2.ID, "", "comparativeLink", monthDateList.ID, weekDateList.ID, dayDateList.ID, previousWeekCheckBox.ID, previousDayCheckBox.ID, currentYearCheckbox.ID, previousYearCheckbox.ID, previousMonthCheckbox.ID, "dateSelectedItem"));
				else
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PostBack", TNS.AdExpress.Web.Functions.Script.PostBack(_webSession.SiteLanguage, false, validateButton1.ID, validateButton2.ID, "", "comparativeLink", monthDateList.ID, weekDateList.ID, dayDateList.ID, previousWeekCheckBox.ID, previousDayCheckBox.ID, currentYearCheckbox.ID, previousYearCheckbox.ID, previousMonthCheckbox.ID, "dateSelectedItem"));
			}
			#endregion
		}
		catch (System.Exception exc) {
			if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
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
		try {
			if (this.IsPostBack) {

				string valueInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];

				#region Url Suivante
				if (_nextUrlOk) {
					if (selectedIndex >= 0 && selectedIndex <= 8)
						validateButton2_Click(this, null);
					else
						validateButton1_Click(this, null);
				}
				else {
					if (valueInput == validateButton1.ID)
						validateButton1_Click(this, null);
					else
						validateButton2_Click(this, null);
				}
				#endregion
			}
		}
		catch (System.Exception exc) {
			if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
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
		_webSession.Source.Close();
		_webSession.Save();
	}
	#endregion

	#region DeterminePostBackMode
	/// <summary>
	/// On l'utilise pour l'initialisation de certains composants
	/// </summary>
	/// <returns>?</returns>
	protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
		System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
        try {
		yearDateList.WebSession = _webSession;
        yearDateList.NbYearsToDisplay = WebApplicationParameters.DataNumberOfYear;
		monthDateList.WebSession = _webSession;
		weekDateList.WebSession = _webSession;
		dayDateList.WebSession = _webSession;
		MenuWebControl2.CustomerWebSession = _webSession;

		previousWeekCheckBox.Language = previousMonthCheckbox.Language = previousYearCheckbox.Language = previousDayCheckBox.Language = currentYearCheckbox.Language = _webSession.SiteLanguage;
		if (VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID).Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press
			|| VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID).Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress
            || VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID).Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.newspaper
            || VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID).Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.magazine
            ) {
			Dictionary<string, string> parutionsDates = GetDatesParution();
			if (parutionsDates != null && parutionsDates.Count > 0) {
				GlobalCalendarWebControl1.IdDivCover = "div_press_Cover";
				GlobalCalendarWebControl1.IdVisualCover = "visual_cover";
				GlobalCalendarWebControl1.ParutionDateList = parutionsDates;
				GlobalCalendarWebControl1.WithParutionDates = true;
			}
		}
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

	#region Validation du calendrier
	/// <summary>
	/// Evènement de validation du calendrier
	/// </summary>
	/// <param name="sender">Objet qui lance l'évènement</param>
	/// <param name="e">Argument</param>
	protected void validateButton1_Click(object sender, System.EventArgs e) {
		try {
			calendarValidation();
			_webSession.Source.Close();
			Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
		}
		catch (System.Exception ex) {
			testSelection = "<script language=\"JavaScript\">alert(\"" + ex.Message + "\");</script>";
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
		try {
            CoreLayer cl = WebApplicationParameters.CoreLayers[WebConstantes.Layers.Id.date];
            IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);
            int selectedValue = -1;

            switch (selectedIndex) {
                case 0:
                    selectedValue = int.Parse(yearDateList.SelectedValue);
                    break;
                case 1:
                    selectedValue = int.Parse(monthDateList.SelectedValue);
                    break;
                case 2:
                    selectedValue = int.Parse(weekDateList.SelectedValue);
                    break;
                case 3:
                    selectedValue = int.Parse(dayDateList.SelectedValue);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    break;
                default:
                    throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
            }

            date.SetDate(ref _webSession, GlobalCalendarWebControl1.FirstDayNotEnable, periodCalendarDisponibilityType, comparativePeriodCalendarType, selectedIndex, selectedValue);

			_webSession.Source.Close();
			Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
		}
		catch (AdExpressException.AnalyseDateSelectionException ex) {
			testSelection = "<script language=\"JavaScript\">alert(\"" + ex.Message + "\");</script>";
		}
	}
	#endregion

	#endregion

	#region Méthodes internes

	#region Validation des calendriers
	/// <summary>
	/// Traitement des dates d'un calendrier
	/// </summary>
	public void calendarValidation() {
		// On sauvegarde les données
        try
        {
            DateTime endDate;
            DateTime beginDate;
            DateTime lastDayEnable = DateTime.Now;

            _webSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
            _webSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
            _webSession.PeriodBeginningDate = GlobalCalendarWebControl1.SelectedStartDate.ToString();
            _webSession.PeriodEndDate = GlobalCalendarWebControl1.SelectedEndDate.ToString();

            endDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6, 2)));
            beginDate = new DateTime(Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(6, 2)));


            if (CompareDateEnd(DateTime.Now, endDate) || CompareDateEnd(beginDate, DateTime.Now))
                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
            else
                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));


            _webSession.Save();
        }
        catch (System.Exception e)
        {
            _webSession.PeriodBeginningDate = "";
            _webSession.PeriodEndDate = "";
            throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
        }
	}
	#endregion

	#region comparaison entre la date de fin et la date d'aujourd'hui
	/// <summary>
	/// Verifie si la date de fin est inférieur ou non à la date de début
	/// </summary>
	/// <returns>vrai si la date de fin et inférieur à la date de début</returns>
	private bool CompareDateEnd(DateTime dateBegin, DateTime dateEnd) {
		if (dateEnd < dateBegin)
			return true;
		else
			return false;
	}
	#endregion

	#endregion

	#region Implémentation méthodes abstraites
	protected override void ValidateSelection(object sender, System.EventArgs e) {
		this.Page_PreRender(sender, e);
	}
	protected override string GetNextUrlFromMenu() {
		return (this.MenuWebControl2.NextUrl);
	}
	#endregion

	#region Gets dates parution
	/// <summary>
	/// Get dates parution
	/// </summary>
	/// <returns>Dates parution</returns>
	protected Dictionary<string, string> GetDatesParution() {
		
		Domain.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
		if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
		object[] parameters = new object[2];
		parameters[0] = _webSession;
		parameters[1] = TNS.AdExpress.Constantes.DB.TableType.Type.dataVehicle;
		Portofolio.IPortofolioResults portofolioResult = (Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
        int nbYears = WebApplicationParameters.DataNumberOfYear-1;
        return portofolioResult.GetVisualList(DateTime.Now.AddYears(-nbYears).ToString("yyyy0101"), DateTime.Now.ToString("yyyyMMdd"));
		//return null;
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
}
