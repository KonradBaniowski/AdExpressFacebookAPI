using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNS.AdExpressI.Rolex.Style
{
  
        #region Base Class
        /// <summary>
        /// Define styles used in Rolex Schedule
        /// </summary>
        public abstract class RolexScheduleStyle
        {
            #region Properties

            public string NoDataDiv = string.Empty;

            #region Versions Cells styles
            public string VersionCell0 = string.Empty;
            public string CellPresent = string.Empty;
            public string CellNotPresent = string.Empty;
            public string CellExtended = string.Empty;
            public string CellTitle = string.Empty;
            public string CellYear = string.Empty;
            public string CellYear1 = string.Empty;
            public string CellPeriod = string.Empty;
            public string CellPeriodIncomplete = string.Empty;
            public string CellDay = string.Empty;
            public string CellDayWE = string.Empty;
            public string CellLevelTotal = string.Empty;
            public string CellLevelL1 = string.Empty;
            public string CellLevelL2 = string.Empty;
            public string CellLevelL3 = string.Empty;
            public string CellLevelL4 = string.Empty;
            public string CellLevelTotalNb = string.Empty;
            public string CellLevelL1Nb = string.Empty;
            public string CellLevelL2Nb = string.Empty;
            public string CellLevelL3Nb = string.Empty;
            public string CellLevelL4Nb = string.Empty;
            public Dictionary<int, string> CellVersions = new Dictionary<int, string>();
            #endregion

            #endregion

            #region Constructor
            /// <summary>
            /// Default Constructor
            /// </summary>
            public RolexScheduleStyle()
            {
                NoDataDiv = "txtViolet11Bold";
                VersionCell0 = "pc0";
                CellNotPresent = "p3";
                CellPresent = "p4";
                CellExtended = "p5";
                CellTitle = "pt";
                CellYear = "par";
                CellYear1 = "pa1";
                CellPeriod = "pp";
                CellPeriodIncomplete = "ppi";
                CellDay = "pd";
                CellDayWE = "pdw";
                CellLevelTotal = "L0";
                CellLevelL1 = "L1";
                CellLevelL2 = "L2";
                CellLevelL3 = "L3";
                CellLevelL4 = "L4";
                CellLevelTotalNb = "L0nb";
                CellLevelL1Nb = "L1nb";
                CellLevelL2Nb = "L2nb";
                CellLevelL3Nb = "L3nb";
                CellLevelL4Nb = "L4nb";

                CellVersions.Add(1, "pc1");
                #region CellVersions
                CellVersions.Add(2, "pc2");
                CellVersions.Add(3, "pc3");
                CellVersions.Add(4, "pc4");
                CellVersions.Add(5, "pc5");
                CellVersions.Add(6, "pc6");
                CellVersions.Add(7, "pc7");
                CellVersions.Add(8, "pc8");
                CellVersions.Add(9, "pc9");
                CellVersions.Add(10, "pc10");
                CellVersions.Add(11, "pc11");
                CellVersions.Add(12, "pc12");
                CellVersions.Add(13, "pc13");
                CellVersions.Add(14, "pc14");
                CellVersions.Add(15, "pc15");
                CellVersions.Add(16, "pc16");
                CellVersions.Add(17, "pc17");
                CellVersions.Add(18, "pc18");
                CellVersions.Add(19, "pc19");
                CellVersions.Add(20, "pc20");
                CellVersions.Add(21, "pc21");
                CellVersions.Add(22, "pc22");
                CellVersions.Add(23, "pc23");
                CellVersions.Add(24, "pc24");
                CellVersions.Add(25, "pc25");
                CellVersions.Add(26, "pc26");
                CellVersions.Add(27, "pc27");
                CellVersions.Add(28, "pc28");
                CellVersions.Add(29, "pc29");
                CellVersions.Add(30, "pc30");
                CellVersions.Add(31, "pc31");
                #endregion
            }
            #endregion
        }
        #endregion

        #region Default Rolex Schedule
        /// <summary>
        /// Defines Defautl Rolex Schedule Styles
        /// </summary>
        public class DefaultRolexScheduleStyle : RolexScheduleStyle
        {

            #region Constructor
            /// <summary>
            /// Default Constructor
            /// </summary>
            public DefaultRolexScheduleStyle()
                : base()
            {

            }
            #endregion

        }
        #endregion

        #region PDF Rolex Schedule
        /// <summary>
        /// Defines Rolex Schedule Styles for PDF export
        /// </summary>
        public class PDFRolexScheduleStyle : RolexScheduleStyle
        {

            #region Constructor
            /// <summary>
            /// Default Constructor
            /// </summary>
            public PDFRolexScheduleStyle()
                : base()
            {
                CellLevelTotal = "L0export";
                CellLevelL1 = "L1export";
                CellLevelL2 = "L2export";
                CellLevelL3 = "L3export";
                CellLevelL4 = "L4export";
                CellLevelTotalNb = "L0nbexport";
                CellLevelL1Nb = "L1nbexport";
                CellLevelL2Nb = "L2nbexport";
                CellLevelL3Nb = "L3nbexport";
                CellLevelL4Nb = "L4nbexport";
            }
            #endregion

        }
        #endregion

        #region Excel Rolex Schedule
        /// <summary>
        /// Defines Rolex Schedule Styles for Excel export
        /// </summary>
        public class ExcelRolexScheduleStyle : RolexScheduleStyle
        {

            #region Constructor
            /// <summary>
            /// Default Constructor
            /// </summary>
            public ExcelRolexScheduleStyle()
            {
                NoDataDiv = "txtViolet11Bold";
                VersionCell0 = "pc0;";
                CellNotPresent = "p3X";
                CellPresent = "p4X";
                CellExtended = "p5X";
                CellTitle = "ptX";
                CellYear = "paX";
                CellYear1 = "pa1";
                CellPeriod = "ppX";
                CellPeriodIncomplete = "ppiX";
                CellDay = "pdX";
                CellDayWE = "pdwX";
                CellLevelTotal = "L0X";
                CellLevelL1 = "L1X";
                CellLevelL2 = "L2X";
                CellLevelL3 = "L3X";
                CellLevelL4 = "L4X";
                CellLevelTotalNb = "L0Xnb";
                CellLevelL1Nb = "L1Xnb";
                CellLevelL2Nb = "L2Xnb";
                CellLevelL3Nb = "L3Xnb";
                CellLevelL4Nb = "L4Xnb";
            }
            #endregion

        }
        #endregion
    }

