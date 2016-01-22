using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// top diffusion Information
    /// </summary>
    public class TopDiffusionInformation {

        #region Variables
        /// <summary>
        /// Top Diffusuion
        /// </summary>
        private string _topdiffusion;
        /// <summary>
        /// Duration
        /// </summary>
        private Int64 _duration;
        #endregion

        #region Constructor
        public TopDiffusionInformation(string topDiffusion, Int64 duration) {
            _topdiffusion = topDiffusion;
            _duration = duration;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Top Diffusion
        /// </summary>
        public string Topdiffusion {
            get { return (_topdiffusion); }
        }
        /// <summary>
        /// Get Duration
        /// </summary>
        public Int64 Duration {
            get { return (_duration); }
        }
        #endregion

    }
}
