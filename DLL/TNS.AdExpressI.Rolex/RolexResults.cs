using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Rolex.DAL;
using TNS.AdExpressI.Rolex.Exceptions;
using TNS.AdExpressI.Rolex.Style;
using TNS.FrameWork.Date;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctExcel = TNS.AdExpress.Web.UI.ExcelWebPage;

namespace TNS.AdExpressI.Rolex
{
    public class RolexResults : IRolexResults
    {
        #region Constantes

        #region Column Indexes
        /// <summary>
        /// Index of L1 label
        /// </summary>
        public const int L1_COLUMN_INDEX = 0;
        /// <summary>
        /// Index of L2 label
        /// </summary>
        public const int L2_COLUMN_INDEX = 1;
        /// <summary>
        /// Index of L3 label
        /// </summary>
        public const int L3_COLUMN_INDEX = 2;
        /// <summary>
        /// Index of ID level 1 column 
        /// </summary>
        public const int L1_ID_COLUMN_INDEX = 3;
        /// <summary>
        ///Index of ID level 2 column 
        /// </summary>
        public const int L2_ID_COLUMN_INDEX = 4;
        /// <summary>
        /// Index of ID level 3 column 
        /// </summary>
        public const int L3_ID_COLUMN_INDEX = 5;
        #endregion

        #endregion

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(CstWeb.Module.Name.ROLEX);
        /// <summary>
        /// Period beginning date
        /// </summary>
        protected string _periodBeginningDate = string.Empty;
        /// <summary>
        /// Period end date
        /// </summary>
        protected string _periodEndDate = string.Empty;
        /// <summary>
        /// Theme
        /// </summary>
        protected string _theme = string.Empty;
        /// <summary>
        /// Result control ID
        /// </summary>
        protected string _resultControlId = string.Empty;
        #endregion

        #region Session
        /// <summary>
        /// User Session
        /// </summary>
        public WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        #endregion

        #region Rolex Schedule Style
        /// <summary>
        /// MediaSchedule Style
        /// </summary>
        protected RolexScheduleStyle _style = null;
        /// <summary>
        /// Get / Set flag to specify if Rolex Schedule output is PDF
        /// </summary>
        public RolexScheduleStyle RolexScheduleStyle
        {
            get { return _style; }
            set { _style = value; }
        }


        #endregion

        #region Is PDF Report
        /// <summary>
        /// Is it a pdf report
        /// </summary>
        protected bool _isPDFReport = false;
        /// <summary>
        /// Get / Set flag to specify if Rolex Schedule output is PDF
        /// </summary>
        public bool IsPDFReport
        {
            get { return _isPDFReport; }
            set { _isPDFReport = value; }
        }
        #endregion

        #region Is Excel Report
        /// <summary>
        /// Is it a excel report?
        /// </summary>
        protected bool _isExcelReport = false;
        /// <summary>
        /// Get / Set flag to specify if report is an Excel Media Schedule
        /// </summary>
        public bool IsExcelReport
        {
            get { return _isExcelReport; }
            set { _isExcelReport = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session"></param>
        public RolexResults(WebSession session)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;

            SetPeriods();
        }



        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="periodBeginningDate">period Beginning Date</param>
        /// <param name="periodEndDate">period End Date</param>
        public RolexResults(WebSession session, string periodBeginningDate, string periodEndDate)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;
            _periodBeginningDate = periodBeginningDate;
            _periodEndDate = periodEndDate;
        }

        #endregion

        #region Implementation of IRolexResults

        /// <summary>
        /// Define Current Module
        /// </summary>
        public string ResultControlId
        {
            set { _resultControlId = value; }
        }

        /// <summary>
        /// Define Theme
        /// </summary>
        public string Theme
        {
            set { _theme = value; }
        }

        /// <summary>
        /// Get Period Beginning Date 
        /// </summary>
        public string PeriodBeginningDate
        {
            get { return _periodBeginningDate; }
        }

        /// <summary>
        /// Get Period EndDate
        /// </summary>
        public string PeriodEndDate
        {
            get { return _periodEndDate; }
        }

        /// <summary>
        /// Get HTML code for the rolex schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        public string GetHtml()
        {
            try
            {
                _isExcelReport = false;
                _isPDFReport = false;
                _style = new DefaultRolexScheduleStyle();
                var htmlVisibility = new StringBuilder();
                var htmlNoVisibility = new StringBuilder();
                 ComputeDesign(ComputeData, ComputeSitesWithoutVisibility, htmlVisibility, htmlNoVisibility);
                 return string.Format("{0}<br/><br/>{1}", htmlVisibility.ToString(), htmlNoVisibility.ToString());
            }
            catch (Exception ex)
            {
                throw new RolexResultsException("Impossible to obtain Rolex schedule", ex);
            }

        }

        /// <summary>
        /// Get HTML code for an excel export of the rolex schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        public virtual string GetExcelHtml()
        {
            try
            {
                _isExcelReport = true;
                _isPDFReport = false;
                _style = new ExcelRolexScheduleStyle();
                var htmlVisibility = new StringBuilder();
                var htmlNoVisibility = new StringBuilder();
                ComputeDesign(ComputeData, ComputeSitesWithoutVisibility, htmlVisibility, htmlNoVisibility);
                return string.Format("{0}{1}<br/><br/>{2}{3}", GetExcelHeader(), htmlVisibility.ToString(), htmlNoVisibility.ToString(), GetExcelFooter());

            }
            catch (Exception ex)
            {
                throw new RolexResultsException("Impossible to obtain Rolex schedule", ex);
            }
        }
        /// <summary>
        /// Get HTML code for a pdf export of the rolex schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        public virtual string[] GetPDFHtml()
        {

            _isExcelReport = false;
            _isPDFReport = true;
            _style = new PDFRolexScheduleStyle();
             var htmlVisibility = new StringBuilder();
            var htmlNoVisibility = new StringBuilder();
            ComputeDesign(ComputeData, ComputeSitesWithoutVisibility, htmlVisibility, htmlNoVisibility);
            var tab = new string[2];
            tab[0] = htmlVisibility.ToString();
            tab[1] = htmlNoVisibility.ToString();
            return tab;

        }
        /// <summary>
        /// Get HTML code for the rolex file
        /// </summary>
        /// <returns>HTML Code</returns>
        public string GetRolexFileHtml(GenericDetailLevel selectedDetailLevel, List<long> selectedLevelValues, out  List<string> visuals)
        {
            var html = new StringBuilder(1000);

            var arrList = new ArrayList { 53, 76, 72 };
            var detailLevel = new GenericDetailLevel(arrList);

            var param = new object[3];
            param[0] = _session;
            param[1] = _periodBeginningDate;
            param[2] = _periodEndDate;
            var rolexScheduleDAL = (IRolexDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            using (var ds = rolexScheduleDAL.GetFileData(selectedDetailLevel, selectedLevelValues, detailLevel))
            {

                visuals = new List<string>();


                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    html.Append("<table class=\"rolexSchedulFileResult\" >");// cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"650\" height=\"590\"
                    html.Append("<tr> <td   class=\"rolexSchedulFileResult\" valign=\"top\" >");//debut 1 //puour le content

                    html.Append("<div class=\"rolexSchedulFileResult\">");//  debut 2
                    html.Append("<div class=\"rolexScheduleResultFileContent\">");//  debut 3

                    long? oldIdSite = null;
                    long? oldIdPage = null;
                    long? oldIdLocation = null;

                    int m = 0;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        var dr = ds.Tables[0].Rows[i];

                        if (oldIdSite != Convert.ToInt64(dr["ID_SITE"]) || oldIdLocation != Convert.ToInt64(dr["ID_LOCATION"])
                           || oldIdPage != Convert.ToInt64(dr["ID_PAGE"]))
                        {


                            #region Content

                            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" valign=\"top\" width=\"650\" height=\"530\">");

                            #region Header

                            //SITE
                            if (oldIdSite != Convert.ToInt64(dr["ID_SITE"]))
                            {
                                html.Append("<tr><td class=\"RolexFilePopUpHeader\">");
                                html.AppendFormat("&nbsp;&nbsp;{0}", dr["SITE"].ToString());

                                html.Append("</td></tr>");
                            }
                            #endregion

                            //Content
                            html.Append("<tr><td class=\"rofDescr\" valign=\"top\">");
                            html.Append("<ul class=\"rof\">");

                            //URL
                            html.AppendFormat(" <li><span class=\"rofSp\"\"> <a class=\"roSite\" href=\"{1}\" target=\"_blank\" >{1}</a></span>", "&nbsp;", dr["URL"].ToString());

                            //Image export pdf
                            if (oldIdSite != Convert.ToInt64(dr["ID_SITE"]) && i == 0)
                            {
                                html.Append("<img align=\"right\" alt=\"" + GestionWeb.GetWebWord(2865, _session.SiteLanguage) +
                                            "\" ");
                                html.Append("id=\"promofile" + _resultControlId + "\" ");
                                html.AppendFormat("src=\"/App_Themes/{0}/Images/Common/export_pdf.gif\" ", _theme);
                                html.AppendFormat(
                                    "onmouseover=\"javascript:this.src='/App_Themes/{0}/Images/Common/export_pdf_over.gif';\" ",
                                    _theme);
                                html.AppendFormat(
                                    "onmouseout=\"javascript:this.src='/App_Themes/{0}/Images/Common/export_pdf.gif';\" ",
                                    _theme);
                                html.AppendFormat(
                                    "onclick=\"javascript:popupOpenBis('/Private/MyAdExpress/PdfSavePopUp.aspx?idSession={0}&levelsValue={1}&resultType={2}&datebegin={3}&dateend={4}','470','210','yes');\" ",
                                    _session.IdSession, string.Join(",", selectedLevelValues.Select(p => p.ToString()).ToArray()),
                                    TNS.AdExpress.Anubis.Constantes.Result.type.ptah.GetHashCode(), dr["DATE_BEGIN_NUM"].ToString(), dr["DATE_END_NUM"].ToString());
                                html.Append(" />");
                            }

                            html.Append("</li>");

                            //Dates
                            string dateBegin = DateString.YYYYMMDDToDD_MM_YYYY(Convert.ToInt32(dr["DATE_BEGIN_NUM"].ToString()), _session.SiteLanguage);
                            string dateEnd = DateString.YYYYMMDDToDD_MM_YYYY(Convert.ToInt32(dr["DATE_END_NUM"].ToString()), _session.SiteLanguage);
                            html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"rofSp\"> {1} {2} {3} {4}</span></li>", GestionWeb.GetWebWord(895, _session.SiteLanguage),
                                GestionWeb.GetWebWord(896, _session.SiteLanguage), dateBegin, GestionWeb.GetWebWord(897, _session.SiteLanguage), dateEnd);

                            // Location
                            html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"rofSp\"\"> {1}</span></li>", GestionWeb.GetWebWord(1732, _session.SiteLanguage), dr["LOCATION"].ToString());



                            //PRESENCE TYPE               
                            html.AppendFormat(" <li><span><b>{0}:</b></span>", GestionWeb.GetWebWord(2957, _session.SiteLanguage));
                            html.Append(" <ul class=\"rofbd\">");
                            DataRow[] dataRows =
                                ds.Tables[0].Select(" ID_SITE =" + Convert.ToString(dr["ID_SITE"]) + " AND ID_LOCATION=" +
                                                    Convert.ToString(dr["ID_LOCATION"]) + " AND ID_PAGE=" +
                                                    Convert.ToString(dr["ID_PAGE"]));
                            foreach (DataRow dataRow in dataRows)
                            {
                                html.AppendFormat("<li><span class=\"rofSp\">{0}</span> </li>", dataRow["PRESENCE_TYPE"].ToString());
                            }
                            html.Append(" </ul>");
                            html.Append(" </li>");

                            // Commentary
                            if (dr["COMMENTARY"] != DBNull.Value)
                                html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"rofSp\"\"> {1}</span>", GestionWeb.GetWebWord(74, _session.SiteLanguage), dr["COMMENTARY"].ToString());

                            html.Append("</ul></td></tr>");

                            //Space                      
                            html.Append("<tr><td valign=\"top\">");
                            html.Append("&nbsp;");
                            html.Append("</td></tr>");


                            #region Image(s)
                            if (dr["VISUAL"] != DBNull.Value && dr["VISUAL"].ToString().Length > 0)
                            {
                                //Button arrow left
                                html.Append("<tr><td>");
                                html.Append("  <table cellpadding=\"0\" border=\"0\" align=\"center\">");
                                html.Append(" <tr><td id=\"previous\">");
                                html.AppendFormat(" <img id=\"imgPrevious_" + m + "_" + _resultControlId + "\" style=\"visibility: hidden; margin-left: 5px; cursor: pointer; margin-right: 5px; vertical-align: middle;\" onmouseover=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_left_down.gif'\" onmouseout=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_left.gif'\" onclick=\"javascript:DisplayPics_"
                                    + _resultControlId + "(-1,this,document.getElementById('imgNext_" + m + "_" + _resultControlId + "'),document.getElementById('img_ro_" + m + "_" + _resultControlId + "'),'" + dr["VISUAL"].ToString() + "');\" src=\"/App_Themes/{0}/Images/Common/Button/arrow_left.gif\" />", _theme);
                                html.Append("</td>");

                                //Show Visual                           
                                string visibility = "hidden";
                                List<string> temsVisuals = new List<string>(dr["VISUAL"].ToString().Split(','));
                                if (temsVisuals.Count > 1) visibility = "visible";
                                for (int k = 0; k < temsVisuals.Count; k++)
                                {
                                    string currentVisual = string.Format("{0}/{1}?p={2}", CstWeb.CreationServerPathes.IMAGES_ROLEX, temsVisuals[k].Trim(), k);
                                    visuals.Add(currentVisual);
                                    if (k == 0) html.AppendFormat(" <td id=\"img_pr_div\" class=\"ro_visu1\" valign=\"top\"><a onclick=\"javascript:ZoomRolexImage_" + _resultControlId + "(''+document.getElementById('img_ro_" + m + "_" + _resultControlId + "').src+'');\"><img class=\"ro_visu\" id=\"img_ro_" + m + "_" + _resultControlId + "\" src=\"{0}\" /> </a></td>", currentVisual);
                                }


                                //Button arrow right
                                html.Append(" <td id=\"next\">");
                                html.AppendFormat(" <img id=\"imgNext_" + m + "_" + _resultControlId + "\" style=\"visibility: {1}; margin-left: 5px; cursor: pointer; margin-right: 5px; vertical-align: middle;\" onmouseover=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_right_down.gif'\" onmouseout=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_right.gif'\" onclick=\"javascript:DisplayPics_"
                                    + _resultControlId + "(1,document.getElementById('imgPrevious_" + m + "_" + _resultControlId + "'),this,document.getElementById('img_ro_" + m + "_" + _resultControlId + "'),'" + dr["VISUAL"].ToString() + "');\" src=\"/App_Themes/{0}/Images/Common/Button/arrow_right.gif\" />", _theme, visibility);
                                html.Append("</td>");



                                html.Append("</table>");
                                html.Append("</td></tr>");

                                m++;
                            }
                            #endregion

                            ////Space                      
                            html.Append("<tr><td valign=\"top\">");
                            html.Append("&nbsp;");
                            html.Append("</td></tr>");

                            html.Append("</table>");


                            #endregion

                        }

                        oldIdSite = Convert.ToInt64(dr["ID_SITE"]);
                        oldIdLocation = Convert.ToInt64(dr["ID_LOCATION"]);
                        oldIdPage = Convert.ToInt64(dr["ID_PAGE"]);
                    }

                    html.Append("</div> ");//fin 3
                    html.Append("</div> ");//fin 2
                    html.Append("</td></tr> ");//fin 1

                    html.Append("<tr> <td class=\"rolexScheduleResultFilterWebControlButtons\">");//debut 1 bis //puour le bouton close
                    #region Button close

                    html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"650\" class=\"rolexScheduleResultWebControlButtons\">");//height=\"60\"
                    html.Append("<tr>");
                    html.Append("<td class=\"rolexScheduleResultWebControlButtonCancel\">");
                    html.AppendFormat("<img src=\"/App_Themes/{0}/Images/Common/Button/initialize_all_up.gif\" onmouseover=\"javascript:this.src='/App_Themes/{0}/Images/Common/Button/initialize_all_down.gif';\" onmouseout=\"javascript:this.src='/App_Themes/{0}/Images/Common/Button/initialize_all_up.gif';\" onclick=\"javascript:displayRolexFile_"
                        + _resultControlId + "('" + string.Join(",", selectedLevelValues.ConvertAll(p => p.ToString()).ToArray()) + "','" + _periodBeginningDate + "','" + _periodEndDate + "',false);\"/>", _theme);
                    html.Append("</td>");
                    html.Append("</tr>");
                    html.Append("</table>");

                    #endregion
                    html.Append("</td></tr> ");//fin 1 bbis 

                    html.Append("</table>");

                }
                else
                {
                    html.AppendFormat("<div align=\"center\" class=\"vpResNoData\">{0}</div>", GestionWeb.GetWebWord(177, _session.SiteLanguage));
                }
            }


            return html.ToString();
        }

        #endregion

        #region ComputeDesign

        /// <summary>
        /// Provide html Code to present Rolex Schedule
        /// </summary>
        /// <param name="computeData"> ComputeData</param>
        /// <param name="computeSitesWithoutVisibility">compute No Visible Sites Data </param>
        /// <returns>HTML code</returns>
        protected virtual void ComputeDesign(Func<object[,]> computeData, Func<DataSet> computeSitesWithoutVisibility, StringBuilder htmlVisibility, StringBuilder htmlNoVisibility)
        {

            var data = computeData();
             
            #region No data
            if (data.GetLength(0) == 0)
            {
                htmlVisibility.AppendFormat("<div align=\"center\" class=\"vpResNoData\">{0}</div>",
                                            GestionWeb.GetWebWord(177, _session.SiteLanguage));
                return;
            }
            #endregion

            var fp =
               (!_isExcelReport) ? WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo
               : WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfoExcel;

            #region Init Variables
            const int rowSpanNb = 2;
            var html = new StringBuilder();
            int firstPeriodIndex = L3_ID_COLUMN_INDEX + 1;
            int nbColTab = data.GetLength(1);
            int nbline = data.GetLength(0);
            bool isExport = _isExcelReport || _isPDFReport;
            int labColSpan = (isExport) ? 2 : 1;
            string cssClasse;
            #endregion

            // html.Append(GetExcelHeader());

            #region Append Rolex Visibility Schedule
            //Beginning of html table
            html.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");

            #region Columns

            //Detail level column
            html.AppendFormat("\r\n\t\t<td colSpan=\"{4}\" rowspan=\"{3}\" width=\"250px\" class=\"{0}\" nowrap>{1}{2}</td>"
              , _style.CellTitle
              , GestionWeb.GetWebWord(2872, _session.SiteLanguage)
              , (!isExport) ? string.Empty : "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
              , rowSpanNb
              , labColSpan);

            //Periods
            string periodHtml = GetPeriodHtml(fp, firstPeriodIndex, nbColTab, data, isExport);
            html.Append(periodHtml);

            #endregion

          
            int i;

            string stringItem = "&nbsp;";
            string cssPresentClass = _style.CellPresent;

            #region Row Treatement
            for (i = 1; i < nbline; i++)
            {
                for (int j = 0; j < nbColTab; j++)
                {
                   
                    switch (j)
                    {
                        case L1_COLUMN_INDEX:
                            if (data[i, j] != null)
                            {
                                cssClasse = _style.CellLevelL1;
                                AppendLevelLabel(data, i, j, string.Empty, html, cssClasse, labColSpan);
                            }

                            break;
                        case L2_COLUMN_INDEX:
                            if (data[i, j] != null)
                            {
                                cssClasse = _style.CellLevelL2;
                                AppendLevelLabel(data, i, j, "&nbsp;", html, cssClasse, labColSpan);
                            }
                            break;
                        case L3_COLUMN_INDEX:
                            if (data[i, j] != null)
                            {
                                cssClasse = _style.CellLevelL3;
                                AppendLevelLabel(data, i, j, "&nbsp;&nbsp;", html, cssClasse, labColSpan);
                            }
                            break;
                        default:
                            var rolexItem = data[i, j] as RolexItem;
                            if (rolexItem != null)
                            {
                                switch ((rolexItem).GraphicItemType)
                                {
                                    case MotherMediaPlan.graphicItemType.present:
                                        var idLevels = rolexItem.Level1.ToString(CultureInfo.InvariantCulture);
                                        idLevels = idLevels + "," + rolexItem.Level2.ToString(fp);
                                        idLevels = idLevels + "," + rolexItem.Level3.ToString(fp);
                                        if (isExport)
                                            html.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, stringItem);
                                        else
                                            html.AppendFormat("<td class=\"{0}\"><a style=\"width:100%;height:100%;text-decoration:none;\"  href=\"javascript:displayRolexFile_"
                                                + _resultControlId + "('{2}',{3},{4}, true);\">{1}</a></td>", cssPresentClass, stringItem, idLevels, rolexItem.DateBegin.ToString("yyyyMMdd")
                                                , rolexItem.DateEnd.ToString("yyyyMMdd"));
                                        break;
                                    default:
                                        html.AppendFormat("<td class=\"{0}\">&nbsp;</td>", _style.CellNotPresent);
                                        break;
                                }
                            }
                            break;
                    }
                }
                html.Append("</tr>");
            }

            #endregion

           

            //End of html table
            html.Append("</table>");

            htmlVisibility.Append(html.ToString());
            #endregion

            //html.Append(GetExcelFooter());

            #region Append Rolex schedule sites without visibility
            html = new StringBuilder();
            using (var siteWithoutVisibility = computeSitesWithoutVisibility())
            {

                if (siteWithoutVisibility!=null && siteWithoutVisibility.Tables.Count>0 && siteWithoutVisibility.Tables[0].Rows.Count>0)
                {
                    //Beginning of html table
                    html.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");

                    #region Columns

                    //Detail level column
                    html.AppendFormat("\r\n\t\t<td colSpan=\"{4}\" rowspan=\"{3}\" width=\"250px\" class=\"{0}\" nowrap>{1}{2}</td>"
                      , _style.CellTitle
                      , GestionWeb.GetWebWord(2987, _session.SiteLanguage)
                      , (!isExport) ? string.Empty : "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                      , rowSpanNb
                      , labColSpan);

                    html.Append(periodHtml);

                    #endregion

                    cssClasse = _style.CellLevelL1;
                    foreach (DataRow dr in siteWithoutVisibility.Tables[0].Rows)
                    {
                        AppendLevelLabel(dr["SITE"].ToString(), string.Empty, html, cssClasse, labColSpan);
                        for (int j = L3_ID_COLUMN_INDEX+1; j < nbColTab; j++)
                        {
                            html.AppendFormat("<td class=\"{0}\">&nbsp;</td>", _style.CellNotPresent);
                        }
                        html.Append("</tr>");
                    }

                    //End of html table
                    html.Append("</table>");
                    htmlNoVisibility.Append(html.ToString());
                }
            }

          

            #endregion

           
         
        }

       

        #endregion

        #region GetExcelFooter
        /// <summary>
        /// Get Excel Footer
        /// </summary>
        /// <returns></returns>
        protected virtual string GetExcelFooter()
        {
            var html = new StringBuilder();
            if (_isExcelReport)
            {
                html.Append(FctExcel.GetFooter(_session));
            }
            return html.ToString();
        }

        #endregion

        #region GetExcelHeader
        /// <summary>
        /// Get Excel Header
        /// </summary>
        /// <returns></returns>
        protected virtual string GetExcelHeader()
        {
            #region Selection callback
            var html = new StringBuilder();
            if (_isExcelReport)
            {
                html.Append(FctExcel.GetLogo(_session));
                html.Append(FctExcel.GetExcelHeader(_session, false, false));
            }

            #endregion

            return html.ToString();
        }
        #endregion

        #region GetPeriodHtml
        protected virtual string GetPeriodHtml(CultureInfo cultureInfo, int firstPeriodIndex, int nbColTab,
                                            object[,] data, bool isExport)
        {
            var html = new StringBuilder();
            var headers = new StringBuilder();
            var periods = new StringBuilder();
            int oldYear = -1;
            int oldMonth = -1, start = -1, colSpan = 0;
            string periodClass = _style.CellPeriod;
            string pWidth = !isExport ? "width=\"17px\"" : string.Empty;
            for (int j = firstPeriodIndex; j < nbColTab; j++)
            {
                int curYear = int.Parse(data[0, j].ToString().Substring(0, 4));
                int curWeek = int.Parse(data[0, j].ToString().Substring(4, 2));
                int curMonth = Dates.GetMonthFromWeek(curYear, curWeek);

                if (oldMonth != curMonth && start > -1)
                {
                    string monthString = "&nbsp;", yearString = string.Empty;
                    if (colSpan > 1)
                    {
                        monthString = MonthString.GetHTMLCharacters(oldMonth, _session.SiteLanguage, 3);
                        yearString = (oldYear.ToString(cultureInfo).Length == 1)
                                         ? "0" + oldYear.ToString(cultureInfo)
                                         : oldYear.ToString(CultureInfo.InvariantCulture);
                        yearString = yearString.Substring(2, 2);
                    }
                    headers.AppendFormat(" <td colspan=\"{0}\" class=\"{3}\">{1} {2}</td>", colSpan, monthString, yearString,
                                         _style.CellYear);
                    colSpan = 0;
                }

                periods.AppendFormat(
                    "<td class=\"{0}\" {2}>&nbsp;{1}&nbsp;</td>"
                    , periodClass
                    , data[0, j].ToString().Substring(4, 2), pWidth);

                start = 0;
                oldMonth = curMonth;
                oldYear = curYear;
                colSpan++;
            }
            if (start > -1)
            {
                string monthString = "&nbsp;", yearString = string.Empty;
                if (colSpan > 1)
                {
                    monthString = MonthString.GetHTMLCharacters(oldMonth, _session.SiteLanguage, 3);
                    yearString = (oldYear.ToString(cultureInfo).Length == 1)
                                     ? "0" + oldYear.ToString(cultureInfo)
                                     : oldYear.ToString(cultureInfo);
                    yearString = yearString.Substring(2, 2);
                }
                headers.AppendFormat(" <td colspan=\"{0}\" class=\"{3}\">{1} {2}</td>", colSpan, monthString, yearString,
                                     _style.CellYear);
            }
            html.AppendFormat("{0}</tr>", headers);
            html.AppendFormat("<tr>{0}</tr>", periods);

            return html.ToString();
        }
        #endregion

        #region Append level label
        protected virtual void AppendLevelLabel(object[,] data, int i, int j, string padding, StringBuilder html,
                                               string cssClasse, int labColSpan)
        {

            string label = (data[i, j] != null) ? Convert.ToString(data[i, j]) : string.Empty;
            AppendLevelLabel(label, padding, html, cssClasse, labColSpan);
        }
        protected virtual void AppendLevelLabel(string label, string padding, StringBuilder html,
                                              string cssClasse, int labColSpan)
        {
            html.AppendFormat("\r\n\t<tr>\r\n\t\t<td class=\"{0}\" colSPan=\"{1}\" nowrap>{4}{2}{3}{5}</td>"
                              , cssClasse
                              , labColSpan
                              , padding
                              , label
                              , ((_isExcelReport) ? "=\"" : "")
                              , ((_isExcelReport) ? "\"" : ""));
        }
        #endregion

        #region ComputeSitesWithoutVisibility
        /// <summary>
        /// Compute Sites Without Visibility
        /// </summary>
        /// <returns></returns>
        protected virtual DataSet ComputeSitesWithoutVisibility()
        {
            var rolexScheduleDAL = GetRolexScheduleDAL();

            DetailLevelItemInformation detailLevelInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.site);
            return rolexScheduleDAL.GetSitesWithoutVisibility(detailLevelInformation);
        } 
        #endregion

        #region Compute Data

        /// <summary>
        /// Compute data from database
        /// </summary>
        /// <returns>Formatted table ready for UI design</returns>
        protected virtual object[,] ComputeData()
        {
            object[,] oTab;

            try
            {
                var detailLevel = _session.GenericMediaDetailLevel;

                var rolexScheduleDAL = GetRolexScheduleDAL();

                using (var ds = rolexScheduleDAL.GetData(detailLevel))
                {
                    Int64 currentLineIndex = 0;
                    int indexPeriod = -1;


                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null)
                        return (new object[0, 0]);

                    var dt = ds.Tables[0];

                    var dtLevels = GetDataLevels(detailLevel, dt);

                    #region Count nb of elements for each classification level
                    int nbLevels = detailLevel.GetNbLevels;
                    Int64 oldIdL1 = long.MinValue;
                    Int64 oldIdL2 = long.MinValue;
                    Int64 oldIdL3 = long.MinValue;
                    int nbL1, nbL2, nbL3;
                    bool newL2, newL3;

                    CountNbItemsByLevel(oldIdL3, out nbL2, out newL3, out nbL3, oldIdL2, nbLevels, dtLevels, oldIdL1, out nbL1, out newL2, detailLevel);
                    newL2 = newL3 = false;
                    oldIdL1 = oldIdL2 = oldIdL3 = long.MinValue;
                    #endregion

                    //No Data
                    if (nbL1 == 0)
                        return (new object[0, 0]);



                    #region Create periods table
                    var periodItemsList = new List<Int64>();
                    var periods = new Dictionary<long, int>();
                    //var years_index = new Dictionary<int, int>();
                    DateTime periodBegin = DateString.YYYYMMDDToDateTime(_periodBeginningDate);
                    DateTime periodEnd = DateString.YYYYMMDDToDateTime(_periodEndDate);

                    int firstPeriodIndex = L3_ID_COLUMN_INDEX + 1;

                    var currentWeek = new AtomicPeriodWeek(periodBegin);
                    var endWeek = new AtomicPeriodWeek(periodEnd);
                    int currentDate = currentWeek.Year;
                    int oldCurrentDate = endWeek.Year;
                    endWeek.Increment();
                    while (!(currentWeek.Week == endWeek.Week && currentWeek.Year == endWeek.Year))
                    {
                        periodItemsList.Add(currentWeek.Year * 100 + currentWeek.Week);
                        periods.Add(currentWeek.Year * 100 + currentWeek.Week, periodItemsList.Count - 1);
                        currentWeek.Increment();

                    }

                    #endregion

                    #region Indexes tables
                    // Column number
                    int nbCol = periodItemsList.Count + firstPeriodIndex;
                    // Line number
                    int nbline = nbL1 + nbL2 + nbL3 + 1;
                    // Result table
                    oTab = new object[nbline, nbCol];
                    #endregion

                    #region Column Labels
                    currentDate = 0;
                    while (currentDate < periodItemsList.Count)
                    {
                        oTab[0, currentDate + firstPeriodIndex] = periodItemsList[currentDate].ToString(); ;
                        currentDate++;
                    }

                    #endregion

                    #region Build result table
                    foreach (DataRow currentRowLevels in dtLevels.Rows)
                    {
                        #region New L1
                        if (nbLevels >= 1 && oldIdL1 != GetLevelId(currentRowLevels, 1, detailLevel))
                        {
                            // Next L2 is new
                            newL2 = true;
                            currentLineIndex++;
                            oldIdL1 = SetLevelData(detailLevel, currentRowLevels, nbCol, firstPeriodIndex, currentLineIndex, oTab, GetLevelId(currentRowLevels, 1, detailLevel)
                              , GetLevelLabel(currentRowLevels, 1, detailLevel), null, null, null, null, 1);
                        }
                        #endregion

                        #region New L2
                        if (nbLevels >= 2 && (oldIdL2 != GetLevelId(currentRowLevels, 2, detailLevel) || newL2))
                        {
                            // Next L3 is new
                            newL3 = true;
                            newL2 = false;
                            currentLineIndex++;
                            oldIdL2 = SetLevelData(detailLevel, currentRowLevels, nbCol, firstPeriodIndex, currentLineIndex, oTab, oldIdL1
                              , null, GetLevelId(currentRowLevels, 2, detailLevel), GetLevelLabel(currentRowLevels, 2, detailLevel), null, null, 2);
                        }
                        #endregion

                        #region New L3
                        if (nbLevels >= 3 && (oldIdL3 != GetLevelId(currentRowLevels, 3, detailLevel) || newL3))
                        {
                            newL3 = false;
                            currentLineIndex++;
                            oldIdL3 = SetLevelData(detailLevel, currentRowLevels, nbCol, firstPeriodIndex, currentLineIndex, oTab, oldIdL1
                                , null, oldIdL2, null, GetLevelId(currentRowLevels, 3, detailLevel), GetLevelLabel(currentRowLevels, 3, detailLevel), 3);
                        }
                        #endregion

                        #region Treat present Period
                        while (dt.Rows.Count > indexPeriod + 1
                           && ((nbLevels >= 1 && dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(1)].Equals(currentRowLevels[detailLevel.GetColumnNameLevelId(1)])) || nbLevels < 1)
                           && ((nbLevels >= 2 && dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(2)].Equals(currentRowLevels[detailLevel.GetColumnNameLevelId(2)])) || nbLevels < 2)
                           && ((nbLevels >= 3 && dt.Rows[indexPeriod + 1][detailLevel.GetColumnNameLevelId(3)].Equals(currentRowLevels[detailLevel.GetColumnNameLevelId(3)])) || nbLevels < 3))
                        {
                            indexPeriod++;
                            var currentRow = dt.Rows[indexPeriod];
                            var dateVisibility = currentRow["date_num"].ToString();
                            if (periods.ContainsKey(Int64.Parse(dateVisibility)))
                            {
                                int currentDateIndex = periods[Int64.Parse(dateVisibility)];
                                var atomicPeriodWeek = new AtomicPeriodWeek(int.Parse(dateVisibility.Substring(0, 4)), int.Parse(dateVisibility.Substring(4, 2)));
                                oTab[currentLineIndex, firstPeriodIndex + currentDateIndex] = new RolexItem(MotherMediaPlan.graphicItemType.present, oldIdL1, oldIdL2, oldIdL3
                                    , atomicPeriodWeek.FirstDay, atomicPeriodWeek.LastDay);
                            }
                        }

                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception err)
            {
                throw new RolexResultsException("Error when computing Rolex visibility calendar.", err);
            }

            return oTab;

        }

        private IRolexDAL GetRolexScheduleDAL()
        {
            if (_module.CountryDataAccessLayer == null)
                throw (new NullReferenceException("Data access layer is null for the Rolex Schedule result"));
            var param = new object[3];
            param[0] = _session;
            param[1] = _periodBeginningDate;
            param[2] = _periodEndDate;
            var rolexScheduleDAL =
                (IRolexDAL)
                AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                    AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName,
                    _module.CountryDataAccessLayer.Class, false
                    , BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            return rolexScheduleDAL;
        }

        private long SetLevelData(GenericDetailLevel detailLevel, DataRow currentRowLevels, int nbCol, int firstPeriodIndex,
                                  long currentLineIndex, object[,] oTab, long? idL1, string level1, long? idL2, string level2, long? idL3, string level3, int currentLevel)
        {
            oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = idL1;
            oTab[currentLineIndex, L1_COLUMN_INDEX] = level1;
            oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = idL2;
            oTab[currentLineIndex, L2_COLUMN_INDEX] = level2;
            oTab[currentLineIndex, L3_COLUMN_INDEX] = level3;
            oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = idL3;

            long oldIdLevel = GetLevelId(currentRowLevels, currentLevel, detailLevel);


            // Create Rolex Items
            for (int mpi = firstPeriodIndex; mpi < nbCol; mpi++)
            {
                oTab[currentLineIndex, mpi] = new RolexItem();
            }
            return oldIdLevel;
        }

        protected virtual void CountNbItemsByLevel(long oldIdL3, out int nbL2, out bool newL3, out int nbL3, long oldIdL2, int nbLevels, DataTable dtLevels, long oldIdL1,
                                                   out int nbL1, out bool newL2, GenericDetailLevel detailLevel)
        {
            newL2 = false;
            newL3 = false;
            nbL1 = 0;
            nbL2 = 0;
            nbL3 = 0;


            foreach (DataRow currentRow in dtLevels.Rows)
            {
                if (nbLevels >= 1 && oldIdL1 != GetLevelId(currentRow, 1, detailLevel))
                {
                    newL2 = true;
                    nbL1++;
                    oldIdL1 = GetLevelId(currentRow, 1, detailLevel);
                }
                if (nbLevels >= 2 && (oldIdL2 != GetLevelId(currentRow, 2, detailLevel) || newL2))
                {
                    newL3 = true;
                    newL2 = false;
                    nbL2++;
                    oldIdL2 = GetLevelId(currentRow, 2, detailLevel);
                }
                if (nbLevels >= 3 && (oldIdL3 != GetLevelId(currentRow, 3, detailLevel) || newL3))
                {
                    newL3 = false;
                    nbL3++;
                    oldIdL3 = GetLevelId(currentRow, 3, detailLevel);
                }
            }
        }

        #endregion

        #region GetDataLevels
        /// <summary>
        /// Get Data Levels
        /// </summary>
        /// <returns>Data Levels</returns>
        protected virtual DataTable GetDataLevels(GenericDetailLevel detailLevel, DataTable dt)
        {
            var columnsLevels = new List<string>();
            for (int iLevel = 1; iLevel <= detailLevel.GetNbLevels; iLevel++)
            {
                columnsLevels.Add(detailLevel.GetColumnNameLevelId(iLevel));
                columnsLevels.Add(detailLevel.GetColumnNameLevelLabel(iLevel));
            }

            #region Create datatable with distinct levels
            var dtLevels = new DataTable();

            //Create Columns
            foreach (string columnLevel in columnsLevels)
                dtLevels.Columns.Add(dt.Columns[columnLevel].ColumnName, dt.Columns[columnLevel].DataType);

            //Fill DataTable
            var rowsAddKey = new Dictionary<string, bool>();
            foreach (DataRow row in dt.Rows)
            {
                var columnsIds = new List<Int64>();
                var columnsValues = new List<object>();

                //Add Values for current row in local variables
                for (int i = 0; i < columnsLevels.Count; i++)
                {
                    if (i % 2 == 0) columnsIds.Add(Int64.Parse(row[columnsLevels[i]].ToString()));
                    columnsValues.Add(row[columnsLevels[i]]);
                }

                //Dertermine if values is already Add
                var currentRowKey = string.Join(";", columnsIds.ConvertAll(p => p.ToString()).ToArray());
                if (!rowsAddKey.ContainsKey(currentRowKey))
                {
                    rowsAddKey.Add(currentRowKey, true);
                    dtLevels.Rows.Add(columnsValues.ToArray());
                }
            }
            #endregion

            dtLevels.PrimaryKey = dtLevels.Columns.Cast<DataColumn>().ToArray();

            return dtLevels;
        }
        #endregion

        #region Get Level Id / Label
        /// <summary>
        /// Get Level Id
        /// </summary>
        /// <param name="dr">Data Source</param>
        /// <param name="level">Requested level</param>
        /// <param name="detailLevel">Levels breakdown</param>
        /// <returns>Level Id</returns>
        protected virtual Int64 GetLevelId(DataRow dr, int level, GenericDetailLevel detailLevel)
        {
            return (Int64.Parse(dr[detailLevel.GetColumnNameLevelId(level)].ToString()));
        }
        /// <summary>
        /// Get Level Label
        /// </summary>
        /// <param name="dr">Data Source</param>
        /// <param name="level">Requested level</param>
        /// <param name="detailLevel">Levels breakdown</param>
        /// <returns>Level Label</returns>
        protected string GetLevelLabel(DataRow dr, int level, GenericDetailLevel detailLevel)
        {
            return (dr[detailLevel.GetColumnNameLevelLabel(level)].ToString());
        }

        #endregion

        #region Set Periods
        /// <summary>
        /// Set periods
        /// </summary>
        protected void SetPeriods()
        {
            if (_session.PeriodType == CstWeb.CustomerSessions.Period.Type.allHistoric)
            {
                var param = new object[1];
                param[0] = _session;
                var rolexScheduleDAL =
                    (IRolexDAL)
                    AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                        AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName,
                        _module.CountryDataAccessLayer.Class, false,
                        BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                DataSet ds = rolexScheduleDAL.GetMinMaxPeriod();
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    string periodBeginningDate = ds.Tables[0].Rows[0]["DATE_BEGIN_NUM"].ToString();
                    var dateBegin = new DateTime(int.Parse(periodBeginningDate.Substring(0, 4)),
                                                 int.Parse(periodBeginningDate.Substring(4, 2)), 1);
                    dateBegin = dateBegin.AddMonths(-1);
                    _periodBeginningDate = dateBegin.ToString("yyyyMMdd");
                    string periodEndDate = ds.Tables[0].Rows[0]["DATE_END_NUM"].ToString();
                    var dateEnd = new DateTime(int.Parse(periodEndDate.Substring(0, 4)),
                                               int.Parse(periodEndDate.Substring(4, 2)),
                                               int.Parse(periodEndDate.Substring(6, 2)));
                    dateEnd = dateEnd.AddMonths(1);
                    int days = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
                    _periodEndDate = dateEnd.ToString("yyyyMM") + days.ToString();
                }
            }
            else
            {
                var dateBegin = new DateTime(int.Parse(_session.PeriodBeginningDate.Substring(0, 4)),
                                             int.Parse(_session.PeriodBeginningDate.Substring(4, 2)), 1);
                dateBegin = dateBegin.AddMonths(-1);
                _periodBeginningDate = dateBegin.ToString("yyyyMMdd");
                var dateEnd = new DateTime(int.Parse(_session.PeriodEndDate.Substring(0, 4)),
                                           int.Parse(_session.PeriodEndDate.Substring(4, 2)),
                                           int.Parse(_session.PeriodEndDate.Substring(6, 2)));
                dateEnd = dateEnd.AddMonths(1);
                int days = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
                _periodEndDate = dateEnd.ToString("yyyyMM") + days.ToString();
            }
        }
        #endregion
    }
}
