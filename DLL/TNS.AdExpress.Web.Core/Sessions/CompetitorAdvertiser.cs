#region Informations
// Auteur: g. Facon
// Date de cr�ation:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables
#endregion

using System;
using System.Windows.Forms;

namespace TNS.AdExpress.Web.Core.Sessions{
	/// <summary>
	/// Classe utilis�e dans la s�lection d'annonceur concurrentiel
	/// R�pr�sente un objet avec un treenode et un nom associ�
	/// </summary>
	[System.Serializable]
	public class CompetitorAdvertiser{

		#region Variables
		/// <summary>
		/// Nom de l'univers associ� � l'arbre
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
