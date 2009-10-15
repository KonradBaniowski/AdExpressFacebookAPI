using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TNS.LinkSystem.LinkKernel;
using System.Xml;
using System.Text.RegularExpressions;

namespace GeneratorTaskAresAlertInError {
    public partial class GeneratorTaskAresAlertInError : Form, ITaskConfigurationPlugin {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public GeneratorTaskAresAlertInError() {
            InitializeComponent();
        }
        #endregion

        #region ITaskConfigurationPlugin Membres
        /// <summary>
        /// GetConfigurationFileList
        /// </summary>
        /// <returns>List of File</returns>
        public List<string> GetConfigurationFileList() {
            return (null);
        }

        /// <summary>
        /// CreateConfigurationDialog
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public string CreateConfigurationDialog(List<XmlDocument> files) {
            if (this.ShowDialog() == DialogResult.OK)
                return (this.pluginConfig.XMLTask);
            return (null);
        }

        /// <summary>
        /// EditConfigurationDialog
        /// </summary>
        /// <param name="strTaskToEdit"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public string EditConfigurationDialog(string strTaskToEdit, List<XmlDocument> files) {
            this.pluginConfig.XMLTask = strTaskToEdit;
            if (this.ShowDialog() == DialogResult.OK)
                return (this.pluginConfig.XMLTask);
            return (strTaskToEdit);
        }

        /// <summary>
        /// Family id 122 corresponds to the process that
        /// creates dynamic tasks which are then dispatched
        /// to the appropriate plugin
        /// </summary>
        /// <returns>122</returns>
        public int GetFamilyID() {
            return (this.pluginConfig.FamilyId);
        }

        /// <summary>
        /// This module id will be dispatched to the 122
        /// family so it can retrieve the row from database
        /// and specify which module id should be set for
        /// the 123 family
        /// </summary>
        /// <returns>2</returns>
        public int GetModuleID() {
            return (this.pluginConfig.ModuleId);
        }

        #endregion

        #region Event Form

        #region Cancel_Click
        /// <summary>
        /// Cancel Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Cancel_Click(object sender, EventArgs e) {
            this.FindForm().Close();
        }
        #endregion

        #region btOk_Click
        /// <summary>
        /// Cancel Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void btOk_Click(object sender, EventArgs e) {
            if (this.pluginConfig.IsValid() == true) {
                Form parent = this.FindForm();
                parent.DialogResult = DialogResult.OK;
                parent.Close();
            }
        }
        #endregion

        private void pluginConfig_Load(object sender, EventArgs e) {

        }

        #endregion
    }
}
