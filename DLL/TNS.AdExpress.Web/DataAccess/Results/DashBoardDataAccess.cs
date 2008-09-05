#region Information
// Auteur: D. V. Mussuma
// Date de cr�ation: 24/03/2005 
// Date de modification: 01/04/2005 
//	22/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	28/11/2005	B.Masson		webSession.Source
//	10/01/2006 D. V. Mussuma		correction bug droits cat�gories

#endregion

#region Using
using System;
using System.Text;
using System.Data;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.FrameWork.Date;
using Oracle.DataAccess.Client;
using WebModule = TNS.AdExpress.Constantes.Web.Module;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Classification;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results {
    /// <summary>
    /// Extraction des donn�es pour les tableaux de bord
    /// </summary>
    public class DashBoardDataAccess {

        #region GetData
        /// <summary>
        /// M�thode qui retourne les donn�es n�cessaires � la construction d'un tableau de bord
        /// en consid�rant les crit�res de la session.
        /// Elle ajoute au fur et � mesure les diff�rents clauses n�cessaires � la requ�te: select, from, jointures
        /// s�lection media et produit, droits, langage et activation, et regroupement
        /// <seealso cref="TNS.AdExpress.Web.Core.Sessions.WebSession"/>
        /// </summary>
        /// <remarks>
        ///	Utilise les m�thodes suivantes:
        ///		private static string getSelectClause(WebSession webSession,string year);
        ///		private static string getFromClause(WebSession webSession,string year);
        ///		private static string getJointClause(WebSession webSession);
        ///		private static string getSelectionClause(WebSession webSession,string year);
        ///		private static string getCustomerRight(WebSession webSession);
        ///		private static string getGroupBy(WebSession webSession);
        /// </remarks>		
        /// <exception cref="TNS.AdExpress.Web.Exceptions.DashBoardDataAccessException()">
        /// Lanc�e en cas d'erreur lors de l'�x�cution de la requ�te.
        /// </exception>	
        /// <param name="webSession">Sesssion utilisateur</param>
        /// <param name="yearN">ann�e courante</param>
        /// <param name="yearN1">ann�e pr�c�dente</param>
        /// <returns>Groupe de donn�es</returns>
        public static DataSet getData(WebSession webSession, int yearN, int yearN1) {

            #region Variables
            string sql = "  ";
            string tempSql = "";
            bool hasUnionAll = false;
            #endregion

            #region Construction de la requ�te
            if (IsRepartitionSelected(webSession)) {
                #region D�taill� par r�partition
                if (webSession.ComparativeStudy) hasUnionAll = true;
                //Requete P�riode N				
                sql += GetSqlQuery(webSession, yearN.ToString(), hasUnionAll);
                if (hasUnionAll) {
                    sql += " UNION ALL";
                    //Requete P�riode N1							
                    sql += GetSqlQuery(webSession, yearN1.ToString(), !hasUnionAll);
                }
                #endregion
            }
            else {
                #region Sans r�partition
                //Requete P�riode N				
                sql += GetSqlQuery(webSession, yearN.ToString(), hasUnionAll);
                #endregion
            }

            //Requ�te sp�cifique pour tableau 13 (medias\familles)
            if (sql.Length > 0 && webSession.PreformatedTable == CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector) {
                tempSql = " Select " + GetMediaSelectFields(webSession, "") + "," + GetSectorSelectFields(webSession, "") + ",period," + WebFunctions.SQLGenerator.GetUnitFieldNameSumUnionWithAlias(webSession) + " from (";
                tempSql += "  " + sql;
                tempSql += " ) group by " + GetMediaGroupBy(webSession) + "," + GetSectorGroupBy(webSession) + ",period ";
                sql = tempSql += "  order by " + GetMediaOrderFields(webSession, true, "") + "," + GetSectorOrderFields(webSession, true) + ",period ";
            }
            #endregion

            #region Execution de la requ�te
            try {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new DashBoardDataAccessException("Impossible de charger les donn�es pour les tableaux de bord:" + sql, err));
            }
            #endregion

        }
        #endregion

        #region M�thodes Priv�es

        #region Requ�te sql
        /// <summary>
        /// Retourne une  requ�te sql
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <param name="year">ann�e table de donn�es</param>
        /// <param name="hasUnionAll">vrai si clause <code>union all</code></param>
        /// <returns>chaine de requete sql</returns>
        private static string GetSqlQuery(WebSession webSession, string year, bool hasUnionAll) {
            string sql = "";

            sql += GetSelectClause(webSession, year);
            //Clause From
            sql += GetFromClause(webSession, year);
            //Clause Where
            sql += " Where " + GetJointClause(webSession);
            //Jointures
            if (GetJointClause(webSession).Length > 0) sql += " and ";
            else sql += "  ";
            //S�lections utilisateurs
            sql += GetSelectionClause(webSession, year);
            //Droits produits et m�dia 
            sql += "  " + GetCustomerRight(webSession);
            //Clause Group by
            sql += "   Group by " + GetGroupBy(webSession);
            //Clause Order by
            if (!hasUnionAll && GetOrderClause(webSession, "desc", hasUnionAll).Length > 0)
                sql += "  " + GetOrderClause(webSession, "desc", !hasUnionAll);



            return sql;
        }
        #endregion

        #region Noms de(s) table(s) de donn�es
        /// <summary>
        /// M�thode priv�e qui d�tecte la ou les table(s) � utiliser en fonction de la s�lection m�dia, produit
        /// et du niveau de d�tail choisi
        ///		d�tection d'une �tude monom�dia  ==> tv,radio,presse
        ///		niveau de d�tail de la nomenclature produit ==> familles
        /// </summary>	
        /// <param name="webSession">Session utilisateur</param>
        /// <param name="year">ann�e de la table</param>
        /// <returns>Cha�ne de caract�re correspondant au nom de(s) table(s) � attaquer</returns>
        private static string GetTablesName(WebSession webSession, string year) {
            //identification du M�dia  s�lectionn�
            ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            string sql = "  ";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector: //Dernier tableau rajout�
                    sql += GetTBord_1_2_3_TableName(webSession, year);
                    if (GetTablesNomenclatureName(webSession).Length > 0) sql += " , " + GetTablesNomenclatureName(webSession);
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                    sql += GetTBord_4_to_12_TableName(webSession, year);
                    if (GetTablesNomenclatureName(webSession).Length > 0) sql += " , " + GetTablesNomenclatureName(webSession);
                    break;
                default:
                    throw new DashBoardDataAccessException("getTablesName(WebSession webSession) --> Impossible d'identifier le tableau de bord � traiter.");
            }
            return sql;
        }
        /// <summary>
        /// Donne le(s) nom(s) des tables de" la nomenclature produit ou m�dia en fonction
        /// de la s�lection de l'utilisateur
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>Cha�ne de caract�re correspondant au nom de(s) table(s) � attaquer</returns>
        private static string GetTablesNomenclatureName(WebSession webSession) {
            string sql = "";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                    return GetMediaTablesName(webSession);
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                    sql += "" + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector  " + DBConstantes.Tables.SECTOR_PREFIXE;
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    sql = GetMediaTablesName(webSession);
                    if (sql.Length > 0) sql += ",";
                    sql += DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector  " + DBConstantes.Tables.SECTOR_PREFIXE;
                    return sql;
                default:
                    return sql;
            }
            return sql;

        }
        /// <summary>
        /// Donne les tables m�dias en fonction de la s�lection client
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>Cha�ne de caract�re correspondant au nom de(s) table(s) m�dias � attaquer</returns>
        private static string GetMediaTablesName(WebSession webSession) {
            string sql = "";
            if (webSession.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia) {
                sql += "" + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".interest_center   " + DBConstantes.Tables.INTEREST_CENTER_PREFIXE;
                if (!IsRepartitionSelected(webSession)) sql += "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".vehicle   " + DBConstantes.Tables.VEHICLE_PREFIXE;
                sql += "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media   " + DBConstantes.Tables.MEDIA_PREFIXE;
            }
            else if (webSession.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter) {
                sql += "" + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".interest_center   " + DBConstantes.Tables.INTEREST_CENTER_PREFIXE;
                if (!IsRepartitionSelected(webSession)) {
                    if (sql.Length > 0) sql += ",";
                    sql += " " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".vehicle   " + DBConstantes.Tables.VEHICLE_PREFIXE;
                }
            }
            else if (webSession.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia) {
                if (!IsRepartitionSelected(webSession)) sql += " " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".vehicle   " + DBConstantes.Tables.VEHICLE_PREFIXE;
                if (sql.Length > 0) sql += ",";
                sql += " " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media   " + DBConstantes.Tables.MEDIA_PREFIXE;
            }
            return sql;
        }

        #region Noms tables pour tableaux de bord 1,2,3
        /// <summary>
        /// Indique le nom de la table � attaquer pour le traitement des tableaux
        /// de bord 1,2,3
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="year">suffixe ann�e de la table de donn�e</param>
        /// <returns>Cha�ne de caract�re correspondant au nom de(s) table(s) � attaquer</returns>
        private static string GetTBord_1_2_3_TableName(WebSession webSession, string year) {
            //identification du M�dia  s�lectionn�
			ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            //Obtient la table � attaquer
            if (IsRepartitionSelected(webSession)) {
                if (IsCrossRepartitionType(webSession)) {
                    //Obtient la table � attaquer
                    switch (vehicleType) {
                        case ClassificationCst.DB.Vehicles.names.radio:
                            return DBConstantes.DashBoard.Tables.TABLEAU_BORD_RADIO_DAY + "_" + year + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
                        case ClassificationCst.DB.Vehicles.names.tv:
                        case ClassificationCst.DB.Vehicles.names.others:
                            return DBConstantes.DashBoard.Tables.TABLEAU_BORD_TV_DAY + "_" + year + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
                        default:
                            throw (new DashBoardDataAccessException("getTBord_1_2_3_TableName(WebSession webSession,string year) : Impossible de d�terminer la table m�dia � traiter."));
                    }
                }
                else return GetTBord_4_to_12_TableName(webSession, year);
            }
            else {
                //Sans d�tail de r�partition
                switch (webSession.DetailPeriod) {
                    case CstPeriodDetail.monthly:
                        return DBConstantes.DashBoard.Tables.TABLEAU_BORD_PLURI_MONTH + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
                    case CstPeriodDetail.weekly:
                        return DBConstantes.DashBoard.Tables.TABLEAU_BORD_PLURI_WEEK + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
                    default:
                        throw (new DashBoardDataAccessException("getTBord_1_2_3_TableName(WebSession webSession,string year) : Impossible de d�terminer la table WEB_PLAN_MEDIA � traiter."));
                }
            }
        }
        #endregion

        #region Noms tables pour tableaux de bord 4 � 12
        /// <summary>
        /// D�termine la table � attaquer pour les tableaux de bord 4 � 12
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="year">suffixe repr�sentant ann�e de la table</param>
        /// <returns>Cha�ne de caract�re correspondant au nom de(s) table(s) � attaquer</returns>
        private static string GetTBord_4_to_12_TableName(WebSession webSession, string year) {
            //identification du M�dia  s�lectionn�
			//string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
            ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            //Obtient la table � attaquer
            switch (vehicleType) {
                case ClassificationCst.DB.Vehicles.names.radio:
                    return GetTBord_4_to_12_RadioTableName(webSession, year);
                case ClassificationCst.DB.Vehicles.names.tv:
                case ClassificationCst.DB.Vehicles.names.others:
                    return GetTBord_4_to_12_TvTableName(webSession, year);
                default:
                    throw (new DashBoardDataAccessException("getTBord_4_to_12_TableName(WebSession webSession,string year) : Impossible de d�terminer la table m�dia � traiter."));
            }
        }
        /// <summary>
        /// D�termine la table � attaquer pour les tableaux de bord 4 � 12
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <param name="year">suffixe repr�sentant ann�e de la table</param>
        /// <returns>Cha�ne de caract�re correspondant au nom de(s) table(s) � attaquer</returns>
        private static string GetTBord_4_to_12_RadioTableName(WebSession webSession, string year) {
            if (IsCrossRepartitionType(webSession))
                return DBConstantes.DashBoard.Tables.TABLEAU_BORD_RADIO_DAY + "_" + year + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
            else {
                switch (webSession.DetailPeriod) {
                    case CstPeriodDetail.monthly:
                        return DBConstantes.DashBoard.Tables.TABLEAU_BORD_RADIO_R_MTH + "_" + year + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
                    case CstPeriodDetail.weekly:
                        return DBConstantes.DashBoard.Tables.TABLEAU_BORD_RADIO_R_WEEK + "_" + year + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
                    default:
                        throw (new DashBoardDataAccessException("getTBord_4_to_12_RadioTableName(WebSession webSession,string year) : Impossible de d�terminer la table m�dia � traiter."));
                }
            }
        }
        /// <summary>
        /// D�termine la table � attaquer pour les tableaux de bord 4 � 12
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <param name="year">suffixe repr�sentant ann�e de la table</param>
        /// <returns>Cha�ne de caract�re correspondant au nom de(s) table(s) � attaquer</returns>
        private static string GetTBord_4_to_12_TvTableName(WebSession webSession, string year) {
            if (IsCrossRepartitionType(webSession))
                return DBConstantes.DashBoard.Tables.TABLEAU_BORD_TV_DAY + "_" + year + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
            else {
                switch (webSession.DetailPeriod) {
                    case CstPeriodDetail.monthly:
                        return DBConstantes.DashBoard.Tables.TABLEAU_BORD_TV_REP_MTH + "_" + year + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
                    case CstPeriodDetail.weekly:
                        return DBConstantes.DashBoard.Tables.TABLEAU_BORD_TV_REP_WEEK + "_" + year + "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE;
                    default:
                        throw (new DashBoardDataAccessException("getTBord_4_to_12_TvTableName(WebSession webSession,string year) : Impossible de d�terminer la table m�dia � traiter."));
                }
            }
        }
        #endregion

        #endregion

        #region  Select
        /// <summary>
        /// chaine de caract�re de la clause "select" 
        /// </summary>
        /// <param name="webSession">sessiion du client</param>
        /// <param name="year">suffixe ann�e de la table de donn�e</param>
        /// <returns>Cha�ne de caract�re correspondant aux champs de(s) table(s) � attaquer</returns>
        private static string GetSelectClause(WebSession webSession, string year) {
            string sql = "  Select  ";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:

                    sql += "  " + GetMediaSelectFields(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    if (WebFunctions.CheckedText.IsStringEmpty(GetDateSelectFields(webSession, year))) {
                        sql += "," + GetDateSelectFields(webSession, year);
                    }

                    sql += "," + GetUnitSelectFields(webSession);
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                    sql += " " + GetSectorSelectFields(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    if (WebFunctions.CheckedText.IsStringEmpty(GetDateSelectFields(webSession, year))) {
                        sql += "," + GetDateSelectFields(webSession, year);
                    }

                    sql += "," + GetUnitSelectFields(webSession);
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                    sql += "  " + GetSectorSelectFields(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    sql += " ," + GetRepartitionField(webSession);
                    sql += "," + GetUnitSelectFields(webSession);
                    if (WebFunctions.CheckedText.IsStringEmpty(GetDateSelectFields(webSession, year))) {
                        sql += "," + GetDateSelectFields(webSession, year);
                    }
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:

                    sql += "  " + GetMediaSelectFields(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    sql += " ," + GetRepartitionField(webSession);
                    sql += "," + GetUnitSelectFields(webSession);
                    if (WebFunctions.CheckedText.IsStringEmpty(GetDateSelectFields(webSession, year))) {
                        sql += "," + GetDateSelectFields(webSession, year);
                    }
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                    sql += "  " + GetRepartitionField(webSession);
                    sql += "," + GetUnitSelectFields(webSession);
                    if (WebFunctions.CheckedText.IsStringEmpty(GetDateSelectFields(webSession, year))) {
                        sql += "," + GetDateSelectFields(webSession, year);
                    }
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    sql += "  " + GetMediaSelectFields(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE) + " , " + GetSectorSelectFields(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    if (WebFunctions.CheckedText.IsStringEmpty(GetDateSelectFields(webSession, year))) {
                        sql += "," + GetDateSelectFields(webSession, year);
                    }
                    sql += "," + GetUnitSelectFields(webSession);
                    break;
                default:
                    throw new DashBoardDataAccessException("getSelectClause(WebSession webSession,string year) --> Impossible d'identifier le tableau de bord � traiter.");
            }
            return sql;
        }
        #region  Select famille
        /// <summary>
        /// chaine de caract�re clause "select" de famille(s)
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="prefixe">prefixe table de donn�es</param>
        /// <returns>Cha�ne de caract�re de familles � s�lectionner</returns>
        private static string GetSectorSelectFields(WebSession webSession, string prefixe) {
            if (prefixe != null && prefixe.Length > 0)
                return prefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector";
            else return " id_sector,sector";
        }
        #endregion

        #region Select Media
        /// <summary>
        /// chaine de caract�re clause "select" de m�dia
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="prefixe">prefixe table de donn�es</param>
        /// <returns>Cha�ne de caract�re de m�dia � s�lectionner</returns>
        private static string GetMediaSelectFields(WebSession webSession, string prefixe) {

            switch (webSession.PreformatedMediaDetail) {
                // selection du niveau media/support
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
                    if (prefixe != null && prefixe.Length > 0)
                        return "  " + prefixe + ".id_media," + DBConstantes.Tables.MEDIA_PREFIXE + ".media";
                    else return "  id_media,media";
                // selection du niveau media/centre d'interet
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
                    if (prefixe != null && prefixe.Length > 0)
                        return "  " + prefixe + ".id_interest_center," + DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".interest_center";
                    else return "  id_interest_center,interest_center";
                // selection du niveau media/centre d'interet/support
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
                    if (prefixe != null && prefixe.Length > 0)
                        return "  " + prefixe + ".id_interest_center," + DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".interest_center,"
                            + prefixe + ".id_media," + DBConstantes.Tables.MEDIA_PREFIXE + ".media";
                    else return "  id_interest_center,interest_center,"
                             + "id_media,media";
                default: return "";
            }
        }
        #endregion

        #region Select Date
        /// <summary>
        /// chaine de caract�re clause "select" des dates
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="year">suffixe ann�e de la table de donn�e</param>
        /// <returns>Cha�ne de caract�re des dates</returns>
        private static string GetDateSelectFields(WebSession webSession, string year) {
            string sql = "";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    if (webSession.PreformatedTable == CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector)
                        sql += "   substr(" + GetDetailPeriodSelectClause(webSession) + ",1,4) as period"; // On tronque la date en r�cup�rant juste l'ann�e pour limiter le nomlbre de lignes ramen�es par la requete afin d'augmenter les perfomances
                    else sql += "  " + GetDetailPeriodSelectClause(webSession) + " as period";
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                    if (IsRepartitionSelected(webSession)) {
                        sql += "  " + year + " as period";
                    }
                    break;
                default:
                    return sql;
            }
            return sql;
        }
        /// <summary>
        /// chaine de caract�re clause "select" des d�tails de dates
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>Cha�ne de caract�re des dates</returns>
        private static string GetDetailPeriodSelectClause(WebSession webSession) {
            string sql = "";
            if (!IsRepartitionSelected(webSession)) {
                if (webSession.DetailPeriod == CstPeriodDetail.monthly)
                    sql += DBConstantes.Tables.DASH_BOARD_PREFIXE + ".month_media_num";
                else if (webSession.DetailPeriod == CstPeriodDetail.weekly)
                    sql += DBConstantes.Tables.DASH_BOARD_PREFIXE + ".week_media_num";
            }
            else {
                if (IsCrossRepartitionType(webSession)) {
                    sql += DBConstantes.Tables.DASH_BOARD_PREFIXE + ".date_media_num";
                }
                else {
                    if (webSession.DetailPeriod == CstPeriodDetail.monthly)
                        sql += DBConstantes.Tables.DASH_BOARD_PREFIXE + ".month_media_num";
                    else if (webSession.DetailPeriod == CstPeriodDetail.weekly)
                        sql += DBConstantes.Tables.DASH_BOARD_PREFIXE + ".week_media_num";
                }
            }
            return sql;
        }
        #endregion

        #region Select Unit�s
        /// <summary>
        /// chaine de caract�re clause "select" des unit�s
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>Cha�ne de caract�re des unit�s � s�lectionner</returns>
        private static string GetUnitSelectFields(WebSession webSession) {
            string sql = "";
            //identification du M�dia  s�lectionn�
			ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            //Choix de l'unit�
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                    if (IsRepartitionSelected(webSession)) {
                        sql += WebFunctions.SQLGenerator.GetUnitFieldsName(webSession, Constantes.DB.TableType.Type.dataVehicle , DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    }
                    else sql += WebFunctions.SQLGenerator.GetUnitFieldsName(webSession, Constantes.DB.TableType.Type.webPlan, DBConstantes.Tables.DASH_BOARD_PREFIXE);

                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    if (IsRepartitionSelected(webSession)) {
                        sql += WebFunctions.SQLGenerator.GetUnitFieldNameSumWithAlias(webSession, Constantes.DB.TableType.Type.dataVehicle);

                    }
                    else sql += WebFunctions.SQLGenerator.GetUnitFieldNameSumWithAlias(webSession, Constantes.DB.TableType.Type.webPlan);
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:

                    if (IsRepartitionSelected(webSession)) {
                        sql += WebFunctions.SQLGenerator.GetUnitFieldNameSumWithAlias(webSession, Constantes.DB.TableType.Type.dataVehicle);
                    }
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                    if (IsRepartitionSelected(webSession)) {
                        sql += WebFunctions.SQLGenerator.GetUnitFieldsName(webSession, Constantes.DB.TableType.Type.dataVehicle, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    }
                    break;
                default:
                    throw new DashBoardDataAccessException(" getUnitSelectFields(WebSession webSession) --> Impossible d'identifier le tableau de bord � traiter.");
            }
            return sql;
        }
        #endregion

        #region select repartition
        /// <summary>
        /// chaine caractere repartition
        /// </summary>	
        /// <param name="webSession">Session du client</param>		
        /// <returns></returns>
        private static string GetRepartitionField(WebSession webSession) {
            //identification du M�dia  s�lectionn�
			ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);

            string repartition = "repartition";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                    if (webSession.NamedDay != CstWeb.Repartition.namedDay.Total || webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total)
                        repartition = DBConstantes.DashBoard.Fields.ID_FORMAT_REPARTITION + " as repartition"; ;
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                    if ((webSession.Format != CstWeb.Repartition.Format.Total) || (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total))
                        repartition = DBConstantes.DashBoard.Fields.ID_DAY + " as repartition";
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                    if (webSession.Format != CstWeb.Repartition.Format.Total || webSession.NamedDay != CstWeb.Repartition.namedDay.Total) {
                        if (ClassificationCst.DB.Vehicles.names.radio == vehicleType)
                            repartition = DBConstantes.DashBoard.Fields.RADIO_TIME_SLICE + " as repartition";
                        else if (ClassificationCst.DB.Vehicles.names.tv == vehicleType || ClassificationCst.DB.Vehicles.names.others == vehicleType)
                            repartition = DBConstantes.DashBoard.Fields.TV_TIME_SLICE + " as repartition"; ;

                    }
                    break;
            }
            return repartition;
        }
        #endregion

        #endregion

        #region  From
        /// <summary>
        /// Clause from de la requete
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="year">suffixe ann�e de la table de donn�e</param>
        /// <returns>Cha�ne de caract�re correspondant � la clause from de la requete</returns>
        private static string GetFromClause(WebSession webSession, string year) {
            return " From " + GetTablesName(webSession, year);
        }

        #endregion

        #region  Jointures
        /// <summary>
        ///Jointures avec les tables de donn�es
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>Cha�ne de caract�re correspondant aux jointures</returns>
        private static string GetJointClause(WebSession webSession) {
            string sql = "";
            //identification du M�dia  s�lectionn�
            //			string vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
            //			bool premier =true;
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                    return GetMediaJointClause(webSession);

                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                    return GetSectorJointClause(webSession);

                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    sql = GetMediaJointClause(webSession);
                    if (sql.Length > 0) sql += "  and ";
                    sql += GetSectorJointClause(webSession);
                    break;

                default:
                    return sql;
            }
            return sql;
        }

        /// <summary>
        ///Jointures avec les tables m�dia
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>Cha�ne de caract�re correspondant aux jointures</returns>
        private static string GetMediaJointClause(WebSession webSession) {
            string sql = "";
            bool premier = true;
            if (!IsRepartitionSelected(webSession)) {
                sql += "   " + DBConstantes.Tables.VEHICLE_PREFIXE + ".id_vehicle=" + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_vehicle"
                    + "   and " + DBConstantes.Tables.VEHICLE_PREFIXE + ".id_language=" + webSession.DataLanguage
                    + "  and " + DBConstantes.Tables.VEHICLE_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
                premier = false;
            }
            if (!premier) sql += " and ";
            if (webSession.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia) {
                sql += DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".id_interest_center=" + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_interest_center  ";
                sql += " and  " + DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".id_language=" + webSession.DataLanguage
                    + "   and  " + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_media=" + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media"
                    + "   and  " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage + "  "
                    + " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            }
            else if (webSession.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter) {
                sql += DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".id_interest_center=" + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_interest_center  "
                    + "   and  " + DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".id_language=" + webSession.DataLanguage
                    + "  and " + DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            }
            else if (webSession.PreformatedMediaDetail == CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia) {
                sql += DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_media=" + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media"
                    + "   and  " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage + "  "
                    + "  and " + DBConstantes.Tables.MEDIA_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
            }
            return sql;
        }

        /// <summary>
        ///Jointures avec les tables de la branche produit
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>Cha�ne de caract�re correspondant aux jointures</returns>
        private static string GetSectorJointClause(WebSession webSession) {

            return "   " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_sector=" + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_sector"
                + "   and " + DBConstantes.Tables.SECTOR_PREFIXE + ".id_language=" + webSession.DataLanguage
                + "  and " + DBConstantes.Tables.SECTOR_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
        }
        #endregion

        #region  s�lection client
        /// <summary>
        /// Clause s�lection client
        /// </summary>
        /// <param name="webSession">s�ssion client</param>
        /// <param name="year">ann�e s�lectionn�e</param>
        /// <returns>Cha�ne de caract�re correspondant aux familles s�lectionn�es</returns>
        private static string GetSelectionClause(WebSession webSession, string year) {
            string sql = "";
            string selectionClause = GetSectorSelectionClause(webSession);

            //Familles s�lectionn�es
            if (selectionClause != null && selectionClause.Length > 0)
                sql += "  " + selectionClause;

            //M�dia s�lectionn�s
            selectionClause = GetMediaSelectionClause(webSession);
            if (selectionClause.Length > 0)
                sql += " " + selectionClause;
            //Dates s�lectionn�es
            selectionClause = GetDateSelectionClause(webSession, year);
            if (selectionClause.Length > 0)
                sql += " " + selectionClause;
            //R�partition s�lectionn�es
            selectionClause = GetRepartitionSelectionClause(webSession);
            if (selectionClause.Length > 0 && IsRepartitionSelected(webSession))
                sql += "  " + selectionClause;
            //Option Encart s�lectionn� : uniquement pour la presse
            if (webSession.CurrentModule == WebModule.Name.TABLEAU_DE_BORD_PRESSE)
                sql += "  " + WebFunctions.SQLGenerator.GetJointForInsertDetail(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
            return sql;
        }

        /// <summary>
        /// Familles s�lectionn�es
        /// </summary>
        /// <param name="webSession">s�ssion client</param>	
        /// <returns>Cha�ne de caract�re correspondant aux familles s�lectionn�es</returns>
        private static string GetSectorSelectionClause(WebSession webSession) {
            string sql = "";
            //string listSector="";
            #region ancienne version
            ////liste des familles s�lectionn�es
            //if(IsDetailSector(webSession)){
            //    //D�tail d'une famille
            //     listSector = webSession.GetSelection(webSession.SelectionUniversProduct,CustomerRightConstante.type.sectorAccess);
            //    if(WebFunctions.CheckedText.IsStringEmpty(listSector)){
            //        sql+=DBConstantes.Tables.DASH_BOARD_PREFIXE+".id_sector in ("+listSector+")";	
            //    }	
            //}else{
            //    //Toutes les familles s�lectionn�es
            //     listSector = webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorAccess);
            //    if(WebFunctions.CheckedText.IsStringEmpty(listSector)){
            //        sql+=DBConstantes.Tables.DASH_BOARD_PREFIXE+".id_sector in ("+listSector+")";	
            //    }	
            //}	
            #endregion
            if (webSession.SecondaryProductUniverses != null && webSession.SecondaryProductUniverses.Count > 0)
                sql = webSession.SecondaryProductUniverses[0].GetSqlConditions(DBConstantes.Tables.DASH_BOARD_PREFIXE, false);
            else if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                sql = webSession.PrincipalProductUniverses[0].GetSqlConditions(DBConstantes.Tables.DASH_BOARD_PREFIXE, false);

            return sql;
        }

        /// <summary>
        /// M�dia s�lectionn�es
        /// </summary>
        /// <param name="webSession">s�ssion client</param>	
        /// <returns>Cha�ne de caract�re correspondant aux familles s�lectionn�es</returns>
        private static string GetMediaSelectionClause(WebSession webSession) {
            bool premier = true;
            string MediaAccessList = "";
            string VehicleAccessList = "";
            string InterestCenterAccessList = "";
            //identification du M�dia  s�lectionn�          
			ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);

            if (IsInterestCenterSelected(webSession)) {
                InterestCenterAccessList = webSession.GetSelection(webSession.CurrentUniversMedia, CustomerRightConstante.type.interestCenterAccess);
            }
            else {
                MediaAccessList = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.mediaAccess);
                VehicleAccessList = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess);
                InterestCenterAccessList = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.interestCenterAccess);
            }
            string sql = "";
            if (WebFunctions.CheckedText.IsStringEmpty(InterestCenterAccessList)) {
                sql += " and ( " + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_interest_center in (" + InterestCenterAccessList + ")";
                premier = false;
            }
            if (WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)) {
                if (!premier) sql += " or ";
                else sql += " and ( ";
                sql += "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_media in (" + MediaAccessList + ")";
                premier = false;
            }
            if (!premier) sql += " )";

            //s�lection Pan euro
            if (!WebFunctions.CheckedText.IsStringEmpty(InterestCenterAccessList) && !WebFunctions.CheckedText.IsStringEmpty(MediaAccessList)
                && vehicleType == ClassificationCst.DB.Vehicles.names.others)
                sql += " and  " + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_category in (" + Media.GetItemsList(CstWeb.AdExpressUniverse.DASHBOARD_PANEURO_MEDIA_LIST_ID).CategoryList + ")";

            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    if (!IsRepartitionSelected(webSession) && VehicleAccessList.Length > 0)
                        sql = " and  " + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".id_vehicle in (" + VehicleAccessList + ") ";
                    break;
                default: return sql;

            }


            return sql;
        }
        /// <summary>
        /// Dates s�lectionn�es
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="year">ann�e s�lectionn�e</param>
        /// <returns>Cha�ne de caract�re correspondant aux dates s�lectionn�es</returns>
        private static string GetDateSelectionClause(WebSession webSession, string year) {
            string sql = "";
            int yearN1 = 0;
            yearN1 = int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)) - 1;

            if (IsCrossRepartitionType(webSession)) {
                if (!IsDetailPeriod(webSession)) {
                    sql += DateMediaNumSelection(webSession, webSession.PeriodBeginningDate, webSession.PeriodEndDate, year);
                }
                else {
                    sql += DateMediaNumSelection(webSession, webSession.DetailPeriodBeginningDate, webSession.DetailPeriodEndDate, year);
                }
            }
            else sql += GetMonthOrWeekMediaNum(webSession, yearN1.ToString());

            return sql;
        }

        /// <summary>
        /// Date mensuelle ou hebdomadaire s�lectionn�e
        /// </summary>
        /// <param name="webSession">session client</param>
        /// <param name="yearN1">ann�e pr�c�dente</param>
        /// <returns>chaine de caract�re Date s�lectionn�e</returns>
        private static string GetMonthOrWeekMediaNum(WebSession webSession, string yearN1) {

            AtomicPeriodWeek currentPeriod, beginPreviousPeriod, endPreviousPeriod;

            string sql = "";
            //Avec r�partition
            if (webSession.DetailPeriod == CstPeriodDetail.monthly) {
                //Mois s�lectionn�s
                if (!IsDetailPeriod(webSession)) {
                    sql += " and ((month_media_num between " + webSession.PeriodBeginningDate
                        + " and " + webSession.PeriodEndDate + ") ";
                    if (webSession.ComparativeStudy) {
                        sql += " or (month_media_num between " + yearN1 + webSession.PeriodBeginningDate.Substring(4, 2)
                            + " and  " + yearN1 + webSession.PeriodEndDate.Substring(4, 2) + ")";
                    }
                    sql += " ) ";
                }
                else {
                    sql += " and ((month_media_num between " + webSession.DetailPeriodBeginningDate
                        + " and " + webSession.DetailPeriodEndDate + ") ";
                    if (webSession.ComparativeStudy) {
                        sql += " or ( month_media_num between " + yearN1 + webSession.DetailPeriodBeginningDate.Substring(4, 2)
                            + " and  " + yearN1 + webSession.DetailPeriodEndDate.Substring(4, 2) + ")";
                    }
                    sql += " ) ";
                }
            }
            else if (webSession.DetailPeriod == CstPeriodDetail.weekly) {
                //semaines s�lectionn�es
                if (!IsDetailPeriod(webSession)) {
                    sql += " and ((week_media_num between " + webSession.PeriodBeginningDate
                        + " and " + webSession.PeriodEndDate + ") ";
                    if (webSession.ComparativeStudy) {
                        currentPeriod = new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(webSession.PeriodBeginningDate.Substring(4, 2)));
                        beginPreviousPeriod = currentPeriod;
                        beginPreviousPeriod.SubWeek(52);
                        currentPeriod = new AtomicPeriodWeek(int.Parse(webSession.PeriodEndDate.Substring(0, 4)), int.Parse(webSession.PeriodEndDate.Substring(4, 2)));
                        endPreviousPeriod = currentPeriod;
                        endPreviousPeriod.SubWeek(52);
                        sql += " or (week_media_num between " + beginPreviousPeriod.Year.ToString() + (beginPreviousPeriod.Week.ToString().Length > 1 ? beginPreviousPeriod.Week.ToString() : "0" + beginPreviousPeriod.Week.ToString())
                            + " and " + endPreviousPeriod.Year.ToString() + (endPreviousPeriod.Week.ToString().Length > 1 ? endPreviousPeriod.Week.ToString() : "0" + endPreviousPeriod.Week.ToString()) + ")";
                    }
                    sql += " ) ";
                }
                else {
                    sql += " and ((week_media_num between " + webSession.DetailPeriodBeginningDate
                        + " and " + webSession.DetailPeriodEndDate + ") ";
                    if (webSession.ComparativeStudy) {
                        currentPeriod = new AtomicPeriodWeek(int.Parse(webSession.DetailPeriodBeginningDate.Substring(0, 4)), int.Parse(webSession.DetailPeriodBeginningDate.Substring(4, 2)));
                        beginPreviousPeriod = currentPeriod;
                        beginPreviousPeriod.SubWeek(52);
                        currentPeriod = new AtomicPeriodWeek(int.Parse(webSession.DetailPeriodEndDate.Substring(0, 4)), int.Parse(webSession.DetailPeriodEndDate.Substring(4, 2)));
                        endPreviousPeriod = currentPeriod;
                        endPreviousPeriod.SubWeek(52);

                        sql += " or (week_media_num between " + beginPreviousPeriod.Year.ToString() + (beginPreviousPeriod.Week.ToString().Length > 1 ? beginPreviousPeriod.Week.ToString() : "0" + beginPreviousPeriod.Week.ToString())
                            + " and " + endPreviousPeriod.Year.ToString() + (endPreviousPeriod.Week.ToString().Length > 1 ? endPreviousPeriod.Week.ToString() : "0" + endPreviousPeriod.Week.ToString()) + ")";
                    }
                    sql += " ) ";
                }
            }

            return sql;
        }
        /// <summary>
        /// Date s�lectionn�e
        /// </summary>
        /// <param name="webSession">session</param>
        /// <param name="wsPeriodBeginningDate">date de d�but de session</param>
        /// <param name="wsPeriodEndDate">date de fin de session</param>
        /// <param name="year">ann�e s�lectionn�e</param>
        /// <returns>chaine de caract�re Date s�lectionn�e</returns>
        private static string DateMediaNumSelection(WebSession webSession, string wsPeriodBeginningDate, string wsPeriodEndDate, string year) {
            string sql = "";
            AtomicPeriodWeek BeginningPeriod = new AtomicPeriodWeek(int.Parse(year), int.Parse(wsPeriodBeginningDate.Substring(4, 2)));
            AtomicPeriodWeek EndPeriod = new AtomicPeriodWeek(int.Parse(year), int.Parse(wsPeriodEndDate.Substring(4, 2)));
            if (webSession.DetailPeriod == CstPeriodDetail.weekly) {
                sql = " and (date_media_num between " + BeginningPeriod.FirstDay.Year.ToString() + (BeginningPeriod.FirstDay.Month.ToString().Length > 1 ? BeginningPeriod.FirstDay.Month.ToString() : "0" + BeginningPeriod.FirstDay.Month.ToString())
                    + (BeginningPeriod.FirstDay.Day.ToString().Length > 1 ? BeginningPeriod.FirstDay.Day.ToString() : "0" + BeginningPeriod.FirstDay.Day.ToString());
                sql += " and " + EndPeriod.LastDay.Year.ToString() + (EndPeriod.LastDay.Month.ToString().Length > 1 ? EndPeriod.LastDay.Month.ToString() : "0" + EndPeriod.LastDay.Month.ToString())
                    + (EndPeriod.LastDay.Day.ToString().Length > 1 ? EndPeriod.LastDay.Day.ToString() : "0" + EndPeriod.LastDay.Day.ToString());
                sql += ")";
            }
            else {
                sql = " and (date_media_num between " + year + wsPeriodBeginningDate.Substring(4, 2) + "01";
                sql += " and " + year + wsPeriodEndDate.Substring(4, 2) + "31";
                sql += ")";
            }
            return sql;
        }
        /// <summary>
        /// verifie si une periode doit �tre detaill�e
        /// </summary>
        /// <param name="webSession">client</param>
        /// <returns>vraie si la periode est s�lectionn�e et faux sinon</returns>
        private static bool IsDetailPeriod(WebSession webSession) {
            return !(webSession.DetailPeriodBeginningDate.Equals("") || webSession.DetailPeriodBeginningDate.Equals("0"));
        }

        /// <summary>
        /// verifie si une famille doit �tre detaill�e
        /// </summary>
        /// <param name="webSession">client</param>
        /// <returns>vraie si la famille est s�lectionn�e et faux sinon</returns>
        private static bool IsDetailSector(WebSession webSession) {
            return !(webSession.SelectionUniversProduct == null || webSession.SelectionUniversProduct.Nodes == null
                || webSession.SelectionUniversProduct.Nodes.Count == 0);
        }

        /// <summary>
        /// V�rifie qu'un centre d'int�r�t � �t� s�lectionn�
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>vrai si un centre d'int�r�t � �t� s�lectionn�</returns>
        private static bool IsInterestCenterSelected(WebSession webSession) {
            return !(webSession.CurrentUniversMedia == null || webSession.CurrentUniversMedia.Nodes == null
                || webSession.CurrentUniversMedia.Nodes.Count == 0);
        }

        /// <summary>
        /// R�partition s�lectionn�es
        /// </summary>
        /// <param name="webSession">session du client</param>	
        /// <returns>Cha�ne de caract�re correspondant aux R�partition s�lectionn�es</returns>
        private static string GetRepartitionSelectionClause(WebSession webSession) {
            string sql = "";
			//Int64 idVehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID;
			ClassificationCst.DB.Vehicles.names vehicletype = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    sql += Get_1_2_3_RepartitionSelectionClause(webSession, vehicletype);
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                    if (IsCrossRepartitionType(webSession))
                        sql += Get_4_to_12_RepartitionSelectionClause(webSession, vehicletype);
                    else
                        sql += "  and repartition_code=" + CstWeb.Repartition.repartitionCode.Format.GetHashCode();
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                    if (IsCrossRepartitionType(webSession))
                        sql += Get_4_to_12_RepartitionSelectionClause(webSession, vehicletype);
                    else
                        sql += "  and repartition_code=" + CstWeb.Repartition.repartitionCode.namedDay.GetHashCode();
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                    if (IsCrossRepartitionType(webSession))
                        sql += Get_4_to_12_RepartitionSelectionClause(webSession, vehicletype);
                    else
                        sql += "  and repartition_code=" + CstWeb.Repartition.repartitionCode.timeInterval.GetHashCode();
                    break;
                default: return sql;
            }
            return sql;
        }

        /// <summary>
        /// R�partition s�lectionn�es pour tableau 1,2,3 et 13
        /// </summary>
        /// <param name="webSession">session du client</param>	
        /// <param name="vehicletype">type de m�dia</param>
        /// <returns>Cha�ne de caract�re correspondant aux R�partition s�lectionn�es</returns>
        private static string Get_1_2_3_RepartitionSelectionClause(WebSession webSession, ClassificationCst.DB.Vehicles.names vehicletype) {
            string sql = "";
            if (webSession.Format != CstWeb.Repartition.Format.Total || webSession.NamedDay != CstWeb.Repartition.namedDay.Total ||
                webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total) {

                switch (webSession.PreformatedTable) {
                    case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                    case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                    case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                    case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                        //Si on croise au moins deux type de r�partition il faut attaquer les tables des 
                        //tableaux de bord d�taill�es par jours
                        if ((webSession.Format != CstWeb.Repartition.Format.Total && webSession.NamedDay != CstWeb.Repartition.namedDay.Total)
                            || (webSession.Format != CstWeb.Repartition.Format.Total && webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total)
                            || (webSession.NamedDay != CstWeb.Repartition.namedDay.Total && webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total)
                            ) {
                            //Format							
                            if (webSession.Format != CstWeb.Repartition.Format.Total)
                                sql += " AND " + DBConstantes.DashBoard.Fields.ID_FORMAT_REPARTITION + "=" + webSession.Format.GetHashCode().ToString();
                            //Jour Nomm�
                            if (webSession.NamedDay != CstWeb.Repartition.namedDay.Total) {
                                if (webSession.NamedDay == CstWeb.Repartition.namedDay.Week_5_day)
                                    sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + " in (" + CstWeb.Repartition.namedDay.Monday.GetHashCode().ToString()
                                        + "," + CstWeb.Repartition.namedDay.Tuesday.GetHashCode().ToString() + "," + CstWeb.Repartition.namedDay.Wednesdays.GetHashCode().ToString()
                                        + "," + CstWeb.Repartition.namedDay.Thursday.GetHashCode().ToString() + "," + CstWeb.Repartition.namedDay.Friday.GetHashCode().ToString()
                                        + ")";
                                else if (webSession.NamedDay == CstWeb.Repartition.namedDay.Week_end)
                                    sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + " in (" + CstWeb.Repartition.namedDay.Saturday.GetHashCode().ToString()
                                        + "," + CstWeb.Repartition.namedDay.Sunday.GetHashCode().ToString() + ")";
                                else
                                    sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + "=" + webSession.NamedDay.GetHashCode().ToString();
                            }
                            //Tranche horaire
                            if (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total) {
                                if (ClassificationCst.DB.Vehicles.names.radio == vehicletype) {
                                    sql += " AND " + DBConstantes.DashBoard.Fields.RADIO_TIME_SLICE + "=" + webSession.TimeInterval.GetHashCode().ToString();
                                }
                                else if (ClassificationCst.DB.Vehicles.names.tv == vehicletype || ClassificationCst.DB.Vehicles.names.others == vehicletype) {
                                    sql += " AND " + DBConstantes.DashBoard.Fields.TV_TIME_SLICE + "=" + webSession.TimeInterval.GetHashCode().ToString();
                                }
                            }

                        }
                        else {
                            //Sinon on attaque les tables de bord mensuels ou hebdomadaires

                            //Format							
                            if (webSession.Format != CstWeb.Repartition.Format.Total) {
                                sql += " AND  repartition_code=" + CstWeb.Repartition.repartitionCode.Format.GetHashCode().ToString();
                                sql += " AND repartition = " + webSession.Format.GetHashCode().ToString();
                            }

                            //Jour Nomm�						
                            if (webSession.NamedDay != CstWeb.Repartition.namedDay.Total) {
                                sql += "  AND repartition_code=" + CstWeb.Repartition.repartitionCode.namedDay.GetHashCode().ToString();
                                if (webSession.NamedDay == CstWeb.Repartition.namedDay.Week_5_day)
                                    sql += " AND repartition  in (" + CstWeb.Repartition.namedDay.Monday.GetHashCode().ToString()
                                        + "," + CstWeb.Repartition.namedDay.Tuesday.GetHashCode().ToString() + "," + CstWeb.Repartition.namedDay.Wednesdays.GetHashCode().ToString()
                                        + "," + CstWeb.Repartition.namedDay.Thursday.GetHashCode().ToString() + "," + CstWeb.Repartition.namedDay.Friday.GetHashCode().ToString()
                                        + ")";
                                else if (webSession.NamedDay == CstWeb.Repartition.namedDay.Week_end)
                                    sql += " AND repartition  in (" + CstWeb.Repartition.namedDay.Saturday.GetHashCode().ToString()
                                        + "," + CstWeb.Repartition.namedDay.Sunday.GetHashCode().ToString() + ")";
                                else
                                    sql += " AND repartition =" + webSession.NamedDay.GetHashCode().ToString();
                            }

                            //Tranche horaire
                            if (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total) {
                                sql += "  AND repartition_code=" + CstWeb.Repartition.repartitionCode.timeInterval.GetHashCode().ToString();
                                sql += " AND repartition = " + webSession.TimeInterval.GetHashCode().ToString();
                            }
                        }
                        break;

                }
            }

            return sql;
        }

        /// <summary>
        /// R�partition s�lectionn�es pour tableau 4 � 12
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="vehicletype">type de m�dia</param>
        /// <returns>Cha�ne de caract�re correspondant aux R�partition s�lectionn�es</returns>
        private static string Get_4_to_12_RepartitionSelectionClause(WebSession webSession, ClassificationCst.DB.Vehicles.names vehicletype) {
            string sql = "";
            switch (webSession.PreformatedTable) {

                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                    if ((webSession.NamedDay != CstWeb.Repartition.namedDay.Total) || (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total)) {
                        //Jour Nomm�
                        if (webSession.NamedDay != CstWeb.Repartition.namedDay.Total) {
                            if (webSession.NamedDay == CstWeb.Repartition.namedDay.Week_5_day)
                                sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + " in (" + CstWeb.Repartition.namedDay.Monday.GetHashCode().ToString()
                                    + "," + CstWeb.Repartition.namedDay.Tuesday.GetHashCode().ToString() + "," + CstWeb.Repartition.namedDay.Wednesdays.GetHashCode().ToString()
                                    + "," + CstWeb.Repartition.namedDay.Thursday.GetHashCode().ToString() + "," + CstWeb.Repartition.namedDay.Friday.GetHashCode().ToString()
                                    + ")";
                            else if (webSession.NamedDay == CstWeb.Repartition.namedDay.Week_end)
                                sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + " in (" + CstWeb.Repartition.namedDay.Saturday.GetHashCode().ToString()
                                    + "," + CstWeb.Repartition.namedDay.Sunday.GetHashCode().ToString() + ")";
                            else
                                sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + "=" + webSession.NamedDay.GetHashCode().ToString();
                        }
                        //Tranche horaire
                        if (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total) {
                            if (ClassificationCst.DB.Vehicles.names.radio == vehicletype) {
                                sql += " AND " + DBConstantes.DashBoard.Fields.RADIO_TIME_SLICE + "=" + webSession.TimeInterval.GetHashCode().ToString();
                            }
                            else if (ClassificationCst.DB.Vehicles.names.tv == vehicletype || ClassificationCst.DB.Vehicles.names.others == vehicletype) {
                                sql += " AND " + DBConstantes.DashBoard.Fields.TV_TIME_SLICE + "=" + webSession.TimeInterval.GetHashCode().ToString();
                            }
                        }
                    }
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                    if ((webSession.Format != CstWeb.Repartition.Format.Total) || (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total)) {
                        //Format							
                        if (webSession.Format != CstWeb.Repartition.Format.Total)
                            sql += " AND " + DBConstantes.DashBoard.Fields.ID_FORMAT_REPARTITION + "=" + webSession.Format.GetHashCode().ToString();
                        //Tranche horaire
                        if (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total) {
                            if (ClassificationCst.DB.Vehicles.names.radio == vehicletype) {
                                sql += " AND " + DBConstantes.DashBoard.Fields.RADIO_TIME_SLICE + "=" + webSession.TimeInterval.GetHashCode().ToString();
                            }
                            else if (ClassificationCst.DB.Vehicles.names.tv == vehicletype || ClassificationCst.DB.Vehicles.names.others == vehicletype) {
                                sql += " AND " + DBConstantes.DashBoard.Fields.TV_TIME_SLICE + "=" + webSession.TimeInterval.GetHashCode().ToString();
                            }
                        }
                    }
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                    if ((webSession.Format != CstWeb.Repartition.Format.Total) || (webSession.NamedDay != CstWeb.Repartition.namedDay.Total)) {
                        //Format							
                        if (webSession.Format != CstWeb.Repartition.Format.Total)
                            sql += " AND " + DBConstantes.DashBoard.Fields.ID_FORMAT_REPARTITION + "=" + webSession.Format.GetHashCode().ToString();
                        //Jour Nomm�
                        if (webSession.NamedDay != CstWeb.Repartition.namedDay.Total) {
                            if (webSession.NamedDay == CstWeb.Repartition.namedDay.Week_5_day)
                                sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + " in (" + CstWeb.Repartition.namedDay.Monday.GetHashCode().ToString()
                                    + "," + CstWeb.Repartition.namedDay.Tuesday.GetHashCode().ToString() + "," + CstWeb.Repartition.namedDay.Wednesdays.GetHashCode().ToString()
                                    + "," + CstWeb.Repartition.namedDay.Thursday.GetHashCode().ToString() + "," + CstWeb.Repartition.namedDay.Friday.GetHashCode().ToString()
                                    + ")";
                            else if (webSession.NamedDay == CstWeb.Repartition.namedDay.Week_end)
                                sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + " in (" + CstWeb.Repartition.namedDay.Saturday.GetHashCode().ToString()
                                    + "," + CstWeb.Repartition.namedDay.Sunday.GetHashCode().ToString() + ")";
                            else
                                sql += " AND " + DBConstantes.DashBoard.Fields.ID_DAY + "=" + webSession.NamedDay.GetHashCode().ToString();
                        }
                    }
                    break;
                default: return sql;
            }
            return sql;
        }
        #endregion

        #region Droits client
        /// <summary>
        /// Droits produits et m�dia du client pour Tableau de bord
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>chaine  de caract�re des droits client</returns>
        private static string GetCustomerRight(WebSession webSession) {
            string sql = "";
            if (IsRepartitionSelected(webSession))
                sql += "  " + WebFunctions.SQLGenerator.getClassificationCustomerMediaRight(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE, DBConstantes.Tables.DASH_BOARD_PREFIXE, true);
            else
                sql += "  " + WebFunctions.SQLGenerator.getClassificationCustomerRecapMediaRight(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE, DBConstantes.Tables.DASH_BOARD_PREFIXE, DBConstantes.Tables.DASH_BOARD_PREFIXE, true);
            return sql;
        }
        #endregion

        #region Regroupement
        /// <summary>
        /// Regroupement des champs des r�sultats  par champs
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>chaine de caract�re correspondant au regroupement des donn�es</returns>
        private static string GetGroupBy(WebSession webSession) {
            string sql = "";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                    //M�dia
                    sql += "  " + GetMediaGroupBy(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                    //familles
                    sql += "  " + GetSectorGroupBy(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);

                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    //M�dia et familles
                    sql = "  " + GetMediaGroupBy(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    if (sql.Length > 0 && GetSectorGroupBy(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE).Length > 0) sql += ",";
                    sql += "  " + GetSectorGroupBy(webSession, DBConstantes.Tables.DASH_BOARD_PREFIXE);
                    break;
                default:
                    break;
            }
            //dates
            if (!IsRepartitionSelected(webSession)) {
                if (sql.Length > 0 && GetDateGroupBy(webSession).Length > 0) sql += "," + GetDateGroupBy(webSession);
                else if (sql.Length == 0 && GetDateGroupBy(webSession).Length > 0) sql += " " + GetDateGroupBy(webSession);
            }
            //repartition
            if (sql.Length > 0 && GetRepartitionGroupBy(webSession).Length > 0) sql += "," + GetRepartitionGroupBy(webSession);
            else if (sql.Length == 0 && GetRepartitionGroupBy(webSession).Length > 0) sql = "  " + GetRepartitionGroupBy(webSession);

            //dates pour r�partition
            if (IsRepartitionSelected(webSession)) {
                if (sql.Length > 0 && GetDateGroupBy(webSession).Length > 0) sql += "," + GetDateGroupBy(webSession);
                else if (sql.Length == 0 && GetDateGroupBy(webSession).Length > 0) sql += " " + GetDateGroupBy(webSession);
            }
            return sql;

        }

        /// <summary>
        /// Regroupement des m�dia s�lectionn�s
        /// </summary>
        /// <param name="webSession">sessiuon du client</param>
        /// <returns>Cha�ne de caract�re correspondant aux regroupement de m�dia</returns>
        private static string GetMediaGroupBy(WebSession webSession) {
            return GetMediaGroupBy(webSession, "");
        }

        /// <summary>
        /// Regroupement des m�dia s�lectionn�s
        /// </summary>
        /// <param name="webSession">sessiuon du client</param>
        /// <param name="prefixe">prefixe table de donn�es</param>
        /// <returns>Cha�ne de caract�re correspondant aux regroupement de m�dia</returns>
        private static string GetMediaGroupBy(WebSession webSession, string prefixe) {
            string sql = "  ";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    sql += GetMediaSelectFields(webSession, prefixe);
                    break;
                default: return "";
            }
            return sql;
        }

        /// <summary>
        /// Regroupement des familles s�lectionn�es
        /// </summary>
        /// <param name="webSession">sessiuon du client</param>
        /// <returns>Cha�ne de caract�re correspondant aux regroupement de familles</returns>
        private static string GetSectorGroupBy(WebSession webSession) {
            return GetSectorGroupBy(webSession, "");
        }

        /// <summary>
        /// Regroupement des familles s�lectionn�es
        /// </summary>
        /// <param name="webSession">sessiuon du client</param>
        /// <param name="prefixe">prefixe table de donn�es</param>
        /// <returns>Cha�ne de caract�re correspondant aux regroupement de familles</returns>
        private static string GetSectorGroupBy(WebSession webSession, string prefixe) {


            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    if (prefixe.Length > 0) return " " + prefixe + ".id_sector," + DBConstantes.Tables.SECTOR_PREFIXE + ".sector";
                    else return " id_sector,sector";
                default: return "";
            }
        }
        /// <summary>
        /// Regroupement des dates
        /// </summary>
        /// <param name="webSession">sesion du client</param>
        /// <returns>chiane caract�re dates � regroup�s</returns>
        private static string GetDateGroupBy(WebSession webSession) {
            string sql = "";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:

                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:

                    if (IsCrossRepartitionType(webSession))
                        sql += "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".date_media_num";
                    else {
                        if (webSession.DetailPeriod == CstPeriodDetail.monthly)
                            sql += "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".month_media_num";
                        else if (webSession.DetailPeriod == CstPeriodDetail.weekly)
                            sql += "  " + DBConstantes.Tables.DASH_BOARD_PREFIXE + ".week_media_num";
                    }
                    break;
                default: return "";
            }
            return sql;
        }
        /// <summary>
        /// Regroupement des r�partitions
        /// </summary>
        /// <param name="webSession">session du client</param>	
        /// <returns>Chaine de caract�re repartition</returns>
        private static string GetRepartitionGroupBy(WebSession webSession) {
            //identification du M�dia  s�lectionn�
            string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
            ClassificationCst.DB.Vehicles.names vehicleType = (ClassificationCst.DB.Vehicles.names)int.Parse(Vehicle);

            switch (webSession.PreformatedTable) {

                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                    if (IsCrossRepartitionType(webSession))
                        return DBConstantes.DashBoard.Fields.ID_DAY;
                    else
                        return " repartition ";
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                    if (IsCrossRepartitionType(webSession)) {
                        if (ClassificationCst.DB.Vehicles.names.radio == vehicleType) {
                            return DBConstantes.DashBoard.Fields.RADIO_TIME_SLICE;
                        }
                        else if (ClassificationCst.DB.Vehicles.names.tv == vehicleType || ClassificationCst.DB.Vehicles.names.others == vehicleType) {
                            return DBConstantes.DashBoard.Fields.TV_TIME_SLICE;
                        }
                        else throw (new DashBoardDataAccessException("getRepartitionGroupBy(WebSession webSession) : Impossible de d�terminer le m�dia � traiter."));
                    }
                    else return " repartition ";
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                    if (IsCrossRepartitionType(webSession))
                        return DBConstantes.DashBoard.Fields.ID_FORMAT_REPARTITION;
                    else
                        return " repartition ";
                default: return "";
            }
        }
        #endregion

        #region Tri
        /// <summary>
        /// ordre des champs de la requete
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="order">ordre</param>
        /// <param name="hasUnionAll">vrai si clause <code>union all</code></param>
        /// <returns>ordre champs</returns>
        public static string GetOrderClause(WebSession webSession, string order, bool hasUnionAll) {
            string sql = "";
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                    if (webSession.DetailPeriod == CstPeriodDetail.monthly)
                        sql += "  Order by  " + GetMediaOrderFields(webSession, hasUnionAll, "") + ","
                            + (!hasUnionAll ? DBConstantes.Tables.DASH_BOARD_PREFIXE + ".month_media_num" : " period ") + " " + order;
                    else if (webSession.DetailPeriod == CstPeriodDetail.weekly)
                        sql += "  Order by  " + GetMediaOrderFields(webSession, hasUnionAll, "") + ","
                            + (!hasUnionAll ? DBConstantes.Tables.DASH_BOARD_PREFIXE + ".week_media_num " : " period ") + "  " + order;
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                    if (webSession.DetailPeriod == CstPeriodDetail.monthly)
                        sql += "  Order by  " + GetSectorOrderFields(webSession, hasUnionAll) + ","
                            + (!hasUnionAll ? DBConstantes.Tables.DASH_BOARD_PREFIXE + ".month_media_num" : " period ") + " " + order;
                    else if (webSession.DetailPeriod == CstPeriodDetail.weekly)
                        sql += "  Order by " + GetSectorOrderFields(webSession, hasUnionAll) + ","
                            + (!hasUnionAll ? DBConstantes.Tables.DASH_BOARD_PREFIXE + ".week_media_num " : " period ") + "  " + order;
                    break;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector:
                    if (webSession.DetailPeriod == CstPeriodDetail.monthly) {
                        sql += "  Order by  " + GetMediaOrderFields(webSession, hasUnionAll, "") + "," + GetSectorOrderFields(webSession, hasUnionAll) + ","
                            + (!hasUnionAll ? DBConstantes.Tables.DASH_BOARD_PREFIXE + ".month_media_num" : " period ") + " " + order;
                    }
                    else if (webSession.DetailPeriod == CstPeriodDetail.weekly) {
                        sql += "  Order by  " + GetMediaOrderFields(webSession, hasUnionAll, "") + "," + GetSectorOrderFields(webSession, hasUnionAll) + ","
                            + (!hasUnionAll ? DBConstantes.Tables.DASH_BOARD_PREFIXE + ".week_media_num " : " period ") + "  " + order;
                    }
                    break;
            }
            return sql;
        }

        #region Tri des Media
        /// <summary>
        /// chaine de caract�re clause de tri des m�dia
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="hasUnionAll">vrai si clause <code>union all</code></param>
        /// <param name="prefixe">Prefixe table de donn�es</param>
        /// <returns>Cha�ne de caract�re de m�dia � s�lectionner</returns>
        private static string GetMediaOrderFields(WebSession webSession, bool hasUnionAll, string prefixe) {
            switch (webSession.PreformatedMediaDetail) {
                // selection du niveau media/support
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
                    if (prefixe != null && prefixe.Length > 0)
                        return "  " + (!hasUnionAll ? DBConstantes.Tables.MEDIA_PREFIXE + ".media ," + prefixe + ".id_media" : " media ," + prefixe + ".id_media");
                    else return "  " + (!hasUnionAll ? DBConstantes.Tables.MEDIA_PREFIXE + ".media ,id_media" : " media ,id_media");
                // selection du niveau media/centre d'interet
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
                    if (prefixe != null && prefixe.Length > 0)
                        return "  " + (!hasUnionAll ? DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".interest_center," + prefixe + ".id_interest_center" : "interest_center," + prefixe + ".id_interest_center");
                    else return "  " + (!hasUnionAll ? DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".interest_center,id_interest_center" : "interest_center,id_interest_center");
                // selection du niveau media/centre d'interet/support
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
                    if (prefixe != null && prefixe.Length > 0)
                        return "  " + (!hasUnionAll ? DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".interest_center," + prefixe + ".id_interest_center," : "interest_center," + prefixe + ".id_interest_center,")
                            + (!hasUnionAll ? DBConstantes.Tables.MEDIA_PREFIXE + ".media ," + prefixe + ".id_media" : " media ," + prefixe + ".id_media");
                    else return "  " + (!hasUnionAll ? DBConstantes.Tables.INTEREST_CENTER_PREFIXE + ".interest_center,id_interest_center," : " interest_center,id_interest_center,")
                             + (!hasUnionAll ? DBConstantes.Tables.MEDIA_PREFIXE + ".media ,id_media" : " media ,id_media");
                default: return "";
            }
        }
        #endregion

        #region  Tri des familles
        /// <summary>
        /// chaine de caract�re clause Tri" de(s) famille(s)
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="hasUnionAll">vrai si clause <code>union all</code></param>
        /// <returns>Cha�ne de caract�re de familles � s�lectionner</returns>
        private static string GetSectorOrderFields(WebSession webSession, bool hasUnionAll) {
            return (!hasUnionAll ? DBConstantes.Tables.SECTOR_PREFIXE + ".sector" : " sector ");
        }
        #endregion

        #endregion

        #region R�partition
        /// <summary>
        ///Indique si une r�partition a �t� s�lectionn�e
        /// </summary>
        /// <param name="webSession">session</param>
        /// <returns>vrai si r�partition s�lectionn�</returns>
        private static bool IsRepartitionSelected(WebSession webSession) {
            //identification du M�dia  s�lectionn�			
            ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            bool isRepartition = false;
            if (ClassificationCst.DB.Vehicles.names.press != vehicleType) {
                isRepartition = !(webSession.Format == CstWeb.Repartition.Format.Total
                    && webSession.NamedDay == CstWeb.Repartition.namedDay.Total
                    && webSession.TimeInterval == CstWeb.Repartition.timeInterval.Total
                    );
                if (!isRepartition) {
                    switch (webSession.PreformatedTable) {
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                            isRepartition = true;
                            break;
                    }
                }
            }
            return isRepartition;
        }

        /// <summary>
        /// V�rifie si l'utilisateur � s�lectionn� au moins
        /// 2 types de r�patition (soit format,jour nomm�,tranche horaire)
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>vrai si au moins 2 types de r�patition s�lectionn�s</returns>
        public static bool IsCrossRepartitionType(WebSession webSession) {
            if ((webSession.Format != CstWeb.Repartition.Format.Total && webSession.NamedDay != CstWeb.Repartition.namedDay.Total)
                || (webSession.Format != CstWeb.Repartition.Format.Total && webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total)
                || (webSession.NamedDay != CstWeb.Repartition.namedDay.Total && webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total)
                )
                return true;
            switch (webSession.PreformatedTable) {
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format:
                    if ((webSession.NamedDay != CstWeb.Repartition.namedDay.Total) || (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total))
                        return true;
                    else return false;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay:
                    if ((webSession.Format != CstWeb.Repartition.Format.Total) || (webSession.TimeInterval != CstWeb.Repartition.timeInterval.Total))
                        return true;
                    else return false;
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:
                case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice:
                    if ((webSession.Format != CstWeb.Repartition.Format.Total) || (webSession.NamedDay != CstWeb.Repartition.namedDay.Total))
                        return true;
                    else return false;
            }

            return false;

        }

        #endregion

        #endregion

    }
}