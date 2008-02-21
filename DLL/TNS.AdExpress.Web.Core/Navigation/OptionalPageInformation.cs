#region Informations
// Auteur: g. Facon
// Date de création:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables 
//  Y. R'kaina 13/12/2006 _optionalNextUrl pour designer l'Url suivante d'une page optionnelle
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace TNS.AdExpress.Web.Core.Navigation{

	/// <summary>
	/// Page en option (Configuration d'un module)
	/// </summary>
	public class OptionalPageInformation:PageInformation{

		#region Variables
		/// <summary>
		/// Doit on montrer la page dans le composant recall
		/// </summary>
		protected bool _showLink;
		/// <summary>
		/// Nom de l'icone
		/// </summary>
		protected string _iconeName;
		/// <summary>
		/// Liste des types d'univers sauvegardés dans la page
		/// </summary>
		protected ArrayList _loadableUnivers=new ArrayList();
		/// <summary>
		/// Url suivante
		/// </summary>
		private string _optionalNextUrl="";

		/// <summary>
		/// Allowed universe levels Identifiers
		/// </summary>
		protected List<long> _allowedLevelsIds = new List<long>();
		/// <summary>
		///  allowed branches identifiers
		/// </summary>
		protected List<int> _allowedBranchesIds = new List<int>();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de la page</param>
		/// <param name="showLink">Doit on montrer la page dans le composant recall</param>
		/// <param name="url">url de la page</param>
		/// <param name="idWebText">identifiant du text</param>
		/// <param name="iconeName">Nom de l'icone</param>
		/// <param name="helpUrlValue">Url de la page d'aide</param>
		///<param name="menuTextId">Identifiant du texte dans le menu correspondant à la page</param>
		public OptionalPageInformation(int id, bool showLink, string url, Int64 idWebText,string iconeName,string helpUrlValue,Int64 menuTextId):base(id,url,idWebText,helpUrlValue,menuTextId){
			_showLink=showLink;
			_iconeName=iconeName;
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de la page</param>
		/// <param name="showLink">Doit on montrer la page dans le composant recall</param>
		/// <param name="url">url de la page</param>
		/// <param name="idWebText">identifiant du text</param>
		/// <param name="iconeName">Nom de l'icone</param>
		/// <param name="helpUrlValue">Url de la page d'aide</param>
		///<param name="menuTextId">Identifiant du texte dans le menu correspondant à la page</param>
		///<param name="optionalNextUrl">Url suivante</param>
		public OptionalPageInformation(int id, bool showLink, string url, Int64 idWebText,string iconeName,string helpUrlValue,Int64 menuTextId,string optionalNextUrl):base(id,url,idWebText,helpUrlValue,menuTextId){
			_showLink=showLink;
			_iconeName=iconeName;
			_optionalNextUrl = optionalNextUrl;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Doit on montrer la page dans le composant recall
		/// </summary>
		public bool ShowLink{
			get{return(_showLink);}
		}

		/// <summary>
		/// Obtient le nom de l'icone
		/// </summary>
		public string IconeName{
			get{return(_iconeName);}
		}

		/// <summary>
		/// Obtient la liste des types d'univers sauvegardés dans la page
		/// La liste contient des integers
		/// </summary>
		public ArrayList LoadableUnivers{
			get{return(_loadableUnivers);}
		}

		/// <summary>
		/// Obtient la liste des types d'univers sauvegardés dans la page
		/// La liste contient des integers.
		/// </summary>
		/// <remarks>
		/// Le format est id,id,id...
		/// </remarks>
		public string LoadableUniversString{
			get{
				string tmp="";
				foreach(int currentType in _loadableUnivers){
					tmp+=currentType.ToString()+",";
				}
				if(tmp.Length>1)tmp=tmp.Substring(0,tmp.Length-1);
				return(tmp);
			}
		}

		/// <summary>
		/// Obtient Url suivante
		/// </summary>
		public string OptionalNextUrl{
			get{return(_optionalNextUrl);}
		}

		/// <summary>
		/// Get universe levels Identifiers
		/// </summary>
		public List<long> AllowedLevelsIds {
			get { return (_allowedLevelsIds); }
		}
		/// <summary>
		/// Get universe branches Identifiers
		/// </summary>
		public List<int> AllowedBranchesIds {
			get { return (_allowedBranchesIds); }
		}
		#endregion
	}
}
