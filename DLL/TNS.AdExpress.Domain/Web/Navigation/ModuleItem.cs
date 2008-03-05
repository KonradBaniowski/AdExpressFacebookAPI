#region Informations
// Auteur: g. Facon
// Date de création:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables
#endregion

using System;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Classe utilisée dans l'affichage des textes des modules
	/// </summary>
	public class ModuleItem{

		#region variables
		/// <summary>
		/// identifiant de l'objet
		/// </summary>
		protected Int64 _id;
		/// <summary>
		/// Code traduction du text correspondant au module
		/// </summary>
		protected Int64 _idWebText;
		/// <summary>
		/// Identifiant de la description de l'item
		/// </summary>
		protected Int64 _descriptionWebTextId;
        /// <summary>
        /// Chemin de l'image qui apparait dans l'info bulle (page de sélection de module)
        /// </summary>
        protected string _descriptionImageName;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="idWebText">Code traduction de l'intitulé du module</param>
		/// <param name="id">identifiant de l'objet</param>
		public ModuleItem(Int64 id,Int64 idWebText){
			_id=id;
			_idWebText=idWebText;
		}
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="idWebText">Code traduction de l'intitulé du module</param>
        /// <param name="id">identifiant de l'objet</param>
        /// <param name="descriptionWebTextId">Identifiant de la description de l'item</param>
        public ModuleItem(Int64 id, Int64 idWebText, Int64 descriptionWebTextId) : this(id, idWebText) {
            _descriptionWebTextId = descriptionWebTextId;
        }
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="idWebText">Code traduction de l'intitulé du module</param>
		/// <param name="id">identifiant de l'objet</param>
		/// <param name="descriptionWebTextId">Identifiant de la description de l'item</param>
        /// <param name="descriptionImageName">Chemin de l'image qui apparait dans l'info bulle</param>
        public ModuleItem(Int64 id, Int64 idWebText, Int64 descriptionWebTextId, string descriptionImageName) : this(id, idWebText) {
			_descriptionWebTextId = descriptionWebTextId;
            _descriptionImageName = descriptionImageName;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient l'identifiant de l'objet
		/// </summary>
		public Int64 Id{
			get{return _id;}		
		}
		
		/// <summary>
		/// Get le code traduction du module
		/// </summary>
		public Int64 IdWebText{
			get{return(_idWebText);}
		}

		/// <summary>
		/// Obtient identifiant de la description de l'item
		/// </summary>
		public Int64 DescriptionWebTextId{
			get{return _descriptionWebTextId;}
		}

        /// <summary>
        /// Obtient le chemin de l'image qui apparait dans l'info bulle 
        /// </summary>
        public string DescriptionImageName{
            get{return _descriptionImageName;}
        }
		#endregion
	}
}
