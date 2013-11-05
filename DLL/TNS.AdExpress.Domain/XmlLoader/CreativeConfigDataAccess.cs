#region Informations
/*
 * Author: G. Ragneau
 * Creation Date: 09/04/2008
 * Modifications:
 *      Author - Date - Description
 * 
 * 
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain.XmlLoader
{
    /// <summary>
    /// Load all the configuration parameter for creatives pathes
    /// </summary>
    public class CreativeConfigDataAccess
    {
        /// <summary>
        /// Load creatives pathes config
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        public static void LoadPathes(IDataSource dataSource)
        {

            #region Variables
            XmlTextReader reader = null;
            string id;
            string value;
            #endregion

            try
            {

                reader = (XmlTextReader)dataSource.GetSource();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {

                        switch (reader.LocalName)
                        {
                            case "VirtualPath":
                            case "Streaming":
                            case "Network":
                                id = reader.GetAttribute("id");
                                value = reader.GetAttribute("value");
                                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(value))
                                    throw new XmlException("Invalid parameter");
                                switch (id)
                                {
                                    case "CREA_ADNETTRACK":
                                        CreationServerPathes.CREA_ADNETTRACK = value;
                                        break;
                                    case "CREA_EVALIANT_MOBILE":
                                        CreationServerPathes.CREA_EVALIANT_MOBILE = value;
                                        break;
                                    case "IMAGES":
                                        CreationServerPathes.IMAGES = value;
                                        break;
                                    case "IMAGES_MD":
                                        CreationServerPathes.IMAGES_MD = value;
                                        break;
                                    case "IMAGES_OUTDOOR":
                                        CreationServerPathes.IMAGES_OUTDOOR = value;
                                        break;
                                    case "IMAGES_INSTORE":
                                        CreationServerPathes.IMAGES_INSTORE = value;
                                        break;
                                    case "DOWNLOAD_RADIO_SERVER":
                                        CreationServerPathes.DOWNLOAD_RADIO_SERVER = value;
                                        break;
                                    case "DOWNLOAD_CREATIVES_RADIO_SERVER":
                                        CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER = value;
                                        break;
                                    case "DOWNLOAD_TV_SERVER":
                                        CreationServerPathes.DOWNLOAD_TV_SERVER = value;
                                        break;
                                    case "DOWNLOAD_PAN_EURO":
                                        CreationServerPathes.DOWNLOAD_PAN_EURO = value;
                                        break;
                                    case "LOGO_PRESS":
                                        CreationServerPathes.LOGO_PRESS = value;
                                        break;
                                    case "MEDIA_UPDATE":
                                        ModuleInfosNews.MEDIA_UPDATE = value;
                                        break;
                                    case "READ_REAL_RADIO_SERVER":
                                        CreationServerPathes.READ_REAL_RADIO_SERVER = value;
                                        break;
                                    case "READ_REAL_CREATIVES_RADIO_SERVER":
                                        CreationServerPathes.READ_REAL_CREATIVES_RADIO_SERVER = value;
                                        break;
                                    case "READ_WM_RADIO_SERVER":
                                        CreationServerPathes.READ_WM_RADIO_SERVER = value;
                                        break;
                                    case "READ_WM_CREATIVES_RADIO_SERVER":
                                        CreationServerPathes.READ_WM_CREATIVES_RADIO_SERVER = value;
                                        break;
                                    case "READ_WM_TV_SERVER":
                                        CreationServerPathes.READ_WM_TV_SERVER = value;
                                        break;
                                    case "READ_WM_PAN_EURO_SERVER":
                                        CreationServerPathes.READ_WM_PAN_EURO_SERVER = value;
                                        break;
                                    case "READ_REAL_TV_SERVER":
                                        CreationServerPathes.READ_REAL_TV_SERVER = value;
                                        break;
                                    case "LOCAL_PATH_RADIO":
                                        CreationServerPathes.LOCAL_PATH_RADIO = value;
                                        break;
                                    case "LOCAL_PATH_CREATIVES_RADIO":
                                        CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO = value;
                                        break;
                                    case "LOCAL_PATH_VIDEO":
                                        CreationServerPathes.LOCAL_PATH_VIDEO = value;
                                        break;
                                    case "LOCAL_PATH_PAN_EURO":
                                        CreationServerPathes.LOCAL_PATH_PAN_EURO = value;
                                        break;
                                    case "LOCAL_PATH_IMAGE":
                                        CreationServerPathes.LOCAL_PATH_IMAGE = value;
                                        break;
                                    case "LOCAL_PATH_MD_IMAGE":
                                        CreationServerPathes.LOCAL_PATH_MD_IMAGE = value;
                                        break;
                                    case "LOCAL_PATH_OUTDOOR":
                                        CreationServerPathes.LOCAL_PATH_OUTDOOR = value;
                                        break;
                                    case "LOCAL_PATH_INSTORE":
                                        CreationServerPathes.LOCAL_PATH_INSTORE = value;
                                        break;
                                    case "LOCAL_PATH_LOGO_PRESS":
                                        CreationServerPathes.LOCAL_PATH_LOGO_PRESS = value;
                                        break;
                                    case "LOCAL_PATH_ADEXNEWS":
                                        ModuleInfosNews.LOCAL_PATH_ADEXNEWS = value;
                                        break;
                                    case "LOCAL_PATH_ADEXREPORT":
                                        ModuleInfosNews.LOCAL_PATH_ADEXREPORT = value;
                                        break;
                                    case "LOCAL_PATH_ADEXREVIEW":
                                        ModuleInfosNews.LOCAL_PATH_ADEXREVIEW = value;
                                        break;
                                    case "LOCAL_PATH_NOUVEAUTES":
                                        ModuleInfosNews.LOCAL_PATH_NOUVEAUTES = value;
                                        break;
                                    case "LOCAL_PATH_MEDIA_UPDATE":
                                        ModuleInfosNews.LOCAL_PATH_MEDIA_UPDATE = value;
                                        break;
                                    case "IMAGES_VP":
                                        CreationServerPathes.IMAGES_VP = value;
                                        break;
                                    case "LOCAL_PATH_VP":
                                        CreationServerPathes.LOCAL_PATH_VP = value;
                                        break;                                    
                                    case "IMAGES_EDITORIAL":
                                        CreationServerPathes.IMAGES_EDITORIAL = value;
                                        break;
                                    case "LOCAL_PATH_EDITORIAL" :
                                        CreationServerPathes.LOCAL_PATH_EDITORIAL = value;
                                        break;
                                    case "LOCAL_PATH_INTERNET":
                                        CreationServerPathes.LOCAL_PATH_INTERNET = value;
                                        break;
                                        break;
                                    case "IMAGES_PRESS_COVER":
                                        CreationServerPathes.IMAGES_PRESS_COVER = value;
                                        break;
                                    case "LOCAL_PATH_IMAGES_COVER":
                                        CreationServerPathes.LOCAL_PATH_IMAGES_COVER = value;
                                        break;
                                    case "LOCAL_PATH_IMAGES_PRESS_CLIPPING":
                                        CreationServerPathes.LOCAL_PATH_IMAGES_PRESS_CLIPPING = value;
                                        break;
                                    case "IMAGES_PRESS_CLIPPING":
                                        CreationServerPathes.IMAGES_PRESS_CLIPPING = value;
                                        break;
                                    case "IMAGES_ROLEX":
                                        CreationServerPathes.IMAGES_ROLEX = value;
                                        break;
                                    case "LOCAL_PATH_ROLEX":
                                        CreationServerPathes.LOCAL_PATH_ROLEX = value;
                                        break;
                                    case "IMAGES_PSA" :
                                        CreationServerPathes.IMAGES_PSA = value;
                                        break;
                                    case "IMAGES_PSA_MR" :
                                        CreationServerPathes.IMAGES_PSA_MR = value;
                                         break;
                                    case "VIDEOS_PSA":
                                         CreationServerPathes.VIDEOS_PSA = value;
                                         break;
                                    case "RADIOS_PSA":
                                         CreationServerPathes.RADIOS_PSA = value;
                                         break;
                                    case "BANNERS_PSA":
                                         CreationServerPathes.BANNERS_PSA = value;
                                         break;
                                    case "LOCAL_PATH_IMAGES_PSA_MR":
                                         CreationServerPathes.LOCAL_PATH_IMAGES_PSA_MR = value;
                                         break;
                                    case "LOCAL_PATH_BANNERS_PSA":
                                         CreationServerPathes.LOCAL_PATH_BANNERS_PSA = value;
                                         break;
                                    case "LOCAL_PATH_EVALIANT":
                                         CreationServerPathes.LOCAL_PATH_EVALIANT = value;
                                         break;  
                                }
                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch (System.Exception err)
            {

                #region Close the file
                if (dataSource.GetSource() != null) dataSource.Close();
                #endregion

                throw (new DataBaseException(" Error : ", err));
            }
            #endregion

            dataSource.Close();
        }
    }
}
