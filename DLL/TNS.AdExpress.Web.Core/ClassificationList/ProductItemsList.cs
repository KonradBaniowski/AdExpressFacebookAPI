#region Informations
// Author: G. Facon
// Creation Date: 07/12/2004 
// Modification Date: 
#endregion

using System;

using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebException=TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Core.ClassificationList{
	/// <summary>
	/// Product Items list used to determine an AdExpress universe
	/// </summary>
	public class ProductItemsList{
		
		#region Variables
		/// <summary>
		/// List of sectors
		/// </summary>
		private string _sectorItemsList="";
		/// <summary>
		/// List of subsectors
		/// </summary>
		private string _subSectorItemsList="";
		/// <summary>
		/// List of groups
		/// </summary>
		private string _groupItemsList="";
		/// <summary>
		/// List of segments
		/// </summary>
		private string _segmentItemsList="";
		/// <summary>
		/// List ofproducts
		/// </summary>
		private string _productItemsList="";
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="idProductItemsList">List Id</param>
		public ProductItemsList(int idProductItemsList){
			switch(idProductItemsList){
				case WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID:
					_productItemsList="180000,50000,50001";
					break;
				case WebConstantes.AdExpressUniverse.AUTOPROMO_RADIO_PRODUCT_LIST_ID:
					_productItemsList="180000";
					break;
				case WebConstantes.AdExpressUniverse.DASHBOARD_PRESS_EXCLUDE_PRODUCT_LIST_ID :
					_sectorItemsList="99";
					_groupItemsList="549,563";
					_segmentItemsList="56202";
					break;
				default:
					throw (new WebException.ProductListException("the methode doesn't contains treatement for the Id: "+idProductItemsList));
			}
		}
		#endregion

		#region Accessors
		/// <summary>
		/// Get list of sectors
		/// </summary>
		public string GetSectorItemsList{
			get{return(_sectorItemsList);}
		}

		/// <summary>
		/// Get list of subsectors
		/// </summary>
		public string GetSubSectorItemsList{
			get{return(_subSectorItemsList);}
		}

		/// <summary>
		/// Get list of groups
		/// </summary>
		public string GetGroupItemsList{
			get{return(_groupItemsList);}
		}

		/// <summary>
		/// Get list of segments
		/// </summary>
		public string GetSegmentItemsList{
			get{return(_segmentItemsList);}
		}

		/// <summary>
		/// Get list of products
		/// </summary>
		public string GetProductItemsList{
			get{return(_productItemsList);}
		}
		#endregion
	}
}
