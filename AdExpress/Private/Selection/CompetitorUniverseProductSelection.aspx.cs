#region Informations
// Auteur: D. Mussuma
// Date de création: 21/11/07
// Date de modification: 

#endregion

using System;
using System.Collections;
using System.ComponentModel;

using System.Data;
using System.Configuration;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using TNS.Classification;
using TNS.Classification.WebControls;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.Core.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using FrameWorkSelection = TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.Classification.Universe;

/// <summary>
/// Page to select competitive product universe
/// </summary>
public partial class Private_Selection_CompetitorUniverseProductSelection : TNS.AdExpress.Web.UI.SelectionWebPage {

	#region Variables
	/// <summary>
	/// fermeture du flash d'attente
	/// </summary>
	public string divClose = "";
	/// <summary>
	/// script ouvrant la popup permettant d'enregistrer un univers
	/// </summary>
	public string saveScript;

	/// <summary>
	/// Event Type (bouton ok, valider...)
	/// </summary>
	protected int eventButton;

	/// <summary>
	///  session Identifier
	/// </summary>
	public string sessionId = "";
	#endregion

	#region Constructor
	/// <summary>
		/// Constructeur
		/// </summary>
	public Private_Selection_CompetitorUniverseProductSelection()
		: base() {
		}
		#endregion

	#region Page loading
	/// <summary>
		/// Page Loading
	/// </summary>
	/// <param name="sender">Sender</param>
		/// <param name="e">Event args</param>
	protected void Page_Load(object sender, EventArgs e) {

		ModuleTitleWebControl1.CustomerWebSession = _webSession;
		InformationWebControl1.Language = _webSession.SiteLanguage;
		sessionId = _webSession.IdSession;
		//Modification de la langue pour les Textes AdExpress
		TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls, _webSession.SiteLanguage);
			

		#region  Images buttons 
		validateButton.ImageUrl = "/Images/" + _siteLanguage + "/button/valider_up.gif";
		validateButton.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/valider_down.gif";
		NextImageButtonRollOverWebControl.ImageUrl = "/Images/" + _siteLanguage + "/button/suivant_up.gif";
		NextImageButtonRollOverWebControl.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/suivant_down.gif";
		//saveUniverseImageButtonRollOverWebControl.ImageUrl = "/Images/" + _siteLanguage + "/button/enregistrer_univers_up.gif";
		//saveUniverseImageButtonRollOverWebControl.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/enregistrer_univers_down.gif";
		
		#endregion

		#region Script
		//Gestion de la sélection d'un radiobutton dans la liste des univers
		if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4")) {
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "InsertIdMySession4", TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
		}
		// Ouverture/fermeture des fenêtres pères
		if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
		}
		#endregion

		#region Events
		
		// Get event target 
		eventButton = 0;
		if (HttpContext.Current.Request.QueryString.Get("saveUnivers") != null) {
			eventButton = int.Parse(HttpContext.Current.Request.QueryString.Get("saveUnivers"));

		}
		// Button validate event
		if (Request.Form.Get("__EVENTTARGET") == "validateButton") {
			eventButton = FrameWorkSelection.eventSelection.VALID_EVENT;
		}
		// Button save universe event
		else if (Request.Form.Get("__EVENTTARGET") == "saveUniverseImageButtonRollOverWebControl") {
			eventButton = FrameWorkSelection.eventSelection.SAVE_EVENT;
		}
		// Button load event
		else if (Request.Form.Get("__EVENTTARGET") == "loadImageButtonRollOverWebControl") {
			eventButton = FrameWorkSelection.eventSelection.LOAD_EVENT;
		}
		// Button recall event
		else if (Request.Form.Get("__EVENTTARGET") == "MenuWebControl2") {
			eventButton = FrameWorkSelection.eventSelection.RECALL_OPTIONS_EVENT;
		}
		else if (Request.Form.Get("__EVENTTARGET") == "initializeImageButtonRollOverWebControl1") {
			eventButton = FrameWorkSelection.eventSelection.INITIALIZE_EVENT;
		}
		// Button next event
		else if (Request.Form.Get("__EVENTTARGET") == "NextImageButtonRollOverWebControl") {
			eventButton = FrameWorkSelection.eventSelection.NEXT_EVENT;
		}
		#endregion

		//if (!Page.IsPostBack) _webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

		#region Universe loading
		// Button load 
		if (eventButton == FrameWorkSelection.eventSelection.LOAD_EVENT) {
			SelectItemsInClassificationWebControl1.EventTarget_ = FrameWorkSelection.eventSelection.LOAD_EVENT;
			if (!loadUniversProduct()) {
				eventButton = -1;
				loadImageButtonRollOverWebControl_Click(null, null);
			}
		}

		//Button save
		if (eventButton == FrameWorkSelection.eventSelection.SAVE_EVENT) {
			SelectItemsInClassificationWebControl1.EventTarget_ = FrameWorkSelection.eventSelection.SAVE_EVENT;
		}
		//Button next
		if (eventButton == FrameWorkSelection.eventSelection.NEXT_EVENT) {
			SelectItemsInClassificationWebControl1.EventTarget_ = FrameWorkSelection.eventSelection.NEXT_EVENT;
		}
		#endregion		

		
		
	}
	#endregion


	#region DeterminePostBack
	/// <summary>
	/// Détermine la valeur de PostBack
	/// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
	/// </summary>
	/// <returns>DeterminePostBackMode</returns>
	protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {

		System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();		
		ComponentsInitOptions();


		if (WebConstantes.Module.Name.BILAN_CAMPAGNE == _webSession.CurrentModule) {
			validateButton.Visible = true;
			idUnivers.Text = "" + GestionWeb.GetWebWord(1677, _webSession.SiteLanguage);//Univers de référence
		}
		else {
			//Univers Label
			int number = _webSession.PrincipalProductUniverses.Count + 1;
			idUnivers.Text = "" + GestionWeb.GetWebWord(977, _webSession.SiteLanguage) + "" + number.ToString();

			validateButton.Visible = (_webSession.PrincipalProductUniverses.Count > 1 && !Page.IsPostBack) ? true : false;
		}
		return (tmp);
	}
	#endregion

	#region Implementation methodes abstraites

	/// <summary>
	/// Event launch to fire validation of the page
	/// </summary>
	/// <param name="sender">Sender Object</param>
	/// <param name="e">Event Arguments</param>
	protected override void ValidateSelection(object sender, System.EventArgs e) {
		this.validateButton_Click(sender,e);
	}
	/// <summary>
	/// Retrieve next Url from the menu
	/// </summary>
	/// <returns>Next Url</returns>
	protected override string GetNextUrlFromMenu() {
		return (this.MenuWebControl2.NextUrl);
	}
	#endregion

	#region Boutton charger
	/// <summary>
	/// load Univers 
	/// </summary>
	protected bool loadUniversProduct() {

		string[] tabParent = null;
		Int64 idUniverse = 0;
		bool selectionnedUnivers = false;

		foreach (string currentKey in Request.Form.AllKeys) {
			tabParent = currentKey.Split('_');
			if (tabParent[0] == "UNIVERSE") {
				idUniverse = Int64.Parse(tabParent[1]);
			}
		}
		if (idUniverse != 0) {
			if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowHideContent1", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
			}

			Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> Universes = (Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetObjectUniverses(idUniverse, _webSession);
			_webSession.PrincipalProductUniverses = Universes;
			idUnivers.Text = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetUniverse(idUniverse, _webSession);
			//_webSession.Save();
			selectionnedUnivers = true;

		}

		return selectionnedUnivers;


	}
	#endregion

	#region loadImageButtonRollOverWebControl_Click
	/// <summary>
	/// Universe loading
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Arguments</param>
	private void loadImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		if (eventButton != FrameWorkSelection.eventSelection.LOAD_EVENT) {
			Response.Write("<script language=javascript>");
			Response.Write("	alert(\"" + GestionWeb.GetWebWord(926, _webSession.SiteLanguage) + "\");");
			Response.Write("history.go(-1);");
			Response.Write("</script>");
		}
	}
	#endregion

	#region Component Init Options
	/// <summary>
	///  Component Init Options
	/// </summary>
	private void ComponentsInitOptions() {

		if (!Page.IsPostBack && _webSession.LastWebPage.IndexOf(this.Page.Request.Url.AbsolutePath) < 0) _webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();//&& !_webSession.LastWebPage.Equals(this.Page.Request.Url.OriginalString)) _webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();//&& !_webSession.LastWebPage.Equals(this.Page.Request.Url.OriginalString)
		MenuWebControl2.CustomerWebSession = _webSession;
		SelectItemsInClassificationWebControl1.CustomerWebSession = _webSession;
		SelectItemsInClassificationWebControl1.SearchRulesTextCode = 2287;
		SelectItemsInClassificationWebControl1.SearchRulesTextCss = "SearchRulesTextCss";
		LoadableUniversWebControl1.CustomerWebSession = _webSession;
		LoadableUniversWebControl1.ListBranchType = TNS.AdExpress.Constantes.Classification.Branch.type.product.GetHashCode().ToString();
		LoadableUniversWebControl1.Dimension_ = TNS.Classification.Universe.Dimension.product;
		LoadableUniversWebControl1.ForGenericUniverse = true;
		LoadableUniversWebControl1.SelectionPage = true;

		switch (_webSession.CurrentModule) {
			case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
			case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
			case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
			case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
			case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
			case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
			case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
			case WebConstantes.Module.Name.ALERTE_POTENTIELS:

				SelectItemsInClassificationWebControl1.ForSelectionPage = false;
				LoadableUniversWebControl1.SelectionPage = false;
				break;
			case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
			case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
				SelectItemsInClassificationWebControl1.DefaultBranchId = 2;//Branche annonceur par défaut
				break;
			case WebConstantes.Module.Name.BILAN_CAMPAGNE:
				SelectItemsInClassificationWebControl1.DefaultBranchId = 4;
				break;
		}

		SelectItemsInClassificationWebControl1.IdSession = _webSession.IdSession;
		SelectItemsInClassificationWebControl1.Dimension_ = TNS.Classification.Universe.Dimension.product;
		SelectItemsInClassificationWebControl1.SiteLanguage = _webSession.SiteLanguage;
	}
	#endregion
	
	#region saveUniverseImageButtonRollOverWebControl_Click
	///// <summary>
	///// Save universe method
	///// </summary>
	///// <param name="sender">Sender</param>
	///// <param name="e">Event args</param>
	//protected void saveUniverseImageButtonRollOverWebControl_Click(object sender, EventArgs e) {
		
	//}
	#endregion

	#region NextImageButtonRollOverWebControl_Click
	/// <summary>
	/// Go to next universe selection
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void NextImageButtonRollOverWebControl_Click(object sender, EventArgs e) {
		try {
			//Annuler l'univers de version
			if ( _webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE) {
				_webSession.IdSlogans = new ArrayList();
				_webSession.SloganIdZoom = -1;				
			}
			if (idUnivers.Text.Length > 0) {
				if (!HasAlreadyThisUniverseName(_webSession.PrincipalProductUniverses, idUnivers.Text)) {
					TNS.AdExpress.Classification.AdExpressUniverse universe = SelectItemsInClassificationWebControl1.GetSelection(_webSession, this.Page, TNS.Classification.Universe.Dimension.product, TNS.Classification.Universe.Security.full, idUnivers.Text);
					if (universe != null && universe.Count() > 0) {
						List<NomenclatureElementsGroup> nGroups = universe.GetIncludes();
						if ((MustSelectIncludeItems() && nGroups != null && nGroups.Count > 0) || !MustSelectIncludeItems()) {
							_webSession.PrincipalProductUniverses.Add(_webSession.PrincipalProductUniverses.Count, universe);
							_webSession.Save();
							DBFunctions.closeDataBase(_webSession);
							//Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");
							if (WebConstantes.Module.Name.BILAN_CAMPAGNE == _webSession.CurrentModule) {
								idUnivers.Text = "" + GestionWeb.GetWebWord(1678, _webSession.SiteLanguage);
								//Max 1 Competitor universe
								if (_webSession.PrincipalProductUniverses.Count >= 1) NextImageButtonRollOverWebControl.Visible = false;
							}
							else {
								//Univers Label
								int number = _webSession.PrincipalProductUniverses.Count + 1;
								idUnivers.Text = "" + GestionWeb.GetWebWord(977, _webSession.SiteLanguage) + "" + number.ToString();
								validateButton.Visible = true;
								//Max 5 Competitor universes
								if (_webSession.PrincipalProductUniverses.Count >= 4) NextImageButtonRollOverWebControl.Visible = false;
							}
						}
						else {
							SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE;
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\"" + GestionWeb.GetWebWord(2299, _webSession.SiteLanguage) + "\");");
							Response.Write("</script>");
						}				
					}
					else {
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\"" + GestionWeb.GetWebWord(878, _webSession.SiteLanguage) + "\");");
						Response.Write("</script>");
					}
				}
				else {
					SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE;
					//Universe name already used
					Response.Write("<script language=javascript>");
					Response.Write("alert(\"" + GestionWeb.GetWebWord(923, _webSession.SiteLanguage) + "\");");
					//Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
			}
			else {
				SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE;
				// Text universe is null
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\"" + GestionWeb.GetWebWord(976, _webSession.SiteLanguage) + "\");");
				//Response.Write("history.go(-1);");
				Response.Write("</script>");

			}
		}
		catch (TNS.Classification.Universe.SecurityException) {
			//_webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
			//_webSession.Save();
			SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.SECURITY_EXCEPTION;
			Response.Write("<script language=javascript>");
			Response.Write("	alert(\"" + GestionWeb.GetWebWord(2285, _webSession.SiteLanguage) + "\");");//TODO:texte a definir
			//Response.Write("history.go(-1);");
			Response.Write("</script>");
		}
		catch (TNS.Classification.Universe.CapacityException) {
			//_webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
			//_webSession.Save();
			SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.MAX_ELEMENTS;
			Response.Write("<script language=javascript>");
			Response.Write("alert(\"" + GestionWeb.GetWebWord(2286, _webSession.SiteLanguage) + "\");");
			//Response.Write("history.go(-1);");
			Response.Write("</script>");
		}
		catch (Exception) {
			Response.Write("<script language=javascript>");
			Response.Write("	alert(\"" + GestionWeb.GetWebWord(922, _webSession.SiteLanguage) + "\");");
			Response.Write("</script>");
		}
	}
	#endregion

	#region validateButton_Click
	/// <summary>
	/// Validation selection method
	/// </summary>
	/// <param name="sender">sender</param>
	/// <param name="e">Event args</param>
	protected void validateButton_Click(object sender, EventArgs e) {
		try {
			//Annuler l'univers de version
			if (_webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE) {
				_webSession.IdSlogans = new ArrayList();
				_webSession.SloganIdZoom = -1;
			}
			if (idUnivers.Text.Length > 0) {
				if (!HasAlreadyThisUniverseName(_webSession.PrincipalProductUniverses,idUnivers.Text)) {
					 TNS.AdExpress.Classification.AdExpressUniverse universe = SelectItemsInClassificationWebControl1.GetSelection(_webSession, this.Page, TNS.Classification.Universe.Dimension.product, TNS.Classification.Universe.Security.full, idUnivers.Text);
					if (universe != null && universe.Count() > 0) {
											 List<NomenclatureElementsGroup> nGroups = universe.GetIncludes(); 

						if ((MustSelectIncludeItems() && nGroups != null && nGroups.Count > 0) || !MustSelectIncludeItems()) {
							_webSession.PrincipalProductUniverses.Add(_webSession.PrincipalProductUniverses.Count, universe);
							_webSession.Save();
							DBFunctions.closeDataBase(_webSession);
							Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");
						}
						else {
							SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE;
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\"" + GestionWeb.GetWebWord(2299, _webSession.SiteLanguage) + "\");");
							Response.Write("</script>");
						}
					}
					else {
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\"" + GestionWeb.GetWebWord(878, _webSession.SiteLanguage) + "\");");
						Response.Write("</script>");
					}
				}
				else {
					SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE;
					//Universe name already used
					Response.Write("<script language=javascript>");
					Response.Write("alert(\"" + GestionWeb.GetWebWord(923, _webSession.SiteLanguage) + "\");");
					//Response.Write("history.go(-1);");
					Response.Write("</script>");
				}
			}
			else {
				SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE;
				// Text universe is null
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\"" + GestionWeb.GetWebWord(976, _webSession.SiteLanguage) + "\");");
				//Response.Write("history.go(-1);");
				Response.Write("</script>");

			}
		}
		catch (TNS.Classification.Universe.SecurityException) {
			//_webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
			//_webSession.Save();
			SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.SECURITY_EXCEPTION;
			Response.Write("<script language=javascript>");
			Response.Write("	alert(\"" + GestionWeb.GetWebWord(2285, _webSession.SiteLanguage) + "\");");//TODO:texte a definir
			//Response.Write("history.go(-1);");
			Response.Write("</script>");
		}
		catch (TNS.Classification.Universe.CapacityException) {
			//_webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
			//_webSession.Save();
			SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.MAX_ELEMENTS;
			Response.Write("<script language=javascript>");
			Response.Write("alert(\"" + GestionWeb.GetWebWord(2286, _webSession.SiteLanguage) + "\");");
			//Response.Write("history.go(-1);");
			Response.Write("</script>");
		}
		catch (Exception) {
			Response.Write("<script language=javascript>");
			Response.Write("	alert(\"" + GestionWeb.GetWebWord(922, _webSession.SiteLanguage) + "\");");
			Response.Write("</script>");
		}

	}
	#endregion	

	#region Methods internal
	/// <summary>
	/// Check if the current name is alreay used for a register universe
	/// </summary>
	/// <param name="universeDictionary">Universe dictionary</param>
	/// <param name="name">name</param>
	/// <returns>true if name is already used for this universe</returns>
	private bool HasAlreadyThisUniverseName(Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universeDictionary,string name) {
		if (name != null && name.Length > 0) {
			for (int i = 0; i < universeDictionary.Count; i++) {
				if (universeDictionary[i].Label.Equals(name)) return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Check if include items must be seleted
	/// </summary>
	/// <returns></returns>
	private bool MustSelectIncludeItems() {
		switch (_webSession.CurrentModule) {			
			case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
			case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
			case WebConstantes.Module.Name.BILAN_CAMPAGNE:
				return true;
			default: return false;
		}
	}
	#endregion
}
