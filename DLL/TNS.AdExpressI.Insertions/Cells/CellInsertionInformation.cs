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
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Insertions.Cells
{
    /// <summary>
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellInsertionInformation : Cell
    {
        protected const string CARRIAGE_RETURN = "<br/>";

        #region Variables
        /// <summary>
        /// List of columns to display
        /// </summary>
        protected List<GenericColumnItemInformation> _columns = null;
        /// <summary>
        /// List of columns names in data container
        /// </summary>
        protected List<string> _columnsName = null;
        /// <summary>
        /// List of values
        /// </summary>
        protected List<Cell> _values = new List<Cell>();
        /// <summary>
        /// List of previous values
        /// </summary>
        protected List<string> _previousValues = new List<string>();
        /// <summary>
        /// List of creations pathes
        /// </summary>
        protected List<string> _visuals = new List<string>();
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session = null;
        /// <summary>
        /// Vehicle
        /// </summary>
        protected VehicleInformation _vehicle = null;

        /// <summary>
        /// if has creative copyright
        /// </summary>
        protected bool _hasCopyright = true;
        #endregion

        #region Properties
        /// <summary>
        /// Get Number Visuals
        /// </summary>
        public Int64 NbVisuals
        {
            get
            {
                if (_visuals != null)
                    return _visuals.Count;
                else
                    return -1;
            }
        }
        /// <summary>
        /// Get Visuals
        /// </summary>
        public List<string> Visuals
        {
            get { return _visuals; }
        }
        /// <summary>
        /// Get / Set if has creative copyright
        /// </summary>
        public bool HasCopyright
        {
            get { return _hasCopyright; }
            set { _hasCopyright = value; }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellInsertionInformation(WebSession session, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells)
        {
            InitCellsValue(session, columns, columnNames, cells);
        }
        #endregion

        #region Implémentation de Cell
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="value">Value</param>        
        [Obsolete("Not Implemented")]
        public override void SetCellValue(object value)
        {
            throw new NotImplementedException("Not implemented for this type of cell.");
        }
        #endregion

        #region Implémentation de ICell (par héritage de Cell)
        /// <summary>
        /// Comparaison les valeurs de deux cellules.
        /// </summary>
        /// <param name="cell">Cellule à comparer</param>
        /// <remarks>Au cas où cell n'est pas comparable, on utilise ToString pour comparer les objets.</remarks>
        /// <returns>1 si objet courant supérieur à cell, -1 si objet courant inférieur à cell et 0 si les deux objets sont égaux</returns>
        public override int CompareTo(object cell)
        {
            return 0;
        }
        /// <summary>
        /// Get label
        /// </summary>
        /// <returns>Label</returns>
        public override string ToString(string format, IFormatProvider fp)
        {
            return string.Empty;
        }
        /// <summary>
        /// Teste l'égalité de deux cellules.
        /// </summary>
        /// <param name="cell">Cellule à comparer</param>
        /// <returns>vrai si les deux cellules sont égales, faux sinon</returns>
        public override bool Equals(object cell)
        {
            if (cell == null)
                return false;

            if (cell.GetType() != this.GetType())
                return false;

            return true;
        }
        /// <summary>
        /// Rendu de code HTML
        /// </summary>
        /// <returns>Code HTML</returns>
        public override string Render()
        {
            return Render(_cssClass);
        }
        /// <summary>
        /// Rendu de code HTML avec un style css spécifique
        /// </summary>
        /// <returns>Code HTML</returns>
        public override string Render(string cssClass)
        {
            StringBuilder str = new StringBuilder();

            if (_newGroup)
                str.Append(RenderSeparator());

            string value;
            string[] values;
            int i = -1;
            str.AppendFormat("<td class=\"{0}\"><table><tr>", cssClass);

            //visuals
            bool hasVisual = false;
            if (_hasCopyright)
            {
                str.Append("<td valign=\"top\">");
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

                str.Append("</td>");
            }
            else
            {
                str.Append("<td valign=\"top\">");
                str.AppendFormat("<span>{0}</span>", GestionWeb.GetWebWord(3015, _session.SiteLanguage));
                str.Append("</td>");
            }

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
                            values = value.Split(',');
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
        /// <summary>
        /// Rendu de code HTML pour Excel
        /// </summary>
        /// <returns>Code HTML pour Excel</returns>
        public override string RenderExcel(string cssClass)
        {
            string str = string.Empty;
            //str = "<td " + ((cssClass.Length > 0) ? " class=\"" + cssClass + "\"" : "") + " align=\"left\">" + Convertion.ToHtmlString(this.ToString()) + "</td>";
            return str;
        }
        #endregion

        #region Get Hashcode
        /// <summary>
        /// Get hashcode
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return 0;
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
                if (cCell is CellUnit)
                {
                    if (g.Id == GenericColumnItemInformation.Columns.numberBoard &&
                        (cValue == null || cValue.Length == 0)) ((CellUnit)cCell).SetCellValue(null);
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

        #region Data rules
        /// <summary>
        /// Apply rules to data to display
        /// </summary>
        /// <returns>True if the data can be displayed, false neither</returns>
        protected virtual bool canBeDisplayed(GenericColumnItemInformation column)
        {

            switch (column.Id)
            {
                case GenericColumnItemInformation.Columns.product:
                    return _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
                case GenericColumnItemInformation.Columns.weight:
                    return _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_POIDS_MARKETING_DIRECT);
                case GenericColumnItemInformation.Columns.slogan:
                    return _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_SLOGAN_ACCESS_FLAG);
                case GenericColumnItemInformation.Columns.volume:
                    return _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_VOLUME_MARKETING_DIRECT);
                default:
                    return true;
            }

        }
        #endregion

        #region InitCellsValue

        /// <summary>
        /// InitCellsValue
        /// </summary>
        protected virtual void InitCellsValue(WebSession session, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells)
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
                    _values.Add(((CellUnit)c).Clone(0.0));
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

        #region SplitStringValue
        /// <summary>
        /// Split string value
        /// </summary>
        /// <param name="s">string</param>
        /// <returns></returns>
        protected virtual string SplitStringValue(string s, char separator)
        {
            string[] sArr = s.Split(separator);
            string stringValue = string.Empty;
            for (int z = 0; z < sArr.Length; z++)
            {
                stringValue += sArr[z].ToString();
                if (sArr.Length > 1 && z < sArr.Length - 1) stringValue += CARRIAGE_RETURN;
            }
            s = stringValue;
            return s;
        }
        #endregion

    }

}
