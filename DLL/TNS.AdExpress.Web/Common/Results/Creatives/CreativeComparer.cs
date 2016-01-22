#region Info
/*
 * Author           : G RAGNEAU 
 * Date             : 13/08/2007
 * Modifications    :
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using TNS.FrameWork.WebResultUI.TableControl;

namespace TNS.AdExpress.Web.Common.Results.Creatives {


	///<summary>
	/// Creative comparer
	/// </summary>
	/// <author>Guillaume.Ragneau</author>
	/// <since>lundi 13 août 2007 11:42:23</since>
	public abstract class CreativeComparer:ITableComparer {


	    #region Attributes
		/// <summary>
		/// Filter Caption
		/// </summary>
        protected string _caption = "Toto";
		/// <summary>
		/// Filter Caption Id (used to display diffent captions depending on language
		/// </summary>
        protected int _captionId = 0;
		/// <summary>
		/// Filter ID
		/// </summary>
        protected int _id = -1;
		/// <summary>
		/// Sort order
		/// </summary>
        protected static SortOrder _sort = SortOrder.ASC;
        #endregion

        #region Constructor
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="caption">Comparer Caption</param>
        public CreativeComparer( string caption ) {
            _caption = caption;
        }

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="caption">Comparer Caption</param>
        public CreativeComparer( int captionId ) {
            _captionId = captionId;
        }
        #endregion
    	
        #region ITableComparer Membres
		/// <summary>
		/// Get Filter Caption
		/// </summary>
		/// <returns>Filter Caption</returns>
        public string GetCaption( ) {
            return _caption;
        }
		/// <summary>
		/// Get Filter Caption Id
		/// </summary>
		/// <returns>Filter Caption</returns>
        public int GetCaptionId( ) {
            return _captionId;
        }
		/// <summary>
		/// Get Filter Id
		/// </summary>
		/// <returns>Filter Id</returns>
        public int GetId( ) {
            return _id;
        }
		/// <summary>
		/// Set Comparer Id
		/// </summary>
		/// <param name="id">Id of the comparer</param>
		/// <returns></returns>
        public void SetId( int id ) {
            this._id = id ;
        }
		/// <summary>
		/// Set Sort Order
		/// </summary>
		/// <param name="sort">Sort Order</param>
        public void SetOrder( SortOrder sort ) {
            _sort = sort;
        }
		/// <summary>
		/// Get Sort Order
		/// </summary>
		/// <returns>Sort Order</returns>
        public SortOrder GetSortOrder( ) {
            return _sort;
        }
        #endregion

        #region IComparer<TableElement> Membres
		/// <summary>
		/// Compare x and y
		/// </summary>
		/// <param name="x">First element</param>
		/// <param name="y">Second element</param>
		/// <returns>-1 if x > y, 0 if x=y, 1 if x < y </returns>
        public int Compare( ITableElement x, ITableElement y ) {
            return this.Compare((CreativeItem)x, (CreativeItem)y) * (int)_sort;
        }
        #endregion

        #region Abstract Method
		/// <summary>
		/// Compare x and y
		/// </summary>
		/// <param name="x">First element</param>
		/// <param name="y">Second element</param>
		/// <returns>-1 if x > y, 0 if x=y, 1 if x < y </returns>
        public abstract int Compare( CreativeItem x, CreativeItem y );
        #endregion

	}
}
