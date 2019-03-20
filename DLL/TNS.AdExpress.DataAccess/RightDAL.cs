using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.DataAccess.Exceptions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using DbCst=TNS.AdExpress.Constantes.DB;
using CstClassif=TNS.AdExpress.Constantes.Classification;
using CustomerCst=TNS.AdExpress.Constantes.Customer;

namespace TNS.AdExpress.DataAccess {
    /// <summary>
    /// Right Data access class
    /// </summary>
    public class RightDAL {


        #region AdExpress Customer Access

        /// <summary>
        /// Vérifie l'existence du projet adExpress 
        /// avec au moins un module.
        /// Si true assigne idLogin
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>True if the customer can access to AdExpress Web site</returns>
        public static bool CanAccessToAdExpressDB(IDataSource source,Int64 loginId) {
            bool moduleExist=false;

            #region Tables initilization
            Table loginTable,rightAssignmentTable,moduleAssignmentTable;
            try {
                loginTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
                rightAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAssignment);
                moduleAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleAssignment);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Imossible to get table names",err));
            } 
            #endregion

            #region Request
            string sql="select  distinct "+moduleAssignmentTable.Prefix+".id_module";
            sql+=" from "+loginTable.SqlWithPrefix+", "+rightAssignmentTable.SqlWithPrefix+", "+moduleAssignmentTable.SqlWithPrefix+" ";
            sql+=" where "+loginTable.Prefix+".id_login="+rightAssignmentTable.Prefix+".id_login";
            sql+=" and "+loginTable.Prefix+".id_login="+moduleAssignmentTable.Prefix+".id_login";
            sql+=" and "+loginTable.Prefix+".id_login="+loginId+"";
            sql+=" and "+rightAssignmentTable.Prefix+".id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";
            sql+=" and "+loginTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
            sql+=" and "+rightAssignmentTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
            sql+=" and "+loginTable.Prefix+".date_expired>=sysdate";
            sql+=" and "+moduleAssignmentTable.Prefix+".date_beginning_module<=sysdate";
            sql+=" and "+moduleAssignmentTable.Prefix+".date_end_module>=sysdate";
            sql+=" and "+moduleAssignmentTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
            #endregion

            #region Execute request
            try {
                DataSet ds=source.Fill(sql);
                if(ds.Tables[0].Rows.Count>0) moduleExist=true;
                else moduleExist=false;
            }
            catch(System.Exception) {
                moduleExist=false;
            }
            #endregion

            return (moduleExist);
        }

        /// <summary>
        /// Check login password
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <returns>Login Id (-1 if the login is not valid)</returns>
        public static Int64 GetLoginId(IDataSource source,string login, string password) {
            Int64 loginId=-1;

            #region Tables initilization
            Table myloginTable;
            try {
                myloginTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightMyLogin);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names",err));
            } 
            #endregion

          

            #region Execution de la requête
            try {
                #region Construction de la requête
                string sql = " select " + myloginTable.Prefix + ".id_login";
                sql += " from " + myloginTable.SqlWithPrefix + " ";
                if (login.Split(' ').Length > 1 || password.Split(' ').Length > 1) return (loginId);

                sql += " where login=upper('" + login + "')";
                sql += " and password=upper('" + password + "')";
                sql += " and date_expired>=sysdate";
                sql += " and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
                #endregion

                DataSet ds=source.Fill(sql);
                foreach(DataRow currentRow in ds.Tables[0].Rows) {
                    loginId=Int64.Parse(currentRow[0].ToString());
                }
            }
            catch(System.Exception err) {
                return (loginId);
            }
            #endregion

            return (loginId);

        }
        #endregion

        #region Right last modification date
        /// <summary>
        /// Get Last modification date
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <returns>Last modification date</returns>
        public static DateTime LastModificationDate(IDataSource source,Int64 loginId) {
            DateTime lastModificationDate;

            #region Tables initilization
            Table rightAssignmentTable;
            try {
                rightAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAssignment);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names",err));
            } 
            #endregion

            #region Request
            string	sql=" select date_modification_right";
			sql+=" from "+rightAssignmentTable.SqlWithPrefix+" ";
            sql+=" where id_login="+loginId+"";
			sql+=" and id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";
            #endregion

            #region Execute request
            try {
                lastModificationDate=(DateTime)source.Fill(sql).Tables[0].Rows[0][0];
            }
            catch(System.Exception err) {
                throw (new RightDALException("Imossible to get last modification date",err));
            } 
            #endregion

            return (lastModificationDate);
        }
        #endregion

        #region Check if some tempates exist
        /// <summary>
        /// Check if some product tempates exist
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>True if some templates exist</returns>
        public static bool IsProductTemplateExist(IDataSource source,Int64 loginId) {
            bool isTemplateExist=false;
            int nbreTemplate=0;

            #region Tables initilization
            Table templateAssignmentTable,productOrderTemplateTable;
            try {
                templateAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightTemplateAssignment);
                productOrderTemplateTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightProductOrderTemplate);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names",err));
            } 
            #endregion

            #region request
            string sql=" select "+templateAssignmentTable.Prefix+".id_template ";
            sql+=" from "+templateAssignmentTable.SqlWithPrefix+",";
            sql+=" "+productOrderTemplateTable.SqlWithPrefix+" ";
            sql+=" where id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"  ";
            sql+=" and id_login="+loginId+" ";
            sql+=" and "+templateAssignmentTable.Prefix+".activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+productOrderTemplateTable.Prefix+".activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+templateAssignmentTable.Prefix+".id_template="+productOrderTemplateTable.Prefix+".id_template ";
            #endregion

            #region Execute request
            try {
                DataSet ds=source.Fill(sql);
                if(ds!=null && ds.Tables!=null && ds.Tables.Count>0 && ds.Tables[0]!=null && ds.Tables[0].Rows!=null &&ds.Tables[0].Rows.Count>0) {
                    nbreTemplate=Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                }

            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive the number of product templates",err));
            }
            
            #endregion

            if(nbreTemplate>0)isTemplateExist=true;
            return isTemplateExist;
        }

        /// <summary>
        /// Check if some media tempates exist
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>True if some templates exist</returns>
        public static bool IsMediaTemplateExist(IDataSource source,Int64 loginId) {
            bool isTemplateExist=false;
            int nbreTemplate=0;

            #region Tables initilization
            Table templateAssignmentTable,mediaOrderTemplateTable;
            try {
                templateAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightTemplateAssignment);
                mediaOrderTemplateTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightMediaOrderTemplate);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names",err));
            } 
            #endregion

            #region Request
            string sql=" select "+templateAssignmentTable.Prefix+".id_template ";
            sql+=" from "+templateAssignmentTable.SqlWithPrefix+",";
            sql+=" "+mediaOrderTemplateTable.SqlWithPrefix+" ";
            sql+=" where id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"  ";
            sql+=" and id_login="+loginId+" ";
            sql+=" and "+templateAssignmentTable.Prefix+".activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+mediaOrderTemplateTable.Prefix+".activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+templateAssignmentTable.Prefix+".id_template="+mediaOrderTemplateTable.Prefix+".id_template ";
            #endregion

            #region Execute request
            try {
                DataSet ds=source.Fill(sql);
                if(ds!=null && ds.Tables!=null && ds.Tables.Count>0 && ds.Tables[0]!=null && ds.Tables[0].Rows!=null &&ds.Tables[0].Rows.Count>0) {
                    nbreTemplate=Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                }

            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive the number of product templates",err));
            }
            #endregion

            if(nbreTemplate>0) isTemplateExist=true;
            return isTemplateExist;

        }
        #endregion

        #region Get Rights

        #region Customer Rights
        /// <summary>
        /// Get product rights
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>SQL code</returns>
        public static DataSet GetProductRights(IDataSource source,Int64 loginId) {

            #region Tables initilization
            Table orderClientProductTable,typeProductTable;
            Schema mau;
            try {
                orderClientProductTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightProductOrder);
                typeProductTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightProductType);
                mau=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mau01);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label",err));
            } 
            #endregion

            #region Request
            string sql="select "+mau.Label+".listnum_to_char(list_product)list,exception,"+typeProductTable.Prefix+".id_type_product";
            sql+=" from "+orderClientProductTable.SqlWithPrefix+","+typeProductTable.SqlWithPrefix+" ";
            sql+=" where "+orderClientProductTable.Prefix+".id_type_product="+typeProductTable.Prefix+".id_type_product";
            sql+=" and id_login="+loginId+"";
            sql+=" and id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";
            sql+=" and "+orderClientProductTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
            sql+=" and "+typeProductTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
            #endregion

            #region Execute request
            try {
                return(source.Fill(sql));
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive rights",err));
            }
            #endregion
        }

        /// <summary>
        /// Get media rights
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>SQL code</returns>
        public static DataSet GetMediaRights(IDataSource source,Int64 loginId) {

            #region Tables initilization
            Table orderClientMediaTable,typeMediaTable;
            Schema mau;
            try {
                orderClientMediaTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightMediaOrder);
                typeMediaTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightMediaType);
                mau=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mau01);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label",err));
            } 
            #endregion

            #region Request
            string sql="select "+mau.Label+".listnum_to_char(list_media)list,exception,"+typeMediaTable.Prefix+".id_type_media";
            sql+=" from "+orderClientMediaTable.SqlWithPrefix+", "+typeMediaTable.SqlWithPrefix+"";
            sql+=" where "+orderClientMediaTable.Prefix+".id_type_media="+typeMediaTable.Prefix+".id_type_media";
            sql+=" and id_login="+loginId+"";
            sql+=" and id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";
            sql+=" and "+orderClientMediaTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
            sql+=" and "+typeMediaTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
            #endregion

            #region Execute request
            try {
                return (source.Fill(sql));
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive rights",err));
            }
            #endregion
        }
        #endregion

        #region Template

        /// <summary>
        /// Get product template
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>template</returns>
        public static DataSet GetProductTemplate(IDataSource source,Int64 loginId) {

            #region Tables initilization
            Table orderTemplateProductTable,typeProductTable,templateTable,templateAssignmentTable;
            Schema mau;
            try {
                orderTemplateProductTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightProductOrderTemplate);
                typeProductTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightProductType);
                templateTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightTemplate);
                templateAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightTemplateAssignment);
                mau=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mau01);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label",err));
            } 
            #endregion

            #region Request
            string sql="select "+mau.Label+".listnum_to_char(list_product)list,exception,"+typeProductTable.Prefix+".id_type_product";
            sql+=" from "+orderTemplateProductTable.SqlWithPrefix+","+typeProductTable.SqlWithPrefix+","+templateTable.SqlWithPrefix+",";
            sql+=" "+templateAssignmentTable.SqlWithPrefix+" ";
            sql+=" where "+orderTemplateProductTable.Prefix+".id_type_product="+typeProductTable.Prefix+".id_type_product";
            sql+=" and "+orderTemplateProductTable.Prefix+".id_template="+templateTable.Prefix+".id_template";
            sql+=" and "+templateTable.Prefix+".id_template="+templateAssignmentTable.Prefix+".id_template ";
            sql+=" and id_login="+loginId+" ";
            sql+= "and "+templateAssignmentTable.Prefix+".id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+" ";
            sql+=" and "+orderTemplateProductTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+typeProductTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+templateTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+templateAssignmentTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            #endregion

            #region Execute request
            try {
                return (source.Fill(sql));
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive rights",err));
            }
            #endregion

        }

        /// <summary>
        /// Get media template
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>template</returns>
        public static DataSet GetMediaTemplate(IDataSource source,Int64 loginId) {

            #region Tables initilization
            Table orderTemplateMediaTable,typeMediaTable,templateTable,templateAssignmentTable;
            Schema mau;
            try {
                orderTemplateMediaTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightMediaOrderTemplate);
                typeMediaTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightMediaType);
                templateTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightTemplate);
                templateAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightTemplateAssignment);
                mau=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mau01);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label",err));
            } 
            #endregion

            #region Request
            string sql="select "+mau.Label+".listnum_to_char(list_media)list,exception,"+typeMediaTable.Prefix+".id_type_media";
            sql+=" from "+orderTemplateMediaTable.SqlWithPrefix+","+typeMediaTable.SqlWithPrefix+","+templateTable.SqlWithPrefix+",";
            sql+=" "+templateAssignmentTable.SqlWithPrefix+"";
            sql+=" where "+orderTemplateMediaTable.Prefix+".id_type_media="+typeMediaTable.Prefix+".id_type_media";
            sql+=" and "+orderTemplateMediaTable.Prefix+".id_template="+templateTable.Prefix+".id_template";
            sql+=" and "+templateTable.Prefix+".id_template="+templateAssignmentTable.Prefix+".id_template ";
            sql+=" and id_login="+loginId+" ";
            sql+= "and "+templateAssignmentTable.Prefix+".id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+" ";
            sql+=" and "+orderTemplateMediaTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+typeMediaTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+templateTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            sql+=" and "+templateAssignmentTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
            
            #endregion

            #region Execute request
            try {
                return (source.Fill(sql));
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive rights",err));
            }
            #endregion

        }
        #endregion

        #region Product class analysis
        /// <summary>
        /// Get Product class analysis right
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <param name="rights">Customer Rights</param>
        /// <returns>template</returns>
        public static DataSet GetProductClassAnalysisRights(IDataSource source,Dictionary<CustomerCst.Right.type,string[]> rights) {

            #region Tables initilization
            Table vehicleTable,categoryTable,mediaTable;
            try {
                vehicleTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapVehicle);
                categoryTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCategory);
                mediaTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMedia);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label",err));
            }
            #endregion

            #region Request
            StringBuilder sql=new StringBuilder(2000);
            bool premier=true;
            //Requête SQL
            sql.Append("Select distinct "+vehicleTable.Prefix+".id_vehicle ");
            sql.Append(" from "+vehicleTable.SqlWithPrefix+","+categoryTable.SqlWithPrefix+","+mediaTable.SqlWithPrefix+" ");
            sql.Append(" where");
            // Langue
            sql.Append(" "+vehicleTable.Prefix+".id_language="+WebApplicationParameters.DefaultDataLanguage.ToString());
            sql.Append(" and " + categoryTable.Prefix + ".id_language=" + WebApplicationParameters.DefaultDataLanguage.ToString());
            sql.Append(" and " + mediaTable.Prefix + ".id_language=" + WebApplicationParameters.DefaultDataLanguage.ToString());
            // Activation
            sql.Append(" and "+vehicleTable.Prefix+".activation<"+DbCst.ActivationValues.UNACTIVATED);
            sql.Append(" and "+categoryTable.Prefix+".activation<"+DbCst.ActivationValues.UNACTIVATED);
            sql.Append(" and "+mediaTable.Prefix+".activation<"+DbCst.ActivationValues.UNACTIVATED);

            // Jointure
            sql.Append(" and "+vehicleTable.Prefix+".id_vehicle="+categoryTable.Prefix+".id_vehicle");
            sql.Append(" and "+categoryTable.Prefix+".id_category="+mediaTable.Prefix+".id_category");


            // Ordre

            premier=true;
            bool beginByAnd=true;
            // le bloc doit il commencer par AND
            // Vehicle
            if(rights.ContainsKey(CustomerCst.Right.type.vehicleAccess) && rights[CustomerCst.Right.type.vehicleAccess].Length>0) {
                if(beginByAnd) sql.Append(" and");
                sql.Append(" ((" + GetInClauseMagicMethod(vehicleTable.Prefix + ".id_vehicle", ConvertIdTableToString(rights[CustomerCst.Right.type.vehicleAccess]), true) + " ");
                premier=false;
            }
            // Category
            if(rights.ContainsKey(CustomerCst.Right.type.categoryAccess) && rights[CustomerCst.Right.type.categoryAccess].Length>0) {
                if(!premier) sql.Append(" or");
                else {
                    if(beginByAnd) sql.Append(" and");
                    sql.Append(" ((");
                }
                sql.Append(" " + GetInClauseMagicMethod(categoryTable.Prefix + ".id_category", ConvertIdTableToString(rights[CustomerCst.Right.type.categoryAccess]), true) + " ");
                premier=false;
            }
            // Media
            if(rights.ContainsKey(CustomerCst.Right.type.mediaAccess) && rights[CustomerCst.Right.type.mediaAccess].Length>0) {
                if(!premier) sql.Append(" or");
                else {
                    if(beginByAnd) sql.Append(" and");
                    sql.Append(" ((");
                }
                sql.Append(" " + GetInClauseMagicMethod(mediaTable.Prefix + ".id_media", ConvertIdTableToString(rights[CustomerCst.Right.type.mediaAccess]), true) + " ");
                premier=false;
            }
            if(!premier) sql.Append(" )");

            // Droits en exclusion
            // Vehicle
            if(rights.ContainsKey(CustomerCst.Right.type.vehicleException) && rights[CustomerCst.Right.type.vehicleException].Length>0) {
                if(!premier) sql.Append(" and");
                else {
                    if(beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                sql.Append(" " + GetInClauseMagicMethod(vehicleTable.Prefix + ".id_vehicle", ConvertIdTableToString(rights[CustomerCst.Right.type.vehicleException]), false) + " ");
                premier=false;
            }
            // Category
            if(rights.ContainsKey(CustomerCst.Right.type.categoryException) && rights[CustomerCst.Right.type.categoryException].Length>0) {
                if(!premier) sql.Append(" and");
                else {
                    if(beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                sql.Append(" " + GetInClauseMagicMethod(categoryTable.Prefix + ".id_category", ConvertIdTableToString(rights[CustomerCst.Right.type.categoryException]), false) + " ");
                premier=false;
            }
            // Media
            if(rights.ContainsKey(CustomerCst.Right.type.mediaException) && rights[CustomerCst.Right.type.mediaException].Length>0) {
                if(!premier) sql.Append(" and");
                else {
                    if(beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                sql.Append(" " + GetInClauseMagicMethod(mediaTable.Prefix + ".id_media", ConvertIdTableToString(rights[CustomerCst.Right.type.mediaException]), false) + " ");
                premier=false;
            }
            if(!premier) sql.Append(" )");

            #endregion

            #region Execute request
            try {
                return (source.Fill(sql.ToString()));
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive rights",err));
            }
            #endregion
        }

        #endregion

        #region Module Frequency
        /// <summary>
        /// Get Module frequencies
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns></returns>
        public static DataSet GetModuleFrequencies(IDataSource source, Int64 loginId) {

            #region Tables initilization
            Table moduleAssignmentTable, frequencyTable;
            try {
                moduleAssignmentTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleAssignment);
                frequencyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightFrequency);
            }
            catch (System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label", err));
            }
            #endregion

            #region Request
            StringBuilder sql = new StringBuilder(1000);
            sql.Append(" select id_module, " + frequencyTable.Prefix + ".id_frequency");
            sql.Append(" from  " + frequencyTable.SqlWithPrefix + "," + moduleAssignmentTable.SqlWithPrefix + " ");
            sql.Append(" where ma.id_login=" + loginId + " ");
            sql.Append(" and " + frequencyTable.Prefix + ".ID_FREQUENCY=" + moduleAssignmentTable.Prefix + ".ID_FREQUENCY ");
            sql.Append(" and " + moduleAssignmentTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
            sql.Append(" and " + frequencyTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
            #endregion

            #region Execute request
            try {
                return (source.Fill(sql.ToString()));
            }
            catch (System.Exception err) {
                throw (new RightDALException("Impossible to retreive module frequencies", err));
            }
            #endregion
        }
        #endregion

        #region Module Assignment
        /// <summary>
        /// Get Module frequencies
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns></returns>
        public static DataSet GetModuleAssignment(IDataSource source, Int64 loginId) {

            #region Tables initilization
            Table moduleAssignmentTable, moduleTable, moduleGroupTable, moduleCategoryTable;
            try {
                moduleAssignmentTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleAssignment);
                moduleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModule);
                moduleGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleGroup);
                moduleCategoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleCategory); ;
            }
            catch (System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label", err));
            }
            #endregion

            #region request
            StringBuilder sql = new StringBuilder(1000);
            sql.Append(" select " + moduleAssignmentTable.Prefix + ".id_module," + moduleAssignmentTable.Prefix + ".date_beginning_module," + moduleAssignmentTable.Prefix + ".date_end_module," + moduleAssignmentTable.Prefix + ".id_frequency," + moduleAssignmentTable.Prefix + ".nb_alert ");
            sql.Append(" from " + moduleAssignmentTable.SqlWithPrefix + "," + moduleTable.SqlWithPrefix + "," + moduleGroupTable.SqlWithPrefix + "," + moduleCategoryTable.SqlWithPrefix + " ");
            sql.Append(" where " + moduleAssignmentTable.Prefix + ".id_module=" + moduleTable.Prefix + ".id_module ");
            sql.Append(" and " + moduleAssignmentTable.Prefix + ".id_module not in(" + TNS.AdExpress.Constantes.Web.Module.NOT_USED_ID_LIST + ") ");
            sql.Append(" and " + moduleTable.Prefix + ".id_module_group=" + moduleGroupTable.Prefix + ".id_module_group ");
            sql.Append(" and " + moduleTable.Prefix + ".id_module_category = " + moduleCategoryTable.Prefix + ".id_module_category(+) ");
            sql.Append(" and " + moduleAssignmentTable.Prefix + ".id_login=" + loginId + " ");
            sql.Append(" and " + moduleGroupTable.Prefix + ".id_project=" + TNS.AdExpress.Constantes.Project.ADEXPRESS_ID + " ");
            sql.Append(" and " + moduleAssignmentTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
            sql.Append(" and " + moduleTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
            sql.Append(" and " + moduleGroupTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
            sql.Append(" order by " + moduleGroupTable.Prefix + ".module_group, " + moduleCategoryTable.Prefix + ".module_category, " + moduleTable.Prefix + ".module");
            #endregion

            #region Execute request
            try {
                return (source.Fill(sql.ToString()));
            }
            catch (System.Exception err) {
                throw (new RightDALException("Impossible to retreive module rights", err));
            }
            #endregion
        }
        #endregion

        #region Get Banners Format Assignement
        /// <summary>
        /// Get Module frequencies
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns></returns>
        public static DataSet GetBannersFormatAssignement(IDataSource source, Int64 loginId) {

            #region Tables initilization
            Table rightGroupFormat;
            Schema mau;
            try {
                rightGroupFormat = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightGroupFormat);
                mau = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mau01);
            }
            catch (System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label", err));
            }
            #endregion

            #region Request
            string sql = "select " + mau.Label + ".listnum_to_char(list_group_format)list,exception";
            sql += " from " + rightGroupFormat.SqlWithPrefix + " ";
            sql += " where id_login=" + loginId + "";
            sql += " and id_project=" + Constantes.Project.ADEXPRESS_ID + "";
            sql += " and " + rightGroupFormat.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
            #endregion

            #region Execute request
            try {
                return (source.Fill(sql));
            }
            catch (System.Exception err) {
                throw (new RightDALException("Impossible to retreive Banners Format Assignement rights : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Right Cohesion

        #region Media Right cohesion
        #endregion

        #region Product Right cohesion
        #endregion

        #endregion

        #region Modules
        /// <summary>
        /// Get Customer Modules rights
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>Accessible modules</returns>
        public static DataSet GetModulesRights(IDataSource source,Int64 loginId) {
           
            #region Tables initilization
            Table moduleAssignmentTable,moduleTable,moduleGroupTable,moduleCategoryTable;
            try {
                moduleAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleAssignment);
                moduleTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModule);
                moduleGroupTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleGroup);
                moduleCategoryTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleCategory); ;
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label",err));
            }
            #endregion

            #region request
            StringBuilder sql=new StringBuilder(1000);
            sql.Append(" select " + moduleTable.Prefix + ".id_module_group," + moduleTable.Prefix + ".id_module," + moduleCategoryTable.Prefix + ".id_module_category ");
            sql.Append(" from "+moduleAssignmentTable.SqlWithPrefix+","+moduleTable.SqlWithPrefix+","+moduleGroupTable.SqlWithPrefix+","+moduleCategoryTable.SqlWithPrefix+" ");
            sql.Append(" where "+moduleAssignmentTable.Prefix+".id_module="+moduleTable.Prefix+".id_module ");
            sql.Append(" and "+moduleAssignmentTable.Prefix+".id_module not in("+TNS.AdExpress.Constantes.Web.Module.NOT_USED_ID_LIST+") ");
            sql.Append(" and "+moduleTable.Prefix+".id_module_group="+moduleGroupTable.Prefix+".id_module_group ");
            sql.Append(" and "+moduleTable.Prefix+".id_module_category = "+moduleCategoryTable.Prefix+".id_module_category(+) ");
            sql.Append(" and "+moduleAssignmentTable.Prefix+".id_login="+loginId+" ");
            sql.Append(" and "+moduleGroupTable.Prefix+".id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+" ");
            sql.Append(" and "+moduleAssignmentTable.Prefix+".date_beginning_module<=sysdate ");
            sql.Append(" and "+moduleAssignmentTable.Prefix+".date_end_module>=sysdate ");
            sql.Append(" and "+moduleAssignmentTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");
            sql.Append(" and "+moduleTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");
            sql.Append(" and "+moduleGroupTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");
            sql.Append(" order by "+moduleGroupTable.Prefix+".module_group, "+moduleCategoryTable.Prefix+".module_category, "+moduleTable.Prefix+".module");
            #endregion

            #region Execute request
            try {
                return (source.Fill(sql.ToString()));
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive module rights",err));
            }
            #endregion

        }
        #endregion

        #region Flags
        /// <summary>
        /// Get Customer Flags rights
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="loginId">Login Id</param>
        /// <returns>Accessible flags</returns>
        public static DataSet GetFlagsRights(IDataSource source,Int64 loginId) {

            #region Tables initilization
            Table flagTable,projectFlagAssignmentTable;
            try {
                flagTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightFlag);
                projectFlagAssignmentTable=WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightProjectFlagAssignment);
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to get table names or schema label",err));
            }
            #endregion

            #region request
            StringBuilder sql=new StringBuilder(1000);
            sql.Append("select "+flagTable.Prefix+".id_flag, flag ");
            sql.Append(" from "+projectFlagAssignmentTable.SqlWithPrefix+",");
            sql.Append(" "+flagTable.SqlWithPrefix+" ");
            sql.Append(" where "+projectFlagAssignmentTable.Prefix+".id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+" ");
            sql.Append(" and "+projectFlagAssignmentTable.Prefix+".id_login="+loginId+" ");
            sql.Append(" and "+projectFlagAssignmentTable.Prefix+".ID_FLAG="+flagTable.Prefix+".ID_FLAG ");
            sql.Append(" and "+projectFlagAssignmentTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");
            sql.Append(" and "+flagTable.Prefix+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");
            #endregion

            #region Execute request
            try {
                return (source.Fill(sql.ToString()));
            }
            catch(System.Exception err) {
                throw (new RightDALException("Impossible to retreive flags rights",err));
            }
            #endregion

        }
        #endregion

        #region Evaliant Country Rights
        /// <summary>
        /// Get Evaliant Country Rights
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <param name="rights">Customer Rights</param>
        /// <returns>SQL code</returns>
        public static string GetEvaliantCountryRights(IDataSource source, string accessRights, string exceptionRights, int dataLanguage ) {

            #region Tables initilization
            View mediaView;
            try {
                mediaView = WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia);
            }
            catch (System.Exception err) {
                throw (new RightDALException("Impossible to init view object for Evaliant Country Rights", err));
            }
            #endregion

            #region Request
            bool first = true;
            string countryList = string.Empty;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select distinct id_country");
            sql.Append(" from " + mediaView.Sql + dataLanguage.ToString() + " ");
            sql.Append(" where id_vehicle = " + CstClassif.DB.Vehicles.names.adnettrack.GetHashCode());
            // Media
            if (accessRights.Length > 0) {
                sql.Append(" and " + GetInClauseMagicMethod("id_media", accessRights, true) + " ");
                first = false;
            }
            // Media
            if (exceptionRights.Length > 0) {
                if (!first) sql.Append(" and ");
                sql.Append(" " + GetInClauseMagicMethod("id_media", exceptionRights, false) + " ");
            }
            #endregion

            #region Execute request
            try {
                DataSet ds=source.Fill(sql.ToString());

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0) {
                    foreach (DataRow currentRow in ds.Tables[0].Rows)
                        countryList += currentRow[0].ToString() + ",";

                    if (countryList.Length > 0)
                        countryList = countryList.Substring(0, countryList.Length - 1);
                }
                return (countryList);
            }
            catch (System.Exception err) {
                throw (new RightDALException("Impossible to retreive rights", err));
            }
            #endregion
        }
        #endregion

        #region Privacy Settings

        public static void GetPrivacySettings(IDataSource source, Int64 loginId, out bool enableTracking, out bool enableTroubleshooting, out DateTime dateExpCookie)
        {
            #region Tables initilization
            Table login;
            Schema mau;
            try
            {
                login = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
                mau = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mau01);
            }
            catch (System.Exception err)
            {
                throw (new RightDALException("Impossible to get table names or schema label", err));
            }
            #endregion

            #region Request
            string sql = "select id_login, login, enable_tracking, enable_troubleshooting, date_exp_cookie";
            sql += " from " + login.SqlWithPrefix + " ";
            sql += " where id_login=" + loginId + "";
            sql += " and activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
            #endregion

            #region Execute request
            try
            {
                DataSet ds = source.Fill(sql);

                enableTracking = false;
                enableTroubleshooting = false;
                dateExpCookie = new DateTime(2000, 1, 1);

                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows != null)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    enableTracking = Convert.ToInt32(row[2]) != 0;
                    enableTroubleshooting = Convert.ToInt32(row[3]) != 0;
                    dateExpCookie = DateTime.Parse(row[4].ToString());
                }
            }
            catch (System.Exception err)
            {
                throw (new RightDALException("Impossible to retreive privacy settings : " + sql, err));
            }
            #endregion
        }

        public static void SetAllPrivacySettings(IDataSource source, Int64 loginId, int enableTracking, int enableTroubleshooting, DateTime dateExpCookie)
        {
            #region Tables initilization
            Table login;
            try
            {
                login = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
            }
            catch (System.Exception err)
            {
                throw (new RightDALException("Impossible to get table names or schema label", err));
            }
            #endregion

            #region Request
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("update {0} ", login.SqlWithPrefix);
            sql.AppendFormat("set ENABLE_TRACKING = {0} ", enableTracking);
            sql.AppendFormat(", ENABLE_TROUBLESHOOTING = {0} ", enableTroubleshooting);
            sql.AppendFormat(", DATE_EXP_COOKIE = TO_DATE('{0}', 'DD/MM/YYYY')", dateExpCookie.ToString("dd/MM/yyyy"));
            sql.AppendFormat(" where id_login = {0} ", loginId);
            #endregion

            #region Execute request
            try
            {
                source.Update(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new RightDALException("Impossible to retreive privacy settings : " + sql, err));
            }
            #endregion
        }

        public static void SetPrivacySettings(IDataSource source, Int64 loginId, int enableTracking, int enableTroubleshooting)
        {
            #region Tables initilization
            Table login;
            try
            {
                login = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
            }
            catch (System.Exception err)
            {
                throw (new RightDALException("Impossible to get table names or schema label", err));
            }
            #endregion

            #region Request
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("update {0} ", login.SqlWithPrefix);
            sql.AppendFormat("set ENABLE_TRACKING = {0} ", enableTracking);
            sql.AppendFormat(", ENABLE_TROUBLESHOOTING = {0} ", enableTroubleshooting);
            sql.AppendFormat(" where id_login = {0} ", loginId);
            #endregion

            #region Execute request
            try
            {
                source.Update(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new RightDALException("Impossible to retreive privacy settings : " + sql, err));
            }
            #endregion
        }

        #endregion

        #endregion

        #region Private Medthods
        /// <summary>
        /// Convert string table to string
        /// </summary>
        /// <remarks>id,id,id</remarks>
        /// <param name="tab"></param>
        /// <returns>string</returns>
        private static string ConvertIdTableToString(string[] tab) {
            string list="";
            foreach(string id in tab) {
                list+=id+",";
            }
            if(list.Length>0) list=list.Substring(0,list.Length-1);
            return (list);
        }
        /// <summary>
        /// In Clause Method
        /// </summary>
        /// <param name="label">Field</param>
        /// <param name="inClauseItems">Items
        /// <example>"3,9,6"</example>
        /// </param>
        /// <param name="include">True if elements are included (in), false either (not in)</param>
        /// <returns>In clause SQL code</returns>
        public static string GetInClauseMagicMethod(string label, string inClauseItems, bool include) {

            string str = string.Empty;
            if (inClauseItems.Length > 0) {
                StringBuilder sb = new StringBuilder();
                string[] strs = inClauseItems.Split(',');
                int i = 0;
                sb.Append("(");
                while (i < strs.Length) {
                    if (i > 0) {
                        sb.Append((include) ? " or " : " and ");
                    }
                    sb.AppendFormat(" {1} {2} ({0}) ", String.Join(",", strs, i, Math.Min(strs.Length - i, 500)), label, (include) ? " in " : " not in ");
                    i += 500;
                }
                sb.Append(")");
                return sb.ToString();
            }

            return str;
        }
        #endregion


    }
        
}
