using System;
using System.Data;
using Aspose.Excel;
using System.Drawing;
using TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.MediaSchedule.Functions {
    /// <summary>
    /// Description résumée de WorkSheet.
    /// </summary>
    public static class WorkSheet {
        #region Mise en page
        /// <summary>
        /// Mise en page de la feuille excel.
        /// </summary>
        /// <param name="sheet">Feuille excel.</param>
        /// <param name="name">nom de la feuille excel</param>
        /// <param name="nbRows">Nombre de lignes</param>
        /// <param name="nbMaxRowByPage">nombre de ligne maximu par page</param>
        /// <param name="s">compteur de page</param>
        /// <param name="printArea">Zone d'impression</param>
        /// <param name="upperLeftColumn">colone la plus à gauche</param>
        /// <param name="vPageBreaks">saut de page vertical</param>
        public static void PageSettings(Aspose.Excel.Worksheet sheet, string name, int nbRows, int nbMaxRowByPage, ref int s, int upperLeftColumn, string vPageBreaks, string headerRowIndex) {

            int nbPages = 0;
            int currentNbRowByPage = nbMaxRowByPage;
            int first = 0;
            nbPages = (int)Math.Ceiling(nbRows * 1.0 / nbMaxRowByPage);
            sheet.Name = name; // A mettre dans web word		
            for (s = 1; s < nbPages; s++) {
                if (first == 0) {
                    sheet.HPageBreaks.Add(nbMaxRowByPage * s, 0, 6);
                    first = 1;
                }
                else {
                    currentNbRowByPage += 38;
                    sheet.HPageBreaks.Add(currentNbRowByPage, 0, 6);
                }
            }

            if (vPageBreaks != null && vPageBreaks.Length > 0)
                sheet.VPageBreaks.Add(vPageBreaks);

            sheet.IsGridlinesVisible = false;
            sheet.PageSetup.Orientation = PageOrientationType.Landscape;
            Aspose.Excel.PageSetup pageSetup = sheet.PageSetup;

            //Set margins, in unit of inches 					
            pageSetup.TopMarginInch = 0.3;
            pageSetup.BottomMarginInch = 0.3;
            pageSetup.HeaderMarginInch = 0.3;
            pageSetup.FooterMarginInch = 0.3;
            pageSetup.RightMargin = 0.3;
            pageSetup.LeftMargin = 0.3;
            pageSetup.CenterHorizontally = true;
            if (headerRowIndex != null) {
                pageSetup.PrintTitleRows = "$" + headerRowIndex + ":$4";//+headerRowIndex;
            }

            //Ajout des logos TNS et Appm
            Pictures pics = sheet.Pictures;
            string tnsLogoPath = @"Images\logoTNSMedia.gif";
            string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
            pics.Add(0, 0, logoPath, 75, 75);
            string appmLogoPath = @"Images\Common\APPM.bmp";
            string bastetImagePath = System.IO.Path.GetFullPath(appmLogoPath);
            pics.Add(0, upperLeftColumn, bastetImagePath, 55, 45);

            //Set current date and current time at the center section of header and change the font of the header
            pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");

            //Set current page number at the center section of footer
            pageSetup.SetFooter(1, "&A" + " - Page " + "&P" + " sur " + "&N");
        }

        /// <summary>
        /// Mise en page de la feuille excel.
        /// </summary>
        /// <param name="sheet">Feuille excel.</param>
        /// <param name="name">nom de la feuille excel</param>
        /// <param name="s">compteur de page</param>
        /// <param name="upperLeftColumn">colone la plus à gauche</param>
        /// <param name="headerRowIndex">En-tête</param>
        public static void PageSettings(Aspose.Excel.Worksheet sheet, string name, ref int s, int upperLeftColumn, string headerRowIndex) {

            sheet.Name = name; // A mettre dans web word		

            sheet.IsGridlinesVisible = false;
            sheet.PageSetup.Orientation = PageOrientationType.Landscape;
            Aspose.Excel.PageSetup pageSetup = sheet.PageSetup;

            //Set margins, in unit of inches 					
            pageSetup.TopMarginInch = 0.3;
            pageSetup.BottomMarginInch = 0.3;
            pageSetup.HeaderMarginInch = 0.3;
            pageSetup.FooterMarginInch = 0.3;
            pageSetup.RightMargin = 0.3;
            pageSetup.LeftMargin = 0.3;
            pageSetup.CenterHorizontally = true;
            pageSetup.FitToPagesTall = 32000;
            pageSetup.FitToPagesWide = 1;
            if (headerRowIndex != null) {
                pageSetup.PrintTitleRows = "$" + headerRowIndex + ":$4";//+headerRowIndex;
            }

            //Ajout des logos TNS et Appm
            Pictures pics = sheet.Pictures;
            string tnsLogoPath = @"Images\logoTNSMedia.gif";
            string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
            pics.Add(0, 0, logoPath, 75, 75);
            string appmLogoPath = @"Images\Common\APPM.bmp";
            string bastetImagePath = System.IO.Path.GetFullPath(appmLogoPath);
            pics.Add(0, upperLeftColumn, bastetImagePath, 55, 45);

            //Set current date and current time at the center section of header and change the font of the header
            pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");

            //Set current page number at the center section of footer
            pageSetup.SetFooter(1, "&A" + " - Page " + "&P" + " sur " + "&N");
        }
        #endregion

        #region Insertion d'une donnée dans une cellule
        /// <summary>
        /// Insert une donnée dans une cellule
        /// </summary>
        /// <param name="cells">Cellules</param>
        /// <param name="data">donnée</param>
        /// <param name="row">ligne</param>
        /// <param name="column">colonne</param>
        /// <param name="isBold">vrai si police en gras</param>
        /// <param name="color">couleur de la police</param>
        /// <param name="size">Taille du texte</param>
        public static void PutCellValue(Aspose.Excel.Cells cells, object data, int row, int column, int startColumn, Aspose.Excel.Style style, string format) {
            if (format == null || format.Length < 1)
                format = "General";

            cells[row, column].PutValue(data);
            cells[row, column].Style = style;
            cells[row, column].Style.Custom = format;

            if (column >= startColumn && format == "General")
                cells[row, column].Style.HorizontalAlignment = TextAlignmentType.Center;
        }
        #endregion
    }
}
