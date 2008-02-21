#region Infromation
// Author: K.Shehzad
// Date of Creation: 10/08/2005
//Date of Modification : 31/07/2006 D. Mussuma integration of appm graph webcontrol
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
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DataAccessFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using FrameWorkConstantes= TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

using Dundas.Charting.WebControl;

namespace AdExpress.Private.Results.Jpeg
{
	/// <summary>
	/// This page is used to generate JPEG for the APPM Charts.
	/// </summary>
	public partial class APPMResultsExport : TNS.AdExpress.Web.UI.WebPage
	{
		#region Variables		
		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
//		/// <summary>
//		/// control graphique pour export d'une  structure de portefeuille
//		/// </summary>
//		protected TNS.AdExpress.Web.UI.Results.APPM.APPMChartUI appmChart;

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
				
					if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.PDVPlan || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.periodicityPlan || _webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.APPM.interestFamily){
						AppmContainerWebControl1.Source = this._dataSource;
//						appmChart.Visible=true;
//						TNS.AdExpress.Web.BusinessFacade.Results.APPMSystem.GetJPEG(this.Page,appmChart,_webSession,this._dataSource);				
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
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {			
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();						
		
			//Conteneur des composants de l'APPM
			AppmContainerWebControl1.CustomerWebSession = _webSession;
			AppmContainerWebControl1.AppmImageType = ChartImageType.Jpeg;
			return tmp;
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Init method of the page
		/// </summary>
		/// <param name="e"></param>
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

		#endregion
	}
}
