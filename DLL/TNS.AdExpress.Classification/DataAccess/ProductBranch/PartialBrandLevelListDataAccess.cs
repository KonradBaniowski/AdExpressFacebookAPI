#region Informations
// Auteur: G. Facon
// Date de création: 10/02/2006
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Classification.DataAccess.ProductBranch{
	/// <summary>
	/// Chargement partiel d'une liste de Groups
	/// </summary>
	public class PartialBrandLevelListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur
		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="codeList">Liste des éléments à charger</param>
		/// <param name="connection">Connexion à la base de données</param>
		public PartialBrandLevelListDataAccess(string codeList,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.brand,codeList,DBConstantes.Language.FRENCH,connection){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <param name="codeList">liste des codes</param>
		public PartialBrandLevelListDataAccess(string codeList,int language,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.brand,codeList,language,connection){
		}
		#endregion

	}
}

