using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.AdExpress.Domain.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Navigation;

namespace TNS.AdExpress.Domain.XmlLoader
{
    public class RightMenuLinksXL
    {
        /// <summary>
        /// Load RightMenuLinks.xml configuration file
        /// </summary>
        /// <param name="source"></param>
        public static void LoadRightMenuLinks(IDataSource source, List<RightMenuLinksItem> rightMenuLinksList)
        {
            bool giveLanguage = false;
            bool giveSession = false;
            bool displayInPopUp = false;
            string target = "", cssIconName = "", javascriptFunctionName = "", width = "800", height = "600";
            XmlTextReader reader = null;
            try
            {
                reader = (XmlTextReader)source.GetSource();
                while(reader.Read())
                {
                    if(reader.NodeType == XmlNodeType.Element)
                    {
                        switch(reader.LocalName)
                        {
                            case "link":
                                giveLanguage = false;
                                giveSession = false;
                                displayInPopUp = false;
                                target = "";
                                cssIconName = "";
                                javascriptFunctionName = "";
                                width = "800";
                                height = "600";

                                if(reader.GetAttribute("language") != null)
                                    giveLanguage = bool.Parse(reader.GetAttribute("language"));

                                if(reader.GetAttribute("idSession") != null)
                                    giveSession = bool.Parse(reader.GetAttribute("idSession"));
                                
                                if(reader.GetAttribute("displayInPopUp") != null)
                                    displayInPopUp = bool.Parse(reader.GetAttribute("displayInPopUp"));

                                if(reader.GetAttribute("target") != null)
                                    target = reader.GetAttribute("target");

                                if(reader.GetAttribute("cssIconName") != null)
                                    cssIconName = reader.GetAttribute("cssIconName");
                                
                                if(reader.GetAttribute("javascriptFunctionName") != null)
                                    javascriptFunctionName = reader.GetAttribute("javascriptFunctionName");
                                
                                if(reader.GetAttribute("width") != null)
                                    width = reader.GetAttribute("width");

                                if(reader.GetAttribute("height") != null)
                                    height = reader.GetAttribute("height");

                                rightMenuLinksList.Add(new RightMenuLinksItem(
                                    Int64.Parse(reader.GetAttribute("idWebText")), 
                                    reader.GetAttribute("href"), 
                                    giveLanguage, 
                                    giveSession, 
                                    displayInPopUp, 
                                    target,
                                    cssIconName, javascriptFunctionName, width, height));
                                break;
                        }
                    }
                }
                
                #region Close the file
                if(reader != null) reader.Close();
                #endregion
            }

            #region Errors management
            catch(System.Exception e)
            {
                #region Close the file
                if(reader != null) reader.Close();
                #endregion

                throw (new RightMenuLinksXLException("Impossible to load rightmenulinks XML file", e));
            }
            #endregion

        }
    }
}
