#region Information
/*
 * Author : 
 * Created on : 
 * Modification:
 *      G Ragneau - 22/07/2008 - Move to TNS.AdEXpress.Web.Core.Utilities
 * 
 * Origine:
 *      Auteur: 
 *      Date de création: 
 */
#endregion



namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Classe de tests sur des textes
	/// </summary>
    public class CheckedText : TNS.AdExpress.Web.Core.Utilities.CheckedText
    {
        /// <summary>
        ///Verifie si une chaine de caractere est non vide ou non null		
        /// </summary>
        /// <param name="InString">Texte Source</param>
        /// <returns>True si la chaîne est non vide ou non null, false sinon</returns>		
        public static bool IsStringEmpty(string InString)
        {
            return IsNotEmpty(InString);
        }

	}
}
