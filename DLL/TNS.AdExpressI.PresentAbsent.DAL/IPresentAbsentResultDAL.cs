#region Information
/*
 * Author : G Ragneau
 * Creation : 18/03/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System.Data;

using TNS.AdExpress.Web.Core.Sessions;
#endregion

namespace TNS.AdExpressI.PresentAbsent.DAL
{

	/// <summary>
	/// Defines interface for methods to Extract data for different type of results of the module Present / Absent Report.
	/// It contains the following public methods :
	/// The method <code>DataTable GetData(); </code> get data of the following types of results :
	/// - Result "Portofolio" : get data corresponding to active products items in the set being searched.
	/// - Result "Present in more than one vehicle" : get data corresponding to products items present 
	/// in more tha one vehicle
	/// - Result "Absent" :  get data corresponding to products items absent from the reference vehicles 
	/// in comparaison with the competing vehicles
	/// - Result "Exclusive to one vehicle" : get data corresponding to the reference vehicles' exclusive 
	/// products items in comparaison with the competing vehicles.
	/// - Result "strenghs" : get data corresponding to the product items whose market share is greater 
	/// than that of the total of the product set, int the reference vehicle set.
	/// - Result "Prospects" : get data corresponding to the product items whose market share is less 
	/// than that of the total of the product set, int the reference vehicle set.
	/// 
	/// All previous results call  method <code>DataSet GetData()</code>.
	/// 
	/// - Result "Summary" : get data corresponding (calls method <code>GetSynthesisData()</code>)to the summary of Present, Absent and 
	///Exclusive items on the main hierarchical levels of the product brand classification.         
	/// 
	/// The method <code>DataSet GetSynthesisData();</code>
	/// loads data for result tab "Summary" : get data corresponding to the summary of Present, Absent and 
	///Exclusive items on the main hierarchical levels of the product brand classification. 
	///  
	/// In the result page, client can choose the vehicle-level detail in column by selecting it from the drop-down menu.
	/// The method  <code>DataSet GetColumnDetails();</code>
	/// gets the list of items corresponding to the vehicle-level selected.	
	///	The method <code>DataSet GetNbParutionData();</code>
	/// Get for each vehicles and period selected  the number of publications
	/// </summary>
    public interface IPresentAbsentResultDAL
    {
		/// <summary>
		///Load data for the module absent/present report.
		///The module absent/present report allows client to view all active product items
		/// from a selection of competing media vehicles (media type, media seller, media vehicle).
		/// This method returns the data for the folloing results :
		/// - Result "Portofolio" : get data corresponding to active products items in the set being searched.
		/// - Result "Present in more than one vehicle" : get data corresponding to products items present 
		/// in more tha one vehicle
		/// - Result "Absent" :  get data corresponding to products items absent from the reference vehicles 
		/// in comparaison with the competing vehicles
		/// - Result "Exclusive to one vehicle" : get data corresponding to the reference vehicles' exclusive 
		/// products items in comparaison with the competing vehicles.
		/// - Result "strenghs" : get data corresponding to the product items whose market share is greater 
		/// than that of the total of the product set, int the reference vehicle set.
		/// - Result "Prospects" : get data corresponding to the product items whose market share is less 
		/// than that of the total of the product set, int the reference vehicle set.
		/// 
		/// - Calls the following methods :
		///<code> CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
		///   DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
		///   GetSynthesisRequest(CstDB.TableType.Type type);
		///   string orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();
		///   string orderFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
		///   string groupByFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();
		///    string productFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
		///  </code>
		/// </summary> 
		/// <example>
		/// GetData method is called in the Bussiness Layer like in the following example. 
		/// The DAL layer is called by reflection with method's parameters. And the the method is called to get the data table of the module.
		/// <code> Dataset dt = null;
		///     if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
		///    object[] parameters = new object[1];
		///   parameters[0] = _session;
		///   IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
		///   dt = presentAbsentDAL.GetData();
		///   </code>
		/// </example>
		/// <remarks> The Data table obtained will be structured as follows :
		/// DATATABLE [ID_MEDIA,"identifier of current column level" ,ID LEVEL 1, LABEL LEVEL 1,...,ID LEVEL N, LABEL LEVEL N,ID_ADRESS,UNIT FIELD]
		/// ID_MEDIA : contains identifiers of vehicles selected.
		/// "identifier of current column level"  : contains identifiers of vehicle-level detail in column.
		/// ID LEVEL N : contains identifiers of classification-level detail in row into the final result table.
		/// LABEL LEVEL N : contains labels of classification-level detail in row into the final result table.
		/// ID_ADRESS : Identifier for compagny description.
		/// UNIT : example EURO (according to unit selected by the client.
		/// </remarks>
		/// <returns>Present/Absent report data set</returns>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
        DataSet GetData();

		/// <summary>
		///Loads data for result tab "Summary" : get data corresponding to the summary of Present, Absent and 
		///Exclusive items on the main hierarchical levels of the product brand classification.         
		///The data table returned contains data in the following order :
		/// <code>select id_sector,id_subsector, d_group_
		/// , id_advertiser,id_brand,id_product,ID_GROUP_ADVERTISING_AGENCY,ID_ADVERTISING_AGENCY
		/// ,id_media, date_num, euro
		/// </code>
		/// So you ca notice that the firts columns correspond to identifiers of the classification product ( from id_sector to id_product).
		/// Then the data columns of advertising agency ("ID_GROUP_ADVERTISING_AGENCY,ID_ADVERTISING_AGENCY").
		/// and column of the identifier of vehicle ("id_media"). 
		/// The column fo current dates of the selection. The "date_num" is an alias.
		/// The field euro is an example of unit's field which depends on the unit selected. 
		/// The unit label changes according to the unit selected by the client (ex. duration, spot).
		///</summary>
		///<remarks> Use the protected method <code>GetSynthesisRequest(CstDB.TableType.Type type);</code> which get summary data
		///accordind to the type of period (monthly, weekly, dayly).</remarks>
		/// <returns>Summary result's data set</returns>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
		DataSet GetSynthesisData();

		/// <summary>    
		/// In the result page, client can choose the vehicle-level detail in column by selecting it from the drop-down menu.
		/// Then this method gets the list of identifiers of items corresponding to the vehicle-level selected.		
		/// </summary>
		/// <returns>Data set with list of vehicle-level items. </returns>		
		/// <remarks>The query must always contains the field of vehicle level ( "id_media" )</remarks>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
		DataSet GetColumnDetails();
		
		/// <summary>
		/// Get for each vehicles and period selected  the number of publications		
		/// </summary>
		/// <remarks> Only for medium PRESS (MAGAZINE/NEWSPAPER)</remarks>
		/// <returns>Data table with in first colum the identifier of vehicle (id_media)
		/// and in the second column the number of publications (NbParution)</returns>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
		DataSet GetNbParutionData();

        long CountData();

    }

}
