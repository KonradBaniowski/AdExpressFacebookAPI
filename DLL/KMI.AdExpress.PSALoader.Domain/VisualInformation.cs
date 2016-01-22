using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.PSALoader.Domain {
    /// <summary>
    /// Contains information concerning media visuals
    /// </summary>
    public class VisualInformation {

        #region Variables
        /// <summary>
        /// Media
        /// </summary>
        private Constantes.Vehicles.names _media;
        /// <summary>
        /// Visual Path
        /// </summary>
        private string _visualpath = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="media">Media</param>
        /// <param name="visualpath">Visual Path</param>
        public VisualInformation(Constantes.Vehicles.names media, string visualpath) {
            _media = media;
            _visualpath = visualpath;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get media
        /// </summary>
        public Constantes.Vehicles.names Media {
            get { return (_media); }
        }
        /// <summary>
        /// Get Visual Path
        /// </summary>
        public string Visualpath {
            get { return (_visualpath); }
        }
        #endregion

    }
}
