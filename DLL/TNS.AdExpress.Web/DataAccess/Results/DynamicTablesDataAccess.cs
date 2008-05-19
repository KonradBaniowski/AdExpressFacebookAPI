#region Information
// Auteur: G. Ragneau
// Date de création: 22/09/2004 
// Date de modification: 27/09/2004 
//	18/02/2005	A.Obermeyer		rajout Marque en personnalisation
//	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using TblFormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables;
using FormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using RightCst = TNS.AdExpress.Constantes.Customer.Right;
using FrequencyCst = TNS.AdExpress.Constantes.Customer.DB.Frequency;
using DBClassificationCst = TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Classe d'accès à la base de données pour le module de tableaux dynamiques (analyse sectorielle)
	/// </summary>
	public class DynamicTablesDataAccess{
		#region GetData
		/// <summary>
		/// Méthode qui retourne les données nécessaires à la construction d'un tableau dynamique
		/// en considérant les critères de la session.
		/// Elle ajoute au fur et à mesure les différents clauses nécessaires à la requête: select, from, jointures
		/// sélection media et produit, droits, langage et activation, tri et regroupement
		/// <seealso cref="TNS.AdExpress.Web.Core.Sessions.WebSession"/>
		/// </summary>
		/// <remarks>!!!!!La classe métier qui s'appuie sur cette classe BDD utilise l'ordre des champs ds le select
		///	Il n'est pas recommandé de changer l'ordre des champs media, produit...
		///	Utilise les méthodes suivantes:
		///		private static void appendSelectClause(WebSession, Stringuilder);
		///		private static void appendFromClause(WebSession, Stringuilder, String);
		///		private static void appendJointClause(WebSession, Stringuilder);
		///		private static void appendSelectionClause(WebSession, Stringuilder);
		///		private static void appendRightClause(WebSession, Stringuilder);
		///		private static void appendActivationLanguageClause(WebSession, Stringuilder, String);
		///		private static void appendRegroupmentAndOrderClause(WebSession, Stringuilder);
		/// </remarks>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.NoDataException()">
		/// Lancée en cas de période non valide
		/// </exception>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.DeliveryFrequencyException()">
		/// Lancée en cas de de fréquence de livraison des données invalides
		/// </exception>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.RecapAdvertiserDataAccessException()">
		/// Lancée en cas:
		///		échec de connection à la base de données
		///		échec d'exécution de la requête
		///		échec de fermeture de la base de données
		/// </exception>
		/// <param name="webSession">Sesssion utilisateur</param>
		/// <returns>DataSet contenant les données</returns>
		public static DataSet GetData(WebSession webSession){

			StringBuilder sql = new StringBuilder(2000);

			#region Construction de la requête
			try{
				
				string dataTable = GetVehicleTableName(webSession);
				//!!!!!La classe métier qui s'appuie sur cette classe BDD utilise l'ordre des champs ds le select
				//Il n'est pas recommandé de changer l'ordre des champs media, produit...
				AppendSelectClause(webSession, sql);
				AppendFromClause(webSession, sql, dataTable);
				AppendJointClause(webSession, sql);
				AppendSelectionClause(webSession, sql);
				AppendRightClause(webSession,sql);
				AppendActivationLanguageClause(webSession, sql, dataTable);
				AppendRegroupmentAndOrderClause(webSession, sql);
			}
			catch(NoDataException e1) {throw e1;}
			catch(DeliveryFrequencyException e3) {throw e3;}
			catch(System.Exception e2) {throw e2;}
			#endregion

			#region Execution de la requête		
            IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new DynamicTablesDataAccessException ("Impossible de charger les données pour les tableaux dynamiques: "+sql.ToString(),err));
			}
			#endregion

			#region Ancien Code avant notion dataSource

//			DataSet ds=new DataSet();
//			//OracleConnection connection=webSession.CustomerLogin.Connection;
//			OracleConnection connection = new OracleConnection(DBCst.Connection.RECAP_CONNECTION_STRING);
//			OracleCommand sqlCommand=null;
//
//			OracleDataAdapter sqlAdapter=null;
//
//			#region Ouverture de la base de données
//			bool DBToClosed=false;
//			// On teste si la base est déjà ouverte
//			if (connection.State==System.Data.ConnectionState.Closed) {
//				DBToClosed=true;
//				try {
//					connection.Open();
//				}
//				catch(System.Exception et) {
//					throw(new RecapAdvertiserDataAccessException("Impossible d'ouvrir la base de données:"+et.Message));
//				}
//			}
//			#endregion
//
//			#region Execution
//			try {
//				 sqlCommand=new OracleCommand(sql.ToString(),connection);
//				sqlAdapter=new OracleDataAdapter(sqlCommand);
//				sqlAdapter.Fill(ds);
//			}
//				#endregion
//
//			#region Traitement d'erreur du chargement des données
//			catch(System.Exception ex) {
//				try {
//					// Fermeture de la base de données
//					if (sqlAdapter!=null) {
//						sqlAdapter.Dispose();
//					}
//					if(sqlCommand!=null) sqlCommand.Dispose();
//					if (DBToClosed) connection.Close();
//				}
//				catch(System.Exception et) {
//					throw(new RecapAdvertiserDataAccessException ("Impossible de fermer la base de données, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
//				}
//				throw(new RecapAdvertiserDataAccessException ("Impossible de charger les données:"+sql+" "+ex.Message));
//			}
//			#endregion
//
//			#region Fermeture de la base de données
//			try {
//				// Fermeture de la base de données
//				if (sqlAdapter!=null) {
//					sqlAdapter.Dispose();
//				}
//				if(sqlCommand!=null)sqlCommand.Dispose();
//				if (DBToClosed) connection.Close();
//			}
//			catch(System.Exception et) {
//				throw(new RecapAdvertiserDataAccessException ("Impossible de fermer la base de données :"+et.Message));
//			}
//			#endregion
//			return ds;
//			#endregion

			#endregion	
		}

		#endregion

//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!droits media
		#region Méthodes Privées

		#region getVehicleTableName
		/// <summary>
		/// Méthode privée qui détecte la table de recap à utiliser en fonction de la sélection média, produit
		/// et du niveau de détail choisi
		///		détection d'une étude monomédia ou pluri média ==> recap_tv ... ou recap_pluri
		///		niveau de détail de la nomenclature produit ==> recap ou recap_segment
		/// </summary>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.DynamicTablesDataAccessException">
		/// Lancée si aucune table de la base de doonées ne correspond au vehicle spécifié dans la session utilisateur.
		/// </exception>
		/// <param name="webSession">Session utilisateur</param>
		/// <returns>Chaîne de caractère correspondant au nom de la table à attaquer</returns>
		private static string GetVehicleTableName(WebSession webSession){

			#region Variables
			Int64 vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID;
			string tableName = "recap_pluri";
			#endregion

			#region Détection du type de table vehicle (pluri ou mono?)
			switch((DBClassificationCst.Vehicles.names) vehicle){
				case DBClassificationCst.Vehicles.names.cinema:
					tableName = "recap_cinema";
					break;
				case DBClassificationCst.Vehicles.names.internet:
					tableName = "recap_internet";
					break;
				case DBClassificationCst.Vehicles.names.outdoor:
					tableName = "recap_outdoor";
					break;
				case DBClassificationCst.Vehicles.names.radio:
					tableName = "recap_radio";
					break;
				case DBClassificationCst.Vehicles.names.tv:
					tableName = "recap_tv";
					break;
				case DBClassificationCst.Vehicles.names.press:
					tableName = "recap_press";
					break;
				case DBClassificationCst.Vehicles.names.plurimedia:
					tableName = "recap_pluri";
					break;
				case DBClassificationCst.Vehicles.names.mediasTactics:
					tableName = "recap_tactic";
					break;
				case DBClassificationCst.Vehicles.names.mobileTelephony:
					tableName = DBCst.Tables.RECAP_MOBILE_TELEPHONY;
					break;
				case DBClassificationCst.Vehicles.names.emailing:
					tableName = DBCst.Tables.RECAP_EMAILING;
					break;	
				case DBClassificationCst.Vehicles.names.internationalPress:
//				case DBClassificationCst.Vehicles.names.mediasTactics:
				case DBClassificationCst.Vehicles.names.others:
				case DBClassificationCst.Vehicles.names.adnettrack:
				default:
                    throw new DynamicTablesDataAccessException("Le vehicle n° " + vehicle + " n'est pas traité.");
			}
			#endregion

			#region Détection de la table à attaquer en fonction du niveau de détail produit nécessaire
			switch (webSession.PreformatedProductDetail){
				case FormatCst.PreformatedProductDetails.groupBrand:
				case FormatCst.PreformatedProductDetails.groupProduct:
				case FormatCst.PreformatedProductDetails.groupAdvertiser:
				case FormatCst.PreformatedProductDetails.advertiser:
				case FormatCst.PreformatedProductDetails.advertiserBrand:
				case FormatCst.PreformatedProductDetails.advertiserProduct:
				case FormatCst.PreformatedProductDetails.brand:
				//changes for the introduction of segmentAdvertiser, segmentProduct, segmentBrand 
				case FormatCst.PreformatedProductDetails.segmentAdvertiser:
				case FormatCst.PreformatedProductDetails.segmentBrand:
				case FormatCst.PreformatedProductDetails.segmentProduct:
				case FormatCst.PreformatedProductDetails.product:

					break;
				default:
					tableName += "_segment";
					break;
			}
			#endregion

			return tableName;
		}
		#endregion

		#region appendSelectClause
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>!!!!!La classe métier qui s'appuie sur cette classe BDD utilise l'ordre des champs ds le select
		///	Il n'est pas recommandé de changer l'ordre des champs media, produit...
		///	étape:
		///		- Sauvegardede l'index du dernier caractère de la requête a son arrivé afin de pouvoir insérer un pmrceau de requête si nécessaire
		///		- Traitement des champs qualitatifs:
		///			- Si le tableau preformaté affiche des données de la nomenclature media:
		///				suivant le niveau de détail demandé, on ajoute les champs à la requete en normalisant leurs noms
		///				chaque nom représente la nomenclature media (id_m) et le niveau considéré(id_m1, id_m2...)
		///				Au cas ou le niveau de détail n'est pas repertorié, on applique par défaut le niveau vehicle uniquement et on update la session
		///			- Si le tableau preformaté affiche des données de la nomenclature produit:
		///				suivant le détail demandé, on ajoute les champs à la requete en normalisant leurs noms
		///				chaque nom représente la nomenclature produit (id_p) et le niveau considéré(id_p1, id_p2)
		///				Au cas ou le niveau de détail n'est pas repertorié, on applique par défaut le détail group uniquement et on update la session
		///				Pour les niveaux faisant intervenir la nomenclature annonceur, on sauvegarde dans une strig un morceau de requete ramenant l'id_advertiser
		///			- si la nompenclature produit est la nomenclature principale, on l'insere devant les champs 
		///			de la nomenclature support qui devient par consequent la omenclature secondaire.
		///		- Traitement des champs quantitatifs:
		///			- ON commence par valider la période considérées en fonction de la fréquence de livraison des données
		///			Si jamais la période n'est pas valide (données non accessiblkes par raport a la frequence), on lance une exception NoData
		///			- construction de la liste des mois month a attaquer. POur chaquer mois, on fera la somme des lignes a ramener
		///			- si le tableau nécessite des années, on utilise le champ total_year
		///			- si le tableau est un mensuel cimulé, on construit la requete en faisant la somme des mois precedent sur la période pour chaque mois a rapatrier
		///			exemple: periode Mars ==> mai, on prend mars et mars+avril et mars+avril+mai...
		///			- si le tableau est un mensuel + total, on ramene tous les mois de la liste month plus un champ qui est la somme de tous les mois
		///		-Traitement de la notion d'annonceur referent ou concurrent:
		///			si l'univers d'annonceurs concurrents ou l'univers d'annonceurs de reference ne sont pas vides (non exclusif), on concatene a la requete le champ id_advertiser
		///		- La colonne total est une facilité de programmation pour la construction des tableaux de type 5.
		///			en effet, en considérant un champ 'total' bidon, on inclu dans la requete un niveau de 
		///			nomenclature suplémentaire qui sera traite de la meme maniere que la nomenclature principale
		///			dans les classes metier (valable uniquement pour la ableau de type 5 pour l instant)
		/// </remarks>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="sql">StringBuilder contenant la requete sql</param>
		private static void AppendSelectClause(WebSession webSession, StringBuilder sql){

			sql.Append("select");

			// Attaquer une table
			int downLoadDate=webSession.DownLoadDate;

			#region champs descriptifs
			int mediaIndex = webSession.PreformatedTable.ToString().IndexOf("edia");
			int productIndex = webSession.PreformatedTable.ToString().IndexOf("roduct");
			int beginningIndex = sql.Length;
			string annonceurPerso = "";

			#region Champs nomenclature media
			if (mediaIndex>-1){
				//nomenclature media présente dans le tableau préformaté
				switch(webSession.PreformatedMediaDetail){
					case FormatCst.PreformatedMediaDetails.vehicle:
						sql.Append(" rcp.id_vehicle as id_m1, vehicle as m1");
						break;
					case FormatCst.PreformatedMediaDetails.vehicleCategory:
						sql.Append(" rcp.id_vehicle as id_m1, vehicle as m1, rcp.id_category as id_m2, category as m2");
						break;
					case FormatCst.PreformatedMediaDetails.vehicleCategoryMedia:
						sql.Append(" rcp.id_vehicle as id_m1, vehicle as m1, rcp.id_category as id_m2, category as m2, rcp.id_media as id_m3, media as m3");
						break;
					case FormatCst.PreformatedMediaDetails.vehicleMedia:
						sql.Append(" rcp.id_vehicle as id_m1, vehicle as m1, rcp.id_media as id_m2, media as m2");
						break;
					default:
						webSession.PreformatedMediaDetail = FormatCst.PreformatedMediaDetails.vehicle;
						sql.Append(" rcp.id_vehicle as id_m1, vehicle as m1");
						break;
						//throw new ASDynamicTablesDataAccessException("Le format de détail " + webSession.PreformatedMediaDetail.ToString() + " n'est pas un cas valide.");
				}
			}
			#endregion

			#region Champs nomenclature produit
			if (productIndex>-1){
				string sqlStr = "";
				//nomenclature produit présente dans le tableau préformaté
				switch(webSession.PreformatedProductDetail){
					case FormatCst.PreformatedProductDetails.group:
						sqlStr = " rcp.id_group_ as id_p1, group_ as p1";
						break;
					case FormatCst.PreformatedProductDetails.groupSegment:
						sqlStr = " rcp.id_group_ as id_p1, group_ as p1, rcp.id_segment as id_p2, segment as p2";
						break;
					case FormatCst.PreformatedProductDetails.groupBrand:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_group_ as id_p1, group_ as p1, rcp.id_brand as id_p2, brand as p2";
						break;
					case FormatCst.PreformatedProductDetails.groupProduct:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_group_ as id_p1, group_ as p1, rcp.id_product as id_p2, product as p2";
						break;
					case FormatCst.PreformatedProductDetails.groupAdvertiser:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_group_ as id_p1, group_ as p1, rcp.id_advertiser as id_p2, advertiser as p2";
						break;
					case FormatCst.PreformatedProductDetails.advertiser:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_advertiser as id_p1, advertiser as p1";
						break;
					case FormatCst.PreformatedProductDetails.advertiserBrand:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_advertiser as id_p1, advertiser as p1, rcp.id_brand as id_p2, brand as p2";
						break;
					case FormatCst.PreformatedProductDetails.advertiserProduct:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_advertiser as id_p1, advertiser as p1, rcp.id_product as id_p2, product as p2";
						break;
					case FormatCst.PreformatedProductDetails.brand:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_brand as id_p1, brand as p1";
						break;
					case FormatCst.PreformatedProductDetails.product:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_product as id_p1, product as p1";
						break;

						//changes to accomodate variété/Announceurs, Variétés/produit, Variétés/Marques 

					case FormatCst.PreformatedProductDetails.segmentAdvertiser:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_segment as id_p1, segment as p1, rcp.id_advertiser as id_p2, advertiser as p2 ";
						break;
					case FormatCst.PreformatedProductDetails.segmentProduct:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_segment as id_p1, segment as p1, rcp.id_product as id_p2, product as p2";
						break;
					case FormatCst.PreformatedProductDetails.segmentBrand:
						annonceurPerso = ", rcp.id_advertiser";
						sqlStr = " rcp.id_segment as id_p1, segment as p1, rcp.id_brand as id_p2, brand as p2 ";
						break;
					

					default:
						//throw new ASDynamicTablesDataAccessException("Le format de détail " + webSession.PreformatedProductDetail.ToString() + " n'est pas un cas valide.");
						sqlStr = " rcp.id_group_ as id_p1, group_ as p1";
						webSession.PreformatedProductDetail = FormatCst.PreformatedProductDetails.group;
						break;
				}
				if (productIndex<2){
					//nomenclature produit en position de parent de la nomenclature media
					sql.Insert(beginningIndex, sqlStr + (mediaIndex>-1?", ":""));
				}
				else{
					//nomenclature produit en position d'enfant de la nomenclature media
					sql.Append(  ( (mediaIndex>-1)?", ":" " )  + sqlStr);
				}
			}
			#endregion

			#endregion

			#region champs de données quantitatives

			//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
			//du dernier mois dispo en BDD
			//traitement de la notion de fréquence
			string absolutEndPeriod = WebFunctions.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
			if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
				throw new NoDataException();

			//Liste des mois de la période sélectionnée
			//Calcul de l'index année dans une table recap (N, N1 ou N2)
			

			int i = DateTime.Now.Year - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			
			if(DateTime.Now.Year>downLoadDate){
				i=i-1;
			}

			string year = (i>0)?i.ToString():"";
			string previousYear = (int.Parse(year!=""?year:"0")+1).ToString();

			//Détermination du nombre de mois et Instanciation de la liste des champs mois
			int firstMonth = int.Parse(webSession.PeriodBeginningDate.Substring(4,2));
			string[] months = new string[int.Parse(absolutEndPeriod.Substring(4,2))-
				firstMonth + 1];
			string[] previousYearMonths = (webSession.ComparativeStudy)?new string[months.GetUpperBound(0)+1]:null;
			//création de la liste des champs période
			bool first = true;
			for(i = 0; i < months.Length; i++){
				months[i] = "sum(exp_euro_n" + year + "_" + (i+firstMonth)+")";
				if (webSession.ComparativeStudy)
					previousYearMonths[i] = "sum(exp_euro_n" + previousYear + "_" + (i+firstMonth)+")";
			}

			//Year
			if(webSession.PreformatedTable.ToString().IndexOf("_Year")>-1 || webSession.PreformatedTable == WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.productYear_X_Media){
				//				sql.Append(", sum(total_year_n"+year+") as N"+year);
				//				if (webSession.ComparativeStudy)
				//					sql.Append(", sum(total_year_n"+previousYear+") as N"+previousYear);
			
				string comparativeStudyMonths="";

				for(i = 0; i < months.Length; i++){
					
					if(i==months.Length-1 && months.Length>1){
						sql.Append( "exp_euro_n"+year+"_"+(i+firstMonth)+" ) as  N"+year);
						comparativeStudyMonths+="exp_euro_n"+previousYear+"_"+(i+firstMonth)+" ) as  N"+previousYear;
					}
					else if(i==0 && months.Length>1){
						sql.Append(", sum(exp_euro_n"+year+"_"+(i+firstMonth)+" + ");
						comparativeStudyMonths+="sum(exp_euro_n"+previousYear+"_"+(i+firstMonth)+" + ";

					}
					else if(months.Length==1){
						sql.Append(", sum(exp_euro_n"+year+"_"+(i+firstMonth)+") as  N"+year);
						comparativeStudyMonths+="sum(exp_euro_n"+previousYear+"_"+(i+firstMonth)+") as  N"+previousYear;
					}
					else{
						sql.Append("exp_euro_n"+year+"_"+(i+firstMonth)+" + ");
						comparativeStudyMonths+="exp_euro_n"+previousYear+"_"+(i+firstMonth)+" + ";
					}
				}
				if (webSession.ComparativeStudy)
					sql.Append(", "+comparativeStudyMonths);



			}

			//Cumul,
			if(webSession.PreformatedTable.ToString().IndexOf("_Cumul")>-1){
				int j;
				for(i = 0; i < months.Length; i++){
					sql.Append(", ");
					first=true;
					for (j = 0 ; j <= i; j++){
						if(!first)sql.Append("+");
						else first = false;
						sql.Append(months[j]);
					}
					sql.Append(" as N" + year + "_" + j);
					//Year N-1 si désirée et possible
					if (webSession.ComparativeStudy){
						sql.Append(", ");
						first=true;
						for (j = 0 ; j <= i; j++){
							if(!first)sql.Append("+");
							else first = false;
							sql.Append(previousYearMonths[j]);
						}
						sql.Append(" as N" + previousYear + "_" + j);
					}
				}
			}

			//Mensual
			if(webSession.PreformatedTable.ToString().IndexOf("_Mensual")>-1){
				int cumulIndex  = sql.Length;
				first=true;
				string previousYearTotal = "";
				for(i = 0; i < months.Length; i++){
					sql.Append(", " + months[i]+"");
					if (webSession.ComparativeStudy)sql.Append(", " + previousYearMonths[i]+"");
					if(!first){
						sql.Insert(cumulIndex, months[i] + "+");
						if (webSession.ComparativeStudy)previousYearTotal = previousYearMonths[i] + "+" + previousYearTotal;
					}
					else{
						first = false;
						sql.Insert(cumulIndex, months[i] + " as total_N" + year);
						if (webSession.ComparativeStudy)previousYearTotal = previousYearMonths[i] + " as total_N" + previousYear;
					}
				}
				sql.Insert(cumulIndex, ", ");
				if (webSession.ComparativeStudy)sql.Insert(sql.ToString().IndexOf(" as total_N" + year) + 11 + year.Length, ", " + previousYearTotal);
			}

			//Year+Mensual (ajout des mois)
			if(webSession.PreformatedTable.ToString().IndexOf("_YearMensual")>-1)
			{
				int cumulIndex  = sql.Length;
				first=true;
				for(i = 0; i < months.Length; i++)
				{
					sql.Append(", " + months[i]+" as Mo_" + (i+1));
				}
			}

			#endregion

			#region Notion de personnalisation des annonceurs?
			//if (webSession.GetSelection(webSession.ReferenceUniversAdvertiser, RightCst.type.advertiserAccess).Split(',').Length > 0)
			//    sql.Append(annonceurPerso);
			//else if (webSession.CompetitorUniversAdvertiser[0] != null)
			//    if (webSession.GetSelection(((TreeNode)webSession.CompetitorUniversAdvertiser[0]), RightCst.type.advertiserAccess).Split(',').Length > 0)
			//        sql.Append(annonceurPerso);
			NomenclatureElementsGroup nomenclatureElementsGroup = null;
			string tempString = "";
			if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0)) {
				nomenclatureElementsGroup = webSession.SecondaryProductUniverses[0].GetGroup(0);
				if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
					if (nomenclatureElementsGroup.Contains(TNSClassificationLevels.ADVERTISER)) tempString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
				}
			}else if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1)) {
				nomenclatureElementsGroup = webSession.SecondaryProductUniverses[1].GetGroup(0);
				if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
					if (nomenclatureElementsGroup.Contains(TNSClassificationLevels.ADVERTISER)) tempString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
				}
			}
			if(tempString != null && tempString.Split(',').Length>0)
			sql.Append(annonceurPerso);
			#endregion

			#region Colonne total
			if (webSession.PreformatedTable == FormatCst.PreformatedTables.productMedia_X_Year ||
				webSession.PreformatedTable == FormatCst.PreformatedTables.productMedia_X_YearMensual)
				sql.Insert(beginningIndex, " '0' as id_p, 'TOTAL' as p, ");
			if ((webSession.PreformatedTable == FormatCst.PreformatedTables.mediaProduct_X_Year || webSession.PreformatedTable == FormatCst.PreformatedTables.mediaProduct_X_YearMensual)
				&& ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()){
				sql.Insert(beginningIndex, " '0' as id_m, 'TOTAL' as m, ");
			}
			#endregion
		}
		#endregion

		#region appendFromClause
		/// <summary>
		/// Ajoute la clause from d'un requete sql.
		/// Etapes:
		///		pour chaque champ qui est susceptible d'être présent dans la requête, on vérifie sa présence 
		///		effective et on ajoute la table a laquelle il correspond au from
		/// </summary>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="sql">Requete sql</param>
		/// <param name="dataTable">nom de la table de recap à attaquer</param>
		private static void AppendFromClause(WebSession webSession, StringBuilder sql, string dataTable){

			sql.Append(" from");

			//table recap
			sql.Append(" " + dataTable + " " + DBCst.Tables.RECAP_PREFIXE);
		
			#region nomenclature media
			sql.Append(", vehicle " + DBCst.Tables.VEHICLE_PREFIXE);
			if(sql.ToString().IndexOf("category")>-1)
				sql.Append(", category " + DBCst.Tables.CATEGORY_PREFIXE);
			if(sql.ToString().IndexOf("media")>-1)
				sql.Append(", media " + DBCst.Tables.MEDIA_PREFIXE);
			#endregion

			#region nomenclature produit
			if(sql.ToString().IndexOf("group_")>-1)
				sql.Append(", group_ " + DBCst.Tables.GROUP_PREFIXE);
			if(sql.ToString().IndexOf("segment")>-1)
				sql.Append(", segment " + DBCst.Tables.SEGMENT_PREFIXE);
			if(sql.ToString().IndexOf("brand")>-1)
				sql.Append(", brand " + DBCst.Tables.BRAND_PREFIXE);
			if(sql.ToString().IndexOf("product")>-1)
				sql.Append(", product " + DBCst.Tables.PRODUCT_PREFIXE);
			if(sql.ToString().IndexOf("advertiser")>-1)
				sql.Append(", advertiser " + DBCst.Tables.ADVERTISER_PREFIXE);
			#endregion

		}
		#endregion

		#region appendJointClause
		/// <summary>
		/// Ajoute a la requête sql les conditions dfe jointures entre la table des recap et les tabmes de 
		/// nomenclature. Pour chaque champ de nomenclmature susceptible d'aparaitre dans la requête, on 
		/// vérifie sa présence effective et si c'est le cas, on effectue la jointure avec la table de recap
		/// </summary>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="sql">Requête sql</param>
		private static void AppendJointClause(WebSession webSession, StringBuilder sql){

			string linkWord = " where ";

			#region nomenclature produit
			if(sql.ToString().IndexOf(" group_ ")>-1){
				sql.Append(linkWord + DBCst.Tables.GROUP_PREFIXE + ".id_group_ = " + DBCst.Tables.RECAP_PREFIXE + ".id_group_");
				linkWord = " and ";
			}
			if(sql.ToString().IndexOf(" segment ")>-1){
				sql.Append(linkWord + DBCst.Tables.SEGMENT_PREFIXE + ".id_segment = " + DBCst.Tables.RECAP_PREFIXE + ".id_segment");
				linkWord = " and ";
			}
			if(sql.ToString().IndexOf(" brand ")>-1){
				sql.Append(linkWord + DBCst.Tables.BRAND_PREFIXE + ".id_brand = " + DBCst.Tables.RECAP_PREFIXE + ".id_brand");
				linkWord = " and ";
			}
			if(sql.ToString().IndexOf(" product ")>-1){
				sql.Append(linkWord + DBCst.Tables.PRODUCT_PREFIXE + ".id_product = " + DBCst.Tables.RECAP_PREFIXE + ".id_product");
				linkWord = " and ";
			}
			if(sql.ToString().IndexOf(" advertiser ")>-1){
				sql.Append(linkWord + DBCst.Tables.ADVERTISER_PREFIXE + ".id_advertiser = " + DBCst.Tables.RECAP_PREFIXE + ".id_advertiser");
				linkWord = " and ";
			}
			#endregion

			#region nomenclature media
			if(sql.ToString().IndexOf(" vehicle ")>-1){
				sql.Append(linkWord + DBCst.Tables.VEHICLE_PREFIXE + ".id_vehicle = " + DBCst.Tables.RECAP_PREFIXE + ".id_vehicle");
				linkWord = " and ";
			}
			if(sql.ToString().IndexOf(" category ")>-1){
				sql.Append(linkWord + DBCst.Tables.CATEGORY_PREFIXE + ".id_category = " + DBCst.Tables.RECAP_PREFIXE + ".id_category");
				linkWord = " and ";
			}
			if(sql.ToString().IndexOf(" media ")>-1){
				sql.Append(linkWord + DBCst.Tables.MEDIA_PREFIXE + ".id_media = " + DBCst.Tables.RECAP_PREFIXE + ".id_media");
			}
			#endregion


		}
		#endregion

		#region appendSelectionClause
		/// <summary>
		/// Ajout de la sélection produit et support du client:
		///		Pour chaque niveau de la nomenclature produit (resp support), on teste sa présence dans l'arbre 
		///		SlectionUniversProduct (resp SelectionUniversMedia). SI le niveau est prtésent dans l'arbre,
		///		on le prend en compte dans la requête
		/// </summary>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="sql">Requête sql</param>
		private static void AppendSelectionClause(WebSession webSession, StringBuilder sql){

			bool first = true;

			#region Sélection media
			//vehicle
			string list = webSession.GetSelection(webSession.SelectionUniversMedia, RightCst.type.vehicleAccess);

			
			//on ne teste pas le vehicle si on est en pluri
			if (list.Length>0 && list.IndexOf(DBClassificationCst.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
				first = false;
				sql.Append(" and ( " + DBCst.Tables.RECAP_PREFIXE + ".id_vehicle in (" + list + ")");
			}
			sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebCst.AdExpressUniverse.RECAP_MEDIA_LIST_ID,DBCst.Tables.RECAP_PREFIXE,true));
			// Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
			if(list.IndexOf(DBClassificationCst.Vehicles.names.plurimedia.GetHashCode().ToString())<0){
				sql.Append(WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBCst.Tables.RECAP_PREFIXE,true));	
			}
			//category
			list = webSession.GetSelection(webSession.SelectionUniversMedia, RightCst.type.categoryAccess);
			if (list.Length>0){
				if (first){
					sql.Append(" and (");
					first = false;
				}
				else sql.Append(" or");
				sql.Append(" " + DBCst.Tables.RECAP_PREFIXE + ".id_category in (" + list + ")");
			}
			//media !!!!!!! necessaire en fonction de la table attaquée?
			list = webSession.GetSelection(webSession.SelectionUniversMedia, RightCst.type.mediaAccess);
			if (list.Length>0 && (sql.ToString().IndexOf("recap_radio")>-1 || sql.ToString().IndexOf("recap_tv")>-1 || sql.ToString().IndexOf("recap_outdoor")>-1 || sql.ToString().IndexOf("recap_tactic")>-1)){
				if (first){
					sql.Append(" and (");
					first = false;
				}
				else sql.Append(" or");
				sql.Append(" " + DBCst.Tables.RECAP_PREFIXE + ".id_media in (" + list + ")");
			}
			if (!first)sql.Append(")");

			//Droits Parrainage TV
			if (webSession.CustomerLogin.GetFlag(DBCst.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG) == null) {			
				sql.Append("   and  " + DBCst.Tables.RECAP_PREFIXE + ".id_category not in (68)  ");
			}

			#endregion

			#region Sélection produits		

			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append("  " + webSession.PrincipalProductUniverses[0].GetSqlConditions(DBCst.Tables.RECAP_PREFIXE, true));						

			#endregion

		}
        #endregion
		
		#region appendRightClause
		/// <summary>
		/// Ajoute les droits clients à la requête:
		///		droits produits
		///		droits media, initialement, on ne l'ai as pas pris en compte mais ils doivent l'être au niveau
		///		vehicle uniquement. CAD que des qu'un utilisateur a droit à un support d'un vehicle, il a droit 
		///		à tous les supports. Les droits media doivent encore être ajoutés.
		/// </summary>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="sql">Requête sql</param>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerProductRight(WebSession, TablePrefixe, bool)
		/// </remarks>
		private static void AppendRightClause(WebSession webSession, StringBuilder sql){
	
			sql.Append(" " + WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBCst.Tables.RECAP_PREFIXE, true));
//!!!!!!!!!!!!!!!! Pas de gestion des droits de la nomenclature media dans les recap (src : G Facon le 27/09/2004)
//			if(sql.ToString().IndexOf("recap_radio")>-1 || sql.ToString().IndexOf("recap_tv")>-1 || sql.ToString().IndexOf("recap_outdoor")>-1)
//				sql.Append(" " + WebFunctions.SQLGenerator.getClassificationCustomerRecapMediaRight(webSession, DBCst.Tables.RECAP_PREFIXE, true));
//			else
//				sql.Append(" " + WebFunctions.SQLGenerator.getShortMediaRight(webSession, DBCst.Tables.RECAP_PREFIXE, true));
		}
        #endregion

		#region appendActivationLanguageClause
		/// <summary>
		/// Ajoute la notion d'activation et de langage à la requête.
		/// </summary>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="sql">Requête sql</param>
		/// <param name="dataTable">Nom de la table des recap</param>
		private static void AppendActivationLanguageClause(WebSession webSession, StringBuilder sql, string dataTable){

			//table de recap
//			sql.Append(" and " + DBCst.Tables.RECAP_PREFIXE + ".id_language_i = " + webSession.SiteLanguage);

			#region nomenclature produit
			if(sql.ToString().IndexOf(" group_ ")>-1){
				sql.Append(" and " + DBCst.Tables.GROUP_PREFIXE + ".id_language = " + webSession.SiteLanguage);
				sql.Append(" and " + DBCst.Tables.GROUP_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			}
			if(sql.ToString().IndexOf(" segment ")>-1){
				sql.Append(" and " + DBCst.Tables.SEGMENT_PREFIXE + ".id_language = " + webSession.SiteLanguage);
				sql.Append(" and " + DBCst.Tables.SEGMENT_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			}
			if(sql.ToString().IndexOf(" brand ")>-1){
				sql.Append(" and " + DBCst.Tables.BRAND_PREFIXE + ".id_language = " + webSession.SiteLanguage);
				sql.Append(" and " + DBCst.Tables.BRAND_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			}
			if(sql.ToString().IndexOf(" product ")>-1){
				sql.Append(" and " + DBCst.Tables.PRODUCT_PREFIXE + ".id_language = " + webSession.SiteLanguage);
				sql.Append(" and " + DBCst.Tables.PRODUCT_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			}
			if(sql.ToString().IndexOf(" advertiser ")>-1){
				sql.Append(" and " + DBCst.Tables.ADVERTISER_PREFIXE + ".id_language = " + webSession.SiteLanguage);
				sql.Append(" and " + DBCst.Tables.ADVERTISER_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			}
			#endregion

			#region nomenclature media
			if(sql.ToString().IndexOf(" vehicle ")>-1){
				sql.Append(" and " + DBCst.Tables.VEHICLE_PREFIXE + ".id_language = " + webSession.SiteLanguage);
				sql.Append(" and " + DBCst.Tables.VEHICLE_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			}
			if(sql.ToString().IndexOf(" category ")>-1){
				sql.Append(" and " + DBCst.Tables.CATEGORY_PREFIXE + ".id_language = " + webSession.SiteLanguage);
				sql.Append(" and " + DBCst.Tables.CATEGORY_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			}
			if(sql.ToString().IndexOf(" media ")>-1){
				sql.Append(" and " + DBCst.Tables.MEDIA_PREFIXE + ".id_language = " + webSession.SiteLanguage);
				sql.Append(" and " + DBCst.Tables.MEDIA_PREFIXE + ".activation < " + DBCst.ActivationValues.UNACTIVATED);
			}
			#endregion

		}
        #endregion

		#region appendRegroupmentAndOrderClause
		/// <summary>
		/// Ajoute les clause order by et group by a la requête
		/// </summary>
		/// <param name="webSession">Session Utilisateur</param>
		/// <param name="sql">sql</param>
		private static void AppendRegroupmentAndOrderClause(WebSession webSession, StringBuilder sql){

			#region Group
			string groupClause = sql.ToString().Remove(sql.ToString().IndexOf("sum("), sql.Length-sql.ToString().IndexOf("sum("))
				.Remove(0,6)
				.Replace("as id_m1","")
				.Replace("as m1","")
				.Replace("as id_m2","")
				.Replace("as m2","")
				.Replace("as id_m3","")
				.Replace("as m3","")
				.Replace("as id_m","")
				.Replace("as m","")
				.Replace("as id_p1","")
				.Replace("as p1","")
				.Replace("as id_p2","")
				.Replace("as p2","")
				.Replace("as id_p","")
				.Replace("as p","");
			groupClause = groupClause.Remove(groupClause.LastIndexOf(','),groupClause.Length-groupClause.LastIndexOf(','));

			if (sql.ToString().Substring(0, sql.ToString().IndexOf("from")).IndexOf("rcp.id_advertiser")>0)
				groupClause += ", rcp.id_advertiser ";
			

			sql.Append(" group by " + groupClause);
			#endregion

			#region Order
			switch(webSession.PreformatedTable){
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.media_X_Year:
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.mediaYear_X_Cumul:
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.mediaYear_X_Mensual:
					sql.Append(" order by m1, id_m1");
					if (sql.ToString().IndexOf("id_m2")>-1) sql.Append(", m2, id_m2");
					if (sql.ToString().IndexOf("id_m3")>-1) sql.Append(", m3, id_m3");
					break;
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.mediaProduct_X_Year:
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.mediaProduct_X_YearMensual:
					if(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID != DBClassificationCst.Vehicles.names.plurimedia.GetHashCode())
						sql.Append(" order by m1, id_m1");
					else
						sql.Append(" order by m,id_m, m1, id_m1");
					if (sql.ToString().IndexOf("id_m2")>-1) sql.Append(", m2, id_m2");
					if (sql.ToString().IndexOf("id_m3")>-1) sql.Append(", m3, id_m3");
					sql.Append(", id_p1");
					if (sql.ToString().IndexOf("id_p2")>-1) sql.Append(", p2, id_p2");
					break;
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.productMedia_X_Year:
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.productMedia_X_YearMensual:
					sql.Append(" order by p, id_p, p1, id_p1");
					if (sql.ToString().IndexOf("id_p2")>-1) sql.Append(", p2, id_p2");
					sql.Append(", m1, id_m1");
					if (sql.ToString().IndexOf("id_m2")>-1) sql.Append(", m2, id_m2");
					if (sql.ToString().IndexOf("id_m3")>-1) sql.Append(", m3, id_m3");
					break;
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.productYear_X_Media:
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.product_X_Year:
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.productYear_X_Cumul:
				case WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.productYear_X_Mensual:
					sql.Append(" order by p1, id_p1");
					if (sql.ToString().IndexOf("id_p2")>-1) sql.Append(", p2, id_p2");
					break;
				default:
					webSession.PreformatedTable = WebCst.CustomerSessions.PreformatedDetails.PreformatedTables.media_X_Year;
					sql.Append(" order by m1, id_m1");
					if (sql.ToString().IndexOf("id_m2")>-1) sql.Append(", m2, id_m2");
					if (sql.ToString().IndexOf("id_m3")>-1) sql.Append(", m3, id_m3");
					break;
					//throw new ASDynamicTablesDataAccessException("Le format de tableau " + webSession.PreformatedTable.ToString() + " n'est pas traité.");
			}
			#endregion

		}
        #endregion

		#endregion

	}
}
