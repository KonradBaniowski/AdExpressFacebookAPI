using System;

using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Hotep.Common;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using System.IO;
using TNS.AdExpressI.ProductClassIndicators.Russia.Engines;
using System.Drawing;
using PDFCreatorPilotLib;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Anubis.Hotep.Exceptions;
using TNS.AdExpress.Anubis.Hotep.Russia.UI;
using Dundas.Charting.WinControl;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstComparisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstDbClassif = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using System.Collections;
using System.Globalization;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.CampaignTypes;
using System.Windows.Forms;
using TNS.AdExpress.Web.UI;
using TNS.FrameWork.WebTheme;
using TNS.Ares.Pdf.Exceptions;

namespace TNS.AdExpress.Anubis.Hotep.Russia.BusinessFacade
{
    public class HotepPdfSystem : TNS.AdExpress.Anubis.Hotep.BusinessFacade.HotepPdfSystem
    {

        #region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSource">DataSource</param>
        /// <param name="config">Config</param>
        /// <param name="rqDetails">Request details</param>
        /// <param name="webSession">Web session</param>
        public HotepPdfSystem(IDataSource dataSource, TNS.AdExpress.Anubis.Hotep.Common.HotepConfig config, DataRow rqDetails, WebSession webSession, TNS.FrameWork.WebTheme.Theme theme)
            :
            base(dataSource,  config,  rqDetails,  webSession, theme)
        {
           
        }
        #endregion

        #region Fill
        /// <summary>
        /// Fill
        /// </summary>
        public override void Fill()
        {

            try
            {

                #region Session Parameters
                SessionParameter();
                #endregion

                #region IndicatorSynthesis
                IndicatorSynthesis();
                #endregion

                #region IndicatorSeasonality
                IndicatorSeasonality();
                #endregion

                #region Indicator Media Strategy
                IndicatorMediaStrategy();
                #endregion

                #region IndicatorPalmares (advertiserChart)
                IndicatorPalmares(CstResult.PalmaresRecap.ElementType.advertiser);
                #endregion

                #region IndicatorPalmares (referenceChart)
                IndicatorPalmares(CstResult.PalmaresRecap.ElementType.product);
                #endregion

                #region IndicatorEvolution (advertiserChart)
                IndicatorEvolution(CstResult.EvolutionRecap.ElementType.advertiser);
                #endregion

                #region IndicatorEvolution (referenceChart)
                IndicatorEvolution(CstResult.EvolutionRecap.ElementType.product);
                #endregion

                #region Indicator Novelty
                IndicatorNovelty();
                #endregion

                #region MainPage
                MainPageDesign();
                #endregion

                #region Header and Footer
                string dateString = Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

                this.AddHeadersAndFooters(
                _webSession,
                imagePosition.leftImage,
                GestionWeb.GetWebWord(2173, _webSession.SiteLanguage) + " - " + dateString,
                0, -1, true);
                #endregion

            }
            catch (Exception e)
            {
                throw new HotepPdfException("Error in Fill in HotepPdfSystem", e);
            }
        }
        #endregion

        #region MainPage
        /// <summary>
        /// Design Main Page
        /// </summary>
        /// <returns></returns>
        protected virtual void MainPageDesign()
        {

            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

            Picture picture = ((Picture)Style.GetTag("pictureTitle"));
            string imgPath = string.Empty;

            if (File.Exists(picture.Path))
            {
                imgPath = picture.Path;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path))
            {
                imgPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path;
            }
            else
            {
                imgPath = picture.Path;
            }


            Image imgG = Image.FromFile(imgPath);

            double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin) / (double)imgG.Width;
            double coef = Math.Min((double)1.0, w);
            w = (double)(this.WorkZoneBottom - this.WorkZoneTop) / (double)imgG.Height;
            coef = Math.Min((double)coef, w);

            int imgI = this.AddImageFromFilename(imgPath, TxImageCompressionType.itcFlate);

            StringBuilder html = new StringBuilder();
            string str = "";

            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

            #region result

            #region HTML
            html.Append("<TABLE id=\"Table1\" align=\"center\" cellSpacing=\"1\" cellPadding=\"1\" width=\"1100\" border=\"0\" style=\"WIDTH: 1100px; HEIGHT: 2px\">");
            html.Append("<P>&nbsp;</P>");
            html.Append("<P>&nbsp;</P>");
            html.Append("<P>&nbsp;</P>");
            html.Append("<TD style=\"HEIGHT: 43px\">");
            html.Append("<P align=\"center\"><B><SPAN class=\"TreeTitleViolet40pt\">" + GestionWeb.GetWebWord(2173, _webSession.SiteLanguage) + "</SPAN></B><SPAN class=\"TreeTitleBlack10pt\">");
            html.Append("<o:p></o:p></SPAN></P>");
            html.Append("</TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD><P align=\"center\"><SPAN class=\"TreeTitleViolet\">" + str + "</SPAN></P></TD>");
            html.Append("</TR>");
            html.Append("<P>&nbsp;</P>");
            html.Append("<P>&nbsp;</P>");
            html.Append("<TR>");
            html.Append("<TD>&nbsp;</TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD>&nbsp;</TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD align=\"center\">");
            html.Append("<TABLE  class=\"TreeHeaderVioletBorder\" id=\"Table1\" cellSpacing=\"0\" cellPadding=\"0\" width=\"600\" align=\"center\">");
            html.Append("<TR>");
            html.Append("<TD colSpan=\"2\" class=\"violetBorderBottom\">" + GestionWeb.GetWebWord(1979, _webSession.SiteLanguage) + "</TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD>&nbsp;</TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            int line = 2;
            foreach (string indicatorName in _titleList)
            {
                if ((line % 2 == 0) && (line > 2))
                {
                    html.Append("</TR>");
                    html.Append("<TR>");
                }
                html.Append("<TD><li style=\"LIST-STYLE-TYPE: square\">" + indicatorName + "</li></TD>");
                line++;
            }
            html.Append("</TR>");
            html.Append("</TABLE>");

            html.Append("</TD>");
            html.Append("</TR>");
            html.Append("</TABLE>");
            #endregion

            this.ConvertHtmlToPDF(html.ToString(), charSet, themeName, _config.WebServer, _config.Html2PdfLogin, _config.Html2PdfPass, 0);

            this.PDFPAGE_ShowImage(imgI,
                (double)(this.PDFPAGE_Width / 2 - coef * imgG.Width / 2), (double)(this.WorkZoneBottom - coef * imgG.Height - 50),
                (double)(coef * imgG.Width), (double)(coef * imgG.Height), 0);

            #endregion

        }
        #endregion

        #region SessionParameter
        /// <summary>
        /// Session parameter design
        /// </summary>
        protected override void SessionParameter()
        {

            int nbLinesEnd = 0;
            int j = 0;
            StringBuilder html = new StringBuilder();
            bool showProductSelection = false;
            IList nbLinesSelectionMedia = new ArrayList(), nbLinesSelectionDetailMedia = new ArrayList(), nbLinesSelectionProduct = new ArrayList();

            try
            {
                IList SelectionMedia = ToHtml(_webSession.SelectionUniversMedia, false, false, 600, false, _webSession.SiteLanguage, 2, 1, true, 20, ref nbLinesEnd, ref nbLinesSelectionMedia);
                nbLinesEnd = 27;

                IList SelectionDetailMedia = ToHtml((TreeNode)_webSession.SelectionUniversMedia.FirstNode, true, true, 600, true, _webSession.SiteLanguage, 3, 1, false, 21, ref nbLinesEnd, ref nbLinesSelectionDetailMedia);
                if (nbLinesSelectionDetailMedia.Count == 0)
                    nbLinesEnd = 21;
                else if (nbLinesSelectionDetailMedia.Count == 1)
                {
                    nbLinesEnd = 17 - (int)nbLinesSelectionDetailMedia[0];
                    if (nbLinesEnd < 0)
                        nbLinesEnd = 28;
                }
                else
                    nbLinesEnd = 27 - (int)nbLinesSelectionDetailMedia[nbLinesSelectionDetailMedia.Count - 1];

                #region Title
                html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
                html.Append("<TR height=\"25\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"14\">");
                html.Append("<TD class=\"TreeTitleViolet20px\">" + GestionWeb.GetWebWord(1752, _webSession.SiteLanguage) + "</TD>");
                html.Append("</TR>");
                #endregion

                #region Etude comparative
                if (_webSession.ComparativeStudy)
                {
                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1118, _webSession.SiteLanguage) + " </TD>");
                    html.Append("</TR>");
                }
                #endregion

                #region Period
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1755, _webSession.SiteLanguage) + " :</TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"20\">");
                html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                    + HtmlFunctions.GetPeriodDetail(_webSession)
                    + "</TD>");
                html.Append("</TR>");
                #endregion

                #region Média
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1292, _webSession.SiteLanguage) + " :</TD>");
                html.Append("<TR>");
                html.Append("<TD align=\"left\">");
                html.Append(SelectionMedia[0].ToString());
                html.Append("</TD>");
                html.Append("</TR>");
                html.Append("</TR>");
                #endregion

                if (SelectionDetailMedia != null && SelectionDetailMedia.Count > 0)
                {

                    #region Détail Média
                    html.Append(GetMediaDetail(SelectionDetailMedia, true, 0));
                    for (j = 1; j < SelectionDetailMedia.Count; j++)
                    {

                        this.ConvertHtmlToPDF(html.ToString(),
                            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                            _config.WebServer,
                            _config.Html2PdfLogin,
                            _config.Html2PdfPass);

                        html = new StringBuilder();

                        #region Détail Média
                        html.Append(GetMediaDetail(SelectionDetailMedia, false, j));
                        #endregion
                    }
                    #endregion

                }

                #region Unité
                if (SelectionDetailMedia == null || SelectionDetailMedia.Count == 0)
                {
                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1795, _webSession.SiteLanguage) + " :</TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"20\">");
                    html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                        + GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage)
                        + "</TD>");
                    html.Append("</TR>");
                }
                #endregion

                #region Campaingn type (RU)
                if (WebApplicationParameters.AllowCampaignTypeOption && CampaignTypesInformation.ContainsKey(_webSession.CampaignType))
                {
                    List<CampaignTypeInformation> campaignTypes = _webSession.GetValidCampaignTypeForResult();

                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(2671, _webSession.SiteLanguage) + " :</TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"20\">");
                    html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                        + GestionWeb.GetWebWord(CampaignTypesInformation.Get(_webSession.CampaignType).WebTextId, _webSession.SiteLanguage)
                        + "</TD>");
                    html.Append("</TR>");
                }
                #endregion

                #region produit
                // Détail produit
                if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
                {
                    int nbLineByPage = 43;
                    int currentLine = 13;
                    if (_webSession.ComparativeStudy) currentLine = currentLine + 3;
                    if (SelectionDetailMedia == null || SelectionDetailMedia.Count == 0) currentLine = currentLine + 4;//6
                    if (SelectionDetailMedia != null && SelectionDetailMedia.Count > 0)
                    {
                        currentLine = 3;
                        nbLineByPage = 34;

                        this.ConvertHtmlToPDF(html.ToString(),
                            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                            _config.WebServer,
                            _config.Html2PdfLogin,
                            _config.Html2PdfPass);

                        html = new StringBuilder();

                        html.Append("<TABLE id=\"Table1\" align=\"center\" cellSpacing=\"1\" cellPadding=\"1\" width=\"1100\" border=\"0\" style=\"WIDTH: 1100px; HEIGHT: 2px\">");

                    }
                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1759, _webSession.SiteLanguage) + " :</TD>");
                    html.Append("<TR><TD>&nbsp;</TD>");
                    html.Append("</TR>");
                    html.Append("<TR>");
                    html.Append("<TD align=\"center\">");

                    html.Append(TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource, 600, true, nbLineByPage, ref currentLine));
                    showProductSelection = true;

                    if (showProductSelection)
                    {
                        html.Append("<br>");
                        html.Append("<div class=\"txtViolet11Bold\" align=\"left\" >" + "&nbsp;&nbsp;" + GestionWeb.GetWebWord(1601, _webSession.SiteLanguage) + "</div>");
                        html.Append(TNS.AdExpress.Web.BusinessFacade.Selections.Products.SectorsSelectedBusinessFacade.GetSectorsSelected(_webSession, false));
                    }
                    html.Append("</TD>");
                    html.Append("</TR>");
                    html.Append("</TR>");

                }
                #endregion

                #region Unité
                if (SelectionDetailMedia != null && SelectionDetailMedia.Count > 0)
                {
                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1795, _webSession.SiteLanguage) + " :</TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"20\">");
                    html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                        + GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage)
                        + "</TD>");
                    html.Append("</TR>");
                }
                #endregion


                html.Append("</TABLE>");

                this.ConvertHtmlToPDF(html.ToString(),
                            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
                            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                            _config.WebServer,
                            _config.Html2PdfLogin,
                            _config.Html2PdfPass);

                html = new StringBuilder();


                _titleList.Add(GestionWeb.GetWebWord(1752, _webSession.SiteLanguage));

            }
            catch (System.Exception e)
            {
                throw (new HotepPdfException("Unable to build the session parameter page for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
        }
        #endregion

        #region Indicator Synthesis (similar to AdExpress)
        /// <summary>
        /// Indicator Synthesis design
        /// </summary>
        protected override void IndicatorSynthesis()
        {

            StringBuilder html = new StringBuilder();

            try
            {
                html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");

                #region Title
                html.Append("<TR height=\"25\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"14\">");
                html.Append("<TD class=\"TreeTitleViolet20px\">" + GestionWeb.GetWebWord(1664, _webSession.SiteLanguage) + "</TD>");
                html.Append("</TR>");
                #endregion

                #region result
                html.Append("<TR height=\"25\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR align=\"center\"><td>");
                html.Append(_productClassIndicator.GetSummary());
                html.Append("</td></tr>");
                #endregion

                html.Append("</table>");

                this.ConvertHtmlToPDF(html.ToString(),
                            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
                            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                            _config.WebServer,
                            _config.Html2PdfLogin,
                            _config.Html2PdfPass);


                _titleList.Add(GestionWeb.GetWebWord(1664, _webSession.SiteLanguage));

            }
            catch (System.Exception e)
            {
                throw (new HotepPdfException("Unable to process the synthesis result for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
        }
        #endregion

        #region Indicator Seasonality
        /// <summary>
        /// Graphiques Indicator Seasonality
        /// </summary>
        protected override void IndicatorSeasonality()
        {
            CultureInfo cInfoSave = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo;


            StreamWriter sw = null;
            Image img = null;
            object[,] tab = null;
            EngineSeasonality engine =  new EngineSeasonality(_webSession, _productClassIndicatorDAL);


            #region Load Data
            if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)
            {
                _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                tab = engine.GetChartData();
                _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;
            }
            else
            {
                tab = engine.GetChartData();
            }
            #endregion

            try
            {
                if (tab.Length != 0)
                {

                    this.NewPage();

                    this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

                    string workFile = Path.GetTempFileName();

                    #region Title
                    Style.GetTag("SeasonalityGraphTitleFontPage").SetStylePdf(this, GetTxFontCharset());
                    this.PDFPAGE_UnicodeTextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1139, _webSession.SiteLanguage));
                    #endregion

                    #region GRP graph

                    UISeasonalityGraph graph = new UISeasonalityGraph(_webSession, _dataSource, _config, tab, Style);
                    graph.BuildSeasonality();
                    graph.SaveAsImage(workFile, ChartImageFormat.Bmp);
                    img = Image.FromFile(workFile);
                    double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width / ((double)img.Width)));
                    coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40) / ((double)img.Height)));
                    int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
                    this.PDFPAGE_ShowImage(i,
                        (this.PDFPAGE_Width / 2) - (coef * img.Width / 2),
                        this.PDFPAGE_Height / 2 - (coef * img.Height / 2),
                        coef * img.Width,
                        coef * img.Height,
                        0);
                    img.Dispose();
                    img = null;
                    graph.Dispose();
                    graph = null;

                    #region Clean File
                    File.Delete(workFile);
                    #endregion

                    #endregion

                    _titleList.Add(GestionWeb.GetWebWord(1139, _webSession.SiteLanguage));

                }

            }
            catch (System.Exception ex)
            {
                try
                {
                    sw.Close();
                    img.Dispose();
                    img = null;
                }
                catch (System.Exception e2) { }
                throw (new HotepPdfException("impossible to generate chart of Indicator Seasonality.", ex));
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = cInfoSave;
        }
        #endregion

        #region Indicator Palmares
        /// <summary>
        /// Graphiques Indicator Palmares
        /// </summary>
        protected override void IndicatorPalmares(FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType)
        {

            StreamWriter sw = null;
            Image img = null;
            bool isPresent = false;

            #region Load Data
            object[,] tab = null;
            EngineTop engine = new TNS.AdExpressI.ProductClassIndicators.Russia.Engines.EngineTop(_webSession, _productClassIndicatorDAL);
            tab = engine.GetData(CstResult.PalmaresRecap.typeYearSelected.currentYear, tableType);
            #endregion

            try
            {
                if (tab.GetLongLength(0) != 1 && double.Parse(tab[0, FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString()) != 0)
                {

                    this.NewPage();

                    this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

                    string workFile = Path.GetTempFileName();

                    #region Title
                    Style.GetTag("PalmaresGraphTitleFontPage").SetStylePdf(this, GetTxFontCharset());
                    this.PDFPAGE_UnicodeTextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1980, _webSession.SiteLanguage));
                    #endregion

                    #region GRP graph

                    UIPalmaresGraph graph = new UIPalmaresGraph(_webSession, _dataSource, _config, tab, Style);
                    graph.BuildPalmares(tableType);
                    graph.SaveAsImage(workFile, ChartImageFormat.Bmp);
                    img = Image.FromFile(workFile);
                    double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width / ((double)img.Width)));
                    coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40) / ((double)img.Height)));
                    int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
                    this.PDFPAGE_ShowImage(i,
                        (this.PDFPAGE_Width / 2) - (coef * img.Width / 2),
                        this.PDFPAGE_Height / 2 - (coef * img.Height / 2),
                        coef * img.Width,
                        coef * img.Height,
                        0);
                    img.Dispose();
                    img = null;
                    graph.Dispose();
                    graph = null;


                    #region Clean File
                    File.Delete(workFile);
                    #endregion

                    #endregion

                    foreach (string indicatorName in _titleList)
                    {
                        if (indicatorName.Equals(GestionWeb.GetWebWord(1165, _webSession.SiteLanguage)))
                            isPresent = true;
                    }
                    if (!isPresent)
                        _titleList.Add(GestionWeb.GetWebWord(1165, _webSession.SiteLanguage));
                }
            }
            catch (System.Exception ex)
            {
                try
                {
                    sw.Close();
                    img.Dispose();
                    img = null;
                }
                catch (System.Exception e2) { }
                throw (new HotepPdfException("Impssoible to generate chart of Indicator Palmares.", ex));
            }

        }
        #endregion

        #region Indicator Novelty (similar to AdExpress)
        /// <summary>
        /// Indicator Novelty design
        /// </summary>
        protected override void IndicatorNovelty()
        {

            int verifResult = 0;

            try
            {

                IList result = new ArrayList();
                IList resultAdvertiser = new ArrayList();
                object[,] resultObject = null;

                #region result
                EngineNovelty engine = new TNS.AdExpressI.ProductClassIndicators.Russia.Engines.EngineNovelty(_webSession, _productClassIndicatorDAL);
                engine.ClassifLevel = CstResult.Novelty.ElementType.product;
                try
                {
                    resultObject = engine.GetData();
                }
                catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
                {
                    resultObject = null;
                }
                result = GetIndicatorNoveltyGraphicHtmlUI(resultObject, _webSession, CstResult.Novelty.ElementType.product);
                engine.ClassifLevel = CstResult.Novelty.ElementType.advertiser;
                try
                {
                    resultObject = engine.GetData();
                }
                catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
                {
                    resultObject = null;
                }
                resultAdvertiser = GetIndicatorNoveltyGraphicHtmlUI(resultObject, _webSession, CstResult.Novelty.ElementType.advertiser);
                #endregion

                #region Html file loading
                if (!((((string)result[0]).Length < 30) || result[0].Equals("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(1224, _webSession.SiteLanguage) + "</div>")))
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (i == 0)
                            addNoveltyPicture(result, i, true);
                        else
                            addNoveltyPicture(result, i, false);
                    }
                    verifResult++;
                }

                if (!(((string)resultAdvertiser[0]).Length < 50 || resultAdvertiser[0].Equals("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(1224, _webSession.SiteLanguage) + "</div>")))
                {
                    for (int i = 0; i < resultAdvertiser.Count; i++)
                    {
                        if ((i == 0) && (verifResult == 0))
                            addNoveltyPicture(resultAdvertiser, i, true);
                        else
                            addNoveltyPicture(resultAdvertiser, i, false);
                    }
                    verifResult++;
                }

                if (verifResult >= 1)
                {
                    _titleList.Add(GestionWeb.GetWebWord(1197, _webSession.SiteLanguage));
                }
                #endregion

            }
            catch (System.Exception e)
            {
                throw (new HotepPdfException("Unable to process the Indicator Novelty result for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
        }
        #endregion

        #region Indicator Evolution
        /// <summary>
        /// Graphiques Indicator Evolution
        /// </summary>
        protected override void IndicatorEvolution(FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType)
        {

            StreamWriter sw = null;
            Image img = null;
            bool isPresent = false;

            #region Load Data
            object[,] tab = null;
            EngineEvolution engine = new TNS.AdExpressI.ProductClassIndicators.Russia.Engines.EngineEvolution(_webSession, _productClassIndicatorDAL);
            tab = engine.GetData(tableType);
            #endregion

            #region Cas année N-2
            DateTime PeriodBeginningDate = TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
            #endregion

            try
            {
                if ((tab.GetLongLength(0) != 0) && (!((PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year - 2) && DateTime.Now.Year <= _webSession.DownLoadDate)
                    || (PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year - 3) && DateTime.Now.Year > _webSession.DownLoadDate))))
                {

                    this.NewPage();

                    this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

                    string workFile = Path.GetTempFileName();

                    #region Title
                    Style.GetTag("EvolutionGraphTitleFontPage").SetStylePdf(this, GetTxFontCharset());
                    this.PDFPAGE_UnicodeTextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1207, _webSession.SiteLanguage));
                    #endregion

                    #region GRP graph

                    UIEvolutionGraph graph = new UIEvolutionGraph(_webSession, _dataSource, _config, tab, Style);
                    graph.BuildEvolution(tableType);
                    graph.SaveAsImage(workFile, ChartImageFormat.Bmp);
                    img = Image.FromFile(workFile);
                    double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width / ((double)img.Width)));
                    coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40) / ((double)img.Height)));
                    int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
                    this.PDFPAGE_ShowImage(i,
                        (this.PDFPAGE_Width / 2) - (coef * img.Width / 2),
                        this.PDFPAGE_Height / 2 - (coef * img.Height / 2),
                        coef * img.Width,
                        coef * img.Height,
                        0);
                    img.Dispose();
                    img = null;
                    graph.Dispose();
                    graph = null;


                    #region Clean File
                    File.Delete(workFile);
                    #endregion

                    #endregion

                    foreach (string indicatorName in _titleList)
                    {
                        if (indicatorName.Equals(GestionWeb.GetWebWord(1207, _webSession.SiteLanguage)))
                            isPresent = true;
                    }
                    if (!isPresent)
                        _titleList.Add(GestionWeb.GetWebWord(1207, _webSession.SiteLanguage));
                }
            }
            catch (System.Exception ex)
            {
                try
                {
                    sw.Close();
                    img.Dispose();
                    img = null;
                }
                catch (System.Exception e2) { }
                throw (new HotepPdfException(" impossible to generate chart of Indicator Evolution.", ex));
            }
        }
        #endregion

        #region Indicator Media Strategy
        /// <summary>
        /// Graphiques Indicator Media Strategy
        /// </summary>
        protected override void IndicatorMediaStrategy()
        {
            CultureInfo cInfoSave = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo;



            StreamWriter sw = null;
            Image img = null;
            object[,] tab = null;
            EngineMediaStrategy engine = new EngineMediaStrategy(_webSession, _productClassIndicatorDAL);

            #region Load Data
            if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)
            {
                _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                tab = engine.GetChartData();
                _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;
            }
            else
            {
                tab = engine.GetChartData();
            }
            #endregion

            try
            {
                if (tab.GetLongLength(0) != 0)
                {
                    bool withPluriByCategory = (_webSession.PreformatedMediaDetail == CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory
                    && CstDbClassif.Vehicles.names.plurimedia == VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID));

                    #region GRP graph

                    #region Init Series

                    #region Constantes
                    //const int NBRE_MEDIA = 5;
                    #endregion

                    #region Niveau de détail
                    int MEDIA_LEVEL_NUMBER;
                    switch (_webSession.PreformatedMediaDetail)
                    {
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
                            MEDIA_LEVEL_NUMBER = 1;
                            break;
                        case CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
                            MEDIA_LEVEL_NUMBER = 2;
                            break;
                        default:
                            MEDIA_LEVEL_NUMBER = 3;
                            break;
                    }
                    #endregion

                    #region Variables
                    Dictionary<string, Series> listSeriesMedia = new Dictionary<string, Series>();
                    Dictionary<int, string> listSeriesName = new Dictionary<int, string>();
                    Dictionary<string, double> listSeriesMediaRefCompetitor = new Dictionary<string, double>();
                    Dictionary<string, DataTable> listTableRefCompetitor = new Dictionary<string, DataTable>();
                    bool universTotalVerif = false;
                    #endregion

                    if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal)
                    {
                        _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                        universTotalVerif = true;
                    }

                    #region Create Series
                    // Serie Univers
                    listSeriesMedia.Add(GestionWeb.GetWebWord(1780, _webSession.SiteLanguage), new Series());
                    listSeriesName.Add(0, GestionWeb.GetWebWord(1780, _webSession.SiteLanguage));
                    // Serie Famille
                    if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal)
                    {
                        listSeriesMedia.Add(GestionWeb.GetWebWord(1189, _webSession.SiteLanguage), new Series());
                        listSeriesName.Add(1, GestionWeb.GetWebWord(1189, _webSession.SiteLanguage));
                        // Serie Marché
                    }
                    else
                    {
                        listSeriesMedia.Add(GestionWeb.GetWebWord(1316, _webSession.SiteLanguage), new Series());
                        listSeriesName.Add(1, GestionWeb.GetWebWord(1316, _webSession.SiteLanguage));
                    }

                    // Create series (one per media)
                    CreatesSeries(tab, listSeriesMediaRefCompetitor, listTableRefCompetitor, listSeriesMedia);
                    #endregion

                    #region Totals
                    double totalUniversValue = 0;
                    double totalSectorValue = 0;
                    double totalMarketValue = 0;

                    ComputeTotals(tab, listSeriesMediaRefCompetitor, ref  totalUniversValue, ref  totalSectorValue, ref  totalMarketValue, MEDIA_LEVEL_NUMBER);
                    #endregion

                    #region Table

                    DataTable tableUnivers = new DataTable();
                    DataTable tableSectorMarket = new DataTable();
                    FillTable(tab, listSeriesMediaRefCompetitor, listTableRefCompetitor, tableUnivers, tableSectorMarket, ref  totalUniversValue, ref  totalSectorValue, ref  totalMarketValue, MEDIA_LEVEL_NUMBER, withPluriByCategory);
                    #endregion

                    #endregion

                    #region Init Series
                    string strSort = "Position  DESC";
                    DataRow[] foundRows = null;
                    foundRows = tableUnivers.Select("", strSort);
                    DataRow[] foundRowsSectorMarket = null;
                    foundRowsSectorMarket = tableSectorMarket.Select("", strSort);
                    double[] yValues = new double[foundRows.Length];
                    string[] xValues = new string[foundRows.Length];
                    double[] yValuesSectorMarket = new double[foundRowsSectorMarket.Length];
                    string[] xValuesSectorMarket = new string[foundRowsSectorMarket.Length];
                    InitSeries(tableUnivers, tableSectorMarket, yValues, xValues, yValuesSectorMarket, xValuesSectorMarket, listSeriesMedia, listTableRefCompetitor, listSeriesName, MEDIA_LEVEL_NUMBER);


                    if (universTotalVerif)
                        _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;

                    #endregion

                    Dictionary<string, Series> listSeriesMediaTemp = new Dictionary<string, Series>();
                    Dictionary<int, string> listSeriesNameTemp = new Dictionary<int, string>();
                    bool first = true;
                    for (int j = 0; j < listSeriesMedia.Count; j++)
                    {
                        listSeriesMediaTemp = new Dictionary<string, Series>();
                        listSeriesNameTemp = new Dictionary<int, string>();

                        for (int nbPie = 0; nbPie < 2 && j < listSeriesMedia.Count; nbPie++, j++)
                        {
                            while (listSeriesMedia[(string)listSeriesName[j]].Points.Count < 1 && j < listSeriesMedia.Count - 1) { j++; }
                            if (listSeriesMedia[(string)listSeriesName[j]].Points.Count > 0)
                            {
                                listSeriesMediaTemp.Add((string)listSeriesName[j], listSeriesMedia[(string)listSeriesName[j]]);
                                listSeriesNameTemp.Add(j, listSeriesName[j]);
                            }
                        }
                        if (listSeriesMediaTemp.Count > 0 && listSeriesNameTemp.Count > 0)
                        {
                            this.NewPage();

                            this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

                            string workFile = Path.GetTempFileName();


                            if (first)
                            {
                                #region Title
                                Style.GetTag("MediaStrategyGraphTitleFontPage").SetStylePdf(this, GetTxFontCharset());
                                this.PDFPAGE_UnicodeTextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1227, _webSession.SiteLanguage));
                                #endregion
                                first = false;
                            }


                            UIMediaStrategyGraph graph = new UIMediaStrategyGraph(_webSession, _dataSource, _config, Style, listSeriesMediaTemp, listSeriesNameTemp);
                            graph.BuildMediaStrategy();
                            graph.SaveAsImage(workFile, ChartImageFormat.Bmp);
                            img = Image.FromFile(workFile);
                            double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width / ((double)img.Width)));
                            coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40) / ((double)img.Height)));
                            int indexImg = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
                            this.PDFPAGE_ShowImage(indexImg,
                                (this.PDFPAGE_Width / 2) - (coef * img.Width / 2),
                                this.PDFPAGE_Height / 2 - (coef * img.Height / 2),
                                coef * img.Width,
                                coef * img.Height,
                                0);
                            img.Dispose();
                            img = null;
                            graph.Dispose();
                            graph = null;

                            #region Clean File
                            File.Delete(workFile);
                            #endregion

                            _titleList.Add(GestionWeb.GetWebWord(1227, _webSession.SiteLanguage));
                        }
                    }
                    #endregion
                }
            }
            catch (System.Exception ex)
            {
                try
                {
                    sw.Close();
                    img.Dispose();
                    img = null;
                }
                catch (System.Exception e2) { }
                throw (new HotepPdfException(" impossible de générer les graphiques Indicator Media Strategy.", ex));
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = cInfoSave;
        }
        #endregion

        #region CreatesSeries
        /// <summary>
        /// Creates Series
        /// </summary>
        /// <param name="tab">tabel data</param>
        /// <param name="listSeriesMediaRefCompetitor">list Series of Media References and Competitor</param>
        /// <param name="listTableRefCompetitor">list Table References and Competitor</param>
        /// <param name="listSeriesMedia"></param>
        protected override void CreatesSeries(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, Dictionary<string, DataTable> listTableRefCompetitor, Dictionary<string, Series> listSeriesMedia)
        {           
            // Create series (one per media)
            for (int i = 0; i < tab.GetLongLength(0); i++)
            {

                //	Dictionary with advertiser label as key and total as value
                if (!Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]))
                {
                    if (!listSeriesMediaRefCompetitor.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        listSeriesMediaRefCompetitor.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), new double());
                    }

                    if (!listTableRefCompetitor.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        DataTable tableCompetitorRef = new DataTable();
                        tableCompetitorRef.Columns.Add("Name");
                        tableCompetitorRef.Columns.Add("Position", typeof(double));
                        listTableRefCompetitor.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), tableCompetitorRef);

                    }

                    if (!listSeriesMedia.ContainsKey(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()))
                    {
                        listSeriesMedia.Add(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString(), new Series());
                    }
                }
            }
        }
        #endregion

        #region ComputeTotals
        /// <summary>
        /// Compute totals values
        /// </summary>
        /// <param name="tab">Table data</param>
        /// <param name="listSeriesMediaRefCompetitor">list Series of Media References and Competitor</param>
        /// <param name="totalUniversValue">total Univers Value</param>
        /// <param name="totalSectorValue">total Sector Value</param>
        /// <param name="totalMarketValue">total Market Value</param>
        /// <param name="MEDIA_LEVEL_NUMBER">MEDIA LEVEL NUMBER</param>
        protected override void ComputeTotals(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, ref double totalUniversValue, ref double totalSectorValue, ref double totalMarketValue, int MEDIA_LEVEL_NUMBER)
        {
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo;

            if (MEDIA_LEVEL_NUMBER > 0)
            {
                for (int i = 0; i < tab.GetLongLength(0); i++)
                {
                    for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                    {
                        switch (j)
                        {

                            #region support
                            // Univers Total
                            case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            #endregion

                            #region Advertisers
                            case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX])
                                    && !Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])
                                   )
                                {
                                    listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            #endregion

                            #region Category
                            // Univers Total	
                            case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:

                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX], fp);
                                }

                                break;
                            #endregion

                            #region PluriMedia
                            // Univers Total	
                            case EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX], fp);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:

                                if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX], fp);
                                }

                                break;
                            #endregion

                            default:
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Fill Table
        /// <summary>
        /// Fill Table
        /// </summary>
        /// <param name="tab">Table data</param>
        /// <param name="listSeriesMediaRefCompetitor">list Series of Media References and Competitor</param>
        /// <param name="listTableRefCompetitor">list Table References and Competitor</param>
        /// <param name="tableUnivers">table Univers</param>
        /// <param name="tableSectorMarket">table SectorMarket</param>
        /// <param name="totalUniversValue">total Univers Value</param>
        /// <param name="totalSectorValue">total Sector Value</param>
        /// <param name="totalMarketValue">total Market Value</param>
        /// <param name="MEDIA_LEVEL_NUMBER">MEDIA LEVEL NUMBER</param>
        /// <param name="withPluriByCategory">with Pluri By Category</param>
        protected override void FillTable(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, Dictionary<string, DataTable> listTableRefCompetitor, DataTable tableUnivers, DataTable tableSectorMarket, ref double totalUniversValue, ref double totalSectorValue, ref double totalMarketValue, int MEDIA_LEVEL_NUMBER, bool withPluriByCategory)
        {
            #region Table
            double elementValue;

            tableUnivers.Columns.Add("Name");
            tableUnivers.Columns.Add("Position", typeof(double));
            tableSectorMarket.Columns.Add("Name");
            tableSectorMarket.Columns.Add("Position", typeof(double));
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo;

            for (int i = 0; i < tab.GetLongLength(0); i++)
            {
                for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                {
                    switch (j)
                    {

                        #region Media
                        case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX], fp) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                                j = j + 6;
                            }

                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX], fp) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 5;
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX], fp) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 4;
                            }
                            break;
                        #endregion

                        #region Advertisers
                        case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX])
                                && !Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX])
                                && !Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])
                                && MEDIA_LEVEL_NUMBER == 3)
                            {

                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX], fp) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }

                                j = j + 12;
                            }
                            else if (!Functions.IsNull(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX])
                                && !Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX])
                                && Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX])
                                && !Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])
                                && MEDIA_LEVEL_NUMBER == 2)
                            {

                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX], fp) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            else if (!Functions.IsNull(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX])
                             && !Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX])
                             && !Functions.IsNull(tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX])
                             && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX], fp) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            break;
                        #endregion

                        #region Categorie
                        case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX], fp) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX], fp) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX], fp) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        #endregion

                        #region PluriMedia
                        case EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX], fp) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX], fp) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:
                            if (!Functions.IsNull(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]) && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX], fp) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"), fp);
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        #endregion

                        default:
                            break;
                    }
                }
            }
            #endregion
        }
        #endregion

        #region  Init Series
        protected override void InitSeries(DataTable tableUnivers, DataTable tableSectorMarket, double[] yValues, string[] xValues, double[] yValuesSectorMarket, string[] xValuesSectorMarket, Dictionary<string, Series> listSeriesMedia, Dictionary<string, DataTable> listTableRefCompetitor, Dictionary<int, string> listSeriesName, int MEDIA_LEVEL_NUMBER)
        {
            #region Init Series
            string strSort = "Position  DESC";
            DataRow[] foundRows = null;
            foundRows = tableUnivers.Select("", strSort);
            DataRow[] foundRowsSectorMarket = null;
            foundRowsSectorMarket = tableSectorMarket.Select("", strSort);
            double otherUniversValue = 0;
            double otherSectorMarketValue = 0;
            int index = 0;
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_webSession.DataLanguage].CultureInfo;


            for (int i = 0; i < foundRows.Length; i++)
            {
                xValues[i] = foundRows[i]["Name"].ToString();
                yValues[i] = Convert.ToDouble(foundRows[i]["Position"], fp);
                otherUniversValue += Convert.ToDouble(foundRows[i]["Position"], fp);
            }

            for (int i = 0; i < foundRowsSectorMarket.Length; i++)
            {
                xValuesSectorMarket[i] = foundRowsSectorMarket[i]["Name"].ToString();
                yValuesSectorMarket[i] = Convert.ToDouble(foundRowsSectorMarket[i]["Position"], fp);
                otherSectorMarketValue += Convert.ToDouble(foundRowsSectorMarket[i]["Position"], fp);
            }


            double[] yVal = new double[foundRows.Length];
            string[] xVal = new string[foundRows.Length];
            // double otherCompetitorRefValue = 0;
            int k = 2;

            foreach (string name in listSeriesMedia.Keys)
            {

                if (name == GestionWeb.GetWebWord(1780, _webSession.SiteLanguage))
                {
                    if (xValues != null && xValues.Length > 0 && xValues[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1780, _webSession.SiteLanguage)].Points.DataBindXY(xValues, yValues);
                }
                else if (_webSession.ComparaisonCriterion == CstComparisonCriterion.sectorTotal && name == GestionWeb.GetWebWord(1189, _webSession.SiteLanguage))
                {
                    if (xValuesSectorMarket != null && xValuesSectorMarket.Length > 0 && xValuesSectorMarket[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1189, _webSession.SiteLanguage)].Points.DataBindXY(xValuesSectorMarket, yValuesSectorMarket);
                }
                else if (name == GestionWeb.GetWebWord(1316, _webSession.SiteLanguage))
                {
                    if (xValuesSectorMarket != null && xValuesSectorMarket.Length > 0 && xValuesSectorMarket[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1316, _webSession.SiteLanguage)].Points.DataBindXY(xValuesSectorMarket, yValuesSectorMarket);
                }
                else
                {
                    DataRow[] foundRowsCompetitorRef = null;
                    foundRowsCompetitorRef = ((DataTable)listTableRefCompetitor[name]).Select("", strSort);
                    //otherCompetitorRefValue = 0;

                    yVal = new double[foundRowsCompetitorRef.Length];
                    xVal = new string[foundRowsCompetitorRef.Length];

                    for (int i = 0; i < foundRowsCompetitorRef.Length; i++)
                    {
                        xVal[i] = foundRowsCompetitorRef[i]["Name"].ToString();
                        yVal[i] = Convert.ToDouble(foundRowsCompetitorRef[i]["Position"], fp);

                    }

                    if (xVal.Length > 0 && xVal[0] != null)
                        listSeriesMedia[name].Points.DataBindXY(xVal, yVal);


                    listSeriesName.Add(k, name);
                    k++;
                }

            }
            #endregion
        }
        #endregion

        #region Détail Média
        protected override string GetMediaDetail(IList SelectionDetailMedia, bool first, int j)
        {

            StringBuilder html = new StringBuilder();
            // Détail référence média
            if (_webSession.isReferenceMediaSelected())
            {

                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                if (first)
                {
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1194, _webSession.SiteLanguage) + " :</TD>");
                    html.Append("<TR><TD>&nbsp;</TD>");
                    html.Append("</TR>");
                }
                html.Append("<TR>");
                html.Append("<TD align=\"center\">");
                html.Append(SelectionDetailMedia[j].ToString());
                html.Append("</TD>");
                html.Append("</TR>");
                html.Append("</TR>");
            }

            // Détail Média
            if (_webSession.SelectionUniversMedia.FirstNode != null && _webSession.SelectionUniversMedia.FirstNode.Nodes.Count > 0)
            {

                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                if (first)
                {
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1194, _webSession.SiteLanguage) + " :</TD>");
                    html.Append("<TR><TD>&nbsp;</TD>");
                    html.Append("</TR>");
                }
                html.Append("<TR>");
                html.Append("<TD align=\"center\">");
                html.Append(SelectionDetailMedia[j].ToString());
                html.Append("</TD>");
                html.Append("</TR>");
                html.Append("</TR>");
            }
            return html.ToString();
        }
        #endregion

        #region AddHeadersAndFooters
        /// <summary>
        /// Add Header and footer to the page between i and j
        /// </summary>
        /// <param name="position">Image position on the header</param>
        /// <param name="title">Title in the header</param>
        /// <param name="fromPage">Beginning page</param>
        /// <param name="toPage">End page</param>
        /// <param name="withCopyright">Indication du CopyRight</param>
        /// <param name="webSession">La WebSession</param>
        public override void AddHeadersAndFooters(WebSession webSession, imagePosition position, string title, int fromPage, int toPage, bool withCopyright)
        {
            try
            {
                if (fromPage < 0)
                    fromPage = 0;
                if (toPage < 0)
                    toPage = this.PageCount;

                for (int i = fromPage; i < toPage; i++)
                {
                    this.SetCurrentPage(i);
                    if (position == imagePosition.leftImage)
                        this.PDFPAGE_Watermark = GetWaterMark(true, false, title);
                    else if (position == imagePosition.rightImage)
                        this.PDFPAGE_Watermark = GetWaterMark(false, true, title);
                    //Page number					
                    string str = GestionWeb.GetWebWord(894, webSession.SiteLanguage) + " " + (i + 1) + " " + GestionWeb.GetWebWord(2042, webSession.SiteLanguage) + " " + this.PageCount;
                    _style.GetTag("footerFont").SetStylePdf(this, GetTxFontCharset());

                    this.PDFPAGE_UnicodeTextOut(
                        this.PDFPAGE_Width / 2 - (this.PDFPAGE_GetTextWidth(str) / 2)
                        , this.WorkZoneBottom + this._footerHeight / 2, 0, str);

                    if (withCopyright)
                    {
                        string strCopyright = GestionWeb.GetWebWord(2848, webSession.SiteLanguage) + " "
                     + WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CompanyNameTexts.GetCompanyShortName(webSession.SiteLanguage) + " "
                     + GestionWeb.GetWebWord(2849, webSession.SiteLanguage);
                        _style.GetTag("copyright").SetStylePdf(this, GetTxFontCharset());
                        this.PDFPAGE_UnicodeTextOut(

                            this.PDFPAGE_Width - this._rightMargin - ((this.PDFPAGE_GetTextWidth(strCopyright)))
                            , this.WorkZoneBottom + this._footerHeight / 2, 0, strCopyright);


                    }
                }
            }
            catch (System.Exception e)
            {
                throw (new PdfException("Unable to buil headers and footers", e));
            }

        }

        #endregion
    }
}
