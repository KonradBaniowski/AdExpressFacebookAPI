using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Translation;
using Oracle.DataAccess.Client;
using AdExClassification = TNS.AdExpress.DataAccess.Classification;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.DB.Common;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;

namespace TNS.AdExpress.Web.Helper
{
    /// <summary>
	/// Class to shaw universe into export file
	/// </summary>
	public class DisplayUniverse
    {

        /// <summary>
        /// Get Html render to show universe selection into excel file
        /// </summary>
        /// <param name="adExpressUniverse">adExpress Universe</param>
        /// <param name="language">language</param>
        /// <param name="source">Data Source</param>
        /// <returns>Html render to show universe selection</returns>
        public static string ToExcel(AdExpressUniverse adExpressUniverse, int language, int dataLanguage, IDataSource source, string domainName)
        {


            #region Variables
            StringBuilder html = new StringBuilder();
            List<NomenclatureElementsGroup> groups = null;
            int baseColSpan = 4;
            #endregion

            if (adExpressUniverse != null && adExpressUniverse.Count() > 0)
            {
                //Groups of items excludes
                groups = adExpressUniverse.GetExludes();
                html.Append(GetUniverseGroupForExcel(groups, baseColSpan, language, dataLanguage, source, AccessType.excludes, domainName));

                //Groups of items includes
                groups = adExpressUniverse.GetIncludes();
                html.Append(GetUniverseGroupForExcel(groups, baseColSpan, language, dataLanguage, source, AccessType.includes, domainName));

            }
            return html.ToString();
        }

        /// <summary>
        /// Get Html render to show universe selection into excel file
        /// </summary>
        /// <param name="adExpressUniverse">adExpress Universe</param>
        /// <param name="language">language</param>
        /// <param name="source">Data Source</param>
        /// <returns>Html render to show universe selection</returns>
        public static string ToHtml(AdExpressUniverse adExpressUniverse, int language, int dataLanguage, IDataSource source, int witdhTable)
        {

            int currentLine = 0;

            return ToHtml(adExpressUniverse, language, dataLanguage, source, witdhTable, false, -1, ref currentLine);
        }

        /// <summary>
        /// Get Html render to show universe selection into excel file
        /// </summary>
        /// <param name="adExpressUniverse">adExpress Universe</param>
        /// <param name="language">language</param>
        /// <param name="source">Data Source</param>
        /// <returns>Html render to show universe selection</returns>
        public static string ToHtml(AdExpressUniverse adExpressUniverse, int language, int dataLanguage, IDataSource source, int witdhTable, bool paginate, int nbLineByPage, ref int currentLine)
        {
            StringBuilder html = new StringBuilder();
            List<NomenclatureElementsGroup> groups = null;
            int baseColSpan = 3;

            if (adExpressUniverse != null && adExpressUniverse.Count() > 0)
            {
                html.Append("<table style=\" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=" + witdhTable + "  >");
                //Groups of items excludes
                groups = adExpressUniverse.GetExludes();
                html.Append(GetUniverseGroupForHtml(groups, baseColSpan, language, dataLanguage, source, AccessType.excludes, paginate, nbLineByPage, ref currentLine));

                //Groups of items includes
                groups = adExpressUniverse.GetIncludes();
                html.Append(GetUniverseGroupForHtml(groups, baseColSpan, language, dataLanguage, source, AccessType.includes, paginate, nbLineByPage, ref currentLine));

                html.Append("</table>");
            }

            return html.ToString();
        }

        #region Méthodes internes
        /// <summary>
        /// Give the Css name accoribng to level
        /// </summary>
        /// <param name="level">Niveau de l'arbre</param>
        /// <param name="cssNameStyle">Css name style</param>
        /// <returns>Nom du style CSS</returns>
        private static string GetLevelCss(int level, string cssNameStyle)
        {
            switch (level)
            {
                case 1:
                    return ("Level" + cssNameStyle + "1");
                case 2:
                    return ("Level" + cssNameStyle + "2");
                case 3:
                    return ("Level" + cssNameStyle + "3");
                default:
                    return ("Level" + cssNameStyle + "1");
            }
        }

        /// <summary>
        /// Get Html render to show universe selection into excel file
        /// </summary>
        /// <param name="groups">universe groups</param>
        /// <param name="baseColSpan">base column span</param>
        /// <param name="language">language</param>
        /// <param name="dataLanguage">Data language</param>
        /// <param name="source">Data Source</param>
        /// <param name="accessType">items access type</param>
        /// <returns>Html render to show universe selection</returns>
        private static string GetUniverseGroupForExcel(List<NomenclatureElementsGroup> groups, int baseColSpan, int language, int dataLanguage, IDataSource source, AccessType accessType, string domainName)
        {

            #region Variables
            int level = 1;
            List<long> itemIdList = null;
            bool lineClosed = false;
            int colSpan = 0;
            string themeName = WebApplicationParameters.Themes[language].Name;
            string img = string.Empty;
            //TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess universeItems = null;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL universeItems = null;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = null;
            TNS.AdExpress.Domain.Layers.CoreLayer cl = null;
            int code = 0;
            StringBuilder html = new StringBuilder();

            if (domainName.Length > 0)
                img = "<img src=\"http://" + domainName + "/App_Themes/" + themeName + "/Images/Common/checkbox.GIF\">";
            else
                img = "<img src=\"/App_Themes/" + themeName + "/Images/Common/checkbox.GIF\">";
            #endregion

            if (accessType == AccessType.includes) code = 2281;
            else code = 2282;


            if (groups != null && groups.Count > 0)
            {
                cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Detail selection control"));
                object[] param = new object[2];
                param[0] = source;
                param[1] = dataLanguage;
                factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

                for (int i = 0; i < groups.Count; i++)
                {
                    List<long> levelIdsList = groups[i].GetLevelIdsList();

                    html.Append(GetBlankLine(baseColSpan));
                    if (i > 0 && accessType == AccessType.includes) code = 2368;
                    html.Append("<tr class=\"excelData\"><td colspan=" + baseColSpan + "  ><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(code, language) + "&nbsp; : </font></td></tr>");

                    //For each group's level
                    if (levelIdsList != null)
                    {

                        for (int j = 0; j < levelIdsList.Count; j++)
                        {


                            //universeItems = new TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess(UniverseLevels.Get(levelIdsList[j]).TableName,groups[i].GetAsString(levelIdsList[j]),dataLanguage,source);
                            universeItems = factoryLevels.CreateDefaultClassificationLevelListDAL(UniverseLevels.Get(levelIdsList[j]), groups[i].GetAsString(levelIdsList[j]));
                            if (universeItems != null)
                            {
                                itemIdList = universeItems.IdListOrderByClassificationItem;
                                if (itemIdList != null && itemIdList.Count > 0)
                                {
                                    //Level label
                                    level = 1;
                                    html.Append("<tr class=\"violetBorder\"><td colspan=" + baseColSpan + " class=" + GetLevelCss(level, "") + " >" + GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, language) + " </td></tr>");

                                    //Show items of the current level
                                    level = 2;
                                    html.Append("<tr>");
                                    for (int k = 0; k < itemIdList.Count; k++)
                                    {

                                        //Compute colspan of current cell
                                        if ((itemIdList.Count == (k + 1)) && (itemIdList.Count % (baseColSpan - 1)) > 0) colSpan = ((baseColSpan - 1) - (itemIdList.Count % (baseColSpan - 1))) + 1;

                                        //Current item label
                                        lineClosed = false;
                                        html.Append("<td class=" + GetLevelCss(level, "") + " colspan=" + colSpan + " style=\"white-space:nowrap;\">" + img + "&nbsp;&nbsp;&nbsp;&nbsp;" + universeItems[Int64.Parse(itemIdList[k].ToString())] + "</td>");
                                        if (k > 0 && ((k + 1) % (baseColSpan - 1)) == 0)
                                        {
                                            lineClosed = true;
                                            html.Append("<td class=" + GetLevelCss(level, "RightBorder") + ">&nbsp;</td></tr>");//Items are showed on three columns
                                        }
                                        colSpan = 0;
                                    }
                                    if (!lineClosed)
                                    {
                                        html.Append("<td class=" + GetLevelCss(level, "RightBorder") + ">&nbsp;</td></tr>");
                                    }
                                    // Bordure en bas et bordure à droite
                                    html.Append("<tr><td colspan=" + baseColSpan + " class=" + GetLevelCss(level, "RightBottomBorder") + " >&nbsp;</td></tr>");
                                    html.Append("</tr>");
                                }


                            }

                        }
                    }
                }
            }
            universeItems = null;
            factoryLevels = null;
            cl = null;
            return html.ToString();
        }

        /// <summary>
        /// Get Html render to show universe selection into excel file
        /// </summary>
        /// <param name="groups">universe groups</param>
        /// <param name="baseColSpan">base column span</param>
        /// <param name="language">language</param>
        /// <param name="source">Data Source</param>
        /// <param name="accessType">items access type</param>
        /// <returns>Html render to show universe selection</returns>
        private static string GetUniverseGroupForHtml(List<NomenclatureElementsGroup> groups, int baseColSpan, int language, int dataLanguage, IDataSource source, AccessType accessType, bool paginate, int nbLineByPage, ref int currentLine)
        {


            #region Variables
            List<long> itemIdList = null;
            //int colSpan = 0;
            string checkBox = "";
            string buttonAutomaticChecked = "checked";
            string disabled = "disabled";
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL universeItems = null;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = null;
            TNS.AdExpress.Domain.Layers.CoreLayer cl = null;
            int code = 0;
            StringBuilder html = new StringBuilder();
            int colonne = 0;
            bool displayBorder = true;
            #endregion

            if (accessType == AccessType.includes) code = 2281;
            else code = 2282;


            if (groups != null && groups.Count > 0)
            {
                cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Detail selection control"));
                object[] param = new object[2];
                param[0] = source;
                param[1] = dataLanguage;
                factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                for (int i = 0; i < groups.Count; i++)
                {
                    List<long> levelIdsList = groups[i].GetLevelIdsList();
                    displayBorder = true;

                    html.Append(GetBlankLine(baseColSpan));
                    if (paginate) currentLine++;

                    if (i > 0 && accessType == AccessType.includes) code = 2368;
                    html.Append("<tr class=\"txtViolet11Bold\"><td colspan=" + baseColSpan + "  >" + GestionWeb.GetWebWord(code, language) + "&nbsp; : </td></tr>");
                    if (paginate) currentLine++;

                    //For each group's level
                    if (levelIdsList != null)
                    {
                        html.Append("<tr><td>");

                        for (int j = 0; j < levelIdsList.Count; j++)
                        {

                            //Level label
                            if (displayBorder) html.Append("<table class=\"UniverseHeaderStyle\"  cellpadding=0 cellspacing=0 width=\"100%\"  >");
                            else html.Append("<table class=\"UniverseHeaderStyleWithoutTop\"  cellpadding=0 cellspacing=0  width=\"100%\">");
                            html.Append("<tr class=\"txtViolet11Bold\" ><td colspan=" + baseColSpan + "  >&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, language)) + " </td></tr>");
                            html.Append("</table>");
                            displayBorder = false;
                            if (paginate) currentLine++;

                            //Show items of the current level							
                            colonne = 0;
                            universeItems = factoryLevels.CreateDefaultClassificationLevelListDAL(UniverseLevels.Get(levelIdsList[j]), groups[i].GetAsString(levelIdsList[j]));
                            if (universeItems != null)
                            {
                                itemIdList = universeItems.IdListOrderByClassificationItem;
                                if (itemIdList != null && itemIdList.Count > 0)
                                {
                                    html.Append("<table class=\"UniverseItemsStyle\" width=\"100%\">");

                                    for (int k = 0; k < itemIdList.Count; k++)
                                    {

                                        //Current item label										
                                        checkBox = "<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"AdvertiserSelectionWebControl1_" + k + "\" name=\"AdvertiserSelectionWebControl1$" + k + "\">";
                                        if (colonne == 2)
                                        {

                                            html.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                                            html.Append(checkBox + "<label for=\"AdvertiserSelectionWebControl1_" + k + "\">" + universeItems[Int64.Parse(itemIdList[k].ToString())] + "</label>");
                                            html.Append("</td>");
                                            colonne = 1;
                                        }
                                        else if (colonne == 1)
                                        {
                                            html.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                                            html.Append(checkBox + "<label for=\"AdvertiserSelectionWebControl1_" + k + "\">" + universeItems[Int64.Parse(itemIdList[k].ToString())] + "</label>");
                                            html.Append("</td>");
                                            html.Append("</tr>");
                                            colonne = 0;

                                            if (paginate) currentLine++;
                                            //Pagination
                                            if (paginate && nbLineByPage > 0 && currentLine > 0 && (currentLine % nbLineByPage) == 0 && k < itemIdList.Count - 1)
                                            {
                                                html.Append("</table>");
                                                html.Append("<table class=\"violetRightLeftBorder paleVioletBackGround\" width=\"100%\">");
                                                currentLine = 0;
                                            }

                                        }
                                        else
                                        {
                                            html.Append("<tr>");
                                            html.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                                            html.Append(checkBox + "<label for=\"AdvertiserSelectionWebControl1_" + k + "\">" + universeItems[Int64.Parse(itemIdList[k].ToString())] + "</label>");
                                            html.Append("</td>");
                                            colonne = 2;
                                        }
                                    }
                                    if (colonne != 0)
                                    {
                                        if (colonne == 2)
                                        {
                                            html.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                                            html.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                                        }
                                        else if (colonne == 1)
                                        {
                                            html.Append("<td class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                                        }
                                        html.Append("</tr>");
                                        if (paginate) currentLine++;
                                    }
                                }
                                html.Append("</table>");
                            }

                        }
                        html.Append("</td></tr>");
                    }
                }
            }

            return html.ToString();
        }

        #region Spacer line
        /// <summary>
        /// Spacer line
        /// </summary>
        /// <returns>HTML</returns>
        private static string GetBlankLine(int colspan)
        {
            return ("<tr><td colspan=" + colspan + ">&nbsp;</td></tr>");
        }
        #endregion

        #endregion

    }
}
