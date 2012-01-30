#region Informations
// Auteur: G.Ragneau
// Date de création: 15/09/2004
// Date de modification:  30/12/2004  D. Mussuma Intégration de WebPage
//	01/08/2006 Modification FindNextUrl
#endregion

#region Namespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
#endregion

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de sélection des annonceurs dans une analyse sectorielle
	/// </summary>
	public partial class ReferenceCompetitorPersonalisation : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Variables MMI
		/// <summary>
		/// Texte "Module X"
		/// </summary>
		/// <summary>
		/// Entete flash et menu
		/// </summary>
		/// <summary>
		/// Control de sélection des annonceurs
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		#endregion

		#region Variables
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose="";
		/// <summary>
		/// Script de message d'erreur
		/// </summary>
		public string errorScript="";
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// Code html bouton precedent
		/// </summary>
		public string previousButton="";		
		#endregion
			
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public ReferenceCompetitorPersonalisation():base(){			
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
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=MenuWebControl2.ID && (nomInput!=validateButton.ID || validateButton.CommandName!="validate")){
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
						divClose = LoadingSystem.GetHtmlCloseDiv();
					}
				}
				else{
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
					divClose = LoadingSystem.GetHtmlCloseDiv();
				}
				#endregion

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);							
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;		
				validateButton.ImageUrl="/App_Themes/"+this.Theme+"/Images/Culture/button/suivant_up.gif";
				validateButton.RollOverImageUrl="/App_Themes/"+this.Theme+"/Images/Culture/button/suivant_down.gif";
				validateButton.ToolTip=GestionWeb.GetWebWord(1098,_webSession.SiteLanguage);
				recapAdvertiserSelectionWebControl.Title = GestionWeb.GetWebWord(1122,_webSession.SiteLanguage);
				#endregion

				#region Url Suivante
//				_nextUrl=this.recallWebControl.NextUrl;
//				if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
//				else validateButton_Click(validateButton, null);
				#endregion

				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"ReferenceCompetitorPersonalisationHelp.aspx";
				#endregion

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
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion
		
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation de la page 
		/// </summary>
		/// <param name="e">Arguments</param>
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

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() 
		{

			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
            try{
//			recallWebControl.CustomerWebSession=_webSession;
			recapAdvertiserSelectionWebControl.WebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			MenuWebControl2.ForbidRecall = true;
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

		#region Bouton Valider
		protected void validateButton_Click(object sender, System.EventArgs e) {

              try{
			string errorText=string.Empty;
			List<long> advertisers = null, savedAdvertisers = null;
			int nbSavedAdvertisers = 0;
			NomenclatureElementsGroup nomenclatureElementsGroup = null;
			AdExpressUniverse adExpressUniverse = null;
			Dictionary<int, AdExpressUniverse> universeDictionary = null;
			string saveAdvertisersString = null;
			//COmmentaires à conserver en vue d'une amélioration ==> recocher les cases déjà cochées en cas d'erreur			
			//_webSession.CurrentUniversAdvertiser=TreeBuilding();
			advertisers = GetAdvertiserSelection();
			TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl button = (TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl)sender;
			if(button.CommandName=="next"){
				_webSession.SecondaryProductUniverses.Clear();
				//Action next ==> enregistrement dans reference
				//if(_webSession.CurrentUniversAdvertiser.Nodes.Count<51){
				if(advertisers.Count<51){
					//_webSession.ReferenceUniversAdvertiser=_webSession.CurrentUniversAdvertiser;
					if(advertisers.Count>0){
						nomenclatureElementsGroup = new NomenclatureElementsGroup(0, AccessType.includes);
						nomenclatureElementsGroup.AddItems(TNSClassificationLevels.ADVERTISER, advertisers);
						adExpressUniverse = new AdExpressUniverse(Dimension.product);
						adExpressUniverse.AddGroup(0,nomenclatureElementsGroup);
						universeDictionary = new Dictionary<int, AdExpressUniverse>();
						universeDictionary.Add(0, adExpressUniverse);
						_webSession.SecondaryProductUniverses = universeDictionary;
						
					}
					this.recapAdvertiserSelectionWebControl.ExceptionsList = (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) ? nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER) : "";
					//this.recapAdvertiserSelectionWebControl.ExceptionsList = _webSession.GetSelection(_webSession.ReferenceUniversAdvertiser,CstWebCustomer.Right.type.advertiserAccess);
					NextToValidate();						
				}
				else{
					this.recapAdvertiserSelectionWebControl.ReCheck=true;
					errorText=GestionWeb.GetWebWord(1193, _webSession.SiteLanguage);
					errorText=errorText.Replace("'"," ");
					errorScript="javascript:alert('"+errorText+"');";
				}
			}
			else{
				if(_webSession.SecondaryProductUniverses !=null && _webSession.SecondaryProductUniverses.Count>0 ){
					nomenclatureElementsGroup = _webSession.SecondaryProductUniverses[0].GetGroup(0);
					if(nomenclatureElementsGroup != null){
						savedAdvertisers = nomenclatureElementsGroup.Get(TNSClassificationLevels.ADVERTISER);
						saveAdvertisersString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
						if (savedAdvertisers != null && savedAdvertisers.Count > 0) nbSavedAdvertisers = savedAdvertisers.Count;
					}
				}
				nbSavedAdvertisers = nbSavedAdvertisers + advertisers.Count;

				
				if(nbSavedAdvertisers<51){
					
					if (advertisers.Count > 0) {
						nomenclatureElementsGroup = new NomenclatureElementsGroup(0, AccessType.includes);
						nomenclatureElementsGroup.AddItems(TNSClassificationLevels.ADVERTISER, advertisers);
						
						adExpressUniverse = new AdExpressUniverse(Dimension.product);
						adExpressUniverse.AddGroup(0, nomenclatureElementsGroup);

						if (_webSession.SecondaryProductUniverses.Count == 0) {
							universeDictionary = new Dictionary<int, AdExpressUniverse>();
							universeDictionary.Add(1, adExpressUniverse);
							_webSession.SecondaryProductUniverses = universeDictionary;
						}
						else {
							_webSession.SecondaryProductUniverses.Add(1, adExpressUniverse);
						}
					}
					//redirection
					_webSession.Save();
					_webSession.Source.Close();
					Response.Redirect(this._nextUrl+"?idSession="+_webSession.IdSession);
				}
				else{
					NextToValidate();
					errorText=GestionWeb.GetWebWord(1193, _webSession.SiteLanguage);
					errorText=errorText.Replace("'"," ");
					if (saveAdvertisersString != null && saveAdvertisersString.Length > 0)
						this.recapAdvertiserSelectionWebControl.ExceptionsList = saveAdvertisersString;
					errorScript="javascript:alert('"+errorText+"');";
				}
			}
			_webSession.Save();
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

		#region Construction de l'arbre
		/// <summary>
		/// Fonction qui construit un arbre à partir des éléments sélectionnés dans le webControl
		/// recapAdvertiserSelectionWebControl
		/// </summary>
		/// <returns></returns>
		private System.Windows.Forms.TreeNode TreeBuilding(){
			System.Windows.Forms.TreeNode advertisers = new System.Windows.Forms.TreeNode("advertiser");
			System.Windows.Forms.TreeNode tmp;
			foreach(ListItem item in this.recapAdvertiserSelectionWebControl.Items){
				if (item.Selected){
					tmp = new System.Windows.Forms.TreeNode(item.Text);
					tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.advertiserAccess,Int64.Parse(item.Value),item.Text);
					tmp.Checked = true;
					advertisers.Nodes.Add(tmp);
				}
			}
			return advertisers;
		}
		#endregion

		#region Get advertiser selection
		/// <summary>
		/// Get advertisers selected
		/// </summary>
		/// <returns>List of advertisers selected</returns>
		private List<long> GetAdvertiserSelection() {
			List<long> advertisers = new List<long>();
			foreach (ListItem item in this.recapAdvertiserSelectionWebControl.Items) {
				if (item.Selected) {					
					advertisers.Add(Int64.Parse(item.Value));
				}
			}
			return advertisers;
		}
		#endregion

        #region NextToValidate
		/// <summary>
		/// Effectue les opérations nécessaires pour passer de la page type "reference" a la page type "concurrent"
		/// (bouton suivvant==>bouton valider, liste des annonceurs selectionnés...)
		/// </summary>
		private void NextToValidate(){
			//Changer le bouton next en valider
			validateButton.CommandName="validate";
			validateButton.ToolTip=GestionWeb.GetWebWord(1099,_webSession.SiteLanguage);
			validateButton.ImageUrl="/App_Themes/"+this.Theme+"/Images/Culture/button/valider_up.gif";
			validateButton.RollOverImageUrl="/App_Themes/"+this.Theme+"/Images/Culture/button/valider_down.gif";
			//Description de l apage
			ModuleTitleWebControl1.CodeDescription=1094;
			recapAdvertiserSelectionWebControl.Title = GestionWeb.GetWebWord(1123,_webSession.SiteLanguage);
			//Bouton precedent
			previousButton="<a href=\"#\" onclick=\"javascript:history.back();\" onmouseover=\"bouton.src='/App_Themes/"+this.Theme+"/Images/Culture/button/back_down.gif';\" onmouseout=\"bouton.src = '/App_Themes/"+this.Theme+"/Images/Culture/button/back_up.gif';\">";
			previousButton+="<img src=\"/App_Themes/"+this.Theme+"/Images/Culture/button/back_up.gif\" border=0 name=bouton></a>&nbsp;";
			MenuWebControl2.ForbidRecall = false;
		}
		#endregion

		#endregion

		#region Implémentation méthodes abstarites
		/// <summary>
		/// Event launch to fire validation of the page
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.validateButton_Click(validateButton,e);
		}
		/// <summary>
		/// Retrieve next Url from the menu
		/// </summary>
		/// <returns>Next Url</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

	}
}
