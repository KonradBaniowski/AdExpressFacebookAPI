#region Information
//Auteur D. Mussuma
//date de création : 24/08/2006
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

using TNS.AdExpress.Web.BusinessFacade.Results;


namespace AdExpress.Private.Results.Excel
{
	/// <summary>
	/// Affiche la synthèse d'une campagne par version au format excel
	/// </summary>	
	public partial class APPMVersionSynthesis : TNS.AdExpress.Web.UI.ExcelWebPage 
	{
		#region Variables
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string _idSession="";		
		/// <summary>
		/// Version ID to study
		/// </summary>
		private string _idVersion="";
		/// <summary>
		/// Date de la 1ere insertion de la version
		/// </summary>
		private string _firstInsertionDate="";		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : Chargement de la session
		/// </summary>
		public APPMVersionSynthesis():base()	{			
	}
		#endregion
		

		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			_idVersion = Page.Request.QueryString.Get("idVersion");
			_firstInsertionDate = Page.Request.QueryString.Get("firstInsertionDate");

			result = APPMSystem.GetVersionSynthesisHtml(_dataSource, _webSession,_idVersion,_firstInsertionDate,true);
		}

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
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
           
		}
		#endregion
	}
}
