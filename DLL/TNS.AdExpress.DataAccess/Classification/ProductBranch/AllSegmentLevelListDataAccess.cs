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
	/// Chargement partiel d'une liste de Segments
	/// </summary>
	public class AllSegmentLevelListDataAccess:ClassificationLevelListDataAccess{
		
		#region Constructeur

		/// <summary>
		/// Constructeur de données Française
		/// </summary>
		/// <param name="source">Connexion à la base de données</param>
		public AllSegmentLevelListDataAccess (IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.segment,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des données</param>
		/// <param name="source">Connexion à la base de données</param>
		public AllSegmentLevelListDataAccess(int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.segment,language,source){
		}
				
		#endregion
	}
}
