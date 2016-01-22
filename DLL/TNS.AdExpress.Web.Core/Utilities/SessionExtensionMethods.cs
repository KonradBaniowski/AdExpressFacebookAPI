using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;

namespace TNS.AdExpress.Web.Core.Utilities
{
    /// <summary>
    /// Web session Extension Methods
    /// </summary>
    public static class SessionExtensionMethods
    {

        public static string GetVpBrandsRights(this WebSession session, string tablePrefix, bool beginByAnd)
        {
            return GetVpBrandsRights(session, tablePrefix, tablePrefix, beginByAnd);
        }

        public static string GetVpBrandsRights(this WebSession session, string circuitPrefix, string brandPrefix, bool beginByAnd)
        {
            var sql = new StringBuilder();
            bool fisrt = true;
            string prefix = string.Empty;

            // Get the circuit authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.circuitAccess].Length > 0)
            {

                if (beginByAnd) sql.Append(" and");
                prefix = !string.IsNullOrEmpty(circuitPrefix) ? string.Format("{0}.", circuitPrefix) : string.Empty;
                sql.AppendFormat(" ((" + SQLGenerator.GetInClauseMagicMethod(prefix + "id_circuit", session.CustomerLogin[CustomerRightConstante.type.circuitAccess], true) + " ");
                fisrt = false;
            }
            // Get the brands authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpBrandAccess].Length > 0)
            {
                if (!fisrt) sql.Append(" or ");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" ((");
                }
                prefix = !string.IsNullOrEmpty(brandPrefix) ? string.Format("{0}.", brandPrefix) : string.Empty;
                sql.AppendFormat(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_brand", session.CustomerLogin[CustomerRightConstante.type.vpBrandAccess], true) + " ");
                fisrt = false;
            }
            if (!fisrt) sql.Append(" )");

            // Get the circuit not authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.circuitException].Length > 0)
            {
                if (!fisrt) sql.Append(" and");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                prefix = !string.IsNullOrEmpty(circuitPrefix) ? string.Format("{0}.", circuitPrefix) : string.Empty;
                sql.AppendFormat(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_circuit", session.CustomerLogin[CustomerRightConstante.type.circuitException], false) + " ");
                fisrt = false;
            }
            // Get the brands not authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpBrandException].Length > 0)
            {
                if (!fisrt) sql.Append(" and");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                prefix = !string.IsNullOrEmpty(brandPrefix) ? string.Format("{0}.", brandPrefix) : string.Empty;
                sql.AppendFormat(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_brand", session.CustomerLogin[CustomerRightConstante.type.vpBrandException], false) + " ");
                fisrt = false;
            }
            if (!fisrt) sql.Append(" )");

            return sql.ToString();
        }

        public static string GetVpProductsRights(this WebSession session, string tablePrefix, bool beginByAnd)
        {
            return GetVpProductsRights(session, tablePrefix, tablePrefix, tablePrefix, beginByAnd);
        }

        public static string GetVpProductsRights(this WebSession session, string segmentPrefix,
            string subSegmentPrefix, string productPrefix, bool beginByAnd)
        {
            var sql = new StringBuilder();
            bool fisrt = true;
            string prefix = string.Empty;

            // Get the segments authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpSegmentAccess].Length > 0)
            {

                if (beginByAnd) sql.Append(" and");
                prefix = !string.IsNullOrEmpty(segmentPrefix) ? string.Format("{0}.", segmentPrefix) : string.Empty;
                sql.AppendFormat(" (("+SQLGenerator.GetInClauseMagicMethod(prefix + "id_segment", session.CustomerLogin[CustomerRightConstante.type.vpSegmentAccess], true) + " ");
                fisrt = false;
            }
            // Get the sub segments authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentAccess].Length > 0)
            {
                if (!fisrt) sql.Append(" or ");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" ((");
                }
                prefix = !string.IsNullOrEmpty(subSegmentPrefix) ? string.Format("{0}.", subSegmentPrefix) : string.Empty;
                sql.AppendFormat(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_category", session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentAccess], true) + " ");
                fisrt = false;
            }
            // Get the products authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpProductAccess].Length > 0)
            {
                if (!fisrt) sql.Append(" or ");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" ((");
                }
                prefix = !string.IsNullOrEmpty(productPrefix) ? string.Format("{0}.", productPrefix) : string.Empty;
                sql.AppendFormat(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_product", session.CustomerLogin[CustomerRightConstante.type.vpProductAccess], true) + " ");
                fisrt = false;
            }
            if (!fisrt) sql.Append(" )");

            // Get the segments not authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpSegmentException].Length > 0)
            {
                if (!fisrt) sql.Append(" and");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                prefix = !string.IsNullOrEmpty(segmentPrefix) ? string.Format("{0}.", segmentPrefix) : string.Empty;
                sql.AppendFormat(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_segment", session.CustomerLogin[CustomerRightConstante.type.vpSegmentException], false) + " ");
                fisrt = false;
            }
            // Get the brands not authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentException].Length > 0)
            {
                if (!fisrt) sql.Append(" and");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                prefix = !string.IsNullOrEmpty(subSegmentPrefix) ? string.Format("{0}.", subSegmentPrefix) : string.Empty;
                sql.AppendFormat(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_category", session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentException], false) + " ");
                fisrt = false;
            }
            // Get the product not authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpProductException].Length > 0)
            {
                if (!fisrt) sql.Append(" and");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                prefix = !string.IsNullOrEmpty(productPrefix) ? string.Format("{0}.", productPrefix) : string.Empty;
                sql.AppendFormat(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_product", session.CustomerLogin[CustomerRightConstante.type.vpProductException], false) + " ");
                fisrt = false;
            }
            if (!fisrt) sql.Append(" )");

            return sql.ToString();
        }

     

        public static string GetVpMediaRights(this WebSession session, string vehiclePrefix,  bool beginByAnd)
        {
            var sql = new StringBuilder();
            bool fisrt = true;
            string prefix = string.Empty;

            // Get the Media type authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpVehicleAccess].Length > 0)
            {

                if (beginByAnd) sql.Append(" and");
                prefix = !string.IsNullOrEmpty(vehiclePrefix) ? string.Format("{0}.", vehiclePrefix) : string.Empty;
                sql.Append(" (( " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_vehicle", session.CustomerLogin[CustomerRightConstante.type.vpVehicleAccess], true) + " ");
                fisrt = false;
            }
         
            if (!fisrt) sql.Append(" )");

            // Get the Media type not authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpVehicleException].Length > 0)
            {
                if (!fisrt) sql.Append(" and");
                else
                {
                    if (beginByAnd) sql.Append(" and");
                    sql.Append(" (");
                }
                prefix = !string.IsNullOrEmpty(vehiclePrefix) ? string.Format("{0}.", vehiclePrefix) : string.Empty;
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(prefix + "id_vehicle", session.CustomerLogin[CustomerRightConstante.type.vpVehicleException], false) + " ");
                fisrt = false;
            }
           
            if (!fisrt) sql.Append(" )");

            return sql.ToString();
        }

        public static bool HasVpMediaRights(this WebSession session)
        {
            return (session.CustomerLogin[CustomerRightConstante.type.vpVehicleAccess].Length > 0
                    || session.CustomerLogin[CustomerRightConstante.type.vpVehicleException].Length > 0);
        }

    }
}
