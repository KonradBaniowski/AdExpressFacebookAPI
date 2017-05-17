using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;

namespace TNS.AdExpress.Web.Utilities.Functions
{
    public class DisplayTreeNode
    {
        #region Affichage d'un arbre

        #region Cas 1 avec witdhTable en int
        /// <summary>
        ///  Affichage d'un arbre au format HTML
        /// </summary>
        /// <param name="root">Arbre à afficher</param>
        /// <param name="write">Arbre en lecture où en écriture</param>
        /// <param name="displayArrow">Affichage de la flêche</param>
        /// <param name="displayCheckbox">Affichage de la checkbox</param>
        /// <param name="witdhTable">Largeur de la table</param>
        /// <param name="displayBorderTable">Affichage de la bordure</param>
        /// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
        /// <param name="SiteLanguage">Langue</param>
        /// <param name="showHideContent">Index ShowHideCOntent ?</param>
        /// <param name="typetree">Type d'arbre ?</param>
        /// <param name="div">Afficher les div true si c'est le cas</param>
        /// <returns>tableau correspondant à l'arbre</returns>
        public static string ToHtml(TreeNode root, bool write, bool displayArrow, bool displayCheckbox, int witdhTable, bool displayBorderTable, bool allSelection, int SiteLanguage, int typetree, int showHideContent, bool div, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source)
        {
            return (ToHtml(root, write, displayArrow, displayCheckbox, witdhTable, displayBorderTable, allSelection, SiteLanguage, typetree, showHideContent, div, true, dataLanguage, source));
        }
        /// <summary>
        ///  Affichage d'un arbre au format HTML
        /// </summary>
        /// <param name="root">Arbre à afficher</param>
        /// <param name="write">Arbre en lecture où en écriture</param>
        /// <param name="displayArrow">Affichage de la flêche</param>
        /// <param name="displayCheckbox">Affichage de la checkbox</param>
        /// <param name="witdhTable">Largeur de la table</param>
        /// <param name="displayBorderTable">Affichage de la bordure</param>
        /// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
        /// <param name="SiteLanguage">Langue</param>
        /// <param name="showHideContent">Index ShowHideCOntent ?</param>
        /// <param name="typetree">Type d'arbre ?</param>
        /// <param name="div">Afficher les div true si c'est le cas</param>
        /// <returns>tableau correspondant à l'arbre</returns>
        public static string ToHtml(TreeNode root, bool write, bool displayArrow, bool displayCheckbox, int witdhTable, bool displayBorderTable, bool allSelection, int SiteLanguage, int typetree, int showHideContent, bool div, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source, bool isExport)
        {
            return (ToHtml(root, write, displayArrow, displayCheckbox, witdhTable, displayBorderTable, allSelection, SiteLanguage, typetree, showHideContent, div, false, dataLanguage, source, isExport));
        }
        #endregion

        #region Cas 2 avec witdhTable en pourcenatge
        /// <summary>
        ///  Affichage d'un arbre au format HTML
        /// </summary>
        /// <param name="root">Arbre à afficher</param>
        /// <param name="write">Arbre en lecture où en écriture</param>
        /// <param name="displayArrow">Affichage de la flêche</param>
        /// <param name="displayCheckbox">Affichage de la checkbox</param>
        /// <param name="witdhTable">Largeur de la table (en pourcentage)</param>
        /// <param name="displayBorderTable">Affichage de la bordure</param>
        /// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
        /// <param name="SiteLanguage">Langue</param>
        /// <param name="showHideContent">Index ShowHideCOntent ?</param>
        /// <param name="typetree">Type d'arbre ?</param>
        /// <param name="div">Afficher les div true si c'est le cas</param>
        /// <returns>tableau correspondant à l'arbre</returns>
        public static string ToHtml(TreeNode root, bool write, bool displayArrow, bool displayCheckbox, string witdhTable, bool displayBorderTable, bool allSelection, int SiteLanguage, int typetree, int showHideContent, bool div, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source)
        {
            return (ToHtml(root, write, displayArrow, displayCheckbox, int.Parse(witdhTable), displayBorderTable, allSelection, SiteLanguage, typetree, showHideContent, div, true, dataLanguage, source));
        }
        #endregion

        #endregion

        #region Méthode interne

        #region Affichage d'un arbre
        /// <summary>
        ///  Affichage d'un arbre au format HTML
        /// </summary>
        /// <param name="root">Arbre à afficher</param>
        /// <param name="write">Arbre en lecture où en écriture</param>
        /// <param name="displayArrow">Affichage de la flêche</param>
        /// <param name="displayCheckbox">Affichage de la checkbox</param>
        /// <param name="witdhTable">Largeur de la table</param>
        /// <param name="displayBorderTable">Affichage de la bordure</param>
        /// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
        /// <param name="SiteLanguage">Langue</param>
        /// <param name="showHideContent">Index ShowHideCOntent ?</param>
        /// <param name="typetree">Type d'arbre ?</param>
        /// <param name="div">Afficher les div true si c'est le cas</param>
        /// <param name="percentage">Pour indiquer si la valeur de witdhTable est en pourcentage ou pas</param>
        /// <returns>tableau correspondant à l'arbre</returns>
        private static string ToHtml(TreeNode root, bool write, bool displayArrow, bool displayCheckbox, int witdhTable, bool displayBorderTable, bool allSelection, int SiteLanguage, int typetree, int showHideContent, bool div, bool percentage, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source)
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            string themeName = WebApplicationParameters.Themes[SiteLanguage].Name;
            //int nbElement=0;
            string treeNode = "", percentageSymbol = "";
            int i = 0;
            int start = 0;
            int colonne = 0;
            string buttonAutomaticChecked = "";
            string disabled = "";
            string tmp = "";
            int j = 0;
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, List<long>> dic = null;
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL> dicClassif = null;
            TNS.AdExpress.Domain.Layers.CoreLayer cl = null;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = null;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL levelItems = null;
            List<long> idList = null;
            long id = -1;
            TNS.AdExpress.Constantes.Customer.Right.type levelType = TNS.AdExpress.Constantes.Customer.Right.type.nothing;

            if (percentage)
                percentageSymbol = "%";

            foreach (TreeNode currentNode in root.Nodes)
            {
                if (dic == null) dic = new Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, List<long>>();
                id = ((LevelInformation)currentNode.Tag).ID;
                levelType = ((LevelInformation)currentNode.Tag).Type;
                if (dic.ContainsKey(levelType))
                {
                    if (!dic[levelType].Contains(id)) dic[levelType].Add(id);
                }
                else
                {
                    idList = new List<long>();
                    idList.Add(id);
                    dic.Add(levelType, idList);
                }

                i = 0;
                while (i < currentNode.Nodes.Count)
                {
                    id = ((LevelInformation)currentNode.Nodes[i].Tag).ID;
                    levelType = ((LevelInformation)currentNode.Nodes[i].Tag).Type;
                    if (dic.ContainsKey(levelType))
                    {
                        if (!dic[levelType].Contains(id)) dic[levelType].Add(id);
                    }
                    else
                    {
                        idList = new List<long>();
                        idList.Add(id);
                        dic.Add(levelType, idList);
                    }
                    i++;
                }
            }
            if (dic != null && dic.Count > 0)
            {
                cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Detail selection control"));
                object[] param = new object[2];
                param[0] = source;
                param[1] = dataLanguage;
                factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

                foreach (KeyValuePair<TNS.AdExpress.Constantes.Customer.Right.type, List<long>> kpv in dic)
                {
                    string[] intArrayStr = Array.ConvertAll<long, string>(kpv.Value.ToArray(), new Converter<long, string>(Convert.ToString));
                    string temp = String.Join(",", intArrayStr);
                    levelItems = factoryLevels.CreateClassificationLevelListDAL(kpv.Key, temp);
                    if (levelItems != null && levelItems.IdListOrderByClassificationItem.Count > 0)
                    {
                        if (dicClassif == null) dicClassif = new Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, ClassificationLevelListDAL>();
                        dicClassif.Add(kpv.Key, levelItems);
                    }
                }
            }

            foreach (TreeNode currentNode in root.Nodes)
            {
                if (start == 0)
                {
                    if (displayBorderTable)
                    {
                        t.Append("<table class=\"TreeHeaderVioletBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + "  >");
                        start = 1;
                    }
                    else
                    {
                        t.Append("<table class=\"TreeHeaderBlancBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
                    }

                }
                else
                {
                    if (displayBorderTable)
                    {
                        t.Append("<table class=\"TreeHeaderVioletBorderWithoutTop\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
                    }
                    else
                    {
                        t.Append("<table class=\"TreeHeaderBlancBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
                    }
                }
                t.Append("<tr>");
                t.Append("<td align=\"left\" height=\"10\"  valign=\"middle\" nowrap>");
                if (displayCheckbox)
                {
                    //En lecture et Non cocher
                    if (!write && !currentNode.Checked)
                    {
                        disabled = "disabled";
                        buttonAutomaticChecked = "";
                        if (typetree == 2)
                        {
                            t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">");
                        }
                        else if (typetree == 3)
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                        else
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                    }
                    //En lecture et cocher
                    else if (!write && currentNode.Checked)
                    {
                        disabled = "disabled";
                        buttonAutomaticChecked = "checked";
                        if (typetree == 2)
                        {
                            t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">");
                        }
                        else if (typetree == 3)
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                        else
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                    }
                    //En Ecriture et Non cocher
                    else if (write && !currentNode.Checked)
                    {
                        disabled = "";
                        buttonAutomaticChecked = "";
                        if (typetree == 2)
                        {
                            if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                            {
                                //t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">");
                                t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">");
                            }
                        }
                        else if (typetree == 3)
                        {
                            disabled = "disabled";
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                        else
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                    }
                    //En Ecriture et cocher
                    else
                    {
                        disabled = "";
                        buttonAutomaticChecked = "checked";
                        if (typetree == 2)
                        {
                            if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                            {
                                t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">");
                            }
                        }
                        else if (typetree == 3)
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                        else
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                    }
                    t.Append("</label>");
                }
                else { t.Append("&nbsp;"); }

                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                {
                    //t.Append("" + ((LevelInformation)currentNode.Tag).Text + "");
                    t.Append("" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "");
                }
                t.Append("</td>");
                if (displayArrow && currentNode.Nodes.Count > 0)
                {
                    if (showHideContent == 1)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent1('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                    else if (showHideContent == 2)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent2('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                    else if (showHideContent == 3)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent3('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                    else if (showHideContent == 4)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent4('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                    else if (showHideContent == 5)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent5('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                }
                else
                {
                    t.Append("<td width=\"15\"></td>");
                }
                t.Append("</tr>");
                t.Append("</table>");
                if (currentNode.Nodes.Count > 0)
                {
                    if (displayBorderTable)
                    {
                        if (div)
                        {
                            t.Append("<div id=\"" + ((LevelInformation)currentNode.Tag).ID + "Content" + showHideContent + "\" class=\"BlancBorderColorWithoutTop\"  style=\"DISPLAY: none; WIDTH: 100%\">");
                        }
                        t.Append("<table class=\"TreeTableVioletBorder\" width=" + witdhTable + percentageSymbol + ">");
                    }
                    else
                    {
                        if (div)
                        {
                            t.Append("<div class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\">");
                        }
                        t.Append("<table class=\"TreeTableBlancBorder\" width=" + witdhTable + percentageSymbol + ">");
                    }
                }
                //Cas où l'on veut mettre le lien tout sélectionner
                if (allSelection)
                {
                    int code = 817;
                    bool parentChecked = true;
                    // cas advertiser
                    if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.advertiserException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess)
                    {
                        code = 817;
                    }
                    // cas Marque
                    if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.brandException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.brandAccess)
                    {
                        code = 817;
                    }
                    // cas Produit
                    if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.productException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.productAccess)
                    {
                        code = 817;
                    }
                    // cas holding company
                    else if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess)
                    {
                        code = 816;
                    }
                    // cas Sector
                    else if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sectorException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess)
                    {
                        code = 968;
                    }
                    else if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.subSectorException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess)
                    {
                        code = 969;
                    }

                    if (typetree == 1)
                    {
                        t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\">" + GestionWeb.GetWebWord(code, SiteLanguage) + "</a></td></tr>");
                    }
                    else if (typetree == 2)
                    {
                        t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection2('" + ((LevelInformation)currentNode.Tag).ID + ((LevelInformation)currentNode.Tag).Text + "')\" ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\">" + GestionWeb.GetWebWord(code, SiteLanguage) + "</a></td></tr>");
                    }

                    if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException
                        || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.advertiserException
                        || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sectorException
                        || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.subSectorException
                        )
                    {
                        parentChecked = false;
                    }


                    if (typetree == 3 && !parentChecked)
                    {
                        t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\">" + GestionWeb.GetWebWord(code, SiteLanguage) + "</a></td></tr>");
                    }

                }
                colonne = 0;
                i = 0;
                while (i < currentNode.Nodes.Count)
                {

                    if (displayArrow)
                    {
                        //En lecture et Non cocher
                        if (!write && !currentNode.Nodes[i].Checked)
                        {
                            disabled = "disabled";
                            buttonAutomaticChecked = "";
                            if (typetree == 2)
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                                {
                                    //+ dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + 
                                    //tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + ((LevelInformation)currentNode.Tag).Text + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br>";
                                }
                            }
                            else
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    //tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br></label>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }
                        }
                        //En lecture et cocher
                        else if (!write && currentNode.Nodes[i].Checked)
                        {
                            disabled = "disabled";
                            buttonAutomaticChecked = "checked";
                            if (typetree == 2)
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    //tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + ((LevelInformation)currentNode.Tag).Text + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).ID + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br>";
                                }
                            }
                            else
                            {

                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    //tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br></label>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }

                        }
                        //En Ecriture et Non cocher
                        else if (write && !currentNode.Nodes[i].Checked)
                        {
                            disabled = "";
                            buttonAutomaticChecked = "";
                            if (typetree == 2)
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                                {

                                    //tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + ((LevelInformation)currentNode.Tag).Text + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br>";
                                }
                            }
                            else if (typetree == 3)
                            {
                                disabled = "disabled";
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    //tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br></label>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }
                            else
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    //tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1$"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }

                        }
                        else
                        {
                            disabled = "";
                            buttonAutomaticChecked = "checked";
                            if (typetree == 2)
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                                {
                                    //tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + ((LevelInformation)currentNode.Tag).Text + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br>";
                                }
                            }
                            else
                            {

                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    //tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br></label>";
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }
                        }
                    }
                    else { t.Append("&nbsp;"); }

                    if (colonne == 2)
                    {

                        t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                        t.Append(tmp);
                        t.Append("</td>");
                        colonne = 1;
                    }
                    else if (colonne == 1)
                    {
                        t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                        t.Append(tmp);
                        t.Append("</td>");
                        t.Append("</tr>");
                        colonne = 0;
                    }
                    else
                    {
                        t.Append("<tr>");
                        t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                        t.Append(tmp);
                        t.Append("</td>");
                        colonne = 2;
                    }

                    //	t.Append(((LevelInformation)currentNode.Nodes[i].Tag).Text);
                    i++;
                }
                if (currentNode.Nodes.Count > 0)
                {
                    if (colonne != 0)
                    {
                        if (colonne == 2)
                        {
                            t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                            t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                        }
                        else if (colonne == 1)
                        {
                            t.Append("<td class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                        }
                        t.Append("</tr>");
                    }
                    t.Append("</table>");
                    if (div)
                    {
                        t.Append("</div>");
                    }
                }

            }

            treeNode = t.ToString();
            return treeNode;
        }
        #endregion


        /// <summary>
        ///  Affichage d'un arbre au format HTML
        /// </summary>
        /// <param name="root">Arbre à afficher</param>
        /// <param name="write">Arbre en lecture où en écriture</param>
        /// <param name="displayArrow">Affichage de la flêche</param>
        /// <param name="displayCheckbox">Affichage de la checkbox</param>
        /// <param name="witdhTable">Largeur de la table</param>
        /// <param name="displayBorderTable">Affichage de la bordure</param>
        /// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
        /// <param name="SiteLanguage">Langue</param>
        /// <param name="showHideContent">Index ShowHideCOntent ?</param>
        /// <param name="typetree">Type d'arbre ?</param>
        /// <param name="div">Afficher les div true si c'est le cas</param>
        /// <param name="percentage">Pour indiquer si la valeur de witdhTable est en pourcentage ou pas</param>
        /// <returns>tableau correspondant à l'arbre</returns>
        private static string ToHtml(TreeNode root, bool write, bool displayArrow, bool displayCheckbox, int witdhTable, bool displayBorderTable, bool allSelection, int SiteLanguage, int typetree, int showHideContent, bool div, bool percentage, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source, bool isExport)
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            string themeName = WebApplicationParameters.Themes[SiteLanguage].Name;
            //int nbElement=0;
            string treeNode = "", percentageSymbol = "";
            int i = 0;
            int start = 0;
            int colonne = 0;
            string buttonAutomaticChecked = "";
            string disabled = "";
            string tmp = "";
            int j = 0;
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, List<long>> dic = null;
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL> dicClassif = null;
            TNS.AdExpress.Domain.Layers.CoreLayer cl = null;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = null;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL levelItems = null;
            List<long> idList = null;
            long id = -1;
            TNS.AdExpress.Constantes.Customer.Right.type levelType = TNS.AdExpress.Constantes.Customer.Right.type.nothing;

            if (percentage)
                percentageSymbol = "%";

            foreach (TreeNode currentNode in root.Nodes)
            {
                if (dic == null) dic = new Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, List<long>>();
                id = ((LevelInformation)currentNode.Tag).ID;
                levelType = ((LevelInformation)currentNode.Tag).Type;
                if (dic.ContainsKey(levelType))
                {
                    if (!dic[levelType].Contains(id)) dic[levelType].Add(id);
                }
                else
                {
                    idList = new List<long>();
                    idList.Add(id);
                    dic.Add(levelType, idList);
                }

                i = 0;
                while (i < currentNode.Nodes.Count)
                {
                    id = ((LevelInformation)currentNode.Nodes[i].Tag).ID;
                    levelType = ((LevelInformation)currentNode.Nodes[i].Tag).Type;
                    if (dic.ContainsKey(levelType))
                    {
                        if (!dic[levelType].Contains(id)) dic[levelType].Add(id);
                    }
                    else
                    {
                        idList = new List<long>();
                        idList.Add(id);
                        dic.Add(levelType, idList);
                    }
                    i++;
                }
            }
            if (dic != null && dic.Count > 0)
            {
                cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Detail selection control"));
                object[] param = new object[2];
                param[0] = source;
                param[1] = dataLanguage;
                factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

                foreach (KeyValuePair<TNS.AdExpress.Constantes.Customer.Right.type, List<long>> kpv in dic)
                {
                    string[] intArrayStr = Array.ConvertAll<long, string>(kpv.Value.ToArray(), new Converter<long, string>(Convert.ToString));
                    string temp = String.Join(",", intArrayStr);
                    levelItems = factoryLevels.CreateClassificationLevelListDAL(kpv.Key, temp);
                    if (levelItems != null && levelItems.IdListOrderByClassificationItem.Count > 0)
                    {
                        if (dicClassif == null) dicClassif = new Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, ClassificationLevelListDAL>();
                        dicClassif.Add(kpv.Key, levelItems);
                    }
                }
            }

            foreach (TreeNode currentNode in root.Nodes)
            {
                if (start == 0)
                {
                    if (displayBorderTable)
                    {
                        t.Append("<table class=\"TreeHeaderVioletBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + "  >");
                        start = 1;
                    }
                    else
                    {
                        t.Append("<table class=\"TreeHeaderBlancBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
                    }

                }
                else
                {
                    if (displayBorderTable)
                    {
                        t.Append("<table class=\"TreeHeaderVioletBorderWithoutTop\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
                    }
                    else
                    {
                        t.Append("<table class=\"TreeHeaderBlancBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
                    }
                }
                t.Append("<tr>");
                t.Append("<td align=\"left\" height=\"10\"  valign=\"middle\" nowrap>");
                if (displayCheckbox)
                {
                    //En lecture et Non cocher
                    if (!write && !currentNode.Checked)
                    {
                        disabled = "disabled";
                        buttonAutomaticChecked = "";
                        if (typetree == 2)
                        {
                            if (!string.IsNullOrEmpty(((LevelInformation)currentNode.Tag).Text))
                                t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">");
                            else t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_\">");
                        }
                        else if (typetree == 3)
                        {

                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                        else
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                    }
                    //En lecture et cocher
                    else if (!write && currentNode.Checked)
                    {
                        disabled = "disabled";
                        buttonAutomaticChecked = "checked";
                        if (typetree == 2)
                        {
                            if (!string.IsNullOrEmpty(((LevelInformation)currentNode.Tag).Text))
                                t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).Text + "\">");
                            else t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_\">");
                        }
                        else if (typetree == 3)
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                        else
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                    }
                    //En Ecriture et Non cocher
                    else if (write && !currentNode.Checked)
                    {
                        disabled = "";
                        buttonAutomaticChecked = "";
                        if (typetree == 2)
                        {
                            if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                            {
                                t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">");
                            }
                        }
                        else if (typetree == 3)
                        {
                            disabled = "disabled";
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                        else
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                    }
                    //En Ecriture et cocher
                    else
                    {
                        disabled = "";
                        buttonAutomaticChecked = "checked";
                        if (typetree == 2)
                        {
                            if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                            {
                                t.Append("<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\" value=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\" name=\"AUTOMATIC_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">");
                            }
                        }
                        else if (typetree == 3)
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                        else
                        {
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
                            j++;
                        }
                    }
                    t.Append("</label>");
                }
                else { t.Append("&nbsp;"); }

                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                {
                    t.Append("" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "");
                }
                t.Append("</td>");
                if (displayArrow && currentNode.Nodes.Count > 0 && !isExport)
                {
                    if (showHideContent == 1)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent1('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                    else if (showHideContent == 2)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent2('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                    else if (showHideContent == 3)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent3('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                    else if (showHideContent == 4)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent4('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                    else if (showHideContent == 5)
                    {
                        t.Append("<td width=\"100%\" align=right onClick=\"showHideContent5('" + ((LevelInformation)currentNode.Tag).ID + "');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
                    }
                }
                else
                {
                    t.Append("<td width=\"15\"></td>");
                }
                t.Append("</tr>");
                t.Append("</table>");
                if (currentNode.Nodes.Count > 0)
                {
                    if (displayBorderTable)
                    {
                        if (div)
                        {
                            if (!isExport) t.Append("<div id=\"" + ((LevelInformation)currentNode.Tag).ID + "Content" + showHideContent + "\" class=\"BlancBorderColorWithoutTop\"  style=\"DISPLAY: none; WIDTH: 100%\">");
                            else t.Append("<div id=\"" + ((LevelInformation)currentNode.Tag).ID + "Content" + showHideContent + "\" class=\"BlancBorderColorWithoutTop\"  style=\"DISPLAY: ''; WIDTH: 100%\">");
                        }
                        t.Append("<table class=\"TreeTableVioletBorder\" width=" + witdhTable + percentageSymbol + ">");
                    }
                    else
                    {
                        if (div)
                        {
                            if (!isExport) t.Append("<div class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\">");
                            else t.Append("<div class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: ''; WIDTH: 100%\">");
                        }
                        t.Append("<table class=\"TreeTableBlancBorder\" width=" + witdhTable + percentageSymbol + ">");
                    }
                }
                //Cas où l'on veut mettre le lien tout sélectionner
                if (allSelection)
                {
                    int code = 817;
                    bool parentChecked = true;
                    // cas advertiser
                    if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.advertiserException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess)
                    {
                        code = 817;
                    }
                    // cas Marque
                    if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.brandException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.brandAccess)
                    {
                        code = 817;
                    }
                    // cas Produit
                    if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.productException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.productAccess)
                    {
                        code = 817;
                    }
                    // cas holding company
                    else if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess)
                    {
                        code = 816;
                    }
                    // cas Sector
                    else if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sectorException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess)
                    {
                        code = 968;
                    }
                    else if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.subSectorException || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess)
                    {
                        code = 969;
                    }

                    if (typetree == 1)
                    {
                        t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\">" + GestionWeb.GetWebWord(code, SiteLanguage) + "</a></td></tr>");
                    }
                    else if (typetree == 2)
                    {
                        t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection2('" + ((LevelInformation)currentNode.Tag).ID + ((LevelInformation)currentNode.Tag).Text + "')\" ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\">" + GestionWeb.GetWebWord(code, SiteLanguage) + "</a></td></tr>");
                    }

                    if (((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException
                        || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.advertiserException
                        || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sectorException
                        || ((LevelInformation)currentNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.subSectorException
                        )
                    {
                        parentChecked = false;
                    }


                    if (typetree == 3 && !parentChecked)
                    {
                        t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"" + ((LevelInformation)currentNode.Tag).ID + "\">" + GestionWeb.GetWebWord(code, SiteLanguage) + "</a></td></tr>");
                    }

                }
                colonne = 0;
                i = 0;
                while (i < currentNode.Nodes.Count)
                {

                    if (displayArrow || isExport)
                    {
                        //En lecture et Non cocher
                        if (!write && !currentNode.Nodes[i].Checked)
                        {
                            disabled = "disabled";
                            buttonAutomaticChecked = "";
                            if (typetree == 2)
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br>";
                                }
                            }
                            else
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }
                        }
                        //En lecture et cocher
                        else if (!write && currentNode.Nodes[i].Checked)
                        {
                            disabled = "disabled";
                            buttonAutomaticChecked = "checked";
                            if (typetree == 2)
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + ((LevelInformation)currentNode.Tag).ID + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br>";
                                }
                            }
                            else
                            {

                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }

                        }
                        //En Ecriture et Non cocher
                        else if (write && !currentNode.Nodes[i].Checked)
                        {
                            disabled = "";
                            buttonAutomaticChecked = "";
                            if (typetree == 2)
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br>";
                                }
                            }
                            else if (typetree == 3)
                            {
                                disabled = "disabled";
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }
                            else
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }

                        }
                        else
                        {
                            disabled = "";
                            buttonAutomaticChecked = "checked";
                            if (typetree == 2)
                            {
                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Tag).Type) && dicClassif[((LevelInformation)currentNode.Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"" + ((LevelInformation)currentNode.Tag).ID + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\"  value=\"" + ((LevelInformation)currentNode.Nodes[i].Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "\" name=\"CKB_" + ((LevelInformation)currentNode.Tag).ID + "_" + dicClassif[((LevelInformation)currentNode.Tag).Type][((LevelInformation)currentNode.Tag).ID] + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br>";
                                }
                            }
                            else
                            {

                                if (dicClassif != null && dicClassif.ContainsKey(((LevelInformation)currentNode.Nodes[i].Tag).Type) && dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type].IdListOrderByClassificationItem.Contains(((LevelInformation)currentNode.Nodes[i].Tag).ID))
                                {
                                    tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + dicClassif[((LevelInformation)currentNode.Nodes[i].Tag).Type][((LevelInformation)currentNode.Nodes[i].Tag).ID] + "<br></label>";
                                    j++;
                                }
                            }
                        }
                    }
                    else { t.Append("&nbsp;"); }

                    if (colonne == 2)
                    {

                        t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                        t.Append(tmp);
                        t.Append("</td>");
                        colonne = 1;
                    }
                    else if (colonne == 1)
                    {
                        t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                        t.Append(tmp);
                        t.Append("</td>");
                        t.Append("</tr>");
                        colonne = 0;
                    }
                    else
                    {
                        t.Append("<tr>");
                        t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                        t.Append(tmp);
                        t.Append("</td>");
                        colonne = 2;
                    }

                    //	t.Append(((LevelInformation)currentNode.Nodes[i].Tag).Text);
                    i++;
                }
                if (currentNode.Nodes.Count > 0)
                {
                    if (colonne != 0)
                    {
                        if (colonne == 2)
                        {
                            t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                            t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                        }
                        else if (colonne == 1)
                        {
                            t.Append("<td class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                        }
                        t.Append("</tr>");
                    }
                    t.Append("</table>");
                    if (div)
                    {
                        t.Append("</div>");
                    }
                }

            }

            treeNode = t.ToString();
            return treeNode;
        }

        #endregion
    }
}
