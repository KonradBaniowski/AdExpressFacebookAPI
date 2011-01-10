using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TNS.InternetClassification.Web.Domain.MultiPart {
    /// <summary>
    /// Panel Class (defined a part of a multipart)
    /// </summary>
    public class Panel {

        #region Variable
        /// <summary>
        /// Panel Name
        /// </summary>
        protected string _name;
        /// <summary>
        /// Associated file Name
        /// </summary>
        protected string _src;
        /// <summary>
        /// Width 
        /// </summary>
        protected int _width;
        /// <summary>
        /// Height
        /// </summary>
        protected int _height;
        /// <summary>
        /// Panel List
        /// </summary>
        protected List<Panel> _panelList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="src">Source</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        public Panel(string name, string src, int width, int height):this(name, src, width, height, null) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="src">Source</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <param name="panelList">Panel List</param>
        public Panel(string name, string src, int width, int height, List<Panel> panelList) {
            _name = name;
            _src = src;
            _width = width;
            _height = height;
            _panelList = panelList;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Panel Name
        /// </summary>
        public string Name {
            get { return _name; }
        }
        /// <summary>
        /// Get Associated file Name
        /// </summary>
        public string Src {
            get { return _src; }
        }
        /// <summary>
        /// Get Width 
        /// </summary>
        public int Width {
            get { return _width; }
        }
        /// <summary>
        /// Get Height
        /// </summary>
        public int Height {
            get { return _height; }
        }
        /// <summary>
        /// Get Panel List
        /// </summary>
        public List<Panel> PanelList {
            get { return _panelList; }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Get Maximum Height
        /// </summary>
        /// <returns>Maximum Height</returns>
        public int GetMaxHeight() {
            if (_panelList != null) {
                var max = (from elem in _panelList
                           select elem.GetMaxHeight()).Max();

                if(max>_height) return max;

            }
            return _height;
        }

        /// <summary>
        /// Get Maximum Width
        /// </summary>
        /// <returns>Maximum Width</returns>
        public int GetMaxWidth() {
            if (_panelList != null) {
                var max = (from elem in _panelList
                           select elem.GetMaxWidth()).Max();

                if (max > _width) return max;
            }
            return _width;
        }
        #endregion

    }
}
