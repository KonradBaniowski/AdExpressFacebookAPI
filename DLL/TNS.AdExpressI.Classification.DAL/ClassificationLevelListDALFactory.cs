#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpressI.Classification.DAL.Exceptions;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpressI.Classification.DAL {
	/// <summary>
	/// Provides all methods to create items list of classification level.
	/// </summary>
	/// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationDALException">
	/// Unknow Detail level information Identifier 
	/// </exception>
	public abstract class ClassificationLevelListDALFactory {

		#region Variables
		/// <summary>
		///Table name
		/// </summary>
		protected TNS.AdExpress.Constantes.Classification.DB.Table.name _table;
		/// <summary>
		/// Database schema
		/// </summary>
		protected string _dbSchema = "";
		/// <summary>
		/// Classification items list
		/// </summary>
		protected Dictionary<long, string> _list;
		/// <summary>
		/// Identifiers' list sorted by labels
		/// </summary>
		protected List<long> _idListOrderByClassificationItem = new List<long>();
		/// <summary>
		/// Data Table
		/// </summary>
		protected DataTable _dataTable;
		/// <summary>
		/// Items' Language
		/// </summary>
		protected int _language = TNS.AdExpress.Constantes.DB.Language.FRENCH;
		/// <summary>
		/// Data Source
		/// </summary>
		IDataSource _source = null;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Data source</param>
		/// <param name="language">languague Identifier</param>
		public ClassificationLevelListDALFactory( IDataSource source, int language) {
			if (source == null) throw (new NullReferenceException(" Parameter source is Null"));
			_source = source;
			_language = language;
		}
		#endregion

		#region IClassificationLevelListDALFactory Implementation

		/// Get partial items list of a classification's level
		/// </summary>
		/// <param name="detailLevelItemInformation">Detail level informations</param>
		/// <param name="idList">classification items' identifier list</param>
		public virtual ClassificationLevelListDAL CreateClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, string idList) {
		 
			switch (detailLevelItemInformation.Id) {
				
				//Create a list of advertiser items				
				case DetailLevelItemInformation.Levels.advertiser :
					return new ProductBrand.AdvertiserLevelListDAL(idList,_language, _source);

				//Create a list of brand level's items		
				case DetailLevelItemInformation.Levels.brand :
					return new ProductBrand.BrandLevelListDAL(idList, _language, _source);
				//Create a list of sub media level's items		
				case DetailLevelItemInformation.Levels.category :
					return new MediaBrand.CategoryLevelListDAL(idList, _language, _source);
				//Create a list of group level's items		
				case DetailLevelItemInformation.Levels.group:
					return new ProductBrand.GroupLevelListDAL(idList, _language, _source);
				//Create a list of Parent level's items		
				case DetailLevelItemInformation.Levels.holdingCompany :
					return new ProductBrand.HoldingCompanyLevelListDAL(idList, _language, _source);
				//Create a list of Media genre level's items		
				case DetailLevelItemInformation.Levels.interestCenter :
					return new MediaBrand.InterestCenterLevelListDAL(idList, _language, _source);
				//Create a list of Vehicle level's items		
				case DetailLevelItemInformation.Levels.media :
					return new MediaBrand.MediaLevelListDAL(idList, _language, _source);
				//Create a list of Media owner level's items		
				case DetailLevelItemInformation.Levels.mediaSeller :
					return new MediaBrand.MediaSellerLevelListDAL(idList, _language, _source);
				//Create a list of Category level's items		
				case DetailLevelItemInformation.Levels.sector :
					return new ProductBrand.SectorLevelListDAL(idList, _language, _source);
				//Create a list of Sub group's level items		
				case DetailLevelItemInformation.Levels.segment :
					return new ProductBrand.SegmentLevelListDAL(idList, _language, _source);
				//Create a list of Sub Category level's items		
				case DetailLevelItemInformation.Levels.subSector :
					return new ProductBrand.SubSectorLevelListDAL(idList, _language, _source);
				//Create a list of Title level's items		
				case DetailLevelItemInformation.Levels.title :
					return new MediaBrand.TitleLevelListDAL(idList, _language, _source);
				//Create a list of Media Type level 's  items		
				case DetailLevelItemInformation.Levels.vehicle :
					return new MediaBrand.VehicleLevelListDAL(idList, _language, _source);
				//Create a list of Product level's items		
				case DetailLevelItemInformation.Levels.product:
					return new ProductBrand.ProductLevelListDAL(idList, _language, _source);
				default :
					throw new Exceptions.ClassificationDALException(" Unknow Detail level information Identifier ");
			}
		}
		/// <summary>	
		/// Get all items list of a classification's level
		/// </summary>
		/// <param name="detailLevelItemInformation">Detail level informations</param>
		public virtual ClassificationLevelListDAL CreateClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation) {
			switch (detailLevelItemInformation.Id) {

				//Create a list of advertiser items				
				case DetailLevelItemInformation.Levels.advertiser:
					return new ProductBrand.AdvertiserLevelListDAL( _language, _source);
				//Create a list of brand level's items		
				case DetailLevelItemInformation.Levels.brand:
					return new ProductBrand.BrandLevelListDAL( _language, _source);
				//Create a list of sub media level's items		
				case DetailLevelItemInformation.Levels.category:
					return new MediaBrand.CategoryLevelListDAL( _language, _source);
				//Create a list of group level's items		
				case DetailLevelItemInformation.Levels.group:
					return new ProductBrand.GroupLevelListDAL( _language, _source);
				//Create a list of Parent level's items		
				case DetailLevelItemInformation.Levels.holdingCompany:
					return new ProductBrand.HoldingCompanyLevelListDAL( _language, _source);
				//Create a list of Media genre level's items		
				case DetailLevelItemInformation.Levels.interestCenter:
					return new MediaBrand.InterestCenterLevelListDAL( _language, _source);
				//Create a list of Vehicle level's items		
				case DetailLevelItemInformation.Levels.media:
					return new MediaBrand.MediaLevelListDAL( _language, _source);
				//Create a list of Media owner level's items		
				case DetailLevelItemInformation.Levels.mediaSeller:
					return new MediaBrand.MediaSellerLevelListDAL( _language, _source);
				//Create a list of Category level's items		
				case DetailLevelItemInformation.Levels.sector:
					return new ProductBrand.SectorLevelListDAL( _language, _source);
				//Create a list of Sub group's level items		
				case DetailLevelItemInformation.Levels.segment:
					return new ProductBrand.SegmentLevelListDAL( _language, _source);
				//Create a list of Sub Category level's items		
				case DetailLevelItemInformation.Levels.subSector:
					return new ProductBrand.SubSectorLevelListDAL( _language, _source);
				//Create a list of Title level's items		
				case DetailLevelItemInformation.Levels.title:
					return new MediaBrand.TitleLevelListDAL( _language, _source);
				//Create a list of Media Type level 's  items		
				case DetailLevelItemInformation.Levels.vehicle:
					return new MediaBrand.VehicleLevelListDAL( _language, _source);
				//Create a list of Product level's items		
				case DetailLevelItemInformation.Levels.product:
					return new ProductBrand.ProductLevelListDAL( _language, _source);
				default:
					throw new Exceptions.ClassificationDALException(" Unknow Detail level information Identifier ");
			}
		}
		/// Get partial items list of a classification's level
		/// </summary>
		/// <param name="table">Target table used to build the list</param>
		/// <param name="idList">classification items' identifier list</param>
		public virtual ClassificationLevelListDAL CreateDefaultClassificationLevelListDAL(string table, string idList) {
			return new ClassificationLevelListDAL(table, idList, _language, _source);
		}
		#endregion

	}
}
