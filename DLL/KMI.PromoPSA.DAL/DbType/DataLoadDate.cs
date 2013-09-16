using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.DAL.DbType {

    [TableName(Name = Constantes.Db.PROMO_SCHEMA + ".DATA_PROMOTION")]
    public class DataLoadDate {

        [MapField("LOAD_DATE")]
        public long LoadDate { get; set; }

    }

}
