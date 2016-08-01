﻿#region Using
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
using TNS.FrameWork.Date;
using WebCore = TNS.AdExpress.Web.Core;
#endregion

namespace TNS.AdExpressI.Insertions.Russia.Cells
{

    /// <summary>
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellCreativesInformation : TNS.AdExpressI.Insertions.Cells.CellCreativesInformation
    {
        protected const char SEPARATOR = '°';

        /// <summary>
        /// Thumbnails Directory
        /// </summary>
        private string _thumbnailsDirectory = "/press_low";

        /// <summary>
        /// Set ThumbnailsDirectory
        /// </summary>
        public string ThumbnailsDirectory
        {
            set { _thumbnailsDirectory = value; }
        }

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        public CellCreativesInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module)
            : base(session, vehicle,columns, columnNames, cells,module)
        {            
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public CellCreativesInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells,module,idColumnsSet)
        {            
        }

     

        #endregion

        #region Add Values
        public override void Add(DataRow row, List<string> visuals)
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
                cValue = (row[_columnsName[i]] != null && row[_columnsName[i]] != System.DBNull.Value) ? row[_columnsName[i]].ToString() : string.Empty;
                cCell = _values[i];
                if (g.Id == GenericColumnItemInformation.Columns.slogan)
                {
                    _idVersion = Convert.ToInt64(row[g.DataBaseIdField]);
                }                
                if ((cCell is CellDate)
                    || g.Id == GenericColumnItemInformation.Columns.firstIssueDate
                    || g.Id == GenericColumnItemInformation.Columns.dateMediaNum)
                {
                    ((CellDate)cCell).Date = DateString.YYYYMMDDToDateTime(row[_columnsName[i]].ToString());
                    ((CellDate)cCell).StringFormat = string.Format("{{0:{0}}}", WebApplicationParameters.GenericColumnItemsInformation.Get(GenericColumnItemInformation.Columns.dateMediaNum.GetHashCode()).StringFormat);
                }
                else if (g.Id == GenericColumnItemInformation.Columns.breakFlightStart
                        || g.Id == GenericColumnItemInformation.Columns.programmeFlightStart
                        || g.Id == GenericColumnItemInformation.Columns.timestart)
                {
                    if (cValue.Length > 0)
                    {
                        string strTime = cValue;
                        string seconde = (strTime.Length == 6) ? strTime.Substring(4, 2) : strTime.Substring(3, 2);
                        string minute = (strTime.Length == 6) ? strTime.Substring(2, 2) : strTime.Substring(1, 2);
                        string hour = (strTime.Length == 6) ? strTime.Substring(0, 2) : "0" + strTime.Substring(0, 1);
                        strTime = hour + ":" + minute + ":" + seconde;
                        CellLabel c = ((CellLabel)cCell);
                        c.Label = strTime;
                    }

                }
                else if (g.Id == GenericColumnItemInformation.Columns.clipExpectedDuration)
                {
                    if (row[_columnsName[i]] != null && row[_columnsName[i]] != System.DBNull.Value)
                        ((CellDuration)cCell).Value = Convert.ToDouble(row[_columnsName[i]]);
                    else _values[i] = new CellDuration(null);
                }
                else if (cCell is CellUnit)
                {
                    ((CellUnit)cCell).Add(Convert.ToDouble(cValue));
                }
                else
                {                   
                        CellLabel c = ((CellLabel)cCell);
                        c.Label = cValue;                   
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
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
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

                if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

                    StringBuilder tmpStr = new StringBuilder();
                    tmpStr.AppendFormat("<td width=\"1%\"><span>{0}<span></td> ", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
                    tmpStr.Append("<td>: ");
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
                                        tmpStr.Append("<br/>");
                                    }
                                    hasData = true;
                                    tmpStr.AppendFormat("{0}", s);
                                }
                            }
                            else tmpStr.AppendFormat("{0}", TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(value, textWrap.NbChar, textWrap.Offset));
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

            string pathes = String.Join(",", _visuals.ToArray()).Replace(_thumbnailsDirectory, string.Empty);
            string encryptedParams = (!string.IsNullOrEmpty(pathes)) ? WebCore.Utilities.QueryStringEncryption.EncryptQueryString(pathes) : "";

            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');

                foreach (string st in tmp)
                {
                    string encryptedParams2 = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(st);
                    str.AppendFormat("<a href=\"javascript:OpenWindow('" + TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path={0}&id_vehicle=" + _vehicle.DatabaseId.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1');\"><img src=\"" + TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path={1}&id_vehicle=" + _vehicle.DatabaseId.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1\"/></a>", encryptedParams, encryptedParams2);
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
        public override string RenderThumbnails()
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

            string pathes = String.Join(",", _visuals.ToArray()).Replace(_thumbnailsDirectory, string.Empty);
            string encryptedParams = (!string.IsNullOrEmpty(pathes)) ? WebCore.Utilities.QueryStringEncryption.EncryptQueryString(pathes) : "";

            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    str.Append("<td class=\"sloganVioletBackGround\" >");
                    string encryptedParams2 = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(st);
                    str.Append("<a href=\"javascript:OpenWindow('" + TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + encryptedParams + "&id_vehicle=" + _vehicle.DatabaseId.ToString() + "&is_blur=false&crypt=1&idSession=" + _session.IdSession + "');\">");
                    str.Append("<img border=0 "
                        + ((st.Length > 0) ? " width=\"70px\" height=\"90px\" src=\"" + TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + encryptedParams2 + "&id_vehicle=" + _vehicle.DatabaseId.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1\"" : "src=\"/App_Themes/" + themeName + "/images/common/detailSpot_down.gif\"")
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

            if (!(_session.SloganIdZoom>long.MinValue))
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

        #endregion

        #region Data rules
        /// <summary>
        /// Apply rules to data to display
        /// </summary>
        /// <returns>True if the data can be displayed, false neither</returns>
        protected override bool canBeDisplayed(GenericColumnItemInformation column)
        {
            switch (column.Id)
            {
                case GenericColumnItemInformation.Columns.product:
                    return _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRODUCT_LEVEL_ACCESS_FLAG);               
                case GenericColumnItemInformation.Columns.slogan:
                    return _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_SLOGAN_ACCESS_FLAG);               
                default:
                    return true;
            }

        }
        #endregion

        #region InitCellsValue
        /// <summary>
        /// InitCellsValue
        /// </summary>
        protected override void InitCellsValue(WebSession session, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells)
        {
            _session = session;
            _columns = columns;
            _columnsName = columnNames;
            for (int i = 0; i < columnNames.Count; i++)
            {
                _previousValues.Add(string.Empty);
            }
            foreach (Cell c in cells)
            {
                if (c is CellUnit)
                {
                    _values.Add(((CellUnit)c).Clone(null));
                }
                else if (c is CellDate)
                {
                    CellDate d = (CellDate)CellDate.GetInstance();
                    d.StringFormat = c.StringFormat;
                    _values.Add(d);
                }
                else
                {
                    _values.Add(new CellLabel(string.Empty));
                }

            }
            for (int i = 0; i < _values.Count; i++)
            {
                _values[i].StringFormat = string.Format("{{0:{0}}}", _columns[i].StringFormat);
            }
        }
        #endregion

        


       
    }
}