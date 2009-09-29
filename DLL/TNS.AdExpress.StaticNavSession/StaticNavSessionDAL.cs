using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Data;

using AdConst = TNS.AdExpress.Constantes.DB;
using AnubisConst = TNS.AdExpress.Anubis.Constantes;
using System.Collections;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.StaticNavSession
{
    public abstract class StaticNavSessionDAL : IStaticNavSessionDAL
    {
        #region Variables

        protected IDataSource source;

        #endregion

        public StaticNavSessionDAL(IDataSource source)
        {
            if (source == null)
                throw new ArgumentNullException("StaticNavSession constructor: datasource cannot be null");
            this.source = source;
        }

        #region IStaticNavSessionDAL Members

        public virtual void InsertData(AnubisConst.Result.type type, string alertTitle, int alertId, int loginId)
        {
            string insertCommand = String.Format(@"
                                        INSERT INTO {0}.{1} (ID_STATIC_NAV_SESSION, STATIC_NAV_SESSION, ID_PDF_RESULT_TYPE, PDF_NAME, STATUS, DATE_CREATION, DATE_MODIFICATION, ID_LOGIN, PDF_USER_FILENAME, DATE_EXEC)
                                        VALUES({2}.SEQ_STATIC_NAV_SESSION.NEXTVAL, NULL, {3}, '{4}', {5}, SYSDATE, SYSDATE, {6}, '{7}', NULL)", AdConst.Schema.UNIVERS_SCHEMA, AdConst.Tables.PDF_SESSION, 
                                        AdConst.Schema.UNIVERS_SCHEMA, type.GetHashCode(), alertId, AnubisConst.Result.status.newOne.GetHashCode(), loginId, alertTitle.Replace("'", "''"));

            this.source.Insert(insertCommand);
        }

        public virtual int InsertData(WebSession webSession, AnubisConst.Result.type resultType, string fileName)
        {
            throw new NotImplementedException("StaticNavSessionDAL.Insert() should be implemented in the inheriting object");
        }

        /// <summary>
        /// Delete the expired rows depending on today's date and
        /// the expiration date in the table. The plugin dictionary
        /// let you filter which type of rows should be deleted.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pluginList"></param>
        /// <param name="filePath"></param>
        public virtual void DeleteExpiredRequests(Dictionary<PluginType, PluginInformation> pluginList, string filePath)
        {
            // Selecting the expired rows
            DataTable expired = GetExpired(pluginList);

            foreach (DataRow row in expired.Rows)
            {
                // Checking if the file path 
                if (filePath.EndsWith("\\") == false)
                    filePath += "\\";

                // Deleting file
                System.IO.File.Delete(filePath + row["PDF_NAME"].ToString());

                // Deleting corresponding row
                DeleteRow(int.Parse(row["ID_STATIC_NAV_SESSION"].ToString()));
            }
        }

        /// <summary>
        /// Returns a datatable containing all the new rows
        /// </summary>
        public virtual DataTable Get()
        {
            return (get(AnubisConst.Result.status.newOne.GetHashCode(), null, false));
        }

        /// <summary>
        /// Returns a datatable containing all the rows corresponding
        /// to the given status
        /// </summary>
        /// <param name="status">Row status</param>
        public virtual DataTable Get(int status)
        {
            return (get(status, null, false));
        }


        /// <summary>
        /// Returns a datatable containing all the rows corresponding
        /// to the given status, filtered by a pluginList
        /// </summary>
        /// <param name="status">Row status</param>
        /// <param name="pluginList">Plugin list limitation</param>
        /// <returns>A datatable</returns>
        public virtual DataTable Get(int status, Dictionary<PluginType, PluginInformation> pluginList)
        {
            return (get(status, pluginList, false));
        }

        public virtual DataTable GetExpired(Dictionary<PluginType, PluginInformation> pluginList)
        {
            return (get(AnubisConst.Result.status.sent.GetHashCode(), pluginList, true));
        }

        private DataTable get(int status, Dictionary<PluginType, PluginInformation> pluginList, bool longevityConstraint)
        {
            string condition = String.Empty;

            // Preparing select query
            string selectCommand = @"
                    SELECT ID_STATIC_NAV_SESSION, PDF_NAME, ID_LOGIN, ID_PDF_RESULT_TYPE
                    FROM " + AdConst.Schema.UNIVERS_SCHEMA + "." + AdConst.Tables.PDF_SESSION;

            bool first = true;
            if (pluginList != null)
                foreach (PluginType plugin in pluginList.Keys)
                {
                    // Adding kind of record constraints
                    if (pluginList[plugin].ResultType > 0)
                    {
                        if (first)
                        {
                            condition += " WHERE (";
                            first = false;
                        }
                        else
                            condition += " OR ";

                        // Kind of record constraint
                        condition += "( ID_PDF_RESULT_TYPE = " + pluginList[plugin].ResultType + " ";
                        // Adding
                        if (longevityConstraint)
                            condition += "AND SYSDATE > DATE_CREATION + " + pluginList[plugin].Longevity.ToString() + " ";
                        condition += ")";
                    }
                }
            if (condition.Length < 1)
                condition += " WHERE ";
            else
                condition += ") AND ";

            // Adding date and status constraints
            condition += "STATUS = " + status.ToString();

            // Adding to select command
            selectCommand += condition;

            try
            {
                // Filling in datatable
                return (this.source.Fill(selectCommand).Tables[0]);
            }
            catch (System.Exception err)
            {
                throw (new NoDataException("Unable to load the list of files to delete", err));
            }        
        }

        /// <summary>
        /// Updates a row's status according to a given static nav
        /// session identifier.
        /// </summary>
        /// <param name="source">Data access interface</param>
        /// <param name="staticNavSessionId">Session identifier</param>
        /// <param name="status">Status to set on the row</param>
        /// <param name="longevity">How long the record should be valid from the update date</param>
        public virtual void UpdateStatus(int staticNavSessionId, int status)
        {
            // Preparing update command
            string updateCommand = @"
                UPDATE " + AdConst.Schema.UNIVERS_SCHEMA + "." + AdConst.Tables.PDF_SESSION + @"
                SET STATUS = " + status.ToString();
            updateCommand += ", DATE_MODIFICATION = SYSDATE";
            updateCommand += " WHERE id_static_nav_session = " + staticNavSessionId.ToString();

            // Updating
            this.source.Update(updateCommand);
        }

        public virtual void Sent(int stativNavSessionId)
        {
            this.UpdateStatus(stativNavSessionId, AnubisConst.Result.status.sent.GetHashCode());
        }

        /// <summary>
        /// Deletes a row according to a static nav session id
        /// </summary>
        /// <param name="source">Data access interface</param>
        /// <param name="staticNavSession">Identifier</param>
        public virtual void DeleteRow(int staticNavSession)
        {
            // Preparing delete command
            string deleteCommand =
                "DELETE " + AdConst.Schema.UNIVERS_SCHEMA + "." + AdConst.Tables.PDF_SESSION + @"
                 WHERE STATUS = " + AnubisConst.Result.status.sent + @"
                 AND EXPIRATION_DATE <= SYSDATE";

            try
            {
                this.source.Delete(deleteCommand);
            }
            catch (System.Exception err)
            {
                throw (new NoDataException("Unable to delete the old requests", err));
            }            
        }

        /// <summary>
        /// Gets a specific row corresponding to the given id
        /// </summary>
        /// <param name="staticNavSession">Static nav session id</param>
        /// <returns>A DataRow object</returns>
        public virtual DataRow GetRow(long staticNavSession)
        {
            string selectCommand = @"
                    SELECT ID_STATIC_NAV_SESSION, PDF_NAME, ID_LOGIN, ID_PDF_RESULT_TYPE
                    FROM " + AdConst.Schema.UNIVERS_SCHEMA + "." + AdConst.Tables.PDF_SESSION + @"
                    WHERE ID_STATIC_NAV_SESSION = " + staticNavSession.ToString();

            DataSet data = this.source.Fill(selectCommand);
            return (data.Tables[0].Rows[0]);
        }

        #endregion
    }
}
