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

using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstVMCMedia = TNS.AdExpress.Constantes.DB.Media;
using CstVMCFormat = TNS.AdExpress.Constantes.DB.Format;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;

using System;
using System.Globalization;
using System.Threading;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Level;
using System.Collections.Generic;
using TNS.FrameWork.WebResultUI;
using System.Data;
using System.Text;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;

namespace TNS.AdExpressI.Insertions.Cells
{
    /// <summary>
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellCreativesTvInformation : CellCreativesInformation
    {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesTvInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, Module module)
            : base(session, vehicle, columns, columnNames, cells, module)
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesTvInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, Module module, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells, module, idColumnsSet) {
        }
        #endregion

        #region Render
        public override string Render(string cssClass)
        {
            StringBuilder str = new StringBuilder();

            if (_newGroup)
                str.Append(RenderSeparator());

            string value;
            string[] values;
            int i = -1;

            #region Init Informations
            List<string> cols = new List<string>();

            bool hasData = false;
            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                _values[i].Parent = this.Parent;
                value = _values[i].ToString();
                if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster && g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

                    StringBuilder tmpStr = new StringBuilder();
                    tmpStr.AppendFormat("<td width=\"1%\"><span>{0}<span></td> ", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
                    tmpStr.Append("<td>: ");
                    hasData = false;
                    if (_values[i] != null)
                    {
                        if (!(_values[i] is CellUnit))
                        {
                            values = value.Split(',');
                            foreach (string s in values)
                            {
                                if (hasData)
                                {
                                    tmpStr.Append("<br/>");
                                }
                                hasData = true;
                                tmpStr.AppendFormat("{0}", s);
                            }
                        }
                        else
                        {
                            tmpStr.AppendFormat("{0}", value);
                        }
                    }
                    tmpStr.Append("</td>");
                    cols.Add(tmpStr.ToString());
                }
            }
            #endregion

            str.AppendFormat("<td class=\"{0}\"><table>", cssClass);

            #region visuals
            bool hasVisual = false;
            str.Append("<tr><td valign=\"top\">");
            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    str.AppendFormat("<a href=\"javascript:openDownload('{0}','{1}','{2}');\" class=\"videoFileBackGround\"></a>", s, this._session.IdSession, _vehicle.DatabaseId);
                    hasVisual = true;
                }
            }
            if (!hasVisual)
            {
                str.AppendFormat("<span>{0}</span>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
            }

            str.Append("</td></tr>");
            #endregion

            #region Info
            str.Append("<p><tr><td><table>");
            int nbLine = (int)Math.Ceiling(((double)cols.Count) / 2.0);
            for (int l = 0; l < nbLine; l++)
            {
                str.Append("<tr>");
                str.Append(cols[l]);
                str.Append("<td>&nbsp;</td>");
                if (l + nbLine < cols.Count)
                {
                    str.Append(cols[l + nbLine]);
                }
                else
                {
                    str.Append("<td></td><td></td>");
                }
                str.Append("<td width=\"100%\"></td></tr>");
            }
            str.Append("</table></td></tr></p>");
            #endregion

            str.Append("</tr></table></td>");

            return str.ToString();
        }
        #endregion

    }

}
