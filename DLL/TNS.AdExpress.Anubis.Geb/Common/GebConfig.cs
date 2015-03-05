#region Informations
// Auteur : B.Masson
// Date de cr�ation : 21/04/2006
// Date de modification :
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
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
        /// Serveur de mail d'envoi des r�sultats
        /// </summary>
        private string _customerMailServer;
        /// <summary>
        /// Port du serveur de mail d'envoi des r�sultats
        /// </summary>
        private int _customerMailPort=25;
        /// <summary>
        /// Mail d'envoi des r�sultats
        /// </summary>
        private string _customerMailFrom;
        /// <summary>
        /// Identifiant de la liste de Baal pour les supports anti dat�
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
        /// Obtient ou d�fini le chemin du fichier excel
        /// </summary>
        public string ExcelPath {
            get { return _excelPath; }
            set { _excelPath = value; }
        }

        /// <summary>
        /// Obtient ou d�fini le serveur des mails de r�sultats
        /// </summary>
        public string CustomerMailServer {
            get { return _customerMailServer; }
            set { _customerMailServer = value; }
        }

        /// <summary>
        /// Obtient ou d�fini le port du serveur des mails des r�sultats
        /// </summary>
        public int CustomerMailPort {
            get { return _customerMailPort; }
            set { _customerMailPort = value; }
        }

        /// <summary>
        /// Obtient ou d�fini le mail d'envoi des r�sultats 
        /// </summary>
        public string CustomerMailFrom {
            get { return _customerMailFrom; }
            set { _customerMailFrom = value; }
        }

        /// <summary>
        /// Obtient ou d�fini l'identifiant de la liste baal pour les supports anti dat�
        /// </summary>
        public int BaalListId {
            get { return _baalListId; }
            set { _baalListId = value; }
        }

        /// <summary>
        /// Obtient ou d�fini les identifiants des supports anti dat�
        /// </summary>
        public string MediaItemsList {
            get { return _mediaItemsList; }
            set { _mediaItemsList = value; }
        }
        /// <summary>
        /// Not CopyrightMedia Ids
        /// </summary>
        public List<long> NotCopyrightMediaIds { get {return new List<long> { 15940, 9178, 9480, 1596, 7011, 24328, 1320, 4171, 4172, 15869, 1576, 1509, 5156, 7906, 8143, 9992, 1648, 1465, 6340, 1994, 9364, 9710, 1374, 5832, 9658, 6780, 9103, 6337, 9109, 7472, 6918, 9497, 5225, 7230, 9709, 7532, 1825, 1838, 5678, 5510, 1365, 4205, 1906, 7935, 1748, 6236, 1363, 6077, 6561, 1845, 4458, 5262, 9892, 4560, 24377, 6682, 24379, 9260, 8628, 18189, 7606, 5193, 7973, 2702, 1300, 7006, 8902, 8901, 5911, 13057, 1387, 8452, 7938, 1592, 1768, 1390, 1395, 9570, 5258, 9173 }; } 
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
                throw (new GebExceptions.GebConfigException("Impossible de charger la liste de supports anti dat� de Baal",err));
            }
        }
        #endregion

    }
}
