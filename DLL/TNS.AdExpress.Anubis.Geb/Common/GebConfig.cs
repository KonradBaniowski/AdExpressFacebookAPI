#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
// Date de modification :
#endregion

#region Namespaces
using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.Baal.Common;
using TNS.Baal.ExtractList;
using GebExceptions=TNS.AdExpress.Anubis.Geb.Exceptions;
#endregion

namespace TNS.AdExpress.Anubis.Geb.Common {
    /// <summary>
    /// Configuration du plugin Geb
    /// </summary>
    public class GebConfig {

        #region variables
        /// <summary>
        /// Chemin du fichier excel
        /// </summary>
        private string _excelPath;
        /// <summary>
        /// Serveur de mail d'envoi des résultats
        /// </summary>
        private string _customerMailServer;
        /// <summary>
        /// Port du serveur de mail d'envoi des résultats
        /// </summary>
        private int _customerMailPort=25;
        /// <summary>
        /// Mail d'envoi des résultats
        /// </summary>
        private string _customerMailFrom;
        /// <summary>
        /// Identifiant de la liste de Baal pour les supports anti daté
        /// </summary>
        private int _baalListId = -1;
        /// <summary>
        /// List of media
        /// </summary>
        private string _mediaItemsList = "";
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public GebConfig(IDataSource source) {
            try {
                DataAccess.GebConfigDataAccess.Load(source,this);
                InitFromBaal();
            }
            catch(System.Exception e) {
                throw (e);
            }
        }
        #endregion

        #region Accesseurs
        /// <summary>
        /// Obtient ou défini le chemin du fichier excel
        /// </summary>
        public string ExcelPath {
            get { return _excelPath; }
            set { _excelPath = value; }
        }

        /// <summary>
        /// Obtient ou défini le serveur des mails de résultats
        /// </summary>
        public string CustomerMailServer {
            get { return _customerMailServer; }
            set { _customerMailServer = value; }
        }

        /// <summary>
        /// Obtient ou défini le port du serveur des mails des résultats
        /// </summary>
        public int CustomerMailPort {
            get { return _customerMailPort; }
            set { _customerMailPort = value; }
        }

        /// <summary>
        /// Obtient ou défini le mail d'envoi des résultats 
        /// </summary>
        public string CustomerMailFrom {
            get { return _customerMailFrom; }
            set { _customerMailFrom = value; }
        }

        /// <summary>
        /// Obtient ou défini l'identifiant de la liste baal pour les supports anti daté
        /// </summary>
        public int BaalListId {
            get { return _baalListId; }
            set { _baalListId = value; }
        }

        /// <summary>
        /// Obtient ou défini les identifiants des supports anti daté
        /// </summary>
        public string MediaItemsList {
            get { return _mediaItemsList; }
            set { _mediaItemsList = value; }
        }
        #endregion

        #region Private method
        /// <summary>
        /// Init list from baal
        /// </summary>
        /// <param name="idList">Baal Id List</param>
        /// <param name="levels">Levels to load</param>
        private void InitFromBaal() {
            try {
                Liste list = Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId(_baalListId);

                ArrayList levels = (ArrayList)list.Levels;
                if(levels.Count == 0)
                    throw (new GebExceptions.GebConfigException("La liste (" + _baalListId.ToString() + ") ne contient pas de support"));

                // Listes des supports
                _mediaItemsList = list.GetLevelIds(Baal.ExtractList.Constantes.Levels.media);
            }
            catch(System.Exception err) {
                throw (new GebExceptions.GebConfigException("Impossible de charger la liste de supports anti daté de Baal",err));
            }
        }
        #endregion

    }
}
