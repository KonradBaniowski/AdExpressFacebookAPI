using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.P3.Domain.Translation;

namespace KMI.P3.Web.Controls.Translation
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:TextWebControl runat=server></{0}:TextWebControl>")]
    public class TextWebControl : WebControl
    {
        #region Variables
        /// <summary>
        /// Code du texte à afficher
        /// </summary>
        private int _code = 0;
        /// <summary>
        /// Langue du texte à afficher
        /// </summary>
        private int _language = 33;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public TextWebControl()
            : base()
        {
        }
        #endregion

        #region Accesseurs
        /// <summary>
        /// Obtient et définit du Code du texte à extraire
        /// </summary>
        [Category("Comportement"),
        Description("Indique le code du texte à afficher."),
        DefaultValue(1)]
        public virtual int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// Obtient et définit du Code du texte à extraire la Langue du texte à extraire
        /// </summary>
        [Category("Comportement"),
        Description("Langue du texte à afficher."),
        DefaultValue(33)]
        public virtual int Language
        {
            get { return _language; }
            set { _language = value; }
        }
        #endregion

        #region Evènements
        /// <summary>
        /// Rendu du texte à extraire
        /// </summary>
        /// <param name="writer">Flux HTML</param>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(GestionWeb.GetWebWord(_code, _language));
        }
        #endregion

    }
}
