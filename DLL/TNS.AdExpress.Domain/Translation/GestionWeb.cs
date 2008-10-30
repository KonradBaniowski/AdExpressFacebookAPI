#region Informations
// Auteur: G. Facon 
// Date de création: 
// Date de modification: 07/06/2004
//	G. Facon 11/08/2005 Nom de variables 
#endregion

using System;
using System.Collections.Generic;
using AdExpress.Localization;
using TNS.AdExpress.Domain.Web;


namespace TNS.AdExpress.Domain.Translation{


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
		public static string GetWebWord(Int64 code,int langue){
			try{
                return Global.GetValue("w" + code.ToString(), WebApplicationParameters.AllowedLanguages[langue].CultureInfo).ToString();
				//return _list[langue].GetWebWord(code);

			}
			catch(System.Exception){
				// le code langue n'est pas correcte
				return("!");
			}
        }
        #region Old Code
        /*
        /// <summary>
		/// Ensemble des listes de mots
		/// </summary>
		/// <remarks>La clé est le code de la langue</remarks>
		private static Dictionary<int,WordList> _list;
		
		/// <summary>
		/// Constructeur static
		/// </summary>
		static GestionWeb(){
            _list=new Dictionary<int,WordList>();
		}
         
        /// <summary>
		/// Texte à extraire.
		/// Si la function retourne ! c'est que le code langue n'est pas correct
		/// Si la function retourne ? c'est que le code du mots n'existe pas
		/// </summary>
		/// <param name="code">Code du mot à extraire</param>
		/// <param name="langue">Langue souhaitée</param>
		/// <returns></returns>
		public static string GetWebWord(Int64 code,int langue){
			try{
				return _list[langue].GetWebWord(code);

			}
			catch(System.Exception){
				// le code langue n'est pas correcte
				return("!");
			}
		}
        
        /// <summary>
		/// Initialise la classe
		/// </summary>
		public static void Init(){
        }

        #region Accessors
        /// <summary>
        /// Add a word list
        /// </summary>
        public static WordList Add {
            set {

                _list.Add(value.LangueSite,value);
            }
        }*/
        #endregion
    }
}
