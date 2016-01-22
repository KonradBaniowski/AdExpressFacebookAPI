using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;

namespace KM.AdExpress.AlertPreRoll.DAL
{
    public class PreRollDb : DbManager
    {
        public PreRollDb(String connexionString, DataProviderBase provider)
            : base(provider, connexionString)
        {
        }
    }

    
}
