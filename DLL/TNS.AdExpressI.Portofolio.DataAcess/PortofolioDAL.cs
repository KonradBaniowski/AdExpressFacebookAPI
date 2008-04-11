#region Information
// Author: G. Facon
// Creation date: 26/03/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Exceptions;
using CustormerConstantes=TNS.AdExpress.Constantes.Customer;

namespace TNS.AdExpress.Portofolio.DAL {
    /// <summary>
    /// Portofolio Data Access Layer
    /// </summary>
    public abstract class PortofolioDAL:IPortofolioDAL {

        #region Variables
        /// <summary>
        /// Customer session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Current Module
        /// </summary>
        protected Module _module;
        /// <summary>
        /// Vehicle name
        /// </summary>
        protected DBClassificationConstantes.Vehicles.names _vehicleName;
        /// <summary>
        /// Begining Date
        /// </summary>
        protected string _beginingDate;
        /// <summary>
        /// End Date
        /// </summary>
        protected string _endDate;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleName">Vehicle name</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        protected PortofolioDAL(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName,string beginingDate,string endDate) {
            if(webSession==null) throw (new ArgumentNullException("Customer session is null"));
            if(beginingDate==null || beginingDate.Length==0) throw (new ArgumentException("Begining Date is invalid"));
            if(endDate==null || endDate.Length==0) throw (new ArgumentException("End Date is invalid"));
            _webSession=webSession;
            _beginingDate=beginingDate;
            _endDate=endDate;
            _vehicleName=vehicleName;
            try {
                // Module
                _module=ModulesList.GetModule(webSession.CurrentModule);

            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible to set parameters",err));
            }
        
        }
        #endregion

        #region IPortofolioDAL Membres

        #region Media portofolio
        /// <summary>
        /// Get Data for the Media portofolio
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetMediaPortofolio() {

            #region Variables
            string dataTableName="";
            string dataTableNameForGad="";
            string dataFieldsForGad="";
            string dataJointForGad="";
            string detailProductTablesNames="";
            string detailProductFields="";
            string detailProductJoints="";
            string detailProductOrderBy="";
            string unitsFields="";
            string productsRights="";
            string sql="";
            string mediaList="";
            string dataJointForInsert="";
            string listProductHap="";
            string mediaRights="";
            string mediaAgencyTable=string.Empty;
            string mediaAgencyJoins=string.Empty;
            #endregion

            #region Construction de la requête
            try {
                dataTableName=WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName,_module.ModuleType);
                detailProductTablesNames=_webSession.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                detailProductFields=_webSession.GenericProductDetailLevel.GetSqlFields();
                detailProductJoints=_webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.SiteLanguage,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                unitsFields = WebFunctions.SQLGenerator.GetUnitFields(_vehicleName,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);
                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);
                detailProductOrderBy=_webSession.GenericProductDetailLevel.GetSqlOrderFields();
                //option encarts (pour la presse)
                if(DBClassificationConstantes.Vehicles.names.press==_vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==_vehicleName)
                    dataJointForInsert=WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true,false);

                if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
                    try {
                        dataTableNameForGad=", "+WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix; //+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GAD+" "+DBConstantes.Tables.GAD_PREFIXE;
                        dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
                        dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    }
                    catch(SQLGeneratorException) { ;}
                }
                //Agence_media
                if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency)||_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)) {
                    mediaAgencyTable=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql+"."+_webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
                    mediaAgencyJoins="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
                    mediaAgencyJoins+="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
                }
            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible d'initialiser les paramètres de la requêtes",err));
            }

            sql+=" select "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media, "+detailProductFields+dataFieldsForGad+","+unitsFields;
            sql+=" from "+dataTableName ;
            if(detailProductTablesNames.Length > 0)
                sql+=", "+detailProductTablesNames;
            sql+=" "+dataTableNameForGad;
            // Période
            sql+=" where date_media_num >="+_beginingDate;
            sql+=" and date_media_num <="+_endDate;
            // Jointures Produits
            sql+=" "+detailProductJoints;
            sql+=" "+dataJointForGad;
            sql+=" "+mediaAgencyJoins;

            //Jointures encart
            if(DBClassificationConstantes.Vehicles.names.press==_vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==_vehicleName)
                sql+=" "+dataJointForInsert;

            #region Sélection de Médias
            mediaList+=_webSession.GetSelection((TreeNode)_webSession.ReferenceUniversMedia,CustormerConstantes.Right.type.mediaAccess);
            if(mediaList.Length>0) sql+=" and "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media in ("+mediaList+")";
            #endregion

            #region Sélection de Produits
            sql += " " + GetProductData();
            #endregion

            // Droits des Médias
            // Droits des Produits
            sql+=" "+productsRights;
            sql+=mediaRights;
            sql+=listProductHap;
            // Group by
            sql+=" group by "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media, "+detailProductFields+dataFieldsForGad;
            // Order by
            sql+=" order by "+detailProductOrderBy+","+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media";
            #endregion

            #region Execution de la requête
            try {
                return (_webSession.Source.Fill(sql.ToString()));
            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible de charger des données pour le détail du portefeuille: "+sql,err));
            }
            #endregion
        }
        #endregion

        #region Calendar
        /// <summary>
        /// Get Portofolio calendar
        /// </summary>
        /// <returns>Calendar data set</returns>
        public DataSet GetDataCalendar() {

            #region Variables
            string dataTableName="";
            string dataTableNameForGad="";
            string dataFieldsForGad="";
            string dataJointForGad="";
            string detailProductTablesNames="";
            string detailProductFields="";
            string detailProductJoints="";
            string detailProductOrderBy="";
            string unitField="";
            string productsRights="";
            string sql="";
            string list="";
            //int positionUnivers=1;
            string mediaList="";
            bool premier;
            string dataJointForInsert="";
            string listProductHap="";
            string mediaRights="";
            string mediaAgencyTable=string.Empty;
            string mediaAgencyJoins=string.Empty;
            #endregion

            #region Construction de la requête
            try {
                dataTableName=WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName,_module.ModuleType);
                detailProductTablesNames=_webSession.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                detailProductFields=_webSession.GenericProductDetailLevel.GetSqlFields();
                detailProductJoints=_webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.SiteLanguage,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                unitField = WebFunctions.SQLGenerator.GetUnitFieldName(_webSession);
                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);
                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);
                detailProductOrderBy=_webSession.GenericProductDetailLevel.GetSqlOrderFields();
                //option encarts (pour la presse)
                if(DBClassificationConstantes.Vehicles.names.press==_vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==_vehicleName)
                    dataJointForInsert=WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true,false);

                if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
                    try {
                        dataTableNameForGad=dataTableNameForGad=", "+WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix;
                        dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
                        dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    }
                    catch(SQLGeneratorException) { ;}
                }
                //Agence_media
                if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency)||_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)) {
                    mediaAgencyTable=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql+"."+_webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
                    mediaAgencyJoins="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
                    mediaAgencyJoins+="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
                }
            }
            catch(Exception e) {
                throw (new PortofolioDALException("Impossible d'initialiser les paramètres de la requêtes"+e.Message));
            }

            sql+=" select "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media, "+detailProductFields+dataFieldsForGad+",sum("+unitField+") as unit";
            sql+=", date_media_num"; 
            sql+=" from "+mediaAgencyTable+dataTableName;
            if(detailProductTablesNames.Length > 0)
                sql+=", "+detailProductTablesNames;
            sql+=" "+dataTableNameForGad;
            // Période
            sql+=" where date_media_num >="+_beginingDate;
            sql+=" and date_media_num <="+_endDate;
            // Jointures Produits
            sql+=" "+detailProductJoints;
            sql+=" "+dataJointForGad;
            sql+=" "+mediaAgencyJoins;
            //Jointures encart
            if(DBClassificationConstantes.Vehicles.names.press==_vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==_vehicleName)
                sql+=" "+dataJointForInsert;

            #region Sélection de Médias
            mediaList+=_webSession.GetSelection((TreeNode)_webSession.ReferenceUniversMedia,CustormerConstantes.Right.type.mediaAccess);
            if(mediaList.Length>0) sql+=" and "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media in ("+mediaList+")";
            #endregion

            #region Sélection de Produits
            sql += " " + GetProductData();
            #endregion

            // Droits des Médias
            // Droits des Produits
            sql+=" "+productsRights;
            sql+=mediaRights;
            sql+=listProductHap;
            // Group by
            sql+=" group by "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media, "+detailProductFields+dataFieldsForGad;
            sql+=",date_media_num";
            // Order by
            sql+=" order by "+detailProductOrderBy+","+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media";
            sql+=",date_media_num";
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible de charger des données pour les nouveauté: "+sql,err));
            }
            #endregion

        }

        #endregion

        #endregion

        /// <summary>
        /// Récupère la liste produit de référence
        /// </summary>
        /// <returns>la liste produit de référence</returns>
        private string GetProductData() {
            string sql="";
            if(_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
                sql= _webSession.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);

            return sql;
        }
    }
}
