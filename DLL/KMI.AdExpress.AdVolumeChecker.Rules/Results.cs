using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Aspose.Cells;
using KMI.AdExpress.AdVolumeChecker.DAL;
using KMI.AdExpress.AdVolumeChecker.Domain;
using KMI.AdExpress.AdVolumeChecker.Rules.Exceptions;

namespace KMI.AdExpress.AdVolumeChecker.Rules {

    public delegate void MessageDelegate(int progress);

    /// <summary>
    /// Rules for Ad Volume Checker
    /// </summary>
    public class Results {

        public static event MessageDelegate Message;

        protected static void OnMessage(int progress) {
            // If event is wired, fire it!
            if (Message != null)
                Message(progress);
        }

        public static void GenerateExcelResult(string connectionString, List<MediaInformation> mediaInformations, DateTime startDate, DateTime endDate, string versionList, string advertiserList, string productList, bool isIn, string path) {

            try {
                double index = 1;
                double percentComplete;
                Workbook workbook = new Workbook();
                License license = new License();
                license.SetLicense("Aspose.Cells.lic");

                AddMainPage(workbook, startDate, endDate);

                workbook.Worksheets.Add("Supports");
                Worksheet worksheet = workbook.Worksheets["Supports"];
                int lineIndex = 4;
                int total = 1000;

                percentComplete = ((index) / total) * 100;
                OnMessage(Convert.ToInt16(percentComplete));

                Cells cells = worksheet.Cells;

                #region Set Styles
                //Set Header Style
                Aspose.Cells.Style headerStyle = cells["A1"].GetStyle();
                headerStyle.HorizontalAlignment = TextAlignmentType.Center;
                headerStyle.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Font.Color = Color.White;
                headerStyle.Font.IsBold = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 11;
                headerStyle.ForegroundColor = Color.FromArgb(79, 129, 189);
                headerStyle.Pattern = BackgroundType.Solid;
                #endregion

                cells[lineIndex, 0].PutValue("Période : " + startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy"));
                cells[lineIndex, 0].SetStyle(headerStyle);
                cells.Merge(lineIndex, 0, 1, 4);
                lineIndex = 6;

                foreach (MediaInformation mediaInformation in mediaInformations) {
                    //mediaInformation.SetTopDiffusionByDays(AdVolumeCheckerDAL.GetData(connectionString, mediaInformation.Id, startDate, endDate, versionList, advertiserList, productList, isIn));
                    mediaInformation.SetTopDiffusionDetailByDays(AdVolumeCheckerDAL.GetDataByTopDiffusion(connectionString, mediaInformation.Id, startDate, endDate, versionList, advertiserList, productList, isIn),
                                                                 AdVolumeCheckerDAL.GetPreviousLastTopDiffusionData(connectionString, mediaInformation.Id, startDate, endDate, versionList, advertiserList, productList, isIn)
                                                                 , startDate, endDate);
                    lineIndex = AddWorkSheet(mediaInformation, worksheet, lineIndex, startDate, endDate);
                    lineIndex += 4;
                }

                total = GetTotalResultNumbr(mediaInformations);
                //index++;
                percentComplete = ((index) / total) * 100;
                OnMessage(Convert.ToInt16(percentComplete));

                worksheet.Pictures.Add(0, 0, AppDomain.CurrentDomain.BaseDirectory + "\\Images\\logo_Tns.gif");

                foreach (MediaInformation mediaInformation in mediaInformations) {
                    foreach (string daySlot in mediaInformation.SpotDetails) {
                        AddDetailWorkSheet(connectionString, mediaInformation, mediaInformation.Id, mediaInformation.Label, daySlot, startDate, workbook, versionList, advertiserList, productList, isIn);
                        index++;
                        percentComplete = ((index) / total) * 100;
                        OnMessage(Convert.ToInt16(percentComplete));
                    }
                }

                if (workbook.Worksheets.Count > 1) {
                    workbook.Worksheets.RemoveAt(0);
                }

                workbook.Save(path);
            }
            catch (System.Exception et) {
                throw (new AdVolumeCheckerRulesException("Error during the generation of the Excel file (Method GenerateExcelResult) : " + et.Message, et));
            }

        }

        #region Add Main Page
        private static void AddMainPage(Workbook workbook, DateTime startDate, DateTime endDate) {

            workbook.Worksheets.Add("Volumes Publicitaires");
            Worksheet worksheet = workbook.Worksheets["Volumes Publicitaires"];
            //Get the cells collection in the sheet
            Cells cells = worksheet.Cells;

            worksheet.IsGridlinesVisible = false;

            worksheet.Pictures.Add(0, 0, AppDomain.CurrentDomain.BaseDirectory + "\\Images\\logo_Tns.gif");

            #region Set Styles
            //Set Header Style
            Aspose.Cells.Style headerStyle = cells["A1"].GetStyle();
            headerStyle.HorizontalAlignment = TextAlignmentType.Center;
            headerStyle.Font.Color = Color.Gray;
            headerStyle.Font.IsBold = true;
            headerStyle.Font.Name = "Calibri";
            headerStyle.Font.Size = 40;
            headerStyle.Pattern = BackgroundType.Solid;

            //Set Header Style
            Aspose.Cells.Style headerStyle2 = cells["A1"].GetStyle();
            headerStyle2.HorizontalAlignment = TextAlignmentType.Center;
            headerStyle2.Font.Color = Color.Gray;
            headerStyle2.Font.IsBold = true;
            headerStyle2.Font.Name = "Calibri";
            headerStyle2.Font.Size = 12;
            headerStyle2.Pattern = BackgroundType.Solid;
            #endregion

            cells[6, 1].PutValue("Volumes Publicitaires");
            cells[6, 1].SetStyle(headerStyle);
            cells.Merge(6, 1, 1, 10);

            cells[8, 3].PutValue("Période : " + startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy"));
            cells[8, 3].SetStyle(headerStyle2);
            cells.Merge(8, 3, 1, 6);

            worksheet.AutoFitColumns();
            worksheet.AutoFitRows();

            worksheet.Pictures.Add(12, 1, AppDomain.CurrentDomain.BaseDirectory + "\\Images\\LogoAdExpress.jpg");
        }
        #endregion

        #region Add Work Sheet
        /// <summary>
        /// Add Work Sheet
        /// </summary>
        /// <param name="mediaInformation">Media Information</param>
        /// <param name="workbook">Workbook</param>
        private static int AddWorkSheet(MediaInformation mediaInformation, Worksheet worksheet, int lineIndex, DateTime startDate, DateTime endDate) {

            try {
                //Get the cells collection in the sheet
                Cells cells = worksheet.Cells;
                //int lineIndex = 0;
                int columnIndex = 0;
                DateTime date = startDate;
                bool completeSlot = false;
                bool uncommpleteSlot = false;

                #region Set Styles
                //Set Header Style
                Aspose.Cells.Style headerStyle = cells["A1"].GetStyle();
                headerStyle.HorizontalAlignment = TextAlignmentType.Center;
                headerStyle.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Font.Color = Color.White;
                headerStyle.Font.IsBold = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 11;
                headerStyle.ForegroundColor = Color.FromArgb(79, 129, 189);
                headerStyle.Pattern = BackgroundType.Solid;

                Aspose.Cells.Style headerStyleMoyenne = cells["A1"].GetStyle();
                headerStyleMoyenne.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyleMoyenne.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyleMoyenne.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyleMoyenne.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyleMoyenne.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                headerStyleMoyenne.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                headerStyleMoyenne.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                headerStyleMoyenne.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                headerStyleMoyenne.Font.IsBold = true;
                headerStyleMoyenne.ForegroundColor = Color.FromArgb(191, 191, 191);
                headerStyleMoyenne.Font.Name = "Calibri";
                headerStyleMoyenne.Font.Size = 10;
                headerStyleMoyenne.Pattern = BackgroundType.Solid;

                Aspose.Cells.Style headerStyleTotal = cells["A1"].GetStyle();
                headerStyleTotal.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyleTotal.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyleTotal.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyleTotal.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyleTotal.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                headerStyleTotal.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                headerStyleTotal.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                headerStyleTotal.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                headerStyleTotal.Font.Color = Color.White;
                headerStyleTotal.Font.IsBold = true;
                headerStyleTotal.ForegroundColor = Color.FromArgb(184, 204, 228);
                headerStyleTotal.Font.Name = "Calibri";
                headerStyleTotal.Font.Size = 10;
                headerStyleTotal.Pattern = BackgroundType.Solid;

                //Set Row Style
                Aspose.Cells.Style rowStyle1 = cells["A1"].GetStyle();
                rowStyle1.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle1.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle1.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle1.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                rowStyle1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                rowStyle1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                rowStyle1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                rowStyle1.ForegroundColor = Color.FromArgb(220, 230, 241);
                rowStyle1.Font.Name = "Calibri";
                rowStyle1.Font.Size = 10;
                rowStyle1.Pattern = BackgroundType.Solid;

                Aspose.Cells.Style rowStyle1Bold = cells["A1"].GetStyle();
                rowStyle1Bold.Copy(rowStyle1);
                rowStyle1Bold.Font.IsBold = true;

                Aspose.Cells.Style rowStyle1BoldRed = cells["A1"].GetStyle();
                rowStyle1BoldRed.Copy(rowStyle1Bold);
                rowStyle1BoldRed.Font.Color = Color.Red;

                Aspose.Cells.Style rowStyle2 = cells["A1"].GetStyle();
                rowStyle2.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle2.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle2.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle2.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                rowStyle2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                rowStyle2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                rowStyle2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                rowStyle2.Font.Name = "Calibri";
                rowStyle2.Font.Size = 10;
                rowStyle2.Pattern = BackgroundType.Solid;

                Aspose.Cells.Style rowStyle2Bold = cells["A1"].GetStyle();
                rowStyle2Bold.Copy(rowStyle2);
                rowStyle2Bold.Font.IsBold = true;

                Aspose.Cells.Style rowStyle2BoldRed = cells["A1"].GetStyle();
                rowStyle2BoldRed.Copy(rowStyle2Bold);
                rowStyle2BoldRed.Font.Color = Color.Red;

                Aspose.Cells.Style rowStyleTotal = cells["A1"].GetStyle();
                rowStyleTotal.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyleTotal.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyleTotal.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyleTotal.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyleTotal.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                rowStyleTotal.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                rowStyleTotal.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                rowStyleTotal.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                rowStyleTotal.Font.Color = Color.White;
                //rowStyleTotal.Font.IsBold = true;
                rowStyleTotal.ForegroundColor = Color.FromArgb(184, 204, 228);
                rowStyleTotal.Font.Name = "Calibri";
                rowStyleTotal.Font.Size = 10;
                rowStyleTotal.Pattern = BackgroundType.Solid;

                Aspose.Cells.Style rowStyleMoyenne = cells["A1"].GetStyle();
                rowStyleMoyenne.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyleMoyenne.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyleMoyenne.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyleMoyenne.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyleMoyenne.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                rowStyleMoyenne.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                rowStyleMoyenne.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                rowStyleMoyenne.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                //rowStyleMoyenne.Font.IsBold = true;
                rowStyleMoyenne.ForegroundColor = Color.FromArgb(191, 191, 191);
                rowStyleMoyenne.Font.Name = "Calibri";
                rowStyleMoyenne.Font.Size = 10;
                rowStyleMoyenne.Pattern = BackgroundType.Solid;

                Aspose.Cells.Style rowStyleMoyenneRed = cells["A1"].GetStyle();
                rowStyleMoyenneRed.Copy(rowStyleMoyenne);
                rowStyleMoyenneRed.Font.IsBold = true;
                rowStyleMoyenneRed.Font.Color = Color.Red;
                #endregion

                #region Init Header
                cells[lineIndex, columnIndex].PutValue("Support");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Tranche Horaire");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Semaine");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Lundi " + date.ToString("dd/MM"));
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Mardi " + date.AddDays(1).ToString("dd/MM"));
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Mercredi " + date.AddDays(2).ToString("dd/MM"));
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Jeudi " + date.AddDays(3).ToString("dd/MM"));
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Vendredi " + date.AddDays(4).ToString("dd/MM"));
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Samedi " + date.AddDays(5).ToString("dd/MM"));
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Dimanche " + date.AddDays(6).ToString("dd/MM"));
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;
                lineIndex++;
                #endregion

                List<string> daysOfWeek = new List<string>(new string[] { "SEMAINE", "LUNDI", "MARDI", "MERCREDI", "JEUDI", "VENDREDI", "SAMEDI", "DIMANCHE" });
                List<string> topDiffusionSlots = new List<string>(new string[] { "03H - 04H", "04H - 05H", "05H - 06H", "06H - 07H", "07H - 08H", "08H - 09H", "09H - 10H", "10H - 11H", "11H - 12H", "12H - 13H", "13H - 14H", "14H - 15H", "15H - 16H", "16H - 17H", "17H - 18H", "18H - 19H", "19H - 20H", "20H - 21H", "21H - 22H", "22H - 23H", "23H - 24H", "24H - 01H", "01H - 02H", "02H - 03H", "TOTAL", "MOYENNE" });

                #region Init Rows
                foreach (string slot in topDiffusionSlots) {

                    columnIndex = 0;

                    cells[lineIndex, columnIndex].PutValue(mediaInformation.Label);
                    if(slot == "TOTAL")
                        cells[lineIndex, columnIndex].SetStyle(headerStyleTotal);
                    else if (slot == "MOYENNE")
                        cells[lineIndex, columnIndex].SetStyle(headerStyleMoyenne);
                    else if (lineIndex % 2 == 0)
                        cells[lineIndex, columnIndex].SetStyle(rowStyle1Bold);
                    else
                        cells[lineIndex, columnIndex].SetStyle(rowStyle2Bold);
                    columnIndex++;

                    cells[lineIndex, columnIndex].PutValue(slot);
                    if (slot == "TOTAL")
                        cells[lineIndex, columnIndex].SetStyle(headerStyleTotal);
                    else if (slot == "MOYENNE")
                        cells[lineIndex, columnIndex].SetStyle(headerStyleMoyenne);
                    else if (lineIndex % 2 == 0)
                        cells[lineIndex, columnIndex].SetStyle(rowStyle1Bold);
                    else
                        cells[lineIndex, columnIndex].SetStyle(rowStyle2Bold);
                    columnIndex++;

                    foreach (string day in daysOfWeek) {

                        int totalSeconds = (int)mediaInformation.TopDiffusionByDays[day][slot];
                        if (slot == "MOYENNE") {
                            if (!mediaInformation.EncryptedMedia)
                                totalSeconds = totalSeconds / 24;
                            else if (day == "SEMAINE")
                                totalSeconds = Convert.ToInt32(totalSeconds / GetAverageTotalUnencryptedTime(mediaInformation));
                            else
                                totalSeconds = Convert.ToInt32(totalSeconds / GetTotalUnencryptedTime(mediaInformation, day));
                        }
                        int seconds = totalSeconds % 60;
                        int minutes = totalSeconds / 60;
                        string time = minutes + " min " + seconds + " sec";

                        if (totalSeconds == 0)
                            cells[lineIndex, columnIndex].PutValue("");
                        else {
                            cells[lineIndex, columnIndex].PutValue(time);
                            if (totalSeconds > mediaInformation.DurationLimit && day != "SEMAINE" && slot != "TOTAL" && slot != "MOYENNE") {
                                string worksheetName = mediaInformation.Label.Replace(" ", string.Empty).Replace("+", string.Empty) + "_" + day + "_" + slot.Replace(" ", string.Empty).Replace("-", "_") + "!A1";
                                worksheet.Hyperlinks.Add(lineIndex, columnIndex, 1, 1, worksheetName);
                            }
                        }

                        if (lineIndex % 2 == 0) {
                            if (totalSeconds > mediaInformation.DurationLimit && day != "SEMAINE" && slot != "TOTAL" && slot != "MOYENNE")
                                cells[lineIndex, columnIndex].SetStyle(rowStyle1BoldRed);
                            else if (slot == "TOTAL")
                                cells[lineIndex, columnIndex].SetStyle(rowStyleTotal);
                            else if (slot == "MOYENNE") {
                                if (totalSeconds > mediaInformation.AverageDurationLimit && day != "SEMAINE")
                                    cells[lineIndex, columnIndex].SetStyle(rowStyleMoyenneRed);
                                else
                                    cells[lineIndex, columnIndex].SetStyle(rowStyleMoyenne);
                            }
                            else
                                cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        }
                        else {
                            if (totalSeconds > mediaInformation.DurationLimit && day != "SEMAINE" && slot != "TOTAL" && slot != "MOYENNE")
                                cells[lineIndex, columnIndex].SetStyle(rowStyle2BoldRed);
                            else if (slot == "TOTAL")
                                cells[lineIndex, columnIndex].SetStyle(rowStyleTotal);
                            else if (slot == "MOYENNE") {
                                if (totalSeconds > mediaInformation.AverageDurationLimit && day != "SEMAINE")
                                    cells[lineIndex, columnIndex].SetStyle(rowStyleMoyenneRed);
                                else
                                    cells[lineIndex, columnIndex].SetStyle(rowStyleMoyenne);
                            }
                            else
                                cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        }

                        if (mediaInformation.EncryptedMedia && slot != "TOTAL" && slot != "MOYENNE" && day != "SEMAINE") {
                            int verif = CheckCompleteSlot(day, slot, mediaInformation, ref completeSlot, ref uncommpleteSlot);

                            Style style = cells[lineIndex, columnIndex].GetStyle();

                            if (completeSlot) {
                                style.ForegroundColor = Color.FromArgb(196, 215, 155);
                                cells[lineIndex, columnIndex].SetStyle(style);
                            }
                            else if (uncommpleteSlot) {
                                style.ForegroundColor = Color.FromArgb(227, 236, 208);
                                cells[lineIndex, columnIndex].SetStyle(style);
                            }

                            completeSlot = false; uncommpleteSlot = false;
                        }

                        columnIndex++;
                    }

                    lineIndex++;

                }

                worksheet.AutoFitColumns();
                worksheet.AutoFitRows();
                #endregion

            }
            catch (System.Exception et) {
                throw (new AdVolumeCheckerRulesException("Error during the generation of the main worksheet (Method AddWorkSheet) : " + et.Message, et));
            }

            return lineIndex;
        }
        #endregion

        #region Add Detail Work Sheet
        private static void AddDetailWorkSheet(string connectionString, MediaInformation mediaInformations, Int64 mediaId, string mediaLabel, string daySlot, DateTime date, Workbook workbook, string versionList, string advertiserList, string productList, bool isIn) {

            try {
                string worksheetName = mediaLabel.Replace(" ", string.Empty).Replace("+", string.Empty) + "_" + daySlot.Replace(" ", string.Empty).Replace("-", "_");
                workbook.Worksheets.Add(worksheetName);
                Worksheet worksheet = workbook.Worksheets[worksheetName];
                //Get the cells collection in the sheet
                Cells cells = worksheet.Cells;
                int lineIndex = 0;
                int columnIndex = 0;
                string[] daySlotValues = daySlot.Split('_');
                string day = daySlotValues[0];
                string slot = daySlotValues[1];
                string previousSlot = GetPreviousSlot(slot);
                DateTime prviousDate;

                date = GetDate(date, day);
                if (previousSlot == "02H - 03H")
                    prviousDate = GetPreviousDate(date, day);
                else
                    prviousDate = date;

                DataSet dsPrevious = AdVolumeCheckerDAL.GetDetailData(connectionString, mediaId, prviousDate, previousSlot, versionList, advertiserList, productList, isIn);
                DataSet ds = AdVolumeCheckerDAL.GetDetailData(connectionString, mediaId, date, slot, versionList, advertiserList, productList, isIn);
                ds = CheckExceedingDuration(ds, dsPrevious);

                #region Set Styles
                //Set Header Style
                Aspose.Cells.Style headerStyle = cells["A1"].GetStyle();
                headerStyle.HorizontalAlignment = TextAlignmentType.Center;
                headerStyle.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                headerStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                headerStyle.Font.Color = Color.White;
                headerStyle.Font.IsBold = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 11;
                headerStyle.ForegroundColor = Color.FromArgb(79, 129, 189);
                headerStyle.Pattern = BackgroundType.Solid;

                //Set Row Style
                Aspose.Cells.Style rowStyle1 = cells["A1"].GetStyle();
                rowStyle1.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle1.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle1.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle1.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                rowStyle1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                rowStyle1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                rowStyle1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                rowStyle1.ForegroundColor = Color.FromArgb(220, 230, 241);
                rowStyle1.Font.Name = "Calibri";
                rowStyle1.Font.Size = 10;
                rowStyle1.Pattern = BackgroundType.Solid;

                Aspose.Cells.Style rowStyle2 = cells["A1"].GetStyle();
                rowStyle2.Borders[BorderType.TopBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle2.Borders[BorderType.BottomBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle2.Borders[BorderType.LeftBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle2.Borders[BorderType.RightBorder].Color = Color.FromArgb(149, 179, 215);
                rowStyle2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                rowStyle2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                rowStyle2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                rowStyle2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                rowStyle2.Font.Name = "Calibri";
                rowStyle2.Font.Size = 10;
                rowStyle2.Pattern = BackgroundType.Solid;
                #endregion

                lineIndex += 4;

                #region Init Header
                cells[lineIndex, columnIndex].PutValue("Top de diffusion");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Spot");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Date de diffusion");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Annonceur");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Agence média");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Support");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Produit");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Famille");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Groupe");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Durée");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Code écran");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Position");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Durée écran");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;

                cells[lineIndex, columnIndex].PutValue("Nb spots écran");
                cells[lineIndex, columnIndex].SetStyle(headerStyle);
                columnIndex++;
                lineIndex++;
                #endregion

                #region Init Rows
                foreach (DataRow row in ds.Tables[0].Rows) {

                    TopDiffusionManagement topDiffusion = new TopDiffusionManagement(int.Parse(row["TOP_DIFFUSION"].ToString()), int.Parse(row["duration"].ToString()), day, slot);
                    int duration = Convert.ToInt32(row["duration"].ToString());

                    //if (!mediaInformations.EncryptedMedia || (mediaInformations.EncryptedMedia && mediaInformations.CheckTopDiffusionInUnencryptedSlot(day, topDiffusion.TopDiffusionTimeSpan, ref duration))) {

                        columnIndex = 0;

                        cells[lineIndex, columnIndex].PutValue(GetTopDiffuionFormat(row["top_diffusion"].ToString()));
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(Int64.Parse(row["id_slogan"].ToString()));
                        worksheet.Hyperlinks.Add(lineIndex, columnIndex, 1, 1, "http://www.tnsadexpress.com/Public/CreativeView.aspx?creation=" + row["id_slogan"].ToString());

                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(GetDateFormat(row["date_media_num"].ToString()));
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(row["advertiser"].ToString());
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(row["advertising_agency"].ToString());
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(row["media"].ToString());
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(row["product"].ToString());
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(row["sector"].ToString());
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(row["group_"].ToString());
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        //cells[lineIndex, columnIndex].PutValue(GetDurationFormat(duration.ToString()));
                        cells[lineIndex, columnIndex].PutValue(GetDurationFormat(row["duration"].ToString()));
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(Int64.Parse(row["id_commercial_break"].ToString()));
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(Int64.Parse(row["id_rank"].ToString()));
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(GetDurationFormat(row["duration_commercial_break"].ToString()));
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        cells[lineIndex, columnIndex].PutValue(Int64.Parse(row["number_message_commercial_brea"].ToString()));
                        if (lineIndex % 2 == 0)
                            cells[lineIndex, columnIndex].SetStyle(rowStyle1);
                        else
                            cells[lineIndex, columnIndex].SetStyle(rowStyle2);
                        columnIndex++;

                        lineIndex++;

                    //}

                }

                worksheet.AutoFitColumns();
                worksheet.AutoFitRows();
                #endregion

                worksheet.Pictures.Add(0, 0, AppDomain.CurrentDomain.BaseDirectory + "\\Images\\logo_Tns.gif");

            }
            catch (System.Exception et) {
                throw (new AdVolumeCheckerRulesException("Error during the generation of a detail worksheet (Method AddDetailWorkSheet) : " + et.Message, et));
            }

        }
        #endregion

        #region Get Previous Slot
        /// <summary>
        /// Get Previous Slot
        /// </summary>
        /// <returns>Previous Slot</returns>
        private static string GetPreviousSlot(string slot) {

            switch (slot) {
                case "03H - 04H": return "02H - 03H";
                case "04H - 05H": return "03H - 04H";
                case "05H - 06H": return "04H - 05H";
                case "06H - 07H": return "05H - 06H";
                case "07H - 08H": return "06H - 07H";
                case "08H - 09H": return "07H - 08H";
                case "09H - 10H": return "08H - 09H";
                case "10H - 11H": return "09H - 10H";
                case "11H - 12H": return "10H - 11H";
                case "12H - 13H": return "11H - 12H";
                case "13H - 14H": return "12H - 13H";
                case "14H - 15H": return "13H - 14H";
                case "15H - 16H": return "14H - 15H";
                case "16H - 17H": return "15H - 16H";
                case "17H - 18H": return "16H - 17H";
                case "18H - 19H": return "17H - 18H";
                case "19H - 20H": return "18H - 19H";
                case "20H - 21H": return "19H - 20H";
                case "21H - 22H": return "20H - 21H";
                case "22H - 23H": return "21H - 22H";
                case "23H - 24H": return "22H - 23H";
                case "24H - 01H": return "23H - 24H";
                case "01H - 02H": return "24H - 01H";
                case "02H - 03H": return "01H - 02H";
            }

            return "";
        }
        #endregion

        #region Get Date
        /// <summary>
        /// Get Date
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="day">Day</param>
        /// <returns></returns>
        private static DateTime GetDate(DateTime date, string day) {

            switch (day) { 
                case "LUNDI"   : return date;
                case "MARDI"   : return date.AddDays(1);
                case "MERCREDI": return date.AddDays(2);
                case "JEUDI"   : return date.AddDays(3);
                case "VENDREDI": return date.AddDays(4);
                case "SAMEDI"  : return date.AddDays(5);
                case "DIMANCHE": return date.AddDays(6);
                default: return date;
            }

        }
        #endregion

        #region Get Previous Date
        /// <summary>
        /// Get Previous Date
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="day">Day</param>
        /// <returns></returns>
        private static DateTime GetPreviousDate(DateTime date, string day) {

            switch (day) {
                case "LUNDI": return date.AddDays(-1);
                case "MARDI": return date;
                case "MERCREDI": return date.AddDays(1);
                case "JEUDI": return date.AddDays(2);
                case "VENDREDI": return date.AddDays(3);
                case "SAMEDI": return date.AddDays(4);
                case "DIMANCHE": return date.AddDays(5);
                default: return date;
            }

        }
        #endregion

        #region Get Date Format
        /// <summary>
        /// Get Date Format
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>Date format dd/MM/yyyy</returns>
        private static string GetDateFormat(string date) {
            return date.Substring(6, 2) + "/" + date.Substring(4, 2) + "/" + date.Substring(0, 4);
        }
        #endregion

        #region Get Duration Format
        private static string GetDurationFormat(string duration) {

            int totalSeconds = int.Parse(duration);
            int seconds = totalSeconds % 60;
            int minutes = totalSeconds / 60;
            string time = minutes + " min " + seconds + " sec";

            return time;
        }
        #endregion

        #region Get Top Diffusion Format
        private static string GetTopDiffuionFormat(string topDiffusion) {
            
            int length = topDiffusion.Length;

            for (int i = 0; i < 6 - length; i++)
                topDiffusion = "0" + topDiffusion;

            return topDiffusion.Substring(0, 2) + " H " + topDiffusion.Substring(2, 2) + " M " + topDiffusion.Substring(4, 2) + " S ";

        }
        #endregion

        #region Get Total result Number
        private static int GetTotalResultNumbr(List<MediaInformation> mediaInformations) {

            int index = 0;

            foreach (MediaInformation mediaInformation in mediaInformations) {
                foreach (string daySlot in mediaInformation.SpotDetails) {
                    index++;
                }
            }

            if (index == 0)
                return 1;
            else
                return index;

        }
        #endregion

        #region Check for exceeding duration
        /// <summary>
        /// Check for exceeding duration
        /// </summary>
        /// <param name="ds">Data Set</param>
        private static DataSet CheckExceedingDuration(DataSet ds, DataSet dsPrevious) {

            int topDiffusion = 0;
            TimeSpan upperBound = new TimeSpan(0,0,0);
            string day = string.Empty;
            string slot = string.Empty;
            Int64 duration;
            DataRow dr;
            TopDiffusionManagement topDiffusionObj;

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {

                topDiffusion = int.Parse(ds.Tables[0].Rows[ds.Tables[0].Rows.Count-1]["top_diffusion"].ToString());
                int hours = int.Parse(topDiffusion.ToString("000000").Substring(0, 2));

                dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
                day = dr["JOUR"].ToString().TrimEnd();
                slot = dr["TRANCHE_HORAIRE"].ToString();
                topDiffusionObj = new TopDiffusionManagement(int.Parse(dr["TOP_DIFFUSION"].ToString()), int.Parse(dr["duration"].ToString()), day, slot);
                duration = topDiffusionObj.GetDuration();

                if (topDiffusionObj.Exceeding) {

                    ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["duration"] = duration;

                    #region duration_commercial_break
                    /*int durationCommercialBreak = int.Parse(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["duration_commercial_break"].ToString()) - (result - upperBound).Seconds;
                    ds.Tables[0].Rows[ds.Tables[0].Rows.Count-1]["duration_commercial_break"] = durationCommercialBreak;
                    int idCommercialBreak = int.Parse(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["id_commercial_break"].ToString());

                    int i = 0;
                    foreach (DataRow row in ds.Tables[0].Rows) {
                        if (int.Parse(row["id_commercial_break"].ToString()) == idCommercialBreak)
                            ds.Tables[0].Rows[i]["duration_commercial_break"] = durationCommercialBreak;
                        i++;
                    }*/
                    #endregion
                }

                if (dsPrevious != null && dsPrevious.Tables.Count > 0 && dsPrevious.Tables[0] != null && dsPrevious.Tables[0].Rows.Count > 0) {

                    dr = dsPrevious.Tables[0].Rows[dsPrevious.Tables[0].Rows.Count - 1];
                    day = dr["JOUR"].ToString().TrimEnd();
                    slot = dr["TRANCHE_HORAIRE"].ToString();
                    topDiffusionObj = new TopDiffusionManagement(int.Parse(dr["TOP_DIFFUSION"].ToString()), int.Parse(dr["duration"].ToString()), day, slot);
                    duration = topDiffusionObj.GetDuration();

                    if (topDiffusionObj.Exceeding) {
                        dsPrevious.Tables[0].Rows[dsPrevious.Tables[0].Rows.Count - 1]["duration"] = topDiffusionObj.GetExceedingDuration();
                        dsPrevious.Tables[0].Rows[dsPrevious.Tables[0].Rows.Count - 1]["TOP_DIFFUSION"] = hours.ToString() + "0000";
                        DataRow destRow = ds.Tables[0].NewRow();
                        destRow.ItemArray = dr.ItemArray.Clone() as object[];
                        ds.Tables[0].Rows.InsertAt(destRow, 0);
                    }
                }

            }

            return ds;
        }
        #endregion

        #region Get Total Unencrypted Time
        /// <summary>
        /// Get Total Unencrypted Time
        /// </summary>
        /// <param name="mediaInformation">Media Information</param>
        /// <param name="day">Day</param>
        /// <returns>Total Unencrypted Time</returns>
        private static double GetTotalUnencryptedTime(MediaInformation mediaInformation, string day) {

            double totalTime = 0;

            foreach (UnencryptedSlot slot in mediaInformation.UnencryptedSlots[day]) {

                TimeSpan start = new TimeSpan(slot.SlotStart.Hour, slot.SlotStart.Minute, slot.SlotStart.Second);
                TimeSpan end = new TimeSpan(slot.SlotEnd.Hour, slot.SlotEnd.Minute, slot.SlotEnd.Second);
                
                totalTime += (end - start).TotalHours;

            }

            return totalTime;

        }
        #endregion

        #region Get Average Total Unencrypted Time
        /// <summary>
        /// Get Average Total Unencrypted Time
        /// </summary>
        /// <param name="mediaInformation">Media Information</param>
        /// <param name="day">Day</param>
        /// <returns>Total Unencrypted Time</returns>
        private static double GetAverageTotalUnencryptedTime(MediaInformation mediaInformation) {

            double totalTime = 0;
            List<string> daysOfWeek = new List<string>(new string[] { "LUNDI", "MARDI", "MERCREDI", "JEUDI", "VENDREDI", "SAMEDI", "DIMANCHE" });

            foreach (string day in daysOfWeek) {
                foreach (UnencryptedSlot slot in mediaInformation.UnencryptedSlots[day]) {

                    TimeSpan start = new TimeSpan(slot.SlotStart.Hour, slot.SlotStart.Minute, slot.SlotStart.Second);
                    TimeSpan end = new TimeSpan(slot.SlotEnd.Hour, slot.SlotEnd.Minute, slot.SlotEnd.Second);

                    totalTime += (end - start).TotalHours;

                }
            }

            return totalTime / 7;

        }
        #endregion

        #region Check Complete Slot
        private static int CheckCompleteSlot(string day, string slot, MediaInformation mediaInformation, ref bool completeSlot, ref bool uncommpleteSlot) {

            TimeSpan slotStart = GetTopDiffusionTimeSpan(slot);
            TimeSpan slotDuration = new TimeSpan(0, 59, 59);
            TimeSpan slotEnd = slotStart + slotDuration;


            foreach (UnencryptedSlot unencryptedSlot in mediaInformation.UnencryptedSlots[day]) {

                TimeSpan start = new TimeSpan(unencryptedSlot.SlotStart.Hour, unencryptedSlot.SlotStart.Minute, unencryptedSlot.SlotStart.Second);
                TimeSpan end = new TimeSpan(unencryptedSlot.SlotEnd.Hour, unencryptedSlot.SlotEnd.Minute, unencryptedSlot.SlotEnd.Second);

                if (slotStart >= start && slotEnd <= end) {
                    completeSlot = true;
                    uncommpleteSlot = false;
                    return 1;
                }
                else if ((start > slotStart && start < slotEnd) || (end > slotStart && end < slotEnd)) {
                    completeSlot = false;
                    uncommpleteSlot = true;
                    return 1;
                }

            }

            completeSlot = false;
            uncommpleteSlot = false;

            return 0;
        }
        #endregion

        #region Get Top Diffusion Time Span
        /// <summary>
        /// Get Top Diffusion Time Span
        /// </summary>
        /// <param name="slot">Slot</param>
        /// <returns>Top Diffusion time Span</returns>
        private static TimeSpan GetTopDiffusionTimeSpan(string slot) {

            switch (slot) {
                case "03H - 04H": return new TimeSpan(3, 0, 0);
                case "04H - 05H": return new TimeSpan(4, 0, 0);
                case "05H - 06H": return new TimeSpan(5, 0, 0);
                case "06H - 07H": return new TimeSpan(6, 0, 0);
                case "07H - 08H": return new TimeSpan(7, 0, 0);
                case "08H - 09H": return new TimeSpan(8, 0, 0);
                case "09H - 10H": return new TimeSpan(9, 0, 0);
                case "10H - 11H": return new TimeSpan(10, 0, 0);
                case "11H - 12H": return new TimeSpan(11, 0, 0);
                case "12H - 13H": return new TimeSpan(12, 0, 0);
                case "13H - 14H": return new TimeSpan(13, 0, 0);
                case "14H - 15H": return new TimeSpan(14, 0, 0);
                case "15H - 16H": return new TimeSpan(15, 0, 0);
                case "16H - 17H": return new TimeSpan(16, 0, 0);
                case "17H - 18H": return new TimeSpan(17, 0, 0);
                case "18H - 19H": return new TimeSpan(18, 0, 0);
                case "19H - 20H": return new TimeSpan(19, 0, 0);
                case "20H - 21H": return new TimeSpan(20, 0, 0);
                case "21H - 22H": return new TimeSpan(21, 0, 0);
                case "22H - 23H": return new TimeSpan(22, 0, 0);
                case "23H - 24H": return new TimeSpan(23, 0, 0);
                case "24H - 01H": return new TimeSpan(24, 0, 0);
                case "01H - 02H": return new TimeSpan(1, 0, 0);
                case "02H - 03H": return new TimeSpan(2, 0, 0);
                default: return new TimeSpan(0, 0, 0);
            }

        }
        #endregion

    }
}
