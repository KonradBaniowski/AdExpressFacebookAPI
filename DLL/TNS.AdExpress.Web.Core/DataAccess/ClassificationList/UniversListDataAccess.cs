#region Informations
// Autjor: G. Facon
// Creation Date: 14/06/2004
// Modification Date: 15/06/2004
//	11/08/2005	G. Facon	New Exception Management and property name 
//	21/11/2005	G. Facon	IDataSource
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using TNS.AdExpress.Web.Core.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using webConstantes=TNS.AdExpress.Constantes.Web;

using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
namespace TNS.AdExpress.Web.Core.DataAccess.ClassificationList{
	/// <summary>
	/// Class used for connections with the datebase in management of the universes
	/// </summary>
	public class UniversListDataAccess{
		
		#region Variables
		/// <summary>
		/// Login Id
		/// </summary>
		protected Int64 _idLogin;
		/// <summary>
		/// Directory id (known from its name)
		/// </summary>
		protected Int64 _idRep;
		/// <summary>
		/// Univers Id (known from its name and id_group_universe_client
		/// </summary>
		protected Int64 _idUniv;
		/// <summary>
		/// Object Id known from:
		/// idUnivers, idType, tableType, Exception, list
		/// </summary>
		protected Int64 _idObject;
		/// <summary>
		/// DataBase connection
		/// </summary>
		protected OracleConnection _cnx;	
		/// <summary>
		/// table With 4 columns :
		/// idGroupUniverse, GroupUniverse, idUnivers, Univers
		/// </summary>
		protected DataTable _dt;
		/// <summary>
		/// Line of dt
		/// </summary>
		protected DataRow _dr;
		/// <summary>
		/// Dataview of dt
		/// </summary>
		protected DataView _dv;	
		/// <summary>
		/// Directory item objects list
		/// </summary>
		protected ArrayList _allItem=new ArrayList();	
		#endregion

		#region Constructors
		/// <summary>
		/// Constructors
		/// </summary>
		/// <param name="idLogin">Login If</param>
		/// <param name="cnx">Connection</param>
		protected UniversListDataAccess(Int64 idLogin,OracleConnection cnx) {
			_idLogin=idLogin;
			_cnx=cnx;
		}
		#endregion

		#region Méthodes

		#region GetData
		/// <summary>
		/// Get the customer universes list which are recorded
		/// </summary>
		/// <param name="webSession">Customer session</param>
		/// <param name="branch">Branch used (media, product)</param>
		/// <param name="ListUniverseClientDescription">ID_UNIVERSE_CLIENT_DESCRIPTION</param>
		/// <returns>Universes list</returns>
		public static DataSet GetData(WebSession webSession, string branch, string ListUniverseClientDescription) {

			#region Request construction		

            /* TODO MODIFICATION
             * Code add to improve the performance of the universe loading process
            * 
            string sql = "select distinct " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ID_GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".COLUMN_NAME ";*/
            string sql = "select distinct " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ID_GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".UNIVERSE_CLIENT ";
			sql += " from " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).SqlWithPrefix + " , "
				+ WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).SqlWithPrefix + " , "
				+ WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseDescription).SqlWithPrefix;
			sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin.ToString();
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED;
			sql += " and (" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED + " or " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ACTIVATION is null) ";
			sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ID_GROUP_UNIVERSE_CLIENT=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_GROUP_UNIVERSE_CLIENT(+) ";

			if (ListUniverseClientDescription.Trim().Length > 0) {
				sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_UNIVERSE_CLIENT_DESCRIPTION = " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseDescription).Prefix + ".ID_UNIVERSE_CLIENT_DESCRIPTION";
				sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_UNIVERSE_CLIENT_DESCRIPTION in (" + ListUniverseClientDescription + ") ";
			}
			if (branch.Length > 0) {
				sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".id_type_universe_client in (" + branch + ")";
			}
			sql += " order by " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".UNIVERSE_CLIENT ";
			#endregion

			#region Request execution
			try {
				return (webSession.Source.Fill(sql));
			}
			catch (System.Exception err) {
				throw (new UniversListException("Impossible to retreive the customer universe whitch are recorded", err));
			}
			#endregion

		}

        /// <summary>
        /// Get the customer universes list which are recorded according to a filter
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <param name="branch">Branch used (media, product)</param>
        /// <param name="ListUniverseClientDescription">ID_UNIVERSE_CLIENT_DESCRIPTION</param>
        /// <param name="filter">Filter</param>
        /// <returns>Universes list</returns>
        public static DataSet GetData(WebSession webSession, string branch, string ListUniverseClientDescription, LevelInformation filter){

            #region Request construction

            /* TODO MODIFICATION
             * Code add to improve the performance of the universe loading process
            * 
            string sql = "select distinct " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ID_GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".COLUMN_NAME ";*/
            string sql = "select distinct " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ID_GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".UNIVERSE_CLIENT ";
            sql += " from " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).SqlWithPrefix + " , "
                + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).SqlWithPrefix + " , "
                + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseDescription).SqlWithPrefix;
            sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin.ToString();
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED;
            sql += " and (" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED + " or " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ACTIVATION is null) ";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".ID_GROUP_UNIVERSE_CLIENT=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_GROUP_UNIVERSE_CLIENT(+) ";

            if (ListUniverseClientDescription.Trim().Length > 0){
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_UNIVERSE_CLIENT_DESCRIPTION = " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseDescription).Prefix + ".ID_UNIVERSE_CLIENT_DESCRIPTION";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".ID_UNIVERSE_CLIENT_DESCRIPTION in (" + ListUniverseClientDescription + ") ";
            }
            if (branch.Length > 0){
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".id_type_universe_client in (" + branch + ")";
            }
            if (filter != null) {
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".filter in (" + filter.ID + ")";
            }
            sql += " order by " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup).Prefix + ".GROUP_UNIVERSE_CLIENT, " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse).Prefix + ".UNIVERSE_CLIENT ";
            #endregion

            #region Request execution
            try{
                return (webSession.Source.Fill(sql));
            }
            catch (System.Exception err){
                throw (new UniversListException("Impossible to retreive the customer universe whitch are recorded", err));
            }
            #endregion

        }

		/// <summary>
		/// Get the customer universes list which are recorded dependind on allowed universe levels
		/// </summary>
		/// <param name="webSession">Customer session</param>
		/// <param name="branch">Branch used (media, product)</param>
		/// <param name="ListUniverseClientDescription">ID_UNIVERSE_CLIENT_DESCRIPTION</param>
		/// <param name="allowedLevels">allowed universe levels</param>
		/// <returns>Universes list</returns>
		public static DataTable GetData(WebSession webSession, string branch, string ListUniverseClientDescription, List<Int64> allowedLevels) {
			DataTable dt = new DataTable();

			DataSet ds = GetData(webSession, branch, ListUniverseClientDescription);
			List<long> levelsIds = null;
            /* TODO MODIFICATION
             * Code add to improve the performance of the universe loading process
            * 
            string[] levelsIds = null;*/
            Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = null;
			bool isValidUniverse = true;
			if (ds != null && ds.Tables[0].Rows.Count > 0) {

				dt.Columns.Add("ID_GROUP_UNIVERSE_CLIENT", ds.Tables[0].Rows[0]["ID_GROUP_UNIVERSE_CLIENT"].GetType());
				dt.Columns.Add("GROUP_UNIVERSE_CLIENT", ds.Tables[0].Rows[0]["GROUP_UNIVERSE_CLIENT"].GetType());
				dt.Columns.Add("ID_UNIVERSE_CLIENT", ds.Tables[0].Rows[0]["ID_UNIVERSE_CLIENT"].GetType());
				dt.Columns.Add("UNIVERSE_CLIENT", ds.Tables[0].Rows[0]["UNIVERSE_CLIENT"].GetType());
                /* TODO MODIFICATION
                 * Code add to improve the performance of the universe loading process
                * 
                dt.Columns.Add("COLUMN_NAME", ds.Tables[0].Rows[0]["COLUMN_NAME"].GetType());*/

                foreach (DataRow dr in ds.Tables[0].Rows) {

                    /* TODO MODIFICATION
                     * Code add to improve the performance of the universe loading process
                    * 
                    if (dr["COLUMN_NAME"] != null && dr["COLUMN_NAME"].ToString().Length > 0)
                        levelsIds = dr["COLUMN_NAME"].ToString().Split('-');

                    foreach (string levelId in levelsIds) {
                        if (!allowedLevels.Contains(Int64.Parse(levelId))) {
                            isValidUniverse = false;
                        }
                    }

                    //Univers valide
                    if (isValidUniverse) {
                        dt.Rows.Add(dr.ItemArray);
                    }

                    isValidUniverse = true;*/

                    universes = (Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>)GetObjectUniverses(long.Parse(dr["ID_UNIVERSE_CLIENT"].ToString()), webSession);
                    if (universes != null && universes.Count > 0) {
                        levelsIds = WebSession.GetLevelListId(universes);
                        for (int j = 0; j < levelsIds.Count; j++) {
                            if (!allowedLevels.Contains(levelsIds[j])) {
                                isValidUniverse = false;
                            }
                        }

                        //Univers valide
                        if (isValidUniverse) {
                            dt.Rows.Add(dr.ItemArray);
                        }
                    }
                    isValidUniverse = true;

                }
			}

			return dt;
		}

        /// <summary>
        /// Get the customer universes list which are recorded dependind on allowed universe levels
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <param name="branch">Branch used (media, product)</param>
        /// <param name="ListUniverseClientDescription">ID_UNIVERSE_CLIENT_DESCRIPTION</param>
        /// <param name="allowedLevels">allowed universe levels</param>
        /// <param name="filter">Filter</param>
        /// <returns>Universes list</returns>
        public static DataTable GetData(WebSession webSession, string branch, string ListUniverseClientDescription, List<Int64> allowedLevels, LevelInformation filter){
            DataTable dt = new DataTable();

            DataSet ds = GetData(webSession, branch, ListUniverseClientDescription,filter);
            List<long> levelsIds = null;
            /* TODO MODIFICATION
             * Code add to improve the performance of the universe loading process
            * 
            string[] levelsIds = null;*/
            Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = null;
            bool isValidUniverse = true;
            if (ds != null && ds.Tables[0].Rows.Count > 0){

                dt.Columns.Add("ID_GROUP_UNIVERSE_CLIENT", ds.Tables[0].Rows[0]["ID_GROUP_UNIVERSE_CLIENT"].GetType());
                dt.Columns.Add("GROUP_UNIVERSE_CLIENT", ds.Tables[0].Rows[0]["GROUP_UNIVERSE_CLIENT"].GetType());
                dt.Columns.Add("ID_UNIVERSE_CLIENT", ds.Tables[0].Rows[0]["ID_UNIVERSE_CLIENT"].GetType());
                dt.Columns.Add("UNIVERSE_CLIENT", ds.Tables[0].Rows[0]["UNIVERSE_CLIENT"].GetType());
                
                /* TODO MODIFICATION
                 * Code add to improve the performance of the universe loading process
                * 
                dt.Columns.Add("COLUMN_NAME", ds.Tables[0].Rows[0]["COLUMN_NAME"].GetType());*/

                foreach (DataRow dr in ds.Tables[0].Rows){

                    /* TODO MODIFICATION
                     * Code add to improve the performance of the universe loading process
                    * 
                    if (dr["COLUMN_NAME"] != null && dr["COLUMN_NAME"].ToString().Length > 0)
                        levelsIds = dr["COLUMN_NAME"].ToString().Split('-');

                    foreach (string levelId in levelsIds) {
                        if (!allowedLevels.Contains(Int64.Parse(levelId))) {
                            isValidUniverse = false;
                        }
                    }

                    //Univers valide
                    if (isValidUniverse) {
                        dt.Rows.Add(dr.ItemArray);
                    }

                    isValidUniverse = true;*/

                    universes = (Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>)GetObjectUniverses(long.Parse(dr["ID_UNIVERSE_CLIENT"].ToString()), webSession);
                    if (universes != null && universes.Count > 0){
                        levelsIds = WebSession.GetLevelListId(universes);
                        for (int j = 0; j < levelsIds.Count; j++){
                            if (!allowedLevels.Contains(levelsIds[j])){
                                isValidUniverse = false;
                            }
                        }

                        //Univers valide
                        if (isValidUniverse){
                            dt.Rows.Add(dr.ItemArray);
                        }
                    }
                    isValidUniverse = true;

                }
            }

            return dt;
        }
		#endregion

		#region GetGroupUniverses
		/// <summary>
		/// Donne la liste des Groupes d'univers
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="webSession">Session du client</param>
		/// <returns>Liste des Groupes d'univers</returns>
		public static DataSet GetGroupUniverses(WebSession webSession){
			Table universeGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup);
			#region Request construction
			//Requête pour récupérer tous les univers d'un idLogin
			string sql = "select distinct " + universeGroupTable.Prefix + ".ID_GROUP_UNIVERSE_CLIENT, " + universeGroupTable.Prefix + ".GROUP_UNIVERSE_CLIENT ";
			sql += " from " + universeGroupTable.SqlWithPrefix;
			sql += " where " + universeGroupTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
			sql += " and " + universeGroupTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
			sql += " order by " + universeGroupTable.Prefix + ".GROUP_UNIVERSE_CLIENT ";		
			#endregion

			#region Request execution
			try{
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw(new UniversListException ("Impossible d'obtenir la liste des Groupes d'univers",err));
			}
			#endregion

		}
		#endregion

		#region IsUniverseExist
		/// <summary>
		/// Vérifie si un univers est déjà enregistré dans la base de données
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="webSession">Session du client</param>
		/// <param name="universeName">Nom de l'univers</param>
		/// <returns>True s'il existe, false sinon</returns>
		public static bool IsUniverseExist(WebSession webSession, string universeName){
			Table universeGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup);
			Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

			#region Request construction
			string sql="select se.UNIVERSE_CLIENT from "+ universeTable.SqlWithPrefix + ", ";
			sql += " " + universeGroupTable.SqlWithPrefix;
			sql+=" where "+universeGroupTable.Prefix +".ID_LOGIN="+webSession.CustomerLogin.IdLogin;
			sql += " and " + universeGroupTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
			sql+=" and "+universeTable.Prefix+".ACTIVATION<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql += " and " + universeGroupTable.Prefix + ".ID_GROUP_UNIVERSE_CLIENT = " + universeTable.Prefix + ".ID_GROUP_UNIVERSE_CLIENT ";
			sql += " and UPPER(" + universeTable.Prefix + ".UNIVERSE_CLIENT) like UPPER('" + universeName + "')";
			#endregion

			#region Request execution
			DataSet ds;
			try{
				ds=webSession.Source.Fill(sql);
				if(ds.Tables[0].Rows.Count>0)return(true);
				return(false);
			}
			catch(System.Exception err){
				throw(new UniversListException("Impossible de vérifier si un univers est déjà enregistré dans la base de données",err));
			}
			#endregion

		}

		/// <summary>
		/// Vérifie si un univers appartient à une descrition client
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idUniverse">id Universe</param>
		/// <param name="idUniverseClientDescription">id Univers Client Description</param>
		/// <returns></returns>
		public static bool IsUniverseBelongToClientDescription(WebSession webSession, Int64 idUniverse,Int64 idUniverseClientDescription) {
			Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

			#region Request construction
			string sql = "select " + universeTable.Prefix + ".UNIVERSE_CLIENT from "
				+ universeTable.SqlWithPrefix;
			sql += " where " + universeTable.Prefix + ".ID_UNIVERSE_CLIENT = " + idUniverse;
			sql += " and " + universeTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
			sql += " and " + universeTable.Prefix + ".ID_UNIVERSE_CLIENT_DESCRIPTION  = " + idUniverseClientDescription;			
			#endregion

			#region Request execution
			DataSet ds;
			try {
				ds = webSession.Source.Fill(sql);
				if (ds.Tables[0].Rows.Count > 0) return (true);
				return (false);
			}
			catch (System.Exception err) {
				throw (new UniversListException("Impossible de vérifier si un univers est appartient à une description cliente ", err));
			}
			#endregion

		}
		#endregion

		#region GetTreeNodeUniverse
		/// <summary>
		/// Méthode pour la récupération et la "deserialization" d'un objet Universe d'adexpress
		/// </summary>
		/// <param name="idUniverse">Identifiant de l'univers</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
		public static Object GetObjectUniverses(Int64 idUniverse, WebSession webSession) {
			return GetTreeNodeUniverse(idUniverse,webSession);
		}

		/// <summary>
		/// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir de la table Universe Client
		/// </summary>
		/// <param name="idUniverse">Identifiant de l'univers</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
		public static Object GetTreeNodeUniverse(Int64 idUniverse,WebSession webSession){
			
			#region Ouverture de la base de données
			bool DBToClosed=false;
			OracleConnection cnx = (OracleConnection) webSession.Source.GetSource();
			
			if (cnx.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					cnx.Open();
				}
				catch(System.Exception e){
					throw(new UniversListException("Impossible d'ouvrir la base de données",e));
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
				Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

				binaryData = new byte[0];
				//create anonymous PL/SQL command
				string block = " BEGIN "+
					" SELECT blob_universe_client INTO :1 FROM " + universeTable.Sql 
					+ " WHERE id_universe_client = " + idUniverse + "; " +
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
					throw(new UniversListException("Impossible de libérer les ressources après échec de la méthode",et));
				}
				throw(new UniversListException("Problème au chargement de la session à partir de la base de données",e));
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
				throw(new UniversListException("Impossible de fermer la base de données : ",et));
			}
			#endregion
			
			//retourne l'objet deserialized ou null si il y a eu un probleme
			return(o);
		}
		#endregion

		#region SaveUniverse
		/// <summary>
		/// Sauvegarde d'un nouvel univers
		/// </summary>
		/// <remarks>A Tester</remarks>
		/// <param name="idGroupUniverse">Identifiant du groupe d'univers</param>
		/// <param name="universe">Non de l'univers</param>
		/// <param name="alUniverseTreeNode"> Liste contenant les 2 arbres utilisés pour la sauvegarde des univers</param>
		/// <param name="type">Branche de l'univers</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="idUniverseClientDescription">idUniverseClientDescription</param>
		/// <returns>Retourne true si l'univers a été crée</returns>
		public static bool SaveUniverse(Int64 idGroupUniverse,string universe,ArrayList alUniverseTreeNode,TNS.AdExpress.Constantes.Classification.Branch.type type,Int64 idUniverseClientDescription,WebSession webSession){

			#region Ouverture de la base de données
			OracleConnection connection=(OracleConnection) webSession.Source.GetSource();
			bool DBToClosed=false;
			bool success=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception e){
					throw(new UniversListException("Impossible d'ouvrir la base de données",e));
				}
			}
			#endregion
			
			int idTypeUniverseClient=type.GetHashCode();

			#region Sérialisation et sauvegarde de la session
			OracleCommand sqlCommand=null;
			MemoryStream ms=null;
			BinaryFormatter bf=null;
			byte[] binaryData=null;
			

			try{
				Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);
				Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
				//"Serialization"
				ms = new MemoryStream();
				bf = new BinaryFormatter();
				bf.Serialize(ms,alUniverseTreeNode);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

				//create anonymous PL/SQL command
				string block = " BEGIN "+
					" INSERT INTO " + universeTable.Sql +
					" (ID_UNIVERSE_CLIENT, ID_GROUP_UNIVERSE_CLIENT, UNIVERSE_CLIENT, BLOB_UNIVERSE_CLIENT,ID_TYPE_UNIVERSE_CLIENT ,ID_UNIVERSE_CLIENT_DESCRIPTION ,DATE_CREATION, DATE_MODIFICATION, ACTIVATION) "+
					" VALUES "+
					" (" + schema .Label+ ".seq_universe_client.nextval, " + idGroupUniverse + ", '" + universe + "', :1, " + idTypeUniverseClient + "," + idUniverseClientDescription + ",sysdate, sysdate," + DBConstantes.ActivationValues.ACTIVATED + "); " +
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
					throw(new MySessionDataAccessException("Impossible de libérer les ressources après échec de la méthode"+et));
				}
				throw(new MySessionDataAccessException("Echec de la sauvegarde de l'objet dans la base de donnée"+e));
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
				throw(new MySessionDataAccessException("Impossible de fermer la base de données",et));
			}
			#endregion

			return(success);
		}

        /// <summary>
        /// Sauvegarde d'un nouvel univers
        /// </summary>
        /// <remarks>A Tester</remarks>
        /// <param name="idGroupUniverse">Identifiant du groupe d'univers</param>
        /// <param name="universe">Non de l'univers</param>
        /// <param name="alUniverseTreeNode"> Liste contenant les 2 arbres utilisés pour la sauvegarde des univers</param>
        /// <param name="type">Branche de l'univers</param>
        /// <param name="webSession">Session utilisateur</param>
        /// <param name="idUniverseClientDescription">idUniverseClientDescription</param>
        /// <param name="filter">filter</param>
        /// <returns>Retourne true si l'univers a été crée</returns>
        public static bool SaveUniverse(Int64 idGroupUniverse, string universe, ArrayList alUniverseTreeNode, TNS.AdExpress.Constantes.Classification.Branch.type type, Int64 idUniverseClientDescription, WebSession webSession, int filter){

            #region Ouverture de la base de données
            OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
            bool DBToClosed = false;
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Closed){
                DBToClosed = true;
                try{
                    connection.Open();
                }
                catch (System.Exception e){
                    throw (new UniversListException("Impossible d'ouvrir la base de données", e));
                }
            }
            #endregion

            int idTypeUniverseClient = type.GetHashCode();

            #region Sérialisation et sauvegarde de la session
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;
            TreeNode tree;
            Int64 filterId=-1;
            string filterField = string.Empty;
            string filterValue = string.Empty;

            try{
                
                Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);
                Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
                //"Serialization"
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, alUniverseTreeNode);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //create anonymous PL/SQL command
                if (filter == 1) {
                    if (webSession.SelectionUniversMedia.FirstNode.Nodes[0] != null) {
                        tree = webSession.SelectionUniversMedia.FirstNode.Nodes[0];
                        filterId = ((TNS.AdExpress.Web.Core.Sessions.LevelInformation)(tree.Tag)).ID;
                        filterField = ", FILTER";  
                        filterValue = "," + filterId + "";
                    }
                }

                string block = " BEGIN " +
                    " INSERT INTO " + universeTable.Sql +
                    " (ID_UNIVERSE_CLIENT, ID_GROUP_UNIVERSE_CLIENT, UNIVERSE_CLIENT, BLOB_UNIVERSE_CLIENT,ID_TYPE_UNIVERSE_CLIENT ,ID_UNIVERSE_CLIENT_DESCRIPTION ,DATE_CREATION, DATE_MODIFICATION, ACTIVATION " + filterField +") " +
                    " VALUES " +
                    " (" + schema.Label + ".seq_universe_client.nextval, " + idGroupUniverse + ", '" + universe + "', :1, " + idTypeUniverseClient + "," + idUniverseClientDescription + ",sysdate, sysdate," + DBConstantes.ActivationValues.ACTIVATED + filterValue +"); " +
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
            catch (System.Exception e){
                // Fermeture des structures
                try{
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et){
                    throw (new MySessionDataAccessException("Impossible de libérer les ressources après échec de la méthode" + et));
                }
                throw (new MySessionDataAccessException("Echec de la sauvegarde de l'objet dans la base de donnée" + e));
            }
            //pas d'erreur
            try{
                // Fermeture des structures
                ms.Close();
                bf = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
                success = true;
            }
            catch (System.Exception et){
                throw (new MySessionDataAccessException("Impossible de fermer la base de données", et));
            }
            #endregion

            return (success);
        }


		/// <summary>
		/// Sauvegarde d'un nouvel univers
		/// </summary>
		/// <remarks>A Tester</remarks>
		/// <param name="idGroupUniverse">Identifiant du groupe d'univers</param>
		/// <param name="universe">Non de l'univers</param>
		/// <param name="universesToSave"> Liste contenant les univers à sauvegarde</param>
		/// <param name="type">Branche de l'univers</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="idUniverseClientDescription">idUniverseClientDescription</param>
		/// <returns>Retourne true si l'univers a été crée</returns>
		public static bool SaveUniverse(Int64 idGroupUniverse, string universe, Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universesToSave, TNS.AdExpress.Constantes.Classification.Branch.type type, Int64 idUniverseClientDescription, WebSession webSession) {

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
					throw (new UniversListException("Impossible d'ouvrir la base de données", e));
				}
			}
			#endregion

			int idTypeUniverseClient = type.GetHashCode();

			#region Sérialisation et sauvegarde de la session
			OracleCommand sqlCommand = null;
			MemoryStream ms = null;
			BinaryFormatter bf = null;
			byte[] binaryData = null;
            /* TODO MODIFICATION
             * Code add to improve the performance of the universe loading process
            * 
            List<long> levelsIds = null;
            string levelsIdsField = string.Empty;*/

            try {
				Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);
				Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

				//"Serialization"
				ms = new MemoryStream();
				bf = new BinaryFormatter();
				bf.Serialize(ms, universesToSave);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

                /* TODO MODIFICATION
                 * Code add to improve the performance of the universe loading process
                * 
                //Get the levels identifiers from the universes selected
                levelsIds = WebSession.GetLevelListId(universesToSave);

                if (levelsIds != null && levelsIds.Count > 0) {
                    for (int j = 0; j < levelsIds.Count; j++) {
                        levelsIdsField += levelsIds[j] + "-";
                    }
                    levelsIdsField = levelsIdsField.Substring(0, levelsIdsField.Length - 1);
                }*/

                //create anonymous PL/SQL command
				string block = " BEGIN " +
					" INSERT INTO " + universeTable.Sql +
                    " (ID_UNIVERSE_CLIENT, ID_GROUP_UNIVERSE_CLIENT, UNIVERSE_CLIENT, BLOB_UNIVERSE_CLIENT,ID_TYPE_UNIVERSE_CLIENT ,ID_UNIVERSE_CLIENT_DESCRIPTION ,DATE_CREATION, DATE_MODIFICATION, ACTIVATION) " +
					" VALUES " +
					" (" + schema.Label + ".seq_universe_client.nextval, " + idGroupUniverse + ", '" + universe + "', :1, " + idTypeUniverseClient + "," + idUniverseClientDescription + ",sysdate, sysdate, " + DBConstantes.ActivationValues.ACTIVATED + "); " +

                    /* TODO MODIFICATION
                     * Code add to improve the performance of the universe loading process
                    * 
                    " (ID_UNIVERSE_CLIENT, ID_GROUP_UNIVERSE_CLIENT, UNIVERSE_CLIENT, BLOB_UNIVERSE_CLIENT,ID_TYPE_UNIVERSE_CLIENT ,ID_UNIVERSE_CLIENT_DESCRIPTION ,DATE_CREATION, DATE_MODIFICATION, COMMENTARY, ACTIVATION) " +
                    " VALUES " +
                    " (" + schema.Label + ".seq_universe_client.nextval, " + idGroupUniverse + ", '" + universe + "', :1, " + idTypeUniverseClient + "," + idUniverseClientDescription + ",sysdate, sysdate," + levelsIdsField + ", " + DBConstantes.ActivationValues.ACTIVATED + "); " +*/

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
			catch (System.Exception e) {
				// Fermeture des structures
				try {
					if (ms != null) ms.Close();
					if (bf != null) bf = null;
					if (sqlCommand != null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch (System.Exception et) {
					throw (new MySessionDataAccessException("Impossible de libérer les ressources après échec de la méthode" + et));
				}
				throw (new MySessionDataAccessException("Echec de la sauvegarde de l'objet dans la base de donnée" + e));
			}
			//pas d'erreur
			try {
				// Fermeture des structures
				ms.Close();
				bf = null;
				if (sqlCommand != null) sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
				success = true;
			}
			catch (System.Exception et) {
				throw (new MySessionDataAccessException("Impossible de fermer la base de données", et));
			}
			#endregion

			return (success);
		}

		#endregion

		/// <summary>
		/// Vérifie l'existance d'un groupe d'univers
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="webSession">Session du client</param>
		/// <param name="GroupUniverseName">Non du groupe d'univers</param>
		/// <returns>True s'il existe, false sinon</returns>
		public static bool IsGroupUniverseExist(WebSession webSession, string GroupUniverseName){
			Table universeGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup);

			#region Request construction
			string sql = "select distinct " + universeGroupTable.Prefix + ".ID_GROUP_UNIVERSE_CLIENT, " + universeGroupTable.Prefix + ".GROUP_UNIVERSE_CLIENT ";
			sql += " from " + universeGroupTable.SqlWithPrefix;
			sql+=" where " + universeGroupTable.Prefix +".ID_LOGIN="+webSession.CustomerLogin.IdLogin;
			sql += " and " + universeGroupTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
			sql += " and UPPER(" + universeGroupTable.Prefix + ".GROUP_UNIVERSE_CLIENT)=UPPER('" + GroupUniverseName + "') ";
			#endregion

			#region Request execution
			DataSet ds;
			try{
				ds=webSession.Source.Fill(sql);
				if(ds.Tables[0].Rows.Count>0)return(true);
				return(false);
			}
			catch(System.Exception err){
				throw(new UniversListException("Impossible de vérifier l'existance d'un groupe d'univers",err));
			}
			#endregion

		}

		/// <summary>
		/// Création d'un groupe d'univers
		/// </summary>
		/// <remarks>testée</remarks>
		/// <param name="nameGroupUniverse">Nom du groupe d'univers</param>
		/// <param name="webSession">Session du client</param>
		public static bool CreateGroupUniverse(string nameGroupUniverse, WebSession webSession){
			Table universeGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup);
			Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Requête pour insérer les champs dans la table Group_universe_client	
			string sql = "INSERT INTO " + universeGroupTable.Sql
				+ " (ID_GROUP_UNIVERSE_CLIENT,ID_LOGIN,GROUP_UNIVERSE_CLIENT,DATE_CREATION,ACTIVATION) ";
			sql += "values (" + schema .Label+ ".seq_GROUPE_UNIVERSE_CLIENT.nextval," + webSession.CustomerLogin.IdLogin + ",'" + nameGroupUniverse + "',SYSDATE," + TNS.AdExpress.Constantes.DB.ActivationValues.ACTIVATED + ")";
			#endregion

			#region Request execution
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
		/// Vérifie s'il existe des univers dans un groupe d'univers
		/// </summary>
		/// <remarks>testée</remarks>
		/// <param name="webSession">Session du client</param>
		/// <param name="idGroupUniverse">Identifiant du groupe d'univers</param>
		/// <returns>True s'il existe, false sinon</returns>
		public static bool IsUniversInGroupUniverseExist(WebSession webSession, Int64 idGroupUniverse){
			
			Table universeGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup);
			Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

			#region Request construction
			string sql = "select " + universeTable.Prefix + ".UNIVERSE_CLIENT from " + universeTable.SqlWithPrefix + ", ";
			sql += " " + universeGroupTable.SqlWithPrefix;
			sql+=" where "+ universeGroupTable.Prefix +".ID_LOGIN="+webSession.CustomerLogin.IdLogin;
			sql += " and " + universeGroupTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
			sql += " and " + universeTable.Prefix +".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
			sql += " and " + universeGroupTable.Prefix + ".ID_GROUP_UNIVERSE_CLIENT = "+universeTable.Prefix+".ID_GROUP_UNIVERSE_CLIENT ";
			sql += " and " + universeGroupTable.Prefix + ".ID_GROUP_UNIVERSE_CLIENT=" + idGroupUniverse + " ";
			#endregion

			#region Request execution
			DataSet ds;
			try{
				ds=webSession.Source.Fill(sql);
				if(ds.Tables[0].Rows.Count>0)return(true);
				return(false);
			}
			catch(System.Exception err){
				throw(new UniversListException("Impossible de vérifier s'il existe des univers dans un groupe d'univers",err));
			}
			#endregion
		}

		/// <summary>
		/// Supprime un groupe d'univers de la table GROUP_UNIVERSE_CLIENT
		/// </summary>
		/// <remarks>testée</remarks>
		/// <param name="idGroupUniverse">Identifiant du Groupe d'univers</param>
		/// <param name="webSession">Session du client</param>
		public static void DropGroupUniverse(Int64 idGroupUniverse,WebSession webSession){

			Table universeGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup);
			Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);


			#region Request construction
			string sql = "delete from " + universeGroupTable .Sql+ " ";
			sql += " where " + schema.Label+ ".GROUP_UNIVERSE_CLIENT.ID_GROUP_UNIVERSE_CLIENT=" + idGroupUniverse + "";
			#endregion

			#region Request execution
			try{
				webSession.Source.Delete(sql);
			}
			catch(System.Exception err){
				throw(new UniversListException("Impossible de Supprimer un groupe d'univers de la table GROUP_UNIVERSE_CLIENT",err));
			}
			#endregion
		}

		/// <summary>
		/// Supprime un univers dans la table UNIVERSE_CLIENT
		/// </summary>
		/// <remarks>testée</remarks>
		/// <param name="idUniverse">Identifiant du répertoire</param>
		/// <param name="webSession">Session du client</param>
		public static bool DropUniverse(Int64 idUniverse,WebSession webSession){
			Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);
			Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql = "delete from " + universeTable.Sql + " ";
			sql += " where " + universeTable.Sql+ ".ID_UNIVERSE_CLIENT=" + idUniverse + "";
			#endregion

			#region Request execution
			try{
				webSession.Source.Delete(sql);
				return(true);
			}
			catch(System.Exception){
				return(false);
			}
			#endregion
		}

		/// <summary>
		///  Renommer un groupe d'univers
		/// </summary>
		/// <remarks>testée</remarks>
		/// <param name="newName">Nouveau nom</param>
		/// <param name="idGroupUniverse">Identifiant du groupe d'univers</param>
		/// <param name="webSession">Session du client</param>
		public static void RenameGroupUniverse(string newName, Int64 idGroupUniverse,WebSession webSession){
			Table universeGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverseGroup);

			#region Requête pour mettre à jour le nom du Groupe d'univers dans la table	
			string sql1 = "update " + universeGroupTable.Sql + " ";
			sql1+="set GROUP_UNIVERSE_CLIENT ='"+newName+"', DATE_MODIFICATION = SYSDATE ";
			sql1+="where ID_GROUP_UNIVERSE_CLIENT ="+idGroupUniverse+"";
			#endregion

			#region Request execution
			try{
				webSession.Source.Update(sql1);
			}
			catch(System.Exception err){
				throw(new UniversListException("Impossible de Renommer un groupe d'univers",err));
			}
			#endregion
				   
		}

		/// <summary>
		///  Renommer un univers
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="newName">Nouveau nom</param>
		/// <param name="idUniverse">Identifiant de l'univers</param>
		/// <param name="webSession">Session du client</param>
		public static void RenameUniverse(string newName, Int64 idUniverse,WebSession webSession){
			Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);
			
			#region Requête pour mettre à jour le nom de l'univers dans la table	
			string sql1 = " update  " + universeTable.Sql + " ";
			sql1+=" set  UNIVERSE_CLIENT='"+newName+"' ,DATE_MODIFICATION=SYSDATE ";
			sql1+=" where  ID_UNIVERSE_CLIENT="+idUniverse+" ";
			#endregion

			#region Request execution
			try{
				webSession.Source.Update(sql1);
			}
			catch(System.Exception err){
				throw(new UniversListException("Impossible de Renommer un univers",err));
			}
			#endregion

		}

		/// <summary>
		/// Déplace un univers d'un Groupe d'univers vers un autre
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idOldGroupUniverse">Identifiant du répertoire source</param>
		/// <param name="idNewGroupUniverse">Identifiant du répertoire de destination</param>
		/// <param name="idUniverse">Identifiant Univers</param>
		/// <param name="webSession">Session du client</param>	
		public static void MoveUniverse(Int64 idOldGroupUniverse,Int64 idNewGroupUniverse,Int64 idUniverse,WebSession webSession){
			Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

			#region Request construction
			string sql = "UPDATE " + universeTable.Sql;
			sql+=" SET ID_GROUP_UNIVERSE_CLIENT="+idNewGroupUniverse+", DATE_MODIFICATION=sysdate ";
			sql+=" WHERE ID_GROUP_UNIVERSE_CLIENT="+idOldGroupUniverse+"";
			sql+=" and ID_UNIVERSE_CLIENT="+idUniverse+"";
			#endregion

			#region Request execution
			try{
				webSession.Source.Update(sql);
			}
			catch(System.Exception err){
				throw(new UniversListException("Impossible de Déplacer un univers d'un Groupe d'univers vers un autre",err));
			}
			#endregion

		}

		/// <summary>
		/// Retourne le nom d'une session à partir de son identifiant
		/// </summary>
		/// <remarks>A Tester</remarks>
		/// <param name="idUnivers">Identifiant de l'univers à charger</param>
		/// <param name="webSession">Session du client</param>
		/// <returns></returns>
		public static string GetUniverse(Int64 idUnivers,WebSession webSession){
			Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

			#region Request construction
			string sql=" select UNIVERSE_CLIENT ";
			sql += " FROM " + universeTable.Sql + " ";
			sql+=" WHERE ID_UNIVERSE_CLIENT="+idUnivers+"";
			#endregion

			#region Request execution
			try{
				return(webSession.Source.Fill(sql).Tables[0].Rows[0][0].ToString());
			}
			catch(System.Exception err){
				throw(new UniversListException("Impossible de Retourner le nom d'une session à partir de son identifiant",err));
			}
			#endregion
		}

		#region UpdateUniverse
		/// <summary>
		///  Renommer un univers
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idUniverse">Identifiant de l'univers</param>
		/// <param name="webSession">Session du client</param>
		public static bool UpdateUniverse(Int64 idUniverse, WebSession webSession, Int64 idUniverseClientDescription, int idTypeUniverseClient, ArrayList alUniverseTreeNode) {

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
					throw (new UniversListException("Impossible d'ouvrir la base de données :" + e.Message));
				}
			}
			#endregion

			#region Sérialisation et Mise à jour de la session
			OracleCommand sqlCommand = null;
			MemoryStream ms = null;
			BinaryFormatter bf = null;
			byte[] binaryData = null;

			try {
				Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

				//"Serialization"
				ms = new MemoryStream();
				bf = new BinaryFormatter();
				bf.Serialize(ms, alUniverseTreeNode);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

				//mise à jour de la session
				string sql = " UPDATE  " + universeTable.Sql + " ";
				sql += " SET  BLOB_UNIVERSE_CLIENT = :1,DATE_MODIFICATION=SYSDATE, ID_TYPE_UNIVERSE_CLIENT=" + idTypeUniverseClient + ",ID_UNIVERSE_CLIENT_DESCRIPTION =" + idUniverseClientDescription;
				sql += " WHERE  ID_UNIVERSE_CLIENT=" + idUniverse + " ";

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
			catch (System.Exception e) {
				// Fermeture des structures
				try {
					if (ms != null) ms.Close();
					if (bf != null) bf = null;
					if (sqlCommand != null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch (System.Exception et) {
					throw (new UniversListException(" Impossible de libérer les ressources après échec de la méthode : " + et.Message));
				}
				throw (new UniversListException(" Echec de la sauvegarde de l'objet dans la base de donnée : " + e.Message));
			}
			//pas d'erreur
			try {
				// Fermeture des structures
				ms.Close();
				bf = null;
				if (sqlCommand != null) sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
				success = true;
			}
			catch (System.Exception et) {
				throw (new UniversListException(" Impossible de fermer la base de données : " + et.Message));
			}
			#endregion

			return (success);

		}

        /// <summary>
        ///  Renommer un univers
        /// </summary>
        /// <remarks>Testée</remarks>
        /// <param name="idUniverse">Identifiant de l'univers</param>
        /// <param name="webSession">Session du client</param>
        public static bool UpdateUniverse(Int64 idUniverse, WebSession webSession, Int64 idUniverseClientDescription, int idTypeUniverseClient, ArrayList alUniverseTreeNode, int filter){

            #region Ouverture de la base de données
            OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
            bool DBToClosed = false;
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Closed){
                DBToClosed = true;
                try{
                    connection.Open();
                }
                catch (System.Exception e){
                    throw (new UniversListException("Impossible d'ouvrir la base de données :" + e.Message));
                }
            }
            #endregion

            #region Sérialisation et Mise à jour de la session
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;
            TreeNode tree;
            Int64 filterId = -1;
            string filterSet = string.Empty;

            try{
                Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

                //"Serialization"
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, alUniverseTreeNode);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //mise à jour de la session
                if (filter == 1){
                    if (webSession.SelectionUniversMedia.FirstNode.Nodes[0] != null){
                        tree = webSession.SelectionUniversMedia.FirstNode.Nodes[0];
                        filterId = ((TNS.AdExpress.Web.Core.Sessions.LevelInformation)(tree.Tag)).ID;
                        filterSet = ", FILTER = " + filterId;
                    }
                }

                string sql = " UPDATE  " + universeTable.Sql + " ";
                sql += " SET  BLOB_UNIVERSE_CLIENT = :1,DATE_MODIFICATION=SYSDATE, ID_TYPE_UNIVERSE_CLIENT=" + idTypeUniverseClient + ",ID_UNIVERSE_CLIENT_DESCRIPTION =" + idUniverseClientDescription + filterSet;
                sql += " WHERE  ID_UNIVERSE_CLIENT=" + idUniverse + " ";

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
                    if (bf != null) bf = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et){
                    throw (new UniversListException(" Impossible de libérer les ressources après échec de la méthode : " + et.Message));
                }
                throw (new UniversListException(" Echec de la sauvegarde de l'objet dans la base de donnée : " + e.Message));
            }
            //pas d'erreur
            try{
                // Fermeture des structures
                ms.Close();
                bf = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
                success = true;
            }
            catch (System.Exception et){
                throw (new UniversListException(" Impossible de fermer la base de données : " + et.Message));
            }
            #endregion

            return (success);

        }

		/// <summary>
		///  Update Universe
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idUniverse">id Universe </param>
		/// <param name="webSession">Session client</param>
		/// <param name="idTypeUniverseClient">id Type universe client</param>
		/// <param name="idUniverseClientDescription">Id universe client description</param>
		/// <param name="universesToSave">Universes to save</param>
		public static bool UpdateUniverse(Int64 idUniverse, WebSession webSession, Int64 idUniverseClientDescription, int idTypeUniverseClient, Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universesToSave) {

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
					throw (new UniversListException("Impossible d'ouvrir la base de données :" + e.Message));
				}
			}
			#endregion

			#region Sérialisation et Mise à jour de la session
			OracleCommand sqlCommand = null;
			MemoryStream ms = null;
			BinaryFormatter bf = null;
			byte[] binaryData = null;
            /* TODO MODIFICATION
             * Code add to improve the performance of the universe loading process
            * 
            List<long> levelsIds = null;
            string levelsIdsField = string.Empty;*/

            try {
				Table universeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerUniverse);

				//"Serialization"
				ms = new MemoryStream();
				bf = new BinaryFormatter();
				bf.Serialize(ms, universesToSave);
				binaryData = new byte[ms.GetBuffer().Length];
				binaryData = ms.GetBuffer();

                /* TODO MODIFICATION
                 * Code add to improve the performance of the universe loading process
                * 
                //Get the levels identifiers from the universes selected
                levelsIds = WebSession.GetLevelListId(universesToSave);

                if (levelsIds != null && levelsIds.Count > 0) {
                    for (int j = 0; j < levelsIds.Count; j++) {
                        levelsIdsField += levelsIds[j] + "-";
                    }
                    levelsIdsField = levelsIdsField.Substring(0, levelsIdsField.Length - 1);
                }*/

                //mise à jour de la session
				string sql = " UPDATE  " + universeTable.Sql + " ";
                /* TODO MODIFICATION
                 * Code add to improve the performance of the universe loading process
                * 
                sql += " SET  BLOB_UNIVERSE_CLIENT = :1,DATE_MODIFICATION=SYSDATE, ID_TYPE_UNIVERSE_CLIENT=" + idTypeUniverseClient + ",ID_UNIVERSE_CLIENT_DESCRIPTION =" + idUniverseClientDescription + ", COMMENTARY =" + levelsIdsField;*/
                sql += " SET  BLOB_UNIVERSE_CLIENT = :1,DATE_MODIFICATION=SYSDATE, ID_TYPE_UNIVERSE_CLIENT=" + idTypeUniverseClient + ",ID_UNIVERSE_CLIENT_DESCRIPTION =" + idUniverseClientDescription;
				sql += " WHERE  ID_UNIVERSE_CLIENT=" + idUniverse + " ";

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
			catch (System.Exception e) {
				// Fermeture des structures
				try {
					if (ms != null) ms.Close();
					if (bf != null) bf = null;
					if (sqlCommand != null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch (System.Exception et) {
					throw (new UniversListException(" Impossible de libérer les ressources après échec de la méthode : " + et.Message));
				}
				throw (new UniversListException(" Echec de la sauvegarde de l'objet dans la base de donnée : " + e.Message));
			}
			//pas d'erreur
			try {
				// Fermeture des structures
				ms.Close();
				bf = null;
				if (sqlCommand != null) sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
				success = true;
			}
			catch (System.Exception et) {
				throw (new UniversListException(" Impossible de fermer la base de données : " + et.Message));
			}
			#endregion

			return (success);

		}
		#endregion

		#endregion

	}
}

