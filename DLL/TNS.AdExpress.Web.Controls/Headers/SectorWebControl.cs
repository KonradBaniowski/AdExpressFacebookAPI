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
using TNS.AdExpress.Web.Controls.Exceptions;

namespace TNS.AdExpress.Web.Controls.Headers {
    /// <summary>
    /// Sector list WebControl
    /// </summary>
    [ToolboxData("<{0}:SectorWebControl runat=server></{0}:SectorWebControl>")]
    public class SectorWebControl : WebControl {

        #region Constantes
        /// <summary>
        /// All Sector
        /// </summary>
        private const Int64 ALL_SECTOR = -1;
        #endregion

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
            _dropDownList = new DropDownList();
        }
        #endregion

        #region OnInit(EventArgs e)
        /// <summary>
        /// OnInit event Handler
        /// </summary>
        /// <param name="e">Event params</param>
        protected override void OnInit(EventArgs e) {

            _dt = new TNS.AdExpress.DataAccess.Classification.ProductBranch.AllSectorLevelListDataAccess(_session.DataLanguage, _session.Source).GetDataTable;

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

                DataTable dtLine = new DataTable();
                dtLine.Columns.Add("id_sector");
                dtLine.Columns.Add("sector");

                DataRow row = null;
                row = dtLine.NewRow();
                row["id_sector"] = ALL_SECTOR;
                row["sector"] = "-------------------";
                dtLine.Rows.Add(row);
                dtLine.Merge(_dt);

                _dropDownList.DataSource = dtLine;
                _dropDownList.DataTextField = "sector";
                _dropDownList.DataValueField = "id_sector";
                _dropDownList.DataBind();

                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                {
                    NomenclatureElementsGroup nomenclatureElementsGroup = _session.PrincipalProductUniverses[0].GetGroup(0);
                    if (nomenclatureElementsGroup != null)
                    {
                        string idSec = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.SECTOR);
                        try
                        {
                            _dropDownList.Items.FindByValue(idSec).Selected = true;
                        }
                        catch (Exception)
                        {
                            _dropDownList.Items.FindByValue("-1").Selected = true;	
                        }
                    }
                }
                else _dropDownList.Items.FindByValue("-1").Selected = true;	
            }
            if(Page.IsPostBack) {
                Int64 idSector = -1;
                try {
                    // Save SectorID in websession
                    if (_dropDownList.SelectedValue != null && _dropDownList.SelectedValue.Length > 0
                        && Int64.TryParse(_dropDownList.SelectedValue, out idSector)
                        && idSector != ALL_SECTOR) {


                        Dictionary<int, AdExpressUniverse> universeDictionary = new Dictionary<int, AdExpressUniverse>();
                        _session.PrincipalProductUniverses.Clear();

                        AdExpressUniverse adExpressUniverse = new AdExpressUniverse(Dimension.product);
                        NomenclatureElementsGroup nGroup = new NomenclatureElementsGroup(0, AccessType.includes);
                        nGroup.AddItem(TNSClassificationLevels.SECTOR, idSector);
                        adExpressUniverse.AddGroup(0, nGroup);
                        universeDictionary.Add(0, adExpressUniverse);
                        _session.PrincipalProductUniverses = universeDictionary;

                    }
                    else {
                        _session.PrincipalProductUniverses = new Dictionary<int, AdExpressUniverse>();
                    }
                    _session.Save();
                }
                catch (Exception er) {
                    throw new SectorWebControlException("Impossible to get sector selected", er);
                }
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
