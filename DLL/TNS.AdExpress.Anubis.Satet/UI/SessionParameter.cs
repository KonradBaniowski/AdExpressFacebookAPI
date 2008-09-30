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

using TNS.AdExpress.Anubis.Satet;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using SatetExceptions=TNS.AdExpress.Anubis.Satet.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using SatetFunctions=TNS.AdExpress.Anubis.Satet.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using CsteCustomer=TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description r�sum�e de SessionParameter.
	/// </summary>
	public class SessionParameter
	{

		#region SessionParameter
		/// <summary>
		/// Session parameter design
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource) {

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

			#region Title
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1752,webSession.SiteLanguage),cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,11,false);
			cellRow+=2;
			#endregion

			#region Period
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1755,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,10,false);
			cellRow++;
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,HtmlFunctions.GetPeriodDetailForExcel(webSession),cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
			cells[cellRow-1,1].Style.IndentLevel=1;
			cellRow+=2;
			#endregion

			#region Wave
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1771,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,10,false);
			cellRow++;
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,((LevelInformation)webSession.SelectionUniversAEPMWave.Nodes[0].Tag).Text,cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
			cells[cellRow-1,1].Style.IndentLevel=1;
			cellRow+=2;
			#endregion

			#region Targets

			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1757,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,10,false);
			cellRow++;

			//Base target
            string targets = "'" + webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmTargetAccess) + "'";
			//Wave
			string idWave = ((LevelInformation)webSession.SelectionUniversAEPMWave.Nodes[0].Tag).ID.ToString();
			DataSet ds = TargetListDataAccess.GetAEPMTargetListFromIDSDataAccess(idWave, targets, webSession.Source);

			foreach(DataRow r in ds.Tables[0].Rows){
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,r["target"].ToString(),cellRow-1,1,false,Color.White,8,2);
				SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				cells[cellRow-1,1].Style.IndentLevel=1;
				cellRow++;
			}
			ds.Dispose();
			ds = null;
			#endregion

			#region Products
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1759,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,10,false);
			cellRow++;
			//reference
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1677,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
			cellRow+=2;
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                TNS.AdExpress.Anubis.Satet.UI.SessionParameter.ToExcel(webSession.PrincipalProductUniverses[0], excel, webSession, sheet, ref cellRow, cells, webSession.Source);
			//TNS.AdExpress.Anubis.Satet.UI.SessionParameter.ToExcel(((CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser,excel,webSession,sheet,ref cellRow,cells);

			cellRow++;
			
			//competitor

			if (webSession.PrincipalProductUniverses.Count>1){
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1678, webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
				SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				cellRow+=2;
			}
			else{
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1668, webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
				SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				cellRow+=2;
			
			}


			if (webSession.PrincipalProductUniverses.Count > 1) {
                TNS.AdExpress.Anubis.Satet.UI.SessionParameter.ToExcel(webSession.PrincipalProductUniverses[1], excel, webSession, sheet, ref cellRow, cells, webSession.Source);
				//TNS.AdExpress.Anubis.Satet.UI.SessionParameter.ToExcel(((CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[2]).TreeCompetitorAdvertiser,excel,webSession,sheet,ref cellRow,cells);
			}
			else{
				ds = GroupSystem.ListFromSelection(dataSource, webSession);
				for(int i =0; i < ds.Tables[0].Rows.Count; i++){
					cells.Merge(cellRow-1,1,1,3);
					SatetFunctions.WorkSheet.PutCellValue(sheet,cells,ds.Tables[0].Rows[i][0].ToString(),cellRow-1,1,false,Color.White,8,2);
					SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,10,false);
					cellRow++;
				}
				ds.Dispose();
				ds = null;
			}
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
			SatetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1752,webSession.SiteLanguage),cellRow+17,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks,header.ToString());
		}
		#endregion

		#region Affichage d'un arbre pour Excel 
		/// <summary>
		/// Affichage d'un arbre pour l'export Excel
		/// </summary>
		/// <param name="root">Arbre</param>
        public static void ToExcel(TreeNode root, Workbook excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells) {
			int maxLevel=0;
			GetNbLevels(root,1,ref maxLevel);
			int nbTD=1;
			TNS.AdExpress.Anubis.Satet.UI.SessionParameter.ToExcel(root,0,maxLevel-1,ref nbTD,excel,webSession,sheet,ref cellRow,cells);
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
        private static void SetLevelStyle(int level, Workbook excel, Worksheet sheet, int cellRow, Cells cells, int nbTD) {
			switch(level){
				case 1:{
					//cells.Merge(cellRow-1,1,1,3);
					if(nbTD==1){
						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,8,false);
						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,2,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,8,false);
						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,3,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,8,false);
					}
//					else if(nbTD==2){
//						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,2,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,8,false);
//						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,3,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,8,false);
//					}
//					else
//						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,3,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,8,false);

					break;
				}
				case 2:{
					SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.FromArgb(222,216,229),Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,8,false);
					SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,2,true,Color.FromArgb(100,72,131),Color.FromArgb(222,216,229),Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
					SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,3,3,true,Color.FromArgb(100,72,131),Color.FromArgb(222,216,229),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
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
		/// <returns>True si le nombre maximum de TD a �t� atteint, sinon false</returns>
		/// <remarks>
		/// - Actuellement, la m�thode g�re 3 niveaux d'affichage mais elle est g�n�rique.
		/// Par cons�quent, 3 styles sont d�finis. Il est possible de rajouter des niveaux de style CSS dans le 'switch case' correspondant
		/// dans la m�thode ci-apr�s et ajouter les niveaux dans la m�thode GetLevelCss(int level)
		/// - Affichage sur 3 colonnes dans le dernier niveau
		/// </remarks>
        private static bool ToExcel(TreeNode root, int level, int maxLevel, ref int nbTD, Workbook excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells) {

			#region Variables
			string img="";
			string imgPath="";
			Pictures pics = sheet.Pictures;
            int indexImage = 0;
			#endregion

			#region Checkbox
			// Non cocher
			if(!root.Checked){
				img=@"Images\checkbox_not_checked.GIF";
				imgPath = System.IO.Path.GetFullPath(img);
			}
				// Cocher
			else if(root.Checked){
				img=@"Images\checkbox.GIF";
				imgPath = System.IO.Path.GetFullPath(img);
			}
			#endregion

			// Si on est dans le dernier niveau de l'arbre
			if(level==maxLevel){ 
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
                indexImage = pics.Add(cellRow - 1, nbTD, imgPath, 100, 100);
                sheet.Pictures[indexImage].Placement = PlacementType.Move;
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,((LevelInformation)root.Tag).Text,cellRow-1,nbTD,false,Color.White,8,2);
				SetLevelStyle(level,excel,sheet,cellRow,cells,nbTD);
				cells[cellRow-1,nbTD].Style.IndentLevel = 2;
			}
			else{
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
				if(level!=0){
                    pics.Add(cellRow - 1, 1, imgPath, 100, 100);
                    sheet.Pictures[indexImage].Placement = PlacementType.Move;
					SatetFunctions.WorkSheet.PutCellValue(sheet,cells,((LevelInformation)root.Tag).Text,cellRow-1,1,false,Color.White,8,2);
					SetLevelStyle(level,excel,sheet,cellRow,cells,nbTD);
					cells[cellRow-1,1].Style.IndentLevel = 2;
				}
				// On pr�pare l'affichage du dernier niveau de l'arbre (nouvelle ligne) si on affiche le p�re d'une feuille
				if(level==maxLevel-1) 
					cellRow++;
			}
			// Boucle sur chaque noeud de l'arbre
			foreach(TreeNode currentNode in root.Nodes){
				// Si le niveau inf�rieur indique qu'il faut changer de ligne et que la demande n'a pas �t� faite par le dernier fils
				//ToExcel(currentNode,level+1,maxLevel,ref nbTD,excel,webSession,sheet,ref cellRow,cells);
				if(ToExcel(currentNode,level+1,maxLevel,ref nbTD,excel,webSession,sheet,ref cellRow,cells) && currentNode!=root.LastNode){
					cellRow++;
				}

				if((currentNode==root.LastNode)&&(level==maxLevel-1)){
					for(int i=1;i<4;i++){
						cells[cellRow-1,i].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells[cellRow-1,i].Style.Borders[BorderType.BottomBorder].Color = Color.FromArgb(100,72,131);}
				}
			}
			//On est dans le niveau p�re des feuilles et il a des fils on fait les bordures
			if(level==maxLevel-1 && root.Nodes.Count>0){
				// Bordure en bas et bordure � droite
				//html.Append("<tr><td colspan=4 class="+GetLevelCss(level+1)+" style=\"border-bottom:solid 1px "+BORDER_COLOR+";border-right:solid 1px "+BORDER_COLOR+";height:5px;font-size:5px;\">&nbsp;</td></tr>");
				nbTD=1;
				cellRow++;
			}
			// Si on est dans le dernier niveau de l'arbre
			if(level==maxLevel){ 
				// On test si on est dans la derni�re colonne, 
				// On affiche une cellule vide pour faire la bordure de droite
				// On pr�pare le changement de ligne commander au niveau supperieur par le return true
				if(nbTD==3){
					nbTD=1;
					return(true);
				}
				nbTD++;
			}
			// On indique au niveau supp�rieur que l'on ne doit pas changer de ligne
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
        public static void ToExcel(TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse, Workbook excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells, IDataSource source) {
			List<NomenclatureElementsGroup> groups = null;

			if (adExpressUniverse != null && adExpressUniverse.Count() > 0) {
				//Groups of items excludes
				groups = adExpressUniverse.GetExludes();
				if (groups != null && groups.Count > 0) {
					SetUniverseGroups(groups, excel, sheet, ref cellRow, cells, source, AccessType.excludes, webSession.SiteLanguage);
					cellRow++;
				}

				//Groups of items includes
				groups = adExpressUniverse.GetIncludes();
				if (groups != null && groups.Count > 0) {
					SetUniverseGroups(groups, excel, sheet, ref cellRow, cells, source, AccessType.includes, webSession.SiteLanguage);
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
        private static void SetUniverseGroups(List<NomenclatureElementsGroup> groups, Workbook excel, Worksheet sheet, ref int cellRow, Cells cells, IDataSource source, AccessType accessType, int language) {

			int nbTD = 1;
            TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess universeItems = null;
            

			int code = (accessType == AccessType.includes) ? 2281 : 2282;
			int level = 1;
			Pictures pics = sheet.Pictures;
			string img = @"Images\checkbox.GIF";
			string imgPath = System.IO.Path.GetFullPath(img);
			ArrayList itemIdList = null;
            int indexImage = 0;

			if (groups != null && groups.Count > 0) {
				for (int i = 0; i < groups.Count; i++) {
					List<long> levelIdsList = groups[i].GetLevelIdsList();

					if (i > 0 && accessType == AccessType.includes) code = 2368;

					//Group title
					SatetFunctions.WorkSheet.PutCellValue(sheet, cells, GestionWeb.GetWebWord(code, language) + " :", cellRow, 1, false, Color.White, 8, 2);//cellRow - 1	
					SatetFunctions.WorkSheet.CellsStyle(cells, null, cellRow, 1, 1, true, Color.FromArgb(100, 72, 131), Color.White, Color.White, CellBorderType.None, CellBorderType.None, CellBorderType.None, CellBorderType.None, 8, false);
					cellRow += 2;

					//For each group's level
					if (levelIdsList != null && levelIdsList.Count > 0) {
						for (int j = 0; j < levelIdsList.Count; j++) {

							

							//Show all level items											
                            universeItems = new TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess(UniverseLevels.Get(levelIdsList[j]).TableName, groups[i].GetAsString(levelIdsList[j]), language, source);
							if (universeItems != null) {
								
								itemIdList = universeItems.IdListOrderByClassificationItem;
								if (itemIdList != null && itemIdList.Count > 0) {
									
									//Level label
									level = 1;
									nbTD = 1;
									SatetFunctions.WorkSheet.PutCellValue(sheet, cells, GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, language), cellRow, 1, false, Color.White, 8, 2);//cellRow - 1	
									SetLevelStyle(level, excel, sheet, cellRow + 1, cells, nbTD);
									cells[cellRow, 1].Style.IndentLevel = 2;
									cellRow++;

									level = 2;
									for (int k = 0; k < itemIdList.Count; k++) {
										//Add item label										
                                        indexImage = pics.Add(cellRow, nbTD, imgPath, 100, 100);//cellRow - 1
                                        sheet.Pictures[indexImage].Placement = PlacementType.Move;
										SatetFunctions.WorkSheet.PutCellValue(sheet, cells, universeItems[Int64.Parse(itemIdList[k].ToString())], cellRow, nbTD, false, Color.White, 8, 2);
										SetLevelStyle(level, excel, sheet, cellRow + 1, cells, nbTD);
										cells[cellRow, nbTD].Style.IndentLevel = 2;	//cellRow - 1									
										if (k == (itemIdList.Count - 1)) {
											for (int n = 1; n < 4; n++) {
												cells[cellRow, n].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
												cells[cellRow, n].Style.Borders[BorderType.BottomBorder].Color = Color.FromArgb(100, 72, 131);
											}
										}
										nbTD++;
										if (nbTD > 3) {//row changing
											nbTD = 1;
											cellRow++;
										}
									}
									//Complete TD missing for one row
									if (nbTD == 2 || nbTD == 3) {
										SetLevelStyle(level, excel, sheet, cellRow + 1, cells, nbTD);
										cells[cellRow, nbTD].Style.IndentLevel = 2;

										if (nbTD == 2) {
											SetLevelStyle(level, excel, sheet, cellRow + 1, cells, 3);
											cells[cellRow, 3].Style.IndentLevel = 2;
										}
										for (int n = 1; n < 4; n++) {											
											cells[cellRow, n].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
											cells[cellRow, n].Style.Borders[BorderType.BottomBorder].Color = Color.FromArgb(100, 72, 131);
										}
										cellRow++;
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
