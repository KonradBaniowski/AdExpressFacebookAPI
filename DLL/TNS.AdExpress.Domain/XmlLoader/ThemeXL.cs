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
        internal static void Load(IDataSource dataSrc, Theme.Theme theme, Dictionary<string, Tag> tagList) {

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

            #endregion

            try {
                Reader = (XmlTextReader)dataSrc.GetSource();
                // Parse XML file
                while (Reader.Read()) {
                    if (Reader.NodeType == XmlNodeType.Element) {
                        switch (Reader.LocalName) {
                            case "font":

                                #region Font
                                
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

                                    if(tagList.ContainsKey(Reader.GetAttribute("name")))
                                        tagList[Reader.GetAttribute("name")] = new Theme.Font(policeName, color, size, isBold, isItalic, isUnderline, isStrikeout);
                                    else
                                        tagList.Add(Reader.GetAttribute("name"), new Theme.Font(policeName, color, size, isBold, isItalic, isUnderline, isStrikeout));
                                }
                                else {
                                    throw new Exception("Font Format Invalid in style xml file !");
                                }
                                #endregion

                                break;
                            case "cell":

                                #region cell
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
                                }
                                catch {
                                    if (tagList.ContainsKey(label))
                                        tagList[label] = new Cell(font, foregroundColor);
                                    else
                                        tagList.Add(label, new Cell(font, foregroundColor));
                                }
                                #endregion

                                break;
                            case "picture":

                                #region Picture

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

                                #endregion

                                break;
                            case "box":

                                #region Box
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
                                #endregion

                                break;
                            case "style":

                                #region Style

                                label = Reader.GetAttribute("name");
                                ReaderTemp = Reader.ReadSubtree();
                                Dictionary<string, Tag> tagListTemp = new Dictionary<string, Tag>();
                                while (ReaderTemp.Read()) {
                                    if (ReaderTemp.NodeType == XmlNodeType.Element) {
                                        switch (ReaderTemp.LocalName) {
                                            case "tag":
                                                value = ReaderTemp.ReadString();
                                                tagListTemp.Add(value, tagList[value]);
                                                break;
                                        }
                                    }
                                }
                                if(theme.StyleList.ContainsKey(label))
                                    theme.StyleList[label] = new Style(tagListTemp);
                                else
                                    theme.StyleList.Add(label, new Style(tagListTemp));
                                #endregion

                                break;
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
