using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CstClassif = TNS.AdExpress.Constantes.Classification;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

namespace TNS.AdExpressI.ProductClassReports.Russia.GenericEngines{
    /// <summary>
    /// Class used to define common functions shared between the diffrent results
    /// </summary>
    public static class CommonFunctions{

        #region InitDictionariesData

        #region For Classif1/Classif2 cases
        /// <summary>
        /// Used to transform all Datatabels (contained in the Dataset) on dicionaries
        /// It helps to access directly an information
        /// The first key correspond to classification levels indexes 
        /// Example : if we have two media levels : we'll have as keys (1_1, 1_2)
        ///           if we one media level and two product levels : we'll have as keys (1_1, 2_1_1, 2_1_2)
        /// The second key correpsond to classification levels identifiers
        /// Example : if we have two media levels (classification identifiers : 234, 1423) : we'll have as identifiers (234, 234_1423)
        ///           if we one media level and two product levels (classification identifiers : media 234; product 23456, 543567) : we'll have as keys (234, 234_23456, 234_23456_543567) 
        /// </summary>
        /// <param name="dsData">DataSet that we've get from DAL</param>
        /// <returns>Dicionary corresponding to all Datatables contained in the Dataset</returns>
        public static Dictionary<string, Dictionary<string, DataRow>> InitDictionariesData(DataSet dsData, int mainLevelsCount, int secondLevelsCount, CstClassif.Branch.type C1_TYPE, CstClassif.Branch.type C2_TYPE)
        {

            Dictionary<string, Dictionary<string, DataRow>> globalDictionary = new Dictionary<string, Dictionary<string, DataRow>>();
            Dictionary<string, DataRow> rowDictionary = new Dictionary<string, DataRow>();
            string C1_ID_NAME = (C1_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.media) ? "ID_M" : "ID_P";
            string C2_ID_NAME = (C2_TYPE == TNS.AdExpress.Constantes.Classification.Branch.type.product) ? "ID_P" : "ID_M";
            string TOTAL_LABEL = string.Empty, C1_LABEL = string.Empty, C2_LABEL = string.Empty;

            DataTable dtT = dsData.Tables["TOTAL"];
            foreach (DataRow dr in dtT.Rows)
            {
                rowDictionary.Add("TOTAL", dr);
            }
            globalDictionary.Add("TOTAL", rowDictionary);
            rowDictionary = new Dictionary<string, DataRow>();

            for (int i = 1; i <= secondLevelsCount; i++)
            {
                dtT = dsData.Tables["TOTAL_" + i + ""];
                foreach (DataRow dr in dtT.Rows)
                {
                    TOTAL_LABEL = string.Empty;

                    for (int k = 1; k <= i; k++)
                        TOTAL_LABEL += dr[C2_ID_NAME + k].ToString() + "_";

                    TOTAL_LABEL = TOTAL_LABEL.Substring(0, TOTAL_LABEL.Length - 1);
                    rowDictionary.Add(TOTAL_LABEL, dr);
                }
                globalDictionary.Add("TOTAL_" + i, rowDictionary);
                rowDictionary = new Dictionary<string, DataRow>();
            }

            for (int i = 1; i <= mainLevelsCount; i++)
            {
                DataTable dtC1 = dsData.Tables["CLASSIF1_" + i + ""];

                foreach (DataRow dr in dtC1.Rows)
                {
                    C1_LABEL = string.Empty;

                    for (int k = 1; k <= i; k++)
                        C1_LABEL += dr[C1_ID_NAME + k].ToString() + "_";

                    C1_LABEL = C1_LABEL.Substring(0, C1_LABEL.Length - 1);
                    rowDictionary.Add(C1_LABEL, dr);
                }
                globalDictionary.Add("CLASSIF1_" + i, rowDictionary);
                rowDictionary = new Dictionary<string, DataRow>();

                for (int j = 1; j <= secondLevelsCount; j++)
                {
                    DataTable dtC2 = dsData.Tables["CLASSIF2_" + i + "_" + j];

                    foreach (DataRow dr in dtC2.Rows)
                    {
                        C2_LABEL = string.Empty;

                        for (int k = 1; k <= i; k++)
                            C2_LABEL += dr[C1_ID_NAME + k].ToString() + "_";

                        for (int l = 1; l <= j; l++)
                            C2_LABEL += dr[C2_ID_NAME + l].ToString() + "_";

                        C2_LABEL = C2_LABEL.Substring(0, C2_LABEL.Length - 1);

                        rowDictionary.Add(C2_LABEL, dr);
                    }
                    globalDictionary.Add("CLASSIF2_" + i + "_" + j, rowDictionary);
                    rowDictionary = new Dictionary<string, DataRow>();
                }
            }

            return globalDictionary;

        }
        #endregion

        #region For Classif1 cases
        /// <summary>
        /// Used to transform all Datatabels (contained in the Dataset) on dicionaries
        /// It helps to access directly an information
        /// The first key correspond to classification levels indexes 
        /// Example : if we have two media levels : we'll have as keys (1_1, 1_2)
        /// The second key correpsond to classification levels identifiers
        /// Example : if we have two media levels (classification identifiers : 234, 1423) : we'll have as identifiers (234, 234_1423)
        /// </summary>
        /// <param name="dsData">DataSet that we've get from DAL</param>
        /// <param name="levelsCount">Levels number</param>
        /// <param name="tableType">Result table type</param>
        /// <returns>Dicionary corresponding to all Datatables contained in the Dataset</returns>
        public static Dictionary<string, Dictionary<string, DataRow>> InitDictionariesData(DataSet dsData, int levelsCount, int tableType)
        {

            Dictionary<string, Dictionary<string, DataRow>> globalDictionary = new Dictionary<string, Dictionary<string, DataRow>>();
            Dictionary<string, DataRow> rowDictionary = new Dictionary<string, DataRow>();
            string C1_ID_NAME = string.Empty;
            string C1_LABEL = string.Empty;
            bool mediaLevel = false;

            if (tableType != CstFormat.PreformatedTables.product_X_Year.GetHashCode() &&
                tableType != CstFormat.PreformatedTables.productYear_X_Cumul.GetHashCode() &&
                tableType != CstFormat.PreformatedTables.productYear_X_Mensual.GetHashCode())
                mediaLevel = true;
            C1_ID_NAME = (mediaLevel) ? "ID_M" : "ID_P";

            DataTable dtT = dsData.Tables["TOTAL"];
            foreach (DataRow dr in dtT.Rows)
            {
                rowDictionary.Add("TOTAL", dr);
            }
            globalDictionary.Add("TOTAL", rowDictionary);
            rowDictionary = new Dictionary<string, DataRow>();


            for (int i = 1; i <= levelsCount; i++)
            {
                DataTable dtC1 = dsData.Tables["CLASSIF1_" + i + ""];

                foreach (DataRow dr in dtC1.Rows)
                {
                    C1_LABEL = string.Empty;

                    for (int k = 1; k <= i; k++)
                        C1_LABEL += dr[C1_ID_NAME + k].ToString() + "_";

                    C1_LABEL = C1_LABEL.Substring(0, C1_LABEL.Length - 1);
                    rowDictionary.Add(C1_LABEL, dr);
                }
                globalDictionary.Add("CLASSIF1_" + i, rowDictionary);
                rowDictionary = new Dictionary<string, DataRow>();

            }

            return globalDictionary;

        }
        #endregion

        #endregion

    }
}
