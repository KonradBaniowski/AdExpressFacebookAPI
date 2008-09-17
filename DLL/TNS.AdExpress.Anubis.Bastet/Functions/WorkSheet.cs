#region Informations
// Auteur: D. V. Mussuma
// Date de création: 02/03/2006
// Date de modification:
#endregion

using System;
using System.Data;
using Aspose.Excel;
using System.Drawing;
using TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.DB;
using ConstantesTracking=TNS.AdExpress.Constantes.Tracking;
using BastetFunctions=TNS.AdExpress.Anubis.Bastet.Functions;

namespace TNS.AdExpress.Anubis.Bastet.Functions
{
	/// <summary>
	/// Fonctionde feuille de statistiques excel.
	/// </summary>
	public class WorkSheet
	{
		#region Connections par tranches horaires

		/// <summary>
		/// Insert les connections par tranche horaire dans une feuille excel
		/// </summary>
		/// <param name="cells">cellules</param>
		/// <param name="row">ligne</param>
		/// <param name="column">colonne</param>
		/// <param name="dt">table de données</param>
		/// <param name="isBold">vrai si police en  gras</param>
		/// <param name="color">couleur de la police</param>
		internal static void SetConnectionByTimeSliceToCells(Aspose.Excel.Cells cells,int row,int column,DataTable dt,bool isBold,System.Drawing.Color color){			
	
			PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_24_8)","").ToString()),row,column,isBold,color);
			
			column++;		
			PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_8_12)","").ToString()),row,column,isBold,color);

			column++;
			PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_12_16)","").ToString()),row,column,isBold,color);			

			column++;
			PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_16_20)","").ToString()),row,column,isBold,color);	
			
			column++;
			PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER_20_24)","").ToString()),row,column,isBold,color);			
			
			column++;
			PutCellValue(cells,int.Parse(dt.Compute("sum(CONNECTION_NUMBER)","").ToString()),row,column,isBold,color);						
		}
		
		#endregion

		#region Connections par mois
		/// <summary>
		/// Insert les données des connections par mois (ou jour nommé) par client
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="sheet">feuille excel</param>
		/// <param name="cells">cellules excel</param>
		/// <param name="cellRow">ligne</param>
		/// <param name="i">index colonne</param>
		/// <param name="j">Index colonne</param>
		/// <param name="category">catégorie de valeurs</param>
		/// <param name="serial">série de valeurs</param>
		/// <param name="sheetName">Nom de la feuille excel</param>
		/// <param name="clientParameter">paramètre cleint</param>
		internal static void SetConnectionByDate(DataTable dt, Worksheet sheet,Aspose.Excel.Cells cells,ref int cellRow,ref int i,ref int j,ref string category,ref string serial,string sheetName,string clientParameter,ConstantesTracking.Period.Type periodType){
			
			string expression="";
			string date="";
			object totalConnectionByDate=0;
			
			
			//Libellés des mois
			PutCellValue(cells," Société ",cellRow-1,0,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);	
			PutCellValue(cells,sheetName,cellRow-1,1,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,1,1,true,Color.White);	
															
			for(i=3;i<dt.Columns.Count;i++){	
				switch(periodType){
					case ConstantesTracking.Period.Type.monthly :
					date = MonthString.GetCharacters(int.Parse(dt.Columns[i].ColumnName.Substring(10,2)),Language.FRENCH.GetHashCode(),3)
						+". "+dt.Columns[i].ColumnName.Substring(8,2);
						break;
					case ConstantesTracking.Period.Type.dayly :						
						date = BastetFunctions.Period.GetNamedDay(int.Parse(dt.Columns[i].ColumnName.Substring(4,1)),Language.FRENCH.GetHashCode());
						break;
				}
				PutCellValue(cells,date,cellRow-1,i-1,true,Color.Black);
				CellsHeaderStyle(cells,null,cellRow-1,i-1,i-1,true,Color.White);						
			}
					
			PutCellValue(cells," Total ",cellRow-1,i-1,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,i-1,i-1,true,Color.White);
			category="C4:"+cells[(cellRow-1),(i-2)].Name;

			cellRow++;
						
			foreach(DataRow dr in  dt.Rows){
				//Connexions par login et par mois
				PutCellValue(cells,dr["company"].ToString(),cellRow-1,0,true,Color.Black);	
				PutCellValue(cells,dr[clientParameter].ToString(),cellRow-1,1,false,Color.Black);						
				for(j=3;j<dt.Columns.Count;j++){
					PutCellValue(cells,dr[j],cellRow-1,j-1,false,Color.Black);							
				}
						
				//total colonne						
				PutCellValue(cells,dr["connection_number"],cellRow-1,j-1,false,Color.Black);												
				cellRow++;
					
			}
						
			//Total lignes
			PutCellValue(cells,"Total",cellRow-1,0,true,Color.Red);
			PutCellValue(cells,"",cellRow-1,1,true,Color.Black);
					
					
			for(j=3;j<dt.Columns.Count;j++){
				expression = "sum("+dt.Columns[j].ColumnName+")";
				totalConnectionByDate = dt.Compute(expression,"");
				if(totalConnectionByDate!=System.DBNull.Value && !totalConnectionByDate.Equals(""))
					PutCellValue(cells,totalConnectionByDate,cellRow-1,j-1,true,Color.Red);
				else PutCellValue(cells,0,cellRow-1,j-1,true,Color.Red);
			}
			serial="C"+cellRow+":"+cells[(cellRow-1),(j-2)].Name;

			totalConnectionByDate = dt.Compute("sum(connection_number)","");
			if(totalConnectionByDate!=System.DBNull.Value && !totalConnectionByDate.Equals(""))	
				PutCellValue(cells,totalConnectionByDate,cellRow-1,j-1,true,Color.Red);	
			else PutCellValue(cells,0,cellRow-1,j-1,true,Color.Red);	
				

			//Ajustement de la taile des cellules en fonction du contenu
			for(int c=0;c<dt.Columns.Count;c++)
				sheet.AutoFitColumn(c);
		}
		#endregion

		#region Connections par mois et par type de client
		/// <summary>
		/// Insert les données des connections par mois par client
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="sheet">feuille excel</param>
		/// <param name="cells">cellules excel</param>
		/// <param name="cellRow">ligne</param>
		/// <param name="i">index colonne</param>
		/// <param name="j">Index colonne</param>
		/// <param name="category">catégorie de valeurs</param>
		/// <param name="serial">série de valeurs</param>
		/// <param name="sheetName">Nom de la feuille excel</param>
		/// <param name="clientParameter">paramètre cleint</param>
		internal static void SetConnectionByClientByDate(DataTable dt, Worksheet sheet,Aspose.Excel.Cells cells,ref int cellRow,ref int i,ref int j,ref string category,ref string serial,string sheetName,string clientParameter,ConstantesTracking.Period.Type periodType){
			
			string expression="";
			string date="";
			object totalConnectionByDate=0;

			//Libellés des mois ou jour	
			PutCellValue(cells,sheetName,cellRow-1,0,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);	
															
			for(i=3;i<dt.Columns.Count;i++){	
				switch(periodType){
					case ConstantesTracking.Period.Type.monthly :
						date = MonthString.GetCharacters(int.Parse(dt.Columns[i].ColumnName.Substring(10,2)),Language.FRENCH.GetHashCode(),3)
							+". "+dt.Columns[i].ColumnName.Substring(8,2);
						break;
					case ConstantesTracking.Period.Type.dayly :						
						date = BastetFunctions.Period.GetNamedDay(int.Parse(dt.Columns[i].ColumnName.Substring(4,1)),Language.FRENCH.GetHashCode());
						break;
				}				
				WorkSheet.PutCellValue(cells,date,cellRow-1,i-2,true,Color.Black);
				WorkSheet.CellsHeaderStyle(cells,null,cellRow-1,i-2,i-2,true,Color.White);						
			}
					
			PutCellValue(cells," Total ",cellRow-1,i-2,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,i-2,i-2,true,Color.White);
			category="B4:"+cells[(cellRow-1),(i-3)].Name;

			cellRow++;
						
			foreach(DataRow dr in  dt.Rows){
				//Connexions par login et par mois (ou jour)
//				PutCellValue(cells,dr["company"].ToString(),cellRow-1,0,true,Color.Black);	
				PutCellValue(cells,dr[clientParameter].ToString(),cellRow-1,0,false,Color.Black);						
				for(j=3;j<dt.Columns.Count;j++){
					PutCellValue(cells,dr[j],cellRow-1,j-2,false,Color.Black);							
				}
						
				//total colonne						
				PutCellValue(cells,dr["connection_number"],cellRow-1,j-2,false,Color.Black);												
				cellRow++;
					
			}
						
			//Total lignes
			PutCellValue(cells,"Total",cellRow-1,0,true,Color.Red);			
					
			for(j=3;j<dt.Columns.Count;j++){
				expression = "sum("+dt.Columns[j].ColumnName+")";
				totalConnectionByDate = dt.Compute(expression,"");
				if(totalConnectionByDate!=System.DBNull.Value && !totalConnectionByDate.Equals(""))
					PutCellValue(cells,totalConnectionByDate,cellRow-1,j-2,true,Color.Red);
				else PutCellValue(cells,0,cellRow-1,j-2,true,Color.Red);
			}
			serial="B"+cellRow+":"+cells[(cellRow-1),(j-3)].Name;

			totalConnectionByDate = dt.Compute("sum(connection_number)","");
			if(totalConnectionByDate!=System.DBNull.Value && !totalConnectionByDate.Equals(""))	
				PutCellValue(cells,totalConnectionByDate,cellRow-1,j-2,true,Color.Red);	
			else PutCellValue(cells,0,cellRow-1,j-2,true,Color.Red);	
				

			//Ajustement de la taile des cellules en fonction du contenu
			for(int c=0;c<dt.Columns.Count;c++)
				sheet.AutoFitColumn(c);
		}
		#endregion

		#region Insertion d'une donnée dans une cellule
		/// <summary>
		/// Insert une donnée dans une cellule
		/// </summary>
		/// <param name="cells">Cellules</param>
		/// <param name="data">donnée</param>
		/// <param name="row">ligne</param>
		/// <param name="column">colonne</param>
		/// <param name="isBold">vrai si police en gras</param>
		/// <param name="color">couleur de la police</param>
		internal static void PutCellValue(Aspose.Excel.Cells cells,object data,int row,int column,bool isBold,System.Drawing.Color color){
			cells[row,column].PutValue(data);
			cells[row,column].Style.Font.Color = color;
			cells[row,column].Style.Font.IsBold = isBold;
			cells[row,column].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;	
			cells[row,column].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;										
			cells[row,column].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
			cells[row,column].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;			
		}
		#endregion

		#region Style des cellules en-têtes
		/// <summary>
		/// Met le style des cellules en-têtes
		/// </summary>
		/// <param name="cells">cellules</param>
		/// <param name="data">donnée</param>
		/// <param name="row">ligne</param>
		/// <param name="firstColumn">1ere colonne de la collection</param>
		/// <param name="lastColumn">dernière colonne de la collection</param>
		/// <param name="isBold">vrai si police en gras</param>
		/// <param name="color">Couleur de la police</param>
		internal static void CellsHeaderStyle(Aspose.Excel.Cells cells,object data,int row,int firstColumn,int lastColumn,bool isBold,System.Drawing.Color color){
			for(int i=firstColumn;i<=lastColumn;i++){
				if(data!=null)cells[row,i].PutValue(data);
				cells[row,i].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells[row,i].Style.ForegroundColor =  Color.FromArgb(128,128,192);					
				cells[row,i].Style.Font.Color = color;
				cells[row,i].Style.Font.IsBold = isBold;				
			}			
		}
		#endregion

		#region Connections par média et par module
		/// <summary>
		/// Insert les données des connections par média et par module
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="sheet">feuille excel</param>
		/// <param name="cells">cellules excel</param>
		/// <param name="cellRow">ligne</param>
		/// <param name="i">index colonne</param>
		/// <param name="j">Index colonne</param>
		/// <param name="category">catégorie de valeurs</param>
		/// <param name="serial">série de valeurs</param>
		/// <param name="sheetName">Nom de la feuille excel</param>
		/// <param name="clientParameter">paramètre cleint</param>
		internal static void SetTopMediaByModule(DataTable dt, Worksheet sheet,Aspose.Excel.Cells cells,ref int cellRow,ref int i,ref int j,ref string category,ref string serial){
			
			double totalConnectionByModule=0;						
			//Libellés des mois
			PutCellValue(cells," Top Média par module ",cellRow-1,0,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);					
			
			//libellés médias
			for(i=0;i<dt.Rows.Count;i++){
				PutCellValue(cells,dt.Rows[i]["vehicle"],cellRow-1,i+1,true,Color.Black);
				CellsHeaderStyle(cells,null,cellRow-1,i+1,i+1,true,Color.White);
			}
			category="B4:"+cells[(cellRow-1),(i)].Name;
			//libellé total ligne		
			PutCellValue(cells," Total ",cellRow-1,i+1,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,i+1,i+1,true,Color.White);
			cellRow++;
			
			//Insertion des valeurs de connections dans cellule excel
			i=0;
			//Pour chaque module
			for(j=3;j<dt.Columns.Count;j++){
				PutCellValue(cells,dt.Columns[j].ColumnName,cellRow-1,0,true,Color.Black);
				
				//Pour chaque média
				foreach(DataRow dr in dt.Rows){

					PutCellValue(cells,dr[j],cellRow-1,i+1,false,Color.Black);
					
					//total ligne du module	
					if(dr[j]!=System.DBNull.Value){
						totalConnectionByModule += double.Parse(dr[j].ToString());								
					}					
					i++;
				}	
				//total ligne														
				PutCellValue(cells,totalConnectionByModule,cellRow-1,i+1,false,Color.Black);
			

				cellRow++;
				totalConnectionByModule=0;
				i=0;
			}
					
			//Total colonne média
			PutCellValue(cells,"Total",cellRow-1,0,true,Color.Red);		
			i=0;
			foreach(DataRow dr in dt.Rows){
				if(dr["connection_number"]!=System.DBNull.Value){
								
					PutCellValue(cells,double.Parse(dr["connection_number"].ToString()),cellRow-1,i+1,true,Color.Red);
				}
				else PutCellValue(cells,0,cellRow-1,i+1,true,Color.Red);
				i++;
			}
			//total connections
			if(dt.Compute("sum(connection_number)","")!=System.DBNull.Value){
								
				PutCellValue(cells,double.Parse(dt.Compute("sum(connection_number)","").ToString()),cellRow-1,i+1,true,Color.Red);
			}
			else PutCellValue(cells,0,cellRow-1,i+1,true,Color.Red);

			
			serial="B"+cellRow+":"+cells[(cellRow-1),(i)].Name;

			//Ajustement de la taile des cellules en fonction du contenu

			for(int c=0;c<=dt.Rows.Count+1;c++)
				sheet.AutoFitColumn(c);
		}
		#endregion

		#region Adresse IP par client
		/// <summary>
		/// Insert les données Adresse IP par client
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="sheet">feuille excel</param>
		/// <param name="cells">cellules excel</param>
		/// <param name="cellRow">ligne</param>
		/// <param name="i">index colonne</param>
		/// <param name="j">Index colonne</param>
		/// <param name="category">catégorie de valeurs</param>
		/// <param name="serial">série de valeurs</param>
		/// <param name="sheetName">Nom de la feuille excel</param>
		/// <param name="clientParameter">paramètre client</param>
		internal static void SetIPAdressByClient(DataTable dt, Worksheet sheet,Aspose.Excel.Cells cells,ref int cellRow,string sheetName){
											
			Int64 oldIdLogin=0;
			Int64 oldIdCompany=0;

			//Libellés des clients
			PutCellValue(cells," Société ",cellRow-1,0,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);	
			PutCellValue(cells,sheetName,cellRow-1,1,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,1,1,true,Color.White);
			PutCellValue(cells,"Adresse IP",cellRow-1,2,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,2,2,true,Color.White);
															

			cellRow++;
						
			foreach(DataRow dr in  dt.Rows){
				//Adresse IP par client
				if(oldIdCompany!=Int64.Parse(dr["id_company"].ToString())){
					PutCellValue(cells,dr["company"].ToString(),cellRow-1,0,true,Color.Black);
				}else{
					cells[cellRow-1,0].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
				}
										
				PutCellValue(cells,dr["login"].ToString(),cellRow-1,1,false,Color.Black);				
				PutCellValue(cells,dr["IP_ADDRESS"],cellRow-1,2,false,Color.Black);	
																												
				cellRow++;
				oldIdLogin=Int64.Parse(dr["id_login"].ToString());
				oldIdCompany=Int64.Parse(dr["id_company"].ToString());
			}								
			cells[cellRow-2,0].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
			//Ajustement de la taile des cellules en fonction du contenu
			for(int c=0;c<4;c++)
				sheet.AutoFitColumn(c);
		}
		#endregion

		#region Mise en page
		/// <summary>
		/// Mise en page de la feuille excel.
		/// </summary>
		/// <param name="sheet">Feuille excel.</param>
		/// <param name="name">nom de la feuille excel</param>
		/// <param name="dt">table de données</param>
		/// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>
		/// <param name="s">compteur de page</param>
		/// <param name="upperLeftColumn">colone la plus à gauche</param>
		internal static void PageSettings(Aspose.Excel.Worksheet sheet, string name,DataTable dt,int nbMaxRowByPage,ref int s,int upperLeftColumn){
			PageSettings(sheet,name,dt,nbMaxRowByPage,ref s,upperLeftColumn,"");
		}

	
		/// <summary>
		/// Mise en page de la feuille excel.
		/// </summary>
		/// <param name="sheet">Feuille excel.</param>
		/// <param name="name">nom de la feuille excel</param>
		/// <param name="dt">table de données</param>
		/// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>
		/// <param name="s">compteur de page</param>
		/// <param name="printArea">Zone d'impression</param>
		/// <param name="upperLeftColumn">colone la plus à gauche</param>
		/// <param name="vPageBreaks">saut de page vertical</param>
		internal static void PageSettings(Aspose.Excel.Worksheet sheet, string name,DataTable dt,int nbMaxRowByPage,ref int s,int upperLeftColumn,string vPageBreaks){
			
			int nbPages=0;
			nbPages=(int)Math.Ceiling(dt.Rows.Count*1.0/nbMaxRowByPage);	
			sheet.Name=name; // A mettre dans web word		
			for(s=1;s<=nbPages+1;s++){
				sheet.HPageBreaks.Add(nbMaxRowByPage*s,0,8);
			}									
			
			if(vPageBreaks!=null && vPageBreaks.Length>0)
				sheet.VPageBreaks.Add(vPageBreaks);

			sheet.IsGridlinesVisible = false;
			sheet.PageSetup.Orientation = PageOrientationType.Landscape;
			Aspose.Excel.PageSetup pageSetup =sheet.PageSetup;

			//Set margins, in unit of inches 					
			pageSetup.TopMarginInch = 0.3; 
			pageSetup.BottomMarginInch = 0.3; 
			pageSetup.HeaderMarginInch = 0.3; 
			pageSetup.FooterMarginInch = 0.3; 										
			pageSetup.RightMargin=0.3;
			pageSetup.LeftMargin=0.3;
			pageSetup.CenterHorizontally=true;
//			if(printArea!=null && printArea.Length>0)
//			pageSetup.PrintArea=printArea;
			//Ajout des logos TNS et Bastet
			Pictures pics = sheet.Pictures;
			string tnsLogoPath=@"Images\logoTNSMedia.gif";	
			string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
			pics.Add(0, 0,logoPath);
			string bastetLogoPath=@"Images\Bastet.gif";
			string bastetImagePath = System.IO.Path.GetFullPath(bastetLogoPath);
			pics.Add(0,upperLeftColumn,bastetImagePath);

			//Set current date and current time at the center section of header and change the font of the header
			pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
			//Set current page number at the center section of footer
			pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");			
		}
		#endregion
		
	}
}
