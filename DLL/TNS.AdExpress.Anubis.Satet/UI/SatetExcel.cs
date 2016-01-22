///////////////////////////////////////////////////////////
//  SatetExcel.cs
//  Implementation of the Class Excel
//  Created on:      29-May.-2006 14:51:12
//  Original author: D.V. Mussuma
///////////////////////////////////////////////////////////


using System;
using System.IO;
using Aspose.Cells;
using System.Drawing;
using System.Data;

using TNS.AdExpress.Anubis.Satet;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using SatetExceptions=TNS.AdExpress.Anubis.Satet.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using SatetFunctions=TNS.AdExpress.Anubis.Satet.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Anubis.Satet.Exceptions;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Document Excel r�sultats APPM
	/// </summary>
	public class SatetExcel {

        #region variables
        /// <summary>
		/// Composant excel
		/// </summary>
        protected Workbook _excel;
        /// <summary>
        /// Composant excel
        /// </summary>
        protected TNS.FrameWork.WebTheme.Style _style;
		/// <summary>
		/// Licence Aspose Excel
		/// </summary>
		License _license;
        #endregion

        #region Constructeur
        /// <summary>
		/// Constructeur
		/// </summary>
        public SatetExcel(TNS.FrameWork.WebTheme.Style style) {
            try {
                _excel = new Workbook();
                _license = new License();
                _license.SetLicense("Aspose.Cells.lic");
                _style = style;

                //initialisation de la palette de couleur Excel
                _style.InitExcelColorPalette(this._excel);
            }
            catch (Exception e) {
                throw new SatetDataAccessException("Error in constructor SatetExcel", e);
            }
		}
		#endregion

		#region Destructeur
		/// <summary>
		/// Destructeur
		/// </summary>
		~SatetExcel(){
			_excel=null;
			_license=null;
		}
		#endregion

		#region Page principale
		/// <summary>
		/// Page pricipale
		/// </summary>
		/// <param name="webSession">Session du client</param>
        protected void MainPageDesign(WebSession webSession, TNS.FrameWork.WebTheme.Style style) {
			int cellRow =9;
			int startIndex=cellRow;

			//Obtaining the reference of the newly added worksheet by passing its sheet index
			Worksheet sheet = this._excel.Worksheets[0];

			Cells cells = sheet.Cells;	

            //Mise en page
            SatetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1587, webSession.SiteLanguage), 15, style);

			//intitul� du fichier excel			
            SatetFunctions.WorkSheet.CellsStyle(_excel, cells, style.GetTag("bigTitle"), GestionWeb.GetWebWord(1587, webSession.SiteLanguage), cellRow - 1, 5, 5, false);

            //Police par default
            SatetFunctions.WorkSheet.CellsStyle(_excel, cells, style.GetTag("defaultTitle"), null, 4, 4, 8, false);
			sheet.AutoFitRow(cellRow-1);
			cellRow++;

			//Date de cr�ation
            SatetFunctions.WorkSheet.CellsStyle(_excel, cells, style.GetTag("createdTitle"), GestionWeb.GetWebWord(1922, webSession.SiteLanguage) + "  " + Dates.DateToString(DateTime.Now, webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern), cellRow - 1, 7, 7, true);
			sheet.AutoFitRow(cellRow-1);
			cellRow+=6;	
		
			// Ajout de l'image dans la cellule
            style.GetTag("LogoAdExpress").SetStyleExcel(sheet, cellRow, 2);			 			
		}
		#endregion

		#region Sauvegarde du fichier
		/// <summary>
		/// Sauvegarde du fichier excel
		/// </summary>
		/// <param name="excelpath"></param>
		protected void Save(string excelpath)
		{
			_excel.Save(excelpath);
		}
		#endregion

	}
}
