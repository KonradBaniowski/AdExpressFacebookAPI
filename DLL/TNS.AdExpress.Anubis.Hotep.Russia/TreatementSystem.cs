#region Information
//Author : Y. Rkaina 
//Creation : 11/07/2006
#endregion

using System;
using System.Data;
using System.Threading;

using TNS.AdExpress.Anubis.Hotep.Common;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using PDFCreatorPilotLib;
using TNS.AdExpress.Domain.Web;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.Ares;
using TNS.AdExpress.Domain.Layers;
using TNS.Ares.StaticNavSession.DAL;
using System.Reflection;
using TNS.AdExpress.Anubis.Hotep.Exceptions;
using System.IO;
using TNS.Ares.Domain.LS;
using TNS.AdExpress.Anubis.Hotep.Russia.BusinessFacade;

namespace TNS.AdExpress.Anubis.Hotep.Russia{
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for Hotep plug-in
	/// </summary>
    public class TreatementSystem : TNS.AdExpress.Anubis.Hotep.TreatementSystem
    {

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public TreatementSystem():base(){
		}
		#endregion

		#region Traitment du résultat
		/// <summary>
		/// Generate the PDF for Hotep plug-in
		/// </summary>
		protected override void ComputeTreatement(){
			HotepPdfSystem pdf = null;
			try{
                RaiseStartWorkEvent();

				#region Request Details
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
				#endregion

				#region PDF management

                pdf = new HotepPdfSystem(_dataSource, _hotepConfig, rqDetails, _webSession, _theme);
				string fileName = pdf.Init();
				pdf.AutoLaunch = false;
				pdf.Fill();
				pdf.EndDoc();
                _dataAccess.RegisterFile(_navSessionId, fileName);
				pdf.Send(fileName);
                _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Hotep);
                //if (pluginInformation != null && pluginInformation.DeleteRowSuccess)
                //    _dataAccess.DeleteRow(_navSessionId);
				#endregion

                RaiseStopWorkerJobEvent();
			}
			catch(System.Exception err){
                if (_dataAccess != null)
                    _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                RaiseErrorEvent(err);
				return;
			}
		}
		#endregion
	}
}
