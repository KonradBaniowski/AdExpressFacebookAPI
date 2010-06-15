#region Informations
// Auteur : B.Masson
// Date de cr�ation : 14/02/2007
// Date de modification :
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using TNS.AdExpress.Hermes.ScheduleServer;

namespace HermesScheduleServer{
	/// <summary>
	/// Formulaire Exit
	/// </summary>
	public class Exit : System.Windows.Forms.Form{

		#region Variables
		/// <summary>
		/// Hermes Schedule Server
		/// </summary>
		TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer _hermesScheduleServer;
		#endregion

		#region Variables MMI
		/// <summary>
		/// Variable n�cessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/// <summary>
		/// Button
		/// </summary>
		private System.Windows.Forms.Button btForce;
		/// <summary>
		/// Label message
		/// </summary>
		private System.Windows.Forms.Label lblMessage;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public Exit(TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer hermesScheduleServer){
			InitializeComponent();
			_hermesScheduleServer = hermesScheduleServer;
			lblMessage.Text="Des traitements sont en cours";
		}
		#endregion

		#region Destructeur
		/// <summary>
		/// Nettoyage des ressources utilis�es.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Code g�n�r� par le Concepteur Windows Form
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent()
		{
			this.btForce = new System.Windows.Forms.Button();
			this.lblMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btForce
			// 
			this.btForce.Location = new System.Drawing.Point(255, 48);
			this.btForce.Name = "btForce";
			this.btForce.TabIndex = 3;
			this.btForce.Text = "Force";
			this.btForce.Click += new System.EventHandler(this.btForce_Click);
			// 
			// lblMessage
			// 
			this.lblMessage.Location = new System.Drawing.Point(7, 8);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(320, 23);
			this.lblMessage.TabIndex = 2;
			this.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// Exit
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(336, 78);
			this.ControlBox = false;
			this.Controls.Add(this.btForce);
			this.Controls.Add(this.lblMessage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Exit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Exit";
			this.ResumeLayout(false);

		}
		#endregion

		#region Ev�nements
		/// <summary>
		/// Bouton Force
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Argument</param>
		private void btForce_Click(object sender, System.EventArgs e) {
			this.Close();
		}
		#endregion

	}
}
