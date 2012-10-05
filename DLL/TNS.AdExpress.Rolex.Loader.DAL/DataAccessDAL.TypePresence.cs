using System.Collections.Generic;
using System.Linq;
using TNS.AdExpress.Rolex.Loader.DAL.DbType;

namespace TNS.AdExpress.Rolex.Loader.DAL
{
    public partial class DataAccessDAL
    {
        public List<DataTypePresence> SelectTypePresence(DataAccessDb db, long idLanguage)
        {
            var query = from p in db.TypePresence where p.IdLanguage == idLanguage select p;
            return query.ToList();
        }
    }
}
