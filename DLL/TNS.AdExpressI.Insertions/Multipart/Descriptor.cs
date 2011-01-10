using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using TNS.AdExpressI.Insertions.Exceptions;

namespace TNS.InternetClassification.Web.Domain.MultiPart {
    /// <summary>
    /// Mutipart Descriptor Class (defined a multipart)
    /// </summary>
    public class Descriptor {

        #region Variables
        /// <summary>
        /// Panel List
        /// </summary>
        protected List<Panel> _panelList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address">Address from the xml to load</param>
        public Descriptor(string address) {
            _panelList = this.Load(address);
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Panel List
        /// </summary>
        public List<Panel> PanelList{
            get { return _panelList; }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Get Maximum Height
        /// </summary>
        /// <returns>Maximum Height</returns>
        public int GetMaxHeight() {
            if (_panelList != null) {
                var max = (from elem in _panelList
                           select elem.GetMaxHeight()).Max();

                return max;
            }
            return 0;
        }

        /// <summary>
        /// Get Maximum Width
        /// </summary>
        /// <returns>Maximum Width</returns>
        public int GetMaxWidth() {
            if (_panelList != null) {
                var max = (from elem in _panelList
                           select elem.GetMaxWidth()).Max();

                return max;
            }
            return 0;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Load a descriptor
        /// </summary>
        /// <param name="address">Address from the xml to load</param>
        private List<Panel> Load(string address) {
            List<Panel> panelList = null;
            Stream str = null;
            XmlDocument xmlDoc = null;
            try {
                WebClient client = new WebClient();
                str = client.OpenRead(address);
                xmlDoc = new XmlDocument();
                xmlDoc.Load(client.OpenRead(address));
                panelList = LoadPanelList(xmlDoc);
            }
            catch (Exception e) {
                throw new DescriptorException("impossible to load this URI : '" + address + "'", e);
            }
            finally {
                if (str != null) {
                    str.Close();
                    str.Dispose();
                    str = null;
                }
            }
            return panelList;

        }

        /// <summary>
        /// Load Panel List
        /// </summary>
        /// <param name="reader">Xml Reader</param>
        /// <returns>Panel List</returns>
        private List<Panel> LoadPanelList(XmlDocument xmlDoc) {
            List<Panel> panelList = new List<Panel>();
            foreach (XmlElement xElement in xmlDoc.DocumentElement) {
                if (xElement.Name.ToLower() == "panel") {
                    string name = xElement.Attributes["name"].Value;
                    string src = xElement.Attributes["src"].Value;
                    int width = Int32.Parse(xElement.Attributes["width"].Value);
                    int height = Int32.Parse(xElement.Attributes["height"].Value);
                    if (xElement.HasChildNodes) {
                        panelList.Add(new Panel(name, src, width, height, LoadNodePanelList(xElement.ChildNodes)));
                    }
                    else {
                        panelList.Add(new Panel(name, src, width, height));
                    }
                }
                else if (xElement.Name.ToLower() == "multipart" && xElement.HasChildNodes) {
                    panelList.AddRange(LoadNodePanelList(xElement.ChildNodes));
                }
            }
            
            return panelList;
        }

        /// <summary>
        /// Load Panel List
        /// </summary>
        /// <param name="reader">Xml Reader</param>
        /// <returns>Panel List</returns>
        private List<Panel> LoadNodePanelList(XmlNodeList xmlNodeList) {
            List<Panel> panelList = new List<Panel>();

            List<string> result = new List<string>();

            foreach (XmlNode xNode in xmlNodeList) {
                if (xNode.Name.ToLower() == "panel") {
                    string name = xNode.Attributes["name"].Value;
                    string src = xNode.Attributes["src"].Value;
                    int width = Int32.Parse(xNode.Attributes["width"].Value);
                    int height = Int32.Parse(xNode.Attributes["height"].Value);
                    if (xNode.HasChildNodes) {
                        panelList.Add(new Panel(name, src, width, height, LoadNodePanelList(xNode.ChildNodes)));
                    }
                    else {
                        panelList.Add(new Panel(name, src, width, height));
                    }
                }
            }
            return panelList;
        }
        #endregion



    }
}
