#region Informations
// Auteur: A. Obermeyer
// Date de cr�ation: 
// Date de modification: 
//	 30/12/2004 A. Obermeyer Int�gration de WebPage
//   31/01/2005 A. obermeyer Correction date la derni�re semaine
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
using TNS.Ares.Domain.LS;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date;
using TNS.AdExpress.Domain.Web;
using refSystem = System.Reflection;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;
using System.Collections.Generic;


namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Page avec la liste des sessions sauvegard�es
	/// </summary>
	public partial class SearchSession : TNS.AdExpress.Web.UI.PrivateWebPage{
		
		#region MMI
		/// <summary>
		/// Contr�le En t�te de page
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
		/// Bouton R�sultat
		/// </summary>
		/// <summary>
		/// Bouton Supprimer
		/// </summary>
		/// <summary>
		/// S�lectionnez un r�sultat
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
		/// Liste des r�pertoires
		/// </summary>
		protected string listRepertories;		
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
        /// Specifies if alerts are activated or not
        /// </summary>

		#endregion

        #region Properties

        public bool IsAlertsActivated {
            get {
                return (AlertConfiguration.IsActivated && _webSession.CustomerLogin.HasModuleAssignmentAlertsAdExpress());
            }
        }
        /// <summary>
        /// Get if can save insertion customised levels
        /// </summary>
        public bool CanSaveInsertionCustomisedLevels
        {
            get
            {
                return (WebApplicationParameters.InsertionOptions.CanSaveLevels);
            }
        }

        #endregion

        #region Constructeur
        /// <summary>
		/// Constructeur
		/// </summary>
		public SearchSession():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){		
			
			try{
                _theme = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
				//Modification de la langue pour les Textes AdExpress                
			
				HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.MY_ADEXPRESS;

				#region Rollover des boutons

				deleteImageButtonRollOverWebControl.Attributes.Add("onclick", "javascript: return confirm('" + GestionWeb.GetWebWord(2572, _webSession.SiteLanguage) + "');");
			
				#endregion

				//Charge la liste des r�pertoires
				TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI myAdexpress=new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(_webSession,TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI.type.mySession,500);
			
				listRepertories= myAdexpress.GetSelectionTableHtmlUI(4,"");
			
				idSession=_webSession.IdSession;

				//Charge le script
				//script=myAdexpress.Script;
			
				// Gestion lorsqu'il n'y a pas de r�pertoire
				if(listRepertories.Length==0){
					resultImageButtonRollOverWebControl.Visible=false;
					deleteImageButtonRollOverWebControl.Visible=false;
					//detailImageButtonRollOverWebControl.Visible=false;
					AdExpressText6.Code=833;
				}	
			
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

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
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

        #region Bouton Alertes

        protected void alertOpenImageButtonRollOver_Click(object sender, System.EventArgs e)
        {
            try
            {
                _webSession.Source.Close();
                Response.Redirect("/Private/Alerts/ShowAlerts.aspx?idSession=" + _webSession.IdSession + "");
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

        #region Bouton Alertes Personnaliser
        /// <summary>
        /// Gestion du bouton Personnaliser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void personalizeAlertesImagebuttonrolloverwebcontrol_Click(object sender, System.EventArgs e) {
            try {
                _webSession.Source.Close();
                Response.Redirect("/Private/Alerts/PersonalizeAlerts.aspx?idSession=" + _webSession.IdSession + "");
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

        #region Bouton supprimer
        /// <summary>
		/// Gestion du bouton supprimer
		/// </summary>
		/// <param name="sender">Objet qui execute l'�v�nement</param>
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
						// Validation : confirmation de suppression de la requ�te
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(286,_webSession.SiteLanguage)+"\");");					
						Response.Write("</script>");
						// Actualise la page
						this.OnLoad(null);
					}
					else{
						// Erreur : la suppression de la requ�te a �chou�e
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(830,_webSession.SiteLanguage)+"\");");					
						Response.Write("</script>");
					}
				}
				else{
					// Erreur : veuillez s�lectionner une requ�te
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
		
		#region Bouton R�sultat
		/// <summary>
		/// Gestion du bouton r�sultat
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

                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);


				foreach (string currentKey in Request.Form.AllKeys){
					tabParent=currentKey.Split('_');
					if(tabParent[0]=="CKB") {
						idMySession=Int64.Parse(tabParent[1]);		
					}
				}

				if (idMySession!=0){

					webSessionSave=(WebSession)MySessionDataAccess.GetResultMySession(idMySession.ToString(),_webSession);
				
					DataTable dtModulesList= right.GetCustomerModuleListHierarchy();

					#region V�rification des droits sur les modules
					foreach(DataRow currentRow in dtModulesList.Rows){
						if((Int64)currentRow["idModule"]==webSessionSave.CurrentModule) {
							validModule=true; 
						}
                        //Verifie droit acc�s resultat courant
                        TNS.AdExpress.Domain.Web.Navigation.Module module = right.GetModule(webSessionSave.CurrentModule);
                        if (module != null)
                        {
                            validResultPage = (module.GetResultPageInformation(Convert.ToInt32(webSessionSave.CurrentTab)) != null);
                        }   
					}
					#endregion

					//Patch page de r�sultats Tableaux dynamiques
					if (webSessionSave != null && webSessionSave.LastReachedResultUrl.Length>0 && webSessionSave.LastReachedResultUrl.IndexOf("ASDynamicTables.aspx") >= 0) {
						webSessionSave.LastReachedResultUrl = webSessionSave.LastReachedResultUrl.Replace("ASDynamicTables.aspx", "ProductClassReport.aspx");
					}

					#region V�rification des flags produit pour le niveau de d�tail produit					
					if((!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("holdingcompany")>=0))||
						(!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE) && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("brand") >= 0))
						|| (!_webSession.CustomerLogin.HasAtLeastOneMediaAgencyFlag() && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("agency") >= 0))
						){
						_webSession.PreformatedProductDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;
					}
					else{
						_webSession.PreformatedProductDetail=webSessionSave.PreformatedProductDetail;
					}

					
					#endregion

					#region V�rification des flags produit pour le niveau de d�tail support
					if((!_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) && (webSessionSave.PreformatedMediaDetail.ToString().ToLower().IndexOf("slogan")>=0))
						){
						_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
					}
					else{
						_webSession.PreformatedMediaDetail=webSessionSave.PreformatedMediaDetail;
					}
					#endregion

					#region Param�tres
					_webSession.UserParameters=webSessionSave.UserParameters;
					#endregion

					#region Niveau de d�tail media (Generic)
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
						
							// Initialisation � media\cat�gorie
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
					
					#region Niveau de d�tail produit (Generic)
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

					#region Niveau de d�tail media AdnetTrack (Generic)
					try{
						if(webSessionSave.GenericAdNetTrackDetailLevel==null){
						
							// Initialisation � media\cat�gorie
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

                    #region Niveau de d�tail colonne (Generic)
                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE ||
                           webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        try {

                            if (webSessionSave.GenericColumnDetailLevel == null) {

                                // Initialisation � Support
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

					

                    #region P�riode s�lectionn�e (GlobalDateSelection)
                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES
                          || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.CELEBRITIES)
                    {

                        int oldYear = 2000;
                        long selectedVehicle = ((LevelInformation)webSessionSave.SelectionUniversMedia.FirstNode.Tag).ID;
                        if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                            //FirstDayNotEnable = WebFunctions.Dates.GetFirstDayNotEnabled(webSessionSave, selectedVehicle, oldYear,_webSession.Source); 
                            FirstDayNotEnable = dateDAL.GetFirstDayNotEnabled(new List<Int64>(new Int64[]{selectedVehicle}), oldYear); 

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

                                        if (WebApplicationParameters.UseComparativeLostWon 
                                            && webSessionSave.CustomerPeriodSelected != null 
                                            && webSessionSave.CustomerPeriodSelected.WithComparativePeriodPersonnalized) {
                                            _webSession.CustomerPeriodSelected = new CustomerPeriod(webSessionSave.PeriodBeginningDate, webSessionSave.PeriodEndDate, webSessionSave.CustomerPeriodSelected.ComparativeStartDate, webSessionSave.CustomerPeriodSelected.ComparativeEndDate);
                                        }
                                        else {
 
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


					//rajout� le 27/10 par Guillaume Ragneau
					if (_webSession.CurrentModule == CstWeb.Module.Name.INDICATEUR 
						|| _webSession.CurrentModule == CstWeb.Module.Name.TABLEAU_DYNAMIQUE){
                           
                            _webSession.LastAvailableRecapMonth = dateDAL.CheckAvailableDateForMedia(((LevelInformation)_webSession.SelectionUniversMedia.Nodes[0].Tag).ID);
						//_webSession.LastAvailableRecapMonth = DBFunctions.CheckAvailableDateForMedia(
						//	((LevelInformation)_webSession.SelectionUniversMedia.Nodes[0].Tag).ID, _webSession);
					}

                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.CELEBRITIES)
                    {
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
										//D�termine le dernier mois accessible en fonction de la fr�quence de livraison du client et
										//du dernier mois dispo en BDD
										//traitement de la notion de fr�quence
										UpdateRecapDates(CstCustomerSession.Period.Type.nLastMonth,ref notValidPeriod,ref invalidPeriodMessage);
									}
                                   break;

								case CstCustomerSession.Period.Type.currentYear:

									_webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy01");
									_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
									
									if( webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR ){
										//D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
										//du dernier mois dispo en BDD
										//traitement de la notion de fr�quence
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
								
									//Ann�e pr�c�dente est fonction de la derni�re ann�e de chargement des donn�es pour les Recap
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

								//Ann�e N-2
								case CstCustomerSession.Period.Type.nextToLastYear:
									//Ann�e N-2 est fonction de la derni�re ann�e de chargement des donn�es pour les Recap
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
										//D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
										//du dernier mois dispo en BDD
										//traitement de la notion de fr�quence	
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

									//Jour pr�c�dent
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
					
					//Ann�e agence m�dia
					_webSession.MediaAgencyFileYear=webSessionSave.MediaAgencyFileYear;
                    if (_webSession.CustomerLogin.HasAtLeastOneMediaAgencyFlag() && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("agency") >= 0)
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
                    _webSession.ComparativePeriodType = webSessionSave.ComparativePeriodType;
                    _webSession.PeriodSelectionType = webSessionSave.PeriodSelectionType;

					if (!_webSession.ComparativeStudy || !webSessionSave.Evolution)
						_webSession.Evolution = false;
					
		
					//Rajout� pour module Tableaux de bord
					_webSession.Format = webSessionSave.Format;
					_webSession.NamedDay = webSessionSave.NamedDay;
					_webSession.TimeInterval = webSessionSave.TimeInterval;
					_webSession.DetailPeriodBeginningDate =  webSessionSave.DetailPeriodBeginningDate;
					_webSession.DetailPeriodEndDate =  webSessionSave.DetailPeriodEndDate;
				
					#region Rajout� pour module Bilan de campagne (APPM)
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
                    //New univers Advertising Agency
                    _webSession.PrincipalAdvertisingAgnecyUniverses = webSessionSave.PrincipalAdvertisingAgnecyUniverses;
                    _webSession.SecondaryAdvertisingAgnecyUniverses = webSessionSave.SecondaryAdvertisingAgnecyUniverses;
                     //Profession universes
				    _webSession.PrincipalProfessionUniverses = webSessionSave.PrincipalProfessionUniverses;

					if(notValidPeriod){
						//Erreur : p�riode non disponible
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
					// Erreur : veuillez s�lectionner une requ�te
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
	

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page:
		///		Fermeture des connections BD
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region M�hodes internes

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
		/// Mets � jour les dates des Recap en fonction de la fr�quenece de livraison des donn�es
		/// </summary>
		/// <param name="periodType">Type de p�riode</param>
		/// <param name="notValidPeriod">Indique si p�riode invalide</param>
		/// <param name="invalidPeriodMessage">Message</param>
		private void UpdateRecapDates(CstCustomerSession.Period.Type periodType, ref bool notValidPeriod,ref string invalidPeriodMessage){

			DateTime downloadDate = new DateTime(_webSession.DownLoadDate,12,31);

			//D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
			//du dernier mois dispo en BDD
			//traitement de la notion de fr�quence
			switch(periodType){

				case CstCustomerSession.Period.Type.nLastMonth :		

					if(DateTime.Now.Year>_webSession.DownLoadDate){
						_webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");						
						_webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
							
					}				
					else if( _webSession.DownLoadDate == DateTime.Now.Year && int.Parse(_webSession.PeriodBeginningDate.Substring(0,4)) < DateTime.Now.Year)
						_webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");//L'�tude se fait sur une ann�e civile pour les module d'analyse sectorieilles
					break;
				
				case CstCustomerSession.Period.Type.currentYear :					
					if(DateTime.Now.Year>_webSession.DownLoadDate){
						_webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - _webSession.PeriodLength).ToString("yyyy01");
						_webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
							
					}
					break;

				case CstCustomerSession.Period.Type.dateToDateMonth :
					//Les analyses sectorielles portent au minimumn jusqu'� l'ann�e N-2
					//Si la p�riode enregistr�e est inferieur � l'ann�e N-2, alors il faut la ramener � l'ann�e N-2
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
                
                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                string absolutEndPeriod = dateDAL.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);
				
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

            CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.date];
            IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, refSystem.BindingFlags.CreateInstance | refSystem.BindingFlags.Instance | refSystem.BindingFlags.Public, null, null, null, null);

            date.UpdateDate(type, ref _webSession, webSessionSave, FirstDayNotEnable);
        }
        #endregion

        #region comparaison entre la date de fin et la date d'aujourd'hui
        /// <summary>
        /// Verifie si la date de fin est inf�rieur ou non � la date de d�but
        /// </summary>
        /// <returns>vrai si la date de fin et inf�rieur � la date de d�but</returns>
        private bool CompareDateEnd(DateTime dateBegin, DateTime dateEnd) {
            if (dateEnd < dateBegin)
                return true;
            else
                return false;
        }
        #endregion

		#endregion

        /// <summary>
        /// Open insertion saved pages
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">arguments</param>
        protected void insertionOpenImageButtonRollOverWebControl_Click(object sender, EventArgs e)
        {
            try
            {
                _webSession.Source.Close();
                Response.Redirect("/Private/MyAdExpress/PersonnalizeInsertion.aspx?idSession=" + _webSession.IdSession + "");
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
}
}
