#region Informations
// Auteur: D.Mussuma, B.Masson
// Date de cr�ation: 17/03/2006
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Classification.DataAccess.MediaBranch{
	/// <summary>
	/// Chargement partiel d'une liste de r�gies
	/// </summary>
	public class PartialMediaSellerListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de donn�es Fran�aise
		/// </summary>
		/// <param name="codeList">Liste des �l�ments � charger</param>
		/// <param name="connection">Connexion � la base de donn�es</param>
		public PartialMediaSellerListDataAccess(string codeList,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.media_seller,codeList,DBConstantes.Language.FRENCH,connection){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des donn�es</param>
		/// <param name="connection">Connexion � la base de donn�es</param>
		/// <param name="codeList">Liste des codes</param>
		public PartialMediaSellerListDataAccess(string codeList,int language,OracleConnection connection):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.media_seller,codeList,language,connection){
		}

		#endregion
	}
}
