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

#region using
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstVMCMedia = TNS.AdExpress.Constantes.DB.Media;
using CstVMCFormat = TNS.AdExpress.Constantes.DB.Format;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;

using System;
using System.Drawing;
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
using System.Reflection;
#endregion

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
        public CellCreativesTvInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module)
            : base(session, vehicle, columns, columnNames, cells, module)
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesTvInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells, module, idColumnsSet) {
        }
        #endregion

        #region Render

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="cssClass">Css class</param>
        /// <returns>HTML Code</returns>
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
                                if (g.Id == GenericColumnItemInformation.Columns.advertiser)
                                {

                                    #region GAD
                                    string openBaliseA = string.Empty;
                                    string closeBaliseA = string.Empty;

                                    if (_adressId != -1)
                                    {
                                        openBaliseA = string.Format("<a class=\"txtViolet11Underline\" href=\"javascript:openGad('{0}','{1}','{2}');\">", _session.IdSession, value, _adressId);
                                        closeBaliseA = "</a>";
                                    }
                                    #endregion

                                    tmpStr.AppendFormat("{0}{1}{2}", openBaliseA, s, closeBaliseA);
                                }
                                else
                                {
                                    tmpStr.AppendFormat("{0}", s);
                                }
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
            str.Append("<tr><th valign=\"top\">");
            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    str.AppendFormat("<a href=\"javascript:openDownload('{0}','{1}','{2}');\"><div class=\"videoFileBackGround\"></div></a>", s, this._session.IdSession, _vehicle.DatabaseId);
                    hasVisual = true;
                }
            }
            if (!hasVisual)
            {
                str.AppendFormat("<span>{0}</span>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
            }

            str.Append("</th></tr>");
            #endregion

            #region Info
            str.Append("<tr><td><p><table>");
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
            str.Append("</table></p></td></tr>");
            #endregion

            str.Append("</tr></table></td>");

            return str.ToString();
        }
        #endregion

        #region RenderThumbnails
        /// <summary>
        /// Render
        /// </summary>
        /// <returns>HTML Code</returns>
        public override string RenderThumbnails() {

            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            StringBuilder str = new StringBuilder();

            //Table
            str.Append("<table align=\"left\"  cellpadding=0 cellspacing=0  border=\"0\" width=100% class=\"violetBackGroundV3\">");

            //Render Verion visual
            str.Append("<tr ><td  height=\"40px\" width=\"100%\" align=\"center\" class=\"sloganVioletBackGround\" >");

            if (_visuals.Count > 0) {
                str.Append("<a href=\"javascript:openDownload('" + _visuals[0] + "','" + this._session.IdSession + "','" + _vehicle.DatabaseId + "');\">");
                str.Append("<img border=0 src=\"/App_Themes/" + themeName + "/Images/common/Picto_pellicule.gif\">");
                str.Append("</a>");
            }
            else {
                str.Append("<span class=\"noVisuDetailVersion\">" + GestionWeb.GetWebWord(843, this._session.SiteLanguage) + "</span>");
            }
            str.Append("</td></tr>");

            //Render version nb cell
            str.Append("<tr align=\"left\"><td align=\"left\" nowrap=\"nowrap\" " +
                ((_session.SloganColors[_idVersion].ToString().Length > 0) ? "class=\"" + _session.SloganColors[_idVersion] + "\">" : "class=\"sloganVioletBackGround\">"));

            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.creativesUtilities];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the creatives utilities class"));
            TNS.AdExpress.Web.Core.Utilities.Creatives creativesUtilities = (TNS.AdExpress.Web.Core.Utilities.Creatives)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);

            if (!creativesUtilities.IsSloganZoom(_session.SloganIdZoom))
            {
                str.Append("<a href=\"javascript:get_version('" + _idVersion + "');\" onmouseover=\"res_" + _idVersion + ".src='/App_Themes/" + themeName + "/Images/Common/button/result2_down.gif';\" onmouseout=\"res_" + _idVersion + ".src ='/App_Themes/" + themeName + "/Images/Common/button/result2_up.gif';\">");
                str.Append("<img name=\"res_" + _idVersion + "\" border=0  align=\"left\" src=\"/App_Themes/" + themeName + "/Images/Common/button/result2_up.gif\">");
                str.Append("</a>");
            }

            str.Append("<div align=\"left\"><font align=\"left\"  size=1>");
            str.Append(" " + _idVersion);
            str.Append("</font></div>");
            str.Append("</td></tr>");

            //End table
            str.Append("</table>");

            return str.ToString();

        }
        #endregion

        #region RenderPDF
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="withDetail">Width detail</param>
        /// <param name="index">Index</param>
        /// <param name="colorList">Color list</param>
        /// <returns>HTML Code</returns>
        public override string RenderPDF(bool withDetail, Int64 index, List<Color> colorList) {

            int colorIndex = int.Parse(_session.SloganColors[_idVersion].ToString().Substring(2, (_session.SloganColors[_idVersion].ToString().Length - 2)));
            string color = colorList[colorIndex].Name.Substring(2, 6);
            string baseColor = colorList[0].Name.Substring(2, 6);
            color = color.Insert(0, "#");
            baseColor = baseColor.Insert(0, "#");
            StringBuilder str = new StringBuilder();
            string value;
            string[] values;
            int i = -1;

            //Table
            str.Append("<table cellpadding=0 cellspacing=0 width=100% border=\"0\" class=\"backGroundWhite\">");
            //Render Verion visual
            str.Append("<tr><td>");
            str.Append("<table cellpadding=0 cellspacing=0 border=\"0\">");
            //output.Append("<tr>");
            str.Append("<tr><td></td></tr>");

            #region Init Informations
            List<string> cols = new List<string>();
            string style1 = string.Empty;
            string style2 = string.Empty;
            bool first = true;

            bool hasData = false;
            foreach (GenericColumnItemInformation g in _columns) {
                i++;
                _values[i].Parent = this.Parent;
                value = _values[i].ToString();
                if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster && g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax) {

                    StringBuilder tmpStr = new StringBuilder();

                    if (first) {
                        style1 = "BORDER-RIGHT: " + baseColor + " 1px solid; BORDER-LEFT: " + color + " 1px solid";
                        style2 = "BORDER-RIGHT: " + color + " 1px solid";
                        first = false;
                    }
                    else if (i == _columns.Count - 1) {
                        style1 = "BORDER-RIGHT: " + baseColor + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid; BORDER-TOP: " + baseColor + " 1px solid; BORDER-LEFT: " + color + " 1px solid";
                        style2 = "BORDER-TOP: " + baseColor + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid; BORDER-RIGHT: " + color + " 1px solid";
                    }
                    else {
                        style1 = "BORDER-RIGHT: " + baseColor + " 1px solid; BORDER-TOP: " + baseColor + " 1px solid; BORDER-LEFT: " + color + " 1px solid";
                        style2 = "BORDER-TOP: " + baseColor + " 1px solid; BORDER-RIGHT: " + color + " 1px solid";
                    }

                    tmpStr.AppendFormat("<td width=\"150\" class=\"backGroundWhite\"  style=\"" + style1 + "\"  nowrap><span>{0} <span></td> ", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
                    tmpStr.Append("<td width=\"150\" class=\"backGroundWhite\"  style=\"" + style2 + "\"> ");
                    hasData = false;
                    if (_values[i] != null) {
                        if (!(_values[i] is CellUnit)) {
                            values = value.Split(',');
                            foreach (string s in values) {
                                if (hasData) {
                                    tmpStr.Append("<br/>");
                                }
                                hasData = true;
                                tmpStr.AppendFormat("{0}", s);
                            }
                        }
                        else {
                            tmpStr.AppendFormat("{0}", value);
                        }
                    }
                    tmpStr.Append("</td>");
                    cols.Add(tmpStr.ToString());
                }
            }
            #endregion

            int nbLine = cols.Count;

            str.Append("<tr><td valign=\"top\"><TABLE width=\"320\" cellSpacing=\"0\" class=\"txtViolet11Bold\" border=\"0\" valign=\"top\">");
            str.Append("<tr><td nowrap colSpan=2 style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-LEFT: " + color +
                              " 1px solid; BORDER-TOP: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\" " +
                             ((_session.SloganColors[_idVersion].ToString().Length > 0) ? "class=\"" + _session.SloganColors[_idVersion].ToString().Replace("c", "m") + "\" style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-TOP: " + color + " 1px solid; BORDER-LEFT: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\">" : "\" style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-TOP: " + color + " 1px solid; BORDER-LEFT: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\">"));
            str.Append("<font size=1>");
            str.Append("&nbsp;" + _idVersion);
            str.Append("</font>");
            str.Append("</td></tr>");

            for (int l = 0; l < nbLine; l++) {
                str.Append("<tr valign=\"top\">" + cols[l] + "</tr>");
            }

            str.Append("</TABLE></td>");
            str.Append("<td class=\"backGroundWhite\" style=\"WIDTH: 40px; BORDER-RIGHT: white 0px solid;BORDER-LEFT: white 1px solid\"><font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</font></td>");

            str.Append("</tr>");
            str.Append("<tr><td class=\"backGroundWhite\" style=\"HEIGHT: 50px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");

            //End table
            str.Append("</table>");
            str.Append("</td>");
            str.Append("</tr>");
            str.Append("</table>");
            str.Append("</td>");

            return str.ToString();
        }
        #endregion

        #endregion

    }

}
