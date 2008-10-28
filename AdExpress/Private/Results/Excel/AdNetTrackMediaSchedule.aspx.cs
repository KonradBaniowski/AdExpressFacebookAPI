#region Informations
// Auteur: D. Mussuma
// Date de création: 30/03/2007
// Date de modification:
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

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.WebResultUI;

namespace AdExpress.Private.Results.Excel
{
	/// <summary>
	///  Page Excel du Plan Média AdNetTrack 
	/// </summary>
	public partial class AdNetTrackMediaSchedule  : TNS.AdExpress.Web.UI.ExcelWebPage {
	
		/// <summary>
		/// Composant d'affichage des plans médias AdNetTrack
		/// </summary>
	
		#region variables
		/// <summary>
		/// Zoom Date
		/// </summary>
		protected string _zooDate = string.Empty;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public AdNetTrackMediaSchedule():base(){
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){		
			try{

                Response.ContentType = "application/vnd.ms-excel";
				
				#region Chargement des paramètres
				TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type selectionType=(TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type) int.Parse(HttpContext.Current.Request.QueryString.Get("idLevel"));
				Int64 id=Int64.Parse(HttpContext.Current.Request.QueryString.Get("id"));
				Int32 vehicleId=Int32.Parse(HttpContext.Current.Request.QueryString.Get("vehicleId"));

				_zooDate = HttpContext.Current.Request.QueryString.Get("zoomDate");
				if(_zooDate !=null && _zooDate.Length>0)
					AlertAdNetTrackMediaScheduleWebControl1.ZoomDate = _zooDate;
                AlertAdNetTrackMediaScheduleWebControl1.VehicleId = vehicleId;


				_webSession.AdNetTrackSelection=new AdNetTrackProductSelection(selectionType,id);
				#endregion

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
		/// DeterminePostBackMode
		/// </summary>
		/// <returns>DeterminePostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			AlertAdNetTrackMediaScheduleWebControl1.CustomerWebSession =	_webSession;	
			AlertAdNetTrackMediaScheduleWebControl1.OutputType = RenderType.excel;
			

			return(tmp);
		}
		#endregion

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
