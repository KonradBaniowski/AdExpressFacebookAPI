﻿namespace TNS.LSForms
{
    partial class PluginConfig
    {

        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.ctlPluginConfig = new System.Windows.Forms.GroupBox();
            this.txtStativNavSession = new System.Windows.Forms.TextBox();
            this.txtTaskName = new System.Windows.Forms.TextBox();
            this.lblAlertId = new System.Windows.Forms.Label();
            this.lblTaskName = new System.Windows.Forms.Label();
            this.ctlPluginConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctlPluginConfig
            // 
            this.ctlPluginConfig.Controls.Add(this.txtStativNavSession);
            this.ctlPluginConfig.Controls.Add(this.txtTaskName);
            this.ctlPluginConfig.Controls.Add(this.lblAlertId);
            this.ctlPluginConfig.Controls.Add(this.lblTaskName);
            this.ctlPluginConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlPluginConfig.Location = new System.Drawing.Point(0, 0);
            this.ctlPluginConfig.Margin = new System.Windows.Forms.Padding(10);
            this.ctlPluginConfig.Name = "ctlPluginConfig";
            this.ctlPluginConfig.Size = new System.Drawing.Size(342, 110);
            this.ctlPluginConfig.TabIndex = 0;
            this.ctlPluginConfig.TabStop = false;
            this.ctlPluginConfig.Text = "Paramètres";
            // 
            // txtStativNavSession
            // 
            this.txtStativNavSession.Location = new System.Drawing.Point(99, 62);
            this.txtStativNavSession.Name = "txtStativNavSession";
            this.txtStativNavSession.Size = new System.Drawing.Size(194, 20);
            this.txtStativNavSession.TabIndex = 3;
            // 
            // txtTaskName
            // 
            this.txtTaskName.Location = new System.Drawing.Point(100, 29);
            this.txtTaskName.Name = "txtTaskName";
            this.txtTaskName.Size = new System.Drawing.Size(193, 20);
            this.txtTaskName.TabIndex = 2;
            this.txtTaskName.Text = "AdExpress - Alert Task in error";
            // 
            // lblAlertId
            // 
            this.lblAlertId.AutoSize = true;
            this.lblAlertId.Location = new System.Drawing.Point(47, 65);
            this.lblAlertId.Name = "lblAlertId";
            this.lblAlertId.Size = new System.Drawing.Size(46, 13);
            this.lblAlertId.TabIndex = 1;
            this.lblAlertId.Text = "Alert Id :";
            // 
            // lblTaskName
            // 
            this.lblTaskName.AutoSize = true;
            this.lblTaskName.Location = new System.Drawing.Point(28, 32);
            this.lblTaskName.Name = "lblTaskName";
            this.lblTaskName.Size = new System.Drawing.Size(66, 13);
            this.lblTaskName.TabIndex = 0;
            this.lblTaskName.Text = "Task name :";
            // 
            // PluginConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctlPluginConfig);
            this.Name = "PluginConfig";
            this.Size = new System.Drawing.Size(342, 110);
            this.ctlPluginConfig.ResumeLayout(false);
            this.ctlPluginConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ctlPluginConfig;
        private System.Windows.Forms.Label lblAlertId;
        private System.Windows.Forms.Label lblTaskName;
        private System.Windows.Forms.TextBox txtStativNavSession;
        private System.Windows.Forms.TextBox txtTaskName;
    }
}