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

using System;
using System.Globalization;
using System.Threading;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Level;
using System.Collections.Generic;
using TNS.FrameWork.WebResultUI;
using System.Data;
using System.Text;

namespace TNS.AdExpressI.Insertions.Cells
{
    /// <summary>
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellInsertionInformation : Cell
    {

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
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellInsertionInformation(List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells)
        {
            _columns = columns;
            _columnsName = columnNames;
            for (int i = 0; i < columnNames.Count; i++) {
                _previousValues.Add(string.Empty);
            }
            _values = cells;
            for(int i = 0; i < _values.Count; i++) {
                _values[i].StringFormat = _columns[i].StringFormat;
            }
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
            return string.Empty ;
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

            string value;
            string[] values;
            int i = -1;
            str.Append("<table>");

            foreach (GenericColumnItemInformation g in _columns) {
                i++;
                value = _values[i].ToString();
                values = value.Split('¤');
            }
            str.Append("</table>");
            //if (_newGroup)
            //    str += RenderSeparator();
            //str += "<td " + ((cssClass.Length > 0) ? " class=\"" + cssClass + "\"" : "") + " align=\"left\">" + Convertion.ToHtmlString(this.ToString()) + "</td>";
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
        public void Add(DataRow row) {

            int i = -1;
            string cValue;
            Cell cCell;

            foreach (GenericColumnItemInformation g in _columns) {

                i++;
                cValue = row[_columnsName[i]].ToString();
                if (cValue != _previousValues[i]) {
                    cCell = _values[i];
                    if (cCell is CellUnit) {
                        ((CellUnit)cCell).Add(Convert.ToDouble(cValue));
                    }
                    else {
                        ((CellLabel)cCell).Label = string.Format("{0}¤{1}",((CellLabel)cCell).Label, cValue);
                    }
                }

            }

        }
        #endregion
    }

}
