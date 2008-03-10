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
	/// Chargement partiel d'une liste de r�gies
	/// </summary>
	public class PartialMediaSellerListDataAccess:ClassificationLevelListDataAccess{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de donn�es Fran�aise
		/// </summary>
		/// <param name="codeList">Liste des �l�ments � charger</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		public PartialMediaSellerListDataAccess(string codeList,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.media_seller,codeList,DBConstantes.Language.FRENCH,source){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="language">Langue des donn�es</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		/// <param name="codeList">Liste des codes</param>
		public PartialMediaSellerListDataAccess(string codeList,int language,IDataSource source):base(TNS.AdExpress.Constantes.Classification.DB.Table.name.media_seller,codeList,language,source){
		}

		#endregion
	}
}
