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
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellInsertionInformation : TNS.AdExpressI.Insertions.Cells.CellInsertionInformation
    {
        #region Constantes
        protected const char SEPARATOR = '°';
        protected const string CARRIAGE_RETURN = "<br/>";
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellInsertionInformation(WebSession session, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, VehicleInformation vehicle)  
            : base(session,columns,columnNames,cells)
        {
            _vehicle = vehicle;  
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

            string pathes = String.Join(",", _visuals.ToArray()).Replace("/press_low", string.Empty);
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
                cValue = row[_columnsName[i]].ToString();
                cCell = _values[i];
                if (cCell is CellUnit)
                {
                    ((CellUnit)cCell).Add(Convert.ToDouble(cValue));
                }
                else if ((cCell is CellDate) || g.Id == GenericColumnItemInformation.Columns.firstIssueDate
                || g.Id == GenericColumnItemInformation.Columns.dateMediaNum)
                {

                    ((CellDate)cCell).Date = DateString.YYYYMMDDToDateTime(row[_columnsName[i]].ToString());
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
                else
                {
                    if (cValue.Length > 0)
                    {
                        CellLabel c = ((CellLabel)cCell);
                        c.Label = cValue;
                    }
                }
                _previousValues[i] = cValue;
            }

        }
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

      
    }
}
