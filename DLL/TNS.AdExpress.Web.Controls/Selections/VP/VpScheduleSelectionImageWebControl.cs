using TNS.AdExpress.Web.Core.Sessions;
using System.ComponentModel;
using System.Web.UI;
using System.Text;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Exceptions;
using System;
using System.Web.UI.WebControls;
namespace TNS.AdExpress.Web.Controls.Selections.VP
{
    /// <summary>
    /// Affiche le r�sultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleResultBaseWebControl runat=server></{0}:VpScheduleResultBaseWebControl>")]
    public class VpScheduleSelectionImageWebControl : VpScheduleSelectionBaseWebControl {

        #region Variables
        /// <summary>
        /// Image
        /// </summary>
        Image _image = null;
        #endregion

        #region Property (Style)
        /// <summary>
        /// Picture Src Path
        /// </summary>
        protected string _pictureSrcPath = "";
        /// <summary>
        /// Get / Set Picture Src Path
        /// </summary>
        [Bindable(true),
        Category("Picture Src Path"),
        DefaultValue("")]
        public string PictureSrcPath {
            get { return _pictureSrcPath; }
            set { _pictureSrcPath = value; }
        }
        /// <summary>
        /// Picture Src Path Over
        /// </summary>
        protected string _pictureSrcPathOver = "";
        /// <summary>
        /// Get / Set Picture Src Path Over
        /// </summary>
        [Bindable(true),
        Category("Picture Src Path Over"),
        DefaultValue("")]
        public string PictureSrcPathOver {
            get { return _pictureSrcPathOver; }
            set { _pictureSrcPathOver = value; }
        }
        #endregion

        #region Ev�nements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            _image = new Image();
            _image.ID = this.ID;
            this.Controls.Add(_image);
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            _image.ImageUrl = _pictureSrcPath;
            _image.Attributes.Add("onmouseover", "javascript:this.src='"+ _pictureSrcPathOver+"'");
            _image.Attributes.Add("onmouseout", "javascript:this.src='" + _pictureSrcPath + "'");
            _image.Attributes.Add("onclick", _validationMethod);
            _image.CssClass = CssClass;
        }
        #endregion

        #region Pr�Render
        /// <summary>
        /// Pr�rendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
        }
        #endregion

        #region Render
        /// <summary> 
        /// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel �crire </param>
        protected override void Render(HtmlTextWriter output) {
            base.Render(output);
            _image.RenderControl(output);
        }
        #endregion

        #endregion

    }
}

