using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using WebFct = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Controls.Headers
{
    /// <summary>
    /// Period Detail Selection WebControl (designed for Media Plan Schedules)
    /// </summary>
    [ToolboxData("<{0}:PeriodDetailWebControl runat=server></{0}:PeriodDetailWebControl>")]
    public class PeriodDetailWebControl : WebControl
    {

        #region Variables

        #region Label CssClass
        /// <summary>
        /// Label Css Class
        /// </summary>
        private string _backgroundColor = "#ffffff";
        /// <summary>
        /// Get / Set BackGround Color
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("#ffffff")]
        public string BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value;
            }
        }
        #endregion

        #region Label Code
        /// <summary>
        /// Label Code
        /// </summary>
        private int _textCode = 2288;
        /// <summary>
        /// Get / Set Label Code
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(2288)]
        public int TextCode
        {
            get
            {
                return _textCode;
            }

            set
            {
                _textCode = value;
            }
        }
        #endregion

        #region Label Language Code
        /// <summary>
        /// Language Code
        /// </summary>
        private int _languageCode = 33;
        /// <summary>
        /// Get / Set Language Code
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(33)]
        public int LanguageCode
        {
            get
            {
                return _languageCode;
            }

            set
            {
                _languageCode = value;
            }
        }
        #endregion

        #region Label CssClass
        /// <summary>
        /// Label Css Class
        /// </summary>
        private string _labelCssCass = "txtGris11Bold";
        /// <summary>
        /// Get / Set Label Css Class
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("txtGris11Bold")]
        public string LabelCssClass
        {
            get
            {
                return _labelCssCass;
            }

            set
            {
                _labelCssCass = value;
            }
        }
        #endregion

        #region ListCssClass
        /// <summary>
        /// Get / Set List Css Class
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("txtNoir11Bold")]
        public string ListCssClass
        {
            get
            {
                return _dropDownList.CssClass;
            }

            set
            {
                _dropDownList.CssClass = value;
            }
        }
        #endregion

        #region List
        /// <summary>
        /// Period details list
        /// </summary>
        private DropDownList _dropDownList;
        #endregion

        #region Session
        /// <summary>
        /// CLient Session
        /// </summary>
        private WebSession _session;
        /// <summary>
        /// Get / Set Client Session
        /// </summary>
        public WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        #endregion

        #endregion

        #region Accessors
        /// <summary>
        /// Add Item
        /// </summary>
        /// <param name="captionCode">Text code of item caption</param>
        /// <param name="periodDetail">Value of Item Caption</param>
        public void AddItem(int captionCode, ConstantesPeriod.DisplayLevel periodDetail)
        {
            _dropDownList.Items.Add(new ListItem(GestionWeb.GetWebWord(captionCode, _languageCode), periodDetail.GetHashCode().ToString()));
        }
        /// <summary>
        /// Get Selected Item
        /// </summary>
        /// <returns></returns>
        public ConstantesPeriod.DisplayLevel SelectedItem()
        {
            return (ConstantesPeriod.DisplayLevel)int.Parse(_dropDownList.SelectedItem.Value);
        }
        /// <summary>
        /// Select Specific period detail
        /// </summary>
        /// <param name="periodDetail">Period Detail</param>
        public void Select(ConstantesPeriod.DisplayLevel periodDetail)
        {
            for (int i = 0; i < _dropDownList.Items.Count; i ++){
                if (periodDetail.GetHashCode() == int.Parse(_dropDownList.Items[i].Value))
                {
                    _dropDownList.SelectedItem.Selected = false;
                    _dropDownList.Items[i].Selected = true;
                }
            }
            
        }
        /// <summary>
        /// Get Selected Value
        /// </summary>
        public ConstantesPeriod.DisplayLevel SelectedValue
        {
            get
            {
                string value = string.Empty;
                if (Page.Request.Form.GetValues(this._dropDownList.ClientID) != null)
                {
                    value = Page.Request.Form.GetValues(this._dropDownList.ClientID)[0];
                }
                if (value != null && value != string.Empty)
                {
                    return (ConstantesPeriod.DisplayLevel)Convert.ToInt32(value);
                }
                return Session.DetailPeriod;

            }
        }
        #endregion

        #region Builder
        /// <summary>
        /// Default Constructor
        /// </summary>
        public PeriodDetailWebControl()
        {
            _dropDownList = new DropDownList();
            this.Controls.Add(_dropDownList);
        }
        #endregion

        #region OnInit(EventArgs e)
        /// <summary>
        /// OnInit event Handler
        /// </summary>
        /// <param name="e">Event params</param>
        protected override void OnInit(EventArgs e)
        {
            this.AddItem(2290, ConstantesPeriod.DisplayLevel.monthly);
            this.AddItem(848, ConstantesPeriod.DisplayLevel.weekly);
            DateTime begin = WebFct.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            if (begin >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
            {
                this.AddItem(2289, ConstantesPeriod.DisplayLevel.dayly);
            }
            base.OnInit(e);
        }
        #endregion

        #region RenderContents(HtmlTextWriter output)
        /// <summary>
        /// Render Control
        /// </summary>
        /// <param name="output">Output</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            output.WriteLine("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" bgcolor=\"{0}\">", this._backgroundColor);
            output.WriteLine("<tr><td class=\"{0}\">", this.LabelCssClass);
            output.Write(GestionWeb.GetWebWord(_textCode, _languageCode));
            output.Write("</td></tr>");
            output.WriteLine("<tr><td>");
            _dropDownList.RenderControl(output);
            output.Write("</td></tr>");
            output.WriteLine("</table>");
        }
        #endregion
    }
}
