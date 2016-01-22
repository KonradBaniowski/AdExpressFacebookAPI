using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.PSALoader.Domain.XmlLoader;

namespace KMI.AdExpress.PSALoader.Domain {
    /// <summary>
    /// Visual Information List
    /// </summary>
    public class VisualInformations {

        #region Variables
        /// <summary>
        /// Visual information list
        /// </summary>
        private static List<VisualInformation> _visualInformation;
        #endregion

        #region Init
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="source">XML File</param>
        public static void Init(IDataSource source) {
            _visualInformation = VisualInformationXL.Load(source);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Visual Information
        /// </summary>
        /// <param name="media">Media</param>
        /// <returns>Visual Information by media</returns>
        public static VisualInformation GetVisualInformation(Constantes.Vehicles.names media) {
            foreach (VisualInformation visualInformation in _visualInformation) {
                if (visualInformation.Media == media)
                    return visualInformation;
            }
            return null;
        }
        #endregion

    }
}
