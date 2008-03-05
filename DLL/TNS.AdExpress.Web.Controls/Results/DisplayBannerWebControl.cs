#region Informations
// Auteur: B. Masson
// Date de création: 06/07/2005
// Date de modification: 06/07/2005
#endregion

using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Domain.Translation;


namespace TNS.AdExpress.Web.Controls.Results{

	#region Enum
	/// <summary>
	/// Type de format des bannières
	/// </summary>
	public enum FormatType{
		/// <summary>
		/// Gif
		/// </summary>
		gif=0,
		/// <summary>
		/// Jpeg
		/// </summary>
		jpeg=1,
		/// <summary>
		/// Html
		/// </summary>
		html=2,
		/// <summary>
		/// Flash
		/// </summary>
		swf=3
	}
	#endregion

	/// <summary>
	/// Composant DisplayBannerWebControl pour l'affichage du visuel d'une bannière
	/// </summary>
	[DefaultProperty("AccesFichier"),
		ToolboxData("<{0}:DisplayBannerWebControl Language=33 runat=server></{0}:DisplayBannerWebControl>")]
		public class DisplayBannerWebControl : System.Web.UI.WebControls.WebControl {

		#region Constantes
		/// <summary>
		/// Chemin du dossier des bannières
		/// </summary>
		public const string VIRTUAL_DIRECTORY=@"\adnettrackCreatives";
		/// <summary>
		/// Save link webtextId
		/// </summary>
		private const Int64 SAVE_LINK_LABEL_ID=874;
		/// <summary>
		/// Save link Help webtext id
		/// </summary>
		private const Int64 SAVE_LINK_LABEL_HELP_ID=920;
		#endregion

		#region Variables
		/// <summary>
		/// Chemin du fichier
		/// </summary>
		protected string _filePath=AppDomain.CurrentDomain.BaseDirectory+@"Images\Common\logoTNShome.gif";
		/// <summary>
		/// Dimension du fichier
		/// </summary>
		protected string _dimension="185*90";
		/// <summary>
		/// Type de format du fichier
		/// </summary>
		protected int _formatId=0;
		/// <summary>
		/// Booléen pour activer ou non le lien de la bannière
		/// </summary>
		protected bool _activeLink = false;
		/// <summary>
		/// Lien de la bannière
		/// </summary>
		protected string _linkBanner = null;
		/// <summary>
		/// If true the component shows the save link
		/// </summary>
		protected bool _canSave=false;
		/// <summary>
		/// Language Id
		/// </summary>
		protected int _languageId=33;
		/// <summary>
		/// Save link Css 
		/// </summary>
		protected string _cssSaveLink="";

		#endregion

		#region Accesseurs
		/// <summary>
		/// Get/Set Save link Css 
		/// </summary>
		[Bindable(true),
		Description("Save link Css ")]
		public string CssSaveLink{
			get{return _cssSaveLink;}
			set{_cssSaveLink= value;}
		}
		/// <summary>
		/// Get/Set File Path
		/// </summary>
		[Bindable(true),
		Description("File Path")]
		public string FilePath{
			get{return _filePath;}
			set{_filePath= value;}
		}

		/// <summary>
		/// Get/Set Banner dimension
		/// </summary>
		[Bindable(true),
		Description("Banner dimension")]
		public string Dimension{
			get{return _dimension;}
			set{_dimension = value;}
		}

		/// <summary>
		/// Get/Set File format
		/// </summary>
		[Bindable(true),
		Description("File format")]
		public int FormatId{
			get{return _formatId;}
			set{_formatId = value;}
		}

		/// <summary>
		/// Get/Set Activate the link to the advertiser site
		/// </summary>
		[Bindable(true),
		Description("Activate the link to the advertiser site")]
		public bool ActiveLink{
			get{return _activeLink;}
			set{_activeLink = value;}
		}

		/// <summary>
		/// Get/Set Advertiser link
		/// </summary>
		[Bindable(true),
		Description("Advertiser link")]
		public string LinkBanner{
			get{return _linkBanner;}
			set{_linkBanner = value;}
		}

		/// <summary>
		/// Get/Set If true the component shows the save link
		/// </summary>
		[Bindable(true),
		Description("If true the component shows the save link")]
		public bool CanSave{
			get{return _canSave;}
			set{_canSave = value;}
		}

		/// <summary>
		/// Get/Set Language Id
		/// </summary>
		[Bindable(true),
		Description("Language Id")]
		public int LanguageId{
			get{return _languageId;}
			set{_languageId = value;}
		}
		#endregion

		#region Render (Affichage)
		/// <summary>
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {

			#region Formatage des longueur et largeur du fichier
			string[] dimensionValue=null;
			string width="";
			string height="";
			if(_dimension.Length!=0){
				//if(_dimension.LastIndexOf('*')>0){
					dimensionValue = _dimension.Split('*');
					width=dimensionValue[0];
					height=dimensionValue[1];
				//}
				//else throw(new DisplayBannerWebControlException("Impossible de déterminer les longueur et largeur du fichier"));
			}
			else throw(new DisplayBannerWebControlException("Le fichier ne possède pas de dimension"));
			#endregion

			#region Chemin complet du fichier
			string pathFile=VIRTUAL_DIRECTORY+@"\"+_filePath;
			#endregion

			#region HTML
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\" bgcolor=\"#D0C8DA\" width=\"100%\" align=\"center\">");
			output.Write("\n<tr>");
			output.Write("\n<td align=\"center\">");
			if(_formatId==(int)FormatType.swf){
				// Bannière de type Flash
				output.Write("\n <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" width=\""+width+"\" height=\""+height+"\">");
				output.Write("\n <PARAM name=\"movie\" value=\""+ pathFile +"\">");
				output.Write("\n <PARAM name=\"play\" value=\"true\">");
				output.Write("\n <PARAM name=\"quality\" value=\"high\">");
				output.Write("\n <EMBED src=\""+ pathFile +"\" play=\"true\" swliveconnect=\"true\" quality=\"high\" width=\""+width+"\" height=\""+height+"\">");
				output.Write("\n </OBJECT>");
			}
			else{
				// Bannière de type autre
				output.Write("\n <p>");
				if(_activeLink){
					output.Write("<a href=\""+ _linkBanner +"\" target=\"_blank\"><img border=0 src=\""+ pathFile +"\" border=\"0\"></a>");
				}
				else{
					output.Write("<img border=0 src=\""+ pathFile +"\" border=\"0\">");
				}
				output.Write("</p>");
			}
			output.Write("\n</td></tr>");
			if(_canSave){
				string cssSaveLink="";
				if(_cssSaveLink.Length>0)cssSaveLink=" class=\""+_cssSaveLink+"\" ";
				output.Write("\n<tr><td align=\"center\"><a href="+pathFile+" "+cssSaveLink+" title=\""+GestionWeb.GetWebWord(SAVE_LINK_LABEL_HELP_ID,_languageId)+"\">"+GestionWeb.GetWebWord(SAVE_LINK_LABEL_ID,_languageId)+"</a></td></tr>");
			}
			output.Write("\n</table>");
			#endregion

		}
		#endregion
	}

}
