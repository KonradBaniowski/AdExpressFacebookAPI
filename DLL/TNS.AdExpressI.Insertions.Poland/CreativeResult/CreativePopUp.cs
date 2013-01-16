using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using CstWeb = TNS.AdExpress.Constantes.Web;
namespace TNS.AdExpressI.Insertions.Poland.CreativeResult
{
    public class CreativePopUp : Insertions.CreativeResult.CreativePopUp
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idSlogan">Slogan identifier</param>
        /// <param name="file">File</param>
        /// <param name="webSession">WebSession</param>
        /// <param name="title">Title</param>
        /// <param name="hasCreationReadRights">True if has Creation Read Rights</param>
        /// <param name="hasCreationDownloadRights">true if has Creation Download Rights</param>
        public CreativePopUp(Page popUp, Vehicles.names vehicle, string idSlogan, string file, WebSession webSession,
                             string title, bool hasCreationReadRights, bool hasCreationDownloadRights) :
                                 base(
                                 popUp, vehicle, idSlogan, file, webSession, title, hasCreationReadRights,
                                 hasCreationDownloadRights)
        {
        }

        #endregion

      

        /// <summary>
        /// Get Creative Pathes
        /// </summary>
        /// <param name="realFormatFound">True if real Format Found</param>
        /// <param name="windowsFormatFound">True if windows Forma tFound</param>       
        protected override void GetCreativePathes(ref bool realFormatFound, ref bool windowsFormatFound)
        {
               Func<string, string, string,bool> isCreativeExists = (s, e,v) =>
                      File.Exists(string.Format("{0}\\{3}\\{1}\\{2}.{3}"
                     , s, v.Substring(0, 5), v, e));

           Func<string, string, string,string> getCreativePath = (s, e,v) => 
            string.Format("{0}/{3}/{1}/{2}.{3}",s, v.Substring(0, 5), v, e);

            switch (_vehicle)
            {
                case Vehicles.names.radio:

                    //Vérification de l'existence du fichier real
                    if (isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO, RA_EXTENSION,_idSlogan))
                        realFormatFound = true;

                    //Vérification de l'existence du fichier wm
                    if (isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO, WMA_EXTENSION, _idSlogan))
                        windowsFormatFound = true;


                    //Construction des chemins real et wm					
                    if (_hasCreationReadRights)
                    {
                        //Fichiers en lectures (streaming)
                        _pathDownloadingRealFile =
                            _pathReadingRealFile =
                            getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER, RA_EXTENSION, _idSlogan);
                         
                        _pathDownloadingWindowsFile =
                            _pathReadingWindowsFile = getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER,
                            WMA_EXTENSION, _idSlogan);
                         
                    }
                    break;

                case Vehicles.names.tv:

                    if (isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO, RM_EXTENSION, _file))
                        realFormatFound = true;

                    if (isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO, WMV_EXTENSION, _file))
                        windowsFormatFound = true;

                    //Construction des chemins real et wm	

                    if (_hasCreationReadRights)
                    {
                        //Fichiers à télécharger 
                        _pathReadingRealFile =  _pathDownloadingRealFile =
                            getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER, RM_EXTENSION, _file);

                        _pathReadingWindowsFile = _pathDownloadingWindowsFile =
                            getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER, WMV_EXTENSION, _file);
                         
                    }
                    break;


                default:
                    _webSession.Source.Close();
                    _popUp.Response.Redirect(string.Format("/Public/Message.aspx?msgTxt={0}&title={1}",
                                                           GestionWeb.GetWebWord(890, _webSession.SiteLanguage),
                                                           GestionWeb.GetWebWord(887, _webSession.SiteLanguage)));
                    break;
            }
        }

        #region IsRadioFileExists

        /// <summary>
        /// Is Radio File Exists
        /// </summary>
        /// <param name="realFormatFound">True if real Format Found</param>
        /// <param name="windowsFormatFound">True if windows Format Found</param>
        protected override void IsRadioFileExists(ref bool realFormatFound, ref bool windowsFormatFound)
        {
            Func<string, bool> isCreativeExists =
                e => _idSlogan != null && File.Exists(string.Format("{0}\\{3}\\{1}\\{2}.{3}",
                                                                    CstWeb.CreationServerPathes
                                                                          .LOCAL_PATH_CREATIVES_RADIO,
                                                                    _idSlogan.Substring(0, 5),
                                                                    _idSlogan, e));

            //Vérification de l'existence du fichier real
            if (isCreativeExists(RA_EXTENSION))
                _isNewRealAudioFilePath = realFormatFound = true;           
            

            //Vérification de l'existence du fichier wm
            if (isCreativeExists(WMA_EXTENSION))
                _isNewWindowsAudioFilePath = windowsFormatFound = true;

        }

        #endregion


    }
}
