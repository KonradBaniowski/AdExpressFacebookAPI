using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromoPSA.Localization;

namespace KMI.PromoPSA.Web.Domain.Translation {
    ///<summary>
    /// Utilisé les listes de mots dans le site AdExpress
    /// </summary>
    ///  <stereotype>utility</stereotype>
    public class GestionWeb {
        /// <summary>
        /// Texte à extraire.
        /// Si la function retourne ! c'est que le code langue n'est pas correct
        /// Si la function retourne ? c'est que le code du mots n'existe pas
        /// </summary>
        /// <param name="code">Code du mot à extraire</param>
        /// <param name="langue">Langue souhaitée</param>
        /// <returns></returns>
        public static string GetWebWord(Int64 code, int langue) {
            try {
                Global.CurrentCultureInfo = WebApplicationParameters.AllowedLanguages[langue].CultureInfo;
                return Global.GetValue("w" + code.ToString(), WebApplicationParameters.AllowedLanguages[langue].CultureInfo).ToString();

            }
            catch (System.Exception) {
                // le code langue n'est pas correcte
                return ("!");
            }
        }
    }
}
