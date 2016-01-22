#region Information
 /* auteur : D. Mussuma
 * créé le :  15/02/2007
 * modifié le :
 * */

#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.Date;
using TNS.FrameWork.DB.Common;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;


namespace TNS.AdExpress.Web.Rules.Results
{
	/// <summary>
	/// Description résumée de AlertsInsertionsCreationsRules.
	/// </summary>
	public class AlertsInsertionsCreationsRules
	{
		#region GetData
		/// <summary>
		/// Obtient les données du détail des insertions
		/// </summary>
        /// <param name="dataSource">Source de données</param>
        /// <param name="idVehicle">ID vehicle</param>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="siteLanguage">Langue du site</param>
		/// <returns>Liste du détail des insertions</returns>
		public static object[,] GetData(Int64 idVehicle,IDataSource dataSource,string idMedia, string idProduct,string dateBegin,string dateEnd,string siteLanguage){

			object[,] tab = null;
			
			DataSet ds = null;

			
			try{				

					//Recupère les données des insertions du support
					ds = AlertsInsertionsCreationsDataAccess.GetData(dataSource,idMedia,idProduct,dateBegin,dateEnd,idVehicle,siteLanguage);
				
					if(ds!=null && ds.Tables[0].Rows.Count>0){
				
						#region Construction du tableau 
						switch(VehiclesInformation.DatabaseIdToEnum(idVehicle)){
							case CstClassification.DB.Vehicles.names.internationalPress :
							case CstClassification.DB.Vehicles.names.press :
								return GetDataPress(ds);
							case CstClassification.DB.Vehicles.names.radio :
								return GetDataRadio(ds);
							case CstClassification.DB.Vehicles.names.tv :
							case CstClassification.DB.Vehicles.names.others :
								return GetDataTV(ds);
							case CstClassification.DB.Vehicles.names.outdoor :
                            case CstClassification.DB.Vehicles.names.instore:
								return GetDataOutDoor(ds);
							default:
								throw new AlertsInsertionsCreationsRulesException("Le vehicle demandé n'est pas un cas traité");
						}
						#endregion

					}
				
					
			}catch(Exception e){
				throw new AlertsInsertionsCreationsRulesException("Impossible de creer le tableau de données  ",e);

			}
			return tab;
		}
		#endregion

		#region Presse
		
		/// <summary>
		/// Génère un tableau de données d'insertions presses préformaté utilisables pour la couche UI:
		///		["date","page","groupz","annonceur","produit","format","surface","couleur","prix","média","catégorie","support","visuel1,visuel2,..."]
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>		
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataPress(DataSet ds){
			object[,] tab = null;
		
			string oldDate = "";
			string oldAdvertisement="";
			string oldMedia="";
			bool first=true;	
			int i = -1;
			int j = 0;

			#region Construction du tableau 

				tab = new object[ds.Tables[0].Rows.Count+1,ds.Tables[0].Columns.Count-4];

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
						tab[i,CstWeb.PressInsertionsColumnIndex.INTEREST_CENTER_INDEX]=current["interest_center"].ToString();
						tab[i,CstWeb.PressInsertionsColumnIndex.MEDIA_SELLER_INDEX]=current["media_seller"].ToString();
						
						if (current["visual"]!=System.DBNull.Value){
							if (current["disponibility_visual"]!=System.DBNull.Value && 
								int.Parse(current["disponibility_visual"].ToString())<=10 &&
								current["activation"]!=System.DBNull.Value && 
								int.Parse(current["activation"].ToString())<=100){
								//visuels disponible
								string[] files = current["visual"].ToString().Split(',');
								string pathWeb=CstWeb.CreationServerPathes.IMAGES+"/"+current["id_media"].ToString()+"/"+current["date_media_num"].ToString()+"/Imagette/";
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
					tab[i,CstWeb.PressInsertionsColumnIndex.ID_SLOGAN_INDEX]=current["id_slogan"].ToString();
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
			

			int i = -1;
			object[,] tab = null;
		
			#region Détail insertion 
			tab = new object[ds.Tables[0].Rows.Count+1,ds.Tables[0].Columns.Count];

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
				TimeSpan tSpan = (new TimeSpan(
							currentRow["top_diffusion"].ToString().Length >= 5 ? int.Parse(currentRow["top_diffusion"].ToString().Substring(0, currentRow["top_diffusion"].ToString().Length - 4)) : 0,
							currentRow["top_diffusion"].ToString().Length >= 3 ? int.Parse(currentRow["top_diffusion"].ToString().Substring(System.Math.Max(currentRow["top_diffusion"].ToString().Length - 4, 0), System.Math.Min(currentRow["top_diffusion"].ToString().Length - 2, 2))) : 0,
							int.Parse(currentRow["top_diffusion"].ToString().Substring(System.Math.Max(currentRow["top_diffusion"].ToString().Length - 2, 0), System.Math.Min(currentRow["top_diffusion"].ToString().Length, 2))))
							);
				tab[i, CstWeb.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX] = ((tSpan.Hours.ToString().Length > 1) ? tSpan.Hours.ToString() : "0" + tSpan.Hours.ToString())
				+ ":" + ((tSpan.Minutes.ToString().Length > 1) ? tSpan.Minutes.ToString() : "0" + tSpan.Minutes.ToString())
				+ ":" + ((tSpan.Seconds.ToString().Length > 1) ? tSpan.Seconds.ToString() : "0" + tSpan.Seconds.ToString());
				
				tab[i,CstWeb.TVInsertionsColumnIndex.ID_COMMERCIAL_BREAK_INDEX]=currentRow["id_commercial_break"].ToString();
                tab[i, CstWeb.TVInsertionsColumnIndex.DURATION_INDEX] = currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.duration].Id.ToString()].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.RANK_INDEX]=currentRow["id_rank"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.BREAK_DURATION_INDEX]=currentRow["duration_commercial_break"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX]=currentRow["number_message_commercial_brea"].ToString();
                tab[i, CstWeb.TVInsertionsColumnIndex.EXPENDITURE_INDEX] = currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX]=currentRow["associated_file"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.VEHICLE_INDEX]=currentRow["vehicle"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX]=currentRow["category"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX]=currentRow["media"].ToString();				
				tab[i,CstWeb.TVInsertionsColumnIndex.INTEREST_CENTER_INDEX]=currentRow["interest_center"].ToString();
				tab[i,CstWeb.TVInsertionsColumnIndex.MEDIA_SELLER_INDEX]=currentRow["media_seller"].ToString();							
				tab[i,CstWeb.TVInsertionsColumnIndex.ID_SLOGAN_INDEX]=currentRow["id_slogan"].ToString();
			}
			#endregion

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
			object[,] tab = null;
			
			int i = -1;
			
			#region détail insertion 

				tab = new object[ds.Tables[0].Rows.Count+1,ds.Tables[0].Columns.Count-1];

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

                    tab[i, CstWeb.RadioInsertionsColumnIndex.DURATION_INDEX] = currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.duration].Id.ToString()].ToString();
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
					tab[i,CstWeb.RadioInsertionsColumnIndex.INTEREST_CENTER_INDEX]=currentRow["interest_center"].ToString();
					tab[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_SELLER_INDEX]=currentRow["media_seller"].ToString();				
					tab[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX]=currentRow["media"].ToString();
					tab[i,CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX]=currentRow["id_slogan"].ToString();
				}

				tab[++i,0]=null;
				#endregion									

			return tab;
		}
		#endregion

		#region Publicité extérieure

		/// <summary>
		/// Génère un tableau de données d'insertions Publicité extérieure préformaté utilisable pour la couche UI:
		///		["date","groupe","annonceur","produit","nombre de panneaux","format","type de réseau","réseau afficheur","prix","média","catégorie","support","fichier affiche publicitaire"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataOutDoor(DataSet ds){
			
			object[,] tab =null;			
			int i = -1;
		
			

				#region détail insertion sans les colonnes génériques
				tab = new object[ds.Tables[0].Rows.Count+1,ds.Tables[0].Columns.Count+1];
			
					
				foreach(DataRow currentRow in ds.Tables[0].Rows){
					i++;
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX] = (new DateTime(
						int.Parse(currentRow["date_media_num"].ToString().Substring(0,4)),
						int.Parse(currentRow["date_media_num"].ToString().Substring(4,2)),
						int.Parse(currentRow["date_media_num"].ToString().Substring(6,2)))).ToString("dd/MM/yyyy");
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX]=currentRow["advertiser"].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX]=currentRow["product"].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX]=currentRow["group_"].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX]=currentRow[UnitsInformation.List[CstWeb.CustomerSessions.Unit.numberBoard].Id.ToString()].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX]=currentRow["type_board"].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX]=currentRow["type_sale"].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX]=currentRow["poster_network"].ToString();
                    tab[i, CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX] = currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.VEHICLE_INDEX]=currentRow["vehicle"].ToString();						
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX]=currentRow["category"].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX]=currentRow["media"].ToString();					
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.INTEREST_CENTER_INDEX]=currentRow["interest_center"].ToString();
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.MEDIA_SELLER_INDEX]=currentRow["media_seller"].ToString();					
					tab[i,CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX]=currentRow["agglomeration"].ToString();

				}
				tab[++i,0]=null;
				#endregion
																
			return tab;
		}
		#endregion
	}
}
