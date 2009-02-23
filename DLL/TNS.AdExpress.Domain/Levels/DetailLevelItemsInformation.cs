#region Informations
// Auteur: G. Facon
// Création: 27/03/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using System.Collections.Generic;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

namespace TNS.AdExpress.Domain.Level {


	///<summary>
	/// Liste des descriptions des niveaux de détail
	/// </summary>
	///  <property />
	public class DetailLevelItemsInformation {

		#region variables


		///<summary>
		/// Liste des descriptions des niveaux de détail
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		///  <label>_list</label>
		private static System.Collections.Hashtable _list=new Hashtable();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		static DetailLevelItemsInformation(){
		}
		#endregion

		#region Accesseurs
		
		#endregion

		#region Méthodes publiques
		/// <summary>
		/// Accès à la description d'un élément de niveau de détail par son identifiant
		/// </summary>
		public static DetailLevelItemInformation Get(int id){
			try{
				return((DetailLevelItemInformation)_list[id]);
			}
			catch(System.Exception err){
				throw(new ArgumentException("impossible to reteive a detail level item information with this Id",err));
			}
		}

		/// <summary>
		/// Accès à la description d'un élément de niveau de détail par son identifiant
		/// </summary>
		public static DetailLevelItemInformation Get(DetailLevelItemInformation.Levels id){
            return Get(id.GetHashCode());
		}

		/// <summary>
		/// Initialisation de la liste à partir du fichier XML
		/// </summary>
		/// <param name="source">Source de données</param>
		public static void Init(IDataSource source){
			_list=DetailLevelItemsInformationXL.Load(source);
		}

        /// <summary>
        /// Get the list of levels matching the preformated details
        /// </summary>
        /// <param name="levels">Levels included</param>
        /// <returns>List of levels</returns>
        public static List<DetailLevelItemInformation> Translate(CstFormat.PreformatedProductDetails detail){
            List<DetailLevelItemInformation> levels = new List<DetailLevelItemInformation>();
            switch (detail)
            {
                case CstFormat.PreformatedProductDetails.advertiser:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    break;
                case CstFormat.PreformatedProductDetails.advertiserBrand:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    break;
                case CstFormat.PreformatedProductDetails.advertiserBrandProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                   break;
                case CstFormat.PreformatedProductDetails.advertiserGroupBrand:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    break;
                case CstFormat.PreformatedProductDetails.advertiserGroupBrandProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.advertiserGroupProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.advertiserGroupSegmentBrandProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.segment));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.advertiserGroupSegmentProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.segment));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.advertiserProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.agencyAdvertiser:
                    levels.Add(Get(DetailLevelItemInformation.Levels.agency));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    break;
                case CstFormat.PreformatedProductDetails.agencyProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.agency));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.brand:
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    break;
                case CstFormat.PreformatedProductDetails.group:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    break;
                case CstFormat.PreformatedProductDetails.group_agencyAgency:
                    levels.Add(Get(DetailLevelItemInformation.Levels.groupMediaAgency));
                    levels.Add(Get(DetailLevelItemInformation.Levels.agency));
                    break;
                case CstFormat.PreformatedProductDetails.group_agencyAgencyAdvertiser:
                    levels.Add(Get(DetailLevelItemInformation.Levels.groupMediaAgency));
                    levels.Add(Get(DetailLevelItemInformation.Levels.agency));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    break;
                case CstFormat.PreformatedProductDetails.group_agencyAgencyProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.groupMediaAgency));
                    levels.Add(Get(DetailLevelItemInformation.Levels.agency));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.groupAdvertiser:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    break;
                case CstFormat.PreformatedProductDetails.groupAdvertiserBrand:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    break;
                case CstFormat.PreformatedProductDetails.groupAdvertiserProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.groupAvertiserBrandProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.groupBrand:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    break;
                case CstFormat.PreformatedProductDetails.groupBrandProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.groupProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.groupSegment:
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    levels.Add(Get(DetailLevelItemInformation.Levels.segment));
                    break;
                case CstFormat.PreformatedProductDetails.holdingCompany:
                    levels.Add(Get(DetailLevelItemInformation.Levels.holdingCompany));
                    break;
                case CstFormat.PreformatedProductDetails.holdingCompanyAdvertiser:
                    levels.Add(Get(DetailLevelItemInformation.Levels.holdingCompany));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    break;
                case CstFormat.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
                    levels.Add(Get(DetailLevelItemInformation.Levels.holdingCompany));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    break;
                case CstFormat.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.holdingCompany));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.product:
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.sector:
                    levels.Add(Get(DetailLevelItemInformation.Levels.sector));
                    break;
                case CstFormat.PreformatedProductDetails.sectorAdvertiser:
                    levels.Add(Get(DetailLevelItemInformation.Levels.sector));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    break;
                case CstFormat.PreformatedProductDetails.sectorAdvertiserProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.sector));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
                    levels.Add(Get(DetailLevelItemInformation.Levels.sector));
                    levels.Add(Get(DetailLevelItemInformation.Levels.holdingCompany));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    break;
                case CstFormat.PreformatedProductDetails.sectorProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.sector));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.sectorSubsector:
                    levels.Add(Get(DetailLevelItemInformation.Levels.sector));
                    levels.Add(Get(DetailLevelItemInformation.Levels.subSector));
                    break;
                case CstFormat.PreformatedProductDetails.sectorSubsectorGroup:
                    levels.Add(Get(DetailLevelItemInformation.Levels.sector));
                    levels.Add(Get(DetailLevelItemInformation.Levels.subSector));
                    levels.Add(Get(DetailLevelItemInformation.Levels.group));
                    break;
                case CstFormat.PreformatedProductDetails.segmentAdvertiser:
                    levels.Add(Get(DetailLevelItemInformation.Levels.segment));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    break;
                case CstFormat.PreformatedProductDetails.segmentAdvertiserBrand:
                    levels.Add(Get(DetailLevelItemInformation.Levels.segment));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    break;
                case CstFormat.PreformatedProductDetails.segmentAdvertiserProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.segment));
                    levels.Add(Get(DetailLevelItemInformation.Levels.advertiser));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                case CstFormat.PreformatedProductDetails.segmentBrand:
                    levels.Add(Get(DetailLevelItemInformation.Levels.segment));
                    levels.Add(Get(DetailLevelItemInformation.Levels.brand));
                    break;
                case CstFormat.PreformatedProductDetails.segmentProduct:
                    levels.Add(Get(DetailLevelItemInformation.Levels.segment));
                    levels.Add(Get(DetailLevelItemInformation.Levels.product));
                    break;
                default:
                    throw (new ArgumentException("Performated detail not supported"));
                    break;
            }
            return levels;
        }
        /// <summary>
        /// Get the list of levels matching the preformated details
        /// </summary>
        /// <param name="levels">Levels included</param>
        /// <returns>List of levels</returns>
        public static List<DetailLevelItemInformation> Translate(CstFormat.PreformatedMediaDetails detail){
            List<DetailLevelItemInformation> levels = new List<DetailLevelItemInformation>();
            switch (detail)
            {
                case CstFormat.PreformatedMediaDetails.vehicle:
                    levels.Add(Get(DetailLevelItemInformation.Levels.vehicle));
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategory:
                    levels.Add(Get(DetailLevelItemInformation.Levels.vehicle));
                    levels.Add(Get(DetailLevelItemInformation.Levels.category));
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
                    levels.Add(Get(DetailLevelItemInformation.Levels.vehicle));
                    levels.Add(Get(DetailLevelItemInformation.Levels.category));
                    levels.Add(Get(DetailLevelItemInformation.Levels.media));
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleMedia:
                    levels.Add(Get(DetailLevelItemInformation.Levels.vehicle));
                    levels.Add(Get(DetailLevelItemInformation.Levels.media));
                    break;
                default:
                    throw (new ArgumentException("Performated detail not supported"));
                    break;
            }
            return levels;
        }
		#endregion

	}
}
