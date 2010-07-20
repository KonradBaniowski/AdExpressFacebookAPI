#region Information
//Authors: Y. Rkaina
//Date of Creation: 05/02/2007
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

using TNS.AdExpress.Anubis.Amset;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using AmsetExceptions=TNS.AdExpress.Anubis.Amset.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using AmsetFunctions=TNS.AdExpress.Anubis.Amset.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Constantes.Customer;
using CustomerConstantes = TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using Oracle.DataAccess.Client;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Anubis.Amset.UI{
	/// <summary>
	/// Description résumée de SessionParameter.
	/// </summary>
	public class SessionParameter{

        #region Variables Theme Name
        private static string _tagTitle = "SessionParameterTitle";
        private static string _tagPeriodTitle = "SessionParameterPeriodTitle";
        private static string _tagPeriodValue = "SessionParameterPeriodValue";
        private static string _tagWaveTitle = "SessionParameterWaveTitle";
        private static string _tagWaveValue = "SessionParameterWaveValue";
        private static string _tagTargetTitle = "SessionParameterTargetTitle";
        private static string _tagTargetValue = "SessionParameterTargetValue";
        private static string _tagProductTitle = "SessionParameterProductTitle";
        private static string _tagProductValue = "SessionParameterProductValue";
        private static string _tagProductCompetitor = "SessionParameterProductCompetitor";
        private static string _tagProductPrincipal = "SessionParameterProductPrincipal";
        private static string _tagGroupTitle = "SessionParameterGroupTitle";
        private static string _tagGroupValue = "SessionParameterGroupValue";
        private static string _tagLvl1Col1 = "SessionParameterLvl1Col1";
        private static string _tagLvl1Col2 = "SessionParameterLvl1Col2";
        private static string _tagLvl1Col3 = "SessionParameterLvl1Col3";
        private static string _tagLvl2Col1 = "SessionParameterLvl2Col1";
        private static string _tagLvl2Col2 = "SessionParameterLvl2Col2";
        private static string _tagLvl2Col3 = "SessionParameterLvl2Col3";
        private static string _tagCheckBox = "SessionParameterCheckBox";
        private static string _tagCheckBoxNotChecked = "SessionParameterCheckBoxNotChecked";
        #endregion

		#region SessionParameter
		/// <summary>
		/// Session parameter design
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style) {

            #region variables
            int nbMaxRowByPage=42;
			int s=1;
			int cellRow = 5;
			int startIndex=cellRow;	
			int header=1;
			int upperLeftColumn=5;
			Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
			Cells cells = sheet.Cells;
			string vPageBreaks="";
			double columnWidth=0,indexLogo=0,index;
			bool verif=true;
            #endregion

            try {

				#region Title
                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagTitle), GestionWeb.GetWebWord(1752, webSession.SiteLanguage), cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagTitle), null, cellRow - 1, 1, 3, false);
				cellRow+=2;
				#endregion

				#region Period
                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagPeriodTitle), GestionWeb.GetWebWord(1755, webSession.SiteLanguage) + " :", cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagPeriodTitle), null, cellRow - 1, 1, 3, false);
				cellRow++;
                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagPeriodValue), HtmlFunctions.GetPeriodDetailForExcel(webSession), cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagPeriodValue), null, cellRow - 1, 1, 1, false);
				cells[cellRow-1,1].Style.IndentLevel=1;
				cellRow+=2;
				#endregion

				#region Wave
                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagWaveTitle), GestionWeb.GetWebWord(1771, webSession.SiteLanguage) + " :", cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagWaveTitle), null, cellRow - 1, 1, 3, false);
				cellRow++;
                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagWaveValue), ((LevelInformation)webSession.SelectionUniversAEPMWave.Nodes[0].Tag).Text, cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagWaveValue), null, cellRow - 1, 1, 1, false);
				cells[cellRow-1,1].Style.IndentLevel=1;
				cellRow+=2;
				#endregion

				#region Targets

                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagTargetTitle), GestionWeb.GetWebWord(1757, webSession.SiteLanguage) + " :", cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagTargetTitle), null, cellRow - 1, 1, 3, false);
				cellRow++;

				//Base target
				string targets = "'" + webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CustomerConstantes.Right.type.aepmTargetAccess) + "'";
				//Wave
				string idWave = ((LevelInformation)webSession.SelectionUniversAEPMWave.Nodes[0].Tag).ID.ToString();
				DataSet ds = TargetListDataAccess.GetAEPMTargetListFromIDSDataAccess(idWave, targets, webSession.CustomerLogin.Source);//.OracleConnectionString

				foreach(DataRow r in ds.Tables[0].Rows){
                    AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagTargetValue), r["target"].ToString(), cellRow - 1, 1, 2);
                    AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagTargetValue), null, cellRow - 1, 1, 1, false);
					cells[cellRow-1,1].Style.IndentLevel=1;
					cellRow++;
				}
				ds.Dispose();
				ds = null;
				cellRow++;
				#endregion

				#region Products
                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagProductTitle), GestionWeb.GetWebWord(1759, webSession.SiteLanguage) + " :", cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagProductTitle), null, cellRow - 1, 1, 3, false);
				cellRow++;
				//reference
                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagProductValue), GestionWeb.GetWebWord(1677, webSession.SiteLanguage) + " :", cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagProductValue), GestionWeb.GetWebWord(1677, webSession.SiteLanguage) + " :", cellRow - 1, 1, 1, false);
				cellRow+=2;			

				if(webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count>0)
					TNS.AdExpress.Anubis.Amset.UI.SessionParameter.ToExcel(webSession.PrincipalProductUniverses[0], excel, webSession, sheet, ref cellRow, cells, webSession.CustomerLogin.Source,style);//.OracleConnectionString

				cellRow++;

                AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_tagGroupTitle), GestionWeb.GetWebWord(1668, webSession.SiteLanguage) + " :", cellRow - 1, 1, 2);
                AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagGroupValue), null, cellRow - 1, 1, 1, false);
				cellRow+=2;

				ds = GroupSystem.ListFromSelection(dataSource, webSession);
				for(int i =0; i < ds.Tables[0].Rows.Count; i++){
					cells.Merge(cellRow-1,1,1,3);
                    cells[cellRow - 1, 1].PutValue(ds.Tables[0].Rows[i][0].ToString());
                    AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagProductPrincipal), null, cellRow - 1, 1, 3, false);
					cellRow++;
				}
				ds.Dispose();
				ds = null;
				#endregion

				//Ajustement de la taile des cellules en fonction du contenu
				for(int c=1;c<=3;c++){
					sheet.AutoFitColumn(c,5,50);
					cells.SetColumnWidth((byte)c,cells.GetColumnWidth((byte)c)+10);
				}		

				for(index=0;index<30;index++){
					columnWidth += cells.GetColumnWidth((byte)index);
					if((columnWidth<125)&&verif)
						indexLogo++;
					else
						verif=false;
				}

				upperLeftColumn=(int)indexLogo-1;

				vPageBreaks = cells[cellRow,(int)indexLogo].Name;
                AmsetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1752, webSession.SiteLanguage), cellRow + 17, nbMaxRowByPage, upperLeftColumn, vPageBreaks, header.ToString(), webSession.SiteLanguage, style);
			}
			catch(Exception e){
				throw(new AmsetExceptions.AmsetExcelSystemException("Unable to build the session parameter page.",e));
			}
		}
		#endregion

		#region Affichage d'un arbre pour Excel 
		/// <summary>
		/// Affichage d'un arbre pour l'export Excel
		/// </summary>
		/// <param name="root">Arbre</param>
        public static void ToExcel(TreeNode root, Workbook excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells, TNS.FrameWork.WebTheme.Style style) {
			int maxLevel=0;
			GetNbLevels(root,1,ref maxLevel);
			int nbTD=1;
            TNS.AdExpress.Anubis.Amset.UI.SessionParameter.ToExcel(root, 0, maxLevel - 1, ref nbTD, excel, webSession, sheet, ref cellRow, cells, style);
		}

		/// <summary>
		/// Donne le nombre de niveau d'un arbre
		/// </summary>
		/// <param name="root">Arbre</param>
		/// <param name="level">Niveau de l'arbre</param>
		/// <param name="max">Nombre maximum de niveau</param>
		private static void GetNbLevels(TreeNode root,int level,ref int max){
			if(max<level)max=level;
			foreach(TreeNode currentNode in root.Nodes){
				GetNbLevels(currentNode,level+1,ref max);
			}
		}
		/// <summary>
		/// Met le style d'une cellule selon le niveau de l'arbre
		/// </summary>
		/// <param name="level">Niveau de l'arbre</param>
        private static void SetLevelStyle(int level, Workbook excel, Worksheet sheet, int cellRow, Cells cells, int nbTD, TNS.FrameWork.WebTheme.Style style) {
			switch(level){
				case 1:
					if(nbTD==1){
                        style.GetTag(_tagLvl1Col1).SetStyleExcel(excel, cells, cellRow - 1, 1);
                        style.GetTag(_tagLvl1Col2).SetStyleExcel(excel, cells, cellRow - 1, 2);
                        style.GetTag(_tagLvl1Col3).SetStyleExcel(excel, cells, cellRow - 1, 3);
					}
					break;
				case 2:
					style.GetTag(_tagLvl2Col1).SetStyleExcel(excel, cells, cellRow - 1, 1);
                    style.GetTag(_tagLvl2Col2).SetStyleExcel(excel, cells, cellRow - 1, 2);
                    style.GetTag(_tagLvl2Col3).SetStyleExcel(excel, cells, cellRow - 1, 3);
					break;
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
        private static bool ToExcel(TreeNode root, int level, int maxLevel, ref int nbTD, Workbook excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells, TNS.FrameWork.WebTheme.Style style) {

            #region Variables
            string TagCheckedName = _tagCheckBox;
            #endregion

            #region Checkbox
            // Non cocher
            if (!root.Checked) {
                TagCheckedName = _tagCheckBoxNotChecked;
            }
            // Cocher
            else if (root.Checked) {
                TagCheckedName = _tagCheckBox;
            }
            #endregion

			// Si on est dans le dernier niveau de l'arbre
			if(level==maxLevel){ 
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
                style.GetTag(TagCheckedName).SetStyleExcel(sheet, cellRow - 1, nbTD);
                cells[cellRow - 1, nbTD].PutValue(((LevelInformation)root.Tag).Text);
                style.GetTag(_tagGroupValue).SetStyleExcel(excel, cells, cellRow - 1, nbTD);
                SetLevelStyle(level, excel, sheet, cellRow, cells, nbTD, style);
				cells[cellRow-1,nbTD].Style.IndentLevel = 2;
			}
			else{
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
				if(level!=0){
                    style.GetTag(TagCheckedName).SetStyleExcel(sheet, cellRow - 1, 1);
                    cells[cellRow - 1, 1].PutValue(((LevelInformation)root.Tag).Text);
                    style.GetTag(_tagGroupValue).SetStyleExcel(excel, cells, cellRow - 1, 1);
                    SetLevelStyle(level, excel, sheet, cellRow, cells, nbTD, style);
					cells[cellRow-1,1].Style.IndentLevel = 2;
				}
				// On prépare l'affichage du dernier niveau de l'arbre (nouvelle ligne) si on affiche le père d'une feuille
				if(level==maxLevel-1) 
					cellRow++;
			}
			// Boucle sur chaque noeud de l'arbre
			foreach(TreeNode currentNode in root.Nodes){
				// Si le niveau inférieur indique qu'il faut changer de ligne et que la demande n'a pas été faite par le dernier fils
				//ToExcel(currentNode,level+1,maxLevel,ref nbTD,excel,webSession,sheet,ref cellRow,cells);
                if (ToExcel(currentNode, level + 1, maxLevel, ref nbTD, excel, webSession, sheet, ref cellRow, cells, style) && currentNode != root.LastNode) {
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
			if(level==maxLevel-1 && root.Nodes.Count>0){
				// Bordure en bas et bordure à droite
				nbTD=1;
				cellRow++;
			}
			// Si on est dans le dernier niveau de l'arbre
			if(level==maxLevel){ 
				// On test si on est dans la dernière colonne, 
				// On affiche une cellule vide pour faire la bordure de droite
				// On prépare le changement de ligne commander au niveau supperieur par le return true
				if(nbTD==3){
					nbTD=1;
					return(true);
				}
				nbTD++;
			}
			// On indique au niveau suppérieur que l'on ne doit pas changer de ligne
			return(false);
		}

		/// <summary>
		/// Recall AdExpress Universe selection into excel sheet
		/// </summary>
		/// <param name="adExpressUniverse">AdExpress Universe</param>
		/// <param name="excel">excel</param>
		/// <param name="webSession">web session</param>
		/// <param name="sheet">sheet</param>
		/// <param name="cellRow">cell row</param>
		/// <param name="cells">cells</param>
		/// <param name="connection">connection</param>
        public static void ToExcel(TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse, Workbook excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style) {//OracleConnection connection
			List<NomenclatureElementsGroup> groups = null;
		
			if (adExpressUniverse != null && adExpressUniverse.Count() > 0) {
				//Groups of items excludes
				groups = adExpressUniverse.GetExludes();
				if (groups != null && groups.Count > 0) {
                    SetUniverseGroups(groups, excel, sheet, ref cellRow, cells, dataSource, AccessType.excludes, webSession.SiteLanguage, style);
					cellRow++;
				}

				//Groups of items includes
				groups = adExpressUniverse.GetIncludes();
				if (groups != null && groups.Count > 0) {
                    SetUniverseGroups(groups, excel, sheet, ref cellRow, cells, dataSource, AccessType.includes, webSession.SiteLanguage, style);
					cellRow++;
				}
			}
		}

		/// <summary>
		/// Set universes group's items into excel sheet
		/// </summary>
		/// <param name="groups">universes groups</param>
		/// <param name="excel">excel</param>
		/// <param name="sheet">sheet</param>
		/// <param name="cellRow">cell row</param>
		/// <param name="cells">cells</param>
		/// <param name="connection">DB connection</param>
		/// <param name="accessType">access type (includes, excludes)</param>
		/// <param name="language">language</param>
        private static void SetUniverseGroups(List<NomenclatureElementsGroup> groups, Workbook excel, Worksheet sheet, ref int cellRow, Cells cells, IDataSource dataSource, AccessType accessType, int language, TNS.FrameWork.WebTheme.Style style) {
			
			int nbTD = 1;
			TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess universeItems = null;
			int code = (accessType == AccessType.includes) ? 2281 : 2282;
			int level = 1;
			ArrayList itemIdList = null;

			if (groups != null && groups.Count > 0) {
				for (int i = 0; i < groups.Count; i++) {
					List<long> levelIdsList = groups[i].GetLevelIdsList();
					if (i > 0 && accessType == AccessType.includes) code = 2368;

					//Group title
                    AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_tagGroupTitle), GestionWeb.GetWebWord(code, language) + " :", cellRow, 1, 1, false);
					cellRow += 2;

					//For each group's level
					if (levelIdsList != null && levelIdsList.Count>0) {
						for (int j = 0; j < levelIdsList.Count; j++) {
							
							

							//Show all level items											
							universeItems = new TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess(UniverseLevels.Get(levelIdsList[j]).TableName, groups[i].GetAsString(levelIdsList[j]), language, dataSource);
							if (universeItems != null) {
								
								itemIdList = universeItems.IdListOrderByClassificationItem;
								if (itemIdList != null && itemIdList.Count > 0) {

									//Level label
									level = 1;
									nbTD = 1;

                                    cells[cellRow, 1].PutValue(GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, language));
                                    style.GetTag(_tagGroupValue).SetStyleExcel(excel, cells, cellRow, 1);
                                    SetLevelStyle(level, excel, sheet, cellRow + 1, cells, nbTD, style);
									cells[cellRow, 1].Style.IndentLevel = 2;
									cellRow++;

									level = 2;
                                    for (int k = 0; k < itemIdList.Count; k++) {
                                        //Add item label										
                                        style.GetTag(_tagCheckBox).SetStyleExcel(sheet, cellRow, nbTD);

                                        cells[cellRow, nbTD].PutValue(universeItems[Int64.Parse(itemIdList[k].ToString())]);
                                        style.GetTag(_tagGroupValue).SetStyleExcel(excel, cells, cellRow, 1);
                                        SetLevelStyle(level, excel, sheet, cellRow + 1, cells, nbTD, style);
                                        cells[cellRow, nbTD].Style.IndentLevel = 2;	//cellRow - 1									
                                        nbTD++;
                                        if (nbTD > 3) {//row changing
                                            nbTD = 1;
                                            cellRow++;
                                        }
                                    }
								}
							}

						}
					}
					cellRow++;
				}
			}

		}
		#endregion

	}
}
