#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 07/12/2005
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
using DateFrameWork=TNS.FrameWork.Date;

namespace TNS.AdExpress.Anubis.Bastet.UI
{
	/// <summary>
	/// Description r�sum�e de Duration.
	/// </summary>
	public class Duration
	{
		/// <summary>
		/// Dur�e moyenne des connections par clients
		/// </summary>
		///<param name="parameters">param�tres des statistiques</param>
		///<param name="excel">fichier excel</param>				
		/// <returns>objet excel</returns>
		internal static Excel ConnexionAverage(Excel excel,BastetCommon.Parameters parameters) {
			try{
				//Chargement des donn�es 
				DataTable dt = DataAccess.Duration.ConnexionAverage(parameters);
				Int64 totalDuration = 0;
				
				if(dt!=null && dt.Rows.Count>0 && dt.Rows[0][0]!=System.DBNull.Value){
					totalDuration = Int64.Parse(dt.Compute("sum(CONNEXION_AVERAGE)","").ToString());
					if(totalDuration>0 ){
						//Variables
						int cellRow =5;
						Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
						sheet.Name="Dur�e moyenne des connections"; // A mettre dans web word	
						sheet.PageSetup.Orientation = PageOrientationType.Landscape;
					
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
						pics.Add(0, 6,bastetImagePath);

						//Set current date and current time at the center section of header and change the font of the header
						pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
						//Set current page number at the center section of footer
						pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");

						int startIndex=cellRow;

						if(parameters!=null && parameters.Logins.Length==0){			
							//Tous les logins

							//Dur�e moyenne des connections				
							cells["B"+cellRow].PutValue(" Dur�e moyenne des connections ");// A mettre dans web word
							cells["B"+cellRow].Style.Font.IsBold = true;
							cells["B"+cellRow].Style.Font.Color = Color.White;
							cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
						
							cells["C"+cellRow].PutValue(" Logins ");// A mettre dans web word
							cells["C"+cellRow].Style.Font.IsBold = true;
							cells["C"+cellRow].Style.Font.Color = Color.White;
							cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["C"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

							cells["D"+cellRow].PutValue(" Dur�e ");// A mettre dans web word
							cells["D"+cellRow].Style.Font.IsBold = true;
							cells["D"+cellRow].Style.Font.Color = Color.White;
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
						
							cellRow++;

							for(int i=0; i<dt.Rows.Count;i++){	
								//soci�t�
								cells["B"+cellRow].PutValue(dt.Rows[i]["COMPANY"].ToString());
								cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

								//login
								cells["C"+cellRow].PutValue(dt.Rows[i]["LOGIN"].ToString());
								cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
								cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
								cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

								//nombre d'utilisation
								cells["D"+cellRow].PutValue(DateFrameWork.DateString.SecondToHH_MM_SS(Int64.Parse(dt.Rows[i]["CONNEXION_AVERAGE"].ToString())));
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

							cells["D"+cellRow].PutValue(DateFrameWork.DateString.SecondToHH_MM_SS(totalDuration));
							cells["D"+cellRow].Style.Font.Color = Color.Red;
							cells["D"+cellRow].Style.Font.IsBold = true;
							cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
							cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	


						}else{
							//Pour les logins s�lection�s

							//Dur�e moyenne de connections
					
							cells["B"+cellRow].PutValue(" Dur�e moyenne des connections ");// A mettre dans web word
							cells["B"+cellRow].Style.Font.IsBold = true;
							cells["B"+cellRow].Style.Font.Color = Color.White;
							cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
							if(dt.Rows[0][0]!=null)
							cells["C"+cellRow].PutValue(DateFrameWork.DateString.SecondToHH_MM_SS(long.Parse(dt.Rows[0][0].ToString())));
							else cells["C"+cellRow].PutValue(DateFrameWork.DateString.SecondToHH_MM_SS(0));
							
							cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
							cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;												

					
						}
				

						//Ajustement de la taile des cellules en fonction du contenu	
						sheet.AutoFitColumn(1);
						sheet.AutoFitColumn(2);
						sheet.AutoFitColumn(3);
					}

				}
			}catch(System.Exception err){
				throw (new BastetExceptions.DurationUIException(" ConnexionAverage : Impossible d'afficher Dur�e moyenne des connections par clients ", err));
			}
			return excel;

		}
	}
}