#region Informations
// Auteur: G. Facon
// Date de création: 25/10/2005
// Date de modification: 
#endregion

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
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Domain.Web.Navigation;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using AdExpressException = TNS.AdExpress.Exceptions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Common.Results;
using TNS.FrameWork.Exceptions;

using TNS.AdExpressI.MediaSchedule;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page de calendrier d'action d'un plan media
	/// </summary>
    public partial class CreativeMediaPlanResults : TNS.AdExpress.Web.UI.BaseResultWebPage {
		
		#region Variables
		/// <summary>
		/// Identifiant Session
		/// </summary>
		public string idsession;
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";		
		/// <summary>
		/// Liste d'annonceurs
		/// </summary>
		protected string listAdvertiser="";
		/// <summary>
		/// Script qui gère la sélection des annonceurs
		/// </summary>
		public string advertiserScript;
		/// <summary>
		/// Texte de l'option "Tout sélectionner"
		/// </summary>
		public string allChecked;		
//		/// <summary>
//		/// Script de fermeture du flash d'attente
//		/// </summary>
		//public string divClose=LoadingSystem.GetHtmlCloseDiv();		
		/// <summary>
		/// Capture de l'évènement responsbale du postBack
		/// </summary>
		protected int  eventButton=0;
		#endregion

		#region Variable MMI
		/// <summary>
		/// Contrôle de texte "Votre sélection annonceurs"
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		/// <summary>
		/// Rappel de la sélection
		/// </summary>
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CreativeMediaPlanResults():base(){
			idsession=HttpContext.Current.Request.QueryString.Get("idSession");
            this._useThemes = false;
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
		
            try{
		
				#region Flash d'attente
//				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
//					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
//				}
//				else{
//					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
//					Page.Response.Flush();
//				}
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
			
				#region Texte et langage du site
				creativeSelectionWebControl.CustomerSession=_webSession;
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				advertiserScript= TNS.AdExpress.Web.Functions.DisplayTreeNode.AddScript();
				allChecked=GestionWeb.GetWebWord(856,_webSession.SiteLanguage);			
				#endregion
	

				#region MAJ de la Session
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.Save();
				#endregion
			
				#region Gestion du Contrôle advertiserSelection
				//Bouton valider
				if(Request.Form.Get("__EVENTTARGET")=="validSubSelectionImageButtonRollOverWebControl"){
					eventButton=7;
				}
				else if(Request.Form.Get("__EVENTTARGET")=="firstSubSelectionLinkButton"){
					eventButton=8;
				}
				else{
					eventButton=9;
				}

				#endregion

				#region Calcul du résultat
				if(eventButton==9){
                    object[,] tab = null;
                    MediaSchedulePeriod period = null;
                    //MediaPlanResultData resultTmp = null;
                    MediaScheduleData resultTmp = null;


                    period = new MediaSchedulePeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, _webSession.DetailPeriod);

                    //tab = TNS.AdExpress.Web.Rules.Results.GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1);

                    //if (_webSession.IdSlogans != null && _webSession.IdSlogans.Count > 0 && tab.GetLength(0) == 0) {
                    //    _webSession.IdSlogans = new ArrayList();
                    //    tab = TNS.AdExpress.Web.Rules.Results.GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1);
                    //}

                    //resultTmp = TNS.AdExpress.Web.UI.Results.GenericMediaScheduleUI.GetHtml(tab, _webSession, period, true);
                    TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);
                    if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Media Schedule result"));
                    object[] param = new object[2];
                    param[0] = _webSession;
                    param[1] = period;
                    IMediaScheduleResults mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                    resultTmp = mediaScheduleResult.GetHtmlCreativeDivision();

                    /* When we don't have a media schedule for a list of versions we show the message : 'no result for this selection'
                     * in the old version we switched to the product media schedule
                     * */
                    //if (resultTmp.HTMLCode.Length <= 0)
                    //{
                    //    _webSession.IdSlogans = new ArrayList();
                    //    resultTmp = mediaScheduleResult.GetHtmlCreativeDivision();
                    //}

                    result += "<table align=\"center\" cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">";
                    result += "\r\n\t<tr height=\"1\">\r\n\t\t<td>";
                    result += "\r\n\t\t</td>\r\n\t</tr>";
                    result += "\r\n\t<tr>\r\n\t\t<td>";

                    if (resultTmp.HTMLCode.Length > 0)
                    {
                        result += resultTmp.HTMLCode;
                    }
                    else
                    {
                        result += "<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, _webSession.SiteLanguage) + "</div>";
                    }

                    result += "\r\n\t\t</td>\r\n\t</tr>";
                    result += "</table>";
				}
				#endregion				
			
				#region Script
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}

				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}

				// Sélection/désélection de tous les fils (cas 2 niveau)
				if (!Page.ClientScript.IsClientScriptBlockRegistered("AllSelection")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllSelection",TNS.AdExpress.Web.Functions.Script.AllSelection());
				}
				// Sélection de tous les fils
				if (!Page.ClientScript.IsClientScriptBlockRegistered("Integration")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"Integration",TNS.AdExpress.Web.Functions.Script.Integration());
				}
				// Sélection de tous les éléments (cas 1 niveau)
				if (!Page.ClientScript.IsClientScriptBlockRegistered("AllLevelSelection")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"AllLevelSelection",TNS.AdExpress.Web.Functions.Script.AllLevelSelection());
				}	
				#endregion				
				
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    WebFunctions.CreativeErrorTreatment.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession), Page, _webSession.SiteLanguage);
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
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region Initialisation
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){    
			this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion
		
		#region DeterminePostBack
		/// <summary>
		/// Détermine la valeur de PostBack
		/// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			return tmp;
		}
		#endregion

		#region Valider une sous-sélection d'annonceur
		/// <summary>
		/// Bouton valider au niveau de la sous sélection d'annonceurs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void validSubSelectionImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		}
		#endregion

		#region Retour à la sélection originale
		/// <summary>
		/// Gestion du lien retour sélection initiale
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void firstSubSelectionLinkButton_Click(object sender, System.EventArgs e) {
			try{
				//Copie de l'arbre globale dans le le current
				_webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode)_webSession.SelectionUniversAdvertiser.Clone();
				//Affichage du résultat
				result=MediaPlanAnalysisUI.GetMediaPlanAnalysisHtmlUI(this,MediaPlanAnalysisRules.GetFormattedTable(_webSession),_webSession);
				_webSession.Save();
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRendu De la page
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e){
			try{
				if (IsPostBack){
					if(eventButton==7){	
						if(_webSession.CurrentUniversAdvertiser.FirstNode!=null){
							_webSession.Save();
							// Calcul du résultat
							result=MediaPlanAnalysisUI.GetMediaPlanAnalysisHtmlUI(this,MediaPlanAnalysisRules.GetFormattedTable(_webSession),_webSession);
						}else{
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\""+GestionWeb.GetWebWord(878,_webSession.SiteLanguage)+"\");");					
							Response.Write("</script>");
						}
					}
				}			
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#endregion

		#region Abstract Methods
		/// <summary>
		/// Retrieve next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return ("");
		}
		#endregion

	}
}
