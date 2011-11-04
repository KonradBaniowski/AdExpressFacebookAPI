#region Information
//  Author : Y Rkaina && D. Mussuma
//  Creation  date: 15/07/2009
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using System.Xml.XPath;
using System.IO;


namespace TNS.AdExpress.VP.Loader.Domain.XmlLoader {
    public class CommonAppDataXL {

        /// <summary>
        /// Common Application Data file description
        /// </summary>
        /// <param name="pathFileSave">source</param>
        /// <returns>layers list</returns>
        public static FileInfo Load(string pathFileSave)
        {
            bool fileExist = false;
            try
            {
                fileExist = File.Exists(pathFileSave);
                if (File.Exists(pathFileSave)) {
                    XPathDocument xPathDoc = new XPathDocument(new FileStream(pathFileSave, FileMode.Open));
                    XPathNavigator xPathNav = xPathDoc.CreateNavigator();
                    string xPathExp = "//Parameters/loadFile[@path]";

                    XPathNodeIterator node = xPathNav.Select(xPathNav.Compile(xPathExp));

                    if (node.MoveNext() && !string.IsNullOrEmpty(node.Current.GetAttribute("path", "")))
                        return new FileInfo(node.Current.GetAttribute("path", ""));
                    return null;
                }
                return null;
            }
            catch (System.Exception err)
            {
                if (fileExist) File.Delete(pathFileSave);
                return null;
            }
            
        }

        /// <summary>
        /// Common Application Data file description
        /// </summary>
        /// <param name="pathFileSave">source</param>
        /// <returns>layers list</returns>
        public static void Create(string pathFileSave, string path) {
            try {
                XmlWriter writer = XmlWriter.Create(pathFileSave); 

                writer.WriteStartDocument(); 
                    writer.WriteStartElement("Parameters"); 
                        writer.WriteStartElement("loadFile");
                            writer.WriteAttributeString("path", path); 
                        writer.WriteEndElement(); 
                    writer.WriteEndElement(); 
                writer.WriteEndDocument(); 
                writer.Flush();
            }
            catch (System.Exception err) {
                throw (new Exception(" Error : ", err));
            }
        }

        /// <summary>
        /// Common Application Data file description
        /// </summary>
        /// <param name="pathFileSave">source</param>
        /// <returns>layers list</returns>
        public static void Save(string pathFileSave, FileInfo fileLoad) {
            try {
                

                XmlDocument document = new XmlDocument();

                XmlReader reader = new XmlTextReader(new FileStream(pathFileSave, FileMode.Open));

                document.Load(reader);
                XPathNavigator navigator = document.CreateNavigator();

                navigator.MoveToChild("Parameters", String.Empty);
                navigator.MoveToChild("loadFile", String.Empty);
                navigator.MoveToAttribute("path", String.Empty);
                navigator.SetValue(fileLoad.FullName);

                navigator.MoveToRoot();

                string xml = navigator.OuterXml;
                reader.Close();


                XmlWriter writer = XmlWriter.Create(new FileStream(pathFileSave, FileMode.Open));
                writer.WriteRaw(xml);
                writer.Close();

            }
            catch (System.Exception err) {
                throw (new Exception(" Error : ", err));
            }
        }
    }
}
