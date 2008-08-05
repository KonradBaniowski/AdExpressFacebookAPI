#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
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
using System.Windows.Forms;

using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.UI;
using WebFunctions = TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Détail d'une Session
	/// </summary>
	public partial class MySessionDetailPopUp : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region MMI
		/// <summary>
		/// Text Produit
		/// </summary>
		/// <summary>
		/// Détail du résultat
		/// </summary>
		/// <summary>
		/// Choix de l'étude
		/// </summary>
		/// <summary>
		/// Affiche le choix de l'etude
		/// </summary>
		/// <summary>
		/// Période
		/// </summary>
		/// <summary>
		/// Date
		/// </summary>
		/// <summary>
		/// Média
		/// </summary>
		/// <summary>
		/// Unité
		/// </summary>
		/// <summary>
		/// Type d'unité (Euro...)
		/// </summary>
		/// <summary>
		/// Bouton Fermer
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// reference Advertiser
		/// </summary>
		/// <summary>
		/// Texte competitor Advertiser
		/// </summary>

		#endregion
		
		#region Variables
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
		/// Script
		/// </summary>
		protected string script;
		/// <summary>
		/// Affichage des annonceurs
		/// </summary> 
		public bool displayAdvertiser=false;
		/// <summary>
		/// Affichage du détail média
		/// </summary>
		public bool displayDetailMedia=false;
		/// <summary>
		/// détail média
		/// </summary>
		public string mediaDetailText="";
		/// <summary>
		/// Annonceur de référence
		/// </summary>
		public string referenceAdvertiserText="";		
		/// <summary>
		/// Affiche les éléments de références
		/// </summary>
		public bool referenceAdvertiserDisplay=false;
		/// <summary>
		/// Affiche les éléments concurrents
		/// </summary>
		public bool competitorAdvertiserDisplay=false;		
		/// <summary>
		/// Affiche les informations sur les agences médias
		/// </summary>
		public bool displayMediaAgency=false;
		/// <summary>
		/// Annonceurs concurrents
		/// </summary>
		public string competitorAdvertiserText="";
		/// <summary>
		/// Affiche les produits
		/// </summary>
		public bool displayProduct=false;
		/// <summary>
		/// Affiche les produits
		/// </summary>
		public string productText="";
		/// <summary>
		/// Etude comparative
		/// </summary>
		public bool comparativeStudy=false;
		/// <summary>
		/// Texte etude comparative
		/// </summary>
		public string comparativeStudyText="";
		/// <summary>
		/// Affiche les éléments dans reference media
		/// </summary>
		public bool displayReferenceDetailMedia=false;
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
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MySessionDetailPopUp():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try{

				#region Variables
				Int64 idMySession;			
				string periodText;
				int idAdvertiser=1;
				int idMedia=1;
				string dateBegin;
				string dateEnd;
                DateTime FirstDayNotEnable = DateTime.Now;
                DateTime lastDayEnable = DateTime.Now;
                DateTime tmpEndDate;
                DateTime tmpBeginDate;
                CstWeb.globalCalendar.comparativePeriodType comparativePeriodType;
                CstWeb.globalCalendar.periodDisponibilityType periodDisponibilityType;
                bool verifCustomerPeriod = false;
				#endregion
			
				idMySession = Int64.Parse(Page.Request.QueryString.Get("idMySession"));
				webSessionSave=(WebSession)MySessionDataAccess.GetResultMySession(idMySession.ToString(),_webSession);

				#region Langage
				//Modification de la langue pour les Textes AdExpress
                for (int j = 0; j < this.Controls.Count; j++) {
                    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[j].Controls, _webSession.SiteLanguage);
                }
				#endregion

				//Rollover
				//closeImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/fermer_up.gif";
				//closeImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/fermer_down.gif";

				// Affichage du module
				moduleLabel.Text=GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSessionSave.CurrentModule),_webSession.SiteLanguage);
			
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
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {

                    int oldYear = 2000;
                    long selectedVehicle = ((LevelInformation)webSessionSave.SelectionUniversMedia.FirstNode.Tag).ID;
                    FirstDayNotEnable = WebFunctions.Dates.GetFirstDayNotEnabled(webSessionSave, selectedVehicle, oldYear,_webSession.Source);
                    _webSession.CurrentModule = webSessionSave.CurrentModule;

                    switch (webSessionSave.DetailPeriod) {
                        case CstCustomerSession.Period.DisplayLevel.monthly:
                            if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.currentYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousYear &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastMonth &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousMonth) {
                                if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.dateToDate) {
                                    string startYearMonth = webSessionSave.PeriodBeginningDate;
                                    string endYearMonth = webSessionSave.PeriodEndDate;
                                    DateTime firstDayOfMonth = new DateTime(int.Parse(endYearMonth.ToString().Substring(0, 4)), int.Parse(endYearMonth.ToString().Substring(4, 2)), 1);
                                    Int32 lastDayOfMonth = ((firstDayOfMonth.AddMonths(1)).AddDays(-1)).Day;
                                    _webSession.PeriodBeginningDate = startYearMonth + "01";
                                    _webSession.PeriodEndDate = endYearMonth + lastDayOfMonth;
                                }
                                else {
                                    _webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                    _webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                }
                                _webSession.PeriodType = CstCustomerSession.Period.Type.dateToDate;
                            }
                            break;
                        case CstCustomerSession.Period.DisplayLevel.weekly:
                            if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastWeek &&
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousWeek) {
                                if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.dateToDate) {
                                    AtomicPeriodWeek startWeek = new AtomicPeriodWeek(int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(0, 4)), int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(4, 2)));
                                    AtomicPeriodWeek endWeek = new AtomicPeriodWeek(int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(0, 4)), int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(4, 2)));
                                    DateTime dateBeginTmp = startWeek.FirstDay;
                                    DateTime dateEndTmp = endWeek.FirstDay.AddDays(6);
                                    _webSession.PeriodBeginningDate = dateBeginTmp.Year.ToString() + dateBeginTmp.Month.ToString("00") + dateBeginTmp.Day.ToString("00");
                                    _webSession.PeriodEndDate = dateEndTmp.Year.ToString() + dateEndTmp.Month.ToString("00") + dateEndTmp.Day.ToString("00");
                                }
                                else {
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

                    switch (webSessionSave.DetailPeriod) {
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
                                webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousDay) {

                                tmpEndDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6, 2)));
                                tmpBeginDate = new DateTime(Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(6, 2)));

                                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {

                                    if (webSessionSave.DetailPeriod == CstCustomerSession.Period.DisplayLevel.monthly) {
                                        comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.dateToDate;
                                        periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                    }
                                    else if (webSessionSave.DetailPeriod == CstCustomerSession.Period.DisplayLevel.weekly) {
                                        comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.comparativeWeekDate;
                                        periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                    }
                                    else {
                                        comparativePeriodType = webSessionSave.CustomerPeriodSelected.ComparativePeriodType;
                                        periodDisponibilityType = webSessionSave.CustomerPeriodSelected.PeriodDisponibilityType;
                                    }

                                    switch (periodDisponibilityType) {

                                        case CstWeb.globalCalendar.periodDisponibilityType.currentDay:
                                            lastDayEnable = DateTime.Now;
                                            break;
                                        case CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                                            lastDayEnable = FirstDayNotEnable.AddDays(-1);
                                            break;

                                    }

                                    if (CompareDateEnd(lastDayEnable, tmpEndDate) || CompareDateEnd(tmpBeginDate, DateTime.Now))
                                        _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, true, comparativePeriodType, periodDisponibilityType);
                                    else {

                                        switch (webSessionSave.CustomerPeriodSelected.PeriodDisponibilityType) {

                                            case CstWeb.globalCalendar.periodDisponibilityType.currentDay:
                                                _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
                                                break;
                                            case CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                                                _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
                                                break;
                                        }
                                    }
                                }
                                else {
                                    if (CompareDateEnd(DateTime.Now, tmpEndDate) || CompareDateEnd(tmpBeginDate, DateTime.Now))
                                        _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                                    else
                                        _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
                                }
                                verifCustomerPeriod = true;
                            }
                            break;
                    }

                    if(!verifCustomerPeriod)
                        UpdateGlobalDates(webSessionSave.PeriodType, webSessionSave, FirstDayNotEnable);
                }
                #endregion

				#region période
				switch(webSessionSave.PeriodType){
					case CstCustomerSession.Period.Type.nLastMonth:
						periodText=webSessionSave.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(783,_webSession.SiteLanguage);					
						infoDateLabel1.Text=periodText;							
						break;
					case CstCustomerSession.Period.Type.nLastYear:
						periodText=webSessionSave.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(781,_webSession.SiteLanguage);					
						infoDateLabel1.Text=periodText;				
						break;
					case CstCustomerSession.Period.Type.previousMonth:
						infoDateLabel1.Text=GestionWeb.GetWebWord(788,_webSession.SiteLanguage);
						break;
					case CstCustomerSession.Period.Type.previousYear:
						infoDateLabel1.Text=GestionWeb.GetWebWord(787,_webSession.SiteLanguage);
						break;
						// Année courante		
					case  CstCustomerSession.Period.Type.currentYear:
						infoDateLabel1.Text=GestionWeb.GetWebWord(1228,_webSession.SiteLanguage);
						break;
						// Année N-2
					case CstCustomerSession.Period.Type.nextToLastYear:
						infoDateLabel1.Text=GestionWeb.GetWebWord(1229,_webSession.SiteLanguage);
						break;
					case CstCustomerSession.Period.Type.dateToDateMonth:
						string monthBegin;
						string monthEnd;
						if(int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(4,2))<10){
							monthBegin=TNS.FrameWork.Date.MonthString.Get(int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(5,1)),_webSession.SiteLanguage,10);
						}
						else{
							monthBegin=TNS.FrameWork.Date.MonthString.Get(int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(4,2)),_webSession.SiteLanguage,10);
						}
						if(int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(4,2))<10){
							monthEnd=TNS.FrameWork.Date.MonthString.Get(int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(5,1)),_webSession.SiteLanguage,10);
						}
						else{
							monthEnd=TNS.FrameWork.Date.MonthString.Get(int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(4,2)),_webSession.SiteLanguage,10);
						}					
						periodText=GestionWeb.GetWebWord(846,_webSession.SiteLanguage)+" "+monthBegin+" "+GestionWeb.GetWebWord(847,_webSession.SiteLanguage)+" "+monthEnd;
						infoDateLabel1.Text=periodText;					
						break;
					case CstCustomerSession.Period.Type.dateToDateWeek:
						AtomicPeriodWeek tmp=new AtomicPeriodWeek(int.Parse(webSessionSave.PeriodBeginningDate.Substring(0,4)),int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(4,2)));
						periodText=tmp.FirstDay.Date.ToString("dd/MM/yyyy");
						tmp=new AtomicPeriodWeek(int.Parse(webSessionSave.PeriodEndDate.Substring(0,4)),int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(4,2)));
						periodText+=" "+GestionWeb.GetWebWord(125,_webSession.SiteLanguage)+"";
						periodText+=" "+tmp.LastDay.Date.ToString("dd/MM/yyyy")+"";
						infoDateLabel1.Text=periodText;
						break;
                    case CstCustomerSession.Period.Type.nLastWeek:
						infoDateLabel1.Text=webSessionSave.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(784,_webSession.SiteLanguage);
						break;
                    case CstCustomerSession.Period.Type.previousWeek:
						infoDateLabel1.Text=GestionWeb.GetWebWord(789,_webSession.SiteLanguage);
						break;
					case CstCustomerSession.Period.Type.dateToDate:
						dateBegin = DateString.YYYYMMDDToDD_MM_YYYY(webSessionSave.PeriodBeginningDate.ToString(),_webSession.SiteLanguage);
						dateEnd = DateString.YYYYMMDDToDD_MM_YYYY(webSessionSave.PeriodEndDate.ToString(),_webSession.SiteLanguage);
						periodText=GestionWeb.GetWebWord(896,_webSession.SiteLanguage)+" "+dateBegin+" "+GestionWeb.GetWebWord(897,_webSession.SiteLanguage)+" "+dateEnd;
						infoDateLabel1.Text=periodText;
						break;
					case CstCustomerSession.Period.Type.cumlDate:
						dateBegin=DateString.YYYYMMDDToDD_MM_YYYY(_webSession.PeriodBeginningDate,_webSession.SiteLanguage);
						dateEnd=DateString.YYYYMMDDToDD_MM_YYYY( _webSession.PeriodEndDate,_webSession.SiteLanguage);

						periodText=GestionWeb.GetWebWord(896,_webSession.SiteLanguage)+" "+dateBegin+" "+GestionWeb.GetWebWord(897,_webSession.SiteLanguage)+" "+dateEnd;
						infoDateLabel1.Text=periodText;
						break;
                    case CstCustomerSession.Period.Type.nLastDays:
						infoDateLabel1.Text=webSessionSave.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(1974,_webSession.SiteLanguage);
						break;
					case CstCustomerSession.Period.Type.previousDay:
						infoDateLabel1.Text=GestionWeb.GetWebWord(1975,_webSession.SiteLanguage);
						break;
				}		
				#endregion

                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                    || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE) {

                    #region Période de l'étude
                    // Période de l'étude
                    if (_webSession.isStudyPeriodSelected()) {
                        displayStudyPeriod = true;
                        StudyPeriod.Text = HtmlFunctions.GetStudyPeriodDetail(_webSession);
                    }
                    #endregion

                    #region Période comparative
                    // Période comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodComparative()) {
                            displayComparativePeriod = true;
                            comparativePeriod.Text = HtmlFunctions.GetComparativePeriodDetail(_webSession);
                        }
                    }
                    #endregion

                    #region Type Sélection comparative
                    // Type Sélection comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isComparativePeriodTypeSelected()) {
                            displayComparativePeriodType = true;
                            ComparativePeriodType.Text = HtmlFunctions.GetComparativePeriodTypeDetail(_webSession);
                        }
                    }
                    #endregion

                    #region Type disponibilité des données
                    // Type disponibilité des données
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodDisponibilityTypeSelected()) {
                            displayPeriodDisponibilityType = true;
                            PeriodDisponibilityType.Text = HtmlFunctions.GetPeriodDisponibilityTypeDetail(_webSession);
                        }
                    }
                    #endregion

                }

                #region Etude Comparative
                // Etude comparative
                if (webSessionSave.ComparativeStudy) {
					comparativeStudy=true;
                    comparativeStudyText = GestionWeb.GetWebWord(1118, _webSession.SiteLanguage);
				}
				#endregion

				mySessionLabel.Text = TNS.AdExpress.Web.DataAccess.MyAdExpress.MySessionsDataAccess.GetSession(idMySession,_webSession);
				unitLabel.Text=GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.UnitsTraductionCodes[webSessionSave.Unit],_webSession.SiteLanguage);	
	
				mediaText= TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSessionSave.SelectionUniversMedia,false,false,false,600,false,false,_webSession.SiteLanguage,2,1,true);
				//if(webSessionSave.isAdvertisersSelected()){
				//    advertiserText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSessionSave.SelectionUniversAdvertiser,false,true,true,600,true,false,_webSession.SiteLanguage,2,1,true);
			
				//    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
				//        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				//    }
				//    if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
				//        Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				//    }
				//}	
				
				#region Agences médias
				// Agence média
				if(_webSession.PreformatedProductDetail==CstCustomerSession.PreformatedDetails.PreformatedProductDetails.agencyProduct
					|| _webSession.PreformatedProductDetail==CstCustomerSession.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser
					|| _webSession.PreformatedProductDetail==CstCustomerSession.PreformatedDetails.PreformatedProductDetails.group_agencyAgency
					)
				{
					displayMediaAgency=true;
					MediaAgency.Text=_webSession.MediaAgencyFileYear.Substring(_webSession.MediaAgencyFileYear.Length-4,4);;
				
				}
				#endregion

				int i=2;

				#region Ancienne version

				#region Produits
				// Produit
				//if (webSessionSave.isSelectionProductSelected()){
				//    displayProduct=true;
			
				//    if(((LevelInformation)webSessionSave.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess 
				//        ||	((LevelInformation)webSessionSave.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
				//        productAdExpressText.Code=815;
				//    }
				//    else if(((LevelInformation)webSessionSave.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess 
				//        ||	((LevelInformation)webSessionSave.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
				//        productAdExpressText.Code=965;
				//    }
				//    else if(((LevelInformation)webSessionSave.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess 
				//        ||	((LevelInformation)webSessionSave.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
				//        productAdExpressText.Code=966;
				//    }
				//    else if(((LevelInformation)webSessionSave.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess 
				//        ||	((LevelInformation)webSessionSave.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
				//        productAdExpressText.Code=964;
				//    }				

				//    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
				//        Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
				//    }

				//    // Affichage du System.Windows.Forms.TreeNode
				//    productText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSessionSave.SelectionUniversProduct,false,true,true,600,true,false,_webSession.SiteLanguage,2,i,true);
				//    i++;
				//}
				#endregion

				#region Annonceurs de références
				//if(webSessionSave.isReferenceAdvertisersSelected()){
				//    referenceAdvertiserDisplay=true;
				//    referenceAdvertiserAdexpresstext.Code=1195;
				//    if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
				//        Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
				//    }			
				//    // Affichage du System.Windows.Forms.TreeNode
				//    referenceAdvertiserText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSessionSave.ReferenceUniversAdvertiser,false,true,true,600,true,false,_webSession.SiteLanguage,2,i,true);
				//    i++;			
				//}
				#endregion

				#region Annonceurs concurrents
				//if(webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//    || webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//    ){
				//    idAdvertiser=0;
			
				//}
				////else{idAdvertiser=2;}

				//if(webSessionSave.isCompetitorAdvertiserSelected()){

				//    competitorAdvertiserDisplay=true;
				//    displayAdvertiser=false;
				//    System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
				//    competitorAdvertiserAdexpressText.Code=1196;
							
				//    while(webSessionSave.CompetitorUniversAdvertiser[idAdvertiser]!=null){
					
				//        System.Windows.Forms.TreeNode tree=null;
				//        if(webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//            || webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//            ){
				//            tree=(System.Windows.Forms.TreeNode)webSessionSave.CompetitorUniversAdvertiser[idAdvertiser];
			
				//        }else{
				//            tree=((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSessionSave.CompetitorUniversAdvertiser[idAdvertiser]).TreeCompetitorAdvertiser;
				//        }

				//        if(tree.FirstNode!=null){
				//            if(webSessionSave.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//                && webSessionSave.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//                ){
				//                t.Append("<TR>");
				//                t.Append("<TD></TD>");
				//                t.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
				//                t.Append("<Label>"+(string)(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSessionSave.CompetitorUniversAdvertiser[idAdvertiser]).NameCompetitorAdvertiser)+"</Label>");
				//                t.Append("</TD></TR>");

				//            }


				//            t.Append("<TR>");
				//            t.Append("<TD>&nbsp;</TD>");
				//            if(webSessionSave.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR 
				//                && webSessionSave.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE 
				//                ){
				//                t.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSessionSave.CompetitorUniversAdvertiser[idAdvertiser]).TreeCompetitorAdvertiser),false,true,true,600,true,false,_webSession.SiteLanguage,2,i,true)+"</TD>");
				//            }
						
				//            else{
				//                t.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(tree,false,true,true,600,true,false,_webSession.SiteLanguage,2,i,true)+"</TD>");
				//            }
				//            t.Append("</TR>");
				//            t.Append("<TR height=\"5\">");
				//            t.Append("<TD></TD>");
				//            t.Append("<TD bgColor=\"#ffffff\"></TD>");
				//            t.Append("</TR>");
					
				//            if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
				//                Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
				//            }
				//            i++;
				//            idAdvertiser++;
				//        }else{idAdvertiser++;}
				//    }

				//    competitorAdvertiserText=t.ToString();
				//}

				#endregion

				#endregion			
				
                //Univers produit principal sélectionné
				string nameProduct = "";
				System.Text.StringBuilder t = null;
				if (webSessionSave.PrincipalProductUniverses != null && webSessionSave.PrincipalProductUniverses.Count > 0) {

					 t = new System.Text.StringBuilder(1000);
					nameProduct = "";

					if (webSessionSave.PrincipalProductUniverses.Count > 1) {
						competitorAdvertiserDisplay = true;
						//productAdExpressText.Code = 2302;
					}
					else {
						displayProduct = true;
						productAdExpressText.Code = 1759;
					}

					TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl = new TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl();
					selectItemsInClassificationWebControl.TreeViewIcons = "/Styles/TreeView/Icons";
					selectItemsInClassificationWebControl.TreeViewScripts = "/Styles/TreeView/Scripts";
					selectItemsInClassificationWebControl.TreeViewStyles = "/Styles/TreeView/Css";
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
					selectItemsInClassificationWebControl.DBSchema = TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA;
					for (int k = 0; k < webSessionSave.PrincipalProductUniverses.Count; k++) {
						if (webSessionSave.PrincipalProductUniverses.Count > 1) {
							if (webSessionSave.PrincipalProductUniverses.ContainsKey(k)) {
								if (k > 0) {
									nameProduct = GestionWeb.GetWebWord(2301, _webSession.SiteLanguage);
								}
								else {
									nameProduct = GestionWeb.GetWebWord(2302, _webSession.SiteLanguage);
								}

								t.Append("<TR><TD></TD>");
                                t.Append("<TD class=\"txtViolet11Bold whiteBackGround\">&nbsp;");
								t.Append("<label>" + nameProduct + "</label></TD>");
								t.Append("</TR>");

								//Universe Label
								if (webSessionSave.PrincipalProductUniverses[k].Label != null && webSessionSave.PrincipalProductUniverses[k].Label.Length > 0) {
									t.Append("<TR>");
									t.Append("<TD></TD>");
                                    t.Append("<TD class=\"txtViolet11Bold whiteBackGround\">&nbsp;");
									t.Append("<Label>" + webSessionSave.PrincipalProductUniverses[k].Label + "</Label>");
									t.Append("</TD></TR>");
								}

								//Render universe html code
								t.Append("<TR height=\"20\">");
								t.Append("<TD>&nbsp;</TD>");
                                t.Append("<TD align=\"center\" vAlign=\"top\" class=\"whiteBackGround\">" + selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.PrincipalProductUniverses[k], _webSession.DataLanguage, _webSession.Source) + "</TD>");
								t.Append("</TR>");
								t.Append("<TR height=\"5\">");
								t.Append("<TD></TD>");
                                t.Append("<TD class=\"whiteBackGround\"></TD>");
								t.Append("</TR>");
								t.Append("<TR height=\"7\">");
								t.Append("<TD colSpan=\"2\"></TD>");
								t.Append("</TR>");

							}
						}
						else {
							if (webSessionSave.PrincipalProductUniverses.ContainsKey(k)) {
								productText += selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.PrincipalProductUniverses[k], _webSession.DataLanguage, _webSession.Source);
							}
						}
					}
					if (webSessionSave.PrincipalProductUniverses.Count > 1)
						competitorAdvertiserText = t.ToString();
				}

				//Univers produit secondaire
				if (webSessionSave.SecondaryProductUniverses != null && webSessionSave.SecondaryProductUniverses.Count > 0) {
					t = new System.Text.StringBuilder(1000);
					nameProduct = "";

					if (WebFunctions.Modules.IsDashBoardModule(webSessionSave)) {
						displayProduct = true;
						productAdExpressText.Code = 1759;
					}
					else {
						referenceAdvertiserAdexpresstext.Code = 2302;
					}
					TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl = new TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl();
					selectItemsInClassificationWebControl.TreeViewIcons = "/Styles/TreeView/Icons";
					selectItemsInClassificationWebControl.TreeViewScripts = "/Styles/TreeView/Scripts";
					selectItemsInClassificationWebControl.TreeViewStyles = "/Styles/TreeView/Css";
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
					selectItemsInClassificationWebControl.DBSchema = TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA;

					if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
						|| webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
					) {
						//Listes des annonceurs de références
						if (webSessionSave.SecondaryProductUniverses.ContainsKey(0)) {
							//Universe Label							
							referenceAdvertiserDisplay = true;
							referenceAdvertiserAdexpresstext.Code = 1195;
                            referenceAdvertiserText += selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.SecondaryProductUniverses[0],_webSession.DataLanguage,_webSession.Source);
						}

						//Listes des annonceurs concurrents
						if (webSessionSave.SecondaryProductUniverses.ContainsKey(1)) {
							
							nameProduct = GestionWeb.GetWebWord(1196, _webSession.SiteLanguage);
							competitorAdvertiserDisplay = true;
							competitorAdvertiserText = RenderProductSelection(webSessionSave, _webSession, nameProduct, 1, selectItemsInClassificationWebControl);
						}
					}
					else {
						for (int k = 0; k < webSessionSave.SecondaryProductUniverses.Count; k++) {
							if (webSessionSave.SecondaryProductUniverses.ContainsKey(k)) {
								if (k > 0) {
									if (k == 1) {
										competitorAdvertiserAdexpressText.Code = 2301;
										competitorAdvertiserDisplay = true;
									}									

									t.Append(RenderProductSelection(webSessionSave, _webSession, nameProduct, k, selectItemsInClassificationWebControl));

								}
								else {
									if (WebFunctions.Modules.IsDashBoardModule(webSessionSave))
                                        productText = selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.SecondaryProductUniverses[k],_webSession.DataLanguage,_webSession.Source);
									else {
										//Universe Label
										if (webSessionSave.SecondaryProductUniverses[k].Label != null && webSessionSave.SecondaryProductUniverses[k].Label.Length > 0) {
											referenceAdvertiserText += "<TR>";
											referenceAdvertiserText += "<TD></TD>";
                                            referenceAdvertiserText += "<TD class=\"txtViolet11Bold whiteBackGround\">&nbsp;";
											referenceAdvertiserText += "<Label>" + webSessionSave.SecondaryProductUniverses[k].Label + "</Label>";
											referenceAdvertiserText += "</TD></TR>";
										}
                                        referenceAdvertiserText += selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.SecondaryProductUniverses[k],_webSession.DataLanguage,_webSession.Source);
									}
								}
							}
						}
						if (webSessionSave.SecondaryProductUniverses.Count > 1)
							competitorAdvertiserText = t.ToString();
					}
				}

				#region Medias concurrents
				if (webSessionSave.isCompetitorMediaSelected()){
					displayDetailMedia=true;
					System.Text.StringBuilder mediaSB=new System.Text.StringBuilder(1000);
				
					mediaSB.Append("<TR><TD></TD>");
                    mediaSB.Append("<TD class=\"txtViolet11Bold whiteBackGround\">&nbsp;");
					mediaSB.Append("<label>"+GestionWeb.GetWebWord(1087,_webSession.SiteLanguage)+"</label></TD>");
					mediaSB.Append("</TR>");

					while((System.Windows.Forms.TreeNode)webSessionSave.CompetitorUniversMedia[idMedia]!=null){
					
						System.Windows.Forms.TreeNode tree=(System.Windows.Forms.TreeNode)webSessionSave.CompetitorUniversMedia[idMedia];				
						mediaSB.Append("<TR height=\"20\">");
						mediaSB.Append("<TD>&nbsp;</TD>");
                        mediaSB.Append("<TD align=\"center\" vAlign=\"top\" class=\"whiteBackGround\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)webSessionSave.CompetitorUniversMedia[idMedia], false, true, true, 600, true, false, _webSession.SiteLanguage, 2, i, true) + "</TD>");
						mediaSB.Append("</TR>");
						mediaSB.Append("<TR height=\"5\">");
						mediaSB.Append("<TD></TD>");
                        mediaSB.Append("<TD class=\"whiteBackGround\"></TD>");
						mediaSB.Append("</TR>");
						mediaSB.Append("<TR height=\"7\">");
						mediaSB.Append("<TD colSpan=\"2\"></TD>");
						mediaSB.Append("</TR>");
						if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
							Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
						}
						i++;
						idMedia++;
					}
					mediaDetailText=mediaSB.ToString();
				}
				#endregion

				#region Sélection Médias
				// Partie détail média
				if(webSessionSave.SelectionUniversMedia.FirstNode!=null && webSessionSave.SelectionUniversMedia.FirstNode.Nodes.Count>0){
			
					displayDetailMedia=true;
					System.Text.StringBuilder detailMedia=new System.Text.StringBuilder(1000);
				
					detailMedia.Append("<TR><TD></TD>");
                    detailMedia.Append("<TD class=\"txtViolet11Bold whiteBackGround\" >&nbsp;");
					detailMedia.Append("<label>"+GestionWeb.GetWebWord(1194,_webSession.SiteLanguage)+"</label></TD>");
					detailMedia.Append("</TR>");				
					
									
					detailMedia.Append("<TR height=\"20\">");
					detailMedia.Append("<TD>&nbsp;</TD>");
                    detailMedia.Append("<TD align=\"center\" vAlign=\"top\" class=\"whiteBackGround\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)webSessionSave.SelectionUniversMedia.FirstNode, false, true, true, 600, true, false, _webSession.SiteLanguage, 2, i, true) + "</TD>");
					detailMedia.Append("</TR>");
					detailMedia.Append("<TR height=\"5\">");
					detailMedia.Append("<TD></TD>");
                    detailMedia.Append("<TD class=\"whiteBackGround\"></TD>");
					detailMedia.Append("</TR>");
					detailMedia.Append("<TR height=\"7\">");
					detailMedia.Append("<TD colSpan=\"2\"></TD>");
					detailMedia.Append("</TR>");
					if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
						Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
					}
					i++;				
					mediaDetailText=detailMedia.ToString();			
				}
				#endregion

				#region Références Médias
				// Détail référence média			
				if(_webSession.isReferenceMediaSelected()){
			
					displayReferenceDetailMedia=true;
					System.Text.StringBuilder referenceDetailMedia=new System.Text.StringBuilder(1000);
				
					referenceDetailMedia.Append("<TR><TD></TD>");
                    referenceDetailMedia.Append("<TD class=\"txtViolet11Bold whiteBackGround\" >&nbsp;");
					referenceDetailMedia.Append("<label>"+GestionWeb.GetWebWord(1194,_webSession.SiteLanguage)+"</label></TD>");
					referenceDetailMedia.Append("</TR>");									
					referenceDetailMedia.Append("<TR height=\"20\">");
					referenceDetailMedia.Append("<TD>&nbsp;</TD>");
                    referenceDetailMedia.Append("<TD align=\"center\" vAlign=\"top\" class=\"whiteBackGround\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((System.Windows.Forms.TreeNode)_webSession.ReferenceUniversMedia, false, true, true, 600, true, false, _webSession.SiteLanguage, 2, i, true) + "</TD>");
					referenceDetailMedia.Append("</TR>");
					referenceDetailMedia.Append("<TR height=\"5\">");
					referenceDetailMedia.Append("<TD></TD>");
                    referenceDetailMedia.Append("<TD class=\"whiteBackGround\"></TD>");
					referenceDetailMedia.Append("</TR>");
					referenceDetailMedia.Append("<TR height=\"7\">");
					referenceDetailMedia.Append("<TD colSpan=\"2\"></TD>");
					referenceDetailMedia.Append("</TR>");
					if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"+i+"")) {
						Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent"+i+"",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(i));
					}
					i++;				
					referenceMediaDetailText=referenceDetailMedia.ToString();				
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
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region Bouton Fermer
		/// <summary>
		/// Gestion du bouton fermer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void closeImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
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
        private void UpdateGlobalDates(CstCustomerSession.Period.Type type, WebSession webSessionSave, DateTime FirstDayNotEnable) {

            AtomicPeriodWeek startWeek;
            AtomicPeriodWeek endWeek;
            AtomicPeriodWeek tmp;
            DateTime dateBegin;
            DateTime dateEnd;
            DateTime compareDate;
            DateTime lastDayEnable = DateTime.Now;
            DateTime tempDate = DateTime.Now;
            DateTime firstDayOfMonth;
            DateTime lastDayOfMonth;
            DateTime lastDayOfWeek;
            DateTime previousMonth;
            Int32 lastDayOfMonthInt;
            bool isLastCompletePeriod = false;
            CstWeb.globalCalendar.comparativePeriodType comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.dateToDate;
            CstWeb.globalCalendar.periodDisponibilityType periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.currentDay;

            if (webSessionSave.CurrentModule == CstWeb.Module.Name.ANALYSE_DYNAMIQUE) {

                try {
                    if (webSessionSave.CustomerPeriodSelected != null) {
                        comparativePeriodType = webSessionSave.CustomerPeriodSelected.ComparativePeriodType;
                        periodDisponibilityType = webSessionSave.CustomerPeriodSelected.PeriodDisponibilityType;
                    }
                    else {

                        switch (type) {
                            case CstCustomerSession.Period.Type.currentYear:
                            case CstCustomerSession.Period.Type.previousYear:
                            case CstCustomerSession.Period.Type.previousMonth:
                            case CstCustomerSession.Period.Type.previousDay:
                            case CstCustomerSession.Period.Type.nLastDays:
                                comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.dateToDate;
                                periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.currentDay;
                                break;
                            case CstCustomerSession.Period.Type.previousWeek:
                                comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.comparativeWeekDate;
                                periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.currentDay;
                                break;
                            case CstCustomerSession.Period.Type.nLastMonth:
                                comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.dateToDate;
                                periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                break;
                            case CstCustomerSession.Period.Type.nLastWeek:
                                comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.comparativeWeekDate;
                                periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                break;
                        }
                    }
                }
                catch (System.Exception) {

                    switch (type) {
                        case CstCustomerSession.Period.Type.currentYear:
                        case CstCustomerSession.Period.Type.previousYear:
                        case CstCustomerSession.Period.Type.previousMonth:
                        case CstCustomerSession.Period.Type.previousDay:
                        case CstCustomerSession.Period.Type.nLastDays:
                            comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.dateToDate;
                            periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.currentDay;
                            break;
                        case CstCustomerSession.Period.Type.previousWeek:
                            comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.comparativeWeekDate;
                            periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.currentDay;
                            break;
                        case CstCustomerSession.Period.Type.nLastMonth:
                            comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.dateToDate;
                            periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                            break;
                        case CstCustomerSession.Period.Type.nLastWeek:
                            comparativePeriodType = CstWeb.globalCalendar.comparativePeriodType.comparativeWeekDate;
                            periodDisponibilityType = CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                            break;
                    }
                }

                switch (periodDisponibilityType) {

                    case CstWeb.globalCalendar.periodDisponibilityType.currentDay:
                        lastDayEnable = DateTime.Now;
                        break;
                    case CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                        lastDayEnable = FirstDayNotEnable.AddDays(-1);
                        isLastCompletePeriod = true;
                        break;

                }
            }

            switch (type) {

                case CstCustomerSession.Period.Type.nLastYear:
                    _webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy0101");
                    _webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");
                    _webSession.PeriodType = CstCustomerSession.Period.Type.nLastYear;
                    break;
                case CstCustomerSession.Period.Type.previousYear:
                    _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy0101");
                    _webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy1231");
                    _webSession.PeriodType = CstCustomerSession.Period.Type.previousYear;
                    break;
                case CstCustomerSession.Period.Type.nLastMonth:

                    if (isLastCompletePeriod) {
                        firstDayOfMonth = new DateTime(lastDayEnable.Year, lastDayEnable.Month, 1);
                        lastDayOfMonth = (firstDayOfMonth.AddMonths(1)).AddDays(-1);

                        if (lastDayEnable == lastDayOfMonth) {
                            _webSession.PeriodBeginningDate = lastDayEnable.AddMonths(1 - webSessionSave.PeriodLength).ToString("yyyyMM01"); ;
                            _webSession.PeriodEndDate = lastDayEnable.ToString("yyyyMMdd");
                        }
                        else {
                            _webSession.PeriodBeginningDate = lastDayEnable.AddMonths(0 - webSessionSave.PeriodLength).ToString("yyyyMM01"); ;
                            previousMonth = lastDayEnable.AddMonths(-1);
                            firstDayOfMonth = new DateTime(previousMonth.Year, previousMonth.Month, 1);
                            lastDayOfMonthInt = ((firstDayOfMonth.AddMonths(1)).AddDays(-1)).Day;
                            _webSession.PeriodEndDate = firstDayOfMonth.ToString("yyyyMM") + lastDayOfMonthInt;
                        }

                    }
                    else {
                        _webSession.PeriodBeginningDate = lastDayEnable.AddMonths(1 - webSessionSave.PeriodLength).ToString("yyyyMM01");
                        _webSession.PeriodEndDate = lastDayEnable.ToString("yyyyMMdd");
                    }
                    _webSession.PeriodType = CstCustomerSession.Period.Type.nLastMonth;
                    break;
                case CstCustomerSession.Period.Type.previousMonth:
                    firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    lastDayOfMonthInt = (firstDayOfMonth.AddDays(-1)).Day;

                    _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM01");
                    _webSession.PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM") + lastDayOfMonthInt;
                    _webSession.PeriodType = CstCustomerSession.Period.Type.previousMonth;
                    break;
                case CstCustomerSession.Period.Type.nLastWeek:

                    startWeek = new AtomicPeriodWeek(lastDayEnable);
                    endWeek = new AtomicPeriodWeek(lastDayEnable);

                    if (isLastCompletePeriod) {
                        lastDayOfWeek = endWeek.FirstDay.AddDays(6);

                        if (lastDayOfWeek == lastDayEnable) {
                            dateEnd = lastDayEnable;
                        }
                        else {
                            startWeek.SubWeek(1);
                            endWeek.SubWeek(1);
                            lastDayOfWeek = endWeek.FirstDay.AddDays(6);
                            dateEnd = lastDayOfWeek;
                        }
                    }
                    else {
                        dateEnd = lastDayEnable;
                    }

                    _webSession.PeriodEndDate = dateEnd.Year.ToString() + dateEnd.Month.ToString("00") + dateEnd.Day.ToString("00");
                    startWeek.SubWeek(webSessionSave.PeriodLength - 1);
                    dateBegin = startWeek.FirstDay;
                    _webSession.PeriodBeginningDate = dateBegin.Year.ToString() + dateBegin.Month.ToString("00") + dateBegin.Day.ToString("00");
                    _webSession.PeriodType = CstCustomerSession.Period.Type.nLastWeek;
                    break;
                case CstCustomerSession.Period.Type.previousWeek:
                    tmp = new AtomicPeriodWeek(DateTime.Now);
                    tmp.SubWeek(1);
                    dateBegin = tmp.FirstDay;
                    _webSession.PeriodBeginningDate = dateBegin.Year.ToString() + dateBegin.Month.ToString("00") + dateBegin.Day.ToString("00");
                    dateEnd = tmp.FirstDay.AddDays(6);
                    _webSession.PeriodEndDate = dateEnd.Year.ToString() + dateEnd.Month.ToString("00") + dateEnd.Day.ToString("00");
                    _webSession.PeriodType = CstCustomerSession.Period.Type.previousWeek;
                    break;
                case CstCustomerSession.Period.Type.nLastDays:
                    tempDate = lastDayEnable;
                    _webSession.PeriodBeginningDate = tempDate.AddDays(1 - webSessionSave.PeriodLength).ToString("yyyyMMdd"); ;
                    _webSession.PeriodEndDate = tempDate.ToString("yyyyMMdd");
                    _webSession.PeriodType = CstCustomerSession.Period.Type.nLastDays;
                    break;
                case CstCustomerSession.Period.Type.previousDay:
                    _webSession.PeriodBeginningDate = _webSession.PeriodEndDate = DateTime.Now.AddDays(1 - webSessionSave.PeriodLength).ToString("yyyyMMdd");
                    _webSession.PeriodType = CstCustomerSession.Period.Type.previousDay;
                    break;
                case CstCustomerSession.Period.Type.currentYear:
                    _webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy0101");
                    _webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");
                    _webSession.PeriodType = CstCustomerSession.Period.Type.currentYear;
                    break;

            }

            compareDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6, 2)));

            if (webSessionSave.CurrentModule == CstWeb.Module.Name.ANALYSE_DYNAMIQUE) {

                if (CompareDateEnd(lastDayEnable, compareDate))
                    _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, true, comparativePeriodType, periodDisponibilityType);
                else {

                    switch (periodDisponibilityType) {

                        case CstWeb.globalCalendar.periodDisponibilityType.currentDay:
                            _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
                            break;
                        case CstWeb.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                            _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
                            break;
                    }
                }

            }
            else {
                if (CompareDateEnd(DateTime.Now, compareDate))
                    _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                else
                    _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
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
		private static string RenderProductSelection(WebSession webSessionSave, WebSession webSession, string nameProduct, int k, TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl) {
			System.Text.StringBuilder t = new System.Text.StringBuilder();

			t.Append("<TR><TD></TD>");
            t.Append("<TD class=\"txtViolet11Bold whiteBackGround\" >&nbsp;");
			t.Append("<label>" + nameProduct + "</label></TD>");
			t.Append("</TR>");

			//Universe Label
			if (webSessionSave.SecondaryProductUniverses[k].Label != null && webSessionSave.SecondaryProductUniverses[k].Label.Length > 0) {
				t.Append("<TR>");
				t.Append("<TD></TD>");
                t.Append("<TD class=\"txtViolet11Bold whiteBackGround\" >&nbsp;");
				t.Append("<Label>" + webSessionSave.SecondaryProductUniverses[k].Label + "</Label>");
				t.Append("</TD></TR>");
			}

			//Render universe html code
			t.Append("<TR height=\"20\">");
			t.Append("<TD>&nbsp;</TD>");
            t.Append("<TD align=\"center\" vAlign=\"top\" class=\"whiteBackGround\">" + selectItemsInClassificationWebControl.ShowUniverse(webSessionSave.SecondaryProductUniverses[k], webSession.DataLanguage, webSession.Source) + "</TD>");
			t.Append("</TR>");
			t.Append("<TR height=\"5\">");
			t.Append("<TD></TD>");
            t.Append("<TD class=\"whiteBackGround\"></TD>");
			t.Append("</TR>");
			t.Append("<TR height=\"7\">");
			t.Append("<TD colSpan=\"2\"></TD>");
			t.Append("</TR>");

			return t.ToString();
		}
		#endregion

		#endregion
	}
}
