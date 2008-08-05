#region Informations
// Auteur: G. Facon
// Date de création: 14/06/2004
// Date de modification: 15/06/2004
//	G. Facon		11/08/2005	New Exception Management and property name 
//	D. V. Mussuma	08/11/2005	Méthode de chargements de topus les identifiants de session 
//	G. Facon		21/11/2005	Utilisation IDataSource A modifier la dernière méthode (louche)

#endregion

using System;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
namespace TNS.AdExpress.Web.DataAccess.MyAdExpress{
	/// <summary>
	/// Donne la liste des sessions d'un client qui sont enregistrées, elle sont classés par répertoire
	/// </summary>
	public class MySessionsDataAccess{

		#region Méthodes

		/// <summary>
		/// Donne la liste des sessions d'un client qui sont enregistrées
		/// </summary>
		/// <remarks>Testé</remarks>
		/// <param name="webSession">Session du client</param>
		/// <returns>Listes de sessions</returns>
		public static DataSet GetData(WebSession webSession){

			#region Construction de la requête

			string sql="select dir.ID_DIRECTORY, dir.DIRECTORY, se.ID_MY_SESSION, se.MY_SESSION ";
			sql += " from " + TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA + ".DIRECTORY dir, " + TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA + ". " + TNS.AdExpress.Constantes.DB.Tables.MY_SESSION + "  se ";
			sql+=" where dir.ID_LOGIN="+webSession.CustomerLogin.IdLogin.ToString();
			sql+=" and dir.ACTIVATION<"+DBConstantes.ActivationValues.UNACTIVATED ;
			sql+=" and (se.ACTIVATION<"+DBConstantes.ActivationValues.UNACTIVATED+" or se.ACTIVATION is null) ";
			sql+=" and dir.ID_DIRECTORY=se.ID_DIRECTORY(+) ";			
			sql +=" order by dir.DIRECTORY, se.MY_SESSION ";

			#endregion

			#region Execution de la requête
			try{			
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible d'obtenir la liste des sessions d'un client qui sont enregistrées",err));
			}
			#endregion
			
		}

		/// <summary>
		/// Donne la liste des répertoires
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="webSession">webSession</param>
		/// <returns>Dataset avec l'identifiant Répertoire et son nom </returns>
		public static DataSet GetDirectories(WebSession webSession){
		
			#region Construction de la requête

			//Requête pour récupérer tous les univers d'un idLogin
			string sql="select distinct dir.ID_DIRECTORY, dir.DIRECTORY ";
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".DIRECTORY dir ";
			sql+=" where dir.ID_LOGIN="+webSession.CustomerLogin.IdLogin;
			sql+=" and dir.ACTIVATION<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";			
			sql+=" order by dir.DIRECTORY ";
		
			#endregion

			#region Execution de la requête
			try{								
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible d'obtenir la liste des répertoires",err));
			}
			#endregion
		}

		/// <summary>
		/// Vérifie si une session est déjà enregistrée dans la base de données
		/// </summary>
		/// <remarks>Pas testée</remarks>
		/// <param name="webSession">webSession</param>
		/// <param name="sessionName">Nom de la session</param>
		/// <returns>Retourne vrai si la session existe</returns>
		public static bool IsSessionExist(WebSession webSession, string sessionName){

			#region Construction de la requête
			string sql="select se.MY_SESSION from "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".MY_SESSION se, ";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".DIRECTORY dir";
			sql+=" where dir.ID_LOGIN="+webSession.CustomerLogin.IdLogin;
			sql+=" and dir.ACTIVATION<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and se.ACTIVATION<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and dir.ID_DIRECTORY = se.ID_DIRECTORY ";
			sql+=" and UPPER(se.MY_SESSION) like UPPER('"+sessionName+"')";
			#endregion

			#region Execution de la requête
			DataSet ds;
			try{
				ds=webSession.Source.Fill(sql);
				if(ds.Tables[0].Rows.Count>0)return(true);
				return(false);
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible de vérifier si une session est déjà enregistrée dans la base de données",err));
			}
			#endregion
		}



	
		/// <summary>
		/// Vérifie s'il existe des sessions dans un répertoire
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="webSession">webSession</param>
		/// <param name="idDirectory">Identifiant répertoire</param>
		/// <returns>Retourne vrai s'il existe des sessions dans un répertoire</returns>
		public static bool IsSessionsInDirectoryExist(WebSession webSession, Int64 idDirectory){

			#region Construction de la requête
			string sql="select se.MY_SESSION from "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".MY_SESSION se, ";
			sql+=" "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".DIRECTORY dir";
			sql+=" where dir.ID_LOGIN="+webSession.CustomerLogin.IdLogin;
			sql+=" and dir.ACTIVATION<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and se.ACTIVATION<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and dir.ID_DIRECTORY = se.ID_DIRECTORY ";
			sql+=" and dir.ID_DIRECTORY="+idDirectory+" ";
			#endregion

			#region Execution de la requête
			DataSet ds;
			try{
				ds=webSession.Source.Fill(sql);
				if(ds.Tables[0].Rows.Count>0)return(true);
				return(false);
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible de vérifier s'il existe des sessions dans un répertoire",err));
			}
			#endregion
		}



		/// <summary>
		/// Vérifie l'existance d'un répertoire
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="webSession">webSession</param>
		/// <param name="DirectoryName">Nom du répertoire</param>
		/// <returns>Retourne vrai si le répertoire existe</returns>
		public static bool IsDirectoryExist(WebSession webSession, string DirectoryName){

			#region Construction de la requête
			string sql="select distinct dir.ID_DIRECTORY, dir.DIRECTORY ";
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".DIRECTORY dir ";
			sql+=" where dir.ID_LOGIN="+webSession.CustomerLogin.IdLogin;
			sql+=" and dir.ACTIVATION<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and UPPER(dir.DIRECTORY)=UPPER('"+DirectoryName+"') ";
			#endregion

			#region Execution de la requête
			DataSet ds;
			try{
				ds=webSession.Source.Fill(sql);
				if(ds.Tables[0].Rows.Count>0)return(true);
				return(false);
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible de vérifier l'existance d'un répertoire",err));
			}
			#endregion
		}




		/// <summary>
		/// Création d'un répertoire
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="nameDirectory">Nom du répertoire</param>
		/// <param name="webSession">webSession</param>
		public static bool CreateDirectory(string nameDirectory, WebSession webSession){

			#region Requête pour insérer les champs dans la table Group_universe_client	
			string sql="INSERT INTO "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".DIRECTORY(ID_DIRECTORY,id_login,DIRECTORY,DATE_CREATION,ACTIVATION) ";
			sql+="values ("+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".seq_directory.nextval,"+webSession.CustomerLogin.IdLogin+",'"+nameDirectory+"',SYSDATE,"+TNS.AdExpress.Constantes.DB.ActivationValues.ACTIVATED+")";
			#endregion

			#region Execution de la requête
			try{
				webSession.Source.Insert(sql);
				return(true);
			}
			catch(System.Exception){
				return(false);
			}
			#endregion
		}


		/// <summary>
		///  Renomme un répertoire
		/// </summary>
		/// <remarks>Testé</remarks>
		/// <param name="newName"></param>
		/// <param name="idDirectory">Identifiant Répertoire</param>
		/// <param name="webSession">webSession</param>
		public static void RenameDirectory(string newName, Int64 idDirectory,WebSession webSession){

			#region Requête pour mettre à jour le nom du répertoire dans la table	
			string sql1="update "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".Directory ";
			sql1+="set DIRECTORY ='"+newName+"', DATE_MODIFICATION = SYSDATE ";
			sql1+="where ID_DIRECTORY ="+idDirectory+"";
			#endregion

			#region Execution de la requête
			try{
				webSession.Source.Update(sql1);
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible de Renommer un répertoire",err));
			}
			#endregion
		}

		/// <summary>
		///  Renomme une session
		/// </summary>
		/// <remarks>Testé</remarks>
		/// <param name="newName">Nouveau nom de la session</param>
		/// <param name="idMySession">Identifiant de la session</param>
		/// <param name="webSession">web Session</param>
		public static void RenameSession(string newName, Int64 idMySession, WebSession webSession) {
			
			#region requête
			string sql = "UPDATE " + TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA + ".MY_SESSION";
			sql += " SET MY_SESSION ='" + newName + "', DATE_MODIFICATION=sysdate ";
			sql += " WHERE ID_MY_SESSION=" + idMySession + "";			
			#endregion

			#region Execution de la requête
			try {
				webSession.Source.Update(sql);
			}
			catch (System.Exception err) {
				throw (new MyAdExpressDataAccessException("Impossible de Renommer une session sauvegardée", err));
			}
			#endregion
		}

		/// <summary>
		/// Supprime un répertoire dans la table Directory
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idDirectory">Identifiant Répertoire</param>
		/// <param name="webSession">webSession</param>
		public static void DropDirectory(Int64 idDirectory,WebSession webSession){

			#region requête
			string sql="delete from "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".DIRECTORY ";
			sql+=" where "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".DIRECTORY.ID_DIRECTORY="+idDirectory+"";
			#endregion

			#region Execution de la requête
			try{
				webSession.Source.Delete(sql);
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible de Supprimer un répertoire dans la table Directory",err));
			}
			#endregion
		}

		/// <summary>
		/// Déplace une session
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idOldDirectory">Identifiant du Répertoire source</param>
		/// <param name="idNewDirectory">Identifiant du Répertoire de destination</param>
		/// <param name="idMySession">Identifiant du résultat</param>
		/// <param name="webSession">Session du client</param>	
		public static void MoveSession(Int64 idOldDirectory,Int64 idNewDirectory,Int64 idMySession,WebSession webSession){
		
			#region requête
			string sql="UPDATE "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".MY_SESSION";
			sql+=" SET ID_DIRECTORY="+idNewDirectory+", DATE_MODIFICATION=sysdate ";
			sql+=" WHERE ID_DIRECTORY="+idOldDirectory+"";
			sql+=" and ID_MY_SESSION="+idMySession+"";
			#endregion

			#region Execution de la requête
			try{
				webSession.Source.Update(sql);
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible de Déplace une session",err));
			}
			#endregion
		}
	
		/// <summary>
		/// Retourne le nom d'une session à partir de son identifiant
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idSession">Identifiant de la session à charger</param>
		/// <param name="webSession">Session du client</param>
		/// <returns></returns>
		public static string GetSession(Int64 idSession,WebSession webSession){

			#region Requête
			string sql=" select MY_SESSION ";
			sql+=" FROM "+TNS.AdExpress.Constantes.DB.Schema.UNIVERS_SCHEMA+".MY_SESSION ";		
			sql +=" WHERE ID_MY_SESSION="+idSession+"";
			#endregion
			
			#region Execution de la requête
			try{
				return(webSession.Source.Fill(sql).Tables[0].Rows[0][0].ToString());
			}
			catch(System.Exception err){
				throw(new MyAdExpressDataAccessException("Impossible d'obtenir le nom d'une session à partir de son identifiant",err));
			}
			#endregion
		}

		#region Chargement des identifiants session
//		/// <summary>
//		/// Charge tous les identifiants de session 
//		/// </summary>
//		/// <returns>liste des identifiants de session</returns>
//		public static DataSet LoadSessionIds(WebSession webSession){
//			string sql="";
//			DataSet ds = null;
//
//			#region Ouverture de la base de données
//			bool DBToClosed=false;
//			
//			OracleConnection cnx = new OracleConnection(DBConstantes.Connection.SESSION_CONNECTION_STRING);
//
//	
//			if (cnx.State==System.Data.ConnectionState.Closed){
//				DBToClosed=true;
//				try{
//					cnx.Open();
//				}
//				catch(System.Exception e){
//					throw(new MySessionDataAccessException("WebSession.Load(...) : Impossible d'ouvrir la base de données : "+e.Message));
//				}
//			}
//			#endregion
//
//			#region chargement des identifiants session
//			OracleCommand sqlCommand=null;
//			OracleDataAdapter sqlAdapter=null;
//			try{				
//				sql+= " SELECT id_my_session,my_session FROM " + DBConstantes.Schema.UNIVERS_SCHEMA + "." + DBConstantes.Tables.MY_SESSION +" ORDER BY id_my_session"; 							
//				ds = new DataSet();				
//				sqlCommand=new OracleCommand(sql);
//				sqlCommand.Connection = cnx;
//				sqlAdapter=new OracleDataAdapter(sqlCommand);
//				sqlAdapter.Fill(ds);
//			}catch(System.Exception et){
//				try{
//					// Fermeture des structures
//					if(sqlAdapter!=null)sqlAdapter.Dispose();
//					if(sqlCommand!=null) sqlCommand.Dispose();
//					if (DBToClosed)cnx.Close();
//				}
//				catch(System.Exception e){
//					throw(new MySessionDataAccessException("WebSession.Load(...) : Impossible de libérer les ressources après échec de la méthode : "+e.Message));
//				}
//				throw(new MySessionDataAccessException("LoadSessionIds() : Impossible de charger les identifiants de session : "+et.Message));
//			}
//			try{
//				// Fermeture des structures
//				if(sqlAdapter!=null)sqlAdapter.Dispose();
//				if(sqlCommand!=null) sqlCommand.Dispose();
//				if (DBToClosed)cnx.Close();
//			}
//			catch(System.Exception ex){
//				throw(new MySessionDataAccessException("LoadSessionIds() : Impossible de fermer la base de données : "+ex.Message));
//			}
//			#endregion
//
//			return ds;
//		}

		#endregion
				
		#endregion
	}
}
