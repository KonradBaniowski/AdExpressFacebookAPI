#region Informations
// Auteur: G. Facon
// Date de création: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using TNS.AdExpress.DataAccess.Classification;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.DataAccess.Classification.ProductBranch {
	/// <summary>
	/// Chargement partiel d'une liste de Sectors
	/// </summary>
	public class AllSectorLevelListDataAccess:ClassificationLevelListDataAccess{
		
		#region Constructeur

		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="source">Connexion à la base de données</param>
		public AllSectorLevelListDataAccess (IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.sector,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="source">Connexion à la base de données</param>
		public AllSectorLevelListDataAccess(int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.sector,language,source){
		}
				
		#endregion
	}
}
