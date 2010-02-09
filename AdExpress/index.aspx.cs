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
using TNS.AdExpress.Domain.Web;

using Oracle.DataAccess.Client;
using TNS.FrameWork.DB.Common;

using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.Ares.Alerts;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Classification;
using System.Reflection;
using TNS.Ares.Domain.LS;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Alerts.DAL;
using TNS.AdExpressI.Date.DAL;


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
				passwordTextbox.Attributes.Add("onkeypress","javascript:trapEnter(event);");
				if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"trapEnter",WebFunctions.Script.TrapEnter("ImageButtonRollOverWebControl1"));
				}
				#endregion
			
				#region Textes et langage du site
				_setLanguage = string.Empty;
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
                #region Nouvelle version
                //Right Object Creation

				TNS.AdExpress.Right newRight = new TNS.AdExpress.Right(login, password, _siteLanguage);
                if(newRight.CanAccessToAdExpress()) {
                    newRight.SetModuleRights();
                    newRight.SetFlagsRights();
                    newRight.SetRights();
                    //newRight.HasModuleAssignmentAlertsAdExpress();
                    if(_webSession==null) _webSession = new WebSession(newRight);
                    _webSession.SiteLanguage=this._siteLanguage;
                    // Année courante pour les recaps                    
                    TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                    if (cl == null) throw (new NullReferenceException("Core layer is null for the Date DAL"));
                    IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);
                    _webSession.DownLoadDate = dateDAL.GetLastLoadedYear();
                    // On met à jour IDataSource à partir de la session elle même.
                    _webSession.Source=newRight.Source;
                    //Sauvegarder la session
                    _webSession.Save();
                    // Tracking (NewConnection)
                    // On obtient l'adresse IP:
                    _webSession.OnNewConnection(this.Request.UserHostAddress);

                    // Checking if the QueryString contains a idAlert and idOcc
                    string idAlert = Request.QueryString["idAlert"];
                    string idOcc = Request.QueryString["idOcc"];
                    if (idAlert != null && idOcc != null)
                    { 
                        int alertId = int.Parse(idAlert);
                        int occId = int.Parse(idOcc);

                        DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
                        TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                        IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null, null);
                        TNS.Alert.Domain.Alert alert = alertDAL.GetAlert(alertId);
                        if (_webSession.CustomerLogin.IdLogin == alert.CustomerId)
                        {
                            TNS.Alert.Domain.AlertOccurence occ = alertDAL.GetOccurrence(occId, alertId);
                            if (occ != null && occ.AlertId == alert.AlertId)
                                Response.Redirect("/Private/Alerts/ShowAlert.aspx?idSession=" + _webSession.IdSession +
                                    "&idOcc=" + idOcc + "&idAlert=" + idAlert);
                            else
                                Response.Redirect("/Private/Alerts/ShowAlerts.aspx?idSession=" + _webSession.IdSession.ToString());
                        }
                    }
                    else
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
            catch (System.Exception)
            {
                // Erreur Web
                Response.Write("<script language=javascript>");
                Response.Write("	alert(\"" + GestionWeb.GetWebWord(880, this._siteLanguage) + "\");");
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
