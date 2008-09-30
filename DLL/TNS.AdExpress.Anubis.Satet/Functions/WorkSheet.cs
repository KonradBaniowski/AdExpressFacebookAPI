#region Informations
// Auteur: D. V. Mussuma
// Date de création: 29/05/2006
// Date de modification:
#endregion

using System;
using System.Data;
using Aspose.Cells;
using System.Drawing;
using TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.DB;


namespace TNS.AdExpress.Anubis.Satet.Functions
{
	/// <summary>
	/// Description résumée de WorkSheet.
	/// </summary>
	public class WorkSheet
	{
		#region Mise en page

		/// <summary>
		/// Mise en page de la feuille excel.
		/// </summary>
		/// <param name="sheet">Feuille excel.</param>
		/// <param name="name">nom de la feuille excel</param>	
		/// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>		
		/// <param name="upperLeftColumn">colone la plus à gauche</param>
		/// <param name="vPageBreaks">saut de page vertical</param>
		internal static void PageSettings(Aspose.Cells.Worksheet sheet, string name,int upperLeftColumn){
            int indexPicture = 0;
			// Nom de la feuille
			sheet.Name=name; 														
			
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
			
			//Ajout des logos TNS et Appm
			Pictures pics = sheet.Pictures;
			string tnsLogoPath=@"Images\logoTNSMedia.gif";	
			string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
            indexPicture = pics.Add(0, 0, logoPath, 75, 75);
            sheet.Pictures[indexPicture].Placement = PlacementType.Move;
			string appmLogoPath=@"Images\Common\APPM.bmp";
			string bastetImagePath = System.IO.Path.GetFullPath(appmLogoPath);
            indexPicture = pics.Add(0, upperLeftColumn, bastetImagePath, 55, 45);
            sheet.Pictures[indexPicture].Placement = PlacementType.Move;
		
			//Set current date and current time at the center section of header and change the font of the header
			pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
			//Set current page number at the center section of footer
			pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");			
		}

		/// <summary>
		/// Mise en page de la feuille excel.
		/// </summary>
		/// <param name="sheet">Feuille excel.</param>
		/// <param name="name">nom de la feuille excel</param>
		/// <param name="nbRows">Nombre de lignes</param>
		/// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>
		/// <param name="s">compteur de page</param>
		/// <param name="upperLeftColumn">colone la plus à gauche</param>
		internal static void PageSettings(Aspose.Cells.Worksheet sheet, string name,int nbRows,int nbMaxRowByPage,ref int s,int upperLeftColumn){
			PageSettings(sheet,name,nbRows,nbMaxRowByPage,ref s,upperLeftColumn,"",null);
		}

	
		/// <summary>
		/// Mise en page de la feuille excel.
		/// </summary>
		/// <param name="sheet">Feuille excel.</param>
		/// <param name="name">nom de la feuille excel</param>
		/// <param name="nbRows">Nombre de lignes</param>
		/// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>
		/// <param name="s">compteur de page</param>
		/// <param name="printArea">Zone d'impression</param>
		/// <param name="upperLeftColumn">colone la plus à gauche</param>
		/// <param name="vPageBreaks">saut de page vertical</param>
		internal static void PageSettings(Aspose.Cells.Worksheet sheet, string name,int nbRows,int nbMaxRowByPage,ref int s,int upperLeftColumn,string vPageBreaks,string headerRowIndex){
			
			int nbPages=0;
			int currentNbRowByPage=nbMaxRowByPage;
			int first=0;
            int indexPicture = 0;
			nbPages=(int)Math.Ceiling(nbRows*1.0/nbMaxRowByPage);	
			sheet.Name=name; // A mettre dans web word		
			for(s=1;s<nbPages;s++){
				if(first==0){
					sheet.HPageBreaks.Add(nbMaxRowByPage*s,0,6);
					first=1;
				}
				else{
					currentNbRowByPage+=38;
					sheet.HPageBreaks.Add(currentNbRowByPage,0,6);
				}
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
			if(headerRowIndex!=null){
				pageSetup.PrintTitleRows="$"+headerRowIndex+":$4";//+headerRowIndex;
			}

			//Ajout des logos TNS et Appm
			Pictures pics = sheet.Pictures;
			string tnsLogoPath=@"Images\logoTNSMedia.gif";	
			string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
            indexPicture = pics.Add(0, 0, logoPath, 75, 75);
            sheet.Pictures[indexPicture].Placement = PlacementType.Move;
			string appmLogoPath=@"Images\Common\APPM.bmp";
			string bastetImagePath = System.IO.Path.GetFullPath(appmLogoPath);
            indexPicture = pics.Add(0, upperLeftColumn, bastetImagePath, 55, 45);
            sheet.Pictures[indexPicture].Placement = PlacementType.Move;

			//Set current date and current time at the center section of header and change the font of the header
			pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
			//Set current page number at the center section of footer
			pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");			
		}

		/// <summary>
		/// Mise en page de la feuille excel.
		/// </summary>
		/// <param name="sheet">Feuille excel.</param>
		/// <param name="name">nom de la feuille excel</param>
		/// <param name="s">compteur de page</param>
		/// <param name="upperLeftColumn">colone la plus à gauche</param>
		/// <param name="headerRowIndex">En-tête</param>
		internal static void PageSettings(Aspose.Cells.Worksheet sheet, string name,ref int s,int upperLeftColumn,string headerRowIndex){
            int indexPicture = 0;
			sheet.Name=name; // A mettre dans web word		
													
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
			pageSetup.FitToPagesTall=32000;
			pageSetup.FitToPagesWide=1;
			if(headerRowIndex!=null){
				pageSetup.PrintTitleRows="$"+headerRowIndex+":$4";//+headerRowIndex;
			}

			//Ajout des logos TNS et Appm
			Pictures pics = sheet.Pictures;
			string tnsLogoPath=@"Images\logoTNSMedia.gif";	
			string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
            indexPicture = pics.Add(0, 0, logoPath, 75, 75);
            sheet.Pictures[indexPicture].Placement = PlacementType.Move;
			string appmLogoPath=@"Images\Common\APPM.bmp";
			string bastetImagePath = System.IO.Path.GetFullPath(appmLogoPath);
            indexPicture = pics.Add(0, upperLeftColumn, bastetImagePath, 55, 45);
            sheet.Pictures[indexPicture].Placement = PlacementType.Move;

			//Set current date and current time at the center section of header and change the font of the header
			pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
			//Set current page number at the center section of footer
			pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");			
		}
		#endregion

		#region Style des cellules 
		/// <summary>
		/// Met le style des cellules en-têtes
		/// </summary>
		/// <param name="cells">cellules</param>
		/// <param name="data">donnée</param>
		/// <param name="row">ligne</param>
		/// <param name="firstColumn">1ere colonne de la collection</param>
		/// <param name="lastColumn">dernière colonne de la collection</param>
		/// <param name="isBold">vrai si police en gras</param>
		/// <param name="fontColor">Couleur de la police</param>
		/// <param name="foregroundColor">Couleur de fond</param>
		///<param name="borderColor">Couleur de la bordure</param>
		///<param name="rightBorderType">Type de la bordure droite</param>
		///<param name="leftBorderType">Type de la bordure gauche</param>
		///<param name="topBorderType">Type de la bordure en haut</param>
		///<param name="bottomBorderType">Type de la bordure en bas</param>
		/// <param name="size">Taille du texte</param>
		/// <param name="textAlignmentTypeCenter">Alignement du texte</param>
		internal static void CellsStyle(Aspose.Cells.Cells cells,object data,int row,int firstColumn,int lastColumn,bool isBold,System.Drawing.Color fontColor,System.Drawing.Color foregroundColor,System.Drawing.Color borderColor,CellBorderType rightBorderType,CellBorderType leftBorderType,CellBorderType topBorderType,CellBorderType bottomBorderType,short size,bool textAlignmentTypeCenter)
		{
			for(int i=firstColumn;i<=lastColumn;i++)
			{
				if(data!=null)cells[row,i].PutValue(data);
				
				cells[row,i].Style.Borders[BorderType.RightBorder].LineStyle = rightBorderType;
				cells[row,i].Style.Borders[BorderType.RightBorder].Color = borderColor;
				cells[row,i].Style.Borders[BorderType.LeftBorder].LineStyle = leftBorderType;
				cells[row,i].Style.Borders[BorderType.LeftBorder].Color = borderColor;
				cells[row,i].Style.Borders[BorderType.TopBorder].LineStyle = topBorderType;
				cells[row,i].Style.Borders[BorderType.TopBorder].Color = borderColor;
				cells[row,i].Style.Borders[BorderType.BottomBorder].LineStyle = bottomBorderType;
				cells[row,i].Style.Borders[BorderType.BottomBorder].Color = borderColor;

				cells[row,i].Style.ForegroundColor =  foregroundColor;
                cells[row, i].Style.Pattern = BackgroundType.Solid;
				cells[row,i].Style.Font.Color = fontColor;
				cells[row,i].Style.Font.IsBold = isBold;
				cells[row,i].Style.Font.Size = size;
				if(textAlignmentTypeCenter==true)
					cells[row,i].Style.HorizontalAlignment = TextAlignmentType.Center;
				//				cells[row,i].Style.Number = CellFormat
			}			
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
		/// <param name="size">Taille du texte</param>
		internal static void PutCellValue(Worksheet sheet,Aspose.Cells.Cells cells,object data,int row,int column,bool isBold,System.Drawing.Color color,short size,int startColumn)
		{
			cells[row,column].PutValue(data);
			cells[row,column].Style.Font.Size = size;
			if(column>=startColumn)
				cells[row,column].Style.HorizontalAlignment = TextAlignmentType.Center;

			//sheet.AutoFitColumn(column);	
			
		}
		#endregion
	}
}
