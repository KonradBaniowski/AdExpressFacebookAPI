#region Information
//Auteur A.Obermeyer
//date de cr�ation : 19/10/04
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
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using ExcelFunction = TNS.AdExpress.Web.UI.ExcelWebPage;
#endregion

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Description r�sum�e de IndicatorSeasonalityResults.
	/// </summary>
	public partial class IndicatorSeasonalityResults : TNS.AdExpress.Web.UI.ExcelWebPage{		

		#region variables
		/// <summary>
		/// Code HTML des r�sultats
		/// </summary>
		public string result=""; 
		/// <summary>
		/// Type de l'�l�ment � trier
		/// </summary>
		string itemType="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public IndicatorSeasonalityResults():base(){			
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

                Response.ContentType = "application/vnd.ms-excel";

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);			
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
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            ProductClassContainerWebControl1.WebSession = _webSession;

            return base.DeterminePostBackMode();
        }
        #endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e){
            if (_webSession!=null) _webSession.Source.Close();
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