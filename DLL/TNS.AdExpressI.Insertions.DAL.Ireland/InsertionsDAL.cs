using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Insertions.DAL.Ireland {
    /// <summary>
    /// Ireland Insertion DAL
    /// </summary>
    public class InsertionsDAL : TNS.AdExpressI.Insertions.DAL.InsertionsDAL{

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Current Module id</param>
        public InsertionsDAL(WebSession session, Int64 moduleId) : base(session, moduleId) {
        }
        #endregion

        #region GetDateFields
        /// <summary>
        /// Ge date fields
        /// </summary>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <returns>date fields</returns>
        protected override string GetDateFields(VehicleInformation vehicleInformation) {

            // Date fields           
            return ", date_media_num ";
        }
        #endregion

        #region GetFields
        /// <summary>
        /// Get SqL fields specific to one Mdia type
        /// </summary>
        /// <param name="idVehicle">ID media type</param>
        /// <param name="prefixeTable">Table Prefixe</param>
        /// <returns> champs de requêtes </returns>
        protected override string GetFields(Vehicles.names idVehicle, string prefixeTable) {
            switch (idVehicle) {
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioSponsorship:
                case Vehicles.names.radioMusic:
                case Vehicles.names.tv:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvAnnounces:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.others:
                    return ",id_media,TO_CHAR( duration)  as advertDimension, TO_CHAR(associated_file) as associated_file";
                case Vehicles.names.internationalPress:
                case Vehicles.names.press:
                case Vehicles.names.magazine:
                case Vehicles.names.newspaper:
                    return ",id_media,format as advertDimension, associated_file";
                case Vehicles.names.outdoor:
                case Vehicles.names.indoor:
                    return ",id_media,type_board as advertDimension, associated_file as associated_file";
                case Vehicles.names.mailValo:
                case Vehicles.names.directMarketing:
                    return ",id_media,TO_CHAR(weight) as advertDimension, TO_CHAR(associated_file) as associated_file";
                case Vehicles.names.adnettrack:
                case Vehicles.names.evaliantMobile:
                    return ",id_media, (dimension || ' / ' || format) as advertDimension, TO_CHAR(associated_file) as associated_file";
                default: return "";
            }
        }
        #endregion

    }
}
