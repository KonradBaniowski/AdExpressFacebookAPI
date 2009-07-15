using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Alerts.DAL
{
    public class AlertsDAL : TNS.AdExpress.Alerts.AlertsDAL
    {
        public AlertsDAL(IDataSource source)
            : base(source)
        { }
    }
}
