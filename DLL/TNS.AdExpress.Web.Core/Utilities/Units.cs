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
using TNS.AdExpress.Domain.Units;
using System.Threading;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Core.Utilities
{
    /// <summary>
    /// Utilities about units
    /// </summary>
    public class Units
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

            if (value == null || value.ToString().Length <= 0)
                return string.Empty;

            if (fp == null)
            {
                fp = Thread.CurrentThread.CurrentUICulture;
            }
            string f = "{0:N}";
            try{
                f = UnitsInformation.Get(unit).StringFormat;
            }
            catch{}

            try {
                switch (unit) {
                    case CstWeb.CustomerSessions.Unit.pages:
                        return string.Format(fp, f, ConvertToPages(value));
                    case CstWeb.CustomerSessions.Unit.duration:
                        return string.Format(fp, f, ConvertToDuration(value));
                    case CstWeb.CustomerSessions.Unit.kEuro:
                    case CstWeb.CustomerSessions.Unit.kpln:
                        return string.Format(fp, f, ConvertToKEuro(value));
                    case CstWeb.CustomerSessions.Unit.versionNb:
                        if (value is CellIdsNumber) return string.Format(fp, f, ((CellIdsNumber)value).Value);
                        else return string.Format(fp, f, Convert.ToDouble(value.ToString()));
                    case CstWeb.CustomerSessions.Unit.euro:
                    case CstWeb.CustomerSessions.Unit.grp:
                    case CstWeb.CustomerSessions.Unit.spot:
                    case CstWeb.CustomerSessions.Unit.insertion:
                    case CstWeb.CustomerSessions.Unit.mmPerCol:
                    case CstWeb.CustomerSessions.Unit.pln:
                    default:
                        return string.Format(fp, f, Convert.ToDouble(value.ToString()));
                }
            }
            catch (Exception e) {
                throw new UnitException("Error for convert unit '" + unit.ToString() + "' with type '" + value.GetType().ToString() + "' with value '" + value.ToString() + "'", e);
            }
        }

        /// <summary>
        /// Format value depending on the unit
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="unit">Unit</param>
        /// <param name="fp">Format provider (if null, current thread culture UI is used)</param>
        /// <returns>Formatted value</returns>
        public static double ConvertUnitValue(object value, CstWeb.CustomerSessions.Unit unit)
        {

            if (value == null || value.ToString().Length <= 0)
                return 0;

            switch (unit)
            {
                case CstWeb.CustomerSessions.Unit.pages:                    
                    return ConvertToPages(value);
                case CstWeb.CustomerSessions.Unit.duration:
                    return ConvertToDuration(value);
                case CstWeb.CustomerSessions.Unit.kEuro:
                case CstWeb.CustomerSessions.Unit.kpln:
                    return Math.Round(ConvertToKEuro(value));
                case CstWeb.CustomerSessions.Unit.euro:
                case CstWeb.CustomerSessions.Unit.grp:
                case CstWeb.CustomerSessions.Unit.spot:
                case CstWeb.CustomerSessions.Unit.insertion:
                case CstWeb.CustomerSessions.Unit.versionNb:
                case CstWeb.CustomerSessions.Unit.mmPerCol:
                case CstWeb.CustomerSessions.Unit.pln:
                default:
                    return Convert.ToDouble(value);
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
            if (!pdm){
                return ConvertUnitValueToString(value, unit, fp);
            }
            else{
                if (fp != null)
                {
                    return string.Format(fp, "{0:percentWOSign}", Convert.ToDouble(value));
                }
                else
                {
                    if (value != null && value.ToString().Length > 0)
                    {
                        return Convert.ToDouble(value).ToString("#,##0.00");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Format value from euro to keuro
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="decimals">Size of the decimal part</param>
        /// <returns>Formatted value</returns>
        /// <remarks>Divide value by 1000</remarks>
        public static double ConvertToKEuro(object value)
        {
            return Convert.ToDouble(value) / 1000;
            
        }

        /// <summary>
        /// Get value as pages
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Formattable value</returns>
        /// <remarks>Divide value by 1000</remarks>
        public static double ConvertToPages(object value)
        {
            return Convert.ToDouble(value) / 1000;
        }
        /// <summary>
        /// Get value (seconds : SSSSSSSSS) as duration (HHMMSS)
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Formattable value</returns>
        /// <remarks>Convert number of seconds in numerical value set as HHMMSS</remarks>
        public static double ConvertToDuration(object value)
        {
            long v = Convert.ToInt64(value);
            long h = (long)v / 3600;
            long m = (long)(v - (h * 3600)) / 60;
            long s = (long)(v - (h * 3600) - (m * 60));
            v = (h * 10000) + (m * 100) + s;
            return v;
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
                units.Add(UnitsInformation.DefaultKCurrency);
                units.Add(UnitsInformation.DefaultCurrency);
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
