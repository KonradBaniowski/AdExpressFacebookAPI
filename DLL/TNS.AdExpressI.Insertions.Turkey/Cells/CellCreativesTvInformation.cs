using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.Insertions.Turkey.Cells
{
    public class CellCreativesTvInformation : Insertions.Cells.CellCreativesTvInformation
    {
        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesTvInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns
            , List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module)
            : base(session, vehicle, columns, columnNames, cells, module)
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesTvInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns
            , List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells, module, idColumnsSet)
        {
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
                    tmpLink = "[" + s + "," + _session.IdSession + "," + _vehicle.DatabaseId + "],";
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
                    g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster &&
                    g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
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

                                    #region GAD / leFac
                                    string openBaliseA = string.Empty;
                                    string closeBaliseA = string.Empty;

                                    if (_adressId != -1)
                                    {
                                        if (_session.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.Customer.DB.Flag.id.leFac.GetHashCode()))
                                            openBaliseA = string.Format("<a class=\"txtViolet11Underline\" href=\"javascript:openLeFac('{0}','{1}','{2}');\">", _session.IdSession, value, _adressId);
                                        else
                                            openBaliseA = string.Format("<a class=\"txtViolet11Underline\" href=\"javascript:openGad('{0}','{1}','{2}');\">", _session.IdSession, value, _adressId);
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
            #endregion

            #region Info
            for (int l = 0; l < cols.Count; l++)
            {
                str.Append(cols[l]);
                str.Append(";");
            }
            str.Append("]");
            #endregion

            return str.ToString();
        }
        #endregion
    }
}
