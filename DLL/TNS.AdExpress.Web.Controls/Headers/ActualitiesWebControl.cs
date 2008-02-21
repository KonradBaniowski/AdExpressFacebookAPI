#region Informations
// Auteur: B.Masson, G.Facon
// Date de création : 16/01/2007
// Date de modification :
#endregion

using System;
using System.Text;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.FrameWork.Net.Rss;
using TNS.FrameWork.Net.Rss.Channel;
using TNS.FrameWork.Net.Rss.Collections;
using TNS.FrameWork.Net.Rss.Item;
using TNS.AdExpress.Web.Core.Translation;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Composant d'affichage des actualités
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:ActualitiesWebControl runat=server></{0}:ActualitiesWebControl>")]
	public class ActualitiesWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Identifiant du langage
		/// </summary>
		protected int _languageId = 33;
        /// <summary>
        /// Items list
        /// </summary>
        protected ArrayList _items = new ArrayList();

		/// <summary>
		/// URL du fichier CSS
		/// </summary>
		protected string _cssUrlPath = string.Empty;
		/// <summary>
		/// Nom du fichier XML du flux RSS
		/// </summary>
		protected string _rssFileName = string.Empty;
		/// <summary>
		/// URL du fichier XML du flux RSS
		/// </summary>
		protected string _rssFileUrl = string.Empty;
		/// <summary>
		/// URL du fichier XML du flux RSS
		/// </summary>
		protected string _ExternRssFileUrl = string.Empty;

		/// <summary>
		/// Css du titre principal
		/// </summary>
		protected string _titleCss = string.Empty;
		/// <summary>
		/// Css des titres rss
		/// </summary>
		protected string _rssTitleCss = string.Empty;
		/// <summary>
		/// Css des descriptions rss
		/// </summary>
		protected string _rssDescriptionCss = string.Empty;

		/// <summary>
		/// Affichage de l'image
		/// </summary>
		protected bool _displayImage = false;
		/// <summary>
		/// URL de l'image
		/// </summary>
		protected string _imageUrlPath = string.Empty;
		/// <summary>
		/// URL de l'image
		/// </summary>
		protected string _imageRssUrlPath = string.Empty;

		/// <summary>
		/// Taille de la colonne de gauche
		/// </summary>
		protected Unit _columnLeftWidth = new Unit("10px");
		/// <summary>
		/// Taille de la colonne de l'image
		/// </summary>
		protected Unit _columnImageWidth = new Unit("");
		#endregion

		#region Accesseurs
		/// <summary>
		/// Définit l'identifiant de la langue
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("33")]
		public int LanguageId {
			get { return (_languageId); }
			set { _languageId = value; }
		}
		/// <summary>
		/// Obtient ou défini l'url du fichier css
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string CssUrlPath {
			get { return (_cssUrlPath); }
			set { _cssUrlPath = value; }
		}
		/// <summary>
		/// Obtient ou défini l'url du fichier XML du flux RSS
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string RssFileName {
			get {
				return (_rssFileName);
			}
			set {
				_rssFileName = value;
			}
		}
		/// <summary>
		/// Obtient ou défini le css du titre principal
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string TitleCss {
			get { return (_titleCss); }
			set { _titleCss = value; }
		}
		/// <summary>
		/// Obtient ou défini le css des titres rss
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string RssTitleCss {
			get { return (_rssTitleCss); }
			set { _rssTitleCss = value; }
		}
		/// <summary>
		/// Obtient ou défini le css des descriptions rss
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string RssDescriptionCss {
			get { return (_rssDescriptionCss); }
			set { _rssDescriptionCss = value; }
		}
		/// <summary>
		/// Obtient ou défini si l'image doit être afficher ou non
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("false")]
		public bool DisplayImage {
			get { return (_displayImage); }
			set { _displayImage = value; }
		}
		/// <summary>
		/// Obtient ou défini l'url de l'image
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string ImageUrlPath {
			get { return (_imageUrlPath); }
			set { _imageUrlPath = value; }
		}
		/// <summary>
		/// Obtient ou défini l'url de l'image du logo RSS
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string ImageRssUrlPath {
			get { return (_imageRssUrlPath); }
			set { _imageRssUrlPath = value; }
		}
		/// <summary>
		/// Obtient ou défini la taille de la colonne de gauche
		/// </summary>
		[Bindable(true),
		Category("Disposition"),
		DefaultValue("")]
		public Unit ColumnLeftWidth {
			get { return (_columnLeftWidth); }
			set { _columnLeftWidth = value; }
		}
		/// <summary>
		/// Obtient ou défini la taille de la colonne des feeds
		/// </summary>
		[Bindable(true),
		Category("Disposition"),
		DefaultValue("")]
		public Unit ColumnImageWidth {
			get { return (_columnImageWidth); }
			set { _columnImageWidth = value; }
		}
		#endregion

		#region PréRendu
		/// <summary>
		/// PréRendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {

#if DEBUG
            _rssFileUrl = "http://thebes/Rss/" + _languageId.ToString() + "/" + _rssFileName;
            _ExternRssFileUrl = _rssFileUrl;
#else
            _rssFileUrl="http://thebes/Rss/"+_languageId.ToString()+"/"+_rssFileName;
            _ExternRssFileUrl="http://www.tnsadexpress.com/Rss/"+_languageId.ToString()+"/"+_rssFileName;
#endif

            try {
                // Read Rss
                RssFeed feed = RssFeed.Read(_rssFileUrl);
                // Channel
                RssChannel channel = (RssChannel)feed.Channels[0];
                // Rss Items collection
                RssItemCollection items = channel.Items;
                // Write html items in arraylist
                foreach(RssItem item in items) {
                    _items.Add("<font " + ((_rssDescriptionCss.Length > 0) ? " class=\"" + _rssDescriptionCss + "\"" : "") + ">&raquo;&nbsp;<font " + ((_rssTitleCss.Length > 0) ? " class=\"" + _rssTitleCss + "\"" : "") + ">" + item.Title + " : </font>" + item.Description + "</font>");
                }
            }
            catch(System.Exception) {
                _items.Clear();
            }
			base.OnPreRender(e);
		}
		#endregion

		#region Rendu
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			StringBuilder html = new StringBuilder(1000);
			if (_items.Count > 0) {
				html.Append("\r\n<table cellpadding=0 cellspacing=0 border=0 width=\"" + this.Width.ToString() + "\">");
				html.Append("\r\n\t<tr>");
				html.Append("\r\n\t\t<td width=\"" + _columnLeftWidth.ToString() + "\">&nbsp;</td>");
				html.Append("\r\n\t\t<td valign=\"top\" align=\"left\" " + ((_columnImageWidth.Value > 0) ? " width=\"" + _columnImageWidth.ToString() + "\"" : "") + " >" + ((_imageUrlPath.Length > 0) ? " <img src=\"" + _imageUrlPath + "\" border=\"0\">" : "") + "</td>");
				html.Append("\r\n\t\t<td valign=\"top\" align=\"left\" >");

				html.Append("\r\n\t\t\t<table cellpadding=0 cellspacing=0 border=0 width=\"100%\">");
				html.Append("\r\n\t\t\t\t<tr>");
				html.Append("\r\n\t\t\t\t\t<td width=\"100%\" " + ((_titleCss.Length > 0) ? " class=\"" + _titleCss + "\"" : "") + ">" + GestionWeb.GetWebWord(2080, _languageId) + "</td>");
				html.Append("\r\n\t\t\t\t\t<td width=\"14\" " + ((_titleCss.Length > 0) ? " class=\"" + _titleCss + "\"" : "") + ">" + ((_imageRssUrlPath.Length > 0) ? "<a href=\"" + ((_ExternRssFileUrl.Length > 0) ? "" + _ExternRssFileUrl + "\" target=\"blank\"" : "#\"") + "><img src=\"" + _imageRssUrlPath + "\" border=\"0\"></a>" : "") + "</td>");
				html.Append("\r\n\t\t\t</tr>");
                foreach (string currentItem in _items) {
                    html.Append("<tr>\r\n\t\t\t\t\t<td colspan=\"2\">" + currentItem + "</td>\r\n\t\t\t</tr>");
                }
				html.Append("\r\n\t\t\t</table>");

				html.Append("\r\n\t\t</td>");
				html.Append("\r\n\t</tr>");
				html.Append("\r\n</table>");
			}
			output.Write(html.ToString());
		}
		#endregion

	}
}
