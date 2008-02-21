#region Information
// Auteur : G Facon
// Créé le : 02/08/2006
// Modifié le : 02/08/2006
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
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExeptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.UI {


	///<summary>
	/// Classe de base pour les pages de sélection
	/// </summary>
	///  <since>02/08/2006</since>
	///  <author>G. Facon</author>
	public class SelectionWebPage : PrivateWebPage {

		#region Constructeur
		///<author>G. Facon</author>
		///  <since>02/08/2006</since>
		///<summary>
		/// Constructeur
		///</summary>
		public SelectionWebPage():base() {
			// Gestion des évènements
			base.Load +=new EventHandler(SelectionWebPage_Load);
			//base.Unload += new EventHandler(WebPage_Unload);
		}
		#endregion

		#region Evènement
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet Source</param>
		/// <param name="e">Arguments</param>
		private void SelectionWebPage_Load(object sender, EventArgs e) {
			#region Url Suivante
			_nextUrl=GetNextUrlFromMenu();
			if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
			else{
				_nextUrlOk=true;
				this.ValidateSelection(null,null);
			}
			#endregion
			
		}


		#endregion

		#region Méthodes 
		/// <summary>
		/// Méthode de validation de la Sélection
		/// </summary>
		/// <param name="sender">Objet Source</param>
		/// <param name="e">Arguments</param>
		protected virtual void ValidateSelection(object sender, System.EventArgs e){
			throw(new NotImplementedException("Doit être implémenté dans l'objet enfant"));
		}

		/// <summary>
		/// Obtient l'url suivante à partir du menu
		/// </summary>
		/// <returns>Url suivante</returns>
		protected virtual string GetNextUrlFromMenu(){
			throw(new NotImplementedException("Doit être implémenté dans l'objet enfant"));
		}

		#endregion
	}
}
