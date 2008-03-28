#region Informations
// Auteur: 
// Date de cr�ation: 
// Date de modification: 
//	22/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	28/10/2005	D. V. Mussuma	centralisation des unit�s	
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
#endregion

#region Using
using System;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;
using AccessCst = TNS.AdExpress.Constantes.Customer.Right.type;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using DBTableFieldsName = TNS.AdExpress.Constantes.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebCommon=TNS.AdExpress.Web.Common;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Navigation;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	///  Extraction des donn�es d'alerte plan m�dia concurentiel de la BD
	/// </summary>
	public class CompetitorMediaPlanAlertDataAccess {

		#region Ancienne version
//        /// <summary>
//        /// Charge les donn�es pour cr�er un plan m�dia concurentiel d'UN vehicle
//        ///		Assignation du nom de la table � attaquer et de l'unit� � s�lectionner
//        ///		Construction et ex�cution de la requ�te
//        /// </summary>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
//        /// Lanc�e si aucune table ne correspond au vehicle,si l'unit� n'est pas valide ou si une erreur Oracle est survenue
//        /// </exception>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="idVehicle">Identifiant du vehicle � traiter</param>
//        /// <param name="beginingDate">Date de d�but</param>
//        /// <param name="endDate">Date de fin</param>
//        /// <returns>Dataset contenant les donn�es rapatri�es de la base de donn�es pour le vehicule consid�r�</returns>
//        /// <remarks>
//        /// Utilise les m�thodes:
//        ///		private static string getTable(DBClassificationConstantes.Vehicles.names idVehicle)
//        ///		private static string getField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit unit)
//        ///		public static string TNS.AdExpress.Web.Functions.SqlGenerator.getAnalyseCustomerProductRight(WebSession webSession,string tablePrefixe,bool beginByAnd)
//        ///		public static string TNS.AdExpress.Web.Functions.SqlGenerator.getAnalyseCustomerMediaRight(WebSession webSession,string tablePrefixe,bool beginByAnd)
//        ///		public string TNS.AdExpress.Web.Core.Sessions.WebSession.GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
//        /// </remarks>
//        public static DataSet GetData(WebSession webSession,string idVehicle, string beginingDate, string endDate){

//            #region Variables
//            string list="";
//            string tableName="";
//            string unitField="";
//            bool premier = true;
//            int positionUnivers=1;
//            #endregion

//            #region R�cup�ration des noms de tables et de champs suivant le vehicule
//            try{
//                //tableName = getTable((DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)));
//                Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
//                tableName=WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult((DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)),currentModuleDescription.ModuleType);
//                //unitField = getField(webSession.Unit);
//                if(DBClassificationConstantes.Vehicles.names.outdoor ==(DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)) 
//                    && webSession.Unit== TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion){
//                    unitField = WebFunctions.SQLGenerator.getUnitField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.numberBoard);
//                }else
//                    unitField = WebFunctions.SQLGenerator.getUnitField(webSession.Unit);
//            }
//            catch(System.Exception exc){
//                throw new CompetitorMediaPlanAlertDataAccessException("MedaiPlanAlertDataAccess.getData(WebSession webSession,string idVehicle, string beginingDate, string endDate) : \n\t",exc);
//            }
//            #endregion

//            #region Construction de la requ�te
//            string sql="";		
//            sql+=" select grp,grpUnivers, id_vehicle, vehicle, id_category, category, id_media, media,id_periodicity,date_num,unit";
//            sql+=" from (";
//            while(webSession.CompetitorUniversAdvertiser[positionUnivers]!=null){
//                if(positionUnivers>1){
//                    sql+=" UNION ";
//                }
//                sql+=" select '"+((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).NameCompetitorAdvertiser+"' as grp , ";
//                sql+=" "+positionUnivers+" as grpUnivers ,wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity";

//                // S�lection de la date
//                sql+=", date_media_num as date_num";
////				if(webSession.Unit== TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.pages) sql+=", (sum("+unitField+"))/1000 as unit";
////				else sql+=", sum("+unitField+") as unit";
//                sql+=", sum("+unitField+") as unit";

//                // Tables
//                sql+=" from "+DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"vehicle vh, "
//                    +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"category ct, "
//                    +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+"media md, "
//                    +DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp ";

//                // Conditions de jointure
//                sql+=" Where vh.id_vehicle=wp.id_vehicle ";
//                sql+=" and ct.id_category=wp.id_category ";
//                sql+=" and md.id_media=wp.id_media ";
//                sql+=" and vh.id_language="+webSession.SiteLanguage.ToString();
//                sql+=" and ct.id_language="+webSession.SiteLanguage.ToString();
//                sql+=" and md.id_language="+webSession.SiteLanguage.ToString();
//                // Activation 
//                sql+=" and vh.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//                sql+=" and ct.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//                sql+=" and md.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
//                // P�riode
//                sql+=" and date_media_num >="+beginingDate;
//                sql+=" and date_media_num <="+endDate;

//                #region Nomenclature Produit (droits)
//                premier=true;
//                //Droits en acc�s
//                sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
//                // Produit � exclure en radio
//                sql+=SQLGenerator.getAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,"wp",true,false);
//                #endregion
				
//                #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et s�lection) 

//                #region S�lection
//                // S�lection en acc�s
//                premier=true;
//                // HoldingCompany
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
//                if(list.Length>0){
//                    sql+=" and ((wp.id_holding_company in ("+list+") ";
//                    premier=false;
//                }
//                // Advertiser
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.advertiserAccess);
//                if(list.Length>0){
//                    if(!premier) sql+=" or";
//                    else sql+=" and ((";
//                    sql+=" wp.id_advertiser in ("+list+") ";
//                    premier=false;
//                }
//                // Marque
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.brandAccess);
//                if(list.Length>0)
//                {
//                    if(!premier) sql+=" or";
//                    else sql+=" and ((";
//                    sql+=" wp.id_brand in ("+list+") ";
//                    premier=false;
//                }
//                // Product
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.productAccess);
//                if(list.Length>0){
//                    if(!premier) sql+=" or";
//                    else sql+=" and ((";
//                    sql+=" wp.id_product in ("+list+") ";
//                    premier=false;
//                }
//                // Sector
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.sectorAccess);
//                if(list.Length>0){
//                    sql+=" and ((wp.id_sector in ("+list+") ";
//                    premier=false;
//                }
//                // subsector
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.subSectorAccess);
//                if(list.Length>0){
//                    if(!premier) sql+=" or";
//                    else sql+=" and ((";
//                    sql+=" wp.id_subsector in ("+list+") ";
//                    premier=false;
//                }
//                // Group
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.groupAccess);
//                if(list.Length>0){
//                    if(!premier) sql+=" or";
//                    else sql+=" and ((";
//                    sql+=" wp.id_group_ in ("+list+") ";
//                    premier=false;
//                }
//                if(!premier) sql+=" )";
			
//                // S�lection en Exception
//                // HoldingCompany
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.holdingCompanyException);
//                if(list.Length>0){
//                    if(premier) sql+=" and (";
//                    else sql+=" and";
//                    sql+=" wp.id_holding_company not in ("+list+") ";
//                    premier=false;
//                }
//                // Advertiser
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.advertiserException);
//                if(list.Length>0){
//                    if(premier) sql+=" and (";
//                    else sql+=" and";
//                    sql+=" wp.id_advertiser not in ("+list+") ";
//                    premier=false;
//                }
//                // Advertiser
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.brandException);
//                if(list.Length>0)
//                {
//                    if(premier) sql+=" and (";
//                    else sql+=" and";
//                    sql+=" wp.id_brand not in ("+list+") ";
//                    premier=false;
//                }
//                // Product
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.productException);
//                if(list.Length>0){
//                    if(premier) sql+=" and (";
//                    else sql+=" and";
//                    sql+=" wp.id_product not in ("+list+") ";
//                    premier=false;
//                }
//                // Sector
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.sectorException);
//                if(list.Length>0){
//                    if(premier) sql+=" and (";
//                    else sql+=" and";
//                    sql+=" wp.id_sector not in ("+list+") ";
//                    premier=false;
//                }
//                // SubSector
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.subSectorException);
//                if(list.Length>0){
//                    if(premier) sql+=" and (";
//                    else sql+=" and";
//                    sql+=" wp.id_subsector not in ("+list+") ";
//                    premier=false;
//                }
//                // group
//                list=webSession.GetSelection(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).TreeCompetitorAdvertiser,CustomerRightConstante.type.groupException);
//                if(list.Length>0){
//                    if(premier) sql+=" and (";
//                    else sql+=" and";
//                    sql+=" wp.id_group_ not in ("+list+") ";
//                    premier=false;
//                }

//                if(!premier) sql+=" )";
//                #endregion

//                #endregion

//                #region Nomenclature Media (droits et s�lection)

//                #region Droits
//                sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true);
//                #endregion

//                #region S�lection
//                //list=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);
//                //if(list.Length>0) sql+=" and ((wp.id_vehicle in ("+list+"))) ";
//                sql+=" and ((wp.id_vehicle= "+idVehicle+")) ";
//                #endregion

//                #endregion
		
//                sql+=" group by wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity , date_media_num";
//                positionUnivers++;
//            }

//            sql+=" )";
//            sql+=" order by id_vehicle, vehicle, category, media, grpUnivers, date_num";
			
			
//            #endregion

//            #region Execution de la requ�te
//            try{
//                return webSession.Source.Fill(sql.ToString());
//            }
//            catch(System.Exception err){
//                throw(new CompetitorMediaPlanAlertDataAccessException ("Impossible de charger les donn�es d'un plan m�dia"+sql,err));
//            }
//            #endregion

//        }
		#endregion

		#region Nouvelle version
		/// <summary>
		/// Charge les donn�es pour cr�er un plan m�dia concurentiel d'UN vehicle
		///		Assignation du nom de la table � attaquer et de l'unit� � s�lectionner
		///		Construction et ex�cution de la requ�te
		/// </summary>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
		/// Lanc�e si aucune table ne correspond au vehicle,si l'unit� n'est pas valide ou si une erreur Oracle est survenue
		/// </exception>
		/// <param name="webSession">Session du client</param>
		/// <param name="idVehicle">Identifiant du vehicle � traiter</param>
		/// <param name="beginingDate">Date de d�but</param>
		/// <param name="endDate">Date de fin</param>
		/// <returns>Dataset contenant les donn�es rapatri�es de la base de donn�es pour le vehicule consid�r�</returns>
		/// <remarks>
		/// Utilise les m�thodes:
		///		private static string getTable(DBClassificationConstantes.Vehicles.names idVehicle)
		///		private static string getField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit unit)
		///		public static string TNS.AdExpress.Web.Functions.SqlGenerator.getAnalyseCustomerProductRight(WebSession webSession,string tablePrefixe,bool beginByAnd)
		///		public static string TNS.AdExpress.Web.Functions.SqlGenerator.getAnalyseCustomerMediaRight(WebSession webSession,string tablePrefixe,bool beginByAnd)
		///		public string TNS.AdExpress.Web.Core.Sessions.WebSession.GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
		/// </remarks>
		public static DataSet GetData(WebSession webSession, string idVehicle, string beginingDate, string endDate) {

			#region Variables
			string list = "";
			string tableName = "";
			string unitField = "";
			bool premier = true;
			int positionUnivers = 0;
			const string DATA_TABLE_PREFIXE = "wp";
			#endregion

			#region R�cup�ration des noms de tables et de champs suivant le vehicule
			try {
				//tableName = getTable((DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)));
				Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
				tableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult((DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle)), currentModuleDescription.ModuleType);
				//unitField = getField(webSession.Unit);
				if (DBClassificationConstantes.Vehicles.names.outdoor == (DBClassificationConstantes.Vehicles.names)(Int64.Parse(idVehicle))
					&& webSession.Unit == TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion) {
					unitField = WebFunctions.SQLGenerator.getUnitField(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.numberBoard);
				}
				else
					unitField = WebFunctions.SQLGenerator.getUnitField(webSession.Unit);
			}
			catch (System.Exception exc) {
				throw new CompetitorMediaPlanAlertDataAccessException("MedaiPlanAlertDataAccess.getData(WebSession webSession,string idVehicle, string beginingDate, string endDate) : \n\t", exc);
			}
			#endregion

			#region Construction de la requ�te
			string sql = "";
			sql += " select grp,grpUnivers, id_vehicle, vehicle, id_category, category, id_media, media,id_periodicity,date_num,unit";
			sql += " from (";
			while (webSession.PrincipalProductUniverses.ContainsKey(positionUnivers) && webSession.PrincipalProductUniverses[positionUnivers] != null) {
				if (positionUnivers > 0) {
					sql += " UNION ";
				}
				//sql += " select '" + ((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[positionUnivers]).NameCompetitorAdvertiser + "' as grp , ";
				sql += " select '" + webSession.PrincipalProductUniverses[positionUnivers].Label + "' as grp , ";
				sql += " " + positionUnivers + " as grpUnivers ,wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity";

				// S�lection de la date
				sql += ", date_media_num as date_num";				
				sql += ", sum(" + unitField + ") as unit";

				// Tables
				sql += " from " + DBTableFieldsName.Schema.ADEXPRESS_SCHEMA + "." + "vehicle vh, "
					+ DBTableFieldsName.Schema.ADEXPRESS_SCHEMA + "." + "category ct, "
					+ DBTableFieldsName.Schema.ADEXPRESS_SCHEMA + "." + "media md, "
					+ DBTableFieldsName.Schema.ADEXPRESS_SCHEMA + "." + tableName + " wp ";

				// Conditions de jointure
				sql += " Where vh.id_vehicle=wp.id_vehicle ";
				sql += " and ct.id_category=wp.id_category ";
				sql += " and md.id_media=wp.id_media ";
				sql += " and vh.id_language=" + webSession.SiteLanguage.ToString();
				sql += " and ct.id_language=" + webSession.SiteLanguage.ToString();
				sql += " and md.id_language=" + webSession.SiteLanguage.ToString();
				// Activation 
				sql += " and vh.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
				sql += " and ct.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
				sql += " and md.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
				// P�riode
				sql += " and date_media_num >=" + beginingDate;
				sql += " and date_media_num <=" + endDate;

				#region Nomenclature Produit (droits)
				premier = true;
				//Droits en acc�s
				sql += SQLGenerator.getAnalyseCustomerProductRight(webSession, "wp", true);
				// Produit � exclure en radio
				sql += SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, "wp", true, false);
				#endregion

				#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et s�lection)

				// S�lection de Produits
				if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0
					&& webSession.PrincipalProductUniverses[positionUnivers] != null)
					sql += webSession.PrincipalProductUniverses[positionUnivers].GetSqlConditions(DATA_TABLE_PREFIXE, true);


				#endregion

				#region Nomenclature Media (droits et s�lection)

				#region Droits
				sql += SQLGenerator.getAnalyseCustomerMediaRight(webSession, DATA_TABLE_PREFIXE, true);
				#endregion

				#region S�lection
				
				sql += " and ((wp.id_vehicle= " + idVehicle + ")) ";
				#endregion

				#endregion

				sql += " group by wp.id_vehicle, vehicle, wp.id_category, category, wp.id_media, media, wp.id_periodicity , date_media_num";
				positionUnivers++;
			}

			sql += " )";
			sql += " order by id_vehicle, vehicle, category, media, grpUnivers, date_num";


			#endregion

			#region Execution de la requ�te
			try {
				return webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new CompetitorMediaPlanAlertDataAccessException("Impossible de charger les donn�es d'un plan m�dia" + sql, err));
			}
			#endregion

		}
#endregion

		/// <summary>
		/// Charge les donn�es pour cr�er un plan m�dia concurentiel pour chaque vehicle s�lectionn� dans la session
		/// sur la p�riode sp�cifi�e
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="beginningDate">date de d�but de p�riode au formazt YYYYMMDD</param>
		/// <param name="endDate">date de fin de p�riode au formazt YYYYMMDD</param>
		/// <returns>Dataset contenant les donn�es rapatri�es de la base de donn�es pour les vehicule s�lectionn� dans la session</returns>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
		/// Lanc�e en cas d'erreur sur un vehicle
		/// </exception>
		/// <remarks>
		/// Utilise les m�thodes:
		///		public string TNS.AdExpress.Web.Core.Sessions.WebSession.GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
		/// </remarks>
		public static DataSet GetPluriMediaDataset(WebSession webSession, string beginningDate, string endDate){
			string[] listVehicles = webSession.GetSelection(webSession.SelectionUniversMedia, AccessCst.vehicleAccess).Split(new char[]{','});
			DataSet ds = new DataSet();
			for(int i=0; i< listVehicles.Length; i++){
				try{
					ds.Merge(GetData(webSession,listVehicles[i],beginningDate,endDate));
				}
				catch(CompetitorMediaPlanAlertDataAccessException exc){
					throw new CompetitorMediaPlanAlertDataAccessException("CompetitorMediaPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate)",exc);
				}
			}
			return ds;
		}

		/// <summary>
		/// Obtient les donn�es pour cr�er un plan m�dia concurentiel plurim�dia � partir  des donn�es 
		/// de la session d'un client
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Donn�es charg�es</returns>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
		/// Lanc�e en cas d'erreur
		/// </exception>
		public static DataSet GetPluriMediaDataset(WebSession webSession){
			try{
				return GetPluriMediaDataset(webSession, webSession.PeriodBeginningDate, webSession.PeriodEndDate);
			}
			catch(CompetitorMediaPlanAlertDataAccessException exc){
				throw new CompetitorMediaPlanAlertDataAccessException("CompetitorMediaPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate)",exc);
			}
		}

	}
}
