#region Informations
// Auteur: G. Facon 
// Date de création: 
// Date de modification: 07/06/2004
//	G. Facon 11/08/2005 Nom de variables 
#endregion

using System;
using System.Collections;


namespace TNS.AdExpress.Web.Core.Translation{


	///<summary>
	/// Utilisé les listes de mots dans le site AdExpress
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class GestionWeb {
		/// <summary>
		/// Ensemble des listes de mots
		/// </summary>
		/// <remarks>La clé est le code de la langue</remarks>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.Translation.WordList</associates>
		private static System.Collections.Hashtable _list;
		
		/// <summary>
		/// Constructeur static
		/// </summary>
		static GestionWeb(){
			_list=new Hashtable();
			try{
				// Création de la liste de mots en français
				_list.Add(33,new WordList(33));
				// Création de la liste de mots en anglais
				_list.Add(44,new WordList(44));
			}
			catch(System.Exception err){
				throw(err);	
			}
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
				return (((WordList)_list[langue]).GetWebWord(code));
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
	}
}
