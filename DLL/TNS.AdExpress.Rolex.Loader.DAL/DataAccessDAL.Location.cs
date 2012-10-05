using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Rolex.Loader.DAL.DbType;

namespace TNS.AdExpress.Rolex.Loader.DAL
{
 public   partial class DataAccessDAL
    {
     public List<DataLocation> SelectLocation(DataAccessDb db, long idLanguage)
     {
         var query = from p in db.Location where p.IdLanguage == idLanguage select p;
         return query.ToList();
     }
    }
}
