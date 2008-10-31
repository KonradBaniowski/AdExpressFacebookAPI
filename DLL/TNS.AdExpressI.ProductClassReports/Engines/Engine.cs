#region Information
/*
 * Author : G Ragneau
 * Created on : 17/07/2008
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



namespace TNS.AdExpressI.ProductClassReports.Engines
{
    /// <summary>
    /// Define default behaviours of engines like global process, lengendaries, properties...
    /// </summary>
    public abstract class Engine
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
        public Engine(WebSession session, int result)
        {
            _session = session;
            _tableType = result;
        }
        #endregion

        #region Access Points
        /// <summary>
        /// Define default process to build a result
        /// </summary>
        /// <returns>Stirng container filled with html code</returns>
        public virtual StringBuilder GetResult()
        {

            StringBuilder html = new StringBuilder(6000000);

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
                UnvalidFrequencyDelivery(html);
                return html;
            }
            catch (NoDataException) { 
                AppendNoData(html);
                return html;
            }
            catch (Exception e) { throw new ProductClassReportsException("Error while retrieving data for database.", e); }
            #endregion

            #region Compute data
            object[,] t = null;
            try
            {
                t = ComputeData(data);
            }
            catch (DeliveryFrequencyException) { 
                UnvalidFrequencyDelivery(html);
                return html;
            }
            catch (System.Exception e)
            {
                throw (new ProductClassReportsException(string.Format("Unable to compute data for product class report {0} ", _tableType), e));
            }
            if (t == null || t.Length == 0)
            {
                AppendNoData(html);
                return html;
            }
            #endregion

            #region Design table
            AppendLegendary(html);
            BuildHTML(html, t);
            #endregion

            return html;

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
        /// Compute data got from the DAL layer before to design the report
        /// </summary>
        /// <param name="data">DAL data</param>
        /// <returns>Table of computed data</returns>
        protected abstract object[,] ComputeData(DataSet data);
        /// <summary>
        /// Design report
        /// </summary>
        /// <param name="str">String container to fill with HTLM code</param>
        /// <param name="data">data to display</param>
        protected abstract void BuildHTML(StringBuilder str, object[,] data);
        #endregion

        #region Common methods about UI

        #region Legendary
        /// <summary>
        /// Append the legendary to the string container
        /// </summary>
        /// <param name="str">StringBuilder to fill with legendary</param>
        protected virtual void AppendLegendary(StringBuilder str)
        {

            //if references are not empty or competitors is not null and there are advertisers
            if (
                (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].Contains(0) && _session.SecondaryProductUniverses[0].GetGroup(0).Contains(TNSClassificationLevels.ADVERTISER))
                ||
                (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].Contains(0) && _session.SecondaryProductUniverses[1].GetGroup(0).Contains(TNSClassificationLevels.ADVERTISER))
                )
            {
                string cssCompeting=string.Empty;
                string cssReference = "Green";
                if (!_excel) 
                    cssCompeting = "Red";
                else 
                    cssCompeting = "palePink";

                str.Append("<table cellPadding=0 cellSpacing=10px border=0 class=\"txtNoir11Bold\"><tr>");
                //str.Append("<td class=Green>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                str.AppendFormat("<td class=\"{0}\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>",cssReference);
                str.AppendFormat("<td>{0}</td>", GestionWeb.GetWebWord(1230, _session.SiteLanguage));
                //str.Append("<td class=Red>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                str.AppendFormat("<td class=\"{0}\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>", cssCompeting);
                str.AppendFormat("<td>{0}</td>", GestionWeb.GetWebWord(1231, _session.SiteLanguage));
                str.AppendFormat("<td>{0}</td>", GestionWeb.GetWebWord(1232, _session.SiteLanguage));
                str.Append("</tr></table>");
            }

        }
        #endregion

        #region No Data
        /// <summary>
        /// Append message meaning "No data for the selected univers"
        /// </summary>
        protected virtual void AppendNoData(StringBuilder str)
        {
            str.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\"><br><br>{0}<br><br><br></div>", GestionWeb.GetWebWord(177, _session.SiteLanguage));
        }
        #endregion

        #region Unvalid frequency
        /// <summary>
        /// Append message meaning "Frequency for data delivering is not valid"
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Code HTML</returns>
        protected virtual void UnvalidFrequencyDelivery(StringBuilder str)
        {
            str.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\"><br><br>{0}<br><br><br></div>", GestionWeb.GetWebWord(1234, _session.SiteLanguage));
        }
        #endregion

        #region Append Data

        #region Afficher une valeur numérique
        /// <summary>
        /// Append numeric value html
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="html">HTML to fill</param>
        /// <param name="data">data</param>
        /// <param name="multiYearLine">True if multiple years</param>
        /// <param name="lineLabel">Line label</param>
        /// <param name="excel">True if Excel</param>
        /// <param name="totalLine">Total line</param>
        protected virtual void AppendNumericData(WebSession _session, StringBuilder html, double data, bool multiYearLine, string lineLabel, bool totalLine, IFormatProvider fp)
        {

            if (!multiYearLine && lineLabel.StartsWith(GestionWeb.GetWebWord(1168, _session.SiteLanguage)))
            {
                if (!_excel)
                {
                    //Evolution
                    if (data > 0) //rise
                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %" : "") + "<img src=/I/g.gif></td>");
                    else if (data < 0) //slide
                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %" : "") + "<img src=/I/r.gif></td>");
                    else if (!Double.IsNaN(data)) // 0 exactly
                        html.Append("<td nowrap>0 %<img src=/I/o.gif></td>");
                    else
                        html.Append("<td nowrap></td>");
                }
                else if (_excel)
                {
                    //Evolution
                    if (data > 0) //rise
                        html.Append("<td nowrap> " + ((!Double.IsInfinity(data)) ? Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %" : "") + "</td>");
                    else if (data < 0) //slide
                        html.Append("<td nowrap> " + ((!Double.IsInfinity(data)) ? Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %" : "") + "</td>");
                    else if (!Double.IsNaN(data)) // 0 exactly
                        html.Append("<td nowrap> 0 %</td>");
                    else
                        html.Append("<td nowrap></td>");
                }
            }
            else if (data != 0 &&
                (!multiYearLine && lineLabel.StartsWith(GestionWeb.GetWebWord(1166, _session.SiteLanguage)))
                || (!multiYearLine && lineLabel.StartsWith(GestionWeb.GetWebWord(806, _session.SiteLanguage))))
            {
                //PDV ou PDM
                html.Append("<td nowrap>" + ((data == 0 || Double.IsNaN(data) || Double.IsInfinity(data)) ? "" : Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %") + "</td>");
            }
            else
            {
                //!PDV et !PDM et !evol
                if (!totalLine)
                {
                    html.Append("<td nowrap>" + ((!double.IsNaN(data) && data != 0) ? Units.ConvertUnitValueToString(data, _session.Unit, fp) : "") + "</td>");
                }
                else
                {
                    html.Append("<td nowrap></td>");
                }
            }
        }
        #endregion

        #region Afficher une valeur numérique
        /// <summary>
        /// Append numeric value html
        /// </summary>
        /// <param name="_session">User Session</param>
        /// <param name="html">Html to fill</param>
        /// <param name="data">Data</param>
        /// <param name="evolution">True if evolution</param>
        /// <param name="percentage">True if percentage</param>
        /// <param name="dataLineUpperbound">?Bonne Question?</param>
        /// <param name="currenCol">Current column</param>
        /// <param name="first_data_column">First column</param>
        /// <param name="excel">True if Excel, False either</param>
        protected virtual void AppendNumericData(WebSession _session, StringBuilder html, double data, bool evolution, bool percentage, int dataLineUpperbound, int currenCol, int first_data_column, bool excel, IFormatProvider fp)
        {

            if (evolution && currenCol == dataLineUpperbound)
            {
                if (!excel)
                {
                    //Evolution
                    if (data > 0) //rise
                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %" : "") + "<img src=/I/g.gif></td>");
                    else if (data < 0) //slide
                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %" : "") + "<img src=/I/r.gif></td>");
                    else if (!Double.IsNaN(data)) // 0 exactly
                        html.Append("<td nowrap> 0 %<img src=/I/o.gif></td>");
                    else
                        html.Append("<td></td>");
                }
                else if (excel)
                {
                    //Evolution
                    if (data > 0) //rise
                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %" : "") + "</td>");
                    else if (data < 0) //slide
                        html.Append("<td nowrap>" + ((!Double.IsInfinity(data)) ? Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %" : "") + "</td>");
                    else if (!Double.IsNaN(data)) // 0 exactly
                        html.Append("<td nowrap> 0 %</td>");
                    else
                        html.Append("<td></td>");
                }
            }
            else if (data != 0 && percentage && ((currenCol - first_data_column) % 2) == 1)
            {
                //PDV ou PDM
                html.Append("<td nowrap>" + ((data == 0 || Double.IsNaN(data) || Double.IsInfinity(data)) ? "" : Units.ConvertUnitValueAndPdmToString(data, _session.Unit, true, fp) + " %") + "</td>");
            }
            else
            {
                //!PDV et !PDM et !evol
                html.Append("<td nowrap>" + ((!double.IsNaN(data) && data != 0) ? Units.ConvertUnitValueToString(data, _session.Unit, fp) : "") + "</td>");

            }

        }
        #endregion

        #region Test si une ligne est une ligne multiAnnée ou non
        /// <summary>
        /// Test if a line is multiyear or not
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="first_data_line">First Line</param>
        /// <param name="nb_year">Number of year</param>
        /// <param name="nb_option">Number of options</param>
        /// <returns>True if the line os multiyear, else false</returns>
        protected virtual bool IsMultiYearLine(int line, int first_data_line, int nb_year, int nb_option)
        {
            return (line - first_data_line) % (1 + nb_year + nb_option) == 0;
        }
        #endregion
        
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
