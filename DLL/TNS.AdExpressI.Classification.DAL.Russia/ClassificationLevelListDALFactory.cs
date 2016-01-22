#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpressI.Classification.DAL.Exceptions;
using TNS.AdExpress.Domain.Level;
using TNS.Classification.Universe;
using TNS.AdExpressI.Common.DAL.Russia;

namespace TNS.AdExpressI.Classification.DAL.Russia
{
    public class ClassificationLevelListDALFactory : TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory
    {



        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="language"></param>

        public ClassificationLevelListDALFactory(IDataSource source, int language)
            : base(source, language)
        {
            _toLowerCase = false;
        }

        #endregion

        #region IClassificationLevelListDALFactory Implementation
        /// Get partial items list of a classification's level
        /// </summary>
        /// <param name="detailLevelItemInformation">Detail level informations</param>
        /// <param name="idList">classification items' identifier list</param>
        public override ClassificationLevelListDAL CreateClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, string idList)
        {
            return new ClassificationLevelListDALRussia(detailLevelItemInformation, idList, _language, _source);
        }
        /// <summary>	
        /// Get all items list of a classification's level
        /// </summary>
        /// <param name="detailLevelItemInformation">Detail level informations</param>
        public override ClassificationLevelListDAL CreateClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation)
        {
            return new ClassificationLevelListDALRussia(detailLevelItemInformation, _language, _source);

        }
        /// Get partial items list of a classification's level
        /// </summary>
        /// <param name="levelType">Classification level type</param>
        /// <param name="idList">classification items' identifier list</param>
        public override ClassificationLevelListDAL CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type levelType, string idList)
        {
            DetailLevelItemInformation detailLevelItemInformation = GetDetailLevelItemInformation(levelType);
            return new ClassificationLevelListDALRussia(detailLevelItemInformation, idList, _language, _source);
        }

        /// Get partial items list of a classification's level
        /// </summary>
        /// <param name="table">Target table used to build the list</param>
        /// <param name="idList">classification items' identifier list</param>
        public override ClassificationLevelListDAL CreateDefaultClassificationLevelListDAL(UniverseLevel level, string idList)
        {
            return new ClassificationLevelListDALRussia(level.TableName, idList, _language, _source);
        }
        #endregion

    }

    public class ClassificationLevelListDALRussia : ClassificationLevelListDAL
    {

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public ClassificationLevelListDALRussia(DetailLevelItemInformation detailLevelItemInformation, int language, IDataSource source)
            : this(detailLevelItemInformation, "", language, source)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public ClassificationLevelListDALRussia(DetailLevelItemInformation detailLevelItemInformation, string idList, int language, IDataSource source)
        {
            GetLevelList(detailLevelItemInformation.DataBaseIdField, idList, language, source, detailLevelItemInformation.DataBaseTableName);
        }

        private void GetLevelList(string id_field, string idList, int language, IDataSource source, string table)
        {
            _list = new Dictionary<long, string>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection conn = (SqlConnection)source.GetSource();

                Hashtable parameters = new Hashtable();

                // SP set Parameters
                parameters["@id_language"] = language;
                parameters["@list_type"] = id_field;
                parameters["@id_list"] = idList;

                dt = GetLevelList(conn, parameters).Tables[0];

                dt.TableName = table;
                _dataTable = dt;
            }
            catch (System.Exception ex)
            {
                throw (new ClassificationDALException("Impossible to load classification's items", ex));
            }
        #endregion

            #region Transformation of DataTable to dictionary
            try
            {
                foreach (DataRow currentRow in dt.Rows)
                {
                    _list.Add(long.Parse(currentRow[0].ToString()), currentRow[1].ToString());
                    _idListOrderByClassificationItem.Add(long.Parse(currentRow[0].ToString()));
                }
            }
            catch (System.Exception ext)
            {
                throw (new ClassificationDALException("Impossible to transfer data in the list", ext));
            }
            #endregion
        }


        /// Constructor of items list of classification's level
        /// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
        /// <param name="table">Target table used to build the list</param>
        /// <param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language identifier</param>
        /// <param name="source">Data source</param>
        public ClassificationLevelListDALRussia(string table, string idList, int language, IDataSource source)
        {
            GetLevelList("id_" + table, idList, language, source, table);
        }

        public static DataSet GetLevelList(SqlConnection conn, Hashtable parameters)
        {
            DataSet ds = new DataSet();

            conn.Open();

            SqlCommand cmd = conn.CreateCommand();

            // SP Construction    
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[cl_level_list_get]";


            CommonDAL.InitParams(parameters, cmd);

            // SP Execute
            using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
            {
                ad.Fill(ds);
            }

            conn.Close();
            return ds;
        }

    }

}
