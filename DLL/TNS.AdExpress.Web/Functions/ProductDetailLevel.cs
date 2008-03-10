#region Informations
// Auteur:
// Création:
// Modification:
//		G. Facon		12/08/2005	Nom des méthodes

#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using TNS.FrameWork;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Fonctions des niveaux de détail produit
	/// </summary>
	public class ProductDetailLevel{

		/// <summary>
		/// Obtient le niveau de détail produit par défaut en fonction du module
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Niveau de détail</returns>
		public static WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails GetDefault(WebSession webSession){
			switch(webSession.CurrentModule){
				case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
				case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
					return(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group);
				case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_CONCURENTIELLE:
				case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE:
				case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_POTENTIELS:
				case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
				case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
				case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES:
					return(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser);
				default:
					return(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup);
			}
		}

		/// <summary>
		/// Obtient le nombre de niveau
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Nombre de niveau</returns>
		/// <remarks>On se base sur le caractère séparateur '/' pour calculer le nombre de niveau</remarks>
		/// <example>Famille/Classe/Groupe => 3 niveaux</example>
		public static int GetLevelNumber(WebSession webSession){
			return((Functions.ProductDetailLevel.LevelProductToHtmlString(webSession)).Split('/').Length);
		}

		/// <summary>
		/// Obtient le texte du niveau de détail produit
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Texte du niveau de détail produi</returns>
		public static string LevelProductToHtmlString(WebSession webSession){
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
						return(GestionWeb.GetWebWord(1103,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
						return(GestionWeb.GetWebWord(1104,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
						return(GestionWeb.GetWebWord(1532,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
						return(GestionWeb.GetWebWord(1491,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
						return(GestionWeb.GetWebWord(1492,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
						return(GestionWeb.GetWebWord(1105,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
						return(GestionWeb.GetWebWord(858,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
						return(GestionWeb.GetWebWord(1106,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
						return(GestionWeb.GetWebWord(1107,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
						return(GestionWeb.GetWebWord(1108,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
						return(GestionWeb.GetWebWord(1109,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
						return(GestionWeb.GetWebWord(1110,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
						return(GestionWeb.GetWebWord(1111,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
						return(GestionWeb.GetWebWord(1112,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
						return(GestionWeb.GetWebWord(1113,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
						return(GestionWeb.GetWebWord(1114,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
						return(GestionWeb.GetWebWord(1642,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
						return(GestionWeb.GetWebWord(1643,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
						return(GestionWeb.GetWebWord(1115,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
						return(GestionWeb.GetWebWord(1116,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
						return(GestionWeb.GetWebWord(1589,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
						return(GestionWeb.GetWebWord(1590,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
						return(GestionWeb.GetWebWord(1591,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
						return(GestionWeb.GetWebWord(1592,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
						return(GestionWeb.GetWebWord(1577,webSession.SiteLanguage));									
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
						return(GestionWeb.GetWebWord(1578,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
						return(GestionWeb.GetWebWord(1579,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
						return(GestionWeb.GetWebWord(1603,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
						return(GestionWeb.GetWebWord(1602,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand:
						return(GestionWeb.GetWebWord(1149,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupSegment:
						return(GestionWeb.GetWebWord(1144,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
						return(GestionWeb.GetWebWord(1145,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
						return(GestionWeb.GetWebWord(1893,webSession.SiteLanguage));

					default:
						return("no value");
				}
			}
			catch(Exception){
				return("no value");
			}
		}

		/// <summary>
		/// Obtient le texte du niveau de détail produit
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Texte du niveau de détail produit</returns>
		public static string LevelProductToExcelString(WebSession webSession){
			return(Convertion.ToHtmlString(LevelProductToHtmlString(webSession)));
		}
		
		/// <summary>
		/// Droit d'acceder à la sélection des accroches en fonctions de la sélection produit.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Vrai si le client peut affiner son univers de versions</returns>
		public static bool CanCustomizeUniverseSlogan(WebSession webSession) {
			#region Ancienne version
			//if (
			//    webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG]!=null 
			//    &&
			//    // Sélection par produit ou marque ou annonceur
			//    (
			//    webSession.GetSelection(webSession.SelectionUniversAdvertiser,RightConstantes.type.productAccess).Length>0 ||
			//    webSession.GetSelection(webSession.SelectionUniversAdvertiser,RightConstantes.type.brandAccess).Length>0 ||
			//    webSession.GetSelection(webSession.SelectionUniversAdvertiser,RightConstantes.type.advertiserAccess).Length>0
			//    ) 
			//    &&
			//    // Pas de famille, classe, groupe,variété, groupe d'annonceur webSession.ProductDetailLevel.LevelProduct
			//    webSession.GetSelection(webSession.SelectionUniversAdvertiser,RightConstantes.type.sectorAccess).Length==0 &&
			//    webSession.GetSelection(webSession.SelectionUniversAdvertiser,RightConstantes.type.subSectorAccess).Length==0 &&
			//    webSession.GetSelection(webSession.SelectionUniversAdvertiser,RightConstantes.type.groupAccess).Length==0 &&
			//    webSession.GetSelection(webSession.SelectionUniversAdvertiser,RightConstantes.type.segmentAccess).Length==0 &&
			//    webSession.GetSelection(webSession.SelectionUniversAdvertiser,RightConstantes.type.holdingCompanyAccess).Length==0 				
			//    )
			//    return(true);
			#endregion


			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count>0 && 
				webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG) != null &&
				// Sélection par produit ou marque ou annonceur						
				(webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
				webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
				webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes)
				) &&
				// Pas de famille, classe, groupe,variété, groupe d'annonceur 
				!webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
				!webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes) &&
				!webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) &&
				!webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) &&
				!webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes)				
				)
				return (true);

			return (false);
		}

		#region afficher raccourci vers plan média
		/// <summary>
		/// Indique s'il faut afficher un lien vers l'alerte plan média
		/// pour la nomenclature de niveau 1
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>True si on peut afficher le lien, false sinon</returns>
		internal static bool ShowMediaPlanL1(WebSession webSession){
			switch(webSession.PreformatedProductDetail){
			//	case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
			//	case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
			//	case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
				
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:				
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:											
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:											
										
				
					return(false);								
				default:
					return(true);
			}
		}
		/// <summary>
		/// Indique s'il faut afficher un lien vers l'alerte plan média
		/// pour la nomenclature de niveau 2
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>True si on peut afficher le lien, false sinon</returns>
		internal static bool ShowMediaPlanL2(WebSession webSession){
			switch(webSession.PreformatedProductDetail){												
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:	
//				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
				//case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:																											
//				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
//				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
																					
					return(false);								
				default:
					return(true);
			}
		}

		/// <summary>
		/// Indique s'il faut afficher un lien vers l'alerte plan média
		/// pour la nomenclature de niveau 3
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>True si on peut afficher le lien, false sinon</returns>
		internal static bool ShowMediaPlanL3(WebSession webSession){
			return(true);
		}

		/// <summary>
		/// Savoir si l'on affiche la creation
		/// </summary>
		/// <param name="webSession">Session du client</param>
		///<param name="level">Niveau affichage</param>
		/// <returns>True si on peut affichier la création, false sinon</returns>
		internal static bool DisplayCreation(WebSession webSession,int level){
			bool displayCreation=false;
			//			string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			//			ClassificationCst.DB.Vehicles.names vehicleType = (ClassificationCst.DB.Vehicles.names)int.Parse(Vehicle);
			//			if(vehicleType==ClassificationCst.DB.Vehicles.names.outdoor) return false;
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
						displayCreation=false;
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
						displayCreation=false;
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:					
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
						displayCreation=true;
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=true;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=false;
						else{
							displayCreation=true;
						}
						
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
						displayCreation=true;
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=true;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL3_INDEX){
							displayCreation=true;
						}
						else{
							displayCreation=true;
						}
						break;

					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;
						else if(level==ResultConstantes.IDL3_INDEX){
							displayCreation=true;
						}
						else{
							displayCreation=true;
						}
						break;

					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
						displayCreation=false;
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
						displayCreation=false;
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL3_INDEX){
							displayCreation=true;
						}
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
						displayCreation=false;
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=false;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=false;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;	
						else if(level==ResultConstantes.IDL3_INDEX)
							displayCreation=false;	
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;	
						else if(level==ResultConstantes.IDL3_INDEX)
							displayCreation=false;	
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=true;						
						else{
							displayCreation=true;
						}
						break;
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
						if(level==ResultConstantes.IDL1_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL2_INDEX)
							displayCreation=false;
						else if(level==ResultConstantes.IDL3_INDEX){
							displayCreation=true;
						}
						else{
							displayCreation=true;
						}
						break;
					default:						
						break;
				}
			}
			catch(Exception){	
			}
			return displayCreation;
		}



		/// <summary>
		/// Savoir si l'on affiche la creation
		/// </summary>
		/// <param name="webSession">Session du client</param>		
		/// <returns>True si on doit affichier la création, false sinon</returns>
		internal static bool DisplayCreation(WebSession webSession){
			bool displayCreation=true;
			//			string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			//			ClassificationCst.DB.Vehicles.names vehicleType = (ClassificationCst.DB.Vehicles.names)int.Parse(Vehicle);
			//			if(vehicleType==ClassificationCst.DB.Vehicles.names.outdoor) return false;
			try{
				switch(webSession.PreformatedProductDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
							displayCreation=false;
							break;
					default:						
						break;
				}
			}
			catch(Exception){				
			}
			return displayCreation;
		}
		#endregion

	}
}
