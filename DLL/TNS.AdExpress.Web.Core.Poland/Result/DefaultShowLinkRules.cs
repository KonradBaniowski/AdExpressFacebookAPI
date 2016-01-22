using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Poland.Result
{
  public  class DefaultShowLinkRules : Core.Result.DefaultShowLinkRules
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellLevel">Cellule de niveau cliqué</param>
        /// <param name="genericDetailLevel">Niveau de détail générique</param>
      public DefaultShowLinkRules(CellLevel cellLevel, GenericDetailLevel genericDetailLevel) 
          : base(cellLevel, genericDetailLevel)
      {
      }

      /// <summary>
      /// Indique si le lien doit être montrée dans la Cellule
      /// </summary>
      /// <returns>True s'il doit être montré, false sinon</returns>
      public override bool ShowLink()
      {
          return _cellLevel != null && (_cellLevel.Level > 0);
      }
    }
}
