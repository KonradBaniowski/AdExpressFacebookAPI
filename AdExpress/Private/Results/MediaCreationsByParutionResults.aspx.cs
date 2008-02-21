#region Information
/*
 * auteur : D. V. Mussuma
 * cr�� le :
 * modifi� le : 27/12/2004
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
	/// MediaInsertionsCreationsResults affiche la liste des insertions d'un vehicule, d'une cat�gorie ou d'un support
	/// suivant les param�tres qu'on lui passe via ids:
	///		ids=-1,-1,10 ==> affichage de la liste de insertions pour le support 10
	///		ids=-1,24,-1 ==> affichage de la liste des insertyions pour la cat�gorie 24
	///		ids=3,-1,-1 ==> affichage de la liste des insertions de la presse
	///	On peut passer un autre param�tre optionnel � la page: zoomDate qui donne une p�riode � couvrir.
	///	Si ce param�tre est absent, on consid�re les dates pr�sentes en session
	/// </summary>
	public partial class MediaCreationsByParutionResults  :  TNS.AdExpress.Web.UI.WebPage{ 
	
		#region Variables		
		/// <summary>
		/// Code html du r�sultat
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
		///		Initialisation de la connection � la base de donn�es
		///		Traduction du site
		///		Extraction du code HTML r�sultat:
		///			Extraction des param�tres pr�sents dans l'URL: ids ==> ["idVehicle","idCategory","idMedia"]
		///			G�n�ration du code HTML suivant qu'une p�riode de zoom est pr�cis�e ou qu'on consid�re les
		///			p�riodes pr�sentes en session
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		/// Utilise les m�thodes:
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
//					#region gestion du d�tail m�dia
//					//gestion du d�tail m�dia
//					if(ids!=null && ids.Length==5 && ids[4].Equals("1"))
//						isMediaDetail=true;
//					#endregion
//
//					if ((zoomDate=Page.Request.QueryString.Get("zoomDate"))!=null && zoomDate.Length>0){
//						//d�tails des insertions pour un zoom sur une  cr�ation
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
	}
}
