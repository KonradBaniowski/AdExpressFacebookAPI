using System;
using System.Collections.Generic;
using TNS.AdExpress.Anubis.Miysis;
using TNS.AdExpress.Anubis.Celebrities.BusinessFacade;
using System.Data;
using TNS.Ares.Domain.LS;

namespace TNS.AdExpress.Anubis.Celebrities{
    /// <summary>
    /// Description résumée de TreatementSystem.
    /// </summary>
    public class TreatementSystem : Miysis.TreatementSystem {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public TreatementSystem() : base() {
        }
        #endregion

        #region Nom du Plug-in
        /// <summary>
        /// Obtient le nom du plug-in
        /// </summary>
        /// <returns>Le nom du plug-in</returns>
        public override string GetPluginName() {
            return ("Celebrities PDF Generator");
        }
        #endregion

        #region Traitment du résultat
        /// <summary>
        /// Generate the PDF for Miysis plug-in
        /// </summary>
        protected virtual void ComputeTreatement() {
            CelebritiesPdfSystem pdf;
            try {
                OnStartWork(_navSessionId, GetPluginName() + " started for " + _navSessionId);

                #region Request Details
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
                #endregion

                #region PDF management

                pdf = new CelebritiesPdfSystem(_dataSource, _miysisConfig, rqDetails, _webSession, _theme);
                string fileName = pdf.Init();
                pdf.AutoLaunch = false;
                //TODO update Database for physical file name
                pdf.Fill();
                pdf.EndDoc();
                _dataAccess.RegisterFile(_navSessionId, fileName);
                pdf.Send(fileName);
                _dataAccess.UpdateStatus(_navSessionId, Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Miysis);
                if (pluginInformation != null && pluginInformation.DeleteRowSuccess)
                    _dataAccess.DeleteRow(_navSessionId);
                #endregion

                OnStopWorkerJob(_navSessionId, "", "", GetPluginName() + " finished for '" + _navSessionId + "'");
            }
            catch (Exception err) {
                if (_dataAccess != null)
                    _dataAccess.UpdateStatus(_navSessionId, Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                OnError(_navSessionId, "Erreur lors du traitement du résultat for '" + _navSessionId + "'.", err);
                return;
            }
        }
        #endregion
    }
}
