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
    public class Style {

        #region Variables
        /// <summary>
        /// List of property name which content skin (name of fontPdf, name of  picture, name of styleCell...)
        /// </summary>
        private Dictionary<string,Tag> _tagList = new Dictionary<string,Tag>();
        #endregion

        #region Assessor
        /*/// <summary>
        /// Get List of property name which content skin (name of fontPdf, name of  picture, name of styleCell...)
        /// </summary>
        public Dictionary<string, Tag> TagList {
            get { return _tagList; }
        }*/
        #endregion

        #region Constructor
        public Style(Dictionary<string, Tag> tagList) {
            if (tagList == null) throw new ArgumentNullException("Tag List Parameter is null ! (in Construtor of Style)");
            _tagList = tagList;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Tag
        /// </summary>
        /// <param name="tagName">Tag Name</param>
        /// <returns>Tag</returns>
        public Tag GetTag(string tagName) {
            if (_tagList != null && _tagList.ContainsKey(tagName))
                return _tagList[tagName];
            else
                throw new Exception("Tag Name '" + tagName + "' is invalid ! (GetTag() in class Style)");
        }
        /// <summary>
        /// Init the palette whith all color defined to xml theme file for this style which use Aspose.cells
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

        #region Private Methods
        /// <summary>
        /// Get all color defined in tag List of style
        /// </summary>
        /// <returns>Color List Defined in tag List of style</returns>
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

    }
}
