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
using CstWeb = TNS.AdExpress.Constantes.Web;

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
using TNS.AdExpress.Web.Core.Utilities;
using System.Reflection;
#endregion

namespace TNS.AdExpressI.Insertions.Cells
{
    /// <summary>
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellCreativesInformation : CellInsertionInformation
    {

        #region Properties
        /// <summary>
        /// Format container. Used to divide result by the number for a bug purpose
        /// </summary>
        protected string _divideString = string.Empty;
        /// <summary>
        /// Insertion media
        /// </summary>
        protected string _mediaCol = string.Empty;
        /// <summary>
        /// Media Id
        /// </summary>
        protected string _idMedia = string.Empty;
        /// <summary>
        /// List of columns visibility
        /// </summary>
        protected List<bool> _visibility = new List<bool>();
        /// <summary>
        /// List of filterable columns
        /// </summary>
        protected List<bool> _isFilter = new List<bool>();
        /// <summary>
        /// Version Id
        /// </summary>
        protected Int64 _idVersion = -1;
        /// <summary>
        /// Current module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module = null;
        /// <summary>
        /// First version parution
        /// </summary>
        protected Int64 _firstParution = -1;
        /// <summary>
        /// Advertiser address Id
        /// </summary>
        protected Int64 _adressId = -1;
        /// <summary>
        /// Column Indexes
        /// </summary>
        protected Dictionary<GenericColumnItemInformation.Columns, Int32> _columnIndex = new Dictionary<GenericColumnItemInformation.Columns, Int32>();


        #endregion

        #region Accessors
        /// <summary>
        /// Get Vehicle
        /// </summary>
        public VehicleInformation Vehicle
        {
            get { return _vehicle; }
        }
        /// <summary>
        /// Get Column Value
        /// </summary>
        /// <param name="c">Column</param>
        /// <returns>value as a string</returns>
        public string GetValue(GenericColumnItemInformation c)
        {
            return _values[_columnIndex[c.Id]].ToString();
        }

        /// <summary>
        /// Get Vehicle
        /// </summary>
        public Int64 IdVersion
        {
            get { return _idVersion; }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module)
            : base(session, columns, columnNames, cells)
        {
            _vehicle = vehicle;
            _module = module;
            Int64 idColumnsSet = WebApplicationParameters.CreativesDetail.GetDetailColumnsId(vehicle.DatabaseId, module.Id);
            int i = -1;
            foreach (GenericColumnItemInformation g in columns)
            {
                i++;
                _visibility.Add(WebApplicationParameters.GenericColumnsInformation.IsVisible(idColumnsSet, g.Id));
                _isFilter.Add(WebApplicationParameters.GenericColumnsInformation.IsFilter(idColumnsSet, g.Id));
                _columnIndex.Add(g.Id, i);
            }

        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module, Int64 idColumnsSet)
            : base(session, columns, columnNames, cells)
        {
            _vehicle = vehicle;
            _module = module;
            int i = -1;
            foreach (GenericColumnItemInformation g in columns)
            {
                i++;
                _visibility.Add(WebApplicationParameters.GenericColumnsInformation.IsVisible(idColumnsSet, g.Id));
                _isFilter.Add(WebApplicationParameters.GenericColumnsInformation.IsFilter(idColumnsSet, g.Id));
                _columnIndex.Add(g.Id, i);
            }

        }
        #endregion

        #region Add Values
        public virtual void Add(DataRow row, List<string> visuals)
        {

            int i = -1;
            string cValue;
            Cell cCell;
            if (_visuals.Count <= 0)
            {
                foreach (string s in visuals)
                {
                    if (!_visuals.Contains(s))
                    {
                        _visuals.Add(s);
                    }
                }
            }
            foreach (GenericColumnItemInformation g in _columns)
            {

                i++;
                cValue = row[_columnsName[i]].ToString();
                cCell = _values[i];
                if (g.Id == GenericColumnItemInformation.Columns.slogan)
                {
                    _idVersion = Convert.ToInt64(row[g.DataBaseIdField]);
                }
                if (g.Id == GenericColumnItemInformation.Columns.firstDateParution)
                {
                    if (_previousValues[i] == null || _previousValues[i].Length == 0 || Convert.ToInt64(_previousValues[i]) > Convert.ToInt64(cValue))
                    {
                        ((CellDate)cCell).Date = Dates.GetPeriodBeginningDate(cValue, CstWeb.CustomerSessions.Period.Type.dateToDate);
                    }
                }
                else if (g.Id == GenericColumnItemInformation.Columns.addressId)
                {
                    if (row[_columnsName[i]] != System.DBNull.Value)
                    {
                        _adressId = Convert.ToInt64(row[_columnsName[i]]);
                    }
                }
                else if (cCell is CellUnit)
                {
                    if (g.Id == GenericColumnItemInformation.Columns.numberBoardSum && (cValue == null || cValue.Length == 0))
                        ((CellUnit)cCell).SetCellValue(null);
                    else ((CellUnit)cCell).Add(Convert.ToDouble(cValue));
                }
                else
                {
                    if (cValue != _previousValues[i] && cValue.Length > 0)
                    {
                        CellLabel c = ((CellLabel)cCell);
                        c.Label = string.Format("{0}{2}{1}", c.Label, cValue, ((c.Label.Length > 0) ? "," : ""));
                    }
                }
                _previousValues[i] = cValue;

            }

        }
        #endregion

        #region Render

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="cssClass">Css class</param>
        /// <returns>HTML code</returns>
        public override string Render(string cssClass)
        {
            StringBuilder str = new StringBuilder();

            if (_newGroup)
                str.Append(RenderSeparator());

            string value;
            string[] values;
            int i = -1;

            #region Informations
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
            string pathes = String.Join(",", _visuals.ToArray()).Replace("/Imagette", string.Empty);
            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    str.AppendFormat("<a href=\"javascript:openPressCreation('{1}');\"><img class=\"thumbnailDimension\" src=\"{0}\"/></a>", st, pathes);
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
        public virtual string RenderThumbnails()
        {

            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            StringBuilder str = new StringBuilder();

            //Table
            str.Append("<table align=\"left\"  cellpadding=0 cellspacing=0  border=\"0\" width=100% class=\"violetBackGroundV3\">");

            //Render Verion visual
            str.Append("<tr ><td  align=\"left\" class=\"sloganVioletBackGround\" >");

            #region visuals
            str.Append("<tr ><td align=\"left\" class=\"sloganVioletBackGround\" >");
            str.Append("<table align=\"left\" border=0 cellpadding=0  cellspacing=0><tr >");


            string pathes = String.Join(",", _visuals.ToArray()).Replace("/Imagette", string.Empty);
            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    str.Append("<td class=\"sloganVioletBackGround\" >");
                    str.Append("<a href=\"javascript:openPressCreation('" + pathes + "');\">");
                    str.Append("<img border=0 "
                        + ((st.Length > 0) ? " width=\"70px\" height=\"90px\" src=\"" + st + "\"" : "src=\"/App_Themes/" + themeName + "/images/common/detailSpot_down.gif\"")
                        + ">");
                    str.Append("</a>");
                    str.Append("</td>");
                }
            }


            str.Append("</tr></table>");
            str.Append("</td></tr>");
            #endregion

            str.Append("</td></tr>");

            //Render version nb cell
            str.Append("<tr align=\"left\"><td align=\"left\" nowrap=\"nowrap\" " +
                ((_session.SloganColors[_idVersion].ToString().Length > 0) ? "class=\"" + _session.SloganColors[_idVersion] + "\">" : "class=\"sloganVioletBackGround\">"));

            TNS.AdExpress.Domain.Layers.CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.creativesUtilities];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the creatives utilities class"));
            var creativesUtilities = (Creatives)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);

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
        /// <returns>HTML Code</returns>
        public virtual string RenderPDF(bool withDetail, Int64 index)
        {

            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            StringBuilder str = new StringBuilder();
            string value;
            string[] values;
            int i = -1;
            bool hasData = false;
            List<string> cols = new List<string>();

            //Table
            str.Append("<table cellpadding=0 cellspacing=0 width=100% border=\"0\" class=\"backGroundWhite\">");
            //Render Verion visual
            str.Append("<tr>");
            str.Append("<TD>");
            str.Append("<table cellpadding=0 cellspacing=0 border=\"0\">");
            str.Append("<tr>");
            str.Append("<td align=\"left\">");
            RenderImage(str, index);
            str.Append("</td>");


            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                _values[i].Parent = this.Parent;
                value = _values[i].ToString();

                if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster && g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

                    StringBuilder tmpStr = new StringBuilder();
                    tmpStr.AppendFormat("<td nowrap>&nbsp;{0}</td> ", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
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

            if (withDetail)
            {

                str.Append("<td valign=\"top\"><TABLE width=\"300\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">");
                for (int l = 0; l < cols.Count; l++)
                {
                    str.Append("<tr valign=\"top\">" + cols[l] + "</tr>");
                }
                str.Append("</TABLE></td>");
            }

            str.Append("</tr>");

            str.Append("<tr><td nowrap " +
                ((_session.SloganColors[_idVersion].ToString().Length > 0) ? "class=\"" + _session.SloganColors[_idVersion].ToString().Replace("c", "m") + "\">" : "\">"));
            str.Append("<font size=1>");
            str.Append("&nbsp;" + _idVersion);
            str.Append("</font></td>");
            str.Append("</tr>");

            //End table
            str.Append("</table>");
            str.Append("</td>");
            str.Append("</tr>");
            str.Append("</table>");
            str.Append("</td>");

            return str.ToString();

        }

        /// <summary>
        /// Render
        /// </summary>
        /// <param name="withDetail">Width detail</param>
        /// <param name="index">Index</param>
        /// <param name="colorList">Color list</param>
        /// <returns>HTML Code</returns>
        public virtual string RenderPDF(bool withDetail, Int64 index, List<Color> colorList) { return ""; }
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

            #region visuals
            bool hasVisual = false;
            bool first = true;

            string pathes = String.Join(",", _visuals.ToArray()).Replace("/Imagette", string.Empty);
            str.AppendFormat("{0}", pathes);
            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    //if (!first)
                    //    str.AppendFormat(",{0}", st, pathes);
                    //else {
                    //    str.AppendFormat("{0}", st, pathes);
                    //    first = false;
                    //}
                    hasVisual = true;
                }
            }
            str.Append("],");

            if (!hasVisual)
            {
                str.Clear();
                str.Append("[" + GestionWeb.GetWebWord(843, _session.SiteLanguage) + "],");

                //TODO a utiliser lorsque lorsque le chemin ne sera plus en dure depuis l'ancien site (dans index de creative)
                //str.Append("[/Content/img/no_visu.jpg],"); 
            }
            #endregion
            str.Append("[],");

            #region Informations
            str.Append("[");
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
                                        openBaliseA = string.Format("<span class=\"gadCreativeLink\" href=\"#gadModal\" data-toggle=\"modal\" data-gad=\"[{0}, {1}, {2}]\"> ", _session.IdSession, value, _adressId);
                                        closeBaliseA = "</span>;";
                                    }
                                    #endregion

                                    tmpStr.AppendFormat("{0}{1}{2}", openBaliseA, s, closeBaliseA);
                                    //tmpStr.AppendFormat("{1};", openBaliseA, s, closeBaliseA);
                                }
                                else
                                {
                                    tmpStr.AppendFormat("{0};", s);
                                }
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

            str.Append("]");
            #endregion

            return str.ToString();
        }
        #endregion

        #endregion

        #region Private Methods
        /// <summary>
        /// Image Render
        /// </summary>
        /// <param name="output">Output</param>
        /// <param name="index">Index</param>
        internal void RenderImage(StringBuilder output, Int64 index)
        {

            Int64 lastIndex = index + 5;
            Int64 end = 0;
            Int64 nbVisuals = _visuals.Count;

            if (nbVisuals < lastIndex)
                end = nbVisuals;
            else
                end = lastIndex;

            if ((nbVisuals % 5) == 0)
            {
                if (nbVisuals > lastIndex)
                {
                    end = lastIndex;
                }
                else if (nbVisuals == lastIndex)
                {
                    end = lastIndex - 1;
                }
                else
                {
                    end = nbVisuals;
                    index--;
                }
            }

            output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" class=\"backGroundWhite\">");
            output.Append("<tr>");

            for (Int64 i = index; i < end; i++)
            {
                if ((end - index) == 1)
                    output.Append("<td width=\"221px\" height=\"300px\" style=\" BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid; BORDER-RIGHT: white 1px solid; BORDER-LEFT: white 1px solid; \">&nbsp;");
                else
                    output.Append("<td width=\"223px\" height=\"300px\" style=\" BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid; BORDER-RIGHT: white 1px solid; BORDER-LEFT: white 1px solid; \">&nbsp;");
                output.Append("</td>");
            }

            output.Append("</tr>");
            output.Append("</table>");
        }
        #endregion

    }

}
