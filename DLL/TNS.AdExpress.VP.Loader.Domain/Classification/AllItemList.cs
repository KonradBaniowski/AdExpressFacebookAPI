using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TNS.AdExpress.VP.Loader.Domain.Classification {
    public class AllItemList {

        #region Variables
        /// <summary>
        /// Item
        /// </summary>
        protected Item _item = null;
        /// <summary>
        /// Item List
        /// </summary>
        protected Dictionary<Int64, Item> _itemList = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="itemList">Item List</param>
        public AllItemList(Item item, Dictionary<Int64, Item> itemList) {
            if(item == null) throw new ArgumentNullException("Item parameter is null");
            if(itemList == null) throw new ArgumentNullException("ItemList parameter is null");
            _item = item;
            _itemList = itemList;
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public AllItemList() { }
        #endregion

        #region GetItemByItemListId
        /// <summary>
        /// Get Item By Item List Id
        /// </summary>
        /// <param name="itemListId">Item By Item List Id</param>
        /// <returns>Item</returns>
        public Item GetItemByItemListId(Int64 itemListId) {
            foreach(Item cItem in _itemList.Values){
                if (cItem.Id == itemListId) return cItem;
            }
            return null;
        }
        #endregion

        #region GetItemByItemListLabel
        /// <summary>
        /// Get Item By Item List Label
        /// </summary>
        /// <param name="itemListLabel">Item By Item List Label</param>
        /// <returns>Item</returns>
        public Item GetItemByItemListLabel(string itemListLabel) {
            foreach (Item cItem in _itemList.Values) {
                if (cItem.Label.Trim().ToUpper() == itemListLabel.Trim().ToUpper()) return cItem;
            }
            return null;
        }
        #endregion

    }
}
