using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Custom Styles Confihuration XML loader
    /// </summary>
    public class CustomStylesXL {

        /// <summary>
        /// Load result options
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        /// <summary>
        public static CustomStyles Load(IDataSource dataSource) {

            #region Variables
            XmlTextReader reader = null;
            CustomStyles customStyles = null;
            int mySessionsWidth = 500;
            int treeViewWidth = 230;
            int creationPopUpWidth = 830;
            int synthesisWidth = 600;
            int fileItemWidth = 650;
            int headerWidth = 648;
            int creativePopUpWidth = 394;
            int chartEvolutionWidth = 850;
            int chartEvolutionHeight = 500;
            int chartMediaStrategyWidth = 850;
            int chartMediaStrategyHeight = 850;
            int chartSeasonalityWidth = 850;
            int chartSeasonalityHeight = 500;
            int chartSeasonalityBigWidth = 1150;
            int chartSeasonalityBigHeight = 700;
            #endregion

            try {
                reader = (XmlTextReader)dataSource.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {

                        switch (reader.LocalName) {
                            case "mySessionsWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    mySessionsWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "treeViewWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    treeViewWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "creationPopUpWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    creationPopUpWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "synthesisWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    synthesisWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "fileItemWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    fileItemWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "headerWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    headerWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "creativePopUpWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    creativePopUpWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "chartEvolutionWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    chartEvolutionWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "chartEvolutionHeight":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    chartEvolutionHeight = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "chartMediaStrategyWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    chartMediaStrategyWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "chartMediaStrategyHeight":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    chartMediaStrategyHeight = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "chartSeasonalityWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    chartSeasonalityWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "chartSeasonalityHeight":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    chartSeasonalityHeight = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "chartSeasonalityBigWidth":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    chartSeasonalityBigWidth = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                            case "chartSeasonalityBigHeight":
                                if (reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0) {
                                    chartSeasonalityBigHeight = Convert.ToInt32(reader.GetAttribute("width"));
                                }
                                break;
                        }
                    }
                }
            }

            #region Error Management
            catch (System.Exception err) {

                #region Close the file
                if (dataSource.GetSource() != null) dataSource.Close();
                #endregion

                throw (new XmlException(" Error while loading RightOptions.xml : ", err));
            }
            #endregion

            dataSource.Close();

            return new CustomStyles(mySessionsWidth, treeViewWidth, creationPopUpWidth, synthesisWidth, fileItemWidth, headerWidth, creativePopUpWidth, chartEvolutionWidth, chartEvolutionHeight, chartMediaStrategyWidth, chartMediaStrategyHeight
                                        ,chartSeasonalityWidth, chartSeasonalityHeight, chartSeasonalityBigWidth, chartSeasonalityBigHeight);

        }

    }
}
