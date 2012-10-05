namespace TNS.AdExpress.Rolex.Loader.Domain
{
    public class CellExcel
    {

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="lineId">Line Id</param>
        /// <param name="columnId">Column Id</param>
        public CellExcel(int lineId, int columnId)
        {
            LineId = lineId;
            ColumnId = columnId;
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public CellExcel() { }
        #endregion

        #region Assessor

        /// <summary>
        /// Get Line Id
        /// </summary>
        public int LineId { get; private set; }

        /// <summary>
        /// Get Column Id
        /// </summary>
        public int ColumnId { get; private set; }

        #endregion
    }
}
