#region Informations
// Auteur: B. Masson
// Date de création: 18/11/2004 
// Date de modification: 
//	G. Facon	11/08/2005	Nom des variables
#endregion

using System;
using System.Collections;

using TNS.AdExpress.Web.Exceptions;

namespace TNS.AdExpress.Web.Common.Results{
	/// <summary>
	/// InfoNewsHashTable permet d'imbriquer plusieurs tableaux de résultat
	/// </summary>
	public class InfoNewsItem{
		
		#region Variables
		/// <summary>
		/// Nom de la liste d'éléments
		/// </summary>
		protected string _name="";
		/// <summary>
		/// Liste des éléments
		/// </summary>
		protected string[,] _list=null;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom de la liste d'éléments</param>
		/// <param name="list">Liste des éléments</param>
		public InfoNewsItem(string name, string[,] list){
			_name = name;
			_list=list;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le nom de la liste d'éléments
		/// </summary>
		public string Name{
			get{return(_name);}
		}

		/// <summary>
		/// Obtient la liste des éléments
		/// </summary>
		public string[,] List{
			get{return(_list);}
		}
		#endregion
	}
}
