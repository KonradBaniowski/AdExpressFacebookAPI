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
    public class CellCreativesRadioInformation : CellCreativesInformation
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

        #region Render

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="cssClass">Css classe</param>
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
                if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual &&
                    g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster
                    && g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

                    var tmpStr = new StringBuilder();
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
                    SetOpenDownloadScript(str, s);
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

        protected virtual void SetOpenDownloadScript(StringBuilder str, string s)
        {
            str.AppendFormat("<a href=\"javascript:openDownload('{0},{1}','{2}','{3}');\"><div class=\"audioFileBackGround\"></div></a>"
                , s, _idVersion, _session.IdSession, _vehicle.DatabaseId);
        }

        #endregion

        #region RenderThumbnails
        /// <summary>
        /// Render
        /// </summary>
        /// <returns>HTML Code</returns>
        public override string RenderThumbnails()
        {

            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            var str = new StringBuilder();

            //Table
            str.Append("<table align=\"left\"  cellpadding=0 cellspacing=0  border=\"0\" width=100% class=\"violetBackGroundV3\">");

            //Render Verion visual
            str.Append("<tr ><td  height=\"40px\" width=\"100%\" align=\"center\" class=\"sloganVioletBackGround\" >");

            if (_visuals.Count > 0)
            {
                SetOpenDownloadScript(str, _visuals[0]);
                str.AppendFormat("<img border=0 src=\"/App_Themes/{0}/Images/common/Picto_Radio.gif\">", themeName);
                str.Append("</a>");
            }
            else
            {
                str.Append("<span class=\"noVisuDetailVersion\">" + GestionWeb.GetWebWord(843, _session.SiteLanguage) + "</span>");
            }

            str.Append("</td></tr>");

            //Render version nb cell
            str.AppendFormat("<tr align=\"left\"><td align=\"left\" nowrap=\"nowrap\" {0}"
                , ((_session.SloganColors[_idVersion].ToString().Length > 0) ? string.Format("class=\"{0}\">", _session.SloganColors[_idVersion]) : "class=\"sloganVioletBackGround\">"));

            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.creativesUtilities];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the creatives utilities class"));
            var creativesUtilities = (TNS.AdExpress.Web.Core.Utilities.Creatives)AppDomain.CurrentDomain.
                CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false
                , BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);

            if (!creativesUtilities.IsSloganZoom(_session.SloganIdZoom))
            {
                str.AppendFormat("<a href=\"javascript:get_version('{0}');\" onmouseover=\"res_{0}.src='/App_Themes/{1}/Images/Common/button/result2_down.gif';\" onmouseout=\"res_{0}.src ='/App_Themes/{1}/Images/Common/button/result2_up.gif';\">"
                    , _idVersion, themeName);
                str.AppendFormat("<img name=\"res_{0}\" border=0  align=\"left\" src=\"/App_Themes/{1}/Images/Common/button/result2_up.gif\">", _idVersion, themeName);
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
        public override string RenderPDF(bool withDetail, Int64 index, List<Color> colorList)
        {

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
            var cols = new List<string>();
            string style1 = string.Empty;
            string style2 = string.Empty;
            bool first = true;

            bool hasData = false;
            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                _values[i].Parent = this.Parent;
                value = _values[i].ToString();
                if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual &&
                    g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster
                    && g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

                    var tmpStr = new StringBuilder();

                    if (first)
                    {
                        style1 = string.Format("BORDER-RIGHT: {0} 1px solid; BORDER-LEFT: {1} 1px solid", baseColor, color);
                        style2 = string.Format("BORDER-RIGHT: {0} 1px solid", color);
                        first = false;
                    }
                    else if (i == _columns.Count - 1)
                    {
                        style1 = string.Format("BORDER-RIGHT: {0} 1px solid; BORDER-BOTTOM: {1} 1px solid; BORDER-TOP: {0} 1px solid; BORDER-LEFT: {1} 1px solid", baseColor, color);
                        style2 = string.Format("BORDER-TOP: {0} 1px solid; BORDER-BOTTOM: {1} 1px solid; BORDER-RIGHT: {1} 1px solid", baseColor, color);
                    }
                    else
                    {
                        style1 = string.Format("BORDER-RIGHT: {0} 1px solid; BORDER-TOP: {0} 1px solid; BORDER-LEFT: {1} 1px solid", baseColor, color);
                        style2 = string.Format("BORDER-TOP: {0} 1px solid; BORDER-RIGHT: {1} 1px solid", baseColor, color);
                    }

                    tmpStr.AppendFormat("<td width=\"150\" class=\"backGroundWhite\"  style=\"{1}\"  nowrap><span>{0} <span></td> "
                        , GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage), style1);
                    tmpStr.AppendFormat("<td width=\"150\" class=\"backGroundWhite\"  style=\"{0}\"> ", style2);
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

            int nbLine = cols.Count;

            str.Append("<tr><td valign=\"top\"><TABLE width=\"320\" cellSpacing=\"0\" class=\"txtViolet11Bold\" border=\"0\" valign=\"top\">");
            str.AppendFormat("<tr><td nowrap colSpan=2 style=\"BORDER-RIGHT: {0} 1px solid; BORDER-LEFT: {0} 1px solid; BORDER-TOP: {0} 1px solid; BORDER-BOTTOM: {0} 1px solid\" {1}"
                , color, ((_session.SloganColors[_idVersion].ToString().Length > 0) ? string.Format("class=\"{0}\" style=\"BORDER-RIGHT: {1} 1px solid; BORDER-TOP: {1} 1px solid; BORDER-LEFT: {1} 1px solid; BORDER-BOTTOM: {1} 1px solid\">"
                , _session.SloganColors[_idVersion].ToString().Replace("c", "m"), color) : string.Format("\" style=\"BORDER-RIGHT: {0} 1px solid; BORDER-TOP: {0} 1px solid; BORDER-LEFT: {0} 1px solid; BORDER-BOTTOM: {0} 1px solid\">", color)));
            str.Append("<font size=1>");
            str.Append("&nbsp;" + _idVersion);
            str.Append("</font>");
            str.Append("</td></tr>");

            for (int l = 0; l < nbLine; l++)
            {
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

        #region RenderString
        public override string RenderString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[");

            if (_newGroup)
                str.Append("-");

            string value;
            string[] values;
            int i = -1;
            string tmpLink = "[],";


            #region visuals
            bool hasVisual = false;
            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    //SetOpenDownloadScript(str, s);
                    //str.Append("App_Themes/KMAE-Fr/Images/Common/audioFile.gif");
                    tmpLink = "[" + _idVersion + "," + _session.IdSession + "," + _vehicle.DatabaseId + "],";
                    hasVisual = true;
                }
            }
            str.Append("],");

            if (!hasVisual)
            {
                str.Clear();
                //str.Append("[" + GestionWeb.GetWebWord(843, _session.SiteLanguage) + "],");

                str.Append("[/Content/img/no_visu.jpg],"); 
            }
            #endregion

            str.Append(tmpLink);

            #region Init Informations
            str.Append("[");
            List<string> cols = new List<string>();
            bool hasData = false;

            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                _values[i].Parent = this.Parent;
                value = _values[i].ToString();
                if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual &&
                    g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster
                    && g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

                    var tmpStr = new StringBuilder();
                    tmpStr.AppendFormat("{0}", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
                    tmpStr.Append(":");
                    hasData = false;
                    if (_values[i] != null)
                    {
                        if (!(_values[i] is CellUnit))
                        {
                            values = value.Split(';');
                            foreach (string s in values)
                            {
                                if (hasData)
                                {
                                    tmpStr.Append(";");
                                }
                                hasData = true;
                                if (g.Id == GenericColumnItemInformation.Columns.advertiser)
                                {

                                    #region GAD
                                    string openBaliseA = string.Empty;
                                    string closeBaliseA = string.Empty;

                                    if (_adressId != -1)
                                    {
                                        openBaliseA = string.Format("<a class=\"txtViolet11Underline\" href=\"javascript:openGad('{0}','{1}','{2}')\">", _session.IdSession, value, _adressId);
                                        closeBaliseA = "</a>";
                                    }
                                    #endregion

                                    //tmpStr.AppendFormat("{0}{1}{2}", openBaliseA, s, closeBaliseA);
                                    tmpStr.AppendFormat("{1}", openBaliseA, s, closeBaliseA);
                                }
                                else
                                {
                                    tmpStr.AppendFormat("{0}", s);
                                }
                                tmpStr.Append(";");
                            }
                        }
                        else
                        {
                            tmpStr.AppendFormat("{0};", value);
                        }
                    }
                    cols.Add(tmpStr.ToString());
                }
            }

            #region Info
            int nbLine = (int)Math.Ceiling(((double)cols.Count) / 2.0);
            for (int l = 0; l < nbLine; l++)
            {
                str.Append(cols[l]);
                if (l + nbLine < cols.Count)
                {
                    str.Append(cols[l + nbLine]);
                }
                str.Append(";");
            }
            #endregion

            str.Append("]");
            #endregion

            return str.ToString();

        }
        #endregion

        #endregion



    }

}
