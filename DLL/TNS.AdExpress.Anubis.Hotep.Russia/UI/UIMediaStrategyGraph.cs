using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Hotep.Common;
using Dundas.Charting.WinControl;

namespace TNS.AdExpress.Anubis.Hotep.Russia.UI
{
    public  class UIMediaStrategyGraph : TNS.AdExpress.Anubis.Hotep.UI.UIMediaStrategyGraph
    {
        #region Constructeur
        public UIMediaStrategyGraph(WebSession webSession,IDataSource dataSource,HotepConfig config,
            TNS.FrameWork.WebTheme.Style style, Dictionary<string, Series> listSeriesMedia,Dictionary<int, string> listSeriesName)
            : base( webSession, dataSource,  config, style, listSeriesMedia,listSeriesName)
        {            
        }
        #endregion
    }
}
