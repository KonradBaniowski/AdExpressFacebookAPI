#region Information
//  Author : A. Rousseau
//  Creation  date: 05/09/2011
//  Modifications:
#endregion

using System.Collections.Generic;
using System;
using TNS.AdExpress.Constantes.Customer;

namespace TNS.AdExpress.Domain.Classification {
    /// <summary>
    /// Vehicle description
    /// </summary>
    public class VehiclesFormatInformation {

        #region Variables
        /// <summary>
        /// Use
        /// </summary>
        private bool _use;
        /// <summary>
        /// Vehicle Format Information List
        /// </summary>
        private Dictionary<Int64, VehicleFormatInformation> _vehicleFormatInformationList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="use">Use</param>
        /// <param name="vehicleFormatInformationList">Vehicle Format Information List</param>
        public VehiclesFormatInformation(bool use, Dictionary<Int64, VehicleFormatInformation> vehicleFormatInformationList)
        {
            _use = use;
            _vehicleFormatInformationList = vehicleFormatInformationList;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get vehicle id
        /// </summary>
        public bool Use{
            get { return _use; }
        }
        /// <summary>
        /// Get Vehicle Format Information List
        /// </summary>
        public Dictionary<Int64, VehicleFormatInformation> VehicleFormatInformationList {
            get { return _vehicleFormatInformationList; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Right Banners Type List
        /// </summary>
        /// <param name="vehicleInformationList">Vehicle Information List</param>
        /// <returns>Right Banners Type List</returns>
        public List<RightBanners.Type> GetRightBannersTypeList(Dictionary<Int64, VehicleInformation> vehicleInformationList)
        {
            var rightBannersTypeList = new List<RightBanners.Type>();
            foreach (var cVehicleInformation in vehicleInformationList.Values)
            {
                if(_vehicleFormatInformationList.ContainsKey(cVehicleInformation.DatabaseId))
                {
                    if(!rightBannersTypeList.Contains(_vehicleFormatInformationList[cVehicleInformation.DatabaseId].RightBannersType))
                        rightBannersTypeList.Add(_vehicleFormatInformationList[cVehicleInformation.DatabaseId].RightBannersType);
                }
            }

            return rightBannersTypeList;
        }
        #endregion

    }
}
