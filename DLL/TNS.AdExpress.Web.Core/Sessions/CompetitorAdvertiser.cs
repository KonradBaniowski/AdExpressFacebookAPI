#region Informations
// Auteur: g. Facon
// Date de création:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables
#endregion

using System;
using System.Windows.Forms;

namespace TNS.AdExpress.Web.Core.Sessions{
	/// <summary>
	/// Classe utilisée dans la sélection d'annonceur concurrentiel
	/// Réprésente un objet avec un treenode et un nom associé
	/// </summary>
	[System.Serializable]
	public class CompetitorAdvertiser{

		#region Variables
		/// <summary>
		/// Nom de l'univers associé à l'arbre
		/// </summary>
		protected string nameCompetitorAdvertiser;
		/// <summary>
		/// Arbre contenant la liste d'annonceur
		/// </summary>
		protected TreeNode treeCompetitorAdvertiser;		 
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CompetitorAdvertiser(string nameCompetitorAdvertiser,TreeNode treeCompetitorAdvertiser){
			this.nameCompetitorAdvertiser=nameCompetitorAdvertiser;
			this.treeCompetitorAdvertiser=treeCompetitorAdvertiser;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le nom de l'univers
		/// </summary>
		public string NameCompetitorAdvertiser{
			get{return this.nameCompetitorAdvertiser;}
		}

		/// <summary>
		/// Obtient l'arbre avec la liste des annonceurs
		/// </summary>
		public TreeNode TreeCompetitorAdvertiser{
			get{return this.treeCompetitorAdvertiser;}
		}

		#endregion
	}
}
