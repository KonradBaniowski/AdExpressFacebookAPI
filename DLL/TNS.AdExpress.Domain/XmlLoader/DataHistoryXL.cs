using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.XmlLoader
{
    public class DataHistoryXL
    {
        #region DataHistory
        /// <summary>
        /// Load Data History
        /// </summary>
        /// <param name="source">Data Source</param>
        /// <returns>Data History number</returns>
        public static int LoadDataHistory(IDataSource source)
        {

            XmlTextReader reader = null;
            int datahistory =0;
            try
            {
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case "dataNumberOfYear":
                                if (reader.GetAttribute("value") == null || reader.GetAttribute("value").Length == 0) throw (new InvalidXmlValueException("Invalid value parameter"));                                                                
                                datahistory = int.Parse( reader.GetAttribute("value"));
                               
                                break;
                        }
                    }
                }

                #region Fermeture du fichier
                if (source.GetSource() != null) source.Close();
                #endregion

            }
            #region Traitement des erreurs
            catch (System.Exception e)
            {

                #region Fermeture du fichier
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new XmlException("Impossible load Data History ", e));
            }
            #endregion

            return (datahistory);
        }
        #endregion
    }
}
