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
    public class CellCreativesVMCInformation : CellCreativesInformation
    {


        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesVMCInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, Module module)
            : base(session, vehicle, columns, columnNames, cells, module)
        {
            List<GenericColumnItemInformation> allColumns = WebApplicationParameters.InsertionsDetail.GetDetailColumns(VehiclesInformation.Get(CstDBClassif.Vehicles.names.directMarketing).DatabaseId, _session.CurrentModule);
            foreach (GenericColumnItemInformation g in allColumns)
            {
                switch (g.Id)
                {
                    case GenericColumnItemInformation.Columns.content:
                        //Data Base ID
                        if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0)
                        {
                            _divideString = g.DataBaseAliasIdField.ToUpper();
                        }
                        else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0)
                        {
                            _divideString = g.DataBaseIdField.ToUpper();
                        }
                        //Database Label
                        if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0)
                        {
                            _divideString = g.DataBaseAliasField.ToUpper();
                        }
                        else if (g.DataBaseField != null && g.DataBaseField.Length > 0)
                        {
                            _divideString = g.DataBaseField.ToUpper();
                        }
                        break;
                    case GenericColumnItemInformation.Columns.media:
                        //Data Base ID
                        if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0)
                        {
                            _mediaCol = g.DataBaseAliasIdField.ToUpper();
                        }
                        else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0)
                        {
                            _mediaCol = g.DataBaseIdField.ToUpper();
                        }

                        break;
                    default:
                        break;
                }


            }
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesVMCInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, Module module, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells, module, idColumnsSet) {
            List<GenericColumnItemInformation> allColumns = WebApplicationParameters.InsertionsDetail.GetDetailColumns(VehiclesInformation.Get(CstDBClassif.Vehicles.names.directMarketing).DatabaseId, _session.CurrentModule);
            foreach (GenericColumnItemInformation g in allColumns) {
                switch (g.Id) {
                    case GenericColumnItemInformation.Columns.content:
                        //Data Base ID
                        if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0) {
                            _divideString = g.DataBaseAliasIdField.ToUpper();
                        }
                        else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0) {
                            _divideString = g.DataBaseIdField.ToUpper();
                        }
                        //Database Label
                        if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0) {
                            _divideString = g.DataBaseAliasField.ToUpper();
                        }
                        else if (g.DataBaseField != null && g.DataBaseField.Length > 0) {
                            _divideString = g.DataBaseField.ToUpper();
                        }
                        break;
                    case GenericColumnItemInformation.Columns.media:
                        //Data Base ID
                        if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0) {
                            _mediaCol = g.DataBaseAliasIdField.ToUpper();
                        }
                        else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0) {
                            _mediaCol = g.DataBaseIdField.ToUpper();
                        }

                        break;
                    default:
                        break;
                }


            }
        }
        #endregion

        #region Add Values
        public override void Add(DataRow row, List<string> visuals)
        {

            int i = -1;
            string cValue;
            Cell cCell;
            _idMedia = row[_mediaCol].ToString();
            foreach (string s in visuals)
            {
                if (!_visuals.Contains(s))
                {
                    _visuals.Add(s);
                }
            }
            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                cValue = row[_columnsName[i]].ToString();
                if (cValue.Length > 0)
                {
                    cCell = _values[i];

                    if (cCell is CellUnit)
                    {
                        int divide = 1;
                        if (g.IsSum)
                        {
                            divide = Math.Max(1, row[_divideString].ToString().Split(',').Length);
                        }
                        ((CellUnit)cCell).Add(Convert.ToDouble(cValue) / divide);
                    }
                    else
                    {
                        if (cValue != _previousValues[i])
                        {
                            CellLabel c = ((CellLabel)cCell);
                            switch (g.Id)
                            {
                                case GenericColumnItemInformation.Columns.mailFormat:
                                    if (cValue != CstVMCFormat.FORMAT_ORIGINAL)
                                    {
                                        c.Label = string.Format("{0}{2}{1}", c.Label, GestionWeb.GetWebWord(2240, _session.SiteLanguage), ((c.Label.Length > 0) ? "," : ""));
                                    }
                                    else
                                    {
                                        c.Label = string.Format("{0}{2}{1}", c.Label, GestionWeb.GetWebWord(2241, _session.SiteLanguage), ((c.Label.Length > 0) ? "," : ""));
                                    }
                                    break;
                                default:
                                    c.Label = string.Format("{0}{2}{1}", c.Label, cValue, ((c.Label.Length > 0) ? "," : ""));
                                    break;
                            }

                        }
                    }

                    _previousValues[i] = cValue;
                }
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
            bool b = base.canBeDisplayed(column);
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
                case GenericColumnItemInformation.Columns.itemNb:
                case GenericColumnItemInformation.Columns.content:
                    return _idMedia != CstVMCMedia.PUBLICITE_NON_ADRESSEE;
                case GenericColumnItemInformation.Columns.format:
                case GenericColumnItemInformation.Columns.mailType:
                case GenericColumnItemInformation.Columns.typeDoc:
                    return _idMedia == CstVMCMedia.PUBLICITE_NON_ADRESSEE;
                case GenericColumnItemInformation.Columns.rapidity:
                    return _idMedia == CstVMCMedia.COURRIER_ADRESSE_GESTION;
                case GenericColumnItemInformation.Columns.mailFormat:
                    return _idMedia == CstVMCMedia.COURRIER_ADRESSE_GENERAL;
                default:
                    return true;
            }
            return true;

        }
        #endregion

        #region Render
        public override string Render(string cssClass)
        {
            StringBuilder str = new StringBuilder();

            if (_newGroup)
                str.Append(RenderSeparator());

            string value;
            string versionId = string.Empty;
            string[] values;
            int i = -1;

            str.AppendFormat("<td class=\"{0}\"><table><tr>", cssClass);



            //visuals
            bool hasVisual = false;
            str.Append("<td valign=\"top\">");
            string pathes = String.Join(",", _visuals.ToArray()).Replace("/Imagette", string.Empty);
            foreach (string s in _visuals)
            {
                string[] tmp = s.Split(',');
                foreach (string st in tmp)
                {
                    str.AppendFormat("<a href=\"javascript:openPressCreation('{1}');\"><img src=\"{0}\"/></a>", st, pathes);
                    hasVisual = true;
                }
            }
            if (!hasVisual)
            {
                str.AppendFormat("<span>{0}</span>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
            }

            str.Append("</td></tr>");


            //Informations
            List<string> cols = new List<string>();
            List<string> description = new List<string>();

            bool hasData = false;
            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                _values[i].Parent = this.Parent;
                value = _values[i].ToString();

                if (canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster && g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

                    switch (g.Id)
                    {
                        case GenericColumnItemInformation.Columns.mailFormat:
                            if (value.Length > 0){
                                if (value != CstVMCFormat.FORMAT_ORIGINAL)
                                {
                                    description.Add(string.Format("{0}", GestionWeb.GetWebWord(2240, _session.SiteLanguage)));
                                }
                                else
                                {
                                    description.Add(string.Format("{0}", GestionWeb.GetWebWord(2241, _session.SiteLanguage)));
                                }
                            }
                            break;
                        case GenericColumnItemInformation.Columns.content:
                        case GenericColumnItemInformation.Columns.format:
                        case GenericColumnItemInformation.Columns.mailType:
                        case GenericColumnItemInformation.Columns.typeDoc:
                            if (value.Length > 0){
                                string[] vs = value.Split(',');
                                foreach (string s in vs)
                                {
                                    description.Add(s);
                                }
                            }
                            break;
                        default:
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
                            break;
                    }
                }
            }
            int nbLine = (int)Math.Ceiling(((double)cols.Count+1) / 2.0);
            StringBuilder t = new StringBuilder();
            t.AppendFormat("<td width=\"1%\" rowspan=\"{1}\"><span>{0}<span></td><td rowspan=\"{1}\">", GestionWeb.GetWebWord(2239, _session.SiteLanguage), (1 + nbLine % 2));
            for (int c = 0; c < description.Count; c++)
            {
                t.AppendFormat("{0}  {1}", ((c < 1) ? string.Empty : "<br/>"), description[c]);
            }
            t.Append("</td>");
            cols.Add(t.ToString());

            str.Append("<p><tr><td><table>");
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
                str.Append("<td width=\"100%\"></td>");
                str.Append("</tr>");
            }
            str.Append("</table></td></tr></p>");
            //end information



            str.Append("</tr></table></td>");

            return str.ToString();
        }
        #endregion

    }

}
