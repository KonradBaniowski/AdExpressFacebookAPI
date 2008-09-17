#region Informations
// Auteur: D. V. Mussuma
// Date de création: 01/03/2006
// Date de modification:
#endregion

using System;
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
	/// Intégration des données dans fichier excel des Top  sociétés qui se connectent le plus
	/// </summary>
	public class Company
	{
		#region Top connection sociétés
		/// <summary>
		/// Top des sociétés qui se connectent le plus
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel TopConnected(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des données
				DataTable dt=null;				
				dt = DataAccess.Company.TopConnected(parameters);
													
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
					strArray = new string[] {"Société"," Type de clients ","00h00-8h00","8H00-12H00","12H00-16h00","16H00-20h00","20h00-24h00"," Total "}; //A mettre en constante
					
					range = cells.CreateRange("A"+startIndex,"H"+startIndex);
					cells.ImportObjectArray(strArray,range.FirstRow,range.FirstColumn,false);									
					range.SetOutlineBorder(BorderType.RightBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.TopBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.Black);
					range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.Black);
					
					BastetFunctions.WorkSheet.CellsHeaderStyle(cells,null,3,0,7,true,Color.White);					
											
					cellRow++;				
								
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
						
						cells["B"+cellRow].PutValue(dr["GROUP_CONTACT"].ToString());	
						cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
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
						
					//Total
					BastetFunctions.WorkSheet.PutCellValue(cells,"Total",cellRow-1,0,true,Color.Red);
					BastetFunctions.WorkSheet.PutCellValue(cells,"",cellRow-1,1,true,Color.Red);
					BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_24_8)","").ToString()),cellRow-1,2,true,Color.Red);						
					BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_8_12)","").ToString()),cellRow-1,3,true,Color.Red);
					BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_12_16)","").ToString()),cellRow-1,4,true,Color.Red);
					BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_16_20)","").ToString()),cellRow-1,5,true,Color.Red);
					BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_20_24)","").ToString()),cellRow-1,6,true,Color.Red);
					BastetFunctions.WorkSheet.PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER)","").ToString()),cellRow-1,7,true,Color.Red);
	
					//Ajustement de la taile des cellules en fonction du contenu
					for(int c=0;c<=7;c++){
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
					BastetFunctions.BastetChart.Build(chart,"C"+lastRow+":G"+lastRow,"C"+startIndex+":G"+startIndex
						,"Connections","Connections par tranche horaire","Tranches horaires","Nombre de Connections");

					#endregion
					
					 vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top sociétés tranche horaire",dt,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.CompanyUIException(" Impossible d'insérer des données dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

		#region Top connections par mois
		/// <summary>
		/// Top connections par mois par société
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Excel TopConnectedByMonth(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des données
				DataTable dt=null;				
				dt = BastetRules.Company.TopConnectedByMonth(parameters);
													
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
							
					BastetFunctions.WorkSheet.SetConnectionByDate(dt,sheet,cells,ref cellRow,ref i,ref j,ref category,ref serial," Type de client ","group_contact",ConstantesTracking.Period.Type.monthly);			
					
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique à la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede à l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart,serial,category,"Connections","Connections par mois","Mois","Nombre de Connections");
					#endregion
					
					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top sociétés par mois",dt,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks);
												
				}
				#endregion	
				
			}catch(System.Exception e){
				throw new BastetExceptions.CompanyUIException(" Impossible d'insérer des données dans le fichier excel.",e);
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
		internal static Excel TopConnectedByDay(Excel excel,BastetCommon.Parameters parameters){
			try{
								
				//Chargement des données
				DataTable dt=null;				
				dt = BastetRules.Company.TopConnectedByDay(parameters);
													
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
							
					BastetFunctions.WorkSheet.SetConnectionByDate(dt,sheet,cells,ref cellRow,ref i,ref j,ref category,ref serial," Type de client ","group_contact",ConstantesTracking.Period.Type.dayly);			
					
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique à la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede à l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart,serial,category,"Connections","Connections par jour nommé","Jour","Nombre de Connections");
					#endregion
					
					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet,"Top sociétés jour nommé",dt,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks);
												
				}
				#endregion	
				
			}catch(System.Exception e){
				throw new BastetExceptions.CompanyUIException(" Impossible d'insérer des données dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion
	}
}
