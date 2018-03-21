using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.Insertions.Poland.Cells
{
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
