#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
#endregion

using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using Oracle.DataAccess.Client;
//using TNS.AdExpress.Rules.Customer;
using TNS.AdExpress.Web.Core.Sessions;
using System.Windows.Forms;
using TNS.AdExpress.Domain.Translation;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;

namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Sauvegarde d'une session
	/// </summary>
	public partial class MySessionSavePopUp : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Constructeur
		/// <summary>
		/// Constructeur 
		/// </summary>
		public MySessionSavePopUp():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){						
			try{

				#region Langue du site
				//Modification de la langue pour les Textes AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls,_webSession.SiteLanguage);
				#endregion

				#region Liste des répertoires

				DataSet dsDir = TNS.AdExpress.Web.DataAccess.MyAdExpress.MySessionsDataAccess.GetDirectories(_webSession);
				directoryDropDownList.Items.Clear();
				directoryDropDownList.DataSource = dsDir.Tables[0];
				directoryDropDownList.DataTextField = dsDir.Tables[0].Columns[1].ToString();
				directoryDropDownList.DataValueField = dsDir.Tables[0].Columns[0].ToString();
				directoryDropDownList.DataBind();

				DataSet ds = TNS.AdExpress.Web.DataAccess.MyAdExpress.MySessionsDataAccess.GetData(_webSession);

				#endregion
				
				directoryDropDownList.EnableViewState = false;
				sessionDropDownList.EnableViewState = false;
				//script
				
				if (!this.Page.ClientScript.IsClientScriptBlockRegistered("SessionJavaScriptFunctions"))
					this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SessionJavaScriptFunctions", SessionJavaScriptFunctions(ds,dsDir));

				directoryDropDownList.Attributes.Add("onChange", "fillSessions(this.options[this.selectedIndex].value);");
				sessionDropDownList.Attributes.Add("onChange", "selectSession();");
				mySessionTextBox.Attributes.Add("onKeypress", "deleteSelectedSession();");
							
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page:
		///		Fermeture des connections BD
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// OnInit
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{
           
            this.Unload += new System.EventHandler(this.Page_UnLoad);
		}
		#endregion

		#region Bouton Ok
		/// <summary>
		/// Gestion du bouton OK
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void oKImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try {
				

				if (directoryDropDownList.Items.Count != 0) {
					//string mySessionName = this.mySessionTextBox.Text;
					string mySessionName = WebFunctions.CheckedText.CheckedAccentText(this.mySessionTextBox.Text);
					string idSelectedSession = Request.Form.GetValues("sessionDropDownList")[0];
					string idSelectedDirectory = Request.Form.GetValues("directoryDropDownList")[0];

					
					if (mySessionName.Length == 0 && !idSelectedSession.Equals("0")) {
						string savedSessionName = WebFunctions.CheckedText.CheckedAccentText(MySessionsDataAccess.GetSession(Int64.Parse(idSelectedSession), _webSession));						
						if (savedSessionName.Length > 0 && MySessionDataAccess.UpdateMySession(Int64.Parse(idSelectedDirectory), idSelectedSession, savedSessionName, _webSession)) {
							#region Tracking utilisation sauvegarde
							_webSession.OnUseMyAdExpressSave();
							#endregion

							// Validation : confirmation d'enregistrement de la requête
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\"" + GestionWeb.GetWebWord(826, _webSession.SiteLanguage) + "\");");
							Response.Write("	window.close();");
							Response.Write("</script>");
						}
						else {
							// Erreur : Echec de l'enregistrement de la requête		
							Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(825, _webSession.SiteLanguage)));
						}
						
					}
					else if (mySessionName.Length != 0 && mySessionName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT) {
						if (!MySessionsDataAccess.IsSessionExist(_webSession, mySessionName)) {
							if (MySessionDataAccess.SaveMySession(Int64.Parse(idSelectedDirectory), mySessionName, _webSession)) {

								#region Tracking utilisation sauvegarde
								_webSession.OnUseMyAdExpressSave();
								#endregion

								// Validation : confirmation d'enregistrement de la requête
								Response.Write("<script language=javascript>");
								Response.Write("	alert(\"" + GestionWeb.GetWebWord(826, _webSession.SiteLanguage) + "\");");
								Response.Write("	window.close();");
								Response.Write("</script>");
							}
							else {
								// Erreur : Echec de l'enregistrement de la requête
								Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(825, _webSession.SiteLanguage)));								
							}
						}
						else {
							// Erreur : session déjà existante
							Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(824, _webSession.SiteLanguage)));							
						}
					}
					else if (mySessionName.Length == 0) {
						// Erreur : Le champs est vide
						Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(822, _webSession.SiteLanguage)));						
					}
					else {
						// Erreur : suppérieur à 50 caractères
						Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(823, _webSession.SiteLanguage)));						
					}
				}
				else {
					// Erreur : Impossible de sauvegarder, pas de répertoire créé
					Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(711, _webSession.SiteLanguage)));					
				}		
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton Annuler
		/// <summary>
		/// Gestion du bouton annuler
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void cancelImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			Response.Write("<script language=javascript>");
			Response.Write("	window.close();");
			Response.Write("</script>");
		
		}
		#endregion

		#region Javascript
		/// <summary>
		/// Génére les javascripts utilisés pour la sauvegarde de résultat
		/// </summary>
		/// <param name="ds">Liste des sessions sauvegardées</param>
		/// <param name="dsDir">Liste des répertoires</param>
		/// <returns>Code Javascript</returns>
		private string SessionJavaScriptFunctions(DataSet ds, DataSet dsDir) {

			
			StringBuilder script = new StringBuilder(2000);
			int i = 0, k = 0;
			
			script.Append("<script language=\"JavaScript\">");
			script.Append("\r\n  var directories = new Array(); ");
			sessionDropDownList.Items.Clear();
			sessionDropDownList.Items.Insert(k, new ListItem("------------------", "0"));
			k++;
			foreach (DataRow dr in ds.Tables[0].Rows) {
			
				if (dr["ID_MY_SESSION"] != System.DBNull.Value) {
					script.Append("\r\n directories[" + i + "] = new Array();");
					script.Append("\r\n directories[" + i + "][\"ID_DIRECTORY\"] = \"" + dr["ID_DIRECTORY"].ToString() + "\"; ");
					script.Append("\r\n directories[" + i + "][\"ID_MY_SESSION\"] = \"" + dr["ID_MY_SESSION"].ToString() + "\"; ");
					script.Append("\r\n directories[" + i + "][\"MY_SESSION\"] = \"" + dr["MY_SESSION"].ToString() + "\"; ");
					i++;
				}
				if (dsDir.Tables[0].Rows.Count > 0 && Int64.Parse(dsDir.Tables[0].Rows[0]["ID_DIRECTORY"].ToString()) == Int64.Parse(dr["ID_DIRECTORY"].ToString()) 
					&& dr["ID_MY_SESSION"] != System.DBNull.Value) {
					sessionDropDownList.Items.Insert(k, new ListItem(dr["MY_SESSION"].ToString(), dr["ID_MY_SESSION"].ToString()));
					k++;
				}				
				
			}
						
			script.Append("\r\n function verif()");
			script.Append("\r\n {");
			script.Append("\r\n\t if (document.layers)");
			script.Append("\r\n\t {");
			script.Append("\r\n\t theForm = document.forms.Form1;");
			script.Append("\r\n\t }");
			script.Append("\r\n\t else");
			script.Append("\r\n\t {");
			script.Append("\r\n\t theForm = document.Form1;");
			script.Append("\r\n\t }");
			script.Append("\r\n}");

			script.Append("\r\n function selectSession()");			
			script.Append("\r\n {");
			script.Append("\r\n\t theForm.mySessionTextBox.value=\"\";");
			script.Append("\r\n }");

			script.Append("\r\n function deleteSelectedSession()");
			script.Append("\r\n {");
			script.Append("\r\n\t theForm.sessionDropDownList.options.selectedIndex = 0;");			
			script.Append("\r\n }");

			script.Append("\r\n function fillSessions(codeDirectory)");
			script.Append("\r\n {");			
			script.Append("\r\n\t verif();");
			script.Append("\r\n\t if(codeDirectory>0)");
			script.Append("\r\n\t {");
			script.Append("\r\n\t\t theForm.sessionDropDownList.options.length = 0;");
			script.Append("\r\n\t\t\t\t theForm.sessionDropDownList.options[theForm.sessionDropDownList.options.length] = new Option(\"------------------\",\"0\");");						
			script.Append("\r\n\t\t for (var i=0;i<directories.length;i++)");
			script.Append("\r\n\t\t {");
			script.Append("\r\n\t\t\t if(i==0)");
			script.Append("\r\n\t\t\t {");
			script.Append("\r\n\t\t\t }");
			script.Append("\r\n\t\t\t if(parseInt(directories[i][\"ID_DIRECTORY\"]) == parseInt(codeDirectory))");
			script.Append("\r\n\t\t\t {");
			script.Append("\r\n\t\t\t\t theForm.sessionDropDownList.options[theForm.sessionDropDownList.options.length] = new Option(directories[i][\"MY_SESSION\"],directories[i][\"ID_MY_SESSION\"]);");						
			script.Append("\r\n\t\t\t }");
			script.Append("\r\n\t\t }");
			script.Append("\r\n\t\t theForm.sessionDropDownList.options.selectedIndex = 0;");
			script.Append("\r\n\t }");
			script.Append("\r\n\t else");
			script.Append("\r\n\t {");
			script.Append("\r\n\t\t theForm.sessionDropDownList.options[theForm.sessionDropDownList.options.length] = new Option(\"------------------\",\"0\");");						
			script.Append("\r\n\t }");

			script.Append("\r\n }");					
			script.Append("\r\n </script>");

			return (script.ToString());
		}
		#endregion

	}
}
