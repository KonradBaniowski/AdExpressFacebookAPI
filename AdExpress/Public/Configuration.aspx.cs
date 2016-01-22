using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Constantes.Web;

namespace AdExpress.Public{
	/// <summary>
	/// Page Web Expliquant la configuration n�cessaire au site AdExpress
	/// </summary>
	public partial class Configuration : WebPage{

        #region Variables
        /// <summary>
        /// Langue du site
        /// </summary>
        public int _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
        #endregion
	
		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la pages
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		private void Page_Load(object sender, System.EventArgs e){
            try
            {
                if(Page.Request.QueryString.Get("siteLanguage") == null)
                {
                    _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
                    PageTitleWebControl1.Language = _siteLanguage;
                }
                else
                {
                    _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
                    PageTitleWebControl1.Language = _siteLanguage;
                }
                foreach(Control current in this.Controls[3].Controls)
                {
                    if(current.GetType() == typeof(TNS.AdExpress.Web.Controls.Translation.AdExpressText))
                    {
                        ((TNS.AdExpress.Web.Controls.Translation.AdExpressText)current).Language = _siteLanguage;
                    }
                }
                //Modification de la langue pour les Textes AdExpress
                //TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls,_siteLanguage);

                //langage de l'ent�te
                HeaderWebControl1.Language = _siteLanguage;
                HeaderWebControl1.ActiveMenu = MenuTraductions.CONFIGURATION;
            }
            catch(System.Exception et)
            {
                Response.Redirect("/Public/Message.aspx?msgTxt=" + et.Message.Replace("&", " ") + "&back=2&siteLanguage=" + _siteLanguage);
            }
		}
		#endregion
	
		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent() {    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#endregion

	}
}
