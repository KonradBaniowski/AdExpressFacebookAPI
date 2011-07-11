#region Information
//Author : Y. Rkaina
//Creation : 10/08/2006
#endregion

#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Reflection;

using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Anubis.Miysis.Exceptions;

using TNS.AdExpress.Web.UI.Results;

using TNS.AdExpress.Constantes.Customer;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.BusinessFacade.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Rules.Results;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;

using PDFCreatorPilotLib;
using TNS.FrameWork.DB.Common;

using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using System.Globalization;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;
using DomainLevel = TNS.AdExpress.Domain.Level;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpressI.Insertions;
using TNS.AdExpressI.Insertions.Cells;
using TNS.Ares;
using TNS.Ares.Pdf;
using TNS.FrameWork.WebTheme;
#endregion

namespace TNS.AdExpress.Anubis.Miysis.BusinessFacade{
	/// <summary>
	/// Generate the PDF document for MiysisPdfSystem module.
	/// </summary>
	public class MiysisPdfSystem : Pdf{

		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Appm Configuration (usefull for PDF layout)
		/// </summary>
		private MiysisConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Position des visuels dans le pdf dans le cas de 4 visuels par page
		/// </summary>
		private enum imgPositionItems {
			/// <summary>
			/// Position : Haut, gauche
			/// </summary>
			TOP_LEFT=0,
			/// <summary>
			/// Position : Haut, droite
			/// </summary>
			TOP_RIGHT=1,
			/// <summary>
			/// Position : Bas, gauche
			/// </summary>
			BOTTOM_LEFT=2,
			/// <summary>
			/// Position : Bas, droite
			/// </summary>
			BOTTOM_RIGHT=3
		};
		/// <summary>
		/// Position des visuels dans le pdf
		/// </summary>
		private enum imgPosition {
			/// <summary>
			/// Position : Haut
			/// </summary>
			TOP=0,
			/// <summary>
			/// Position : Bas
			/// </summary>
			BOTTOM=1,
			/// <summary>
			/// Dépend du traitement
			/// </summary>
			DYNAMIC=-1
		};
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public MiysisPdfSystem(IDataSource dataSource, MiysisConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
            :
        base(theme.GetStyle("Miysis")) {
            try {
                this._dataSource = dataSource;
                this._config = config;
                this._rqDetails = rqDetails;
                this._webSession = webSession;
            }
            catch (Exception e) {
                throw new MiysisPdfException("Error in Constructor MiysisPdfSystem", e);
            }

        }
	    #endregion

		#region Init
		internal string Init(){
			try{
				string shortFName = "";
				string fName =  GetFileName(_rqDetails, ref shortFName);
				bool display = false;
#if(DEBUG)
				display = true;
#endif
				base.Init(true,fName,_config.PdfCreatorPilotLogin,_config.PdfCreatorPilotPass);
				this.DocumentInfo_Creator = this.DocumentInfo_Author = _config.PdfAuthor;
				this.DocumentInfo_Subject = _config.PdfSubject;
				this.DocumentInfo_Title = GestionWeb.GetWebWord(751, _webSession.SiteLanguage);
				this.DocumentInfo_Producer = _config.PdfProducer;
				this.DocumentInfo_Keywords = _config.PdfKeyWords;
				return shortFName;
			}
			catch(System.Exception e){
                throw new MiysisPdfException("Error to initialize MiysisPdfSystem in Init()", e);
			}
		}
		#endregion

		#region Fill
		internal void Fill(){

			try{

				#region MainPage
				MainPageDesign();
				#endregion

				#region SessionParameter
				SessionParameter();
				#endregion
				
				#region Impression
				MediaPlanImpression();
				#endregion				

				#region Header and Footer
				string dateString = Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

				this.AddHeadersAndFooters(
				_webSession,
				imagePosition.leftImage,
				GestionWeb.GetWebWord(751, _webSession.SiteLanguage) + " - " + dateString,
				0, -1, true);
				#endregion
				
			}
			catch(System.Exception e){
                throw new MiysisPdfException("Error to Fill Pdf in Fill()"+ e.StackTrace + e.Source, e);
			}
		}
		#endregion

		#region Send
		internal void Send(string fileName){
            //try{    

            //    MailMessage mail = new MailMessage();
            //    mail.From = new MailAddress(_config.CustomerMailFrom);
            //    foreach (string s in _webSession.EmailRecipient) {
            //        mail.To.Add(s);
            //    }
            //    mail.Subject =  GestionWeb.GetWebWord(2006, _webSession.SiteLanguage);
            //    mail.SubjectEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
            //    mail.Body = GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + "\"" + _webSession.ExportedPDFFileName + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer) + "<br><br>" + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage);
            //    mail.BodyEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
            //    mail.IsBodyHtml = true;
            //    SmtpClient emailClient = new SmtpClient(_config.CustomerMailServer, _config.CustomerMailPort);
            //    emailClient.Send(mail);
            //}
            //catch (System.Exception e) {
            //    throw new MiysisPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession, e);
            //}

            try {
                ArrayList to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient) {
                    to.Add(s);
                }
                SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    GestionWeb.GetWebWord(2006, _webSession.SiteLanguage),
                    GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + "\"" + _webSession.ExportedPDFFileName
                    + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer)
                    //				+ "<br><br>" + "<a href=\"http://www.tnsadexpress.com/AdExCustomerFiles/" + _webSession.CustomerLogin.IdLogin + "/" + fileName + ".pdf\">" + _webSession.ExportedPDFFileName +"</a>"
                    + "<br><br>"
                    + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);
                mail.SubjectEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                mail.BodyEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
                mail.SendWithoutThread(false);
            }
            catch (System.Exception e) {
                throw new MiysisPdfException("Error to Send mail to client in Send(string fileName)", e);
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
				throw(new MiysisPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
			}
		}
		#endregion

		#endregion

		#region MainPage
		/// <summary>
		/// Design Main Page
		/// </summary>
		/// <returns></returns>
		private void MainPageDesign(){
			
			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;
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
			this.PDFPAGE_ShowImage(imgI,
				(double)(this.PDFPAGE_Width/2 - coef*imgG.Width/2), (double)(this.WorkZoneBottom - coef*imgG.Height - 100),
				(double)(coef*imgG.Width),(double)(coef*imgG.Height),0);

            Style.GetTag("bigTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);			
			string str = "";
			
			this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(GestionWeb.GetWebWord(751, _webSession.SiteLanguage))) / 2,
					(this.PDFPAGE_Height) / 4, 0, GestionWeb.GetWebWord(751, _webSession.SiteLanguage));			
			
            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

            Style.GetTag("createdTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
			this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str))/2, 
				1*this.PDFPAGE_Height/3,0,str);

		}
		#endregion

		#region SessionParameter
		private void SessionParameter(){
		
		//Formatting date to be used in the query
		string dateBegin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd");
		string dateEnd = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd");

        StringBuilder html = new StringBuilder();
        
        html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");

		#region Title
		html.Append("<TR height=\"25\">");
		html.Append("<TD></TD>");
		html.Append("</TR>");
		html.Append("<TR height=\"14\">");
		//html.Append("<TD style=\"font-family: Arial, Helvetica, sans-serif; font-size: 20px; color: #644883; font-weight: bold;\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752,_webSession.SiteLanguage)) + "</TD>");
		html.Append("<TD class=\"mi\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752, _webSession.SiteLanguage)) + "</TD>");
		html.Append("</TR>");
		#endregion
		
		#region Period
		html.Append("<TR height=\"7\">");
		html.Append("<TD></TD>");
		html.Append("</TR>");
		html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
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
        // Période comparative
        if (_webSession.ComparativeStudy && TNS.AdExpress.Domain.Web.WebApplicationParameters.UseComparativeMediaSchedule) {
            html.Append(GetComparativePeriodDetail(_webSession, TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA, false));
        }
        // Type Sélection comparative
        if (_webSession.ComparativeStudy && TNS.AdExpress.Domain.Web.WebApplicationParameters.UseComparativeMediaSchedule) {
            html.Append(GetComparativePeriodTypeDetail(_webSession, TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA));
        }
		#endregion

		#region Média
		//TODO Média
		html.Append("<TR height=\"7\">");
		html.Append("<TD></TD>");
		html.Append("</TR>");
		html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
		html.Append("<TD></TD>");
		html.Append("</TR>");
		html.Append("<TR>");
		html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1292, _webSession.SiteLanguage)) + " :</TD>");
		html.Append("</TR>");
		html.Append("<TR>");
		html.Append("<TD align=\"left\">");
		html.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.SelectionUniversMedia,false,false,false,600,false,false,_webSession.SiteLanguage,2,1,true));
		html.Append("</TD>");
		html.Append("</TR>");
		#endregion

		#region Unité
		//TODO Unité
		html.Append("<TR height=\"7\">");
		html.Append("<TD></TD>");
		html.Append("</TR>");
		html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
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
		#endregion

        #region produit
        bool first = true;
		const int nbLineByPage = 32;
		int currentLine = 10;
		if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0) {

			html.Append("<TR height=\"7\">");
			html.Append("<TD></TD>");
			html.Append("</TR>");
			html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
			html.Append("<TD></TD>");
			html.Append("</TR>");
			html.Append("<TR>");
			html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage)) + " :</TD>");
			html.Append("</TR>");

			html.Append("<TR align=\"center\">");
			html.Append("<TD>");
            html.Append(Convertion.ToHtmlString(TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.Source, 600, true, nbLineByPage, ref currentLine)));
			html.Append("</TD>");
			html.Append("</TR>");

		}
        if ((_webSession.ProductDetailLevel != null && _webSession.ProductDetailLevel.ListElement != null) || _webSession.SloganIdZoom > -1) {
            html.Append("<TR height=\"7\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
			html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(518, _webSession.SiteLanguage)) + " :</TD>");
            html.Append("</TR>");
            if (_webSession.ProductDetailLevel != null && _webSession.ProductDetailLevel.ListElement != null)
            {
                html.Append("<TR>");
                html.Append("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;");
                #region level
                int code = -1;
                switch (((LevelInformation)_webSession.ProductDetailLevel.ListElement.Tag).Type)
                {
                    case CstRights.type.sectorAccess:
                        code = 175;
                        break;
                    case CstRights.type.subSectorAccess:
                        code = 1931;
                        break;
                    case CstRights.type.groupAccess:
                        code = 859;
                        break;
                    case CstRights.type.advertiserAccess:
                        code = 857;
                        break;
                    case CstRights.type.productAccess:
                        code = 858;
                        break;
                    case CstRights.type.brandAccess:
                        code = 1889;
                        break;
                    case CstRights.type.holdingCompanyAccess:
                        code = 1589;
                        break;
                    case CstRights.type.segmentAccess:
                        code = 592;
                        break;

                }
                #endregion
                html.Append(Convertion.ToHtmlString(string.Format("{0} \"{1}\"",
                    GestionWeb.GetWebWord(code, _webSession.SiteLanguage),
                    ((LevelInformation)_webSession.ProductDetailLevel.ListElement.Tag).Text
                    )
                ));
                html.Append("</TD>");
                html.Append("</TR>");
            }
            if (_webSession.SloganIdZoom > -1)
            {
                html.Append("<TR>");
                html.Append("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;");
                html.Append(Convertion.ToHtmlString(string.Format("{0} \"{1}\"",
                    GestionWeb.GetWebWord(1987, _webSession.SiteLanguage),
                    _webSession.SloganIdZoom
                    )
                ));
                html.Append("</TD>");
                html.Append("</TR>");
            }
        }
        #endregion

        html.Append("</TABLE>");

        this.ConvertHtmlToPDF(html.ToString(),
            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
            _config.WebServer,
            _config.Html2PdfLogin,
            _config.Html2PdfPass);

		}
		#endregion

		#region MediaPlanIpression
		private void MediaPlanImpression(){
	
			#region GETHTML
			StringBuilder html=new StringBuilder(10000);
			ArrayList versionsUIs = new ArrayList();
			int startIndexVisual=0;
			Int64 nbToSplit=0;
			int nbLines=0;
            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            MediaScheduleData result = null;
            MediaSchedulePeriod period = null;
            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
            DateTime begin;
            DateTime end;
            object[] param = null;

            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CstRights.type.vehicleAccess).Split(',');
			try{

                #region result
                Int64 idVehicle = Int64.Parse(vehicles[0]);

                begin = Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                end = Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);

                if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, _webSession.DetailPeriod, _webSession.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, _webSession.DetailPeriod);
                
                if (vehicles.Length == 1) {
                    param = new object[3];
                    param[2] = idVehicle;
                }
                else
                    param = new object[2];

                param[0] = _webSession;
                param[1] = period;

                IMediaScheduleResults mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                mediaScheduleResult.Module = module;
                result = mediaScheduleResult.GetPDFHtml();
                SortedDictionary<Int64, List<CellCreativesInformation>> creativeCells = new SortedDictionary<Int64, List<CellCreativesInformation>>();

                if (vehicles.Length == 1 && _webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.dayly && result.VersionsDetail.Count > 0) {

                    VersionsPluriMediaUI versionsUI = new VersionsPluriMediaUI(_webSession, period, "");
                    //html.Append("\r\n\t<tr class=\"whiteBackGround\">\r\n\t\t<td>");
                    startIndexVisual = decoupageVersionHTML(html, versionsUI.GetExportMSCreativesHtml(ref creativeCells, base.Style), true, int.Parse(vehicles[0]));
                    VehicleInformation currentVehicle = VehiclesInformation.Get(idVehicle);
                    if (currentVehicle.Id != DBCst.Vehicles.names.tv && currentVehicle.Id != DBCst.Vehicles.names.radio)
                        BuildVersionPDF(creativeCells, startIndexVisual);

                }
                #endregion

				#region Construction du tableaux global		
				Double w,width;
                TimeSpan timeSpan = period.End.Subtract(period.Begin);
                nbToSplit = (Int64)Math.Round((double)timeSpan.Days / 7);

                if (_webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.weekly)
                    nbToSplit = (Int64)Math.Round((double)nbToSplit / 7);
                else if (_webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.monthly)
                    nbToSplit = (Int64)Math.Round((double)nbToSplit / 30);

                if (nbToSplit < 5) {
                        width = 863;
                        w = 1;
                        nbLines = (int)Math.Round(w * (width / 20)) - 4;
                    }
                    else {
                        w = 0.652;
                        width = 996 + (133 * (nbToSplit - 5));
                        nbLines = (int)Math.Round(w * (width / 20)) - 4;
                    }

				decoupageHTML(html,Convertion.ToHtmlString(result.HTMLCode),nbLines,false);
				html.Append("\r\n\t\t</td>\r\n\t</tr>");
				html.Append("</table>");
                #endregion
                
                html.Append("</BODY>");
				html.Append("</HTML>");
            
            }
			catch(System.Exception err){
				throw(new MiysisPdfException("Unable to process Media Schedule Alert export result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",err)); 
			}
			finally{
				_webSession.CurrentModule = module.Id;
			}
			#endregion

		}
		#endregion

		#region Découpage du code HTML

        #region decoupageHTML
        /// <summary>
		/// Découpage du code HTML pour l'export PDF du plan média (Tableau)
		/// </summary>
		/// <param name="html">Le code HTML à générer</param>
		/// <param name="strHtml">Le code HTML à découper</param>
		/// <param name="nbLines">Nombres de lignes pour le découpage</param>
		/// <param name="htmlHeader">L'entête HTML</param>
		private void decoupageHTML(StringBuilder html,string strHtml, int nbLines,bool version){

			int startIndex=0,oldStartIndex=0,tmp;
			ArrayList partieHTML = new ArrayList();
            string resultTableHeader = string.Empty;
            if (strHtml.Length > 0) {
                for (int i = 0; i < 3; i++)
                    startIndex = strHtml.IndexOf("</tr>", startIndex + 1);
                resultTableHeader = strHtml.Substring(0, startIndex);
            }
			double coef=0;
			int start=0;
            startIndex = 0;

			 while((startIndex < strHtml.Length)&&(startIndex!=-1)){
				tmp=0;
				
				if(start==0){
					while((tmp<nbLines+4)&&(startIndex<strHtml.Length)&&(startIndex!=-1)){
						startIndex=strHtml.IndexOf("<tr>",startIndex+1);
						tmp++;
					}
				start=1;
				}
				else{
					while((tmp<nbLines)&&(startIndex<strHtml.Length)&&(startIndex!=-1)){
						startIndex=strHtml.IndexOf("<tr>",startIndex+1);
						tmp++;
					}
				}
				if(startIndex==-1)
					partieHTML.Add(strHtml.Substring(oldStartIndex,strHtml.Length-oldStartIndex));
				else
					partieHTML.Add(strHtml.Substring(oldStartIndex,startIndex-oldStartIndex));
				oldStartIndex=startIndex;
			}
			
			for(int i=0; i<partieHTML.Count; i++){
                if (i > 0) html.Append(resultTableHeader);
				html.Append(partieHTML[i].ToString());
                if (i < partieHTML.Count - 1) html.Append("</table>");
				if(version)
                    SnapShots(html.ToString(), i, partieHTML.Count, true, ref coef);
				else
                    SnapShots(html.ToString(), i, partieHTML.Count, false, ref coef);
                html = new StringBuilder();
			}
        }
        #endregion

        #region decoupageVersionHTML
        /// <summary>
        /// Découpage du code HTML pour l'export PDF du plan média (Visuels)
        /// </summary>
        /// <param name="html">Le code HTML à générer</param>
        /// <param name="strHtml">Le code HTML à découper</param>
        private int decoupageVersionHTML(StringBuilder html, string strHtml, bool version, int vehicle) {

            int startIndex = 0, oldStartIndex = 0, tmp, startIndexVisual = 0;
            ArrayList partieHTML = new ArrayList();
            StringBuilder htmltmp = new StringBuilder(1000);
            htmltmp.Append(html.ToString());

            while ((startIndex < strHtml.Length) && (startIndex != -1)) {
                tmp = 0;

                while ((tmp < 1) && (startIndex < strHtml.Length) && (startIndex != -1)) {
                    startIndex = strHtml.IndexOf("<br>", startIndex + 1);
                    tmp++;
                }

                if (startIndex == -1)
                    partieHTML.Add(strHtml.Substring(oldStartIndex, strHtml.Length - oldStartIndex));
                else
                    partieHTML.Add(strHtml.Substring(oldStartIndex, startIndex - oldStartIndex));
                oldStartIndex = startIndex;
            }

            for (int i = 0; i < partieHTML.Count - 1; i++) {
                if ((((DBCst.Vehicles.names)vehicle == DBCst.Vehicles.names.radio) || ((DBCst.Vehicles.names)vehicle == DBCst.Vehicles.names.tv)) && (i > 0))
                {
                    htmltmp.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
                    htmltmp.Append("<tr><td bgcolor=\"#ffffff\" style=\"HEIGHT: 50px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");
                }
                htmltmp.Append(partieHTML[i].ToString());
                if (version) {
                    if (i == 0) {
                        startIndexVisual = this.ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                    }
                    else {
                        this.ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                    }
                }
                else {
                    this.ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                }
                htmltmp = new StringBuilder();
            }
            return startIndexVisual;
        }
        #endregion

		#endregion

		#region Création et Insertion d'une image dans une nouvelle page du PDF
		/// <summary>
		/// Création et Insertion d'une image dans une nouvelle page du PDF
		/// </summary>
		/// <param name="html">Le code HTML</param>
		/// <param name="i">Index d'une partie de code</param>
		/// <param name="partieHTML">Une partie du code HTML</param>
		private void SnapShots(string html, int currentIndexPart, int nbpart, bool version, ref double coef){
		
			string filePath="";

			#region Création et Insertion d'une image dans une nouvelle page du PDF

			this.NewPage();
			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

            byte[] data = this.ConvertHtmlToSnapJpgByte(html,
            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
            _config.WebServer);

            filePath = Path.GetTempFileName();
			FileStream  fs = File.OpenWrite(filePath); 	
			BinaryWriter br = new BinaryWriter(fs); 
            if(data!=null)
			    br.Write(data);
			br.Close();
			fs.Close();

			Image imgG ;
			int imgI =0;
			double X1=10;

			imgG = Image.FromFile(filePath);
			imgI = this.AddImageFromFilename(filePath,TxImageCompressionType.itcFlate);

			double w = 0;
            if ((currentIndexPart < (nbpart - 1)) || (nbpart == 1)) {
				w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
				coef = Math.Min((double)1.0,w);
				w = ((double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height);
				coef = Math.Min((double)coef, w);
			}

			if(version) {
                if (currentIndexPart == (nbpart - 1)) {
					this.PDFPAGE_ShowImage(imgI,
						X1, 64,
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
				else {
					this.PDFPAGE_ShowImage(imgI,
						X1, this.PDFPAGE_Height/2 - (coef * imgG.Height / 2),
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
			}
			else{
                if (currentIndexPart == (nbpart - 1)) {
					this.PDFPAGE_ShowImage(imgI,
						X1, 40,
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
				else {
					this.PDFPAGE_ShowImage(imgI,
						X1, this.PDFPAGE_Height/2 - (coef * imgG.Height / 2),
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
			}

			imgG.Dispose();
			imgG = null;

			#region Clean File
			File.Delete(filePath);
			#endregion

			#endregion
		}

		#endregion

		#region BuildVersionPDF(String title) 
		/// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
        protected void BuildVersionPDF(SortedDictionary<Int64, List<CellCreativesInformation>> creativeCells, int startIndex) {

			ArrayList partitHTMLVersion = new ArrayList();
			StringBuilder htmltmp=new StringBuilder(1000);
			int end=0;
			double indexPage=0;
			int indexFirstCase=0, indexSecondCase=0, indexThirdCase=0, X1=0, Y1=0, nbLines=0;
			bool first=true, second=true;
            long nbVisuel = 0;

            if (creativeCells != null) {

                foreach (List<CellCreativesInformation> currentList in creativeCells.Values) {
                    foreach (CellCreativesInformation item in currentList) {

                        nbVisuel = item.Visuals.Count;

                        if (nbVisuel == 1) {

                            if (indexPage == 0) {
                                switch ((imgPositionItems)indexFirstCase) {
                                    case imgPositionItems.TOP_LEFT: X1 = 29; Y1 = 77; break;
                                    case imgPositionItems.TOP_RIGHT: X1 = 557; Y1 = 77; break;
                                    case imgPositionItems.BOTTOM_LEFT: X1 = 29; Y1 = 404; break;
                                    case imgPositionItems.BOTTOM_RIGHT: X1 = 557; Y1 = 404; break;
                                }
                            }
                            else {
                                switch ((imgPositionItems)indexFirstCase) {
                                    case imgPositionItems.TOP_LEFT: X1 = 27; Y1 = 75; break;
                                    case imgPositionItems.TOP_RIGHT: X1 = 554; Y1 = 75; break;
                                    case imgPositionItems.BOTTOM_LEFT: X1 = 27; Y1 = 401; break;
                                    case imgPositionItems.BOTTOM_RIGHT: X1 = 554; Y1 = 401; break;
                                }
                            }
                            indexFirstCase++;
                            InsertImageInPDF(0, item, indexPage, startIndex, X1, Y1);
                            if ((indexFirstCase == 1) || (indexFirstCase == 3)) {
                                nbLines++;
                            }
                            if (indexFirstCase == 4) {
                                indexPage++;
                                indexFirstCase = 0;
                            }
                        }
                        else if (nbVisuel < 5) {

                            if (first) {
                                if ((nbLines % 2) == 0) {
                                    indexPage = (nbLines / 2);
                                }
                                else {
                                    indexPage = Math.Ceiling((double)(nbLines / 2));
                                    indexSecondCase--;
                                }
                                first = false;
                            }

                            if (indexPage == 0) {
                                switch ((imgPosition)indexSecondCase) {
                                    case imgPosition.TOP: X1 = 25; Y1 = 70; break;
                                    case imgPosition.DYNAMIC:
                                    case imgPosition.BOTTOM: X1 = 25; Y1 = 393; break;
                                }
                            }
                            else {
                                switch ((imgPosition)indexSecondCase) {
                                    case imgPosition.TOP: X1 = 25; Y1 = 70; break;
                                    case imgPosition.DYNAMIC:
                                    case imgPosition.BOTTOM: X1 = 25; Y1 = 393; break;
                                }
                            }

                            InsertImageInPDF(0, item, indexPage, startIndex, X1, Y1);
                            if (indexSecondCase == -1)
                                indexPage++;
                            indexSecondCase++;
                            nbLines++;
                            if (indexSecondCase == 2) {
                                indexPage++;
                                indexSecondCase = 0;
                            }
                        }
                        else if (nbVisuel >= 5) {

                            end = (int)Math.Ceiling((double)nbVisuel / 4);

                            for (int i = 0; i < end; i++) {

                                if ((first) || (second)) {
                                    if ((nbLines % 2) == 0) {
                                        indexPage = (nbLines / 2);
                                    }
                                    else {
                                        indexPage = Math.Ceiling((double)(nbLines / 2));
                                        indexThirdCase--;
                                    }
                                    first = false;
                                    second = false;
                                }

                                if (indexPage == 0) {
                                    switch ((imgPosition)indexThirdCase) {
                                        case imgPosition.TOP: X1 = 25; Y1 = 70; break;
                                        case imgPosition.DYNAMIC:
                                        case imgPosition.BOTTOM: X1 = 25; Y1 = 397; break;
                                    }
                                }
                                else {
                                    switch ((imgPosition)indexThirdCase) {
                                        case imgPosition.TOP: X1 = 25; Y1 = 70; break;
                                        case imgPosition.DYNAMIC:
                                        case imgPosition.BOTTOM: X1 = 25; Y1 = 394; break;
                                    }
                                }

                                if (i == 0) {
                                    InsertImageInPDF(0, item, indexPage, startIndex, X1, Y1);
                                }
                                else {
                                    if (i == end - 1) {
                                        InsertImageInPDF(i + (4 * i), item, indexPage, startIndex, X1, Y1);
                                    }
                                    else {
                                        InsertImageInPDF(i + (4 * i), item, indexPage, startIndex, X1, Y1);
                                    }
                                }

                                if (indexThirdCase == -1)
                                    indexPage++;
                                indexThirdCase++;

                                if (indexThirdCase == 2) {
                                    indexPage++;
                                    indexThirdCase = 0;
                                }
                            }
                        }
                    }
                }
				}			
		}
		#endregion

		#region Insertion des images (New version)
		///<summary>Insertion des images New version</summary>
		///<param name="output"></param>
		///<param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		///  <author>rkaina</author>
		///  <since>vendredi 18 août 2006</since>
        protected void InsertImageInPDF(Int64 index, CellCreativesInformation item, double indexPage, int startIndex, int X1, int Y1) {
			Int64 lastIndex=index+5;
            Int64 nbVisuel = 0;

			this.SetCurrentPage((int)indexPage+startIndex);
		
            nbVisuel = item.Visuals.Count;

            Int64 end=0;

            if (nbVisuel < lastIndex)
                end = nbVisuel;
			else
				end=lastIndex;

            if ((nbVisuel % 5) == 0) {
                if (nbVisuel > lastIndex) {
					end=lastIndex;
				}
                else if (nbVisuel == lastIndex) {
					end=lastIndex-1;
				} 
				else {
                    end = nbVisuel;
					index--;
				}
			}
			
			Image imgG ;
			int imgI =0;
			string path="";
			
			for(Int64 i=index;i<end;i++) {
				path="";
				path=item.Visuals[(int)i].Replace("/imagette","");
                path = item.Visuals[(int)i].Replace("/ImagesPresse", "");

                if (item.Vehicle.Id == DBCst.Vehicles.names.directMarketing) {
                    imgG = Image.FromFile(_config.VMCScanPath + path);
                    imgI = this.AddImageFromFilename(_config.VMCScanPath + path, TxImageCompressionType.itcFlate);
                }
                else if (item.Vehicle.Id == DBCst.Vehicles.names.outdoor) {
                    imgG = Image.FromFile(_config.OutdoorScanPath + path);
                    imgI = this.AddImageFromFilename(_config.OutdoorScanPath + path, TxImageCompressionType.itcFlate);
                }
                else {
                    imgG = Image.FromFile(_config.PressScanPath + path);
                    imgI = this.AddImageFromFilename(_config.PressScanPath + path, TxImageCompressionType.itcFlate);
                }

				double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
				double coef = Math.Min((double)1.0,w);
				w = (double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height;
				coef = Math.Min((double)coef, w);
				
				if((end-index==4)&&(nbVisuel!=5)) {
					this.PDFPAGE_ShowImage(imgI,
						X1, Y1,
						192,296,0);
					X1+=192+1;
				}
				else if(end-index==5){
					this.PDFPAGE_ShowImage(imgI,
						X1, Y1,
						213,300,0);
					X1+=213+1;
				}
				else {
					this.PDFPAGE_ShowImage(imgI,
						X1, Y1,
						223,296,0);
					X1+=223+1;
				} 
			}
		} 
		#endregion

		#endregion

        #region Methode Override

        #region GetHtmlHeader
        /// <summary>
        /// Get Html Header
        /// </summary>
        /// <returns>Html Header</returns>
        protected override string GetHtmlBodyStyle() {
            return string.Empty;
        }
        #endregion

        #endregion

        #region Evenement Envoi mail client
        /// <summary>
		/// Rise exception when the customer mail has not been sent
		/// </summary>
		/// <param name="source">Error source></param>
		/// <param name="message">Error message</param>
		private void mail_mailKoHandler(object source, string message){
			throw new Exceptions.MiysisPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
		}
		#endregion

        #region Private Methods

        #region Comparative Period Detail
        /// <summary>
        /// Dates sélectionnées
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="currentModule">Module en cours</param>
        /// <param name="dateFormatText">Booléen date en format texte</param>
        /// <returns>HTML</returns>
        /// <remarks>Date format to be like for example novembre 2004 - janvier 2005</remarks>
        private string GetComparativePeriodDetail(WebSession webSession, int currentModuleId, bool dateFormatText) {
        
            StringBuilder html = new StringBuilder();
            string dateBegin;
            string dateEnd;
            DateTime dateBeginDT;
            DateTime dateEndDT;

            if (currentModuleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                // get date begin and date end according to period type
                dateBeginDT = Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
                dateEndDT = Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);

                // get comparative date begin and date end
                dateBeginDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateBeginDT.Date, webSession.ComparativePeriodType);
                dateEndDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateEndDT.Date, webSession.ComparativePeriodType);

                // Formating date begin and date end
                html.Append("<tr height=\"20\">");
                html.Append("<td class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(2292, webSession.SiteLanguage) + "</td></tr>");
                html.Append("<tr height=\"20\">");
		        html.Append("<td class=\"txtViolet11\" vAlign=\"top\">&nbsp;");
                if (dateFormatText) {
                    dateBegin = WebFunctions.Dates.getPeriodTxt(webSession, dateBeginDT.ToString("yyyyMMdd"));
                    dateEnd = WebFunctions.Dates.getPeriodTxt(webSession, dateEndDT.ToString("yyyyMMdd"));
                }
                else {
                    dateBegin = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(dateBeginDT.ToString("yyyyMMdd"), webSession.PeriodType), webSession.SiteLanguage);
                    dateEnd = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(dateEndDT.ToString("yyyyMMdd"), webSession.PeriodType), webSession.SiteLanguage);
                }
                if (!dateBegin.Equals(dateEnd)) {
                    html.Append(Convertion.ToHtmlString(GestionWeb.GetWebWord(896, webSession.SiteLanguage)) + " ");
                    html.Append(dateBegin);
                    html.Append(" " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " ");
                    html.Append(dateEnd);
                }
                else {
                    html.Append(dateBegin);
                }

                html.Append("</td></tr>");
            }
            return (html.ToString());
        }
        #endregion

        #region Comparative Period Type Detail
        /// <summary>
        /// Dates sélectionnées
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="currentModule">Module en cours</param>
        /// <returns>HTML</returns>
        /// <remarks>Date format to be like for example novembre 2004 - janvier 2005</remarks>
        private string GetComparativePeriodTypeDetail(WebSession webSession, int currentModuleId) {
            StringBuilder html = new StringBuilder();

            if (currentModuleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                // Formating date begin and date end
                html.Append("<tr height=\"20\">");
                html.Append("<td class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(2293, webSession.SiteLanguage) + "</td></tr>");
                html.Append("<tr height=\"20\">");
                html.Append("<td class=\"txtViolet11\" vAlign=\"top\">&nbsp;");
                html.Append(HtmlFunctions.GetComparativePeriodTypeDetail(webSession, currentModuleId));
                html.Append("</td></tr>");
            }
            return (html.ToString());
        }
        #endregion
        #endregion

    }
}
