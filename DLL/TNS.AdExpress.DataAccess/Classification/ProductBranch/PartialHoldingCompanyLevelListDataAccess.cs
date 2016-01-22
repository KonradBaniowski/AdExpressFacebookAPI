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
	/// Chargement partiel d'une liste de HoldingCompanies
	/// </summary>
	public class PartialHoldingCompanyLevelListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="codeList">Liste des éléments à charger</param>
		/// <param name="source">Connexion à la base de données</param>
		public PartialHoldingCompanyLevelListDataAccess(string codeList,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.holding_company,codeList,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="source">Connexion à la base de données</param>
		/// <param name="codeList">Liste des codes</param>
		public PartialHoldingCompanyLevelListDataAccess(string codeList,int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.holding_company,codeList,language,source){
		}

		#endregion
	}
}
