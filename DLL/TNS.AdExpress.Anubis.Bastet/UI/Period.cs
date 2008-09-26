#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 28/11/2005
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
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using AnubisBastet=TNS.AdExpress.Anubis.Bastet;



namespace TNS.AdExpress.Anubis.Bastet.UI
{
	/// <summary>
	/// Obtient les donn�es des p�riodes les plus utilis�s
	/// </summary>
	public class Period
	{
		
		/// <summary>
		/// Top des p�riodes les plus utilis�s dans AdExpress 
		/// </summary>		
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>				
		/// <returns>objet excel</returns>
		internal static Excel TopUsed(Excel excel,BastetCommon.Parameters parameters) {
			try{
				//Chargement des donn�es des r�sultats
				DataTable dt = DataAccess.Period.TopUsed(parameters);
		
				if(dt!=null && dt.Rows.Count>0){

					//Variables				
					int cellRow =5;			
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					sheet.Name="Top p�riodes utilis�es"; // A mettre dans web word
				
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


					#region Top p�riodes utilis�	
					//Top Groupe de module utilis�				
					cells["B"+cellRow].PutValue(" Top p�riodes utilis�es ");// A mettre dans web word
					cells["B"+cellRow].Style.Font.IsBold = true;
					cells["B"+cellRow].Style.Font.Color = Color.White;
					cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

					cells["C"+cellRow].PutValue(" Nombre d'utilisation ");// A mettre dans web word
					cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
					cells["C"+cellRow].Style.Font.IsBold = true;
					cells["C"+cellRow].Style.Font.Color = Color.White;
					cells["C"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

					sheet.AutoFitColumn(1);
					cellRow++;

					//top des �l�ments				
					for(int i=0; i<dt.Rows.Count;i++){						
						cells["B"+cellRow].PutValue(dt.Rows[i]["PERIODE"].ToString());
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
				throw (new  BastetExceptions.PeriodUIException(" TopUsed : Impossible d'obtenir la liste des p�riodes les plus utilis�s ", err));
			}
			return excel;
		}

		

		
	}
}