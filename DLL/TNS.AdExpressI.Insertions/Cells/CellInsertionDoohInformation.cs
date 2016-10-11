using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.Insertions.Cells
{
    /// <summary>
    /// Cellule contenant les informations d'une insertions (DOOH)
    /// </summary>
    [System.Serializable]
    public class CellInsertionDoohInformation : CellInsertionInformation
    {
        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellInsertionDoohInformation(WebSession session, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells)
            : base(session, columns, columnNames, cells)
        {
        }
        #endregion

        #region Render
        public override string RenderString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("[");
            bool hasVisual = false;
            bool first = true;
            string value;
            string[] values;
            int i = -1;
            bool hasData = false;
            string tmpLink = "[],";
            bool isImage = false;

            if (_visuals.Count > 0)
            {
                isImage = _visuals[0].Contains("jpg") ? true : false;
            }

            if (isImage)
            {
                #region Visuals
                string pathes = String.Join(",", _visuals.ToArray()).Replace("/Imagette", string.Empty);
                str.AppendFormat("{0}", pathes);
                foreach (string s in _visuals)
                {

                    string[] tmp = s.Split(',');
                    foreach (string st in tmp)
                    {
                        //if (!first)
                        //    str.AppendFormat(",{0}", st);
                        //else {
                        //    str.AppendFormat("{0}", st);
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

                    //TODO a utiliser lorsque lorsque le chemin ne sera plus en dure depuis l'ancien site (dans index de insertions)
                    //str.Append("[/Content/img/no_visu.jpg],"); 
                }
                #endregion

                str.Append("[],");
            }
            else
            {
                #region Videos
                foreach (string s in _visuals)
                {
                    string[] tmp = s.Split(',');
                    foreach (string st in tmp)
                    {
                        tmpLink = "[" + s + "," + _session.IdSession + "," + VehiclesInformation.Get(AdExpress.Constantes.Classification.DB.Vehicles.names.dooh).DatabaseId + "],";
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

                str.Append(tmpLink);
            }

            str.Append("[");
            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                if (canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster)
                {
                    str.AppendFormat("{0}", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
                    str.Append(":");
                    _values[i].Parent = this.Parent;
                    value = _values[i].ToString();
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
                                    str.Append(";");
                                }
                                hasData = true;
                                str.AppendFormat("{0};", s);
                            }
                        }
                        else
                        {
                            str.AppendFormat("{0};", value);
                        }
                    }
                }
            }
            str.Append("]");


            return str.ToString();
        }
        #endregion
    }
}
