#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 24/11/2005
// Date de modification:
#endregion

using System;
using System.Collections;

using Aspose.Excel;
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

namespace TNS.AdExpress.Anubis.Bastet.UI
{
	/// <summary>
	/// Int�gration des donn�es dans fichier excel des Top  clients qui se connectent le plus
	/// </summary>
	public class Client
	{
		#region Top connections clients
		/// <summary>
		/// Top des clients qui se connectent le plus
		/// </summary>		
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel TopConnected(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des donn�es
				DataTable dt = DataAccess.Client.TopConnected(parameters);
													
				#region Int�gration  donn�es client
						
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
							
					//libell�s tranches horaires
					if(parameters!=null && parameters.Logins.Length==0){
						strArray = new string[] {"Soci�t�","Client","00h00-8h00","8H00-12H00","12H00-16h00","16H00-20h00","20h00-24h00","Total"}; //A mettre en constante
						range = cells.CreateRange("A"+startIndex,"H"+startIndex);
						BastetFunctions.WorkSheet.CellsHeaderStyle(cells,null,3,0,7,true,Color.White);
					}
					else{
						strArray = new string[] {"00h00-8h00","8H00-12H00","12H00-16h00","16H00-20h00","20h00-24h00","Total"}; //A mettre en constante
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
						BastetFunctions.WorkSheet.PutCellValue(cells,"Total",cellRow-1,0,true,Color.Red);
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

						//Ajout un graphique � la feuille excel				
						int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

						//Accede � l'instance d'un nouveau graphique
						Chart chart=sheet.Charts[chartIndex];
					
						//Construit le graphique
						BastetFunctions.BastetChart.Build(chart,"C"+lastRow+":G"+lastRow,"C"+startIndex+":G"+startIndex
							,"Connections","Connections par tranche horaire","Tranches horaires","Nombre de Connections");

						#endregion
					}
					else{
						//Cas Logins s�lectionn�s
										
						cells["B"+cellRow].PutValue("Nombre de connections clients ");
						cells["B"+cellRow].Style.Font.Color = Color.Black;
						cells["B"+cellRow].Style.ForegroundColor = Color.Silver;
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
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top clients tranche horaire",dt,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks);
					//Ajustement de la taile des cellules en fonction du contenu
					for(int c=0;c<=7;c++){
						sheet.AutoFitColumn(c);
					}	
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'ins�rer des donn�es dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par type de client
		/// <summary>
		/// Top des connections par type de client
		/// </summary>		
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel TopTypeConnected(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des donn�es
				DataTable dt=null;				
				dt = DataAccess.Client.TopTypeConnected(parameters);
													
				#region Int�gration  donn�es client
						
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
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top type clients tranche horaire",dt,nbMaxRowByPage,ref s,5);
							
					//libell�s tranches horaires
					strArray = new string[] {" Type de clients ","00h00-8h00","8H00-12H00","12H00-16h00","16H00-20h00","20h00-24h00"}; //A mettre en constante
					
					range = cells.CreateRange("A"+startIndex,"G"+startIndex);
					cells.ImportObjectArray(strArray,range.FirstRow,range.FirstColumn,false);									
					range.SetOutlineBorder(BorderType.RightBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.TopBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.Black);
					
					BastetFunctions.WorkSheet.CellsHeaderStyle(cells,null,startIndex-1,0,5,true,Color.White);					
					BastetFunctions.WorkSheet.CellsHeaderStyle(cells," Total",startIndex-1,6,6,true,Color.White);					
					
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
					BastetFunctions.WorkSheet.PutCellValue(cells,"Total",cellRow-1,0,true,Color.Red);

					BastetFunctions.WorkSheet.SetConnectionByTimeSliceToCells(cells,cellRow-1,1,dt,true,Color.Red);

					//Ajustement de la taile des cellules en fonction du contenu
					for(int c=0;c<=6;c++){
						sheet.AutoFitColumn(c);
					}										
					
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique � la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede � l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart,"B"+lastRow+":F"+lastRow,"B"+startIndex+":F"+startIndex
						,"Connections","Connections par tranche horaire","Tranches horaires","Nombre de Connections");

					#endregion

					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'ins�rer des donn�es dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par mois
		/// <summary>
		/// Top connections par mois par client
		/// </summary>		
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel TopConnectedByMonth(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des donn�es
				DataTable dt=null;				
				dt = BastetRules.Client.TopConnectedByMonth(parameters);
													
				#region Int�gration  donn�es client
						
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
									
					BastetFunctions.WorkSheet.SetConnectionByDate(dt,sheet,cells,ref cellRow,ref i,ref j,ref category,ref serial," Top clients par mois","login",ConstantesTracking.Period.Type.monthly);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique � la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede � l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart,serial,category,"Connections","Connections par mois","Mois","Nombre de Connections");
					#endregion
					
					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top clients par mois",dt,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'ins�rer des donn�es dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par type de client et par mois 
		/// <summary>
		/// Top connections par type de client et par mois
		/// </summary>		
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel TopTypeConnectedByMonth(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des donn�es
				DataTable dt=null;	
				if(parameters==null || parameters.Logins.Length==0)
				dt = BastetRules.Client.TopTypeConnectedByMonth(parameters);
													
				#region Int�gration  donn�es client
						
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
												
					BastetFunctions.WorkSheet.SetConnectionByClientByDate(dt,sheet,cells,ref cellRow,ref i,ref j,ref category,ref serial," Top type de clients par mois","group_contact",ConstantesTracking.Period.Type.monthly);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique � la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,6);

					//Accede � l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart,serial,category,"Connections","Connections par mois","Mois","Nombre de Connections");
					#endregion

					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top type clients par mois",dt,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'ins�rer des donn�es dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par jour nomm�
		/// <summary>
		/// Top connections par mois par jour nomm�
		/// </summary>		
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel TopConnectedByDay(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des donn�es
				DataTable dt=null;				
				dt = BastetRules.Client.TopConnectedByDay(parameters);
													
				#region Int�gration  donn�es client
						
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
					//					BastetFunctions.WorkSheet.PageSettings(sheet," Top clients par mois",dt,nbMaxRowByPage,ref s,5);							
									
					BastetFunctions.WorkSheet.SetConnectionByDate(dt,sheet,cells,ref cellRow,ref i,ref j,ref category,ref serial," Top clients par jour nomm�","login",ConstantesTracking.Period.Type.dayly);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique � la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede � l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart,serial,category,"Connections","Connections par jour","Jour","Nombre de Connections");
					#endregion
					
					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i+1)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top clients jour nomm�",dt,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'ins�rer des donn�es dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par type de client et par jour nomm� 
		/// <summary>
		/// Top connections par type de client et par jour nomm� 
		/// </summary>		
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel TopTypeConnectedByDay(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des donn�es
				DataTable dt=null;	
				if(parameters==null || parameters.Logins.Length==0)
					dt = BastetRules.Client.TopTypeConnectedByDay(parameters);
													
				#region Int�gration  donn�es client
						
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
												
					BastetFunctions.WorkSheet.SetConnectionByClientByDate(dt,sheet,cells,ref cellRow,ref i,ref j,ref category,ref serial," Top type de clients par jour nomm�","group_contact",ConstantesTracking.Period.Type.dayly);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique � la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede � l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart,serial,category,"Connections","Connections par jour nomm�","Jour","Nombre de Connections");
					#endregion

					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top type clients jour nomm�",dt,nbMaxRowByPage,ref s,upperLeftColumn,cells[(cellRow-1+30),(i)].Name);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'ins�rer des donn�es dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

		#region IP adresse
		/// <summary>
		/// IP adresse par client
		/// </summary>		
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel IPAddress(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des donn�es
				DataTable dt=null;
				if(parameters!=null && parameters.Logins.Length>0)
				 dt = DataAccess.Client.IPAddress(parameters);
													
				#region Int�gration  donn�es client
						
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
									
					BastetFunctions.WorkSheet.SetIPAdressByClient(dt,sheet,cells,ref cellRow," Client ");
					#region Graphique

					int lastRow=cellRow;
				
					#endregion
					
					vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet," Suivi des adresses IP par clients ",dt,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'ins�rer des donn�es dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

	}
}