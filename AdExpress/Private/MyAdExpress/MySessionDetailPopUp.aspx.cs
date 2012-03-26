#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.CampaignTypes;
using TNS.AdExpress.Domain.DataBaseDescription;
using System.Globalization;
using TNS.AdExpress.Web.Controls.Selections;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;
using TNS.Classification.Universe;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomerSession = TNS.AdExpress.Constantes.Web.CustomerSessions;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Web.UI;
using TNS.AdExpressI.Date;
using TNS.Alert.Domain;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.LS;
using TNS.FrameWork.Date;

using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Level;
#endregion
using System.Reflection;
using TNS.AdExpress.Constantes.Classification.DB;
using System.Collections.Generic;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Web.Core.Utilities;
using Module = TNS.AdExpress.Constantes.Web.Module;
using Schema = TNS.AdExpress.Constantes.DB.Schema;

namespace AdExpress.Private.MyAdExpress
{
    /// <summary>
    /// Détail d'une Session
    /// </summary>
    public partial class MySessionDetailPopUp : TNS.AdExpress.Web.UI.PrivateWebPage
    {

        #region Variables
        /// <summary>
        /// Affiche la période dans page aspx
        /// </summary>
        public bool displayPeriod = false;
        /// <summary>
        /// _webSession sauvegardée dans mon AdExpress
        /// </summary>
        protected WebSession webSessionSave;
        /// <summary>
        /// Texte
        /// </summary>
        protected string mediaText;
        /// <summary>
        /// Texte
        /// </summary>
        protected string advertiserText;
        /// <summary>
        /// Liste des médias
        /// </summary>
        protected string listOfVehicleText;
        /// <summary>
        /// Script
        /// </summary>
        protected string script;
        /// <summary>
        /// Affichage des annonceurs
        /// </summary> 
        public bool displayAdvertiser = false;
        /// <summary>
        /// Affichage du détail média
        /// </summary>
        public bool displayDetailMedia = false;
        /// <summary>
        /// détail média
        /// </summary>
        public string mediaDetailText = "";
        /// <summary>
        /// Annonceur de référence
        /// </summary>
        public string referenceAdvertiserText = "";
        /// <summary>
        /// Affiche les éléments de références
        /// </summary>
        public bool referenceAdvertiserDisplay = false;
        /// <summary>
        /// Affiche les éléments concurrents
        /// </summary>
        public bool competitorAdvertiserDisplay = false;
        /// <summary>
        /// Affiche les informations sur les agences médias
        /// </summary>
        public bool displayMediaAgency = false;
        /// <summary>
        /// Annonceurs concurrents
        /// </summary>
        public string competitorAdvertiserText = "";
        /// <summary>
        /// Affiche les produits
        /// </summary>
        public bool displayProduct = false;
        public bool displayDetailProduct = false;
        /// <summary>
        /// Affiche les produits
        /// </summary>
        public string productText = "";
        /// <summary>
        /// Etude comparative
        /// </summary>
        public bool comparativeStudy = false;
        /// <summary>
        /// Texte etude comparative
        /// </summary>
        public string comparativeStudyText = "";
        /// <summary>
        /// Affiche les éléments dans reference media
        /// </summary>
        public bool displayReferenceDetailMedia = false;
        /// <summary>
        /// Affiche le code pour les éléments contenue dans reference media
        /// </summary>
        public string referenceMediaDetailText;
        /// <summary>
        /// Affiche la période de l'étude dans la page aspx
        /// </summary>
        public bool displayStudyPeriod = false;
        /// <summary>
        /// Affiche la période comparative dans la page aspx
        /// </summary>
        public bool displayComparativePeriod = false;
        /// <summary>
        /// Affiche le type de la période comparative dans la page aspx
        /// </summary>
        public bool displayComparativePeriodType = false;
        /// <summary>
        /// Affiche le type de la disponibilité des données dans la page aspx
        /// </summary>
        public bool displayPeriodDisponibilityType = false;
        /// <summary>
        /// Affiche les media dans page aspx
        /// </summary>
        public bool displayMedia = false;
        /// <summary>
        /// Display Advertising Agency
        /// </summary>
        public bool displayAdvertisingAgency;
        /// <summary>
        /// Adevrtising Agency Text
        /// </summary>
        public string advertisingAgencyText = string.Empty;

        /// <summary>
        /// Affiche l'unité
        /// </summary>
        public bool displayUnit = false;
        /// <summary>
        /// détail produit
        /// </summary>
        public string productDetailText = "";

        public bool displayGenericMediaDetailLevel = false;

        public bool displayPersonnalizedLevel = false;

        /// <summary>
        /// Affiche liste des médias
        /// </summary>
        public bool displayListOfVehicles = false;

        public bool displayMediaSelection = false;

        public string mediaSelectiondText = "";

        public bool displayCampaignTypeSelection = false;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public MySessionDetailPopUp()
            : base()
        {
        }
        #endregion

        #region Chargement de la page
        /// <summary>
        /// Chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                #region Variables
                Int64 idMySession;
                string periodText;
                int idAdvertiser = 1;
                int idMedia = 1;
                string dateBegin;
                string dateEnd;
                DateTime FirstDayNotEnable = DateTime.Now;
                DateTime lastDayEnable = DateTime.Now;
                DateTime tmpEndDate;
                DateTime tmpBeginDate;
                CstWeb.globalCalendar.comparativePeriodType comparativePeriodType;
                CstWeb.globalCalendar.periodDisponibilityType periodDisponibilityType;
                bool verifCustomerPeriod = false;
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
                TNS.AdExpress.Domain.Layers.CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                #endregion


                try
                {
                    if (TNS.Alert.Domain.AlertConfiguration.IsActivated && Page.Request.QueryString.Get("idAlertSession") != null)
                    {
                        DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
                        TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                        IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null, null);
                        int idAlert = Int32.Parse(Page.Request.QueryString.Get("idAlertSession"));
                        Alert alert = alertDAL.GetAlert(idAlert);
                        webSessionSave = (WebSession)alert.Session;
                        mySessionLabel.Text = alert.Title;
                    }
                    else
                    {
                        idMySession = Int64.Parse(Page.Request.QueryString.Get("idMySession"));
                        webSessionSave = (WebSession)MySessionDataAccess.GetResultMySession(idMySession.ToString(), _webSession);
                        mySessionLabel.Text = TNS.AdExpress.Web.DataAccess.MyAdExpress.MySessionsDataAccess.GetSession(idMySession, _webSession);
                    }
                }
                catch (Exception)
                {

                }
                #region Langage
                //Modification de la langue pour les Textes AdExpress
                //for (int j = 0; j < this.Controls.Count; j++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[j].Controls, _webSession.SiteLanguage);
                //}
                #endregion

                // Affichage du module
                moduleLabel.Text = GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSessionSave.CurrentModule), _webSession.SiteLanguage);

                #region SelectionUniversAdvertiser
                ////Affichage type d'advertiser
                //// Holding Company
                //if(webSessionSave.SelectionUniversAdvertiser.FirstNode!=null){

                //    displayAdvertiser=true;
                //    if(((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess 
                //        ||	((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
                //        advertiserAdexpresstext.Code=814;}
                //        // Advertiser
                //    else if(((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess
                //        ||	((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
                //        advertiserAdexpresstext.Code=813;}
                //        // Marque
                //    else if(((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess
                //        ||	((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) 
                //    {
                //        advertiserAdexpresstext.Code=1585;}
                //        // Product
                //    else if(((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
                //        ||	((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
                //        advertiserAdexpresstext.Code=815;}
                //        // Sector
                //    else if(((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
                //        ||	((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
                //        advertiserAdexpresstext.Code=965;}
                //        // SubSector
                //    else if(((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
                //        ||	((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
                //        advertiserAdexpresstext.Code=966;}
                //        // Group
                //    else if(((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
                //        ||	((LevelInformation)webSessionSave.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
                //        advertiserAdexpresstext.Code=967;}

                //}else{
                //    displayAdvertiser=false;
                //}
                #endregion

                #region Période sélectionnée (GlobalDateSelection)
                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES)
                {

                    int oldYear = 2000;
                    long selectedVehicle = ((LevelInformation)webSessionSave.SelectionUniversMedia.FirstNode.Tag).ID;
                    if (webSessionSave.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA
                        && webSessionSave.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES
                        && webSessionSave.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES)
                        //FirstDayNotEnable = WebFunctions.Dates.GetFirstDayNotEnabled(webSessionSave, selectedVehicle, oldYear,_webSession.Source);
                        FirstDayNotEnable = dateDAL.GetFirstDayNotEnabled(new List<Int64>(new Int64[] { selectedVehicle }), oldYear);
                    _webSession.CurrentModule = webSessionSave.CurrentModule;

                    switch (webSessionSave.DetailPeriod)
                    {
                        case CstCustomerSession.Period.DisplayLevel.monthly:
                            if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.currentYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastMonth &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousMonth)
                            {
                                if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.dateToDate)
                                {
                                    string startYearMonth = webSessionSave.PeriodBeginningDate;
                                    string endYearMonth = webSessionSave.PeriodEndDate;
                                    DateTime firstDayOfMonth = new DateTime(int.Parse(endYearMonth.ToString().Substring(0, 4)), int.Parse(endYearMonth.ToString().Substring(4, 2)), 1);
                                    Int32 lastDayOfMonth = ((firstDayOfMonth.AddMonths(1)).AddDays(-1)).Day;
                                    _webSession.PeriodBeginningDate = startYearMonth + "01";
                                    _webSession.PeriodEndDate = endYearMonth + lastDayOfMonth;
                                }
                                else
                                {
                                    _webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                    _webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                }
                                _webSession.PeriodType = CstCustomerSession.Period.Type.dateToDate;
                            }
                            break;
                        case CstCustomerSession.Period.DisplayLevel.weekly:
                            if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastWeek &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousWeek)
                            {
                                if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.dateToDate)
                                {
                                    AtomicPeriodWeek startWeek = new AtomicPeriodWeek(int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(0, 4)), int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(4, 2)));
                                    AtomicPeriodWeek endWeek = new AtomicPeriodWeek(int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(0, 4)), int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(4, 2)));
                                    DateTime dateBeginTmp = startWeek.FirstDay;
                                    DateTime dateEndTmp = endWeek.FirstDay.AddDays(6);
                                    _webSession.PeriodBeginningDate = dateBeginTmp.Year.ToString() + dateBeginTmp.Month.ToString("00") + dateBeginTmp.Day.ToString("00");
                                    _webSession.PeriodEndDate = dateEndTmp.Year.ToString() + dateEndTmp.Month.ToString("00") + dateEndTmp.Day.ToString("00");
                                }
                                else
                                {
                                    _webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                    _webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                }
                                _webSession.PeriodType = CstCustomerSession.Period.Type.dateToDate;
                            }
                            break;
                        default:
                            _webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                            _webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                            _webSession.PeriodType = CstCustomerSession.Period.Type.dateToDate;
                            break;
                    }

                    switch (webSessionSave.DetailPeriod)
                    {
                        case CstCustomerSession.Period.DisplayLevel.monthly:
                        case CstCustomerSession.Period.DisplayLevel.weekly:
                        case CstCustomerSession.Period.DisplayLevel.dayly:
                            if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.currentYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastMonth &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousMonth &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastWeek &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousWeek &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastDays &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousDay)
                            {

                                tmpEndDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6, 2)));
                                tmpBeginDate = new DateTime(Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(6, 2)));

                                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                                {

                                    if (webSessionSave.DetailPeriod == CstCustomerSession.Period.DisplayLevel.monthly)
                                    {
                                        comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.dateToDate;
                                        periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                    }
                                    else if (webSessionSave.DetailPeriod == CstCustomerSession.Period.DisplayLevel.weekly)
                                    {
                                        comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.comparativeWeekDate;
                                        periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                    }
                                    else
                                    {
                                        comparativePeriodType = webSessionSave.CustomerPeriodSelected.ComparativePeriodType;
                                        periodDisponibilityType = webSessionSave.CustomerPeriodSelected.PeriodDisponibilityType;
                                    }

                                    switch (periodDisponibilityType)
                                    {

                                        case CstWeb.globalCalendar.periodDisponibilityType.currentDay:
                                            lastDayEnable = DateTime.Now;
                                            break;
                                        case CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                                            lastDayEnable = FirstDayNotEnable.AddDays(-1);
                                            break;

                                    }

                                    if (CompareDateEnd(lastDayEnable, tmpEndDate) || CompareDateEnd(tmpBeginDate, DateTime.Now))
                                        _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, true, comparativePeriodType, periodDisponibilityType);
                                    else
                                    {

                                        switch (webSessionSave.CustomerPeriodSelected.PeriodDisponibilityType)
                                        {

                                            case CstWeb.globalCalendar.periodDisponibilityType.currentDay:
                                                _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
                                                break;
                                            case CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                                                _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (CompareDateEnd(DateTime.Now, tmpEndDate) || CompareDateEnd(tmpBeginDate, DateTime.Now))
                                        _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                                    else
                                        _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
                                }
                                verifCustomerPeriod = true;
                            }
                            break;
                    }

                    if (!verifCustomerPeriod)
                        UpdateGlobalDates(webSessionSave.PeriodType, webSessionSave, FirstDayNotEnable);
                }
                #endregion

                #region période
                if (displayPeriod = (webSessionSave.isDatesSelected() && webSessionSave.CurrentModule != Module.Name.TENDACES))
                {
                    int languageTemp = webSessionSave.SiteLanguage;
                    webSessionSave.SiteLanguage = _webSession.SiteLanguage;
                    infoDateLabel1.Text = HtmlFunctions.GetPeriodDetail(webSessionSave);
                    webSessionSave.SiteLanguage = languageTemp;
                }
                #endregion

                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                {
                    // Période de l'étude
                    if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    {
                        displayStudyPeriod = true;
                        StudyPeriod.Text = HtmlFunctions.GetStudyPeriodDetail(webSessionSave, webSessionSave.CurrentModule);
                    }

                    // Période comparative
                    if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    {
                        displayComparativePeriod = true;
                        comparativePeriod.Text = HtmlFunctions.GetComparativePeriodDetail(webSessionSave, webSessionSave.CurrentModule);
                    }

                    // Type Sélection comparative
                    if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    {
                        displayComparativePeriodType = true;
                        ComparativePeriodType.Text = HtmlFunctions.GetComparativePeriodTypeDetail(webSessionSave, webSessionSave.CurrentModule);
                    }
                }
                else if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE)
                {

                    #region Période de l'étude
                    // Période de l'étude
                    if (_webSession.isStudyPeriodSelected())
                    {
                        displayStudyPeriod = true;
                        StudyPeriod.Text = HtmlFunctions.GetStudyPeriodDetail(webSessionSave, webSessionSave.CurrentModule);
                    }
                    #endregion

                    #region Période comparative
                    // Période comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                    {
                        if (_webSession.isPeriodComparative())
                        {
                            displayComparativePeriod = true;
                            comparativePeriod.Text = HtmlFunctions.GetComparativePeriodDetail(webSessionSave, webSessionSave.CurrentModule);
                        }
                    }
                    #endregion

                    #region Type Sélection comparative
                    // Type Sélection comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                    {
                        if (_webSession.isComparativePeriodTypeSelected())
                        {
                            displayComparativePeriodType = true;
                            ComparativePeriodType.Text = HtmlFunctions.GetComparativePeriodTypeDetail(webSessionSave, webSessionSave.CurrentModule);
                        }
                    }
                    #endregion

                    #region Type disponibilité des données
                    // Type disponibilité des données
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                    {
                        if (_webSession.isPeriodDisponibilityTypeSelected())
                        {
                            displayPeriodDisponibilityType = true;
                            PeriodDisponibilityType.Text = HtmlFunctions.GetPeriodDisponibilityTypeDetail(webSessionSave);
                        }
                    }
                    #endregion

                }

                #region Etude Comparative
                // Etude comparative
                if (webSessionSave.ComparativeStudy)
                {
                    comparativeStudy = true;
                    comparativeStudyText = GestionWeb.GetWebWord(1118, _webSession.SiteLanguage);
                }
                #endregion

                #region Generic MediaDetail Level
                try
                {
                    if (webSessionSave.GenericMediaDetailLevel != null && webSessionSave.GenericMediaDetailLevel.Levels.Count > 0)
                    {
                        displayGenericMediaDetailLevel = true;
                        genericMediaDetailLevelLabel1.Text = webSessionSave.GenericMediaDetailLevel.GetLabel(_webSession.SiteLanguage);


                    }
                }
                catch (Exception)
                {
                    ArrayList levels = new ArrayList();
                    levels.Add(1);
                    webSessionSave.GenericMediaDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                  
                    WebFunctions.MediaDetailLevel.GetGenericLevelDetail(webSessionSave, ref displayGenericMediaDetailLevel, genericMediaDetailLevelLabel1, false);

                }


                #endregion

                #region Personnalized level
                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.VP && webSessionSave.GenericMediaDetailLevel != null && webSessionSave.GenericMediaDetailLevel.Levels.Count > 0)
                {
                    displayPersonnalizedLevel = true;
                    DetailLevelItemInformation persoLevel = DetailLevelItemsInformation.Get(webSessionSave.PersonnalizedLevel);
                    personnalizedLevelLevelLabel1.Text = GestionWeb.GetWebWord(persoLevel.WebTextId, _webSession.SiteLanguage);

                }
                #endregion


                displayUnit = (webSessionSave.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.VP);
                unitLabel.Text = GestionWeb.GetWebWord(webSessionSave.GetSelectedUnit().WebTextId, _webSession.SiteLanguage);

                if (displayMedia = webSessionSave.isMediaSelected() && (_webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.VP))
                {
                    mediaText = TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSessionSave.SelectionUniversMedia, false, false, false, 600, false, false, _webSession.SiteLanguage, 2, 1, true, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource);
                }

                #region Agences médias
                // Agence média
                if (_webSession.PreformatedProductDetail == CstCustomerSession.PreformatedDetails.PreformatedProductDetails.agencyProduct
                    || _webSession.PreformatedProductDetail == CstCustomerSession.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser
                    || _webSession.PreformatedProductDetail == CstCustomerSession.PreformatedDetails.PreformatedProductDetails.group_agencyAgency
                    )
                {
                    displayMediaAgency = true;
                    MediaAgency.Text = _webSession.MediaAgencyFileYear.Substring(_webSession.MediaAgencyFileYear.Length - 4, 4); ;

                }
                #endregion

                int i = 2;

                #region Univers produit principal sélectionné

                //Univers produit principal sélectionné
                string nameProduct = "";
                StringBuilder t = null;
                if (webSessionSave.PrincipalProductUniverses != null && webSessionSave.PrincipalProductUniverses.Count > 0)
                {

                    t = new StringBuilder(1000);
                    nameProduct = "";

                    if (webSessionSave.PrincipalProductUniverses.Count > 1)
                    {
                        competitorAdvertiserDisplay = true;
                    }
                    else
                    {
                        displayProduct = true;
                        productAdExpressText.Code = 1759;
                    }

                    SelectItemsInClassificationWebControl selectItemsInClassificationWebControl =
                        new SelectItemsInClassificationWebControl();
                    selectItemsInClassificationWebControl.TreeViewIcons = "/App_Themes/" +
                                                                          WebApplicationParameters.Themes[
                                                                              _webSession.SiteLanguage].Name +
                                                                          "/Styles/TreeView/Icons";
                    selectItemsInClassificationWebControl.TreeViewScripts = "/App_Themes/" +
                                                                            WebApplicationParameters.Themes[
                                                                                _webSession.SiteLanguage].Name +
                                                                            "/Styles/TreeView/Scripts";
                    selectItemsInClassificationWebControl.TreeViewStyles = "/App_Themes/" +
                                                                           WebApplicationParameters.Themes[
                                                                               _webSession.SiteLanguage].Name +
                                                                           "/Styles/TreeView/Css";
                    selectItemsInClassificationWebControl.ChildNodeExcludeCss = "txtChildNodeExcludeCss";
                    selectItemsInClassificationWebControl.ChildNodeIncludeCss = "txtChildNodeIncludeCss";
                    selectItemsInClassificationWebControl.ParentNodeChildExcludeCss = "txtParentNodeChildExcludeCss";
                    selectItemsInClassificationWebControl.ParentNodeChildIncludeCss = "txtParentNodeChildIncludeCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameBodyCss = "treeExcludeFrameBodyCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameCss = "treeExcludeFrameCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameHeaderCss = "treeExcludeFrameHeaderCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameBodyCss = "treeIncludeFrameBodyCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameCss = "treeIncludeFrameCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameHeaderCss = "treeIncludeFrameHeaderCss";
                    selectItemsInClassificationWebControl.SiteLanguage = _webSession.SiteLanguage;
                    selectItemsInClassificationWebControl.DBSchema =
                        WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;
                    for (int k = 0; k < webSessionSave.PrincipalProductUniverses.Count; k++)
                    {
                        if (webSessionSave.PrincipalProductUniverses.Count > 1)
                        {
                            if (webSessionSave.PrincipalProductUniverses.ContainsKey(k))
                            {
                                if (k > 0)
                                {
                                    nameProduct = GestionWeb.GetWebWord(2301, _webSession.SiteLanguage);
                                }
                                else
                                {
                                    nameProduct = GestionWeb.GetWebWord(2302, _webSession.SiteLanguage);
                                }

                                t.Append("<TR>");
                                t.Append("<TD class=\"txtViolet11Bold backGroundWhite\">&nbsp;");
                                t.Append("<label>" + nameProduct + "</label></TD>");
                                t.Append("</TR>");

                                //Universe Label
                                if (!string.IsNullOrEmpty(webSessionSave.PrincipalProductUniverses[k].Label))
                                {
                                    t.Append("<TR>");
                                    t.Append("<TD class=\"txtViolet11Bold backGroundWhite\">&nbsp;");
                                    t.Append("<Label>" + webSessionSave.PrincipalProductUniverses[k].Label + "</Label>");
                                    t.Append("</TD></TR>");
                                }

                                //Render universe html code
                                t.Append("<TR height=\"20\">");
                                t.Append("<TD align=\"center\" vAlign=\"top\" class=\"backGroundWhite\">" +
                                         selectItemsInClassificationWebControl.ShowUniverse(
                                             webSessionSave.PrincipalProductUniverses[k], _webSession.DataLanguage,
                                             DBFunctions.GetDataSource(_webSession)) + "</TD>");
                                t.Append("</TR>");
                                t.Append("<TR height=\"5\">");
                                t.Append("<TD class=\"backGroundWhite\"></TD>");
                                t.Append("</TR>");
                                t.Append("<TR height=\"10\"><TD></TD></TR>");

                            }
                        }
                        else
                        {
                            if (webSessionSave.PrincipalProductUniverses.ContainsKey(k))
                            {
                                productText +=
                                    selectItemsInClassificationWebControl.ShowUniverse(
                                        webSessionSave.PrincipalProductUniverses[k], _webSession.DataLanguage,
                                        DBFunctions.GetDataSource(_webSession));
                            }
                        }
                    }
                    if (webSessionSave.PrincipalProductUniverses.Count > 1)
                        competitorAdvertiserText = t.ToString();
                }

                #endregion


                #region Univers produit secondaire

                //Univers produit secondaire
                if (webSessionSave.SecondaryProductUniverses != null && webSessionSave.SecondaryProductUniverses.Count > 0)
                {
                    t = new StringBuilder(1000);
                    nameProduct = "";

                    if (Modules.IsDashBoardModule(webSessionSave))
                    {
                        displayProduct = true;
                        productAdExpressText.Code = 1759;
                    }
                    else
                    {
                        referenceAdvertiserAdexpresstext.Code = 2302;
                    }
                    SelectItemsInClassificationWebControl selectItemsInClassificationWebControl =
                        new SelectItemsInClassificationWebControl();
                    selectItemsInClassificationWebControl.TreeViewIcons = "/App_Themes/" +
                                                                          WebApplicationParameters.Themes[
                                                                              _webSession.SiteLanguage].Name +
                                                                          "/Styles/TreeView/Icons";
                    selectItemsInClassificationWebControl.TreeViewScripts = "/App_Themes/" +
                                                                            WebApplicationParameters.Themes[
                                                                                _webSession.SiteLanguage].Name +
                                                                            "/Styles/TreeView/Scripts";
                    selectItemsInClassificationWebControl.TreeViewStyles = "/App_Themes/" +
                                                                           WebApplicationParameters.Themes[
                                                                               _webSession.SiteLanguage].Name +
                                                                           "/Styles/TreeView/Css";
                    selectItemsInClassificationWebControl.ChildNodeExcludeCss = "txtChildNodeExcludeCss";
                    selectItemsInClassificationWebControl.ChildNodeIncludeCss = "txtChildNodeIncludeCss";
                    selectItemsInClassificationWebControl.ParentNodeChildExcludeCss = "txtParentNodeChildExcludeCss";
                    selectItemsInClassificationWebControl.ParentNodeChildIncludeCss = "txtParentNodeChildIncludeCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameBodyCss = "treeExcludeFrameBodyCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameCss = "treeExcludeFrameCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameHeaderCss = "treeExcludeFrameHeaderCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameBodyCss = "treeIncludeFrameBodyCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameCss = "treeIncludeFrameCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameHeaderCss = "treeIncludeFrameHeaderCss";
                    selectItemsInClassificationWebControl.SiteLanguage = _webSession.SiteLanguage;
                    selectItemsInClassificationWebControl.DBSchema = Schema.ADEXPRESS_SCHEMA;

                    if (webSessionSave.CurrentModule == Module.Name.INDICATEUR
                        || webSessionSave.CurrentModule == Module.Name.TABLEAU_DYNAMIQUE
                        )
                    {
                        //Listes des annonceurs de références
                        if (webSessionSave.SecondaryProductUniverses.ContainsKey(0))
                        {
                            //Universe Label							
                            referenceAdvertiserDisplay = true;
                            referenceAdvertiserAdexpresstext.Code = 1195;
                            referenceAdvertiserText +=
                                selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.SecondaryProductUniverses[0],
                                                                                   _webSession.DataLanguage,
                                                                                   DBFunctions.GetDataSource(_webSession));
                        }

                        //Listes des annonceurs concurrents
                        if (webSessionSave.SecondaryProductUniverses.ContainsKey(1))
                        {

                            nameProduct = GestionWeb.GetWebWord(1196, _webSession.SiteLanguage);
                            competitorAdvertiserDisplay = true;
                            competitorAdvertiserText = RenderProductSelection(webSessionSave, _webSession, nameProduct, 1,
                                                                              selectItemsInClassificationWebControl);
                        }
                    }
                    else
                    {
                        for (int k = 0; k < webSessionSave.SecondaryProductUniverses.Count; k++)
                        {
                            if (webSessionSave.SecondaryProductUniverses.ContainsKey(k))
                            {
                                if (k > 0)
                                {
                                    if (k == 1)
                                    {
                                        competitorAdvertiserDisplay = true;
                                    }

                                    t.Append(RenderProductSelection(webSessionSave, _webSession, nameProduct, k,
                                                                    selectItemsInClassificationWebControl));

                                }
                                else
                                {
                                    if (Modules.IsDashBoardModule(webSessionSave))
                                        productText =
                                            selectItemsInClassificationWebControl.ShowUniverse(
                                                webSessionSave.SecondaryProductUniverses[k], _webSession.DataLanguage,
                                                DBFunctions.GetDataSource(_webSession));
                                    else
                                    {
                                        //Universe Label
                                        if (webSessionSave.SecondaryProductUniverses[k].Label != null &&
                                            webSessionSave.SecondaryProductUniverses[k].Label.Length > 0)
                                        {
                                            referenceAdvertiserText += "<TR>";
                                            referenceAdvertiserText += "<TD class=\"txtViolet11Bold backGroundWhite\">&nbsp;";
                                            referenceAdvertiserText += "<Label>" +
                                                                       webSessionSave.SecondaryProductUniverses[k].Label +
                                                                       "</Label>";
                                            referenceAdvertiserText += "</TD></TR>";
                                        }
                                        referenceAdvertiserText +=
                                            selectItemsInClassificationWebControl.ShowUniverse(
                                                webSessionSave.SecondaryProductUniverses[k], _webSession.DataLanguage,
                                                DBFunctions.GetDataSource(_webSession));
                                    }
                                }
                            }
                        }
                        if (webSessionSave.SecondaryProductUniverses.Count > 1)
                            competitorAdvertiserText = t.ToString();
                    }
                }

                #endregion

                #region Univers support principal sélectionné
                if (webSessionSave.PrincipalMediaUniverses != null && webSessionSave.PrincipalMediaUniverses.Count > 0)
                {
                    displayMediaSelection = true;
                    mediaSelectiondWebText.Code = 2540; // Univers support

                    if (webSessionSave.PrincipalMediaUniverses[0].ContainsLevel(TNSClassificationLevels.REGION, AccessType.includes))
                        mediaSelectiondWebText.Code = 2680;

                    SelectItemsInClassificationWebControl selectItemsInClassificationWebControl = new TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl();
                    selectItemsInClassificationWebControl.TreeViewIcons = "/App_Themes/" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + "/Styles/TreeView/Icons";
                    selectItemsInClassificationWebControl.TreeViewScripts = "/App_Themes/" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + "/Styles/TreeView/Scripts";
                    selectItemsInClassificationWebControl.TreeViewStyles = "/App_Themes/" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + "/Styles/TreeView/Css";
                    selectItemsInClassificationWebControl.ChildNodeExcludeCss = "txtChildNodeExcludeCss";
                    selectItemsInClassificationWebControl.ChildNodeIncludeCss = "txtChildNodeIncludeCss";
                    selectItemsInClassificationWebControl.ParentNodeChildExcludeCss = "txtParentNodeChildExcludeCss";
                    selectItemsInClassificationWebControl.ParentNodeChildIncludeCss = "txtParentNodeChildIncludeCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameBodyCss = "treeExcludeFrameBodyCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameCss = "treeExcludeFrameCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameHeaderCss = "treeExcludeFrameHeaderCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameBodyCss = "treeIncludeFrameBodyCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameCss = "treeIncludeFrameCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameHeaderCss = "treeIncludeFrameHeaderCss";
                    selectItemsInClassificationWebControl.SiteLanguage = _webSession.SiteLanguage;
                    selectItemsInClassificationWebControl.DBSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;

                    mediaSelectiondText += selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.PrincipalMediaUniverses[0], _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource);
                }
                #endregion

                #region Advertising Agency Univers
                //Univers advertising agency principal sélectionné
                string nameAdvertisingAgnecy = "";
                System.Text.StringBuilder tmp = null;
                if (webSessionSave.PrincipalAdvertisingAgnecyUniverses != null && webSessionSave.PrincipalAdvertisingAgnecyUniverses.Count > 0)
                {

                    tmp = new System.Text.StringBuilder(1000);
                    nameAdvertisingAgnecy = "";

                    displayAdvertisingAgency = true;
                    advertisingAgencyAdExpressText.Code = 2817;

                    TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl = new TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl();
                    selectItemsInClassificationWebControl.TreeViewIcons = "/App_Themes/" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + "/Styles/TreeView/Icons";
                    selectItemsInClassificationWebControl.TreeViewScripts = "/App_Themes/" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + "/Styles/TreeView/Scripts";
                    selectItemsInClassificationWebControl.TreeViewStyles = "/App_Themes/" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + "/Styles/TreeView/Css";
                    selectItemsInClassificationWebControl.ChildNodeExcludeCss = "txtChildNodeExcludeCss";
                    selectItemsInClassificationWebControl.ChildNodeIncludeCss = "txtChildNodeIncludeCss";
                    selectItemsInClassificationWebControl.ParentNodeChildExcludeCss = "txtParentNodeChildExcludeCss";
                    selectItemsInClassificationWebControl.ParentNodeChildIncludeCss = "txtParentNodeChildIncludeCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameBodyCss = "treeExcludeFrameBodyCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameCss = "treeExcludeFrameCss";
                    selectItemsInClassificationWebControl.TreeExcludeFrameHeaderCss = "treeExcludeFrameHeaderCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameBodyCss = "treeIncludeFrameBodyCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameCss = "treeIncludeFrameCss";
                    selectItemsInClassificationWebControl.TreeIncludeFrameHeaderCss = "treeIncludeFrameHeaderCss";
                    selectItemsInClassificationWebControl.SiteLanguage = _webSession.SiteLanguage;
                    selectItemsInClassificationWebControl.DBSchema = WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label;
                    for (int k = 0; k < webSessionSave.PrincipalAdvertisingAgnecyUniverses.Count; k++)
                    {
                        if (webSessionSave.PrincipalAdvertisingAgnecyUniverses.ContainsKey(k))
                        {
                            advertisingAgencyText += selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.PrincipalAdvertisingAgnecyUniverses[k], _webSession.DataLanguage, _webSession.Source);
                        }
                    }
                }
                #endregion


                #region Medias concurrents
                if (webSessionSave.isCompetitorMediaSelected())
                {
                    displayListOfVehicles = true;
                    System.Text.StringBuilder mediaSB = new System.Text.StringBuilder(1000);

                    mediaSB.Append("<TR>");
                    mediaSB.Append("<TD class=\"txtViolet11Bold backGroundWhite\">&nbsp;");
                    mediaSB.Append("<label>" + GestionWeb.GetWebWord(1087, _webSession.SiteLanguage) + "</label></TD>");
                    mediaSB.Append("</TR>");

                    while ((System.Windows.Forms.TreeNode)webSessionSave.CompetitorUniversMedia[idMedia] != null)
                    {

                        System.Windows.Forms.TreeNode tree = (System.Windows.Forms.TreeNode)webSessionSave.CompetitorUniversMedia[idMedia];
                        mediaSB.Append("<TR height=\"20\">");
                        mediaSB.Append("<TD align=\"center\" vAlign=\"top\" class=\"backGroundWhite\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)webSessionSave.CompetitorUniversMedia[idMedia], false, true, true, 600, true, false, _webSession.SiteLanguage, 2, i, true, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource) + "</TD>");
                        mediaSB.Append("</TR>");
                        mediaSB.Append("<TR height=\"5\">");
                        mediaSB.Append("<TD class=\"backGroundWhite\"></TD>");
                        mediaSB.Append("</TR>");
                        mediaSB.Append("<TR height=\"10\"><TD></TD></TR>");
                        if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent" + i + ""))
                        {
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent" + i + "", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
                        }
                        i++;
                        idMedia++;
                    }
                    listOfVehicleText = mediaSB.ToString();
                }
                #endregion

                #region Sélection Médias
                // Partie détail média
                if (webSessionSave.SelectionUniversMedia.FirstNode != null && webSessionSave.SelectionUniversMedia.FirstNode.Nodes.Count > 0)
                {

                    displayDetailMedia = true;
                    System.Text.StringBuilder detailMedia = new System.Text.StringBuilder(1000);

                    detailMedia.Append("<TR>");
                    detailMedia.Append("<TD class=\"txtViolet11Bold backGroundWhite\" >&nbsp;");
                    int webTextId = (_webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.VP) ? 1194 : 2869;
                    detailMedia.Append("<label>" + GestionWeb.GetWebWord(webTextId, _webSession.SiteLanguage) + "</label></TD>");
                    detailMedia.Append("</TR>");

                    detailMedia.Append("<TR height=\"20\">");
                    detailMedia.Append("<TD align=\"center\" vAlign=\"top\" class=\"backGroundWhite\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)webSessionSave.SelectionUniversMedia.FirstNode, false, true, true, 600, true, false, _webSession.SiteLanguage, 2, i, true, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource) + "</TD>");
                    detailMedia.Append("</TR>");
                    detailMedia.Append("<TR height=\"5\">");
                    detailMedia.Append("<TD class=\"backGroundWhite\"></TD>");
                    detailMedia.Append("</TR>");
                    detailMedia.Append("<TR height=\"10\"><TD></TD></TR>");
                    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent" + i + ""))
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent" + i + "", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
                    }
                    i++;
                    mediaDetailText = detailMedia.ToString();
                }
                #endregion

                #region Sélection Produit
                // Partie détail média
                if (webSessionSave.SelectionUniversProduct.FirstNode != null && webSessionSave.SelectionUniversProduct.FirstNode.Nodes.Count > 0)
                {

                    displayDetailProduct = true;
                    System.Text.StringBuilder detailProduct = new System.Text.StringBuilder(1000);

                    detailProduct.Append("<TR>");
                    detailProduct.Append("<TD class=\"txtViolet11Bold backGroundWhite\" >&nbsp;");
                    detailProduct.Append("<label>" + GestionWeb.GetWebWord(1759, _webSession.SiteLanguage) + "</label></TD>");
                    detailProduct.Append("</TR>");

                    detailProduct.Append("<TR height=\"20\">");
                    detailProduct.Append("<TD align=\"center\" vAlign=\"top\" class=\"backGroundWhite\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)webSessionSave.SelectionUniversProduct.FirstNode, false, true, true, 600, true, false, _webSession.SiteLanguage, 2, i, true, _webSession.DataLanguage, _webSession.Source, false) + "</TD>");
                    detailProduct.Append("</TR>");
                    detailProduct.Append("<TR height=\"5\">");
                    detailProduct.Append("<TD class=\"backGroundWhite\"></TD>");
                    detailProduct.Append("</TR>");
                    detailProduct.Append("<TR height=\"10\"><TD></TD></TR>");
                    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent" + i + ""))
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent" + i + "", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
                    }
                    i++;
                    productDetailText = detailProduct.ToString();
                }
                #endregion

                #region Références Médias
                // Détail référence média			
                if (_webSession.isReferenceMediaSelected())
                {

                    displayReferenceDetailMedia = true;
                    System.Text.StringBuilder referenceDetailMedia = new System.Text.StringBuilder(1000);

                    referenceDetailMedia.Append("<TR>");
                    referenceDetailMedia.Append("<TD class=\"txtViolet11Bold backGroundWhite\" >&nbsp;");
                    referenceDetailMedia.Append("<label>" + GestionWeb.GetWebWord(1194, _webSession.SiteLanguage) + "</label></TD>");
                    referenceDetailMedia.Append("</TR>");
                    referenceDetailMedia.Append("<TR height=\"20\">");
                    referenceDetailMedia.Append("<TD align=\"center\" vAlign=\"top\" class=\"backGroundWhite\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)_webSession.ReferenceUniversMedia, false, true, true, 600, true, false, _webSession.SiteLanguage, 2, i, true, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource) + "</TD>");
                    referenceDetailMedia.Append("</TR>");
                    referenceDetailMedia.Append("<TR height=\"5\">");
                    referenceDetailMedia.Append("<TD class=\"backGroundWhite\"></TD>");
                    referenceDetailMedia.Append("</TR>");
                    referenceDetailMedia.Append("<TR height=\"10\"><TD></TD></TR>");
                    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent" + i + ""))
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent" + i + "", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
                    }
                    i++;
                    referenceMediaDetailText = referenceDetailMedia.ToString();
                }
                #endregion


                #region Campaign type
                if (WebApplicationParameters.AllowCampaignTypeOption &&
                   webSessionSave.CampaignType != CstWeb.CustomerSessions.CampaignType.notDefined
                    )
                {
                    displayCampaignTypeSelection = true;
                    AdExpressTextCampaintypeValue.Text = GestionWeb.GetWebWord(CampaignTypesInformation.Get(webSessionSave.CampaignType).WebTextId, _webSession.SiteLanguage);

                }
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

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// OnInit
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

        #region Bouton Fermer
        /// <summary>
        /// Gestion du bouton fermer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void closeImageButtonRollOverWebControl_Click(object sender, System.EventArgs e)
        {
            Response.Write("<script language=javascript>");
            Response.Write("	window.close();");
            Response.Write("</script>");
        }
        #endregion

        #region Méthodes Internes

        #region UpdateGlobalDates
        /// <summary>
        /// Adaptation des dates par rapport au composant GlobaldateSelection
        /// </summary>
        private void UpdateGlobalDates(CstCustomerSession.Period.Type type, WebSession webSessionSave, DateTime FirstDayNotEnable)
        {

            TNS.AdExpress.Domain.Layers.CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.date];
            IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);

            date.UpdateDate(type, ref _webSession, webSessionSave, FirstDayNotEnable);
            Dictionary<Vehicles.names, DateTime> d = date.GetLastAvailableDate();
        }
        #endregion

        #region comparaison entre la date de fin et la date d'aujourd'hui
        /// <summary>
        /// Verifie si la date de fin est inférieur ou non à la date de début
        /// </summary>
        /// <returns>vrai si la date de fin et inférieur à la date de début</returns>
        private bool CompareDateEnd(DateTime dateBegin, DateTime dateEnd)
        {
            if (dateEnd < dateBegin)
                return true;
            else
                return false;
        }
        #endregion

        #region product selection render
        /// <summary>
        /// Render product selection
        /// </summary>
        /// <param name="webSessionSave">webSession Save</param>
        /// <param name="webSession">webSession</param>
        /// <param name="nameProduct">name Product</param>
        /// <param name="k">index into universes</param>
        /// <param name="selectItemsInClassificationWebControl">select Items In Classification WebControl</param>
        /// <returns></returns>
        private static string RenderProductSelection(WebSession webSessionSave, WebSession webSession, string nameProduct, int k, TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl)
        {
            System.Text.StringBuilder t = new System.Text.StringBuilder();

            t.Append("<TR>");
            t.Append("<TD class=\"txtViolet11Bold backGroundWhite\" >&nbsp;");
            t.Append("<label>" + nameProduct + "</label></TD>");
            t.Append("</TR>");

            //Universe Label
            if (webSessionSave.SecondaryProductUniverses[k].Label != null && webSessionSave.SecondaryProductUniverses[k].Label.Length > 0)
            {
                t.Append("<TR>");
                t.Append("<TD class=\"txtViolet11Bold backGroundWhite\" >&nbsp;");
                t.Append("<Label>" + webSessionSave.SecondaryProductUniverses[k].Label + "</Label>");
                t.Append("</TD></TR>");
            }

            //Render universe html code
            t.Append("<TR height=\"20\">");
            //t.Append("<TD>&nbsp;</TD>");
            t.Append("<TD align=\"center\" vAlign=\"top\" class=\"backGroundWhite\">" + selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.SecondaryProductUniverses[k], webSession.DataLanguage, DBFunctions.GetDataSource(webSession)) + "</TD>");
            t.Append("</TR>");
            t.Append("<TR height=\"5\">");
            t.Append("<TD class=\"backGroundWhite\"></TD>");
            t.Append("</TR>");
            t.Append("<TR height=\"10\"><TD></TD></TR>");

            return t.ToString();
        }
        #endregion

        #endregion
    }
}
