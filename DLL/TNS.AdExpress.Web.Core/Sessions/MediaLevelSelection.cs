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
	/// Liste d'éléments media
	/// </summary>
	[System.Serializable]
	public class MediaLevelSelection{

		#region Variables
		/// <summary>
		/// Niveau de detail pour la branche média
		/// </summary>
		protected Level.type levelMedia;
		/// <summary>
		/// Liste des médias
		/// </summary>
		protected TreeNode listElement;

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="levelMedia">Niveau de détail</param>
		/// <param name="listElement">liste des éléments</param>
		public MediaLevelSelection(Level.type levelMedia,TreeNode listElement){
			this.levelMedia=levelMedia;
			this.listElement=listElement;
		}
		#endregion
		
		#region Accesseurs
		/// <summary>
		/// Obtient le niveau de detail pour la branche media
		/// </summary>
		public Level.type LevelMedia{
			get{return(this.levelMedia);}
			set{this.levelMedia=value;}

		}

		/// <summary>
		/// Obtient la liste d'éléments
		/// </summary>
		public TreeNode ListElement{
			get{return(this.listElement);}
			set{this.listElement=value;}

		}
		#endregion
		
	}
}
