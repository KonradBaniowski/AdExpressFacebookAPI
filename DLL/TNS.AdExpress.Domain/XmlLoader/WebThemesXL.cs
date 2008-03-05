#region Informations
// Author: G. Facon
// Creation Date: 21/02/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Core;
using TNS.AdExpress.Domain.Exceptions;


namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Load all the configuration parameter for theme management
    /// </summary>
    public class WebThemesXL{
        /// <summary>
        /// Load website themes
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        /// <returns>Website themes</returns>
        /// <summary>
        /// <remarks>The key is the site language</remarks>
        internal static Dictionary<Int64,WebTheme> LoadThemes(IDataSource dataSource) {

            #region Variables
            Dictionary<Int64,WebTheme> themes=new Dictionary<Int64,WebTheme>();
            XmlTextReader reader=null;
            int id;
            string name="";
            int siteLanguage;
            #endregion

            try {
                reader=(XmlTextReader)dataSource.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {

                        switch(reader.LocalName) {
                            case "theme":
                                if(reader.GetAttribute("id")==null || reader.GetAttribute("id").Length==0) throw (new InvalidXmlValueException("Invalid id parameter"));
                                id=int.Parse(reader.GetAttribute("id"));
                                if(reader.GetAttribute("name")==null || reader.GetAttribute("name").Length==0) throw (new XmlNullValueException("Invalid name parameter"));
                                name=reader.GetAttribute("name");
                                if(reader.GetAttribute("siteLanguage")==null || reader.GetAttribute("siteLanguage").Length==0) throw (new InvalidXmlValueException("Invalid siteLanguage parameter"));
                                siteLanguage=int.Parse(reader.GetAttribute("siteLanguage"));

                                themes.Add(siteLanguage,new WebTheme(id,name,siteLanguage));
                                break;
                        }
                    }
                }
            }
            #region Error Management
            catch(System.Exception err) {

                #region Close the file
                if(dataSource.GetSource()!=null) dataSource.Close();
                #endregion

                throw (new WebThemesXLException(" Error : ",err));
            }
            #endregion

            dataSource.Close();
            return (themes);
        }
    }
}
