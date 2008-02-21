#region Informations
// Auteur: G. Facon
// Date de création: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Classification.DataAccess.MediaBranch{
	/// <summary>
	/// Chargement partiel d'une liste de Categories
	/// </summary>
	public class PartialCategoryListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="codeList">Liste des éléments à charger</param>
		/// <param name="connection">Connexion à la base de données</param>
		public PartialCategoryListDataAccess(string codeList,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.category,codeList,DBConstantes.Language.FRENCH,connection){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <param name="codeList">Liste des codes</param>
		public PartialCategoryListDataAccess(string codeList,int language,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.category,codeList,language,connection){
		}

		#endregion
	}
}
