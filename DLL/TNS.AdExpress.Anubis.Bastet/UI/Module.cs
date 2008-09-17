#region Informations
// Auteur: D. V. Mussuma
// Date de création: 24/11/2005
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
	/// Top des modules et groupes de modules utilisés
	/// </summary>
	public class Module
	{
		/// <summary>
		/// Top des modules et groupes de modules utilisés
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>				
		/// <returns>objet excel</returns>
		internal static Excel TopUsed(Excel excel,BastetCommon.Parameters parameters)
		{
			try{
			
				//Chargement des données des modules
				DataTable dtModule = DataAccess.Module.TopUsed(parameters,false);

				//Chargement des données des groupes modules
				DataTable dtModuleGroup = DataAccess.Module.TopUsed(parameters,true);

				if(dtModule!=null && dtModule.Rows.Count>0){
					//Variables				
					int cellRow =5;	
					int startIndex=cellRow;
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					sheet.Name="Top modules"; // A mettre dans web word

					//Saut de page horizontal
					int nbPages=0;
					const int nbMaxRowByPage=40;
					nbPages=(int)Math.Ceiling(dtModule.Rows.Count*1.0/nbMaxRowByPage);
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

					#region Top Groupe de module utilisé	
					//Top Groupe de module utilisé				
					cells["B"+cellRow].PutValue("Top Groupes de modules utilisés");// A mettre dans web word
					cells["B"+cellRow].Style.Font.IsBold = true;
					cells["B"+cellRow].Style.Font.Color = Color.White;
					cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

					cells["C"+cellRow].PutValue(" Nombre d'accès ");// A mettre dans web word
					cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Font.IsBold = true;
					cells["C"+cellRow].Style.Font.Color = Color.White;
					cells["C"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
					cellRow++;

					//Pour chaque éléments
					for(int i=0; i<dtModuleGroup.Rows.Count;i++){																
						cells["B"+cellRow].PutValue(dtModuleGroup.Rows[i]["MODULE_GROUP"].ToString());// A mettre dans web word
						cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].PutValue(Int64.Parse(dtModuleGroup.Rows[i]["CONNECTION_NUMBER"].ToString()));
						cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
						cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;											
						cellRow++;
					}

					#endregion

					#region Top modules utilisés
	
					cellRow=startIndex;
					//Top module utilisé				
					cells["E"+cellRow].PutValue("Top modules utilisés");// A metre dans web word
					cells["E"+cellRow].Style.Font.IsBold = true;
					cells["E"+cellRow].Style.Font.Color = Color.White;
					cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["E"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["E"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["E"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["E"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

					cells["F"+cellRow].PutValue(" Nombre d'accès ");// A mettre dans web word
					cells["F"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["F"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["F"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["F"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["F"+cellRow].Style.Font.IsBold = true;
					cells["F"+cellRow].Style.Font.Color = Color.White;
					cells["F"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

					cellRow++;	

					int startRowmodule=cellRow;
					//Pour les  1ers éléments
					for(int i=0; i<dtModule.Rows.Count;i++){																
						cells["E"+cellRow].PutValue(dtModule.Rows[i]["MODULE"].ToString());
						cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["E"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells["E"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
						cells["F"+cellRow].PutValue(int.Parse(dtModule.Rows[i]["CONNECTION_NUMBER"].ToString()));
						cells["F"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						cells["F"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
						cells["F"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;												
						cellRow++;
					}
							

					#endregion

					//Ajustement de la taile des cellules en fonction du contenu	
					sheet.AutoFitColumn(1);
					sheet.AutoFitColumn(2);
			
					sheet.AutoFitColumn(4);
					sheet.AutoFitColumn(5);
				}
			}catch(Exception err){
				throw (new  BastetExceptions.ModuleUIException(" TopUsed : Impossible d'obtenir la liste des modules les plus utilisés ", err));
			}
			return excel;
		}
	}
}
