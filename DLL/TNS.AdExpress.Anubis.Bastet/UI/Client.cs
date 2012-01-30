#region Informations
// Auteur: D. V. Mussuma
// Date de création: 24/11/2005
// Date de modification:
#endregion

using System;
using System.Collections;

using Aspose.Cells;
using System.Drawing;
using System.Data;
using TNS.AdExpress.Anubis.Bastet;
using BastetCommon=TNS.AdExpress.Bastet.Common;
//using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.DB;
using BastetExceptions=TNS.AdExpress.Anubis.Bastet.Exceptions;
using BastetFunctions=TNS.AdExpress.Anubis.Bastet.Functions;
using BastetRules=TNS.AdExpress.Anubis.Bastet.Rules;
using TNS.FrameWork.Date;
using ConstantesTracking=TNS.AdExpress.Constantes.Tracking;
using TNS.AdExpress.Bastet.Translation;
using Aspose.Cells.Charts;


namespace TNS.AdExpress.Anubis.Bastet.UI
{
	/// <summary>
	/// Intégration des données dans fichier excel des Top  clients qui se connectent le plus
	/// </summary>
	public class Client
	{
		#region Top connections clients
		/// <summary>
		/// Top des clients qui se connectent le plus
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Workbook TopConnected(Workbook excel, BastetCommon.Parameters parameters, int language, DataAccess.Client dataAccessClient) {
			try{
								
				//Chargement des données
                DataTable dt = dataAccessClient.TopConnected();
													
				#region Intégration  données client
						
				if(dt!=null && dt.Rows.Count>0){
					//Insertion dans fichier excel
					int cellRow =4;
					int startIndex=cellRow;
					int[] iArray = null;
					string[] strArray ;	
					int upperLeftColumn=5;
					int minimunVPageBreaks=9;
					int s=1;
					int totalCompanyRow=0;
					Int64 oldIdCompany=0;
					const int nbMaxRowByPage=40;
					string vPageBreaks="";				
					Range range =null;			
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;					
							
					//libellés tranches horaires
					if(parameters!=null && parameters.Logins.Length==0){
						strArray = new string[] { GestionWeb.GetWebWord(1132, language), GestionWeb.GetWebWord(2485, language), "00h00-8h00", "8H00-12H00", "12H00-16h00", "16H00-20h00", "20h00-24h00", GestionWeb.GetWebWord(1401, language) }; 
						range = cells.CreateRange("A"+startIndex,"H"+startIndex);
						BastetFunctions.WorkSheet.CellsHeaderStyle(cells,null,3,0,7,true,Color.White);
					}
					else{
						strArray = new string[] { "00h00-8h00", "8H00-12H00", "12H00-16h00", "16H00-20h00", "20h00-24h00", GestionWeb.GetWebWord(1401, language) }; 
						range = cells.CreateRange("C"+startIndex,"H"+startIndex);
						BastetFunctions.WorkSheet.CellsHeaderStyle(cells,null,3,2,7,true,Color.White);
					}						
					cells.ImportObjectArray(strArray,range.FirstRow,range.FirstColumn,false);									
					range.SetOutlineBorder(BorderType.RightBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.TopBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.Black);

						

					cellRow++;

					//Tous les logins
					if(parameters!=null && parameters.Logins.Length==0){
		
						//Ei to Ji 	
						totalCompanyRow=cellRow;
				
						foreach(DataRow dr in  dt.Rows){
						
							//Connexions par login
							range = cells.CreateRange("C"+cellRow,"H"+cellRow);
							iArray = new int[]{int.Parse(dr["CONNECTION_NUMBER_24_8"].ToString()),int.Parse(dr["CONNECTION_NUMBER_8_12"].ToString()),int.Parse(dr["CONNECTION_NUMBER_12_16"].ToString()),int.Parse(dr["CONNECTION_NUMBER_16_20"].ToString()),
												  int.Parse(dr["CONNECTION_NUMBER_20_24"].ToString()),int.Parse(dr["CONNECTION_NUMBER"].ToString())};
							cells.ImportArray(iArray,range.FirstRow,range.FirstColumn,false);
							range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.Black);
							range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.Black);
							
							cells["A"+cellRow].PutValue(dr["COMPANY"].ToString());	
							cells["A"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["A"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
							cells["A"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["A"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["A"+cellRow].Style.Font.IsBold = true;

							cells["B"+cellRow].PutValue(dr["LOGIN"].ToString());
							cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;					
							cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							
							cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
							cells["F"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
							cells["G"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;							
							cells["H"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
										
							oldIdCompany = Int64.Parse(dr["ID_COMPANY"].ToString());
							cellRow++;
					
						}
						//Ajustement de la taile des cellules en fonction du contenu					
						
						//Total
						BastetFunctions.WorkSheet.PutCellValue(cells,GestionWeb.GetWebWord(1401, language), cellRow - 1, 0, true, Color.Red);
						BastetFunctions.WorkSheet.PutCellValue(cells,"",cellRow-1,1,true,Color.Red);
						BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_24_8)","").ToString()),cellRow-1,2,true,Color.Red);						
						BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_8_12)","").ToString()),cellRow-1,3,true,Color.Red);
						BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_12_16)","").ToString()),cellRow-1,4,true,Color.Red);
						BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_16_20)","").ToString()),cellRow-1,5,true,Color.Red);
						BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_20_24)","").ToString()),cellRow-1,6,true,Color.Red);
						BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER)","").ToString()),cellRow-1,7,true,Color.Red);
	
						#region Graphique

						int lastRow=cellRow;

						//Saut de page pour graphique
						cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

						//Ajout un graphique à la feuille excel				
						int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

						//Accede à l'instance d'un nouveau graphique
						Chart chart=sheet.Charts[chartIndex];
					
						//Construit le graphique
						BastetFunctions.BastetChart.Build(chart,"C"+lastRow+":G"+lastRow,"C"+startIndex+":G"+startIndex
							, GestionWeb.GetWebWord(2486, language), GestionWeb.GetWebWord(2487, language), GestionWeb.GetWebWord(2488, language), GestionWeb.GetWebWord(2489, language));

						#endregion
					}
					else{
						//Cas Logins sélectionnés

						cells["B" + cellRow].PutValue(GestionWeb.GetWebWord(2490, language));
						cells["B"+cellRow].Style.Font.Color = Color.Black;
						cells["B"+cellRow].Style.ForegroundColor = Color.Silver;
						cells["B" + cellRow].Style.Pattern = BackgroundType.Solid;
						cells["B"+cellRow].Style.Font.IsBold = true;
						cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;					
						cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;					
						cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						
						cells["C"+cellRow].PutValue(int.Parse(dt.Compute("sum(CONNECTION_NUMBER_24_8)","").ToString()));
						cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;					
						cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					
						cells["D"+cellRow].PutValue(int.Parse(dt.Compute("sum(CONNECTION_NUMBER_8_12)","").ToString()));
						cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;					
						cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

						cells["E"+cellRow].PutValue(int.Parse(dt.Compute("sum(CONNECTION_NUMBER_12_16)","").ToString()));
						cells["E"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;					
						cells["E"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

						cells["F"+cellRow].PutValue(int.Parse(dt.Compute("sum(CONNECTION_NUMBER_16_20)","").ToString()));
						cells["F"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;					
						cells["F"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						
						cells["G"+cellRow].PutValue(int.Parse(dt.Compute("sum(CONNECTION_NUMBER_20_24)","").ToString()));
						cells["G"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;					
						cells["G"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						
						cells["H"+cellRow].PutValue(int.Parse(dt.Compute("sum(CONNECTION_NUMBER)","").ToString()));	
						cells["H"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;					
						cells["H"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["H"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;					
						cells["H"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					}
				
					vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2491, language), dt, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, language);
					//Ajustement de la taile des cellules en fonction du contenu
					for(int c=0;c<=7;c++){
						sheet.AutoFitColumn(c);
					}	
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'insérer des données dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par type de client
		/// <summary>
		/// Top des connections par type de client
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
        internal static Workbook TopTypeConnected(Workbook excel, BastetCommon.Parameters parameters, int language, DataAccess.Client dataAccessClient) {
			try{
								
				//Chargement des données
				DataTable dt=null;
                dt = dataAccessClient.TopTypeConnected();
													
				#region Intégration  données client
						
				if(dt!=null && dt.Rows.Count>0){
					//Insertion dans fichier excel
					int cellRow =4;
					int startIndex=cellRow;
					int[] iArray = null;
					string[] strArray ;										
					Range range =null;
					int s=1;
					int totalCompanyRow=0;
					const int nbMaxRowByPage=40;
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;
					BastetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2492, language), dt, nbMaxRowByPage, ref s, 5, language);
							
					//libellés tranches horaires
					strArray = new string[] { " "+GestionWeb.GetWebWord(2493, language)+" ", "00h00-8h00", "8H00-12H00", "12H00-16h00", "16H00-20h00", "20h00-24h00" }; 
					
					range = cells.CreateRange("A"+startIndex,"G"+startIndex);
					cells.ImportObjectArray(strArray,range.FirstRow,range.FirstColumn,false);									
					range.SetOutlineBorder(BorderType.RightBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.TopBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.Black);
					
					BastetFunctions.WorkSheet.CellsHeaderStyle(cells,null,startIndex-1,0,5,true,Color.White);
					BastetFunctions.WorkSheet.CellsHeaderStyle(cells, GestionWeb.GetWebWord(1401, language), startIndex - 1, 6, 6, true, Color.White);					
					
					cellRow++;
					
					totalCompanyRow=cellRow;
				
					foreach(DataRow dr in  dt.Rows){
						
						//Connexions par login
						range = cells.CreateRange("B"+cellRow,"G"+cellRow);
						iArray = new int[]{int.Parse(dr["CONNECTION_NUMBER_24_8"].ToString()),int.Parse(dr["CONNECTION_NUMBER_8_12"].ToString()),int.Parse(dr["CONNECTION_NUMBER_12_16"].ToString()),int.Parse(dr["CONNECTION_NUMBER_16_20"].ToString()),
											  int.Parse(dr["CONNECTION_NUMBER_20_24"].ToString()),int.Parse(dr["CONNECTION_NUMBER"].ToString())};
						cells.ImportArray(iArray,range.FirstRow,range.FirstColumn,false);
						range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.Black);
						range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.Black);
							
						BastetFunctions.WorkSheet.PutCellValue(cells,dr["GROUP_CONTACT"].ToString(),cellRow-1,0,true,Color.Black);
																											
						cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
						cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
						cells["F"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
						cells["G"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;																				
								
						cellRow++;
					
					}
				
					//Total
					BastetFunctions.WorkSheet.PutCellValue(cells, GestionWeb.GetWebWord(1401, language), cellRow - 1, 0, true, Color.Red);

					BastetFunctions.WorkSheet.SetConnectionByTimeSliceToCells(cells,cellRow-1,1,dt,true,Color.Red);

					//Ajustement de la taile des cellules en fonction du contenu
					for(int c=0;c<=6;c++){
						sheet.AutoFitColumn(c);
					}										
					
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique à la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede à l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart,"B"+lastRow+":F"+lastRow,"B"+startIndex+":F"+startIndex
						, GestionWeb.GetWebWord(2486, language), GestionWeb.GetWebWord(2487, language), GestionWeb.GetWebWord(2488, language), GestionWeb.GetWebWord(2489, language));

					#endregion

					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible to insert data into excel file.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par mois
		/// <summary>
		/// Top connections par mois par client
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Workbook TopConnectedByMonth(Workbook excel, BastetCommon.Parameters parameters, int language, DataAccess.Client clientDataAccess) {
			try{
								
				//Chargement des données
				DataTable dt=null;
                dt = BastetRules.Client.TopConnectedByMonth(parameters, clientDataAccess);
													
				#region Intégration  données client
						
				if(dt!=null && dt.Rows.Count>0){
					//Insertion dans fichier excel
					int cellRow =4;
					int startIndex=cellRow;
					int s=1;
					object totalConnectionByMonth=0;
					const int nbMaxRowByPage=40;
					int j=3;
					int i=3;
					string serial="";
					string category="";
					int upperLeftColumn=5;
					int minimunVPageBreaks=9;
					string vPageBreaks="";
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;

					BastetFunctions.WorkSheet.SetConnectionByDate(dt, sheet, cells, ref cellRow, ref i, ref j, ref category, ref serial, GestionWeb.GetWebWord(2494, language), "login", ConstantesTracking.Period.Type.monthly,language);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique à la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede à l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart, serial, category, GestionWeb.GetWebWord(2486, language), GestionWeb.GetWebWord(2496, language), GestionWeb.GetWebWord(2290, language), GestionWeb.GetWebWord(2489, language));
					#endregion
					
					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2494, language), dt, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, language);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible to insert data into excel file.", e);
			}
			return excel;

		}
		#endregion

		#region Top connections par type de client et par mois 
		/// <summary>
		/// Top connections par type de client et par mois
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Workbook TopTypeConnectedByMonth(Workbook excel, BastetCommon.Parameters parameters, int language, DataAccess.Client clientDataAccess) {
			try{
								
				//Chargement des données
				DataTable dt=null;	
				if(parameters==null || parameters.Logins.Length==0)
                    dt = BastetRules.Client.TopTypeConnectedByMonth(parameters, clientDataAccess);
													
				#region Intégration  données client
						
				if((parameters==null || parameters.Logins.Length==0) && dt!=null && dt.Rows.Count>0){
					//Insertion dans fichier excel
					int cellRow =4;
					int startIndex=cellRow;
					int s=1;				
					object totalConnectionByMonth=0;
					const int nbMaxRowByPage=40;
					int j=3;
					int i=3;
					string serial="";
					string category="";
					int upperLeftColumn=5;
					int minimunVPageBreaks=8;
					string vPageBreaks="";
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;

					BastetFunctions.WorkSheet.SetConnectionByClientByDate(dt, sheet, cells, ref cellRow, ref i, ref j, ref category, ref serial, GestionWeb.GetWebWord(2497, language), "group_contact", ConstantesTracking.Period.Type.monthly, language);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique à la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,6);

					//Accede à l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart, serial, category, GestionWeb.GetWebWord(2486, language), GestionWeb.GetWebWord(2496, language), GestionWeb.GetWebWord(2290, language), GestionWeb.GetWebWord(2489, language));
					#endregion

					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2497, language), dt, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, language);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible to insert data into excel file.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par jour nommé
		/// <summary>
		/// Top connections par mois par jour nommé
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
        internal static Workbook TopConnectedByDay(Workbook excel, BastetCommon.Parameters parameters, int language, DataAccess.Client clientDataAccess) {
			try{
								
				//Chargement des données
				DataTable dt=null;
                dt = BastetRules.Client.TopConnectedByDay(parameters, clientDataAccess);
													
				#region Intégration  données client
						
				if(dt!=null && dt.Rows.Count>0){
					//Insertion dans fichier excel
					int cellRow =4;
					int startIndex=cellRow;
					int s=1;
					object totalConnectionByDay=0;
					const int nbMaxRowByPage=40;
					int j=3;
					int i=3;
					string serial="";
					string category="";
					int upperLeftColumn=5;
					int minimunVPageBreaks=9;
					string vPageBreaks="";

					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;	

					BastetFunctions.WorkSheet.SetConnectionByDate(dt, sheet, cells, ref cellRow, ref i, ref j, ref category, ref serial, " " + GestionWeb.GetWebWord(2498, language), "login", ConstantesTracking.Period.Type.dayly, language);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique à la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede à l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart, serial, category, GestionWeb.GetWebWord(2486, language), GestionWeb.GetWebWord(2499, language), GestionWeb.GetWebWord(2289, language), GestionWeb.GetWebWord(2489, language));
					#endregion
					
					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i+1)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2500, language), dt, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, language);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible to insert data into excel file.", e);
			}
			return excel;

		}
		#endregion

		#region Top connections par type de client et par jour nommé 
		/// <summary>
		/// Top connections par type de client et par jour nommé 
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
        internal static Workbook TopTypeConnectedByDay(Workbook excel, BastetCommon.Parameters parameters, int language, DataAccess.Client clientDataAccess) {
			try{
								
				//Chargement des données
				DataTable dt=null;	
				if(parameters==null || parameters.Logins.Length==0)
                    dt = BastetRules.Client.TopTypeConnectedByDay(parameters, clientDataAccess);
													
				#region Intégration  données client
						
				if((parameters==null || parameters.Logins.Length==0) && dt!=null && dt.Rows.Count>0){
					//Insertion dans fichier excel
					int cellRow =4;
					int startIndex=cellRow;
					int s=1;				
					object totalConnectionByMonth=0;
					const int nbMaxRowByPage=40;
					int j=3;
					int i=3;
					string serial="";
					string category="";
					int upperLeftColumn=5;
					int minimunVPageBreaks=8;
					string vPageBreaks="";

					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;	
					//					BastetFunctions.WorkSheet.PageSettings(sheet," Top type de clients par mois",dt,nbMaxRowByPage,ref s,5);

					BastetFunctions.WorkSheet.SetConnectionByClientByDate(dt, sheet, cells, ref cellRow, ref i, ref j, ref category, ref serial, GestionWeb.GetWebWord(2502, language), "group_contact", ConstantesTracking.Period.Type.dayly, language);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique à la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede à l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart, serial, category, GestionWeb.GetWebWord(2486, language), GestionWeb.GetWebWord(2503, language), GestionWeb.GetWebWord(2289, language), GestionWeb.GetWebWord(2489, language));
					#endregion

					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2501, language), dt, nbMaxRowByPage, ref s, upperLeftColumn, cells[(cellRow - 1 + 30), (i)].Name,language);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible to insert data into excel file.", e);
			}
			return excel;

		}
		#endregion

		#region IP adresse
		/// <summary>
		/// IP adresse par client
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
        internal static Workbook IPAddress(Workbook excel, BastetCommon.Parameters parameters, int language, DataAccess.Client dataAccessClient) {
			try{
								
				//Chargement des données
				DataTable dt=null;
				if(parameters!=null && parameters.Logins.Length>0)
                    dt = dataAccessClient.IPAddress();
													
				#region Intégration  données client
						
				if( dt!=null && dt.Rows.Count>0){
					//Insertion dans fichier excel
					int cellRow =4;
					int startIndex=cellRow;
					int s=1;

					const int nbMaxRowByPage=40;					
					
					int upperLeftColumn=5;
					int minimunVPageBreaks=8;
					string vPageBreaks="";
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;

					BastetFunctions.WorkSheet.SetIPAdressByClient(dt, sheet, cells, ref cellRow, " "+GestionWeb.GetWebWord(2485, language) +" ",language);
					#region Graphique

					int lastRow=cellRow;
				
					#endregion
					
					vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet, " " + GestionWeb.GetWebWord(2504, language) + " ", dt, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, language);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible to insert data into excel file.", e);
			}
			return excel;

		}
		#endregion

        #region Top connections Clients, IP, time slot
        /// <summary>
        /// Top connections Clients, IP, time slot
        /// </summary>		
        ///<param name="parameters">parametres</param>
        ///<param name="excel"> excel file</param>
        internal static Workbook TopConnectedByIpTimeSlot(Workbook excel, BastetCommon.Parameters parameters, int language)
        {
            try
            {

                //Data loading
                DataTable dt = DataAccess.Client.TopConnectedByIpTimeSlot(parameters);

                #region Intégration  données client

                if (dt != null && dt.Rows.Count > 0)
                {
                    //Insertion into excel file
                    int cellRow = 4;
                    int startIndex = cellRow;
                    int[] iArray = null, iArrayTotal =null;
                    string[] strArray;
                    int upperLeftColumn = 5;
                    int minimunVPageBreaks = 9;
                    int s = 1, start = 0;
                    const int nbMaxRowByPage = 40;
                    string vPageBreaks = "", oldLogin ="";
                    Range range = null;
                    Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
                    Cells cells = sheet.Cells;
                    long oldIdLogin = long.MinValue, oldIdCompany = long.MinValue;
                    int total_24_3 = 0, total_3_6 = 0, total_6_9 = 0, total_9_12 = 0, total_12_15 = 0, total_15_18 = 0, total_18_21 = 0, total_21_24 = 0;
                    int totalLine = 0, totalCol =0;

                    //Header Number connections
                    int i = 4;
                    cells["E" + cellRow].PutValue(GestionWeb.GetWebWord(2490, language));
                    cells[cellRow-1, i].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    cells[cellRow - 1, i].Style.ForegroundColor = Color.FromArgb(128, 128, 192);
                    cells[cellRow - 1, i].Style.Pattern = BackgroundType.Solid;
                    cells[cellRow - 1, i].Style.Font.Color = Color.White;
                    cells[cellRow - 1, i].Style.Font.IsBold = true;                  
                    cells["E" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    cells["E" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    cells.Merge(cellRow - 1, 4, 1, 9);
                    for (int j = 4; j <= 12; j++)
                    {
                        cells[cellRow - 1, j].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    }
                    cells["M" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    cells["E" + cellRow].Style.HorizontalAlignment = TextAlignmentType.Center;
                    cellRow++;                 

                    //Labels Customer, IP address, Date, connection numbers by time slots
                    startIndex = cellRow;
                    strArray = new string[] { GestionWeb.GetWebWord(1132, language), GestionWeb.GetWebWord(2485, language), GestionWeb.GetWebWord(2532, language), GestionWeb.GetWebWord(2806, language), "00h00-3h00", "03H00-6H00", "06H00-9h00", "09H00-12h00", "12H00-15h00", "15H00-18h00", "18H00-21h00", "21H00-24h00","TOTAL"};
                     range = cells.CreateRange("A" + startIndex, "M" + startIndex);
                     BastetFunctions.WorkSheet.CellsHeaderStyle(cells, null, cellRow-1, 0, 12, true, Color.White);                                       
                    cells.ImportObjectArray(strArray, range.FirstRow, range.FirstColumn, false);
                    range.SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                    range.SetOutlineBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                    range.SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                    range.SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                    cellRow++;

                    //Treating each company, customer, IP adress, Date, connection numbers by time slots                                                            

                        foreach (DataRow dr in dt.Rows)
                        {

                            //Connexions par login
                           

                            if (oldIdLogin != long.Parse(dr["id_login"].ToString()) && start > 0)
                            {
                                range = cells.CreateRange("E" + cellRow, "M" + cellRow);

                                //Add tota line
                                totalCol = total_24_3+ total_3_6 + total_6_9 + total_9_12+ total_12_15 + total_15_18 + total_18_21 + total_21_24;
                                iArrayTotal = new int[] { total_24_3, total_3_6, total_6_9, total_9_12, total_12_15, total_15_18, total_18_21, total_21_24, totalCol };

                                cells.ImportArray(iArrayTotal, range.FirstRow, range.FirstColumn, false);
                                range.SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                                range.SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);

                                //Company
                               // cells["A" + cellRow].PutValue(dr["COMPANY"].ToString());
                                cells["A" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                                cells["A" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                                cells["A" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["A" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                                cells["A" + cellRow].Style.Font.IsBold = true;

                                //Login
                                cells["B" + cellRow].PutValue(oldLogin);
                                cells["B" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                                cells["B" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                                cells["B" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                                //IP ADDRESS
                                cells["C" + cellRow].PutValue("TOTAL");
                                cells["C" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                                cells["C" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                                cells["C" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                                cells["C" + cellRow].Style.Font.IsBold = true;

                                //DATE                               
                                //cells["D" + cellRow].PutValue(dateConnection);
                                cells["D" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                                cells["D" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                                cells["D" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;


                                cells["E" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["E" + cellRow].Style.Font.IsBold = true;
                                cells["F" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["F" + cellRow].Style.Font.IsBold = true;
                                cells["G" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["G" + cellRow].Style.Font.IsBold = true;
                                cells["H" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["H" + cellRow].Style.Font.IsBold = true;
                                cells["I" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["I" + cellRow].Style.Font.IsBold = true;
                                cells["J" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["J" + cellRow].Style.Font.IsBold = true;
                                cells["K" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["K" + cellRow].Style.Font.IsBold = true;
                                cells["L" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["L" + cellRow].Style.Font.IsBold = true;
                                cells["M" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                                cells["M" + cellRow].Style.Font.IsBold = true;
                                cellRow++;

                                total_24_3 = 0; total_3_6 = 0; total_6_9 = 0; total_9_12 = 0; total_12_15 = 0; total_15_18 = 0; total_18_21 = 0; total_21_24 = 0;
                                totalCol=0;

                            }
                            range = cells.CreateRange("E" + cellRow, "M" + cellRow);
                            totalLine = int.Parse(dr["CONNECTION_NUMBER_24_3"].ToString()) + int.Parse(dr["CONNECTION_NUMBER_3_6"].ToString()) + int.Parse(dr["CONNECTION_NUMBER_6_9"].ToString()) + int.Parse(dr["CONNECTION_NUMBER_9_12"].ToString()) +
                                                  int.Parse(dr["CONNECTION_NUMBER_12_15"].ToString()) + int.Parse(dr["CONNECTION_NUMBER_15_18"].ToString()) + int.Parse(dr["CONNECTION_NUMBER_18_21"].ToString()) + int.Parse(dr["CONNECTION_NUMBER_21_24"].ToString());

                            iArray = new int[]{int.Parse(dr["CONNECTION_NUMBER_24_3"].ToString()),int.Parse(dr["CONNECTION_NUMBER_3_6"].ToString()),int.Parse(dr["CONNECTION_NUMBER_6_9"].ToString()),int.Parse(dr["CONNECTION_NUMBER_9_12"].ToString()),
												  int.Parse(dr["CONNECTION_NUMBER_12_15"].ToString()),int.Parse(dr["CONNECTION_NUMBER_15_18"].ToString()),int.Parse(dr["CONNECTION_NUMBER_18_21"].ToString()),int.Parse(dr["CONNECTION_NUMBER_21_24"].ToString()),totalLine};
                          
                            total_24_3 = total_24_3 + int.Parse(dr["CONNECTION_NUMBER_24_3"].ToString());
                            total_3_6 = total_3_6 + int.Parse(dr["CONNECTION_NUMBER_3_6"].ToString());
                            total_6_9 = total_6_9 + int.Parse(dr["CONNECTION_NUMBER_6_9"].ToString());
                            total_9_12 = total_9_12 + int.Parse(dr["CONNECTION_NUMBER_9_12"].ToString());
                            total_12_15 = total_12_15 + int.Parse(dr["CONNECTION_NUMBER_12_15"].ToString());
                            total_15_18 = total_15_18 + int.Parse(dr["CONNECTION_NUMBER_15_18"].ToString());
                            total_18_21 = total_18_21 + int.Parse(dr["CONNECTION_NUMBER_18_21"].ToString());
                            total_21_24 = total_21_24 + int.Parse(dr["CONNECTION_NUMBER_21_24"].ToString());

                            cells.ImportArray(iArray, range.FirstRow, range.FirstColumn, false);
                            range.SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                            range.SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);

                            //Company
                            if (oldIdCompany != long.Parse(dr["id_company"].ToString()))
                            cells["A" + cellRow].PutValue(dr["COMPANY"].ToString());
                            cells["A" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            cells["A" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            cells["A" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["A" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                            cells["A" + cellRow].Style.Font.IsBold = true;

                            //Login
                            cells["B" + cellRow].PutValue(dr["LOGIN"].ToString());
                            cells["B" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            cells["B" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            cells["B" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                            //IP ADDRESS
                            cells["C" + cellRow].PutValue(dr["IP_ADDRESS"].ToString());
                            cells["C" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            cells["C" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            cells["C" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                            
                            //DATE
                            string dateConnection = dr["DATE_CONNECTION"].ToString();
                           dateConnection =  dateConnection.Substring(6, 2) + "/" + dateConnection.Substring(4, 2) + "/" + dateConnection.Substring(0, 4);
                            cells["D" + cellRow].PutValue(dateConnection);
                            cells["D" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            cells["D" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            cells["D" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                           
                            cells["E" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["F" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["G" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["H" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["I" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["J" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["K" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["L" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["M" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["M" + cellRow].Style.Font.IsBold = true;
                            cellRow++;
                            start = 1;

                            oldLogin = dr["LOGIN"].ToString();
                            oldIdLogin = long.Parse(dr["id_login"].ToString());
                            oldIdCompany = long.Parse(dr["id_company"].ToString());
                        }

                        if (start > 0)
                        {
                            range = cells.CreateRange("E" + cellRow, "M" + cellRow);

                            //Add tota line
                            totalCol = total_24_3 + total_3_6 + total_6_9 + total_9_12 + total_12_15 + total_15_18 + total_18_21 + total_21_24;
                            iArrayTotal = new int[] { total_24_3, total_3_6, total_6_9, total_9_12, total_12_15, total_15_18, total_18_21, total_21_24, totalCol };

                            cells.ImportArray(iArrayTotal, range.FirstRow, range.FirstColumn, false);
                            range.SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                            range.SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                            //Company
                            // cells["A" + cellRow].PutValue(dr["COMPANY"].ToString());
                            cells["A" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            cells["A" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            cells["A" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["A" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                            cells["A" + cellRow].Style.Font.IsBold = true;

                            //Login
                            cells["B" + cellRow].PutValue(oldLogin);
                            cells["B" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            cells["B" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            cells["B" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                            //IP ADDRESS
                            cells["C" + cellRow].PutValue("TOTAL");
                            cells["C" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            cells["C" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            cells["C" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                            cells["C" + cellRow].Style.Font.IsBold = true;

                            //DATE                               
                            //cells["D" + cellRow].PutValue(dateConnection);
                            cells["D" + cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            cells["D" + cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            cells["D" + cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                            cells["E" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["E" + cellRow].Style.Font.IsBold = true;
                            cells["F" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["F" + cellRow].Style.Font.IsBold = true;
                            cells["G" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["G" + cellRow].Style.Font.IsBold = true;
                            cells["H" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["H" + cellRow].Style.Font.IsBold = true;
                            cells["I" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["I" + cellRow].Style.Font.IsBold = true;
                            cells["J" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["J" + cellRow].Style.Font.IsBold = true;
                            cells["K" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["K" + cellRow].Style.Font.IsBold = true;
                            cells["L" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["L" + cellRow].Style.Font.IsBold = true;
                            cells["M" + cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            cells["M" + cellRow].Style.Font.IsBold = true;
                          
                            total_24_3 = 0; total_3_6 = 0; total_6_9 = 0; total_9_12 = 0; total_12_15 = 0; total_15_18 = 0; total_18_21 = 0; total_21_24 = 0;

                        }
                       //Ajustement de la taile des cellules en fonction du contenu					                      
                    
                   
                    vPageBreaks = cells[(cellRow - 1 + 30), (minimunVPageBreaks)].Name;
                    BastetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2491, language), dt, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, language);
                    //Ajustement de la taile des cellules en fonction du contenu
                    for (int c = 0; c <= 12; c++)
                    {
                        sheet.AutoFitColumn(c);
                    }
                #endregion
                }

            }
            catch (System.Exception e)
            {
                throw new BastetExceptions.ClientUIException(" Impossible to insert data in excel file.", e);
            }
            return excel;

        }
        #endregion

	}
}
