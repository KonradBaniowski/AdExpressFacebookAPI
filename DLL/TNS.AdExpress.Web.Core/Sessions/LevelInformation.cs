using System;
using CustomerCst=TNS.AdExpress.Constantes.Customer;

namespace TNS.AdExpress.Web.Core.Sessions{
	/// <summary>
	/// Classe qui contient des informations sur un noeud d'un arbre de sélection d'élément de nomenclature produit ou support
	/// Classe destinée à être utilisée dans un objet TreeNode
	/// </summary>
	[System.Serializable]
	public class LevelInformation{

		#region Variables
		/// <summary>
		/// Type de Node : niveau et accès (en accès ou en exception)
		/// </summary>
        protected CustomerCst.Right.type type;
		
		/// <summary>
		/// Identifiant de l'élément. Le libellé de l'élément est contenu dans le champ Text du treenode
		/// </summary>
		protected Int64 id;

		/// <summary>
		/// libellé de l'élément
		/// </summary>
		protected string text;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="type">Type de Node : niveau et accès (en accès ou en exception)</param>
		/// <param name="id">Identifiant de l'élément.</param>
		/// <param name="text">Label de l'élément</param>
        public LevelInformation(CustomerCst.Right.type type,Int64 id,string text) {
			this.type = type;
			this.id = id;
			this.text = text;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Get type
		/// </summary>
        public CustomerCst.Right.type Type {
			get{
				return this.type;
			}
		}

		/// <summary>
		/// Get Id
		/// </summary>
		public Int64 ID{
			get{
				return this.id;
			}
		}
		
		/// <summary>
		/// Get label
		/// </summary>
		public string Text{
			get{
				return text;
			}
		}
		#endregion
	}
}
