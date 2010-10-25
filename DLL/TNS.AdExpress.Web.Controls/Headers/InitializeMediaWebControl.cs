#region Informations
// Auteur : D. Mussuma
// Date de création : 16/12/2004
#endregion

using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Windows.Forms;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Headers {

    /// <summary>
    /// Contrôle pour l'initialisation des médias
    /// </summary>
    [ToolboxData("<{0}:InitializeProductWebControl runat=server></{0}:InitializeProductWebControl>")]
    public class InitializeMediaWebControl : System.Web.UI.WebControls.WebControl {

        #region Variables
        /// <summary>
        /// CheckBox pour l'initialisation des medias
        /// </summary>
        public System.Web.UI.WebControls.CheckBox initializeMediaCheckBox = new System.Web.UI.WebControls.CheckBox();
        #endregion

        #region Propriétés
        /// <summary>
        /// Option affiner univers supports
        /// </summary>
        [Bindable(true),
        Description("Option :Option affiner univers supports")]
        protected bool _initializeMedia = false;
        /// <summary></summary>
        public bool InitializeMedia {
            get { return _initializeMedia; }
            set { _initializeMedia = value; }
        }

        /// <summary>
        /// Session du client 
        /// </summary>
        protected WebSession customerWebSession = null;
        /// <summary> Session du client </summary>
        public WebSession CustomerWebSession {
            get { return customerWebSession; }
            set { customerWebSession = value; }
        }

        /// <summary>
        /// CssClass générale 
        /// </summary>
        [Bindable(true), DefaultValue("txtNoir11Bold"),
        Description("Option choix de l'unité")]
        protected string cssClass = "txtNoir11Bold";
        /// <summary></summary>
        public string CommonCssClass {
            get { return cssClass; }
            set { cssClass = value; }
        }

        /// <summary>
        /// Autopostback Vrai par défaut
        /// </summary>
        [Bindable(true),
        Description("autoPostBack")]
        protected bool autoPostBackOption = true;
        /// <summary></summary>
        public bool AutoPostBackOption {
            get { return autoPostBackOption; }
            set { autoPostBackOption = value; }
        }
        #endregion

        #region Constructeur
        /// <summary>
        /// Contructeur
        /// </summary>
        public InitializeMediaWebControl()
            : base() {
            this.EnableViewState = true;
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement
        /// </summary>
        /// <param name="e">arguments</param>
        protected override void OnLoad(EventArgs e) {
            if(_initializeMedia) {
                initializeMediaCheckBox = new System.Web.UI.WebControls.CheckBox();
                initializeMediaCheckBox.ID = this.ID + "_initializeMedia";
                initializeMediaCheckBox.EnableViewState = true;
                initializeMediaCheckBox.ToolTip = GestionWeb.GetWebWord(2069, customerWebSession.SiteLanguage);
                initializeMediaCheckBox.CssClass = cssClass;
                initializeMediaCheckBox.AutoPostBack = autoPostBackOption;
                initializeMediaCheckBox.Width = new System.Web.UI.WebControls.Unit("100%");
                initializeMediaCheckBox.Text = GestionWeb.GetWebWord(2069, customerWebSession.SiteLanguage);

                Controls.Add(initializeMediaCheckBox);

                if(Page.IsPostBack && Page.Request.Form.GetValues(this.ID + "_initializeMedia") != null
                    && Page.Request.Form.GetValues(this.ID + "_initializeMedia")[0] != null)
                    initializeMediaCheckBox.Checked = true;
                else initializeMediaCheckBox.Checked = false;

                if(initializeMediaCheckBox.Checked) {
                    if(customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA
                        || customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                    {
                        customerWebSession.SecondaryMediaUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                        customerWebSession.Save();
                    }
                    else {
                        customerWebSession.CompetitorUniversMedia = new Hashtable(5);
                        customerWebSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("Media");
                        customerWebSession.Save();
                    }
                }
                if(customerWebSession.CurrentUniversMedia.Nodes.Count > 0
                    || customerWebSession.SecondaryMediaUniverses.Count > 0) {
                    initializeMediaCheckBox.Enabled = true;
                }
                else {
                    initializeMediaCheckBox.Enabled = false;
                }

            }
            base.OnLoad(e);
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {

            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");

            //options de affiner supports
            if(InitializeMedia) {
                output.Write("\n<tr>");
                output.Write("\n<td>");
                initializeMediaCheckBox.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }

            output.Write("\n</table>");
        }
        #endregion

    }
}
