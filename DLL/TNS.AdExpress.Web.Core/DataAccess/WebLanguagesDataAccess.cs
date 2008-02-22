#region Informations
// Author: G. Facon
// Creation Date: 21/02/2008
// Modifications: 
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Core.DataAccess {

    
    /// <summary>
    /// Load all the configuration parameter for languages management
    /// </summary>
    public class WebLanguagesDataAccess {

        #region Default Language
        /// <summary>
        /// Load default Language
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <returns>Return the default language of the application</returns>
        internal static int LoadDefaultLanguage(IDataSource dataSource) {

            #region Variables
            XmlTextReader reader=null;
            int defaultLanguageId=-1;
            #endregion

            try {
                reader=(XmlTextReader)dataSource.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {
                        switch(reader.LocalName) {
                            case "languages":
                                if(reader.GetAttribute("default")==null) throw (new XmlNullValueException("The default language value is null"));
                                if(reader.GetAttribute("default").Length==0) throw (new XmlException("The default language is empty"));
                                defaultLanguageId=int.Parse(reader.GetAttribute("default"));
                                break;
                        }
                    }
                }
            }
            #region Traitement des erreurs
            catch(System.Exception err) {
                #region Fermeture du fichier
                if(dataSource.GetSource()!=null) dataSource.Close();
                #endregion

                throw (new WebLanguagesDataAccessException(" Error : ",err));
            }
            #endregion

            return (defaultLanguageId);
        }
        #endregion

        #region Web Languages
        /// <summary>
        /// Chargement de la langue par defaut de Cyberpub
        /// </summary>
        /// <param name="dataSource">Source de données</param>
        /// <returns>Langue par defaut de l'application</returns>
        /// <summary>
        internal static Dictionary<Int64,WebLanguage> LoadLanguages(IDataSource dataSource) {

            #region Variables
            Dictionary<Int64,WebLanguage> languages=new Dictionary<Int64,WebLanguage>();
            XmlTextReader reader=null;
            Int64 id;
            string name="";
            string imageSourceText="";
            string localization;
            Int64 classificationLanguageId;
            string charset="";
            #endregion

            try {
                reader=(XmlTextReader)dataSource.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {
                        name="";
                        imageSourceText="";
                        charset="";
                        switch(reader.LocalName) {
                            case "language":
                                if(reader.GetAttribute("id")==null || reader.GetAttribute("id").Length==0) throw (new InvalidXmlValueException("Invalid id parameter"));
                                id=Int64.Parse(reader.GetAttribute("id"));
                                if(reader.GetAttribute("localization")==null || reader.GetAttribute("localization").Length==0) throw (new XmlNullValueException("Invalid localization parameter"));
                                localization=reader.GetAttribute("localization");
                                if(reader.GetAttribute("name")!=null) name=reader.GetAttribute("name");
                                if(reader.GetAttribute("charset")!=null) charset=reader.GetAttribute("charset");
                                if(reader.GetAttribute("imageSourceText")!=null) imageSourceText=reader.GetAttribute("imageSourceText");
                                if(reader.GetAttribute("classificationLanguageId")!=null && reader.GetAttribute("classificationLanguageId").Length>0)
                                    classificationLanguageId=Int64.Parse(reader.GetAttribute("classificationLanguageId"));
                                else
                                    classificationLanguageId=id;
                                languages.Add(id,new WebLanguage(id,name,imageSourceText,localization,classificationLanguageId,charset));
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

                throw (new WebLanguagesDataAccessException(" Error : ",err));
            }
            #endregion

            dataSource.Close();
            return (languages);
        }
        #endregion
    }
}
