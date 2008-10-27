using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;
using System.Drawing;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// Style Object contains a list of Tag
    /// </summary>
    public class Theme {

        #region Variables
        /// <summary>
        /// List of Style
        /// </summary>
        private Dictionary<string, Style> _styleList = null;
        /// <summary>
        /// List Tag Global
        /// </summary>
        private Dictionary<string, Tag> _tagList = null;
        #endregion

        #region Assessor
        /*/// <summary>
        /// Get / Set List of Style
        /// </summary>
        public Dictionary<string, Style> StyleList {
            get { return _styleList; }
            set { _styleList = value; }
        }*/
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public Theme(IDataSource dataSource) {
            Init(dataSource);
        }
        #endregion

        #region Methods

        #region Private Methods
        /// <summary>
        /// Init The List of Style
        /// </summary>
        /// <param name="dataSource">dataSource XML</param>
        private void Init(IDataSource dataSource) {
            _styleList = new Dictionary<string, Style>();
            _tagList = new Dictionary<string, Tag>();
            UpdateList(dataSource);
        }

        /// <summary>
        /// Get all color defined in tag List Global
        /// </summary>
        /// <returns>Color List Defined in tag List Global</returns>
        private List<Color> getColorList() {
            List<Color> colorList = new List<Color>();
            foreach (Tag tag in _tagList.Values) {

                if (tag.GetType() == typeof(Font)) {
                    if (!colorList.Contains(((Font)tag).Color))
                        colorList.Add(((Font)tag).Color);
                }
                else if (tag.GetType() == typeof(Cell)) {
                    if (!colorList.Contains(((Cell)tag).ForegroundColor))
                        colorList.Add(((Cell)tag).ForegroundColor);
                    foreach (Border border in ((Cell)tag).Borders.Border.Values)
                        if (!colorList.Contains(border.Color))
                            colorList.Add(border.Color);
                }


            }

            return colorList;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get the Style
        /// </summary>
        /// <param name="styleName">Style Name</param>
        /// <returns>Style</returns>
        public Style GetStyle(string styleName) {
            if(_styleList != null && _styleList.ContainsKey(styleName))
                return _styleList[styleName];
            else
                throw new Exception("Style Name '" + styleName + "' is invalid ! (GetStyle() in class Theme)");
        }

        /// <summary>
        /// Update The list of Style
        /// </summary>
        /// <param name="dataSource">Datasource Xml</param>
        public void UpdateList(IDataSource dataSource) {
            ThemeXL.Load(dataSource, _styleList, _tagList);
        }

        /// <summary>
        /// Init the palette whith all color defined to xml theme file which use Aspose.cells
        /// </summary>
        /// <param name="excel">Object Excel to Init</param>
        /// <param name="styleName">Style Name</param>
        public void InitExcelColorPalette(Aspose.Cells.Workbook excel) {
            List<Color> color = getColorList();
            if (color.Count > 0) {
                for (int i = 0; i < color.Count && i < excel.Colors.Length; i++) {
                    excel.ChangePalette(color[i], i);
                }
            }
        }
        #endregion

        #endregion

        

    }
}
