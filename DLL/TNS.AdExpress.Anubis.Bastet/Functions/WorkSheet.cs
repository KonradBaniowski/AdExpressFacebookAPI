#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 02/03/2006
// Date de modification:
#endregion

using System;
using System.Data;
using Aspose.Cells;
using System.Drawing;
using TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.DB;
using ConstantesTracking=TNS.AdExpress.Constantes.Tracking;
using BastetFunctions=TNS.AdExpress.Anubis.Bastet.Functions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Translation;
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
		/// <param name="dt">table de donn�es</param>
		/// <param name="isBold">vrai si police en  gras</param>
		/// <param name="color">couleur de la police</param>
		internal static void SetConnectionByTimeSliceToCells(Aspose.Cells.Cells cells,int row,int column,DataTable dt,bool isBold,System.Drawing.Color color){			
	
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
		/// Insert les donn�es des connections par mois (ou jour nomm�) par client
		/// </summary>
		/// <param name="dt">table de donn�es</param>
		/// <param name="sheet">feuille excel</param>
		/// <param name="cells">cellules excel</param>
		/// <param name="cellRow">ligne</param>
		/// <param name="i">index colonne</param>
		/// <param name="j">Index colonne</param>
		/// <param name="category">cat�gorie de valeurs</param>
		/// <param name="serial">s�rie de valeurs</param>
		/// <param name="sheetName">Nom de la feuille excel</param>
		/// <param name="clientParameter">param�tre cleint</param>
		internal static void SetConnectionByDate(DataTable dt, Worksheet sheet, Aspose.Cells.Cells cells, ref int cellRow, ref int i, ref int j, ref string category, ref string serial, string sheetName, string clientParameter, ConstantesTracking.Period.Type periodType, int language) {
			
			string expression="";
			string date="";
			object totalConnectionByDate=0;
			
			
			//Libell�s des mois
			PutCellValue(cells," "+GestionWeb.GetWebWord(1132,language)+" ",cellRow-1,0,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);	
			PutCellValue(cells,sheetName,cellRow-1,1,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,1,1,true,Color.White);	
															
			for(i=3;i<dt.Columns.Count;i++){	
				switch(periodType){
					case ConstantesTracking.Period.Type.monthly :
						date = MonthString.GetCharacters(int.Parse(dt.Columns[i].ColumnName.Substring(10, 2)), WebApplicationParameters.AllowedLanguages[language].CultureInfo, 3)
						+". "+dt.Columns[i].ColumnName.Substring(8,2);
						break;
					case ConstantesTracking.Period.Type.dayly :						
						date = BastetFunctions.Period.GetNamedDay(int.Parse(dt.Columns[i].ColumnName.Substring(4,1)),language);
						break;
				}
				PutCellValue(cells,date,cellRow-1,i-1,true,Color.Black);
				CellsHeaderStyle(cells,null,cellRow-1,i-1,i-1,true,Color.White);						
			}
					
			PutCellValue(cells," " + GestionWeb.GetWebWord(1401,language) +" ",cellRow-1,i-1,true,Color.Black);
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
			PutCellValue(cells, GestionWeb.GetWebWord(1401,language), cellRow - 1, 0, true, Color.Red);
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
		/// Insert les donn�es des connections par mois par client
		/// </summary>
		/// <param name="dt">table de donn�es</param>
		/// <param name="sheet">feuille excel</param>
		/// <param name="cells">cellules excel</param>
		/// <param name="cellRow">ligne</param>
		/// <param name="i">index colonne</param>
		/// <param name="j">Index colonne</param>
		/// <param name="category">cat�gorie de valeurs</param>
		/// <param name="serial">s�rie de valeurs</param>
		/// <param name="sheetName">Nom de la feuille excel</param>
		/// <param name="clientParameter">param�tre cleint</param>
		internal static void SetConnectionByClientByDate(DataTable dt, Worksheet sheet,Aspose.Cells.Cells cells,ref int cellRow,ref int i,ref int j,ref string category,ref string serial,string sheetName,string clientParameter,ConstantesTracking.Period.Type periodType,int language){
			
			string expression="";
			string date="";
			object totalConnectionByDate=0;

			//Libell�s des mois ou jour	
			PutCellValue(cells,sheetName,cellRow-1,0,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);	
															
			for(i=3;i<dt.Columns.Count;i++){	
				switch(periodType){
					case ConstantesTracking.Period.Type.monthly :
						date = MonthString.GetCharacters(int.Parse(dt.Columns[i].ColumnName.Substring(10, 2)),WebApplicationParameters.AllowedLanguages[language].CultureInfo, 3)
							+". "+dt.Columns[i].ColumnName.Substring(8,2);
						break;
					case ConstantesTracking.Period.Type.dayly :
						date = BastetFunctions.Period.GetNamedDay(int.Parse(dt.Columns[i].ColumnName.Substring(4, 1)), language);
						break;
				}				
				WorkSheet.PutCellValue(cells,date,cellRow-1,i-2,true,Color.Black);
				WorkSheet.CellsHeaderStyle(cells,null,cellRow-1,i-2,i-2,true,Color.White);						
			}

			PutCellValue(cells, " " + GestionWeb.GetWebWord(1401,language)+" ", cellRow - 1, i - 2, true, Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,i-2,i-2,true,Color.White);
			category="B4:"+cells[(cellRow-1),(i-3)].Name;

			cellRow++;
						
			foreach(DataRow dr in  dt.Rows){
				//Connexions par login et par mois (ou jour)
				PutCellValue(cells,dr[clientParameter].ToString(),cellRow-1,0,false,Color.Black);						
				for(j=3;j<dt.Columns.Count;j++){
					PutCellValue(cells,dr[j],cellRow-1,j-2,false,Color.Black);							
				}
						
				//total colonne						
				PutCellValue(cells,dr["connection_number"],cellRow-1,j-2,false,Color.Black);												
				cellRow++;
					
			}
						
			//Total lignes
			PutCellValue(cells, GestionWeb.GetWebWord(1401,language), cellRow - 1, 0, true, Color.Red);			
					
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

		#region Insertion d'une donn�e dans une cellule
		/// <summary>
		/// Insert une donn�e dans une cellule
		/// </summary>
		/// <param name="cells">Cellules</param>
		/// <param name="data">donn�e</param>
		/// <param name="row">ligne</param>
		/// <param name="column">colonne</param>
		/// <param name="isBold">vrai si police en gras</param>
		/// <param name="color">couleur de la police</param>
		internal static void PutCellValue(Aspose.Cells.Cells cells,object data,int row,int column,bool isBold,System.Drawing.Color color){
			cells[row,column].PutValue(data);
			cells[row,column].Style.Font.Color = color;
			cells[row,column].Style.Font.IsBold = isBold;
			cells[row,column].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;	
			cells[row,column].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;										
			cells[row,column].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
			cells[row,column].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;			
		}
		#endregion

		#region Style des cellules en-t�tes
		/// <summary>
		/// Met le style des cellules en-t�tes
		/// </summary>
		/// <param name="cells">cellules</param>
		/// <param name="data">donn�e</param>
		/// <param name="row">ligne</param>
		/// <param name="firstColumn">1ere colonne de la collection</param>
		/// <param name="lastColumn">derni�re colonne de la collection</param>
		/// <param name="isBold">vrai si police en gras</param>
		/// <param name="color">Couleur de la police</param>
		internal static void CellsHeaderStyle(Aspose.Cells.Cells cells,object data,int row,int firstColumn,int lastColumn,bool isBold,System.Drawing.Color color){
			for(int i=firstColumn;i<=lastColumn;i++){
				if(data!=null)cells[row,i].PutValue(data);
				cells[row,i].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells[row,i].Style.ForegroundColor =  Color.FromArgb(128,128,192);
				cells[row, i].Style.Pattern = BackgroundType.Solid;
				cells[row,i].Style.Font.Color = color;
				cells[row,i].Style.Font.IsBold = isBold;				
			}			
		}
		#endregion

		#region Connections par m�dia et par module
		/// <summary>
		/// Insert les donn�es des connections par m�dia et par module
		/// </summary>
		/// <param name="dt">table de donn�es</param>
		/// <param name="sheet">feuille excel</param>
		/// <param name="cells">cellules excel</param>
		/// <param name="cellRow">ligne</param>
		/// <param name="i">index colonne</param>
		/// <param name="j">Index colonne</param>
		/// <param name="category">cat�gorie de valeurs</param>
		/// <param name="serial">s�rie de valeurs</param>
		/// <param name="sheetName">Nom de la feuille excel</param>
		/// <param name="clientParameter">param�tre cleint</param>
		internal static void SetTopMediaByModule(DataTable dt, Worksheet sheet,Aspose.Cells.Cells cells,ref int cellRow,ref int i,ref int j,ref string category,ref string serial,int language){
			
			double totalConnectionByModule=0;						
			//Libell�s des mois
			PutCellValue(cells, " "+GestionWeb.GetWebWord(2530,language)+" ", cellRow - 1, 0, true, Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);					
			
			//libell�s m�dias
			for(i=0;i<dt.Rows.Count;i++){
				PutCellValue(cells,dt.Rows[i]["vehicle"],cellRow-1,i+1,true,Color.Black);
				CellsHeaderStyle(cells,null,cellRow-1,i+1,i+1,true,Color.White);
			}
			category="B4:"+cells[(cellRow-1),(i)].Name;
			//libell� total ligne		
			PutCellValue(cells,  GestionWeb.GetWebWord(1401,language) , cellRow - 1, i + 1, true, Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,i+1,i+1,true,Color.White);
			cellRow++;
			
			//Insertion des valeurs de connections dans cellule excel
			i=0;
			//Pour chaque module
			for(j=3;j<dt.Columns.Count;j++){
				PutCellValue(cells,dt.Columns[j].ColumnName,cellRow-1,0,true,Color.Black);
				
				//Pour chaque m�dia
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
					
			//Total colonne m�dia
			PutCellValue(cells, GestionWeb.GetWebWord(1401, language), cellRow - 1, 0, true, Color.Red);		
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
		/// Insert les donn�es Adresse IP par client
		/// </summary>
		/// <param name="dt">table de donn�es</param>
		/// <param name="sheet">feuille excel</param>
		/// <param name="cells">cellules excel</param>
		/// <param name="cellRow">ligne</param>
		/// <param name="i">index colonne</param>
		/// <param name="j">Index colonne</param>
		/// <param name="category">cat�gorie de valeurs</param>
		/// <param name="serial">s�rie de valeurs</param>
		/// <param name="sheetName">Nom de la feuille excel</param>
		/// <param name="clientParameter">param�tre client</param>
		internal static void SetIPAdressByClient(DataTable dt, Worksheet sheet, Aspose.Cells.Cells cells, ref int cellRow, string sheetName, int language) {
											
			Int64 oldIdLogin=0;
			Int64 oldIdCompany=0;

			//Libell�s des clients
			PutCellValue(cells,GestionWeb.GetWebWord(1132,language),cellRow-1,0,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,0,0,true,Color.White);	
			PutCellValue(cells,sheetName,cellRow-1,1,true,Color.Black);
			CellsHeaderStyle(cells,null,cellRow-1,1,1,true,Color.White);
			PutCellValue(cells,GestionWeb.GetWebWord(2532,language),cellRow-1,2,true,Color.Black);
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
		/// <param name="dt">table de donn�es</param>
		/// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>
		/// <param name="s">compteur de page</param>
		/// <param name="upperLeftColumn">colone la plus � gauche</param>
		internal static void PageSettings(Aspose.Cells.Worksheet sheet, string name,DataTable dt,int nbMaxRowByPage,ref int s,int upperLeftColumn,int language){
			PageSettings(sheet,name,dt,nbMaxRowByPage,ref s,upperLeftColumn,"",language);
		}

	
		/// <summary>
		/// Mise en page de la feuille excel.
		/// </summary>
		/// <param name="sheet">Feuille excel.</param>
		/// <param name="name">nom de la feuille excel</param>
		/// <param name="dt">table de donn�es</param>
		/// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>
		/// <param name="s">compteur de page</param>
		/// <param name="printArea">Zone d'impression</param>
		/// <param name="upperLeftColumn">colone la plus � gauche</param>
		/// <param name="vPageBreaks">saut de page vertical</param>
		internal static void PageSettings(Aspose.Cells.Worksheet sheet, string name, DataTable dt, int nbMaxRowByPage, ref int s, int upperLeftColumn, string vPageBreaks, int language) {
			
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
			Aspose.Cells.PageSetup pageSetup =sheet.PageSetup;

			//Set margins, in unit of inches 					
			pageSetup.TopMarginInch = 0.3; 
			pageSetup.BottomMarginInch = 0.3; 
			pageSetup.HeaderMarginInch = 0.3; 
			pageSetup.FooterMarginInch = 0.3; 										
			pageSetup.RightMargin=0.3;
			pageSetup.LeftMargin=0.3;
			pageSetup.CenterHorizontally=true;

			//Ajout des logos TNS et Bastet
			Pictures pics = sheet.Pictures;
			string tnsLogoPath = TNS.AdExpress.Anubis.Bastet.Constantes.Images.LOGO_TNS;
			string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
			int picIndex = pics.Add(0, 0,logoPath);
			pics[picIndex].Placement = Aspose.Cells.PlacementType.Move;

			string bastetLogoPath = TNS.AdExpress.Anubis.Bastet.Constantes.Images.LOGO_BASTET;
			string bastetImagePath = System.IO.Path.GetFullPath(bastetLogoPath);
			picIndex = pics.Add(0,upperLeftColumn,bastetImagePath);
			pics[picIndex].Placement = Aspose.Cells.PlacementType.Move;

			//Set current date and current time at the center section of header and change the font of the header
			pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
			//Set current page number at the center section of footer
			pageSetup.SetFooter(1, "&A" + " - " + GestionWeb.GetWebWord(894, language) + " " + "&P" + " " + GestionWeb.GetWebWord(2042, language) + " " + "&N");
		
		}
		#endregion
		
	}
}
