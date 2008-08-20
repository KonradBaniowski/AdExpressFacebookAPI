#region Informations
// Auteur: ?
// Date de cr�ation: 07/09/2004 
// Date de modification: 07/09/2004 
// 29/11/2005 Par B.Masson > Mise en place de IDataSource pour Execution de la requ�te
#endregion

using System;
using System.Data;
using Oracle.DataAccess.Client;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebException=TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.DataAccess.Selections.Periods{

	/// <summary>
	/// Traite les p�riodes des r�caps
	/// </summary>
	public class RecapDataAccess{

		/// <summary>
		/// Obtient la derni�re ann�e charg�e dans la base de donn�es pour les recap
		/// </summary>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.RecapDataAccessException">Erreur lors de l'ouverture, fermeture de la base donn�es ou de l'execution de la requ�te</exception>
		/// <returns>Derni�re ann�e charg�e</returns>
		internal static int GetLastLoadedYear(){

            #region Tables initilization
            Table recapInfo;
            try {
                recapInfo=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInfo);
            }
            catch(System.Exception err) {
                throw (new WebException.RecapDataAccessException("Impossible to get table names or schema label",err));
            }
            #endregion

			#region Requ�te
            string sql="select current_year from "+recapInfo.Sql+" where id_recap_info = (select max(id_recap_info) from "+recapInfo.Sql+")";
			#endregion

			#region Execution de la requ�te
			try{
                IDataSource source=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
				DataSet ds = source.Fill(sql.ToString());
				return(int.Parse(ds.Tables[0].Rows[0]["current_year"].ToString()));
			}
			catch(System.Exception err){
				throw (new WebException.RecapDataAccessException("Impossible d'obtenir la derni�re ann�e charg�e dans la base de donn�es pour les recap",err));
			}
			#endregion

			#region Ancien code
//			
//			#region Ouverture de la base de donn�es
//			bool DBToClosed=false;
//			// On teste si la base est d�j� ouverte			
//				DBToClosed=true;
//				try {
//					connection = new OracleConnection(DBConstantes.Connection.RECAP_CONNECTION_STRING);
//					connection.Open();
//				}
//				catch(System.Exception et) {
//					throw(new WebException.RecapDataAccessException("Impossible d'ouvrir la base de donn�es:"+et.Message));
//				}			
//			#endregion
//
//			try {
//				sqlCommand=new OracleCommand(sql,connection);
//				sqlAdapter=new OracleDataAdapter(sqlCommand);
//				sqlAdapter.Fill(ds);
//			}
//			#region Traitement d'erreur du chargement des donn�es
//			catch(System.Exception ex) {
//				try {
//					// Fermeture de la base de donn�es
//					if (sqlAdapter!=null) {
//						sqlAdapter.Dispose();
//					}
//					if(sqlCommand!=null) sqlCommand.Dispose();
//					if (DBToClosed) connection.Close();
//				}
//				catch(System.Exception et) {
//					throw(new WebException.RecapDataAccessException ("Impossible de fermer la base de donn�es, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
//				}
//				throw(new WebException.RecapDataAccessException ("Impossible de charger les donn�es:"+sql+" "+ex.Message));
//			}
//			#endregion
//
//			#region Fermeture de la base de donn�es
//			try {
//				// Fermeture de la base de donn�es
//				if (sqlAdapter!=null) {
//					sqlAdapter.Dispose();
//				}
//				if(sqlCommand!=null)sqlCommand.Dispose();
//				if (DBToClosed) connection.Close();
//			}
//			catch(System.Exception et) {
//				throw(new WebException.RecapDataAccessException ("Impossible de fermer la base de donn�es :"+et.Message));
//			}
//			#endregion
//
//			try{
//				return(int.Parse(ds.Tables[0].Rows[0]["current_year"].ToString()));
//			}
//			catch(System.Exception ex){
//				throw(new WebException.RecapDataAccessException("Impossible de retourner le r�sultat:"+ex.Message));
//			}
//
			#endregion
			
		}

	}
}
