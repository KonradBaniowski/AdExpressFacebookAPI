using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Result
{
    [System.Serializable]
    public class CellExcelCreativesLink : Cell
    {
        protected string _sloganId = "";

        public string SloganId
        {
            get
            {
                return _sloganId;
            }
        }

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public CellExcelCreativesLink()
        {
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
            if (value == null)
                return;
            this._sloganId = Convert.ToString(value);
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
        /// Get hashcode
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// Rendu de code HTML
        /// </summary>
        /// <returns>Code HTML</returns>
        public override string Render()
        {
            throw new NotImplementedException("Not implemented for this type of cell.");
        }

        /// <summary>
        /// Rendu de code HTML avec un style css spécifique
        /// </summary>
        /// <returns>Code HTML</returns>
        public override string Render(string cssClass)
        {
            throw new NotImplementedException("Not implemented for this type of cell.");
        }

        /// <summary>
        /// Rendu de code HTML pour Excel
        /// </summary>
        /// <returns>Code HTML pour Excel</returns>
        public override string RenderExcel(string cssClass)
        {
            throw new NotImplementedException("Not implemented for this type of cell.");
        }

        public override string RenderString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("https://creatives.kantarmedia.com.tr/CreativeTr/" + _sloganId);

            return str.ToString();
        }

        public string RenderString(string sloganEncrypted, string dateEncrypted, string languageEncrypted)
        {
            StringBuilder str = new StringBuilder();

            str.AppendFormat("https://creatives.kantarmedia.com.tr/CreativeTr/{0}/{1}/{2}", sloganEncrypted, dateEncrypted, languageEncrypted);

            return str.ToString();
        }
        #endregion
    }
}
