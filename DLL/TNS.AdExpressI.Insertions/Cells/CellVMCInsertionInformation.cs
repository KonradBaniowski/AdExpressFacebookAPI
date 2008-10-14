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

namespace TNS.AdExpressI.Insertions.Cells
{
    /// <summary>
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellVMCInsertionInformation : CellInsertionInformation
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
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellVMCInsertionInformation(WebSession session, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells)
            : base(session, columns, columnNames, cells)
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
    }

}
