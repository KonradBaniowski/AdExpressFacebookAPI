#region Information
//Author : Y. Rkaina 
//Creation : 11/07/2006
#endregion

#region Using
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Reflection;
using System.Windows.Forms;

using TNS.AdExpress.Anubis.Hotep.Common;
using TNS.AdExpress.Anubis.Hotep.Exceptions;
using TNS.AdExpress.Anubis.Hotep.UI;

using TNS.AdExpress.Constantes.Customer;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstDbClassif = TNS.AdExpress.Constantes.Classification.DB;

using TNS.AdExpress.Web.BusinessFacade.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.Exceptions;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;

using Dundas.Charting.WinControl;
using PDFCreatorPilotLib;
using TNS.FrameWork.DB.Common;
using System.Globalization;
using Oracle.DataAccess.Client;

using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpressI.ProductClassIndicators;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Exceptions;
using TNS.Ares.Pdf;
using TNS.Ares;
using TNS.FrameWork.WebTheme;
using System.Net.Mail;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Domain.Units;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
#endregion

namespace TNS.AdExpress.Anubis.Hotep.BusinessFacade{
	/// <summary>
	/// Generate the PDF document for Indicateurs module.
	/// </summary>
	public class HotepPdfSystem : Pdf{

        #region Constantes
       protected const int NBRE_MEDIA = 5;
        #endregion

		#region Variables
        protected IDataSource _dataSource = null;
		/// <summary>
		/// Appm Configuration (usefull for PDF layout)
		/// </summary>
        protected HotepConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
        protected DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
        protected WebSession _webSession = null;
		/// <summary>
		/// Liste des indicateurs (présents dans le PDF)
		/// </summary>
		protected ArrayList _titleList = null;
        /// <summary>
        /// Product Class Indicator
        /// </summary>
        protected IProductClassIndicators _productClassIndicator;
        /// <summary>
        /// Product Class Indicator DAL
        /// </summary>
        protected IProductClassIndicatorsDAL _productClassIndicatorDAL;
		#endregion

		#region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSource">DataSource</param>
        /// <param name="config">Config</param>
        /// <param name="rqDetails">Request details</param>
        /// <param name="webSession">Web session</param>
        public HotepPdfSystem(IDataSource dataSource, HotepConfig config, DataRow rqDetails, WebSession webSession, TNS.FrameWork.WebTheme.Theme theme)
            :
            base(theme.GetStyle("Hotep")) {
            try {
                this._dataSource = dataSource;
                this._config = config;
                this._rqDetails = rqDetails;
                this._webSession = webSession;
                this._titleList = new ArrayList();
                WebNavigation.Module module = WebNavigation.ModulesList.GetModule(CstWeb.Module.Name.INDICATEUR);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Indicator result"));
                object[] param = new object[1] { webSession };
                _productClassIndicator = (IProductClassIndicators)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                _productClassIndicator.Pdf = true;
                _productClassIndicatorDAL = (IProductClassIndicatorsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            }
            catch (Exception e) {
                throw new HotepPdfException("Error in constructor HotepPdfSystem", e);
            }
		}
		#endregion

		#region Init
        /// <summary>
        /// Init
        /// </summary>
        /// <returns>File name</returns>
		public virtual string Init(){
			try{
				string shortFName = "";
				string fName =  GetFileName(_rqDetails, ref shortFName);
				
                base.Init(true,fName,_config.PdfCreatorPilotLogin,_config.PdfCreatorPilotPass);
				this.DocumentInfo_Creator = this.DocumentInfo_Author = _config.PdfAuthor;
				this.DocumentInfo_Subject = _config.PdfSubject;
				this.DocumentInfo_Title = _config.PdfTitle;
				this.DocumentInfo_Producer = _config.PdfProducer;
				this.DocumentInfo_Keywords = _config.PdfKeyWords;
				return shortFName;
			}
			catch(System.Exception e){
                throw new HotepPdfException("Error for initialize HotepPdfSystem",e);
			}
		}
		#endregion

		#region Fill
        /// <summary>
        /// Fill
        /// </summary>
		public virtual void Fill(){

			try{

                bool evolution = true;
                VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
                if (vehicleInfo != null && (vehicleInfo.Id == Constantes.Classification.DB.Vehicles.names.plurimedia
                    || vehicleInfo.Id == Constantes.Classification.DB.Vehicles.names.PlurimediaWithoutMms) 
                    && WebApplicationParameters.HidePlurimediaEvol)
                    evolution = false;

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
                if (evolution)
                    IndicatorEvolution(CstResult.EvolutionRecap.ElementType.advertiser);
                #endregion

                #region IndicatorEvolution (referenceChart)
                if (evolution)
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
                GestionWeb.GetWebWord(1053, _webSession.SiteLanguage) + " - " + dateString,
                0, -1, true);
                #endregion

            }
            catch (Exception e) {
                throw new HotepPdfException("Error in Fill in HotepPdfSystem", e);
            }
		}
		#endregion

		#region Send
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="fileName">File name</param>
		public virtual  void Send(string fileName){
            try{
                ArrayList to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient) {
                    to.Add(s);
                }
                SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    GestionWeb.GetWebWord(1971, _webSession.SiteLanguage),
                    Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding),
                    GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + "\"" + _webSession.ExportedPDFFileName + "\""
                    + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer) + "<br><br>"
                    + String.Format(GestionWeb.GetWebWord(1776, _webSession.SiteLanguage), _config.WebServer),
                    Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);
                mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
                mail.SendWithoutThread(false);

            }
            catch (Exception e) {
                throw new HotepPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession , e);
            }
		}
		#endregion

		#region Méthode Interne

		#region File IO management

		#region GetFileName
		/// <summary>
		/// Generate a valid file name from customer request
		/// </summary>
		/// <param name="rqDetails">Details of the customer request</param>
		/// <param name="shortName">Return value : short name of the File (the method return the complet path)</param>
		/// <returns>Complet File Name String (path + short name)</returns>
		private string GetFileName(DataRow rqDetails,ref string shortName){
			string pdfFileName;

			try{

				pdfFileName = this._config.PdfPath;
				pdfFileName += @"\" + rqDetails["ID_LOGIN"].ToString() ;

				if(!Directory.Exists(pdfFileName)){
					Directory.CreateDirectory(pdfFileName);
				}
				shortName = DateTime.Now.ToString("yyyyMMdd_") 
					+ rqDetails["id_static_nav_session"].ToString()
					+ "_"
					+ TNS.Ares.Functions.GetRandomString(30,40);

				pdfFileName += @"\" + shortName + ".pdf";

				string checkPath = 	Regex.Replace(pdfFileName, @"(\.pdf)+", ".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);


				int i = 0;
				while(File.Exists(checkPath)){
					if(i<=1){
						checkPath = Regex.Replace(pdfFileName, @"(\.pdf)+", "_"+(i+1)+".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					else{
						checkPath = Regex.Replace(pdfFileName, "(_"+i+@"\.pdf)+", "_"+(i+1)+".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					i++;
				}
				return checkPath;
			}
			catch(System.Exception e){
				throw(new HotepPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
			}
		}
		#endregion

		#endregion

		#region MainPage
		/// <summary>
		/// Design Main Page
		/// </summary>
		/// <returns></returns>
		protected virtual void MainPageDesign(){

            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

            Picture picture = ((Picture)Style.GetTag("pictureTitle"));
            string imgPath = string.Empty;

            if (File.Exists(picture.Path)) {
                imgPath = picture.Path;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path)) {
                imgPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path;
            }
            else {
                imgPath = picture.Path;
            }


			Image imgG = Image.FromFile(imgPath);
			
			double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
			double coef = Math.Min((double)1.0,w);
			w = (double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height;
			coef = Math.Min((double)coef, w);
			
			int imgI = this.AddImageFromFilename(imgPath,TxImageCompressionType.itcFlate);

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
            html.Append("<P align=\"center\"><B><SPAN class=\"TreeTitleViolet40pt\">" + GestionWeb.GetWebWord(1053, _webSession.SiteLanguage) + "</SPAN></B><SPAN class=\"TreeTitleBlack10pt\">");
			html.Append("<o:p></o:p></SPAN></P>");
			html.Append("</TD>");
			html.Append("</TR>");
			html.Append("<TR>");
            html.Append("<TD><P align=\"center\"><SPAN class=\"TreeTitleViolet\">" + Convertion.ToHtmlString(str) + "</SPAN></P></TD>");
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
            html.Append("<TD colSpan=\"2\" class=\"violetBorderBottom\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1979, _webSession.SiteLanguage)) + "</TD>");
			html.Append("</TR>");
			html.Append("<TR>");
			html.Append("<TD>&nbsp;</TD>");
			html.Append("</TR>");
			html.Append("<TR>");
			int line=2;
			foreach(string indicatorName in _titleList){
				if((line%2==0)&&(line>2)){
					html.Append("</TR>");
					html.Append("<TR>");
				}
				html.Append("<TD><li style=\"LIST-STYLE-TYPE: square\">"+indicatorName+"</li></TD>");
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
				(double)(this.PDFPAGE_Width/2 - coef*imgG.Width/2), (double)(this.WorkZoneBottom - coef*imgG.Height - 50),
				(double)(coef*imgG.Width),(double)(coef*imgG.Height),0);

			#endregion

		}
		#endregion

		#region SessionParameter
		/// <summary>
		/// Session parameter design
		/// </summary>
		protected virtual void SessionParameter(){

			int nbLinesEnd=0;
			int j=0;
            StringBuilder html = new StringBuilder();
			bool showProductSelection = false;
			IList nbLinesSelectionMedia=new ArrayList(), nbLinesSelectionDetailMedia=new ArrayList(), nbLinesSelectionProduct=new ArrayList();

			try{
				IList SelectionMedia = ToHtml(_webSession.SelectionUniversMedia,false,false,600,false,_webSession.SiteLanguage,2,1,true,20,ref nbLinesEnd,ref nbLinesSelectionMedia);
				nbLinesEnd=27;

				IList SelectionDetailMedia =ToHtml((TreeNode)_webSession.SelectionUniversMedia.FirstNode,true,true,600,true,_webSession.SiteLanguage,3,1,false,21,ref nbLinesEnd,ref nbLinesSelectionDetailMedia);
				if(nbLinesSelectionDetailMedia.Count==0)
					nbLinesEnd=21;
				else if(nbLinesSelectionDetailMedia.Count==1){
					nbLinesEnd=17-(int)nbLinesSelectionDetailMedia[0];
					if(nbLinesEnd<0)
						nbLinesEnd=28;
				}
				else
					nbLinesEnd=27-(int)nbLinesSelectionDetailMedia[nbLinesSelectionDetailMedia.Count-1];

				#region Title
				html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				html.Append("<TR height=\"25\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"14\">");
                html.Append("<TD class=\"TreeTitleViolet20px\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752, _webSession.SiteLanguage)) + "</TD>");
				html.Append("</TR>");
				#endregion

				#region Etude comparative
				if(_webSession.ComparativeStudy){
					html.Append("<TR height=\"7\">");
					html.Append("<TD></TD>");
					html.Append("</TR>");
					html.Append("<TR height=\"1\" class=\"lightPurple\">");
					html.Append("<TD></TD>");
					html.Append("</TR>");
					html.Append("<TR>");
					html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1118, _webSession.SiteLanguage)) + " </TD>");
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
				html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1755, _webSession.SiteLanguage)) + " :</TD>");
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
				html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1292, _webSession.SiteLanguage)) + " :</TD>");
				html.Append("<TR>");
                html.Append("<TD align=\"left\">");
				html.Append(SelectionMedia[0].ToString());
				html.Append("</TD>");
				html.Append("</TR>");
				html.Append("</TR>");
				#endregion

				if (SelectionDetailMedia !=null && SelectionDetailMedia.Count > 0) {

					#region Détail Média
				    html.Append(GetMediaDetail(SelectionDetailMedia,true,0));
					for(j=1; j<SelectionDetailMedia.Count; j++){

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
				if (SelectionDetailMedia == null || SelectionDetailMedia.Count == 0) {
					html.Append("<TR height=\"7\">");
					html.Append("<TD></TD>");
					html.Append("</TR>");
					html.Append("<TR height=\"1\" class=\"lightPurple\">");
					html.Append("<TD></TD>");
					html.Append("</TR>");
					html.Append("<TR>");
					html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1795, _webSession.SiteLanguage)) + " :</TD>");
					html.Append("</TR>");
					html.Append("<TR height=\"20\">");
					html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                        + GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage)
						+ "</TD>");
					html.Append("</TR>");
				}
				#endregion
				
				#region produit
				// Détail produit
				if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0) {
					int nbLineByPage = 43;
					int currentLine = 13;
					if (_webSession.ComparativeStudy) currentLine = currentLine + 3;
					if (SelectionDetailMedia == null || SelectionDetailMedia.Count == 0) currentLine = currentLine + 4;//6
					if (SelectionDetailMedia != null && SelectionDetailMedia.Count > 0) {
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
					html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage)) + " :</TD>");
					html.Append("<TR><TD>&nbsp;</TD>");
					html.Append("</TR>");
					html.Append("<TR>");
					html.Append("<TD align=\"center\">");

                    html.Append(Convertion.ToHtmlString(TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource, 600, true, nbLineByPage, ref currentLine)));
					showProductSelection = true;

					if (showProductSelection) {
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
				if (SelectionDetailMedia != null && SelectionDetailMedia.Count > 0) {
					html.Append("<TR height=\"7\">");
					html.Append("<TD></TD>");
					html.Append("</TR>");
					html.Append("<TR height=\"1\" class=\"lightPurple\">");
					html.Append("<TD></TD>");
					html.Append("</TR>");
					html.Append("<TR>");
					html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1795, _webSession.SiteLanguage)) + " :</TD>");
					html.Append("</TR>");
					html.Append("<TR height=\"20\">");
					html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                        + GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage)
						+ "</TD>");
					html.Append("</TR>");
				}
				#endregion

                #region Search Legend
                string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                if (vehicleSelection.Length > 0 && VehiclesInformation.Contains(ClassificationCst.DB.Vehicles.names.search)
                                && vehicleSelection.Contains(VehiclesInformation.Get(ClassificationCst.DB.Vehicles.names.search).DatabaseId.ToString())) {
                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(3012, _webSession.SiteLanguage)) + "</TD>");
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


				_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1752,_webSession.SiteLanguage)));
			
			}
			catch(System.Exception e){
				throw(new HotepPdfException("Unable to build the session parameter page for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e));
			}
		}
		#endregion

		#region Indicator Synthesis (similar to AdExpress)
		/// <summary>
		/// Indicator Synthesis design
		/// </summary>
		protected virtual void IndicatorSynthesis(){

            StringBuilder html = new StringBuilder();

			try{
                html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");

				#region Title
				html.Append("<TR height=\"25\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"14\">");
                html.Append("<TD class=\"TreeTitleViolet20px\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1664, _webSession.SiteLanguage)) + "</TD>");
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


				_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1664,_webSession.SiteLanguage)));
			
			}
			catch(System.Exception e){
				throw(new HotepPdfException("Unable to process the synthesis result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e)); 
			}
		}
		#endregion

		#region Indicator Seasonality
		/// <summary>
		/// Graphiques Indicator Seasonality
		/// </summary>
		protected virtual void IndicatorSeasonality(){

			StreamWriter sw = null;
			Image img = null;
			object[,] tab=null;
            EngineSeasonality engine = new EngineSeasonality(_webSession, _productClassIndicatorDAL);
		
			#region Load Data
			if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal){
				_webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                tab = engine.GetChartData();
				_webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;}
			else{
                tab = engine.GetChartData();
			}
			#endregion

			try{
				if(tab.Length!=0){

					this.NewPage();

					this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

					string workFile = Path.GetTempFileName();

					#region Title
                    Style.GetTag("SeasonalityGraphTitleFontPage").SetStylePdf(this, GetTxFontCharset());
					this.PDFPAGE_UnicodeTextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1139 ,_webSession.SiteLanguage));
					#endregion

					#region GRP graph
			
					UISeasonalityGraph graph = new UISeasonalityGraph(_webSession, _dataSource, _config, tab,Style);
					graph.BuildSeasonality();
					graph.SaveAsImage(workFile,ChartImageFormat.Bmp);
					img = Image.FromFile(workFile);
					double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width/((double)img.Width)));
					coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40)/((double)img.Height)));
					int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
					this.PDFPAGE_ShowImage(i, 
						(this.PDFPAGE_Width / 2) - (coef * img.Width /2),
						this.PDFPAGE_Height/2 - (coef * img.Height / 2),
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

					_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1139,_webSession.SiteLanguage)));

				}

			}
			catch(System.Exception ex){
				try{
					sw.Close();
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException("impossible de générer les graphiques Indicator Seasonality.",ex)); 
			}
			
		}
		#endregion

		#region Indicator Palmares
		/// <summary>
		/// Graphiques Indicator Palmares
		/// </summary>
		protected virtual  void IndicatorPalmares(FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType){

			StreamWriter sw = null;
			Image img = null;
			bool isPresent=false;
		
			#region Load Data
            object[,] tab = null;
            EngineTop engine = new EngineTop(_webSession, _productClassIndicatorDAL);
            tab = engine.GetData(CstResult.PalmaresRecap.typeYearSelected.currentYear, tableType);
			#endregion

			try{
				if(tab.GetLongLength(0)!=1 && double.Parse(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString())!=0){

					this.NewPage();

					this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

					string workFile = Path.GetTempFileName();

					#region Title
                    Style.GetTag("PalmaresGraphTitleFontPage").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
					this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1980 ,_webSession.SiteLanguage));
					#endregion

					#region GRP graph
			
					UIPalmaresGraph graph = new UIPalmaresGraph(_webSession, _dataSource, _config, tab,Style);
					graph.BuildPalmares(tableType);
					graph.SaveAsImage(workFile,ChartImageFormat.Bmp);
					img = Image.FromFile(workFile);
					double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width/((double)img.Width)));
					coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40)/((double)img.Height)));
					int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
					this.PDFPAGE_ShowImage(i, 
						(this.PDFPAGE_Width / 2) - (coef * img.Width /2),
						this.PDFPAGE_Height/2 - (coef * img.Height / 2),
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

					foreach(string indicatorName in _titleList){
						if(indicatorName.Equals(Convertion.ToHtmlString(GestionWeb.GetWebWord(1165,_webSession.SiteLanguage))))
							isPresent=true;
					}
					if(!isPresent)
					_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1165,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception ex){
				try{
					sw.Close();
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException("impossible de générer les graphiques Indicator Palmares.",ex)); 
			}
			
		}
		#endregion

		#region Indicator Novelty (similar to AdExpress)
		/// <summary>
		/// Indicator Novelty design
		/// </summary>
		protected virtual void IndicatorNovelty(){
			
			int verifResult=0;

            try {

                IList result = new ArrayList();
                IList resultAdvertiser = new ArrayList();
                object[,] resultObject = null;

                #region result
                EngineNovelty engine = new EngineNovelty(_webSession, _productClassIndicatorDAL);
                engine.ClassifLevel = CstResult.Novelty.ElementType.product;
                try {
                    resultObject = engine.GetData();
                }
                catch (TNS.AdExpress.Domain.Exceptions.NoDataException) {
                    resultObject = null;
                }
                result = GetIndicatorNoveltyGraphicHtmlUI(resultObject, _webSession, CstResult.Novelty.ElementType.product);
                engine.ClassifLevel = CstResult.Novelty.ElementType.advertiser;
                try {
                    resultObject = engine.GetData();
                }
                catch (TNS.AdExpress.Domain.Exceptions.NoDataException) {
                    resultObject = null;
                }
                resultAdvertiser = GetIndicatorNoveltyGraphicHtmlUI(resultObject, _webSession, CstResult.Novelty.ElementType.advertiser);
                #endregion

                #region Html file loading
                if (!((((string)result[0]).Length < 30) || result[0].Equals("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(1224, _webSession.SiteLanguage) + "</div>"))) {
                    for (int i = 0; i < result.Count; i++) {
                        if (i == 0)
                            addNoveltyPicture(result, i, true);
                        else
                            addNoveltyPicture(result, i, false);
                    }
                    verifResult++;
                }

                if (!(((string)resultAdvertiser[0]).Length < 50 || resultAdvertiser[0].Equals("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(1224, _webSession.SiteLanguage) + "</div>"))) {
                    for (int i = 0; i < resultAdvertiser.Count; i++) {
                        if ((i == 0) && (verifResult == 0))
                            addNoveltyPicture(resultAdvertiser, i, true);
                        else
                            addNoveltyPicture(resultAdvertiser, i, false);
                    }
                    verifResult++;
                }

                if (verifResult >= 1) {
                    _titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1197, _webSession.SiteLanguage)));
                }
                #endregion

            }
            catch (System.Exception e) {
                throw (new HotepPdfException("Unable to process the Indicator Novelty result for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
		}
		#endregion

		#region Indicator Evolution
		/// <summary>
		/// Graphiques Indicator Evolution
		/// </summary>
		protected virtual void IndicatorEvolution(FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType){

			StreamWriter sw = null;
			Image img = null;
			bool isPresent=false;
		
			#region Load Data
            object[,] tab = null;
            EngineEvolution engine = new EngineEvolution(_webSession, _productClassIndicatorDAL);
            tab = engine.GetData(tableType);
			#endregion

			#region Cas année N-2
			DateTime PeriodBeginningDate = TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
			#endregion

			try{
                if ((tab!=null && tab.GetLongLength(0) != 0) && (!((PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear - 1)) && DateTime.Now.Year <= _webSession.DownLoadDate)
                    || (PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear) && DateTime.Now.Year > _webSession.DownLoadDate)))) {

					this.NewPage();

					this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

					string workFile = Path.GetTempFileName();

					#region Title
                    Style.GetTag("EvolutionGraphTitleFontPage").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
					this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1207 ,_webSession.SiteLanguage));
					#endregion

					#region GRP graph
			
					UIEvolutionGraph graph = new UIEvolutionGraph(_webSession, _dataSource, _config, tab,Style);
					graph.BuildEvolution(tableType);
					graph.SaveAsImage(workFile,ChartImageFormat.Bmp);
					img = Image.FromFile(workFile);
					double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width/((double)img.Width)));
					coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40)/((double)img.Height)));
					int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
					this.PDFPAGE_ShowImage(i, 
						(this.PDFPAGE_Width / 2) - (coef * img.Width /2),
						this.PDFPAGE_Height/2 - (coef * img.Height / 2),
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

					foreach(string indicatorName in _titleList){
						if(indicatorName.Equals(Convertion.ToHtmlString(GestionWeb.GetWebWord(1207,_webSession.SiteLanguage))))
							isPresent=true;
					}
					if(!isPresent)
						_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1207,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception ex){
				try{
					sw.Close();
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException(" impossible de générer les graphiques Indicator Evolution.",ex)); 
			}
		}
		#endregion

		#region Indicator Media Strategy
		/// <summary>
		/// Graphiques Indicator Media Strategy
		/// </summary>
		protected virtual void IndicatorMediaStrategy(){

			StreamWriter sw = null;
			Image img = null;
			object [,] tab=null;
            EngineMediaStrategy engine = new EngineMediaStrategy(_webSession, _productClassIndicatorDAL);
		
			#region Load Data
			if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal){
				_webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                tab = engine.GetChartData();
				_webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;}
			else{
                tab = engine.GetChartData();
			}
			#endregion

			try{
                if (tab.GetLongLength(0) != 0) {
					bool withPluriByCategory = (_webSession.PreformatedMediaDetail == CstPreformatedDetail.PreformatedMediaDetails.vehicleCategory
                    && (CstDbClassif.Vehicles.names.plurimedia == VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID)
                    || CstDbClassif.Vehicles.names.PlurimediaWithoutMms == VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID)));

                    #region GRP graph

                    #region Init Series

                    #region Constantes
                    //const int NBRE_MEDIA = 5;
                    #endregion

                    #region Niveau de détail
                    int MEDIA_LEVEL_NUMBER;
                    switch (_webSession.PreformatedMediaDetail) {
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

                    if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal) {
                        _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                        universTotalVerif = true;
                    }

                    #region Create Series
                    // Serie Univers
                    listSeriesMedia.Add(GestionWeb.GetWebWord(1780, _webSession.SiteLanguage), new Series());
                    listSeriesName.Add(0, GestionWeb.GetWebWord(1780, _webSession.SiteLanguage));
                    // Serie Famille
                    if (_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal) {
                        listSeriesMedia.Add(GestionWeb.GetWebWord(1189, _webSession.SiteLanguage), new Series());
                        listSeriesName.Add(1, GestionWeb.GetWebWord(1189, _webSession.SiteLanguage));
                        // Serie Marché
                    }
                    else {
                        listSeriesMedia.Add(GestionWeb.GetWebWord(1316, _webSession.SiteLanguage), new Series());
                        listSeriesName.Add(1, GestionWeb.GetWebWord(1316, _webSession.SiteLanguage));
                    }

                    // Create series (one per media)
                    CreatesSeries(tab,  listSeriesMediaRefCompetitor,  listTableRefCompetitor,  listSeriesMedia);
                    #endregion

                    #region Totals
                    double totalUniversValue = 0;
                    double totalSectorValue = 0;
                    double totalMarketValue = 0;

                     ComputeTotals( tab,  listSeriesMediaRefCompetitor, ref  totalUniversValue, ref  totalSectorValue, ref  totalMarketValue,  MEDIA_LEVEL_NUMBER);
                    #endregion

                    #region Table
                    
                    DataTable tableUnivers = new DataTable();
                    DataTable tableSectorMarket = new DataTable();
                    FillTable( tab,  listSeriesMediaRefCompetitor, listTableRefCompetitor,  tableUnivers,  tableSectorMarket, ref  totalUniversValue, ref  totalSectorValue, ref  totalMarketValue,  MEDIA_LEVEL_NUMBER,  withPluriByCategory);
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
                    InitSeries( tableUnivers,  tableSectorMarket,  yValues,  xValues,  yValuesSectorMarket,  xValuesSectorMarket, listSeriesMedia, listTableRefCompetitor,  listSeriesName,  MEDIA_LEVEL_NUMBER);
                            

                    if (universTotalVerif)
                        _webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;

                    #endregion

                    Dictionary<string, Series> listSeriesMediaTemp = new Dictionary<string, Series>();
                    Dictionary<int, string> listSeriesNameTemp = new Dictionary<int, string>();
                    bool first = true;
                    for (int j = 0; j < listSeriesMedia.Count; j++) {
                        listSeriesMediaTemp = new Dictionary<string, Series>();
                        listSeriesNameTemp = new Dictionary<int, string>();

                        for (int nbPie = 0; nbPie < 2 && j < listSeriesMedia.Count; nbPie++, j++) {
                            while (listSeriesMedia[(string)listSeriesName[j]].Points.Count < 1 && j < listSeriesMedia.Count-1) { j++; }
                            if (listSeriesMedia[(string)listSeriesName[j]].Points.Count > 0) {
                                listSeriesMediaTemp.Add((string)listSeriesName[j], listSeriesMedia[(string)listSeriesName[j]]);
                                listSeriesNameTemp.Add(j, listSeriesName[j]);
                            }
                        }
                        if (listSeriesMediaTemp.Count > 0 && listSeriesNameTemp.Count > 0) {
                            this.NewPage();

                            this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

                            string workFile = Path.GetTempFileName();


                            if (first) {
                                #region Title
                                Style.GetTag("MediaStrategyGraphTitleFontPage").SetStylePdf(this, GetTxFontCharset());
                                this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1227, _webSession.SiteLanguage));
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

                            _titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1227, _webSession.SiteLanguage)));
                        }
                    }
                    #endregion
                }
			}
			catch(System.Exception ex){
				try{
					sw.Close();
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException(" impossible de générer les graphiques Indicator Media Strategy.",ex)); 
			}
		}
		#endregion

		#region Aucun Résultat
		/// <summary>
		/// Affichage d'un message d'erreur
		/// </summary>
		/// <returns></returns>
		private string noResult(string message){
			System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
			t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
			t.Append("</td></tr></table>");
			return t.ToString();
		} 
		#endregion

		#region Affichage d'un arbre
		/// <summary>
		///  Affichage d'un arbre au format HTML
		/// </summary>
		/// <param name="root">Arbre à afficher</param>
		/// <param name="write">Arbre en lecture où en écriture</param>
		/// <param name="displayArrow">Affichage de la flêche</param>
		/// <param name="displayCheckbox">Affichage de la checkbox</param>
		/// <param name="witdhTable">Largeur de la table</param>
		/// <param name="displayBorderTable">Affichage de la bordure</param>
		/// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
		/// <param name="SiteLanguage">Langue</param>
		/// <param name="showHideContent">Index ShowHideCOntent ?</param>
		/// <param name="typetree">Type d'arbre ?</param>
		/// <param name="div">Afficher les div true si c'est le cas</param>
		/// <returns>tableau correspondant à l'arbre</returns>
		protected virtual IList ToHtml(TreeNode root,bool displayArrow,bool displayCheckbox,int witdhTable,bool displayBorderTable,int SiteLanguage,int typetree,int showHideContent,bool div,int nbLinesStart, ref int nbLinesEnd,ref IList nbLinesSelection){
		
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			int i=0;
			int start=0;
			int colonne=0;
			string buttonAutomaticChecked="";
			string disabled="";
			string tmp="";
			int j=0;
			int nbLines=0;
			IList htmlTableList = new ArrayList();
			int debut=0;

			foreach(TreeNode currentNode in root.Nodes){
				if(start==0){
					if(displayBorderTable){
                        t.Append("<table align=\"center\" class=\"TreeHeaderVioletBorder\"  width=" + witdhTable + "  >");
						start=1;
					}
					else{
                        t.Append("<table class=\"TreeHeaderBlancBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + ">");
					}

				}
				else{
					if(displayBorderTable){
                        t.Append("<table align=\"center\"  class=\"TreeHeaderVioletBorderWithoutTop\" width=" + witdhTable + ">");
					} 
					else{
                        t.Append("<table align=\"center\" class=\"TreeHeaderBlancBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + ">");
					}
				}
				t.Append("<tr>");
				t.Append("<td align=\"left\" height=\"10\"  valign=\"middle\" nowrap>");		
				if(displayCheckbox){
					//En lecture et Non cocher
					if(!currentNode.Checked){
						disabled="disabled";
						buttonAutomaticChecked="";
						if(typetree==2){
							t.Append("<input type=\"checkbox\"  "+disabled+" "+buttonAutomaticChecked+"  ID=\""+((LevelInformation)currentNode.Tag).ID+"\" value=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\" name=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">");
						}
						else if(typetree==3){ 
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
						else{
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" onclick=\"integration2('"+((LevelInformation)currentNode.Tag).ID+"',"+j+")\" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
					}
					//En lecture et cocher
					else if(currentNode.Checked){
						disabled="disabled";
						buttonAutomaticChecked="checked";
						if(typetree==2){
							t.Append("<input type=\"checkbox\"  "+disabled+" "+buttonAutomaticChecked+"  ID=\""+((LevelInformation)currentNode.Tag).ID+"\" value=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\" name=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">");
						}
						else if(typetree==3){
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
						else{
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" onclick=\"integration2('"+((LevelInformation)currentNode.Tag).ID+"',"+j+")\" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
					}
				}
				else{t.Append("&nbsp;");}
				t.Append(""+((LevelInformation)currentNode.Tag).Text+"");
				t.Append("</label>");
				t.Append("</td>");
				t.Append("<td width=\"15\"></td>");
				t.Append("</tr>");
				t.Append("</table>");

				nbLines++;

				#region découpage au niveau du père
				if ((nbLines >= nbLinesStart)&&(debut==0))
				{
					htmlTableList.Add(t.ToString());
					nbLinesSelection.Add(nbLines);
					debut=1;
					nbLines=0;
					start=0;
					t = new System.Text.StringBuilder(1000);
				}
				else if(nbLines == 30)
				{
					htmlTableList.Add(t.ToString());
					nbLinesSelection.Add(nbLines);
					nbLines=0;
					start=0;
					t = new System.Text.StringBuilder(1000);
				}
				#endregion
				
				if(currentNode.Nodes.Count>0)
				{
					if(displayBorderTable){
						if(div){
                            t.Append("<div id=\"" + ((LevelInformation)currentNode.Tag).ID + "Content" + showHideContent + "\" class=\"BlancBorderColorWithoutTop\"  style=\"DISPLAY: none; WIDTH: 100%\">");
						}
                        t.Append("<table align=\"center\"  class=\"TreeTableVioletBorder\" width=" + witdhTable + ">");
					}
					else{
						if(div){
                            t.Append("<div calss=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\">");
						}
                        t.Append("<table align=\"center\"  class=\"TreeTableBlancBorder\" width=" + witdhTable + ">");
					}
				}

				colonne=0;
				i=0;
			
				while(i<currentNode.Nodes.Count){
					
					if(displayArrow){
						//En lecture et Non cocher
						if(!currentNode.Nodes[i].Checked){
							disabled="disabled";
							buttonAutomaticChecked="";
							if(typetree==2){
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\""+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"\"  value=\""+((LevelInformation)currentNode.Nodes[i].Tag).ID+"_"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"\" name=\"CKB_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br>";
							}
							else{
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
								j++;
							}
						}
						//En lecture et cocher
						else if(currentNode.Nodes[i].Checked){
							disabled="disabled";
							buttonAutomaticChecked="checked";
							if(typetree==2){
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\""+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"\"  value=\""+((LevelInformation)currentNode.Nodes[i].Tag).ID+"_"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"\" name=\"CKB_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br>";
							}
							else{
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
								j++;
							}
						
						}
				
					}
					else{t.Append("&nbsp;");}

					if(colonne==2){
								
						t.Append("<td class=\"txtViolet10\" width=33%>");						
						t.Append(tmp);						
						t.Append("</td>");
						colonne=1;
					}
					else if(colonne==1){
						t.Append("<td class=\"txtViolet10\" width=33%>");								
						t.Append(tmp);
						t.Append("</td>");								
						t.Append("</tr>");
						colonne=0;
					}
					else{
						t.Append("<tr>");
						t.Append("<td class=\"txtViolet10\" width=33%>");
						t.Append(tmp);
						t.Append("</td>");								
						colonne=2;
						nbLines++;
					}

					i++;

					#region découpage du code HTML au niveau des fils
					if (((nbLines >= nbLinesStart)&&(debut==0)&&(colonne==0))||((nbLines >= nbLinesStart)&&(debut==0)&&(i>=currentNode.Nodes.Count))){
						
						t.Append("</table>");
						
						htmlTableList.Add(t.ToString());
						nbLinesSelection.Add(nbLines);
						debut=1;
						nbLines=0;
						start=0;
						t = new System.Text.StringBuilder(1000);

                        t.Append("<table align=\"center\" class=\"TreeHeaderBlancBorder\" style=\"WIDTH: 100%\">");

						if(currentNode.Nodes.Count>0)
						{
							if(displayBorderTable)
							{
								if(div)
								{
                                    t.Append("<div  id=\"" + ((LevelInformation)currentNode.Tag).ID + "Content" + showHideContent + "\" class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\">");
								}
                                t.Append("<table align=\"center\" class=\"TreeTableVioletBorder\"  width=" + witdhTable + ">");
							}
							else
							{
								if(div)
								{
                                    t.Append("<div class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\">");
								}
                                t.Append("<table align=\"center\" class=\"TreeTableBlancBorder\" width=" + witdhTable + ">");
							}
						}
					}
					else if(((nbLines == 30)&&(colonne==0))||((nbLines == 30)&&(i>=currentNode.Nodes.Count)))
					{
						t.Append("</table>");
							
						htmlTableList.Add(t.ToString());
						nbLinesSelection.Add(nbLines);
						nbLines=0;
						start=0;
						t = new System.Text.StringBuilder(1000);

                        t.Append("<table align=\"center\" class=\"TreeTableBlancBorder\"  style=\"WIDTH: 100%\">");

						if(currentNode.Nodes.Count>0)
						{
							if(displayBorderTable)
							{
								if(div)
								{
                                    t.Append("<div  id=\"" + ((LevelInformation)currentNode.Tag).ID + "Content" + showHideContent + "\" class=\"BlancBorderColorWithoutTop\"  style=\"DISPLAY: none; WIDTH: 100%\">");
								}
                                t.Append("<table align=\"center\" class=\"TreeTableVioletBorder\" width=" + witdhTable + ">");
							}
							else
							{
								if(div)
								{
                                    t.Append("<div class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\">");
								}
                                t.Append("<table align=\"center\" class=\"TreeTableBlancBorder\" width=" + witdhTable + ">");
							}
						}
					}
					#endregion

				}	
				if(currentNode.Nodes.Count>0){
					t.Append("</table>");
					if(div){
						t.Append("</div>");				
					}
				}
				
			}

			if((nbLines>0)&&(nbLines<30)){
				htmlTableList.Add(t.ToString());
				nbLinesSelection.Add(nbLines);
			}

			return htmlTableList;
		}
		#endregion

		#region Détail Média
		protected virtual string GetMediaDetail(IList SelectionDetailMedia,bool first,int j){

            StringBuilder html = new StringBuilder();
			// Détail référence média
			if(_webSession.isReferenceMediaSelected()){
							
				html.Append("<TR height=\"7\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"1\" class=\"lightPurple\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				if(first){
					html.Append("<TR>");
					html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" +Convertion.ToHtmlString(GestionWeb.GetWebWord(1194,_webSession.SiteLanguage))+" :</TD>");
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
			if(_webSession.SelectionUniversMedia.FirstNode!=null && _webSession.SelectionUniversMedia.FirstNode.Nodes.Count>0){
			
				html.Append("<TR height=\"7\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"1\" class=\"lightPurple\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				if(first){
					html.Append("<TR>");
					html.Append("<TD class=\"txtViolet11Bold\">&nbsp;"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1194,_webSession.SiteLanguage))+" :</TD>");
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

		#region Sortie HTML
		/// <summary>
		/// Crée le code HTML pour l'affichage du tableau (version graphique) de résultats qui permettra de détecter les réels nouveaux produits
		/// ou annonceurs des démarrages de campagne. Par nouveau il faut comprendre, un annonceur ou produit actif sur le
		/// dernier mois , mais inactif (pas d'investissement) depuis le début de l'anné.
		/// </summary>
		/// <param name="page">Page qui affiche les nouveautés</param>
		/// <param name="tab">tableau de résultats </param>
		/// <param name="webSession">session client</param>
		/// <param name="elementType">élément annonceur ou référence</param>
		/// <returns>Code</returns>
		//public static string GetIndicatorNoveltyGraphicHtmlUI(Page page,object[,] tab,WebSession webSession,ConstResults.Novelty.ElementType elementType){
		protected virtual IList GetIndicatorNoveltyGraphicHtmlUI(object[,] tab,WebSession webSession,CstResult.Novelty.ElementType elementType){
	
			#region variables
			string currentMonth="";
			bool PreviousYearActiveMonth=false;
			DateTime currentMonthDate;			
			string classe="";
			string classe2="";
			string classe3="";
			string pluszero="";
			string AdvertiserAccessList="";
			string[] AdvertiserSplitter={","};
			string CompetitorAdvertiserAccessList="";			
			ArrayList AdvertiserAccessListArr=null;
			ArrayList CompetitorAdvertiserAccessListArr=null;
			string IdElementToPersonnalize="";
            UnitInformation selectedCurrency = _webSession.GetSelectedUnit();
			#endregion

			#region variable pour le découpage du code HTML
			int nbLines=0;
			IList htmlTableList = new ArrayList();
			#endregion

			#region Pas de données à afficher
            if (tab==null || tab.GetLength(0) == 0)
			{
				if(elementType==CstResult.Novelty.ElementType.product){
					htmlTableList.Add(GestionWeb.GetWebWord(1238,webSession.SiteLanguage));
					return(htmlTableList);
				}
				else{
					htmlTableList.Add(GestionWeb.GetWebWord(1239,webSession.SiteLanguage));
					return(htmlTableList);
				}
			}
			#endregion
			
			if(webSession.ComparativeStudy){

				#region identifiant annonceurs référence et concurrents 
				AdvertiserAccessList = webSession.GetSelection(webSession.ReferenceUniversAdvertiser,CstRights.type.advertiserAccess);		
				if(webSession.CompetitorUniversAdvertiser[0]!=null){
					CompetitorAdvertiserAccessList = webSession.GetSelection((TreeNode)webSession.CompetitorUniversAdvertiser[0],CstRights.type.advertiserAccess);		
				}

				#region recuperation éléments références et concurrents
				if(TNS.AdExpress.Web.Functions.CheckedText.IsStringEmpty(AdvertiserAccessList)){				
					AdvertiserAccessListArr = new ArrayList(AdvertiserAccessList.Split(','));
				}
				if(TNS.AdExpress.Web.Functions.CheckedText.IsStringEmpty(CompetitorAdvertiserAccessList)){
					CompetitorAdvertiserAccessListArr = new ArrayList(CompetitorAdvertiserAccessList.Split(','));
				}			
				#endregion

				#endregion

				#region construction du tableau de nouveautés
				System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
				//Debut tableau
				t.Append("\n<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");

				//fin période étudiée
				//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
				//du dernier mois dispo en BDD
				//traitement de la notion de fréquence
                TNS.AdExpress.Domain.Layers.CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

                string absolutEndPeriod = dateDAL.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
                //string absolutEndPeriod = TNS.AdExpress.Web.Functions.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
				if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
					throw new NoDataException();
				DateTime PeriodEndDate = TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
				//Mois actif
				currentMonth = TNS.AdExpress.Web.Functions.Dates.CurrentActiveMonth(PeriodEndDate,webSession);
				//Date mois actif
				currentMonthDate = TNS.AdExpress.Web.Functions.Dates.GetDateFromAlias(currentMonth);

				#region ligne libellés
				classe2="p7";
				classe="p9";
				classe3="p9ind";
				//Ligne Libellés tableau				
				//Colonne libéllés produits ou annonceurs
				if(CstResult.Novelty.ElementType.advertiser==elementType)
					t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1222,webSession.SiteLanguage)+"</td>");
				else t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1223,webSession.SiteLanguage)+"</td>");
				//Période d'inactivité
				t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1220,webSession.SiteLanguage)+"</td>");			
				//Colonne separation nbre de mois d'inactivité/ graphique dernier mois actif sur N-1
                t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");

				//Cellules mois  de l'année N-1
				for (int j=1;j<=12;j++){
					if(j<10)pluszero="0";
					else pluszero="";
					t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+j+"-"+PeriodEndDate.AddYears(-1).Year+"</td>");
				}
				//Colonne separation année N/N-1
                t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");

				//cellules mois de la période N
				for(int m=1;m<currentMonthDate.Month;m++){
					if(m<10)pluszero="0";
					else pluszero="";
					t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+m+"-"+PeriodEndDate.Year+"</td>");
				}
				//Colonne separation miis actif année N / et mois inactif année N
                t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");
				//Colonne mois en cours (KE)
				if(currentMonthDate.Month<10)pluszero="0";
				else pluszero="";
                t.Append("<td  nowrap align=center class=\"p2\">" + (GestionWeb.GetWebWord(2786, webSession.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(webSession.SiteLanguage) + ")") + "<br>" + pluszero + currentMonthDate.Month + "-" + currentMonthDate.Year + "</td>");
				t.Append("\n</tr>");
			
				#endregion

				#region lignes produits ou annonceurs
				
				for(int i=0;i<tab.GetLength(0);i++){

					#region personnalisation des éléments de références et concurrents
					if(tab[i,CstResult.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX]!=null){
						IdElementToPersonnalize = tab[i,CstResult.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX].ToString();
						if(AdvertiserAccessListArr!=null && AdvertiserAccessListArr.Contains(IdElementToPersonnalize)){
							classe2="p15"; 
							classe="p151";
						}
						else if(CompetitorAdvertiserAccessListArr !=null && CompetitorAdvertiserAccessListArr.Contains(IdElementToPersonnalize)){							
							classe2="p14"; 
							classe="p141";	
						}
						else{						
							classe2="p7";
							classe="p9";
						}
					}
					#endregion

					t.Append("<tr>");
					//Colonne libéllé produit ou annonceur 
					if(tab[i,CstResult.Novelty.ELEMENT_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe2+">"+tab[i,CstResult.Novelty.ELEMENT_COLUMN_INDEX].ToString()+"</td>");
					else t.Append("<td nowrap  class="+classe2+">&nbsp;</td>");
					//Colonne Période d'inactivité (nbre de mois)
					if(tab[i,CstResult.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe+">"+tab[i,CstResult.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX].ToString()+"</td>");
					else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
					//Colonne separation nbre de mois d'inactivité/ graphique dernier mois actif sur N-1
                    t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");

					//Cellules mois  de l'année N-1
					for (int k=1;k<=12;k++){
						//Colonne libéllés dernier mois actifs année N-1 (kE)
						if(tab[i,CstResult.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]!=null && !tab[i,CstResult.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX].ToString().Equals("")){
							//PreviousYearActiveMonth = tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX].ToString();
							if(tab[i,CstResult.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX].ToString().Equals(k.ToString())){							
								//								t.Append("<td nowrap  class=pmcategorynb>"+double.Parse(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
                                t.Append("<td nowrap  class=pmcategorynb>" + TNS.AdExpress.Web.Functions.Units.ConvertUnitValueToString(tab[i, CstResult.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString(), webSession.Unit, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo) + "</td>");
								PreviousYearActiveMonth=true;
							}
							else if(PreviousYearActiveMonth) t.Append("<td nowrap class="+classe3+">&nbsp;</td>");
							else t.Append("<td nowrap  >&nbsp;</td>");
						}
						else t.Append("<td nowrap >&nbsp;</td>");
					}
					//Colonne separation année N/N-1
                    t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");
					//cellules mois de la période N
					for(int l=1;l<currentMonthDate.Month;l++){
						if(PreviousYearActiveMonth)t.Append("<td nowrap class="+classe3+">&nbsp;</td>");
						else t.Append("<td nowrap >&nbsp;</td>");
					}
					PreviousYearActiveMonth=false;
					//Colonne separation mois actif année N / et mois inactif année N
                    t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");

					//Colonne mois en cours (KE)
					if(tab[i,CstResult.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX]!=null && !tab[i,CstResult.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString().Equals("-"))
						//						t.Append("<td nowrap  class=\"pmcategorynb\">"+double.Parse(tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
						t.Append("<td nowrap  class=\"pmcategorynb\">"+TNS.AdExpress.Web.Functions.Units.ConvertUnitValueToString(tab[i,CstResult.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo) + "</td>");
					else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
					t.Append("\n</tr>");

					nbLines++;

					//Fin d'une ligne on crée une image toutes les 30 lignes

					if(nbLines%35==0){

						//Insertion de 30 lignes html dans l'image	
						#region footer
						t.Append("\n</TD></TR></TABLE>");
						t.Append("</form></body></html>");					
						#endregion

						htmlTableList.Add(t.ToString());

						t = new System.Text.StringBuilder(10000);

                        t.Append("<TR height=\"25\">");
                        t.Append("<TD></TD>");
                        t.Append("</TR>");
                        t.Append("<TR align=\"center\"><td>");
						t.Append("\n<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");

						#region ligne libellés
						classe2="p7";
						classe="p9";
						classe3="p9ind";
						//Ligne Libellés tableau				
						//Colonne libéllés produits ou annonceurs
						if(CstResult.Novelty.ElementType.advertiser==elementType)
							t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1222,webSession.SiteLanguage)+"</td>");
						else t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1223,webSession.SiteLanguage)+"</td>");
						//Période d'inactivité
						t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1220,webSession.SiteLanguage)+"</td>");			
						//Colonne separation nbre de mois d'inactivité/ graphique dernier mois actif sur N-1
                        t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");

						//Cellules mois  de l'année N-1
						for (int j=1;j<=12;j++){
							if(j<10)pluszero="0";
							else pluszero="";
							t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+j+"-"+PeriodEndDate.AddYears(-1).Year+"</td>");
						}
						//Colonne separation année N/N-1
                        t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");

						//cellules mois de la période N
						for(int m=1;m<currentMonthDate.Month;m++){
							if(m<10)pluszero="0";
							else pluszero="";
							t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+m+"-"+PeriodEndDate.Year+"</td>");
						}
						//Colonne separation miis actif année N / et mois inactif année N
                        t.Append("<td style=\"border-bottom: 0px; border-left: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; border-top: 0px; border-right: 0px; padding-top: 0px;height: 100%;\"><div class=\"gSepBackGround\"></div></td>");
						//Colonne mois en cours (KE)
						if(currentMonthDate.Month<10)pluszero="0";
						else pluszero="";
						t.Append("<td  nowrap align=center class=\"p2\">"+TNS.FrameWork.Convertion.ToHtmlString(GestionWeb.GetWebWord(1221,webSession.SiteLanguage))+"<br>"+pluszero+currentMonthDate.Month+"-"+currentMonthDate.Year+"</td>");
						t.Append("\n</tr>");
						#endregion
						
					}
				}
				#endregion

				//Fin tableau
				t.Append("\n</TD></TR></TABLE>");

				#region tableau légende
				t.Append("<table><tr><td nowrap class=pmcategorynb width=25>&nbsp;&nbsp;</td><td class=txtNoir11>"+GestionWeb.GetWebWord(1225,webSession.SiteLanguage)+"</td></tr>");
				t.Append("<tr><td nowrap class="+classe3+" width=25>&nbsp;&nbsp;</td><td class=txtNoir11>"+GestionWeb.GetWebWord(1226,webSession.SiteLanguage)+"</td></tr></table>");
				#endregion

				#endregion

				htmlTableList.Add(t.ToString());
				return(htmlTableList);
			}
			else{
				htmlTableList.Add("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(1224,webSession.SiteLanguage)				
					+"</div>");
				return(htmlTableList);

			}			
		}
		#endregion

		#region Ajout de l'image qui correspond au tableau de nouveautés 
        /// <summary>
        /// addNoveltyPicture
        /// </summary>
        /// <param name="result"></param>
        /// <param name="i"></param>
        /// <param name="withTitle"></param>
		protected virtual void addNoveltyPicture(IList result,int i,bool withTitle){
		
			StringBuilder htmlTMP=null;
			string filePath="";
		
			htmlTMP=new StringBuilder();

            htmlTMP.Append("<TR height=\"25\">");
            htmlTMP.Append("<TD></TD>");
            htmlTMP.Append("</TR>");
            htmlTMP.Append("<TR align=\"center\"><td>");
			htmlTMP.Append(result[i]);
			htmlTMP.Append("</td></tr>");

			this.NewPage();

			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

			if(withTitle){

				#region Title
                Style.GetTag("NoveltyPictureTitleFontPage").SetStylePdf(this, GetTxFontCharset());
				this.PDFPAGE_UnicodeTextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1197 ,_webSession.SiteLanguage));
				#endregion

			}

            byte[] data = this.ConvertHtmlToSnapJpgByte(htmlTMP.ToString(),
                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                _config.WebServer);
	
			filePath=Path.GetTempFileName();
			FileStream  fs = File.OpenWrite(filePath); 	
			BinaryWriter br = new BinaryWriter(fs); 

			br.Write(data);
			br.Close();
			fs.Close();

			Image imgG ;
			int imgI =0;
			double X1=2;

			imgG = Image.FromFile(filePath);
			imgI = this.AddImageFromFilename(filePath,TxImageCompressionType.itcFlate);

			double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
			double coef = Math.Min((double)1.0,w);
			w = ((double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height);
			coef = Math.Min((double)coef, w);

	
			this.PDFPAGE_ShowImage(imgI,
				X1, this.PDFPAGE_Height/2 - (coef * imgG.Height / 2),
				coef * imgG.Width,
				coef * imgG.Height,
				0);

			imgG.Dispose();
			imgG = null;

			#region Clean File
			File.Delete(filePath);
			#endregion
		}
		#endregion
		
		#endregion

        #region Evenement Envoi mail client
        /// <summary>
        /// Rise exception when the customer mail has not been sent
        /// </summary>
        /// <param name="source">Error source></param>
        /// <param name="message">Error message</param>
        private void mail_mailKoHandler(object source, string message) {
            throw new Exceptions.HotepPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
        }
        #endregion

        #region GetTxFontCharset
        /// <summary>
        /// Get Text Font Charset
        /// </summary>
        /// <returns></returns>
        protected override TxFontCharset GetTxFontCharset()
        {
            return _config.PdfCreatorPilotCharsets[_webSession.SiteLanguage.ToString()];

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
        protected virtual void CreatesSeries(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, Dictionary<string, DataTable> listTableRefCompetitor, Dictionary<string, Series> listSeriesMedia)
        {
            // Create series (one per media)
            for (int i = 1; i < tab.GetLongLength(0); i++)
            {

                //	Dictionary with advertiser label as key and total as value
                if (tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null)
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
        protected virtual void ComputeTotals(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, ref double totalUniversValue, ref double totalSectorValue, ref double totalMarketValue, int MEDIA_LEVEL_NUMBER)
        {
            #region Once Media
            if (MEDIA_LEVEL_NUMBER == 2 || MEDIA_LEVEL_NUMBER == 3)
            {
                for (int i = 1; i < tab.GetLongLength(0); i++)
                {
                    for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                    {
                        switch (j)
                        {

                            #region support
                            // Univers Total
                            case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            #endregion

                            #region Advertisers
                            case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && MEDIA_LEVEL_NUMBER == 3)
                                {
                                    listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }
                                else if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] == null
                                    && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            #endregion

                            #region Category
                            // Univers Total	
                            case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Sector Total
                            case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            // Market Total
                            case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:

                                if (tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]);
                                }

                                break;
                            #endregion

                            default:
                                break;
                        }
                    }
                }
            }
            #endregion

            #region PluriMedia
            if (MEDIA_LEVEL_NUMBER == 1)
            {
                for (int i = 0; i < tab.GetLongLength(0); i++)
                {
                    for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                    {
                        switch (j)
                        {
                            case EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX] != null)
                                {
                                    totalUniversValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX] != null)
                                {
                                    totalSectorValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX] != null)
                                {
                                    totalMarketValue += Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]);
                                }
                                break;
                            case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                                if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                    && tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX] != null)
                                {
                                    listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] += Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]);
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            #endregion
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
        protected virtual void FillTable(object[,] tab, Dictionary<string, double> listSeriesMediaRefCompetitor, Dictionary<string, DataTable> listTableRefCompetitor, DataTable tableUnivers, DataTable tableSectorMarket, ref double totalUniversValue, ref double totalSectorValue, ref double totalMarketValue, int MEDIA_LEVEL_NUMBER, bool withPluriByCategory)
        {
            #region Table
            double elementValue;          
            // Define columns
            tableUnivers.Columns.Add("Name");
            tableUnivers.Columns.Add("Position", typeof(double));
            tableSectorMarket.Columns.Add("Name");
            tableSectorMarket.Columns.Add("Position", typeof(double));

            for (int i = 1; i < tab.GetLongLength(0); i++)
            {
                for (int j = 0; j < EngineMediaStrategy.NB_MAX_COLUMNS; j++)
                {
                    switch (j)
                    {

                        #region Media
                        case EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                                j = j + 6;
                            }

                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 5;
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 3)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                                j = j + 4;
                            }
                            break;
                        #endregion

                        #region Advertisers
                        case EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                && tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] != null
                                && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                && MEDIA_LEVEL_NUMBER == 3)
                            {

                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }

                                j = j + 12;
                            }
                            else if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                                && tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX] != null
                                && tab[i, EngineMediaStrategy.LABEL_MEDIA_COLUMN_INDEX] == null
                                && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                                && MEDIA_LEVEL_NUMBER == 2)
                            {

                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            else if (tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX] != null
                        && tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null
                        && tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX] != null
                             && MEDIA_LEVEL_NUMBER == 1)
                            {
                                if (listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]) / listSeriesMediaRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()] * 100;
                                    DataRow row1 = listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    listTableRefCompetitor[tab[i, EngineMediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString()].Rows.Add(row1);
                                }
                                j = j + 12;
                            }
                            break;
                        #endregion

                        #region Categorie
                        case EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX] != null && MEDIA_LEVEL_NUMBER == 2)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_CATEGORY_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        #endregion

                        #region PluriMedia
                        case EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1 && !withPluriByCategory)
                            {
                                if (totalUniversValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]) / totalUniversValue * 100;
                                    DataRow row = tableUnivers.NewRow();
                                    row["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableUnivers.Rows.Add(row);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1 && !withPluriByCategory)
                            {
                                if (totalSectorValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]) / totalSectorValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
                                    tableSectorMarket.Rows.Add(row1);
                                }
                            }
                            break;
                        case EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX:
                            if (tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX] != null && i > 1 && !withPluriByCategory)
                            {
                                if (totalMarketValue != 0)
                                {
                                    elementValue = Convert.ToDouble(tab[i, EngineMediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]) / totalMarketValue * 100;
                                    DataRow row1 = tableSectorMarket.NewRow();
                                    row1["Name"] = tab[i, EngineMediaStrategy.LABEL_VEHICLE_COLUMN_INDEX];
                                    row1["Position"] = Convert.ToDouble(elementValue.ToString("0.00"));
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
        protected virtual void InitSeries(DataTable tableUnivers, DataTable tableSectorMarket, double[] yValues, string[] xValues, double[] yValuesSectorMarket, string[] xValuesSectorMarket, Dictionary<string, Series> listSeriesMedia, Dictionary<string, DataTable> listTableRefCompetitor, Dictionary<int, string> listSeriesName, int MEDIA_LEVEL_NUMBER)
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

            if (MEDIA_LEVEL_NUMBER != 1)
            {
                for (int i = 0; i < 5 && i < foundRows.Length; i++)
                {
                    xValues[i] = foundRows[i]["Name"].ToString();
                    yValues[i] = Convert.ToDouble(foundRows[i]["Position"]);
                    otherUniversValue += Convert.ToDouble(foundRows[i]["Position"]);
                    index = i + 1;
                }
                if (foundRows.Length > NBRE_MEDIA)
                {
                    xValues[index] = GestionWeb.GetWebWord(647, _webSession.SiteLanguage);
                    yValues[index] = 100 - otherUniversValue;
                }

                for (int i = 0; i < 5 && i < foundRowsSectorMarket.Length; i++)
                {
                    xValuesSectorMarket[i] = foundRowsSectorMarket[i]["Name"].ToString();
                    yValuesSectorMarket[i] = Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    otherSectorMarketValue += Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    index = i + 1;
                }
                if (foundRowsSectorMarket.Length > NBRE_MEDIA)
                {
                    xValuesSectorMarket[index] = GestionWeb.GetWebWord(647, _webSession.SiteLanguage);
                    yValuesSectorMarket[index] = 100 - otherSectorMarketValue;
                }
            }
            // Cas PluriMedia
            else
            {
                for (int i = 0; i < foundRows.Length; i++)
                {
                    xValues[i] = foundRows[i]["Name"].ToString();
                    yValues[i] = Convert.ToDouble(foundRows[i]["Position"]);
                    otherUniversValue += Convert.ToDouble(foundRows[i]["Position"]);
                }

                for (int i = 0; i < foundRowsSectorMarket.Length; i++)
                {
                    xValuesSectorMarket[i] = foundRowsSectorMarket[i]["Name"].ToString();
                    yValuesSectorMarket[i] = Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                    otherSectorMarketValue += Convert.ToDouble(foundRowsSectorMarket[i]["Position"]);
                }
            }

            double[] yVal = new double[foundRows.Length];
            string[] xVal = new string[foundRows.Length];
            double otherCompetitorRefValue = 0;
            int k = 2;

            foreach (string name in listSeriesMedia.Keys)
            {

                if (name == GestionWeb.GetWebWord(1780, _webSession.SiteLanguage))
                {
                    if (xValues != null && xValues.Length > 0 && xValues[0] != null)
                        listSeriesMedia[GestionWeb.GetWebWord(1780, _webSession.SiteLanguage)].Points.DataBindXY(xValues, yValues);
                }
                else if (_webSession.ComparaisonCriterion == CstWeb.CustomerSessions.ComparisonCriterion.sectorTotal && name == GestionWeb.GetWebWord(1189, _webSession.SiteLanguage))
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
                    otherCompetitorRefValue = 0;

                    yVal = new double[foundRowsCompetitorRef.Length];
                    xVal = new string[foundRowsCompetitorRef.Length];
                    if (MEDIA_LEVEL_NUMBER != 1)
                    {
                        for (int i = 0; i < foundRowsCompetitorRef.Length && i < NBRE_MEDIA; i++)
                        {


                            xVal[i] = foundRowsCompetitorRef[i]["Name"].ToString();
                            yVal[i] = Convert.ToDouble(foundRowsCompetitorRef[i]["Position"]);

                            otherCompetitorRefValue += Convert.ToDouble(foundRowsCompetitorRef[i]["Position"]);
                            index = i + 1;
                        }
                        if (foundRowsCompetitorRef.Length > NBRE_MEDIA)
                        {
                            xVal[index] = "Autres";
                            yVal[index] = 100 - otherCompetitorRefValue;
                        }
                    }
                    // PluriMedia
                    else
                    {
                        for (int i = 0; i < foundRowsCompetitorRef.Length; i++)
                        {
                            xVal[i] = foundRowsCompetitorRef[i]["Name"].ToString();
                            yVal[i] = Convert.ToDouble(foundRowsCompetitorRef[i]["Position"]);

                        }
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
    }
}
