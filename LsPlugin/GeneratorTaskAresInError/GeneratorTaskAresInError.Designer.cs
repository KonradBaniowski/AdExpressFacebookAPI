using TNS.LSForms;
namespace GeneratorTaskAresInError {
    partial class GeneratorTaskAresInError {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent() {
            this.btOk = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.pluginConfig = new TNS.LSForms.PluginConfig();
            this.SuspendLayout();
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(86, 131);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(94, 41);
            this.btOk.TabIndex = 1;
            this.btOk.Text = "Ok";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(186, 131);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(94, 41);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // pluginConfig
            // 
            this.pluginConfig.Location = new System.Drawing.Point(12, 12);
            this.pluginConfig.Name = "pluginConfig";
            this.pluginConfig.Size = new System.Drawing.Size(342, 110);
            this.pluginConfig.TabIndex = 0;
            this.pluginConfig.XMLTask = "<task_meta family_id=\"122\" module_id=\"2\" name=\"AdExpress - Task in error\" paramet" +
                "er_id=\"\" />";
            // 
            // GeneratorTaskAresInError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 182);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.pluginConfig);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GeneratorTaskAresInError";
            this.Text = "Ares task generator";
            this.ResumeLayout(false);

        }

        private PluginConfig pluginConfig;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Button Cancel;
        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private PluginConfig pluginConfig1;
    }
}

