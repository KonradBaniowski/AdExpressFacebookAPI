using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using System.Data;

namespace TNS.AdExpressI.Insertions.DAL
{
    public interface IInsertionsDAL
    {
        /// <summary>
        /// Get Media types identifiers according to levels of classification selected by the customer
        /// <example> If the filters variable contain : (media type id = 1)/(sub media id = 11)
        ///     // get list of media type identifiers (in our classification we use vehicle for media type)
        ///     select distinct id_vehicle 
        ///     // A view that contains all the media classification (id_vehicle(media type), id_media(vehicle), id_interest_center, id_basic_media, id_title ...) 
        ///     from adexpr03.all_media_44
        ///     // The data is filtered by the media type 1
        ///     where id_vehicle = 1
        ///     // The data is filtered by the sub media 11
        ///     and id_category = 11
        /// </example>
        /// </summary>
        /// <param name="filters">Dictionary of [classification levels, value(corresponds to the value of the classification level in the database)]</param>
        /// <returns>Media types identifiers list</returns>
        Int64[] GetVehiclesIds(Dictionary<DetailLevelItemInformation, Int64> filters);

        /// <summary>
        /// Get list of media type for which there're data in the database
        /// </summary>
        /// <param name="vehicles">List of Media type object to check</param>
        /// <param name="filters">List identiifers selected by the customer. Used as data filters in the query ex. "</param>
        /// <param name="fromDate">User Period beginning</param>
        /// <param name="toDate">User Period End</param>
        /// <param name="universId">User Univers Selection</param>
        /// <param name="module">Current Module</param>
        /// <param name="sloganNotNull">True if slogan not null</param>
        /// <returns>List of vehicles present</returns>
        List<VehicleInformation> GetPresentVehicles(List<VehicleInformation> vehicles, string filters, int fromDate, int toDate, int universId, Module module, bool sloganNotNull);

        /// <summary>
        /// Extract advertising detail for insertions details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">User Univers Selection (correspondig to which is the current univers if we have a competitor study)</param>
        /// <param name="filters">Filters Identifiers (A list of values correspondig to the classification levels)</param>
        /// <returns>Advertising detail Data</returns>
        DataSet GetInsertionsData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters);

        /// <summary>
        /// Extract advertising detail for creatives details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">User Univers Selection (correspondig to which is the current univers if we have a competitor study)</param>
        /// <param name="filters">Filters Identifiers (A list of values correspondig to the classification levels)</param>
        /// <returns>Advertising detail Data</returns>	
        DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters);

        /// <summary>
        /// Extract advertising detail for creatives details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="columns">columns</param>
        /// <returns>Advertising detail Data</returns>		
        DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate,
                                 List<GenericColumnItemInformation> columns);

        long CountCreativeData(VehicleInformation vehicle, int fromDate, int toDate,
                                 List<GenericColumnItemInformation> columns);

        /// <summary>
        /// Extract advertising detail for media schedule creatives details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">User Univers Selection (correspondig to which is the current univers if we have a competitor study)</param>
        /// <param name="filters">Filters Identifiers (A list of values correspondig to the classification levels)</param>
        /// <returns>Advertising detail Data</returns>		
        DataSet GetMSCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters);

        /// <summary>
        /// Get one verion informations like :
        /// ID version , product label,group label,advertiser label
        /// </summary>
        /// <remarks>Use only for media type Tv in France</remarks>
        /// <param name="idVersion">ID version</param>   
        /// <param name="idVehicle">ID Media type</param>
        /// <returns>ID version , product label,group label,advertiser label</returns>
        DataSet GetVersion(string idVersion, long idVehicle);


        /// <summary>
        /// Get version(s) data. The query will return the following fields, in order :
        /// "id_advertiser" : advertiser identifier.
        /// "advertiser" : advertiser label.
        /// "id_product" : product identifier.
        /// "product" : product label.
        /// "id_vehicle" : media type identifier.
        /// "vehicle" :  media type label.
        /// "id_slogan" : version identifier.
        /// "date_media_num" : publication date with format YYYYMMDD.
        /// "id_media" : vehicle identifier.
        /// "advertDimension" : It will be the duration of spot for media type RADIO  and TV. 
        /// The FORMAT of Ad for media type PRESS. The dimension and extension for INTERNET.
        /// The type of board for media type Outdoor.
        /// "associated_file" : creative file.
        /// 
        /// The query data will be filtered with the following parameters :
        /// 
        /// - The List of media type selected : 
        /// <code> string mediaTypeStringIds = _session.CustomerDataFilters.SelectedMediaType;</code>
        /// - The selected period represented by parameters <code>beginningDate</code> and <code>endDate</code> which correspond respectively to the begin and end dates of period.
        /// - The customer  product classification rights, obtained via variable <code>_session.CustomerDataFilters.ProductsRights</code>.
        /// - The customer media classification rights, obtained via variable <code>_session.CustomerDataFilters.MediaRights</code>.
        /// - The  product classification items selected via variable <code>_session.CustomerDataFilters.PrincipalProductUniverses</code>
        /// - Allowed media type via variable <code>module.AllowedMediaUniverse.VehicleList</code>
        /// 
        /// The query will be as the following example :        
        /// <code> 
        /// //Select fields
        /// SELECT distinct id_advertiser,advertiser,id_product,product,id_vehicle, vehicle  ,  id_slogan, date_media_num ,id_media, advertDimension, associated_file
        /// 
        /// FROM My_Table
        /// 
        /// WHERE
        /// 
        /// //Filtering with selected media types
        /// id_vehicle in (100,103,200,400,500,600)
        /// 
        /// //Filtering with period
        ///  AND date_media_num 20110101 //beginingDate
        ///  AND date_media_num 20110131 //endDate
        ///  
        /// //Filtering with product classification rights
        /// AND id_advertiser in (20055,46888,2233655,82100)
        /// 
        /// //Filtering with media classification rights
        /// AND id_media in (77748,8655,835,7536)
        /// 
        /// //Filtering with product items selected
        /// AND id_product in (546546,8922,625887,1354,798692)
        /// 
        /// //Filtering with Allowed media type via
        /// AND id_vehicle in (100,103,200,400,500,600)
        /// 
        /// ORDER BY advertiser,id_advertiser,product ,id_product, vehicle,id_vehicle,id_slogan,associated_file,date_media_num
        /// </code>
        /// </summary>
        /// <param name="beginningDate">date beginning (YYYYMMDD)</param>
        /// <param name="endDate">date end (YYYYMMDD)</param>
        /// <returns>versions data ["id_advertiser","advertiser","id_product","product","id_vehicle","vehicle","id_slogan","date_media_num","id_media","advertDimension", "associated_file" </returns>
        DataSet GetVersions(string beginningDate, string endDate);

        
    }
}
