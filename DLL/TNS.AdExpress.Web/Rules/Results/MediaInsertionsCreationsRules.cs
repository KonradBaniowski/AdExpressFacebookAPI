#region Information
/*
 * auteur : Guillaume Ragneau
 * créé le :
 * modifié le : 24/08/2004
 * par : G. Facon
 * */
// K.Shehzad Addition of Agglomeration colunm for Outdoor creations 06/07/2005
//12/08/2005 :A.Dadouch modification des excpetion
//	12/12/2005	D. V. Mussuma	Gestion du niveau de détail média
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

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;


namespace TNS.AdExpress.Web.Rules.Results{
	/// <summary>
	/// Traitement des données issues de MediaInsertionsCreationDataAccess ==> tableau de données 
	/// prêtes à l'emploi pour la couche UI
	/// </summary>
	public class MediaInsertionsCreationsRules{
		
		#region Constantes
		const string DATE_MEDIA_NUM = "DATE_MEDIA_NUM";
		const string DAY_OF_WEEK = "DAY_OF_WEEK";
		const string TOP_DIFFUSION = "TOP_DIFFUSION";
		const string ID_TOP_DIFFUSION = "ID_TOP_DIFFUSION";
		const string ID_SLOGAN = "ID_SLOGAN";
		const string SLOGAN = "SLOGAN";
		const string ASSOCIATED_FILE = "ASSOCIATED_FILE";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MediaInsertionsCreationsRules(){
		}
		#endregion

		#region Traitement des données

		#region Traitement des données 
		/// <summary>
		/// Construction d'un tableau de données exploitables par la couche UI:
		///		Extraction des dates de calcul à partir des périodes de début et de fin
		///		Extraction des données à partir de la BD
		///		Extraction du tableau de données suivant le vehicle considéré
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idMediaLevel1">Identifiant Media 1</param>
		/// <param name="idMediaLevel2">Identifiant Media 2</param>
		/// <param name="idMediaLevel3">Identifiant Media niveau 3</param>
		/// <param name="idAdvertiser">Identifiant annonceur</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="idVehicle">Identifiant Média</param>
		/// <param name="mediaImpactedList">Liste des médias impacté par le détail</param>		
		/// <remarks>
		/// - Utilise les méthodes:
		///	<code>	public static DataSet TNS.AdExpress.Web.DataAccess.Results.MediaCreationDataAccess.GetData(WebSession webSession,ListDictionary mediaList,int dateBegin,int dateEnd,bool isMediaDetail,Int64 idVehicle)
		///		public static DataSet TNS.AdExpress.Web.DataAccess.Results.GetData(WebSession webSession,Int64 idVehicle,Int64 idCategory,Int64 idMedia,Int64 idAdvertiser,int dateBegin,int dateEnd)		
		///		private static object[,] GetDataPress(DataSet ds, WebSession webSession,bool isMediaDetail)
		///		private static object[,] GetDataTV(DataSet ds, WebSession webSession,bool isMediaDetail)
		///		private static object[,] GetDataRadio(DataSet ds, WebSession webSession,bool isMediaDetail)
		///		private static object[,] GetDataOutDoor(DataSet ds, WebSession webSession,bool isMediaDetail)</code>
		///	- Les paramètres string idMediaLevel1,string idMediaLevel2,string idMediaLevel3 sont essentiellement utilisés lorsqu'il y a des univers concurrents
		///	</remarks>
		///	<history>D. V. Mussuma 12/12/2005</history>
		/// <returns>Tableau de d'objets (données)</returns>
		public static object[,] GetData(WebSession webSession,string idMediaLevel1,string idMediaLevel2,string idMediaLevel3,string idAdvertiser ,int dateBegin, int dateEnd,string idVehicle,ListDictionary mediaImpactedList){	
			return GetData(webSession,idMediaLevel1,idMediaLevel2,idMediaLevel3,idAdvertiser,dateBegin,dateEnd,idVehicle,mediaImpactedList,null);
		}


		/// <summary>
		/// Construction d'un tableau de données exploitables par la couche UI:
		///		Extraction des dates de calcul à partir des périodes de début et de fin
		///		Extraction des données à partir de la BD
		///		Extraction du tableau de données suivant le vehicle considéré
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idMediaLevel1">Identifiant Media 1</param>
		/// <param name="idMediaLevel2">Identifiant Media 2</param>
		/// <param name="idMediaLevel3">Identifiant Media niveau 3</param>
		/// <param name="idAdvertiser">Identifiant annonceur</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>		
		/// <param name="idVehicle">Identifiant Média</param>
		/// <param name="mediaImpactedList">Liste des médias impacté par le détail</param>
		/// <param name="fieldsList">Liste des champs d'éléments  à traiter</param>
		/// <remarks>
		/// - Utilise les méthodes:
		///	<code>	public static DataSet TNS.AdExpress.Web.DataAccess.Results.MediaCreationDataAccess.GetData(WebSession webSession,ListDictionary mediaList,int dateBegin,int dateEnd,bool isMediaDetail,Int64 idVehicle)
		///		public static DataSet TNS.AdExpress.Web.DataAccess.Results.GetData(WebSession webSession,Int64 idVehicle,Int64 idCategory,Int64 idMedia,Int64 idAdvertiser,int dateBegin,int dateEnd)		
		///		private static object[,] GetDataPress(DataSet ds, WebSession webSession,bool isMediaDetail)
		///		private static object[,] GetDataTV(DataSet ds, WebSession webSession,bool isMediaDetail)
		///		private static object[,] GetDataRadio(DataSet ds, WebSession webSession,bool isMediaDetail)
		///		private static object[,] GetDataOutDoor(DataSet ds, WebSession webSession,bool isMediaDetail)</code>
		///	- Les paramètres string idMediaLevel1,string idMediaLevel2,string idMediaLevel3 sont essentiellement utilisés lorsqu'il y a des univers concurrents
		///	</remarks>
		///	<history>D. V. Mussuma 12/12/2005</history>
		/// <returns>Tableau de d'objets (données)</returns>
		public static object[,] GetData(WebSession webSession,string idMediaLevel1,string idMediaLevel2,string idMediaLevel3,string idAdvertiser ,int dateBegin, int dateEnd,string idVehicle,ListDictionary mediaImpactedList,ArrayList fieldsList){	
			#region variables		
			DataSet ds=null;
			string localIdVehicle="";
			
			#endregion

			try{
				#region Récupération des données					

                if(IsRequiredGenericColmuns(webSession) || IsMDSelected(idVehicle)){			
					//Récupération des données pour identifiant média (vehicle) issu de la sélection de l'onglet correspondant.
					if(idVehicle!=null && idVehicle.Length>0 && long.Parse(idVehicle.ToString())>-1){
						//Pas de droit publicité extérieure
						if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null 
							&& (CstClassification.DB.Vehicles.names)int.Parse(idVehicle.ToString())==CstClassification.DB.Vehicles.names.outdoor ){
							ds = null;
						}
						else{
                                if((CstClassification.DB.Vehicles.names)int.Parse(idVehicle.ToString())==CstClassification.DB.Vehicles.names.directMarketing)
                                    ds = MediaCreationDataAccess.GetMDData(webSession, mediaImpactedList, dateBegin, dateEnd, long.Parse(idVehicle),false);
                                else{
								ds = MediaCreationDataAccess.GetData(webSession,mediaImpactedList,dateBegin,dateEnd,long.Parse(idVehicle));
								GetFieldsList(webSession, ds.Tables[0], fieldsList, (CstClassification.DB.Vehicles.names)int.Parse(idVehicle.ToString()));
                                }
							
						}
					}												
				}
				else{
					//Récupération des données
						//Pas de droit publicité extérieure
						if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null 
							&& (CstClassification.DB.Vehicles.names)int.Parse(idMediaLevel1.ToString())==CstClassification.DB.Vehicles.names.outdoor ){
							ds = null;
						}
						else 
						ds = TNS.AdExpress.Web.DataAccess.Results.CompetitorMediaInsertionsCreationDataAccess.GetData(webSession, Int64.Parse(idMediaLevel1), Int64.Parse(idMediaLevel2), Int64.Parse(idMediaLevel3),Int64.Parse(idAdvertiser),dateBegin,dateEnd);
						localIdVehicle = idMediaLevel1;
					}
					
					//Aucune données
				if(idVehicle==null || idVehicle.Length==0  || ds==null || ds.Equals(System.DBNull.Value) || ds.Tables[0]==null 
					||  ds.Tables[0].Rows.Count==0)
					return null;
				#endregion		

				#region Construction du tableau 
				switch((CstClassification.DB.Vehicles.names) int.Parse(idVehicle)){
					case CstClassification.DB.Vehicles.names.internationalPress :
					case CstClassification.DB.Vehicles.names.press :
						return GetDataPress(ds,webSession,fieldsList);
					case CstClassification.DB.Vehicles.names.radio :
						return GetDataRadio(ds,webSession,fieldsList);
					case CstClassification.DB.Vehicles.names.tv :
					case CstClassification.DB.Vehicles.names.others :
						return GetDataTV(ds,webSession,fieldsList);
					case CstClassification.DB.Vehicles.names.outdoor :
						return GetDataOutDoor(ds,webSession,fieldsList);
                    case CstClassification.DB.Vehicles.names.directMarketing:
                        return GetDataMD(ds, webSession, fieldsList, mediaImpactedList);
					default:
						throw new MediaInsertionsCreationsRulesException("Le vehicle demandé n'est pas un cas traité");
				}
				#endregion
				
			}catch(Exception e){
				throw new MediaInsertionsCreationsRulesException("Impossible de creer le tableau de données pour MediaInsertionsCreationsRules",e);

			}
		}
		#endregion
			
		#region Presse
		/// <summary>
		/// Génère un tableau de données d'insertions presses préformaté utilisables pour la couche UI:
		///		["date","page","groupz","annonceur","produit","format","surface","couleur","prix","média","catégorie","support","visuel1,visuel2,..."]
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <param name="webSession">session du client</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataPress(DataSet ds, WebSession webSession){
			return GetDataPress(ds,webSession,null);
		}

		/// <summary>
		/// Génère un tableau de données d'insertions presses préformaté utilisables pour la couche UI:
		///		["date","page","groupz","annonceur","produit","format","surface","couleur","prix","média","catégorie","support","visuel1,visuel2,..."]
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <param name="webSession">session du client</param>		
		/// <param name="fieldsList">Liste des champs à récuperer</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataPress(DataSet ds, WebSession webSession,ArrayList fieldsList){
			object[,] tab = null;
		
			string oldDate = "";
			string oldAdvertisement="";
			string oldMedia="";
			bool first=true;	
			int i = -1;
			int j = 0;

		
			if(IsRequiredGenericColmuns(webSession)){

				#region Construction du tableau  des insertions avec les colonnes génériques
														
				int currentLine=0;
				int nbLines=0;
				bool isNewInsertion=true;
				bool isColumnsContainsIdMedia=ds.Tables[0].Columns.Contains("id_media");
				bool isColumnsContainsDateMediaNum=ds.Tables[0].Columns.Contains("date_media_num");
				bool isColumnsContainsDateCoverNum=ds.Tables[0].Columns.Contains("date_cover_num");
				bool isColumnsContainsIdAdvertisement=ds.Tables[0].Columns.Contains("id_advertisement");
				bool isColumnsContainsVisual=ds.Tables[0].Columns.Contains("visual");

					
				if(fieldsList!=null && fieldsList.Count>0){		
					//Nombre de lignes du tableau	
					for( i=0;i<ds.Tables[0].Rows.Count;i++){
						//Vérifie s'il s'agit d'une nouvelle insertion
						if( (isColumnsContainsDateMediaNum && ds.Tables[0].Rows[i]["date_media_num"].ToString().CompareTo(oldDate)!=0)
							|| (isColumnsContainsIdAdvertisement && ds.Tables[0].Rows[i]["id_advertisement"].ToString().CompareTo(oldAdvertisement)!=0)
							|| (isColumnsContainsIdMedia && ds.Tables[0].Rows[i]["id_media"].ToString().CompareTo(oldMedia)!=0)
							){
								
							if(isColumnsContainsIdMedia)
								oldMedia = ds.Tables[0].Rows[i]["id_media"].ToString();
							if(isColumnsContainsIdAdvertisement)
								oldAdvertisement=ds.Tables[0].Rows[i]["id_advertisement"].ToString();
							if(isColumnsContainsDateMediaNum)
								oldDate=ds.Tables[0].Rows[i]["date_media_num"].ToString();
							nbLines++;
						}
					}

					//Initialisation du tableau de résultats
					tab = new object[nbLines+1,fieldsList.Count];
						
							
					//1ere ligne contiendra les libellés des champs de données
					for(int k=0;k<fieldsList.Count;k++){
						tab[0,k]=fieldsList[k].ToString();
					}
						
					oldDate = "";
					oldAdvertisement="";
					oldMedia="";
					//On remplit chaque ligne du tableau de résultats
					for( i=0;i<ds.Tables[0].Rows.Count;i++){
						//Vérifie s'il s'agit d'une nouvelle insertion
						if( (isColumnsContainsDateMediaNum && ds.Tables[0].Rows[i]["date_media_num"].ToString().CompareTo(oldDate)!=0)
							|| (isColumnsContainsIdAdvertisement && ds.Tables[0].Rows[i]["id_advertisement"].ToString().CompareTo(oldAdvertisement)!=0)
							|| (isColumnsContainsIdMedia && ds.Tables[0].Rows[i]["id_media"].ToString().CompareTo(oldMedia)!=0)
							){
							isNewInsertion=true;
							if(isColumnsContainsIdMedia)
								oldMedia = ds.Tables[0].Rows[i]["id_media"].ToString();
							if(isColumnsContainsIdAdvertisement)
								oldAdvertisement=ds.Tables[0].Rows[i]["id_advertisement"].ToString();
							if(isColumnsContainsDateMediaNum)
								oldDate=ds.Tables[0].Rows[i]["date_media_num"].ToString();
							currentLine++;
						}
						else isNewInsertion=false;

						for( j=0;j<fieldsList.Count;j++){	
							if(isNewInsertion){//Nouvelle insetion
								
								if(isColumnsContainsVisual && fieldsList[j].ToString().Equals("VISUAL")){
									//Gestion colonne des visuel(s)
									if (ds.Tables[0].Rows[i]["visual"]!=System.DBNull.Value){
										if (ds.Tables[0].Rows[i]["disponibility_visual"]!=System.DBNull.Value && 
											int.Parse(ds.Tables[0].Rows[i]["disponibility_visual"].ToString())<=10 &&
											ds.Tables[0].Rows[i]["activation"]!=System.DBNull.Value && 
											int.Parse(ds.Tables[0].Rows[i]["activation"].ToString())<=100
											&& isColumnsContainsIdMedia
											&& isColumnsContainsDateCoverNum
											){
											//visuel(s) disponible(s)
											string[] files = ds.Tables[0].Rows[i]["visual"].ToString().Split(',');
											string pathWeb=CstWeb.CreationServerPathes.IMAGES+"/"+ds.Tables[0].Rows[i]["id_media"].ToString()+"/"+ds.Tables[0].Rows[i]["date_cover_num"].ToString()+"/Imagette/";
											int fileIndex=0;
											for(fileIndex=0;fileIndex<files.Length; fileIndex++){
												if(first){tab[currentLine,j]=pathWeb+files[fileIndex];}
												else{tab[currentLine,j]+=","+pathWeb+files[fileIndex];}
												first=false;
											}
											first=true;
										}
									}
									else
										tab[currentLine,j]="";

								}else if(!fieldsList[j].ToString().Equals("DISPONIBILITY_VISUAL") 
									&& !fieldsList[j].ToString().Equals("ACTIVATION"))
									
									tab[currentLine,j]=ApplyColumnRule(webSession,fieldsList[j].ToString(),ds.Tables[0].Rows[i][fieldsList[j].ToString()]);//Autres colonnes
							}
							else{
								//Gestion des emplacements
								if(fieldsList[j].ToString().Equals("LOCATION")){
									tab[currentLine,j]+="<br> " +ds.Tables[0].Rows[i][j].ToString();
								}
							}																			
						}

					}
				}
					
				

					
				  
				#endregion

			}else{

				#region Construction du tableau sans gestion des colonnes génériques

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
						tab[i,CstWeb.PressInsertionsColumnIndex.AREA_PAGE_INDEX]=float.Parse(current["area_page"].ToString())/1000;
						tab[i,CstWeb.PressInsertionsColumnIndex.COLOR_INDEX]=current["color"].ToString();
						tab[i,CstWeb.PressInsertionsColumnIndex.EXPENDITURE_INDEX]=current["expenditure_euro"].ToString();
						tab[i,CstWeb.PressInsertionsColumnIndex.VEHICLE_INDEX]=current["vehicle"].ToString();					
						tab[i,CstWeb.PressInsertionsColumnIndex.CATEGORY_INDEX]=current["category"].ToString();
						tab[i,CstWeb.PressInsertionsColumnIndex.MEDIA_INDEX]=current["media"].ToString();
						if(!webSession.isCompetitorAdvertiserSelected()){
							tab[i,CstWeb.PressInsertionsColumnIndex.INTEREST_CENTER_INDEX]=current["interest_center"].ToString();
							tab[i,CstWeb.PressInsertionsColumnIndex.MEDIA_SELLER_INDEX]=current["media_seller"].ToString();
						}

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
					if(WebFunctions.MediaDetailLevel.HasSloganRight(webSession) && ds.Tables[0].Columns.Contains("id_slogan"))
						tab[i,CstWeb.PressInsertionsColumnIndex.ID_SLOGAN_INDEX]=current["id_slogan"].ToString();
				}
			
				//finalisation du tableau
				i++;
				tab[i,0]=null;
				#endregion				
				
			}

		

			return tab;
		}
		#endregion

		#region TV
		/// <summary>
		/// Génère un tableau de données d'insertions TV préformaté utilisable pour la couche UI:
		///		["date","groupe","annonceur","produit","top diffusion","durée","rang","durée de l'écran","nb de spot de l'écran","prix","média","catégorie","support","fichier spot tv","Code écran"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <param name="webSession">session du client</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataTV(DataSet ds, WebSession webSession){
			return GetDataTV(ds,webSession,null);
		}
//		private static object[,] GetDataTV(DataSet ds, WebSession webSession,bool isMediaDetail){
//			return GetDataTV(ds,webSession,isMediaDetail,null);
//		}
		/// <summary>
		/// Génère un tableau de données d'insertions TV préformaté utilisable pour la couche UI:
		///		["date","groupe","annonceur","produit","top diffusion","durée","rang","durée de l'écran","nb de spot de l'écran","prix","média","catégorie","support","fichier spot tv","Code écran"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <param name="webSession">session du client</param>
		/// <param name="fieldsList">Liste des champs à récuperer</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataTV(DataSet ds, WebSession webSession,ArrayList fieldsList){
			

			int i = -1;
			object[,] tab = null;
			if(IsRequiredGenericColmuns(webSession)){

					
				#region Détail insertion avec les colonnes génériques
				
				if(fieldsList!=null && fieldsList.Count>0){					 										  
					tab = new object[ds.Tables[0].Rows.Count+1,fieldsList.Count];
						
					//1ere ligne contiendra les libellés des champs de données
					for(int k=0;k<fieldsList.Count;k++){
						tab[0,k]=fieldsList[k].ToString();
					}

					//On remplit chaque ligne du tableau de résultats
					for( i=0;i<ds.Tables[0].Rows.Count;i++){
						for(int j=0;j<fieldsList.Count;j++){							 
							tab[i+1,j]=ApplyColumnRule(webSession,fieldsList[j].ToString(),ds.Tables[0].Rows[i][fieldsList[j].ToString()]);						
						}
					}
				}
				#endregion

			}else{
					#region Détail insertion sans les colonnes génériques
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
						tab[i,CstWeb.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX] = ((tSpan.Hours.ToString().Length > 1) ? tSpan.Hours.ToString() : "0" + tSpan.Hours.ToString())
						+ ":" + ((tSpan.Minutes.ToString().Length > 1) ? tSpan.Minutes.ToString() : "0" + tSpan.Minutes.ToString())
						+ ":" + ((tSpan.Seconds.ToString().Length > 1) ? tSpan.Seconds.ToString() : "0" + tSpan.Seconds.ToString());
			
						tab[i,CstWeb.TVInsertionsColumnIndex.ID_COMMERCIAL_BREAK_INDEX]=currentRow["id_commercial_break"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.DURATION_INDEX]=currentRow["duration"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.RANK_INDEX]=currentRow["id_rank"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.BREAK_DURATION_INDEX]=currentRow["duration_commercial_break"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX]=currentRow["number_message_commercial_brea"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.EXPENDITURE_INDEX]=currentRow["expenditure_euro"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX]=currentRow["associated_file"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.VEHICLE_INDEX]=currentRow["vehicle"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX]=currentRow["category"].ToString();
						tab[i,CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX]=currentRow["media"].ToString();
						if(!webSession.isCompetitorAdvertiserSelected()){
							tab[i,CstWeb.TVInsertionsColumnIndex.INTEREST_CENTER_INDEX]=currentRow["interest_center"].ToString();
							tab[i,CstWeb.TVInsertionsColumnIndex.MEDIA_SELLER_INDEX]=currentRow["media_seller"].ToString();
						}
						if(WebFunctions.MediaDetailLevel.HasSloganRight(webSession) && ds.Tables[0].Columns.Contains("id_slogan"))
							tab[i,CstWeb.TVInsertionsColumnIndex.ID_SLOGAN_INDEX]=currentRow["id_slogan"].ToString();
					}
					#endregion

					tab[++i,0]=null;
				
			}
			
			

			return tab;
		}
		#endregion
	
		#region Radio

		/// <summary>
		/// Génère un tableau de données d'insertions radio préformaté utilisable pour la couche UI:
		///		["date",groupe","annonceur","produit","top diffusion","durée","rang","durée de l'écran","rang HAP","durée écran HAP","nb spots HAP","nb de spot de l'écran","prix","média","catégorie","support","fichier spot radio"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		///  <param name="webSession">session du client</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataRadio(DataSet ds, WebSession webSession){
			return GetDataRadio(ds,webSession,null);
		}


		/// <summary>
		/// Génère un tableau de données d'insertions radio préformaté utilisable pour la couche UI:
		///		["date",groupe","annonceur","produit","top diffusion","durée","rang","durée de l'écran","rang HAP","durée écran HAP","nb spots HAP","nb de spot de l'écran","prix","média","catégorie","support","fichier spot radio"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		///  <param name="webSession">session du client</param>
		/// <param name="fieldsList">Liste des champs à récuperer</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataRadio(DataSet ds, WebSession webSession,ArrayList fieldsList){
			object[,] tab = null;
			
			int i = -1;

			if(IsRequiredGenericColmuns(webSession)){
					

				#region détail insertion avec les colonnes génériques

				
				if(fieldsList!=null && fieldsList.Count>0){	
					tab = new object[ds.Tables[0].Rows.Count+1,fieldsList.Count];

							
					//1ere ligne contiendra les libellés des champs de données
					for(int k=0;k<fieldsList.Count;k++){
						tab[0,k]=fieldsList[k].ToString();
					}

					//On remplit chaque ligne du tableau de résultats
					for( i=0;i<ds.Tables[0].Rows.Count;i++){
						for(int j=0;j<fieldsList.Count;j++){	
							tab[i+1,j]=ApplyColumnRule(webSession,fieldsList[j].ToString(),ds.Tables[0].Rows[i][fieldsList[j].ToString()]);						
						
						}
					}
				}
				  
				#endregion

			}else{

					#region détail insertion sans les colonnes génériques

				tab = new object[ds.Tables[0].Rows.Count + 1, ds.Tables[0].Columns.Count + 1];//-1

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
			
						tab[i,CstWeb.RadioInsertionsColumnIndex.DURATION_INDEX]=currentRow["duration"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.RANK_INDEX]=currentRow["rank"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.BREAK_DURATION_INDEX]=currentRow["duration_commercial_break"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX]=currentRow["number_spot_com_break"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.RANK_WAP_INDEX]=currentRow["rank_wap"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.DURATION_BREAK_WAP_INDEX]=currentRow["duration_com_break_wap"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_WAP_NB_INDEX]=currentRow["number_spot_com_break_wap"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.EXPENDITURE_INDEX]=currentRow["expenditure_euro"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.FILE_INDEX]=currentRow["associated_file"].ToString();
						tab[i,CstWeb.RadioInsertionsColumnIndex.VEHICLE_INDEX]=currentRow["vehicle"].ToString();
 
						tab[i,CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX]=currentRow["CATEGORY"].ToString();
						if(!webSession.isCompetitorAdvertiserSelected()){
							tab[i,CstWeb.RadioInsertionsColumnIndex.INTEREST_CENTER_INDEX]=currentRow["interest_center"].ToString();
							tab[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_SELLER_INDEX]=currentRow["media_seller"].ToString();
						}
						tab[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX]=currentRow["media"].ToString();
						if ( ds.Tables[0].Columns.Contains("id_slogan") && currentRow["id_slogan"] !=System.DBNull.Value) //WebFunctions.MediaDetailLevel.HasSloganRight(webSession) &&
							tab[i,CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX]=currentRow["id_slogan"].ToString();
					}

					tab[++i,0]=null;
					#endregion
			
			}
			

			return tab;
		}
		#endregion

		#region Publicité extérieure

		/// <summary>
		/// Génère un tableau de données d'insertions Publicité extérieure préformaté utilisable pour la couche UI:
		///		["date","groupe","annonceur","produit","nombre de panneaux","format","type de réseau","réseau afficheur","prix","média","catégorie","support","fichier affiche publicitaire"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <param name="webSession">session du client</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataOutDoor(DataSet ds, WebSession webSession){
			return GetDataOutDoor(ds,webSession,null);
		}

		/// <summary>
		/// Génère un tableau de données d'insertions Publicité extérieure préformaté utilisable pour la couche UI:
		///		["date","groupe","annonceur","produit","nombre de panneaux","format","type de réseau","réseau afficheur","prix","média","catégorie","support","fichier affiche publicitaire"] (ordre non représentatif)
		/// </summary>
		/// <param name="ds">Données à traiter en provenance de la BD</param>
		/// <param name="webSession">session du client</param>
		///<param name="fieldsList">Liste des champs à récuperer</param>
		/// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
		private static object[,] GetDataOutDoor(DataSet ds, WebSession webSession,ArrayList fieldsList){
			
			object[,] tab =null;
			bool first = true;	

			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null){
				return tab;
			}
			int i = -1;
		
			if(IsRequiredGenericColmuns(webSession)){
			

				#region détail insertion avec les colonnes génériques

					
				if(fieldsList!=null && fieldsList.Count>0){		
					tab = new object[ds.Tables[0].Rows.Count+1,fieldsList.Count];

							
					//1ere ligne contiendra les libellés des champs de données
					for(int k=0;k<fieldsList.Count;k++){
						tab[0,k]=fieldsList[k].ToString();
					}
					bool isColumnsContainsVisual = (ds !=null) && ds.Tables[0].Columns.Contains(ASSOCIATED_FILE);

					//On remplit chaque ligne du tableau de résultats
					for( i=0;i<ds.Tables[0].Rows.Count;i++){
						for(int j=0;j<fieldsList.Count;j++){
							if (ds.Tables[0].Columns.Contains(ASSOCIATED_FILE) && fieldsList[j].ToString().Equals(ASSOCIATED_FILE)) {

								#region Construction chemin d'accès aux visuels
								
#if Debug
									//if (isColumnsContainsVisual){ // && ds.Tables[0].Rows[i][ASSOCIATED_FILE] != System.DBNull.Value) {

									////visuels disponible
									//string[] files = "25380719001.jpg".Split(',');
									//string pathWeb = string.Empty;
									//string idAssociatedFile = "25380719001.jpg";
#endif
								if (isColumnsContainsVisual && ds.Tables[0].Rows[i][ASSOCIATED_FILE] != System.DBNull.Value) {

									//visuels disponible
									string[] files = ds.Tables[0].Rows[i][ASSOCIATED_FILE].ToString().Split(',');
									string pathWeb = string.Empty;
									string idAssociatedFile = ds.Tables[0].Rows[i][ASSOCIATED_FILE].ToString();

									pathWeb = CstWeb.CreationServerPathes.IMAGES_OUTDOOR;
									string dir1 = idAssociatedFile.Substring(idAssociatedFile.Length - 8, 1);
									pathWeb = string.Format(@"{0}/{1}", pathWeb, dir1);
									string dir2 = idAssociatedFile.Substring(idAssociatedFile.Length - 9, 1);
									pathWeb = string.Format(@"{0}/{1}", pathWeb, dir2);
									string dir3 = idAssociatedFile.Substring(idAssociatedFile.Length - 10, 1);
									pathWeb = string.Format(@"{0}/{1}/Imagette/", pathWeb, dir3);

									for (int m = 0; m < files.Length; m++) {

										if (first) { tab[i + 1, j] = pathWeb + files[m]; }
										else { tab[i + 1, j] += "," + pathWeb + files[m]; }
										first = false;
									}
									first = true;

								}
								else
									tab[i + 1, j] = "";
								#endregion

							}
							else
							tab[i+1,j]=ApplyColumnRule(webSession,fieldsList[j].ToString(),ds.Tables[0].Rows[i][fieldsList[j].ToString()]);						
						
						}
					}
				}
				  
				#endregion

			}else{

				#region détail insertion sans les colonnes génériques
					tab = new object[ds.Tables[0].Rows.Count+1,ds.Tables[0].Columns.Count+1];
			
					
					foreach(DataRow currentRow in ds.Tables[0].Rows){
						i++;
						tab[i,CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX] = (new DateTime(
							int.Parse(currentRow["date_media_num"].ToString().Substring(0,4)),
							int.Parse(currentRow["date_media_num"].ToString().Substring(4,2)),
							int.Parse(currentRow["date_media_num"].ToString().Substring(6,2)))).ToString("dd/MM/yyyy");
						tab[i,CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX] =(currentRow["advertiser"] != System.DBNull.Value)? currentRow["advertiser"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX] = (currentRow["product"] != System.DBNull.Value) ? currentRow["product"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX] = (currentRow["group_"] != System.DBNull.Value) ? currentRow["group_"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX] = (currentRow["number_board"] != System.DBNull.Value) ? currentRow["number_board"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX] = (currentRow["type_board"] != System.DBNull.Value) ? currentRow["type_board"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX] = (currentRow["type_sale"] != System.DBNull.Value) ? currentRow["type_sale"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX] = (currentRow["poster_network"] != System.DBNull.Value) ? currentRow["poster_network"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX] = (currentRow["expenditure_euro"] != System.DBNull.Value) ? currentRow["expenditure_euro"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.VEHICLE_INDEX] = (currentRow["vehicle"] != System.DBNull.Value) ? currentRow["vehicle"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX] = (currentRow["category"] != System.DBNull.Value) ? currentRow["category"].ToString() : "";
						tab[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX] = (currentRow["media"] != System.DBNull.Value) ? currentRow["media"].ToString() : ""; 
						if(!webSession.isCompetitorAdvertiserSelected()){
							tab[i, CstWeb.OutDoorInsertionsColumnIndex.INTEREST_CENTER_INDEX] = (currentRow["interest_center"] != System.DBNull.Value) ? currentRow["interest_center"].ToString() : "";
							tab[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_SELLER_INDEX] = (currentRow["media_seller"] != System.DBNull.Value) ? currentRow["media_seller"].ToString() : ""; 
						}
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
						if ( ds.Tables[0].Columns.Contains("id_slogan") && currentRow["id_slogan"] != System.DBNull.Value && WebFunctions.MediaDetailLevel.HasSloganRight(webSession) )
							tab[i, CstWeb.OutDoorInsertionsColumnIndex.ID_SLOGAN_INDEX] = currentRow["id_slogan"].ToString();

					}
					tab[++i,0]=null;
					#endregion										
			}
			
			return tab;
		}
		#endregion

        #region Marketing Direct
        /// <summary>
        /// Génère un tableau de données des versions du MD préformaté utilisables pour la couche UI:
        /// </summary>
        /// <param name="ds">Données à traiter en provenance de la BD</param>
        /// <param name="webSession">session du client</param>
        /// <param name="mediaImpactedList">Liste des médias impacté par le détail</param>
        /// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
        private static object[,] GetDataMD(DataSet ds, WebSession webSession, ListDictionary mediaImpactedList){
            return GetDataMD(ds, webSession, null,mediaImpactedList);
        }

        /// <summary>
        /// Génère un tableau de données des versions du MD préformaté utilisables pour la couche UI:
        /// </summary>
        /// <param name="ds">Données à traiter en provenance de la BD</param>
        /// <param name="webSession">session du client</param>		
        /// <param name="fieldsList">Liste des champs à récuperer</param>
        /// <param name="mediaImpactedList">Liste des médias impacté par le détail</param>
        /// <returns>Tableau d'objets contenant les données issues de la BD traitées</returns>
        private static object[,] GetDataMD(DataSet ds, WebSession webSession, ArrayList fieldsList, ListDictionary mediaImpactedList){
            object[,] tab = null;

            string oldDate = "";
            string oldAdvertisement = "";
            string oldMedia = "";
            string oldProduct = "";
            bool first = true;
            int i = -1;     
            int j = 0;

            tab = new object[ds.Tables[0].Rows.Count + 1, ds.Tables[0].Columns.Count];

            foreach (DataRow current in ds.Tables[0].Rows){

                //if (current["date_media_num"].ToString().CompareTo(oldDate) != 0
                //    || current["id_advertiser"].ToString().CompareTo(oldAdvertisement) != 0
                //    || current["id_media"].ToString().CompareTo(oldMedia) != 0
                //    || current["id_product"].ToString().CompareTo(oldProduct) != 0)
                {

                    //nouvel insertion
                    oldMedia = current["id_media"].ToString();
                    oldAdvertisement = current["id_advertiser"].ToString();
                    oldProduct = current["id_product"].ToString();
                    oldDate = current["date_media_num"].ToString();
                    first = true;
                    i++;
                    for (j = 0; j < tab.GetLength(1); j++){
                        tab[i, j] = "";
                    }
                    tab[i, CstWeb.MDVersionsColumnIndex.DATE_INDEX] = (new DateTime(int.Parse(current["date_media_num"].ToString().Substring(0, 4)),
                        int.Parse(current["date_media_num"].ToString().Substring(4, 2)),
                        int.Parse(current["date_media_num"].ToString().Substring(6, 2)))).ToString("dd/MM/yyyy");
                    tab[i, CstWeb.MDVersionsColumnIndex.GROUP_INDEX] = current["group_"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.ADVERTISER_INDEX] = current["advertiser"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.PRODUCT_INDEX] = current["product"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.EXPENDITURE_INDEX] = current["expenditure_euro"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.CATEGORY_INDEX] = current["category"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.ID_MEDIA_INDEX] = current["id_media"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.MEDIA_INDEX] = current["media"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.WEIGHT_INDEX] = current["weight"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.VOLUME_INDEX] = current["volume"].ToString();
                    tab[i, CstWeb.MDVersionsColumnIndex.SLOGAN_INDEX] = current["id_slogan"].ToString();

                    if (mediaImpactedList["id_media"] != null  && mediaImpactedList["id_media"].ToString() != "-1"){

                        switch (mediaImpactedList["id_media"].ToString()){

                            case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                                tab[i, CstWeb.MDVersionsColumnIndex.FORMAT_INDEX] = current["format"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAIL_FORMAT_INDEX] = current["mail_format"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAIL_TYPE_INDEX] = current["mail_type"].ToString();
                                break;
                            case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                                tab[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX] = current["wp_mail_format"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX] = current["object_count"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX] = GetItemContentList(current);
                                break;
                            case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                                tab[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX] = current["object_count"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX] = GetItemContentList(current);
                                break;
                            case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                                tab[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX] = current["object_count"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAILING_RAPIDITY_INDEX] = current["mailing_rapidity"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX] = GetItemContentList(current);
                                break;
                            default:
                                throw new Exceptions.MediaInsertionsCreationsRulesException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas à un support du MD.");
                        }

                    }
                    else if (mediaImpactedList["id_category"] != null && mediaImpactedList["id_category"].ToString() != "-1"){

                        switch (mediaImpactedList["id_category"].ToString()){

                            case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                                tab[i, CstWeb.MDVersionsColumnIndex.FORMAT_INDEX] = current["format"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAIL_FORMAT_INDEX] = current["mail_format"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAIL_TYPE_INDEX] = current["mail_type"].ToString();
                                break;
                            case DBConstantes.Category.COURRIER_ADRESSE:
                                tab[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX] = current["wp_mail_format"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX] = current["object_count"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAILING_RAPIDITY_INDEX] = current["mailing_rapidity"].ToString();
                                tab[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX] = GetItemContentList(current);
                                break;
                            default:
                                throw new Exceptions.MediaInsertionsCreationsRulesException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette catégorie ne correspond pas à une catégorie du MD.");
                        }

                    }
                    else{

                        tab[i, CstWeb.MDVersionsColumnIndex.FORMAT_INDEX] = current["format"].ToString();
                        tab[i, CstWeb.MDVersionsColumnIndex.MAIL_FORMAT_INDEX] = current["mail_format"].ToString();
                        tab[i, CstWeb.MDVersionsColumnIndex.MAIL_TYPE_INDEX] = current["mail_type"].ToString();
                        tab[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX] = current["wp_mail_format"].ToString();
                        tab[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX] = current["object_count"].ToString();
                        tab[i, CstWeb.MDVersionsColumnIndex.MAILING_RAPIDITY_INDEX] = current["mailing_rapidity"].ToString();
                        tab[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX] = GetItemContentList(current);

                    }

                    if (current["associated_file"] != System.DBNull.Value){

                        //visuels disponible
                        string[] files = current["associated_file"].ToString().Split(',');
                        //string pathWeb = CstWeb.CreationServerPathes.IMAGES_MD + "/" + current["id_media"].ToString() + "/" + current["date_media_num"].ToString() + "/Imagette/";
                        string pathWeb = string.Empty;
                        string idSlogan = current["associated_file"].ToString();
                        pathWeb = CstWeb.CreationServerPathes.IMAGES_MD;
                        string dir1 = idSlogan.Substring(idSlogan.Length - 8, 1);
                        pathWeb = string.Format(@"{0}/{1}", pathWeb, dir1);
                        string dir2 = idSlogan.Substring(idSlogan.Length - 9, 1);
                        pathWeb = string.Format(@"{0}/{1}", pathWeb, dir2);
                        string dir3 = idSlogan.Substring(idSlogan.Length - 10, 1);
                        pathWeb = string.Format(@"{0}/{1}/imagette/", pathWeb, dir3);

                        for (j = 0; j < files.Length; j++){

                            if (first) { tab[i, CstWeb.MDVersionsColumnIndex.ASSOCIATED_FILE_INDEX] = pathWeb + files[j]; }
                            else { tab[i, CstWeb.MDVersionsColumnIndex.ASSOCIATED_FILE_INDEX] += "," + pathWeb + files[j]; }
                            first = false;
                        }
                        first = true;
                        
                    }
                    else
                        tab[i, CstWeb.MDVersionsColumnIndex.ASSOCIATED_FILE_INDEX] = "";
                }

            }

            //finalisation du tableau
            i++;
            tab[i, 0] = null;
            
            return tab;
        }
        #endregion

		#region Identification des médias impliqués
		

		/// <summary>
		/// Obtient les identifiants des médias (vehicle) à traiter (vehicle)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="mediaImpactedList">Liste des médias impactés</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>identifiants des médias</returns>
		internal static ArrayList GetIdsVehicle(WebSession webSession,ListDictionary mediaImpactedList,string dateBegin,string dateEnd){
			
			#region variables
			ArrayList vehicleList=null;
			DataSet ds=null;
			//vehicle en accès
			string vehicleAccessList = webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);	
			string[] idSelectedVehicle = null;
			if(vehicleAccessList.Length>0)idSelectedVehicle = vehicleAccessList.Split(',');
			string tableName="";
			Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
			

			#endregion

			#region Identification de(s) média(s) (vehicle) sélection

			if(mediaImpactedList!=null && mediaImpactedList.Count>0){
				vehicleList = new ArrayList();	
			
				//Si Parraiange TV
				if(WebFunctions.Modules.IsSponsorShipTVModule(webSession)){
					vehicleList.Add(CstClassification.DB.Vehicles.names.tv.GetHashCode().ToString());
					return vehicleList;	
				}

				//Cas  média demandé (vehicle) 					
				if( mediaImpactedList["id_vehicle"]!=null && long.Parse(mediaImpactedList["id_vehicle"].ToString())>-1
					//&&
					//!(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null 
					//&& (CstClassification.DB.Vehicles.names)int.Parse(mediaImpactedList["id_vehicle"].ToString())== CstClassification.DB.Vehicles.names.outdoor 
					//)						
					){

					if(mediaImpactedList["id_vehicle"] != null && mediaImpactedList["id_vehicle"].ToString().Length>0 && int.Parse(mediaImpactedList["id_vehicle"].ToString()) == CstClassification.DB.Vehicles.names.internet.GetHashCode()){
						//Remplacer média Internet par Adnettrack
						vehicleList.Add(CstClassification.DB.Vehicles.names.adnettrack.GetHashCode());
					}
					else vehicleList.Add(mediaImpactedList["id_vehicle"].ToString());
					return vehicleList;					
				}

				//Cas catégorie demandée				
				if(mediaImpactedList["id_category"]!=null && long.Parse(mediaImpactedList["id_category"].ToString())>-1){
					ds = MediaCreationDataAccess.GetIdsVehicle(webSession,long.Parse(mediaImpactedList["id_category"].ToString()),-1);
					if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0){
						foreach(DataRow dr in ds.Tables[0].Rows){
							
							//if( !(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null 
							//    && (CstClassification.DB.Vehicles.names)int.Parse(dr["id_vehicle"].ToString())==CstClassification.DB.Vehicles.names.outdoor )
							//    ){
								if(dr["id_vehicle"] != null && dr["id_vehicle"].ToString().Length>0 && int.Parse(dr["id_vehicle"].ToString()) == CstClassification.DB.Vehicles.names.internet.GetHashCode()){
									//Remplacer média Internet par Adnettrack
									vehicleList.Add(CstClassification.DB.Vehicles.names.adnettrack.GetHashCode());
								}
								else vehicleList.Add(dr["id_vehicle"].ToString());
							//}
						}
						return vehicleList;
					}
				}

				//Cas support demandé				
				if(mediaImpactedList["id_media"]!=null && long.Parse(mediaImpactedList["id_media"].ToString())>-1){
					ds = MediaCreationDataAccess.GetIdsVehicle(webSession,-1,long.Parse(mediaImpactedList["id_media"].ToString()));
					if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0){
						foreach(DataRow dr in ds.Tables[0].Rows){
							//if( !(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null 
							//    && (CstClassification.DB.Vehicles.names)int.Parse(dr["id_vehicle"].ToString())==CstClassification.DB.Vehicles.names.outdoor )
							//    ){
								if(dr["id_vehicle"] != null && dr["id_vehicle"].ToString().Length>0 && int.Parse(dr["id_vehicle"].ToString()) == CstClassification.DB.Vehicles.names.internet.GetHashCode()){
									//Remplacer média Internet par Adnettrack
									vehicleList.Add(CstClassification.DB.Vehicles.names.adnettrack.GetHashCode());
								}
								else vehicleList.Add(dr["id_vehicle"].ToString());
							//}
						}
						return vehicleList;
					}
				}

				//Cas accroche demandée								
				if((mediaImpactedList["id_slogan"]!=null && long.Parse(mediaImpactedList["id_slogan"].ToString())>-1)){
					if(long.Parse(mediaImpactedList["id_slogan"].ToString())>0) {						
						ds = MediaCreationDataAccess.GetIdsVehicle(webSession,long.Parse(mediaImpactedList["id_slogan"].ToString()));
						if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0 ){
							DataRow dr = ds.Tables[0].Rows[0];
							//if( !(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null 
							//    && (CstClassification.DB.Vehicles.names)int.Parse(dr["id_vehicle"].ToString())==CstClassification.DB.Vehicles.names.outdoor )
							//    ){
								if(dr["id_vehicle"] != null && dr["id_vehicle"].ToString().Length>0 && int.Parse(dr["id_vehicle"].ToString()) == CstClassification.DB.Vehicles.names.internet.GetHashCode()){
									//Remplacer média Internet par Adnettrack
									vehicleList.Add(CstClassification.DB.Vehicles.names.adnettrack.GetHashCode());
								}
								else vehicleList.Add(dr["id_vehicle"].ToString());
							//}
							return vehicleList;
						}						
					}
					else if(long.Parse(mediaImpactedList["id_slogan"].ToString())==0){						
						for(int i=0;i<idSelectedVehicle.Length;i++){
							if(WebFunctions.Modules.IsSponsorShipTVModule(webSession))
								tableName =  DbTables.DATA_SPONSORSHIP;
							else{
								if(idSelectedVehicle[i] != null && idSelectedVehicle[i].ToString().Length>0 && int.Parse(idSelectedVehicle[i].ToString()) == CstClassification.DB.Vehicles.names.internet.GetHashCode()){
									//Remplacer média Internet par Adnettrack
									idSelectedVehicle[i] =  CstClassification.DB.Vehicles.names.adnettrack.GetHashCode().ToString();
								}								
								tableName =  WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult((CstClassification.DB.Vehicles.names)int.Parse(idSelectedVehicle[i].ToString()),currentModuleDescription.ModuleType);
							}
							if(tableName.Length>0) {								
								ds = MediaCreationDataAccess.GetIdsVehicle(webSession,0,tableName,dateBegin,dateEnd,idSelectedVehicle[i].ToString());
								if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0 && (Int64.Parse(ds.Tables[0].Rows[0][0].ToString())>0)){																	
									//if( !(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null 
									//    && (CstClassification.DB.Vehicles.names)int.Parse(idSelectedVehicle[i].ToString())==CstClassification.DB.Vehicles.names.outdoor )
									//    ){
										 vehicleList.Add(idSelectedVehicle[i].ToString());
									//}
								}
							}
						}
						return vehicleList;
					}
					return vehicleList;
				}
											
				//Autres cas
				if(idSelectedVehicle!=null && idSelectedVehicle.Length>0){
					for(int i=0;i<idSelectedVehicle.Length;i++){
						if(WebFunctions.Modules.IsSponsorShipTVModule(webSession))
							tableName =  DbTables.DATA_SPONSORSHIP;
						else{
							if(idSelectedVehicle[i] != null && idSelectedVehicle[i].ToString().Length>0 && int.Parse(idSelectedVehicle[i].ToString()) == CstClassification.DB.Vehicles.names.internet.GetHashCode()){
								//Remplacer média Internet par Adnettrack
								idSelectedVehicle[i] =  CstClassification.DB.Vehicles.names.adnettrack.GetHashCode().ToString();
							}							
							tableName =  WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult((CstClassification.DB.Vehicles.names)int.Parse(idSelectedVehicle[i].ToString()),currentModuleDescription.ModuleType);
						}
						if(tableName.Length>0) {							 
							ds = MediaCreationDataAccess.GetIdsVehicle(webSession,mediaImpactedList,tableName,dateBegin,dateEnd,idSelectedVehicle[i].ToString());
							if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0 && Int64.Parse(ds.Tables[0].Rows[0][0].ToString())>0){								
								//if( !(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG]==null 
								//    && (CstClassification.DB.Vehicles.names)int.Parse(idSelectedVehicle[i].ToString())==CstClassification.DB.Vehicles.names.outdoor )
								//    ){
									 vehicleList.Add(idSelectedVehicle[i].ToString());
								//}
							}
						}

					}
					return vehicleList;
				}
				
			}
			#endregion

			return vehicleList;
		}

		
		#endregion

		#region Liste des médias (vehicle) impactés
		/// <summary>
		/// Obtient la liste des médias (vehicle) impactés
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idMediaLevel1">Média Niveau 1</param>
		/// <param name="idMediaLevel2">Média Niveau 2</param>
		/// <param name="idMediaLevel3">Média Niveau 3</param>
		/// <param name="idMediaLevel4">Média Niveau 4</param>
		/// <param name="vehicleArr">Liste des médias impactés</param>
		/// <param name="idVehicle">Média en cours</param>
		/// <param name="zoomDate">Zoom Date</param>
        public static void GetImpactedVehicleIds(WebSession webSession, string idMediaLevel1, string idMediaLevel2, string idMediaLevel3, string idMediaLevel4, ref ArrayList vehicleArr, ref string idVehicle, string zoomDate)
        {

            #region variables
            int dateBegin = 0;
            int dateEnd = 0;
            ListDictionary mediaImpactedList = null;
            #endregion

            #region Mise en forme des dates et du media
            CstWeb.CustomerSessions.Period.Type periodType = webSession.PeriodType;
            string periodBegin = webSession.PeriodBeginningDate;
            string periodEnd = webSession.PeriodEndDate;

            if (zoomDate != null && zoomDate.Length > 0)
            {

                if (webSession.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.weekly){
                        periodType = CstWeb.CustomerSessions.Period.Type.dateToDateWeek;
                }
                else{
                        periodType = CstWeb.CustomerSessions.Period.Type.dateToDateMonth;
                }

                dateBegin = Convert.ToInt32(
                    WebFunctions.Dates.Max(WebFunctions.Dates.getPeriodBeginningDate(zoomDate, periodType),
                        WebFunctions.Dates.getPeriodBeginningDate(periodBegin, webSession.PeriodType)).ToString("yyyyMMdd")
                    );

                dateEnd = Convert.ToInt32(
                    WebFunctions.Dates.Min(WebFunctions.Dates.getPeriodEndDate(zoomDate, periodType),
                        WebFunctions.Dates.getPeriodEndDate(periodEnd, webSession.PeriodType)).ToString("yyyyMMdd")
                    );

            }
            else
            {

                dateBegin = Convert.ToInt32(WebFunctions.Dates.getPeriodBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                dateEnd = Convert.ToInt32(WebFunctions.Dates.getPeriodEndDate(periodEnd, periodType).ToString("yyyyMMdd"));

            }
            #endregion

            try
            {
                //Obtention média impactés				
                mediaImpactedList = Functions.MediaDetailLevel.GetImpactedMedia(webSession, long.Parse(idMediaLevel1), long.Parse(idMediaLevel2), long.Parse(idMediaLevel3), long.Parse(idMediaLevel4));
            }
            catch (System.Exception ex)
            {
                throw (new MediaInsertionsCreationsRulesException("Impossible d'obtenir les médias impactés par le détail média.", ex));
            }

            #region Identification de(s) média(s) (Vehicle)
            if (!webSession.isCompetitorAdvertiserSelected())
            {
                vehicleArr = MediaInsertionsCreationsRules.GetIdsVehicle(webSession, mediaImpactedList, dateBegin.ToString(), dateEnd.ToString());

                if (vehicleArr != null && vehicleArr.Count > 0 && webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG] == null
                    && vehicleArr.Contains((DBClassificationConstantes.Vehicles.names.outdoor.GetHashCode()).ToString()))
                {
                    vehicleArr.Remove((DBClassificationConstantes.Vehicles.names.outdoor.GetHashCode()).ToString());
                }
                if (idVehicle == null || idVehicle.Length == 0 || long.Parse(idVehicle) == -1)
                {

                    if (vehicleArr != null && vehicleArr.Count > 0)
                    {
                        idVehicle = vehicleArr[0].ToString();
                    }
                }
            }
            else idVehicle = idMediaLevel1;
            #endregion
        }
		#endregion

		#region Règles métiers pour chaque colonne
		/// <summary>
		/// Applique les règles métiers spécifiques à une colonne
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="columnName">Non de la colonne</param>
		/// <param name="cellValue">Valeur de la cellule</param>
		/// <returns>Valeur traitée</returns>
		private static object ApplyColumnRule(WebSession webSession,string columnName,object cellValue){
			
			if(cellValue!=null && cellValue!=System.DBNull.Value){
				switch(columnName){
						//Traitement date
					case DATE_MEDIA_NUM :
						return (new DateTime(
							int.Parse(cellValue.ToString().Substring(0,4)),
							int.Parse(cellValue.ToString().Substring(4,2)),
							int.Parse(cellValue.ToString().Substring(6,2)))).ToString("dd/MM/yyyy");

					//TRAITEMENT JOUR DE LA SEMAINE
					case DAY_OF_WEEK :
						DateTime dat = 
						new DateTime(
							int.Parse(cellValue.ToString().Substring(0,4)),
							int.Parse(cellValue.ToString().Substring(4,2)),
							int.Parse(cellValue.ToString().Substring(6,2)));
						if(dat.DayOfWeek == System.DayOfWeek.Wednesday)return WebFunctions.Dates.getDay(webSession,"Wednesdays"); // s en trop
						return WebFunctions.Dates.getDay(webSession,dat.DayOfWeek.ToString());


					case TOP_DIFFUSION://Top diffusion télé
					case ID_TOP_DIFFUSION ://Top diffusion radio
						
						TimeSpan tSpan =(new TimeSpan(
							cellValue.ToString().Length>=5?int.Parse(cellValue.ToString().Substring(0,cellValue.ToString().Length-4)):0,
							cellValue.ToString().Length>=3?int.Parse(cellValue.ToString().Substring(System.Math.Max(cellValue.ToString().Length-4,0),System.Math.Min(cellValue.ToString().Length-2,2))):0,
							int.Parse(cellValue.ToString().Substring(System.Math.Max(cellValue.ToString().Length-2,0),System.Math.Min(cellValue.ToString().Length,2))))
							);
						return ((tSpan.Hours.ToString().Length > 1) ? tSpan.Hours.ToString() : "0" + tSpan.Hours.ToString())
						+ ":" + ((tSpan.Minutes.ToString().Length > 1) ? tSpan.Minutes.ToString() : "0" + tSpan.Minutes.ToString())
						+ ":" + ((tSpan.Seconds.ToString().Length > 1) ? tSpan.Seconds.ToString() : "0" + tSpan.Seconds.ToString());
								
						//Accroche
					case ID_SLOGAN :
					case SLOGAN :
						if(WebFunctions.MediaDetailLevel.HasSloganRight(webSession))return cellValue.ToString();
						else return null;					
						
					default : 
						return cellValue.ToString();
				}
				
			
			}

			return cellValue;
		}

		#endregion

		#region Liste des libellés de colonnes
		/// <summary>
		/// Obtient les champs en base de données des éléments à détailler
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dt">Table de donnée</param>
		/// <param name="fieldsList">Liste des champs à traiter</param>
		/// <returns>champs en base de données des éléments à détailler</returns>
		public static void GetFieldsList(WebSession webSession,DataTable dt,ArrayList fieldsList,CstClassification.DB.Vehicles.names idVehicle){
			
			if(webSession!=null){
				//Libellés des champs en base de données des éléments à regrouper
				if( webSession.DetailLevel!=null &&  webSession.DetailLevel.GetNbLevels>0){					
					foreach(DetailLevelItemInformation detailLevelItemInformation in webSession.DetailLevel.Levels){				
						
						//Identifiant en base de données
						if(detailLevelItemInformation.DataBaseAliasIdField!=null && detailLevelItemInformation.DataBaseAliasIdField.Length>0){
							if(dt.Columns.Contains(detailLevelItemInformation.DataBaseAliasIdField.ToUpper())  && !fieldsList.Contains(detailLevelItemInformation.DataBaseAliasIdField.ToUpper()))
							fieldsList.Add(detailLevelItemInformation.DataBaseAliasIdField.ToUpper());
						}else if(detailLevelItemInformation.DataBaseIdField!=null && detailLevelItemInformation.DataBaseIdField.Length>0 && !fieldsList.Contains(detailLevelItemInformation.DataBaseIdField.ToUpper())){
							if(dt.Columns.Contains(detailLevelItemInformation.DataBaseIdField.ToUpper()))
								fieldsList.Add(detailLevelItemInformation.DataBaseIdField.ToUpper());
						}
						//libellé en base de données
						if(detailLevelItemInformation.DataBaseAliasField!=null && detailLevelItemInformation.DataBaseAliasField.Length>0 && !fieldsList.Contains(detailLevelItemInformation.DataBaseAliasField.ToUpper())){
							if(dt.Columns.Contains(detailLevelItemInformation.DataBaseAliasField.ToUpper()))
								fieldsList.Add(detailLevelItemInformation.DataBaseAliasField.ToUpper());
						}
						else if(detailLevelItemInformation.DataBaseField!=null && detailLevelItemInformation.DataBaseField.Length>0 && !fieldsList.Contains(detailLevelItemInformation.DataBaseField.ToUpper())){
							if(dt.Columns.Contains(detailLevelItemInformation.DataBaseField.ToUpper()))
							fieldsList.Add(detailLevelItemInformation.DataBaseField.ToUpper());
						}
					}
				}

				//libellés des champs en base de données des éléments en colonnes
				if( webSession.GenericInsertionColumns!=null &&  webSession.GenericInsertionColumns.GetNbColumns>0){					
					foreach(GenericColumnItemInformation genericColumnItemInformation in webSession.GenericInsertionColumns.Columns){				
						
						//Identifiant en base de données
						if(genericColumnItemInformation.DataBaseAliasIdField!=null && genericColumnItemInformation.DataBaseAliasIdField.Length>0){
							if(dt.Columns.Contains(genericColumnItemInformation.DataBaseAliasIdField.ToUpper()) && !fieldsList.Contains(genericColumnItemInformation.DataBaseAliasIdField.ToUpper()))
								fieldsList.Add(genericColumnItemInformation.DataBaseAliasIdField.ToUpper());
						}
						else if(genericColumnItemInformation.DataBaseIdField!=null && genericColumnItemInformation.DataBaseIdField.Length>0){
							if(dt.Columns.Contains(genericColumnItemInformation.DataBaseIdField.ToUpper()) && !fieldsList.Contains(genericColumnItemInformation.DataBaseIdField.ToUpper()))
							fieldsList.Add(genericColumnItemInformation.DataBaseIdField.ToUpper());
						}
						//libellé en base de données
						if(genericColumnItemInformation.DataBaseAliasField!=null && genericColumnItemInformation.DataBaseAliasField.Length>0 &&  !fieldsList.Contains(genericColumnItemInformation.DataBaseAliasField.ToUpper())){
							if(dt.Columns.Contains(genericColumnItemInformation.DataBaseAliasField.ToUpper()))
								fieldsList.Add(genericColumnItemInformation.DataBaseAliasField.ToUpper());
						}else if(genericColumnItemInformation.DataBaseField!=null && genericColumnItemInformation.DataBaseField.Length>0 &&  !fieldsList.Contains(genericColumnItemInformation.DataBaseField.ToUpper())){
							if(dt.Columns.Contains(genericColumnItemInformation.DataBaseField.ToUpper()))
							fieldsList.Add(genericColumnItemInformation.DataBaseField.ToUpper());
						}
					}
				}
			}

			//Ajoute l'identifiant (uniquement pour la radio) de la version qui sera nécessaire pour construire le chemin du fichier audio de la radio
			if (WebFunctions.MediaDetailLevel.HasSloganRight(webSession) && CstClassification.DB.Vehicles.names.radio == idVehicle
				&& fieldsList != null && !fieldsList.Contains(ID_SLOGAN)) 
				fieldsList.Add(ID_SLOGAN);

		
		}
		#endregion

		#region Indique si nécessite colonnne génériques
		/// <summary>
		/// Indique si nécessite colonnne génériques
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Vrai si nécessite colonnne génériques</returns>
		public static bool IsRequiredGenericColmuns(WebSession webSession){
			switch(webSession.CurrentModule) {
				case CstWeb.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
				case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
					return false;
				default : return true;
			}

		}
		#endregion

        #region Indique si le media Marketing Direct est seléctioné
        /// <summary>
        /// Indique si nécessite colonnne génériques
        /// </summary>
        /// <returns>Vrai si le media MD est seléctioné</returns>
        public static bool IsMDSelected(string idVehicle) {
            if (int.Parse(idVehicle) == DBClassificationConstantes.Vehicles.names.directMarketing.GetHashCode())
                return true;
            return false;
        }
        #endregion

        #region Renvoie la liste des item content
        /// <summary>
        /// Renvoie la liste des item content
        /// </summary>
        /// <returns></returns>
        public static string GetItemContentList(DataRow row) {

            string mailContent = "";
            int k = 0;
            
            for (k = 1; k <= 10; k++) {
                if (row["item" + k].ToString() != "")
                    mailContent += row["item" + k].ToString() + ",";
            }

            if (mailContent != "")
                mailContent = mailContent.Substring(0, mailContent.Length - 1);

            return mailContent;
        }
        #endregion

        #endregion

    }
}
