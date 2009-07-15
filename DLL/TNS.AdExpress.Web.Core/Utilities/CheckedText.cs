#region Information
/*
 * Author : G Ragneau
 * Created on : 22/07/2008
 * Modification:
 *      G Ragneau - 22/07/2008 - Move from TNS.AdExpress.Web.Functions
 * 
 * Origine:
 *      Auteur: 
 *      Date de création: 
 */
#endregion

using System;
using System.Text.RegularExpressions;


namespace TNS.AdExpress.Web.Core.Utilities{
	/// <summary>
	/// Classe de tests sur des textes
	/// </summary>
	public class CheckedText{		
		

		/// <summary>
		/// Vérifie la cohérence du texte :
		/// Texte accepté : texte contenant seulement des chiffres, les lettres de l'alphabet
		/// où @
		/// </summary>
		/// <param name="text">Texte Source</param>
		/// <returns>True si le texte suit la syntaxe, false sinon</returns>
		public static bool CheckedProductText(string text){
			Regex r=new Regex("^(?<list>([0-9]|[a-z]|[A-Z]|[ ]|[@]))");
			if(r.Match(text).Success && text.Length>=2)
				return true;
			else{
				return false;
			}	
		}

		/// <summary>
		/// Remplace les caractères suivants :
		/// '," par un espace
		/// ç par c, é è ê par e, à â par a, ô par o et ù par u
		/// </summary>
		/// <param name="text">Texte Source</param>
		/// <returns>Chaîne traitée</returns>
		public static string NewText(string text){
			text=text.TrimEnd();
			text=text.TrimStart();		
			text=Regex.Replace(text,"[']|[\"]"," ");
			text=Regex.Replace(text,"[ç]","c");
			text=Regex.Replace(text,"[é]|[è]|[ê]","e");
			text=Regex.Replace(text,"[à]|[â]","a");
			text=Regex.Replace(text,"[ô]","o");
			text=Regex.Replace(text,"[ù]","u");
			return text;
		}

		/// <summary>
		/// Remplace le caractère suivant nécessaire au bon fonctionnement de la requête :
		/// ' par ''
		/// </summary>
		/// <param name="text">Texte Source</param>
		/// <returns>Chaîne traitée</returns>
		public static string CheckedAccentText(string text){
			text=text.TrimEnd();
			text=text.TrimStart();
			text=Regex.Replace(text,"[']","''");
			return text;
		}
		/// <summary>
		///Verifie si une chaine de caractere est non vide ou non null		
		/// </summary>
		/// <param name="InString">Texte Source</param>
		/// <returns>True si la chaîne est non vide ou non null, false sinon</returns>		
		public static bool IsNotEmpty(string InString){
			return (InString!=null && !InString.Equals(""));				
		}
		/// <summary>
		/// Verifie si une adresse mail est correcte ou pas
		/// </summary>
		/// <param name="mail">l'adresse mail</param>
		/// <returns>True si l'adresse mail est correcte, false sinon</returns>
		public static bool CheckedMailText(string mail) {
			Regex mailRegexp = new Regex(@"^[\w_.~-]+@[\w][\w.\-]*[\w]\.[\w][\w.]*[a-zA-Z]$");
			bool mailOK = mailRegexp.IsMatch(mail);
			if (mailOK)
				return true;
			else
				return false;
		}

        /// <summary>
        /// Vérifie si un tableau d'adresses email est correct ou pas
        /// </summary>
        /// <param name="emails">Le tableau d'adresses email</param>
        /// <returns>True si l'adresse mail est correcte, false sinon</returns>
        public static bool CheckedMailText(string[] emails)
        {
            foreach (string email in emails)
                if (email.Trim().Length != 0 && !CheckedMailText(email))
                    return (false);
            return (true);
        }

	}
}
