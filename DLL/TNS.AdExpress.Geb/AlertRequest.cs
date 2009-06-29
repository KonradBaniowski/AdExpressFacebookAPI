#region Informations
// Auteur : B.Masson
// Date de création : 14/04/2006
// Date de modification :
#endregion

using System;
using System.Collections;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using AnubisConstantes=TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using GebExceptions=TNS.AdExpress.Geb.Exceptions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Geb{
	/// <summary>
	/// Classe pour la sauvegarde et le chargement d'une alerte (BLOB)
	/// </summary>
	[System.Serializable]
	public class AlertRequest{

		const Int64 ALERT_TYPE_ID = 1;

		#region Enumérateurs
		/// <summary>
		/// Enumérateurs des paramètres
		/// </summary>
		enum Parameters{
			alertId = 0,
			alertTypeId = 1,
			dateMediaNum = 2
		}
		#endregion
		
		#region variables
		/// <summary>
		/// Paramètres d'une alerte :
		/// - Identifiant de l'alerte
		/// - identifiant du type d'alerte
		/// - Date du support
		/// </summary>
		protected Hashtable _alertParameters = new Hashtable();
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public AlertRequest(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="alertId">Identifiant de l'alerte</param>
		/// <param name="dateMediaNum">Date de pige du support</param>
		public AlertRequest(Int64 alertId, string dateMediaNum){
			_alertParameters.Add(Parameters.alertId, alertId);
			_alertParameters.Add(Parameters.alertTypeId, ALERT_TYPE_ID);
			_alertParameters.Add(Parameters.dateMediaNum, dateMediaNum);

			// Controle des paramètres
//			string a = _alertParameters[Parameters.alertId].ToString();
//			string b = _alertParameters[Parameters.alertTypeId].ToString();
//			string c = _alertParameters[Parameters.dateMediaNum].ToString();
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient l'identifiant de l'alerte
		/// </summary>
		public Int64 AlertId{
			get{
				if(_alertParameters[Parameters.alertId]==null)
					throw(new GebExceptions.AlertRequestException("Erreur de paramètre : L'identifiant de l'alerte est null"));
				return (Int64)_alertParameters[Parameters.alertId];
			}
		}

		/// <summary>
		/// Obtient l'identifiant du type d'alerte
		/// </summary>
		public Int64 AlertTypeId{
			get{
				if(_alertParameters[Parameters.alertTypeId]==null)
					throw(new GebExceptions.AlertRequestException("Erreur de paramètre : L'identifiant du type d'alerte est null"));
				return (Int64)_alertParameters[Parameters.alertTypeId];
			}
		}

		/// <summary>
		/// Obtient la date de pige du support
		/// </summary>
		public string DateMediaNum{
			get{
				if(_alertParameters[Parameters.dateMediaNum]==null)
					throw(new GebExceptions.AlertRequestException("Erreur de paramètre : La date du support est null"));
				return _alertParameters[Parameters.dateMediaNum].ToString();
			}
		}
		#endregion

		#region Gestion du BLOB

		#region Sauvegarde du BLOB dans la table
		/// <summary>
		/// Sauvegarde du BLOB dans la table static_nav_session
		/// </summary>
		/// <returns>Identifiant correspondant à l'alerte sauvegardé</returns>
		public Int64 Save(IDataSource source){

			#region Variables
			Int64 idStaticNavSession=-1;
			OracleConnection connection = (OracleConnection)source.GetSource();
			#endregion

			#region Ouverture de la base de données
			bool DBToClosed=false;
			// On teste si la base est déjà ouverte
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception et){
					throw(new GebExceptions.AlertRequestException("Impossible d'ouvrir la base de données",et));
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
				bf.Serialize(ms,this);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

				//create anonymous PL/SQL command
				string block = ""+
					" BEGIN "+
					" Select " + Schema.UNIVERS_SCHEMA + ".SEQ_STATIC_NAV_SESSION.NEXTVAL into :new_id from dual;"+
					
					" INSERT INTO " + Schema.UNIVERS_SCHEMA + "." + Tables.PDF_SESSION + "(id_login,id_static_nav_session, static_nav_session,id_pdf_result_type,pdf_user_filename,status,date_creation,date_modification) VALUES("+(Int64)_alertParameters[Parameters.alertId]+", :new_id, :blobtodb,"+AnubisConstantes.Result.type.gebAlert.GetHashCode()+",'Alerte GEB "+_alertParameters[Parameters.alertId].ToString()+"',0,sysdate,sysdate); " +
					" END; ";
				sqlCommand = new OracleCommand(block);
				sqlCommand.Connection =  connection;
				sqlCommand.CommandType = CommandType.Text;			

				//Fill parametres
				OracleParameter param2 = sqlCommand.Parameters.Add("new_id",OracleDbType.Int64);
				param2.Direction = ParameterDirection.Output;
				OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
				param.Direction = ParameterDirection.Input;
				param.Value = binaryData;
				
				//Execute PL/SQL block
				sqlCommand.ExecuteNonQuery();
				idStaticNavSession=(Int64)param2.Value;
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
					throw(new GebExceptions.AlertRequestException("AlertFactory.Save() : Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new GebExceptions.AlertRequestException("AlertFactory.Save() : Echec de la sauvegarde de l'objet dans la base de donnée",e));
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
				throw(new GebExceptions.AlertRequestException("Impossible de fermer la base de données",et));
			}
			#endregion
			
			return(idStaticNavSession);
		}
		#endregion

		#region Chargement du BLOB de la table
		/// <summary>
		/// Chargement du BLOB de la table static_nav_session
		/// </summary>
		/// <param name="alertId">Identifiant de l'alerte</param>
		/// <returns></returns>
		public static Object Load(Int64 idStaticNavSession){

			#region Ouverture de la base de données

            OracleConnection cnx =(OracleConnection)WebApplicationParameters.DataBaseDescription.GetDefaultConnection(TNS.AdExpress.Domain.DataBaseDescription.DefaultConnectionIds.geb).GetSource();
            //    new OracleConnection(Connection.GEB_CONNECTION_STRING);
			try{
				cnx.Open();
			}
			catch(System.Exception e){
				throw(new GebExceptions.AlertRequestException("Impossible d'ouvrir la base de données",e));
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
					" SELECT static_nav_session INTO :1 FROM " + Schema.UNIVERS_SCHEMA + "." + Tables.PDF_SESSION+ " WHERE id_static_nav_session = " + idStaticNavSession.ToString() + "; " +
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
					throw(new GebExceptions.AlertRequestException("Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new GebExceptions.AlertRequestException("Problème au chargement de la session à partir de la base de données",e));
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
				throw(new GebExceptions.AlertRequestException("Impossible de fermer la base de données",et));
			}
			#endregion

			//retourne l'objet deserialized ou null si il y a eu un probleme
			return (o);
		}
		#endregion

		#endregion

	}
}
