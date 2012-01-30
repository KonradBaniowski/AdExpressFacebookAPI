using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Exceptions;
using System.Data;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using Oracle.DataAccess.Client;
using System.IO;

namespace TNS.AdExpress.Web.Core.DataAccess.ClassificationList
{
    /// <summary>
    /// Insertion customised levels DAL
    /// </summary>
    public class InsertionLevelDAL
    {
        #region GetData
        /// <summary>
        ///Get list fo insertions customised levels saved 
        /// </summary>
        /// <param name="webSession">Session</param>
        /// <returns>inseertion customised levels list</returns>
        public static DataSet GetData(WebSession webSession)
        {

            #region Construction de la requête
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);
            Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);

            string sql = "select " + directoryTable.Prefix + ".ID_GROUP_INSERTION_SAVE, " + directoryTable.Prefix + ".GROUP_INSERTION_SAVE, " + insertionsSavedLevelsTable.Prefix + ".ID_INSERTION_SAVE, " + insertionsSavedLevelsTable.Prefix + ".INSERTION_SAVE ";
            sql += " , " + insertionsSavedLevelsTable.Prefix + ".COLUMN_LIST, " + insertionsSavedLevelsTable.Prefix + ".LEVE_LIST ";
            sql += " from " + directoryTable.SqlWithPrefix + " , " + insertionsSavedLevelsTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin.ToString();
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED;
            sql += " and (" + insertionsSavedLevelsTable.Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED + " ) ";
            sql += " and " + directoryTable.Prefix + ".ID_GROUP_INSERTION_SAVE=" + insertionsSavedLevelsTable.Prefix + ".ID_GROUP_INSERTION_SAVE(+) ";
            sql += " order by " + directoryTable.Prefix + ".GROUP_INSERTION_SAVE, " + insertionsSavedLevelsTable.Prefix + ".INSERTION_SAVE ";

            #endregion

            #region Execution of SQL statement
            try
            {
                return (webSession.Source.Fill(sql));
            }
            catch (System.Exception err)
            {
                throw (new InsertionLevelDALException("Impossible to obtain list of insertion levels save for a customer", err));
            }
            #endregion

        }
        /// <summary>
        ///Get list fo insertions customised levels saved 
        /// </summary>
        /// <param name="webSession">Session</param>
        /// <param name="idVehicle">ID media type</param>
        /// <returns>inseertion customised levels list</returns>
        public static DataSet GetData(WebSession webSession, long idVehicle)
        {

            #region Construction de la requête
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);
            Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);

            string sql = "select " + directoryTable.Prefix + ".ID_GROUP_INSERTION_SAVE, " + directoryTable.Prefix + ".GROUP_INSERTION_SAVE, " + insertionsSavedLevelsTable.Prefix + ".ID_INSERTION_SAVE, " + insertionsSavedLevelsTable.Prefix + ".INSERTION_SAVE ";
            sql += " , " + insertionsSavedLevelsTable.Prefix + ".COLUMN_LIST, " + insertionsSavedLevelsTable.Prefix + ".LEVE_LIST ";
            sql += " from " + directoryTable.SqlWithPrefix + " , " + insertionsSavedLevelsTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin.ToString();          
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED;
            sql += " and " + insertionsSavedLevelsTable.Prefix + ".ID_VEHICLE = " + idVehicle;
            sql += " and (" + insertionsSavedLevelsTable.Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED + " ) ";
            sql += " and " + directoryTable.Prefix + ".ID_GROUP_INSERTION_SAVE=" + insertionsSavedLevelsTable.Prefix + ".ID_GROUP_INSERTION_SAVE(+) ";
            sql += " order by " + directoryTable.Prefix + ".GROUP_INSERTION_SAVE, " + insertionsSavedLevelsTable.Prefix + ".INSERTION_SAVE ";

            #endregion

            #region Execution of SQL statement
            try
            {
                return (webSession.Source.Fill(sql));
            }
            catch (System.Exception err)
            {
                throw (new InsertionLevelDALException("Impossible to obtain list of insertion levels save for a customer", err));
            }
            #endregion

        }
        /// <summary>
        ///Get data for saved customised levels  
        /// </summary>
        /// <param name="webSession">Session</param>
        /// <param name="idVehicle">ID media type</param>
        /// <returns>inseertion customised levels list</returns>
        public static DataSet GetInsertionSaveData(WebSession webSession, string idInsertionSaveList)
        {
            try
            {
                #region Construction de la requête
                Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);

                string sql = "select * ";
                sql += " from " + insertionsSavedLevelsTable.SqlWithPrefix;
                sql += " where ";
                sql += "  " + insertionsSavedLevelsTable.Prefix + ".ID_INSERTION_SAVE in ( " + idInsertionSaveList + " ) ";
                sql += " and " + insertionsSavedLevelsTable.Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED + " ";
                sql += " order by " + insertionsSavedLevelsTable.Prefix + ".INSERTION_SAVE, " + insertionsSavedLevelsTable.Prefix + ".ID_INSERTION_SAVE ";

                #endregion

                return (webSession.Source.Fill(sql));
            }
            catch (System.Exception err)
            {
                throw (new InsertionLevelDALException("Impossible to obtain list of insertion levels save for a customer", err));
            }

        }
        /// <summary>
        /// Delete insertion save
        /// </summary>
        /// <param name="idInsertionsave">id Insertion save</param>
        /// <param name="webSession">Session</param>
        public static bool DropInsertionSave(Int64 idInsertionsave, WebSession webSession)
        {
            Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);
            Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

            #region Request construction
            string sql = "delete from " + insertionsSavedLevelsTable.Sql + " ";
            sql += " where " + insertionsSavedLevelsTable.Sql + ".ID_INSERTION_SAVE=" + idInsertionsave + "";
            #endregion

            #region Request execution
            try
            {
                webSession.Source.Delete(sql);
                return (true);
            }
            catch (System.Exception)
            {
                return (false);
            }
            #endregion
        }
        #endregion

        /// <summary>
        ///  Renommer Insertion Save
        /// </summary>
        /// <param name="newName">New label</param>
        /// <param name="idInsertionSave">id Insertion Save</param>
        /// <param name="webSession">Session</param>
        public static void RenameInsertionSave(string newName, Int64 idInsertionSave, WebSession webSession)
        {
            Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);

            #region Requête pour mettre à jour le nom de l'univers dans la table
            string sql1 = " update  " + insertionsSavedLevelsTable.Sql + " ";
            sql1 += " set  INSERTION_SAVE='" + newName + "' ,DATE_MODIFICATION=SYSDATE ";
            sql1 += " where  ID_INSERTION_SAVE=" + idInsertionSave + " ";
            #endregion

            #region Request execution
            try
            {
                webSession.Source.Update(sql1);
            }
            catch (System.Exception err)
            {
                throw (new UniversListException("Impossible to rename insertion save", err));
            }
            #endregion

        }
        /// <summary>
        /// Move insertion save to another directory
        /// </summary>
        /// <remarks>Testée</remarks>
        /// <param name="idOldDirectory">Identifiant old directory</param>
        /// <param name="idNewDirectory">Identifiant new directory</param>
        /// <param name="idInsertionSave">Id insertion save</param>
        /// <param name="webSession">Session</param>	
        public static void MoveInsertionSave(Int64 idOldDirectory, Int64 idNewDirectory, Int64 idInsertionSave, WebSession webSession)
        {
            Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);

            #region Request construction
            string sql = "UPDATE " + insertionsSavedLevelsTable.Sql;
            sql += " SET ID_GROUP_INSERTION_SAVE=" + idNewDirectory + ", DATE_MODIFICATION=sysdate ";
            sql += " WHERE ID_GROUP_INSERTION_SAVE=" + idOldDirectory + "";
            sql += " and ID_INSERTION_SAVE=" + idInsertionSave + "";
            #endregion

            #region Request execution
            try
            {
                webSession.Source.Update(sql);
            }
            catch (System.Exception err)
            {
                throw (new UniversListException("Impossible to move insertion save to another directory", err));
            }
            #endregion

        }

        #region GetGroupInsertionLevels
        /// <summary>
        /// Donne la liste des Groupes des niveaux insertions
        /// </summary>
        /// <remarks>Testée</remarks>
        /// <param name="webSession">Session du client</param>
        /// <returns>Liste des Groupes des niveaux insertions</returns>
        public static DataSet GetGroupInsertionLevels(WebSession webSession)
        {
           

            #region Request execution
            try
            {
                Table insertionLevelGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);
               
                #region Request construction
                //Requête pour récupérer tous les univers d'un idLogin
                string sql = "select distinct " + insertionLevelGroupTable.Prefix + ".ID_GROUP_INSERTION_SAVE, " + insertionLevelGroupTable.Prefix + ".GROUP_INSERTION_SAVE ";
                sql += " from " + insertionLevelGroupTable.SqlWithPrefix;
                sql += " where " + insertionLevelGroupTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
                sql += " and " + insertionLevelGroupTable.Prefix + ".ACTIVATION<" + DBConstantes.ActivationValues.UNACTIVATED + " ";
                sql += " order by " + insertionLevelGroupTable.Prefix + ".GROUP_INSERTION_SAVE ";
                #endregion

                return (webSession.Source.Fill(sql));
            }
            catch (System.Exception err)
            {
                throw (new InsertionLevelDALException("Impossible to get the list of insertions directories", err));
            }
            #endregion

        }

        /// <summary>
        /// Chekc if group insertion save exists
        /// </summary>  
        /// <param name="webSession">Session</param>
        /// <param name="groupInsertionSaveName">Group Insertion Save Name</param>
        /// <returns>True if exists, false if not</returns>
        public static bool IsGroupInsertionSaveExist(WebSession webSession, string groupInsertionSaveName)
        {
            Table insertionLevelGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);

            #region Request construction
            string sql = " select distinct " + insertionLevelGroupTable.Prefix + ".ID_GROUP_INSERTION_SAVE, " + insertionLevelGroupTable.Prefix + ".GROUP_INSERTION_SAVE ";
            sql += " from " + insertionLevelGroupTable.SqlWithPrefix;
            sql += " where " + insertionLevelGroupTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
            sql += " and " + insertionLevelGroupTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and UPPER(" + insertionLevelGroupTable.Prefix + ".GROUP_INSERTION_SAVE)=UPPER('" + groupInsertionSaveName + "') ";
            #endregion

            #region Request execution
            DataSet ds;
            try
            {
                ds = webSession.Source.Fill(sql);
                if (ds.Tables[0].Rows.Count > 0) return (true);
                return (false);
            }
            catch (System.Exception err)
            {
                throw (new InsertionLevelDALException("Impossible to check if group insertion save exists", err));
            }
            #endregion

        }
        /// <summary>
        /// Create Group Insertion Save
        /// </summary>
        /// <remarks>testée</remarks>
        /// <param name="nameGroupInsertionSave">name Group Insertion Save</param>
        /// <param name="webSession">Session</param>
        public static bool CreateGroupInsertionSave(string nameGroupInsertionSave, WebSession webSession)
        {
            Table insertionLevelGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);
            Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

            #region Requête pour insérer les champs dans la table Group_universe_client
            string sql = " INSERT INTO " + insertionLevelGroupTable.Sql
                + " (ID_GROUP_INSERTION_SAVE,ID_LOGIN,GROUP_INSERTION_SAVE,DATE_CREATION,ACTIVATION) ";
            sql += " values (" + schema.Label + ".SEQ_GROUP_INSERTION_SAVE.nextval," + webSession.CustomerLogin.IdLogin + ",'" + nameGroupInsertionSave + "',SYSDATE," + TNS.AdExpress.Constantes.DB.ActivationValues.ACTIVATED + ")";
            #endregion

            #region Request execution
            try
            {
                webSession.Source.Insert(sql);
                return (true);
            }
            catch (System.Exception)
            {
                return (false);
            }
            #endregion
        }
        #endregion

        /// <summary>
        ///  Rename group insertion save
        /// </summary>
        /// <param name="newName">new label</param>
        /// <param name="idGroupUniverse">Id Group Insertion Save</param>
        /// <param name="webSession">Session</param>
        public static void RenameGroupInsertionSave(string newName, Int64 idGroupInsertionSave, WebSession webSession)
        {
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);

            #region Requête pour mettre à jour le nom du Groupe d'univers dans la table
            string sql1 = "update " + directoryTable.Sql + " ";
            sql1 += "set GROUP_INSERTION_SAVE ='" + newName + "', DATE_MODIFICATION = SYSDATE ";
            sql1 += "where ID_GROUP_INSERTION_SAVE =" + idGroupInsertionSave + "";
            #endregion

            #region Request execution
            try
            {
                webSession.Source.Update(sql1);
            }
            catch (System.Exception err)
            {
                throw (new UniversListException("Impossible to rename  Group Insertion Save", err));
            }
            #endregion

        }

        /// <summary>
        /// Delete insertion saved levels directory
        /// </summary>
        /// <param name="idGroupInsertionSave">id Group Insertion Save</param>
        /// <param name="webSession">Session</param>
        public static void DropGroupInsertionSave(Int64 idGroupInsertionSave, WebSession webSession)
        {

            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);
            Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);


            #region Request construction
            string sql = "delete from " + directoryTable.Sql + " ";
            sql += " where ID_GROUP_INSERTION_SAVE=" + idGroupInsertionSave + "";
            #endregion

            #region Request execution
            try
            {
                webSession.Source.Delete(sql);
            }
            catch (System.Exception err)
            {
                throw (new UniversListException("Impossible to delete directory of saved levels", err));
            }
            #endregion
        }

        /// <summary>
        /// Check if insertion save belong to a group
        /// </summary>
        /// <remarks>testée</remarks>
        /// <param name="webSession">Session du client</param>
        /// <param name="idGroupInsertionSave">Id insertion directoy</param>
        /// <returns>True s'il existe, false sinon</returns>
        public static bool IsInsertionInGroupInsertionExist(WebSession webSession, Int64 idGroupInsertionSave)
        {
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);
            Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);

            #region Request construction
            string sql = "select " + insertionsSavedLevelsTable.Prefix + ".INSERTION_SAVE from " + insertionsSavedLevelsTable.SqlWithPrefix + ", ";
            sql += " " + directoryTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and " + insertionsSavedLevelsTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and " + directoryTable.Prefix + ".ID_GROUP_INSERTION_SAVE = " + insertionsSavedLevelsTable.Prefix + ".ID_GROUP_INSERTION_SAVE ";
            sql += " and " + directoryTable.Prefix + ".ID_GROUP_INSERTION_SAVE=" + idGroupInsertionSave + " ";
            #endregion

            #region Request execution
            DataSet ds;
            try
            {
                ds = webSession.Source.Fill(sql);
                if (ds.Tables[0].Rows.Count > 0) return (true);
                return (false);
            }
            catch (System.Exception err)
            {
                throw (new UniversListException("Impossible to check if saved levels exists into directory", err));
            }
            #endregion
        }


        #region UpdateInsertionLevels        
        /// <summary>
        ///  Update Insertion customised Levels
        /// </summary>
        /// <param name="idInsertionSave">id Insertion Save </param>
        /// <param name="webSession">Session client</param>
        /// <param name="IdVehicle">id media type</param>
        /// <param name="idLevelList">id Level List</param>
        /// <param name="idColumnList">Id Column list</param>
        /// <returns>True if upadte is right</returns>
        public static bool UpdateInsertionLevels(Int64 idInsertionSave, WebSession webSession, Int64 IdVehicle, string idLevelList,string idColumnList)
        {

            #region Ouverture de la base de données
            OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
            bool DBToClosed = false;
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    connection.Open();
                }
                catch (System.Exception e)
                {
                    throw (new InsertionLevelDALException("Impossible to open database :" + e.Message));
                }
            }
            #endregion

            #region Sérialisation et Mise à jour de la session
            OracleCommand sqlCommand = null;
          

            try
            {
                Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);


                //mise à jour de la session
                string sql = " UPDATE  " + insertionsSavedLevelsTable.Sql + " ";
                sql += " SET  DATE_MODIFICATION=SYSDATE, ID_VEHICLE=" + IdVehicle + ",LEVE_LIST ='" + idLevelList + "',COLUMN_LIST ='" + idColumnList + "'";
                sql += " WHERE  ID_INSERTION_SAVE=" + idInsertionSave + " ";

                //Exécution de la requête
                sqlCommand = new OracleCommand(sql);
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;                        
                //Execution PL/SQL block
                sqlCommand.ExecuteNonQuery();

            }
            #endregion

            #region Error management 
            catch (System.Exception e)
            {
                // Fermeture des structures
                try
                {                 
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et)
                {
                    throw (new InsertionLevelDALException(" Impossible to free ressources : " + et.Message));
                }
                throw (new InsertionLevelDALException(" Impossible to update data : " + e.Message));
            }
            //pas d'erreur
            try
            {
                // Fermeture des structures              
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
                success = true;
            }
            catch (System.Exception et)
            {
                throw (new InsertionLevelDALException(" Impossible to close database: " + et.Message));
            }
            #endregion

            return (success);

        }
        #endregion

        #region IsUniverseExist
        /// <summary>
        /// Check if an insertion levels group already exist with the same label
        /// </summary>
        /// <remarks>Testée</remarks>
        /// <param name="webSession">Sessiont</param>
        /// <param name="InsertionLevelsName">Insertion Levels Name</param>
        /// <returns>True if exists else false</returns>
        public static bool IsInsertionLevelsExist(WebSession webSession, string InsertionLevelsName)
        {
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerInsertionLevelsGroup);
            Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);


            #region Request construction
            string sql = "select "+insertionsSavedLevelsTable.Prefix +".INSERTION_SAVE from " + insertionsSavedLevelsTable.SqlWithPrefix + ", ";
            sql += " " + directoryTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and " + insertionsSavedLevelsTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and " + directoryTable.Prefix + ".ID_GROUP_INSERTION_SAVE = " + insertionsSavedLevelsTable.Prefix + ".ID_GROUP_INSERTION_SAVE ";
            sql += " and UPPER(" + insertionsSavedLevelsTable.Prefix + ".INSERTION_SAVE) like UPPER('" + InsertionLevelsName + "')";
            #endregion

            #region Request execution
            DataSet ds;
            try
            {
                ds = webSession.Source.Fill(sql);
                if (ds.Tables[0].Rows.Count > 0) return (true);
                return (false);
            }
            catch (System.Exception err)
            {
                throw (new InsertionLevelDALException("Impossible to check Insertion result label already exists into database", err));
            }
            #endregion

        }
        #endregion

        #region  Save Insertion customised Levels
        /// <summary>
        /// Save Insertion customised Levels
        /// </summary>
        /// <param name="idGroupInsertionSave">id Group Insertion levels</param>
        /// <param name="insertionSave">Insertion levels to save label</param>
        /// <param name="idVehicle">ID media type</param>
        /// <param name="idLevelList">id Level List</param>
        /// <param name="idColumList">id Column List</param>
        /// <param name="webSession">Session utilisateur</param>
        /// <returns>True if success</returns>
        public static bool SaveInsertionLeves(Int64 idGroupInsertionSave, string insertionSave, Int64 idVehicle, string idLevelList,string idColumnList, WebSession webSession)
        {

            #region Ouverture de la base de données
            OracleConnection connection = (OracleConnection)webSession.Source.GetSource();
            bool DBToClosed = false;
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                DBToClosed = true;
                try
                {
                    connection.Open();
                }
                catch (System.Exception e)
                {
                    throw (new UniversListException("Impossible to open database", e));
                }
            }
            #endregion


            #region Save insertion levels
            OracleCommand sqlCommand = null;
         

            try
            {
                Table insertionsSavedLevelsTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.insertionLevelsSave);
                Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
            
                //create anonymous PL/SQL command
                string block = " BEGIN " +
                    " INSERT INTO " + insertionsSavedLevelsTable.Sql +
                    " (ID_INSERTION_SAVE, ID_GROUP_INSERTION_SAVE, INSERTION_SAVE, ID_VEHICLE ,LEVE_LIST ,COLUMN_LIST,DATE_CREATION, DATE_MODIFICATION, ACTIVATION) " +
                    " VALUES " +
                    " (" + schema.Label + ".SEQ_INSERTION_SAVE.nextval, " + idGroupInsertionSave + ", '" + insertionSave + "',  " + idVehicle + ",'" + idLevelList + "','" + idColumnList + "',sysdate, sysdate, " + DBConstantes.ActivationValues.ACTIVATED + "); " +                
                    " END; ";
                sqlCommand = new OracleCommand(block);
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;              
                //Execute PL/SQL block
                sqlCommand.ExecuteNonQuery();

            }
            #endregion

            #region Error managementl'objet
            catch (System.Exception e)
            {
                // Fermeture des structures
                try
                {                 
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et)
                {
                    throw (new MySessionDataAccessException("Impossible de libérer les ressources après échec de la méthode" + et));
                }
                throw (new MySessionDataAccessException("Impossible to insert data " + e));
            }
            //pas d'erreur
            try
            {
                // Fermeture des structures            
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
                success = true;
            }
            catch (System.Exception et)
            {
                throw (new MySessionDataAccessException(" Impossible to close database:", et));
            }
            #endregion

            return (success);
        }
        #endregion
    }
}
