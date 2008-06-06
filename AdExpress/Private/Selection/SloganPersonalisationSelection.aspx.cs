#region Informations
// Auteur: D.V Mussuma 
// Date de création: 31/05/2006 
// Date de modification:
//	01/08/2006 Modification FindNextUrl 
#endregion

#region namespace
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
using TNS.AdExpress.Domain.Web.Navigation;

using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantesClassification=TNS.AdExpress.Constantes.Classification.DB;
#endregion

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Classe qui permet d'affiner l'univers de versions client.
	/// </summary>
	public partial class SloganPersonalisationSelection :  TNS.AdExpress.Web.UI.SelectionWebPage{
							
		#region Variables MMI
		/// <summary>
		/// Control de sélection des versions
		/// </summary>
		/// <summary>
		/// Entête de la page
		/// </summary>
		/// <summary>
		/// Titre de la page
		/// </summary>
		/// <summary>
		/// Texte "Sélectionner tous les médias"
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText2;
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Informations
		/// </summary>
		/// <summary>
		/// Bouton de validation 
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Zoom Period
		/// </summary>
		protected string _zoom;

		/// <summary>
		/// Detail Period
		/// </summary>
		protected string _detailPeriod;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public SloganPersonalisationSelection():base(){					
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

			#region Textes et langage du site
			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);			
			ModuleTitleWebControl1.CustomerWebSession = _webSession;
			InformationWebControl1.Language = _webSession.SiteLanguage;
            HeaderWebControl1.Language = _webSession.SiteLanguage;
			//validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
			//validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";			
			#endregion

			#region Rappel des différentes sélections
			ArrayList linkToShow=new ArrayList();			
			if(_webSession.isSelectionProductSelected())linkToShow.Add(2);			
			if(_webSession.isDatesSelected())linkToShow.Add(5);
			#endregion
									
			#region Définition de la page d'aide
//			helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"ReferenceCompetitorPersonalisationHelp.aspx";
			#endregion

			#region Script
			// Cochage/Decochage des checkbox pères, fils et concurrents
			if (!Page.ClientScript.IsClientScriptBlockRegistered("CheckAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"CheckAllChilds",TNS.AdExpress.Web.Functions.Script.CheckAllChilds());
			}
			// Ouverture/fermeture des fenêtres pères
			if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
			}
			// fermer/ouvrir tous les calques
			if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ExpandColapseAllDivs",TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
			}	
			// Sélection de tous les fils
			if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"SelectAllChilds",TNS.AdExpress.Web.Functions.Script.SelectAllChilds());
			}	
						
			#endregion

		}
		#endregion

		#region PréRendu de la page

		/// <summary>
		/// Evènement de PréRendu
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Argument</param>
		protected void Page_PreRender(object sender, System.EventArgs e){
		}
		#endregion

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

		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode(){
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			SloganPersonalisationWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = this._webSession;
			_zoom = Page.Request.QueryString.Get("zoomDate");
			_detailPeriod = Page.Request.QueryString.Get("detailPeriod");

			if (_zoom != null && _zoom.Length > 0)
				SloganPersonalisationWebControl1.ZoomDate = _zoom;
			if (_detailPeriod != null && _detailPeriod.Length > 0) {
				switch ((WebConstantes.CustomerSessions.Period.DisplayLevel)int.Parse(_detailPeriod.Trim())) {
					case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly :
						SloganPersonalisationWebControl1.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateMonth;
						break;
					case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly :
						SloganPersonalisationWebControl1.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
						break;
				}

			}

			return tmp;
		}
		#endregion

		#region Bouton valider
		/// <summary>
		/// Sauvegarde de l'univers des versions sélectionnées
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {	
			
			#region Variables locales
			ArrayList sloganList = new ArrayList();
			int nbSloganSelected=0;
			bool toMuchSlogan=false;
			#endregion

			 
			#region Construction de la liste des versions sélectionnéess		
			sloganList.Clear();
			foreach(ListItem item in this.SloganPersonalisationWebControl1.Items ) {
				if(sloganList==null)sloganList= new ArrayList();
				
				//Ajout de la version sauvegardée
				if (item.Value.IndexOf("slg_") > -1 &&  item.Selected && !sloganList.Contains(Int64.Parse(item.Value.Remove(0,4)))){
					if(nbSloganSelected<1000){
						sloganList.Add(Int64.Parse(item.Value.Remove(0,4)));
					}else{
						toMuchSlogan=true;
						sloganList=null;
						break;
					}
					nbSloganSelected++;
				}
			}

			if(toMuchSlogan){//le nombre maximum d'expressions autorisé dans une liste est de 1000
				
				Response.Write("<script language=javascript>");
				Response.Write(" alert(\""+GestionWeb.GetWebWord(2007,_webSession.SiteLanguage)+"\");");
				Response.Write("</script>");
			}
			#endregion

			//Enregistrement dans la session 
			if(sloganList!=null){
				_webSession.IdSlogans=sloganList;
				_webSession.Save();
				_webSession.Source.Close();

				Response.Redirect(this._nextUrl + "?idSession=" + _webSession.IdSession + ((WithZoomDate(_zoom,0,_nextUrl)) ? "&zoomDate=" + _zoom : ""));
			}

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
			this.validateButton_Click(sender,e);
		}
		/// <summary>
		/// Retrieve next Url from the menu
		/// </summary>
		/// <returns>Next Url</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

		/// <summary>
		/// Check if must use zoom date
		/// </summary>
		/// <param name="zoomDate">zoom Date</param>
		/// <param name="resultid">result page id</param>
		/// <param name="nextUrl">next Url</param>
		/// <returns></returns>
		private bool WithZoomDate(string zoomDate, int resultid, string nextUrl) {
			ResultPageInformation resultPage = _currentModule.GetResultPageInformation(resultid);
			return (resultPage != null && resultPage.Url == nextUrl && zoomDate != null && zoomDate.Length > 0);						
		}

	}
}
