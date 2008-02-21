#region Infromation
// Author: Y. R'kaina
// Date of Creation: 24/01/2007
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
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DataAccessFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using FrameWorkConstantes= TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using WebConstantes = TNS.AdExpress.Constantes.Web;

using Dundas.Charting.WebControl;

namespace AdExpress.Private.Results.Jpeg{
	/// <summary>
	/// Description résumée de SectorDataResultsExport.
	/// </summary>
	public partial class SectorDataResultsExport :  TNS.AdExpress.Web.UI.PrivateWebPage{
		
		#region Variables
		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Conteneur des composants destinés à l'APPM.
		/// </summary>
		#endregion

		#region Events

		#region PageLoad
		/// <summary>
		/// Event fired during page load
		/// </summary>
		/// <param name="sender">object sender</param>
		/// <param name="e">EventArgs</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
				#region Flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion	
				
				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.supportPlan||_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.PDVPlan||_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.periodicityPlan){
					sectorDataContainerWebControl1.Source = this._dataSource;
				}
			}		
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns>collections triées de valeurs</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
		{			
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();						
		
			//Conteneur des composants du sector data
			sectorDataContainerWebControl1.CustomerWebSession = _webSession;
			sectorDataContainerWebControl1.ImageType = ChartImageType.Jpeg;
			return tmp;
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
			if(!this.Page.ClientScript.IsClientScriptBlockRegistered("SectorDataResultsExport"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"SectorDataResultsExport",GetJavaScript());
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){
           
		}
		#endregion

		#endregion

		#region GetJavaScript()
		private string GetJavaScript(){
			StringBuilder script=new StringBuilder(2000);

            script.Append("<script language=\"JavaScript\">");
			script.Append("\r\nfunction setWindow(){");
			if((_webSession.Unit==WebConstantes.CustomerSessions.Unit.grp)||(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataSeasonality))
				script.Append("\r\n\twindow.resizeTo(1010,800);");
			else{
				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataPeriodicity)
					script.Append("\r\n\twindow.resizeTo(1010,500);");
				else 
					script.Append("\r\n\twindow.resizeTo(1010,650);");
			}
			script.Append("\r\n}");
			script.Append("\r\n</script>");

			return(script.ToString());
		}
		#endregion
	}
}
