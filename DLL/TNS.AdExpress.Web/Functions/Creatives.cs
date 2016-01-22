#region Information
/*
 * Author : G Ragneau
 * Created on : 19/03/2008
 * Modification :
 *      Y R'kaina - 24/09/2008 - Add Creatives class used in AdExpress 3.0
 * */
#endregion

using System;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
//using TNS.AdExpress.Web.Core.ClassificationList;

namespace TNS.AdExpress.Web.Functions {
    /// <summary>
    /// Creatives
    /// </summary>
    public class Creatives {
        private static Object _mutex = new object();
        /// <summary>
        /// List of media using kiosque dateto access visuals
        /// </summary>
        private static string[] _mediaList = null;

        /// <summary>
        /// Compute visual file path depending on media Id and access mode (web or local network)
        /// </summary>
        /// <param name="mediaId">Media Identifiant</param>
        /// <param name="dateKiosque">Kiosque date as YYYYMMMDD</param>
        /// <param name="dateCover">Cover date as YYYYMMMDD</param>
        /// <param name="filename">File name (XXX...XXX.jpg) or empty string</param>
        /// <param name="webpath">Specify access mode (web or local network)</param>
        /// <param name="smallSize">Specify if imagetes are required</param>
        /// <returns>Entire File Path, or root path if file name is not specified</returns>
        public static string GetCreativePath(Int64 mediaId, Int64 dateKiosque, Int64 dateCover, string filename, bool webpath, bool smallSize,bool hasCopyRight) {
            string path = CreationServerPathes.LOCAL_PATH_IMAGE;
            char separator = '\\';
            if (webpath) {
                separator = '/';
                path = string.Format("{0}/", CreationServerPathes.IMAGES);
            }
            lock (_mutex) {
                if (_mediaList == null) {
                    try {
                       
                        MediaItemsList mediaItemsList = Media.GetItemsList(AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID);
                        _mediaList = mediaItemsList.MediaList.Split(',');
                    }
                    catch { }
                }
            }
            long date = dateCover;
            if (Array.IndexOf(_mediaList, mediaId.ToString()) > -1)                
                date = dateKiosque;
            bool isbefore2015 = Functions.Rights.ParutionDateBefore2015(date.ToString());
            return string.Format("{0}{1}{2}{1}{3}{1}{4}{5}{6}", path, separator, mediaId, date.ToString()
                                 , (smallSize) ? string.Format("imagette{0}", separator) : string.Empty
                                 , (!hasCopyRight && !isbefore2015) ? string.Format("blur{0}", separator) : string.Empty
                                 , filename);
        }
    }
}
