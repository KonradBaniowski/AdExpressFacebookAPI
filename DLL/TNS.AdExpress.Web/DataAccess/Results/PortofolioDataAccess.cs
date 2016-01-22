//#region Informations
//// Auteur: A. Obermeyer & D. V. Mussuma
//// Date de création: 01/12/2004
//// Date de modification: 
////	15/12/2004	D. Mussuma		Ajout méthode pour récupérer les données de structure
////				K.Shehzad	Modifications for Outdoor 
////	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
////	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
////	25/11/2005	B.Masson		webSession.Source
////	8/12/2005   D. V. Mussuma Surcharge de la méthode  GetDetailMedia
//#endregion

//#region Using
//using System;
//using System.Data;
//using System.Text;
//using System.Collections;
//using System.Windows.Forms;
//using Oracle.DataAccess.Client;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.Exceptions;
//using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
//using DBConstantes=TNS.AdExpress.Constantes.DB;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
//using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
//using WebConstantes = TNS.AdExpress.Constantes.Web;
//using CstProject = TNS.AdExpress.Constantes.Project;
//using TNS.AdExpress.Constantes.FrameWork.Results;
//using TNS.FrameWork.DB.Common;
//using TNS.AdExpress.Domain.Level;
//using TNS.AdExpress.Web.Core.Exceptions;
//#endregion

//namespace TNS.AdExpress.Web.DataAccess.Results{
//    /// <summary>
//    /// Accès au données pour le module portefeuille d'un support
//    /// </summary>
//    public class PortofolioDataAccess{
		
//        #region Synthèse publicitaire
		
//        #region Périodicité, Catégorie,Régie,Centre d'intérêt
//        /// <summary>
//        ///  Recupère les champs suivant : category, Régie, centre d'intérêt
//        /// et en plus périodicité pour la presse
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <param name="idMedia">identifiant media</param>
//        /// <returns>category, Régie, centre d'intérêt et en plus périodicité pour la presse</returns>
//        public static DataSet GetCategoryMediaSellerData(WebSession webSession,Int64 idVehicle,Int64 idMedia){
		
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="me";
//            #endregion

//            #region Variables
//            string sql="";
//            string tableName="";
//            string fields = "";			
//            string joint="";	
//            string mediaRights="";
//            #endregion

//            #region Construction de la requête
//            try{
//                tableName = GetTable((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//                fields = GetFields((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//                joint = GetJoint((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()),webSession);
//                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                sql+=" select "+fields; 
//                sql+=" from "+tableName;
//                sql+=" where me.id_media="+idMedia+"";
//                sql+=joint;	
//            }
//            catch(System.Exception err){
//                throw(new PortofolioDataAccessException("Impossible de construire la requête",err));
//            }
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la Synthèse publicitaire: "+sql,err));
//            }
//            #endregion
		
//        }
//        #endregion

//        #region Investissement,première date de parution,dernière date de parution
//        /// <summary>
//        /// Récupère le total investissement, les dates de parutions
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <param name="idMedia">identifiant media</param>
//        /// <param name="dateBegin">date de début</param>
//        /// <param name="dateEnd">date de fin</param>
//        /// <returns>Récupère le total investissement, les dates de parutions</returns>
//        public static DataSet GetInvestment(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
//            #region Construction de la requête
			
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

//            string select=GetSelectData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//            string table=GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//            string product=GetProductData(webSession);
//            string productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//            string mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//            //liste des produit hap
//            string listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);


//            string sql=select;

//            sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+table+" wp ";
//            sql+=" where id_media="+idMedia+"";
//            //sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";					
//            if(dateBegin.Length>0)
//                sql+=" and date_media_num>="+dateBegin+" ";
//            if(dateEnd.Length>0)			
//                sql+=" and date_media_num<="+dateEnd+"";

////			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())!=DBClassificationConstantes.Vehicles.names.outdoor)sql+=" and insertion=1";
//            sql+=product;
//            sql+=productsRights;
//            sql+=mediaRights;
//            sql+=listProductHap;
////			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.outdoor)
////				sql+=" group by TYPE_SALE";
			
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la Synthèse publicitaire: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #region Nombre d'insertions
//        /// <summary>
//        /// Récupère le Nombre d'insertions
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <param name="idMedia">identifiant media</param>
//        /// <param name="dateBegin">date de début</param>
//        /// <param name="dateEnd">date de fin</param>
//        /// <returns>Récupère le total investissement, les dates de parutions</returns>
//        public static DataSet GetInsertionNumber(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
//            #region Construction de la requête
			
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

			
//            string table=GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//            string product=GetProductData(webSession);
//            string productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//            string mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//            //liste des produit hap
//            string listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);


//            string insertionField="insertion";
//            if((DBClassificationConstantes.Vehicles.names)idVehicle==DBClassificationConstantes.Vehicles.names.outdoor)
//                insertionField="NUMBER_BOARD";

//            string sql=" select sum("+insertionField+") as insertion " ;

//            sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+table+" wp ";
//            sql+=" where id_media="+idMedia+"";
//            //sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";					
//            if(dateBegin.Length>0)
//                sql+=" and date_media_num>="+dateBegin+" ";
//            if(dateEnd.Length>0)			
//                sql+=" and date_media_num<="+dateEnd+"";

//            if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())!=DBClassificationConstantes.Vehicles.names.outdoor)sql+=" and insertion=1";
//            sql+=product;
//            sql+=productsRights;
//            sql+=mediaRights;
//            sql+=listProductHap;
			
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la Synthèse publicitaire: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #region TypeSale outdoor
//        /// <summary>
//        /// Récupère le type_sale pour affichage
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <param name="idMedia">identifiant média</param>
//        /// <param name="dateBegin">date de début</param>
//        /// <param name="dateEnd">date de fin </param>
//        /// <returns>DataSet</returns>
//        public static DataSet GetTypeSale(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
//            #region Construction de la requête
			
//            string table=GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
			
//            string sql="select distinct type_sale";
//            sql+=" from  "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+table;
//            sql+=" where id_media="+idMedia+" ";
//            if(dateBegin.Length>0)
//                sql+=" and  date_media_num>="+dateBegin+" ";
//            if(dateEnd.Length>0)
//                sql+=" and  date_media_num<="+dateEnd+" ";		

//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la Synthèse publicitaire: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #region Pages
//        /// <summary>
//        /// Récupère le nombre de pages
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <param name="idMedia">identifiant média</param>
//        /// <param name="dateBegin">date de début</param>
//        /// <param name="dateEnd">date de fin </param>
//        /// <returns>DataSet</returns>
//        public static DataSet GetPage(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
//            #region Construction de la requête
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="al";
//            #endregion		

//            string sql="select sum(number_page_media) page";
//            sql += " from  " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.ALARM_MEDIA + " " + DATA_TABLE_PREFIXE;
//            sql+=" where al.id_media="+idMedia+" ";
//            //sql+=" and al.ID_LANGUAGE_I="+webSession.SiteLanguage+" ";
//            if(dateBegin.Length>0)//date_cover_num
//                sql+=" and  al.DATE_ALARM>="+dateBegin+" ";//date_cover_num
//            if(dateEnd.Length>0)//date_cover_num
//                sql+=" and  al.DATE_ALARM<="+dateEnd+" ";		//date_cover_num

//            /*sql += " and wp.id_media=al.id_media";//date_cover_num
//            sql += " and wp.date_cover_num=al.date_alarm";//date_cover_num
//            if(dateBegin.Length>0)//date_cover_num
//                sql+=" and  wp.date_media_num>="+dateBegin+" ";//date_cover_num
//            if(dateEnd.Length>0)//date_cover_num
//                sql+=" and  wp.date_media_num<="+dateEnd+" ";//date_cover_num*/


//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la Synthèse publicitaire: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #region Nombre de produit(nouveau,dans la pige), annonceurs
//        /// <summary>
//        /// Tableau contenant le nombre de produits, 
//        /// le nombre de nouveau produit dans le support,
//        /// le nombre de nouveau produit dans la pige,
//        /// le nombre d'annonceurs		
//        /// </summary>
//        /// <remarks>N'utilise pas IDataSource car dataReader et traitement sur les lignes</remarks>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="idVehicle">Identifiant du media (vehicle)</param>
//        /// <param name="idMedia">Identifiant du support (media)</param>
//        /// <param name="dateBegin">Date de début</param>
//        /// <param name="dateEnd">Date de fin</param>
//        /// <returns>Données</returns>
//        public static object [] NumberProductAdvertiser(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

//            #region Variables
//            object [] tab=new object[4];
//            string sql="";
//            string tableName="";
//            int compteur=0;
//            string productsRights=null;
//            string product=null;
//            string mediaRights=null;
//            string listProductHap=null;
//            #endregion
			
//            try{
//                tableName = GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//                product=GetProductData(webSession);
//                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de construire la requête pour la Synthèse publicitaire: "+sql,err));
//            }

//            #region Execution de la requête

//            OracleConnection connection=(OracleConnection)webSession.Source.GetSource();
//            OracleCommand sqlCommand=null;

//            OracleDataAdapter sqlAdapter=null;
//            OracleDataReader sqlOracleDataReader=null;
			
//            #region Ouverture de la base de données
//            bool DBToClosed=false;
//            // On teste si la base est déjà ouverte
//            if (connection.State==System.Data.ConnectionState.Closed){
//                DBToClosed=true;
//                try{
//                    connection.Open();
//                }
//                catch(System.Exception et){
//                    throw(new PortofolioDataAccessException("Impossible d'ouvrir la base de données:"+et.Message));
//                }
//            }
//            #endregion		
			
			
//            try{
//                for(int i=0 ;i<=3;i++){
//                    if(((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())!=DBClassificationConstantes.Vehicles.names.outdoor)||(i!=2&&i!=1)){
//                        if(i<=2){
//                            sql=" select  id_product ";
//                        }
//                        else{
//                            sql=" select  id_advertiser ";
//                        }
//                        sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp" ;
//                        sql+=" where id_media="+idMedia+"";
//                        //sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";
//                        // Support
//                        if(i==1)
//                            sql+=" and new_product=1 ";
//                        // Pige
//                        if(i==2)
//                            sql+=" and new_product=2 ";
				
//                        if(dateBegin.Length>0)
//                            sql+=" and  DATE_MEDIA_NUM>= "+dateBegin+" ";
//                        if(dateEnd.Length>0)
//                            sql+=" and  DATE_MEDIA_NUM<= "+dateEnd+" ";

//                        sql+=product;
//                        sql+=productsRights;
//                        sql+=mediaRights;
//                        sql+=listProductHap;

//                        if(i<=2){
//                            sql+=" group by id_product ";
//                        }
//                        else{
//                            sql+=" group by id_advertiser ";
//                        }

//                        sqlCommand=new OracleCommand(sql,connection);
//                        sqlAdapter=new OracleDataAdapter(sqlCommand);
//                        sqlOracleDataReader=sqlCommand.ExecuteReader();
				
//                        while(sqlOracleDataReader.Read()){					
//                            compteur++;
//                        }
//                        tab[i]=compteur;
//                        compteur=0;
//                    }
//                }
			
//            }
//            #region Traitement d'erreur du chargement des données
//            catch(System.Exception ex){
//                try{
//                    // Fermeture de la base de données
//                    if (sqlAdapter!=null){
//                        sqlAdapter.Dispose();
//                    }
//                    if(sqlCommand!=null) sqlCommand.Dispose();
//                    if (DBToClosed) connection.Close();
//                }
//                catch(System.Exception et){
//                    throw(new PortofolioDataAccessException ("Impossible de fermer la base de données, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
//                }
//                throw(new PortofolioDataAccessException ("Impossible de charger les données d'un portefeuille:"+sql+" "+ex.Message));
//            }
//            #endregion

//            #endregion	

//            #region Fermeture de la base de données
//            try{
//                // Fermeture de la base de données
//                if (sqlAdapter!=null){
//                    sqlAdapter.Dispose();
//                }
//                if(sqlCommand!=null)sqlCommand.Dispose();
//                if (DBToClosed) connection.Close();
//            }
//            catch(System.Exception et){
//                throw(new PortofolioDataAccessException ("Impossible de fermer la base de données :"+et.Message));
//            }
//            #endregion

//            return tab;
//        }
//        #endregion

//        #region Page Encart
//        /// <summary>
//        /// Encart
//        /// </summary>
//        /// <remarks>N'utilise pas IDataSource car dataReader et traitement sur les lignes</remarks>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="idVehicle">Identifiant du media (vehicle)</param>
//        /// <param name="idMedia">Identifiant du support (media)</param>
//        /// <param name="dateBegin">Date de début</param>
//        /// <param name="dateEnd">Date de fin</param>
//        /// <returns>Données</returns>
//        public static object [] NumberPageEncart(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
			
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            const string LIST_ENCART="85,108,999";
//            #endregion

//            #region Variables
//            object [] tab=new object[4];
//            string sql="";
//            string tableName="";
//            string productsRights=null;
//            string mediaRights=null;
//            string product=null;
//            string listProductHap=null;
//            #endregion
			
//            try{
//                tableName = GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                product=GetProductData(webSession);
//                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de construire la requête pour la Synthèse publicitaire: "+sql,err));
//            }

//            #region Execution de la requête

//            OracleConnection connection=(OracleConnection)webSession.Source.GetSource();
//            OracleCommand sqlCommand=null;

//            OracleDataAdapter sqlAdapter=null;
//            OracleDataReader sqlOracleDataReader=null;
			
//            #region Ouverture de la base de données
//            bool DBToClosed=false;
//            // On teste si la base est déjà ouverte
//            if (connection.State==System.Data.ConnectionState.Closed){
//                DBToClosed=true;
//                try{
//                    connection.Open();
//                }
//                catch(System.Exception et){
//                    throw(new PortofolioDataAccessException("Impossible d'ouvrir la base de données:"+et.Message));
//                }
//            }
//            #endregion		
			
			
//            try{

//                for(int i=0 ;i<=2;i++){
					
////					sql=" select sum(area_page)/1000 as page ";
//                    sql=" select sum(area_page) as page ";
//                    sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp";
//                    sql+=" where ID_MEDIA="+idMedia+" ";
//                    //sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";
//                    // hors encart
//                    if(i==1){
//                        sql+=" and id_inset=null ";
//                    }
//                    // Encart
//                    if(i==2){
//                        sql+=" and id_inset in ("+LIST_ENCART+") ";
//                    }

//                    if(dateBegin.Length>0)
//                        sql+=" and  DATE_MEDIA_NUM>="+dateBegin+" ";
//                    if(dateEnd.Length>0)
//                        sql+=" and  DATE_MEDIA_NUM<="+dateEnd+" ";

//                    sql+=product;
//                    sql+=productsRights;
//                    sql+=mediaRights;
//                    sql+=listProductHap;

//                    sqlCommand=new OracleCommand(sql,connection);
//                    sqlAdapter=new OracleDataAdapter(sqlCommand);
//                    sqlOracleDataReader=sqlCommand.ExecuteReader();
				
//                    while(sqlOracleDataReader.Read()){
//                        tab[i]=sqlOracleDataReader[0].ToString();						
//                    }					
//                }
			
//            }
//            #region Traitement d'erreur du chargement des données
//            catch(System.Exception ex){
//                try{
//                    // Fermeture de la base de données
//                    if (sqlAdapter!=null){
//                        sqlAdapter.Dispose();
//                    }
//                    if(sqlCommand!=null) sqlCommand.Dispose();
//                    if (DBToClosed) connection.Close();
//                }
//                catch(System.Exception et){
//                    throw(new PortofolioDataAccessException ("Impossible de fermer la base de données, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
//                }
//                throw(new PortofolioDataAccessException ("Impossible de charger les données d'un portefeuille:"+sql+" "+ex.Message));
//            }
//            #endregion
		
//            return tab;
		
//        }
//        #endregion

//        #endregion

//        #region Insertion Investissement par support
//        /// <summary>
//        /// Récupère les investissements par supports
//        /// </summary>
//        /// <remarks>N'utilise pas IDataSource car dataReader et traitement sur les lignes</remarks>
//        /// <param name="webSession">Session client</param>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <param name="idMedia">identifiant media</param>
//        /// <param name="dateBegin">début de la période</param>
//        /// <param name="dateEnd">fin de la période</param>
//        /// <returns>Données</returns>
//        public static Hashtable GetInvestmentByMedia(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){

//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

//            #region Variables
//            string sql="";		
//            Hashtable htInvestment=new Hashtable();
//            string product=null;
//            string productsRights=null;
//            string mediaRights=null;
//            string listProductHap=null;
//            #endregion

//            try{
//                product=GetProductData(webSession);
//                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de construire la requête pour la Synthèse publicitaire: "+sql,err));
//            }
		 
//            #region Construction de la requête
//            sql+=" select sum(insertion) insertion,sum(expenditure_euro) investment,date_cover_num date1";
//            sql+="  from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.ALERT_DATA_PRESS+" wp";
//            sql+=" where id_media="+idMedia+" ";
//            //sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";
//            if(dateBegin.Length>0)
//                sql+=" and date_media_num>="+dateBegin+" ";
//            if(dateEnd.Length>0)
//                sql+="  and date_media_num<="+dateEnd+" ";
//            sql+=product;
//            sql+=productsRights;
//            sql+=mediaRights;
//            sql+=listProductHap;
//            sql+=" group by date_cover_num ";	//,insertion
//            #endregion

//            #region Execution de la requête
//            try{
//                DataSet ds = webSession.Source.Fill(sql);
////				if(ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0){
//                if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0){
//                    string[] value1=null;
//                    foreach(DataRow current in ds.Tables[0].Rows){
//                        value1=new string[2];
//                        value1[0]=current["investment"].ToString();
//                        value1[1]=current["insertion"].ToString();
//                        htInvestment.Add(current["date1"],value1);
//                    }
//                }
////				else
////					throw(new PortofolioDataAccessException("Impossible de récupèrer la date de dernière parution d'un média"));
//            }
//            catch(System.Exception err){
//                throw(new PortofolioDataAccessException("Erreur dans la récupération des investissements par support", err));
//            }
//            #endregion

//            #region Ancien code

////			#region Execution de la requête
////			OracleConnection connection=new OracleConnection(webSession.CustomerLogin.OracleConnectionString);
////			OracleCommand sqlCommand=null;
////			OracleDataReader sqlOracleDataReader=null;
////			OracleDataAdapter sqlAdapter=null;
////
////			#region Ouverture de la base de données
////			bool DBToClosed=false;
////			// On teste si la base est déjà ouverte
////			if (connection.State==System.Data.ConnectionState.Closed){
////				DBToClosed=true;
////				try{
////					connection.Open();
////				}
////				catch(System.Exception et){
////					throw(new PortofolioDataAccessException("Impossible d'ouvrir la base de données:"+et.Message));
////				}
////			}
////			#endregion
////
////			try{
////				sqlCommand=new OracleCommand(sql,connection);
////				sqlAdapter=new OracleDataAdapter(sqlCommand);
////				sqlOracleDataReader=sqlCommand.ExecuteReader();
////				string[] value1=null;
////				while(sqlOracleDataReader.Read()){
////					
////					if((decimal)sqlOracleDataReader["insertion"]>0){
////						value1=new string[2];
////						value1[0]=sqlOracleDataReader["investment"].ToString();
////						value1[1]=sqlOracleDataReader["insertion"].ToString();
////						htInvestment.Add(sqlOracleDataReader["date1"],value1);
////					}
////				}
////				
////			}
////			#region Traitement d'erreur du chargement des données
////			catch(System.Exception ex){
////				try{
////					// Fermeture de la base de données
////					if (sqlAdapter!=null){
////						sqlAdapter.Dispose();
////					}
////					if(sqlCommand!=null) sqlCommand.Dispose();
////					if (DBToClosed) connection.Close();
////				}
////				catch(System.Exception et){
////					throw(new PortofolioDataAccessException ("Impossible de fermer la base de données, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
////				}
////				throw(new PortofolioDataAccessException ("Impossible de charger les données d'un portefeuille:"+sql+" "+ex.Message));
////			}
////			#endregion
////
////			#region Fermeture de la base de données
////			try{
////				// Fermeture de la base de données
////				if (sqlAdapter!=null){
////					sqlAdapter.Dispose();
////				}
////				if(sqlCommand!=null)sqlCommand.Dispose();
////				if (DBToClosed) connection.Close();
////			}
////			catch(System.Exception et){
////				throw(new PortofolioDataAccessException ("Impossible de fermer la base de données :"+et.Message));
////			}
////			#endregion
////
////			#endregion	

//            #endregion

//            return(htInvestment);
//        }
//        #endregion
	
//        #region Ecran
//        /// <summary>
//        /// récupère les écrans
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="idVehicle">Identifiant du media</param>
//        /// <param name="idMedia">Identifiant du support</param>
//        /// <param name="dateBegin">Date de debut</param>
//        /// <param name="dateEnd">Date de fin</param>
//        /// <returns>Ecrans</returns>
//        public static DataSet GetEcranData(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

//            #region Variables
//            string select="";
//            string table="";
//            string product="";
//            string productsRights="";
//            string mediaRights="";
//            string listProductHap="";
//            #endregion

//            #region Construction de la requête
//            try{
//                select=GetSelectDataEcran((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//                table=GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//                product=GetProductData(webSession);
//                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
//            }
//            catch(System.Exception err){
//                throw(new PortofolioDataAccessException("Impossible de construire la requête",err));
//            }

//            string	sql="select sum(insertion) as nbre_ecran,sum(ecran_duration) as ecran_duration ,sum(nbre_spot) as nbre_spot";
//            //	sql+=" ,sum(ecran_duration)/sum(insertion) as averageDurationEcran";	
//            //	sql+=" ,sum(nbre_spot)/sum(insertion) as averageSpotByEcran";	
	
//            sql+=" from ( ";

//            sql+=select;
//            sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+table+" wp ";
//            sql+=" where id_media="+idMedia+" ";
//            //sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";					
//            if(dateBegin.Length>0)
//                sql+=" and date_media_num>="+dateBegin+" ";
//            if(dateEnd.Length>0)			
//                sql+=" and date_media_num<="+dateEnd+"";

//            sql+=" and insertion=1";
//            sql+=product;
//            sql+=productsRights;
//            sql+=mediaRights;
//            sql+=listProductHap;
//            sql+=" )";

//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la Synthèse publicitaire: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #endregion

//        #region	Détail Portefeuille
//        /// <summary>
//        /// Charge les données pour créer une alerte portefeuille d'un support
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="moduleType">Type du module</param>
//        /// <param name="beginingDate">Date de début</param>
//        /// <param name="endDate">Date de fin</param>
//        /// <param name="vehicleName">Média à traiter</param>
//        /// <returns>Données de l'alerte concurrentielle</returns>
//        public static DataSet GetData(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName,WebConstantes.Module.Type moduleType, string beginingDate, string endDate){

//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

//            #region Variables
//            string dataTableName="";
//            string dataTableNameForGad="";
//            string dataFieldsForGad="";
//            string dataJointForGad="";
//            string detailProductTablesNames="";
//            string detailProductFields="";
//            string detailProductJoints="";
//            string detailProductOrderBy="";
//            string unitsFields="";
//            string productsRights="";
//            string sql="";
//            string list="";
//            string mediaList="";			
//            bool premier;
//            string dataJointForInsert="";
//            string listProductHap="";
//            string mediaRights="";
//            string mediaAgencyTable=string.Empty;
//            string mediaAgencyJoins=string.Empty;
//            #endregion

//            #region Construction de la requête
//            try{
//                dataTableName=WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleName,moduleType);
//                detailProductTablesNames=webSession.GenericProductDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
//                detailProductFields=webSession.GenericProductDetailLevel.GetSqlFields();
//                detailProductJoints=webSession.GenericProductDetailLevel.GetSqlJoins(webSession.SiteLanguage,DATA_TABLE_PREFIXE);
//                unitsFields = WebFunctions.SQLGenerator.GetUnitFieldsName(webSession,Constantes.DB.TableType.Type.dataVehicle,DATA_TABLE_PREFIXE);
//                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//                detailProductOrderBy=webSession.GenericProductDetailLevel.GetSqlOrderFields();
//                //option encarts (pour la presse)
//                if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
//                    dataJointForInsert=WebFunctions.SQLGenerator.GetJointForInsertDetail(webSession,DATA_TABLE_PREFIXE);
//                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				
//                if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)){
//                    try{
//                        dataTableNameForGad=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GAD+" "+DBConstantes.Tables.GAD_PREFIXE;
//                        dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
//                        dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);
//                    }
//                    catch(SQLGeneratorException){;}
//                }
//                ////Agence_media
//                //if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency)||webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)){
//                //    mediaAgencyTable=DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
//                //    mediaAgencyJoins="And "+DATA_TABLE_PREFIXE+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
//                //    mediaAgencyJoins+="And "+DATA_TABLE_PREFIXE+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
//                //}
//            } 
//            catch(System.Exception err){
//                throw(new PortofolioDataAccessException("Impossible d'initialiser les paramètres de la requêtes",err));
//            }

//            sql+=" select "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad+","+unitsFields;
//            sql+=" from "+mediaAgencyTable+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DATA_TABLE_PREFIXE;
//            if (detailProductTablesNames.Length > 0)
//                sql+=", "+detailProductTablesNames;
//            sql+=" "+dataTableNameForGad;
//            // Période
//            sql+=" where date_media_num >="+beginingDate;
//            sql+=" and date_media_num <="+endDate;
//            // Jointures Produits
//            sql+=" "+detailProductJoints;
//            sql+=" "+dataJointForGad;
//            sql+=" "+mediaAgencyJoins;

//            //Jointures encart
//            if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
//                sql+=" "+dataJointForInsert;
			
//            #region Sélection de Médias
//            mediaList+=webSession.GetSelection((TreeNode) webSession.ReferenceUniversMedia,CustormerConstantes.Right.type.mediaAccess);						
//            if (mediaList.Length>0)sql+=" and "+DATA_TABLE_PREFIXE+".id_media in ("+mediaList+")";
//            #endregion
		
//            #region Ancienne version Sélection de Produits
//            //// Sélection en accès
//            //premier=true;
//            //// Sector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorAccess);
//            //if(list.Length>0){
//            //    sql+=" and ((wp.id_sector in ("+list+") ";
//            //    premier=false;
//            //}
//            //// SubSector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorAccess);
//            //if(list.Length>0){
//            //    if(!premier) sql+=" or";
//            //    else sql+=" and ((";
//            //    sql+=" wp.id_subsector in ("+list+") ";
//            //    premier=false;
//            //}
//            //// Group
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupAccess);
//            //if(list.Length>0){
//            //    if(!premier) sql+=" or";
//            //    else sql+=" and ((";
//            //    sql+=" wp.id_group_ in ("+list+") ";
//            //    premier=false;
//            //}

//            //if(!premier) sql+=" )";
			
//            //// Sélection en Exception
//            //// Sector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_sector not in ("+list+") ";
//            //    premier=false;
//            //}
//            //// SubSector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_subsector not in ("+list+") ";
//            //    premier=false;
//            //}
//            //// Group
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_group_ not in ("+list+") ";
//            //    premier=false;
//            //}
//            //if(!premier) sql+=" )";
//            #endregion

//            #region Sélection de Produits
//            sql += " " + GetProductData(webSession);
//            #endregion

//            // Droits des Médias
//            // Droits des Produits
//            sql+=" "+productsRights;
//            sql+=mediaRights;
//            sql+=listProductHap;
//            // Group by
//            sql+=" group by "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad;
//            // Order by
//            sql+=" order by "+detailProductOrderBy+","+DATA_TABLE_PREFIXE+".id_media";
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour le détail du portefeuille: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #region Performances
//        /// <summary>
//        /// Obtient les parutions,euros,spot et durée pour les résultas de performances
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="dateBegin">date de début </param>
//        /// <param name="dateEnd">date de fin</param>
//        /// <returns>résultats pour les performancesle portefeuille d'un support</returns>
//        public static DataSet GetPerformancesData(WebSession webSession,int dateBegin,int dateEnd){

//            #region variables
//            //DataSet ds=null;
//            //Media sélectionné
//            string idVehicle="";
//            //support sélectionné
//            string idMedia="";
//            string tableName="";
//            string fields="";
//            string sql="";
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

//            #region construction de la requête 
//            try{

//                #region Sélection du vehicle et du support
//                //ID média sélectionné
//                idVehicle=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);								
//                //Id support sélectionné
//                idMedia =webSession.GetSelection(webSession.ReferenceUniversMedia,CustomerRightConstante.type.mediaAccess);											
//                //Performance uniquement disponibles pour la presse
//                if(((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()))!=DBClassificationConstantes.Vehicles.names.press
//                    && (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())!=DBClassificationConstantes.Vehicles.names.internationalPress)
//                    throw new Exceptions.PortofolioDataAccessException("getPerformancesData(WebSession webSession,int dateBegin,int dateEnd)--> La page de résultats Performances est uniquement disponible pour la presse.");
//                #endregion
				
//                //Nom de la table
//                switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
//                    case DBClassificationConstantes.Vehicles.names.press:
//                        tableName=WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(DBClassificationConstantes.Vehicles.names.press,WebConstantes.Module.Type.analysis);
//                        break;
//                    case DBClassificationConstantes.Vehicles.names.internationalPress:
//                        tableName=WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(DBClassificationConstantes.Vehicles.names.internationalPress,WebConstantes.Module.Type.analysis);
//                        break;
//                }
//                //tableName = getTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));								
////				tableName=WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult(DBClassificationConstantes.Vehicles.names.press,WebConstantes.Module.Type.analysis);
				
//                //Champs demandés
//                fields = getPerformancesFields(DATA_TABLE_PREFIXE);							
//            }
//            catch(System.Exception ex){
//                throw new Exceptions.PortofolioDataAccessException("getPerformancesData(WebSession webSession,int dateBegin,int dateEnd)--> Impossible de déterminer la table ou les champs à traiter",ex);
//            }

//            #region construction de la requête
//            if(WebFunctions.CheckedText.IsStringEmpty(tableName.ToString().ToString())){
//                // Sélection de la nomenclature Support
//                sql+=" select " + fields;
//                // Tables
//                sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" "+DATA_TABLE_PREFIXE+" ";
//                sql+=" where ";
//                //Choix de la langue
//                //sql+=" "+DATA_TABLE_PREFIXE+".id_language_data_i ="+webSession.SiteLanguage;
//                // Période
//                sql+=" "+DATA_TABLE_PREFIXE+".date_media_num>="+dateBegin;
//                sql+=" and "+DATA_TABLE_PREFIXE+".date_media_num<="+dateEnd;
//                //Encarts
//                sql+=WebFunctions.SQLGenerator.GetJointForInsertDetail(webSession,DATA_TABLE_PREFIXE);
				
//                #region Nomenclature Produit (droits)
//                //Droits en accès
//                sql+=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//                //liste des produit hap
//                string listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
//                if(WebFunctions.CheckedText.IsStringEmpty(listProductHap.ToString().Trim()))		
//                    sql+=listProductHap;
//                //Liste des produits sélectionnés
//                string product=GetProductData(webSession);
//                if(WebFunctions.CheckedText.IsStringEmpty(product.ToString().Trim()))
//                    sql+=product;
//                #endregion

//                #region Nomenclature Media (droits et sélection)

//                #region Droits
//                sql+=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                #endregion

//                #region Sélection média
//                //sélection média (vehicle)
//                if(WebFunctions.CheckedText.IsStringEmpty(idVehicle.ToString().Trim()))					
//                    sql+=" and "+DATA_TABLE_PREFIXE+".id_vehicle="+idVehicle.ToString();
//                //sélection support	
//                if(WebFunctions.CheckedText.IsStringEmpty(idMedia.ToString().Trim()))					
//                    sql+=" and "+DATA_TABLE_PREFIXE+".id_media="+idMedia.ToString();								
//                #endregion

//                #endregion
				
//                //Regroupement et tri
//                sql+=" group by "+DATA_TABLE_PREFIXE+".date_media_num ";
//                sql+=" order by  "+DATA_TABLE_PREFIXE+".date_media_num asc";	
//            }
//            #endregion
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la Synthèse publicitaire: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #region Nouveautés
//        /// <summary>
//        /// Dataset avec les nouveaux produits
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <param name="idMedia">identifiant média</param>
//        /// <param name="dateBegin">date de début</param>
//        /// <param name="dateEnd">date de fin</param>
//        /// <returns>Liste des nouveaux produits</returns>
//        public static DataSet GetNewProduct(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion
			
//            #region Variables
//            string sql="";
//            string tableName="";
//            string selectNewProduct="";
//            string productsRights="";
//            string product="";
//            string mediaRights="";
//            string listProductHap="";
//            #endregion

//            try{
//                selectNewProduct=GetSelectNewProduct(webSession,(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//                tableName = getTableDataNewProduct((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
			
//                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//                product=GetProductData(webSession);
//                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
//            }
//            catch(System.Exception err){
//                throw(new PortofolioDataAccessException("Impossible de construire la requête",err));
//            }

//            #region Construction de la requête
//            sql=selectNewProduct;			
//            sql+=" from "+tableName;
//            sql+=" where id_media="+idMedia+"";
		
//            //Jointure
//            sql+=" and pr.id_product=wp.id_product ";
					
//            // Langues
//        //	sql+=" and pr.id_language=wp.id_language_data_i ";
			
//            // activation
			
//            sql+=" and pr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
 
//            // langue
//        //	sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";
//            sql+=" and pr.id_language="+webSession.SiteLanguage+" ";

//            if(dateBegin.Length>0)
//                sql+=" and  DATE_MEDIA_NUM>= "+dateBegin+" ";
//            if(dateEnd.Length>0)
//                sql+=" and  DATE_MEDIA_NUM<= "+dateEnd+" ";

//            if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.press
//                || (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.internationalPress){
//                sql+=" and co.id_color=wp.id_color ";
//            //	sql+=" and co.id_language=wp.id_language_data_i ";
//                sql+=" and co.id_language="+webSession.SiteLanguage+" ";
//                sql+=" and co.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";

//                sql+=" and fo.id_format=wp.id_format ";
//            //	sql+=" and fo.id_language=wp.id_language_data_i ";
//                sql+=" and fo.id_language="+webSession.SiteLanguage+" ";
//                sql+=" and fo.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
//            }
			
//            sql+=" and wp.id_product in( ";

//            sql+=" select distinct id_product ";
//            sql+=" from   "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()))+" wp ";
//            if(webSession.NewProduct==TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support){
//                // Cas nouveau dans le support	
//                sql+=" where new_product in (1,2) ";
//            }
//                // cas dans la pige
//            else {
//                sql+=" where new_product=2 ";
//            }
			
//            sql+=" and id_media="+idMedia+" ";
//            sql+=" and  DATE_MEDIA_NUM>= "+dateBegin+" ";
//            sql+=" and  DATE_MEDIA_NUM<= "+dateEnd+" ";
//        //	sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";
//            sql+=product;
//            sql+=productsRights;
//            sql+=mediaRights;
//            sql+=listProductHap;
 
//            sql+=" ) ";

//            sql+=product;
//            sql+=productsRights;
//            sql+=mediaRights;
//            sql+=listProductHap;

//            sql+=" group by wp.id_product,pr.product ";
////			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.press
////				|| (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.internationalPress){
////				
////				sql+=",color,format ";
////			}
			
//            sql+=" order by product ";
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour les nouveauté: "+sql,err));
//            }
//            #endregion
			
//        }
//        #endregion

//        #region Structure

//        #region Dataset pour tv ou radio
//        /// <summary>
//        /// Obtient les données pour la Structure du média télé ou radio
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="dateBegin">date de début</param>
//        /// <param name="dateEnd">date de fin</param>
//        /// <param name="HourBegin">heure de début</param>
//        /// <param name="HourEnd">heure de fin</param>
//        /// <returns>groupe de données</returns>
//        public static DataSet GetTvOrRadioStructData(WebSession webSession,int dateBegin,int dateEnd,int HourBegin,int HourEnd){			
			
//            #region variables
//            string tableName="";
//            string fields = "";			
//            //string list="";
//            //bool premier = true;
//            string idVehicle="";
//            string idMedia ="";
//            string sql="";
//            //DataSet  ds=null;
//            //produits sélectionnés
//            string product="";
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion
//            #endregion
			
//            #region construction de la requête
//            try{
//                #region Sélection du vehicle et du support
//                //ID média sélectionné
//                idVehicle=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);								
//                //Id support sélectionné
//                idMedia =webSession.GetSelection(webSession.ReferenceUniversMedia,CustomerRightConstante.type.mediaAccess);											
//                #endregion

//                //Nom de la table
//                tableName = GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));				
//                //Champs demandés
//                fields = GetTvOrRadioStructFields();							
//            }
//            catch(Exception){
//                throw new Exceptions.PortofolioDataAccessException("getPressStructFields(PortofolioStructure.Ventilation ventilation) or getTvOrRadioStructFields()--> Impossible de déterminer la table ou les champs à traiter.");
//            }

//            if(WebFunctions.CheckedText.IsStringEmpty(tableName.ToString().ToString())){
//                // Sélection de la nomenclature Support
//                sql+=" select " + fields;
//                // Tables
//                sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp ";
//                sql+=" where ";
//                //Choix de la langue
//        //		sql+=" wp.id_language_data_i ="+webSession.SiteLanguage;
//                // Période
//                sql+="  wp.date_media_num>="+dateBegin;
//                sql+=" and wp.date_media_num<="+dateEnd;
//                // Tranche horaire
//                sql+=GetHourInterval(HourBegin,HourEnd,(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));

//                #region Nomenclature Produit (droits)
//                //premier=true;
//                //Droits en accès
//                sql+=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
//                //liste des produit hap
//                string listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
//                if(WebFunctions.CheckedText.IsStringEmpty(listProductHap.ToString().Trim()))		
//                sql+=listProductHap;
//                //Liste des produits sélectionnés
//                product=GetProductData(webSession);
//                if(WebFunctions.CheckedText.IsStringEmpty(product.ToString().Trim()))
//                    sql+=product;
//                #endregion

//                #region Nomenclature Media (droits et sélection)

//                #region Droits
//                sql+=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true);
//                #endregion

//                #region Sélection média	
//                //sélection média (vehicle)
//                if(WebFunctions.CheckedText.IsStringEmpty(idVehicle.ToString().Trim()))					
//                    sql+=" and wp.id_vehicle="+idVehicle.ToString();
//                //sélection support	
//                if(WebFunctions.CheckedText.IsStringEmpty(idMedia.ToString().Trim()))					
//                    sql+=" and wp.id_media="+idMedia.ToString();								
//                #endregion

//                #endregion
				
//            }
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la structure: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #region DataSet pour presse
//        /// <summary>
//        ///  Obtient les données pour la Structure du média presse
//        /// </summary>
//        /// <param name="webSession">session du client</param>
//        /// <param name="dateBegin">date de début</param>
//        /// <param name="dateEnd">date de fin</param>
//        /// <param name="ventilation">format ou couleur ou emplacements ou encarts</param>
//        /// <returns>Groupe de données</returns>
//        public static DataSet GetPressStructData(WebSession webSession,int dateBegin,int dateEnd,PortofolioStructure.Ventilation ventilation){			
			
//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

//            #region variables
//            string tableName="";
//            string fields = "";			
//            //string list="";
//            //bool premier = true;
//            string idVehicle="";
//            string idMedia ="";
//            string sql="";
//            //DataSet  ds=null;
//            //produits sélectionnés
//            string product="";
//            #endregion
		
//            #region construction de la requête
//            try{
//                #region identifiants du vehicle et du support
//                //ID média sélectionné
//                idVehicle=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);								
//                //Id support sélectionné
//                idMedia =webSession.GetSelection(webSession.ReferenceUniversMedia,CustomerRightConstante.type.mediaAccess);											
//                #endregion

//                //Nom de la table
//                tableName = GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));								
//                //Champs demandés
//                fields = GetPressStructFields(ventilation);
//            }
//            catch(Exception){
//                throw new Exceptions.PortofolioDataAccessException("getPressStructFields(PortofolioStructure.Ventilation ventilation) or getTvOrRadioStructFields()--> Impossible de déterminer la table ou les champs à traiter.");
//            }

//            if(WebFunctions.CheckedText.IsStringEmpty(tableName.ToString().Trim())){
//                // Sélection de la nomenclature Support
//                sql+=" select " + fields;
//                // Tables
//                sql+=" from ";
//                sql+=GetPressStructTables(tableName,ventilation);
//                sql+=" where ";
//                //Choix de la langue
//                sql+=GetPressStructLanguage(webSession,ventilation);
//                //Jointures
//                sql+=GetPressStructJoint(ventilation,dateBegin,dateEnd);				
//                // Période
//                sql+=" and wp.date_media_num>="+dateBegin;
//                sql+=" and wp.date_media_num<="+dateEnd;
				
//                #region Nomenclature Produit (droits)
//                //premier=true;
//                //Droits en accès
//                sql+=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
//                //liste des produit hap
//                string listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
//                if(WebFunctions.CheckedText.IsStringEmpty(listProductHap.ToString().Trim()))		
//                    sql+=listProductHap;
//                //Liste des produits sélectionnés
//                product=GetProductData(webSession);
//                if(WebFunctions.CheckedText.IsStringEmpty(product.ToString().Trim()))
//                    sql+=product;
//                #endregion

//                #region Sélection média	

//                #region Droits média
//                sql+=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true);								
//                #endregion

//                //sélection média (vehicle)
//                if(WebFunctions.CheckedText.IsStringEmpty(idVehicle.ToString().Trim()))					
//                    sql+=" and wp.id_vehicle="+idVehicle.ToString();
//                //sélection support	
//                if(WebFunctions.CheckedText.IsStringEmpty(idMedia.ToString().Trim()))					
//                    sql+=" and wp.id_media="+idMedia.ToString();								
//                #endregion

//                #region regroupement
//                sql+=GetPressStructGroupBy(ventilation);
//                #endregion

//                //tri
//                sql+=" order by insertion desc";
//            }
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour la structure: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #endregion

//        #region Calendrier d'actions
//        /// <summary>
//        /// Charge les données pour le calendrier d'action
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="vehicleName">Média à traiter</param>
//        /// <param name="moduleType">Type du module</param>
//        /// <param name="beginingDate">Date de début</param>
//        /// <param name="endDate">Date de fin</param>
//        /// <returns>Données de l'alerte concurrentielle</returns>
//        public static DataSet GetDataCalendar(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName,WebConstantes.Module.Type moduleType, string beginingDate, string endDate){

//            #region Constantes
//            //préfixe table à utiliser
//            const string DATA_TABLE_PREFIXE="wp";
//            #endregion

//            #region Variables
//            string dataTableName="";
//            string dataTableNameForGad="";
//            string dataFieldsForGad="";
//            string dataJointForGad="";
//            string detailProductTablesNames="";
//            string detailProductFields="";
//            string detailProductJoints="";
//            string detailProductOrderBy="";
//            string unitField="";
//            string productsRights="";
//            string sql="";
//            string list="";
//            //int positionUnivers=1;
//            string mediaList="";			
//            bool premier;
//            string dataJointForInsert="";
//            string listProductHap="";
//            string mediaRights="";
//            string mediaAgencyTable=string.Empty;
//            string mediaAgencyJoins=string.Empty;
//            #endregion

//            #region Construction de la requête
//            try{
//                dataTableName=WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleName,moduleType);
//                detailProductTablesNames=webSession.GenericProductDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
//                detailProductFields=webSession.GenericProductDetailLevel.GetSqlFields();
//                detailProductJoints=webSession.GenericProductDetailLevel.GetSqlJoins(webSession.SiteLanguage,DATA_TABLE_PREFIXE);
//                unitField = WebFunctions.SQLGenerator.GetUnitFieldName(webSession);
//                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
//                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
//                detailProductOrderBy=webSession.GenericProductDetailLevel.GetSqlOrderFields();
//                //option encarts (pour la presse)
//                if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
//                    dataJointForInsert=WebFunctions.SQLGenerator.GetJointForInsertDetail(webSession,DATA_TABLE_PREFIXE);
//                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				
//                if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)){
//                    try{
//                        dataTableNameForGad=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GAD+" "+DBConstantes.Tables.GAD_PREFIXE;
//                        dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
//                        dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);
//                    }
//                    catch(SQLGeneratorException){;}
//                }
//                ////Agence_media
//                //if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency)||webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)){
//                //    mediaAgencyTable=DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
//                //    mediaAgencyJoins="And "+DATA_TABLE_PREFIXE+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
//                //    mediaAgencyJoins+="And "+DATA_TABLE_PREFIXE+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
//                //}
//            } 
//            catch(Exception e){
//                throw(new PortofolioDataAccessException("Impossible d'initialiser les paramètres de la requêtes"+e.Message));
//            }
	
//            sql+=" select "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad+",sum("+unitField+") as unit";
//            sql+=", date_media_num";
//            sql+=" from "+mediaAgencyTable+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DATA_TABLE_PREFIXE;
//            if (detailProductTablesNames.Length > 0)
//                sql+=", "+detailProductTablesNames;
//            sql+=" "+dataTableNameForGad;
//            // Période
//            sql+=" where date_media_num >="+beginingDate;
//            sql+=" and date_media_num <="+endDate;
//            // Jointures Produits
//            sql+=" "+detailProductJoints;
//            sql+=" "+dataJointForGad;
//            sql+=" "+mediaAgencyJoins;
//            //Jointures encart
//            if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
//                sql+=" "+dataJointForInsert;
			
//            #region Sélection de Médias
//            mediaList+=webSession.GetSelection((TreeNode) webSession.ReferenceUniversMedia,CustormerConstantes.Right.type.mediaAccess);						
//            if (mediaList.Length>0)sql+=" and "+DATA_TABLE_PREFIXE+".id_media in ("+mediaList+")";
//            #endregion
		
//            #region Ancienne version Sélection de Produits
//            //// Sélection en accès
//            //premier=true;
//            //// Sector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorAccess);
//            //if(list.Length>0){
//            //    sql+=" and ((wp.id_sector in ("+list+") ";
//            //    premier=false;
//            //}
//            //// SubSector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorAccess);
//            //if(list.Length>0){
//            //    if(!premier) sql+=" or";
//            //    else sql+=" and ((";
//            //    sql+=" wp.id_subsector in ("+list+") ";
//            //    premier=false;
//            //}
//            //// Group
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupAccess);
//            //if(list.Length>0){
//            //    if(!premier) sql+=" or";
//            //    else sql+=" and ((";
//            //    sql+=" wp.id_group_ in ("+list+") ";
//            //    premier=false;
//            //}

//            //if(!premier) sql+=" )";
			
//            //// Sélection en Exception
//            //// Sector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_sector not in ("+list+") ";
//            //    premier=false;
//            //}
//            //// SubSector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_subsector not in ("+list+") ";
//            //    premier=false;
//            //}
//            //// Group
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_group_ not in ("+list+") ";
//            //    premier=false;
//            //}
//            //if(!premier) sql+=" )";
//            #endregion

//            #region Sélection de Produits
//            sql += " " + GetProductData(webSession);
//            #endregion

//            // Droits des Médias
//            // Droits des Produits
//            sql+=" "+productsRights;
//            sql+=mediaRights;
//            sql+=listProductHap;
//            // Group by
//            sql+=" group by "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad;
//            sql+=",date_media_num";
//            // Order by
//            sql+=" order by "+detailProductOrderBy+","+DATA_TABLE_PREFIXE+".id_media";
//            sql+=",date_media_num";
//            #endregion

//            #region Execution de la requête
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.PortofolioDataAccessException ("Impossible de charger des données pour les nouveauté: "+sql,err));
//            }
//            #endregion

//        }
//        #endregion

//        #region Méthodes internes
//        /// <summary>
//        /// Partie select de la requête pour la planche 
//        /// Nouveauté
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="idVehicle">Identifiant du media (vehicle)</param>
//        /// <returns>SQL</returns>
//        protected static string GetSelectNewProduct(WebSession webSession,DBClassificationConstantes.Vehicles.names idVehicle){
//            string sql="";
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                case DBClassificationConstantes.Vehicles.names.press:
//                    sql="select  distinct wp.id_product";
//                    sql+=" , sum(wp.expenditure_euro) as valeur ";
//                    //sql+=" ,format as format ";
//                    //,sum(media_paging) as pages";
//                    sql+=" ,sum (insertion) as insertion ";
//                    sql+=" ,pr.product as produit ";
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    sql=" select distinct wp.id_product ";
//                    sql+=" ,pr.product as produit ";
//                    sql+=" ,sum(wp.expenditure_euro) as valeur ";
//                    sql+=" ,sum (insertion) as insertion ";
//                    sql+=" ,sum(duration) as duree ";
//                    return	sql;					
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                    sql=" select distinct wp.id_product ";
//                    sql+=" ,pr.product as produit ";
//                    sql+=" ,sum(wp.expenditure_euro) as valeur ";
//                    sql+=" ,sum (insertion) as insertion ";
//                    sql+=" ,sum(duration) as duree ";
//                    return sql;
//                    //				case DBClassificationConstantes.Vehicles.names.outdoor:
//                    //					sql=" select distinct wp.id_product ";
//                    //					sql+=" ,pr.product as produit ";
//                    //					sql+=" ,sum(wp.expenditure_euro) as valeur ";
//                    //					sql+=" ,sum (number_board) as number_board ";
//                    //					return sql;
//                    //						
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }
		
//        }
		
//        /// <summary>
//        /// Partie table pour la planche nouveauté
//        /// </summary>
//        /// <param name="idVehicle">Identifiant du media (vehicle)</param>
//        /// <returns>SQL</returns>
//        private static string getTableDataNewProduct(DBClassificationConstantes.Vehicles.names idVehicle){
//            string sql="";
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                    sql+= DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.ALERT_DATA_PRESS_INTER +" wp";
//                    sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr ";
//                    sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".color co ";
//                    sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".format fo ";
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.press:
//                    sql+= DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.ALERT_DATA_PRESS +" wp";
//                    sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr ";
//                    sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".color co ";
//                    sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".format fo ";
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    sql+= DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.ALERT_DATA_RADIO +" wp";
//                    sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr ";
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                    sql+= DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.ALERT_DATA_TV +" wp";
//                    sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr ";
//                    return sql;
//                    //				case DBClassificationConstantes.Vehicles.names.outdoor:
//                    //					sql+= DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.ALERT_DATA_OUTDOOR +" wp";
//                    //					sql+=" ,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr ";
//                    //					return sql;

//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }
//        }

//        /// <summary>
//        /// Obtient le select pour les ecran
//        /// </summary>
//        /// <param name="idVehicle">Identifiant du media (vehicle)</param>
//        /// <returns>SQL</returns>
//        protected static string GetSelectDataEcran(DBClassificationConstantes.Vehicles.names idVehicle){
//            string sql="";	
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                case DBClassificationConstantes.Vehicles.names.press:
//                case DBClassificationConstantes.Vehicles.names.internet:
//                    return "";
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    sql+=" select  distinct ID_COBRANDING_ADVERTISER";
//                    sql+=" ,duration_commercial_break as ecran_duration";
//                    sql+=" , NUMBER_spot_com_break nbre_spot";
//                    sql+=" , insertion ";

//                    return	sql;
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                    sql+="select  distinct id_commercial_break ";
//                    sql+=" ,duration_commercial_break as ecran_duration";
//                    sql+=" ,NUMBER_MESSAGE_COMMERCIAL_BREA nbre_spot "; 
//                    sql+=" ,insertion ";					
//                    return sql;
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }
//        }

//        /// <summary>
//        /// Obtient les Champs
//        /// </summary>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <returns>SQL</returns>
//        private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle){
//            string sql="category,media_seller,interest_center,media as support ";
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                case DBClassificationConstantes.Vehicles.names.press:
//                    sql+=", periodicity ";
//                    //	,number_printing_sold_year as ojd";
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.radio:					
//                    return sql;														   
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.outdoor:
//                case DBClassificationConstantes.Vehicles.names.internet:
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.directMarketing:
//                    return sql;
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }
//        }

//        /// <summary>
//        /// Obtient les Jointures
//        /// </summary>
//        /// <param name="idVehicle">identifiant vehicle</param>
//        /// <param name="webSession">session client</param>
//        /// <returns>SQL</returns>
//        private static string GetJoint(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSession){
//            string sql= "and me.id_basic_media=bm.id_basic_media";
//            sql+=" and me.id_media_seller=ms.id_media_seller";
//            sql+=" and bm.id_category=ca.id_category ";
//            sql+=" and me.id_interest_center=ic.id_interest_center";
//            sql+=" and me.id_language="+webSession.SiteLanguage+" ";			
//            sql+=" and bm.id_language="+webSession.SiteLanguage+" ";
//            sql+=" and ms.id_language="+webSession.SiteLanguage+" ";
//            sql+=" and ca.id_language="+webSession.SiteLanguage+" ";
//            sql+=" and ic.id_language="+webSession.SiteLanguage+" ";
//            //	sql+=" and mc.id_language="+webSession.SiteLanguage+" ";

//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                case DBClassificationConstantes.Vehicles.names.press:
//                    sql+="and pe.id_periodicity=me.id_periodicity";
//                    //sql+=" and me.id_media=mc.id_media(+)";
//                    sql+=" and pe.id_language="+webSession.SiteLanguage+" ";
//                    //sql+=" and pe.year="+webSession.SiteLanguage+" ";
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    return sql;														   
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                case DBClassificationConstantes.Vehicles.names.outdoor:
//                case DBClassificationConstantes.Vehicles.names.internet:
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.directMarketing:
//                    return sql;
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }
//        }
		
//        /// <summary>
//        /// Obtient les Tables 
//        /// </summary>
//        /// <param name="idVehicle">identidiant vehicle</param>
//        /// <returns>Obtient le nom de la table</returns>
//        private static string GetTable(DBClassificationConstantes.Vehicles.names idVehicle){
//            string sql="";
//            sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".media me,";			
//            sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media bm,";
//            sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".media_seller ms,";
//            sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".category ca, ";
//            sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".interest_center ic ";
			
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                case DBClassificationConstantes.Vehicles.names.press:
//                    sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".periodicity pe ";
//                    //	sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".media_circulation mc ";
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                case DBClassificationConstantes.Vehicles.names.outdoor:
//                case DBClassificationConstantes.Vehicles.names.internet:
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.directMarketing:
//                    return sql;
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }
//        }

//        /// <summary>
//        /// Donne les champs à utiliser pour le vehicle radio ou télé
//        /// </summary>		
//        /// <exception>
//        /// Lancée quand le cas du vehicle spécifié n'est pas traité
//        /// </exception>
//        /// <returns>Chaine contenant les champs sélectionnés</returns>
//        private static string GetTvOrRadioStructFields(){			
//            return " sum(wp.expenditure_euro) as euros"																
//                +", sum(wp.insertion) as spot"
//                +", sum(wp.duration) as duration";																			
//        }

//        /// <summary>
//        /// Donne les champs à utiliser pour le vehicle presse
//        /// </summary>
//        /// <param name="ventilation">une ventilation par format, type de couleur,encarts</param>		
//        /// <exception>
//        /// Lancée quand le cas du vehicle spécifié n'est pas traité
//        /// </exception>
//        /// <returns>Chaine contenant les champs sélectionnés</returns>
//        private static string GetPressStructFields(PortofolioStructure.Ventilation ventilation){
//            switch(ventilation){
//                case PortofolioStructure.Ventilation.color :
//                    return " color "
//                    +", sum(wp.insertion) as insertion";
//                case PortofolioStructure.Ventilation.format :
//                    return " format "
//                    +", sum(wp.insertion) as insertion";
//                case PortofolioStructure.Ventilation.insert :
//                    return " inset "
//                    +", sum(wp.insertion) as insertion";
//                case PortofolioStructure.Ventilation.location :
//                    return " location "
//                    +", sum(wp.insertion) as insertion";				
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getPressStructFields(PortofolioStructure.Ventilation ventilation)--> Pas de ventilation (format, couleur) déterminé.");
//            }				
//        }
//        /// <summary>
//        ///	Regroupe les champs à utiliser pour le vehicle presse
//        /// </summary>
//        /// <param name="ventilation">une ventilation par format, type de couleur,encarts</param>		
//        /// <exception>
//        /// Lancée quand le cas du vehicle spécifié n'est pas traité
//        /// </exception>
//        /// <returns>Chaine contenant les champs sélectionnés groupées</returns>
//        private static string GetPressStructGroupBy(PortofolioStructure.Ventilation ventilation){
//            switch(ventilation){
//                case PortofolioStructure.Ventilation.color :
//                    return " group by color ";
//                case PortofolioStructure.Ventilation.format :
//                    return " group by format ";						
//                case PortofolioStructure.Ventilation.insert :
//                    return " group by inset ";						
//                case PortofolioStructure.Ventilation.location :
//                    return " group by location ";								
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getPressStructGroupBy(PortofolioStructure.Ventilation ventilation)--> Pas de ventilation (format, couleur) déterminé.");
//            }				
//        }
		
//        /// <summary>
//        /// Obtient la tranche horaire utilisée.
//        /// </summary>
//        /// <param name="HourBegin">heure de début</param>
//        /// <param name="HourEnd">heure de fin</param>
//        /// <param name="idVehicle">identifiant du média</param>
//        /// <returns>tranche horaire de diffusion</returns>
//        private static string GetHourInterval(int HourBegin,int HourEnd,DBClassificationConstantes.Vehicles.names idVehicle){
//            string sql="";
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.radio : 
//                    sql+=" and wp.id_top_diffusion>="+HourBegin;
//                    sql+=" and wp.id_top_diffusion<="+HourEnd;
//                    return sql;
//                case DBClassificationConstantes.Vehicles.names.tv :
//                case DBClassificationConstantes.Vehicles.names.others :
//                    sql+=" and wp.top_diffusion>="+HourBegin;
//                    sql+=" and wp.top_diffusion<="+HourEnd;
//                    return sql;
//                default :
//                    throw new Exceptions.PortofolioDataAccessException("getHourInterval(int HourBegin,int HourEnd,DBClassificationConstantes.Vehicles.names idVehicle)--> Impossible de déterminer le typde de média.");
//            }			
//        }
//        /// <summary>
//        /// Obtient les tables à utiliser pour la presse
//        /// </summary>
//        /// <param name="ventilation">format ou couleur ou emplacements ou encarts</param>
//        /// <param name="tableName">nom de la table média sélectionné</param>
//        /// <returns>tables</returns>
//        private static string GetPressStructTables(string tableName,PortofolioStructure.Ventilation ventilation){
//            switch(ventilation){
//                case PortofolioStructure.Ventilation.color :
//                    return " "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp "
//                    +", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.COLOR+" co ";					
//                case PortofolioStructure.Ventilation.format :
//                        return " "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp "												
//                        +" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.FORMAT+" fo ";		
//                case PortofolioStructure.Ventilation.insert :
//                        return " "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp "	
//                        +" , " +DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.INSERT+" srt ";
//                case PortofolioStructure.Ventilation.location:
//                    return " "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp "	
//                    +" , " +DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.LOCATION+" lo "
//                    + " , " + DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_LOCATION+"  dl ";
//                default :
//                    throw new Exceptions.PortofolioDataAccessException("getPressStructTables(PortofolioStructure.Ventilation ventilation)--> Impossible de déterminer le type de ventilation pour le média presse.");
//            }
//        }

//        /// <summary>
//        /// Effectue les jointures via l'identifiant de la langue de l'utilisateur
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="ventilation">format ou couleur ou emplacements ou encarts</param>
//        /// <returns>jointures langue</returns>
//        private static string GetPressStructLanguage(WebSession webSession,PortofolioStructure.Ventilation ventilation){
//            switch(ventilation){
//                case PortofolioStructure.Ventilation.color :
//                    return " co.id_language = "+webSession.SiteLanguage
//                        +" and co.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
//                case PortofolioStructure.Ventilation.format :
//                    return " fo.id_language="+webSession.SiteLanguage
//                        +" and fo.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
//                case PortofolioStructure.Ventilation.insert :
//                    return " srt.id_language="+webSession.SiteLanguage
//                        +" and srt.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";	   
//                case PortofolioStructure.Ventilation.location:
//                    return " lo.id_language="+webSession.SiteLanguage
//                        +" and dl.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+""
//                        +" and lo.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
//                default :
//                    throw new Exceptions.PortofolioDataAccessException("getPressStructLanguage(WebSession webSession,PortofolioStructure.Ventilation ventilation)--> Impossible de déterminer le type de language pour la presse.");
//            }
//        }
		
//        /// <summary>
//        /// Effectue les jointures pour la presse
//        /// </summary>
//        /// <param name="ventilation">format ou couleur ou emplacements ou encarts</param>
//        /// <param name="dateBegin">date de début</param>
//        /// <param name="dateEnd">date de fin</param>
//        /// <returns>champq joints</returns>
//        private static string GetPressStructJoint(PortofolioStructure.Ventilation ventilation,int dateBegin,int dateEnd){
//            switch(ventilation){
//                case PortofolioStructure.Ventilation.color :
//                    return " and   wp.id_color = co.id_color ";						
//                case PortofolioStructure.Ventilation.format :
//                    return " and  wp.id_format =fo.id_format";																	
//                case PortofolioStructure.Ventilation.insert :
//                    return " and  wp.id_inset = srt.id_inset"
//                     +" and wp.id_inset in ( " +WebConstantes.CustomerSessions.InsertType.encart.GetHashCode()+","+WebConstantes.CustomerSessions.InsertType.flyingEncart.GetHashCode()+ " )"				
//                    +" and srt.id_inset in ( " +WebConstantes.CustomerSessions.InsertType.encart.GetHashCode()+","+WebConstantes.CustomerSessions.InsertType.flyingEncart.GetHashCode()+ " )";				
//                case PortofolioStructure.Ventilation.location:
//                    return " and  wp.id_media = dl.id_media "
//                        +" and lo.id_location=dl.id_location "
//                    // Période
//                    + " and wp.date_media_num=dl.date_media_num "
//                    +"  and dl.ID_ADVERTISEMENT=wp.ID_ADVERTISEMENT ";
//                default :
//                    throw new Exceptions.PortofolioDataAccessException("getPressStructJoint(PortofolioStructure.Ventilation ventilation)--> Impossible d'effectuer des jointures pour la presse.");
//            }
//        }
//        /// <summary>
//        /// Donne la table à utiliser pour le vehicle indiqué
//        /// </summary>
//        /// <param name="idVehicle">Identifiant du vehicle</param>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
//        /// Lancée quand le cas du vehicle spécifié n'est pas traité
//        /// </exception>
//        /// <returns>Chaine contenant le nom de la table correspondante</returns>
//        private static string GetTableData(DBClassificationConstantes.Vehicles.names idVehicle){
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                    return DBConstantes.Tables.ALERT_DATA_PRESS_INTER;
//                case DBClassificationConstantes.Vehicles.names.press:
//                    return DBConstantes.Tables.ALERT_DATA_PRESS;
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    return DBConstantes.Tables.ALERT_DATA_RADIO;
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                    return DBConstantes.Tables.ALERT_DATA_TV;
//                case DBClassificationConstantes.Vehicles.names.outdoor:
//                    return DBConstantes.Tables.ALERT_DATA_OUTDOOR;
//                case DBClassificationConstantes.Vehicles.names.directMarketing:
//                    return DBConstantes.Tables.ALERT_DATA_MARKETING_DIRECT;
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }		
//        }
		
//        /// <summary>
//        /// Obtient Select
//        /// </summary>
//        /// <param name="idVehicle">Identifiant du media (vehicle)</param>
//        /// <returns>SQL</returns>
//        private static string GetSelectData(DBClassificationConstantes.Vehicles.names idVehicle){
//            switch(idVehicle){
//                case DBClassificationConstantes.Vehicles.names.internationalPress:
//                case DBClassificationConstantes.Vehicles.names.press:
//                    return " select sum(EXPENDITURE_EURO) as investment,min(DATE_MEDIA_NUM) first_date,max(DATE_MEDIA_NUM) last_date ";
//                case DBClassificationConstantes.Vehicles.names.radio:
//                    return	" select sum(EXPENDITURE_EURO) as investment,min(DATE_MEDIA_NUM) first_date,max(DATE_MEDIA_NUM) last_date, "
//                        +" sum(insertion) as insertion,sum(duration) as duration";
//                case DBClassificationConstantes.Vehicles.names.tv:
//                case DBClassificationConstantes.Vehicles.names.others:
//                    return " select sum(EXPENDITURE_EURO) as investment,min(DATE_MEDIA_NUM) first_date,max(DATE_MEDIA_NUM) last_date, "
//                        +" sum(insertion) as insertion,sum(duration) as duration";
//                case DBClassificationConstantes.Vehicles.names.outdoor:
//                    return " select sum(EXPENDITURE_EURO) as investment,min(DATE_CAMPAIGN_BEGINNING) first_date,max(DATE_CAMPAIGN_END) last_date, "
//                        +" sum(NUMBER_BOARD) as number_board ";
//                default:
//                    throw new Exceptions.PortofolioDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
//            }
//        }

//        /// <summary>
//        /// Récupère la liste produit de référence
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <returns>la liste produit de référence</returns>
//        internal static string GetProductData(WebSession webSession){
//            bool premier;
//            string list;
//            string sql="";

//            #region Ancienne version Sélection de Produits
//            //// Sélection en accès
//            //premier=true;
//            //// Sector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorAccess);
//            //if(list.Length>0){
//            //    sql+=" and ((wp.id_sector in ("+list+") ";
//            //    premier=false;
//            //}
//            //// SubSector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorAccess);
//            //if(list.Length>0){
//            //    if(!premier) sql+=" or";
//            //    else sql+=" and ((";
//            //    sql+=" wp.id_subsector in ("+list+") ";
//            //    premier=false;
//            //}
//            //// Group
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupAccess);
//            //if(list.Length>0){
//            //    if(!premier) sql+=" or";
//            //    else sql+=" and ((";
//            //    sql+=" wp.id_group_ in ("+list+") ";
//            //    premier=false;
//            //}

//            //if(!premier) sql+=" )";
			
//            //// Sélection en Exception
//            //// Sector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_sector not in ("+list+") ";
//            //    premier=false;
//            //}
//            //// SubSector
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_subsector not in ("+list+") ";
//            //    premier=false;
//            //}
//            //// Group
//            //list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupException);
//            //if(list.Length>0){
//            //    if(premier) sql+=" and (";
//            //    else sql+=" and";
//            //    sql+=" wp.id_group_ not in ("+list+") ";
//            //    premier=false;
//            //}
//            //if(!premier) sql+=" )";
//            #endregion

//            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
//                sql= webSession.PrincipalProductUniverses[0].GetSqlConditions("wp", true);

//            return sql;
//        }

//        /// <summary>
//        /// Champs de la requête perfomance
//        /// </summary>
//        /// <param name="dataTablePrefixe">préfixe de la table</param>
//        /// <returns>les champs à sélectionner</returns>
//        private static string getPerformancesFields(string dataTablePrefixe){
//            return(dataTablePrefixe+".date_media_num as date_media_num,"+"sum("+dataTablePrefixe+".expenditure_euro) as euro,sum("+dataTablePrefixe+".area_mmc) as mmPerCol,sum("+dataTablePrefixe+".area_page) as pages,sum("+dataTablePrefixe+".insertion) as insertion ");				
//        }


//        #endregion		

//    }
//}
