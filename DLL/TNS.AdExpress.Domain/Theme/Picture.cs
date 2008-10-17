using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// Define an object picture
    /// </summary>
    public class Picture : Tag {

        #region Variables
        /// <summary>
        /// Path of picture
        /// </summary>
        private string _path = string.Empty;
        /// <summary>
        /// Height of picture
        /// </summary>
        private double _height = -1;
        /// <summary>
        /// Width of picture
        /// </summary>
        private double _width = -1;
        #endregion

        #region Assessor
        /// <summary>
        /// Get Path of picture
        /// </summary>
        public string Path { get { return _path; } }
        /// <summary>
        /// Get Height of picture
        /// </summary>
        public double Height { get { return _height; } }
        /// <summary>
        /// get Width of picture
        /// </summary>
        public double Width { get { return _width; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"> Path of picture</param>
        /// <param name="height">Height of picture</param>
        /// <param name="width">Width of picture</param>
        public Picture(string path, double height, double width)
            : this(path) {
            _height = height;
            _width = width;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"> Path of picture</param>
        public Picture(string path) {
            if (path == null) throw new ArgumentNullException("Path parameter is null !");
            if (path.Length<1) throw new ArgumentException("Path parameter is invalid !");
            _path = path;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="cells">Excel Object</param>
        public override void SetStyle(Aspose.Cells.Workbook excel, Aspose.Cells.Cells cells, int row, int column) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Apllied a style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        public override void SetStyle(Aspose.Cells.Worksheet sheet) {
            
        }
        /// <summary>
        /// Set a Style to Excel Object
        /// </summary>
        /// <param name="sheet">Sheet Excel Object</param>
        /// <param name="upperLeftRow">Upper Left Row</param>
        /// <param name="upperLeftColumn">Upper Left Column</param>
        public override void SetStyle(Aspose.Cells.Worksheet sheet, int upperLeftRow, int upperLeftColumn) {
            string logoPath = System.IO.Path.GetFullPath(this._path);
            int indexPicture = sheet.Pictures.Add(upperLeftRow, upperLeftColumn, this._path, Convert.ToInt32(this._width), Convert.ToInt32(this._height));
            sheet.Pictures[indexPicture].Placement = Aspose.Cells.PlacementType.Move;
        }

        /*/// <summary>
        /// Apllied a style to PDF Object
        /// </summary>
        /// <param name="pdfObject">PDF Object</param>
        public override void SetStyle(PDFCreatorPilotLib.PDFDocument3Class pdfObject) {
            throw new Exception("The method or operation is not implemented.");
        }*/
        #endregion
    }
}
