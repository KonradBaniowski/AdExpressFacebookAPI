#region Information
/*
 * Author : Y Rkaina && D. Mussuma
 * Created on : 16/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using VehicleClassificationCst = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;


namespace TNS.AdExpressI.Classification.DAL {
    /// <summary>
    /// provides the queries to obtain the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
    /// <example>
    /// If the user wants to select vehicles displayed by Sub media\Vehicle, the clause select of the query will be
    /// : select distinct id_category as idDetailMedia, category as detailMedia, id_media, media from ...
    /// </example>
    /// Use the methods <code>GetData();</code> or <code>GetData(string keyWord);</code>
    /// </summary>	   
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
    /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
	public abstract class DetailDAL : EngineDAL
    {
        #region Variables
        /// <summary>
        /// Description of media detail levels selected by the customer
        /// </summary>
        protected GenericDetailLevel _genericDetailLevel = null;
        /// <summary>
        /// List of identifiers of vehicles
        /// </summary>
		protected string _listMedia = "";
        /// <summary>
        /// Key word for searching
        /// </summary>
		protected string _keyWord = "";
        #endregion

        #region Constructor(s)
        /// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="genericDetailLevel">generic detail level selected by the user</param>
		/// <param name="listMedia">List of media selected by the user</param>
        public DetailDAL(WebSession session, GenericDetailLevel genericDetailLevel)
			: base(session) {
			_genericDetailLevel = genericDetailLevel;
			//Gets media type level’s table description(table label, prefix ...)
			vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);

			//Gets sub media level’s table description(table label, prefix)
			categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category);

			//Gets basic media level’s table description(table label, prefix)
			basicMediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia);

			//Gets vehicle level’s table description(table label, prefix)
			mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media);

		}
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="genericDetailLevel">generic detail level selected by the user</param>
		/// <param name="listMedia">List of media selected by the user</param>
		public DetailDAL(WebSession session, GenericDetailLevel genericDetailLevel, string listMedia)
			: base(session) {
			_genericDetailLevel = genericDetailLevel;
			_listMedia = listMedia;
			//Gets media type level’s table description(table label, prefix ...)
			vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);

			//Gets sub media level’s table description(table label, prefix)
			categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category);

			//Gets basic media level’s table description(table label, prefix)
			basicMediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia);

			//Gets vehicle level’s table description(table label, prefix)
			mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media);
			
		}
		
		#endregion

		#region Methods

        /// <summary>
        /// Get the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
        /// According to the vehicle classification levels choosen by the customer.
        /// <example>
        /// If the user wants to select vehicles displayed by Sub media\Vehicle, the clause select of the query will be
        /// : select distinct id_category as idDetailMedia, category as detailMedia, id_media, media from ...
        /// </example>
        /// </summary>	
        /// <returns>Dataset with  a data set with table of 4 columns : "idDetailMedia", "detailMedia", "id_media", "media".
        /// The column "idDetailMedia" corresponds to the identifier of the level parent.
        /// The column "detailMedia" corresponds to the label of the level parent.
        /// The column "id_media" corresponds to the identifier of the vehicle.
        /// The column "media" corresponds to the label of the vehicle.
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
		public virtual DataSet GetData() {

			#region Variables
			//bool premier = true;
			DataSet dsListAdvertiser = null;
			var sql = new StringBuilder(500);

            #endregion

			#region Building SQL Query
			try {
                /*Get code SQL of the fields corresponding to the level items of the current media detail level */                 
				string fields = _genericDetailLevel.GetSqlFieldsWithoutTablePrefix();

                /*Get code SQL of the clause ORDER corresponding to the level items of current detail level*/
				string orderFields = _genericDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();

                /*Defines the clause SELECT with the fields previously obtained. 
                 * The field "activation" determines if a data row is always valid.*/
                sql.AppendFormat("Select distinct {0}", fields);
                if (_session.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.VP)
                    sql.Append(", activation");

                /*Get the view with all media-classification items. Example, for France the view contains the fields
                *"id_vehicle,vehicle,id_basic_media,basic_media,id_media,media,...". So for each level 
                 *of the classification the view will contain identifier and label. 
                 It allows to research any items of the classification*/
                sql.AppendFormat(" from {0}", GetView());
				
                /*SQL conditions */
                sql.Append(" where 0=0");

                /*Get the identifier of the selected current media type. 
                 * Example if the user as selected the media PRESS,
                the joins could be like this : id_vehicle = 3" */
			    FilterWithSelectedMediaType(sql);

                /*Filter data with the identifier of the sub media selected.
                 Remark : Use in Russia
                 */
			    FilterWithSubMedia(sql);

                //This section is specifical to the media Internet. obtains the list of active vehicle for Internet. (Only for France)
			    GetInternetActiveVehicles(sql);

				//Restriction on the vehicles selected by the customer
                if (_session.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.VP && !string.IsNullOrEmpty(_listMedia)) {
                    sql.AppendFormat(" and id_media in ({0}) ", _listMedia);
				}

                //Restriction on the media universe allowed for the customer and module of TV sponsorship
			    FilterWithTvSponsorshipUniverse(sql);

                //Obtains the media rights of the customer
                sql.Append(GetCustomerRights());
				
                //Order by current levels
				sql.AppendFormat(" order by {0}" , orderFields);

				
			}
			catch (System.Exception err) {
				throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail ", err));
			}
			#endregion

			#region Execution of the query
			try {
                //Execution of the query
				dsListAdvertiser = _session.Source.Fill(sql.ToString());
				dsListAdvertiser.Tables[0].TableName = "dsListAdvertiser";
			}
			catch (System.Exception err) {
                throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail. sql:" + sql.ToString(), err));
			}
			#endregion

			return dsListAdvertiser;
		}

		/// <summary>
		/// Get the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
        /// According to the vehicle classification levels choosen by the customer.
		/// <example>
        /// If the user wants to select vehicles displayed by Sub media\Vehicle, the clause select of the query will be
        /// : select distinct id_category as idDetailMedia, category as detailMedia, id_media, media from ...
        /// </example>
		/// </summary>	
        /// <param name="keyWord">key word to search corresponding vehicles</param>
		/// <returns>Dataset with  a data table of 4 columns : "idDetailMedia", "detailMedia", "id_media", "media".
        /// The column "idDetailMedia" corresponds to the identifier of the level parent.
        /// The column "detailMedia" corresponds to the label of the level parent.
        /// The column "id_media" corresponds to the identifier of the vehicle.
        /// The column "media" corresponds to the label of the vehicle.
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
		public virtual DataSet GetData(string keyWord) {
			#region Variables
			//bool premier = true;
			DataSet dsListAdvertiser = null;
			StringBuilder sql = new StringBuilder(500);
			string activeMediaList = string.Empty;
            //Get data filters object which contains query's filters methods
            TNS.AdExpress.Web.Core.CustomerDataFilters dataFilters = new TNS.AdExpress.Web.Core.CustomerDataFilters(_session);            
			#endregion

			#region Requête
			try {
				string fields = _genericDetailLevel.GetSqlFieldsWithoutTablePrefix();
				string orderFields = _genericDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();


                /*Defines the clause SELECT with the fields previously obtained. 
                 * The field "activation" determines if a data row is always valid.*/
                sql.AppendFormat("Select distinct {0}, activation", fields);

                /*Get the view with all media-classification items. Example, for France the view contains the fields
              *"id_vehicle,vehicle,id_basic_media,basic_media,id_media,media,...". So for each level 
               *of the classification the view will contain identifier and label. 
               It allows to research any items of the classification*/
                sql.AppendFormat(" from {0}", GetView());

                /*SQL conditions */
				sql.Append(" where");

                /*Get the identifier of the selected current media type. 
                * Example if the user as selected the media PRESS,
               the joins could be like this : id_vehicle = 3" */
                sql.AppendFormat(" id_vehicle={0}", _session.CustomerDataFilters.SelectedMediaType);

                /*Filter data with the identifier of the sub media selected.
                 Remark : Used in Russia
                 */
                string idSubMedia = _session.CustomerDataFilters.SelectedMediaCategory;
                if(!string.IsNullOrEmpty(idSubMedia))
                sql.AppendFormat(" and id_category ={0}", idSubMedia);

                //This section is specifical to the media Internet. obtains the list of active vehicle for Internet.
				GetInternetActiveVehicles(sql);

			    //Search vehicles by key word 
				int j = 0;
				foreach (DetailLevelItemInformation currentLevel in _genericDetailLevel.Levels) {
					if (j > 0) sql.Append(" or ");
					else sql.Append("  and (");
					sql.Append("  " + currentLevel.DataBaseField + " like UPPER('%" + keyWord + "%') ");
					j++;
				}
                //Restriction on the vehicles selected by the customer
				if (_listMedia!=null && _listMedia.Length > 0) {
					sql.Append(" or id_media in (" + _listMedia + ") ");
				}
				sql.Append(" ) ");
				


				//Condition universe media in access
				if (_module != null && _module.ModuleType == WebConstantes.Module.Type.tvSponsorship)
					sql.Append(GetAllowedMediaUniverseSql(true));

                //Obtains the media rights of the customer
                sql.Append(GetMediaRights("",true));

                //Order by current levels
                sql.AppendFormat(" order by {0}", orderFields);	
			}
			catch (System.Exception err) {
				throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail ", err));
			}
			#endregion

			#region Execution of the SQL query
			try {
                // Execution of the SQL query
				dsListAdvertiser = _session.Source.Fill(sql.ToString());
				dsListAdvertiser.Tables[0].TableName = "dsListAdvertiser";
			}
			catch (System.Exception err) {
				throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail", err));
			}
			#endregion

			return dsListAdvertiser;
		}

        /// <summary>
        /// Get the list of sub media corresponding to media type selected
        /// </summary>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
        /// <returns>Dataset with  a data table of 2 columns : "Id_SubMedia", "SubMedia".
        /// The column "Id_SubMedia" corresponds to the identifier of the level sub media.
        /// The column "SubMedia" corresponds to the label of the level sub media.       
        /// </returns>
        public virtual DataSet GetSubMediaData()
        {
            #region Variables
            //bool premier = true;
            DataSet dsListSubMedia = null;
            StringBuilder sql = new StringBuilder(500);          
            
            string subMediaId = "";
            #endregion

            #region Building SQL Query
            try
            {
             

                /*Defines the clause SELECT with the fields for identifier and label of sub media level.*/
                sql.Append("Select distinct id_category as Id_SubMedia, category as SubMedia");

                /*Get the view with all media-classification items. Example, for France the view contains the fields
                *"id_vehicle,vehicle,id_basic_media,basic_media,id_media,media,...". So for each level 
                 *of the classification the view will contain identifier and label. 
                 It allows to research any items of the classification*/
                sql.AppendFormat(" from {0}", GetView());

                /*SQL conditions */
                sql.Append(" where");

                /*Get the identifier of the selected current media type. 
                 * Example if the user as selected the media RADIO,
                the joins could be like this : id_vehicle = 300" */
                sql.AppendFormat(" id_vehicle={0}", _session.CustomerDataFilters.SelectedMediaType);             
              
                //Obtains the media rights of the customer
                sql.Append(GetMediaRights("", true));

                //Order by current levels
                sql.AppendFormat(" order by category,id_category");


            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail ", err));
            }
            #endregion

            #region Execution of the query
            try
            {
                //Execution of the query
                dsListSubMedia = _session.Source.Fill(sql.ToString());
               
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail", err));
            }
            #endregion

            return dsListSubMedia;
        }
		#endregion

        /// <summary>
        /// Get media universe sql conditions
        /// </summary>       
        /// <param name="startWithAnd">Determine if sql condition start with "and"</param>
        /// <returns>Sql conditions</returns>
        protected string GetAllowedMediaUniverseSql(bool startWithAnd)
        {
           
            string sql = "", temp = "";
            bool first = true;

            MediaItemsList mediaList = _session.CustomerDataFilters.AllowedMediaUniverse;

            if (mediaList != null)
            {
                //Get media type  list
                temp = mediaList.GetVehicleListSQL(false, string.Empty);
                if (temp.Length > 0)
                {
                    if (startWithAnd) sql = " and ";
                    sql += " ( " + temp;
                    first = false;
                }
                //Get sub media  list
                temp = mediaList.GetCategoryListSQL(false, string.Empty);
                if (temp.Length > 0)
                {
                    if (!first) sql += " or";
                    else
                    {
                        if (startWithAnd) sql += " and";
                        sql += " (";
                    }
                    sql += " " + temp;
                    first = false;
                }
                //Get vehicle list
                temp = mediaList.GetMediaListSQL(false, string.Empty);
                if (temp.Length > 0)
                {
                    if (!first) sql += " or";
                    else
                    {
                        if (startWithAnd) sql += " and";
                        sql += " (";
                    }
                    sql += " " + temp;
                    first = false;
                }
                if (!first) sql += " )";
            }
            return sql;

        }
        /// <summary>
        /// Get View
        /// </summary>
        /// <returns>View</returns>
        protected abstract string GetView();

        protected virtual string GetCustomerRights()
        {
            return  GetMediaRights(string.Empty, true);
        }

        protected virtual void GetInternetActiveVehicles(StringBuilder sql)
        {
            if (ContainsVehicle(VehicleClassificationCst.internet) || ContainsVehicle(VehicleClassificationCst.mms))
            {
                string activeMediaList = AdExpress.Web.Core.ActiveMediaList.
                    GetActiveMediaList(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);

                string inClauseSqlCode = AdExpress.Web.Core.Utilities.
                    SQLGenerator.GetInClauseMagicMethod("id_media", activeMediaList);

                if (!string.IsNullOrEmpty(inClauseSqlCode))
                {
                    sql.AppendFormat(" and {0} ", inClauseSqlCode);
                }
            }
        }

        protected virtual bool ContainsVehicle(VehicleClassificationCst vehicleName)
        {
            return _session.CurrentModule != AdExpress.Constantes.Web.Module.Name.VP
                   && VehiclesInformation.Contains(vehicleName)
                   && _session.SelectionUniversMedia != null && _session.SelectionUniversMedia.FirstNode != null &&
                   ((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID
                   == VehiclesInformation.EnumToDatabaseId(vehicleName);
        }

        protected virtual void FilterWithSubMedia(StringBuilder sql)
        {
            string idSubMedia = null;
            if (_session.CurrentModule != AdExpress.Constantes.Web.Module.Name.VP
                && _session.CustomerDataFilters != null)
                idSubMedia = _session.CustomerDataFilters.SelectedMediaCategory;
            if (_session.CurrentModule != AdExpress.Constantes.Web.Module.Name.VP
                && !string.IsNullOrEmpty(idSubMedia))
                sql.AppendFormat(" and id_category ={0}", idSubMedia);
        }

        protected virtual void FilterWithSelectedMediaType(StringBuilder sql)
        {
            if (_session.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.VP
                   && _session.CustomerDataFilters != null &&
                   !string.IsNullOrEmpty(_session.CustomerDataFilters.SelectedMediaType))
                sql.AppendFormat(" and id_vehicle={0}", _session.CustomerDataFilters.SelectedMediaType);
        }

        protected virtual void FilterWithTvSponsorshipUniverse(StringBuilder sql)
        {
            if (_session.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.VP && _module != null
                  && _module.ModuleType == WebConstantes.Module.Type.tvSponsorship)
                sql.Append(_module.GetAllowedMediaUniverseSqlWithOutPrefix(true));
        }
	}
}
