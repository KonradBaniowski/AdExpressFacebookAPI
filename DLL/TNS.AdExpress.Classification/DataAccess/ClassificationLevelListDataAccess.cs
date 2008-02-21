#region Informations
// Auteur: G. Facon, B. Masson
// Date de cr�ation: 12/02/2004
// Date de modification: 30/09/2004
#endregion

using System;
using System.Collections;
using Oracle.DataAccess.Client;
using System.Data;
using TNS.AdExpress.Classification.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Classification.DataAccess{
	/// <summary>
	/// Contientla liste des libell�s d'un niveau de nomenclature
	/// </summary>
	public class ClassificationLevelListDataAccess{

		#region Variables
		/// <summary>
		/// Nom de la table
		/// </summary>
		protected TNS.AdExpress.Constantes.Classification.DB.Table.name table;
		/// <summary>
		/// Liste des �l�ments
		/// </summary>
		protected Hashtable list;
		/// <summary>
		/// Liste des identifiants tri�s par libell�s
		/// </summary>
		protected ArrayList idListOrderByClassificationItem=new ArrayList();
		/// <summary>
		/// Langue des �l�ment
		/// </summary>
		protected int language=TNS.AdExpress.Constantes.DB.Language.FRENCH;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur de la liste de tous les �l�ments d'un niveau d'une nomenclature
		/// </summary>
		/// <param name="table">Table servant � la construction de la liste</param>
		/// <param name="language">Langue des donn�es</param>
		/// <param name="connection">Connexion � la base de donn�es</param>
		public ClassificationLevelListDataAccess(TNS.AdExpress.Constantes.Classification.DB.Table.name table,int language,OracleConnection connection){
			this.language=language;
			this.table=table;
			list=new Hashtable();

			#region Cr�ation de la requ�te
			string sql="select id_"+table.ToString()+", "+table.ToString();
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+table.ToString();
			sql+=" where id_language="+language+" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" order by "+table.ToString();
			#endregion
				
			#region Ouverture de la base de donn�es
			bool DBToClosed=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception err){
					throw(new ClassificationDataDBException("Impossible d'ouvrir la base de donn�es",err));
				}
			}
			#endregion

			OracleCommand sqlCommand=null;
			OracleDataAdapter sqlAdapter=null;
			DataSet ds=new DataSet();
			DataTable dt;

			try{
				sqlCommand=new OracleCommand(sql,connection);
				sqlAdapter=new OracleDataAdapter(sqlCommand);
				sqlAdapter.Fill(ds,table.ToString());
				dt=ds.Tables[table.ToString()];
				ds.Tables.Clear();
				ds=null;
			}

			#region Traitement d'erreur du chargement des donn�es
			catch(System.Exception ex){
				try{
					// Fermeture de la base de donn�es
					if (sqlAdapter!=null){
						sqlAdapter.Dispose();
					}
					if(sqlCommand!=null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new ClassificationDataDBException ("Impossible de fermer la base de donn�es lors d'une erreur d'authentification",et));
				}
				throw(new ClassificationDataDBException ("Impossible de charger les �l�ments de la nomenclature",ex));
			}
			#endregion

			#region Fermeture de la base de donn�es
			try{
				// Fermeture de la base de donn�es
				if (sqlAdapter!=null){
					sqlAdapter.Dispose();
				}
				if(sqlCommand!=null)sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
			}
			catch(System.Exception et){
				throw(new ClassificationDataDBException ("Impossible de fermer la base de donn�es lors de l'authentification",et));
			}
			#endregion

			#region Transformation du DataTable en HashTable
			try{
				foreach(DataRow currentRow in dt.Rows){
					list.Add(currentRow[0],currentRow[1]);
					idListOrderByClassificationItem.Add(currentRow[0]);
				}
			}
			catch(System.Exception ext){
				throw(new ClassificationDataDBException("Impossible de transf�rer les donn�es dans la liste",ext));
			}
			#endregion
			
		}


		/// <summary>
		/// Constructeur de la liste de tous les �l�ments d'un niveau d'une nomenclature
		/// </summary>
		/// <param name="table">Table servant � la construction de la liste</param>
		/// <param name="connection">Connexion � la base de donn�es</param>
		/// <param name="codeslist">Liste des codes</param>
		/// <param name="language">langue du site</param>
		public ClassificationLevelListDataAccess(TNS.AdExpress.Constantes.Classification.DB.Table.name table,string codeslist,int language,OracleConnection connection){
			this.language=language;
			this.table=table;
			list=new Hashtable();

			#region Cr�ation de la requ�te
			string sql="select id_"+table.ToString()+", "+table.ToString();
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+table.ToString();
			sql+=" where id_language="+language+" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and id_"+table.ToString()+" in("+codeslist+")";
			sql+=" order by "+table.ToString();
			#endregion
			
			#region Ouverture de la base de donn�es
			bool DBToClosed=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception e){
					throw(new ClassificationDataDBException("Impossible d'ouvrir la base de donn�es",e));
				}
			}
			#endregion

			OracleCommand sqlCommand=null;
			OracleDataAdapter sqlAdapter=null;
			DataSet ds=new DataSet();
			DataTable dt;

			try{
				sqlCommand=new OracleCommand(sql,connection);
				sqlAdapter=new OracleDataAdapter(sqlCommand);
				sqlAdapter.Fill(ds,table.ToString());
				dt=ds.Tables[table.ToString()];
				ds.Tables.Clear();
				ds=null;
			}

			#region Traitement d'erreur du chargement des donn�es
			catch(System.Exception ex){
				try{
					// Fermeture de la base de donn�es
					if (sqlAdapter!=null){
						sqlAdapter.Dispose();
					}
					if(sqlCommand!=null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new ClassificationDataDBException ("Impossible de fermer la base de donn�es lors d'une erreur d'authentification",et));
				}
				throw(new ClassificationDataDBException ("Impossible de charger les �l�ments de la nomenclature",ex));
			}
			#endregion

			#region Fermeture de la base de donn�es
			try{
				// Fermeture de la base de donn�es
				if (sqlAdapter!=null){
					sqlAdapter.Dispose();
				}
				if(sqlCommand!=null)sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
			}
			catch(System.Exception et){
				throw(new ClassificationDataDBException ("Impossible de fermer la base de donn�es lors de l'authentification",et));
			}
			#endregion

			#region Transformation du DataTable en HashTable
			try{
				foreach(DataRow currentRow in dt.Rows){
					list.Add(currentRow[0],currentRow[1]);
					idListOrderByClassificationItem.Add(currentRow[0]);
				}
			}
			catch(System.Exception ext){
				throw(new ClassificationDataDBException("Impossible de transf�rer les donn�es dans la liste"+ext));
			}
			#endregion
			
		}

		/// <summary>
		/// Constructeur de la liste de tous les �l�ments d'un niveau d'une nomenclature
		/// </summary>
		/// <param name="table">Table servant � la construction de la liste</param>
		/// <param name="connection">Connexion � la base de donn�es</param>
		/// <param name="codeslist">Liste des codes</param>
		/// <param name="language">langue du site</param>
		public ClassificationLevelListDataAccess(string table, string codeslist, int language, OracleConnection connection) {
			this.language = language;
			list = new Hashtable();

			#region Cr�ation de la requ�te
			string sql = "select id_" + table + ", " + table;
			sql += " from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + table;
			sql += " where id_language=" + language + " and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql += " and id_" + table + " in(" + codeslist + ")";
			sql += " order by " + table;
			#endregion

			#region Ouverture de la base de donn�es
			bool DBToClosed = false;
			if (connection.State == System.Data.ConnectionState.Closed) {
				DBToClosed = true;
				try {
					connection.Open();
				}
				catch (System.Exception e) {
					throw (new ClassificationDataDBException("Impossible d'ouvrir la base de donn�es", e));
				}
			}
			#endregion

			OracleCommand sqlCommand = null;
			OracleDataAdapter sqlAdapter = null;
			DataSet ds = new DataSet();
			DataTable dt;

			try {
				sqlCommand = new OracleCommand(sql, connection);
				sqlAdapter = new OracleDataAdapter(sqlCommand);
				sqlAdapter.Fill(ds, table.ToString());
				dt = ds.Tables[table];
				ds.Tables.Clear();
				ds = null;
			}

			#region Traitement d'erreur du chargement des donn�es
			catch (System.Exception ex) {
				try {
					// Fermeture de la base de donn�es
					if (sqlAdapter != null) {
						sqlAdapter.Dispose();
					}
					if (sqlCommand != null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch (System.Exception et) {
					throw (new ClassificationDataDBException("Impossible de fermer la base de donn�es lors d'une erreur d'authentification", et));
				}
				throw (new ClassificationDataDBException("Impossible de charger les �l�ments de la nomenclature", ex));
			}
			#endregion

			#region Fermeture de la base de donn�es
			try {
				// Fermeture de la base de donn�es
				if (sqlAdapter != null) {
					sqlAdapter.Dispose();
				}
				if (sqlCommand != null) sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
			}
			catch (System.Exception et) {
				throw (new ClassificationDataDBException("Impossible de fermer la base de donn�es lors de l'authentification", et));
			}
			#endregion

			#region Transformation du DataTable en HashTable
			try {
				foreach (DataRow currentRow in dt.Rows) {
					list.Add(currentRow[0], currentRow[1]);
					idListOrderByClassificationItem.Add(currentRow[0]);
				}
			}
			catch (System.Exception ext) {
				throw (new ClassificationDataDBException("Impossible de transf�rer les donn�es dans la liste" + ext));
			}
			#endregion
		}

		/// <summary>
		/// Constructeur de la liste de tous les �l�ments d'un niveau d'une nomenclature
		/// </summary>
		/// <param name="table">Table servant � la construction de la liste</param>
		/// <param name="connection">Connexion � la base de donn�es</param>
		/// <param name="codeslist">Liste des codes</param>
		/// <param name="language">langue du site</param>
		/// <param name="dbSchema">Scema base de donn�es</param>
		public ClassificationLevelListDataAccess(string table, string codeslist, int language, OracleConnection connection,string dbSchema) {
			this.language = language;
			list = new Hashtable();

			#region Cr�ation de la requ�te
			string sql = "select id_" + table + ", " + table;
			sql += " from " + dbSchema + "." + table;
			sql += " where id_language=" + language + " and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql += " and id_" + table + " in(" + codeslist + ")";
			sql += " order by " + table;
			#endregion

			#region Ouverture de la base de donn�es
			bool DBToClosed = false;
			if (connection.State == System.Data.ConnectionState.Closed) {
				DBToClosed = true;
				try {
					connection.Open();
				}
				catch (System.Exception e) {
					throw (new ClassificationDataDBException("Impossible d'ouvrir la base de donn�es", e));
				}
			}
			#endregion

			OracleCommand sqlCommand = null;
			OracleDataAdapter sqlAdapter = null;
			DataSet ds = new DataSet();
			DataTable dt;

			try {
				sqlCommand = new OracleCommand(sql, connection);
				sqlAdapter = new OracleDataAdapter(sqlCommand);
				sqlAdapter.Fill(ds, table.ToString());
				dt = ds.Tables[table];
				ds.Tables.Clear();
				ds = null;
			}

			#region Traitement d'erreur du chargement des donn�es
			catch (System.Exception ex) {
				try {
					// Fermeture de la base de donn�es
					if (sqlAdapter != null) {
						sqlAdapter.Dispose();
					}
					if (sqlCommand != null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch (System.Exception et) {
					throw (new ClassificationDataDBException("Impossible de fermer la base de donn�es lors d'une erreur d'authentification", et));
				}
				throw (new ClassificationDataDBException("Impossible de charger les �l�ments de la nomenclature", ex));
			}
			#endregion

			#region Fermeture de la base de donn�es
			try {
				// Fermeture de la base de donn�es
				if (sqlAdapter != null) {
					sqlAdapter.Dispose();
				}
				if (sqlCommand != null) sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
			}
			catch (System.Exception et) {
				throw (new ClassificationDataDBException("Impossible de fermer la base de donn�es lors de l'authentification", et));
			}
			#endregion

			#region Transformation du DataTable en HashTable
			try {
				foreach (DataRow currentRow in dt.Rows) {
					list.Add(currentRow[0], currentRow[1]);
					idListOrderByClassificationItem.Add(currentRow[0]);
				}
			}
			catch (System.Exception ext) {
				throw (new ClassificationDataDBException("Impossible de transf�rer les donn�es dans la liste" + ext));
			}
			#endregion

		}
		#endregion
		
		#region accesseur
		/// <summary>
		/// Retourne la valeur dont l'dentifiant est ID
		/// </summary>
		public string this [Int64 id]{
			get{
				try{
					return(list[id].ToString());
				}
				catch(System.Exception e){
					throw(new ClassificationDataDBException("Il n'existe pas de valeur pour l'identifiant "+id.ToString()+". Le support est peut �tre d�sactiv�, dans ce cas il sera supprim� lors de la prochaine sauvegarde des supports",e));
				}
			}
		}

		/// <summary>
		/// Retourne la liste des identifiants tri�s par libell�s
		/// </summary>
		public ArrayList IdListOrderByClassificationItem{
			get{return(this.idListOrderByClassificationItem);}
		}
		#endregion

	}
}
