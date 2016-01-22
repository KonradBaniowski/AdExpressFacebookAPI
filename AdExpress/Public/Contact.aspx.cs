using System;
using System.Collections;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Web;
//using System.Web.SessionState;
using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.HtmlControls;

using TNSMail=TNS.FrameWork.Net.Mail;
using TNS.AdExpress.Domain.Translation;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstTrad = TNS.AdExpress.Constantes.DB.Language;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace AdExpress.Public{
	/// <summary>
	/// Page permettant à un visiteur de faire une demande d'information ou de contacter quelqu'un.
	/// Si le visiteur saisie des informations valides, un mail est envoyer à ...
	/// Paramètre iundispensable:la langue du site
	/// </summary>
	public partial class Contact : WebPage{
	
		#region Variables
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage=TNS.AdExpress.Constantes.DB.Language.FRENCH;
		#endregion
	
		#region Evènements

		#region Chargement
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void Page_Load(object sender, System.EventArgs e){
			try{
                if(Page.Request.QueryString.Get("siteLanguage") == null) {
                    PageTitleWebControl1.Language = _siteLanguage = CstTrad.FRENCH;
                }
                else {
                    PageTitleWebControl1.Language = _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
                }
				foreach(Control current in this.Controls[3].Controls){
					if(current.GetType()==typeof(TNS.AdExpress.Web.Controls.Translation.AdExpressText)){
						((TNS.AdExpress.Web.Controls.Translation.AdExpressText)current).Language=_siteLanguage;
					}
				}
				//langage du bouton valider
                validateButton.ImageUrl = "/App_Themes/"+this.Theme+"/Images/Culture/Button/valider_up.gif";
                validateButton.RollOverImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/Button/valider_down.gif";
				//langage de l'entête
				HeaderWebControl1.Language = _siteLanguage;
				HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.INFO_REQUEST;
				nameValid.ErrorMessage = GestionWeb.GetWebWord(765, _siteLanguage);
				companyValid.ErrorMessage = GestionWeb.GetWebWord(766, _siteLanguage);
				mailValid.ErrorMessage = GestionWeb.GetWebWord(767, _siteLanguage);
				mailFormatValid.ErrorMessage = GestionWeb.GetWebWord(768, _siteLanguage);
			}catch(System.Exception et){
				Response.Redirect("/Public/Message.aspx?msgTxt="+et.Message.Replace("&"," ")+"&back=2&siteLanguage="+_siteLanguage);
			}
		}
		#endregion

		#region Initialisation

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){    
			this.validateButton.Click += new System.EventHandler(this.sendMail);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#endregion

		#region Envoi du Mail
		/// <summary>
		/// Réaction au clique sur le bouton valider
		/// </summary>
		/// <param name="sender">Objet qui envoie l'évènement</param>
		/// <param name="e">Arguments</param>
		private void sendMail(object sender, System.EventArgs e) {
			ArrayList tos = new ArrayList();
			tos.Add(CstWeb.Contact.mailRecipient);
			string message = "Une demande d'information a ete postee sur le site d'AdExpress 3.0 le " + DateTime.Today.ToShortDateString();
			message += ((nameTxt.Text.Length>0)?"\n":"")+nameTxt.Text;
			message += ((companyTxt.Text.Length>0)?"\n":"")+companyTxt.Text;
			message += ((adrTxt.Text.Length>0)?"\n":"")+adrTxt.Text;
			message += ((cpTxt.Text.Length>0)?"\n":"")+cpTxt.Text;
			message += ((townTxt.Text.Length>0)?"\n":"")+townTxt.Text;
			message += ((countryTxt.Text.Length>0)?"\n":"")+countryTxt.Text;
			message += ((telTxt.Text.Length>0)?"\n":"")+telTxt.Text;
			message += ((mailTxt.Text.Length>0)?"\n":"")+mailTxt.Text;
			message += ((commentTxt.Text.Length>0)?"\n\nCommentaire :\n":"")+commentTxt.Text;
			message=Convertion.ToHtmlString(message);

            TNSMail.SmtpUtilities mail = new TNSMail.SmtpUtilities("Contact@TNSAdExpress.com", tos, "AdExpress 3.0 : demande d'information", message, true, "frmitpu-msprod", 25);			
			
			mail.mailKoHandler += new TNSMail.SmtpUtilities.mailKoEventHandler(GestKO);
			mail.Send(true);
			Response.Redirect("/Public/Message.aspx?msgCode=2&back=2&siteLanguage="+_siteLanguage);
		}
		#endregion

		#endregion
		
		private void GestKO(object o, string str){
			Response.Redirect("/Public/Message.aspx?msgCode=2&back=1&siteLanguage="+_siteLanguage);
		}
		
	}
}
