using System;
using System.Data;
using System.IO;
using TNS.AdExpress.Anubis.Apis.BusinessFacade;
using TNS.AdExpress.Anubis.Apis.Common;
using TNS.AdExpress.Anubis.Apis.Exceptions;
using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Domain.Web;
using TNS.Ares.Domain.LS;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.WebTheme;

namespace TNS.AdExpress.Anubis.Apis{
    /// <summary>
    /// Description résumée de TreatementSystem.
    /// </summary>
    public class TreatementSystem : Miysis.TreatementSystem {

        /// <summary>
        /// Configuration du plug-in
        /// </summary>
        public ApisConfig _apisConfig;
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
        protected override void ComputeTreatement() {
            try {
                OnStartWork(_navSessionId, GetPluginName() + " started for " + _navSessionId);

                #region Request Details
                var rqDetails = _dataAccess.GetRow(_navSessionId);
                #endregion

                #region PDF management

                var pdf = new ApisPdfSystem(_dataSource, _apisConfig, rqDetails, _webSession, _theme);            
                string fileName = pdf.Init();
                pdf.AutoLaunch = false;             
                pdf.Fill();
                pdf.EndDoc();
                _dataAccess.RegisterFile(_navSessionId, fileName);
                pdf.Send(fileName);
                _dataAccess.UpdateStatus(_navSessionId, Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                var pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Apis);
                if (pluginInformation != null && pluginInformation.DeleteRowSuccess)
                    _dataAccess.DeleteRow(_navSessionId);
                #endregion

                OnStopWorkerJob(_navSessionId, "", "", GetPluginName() + " finished for '" + _navSessionId + "'");
            }
            catch (Exception err) {
                if (_dataAccess != null)
                    _dataAccess.UpdateStatus(_navSessionId, Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                OnError(_navSessionId, "Erreur lors du traitement du résultat for '" + _navSessionId + "'.", err);
            }
        }
        #endregion

        /// <summary>
        /// Initialize Configuration
        /// </summary>
        /// <param name="configurationFilePath"></param>
        protected override void InitializeConfig(string configurationFilePath)
        {
            #region Check Path File
            if (configurationFilePath == null)
            {
                throw new ApisPdfException("Impossible de lancer le traitement d'un job", new ArgumentNullException("configurationFilePath"));
            }
            if (configurationFilePath.Length == 0)
            {
                throw new ApisPdfException("Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
            }
            #endregion

            #region Initialize Apis
            try
            {
                _apisConfig = new ApisConfig(new XmlReaderDataSource(configurationFilePath));
            }
            catch (Exception err)
            {
                throw new ApisPdfException("Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration", err);
            }
            #endregion
        }


        /// <summary>
        /// Initialize Theme
        /// </summary>
        protected override void InitializeTheme()
        {
            #region Initialize Theme
            var pathFileTheme = string.Empty;
            try
            {

                if (File.Exists(_apisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml"))
                {
                    pathFileTheme = _apisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                }
                else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + _apisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml"))
                {
                    pathFileTheme = AppDomain.CurrentDomain.BaseDirectory + _apisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                }
                else
                {
                    pathFileTheme = _apisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                }
                _theme = new Theme(new XmlReaderDataSource(pathFileTheme));
            }
            catch (Exception err)
            {
                throw new ApisPdfException(string.Format("File of theme not found ! (in Plugin Apis in TreatmentSystem class) - Path : '{0}'", pathFileTheme), err);
            }
            #endregion
        }

       
    }
}
