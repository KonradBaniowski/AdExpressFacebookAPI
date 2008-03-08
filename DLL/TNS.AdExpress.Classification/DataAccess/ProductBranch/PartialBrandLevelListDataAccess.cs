#region Informations
// Auteur: G. Facon
// Date de cr�ation: 10/02/2006
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
		/// Constructeur de donn�es Fran�aise
		/// </summary>
		/// <param name="codeList">Liste des �l�ments � charger</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		public PartialBrandLevelListDataAccess(string codeList,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.brand,codeList,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des donn�es</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		/// <param name="codeList">liste des codes</param>
		public PartialBrandLevelListDataAccess(string codeList,int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.brand,codeList,language,source){
		}
		#endregion

	}
}

