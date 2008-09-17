
#region Informations
// Auteur: D. V. Mussuma
// Date de création: 28/11/2005
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
namespace TNS.AdExpress.Anubis.Bastet.UI
{
	/// <summary>
	///  Top des resultats (onglets) les plus utilisés dans AdExpress 
	/// </summary>
	public class Tab
	{

		/// <summary>
		/// Top des résultats (onglets)les plus utilisés dans AdExpress 
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>				
		/// <returns>objet excel</returns>
		internal static Excel TopUsed(Excel excel,BastetCommon.Parameters parameters) {
			try{
				//Chargement des données des résultats
				DataTable dt = DataAccess.Tab.TopUsed(parameters);
		
				if(dt!=null && dt.Rows.Count>0){
					//Variables				
					int cellRow =5;			
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					sheet.Name="Top options utilisées"; // A mettre dans web word		

					//Saut de page horizontal
					int nbPages=0;
					const int nbMaxRowByPage=40;
					nbPages=(int)Math.Ceiling(dt.Rows.Count*1.0/nbMaxRowByPage);
					for(int s=1;s<=nbPages+1;s++){
						sheet.HPageBreaks.Add(nbMaxRowByPage*s,0,8);
					}	
					Cells cells = sheet.Cells;

					sheet.IsGridlinesVisible = false;
					sheet.PageSetup.Orientation = PageOrientationType.Landscape;
					Aspose.Excel.PageSetup pageSetup =sheet.PageSetup;
				
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
					pics.Add(0, 5,bastetImagePath);

					//Set current date and current time at the center section of header and change the font of the header
					pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
					//Set current page number at the center section of footer
					pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");


					#region Top options utilisées	
					
					cells["B"+cellRow].PutValue(" Module ");// A mettre dans web word
					cells["B"+cellRow].Style.Font.IsBold = true;	
					cells["B"+cellRow].Style.Font.Color = Color.White;
					cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

					cells["C"+cellRow].PutValue(" Top options utilisées ");// A mettre dans web word
					cells["C"+cellRow].Style.Font.IsBold = true;	
					cells["C"+cellRow].Style.Font.Color = Color.White;
					cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

					cells["D"+cellRow].PutValue(" Nombre d'utilisation ");// A mettre dans web word
					cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["D"+cellRow].Style.Font.IsBold = true;
					cells["D"+cellRow].Style.Font.Color = Color.White;
					cells["D"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

					cellRow++;
				
					//Pour chaque résultat
					for(int i=0; i<dt.Rows.Count;i++){	
						cells["B"+cellRow].PutValue(dt.Rows[i]["MODULE"].ToString());
						cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
										
						cells["C"+cellRow].PutValue(dt.Rows[i]["result"].ToString());
						cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

						cells["D"+cellRow].PutValue(Int64.Parse(dt.Rows[i]["CONNECTION_NUMBER"].ToString()));
						cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;		
											
						cellRow++;
					}
					#endregion

				
					//Ajustement de la taile des cellules en fonction du contenu
					sheet.AutoFitColumn(1);	
					sheet.AutoFitColumn(2);
					sheet.AutoFitColumn(3);
				}
			}catch(Exception err){
				throw (new  BastetExceptions.TabUIException(" TopUsed : Impossible d'obtenir la liste des options (onglets) les plus utilisés ", err));
			}
			return excel;
		}
	}
}
