using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// 
    /// </summary>
    public abstract class Tag {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cells"></param>
        public abstract void SetStyle(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        public abstract void SetStyle(Aspose.Cells.Worksheet sheet);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="upperLeftRow"></param>
        /// <param name="upperLeftColumn"></param>
        public abstract void SetStyle(Aspose.Cells.Worksheet sheet, int upperLeftRow, int upperLeftColumn);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdfObject"></param>
        //public abstract void SetStyle(PDFCreatorPilotLib.PDFDocument3Class pdfObject);

        
    }
}
