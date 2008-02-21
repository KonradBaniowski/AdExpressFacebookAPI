#region Informations
// Auteur: G. Facon
// Date de création: 29/06/2005
// Date de modification:
#endregion

using System;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Exceptions;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Common{
	/// <summary>
	/// Source de données Oracle
	/// </summary>
	[System.Serializable]
	public class _OracleDataSource:_IDataSource,IDisposable{

		#region Variables
		/// <summary>
		/// Source de données
		/// </summary>
		[System.NonSerialized]protected OracleConnection _source=null;
		/// <summary>
		/// Type de la source de données
		/// </summary>
		protected DataSource.Type _sourceType=DataSource.Type.oracle;
		/// <summary>
		/// Indique si dispose a été appellée
		/// </summary>
		private bool _disposed=false;
		/// <summary>
		/// Chaine de connection
		/// </summary>
		private string _connectionString;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Source de données</param>
		public _OracleDataSource(OracleConnection source){
			if(source==null)throw(new NullReferenceException("La source de données est NULL"));
			_source=source;
			_connectionString=source.ConnectionString;
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="connectionString">Chaîne de connexion à la source de données</param>
		public _OracleDataSource(string connectionString){
			if(connectionString==null)throw(new NullReferenceException("La chaîne de connxion à la source de données est NULL"));
			if(connectionString.Length==0)throw(new ArgumentException("La chaîne de connxion à la source de données est vide"));
			try{
				_connectionString=connectionString;
				_source=new OracleConnection(connectionString);
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException("Impossible de créer la source de données",err));
			}
		}

		#endregion

		#region Destructeur
		/// <summary>
		/// Destructeur
		/// </summary>
		public void Dispose(){
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Destructeur
		/// </summary>
		/// <param name="disposing">booleen</param>
		protected virtual void Dispose(bool disposing){
			if(!_disposed){
				if(disposing){
					_source.Dispose();
					_source=null;
				}
			}
			_disposed=true;
		}


		#endregion

		#region Accesseurs
		/// <summary>
		/// Accès à la source de données
		/// </summary>
		private OracleConnection Source{
			get{
				try{
					if(_source==null)BuildConnection();
					return(_source);
				}
				catch(System.Exception err){
					throw(err);
				}
			}
		}
		#endregion

		#region Méthodes Internes

		#region Création de la connexion Oracle
		/// <summary>
		/// Construit la connection Oracle
		/// </summary>
		private void BuildConnection(){
			try{
				_source=new OracleConnection(_connectionString);
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException("Impossible de créer la connection Oracle",err));
			}
		
		}
		#endregion

		#region Sélection d'éléments
		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		/// <param name="afterError">true si l'execution est demandé après une erreur, false sinon</param>
		/// <returns>Resultat</returns>
		private DataSet Fill(string command,bool afterError){
			
			#region Variables
			OracleCommand sqlCommand=null;
			OracleDataAdapter sqlAdapter=null;
			DataSet ds=new DataSet();
			#endregion

			if(afterError)BuildConnection();

			#region Ouverture de la base de données
			bool DBToClosed=false;
			if (Source.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					Open();
				}
				catch(System.Exception err){
					if(!afterError && err.Message.IndexOf("ORA-03113")>=0){
						try{
							return(Fill(command,true));
						}
						catch(System.Exception err2){
							throw(new OracleDataSourceException("Erreur après réexecution",err2));
						}
					}
					throw(new OracleDataSourceException("Impossible d'ouvrir la base de données",err));
				}
			}
			#endregion

			#region Execution de la requête
			try{
				sqlCommand=new OracleCommand(command,Source);
				sqlAdapter=new OracleDataAdapter(sqlCommand);
				sqlAdapter.Fill(ds);
			}
			#region Traitement d'erreur du chargement des données
			catch(System.Exception ex){
				try{
					// Fermeture de la base de données
					if (sqlAdapter!=null)sqlAdapter.Dispose();
					if(sqlCommand!=null) sqlCommand.Dispose();
					if (DBToClosed) Close();
				}
				catch(System.Exception et){
					throw(new OracleDataSourceException ("Impossible de fermer la base de données, lors de la gestion d'erreur de l'execution de la requête",et));
				}
				try{
					if(!afterError && ex.Message.IndexOf("ORA-03113")>=0)return(Fill(command,true));
				}
				catch(System.Exception err2){
					throw(new OracleDataSourceException("Erreur après réexecution",err2));
				}
				throw(new OracleDataSourceException ("Impossible d'executer la requête",ex));
			}
			#endregion

			#endregion

			#region Fermeture de la base de données
			try{
				// Fermeture de la base de données
				if (sqlAdapter!=null)sqlAdapter.Dispose();
				if(sqlCommand!=null)sqlCommand.Dispose();
				if (DBToClosed) Close();
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException ("Impossible de fermer la base de données",err));
			}
			#endregion

			return(ds);
		}
		#endregion

		#region NonQuery
		/// <summary>
		/// Requête de type ExecuteNonQuery
		/// </summary>
		/// <param name="command">Requête SQL</param>
		private void NonQuery(string command){

			#region Variables
			OracleCommand sqlCommand=null;
			#endregion

			#region Ouverture de la base de données
			bool DBToClosed=false;
			if (Source.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					Open();
				}
				catch(System.Exception err){
					throw(new OracleDataSourceException("Impossible d'ouvrir la base de données",err));
				}
			}
			#endregion

			#region Execution de la requête
			try{
				sqlCommand=new OracleCommand(command,Source);
				sqlCommand.ExecuteNonQuery();
			}
			#region Traitement d'erreur du chargement des données
			catch(System.Exception ex){
				try{
					// Fermeture de la base de données
					if(sqlCommand!=null) sqlCommand.Dispose();
					if(DBToClosed) Close();
				}
				catch(System.Exception et){
					throw(new OracleDataSourceException ("Impossible de fermer la base de données, lors de la gestion d'erreur de l'execution de la requête",et));
				}
				throw(new OracleDataSourceException ("Impossible d'executer la requête",ex));
			}
			#endregion

			#endregion

			#region Fermeture de la base de données
			try{
				// Fermeture de la base de données
				if(sqlCommand!=null)sqlCommand.Dispose();
				if(DBToClosed) Close();
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException ("Impossible de fermer la base de données",err));
			}
			#endregion
		}
		#endregion

		#endregion

		#region méthodes Externes

		#region Source de données
		/// <summary>
		/// Obtient la source de données
		/// </summary>
		/// <returns>Source de données</returns>
		public object GetSource(){
			return(Source);
		}


		/// <summary>
		/// Obtient le type de la source de données
		/// </summary>
		/// <returns>Type de la source de données</returns>
		public DataSource.Type DataSourceType(){
			return(_sourceType);
		}
		#endregion

		#region Ouvre une connexion
		/// <summary>
		/// Ouvre la source de données
		/// </summary>
		public void Open(){
			try{
				if (Source.State==System.Data.ConnectionState.Closed)Source.Open();
				else throw(new OracleDataSourceException ("La source est déjà ouverte"));

			}
			catch(System.Exception err){
				throw(new OracleDataSourceException("Impossible d'ouvrir la source de données: "+_connectionString,err));
			}
		}
		#endregion

		#region Ferme une connexion
		/// <summary>
		/// Ferme la source de données
		/// </summary>
		public void Close(){
			try{
				if (Source.State!=System.Data.ConnectionState.Closed) Source.Close();
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException ("Impossible de fermer la source de données",err));
			}
		}
		#endregion

		#region Sélection d'éléments
		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		/// <returns>Resultat</returns>
		public DataSet Fill(string command){
			try{
				return(Fill(command,false));
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException ("Impossible d'executer la requête",err));
			}
		}
		#endregion

		/// <summary>
		/// Modifie un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Update(string command){
			try{
				NonQuery(command);
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException ("Impossible d'executer la requête",err));
			}
		}

		/// <summary>
		/// Ajoute un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Insert(string command){
			try{
				NonQuery(command);
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException ("Impossible d'executer la requête",err));
			}
		}

		/// <summary>
		/// Suppression d'un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Delete(string command){
			try{
				NonQuery(command);
			}
			catch(System.Exception err){
				throw(new OracleDataSourceException ("Impossible d'executer la requête",err));
			}
		}

		#endregion
	}
}
