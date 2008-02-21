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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using BusinessFacade=TNS.AdExpress.Web.BusinessFacade.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page des informations du GAD
	/// </summary>
	public partial class Gad : System.Web.UI.Page{

		#region Variables
		///<summary>
		/// WebSession
		/// </summary>
		///  <directed>True</directed>
		///  <supplierCardinality>1</supplierCardinality>
		protected WebSession _webSession;
		/// <summary>
		/// Entier pour la langue du site
		/// </summary>
		public int _siteLanguage;
		/// <summary>
		/// Identifiant de l'adresse passé dans l'URL
		/// </summary>
		protected string idAddress;
		/// <summary>
		/// Nom de l'annonceur
		/// </summary>
		protected string advertiser;
		/// <summary>
		/// Lien internet vers le site du GAD
		/// </summary>
		public string linkGad="";
		/// <summary>
		/// Contact email du GAD
		/// </summary>
		public string emailGad="";
		/// <summary>
		/// Lien internet vers le site du Doc Marketing
		/// </summary>
		public string _docMarketingTarget = string.Empty;
		/// <summary>
		/// Titre de la fenêtre sur le site du Doc Marketing
		/// </summary>
		public string _docMarketingTitle = string.Empty;
		#endregion

		#region Variables MMI
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Label
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		protected System.Web.UI.WebControls.HyperLink _docMarketingLink;
		/// <summary>
		/// Contrôle : Bouton fermer
		/// </summary>
		#endregion
		
		#region Chargement de la page
		protected void Page_Load(object sender, System.EventArgs e){
			

			//Récupération de l'adresse et de l'annonceur
			idAddress = Page.Request.QueryString.Get("idAddress").ToString();
			advertiser = Page.Request.QueryString.Get("advertiser").ToString();

			//Session de l'utilisateur
			_webSession = (WebSession)WebSession.Load(Page.Request.QueryString.Get("idSession"));

			//Langage du site
			_siteLanguage = _webSession.SiteLanguage;
	
			#region Enregistrement Tracking pour le Gad
			if(!IsPostBack)_webSession.OnUseGad();
			#endregion

			try{
				linkGadLabel.Text=GestionWeb.GetWebWord(1137,_siteLanguage);
				linkGad = "http://"+GestionWeb.GetWebWord(1137,_siteLanguage);

				emailGadLabel.Text=GestionWeb.GetWebWord(1138,_siteLanguage);
				emailGad = "mailto:"+GestionWeb.GetWebWord(1138,_siteLanguage);
				
				advertiserLabel.Text = advertiser;

				BusinessFacade.GadSystem result = new BusinessFacade.GadSystem(_webSession,idAddress);
				companyLabel.Text		= result.Company;
				streetLabel.Text		= result.Street;
				street2Label.Text		= result.Street2;
				codePostalLabel.Text	= result.CodePostal;
				townLabel.Text			= result.Town;
				phoneLabel.Text			= result.Phone;
				faxLabel.Text			= result.Fax;
				emailLabel.Text			= result.Email;
				if (result.DocMarketingId.Length > 0){
					_docMarketingTarget = string.Format("<a href=\"javascript:OpenGad('{0}');\" onMouseOver=\"advertiserFile.src=ficheDown.src\" onMouseOut=\"advertiserFile.src=ficheUp.src\"><img title=\"{1}\" border=0 name=\"advertiserFile\" src=\"/Images/{2}/button/bt_fiche_up.gif\"/></a>", 
						//lien
						string.Format(GestionWeb.GetWebWord(2092,_siteLanguage) , result.DocMarketingId, result.DocMarketingKey),
						GestionWeb.GetWebWord(2098,_siteLanguage),
						_siteLanguage
						);
					_docMarketingTitle = string.Format(GestionWeb.GetWebWord(2095 ,_siteLanguage), result.Company).Replace("'", "''");
				}
				else{
					_docMarketingTarget = string.Format("<img title=\"{0}\" border=0 name=\"advertiserFile\" src=\"/Images/"+_siteLanguage+"/button/bt_fiche_off.gif\"/>",
						GestionWeb.GetWebWord(2098,_siteLanguage)); 
				}
			}
			catch(System.Exception et){
				Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(959, _siteLanguage)) + " " + et.Message);
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
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
           
		}
		#endregion

		#region Bouton Fermer
		/// <summary>
		/// Gestion du bouton fermer
		/// </summary>
		/// <param name="sender">Objet Source</param>
		/// <param name="e">Arguments</param>
		private void closeImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			Response.Write("<script language=javascript>");
			Response.Write("	window.close();");
			Response.Write("</script>");
		}
		#endregion
	}
}
