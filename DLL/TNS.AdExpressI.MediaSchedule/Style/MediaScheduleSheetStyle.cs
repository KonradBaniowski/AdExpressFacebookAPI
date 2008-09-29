using System; 
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Aspose.Excel;

namespace TNS.AdExpressI.MediaSchedule.Style {
    #region Base Class
    /// <summary>
    /// Define styles used in MediaSchedule for Anubis
    /// </summary>
    public abstract class MediaScheduleSheetStyle {

        #region Properties

        #region styles
        public Aspose.Excel.Style VersionCell0;
        public Aspose.Excel.Style CellPresent;
        public Aspose.Excel.Style CellNotPresent;
        public Aspose.Excel.Style CellExtended;
        public Aspose.Excel.Style CellTitle;
        public Aspose.Excel.Style CellYear;
        public Aspose.Excel.Style CellYear1;
        public Aspose.Excel.Style CellPeriod;
        public Aspose.Excel.Style CellPeriodIncomplete;
        public Aspose.Excel.Style CellDay;
        public Aspose.Excel.Style CellDayWE;
        public Aspose.Excel.Style CellLevelTotal;
        public Aspose.Excel.Style CellLevelL1;
        public Aspose.Excel.Style CellLevelL2_1;
        public Aspose.Excel.Style CellLevelL2_2;
        public Aspose.Excel.Style CellLevelL3;
        public Aspose.Excel.Style CellLevelL4;
        public Aspose.Excel.Style CellLevelTotalNb;
        public Aspose.Excel.Style CellLevelTotalPdmNb;
        public Aspose.Excel.Style CellLevelL1Nb;
        public Aspose.Excel.Style CellLevelL1PdmNb;
        public Aspose.Excel.Style CellLevelL2_1Nb;
        public Aspose.Excel.Style CellLevelL2_1PdmNb;
        public Aspose.Excel.Style CellLevelL2_2Nb;
        public Aspose.Excel.Style CellLevelL2_2PdmNb;
        public Aspose.Excel.Style CellLevelL3Nb;
        public Aspose.Excel.Style CellLevelL4Nb;
        public Dictionary<int, Aspose.Excel.Style> CellVersions = new Dictionary<int, Aspose.Excel.Style>();
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MediaScheduleSheetStyle(Excel excel) {

            int i = 0;

            #region VersionCell0
            excel.Styles.Add();
            VersionCell0 = excel.Styles[i];
            VersionCell0.ForegroundColor = Color.FromArgb(100, 72, 131);
            VersionCell0.Font.Color = Color.White;
            VersionCell0.Font.Size = 8;
            VersionCell0.Font.IsBold = false;
            VersionCell0.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            VersionCell0.Borders[BorderType.RightBorder].Color = Color.FromArgb(177, 163, 193);
            VersionCell0.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            VersionCell0.Borders[BorderType.BottomBorder].Color = Color.FromArgb(177, 163, 193);
            #endregion

            #region CellNotPresent
            i++;
            excel.Styles.Add();
            CellNotPresent = excel.Styles[i];
            CellNotPresent.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellNotPresent.Font.Color = Color.White;
            CellNotPresent.Font.Size = 8;
            CellNotPresent.Font.IsBold = false;
            CellNotPresent.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellNotPresent.Borders[BorderType.RightBorder].Color = Color.FromArgb(177, 163, 193);
            CellNotPresent.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellNotPresent.Borders[BorderType.BottomBorder].Color = Color.FromArgb(177, 163, 193);
            #endregion

            #region CellPresent
            i++;
            excel.Styles.Add();
            CellPresent = excel.Styles[i];
            CellPresent.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellPresent.Font.Color = Color.White;
            CellPresent.Font.Size = 8;
            CellPresent.Font.IsBold = false;
            CellPresent.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellPresent.Borders[BorderType.RightBorder].Color = Color.FromArgb(177, 163, 193);
            CellPresent.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellPresent.Borders[BorderType.BottomBorder].Color = Color.FromArgb(177, 163, 193);
            #endregion

            #region CellExtended
            i++;
            excel.Styles.Add();
            CellExtended = excel.Styles[i];
            CellExtended.ForegroundColor = Color.FromArgb(162, 125, 203);
            CellExtended.Font.Color = Color.FromArgb(162, 125, 203);
            CellExtended.Font.Size = 8;
            CellExtended.Font.IsBold = false;
            CellExtended.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellExtended.Borders[BorderType.RightBorder].Color = Color.FromArgb(177, 163, 193);
            CellExtended.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellExtended.Borders[BorderType.BottomBorder].Color = Color.FromArgb(177, 163, 193);
            #endregion

            #region CellTitle
            i++;
            excel.Styles.Add();
            CellTitle = excel.Styles[i];
            CellTitle.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellTitle.Font.Color = Color.White;
            CellTitle.Font.Size = 9;
            CellTitle.Font.IsBold = true;
            CellTitle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellTitle.Borders[BorderType.RightBorder].Color = Color.White;
            CellTitle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellTitle.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellYear
            i++;
            excel.Styles.Add();
            CellYear = excel.Styles[i];
            CellYear.ForegroundColor = Color.FromArgb(100, 72, 131);
            CellYear.Font.Color = Color.White;
            CellYear.Font.Size = 9;
            CellYear.Font.IsBold = true;
            #endregion

            #region CellYear1
            i++;
            excel.Styles.Add();
            CellYear1 = excel.Styles[i];
            CellYear1.ForegroundColor = Color.FromArgb(100, 72, 131) ;
            CellYear1.Font.Color = Color.White;
            CellYear1.Font.Size = 9;
            CellYear1.Font.IsBold = true;
            CellYear1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellYear1.Borders[BorderType.RightBorder].Color = Color.White;
            #endregion

            #region CellPeriod
            i++;
            excel.Styles.Add();
            CellPeriod = excel.Styles[i];
            CellPeriod.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellPeriod.Font.Color = Color.Black;
            CellPeriod.Font.Size = 8;
            CellPeriod.Font.IsBold = false;
            CellPeriod.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellPeriod.Borders[BorderType.RightBorder].Color = Color.FromArgb(100, 72, 131);
            CellPeriod.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellPeriod.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellPeriodIncomplete
            i++;
            excel.Styles.Add();
            CellPeriodIncomplete = excel.Styles[i];
            CellPeriodIncomplete.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellPeriodIncomplete.Font.Color = Color.Black;
            CellPeriodIncomplete.Font.Size = 8;
            CellPeriodIncomplete.Font.IsBold = false;
            CellPeriodIncomplete.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellPeriodIncomplete.Borders[BorderType.RightBorder].Color = Color.FromArgb(100, 72, 131);
            CellPeriodIncomplete.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellPeriodIncomplete.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellDay
            i++;
            excel.Styles.Add();
            CellDay = excel.Styles[i];
            CellDay.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellDay.Font.Color = Color.Black;
            CellDay.Font.Size = 8;
            CellDay.Font.IsBold = false;
            CellDay.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellDay.Borders[BorderType.RightBorder].Color = Color.FromArgb(100, 72, 131);
            CellDay.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellDay.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellDayWE
            i++;
            excel.Styles.Add();
            CellDayWE = excel.Styles[i];
            CellDayWE.ForegroundColor = Color.FromArgb(177, 163, 193);
            CellDayWE.Font.Color = Color.Black;
            CellDayWE.Font.Size = 8;
            CellDayWE.Font.IsBold = false;
            CellDayWE.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellDayWE.Borders[BorderType.RightBorder].Color = Color.FromArgb(100, 72, 131);
            CellDayWE.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellDayWE.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelTotal
            i++;
            excel.Styles.Add();
            CellLevelTotal = excel.Styles[i];
            CellLevelTotal.ForegroundColor = Color.White ;
            CellLevelTotal.Font.Color = Color.Black;
            CellLevelTotal.Font.Size = 8;
            CellLevelTotal.Font.IsBold = true;
            CellLevelTotal.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotal.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelTotal.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotal.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL1
            i++;
            excel.Styles.Add();
            CellLevelL1 = excel.Styles[i];
            CellLevelL1.ForegroundColor = Color.FromArgb(177, 163, 193);//Color.FromArgb(100,72,131)
            CellLevelL1.Font.Color = Color.Black;
            CellLevelL1.Font.Size = 8;
            CellLevelL1.Font.IsBold = true;
            CellLevelL1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_1
            i++;
            excel.Styles.Add();
            CellLevelL2_1 = excel.Styles[i];
            CellLevelL2_1.ForegroundColor = Color.FromArgb(225, 224, 218);
            CellLevelL2_1.Font.Color = Color.Black;
            CellLevelL2_1.Font.Size = 8;
            CellLevelL2_1.Font.IsBold = false;
            CellLevelL2_1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_2
            i++;
            excel.Styles.Add();
            CellLevelL2_2 = excel.Styles[i];
            CellLevelL2_2.ForegroundColor = Color.FromArgb(208, 200, 218);
            CellLevelL2_2.Font.Color = Color.Black;
            CellLevelL2_2.Font.Size = 8;
            CellLevelL2_2.Font.IsBold = false;
            CellLevelL2_2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL3
            i++;
            excel.Styles.Add();
            CellLevelL3 = excel.Styles[i];
            CellLevelL3.ForegroundColor = Color.FromArgb(100, 72, 131); ;
            CellLevelL3.Font.Color = Color.White;
            CellLevelL3.Font.Size = 9;
            CellLevelL3.Font.IsBold = true;
            CellLevelL3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL3.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL3.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL4
            i++;
            excel.Styles.Add();
            CellLevelL4 = excel.Styles[i];
            CellLevelL4.ForegroundColor = Color.FromArgb(100, 72, 131); ;
            CellLevelL4.Font.Color = Color.White;
            CellLevelL4.Font.Size = 9;
            CellLevelL4.Font.IsBold = true;
            CellLevelL4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL4.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL4.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelTotalNb
            i++;
            excel.Styles.Add();
            CellLevelTotalNb = excel.Styles[i];
            CellLevelTotalNb.ForegroundColor = Color.White;
            CellLevelTotalNb.Font.Color = Color.Black;
            CellLevelTotalNb.Font.Size = 8;
            CellLevelTotalNb.Font.IsBold = true;
            CellLevelTotalNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotalNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelTotalNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotalNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelTotalPdmNb
            i++;
            excel.Styles.Add();
            CellLevelTotalPdmNb = excel.Styles[i];
            CellLevelTotalPdmNb.ForegroundColor = Color.White;
            CellLevelTotalPdmNb.Font.Color = Color.Black;
            CellLevelTotalPdmNb.Font.Size = 8;
            CellLevelTotalPdmNb.Font.IsBold = true;
            CellLevelTotalPdmNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotalPdmNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelTotalPdmNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelTotalPdmNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL1Nb
            i++;
            excel.Styles.Add();
            CellLevelL1Nb = excel.Styles[i];
            CellLevelL1Nb.ForegroundColor = Color.FromArgb(177, 163, 193);//Color.FromArgb(100,72,131)
            CellLevelL1Nb.Font.Color = Color.Black;
            CellLevelL1Nb.Font.Size = 8;
            CellLevelL1Nb.Font.IsBold = true;
            CellLevelL1Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL1Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL1PdmNb
            i++;
            excel.Styles.Add();
            CellLevelL1PdmNb = excel.Styles[i];
            CellLevelL1PdmNb.ForegroundColor = Color.FromArgb(177, 163, 193);//Color.FromArgb(100,72,131)
            CellLevelL1PdmNb.Font.Color = Color.Black;
            CellLevelL1PdmNb.Font.Size = 8;
            CellLevelL1PdmNb.Font.IsBold = true;
            CellLevelL1PdmNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1PdmNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL1PdmNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL1PdmNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_1Nb
            i++;
            excel.Styles.Add();
            CellLevelL2_1Nb = excel.Styles[i];
            CellLevelL2_1Nb.ForegroundColor = Color.FromArgb(225, 224, 218);
            CellLevelL2_1Nb.Font.Color = Color.Black;
            CellLevelL2_1Nb.Font.Size = 8;
            CellLevelL2_1Nb.Font.IsBold = false;
            CellLevelL2_1Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_1Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_1PdmNb
            i++;
            excel.Styles.Add();
            CellLevelL2_1PdmNb = excel.Styles[i];
            CellLevelL2_1PdmNb.ForegroundColor = Color.FromArgb(225, 224, 218);
            CellLevelL2_1PdmNb.Font.Color = Color.Black;
            CellLevelL2_1PdmNb.Font.Size = 8;
            CellLevelL2_1PdmNb.Font.IsBold = false;
            CellLevelL2_1PdmNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1PdmNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_1PdmNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_1PdmNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_2Nb
            i++;
            excel.Styles.Add();
            CellLevelL2_2Nb = excel.Styles[i];
            CellLevelL2_2Nb.ForegroundColor = Color.FromArgb(208, 200, 218);
            CellLevelL2_2Nb.Font.Color = Color.Black;
            CellLevelL2_2Nb.Font.Size = 8;
            CellLevelL2_2Nb.Font.IsBold = false;
            CellLevelL2_2Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_2Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL2_2PdmNb
            i++;
            excel.Styles.Add();
            CellLevelL2_2PdmNb = excel.Styles[i];
            CellLevelL2_2PdmNb.ForegroundColor = Color.FromArgb(208, 200, 218);
            CellLevelL2_2PdmNb.Font.Color = Color.Black;
            CellLevelL2_2PdmNb.Font.Size = 8;
            CellLevelL2_2PdmNb.Font.IsBold = false;
            CellLevelL2_2PdmNb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2PdmNb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL2_2PdmNb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL2_2PdmNb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL3Nb
            i++;
            excel.Styles.Add();
            CellLevelL3Nb = excel.Styles[i];
            CellLevelL3Nb.ForegroundColor = Color.FromArgb(100, 72, 131); ;
            CellLevelL3Nb.Font.Color = Color.White;
            CellLevelL3Nb.Font.Size = 9;
            CellLevelL3Nb.Font.IsBold = true;
            CellLevelL3Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL3Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL3Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL3Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellLevelL4Nb
            i++;
            excel.Styles.Add();
            CellLevelL4Nb = excel.Styles[i];
            CellLevelL4Nb.ForegroundColor = Color.FromArgb(100, 72, 131); ;
            CellLevelL4Nb.Font.Color = Color.White;
            CellLevelL4Nb.Font.Size = 9;
            CellLevelL4Nb.Font.IsBold = true;
            CellLevelL4Nb.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            CellLevelL4Nb.Borders[BorderType.RightBorder].Color = Color.White;
            CellLevelL4Nb.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            CellLevelL4Nb.Borders[BorderType.BottomBorder].Color = Color.White;
            #endregion

            #region CellVersions
            i++;
            excel.Styles.Add();
            Aspose.Excel.Style cellTemp = excel.Styles[i];
            cellTemp.ForegroundColor = Color.FromArgb(100, 72, 131); ;
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
        public ExcelSheetMediaScheduleStyle(Excel excel)
            : base(excel) {

            
        }
        #endregion

    }
}

