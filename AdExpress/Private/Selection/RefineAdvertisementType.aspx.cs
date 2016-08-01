﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
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
using TNS.AdExpress.Domain.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using FrameWorkSelection = TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;

/// <summary>
/// Class used to select product universe
/// </summary>
public partial class Private_Selection_RefineAdvertisementType : TNS.AdExpress.Web.UI.SelectionWebPage
{

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
    /// <summary>
    ///  session Identifier
    /// </summary>
    public string brancheType = TNS.AdExpress.Constantes.Classification.Branch.type.advertisementType.GetHashCode().ToString();
    /// <summary>
    /// Determnine if keep old refine selection
    /// </summary>
    protected bool _keepRefineUniverseSelection = false;
    protected bool _isSelectionPage = false;
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public Private_Selection_RefineAdvertisementType()
        : base()
    {
    }
    #endregion

    #region Page_Load
    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            ModuleTitleWebControl1.CustomerWebSession = _webSession;
            InformationWebControl1.Language = _webSession.SiteLanguage;
            sessionId = _webSession.IdSession;
            HeaderWebControl1.Language = _webSession.SiteLanguage;

            #region Script
            //Gestion de la sélection d'un radiobutton dans la liste des univers
            if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "InsertIdMySession4", TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
            }
            // Ouverture/fermeture des fenêtres pères
            if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
            }
            #endregion

            #region Evènemment
            // Connaître le boutton qui a été cliqué 
            eventButton = 0;
            if (HttpContext.Current.Request.QueryString.Get("saveUnivers") != null)
            {
                eventButton = int.Parse(HttpContext.Current.Request.QueryString.Get("saveUnivers"));

            }
            // Boutton valider
            if (Request.Form.Get("__EVENTTARGET") == "validateButton")
            {
                eventButton = FrameWorkSelection.eventSelection.VALID_EVENT;
            }
            // Boutton sauvegarder
            else if (Request.Form.Get("__EVENTTARGET") == "saveUniverseImageButtonRollOverWebControl")
            {
                eventButton = FrameWorkSelection.eventSelection.SAVE_EVENT;
            }
            // Boutton Charger
            else if (Request.Form.Get("__EVENTTARGET") == "loadImageButtonRollOverWebControl")
            {
                eventButton = FrameWorkSelection.eventSelection.LOAD_EVENT;
            }
            // Controle Recall
            else if (Request.Form.Get("__EVENTTARGET") == "MenuWebControl2")
            {
                eventButton = FrameWorkSelection.eventSelection.RECALL_OPTIONS_EVENT;
            }
            else if (Request.Form.Get("__EVENTTARGET") == "initializeImageButtonRollOverWebControl1")
            {
                eventButton = FrameWorkSelection.eventSelection.INITIALIZE_EVENT;
            }

            if (!Page.IsPostBack && _keepRefineUniverseSelection)
            {
                eventButton = FrameWorkSelection.eventSelection.INITIALIZE_WITH_PREVIOUS_SELECTION_EVENT;
                SelectItemsInClassificationWebControl1.EventTarget_ = FrameWorkSelection.eventSelection.INITIALIZE_WITH_PREVIOUS_SELECTION_EVENT;
            }
            #endregion

            #region Chargement Univers
            // Bouton Charger 
            if (eventButton == FrameWorkSelection.eventSelection.LOAD_EVENT)
            {
                SelectItemsInClassificationWebControl1.EventTarget_ = FrameWorkSelection.eventSelection.LOAD_EVENT;
                if (!loadUniversProduct())
                {
                    eventButton = -1;
                    loadImageButtonRollOverWebControl_Click(null, null);
                }
            }

            //Bouton Enregister
            if (eventButton == FrameWorkSelection.eventSelection.SAVE_EVENT)
            {
                SelectItemsInClassificationWebControl1.EventTarget_ = FrameWorkSelection.eventSelection.SAVE_EVENT;
            }
            #endregion


            _webSession.Save();
        }
        catch (System.Exception exc)
        {
            if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
            {
                this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
            }
        }
    }
    #endregion

    #region DeterminePostBack
    /// <summary>
    /// Détermine la valeur de PostBack
    /// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
    /// </summary>
    /// <returns>DeterminePostBackMode</returns>
    protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
    {

        System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
        try
        {
            //Component initialisation options
            ComponentsInitOptions();
        }
        catch (System.Exception exc)
        {
            if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
            {
                this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
            }
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
    protected override void ValidateSelection(object sender, System.EventArgs e)
    {
        this.validateButton_Click(sender, e);
    }
    /// <summary>
    /// Retrieve next Url from the menu
    /// </summary>
    /// <returns>Next Url</returns>
    protected override string GetNextUrlFromMenu()
    {
        return (this.MenuWebControl2.NextUrl);
    }
    #endregion

    #region Bouton Valider
    /// <summary>
    /// Validate button
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">Event args</param>
    protected void validateButton_Click(object sender, EventArgs e)
    {
        try
        {

            //Recupération de la sélection
            TNS.AdExpress.Classification.AdExpressUniverse universe = SelectItemsInClassificationWebControl1.GetSelection(_webSession, this.Page, TNS.Classification.Universe.Dimension.advertisementType, TNS.Classification.Universe.Security.full);
            if (universe != null && universe.Count() > 0)
            {
                List<NomenclatureElementsGroup> nGroups = universe.GetIncludes();
                if ((MustSelectIncludeItems() && nGroups != null && nGroups.Count > 0) || !MustSelectIncludeItems())
                {
                    Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universeDictionary = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                    universeDictionary.Add(universeDictionary.Count, universe);
                    _webSession.AdvertisementTypeUniverses = universeDictionary;
                    _webSession.Save();
                    _webSession.Source.Close();
                    Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");
                }
                else
                {
                    SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.SECURITY_EXCEPTION;
                    Response.Write("<script language=javascript>");
                    Response.Write("	alert(\"" + GestionWeb.GetWebWord(2299, _webSession.SiteLanguage) + "\");");
                    Response.Write("</script>");

                }
            }
            else if (!_isSelectionPage)
            {
                _webSession.AdvertisementTypeUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                _webSession.Save();
                _webSession.Source.Close();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");
            }
            else
            {
                Response.Write("<script language=javascript>");
                Response.Write("	alert(\"" + GestionWeb.GetWebWord(878, _webSession.SiteLanguage) + "\");");
                Response.Write("</script>");
            }
        }
        catch (TNS.Classification.Universe.SecurityException)
        {
            _webSession.AdvertisementTypeUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.Save();
            SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.SECURITY_EXCEPTION;
            Response.Write("<script language=javascript>");
            Response.Write("	alert(\"" + GestionWeb.GetWebWord(2285, _webSession.SiteLanguage) + "\");");
            Response.Write("</script>");
        }
        catch (TNS.Classification.Universe.CapacityException)
        {
            _webSession.AdvertisementTypeUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.Save();
            SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.MAX_ELEMENTS;
            Response.Write("<script language=javascript>");
            Response.Write("alert(\"" + GestionWeb.GetWebWord(2286, _webSession.SiteLanguage) + "\");");
            Response.Write("</script>");
        }
        catch (Exception)
        {
            Response.Write("<script language=javascript>");
            Response.Write("	alert(\"" + GestionWeb.GetWebWord(922, _webSession.SiteLanguage) + "\");");
            Response.Write("</script>");
        }

    }
    #endregion

    #region Sauvegarder univers
    /// <summary>
    /// Save Universe
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    protected void saveUniverseImageButtonRollOverWebControl_Click(object sender, EventArgs e)
    {
        Int64 idUniverseClientDescription = -1;
        TNS.AdExpress.Constantes.Classification.Branch.type branchType = TNS.AdExpress.Constantes.Classification.Branch.type.advertisementType;
        idUniverseClientDescription = WebConstantes.LoadableUnivers.GENERIC_UNIVERSE;
        try
        {
            //Recupération de la sélection
            TNS.AdExpress.Classification.AdExpressUniverse universe = SelectItemsInClassificationWebControl1.GetSelection(_webSession, this.Page, TNS.Classification.Universe.Dimension.advertisementType, TNS.Classification.Universe.Security.full);
            //if (universe != null && universe.Count() > 0)
            //{
            Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universeDictionary = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            universeDictionary.Add(universeDictionary.Count, universe);
            _webSession.AdvertisementTypeUniverses = universeDictionary;
            _webSession.Save();
            _webSession.Source.Close();
            saveScript = "window.showModalDialog('/Private/Universe/RegisterUniverse.aspx?idSession=" + _webSession.IdSession + "&brancheType=" + branchType
                + "&idUniverseClientDescription=" + idUniverseClientDescription + "&atd=" + DateTime.Now.ToString("yyyyMMddhhmmss") + " ',null, 'dialogHeight:300px;dialogWidth:450px;help:no;resizable:no;scroll:no;status:no;');";
            //}
            //else
            //{
            //    Response.Write("<script language=javascript>");
            //    Response.Write("	alert(\"" + GestionWeb.GetWebWord(878, _webSession.SiteLanguage) + "\");");
            //    Response.Write("</script>");
            //}
        }
        catch (TNS.Classification.Universe.SecurityException)
        {
            _webSession.AdvertisementTypeUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.Save();
            SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.SECURITY_EXCEPTION;
            Response.Write("<script language=javascript>");
            Response.Write("alert(\"" + GestionWeb.GetWebWord(2285, _webSession.SiteLanguage) + "\");");
            Response.Write("</script>");
        }
        catch (TNS.Classification.Universe.CapacityException)
        {
            _webSession.AdvertisementTypeUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.Save();
            SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.MAX_ELEMENTS;
            Response.Write("<script language=javascript>");
            Response.Write("alert(\"" + GestionWeb.GetWebWord(2286, _webSession.SiteLanguage) + "\");");
            Response.Write("</script>");
        }
        catch (Exception)
        {
            Response.Write("<script language=javascript>");
            Response.Write("	alert(\"" + GestionWeb.GetWebWord(922, _webSession.SiteLanguage) + "\");");
            Response.Write("</script>");
        }
    }
    #endregion

    #region Boutton charger
    /// <summary>
    /// load Univers 
    /// </summary>
    protected bool loadUniversProduct()
    {

        string[] tabParent = null;
        Int64 idUniverse = 0;
        bool selectionnedUnivers = false;

        foreach (string currentKey in Request.Form.AllKeys)
        {
            tabParent = currentKey.Split('_');
            if (tabParent[0] == "UNIVERSE")
            {
                idUniverse = Int64.Parse(tabParent[1]);
            }
        }
        if (idUniverse != 0)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowHideContent1", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
            }

            Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> Universes = (Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetObjectUniverses(idUniverse, _webSession);
            _webSession.AdvertisementTypeUniverses = Universes;
            _webSession.Save();
            selectionnedUnivers = true;

        }

        return selectionnedUnivers;


    }
    #endregion

    #region Charger
    /// <summary>
    /// Universe loading
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    private void loadImageButtonRollOverWebControl_Click(object sender, System.EventArgs e)
    {
        if (eventButton != FrameWorkSelection.eventSelection.LOAD_EVENT)
        {
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
    private void ComponentsInitOptions()
    {

        string dbSchema = WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label;

        ReInitializeSelection(_isSelectionPage);
        MenuWebControl2.CustomerWebSession = _webSession;
        SelectItemsInClassificationWebControl1.DBSchema = dbSchema;
        SelectItemsInClassificationWebControl1.CustomerWebSession = _webSession;
        SelectItemsInClassificationWebControl1.SearchRulesTextCode = 2287;
        SelectItemsInClassificationWebControl1.SearchRulesTextCss = "SearchRulesTextCss";
        LoadableUniversWebControl1.CustomerWebSession = _webSession;
        LoadableUniversWebControl1.ListBranchType = TNS.AdExpress.Constantes.Classification.Branch.type.advertisementType.GetHashCode().ToString();
        LoadableUniversWebControl1.Dimension_ = TNS.Classification.Universe.Dimension.advertisementType;
        LoadableUniversWebControl1.ForGenericUniverse = true;
        LoadableUniversWebControl1.SelectionPage = _isSelectionPage;
        SelectItemsInClassificationWebControl1.ForSelectionPage = _isSelectionPage;

        if (_isSelectionPage)
        {
            foreach (SelectionPageInformation currentPage in _currentModule.SelectionsPages)
            {
                if (currentPage.Url.Equals(this.Page.Request.Url.AbsolutePath))
                    SelectItemsInClassificationWebControl1.DefaultBranchId = currentPage.DefaultBranchId;
            }
        }
        else
        {
            foreach (OptionalPageInformation currentPage in _currentModule.OptionalsPages)
            {
                if (currentPage.Url.Equals(this.Page.Request.Url.AbsolutePath))
                    SelectItemsInClassificationWebControl1.DefaultBranchId = currentPage.DefaultBranchId;
            }
        }
        SelectItemsInClassificationWebControl1.IdSession = _webSession.IdSession;
        SelectItemsInClassificationWebControl1.Dimension_ = TNS.Classification.Universe.Dimension.advertisementType;
        SelectItemsInClassificationWebControl1.SiteLanguage = _webSession.SiteLanguage;
    }

    /// <summary>
    /// Check if include items must be seleted
    /// </summary>
    /// <returns></returns>
    private bool MustSelectIncludeItems()
    {
        return false;
    }

    private void ReInitializeSelection(bool isSelectionPage)
    {
        _keepRefineUniverseSelection = !isSelectionPage && WebApplicationParameters.KeepRefineUniverseSelection;
        if (!_keepRefineUniverseSelection)
        {
            if (!Page.IsPostBack && _webSession.LastWebPage.IndexOf(this.Page.Request.Url.AbsolutePath) < 0)
                _webSession.AdvertisementTypeUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
        }

    }
    #endregion
}