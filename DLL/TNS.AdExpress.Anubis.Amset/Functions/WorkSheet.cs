#region Informations
// Auteur: Y. R'kaina
// Date de création: 05/02/2007
// Date de modification:
#endregion

using System;
using System.Data;
using Aspose.Excel;
using System.Collections;
using System.Drawing;
using TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Anubis.Amset.Functions{
	/// <summary>
	/// Description résumée de WorkSheet.
	/// </summary>
	public class WorkSheet{
	
		#region Mise en page
		/// <summary>
		/// Mise en page de la feuille excel.
		/// </summary>
		/// <param name="sheet">Feuille excel.</param>
		/// <param name="name">nom de la feuille excel</param>	
		/// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>		
		/// <param name="upperLeftColumn">colone la plus à gauche</param>
		/// <param name="vPageBreaks">saut de page vertical</param>
		internal static void PageSettings(Aspose.Excel.Worksheet sheet, string name,int upperLeftColumn){
		
			// Nom de la feuille
			sheet.Name=name; 														
			
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
			
			//Ajout des logos TNS et Appm
			Pictures pics = sheet.Pictures;
			string tnsLogoPath=@"Images\logoTNSMedia.gif";	
			string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
			pics.Add(0, 0,logoPath,75,75);
			string appmLogoPath=@"Images\Common\APPM.bmp";
			string bastetImagePath = System.IO.Path.GetFullPath(appmLogoPath);
			pics.Add(0,upperLeftColumn,bastetImagePath,55,45);
		
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
		/// <param name="printArea">Zone d'impression</param>
		/// <param name="upperLeftColumn">colone la plus à gauche</param>
		/// <param name="vPageBreaks">saut de page vertical</param>
		internal static void PageSettings(Aspose.Excel.Worksheet sheet, string name,int nbRows,int nbMaxRowByPage,int upperLeftColumn,string vPageBreaks,string headerRowIndex){
			int nbPages=0;
			int currentNbRowByPage=nbMaxRowByPage;
			int first=0;
			nbPages=(int)Math.Ceiling(nbRows*1.0/nbMaxRowByPage);	
			sheet.Name=name; // A mettre dans web word		
			for(int s=1;s<nbPages;s++){
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
			Aspose.Excel.PageSetup pageSetup =sheet.PageSetup;

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
			pics.Add(0, 0,logoPath,75,75);
			string appmLogoPath=@"Images\Common\APPM.bmp";
			string bastetImagePath = System.IO.Path.GetFullPath(appmLogoPath);
			pics.Add(0,upperLeftColumn,bastetImagePath,55,45);

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
		internal static void CellsStyle(Aspose.Excel.Cells cells,object data,int row,int firstColumn,int lastColumn,bool isBold,System.Drawing.Color fontColor,System.Drawing.Color foregroundColor,System.Drawing.Color borderColor,CellBorderType rightBorderType,CellBorderType leftBorderType,CellBorderType topBorderType,CellBorderType bottomBorderType,short size,bool textAlignmentTypeCenter){
			for(int i=firstColumn;i<=lastColumn;i++){
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
				cells[row,i].Style.Font.Color = fontColor;
				cells[row,i].Style.Font.IsBold = isBold;
				cells[row,i].Style.Font.Size = size;
				
				if(textAlignmentTypeCenter==true)
					cells[row,i].Style.HorizontalAlignment = TextAlignmentType.Center;
			}			
		}
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
		internal static void CellsStyle(Aspose.Excel.Cells cells,object data,int firstRow,int lastRow,int firstColumn,int lastColumn,bool isBold,System.Drawing.Color fontColor,System.Drawing.Color foregroundColor,System.Drawing.Color borderColor,CellBorderType rightBorderType,CellBorderType leftBorderType,CellBorderType topBorderType,CellBorderType bottomBorderType,short size,bool textAlignmentTypeCenter){
			for(int j=firstRow;j<=lastRow;j++){
				for(int i=firstColumn;i<=lastColumn;i++){
					if(data!=null)cells[j,i].PutValue(data);
				
					cells[j,i].Style.Borders[BorderType.RightBorder].LineStyle = rightBorderType;
					cells[j,i].Style.Borders[BorderType.RightBorder].Color = borderColor;
					cells[j,i].Style.Borders[BorderType.LeftBorder].LineStyle = leftBorderType;
					cells[j,i].Style.Borders[BorderType.LeftBorder].Color = borderColor;
					cells[j,i].Style.Borders[BorderType.TopBorder].LineStyle = topBorderType;
					cells[j,i].Style.Borders[BorderType.TopBorder].Color = borderColor;
					cells[j,i].Style.Borders[BorderType.BottomBorder].LineStyle = bottomBorderType;
					cells[j,i].Style.Borders[BorderType.BottomBorder].Color = borderColor;

					cells[j,i].Style.ForegroundColor =  foregroundColor;					
					cells[j,i].Style.Font.Color = fontColor;
					cells[j,i].Style.Font.IsBold = isBold;
					cells[j,i].Style.Font.Size = size;
				
					if(textAlignmentTypeCenter==true)
						cells[j,i].Style.HorizontalAlignment = TextAlignmentType.Center;
				}			
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
		internal static void PutCellValue(Worksheet sheet,Aspose.Excel.Cells cells,object data,int row,int column,bool isBold,short size,int startColumn){
			cells[row,column].PutValue(data);
			cells[row,column].Style.Font.Size = size;
			if(column>startColumn)
				cells[row,column].Style.HorizontalAlignment = TextAlignmentType.Right;
		}
		#endregion

		#region Render Headers
		/// <summary>
		/// Génère le rendu des Headers pour aspose
		/// </summary>
		/// <param name="sheet">Feuille excel</param>
		/// <param name="cells">Les cellules du tableau</param>
		/// <param name="root">L'arbre qui contient les headers</param>
		/// <param name="cellRow">L'index de la prmière ligne</param>
		/// <param name="cellColumn">L'index de la prmière colonne</param>
		internal static int RenderHeader(Worksheet sheet, Cells cells, HeaderBase root, int cellRow, int cellColumn){
		
			Queue nodes = new Queue();
			int depth = 0;

			//Initialisation niveaux de profondeurs et nombres de feuilles
			GetDepth(root,1, ref depth);

			for(int i = 0; i < root.Count; i++){
				nodes.Enqueue(root[i]);
			}

			HeaderBase cNd;
			object o;

			while(nodes.Count > 0){
				o = nodes.Dequeue();
				cNd = o as HeaderBase;
				Render(sheet,cells,cNd,cellRow,ref cellColumn,depth);
			}
			
			return cellRow+depth;
	
		}
		/// <summary>
		/// Génère le rendu d'un noeud
		/// </summary>
		/// <param name="sheet">Feuille excel</param>
		/// <param name="cells">Les cellules du tableau</param>
		/// <param name="headerBase">HeaderBase</param>
		/// <param name="indexRow">L'index de la prmière ligne</param>
		/// <param name="indexColumn">L'index de la prmière colonne</param>
		/// <param name="depth">La profondeur</param>
		internal static void Render(Worksheet sheet, Cells cells, HeaderBase headerBase, int indexRow, ref int indexColumn, int depth){

			if(headerBase.Count==0){
				PutCellValue(sheet,cells,headerBase.Label,indexRow,indexColumn,false,8,2);
				cells.Merge(indexRow,indexColumn,depth,headerBase.ColSpan);
				CellsStyle(cells,null,indexRow,indexRow+(depth-1),indexColumn,indexColumn,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,8,true);
				indexColumn++;
			}
			else{
				PutCellValue(sheet,cells,headerBase.Label,indexRow,indexColumn,false,8,2);
				cells.Merge(indexRow,indexColumn,depth-1,headerBase.ColSpan);
				CellsStyle(cells,null,indexRow,indexColumn,indexColumn+headerBase.ColSpan-1,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,8,true);

				for(int i=0; i<headerBase.Count;i++)
					Render(sheet,cells,headerBase[i],indexRow+1,ref indexColumn,depth-1);

			}
		
		}

		/// <summary>
		/// Préparation du rendu des entêtes
		/// </summary>
		/// <param name="level">Niveau</param>
		/// <param name="depth">Profondeur</param>
		/// <returns>colSpan</returns>
		internal static int GetDepth(HeaderBase root, int level, ref int depth){
			if (level>depth) depth = level;
			int i = 0;

			for(int j=0;j<root.Count;j++){
				if (root[j].Count > 0){
					i += GetDepth(root[j],level+1, ref depth);
				}
				else{
					i++;
				}
			}
			root.ColSpan = i;
			return i;
		}
		#endregion
	}
}
