using System; 
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Aspose.Cells;

namespace TNS.AdExpressI.MediaSchedule.Style {
    #region Base Class
    /// <summary>
    /// Define styles used in MediaSchedule for Anubis
    /// </summary>
    public abstract class MediaScheduleSheetStyle {

        #region Properties

        #region styles
        public Aspose.Cells.Style VersionCell0;
        public Aspose.Cells.Style CellPresent;
        public Aspose.Cells.Style CellNotPresent;
        public Aspose.Cells.Style CellExtended;
        public Aspose.Cells.Style CellTitle;
        public Aspose.Cells.Style CellYear;
        public Aspose.Cells.Style CellYear1;
        public Aspose.Cells.Style CellPeriod;
        public Aspose.Cells.Style CellPeriodIncomplete;
        public Aspose.Cells.Style CellDay;
        public Aspose.Cells.Style CellDayWE;
        public Aspose.Cells.Style CellLevelTotal;
        public Aspose.Cells.Style CellLevelL1;
        public Aspose.Cells.Style CellLevelL2_1;
        public Aspose.Cells.Style CellLevelL2_2;
        public Aspose.Cells.Style CellLevelL3;
        public Aspose.Cells.Style CellLevelL4;
        public Aspose.Cells.Style CellLevelTotalNb;
        public Aspose.Cells.Style CellLevelTotalPdmNb;
        public Aspose.Cells.Style CellLevelL1Nb;
        public Aspose.Cells.Style CellLevelL1PdmNb;
        public Aspose.Cells.Style CellLevelL2_1Nb;
        public Aspose.Cells.Style CellLevelL2_1PdmNb;
        public Aspose.Cells.Style CellLevelL2_2Nb;
        public Aspose.Cells.Style CellLevelL2_2PdmNb;
        public Aspose.Cells.Style CellLevelL3Nb;
        public Aspose.Cells.Style CellLevelL4Nb;
        public Dictionary<int, Aspose.Cells.Style> CellVersions = new Dictionary<int, Aspose.Cells.Style>();
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MediaScheduleSheetStyle(Workbook excel) {

            int indexColor = 0;

            #region VersionCell0
            indexColor = excel.Styles.Add();
            VersionCell0 = excel.Styles[indexColor];
            VersionCell0.ForegroundColor = Color.FromArgb(100, 72, 131);
            VersionCell0.Pattern = BackgroundType.Solid;
            VersionCell0.Font.Color = Color.White;
            VersionCell0.Font.Size = 8;
            VersionCell0.Font.IsBold = false;
            VersionCell0.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            VersionCell0.Borders[BorderType.RightBorder].Color = Color.FromArgb(177, 163, 193);
            VersionCell0.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            VersionCell0.Borders[BorderType.BottomBorder].Color = Color.FromArgb(177, 163, 193);
            #endregion

            #region CellNotPresent
            indexColor = excel.Styles.Add();
            CellNotPresent = excel.Styles[indexColor];
            CellNotPresent.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellNotPresent.Pattern = BackgroundType.Solid;
            CellNotPresent.Font.Color = Color.White;
            CellNotPresent.Font.Size = 8;
            CellNotPresent.Font.IsBold = false;
            CellNotPresent.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellNotPresent.Borders[BorderType.RightBorder].Color = Color.FromArgb(177, 163, 193);
            CellNotPresent.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellNotPresent.Borders[BorderType.BottomBorder].Color = Color.FromArgb(177, 163, 193);
            #endregion

            #region CellPresent
            
            indexColor = excel.Styles.Add();
            CellPresent = excel.Styles[indexColor];
            CellPresent.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellPresent.Pattern = BackgroundType.Solid;
            CellPresent.Font.Color = Color.White;
            CellPresent.Font.Size = 8;
            CellPresent.Font.IsBold = false;
            CellPresent.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellPresent.Borders[BorderType.RightBorder].Color = Color.FromArgb(177, 163, 193);
            CellPresent.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellPresent.Borders[BorderType.BottomBorder].Color = Color.FromArgb(177, 163, 193);
            #endregion

            #region CellExtended
            
            indexColor = excel.Styles.Add();
            CellExtended = excel.Styles[indexColor];
            CellExtended.ForegroundColor = Color.FromArgb(162, 125, 203);
            CellExtended.Pattern = BackgroundType.Solid;
            CellExtended.Font.Color = Color.FromArgb(162, 125, 203);
            CellExtended.Font.Size = 8;
            CellExtended.Font.IsBold = false;
            CellExtended.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellExtended.Borders[BorderType.RightBorder].Color = Color.FromArgb(177, 163, 193);
            CellExtended.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellExtended.Borders[BorderType.BottomBorder].Color = Color.FromArgb(177, 163, 193);
            #endregion

            #region CellTitle
            
            indexColor = excel.Styles.Add();
            CellTitle = excel.Styles[indexColor];
            CellTitle.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellTitle.Pattern = BackgroundType.Solid;
            CellTitle.Font.Color = Color.White;
            CellTitle.Font.Size = 9;
            CellTitle.Font.IsBold = true;
            CellTitle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellTitle.Borders[BorderType.RightBorder].Color = Color.White;
            CellTitle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellTitle.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellYear
            
            indexColor = excel.Styles.Add();
            CellYear = excel.Styles[indexColor];
            CellYear.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellYear.Pattern = BackgroundType.Solid;
            CellYear.Font.Color = Color.White;
            CellYear.Font.Size = 9;
            CellYear.Font.IsBold = true;
            #endregion

            #region CellYear1
            
            indexColor = excel.Styles.Add();
            CellYear1 = excel.Styles[indexColor];
            CellYear1.ForegroundColor = Color.FromArgb(100, 72, 131) ;
            CellYear1.Pattern = BackgroundType.Solid;
            CellYear1.Font.Color = Color.White;
            CellYear1.Font.Size = 9;
            CellYear1.Font.IsBold = true;
            CellYear1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellYear1.Borders[BorderType.RightBorder].Color = Color.White;
            #endregion

            #region CellPeriod
            
            indexColor = excel.Styles.Add();
            CellPeriod = excel.Styles[indexColor];
            CellPeriod.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellPeriod.Pattern = BackgroundType.Solid;
            CellPeriod.Font.Color = Color.Black;
            CellPeriod.Font.Size = 8;
            CellPeriod.Font.IsBold = false;
            CellPeriod.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellPeriod.Borders[BorderType.RightBorder].Color = Color.FromArgb(100, 72, 131);
            CellPeriod.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellPeriod.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellPeriodIncomplete
            
            indexColor = excel.Styles.Add();
            CellPeriodIncomplete = excel.Styles[indexColor];
            CellPeriodIncomplete.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellPeriodIncomplete.Pattern = BackgroundType.Solid;
            CellPeriodIncomplete.Font.Color = Color.Black;
            CellPeriodIncomplete.Font.Size = 8;
            CellPeriodIncomplete.Font.IsBold = false;
            CellPeriodIncomplete.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellPeriodIncomplete.Borders[BorderType.RightBorder].Color = Color.FromArgb(100, 72, 131);
            CellPeriodIncomplete.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellPeriodIncomplete.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellDay
            
            indexColor = excel.Styles.Add();
            CellDay = excel.Styles[indexColor];
            CellDay.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellDay.Pattern = BackgroundType.Solid;
            CellDay.Font.Color = Color.Black;
            CellDay.Font.Size = 8;
            CellDay.Font.IsBold = false;
            CellDay.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellDay.Borders[BorderType.RightBorder].Color = Color.FromArgb(100, 72, 131);
            CellDay.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellDay.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellDayWE
            
            indexColor = excel.Styles.Add();
            CellDayWE = excel.Styles[indexColor];
            CellDayWE.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellDayWE.Pattern = BackgroundType.Solid;
            CellDayWE.Font.Color = Color.Black;
            CellDayWE.Font.Size = 8;
            CellDayWE.Font.IsBold = false;
            CellDayWE.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellDayWE.Borders[BorderType.RightBorder].Color = Color.FromArgb(100, 72, 131);
            CellDayWE.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellDayWE.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelTotal
            
            indexColor = excel.Styles.Add();
            CellLevelTotal = excel.Styles[indexColor];
            CellLevelTotal.ForegroundColor = Color.White ;
            CellLevelTotal.Pattern = BackgroundType.Solid;
            CellLevelTotal.Font.Color = Color.Black;
            CellLevelTotal.Font.Size = 8;
            CellLevelTotal.Font.IsBold = true;
            CellLevelTotal.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotal.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelTotal.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotal.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL1
            
            indexColor = excel.Styles.Add();
            CellLevelL1 = excel.Styles[indexColor];
            CellLevelL1.ForegroundColor = Color.FromArgb(177, 163, 193);//Color.FromArgb(100,72,131)
            CellLevelL1.Pattern = BackgroundType.Solid;
            CellLevelL1.Font.Color = Color.Black;
            CellLevelL1.Font.Size = 8;
            CellLevelL1.Font.IsBold = true;
            CellLevelL1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_1
            
            indexColor = excel.Styles.Add();
            CellLevelL2_1 = excel.Styles[indexColor];
            CellLevelL2_1.ForegroundColor = Color.FromArgb(225, 224, 218);
            CellLevelL2_1.Pattern = BackgroundType.Solid;
            CellLevelL2_1.Font.Color = Color.Black;
            CellLevelL2_1.Font.Size = 8;
            CellLevelL2_1.Font.IsBold = false;
            CellLevelL2_1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_2
            
            indexColor = excel.Styles.Add();
            CellLevelL2_2 = excel.Styles[indexColor];
            CellLevelL2_2.ForegroundColor = Color.FromArgb(208, 200, 218);
            CellLevelL2_2.Pattern = BackgroundType.Solid;
            CellLevelL2_2.Font.Color = Color.Black;
            CellLevelL2_2.Font.Size = 8;
            CellLevelL2_2.Font.IsBold = false;
            CellLevelL2_2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL3
            
            indexColor = excel.Styles.Add();
            CellLevelL3 = excel.Styles[indexColor];
            CellLevelL3.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellLevelL3.Pattern = BackgroundType.Solid;
            CellLevelL3.Font.Color = Color.White;
            CellLevelL3.Font.Size = 9;
            CellLevelL3.Font.IsBold = true;
            CellLevelL3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL3.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL3.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL4
            
            indexColor = excel.Styles.Add();
            CellLevelL4 = excel.Styles[indexColor];
            CellLevelL4.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellLevelL4.Pattern = BackgroundType.Solid;
            CellLevelL4.Font.Color = Color.White;
            CellLevelL4.Font.Size = 9;
            CellLevelL4.Font.IsBold = true;
            CellLevelL4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL4.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL4.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelTotalNb
            
            indexColor = excel.Styles.Add();
            CellLevelTotalNb = excel.Styles[indexColor];
            CellLevelTotalNb.ForegroundColor = Color.White;
            CellLevelTotalNb.Pattern = BackgroundType.Solid;
            CellLevelTotalNb.Font.Color = Color.Black;
            CellLevelTotalNb.Font.Size = 8;
            CellLevelTotalNb.Font.IsBold = true;
            CellLevelTotalNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotalNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelTotalNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotalNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelTotalPdmNb
            
            indexColor = excel.Styles.Add();
            CellLevelTotalPdmNb = excel.Styles[indexColor];
            CellLevelTotalPdmNb.ForegroundColor = Color.White;
            CellLevelTotalPdmNb.Pattern = BackgroundType.Solid;
            CellLevelTotalPdmNb.Font.Color = Color.Black;
            CellLevelTotalPdmNb.Font.Size = 8;
            CellLevelTotalPdmNb.Font.IsBold = true;
            CellLevelTotalPdmNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotalPdmNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelTotalPdmNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotalPdmNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL1Nb
            
            indexColor = excel.Styles.Add();
            CellLevelL1Nb = excel.Styles[indexColor];
            CellLevelL1Nb.ForegroundColor = Color.FromArgb(177, 163, 193);//Color.FromArgb(100,72,131)
            CellLevelL1Nb.Pattern = BackgroundType.Solid;
            CellLevelL1Nb.Font.Color = Color.Black;
            CellLevelL1Nb.Font.Size = 8;
            CellLevelL1Nb.Font.IsBold = true;
            CellLevelL1Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL1Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL1PdmNb
            
            indexColor = excel.Styles.Add();
            CellLevelL1PdmNb = excel.Styles[indexColor];
            CellLevelL1PdmNb.ForegroundColor = Color.FromArgb(177, 163, 193);//Color.FromArgb(100,72,131)
            CellLevelL1PdmNb.Pattern = BackgroundType.Solid;
            CellLevelL1PdmNb.Font.Color = Color.Black;
            CellLevelL1PdmNb.Font.Size = 8;
            CellLevelL1PdmNb.Font.IsBold = true;
            CellLevelL1PdmNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1PdmNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL1PdmNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1PdmNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_1Nb
            
            indexColor = excel.Styles.Add();
            CellLevelL2_1Nb = excel.Styles[indexColor];
            CellLevelL2_1Nb.ForegroundColor = Color.FromArgb(225, 224, 218);
            CellLevelL2_1Nb.Pattern = BackgroundType.Solid;
            CellLevelL2_1Nb.Font.Color = Color.Black;
            CellLevelL2_1Nb.Font.Size = 8;
            CellLevelL2_1Nb.Font.IsBold = false;
            CellLevelL2_1Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_1Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_1PdmNb
            
            indexColor = excel.Styles.Add();
            CellLevelL2_1PdmNb = excel.Styles[indexColor];
            CellLevelL2_1PdmNb.ForegroundColor = Color.FromArgb(225, 224, 218);
            CellLevelL2_1PdmNb.Pattern = BackgroundType.Solid;
            CellLevelL2_1PdmNb.Font.Color = Color.Black;
            CellLevelL2_1PdmNb.Font.Size = 8;
            CellLevelL2_1PdmNb.Font.IsBold = false;
            CellLevelL2_1PdmNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1PdmNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_1PdmNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1PdmNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_2Nb
            
            indexColor = excel.Styles.Add();
            CellLevelL2_2Nb = excel.Styles[indexColor];
            CellLevelL2_2Nb.ForegroundColor = Color.FromArgb(208, 200, 218);
            CellLevelL2_2Nb.Pattern = BackgroundType.Solid;
            CellLevelL2_2Nb.Font.Color = Color.Black;
            CellLevelL2_2Nb.Font.Size = 8;
            CellLevelL2_2Nb.Font.IsBold = false;
            CellLevelL2_2Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_2Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_2PdmNb
            
            indexColor = excel.Styles.Add();
            CellLevelL2_2PdmNb = excel.Styles[indexColor];
            CellLevelL2_2PdmNb.ForegroundColor = Color.FromArgb(208, 200, 218);
            CellLevelL2_2PdmNb.Pattern = BackgroundType.Solid;
            CellLevelL2_2PdmNb.Font.Color = Color.Black;
            CellLevelL2_2PdmNb.Font.Size = 8;
            CellLevelL2_2PdmNb.Font.IsBold = false;
            CellLevelL2_2PdmNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2PdmNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_2PdmNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2PdmNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL3Nb
            
            indexColor = excel.Styles.Add();
            CellLevelL3Nb = excel.Styles[indexColor];
            CellLevelL3Nb.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellLevelL3Nb.Pattern = BackgroundType.Solid;
            CellLevelL3Nb.Font.Color = Color.White;
            CellLevelL3Nb.Font.Size = 9;
            CellLevelL3Nb.Font.IsBold = true;
            CellLevelL3Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL3Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL3Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL3Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL4Nb
            
            indexColor = excel.Styles.Add();
            CellLevelL4Nb = excel.Styles[indexColor];
            CellLevelL4Nb.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellLevelL4Nb.Pattern = BackgroundType.Solid;
            CellLevelL4Nb.Font.Color = Color.White;
            CellLevelL4Nb.Font.Size = 9;
            CellLevelL4Nb.Font.IsBold = true;
            CellLevelL4Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL4Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL4Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL4Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellVersions
            
            indexColor = excel.Styles.Add();
            Aspose.Cells.Style cellTemp = excel.Styles[indexColor];
            cellTemp.ForegroundColor = Color.FromArgb(100, 72, 131);
            cellTemp.Pattern = BackgroundType.Solid;
            cellTemp.Font.Color = Color.White;
            cellTemp.Font.Size = 9;
            cellTemp.Font.IsBold = true;
            cellTemp.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            cellTemp.Borders[BorderType.RightBorder].Color = Color.White;
            cellTemp.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            cellTemp.Borders[BorderType.BottomBorder].Color = Color.White;

            CellVersions.Add(2, cellTemp);
            CellVersions.Add(3, cellTemp);
            CellVersions.Add(4, cellTemp);
            CellVersions.Add(5, cellTemp);
            CellVersions.Add(6, cellTemp);
            CellVersions.Add(7, cellTemp);
            CellVersions.Add(8, cellTemp);
            CellVersions.Add(9, cellTemp);
            CellVersions.Add(10, cellTemp);
            CellVersions.Add(11, cellTemp);
            CellVersions.Add(12, cellTemp);
            CellVersions.Add(13, cellTemp);
            CellVersions.Add(14, cellTemp);
            CellVersions.Add(15, cellTemp);
            CellVersions.Add(16, cellTemp);
            CellVersions.Add(17, cellTemp);
            CellVersions.Add(18, cellTemp);
            CellVersions.Add(19, cellTemp);
            CellVersions.Add(20, cellTemp);
            CellVersions.Add(21, cellTemp);
            CellVersions.Add(22, cellTemp);
            CellVersions.Add(23, cellTemp);
            CellVersions.Add(24, cellTemp);
            CellVersions.Add(25, cellTemp);
            CellVersions.Add(26, cellTemp);
            CellVersions.Add(27, cellTemp);
            CellVersions.Add(28, cellTemp);
            CellVersions.Add(29, cellTemp);
            CellVersions.Add(30, cellTemp);
            CellVersions.Add(31, cellTemp);
            #endregion

        }
        #endregion
    }
    #endregion

    public class ExcelSheetMediaScheduleStyle : MediaScheduleSheetStyle {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ExcelSheetMediaScheduleStyle(Workbook excel)
            : base(excel) {

            
        }
        #endregion

    }
}

