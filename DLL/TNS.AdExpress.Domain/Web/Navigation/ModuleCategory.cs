#region Informations
// Auteur: B.Masson
// Date de création : 26/01/2007
// Date de modification :
#endregion

using System;
using System.Collections;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Classe utilisée dans l'affichage des textes des catégories de modules
	/// </summary>
	public class ModuleCategory:ModuleItem{

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">Identifiant</param>
		/// <param name="idWebText">Code de traduction de la catégorie de modules</param>
		public ModuleCategory(Int64 id, Int64 idWebText):base(id,idWebText,0){
		}
		#endregion

	}
}
