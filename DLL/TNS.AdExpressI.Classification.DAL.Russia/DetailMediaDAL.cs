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
using System.Data.SqlClient;


namespace TNS.AdExpressI.Classification.DAL.Russia
{
    class DetailMediaDAL : TNS.AdExpressI.Classification.DAL.DetailMediaDAL
    {
        #region Constructor(s)
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="genericDetailLevel">generic detail level selected by the user</param>
        /// <param name="listMedia">List of media selected by the user</param>
        public DetailMediaDAL(WebSession session, GenericDetailLevel genericDetailLevel)
            : base(session, genericDetailLevel)
        {

        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="genericDetailLevel">generic detail level selected by the user</param>
        /// <param name="listMedia">List of media selected by the user</param>
        public DetailMediaDAL(WebSession session, GenericDetailLevel genericDetailLevel, string listMedia)
            : base(session, genericDetailLevel, listMedia)
        {

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
        public override DataSet GetData()
        {
            #region Variables
            //bool premier = true;
            DataSet dsListAdvertiser = null;
            StringBuilder sql = new StringBuilder(500);
            string activeMediaList = string.Empty, subMediaId = string.Empty;

            #endregion

            #region Building SQL Query
            try
            {
                /*Get code SQL of the fields corresponding to the level items of the current media detail level */
                string fields = _genericDetailLevel.GetSqlFieldsWithoutTablePrefix();

                /*Get code SQL of the clause ORDER corresponding to the level items of current detail level*/
                string orderFields = _genericDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();

                /*Defines the clause SELECT with the fields previously obtained. 
                 * The field "activation" determines if a data row is always valid.*/
                sql.AppendFormat("Select distinct {0}, activation", fields);

                /*Get the view with all media-classification items. Example, for France the view contains the fields
                *"id_vehicle,vehicle,id_basic_media,basic_media,id_media,media,...". So for each level 
                 *of the classification the view will contain identifier and label. 
                 It allows to research any items of the classification*/
                sql.AppendFormat(" from {0}", WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia).Sql + _session.DataLanguage);

                /*SQL conditions */
                sql.Append(" where");

                /*Get the identifier of the selected current media type. 
                 * Example if the user as selected the media PRESS,
                the joins could be like this : id_vehicle = 3" */
                sql.AppendFormat(" id_vehicle={0}", _session.CustomerDataFilters.SelectedMediaType);

                //Get sub media selected
                subMediaId = _session.CustomerDataFilters.SelectedMediaCategory;
                if (!string.IsNullOrEmpty(subMediaId))
                    sql.AppendFormat(" and id_category={0}", subMediaId);


                //This section is specifical to the media Internet. obtains the list of active vehicle for Internet. (Only for France)
                if (VehiclesInformation.Contains(VehicleClassificationCst.internet) && ((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID == VehiclesInformation.EnumToDatabaseId(VehicleClassificationCst.internet))
                {
                    activeMediaList = TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
                    string inClauseSQLCode = TNS.AdExpress.Web.Core.Utilities.SQLGenerator.GetInClauseMagicMethod("id_media", activeMediaList);
                    if (inClauseSQLCode.Length > 0)
                    {
                        sql.AppendFormat(" and {0} ", inClauseSQLCode);
                    }
                }
                //Restriction on the vehicles selected by the customer
                if (_listMedia != null && _listMedia.Length > 0)
                {
                    sql.AppendFormat(" and id_media in ({0}) ", _listMedia);
                }

                //Restriction on the media universe allowed for the customer and module of TV sponsorship
                if (_module != null && _module.ModuleType == WebConstantes.Module.Type.tvSponsorship)
                    sql.Append(_module.GetAllowedMediaUniverseSqlWithOutPrefix(true));

                //Obtains the media rights of the customer
                sql.Append(GetMediaRights("", true));

                //Order by current levels
                sql.AppendFormat(" order by {0}", orderFields);


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
                dsListAdvertiser = _session.Source.Fill(sql.ToString());
                dsListAdvertiser.Tables[0].TableName = "dsListAdvertiser";
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail", err));
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
        public override DataSet GetData(string keyWord)
        {
            #region Variables
            //bool premier = true;
            DataSet dsListAdvertiser = null;
            StringBuilder sql = new StringBuilder(500);
            string activeMediaList = string.Empty;
            //Get data filters object which contains query's filters methods
            TNS.AdExpress.Web.Core.CustomerDataFilters dataFilters = new TNS.AdExpress.Web.Core.CustomerDataFilters(_session);
            #endregion

            #region Requête
            try
            {
                string fields = _genericDetailLevel.GetSqlFieldsWithoutTablePrefix();
                string orderFields = _genericDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();


                /*Defines the clause SELECT with the fields previously obtained. 
                 * The field "activation" determines if a data row is always valid.*/
                sql.AppendFormat("Select distinct {0}, activation", fields);

                /*Get the view with all media-classification items. Example, for France the view contains the fields
              *"id_vehicle,vehicle,id_basic_media,basic_media,id_media,media,...". So for each level 
               *of the classification the view will contain identifier and label. 
               It allows to research any items of the classification*/
                sql.AppendFormat(" from {0}", WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia).Sql + _session.DataLanguage);

                /*SQL conditions */
                sql.Append(" where");

                /*Get the identifier of the selected current media type. 
                * Example if the user as selected the media PRESS,
               the joins could be like this : id_vehicle = 3" */
                sql.AppendFormat(" id_vehicle={0}", _session.CustomerDataFilters.SelectedMediaType);

                //This section is specifical to the media Internet. obtains the list of active vehicle for Internet.
                if (VehiclesInformation.Contains(VehicleClassificationCst.internet) && ((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID == VehiclesInformation.EnumToDatabaseId(VehicleClassificationCst.internet))
                {
                    activeMediaList = TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
                    string inClauseSQLCode = TNS.AdExpress.Web.Core.Utilities.SQLGenerator.GetInClauseMagicMethod("id_media", activeMediaList);
                    if (inClauseSQLCode.Length > 0)
                    {
                        sql.AppendFormat(" and {0} ", inClauseSQLCode);
                    }
                }

                //Search vehicles by key word 
                int j = 0;
                foreach (DetailLevelItemInformation currentLevel in _genericDetailLevel.Levels)
                {
                    if (j > 0) sql.Append(" or ");
                    else sql.Append("  (");
                    sql.Append("  " + currentLevel.DataBaseField + " like UPPER('%" + keyWord + "%') ");
                    j++;
                }
                //Restriction on the vehicles selected by the customer
                if (_listMedia != null && _listMedia.Length > 0)
                {
                    sql.Append(" or id_media in (" + _listMedia + ") ");
                }
                sql.Append(" ) ");



                //Condition universe media in access
                if (_module != null && _module.ModuleType == WebConstantes.Module.Type.tvSponsorship)
                    sql.Append(GetAllowedMediaUniverseSql(true));

                //Obtains the media rights of the customer
                sql.Append(GetMediaRights("", true));

                //Order by current levels
                sql.AppendFormat(" order by {0}", orderFields);
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail ", err));
            }
            #endregion

            #region Execution of the SQL query
            try
            {
                // Execution of the SQL query
                dsListAdvertiser = _session.Source.Fill(sql.ToString());
                dsListAdvertiser.Tables[0].TableName = "dsListAdvertiser";
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail", err));
            }
            #endregion

            return dsListAdvertiser;
        }

        #endregion


    }
}
