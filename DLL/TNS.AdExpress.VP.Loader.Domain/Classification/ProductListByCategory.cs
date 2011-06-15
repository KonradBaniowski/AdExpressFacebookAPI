using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TNS.AdExpress.VP.Loader.Domain.Classification {
    public class ProductListByCategory:AllItemList {

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="productList">Product List</param>
        public ProductListByCategory(Item category, Dictionary<Int64, Item> productList)
            : base(category, productList) {
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public ProductListByCategory() : base() { }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Category
        /// </summary>
        public Item Category {
            get { return _item; }
            set { _item = value; }
        }
        /// <summary>
        /// Get Product List
        /// </summary>
        public Dictionary<Int64, Item> ProductList {
            get { return _itemList; }
            set { _itemList = value; }
        }
        #endregion

    }
}
