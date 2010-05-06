#region Information
// Author : D. Mussuma
// Creation : 02/02/2007
#endregion

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;

using TNS.AdExpress.Anubis.Shou.Common;
using TNS.AdExpress.Anubis.Shou.Exceptions;

using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.FrameWork.Date;
using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.DB.Common;

using PDFCreatorPilotLib;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.Ares;
using TNS.Ares.Pdf;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Shou.BusinessFacade
{
	/// <summary>
	/// Genere le  PDF des justificatifs.
	/// </summary>
	public class ShouPdfSystem : Pdf {
	
		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Mnevis Configuration (usefull for PDF layout)
		/// </summary>
		private ShouConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Parametres d'une fiche justificative
		/// </summary>
		private ProofDetail _proofDetail = null;

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ShouPdfSystem(IDataSource dataSource, ShouConfig config, DataRow rqDetails, ProofDetail proofDetail,Theme theme):
            base(theme.GetStyle("Shou")) {
			this._dataSource = dataSource;
			this._config = config;
			this._rqDetails = rqDetails;
			_proofDetail = proofDetail;
			this._webSession = proofDetail.CustomerWebSession;
		}
		#endregion

		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <returns></returns>
		internal string Init() {
			try {
				string shortFName = "";
				string fName =  GetFileName(_rqDetails, ref shortFName);
				bool display = false;
				#if(DEBUG)
				display = true;
				#endif
				base.Init(display, fName, _config.PdfCreatorPilotLogin, _config.PdfCreatorPilotPass);
				this.DocumentInfo_Creator = this.DocumentInfo_Author = _config.PdfAuthor;
				this.DocumentInfo_Subject = _config.PdfSubject;
				this.DocumentInfo_Title = _config.PdfTitle;
				this.DocumentInfo_Producer = _config.PdfProducer;
				this.DocumentInfo_Keywords = _config.PdfKeyWords;				

				return shortFName;
			}
			catch(System.Exception e) {
				throw new ShouPdfException("Impossible to init ShouPdfSystem", e);
			}
		}
		#endregion

		#region Fill
		/// <summary>
		/// Fill the PDF file
		/// </summary>
		internal void Fill() {
			
			try {
				
				#region Proof Parameters
                ProofParameters();
				#endregion
				
				#region Insertion des visuels
				if(_webSession.Visuals!=null && _webSession.Visuals.Count>0){
                    Visuals(_webSession.Visuals[0].ToString(), 0);
				}	
				#endregion
				
				#region Header and Footer
                this.AddHeadersAndFooters(_webSession, true, false,
                    Convertion.ToHtmlString(GestionWeb.GetWebWord(1766, _webSession.SiteLanguage)) + " - " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern),
					0,-1);
				#endregion

			}
			catch(System.Exception e) {
				throw new ShouPdfSystemException(" impossible de remplir le fichier pdf ",e);
			}
		}
		#endregion

		#region Send
		/// <summary>
		/// Envoie du mail
		/// </summary>
		/// <param name="fileName">Nomde du fichier</param>
		internal void Send(string fileName) {
            try {
                ArrayList to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient) {
                    to.Add(s);
                }
                SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(2115, _webSession.SiteLanguage)),
                    WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + "\"" + _webSession.ExportedPDFFileName
                    + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer)
                    + "<br><br>"
                    + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage)),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);
                mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
                mail.SendWithoutThread(false);
            }
            catch (Exception e) {
                throw new ShouPdfException("Impossible to send mail to client", e);
            }
		}
		#endregion

		#region Méthodes Internes

		#region File IO management

		#region GetFileName
		/// <summary>
		/// Generate a valid file name from customer request
		/// </summary>
		/// <param name="rqDetails">Details of the customer request</param>
		/// <param name="shortName">Return value : short name of the File (the method return the complet path)</param>
		/// <returns>Complet File Name String (path + short name)</returns>
		private string GetFileName(DataRow rqDetails,ref string shortName) {
			string pdfFileName;

			try {

				pdfFileName = this._config.PdfPath;
				pdfFileName += @"\" + rqDetails["ID_LOGIN"].ToString() ;

				if(!Directory.Exists(pdfFileName)) {
					Directory.CreateDirectory(pdfFileName);
				}
				shortName = DateTime.Now.ToString("yyyyMMdd_") 
					+ rqDetails["id_static_nav_session"].ToString()
					+ "_"
					+ TNS.Ares.Functions.GetRandomString(30,40);

				pdfFileName += @"\" + shortName + ".pdf";

				string checkPath = 	Regex.Replace(pdfFileName, @"(\.pdf)+", ".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);


				int i = 0;
				while(File.Exists(checkPath)) {
					if(i<=1) {
						checkPath = Regex.Replace(pdfFileName, @"(\.pdf)+", "_"+(i+1)+".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					else {
						checkPath = Regex.Replace(pdfFileName, "(_"+i+@"\.pdf)+", "_"+(i+1)+".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					i++;
				}
				return checkPath;
			}
			catch(System.Exception e) {
				throw(new ShouPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
			}
		}
		#endregion

		#endregion		

		#region Insertion des visuels

		

		/// <summary>
		/// Insertion des visuels
		/// </summary>
		/// <param name="fileList">liste des liens des visuels</param>
		/// <param name="indexPage">Index Nouvelle page</param>
		private void Visuals(string fileList,int indexPage){

			#region variables
			string[] visualList = null;
			int nbVisuals = 0;
			int currentPageLineIndex = 1;
			string[] allMediaFiles = null;
			ArrayList prevEnvironmentPages = null, nextEnvironmentPages = null;
			bool hasRedactionalEnvironment = false;
			string str = null;
			#endregion

			if (fileList != null && fileList.Length > 0 && _proofDetail != null) {

				visualList = fileList.ToString().Split(',');

				if (visualList != null && visualList.Length > 0 && _proofDetail != null) {

					//Nombre de visuels
					nbVisuals = visualList.Length;

					//Nouvelle page
					this.SetCurrentPage((int)indexPage);	


					//Environnement rédactionnel 										
                    allMediaFiles = Directory.GetFiles(_config.ScanPath + _proofDetail.IdMedia + @"\" + _proofDetail.DateCover + @"\" + "imagette", "*.jpg");

					#region Insertion uniquement des visuels
					//Insertion uniquement des visuels				
					InsertVisuals(visualList, nbVisuals, ref currentPageLineIndex, false, true, 1.0);

					#endregion

					#region Environnement rédactionnel

					//Obtention des pages d'environnement rédéctionnel
					prevEnvironmentPages = GetEnvironmentPages(_proofDetail, visualList[0].ToString(), allMediaFiles, true);
					nextEnvironmentPages = GetEnvironmentPages(_proofDetail, visualList[visualList.Length - 1].ToString(), allMediaFiles, false);

					if ((prevEnvironmentPages != null && prevEnvironmentPages.Count > 0) || (nextEnvironmentPages != null && nextEnvironmentPages.Count > 0))
						hasRedactionalEnvironment = true;

					//Titre Environnement rédactionnel
					if (hasRedactionalEnvironment) {

						//Environnement rédactionnel
						currentPageLineIndex = 0;
						this.NewPage();
						this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;


						str = GestionWeb.GetWebWord(2113, _webSession.SiteLanguage);
                        Style.GetTag("ShouTitleFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
						this.PDFPAGE_TextOut(this.LeftMargin + 10,
							this.TopMargin + 35, 0, str);

					}

					//Récupère les 3 pages avant l'insertion					

					if (prevEnvironmentPages != null && prevEnvironmentPages.Count > 0) {

						InsertVisuals(prevEnvironmentPages, prevEnvironmentPages.Count, ref currentPageLineIndex, true, false, 0.7);
					}

					//Insertion des visuels
					if (hasRedactionalEnvironment) InsertVisuals(visualList, nbVisuals, ref currentPageLineIndex, false, false, 0.7);

					//Récupère les 3 pages après l'insertion					
					if (nextEnvironmentPages != null && nextEnvironmentPages.Count > 0) {

						InsertVisuals(nextEnvironmentPages, nextEnvironmentPages.Count, ref currentPageLineIndex, true, false, 0.7);
					}
					#endregion

				}
			}			

		}

		/// <summary>
		/// Obtient les pages de l'environnement rédactionnel de l'insertion
		/// </summary>
		/// <param name="_proofDetail">Informations de l'insertion</param>
		/// <param name="referenceVisual">visuel de référence</param>
		/// <param name="allMediaFiles">Toutes les pages du support</param>
		/// <param name="pagesBeforeInsertion">Indique si l'on recherche les pages précédent l'insertion</param>
		/// <returns>Liste des pages de l'environnement rédactionnel de l'insertion</returns>
		private ArrayList GetEnvironmentPages(ProofDetail _proofDetail,string referenceVisual,string[] allMediaFiles,bool pagesBeforeInsertion){
			
			ArrayList environmentPages = null;
			string imgPath = null;
			int nbMaxPagesByLine = 5;

            imgPath = _config.ScanPath + _proofDetail.IdMedia + @"\" + _proofDetail.DateCover + @"\" + "imagette" + @"\" + referenceVisual;

		
			for (int j=0; j<allMediaFiles.Length;j++) {
						
				if(imgPath.ToLower().Equals(allMediaFiles[j].ToString().ToLower())) {
					environmentPages = new ArrayList();

					//Récupère les 5 pages avant l'insertion
					if(pagesBeforeInsertion && j>0){						
						for (int k = j - nbMaxPagesByLine; k > 0 && k < j ; k++) {
							environmentPages.Add(allMediaFiles[k].ToString());
						}

					}
					//Récupère les 5 pages après l'insertion
					else if(!pagesBeforeInsertion && j<allMediaFiles.Length-1){
						for(int k = j+1; k < allMediaFiles.Length && k<=j+nbMaxPagesByLine; k++){
							environmentPages.Add(allMediaFiles[k].ToString());
						}
					}
					
					break;
				}																	
			}

			return environmentPages;
		}


		/// <summary>
		/// Insert le(s) visuel(s) dans le pdf
		/// </summary>
		/// <param name="visualList">Liste des visuels</param>
		/// <param name="nbVisuals">Nombre de visuels</param>
		/// <param name="currentPageLineIndex">Index de la ligne courante de la page courante</param>
		/// <param name="environmentPages">Indique s'il s'agit des pages d'environnement rédactionnel</param>
		/// <param name="onlyAdverts">Indique si le traitement porte exclusivement sur les publicités</param>
		/// <param name="zoomValue">Coefficient d'aggrandissement du visuel</param>
		private void InsertVisuals(IList visualList,int nbVisuals,ref int currentPageLineIndex,bool environmentPages,bool onlyAdverts,double zoomValue){
			
			#region variables			
			int xPos = 0 , imgI = 0;
			double X1 = 0, Y1 = 0;
			string imgPath = null;			
			Image imgG ;
			int nbMaxLinesByPage = 0;
			int nbPagesBylines = 0;
			#endregion


			nbMaxLinesByPage = (onlyAdverts) ? 2 : 3;
			nbPagesBylines = (onlyAdverts || (nbVisuals == 4)) ? 4 : 5;

			//Nouvelle ligne			
			if (currentPageLineIndex < nbMaxLinesByPage) currentPageLineIndex++;
			else {
				currentPageLineIndex = currentPageLineIndex - (nbMaxLinesByPage - 1);
				this.NewPage();
				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;
			}

			#region Insertion de chaque visuel

			//Insertion de chaque visuel
			for (int i = 0; i < nbVisuals; i++) {

				//Nouvelle ligne
				if (i % nbPagesBylines == 0 && i > 0) {
					xPos = 0;
					if (currentPageLineIndex < nbMaxLinesByPage) currentPageLineIndex++;
					else {
						currentPageLineIndex = currentPageLineIndex - (nbMaxLinesByPage - 1);
						this.NewPage();
						this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;
					}
				}



				if (environmentPages) imgPath = visualList[i].ToString();
                else imgPath = _config.ScanPath + _proofDetail.IdMedia + @"\" + _proofDetail.DateCover + @"\" + "imagette" + @"\" + visualList[i].ToString();



				imgG = Image.FromFile(imgPath);
				imgI = this.AddImageFromFilename(imgPath, TxImageCompressionType.itcFlate);
				double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin) / (double)imgG.Width;
				double coef = Math.Min((double)zoomValue, w);
				w = (double)(this.WorkZoneBottom - this.WorkZoneTop) / (double)imgG.Height;
				coef = Math.Min((double)coef, w);

				switch (xPos) {
					case 0:
						//position X 1ere visuel
						if (nbVisuals == 1)
							X1 = (double)(this.PDFPAGE_Width / 2 - (imgG.Width * zoomValue) / 2);
						else if (nbVisuals == 2)
							X1 = (double)(this.PDFPAGE_Width / 2 - imgG.Width * zoomValue);
						else if (nbVisuals == 3)
							X1 = (double)((this.PDFPAGE_Width - imgG.Width * zoomValue * 3 + 4) / 2);
						else if (nbVisuals >= 4 || environmentPages)
							X1 = (double)((this.PDFPAGE_Width - imgG.Width * zoomValue * nbPagesBylines + 6) / 2);
						break;
					case 1:
						//position X 2eme visuel
						if (nbVisuals == 2)
							X1 = (double)(this.PDFPAGE_Width / 2 + 2);
						else if (nbVisuals >= 3)
							X1 = X1 + imgG.Width * zoomValue + 2;
						break;
					case 2:
					case 3:
					case 4:
						//position X 3eme ou 4eme visuel
						X1 = X1 + imgG.Width * zoomValue + 2;
						break;
				}

				//Position Y des visuels
				switch (currentPageLineIndex) {
					case 1: Y1 = 70; break;
					case 2: Y1 = (onlyAdverts) ? 410 : 290; break;
					case 3: Y1 = 510; break;
					default: break;
				}


				//Intègre visuel(s) dans pdf
				this.PDFPAGE_ShowImage(imgI,
					X1, Y1,
					(double)(coef * imgG.Width), (double)(coef * imgG.Height), 0);

				xPos++;

			}
			#endregion
		}
		#endregion

		#region ProofParameters
		/// <summary>
		/// Proof parameter design
		/// </summary>
		private void ProofParameters() {

            StringBuilder html = new StringBuilder();
			string styleTitle = "class=\"jusT\"";
			string styleValue = "class=\"jusV\"";
			string tableCss = "class=\"jus\"";
            double currentLeftMargin = this.LeftMargin;
            double currentRightMargin = this.RightMargin;

			#region GetData
			DataTable dtResult = TNS.AdExpress.Web.Rules.Results.ProofRules.GetProofFileData(_webSession, _proofDetail.IdMedia, _proofDetail.IdProduct, _proofDetail.DateCover, _proofDetail.MediaPaging, _proofDetail.DateParution);

			#endregion

            try {

                html.Append("<TABLE align=center WIDTH=100%><tr align=center>");

                #region Debut Tableau général
                html.Append("<table  " + tableCss + " align=\"center\" cellSpacing=\"0\" cellSpacing=\"0\">");

                //Titre
                html.Append("<TR  align=\"center\" height=\"10\" ><TD class=\"jus\" align=\"center\"  colspan=2>" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1766, _webSession.SiteLanguage)) + "</TD></TR>");
                html.Append("<tr  align=\"center\" >");
                #endregion

                #region Couverture



                html.Append("<TD width=\"50%\" valign=\"top\" bgcolor=\"#E9E6EF\" ><TABLE align=\"center\" width=\"100%\" cellpadding=\"0\" cellSpacing=\"0\" border=\"0\">");
                html.Append("<tr><td >");//Debut cellule image couverture
                html.Append("&nbsp;</td><tr>");//fin cellule image couverture				

                html.Append("</TABLE></td>");
                #endregion

                html.Append("<td width=\"50%\" valign=\"top\" >");//Debut cellule détail fiche
                #region Détail fiche justificative
                html.Append("<TABLE align=\"center\" width=\"100%\" valign=\"top\" cellpadding=\"0\" cellSpacing=\"0\" border=\"0\">");

                //Numero de page
                if (dtResult.Rows[0]["media_paging"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD  " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(894, _webSession.SiteLanguage)) + " </TD><TD  " + styleValue + "  nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["media_paging"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Annonceur
                if (dtResult.Rows[0]["advertiser"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD  " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(857, _webSession.SiteLanguage)) + " </TD><TD  " + styleValue + "  nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["advertiser"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Produit
                if (dtResult.Rows[0]["product"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD  " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(858, _webSession.SiteLanguage)) + " </TD><TD  " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["product"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Groupe
                if (dtResult.Rows[0]["group_"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD   " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(859, _webSession.SiteLanguage)) + " </TD><TD   " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["group_"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Surface en page
                if (dtResult.Rows[0]["area_page"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD   " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1767, _webSession.SiteLanguage)) + " </TD><TD  " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["area_page"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Surface en mmc
                if (dtResult.Rows[0]["area_mmc"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD   " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1768, _webSession.SiteLanguage)) + " </TD><TD  " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["area_mmc"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Descriptif
                if (dtResult.Rows[0]["location"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD   " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1769, _webSession.SiteLanguage)) + " </TD><TD  " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["location"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Format
                if (dtResult.Rows[0]["format"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD   " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1420, _webSession.SiteLanguage)) + " </TD><TD  " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["format"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Couleur
                if (dtResult.Rows[0]["color"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD  " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1438, _webSession.SiteLanguage)) + " </TD><TD   " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["color"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Rang famille
                if (dtResult.Rows[0]["rank_sector"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD  " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1426, _webSession.SiteLanguage)) + " </TD><TD   " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["rank_sector"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Rang groupe
                if (dtResult.Rows[0]["rank_group_"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD   " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1427, _webSession.SiteLanguage)) + " </TD><TD   " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["rank_group_"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Rang support
                if (dtResult.Rows[0]["rank_media"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD   " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1428, _webSession.SiteLanguage)) + " </TD><TD  " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["rank_media"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                //Investissement
                if (dtResult.Rows[0]["expenditure_euro"] != System.DBNull.Value) {
                    html.Append("<TR  valign=\"top\">");
                    html.Append("<TD   " + styleTitle + " nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1770, _webSession.SiteLanguage)) + "  (" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1423, _webSession.SiteLanguage)) + ") </TD><TD  " + styleValue + " nowrap>  &nbsp;&nbsp;&nbsp;" + dtResult.Rows[0]["expenditure_euro"].ToString() + "</TD>");
                    html.Append("</TR>");
                }

                html.Append("</TABLE>");

                #endregion
                html.Append("</td>");//fin cellule détail fiche

                #region Fin Tableau général
                html.Append("</tr>");
                html.Append("</table>");
                #endregion

                html.Append("</TR></TABLE >");

                this.LeftMargin = 170;
                this.RightMargin = 170;
                this.ConvertHtmlToPDF(html.ToString(),
                    WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                    WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                    _config.WebServer,
                    _config.Html2PdfLogin,
                    _config.Html2PdfPass,
                    0);

                #region Insertion Couverture

                string imgPath = _config.ScanPath + _proofDetail.IdMedia + @"\" + _proofDetail.DateCover + @"\" + "imagette" + @"\" + TNS.AdExpress.Constantes.Web.CreationServerPathes.COUVERTURE;
                if (File.Exists(imgPath)) {
                    Image imgG = Image.FromFile(imgPath);
                    double zoomValue = 0.6;
                    int imgI = this.AddImageFromFilename(imgPath, TxImageCompressionType.itcFlate);
                    double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin) / (double)imgG.Width;
                    double coef = Math.Min((double)zoomValue, w);
                    w = (double)(this.WorkZoneBottom - this.WorkZoneTop) / (double)imgG.Height;
                    coef = Math.Min((double)coef, w);

                    this.PDFPAGE_ShowImage(imgI, 250, 85, (double)(coef * imgG.Width), (double)(coef * imgG.Height), 0);
                }
                string str = null;
                Style.GetTag("ShouTitleFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);


                //Support
                if (dtResult.Rows[0]["Media"] != System.DBNull.Value) {
                    str = Convertion.ToHtmlString(GestionWeb.GetWebWord(971, _webSession.SiteLanguage)) + " : " + dtResult.Rows[0]["Media"].ToString();
                    this.PDFPAGE_TextOut(250,
                        300, 0, str);

                }
                Style.GetTag("ShouTitleFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);

                //Date de parution
                if (dtResult.Rows[0]["datePublication"] != System.DBNull.Value) {
                    str = Convertion.ToHtmlString(GestionWeb.GetWebWord(1381, _webSession.SiteLanguage)) + " : " + Dates.DateToString((DateTime)dtResult.Rows[0]["datePublication"], _webSession.SiteLanguage);
                    this.PDFPAGE_TextOut(250,
                        315, 0, str);
                }

                //Nombre de pages
                if (dtResult.Rows[0]["number_page_media"] != System.DBNull.Value) {
                    str = Convertion.ToHtmlString(GestionWeb.GetWebWord(1385, _webSession.SiteLanguage)) + " : " + dtResult.Rows[0]["number_page_media"].ToString();
                    this.PDFPAGE_TextOut(250,
                        330, 0, str);
                }

                #endregion

            }
            catch (System.Exception e) {
                throw (new Exceptions.ShouPdfException("Unable to build the session parameter page for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
            finally {
                this.LeftMargin = currentLeftMargin;
                this.RightMargin = currentRightMargin;
            }
		}
		#endregion

		#region Evenement Envoi mail client
		/// <summary>
		/// Rise exception when the customer mail has not been sent
		/// </summary>
		/// <param name="source">Error source></param>
		/// <param name="message">Error message</param>
		private void mail_mailKoHandler(object source, string message) {
			throw new Exceptions.ShouPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
		}
		#endregion

		#endregion

	}
}
