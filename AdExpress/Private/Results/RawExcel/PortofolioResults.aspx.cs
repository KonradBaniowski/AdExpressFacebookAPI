#region Information
// Auteur A.Obermeyer
// Date de cr�ation : 08/12/04
// Date de modification : 
// 22/06/2005 par B.Masson - Sortie Excel des donn�es brutes
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
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
#endregion

namespace AdExpress.Private.Results.RawExcel{
	/// <summary>
	/// Affiche les r�sultats du portefeuille d'un support au format Excel des donn�es brutes
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

        #region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
        protected void Page_Load(object sender, System.EventArgs e) {
            Response.ContentType = "application/vnd.ms-excel";
        }
        #endregion

        #region DeterminePostBackMode
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
            try
            {
            _resultWebControl.CustomerWebSession = _webSession;

            }		
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
			return base.DeterminePostBackMode ();
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
