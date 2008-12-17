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
using CstFmk = TNS.AdExpress.Constantes.FrameWork;
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
    public class CellCreativesEvaliantInformation : CellCreativesInformation
    {

        #region Constantes
        /// <summary>
        /// Chemin du dossier des bannières
        /// </summary>
        public const string VIRTUAL_DIRECTORY = @"\adnettrackCreatives";
        /// <summary>
        /// Save link webtextId
        /// </summary>
        private const Int64 SAVE_LINK_LABEL_ID = 874;
        /// <summary>
        /// Save link Help webtext id
        /// </summary>
        private const Int64 SAVE_LINK_LABEL_HELP_ID = 920;
        /// <summary>
        /// Jpeg id
        /// </summary>
        private const string JPEG_ID = "JPEG";
        /// <summary>
        /// Gif id
        /// </summary>
        private const string GIF_ID = "GIF";
        /// <summary>
        /// Flash id
        /// </summary>
        private const string FLASH_ID = "SWF";
        /// <summary>
        /// Jpeg text
        /// </summary>
        private const string JPEG_TEXT = "JPEG";
        /// <summary>
        /// Gif text
        /// </summary>
        private const string GIF_TEXT = "GIF";
        /// <summary>
        /// Flash text
        /// </summary>
        private const string FLASH_TEXT = "FLASH";
        /// <summary>
        /// Chemin de la page des plans médias AdNetTrack
        /// </summary>
        private const string MEDIA_SCHEDULE_PATH = "/Private/Results/AdNetTrackMediaSchedule.aspx";
        #endregion

        #region Properties
        Int64 _idAdvertiser = -1;
        Int64 _idProduct = -1;
        Int64 _idVersion = -1;
        string _dimension = string.Empty;
        string _format = string.Empty;
        string _url = string.Empty;
        string _zoomDate = string.Empty;
        Int64 _universId = -1;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesEvaliantInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, Module module, string zoomDate, Int64 universId)
            : base(session, vehicle, columns, columnNames, cells, module)
        {
            this._universId = universId;
            this._zoomDate = zoomDate;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesEvaliantInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, Module module, string zoomDate, Int64 universId, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells, module, idColumnsSet) {
            this._universId = universId;
            this._zoomDate = zoomDate;
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
                if (g.Id == GenericColumnItemInformation.Columns.advertiser)
                {
                    _idAdvertiser = Convert.ToInt64(row[g.DataBaseIdField]);
                }
                if (g.Id == GenericColumnItemInformation.Columns.productAdNetTrack)
                {
                    _idProduct = Convert.ToInt64(row[g.DataBaseIdField]);
                }
                if (g.Id == GenericColumnItemInformation.Columns.bannerHashcode)
                {
                    _idVersion = Convert.ToInt64(row[g.DataBaseField]);
                }
                if (g.Id == GenericColumnItemInformation.Columns.bannerDimension || g.Id == GenericColumnItemInformation.Columns.bannerInternetDimension)
                {
                    _dimension = row[g.DataBaseField].ToString();
                }
                if (g.Id == GenericColumnItemInformation.Columns.bannerFormat || g.Id == GenericColumnItemInformation.Columns.bannerInternetFormat)
                {
                    _format = row[g.DataBaseField].ToString();
                }
                if (g.Id == GenericColumnItemInformation.Columns.bannerUrl)
                {
                    _url = row[g.DataBaseField].ToString();
                }
                if (g.Id == GenericColumnItemInformation.Columns.addressId && row[_columnsName[i]] != System.DBNull.Value)
                {
                    _adressId = Convert.ToInt64(row[_columnsName[i]]);
                }


                cValue = row[_columnsName[i]].ToString();
                cCell = _values[i];
                if (cCell is CellUnit)
                {
                    ((CellUnit)cCell).Add(Convert.ToDouble(cValue));
                }
                else if (cCell is CellDate)
                {
                    ((CellDate)cCell).SetCellValue(row[_columnsName[i]]);
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

        #region Render
        public override string Render(string cssClass)
        {
            StringBuilder str = new StringBuilder();

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
                if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual && g.Id != GenericColumnItemInformation.Columns.associatedFile && g.Id != GenericColumnItemInformation.Columns.poster && g.Id != GenericColumnItemInformation.Columns.dateCoverNum && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

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
                                if (g.Id == GenericColumnItemInformation.Columns.advertiser)
                                {

                                    #region GAD
                                    string openBaliseA = string.Empty;
                                    string closeBaliseA = string.Empty;

                                    if (_adressId != -1)
                                    {
                                        openBaliseA = string.Format("<a class=\"txtViolet11Underline\" href=\"javascript:openGad('{0}','{1}','{2}');\">", _session.IdSession, value, _adressId);
                                        closeBaliseA = "</a>";
                                    }
                                    #endregion

                                    tmpStr.AppendFormat("{0}{1}{2}", openBaliseA, s, closeBaliseA);
                                }
                                else
                                {
                                    tmpStr.AppendFormat("{0}", s);
                                }
                            }
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

            #region Visual
            bool hasVisual = false;
            str.Append("<tr><th valign=\"top\">");
            if (_visuals.Count < 1)
            {
                str.AppendFormat("<span>{0}</span>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
            }
            else
            {
                RenderBanner(str);
            }

            str.Append("</th></tr>");
            #endregion

            #region Links
            //Liens plans médias
            StringBuilder t = new StringBuilder();
            if (_session.CustomerLogin.CustormerFlagAccess(CstFlags.MEDIA_SCHEDULE_ADNETTRACK_ACCESS_FLAG))
            {
                t.AppendFormat("<td width=\"1%\"><span>{0}<span></td><td>: ", GestionWeb.GetWebWord(2156, _session.SiteLanguage));

                t.AppendFormat("<a href=\"#\" onclick=\"javascript:window.open('{0}?idSession={1}&idLevel={2}&id={3}&zoomDate={4}&universId={5}&moduleId={6}&vehicleId={8}', '', 'toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1');\" class=\"roll06\">{7}</a>",
                    MEDIA_SCHEDULE_PATH,
                    _session.IdSession,
                    CstFmk.Results.AdNetTrackMediaSchedule.Type.advertiser.GetHashCode(),
                    _idAdvertiser,
                    _zoomDate,
                    _universId,
                    _module.Id,
					GestionWeb.GetWebWord(857, _session.SiteLanguage),
                    _vehicle.DatabaseId
                );
                if (_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                {
                    t.AppendFormat(" | <a href=\"#\" onclick=\"javascript:window.open('{0}?idSession={1}&idLevel={2}&id={3}&zoomDate={4}&universId={5}&moduleId={6}&vehicleId={8}', '', 'toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1');\" class=\"roll06\">{7}</a>",                  
                        MEDIA_SCHEDULE_PATH,
                        _session.IdSession,
                        CstFmk.Results.AdNetTrackMediaSchedule.Type.product.GetHashCode(),
                        _idProduct,
                        _zoomDate,
                        _universId,
                        _module.Id,
                        GestionWeb.GetWebWord(858, _session.SiteLanguage),
                        _vehicle.DatabaseId
                    );
                }
                t.AppendFormat(" | <a href=\"#\" onclick=\"javascript:window.open('{0}?idSession={1}&idLevel={2}&id={3}&zoomDate={4}&universId={5}&moduleId={6}&vehicleId={8}', '', 'toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1');\" class=\"roll06\">{7}</a>",                  
                    MEDIA_SCHEDULE_PATH,
                    _session.IdSession,
                    CstFmk.Results.AdNetTrackMediaSchedule.Type.visual.GetHashCode(),
                    _idVersion,
                    _zoomDate,
                    _universId,
                    _module.Id,
                    GestionWeb.GetWebWord(1909, _session.SiteLanguage),
                        _vehicle.DatabaseId
                );
                t.Append("</td>");
            }
            cols.Add(t.ToString());
            #endregion

            #region Description
            str.Append("<tr><td><p><table>");
            int nbLine = cols.Count;
            for (int l = 0; l < nbLine; l++)
            {
                str.Append("<tr>");
                str.Append(cols[l]);
                str.Append("<td>&nbsp;</td>");
                if (l+nbLine < cols.Count)
                {
                    str.Append(cols[l+nbLine]);
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

        #region Render Banner
        /// <summary>
        /// Render Banner Creative Zone
        /// </summary>
        /// <param name="output"></param>
        private void RenderBanner(StringBuilder output)
        {


            #region Formatage des longueur et largeur du fichier
            string[] dimensionValue = null;
            string width = "";
            string height = "";
            if (_dimension.Length != 0)
            {
                dimensionValue = _dimension.Split('*');
                width = dimensionValue[0];
                height = dimensionValue[1];
            }
            else throw (new Exception("File does not have dimension"));
            #endregion

            if (_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INTERNET_ACCESS_FLAG))
            {

                if (_format == FLASH_ID)
                {
                    // Flash banner
                    output.AppendFormat("\n <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" width=\"{0}\" height=\"{1}\">",
                        width,
                        height);
                    output.AppendFormat("\n <PARAM name=\"movie\" value=\"{0}\">", _visuals[0]);
                    output.Append("\n <PARAM name=\"play\" value=\"true\">");
                    output.Append("\n <PARAM name=\"quality\" value=\"high\">");
                    output.AppendFormat("\n <EMBED src=\"{0}\" play=\"true\" swliveconnect=\"true\" quality=\"high\" width=\"{1}\" height=\"{2}\">",
                         _visuals[0],
                        width,
                        height);
                    output.Append("\n </OBJECT>");
                }
                else
                {
                    // Other type of media
                    output.Append("\n <br/><br/>");
                    output.AppendFormat("<a href=\"{0}\" target=\"_blank\"><img border=0 src=\"{1}\" border=\"0\"></a>",
                        _url,
                        _visuals[0]);
                }

                if (_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DOWNLOAD_ACCESS_FLAG))
                {
                    output.AppendFormat("\n<br/><a href={0} class=\"roll06\" title=\"{1}\">{2}</a>",
                        _visuals[0],
                        GestionWeb.GetWebWord(SAVE_LINK_LABEL_HELP_ID, _session.SiteLanguage),
                        GestionWeb.GetWebWord(SAVE_LINK_LABEL_ID, _session.SiteLanguage));
                }


            }
            else
            {
                output.AppendFormat("<p valign=\"top\" width=\"240\">{0}</p>", GestionWeb.GetWebWord(2250, _session.SiteLanguage));
            }

        }
        #endregion
    }

}
