#region Information
/*
 * Author : G Ragneau
 * Created on : 12/01/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Classification.Universe;

using TNS.AdExpressI.ProductClassReports.Exceptions;
using TNS.AdExpressI.ProductClassReports.DAL;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;



namespace TNS.AdExpressI.ProductClassReports.GenericEngines
{
    /// <summary>
    /// Define default behaviours of engines like global process, lengendaries, properties...
    /// </summary>
    public abstract class GenericEngine
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Type of result
        /// </summary>
        protected int _tableType;
        /// <summary>
        /// Type of output
        /// </summary>
        protected bool _excel = false;

        #region Rules Engine attributes
        /// <summary>
        /// Specify if the table contains advertisers as references or competitors
        /// </summary>
        protected int _isPersonalized = 0;

        #endregion

        /// <summary>
        /// Report vehicle
        /// </summary>
        protected Vehicles.names _vehicle;

        #endregion

        #region Accessors
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        /// <summary>
        /// Type of result
        /// </summary>
        protected int TableType
        {
            get { return _tableType; }
            set { _tableType = value; }
        }
        /// <summary>
        /// Type of output
        /// </summary>
        public bool Excel
        {
            get { return _excel; }
            set { _excel = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Type of table</param>
        public GenericEngine(WebSession session, int result)
        {
            _session = session;
            _tableType = result;
            _vehicle = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
        }
        #endregion

        #region Access Points
        /// <summary>
        /// Define default process to build a result
        /// </summary>
        /// <returns>Data Result</returns>
        public virtual ResultTable GetResult()
        {

            #region DAL Access
            DataSet data = null;
            try
            {
                data = GetData();
                if (data == null || data.Tables[0].Rows.Count < 1)
                {
                    throw new NoDataException();
                }
            }
            catch (DeliveryFrequencyException) { 
                return GetUnvalidFrequencyDelivery();
            }
            catch (NoDataException) { 
                return GetNoData();
            }
            catch (Exception e) { throw new ProductClassReportsException("Error while retrieving data for database.", e); }
            #endregion

            #region Compute data
            try
            {
                return ComputeData(data);
            }
            catch (DeliveryFrequencyException) { 
                return GetUnvalidFrequencyDelivery();
            }
            catch (System.Exception e)
            {
                throw (new ProductClassReportsException(string.Format("Unable to compute data for product class report {0} ", _tableType), e));
            }
            #endregion

            return null;

        }
        #endregion

        #region Table engine
        /// <summary>
        /// Access to the DAL layer to get data required for the report
        /// </summary>
        /// <returns>DataSet loaded with data required by the report</returns>
        protected virtual DataSet GetData()
        {
            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_session.CurrentModule);
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("DataAccess layer is null for the Product Class Analysis"));
            object[] param = new object[1];
            param[0] = _session;
            IProductClassReportsDAL productClassReportDAL = (IProductClassReportsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            return productClassReportDAL.GetData();
        }
        /// <summary>
        /// Compute data got from the DAL layer
        /// </summary>
        /// <param name="data">DAL data</param>
        /// <returns>Table of computed data</returns>
        protected abstract ResultTable ComputeData(DataSet data);
        #endregion

        #region Common methods about UI

        #region No Data
        /// <summary>
        /// Append message meaning "No data for the selected univers"
        /// </summary>
        protected virtual ResultTable GetNoData()
        {
            return null;
            //str.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\"><br><br>{0}<br><br><br></div>", GestionWeb.GetWebWord(177, _session.SiteLanguage));
        }
        #endregion

        #region Unvalid frequency
        /// <summary>
        /// Append message meaning "Frequency for data delivering is not valid"
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Code HTML</returns>
        protected virtual ResultTable GetUnvalidFrequencyDelivery()
        {
            return null;
            //str.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\"><br><br>{0}<br><br><br></div>", GestionWeb.GetWebWord(1234, _session.SiteLanguage));
        }
        #endregion

        #endregion

        #region Common Methods about rules
        /// <summary>
		/// Method to clean data from empty lines
		/// </summary>
		/// <param name="dsData">Dataset from database access</param>
		/// <param name="firstData">First column with data</param>
		/// <returns>DataTable without empty lines</returns>
		protected void CleanDataTable(DataTable dtData,int firstData){

            int maxColumn = dtData.Columns.Count - _isPersonalized;
            bool hasData = false;

            for(int i = dtData.Rows.Count-1; i >= 0; i--){

                hasData = false;
                for (int j = firstData; j < maxColumn; j++)
                {

                    if (Convert.ToDouble(dtData.Rows[i][j]) != 0)
                    {
                        hasData = true;
                        break;
                    }

                }
                if (!hasData){
                    dtData.Rows.RemoveAt(i);
                }

            }

		}
        #endregion

    }
}
