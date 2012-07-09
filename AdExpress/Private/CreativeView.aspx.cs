#region Information
// Auteur : G. Facon
// Créé le : 25/10/2005
// Modifié le : 
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using AdExpressWebRules = TNS.AdExpress.Web.Rules;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.UI;
using ProductClassification = TNS.AdExpress.DataAccess.Classification.ProductBranch;
using MediaClassification = TNS.AdExpress.DataAccess.Classification.MediaBranch;
using AdExpressException = TNS.AdExpress.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;

using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress;
using TNS.AdExpressI.Visual;
using System.Reflection;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;

namespace AdExpress.Private
{
    /// <summary>
    /// Point d'entrer du site AdExpress Pour TNS Creative Explorer
    /// </summary>
    public partial class CreativeView : TNS.AdExpress.Web.UI.WebPage
    {

        #region Evènements

        #region Chargement de la Page
        /// <summary>
        /// Chargement de la Page
        /// </summary>
        /// <param name="sender">Objet Source</param>
        /// <param name="e">Paramètres</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                #region On récupère les paramètres de TNS Creative Explorer
                string path = HttpContext.Current.Request.QueryString.Get("path");
                Int64 vehicleId = Int64.Parse(HttpContext.Current.Request.QueryString.Get("id_vehicle"));
                bool isBlur = false;
                bool.TryParse(HttpContext.Current.Request.QueryString.Get("is_blur"), out isBlur);
                string encrypt = HttpContext.Current.Request.QueryString.Get("crypt");
                string idSession = HttpContext.Current.Request.QueryString.Get("idSession");
                string save = HttpContext.Current.Request.QueryString.Get("cd");
                string isCover = HttpContext.Current.Request.QueryString.Get("cv");



                #endregion
                object[] parameters = null;
                if (!string.IsNullOrEmpty(idSession) || !string.IsNullOrEmpty(encrypt) || !string.IsNullOrEmpty(isCover))
                {
                    parameters = new object[5];
                    parameters[0] = vehicleId;
                    parameters[1] = path;
                    parameters[2] = idSession;
                    parameters[3] = (!string.IsNullOrEmpty(encrypt) && encrypt == "1");
                    parameters[4] = (!string.IsNullOrEmpty(isCover) && isCover == "1");
                }
                else
                {
                    parameters = new object[2];
                    parameters[0] = vehicleId;
                    parameters[1] = path;
                }
                IVisual visual = (IVisual)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].AssemblyName, WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                visual.Theme = this.Theme;
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();

                string contentType = visual.GetContentType();
                if (!string.IsNullOrEmpty(contentType))
                    Page.Response.ContentType = contentType;

                string headerDsipostion = visual.AddHeader();
                if (!string.IsNullOrEmpty(save) && save.Equals("sv") && !string.IsNullOrEmpty(headerDsipostion))
                    Page.Response.AddHeader("Content-Disposition", headerDsipostion);
                Page.Response.BinaryWrite(visual.GetBinaries(isBlur));

            }
            catch (Exception exc)
            {

            }

        }
        #endregion

        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Paramètres</param>
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion


        /// <summary>
        /// Add javascripts
        /// </summary>
        protected override void AddScritps()
        {
        }

    }
}
