#region Informations
// Auteur: A. Obermeyer
// Date de création: 10/12/2004
// Date de modification:
//	G. Facon	11/08/2005	Nom des variables  
#endregion

using System;
using System.Windows.Forms;
using TNS.AdExpress.Constantes.Classification;

namespace TNS.AdExpress.Web.Core.Sessions{

	/// <summary>
	/// Liste d'élément produit
	/// </summary>
	 [System.Serializable]
	public class ProductLevelSelection{


		#region Variables
		/// <summary>
		/// Niveau de detail pour la branche produit
		/// </summary>
		protected Level.type levelProduct;
		/// <summary>
		/// Liste des produits
		/// </summary>
		protected TreeNode listElement;

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="levelProduct">Niveau de détail</param>
		/// <param name="listElement">TreeNode avec la liste des éléments</param>
		public ProductLevelSelection(Level.type levelProduct,TreeNode listElement){
			this.levelProduct=levelProduct;
			this.listElement=listElement;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le niveau de detail pour la branche produit
		/// </summary>
		public Level.type LevelProduct{
			get{return(levelProduct);}
			set{this.levelProduct=value;}

		}

		/// <summary>
		/// Obtient la liste
		/// </summary>
		public TreeNode ListElement{
			get{return(listElement);}
			set{this.listElement=value;}

		}
		#endregion
	}
}
