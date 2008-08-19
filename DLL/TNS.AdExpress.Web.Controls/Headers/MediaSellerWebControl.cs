#region Informations
// Auteur: A.DADOUCH
// Date de création: 19/05/2005  
//Date de modification :
// D. Mussuma 09/11/2005 Ajout choix affichage des supports par Titre
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Controls.Results;
using TNS.AdExpress.Web.Controls.Buttons;

using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using CustomerCst = TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Description résumée de ModuleRegieWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:MediaSellerWebControl runat=server></{0}:MediaSellerWebControl>")]
	public class MediaSellerWebControl : System.Web.UI.WebControls.WebControl{
	
		#region Variables

		/// <summary>
		/// Choix du niveau de détail media
		/// </summary>
		public RadioButtonList mediaDetail;
		/// <summary>
		/// Session du client (utile pour la langue)
		/// </summary>
		protected WebSession customerWebSession = null;
		
		#endregion		

        #region Variables MMI
        /// <summary>
        /// Valider la sélection
        /// </summary>
        public ImageButtonRollOverWebControl _buttonOk;
        #endregion

        #region Propriétés

        /// <summary>
		/// Websession
		///</summary>
		public WebSession CustomerWebSession{
			get{return customerWebSession;}
			set{customerWebSession=value;}
		}

		/// <summary>
		/// CssClass générale 
		/// </summary>
		[Bindable(true),DefaultValue("txtNoir11Bold"),
		Description("Option choix de régie")]
		protected string cssClass = "txtNoir11Bold";
		/// <summary></summary>
		public string CommonCssClass{
			get{return cssClass;}
			set{cssClass=value;}
		}

		/// <summary>
		/// Option unité
		/// </summary>
		[Bindable(true),
		Description("Option choix de régie")]
		protected bool mediaSellerOption = true;
		/// <summary></summary>
		public bool MediaSellerOption{
			get{return mediaSellerOption;}
			set{mediaSellerOption=value;}
		}

		#region Propriétés de TblChoice
		/// <summary>
		/// hauteur de l'image
		/// </summary>
		[Bindable(true),Category("Appearance")]
		private double imageHeight = 15.0 ;
		/// <summary></summary>
		public double ImageHeight{
			get{return imageHeight;}
			set{imageHeight = value;}
		}

		/// <summary>
		/// largeur de l'image
		/// </summary>
		[Bindable(true),Category("Appearance")]
		private double imageWidth = 15.0 ;
		/// <summary></summary>
		public double ImageWidth{
			get{return imageWidth;}
			set{imageWidth = value;}
		}

		/// <summary>
		/// largeur des bordures
		/// </summary>
		[Bindable(true),Category("Appearance"), DefaultValue(1.0)]
		private  double borderWidth=1.0;
		/// <summary></summary>
		public new double BorderWidth{
			get{return borderWidth;}
			set{borderWidth = Math.Max(0, value);}
		}

		/// <summary>
		/// option afficher images
		/// </summary>
		[Bindable(true),DefaultValue(true)]
		protected bool pictShow=true;
		/// <summary></summary>
		public bool ShowPictures{
			get{return pictShow;}
			set{pictShow = value;}
		}
		/// <summary>
		///feuille de style 
		/// </summary>
		[Bindable(true),DefaultValue("ddlOut")]
		protected string outCssClass="ddlOut";
		/// <summary></summary>
		public string OutCssClass{
			get{return outCssClass;}
			set{outCssClass = value;}
		}

		/// <summary>
		///feuille de style pour l'élément en roll-over 
		/// </summary>
		[Bindable(true),DefaultValue("ddlOver")]
		protected string overCssClass="ddlOver";
		/// <summary></summary>
		public string OverCssClass{
			get{return overCssClass;}
			set{overCssClass = value;}
		}

		/// <summary>
		/// liste des éléments
		/// </summary>
		[Bindable(true),DefaultValue("")]
		private string texts = "";
		/// <summary></summary>
		public string List{
			get{return texts;}
			set{texts = value;}
		}

		/// <summary>
		///noms des images 
		/// </summary>
		[Bindable(true),DefaultValue("")]
		private string images = "";
		/// <summary></summary>
		public string Images{
			get{return images;}
			set{images = value;}
		}

		/// <summary>
		/// index liste
		/// </summary>
		[Bindable(true),DefaultValue(0)]
		private int index;
		/// <summary></summary>
		public int ListIndex{
			get{return index;}
			set{index = Math.Min(value, texts.Split('|').Length - 1);}
		}
		#endregion

		/// <summary>
		/// Autopostback Vrai par défaut
		/// </summary>
		[Bindable(true),
		Description("autoPostBack")]
		protected bool autoPostBackOption = false;
		/// <summary></summary>
		public bool AutoPostBackOption{
			get{return autoPostBackOption;}
			set{autoPostBackOption=value;}
		}


		/// <summary>
		/// Option resultat
		/// </summary>
		[Bindable(true),
		Description("Option type de résultat")]
		protected bool resultOption = true;
		/// <summary></summary>
		public bool ResultOption{
			get{return resultOption;}
			set{resultOption=value;}
		}

        /// <summary>
        /// Block_Fleche image path
        /// </summary>
        protected string blockFlechePath = string.Empty;
        /// <summary>
        /// Set or Get block_fleche image path
        /// </summary>
        public string BlockFlechePath {
            get { return blockFlechePath; }
            set { blockFlechePath = value; }
        }

        /// <summary>
        /// Block_dupli image path
        /// </summary>
        protected string blockDupliPath = string.Empty;
        /// <summary>
        /// Set or Get block_dupli image path
        /// </summary>
        public string BlockDupliPath {
            get { return blockDupliPath; }
            set { blockDupliPath = value; }
        }

        /// <summary>
        /// Title style
        /// </summary>
        protected string titleUppercaseCss = string.Empty;
        /// <summary>
        /// Set or Get Title style
        /// </summary>
        public string TitleUppercaseCss {
            get { return titleUppercaseCss; }
            set { titleUppercaseCss = value; }
        }
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MediaSellerWebControl():base(){
			
			this.EnableViewState = true;
			this.PreRender += new EventHandler(Custom_PreRender);
		}

		#endregion

		#region Evenements

		#region Init
        /// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {

            #region bouton OK
            _buttonOk = new ImageButtonRollOverWebControl();
            _buttonOk.ID = "okImageButton";
            _buttonOk.ImageUrl = "/App_Themes/" + WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name + "/Images/Common/Button/ok_up.gif";
            _buttonOk.RollOverImageUrl = "/App_Themes/" + WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name + "/Images/Common/Button/ok_down.gif";
            Controls.Add(_buttonOk);
            #endregion

		}

		#endregion

		#region Custom PreRender

		/// <summary>
		///custom prerender 
		/// </summary>
		/// <param name="sender">object qui lance l'évènement</param>
		/// <param name="e">arguments</param>
        private void Custom_PreRender(object sender, System.EventArgs e)
        {

            #region régie ou centred'intêret ou Titre
            //Option détail Média
            if (mediaSellerOption)
            {
                mediaDetail = new RadioButtonList();
                mediaDetail.Width = new System.Web.UI.WebControls.Unit("100%");

                VehicleInformation vehicleInformation = VehiclesInformation.Get(((LevelInformation)customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID);

                if (vehicleInformation.MediaSelectionParentsItemsInformationList.Count > 1) {
                    foreach (DetailLevelItemInformation currentLevel in vehicleInformation.MediaSelectionParentsItemsInformationList)
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(currentLevel.WebTextId, customerWebSession.SiteLanguage), "MediaSeller_" + currentLevel.Id.GetHashCode().ToString()));

                    mediaDetail.ID = "mediaDetail_" + this.ID;
                    mediaDetail.AutoPostBack = autoPostBackOption;
                    mediaDetail.CssClass = cssClass;

                    try {
                        mediaDetail.Items.FindByValue("MediaSeller_" + customerWebSession.MediaSelectionParent.GetHashCode().ToString()).Selected = true;
                    }
                    catch (System.Exception) {
                        mediaDetail.Items[0].Selected = true;
                    }

                    Controls.Add(mediaDetail);
                }
                else
                    mediaSellerOption = false;

            }
            #endregion
        }
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {

            string themeName = WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name;

            if (mediaSellerOption) {
                output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"whiteBackGround\">");
                output.Write("\n<tr>");
                output.Write("\n<td>");
                //debut tableau titre
                output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
                output.Write("\n<tr>");
                output.Write("\n<td class=\"headerLeft\" colSpan=\"4\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\"></td>");
                output.Write("\n</tr>");
                output.Write("\n<tr>");
                output.Write("\n<td style=\"HEIGHT: 14px\" vAlign=\"top\"><IMG height=\"12\" src=\"" + blockFlechePath + "\" width=\"12\"></td>");
                output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" background=\"" + blockDupliPath + "\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"13\"></td>");
                output.Write("\n<td class=\"txtNoir11Bold " + titleUppercaseCss + "\" width=\"100%\">" + GestionWeb.GetWebWord(1605, customerWebSession.SiteLanguage) + "</td>");
                output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/images/Common/pixel.gif\" width=\"1\"></td>");
                output.Write("\n</tr>");
                output.Write("\n<tr>");
                output.Write("\n<td></td>");
                output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/images/Common/pixel.gif\"></td>");
                output.Write("\n</tr>");
                output.Write("\n</table>");
                output.Write("\n</td>");
                output.Write("\n</tr>");
                //Descriptif
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
                output.Write("\n<tr>");
                output.Write("\n<td class=\"txtGris11Bold\">");
                output.Write(GestionWeb.GetWebWord(1606, customerWebSession.SiteLanguage));
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");

                //détail média
                output.Write("\n<tr>");
                output.Write("\n<td class=\"txtGris11Bold\">");
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr>");
                output.Write("\n<td>");
                mediaDetail.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");

                #region Button
                output.Write("\n<tr class=\"whiteBackGround\">");
			    output.Write("\n<td height=\"10\"></td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
                output.Write("\n<td class=\"whiteBackGround\">");
                _buttonOk.RenderControl(output); 
                output.Write("\n</td>");
                output.Write("\n</TR>");
				output.Write("\n<TR>");
				output.Write("\n<TD class=\"whiteBackGround\" height=\"5\"></TD>");
				output.Write("\n</TR>");
                #endregion

                output.Write("\n</table>");
            }

		}
		#endregion

		#endregion

	}
}