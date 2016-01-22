using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using AdExpress.Exceptions;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;
using TNS.AdExpress.Domain.Web;

public partial class Private_P3 : System.Web.UI.Page
{
    const string SPLITTER = "¤¤";
    const int NB_PARAMS = 4;

    #region Variables
    /// <summary>
    /// Langue de la page
    /// </summary>
    private int _siteLanguage = -1;
    private string _login = "";
    private string _password = "";
    private string _date = "";
    private string _url = "";
    string _redirectUrl = "/index.aspx";
    #endregion

    	#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
    public Private_P3()
        : base()
    {
        
		}
		#endregion

    #region Chargement de la Page
    /// <summary>
    /// Chargement de la Page
    /// </summary>
    /// <param name="sender">Objet Source</param>
    /// <param name="e">Paramètres</param>
    protected void Page_Load(object sender, System.EventArgs e)
    {


        WebSession webSession = null;
        try
        {

            #region On récupère les paramètres de TNS Creative Explorer
            string paramsEncryted = HttpContext.Current.Request.QueryString.Get("p");
            string paramsDecryted = (!string.IsNullOrEmpty(paramsEncryted)) ? TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(paramsEncryted) : "";
            string[] paramsArr = paramsDecryted.Split(new string[] { SPLITTER }, StringSplitOptions.None);

            if (paramsArr != null && paramsArr.Length == NB_PARAMS)
            {
                _login = paramsArr[0];
                _password = paramsArr[1];
                _siteLanguage = int.Parse(paramsArr[2]);
                _date = paramsArr[3];
            }
            #endregion

            #region Connection to AdExpress
            //WebSession webSession=null;
            //Creer l'objet Right
            TNS.AdExpress.Right loginRight = new TNS.AdExpress.Right(_login, _password, _siteLanguage);
            //Vérifier le droit d'accès au site
            if (CanAccessToAdExpress(loginRight))
            {
                //Creer une session
                if (webSession == null) webSession = new WebSession(loginRight);
                loginRight.SetModuleRights();
                loginRight.SetFlagsRights();
                loginRight.SetRights();
                if (WebApplicationParameters.VehiclesFormatInformation.Use)
                    loginRight.SetBannersAssignement();
                webSession.SiteLanguage = _siteLanguage;

                // Année courante pour les recaps                    
                TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Date DAL"));
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);
                webSession.DownLoadDate = dateDAL.GetLastLoadedYear();
                // On met à jour IDataSource à partir de la session elle même.
                webSession.Source = loginRight.Source;
                //webSession.LastReachedResultUrl = Page.Request.Url.AbsolutePath;

                //Sauvegarder la session
                webSession.Save();

                // Tracking (NewConnection)
                // On obtient l'adresse IP:
                webSession.OnNewConnection(this.Request.UserHostAddress);

                _url = "/Private/selectionModule.aspx?idSession=" + webSession.IdSession + "&siteLanguage=" + webSession.SiteLanguage;
                Response.Redirect(_url);

                //Response.Flush();
                //Response.End();
            }
            else
            {

                HttpContext.Current.Response.Redirect(_redirectUrl);
            }
            #endregion
        }
        catch (System.Threading.ThreadAbortException) { }
        catch (System.Exception err)
        {
            //Redirect vers page index            
            HttpContext.Current.Response.Redirect(_redirectUrl);
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
    protected void Page_UnLoad(object sender, System.EventArgs e)
    {
        try
        {
           // HttpContext.Current.Response.Redirect(_url);

        }
        catch (System.Exception exc)
        {
            //Redirect vers page index
            string redirectUrl = "/index.aspx";
            HttpContext.Current.Response.Redirect(redirectUrl);

        }
    }
    #endregion

    #region OnPreRender
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        

    }
    #endregion

    private bool CanAccessToAdExpress(TNS.AdExpress.Right loginRight)
    {
        return (_login.Length > 0 && _password.Length > 0 && _siteLanguage > 0
            && (DateTime.Now.ToString("yyyyMMdd") == _date)
            && loginRight.CanAccessToAdExpress()
            );
    }
}
