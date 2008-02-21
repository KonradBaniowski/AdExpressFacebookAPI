#region Informations
// Auteur: G. Facon
// Date de création: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;	
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Classification.DataAccess.ProductBranch{
	/// <summary>
	/// Chargement partiel d'une liste de SubSectors
	/// </summary>
	public class AllSubSectorLevelListDataAccess:ClassificationLevelListDataAccess{
		
		#region Constructeur

		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="connection">Connexion à la base de données</param>
		public AllSubSectorLevelListDataAccess (OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.subsector,DBConstantes.Language.FRENCH,connection){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="connection">Connexion à la base de données</param>
		public AllSubSectorLevelListDataAccess(int language,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.subsector,language,connection){
		}
				
		#endregion
	}
}
