#region Information
//Auteur A.Obermeyer
//date de cr�ation : 
//date de modification : 30/12/2004  D. Mussuma Int�gration de WebPage
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
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
#endregion

namespace AdExpress.Private.Results.Jpeg{
	/// <summary>
	/// Export jpeg 
	/// </summary>
	public partial class IndicatorResultsExport : TNS.AdExpress.Web.UI.PrivateWebPage{	

		#region Variables
		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		#endregion
		
		#region variables MMI
		/// <summary>
		/// Graphique annonceur
		/// </summary>
		/// <summary>
		/// Graphique r�f�rence
		/// </summary>
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){			
			try{
				#region Flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion

				//Graphique palmares
				if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES){
					advertiserChart.PalmaresBar(_webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear,FrameWorkConstantes.PalmaresRecap.ElementType.advertiser,false);
					referenceChart.PalmaresBar(_webSession,FrameWorkConstantes.PalmaresRecap.typeYearSelected.currentYear,FrameWorkConstantes.PalmaresRecap.ElementType.product,false);
				}
					//Graphique saisonnalit�
				else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY){
					referenceChart.Visible=false;
					advertiserChart.SeasonalityLine(_webSession,true,false);
				}
					//Graphique �volution
				else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION){
					advertiserChart.EvolutionBar(_webSession,FrameWorkConstantes.EvolutionRecap.ElementType.advertiser,false);
					referenceChart.EvolutionBar(_webSession,FrameWorkConstantes.EvolutionRecap.ElementType.product,false);
				}
					//Graphique Strat�gie m�dia
				else if(_webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY){
					referenceChart.Visible=false;
					advertiserChart.MediaStrategyBar(_webSession,false);			
				}
			}			
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
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
            
		}
		#endregion
	}
}
