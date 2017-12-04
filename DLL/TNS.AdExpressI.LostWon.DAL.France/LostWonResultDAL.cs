using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.LostWon.DAL.France
{
    public class LostWonResultDAL : DAL.LostWonResultDAL
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public LostWonResultDAL(WebSession session) : base(session) { }
        #endregion

        #region Group BY
        protected override string GetGroupOptional(UnitInformation u, string dataTablePrefixe)
        {
            return $", {WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label}.LISTNUM_TO_CHAR({dataTablePrefixe}.{u.DatabaseMultimediaField}) ";
        }
        #endregion
    }
}
