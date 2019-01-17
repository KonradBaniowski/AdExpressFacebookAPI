using System;
using System.Data;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.LostWon.Turkey
{
    public class LostWonResult : LostWon.LostWonResult
    {
        public LostWonResult(WebSession session) : base(session)
        {
        }

        protected override Int32 InitFinalLine(ResultTable fromTab, ResultTable toTab, Int32 fromLine, CellLevel parent,
            Int32 msIndex)
        {
            CellLevel cFromLevel = (CellLevel) fromTab[fromLine, 1];
            Int32 cLine = toTab.AddNewLine(fromTab.GetLineStart(fromLine).LineType);
            AdExpressCellLevel cell = new AdExpressCellLevel(cFromLevel.Id, cFromLevel.Label, parent, cFromLevel.Level,
                cLine, _session);
            toTab[cLine, 1] = cell;

            //Links
            if (_showMediaSchedule) toTab[cLine, msIndex] = new CellMediaScheduleLink(cell, _session);


            return cLine;

        }

        #region InitLine

        /// <summary>
        /// Delegate to init lines
        /// </summary>
        /// <param name="tab">Table to fill</param>
        /// <param name="row">Data container</param>
        /// <param name="cellFactory">Cell Factory</param>
        /// <param name="level">Current level</param>
        /// <param name="parent">Parent level</param>
        /// <returns>Index of current line</returns>
        protected override Int32 InitLine(ResultTable tab, DataRow row, CellUnitFactory cellFactory, Int32 level,
            CellLevel parent)
        {

            Int32 cLine = -1;
            CellLevel cell;
            switch (level)
            {
                case 1:
                    cLine = tab.AddNewLine(LineType.level1);
                    break;
                case 2:
                    cLine = tab.AddNewLine(LineType.level2);
                    break;
                case 3:
                    cLine = tab.AddNewLine(LineType.level3);
                    break;
                default:
                    throw new ArgumentException($"Level {level} is not supported.");
            }
            tab[cLine, 1] = cell = new CellLevel(
                _session.GenericProductDetailLevel.GetIdValue(row, level)
                , _session.GenericProductDetailLevel.GetLabelValue(row, level)
                , parent
                , level
                , cLine);


            for (Int32 i = 2; i <= tab.DataColumnsNumber; i++)
            {
                tab[cLine, i] = cellFactory.Get(0.0);
            }
            return cLine;

        }

        #endregion

        public override GridResult GetGridResult()
        {
            var gridResult = new GridResult();

            //Count nb rows
            switch ((Int32) _session.CurrentTab)
            {
                case DynamicAnalysis.LOST:
                case DynamicAnalysis.LOYAL:
                case DynamicAnalysis.LOYAL_DECLINE:
                case DynamicAnalysis.LOYAL_RISE:
                case DynamicAnalysis.PORTEFEUILLE:
                case DynamicAnalysis.WON:

                    long nbRows = CountData();
                    if (nbRows == 0)
                    {
                        gridResult.HasData = false;
                        return gridResult;
                    }
                    if (nbRows > CstWeb.Core.MAX_ALLOWED_DATA_ROWS)
                    {
                        gridResult.HasData = true;
                        gridResult.HasMoreThanMaxRowsAllowed = true;
                        return gridResult;
                    }

                    break;
                //case DynamicAnalysis.SYNTHESIS:
                //    return GetSynthesisData();

            }
            return base.GetGridResult();
        }


    }
}
