#region Informations
// Auteur: A. Obermeyer
// Date de cr�ation: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Int�gration de WebPage
#endregion

using System;
using System.IO;
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
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core;

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Export Excel de l'alerte plan m�dia
	/// </summary>
	public partial class MediaPlanAlertResults : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";
		
		#endregion

		#region Ev�nements

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MediaPlanAlertResults():base(){		
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region R�sultats
				try{
                    //if(_webSession.isCompetitorAdvertiserSelected()){
                    //    result=CompetitorMediaPlanAlertUI.GetMediaPlanAlertExcelUI(CompetitorMediaPlanAlertRules.GetFormattedTable(_webSession, _webSession.PeriodBeginningDate, _webSession.PeriodEndDate),_webSession,_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                    //}
                    //else{
						result=GenericMediaPlanAlertUI.GetMediaPlanAlertWithMediaDetailLevelExcelUI(GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession),_webSession,_webSession.PeriodBeginningDate, _webSession.PeriodEndDate,false);
                    //}
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
		
		#region DeterminePostBack
		/// <summary>
		/// D�termine si postBack est vrai ou non
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			return tmp;
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifi�es pas encore charg�s)
		/// </summary>
		/// <param name="e"></param>
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
