#region Information
/*
 * Auteur : D. Mussuma
  Cr�� le :  30/11/2006
  Date de modification : 

	*/
#endregion

#region Namespace
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
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Functions;


#endregion

namespace AdExpress.Private.Results
{
	/// <summary>
	/// Page de r�sultats du parrainage TV
	/// </summary>
	public partial class SponsorshipResults : TNS.AdExpress.Web.UI.ResultWebPage{	
	
		#region Variables

		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession;							
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

		#region variables MMI
		/// <summary>
		/// Contr�le Titre du module
		/// </summary>
		/// <summary>
		/// Contr�le Options des r�sultats //ResultsTableTypesWebControl1
		/// </summary>
		/// <summary>
		/// Contr�le Type de tableau de r�sultats
		/// </summary>
		/// <summary>
		/// Annnule la personnalisation des �l�ments de r�f�rences et concurrents
		/// </summary>
		/// <summary>
		/// Bouton de validation
		/// </summary>
		/// <summary>
		/// Contr�le passerelle vres les autres modules
		/// </summary>
		/// <summary>
		/// Contr�le menu d'ent�te 
		/// </summary>
		/// <summary>
		/// Contr�le information
		/// </summary>
		/// <summary>
		/// Contr�le niveau de d�tail g�n�rique
		/// </summary>

		/// <summary>
		/// Composant tableau de r�sultat
		/// </summary>
		/// <summary>
		/// Option affiner univers support
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page : 
		///		Flash d'attente
		///		Initialisation des connections � la BD
		///		Redirection en cas changement d'un crit�re de s�lection
		///		Traduction du site
		///		Extraction du code HTML r�pondant � la s�lection utilisateur
		///		MAJ dans la session de la derni�re page de r�sultats atteinte lors de la navigation
		/// </summary>		
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try{	

				#region Gestion du flash d'attente
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

				#region Url Suivante
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

				#region Textes et Langage du site
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
                
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				#endregion

				#region scripts
                if (!Page.ClientScript.IsClientScriptBlockRegistered("ImageDropDownListScripts")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ImageDropDownListScripts", TNS.AdExpress.Web.Functions.Script.ImageDropDownListScripts(ResultsTableTypesWebControl1.ShowPictures));
                }

				//scripts = TNS.AdExpress.Web.Functions.Script.ImageDropDownListScripts(ResultsTableTypesWebControl1.ShowPictures);
				scriptBody = "javascript:openMenuTest();";
				#endregion								

				#region Option affiner supports ou produits
				if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS){
					InitializeProductWebControl1.Visible = false;
					_webSession.Save();
			
													
				}else if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES){
					InitializeMediaWebcontrol1.Visible = false;
					_webSession.Save();								
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

		#region DeterminePostBackMode
		/// <summary>
		/// Evaluation de l'�v�nement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {

			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			
			ResultsTableTypesWebControl1.CustomerWebSession = _webSession;
			InitializeProductWebControl1.CustomerWebSession=_webSession;
			InitializeMediaWebcontrol1.CustomerWebSession = _webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			
			_genericMediaLevelDetailSelectionWebControl.CustomerWebSession=_webSession;			
			if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS)
				_genericMediaLevelDetailSelectionWebControl.GenericDetailLevelType = WebConstantes.GenericDetailLevel.Type.devicesAnalysis;
			else if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
				_genericMediaLevelDetailSelectionWebControl.GenericDetailLevelType = WebConstantes.GenericDetailLevel.Type.programAnalysis;
			
			resultwebcontrol1.CustomerWebSession = _webSession;
			SponsorshipOptionsWebControl();

			return tmp;
		}
		#endregion

		#region Pr�render
		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
			try{

				#region MAJ _webSession
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

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{
            
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Retrieve next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return MenuWebControl2.NextUrl;
		}
		#endregion

		#region M�thodes priv�es
		/// <summary>
		/// Options des Contr�les fils � afficher dans le contr�le du choix des options du parrainage
		/// </summary>
		private void SponsorshipOptionsWebControl(){
			TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables PreformatedTable = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media;
			Int64 indexPreformatedTable;
			 Int64 sponsorshipListIndex=24;
			//Bouton valider
			if(Request.Form.Get("__EVENTTARGET")=="okImageButton"){
				//Tableau demand�
				indexPreformatedTable = Int64.Parse(Page.Request.Form.GetValues("DDLResultsTableTypesWebControl1")[0]);
				
				PreformatedTable = (TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables) (indexPreformatedTable+sponsorshipListIndex) ;
				_webSession.PreformatedTable = PreformatedTable;
			}
			ResultsOptionsWebControl1.CustomerWebSession = _webSession;	
			switch(_webSession.PreformatedTable){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media :
					ResultsOptionsWebControl1.UnitOption = true;		
					break;	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period :
					ResultsOptionsWebControl1.UnitOption = true;
					break;	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units :
					if(_webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
					_webSession.PercentageAlignment = WebConstantes.Percentage.Alignment.none;
					ResultsOptionsWebControl1.UnitOption = false;
					break;	
			}
				_webSession.Save();
		}
		#endregion
	}
}
