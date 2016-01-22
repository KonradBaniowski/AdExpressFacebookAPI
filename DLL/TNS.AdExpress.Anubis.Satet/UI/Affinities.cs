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
using CsteCustomer=TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Satet.UI {
    /// <summary>
    /// Description résumée de Affinities.
    /// </summary>
    public class Affinities {

        #region Variables Theme Name
        private static string _rowTitle = "AffinitiesRowTitle";
        private static string _rowTitleFirstCol = "AffinitiesRowTitleFirstCol";
        private static string _rowBaseTargetSelected = "AffinitiesBaseTargetSelected";
        private static string _rowBaseTargetSelectedFirstCol = "AffinitiesBaseTargetSelectedFirstCol";
        private static string _rowDefault = "AffinitiesRowDefault";
        private static string _rowDefaultFirstCol = "AffinitiesRowDefaultFirstCol";
        #endregion

        #region Affinités
        /// <summary>
        /// Affinités
        /// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style) {

            #region targets
            //base target
            Int64 idBaseTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmBaseTargetAccess));
            //additional target
            Int64 idAdditionalTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmTargetAccess));
            #endregion

            #region Wave
            Int64 idWave = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave, CsteCustomer.Right.type.aepmWaveAccess));
            #endregion

            // Données resultats
            DataTable affinitiesData = TNS.AdExpress.Web.Rules.Results.APPM.AffintiesRules.GetData(webSession, dataSource, int.Parse(webSession.PeriodBeginningDate), int.Parse(webSession.PeriodEndDate), idBaseTarget, idWave);

            if ((affinitiesData != null) && affinitiesData.Rows.Count > 0) {

                #region Variables
                int nbMaxRowByPage = 42;
                int s = 1;
                int cellRow = 5;
                int startIndex = cellRow;
                int header = 1;
                int upperLeftColumn = 12;
                Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
                Cells cells = sheet.Cells;
                object[] oArray;
                Range range = null;
                string vPageBreaks = "";
                double columnWidth = 0, indexLogo = 0, index;
                bool verif = true;
                string excelPatternNameMax0 = "max0";
                string excelPatternNameMax3 = "max3";
                string currentStyle = string.Empty;
                string currentStyleFirstCol = string.Empty;
                #endregion

                #region En-tête du tableau  			
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleFirstCol), GestionWeb.GetWebWord(1708, webSession.SiteLanguage), cellRow - 1, 0, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(1679, webSession.SiteLanguage), cellRow - 1, 1, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(1686, webSession.SiteLanguage), cellRow - 1, 2, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(1685, webSession.SiteLanguage), cellRow - 1, 3, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(1686, webSession.SiteLanguage), cellRow - 1, 4, 1);
                #endregion

                //Insertion des résultats
                cellRow++;

                #region insertion des résultats
                oArray = new object[5];

                foreach (DataRow row in affinitiesData.Rows) {
                    if (Convert.ToInt64(row["id_target"]) == idBaseTarget) {
                        currentStyle = _rowBaseTargetSelected;
                        currentStyleFirstCol = _rowBaseTargetSelectedFirstCol;
                    }
                    else {
                        currentStyle = _rowDefault;
                        currentStyleFirstCol = _rowDefaultFirstCol;
                    }

                    SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyleFirstCol), row["target"], cellRow - 1, 0, 1);
                    SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(row["totalGRP"]), cellRow - 1, 1, 1);
                    SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(row["GRPAffinities"]), cellRow - 1, 2, 1);
                    SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(row["cgrp"]), cellRow - 1, 3, 1);
                    SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(row["cgrpAffinities"]), cellRow - 1, 4, 1);
                    

                    for (int i = 1; i <= 4; i++) {
                        if (i > 0) {
                            cells[cellRow - 1, i].Style.HorizontalAlignment = TextAlignmentType.Right;
                            if (i == 1) 
                                cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
                            else 
                                cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
                        }
                        else {
                            cells[cellRow - 1, i].Style.HorizontalAlignment = TextAlignmentType.Left;
                        }
                    }
                    cellRow++;
                }
                #endregion

                #region Ajustement de la taile des cellules en fonction du contenu
                for (int c = 0; c <= 4; c++) {
                    sheet.AutoFitColumn(c);
                    cells[1, c].Style.HorizontalAlignment = TextAlignmentType.Center;
                }
                #endregion

                #region Mise en page de la feuille excel
                for (index = 0; index < 20; index++) {
                    columnWidth += cells.GetColumnWidth((byte)index);
                    if ((columnWidth < 125) && verif)
                        indexLogo++;
                    else
                        verif = false;
                }

                upperLeftColumn = (int)indexLogo;
                vPageBreaks = cells[cellRow, (int)indexLogo + 1].Name;
                SatetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1687, webSession.SiteLanguage), affinitiesData.Rows.Count + 4, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), style);
                #endregion
            }

        }
        #endregion

    }
}
