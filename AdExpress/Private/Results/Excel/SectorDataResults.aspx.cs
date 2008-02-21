#region Information
//Auteur Y.R'kaina
//date de cr�ation : 16/01/2007
#endregion

using System;
using System.Windows.Forms;
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

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// This page is used to generate the Excel format for the Sector Data results.
	/// </summary>
	public partial class SectorDataResults : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables
		/// <summary>
		/// Conteneur des composants destin�s au donn�es de cadrage.
		/// </summary>
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : Chargement de la session
		/// </summary>
		public SectorDataResults():base(){			
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

		#region DetermeinePostBack
		/// <summary>
		/// DeterminePostBackMode
		/// </summary>
		/// <returns>PostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode(){
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			SectorDataContainerWebControl1.CustomerWebSession=_webSession;
			return tmp;
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
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
