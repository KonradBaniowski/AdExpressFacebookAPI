using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.AdVolumeChecker.Domain;
using KMI.AdExpress.AdVolumeChecker.Domain.XmlLoader;
using KMI.AdExpress.AdVolumeChecker.Exceptions;
using KMI.AdExpress.AdVolumeChecker.Rules;
using TNS.Ares.Domain.LS;
using TNS.FrameWork.DB.BusinessFacade.Oracle;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.Common.Oracle;
using TNS.FrameWork.Net.Mail;

namespace KMI.AdExpress.AdVolumeChecker {

    public delegate void MessageDelegate(int progress);

    /// <summary>
    /// This alert will allow CSA to monitor Ad volumes
    /// </summary>
    public class AdVolumeCheckerShell {

        #region Variables
        /// <summary>
        /// Mail qui envoie le log
        /// </summary>
        protected SmtpUtilities _errMail;
        #endregion

        public event MessageDelegate Message;

        protected void OnMessage(int progress) {
            // If event is wired, fire it!
            if (Message != null)
                Message(progress);
        }

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errMail">Error Mail</param>
        public AdVolumeCheckerShell(SmtpUtilities errMail) {
            _errMail = errMail;
        }
        #endregion

        #region DoTask
        /// <summary>
        /// Dotask
        /// </summary>
        public void DoTask(string versionList, string advertiserList, string productList, bool isIn, string path, string period) {

            try {

                try {

                    // Main directoty initialization
                    string configurationPathDirecory = AppDomain.CurrentDomain.BaseDirectory + Constantes.Application.APPLICATION_CONFIGURATION_DIRECTORY + @"\";

                    DateTime startDate = GetStartDate(period);
                    DateTime endDate = GetEndDate(period);

                    MediaInformations.Init(new XmlReaderDataSource(configurationPathDirecory + Constantes.Application.MEDIA_CONFIGURATION));

                    Results.Message += Results_Message;
                    Results.GenerateExcelResult(DataBaseInformation.ConnectionString, MediaInformations.MediaInformationList, startDate, endDate, versionList, advertiserList, productList, isIn, path);

                    #region Stop work and send notification mail
                    _errMail.SendWithoutThread("Ad Volume Checker", "<html><body><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Ad Volume Checker treatment was successfully executed.</font></body></html>", true, false);
                    #endregion

                }
                catch (Exception e) {
                    throw new AdVolumeCheckerShellException("Error during Ad Volume Checker loader main treatment : " + e.Message, e);
                }
               
            }
            catch (Exception ex) {
                throw new AdVolumeCheckerShellException("An error occurred during Ad Volume Checker treatment task : " + ex.Message, ex);
           }

        }

        private void Results_Message(int progress) {
            OnMessage(progress);
        }
        #endregion

        #region Get Start Date
        private DateTime GetStartDate(string period) {

            string date = period.Substring(3, 10);

            return new DateTime(int.Parse(date.Substring(6, 4)), int.Parse(date.Substring(3, 2)), int.Parse(date.Substring(0, 2)));

        }
        private DateTime GetEndDate(string period) {

            string date = period.Substring(17, 10);

            return new DateTime(int.Parse(date.Substring(6, 4)), int.Parse(date.Substring(3, 2)), int.Parse(date.Substring(0, 2)));

        }
        #endregion

    }

    

}
