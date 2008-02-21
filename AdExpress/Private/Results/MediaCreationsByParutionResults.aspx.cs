#region Information
/*
 * auteur : D. V. Mussuma
 * créé le :
 * modifié le : 27/12/2004
 * par : D. V. Mussuma
 * */
#endregion

#region Namespace
using System;
using System.Collections;
using System.Windows.Forms;
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
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.UI.Results;

using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DataAccessFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
#endregion

namespace AdExpress.Private.Results
{
	/// <summary>
	/// MediaInsertionsCreationsResults affiche la liste des insertions d'un vehicule, d'une catégorie ou d'un support
	/// suivant les paramètres qu'on lui passe via ids:
	///		ids=-1,-1,10 ==> affichage de la liste de insertions pour le support 10
	///		ids=-1,24,-1 ==> affichage de la liste des insertyions pour la catégorie 24
	///		ids=3,-1,-1 ==> affichage de la liste des insertions de la presse
	///	On peut passer un autre paramètre optionnel à la page: zoomDate qui donne une période à couvrir.
	///	Si ce paramètre est absent, on considère les dates présentes en session
	/// </summary>
	public partial class MediaCreationsByParutionResults  :  TNS.AdExpress.Web.UI.WebPage{ 
	
		#region Variables		
		/// <summary>
		/// Code html du résultat
		/// </summary>
		public string result = "";		
		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page:
		///		Chargement de la session
		///		Envoi du flash d'attente
		///		Initialisation de la connection à la base de données
		///		Traduction du site
		///		Extraction du code HTML résultat:
		///			Extraction des paramètres présents dans l'URL: ids ==> ["idVehicle","idCategory","idMedia"]
		///			Génération du code HTML suivant qu'une période de zoom est précisée ou qu'on considère les
		///			périodes présentes en session
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		/// Utilise les méthodes:
		///		public static string LoadingSystem.GetHtmlDiv(int language, Page page)
		///		public static void TNS.AdExpress.Web.DataAccess.Functions.closeDataBase(WebSession _webSession)
		/// </remarks>
		protected void Page_Load(object sender, System.EventArgs e)
		{		
//			try{
//				#region Flash d'attente
//				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
//				Page.Response.Flush();
//				#endregion						
//
//				#region Resultat
//				string[] ids=null;
//				string zoomDate="";
//				//bool isMediaDetail=false;
//
//				try{
//					ids = Page.Request.QueryString.Get("ids").Split(',');
//				
//					#region gestion du détail média
//					//gestion du détail média
//					if(ids!=null && ids.Length==5 && ids[4].Equals("1"))
//						isMediaDetail=true;
//					#endregion
//
//					if ((zoomDate=Page.Request.QueryString.Get("zoomDate"))!=null && zoomDate.Length>0){
//						//détails des insertions pour un zoom sur une  création
//						try{
////							result = MediaInsertionsCreationsResultsUI.GetMediaParutionCreationsResultsUI(_webSession,ids[0], ids[1], ids[2],ids[3], zoomDate, this.Page,isMediaDetail);
//						}
//						catch(System.Exception exc2){
//							DataAccessFunctions.closeDataBase(_webSession);
//							Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(959, _webSession.SiteLanguage)+"\n\n"+exc2.Message));
//						}
//					}				
//				}
//				catch(System.Exception){
//					DataAccessFunctions.closeDataBase(_webSession);
//					Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
//				}
//				#endregion
//			}		
//			catch(System.Exception exc){
//				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
//					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
//				}
//			}
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
