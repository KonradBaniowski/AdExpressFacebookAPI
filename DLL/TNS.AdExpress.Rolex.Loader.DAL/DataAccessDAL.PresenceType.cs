using System.Collections.Generic;
using System.Linq;
using TNS.AdExpress.Rolex.Loader.DAL.DbType;

namespace TNS.AdExpress.Rolex.Loader.DAL
{
    public partial class DataAccessDAL
    {
        public List<DataPresenceType> SelectPresenceType(DataAccessDb db, long idLanguage)
        {
            var query = from p in db.PresenceType where p.IdLanguage == idLanguage select p;
            return query.ToList();
        }
    }
}
