using System.Web.UI;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Insertions.France.CreativeResult
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
        public CreativePopUp(Page popUp, Vehicles.names vehicle, string idSlogan, string file, WebSession webSession, string title, bool hasCreationReadRights, bool hasCreationDownloadRights)
            : base(popUp, vehicle, idSlogan, file, webSession, title, hasCreationReadRights, hasCreationDownloadRights) { }
        #endregion
    }
}
