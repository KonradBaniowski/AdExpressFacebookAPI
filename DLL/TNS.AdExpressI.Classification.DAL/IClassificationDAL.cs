#region Information
/*
 * Author : Y Rkaina && D. Mussuma
 * Created on : 15/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Classification.Universe;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;

namespace TNS.AdExpressI.Classification.DAL {

    /// <summary>
    /// This class provides all the SQL queries to search or select items of the product or vehicle
    /// clasification brand.
    /// The data can be filtered according to the rights or the selections of the customer.
    /// It contains the methods :
    /// - <code>GetMediaType();</code> Which provides to the customer the media items to select into a module.
    /// - <code>GetDetailMedia();</code> get the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
    /// According to the vehicle classification levels choosen by the customer.
    /// - <code>GetDetailMedia(string keyWord);</code> Obtains the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
    /// According to the vehicle classification levels choosen by the customer. Depends also on the classification label searched by the customer (keyWord).
    /// <code>GetItems;</code> All the methods with the same name provide to the customer the possibility to search items in product or vehicle classification brand.
    /// <code>GetRecapItems</code> provides to the customer the possibility to search items in product or vehicle classification brand. 
    /// <remarks>This methods is used only intio the modules
    /// " Product class analysis: Graphic key reports " and "Product class analysis: Detailed reports".</remarks>
    /// </summary>
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
    /// Impossible to execute query
    /// </exception>
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
    /// execution or building of the query to search classification items</exception>
	public interface IClassificationDAL {

        /// <summary>
        /// This method provides SQL queries to get the media classification level's items.
        /// The data are filtered by customer's media rights and selected working set.		
        /// </summary>
        /// <returns>Data set 
        /// with media's identifiers ("idMediaType" column) and media's labels ("mediaType" column).
        /// </returns>
        /// <example>
        /// idMediaType     |       mediaType
        /// 2               |       RADIO
        /// 3               |       TV
        /// </example>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Impossible to execute query
        /// </exception>
        DataSet GetMediaType();

        /// <summary>
        /// Get the list of vehicles organised by Media genre or Sub media or Media Owner or Title...
        /// According to the vehicle classification levels choosen by the customer.
        /// See Vehicles.xml configuration file (MediaSelectionDetailLevel tag)
        /// <example>
        /// If the user wants to select vehicles displayed by Sub media\Vehicle, the clause select of the query will be
        /// : select distinct id_category as idDetailMedia, category as detailMedia, id_media, media from ...
        /// 
        /// idDetailMedia (parent vehicle)  |   detailMedia (parent vehicle)|   id_media  (vehicle)  | media (vehicle)
        /// 1                               |   PARIS RADIO STATIONS        |   2330                 | EUROPE 1
        /// </example>
        /// </summary>	
        /// <returns>Dataset with  a data table of 4 columns : "idDetailMedia", "detailMedia", "id_media", "media".
        /// The column "idDetailMedia" corresponds to the identifier of the level parent.
        /// The column "detailMedia" corresponds to the label of the level parent.
        /// The column "id_media" corresponds to the identifier of the vehicle.
        /// The column "media" corresponds to the label of the vehicle.
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.
        /// </exception>
        /// <remarks>
        /// The media type to use is available through the client selection
        /// </remarks>
		DataSet GetDetailMedia();

        /// <summary>
        /// Get the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
        /// According to the vehicle classification levels choosen by the customer.
        /// <example>
        /// If the user wants to select vehicles displayed by Sub media\Vehicle, the clause select of the query will be
        /// : select distinct id_category as idDetailMedia, category as detailMedia, id_media, media from ...
        /// 
        /// idDetailMedia (parent vehicle)  |   detailMedia (parent vehicle)|   id_media  (vehicle)  | media (vehicle)
        /// 1                               |   PARIS RADIO STATIONS        |   2330                 | EUROPE 1
        /// 
        /// </example>
        /// </summary>	
        /// <param name="keyWord">key word to search  corresponding vehicles </param>
        /// <returns>Dataset with  a data table of 4 columns : "idDetailMedia", "detailMedia", "id_media", "media".
        /// The column "idDetailMedia" corresponds to the identifier of the level parent.
        /// The column "detailMedia" corresponds to the label of the level parent.
        /// The column "id_media" corresponds to the identifier of the vehicle.
        /// The column "media" corresponds to the label of the vehicle.
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.
        /// </exception>
        /// <remarks>
        /// The media type to use is available through the client selection
        /// </remarks>
		DataSet GetDetailMedia(string keyWord);

        /// <summary>
        /// Get the list of sub media corresponding to media type selected
        /// </summary>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
        /// <returns>Dataset with  a data table of 2 columns : "Id_SubMedia", "SubMedia".
        /// The column "Id_SubMedia" corresponds to the identifier of the level sub media.
        /// The column "SubMedia" corresponds to the label of the level sub media.       
        /// </returns>
        DataSet GetSubMediaData();

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  <example>
        /// To search data for vehicle level :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on View of media classification
        /// from all_media_33 wp
        /// 
        /// //Search by label
        /// where wp.media like '%CNN%'
        /// 
        /// //media's rights
        /// and wp.id_vehicle in (1,2,3)
        /// 
        /// ...
        /// 
        /// //Order by vehicle label
        /// order by media
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.
        /// names are defined in the configuration file: UniverseLevels.xml (tag dbTable)
        /// 
        /// </param>
        /// <param name="wordToSearch">Key word to search</param>
        /// <remarks>     
        /// - We use the parameter "classificationLevelLabel" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel, ...){
        ///  
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        ///  //Set Table of the query with the current classification level      
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>
        /// 
        /// Then if parameter "classificationLevelLabel" is equal to "product " the query string will be as follows :
        /// <code>
        /// select distinct pr.id_product as id_item, pr.product as item
        /// from product pr 
        /// order by product
        /// </code>
        /// </remarks>        
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        DataSet GetItems(int classificationLevelInt, string wordToSearch);

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  
        /// The parameter "classificationLevelLabel" corresponds to classification level label
        /// where items must be included in SELECT clause.
        /// 
        /// The parameters "selectedClassificationLevelIds" and "selectedClassificationLevelLabel"
        /// correspond respectively to identifier list and the label of classification level  selected by the customer.
        /// It uses to filter data of the result level ( "classificationLevelLabel").
        /// 
        ///  To build the SQL quer, the preceding parameters are used as follows :
        ///  <example>              
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel, string selectedClassificationLevelIds, string selectedClassificationLevelLabel){
        ///  
        /// View oVioew = ...//Get classification brand view : all_product_33 or all_media_44 (each one contains all items of its classification) 
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        /// 
        ///   /*FROM clause : Obtains the targets tables of the query according to the web site data language*/
        ///  sql.AppendFormat(" from {0}{1} wp", oView.Sql,_session.DataLanguage.ToString());
        /// 
        ///  /*Query conditions */ 
        ///    * Restriction on the classification items selected by the customer*/
        ///	    sql.AppendFormat(" where wp.id_{0} in ( {1} ) ",selectedClassificationLevelLabel,selectedClassificationLevelIds);
        /// ...
        /// 
        ///  /*Query orders fields by items' labels */
        ///     sql.AppendFormat(" order by {0} ", classificationLevelLabel);
        /// }
        /// </code>
        /// 
        /// Then if parameters of the method are :
        ///  <code>
        ///  //Current classification level is product
        /// string classificationLevelLabel = "product";
        ///
        /// //If user has selected advertiser level
        /// string  selectedClassificationLevelLabel="advertiser";
        /// string selectedClassificationLevelIds ="1,35,56,35";
        ///  
        ///  </code>
        ///  
        /// The query string will be as follows :       
        /// <code>
        /// 
        /// //Get product ID and label
        /// Select distinct wp.id_product as id_item, wp.product  as item
        /// 
        /// //Query on product classification View
        /// from all_product_33 wp
        /// 
        /// //Filter by advertiser to obtain associated product
        /// where wp.advertiser in (1,35,56,35) 
        /// 
        /// //product's rights      
        /// ...
        /// 
        /// //Order by product label
        /// order by product
        /// </code>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.
        /// names are defined in the configuration file: UniverseLevels.xml (tag dbTable)
        /// </param>
        /// <param name="selectedClassificationLevelIds"> Selected (by user) classification level identifiers list (identifers separated by comma). These identifiers are used
        /// to filter the data.</param>
        /// <param name="selectedClassificationLevelLabel">Selected (by user) classification level label. This lable is used as field
        /// to filter the data as follows :         
        /// </param>
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        DataSet GetItems(int classificationLevelInt, string selectedClassificationLevelIds, string selectedClassificationLevelLabel);

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights (it means product or media restriction) and identiifer 
        ///  list of classification level's items.
        /// <remarks>     
        /// - We use the parameter "classificationLevelLabel" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel,, string idList){
        ///  
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        /// 
        ///  //Set Table of the query with the current classification level      
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// 
        /// /*Query conditions */
        ///   /* Restriction on the classification items selected by the customer*/
        /// sql.AppendFormat(" where wp.id_{0} in ({1})", classificationLevelLabel, idList);
        /// 
        /// //ordr fields
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>      
        /// </remarks>      
        ///  <example>
        /// To find vehicle level items with identifier list (5,56,59,56) the query string generated will be :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on View of vehicle classification
        /// from all_media_33 wp
        /// 
        /// //Filtering with vehicles ID
        /// where wp.id_media in (5,56,59,56) 
        /// 
        /// //media's rights
        /// and wp.id_vehicle in (1,2,3)
        /// 
        /// ...
        /// 
        /// //Order by vehicle label
        /// order by media
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.</param>
        ///<param name="selectedClassificationLevelIds"> Selected (by user) classification level identifiers list (identifers separated by comma). Identifiers are used
        /// to filter the data.</param>
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        DataSet GetSelectedItems(string classificationLevelLabel, string idList);

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  <remarks>This method is use only for the modules  " Product class analysis: Graphic key reports "
        ///  and "Product class analysis: Detailed reports"
        /// 
        /// /// - We use the parameter "classificationLevelLabel" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel, CustomerRightConstante.type customerRightType){
        ///  
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        ///  //Set Table of the query with the current classification level 
        ///  
        ///   /*FROM clause : get the targets tables of the query*/
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// 
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>
        /// 
        /// </remarks>
        ///  <example>
        /// To search data for vehicle level :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on table of vehicle classification
        /// from media wp
        ///                
        /// //vehicle's rights
        /// and wp.id_media in (1,2,3)
        /// 
        /// ...
        /// 
        /// //Order by vehicle label
        /// order by media
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.</param>
        /// <param name="customerRightType">Enumerator allowing to get user rights of the current classification level.</param>          
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        DataSet GetRecapItems(string classificationLevelLabel, CustomerRightConstante.type customerRightType);
			
		
	}
}
