using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// 
    /// </summary>
    public abstract class Tag {
        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="cells">Excel Object</param>
        public abstract void SetStyleExcel(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column);
        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        public abstract void SetStyleExcel(Aspose.Cells.Worksheet sheet);
        /// <summary>
        /// Set a Style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        /// <param name="upperLeftRow">Upper Left Row</param>
        /// <param name="upperLeftColumn">Upper Left Column</param>
        public abstract void SetStyleExcel(Aspose.Cells.Worksheet sheet, int upperLeftRow, int upperLeftColumn);
        /// <summary>
        /// Apllied a style to PDF Object
        /// </summary>
        /// <param name="pdfObject">PDF Object</param>
        /// <param name="charset">Charset</param>
        public abstract void SetStylePdf(PDFCreatorPilotLib.PDFDocument3Class pdfObject, PDFCreatorPilotLib.TxFontCharset charset);
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="legend">legend To Init</param>
        public abstract void SetStyleDundas(Dundas.Charting.WinControl.Legend legend);
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="title">title</param>
        public abstract void SetStyleDundas(Dundas.Charting.WinControl.Title title);
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="label">label</param>
        public abstract void SetStyleDundas(Dundas.Charting.WinControl.Label label);
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="series">series</param>
        public abstract void SetStyleDundas(Dundas.Charting.WinControl.Series series);
        /// <summary>
        /// Apllied a style Line To an Object chart
        /// </summary>
        /// <param name="chart">Chart</param>
        public abstract void SetStyleDundas(Dundas.Charting.WinControl.Chart chart);
        /// <summary>
        /// Apllied a style Font To an Object Font
        /// </summary>
        /// <param name="axis">Title axis</param>
        public abstract void SetStyleDundas(Dundas.Charting.WinControl.Axis axis);
        /// <summary>
        /// Apllied a style Color To an Object Color
        /// </summary>
        /// <param name="axis">Title axis</param>
        public abstract void SetStyleDundas(ref Color color);
    }
}
