#region Information
 // Author : Y. R'kaina
 // Creation : 05/02/2007
 // Modifications :
#endregion

using System;
using System.IO;
using Aspose.Cells;
using System.Drawing;
using System.Data;

using TNS.AdExpress.Anubis.Amset;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using AmsetExceptions=TNS.AdExpress.Anubis.Amset.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using AmsetFunctions=TNS.AdExpress.Anubis.Amset.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Amset.UI{
	/// <summary>
	/// Document Excel résultats Amset
	/// </summary>
	public class AmsetExcel{
		
		#region Variables
		/// <summary>
		/// Composant excel
		/// </summary>
		protected Workbook _excel;
		/// <summary>
		/// Licence Aspose Excel
		/// </summary>
		License _license;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public AmsetExcel(){

			_excel = new Workbook();
			_license = new License();
			_license.SetLicense("Aspose.Cells.lic");
			
			//Ajout de couleurs			
			AddColor(Color.FromArgb(128,128,192));
		}
		#endregion

		#region Destructeur
		/// <summary>
		/// Destructeur
		/// </summary>
		~AmsetExcel(){
			_excel=null;
			_license=null;
		}
		#endregion

		#region Page principale
		/// <summary>
		/// Page pricipale
		/// </summary>
		/// <param name="webSession">Session du client</param>
		protected void MainPageDesign(WebSession webSession){

			int cellRow =9;
			int startIndex=cellRow;			
			
			try{
				//Adding Orchid color to the palette 
				_excel.ChangePalette(Color.FromArgb(177,163,193),55);
				_excel.ChangePalette(Color.FromArgb(208,200,218),54);
				_excel.ChangePalette(Color.FromArgb(100,72,131),53);
				_excel.ChangePalette(Color.FromArgb(177,163,193),52);
				_excel.ChangePalette(Color.FromArgb(233,230,239),51);
				_excel.ChangePalette(Color.FromArgb(107,89,139),50);
				_excel.ChangePalette(Color.FromArgb(162,125,203),49);
				_excel.ChangePalette(Color.FromArgb(225,224,218),48);
				_excel.ChangePalette(Color.FromArgb(222,216,229),47);

				//Obtaining the reference of the newly added worksheet by passing its sheet index
				Worksheet sheet = this._excel.Worksheets[0];

				Cells cells = sheet.Cells;
				AmsetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2071, webSession.SiteLanguage), 15, webSession.SiteLanguage);

				//intitulé du fichier excel			
				cells["E"+cellRow].PutValue(GestionWeb.GetWebWord(2071,webSession.SiteLanguage));
				cells["E"+cellRow].Style.Font.Size =40;
				cells["E"+cellRow].Style.Font.Color = Color.FromArgb(100,72,131);
				cells["E"+cellRow].Style.Font.IsBold = true;		
				AmsetFunctions.WorkSheet.CellsStyle(cells,null,4,4,8,true,Color.FromArgb(100,72,131),Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				sheet.AutoFitRow(cellRow-1);
				cellRow++;

				//Date de création
				cells["H"+cellRow].PutValue(GestionWeb.GetWebWord(1922,webSession.SiteLanguage)+"  "+WebFunctions.Dates.DateToString(DateTime.Now,webSession.SiteLanguage));
				cells["H"+cellRow].Style.Font.Size =12;
				cells["H"+cellRow].Style.Font.Color =  Color.FromArgb(100,72,131);
				cells["H"+cellRow].Style.Font.IsBold = true;
				cells["H"+cellRow].Style.HorizontalAlignment = TextAlignmentType.Left;
				sheet.AutoFitRow(cellRow-1);
				cellRow+=6;	
		
				// Ajout de l'image dans la cellule
				Pictures pics = sheet.Pictures;
				string adexpressLogoPath=@"Images\" + webSession.SiteLanguage + @"\LogoAdExpress.jpg";
				int pos = sheet.Pictures.Add(cellRow,2,adexpressLogoPath,70,80);
				Picture pic = sheet.Pictures[pos];
				pic.Placement = Aspose.Cells.PlacementType.Move;
			}
			catch(Exception e){
				throw(new  AmsetExceptions.AmsetExcelSystemException("Unable to build the main page",e));
			}
		}
		#endregion

		#region Sauvegarde du fichier
		/// <summary>
		/// Sauvegarde du fichier excel
		/// </summary>
		/// <param name="excelpath"></param>
		protected void Save(string excelpath){
			try{
				_excel.Save(excelpath);
			}
			catch(Exception e){
				throw(new AmsetExceptions.AmsetExcelSystemException("Unable to save the excel file",e));
			}
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Code couleur
		/// </summary>
		private int    m_indexCouleur=55;
  
		/// <summary>
		/// Création éventuel de la couleur
		/// </summary>
		/// <param name="couleur">Couleur existante ? si non créé la</param>
		private void AddColor(Color couleur){
			// Création de la couleur
			if(!_excel.IsColorInPalette(couleur)){
				if (m_indexCouleur>=0) 
					_excel.ChangePalette(couleur, m_indexCouleur--);
			}
		}
		#endregion

	}
}
