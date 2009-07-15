#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//	 30/12/2004 A. Obermeyer Intégration de WebPage
//   31/01/2005 A. obermeyer Correction date la dernière semaine
//   21/06/2005 K. Shehzad Changes for brand and holding company rights
//	 29/11/2005	B.Masson	webSession.Source
//	 02/04/2007 Y.R'kaina Demande de confirmation de suppression
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
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess;
using Oracle.DataAccess.Client;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Utilities;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebModule=TNS.AdExpress.Constantes.Web.Module;
using WebRules=TNS.AdExpress.Web.Rules;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Alerts;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.DataBaseDescription;
using System.Reflection;

using AlertPeriodicity = TNS.AdExpress.Constantes.DB.Alerts.AlertPeriodicity;
using AlertStatuses = TNS.AdExpress.Constantes.DB.Alerts.AlertStatuses;


namespace AdExpress.Private.Alerts{
	/// <summary>
	/// Page avec la liste des sessions sauvegardées
	/// </summary>
	public partial class ShowAlerts : TNS.AdExpress.Web.UI.PrivateWebPage {
		
		#region MMI
		/// <summary>
		/// Contrôle En tête de page
		/// </summary>
		/// <summary>
		/// Mon AdExpress
		/// </summary>
		/// <summary>
		/// Commentaire Mon AdExpress
		/// </summary>
		/// <summary>
		/// Bouton de personalisation
		/// </summary>
		/// <summary>
		/// Bouton Résultat
		/// </summary>
		/// <summary>
		/// Bouton Supprimer
		/// </summary>
		/// <summary>
		/// Sélectionnez un résultat
		/// </summary>
		/// <summary>
		/// Mes Univers
		/// </summary>
		/// <summary>
		/// Bouton ouvrir Univers
		/// </summary>
		/// <summary>
		/// Commentaire Mes Univers
		/// </summary>
		/// <summary>
		/// Bouton ouvrir pdf
		/// </summary>
		/// <summary>
		/// Commentaire Mes pdf
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Liste des répertoires
		/// </summary>
		protected string listAlerts;		
		/// <summary>
		/// Script
		/// </summary>
		protected string script;
        /// <summary>
        /// Current Theme Name
        /// </summary>
        protected string _theme;
		/// <summary>
		/// id Session
		/// </summary>
		public Int64 idMySession=0;
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// idSession
		/// </summary>
		public string idSession;
        /// <summary>
        /// Alert Data Access Layer
        /// </summary>
        private IAlertDAL alertDAL = null;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public ShowAlerts()
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
		protected void Page_Load(object sender, System.EventArgs e){		
			
			try{
                _theme = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
				//Modification de la langue pour les Textes AdExpress                
			
				HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.MY_ADEXPRESS;

				//Charge la liste des répertoires
				TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI myAdexpress=new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(_webSession,TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI.type.mySession,500);

                idSession = _webSession.IdSession;

                #region Alerts
                if (NyxConfiguration.IsAlertsActivated)
                {
                    // Loading Data Access Layer
                    DataAccessLayer layer = NyxConfiguration.GetDataAccessLayer(NyxDataAccessLayer.Alert);
                    TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                    alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null, null);

                    // Loading alerts and binding them
                    AlertCollection alerts = alertDAL.GetAlerts(_webSession.CustomerLogin.IdLogin);
                    this.repeaterAlerts.DataSource = alerts;
                    this.repeaterAlerts.DataBind();


                    //Charge le script
                    //script=myAdexpress.Script;

                    // Gestion lorsqu'il n'y a pas de répertoire
                    if (alerts.Count == 0)
                        AdExpressText6.Code = 833;
                }
                else
                    this.blockAlerts.Visible = false;
                #endregion

                #region Script
                // Script
                if (!Page.ClientScript.IsClientScriptBlockRegistered("script")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "script", myAdexpress.Script);
                }
				// popup
				if (!Page.ClientScript.IsClientScriptBlockRegistered("myAdExpress")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"myAdExpress",TNS.AdExpress.Web.Functions.Script.MyAdExpress(idSession,_webSession));
				}
				// Champ hidden 
				if (!Page.ClientScript.IsClientScriptBlockRegistered("insertHidden")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"insertHidden",TNS.AdExpress.Web.Functions.Script.InsertHidden());
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

        /// <summary>
        /// Alerts Binding callback
        /// </summary>
        /// <param name="sender">Object calling the method</param>
        /// <param name="e">Data binding parameter, which contains an Alert object</param>
        protected void alertsItemBinding(object sender, RepeaterItemEventArgs e)
        {
            Alert alert = (Alert)e.Item.DataItem;
            AlertOccurenceCollection occurrences = alertDAL.GetOccurrences(alert.AlertId);

            HtmlContainerControl contentContainer = (HtmlContainerControl)e.Item.FindControl("content");
            HtmlContainerControl headerAlert = (HtmlContainerControl)e.Item.FindControl("headerAlert");
            headerAlert.InnerHtml += String.Format(" - {0} {1}", occurrences.Count, (occurrences.Count <= 1 ? GestionWeb.GetWebWord(2600, _siteLanguage) : GestionWeb.GetWebWord(2601, _siteLanguage)));

            HtmlImage status = (HtmlImage)e.Item.FindControl("flagStatus");
            switch (alert.Status)
            {
                case AlertStatuses.ToDelete:
                    status.Src = "/App_Themes/DefaultAdExpressFr/Images/Common/flagUnactive.gif";
                    break;
                case AlertStatuses.Activated:
                    status.Src = "/App_Themes/DefaultAdExpressFr/Images/Common/flagActivated.gif";
                    break;
            }

            // Checking if this alert has any occurrence. If so, binding the
            // inner repeater to the occurrences collection
            if (occurrences.Count > 0)
            {
                // Binding occurrences' repeater
                Repeater repeaterAlertOccurrences = (Repeater)e.Item.FindControl("repeaterAlertOccurrences");
                repeaterAlertOccurrences.Visible = true;
                repeaterAlertOccurrences.DataSource = occurrences;
                repeaterAlertOccurrences.DataBind();

                // Changing occurrences' list visibility
                HtmlContainerControl listOccurrences = (HtmlContainerControl)e.Item.FindControl("listOccurrences");
                listOccurrences.Visible = true;
                headerAlert.Attributes.Add("onclick", String.Format("var elem = document.getElementById('{0}'); ", contentContainer.ClientID) + "if (elem.style.display == '') { elem.style.display = 'none'; } else { elem.style.display = ''; } ;");

                // Setting the alert detailed information
                HtmlContainerControl details = (HtmlContainerControl)e.Item.FindControl("alertDetails");
                WebSession session = (WebSession)alert.Session;
                TNS.AdExpress.Domain.Web.Navigation.Module module = session.CustomerLogin.GetModule(session.CurrentModule);
                string dayName = null;
                if (alert.Periodicity == AlertPeriodicity.Weekly)
                    dayName = WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo.DateTimeFormat.DayNames.GetValue(alert.PeriodicityValue - 1).ToString();

                // Getting HTML value
                details.InnerHtml = alert.ToHtml(_siteLanguage, module.IdWebText, dayName);
                
            }
            else
            {
                // Otherwise, displaying a message indicating that this
                // alert doesn't have any occurrence
                Label lblNoOccurrence = (Label)e.Item.FindControl("lblNoOccurrence");
                lblNoOccurrence.Visible = true;
                lblNoOccurrence.Text = GestionWeb.GetWebWord(2587, _siteLanguage);
            }
        }

        /// <summary>
        /// Callback called when creating the occurrences' list
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Arguments</param>
        protected void alertOccurrenceItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Getting occurrence
            AlertOccurence occ = (AlertOccurence)e.Item.DataItem;

            // Creating link with specific occurrence data
            HyperLink hl = (HyperLink)e.Item.FindControl("lnkOccurrence");
            hl.Text = GestionWeb.GetWebWord(2588, _siteLanguage);
            hl.Text += " " + occ.DateSend.ToString();
            hl.NavigateUrl = String.Format("/Private/Alerts/ShowAlert.aspx?idSession={0}&idOcc={1}", Request.QueryString["idSession"], occ.AlertOccurrenceId);
        }

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
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
		private void InitializeComponent() {
           
            this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion

		#region Bouton Personnaliser
		/// <summary>
		/// Gestion du bouton Personnaliser
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void personalizeImagebuttonrolloverwebcontrol_Click(object sender, System.EventArgs e) {
			try{
                _webSession.Source.Close();
				Response.Redirect("/Private/MyAdexpress/PersonnalizeSession.aspx?idSession="+_webSession.IdSession+"");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton Personnaliser
		/// <summary>
		/// Gestion du bouton Personnaliser
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void universOpenImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{
                _webSession.Source.Close();
				Response.Redirect("/Private/Universe/PersonnalizeUniverse.aspx?idSession="+_webSession.IdSession+"");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton ouvir Pdf
		/// <summary>
		/// Gestion du bouton ouvir Pdf
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void pdfOpenImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{
                _webSession.Source.Close();
				Response.Redirect("/Private/MyAdexpress/PdfFiles.aspx?idSession="+_webSession.IdSession+"");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}

		#endregion

		#region Bouton supprimer
		/// <summary>
		/// Gestion du bouton supprimer
		/// </summary>
		/// <param name="sender">Objet qui execute l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void deleteImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{
			
				string[] tabParent=null;
				Int64 idMySession=0;

				foreach (string currentKey in Request.Form.AllKeys){
					tabParent=currentKey.Split('_');
					if(tabParent[0]=="CKB") {
						idMySession=Int64.Parse(tabParent[1]);		
					}
				}
				if (idMySession!=0){
					//MySessionDataAccess deleteSession=new MySessionDataAccess(idMySession,new OracleConnection(_webSession.CustomerLogin.OracleConnectionString));
					MySessionDataAccess deleteSession=new MySessionDataAccess(idMySession,_webSession);
					if (deleteSession.Delete()){
						// Validation : confirmation de suppression de la requête
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(286,_webSession.SiteLanguage)+"\");");					
						Response.Write("</script>");
						// Actualise la page
						this.OnLoad(null);
					}
					else{
						// Erreur : la suppression de la requête a échouée
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(830,_webSession.SiteLanguage)+"\");");					
						Response.Write("</script>");
					}
				}
				else{
					// Erreur : veuillez sélectionner une requête
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(831,_webSession.SiteLanguage)+"\");");					
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion
		
		#region Bouton Résultat
		/// <summary>
		/// Gestion du bouton résultat
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void resultImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		
			try{
				string[] tabParent=null;
				Int64 idMySession=0;
				WebSession webSessionSave;
				AtomicPeriodWeek tmp;
				bool validModule=false;
				bool notValidPeriod = false;
				//TNS.FrameWork.DB.Common.IDataSource source = new TNS.FrameWork.DB.Common.InternationalOracleDataSource("User Id=" + _webSession.CustomerLogin.Login + "; Password=" + _webSession.CustomerLogin.PassWord + " " + TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING);
				TNS.AdExpress.Right right=new TNS.AdExpress.Right(_webSession.CustomerLogin.Login,_webSession.CustomerLogin.PassWord,_webSession.SiteLanguage);
				string PeriodBeginningDate="";
				string PeriodEndDate="";
				string invalidPeriodMessage="";
                DateTime tmpEndDate;
                DateTime tmpBeginDate;
                DateTime lastDayEnable = DateTime.Now;
                DateTime FirstDayNotEnable = DateTime.Now;
                CstWeb.globalCalendar.comparativePeriodType comparativePeriodType;
                CstWeb.globalCalendar.periodDisponibilityType periodDisponibilityType;
                bool verifCustomerPeriod = false;
                bool validResultPage = true;

			
				foreach (string currentKey in Request.Form.AllKeys){
					tabParent=currentKey.Split('_');
					if(tabParent[0]=="CKB") {
						idMySession=Int64.Parse(tabParent[1]);		
					}
				}

				if (idMySession!=0){

					webSessionSave=(WebSession)MySessionDataAccess.GetResultMySession(idMySession.ToString(),_webSession);
				
					DataTable dtModulesList= right.GetCustomerModuleListHierarchy();

					#region Vérification des droits sur les modules
					foreach(DataRow currentRow in dtModulesList.Rows){
						if((Int64)currentRow["idModule"]==webSessionSave.CurrentModule) {
							validModule=true; 
						}
                        //Verifie droit accès resultat courant
                        TNS.AdExpress.Domain.Web.Navigation.Module module = right.GetModule(webSessionSave.CurrentModule);
                        if (module != null)
                        {
                            validResultPage = (module.GetResultPageInformation(Convert.ToInt32(webSessionSave.CurrentTab)) != null);
                        }   
					}
					#endregion

					//Patch page de résultats Tableaux dynamiques
					if (webSessionSave != null && webSessionSave.LastReachedResultUrl.Length>0 && webSessionSave.LastReachedResultUrl.IndexOf("ASDynamicTables.aspx") >= 0) {
						webSessionSave.LastReachedResultUrl = webSessionSave.LastReachedResultUrl.Replace("ASDynamicTables.aspx", "ProductClassReport.aspx");
					}

					#region Vérification des flags produit pour le niveau de détail produit					
					if((!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("holdingcompany")>=0))||
						(!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE) && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("brand") >= 0))
						|| (!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_AGENCY) && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("agency") >= 0))
						){
						_webSession.PreformatedProductDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;
					}
					else{
						_webSession.PreformatedProductDetail=webSessionSave.PreformatedProductDetail;
					}

					
					#endregion

					#region Vérification des flags produit pour le niveau de détail support
					if((!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) && (webSessionSave.PreformatedMediaDetail.ToString().ToLower().IndexOf("slogan")>=0))
						){
						_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
					}
					else{
						_webSession.PreformatedMediaDetail=webSessionSave.PreformatedMediaDetail;
					}
					#endregion

					#region Paramètres
					_webSession.UserParameters=webSessionSave.UserParameters;
					#endregion

					#region Niveau de détail media (Generic)
					try{
                        if(webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE||
                           webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE) {
                            ArrayList levels=new ArrayList();
                            levels.Add(1);
                            levels.Add(2);
                            levels.Add(3);
                            _webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                        }
						if(webSessionSave.GenericMediaDetailLevel==null){
						
							// Initialisation à media\catégorie
							ArrayList levels=new ArrayList();
							levels.Add(1);
							levels.Add(2);
							_webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
						
						}
					}
					catch(System.Exception){
						ArrayList levels=new ArrayList();
						levels.Add(1);
						levels.Add(2);
						_webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
					}
					#endregion
					
					#region Niveau de détail produit (Generic)
					try{
						if(webSessionSave.GenericProductDetailLevel==null){
							ArrayList levels=PopulateGenericProductDetailLevel(webSessionSave);
							_webSession.GenericProductDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.customLevels);
						}
					}
					catch(System.NotImplementedException){
						ArrayList levels=PopulateGenericProductDetailLevel(webSessionSave);
						_webSession.GenericProductDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.customLevels);					
					}
					catch(System.Exception){
						ArrayList levels=new ArrayList();
						levels.Add(8);
						_webSession.GenericProductDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
					}
					#endregion

					#region Niveau de détail media AdnetTrack (Generic)
					try{
						if(webSessionSave.GenericAdNetTrackDetailLevel==null){
						
							// Initialisation à media\catégorie
							ArrayList levels=new ArrayList();
							levels.Add(1);
							levels.Add(2);
							_webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
						
						}
					}
					catch(System.Exception){
						ArrayList levels=new ArrayList();
						levels.Add(1);
						levels.Add(2);
						_webSession.GenericAdNetTrackDetailLevel = new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
					}
					#endregion

                    #region Niveau de détail colonne (Generic)
                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE ||
                           webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        try {

                            if (webSessionSave.GenericColumnDetailLevel == null) {

                                // Initialisation à Support
                                ArrayList levels = new ArrayList();
                                levels.Add(3);
                                _webSession.GenericColumnDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

                            }

                        }
                        catch (System.Exception) {
                            ArrayList levels = new ArrayList();
                            levels.Add(3);
                            _webSession.GenericColumnDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                        }
                    }
                    #endregion

					if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS && (webSessionSave.SelectionUniversMedia.FirstNode == null || webSessionSave.SelectionUniversMedia.FirstNode.Tag == null)) {
                        _webSession.SelectionUniversMedia.Nodes.Clear();
                        System.Windows.Forms.TreeNode tmpNode=new System.Windows.Forms.TreeNode("TELEVISION");
						tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, VehiclesInformation.EnumToDatabaseId(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv), "TELEVISION");
						webSessionSave.SelectionUniversMedia.Nodes.Add(tmpNode);
                    }

                    _webSession.CompetitorUniversAdvertiser=webSessionSave.CompetitorUniversAdvertiser;
					_webSession.CompetitorUniversMedia=webSessionSave.CompetitorUniversMedia;
					_webSession.CompetitorUniversProduct=webSessionSave.CompetitorUniversProduct;
					_webSession.CurrentModule=webSessionSave.CurrentModule;
					_webSession.CurrentTab=webSessionSave.CurrentTab;
					_webSession.SelectionUniversAdvertiser=webSessionSave.SelectionUniversAdvertiser;
					_webSession.SelectionUniversMedia=webSessionSave.SelectionUniversMedia;
					_webSession.SelectionUniversProduct=webSessionSave.SelectionUniversProduct;
					_webSession.CurrentUniversAdvertiser=webSessionSave.CurrentUniversAdvertiser;
					_webSession.CurrentUniversMedia=webSessionSave.CurrentUniversMedia;
					_webSession.CurrentUniversProduct=webSessionSave.CurrentUniversProduct;

					_webSession.DetailPeriod=webSessionSave.DetailPeriod;

					
					_webSession.Percentage=webSessionSave.Percentage;
					_webSession.Insert=webSessionSave.Insert;


					_webSession.PeriodLength=webSessionSave.PeriodLength;
					_webSession.PeriodType=webSessionSave.PeriodType;

					

                    #region Période sélectionnée (GlobalDateSelection)
                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {

                        int oldYear = 2000;
                        long selectedVehicle = ((LevelInformation)webSessionSave.SelectionUniversMedia.FirstNode.Tag).ID;
						if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
							FirstDayNotEnable = WebFunctions.Dates.GetFirstDayNotEnabled(webSessionSave, selectedVehicle, oldYear,_webSession.Source); 

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
                                        DateTime dateBegin = startWeek.FirstDay;
                                        DateTime dateEnd = endWeek.FirstDay.AddDays(6);
                                        _webSession.PeriodBeginningDate = dateBegin.Year.ToString() + dateBegin.Month.ToString("00") + dateBegin.Day.ToString("00");
                                        _webSession.PeriodEndDate = dateEnd.Year.ToString() + dateEnd.Month.ToString("00") + dateEnd.Day.ToString("00");
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

                                    tmpEndDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6,2)));
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
                                        else 
                                            _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
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
                    }
                    #endregion


					//rajouté le 27/10 par Guillaume Ragneau
					if (_webSession.CurrentModule == CstWeb.Module.Name.INDICATEUR 
						|| _webSession.CurrentModule == CstWeb.Module.Name.TABLEAU_DYNAMIQUE){
						_webSession.LastAvailableRecapMonth = DBFunctions.CheckAvailableDateForMedia(
							((LevelInformation)_webSession.SelectionUniversMedia.Nodes[0].Tag).ID, _webSession);
					}

                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                        if(!verifCustomerPeriod)
                            UpdateGlobalDates(webSessionSave.PeriodType, webSessionSave, FirstDayNotEnable);
                    }
					else if(!WebFunctions.Modules.IsDashBoardModule(webSessionSave) && webSessionSave.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
//						if(DateTime.Now.Year<=_webSession.DownLoadDate){ 
							
							switch(webSessionSave.PeriodType){

								case CstCustomerSession.Period.Type.nLastMonth:

									_webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-(webSessionSave.PeriodLength-1)).ToString("yyyyMM");
									_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");

									if( webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR ){
										//Détermine le dernier mois accessible en fonction de la fréquence de livraison du client et
										//du dernier mois dispo en BDD
										//traitement de la notion de fréquence
										UpdateRecapDates(CstCustomerSession.Period.Type.nLastMonth,ref notValidPeriod,ref invalidPeriodMessage);
									}
                                   break;

								case CstCustomerSession.Period.Type.currentYear:

									_webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy01");
									_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
									
									if( webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR ){
										//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
										//du dernier mois dispo en BDD
										//traitement de la notion de fréquence
										UpdateRecapDates(CstCustomerSession.Period.Type.currentYear,ref notValidPeriod,ref invalidPeriodMessage);
									}
									break;

								case CstCustomerSession.Period.Type.nLastYear:
									_webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy01");
									_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
									break;

								case CstCustomerSession.Period.Type.previousMonth:
									_webSession.PeriodEndDate = _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
									break;

								case CstCustomerSession.Period.Type.previousYear:
								
									//Année précédente est fonction de la dernière année de chargement des données pour les Recap
									if( (webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR )
										&& (DateTime.Now.AddYears(-1).Year==_webSession.DownLoadDate)
										){
										_webSession.PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy")+"12";
										_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy")+"01";
									}else{
										_webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy")+"12";
										_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy")+"01";
									
									}
									break;

								//Année N-2
								case CstCustomerSession.Period.Type.nextToLastYear:
									//Année N-2 est fonction de la dernière année de chargement des données pour les Recap
									if( (webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR )
										&& (DateTime.Now.AddYears(-1).Year==_webSession.DownLoadDate)
										){
										_webSession.PeriodEndDate = DateTime.Now.AddYears(-3).ToString("yyyy")+"12";
										_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-3).ToString("yyyy")+"01";
									}else{
										_webSession.PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy")+"12";
										_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy")+"01";
									
									}
									break;

								case CstCustomerSession.Period.Type.dateToDateMonth:

									_webSession.PeriodBeginningDate=webSessionSave.PeriodBeginningDate;
									_webSession.PeriodEndDate=webSessionSave.PeriodEndDate;

									if( webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR ){
										//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
										//du dernier mois dispo en BDD
										//traitement de la notion de fréquence	
											UpdateRecapDates(CstCustomerSession.Period.Type.dateToDateMonth,ref notValidPeriod,ref invalidPeriodMessage);
									}
									break;
								case CstCustomerSession.Period.Type.dateToDateWeek:
									_webSession.PeriodBeginningDate=webSessionSave.PeriodBeginningDate;
									_webSession.PeriodEndDate=webSessionSave.PeriodEndDate;
									break;

								case CstCustomerSession.Period.Type.nLastWeek:
									tmp = new AtomicPeriodWeek(DateTime.Now);
									if (tmp.Week < 10){
										_webSession.PeriodEndDate = tmp.Year.ToString()+"0"+tmp.Week.ToString();
									}
									else{
										_webSession.PeriodEndDate = tmp.Year.ToString()+tmp.Week.ToString();
									}
									tmp.SubWeek(webSessionSave.PeriodLength-1);
									if(tmp.Week<10){
										_webSession.PeriodBeginningDate = tmp.Year.ToString()+"0"+tmp.Week.ToString();
									}else {
										_webSession.PeriodBeginningDate = tmp.Year.ToString()+tmp.Week.ToString();
									}
									break;

								case CstCustomerSession.Period.Type.previousWeek:
									tmp = new AtomicPeriodWeek(DateTime.Now);
									tmp.SubWeek(1);
									if(tmp.Week<10){
										_webSession.PeriodBeginningDate =_webSession.PeriodEndDate= tmp.Year.ToString()+"0"+tmp.Week.ToString();
									}else {
										_webSession.PeriodBeginningDate =_webSession.PeriodEndDate= tmp.Year.ToString()+tmp.Week.ToString();
									}
									//_webSession.PeriodEndDate = _webSession.PeriodBeginningDate = tmp.Week.ToString();
									break;

								case CstCustomerSession.Period.Type.dateToDate:
									_webSession.PeriodBeginningDate=webSessionSave.PeriodBeginningDate;
									_webSession.PeriodEndDate=webSessionSave.PeriodEndDate;
									break;

									//N derniers jours
								case CstCustomerSession.Period.Type.nLastDays :
									
									_webSession.PeriodBeginningDate = DateTime.Now.AddDays(1 - _webSession.PeriodLength).ToString("yyyyMMdd");
									_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");									
									break;

									//Jour précédent
								case CstCustomerSession.Period.Type.previousDay :
														
									_webSession.PeriodBeginningDate = _webSession.PeriodEndDate =DateTime.Now.AddDays(1 - _webSession.PeriodLength).ToString("yyyyMMdd");
								
									break;

								default :
									_webSession.PeriodBeginningDate=webSessionSave.PeriodBeginningDate;
									_webSession.PeriodEndDate=webSessionSave.PeriodEndDate;	
									break;
							}
//						}else{							
//								_webSession.PeriodBeginningDate=webSessionSave.PeriodBeginningDate;
//								_webSession.PeriodEndDate=webSessionSave.PeriodEndDate;										
//						}
					}else{
						if(WebFunctions.Modules.IsDashBoardModule(webSessionSave)){
							try{
							WebFunctions.Dates.WebSessionSaveDownloadDates(webSessionSave,ref PeriodBeginningDate,ref PeriodEndDate);
							}catch(System.Exception err){
								notValidPeriod = true;
								invalidPeriodMessage = err.Message;								
							}
							_webSession.PeriodBeginningDate=PeriodBeginningDate;
							_webSession.PeriodEndDate=PeriodEndDate;
							if(_webSession.PeriodType==CstCustomerSession.Period.Type.LastLoadedWeek || _webSession.PeriodType==CstCustomerSession.Period.Type.LastLoadedMonth)
								_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
						}else if(webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE){
							try{
								WebFunctions.Modules.LoadModuleStudyPeriodDates(_webSession,webSessionSave);
							}catch(System.Exception err){
								notValidPeriod = true;
								invalidPeriodMessage = err.Message;								
							}
						}
					}					
					
					//Année agence média
					_webSession.MediaAgencyFileYear=webSessionSave.MediaAgencyFileYear;					
					if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_AGENCY) && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("agency")>=0)
						&& !WebFunctions.CheckedText.IsStringEmpty(_webSession.MediaAgencyFileYear)
						&& _webSession.PeriodBeginningDate.Length>0
						){
						_webSession.MediaAgencyFileYear = DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+_webSession.PeriodBeginningDate.Substring(0,4); 
					}

					_webSession.ReachedModule=webSessionSave.ReachedModule;
					_webSession.ReferenceUniversAdvertiser=webSessionSave.ReferenceUniversAdvertiser;
					_webSession.ReferenceUniversMedia=webSessionSave.ReferenceUniversMedia;
					_webSession.ReferenceUniversProduct=webSessionSave.ReferenceUniversProduct;
                    //_webSession.Sort=webSessionSave.Sort;
					_webSession.Sorting=webSessionSave.Sorting;
					_webSession.Unit=webSessionSave.Unit;

					//Patch last reahce result URL pour tableaux Dynamiques
					_webSession.LastReachedResultUrl=webSessionSave.LastReachedResultUrl;
					


					_webSession.ModuleTraductionCode=webSessionSave.ModuleTraductionCode;


					_webSession.ComparaisonCriterion = webSessionSave.ComparaisonCriterion;
					
					if(_webSession.PeriodEndDate.Length>0 && (DateTime.Now.Year - int.Parse(_webSession.PeriodEndDate.Substring(0,4)) < 2))
						_webSession.ComparativeStudy = webSessionSave.ComparativeStudy;
					else _webSession.ComparativeStudy = false;
					_webSession.CustomizedReferenceComcurrentElements = webSessionSave.CustomizedReferenceComcurrentElements;
					_webSession.PreformatedTable = webSessionSave.PreformatedTable;
					_webSession.PDM = webSessionSave.PDM;
					_webSession.PDV = webSessionSave.PDV;
					_webSession.PersonalizedElementsOnly=webSessionSave.PersonalizedElementsOnly;
					_webSession.Graphics=webSessionSave.Graphics;

					if (!_webSession.ComparativeStudy || !webSessionSave.Evolution)
						_webSession.Evolution = false;
					
		
					//Rajouté pour module Tableaux de bord
					_webSession.Format = webSessionSave.Format;
					_webSession.NamedDay = webSessionSave.NamedDay;
					_webSession.TimeInterval = webSessionSave.TimeInterval;
					_webSession.DetailPeriodBeginningDate =  webSessionSave.DetailPeriodBeginningDate;
					_webSession.DetailPeriodEndDate =  webSessionSave.DetailPeriodEndDate;
				
					#region Rajouté pour module Bilan de campagne (APPM)
					_webSession.CurrentUniversAEPMTarget = webSessionSave.CurrentUniversAEPMTarget;
					_webSession.SelectionUniversAEPMWave = webSessionSave.SelectionUniversAEPMWave;
					_webSession.SelectionUniversOJDWave = webSessionSave.SelectionUniversOJDWave;
					_webSession.SelectionUniversAEPMTarget = webSessionSave.SelectionUniversAEPMTarget;
					_webSession.CurrentUniversAEPMWave = webSessionSave.CurrentUniversAEPMWave;
					_webSession.CurrentUniversOJDWave = webSessionSave.CurrentUniversOJDWave;
					_webSession.EmailRecipient = webSessionSave.EmailRecipient;
					_webSession.Ecart = webSessionSave.Ecart;
					_webSession.ExportedPDFFileName = webSessionSave.ExportedPDFFileName;
					_webSession.PublicationBeginningDate = webSessionSave.PublicationBeginningDate ;
					_webSession.PublicationEndDate = webSessionSave.PublicationEndDate;
					_webSession.PublicationDateType = webSessionSave.PublicationDateType;
					#endregion

					//Nouveaux univers produit
					_webSession.PrincipalProductUniverses = webSessionSave.PrincipalProductUniverses;
					_webSession.SecondaryProductUniverses = webSessionSave.SecondaryProductUniverses;
					//Nouveaux univers Media
					_webSession.PrincipalMediaUniverses = webSessionSave.PrincipalMediaUniverses;
					_webSession.SecondaryMediaUniverses = webSessionSave.SecondaryMediaUniverses;

					if(notValidPeriod){
						//Erreur : période non disponible
						Response.Write("<script language=javascript>");
//						Response.Write("	alert(\""+GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)+"\");");	
						Response.Write("	alert(\""+invalidPeriodMessage+"\");");					
						Response.Write("</script>");
					}
					else if (validModule){
                        if (validResultPage)
                        {
                            _webSession.Save();
                            if (_webSession.LastReachedResultUrl.Length != 0)
                            {
                                //_webSession.Source.Close();
                                Response.Redirect(_webSession.LastReachedResultUrl + "?idSession=" + _webSession.IdSession);
                            }
                            else
                            {

                                //Erreur : Impossible de charger la session
                                Response.Write("<script language=javascript>");
                                Response.Write("	alert(\"" + GestionWeb.GetWebWord(851, _webSession.SiteLanguage) + "\");");
                                Response.Write("</script>");
                            }
                        }
                        else
                        {
                            //Erreur : Cette session n'est plus dispo
                            Response.Write("<script language=javascript>");
                            Response.Write("	alert(\"" + GestionWeb.GetWebWord(2455, _webSession.SiteLanguage) + "\");");
                            Response.Write("</script>");
                        }
					}
					else{
						// Erreur : Droits insuffisants
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(832,_webSession.SiteLanguage)+"\");");					
						Response.Write("</script>");
					}
				}
				else{
					// Erreur : veuillez sélectionner une requête
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(831,_webSession.SiteLanguage)+"\");");					
					Response.Write("</script>");
				}				
			}
			catch(TNS.AdExpress.Domain.Exceptions.NoDataException) {
				Response.Write("<script language=\"JavaScript\">alert(\""+GestionWeb.GetWebWord(1787,_webSession.SiteLanguage)+"\");</script>");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}

		#endregion	
	

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page:
		///		Fermeture des connections BD
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region Méhodes internes

        #region PopulateGenericProductDetailLevel
        /// <summary>
		/// Populate GenericProductDetailLevel array list while GenericProductDetailLevel save field is null
		/// </summary>
		/// <param name="webSessionSave">Customer session saved</param>
		/// <returns>List of levels Ids</returns>
		private ArrayList PopulateGenericProductDetailLevel(WebSession webSessionSave){
			ArrayList levels=new ArrayList();
			try{
				switch(webSessionSave.PreformatedProductDetail){
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
						levels.Add(11);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
						levels.Add(8);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
						levels.Add(8);
						levels.Add(9);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
						levels.Add(8);
						levels.Add(9);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupBrand:
						levels.Add(8);
						levels.Add(13);
						levels.Add(9);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupBrandProduct:
						levels.Add(8);
						levels.Add(13);
						levels.Add(9);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupProduct:
						levels.Add(8);
						levels.Add(13);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupSegmentBrandProduct:
						levels.Add(8);
						levels.Add(13);
						levels.Add(14);
						levels.Add(9);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupSegmentProduct:
						levels.Add(8);
						levels.Add(13);
						levels.Add(14);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
						levels.Add(8);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
						levels.Add(16);
						levels.Add(8);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
						levels.Add(16);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand:
						levels.Add(9);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
						levels.Add(13);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
						levels.Add(15);
						levels.Add(16);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
						levels.Add(15);
						levels.Add(16);
						levels.Add(8);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
						levels.Add(15);
						levels.Add(16);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
						levels.Add(13);
						levels.Add(8);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiserBrand:
						levels.Add(13);
						levels.Add(8);
						levels.Add(9);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiserProduct:
						levels.Add(13);
						levels.Add(8);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAvertiserBrandProduct:
						levels.Add(13);
						levels.Add(8);
						levels.Add(9);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
						levels.Add(13);
						levels.Add(9);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
						levels.Add(13);
						levels.Add(9);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
						levels.Add(13);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupSegment:
						levels.Add(13);
						levels.Add(14);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
						levels.Add(7);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
						levels.Add(7);
						levels.Add(8);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
						levels.Add(7);
						levels.Add(8);
						levels.Add(9);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
						levels.Add(7);
						levels.Add(8);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
						levels.Add(11);
						levels.Add(8);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
						levels.Add(11);
						levels.Add(8);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
						levels.Add(11);
						levels.Add(7);
						levels.Add(8);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
						levels.Add(11);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
						levels.Add(11);
						levels.Add(12);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
						levels.Add(11);
						levels.Add(12);
						levels.Add(13);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
						levels.Add(14);
						levels.Add(8);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
						levels.Add(14);
						levels.Add(8);
						levels.Add(9);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
						levels.Add(14);
						levels.Add(8);
						levels.Add(10);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
						levels.Add(14);
						levels.Add(9);
						break;
					case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
						levels.Add(14);
						levels.Add(10);
						break;
					default:
						levels.Add(8);
						break;
				}
			}
			catch(System.Exception){
				levels.Clear();
				levels.Add(8);
			}
			return(levels);
        }
        #endregion

        #region UpdateRecapDates
        /// <summary>
		/// Mets à jour les dates des Recap en fonction de la fréquenece de livraison des données
		/// </summary>
		/// <param name="periodType">Type de période</param>
		/// <param name="notValidPeriod">Indique si période invalide</param>
		/// <param name="invalidPeriodMessage">Message</param>
		private void UpdateRecapDates(CstCustomerSession.Period.Type periodType, ref bool notValidPeriod,ref string invalidPeriodMessage){

			DateTime downloadDate = new DateTime(_webSession.DownLoadDate,12,31);

			//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
			//du dernier mois dispo en BDD
			//traitement de la notion de fréquence
			switch(periodType){

				case CstCustomerSession.Period.Type.nLastMonth :		

					if(DateTime.Now.Year>_webSession.DownLoadDate){
						_webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");						
						_webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
							
					}				
					else if( _webSession.DownLoadDate == DateTime.Now.Year && int.Parse(_webSession.PeriodBeginningDate.Substring(0,4)) < DateTime.Now.Year)
						_webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");//L'étude se fait sur une année civile pour les module d'analyse sectorieilles
					break;
				
				case CstCustomerSession.Period.Type.currentYear :					
					if(DateTime.Now.Year>_webSession.DownLoadDate){
						_webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - _webSession.PeriodLength).ToString("yyyy01");
						_webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
							
					}
					break;

				case CstCustomerSession.Period.Type.dateToDateMonth :
					//Les analyses sectorielles portent au minimumn jusqu'à l'année N-2
					//Si la période enregistrée est inferieur à l'année N-2, alors il faut la ramener à l'année N-2
					if(int.Parse(_webSession.PeriodBeginningDate.ToString().Substring(0,4))< DateTime.Now.AddYears(-2).Year ||
						int.Parse(_webSession.PeriodEndDate.ToString().Substring(0,4))<DateTime.Now.AddYears(-2).Year){
						_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy")+_webSession.PeriodBeginningDate.Substring(4,2);
						_webSession.PeriodEndDate =  DateTime.Now.AddYears(-2).ToString("yyyy")+_webSession.PeriodEndDate.Substring(4,2);							
					}
					
					if(int.Parse(_webSession.PeriodBeginningDate.ToString().Substring(0,4))< DateTime.Now.AddYears(-1).Year ||
						int.Parse(_webSession.PeriodEndDate.ToString().Substring(0,4))<DateTime.Now.AddYears(-1).Year)
						_webSession.ComparativeStudy = false;
					break;

				default:
					break;
			}
			
			if(periodType == CstCustomerSession.Period.Type.nLastMonth || periodType == CstCustomerSession.Period.Type.currentYear){
				string absolutEndPeriod = Dates.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
				if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) 	|| (absolutEndPeriod.Substring(4,2).Equals("00"))){
					notValidPeriod=true;
					invalidPeriodMessage = GestionWeb.GetWebWord(1787,_webSession.SiteLanguage);
				}else{						
					_webSession.PeriodEndDate = absolutEndPeriod;											
				}
			}
        }
        #endregion

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
                else 
                    _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
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

		#endregion
	
		
	}
}
