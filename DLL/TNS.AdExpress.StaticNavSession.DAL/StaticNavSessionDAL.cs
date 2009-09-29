using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Data;

using AdConst = TNS.AdExpress.Constantes.DB;
using AnubisConst = TNS.AdExpress.Anubis.Constantes;
using System.Collections;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.StaticNavSession.DAL
{
    public abstract class StaticNavSessionDAL : TNS.AdExpress.StaticNavSession.StaticNavSessionDAL
    {
        public StaticNavSessionDAL(IDataSource source) : base(source)
        { }

    }
}
