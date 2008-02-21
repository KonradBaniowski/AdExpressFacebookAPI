#region Informations
// Author: G. Facon
// Creation Date: 07/12/2004 
// Modification Date: 
//	07/12/2004	G. Facon	?
//	10/08/2006	Y. Rkaina	?
#endregion

using System;
using System.Collections;

using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebException=TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Core.ClassificationList{
	/// <summary>
	/// This classe contains product classification items Lists used in AdExpress web site
	/// </summary>
	/// <example>
	/// For exemple, in all the results we have to exclude some product
	/// </example>
	public class Product{

		#region Variables
		/// <summary>
		/// Product classification universe list
		/// </summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.ClassificationList.ProductItemsList</associates>
		protected static Hashtable _list=null;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialization of the lists
		/// </summary>
		public static void Init(){
			try{
				_list=new Hashtable();
				// Product to exclude 
				_list.Add(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,new ProductItemsList(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID));
				// Autopromotion product to exclure
				_list.Add(WebConstantes.AdExpressUniverse.AUTOPROMO_RADIO_PRODUCT_LIST_ID,new ProductItemsList(WebConstantes.AdExpressUniverse.AUTOPROMO_RADIO_PRODUCT_LIST_ID));
				// Products to exclude of the reports (tableaux de bord)
				_list.Add(WebConstantes.AdExpressUniverse.DASHBOARD_PRESS_EXCLUDE_PRODUCT_LIST_ID,new ProductItemsList(WebConstantes.AdExpressUniverse.DASHBOARD_PRESS_EXCLUDE_PRODUCT_LIST_ID));
			}
			catch(System.Exception ex){
				throw (new WebException.ProduitException("Impossible to initialize a product list",ex));
			}
		}
		#endregion

		#region Data Access
		/// <summary>
		/// Get the list
		/// </summary>
		/// <param name="idProductItemsList">List Id</param>
		/// <returns></returns>
		public static ProductItemsList GetProductItemsList(int idProductItemsList){
			try{
				if(_list==null)
					Init();
				return((ProductItemsList)_list[idProductItemsList]);
			}
			catch(System.Exception){
				throw (new WebException.ProduitException("the Product list doesn't exists for Id: "+idProductItemsList));
			}
		}
		#endregion
	}
}
