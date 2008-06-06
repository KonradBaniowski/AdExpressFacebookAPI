#region Information
//Auteur K.Shehzad
//date de création : 20/07/2005
#endregion
using System;
using System.Windows.Forms;
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

namespace AdExpress.Private.Results.Excel
{
	/// <summary>
	///	This page is used to generate the Excel format for the APPM results.
	/// </summary>
	public partial class APPMResults : TNS.AdExpress.Web.UI.ExcelWebPage
	{
		
		#region Variables
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : Chargement de la session
		/// </summary>
		public APPMResults():base()	{			
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this._dataSource = _webSession.Source;
			try{
                Response.ContentType = "application/vnd.ms-excel";

				#region Connexion à la base de données
				// Ouverture de la base de données
                //if(_webSession.CustomerLogin.Connection!=null){
                //    if(_webSession.CustomerLogin.Connection.State==System.Data.ConnectionState.Closed) _webSession.CustomerLogin.Connection.Open();
                //}
                _webSession.Source.Open();
				#endregion						

				#region chargement de l'univers						
				//_webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser;			
				#endregion

				#region Resultat
				result=TNS.AdExpress.Web.BusinessFacade.Results.APPMSystem.GetExcel(this.Page,_webSession,this._dataSource);
				#endregion	
			}	
			catch(System.Exception exc)
			{
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
				{
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
		private void Page_UnLoad(object sender, System.EventArgs e)
		{			
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// OnInit of the Class
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
