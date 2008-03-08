#region Informations
// Auteur: G. Facon
// Date de création: 10/02/2006
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;

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
		/// <param name="source">Connexion à la base de données</param>
		public PartialBrandLevelListDataAccess(string codeList,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.brand,codeList,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="source">Connexion à la base de données</param>
		/// <param name="codeList">liste des codes</param>
		public PartialBrandLevelListDataAccess(string codeList,int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.brand,codeList,language,source){
		}
		#endregion

	}
}

