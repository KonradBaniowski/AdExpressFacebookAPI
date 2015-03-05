#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion

using System;
using System.Data;
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

using TNS.AdExpress.Domain.Units;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Exceptions;


namespace TNS.AdExpressI.Portofolio.DAL.France.Engines {
	/// <summary>
	/// Get different data for calendar of advertising activity
	/// </summary>
    public class CalendarEngine : DAL.Engines.CalendarEngine
    {

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		/// <param name="module">Module</param>
		public CalendarEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
		}
		#endregion




        protected override string GetQueryFields(string dataTableName, ref string detailProductTablesNames, ref string detailProductFields, ref string detailProductJoints,
         ref string unitFieldNameSumWithAlias, ref string dataGroupby, ref string mediaRights, ref string productsRights, ref string detailProductOrderBy, ref string dataJointForInsert,
         ref string listProductHap, ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            try
            {
                dataTableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
                detailProductTablesNames = _webSession.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                detailProductFields = _webSession.GenericProductDetailLevel.GetSqlFields();
                detailProductJoints = _webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                if (_webSession.GetSelectedUnit().Id != TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.versionNb)
                    unitFieldNameSumWithAlias = WebFunctions.SQLGenerator.GetUnitFieldNameSumWithAlias(_webSession, DBConstantes.TableType.Type.dataVehicle4M); //WebFunctions.SQLGenerator.GetUnitFieldName(_webSession);
                else
                {
                    unitFieldNameSumWithAlias = GetUnitFieldName(_webSession, DBConstantes.TableType.Type.dataVehicle4M);
                    dataGroupby = string.Format(",{0}.hashcode", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                }

                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                detailProductOrderBy = _webSession.GenericProductDetailLevel.GetSqlOrderFields();
                //option encarts (pour la presse)
                if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id
                    || DBClassificationConstantes.Vehicles.names.newspaper == _vehicleInformation.Id || DBClassificationConstantes.Vehicles.names.magazine == _vehicleInformation.Id)
                    dataJointForInsert = WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                if (_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                {
                    try
                    {
                        dataTableNameForGad = dataTableNameForGad = ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix;
                        dataFieldsForGad = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad();
                        dataJointForGad = "and " + WebFunctions.SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    }
                    catch (SQLGeneratorException)
                    {
                        ;
                    }
                }
            }
            catch (Exception e)
            {
                throw (new PortofolioDALException("Impossible to init query parameters" + e.Message));
            }
            return dataTableName;
        }


	
	}
}
