using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Anubis.Miysis.BusinessFacade;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Miysis.Common;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Anubis.Miysis.Exceptions;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Anubis.Celebrities.BusinessFacade {
    /// <summary>
    /// 
    /// </summary>
    public class CelebritiesPdfSystem : Miysis.BusinessFacade.MiysisPdfSystem {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public CelebritiesPdfSystem(IDataSource dataSource, MiysisConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
            : base(dataSource, config, rqDetails, webSession, theme) {
            
            try {
                this._dataSource = dataSource;
                this._config = config;
                this._rqDetails = rqDetails;
                this._webSession = webSession;
            }
            catch (Exception e) {
                throw new MiysisPdfException("Error in Constructor MiysisPdfSystem", e);
            }
        }
        #endregion

        #region Get Title
        /// <summary>
        /// Get Title
        /// </summary>
        /// <returns>Title</returns>
        protected override string GetTitle() {
            return GestionWeb.GetWebWord(2949, _webSession.SiteLanguage);
        }
        #endregion

        #region Get Mail Content
        /// <summary>
        /// Get Mail Content
        /// </summary>
        /// <returns>Mail content</returns>
        protected override string GetMailContent() {
            return GestionWeb.GetWebWord(2966, _webSession.SiteLanguage);
        }
        #endregion

    }
}
