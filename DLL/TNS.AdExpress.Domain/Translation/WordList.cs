#region Informations
// Auteur: G. Facon
// Date de cr�ation:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables
#endregion


using System;
using System.Data;
using TNS.AdExpress.Domain.Exceptions;


namespace TNS.AdExpress.Domain.Translation{
	/// <summary>
	/// Classe g�rant les textes du site Web AdExpress
	/// </summary>
	public class WordList{

		#region Variables
		/// <summary>
		/// Identifiant de la langue
		/// </summary>
		private int _idLanguage;
		/// <summary>
		/// Liste des mots
		/// </summary>
		private string[] _list;
		/// <summary>
		/// Identifiant maximum de id_web_text
		/// </summary>
		private int _max;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="langueSite">Identifiant de la langue du site AdExpress</param>
        /// : base(WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.translation)) 
        public WordList(int langueSite,string[] list){
            if(list==null) throw (new ArgumentNullException("la liste de mots est null pour la langue:"+langueSite));
            _idLanguage=langueSite;
            _list=list;
			_max=_list.Length;
		}
		#endregion

		#region Recuperation Texte
		/// <summary>
		/// Donne le texte ayant pour identifiant idText
		/// </summary>
		/// <param name="idText">Identifiant du texte recherch�</param>
		/// <returns>Texte demand�</returns>
		public string getWord(Int64 idText){
			//Le texte est en dehors des limites du tableau
			if (idText<1 || idText>_max){
				throw(new TranslationException("Index invalide car idText ("+idText+") est inf�rieur � 1 ou sup�rieur � "+_max.ToString()));
			}
			string text=_list[idText];
			//Le texte n'a pas �t� entr� dans la base de donn�es
			if (text==null){
				throw(new TranslationException("Le Texte dont l'identifiant idText est "+idText+" n'existe pas"));
			}
			return _list[idText];
		}

		/// <summary>
		/// Donne le texte ayant pour identifiant idText
		/// Si le texte n'existe pas, elle retourne "?"
		/// </summary>
		/// <param name="idText">Identifiant du texte recherch�</param>
		/// <returns>Texte demand�</returns>
		public string GetWebWord(Int64 idText){
			try{
				return(this.getWord(idText));
			}
			catch(TranslationException ){
				return("?");
			}
		}
		#endregion

		#region Accesseurs

		/// <summary>
		/// Accesseur de la langue en lecture
		/// </summary>
		public int LangueSite{
			get{return(_idLanguage);}
		}

		#endregion
	}
}
