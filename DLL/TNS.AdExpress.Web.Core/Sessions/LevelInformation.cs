using System;
using CustomerCst=TNS.AdExpress.Constantes.Customer;

namespace TNS.AdExpress.Web.Core.Sessions{
	/// <summary>
	/// Classe qui contient des informations sur un noeud d'un arbre de s�lection d'�l�ment de nomenclature produit ou support
	/// Classe destin�e � �tre utilis�e dans un objet TreeNode
	/// </summary>
	[System.Serializable]
	public class LevelInformation{

		#region Variables
		/// <summary>
		/// Type de Node : niveau et acc�s (en acc�s ou en exception)
		/// </summary>
        protected CustomerCst.Right.type type;
		
		/// <summary>
		/// Identifiant de l'�l�ment. Le libell� de l'�l�ment est contenu dans le champ Text du treenode
		/// </summary>
		protected Int64 id;

		/// <summary>
		/// libell� de l'�l�ment
		/// </summary>
		protected string text;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="type">Type de Node : niveau et acc�s (en acc�s ou en exception)</param>
		/// <param name="id">Identifiant de l'�l�ment.</param>
		/// <param name="text">Label de l'�l�ment</param>
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
