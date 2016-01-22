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
	/// Classe utilis�e dans la s�lection d'un media
	/// R�pr�sente un objet avec un treenode et un nom associ�
	/// </summary>
	[System.Serializable]
	public class CompetitorMedia{	
		
		#region Variables
		/// <summary>
		/// Nom de l'univers associ� � l'arbre
		/// </summary>
		protected string nameCompetitorMedia;
		/// <summary>
		/// Arbre contenant la liste d'annonceur
		/// </summary>
		protected TreeNode treeCompetitorMedia;		 
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CompetitorMedia(string nameCompetitorMedia,TreeNode treeCompetitorMedia){
			this.nameCompetitorMedia=nameCompetitorMedia;
			this.treeCompetitorMedia=treeCompetitorMedia;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le nom de l'univers
		/// </summary>
		public string NameCompetitorMedia{
			get{return nameCompetitorMedia;}
		}

		/// <summary>
		/// Obtient l'arbre avec la liste des medias
		/// </summary>
		public TreeNode TreeCompetitorMedia{
			get{return treeCompetitorMedia;}
		}

		#endregion
	}
}
