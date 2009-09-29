#region Informations
// Auteur: D. V. Mussuma
// Date de création: 5/12/2005
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
using BastetExceptions=TNS.AdExpress.Anubis.Bastet.Exceptions;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using AnubisBastet=TNS.AdExpress.Anubis.Bastet;
using BastetFunctions=TNS.AdExpress.Anubis.Bastet.Functions;
using BastetRules=TNS.AdExpress.Anubis.Bastet.Rules;
using TNS.AdExpress.Domain.Translation;
using System.IO;

namespace TNS.AdExpress.Anubis.Bastet.UI
{
	/// <summary>
	/// Description résumée de Vehicle.
	/// </summary>
	public class Vehicle
	{
		#region Top des médias les plus utilisés 
		/// <summary>
		/// Top des médias les plus utilisés dans AdExpress 
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>				
		/// <returns>objet excel</returns>
		internal static Workbook TopUsed(Workbook excel,BastetCommon.Parameters parameters,int language) {
			try{
				//Chargement des données des résultats
				DataTable dt = DataAccess.Vehicle.TopUsed(parameters,language);
		
				if(dt!=null && dt.Rows.Count>0){

					//Variables				
					int cellRow =5;			
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					sheet.Name =  GestionWeb.GetWebWord(2529, language) ; // A mettre dans web word	
			
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
					Aspose.Cells.PageSetup pageSetup =sheet.PageSetup;
				
					//Set margins, in unit of inches 					
					pageSetup.TopMarginInch = 0.3; 
					pageSetup.BottomMarginInch = 0.3; 
					pageSetup.HeaderMarginInch = 0.3; 
					pageSetup.FooterMarginInch = 0.3; 
				
					//Ajout du logo TNS
					Pictures pics = sheet.Pictures;
					string tnsLogoPath = TNS.AdExpress.Anubis.Bastet.Constantes.Images.LOGO_TNS;
					string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tnsLogoPath);
					int picIndex = pics.Add(0, 0,logoPath);
					pics[picIndex].Placement = Aspose.Cells.PlacementType.Move;

					string bastetLogoPath = TNS.AdExpress.Anubis.Bastet.Constantes.Images.LOGO_BASTET;
					string bastetImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bastetLogoPath);
					picIndex = pics.Add(0, 6, bastetImagePath);
					pics[picIndex].Placement = Aspose.Cells.PlacementType.Move;

					//Set current date and current time at the center section of header and change the font of the header
					pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
					//Set current page number at the center section of footer
					pageSetup.SetFooter(1, "&A" + " - " + GestionWeb.GetWebWord(894, language) + " " + "&P" + " " + GestionWeb.GetWebWord(2042, language) + " " + "&N");


					#region Top périodes utilisé	
					//Top Groupe de module utilisé				
					cells["B" + cellRow].PutValue(" " + GestionWeb.GetWebWord(2529, language) + " ");
					cells["B"+cellRow].Style.Font.IsBold = true;
					cells["B"+cellRow].Style.Font.Color = Color.White;
					cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
					cells["B" + cellRow].Style.Pattern = BackgroundType.Solid;

					cells["C" + cellRow].PutValue(" " + GestionWeb.GetWebWord(2515, language) + " ");
					cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Font.IsBold = true;
					cells["C"+cellRow].Style.Font.Color = Color.White;
					cells["C"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
					cells["C" + cellRow].Style.Pattern = BackgroundType.Solid;

					sheet.AutoFitColumn(1);
					cellRow++;

					//top des éléments				
					for(int i=0; i<dt.Rows.Count;i++){						
						cells["B"+cellRow].PutValue(dt.Rows[i]["vehicle"].ToString());
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
				}
			}catch(Exception err){
				throw (new  BastetExceptions.VehicleUIException(" TopUsed : Impossible d'obtenir la liste des médias les plus utilisés ", err));
			}
			return excel;
		}
		#endregion

		#region Top médias par module
		/// <summary>
		/// Top connections médias par module
		/// </summary>		
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>
		internal static Workbook TopByModule(Workbook excel,BastetCommon.Parameters parameters,int language){
			try{
								
				//Chargement des données
				DataTable dt=null;				
				dt = BastetRules.Vehicle.TopByModule(parameters,language);
													
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
					int minimunVPageBreaks=10;
					string vPageBreaks="";
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;	
									
					BastetFunctions.WorkSheet.SetTopMediaByModule(dt,sheet,cells,ref cellRow,ref i,ref j,ref category,ref serial,language);
					#region Graphique

					int lastRow=cellRow;

					//Saut de page pour graphique
					cellRow = cellRow + (nbMaxRowByPage - (cellRow % nbMaxRowByPage)) + 2;

					//Ajout un graphique à la feuille excel				
					int chartIndex=sheet.Charts.Add(ChartType.LineStackedWithDataMarkers,cellRow,0,cellRow+30,8);

					//Accede à l'instance d'un nouveau graphique
					Chart chart=sheet.Charts[chartIndex];
					
					//Construit le graphique
					BastetFunctions.BastetChart.Build(chart, serial, category, GestionWeb.GetWebWord(2486, language), GestionWeb.GetWebWord(2531, language), GestionWeb.GetWebWord(1292, language), GestionWeb.GetWebWord(2489, language));
					#endregion
					
					if(i>=minimunVPageBreaks)vPageBreaks=cells[(cellRow-1+30),(i)].Name;
					else vPageBreaks=cells[(cellRow-1+30),(minimunVPageBreaks)].Name;
					BastetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2530, language), dt, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks,language);
					#endregion								
				}
				
			}catch(System.Exception e){
				throw new BastetExceptions.ClientUIException(" Impossible d'insérer des données dans le fichier excel.",e);
			}
			return excel;

		}
		#endregion

	}
}
