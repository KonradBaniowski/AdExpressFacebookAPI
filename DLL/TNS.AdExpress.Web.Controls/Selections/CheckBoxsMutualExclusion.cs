using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TNS.AdExpress.Web.Controls.Selections{
    /// <summary>
    /// Web control used to handle mutual exclusion between a list of checkboxs
    /// </summary>
    [ToolboxData("<{0}:CheckBoxsMutualExclusion runat=server></{0}:CheckBoxsMutualExclusion>")]
    public class CheckBoxsMutualExclusion : System.Web.UI.WebControls.WebControl
    {

        #region Variables
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
        /// <param name="checkBoxList">CheckBox list</param>
        public CheckBoxsMutualExclusion(List<CheckBox> checkBoxList) : base() 
        {
            this.EnableViewState = true;
            this.PreRender += new EventHandler(Custom_PreRender);
            _checkBoxList = checkBoxList;
        }
        #endregion

        #region Events

        #region Load
        /// <summary>
        /// launched when the control is loaded
        /// </summary>
        /// <param name="e">arguments</param>
        protected override void OnLoad(EventArgs e) {

            _checkBoxNameList = new List<string>();

            foreach (CheckBox currentCheckBox in _checkBoxList) {
                _checkBoxNameList.Add(currentCheckBox.ID);
            }

            // Javascript function for mutual exclusion between pdm and pdv (Be able to check one of theme)
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("MutualExclusion"))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MutualExclusion", TNS.AdExpress.Web.Functions.Script.MutualExclusion(_checkBoxNameList));
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
            foreach (CheckBox currentCheckBox in _checkBoxList) {
                currentCheckBox.Attributes.Add("onclick", "javascript:MutualExclusion('" + currentCheckBox.ID + "');");
            }
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {

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
