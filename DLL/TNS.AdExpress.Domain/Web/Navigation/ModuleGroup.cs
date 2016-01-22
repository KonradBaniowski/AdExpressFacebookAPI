#region Informations
// Auteur: g. Facon
// Date de cr�ation:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables
#endregion

using System;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Classe utilis�e dans l'affichage des textes des modules
	/// </summary>
	public class ModuleGroup:ModuleItem{

		#region Variables
		/// <summary>
		/// Chemin Flash
		/// </summary>
		protected string _flashPath="";
		/// <summary>
		/// Chemin image qui remplace image flash
		/// </summary>
		protected string _missingFlashUrl="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur utilis� pour les groupes de Modules
		/// </summary>
		/// <param name="id">identifiant </param>
		/// <param name="idWebText">Code traduction de l'intitul� du module</param>
		/// <param name="flashPath">adresse du fichier flash</param>
		/// <param name="missingFlashUrl">adresse de l'image flash</param>
		/// <param name="descriptionWebTextId">Commentaire groupe de modules</param>
		public ModuleGroup(Int64 id,Int64 idWebText,string flashPath,string missingFlashUrl,Int64 descriptionWebTextId):base(id,idWebText,descriptionWebTextId){			
			_flashPath=flashPath;
			_missingFlashUrl=missingFlashUrl;
		}
		#endregion

		#region Accesseur
		/// <summary>
		/// Obtient l'adresse du fichier flash
		/// </summary>
		public string FlashPath{
			get{return _flashPath;}		
		}
		/// <summary>
		/// Obtient l'adresse de l'image rempla�ant le flash 
		/// </summary>
		public string MissingFlashUrl{
			get{return _missingFlashUrl;}		
		}
		#endregion

	}
}
