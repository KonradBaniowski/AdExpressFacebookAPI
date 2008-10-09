#region Informations
// Auteur: D. V. Mussuma
// Date de création: 28/11/2005
// Date de modification:
#endregion

using System;
using Aspose.Cells;
using System.Drawing;
using System.Data;
using TNS.AdExpress.Anubis.Bastet;
using BastetCommon=TNS.AdExpress.Bastet.Common;
//using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.DB;
//using TNS.AdExpress.Anubis.Bastet.Rules;
using BastetExceptions=TNS.AdExpress.Anubis.Bastet.Exceptions;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Anubis.Bastet.UI
{
	/// <summary>
	///unités les plus utilisés dans AdExpress 
	/// </summary>
	public class Unit
	{
		/// <summary>
		/// Top des unités les plus utilisés dans AdExpress 
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>				
		/// <returns>objet excel</returns>
		internal static Workbook TopUsed(Workbook excel, BastetCommon.Parameters parameters, int language) {
			try{
				//Chargement des données des résultats
				DataTable dt = DataAccess.Unit.TopUsed(parameters);
			
		
				if(dt!=null && dt.Rows.Count>0){
					//Insertion dans fichier excel

					//Variables								
					int cellRow =5;			
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					sheet.Name = GestionWeb.GetWebWord(2528, language); 
				
					//Saut de page horizontal
					int nbPages=0;
					const int nbMaxRowByPage=40;
					nbPages=(int)Math.Ceiling(dt.Rows.Count*1.0/nbMaxRowByPage);
					for(int s=1;s<=nbPages+1;s++){
						sheet.HPageBreaks.Add(nbMaxRowByPage*s,0,8);
					}	

					Cells cells = sheet.Cells;
					//				DataTable unitWord;

					sheet.IsGridlinesVisible = false;
					sheet.PageSetup.Orientation = PageOrientationType.Landscape;
					Aspose.Cells.PageSetup pageSetup =sheet.PageSetup;
				
					//Set margins, in unit of inches 					
					pageSetup.TopMarginInch = 0.3; 
					pageSetup.BottomMarginInch = 0.3; 
					pageSetup.HeaderMarginInch = 0.3; 
					pageSetup.FooterMarginInch = 0.3; 

					//Ajout du logo TNS
					Pictures pics = sheet.Pictures;
					string tnsLogoPath = TNS.AdExpress.Anubis.Bastet.Constantes.Images.LOGO_TNS;	
					string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
					int picIndex = pics.Add(0, 0,logoPath);
					pics[picIndex].Placement = Aspose.Cells.PlacementType.Move;

					string bastetLogoPath = TNS.AdExpress.Anubis.Bastet.Constantes.Images.LOGO_BASTET;
					string bastetImagePath = System.IO.Path.GetFullPath(bastetLogoPath);
					picIndex = pics.Add(0, 6, bastetImagePath);
					pics[picIndex].Placement = Aspose.Cells.PlacementType.Move;

					//Set current date and current time at the center section of header and change the font of the header
					pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
					//Set current page number at the center section of footer
					pageSetup.SetFooter(1, "&A" + " - " + GestionWeb.GetWebWord(894, language) + " " + "&P" + " " + GestionWeb.GetWebWord(2042, language) + " " + "&N");



					#region Top unités utilisé	

					//Top unités utilisés
					cells["B" + cellRow].PutValue(" "+GestionWeb.GetWebWord(2528, language)+" ");
					cells["B"+cellRow].Style.Font.IsBold = true;
					cells["B"+cellRow].Style.Font.Color = Color.White;
					cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
					cells["B" + cellRow].Style.Pattern = BackgroundType.Solid;

					cells["C" + cellRow].PutValue(" " + GestionWeb.GetWebWord(2515, language) + " ");
					cells["C"+cellRow].Style.Font.Color = Color.White;
					cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Font.IsBold = true;
					cells["C"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
					cells["C" + cellRow].Style.Pattern = BackgroundType.Solid;
					cellRow++;

				
					//top éléments
				
					for(int i=0; i<dt.Rows.Count;i++){	
						
						cells["B"+cellRow].PutValue(dt.Rows[i]["unit"].ToString());
						cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

						cells["C"+cellRow].PutValue(Int64.Parse(dt.Rows[i]["CONNECTION_NUMBER"].ToString()));
						cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;																			
						cellRow++;
					
					}			
					#endregion

				
					//Ajustement de la taile des cellules en fonction du contenu
					sheet.AutoFitColumn(1);	
					sheet.AutoFitColumn(2);
					sheet.AutoFitColumn(3);
				}
			}catch(Exception err){
				throw (new  BastetExceptions.UnitUIException(" TopUsed : Impossible d'obtenir la liste des unités les plus utilisés ", err));
			}
			return excel;
		}
	}
}
