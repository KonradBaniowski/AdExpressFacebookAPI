using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using ConstFrameWorkResults = TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpress.Web.Controls.Results {
	/// <summary>
	/// Description résumée de BannerInformationWebControl.
	/// </summary>
	[DefaultProperty("Text"),
		ToolboxData("<{0}:BannerInformationWebControl runat=server></{0}:BannerInformationWebControl>")]
	public class BannerInformationWebControl : System.Web.UI.WebControls.WebControl {

		#region Constantes
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

		#region Variables
		/// <summary>
		/// Banner Id
		/// </summary>
		Int64 _hashcode = 0;
		/// <summary>
		/// Banner file path
		/// </summary>
		private string _filePath = AppDomain.CurrentDomain.BaseDirectory + @"Images\Common\logoTNShome.gif";
		/// <summary>
		/// Banner dimension
		/// </summary>
		private string _dimension = "185*90";
		/// <summary>
		/// Banner file type
		/// </summary>
		private int _formatId = 0;
		/// <summary>
		/// Banner file type
		/// </summary>
		private string _format = string.Empty;
		/// <summary>
		/// Banner Link
		/// </summary>
		string _linkBanner = string.Empty;
		/// <summary>
		/// Product Label
		/// </summary>
		string _productLabel = string.Empty;
		/// <summary>
		/// Product Id
		/// </summary>
		Int64 _productId = 0;
		/// <summary>
		/// advertiser Label
		/// </summary>
		string _advertiserLabel = string.Empty;
		/// <summary>
		/// Advertiser Id
		/// </summary>
		Int64 _advertiserId = 0;
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession = null;
		/// <summary>
		/// Save link Css 
		/// </summary>
		protected string _cssLink = "";
		/// <summary>
		/// Text Css 
		/// </summary>
		protected string _cssText = "";

		/// <summary>
		/// Zoom de La période sélectionnée
		/// </summary>
		protected string _zoomDate = string.Empty;

		/// <summary>
		/// Paramètres d'url
		/// </summary>
		protected string _urlParameters = string.Empty;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Get/Set Text Css 
		/// </summary>
		[Bindable(true),
		Category("Appearance"),
		Description("TextCss ")]
		public string CssText {
			get { return _cssText; }
			set { _cssText = value; }
		}

		/// <summary>
		/// Get/Set Save link Css 
		/// </summary>
		[Bindable(true),
		Category("Appearance"),
		Description("Save link Css ")]
		public string CssLink {
			get { return _cssLink; }
			set { _cssLink = value; }
		}
		/// <summary>
		/// Get/Set  Customer Session
		/// </summary>
		[Bindable(false)]
		public WebSession CustomerWebSession {
			get { return (_customerWebSession); }
			set { _customerWebSession = value; }
		}
		/// <summary>
		/// Get/Set banner Id
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public Int64 Hashcode {
			get { return (_hashcode); }
			set { _hashcode = value; }
		}
		/// <summary>
		/// Get/Set  Banner file path
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public string FilePath {
			get { return (_filePath); }
			set { _filePath = value; }
		}
		/// <summary>
		/// Get/Set Banner dimension
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public string Dimension {
			get { return (_dimension); }
			set { _dimension = value; }
		}

		/// <summary>
		/// Get/Set Banner format Id
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public int FormatId {
			get { return (_formatId); }
			set { _formatId = value; }
		}

		/// <summary>
		/// Get/Set Banner link
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public string LinkBanner {
			get { return (_linkBanner); }
			set { _linkBanner = value; }
		}

		/// <summary>
		/// Get/Set Product Label
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public string ProductLabel {
			get { return (_productLabel); }
			set { _productLabel = value; }
		}

		/// <summary>
		/// Get/Set Product Id
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public Int64 ProductLId {
			get { return (_productId); }
			set { _productId = value; }
		}
		/// <summary>
		/// Get/Set Advertiser Label
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public string AdvertiserLabel {
			get { return (_advertiserLabel); }
			set { _advertiserLabel = value; }
		}

		/// <summary>
		/// Get/Set Advertiser Id
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue(0)]
		public Int64 AdvertiserLId {
			get { return (_advertiserId); }
			set { _advertiserId = value; }
		}


		/// <summary>
		/// Get/Set Zoom date
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue("")]
		public string ZoomDate {
			get { return _zoomDate; }
			set { _zoomDate = value; }
		}

		/// <summary>
		/// Get/Set url Parameters
		/// </summary>
		[Bindable(true),
		Category("Appearance"),
		Description("Url paramètres ")]
		public string UrlParameters {
			get { return _urlParameters; }
			set { _urlParameters = value; }
		}
		#endregion

		#region PréRender
		/// <summary>
		/// PréRender
		/// </summary>
		/// <param name="e">Event Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if (_customerWebSession != null) {
				TNS.AdExpress.Web.Controls.Results.DisplayBannerWebControl displayBannerWebControl = new TNS.AdExpress.Web.Controls.Results.DisplayBannerWebControl();
				displayBannerWebControl.FilePath = _filePath;
				displayBannerWebControl.Dimension = _dimension;
				displayBannerWebControl.FormatId = _formatId;
				displayBannerWebControl.LinkBanner = _linkBanner;
				displayBannerWebControl.ActiveLink = true;
				displayBannerWebControl.LanguageId = _customerWebSession.SiteLanguage;
				// The customer have the right to save the banner
				if (_customerWebSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_DOWNLOAD_ACCESS_FLAG) != null) {
					displayBannerWebControl.CanSave = true;
				}
				switch (_formatId) {
					case 0:
						_format = GIF_TEXT;
						break;
					case 1:
						_format = JPEG_TEXT;
						break;
					case 3:
						_format = FLASH_TEXT;
						break;
				}
				displayBannerWebControl.CssSaveLink = _cssLink;
				this.Controls.Add(displayBannerWebControl);
			}
		}

		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			if (_customerWebSession != null) {

				string cssText = "";
				if (_cssText.Length > 0) cssText = " class=\"" + _cssText + "\" ";
				output.Write("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"1\"  bgcolor=\"#B1A3C1\">");
				// Visuel
				output.Write("<tr bgcolor=\"#D0C8DA\"><td>");
				Controls[0].RenderControl(output);
				output.Write("</td></tr>");
				output.Write("<tr><td >");
				output.Write("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"5\" " + cssText + " bgcolor=\"#B1A3C1\">");
				output.Write("<tr nowrap>");
				output.Write("<td width=\"150\" nowrap><strong>" + GestionWeb.GetWebWord(857, _customerWebSession.SiteLanguage) + "&nbsp;:</strong></td>");
				output.Write("<td nowrap>" + _advertiserLabel + "</td>");
				output.Write("</tr>");
				if (_customerWebSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.MEDIA_SCHEDULE_ADNETTRACK_ACCESS_FLAG) != null) {
					output.Write("<tr nowrap>");
					output.Write("<td width=\"150\" nowrap><strong>" + GestionWeb.GetWebWord(2154, _customerWebSession.SiteLanguage) + "&nbsp;:</strong></td>");
					output.Write("<td nowrap>" + _productLabel + "</td>");
					output.Write("</tr>");
				}
				output.Write("<tr nowrap>");
				output.Write("<td width=\"150\" nowrap><strong>" + GestionWeb.GetWebWord(2155, _customerWebSession.SiteLanguage) + "&nbsp;:</strong></td>");
				output.Write("<td nowrap>" + _format + "</td>");
				output.Write("</tr>");
				if (_customerWebSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.MEDIA_SCHEDULE_ADNETTRACK_ACCESS_FLAG) != null) {
					output.Write("<tr nowrap>");
					output.Write("<td width=\"150\" nowrap><strong>" + GestionWeb.GetWebWord(2156, _customerWebSession.SiteLanguage) + "&nbsp;:</strong></td>");
					output.Write("<td nowrap><a href=\"" + MEDIA_SCHEDULE_PATH + "?idSession=" + _customerWebSession.IdSession + "&idLevel=" + ConstFrameWorkResults.AdNetTrackMediaSchedule.Type.advertiser.GetHashCode() + "&id=" + _advertiserId + "&zoomDate=" + _zoomDate + "&urlParameters=" + _urlParameters + "\" class=\"" + _cssLink + "\">" + GestionWeb.GetWebWord(857, _customerWebSession.SiteLanguage) + "</a> | <a href=\"" + MEDIA_SCHEDULE_PATH + "?idSession=" + _customerWebSession.IdSession + "&idLevel=" + ConstFrameWorkResults.AdNetTrackMediaSchedule.Type.product.GetHashCode() + "&id=" + _productId + "&zoomDate=" + _zoomDate + "&urlParameters=" + _urlParameters + "\" class=\"" + _cssLink + "\">" + GestionWeb.GetWebWord(858, _customerWebSession.SiteLanguage) + "</a> | <a href=\"" + MEDIA_SCHEDULE_PATH + "?idSession=" + _customerWebSession.IdSession + "&idLevel=" + ConstFrameWorkResults.AdNetTrackMediaSchedule.Type.visual.GetHashCode() + "&id=" + _hashcode + "&zoomDate=" + _zoomDate + "&urlParameters=" + _urlParameters + "\" class=\"" + _cssLink + "\">" + GestionWeb.GetWebWord(1909, _customerWebSession.SiteLanguage) + "</a></td>");
					output.Write("</tr>");
				}
				output.Write("</table>");
				output.Write("</td>");
				output.Write("</tr>");
				output.Write("</table><br>");
			}
			//output.Write(Text);
		}

		/// <summary>
		/// Obtient le rendu HTML du contrôle
		/// <remarks>Est utilisé pour la pagination des résultats</remarks>
		/// </summary>
		internal string GetRender() {
			this.OnPreRender(null);
			HtmlTextWriter txt = new HtmlTextWriter(new StringWriter());
			this.Render(txt);
			string str = txt.InnerWriter.ToString();
			txt = null;
			return str;
		}
		#endregion
	}
}
