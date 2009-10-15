using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using Oracle.DataAccess.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;
using AresConst = TNS.Ares.Constantes.Constantes;
using TNS.Ares.Domain.LS;
using TNS.Ares.Domain.DataBaseDescription;


namespace TNS.Ares.StaticNavSession.DAL.Default
{
    public class StaticNavSessionDAL : TNS.Ares.StaticNavSession.DAL.StaticNavSessionDAL
    {
        public StaticNavSessionDAL(IDataSource source)
            : base(source)
        {
        }
        
    }
}
