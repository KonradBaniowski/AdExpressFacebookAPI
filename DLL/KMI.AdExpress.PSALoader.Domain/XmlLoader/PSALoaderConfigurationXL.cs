using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Xml;

namespace KMI.AdExpress.PSALoader.Domain.XmlLoader {
    /// <summary>
    /// Used to load PSA Loader configuration
    /// </summary>
    public class PSALoaderConfigurationXL {

        /// <summary>
        /// Load PSA Loader configuration
        /// </summary>
        /// <param name="dataSrc">Data source</param>
        /// <returns>Return PSALoaderConfiguration object which contains PSA Loader Configuration</returns>
        public static PSALoaderConfiguration Load(IDataSource source) {
            PSALoaderConfiguration cfg = null;
            try {
                XmlTextReader Reader = (XmlTextReader)source.GetSource();
                while (Reader.Read()) {
                    if (Reader.NodeType == XmlNodeType.Element) {
                        switch (Reader.LocalName) {
                            case "psaLoader":
                                cfg = new PSALoaderConfiguration(Reader.GetAttribute("serverName"),
                                                                Reader.GetAttribute("appPool"),
                                                                Reader.GetAttribute("userName"),
                                                                Reader.GetAttribute("password")
                                );
                                break;
                        }
                    }
                }
                Reader.Close();
                source.Close();
                if (cfg == null) throw (new XmlException("No PSA Loader informations have been configured"));
            }
            catch (System.Exception err) {
                throw (new ApplicationException("Impossible to load the data XML file for PSA Loader configuration", err));
            }
            return (cfg);
        }

    }
}
