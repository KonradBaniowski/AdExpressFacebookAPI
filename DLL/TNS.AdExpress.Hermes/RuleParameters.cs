#region Informations
// Auteur : G.Facon, B.Masson
// Date de création : 12/02/2007
// Date de modification :
#endregion

#region Namespaces
using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

using TNS.AdExpress.Constantes.DB;
using HermesExceptions=TNS.AdExpress.Hermes.Exceptions;
using AnubisConstantes=TNS.AdExpress.Anubis.Constantes;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
#endregion

namespace TNS.AdExpress.Hermes {
	/// <summary>
	/// Rule used to monitor the media transfer.
	/// </summary>
	///<author>G. Facon</author>
	///<since>12/02/07</since>
	[System.Serializable]
	public class RuleParameters:Rule{

		#region Variables
		/// <summary>
		/// Day to monitor
		/// </summary>
		///<author>G. Facon</author>
		///<since>12/02/07</since>
		private DateTime _day;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rule">Based rule</param>
		/// <param name="day">Day to monitor</param>
		public RuleParameters(Rule rule, DateTime day){
			base._id = rule.Id;
			base._tableName = rule.TableName;
			base._mediaListId = rule.MediaListId;
			base._hourBegin = rule.HourBegin;
			base._hourEnd = rule.HourEnd;
			base._diffusionId = rule.DiffusionId;
			base._pluginId = rule.PluginId;
			_day = day;
		}
		#endregion

		#region Accessors
		/// <summary>
		/// Get Day
		/// </summary>
		public DateTime Day{
			get{return(_day);}
		}
		#endregion

		#region BLOB management

		#region Sauvegarde du BLOB
		/// <summary>
		/// Sauvegarde du BLOB dans la table static_nav_session
		/// </summary>
		/// <returns>Identifiant de la ligne à traiter</returns>
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
					throw(new HermesExceptions.RuleParametersException("Impossible d'ouvrir la base de données",et));
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
					
					" INSERT INTO " + Schema.UNIVERS_SCHEMA + "." + Tables.PDF_SESSION + "(id_login,id_static_nav_session,static_nav_session,id_pdf_result_type,pdf_user_filename,status,date_creation,date_modification) VALUES("+(Int64)base.Id+", :new_id, :blobtodb,"+base.PluginId.GetHashCode()+",'Indicators "+base.TableName.ToString()+"',0,sysdate,sysdate); " +
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
					throw(new HermesExceptions.RuleParametersException("AlertFactory.Save() : Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new HermesExceptions.RuleParametersException("AlertFactory.Save() : Echec de la sauvegarde de l'objet dans la base de donnée",e));
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
				throw(new HermesExceptions.RuleParametersException("Impossible de fermer la base de données",et));
			}
			#endregion
			
			return(idStaticNavSession);
		}
		#endregion

		#region Chargement du BLOB
		/// <summary>
		/// Chargement du BLOB de la table static_nav_session
		/// </summary>
		/// <param name="alertId">Identifiant de la ligne à charger</param>
		/// <returns>Objet</returns>
		public static Object Load(Int64 idStaticNavSession){

			#region Ouverture de la base de données
			//OracleConnection cnx = new OracleConnection(Connection.HERMES_CONNECTION_STRING);
            OracleConnection cnx =(OracleConnection)WebApplicationParameters.DataBaseDescription.GetDefaultConnection(TNS.AdExpress.Domain.DataBaseDescription.DefaultConnectionIds.hermes).GetSource(); 
			try{
				cnx.Open();
			}
			catch(System.Exception e){
				throw(new HermesExceptions.RuleParametersException("Impossible d'ouvrir la base de données",e));
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
					throw(new HermesExceptions.RuleParametersException("Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new HermesExceptions.RuleParametersException("Problème au chargement de la session à partir de la base de données",e));
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
				throw(new HermesExceptions.RuleParametersException("Impossible de fermer la base de données",et));
			}
			#endregion

			//retourne l'objet deserialized ou null si il y a eu un probleme
			return (o);
		}
		#endregion

		#endregion

	}
}
