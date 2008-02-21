#region Informations
// Auteur: B.Masson 
// Date de cr�ation: 27/03/2007
// Date de modification: 
#endregion

using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.DataAccess.Session;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Description r�sum�e de GenericLevelDetailInPopUpWebControl.
	/// </summary>
	[ToolboxData("<{0}:OptionLayerWebControl runat=server></{0}:OptionLayerWebControl>")]
	public class OptionLayerWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
        /// <summary>
        /// Bool�en pour l'affichage de l'option "D�tail de la p�riode"
        /// </summary>
        protected bool _displayPeriodDetailOption = false;
        /// <summary>
        /// Bool�en pour l'affichage du bouton valider
        /// </summary>
        protected bool _displayValidateButton = true;
		/// <summary>
		/// Customer Session
		/// </summary>
		protected WebSession _customerWebSession;
		/// <summary>
		/// BackgoundColor
		/// </summary>
		protected string _backgroundColor="#ffffff";
		/// <summary>
		/// Bool�en pour l'affichage du composant sur l'info du bouton droit
		/// </summary>
		protected bool _displayInformationComponent=false;
		/// <summary>
		/// Bouton
		/// </summary>
		private Buttons.ImageButtonRollOverWebControl _imageButtonRollOverWebControl=null;

		/// <summary>
		/// Composant pour l'affichage des info du bouton droit
		/// </summary>
		private InformationWebControl _informationWebControl=null;
		/// <summary>
		/// Composant des niveaux de d�tail g�n�rique pour AdNettrack
		/// </summary>
		private GenericAdNetTrackLevelDetailSelectionWebControl _genericAdNetTrackLevelDetailSelectionWebControl=null;
		/// <summary>
		/// Composant des niveaux de d�tail g�n�rique
		/// </summary>
		private GenericMediaLevelDetailSelectionWebControl _genericMediaLevelDetailSelectionWebControl=null;

		/// <summary>
		/// Nombre de niveaux de d�taille personnalis�s
		/// </summary>
		protected int _nbDetailLevelItemList=4;
		/// <summary>
		/// Classe Css pour le libell� de la liste des niveaux par d�faut
		/// </summary>
		protected string _cssDefaultListLabel="txtGris11Bold";
		/// <summary>
		/// Classe Css pour le libell� des listes personnalis�es
		/// </summary>
		protected string _cssListLabel="txtViolet11Bold";
		/// <summary>
		/// Classe Css pour les listbox
		/// </summary>
		protected string _cssListBox="txtNoir11Bold";
		/// <summary>
		/// Classe Css pour le titre de la section personnalis�e
		/// </summary>
		protected string _cssCustomSectionTitle="txtViolet11Bold";
		/// <summary>
		/// Chemin de la page de sauvegarde
		/// </summary>
		protected string _saveASPXFilePath="test.aspx";
		/// <summary>
		/// Chemin de la page de sauvegarde
		/// </summary>
		protected string _removeASPXFilePath="test.aspx";
		///<summary>
		/// Type des niveaux de d�tail
		/// </summary>
		protected WebConstantes.GenericDetailLevel.Type _genericDetailLevelType=WebConstantes.GenericDetailLevel.Type.mediaSchedule;
		///<summary>
		/// Profile du composant
		/// </summary>
		protected WebConstantes.GenericDetailLevel.ComponentProfile _componentProfile=WebConstantes.GenericDetailLevel.ComponentProfile.media;
		/// <summary>
		/// Force le composant � s'initialiser avec les valeurs du module
		/// </summary>
		protected Int64 _forceModuleId=-1;
        /// <summary>
        /// Option Detail de la p�riode
        /// </summary>
        PeriodDetailWebControl _periodDetail = null;
        #endregion

		#region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
		public OptionLayerWebControl(){
            _periodDetail = new PeriodDetailWebControl();
            this.Controls.Add(_periodDetail);
            _periodDetail.LabelCssClass = "txtViolet11Bold";
		}
		#endregion
	
		#region Accesseurs
        /// <summary>
        /// Force le composant � s'initialiser avec les valeurs du module
        /// </summary>
        public Int64 ForceModuleId
        {
            set { _forceModuleId = value; }
        }
		/// <summary>
		/// D�finit la session du client
		/// </summary>
		[Bindable(false)]
		public WebSession CustomerWebSession{
			set{
                _customerWebSession = value;
                _periodDetail.Session = value;
				_periodDetail.LanguageCode = _customerWebSession.SiteLanguage;
            }
		}
		/// <summary>
		/// Obtient ou d�finit la couleur de fond du composant
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("#ffffff"),
		Description("Couleur de fond du composant")]
		public string BackGroundColor{
			get{return(_backgroundColor);}
			set{_backgroundColor=value;}
		}
		/// <summary>
		/// Obtient ou d�finit la couleur de fond du composant
		/// </summary>
		[Bindable(true), 
		Category("Component"), 
		Description("L'information du bouton droit doit-elle apparaitre")]
		public bool DisplayInformationComponent{
			get{return(_displayInformationComponent);}
			set{_displayInformationComponent=value;}
		}
        /// <summary>
        /// Bool�en pour l'affichage du bouton valider
        /// </summary>
        [Bindable(true), 
		Category("Component"), 
		Description("Le bouton valider doit-il apparaitre"),
        DefaultValueAttribute("True")]
        public bool DisplayValidateButton
        {
            get { return (_displayValidateButton); }
            set { _displayValidateButton = value; }
		}
        /// <summary>
        /// Bool�en pour l'affichage de l'option "Detail de la p�riode"
        /// </summary>
        [Bindable(true), 
		Category("Component"), 
		Description("L'option d�tail de la p�riode doit-elle apparaitre"),
        DefaultValueAttribute("False")]
        public bool DisplayPeriodDetailOption
        {
            get { return (_displayPeriodDetailOption); }
            set { _displayPeriodDetailOption = value; }
		}
        /// <summary>
        /// Control option "Detail de la p�riode"
        /// </summary>
        [Bindable(false), 
		Description("Control d�tail de la p�riode")]
        public PeriodDetailWebControl PeriodDetailControl
        {
            get { return (_periodDetail); }
		}
		/// <summary>
		/// Obtient ou d�finit le nombre de niveaux de d�taille personnalis�s
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("4")]
		public int NbDetailLevelItemList{
			get{return(_nbDetailLevelItemList);}
			set{
				if(value<1 || value>4)throw(new ArgumentOutOfRangeException("The value of NbDetailLevelItemList must be between 1 and 4"));
				_nbDetailLevelItemList=value;
			}
		}
		/// <summary>
		/// Obtient ou d�finit le type des niveaux de d�tail
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		Description("Type des niveaux de d�tail")]
		public WebConstantes.GenericDetailLevel.Type GenericDetailLevelType{
			get{return(_genericDetailLevelType);}
			set{_genericDetailLevelType=value;}
		}
		/// <summary>
		/// Obtient ou d�finit Le profile du coposant
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		Description("Profile du composant"),
		DefaultValue("media")
		]
		public WebConstantes.GenericDetailLevel.ComponentProfile GenericDetailLevelComponentProfile{
			get{return(_componentProfile);}
			set{_componentProfile=value;}
		}

		#region Css
		/// <summary>
		/// Obtient ou d�finit la classe Css pour le libell� de la liste des niveaux par d�faut
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("txtGris11Bold"),
		Description("Classe Css pour le libell� de la liste des niveaux par d�faut")]
		public string CssDefaultListLabel{
			get{return(_cssDefaultListLabel);}
			set{_cssDefaultListLabel=value;}
		}
		/// <summary>
		/// Obtient ou d�finit la classe Css pour le libell� des listes personnalis�es
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("txtViolet11Bold"),
		Description("Classe Css pour le libell� des listes personnalis�es")]
		public string CssListLabel{
			get{return(_cssListLabel);}
			set{_cssListLabel=value;}
		}
		/// <summary>
		/// Obtient ou d�finit la classe Css pour les listbox
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("txtNoir11Bold"),
		Description("Classe Css pour les listbox")]
		public string CssListBox{
			get{return(_cssListBox);}
			set{_cssListBox=value;}
		}
		/// <summary>
		/// Obtient ou d�finit la classe Css pour le titre de la section personnalis�e
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("txtViolet11Bold"),
		Description("Classe Css pour le titre de la section personnalis�e")]
		public string CssCustomSectionTitle{
			get{return(_cssCustomSectionTitle);}
			set{_cssCustomSectionTitle=value;}
		}
		#endregion

		#region removeASPXFilePath
		/// <summary>
		/// Obtient ou d�finit la Page permettant de sauvegarer le niveaux de d�tail
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("test.aspx"),
		Description("Page permettant de supprimer un niveaux de d�tail sauvegard�")]
		public string RemoveASPXFilePath{
			get{return(_removeASPXFilePath);}
			set{_removeASPXFilePath=value;}
		}
		#endregion

		#region saveASPXFilePath
		/// <summary>
		/// Obtient ou d�finit la Page permettant de sauvegarer le niveaux de d�tail
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("test.aspx"),
		Description("Page permettant de sauvegarer le niveaux de d�tail")]
		public string SaveASPXFilePath{
			get{return(_saveASPXFilePath);}
			set{_saveASPXFilePath=value;}
		}
		#endregion

		#endregion

		#region Ev�nements

        #region Init
        /// <summary>
		/// Init
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			base.OnInit (e);
			// Composant info bouton droit
			if(DisplayInformationComponent){
				_informationWebControl=new InformationWebControl();
				_informationWebControl.Language=_customerWebSession.SiteLanguage;
				_informationWebControl.InLeftMenu=true;
				_informationWebControl.BackGroundColor=_backgroundColor;
				this.Controls.Add(_informationWebControl);
			}
			// Bouton
			_imageButtonRollOverWebControl=new TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl();
			_imageButtonRollOverWebControl.ID = "ImageButtonRollOverWebControl_"+this.ID;
			_imageButtonRollOverWebControl.ImageUrl = "/Images/Common/Button/ok_up.gif";
			_imageButtonRollOverWebControl.RollOverImageUrl = "/Images/Common/Button/ok_down.gif";
			this.Controls.Add(_imageButtonRollOverWebControl);

			switch((WebConstantes.GenericDetailLevel.ComponentProfile)_componentProfile){
				case WebConstantes.GenericDetailLevel.ComponentProfile.adnettrack:
					// Composant niveaux de d�tail g�n�rique pour AdNettrack
					_genericAdNetTrackLevelDetailSelectionWebControl = new GenericAdNetTrackLevelDetailSelectionWebControl();
					_genericAdNetTrackLevelDetailSelectionWebControl.CustomerWebSession = _customerWebSession;
					_genericAdNetTrackLevelDetailSelectionWebControl.CssCustomSectionTitle = _cssCustomSectionTitle;
					_genericAdNetTrackLevelDetailSelectionWebControl.CssDefaultListLabel = _cssDefaultListLabel;
					_genericAdNetTrackLevelDetailSelectionWebControl.CssListBox = _cssListBox;
					_genericAdNetTrackLevelDetailSelectionWebControl.CssListLabel = _cssListLabel;
					_genericAdNetTrackLevelDetailSelectionWebControl.GenericDetailLevelComponentProfile = _componentProfile;
					_genericAdNetTrackLevelDetailSelectionWebControl.GenericDetailLevelType = _genericDetailLevelType;
					_genericAdNetTrackLevelDetailSelectionWebControl.NbDetailLevelItemList = _nbDetailLevelItemList;
					_genericAdNetTrackLevelDetailSelectionWebControl.RemoveASPXFilePath = _removeASPXFilePath;
					_genericAdNetTrackLevelDetailSelectionWebControl.SaveASPXFilePath = _saveASPXFilePath;
					_genericAdNetTrackLevelDetailSelectionWebControl.Width=this.Width;
					_genericAdNetTrackLevelDetailSelectionWebControl.BackGroundColor = _backgroundColor;
					this.Controls.Add(_genericAdNetTrackLevelDetailSelectionWebControl);
					break;
				case WebConstantes.GenericDetailLevel.ComponentProfile.media:
				case WebConstantes.GenericDetailLevel.ComponentProfile.product:
					// Composant niveaux de d�tail g�n�rique
					_genericMediaLevelDetailSelectionWebControl = new GenericMediaLevelDetailSelectionWebControl();
                    _genericMediaLevelDetailSelectionWebControl.ForceModuleId = _forceModuleId;
					_genericMediaLevelDetailSelectionWebControl.CustomerWebSession = _customerWebSession;
					_genericMediaLevelDetailSelectionWebControl.CssCustomSectionTitle = _cssCustomSectionTitle;
					_genericMediaLevelDetailSelectionWebControl.CssDefaultListLabel = _cssDefaultListLabel;
					_genericMediaLevelDetailSelectionWebControl.CssListBox = _cssListBox;
					_genericMediaLevelDetailSelectionWebControl.CssListLabel = _cssListLabel;
					_genericMediaLevelDetailSelectionWebControl.GenericDetailLevelComponentProfile = _componentProfile;
					_genericMediaLevelDetailSelectionWebControl.GenericDetailLevelType = _genericDetailLevelType;
					_genericMediaLevelDetailSelectionWebControl.NbDetailLevelItemList = _nbDetailLevelItemList;
					_genericMediaLevelDetailSelectionWebControl.RemoveASPXFilePath = _removeASPXFilePath;
					_genericMediaLevelDetailSelectionWebControl.SaveASPXFilePath = _saveASPXFilePath;
					_genericMediaLevelDetailSelectionWebControl.Width=this.Width;
					_genericMediaLevelDetailSelectionWebControl.BackGroundColor = _backgroundColor;
					this.Controls.Add(_genericMediaLevelDetailSelectionWebControl);
					break;
			}
		}
		#endregion

		#region Pr�render
		/// <summary>
		/// Pr�paration du rendu des niveaux de d�tails personnalis�s.
		/// </summary>
		/// <param name="e">Sender</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
			if(!this.Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayerOption"))
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayerOption",TNS.AdExpress.Web.Functions.Script.DivDisplayerOption(_customerWebSession.SiteLanguage.ToString()));
		}
		#endregion

		#region Render
		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output){
			output.Write("<table border=\"0\" cellSpacing=\"0\" cellPadding=\"0\" height=\"100%\">");
			output.Write("<tr>");
			output.Write("<td vAlign=\"top\" bgColor=\""+_backgroundColor+"\" height=\"100%\">");

			// D�but Div Options d'analyse
			output.Write("<div id=\"option\" style=\"DISPLAY: none\">");
			output.Write("<table cellSpacing=\"0\" cellPadding=\"0\" bgColor=\""+_backgroundColor+"\" border=\"0\">");
			output.Write("<tr>");
			output.Write("<td width=\"3\" bgColor=\"#644883\" height=\"1\"></td>");
			output.Write("<td bgColor=\"#644883\" height=\"1\"></td>");
			output.Write("</tr>");
			
			// Option D�tail p�riode
            if (_displayPeriodDetailOption)
            {
                _periodDetail.BackgroundColor = _backgroundColor;
                output.Write("<tr>");
                output.Write("<td width=\"3\" bgColor=\"" + _backgroundColor + "\">&nbsp;</td>");
                output.Write("<td>");
                _periodDetail.RenderControl(output);
                output.Write("</td>");
                output.Write("</tr>");

                output.Write("<tr>");
                output.Write("<td width=\"3\" bgColor=\"" + _backgroundColor + "\"></td>");
                output.Write("<td height=\"10\">&nbsp;</td>");
                output.Write("</tr>");
            }
            
            // Composant D�tails G�n�riques
			output.Write("<tr>");
            output.Write("<td width=\"3\" bgColor=\"" + _backgroundColor + "\">&nbsp;</td>");
			output.Write("<td>");
			switch((WebConstantes.GenericDetailLevel.ComponentProfile)_componentProfile){
				case WebConstantes.GenericDetailLevel.ComponentProfile.adnettrack:
					_genericAdNetTrackLevelDetailSelectionWebControl.RenderControl(output);
					break;
				case WebConstantes.GenericDetailLevel.ComponentProfile.media:
				case WebConstantes.GenericDetailLevel.ComponentProfile.product:
					_genericMediaLevelDetailSelectionWebControl.RenderControl(output);
					break;
			}
			output.Write("</td>");
			output.Write("</tr>");
			output.Write("<tr>");
            output.Write("<td width=\"3\" bgColor=\"" + _backgroundColor + "\">&nbsp;</td>");
			output.Write("<td height=\"3\"></td>");
			output.Write("</tr>");


			
			// Bouton OK
            if (_displayValidateButton)
            {
                output.Write("<tr>");
                output.Write("<td width=\"3\" bgColor=\"" + _backgroundColor + "\">&nbsp;</td>");
                output.Write("<td>");
                _imageButtonRollOverWebControl.RenderControl(output);
                output.Write("</td>");
                output.Write("</tr>");

                output.Write("<tr>");
                output.Write("<td width=\"3\" bgColor=\"" + _backgroundColor + "\">&nbsp;</td>");
                output.Write("<td height=\"10\">&nbsp;</td>");
                output.Write("</tr>");
            }

			// Affichage composant Info Bouton Droit
			if(_displayInformationComponent){
				output.Write("<tr>");
                output.Write("<td width=\"3\" bgColor=\"" + _backgroundColor + "\">&nbsp;</td>");
				output.Write("<td>");
				_informationWebControl.RenderControl(output);
				output.Write("</td>");
				output.Write("</tr>");
			}
			output.Write("</table>");
			output.Write("</div>");
			// Fin Div Options d'analyse

			output.Write("</td>");
			output.Write("<td valign=\"top\" height=\"100%\">");

			// D�but Onglet
			output.Write("<table height=\"100%\" cellSpacing=\"0\" cellPadding=\"0\" bgColor=\"#ffffff\" border=\"0\">");
			output.Write("<tr>");
			output.Write("<td bgColor=\"#644883\" height=\"1\"></td>");
			output.Write("</tr>");
			output.Write("<tr>");
			output.Write("<td onclick=\"javascript:DivDisplayerOption('option');\" style=\"cursor:hand;cursor:pointer;\"><IMG src=\"/Images/"+_customerWebSession.SiteLanguage+"/Others/OpenOptions.gif\" border=\"0\" id=\"ImgOpenOption\"></td>");
			output.Write("</tr>");
			output.Write("<tr style=\"cursor:hand;cursor:pointer;\">");
			output.Write("<td onclick=\"javascript:DivDisplayerOption('option');\"><IMG src=\"/Images/"+_customerWebSession.SiteLanguage+"/Others/TextOptions.gif\" border=\"0\"></td>");
			output.Write("</tr>");
			output.Write("<tr>");
			output.Write("<td style=\"background-image:url(/Images/"+_customerWebSession.SiteLanguage+"/Others/BackgroundOptions.gif);\" height=\"100%\"></td>");
			output.Write("</tr>");
			output.Write("</table>");
			// Fin Onglet

			output.Write("</td>");
			output.Write("</tr>");
			output.Write("</table>");
		}
		#endregion

		#endregion



	}
}
