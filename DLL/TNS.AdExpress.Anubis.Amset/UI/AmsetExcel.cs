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
using TNS.AdExpress.Web.Functions;

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
        /// <summary>
        /// Style
        /// </summary>
        protected TNS.AdExpress.Domain.Theme.Style _style;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public AmsetExcel(TNS.AdExpress.Domain.Theme.Style style) {

			_excel = new Workbook();
			_license = new License();
			_license.SetLicense("Aspose.Cells.lic");
            _style = style;

            //initialisation de la palette de couleur Excel
            _style.InitExcelColorPalette(this._excel);
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
        protected void MainPageDesign(WebSession webSession, TNS.AdExpress.Domain.Theme.Style style) {

			int cellRow =9;
			int startIndex=cellRow;			
			
			try{
				//Obtaining the reference of the newly added worksheet by passing its sheet index
				Worksheet sheet = this._excel.Worksheets[0];

				Cells cells = sheet.Cells;
				AmsetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(2071, webSession.SiteLanguage), 15, webSession.SiteLanguage,_style);

                //intitulé du fichier excel			
                AmsetFunctions.WorkSheet.CellsStyle(_excel, cells, style.GetTag("bigTitle"), GestionWeb.GetWebWord(2071, webSession.SiteLanguage), cellRow - 1, 4, 4, false);

                //Police par default
                AmsetFunctions.WorkSheet.CellsStyle(_excel, cells, style.GetTag("defaultTitle"), null, 4, 4, 8, false);
				sheet.AutoFitRow(cellRow-1);
				cellRow++;

				//Date de création
                AmsetFunctions.WorkSheet.CellsStyle(_excel, cells, style.GetTag("createdTitle"), GestionWeb.GetWebWord(1922, webSession.SiteLanguage) + "  " + Dates.DateToString(DateTime.Now, webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern), cellRow - 1, 7, 7, true);
				sheet.AutoFitRow(cellRow-1);
				cellRow+=6;	
		
				// Ajout de l'image dans la cellule
                style.GetTag("LogoAdExpress").SetStyleExcel(sheet, cellRow, 2);
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

	}
}
