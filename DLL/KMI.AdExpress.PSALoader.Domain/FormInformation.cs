using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace KMI.AdExpress.PSALoader.Domain {
    /// <summary>
    /// This class contains information concerning a form
    /// </summary>
    public class FormInformation {

        #region Variables
        /// <summary>
        /// Form Id
        /// </summary>
        private Int64 _formId;
        /// <summary>
        /// Accroche Id
        /// </summary>
        private Int64 _sloganId;
        /// <summary>
        /// Date Media Num : yyyyMM
        /// </summary>
        private string _dateMediaNum;
        /// <summary>
        /// Vehicle : Press, Tv ...
        /// </summary>
        private Constantes.Vehicles.names _vehicle;
        /// <summary>
        /// File Extension : jpg, mp4 ...
        /// </summary>
        private string _extension = string.Empty;
        /// <summary>
        /// Tv Script
        /// </summary>
        private string _script = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="formId">Form Id</param>
        /// <param name="sloganId">Accroche Id</param>
        /// <param name="dateMediaNum">Date Media Num : yyyyMM</param>
        /// <param name="vehicle">Media : Press, Tv ...</param>
        /// <param name="extension">File Extension : jpg, mp4 ...</param>
        /// <param name="script">Tv Script</param>
        public FormInformation(Int64 formId, Int64 sloganId, string dateMediaNum, Constantes.Vehicles.names vehicle, string extension, string script) {

            //if (extension == null || extension.Length == 0) throw (new ArgumentException("Invalid extension parameter"));

            _formId = formId;
            _sloganId = sloganId;
            _dateMediaNum = dateMediaNum;
            _vehicle = vehicle;
            _extension = extension;
            _script = script;

        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Form Id
        /// </summary>
        public Int64 FormId {
            get { return (_formId); }
        }
        /// <summary>
        /// Get Accroche Id
        /// </summary>
        public Int64 SloganId {
            get { return (_sloganId); }
        }
        /// <summary>
        /// Get Date Media Num : yyyyMM
        /// </summary>
        public string DateMediaNum {
            get {
                string[] dateElements = _dateMediaNum.Split('/');
                return (dateElements[2] + dateElements[1] + dateElements[0]); 
            }
        }
        /// <summary>
        /// Get VehicleId : 1, 3 ...
        /// </summary>
        public int VehicleId {
            get { return (_vehicle.GetHashCode()); }
        }
        /// <summary>
        /// File Extension : jpg, mp4 ...
        /// </summary>
        public string Extension {
            get { return (_extension); }
        }
        /// <summary>
        /// Get Tv Script
        /// </summary>
        public string Script {
            get { return (_script); }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Get Promotion Visual files name
        /// </summary>
        /// <returns></returns>
        public string GetPromotionVisual() {

            //Treatment to extract visual files name
            switch (_vehicle) { 
                case Constantes.Vehicles.names.PRESSE:            
                case Constantes.Vehicles.names.PUBLICITE_EXTERIEURE:
                case Constantes.Vehicles.names.INTERNET:
                case Constantes.Vehicles.names.EVALIANT_MOBILE:
                case Constantes.Vehicles.names.COURRIER_CREATIONS:
                    return GetVisualNames() ;
                case Constantes.Vehicles.names.TELEVISION:
                case Constantes.Vehicles.names.RADIO:
                    return _sloganId + "." + _extension;
                default:
                    return string.Empty;
            }

        }
        /// <summary>
        /// Get Tv Board files name
        /// </summary>
        /// <returns></returns>
        public string GetTvBoard() {
            return GetVisualNames();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Get Visual Names
        /// </summary>
        /// <returns></returns>
        private string GetVisualNames() {

            string[] visualFiles = Directory.GetFiles(BuildVisualPath(), _formId + "*");
            string visualNames = string.Empty;

            foreach (string s in visualFiles)
                visualNames += Path.GetFileName(s) + ",";

            if (visualNames.Length > 0)
                visualNames = visualNames.Substring(0, visualNames.Length - 1);

            return visualNames;
        }

        /// <summary>
        /// Build Visual Path
        /// </summary>
        /// <returns></returns>
        private string BuildVisualPath() {

            string firstLetter = _formId.ToString().Substring(0,1);
            string NextThreeletters = _formId.ToString().Substring(1, 3);

            switch (_vehicle) {
                case Constantes.Vehicles.names.PRESSE:
                case Constantes.Vehicles.names.TELEVISION:            
                    return VisualInformations.GetVisualInformation(Constantes.Vehicles.names.PRESSE).Visualpath + firstLetter + "\\" + NextThreeletters;
                    //return VisualInformations.GetVisualInformation(Constantes.Vehicles.names.TELEVISION).Visualpath + DateMediaNum.Substring(4,4);
                case Constantes.Vehicles.names.PUBLICITE_EXTERIEURE:
                    return VisualInformations.GetVisualInformation(Constantes.Vehicles.names.PUBLICITE_EXTERIEURE).Visualpath + firstLetter + "\\" + NextThreeletters;
                case Constantes.Vehicles.names.INTERNET:
                    return VisualInformations.GetVisualInformation(Constantes.Vehicles.names.INTERNET).Visualpath + firstLetter + "\\" + NextThreeletters;
                case Constantes.Vehicles.names.EVALIANT_MOBILE:
                    return VisualInformations.GetVisualInformation(Constantes.Vehicles.names.EVALIANT_MOBILE).Visualpath + firstLetter + "\\" + NextThreeletters;
                case Constantes.Vehicles.names.COURRIER_CREATIONS:
                    return VisualInformations.GetVisualInformation(Constantes.Vehicles.names.COURRIER_CREATIONS).Visualpath + firstLetter + "\\" + NextThreeletters;
                default:
                    return string.Empty;
            }

        }
        #endregion

    }
}
