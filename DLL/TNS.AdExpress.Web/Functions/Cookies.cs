#region Informations
// Auteur: D. Mussuma
// Date de création: 12/02/2007 
// Date de modification: 
#endregion
using System;
using System.Web;
using System.Web.UI.WebControls;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Functions
{
	/// <summary>
	/// Description résumée de Cookies.
	/// </summary>
	public class Cookies
	{
		/// <summary>
		/// Charge l'email pour l'export distant à effectuer
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="isRegisterEmailForRemotingExport">Cookie Indique s'il faut enregister les préférences popur l'export</param>
		/// <param name="savedEmailForRemotingExport">Cookie Mail pour l'export</param>
		/// <param name="cbxRegisterMail">Case à cocher enregister l'email</param>
		/// <param name="tbxMail">Zone de texte pour l'email</param>
		public static void LoadSavedEmailForRemotingExport(System.Web.UI.Page page,HttpCookie isRegisterEmailForRemotingExport, HttpCookie savedEmailForRemotingExport, CheckBox cbxRegisterMail, TextBox tbxMail)
		{
			//Si les cookies existent				
			isRegisterEmailForRemotingExport =  page.Request.Cookies[WebConstantes.Cookies.IsRegisterEmailForRemotingExport];
			savedEmailForRemotingExport = page.Request.Cookies[WebConstantes.Cookies.SavedEmailForRemotingExport];

			if(isRegisterEmailForRemotingExport!=null && savedEmailForRemotingExport!=null){
							
				//Récupère le booléen qui indique si l'email doit être sauvegardé
				cbxRegisterMail.Checked = (isRegisterEmailForRemotingExport.Value.Equals("true")) ? true : false;
							
				//Récupère l'email enregistré
				if(cbxRegisterMail.Checked)
					tbxMail.Text = savedEmailForRemotingExport.Value.ToString().Trim();
			}
		}

        /// <summary>
        /// Gets or sets the cookie containing the alert email list
        /// </summary>
        /// <param name="page">Current web page</param>
        /// <param name="get">Defines if we shoulg set or get the cookie</param>
        /// <param name="emailList">Email list</param>
        /// <returns>The cookie value if existing</returns>
        public static string ManageEmailListCookie(System.Web.UI.Page page, bool get, string emailList)
        {
            HttpCookie cookie = page.Request.Cookies[WebConstantes.Cookies.AlertEmailList];
            if (get)
            {
                if (cookie != null)
                    return (cookie.Value);
                else
                    return (null);
            }
            else
            {
                page.Response.Cookies[WebConstantes.Cookies.AlertEmailList].Value = emailList;
                page.Response.Cookies[WebConstantes.Cookies.AlertEmailList].Expires = DateTime.MaxValue;
                return (null);
            }

        }

		/// <summary>
		/// Enregistre l'email pour l'export distant d'un fichier (anubis) dans un cookie
		/// </summary>
		/// <param name="page">Page courante</param>
		/// <param name="mail">Email</param>
		/// <param name="cbxRegisterMail">Case à cocher enregister l'email </param>
		public static void SaveEmailForRemotingExport(System.Web.UI.Page page,string mail,CheckBox cbxRegisterMail){
			HttpCookie isRegisterEmailForRemotingExport = null, savedEmailForRemotingExport = null ;

			if(page.Request.Cookies[WebConstantes.Cookies.IsRegisterEmailForRemotingExport]!=null && page.Request.Cookies[WebConstantes.Cookies.SavedEmailForRemotingExport]!=null){
							
				//Enregistre le cookie qui indique s'il faut sauvegarder l'email
				page.Response.Cookies[WebConstantes.Cookies.IsRegisterEmailForRemotingExport].Value = (cbxRegisterMail.Checked)? "true" : "false"; 
				page.Response.Cookies[WebConstantes.Cookies.IsRegisterEmailForRemotingExport].Expires =  DateTime.MaxValue;

				//Enregister l'email
				page.Response.Cookies[WebConstantes.Cookies.SavedEmailForRemotingExport].Value = mail.Trim(); 
				page.Response.Cookies[WebConstantes.Cookies.SavedEmailForRemotingExport].Expires =  DateTime.MaxValue;

			}else{
				//Crée les cookies s'ils n'existent pas
				isRegisterEmailForRemotingExport = new HttpCookie(WebConstantes.Cookies.IsRegisterEmailForRemotingExport,(cbxRegisterMail.Checked)? "true" : "false");
				isRegisterEmailForRemotingExport.Expires = DateTime.MaxValue;
				page.Response.Cookies.Add(isRegisterEmailForRemotingExport);

				savedEmailForRemotingExport = new HttpCookie(WebConstantes.Cookies.SavedEmailForRemotingExport,mail.Trim());
				savedEmailForRemotingExport.Expires = DateTime.MaxValue;
				page.Response.Cookies.Add(savedEmailForRemotingExport);
			}
		}
	}
}
