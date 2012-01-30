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
using TNS.FrameWork.Date;
using System.IO;

namespace TNS.AdExpressI.Insertions.Russia.Cells
{
    

    /// <summary>
    /// Cellule contenant les informations d'une insertions
    /// </summary>
    [System.Serializable]
    public class CellCreativesInternetInformation : CellCreativesInformation
    {

        #region Constantes
        /// <summary>
        /// Chemin du dossier des bannières
        /// </summary>
        public const string VIRTUAL_DIRECTORY = @"\InternetCreatives";
        /// <summary>
        /// Save link webtextId
        /// </summary>
        private const Int64 SAVE_LINK_LABEL_ID = 874;
        /// <summary>
        /// Save link Help webtext id
        /// </summary>
        private const Int64 SAVE_LINK_LABEL_HELP_ID = 920;
        /// <summary>
        /// PNG id
        /// </summary>
        private const string PNG_ID = "PNG";
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
        /// PNG text
        /// </summary>
        private const string PNG_TEXT = "PNG";
     
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
        /// <summary>
        /// Banner width
        /// </summary>
        protected string _width = "";
        /// <summary>
        /// Banner height
        /// </summary>
        protected string _height = "";

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesInternetInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, Module module, string zoomDate, Int64 universId)
            : base(session, vehicle, columns, columnNames, cells, module)
        {
            this._universId = universId;
            this._zoomDate = zoomDate;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellCreativesInternetInformation(WebSession session, VehicleInformation vehicle, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, Module module, string zoomDate, Int64 universId, Int64 idColumnsSet)
            : base(session, vehicle, columns, columnNames, cells, module, idColumnsSet)
        {
            this._universId = universId;
            this._zoomDate = zoomDate;
        }
        #endregion        

        #region Render
        public override string Render(string cssClass)
        {
            StringBuilder str = new StringBuilder();
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
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
                if (_values[i] != null) _values[i].Parent = this.Parent;
                value = (_values[i] != null) ? _values[i].ToString() : "";

                if (g.Id == GenericColumnItemInformation.Columns.bannerWidth)
                    _width = (_values[i] != null) ? _values[i].ToString() : "";
                else if (g.Id == GenericColumnItemInformation.Columns.bannerHeight)
                    _height = (_values[i] != null) ? _values[i].ToString() : "";
                else if (_visibility[i] && canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual 
                    && g.Id != GenericColumnItemInformation.Columns.associatedFile 
                    && g.Id != GenericColumnItemInformation.Columns.poster 
                    && g.Id != GenericColumnItemInformation.Columns.associatedFileMax)
                {

                    StringBuilder tmpStr = new StringBuilder();
                    tmpStr.AppendFormat("<td width=\"1%\"><span>{0}<span></td> ", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
                    tmpStr.Append("<td>: ");
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
                                        tmpStr.Append("<br/>");
                                    }
                                    hasData = true;
                                    tmpStr.AppendFormat("{0}", s);
                                }
                            }
                            else tmpStr.AppendFormat("{0}", TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(value, textWrap.NbChar, textWrap.Offset));
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
            str.Append("<tr><th valign=\"top\">");

            if (_visuals.Count < 1){
                str.AppendFormat("<span>{0}</span>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
            }
            else{
                RenderBanner(str);
            }

            str.Append("</th></tr>");
            #endregion            

            #region Description
            str.Append("<tr><td><p><table>");
            int nbLine = cols.Count;
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

            if (_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INTERNET_ACCESS_FLAG))
            {
                string encryptedParams = "",creativePath ="";          
                if (Path.GetExtension(_visuals[0]).Substring(1).ToUpper() == FLASH_ID)
                {
                    // Flash banner

                     encryptedParams = (!string.IsNullOrEmpty(_visuals[0])) ? TNS.AdExpress.Web.Functions.QueryStringEncryption.EncryptQueryString(_visuals[0]) : "";
                     creativePath = TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + encryptedParams + "&id_vehicle=" + _vehicle.DatabaseId.ToString() + "&is_blur=false&crypt=1&idSession=" + _session.IdSession;

                    output.Append("\n <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\"");
                    if (!string.IsNullOrEmpty(_width)) output.AppendFormat("\n width=\"{0}\" ", _width);
                    if (!string.IsNullOrEmpty(_height)) output.AppendFormat("\n  height=\"{0}\" ", _height);
                    output.Append("\n >");
                    output.AppendFormat("\n <PARAM name=\"movie\" value=\"{0}\">", creativePath);
                    output.Append("\n <PARAM name=\"play\" value=\"true\">");
                    output.Append("\n <PARAM name=\"quality\" value=\"high\">");                   
                    output.AppendFormat("\n <EMBED src=\"{0}\" play=\"true\" swliveconnect=\"true\" quality=\"high\" ",
                        creativePath);
                    if (!string.IsNullOrEmpty(_width)) output.AppendFormat("\n width=\"{0}\" ", _width);
                    if (!string.IsNullOrEmpty(_height)) output.AppendFormat("\n height=\"{0}\" ", _height);
                    output.Append("\n >");
                    output.Append("\n </OBJECT>");
                }
                else
                {
                    // Other type of image
                     encryptedParams = (!string.IsNullOrEmpty(_visuals[0])) ? TNS.AdExpress.Web.Functions.QueryStringEncryption.EncryptQueryString(_visuals[0]) : "";
                     creativePath = TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + encryptedParams + "&id_vehicle=" + _vehicle.DatabaseId.ToString() + "&is_blur=false&crypt=1&idSession=" + _session.IdSession;

                    output.Append("\n <br/><br/>");
                    output.AppendFormat("<img border=0 src=\"{0}\" border=\"0\" ",
                     creativePath);
                    if (!string.IsNullOrEmpty(_width)) output.AppendFormat(" width=\"{0}\" ", _width);
                    if (!string.IsNullOrEmpty(_height)) output.AppendFormat(" height=\"{0}\" ", _height);
                    output.Append(" >");
                }

                if (_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DOWNLOAD_ACCESS_FLAG))
                {
                    if (!string.IsNullOrEmpty(creativePath)) creativePath = creativePath + "&cd=sv";
                    output.AppendFormat("\n<br/><a href={0} class=\"roll06\" title=\"{1}\">{2}</a>",
                        creativePath,
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
