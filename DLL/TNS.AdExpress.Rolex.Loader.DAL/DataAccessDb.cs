using System;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using BLToolkit.Data.Linq;
using TNS.AdExpress.Rolex.Loader.DAL.DbType;

namespace TNS.AdExpress.Rolex.Loader.DAL
{
    public class DataAccessDb : DbManager
    {
        public DataAccessDb(String connexionString, DataProviderBase provider)
            : base(provider, connexionString)
        {
        }

        public Table<DataLocation> Location { get { return GetTable<DataLocation>(); } }
        public Table<DataTypePresence> TypePresence { get { return GetTable<DataTypePresence>(); } }
        public Table<DataMedia> Media { get { return GetTable<DataMedia>(); } }
        public Table<DataRolex> Rolex { get { return GetTable<DataRolex>(); } }
        public long GetVisualSequenceId() { return DataAccessDbConnectionFactory.GetSequenceId("ROLEX03", "VISUAL", this); }
        public long GetDataRolexSequenceId() { return DataAccessDbConnectionFactory.GetSequenceId("ROLEX03", "DATA_ROLEX", this); }
        public long GetPageIdSequenceId() { return DataAccessDbConnectionFactory.GetSequenceId("ROLEX03", "ID_PAGE", this); }
    }
}
