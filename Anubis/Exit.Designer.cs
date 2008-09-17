namespace Anubis {
	partial class Exit {
		///// <summary>
		///// Variable nécessaire au concepteur.
		///// </summary>
		//private System.ComponentModel.IContainer components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Exit));
			this.btForce = new System.Windows.Forms.Button();
			this.lblMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btForce
			// 
			this.btForce.Location = new System.Drawing.Point(248, 48);
			this.btForce.Name = "btForce";
			this.btForce.Size = new System.Drawing.Size(75, 23);
			this.btForce.TabIndex = 1;
			this.btForce.Text = "Force";
			this.btForce.Click += new System.EventHandler(this.btForce_Click);
			// 
			// lblMessage
			// 
			this.lblMessage.Location = new System.Drawing.Point(5, 16);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(328, 24);
			this.lblMessage.TabIndex = 2;
			this.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// Exit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(338, 88);
			this.ControlBox = false;
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.btForce);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Exit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Exit";
			this.ResumeLayout(false);

		}

		#endregion
	}
}