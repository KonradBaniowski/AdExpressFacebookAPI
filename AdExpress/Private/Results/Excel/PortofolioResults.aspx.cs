#region Information
//Auteur A.Obermeyer
//date de cr�ation : 08/12/04
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
using TNS.AdExpress.Web.Core.Translation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
#endregion

namespace AdExpress.Private.Results.Excel{

	/// <summary>
	/// Affiche les r�sultats du portefeuille d'un support au format Excel
	/// </summary>
	public partial class PortofolioResults : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : Chargement de la session
		/// </summary>
		public PortofolioResults():base(){			
		}
		#endregion

		#region DeterminePostBackMode
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			_resultWebControl.CustomerWebSession = _webSession;
			return base.DeterminePostBackMode ();
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Resultat
				switch(_webSession.CurrentTab) {
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
						_resultWebControl.Visible = true;
						break;	
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:	
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY:
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
						_resultWebControl.Visible = false;
						result=TNS.AdExpress.Web.BusinessFacade.Results.PortofolioSystem.GetExcel(Page,_webSession);
						break;
					default:					
						break;
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
		private void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Ev�nement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
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
