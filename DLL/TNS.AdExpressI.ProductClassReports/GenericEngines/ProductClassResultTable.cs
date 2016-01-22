#region Information
/*
 * Author : G Ragneau
 * Created on : 20/01/2009
 * Modified:
 *      Date - Author - Description
 * 
 * 
 * 
 * 
 * 
 * 
 * */
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.WebResultUI.Tools;

namespace TNS.AdExpressI.ProductClassReports.GenericEngines
{

    /// <summary>
    /// Result Table Extension : 
    ///     Insert Line
    ///     Line Ids    
    /// </summary>
    public class ProductClassResultTable:ResultTable
    {

        #region Properties
        /// <summary>
        /// Line Hierarchy
        /// </summary>
        ClassifLevels<CellLevel> _levels = new ClassifLevels<CellLevel>();
        /// <summary>
        /// Indicate if sub levels must be sorted or not
        /// </summary>
        protected bool _sortSubLevels = true;
        /// <summary>
        /// Get / Set flag which indicates if sub levels must be sorted or not
        /// </summary>
        public bool SortSubLevels
        {
            get { return _sortSubLevels; }
            set { _sortSubLevels = value; }
        }
        #endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="nbLines">Initial Number of lines. If required, this number could be extended.</param>
		/// <param name="nbColumns">Number of columns</param>
        public ProductClassResultTable(int nbLines, int nbColumns):base(nbLines, nbColumns)
        {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="nbLines">Initial Number of lines. If required, this number could be extended.</param>
		/// <param name="headers">Columns headers. Used to generate the number of columns and to identify the columns</param>
        public ProductClassResultTable(int nbLines, Headers headers):base(nbLines, headers)
        {
		}
		#endregion

        #region Methods
        /// <summary>
        /// Insert a new line in the result table under the mainkays classification entries and the subKeys entries.
        /// if subkeys is null, level is added to main levels. if not, level is added to the sub levels
        /// </summary>
        /// <param name="lineType">Type of line</param>
        /// <param name="mainKeys">Main classification entries</param>
        /// <param name="subKeys">Sub classification entries (if null, level is added to the main classification)</param>
        /// <param name="level">Level to add</param>
        /// <returns>index of the added line</returns>
        public int InsertNewLine(LineType lineType, List<Int64> mainKeys, List<Int64> subKeys, CellLevel level)
        {
            int l = -1;
            bool isNewLine = false;

            if (subKeys == null || subKeys.Count < 1)
            {
                return this.AddNewLine(lineType, mainKeys, level);
            }
            else
            {
                if (!_levels.MyContainsSubLevel(mainKeys, subKeys))
                {
                    isNewLine = true;
                    //add level
                    _levels.AddSubLevel(mainKeys, subKeys, level);
                    CellLevel parent;
                    if (subKeys.Count > 1)
                    {
                        parent = _levels.GetSubValue(mainKeys, subKeys.GetRange(0, subKeys.Count - 1));
                    }
                    else
                    {
                        parent = _levels.GetValue(mainKeys);
                    }
                    l = parent.LineIndexInResultTable + 1;
                }
                else
                {
                    l = _levels.GetSubValue(mainKeys, subKeys).LineIndexInResultTable;
                }
            }

            if (isNewLine)
            {

                _currentLineIndex++;
                level.LineIndexInResultTable = l = (l>-1)?l:_currentLineIndex;
                if (!_linesStart.ContainsKey(lineType)) _linesStart.Add(lineType, new LineStart(lineType));

                //Insert current line in _data
                _data.Insert(l, new ICell[_nbColumns]);
                _dataOrder.Insert(l, new int[2] { l, -1 });

                SetLineStart(new ProductClassLineStart(lineType), l);

                _data[l][_stopLineColumnIndex] = new LineStop();
                _data[l][_levelColumn] = level;

                //Move next lines
                for (int i = l + 1; i <= _currentLineIndex; i++)
                {
                    _dataOrder[i][0] = i;
                    ((CellLevel)_data[i][_levelColumn]).LineIndexInResultTable = i;
                }

            }

            return l;

        }
        /// <summary>
        /// Insert a new line in the result table under the mainkays classification entries.
        /// </summary>
        /// <param name="lineType">Type of line</param>
        /// <param name="mainKeys">Main classification entries</param>
        /// <param name="level">Level to add</param>
        /// <returns>index of the added line</returns>
        public int InsertNewLine(LineType lineType, List<Int64> mainKeys, CellLevel level)
        {

            return InsertNewLine(lineType, mainKeys, null, level);

        }
        public int AddNewLine(LineType lineType, List<Int64> mainKeys, CellLevel level)
        {
            int cLine = base.AddNewLine(lineType);
            _data[cLine][_levelColumn] = level;
            SetLineStart(new ProductClassLineStart(lineType), cLine);
            level.LineIndexInResultTable = cLine;
            if (!_levels.MyContains(mainKeys))
            {
                _levels.Add(mainKeys, level);
            }
            return cLine;

        }
        /// <summary>
        /// Add a new line in the result table matching the mainkays classification entries and the subKeys entries.
        /// if subkeys is null, level is added to main levels. if not, level is added to the sub levels
        /// </summary>
        /// <param name="lineType">Type of line</param>
        /// <param name="mainKeys">Main classification entries</param>
        /// <param name="subKeys">Sub classification entries (if null, level is added to the main classification)</param>
        /// <param name="level">Level to add</param>
        /// <returns>index of the added line</returns>
        public int AddNewLine(LineType lineType, List<Int64> mainKeys, List<Int64> subKeys, CellLevel level)
        {
            int l = -1;
            bool isNewLine = false;

            if (subKeys == null || subKeys.Count < 1)
            {
                return this.AddNewLine(lineType, mainKeys, level);
            }
            else
            {
                if (!_levels.MyContainsSubLevel(mainKeys, subKeys))
                {
                    isNewLine = true;
                    //add level
                    _levels.AddSubLevel(mainKeys, subKeys, level);
                }
            }

            if (isNewLine)
            {

                l = base.AddNewLine(lineType);
                _data[l][_levelColumn] = level;
                SetLineStart(new ProductClassLineStart(lineType), l);
                level.LineIndexInResultTable = l;

            }

            return l;

        }
        /// <summary>
        /// Check if a line exist
        /// </summary>
        /// <param name="mainKeys">Entries keys</param>
        /// <returns>either -1 if line does not exist, either index of the line</returns>
        public int Contains(List<Int64> mainKeys)
        {
            CellLevel c = _levels.GetValue(mainKeys);
            return (c != null) ? c.LineIndexInResultTable : -1;
        }
        /// <summary>
        /// Check if a line exist
        /// </summary>
        /// <param name="mainKeys">Entries keys</param>
        /// <returns>either -1 if line does not exist, either index of the line</returns>
        public int Contains(List<Int64> mainKeys, List<Int64> subKeys)
        {
            if (subKeys == null || subKeys.Count < 1)
            {
                return this.Contains(mainKeys);
            }
            CellLevel c = _levels.GetSubValue(mainKeys, subKeys);
            return (c != null) ? c.LineIndexInResultTable : -1;
        }
        /// <summary>
        /// Get the level of a set of keys
        /// </summary>
        /// <param name="mainKeys">Entries keys</param>
        /// <returns>level or null</returns>
        public CellLevel GetLevel(List<Int64> mainKeys)
        {
            return _levels.GetValue(mainKeys);
        }
        /// <summary>
        /// Get the level of a set of keys
        /// </summary>
        /// <param name="mainKeys">Entries keys</param>
        /// <param name="subKeys">Secondary levels keys</param>
        /// <returns>level or null</returns>
        public CellLevel GetLevel(List<Int64> mainKeys, List<Int64> subKeys)
        {
            if (subKeys == null || subKeys.Count < 1)
            {
                return this.GetLevel(mainKeys);
            }
            return _levels.GetSubValue(mainKeys, subKeys);
        }
        #endregion

        #region sorting
        /// <summary>
        /// Sort data
        /// </summary>
        /// <param name="sortOrder">Type of sorting</param>
        /// <param name="columnIndex">Column considered to sort the data</param>
        public override void Sort(SortOrder sortOrder, int columnIndex)
        {

            if (columnIndex == LINE_START_COLUMN_INDEX || columnIndex == _nbColumns - 1) throw (new ArgumentException("Column 0 and last column can not be sorted."));

            //attributs
            this._sort = sortOrder;

            while (columnIndex < _nbColumns - 1)
            {
                this._sortIndex = columnIndex;

                //variables locales
                List<int[]> nTab = new List<int[]>(_dataOrder.Count);
                int iLine = 0;
                int iOrderLine = -1;
                LineStart lStart = (LineStart)_data[iLine][0];
                List<LineStart> levels = new List<LineStart>();

                try
                {
                    //lancement du tri
                    SortClassifLevel(this._levels, nTab, ref iOrderLine, true);
                    if (nTab.Count > 0)
                    {
                        _dataOrder = nTab;
                    }
                    break;
                }
                catch
                {
                    columnIndex++;
                }
            }

        }

        /// <summary>
        /// Sort the data
        /// </summary>
        /// <param name="levels">Levels to sort</param>
        /// <param name="tOrder">Sorted data</param>
        /// <param name="iOrderLine">Last sorted data in the sorted table tOrder</param>
        /// <param name="sortCurrentLevel">Indicate if current Level must be sorted</param>
        protected void SortClassifLevel(ClassifLevels<CellLevel> levels, List<int[]> tOrder, ref int iOrderLine, bool sortCurrentLevel)
        {

            int iParentLine = iOrderLine;
            List<int> lIndex = null;
            List<ClassifLevels<CellLevel>> lIndexClassif = null;
            ICell[] tValue = null;
            int[] tNewOrder = null;


            //Sort Sub Levels
            if (levels.SubLevels != null && levels.SubLevels.Count > 0)
            {
                lIndex = new List<int>();
                lIndexClassif = new List<ClassifLevels<CellLevel>>();
                foreach(ClassifLevels<CellLevel> c in levels.SubLevels.Values)
                {
                    lIndex.Add(c.Value.LineIndexInResultTable);
                    lIndexClassif.Add(c);
                }
                //Sort values
                tValue = new ICell[lIndex.Count];
                tNewOrder = new Int32[lIndex.Count];
                for (int i = 0; i < lIndex.Count; i++)
                {
                    tValue[i] = _data[lIndex[i]][this._sortIndex];
                    tNewOrder[i] = i;
                }
                if (_sortSubLevels)
                {
                    tNewOrder = Tri.Tri_ICell(0, tValue.Length - 1, tValue, Convert.ToBoolean(this._sort.GetHashCode()));
                }
                //Add Values and sort sub levels
                for (int i = 0; i < tValue.Length; i++)
                {
                    //Ajout du niveau
                    ++iOrderLine;
                    tOrder.Add(new Int32[2] { lIndex[tNewOrder[i]], iParentLine });

                    //tris des sous niveaux
                    SortClassifLevel(lIndexClassif[tNewOrder[i]], tOrder, ref iOrderLine, _sortSubLevels);
                }

            }
            //Sort Main Levels
            if (levels.Count > 0)
            {
                lIndex = new List<int>();
                lIndexClassif = new List<ClassifLevels<CellLevel>>();
                foreach (ClassifLevels<CellLevel> c in levels.Values)
                {
                    lIndex.Add(c.Value.LineIndexInResultTable);
                    lIndexClassif.Add(c);
                }
                //Sort values
                tValue = new ICell[lIndex.Count];
                tNewOrder = new Int32[lIndex.Count];
                for (int i = 0; i < lIndex.Count; i++)
                {
                    tValue[i] = _data[lIndex[i]][this._sortIndex];
                    tNewOrder[i] = i;
                }
                if (sortCurrentLevel)
                {
                    tNewOrder = Tri.Tri_ICell(0, tValue.Length - 1, tValue, Convert.ToBoolean(this._sort.GetHashCode()));
                }
                //Add Values and sort sub levels
                for (int i = 0; i < tValue.Length; i++)
                {
                    //Ajout du niveau
                    ++iOrderLine;
                    tOrder.Add(new Int32[2] { lIndex[tNewOrder[i]], iParentLine });

                    //tris des sous niveaux
                    SortClassifLevel(lIndexClassif[tNewOrder[i]], tOrder, ref iOrderLine, sortCurrentLevel);
                }

            }
        }
        #endregion

    }

}
