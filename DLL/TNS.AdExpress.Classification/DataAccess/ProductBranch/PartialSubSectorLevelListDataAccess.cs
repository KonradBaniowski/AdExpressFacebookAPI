#region Informations
// Auteur: G. Facon
// Date de cr�ation: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Classification.DataAccess.ProductBranch{
	/// <summary>
	/// Chargement partiel d'une liste de SubSectors
	/// </summary>
	public class PartialSubSectorLevelListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de donn�es Fran�aise
		/// </summary>
		/// <param name="codeList">Liste des �l�ments � charger</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		public PartialSubSectorLevelListDataAccess(string codeList,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.subsector,codeList,DBConstantes.Language.FRENCH,source){
		}
	
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des donn�es</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		/// <param name="codeList">Liste des codes</param>
		public PartialSubSectorLevelListDataAccess(string codeList,int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.subsector,codeList,language,source){
		}

		#endregion
	}
}
