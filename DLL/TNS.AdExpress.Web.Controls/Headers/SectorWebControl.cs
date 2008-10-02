using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using WebFct = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Date;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Controls.Headers {
    /// <summary>
    /// Sector list WebControl
    /// </summary>
    [ToolboxData("<{0}:SectorWebControl runat=server></{0}:SectorWebControl>")]
    public class SectorWebControl : WebControl {

        #region Variables
        /// <summary>
        /// Period details list
        /// </summary>
        private DropDownList _dropDownList;
        /// <summary>
        /// Label Css Class
        /// </summary>
        private string _backgroundColor = "#ffffff";
        /// <summary>
        /// Label Code
        /// </summary>
        private int _textCode = 1847;
        /// <summary>
        /// Language Code
        /// </summary>
        private int _languageCode = 33;
        /// <summary>
        /// Label Css Class
        /// </summary>
        private string _labelCssCass = "txtGris11Bold";
        /// <summary>
        /// CLient Session
        /// </summary>
        private WebSession _session;
        /// <summary>
        /// DataTable
        /// </summary>
        private DataTable _dt = null;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set BackGround Color
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("#ffffff")]
        public string BackgroundColor {
            get {return _backgroundColor;}
            set {_backgroundColor = value;}
        }       
        /// <summary>
        /// Get / Set Label Code
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(2288)]
        public int TextCode {
            get {return _textCode;}
            set {_textCode = value;}
        }
        /// <summary>
        /// Get / Set Language Code
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(33)]
        public int LanguageCode {
            get {return _languageCode;}
            set {_languageCode = value;}
        }
        /// <summary>
        /// Get / Set Label Css Class
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("txtGris11Bold")]
        public string LabelCssClass {
            get {return _labelCssCass;}
            set {_labelCssCass = value;}
        }
        /// <summary>
        /// Get / Set List Css Class
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("txtNoir11Bold")]
        public string ListCssClass {
            get {return _dropDownList.CssClass;}
            set {_dropDownList.CssClass = value;}
        }
        /// <summary>
        /// Get / Set Client Session
        /// </summary>
        public WebSession Session {
            get { return _session; }
            set { _session = value; }
        }
        /// <summary>
        /// Get / Set DataTable
        /// </summary>
        public DataTable DataTable {
            get { return _dt; }
            set { _dt = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SectorWebControl(){
        }
        #endregion

        #region OnInit(EventArgs e)
        /// <summary>
        /// OnInit event Handler
        /// </summary>
        /// <param name="e">Event params</param>
        protected override void OnInit(EventArgs e) {
            _dropDownList = new DropDownList();
            _dropDownList.EnableViewState = true;
            this.Controls.Add(_dropDownList);

            base.OnInit(e);
        }
        #endregion

        #region OnLoad(EventArgs e)
        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e">Event params</param>
        protected override void OnLoad(EventArgs e) {
            if(!Page.IsPostBack) {
                _dropDownList.DataSource = _dt;
                _dropDownList.DataTextField = "sector";
                _dropDownList.DataValueField = "id_sector";
                _dropDownList.DataBind();
            }
            if(Page.IsPostBack) {
                // Save SectorID in websession
                if(_dropDownList.SelectedValue != null && _dropDownList.SelectedValue.Length > 0) {
                    Dictionary<int, AdExpressUniverse>  universeDictionary = new Dictionary<int, AdExpressUniverse>();
                    _session.PrincipalProductUniverses.Clear();
                    AdExpressUniverse adExpressUniverse = new AdExpressUniverse(Dimension.product);
                    NomenclatureElementsGroup nGroup = new NomenclatureElementsGroup(0, AccessType.includes);
                    nGroup.AddItem(TNSClassificationLevels.SECTOR, long.Parse(_dropDownList.SelectedValue));
                    adExpressUniverse.AddGroup(0, nGroup);
                    universeDictionary.Add(0, adExpressUniverse);
                    _session.PrincipalProductUniverses = universeDictionary;
                }
                else {
                    _session.PrincipalProductUniverses = new Dictionary<int, AdExpressUniverse>();
                }
                _session.Save();
            }
        }
        #endregion

        #region RenderContents(HtmlTextWriter output)
        /// <summary>
        /// Render Control
        /// </summary>
        /// <param name="output">Output</param>
        protected override void RenderContents(HtmlTextWriter output) {
            output.WriteLine("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" bgcolor=\"{0}\">", this._backgroundColor);
            output.WriteLine("<tr><td class=\"{0}\">", this.LabelCssClass);
            output.Write(GestionWeb.GetWebWord(_textCode, _languageCode));
            output.Write(" :</td></tr>");
            output.WriteLine("<tr><td>");
            _dropDownList.RenderControl(output);
            output.Write("</td></tr>");
            output.WriteLine("</table>");
        }
        #endregion

    }
}
