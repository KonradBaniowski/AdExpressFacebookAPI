#region Informations
// Auteur: G. Facon
// Date de création:
// Date de modification: 06/07/2004
#endregion

using System;
using System.Web.UI;

namespace TNS.AdExpress.Web.Translation.Functions{


	///<summary>
	/// Contient les fonctions de traduction
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class Translate {

		/// <summary>
		/// Change la propriété Language de tous les contrôles de type:
		/// <list type="radio"> 
		/// <item>TNS.AdExpress.Web.Controls.Translation.AdExpressText</item>
		/// <item>TNS.AdExpress.Web.Controls.Headers.HelpWebControl</item>
		/// <item>TNS.AdExpress.Web.Controls.Headers.HeaderWebControl</item>
		/// </list>
		///  de la page.
		/// </summary>
		/// <param name="list">Liste des contrôles de la page</param>
		/// <param name="language">Identifiant de la langue à utiliser</param>
		public static void SetTextLanguage(System.Web.UI.ControlCollection list, int language){
			foreach(Control current in list){
				if(current.GetType()==typeof(TNS.AdExpress.Web.Controls.Translation.AdExpressText)){
					((TNS.AdExpress.Web.Controls.Translation.AdExpressText)current).Language=language;
				}
				else if(current.GetType()==typeof(TNS.AdExpress.Web.Controls.Headers.HelpWebControl)){
					((TNS.AdExpress.Web.Controls.Headers.HelpWebControl)current).Language=language;
				}
				else if(current.GetType()==typeof(TNS.AdExpress.Web.Controls.Headers.HeaderWebControl)){
					((TNS.AdExpress.Web.Controls.Headers.HeaderWebControl)current).Language=language;
				}
			}
		}
	}
}
