using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMI.PromoPSA.Web.Domain.Translation {
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
        public static void SetAllTextLanguage(System.Web.UI.Page page, int language) {
            foreach (System.Web.UI.Control currentList in page.Controls) {
                foreach (System.Web.UI.Control current in currentList.Controls) {
                    if (current is ITranslation) ((ITranslation)current).Language = language;
                }
            }
        }
    }
}
