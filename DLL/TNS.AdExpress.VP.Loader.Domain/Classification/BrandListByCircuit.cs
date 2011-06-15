using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TNS.AdExpress.VP.Loader.Domain.Classification {
    public class BrandListByCircuit:AllItemList {

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="circuit">Circuit</param>
        /// <param name="brandList">Brand List</param>
        public BrandListByCircuit(Item circuit, Dictionary<Int64, Item> brandList)
            : base(circuit, brandList) {
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public BrandListByCircuit() : base() { }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Circuit
        /// </summary>
        public Item Circuit {
            get { return _item; }
            set { _item = value; }
        }
        /// <summary>
        /// Get Brand List
        /// </summary>
        public Dictionary<Int64, Item> BrandList {
            get { return _itemList; }
            set { _itemList = value; }
        }
        #endregion

    }
}
