#region Informations
/* Author: D. Mussuma
 * Created On : 21/04/2010
 * Updates :
 *      Date - Author - Description
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork;
using TNS.FrameWork.WebResultUI;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;
using System.Data.SqlTypes;
using TNS.FrameWork.Date;
using System.IO;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Insertions.Russia.Cells
{
    /// <summary>
    /// Internet cell insertion information 
    /// </summary>
    [System.Serializable]
    public class CellInsertionInternetInformation : TNS.AdExpressI.Insertions.Russia.Cells.CellInsertionInformation
    {
        /// <summary>
        /// Flash id
        /// </summary>
        private const string FLASH_ID = "SWF";

        /// <summary>
        /// Save link webtextId
        /// </summary>
        private const Int64 SAVE_LINK_LABEL_ID = 874;
        /// <summary>
        /// Save link Help webtext id
        /// </summary>
        private const Int64 SAVE_LINK_LABEL_HELP_ID = 920;
        /// <summary>
        /// Banner width
        /// </summary>
        protected string _width = "";
        /// <summary>
        /// Banner height
        /// </summary>
        protected string _height = "";

         #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="label">Texte</param>
        public CellInsertionInternetInformation(WebSession session, List<GenericColumnItemInformation> columns, List<string> columnNames, List<Cell> cells, VehicleInformation vehicle)
            : base(session, columns, columnNames, cells, vehicle)
        {                
        }
       
        #endregion

        #region Render
        /// <summary>
        /// Rendu de code HTML avec un style css spécifique
        /// </summary>
        /// <returns>Code HTML</returns>
        public override string Render(string cssClass)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder strInfos = new StringBuilder();
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            if (_newGroup)
                str.Append(RenderSeparator());

            string value;
            string[] values;
            int i = -1;

            #region Informations
            //Informations
            strInfos.Append("<td><table>");
            bool hasData = false;

            foreach (GenericColumnItemInformation g in _columns)
            {
                i++;
                if( g.Id == GenericColumnItemInformation.Columns.bannerWidth)
                    _width = (_values[i] != null) ? _values[i].ToString() : "";
                else if (g.Id == GenericColumnItemInformation.Columns.bannerHeight)
                    _height = (_values[i] != null) ? _values[i].ToString() : "";
                else if (canBeDisplayed(g) && g.Id != GenericColumnItemInformation.Columns.visual
                    && g.Id != GenericColumnItemInformation.Columns.associatedFile
                    && g.Id != GenericColumnItemInformation.Columns.poster
                    )
                {
                    strInfos.Append("<tr>");
                    strInfos.AppendFormat("<td><span>{0}<span></td><td><span>:<span></td>", GestionWeb.GetWebWord(g.WebTextId, _session.SiteLanguage));
                    strInfos.Append("<td>");
                    if (_values[i] != null) _values[i].Parent = this.Parent;
                    value = (_values[i] != null) ? _values[i].ToString() : "";
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
                                        strInfos.Append("<br/>");
                                    }
                                    hasData = true;
                                    strInfos.AppendFormat("{0}", s);
                                }
                            }
                            else strInfos.AppendFormat("{0}", TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(value, textWrap.NbChar, textWrap.Offset));
                        }
                        else
                        {
                            strInfos.AppendFormat("{0}", value);
                        }
                    }
                    strInfos.Append("</td>");
                    strInfos.Append("</tr>");
                }
            }
            strInfos.Append("</table></td>");
            //end information
            #endregion

            str.AppendFormat("<td class=\"{0}\"><table><tr>", cssClass);

            //Render visuals
            str.Append("<td valign=\"top\">");

            if (_visuals ==null || _visuals.Count < 1)
            {
                str.AppendFormat("<span>{0}</span>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
            }
            else
            {
                RenderBanner(str);
            }

            str.Append("</td>");

            //Render Informations 
            str.Append(strInfos.ToString());

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

                if (Path.GetExtension(_visuals[0]).ToUpper() == FLASH_ID)
                {
                    // Flash banner

                    output.Append("\n <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\"");
                    if (!string.IsNullOrEmpty(_width)) output.AppendFormat("\n width=\"{0}\" ", _width);
                    if (!string.IsNullOrEmpty(_height)) output.AppendFormat("\n  height=\"{0}\" ", _height);
                    output.Append("\n >");
                    output.AppendFormat("\n <PARAM name=\"movie\" value=\"{0}\">", _visuals[0]);
                    output.Append("\n <PARAM name=\"play\" value=\"true\">");
                    output.Append("\n <PARAM name=\"quality\" value=\"high\">");
                    output.AppendFormat("\n <EMBED src=\"{0}\" play=\"true\" swliveconnect=\"true\" quality=\"high\" ",
                         _visuals[0]);
                    if (!string.IsNullOrEmpty(_width)) output.AppendFormat("\n width=\"{0}\" ", _width);
                    if (!string.IsNullOrEmpty(_height)) output.AppendFormat("\n height=\"{0}\" ",_height);
                    output.Append("\n >");
                    output.Append("\n </OBJECT>");
                }
                else
                {
                    // Other type of image
                    output.Append("\n <br/><br/>");
                    output.AppendFormat("<img border=0 src=\"{0}\" border=\"0\">",
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
