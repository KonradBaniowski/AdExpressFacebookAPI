using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Cellule contenant les informations d'une creation DOOH
    /// </summary>
    [System.Serializable]
    public class CellCreativesDoohInformation : CellCreativesInformation
    {
        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesDoohInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns
            , List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module)
            : base(session, vehicle, columns, columnNames, cells, module)
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesDoohInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns
            , List<string> columnNames, List<Cell> cells, TNS.AdExpress.Domain.Web.Navigation.Module module, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells, module, idColumnsSet) {
        }
        #endregion

        #region Render

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="cssClass">Css class</param>
        /// <returns>HTML Code</returns>
        public override string Render(string cssClass)
        {
            throw new NotImplementedException("Not implemented for this type of cell.");
        }
        #endregion

        #region RenderThumbnails
        /// <summary>
        /// Render
        /// </summary>
        /// <returns>HTML Code</returns>
        public override string RenderThumbnails()
        {
            throw new NotImplementedException("Not implemented for this type of cell.");
        }
        #endregion

        #region RenderPDF
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="withDetail">Width detail</param>
        /// <param name="index">Index</param>
        /// <param name="colorList">Color list</param>
        /// <returns>HTML Code</returns>
        public override string RenderPDF(bool withDetail, Int64 index, List<Color> colorList)
        {
            throw new NotImplementedException("Not implemented for this type of cell.");
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
            bool isImage = false;

            if (_visuals.Count > 0)
            {
                isImage = _visuals[0].Contains("jpg") ? true : false;
            }

            if (isImage)
            {
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
                    //str.Append("[" + GestionWeb.GetWebWord(843, _session.SiteLanguage) + "],");
                    
                    str.Append("[/Content/img/no_visu.jpg],"); 
                }
                #endregion

                str.Append("[],");
            }
            else
            {
                #region Videos
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
            }

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

                                    #region GAD / leFac
                                    string openBaliseA = string.Empty;
                                    string closeBaliseA = string.Empty;

                                    if (_adressId != -1)
                                    {
                                        if (_session.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.Customer.DB.Flag.id.leFac.GetHashCode()))
                                            openBaliseA = string.Format("<span class=\"leFacCreativeLink\" href=\"#leFacModal\" data-toggle=\"modal\" data-lefac=\"[{0}, {1}, {2}]\"> ", _session.IdSession, value, _adressId);
                                        else
                                            openBaliseA = string.Format("<span class=\"gadCreativeLink\" href=\"#gadModal\" data-toggle=\"modal\" data-gad=\"[{0}, {1}, {2}]\"> ", _session.IdSession, value, _adressId);
                                        
                                        closeBaliseA = "</span>";
                                    }
                                    #endregion

                                    tmpStr.AppendFormat("{0}{1}{2}", openBaliseA, s, closeBaliseA);
                                    //tmpStr.AppendFormat("{1};", openBaliseA, s, closeBaliseA);
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
    }
}
