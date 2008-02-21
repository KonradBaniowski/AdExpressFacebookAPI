#region Information
 // Auteur : G Facon
 // Création: 12/08/2005
 // Modification:
#endregion

using System;
using System.Web.UI;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.UI.Global.Loading;

namespace TNS.AdExpress.Web.BusinessFacade.Global.Loading{
	/// <summary>
	/// Accès au code du chargement de la page (Loading flash)
	/// </summary>
	public class LoadingSystem{
		/// <summary>
		/// Enregistre le code flash dans la page et renvoie le code html d'affichage
		/// </summary>
		/// <param name="languageId">Langue du flash</param>
		/// <param name="page">Page recevant le code</param>
		/// <returns>Code HTML généré</returns>
		public static string GetHtmlDiv(int languageId, Page page){
			if(page==null)throw(new ArgumentNullException("La Page est null"));
			try{
				return(LoadingUI.GetHtmlDiv(languageId,page));
			}
			catch(System.Exception err){
				throw(new LoadingSystemException("Impossible de générer le code pour le flash d'attente",err));
			}
		}

		/// <summary>
		/// Construit le script de fermeture du flash d'attente
		/// </summary>
		/// <returns>Code html de la fermeture du flash</returns>
		public static string GetHtmlCloseDiv(){
			return(LoadingUI.GetHtmlCloseDiv());
		}

	}
}
