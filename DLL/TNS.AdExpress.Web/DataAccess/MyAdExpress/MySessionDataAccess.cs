#region Informations
// Auteur: G. Facon 
// Date de création: 15/06/2004 
// Date de modification: 15/06/2004
// 11/08/2005	G. Facon	New Exception Management and property name
// 29/11/2005	B.Masson	webSession.Source
#endregion

using System;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.Web;
using DateFrameWork=TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Web.DataAccess.MyAdExpress{
	/// <summary>
	/// Gestion d'une sauvegarde de requête
	/// </summary>
	public class MySessionDataAccess{

		#region Variables
		/// <summary>
		/// Identifiant de la sauvegarde de session
		/// </summary>
		protected Int64 _idMySession=-1;
		/// <summary>
		/// Identifiant du répertoire
		/// </summary>
		protected Int64 _idDirectory=-1;
		/// <summary>
		/// Nom de la sauvegarde de session
		/// </summary>
		protected string _mySession="";
		/// <summary>
		/// Objet contenant les éléments sauvegardé
		/// </summary>
		protected WebSession _savedSession=null;
		/// <summary>
		/// Indique si la session est sauvegardée dans la base de données ou nom
		/// </summary>
		private bool _isSaved=false;
		/// <summary>
		/// Session du client
		/// </summary>
		private WebSession _webSession;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="idMySession">Identifiant de la sauvegarde</param>
		/// <param name="webSession">Session du client</param>
		public MySessionDataAccess(Int64 idMySession,WebSession webSession){
			_idMySession=idMySession;
			_webSession =webSession;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient l'identifiant de la sauvegarde
		/// </summary>
		public Int64 IdMySession{
			get{return(_idMySession);}
		}

		/// <summary>
		/// Obtient le nom de la sauvegarde
		/// </summary>
		public string MySession{
			get{return(_mySession);}
		}

		/// <summary>
		/// Obtient l'élément sauvegardé
		/// </summary>
		public WebSession SavedSession{
			get{return(_savedSession);}
		}

		/// <summary>
		/// Indique si la session est sauvegardée dans la base de données
		/// </summary>
		public bool IsSaved{
			get{return(_isSaved);}
		}
		#endregion

		#region Sauvegarde d'une nouvelle session
		/// <summary>
		/// Sauvegarde d'une nouvelle session
		/// </summary>
		/// <param name="idDirectory">Identifiant du répertoire de sauvegarde</param>
		/// <param name="mySession">Nom de la session</param>
		/// <param name="webSession">Session du client</param>
		public static bool SaveMySession(Int64 idDirectory,string mySession,WebSession webSession){

			#region Ouverture de la base de données
			OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
			bool DBToClosed=false;
			bool success=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception e){
					throw(new MySessionDataAccessException("Impossible d'ouvrir la base de données :"+e.Message));
				}
			}
			#endregion
			
			#region Sérialisation et sauvegarde de la session
			OracleCommand sqlCommand=null;
			MemoryStream ms=null;
			BinaryFormatter bf=null;
			byte[] binaryData=null;
			

			try{
				Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);
				Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
				//"Serialization"
				ms = new MemoryStream();
				bf = new BinaryFormatter();
				bf.Serialize(ms,webSession);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

				//create anonymous PL/SQL command
				string block = " BEGIN "+
					" INSERT INTO " + mySessionTable.Sql +
					" (ID_MY_SESSION, ID_DIRECTORY, MY_SESSION, BLOB_CONTENT, DATE_CREATION, DATE_MODIFICATION, ACTIVATION) "+
					" VALUES "+
					" (" + schema.Label + ".seq_my_session.nextval, " + idDirectory + ", '" + mySession + "', :1, sysdate, sysdate," + DBConstantes.ActivationValues.ACTIVATED + "); " +
					" END; ";
				sqlCommand = new OracleCommand(block);
				sqlCommand.Connection = connection;
				sqlCommand.CommandType = CommandType.Text;
				//Fill parametres
				OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
				param.Direction = ParameterDirection.Input;
				param.Value = binaryData;
				
				//Execute PL/SQL block
				sqlCommand.ExecuteNonQuery();

			}
			#endregion

			#region Gestion des erreurs dues à la sérialisation et à la sauvegarde de l'objet
			catch(System.Exception e){
				// Fermeture des structures
				try{
					if (ms != null) ms.Close();
					if (bf != null) bf=null;
					if(sqlCommand!=null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et) {
					throw(new MySessionDataAccessException("WebSession.Save() : Impossible de libérer les ressources après échec de la méthode : "+et.Message));
				}
				throw(new MySessionDataAccessException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donnée : "+e.Message));
			}
			//pas d'erreur
			try{
				// Fermeture des structures
				ms.Close();
				bf=null;
				if(sqlCommand!=null) sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
				success=true;
			}
			catch(System.Exception et){
				throw(new MySessionDataAccessException("WebSession.Save() : Impossible de fermer la base de données : "+et.Message));
			}
			#endregion

			return(success);
		}
		#endregion

		/// <summary>
		/// Mise à jour d'une session
		/// </summary>
		/// <param name="idDirectory">Identifiant du répertoire de sauvegarde</param>
		/// <param name="idMySession">Id session</param>
		/// <param name="mySession">Nom de la session</param>
		/// <param name="webSession">Session du client</param>
		public static bool UpdateMySession(Int64 idDirectory, string idMySession ,string mySession, WebSession webSession) {
			
			#region Ouverture de la base de données
			OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
			bool DBToClosed = false;
			bool success = false;
			if (connection.State == System.Data.ConnectionState.Closed) {
				DBToClosed = true;
				try {
					connection.Open();
				}
				catch (System.Exception e) {
					throw (new MySessionDataAccessException("Impossible d'ouvrir la base de données :" + e.Message));
				}
			}
			#endregion

			#region Sérialisation et Mise à jour de la session
			OracleCommand sqlCommand = null;
			MemoryStream ms = null;
			BinaryFormatter bf = null;
			byte[] binaryData = null;

			try {
				Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);
				//"Serialization"
				ms = new MemoryStream();
				bf = new BinaryFormatter();
				bf.Serialize(ms, webSession);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

				//mise à jour de la session
				string sql = " BEGIN " +
					" UPDATE " + mySessionTable .Sql +
					" SET BLOB_CONTENT = :1,ID_DIRECTORY=" + idDirectory + ",MY_SESSION='" + mySession + "',DATE_MODIFICATION=sysdate" +
					" WHERE ID_MY_SESSION=" + idMySession + " ;" +
					" END; ";
				//Exécution de la requête
				sqlCommand = new OracleCommand(sql);
				sqlCommand.Connection = connection;
				sqlCommand.CommandType = CommandType.Text;
				//parametres
				OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
				param.Direction = ParameterDirection.Input;
				param.Value = binaryData;
				//Execution PL/SQL block
				sqlCommand.ExecuteNonQuery();

			}
			#endregion

			#region Gestion des erreurs dues à la sérialisation et à la sauvegarde de l'objet
			catch (System.Exception e){
				// Fermeture des structures
				try{
					if (ms != null) ms.Close();
					if (bf != null) bf=null;
					if(sqlCommand!=null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et) {
					throw(new MySessionDataAccessException("UpdateMySession : Impossible de libérer les ressources après échec de la méthode : "+et.Message));
				}
				throw(new MySessionDataAccessException("UpdateMySession : Echec de la sauvegarde de l'objet dans la base de donnée : "+e.Message));
			}
			//pas d'erreur
			try{
				// Fermeture des structures
				ms.Close();
				bf=null;
				if(sqlCommand!=null) sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
				success=true;
			}
			catch(System.Exception et){
				throw(new MySessionDataAccessException("UpdateMySession : Impossible de fermer la base de données : "+et.Message));
			}
			#endregion

			return (success);
		}

		#region Action sur la sauvegarde

		/// <summary>
		/// Suppression de la sauvegarde
		/// </summary>
		public bool Delete(){
			bool isValid = false;
			if(_isSaved) throw(new MySessionDataAccessException("La sauvegarde de session n'est pas indiquée comme présente dans la base de données"));
			if(_idMySession<0) throw(new MySessionDataAccessException("L'identifiant de la sauvegarde de session n'est pas défini"));

			#region Construction de la requête SQL
			Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

			string sql="delete from "+mySessionTable.Sql;
			sql+=" where id_my_session="+_idMySession;
			#endregion

			#region Execution de la requête
			try{
				_webSession.Source.Delete(sql);
				isValid=true;
			}
			catch(System.Exception err){
				throw(new MySessionDataAccessException("Impossible de supprimer la sauvegarde",err)); 
			}
			#endregion			

			_isSaved=true;
			return isValid ;
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Vérifie si la sauvegarde est dans la base de données
		/// </summary>
		/// <returns>True si la sauvegarde est présente, false sinon</returns>
		public bool CheckInDataBase(){
			bool found=false;
			if(_idMySession<0) throw(new MySessionDataAccessException("L'identifiant de la sauvegarde de session n'est pas défini"));
			Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);			

			#region Construction de la requête SQL
			string sql = "select count(rownum) as nb from " + mySessionTable.Sql;
			sql+=" where id_my_session="+_idMySession.ToString();
			sql+=" and activation<"+DBConstantes.ActivationValues.UNACTIVATED;
			#endregion

			#region Execution de la requête
			try{
				DataSet ds = _webSession.Source.Fill(sql);
				if(Int64.Parse(ds.Tables[0].Rows[0]["nb"].ToString())>0) found=true;
			}
			catch(System.Exception err){
				throw(new MySessionDataAccessException("Impossible de déplacer la sauvegarde vers un nouveau répertoire",err)); 
			}
			#endregion			

			return(found);
		}
		#endregion

		#region Récupération d'un session
		/// <summary>
		/// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des sessions
		/// </summary>
		/// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
		/// <param name="idWebSession">Identifiant de la session</param>
		/// <param name="webSession">Session à la connexion d l'utilisateur</param>
		public static Object GetResultMySession(string idWebSession,WebSession webSession){
			
			#region Ouverture de la base de données
			bool DBToClosed=false;
			OracleConnection cnx = (OracleConnection) webSession.Source.GetSource();

			if (cnx.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					cnx.Open();
				}
				catch(System.Exception e){
					throw(new MySessionDataAccessException("WebSession.Load(...) : Impossible d'ouvrir la base de données",e));
				}
			}
			#endregion
			
			#region Chargement et deserialization de l'objet
			OracleCommand sqlCommand=null;
			MemoryStream ms=null;
			BinaryFormatter bf=null;
			byte[] binaryData=null;
			Object o = null;
			try{
				Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

				binaryData = new byte[0];
				//create anonymous PL/SQL command
				string block = " BEGIN "+
					" SELECT Blob_content INTO :1 FROM " + mySessionTable.Sql + " WHERE id_my_session = " + idWebSession + "; " +
					" END; ";
				sqlCommand = new OracleCommand(block);
				sqlCommand.Connection = cnx;
				sqlCommand.CommandType = CommandType.Text;
				//Initialize parametres
				OracleParameter param = sqlCommand.Parameters.Add("blobfromdb1", OracleDbType.Blob);
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
					if (DBToClosed) cnx.Close();
				}
				catch(System.Exception et){
					throw(new MySessionDataAccessException("WebSession.Load(...) : Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new MySessionDataAccessException("WebSession.Load(...) : Problème au chargement de la session à partir de la base de données",e));
			}
			try{
				// Fermeture des structures
				if (ms != null) ms.Close();
				if (bf != null) bf=null;
				if (binaryData != null) binaryData=null;
				if(sqlCommand!=null) sqlCommand.Dispose();
				if (DBToClosed) cnx.Close();
			}
			catch(System.Exception et){
				throw(new MySessionDataAccessException("WebSession.Load(...) : Impossible de fermer la base de données",et));
			}
			#endregion
			
			//retourne l'objet deserialized ou null si il y a eu un probleme
			return(o);
		}
		#endregion

	}
}

