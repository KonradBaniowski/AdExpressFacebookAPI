using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Xml;
using TNS.AdExpress.Domain.Theme;
using System.Drawing;



namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Load style object  excel and pdf
    /// </summary>
    public class ThemeXL {
        /// <summary>
        /// Load Anubis Style configuration
        /// </summary>
        /// <param name="dataSrc">Data Source</param>
        /// <param name="styles">Styles Configuration</param>
        internal static void Load(IDataSource dataSrc, Dictionary<string, Style> style, Dictionary<string, Tag> tagList) {

            #region variables
            XmlTextReader Reader;
            string label;
            string value;
            XmlReader ReaderTemp;

            #region Attributs Font
            string policeName;
            Color color;
            double size;
            bool isItalic;
            bool isUnderline;
            bool isBold;
            bool isStrikeout;
            #endregion

            #region Attributs Cell
            Theme.Font font = new Theme.Font();
            Color foregroundColor;
            Borders borders = new Borders();
            Border border;
            #endregion

            #region Attributs Picture
            string path;
            double height;
            double width;
            #endregion

            #region Attribut layout
            Margin margin;
            double top;
            double right;
            double bottom;
            double left;
            #endregion

            #region Line
            Line.BordersStyle borderStyle = Line.BordersStyle.NotSet;
            #endregion

            #endregion

            try {
                Reader = (XmlTextReader)dataSrc.GetSource();
                // Parse XML file
                while (Reader.Read()) {
                    if (Reader.NodeType == XmlNodeType.Element) {
                        switch (Reader.LocalName) {

                            #region Font
                            case "font":
                                if (Reader.GetAttribute("policeName") != null && Reader.GetAttribute("policeName").Length > 0
                                    && Reader.GetAttribute("color") != null && Reader.GetAttribute("color").Length > 0
                                    && Reader.GetAttribute("size") != null && Reader.GetAttribute("size").Length > 0) {

                                    policeName = Reader.GetAttribute("policeName");
                                    color = Color.FromArgb(int.Parse(Reader.GetAttribute("color").Split(",".ToCharArray())[0]), int.Parse(Reader.GetAttribute("color").Split(",".ToCharArray())[1]), int.Parse(Reader.GetAttribute("color").Split(",".ToCharArray())[2]));
                                    size = double.Parse(Reader.GetAttribute("size"));

                                    if (Reader.GetAttribute("isItalic") != null && Reader.GetAttribute("isItalic").Length > 0)
                                        isItalic = bool.Parse(Reader.GetAttribute("isItalic"));
                                    else
                                        isItalic = false;
                                    if (Reader.GetAttribute("isUnderline") != null && Reader.GetAttribute("isUnderline").Length > 0)
                                        isUnderline = bool.Parse(Reader.GetAttribute("isUnderline"));
                                    else
                                        isUnderline = false;
                                    if (Reader.GetAttribute("isBold") != null && Reader.GetAttribute("isBold").Length > 0)
                                        isBold = bool.Parse(Reader.GetAttribute("isBold"));
                                    else
                                        isBold = false;
                                    if (Reader.GetAttribute("isStrikeout") != null && Reader.GetAttribute("isStrikeout").Length > 0)
                                        isStrikeout = bool.Parse(Reader.GetAttribute("isStrikeout"));
                                    else
                                        isStrikeout = false;

                                    if (tagList.ContainsKey(Reader.GetAttribute("name")))
                                        tagList[Reader.GetAttribute("name")] = new Theme.Font(policeName, color, size, isBold, isItalic, isUnderline, isStrikeout);
                                    else
                                        tagList.Add(Reader.GetAttribute("name"), new Theme.Font(policeName, color, size, isBold, isItalic, isUnderline, isStrikeout));
                                }
                                else {
                                    throw new Exception("Font Format Invalid in style xml file !");
                                }
                                break;
                            #endregion

                            #region cell
                            case "cell":
                                if (borders == null)
                                    borders = new Borders();

                                label = Reader.GetAttribute("name");

                                if (Reader.GetAttribute("fontName") != null && Reader.GetAttribute("fontName").Length > 0 && tagList.ContainsKey(Reader.GetAttribute("fontName")))
                                    if (tagList[Reader.GetAttribute("fontName")].GetType() == typeof(Theme.Font))
                                        font = (Theme.Font)tagList[Reader.GetAttribute("fontName")];
                                    else
                                        font = new Theme.Font();

                                if (Reader.GetAttribute("foregroundColor") != null && Reader.GetAttribute("foregroundColor").Length > 0)
                                    foregroundColor = Color.FromArgb(int.Parse(Reader.GetAttribute("foregroundColor").Split(",".ToCharArray())[0]), int.Parse(Reader.GetAttribute("foregroundColor").Split(",".ToCharArray())[1]), int.Parse(Reader.GetAttribute("foregroundColor").Split(",".ToCharArray())[2]));
                                else
                                    foregroundColor = Color.White;

                                //borders = Reader.GetAttribute("borders");
                                try {
                                    ReaderTemp = Reader.ReadSubtree();
                                    while (ReaderTemp.Read()) {
                                        if (ReaderTemp.NodeType == XmlNodeType.Element) {
                                            switch (ReaderTemp.LocalName) {
                                                case "border":
                                                    border = new Border();
                                                    border.Color = Color.FromArgb(int.Parse(ReaderTemp.GetAttribute("color").Split(",".ToCharArray())[0]), int.Parse(ReaderTemp.GetAttribute("color").Split(",".ToCharArray())[1]), int.Parse(ReaderTemp.GetAttribute("color").Split(",".ToCharArray())[2]));
                                                    border.LineStyle = ((Border.CellBorderType)Enum.Parse(typeof(Border.CellBorderType), ReaderTemp.GetAttribute("lineStyle")));
                                                    borders.Border.Add(((Borders.BorderType)Enum.Parse(typeof(Borders.BorderType), ReaderTemp.GetAttribute("type"))), border);
                                                    break;
                                            }
                                        }
                                    }
                                    if (tagList.ContainsKey(label))
                                        tagList[label] = new Cell(font, foregroundColor, borders);
                                    else
                                        tagList.Add(label, new Cell(font, foregroundColor, borders));

                                    borders = null;
                                }
                                catch {
                                    if (tagList.ContainsKey(label))
                                        tagList[label] = new Cell(font, foregroundColor);
                                    else
                                        tagList.Add(label, new Cell(font, foregroundColor));
                                }
                                break;
                            #endregion

                            #region Picture
                            case "picture":
                                if (Reader.GetAttribute("path") != null && Reader.GetAttribute("path").Length > 0) {
                                    path = Reader.GetAttribute("path");
                                    if (Reader.GetAttribute("width") != null && Reader.GetAttribute("width").Length > 0
                                        && Reader.GetAttribute("height") != null && Reader.GetAttribute("height").Length > 0) {
                                        width = double.Parse(Reader.GetAttribute("width"));
                                        height = double.Parse(Reader.GetAttribute("height"));


                                        if (tagList.ContainsKey(Reader.GetAttribute("name")))
                                            tagList[Reader.GetAttribute("name")] = new Picture(path, height, width);
                                        else
                                            tagList.Add(Reader.GetAttribute("name"), new Picture(path, height, width));
                                    }
                                    else {
                                        if (tagList.ContainsKey(Reader.GetAttribute("name")))
                                            tagList[Reader.GetAttribute("name")] = new Picture(path);
                                        else
                                            tagList.Add(Reader.GetAttribute("name"), new Picture(path));
                                    }

                                }
                                else {
                                    throw new Exception("Path of Picture is not specified in Theme xml File !");
                                }
                                break;
                            #endregion

                            #region Box
                            case "box":
                                if (Reader.GetAttribute("width") != null && Reader.GetAttribute("width").Length > 0)
                                    width = double.Parse(Reader.GetAttribute("width"));
                                else
                                    width = 0;
                                if (Reader.GetAttribute("height") != null && Reader.GetAttribute("height").Length > 0)
                                    height = double.Parse(Reader.GetAttribute("height"));
                                else
                                    height = 0;

                                if (Reader.GetAttribute("marginTop") != null && Reader.GetAttribute("marginTop").Length > 0)
                                    top = double.Parse(Reader.GetAttribute("marginTop"));
                                else
                                    top = 0;
                                if (Reader.GetAttribute("marginRight") != null && Reader.GetAttribute("marginRight").Length > 0)
                                    right = double.Parse(Reader.GetAttribute("marginRight"));
                                else
                                    right = 0;
                                if (Reader.GetAttribute("marginBottom") != null && Reader.GetAttribute("marginBottom").Length > 0)
                                    bottom = double.Parse(Reader.GetAttribute("marginBottom"));
                                else
                                    bottom = 0;
                                if (Reader.GetAttribute("marginLeft") != null && Reader.GetAttribute("marginLeft").Length > 0)
                                    left = double.Parse(Reader.GetAttribute("marginLeft"));
                                else
                                    left = 0;

                                margin = new Margin(top, right, bottom, left);

                                if (tagList.ContainsKey(Reader.GetAttribute("name")))
                                    tagList[Reader.GetAttribute("name")] = new Box(margin, height, width);
                                else
                                    tagList.Add(Reader.GetAttribute("name"), new Box(margin, height, width));
                                break;
                            #endregion

                            #region colorList
                            case "colorList":
                                label = Reader.GetAttribute("name");

                                ReaderTemp = Reader.ReadSubtree();
                                List<Color> pieListTemp = new List<Color>();
                                while (ReaderTemp.Read()) {
                                    if (ReaderTemp.NodeType == XmlNodeType.Element) {
                                        switch (ReaderTemp.LocalName) {
                                            case "color":
                                                value = ReaderTemp.ReadString();
                                                pieListTemp.Add(Color.FromArgb(int.Parse(value.Split(",".ToCharArray())[0]), int.Parse(value.Split(",".ToCharArray())[1]), int.Parse(value.Split(",".ToCharArray())[2])));
                                                break;
                                        }
                                    }
                                }
                                if (tagList.ContainsKey(label))
                                    tagList[label] = new Colors(pieListTemp);
                                else
                                    tagList.Add(label, new Colors(pieListTemp));
                                break;
                            #endregion

                            #region line
                            case "line":
                                if (Reader.GetAttribute("borderStyle") != null && Reader.GetAttribute("borderStyle").Length > 0
                                    && Reader.GetAttribute("color") != null && Reader.GetAttribute("color").Length > 0
                                    && Reader.GetAttribute("size") != null && Reader.GetAttribute("size").Length > 0) {

                                    borderStyle = ((Line.BordersStyle)Enum.Parse(typeof(Line.BordersStyle), Reader.GetAttribute("borderStyle")));
                                    color = Color.FromArgb(int.Parse(Reader.GetAttribute("color").Split(",".ToCharArray())[0]), int.Parse(Reader.GetAttribute("color").Split(",".ToCharArray())[1]), int.Parse(Reader.GetAttribute("color").Split(",".ToCharArray())[2]));
                                    size = double.Parse(Reader.GetAttribute("size"));

                                    if (tagList.ContainsKey(Reader.GetAttribute("name")))
                                        tagList[Reader.GetAttribute("name")] = new Theme.Line(size, color, borderStyle);
                                    else
                                        tagList.Add(Reader.GetAttribute("name"), new Theme.Line(size, color, borderStyle));
                                }
                                else {
                                    throw new Exception("Line Format Invalid in style xml file !");
                                }
                                break;
                            #endregion

                            #region Style
                            case "style":
                                label = Reader.GetAttribute("name");
                                ReaderTemp = Reader.ReadSubtree();
                                Dictionary<string, Tag> tagListTemp = new Dictionary<string, Tag>();
                                string labelTag = string.Empty;
                                while (ReaderTemp.Read()) {
                                    if (ReaderTemp.NodeType == XmlNodeType.Element) {
                                        switch (ReaderTemp.LocalName) {
                                            case "tag":
                                                labelTag = ReaderTemp.GetAttribute("name");
                                                value = ReaderTemp.ReadString();
                                                if (tagList.ContainsKey(value)) {
                                                    if (tagListTemp.ContainsKey(labelTag))
                                                        tagListTemp[labelTag] = tagList[value];
                                                    else
                                                        tagListTemp.Add(labelTag, tagList[value]);
                                                }
                                                else {
                                                    throw new Exception("Le tag '" + value + "' n'est pas référencé dans le fichier xml des themes!");
                                                }
                                                break;
                                        }
                                    }
                                }
                                if (style.ContainsKey(label))
                                    style[label] = new Style(tagListTemp);
                                else
                                    style.Add(label, new Style(tagListTemp));

                                tagListTemp = null;
                                break;
                            #endregion
                        }
                    }
                }
                Reader.Close();
            }
            catch (System.Exception err) {
                //throw (new AppmConfigDataAccessException("Unable to load Appm configuration", err));
            }
        }
    }
}
