#region Informations
/*
 * Author : G Ragneau
 * Created on : 23/09/2008
 * Modifications :
 *      Date - Author - Description
 * 
 * 
 * 
 * 
 * 
 * */
/*
 * history: moved from TNS.AdExpress.Web
 * Auteur:D. V. Mussuma
 * Création: 12/12/2005
 * Modification:
 *      12/01/2006 B. Masson Ajout des niveaux de détail supports

 * */
#endregion

using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
namespace TNS.AdExpress.Web.Core.Poland.Utilities
{
    /// <summary>
    /// Fonctions des niveaux de détail média
    /// </summary>
    public class MediaDetailLevel : Core.Utilities.MediaDetailLevel
    {

        public MediaDetailLevel()
            : base()
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerWebSession">_customer WebSession</param>
        /// <param name="componentProfile">component Profile</param>
        public MediaDetailLevel(WebSession customerWebSession, WebConstantes.GenericDetailLevel.ComponentProfile componentProfile)
            : base(customerWebSession, componentProfile)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerWebSession">_customer WebSession</param>
        public MediaDetailLevel(WebSession customerWebSession)
            : base(customerWebSession)
        {          
        }



    }
}
