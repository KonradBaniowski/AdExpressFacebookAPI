#region Information
/*
 * auteur : D. V. MUSSUMA
 * créé le : 22/12/2004
 * modifié le : 22/12/2004
 * par : D. V. MUSSUMA
 * */
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
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DataAccessFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using FrameWorkConstantes= TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
#endregion

namespace AdExpress.Private.Results.Jpeg{
	/// <summary>
	/// Affiche le graphique du portefeuille d'un support (format jpeg)
	/// </summary>
	public partial class PortofolioResultsExport : TNS.AdExpress.Web.UI.WebPage{	
	
		#region Variables		
		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// control graphique pour export d'une  structure de portefeuille
		/// </summary>
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PortofolioResultsExport():base(){								
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
			try{
				#region Flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion				
			
				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE){
					portofolioChart.Visible=true;
					portofolioChart.StructureChart(_webSession,false);				
				}
			}		
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

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
