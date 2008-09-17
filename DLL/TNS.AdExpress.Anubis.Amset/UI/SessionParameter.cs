#region Information
//Authors: Y. Rkaina
//Date of Creation: 05/02/2007
//Date of modification:
#endregion

using System;
using System.IO;
using Aspose.Excel;
using System.Drawing;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using TNS.AdExpress.Anubis.Amset;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using AmsetExceptions=TNS.AdExpress.Anubis.Amset.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using AmsetFunctions=TNS.AdExpress.Anubis.Amset.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Constantes.Customer;
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

		#region SessionParameter
		/// <summary>
		/// Session parameter design
		/// </summary>
		internal static void SetExcelSheet(Excel excel,WebSession webSession,IDataSource dataSource){

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

			try{

				#region Title
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1752,webSession.SiteLanguage),cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,11,false);
				cellRow+=2;
				#endregion

				#region Period
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1755,webSession.SiteLanguage)+" :",cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,10,false);
				cellRow++;
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,HtmlFunctions.GetPeriodDetailForExcel(webSession),cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				cells[cellRow-1,1].Style.IndentLevel=1;
				cellRow+=2;
				#endregion

				#region Wave
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1771,webSession.SiteLanguage)+" :",cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,10,false);
				cellRow++;
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((LevelInformation)webSession.SelectionUniversAEPMWave.Nodes[0].Tag).Text,cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				cells[cellRow-1,1].Style.IndentLevel=1;
				cellRow+=2;
				#endregion

				#region Targets

				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1757,webSession.SiteLanguage)+" :",cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,10,false);
				cellRow++;

				//Base target
				string targets = "'" + webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmTargetAccess) + "'";
				//Wave
				string idWave = ((LevelInformation)webSession.SelectionUniversAEPMWave.Nodes[0].Tag).ID.ToString();
				DataSet ds = TargetListDataAccess.GetAEPMTargetListFromIDSDataAccess(idWave, targets, webSession.CustomerLogin.OracleConnectionString);

				foreach(DataRow r in ds.Tables[0].Rows){
					AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,r["target"].ToString(),cellRow-1,1,false,8,2);
					AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
					cells[cellRow-1,1].Style.IndentLevel=1;
					cellRow++;
				}
				ds.Dispose();
				ds = null;
				cellRow++;
				#endregion

				#region Products
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1759,webSession.SiteLanguage)+" :",cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,10,false);
				cellRow++;
				//reference
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1677,webSession.SiteLanguage)+" :",cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				cellRow+=2;
				
				#region Ancienne version
				//TNS.AdExpress.Anubis.Amset.UI.SessionParameter.ToExcel(webSession.CurrentUniversAdvertiser,excel,webSession,sheet,ref cellRow,cells);
				#endregion

				if(webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count>0)
					TNS.AdExpress.Anubis.Amset.UI.SessionParameter.ToExcel(webSession.PrincipalProductUniverses[0], excel, webSession, sheet, ref cellRow, cells, new OracleConnection(webSession.CustomerLogin.OracleConnectionString));

				cellRow++;
			
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1668, webSession.SiteLanguage)+" :",cellRow-1,1,false,8,2);
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(222,216,229),CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				cellRow+=2;

				ds = GroupSystem.ListFromSelection(dataSource, webSession);
				for(int i =0; i < ds.Tables[0].Rows.Count; i++){
					cells.Merge(cellRow-1,1,1,3);
					AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,ds.Tables[0].Rows[i][0].ToString(),cellRow-1,1,false,8,2);
					AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,10,false);
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
				AmsetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1752,webSession.SiteLanguage),cellRow+17,nbMaxRowByPage,upperLeftColumn,vPageBreaks,header.ToString());
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
		public static void ToExcel(TreeNode root, Excel excel, WebSession webSession,Worksheet sheet,ref int cellRow,Cells cells){
			int maxLevel=0;
			GetNbLevels(root,1,ref maxLevel);
			int nbTD=1;
			TNS.AdExpress.Anubis.Amset.UI.SessionParameter.ToExcel(root,0,maxLevel-1,ref nbTD,excel,webSession,sheet,ref cellRow,cells);
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
		private static void SetLevelStyle(int level, Excel excel, Worksheet sheet,int cellRow,Cells cells,int nbTD){
			switch(level){
				case 1:
					if(nbTD==1){
						AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,8,false);
						AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,2,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,8,false);
						AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,3,3,true,Color.FromArgb(100,72,131),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.Thin,8,false);
					}
					break;
				case 2:
					AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,1,true,Color.FromArgb(100,72,131),Color.FromArgb(222,216,229),Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,8,false);
					AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,2,true,Color.FromArgb(100,72,131),Color.FromArgb(222,216,229),Color.FromArgb(100,72,131),CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
					AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,3,3,true,Color.FromArgb(100,72,131),Color.FromArgb(222,216,229),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
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
		private static bool ToExcel(TreeNode root, int level, int maxLevel, ref int nbTD, Excel excel, WebSession webSession, Worksheet sheet,ref int cellRow,Cells cells){

			#region Variables
			string img="";
			string imgPath="";
			Pictures pics = sheet.Pictures;
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
				pics.Add(cellRow-1,nbTD,imgPath);
				AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((LevelInformation)root.Tag).Text,cellRow-1,nbTD,false,8,2);
				SetLevelStyle(level,excel,sheet,cellRow,cells,nbTD);
				cells[cellRow-1,nbTD].Style.IndentLevel = 2;
			}
			else{
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
				if(level!=0){
					pics.Add(cellRow-1, 1,imgPath);
					AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((LevelInformation)root.Tag).Text,cellRow-1,1,false,8,2);
					SetLevelStyle(level,excel,sheet,cellRow,cells,nbTD);
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
				if(ToExcel(currentNode,level+1,maxLevel,ref nbTD,excel,webSession,sheet,ref cellRow,cells) && currentNode!=root.LastNode){
					cellRow++;
				}

				if((currentNode==root.LastNode)&&(level==maxLevel-1)){
					for(int i=1;i<4;i++){
						cells[cellRow-1,i].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells[cellRow-1,i].Style.Borders[BorderType.BottomBorder].Color = Color.FromArgb(100,72,131);}
				}
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
		public static void ToExcel(TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse, Excel excel, WebSession webSession, Worksheet sheet, ref int cellRow, Cells cells, OracleConnection connection) {
			List<NomenclatureElementsGroup> groups = null;
		
			if (adExpressUniverse != null && adExpressUniverse.Count() > 0) {
				//Groups of items excludes
				groups = adExpressUniverse.GetExludes();
				if (groups != null && groups.Count > 0) {
					SetUniverseGroups(groups, excel, sheet, ref cellRow, cells, connection, AccessType.excludes, webSession.SiteLanguage);
					cellRow++;
				}

				//Groups of items includes
				groups = adExpressUniverse.GetIncludes();
				if (groups != null && groups.Count > 0) {
					SetUniverseGroups(groups, excel, sheet, ref cellRow, cells, connection, AccessType.includes, webSession.SiteLanguage);
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
		private static void SetUniverseGroups(List<NomenclatureElementsGroup> groups, Excel excel, Worksheet sheet, ref int cellRow, Cells cells,OracleConnection connection, AccessType accessType, int language) {
			
			int nbTD = 1;
			TNS.AdExpress.Classification.DataAccess.ClassificationLevelListDataAccess universeItems = null;
			int code = (accessType == AccessType.includes) ? 2281 : 2282;
			int level = 1;
			Pictures pics = sheet.Pictures;
			string img = @"Images\checkbox.GIF";
			string imgPath = System.IO.Path.GetFullPath(img);
			ArrayList itemIdList = null;

			if (groups != null && groups.Count > 0) {
				for (int i = 0; i < groups.Count; i++) {
					List<long> levelIdsList = groups[i].GetLevelIdsList();
					if (i > 0 && accessType == AccessType.includes) code = 2368;

					//Group title
					AmsetFunctions.WorkSheet.PutCellValue(sheet, cells, GestionWeb.GetWebWord(code, language) + " :", cellRow, 1, false, 8, 2);//cellRow - 1	
					AmsetFunctions.WorkSheet.CellsStyle(cells, null, cellRow, 1, 1, true, Color.FromArgb(100, 72, 131), Color.White, Color.White, CellBorderType.None, CellBorderType.None, CellBorderType.None, CellBorderType.None, 8, false);	
					cellRow += 2;

					//For each group's level
					if (levelIdsList != null && levelIdsList.Count>0) {
						for (int j = 0; j < levelIdsList.Count; j++) {
							
							

							//Show all level items											
							universeItems = new TNS.AdExpress.Classification.DataAccess.ClassificationLevelListDataAccess(UniverseLevels.Get(levelIdsList[j]).TableName, groups[i].GetAsString(levelIdsList[j]), language, connection);
							if (universeItems != null) {
								
								itemIdList = universeItems.IdListOrderByClassificationItem;
								if (itemIdList != null && itemIdList.Count > 0) {
									//Level label
									level = 1;
									nbTD = 1;
									AmsetFunctions.WorkSheet.PutCellValue(sheet, cells, GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, language), cellRow, 1, false, 8, 2);//cellRow - 1	
									SetLevelStyle(level, excel, sheet, cellRow + 1, cells, nbTD);
									cells[cellRow, 1].Style.IndentLevel = 2;
									cellRow++;

									level = 2;
									for (int k = 0; k < itemIdList.Count; k++) {
										//Add item label										
										pics.Add(cellRow, nbTD, imgPath);//cellRow - 1	
										AmsetFunctions.WorkSheet.PutCellValue(sheet, cells, universeItems[Int64.Parse(itemIdList[k].ToString())], cellRow, nbTD, false, 8, 2);
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
