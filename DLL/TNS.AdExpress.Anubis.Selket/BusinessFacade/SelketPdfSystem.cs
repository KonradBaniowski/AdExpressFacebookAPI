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

using TNS.AdExpress.Anubis.Selket.Common;
using TNS.AdExpress.Anubis.Selket.Exceptions;

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
using TNS.AdExpress.Domain.Web.Navigation;
using DomainLevel = TNS.AdExpress.Domain.Level;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.Ares;
using TNS.Ares.Pdf;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Anubis.Selket.Common;
using TNS.AdExpress.Anubis.Selket.Exceptions;
using TNS.AdExpressI.VP;
using TNS.AdExpressI.VP.DAL;
#endregion

namespace TNS.AdExpress.Anubis.Selket.BusinessFacade
{
	/// <summary>
	/// Generate the PDF document for Selket Pdf System module.
	/// </summary>
	public class SelketPdfSystem : Pdf{

		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Appm Configuration (usefull for PDF layout)
		/// </summary>
		private SelketConfig _config = null;
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
        public SelketPdfSystem(IDataSource dataSource, SelketConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
            :
        base(theme.GetStyle("Selket")) {
            try {
                this._dataSource = dataSource;
                this._config = config;
                this._rqDetails = rqDetails;
                this._webSession = webSession;
            }
            catch (Exception e) {
                throw new SelketPdfException("Error in Constructor Selket Pdf System", e);
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
                throw new SelketPdfException("Error to initialize SelketPdfSystem in Init()", e);
			}
		}
		#endregion

		#region Fill
		internal void Fill(){

			try{

				#region MainPage
				MainPageDesign();
				#endregion
			
				#region Impression
                VpFileResult();
				#endregion				

				#region Header and Footer
				string dateString = Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

                string footertext = (_webSession.IdPromotion > -1) ? GestionWeb.GetWebWord(2893, _webSession.SiteLanguage) : GestionWeb.GetWebWord(2899, _webSession.SiteLanguage);
				this.AddHeadersAndFooters(
				_webSession,
				imagePosition.leftImage,
                footertext + " " + GestionWeb.GetWebWord(2858, _webSession.SiteLanguage) + " - " + dateString,
				0, -1, true);
				#endregion
				
			}
			catch(System.Exception e){
                throw new SelketPdfException("Error to Fill Pdf in Fill()" + e.StackTrace + e.Source, e);
			}
		}
		#endregion

		#region Send
		internal void Send(string fileName){
          

            try {
                ArrayList to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient) {
                    to.Add(s);
                }
                SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    GestionWeb.GetWebWord(2897, _webSession.SiteLanguage),
                    GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + "\"" + _webSession.ExportedPDFFileName
                    + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer)
                    + "<br><br>"
                    + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);
                mail.SubjectEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                mail.BodyEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
                mail.SendWithoutThread(false);
            }
            catch (System.Exception e) {
                throw new SelketPdfException("Error to Send mail to client in Send(string fileName)", e);
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
                throw (new SelketPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
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

            string headertext = (_webSession.IdPromotion > -1) ? GestionWeb.GetWebWord(2893, _webSession.SiteLanguage) : GestionWeb.GetWebWord(2899, _webSession.SiteLanguage);
            this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(headertext + " " + GestionWeb.GetWebWord(2858, _webSession.SiteLanguage))) / 2,
                    (this.PDFPAGE_Height) / 4, 0, headertext + " " + GestionWeb.GetWebWord(2858, _webSession.SiteLanguage));			
			
            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

            Style.GetTag("createdTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
			this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str))/2, 
				1*this.PDFPAGE_Height/3,0,str);

		}
		#endregion		

		#region vP File result
        /// <summary>
        /// Get VP promo file string
        /// </summary>
		protected virtual  void VpFileResult(){

			#region GETHTML
			StringBuilder html=new StringBuilder(10000);		    			
            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            object[] param = null;
             DataSet ds = null;

            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(WebConstantes.Module.Name.VP);
            if(_webSession.IdPromotion>-1){
            param = new object[1] { _webSession };
            IVeillePromoDAL vpDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
             ds =  vpDAL.GetData(_webSession.IdPromotion);
            }else{
            param = new object[1] { _webSession };
            IVeillePromo vpResult = (IVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
           
             param = new object[3] { _webSession,vpResult.PeriodBeginningDate, vpResult.PeriodEndDate};
            IVeillePromoDAL vpDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
             ds =  vpDAL.GetData(_webSession.IdPromotion);
            }

            //DataRow dr = null;
            
          
			try{

              
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    //dr = ds.Tables[0].Rows[0];

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        #region file result
                        html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" height=\"100%\">");

                        #region Header
                        //Header
                        html.Append("<tr><td class=\"VpFilePopUpHeader\">");
                        html.AppendFormat("&nbsp;&nbsp;{0} ", GestionWeb.GetWebWord(2883, _webSession.SiteLanguage));
                        html.Append("</td></tr>");
                        #endregion

                        #region Content
                        html.Append("<tr>");
                        html.Append("<td width=\"100%\">");//class=\"vpfContent\"
                        html.Append("<div class=\"vpfContent\">");
                        html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" height=\"100%\">");

                        //Content
                        html.Append("<tr  width=\"100%\"><td class=\"vpfDescrPdf\" width=\"100%\">");
                        html.Append("<ul class=\"prf\">");

                        //VP Brand
                        html.AppendFormat(" <li ><span><b>{0}:</b></span><span class=\"prfSp\"> {1}</span></li>", GestionWeb.GetWebWord(2876, _webSession.SiteLanguage), dr["BRAND"].ToString());

                        string dateBegin = TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY(Convert.ToInt32(dr["DATE_BEGIN_NUM"].ToString()), _webSession.SiteLanguage);
                        string dateEnd = TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY(Convert.ToInt32(dr["DATE_END_NUM"].ToString()), _webSession.SiteLanguage);
                        //Date
                        html.AppendFormat(" <li ><span><b>{0}:</b></span><span class=\"prfSp\"> {1} {2} {3} {4}</span></li>", GestionWeb.GetWebWord(895, _webSession.SiteLanguage), GestionWeb.GetWebWord(896, _webSession.SiteLanguage), dateBegin, GestionWeb.GetWebWord(897, _webSession.SiteLanguage), dateEnd);

                        //Product classification
                        html.AppendFormat("  <li ><span><b>{0}:</b></span><span class=\"prfSp\"> {1} /{2} / {3}</span></li>", GestionWeb.GetWebWord(2884, _webSession.SiteLanguage), dr["SEGMENT"].ToString(), dr["CATEGORY"].ToString(), dr["PRODUCT"].ToString());

                        //Promotion Content
                        if (dr["PROMOTION_CONTENT"] != System.DBNull.Value && dr["PROMOTION_CONTENT"].ToString().Length > 0)
                        {
                            string promoContent = dr["PROMOTION_CONTENT"].ToString();
                            html.AppendFormat("  <li><span><b>{0}:</b></span><span class=\"prfSp\"> {1}</span></li>", GestionWeb.GetWebWord(2885, _webSession.SiteLanguage), promoContent);
                        }

                        //Brands
                        if (dr["PROMOTION_BRAND"] != System.DBNull.Value && dr["PROMOTION_BRAND"].ToString().Length > 0)
                        {
                            string[] brands = dr["PROMOTION_BRAND"].ToString().Split(',');
                            html.AppendFormat(" <li><span><b>{0}:</b></span>", GestionWeb.GetWebWord(1149, _webSession.SiteLanguage));

                            if (brands.Length == 1)
                            {
                                html.AppendFormat("<span class=\"prfSp\"> {0}</span>", brands[0]);
                            }
                            else
                            {
                                html.Append(" <ul class=\"prfbd\">");
                                for (int i = 0; i < brands.Length; i++)
                                {
                                    html.AppendFormat("<li ><span class=\"prfSp\">{0}</span> </li>", brands[i]);
                                }
                                html.Append(" </ul>");
                            }
                            html.Append(" </li>");

                        }
                        //Promotion's Conditions
                        if (dr["CONDITION_TEXT"] != System.DBNull.Value && dr["CONDITION_TEXT"].ToString().Length > 0)
                        {
                            string conditionText = dr["CONDITION_TEXT"].ToString();

                            html.AppendFormat(" <li ><span><b>{0}:</b></span><span class=\"prfSp\">", GestionWeb.GetWebWord(2886, _webSession.SiteLanguage));
                            html.Append("  <br />");
                            html.Append(conditionText);
                            html.Append("</span></li>");

                        }
                        html.Append("</ul></td></tr>");

                        html.Append("</table>");
                        html.Append("</div>");
                        html.Append("</td>");
                        html.Append("</tr>");
                        #endregion

                        html.Append("</table>");


                        this.ConvertHtmlToPDF(html.ToString(),
                            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                            _config.WebServer,
                            _config.Html2PdfLogin,
                            _config.Html2PdfPass);

                        //Add promotion visuals
                        if (dr["PROMOTION_VISUAL"] != System.DBNull.Value && dr["PROMOTION_VISUAL"].ToString().Length > 0)
                        {
                            string[] promoVisuals = dr["PROMOTION_VISUAL"].ToString().Split(',');
                            AddpromoVisuals(promoVisuals);
                        }
                        //Add promotion conditionnal visuals
                        if (dr["CONDITION_VISUAL"] != System.DBNull.Value && dr["CONDITION_VISUAL"].ToString().Length > 0)
                        {
                            string[] promoVisuals = dr["CONDITION_VISUAL"].ToString().Split(',');
                            AddpromoVisuals(promoVisuals);
                        }
                        #endregion

                        html = new StringBuilder(10000);
                    }
                }             
               

				
            
            }
			catch(System.Exception err){
				throw(new SelketPdfException("Unable to process vp Schedule export result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",err)); 
			}
			finally{
				_webSession.CurrentModule = module.Id;
			}
			#endregion
		}
		#endregion
		

        #region Add visuals

        protected virtual void AddpromoVisuals(string[] imgList)
        {
            if (imgList != null && imgList.Length > 0)
            {
                for (int i = 0; i < imgList.Length; i++)
                {
                    string visualPromo = WebConstantes.CreationServerPathes.LOCAL_PATH_VP + imgList[i];
                    if (File.Exists(visualPromo))
                    {
                        this.NewPage();
                        this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;                        
                        int imgI = 0;
                        double w = 0;                      

                        Image imgG = Image.FromFile(visualPromo);
                        imgI = this.AddImageFromFilename(visualPromo, TxImageCompressionType.itcFlate);
                        w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin) / (double)imgG.Width;
                        double coef = coef = Math.Min((double)1.0, w);
                        w = ((double)(this.WorkZoneBottom - this.WorkZoneTop) / (double)imgG.Height);
                        coef = Math.Min((double)coef, w);
                        double X1 = (double)(this.PDFPAGE_Width / 2 - (coef * imgG.Width) / 2);
                        double Y1 = (double)(this.PDFPAGE_Height / 2 - (coef * imgG.Height) / 2);

                        this.PDFPAGE_ShowImage(imgI,
                        X1, Y1,
                        coef * imgG.Width,
                        coef * imgG.Height,
                        0);

                        imgG.Dispose();
                        imgG = null;
                    }

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
        protected override string GetHtmlHeader(string charset, string themeName, string serverName)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >");
            html.Append("<HTML>");
            html.Append("<HEAD>");
            html.Append("<META http-equiv=\"Content-Type\" content=\"text/html; charset=" + charset + "\">");
            html.Append("<meta content=\"Microsoft Visual Studio .NET 7.1\" name=\"GENERATOR\">");
            html.Append("<meta content=\"C#\" name=\"CODE_LANGUAGE\">");
            html.Append("<meta content=\"JavaScript\" name=\"vs_defaultClientScript\">");
            html.Append("<meta content=\"http://schemas.microsoft.com/intellisense/ie5\" name=\"vs_targetSchema\">");
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/AdExpress.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/GenericUI.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/MediaSchedule.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/Vp.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<meta http-equiv=\"expires\" content=\"Wed, 23 Feb 1999 10:49:02 GMT\">");
            html.Append("<meta http-equiv=\"expires\" content=\"0\">");
            html.Append("<meta http-equiv=\"pragma\" content=\"no-cache\">");
            html.Append("<meta name=\"Cache-control\" content=\"no-cache\">");
            html.Append("</HEAD>");
            html.Append("<body " + GetHtmlBodyStyle() + ">");
            html.Append("<form>");
            return html.ToString();
        }
       

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
			throw new Exceptions.SelketPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
		}
		#endregion

      
    }
}
