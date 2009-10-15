using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TNS.LSForms
{
    public partial class PluginConfig : UserControl
    {
        #region Variables
        /// <summary>
        /// dynamik Task Format
        /// </summary>
        private string _taskFormat = string.Empty;
        /// <summary>
        /// Family Id
        /// </summary>
        private int _familyId = 122;
        /// <summary>
        /// Module Id
        /// </summary>
        private int _moduleId = 2;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PluginConfig()
        {
            _taskFormat = "<task_meta family_id=\"" + _familyId + "\" module_id=\"" + _moduleId + "\" name=\"{0}\" parameter_id=\"{1}\" />";
            InitializeComponent();
        }
        #endregion

        #region Assessor

        #region XMLTask
        /// <summary>
        /// Get or Set XMLTask
        /// </summary>
        public string XMLTask
        {
            get { return string.Format(this._taskFormat, this.txtTaskName.Text.Replace("\"", ""), this.txtStativNavSession.Text.Trim()); }
            set
            {
                Regex xmlTask = new Regex("name=\"([^\"]+)\" parameter_id=\"([^\"]+)\"");
                if (xmlTask.IsMatch(value))
                {
                    Match res = xmlTask.Match(value);
                    this.txtTaskName.Text = res.Groups[1].Captures[0].Value;
                    this.txtStativNavSession.Text = res.Groups[2].Captures[0].Value;
                }
            }
        }
        #endregion

        #region FamilyId
        /// <summary>
        /// Get Family Id
        /// </summary>
        public int FamilyId {
            get { return _familyId; }
        }
        #endregion

        #region ModuleId
        /// <summary>
        /// Get Module Id
        /// </summary>
        public int ModuleId {
            get { return _moduleId; }
        }
        #endregion

        #endregion

        #region Public Methods

        #region IsValid
        /// <summary>
        /// Is Valid
        /// </summary>
        /// <returns>Valid or not</returns>
        public bool IsValid() {
            int test;

            if (int.TryParse(this.txtStativNavSession.Text, out test) == false ||
                this.txtTaskName.Text.Trim().Length == 0)
                return (false);
            return (true);
        }
        #endregion

        #endregion
    }
}
