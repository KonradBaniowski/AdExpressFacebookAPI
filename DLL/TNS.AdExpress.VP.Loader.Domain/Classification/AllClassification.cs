using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.VP.Loader.Domain.Exceptions;

namespace TNS.AdExpress.VP.Loader.Domain.Classification {
    public class AllClassification {

        #region variables
        ///<summary>
        /// Product List By Category List By Segment
        /// </summary>
        private static Dictionary<Int64, ProductListByCategoryListBySegment> _productListByCategoryListBySegment = null;
        ///<summary>
        /// Brand List By Circuit
        /// </summary>
        private static Dictionary<Int64, BrandListByCircuit> _brandListByCircuit = null;
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
        static AllClassification() {
		}
		#endregion

        #region Get Brand informations
        /// <summary>
        /// Get Brand informations
        /// </summary>
        public static Item GetBrand(string brandLabel) {
            List<Item> itemList = new List<Item>();
            if (_brandListByCircuit != null) {
                foreach (BrandListByCircuit brandListByCircuit in _brandListByCircuit.Values) {
                    List<Item> itemListTemp = null;
                    itemListTemp = (from brand in brandListByCircuit.BrandList.Values
                                    where brand.Label.Trim().ToUpper() == brandLabel.Trim().ToUpper()
                                    select brand).ToList<Item>();

                    if (itemListTemp != null && itemListTemp.Count>0)
                        itemList.AddRange(itemListTemp);
                }
            }
            if (itemList != null && itemList.Count == 1) return itemList[0];
            else throw (new AllClassificationException("impossible to reteive the requested Brand " + brandLabel));
        }
        #endregion

        #region Get Product informations
        /// <summary>
        /// Get Product informations
        /// </summary>
        public static Item GetProduct(string productLabel) {
            List<Item> itemList = new List<Item>();

            if (_productListByCategoryListBySegment != null) {
                foreach (ProductListByCategoryListBySegment productListByCategoryListBySegment in _productListByCategoryListBySegment.Values) {
                    foreach (ProductListByCategory productListByCategory in productListByCategoryListBySegment.ProductListByCategory.Values) {
                        List<Item> itemListTemp = null;
                        itemListTemp = (from product in productListByCategory.ProductList.Values
                                        where product.Label.Trim().ToUpper() == productLabel.Trim().ToUpper()
                                        select product).ToList<Item>();

                        if (itemListTemp != null && itemListTemp.Count>0)
                            itemList.AddRange(itemListTemp);
                    }
                }
            }
            if (itemList != null && itemList.Count == 1) return itemList[0];
            else throw (new AllClassificationException("impossible to reteive the requested Product " + productLabel));
        }
        #endregion

        #region Get Circuit informations
        /// <summary>
        /// Get Circuit informations
        /// </summary>
        public static Item GetCircuit(Int64 brandId) {
            List<Item> itemList = null;
            if (_brandListByCircuit != null) {
                itemList = (from brandListByCircuit in _brandListByCircuit.Values
                            where brandListByCircuit.BrandList.ContainsKey(brandId)
                            select brandListByCircuit.Circuit).ToList<Item>();
            }
            if (itemList != null && itemList.Count == 1) return itemList[0];
            else throw (new AllClassificationException("impossible to reteive the requested Circuit " + brandId));
        }
        #endregion

        #region Get Category informations
        /// <summary>
        /// Get Category informations
        /// </summary>
        public static Item GetCategory(Int64 productId) {
            List<Item> itemList = new List<Item>();
            if (_brandListByCircuit != null) {
                foreach (ProductListByCategoryListBySegment productListByCategoryListBySegment in _productListByCategoryListBySegment.Values) {
                    List<Item> itemListTemp = null;

                    itemListTemp = (from productListByCategory in productListByCategoryListBySegment.ProductListByCategory.Values
                                where productListByCategory.ProductList.ContainsKey(productId)
                                select productListByCategory.Category).ToList<Item>();

                    if (itemListTemp != null && itemListTemp.Count>0)
                        itemList.AddRange(itemListTemp);
                }
            }
            if (itemList != null && itemList.Count == 1) return itemList[0];
            else throw (new AllClassificationException("impossible to reteive the requested Category " + productId));
        }
        #endregion

        #region Get Segment informations
        /// <summary>
        /// Get Segment informations
        /// </summary>
        public static Item GetSegmentByProductId(Int64 productId) {
            List<Item> itemList = null;
            if (_brandListByCircuit != null) {

                itemList = (from productListByCategoryListBySegment in _productListByCategoryListBySegment.Values
                            where (from productListByCategory in productListByCategoryListBySegment.ProductListByCategory.Values
                                   where productListByCategory.ProductList.ContainsKey(productId)
                                   select productListByCategory.Category
                                   ).Count()>0
                            select productListByCategoryListBySegment.Segment).ToList<Item>();
            }
            if (itemList != null && itemList.Count == 1) return itemList[0];
            else throw (new AllClassificationException("impossible to reteive the requested Category " + productId));
        }

        /// <summary>
        /// Get Segment informations
        /// </summary>
        public static Item GetSegmentByCategory(Int64 categoryId) {
            List<Item> itemList = null;
            if (_brandListByCircuit != null) {
                itemList = (from item in _productListByCategoryListBySegment.Values
                            where item.ProductListByCategory.ContainsKey(categoryId)
                            select item.Segment).ToList<Item>();
            }
            if (itemList != null && itemList.Count == 1) return itemList[0];
            else throw (new AllClassificationException("impossible to reteive the requested Category " + categoryId));
        }
        #endregion

        #region Init
        /// <summary>
        /// Initialisation de la liste
        /// </summary>
        /// <param name="brandListByCircuit">Brand List By Circuit</param>
        /// <param name="productListByCategoryListBySegment">Product List By Category List By Segment</param>
        public static void Init(Dictionary<Int64, ProductListByCategoryListBySegment> productListByCategoryListBySegment, Dictionary<Int64, BrandListByCircuit> brandListByCircuit) {
            if (productListByCategoryListBySegment == null) throw new ArgumentNullException("productListByCategoryListBySegment parameter is null");
            if (brandListByCircuit == null) throw new ArgumentNullException("brandListByCircuit parameter is null");
            _productListByCategoryListBySegment = productListByCategoryListBySegment;
            _brandListByCircuit = brandListByCircuit;
        }
        #endregion
    }
}
