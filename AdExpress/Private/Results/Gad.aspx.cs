using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using BusinessFacade=TNS.AdExpress.Web.BusinessFacade.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page des informations du GAD
	/// </summary>
    public partial class Gad : TNS.AdExpress.Web.UI.PrivateWebPage {

		#region Variables
        /////<summary>
        ///// WebSession
        ///// </summary>
        /////  <directed>True</directed>
        /////  <supplierCardinality>1</supplierCardinality>
        //protected WebSession _webSession;
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
		/// Texte
		/// </summary>
		protected System.Web.UI.WebControls.HyperLink _docMarketingLink;
		/// <summary>
		/// Contrôle : Bouton fermer
		/// </summary>
		#endregion

		public Gad()
		{
          

		}
		
		#region Chargement de la page
        /// <summary>
        /// Page load
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Event Args</param>
		protected void Page_Load(object sender, System.EventArgs e){
						

			#region Enregistrement Tracking pour le Gad
			if(!IsPostBack)_webSession.OnUseGad();
			#endregion

			
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

        #region DeterminePostBackMode
        /// <summary>
        /// Evaluation de l'évènement PostBack:
        ///		base.DeterminePostBackMode();
        ///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            gadWebControl1.CustomerWebSession = _webSession;
            
            return tmp;
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
