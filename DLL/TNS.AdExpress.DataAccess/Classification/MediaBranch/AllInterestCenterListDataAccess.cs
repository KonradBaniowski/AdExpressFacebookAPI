#region Informations
// Auteur: D.Mussuma, B.Masson
// Date de création: 17/03/2006
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.DataAccess.Classification;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.DataAccess.Classification.MediaBranch {
	/// <summary>
	/// Chargement d'une liste de centre d'interet
	/// </summary>
	public class AllInterestCenterListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur

		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="source">Connexion à la base de données</param>
		public AllInterestCenterListDataAccess(IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.interest_center,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="source">Connexion à la base de données</param>
		public AllInterestCenterListDataAccess(int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.interest_center,language,source){
		}

		#endregion
	}
}
