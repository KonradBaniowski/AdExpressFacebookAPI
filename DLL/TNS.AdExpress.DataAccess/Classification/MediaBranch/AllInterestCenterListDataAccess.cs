#region Informations
// Auteur: D.Mussuma, B.Masson
// Date de cr�ation: 17/03/2006
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.DataAccess.Classification;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.DataAccess.Classification.MediaBranch {
	/// <summary>
	/// Chargement d'une liste de centre d'interet
	/// </summary>
	public class AllInterestCenterListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur

		/// <summary>
		/// Constructeur de donn�es Fran�aise
		/// </summary>
		/// <param name="source">Connexion � la base de donn�es</param>
		public AllInterestCenterListDataAccess(IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.interest_center,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des donn�es</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		public AllInterestCenterListDataAccess(int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.interest_center,language,source){
		}

		#endregion
	}
}
