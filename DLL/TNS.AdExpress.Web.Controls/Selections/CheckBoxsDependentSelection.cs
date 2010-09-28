using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TNS.AdExpress.Web.Controls.Selections
{
    /// <summary>
    /// Web control used to handle dependent selection (allows a checkbox list to be selectionable only if the reference one is checked)
    /// </summary>
    [ToolboxData("<{0}:CheckBoxsDependentSelection runat=server></{0}:CheckBoxsDependentSelection>")]
    public class CheckBoxsDependentSelection : System.Web.UI.WebControls.WebControl
    {

        #region Variables
        /// <summary>
        /// Reference CheckBox
        /// </summary>
        private CheckBox _referenceCheckBox;
        /// <summary>
        /// CheckBox list
        /// </summary>
        private List<CheckBox> _checkBoxList;
        /// <summary>
        /// CheckBox name list
        /// </summary>
        private List<string> _checkBoxNameList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="referenceCheckBox">Reference CheckBox</param>
        /// <param name="checkBoxList">CheckBox list</param>
        public CheckBoxsDependentSelection(CheckBox referenceCheckBox, List<CheckBox> checkBoxList)
            : base() 
        {
            this.EnableViewState = true;
            this.PreRender += new EventHandler(Custom_PreRender);
            _referenceCheckBox = referenceCheckBox;
            _checkBoxList = checkBoxList;
        }
        #endregion

        #region Events

        #region Load
        /// <summary>
        /// launched when the control is loaded
        /// </summary>
        /// <param name="e">arguments</param>
        protected override void OnLoad(EventArgs e)
        {

            _checkBoxNameList = new List<string>();

            foreach (CheckBox currentCheckBox in _checkBoxList)
            {
                _checkBoxNameList.Add(currentCheckBox.ID);
            }

            // Javascript function that allow a checkbox list to be selectionable only if the reference one is checked
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("EnableCheckBoxList"))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EnableCheckBoxList", TNS.AdExpress.Web.Functions.Script.EnableCheckBoxList(_checkBoxNameList));
            }

            base.OnLoad(e);
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
            _referenceCheckBox.Attributes.Add("onclick", "javascript:EnableCheckBoxList('" + _referenceCheckBox.ID + "');");
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {

            _referenceCheckBox.RenderControl(output);
            output.Write("&nbsp;&nbsp;");

            foreach (CheckBox currentCheckBox in _checkBoxList)
            {
                currentCheckBox.RenderControl(output);
                output.Write("&nbsp;&nbsp;");
            }

        }
        #endregion

        #endregion

    }
}
