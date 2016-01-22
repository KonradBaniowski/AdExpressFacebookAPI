#region Information
/*
 * Author : G. Ragneau
 * Creation : -
 * Modifications:
 *		25/10/2005 : G. Ragneau - Ajout de DeleteOldRequest
 * */
#endregion

using System;
using System.Collections;
using System.Data;

using TNS.AdExpress.Anubis.DataAccess;
using TNS.AdExpress.Anubis.Rules;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.BusinessFacade{
	/// <summary>
	/// Point d'acc�s aux requ�tes clients
	/// 
	/// </summary>
	public class RequestsSystem{

		/// <summary>
		/// Obtient la liste des requ�tes � traiter
		/// </summary>
		/// <param name="dataSource">Source de donn�es</param>
		/// <param name="notIn">Identifiant Session � exclure</param>
		/// <param name="plgLst">Liste des types de requ�tes autoris�es (Si null, pas de limitation)</param>
		/// <returns>liste des requ�tes � traiter</returns>
		public static DataTable Get(IDataSource dataSource,string notIn, Hashtable plgLst){
			if (dataSource == null)throw new ArgumentNullException("dataSource ne peut pas �tre null");
			try{
				return(RequestsDataAccess.Get(dataSource,notIn, plgLst));
			}
			catch(System.Exception err){
				throw(err);
			}
		}

		/// <summary>
		/// Supprime les r�sultats ayant d�pass� la date de p�remption d�termin�e par le plugin g�n�rateur du r�sultat.
		/// </summary>
		/// <param name="dataSource">Source de donn�es</param>
		/// <param name="plugLst">Listes des plugins dont on veut supprimer les r�sultats p�rim�s</param>
		public static void DeleteOldRequests(IDataSource dataSource, Hashtable plugLst){
			if (dataSource == null)throw new ArgumentNullException("dataSource ne peut pas �tre null");
			if (plugLst == null)return;
			if(plugLst.Count <= 0)return;
			try{
				RequestsRules.DeleteOldRequests(dataSource,plugLst);
			}
			catch(System.Exception err){
				throw(err);
			}
		}

	}
}
