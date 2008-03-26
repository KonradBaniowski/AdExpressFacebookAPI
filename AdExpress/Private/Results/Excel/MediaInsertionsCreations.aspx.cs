#region Information
/*
 * auteur : Guillaume Ragneau
 * cr�� le :
 * modifi� le : 22/07/2004
 * par : Guillaume Ragneau
 *   19/12/2004 A. Obermeyer Int�gration de WebPage
 * */
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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ConstantesWeb=TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.Results.Excel{

	/// <summary>
	/// G�n�re une page Excel donnant le d�tail des insertions d'un support, d'une cat�gorie ou d'un m�dia
	/// </summary>
	public partial class MediaInsertionsCreations : TNS.AdExpress.Web.UI.PrivateWebPage{
	
		#region Variables
		/// <summary>
		/// Code html du r�sultat
		/// </summary>
		public string result = "";	
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page:
		///		Chargement de la session
		///		Initialisation de la connection � la base de donn�es
		///		Traduction du site
		///		G�n�ration du code HTML:
		///			Extraction des param�tres de l'URL : ids ==> [vehicle, category, media]
		///			Extraction de zoomDate : Si le param�tre est pr�sent, on effectue les calculs sur la p�riode 
		///			en tenant compte du type de p�riode en session. Si zoomDate est absent dans l'URL on tient 
		///			compte des p�riodes pr�sentes en session	
		/// </summary>
		/// <remarks>
		/// Page_Load ==> public static string getMediaInsertionsCreationsExcelUI(WebSession _webSession, string idVehicle, string idCategory, string idMedia, string period,Page page)
		/// Page_Load ==> public static string getMediaInsertionsCreationsExcelUI(WebSession _webSession, string idVehicle, string idCategory, string idMedia, Page page)
		/// </remarks>
		/// <param name="sender">Objet responsable de l'appel de la fonction</param>
		/// <param name="e">Param�tres</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Resultat
				string[] ids=null;
				string zoomDate="";
                //bool isMediaDetail=false;
				string idVehicleFromTab="-1";

				try{

                    Response.ContentType = "application/vnd.ms-excel";

					//R�cup�ration du media et du vehicule
					ids = Page.Request.QueryString.Get("ids").Split(',');
					idVehicleFromTab =Page.Request.QueryString.Get("idVehicleFromTab");
					zoomDate =  Page.Request.QueryString.Get("zoomDate");
					if(idVehicleFromTab==null || idVehicleFromTab.Equals(""))idVehicleFromTab="-1";

					#region gestion du d�tail m�dia
					//gestion du d�tail m�dia
//					if(ids!=null && ids.Length==6 && ids[5].Equals("1"))
//						isMediaDetail=true;

					if(_webSession.CurrentModule==ConstantesWeb.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE){
						_webSession.PreformatedMediaDetail = ConstantesWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia;
					}
					#endregion

					try{
						result = MediaInsertionsCreationsResultsUI.GetMediaInsertionsCreationsExcelUI(_webSession,ids[0],ids[1],ids[2],ids[3],ids[4],zoomDate,this.Page,ref idVehicleFromTab);
					}
					catch(System.Exception exc2){
						Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(959, _webSession.SiteLanguage)+"\n\n"+exc2.Message));
					}
				}
				catch(System.Exception){
					Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
				}
				#endregion
		
            }
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){
			
		}
		#endregion

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
           
            this.Unload += new System.EventHandler(this.Page_UnLoad);
		}
		#endregion
	}
}
