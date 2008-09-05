#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpress.Web.Exceptions;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpressI.Portofolio.DAL.Engines {
	/// <summary>
	/// Get different data for portofolio structure
	/// </summary>
	public class StructureEngine : Engine {

		#region Variables
		/// <summary>
		/// Beginning hour interval list
		/// </summary>
		protected Dictionary<string, double> _hourBeginningList = null;
		/// <summary>
		/// End hour interval list
		/// </summary>
		protected Dictionary<string, double> _hourEndList = null;
		/// <summary>
		/// Ventilation type list for press result
		/// </summary>
		protected List<PortofolioStructure.Ventilation> _ventilationTypeList = null;		
		#endregion

		#region PortofolioDetailEngine
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		/// <param name="module">module</param>
		public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		/// <param name="module">Module</param>
		/// <param name="hourBeginningList">Hour beginning list</param>
		/// <param name="hourEndList">Hour end list</param>
		public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
			if (hourBeginningList==null || hourBeginningList.Count == 0) throw (new ArgumentException("hourBeginningList parameter is invalid"));
			if (hourEndList==null || hourEndList.Count == 0) throw (new ArgumentException("hourEndList parameter is invalid"));
			if (hourBeginningList.Count != hourEndList.Count ) throw (new ArgumentException("hourEndList and hourBeginningList parameter don't have the same number of elements"));
			_hourBeginningList = hourBeginningList;
			_hourEndList = hourEndList;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		/// <param name="module">module</param>
		/// <param name="ventilationTypeList">ventilation Type List</param>
		public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, List<PortofolioStructure.Ventilation> ventilationTypeList)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
			if (ventilationTypeList == null || ventilationTypeList.Count == 0) throw (new ArgumentException("ventilationTypeList parameter is invalid"));
			_ventilationTypeList = ventilationTypeList;
		}

		#endregion

		#region Implementation abstract methods
		/// <summary>
		/// Get data for structure results
		/// </summary>
		/// <returns></returns>
		protected override DataSet ComputeData() {
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.radio:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return GetStructData();
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return GetPressStructData();
				default: throw new PortofolioDALException("The method to get data is not defined for this vehicle.");
			}
		}
		#endregion

		#region Dataset for tv or radio
		/// <summary>
		/// Get structure data 
		/// </summary>
		/// <remarks>Used for tv or radio</remarks>		
		/// <returns>DataSet</returns>
		protected virtual DataSet GetStructData() {

			#region variables
			string tableName = "";
			string fields = "";			
			StringBuilder sql = new StringBuilder(2000);
			string product = "";
			double hourBegin;
			double hourEnd;
			bool start = true;
			#endregion

			#region construction de la requête
			try {
				//Table name
				tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert);
				
			}
			catch (Exception) {
				throw new PortofolioDALException("GetStructData : impossible to determine Table and Fields for the query.");
			}

			if (tableName != null && tableName.Length > 0) {
				foreach (KeyValuePair<string, double> kpv in _hourBeginningList) {
					if (!start) sql.Append("  union all ");

					//Fields
					fields = GetStructFields(kpv.Key);

					// Select 
					sql.Append("  select " + fields);

					// Tables
					sql.Append(" from " + tableName + " ");

					//Where
					sql.Append("  where ");

					// Period conditions
					sql.Append("  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num >= " + _beginingDate);
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num <= " + _endDate);

					// Hour interval
					hourBegin = kpv.Value;
					hourEnd = _hourEndList[kpv.Key];
					sql.Append(" " + GetHourInterval(hourBegin, hourEnd));

					#region Product Rights

					//Access Rights
					sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

					//list products hap
					string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
					if (listProductHap != null && listProductHap.Length > 0)
						sql.Append(listProductHap);

					//List selected products
					product = GetProductData();
					if (product != null && product.Length > 0)
						sql.Append(product);
					#endregion

					#region Nomenclature Media (Rights and selection)

					sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

					#region Media Rights
					sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
					#endregion

					#region Media selection
					//Vehicle selection média (vehicle)

					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_vehicle = " + _vehicleInformation.Id.GetHashCode().ToString());

					//Media selection	
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media = " + _idMedia.ToString());
					#endregion

					#endregion

					start = false;
				}

			}
			#endregion

			#region Query execution
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for GetStructData(int hourBegin, int hourEnd) : " + sql.ToString(), err));
			}
			#endregion

		}
		#endregion

		#region Get Tv Or Radio Struct Fields
		/// <summary>
		/// Get Tv Or Radio Struct Fields
		/// </summary>		
		/// <returns>SQL</returns>
		protected virtual string GetStructFields(string hourIntevalKey) {
            return " '" + hourIntevalKey + "' as HourInterval," + WebFunctions.SQLGenerator.GetUnitFieldsNameForPortofolio(_webSession, DBConstantes.TableType.Type.dataVehicle, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
		}
		#endregion

		#region Get Hour Interval
		/// <summary>
		/// Get Hour Interval
		/// </summary>
		/// <param name="hourBegin">Hour Begin</param>
		/// <param name="hourEnd">Hour End</param>
		/// <returns>String SQL</returns>
		protected virtual string GetHourInterval(double hourBegin, double hourEnd) {
			string sql = "";
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.radio:
					sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion>=" + hourBegin;
					sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion<=" + hourEnd;
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>=" + hourBegin;
					sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion<=" + hourEnd;
					return sql;
				default:
					throw new PortofolioDALException("GetHourInterval : Vehicle unknown.");
			}
		}
		#endregion

		#region Get Press Struct Data
		/// <summary>
		///  Get Press Struct Data
		/// </summary>
		/// <returns>DataSet</returns>
		protected virtual DataSet GetPressStructData() {

			#region variables
			string tableName = "";
			string fields = "";
			string idVehicle = "";
			string idMedia = "";
			StringBuilder sql = new StringBuilder(2000);
			string product = "";
			#endregion

			#region Build query
			try {
				//Table name
				tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert);
				
			}
			catch (Exception) {
				throw new PortofolioDALException("getPressStructFields(PortofolioStructure.Ventilation ventilation)--> Table unknown.");
			}

			if (tableName != null && tableName.Length > 0 && _ventilationTypeList != null || _ventilationTypeList.Count > 0) {
				
				sql.Append(" select ventilationType,ventilation,insertion from ( ");

				for (int i = 0; i < _ventilationTypeList.Count; i++) {
					if (i > 0) sql.Append(" union all ");

					//Fields
					fields = GetPressStructFields(_ventilationTypeList[i]);

					// Select
					sql.Append(" select " + fields);

					// Tables
					sql.Append(" from ");
					sql.Append(GetPressStructTables(tableName, _ventilationTypeList[i]));

					sql.Append(" where ");
					//Language joints
					sql.Append(GetPressStructLanguage(_ventilationTypeList[i]));

					//press joints
					sql.Append(GetPressStructJoint(_ventilationTypeList[i]));

					// Period condition
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num>=" + _beginingDate);
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num<=" + _endDate);

					#region Products Rights

					//Access Rights
					sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

					//list products hap
					string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
					if (listProductHap != null && listProductHap.Length > 0)
						sql.Append(listProductHap);

					//List selected products
					product = GetProductData();
					if (product != null && product.Length > 0)
						sql.Append(product);
					#endregion

					#region Nomenclature Media (Rights and selection)

					sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

					#region Media Rights
					sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
					#endregion

					#region Media selection
					//Vehicle selection 

					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_vehicle = " + _vehicleInformation.Id.GetHashCode().ToString());

					//Media selection	
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media = " + _idMedia.ToString());
					#endregion

					#endregion

					#region regroupement
					sql.Append(GetPressStructGroupBy(_ventilationTypeList[i]));
					#endregion
				}
				//Order
				sql.Append(" )  group by ventilationType,ventilation,insertion order by ventilationType,insertion desc ");
			}
			#endregion

			#region Query Execution
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for GetPressStructData : " + sql.ToString(), err));
			}
			#endregion

		}
		#endregion

		#region Get Press Struct Fields
		/// <summary>
		/// Get Press Struct Fields
		/// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <returns>SQL</returns>
		protected virtual string GetPressStructFields(PortofolioStructure.Ventilation ventilation) {
            string temp = "'" + ventilation.GetHashCode() + "' as ventilationType, " + WebFunctions.SQLGenerator.GetUnitFieldsNameForPortofolio(_webSession, DBConstantes.TableType.Type.dataVehicle4M, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix)+", ";
			switch (ventilation) {
				case PortofolioStructure.Ventilation.color:
					return temp + " color as ventilation";
				case PortofolioStructure.Ventilation.format:
					return temp + " format as ventilation";
				case PortofolioStructure.Ventilation.insert:
					return temp + " inset as ventilation";
				case PortofolioStructure.Ventilation.location:
					return temp + " location as ventilation";
				default:
					throw new PortofolioDALException("getPressStructFields(PortofolioStructure.Ventilation ventilation)--> No ventilation (format, coulor) corresponding.");
			}
		}
		#endregion

		#region Get Press Struct Tables
		/// <summary>
		/// Get Press Struct Tables
		/// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <param name="tableName">Tables name</param>
		/// <returns>tables name string</returns>
		protected virtual string GetPressStructTables(string tableName, PortofolioStructure.Ventilation ventilation) {
			switch (ventilation) {
				case PortofolioStructure.Ventilation.color:
					return tableName
					+ ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix;
				case PortofolioStructure.Ventilation.format:
					return tableName
					+ " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix;
				case PortofolioStructure.Ventilation.insert:
					return tableName
					+ " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).SqlWithPrefix;
				case PortofolioStructure.Ventilation.location:
					return tableName
					+ " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).SqlWithPrefix
					 + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataLocation).SqlWithPrefix;
				default:
					throw new PortofolioDALException("getPressStructTables(PortofolioStructure.Ventilation ventilation)--> Impossible to determnie ventilation type for press vehicle.");
			}
		}
		#endregion

		#region Get Press Struct Language
		/// <summary>
		/// Get Press Struct Language
		/// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <returns>SQL</returns>
		protected virtual string GetPressStructLanguage(PortofolioStructure.Ventilation ventilation) {
			switch (ventilation) {
				case PortofolioStructure.Ventilation.color:
					return " " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".id_language = " + _webSession.DataLanguage
						+ " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
				case PortofolioStructure.Ventilation.format:
					return " " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".id_language=" + _webSession.DataLanguage
						+ " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
				case PortofolioStructure.Ventilation.insert:
					return " " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).Prefix + ".id_language=" + _webSession.DataLanguage
						+ " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
				case PortofolioStructure.Ventilation.location:
					return " " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).Prefix + ".id_language=" + _webSession.DataLanguage
						+ " and dl.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + ""
						+ " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
				default:
					throw new PortofolioDALException("getPressStructLanguage(PortofolioStructure.Ventilation ventilation)--> Impossible de déterminer le type de language pour la presse.");
			}
		}
		#endregion

		#region Get Press Struct Joint
		/// <summary>
		/// Get Press Struct Joint
		/// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <returns>champq joints</returns>
		protected virtual string GetPressStructJoint(PortofolioStructure.Ventilation ventilation) {
			switch (ventilation) {
				case PortofolioStructure.Ventilation.color:
					return " and  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_color = " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".id_color ";
				case PortofolioStructure.Ventilation.format:
					return " and  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_format =" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".id_format";
				case PortofolioStructure.Ventilation.insert:
					return " and  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_inset = " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).Prefix + ".id_inset"
					 + " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_inset in ( " + WebConstantes.CustomerSessions.InsertType.encart.GetHashCode() + "," + WebConstantes.CustomerSessions.InsertType.flyingEncart.GetHashCode() + " )"
					+ " and srt.id_inset in ( " + WebConstantes.CustomerSessions.InsertType.encart.GetHashCode() + "," + WebConstantes.CustomerSessions.InsertType.flyingEncart.GetHashCode() + " )";
				case PortofolioStructure.Ventilation.location:
					return " and  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media = dl.id_media "
						+ " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).Prefix + ".id_location=dl.id_location "
						//Period
					+ " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num=dl.date_media_num "
					+ "  and dl.ID_ADVERTISEMENT=" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".ID_ADVERTISEMENT ";
				default:
					throw new PortofolioDALException("getPressStructJoint(PortofolioStructure.Ventilation ventilation) : Impossible to realise joint sql for Press vehicle.");
			}
		}
		#endregion

		#region Get Press Struct Group By
		/// <summary>
		///	Get Press Struct Group By
		/// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <returns>Sql string</returns>
		protected virtual string GetPressStructGroupBy(PortofolioStructure.Ventilation ventilation) {
			switch (ventilation) {
				case PortofolioStructure.Ventilation.color:
					return " group by color ";
				case PortofolioStructure.Ventilation.format:
					return " group by format ";
				case PortofolioStructure.Ventilation.insert:
					return " group by inset ";
				case PortofolioStructure.Ventilation.location:
					return " group by location ";
				default:
					throw new PortofolioDALException("getPressStructGroupBy(PortofolioStructure.Ventilation ventilation) : No ventilation (format, couleur) defined.");
			}
		}
		#endregion
	}
}
