using System;
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
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Classification;

/// <summary>
/// Class used to cusotmized product universe
/// </summary>

public partial class Private_Selection_CustomizeItemsSelection : TNS.AdExpress.Web.UI.SelectionWebPage
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
    #endregion

    #region Constructor
    /// <summary>
    /// Constructeur
    /// </summary>
    public Private_Selection_CustomizeItemsSelection()
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
        #endregion



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

        //Component initialisation options
        ComponentsInitOptions();

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

            Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universeDictionary = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            int nbAdvertisers = 0;
            int nbBrands = 0;
            const int maxItemsBylevels = 50;
            List<NomenclatureElementsGroup> nGroups = null;
            bool hasConflictAdvertiser = false, hasConflictBrand = false;
            List<long> oldAdvertisers = null, curAdvertisers = null, oldBrands = null, curBrands = null;

            //Recupération de la sélection
            Dictionary<int, NomenclatureElementsGroup> dicGroups = SelectItemsInClassificationWebControl1.GetNomenclatureGroupsSelection(_webSession, this.Page, TNS.Classification.Universe.Dimension.product, TNS.Classification.Universe.Security.none);
            if (dicGroups != null && dicGroups.Count > 0)
            {

                 foreach (KeyValuePair<int,NomenclatureElementsGroup> kpv in dicGroups){
               
                    if (kpv.Value.Contains(TNSClassificationLevels.ADVERTISER))
                    {
                        curAdvertisers =kpv.Value.Get(TNSClassificationLevels.ADVERTISER);
                        nbAdvertisers += curAdvertisers.Count;
                        if (oldAdvertisers != null)
                        {
                            for (int k = 0; k < oldAdvertisers.Count; k++)
                            {
                                if (curAdvertisers.Contains(oldAdvertisers[k]))
                                {
                                    hasConflictAdvertiser = true; break;
                                }
                            }
                        }
                        oldAdvertisers = curAdvertisers;
                    }
                    if (kpv.Value.Contains(TNSClassificationLevels.BRAND))
                    {
                        curBrands = kpv.Value.Get(TNSClassificationLevels.BRAND);
                        nbBrands += curBrands.Count;
                        if (oldBrands != null)
                        {
                            for (int k = 0; k < oldBrands.Count; k++)
                            {
                                if (curBrands.Contains(oldBrands[k]))
                                {
                                    hasConflictBrand = true; break;
                                }
                            }
                        }
                        oldBrands = curBrands;
                    }
                }
            }

            if (dicGroups != null && dicGroups.Count > 0)
            {
                AdExpressUniverse adExpressUniverse = null;
                foreach (KeyValuePair<int,NomenclatureElementsGroup> kpv in dicGroups){
                      adExpressUniverse = new AdExpressUniverse(Dimension.product);
                      adExpressUniverse.AddGroup(0, kpv.Value);
                      universeDictionary.Add(kpv.Key, adExpressUniverse);
                }               
            }
            _webSession.SecondaryProductUniverses = universeDictionary;
            _webSession.Source.Close();

            int codeError = 0;
            if (hasConflictBrand || hasConflictAdvertiser)
            {
                codeError = (hasConflictAdvertiser) ? 2852 : 2853;
                SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.VALIDATION_NOT_POSSIBLE;
                Response.Write("<script language=javascript>");
                Response.Write("	alert(\"" + GestionWeb.GetWebWord(codeError, _webSession.SiteLanguage) + "\");");
                Response.Write("</script>");
            }
            else if ((nbAdvertisers > maxItemsBylevels) || (nbBrands > maxItemsBylevels))
            {
                codeError = (nbAdvertisers > maxItemsBylevels) ? 1193 : 2851;
                SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.MAX_ELEMENTS;
                Response.Write("<script language=javascript>");
                Response.Write("	alert(\"" + GestionWeb.GetWebWord(codeError, _webSession.SiteLanguage) + "\");");
                Response.Write("</script>");
            }
            else
            {
                _webSession.Save();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");
            }
        }
        catch (TNS.Classification.Universe.SecurityException)
        {
            _webSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.Save();
            SelectItemsInClassificationWebControl1.ErrorCode = FrameWorkSelection.error.SECURITY_EXCEPTION;
            Response.Write("<script language=javascript>");
            Response.Write("	alert(\"" + GestionWeb.GetWebWord(2285, _webSession.SiteLanguage) + "\");");
            Response.Write("</script>");
        }
        catch (TNS.Classification.Universe.CapacityException)
        {
            _webSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
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


    #region Component Init Options
    /// <summary>
    ///  Component Init Options
    /// </summary>
    private void ComponentsInitOptions()
    {

        if (!Page.IsPostBack && _webSession.LastWebPage.IndexOf(this.Page.Request.Url.AbsolutePath) < 0)
        {
            _webSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.Save();
        }

        MenuWebControl2.CustomerWebSession = _webSession;
        SelectItemsInClassificationWebControl1.DBSchema = WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.recap01).Label;
        SelectItemsInClassificationWebControl1.CustomerWebSession = _webSession;
        SelectItemsInClassificationWebControl1.SearchRulesTextCode = 2287;
        SelectItemsInClassificationWebControl1.SearchRulesTextCss = "SearchRulesTextCss";
        SelectItemsInClassificationWebControl1.NbMaxExcludeTree = 0;
        SelectItemsInClassificationWebControl1.ForSelectionPage = false;
        SelectItemsInClassificationWebControl1.DefaultBranchId = 9;//Branche famille par défaut
        SelectItemsInClassificationWebControl1.NbMaxItemByLevel = 1000;
        SelectItemsInClassificationWebControl1.IdSession = _webSession.IdSession;
        SelectItemsInClassificationWebControl1.Dimension_ = TNS.Classification.Universe.Dimension.product;
        SelectItemsInClassificationWebControl1.SiteLanguage = _webSession.SiteLanguage;
        SelectItemsInClassificationWebControl1.ShowAllTree = true;
    }


    #endregion
}
