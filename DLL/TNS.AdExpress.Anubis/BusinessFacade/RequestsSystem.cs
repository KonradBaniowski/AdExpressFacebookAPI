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
	/// Point d'accès aux requêtes clients
	/// 
	/// </summary>
	public class RequestsSystem{

		/// <summary>
		/// Obtient la liste des requêtes à traiter
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="notIn">Identifiant Session à exclure</param>
		/// <param name="plgLst">Liste des types de requêtes autorisées (Si null, pas de limitation)</param>
		/// <returns>liste des requêtes à traiter</returns>
		public static DataTable Get(IDataSource dataSource,string notIn, Hashtable plgLst){
			if (dataSource == null)throw new ArgumentNullException("dataSource ne peut pas être null");
			try{
				return(RequestsDataAccess.Get(dataSource,notIn, plgLst));
			}
			catch(System.Exception err){
				throw(err);
			}
		}

		/// <summary>
		/// Supprime les résultats ayant dépassé la date de péremption déterminée par le plugin générateur du résultat.
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="plugLst">Listes des plugins dont on veut supprimer les résultats périmés</param>
		public static void DeleteOldRequests(IDataSource dataSource, Hashtable plugLst){
			if (dataSource == null)throw new ArgumentNullException("dataSource ne peut pas être null");
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
