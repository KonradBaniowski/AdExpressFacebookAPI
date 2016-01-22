#region Informations
// Auteur: G. Facon
// Date de création: 26/05/2005
// Date de modification: 26/05/2005
#endregion

using System;
using TNS.AdExpress.Anubis.Constantes;
//using TNS.AdExpress.
//using AdExpressDBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Anubis.Common.Parameters{

	/// <summary>
	/// Paramètres Client générique pour construire un resultat PDF
	/// </summary>
	[System.Serializable]
	public class Generic{

		#region Variables
		/// <summary>
		/// Identifiant de session
		/// </summary>
		protected string _idSession;
		/// <summary>
		/// Langage du site
		/// </summary>
		protected int _siteLanguage=TNS.AdExpress.Constantes.DB.Language.FRENCH;
		/// <summary>
		/// Type de resultat PDF à générer
		/// </summary>
		protected Result.type _resultType;
		///// <summary>
		///// Droits client
		///// </summary>
		//protected WebRightRules _customerLogin = null;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="resultType">Type de résultat</param>
		public Generic(Result.type resultType){
			_resultType=resultType;

		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit l'identifiant de session
		/// </summary>
		public string IdSession{
			get {return _idSession;}
			set {_idSession = value;}
		}

		/// <summary>
		/// Obtient ou définit la langue choisie par l'utilisateur
		/// </summary>
		public int SiteLanguage{
			get{return(_siteLanguage);}
			set{_siteLanguage=value;}
		}

		/// <summary>
		/// Obtient le type de resultats pour générer le PDF
		/// </summary>
		public Result.type ResultType{
			get{return(_resultType);}
		}

		#endregion

		#region méthode externes
		/*
		#region Blob
		/// <summary>
		/// Méthode qui sauvegarde l'objet courant dans la table de sauvegarde des résultats PDF
		/// </summary>
		public void Save(){
			//string login=customerLogin.

			#region Ouverture de la base de données
			OracleConnection connection = new OracleConnection(Connection.SESSION_CONNECTION_STRING);
			try{
				connection.Open();
			}
			catch(System.Exception e){
				throw(new WebSessionException("WebSession.Save() : Impossible d'ouvrir la base de données"+e.Message));
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
				string block = " BEGIN "+
					" DELETE " + Schema.APPLICATION_SCHEMA + "." + Tables.TABLE_SESSION + " WHERE ID_NAV_SESSION=" + this.idSession + "; " +
					" INSERT INTO " + Schema.APPLICATION_SCHEMA + "." + Tables.TABLE_SESSION + "(id_nav_session, nav_session) VALUES("+ this.idSession + ", :1); " +
					" END; ";
				sqlCommand = new OracleCommand(block);
				sqlCommand.Connection = cnx;
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
					cnx.Close();
					cnx.Dispose();
				}
				catch(System.Exception et) {
					throw(new WebSessionException("WebSession.Save() : Impossible de libérer les ressources après échec de la méthode : "+et.Message));
				}
				throw(new WebSessionException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donnée : "+e.Message));
			}
			//pas d'erreur
			try{
				// Fermeture des structures
				ms.Close();
				bf=null;
				sqlCommand.Dispose();
				cnx.Close();
				cnx.Dispose();
			}
			catch(System.Exception et){
				throw(new WebSessionException("WebSession.Save() : Impossible de fermer la base de données : "+et.Message));
			}

			#endregion

		}
		
		/*	
		/// <summary>
		/// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des sessions
		///		Ouverture de la BD
		///		Requête BD de sélection d'un un Blob
		///		Désérialisation
		///		Libération des ressources
		/// </summary>
		/// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
		/// <param name="idWebSession">Identifiant de la session</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.WebSessionException">
		/// Lancée dans les cas suivant:
		///		connection à la BD impossible à ouvrir
		///		problème à la sélection de l'enregistrement ou à la désérialisation
		///		impossible de libérer les ressources
		/// </exception>
		public static Object Load(string idWebSession){
			
			#region Ouverture de la base de données
			OracleConnection cnx = new OracleConnection(Connection.SESSION_CONNECTION_STRING);
			try{
				cnx.Open();
			}
			catch(System.Exception e){
				throw(new WebSessionException("WebSession.Load(...) : Impossible d'ouvrir la base de données : "+e.Message));
			}			
			#endregion
			
			#region Chargement et deserialization de l'objet
			OracleCommand sqlCommand=null;
			MemoryStream ms=null;
			BinaryFormatter bf=null;
			byte[] binaryData=null;
			Object o = null;
			int i=0;
			try{
				binaryData = new byte[0];
				i=1;
				//create anonymous PL/SQL command
				string block = " BEGIN "+
					" SELECT nav_session INTO :1 FROM " + Schema.APPLICATION_SCHEMA + "." + Tables.TABLE_SESSION + " WHERE id_nav_session = " + idWebSession + "; " +
					" END; ";
				i=2;
				sqlCommand = new OracleCommand(block);
				i=3;
				sqlCommand.Connection = cnx;
				i=4;
				sqlCommand.CommandType = CommandType.Text;
				i=5;
				//Initialize parametres
				OracleParameter param = sqlCommand.Parameters.Add("blobfromdb", OracleDbType.Blob);
				i=6;
				param.Direction = ParameterDirection.Output;
				i=7;

				//Execute PL/SQL block
				sqlCommand.ExecuteNonQuery();
				i=8;
				//Récupération des octets du blob
				binaryData = (byte[]) ((OracleBlob)(sqlCommand.Parameters[0].Value)).Value;
				i=9;
				
				//Deserialization oft the object
				ms = new MemoryStream();
				i=10;
				ms.Write(binaryData, 0, binaryData.Length);
				i=11;
				bf=new BinaryFormatter();
				i=12;
				ms.Position = 0;
				i=13;
				o = bf.Deserialize(ms);
				i=14;
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
					throw(new WebSessionException("WebSession.Load(...) : Impossible de libérer les ressources après échec de la méthode : "+et.Message));
				}
				throw(new WebSessionException("WebSession.Load(...) : Problème au chargement de la session à partir de la base de données : "+e.Message+" "+i.ToString()));
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
				throw(new WebSessionException("WebSession.Load(...) : Impossible de fermer la base de données : "+et.Message));
			}
			#endregion
			
			//retourne l'objet deserialized ou null si il y a eu un probleme
			return(o);
		}	
		#endregion
		

		#endregion
		*/
		#endregion
	}
}
