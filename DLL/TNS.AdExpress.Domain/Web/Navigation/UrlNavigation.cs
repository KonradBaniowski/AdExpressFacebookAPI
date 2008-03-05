#region Informations
// Auteur: g. Facon
// Date de cr�ation:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables
//	G. Facon 01/08/2006 Gestion de l'acc�s au information de la page de r�sultat
#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Domain.Web.Navigation {


	///<summary>
	/// Permet de trouver des liens pour la navigation dans le site
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class UrlNavigation {

		/// <summary>
		/// D�termine l'url suivante
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="currentUrl">Url Courrante</param>
		/// <returns>Url</returns>
//		public static string FindNextUrl(WebSession webSession,string currentUrl){
//			// Chargement de la description du module
//			Module currentModuleDescription=TNS.AdExpress.Web.Core.Navigation.ModulesList.GetModule(webSession.CurrentModule);
//			int i=0;
//			foreach(SelectionPageInformation currentPage in currentModuleDescription.SelectionsPages){
//				if(currentPage.Url==currentUrl){
//					break;
//				}
//				i++;
//			}
//			i++;
//			// La page suivante est une s�lection
//			if (i<currentModuleDescription.SelectionsPages.Count)	return(((SelectionPageInformation)currentModuleDescription.SelectionsPages[i]).Url);
//			// C'est une page de r�sultat
//			else return(((ResultPageInformation) currentModuleDescription.GetResultPageInformation(0)).Url);
//		}
	}
}
