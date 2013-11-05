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

            // Get the circuit authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.circuitAccess].Length > 0)
            {

                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" (({0}id_circuit in ({1}) ",
                    !string.IsNullOrEmpty(circuitPrefix) ? string.Format("{0}.",circuitPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.circuitAccess]);
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
                sql.AppendFormat(" {0}id_brand in ({1}) ",
                     !string.IsNullOrEmpty(brandPrefix) ? string.Format("{0}.", brandPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpBrandAccess]);
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
                sql.AppendFormat(" {0}id_circuit not in ({1}) ",
                     !string.IsNullOrEmpty(circuitPrefix) ? string.Format("{0}.", circuitPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.circuitException]);
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
                sql.AppendFormat(" {0}id_brand not in ({1}) ",
                     !string.IsNullOrEmpty(brandPrefix) ? string.Format("{0}.", brandPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpBrandException]);
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

            // Get the segments authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpSegmentAccess].Length > 0)
            {

                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" (({0}id_segment in ({1}) ",
                      !string.IsNullOrEmpty(segmentPrefix) ? string.Format("{0}.", segmentPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpSegmentAccess]);
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
                sql.AppendFormat(" {0}id_category in ({1}) ",
                    !string.IsNullOrEmpty(subSegmentPrefix) ? string.Format("{0}.", subSegmentPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentAccess]);
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
                sql.AppendFormat(" {0}id_product in ({1}) ",
                       !string.IsNullOrEmpty(productPrefix) ? string.Format("{0}.", productPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpProductAccess]);
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
                sql.AppendFormat(" {0}id_segment not in ({1}) ",
                    !string.IsNullOrEmpty(segmentPrefix) ? string.Format("{0}.", segmentPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpSegmentException]);
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
                sql.AppendFormat(" {0}id_category not in ({1}) ",
                     !string.IsNullOrEmpty(subSegmentPrefix) ? string.Format("{0}.", subSegmentPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpSubSegmentException]);
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
                sql.AppendFormat(" {0}id_product not in ({1}) ",
                        !string.IsNullOrEmpty(productPrefix) ? string.Format("{0}.", productPrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpProductException]);
                fisrt = false;
            }
            if (!fisrt) sql.Append(" )");

            return sql.ToString();
        }

     

        public static string GetVpMediaRights(this WebSession session, string vehiclePrefix,  bool beginByAnd)
        {
            var sql = new StringBuilder();
            bool fisrt = true;

            // Get the Media type authorized for the current customer
            if (session.CustomerLogin[CustomerRightConstante.type.vpVehicleAccess].Length > 0)
            {

                if (beginByAnd) sql.Append(" and");
                sql.AppendFormat(" (({0}id_vehicle in ({1}) ",
                     !string.IsNullOrEmpty(vehiclePrefix) ? string.Format("{0}.", vehiclePrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpVehicleAccess]);
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
                sql.AppendFormat(" {0}id_vehicle not in ({1}) ",
                     !string.IsNullOrEmpty(vehiclePrefix) ? string.Format("{0}.", vehiclePrefix) : string.Empty,
                    session.CustomerLogin[CustomerRightConstante.type.vpVehicleException]);
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
