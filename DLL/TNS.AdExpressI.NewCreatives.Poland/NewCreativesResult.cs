using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.NewCreatives.Poland
{
    public class NewCreativesResult:NewCreatives.NewCreativesResult {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public NewCreativesResult(WebSession session)
            : base(session)
        {
        }
        #endregion

        protected override bool CanShowCreative()
        {
            return _vehicleInformation.ShowCreations;
        }

        protected override void SetCellCreativesLink(ResultTable tab, int iCurLine, int iCol, AdExpressCellLevel[] cellLevels, int i)
        {
            tab[iCurLine, iCol] = new TNS.AdExpress.Web.Core.Poland.Result.CellCreativesLink(cellLevels[i],
                _webSession, _webSession.GenericProductDetailLevel, string.Empty, -1);
        }

    }
}
