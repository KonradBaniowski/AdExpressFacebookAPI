using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNS.AdExpress.Domain.Web {
    /// <summary>
    /// Custom Styles related to web site resolution (1024x768, 1280x1024 ...)
    /// </summary>
    public class CustomStyles {

        #region Variables
        /// <summary>
        /// My Sessions Width
        /// </summary>
        private int _mySessionsWidth = 500;
        /// <summary>
        /// Tree View Width
        /// </summary>
        private int _treeViewWidth = 230;
        /// <summary>
        /// Creation PopUp Width
        /// </summary>
        private int _creationPopUpWidth = 830;
        /// <summary>
        /// Synthesis Width
        /// </summary>
        private int _synthesisWidth = 600;
        /// <summary>
        /// File Item Width
        /// </summary>
        private int _fileItemWidth = 650;
        /// <summary>
        /// Header Width
        /// </summary>
        private int _headerWidth = 648;
        /// <summary>
        /// Creative PopUp Width
        /// </summary>
        private int _creativePopUpWidth = 394;
        /// <summary>
        /// Chart Evolution Width
        /// </summary>
        private int _chartEvolutionWidth = 850;
        /// <summary>
        /// Chart Evolution Height
        /// </summary>
        private int _chartEvolutionHeight = 500;
        /// <summary>
        /// Chart Media Strategy Width
        /// </summary>
        private int _chartMediaStrategyWidth = 850;
        /// <summary>
        /// Chart Media Strategy Height
        /// </summary>
        private int _chartMediaStrategyHeight = 850;
        /// <summary>
        /// Chart Seasonality Width
        /// </summary>
        private int _chartSeasonalityWidth = 850;
        /// <summary>
        /// Chart Seasonality Height
        /// </summary>
        private int _chartSeasonalityHeight = 500;
        /// <summary>
        /// Chart Seasonality Width
        /// </summary>
        private int _chartSeasonalityBigWidth = 1150;
        /// <summary>
        /// Chart Seasonality Height
        /// </summary>
        private int _chartSeasonalityBigHeight = 700;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Campaign Type Id</param>     
        /// <param name="webTextId">Text Id</param>
        /// <param name="databaseId">Data base Id</param>     
        public CustomStyles(int mySessionsWidth, int treeViewWidth, int creationPopUpWidth, int synthesisWidth, int fileItemWidth, int headerWidth, int creativePopUpWidth
                            , int chartEvolutionWidth, int chartEvolutionHeight, int chartMediaStrategyWidth, int chartMediaStrategyHeight, int chartSeasonalityWidth, int chartSeasonalityHeight
                            , int chartSeasonalityBigWidth, int chartSeasonalityBigHeight) {

            _mySessionsWidth = mySessionsWidth;
            _treeViewWidth = treeViewWidth;
            _creationPopUpWidth = creationPopUpWidth;
            _synthesisWidth = synthesisWidth;
            _fileItemWidth = fileItemWidth;
            _headerWidth = headerWidth;
            _creativePopUpWidth = creativePopUpWidth;
            _chartEvolutionWidth = chartEvolutionWidth;
            _chartEvolutionHeight = chartEvolutionHeight;
            _chartMediaStrategyWidth = chartMediaStrategyWidth;
            _chartMediaStrategyHeight = chartMediaStrategyHeight;
            _chartSeasonalityWidth = chartSeasonalityWidth;
            _chartSeasonalityHeight = chartSeasonalityHeight;
            _chartSeasonalityBigWidth = chartSeasonalityBigWidth;
            _chartSeasonalityBigHeight = chartSeasonalityBigHeight;
          
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get My Sessions Width
        /// </summary>
        public int MySessionsWidth {
            get { return (_mySessionsWidth); }
        }
        /// <summary>
        /// Get Tree View Width
        /// </summary>
        public int TreeViewWidth {
            get { return (_treeViewWidth); }
        }
        /// <summary>
        /// Get Creation PopUp Width
        /// </summary>
        public int CreationPopUpWidth {
            get { return (_creationPopUpWidth); }
        }
        /// <summary>
        /// Get Synthesis Width
        /// </summary>
        public int SynthesisWidth {
            get { return (_synthesisWidth); }
        }
        /// <summary>
        /// Get File Item Width
        /// </summary>
        public int FileItemWidth {
            get { return (_fileItemWidth); }
        }
        /// <summary>
        /// Get Header Width
        /// </summary>
        public int HeaderWidth {
            get { return (_headerWidth); }
        }
        /// <summary>
        /// Get Creative PopUp Width
        /// </summary>
        public int CreativePopUpWidth {
            get { return (_creativePopUpWidth); }
        }
        /// <summary>
        /// Get Chart Evolution Width
        /// </summary>
        public int ChartEvolutionWidth {
            get { return (_chartEvolutionWidth); }
        }
        /// <summary>
        /// Get Chart Evolution Height 
        /// </summary>
        public int ChartEvolutionHeight {
            get { return (_chartEvolutionHeight); }
        }
        /// <summary>
        /// Get Chart Media Strategy Width
        /// </summary>
        public int ChartMediaStrategyWidth {
            get { return (_chartMediaStrategyWidth); }
        }
        /// <summary>
        /// Get Chart Media Strategy Height
        /// </summary>
        public int ChartMediaStrategyHeight {
            get { return (_chartMediaStrategyHeight); }
        }
        /// <summary>
        /// Get Chart Seasonality Width
        /// </summary>
        public int ChartSeasonalityWidth {
            get { return (_chartSeasonalityWidth); }
        }
        /// <summary>
        /// Get Chart Seasonality Height
        /// </summary>
        public int ChartSeasonalityHeight {
            get { return (_chartSeasonalityHeight); }
        }
        /// <summary>
        /// Get Chart Seasonality Big Width
        /// </summary>
        public int ChartSeasonalityBigWidth {
            get { return (_chartSeasonalityBigWidth); }
        }
        /// <summary>
        /// Get Chart Seasonality Big Height
        /// </summary>
        public int ChartSeasonalityBigHeight {
            get { return (_chartSeasonalityBigHeight); }
        }
        #endregion

    }
}
