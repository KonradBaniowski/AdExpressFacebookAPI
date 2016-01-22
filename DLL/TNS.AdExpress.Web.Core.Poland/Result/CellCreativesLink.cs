using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Poland.Result
{
   public  class CellCreativesLink : Core.Result.CellCreativesLink
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Niveau de détail</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
        /// <param name="zoomDate">Date du zoom, vide si pas de zoom</param>
        /// <param name="universId">Identifiant de l'univers, -1 s'il n'y en a pas</param>
       public CellCreativesLink(CellLevel cellLevel, WebSession webSession,
           GenericDetailLevel genericDetailLevel, string zoomDate, int universId)
           : base(cellLevel, webSession, genericDetailLevel, zoomDate, universId)
       {
           _linkRules = new DefaultShowLinkRules(cellLevel, genericDetailLevel);
       }

       /// <summary>
       /// Constructeur
       /// </summary>
       /// <param name="cellLevel">Niveau de détail</param>
       /// <param name="webSession">Session du client</param>
       /// <param name="genericDetailLevel">Niveau de détail générique</param>
       public CellCreativesLink(CellLevel cellLevel, WebSession webSession,
           GenericDetailLevel genericDetailLevel) : base(cellLevel, webSession, genericDetailLevel)
       {
           _linkRules = new DefaultShowLinkRules(cellLevel, genericDetailLevel);
       }
    }
}
