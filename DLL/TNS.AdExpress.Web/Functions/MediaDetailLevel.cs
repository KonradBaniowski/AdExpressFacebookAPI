#region Informations
// Auteur:D. V. Mussuma
// Création: 12/12/2005
// Modification:
//	12/01/2006 B. Masson Ajout des niveaux de détail supports
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using ConstantesFrwkResults=TNS.AdExpress.Constantes.FrameWork.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using TNS.FrameWork;
using ConstantesCustomer=TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Web.Navigation;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Fonctions des niveaux de détail média
	/// </summary>
	public class MediaDetailLevel{

		/// <summary>
		/// Obtient le texte du niveau de détail support
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Texte du niveau de détail support</returns>
		public static string LevelMediaToHtmlString(WebSession webSession){
			try{
				switch(webSession.PreformatedMediaDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
						return(GestionWeb.GetWebWord(1292,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
						return(GestionWeb.GetWebWord(1142,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
						return(GestionWeb.GetWebWord(1143,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
						return(GestionWeb.GetWebWord(1544,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
						return(GestionWeb.GetWebWord(1542,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
						return(GestionWeb.GetWebWord(1543,webSession.SiteLanguage));

					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
						return(GestionWeb.GetWebWord(1860,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
						return(GestionWeb.GetWebWord(1861,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
						return(GestionWeb.GetWebWord(1862,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
						return(GestionWeb.GetWebWord(1863,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
						return(GestionWeb.GetWebWord(1864,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
						return(GestionWeb.GetWebWord(1865,webSession.SiteLanguage));

					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
						return(GestionWeb.GetWebWord(1866,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
						return(GestionWeb.GetWebWord(1867,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
						return(GestionWeb.GetWebWord(1868,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
						return(GestionWeb.GetWebWord(1869,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
						return(GestionWeb.GetWebWord(1870,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
						return(GestionWeb.GetWebWord(1871,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
						return(GestionWeb.GetWebWord(1872,webSession.SiteLanguage));

					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
						return(GestionWeb.GetWebWord(1873,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
						return(GestionWeb.GetWebWord(1874,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
						return(GestionWeb.GetWebWord(1875,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
						return(GestionWeb.GetWebWord(1876,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
						return(GestionWeb.GetWebWord(1877,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
						return(GestionWeb.GetWebWord(1878,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
						return(GestionWeb.GetWebWord(1879,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
						return(GestionWeb.GetWebWord(1880,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.Media:
						return(GestionWeb.GetWebWord(971,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.programTypeProgram:
						return(GestionWeb.GetWebWord(2070,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sponsorshipForm:
						return(GestionWeb.GetWebWord(2052,webSession.SiteLanguage));
					default:
						return("no value");
				}
			}
			catch(Exception){
				return("no value");
			}
		}

		/// <summary>
		/// Obtient le texte du niveau de détail support
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Texte du niveau de détail support</returns>
		public static string LevelMediaToExcelString(WebSession webSession){
			return(Convertion.ToHtmlString(LevelMediaToHtmlString(webSession)));
		}

		/// <summary>
		/// Vérifie si le niveau 2 de détail média concerne les centres d'intérêts 
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>vrai si centres d'intérêts </returns>
		public static bool IsLevel2InterestCenter(WebSession webSession){
			switch(webSession.PreformatedMediaDetail){			
				case  WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter :
				case  WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia :
					return true;
				default :
					return false;
			}
		}

		/// <summary>
		/// Obtient la liste des média impacté par le détail média.
		/// </summary>
		/// <param name="webSession">Session du client</param>	
		/// <param name="idMediaLevel1">Identifiant du media niveau 1</param>
		/// <param name="idMediaLevel2">Identifiant du media niveau 2</param>
		/// <param name="idMediaLevel3">Identifiant du media niveau 3</param>
		/// <param name="idMediaLevel4">Identifiant du media niveau 4</param>
		/// <returns>liste des média </returns>
		public static ListDictionary  GetImpactedMedia(WebSession webSession,Int64 idMediaLevel1,Int64 idMediaLevel2,Int64 idMediaLevel3,Int64 idMediaLevel4){
            string temp = "";
            ListDictionary mediaCol = null;
            //			object[,] tempMediaList = null;
            int indexLevel = 0;
            if (webSession.PreformatedMediaDetail.ToString().Length > 0) {
                temp = webSession.PreformatedMediaDetail.ToString().ToUpper();
                mediaCol = new ListDictionary();

                if (webSession.GenericMediaDetailLevel != null && webSession.GenericMediaDetailLevel.Levels != null && webSession.GenericMediaDetailLevel.Levels.Count > 0) {
                    for (int i = 0; i < webSession.GenericMediaDetailLevel.Levels.Count; i++) {
                        if (webSession.GenericMediaDetailLevel.Levels[i] != null
                            && ((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField != null
                            && ((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField.Length > 0
                            ) {
                            indexLevel++;
                            switch (indexLevel) {
                                case 1:
                                    mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, idMediaLevel1); //identifiant du média est la clé
                                    break;
                                case 2:
                                    mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, idMediaLevel2);
                                    break;
                                case 3:
                                    mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, idMediaLevel3);
                                    break;
                                case 4:
                                    mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, idMediaLevel4);
                                    break;
                                default: mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, -1);
                                    break;
                            }
                        }
                    }
                }

            }
            return mediaCol;
        }

		/// <summary>
		/// Obtient la liste des filtres dans les niveaux de détails génériques.
        /// Similaire à GetImpactedMedia mais utilise le collections génériques typées
		/// </summary>
		/// <param name="webSession">Session du client</param>	
		/// <param name="idLevel1">Identifiant du niveau 1</param>
		/// <param name="idLevel2">Identifiant du niveau 2</param>
		/// <param name="idLevel3">Identifiant du niveau 3</param>
		/// <param name="idLevel4">Identifiant du niveau 4</param>
		/// <returns>liste des niveaux de détails uimpliqués dans les filtres </returns>
        public static Dictionary<DetailLevelItemInformation, long> GetFilters(WebSession webSession, Int64 idLevel1, Int64 idLevel2, Int64 idLevel3, Int64 idLevel4) {
            string temp = "";
            Dictionary<DetailLevelItemInformation, long> mediaCol = null;
            //			object[,] tempMediaList = null;
            int indexLevel = 0;
            if (webSession.PreformatedMediaDetail.ToString().Length > 0) {
                temp = webSession.PreformatedMediaDetail.ToString().ToUpper();
                mediaCol = new Dictionary<DetailLevelItemInformation, long>();

                if (webSession.GenericMediaDetailLevel != null && webSession.GenericMediaDetailLevel.Levels != null && webSession.GenericMediaDetailLevel.Levels.Count > 0) {
                    for (int i = 0; i < webSession.GenericMediaDetailLevel.Levels.Count; i++) {
                        if (webSession.GenericMediaDetailLevel.Levels[i] != null
                            && ((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField != null
                            && ((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField.Length > 0
                            ) {
                            indexLevel++;
                            switch (indexLevel) {
                                case 1:
                                    mediaCol.Add((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i], idLevel1); //identifiant du média est la clé
                                    break;
                                case 2:
                                    mediaCol.Add((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i], idLevel2);
                                    break;
                                case 3:
                                    mediaCol.Add((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i], idLevel3);
                                    break;
                                case 4:
                                    mediaCol.Add((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i], idLevel4);
                                    break;
                                default: mediaCol.Add((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i], -1);
                                    break;
                            }
                        }
                    }
                }

            }
            return mediaCol;

        }
		
		/// <summary>
		/// Obtient la liste de niveaux de détail média
		/// </summary>
		/// <returns>liste de niveaux de détail média</returns>
		public static object[,] GetMediaList(){
			object[,] tab = new object[6,2];
		
			tab[0,0]=ConstantesFrwkResults.CommonMother.VEHICLE_LABEL;
			tab[0,1]=DBConstantes.Fields.ID_VEHICLE;

			tab[1,0]=ConstantesFrwkResults.CommonMother.CATEGORY_LABEL;
			tab[1,1]=DBConstantes.Fields.ID_CATEGORY;

			tab[2,0]=ConstantesFrwkResults.CommonMother.INTERESTCENTER_LABEL;
			tab[2,1]=DBConstantes.Fields.ID_INTEREST_CENTER;

			tab[3,0]=ConstantesFrwkResults.CommonMother.MEDIASELLER_LABEL;
			tab[3,1]=DBConstantes.Fields.ID_MEDIA_SELLER;

			tab[4,0]=ConstantesFrwkResults.CommonMother.MEDIA_LABEL;
			tab[4,1]=DBConstantes.Fields.ID_MEDIA;

			tab[5,0]= ConstantesFrwkResults.CommonMother.SLOGAN_LABEL;
			tab[5,1]=DBConstantes.Fields.ID_SLOGAN;
		
			return tab;
		}

		///// <summary>
		///// Vérifie si le client aux accroches en fonction du niveau de détail produit et média.
		///// </summary>
		///// <remarks>Les niveaux de détail par accroche sont disponibles uniquement dans 
		///// un plan media défini par marque ou par produit
		///// </remarks>
		///// <param name="webSession">Session du client</param>
		///// <returns>Vrai si le client à accès aux accroches</returns>
		//public static bool HasSloganRight(WebSession webSession){
		//    if(webSession!=null && webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)!=null 
		//        )return true;
		//    else return false;
		//}

		#region Niveau de détail support générique
		/// <summary>
		/// Niveau de détail support ou média(Niveaux détaillés par  :)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="displayGenericlevelDetailLabel">Indique si affiche niveau de détail générique</param>
		/// <param name="genericlevelDetailLabel">Libellé niveaux de détail</param>
		/// <param name="excel">Indique si sortie excel</param>
		/// <returns>HTML</returns>
		public static void GetGenericLevelDetail(WebSession webSession, ref bool displayGenericlevelDetailLabel,System.Web.UI.WebControls.Label genericlevelDetailLabel,bool excel){
			ArrayList detailSelections = null;

			//webSession.CustomerLogin.ModuleList();
			Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
			try{
				detailSelections = ((ResultPageInformation) currentModule.GetResultPageInformation((int)webSession.CurrentTab)).DetailSelectionItemsType;
			}
			catch(System.Exception){
				if(currentModule.Id==WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
					detailSelections = ((ResultPageInformation) currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
			}
			foreach(int currentType in detailSelections){
				switch((WebConstantes.DetailSelection.Type)currentType){
					case WebConstantes.DetailSelection.Type.genericMediaLevelDetail :
						genericlevelDetailLabel.Text  = (excel)? Convertion.ToHtmlString(webSession.GenericMediaDetailLevel.GetLabel(webSession.SiteLanguage)): webSession.GenericMediaDetailLevel.GetLabel(webSession.SiteLanguage);
						if(genericlevelDetailLabel.Text!=null && genericlevelDetailLabel.Text.Length>0)
							displayGenericlevelDetailLabel = true;
						break;
					case WebConstantes.DetailSelection.Type.genericProductLevelDetail :
						genericlevelDetailLabel.Text = (excel)? Convertion.ToHtmlString(webSession.GenericProductDetailLevel.GetLabel(webSession.SiteLanguage)) : webSession.GenericProductDetailLevel.GetLabel(webSession.SiteLanguage);
						if(genericlevelDetailLabel.Text !=null && genericlevelDetailLabel.Text.Length>0)
							displayGenericlevelDetailLabel = true;
						break;
					default:
						break;
				}
			}
		}
		#endregion

        #region Niveau de détail colonne générique
        /// <summary>
        /// Niveau de détail colonne générique
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="displayGenericlevelDetailColumnLabel">Indique si affiche niveau de détail générique</param>
        /// <param name="genericlevelDetailColumnLabel">Libellé niveaux de détail</param>
        /// <param name="excel">Indique si sortie excel</param>
        /// <returns>HTML</returns>
        public static void GetGenericLevelDetailColumn(WebSession webSession, ref bool displayGenericlevelDetailColumnLabel, System.Web.UI.WebControls.Label genericlevelDetailColumnLabel, bool excel) {
            ArrayList detailSelections = null;

            //webSession.CustomerLogin.ModuleList();
            Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
            try {
                detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation((int)webSession.CurrentTab)).DetailSelectionItemsType;
            }
            catch (System.Exception) {
                if (currentModule.Id == WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
                    detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
            }
            foreach (int currentType in detailSelections) {
                switch ((WebConstantes.DetailSelection.Type)currentType) {
                    case WebConstantes.DetailSelection.Type.genericColumnLevelDetail:
                        genericlevelDetailColumnLabel.Text = (excel) ? Convertion.ToHtmlString(webSession.GenericColumnDetailLevel.GetLabel(webSession.SiteLanguage)) : webSession.GenericColumnDetailLevel.GetLabel(webSession.SiteLanguage);
                        if (genericlevelDetailColumnLabel.Text != null && genericlevelDetailColumnLabel.Text.Length > 0)
                            displayGenericlevelDetailColumnLabel = true;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
		

	}
}
