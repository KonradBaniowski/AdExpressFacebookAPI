using System;
using System.Collections.Generic;
using System.Text;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Functions;
using System.Threading;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Hotep.Russia
{
    public class Functions
    {
        #region Units formatting
        /// <summary>
        /// Format value depending on the unit
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="unit">Unit</param>
        /// <param name="fp">Format provider (if null, current thread culture UI is used)</param>
        /// <returns>Formatted value</returns>
        public static string ConvertUnitValueToString(object value, CstWeb.CustomerSessions.Unit unit, IFormatProvider fp)
        {

            if (IsNull(value) || value.ToString().Length <= 0)
                return string.Empty;

            if (fp == null)
            {
                fp = Thread.CurrentThread.CurrentUICulture;
            }
            string f = "{0:N}";
            try
            {
                f = UnitsInformation.Get(unit).StringFormat;
            }
            catch { }


            switch (unit)
            {
                case CstWeb.CustomerSessions.Unit.pages:
                    return string.Format(fp, f, ConvertToDouble(value, fp));
                case CstWeb.CustomerSessions.Unit.duration:
                    return string.Format(fp, f, ConvertToDuration(value, fp));
                case CstWeb.CustomerSessions.Unit.kEuro:
                case CstWeb.CustomerSessions.Unit.kRubles:
                case CstWeb.CustomerSessions.Unit.kusd:
                    return string.Format(fp, f, ConvertToKEuro(value, fp));
                case CstWeb.CustomerSessions.Unit.euro:
                case CstWeb.CustomerSessions.Unit.grp:
                case CstWeb.CustomerSessions.Unit.spot:
                case CstWeb.CustomerSessions.Unit.insertion:
                case CstWeb.CustomerSessions.Unit.versionNb:
                case CstWeb.CustomerSessions.Unit.mmPerCol:
                default:
                    return string.Format(fp, f, Convert.ToDouble(value.ToString()));
            }

        }

        /// <summary>
        /// Format value with specified unit format (PDM or not)
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="unit">Unit</param>
        /// <param name="pdm">Format as PDM if true</param>
        /// <returns>Formatted value</returns>
        public static string ConvertUnitValueAndPdmToString(object value, CstWeb.CustomerSessions.Unit unit, bool pdm, IFormatProvider fp)
        {
            if (!pdm)
            {
                return ConvertUnitValueToString(value, unit, fp);
            }
            else
            {
                if (fp != null)
                {
                    return string.Format(fp, "{0:percentWOSign}", Convert.ToDouble(value, fp));
                }
                else
                {
                    if (!IsNull(value) && value.ToString().Length > 0)
                    {
                        return Convert.ToDouble(value, fp).ToString("#,##0.00");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Determine if object is null
        /// </summary>
        /// <param name="obj">objet</param>
        /// <returns>True if object is null</returns>
        public static bool IsNull(object obj)
        {
            return (obj == null || obj == System.DBNull.Value);
        }

        #region ConvertToDouble
        /// <summary>
        /// Convert to double according to culture info
        /// </summary>
        /// <param name="o">Object to convert</param>
        /// <param name="dataLanguage">ID data language</param>
        /// <returns>double</returns>
        public static double ConvertToDouble(object o, IFormatProvider fp)
        {

            return Convert.ToDouble(o, fp);
        }

        /// <summary>
        /// Format value from euro to keuro
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="decimals">Size of the decimal part</param>
        /// <returns>Formatted value</returns>
        /// <remarks>Divide value by 1000</remarks>
        public static double ConvertToKEuro(object value, IFormatProvider fp)
        {
            return Convert.ToDouble(value, fp) / 1000;

        }

        /// <summary>
        /// Get value (seconds : SSSSSSSSS) as duration (HHMMSS)
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Formattable value</returns>
        /// <remarks>Convert number of seconds in numerical value set as HHMMSS</remarks>
        public static double ConvertToDuration(object value, IFormatProvider fp)
        {
            long v = Convert.ToInt64(value, fp);
            long h = (long)v / 3600;
            long m = (long)(v - (h * 3600)) / 60;
            long s = (long)(v - (h * 3600) - (m * 60));
            v = (h * 10000) + (m * 100) + s;
            return v;
        }
        #endregion
    }
}
