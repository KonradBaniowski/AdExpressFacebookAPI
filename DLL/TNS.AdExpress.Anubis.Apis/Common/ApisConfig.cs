using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Apis.Common
{
    public class ApisConfig : MiysisConfig
    {
        #region Constructor
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de données</param>
        public ApisConfig(IDataSource dataSource):base(dataSource)
        {
			
		}
		#endregion
    }
}
