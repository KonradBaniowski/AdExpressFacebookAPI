#region Informations
// Auteur: G. Facon, B. Masson
// Date de cr�ation: 12/02/2004
// Date de modification: 30/09/2004
#endregion

using System;
using System.Collections;
using System.Data;
using TNS.AdExpress.Classification.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.DataAccess.Classification {
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
        /// Database schema
        /// </summary>
        protected string _dbSchema="";
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
        /// <remarks>Use only in TNS AdExpress website</remarks>
		/// <param name="table">Table servant � la construction de la liste</param>
		/// <param name="language">Langue des donn�es</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		public ClassificationLevelListDataAccess(TNS.AdExpress.Constantes.Classification.DB.Table.name table,int language,IDataSource source){
			this.language=language;
			this.table=table;
            DataTable dt;
			list=new Hashtable();
            if(_dbSchema==null || _dbSchema.Length==0) _dbSchema=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;

			#region Cr�ation de la requ�te
			string sql="select id_"+table.ToString()+", "+table.ToString();
            sql+=" from "+_dbSchema+"."+table.ToString();
			sql+=" where id_language="+language+" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" order by "+table.ToString();
			#endregion

            #region Request
            try {
                dt=source.Fill(sql).Tables[0];
                dt.TableName=table.ToString();
			}
			catch(System.Exception ex){
		        throw(new ClassificationDALException ("Impossible de charger les �l�ments de la nomenclature",ex));
			}
            #endregion


            #region Transformation du DataTable en HashTable
            try {
				foreach(DataRow currentRow in dt.Rows){
					list.Add(currentRow[0],currentRow[1]);
					idListOrderByClassificationItem.Add(currentRow[0]);
				}
			}
			catch(System.Exception ext){
				throw(new ClassificationDALException("Impossible de transf�rer les donn�es dans la liste",ext));
			}
			#endregion
			
		}


		/// <summary>
		/// Constructeur de la liste de tous les �l�ments d'un niveau d'une nomenclature
		/// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
		/// <param name="table">Table servant � la construction de la liste</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		/// <param name="codeslist">Liste des codes</param>
		/// <param name="language">langue du site</param>
        public ClassificationLevelListDataAccess(TNS.AdExpress.Constantes.Classification.DB.Table.name table,string codeslist,int language,IDataSource source) {
			this.language=language;
			this.table=table;
			list=new Hashtable();
            DataTable dt;

            if(_dbSchema==null || _dbSchema.Length==0) if(_dbSchema==null || _dbSchema.Length==0) _dbSchema=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;

			#region Cr�ation de la requ�te
			string sql="select id_"+table.ToString()+", "+table.ToString();
			sql+=" from "+_dbSchema+"."+table.ToString();
			sql+=" where id_language="+language+" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and id_"+table.ToString()+" in("+codeslist+")";
			sql+=" order by "+table.ToString();
			#endregion

            #region Request
            try {
                dt=source.Fill(sql).Tables[0];
                dt.TableName=table.ToString();
            }
            catch(System.Exception ex) {
                throw (new ClassificationDALException("Impossible de charger les �l�ments de la nomenclature",ex));
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
				throw(new ClassificationDALException("Impossible de transf�rer les donn�es dans la liste"+ext));
			}
			#endregion
			
		}

		/// <summary>
		/// Constructeur de la liste de tous les �l�ments d'un niveau d'une nomenclature
		/// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
		/// <param name="table">Table servant � la construction de la liste</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		/// <param name="codeslist">Liste des codes</param>
		/// <param name="language">langue du site</param>
		public ClassificationLevelListDataAccess(string table, string codeslist, int language, IDataSource source) {
			this.language = language;
			list = new Hashtable();
            DataTable dt;

            if(_dbSchema==null || _dbSchema.Length==0) if(_dbSchema==null || _dbSchema.Length==0) _dbSchema=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;

			#region Cr�ation de la requ�te
			string sql = "select id_" + table + ", " + table;
			sql += " from " + _dbSchema + "." + table;
			sql += " where id_language=" + language + " and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql += " and id_" + table + " in(" + codeslist + ")";
			sql += " order by " + table;
			#endregion

            #region Request
            try {
                dt=source.Fill(sql).Tables[0];
                dt.TableName=table.ToString();
            }
            catch(System.Exception ex) {
                throw (new ClassificationDALException("Impossible de charger les �l�ments de la nomenclature",ex));
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
				throw (new ClassificationDALException("Impossible de transf�rer les donn�es dans la liste" + ext));
			}
			#endregion
		}

		/// <summary>
		/// Constructeur de la liste de tous les �l�ments d'un niveau d'une nomenclature
		/// </summary>
		/// <param name="table">Table servant � la construction de la liste</param>
		/// <param name="source">Connexion � la base de donn�es</param>
		/// <param name="codeslist">Liste des codes</param>
		/// <param name="language">langue du site</param>
		/// <param name="dbSchema">Scema base de donn�es</param>
		public ClassificationLevelListDataAccess(string table, string codeslist, int language,IDataSource source,string dbSchema) {
			this.language = language;
            DataTable dt;
            list = new Hashtable();
            _dbSchema=dbSchema;

			#region Cr�ation de la requ�te
			string sql = "select id_" + table + ", " + table;
			sql += " from " + _dbSchema + "." + table;
			sql += " where id_language=" + language + " and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql += " and id_" + table + " in(" + codeslist + ")";
			sql += " order by " + table;
			#endregion

            #region Request
            try {
                dt=source.Fill(sql).Tables[0];
                dt.TableName=table.ToString();
            }
            catch(System.Exception ex) {
                throw (new ClassificationDALException("Impossible de charger les �l�ments de la nomenclature",ex));
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
				throw (new ClassificationDALException("Impossible de transf�rer les donn�es dans la liste" + ext));
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
					throw(new ClassificationDALException("Il n'existe pas de valeur pour l'identifiant "+id.ToString()+". Le support est peut �tre d�sactiv�, dans ce cas il sera supprim� lors de la prochaine sauvegarde des supports",e));
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
