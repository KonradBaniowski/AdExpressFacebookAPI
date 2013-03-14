﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using WebFunctions = TNS.AdExpress.Web.Functions;

public partial class Private_Results_AdvertisingAgencyResults : TNS.AdExpress.Web.UI.BaseResultWebPage
{

    #region Variables
    /// <summary>
    /// Script de fermeture du flash d'attente
    /// </summary>
    public string divClose = LoadingSystem.GetHtmlCloseDiv();
    #endregion

    #region Constructeur
    /// <summary>
    /// Constructeur
    /// </summary>
    public Private_Results_AdvertisingAgencyResults()
        : base()
    {
    }
    #endregion

    #region Evènements

    #region Chargement de la page
    /// <summary>
    /// Evènement de chargement de la page
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            #region Flash d'attente
            if (Page.Request.Form.GetValues("__EVENTTARGET") != null)
            {
                string nomInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];
                if (nomInput != MenuWebControl2.ID)
                {
                    Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                    Page.Response.Flush();
                }
            }
            else
            {
                Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                Page.Response.Flush();
            }  
            #endregion

            #region Url Suivante
            if (_nextUrl.Length != 0)
            {
                _webSession.Source.Close();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            }
            #endregion

            #region Validation du menu
            if (Page.Request.QueryString.Get("validation") == "ok")
            {
                _webSession.Save();
            }
            #endregion

            #region Texte et langage du site
            Moduletitlewebcontrol2.CustomerWebSession = _webSession;
            InformationWebControl1.Language = _webSession.SiteLanguage;
            #endregion

            _webSession.LastReachedResultUrl = Page.Request.Url.AbsolutePath;
            _webSession.ReachedModule = true;

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

    #region Déchargement de la page
    /// <summary>
    /// Evènement de déchargement de la page
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Arguments</param>
    protected void Page_UnLoad(object sender, System.EventArgs e)
    {
    }
    #endregion

    #region Initialisation
    /// <summary>
    /// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
    /// </summary>
    /// <param name="e"></param>
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
    /// Détermine la valeur de PostBack
    /// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
    /// </summary>
    /// <returns>DeterminePostBackMode</returns>
    protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
    {
        System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
        DateTime begin;

        ResultsOptionsWebControl1.CustomerWebSession = _webSession;
        _resultWebControl.CustomerWebSession = _webSession;
        MenuWebControl2.CustomerWebSession = _webSession;
        ResultsOptionsWebControl1.PdmOption = true;
        ResultsOptionsWebControl1.PdvOption = true;

        begin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
        begin = begin.AddMonths(-12);
        if (begin.Year >= DateTime.Now.Year - 2)
        {
            ResultsOptionsWebControl1.EvolutionOption = true;
            ResultsOptionsWebControl1.ComparativeStudyOption = true;
            ResultsOptionsWebControl1.DependentSelection = true;
        }
        else 
        {
            ResultsOptionsWebControl1.EvolutionOption = false;
            ResultsOptionsWebControl1.ComparativeStudyOption = false;
            ResultsOptionsWebControl1.DependentSelection = false;
            _webSession.ComparativeStudy = false;
            _webSession.Evolution = false;
        }
        ResultsOptionsWebControl1.MutualExclusion = true;

        ForbidMediaRefinePage();

        return (tmp);
    }
    #endregion

    #endregion

    #region Abstract Methods
    /// <summary>
    /// Get next Url from contextual menu
    /// </summary>
    /// <returns></returns>
    protected override string GetNextUrlFromMenu()
    {
        return this.MenuWebControl2.NextUrl;
    }
    #endregion

    private void ForbidMediaRefinePage()
    {
        string listStr = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
        if (!string.IsNullOrEmpty(listStr))
        {
            List<string> arrStr = new List<string>(listStr.Split(','));
            List<Int64> idMedias = arrStr.ConvertAll(Convert.ToInt64);
            bool hasMediaLevel = false;
            List<DetailLevelItemInformation> levelInfos = VehiclesInformation.GetSelectionDetailLevelList(idMedias);
            foreach (DetailLevelItemInformation currentDetailLevelItem in levelInfos)
            {
                if (currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.media)
                {
                    hasMediaLevel = true; break;
                }
            }
            if (!hasMediaLevel)
            {
                 MenuWebControl2.ForbidOptionPagesList = new ArrayList();
                MenuWebControl2.ForbidOptionPagesList.Add(6);
            }

        }
    }

}
