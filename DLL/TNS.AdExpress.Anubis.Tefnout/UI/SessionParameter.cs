#region Information
//Authors: Y. Rkaina
//Date of Creation: 05/06/2006
//Date of modification:
#endregion

using System;
using System.IO;
using Aspose.Cells;
using System.Drawing;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using System.Windows.Forms;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using TefnoutExceptions = TNS.AdExpress.Anubis.Tefnout.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TefnoutFunctions = TNS.AdExpress.Anubis.Tefnout.Functions;
using CsteCustomer = TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Anubis.Tefnout.UI
{
    /// <summary>
    /// Description résumée de SessionParameter.
    /// </summary>
    public class SessionParameter
    {
        #region Variables Theme Name
        private static string _tagTitle = "SessionParameterTitle";
        private static string _tagPeriodTitle = "SessionParameterPeriodTitle";
        private static string _tagPeriodValue = "SessionParameterPeriodValue";
        private static string _tagGroupValue = "SessionParameterGroupValue";
        private static string _tagProductTitle = "SessionParameterProductTitle";
        //private static string _tagProductValue = "SessionParameterProductValue";
        //private static string _tagProductCompetitor = "SessionParameterProductCompetitor";
        //private static string _tagProductPrincipal = "SessionParameterProductPrincipal";
        //private static string _tagGroupTitle = "SessionParameterGroupTitle";
        private static string _tagLvl1Col1 = "SessionParameterLvl1Col1";
        private static string _tagLvl1Col2 = "SessionParameterLvl1Col2";
        private static string _tagLvl1Col3 = "SessionParameterLvl1Col3";
        private static string _tagLvl2Col1 = "SessionParameterLvl2Col1";
        private static string _tagLvl2Col2 = "SessionParameterLvl2Col2";
        private static string _tagLvl2Col3 = "SessionParameterLvl2Col3";
        private static string _tagLvl3Col1 = "SessionParameterLvl3Col1";
        private static string _tagLvl3Col2 = "SessionParameterLvl3Col2";
        private static string _tagLvl3Col3 = "SessionParameterLvl3Col3";
        private static string _tagCheckBox = "SessionParameterCheckBox";
        private static string _tagCheckBoxNotChecked = "SessionParameterCheckBoxNotChecked";
        #endregion

        #region SessionParameter
        /// <summary>
        /// Session parameter design
        /// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style)
        {

            #region Variables
            int nbMaxRowByPage = 42;
            int s = 1;
            int cellRow = 5;
            int startIndex = cellRow;
            int header = 1;
            int upperLeftColumn = 5;
            Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
            Cells cells = sheet.Cells;
            string vPageBreaks = "";
            double columnWidth = 0, indexLogo = 0, index;
            bool verif = true;
            object[] param = new object[2];
            param[0] = webSession.Source;
            param[1] = webSession.DataLanguage;
            IClassificationLevelListDALFactory classificationLevelListDALFactory = (IClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList].AssemblyName, WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            #endregion

            #region Title
            //SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1752,webSession.SiteLanguage),cellRow-1,1,false,Color.White,8,2);
            cells[cellRow - 1, 1].PutValue(GestionWeb.GetWebWord(1752, webSession.SiteLanguage));
            TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagTitle), null, cellRow - 1, 1, 3, false);
            cellRow += 2;
            #endregion

            #region Period
            cells[cellRow - 1, 1].PutValue(GestionWeb.GetWebWord(1755, webSession.SiteLanguage) + " :");
            TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagPeriodTitle), null, cellRow - 1, 1, 3, false);
            cellRow++;
            TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagPeriodValue), HtmlFunctions.GetPeriodDetailForExcel(webSession), cellRow - 1, 1, 1, false);
            cells[cellRow - 1, 1].Style.IndentLevel = 1;
            cellRow += 2;
            #endregion

            #region Concurrent
            if (webSession.SelectionUniversMedia != null && webSession.SelectionUniversMedia.Nodes.Count > 0)
            {
                cells[cellRow - 1, 1].PutValue(GestionWeb.GetWebWord(1678, webSession.SiteLanguage) + " :");
                TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagProductTitle), null, cellRow - 1, 1, 3, false);
                cellRow++;
                cellRow += 2;
                TNS.AdExpress.Anubis.Tefnout.UI.SessionParameter.ToExcel(webSession.SelectionUniversMedia, excel, webSession, sheet, ref cellRow, cells, style, classificationLevelListDALFactory);

                cellRow++;
            }
            #endregion

            #region Products
            if (webSession.SelectionUniversProduct != null && webSession.SelectionUniversProduct.Nodes.Count > 0)
            {
                cells[cellRow - 1, 1].PutValue(GestionWeb.GetWebWord(1759, webSession.SiteLanguage) + " :");
                TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagProductTitle), null, cellRow - 1, 1, 3, false);
                cellRow++;
                cellRow += 2;
                TNS.AdExpress.Anubis.Tefnout.UI.SessionParameter.ToExcel(webSession.SelectionUniversProduct, excel, webSession, sheet, ref cellRow, cells, style, classificationLevelListDALFactory);

                cellRow++;
            }
            #endregion

            #region Detail levels
            string detailstr = "";
            if (webSession.GenericMediaDetailLevel != null && webSession.GenericMediaDetailLevel.Levels.Count > 0)
            {

                cells[cellRow - 1, 1].PutValue(GestionWeb.GetWebWord(2871, webSession.SiteLanguage) + " :");
                TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagPeriodTitle), null, cellRow - 1, 1, 3, false);
                cellRow++;

                int nbLevels = webSession.GenericMediaDetailLevel.Levels.Count;
                 detailstr = "";
                for (int k = 1; k <= nbLevels; k++)
                {
                    detailstr += GestionWeb.GetWebWord(webSession.GenericMediaDetailLevel[k].WebTextId, webSession.SiteLanguage)+"/";
                }
                if (detailstr.Length > 0) detailstr = detailstr.Substring(0,detailstr.Length - 1);
                TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagPeriodValue), detailstr, cellRow - 1, 1, 1, false);
                cells[cellRow - 1, 1].Style.IndentLevel = 1;
                cellRow += 2;

                cellRow++;
            }
            #endregion

            #region Personnalized level
          

                cells[cellRow - 1, 1].PutValue(GestionWeb.GetWebWord(1896, webSession.SiteLanguage) + " :");
                TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagPeriodTitle), null, cellRow - 1, 1, 3, false);
                cellRow++;

                DetailLevelItemInformation persoMevel = DetailLevelItemsInformation.Get(webSession.PersonnalizedLevel);
                detailstr = GestionWeb.GetWebWord(persoMevel.WebTextId, webSession.SiteLanguage) ;                               
                TefnoutFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagPeriodValue), detailstr, cellRow - 1, 1, 1, false);
                cells[cellRow - 1, 1].Style.IndentLevel = 1;
                cellRow += 2;

                cellRow++;
           
            #endregion


            #region Mise en forme
            //Ajustement de la taile des cellules en fonction du contenu
            for (int c = 1; c <= 3; c++)
            {
                sheet.AutoFitColumn(c, 5, 50);
                cells.SetColumnWidth((byte)c, cells.GetColumnWidth((byte)c) + 10);
            }

            for (index = 0; index < 30; index++)
            {
                columnWidth += cells.GetColumnWidth((byte)index);
                if ((columnWidth < 125) && verif)
                    indexLogo++;
                else
                    verif = false;
            }

            upperLeftColumn = (int)indexLogo - 1;

            vPageBreaks = cells[cellRow, (int)indexLogo].Name;
            TefnoutFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1752, webSession.SiteLanguage), cellRow + 17, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), style);
            #endregion
        }
        #endregion

        #region Affichage d'un arbre pour Excel
        /// <summary>
        /// Affichage d'un arbre pour l'export Excel
        /// </summary>
        /// <param name="root">Arbre</param>
        public static void ToExcel(TreeNode root, Workbook excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells, TNS.FrameWork.WebTheme.Style style, IClassificationLevelListDALFactory classificationLevelListDALFactory)
        {
            int maxLevel = 0;
            GetNbLevels(root, 1, ref maxLevel);
            int nbTD = 1;
            TNS.AdExpress.Anubis.Tefnout.UI.SessionParameter.ToExcel(root, 0, maxLevel - 1, ref nbTD, excel, webSession, sheet, ref cellRow, cells, style, classificationLevelListDALFactory);
        }

        /// <summary>
        /// Donne le nombre de niveau d'un arbre
        /// </summary>
        /// <param name="root">Arbre</param>
        /// <param name="level">Niveau de l'arbre</param>
        /// <param name="max">Nombre maximum de niveau</param>
        private static void GetNbLevels(TreeNode root, int level, ref int max)
        {
            if (max < level) max = level;
            foreach (TreeNode currentNode in root.Nodes)
            {
                GetNbLevels(currentNode, level + 1, ref max);
            }
        }
        /// <summary>
        /// Met le style d'une cellule selon le niveau de l'arbre
        /// </summary>
        /// <param name="level">Niveau de l'arbre</param>
        private static void SetLevelStyle(int level, Workbook excel, Worksheet sheet, int cellRow, Cells cells, int nbTD, TNS.FrameWork.WebTheme.Style style)
        {
            switch (level)
            {
                case 1:
                    {
                        //cells.Merge(cellRow-1,1,1,3);
                        if (nbTD == 1)
                        {
                            style.GetTag(_tagLvl1Col1).SetStyleExcel(excel, cells, cellRow - 1, 1);
                            style.GetTag(_tagLvl1Col2).SetStyleExcel(excel, cells, cellRow - 1, 2);
                            style.GetTag(_tagLvl1Col3).SetStyleExcel(excel, cells, cellRow - 1, 3);
                        }
                        //					else if(nbTD==2){
                        //						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,2,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,8,false);
                        //						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,3,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,8,false);
                        //					}
                        //					else
                        //						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,3,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,8,false);

                        break;
                    }
                case 2:
                    {
                        style.GetTag(_tagLvl2Col1).SetStyleExcel(excel, cells, cellRow - 1, 1);
                        style.GetTag(_tagLvl2Col2).SetStyleExcel(excel, cells, cellRow - 1, 2);
                        style.GetTag(_tagLvl2Col3).SetStyleExcel(excel, cells, cellRow - 1, 3);
                        break;
                    }
                case 3:
                    {
                        style.GetTag(_tagLvl3Col1).SetStyleExcel(excel, cells, cellRow - 1, 1);
                        style.GetTag(_tagLvl3Col2).SetStyleExcel(excel, cells, cellRow - 1, 2);
                        style.GetTag(_tagLvl3Col3).SetStyleExcel(excel, cells, cellRow - 1, 3);
                        break;
                    }
                //case 3:
                //				default:
                //					return("Level1");
            }
        }
        /// <summary>
        /// Affichage d'un arbre pour l'export Excel
        /// </summary>
        /// <param name="root">Arbre</param>
        /// <param name="level">Niveau de l'arbre</param>
        /// <param name="maxLevel">Nombre maximum de niveaud e l'arbre</param>
        /// <param name="nbTD">Nombre de cellule TD</param>
        /// <returns>True si le nombre maximum de TD a été atteint, sinon false</returns>
        /// <remarks>
        /// - Actuellement, la méthode gère 3 niveaux d'affichage mais elle est générique.
        /// Par conséquent, 3 styles sont définis. Il est possible de rajouter des niveaux de style CSS dans le 'switch case' correspondant
        /// dans la méthode ci-après et ajouter les niveaux dans la méthode GetLevelCss(int level)
        /// - Affichage sur 3 colonnes dans le dernier niveau
        /// </remarks>
        private static bool ToExcel(TreeNode root, int level, int maxLevel, ref int nbTD, Workbook excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells, TNS.FrameWork.WebTheme.Style style, IClassificationLevelListDALFactory classificationLevelListDALFactory)
        {

            #region Variables
            string TagCheckedName = _tagCheckBox;
            #endregion

            #region Checkbox
            // Non cocher
            if (!root.Checked)
            {
                TagCheckedName = _tagCheckBoxNotChecked;
            }
            // Cocher
            else if (root.Checked)
            {
                TagCheckedName = _tagCheckBox;
            }
            #endregion

            if (root.Tag != null)
            {
                // Si on est dans le dernier niveau de l'arbre
                if (level == maxLevel)
                {
                    // Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
                    style.GetTag(TagCheckedName).SetStyleExcel(sheet, cellRow - 1, nbTD);
                    ClassificationLevelListDAL classificationLevelListDAL = classificationLevelListDALFactory.CreateClassificationLevelListDAL(((LevelInformation)root.Tag).Type, ((LevelInformation)root.Tag).ID.ToString(), WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.promo03).Label);
                    cells[cellRow - 1, nbTD].PutValue(classificationLevelListDAL[((LevelInformation)root.Tag).ID]);
                    style.GetTag(_tagGroupValue).SetStyleExcel(excel, cells, cellRow - 1, nbTD);
                    SetLevelStyle(level, excel, sheet, cellRow, cells, nbTD, style);
                    cells[cellRow - 1, nbTD].Style.IndentLevel = 2;
                }
                else
                {
                    // Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
                    if (level != 0)
                    {
                        style.GetTag(TagCheckedName).SetStyleExcel(sheet, cellRow - 1, 1);
                        ClassificationLevelListDAL classificationLevelListDAL = classificationLevelListDALFactory.CreateClassificationLevelListDAL(((LevelInformation)root.Tag).Type, ((LevelInformation)root.Tag).ID.ToString(), WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.promo03).Label);
                        cells[cellRow - 1, 1].PutValue(classificationLevelListDAL[((LevelInformation)root.Tag).ID]);
                        style.GetTag(_tagGroupValue).SetStyleExcel(excel, cells, cellRow - 1, 1);
                        SetLevelStyle(level, excel, sheet, cellRow, cells, nbTD, style);
                        cells[cellRow - 1, 1].Style.IndentLevel = 2;
                    }
                    // On prépare l'affichage du dernier niveau de l'arbre (nouvelle ligne) si on affiche le père d'une feuille
                    if (level < maxLevel)
                        cellRow++;
                }
            }
            // Boucle sur chaque noeud de l'arbre
            foreach (TreeNode currentNode in root.Nodes)
            {
                // Si le niveau inférieur indique qu'il faut changer de ligne et que la demande n'a pas été faite par le dernier fils
                //ToExcel(currentNode,level+1,maxLevel,ref nbTD,excel,webSession,sheet,ref cellRow,cells);
                if (ToExcel(currentNode, level + 1, maxLevel, ref nbTD, excel, webSession, sheet, ref cellRow, cells, style, classificationLevelListDALFactory) && currentNode != root.LastNode)
                {
                    cellRow++;
                }
                /*
				if((currentNode==root.LastNode)&&(level==maxLevel-1)){
					for(int i=1;i<4;i++){
						cells[cellRow-1,i].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells[cellRow-1,i].Style.Borders[BorderType.BottomBorder].Color = Color.FromArgb(100,72,131);}
				}*/
            }
            //On est dans le niveau père des feuilles et il a des fils on fait les bordures
            if (level == maxLevel - 1 && root.Nodes.Count > 0)
            {
                // Bordure en bas et bordure à droite
                //html.Append("<tr><td colspan=4 class="+GetLevelCss(level+1)+" style=\"border-bottom:solid 1px "+BORDER_COLOR+";border-right:solid 1px "+BORDER_COLOR+";height:5px;font-size:5px;\">&nbsp;</td></tr>");
                nbTD = 1;
                cellRow++;
            }
            // Si on est dans le dernier niveau de l'arbre
            if (level == maxLevel)
            {
                // On test si on est dans la dernière colonne, 
                // On affiche une cellule vide pour faire la bordure de droite
                // On prépare le changement de ligne commander au niveau supperieur par le return true
                if (nbTD == 3)
                {
                    nbTD = 1;
                    return (true);
                }
                nbTD++;
            }
            // On indique au niveau suppérieur que l'on ne doit pas changer de ligne
            return (false);
        }

        #endregion

    }
}
