#region Information
/*
 * Author : G. Ragneau
 * Creation : 25/10/2005
 * Modifications:
 * */
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;

using TNS.AdExpress.Anubis.Common.Configuration;
using TNS.AdExpress.Anubis.DataAccess;
using TNS.AdExpress.Anubis.Exceptions;
using TNS.FrameWork.DB.Common;


namespace TNS.AdExpress.Anubis.Rules{
	/// <summary>
	/// Couche m�tier de gestion des requ�tes clients
	/// </summary>
	public class RequestsRules{

		/// <summary>
		/// Supprime les r�sultats ayant d�pass� la date de p�remption d�termin�e par le plugin g�n�rateur du r�sultat.
		/// </summary>
		/// <param name="dataSource">Source de donn�es</param>
		/// <param name="plugLst">Listes des plugins dont on veut supprimer les r�sultats p�rim�s</param>
		internal static void DeleteOldRequests(IDataSource dataSource, Hashtable plugLst){

			if (dataSource == null)throw new ArgumentNullException("dataSource ne peut pas �tre null");
			if (plugLst == null)return;
			if(plugLst.Count <= 0)return;
			
			DataTable fileLst = null;
			ArrayList errFiles = new ArrayList();

			try{
				fileLst = RequestsDataAccess.DeleteOldRequests(dataSource,plugLst);
				if (fileLst != null){
					foreach(DataRow row in fileLst.Rows){
						try{
							File.Delete(((Plugin)plugLst[int.Parse(row[2].ToString())]).ResultsRoot + @"\" + row[1].ToString() + @"\" + row[0].ToString());
						}
						catch(System.Exception exc){
							errFiles.Add(@"Unable to delete file \\" + row[1].ToString() + @"\" + row[0].ToString() + " : " + exc.Message);
						}
					}
					if (errFiles.Count > 0){
						string message = "Error while deleting files<br>";
						foreach( object obj in errFiles){
							message += "<br>" + (string) obj;
						}
						throw(new RequestsRulesException(message));
					}
				}
			}
			catch(RequestsRulesException ex){
				throw ex;
			}
			catch(System.Exception err){
				throw(new RequestsRulesException("Unable to clean old requests",err));
			}
		}

	}
}
