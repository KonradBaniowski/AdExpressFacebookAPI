#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
// Modification date:
#endregion
using System;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Result;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;

using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;

using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.FrameWork.Collections;


namespace TNS.AdExpressI.Portofolio.Russia.Engines {
	/// <summary>
	/// Compute media detail's results
	/// </summary>
	public class MediaDetailEngine : TNS.AdExpressI.Portofolio.Engines.MediaDetailEngine {

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd,bool excel)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, excel) {
		}

		#endregion

        #region GetFormattedTableDetailMedia
        /// <summary>
        /// Create a table with each week day the media's investment
        /// and the number of spot
        /// </summary>
        /// <returns>table with each week day the media's investment
        ///  and the number of spot</returns>
        override public DataTable GetFormattedTableDetailMedia(IPortofolioDAL portofolioDAL){

            List<UnitInformation> unitsList = _webSession.GetValidUnitForResult();
            DataSet ds = portofolioDAL.GetData();
            if (ds == null || ds.Tables.Count == 0)
                return null;
            else 
                return ds.Tables[0];
        }
        #endregion

        #region HTML for vehicle view
        ///// <summary>
        ///// Get view of the vehicle (HTML)
        ///// </summary>
        ///// <param name="excel">True for excel result</param>
        ///// <param name="resultType">Result Type (Synthesis, MediaDetail)</param>
        ///// <returns>HTML code</returns>
        //public override string GetVehicleViewHtml(bool excel, int resultType)
        //{
        //    return Engines.CommonFunction.GetVehicleViewHtml(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, excel, resultType);
        //}
        #endregion

        #region GetScreenCodeText
        /// <summary>
        /// Get screen code formated depending on country specificities
        /// </summary>
        /// <param name="screenCode">screen code that we get from DAL</param>
        /// <returns>Screen code formated</returns>
        /// <remarks>We've added this method for Russia</remarks>
        override protected string GetScreenCodeText(string screenCode){

            Dictionary<string, int> timeSlotDic = GetTimeSlotWebWordCode();

            return GestionWeb.GetWebWord(timeSlotDic[screenCode], _webSession.SiteLanguage);

        }
        #endregion

        #region GetTimeSlotWebWordCode
        /// <summary>
        ///Get hour interval web word code 
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, int> GetTimeSlotWebWordCode(){

            Dictionary<string, int> dic = new Dictionary<string, int>();
            
            dic.Add("0509", 2759);
            dic.Add("0609", 2750);
            dic.Add("0912", 2751);
            dic.Add("1215", 2752);
            dic.Add("1518", 2753);
            dic.Add("1821", 2754);
            dic.Add("2124", 2755);
            dic.Add("2405", 2760);
            dic.Add("2406", 2756);

            return dic;
        }
        #endregion

    }
}
