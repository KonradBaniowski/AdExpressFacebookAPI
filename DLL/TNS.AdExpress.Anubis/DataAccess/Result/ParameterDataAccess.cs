#region Informations
// Auteur: G. Facon
// Date de création: 02/06/2005
// Date de modification: 02/06/2005
//		17/08/2005 : G. RAGNEAU - GetRequest
//		24/05/2006 : D. Mussuma - surcharge méthode Save

#endregion

using System;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

using TNS.AdExpress.Anubis.Exceptions;
using AnubisConstantes=TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using DbSchemas=TNS.AdExpress.Constantes.DB.Schema;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;


namespace TNS.AdExpress.Anubis.DataAccess.Result{

	
	/// <summary>
	/// Gestion des paramètres de résultats dans la base de données
	/// </summary>
	internal class ParameterDataAccess{

		#region Chargement et sauvegarde des sessions statiques

		/// <summary>
		/// Sauvegarde des paramètres d'un résultat PDF à partir de la base de données
		/// </summary>
		/// <param name="webSession">Session client à sauvegarder</param>
		/// <param name="resultType">Type de résultat</param>
		/// <returns>Identifiant correspondant à la session sauvegardée</returns>
		internal static Int64 Save(WebSession webSession,TNS.AdExpress.Anubis.Constantes.Result.type resultType)
		{
			return Save(webSession,resultType,webSession.ExportedPDFFileName);
		}

		/// <summary>
		/// Sauvegarde des paramètres d'un résultat (PDF;Texte;...) à partir de la base de données
		/// </summary>
		/// <param name="webSession">Session client à sauvegarder</param>
		/// <param name="resultType">Type de résultat</param>
		/// <param name="fileName">Nom du fichier</param>
		/// <returns>Identifiant correspondant à la session sauvegardée</returns>
		internal static Int64 Save(WebSession webSession,TNS.AdExpress.Anubis.Constantes.Result.type resultType,string fileName){

			Int64 idStaticNavSession=-1;
			OracleConnection connection =(OracleConnection) webSession.CustomerLogin.Source.GetSource();

			#region Ouverture de la base de données
			bool DBToClosed=false;
			// On teste si la base est déjà ouverte
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception et){
					throw(new ParameterDataAccessException("Impossible d'ouvrir la base de données",et));
				}
			}
			#endregion
			
			#region Sérialisation et sauvegarde de la session
			OracleCommand sqlCommand=null;
			MemoryStream ms=null;
			BinaryFormatter bf=null;
			byte[] binaryData=null;

			try{
				//"Serialization"
				ms = new MemoryStream();
				bf = new BinaryFormatter();
				bf.Serialize(ms,webSession);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

				//create anonymous PL/SQL command
				string block = ""+//" DECLARE "+
					//" NEW_ID NUMBER;"+
					" BEGIN "+
					//" DELETE " + Schema.APPLICATION_SCHEMA + "." + Tables.TABLE_SESSION + " WHERE ID_NAV_SESSION=" + this.idSession + "; " +
					" Select " + TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA + ".SEQ_STATIC_NAV_SESSION.NEXTVAL into :new_id from dual;"+
					
					" INSERT INTO " + TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA + "." + Tables.PDF_SESSION + "(id_login,id_static_nav_session, static_nav_session,id_pdf_result_type,pdf_user_filename,status,date_creation,date_modification) VALUES("+webSession.CustomerLogin.IdLogin+", :new_id, :blobtodb,"+resultType.GetHashCode()+",'"+fileName+"',0,sysdate,sysdate); " +
					//" RETURN(NEW_ID);"+
					" END; ";
				sqlCommand = new OracleCommand(block);
				sqlCommand.Connection = connection;
				sqlCommand.CommandType = CommandType.Text;
				//Fill parametres
				OracleParameter param2 = sqlCommand.Parameters.Add("new_id",OracleDbType.Int64);
				param2.Direction = ParameterDirection.Output;
				OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
				param.Direction = ParameterDirection.Input;
				param.Value = binaryData;
				
				
				//Execute PL/SQL block
				sqlCommand.ExecuteNonQuery();
				idStaticNavSession=Int64.Parse(param2.Value.ToString()); 

			}
				#endregion

			#region Gestion des erreurs dues à la sérialisation et à la sauvegarde de l'objet
			catch(System.Exception e){
				// Fermeture des structures
				try{
					if (ms != null) ms.Close();
					if (bf != null) bf=null;
					if(sqlCommand!=null) sqlCommand.Dispose();
					connection.Close();
					connection.Dispose();
				}
				catch(System.Exception et) {
					throw(new ParameterDataAccessException("WebSession.Save() : Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new ParameterDataAccessException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donnée",e));
			}
			#endregion
			
			#region Fermeture de la base de données
			try{
				ms.Close();
				bf=null;
				if(sqlCommand!=null)sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
			}
			catch(System.Exception et){
				throw(new ParameterDataAccessException ("Impossible de fermer la base de données",et));
			}
			#endregion
			
			return(idStaticNavSession);
		}

		/// <summary>
		/// Sauvegarde des paramètres d'un résultat (PDF;Texte;...) à partir de la base de données
		/// </summary>
		/// <param name="proofDetail">Paramètres d'une fiche justificative  à sauvegarder</param>
		/// <param name="resultType">Type de résultat</param>
		/// <param name="fileName">Nom du fichier</param>
		/// <returns>Identifiant correspondant à la session sauvegardée</returns>
		internal static Int64 Save(ProofDetail proofDetail,TNS.AdExpress.Anubis.Constantes.Result.type resultType,string fileName){

			Int64 idStaticNavSession=-1;
			OracleConnection connection = (OracleConnection)proofDetail.CustomerWebSession.CustomerLogin.Source.GetSource();

			#region Ouverture de la base de données
			bool DBToClosed=false;
			// On teste si la base est déjà ouverte
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception et){
					throw(new ParameterDataAccessException("Impossible d'ouvrir la base de données",et));
				}
			}
			#endregion
			
			#region Sérialisation et sauvegarde de la session
			OracleCommand sqlCommand=null;
			MemoryStream ms=null;
			BinaryFormatter bf=null;
			byte[] binaryData=null;

			try{
				//"Serialization"
				ms = new MemoryStream();
				bf = new BinaryFormatter();
				bf.Serialize(ms,proofDetail);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

				//create anonymous PL/SQL command
				string block = ""+//" DECLARE "+
					//" NEW_ID NUMBER;"+
					" BEGIN "+
					//" DELETE " + Schema.APPLICATION_SCHEMA + "." + Tables.TABLE_SESSION + " WHERE ID_NAV_SESSION=" + this.idSession + "; " +
					" Select " + TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA + ".SEQ_STATIC_NAV_SESSION.NEXTVAL into :new_id from dual;"+
					
					" INSERT INTO " + TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA + "." + Tables.PDF_SESSION + "(id_login,id_static_nav_session, static_nav_session,id_pdf_result_type,pdf_user_filename,status,date_creation,date_modification) VALUES("+proofDetail.CustomerWebSession.CustomerLogin.IdLogin+", :new_id, :blobtodb,"+resultType.GetHashCode()+",'"+fileName+"',0,sysdate,sysdate); " +
					//" RETURN(NEW_ID);"+
					" END; ";
				sqlCommand = new OracleCommand(block);
				sqlCommand.Connection = connection;
				sqlCommand.CommandType = CommandType.Text;
				//Fill parametres
				OracleParameter param2 = sqlCommand.Parameters.Add("new_id",OracleDbType.Int64);
				param2.Direction = ParameterDirection.Output;
				OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
				param.Direction = ParameterDirection.Input;
				param.Value = binaryData;
				
				
				//Execute PL/SQL block
				sqlCommand.ExecuteNonQuery();
				idStaticNavSession = Int64.Parse(param2.Value.ToString());

			}
				#endregion

				#region Gestion des erreurs dues à la sérialisation et à la sauvegarde de l'objet
			catch(System.Exception e){
				// Fermeture des structures
				try{
					if (ms != null) ms.Close();
					if (bf != null) bf=null;
					if(sqlCommand!=null) sqlCommand.Dispose();
					connection.Close();
					connection.Dispose();
				}
				catch(System.Exception et) {
					throw(new ParameterDataAccessException("WebSession.Save() : Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new ParameterDataAccessException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donnée",e));
			}
			#endregion
			
			#region Fermeture de la base de données
			try{
				ms.Close();
				bf=null;
				if(sqlCommand!=null)sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
			}
			catch(System.Exception et){
				throw(new ParameterDataAccessException ("Impossible de fermer la base de données",et));
			}
			#endregion
			
			return(idStaticNavSession);
		}


		/// <summary>
		/// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des static_nav_sessions
		/// </summary>
		/// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
		/// <param name="idStaticNavSession">Identifiant de la session sauvegardée</param>
		public static Object Load(Int64 idStaticNavSession){
			
			#region Ouverture de la base de données
			//TODO : develop IDataSource for blob loading
			//OracleConnection cnx = new OracleConnection(Connection.SESSION_CONNECTION_STRING_TEST);
            OracleConnection cnx = (OracleConnection)WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration).GetSource();
			try{
				cnx.Open();
			}
			catch(System.Exception e){
				throw(new ParameterDataAccessException("Impossible d'ouvrir la base de données",e));
			}			
			#endregion
			
			#region Chargement et deserialization de l'objet
			OracleCommand sqlCommand=null;
			MemoryStream ms=null;
			BinaryFormatter bf=null;
			byte[] binaryData=null;
			Object o = null;
			try{
				binaryData = new byte[0];
				//create PL/SQL command
				string block = " BEGIN "+
					" SELECT static_nav_session INTO :1 FROM " + TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA + "." + Tables.PDF_SESSION+ " WHERE id_static_nav_session = " + idStaticNavSession.ToString() + "; " +
					" END; ";
				sqlCommand = new OracleCommand(block);
				sqlCommand.Connection = cnx;
				sqlCommand.CommandType = CommandType.Text;
				//Initialize parametres
				OracleParameter param = sqlCommand.Parameters.Add("blobfromdb", OracleDbType.Blob);
				param.Direction = ParameterDirection.Output;

				//Execute PL/SQL block
				sqlCommand.ExecuteNonQuery();
				//Récupération des octets du blob
				binaryData = (byte[]) ((OracleBlob)(sqlCommand.Parameters[0].Value)).Value;
				
				//Deserialization oft the object
				ms = new MemoryStream();
				ms.Write(binaryData, 0, binaryData.Length);
				bf=new BinaryFormatter();
				ms.Position = 0;
				o = bf.Deserialize(ms);
			}
			#endregion

			#region Gestion des erreurs de chargement et de deserialization de l'objet
			catch(System.Exception e){
				try{
					// Fermeture des structures
					if (ms != null) ms.Close();
					if (bf != null) bf=null;
					if (binaryData != null) binaryData=null;
					if(sqlCommand!=null) sqlCommand.Dispose();
					cnx.Close();
				}
				catch(System.Exception et){
					throw(new ParameterDataAccessException("Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new ParameterDataAccessException("Problème au chargement de la session à partir de la base de données",e));
			}
			try{
				// Fermeture des structures
				if (ms != null) ms.Close();
				if (bf != null) bf=null;
				if (binaryData != null) binaryData=null;
				if(sqlCommand!=null) sqlCommand.Dispose();
				cnx.Close();
			}
			catch(System.Exception et){
				throw(new ParameterDataAccessException("Impossible de fermer la base de données",et));
			}
			#endregion
			
			//retourne l'objet deserialized ou null si il y a eu un probleme
			return(o);
		}
		#endregion

		#region Mise à jour de la demande
		/// <summary>
		/// Changement de status
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="idStaticNavSession">Demande à mettre à jour</param>
		/// <param name="pdfStatus">Nouveau Statut</param>
		internal static void ChangeStatus(IDataSource dataSource,Int64 idStaticNavSession,AnubisConstantes.Result.status pdfStatus){

			#region Requête
			string sql="update "+DbSchemas.UNIVERS_SCHEMA+"."+DbTables.PDF_SESSION+" set status="+pdfStatus.GetHashCode()+" where id_static_nav_session="+idStaticNavSession;
			#endregion

			try{
				dataSource.Update(sql);
			}
			catch(System.Exception err){
				throw(err);
			}
		}

		/// <summary>
		/// Register the physical file name associated to a session and update the staut to done
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="idStaticNavSession">User session</param>
		/// <param name="fileName">File Name</param>
		internal static void RegisterFile(IDataSource dataSource, Int64 idStaticNavSession,string fileName){

			#region Requête
			string sql="update "+DbSchemas.UNIVERS_SCHEMA+"."+DbTables.PDF_SESSION+" set status="+AnubisConstantes.Result.status.done.GetHashCode()+", pdf_name='" + fileName + "' where id_static_nav_session="+idStaticNavSession;
			#endregion

			try{
				dataSource.Update(sql);
			}
			catch(System.Exception err){
				throw(err);
			}
		}
		#endregion

		#region GetRequestDetails
		/// <summary>
		/// Get all details of the request idStaticNavSession apart from the blob field
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="idStaticNavSession">Session ID to get</param>
		/// <returns>DataSet containing all fields of the request except from the blob filed</returns>
		internal static DataSet GetRequestDetails(IDataSource dataSource, Int64 idStaticNavSession){

			#region Requête
			string sql="select ID_STATIC_NAV_SESSION, ID_PDF_RESULT_TYPE, PDF_NAME, PDF_USER_FILENAME, STATUS, DATE_CREATION, DATE_MODIFICATION, ID_LOGIN from "
				+ DbSchemas.UNIVERS_SCHEMA+"."+DbTables.PDF_SESSION
				+ " where id_static_nav_session="+idStaticNavSession;
			#endregion

			try{
				return dataSource.Fill(sql);
			}
			catch(System.Exception err){
				throw(err);
			}
		}
		#endregion

		#region Suppression d'une demande
		/// <summary>
		/// Delete a customer request
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="idStaticNavSession">Session ID to delete</param>
		internal static void DeleteRequest(IDataSource dataSource, Int64 idStaticNavSession){
			#region Requête
			string sql=" DELETE "+ DbSchemas.UNIVERS_SCHEMA+"."+DbTables.PDF_SESSION
				+ " where id_static_nav_session="+idStaticNavSession;
			#endregion

			try{
				 dataSource.Delete(sql);
			}
			catch(System.Exception err){
				throw(err);
			}
		}
		#endregion
	}
}
