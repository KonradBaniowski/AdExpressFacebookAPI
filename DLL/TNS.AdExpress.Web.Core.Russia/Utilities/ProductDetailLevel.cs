#region Informations
// Auteur:
// Création:
// Modification:
//		G. Facon		12/08/2005	Nom des méthodes

#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using TNS.FrameWork;
using RightConstantes = TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using System.Collections;

namespace TNS.AdExpress.Web.Core.Russia.Utilities
{
    /// <summary>
    /// Fonctions des niveaux de détail produit
    /// </summary>
    public class ProductDetailLevel : TNS.AdExpress.Web.Core.Utilities.ProductDetailLevel
    {

        public ProductDetailLevel()
            : base()
        {
        }

        /// <summary>
        /// Obtient le niveau de détail produit par défaut en fonction du module
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Niveau de détail</returns>
        public override WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails GetDefaultPreformatedProductDetails(WebSession webSession)
        {
            switch (webSession.CurrentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
                case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
                    return (WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group);
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_CONCURENTIELLE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_POTENTIELS:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES:
                    return (WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser);
                default:
                    return (WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup);
            }
        }


        /// <summary>
        /// Get Default Generic Detail Level Ids
        /// </summary>
        /// <param name="currentModule">current Module</param>
        /// <returns>Array List etail Level Ids</returns>
        public override ArrayList GetDefaultGenericDetailLevelIds(long currentModule)
        {
            #region Niveau de détail media (Generic)
            ArrayList levels = new ArrayList();
            switch (currentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES:
                    // Famille
                    levels.Add(11);
                    break;
                default:
                    levels.Add(8);
                    break;
            }
            return levels;
            #endregion
        }
    }
}
