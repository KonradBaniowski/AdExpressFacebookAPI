#region Informations
// Auteur: G. Facon
// Date de cr�ation: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Classification.DataAccess.MediaBranch{
	/// <summary>
	/// Chargement partiel d'une liste de vehicles
	/// </summary>
	public class PartialVehicleListDataAccess:ClassificationLevelListDataAccess{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de donn�es Fran�aise
		/// </summary>
		/// <param name="codeList">Liste des �l�ments � charger</param>
		/// <param name="connection">Connexion � la base de donn�es</param>
		public PartialVehicleListDataAccess(string codeList,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.vehicle,codeList,DBConstantes.Language.FRENCH,connection){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des donn�es</param>
		/// <param name="connection">Connexion � la base de donn�es</param>
		/// <param name="codeList">Liste des codes</param>
		public PartialVehicleListDataAccess(string codeList,int language,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.vehicle,codeList,language,connection){
		}

		#endregion
	}
}
