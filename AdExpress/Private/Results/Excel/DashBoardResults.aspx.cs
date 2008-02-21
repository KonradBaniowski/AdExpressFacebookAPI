#region Informations
/*
Auteur : A.Dadouch
Cr�ation : 28/04/2005
*/
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
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Sessions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;

namespace AdExpress.Private.Results.Excel{

	/// <summary>
	/// Export Excel pour le module Tendances
	/// </summary>
	public partial class DashBoardResults : TNS.AdExpress.Web.UI.PrivateWebPage{		

		#region Variables
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";
		 
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DashBoardResults():base(){	
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
				#region S�lection du vehicle
				
				string vehicleSelection=((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();					
				DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.DashBoardDataAccessException("La s�lection de m�dias est incorrecte"));
				#endregion

				#region Calcul du r�sultat	
				result=TNS.AdExpress.Web.BusinessFacade.Results.DashBoardSystem.GetExcel(Page,_webSession);
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
		/// Initialisation
		/// </summary>
		/// <param name="e">Argument</param>
		override protected void OnInit(EventArgs e) {
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
		private void InitializeComponent() {
           
		}
		#endregion
	}
}
