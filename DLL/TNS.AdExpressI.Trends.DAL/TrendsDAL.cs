#region Information
/*
 * Author : G Facon
 * Creation : 09/10/2009
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Units;
using CstCustomer=TNS.AdExpress.Constantes.Customer;
using TNSExceptions=TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using WebFunctions=TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web;
#endregion


namespace TNS.AdExpressI.Trends.DAL {
    
    /// <summary>
    /// Trends Report DAL
    /// </summary>
    public abstract class TrendsDAL:ITrendsDAL {

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current vehicle
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        #endregion


        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Customer session which contains user configuration parameters and universe selection</param>
        public TrendsDAL(WebSession session){
            this._session = session;

            #region Sélection du vehicle
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if(vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new TNSExceptions.VehicleException("Selection of media type is not correct"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion

        }
        #endregion


        #region ITrendsDAL Membres
        /// <summary>
        /// Retreive the data for the trends report result
        /// </summary>
        /// <returns>
        /// DataSet Containing the following tables
        /// 
        /// Tables[0] -> DataTable for the Total  (Media Type level)
        /// Tables[1] -> DataTable for the second (Media Category level)
        /// Tables[3] -> DataTable for the third  (Media Vehicle level)
        /// </returns>
        public virtual DataSet GetData() {

            #region Variable
            DataSet ds=new DataSet();
            #endregion

            // Get the levels requestedName
            // Used to known how many levels can be shown
            // For example in Finland AdExpress cannot display media vehicle. So it only display:
            // Media type\Media Category
            TNS.AdExpress.Domain.Level.GenericDetailLevel levelRequested=_session.DetailLevel;
            

            #region The First Level
            if(levelRequested.GetNbLevels>=1) {
                Table totalTable=WebFunctions.SQLGenerator.GetTrendTotalTableInformtation(_session.DetailPeriod);

                #region Execution de la requête
                try {
                    DataSet ds1=_session.Source.Fill(GetOneLevelRequest(totalTable,levelRequested[1],DBConstantes.Hathor.TYPE_TENDENCY_TOTAL));
                    ds.Tables.Add(ds1.Tables[0].Copy());
                    //ds.Tables.Add(_session.Source.Fill(GetRequest(totalTable,levelRequested[1],DBConstantes.Hathor.TYPE_TENDENCY_TOTAL)).Tables[0]);
                }
                catch(System.Exception err) {
                    throw (new DataBaseException("Request Error for Trends report Level 1: ",err));
                }
                #endregion
            }
            #endregion

            #region The Second Level process
            // If there is only 2 main Levels, AdExpress will have to find the data in the subtotal table
            if(levelRequested.GetNbLevels==2) {
                Table totalTable=WebFunctions.SQLGenerator.GetTrendTotalTableInformtation(_session.DetailPeriod);

                #region Execution de la requête
                try {
                    DataSet ds2=_session.Source.Fill(GetOneLevelRequest(totalTable,levelRequested[2],DBConstantes.Hathor.TYPE_TENDENCY_SUBTOTAL));
                    ds2.Tables[0].TableName="Level2";
                    ds.Tables.Add(ds2.Tables[0].Copy());
                }
                catch(System.Exception err) {
                    throw (new DataBaseException("Request Error for Trends report Level 1: ",err));
                }
                #endregion

                return (ds);
            }
            if(levelRequested.GetNbLevels>=2) {
            
            
            }
            #endregion
            
            return (ds);
        }
        #endregion


        /// <summary>
        /// Retrieve the data in one level for the trends report
        /// </summary>
        /// <param name="trendsTable">Trends table to use</param>
        /// <param name="levelInformation">Level information needed</param>
        /// <param name="trendsTypeId">Type of data to retrieve (Total or subtotal)</param>
        /// <returns>Request</returns>
        protected virtual string GetOneLevelRequest(Table trendsTable,DetailLevelItemInformation levelInformation,string trendsTypeId) {

            #region Variables
            StringBuilder sqlRequest=new StringBuilder(1000);
            string trendsTotalTableName;
            string unitsConditions="";
            #endregion

            // Get valid units that AdExpress has to display in the trend report according to the media
            List<UnitInformation> units =  _session.GetValidUnitForResult();

            // In the trends report, the First level is the total (Media type)

            //Get the table description
            trendsTotalTableName=trendsTable.SqlWithPrefix;

            sqlRequest.Append("Select "+trendsTable.Prefix+".DATE_PERIOD,");
            sqlRequest.Append(levelInformation.GetSqlField()+", ");

            //Get Unit items for select
            foreach(UnitInformation currentUnit in units) {
                unitsConditions+="sum("+trendsTable.Prefix+"."+currentUnit.DatabaseTrendsField+"_cur) as "+currentUnit.DatabaseTrendsField+"_cur, ";
                unitsConditions+="sum("+trendsTable.Prefix+"."+currentUnit.DatabaseTrendsField+"_prev) as "+currentUnit.DatabaseTrendsField+"_prev, ";
                unitsConditions+=" decode(sum("+trendsTable.Prefix+"."+currentUnit.DatabaseTrendsField+"_prev),0,100,round(((sum("+trendsTable.Prefix+"."+currentUnit.DatabaseTrendsField+"_cur)-sum("+trendsTable.Prefix+"."+currentUnit.DatabaseTrendsField+"_prev))/sum("+trendsTable.Prefix+"."+currentUnit.DatabaseTrendsField+"_prev))*100,'2')) as "+currentUnit.DatabaseTrendsField+"_evol, ";
            }
            if(unitsConditions.Length==0) throw (new UnitException("Trends Request build: No unit available"));
            sqlRequest.Append(unitsConditions.Substring(0,unitsConditions.Length-2)+" ");


            // From SQL instruction
            sqlRequest.Append("from "+trendsTotalTableName+" ,"+WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql+levelInformation.GetTableNameWithPrefix()+" ");

            // Where SQL instruction
            sqlRequest.Append("where 0=0 ");

            //Build Media classification join
            sqlRequest.Append("and "+trendsTable.Prefix+"."+levelInformation.GetSqlFieldIdWithoutTablePrefix()+"="+levelInformation.GetSqlFieldId()+" ");
            sqlRequest.Append("and "+levelInformation.DataBaseTableNamePrefix+".id_language="+_session.DataLanguage.ToString());
            sqlRequest.Append("and "+levelInformation.DataBaseTableNamePrefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);

            // report in PDM
            if(_session.PDM) {
                sqlRequest.Append("and "+trendsTable.Prefix+".id_pdm = "+ DBConstantes.Hathor.PDM_TRUE+" ");
            }
            else {
                sqlRequest.Append("and "+trendsTable.Prefix+".id_pdm = "+ DBConstantes.Hathor.PDM_FALSE+" ");
            }

            // Level 0=Total
            sqlRequest.Append("and id_type_tendency="+trendsTypeId+" ");

            // Period
            if(_session.PeriodType==CustomerSessions.Period.Type.cumlDate) {
                sqlRequest.Append("and "+trendsTable.Prefix+".id_cumulative ="+DBConstantes.Hathor.CUMULATIVE_TRUE+" ");
            }
            else {
                sqlRequest.Append("and "+trendsTable.Prefix+".date_period between "+_session.PeriodBeginningDate+ " and "+_session.PeriodEndDate+" ");
                sqlRequest.Append("and "+trendsTable.Prefix+".id_cumulative ="+DBConstantes.Hathor.CUMULATIVE_FALSE+" ");
            }

            // Media type working set
            sqlRequest.Append("and "+trendsTable.Prefix+".id_vehicle="+_vehicleInformation.DatabaseId.ToString());

            // Group by
            sqlRequest.Append(" group by "+levelInformation.GetSqlFieldForGroupBy()+", "+trendsTable.Prefix+".DATE_PERIOD ");        
            return(sqlRequest.ToString());
        }
    }
}
