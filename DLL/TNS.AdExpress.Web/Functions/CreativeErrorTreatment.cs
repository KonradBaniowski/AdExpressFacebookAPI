#region Information
// Auteur: Y. R'kaina
// Date de création: 04/02/2008 
// Date de modification: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

using TNS.AdExpress.Web.UI;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Functions {

    /// <summary>
    /// Gestion d'erreur pour le plan media version quali
    /// </summary>
    public class CreativeErrorTreatment {
        
        #region Gestion des erreurs
        /// <summary>
        /// Evènement d'erreur
        /// </summary>
        /// <param name="page">La page qui appel la méthode</param>
        /// <param name="siteLanguage">Le language du site</param>
        /// <param name="e">Argument</param>
        public static void OnError(EventArgs e, Page page, int siteLanguage) {
            EventArgs errorArgs = e;
            
            TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
            try {
                BaseException err = ((BaseException)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]);
                cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException((System.Web.UI.Page)(((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.sender]), err.Message, err.GetHtmlDetail(), ((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.custormerSession]));
            }
            catch (System.Exception) {
                try {
                    cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException((System.Web.UI.Page)(((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.sender]), ((System.Exception)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.error]).Message, ((System.Exception)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.error]).StackTrace, ((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.custormerSession]));
                }
                catch (System.Exception es) {
                    throw (es);
                }
            }
            cwe.SendMail();
            manageCustomerError(cwe, page, siteLanguage);
        }

        /// <summary>
        /// Traite l'affichage d'erreur en fonction du mode compilation
        /// </summary>
        /// <param name="source">La source de données</param>
        /// <param name="page">La page qui appel la méthode</param>
        /// <param name="siteLanguage">La langue du site</param>
        private static void manageCustomerError(object source, Page page, int siteLanguage) {
            //#if DEBUG			
            //	throw((TNS.AdExpress.Web.Exceptions.CustomerWebException)source);				
            //#else
            // Script
            if (!page.ClientScript.IsClientScriptBlockRegistered("redirectError")) {
                page.Response.Write(WebFunctions.Script.RedirectCreativeError(siteLanguage.ToString()));
                page.Response.Flush();
                page.Response.End();
            }
            //#endif
        }
        #endregion

    }
}
