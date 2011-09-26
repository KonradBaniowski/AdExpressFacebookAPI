#region Information
///  Author : A. Rousseau
//  Creation  date: 05/09/2011
//  Modifications:
#endregion

using System;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Domain.Classification {
    /// <summary>
    /// Vehicle description
    /// </summary>
    public class VehicleFormatInformation {

        #region Variables
        /// <summary>
        /// Vehicle Identifier
        /// </summary>
        private Int64 _vehicleId;
        /// <summary>
        /// Data Table Name
        /// </summary>
        private TableIds _dataTableName;
        /// <summary>
        /// Format Table Name
        /// </summary>
        private TableIds _formatTableName;
        /// <summary>
        /// Right Banners Type
        /// </summary>
        private Constantes.Customer.RightBanners.Type _rightBannersType;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vehicleId">Vehicle id</param>
        /// <param name="dataTableName">Data table name</param>
        /// <param name="formatTableName">Format Table Name</param>
        /// <param name="rightBannersType">Right Banners Type</param>
        public VehicleFormatInformation(Int64 vehicleId, Constantes.Customer.RightBanners.Type rightBannersType, TableIds dataTableName, TableIds formatTableName)
        {

            _vehicleId = vehicleId;
            _dataTableName = dataTableName;
            _formatTableName = formatTableName;
            _rightBannersType = rightBannersType;

        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get vehicle id
        /// </summary>
        public Int64 VehicleId {
            get { return _vehicleId; }
        }
        /// <summary>
        /// Get Right Banners Type
        /// </summary>
        public Constantes.Customer.RightBanners.Type RightBannersType {
            get { return _rightBannersType; }
        }
        /// <summary>
        /// Get Data Table Name
        /// </summary>
        public TableIds DataTableName {
            get { return _dataTableName; }
        }
        /// <summary>
        /// Get Format Table Name
        /// </summary>
        public TableIds FormatTableName {
            get { return _formatTableName; }
        }
        #endregion

    }
}
