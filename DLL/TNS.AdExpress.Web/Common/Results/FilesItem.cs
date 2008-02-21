#region Informations
// Auteur: A.DADOUCH
// Date de création:22/08/2005
#endregion

using System;
using System.Collections;

using TNS.AdExpress.Web.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Common.Results{
	/// <summary>
	/// permet d'imbriquer plusieurs tableaux de résultat
	/// </summary>
	public class FilesItem{
		
		#region Variables
		/// <summary>
		/// Nom de la liste d'éléments
		/// </summary>
		protected string _name="";
		/// <summary>
		/// Type du fichier
		/// </summary>
		protected TNS.AdExpress.Anubis.Constantes.Result.type _type;
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
		/// <param name="type">Type du fichier</param>
		public FilesItem(string name, string[,] list, TNS.AdExpress.Anubis.Constantes.Result.type type){
			_name = name;
			_list=list;
			_type=type;
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

		/// <summary>
		/// Obtient le type du fichier
		/// </summary>
		public TNS.AdExpress.Anubis.Constantes.Result.type Type
		{
			get{return(_type);}
		}
		#endregion
	}
}