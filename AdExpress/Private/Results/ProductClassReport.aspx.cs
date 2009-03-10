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
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;

#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Tableaux dynamiques de l'analyse sectorielle
	/// </summary>
	public partial class ProductClassReport :  TNS.AdExpress.Web.UI.ResultWebPage{
		
		#region Variables
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";				
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();				
		/// <summary>
		/// JKavaScripts � ins�rer
		/// </summary>
		public string scripts="";
		/// <summary>
		/// JKavaScripts bodyOnclick
		/// </summary>
		public string scriptBody="";		
		#endregion

		#region Variable MMI
		/// <summary>
		/// Contextual Menu
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl InitializeProductWebControl1;
//		/// <summary>
//		/// Annule la personnnalisation des �l�ments de r�f�rence ou concurrents
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
			// On r�initialise en KEuro car d'anciennes sessions peuvent �tre en Euro
			_webSession.Unit = WebConstantes.CustomerSessions.Unit.kEuro;
		}
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){

			try{				

				#region Flash d'attente
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=MenuWebControl2.ID){
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
					}
				}
				else{
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
				}
				#endregion

				VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
				if (vehicleInfo != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null
					&& !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category)
					&& !vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media)) {					
					_webSession.PreformatedMediaDetail = CstPreformatedDetail.PreformatedMediaDetails.vehicle;
					_webSession.Save();
				}

				#region Url Suivante
//				_nextUrl=this.recallWebControl.NextUrl;
				if(_nextUrl.Length!=0){
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}			
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
			
				#region Texte et langage du site
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
//				ExportWebControl1.CustomerWebSession=_webSession;
				ValidateSelectionButton.ToolTip = GestionWeb.GetWebWord(1183,_webSession.SiteLanguage);
				#endregion
	
				#region D�finition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"ASDynamicTablesHelp.aspx";
				#endregion

				#region Calcul du r�sultat
				scripts = WebFunction.ImageDropDownListScripts(ResultsOptionsWebControl1.ShowPictures);
				scriptBody = "javascript:openMenuTest();";
                #endregion

                #region Script
                // Script
                if (!Page.ClientScript.IsClientScriptBlockRegistered("ImageDropDownListScripts")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ImageDropDownListScripts", scripts);
                }
                #endregion

                #region MAJ de la Session
                _webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.Save();
				#endregion
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Initialisation
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifi�es pas encore charg�s)
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			base.OnInit(e);
		}
		#endregion
		
		#region DeterminePostBack
		/// <summary>
		/// D�termine la valeur de PostBack
		/// Initialise la propri�t� CustomerSession des composants "options de r�sultats" et gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			ResultsOptionsWebControl1.CustomerWebSession = _webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
            ResultWebControl1.CustomerWebSession = _webSession;

			return tmp;
		}
		#endregion

        #region PreInit
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            CstPreformatedDetail.PreformatedTables t = _webSession.PreformatedTable;
            if (Page.IsPostBack){
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
        #endregion

        protected void ValidateSelectionButton_Click(object sender, System.EventArgs e) {
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Get next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return this.MenuWebControl2.NextUrl;
		}
		#endregion

	}
}