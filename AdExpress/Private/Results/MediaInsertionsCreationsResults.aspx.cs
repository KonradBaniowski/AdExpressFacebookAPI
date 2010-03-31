#region Information
/*
 * auteur : Guillaume Ragneau
 * cr�� le :
 * modifi� le : 22/07/2004
 * par : Guillaume Ragneau
 *   19/12/2004 A. Obermeyer Int�gration de WebPage
 *	 13/12/2005 D. V. Mussuma Gestion du d�tail m�dia
 * */
#endregion

using System;
using System.Collections;
using System.Collections.Specialized;

using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Oracle.DataAccess.Client;

using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.Rules.Results;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DataAccessFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.AdExpress.Web.Controls.Headers;
using CommonGlobal=TNS.AdExpress.Domain.Web.Navigation;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using ConstantesWeb=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;

namespace AdExpress.Private.Results{
	/// <summary>
	/// MediaInsertionsCreationsResults affiche la liste des insertions d'un vehicule, d'une cat�gorie ou d'un support
	/// suivant les param�tres qu'on lui passe via ids:
	///		ids=-1,-1,10 ==> affichage de la liste de insertions pour le support 10
	///		ids=-1,24,-1 ==> affichage de la liste des insertyions pour la cat�gorie 24
	///		ids=3,-1,-1 ==> affichage de la liste des insertions de la presse
	///	On peut passer un autre param�tre optionnel � la page: zoomDate qui donne une p�riode � couvrir.
	///	Si ce param�tre est absent, on consid�re les dates pr�sentes en session
	/// </summary>
	public partial class MediaInsertionsCreationsResults :  TNS.AdExpress.Web.UI.PrivateWebPage{
	
//        #region Variables
//        /// <summary>
//        /// Code html du r�sultat
//        /// </summary>
//        public string result = "";
//        /// <summary>
//        /// Bouton
//        /// </summary>
//        protected System.Web.UI.WebControls.Button Button1;
//        /// <summary>
//        /// Cont�le d'affichage des onglets m�dia
//        /// </summary>
//        /// <summary>
//        /// Composant pour la navigation par p�riodes
//        /// </summary>
//        /// <summary>
//        /// Composant pour la gestion du Drag And Drop
//        /// </summary>
//        /// <summary>
//        /// Bouton ok
//        /// </summary>
//        protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl okImageButton;		
//        /// <summary>
//        /// Code html de fermeture du flash d'attente
//        /// </summary>
//        public string divClose= LoadingSystem.GetHtmlCloseDiv();
//        /// <summary>
//        /// Liste des param�tres post�s par le navigateur
//        /// </summary>				
//        private string[] ids=null;
//        /// <summary>
//        /// Liste des param�tres post�s par le navigateur
//        /// </summary>	
//        private string idsString="";
//        /// <summary>
//        /// Zoom Date
//        /// </summary>
//        private string zoomDate="";
////		/// <summary>
////		/// Indique si d�tail m�dia
////		/// </summary>
////		private bool isMediaDetail=false;
//        /// <summary>
//        /// Identifiant du m�dia courant
//        /// </summary>
//        private string idVehicle="-1";
//        /// <summary>
//        /// Indique si le media Internet est selection�
//        /// </summary>
//        private bool isInternetSelected = false;
//        /// <summary>
//        /// Contextual Menu
//        /// </summary>
//        /// <summary>
//        /// Info options du bouton droit
//        /// </summary>
//        /// <summary>
//        /// Liste des m�dias impact�s
//        /// </summary>
//        private ArrayList vehicleArr = null;
//        /// <summary>
//        /// Composant pour la gestion du Drag And Drop
//        /// </summary>
	
//        #endregion

//        #region Chargement de la page
//        /// <summary>
//        /// Chargement de la page:
//        ///		Chargement de la session
//        ///		Envoi du flash d'attente
//        ///		Initialisation de la connection � la base de donn�es
//        ///		Traduction du site
//        ///		Extraction du code HTML r�sultat:
//        ///			Extraction des param�tres pr�sents dans l'URL: ids ==> ["idVehicle","idCategory","idMedia"]
//        ///			G�n�ration du code HTML suivant qu'une p�riode de zoom est pr�cis�e ou qu'on consid�re les
//        ///			p�riodes pr�sentes en session
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        /// <remarks>
//        /// Utilise les m�thodes:
//        ///		public static string LoadingSystem.GetHtmlDiv(int language, Page page)
//        ///		public static void TNS.AdExpress.Web.DataAccess.Functions.closeDataBase(WebSession _webSession)
//        /// </remarks>
//        protected void Page_Load(object sender, System.EventArgs e){			

//            try{

//                #region Flash d'attente
//                Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
//                Page.Response.Flush();
//                #endregion		

//                InformationWebControl1.Language = _webSession.SiteLanguage;
//                //Call the internal method Page.RegisterPostBackScript()
//                MethodInfo methodInfo =
//                typeof(Page).GetMethod("RegisterPostBackScript",
//                          BindingFlags.Instance | BindingFlags.NonPublic);

//                if (methodInfo != null)
//                {
//                    methodInfo.Invoke(Page, new object[] { });
//                }

//            }
//            catch(System.Exception exc){
//                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
//                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
//                }
//            }
			
//        }
//        #endregion

//        #region Pr�render
//        /// <summary>
//        /// OnPreRender
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        protected override void OnPreRender(EventArgs e) {
//            base.OnPreRender (e);

//            #region Resultat
//            try{ 
//                bool uiEmpty=false;

//                try{
//                    //D�tails des insertions pour une alerte
//                    if(idVehicle!=null && idVehicle.Length>0){
//                        if(uiEmpty) {
//                            GenericDetailSelectionWebControl2.Visible=false;
//                            GenericDetailSelectionWebControl2.Enabled=false;
//                        }
//                        SetVehicleTabOptions(_webSession,VehicleHeaderWebControl1,vehicleArr,idVehicle,idsString,zoomDate,WebFunctions.Script.GenerateNumber());

						
						
//                        // Affichage de l'aide si on n'est pas en Alerte et Analyse Plan Media concurrentielle
//                        switch(_webSession.CurrentModule){
//                            case ConstantesWeb.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
//                            case ConstantesWeb.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
//                                MenuWebControl2.ForceHelp = "";
//                                break;
//                            default:
//                                MenuWebControl2.ForceHelp = ConstantesWeb.Links.HELP_FILE_PATH+"MediaInsertionsCreationsResultsHelp.aspx";
//                                break;
//                        }

//                        if (idVehicle != null && idVehicle.Length > 0 && int.Parse(idVehicle) != DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode()) 
//                        {
//                            MenuWebControl2.ForcePrint = "/Private/Results/Excel/MediaInsertionsCreations.aspx?idSession=" + _webSession.IdSession
//                                + "&zoomDate=" + zoomDate
//                                + "&ids=" + ids[0] + "," + ids[1] + "," + ids[2] + "," + ids[3] + "," + ids[4] 
//                                + "&idVehicleFromTab=" + idVehicle;
//                            MenuWebControl2.DisplayHtmlPrint = true;
//                        }

//                        if (int.Parse(idVehicle) == DBClassificationConstantes.Vehicles.names.directMarketing.GetHashCode()) {
//                            MenuWebControl2.ForceHelp = "";
//                        }

//                    }
//                    else{
//                        result = "<TABLE width=\"500\" class=\"insertionBorderV2 whiteBackGround\""
//                            + "cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">"
//                            + MediaInsertionsCreationsResultsUI.GetUIEmpty(_webSession.SiteLanguage);
//                        MenuWebControl2.Visible = MenuWebControl2.Enabled = false;
//                        InformationWebControl1.Visible = false;
//                    }
//                    _webSession.Save();
//                }
//                catch(System.Exception exc2){
//                    _webSession.Source.Close();
//                    Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(959, _webSession.SiteLanguage)+"\n\n"+exc2.Message));
//                }
//            }
//            catch(System.Exception){
//                _webSession.Source.Close();
//                Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
//            }
//            #endregion

//        }
//        #endregion

//        #region D�chargement de la page
//        /// <summary>
//        /// Ev�nement de d�chargement de la page
//        /// </summary>
//        /// <param name="sender">Objet qui lance l'�v�nement</param>
//        /// <param name="e">Arguments</param>
//        protected void Page_UnLoad(object sender, System.EventArgs e){
//        }
//        #endregion

//        #region DeterminePostBackMode
//        /// <summary>
//        /// DeterminePostBackMode
//        /// </summary>
//        /// <returns>DeterminePostBackMode</returns>
//        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
//        {

//            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();

//            MenuWebControl2.CustomerWebSession = _webSession;
//            MenuWebControl2.ForbidHelpPages = true;
//            bool withGenericDetailSelection = true;
//            isInternetSelected = false;

//            try
//            {
//                //Parem�tres post�s
//                idsString = Page.Request.QueryString.Get("ids");
//                ids = idsString.Split(',');
//                idVehicle = Page.Request.QueryString.Get("idVehicleFromTab");
//                if (Request.Form[periodNavigation.ID] != null)
//                {
//                    zoomDate = Request.Form[periodNavigation.ID];
//                }
//                else
//                {
//                    zoomDate = Page.Request.QueryString.Get("zoomDate");
//                    if (zoomDate != null && zoomDate.Length <= 0)
//                    {
//                        DateTime begin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
//                        if (_webSession.DetailPeriod == ConstantesWeb.CustomerSessions.Period.DisplayLevel.weekly)
//                        {
//                            AtomicPeriodWeek week = new AtomicPeriodWeek(begin);
//                            zoomDate = string.Format("{0}{1}", week.Year, week.Week.ToString("0#"));
//                        }
//                        else
//                        {
//                            zoomDate = begin.ToString("yyyyMM");
//                        }
//                    }
//                }


//                periodNavigation.Value = zoomDate;


//                //Obtient le m�dia (vehicle) courant
//                MediaInsertionsCreationsRules.GetImpactedVehicleIds(_webSession, ids[0], ids[1], ids[2], ids[3], ref vehicleArr, ref idVehicle, string.Empty);

//                #region initialisation de idVehicle et de vehicleArr si le media internet est selectione
//                ArrayList listVehicle = new ArrayList();

//                if (vehicleArr != null)
//                {

//                    if (vehicleArr.Count > 1)
//                    {

//                        for (int i = 0; i < vehicleArr.Count; i++)
//                            listVehicle.Add(vehicleArr[i].ToString());

//                        vehicleArr = listVehicle;

//                        if (vehicleArr.Contains((DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode()).ToString()))
//                        {
//                            vehicleArr.Remove((DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode()).ToString());
//                            if (idVehicle.Equals((DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode()).ToString()))
//                                idVehicle = vehicleArr[0].ToString();
//                        }
//                    }
//                }
//                #endregion

//                DetailInsertionPeriodNavigationWebControl1.DisplayVehicle = !(vehicleArr != null && vehicleArr.Count >= 2);

//                //Initialisation du composant DetailInsertionPeriodNavigationWebControl1
//                if (idVehicle != null && idVehicle.Length > 0 && !idVehicle.Equals((DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode()).ToString()))
//                {
//                    DetailInsertionPeriodNavigationWebControl1.CustomerWebSession = _webSession;
//                    DetailInsertionPeriodNavigationWebControl1.IdVehicle = Int64.Parse(idVehicle.ToString());
//                    DetailInsertionPeriodNavigationWebControl1.ZoomDate = (zoomDate != null) ? zoomDate : "";
//                    DetailInsertionPeriodNavigationWebControl1.Align = "left";
//                    DetailInsertionPeriodNavigationWebControl1.Ids = idsString;
//                    DetailInsertionPeriodNavigationWebControl1.Url = String.Format("http://{0}{1}", Page.Request.Url.Authority, Page.Request.Url.AbsolutePath);
//                    DetailInsertionPeriodNavigationWebControl1.PeriodContainerName = periodNavigation.ID;
//                }
//                else
//                {
//                    DetailInsertionPeriodNavigationWebControl1.Visible = false;
//                    DetailInsertionPeriodNavigationWebControl1.Enabled = false;

//                }

//                //Initialisation du composant MediaInsertionsCreationsResultsWebControl1 d'affichage des r�sultats
//                MediaInsertionsCreationsResultsWebControl1.CustomerWebSession = _webSession;
//                if (idVehicle != null && idVehicle.Length > 0 && (int.Parse(idVehicle) == DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode() || int.Parse(idVehicle) == DBClassificationConstantes.Vehicles.names.directMarketing.GetHashCode()))
//                {
//                    //				idVehicle = DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode().ToString();//On force le passage � l'identifiant AdNetTrack pour l'�tude du d�tail spot � spot du media Internet				
//                    GenericDetailSelectionWebControl2.Visible = GenericDetailSelectionWebControl2.Enabled = false;
//                    withGenericDetailSelection = false;
//                    if (int.Parse(idVehicle) == DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode())
//                    {
//                        MenuWebControl2.Visible = MenuWebControl2.Enabled = false;
//                        MediaInsertionsCreationsResultsWebControl1.AllowPaging = true;
//                        InformationWebControl1.Visible = false;
//                    }
//                    else
//                    {
//                        MenuWebControl2.Visible = MenuWebControl2.Enabled = true;
//                        MediaInsertionsCreationsResultsWebControl1.AllowPaging = false;
//                        InformationWebControl1.Visible = true;
//                    }
//                }

//                MediaInsertionsCreationsResultsWebControl1.ZoomDate = (zoomDate != null) ? zoomDate : "";
//                MediaInsertionsCreationsResultsWebControl1.Ids = idsString;
//                MediaInsertionsCreationsResultsWebControl1.IdVehicle = idVehicle;

//                //Initialisation de CustomerWebSession du composant GenericDetailSelectionWebControl2
//                if (idVehicle != null && idVehicle.Length > 0 && MediaInsertionsCreationsRules.IsRequiredGenericColmuns(_webSession) && withGenericDetailSelection)
//                {
//                    GenericDetailSelectionWebControl2.CustomerWebSession = _webSession;
//                    GenericDetailSelectionWebControl2.IdVehicleFromTab = Int64.Parse(idVehicle.ToString());
//                    GenericDetailSelectionWebControl2.ZoomDate = (zoomDate != null) ? zoomDate : "";
//                    GenericDetailSelectionWebControl2.Language = _webSession.SiteLanguage;
//                }
//                else
//                {
//                    GenericDetailSelectionWebControl2.Visible = false;
//                    GenericDetailSelectionWebControl2.Enabled = false;
//                }


//            }
//            catch (System.Exception exc)
//            {
//                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
//                {
//                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
//                }
//            }

//            return (tmp);

//        }
//        #endregion

//        #region Code g�n�r� par le Concepteur Web Form
//        /// <summary>
//        /// Initialisation
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        override protected void OnInit(EventArgs e)
//        {
//            //
//            // CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
//            //
//            InitializeComponent();
//            base.OnInit(e);
//        }
		

//        /// <summary>
//        /// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
//        /// le contenu de cette m�thode avec l'�diteur de code.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.Unload += new System.EventHandler(this.Page_UnLoad);
           
//        }
//        #endregion

//        #region M�thodes internes

//        /// <summary>
//        /// Options des onglets m�dia
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="vehicleHeaderWebControl">composant des onglets m�dias</param>
//        /// <param name="vehicleArr">tableau de m�dias</param>
//        /// <param name="idVehicleFromTab">identifiant du m�dia</param>
//        /// <param name="ids">param�tres d�tail m�dia</param>
//        /// <param name="zoomDate">date �tudi�e</param>
//        /// <param name="param">param�tre de cache</param>
//        private void SetVehicleTabOptions(WebSession webSession, VehicleHeaderWebControl vehicleHeaderWebControl,ArrayList vehicleArr,string idVehicleFromTab,string ids,string zoomDate,string param){
//            WebHeaderMediaDetailMenuItem headerMediaDetailMenuItem =null;
//            ListDictionary ht =null;
//            if(vehicleArr!=null && vehicleArr.Count>1){
//                ht = new ListDictionary();
//                vehicleHeaderWebControl.CustomerWebSession = webSession;
//                for(int i=0;i<vehicleArr.Count;i++){
//                    if(vehicleArr[i]!=null && vehicleArr[i].ToString().Length>0 && GetVehicleWebWord((DBClassificationConstantes.Vehicles.names)int.Parse(vehicleArr[i].ToString()))>0){
//                        headerMediaDetailMenuItem = new WebHeaderMediaDetailMenuItem(GetVehicleWebWord((DBClassificationConstantes.Vehicles.names)int.Parse(vehicleArr[i].ToString())),"/Private/Results/MediaInsertionsCreationsResults.aspx",true,true,"",ids,zoomDate,param,vehicleArr[i].ToString());
//                        ht.Add(vehicleArr[i].ToString(),headerMediaDetailMenuItem);						
//                        if(vehicleArr[i].ToString().Equals(idVehicleFromTab) && vehicleHeaderWebControl!=null)vehicleHeaderWebControl.ActiveMenu = int.Parse(idVehicleFromTab);
//                    }
//                }				
//                if(ht!=null && ht.Count>0){					
//                    vehicleHeaderWebControl.Headers = ht;
//                }
				
//            }else if(vehicleHeaderWebControl!=null)vehicleHeaderWebControl.Visible=false;

//        }

//        /// <summary>
//        /// D�termine le code correspondant au libell� d'un m�dia (vehicle)
//        /// </summary>
//        /// <param name="vehicleName">Vehicle</param>
//        /// <returns>Nom de la table</returns>
//        public static int GetVehicleWebWord(DBClassificationConstantes.Vehicles.names vehicleName){
//            switch(vehicleName){
//                case DBClassificationConstantes.Vehicles.names.press:
//                    return 1298;
//                case DBClassificationConstantes.Vehicles.names.newspaper:
//                    return 2620;
//                case DBClassificationConstantes.Vehicles.names.magazine:
//                    return 2621;
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                    return 646;
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    return 1299;
//                case DBClassificationConstantes.Vehicles.names.tv:				
//                    return 1300;
//                case DBClassificationConstantes.Vehicles.names.others:
//                    return 647;
//                case DBClassificationConstantes.Vehicles.names.outdoor:
//                    return 1302;
//                case DBClassificationConstantes.Vehicles.names.instore:
//                    return 2665;
//                case DBClassificationConstantes.Vehicles.names.adnettrack:
//                    return 648;
//                case DBClassificationConstantes.Vehicles.names.internet:
//                    return 1301;
//                case DBClassificationConstantes.Vehicles.names.directMarketing:
//                    return 2219;
//                default:
//                    return 0;
//            }
//        }

//        #endregion
	}
}
