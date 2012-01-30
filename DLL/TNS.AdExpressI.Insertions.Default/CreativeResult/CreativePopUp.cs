
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.UI;

using CstClassificationVehicle = TNS.AdExpress.Constantes.Classification.DB.Vehicles;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Domain.Translation;
using System.Data;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpressI.Insertions.Default.CreativeResult
{
     /// <summary>
    /// Class used to display tv and radio creatives in reading and streamin mode
    /// </summary>
    public class CreativePopUp : TNS.AdExpressI.Insertions.CreativeResult.CreativePopUp{

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idSlogan">Slogan identifier</param>
        /// <param name="file">File</param>
        /// <param name="webSession">WebSession</param>
        /// <param name="title">Title</param>
        public CreativePopUp(Page popUp, CstClassificationVehicle.names vehicle, string idSlogan, string file, WebSession webSession, string title, bool hasCreationReadRights, bool hasCreationDownloadRights)
            : base(popUp, vehicle, idSlogan, file, webSession, title, hasCreationReadRights, hasCreationDownloadRights) { }
        #endregion
    }
}
