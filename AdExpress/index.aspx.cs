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
using TNS.AdExpress;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;

using Oracle.DataAccess.Client;
using TNS.FrameWork.DB.Common;

using WebFunctions = TNS.AdExpress.Web.Functions;

namespace AdExpress{
	/// <summary>
	/// Page d'accueil
	/// </summary>
	//public partial class index : System.Web.UI.Page{
    public partial class index : TNS.AdExpress.Web.UI.WebPage {

		#region Variables MMI
		/// <summary>
		/// Login
		/// </summary>
		protected System.Web.UI.WebControls.TextBox TextBox1;
		/// <summary>
		/// Bouton valider
		/// </summary>
		protected System.Web.UI.WebControls.ImageButton ImageButton1;
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText2;
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		#endregion

		#region Variables

		///<summary>
		/// objet WebSession
		/// </summary>
		///  <directed>True</directed>
		///  <supplierCardinality>1</supplierCardinality>
		protected WebSession _webSession = null;
        ///// <summary>
        ///// Langue du site
        ///// </summary>
        //public int _siteLanguage=TNS.AdExpress.Constantes.DB.Language.FRENCH;
		/// <summary>
		/// Identifiant de la nouvelle langue
		/// </summary>
		protected int newLanguage=TNS.AdExpress.Constantes.DB.Language.ENGLISH;
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Saisie de l'identifiant de l'utilisateur
		/// </summary>
		/// <summary>
		/// Saisie du mot de passe
		/// </summary>
		/// <summary>
		/// Composant entête
		/// </summary>
		/// <summary>
		/// Texte d'accueil
		/// </summary>
		/// <summary>
		/// Texte de la nouvelle Langue
		/// </summary>
		protected string newLanguageText="English Version";
		/// <summary>
		/// Code javascript pour positionner la langue dans le cookie
		/// </summary>
		public string _setLanguage = string.Empty;
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
				#region Gestion de l'évènements "enter" sur le textBox passwd
				passwordTextbox.Attributes.Add("onkeypress","javascript:trapEnter()");
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",WebFunctions.Script.TrapEnter("ImageButtonRollOverWebControl1"));
				}
				#endregion
			
				#region Textes et langage du site
				_setLanguage = string.Empty;
				if(Page.Request.QueryString.Get("siteLanguage")!=null){
                    //_siteLanguage=int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
                    _setLanguage = string.Format("setPermanentCookie(\"{1}\",{0});", _siteLanguage, Cookies.LANGUAGE);
				}
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_siteLanguage);
				//langage de l'entête
				HeaderWebControl1.Language = _siteLanguage;
				//Bouton valider
				//ImageButtonRollOverWebControl1.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//ImageButtonRollOverWebControl1.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				#endregion

			}
			catch(System.Exception et){
				string bobo=et.Message;
			}
		}
		#endregion

		#region Valider
		protected void ImageButtonRollOverWebControl1_Click(object sender, System.EventArgs e) {
			string login=loginTextbox.Text;
			string password=passwordTextbox.Text;
			//string connectionString="User Id="+login+";Password="+password+Connection.SESSION_CONNECTION_STRING;
			try{
                #region Acienne version Creer l'objet Right
                /*TNS.FrameWork.DB.Common.IDataSource source = new TNS.FrameWork.DB.Common.OracleDataSource("User Id=" + login + "; Password=" + password + " " + TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING);
				TNS.AdExpress.Web.Core.WebRight loginRight=new TNS.AdExpress.Web.Core.WebRight(login,password,source);
				//Vérifier le droit d'accès au site
				if (loginRight.CanAccessToAdExpress(source)){
					// Regarde à partir de quel tables charger les droits clients
					// (template ou droits propres au login)
						loginRight.htRightUserBD();				
					//Vérifier la cohérence des droits
					//if (loginRight.checkRightCohesion()){
					if(true){
						//Creer une session
						if(_webSession==null) _webSession = new WebSession(loginRight);
						_webSession.SiteLanguage=this._siteLanguage;
						// Année courante pour les recaps
						_webSession.DownLoadDate=TNS.AdExpress.Web.BusinessFacade.Selections.Periods.RecapBusinessFacade.GetLastLoadedYear();
						// On met à jour IDataSource à partir de la session elle même.
						_webSession.Source=source;
						//Sauvegarder la session
						_webSession.Save();
						// Tracking (NewConnection)
						// On obtient l'adresse IP:
						_webSession.OnNewConnection(this.Request.UserHostAddress);
						//Se Rediriger vers la page des modules
// ------------->       Response.Redirect("Private/selectionModule.aspx?idSession="+_webSession.IdSession);
					}
//					else{
//						// Problèmes de droits
//						Response.Write("<script language=javascript>");
//						Response.Write("	alert(\""+GestionWeb.GetWebWord(879,this._siteLanguage)+"\");");
//						Response.Write("</script>");
//					}
				}
				else{
					// L'accès est impossible
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(880,this._siteLanguage)+"\");");
					Response.Write("</script>");
                }*/
                #endregion

                #region Nouvelle version
                //Right Object Creation

                TNS.AdExpress.Right newRight=new  TNS.AdExpress.Right(login,password);
                if(newRight.CanAccessToAdExpress()) {
                    newRight.SetModuleRights();
                    newRight.SetFlagsRights();
                    newRight.SetRights();
                    if(_webSession==null) _webSession = new WebSession(newRight);
                    _webSession.SiteLanguage=this._siteLanguage;
                    // Année courante pour les recaps
                    _webSession.DownLoadDate=TNS.AdExpress.Web.BusinessFacade.Selections.Periods.RecapBusinessFacade.GetLastLoadedYear();
                    // On met à jour IDataSource à partir de la session elle même.
                    _webSession.Source=newRight.Source;
                    //Sauvegarder la session
                    _webSession.Save();
                    // Tracking (NewConnection)
                    // On obtient l'adresse IP:
                    _webSession.OnNewConnection(this.Request.UserHostAddress);
                    //Se Rediriger vers la page des modules
                    Response.Redirect("Private/selectionModule.aspx?idSession="+_webSession.IdSession);
                }
				else{
					// L'accès est impossible
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(880,this._siteLanguage)+"\");");
					Response.Write("</script>");
                }
                #endregion
            }
            catch(TNS.AdExpress.Exceptions.AdExpressCustomerException){
				// Erreur de droits
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(880,this._siteLanguage)+"\");");
				Response.Write("</script>");

			}
			catch(TNS.AdExpress.Web.Core.Exceptions.WebSessionException){
				// Erreur Web
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(880,this._siteLanguage)+"\");");
				Response.Write("</script>");

			}
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs non modifés)
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
			loginTextbox.Attributes.Add("AUTOCOMPLETE","ON");
			loginTextbox.Attributes.Add("value","");
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		
	}
}
