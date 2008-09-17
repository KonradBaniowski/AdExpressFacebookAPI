#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 25/11/2005
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
//using TNS.AdExpress.Anubis.Bastet.Rules;
using BastetExceptions=TNS.AdExpress.Anubis.Bastet.Exceptions;

namespace TNS.AdExpress.Anubis.Bastet.UI
{
	/// <summary>
	///Top export de fichiers
	/// </summary>
	public class Export
	{
		/// <summary>
		/// Top export de fichiers
		/// </summary>
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>				
		/// <returns>objet excel</returns>
		internal static Excel Top(Excel excel,BastetCommon.Parameters parameters) {			
			try{
				//Chargement des donn�es du Top export de fichiers
				DataTable dt = DataAccess.Export.Top(parameters);

				if(dt!=null && dt.Rows.Count>0){
					//Variables
					int cellRow =5;							
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					sheet.Name="Top export de fichiers"; // A mettre dans web word	
			
					//Saut de page horizontal
					int nbPages=0;
					const int nbMaxRowByPage=40;
					nbPages=(int)Math.Ceiling(dt.Rows.Count*1.0/nbMaxRowByPage);
					for(int s=1;s<=nbPages+1;s++){
						sheet.HPageBreaks.Add(nbMaxRowByPage*s,0,8);
					}	

					Cells cells = sheet.Cells;			
					int startIndex=cellRow;

					sheet.IsGridlinesVisible = false;
					sheet.PageSetup.Orientation = PageOrientationType.Landscape;
					Aspose.Excel.PageSetup pageSetup = sheet.PageSetup;
				
					//Set margins, in unit of inches 					
					pageSetup.TopMarginInch = 0.3; 
					pageSetup.BottomMarginInch = 0.3; 
					pageSetup.HeaderMarginInch = 0.3; 
					pageSetup.FooterMarginInch = 0.3; 

					//Ajout du logo TNS
					Pictures pics = sheet.Pictures;
					string tnsLogoPath=@"Images\logoTNSMedia.gif";	
					string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
					pics.Add(0, 0,logoPath);
					string bastetLogoPath=@"Images\Bastet.gif";
					string bastetImagePath = System.IO.Path.GetFullPath(bastetLogoPath);
					pics.Add(0, 6,bastetImagePath);

					//Set current date and current time at the center section of header and change the font of the header
					pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
					//Set current page number at the center section of footer
					pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");

					//Tous les logins
					if(parameters!=null && parameters.Logins.Length==0){
		
						#region Top export
				
						//Top utilisateurs du fichier GAD				
						cells["B"+startIndex].PutValue("Top export de fichiers");// A mettre dans web word
						cells["B"+startIndex].Style.Font.IsBold = true;
						cells["B"+cellRow].Style.Font.Color = Color.White;
						cells["B"+startIndex].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["B"+startIndex].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["B"+startIndex].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["B"+startIndex].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
						cells["B"+startIndex].Style.ForegroundColor = Color.FromArgb(128,128,192);

						cells["C"+startIndex].PutValue(" Logins ");// A mettre dans web word
						cells["C"+startIndex].Style.Font.IsBold = true;
						cells["C"+cellRow].Style.Font.Color = Color.White;
						cells["C"+startIndex].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["C"+startIndex].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["C"+startIndex].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
						cells["C"+startIndex].Style.ForegroundColor = Color.FromArgb(128,128,192);

						cells["D"+startIndex].PutValue(" Nombre d'export ");// A mettre dans web word
						cells["D"+startIndex].Style.Font.IsBold = true;
						cells["D"+cellRow].Style.Font.Color = Color.White;
						cells["D"+startIndex].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["D"+startIndex].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["D"+startIndex].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
						cells["D"+startIndex].Style.ForegroundColor = Color.FromArgb(128,128,192);
					
						cellRow++;

						//Pour les 10 1ers �l�ments
						for(int i=0; i<dt.Rows.Count;i++){	
							//soci�t�									
							cells["B"+cellRow].PutValue(dt.Rows[i]["COMPANY"].ToString());// A mettre dans web word
							cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
						
							//Logins
							cells["C"+cellRow].PutValue(dt.Rows[i]["LOGIN"].ToString());
							cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
							cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

							//nombre d'utilisation
							cells["D"+cellRow].PutValue(Int64.Parse(dt.Rows[i]["CONNECTION_NUMBER"].ToString()));
							cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;						
							cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;	
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
																
							cellRow++;
						}
				
						//Total
						cells["B"+cellRow].PutValue(" Total ");// A metre dans web word
						cells["B"+cellRow].Style.Font.Color = Color.Red;
						cells["B"+cellRow].Style.Font.IsBold = true;
						cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

						cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
						cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

						cells["D"+cellRow].PutValue(Int64.Parse(dt.Compute("sum(CONNECTION_NUMBER)","").ToString()));
						cells["D"+cellRow].Style.Font.Color = Color.Red;
						cells["D"+cellRow].Style.Font.IsBold = true;
						cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
						cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;	
						cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
					
						#endregion

					}else{
						//Logins s�lectionn�s
						if(dt!=null && dt.Rows.Count>0){
							cells["B"+startIndex].PutValue(" Nombre d'exports de fichiers ");// A mettre dans web word
							cells["B"+startIndex].Style.Font.IsBold = true;	
							cells["B"+cellRow].Style.Font.Color = Color.White;
							cells["B"+startIndex].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
							cells["B"+startIndex].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["B"+startIndex].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["B"+startIndex].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["B"+startIndex].Style.ForegroundColor = Color.FromArgb(128,128,192);

							cells["C"+startIndex].PutValue(Int64.Parse(dt.Compute("sum(CONNECTION_NUMBER)","").ToString()));
							cells["C"+startIndex].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;						
							cells["C"+startIndex].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["C"+startIndex].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["C"+startIndex].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

						}
					}
				
					//Ajustement de la taile des cellules en fonction du contenu
					sheet.AutoFitColumn(1);	
					sheet.AutoFitColumn(2);
					sheet.AutoFitColumn(3);

				}
			}catch(Exception err){
				throw (new BastetExceptions.ExportUIException(" Top : Impossible d'obtenir des clients effectuant le plus d'export excel ", err));
			}
			return excel;

		}
	}
}
