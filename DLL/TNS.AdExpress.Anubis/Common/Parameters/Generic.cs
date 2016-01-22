#region Informations
// Auteur: G. Facon
// Date de cr�ation: 26/05/2005
// Date de modification: 26/05/2005
#endregion

using System;
using TNS.AdExpress.Anubis.Constantes;
//using TNS.AdExpress.
//using AdExpressDBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Anubis.Common.Parameters{

	/// <summary>
	/// Param�tres Client g�n�rique pour construire un resultat PDF
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
		/// Type de resultat PDF � g�n�rer
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
		/// <param name="resultType">Type de r�sultat</param>
		public Generic(Result.type resultType){
			_resultType=resultType;

		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou d�finit l'identifiant de session
		/// </summary>
		public string IdSession{
			get {return _idSession;}
			set {_idSession = value;}
		}

		/// <summary>
		/// Obtient ou d�finit la langue choisie par l'utilisateur
		/// </summary>
		public int SiteLanguage{
			get{return(_siteLanguage);}
			set{_siteLanguage=value;}
		}

		/// <summary>
		/// Obtient le type de resultats pour g�n�rer le PDF
		/// </summary>
		public Result.type ResultType{
			get{return(_resultType);}
		}

		#endregion

		#region m�thode externes
		/*
		#region Blob
		/// <summary>
		/// M�thode qui sauvegarde l'objet courant dans la table de sauvegarde des r�sultats PDF
		/// </summary>
		public void Save(){
			//string login=customerLogin.

			#region Ouverture de la base de donn�es
			OracleConnection connection = new OracleConnection(Connection.SESSION_CONNECTION_STRING);
			try{
				connection.Open();
			}
			catch(System.Exception e){
				throw(new WebSessionException("WebSession.Save() : Impossible d'ouvrir la base de donn�es"+e.Message));
			}
			#endregion
			
			#region S�rialisation et sauvegarde de la session
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

				#region Gestion des erreurs dues � la s�rialisation et � la sauvegarde de l'objet
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
					throw(new WebSessionException("WebSession.Save() : Impossible de lib�rer les ressources apr�s �chec de la m�thode : "+et.Message));
				}
				throw(new WebSessionException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donn�e : "+e.Message));
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
				throw(new WebSessionException("WebSession.Save() : Impossible de fermer la base de donn�es : "+et.Message));
			}

			#endregion

		}
		
		/*	
		/// <summary>
		/// M�thode pour la r�cup�ration et la "deserialization" d'un objet WebSession � partir du champ BLOB de la table des sessions
		///		Ouverture de la BD
		///		Requ�te BD de s�lection d'un un Blob
		///		D�s�rialisation
		///		Lib�ration des ressources
		/// </summary>
		/// <returns>Retourne l'objet r�cup�r� ou null si il y a eu un probl�me non g�r�</returns>
		/// <param name="idWebSession">Identifiant de la session</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.WebSessionException">
		/// Lanc�e dans les cas suivant:
		///		connection � la BD impossible � ouvrir
		///		probl�me � la s�lection de l'enregistrement ou � la d�s�rialisation
		///		impossible de lib�rer les ressources
		/// </exception>
		public static Object Load(string idWebSession){
			
			#region Ouverture de la base de donn�es
			OracleConnection cnx = new OracleConnection(Connection.SESSION_CONNECTION_STRING);
			try{
				cnx.Open();
			}
			catch(System.Exception e){
				throw(new WebSessionException("WebSession.Load(...) : Impossible d'ouvrir la base de donn�es : "+e.Message));
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
				//R�cup�ration des octets du blob
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
					throw(new WebSessionException("WebSession.Load(...) : Impossible de lib�rer les ressources apr�s �chec de la m�thode : "+et.Message));
				}
				throw(new WebSessionException("WebSession.Load(...) : Probl�me au chargement de la session � partir de la base de donn�es : "+e.Message+" "+i.ToString()));
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
				throw(new WebSessionException("WebSession.Load(...) : Impossible de fermer la base de donn�es : "+et.Message));
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
