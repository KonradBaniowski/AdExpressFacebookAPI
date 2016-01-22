#region Informations
// Auteur: D. Mussuma
// Date de cr�ation: 1/06/2006
// Date de modification: 
#endregion

using System;
using System.Data;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using AccessCst = TNS.AdExpress.Constantes.Customer.Right.type;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using DBTableFieldsName = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebCommon = TNS.AdExpress.Web.Common;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.Web;
using DbSchemas = TNS.AdExpress.Constantes.DB.Schema;
using DbTables = TNS.AdExpress.Constantes.DB.Tables;
using CustomerCst = TNS.AdExpress.Constantes.Customer;

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;


namespace TNS.AdExpress.Web.DataAccess.Selections.Slogans
{
    /// <summary>
    /// Classe qui fournit les donn�es concernant les Versions.
    /// </summary>
    public class SloganDataAccess
    {
        #region Liste de slogan en fonction de l'univers s�lectionn�

        /// <summary>
        /// Obtient la liste des versions en fonction de l'univers s�lectionn� par le client.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginningDate">Date de d�but</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>Liste des versions par produit et media(vehicle)</returns>
        public static DataSet GetData(WebSession webSession, string beginningDate, string endDate)
        {

            string sql = "";
            string tempSql = "";
            bool first = true;
            string[] listVehicles = null;

            if (webSession.CurrentModule == Constantes.Web.Module.Name.JUSTIFICATIFS_PRESSE) listVehicles = new string[] { "" + VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.press) + "" };
            else listVehicles = webSession.GetSelection(webSession.SelectionUniversMedia, AccessCst.vehicleAccess).Split(new char[] { ',' });

            //DataSet ds = new DataSet();
            for (int i = 0; i < listVehicles.Length; i++)
            {
                try
                {

                    if (webSession.CurrentModule == Constantes.Web.Module.Name.BILAN_CAMPAGNE)
                        tempSql = GetSQLQueryForAPPM(webSession, listVehicles[i], beginningDate, endDate);
                    else
                        tempSql = GetSQLQuery(webSession, listVehicles[i], beginningDate, endDate);

                    if (tempSql.Length > 0)
                    {
                        if (!first) sql += "  union  ";
                        sql += tempSql;
                        first = false;
                    }

                }
                catch (System.Exception err)
                {
                    throw new Exceptions.SloganDataAccessException(" Impssible de charger la liste des versions en fonction de l'univers client.", err);
                }

            }
            if (sql.Length > 0)
            {
                tempSql = sql;
                // Ordre
                sql = " select * from (";
                sql += " " + tempSql;
                sql += " ) order by advertiser,id_advertiser,product ,id_product, vehicle,id_vehicle,id_slogan,sloganFile,date_media_num";
            }

            #region Execution de la requ�te
            try
            {
                return (sql.Length > 0) ? webSession.Source.Fill(sql.ToString()) : null;
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.SloganDataAccessException("Impossible de charger les donn�es pour Affiner l'univers de version " + sql, err));
            }
            #endregion
        }


        #endregion

        #region M�thodes priv�es

        #region Requete pour les modules d'alerte et d'analyse
        /// <summary>
        /// Obtient la requ�te sql pour la liste des slogans
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="beginingDate">Date de d�but</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>requ�te sql</returns>
        private static string GetSQLQuery(WebSession webSession, string idVehicle, string beginingDate, string endDate)
        {

            #region Variables
            string list = "";
            string tableName = "";
            //			string unitField="";
            bool premier = true;
            string sql = "";
            VehicleInformation vehicleInformation = null;
            if (idVehicle != null)
                vehicleInformation = VehiclesInformation.Get(Int64.Parse(idVehicle));
            Table TblVehicle = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.vehicle);
            Table TblProduct = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.product);
            Table TblAdvertiser = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.advertiser);
            Table TblFormat = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.format);
            #endregion

            if (vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(TNS.AdExpress.Domain.Level.DetailLevelItemInformation.Levels.slogan)
                && vehicleInformation != null && vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet)
            {
                #region R�cup�ration des noms de tables et de champs suivant le m�dia(vehcile)
                TNS.AdExpress.Domain.Web.Navigation.Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
                string tablePrefixe = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                if(currentModuleDescription.ModuleType == Constantes.Web.Module.Type.tvSponsorship)
                    tableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleInformation.Id, Constantes.Web.Module.Type.tvSponsorship, webSession.IsSelectRetailerDisplay);
                else if (Dates.Is4M(beginingDate))
                    tableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleInformation.Id, Constantes.Web.Module.Type.alert, webSession.IsSelectRetailerDisplay);
                else
                    tableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleInformation.Id, Constantes.Web.Module.Type.analysis, webSession.IsSelectRetailerDisplay);
                #endregion

                #region Construction de la requ�te

                // S�lection 
                sql = string.Format("select distinct {0}.id_advertiser,advertiser,{0}.id_product,product,{0}.id_vehicle, vehicle", tablePrefixe);
                if (vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
                    && vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile)
                {
                    sql = string.Format("{1}, nvl({0}.id_slogan,0) as id_slogan", tablePrefixe, sql);
                }
                else
                {
                    sql = string.Format("{1}, nvl({0}.hashcode,0) as id_slogan", tablePrefixe, sql);
                }
                // S�lection de la date
                if (vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    )
                    sql += ", date_cover_num as date_media_num ";
                else sql += ", date_media_num ";
                //S�lection champs sp�cifiques � chaque m�dia
                if (GetFields(vehicleInformation.Id, tablePrefixe).Length > 0)
                    sql += "," + GetFields(vehicleInformation.Id, tablePrefixe);

                // Tables
                sql += " from " + TblVehicle.SqlWithPrefix + ", "
                    + TblProduct.SqlWithPrefix + ", "
                + TblAdvertiser.SqlWithPrefix + ", ";
                if (vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    )
                {
                    sql += TblFormat.SqlWithPrefix + ",";
                }

                sql += WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label + "." + tableName + "  " + tablePrefixe;

                // Conditions de jointure
                sql += " Where " + TblVehicle.Prefix + ".id_vehicle=" + tablePrefixe + ".id_vehicle ";
                sql += " and " + TblVehicle.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
                sql += " and " + TblVehicle.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
                sql += " and " + TblProduct.Prefix + ".id_product=" + tablePrefixe + ".id_product ";
                sql += " and " + TblProduct.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
                sql += " and " + TblAdvertiser.Prefix + ".id_advertiser=" + tablePrefixe + ".id_advertiser ";
                sql += " and " + TblAdvertiser.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
                sql += " and " + TblAdvertiser.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
                if (vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine)
                {
                    sql += " and " + TblFormat.Prefix + ".id_format(+)=" + tablePrefixe + ".id_format ";
                    sql += " and " + TblFormat.Prefix + ".id_language(+)=" + webSession.DataLanguage.ToString();
                    sql += " and " + TblFormat.Prefix + ".activation(+)<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
                }

                // P�riode
                sql += " and date_media_num >=" + beginingDate;
                sql += " and date_media_num <=" + endDate;

                // Gestion des s�lections et des droits

                #region Nomenclature Produit (droits)
                premier = true;
                //Droits en acc�s
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
                sql += SQLGenerator.GetClassificationCustomerProductRight(webSession, tablePrefixe, true, module.ProductRightBranches);
                // Produit � exclure 
                ProductItemsList productItemsList = null;
                if (Product.Contains(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID) && (productItemsList = Product.GetItemsList(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)) != null)
                {
                    sql += productItemsList.GetExcludeItemsSql(true, tablePrefixe);
                }
                #endregion

                #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et s�lection)

                if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                    sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(tablePrefixe, true);

                #endregion

                #region Nomenclature Media (droits et s�lection)
                //Media Universe
                sql += WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, tablePrefixe);

                #region Droits
                sql += SQLGenerator.getAnalyseCustomerMediaRight(webSession, tablePrefixe, true);
                #endregion

                #region S�lection
                sql += " and ((" + tablePrefixe + ".id_vehicle= " + idVehicle + ")) ";
                #endregion

                #endregion

                #region Nomenclature Produit (Niveau de d�tail)
                // Niveau de produit
                sql += SQLGenerator.getLevelProduct(webSession, tablePrefixe, true);
                #endregion

                if (vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
                    && vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.evaliantMobile)
                {
                    sql += " and " + tablePrefixe + ".id_slogan!=0 ";
                }

                #endregion

            }

            return sql;
        }
        #endregion

        #region Requ�te pour l'APPM
        /// <summary>
        /// Obtient la requ�te sql pour la liste des slogans de l'APPM
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="beginingDate">Date de d�but</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>requ�te sql</returns>
        private static string GetSQLQueryForAPPM(WebSession webSession, string idVehicle, string beginingDate, string endDate)
        {

            #region Variables
            string tableName = "";
            string sql = "";
            string tablePrefixe = "";
            VehicleInformation vehicleInformation = null;
            if (idVehicle != null)
                vehicleInformation = VehiclesInformation.Get(Int64.Parse(idVehicle));
            #endregion

            #region R�cup�ration des noms de tables et de champs suivant le m�dia(vehcile)
            TNS.AdExpress.Domain.Web.Navigation.Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
            tablePrefixe = WebApplicationParameters.GetDataTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.dataPressAPPM, webSession.IsSelectRetailerDisplay).Prefix;
            tableName = " " + WebApplicationParameters.GetDataTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.dataPressAPPM, webSession.IsSelectRetailerDisplay).SqlWithPrefix;

            #endregion

            #region Construction de la requ�te
            if (vehicleInformation != null)
            {
                //additional target
                Int64 idAdditionalTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CustomerCst.Right.type.aepmTargetAccess));

                // S�lection 
                sql += "select distinct " + tablePrefixe + ".id_advertiser,advertiser," + tablePrefixe + ".id_product,product," + tablePrefixe + ".id_vehicle, vehicle, nvl(" + tablePrefixe + ".id_slogan,0) as id_slogan";
                // S�lection de la date
                sql += ", date_media_num,date_cover_num ";
                //S�lection champs sp�cifiques � chaque m�dia
                if (GetFields(vehicleInformation.Id, tablePrefixe).Length > 0)
                    sql += "," + GetFields(vehicleInformation.Id, tablePrefixe);

                // Tables
                sql += " from " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.vehicle).SqlWithPrefix + ", "
                    + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.product).SqlWithPrefix + ", "
                    + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.advertiser).SqlWithPrefix + ", ";
                if (vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine)
                {
                    sql += WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.format).SqlWithPrefix + ",";
                }

                sql += " " + tableName + ",";
                sql += WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.appmTargetMediaAssignment).SqlWithPrefix;

                // Conditions de jointure
                sql += " Where " + tablePrefixe + ".id_slogan!=0 ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.vehicle).Prefix + ".id_vehicle=" + tablePrefixe + ".id_vehicle ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.vehicle).Prefix + ".id_language=" + webSession.DataLanguage.ToString();
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.vehicle).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.product).Prefix + ".id_product=" + tablePrefixe + ".id_product ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.product).Prefix + ".id_language=" + webSession.DataLanguage.ToString();
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.advertiser).Prefix + ".id_advertiser=" + tablePrefixe + ".id_advertiser ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.advertiser).Prefix + ".id_language=" + webSession.DataLanguage.ToString();
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.advertiser).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;

                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.format).Prefix + ".id_format(+)=" + tablePrefixe + ".id_format ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.format).Prefix + ".id_language(+)=" + webSession.DataLanguage.ToString();
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.format).Prefix + ".activation(+)<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;

                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.appmTargetMediaAssignment).Prefix + ".id_media_secodip = " + tablePrefixe + ".id_media ";

                // P�riode								
                sql += " and  date_media_num>=" + beginingDate + " and  date_media_num<=" + endDate + "  ";

                // Gestion des s�lections et des droits

                #region Nomenclature Produit (droits)
                //			premier=true;
                //Droits en acc�s
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
                sql += SQLGenerator.GetClassificationCustomerProductRight(webSession, tablePrefixe, true, module.ProductRightBranches);
                // Produit � exclure 
                ProductItemsList productItemsList = null;
                if (Product.Contains(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID) && (productItemsList = Product.GetItemsList(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)) != null)
                {
                    sql += productItemsList.GetExcludeItemsSql(true, tablePrefixe);
                }
                #endregion

                // Gestion des s�lections et des droits
                //product selection
                if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                    sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(tablePrefixe, true);

                //on one target
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.appmTargetMediaAssignment).Prefix + ".id_target in(" + idAdditionalTarget + ") ";
                //outside encart
                sql += " and " + tablePrefixe + ".id_inset is null ";
                //Media Universe
                sql += SQLGenerator.GetResultMediaUniverse(webSession, tablePrefixe);
                //media rights
                sql += SQLGenerator.getAnalyseCustomerMediaRight(webSession, tablePrefixe, true);
            }
            #endregion

            return sql;
        }
        #endregion

        /// <summary>
        /// Obtient les champs de requ�tes sp�cifiques � un m�dia
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="prefixeTable">Prefixe de la table de donn�es</param>
        /// <returns> champs de requ�tes </returns>
        private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle, string prefixeTable)
        {
            switch (idVehicle)
            {

                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                    return "id_media,TO_CHAR( duration)  as advertDimension, TO_CHAR(associated_file) as sloganFile";
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    return "id_media,format as advertDimension, visual as sloganFile";
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                case DBClassificationConstantes.Vehicles.names.indoor:
                    return "id_media,type_board as advertDimension, associated_file as sloganFile";
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return "id_media,TO_CHAR(weight) as advertDimension, TO_CHAR(associated_file) as sloganFile";
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return "id_media, (dimension || ' / ' || format) as advertDimension, TO_CHAR(associated_file) as sloganFile";
                default: return "";
            }
        }
        #endregion
    }
}
