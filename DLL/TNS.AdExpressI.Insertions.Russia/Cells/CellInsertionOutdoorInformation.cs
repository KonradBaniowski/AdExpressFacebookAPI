#region Informations
/* Author: D. Mussuma
 * Created On : 21/04/2010
 * Updates :
 *      Date - Author - Description
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork;
using TNS.FrameWork.WebResultUI;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;
using System.Data.SqlTypes;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Insertions.Russia.Cells
{
    /// <summary>
    /// Outdoor cell insertion information 
    /// </summary>
    [System.Serializable]
    public class CellInsertionOutdoorInformation : TNS.AdExpressI.Insertions.Russia.Cells.CellInsertionInformation
    {

          #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellInsertionOutdoorInformation(WebSession session, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, VehicleInformation vehicle)
            : base(session, columns, columnNames, cells, vehicle)
        {                
        }
        #endregion

        #region Render
        /// <summary>
        /// Rendu de code HTML avec un style css spécifique
        /// </summary>
        /// <returns>Code HTML</returns>
        public override string Render(string cssClass)
        {
            StringBuilder str = new StringBuilder();
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            if (_newGroup)
                str.Append(RenderSeparator());

            string value;
            string[] values;
            int i = -1;
            str.AppendFormat("<td class=\"{0}\"><table><tr>", cssClass);

            //visuals
            bool hasVisual = false;
            str.Append("<td valign=\"top\">");

            string pathes = String.Join(",", _visuals.ToArray()).Replace("/outdoor_id_low", string.Empty);
            string encryptedParams = (!string.IsNullOrEmpty(pathes)) ? TNS.AdExpress.Web.Functions.QueryStringEncryption.EncryptQueryString(pathes) : "";

            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    string encryptedParams2 = TNS.AdExpress.Web.Functions.QueryStringEncryption.EncryptQueryString(st);
                    str.AppendFormat("<a href=\"javascript:OpenWindow('" + TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path={0}&id_vehicle=" + _vehicle.DatabaseId.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1');\"><img src=\"" + TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path={1}&id_vehicle=" + _vehicle.DatabaseId.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1\"/></a>", encryptedParams, encryptedParams2);
                    hasVisual = true;
                }
            }

            if (!hasVisual)
            {
                str.AppendFormat("<span>{0}</span>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
            }

            str.Append("</td>");


            //Informations
            str.Append("<td><table>");
            bool hasData = false;

            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                if (canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster)
                {
                    str.Append("<tr>");
                    str.AppendFormat("<td><span>{0}<span></td><td><span>:<span></td>", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
                    str.Append("<td>");
                    _values[i].Parent = this.Parent;
                    value = _values[i].ToString();
                    hasData = false;
                    if (_values[i] != null)
                    {
                        if (!(_values[i] is CellUnit))
                        {
                            if (g.IsContainsSeparator)
                            {
                                values = value.Split(SEPARATOR);
                                foreach (string s in values)
                                {
                                    if (hasData)
                                    {
                                        str.Append("<br/>");
                                    }
                                    hasData = true;
                                    str.AppendFormat("{0}", s);
                                }
                            }
                            else str.AppendFormat("{0}", TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(value, textWrap.NbChar, textWrap.Offset));
                        }
                        else
                        {
                            str.AppendFormat("{0}", value);
                        }
                    }
                    str.Append("</td>");
                    str.Append("</tr>");
                }
            }
            str.Append("</table></td>");
            //end information



            str.Append("</tr></table></td>");

            return str.ToString();
        }
        #endregion
    }
}
