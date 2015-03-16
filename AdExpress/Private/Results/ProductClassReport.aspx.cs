#region Informations
/*
 * Author : G. Ragneau
 * Created on : 12/01/2009
 * Modified on:
 *      date - author - descriptino
 * 
 * 
 * 
 * 
 * 
 * 
 * */
#endregion

#region Namespace
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using WebFunction = TNS.AdExpress.Web.Functions.Script;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress;
using ProductClassReports = TNS.AdExpressI.ProductClassReports;
using TNS.AdExpress.Domain.Web;

#endregion

namespace AdExpress.Private.Results
{
    /// <summary>
    /// Tableaux dynamiques de l'analyse sectorielle
    /// </summary>
    public partial class ProductClassReport : TNS.AdExpress.Web.UI.ResultWebPage
    {

        #region Variables
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string result = "";
        /// <summary>
        /// Script de fermeture du flash d'attente
        /// </summary>
        public string divClose = LoadingSystem.GetHtmlCloseDiv();
        /// <summary>
        /// JKavaScripts à insérer
        /// </summary>
        public string scripts = "";
        /// <summary>
        /// JKavaScripts bodyOnclick
        /// </summary>
        public string scriptBody = "";
        #endregion

        #region Variable MMI
        /// <summary>
        /// Contextual Menu
        /// </summary>
        //protected TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl InitializeProductWebControl1;
        //		/// <summary>
        //		/// Annule la personnnalisation des éléments de référence ou concurrents
        //		/// </summary>
        //		protected TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl InitializeProductWebControl1;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur : chargement de la session
        /// </summary>
        public ProductClassReport()
            : base()
        {
            _webSession.CurrentModule = WebConstantes.Module.Name.TABLEAU_DYNAMIQUE;
            _webSession.CurrentTab = 0;
        }
        #endregion

        #region Evènements

        #region Chargement de la page
        /// <summary>
        /// Evènement de chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
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

                DetailPeriodWebControl1.WebSession = _webSession;

                VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
                if (vehicleInfo != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null
                    && !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category)
                    && !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media)
                    && !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.region)
                    )
                {
                    _webSession.PreformatedMediaDetail = CstPreformatedDetail.PreformatedMediaDetails.vehicle;
                    _webSession.Save();
                }

                if (vehicleInfo != null 
                    && ( vehicleInfo.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.plurimedia || 
                    vehicleInfo.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.PlurimediaWithoutMms)
                    && WebApplicationParameters.HidePlurimediaEvol) {
                    ResultsOptionsWebControl1.EvolutionOption = false;
                    _webSession.Evolution = false;
                }
                else {
                    ResultsOptionsWebControl1.EvolutionOption = true;
                }

                #region Url Suivante
                //				_nextUrl=this.recallWebControl.NextUrl;
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
                ModuleBridgeWebControl1.CustomerWebSession = _webSession;
                object[] param = null;
                TNS.AdExpress.Domain.Web.Navigation.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Product Class reports"));
                param = new object[1];
                param[0] = _webSession;
                ProductClassReports.IProductClassReports productClassLayer = (ProductClassReports.IProductClassReports)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                if (!productClassLayer.ShowModuleBridge()) ModuleBridgeWebControl1.Visible = false;
                productClassLayer = null;
                InformationWebControl1.Language = _webSession.SiteLanguage;
                //				ExportWebControl1.CustomerWebSession=_webSession;
                ValidateSelectionButton.ToolTip = GestionWeb.GetWebWord(1183, _webSession.SiteLanguage);
                #endregion

                #region Définition de la page d'aide
                //				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"ASDynamicTablesHelp.aspx";
                #endregion

                #region Calcul du résultat
                scripts = WebFunction.ImageDropDownListScripts(ResultsOptionsWebControl1.ShowPictures);
                scriptBody = "javascript:openMenuTest();";
                #endregion

                #region Script
                // Script
                if (!Page.ClientScript.IsClientScriptBlockRegistered("ImageDropDownListScripts"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ImageDropDownListScripts", scripts);
                }
                #endregion

                #region MAJ de la Session
                _webSession.LastReachedResultUrl = Page.Request.Url.AbsolutePath;
                _webSession.ReachedModule = true;
                _webSession.Save();
                #endregion
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
            base.OnInit(e);
        }
        #endregion

        #region DeterminePostBack
        /// <summary>
        /// Détermine la valeur de PostBack
        /// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();

            try
            {
                ResultsOptionsWebControl1.CustomerWebSession = _webSession;
                MenuWebControl2.CustomerWebSession = _webSession;
                ResultWebControl1.CustomerWebSession = _webSession;

                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_webSession.CurrentModule);
                ResultPageInformation resultPageInformation = module.GetResultPageInformation(_webSession.CurrentTab);
                ResultsOptionsWebControl1.UnitOption = resultPageInformation.CanDisplayUnitOption;
                //ResultsOptionsWebControl1.CampaignTypeOption = resultPageInformation.CanDisplayCampaignType;

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

        #region PreInit
        protected override void OnPreInit(EventArgs e)
        {
            try{
            base.OnPreInit(e);         
            CstPreformatedDetail.PreformatedTables t = _webSession.PreformatedTable;
            if (Page.IsPostBack)
            {
                t = (CstPreformatedDetail.PreformatedTables)Int64.Parse(this.Request.Form.GetValues("DDL" + ResultsOptionsWebControl1.ID)[0]);
            }
            switch (t)
            {
                case CstPreformatedDetail.PreformatedTables.media_X_Year:
                case CstPreformatedDetail.PreformatedTables.product_X_Year:
                    ResultWebControl1.SkinID = "productClassResultTableClassif1XYear";
                    break;
                case CstPreformatedDetail.PreformatedTables.productYear_X_Media:
                case CstPreformatedDetail.PreformatedTables.productYear_X_Cumul:
                case CstPreformatedDetail.PreformatedTables.productYear_X_Mensual:
                case CstPreformatedDetail.PreformatedTables.mediaYear_X_Cumul:
                case CstPreformatedDetail.PreformatedTables.mediaYear_X_Mensual:
                    ResultWebControl1.SkinID = "productClassResultTableProductXMedia";
                    break;
                default:
                    ResultWebControl1.SkinID = "productClassResultTable";
                    break;
            }
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

        protected void ValidateSelectionButton_Click(object sender, System.EventArgs e)
        {
        }
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

        #region SetAdvertisementTypeOptions
        ///// <summary>
        ///// Indicate if customer can refine ad type
        ///// </summary>		
        //private void SetAdvertisementTypeOptions()
        //{
        //    TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_webSession.CurrentModule);
        //    ArrayList detailSelections = ((ResultPageInformation)module.GetResultPageInformation((int)_webSession.CurrentTab)).DetailSelectionItemsType;
        //    if (detailSelections.Contains(WebConstantes.DetailSelection.Type.advertisementType.GetHashCode()))
        //    {
        //        InitializeProductWebControl1.Visible = true;
        //        InitializeProductWebControl1.InitializeAdvertisementType = true;
        //    }
        //}
        #endregion
    }
}
