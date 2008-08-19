#region Information
/*
 * Author : G Ragneau
 * Created on : 22/07/2008
 * Historique TNS.AdExpress.Web.Functions:
 *      24/10/2005 D. V. Mussuma Ajout des fonctions de conversion des unités
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;

using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Core.Utilities
{
    /// <summary>
    /// Utilities about units
    /// </summary>
    public class Units
    {

        #region Attributes
        /// <summary>
        /// List of formats depending on the size of decimal part
        /// </summary>
        protected static Dictionary<int, string> _formats = new Dictionary<int, string>();
        #endregion

        #region Units formatting
        /// <summary>
        /// Format value depending on the type of unit required
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="unit">Unit</param>
        /// <returns>Formatted value</returns>
        public static string ConvertUnitValueToString(object value, CstWeb.CustomerSessions.Unit unit)
        {
            switch (unit)
            {
                case CstWeb.CustomerSessions.Unit.pages:
                    return ConvertUnitValueToString(value, unit, 3);
                case CstWeb.CustomerSessions.Unit.grp:
                    return ConvertUnitValueToString(value, unit, 2);
                case CstWeb.CustomerSessions.Unit.kEuro:
                case CstWeb.CustomerSessions.Unit.duration:
                case CstWeb.CustomerSessions.Unit.euro:
                case CstWeb.CustomerSessions.Unit.mmPerCol:
                case CstWeb.CustomerSessions.Unit.spot:
                case CstWeb.CustomerSessions.Unit.insertion:
                case CstWeb.CustomerSessions.Unit.volume:
                default:
                    return ConvertUnitValueToString(value, unit, 0);
            }
        }

        /// <summary>
        /// Format value depending on the unit
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="unit">Unit</param>
        /// <param name="decimals">Size of the decimal part</param>
        /// <returns>Formatted value</returns>
        public static string ConvertUnitValueToString(object value, CstWeb.CustomerSessions.Unit unit, int decimals)
        {

            #region Check Format
            if (!_formats.ContainsKey(decimals))
            {
                string decimalsString = "### ### ### ##0.";
                for (int i = 0; i < decimals; i++)
                {
                    decimalsString += "#";
                }
                _formats.Add(decimals, decimalsString);
            }
            #endregion

            if (value == null || value.ToString().Length <= 0)
                return string.Empty;

            switch (unit)
            {
                case CstWeb.CustomerSessions.Unit.pages:
                    return ConvertToPages(value, decimals);
                case CstWeb.CustomerSessions.Unit.duration:
                    return DateString.SecondToHH_MM_SS(Convert.ToInt64(value));
                case CstWeb.CustomerSessions.Unit.kEuro:
                    return ConvertToKEuro(value, decimals);
                case CstWeb.CustomerSessions.Unit.euro:
                case CstWeb.CustomerSessions.Unit.grp:
                case CstWeb.CustomerSessions.Unit.spot:
                case CstWeb.CustomerSessions.Unit.insertion:
                case CstWeb.CustomerSessions.Unit.mmPerCol:
                default:
                    return Convert.ToDouble(value).ToString(_formats[decimals]);
            }
        }

        /// <summary>
        /// Format value with specified unit format (PDM or not)
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="unit">Unit</param>
        /// <param name="pdm">Format as PDM if true</param>
        /// <returns>Formatted value</returns>
        public static string ConvertUnitValueAndPdmToString(object value, CstWeb.CustomerSessions.Unit unit, bool pdm)
        {
            if (!pdm) return ConvertUnitValueToString(value, unit);
            else return Convert.ToDouble(value).ToString("# ##0.00");	// ToString("# ### ##0.##")
        }

        /// <summary>
        /// Format value from euro to keuro
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Formatted value</returns>
        /// <remarks>Divide value by 1000</remarks>
        protected static string ConvertToKEuro(object value)
        {
            return ConvertToKEuro(value, 0);
        }

        /// <summary>
        /// Format value from euro to keuro
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="decimals">Size of the decimal part</param>
        /// <returns>Formatted value</returns>
        /// <remarks>Divide value by 1000</remarks>
        protected static string ConvertToKEuro(object value, int decimals)
        {
            double d = Convert.ToDouble(value) / 1000;
            
            if ( d == 0 ) return string.Empty;
            
            return d.ToString(_formats[decimals]);

        }

        /// <summary>
        /// Format value as Pages
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Formatted string</returns>
        /// <remarks>Divide value by 1000</remarks>
        protected static string ConvertToPages(object value)
        {
            return ConvertToPages(value, 0);
        }

        /// <summary>
        /// Format value as Pages
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="decimals">Size of the decimal part</param>
        /// <returns>Formatted string</returns>
        /// <remarks>Divide value by 1000</remarks>
        protected static string ConvertToPages(object value, int decimals)
        {
            double d = Convert.ToDouble(value) / 1000;

            if (d == 0) return string.Empty;

            return d.ToString(_formats[decimals]);

        }
        #endregion

        #region Allowed units depending on medias
        /// <summary>
        /// Get allowed units depending on media selection
        /// </summary>
        /// <param name="vehicleSelection">List of media joined by commas</param>
        /// <returns>Allowed units</returns>
        public static List<CstWeb.CustomerSessions.Unit> getUnitsFromVehicleSelection(string vehicleSelection)
        {
            List<CstWeb.CustomerSessions.Unit> units = new List<CstWeb.CustomerSessions.Unit>();

            List<String> vehicles = new List<String>(vehicleSelection.Split(','));

            List<Int64> vehiclesLong = new List<Int64>();
                      

            if (vehicles != null && vehicles.Count > 0) {

                foreach (string currentVehicle in vehicles)
                    vehiclesLong.Add(Int64.Parse(currentVehicle));

                units = VehiclesInformation.GetCommunUnitList(vehiclesLong);

                #region Old Code
                //if (vehicles.Contains("7") || vehicles.Contains("9"))
                //{
                //    //cinéma or internet in the list
                //    units.Add(CstWeb.CustomerSessions.Unit.euro);
                //}
                //else
                //{
                //    if (vehicles.Count == 2 && vehicles.Contains("2") && vehicles.Contains("3"))
                //    {
                //        //radio or tv only
                //        units.Add(CstWeb.CustomerSessions.Unit.kEuro);
                //        units.Add(CstWeb.CustomerSessions.Unit.euro);
                //        units.Add(CstWeb.CustomerSessions.Unit.duration);
                //        units.Add(CstWeb.CustomerSessions.Unit.spot);
                //    }
                //    else if (vehicles.Count >= 2)
                //    {
                //        //more than one media (different of "radio, tv")
                //        units.Add(CstWeb.CustomerSessions.Unit.kEuro);
                //        units.Add(CstWeb.CustomerSessions.Unit.euro);
                //        if (!vehicles.Contains("10"))
                //            units.Add(CstWeb.CustomerSessions.Unit.insertion);
                //    }
                //    else
                //    {
                //        switch (vehicles[0])
                //        {
                //            case "1":
                //            case "11":
                //                //presse
                //                units.Add(CstWeb.CustomerSessions.Unit.kEuro);
                //                units.Add(CstWeb.CustomerSessions.Unit.euro);
                //                units.Add(CstWeb.CustomerSessions.Unit.pages);
                //                units.Add(CstWeb.CustomerSessions.Unit.mmPerCol);
                //                units.Add(CstWeb.CustomerSessions.Unit.insertion);
                //                break;
                //            case "2":
                //            //radio
                //            case "3":
                //            case "5":
                //                //tv
                //                units.Add(CstWeb.CustomerSessions.Unit.kEuro);
                //                units.Add(CstWeb.CustomerSessions.Unit.euro);
                //                units.Add(CstWeb.CustomerSessions.Unit.duration);
                //                units.Add(CstWeb.CustomerSessions.Unit.spot);
                //                break;
                //            case "8":
                //                //affichage
                //                units.Add(CstWeb.CustomerSessions.Unit.kEuro);
                //                units.Add(CstWeb.CustomerSessions.Unit.euro);
                //                units.Add(CstWeb.CustomerSessions.Unit.numberBoard);
                //                //units.Add(CstWeb.CustomerSessions.Unit.insertion);
                //                break;
                //            case "10":
                //                //MarketingDirect
                //                units.Add(CstWeb.CustomerSessions.Unit.kEuro);
                //                units.Add(CstWeb.CustomerSessions.Unit.euro);
                //                units.Add(CstWeb.CustomerSessions.Unit.volume);
                //                break;
                //            default:
                //                units.Add(CstWeb.CustomerSessions.Unit.kEuro);
                //                units.Add(CstWeb.CustomerSessions.Unit.euro);
                //                break;
                //        }
                //    }
                //}
                #endregion

            }
            else
            {
                units.Add(CstWeb.CustomerSessions.Unit.kEuro);
                units.Add(CstWeb.CustomerSessions.Unit.euro);
            }
            return units;
        }

        /// <summary>
        /// Get allowed units for appm
        /// </summary>
        /// <returns>Allowed units for appm</returns>
        public static List<CstWeb.CustomerSessions.Unit> getUnitsFromAppmPress()
        {

            List<CstWeb.CustomerSessions.Unit> unitsAppm = new List<CstWeb.CustomerSessions.Unit>();
            unitsAppm.Add(CstWeb.CustomerSessions.Unit.kEuro);
            unitsAppm.Add(CstWeb.CustomerSessions.Unit.grp);
            unitsAppm.Add(CstWeb.CustomerSessions.Unit.euro);
            unitsAppm.Add(CstWeb.CustomerSessions.Unit.pages);
            unitsAppm.Add(CstWeb.CustomerSessions.Unit.insertion);
            return unitsAppm;

        }

        /// <summary>
        /// Insets unit list
        /// </summary>
        /// <returns>Inset list</returns>
        public static List<CstWeb.CustomerSessions.Insert> getInserts()
        {
            List<CstWeb.CustomerSessions.Insert> inserts = new List<CstWeb.CustomerSessions.Insert>();
            inserts.Add(CstWeb.CustomerSessions.Insert.total);
            inserts.Add(CstWeb.CustomerSessions.Insert.insert);
            inserts.Add(CstWeb.CustomerSessions.Insert.withOutInsert);
            return inserts;
        }
        #endregion

    }
}
