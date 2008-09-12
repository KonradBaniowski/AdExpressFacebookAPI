#region Information
/*
 * auteur : D. V.Mussuma
 * créé le :
//date de modification : 30/12/2004  D. Mussuma Intégration de WebPage  
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
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI.Results;

using WebFunctions = TNS.AdExpress.Web.Functions;
#endregion

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Affiche les créations média par parution, au format excel.
	/// </summary>
	public partial class MediaCreationsByParution :  TNS.AdExpress.Web.UI.ExcelWebPage{

//        #region Variables
//        /// <summary>
//        /// Code html du résultat
//        /// </summary>
//        public string result = "";		
//        #endregion

//        #region Evènements

//        #region Chargement de la page
//        /// <summary>
//        /// Chargement de la page:
//        ///		Chargement de la session
//        ///		Initialisation de la connection à la base de données
//        ///		Traduction du site
//        ///		Génération du code HTML:
//        ///			Extraction des paramètres de l'URL : ids ==> [vehicle, category, media]
//        ///			Extraction de zoomDate : Si le paramètre est présent, on effectue les calculs sur la période 
//        ///			en tenant compte du type de période en session. Si zoomDate est absent dans l'URL on tient 
//        ///			compte des périodes présentes en session	
//        /// </summary>
//        /// <remarks>
//        /// Page_Load ==> public static string getMediaParutionCreationsExcelUI(WebSession _webSession, string idVehicle, string idCategory, string idMedia, string period,Page page)
//        /// </remarks>
//        /// <param name="sender">Objet responsable de l'appel de la fonction</param>
//        /// <param name="e">Paramètres</param>
//        protected void Page_Load(object sender, System.EventArgs e) {	
////			try{
////				#region Resultat
////				string[] ids=null;
////				string zoomDate="";
////				bool isMediaDetail=false;
////
////				try{
////					//Récupération du media et du vehicle
////					ids = Page.Request.QueryString.Get("ids").Split(',');
////
////					#region gestion du détail média
////					//gestion du détail média
////					if(ids!=null && ids.Length==5 && ids[5].Equals("1"))
////						isMediaDetail=true;
////					#endregion
////
////					if ((zoomDate=Page.Request.QueryString.Get("zoomDate"))!=null && zoomDate.Length>0){
////						//Détails insertions pour une période de zoom
////						try{
//////							result = MediaInsertionsCreationsResultsUI.GetMediaParutionCreationsExcelUI(_webSession,ids[0],ids[1],ids[2],ids[3],zoomDate, this.Page,isMediaDetail);
////						}
////						catch(System.Exception exc2){
////							Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(959, _webSession.SiteLanguage)+"\n\n"+exc2.Message));
////						}
////					}				
////				}
////				catch(System.Exception){
////					Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
////				}
////				#endregion
////			}		
////			catch(System.Exception exc){
////				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
////					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
////				}
////			}
//        }
//        #endregion

//        #endregion

//        #region Code généré par le Concepteur Web Form
//        /// <summary>
//        /// Initialisation
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        override protected void OnInit(EventArgs e)
//        {
//            //
//            // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
//            //
//            InitializeComponent();
//            base.OnInit(e);
//        }
		
//        /// <summary>
//        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
//        /// le contenu de cette méthode avec l'éditeur de code.
//        /// </summary>
//        private void InitializeComponent()
//        {    
//        }
//        #endregion
	}
}
