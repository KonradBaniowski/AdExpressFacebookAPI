#region Informations
// Auteur: G. Facon
// Date de création: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using TNS.AdExpress.DataAccess.Classification;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.DataAccess.Classification.MediaBranch {
	/// <summary>
	/// Chargement d'une liste de Categories
	/// </summary>
	public class AllCategoryListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur

		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="source">Connexion à la base de données</param>
		public AllCategoryListDataAccess(IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.category,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="source">Connexion à la base de données</param>
		public AllCategoryListDataAccess(int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.category,language,source){
		}

		#endregion
	}
}
