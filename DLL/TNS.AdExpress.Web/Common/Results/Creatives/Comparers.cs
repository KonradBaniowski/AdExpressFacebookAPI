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

using TNS.AdExpress.Web.Common.Results.Creatives;

namespace TNS.AdExpress.Web.Common.Results.Creatives.Comparers {


    #region Advertiser Comparer
    ///<summary>
    /// Creative Advertiser Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 13 août 2007 11:24:28</since>
    public class AdvertiserComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Advertiser Comparer Caption</param>
        public AdvertiserComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Advertiser Comparer Caption ID</param>
        public AdvertiserComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem Advertisers
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return x.Advertiser.CompareTo(y.Advertiser);
        }
        #endregion

    }
    #endregion

    #region Group Comparer
    ///<summary>
    /// Creative Group Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 13 août 2007 11:24:28</since>
    public class GroupComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Group Comparer Caption</param>
        public GroupComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Group Comparer Caption Id</param>
        public GroupComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem groups
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return x.Group.CompareTo(y.Group);
        }
        #endregion

    }
    #endregion

    #region Product Comparer
    ///<summary>
    /// Creative Product Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 13 août 2007 11:24:28</since>
    public class ProductComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Product Comparer Caption</param>
        public ProductComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="captionId">Product Comparer Caption</param>
        public ProductComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem products
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return x.Product.CompareTo(y.Product);
        }
        #endregion

    }
    #endregion

    #region Version Comparer
    ///<summary>
    /// Creative Version Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 13 août 2007 11:24:28</since>
    public class VersionComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Version Comparer Caption</param>
        public VersionComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="captionId">Version Comparer Caption</param>
        public VersionComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem versions
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return x.Id.CompareTo(y.Id);
        }
        #endregion

    }
    #endregion

    #region Budget Comparer
    ///<summary>
    /// Creative Budget Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 20 septembre 2007 11:24:28</since>
    public class BudgetComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Budget Comparer Caption</param>
        public BudgetComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="captionId">Budget Comparer Caption</param>
        public BudgetComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem Budgets
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return x.Budget.CompareTo(y.Budget);
        }
        #endregion

    }
    #endregion

    #region Active Media Nb Comparer
    ///<summary>
    /// Creative Active Media Nb Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 13 août 2007 11:24:28</since>
    public class MediaNbComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Active Media Nb Comparer Caption</param>
        public MediaNbComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="captionId">Active Media Nb Comparer Caption</param>
        public MediaNbComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem Active Media Nb
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return x.MediaNb.CompareTo(y.MediaNb);
        }
        #endregion

    }
    #endregion

    #region Insert Nb Comparer
    ///<summary>
    /// Creative Insert Nb Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 20 septembre 2007 11:24:28</since>
    public class InsertNbComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Insert Nb Comparer Caption</param>
        public InsertNbComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="captionId">Insert Nb Comparer Caption</param>
        public InsertNbComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem Insert Nbs
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return x.InsertNb.CompareTo(y.InsertNb);
        }
        #endregion

    }
    #endregion

}

