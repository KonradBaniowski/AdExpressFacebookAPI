#region Informations
// Auteur: G. Facon 
// Date de création: 13/07/2006
// Date de modification:
#endregion

using System;

namespace TNS.AdExpress.Web.Common.Results{
	///<summary>
	/// Données servant à construire un Résultat (UI)
	/// </summary>
	///  <since>13/07/2006</since>
	///  <author>G. Facon</author>
	public class ResultData{

		#region Variables
		///<summary>
		/// Code HTML du calendrier d'action
		/// </summary>
		///  <since>13/07/2006</since>
		///  <author>G. Facon</author>
		private string _mediaPlanHTMLCode="";
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		///  <since>13/07/2006</since>
		///  <author>G. Facon</author>
		public ResultData(){		
		}
		#endregion

		#region Accesseurs
		///<summary>
		/// Obtient ou définit le code HTML du calendrier d'action
		/// </summary>
		///  <since>13/07/2006</since>
		///  <author>G. Facon</author>
		public string HTMLCode {
			get{return(_mediaPlanHTMLCode);}
			set{_mediaPlanHTMLCode=value;}
		}
		#endregion
	}
}
