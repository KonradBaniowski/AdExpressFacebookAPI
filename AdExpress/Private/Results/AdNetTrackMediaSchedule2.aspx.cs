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
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.Core.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.DB.Common;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Description résumée de AdNetTrackMediaSchedule2.
	/// </summary>
	public partial class AdNetTrackMediaSchedule2 : TNS.AdExpress.Web.UI.WebPage{

		public string _result="";


		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public AdNetTrackMediaSchedule2():base(){
		}
		#endregion
	
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				_result = TNS.AdExpress.Web.UI.ExcelWebPage.GetExcelHeaderForAdnettrackMediaPlanPopUp(_webSession,false,_webSession.PeriodBeginningDate,_webSession.PeriodEndDate);
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}

		#region DeterminePostBackMode
		/// <summary>
		/// DeterminePostBackMode
		/// </summary>
		/// <returns>DeterminePostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			OptionLayerWebControl1.CustomerWebSession = _webSession;	
			MenuWebControl1.CustomerWebSession = _webSession;
			return(tmp);
		}
		#endregion

		#region Code généré par le Concepteur Web Form
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

		#region Prérender
		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
			try{
				#region MAJ _webSession
				_webSession.Save();
				#endregion

				MenuWebControl1.ForbidHelpPages = true;
				//MenuWebControl1.ForceHelp = "/Private/Results/Helps/.../ .aspx?idSession="+_webSession.IdSession;
				MenuWebControl1.ForcePrint = "#";
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion
	}
}
