using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Results;
using System.Globalization;
using TNS.AdExpress.VP.Loader.Domain.Classification;
using TNS.AdExpress.Constantes.DB;
using System.Data;
using TNS.AdExpressI.VP.Loader.Exceptions;
using TNS.AdExpressI.VP.Loader.DAL.Classification;
using System.Reflection;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpressI.VP.Loader.Classification {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public abstract class ClassifVeillePromo : VeillePromo, IClassifVeillePromo {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        public ClassifVeillePromo(DataBase dataBase):base(dataBase) {
        }
        #endregion

        #region IVeillePromoDAL Membres

        #region Get All Product
        /// <summary>
        /// Get Data all product
        /// </summary>
        /// <returns>Data all product</returns>
        public Dictionary<Int64, ProductListByCategoryListBySegment> GetAllProduct() {
            try {

                #region Get Data
                IClassifVeillePromoDAL veillePromoDAL = (IClassifVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);
                DataSet ds = veillePromoDAL.GetAllProduct();
                #endregion

                if (ds == null || ds.Tables == null || ds.Tables.Count != 1 || ds.Tables[0] == null || ds.Tables[0].Rows == null || ds.Tables[0].Rows.Count <= 0)
                    return null;

                Dictionary<Int64, ProductListByCategoryListBySegment> productListByCategoryListBySegment = new Dictionary<long, ProductListByCategoryListBySegment>();
                Dictionary<Int64, ProductListByCategory> productListByCategory = new Dictionary<long, ProductListByCategory>();
                Dictionary<Int64, Item> productList = new Dictionary<Int64, Item>();
                Int64 cIdSegment;
                Int64 cIdCategory;
                Int64 cIdProduct;
                Int64? cIdSegmentOld = null;
                Int64? cIdCategoryOld = null;
                Int64? cIdProductOld = null;


                foreach (DataRow cRow in ds.Tables[0].Rows) {
                    cIdSegment = Int64.Parse(cRow["id_segment"].ToString());
                    cIdCategory = Int64.Parse(cRow["id_category"].ToString());
                    cIdProduct = Int64.Parse(cRow["id_product"].ToString());


                    //NEW Category OR NEW SEGMENT
                    if ((cIdCategoryOld == null || cIdCategory != cIdCategoryOld.Value)
                        || (cIdSegmentOld == null || cIdSegment != cIdSegmentOld.Value)) {
                        productList = new Dictionary<Int64, Item>();
                        productListByCategory = new Dictionary<Int64, ProductListByCategory>();
                        productListByCategory.Add(cIdCategory,
                            new ProductListByCategory(
                                new Item(cIdCategory, cRow["category"].ToString()),
                                productList
                            )
                        );
                    }

                    //NEW SEGMENT
                    if (cIdSegmentOld == null || cIdSegment != cIdSegmentOld.Value) {
                        productListByCategoryListBySegment.Add(
                            cIdSegment,
                            new ProductListByCategoryListBySegment(
                                new Item(cIdSegment, cRow["segment"].ToString()),
                                productListByCategory
                            )
                        );
                    }

                    productListByCategoryListBySegment[cIdSegment].ProductListByCategory[cIdCategory].ProductList.Add(
                        cIdProduct
                        , new Item(cIdProduct, cRow["product"].ToString()));


                    cIdSegmentOld = cIdSegment;
                    cIdCategoryOld = cIdCategory;
                    cIdProductOld = cIdProduct;

                }

                return productListByCategoryListBySegment;

            }
            catch (System.Exception err) {
                throw new VeillePromoClassifException("DataRules GetAllProduct Error", err);
            }
        }
        #endregion

        #region Get All Brand
        /// <summary>
        /// Get Data all Brand
        /// </summary>
        /// <returns>Data all Brand</returns>
        public Dictionary<Int64, BrandListByCircuit> GetAllBrand() {

            try {

                #region Get Data
                IClassifVeillePromoDAL veillePromoDAL = (IClassifVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);
                DataSet ds = veillePromoDAL.GetAllBrand();
                #endregion

                if (ds == null || ds.Tables == null || ds.Tables.Count != 1 || ds.Tables[0] == null || ds.Tables[0].Rows == null || ds.Tables[0].Rows.Count <= 0)
                    return null;

                Dictionary<Int64, BrandListByCircuit> brandListByCircuit = new Dictionary<long, BrandListByCircuit>();
                Dictionary<Int64, Item> brandList = new Dictionary<Int64, Item>();
                Int64 cIdCircuit;
                Int64 cIdBrand;
                Int64? cIdCircuitOld = null;
                Int64? cIdBrandOld = null;


                foreach (DataRow cRow in ds.Tables[0].Rows) {
                    cIdCircuit = Int64.Parse(cRow["id_circuit"].ToString());
                    cIdBrand = Int64.Parse(cRow["id_brand"].ToString());


                    //NEW Circuit
                    if (cIdCircuitOld == null || cIdCircuit != cIdCircuitOld.Value) {
                        brandList = new Dictionary<Int64, Item>();
                        brandListByCircuit = new Dictionary<Int64, BrandListByCircuit>();
                        brandListByCircuit.Add(cIdCircuit,
                            new BrandListByCircuit(
                                new Item(cIdCircuit, cRow["circuit"].ToString()),
                                brandList
                            )
                        );
                    }

                    brandListByCircuit[cIdCircuit].BrandList.Add(
                        cIdBrand
                        , new Item(cIdBrand, cRow["brand"].ToString()));


                    cIdCircuitOld = cIdCircuit;
                    cIdBrandOld = cIdBrand;

                }

                return brandListByCircuit;

            }
            catch (System.Exception err) {
                throw new VeillePromoClassifException("DataRules GetAllBrand Error", err);
            }
        }
        #endregion

        #endregion

    }
}
