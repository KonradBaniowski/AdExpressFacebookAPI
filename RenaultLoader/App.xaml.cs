using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using TNS.AdExpress.VP.Loader.Domain.XmlLoader;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.VP.Loader.Domain.Classification;
using TNS.AdExpressI.VP.Loader.Classification;
using System.Reflection;
using System.IO;
using TNS.AdExpress.VP.Loader.Domain.Web;

namespace RenaultLoader
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        protected override void OnStartup(StartupEventArgs e) {

            base.OnStartup(e);
            try {
                // Code qui s'exécute au démarrage de l'application
                string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
                string directoryCountryName = AppParamtersXL.LoadDirectoryName(new XmlReaderDataSource(configurationDirectoryRoot + "ApplicationParameter.xml"));
                string countryConfigurationDirectoryRoot = configurationDirectoryRoot + directoryCountryName + @"\";
                ApplicationParameters.Initialize(countryConfigurationDirectoryRoot);

                IClassifVeillePromo veillePromoDAL = (IClassifVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.classification].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.classification].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { ApplicationParameters.DataBaseDescription }, null, null, null);
                AllClassification.Init(veillePromoDAL.GetAllProduct(), veillePromoDAL.GetAllBrand());

            }
            catch (System.Exception error) {
                string body = "";
                try {
                    TNS.FrameWork.Exceptions.BaseException err = (TNS.FrameWork.Exceptions.BaseException)error;
                    body = "<html><b><u>" + Environment.MachineName + ":</u></b><br>" + "<font color=#FF0000>L'initialisation de l'application a &eacute;chou&eacute;.</font><br>Erreur" + err.GetHtmlDetail() + "</font></html>";
                }
                catch (System.Exception) {
                    try {
                        body = "<html><b><u>" + Environment.MachineName + ":</u></b><br>" + "<font color=#FF0000>L'initialisation de l'application a &eacute;chou&eacute;.</font><br>Erreur(" + error.GetType().FullName + "):" + error.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + error.StackTrace.Replace("at ", "<br>at ") + "</font></html>";
                    }
                    catch (System.Exception es) {
                        throw (es);
                    }
                }
                TNS.FrameWork.Net.Mail.SmtpUtilities errorMail = new TNS.FrameWork.Net.Mail.SmtpUtilities(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIGURATION_DIRECTORY_NAME + @"\ErrorMail.xml"));
                errorMail.Send("Erreur d'initialisation de l'outils d'integration de la veille promotionnelle renault " + (Environment.MachineName), body, true, false);
                System.Windows.MessageBox.Show("Une erreur est survenue lors du chargement de l'application");
                System.Windows.Application.Current.Shutdown();
            }
        }

    }
}
