#region Informations
// Auteur: D.Mussuma, B.Masson
// Date de création: 17/03/2006
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Classification.DataAccess.MediaBranch{
	/// <summary>
	/// Chargement partiel d'une liste de centre d'interet
	/// </summary>
	public class PartialInterestCenterListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="codeList">Liste des éléments à charger</param>
		/// <param name="connection">Connexion à la base de données</param>
		public PartialInterestCenterListDataAccess(string codeList,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.interest_center,codeList,DBConstantes.Language.FRENCH,connection){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <param name="codeList">Liste des codes</param>
		public PartialInterestCenterListDataAccess(string codeList,int language,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.interest_center,codeList,language,connection){
		}

		#endregion
	}
}
