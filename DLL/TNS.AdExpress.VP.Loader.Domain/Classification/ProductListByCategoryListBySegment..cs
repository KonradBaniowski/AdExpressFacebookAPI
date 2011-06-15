using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;

namespace TNS.AdExpress.VP.Loader.Domain.Classification {
    public class ProductListByCategoryListBySegment {

        #region Variables
        /// <summary>
        /// Segment
        /// </summary>
        Item _segment = null;
        /// <summary>
        /// Product List By Category
        /// </summary>
        Dictionary<Int64, ProductListByCategory> _productListByCategory = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="segment">Segment</param>
        /// <param name="productListByCategory">Product List By Category</param>
        public ProductListByCategoryListBySegment(Item segment, Dictionary<Int64, ProductListByCategory> productListByCategory) {
            if (segment == null) throw new ArgumentNullException("Segment parameter is null");
            if (productListByCategory == null) throw new ArgumentNullException("ProductListByCategory parameter is null");
            if (productListByCategory.Count <= 0) throw new ArgumentException("ProductListByCategory parameter is invalid");
            _segment = segment;
            _productListByCategory = productListByCategory;
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public ProductListByCategoryListBySegment() { }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Segment
        /// </summary>
        public Item Segment {
            get { return _segment; }
            set { _segment = value; }
        }
        /// <summary>
        /// Get Product List By Category
        /// </summary>
        public Dictionary<Int64, ProductListByCategory> ProductListByCategory {
            get { return _productListByCategory; }
            set { _productListByCategory = value; }
        }
        #endregion

    }
}
