using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TNS.FrameWork.DB.Common;
using KMI.P3.Domain.Exceptions;
using KMI.P3.Domain.Web.Navigation;
using System.Xml;

namespace KMI.P3.Domain.XmlLoader
{
    ///<summary>
	/// Class used to load the data relating to the page headings
	/// </summary>
	///  <stereotype>utility</stereotype>
    public class HeaderXL {
     #region Load XML file

        /// <summary>
        /// static Method which load from a file XML the Hashtable containing Headers 
        /// </summary>
        /// <param name="pathXMLFile">Xml file path</param>	
        /// Hashtable instanciée qui va être renseigné avec des objets Header contenant les spécifications de chaque entêtes du site
        /// <param name="headers">Instanciate Hashtable which will be indicated the specifications of each Headers objects</param>
        public static Dictionary<string,WebHeader> Load(IDataSource source) {
            bool giveLanguage=false;
            bool giveSession=false;
            string target="";
            bool displayInPopUp = false;
            XmlTextReader reader=null;
            Dictionary<string,WebHeader> headers=new Dictionary<string,WebHeader>();
            try {
                reader=(XmlTextReader)source.GetSource();
                string name="";
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {
                        switch(reader.LocalName) {
                            case "header":
                                if(reader.GetAttribute("name")!=null) {
                                    name = reader.GetAttribute("name");
                                    headers.Add((name=reader.GetAttribute("name")),new WebHeader());
                                }
                                break;
                            case "menuItem":
                                giveLanguage=false;
                                giveSession=false;
                                target="";
                                displayInPopUp = false;
                                if(name.CompareTo("")!=0 && reader.GetAttribute("name")!=null && reader.GetAttribute("idWebText")!=null && reader.GetAttribute("href")!=null) {
                                    if(reader.GetAttribute("language")!=null) {
                                        giveLanguage=bool.Parse(reader.GetAttribute("language"));
                                    }
                                    if(reader.GetAttribute("idSession")!=null) {
                                        giveSession=bool.Parse(reader.GetAttribute("idSession"));
                                    }
                                    if(reader.GetAttribute("target")!=null) {
                                        target=reader.GetAttribute("target");
                                    }
                                    if(reader.GetAttribute("displayInPopUp") != null) {
                                        displayInPopUp = bool.Parse(reader.GetAttribute("displayInPopUp"));
                                    }
                                    headers[name].MenuItems.Add(new WebHeaderMenuItem(Int64.Parse(reader.GetAttribute("idWebText")),reader.GetAttribute("href"),giveLanguage,giveSession,target,displayInPopUp));
                                }
                                break;
                        }
                    }
                }
                name="";
                #region Close the file
                if(reader!=null) reader.Close();
                #endregion
            }

            #region Errors management
            catch(System.Exception e) {
                #region Close the file
                if(reader!=null) reader.Close();
                #endregion
                throw (new HeaderException("Impossible to load header XML file",e));
            }
            #endregion
            return (headers);
        }
        #endregion
    }
}
