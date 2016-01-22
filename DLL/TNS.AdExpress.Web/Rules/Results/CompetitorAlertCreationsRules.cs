#region Information
// Auteur : A.Obermeyer
// Création : 24/09/2004 
// Modification:
// 12/08/2005	A.Dadouch	Nom de fonctions
//				K.Shehzad	Addition of Agglomeration colunm for Outdoor creations 06/07/2005	
#endregion

using System;
using System.Data;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.Date;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.Rules.Results{

	/// <summary>
	/// Utilisée dans l'affichage des créations pour l'alerte concurrentielle
	/// </summary>
	public class CompetitorAlertCreationsRules{
		const string ASSOCIATED_FILE = "ASSOCIATED_FILE";

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CompetitorAlertCreationsRules(){
		}
		#endregion

		#region Traitement des données
		/// <summary>
		///Traitement des données de l'alerte concurentielle
		/// </summary>
		/// <param name="webSession">Session client</param>
        /// <param name="periodBegin">Début de période au format YYYYMMDD</param>
        /// <param name="periodEnd">Fin de période au format YYYYMMDD</param>
        /// <param name="idVehicle">Identifiant du média</param>
		/// <param name="idElement">Identifiant de l'element</param>
		/// <param name="level">Niveau</param>
		/// <returns>Tableau de données</returns>
		public static object[,] GetData( WebSession webSession,int periodBegin, int periodEnd, string idVehicle, Int64 idElement,int level){


				#region Récupération des données
			
				DataSet ds=null;
			
				//	ds = MediaCreationDataAccess.getData(webSession, Int64.Parse(idVehicle), Int64.Parse(idCategory), Int64.Parse(idMedia), dateBegin, dateEnd);
                ds = CompetitorAlertCreationDataAccess.GetData(webSession, Int64.Parse(idVehicle), idElement, level, periodBegin, periodEnd);	
			
				#endregion

				#region Construction du tableau
				switch(VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle))){
					case CstClassification.DB.Vehicles.names.press:
						return GetDataPress(ds);
					case CstClassification.DB.Vehicles.names.internationalPress:
						return GetDataPress(ds);
					case CstClassification.DB.Vehicles.names.radio:
						return GetDataRadio(ds);
					case CstClassification.DB.Vehicles.names.tv:
					case CstClassification.DB.Vehicles.names.others:
						return GetDataTV(ds);
					case CstClassification.DB.Vehicles.names.outdoor:
                        return GetDataOutDoor(ds, webSession);
                    case CstClassification.DB.Vehicles.names.instore:
						return GetDataInStore(ds, webSession);
					default:
						throw new MediaInsertionsCreationsRulesException("Le vehicle demandé n'est pas un cas traité");
				}
				#endregion
		}
		
		#region Presse
		/// <summary>
		/// Génère un tableau de données d'insertions presses préformaté utilisables pour la couche UI:
		///		["date","page","groupz","annonceur","produit","format","surface","couleur","prix","média","catégorie","support","visuel1,visuel2,..."]
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataPress(DataSet ds){
			object[,] tab = new object[ds.Tables[0].Rows.Count+1,ds.Tables[0].Columns.Count-4];
				
			#region Construction du tableau
			string oldDate = "";
			string oldAdvertisement="";
			string oldMedia="";
			bool first=true;
			int i = -1;
			int j = 0;
			foreach(DataRow current in ds.Tables[0].Rows){
				if(current["date_media_num"].ToString().CompareTo(oldDate)!=0
					|| current["id_advertisement"].ToString().CompareTo(oldAdvertisement)!=0
					|| current["id_media"].ToString().CompareTo(oldMedia)!=0){
					//nouvel insertion
					oldMedia = current["id_media"].ToString();
					oldAdvertisement=current["id_advertisement"].ToString();
					oldDate=current["date_media_num"].ToString();
					first=true;
					i++;
					for(j=0;j<tab.GetLength(1);j++){
						tab[i,j]="";
					}
					tab[i,CstWeb.PressInsertionsColumnIndex.DATE_INDEX]=(new DateTime(int.Parse(current["date_media_num"].ToString().Substring(0,4)),
						int.Parse(current["date_media_num"].ToString().Substring(4,2)),
						int.Parse(current["date_media_num"].ToString().Substring(6,2)))).ToString("dd/MM/yyyy");
					tab[i,CstWeb.PressInsertionsColumnIndex.MEDIA_PAGING_INDEX]=current["media_paging"].ToString();
					tab[i,CstWeb.PressInsertionsColumnIndex.GROUP_INDEX]=current["group_"].ToString();
					tab[i,CstWeb.PressInsertionsColumnIndex.ADVERTISER_INDEX]=current["advertiser"].ToString();
					tab[i,CstWeb.PressInsertionsColumnIndex.PRODUCT_INDEX]=current["product"].ToString();
					tab[i,CstWeb.PressInsertionsColumnIndex.FORMAT_INDEX]=current["format"].ToString();
                    tab[i, CstWeb.PressInsertionsColumnIndex.AREA_PAGE_INDEX] = float.Parse(current[UnitsInformation.List[CstWeb.CustomerSessions.Unit.pages].Id.ToString()].ToString()) / 1000;
					tab[i,CstWeb.PressInsertionsColumnIndex.COLOR_INDEX]=current["color"].ToString();
                    tab[i, CstWeb.PressInsertionsColumnIndex.EXPENDITURE_INDEX] = current[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString();
					tab[i,CstWeb.PressInsertionsColumnIndex.VEHICLE_INDEX]=current["vehicle"].ToString();
					tab[i,CstWeb.PressInsertionsColumnIndex.CATEGORY_INDEX]=current["category"].ToString();
					tab[i,CstWeb.PressInsertionsColumnIndex.MEDIA_INDEX]=current["media"].ToString();

					if (current["visual"]!=System.DBNull.Value){
						if (current["disponibility_visual"]!=System.DBNull.Value && 
							int.Parse(current["disponibility_visual"].ToString())<=10 &&
							current["activation"]!=System.DBNull.Value && 
							int.Parse(current["activation"].ToString())<=100){
							//visuels disponible
							string[] files = current["visual"].ToString().Split(',');
							string pathWeb=CstWeb.CreationServerPathes.IMAGES+"/"+current["id_media"].ToString()+"/"+current["date_cover_num"].ToString()+"/Imagette/";
							for(j=0;j<files.Length; j++){
								if(first){tab[i,CstWeb.PressInsertionsColumnIndex.VISUAL_INDEX]=pathWeb+files[j];}
								else{tab[i,CstWeb.PressInsertionsColumnIndex.VISUAL_INDEX]+=","+pathWeb+files[j];}
								first=false;
							}
							first=true;
						}
					}
					else
						tab[i,CstWeb.PressInsertionsColumnIndex.VISUAL_INDEX]="";
				}
				if(first)
					tab[i,CstWeb.PressInsertionsColumnIndex.LOCATION_INDEX]=current["location"].ToString();
				else
					tab[i,CstWeb.PressInsertionsColumnIndex.LOCATION_INDEX]+="<br> " + current["location"].ToString();
				first=false;
			}
			
			//finalisation du tableau
			i++;
			tab[i,0]=null;
			#endregion

			return tab;
		}
		#endregion

		#region TV
		/// <summary>
		/// Génère un tableau de données d'insertions TV préformaté utilisable pour la couche UI:
		///		["date","groupe","annonceur","produit","top diffusion","durée","rang","durée de l'écran","nb de spot de l'écran","prix","média","catégorie","support","fichier spot tv","Code écran"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataTV(DataSet ds){
			object[,] tab = new object[ds.Tables[0].Rows.Count+1,ds.Tables[0].Columns.Count];

			int i = -1;
			foreach(DataRow currentRow in ds.Tables[0].Rows){
				i++;
				tab[i,CstWeb.TVInsertionsColumnIndex.DATE_INDEX] = (new DateTime(
					int.Parse(currentRow["date_media_num"].ToString().Substring(0,4)),
					int.Parse(currentRow["date_media_num"].ToString().Substring(4,2)),
					int.Parse(currentRow["date_media_num"].ToString().Substring(6,2)))).ToString("dd/MM/yyyy");
				tab[i,CstWeb.TVInsertionsColumnIndex.ADVERTISER_INDEX]=currentRow["advertiser"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.PRODUCT_INDEX]=currentRow["product"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.GROUP_INDEX]=currentRow["group_"].ToString();
				#region Old version
				//tab[i,CstWeb.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX]=(new TimeSpan(
				//    currentRow["top_diffusion"].ToString().Length>=5?int.Parse(currentRow["top_diffusion"].ToString().Substring(0,currentRow["top_diffusion"].ToString().Length-4)):0,
				//    currentRow["top_diffusion"].ToString().Length>=3?int.Parse(currentRow["top_diffusion"].ToString().Substring(System.Math.Max(currentRow["top_diffusion"].ToString().Length-4,0),System.Math.Min(currentRow["top_diffusion"].ToString().Length-2,2))):0,
				//    int.Parse(currentRow["top_diffusion"].ToString().Substring(System.Math.Max(currentRow["top_diffusion"].ToString().Length-2,0),System.Math.Min(currentRow["top_diffusion"].ToString().Length,2))))
				//    ).ToString();
				#endregion
                if (Int64.Parse(currentRow["id_category"].ToString()) == TNS.AdExpress.Constantes.DB.Category.ID_DIGITAL_TV)
                {
                    tab[i, CstWeb.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX] = "";//Masquer top diffusion pour la TNT
                }
                else
                {
                    TimeSpan tSpan = (new TimeSpan(
                        currentRow["top_diffusion"].ToString().Length >= 5 ? int.Parse(currentRow["top_diffusion"].ToString().Substring(0, currentRow["top_diffusion"].ToString().Length - 4)) : 0,
                        currentRow["top_diffusion"].ToString().Length >= 3 ? int.Parse(currentRow["top_diffusion"].ToString().Substring(System.Math.Max(currentRow["top_diffusion"].ToString().Length - 4, 0), System.Math.Min(currentRow["top_diffusion"].ToString().Length - 2, 2))) : 0,
                        int.Parse(currentRow["top_diffusion"].ToString().Substring(System.Math.Max(currentRow["top_diffusion"].ToString().Length - 2, 0), System.Math.Min(currentRow["top_diffusion"].ToString().Length, 2))))
                        );
                    tab[i, CstWeb.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX] = ((tSpan.Hours.ToString().Length > 1) ? tSpan.Hours.ToString() : "0" + tSpan.Hours.ToString())
                    + ":" + ((tSpan.Minutes.ToString().Length > 1) ? tSpan.Minutes.ToString() : "0" + tSpan.Minutes.ToString())
                    + ":" + ((tSpan.Seconds.ToString().Length > 1) ? tSpan.Seconds.ToString() : "0" + tSpan.Seconds.ToString());
                }
                tab[i, CstWeb.TVInsertionsColumnIndex.ID_COMMERCIAL_BREAK_INDEX] = currentRow["id_commercial_break"].ToString();
                tab[i, CstWeb.TVInsertionsColumnIndex.DURATION_INDEX] = currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.duration].Id.ToString()].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.RANK_INDEX]=currentRow["id_rank"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.BREAK_DURATION_INDEX]=currentRow["duration_commercial_break"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX]=currentRow["number_message_commercial_brea"].ToString();
                tab[i, CstWeb.TVInsertionsColumnIndex.EXPENDITURE_INDEX] = currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX]=currentRow["associated_file"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.VEHICLE_INDEX]=currentRow["vehicle"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX]=currentRow["category"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX]=currentRow["media"].ToString();
			}
			
			tab[++i,0]=null;

			return tab;
		}
		#endregion
	
		#region Radio
		/// <summary>
		/// Génère un tableau de données d'insertions radio préformaté utilisable pour la couche UI:
		///		["date",groupe","annonceur","produit","top diffusion","durée","rang","durée de l'écran","rang HAP","durée écran HAP","nb spots HAP","nb de spot de l'écran","prix","média","catégorie","support","fichier spot radio"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataRadio(DataSet ds){
			object[,] tab = new object[ds.Tables[0].Rows.Count + 1, ds.Tables[0].Columns.Count + 1]; //-1
			
			int i = -1;
			foreach(DataRow currentRow in ds.Tables[0].Rows){
				i++;
				tab[i,CstWeb.RadioInsertionsColumnIndex.DATE_INDEX] = (new DateTime(
					int.Parse(currentRow["date_media_num"].ToString().Substring(0,4)),
					int.Parse(currentRow["date_media_num"].ToString().Substring(4,2)),
					int.Parse(currentRow["date_media_num"].ToString().Substring(6,2)))).ToString("dd/MM/yyyy");
				tab[i,CstWeb.RadioInsertionsColumnIndex.ADVERTISER_INDEX]=currentRow["advertiser"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.PRODUCT_INDEX]=currentRow["product"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.GROUP_INDEX]=currentRow["group_"].ToString();
				#region Old version
				//tab[i,CstWeb.RadioInsertionsColumnIndex.TOP_DIFFUSION_INDEX]=(new TimeSpan(
				//    currentRow["id_top_diffusion"].ToString().Length>=5?int.Parse(currentRow["id_top_diffusion"].ToString().Substring(0,currentRow["id_top_diffusion"].ToString().Length-4)):0,
				//    currentRow["id_top_diffusion"].ToString().Length>=3?int.Parse(currentRow["id_top_diffusion"].ToString().Substring(System.Math.Max(currentRow["id_top_diffusion"].ToString().Length-4,0),System.Math.Min(currentRow["id_top_diffusion"].ToString().Length-2,2))):0,
				//    int.Parse(currentRow["id_top_diffusion"].ToString().Substring(System.Math.Max(currentRow["id_top_diffusion"].ToString().Length-2,0),System.Math.Min(currentRow["id_top_diffusion"].ToString().Length,2))))
				//    ).ToString();
				#endregion
				TimeSpan tSpan = (new TimeSpan(
				   currentRow["id_top_diffusion"].ToString().Length >= 5 ? int.Parse(currentRow["id_top_diffusion"].ToString().Substring(0, currentRow["id_top_diffusion"].ToString().Length - 4)) : 0,
				   currentRow["id_top_diffusion"].ToString().Length >= 3 ? int.Parse(currentRow["id_top_diffusion"].ToString().Substring(System.Math.Max(currentRow["id_top_diffusion"].ToString().Length - 4, 0), System.Math.Min(currentRow["id_top_diffusion"].ToString().Length - 2, 2))) : 0,
				   int.Parse(currentRow["id_top_diffusion"].ToString().Substring(System.Math.Max(currentRow["id_top_diffusion"].ToString().Length - 2, 0), System.Math.Min(currentRow["id_top_diffusion"].ToString().Length, 2))))
				   );
				tab[i, CstWeb.RadioInsertionsColumnIndex.TOP_DIFFUSION_INDEX] = ((tSpan.Hours.ToString().Length > 1) ? tSpan.Hours.ToString() : "0" + tSpan.Hours.ToString())
				+ ":" + ((tSpan.Minutes.ToString().Length > 1) ? tSpan.Minutes.ToString() : "0" + tSpan.Minutes.ToString())
				+ ":" + ((tSpan.Seconds.ToString().Length > 1) ? tSpan.Seconds.ToString() : "0" + tSpan.Seconds.ToString());
			
				tab[i,CstWeb.RadioInsertionsColumnIndex.DURATION_INDEX]=currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.duration].Id.ToString()].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.RANK_INDEX]=currentRow["rank"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.BREAK_DURATION_INDEX]=currentRow["duration_commercial_break"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX]=currentRow["number_spot_com_break"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.RANK_WAP_INDEX]=currentRow["rank_wap"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.DURATION_BREAK_WAP_INDEX]=currentRow["duration_com_break_wap"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_WAP_NB_INDEX]=currentRow["number_spot_com_break_wap"].ToString();
                tab[i, CstWeb.RadioInsertionsColumnIndex.EXPENDITURE_INDEX] = currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.FILE_INDEX]=currentRow["associated_file"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.VEHICLE_INDEX]=currentRow["vehicle"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX]=currentRow["CATEGORY"].ToString();
				tab[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX]=currentRow["media"].ToString();
				if (currentRow["id_slogan"] != System.DBNull.Value) tab[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX] = currentRow["id_slogan"].ToString();
			}

			tab[++i,0]=null;

			return tab;
		}
		#endregion

		#region Publicité extérieure

		#region Old version
		///// <summary>
		///// Génère un tableau de données d'insertions Publicité extérieure préformaté utilisable pour la couche UI:
		/////		["date","groupe","annonceur","produit","nombre de panneaux","format","type de réseau","réseau afficheur","prix","média","catégorie","support","fichier affiche publicitaire"] (ordre non représentatif)
		///// </summary>
		///// <param name="ds">Données à traiter en provenance de la BD</param>
		///// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		//private static object[,] GetDataOutDoor(DataSet ds){
		//    object[,] tab = new object[ds.Tables[0].Rows.Count+1,ds.Tables[0].Columns.Count+1];
		//    int i = -1;
		//    foreach(DataRow currentRow in ds.Tables[0].Rows){
		//        i++;
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX] = (new DateTime(
		//            int.Parse(currentRow["date_media_num"].ToString().Substring(0,4)),
		//            int.Parse(currentRow["date_media_num"].ToString().Substring(4,2)),
		//            int.Parse(currentRow["date_media_num"].ToString().Substring(6,2)))).ToString("dd/MM/yyyy");
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX]=currentRow["advertiser"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX]=currentRow["product"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX]=currentRow["group_"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX]=currentRow["number_board"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX]=currentRow["type_board"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX]=currentRow["type_sale"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX]=currentRow["poster_network"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX]=currentRow["expenditure_euro"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.VEHICLE_INDEX]=currentRow["vehicle"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX]=currentRow["category"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX]=currentRow["media"].ToString();
		//        tab[i,CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX]=currentRow["agglomeration"].ToString();
		//    }
		//    tab[++i,0]=null;
		//    return tab;
		//}
		#endregion

		/// <summary>
		/// Génère un tableau de données d'insertions Publicité extérieure préformaté utilisable pour la couche UI:
		///		["date","groupe","annonceur","produit","nombre de panneaux","format","type de réseau","réseau afficheur","prix","média","catégorie","support","fichier affiche publicitaire"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <param name="webSession">session du client</param>		
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataOutDoor(DataSet ds, WebSession webSession) {

			object[,] tab = null;
			bool first = true;

			if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)) {
				return tab;
			}
			int i = -1;

			

				#region détail insertion sans les colonnes génériques
				tab = new object[ds.Tables[0].Rows.Count + 1, ds.Tables[0].Columns.Count + 1];


				foreach (DataRow currentRow in ds.Tables[0].Rows) {
					i++;
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX] = (new DateTime(
						int.Parse(currentRow["date_media_num"].ToString().Substring(0, 4)),
						int.Parse(currentRow["date_media_num"].ToString().Substring(4, 2)),
						int.Parse(currentRow["date_media_num"].ToString().Substring(6, 2)))).ToString("dd/MM/yyyy");
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX] = (currentRow["advertiser"] != System.DBNull.Value) ? currentRow["advertiser"].ToString() : "";
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX] = (currentRow["product"] != System.DBNull.Value) ? currentRow["product"].ToString() : "";
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX] = (currentRow["group_"] != System.DBNull.Value) ? currentRow["group_"].ToString() : "";
                    tab[i, CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX] = (currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.numberBoard].Id.ToString()] != System.DBNull.Value) ? currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.numberBoard].Id.ToString()].ToString() : "";
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX] = (currentRow["type_board"] != System.DBNull.Value) ? currentRow["type_board"].ToString() : "";
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX] = (currentRow["type_sale"] != System.DBNull.Value) ? currentRow["type_sale"].ToString() : "";
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX] = (currentRow["poster_network"] != System.DBNull.Value) ? currentRow["poster_network"].ToString() : "";
                    tab[i, CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX] = (currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()] != System.DBNull.Value) ? currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString() : "";
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.VEHICLE_INDEX] = (currentRow["vehicle"] != System.DBNull.Value) ? currentRow["vehicle"].ToString() : "";
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX] = (currentRow["category"] != System.DBNull.Value) ? currentRow["category"].ToString() : "";
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX] = (currentRow["media"] != System.DBNull.Value) ? currentRow["media"].ToString() : "";					
					tab[i, CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX] = (currentRow["agglomeration"] != System.DBNull.Value) ? currentRow["agglomeration"].ToString() : "";
					if (currentRow[ASSOCIATED_FILE] != System.DBNull.Value) {

						//visuels disponible
						string[] files = currentRow[ASSOCIATED_FILE].ToString().Split(',');
						string pathWeb = string.Empty;
						string idAssociatedFile = currentRow[ASSOCIATED_FILE].ToString();
						pathWeb = CstWeb.CreationServerPathes.IMAGES_OUTDOOR;
						string dir1 = idAssociatedFile.Substring(idAssociatedFile.Length - 8, 1);
						pathWeb = string.Format(@"{0}/{1}", pathWeb, dir1);
						string dir2 = idAssociatedFile.Substring(idAssociatedFile.Length - 9, 1);
						pathWeb = string.Format(@"{0}/{1}", pathWeb, dir2);
						string dir3 = idAssociatedFile.Substring(idAssociatedFile.Length - 10, 1);
						pathWeb = string.Format(@"{0}/{1}/Imagette/", pathWeb, dir3);

						for (int j = 0; j < files.Length; j++) {

							if (first) { tab[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX] = pathWeb + files[j]; }
							else { tab[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX] += "," + pathWeb + files[j]; }
							first = false;
						}
						first = true;

					}
					else
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX] = "";
					//if (ds.Tables[0].Columns.Contains("id_slogan") && currentRow["id_slogan"] != System.DBNull.Value && WebFunctions.MediaDetailLevel.HasSloganRight(webSession))
					//    tab[i, CstWeb.OutDoorInsertionsColumnIndex.ID_SLOGAN_INDEX] = currentRow["id_slogan"].ToString();

				}
				tab[++i, 0] = null;
				#endregion
			

			return tab;
		}

		#endregion

        #region Publicité interrieur        

        /// <summary>
        /// Génère un tableau de données d'insertions Publicité extérieure préformaté utilisable pour la couche UI:
        ///		["date","groupe","annonceur","produit","nombre de panneaux","format","type de réseau","réseau afficheur","prix","média","catégorie","support","fichier affiche publicitaire"] (ordre non représentatif)
        /// </summary>
        /// <param name="ds">Données à traiter en provenance de la BD</param>
        /// <param name="webSession">session du client</param>		
        /// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
        private static object[,] GetDataInStore(DataSet ds, WebSession webSession) {

            object[,] tab = null;
            bool first = true;

            if (!webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_INSTORE_ACCESS_FLAG)) {
                return tab;
            }
            int i = -1;



            #region détail insertion sans les colonnes génériques
            tab = new object[ds.Tables[0].Rows.Count + 1, ds.Tables[0].Columns.Count + 1];


            foreach (DataRow currentRow in ds.Tables[0].Rows) {
                i++;
                tab[i, CstWeb.InStoreInsertionsColumnIndex.DATE_INDEX] = (new DateTime(
                    int.Parse(currentRow["date_media_num"].ToString().Substring(0, 4)),
                    int.Parse(currentRow["date_media_num"].ToString().Substring(4, 2)),
                    int.Parse(currentRow["date_media_num"].ToString().Substring(6, 2)))).ToString("dd/MM/yyyy");
                tab[i, CstWeb.InStoreInsertionsColumnIndex.ADVERTISER_INDEX] = (currentRow["advertiser"] != System.DBNull.Value) ? currentRow["advertiser"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.PRODUCT_INDEX] = (currentRow["product"] != System.DBNull.Value) ? currentRow["product"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.GROUP_INDEX] = (currentRow["group_"] != System.DBNull.Value) ? currentRow["group_"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.NUMBER_BOARD_INDEX] = (currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.numberBoard].Id.ToString()] != System.DBNull.Value) ? currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.numberBoard].Id.ToString()].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.TYPE_BOARD_INDEX] = (currentRow["type_board"] != System.DBNull.Value) ? currentRow["type_board"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.TYPE_SALE_INDEX] = (currentRow["type_sale"] != System.DBNull.Value) ? currentRow["type_sale"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.POSTER_NETWORK_INDEX] = (currentRow["poster_network"] != System.DBNull.Value) ? currentRow["poster_network"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.EXPENDITURE_INDEX] = (currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()] != System.DBNull.Value) ? currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.VEHICLE_INDEX] = (currentRow["vehicle"] != System.DBNull.Value) ? currentRow["vehicle"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.CATEGORY_INDEX] = (currentRow["category"] != System.DBNull.Value) ? currentRow["category"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.MEDIA_INDEX] = (currentRow["media"] != System.DBNull.Value) ? currentRow["media"].ToString() : "";
                tab[i, CstWeb.InStoreInsertionsColumnIndex.AGGLOMERATION_INDEX] = (currentRow["agglomeration"] != System.DBNull.Value) ? currentRow["agglomeration"].ToString() : "";
                if (currentRow[ASSOCIATED_FILE] != System.DBNull.Value) {

                    //visuels disponible
                    string[] files = currentRow[ASSOCIATED_FILE].ToString().Split(',');
                    string pathWeb = string.Empty;
                    string idAssociatedFile = currentRow[ASSOCIATED_FILE].ToString();
                    pathWeb = CstWeb.CreationServerPathes.IMAGES_INSTORE;
                    string dir1 = idAssociatedFile.Substring(idAssociatedFile.Length - 8, 1);
                    pathWeb = string.Format(@"{0}/{1}", pathWeb, dir1);
                    string dir2 = idAssociatedFile.Substring(idAssociatedFile.Length - 9, 1);
                    pathWeb = string.Format(@"{0}/{1}", pathWeb, dir2);
                    string dir3 = idAssociatedFile.Substring(idAssociatedFile.Length - 10, 1);
                    pathWeb = string.Format(@"{0}/{1}/Imagette/", pathWeb, dir3);

                    for (int j = 0; j < files.Length; j++) {

                        if (first) { tab[i, CstWeb.InStoreInsertionsColumnIndex.FILES_INDEX] = pathWeb + files[j]; }
                        else { tab[i, CstWeb.InStoreInsertionsColumnIndex.FILES_INDEX] += "," + pathWeb + files[j]; }
                        first = false;
                    }
                    first = true;

                }
                else
                    tab[i, CstWeb.InStoreInsertionsColumnIndex.FILES_INDEX] = "";
                //if (ds.Tables[0].Columns.Contains("id_slogan") && currentRow["id_slogan"] != System.DBNull.Value && WebFunctions.MediaDetailLevel.HasSloganRight(webSession))
                //    tab[i, CstWeb.InStoreInsertionsColumnIndex.ID_SLOGAN_INDEX] = currentRow["id_slogan"].ToString();

            }
            tab[++i, 0] = null;
            #endregion


            return tab;
        }

        #endregion

		#endregion



	}
}
