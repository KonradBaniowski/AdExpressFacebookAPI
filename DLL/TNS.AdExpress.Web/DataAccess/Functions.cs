#region Information
/*
 * auteur : Guillaume Facon
 * créé le :
 * modifié le : 
 *		30/10/2004 par G. RAGNEAU
 *		27/06/2005 par G. RAGNEAU : GetWebPlanTable
 *		12/12/2005 par D. V. Mussuma : fonction de tri des requêtes spot à spot
 * */
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using System.Windows.Forms;
using System.Text;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ConstantesFrameWork = TNS.AdExpress.Constantes.FrameWork;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.DataAccess{
	/// <summary>
	/// Fonctions liées à la base de données
	/// </summary>
	public class Functions{

		#region Ferme et dispose la connexion à la base de données
		/// <summary>
		/// Ferme et dispose la connexion à la base de données
		/// </summary>
		/// <param name="webSession"></param>
        //public static void closeDataBase(WebSession webSession){
        //    if(webSession.CustomerLogin.Connection!=null){
        //        if(webSession.CustomerLogin.Connection.State==System.Data.ConnectionState.Open)webSession.CustomerLogin.Connection.Close();
        //        webSession.CustomerLogin.Connection.Dispose();
        //        webSession.CustomerLogin.Connection=null;
        //    }
        //}
		#endregion

		#region Vérification de la dernière date disponible pour un media dans la base de données des recap
		/// <summary>
		/// Vérification de la dernière date disponible pour un media dans la base de données des recap
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Dernier mois disponible</returns>
		public static string CheckAvailableDateForMedia(Int64 idVehicle, WebSession webSession){

			string lastAvailableDate = (DateTime.Now.Year-2).ToString();

			string sql = "";

			DataSet ds;

			#region Construction de la requete
			sql = "select ";
			// Cas ou l'année en cours est différente de la dernière année chargée
			if(DateTime.Now.Year>webSession.DownLoadDate){
				for(int i = 3; i >0; i--){
					for(int j = 1; j <= 12; j++){
						sql += " max(exp_euro_" + ((i-1!=0)?"N"+(i-1):"N") + "_" + j + ") as N"
							+ (DateTime.Now.Year-i) + j.ToString("0#") + ",";
					}
				}			
			}
			else{
				for(int i = 2; i >=0; i--){
					for(int j = 1; j <= 12; j++){
						sql += " max(exp_euro_" + ((i!=0)?"N"+i:"N") + "_" + j + ") as N"
							+ (DateTime.Now.Year-i) + j.ToString("0#") + ",";
					}
				}
			}

			sql = sql.Remove(sql.Length-1, 1);

			sql += " from " + WebFunctions.SQLGenerator.getVehicleTableNameForSectorAnalysisResult(VehiclesInformation.DatabaseIdToEnum(idVehicle));
			#endregion

			#region Exécution de la requete
            IDataSource source=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
			ds = source.Fill(sql);

			#endregion

			#region Extraction du dernier mois disponible
			
			for(int i = ds.Tables[0].Columns.Count-1; i >= 0; i--){
				if (ds.Tables[0].Rows[0][i].ToString() != "0"){
					lastAvailableDate = ds.Tables[0].Columns[i].ColumnName.Remove(0,1);
					break;
				}
			}
			#endregion

			return lastAvailableDate;

		}
		#endregion

		#region GetVehiculeTableName
		/// <summary>
		/// Méthode privée qui détecte la table de recap à utiliser en fonction de la sélection média, produit
		/// et du niveau de détail choisi
		///		détection d'une étude monomédia ou pluri média ==> recap_tv ... ou recap_pluri
		///	On ne considère pas le cas des tablme agrégées
		/// </summary>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.DynamicTablesDataAccessException">
		/// Lancée si aucune table de la base de doonées ne correspond au vehicle spécifié dans la session utilisateur.
		/// </exception>		
		/// <returns>Chaîne de caractère correspondant au nom de la table à attaquer</returns>
		public static string getVehicleTableName(Int64 idVehicle){

			switch((ClassificationConstantes.DB.Vehicles.names) idVehicle){
				case ClassificationConstantes.DB.Vehicles.names.cinema:
					return "recap_cinema";
				case ClassificationConstantes.DB.Vehicles.names.internet:
					return "recap_internet";
				case ClassificationConstantes.DB.Vehicles.names.outdoor:
					return "recap_outdoor";
				case ClassificationConstantes.DB.Vehicles.names.radio:
					return "recap_radio";
				case ClassificationConstantes.DB.Vehicles.names.tv:
					return "recap_tv";
				case ClassificationConstantes.DB.Vehicles.names.press:
					return "recap_press";
				case ClassificationConstantes.DB.Vehicles.names.plurimedia:
					return "recap_pluri";
				case ClassificationConstantes.DB.Vehicles.names.mediasTactics:
					return "recap_tactic";
				case ClassificationConstantes.DB.Vehicles.names.mobileTelephony:
					return "recap_message";
				case ClassificationConstantes.DB.Vehicles.names.emailing:
					return "recap_emailing";
				case ClassificationConstantes.DB.Vehicles.names.internationalPress:
				case ClassificationConstantes.DB.Vehicles.names.others:
				case ClassificationConstantes.DB.Vehicles.names.adnettrack:
				default:
					throw new WebExceptions.DynamicTablesDataAccessException("Le vehicle n° " + idVehicle + " n'est pas traité.");
			}

		}
		#endregion

		#region GetWebPlanTable
		/// <summary>
		/// Determine the correct table to use depending on the type of^period in the web session
		/// </summary>
		/// <param name="webSession">User Session</param>
		/// <returns>Table name to use</returns>
		public static string GetWebPlanTable(WebSession webSession){
			switch(webSession.PeriodType){
				case WebConstantes.CustomerSessions.Period.Type.dateToDateWeek:
				case WebConstantes.CustomerSessions.Period.Type.LastLoadedWeek:
				case WebConstantes.CustomerSessions.Period.Type.nLastWeek:
				case WebConstantes.CustomerSessions.Period.Type.previousWeek:
					return DBConstantes.Tables.WEB_PLAN_MEDIA_WEEK;
				default :
					return DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
			}
		}
		#endregion

		#region GetAPPMWebPlanTable
		/// <summary>
		/// Determine the correct table for APPM module to be used depending on the type of period in the web session
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <returns>Table name to use</returns>
		public static string GetAPPMWebPlanTable(WebSession webSession) {
			switch(webSession.PeriodType) {
				case WebConstantes.CustomerSessions.Period.Type.dateToDateWeek:
				case WebConstantes.CustomerSessions.Period.Type.LastLoadedWeek:
				case WebConstantes.CustomerSessions.Period.Type.nLastWeek:
				case WebConstantes.CustomerSessions.Period.Type.previousWeek:
					return DBConstantes.Tables.WEB_PLAN_APPM_WEEK;
				default :
					return DBConstantes.Tables.WEB_PLAN_APPM_MONTH;
			}
		}
		#endregion

		#region GetDateFieldWebPlanTable
		/// <summary>
		/// Determine the correct date field to use depending on the type of period in the web session
		/// </summary>
		/// <param name="webSession">User Session</param>
		/// <returns>Field name to use</returns>
		public static string GetDateFieldWebPlanTable(WebSession webSession) {
			switch(webSession.PeriodType){
				case WebConstantes.CustomerSessions.Period.Type.dateToDateWeek:
				case WebConstantes.CustomerSessions.Period.Type.LastLoadedWeek:
				case WebConstantes.CustomerSessions.Period.Type.nLastWeek:
				case WebConstantes.CustomerSessions.Period.Type.previousWeek:
					return "week_media_num";
				default :
					return "month_media_num";
			}
		}
		#endregion

		#region Exécution de la requête
		/// <summary>
		/// Exécution de la requête
		/// </summary>
		/// <param name="sql">sql</param>	
		/// <param name="ConnectionString">chaîne de connection</param>
		/// <returns>DataSet</returns>
		public static DataSet ExecuteQuery(string sql,string ConnectionString){
			return ExecuteQuery(sql,new OracleConnection(ConnectionString));
		}

		/// <summary>
		/// Exécution de la requête
		/// </summary>
		/// <param name="sql">sql</param>
		/// <param name="connection">objet oracle connection</param>
		/// <returns>DataSet</returns>
		public static DataSet ExecuteQuery(string sql, OracleConnection connection){


			DataSet ds=new DataSet();
			OracleCommand sqlCommand=null;

			OracleDataAdapter sqlAdapter=null;

			#region Ouverture de la base de données
			bool DBToClosed=false;
			// On teste si la base est déjà ouverte
			if (connection.State==System.Data.ConnectionState.Closed) {
				DBToClosed=true;
				try {
					connection.Open();
				}
				catch(System.Exception et) {
					throw(new WebExceptions.RecapAdvertiserDataAccessException("Impossible d'ouvrir la base de données:"+et.Message));
				}
			}
			#endregion

			#region Execution
			try {
				sqlCommand=new OracleCommand(sql,connection);
				sqlAdapter=new OracleDataAdapter(sqlCommand);
				sqlAdapter.Fill(ds);
			}
				#endregion

				#region Traitement d'erreur du chargement des données
			catch(System.Exception ex) {
				try {
					// Fermeture de la base de données
					if (sqlAdapter!=null) {
						sqlAdapter.Dispose();
					}
					if(sqlCommand!=null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et) {
					throw(new WebExceptions.RecapAdvertiserDataAccessException ("Impossible de fermer la base de données, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
				}
				throw(new WebExceptions.RecapAdvertiserDataAccessException ("Impossible de charger les données:"+sql+" "+ex.Message));
			}
			#endregion

			#region Fermeture de la base de données
			try {
				// Fermeture de la base de données
				if (sqlAdapter!=null) {
					sqlAdapter.Dispose();
				}
				if(sqlCommand!=null)sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
			}
			catch(System.Exception et) {
				throw(new WebExceptions.RecapAdvertiserDataAccessException ("Impossible de fermer la base de données :"+et.Message));
			}
			#endregion

			return ds;
		}
		#endregion

		#region SelectDistinct
		/// <summary>
		/// Select the distinct values from a column
		/// </summary>
		/// <param name="TableName">Name of the new table</param>
		/// <param name="SourceTable">DataTable Source</param>
		/// <param name="FieldName">Field Name</param>
		/// <returns>DataTable with distinc value from SourceTable.FieldName</returns>
		public static DataTable SelectDistinct(string TableName, DataTable SourceTable, string FieldName) {
			DataTable dt = new DataTable(TableName);
			dt.Columns.Add(FieldName, SourceTable.Columns[FieldName].DataType);

			object LastValue = null;
			foreach (DataRow dr in SourceTable.Select("", FieldName)) {
				if (  LastValue == null || !(ColumnEqual(LastValue, dr[FieldName])) ) {
					LastValue = dr[FieldName];
					dt.Rows.Add(new object[]{LastValue});
				}
			}
			return dt;
		}

		/// <summary>
		/// Select the distinct values from n column
		/// </summary>
		/// <param name="TableName">Name of the new table</param>
		/// <param name="SourceTable">DataTable Source</param>
		/// <param name="FieldsName">Table of Fields Name (the order is also the sort order)</param>
		/// <returns>DataTable with distinc value from SourceTable.FieldName</returns>
		public static DataTable SelectDistinct(string TableName, DataTable SourceTable, string[] FieldsName) {
			DataTable dt = new DataTable(TableName);
			string fields = "";
			foreach(string str in FieldsName){
				dt.Columns.Add(str, SourceTable.Columns[str].DataType);
				fields += "," + str;
			}
			fields = fields.Remove(0,1);

			object[] LastValues = new object[FieldsName.Length];
			bool distinct = false;
			int i = 0;
			DataRow row = null;
			foreach (DataRow dr in SourceTable.Select("", fields)) {
				i = 0;
				distinct = false;
				while( !distinct && i < FieldsName.Length){
					if (LastValues[i] == null || !(ColumnEqual(LastValues[i], dr[FieldsName[i]])) ){
						distinct = true;
					}
					i++;
				}
				if (distinct) {
					row = dt.NewRow();
					for(i=0; i < FieldsName.Length; i++){
						LastValues[i] = dr[FieldsName[i]];
						row[i] = dr[FieldsName[i]];
					}
					dt.Rows.Add(row);
				}
			}
			return dt;
		}
		#endregion

		#region ColumnEqual
		/// <summary>
		/// Compares two values to see if they are equal. Also compares DBNULL.Value.
		/// </summary>
		/// <remarks>If your DataTable contains object fields, then you must extend this function to handle them in a meaningful way if you intend to group on them.</remarks>
		/// <param name="A">First Value</param>
		/// <param name="B">Second Value</param>
		/// <returns>True if A==B and false else</returns>
		private static bool ColumnEqual(object A, object B) {

			if ( A == DBNull.Value && B == DBNull.Value ) //  both are DBNull.Value
				return true;
			if ( A == DBNull.Value || B == DBNull.Value ) //  only one is DBNull.Value
				return false;
			return ( A.Equals(B) );  // value type standard comparison
		}
		#endregion

		#region Tri

		#region Tri spot à spot
        ///// <summary>
        ///// Donne l'ordre de tri des enregistrements extraits
        ///// </summary>
        ///// <param name="idVehicle">Identifiant du vehicle</param>
        ///// <param name="webSesssion">Session du client</param>
        ///// <param name="isMediaDetail">vrai si gère le niveau de détail média</param>
        ///// <param name="prefixeMediaPlanTable">prefixe table média</param>
        ///// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        ///// Lancée quand le cas du vehicle spécifié n'est pas traité
        ///// </exception>
        ///// <returns>Chaine contenant les champs de tris</returns>
        //public static string GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail,string prefixeMediaPlanTable){
        //    string sql="";
        //    switch(idVehicle){
        //        case DBClassificationConstantes.Vehicles.names.press:
        //        case DBClassificationConstantes.Vehicles.names.internationalPress:					
					
        //            sql+="  "+GetMediaInsertionsOrder(webSesssion.PreformatedMediaDetail,prefixeMediaPlanTable);
        //            sql+=", wp.date_media_Num"
        //                +", wp.id_advertisement"
        //                +", wp.media_paging"
        //                +", location";
        //            return sql;
        //        case DBClassificationConstantes.Vehicles.names.radio:
					
				
        //            sql+=" "+GetMediaInsertionsOrder(webSesssion.PreformatedMediaDetail,prefixeMediaPlanTable);
        //            sql+=","+GetRadioInsertionsOrder(webSesssion.Sort)+" "+webSesssion.SortOrder;
        //            if(!isMediaDetail || WebConstantes.RadioInsertionsColumnIndex.TOP_DIFFUSION_INDEX!=webSesssion.Sort)
        //                sql+=", wp.id_top_diffusion";
        //            sql+=", wp.id_cobranding_advertiser";
        //            return sql;

        //        case DBClassificationConstantes.Vehicles.names.tv:
        //        case DBClassificationConstantes.Vehicles.names.others:
					
				
        //            sql+="  "+GetMediaInsertionsOrder(webSesssion.PreformatedMediaDetail,prefixeMediaPlanTable);
        //            sql+=","+GetTvInsertionsOrder(webSesssion.Sort)+" "+webSesssion.SortOrder;
        //            if(!isMediaDetail || WebConstantes.TVInsertionsColumnIndex.ID_COMMERCIAL_BREAK_INDEX!=webSesssion.Sort)
        //                sql+=", wp.id_commercial_break";
        //            if(!isMediaDetail || WebConstantes.TVInsertionsColumnIndex.RANK_INDEX!=webSesssion.Sort)
        //                sql+=", wp.id_rank";
        //            return sql;
        //        case DBClassificationConstantes.Vehicles.names.outdoor:					

        //            sql+=" "+GetMediaInsertionsOrder(webSesssion.PreformatedMediaDetail,prefixeMediaPlanTable);
        //            sql+=","+GetOutDoorInsertionsOrder(webSesssion.Sort)+" "+webSesssion.SortOrder;
        //            if(!isMediaDetail || WebConstantes.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX!=webSesssion.Sort)
        //                sql+=", wp.number_board";
        //            return sql;
				
        //        default:
        //            throw new Exceptions.FunctionsDataAccessException(" GetInsertionsOrder : Le cas de ce média n'est pas gérer. Pas de table correspondante.");
        //    }
        //}

		/// <summary>
		/// Obtient l'ordre de tri des médias correspondants au détail media demandée par le client. 
		/// </summary>
		/// <param name="preformatedMediaDetail">Niveau du détail media demandé</param>
		/// <param name="prefixeMediaPlanTable">prefixe table média</param>
		/// <returns>Chaîne contenant les tables médias</returns>
		public static string GetMediaInsertionsOrder(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail,string prefixeMediaPlanTable){
			string sql="";
			
			//Vehicles							
			sql+=" vehicle ";								
			
			//Categories			
			sql+=" ,category ";					
				
			

			// Media							
		
			sql+=", media ";												
			

			//Accroches
			switch(preformatedMediaDetail){										
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:					
					if(sql.Length>0)sql+=",";
					sql+=prefixeMediaPlanTable+".id_slogan";										
					break;
			}
	
			return(sql);
		}

		/// <summary>
		/// Obtient la column des spots radio à trier
		/// </summary>
		/// <param name="indexColumnToSort">index de colonne</param>
		/// <returns>champs spots radio à trier</returns>
		public static string GetRadioInsertionsOrder(int indexColumnToSort){
			switch(indexColumnToSort){				
				case WebConstantes.RadioInsertionsColumnIndex.DATE_INDEX :
					return DBConstantes.Fields.DATE_MEDIA_NUM;					
				case WebConstantes.RadioInsertionsColumnIndex.ADVERTISER_INDEX :
					return DBConstantes.Fields.ADVERTISER;
				case WebConstantes.RadioInsertionsColumnIndex.PRODUCT_INDEX :
					return DBConstantes.Fields.PRODUCT;
				case WebConstantes.RadioInsertionsColumnIndex.GROUP_INDEX :
					return DBConstantes.Fields.GROUP_;				
				case WebConstantes.RadioInsertionsColumnIndex.TOP_DIFFUSION_INDEX:
					return DBConstantes.Fields.ID_TOP_DIFFUSION;
				case WebConstantes.RadioInsertionsColumnIndex.DURATION_INDEX:
                    return UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField;
				case WebConstantes.RadioInsertionsColumnIndex.RANK_INDEX:
					return DBConstantes.Fields.RANK;
				case WebConstantes.RadioInsertionsColumnIndex.BREAK_DURATION_INDEX:
					return DBConstantes.Fields.DURATION_COMMERCIAL_BREAK;
				case WebConstantes.RadioInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX:
					return DBConstantes.Fields.NUMBER_SPOT_COM_BREAK;
				case WebConstantes.RadioInsertionsColumnIndex.RANK_WAP_INDEX:
					return DBConstantes.Fields.RANK_WAP;
				case WebConstantes.RadioInsertionsColumnIndex.DURATION_BREAK_WAP_INDEX:
					return DBConstantes.Fields.DURATION_COM_BREAK_WAP;
				case WebConstantes.RadioInsertionsColumnIndex.BREAK_SPOTS_WAP_NB_INDEX:
					return DBConstantes.Fields.NUMBER_SPOT_COM_BREAK_WAP;
				default :	throw new WebExceptions.FunctionsDataAccessException(" GetRadioInsertionsOrder : Impossible de déterminer la colonne à trier."); 		
			}
				
		}

		/// <summary>
		/// Obtient la column des spots télé à trier
		/// </summary>
		/// <param name="indexColumnToSort">index de colonne</param>
		/// <returns>champs spots télé à trier</returns>
		public static string GetTvInsertionsOrder(int indexColumnToSort){
			switch(indexColumnToSort){				
				case WebConstantes.TVInsertionsColumnIndex.DATE_INDEX :
					return DBConstantes.Fields.DATE_MEDIA_NUM;					
				case WebConstantes.TVInsertionsColumnIndex.ADVERTISER_INDEX :
					return DBConstantes.Fields.ADVERTISER;
				case WebConstantes.TVInsertionsColumnIndex.PRODUCT_INDEX :
					return DBConstantes.Fields.PRODUCT;
				case WebConstantes.TVInsertionsColumnIndex.GROUP_INDEX :
					return DBConstantes.Fields.GROUP_;				
				case WebConstantes.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX:
					return DBConstantes.Fields.TOP_DIFFUSION;
				case WebConstantes.TVInsertionsColumnIndex.ID_COMMERCIAL_BREAK_INDEX:
					return DBConstantes.Fields.ID_COMMERCIAL_BREAK_INDEX;
				case WebConstantes.TVInsertionsColumnIndex.DURATION_INDEX:
                    return UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField;
				case WebConstantes.TVInsertionsColumnIndex.RANK_INDEX:
					return DBConstantes.Fields.ID_RANK;
				case WebConstantes.TVInsertionsColumnIndex.BREAK_DURATION_INDEX:
					return DBConstantes.Fields.DURATION_COMMERCIAL_BREAK;
				case WebConstantes.TVInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX:
					return DBConstantes.Fields.NUMBER_MESSAGE_COMMERCIAL_BREA;
				case WebConstantes.TVInsertionsColumnIndex.EXPENDITURE_INDEX:
                    return UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseField;								
				default :	throw new WebExceptions.FunctionsDataAccessException(" GetTvInsertionsOrder : Impossible de déterminer la colonne à trier."); 		
			}
				
		}

		/// <summary>
		/// Obtient la column des affiches à trier
		/// </summary>
		/// <param name="indexColumnToSort">index de colonne</param>
		/// <returns>champs  affiche à trier</returns>
		public static string GetOutDoorInsertionsOrder(int indexColumnToSort){
			switch(indexColumnToSort){				
				case WebConstantes.OutDoorInsertionsColumnIndex.DATE_INDEX :
					return DBConstantes.Fields.DATE_MEDIA_NUM;					
				case WebConstantes.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX :
					return DBConstantes.Fields.ADVERTISER;
				case WebConstantes.OutDoorInsertionsColumnIndex.PRODUCT_INDEX :
					return DBConstantes.Fields.PRODUCT;
				case WebConstantes.OutDoorInsertionsColumnIndex.GROUP_INDEX :
					return DBConstantes.Fields.GROUP_;				
				case WebConstantes.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX:
                    return UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].DatabaseField;
				case WebConstantes.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX:
					return DBConstantes.Fields.TYPE_BOARD;
				case WebConstantes.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX:
					return DBConstantes.Fields.TYPE_SALE;
				case WebConstantes.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX:
					return DBConstantes.Fields.POSTER_NETWORK;				
				case WebConstantes.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX:
                    return UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].DatabaseField;								
				default :	throw new WebExceptions.FunctionsDataAccessException(" GetTvInsertionsOrder : Impossible de déterminer la colonne à trier."); 		
			}
				
		}
		#endregion
	

		#endregion

		#region média
		/// <summary>
		/// Détermine si un support appartient à la TV Nationale Thématiques
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="idMedia">identifiant du support</param>
		/// <returns>vrai si support appartient à la TV Nationale Thématiques</returns>
		public static bool IsBelongToTvNatThematiques(WebSession webSession, string idMedia){

			StringBuilder t = new StringBuilder(1000);
			DataTable dt=null;

			t.Append(" select  "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media,"+DBConstantes.Tables.MEDIA_PREFIXE+".media,"+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category ");
			t.Append(" from  "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".CATEGORY  "+DBConstantes.Tables.CATEGORY_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".MEDIA "+DBConstantes.Tables.MEDIA_PREFIXE);
			t.Append(" "+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".BASIC_MEDIA "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			t.Append(" where ");
			t.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media ="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media ");
			t.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category ");
			t.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category=35 ");
			t.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language=33 ");
			t.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language=33 ");
			t.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language=33 ");
			t.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media = "+idMedia );
			t.Append("  order by media ");

			#region Execution de la requête
			try{
				dt = webSession.Source.Fill(t.ToString()).Tables[0];

				if(dt!=null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count>0)return true;
				else return false;
			}
			catch(System.Exception err){
				throw(new WebExceptions.FunctionsDataAccessException("Impossible de déterminer si le média appartient aux thématiques TV: "+t,err));
			}
			#endregion

		}
		
//		/// <summary>
//		/// Obtient l'identifiant du média en fonction de son libellé
//		/// </summary>
//		/// <param name="mediaLabel">libellé du média</param>
//		/// <returns>identifiant média</returns>
//		public static string GetIdMedia(string mediaLabel){
//
//			Hashtable ht = new Hashtable();
//			ht.Add(ConstantesFrameWork.Results.CommonMother.VEHICLE_LABEL,DBConstantes.Fields.ID_VEHICLE);
//			ht.Add(ConstantesFrameWork.Results.CommonMother.CATEGORY_LABEL,DBConstantes.Fields.ID_CATEGORY);
//			ht.Add(ConstantesFrameWork.Results.CommonMother.MEDIA_LABEL,DBConstantes.Fields.ID_MEDIA);
//			ht.Add(ConstantesFrameWork.Results.CommonMother.MEDIASELLER_LABEL,DBConstantes.Fields.ID_MEDIA_SELLER);
//			ht.Add(ConstantesFrameWork.Results.CommonMother.INTERESTCENTER_LABEL,DBConstantes.Fields.ID_INTEREST_CENTER);
//			ht.Add(ConstantesFrameWork.Results.CommonMother.SLOGAN_LABEL,DBConstantes.Fields.ID_SLOGAN);
//
//			if(ht[mediaLabel]!=null)return ht[mediaLabel].ToString();
//			else return "";
//		}
		#endregion

		#region Liste concaténée Champs investissements  mensuels des tables de Recap
		/// <summary>
		/// Obtient la liste des libellés de colonne des dépenses  mensuelles en euro  pour les Recap.
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="comparativeStudy">Vrai si étude comparative</param>
		/// <returns>liste des mois</returns>
		public static string SumMonthlyExpenditureEuroToString(WebSession webSession,bool comparativeStudy){
			return SumMonthlyExpenditureEuroToString(webSession,comparativeStudy,false);
		}

		/// <summary>
		/// Obtient la liste des libellés de colonne des dépenses  mensuelles en euro  pour les Recap.
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="comparativeStudy">Vrai si étude comparative</param>
		/// <param name="onlyComparativeMonthString">Vrai si retourne uniquement les mois de l'année précédente à l'année sélectionnée.</param>
		/// <returns>liste des mois</returns>
		public static string SumMonthlyExpenditureEuroToString(WebSession webSession,bool comparativeStudy,bool onlyComparativeMonthString){
			#region Mise en forme des dates
			//Determine la période de l'étude
			string StudyMonths="";
			//Periode etude comparative
			string ComparativeStudyMonths="";
			string YearSelected="";
			string ComparativeYearSelected="";
			int year=0;
			int comparativeYear=0;
			
			
			//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
			//du dernier mois dispo en BDD
			//traitement de la notion de fréquence
			string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
			
			if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
				throw new TNS.AdExpress.Domain.Exceptions.NoDataException();

			DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
			DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
			#endregion

			#region dates (mensuels) des investissements 
			int StartMonth= PeriodBeginningDate.Month;
			int EndMonth = PeriodEndDate.Month;
			WebFunctions.Dates.GetYearSelected(webSession,ref YearSelected,ref year,PeriodBeginningDate);		
			if( !PeriodEndDate.Equals(null) && !PeriodBeginningDate.Equals(null)) {				
				for(int i=StartMonth;i<=EndMonth;i++) {
					if(!onlyComparativeMonthString){
						if(i==EndMonth && StartMonth!=EndMonth){
							StudyMonths+="exp_euro_N"+YearSelected+"_"+i.ToString()+" ) as  total_N";
						}
						else if(i==StartMonth && StartMonth!=EndMonth){
							StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+" + ";
						}
						else if(StartMonth==EndMonth){
							StudyMonths+="sum(exp_euro_N"+YearSelected+"_"+i.ToString()+") total_N  ";
						}
						else{
							StudyMonths+="exp_euro_N"+YearSelected+"_"+i.ToString()+" + ";
						}
					}
					
					//Recuperation de la periode N-1 pour etude comparative
					if(comparativeStudy && (year==0 || year==1)){
						ComparativeYearSelected=(year==1)? "2" : "1";
						comparativeYear=(year==1)? 2 : 1;
						if(i==EndMonth && StartMonth!=EndMonth){
							ComparativeStudyMonths+="exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+") as total_N1 ";
						}
						else if(i==StartMonth && StartMonth!=EndMonth){
							ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+"  + ";
						}
						else if(StartMonth==EndMonth){
							ComparativeStudyMonths+="sum(exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+")  as total_N1 ";
						}
						else{
							ComparativeStudyMonths+="exp_euro_N"+ComparativeYearSelected+"_"+i.ToString()+"  + ";
						}
					}
				}
				
				
				if(comparativeStudy){					
					if(onlyComparativeMonthString)StudyMonths=ComparativeStudyMonths;
					else StudyMonths+=","+ComparativeStudyMonths;
				}
				
			}
			
			#endregion

			return StudyMonths;
				
			
		}
		#endregion

		#region Sélection Univers Recap
		/// <summary>
		/// Univers sélectionné média et produit sélectionné par le client dans l'annalyse sectorielle.
		/// </summary>
		public class RecapUniversSelection{
			/// <summary>
			/// Session client
			/// </summary>
			private WebSession _webSession;
			/// <summary>
			/// Annonceurs en accès
			/// </summary>
			private string _advertiserAccessList="";
			/// <summary>
			/// Annonceurs en concurrents
			/// </summary>
			private string _competitorAdvertiserAccessList="";
			/// <summary>
			/// Variétés en accès
			/// </summary>
			string _segmentAccessList="";	
			/// <summary>
			/// Variétés en exception
			/// </summary>
			private string _segmentExceptionList = "";
			/// <summary>
			/// Groupes en accès
			/// </summary>
			private string _groupAccessList ="";
			/// <summary>
			/// Groupes en exception
			/// </summary>
			private string _groupExceptionList = "";
			/// <summary>
			/// Catégories en accès
			/// </summary>
			private string _categoryAccessList = "";
			/// <summary>
			/// Supports en accès
			/// </summary>
			private string _mediaAccessList = "";
			/// <summary>
			/// Médias en accès
			/// </summary>
			private string _vehicleAccessList = "";

			/// <summary>
			/// Constructeur
			/// </summary>
			/// <param name="webSession">Session client</param>
			public RecapUniversSelection(WebSession webSession){
				
				_webSession = webSession;
				List<NomenclatureElementsGroup> nElmtGr = null;
				NomenclatureElementsGroup nomenclatureElementsGroup = null;
				string tempListAsString = "";

				if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0)) {
					nomenclatureElementsGroup = webSession.SecondaryProductUniverses[0].GetGroup(0);
					if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
						tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
						if (tempListAsString != null && tempListAsString.Length > 0) _advertiserAccessList = tempListAsString;
					}
					nomenclatureElementsGroup = null;
					tempListAsString = "";
				}
				//_advertiserAccessList = _webSession.GetSelection(_webSession.ReferenceUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
				if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1)) {
					nomenclatureElementsGroup = webSession.SecondaryProductUniverses[1].GetGroup(0);
					if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
						tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
						if (tempListAsString != null && tempListAsString.Length > 0) _competitorAdvertiserAccessList = tempListAsString;
					}
					tempListAsString = "";
				}
				//if(_webSession.CompetitorUniversAdvertiser[0]!=null)
				//    _competitorAdvertiserAccessList = _webSession.GetSelection((TreeNode)_webSession.CompetitorUniversAdvertiser[0],CustomerRightConstante.type.advertiserAccess);		
				if(WebFunctions.CheckedText.IsStringEmpty(_competitorAdvertiserAccessList)) {
					if(WebFunctions.CheckedText.IsStringEmpty(AdvertiserAccessList))_advertiserAccessList+=","+CompetitorAdvertiserAccessList;
					else _advertiserAccessList=_competitorAdvertiserAccessList;
				}
			
				
				if (webSession.PrincipalProductUniverses.Count > 0) {
					//Recuperation des éléments sélectionnés en inclusion
					nElmtGr = webSession.PrincipalProductUniverses[0].GetIncludes();
					if (nElmtGr != null && nElmtGr.Count > 0) {
						tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.GROUP_);
						if (tempListAsString != null && tempListAsString.Length > 0) _groupAccessList = tempListAsString;
						tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.SEGMENT);
						if (tempListAsString != null && tempListAsString.Length > 0) _segmentAccessList = tempListAsString;
					}
					//Recuperation des éléments sélectionnés exclusion
					nElmtGr = webSession.PrincipalProductUniverses[0].GetExludes();
					if (nElmtGr != null && nElmtGr.Count > 0) {
						tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.GROUP_);
						if (tempListAsString != null && tempListAsString.Length > 0) _groupExceptionList = tempListAsString;
						tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.SEGMENT);
						if (tempListAsString != null && tempListAsString.Length > 0) _segmentExceptionList = tempListAsString;
					}
				}					
							
				_categoryAccessList = _webSession.GetSelection(_webSession.CurrentUniversMedia,CustomerRightConstante.type.categoryAccess);	
				_mediaAccessList = _webSession.GetSelection(_webSession.CurrentUniversMedia,CustomerRightConstante.type.mediaAccess);		
				_vehicleAccessList = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			}
			
			/// <summary>
			/// Annonceurs en accès
			/// </summary>
			internal string AdvertiserAccessList{
				get{return _advertiserAccessList;}					
			}
			
			/// <summary>
			/// Annonceurs  concurrents
			/// </summary>
			internal string CompetitorAdvertiserAccessList{
				get{return _competitorAdvertiserAccessList;}					
			}
			
			/// <summary>
			/// Variétés en accès
			/// </summary>
			internal string SegmentAccessList{
				get{return _segmentAccessList;}					
			}
			/// <summary>
			/// Variétés en exception
			/// </summary>
			internal string SegmentExceptionList{
				get{return _segmentExceptionList;}					
			}
			/// <summary>
			/// groupes en accès
			/// </summary>
			internal string GroupAccessList{
				get{return _groupAccessList;}					
			}
			/// <summary>
			/// Groupes en exception
			/// </summary>
			internal string GroupExceptionList{
				get{return _groupExceptionList;}					
			}
			/// <summary>
			/// Catégories en accès
			/// </summary>
			internal string CategoryAccessList{
				get{return _categoryAccessList;}					
			}
			/// <summary>
			/// Supports en accès
			/// </summary>
			internal string MediaAccessList{
				get{return _mediaAccessList;}					
			}
			/// <summary>
			/// Médias en accès
			/// </summary>
			internal string VehicleAccessList{
				get{return _vehicleAccessList;}					
			}

			
		}
		#endregion
		
		
	}
}
