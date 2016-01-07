#region Informations
/* Author: G. Ragneau
 * Created On : 29/09/2008
 * Updates :
 *      Date - Author - Description
 * 
 * 
 * 
 * */
#endregion

#region Using

using System;
using TNS.AdExpressI.Insertions.Cells;
using TNS.AdExpress.Domain.Level;
using System.Collections.Generic;
using TNS.FrameWork.WebResultUI;
using System.Text;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;
using System.Reflection;

#endregion

namespace TNS.AdExpressI.Insertions.France.Cells
{
    /// <summary>
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellCreativesRadioInformation : Insertions.Cells.CellCreativesRadioInformation
    {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesRadioInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns,
            List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module)
            : base(session, vehicle, columns, columnNames, cells, module)
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesRadioInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns,
            List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells, module, idColumnsSet)
        {
        }
        #endregion






        protected override void SetOpenDownloadScript(StringBuilder str, string s)
        {
            str.AppendFormat("<a href=\"javascript:openDownload('{0}','{1}','{2}');\"><div class=\"audioFileBackGround\"></div></a>"
                , _idVersion, _session.IdSession, _vehicle.DatabaseId);
        }




    }

}
