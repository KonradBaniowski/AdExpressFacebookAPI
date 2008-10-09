#region Informations
// Auteur: B. Masson, D.Mussuma
// Date de création: 16/11/2005
// Date de modification: 
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
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using DbSchemas=TNS.AdExpress.Constantes.DB.Schema;
using TNS.AdExpress.Bastet.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;


namespace TNS.AdExpress.Bastet.Common{
	/// <summary>
	/// La classe Parameters comprend tous les paramètres nécessaires à la génération des statistiques d'AdExpress.
	/// </summary>
	[System.Serializable]
	public class Parameters {
		
		#region variables
		/// <summary>
		/// Source de données
		/// </summary>
		protected IDataSource _source = null;
		/// <summary>
		/// Identifiant du login qui fait la demande
		/// </summary>
		protected Int64 _loginId;
		/// <summary>
		/// Date de début
		/// </summary>
		protected string _periodBeginningDate;
		/// <summary>
		/// Date de fin
		/// </summary>
		protected string _periodEndDate;
		/// <summary>
		/// Logins clients
		/// </summary>
		protected string _logins;
		/// <summary>
		/// Emails destinataires
		/// </summary>
		protected ArrayList _emailsRecipient = new ArrayList();
		/// <summary>
		/// Nom du fichier Excel
		/// </summary>
		protected string _exportExcelFileName;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public Parameters(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="loginId">Identifiant du login qui fait la demande</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="loginsList">Logins clients</param>
		/// <param name="emails">Emails destinataires</param>
		public Parameters(IDataSource source, Int64 loginId, string dateBegin, string dateEnd, string loginsList, ArrayList emails){
			_source = source;
			_loginId = loginId;
			_periodBeginningDate = dateBegin;
			_periodEndDate = dateEnd;
			_logins = loginsList;
			_emailsRecipient = emails;
			_exportExcelFileName = "Tracking_"+dateBegin+"-"+dateEnd;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou défini la date de début
		/// </summary>
		public string PeriodBeginningDate{
			get{return _periodBeginningDate;}
			set{_periodBeginningDate = value;}
		}

		/// <summary>
		/// Obtient ou défini la date de fin
		/// </summary>
		public string PeriodEndDate{
			get{return _periodEndDate;}
			set{_periodEndDate = value;}
		}

		/// <summary>
		/// Obtient ou défini la liste des logins
		/// </summary>
		public string Logins{
			get{return _logins;}
			set{_logins = value;}
		}

		/// <summary>
		/// Obtient ou défini la liste des emails destinataires
		/// </summary>
		public ArrayList EmailsRecipient{
			get{return _emailsRecipient;}
			set{_emailsRecipient = value;}
		}

		/// <summary>
		/// Obtient ou défini la source de données
		/// </summary>
		public IDataSource Source{
			get{return _source;}
			set{_source = value;}
		}
		#endregion

		#region Blob
		
		#region Chargement et sauvegarde des sessions statiques
		/// <summary>
		/// Sauvegarde des paramètres d'un résultat PDF à partir de la base de données
		/// </summary>
		/// <param name="webSession">Session client à sauvegarder</param>
		/// <param name="resultType">Type de résultat</param>
		/// <returns>Identifiant correspondant à la session sauvegardée</returns>
		public Int64 Save(){
			
			// test que tous les paramètres soient non null et vide !!!!!!!!!!!!!!!

			Int64 idStaticNavSession=-1;
			OracleConnection connection = (OracleConnection)_source.GetSource();

			#region Ouverture de la base de données
			bool DBToClosed=false;
			// On teste si la base est déjà ouverte
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception et){
					throw(new ParametersException("Impossible d'ouvrir la base de données",et));
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
					
					" INSERT INTO " + Schema.UNIVERS_SCHEMA + "." + Tables.PDF_SESSION + "(id_login,id_static_nav_session, static_nav_session,id_pdf_result_type,pdf_user_filename,status,date_creation,date_modification) VALUES("+_loginId+", :new_id, :blobtodb,"+AnubisConstantes.Result.type.bastet.GetHashCode()+",'"+ _exportExcelFileName +"',0,sysdate,sysdate); " +
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
					throw(new ParametersException("WebSession.Save() : Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new ParametersException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donnée",e));
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
				throw(new ParametersException ("Impossible de fermer la base de données",et));
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
			IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(TNS.AdExpress.Domain.DataBaseDescription.DefaultConnectionIds.webAdministration);
			//OracleConnection cnx = new OracleConnection(Connection.SESSION_CONNECTION_STRING_TEST);
			OracleConnection cnx = (OracleConnection)dataSource.GetSource();
			try{
				cnx.Open();
				
			}
			catch(System.Exception e){
				throw(new ParametersException("Impossible d'ouvrir la base de données",e));
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
					throw(new ParametersException("Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new ParametersException("Problème au chargement de la session à partir de la base de données",e));
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
				throw(new ParametersException("Impossible de fermer la base de données",et));
			}
			#endregion
			
			//retourne l'objet deserialized ou null si il y a eu un probleme
			return(o);
		}
		#endregion

		#endregion


	}
}
