using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TNS.AdExpress.Anubis.BusinessFacade.Core;

namespace Anubis {
	public partial class Exit : Form {

		#region Variables
		/// <summary>
		/// Serveur de distribution des Jobs
		/// </summary>
		DistributionSystem _distributionServer;
		#endregion

		#region Variables MMI
		/// <summary>
		/// Bouton force
		/// </summary>
		private System.Windows.Forms.Button btForce;
		/// <summary>
		/// Message
		/// </summary>
		private System.Windows.Forms.Label lblMessage;
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Délégués
		/// <summary>
		/// Délégué pour rappel de la méthode de notification d'un message
		/// </summary>
		public delegate void MessageCallBack();	

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public Exit(DistributionSystem distributionServer) {
			_distributionServer = distributionServer;
			_distributionServer.OnStopJob += new TNS.AdExpress.Anubis.BusinessFacade.Core.DistributionSystem.StopJob(_distributionServer_OnStopJob);
			InitializeComponent();
			lblMessage.Text = _distributionServer.RunningJobCount.ToString() + " job(s) still running";
		}
		#endregion

		/// <summary>
		/// Un job s'est arrêté
		/// </summary>
		/// <param name="navSessionId">Identifiant du résultat</param>
		/// <param name="message">Message</param>
		private void _distributionServer_OnStopJob(long navSessionId, string message) {
			#region Old version
			//lblMessage.Text = _distributionServer.RunningJobCount.ToString() + " job(s) still running";
			//if (_distributionServer.RunningJobCount == 0) btForce_Click(this, null);
			#endregion

			if (this.InvokeRequired) {
				MessageCallBack callBack = new MessageCallBack(MT_SetMessage);
				this.Invoke(callBack);
			}
			else {
				lblMessage.Text = _distributionServer.RunningJobCount.ToString() + " job(s) still running";
			}
			if (_distributionServer.RunningJobCount == 0) btForce_Click(this, null);
		}

		private void btForce_Click(object sender, System.EventArgs e) {
			this.Close();
			//this.Dispose();
		}

		private void MT_SetMessage() {
			lblMessage.Text = _distributionServer.RunningJobCount.ToString() + " job(s) still running";
		}
	}
}