#region Informations
// Auteur: D. V. Mussuma
// Date de création: 25/11/2005
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
	/// Top utilisateurs du fichier GAD et AGM
	/// </summary>
	public class File
	{
		/// <summary>
		/// Top utilisateurs des fichiers GAD et AGM
		/// </summary>
		///<param name="parameters">paramètres des statistiques</param>
		///<param name="excel">fichier excel</param>				
		/// <returns>objet excel</returns>
		internal static Excel TopUsers(Excel excel,BastetCommon.Parameters parameters) {
			try{
				//Chargement des données du top utilisation fichier GAD
				DataTable dtGAD = DataAccess.File.TopGAD(parameters);

				//Chargement des données du top utilisation fichier AGM
				DataTable dtAGM = DataAccess.File.TopAGM(parameters);

				if((dtGAD!=null && dtGAD.Rows.Count>0) || (dtAGM!=null && dtAGM.Rows.Count>0)){
					//Variables
					int cellRow =5;
					bool isGadExist=false;			
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					sheet.Name="Top fichier GAD et AGM"; // A mettre dans web word	
					sheet.PageSetup.Orientation = PageOrientationType.Landscape;
				
					//Saut de page horizontal
					int nbPages=0;
					const int nbMaxRowByPage=40;
					int nbRows=0;
					if(dtGAD!=null && dtGAD.Rows.Count>0)nbRows=dtGAD.Rows.Count;
					if(dtAGM!=null && dtAGM.Rows.Count>0)nbRows+=dtAGM.Rows.Count;
					nbRows+=6;

					nbPages=(int)Math.Ceiling(nbRows*1.0/nbMaxRowByPage);
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

					int startIndex=cellRow;

					if(parameters!=null && parameters.Logins.Length==0){			
						//Tous les logins
						#region Top utilisateurs du fichier GAD
						if(dtGAD!=null && dtGAD.Rows.Count>0){
							//Top utilisateurs du fichier GAD				
							cells["B"+cellRow].PutValue("Top utilisateurs du fichier GAD ");// A mettre dans web word
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

							cells["D"+cellRow].PutValue(" Nombre d'utilisation ");// A mettre dans web word
							cells["D"+cellRow].Style.Font.IsBold = true;
							cells["D"+cellRow].Style.Font.Color = Color.White;
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
						
							cellRow++;

							for(int i=0; i<dtGAD.Rows.Count;i++){	
								//société
								cells["B"+cellRow].PutValue(dtGAD.Rows[i]["COMPANY"].ToString());
								cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

								//login
								cells["C"+cellRow].PutValue(dtGAD.Rows[i]["LOGIN"].ToString());
								cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
								cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
								cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

								//nombre d'utilisation
								cells["D"+cellRow].PutValue(Int64.Parse(dtGAD.Rows[i]["CONNECTION_NUMBER"].ToString()));
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

							cells["D"+cellRow].PutValue(Int64.Parse(dtGAD.Compute("sum(CONNECTION_NUMBER)","").ToString()));
							cells["D"+cellRow].Style.Font.Color = Color.Red;
							cells["D"+cellRow].Style.Font.IsBold = true;
							cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
							cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	


							isGadExist = true;
						}
						#endregion

						#region Top utilisateurs du fichier AGM
						if(dtAGM!=null && dtAGM.Rows.Count>0){
							if(isGadExist)cellRow=cellRow+5;
							//Top module utilisé				
							cells["B"+cellRow].PutValue("Top utilisateurs du fichier Agences média ");// A metre dans web word
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
					
							cells["D"+cellRow].PutValue(" Nombre d'utilisation ");// A mettre dans web word
							cells["D"+cellRow].Style.Font.IsBold = true;
							cells["D"+cellRow].Style.Font.Color = Color.White;
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

							cellRow++;	
							int startRowAGM=cellRow;
							//Pour les 10 1ers éléments
							for(int i=0; i<dtAGM.Rows.Count;i++){																
								cells["B"+cellRow].PutValue(dtAGM.Rows[i]["COMPANY"].ToString());// A mettre dans web word
								cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
								cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

								cells["C"+cellRow].PutValue(dtAGM.Rows[i]["LOGIN"].ToString());
								cells["C"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
								cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
								cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;

								cells["D"+cellRow].PutValue(Int64.Parse(dtAGM.Rows[i]["CONNECTION_NUMBER"].ToString()));
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

							cells["D"+cellRow].PutValue(Int64.Parse(dtAGM.Compute("sum(CONNECTION_NUMBER)","").ToString()));
							cells["D"+cellRow].Style.Font.Color = Color.Red;
							cells["D"+cellRow].Style.Font.IsBold = true;
							cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;							
							cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
						}
						#endregion
					}else{
						//Pour les logins sélectionés

						//Top utilisateurs du fichier GAD
						if(dtGAD!=null && dtGAD.Rows.Count>0){
							cells["B"+cellRow].PutValue("Nombre d'utilisation du fichier GAD");// A mettre dans web word
							cells["B"+cellRow].Style.Font.IsBold = true;
							cells["B"+cellRow].Style.Font.Color = Color.White;
							cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

							cells["C"+cellRow].PutValue(Int64.Parse(dtGAD.Compute("sum(CONNECTION_NUMBER)","").ToString()));
							cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
							cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;	
						
						}

						//Top utilisateurs du fichier AGM
						if(dtAGM!=null && dtAGM.Rows.Count>0){
							cellRow=cellRow+2;

							cells["B"+cellRow].PutValue("Nombre d'utilisation du fichier Agences Médias");// A mettre dans web word
							cells["B"+cellRow].Style.Font.IsBold = true;
							cells["B"+cellRow].Style.Font.Color = Color.White;
							cells["B"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["B"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;	
							cells["B"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);

							cells["C"+cellRow].PutValue(Int64.Parse(dtAGM.Compute("sum(CONNECTION_NUMBER)","").ToString()));
							cells["C"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
							cells["C"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
							cells["C"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;	
						}
					}

					//Ajustement de la taile des cellules en fonction du contenu	
					sheet.AutoFitColumn(1);
					sheet.AutoFitColumn(2);
					sheet.AutoFitColumn(3);

				}
			}catch(Exception err){
				throw (new BastetExceptions.FileUIException(" TopUsers : Impossible d'obtenir la liste des clients utilisant le plus les fichiers GAD et AGM ", err));
			}
			return excel;

		}
	}
}
