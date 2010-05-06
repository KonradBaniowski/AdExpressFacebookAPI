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
using TNS.AdExpress.Bastet.Translation;
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Bastet.Exceptions;
using System.Globalization;

namespace TNS.AdExpress.Bastet.XmlLoader{
    /// <summary>
    /// Load all the configuration parameter for languages management
    /// </summary>
    public class WebLanguagesXL {

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

                throw (new TNS.AdExpress.Domain.Exceptions.WebLanguagesXLException(" Error : ",err));
            }
            #endregion

            return (defaultLanguageId);
        }
        #endregion

        #region Default DataLanguage
        /// <summary>
		/// Load default Data Language
		/// </summary>
		/// <param name="dataSource">Data source</param>
		/// <returns>Return the default data language of the application</returns>
		internal static int LoadDefaultDataLanguage(IDataSource dataSource) {

			#region Variables
			XmlTextReader reader = null;
			int defaultDataLanguageId = -1;
			#endregion

			try {
				reader = (XmlTextReader)dataSource.GetSource();
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						switch (reader.LocalName) {
							case "languages":
								if (reader.GetAttribute("defaultDataLanguage") == null) throw (new XmlNullValueException("The default data language value is null"));
								if (reader.GetAttribute("defaultDataLanguage").Length == 0) throw (new XmlException("The default data language is empty"));
								defaultDataLanguageId = int.Parse(reader.GetAttribute("defaultDataLanguage"));
								break;
						}
					}
				}
			}
			#region Traitement des erreurs
			catch (System.Exception err) {
				#region Fermeture du fichier
				if (dataSource.GetSource() != null) dataSource.Close();
				#endregion

				throw (new TNS.AdExpress.Domain.Exceptions.WebLanguagesXLException(" Error : ", err));
			}
			#endregion

			return (defaultDataLanguageId);
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
            int id;
            string name="";
            string imageSourceText = "";
            string localization;
            int classificationLanguageId;
            string charset="";
            string contentEncoding = "";
            string excelContentEncoding = "";
            string nlsSort = "";
			bool isUTF8 = false;
            BastetCultureInfo cInfo = null;
            BastetCultureInfo cInfoExcel = null;
            string formatName = string.Empty;
            string format = string.Empty;
            string excelFormat = string.Empty;
            string numberGroupSeparator = string.Empty;
            string numberDecimalSeparator = string.Empty;
            #endregion

            try {
                reader=(XmlTextReader)dataSource.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {
                        name = "";
                        imageSourceText = "";
                        charset = "";
                        contentEncoding = "";
                        nlsSort = "";
                        isUTF8 = false;
                        numberGroupSeparator = string.Empty;
                        numberDecimalSeparator = string.Empty;
                        switch (reader.LocalName) {
                            case "language":
                                if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
                                id = int.Parse(reader.GetAttribute("id"));
                                if (reader.GetAttribute("localization") == null || reader.GetAttribute("localization").Length == 0) throw (new XmlNullValueException("Invalid localization parameter"));
                                localization = reader.GetAttribute("localization");
                                if (reader.GetAttribute("NumberGroupSeparator") != null && reader.GetAttribute("NumberGroupSeparator").Length > 0) numberGroupSeparator = reader.GetAttribute("NumberGroupSeparator");
                                if (reader.GetAttribute("NumberDecimalSeparator") != null && reader.GetAttribute("NumberDecimalSeparator").Length > 0) numberDecimalSeparator = reader.GetAttribute("NumberDecimalSeparator");
                                if (reader.GetAttribute("name") != null) name = reader.GetAttribute("name");
                                if (reader.GetAttribute("charset") != null) charset = reader.GetAttribute("charset");
                                if (reader.GetAttribute("contentEncoding") != null) contentEncoding = reader.GetAttribute("contentEncoding");
                                if (reader.GetAttribute("excelContentEncoding") != null) excelContentEncoding = reader.GetAttribute("excelContentEncoding");
                                if (reader.GetAttribute("imageSourceText") != null) imageSourceText = reader.GetAttribute("imageSourceText");
                                if (reader.GetAttribute("classificationLanguageId") != null && reader.GetAttribute("classificationLanguageId").Length > 0)
                                    classificationLanguageId = int.Parse(reader.GetAttribute("classificationLanguageId"));
                                else
                                    classificationLanguageId = id;
                                if (reader.GetAttribute("nlsSort") != null && reader.GetAttribute("nlsSort").Length > 0)
                                    nlsSort = reader.GetAttribute("nlsSort");
                                cInfo = new BastetCultureInfo(localization);
                                cInfoExcel = new BastetCultureInfo(localization);
                                if (numberDecimalSeparator.Length > 0) cInfoExcel.NumberFormat.NumberDecimalSeparator = cInfo.NumberFormat.NumberDecimalSeparator = numberDecimalSeparator;
                                if (numberGroupSeparator.Length > 0) cInfoExcel.NumberFormat.NumberGroupSeparator = cInfo.NumberFormat.NumberGroupSeparator = numberGroupSeparator;
                                languages.Add(id, new WebLanguage(id, imageSourceText, name, localization, classificationLanguageId, charset, contentEncoding, excelContentEncoding, nlsSort, cInfo, cInfoExcel));
                                break;
                            case "unitformat":
                                if (reader.GetAttribute("name") != null) formatName = reader.GetAttribute("name");
                                if (reader.GetAttribute("format") != null) format = reader.GetAttribute("format");
                                if (reader.GetAttribute("excelFormat") != null && reader.GetAttribute("excelFormat").Length > 0) excelFormat = reader.GetAttribute("excelFormat");
                                else excelFormat = format;
                                cInfo.AddPattern(formatName, format);
                                cInfo.AddExcelPattern(formatName, excelFormat);
                                cInfoExcel.AddPattern(formatName, excelFormat);
                                cInfoExcel.AddExcelPattern(formatName, excelFormat);
                                break;
                            case "dateFormat":
                                if (reader.GetAttribute("name") != null) formatName = reader.GetAttribute("name");
                                if (reader.GetAttribute("format") != null) format = reader.GetAttribute("format");
                                if (reader.GetAttribute("excelFormat") != null && reader.GetAttribute("excelFormat").Length > 0) excelFormat = reader.GetAttribute("excelFormat");
                                else excelFormat = format;
                                cInfo.AddPattern(formatName, format);
                                cInfo.AddExcelPattern(formatName, excelFormat);
                                cInfoExcel.AddPattern(formatName, excelFormat);
                                cInfoExcel.AddExcelPattern(formatName, excelFormat);
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

                throw (new TNS.AdExpress.Domain.Exceptions.WebLanguagesXLException(" Error : ",err));
            }
            #endregion

            dataSource.Close();
            return (languages);
        }
        #endregion
    }
}
