using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.DatabaseDescriptionMapper.Constantes;

namespace TNS.DatabaseDescriptionMapper
{
    public class DatabaseConfiguration
    {
        private Dictionary<Tables, TableDescription> _tables;

        public TableDescription GetTable(Tables tableId)
        {
            try
            {
                TableDescription table = _tables[tableId];
                if (table == null) throw (new Exception("Table Object is null"));
                return (table);
            }
            catch (System.Exception err)
            {
                throw (new Exception("Impossible to retreive table object", err));
            }
        }
    }
}
