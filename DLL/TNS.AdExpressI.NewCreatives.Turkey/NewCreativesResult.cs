using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;


namespace TNS.AdExpressI.NewCreatives.Turkey
{
    public class NewCreativesResult : NewCreatives.NewCreativesResult
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public NewCreativesResult(WebSession session)
            : base(session)
        {
        }
        #endregion

        protected override void SetAddressId(int i, DataRow row, AdExpressCellLevel[] cellLevels)
        {
        }

        #region Calendar headers
        /// <summary>
        /// Calendar Headers and Cell factory
        /// </summary>
        protected override void GetCalendarHeaders(out Headers headers, out CellUnitFactory cellFactory, ArrayList parutions)
        {
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
            CellPDM cellPDM = null;

            headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(PROD_COL, _webSession.SiteLanguage), PROD_COL));
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(TOTAL_COL, _webSession.SiteLanguage), TOTAL_COL));
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(POURCENTAGE_COL, _webSession.SiteLanguage), POURCENTAGE_COL));

            _showCreative = CanShowCreative();
            // Add Creative column
            if (_showCreative)
            {
                headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(VERSION_COL, _webSession.SiteLanguage), VERSION_COL));
            }

            if (_showMediaSchedule)
            {
                // Media schedule column
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
            }

            // Une colonne par date de parution
            parutions.Sort();
            foreach (Int32 parution in parutions)
            {
                switch (_webSession.DetailPeriod)
                {
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly:
                        headers.Root.Add(new Header(true
                            , MonthString.GetCharacters(int.Parse(parution.ToString().Substring(4, 2)), cultureInfo, 0) + " " + parution.ToString().Substring(0, 4)
                            , (long)parution));
                        break;
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly:
                        headers.Root.Add(new Header(true
                            , string.Format("S{0} - {1}", parution.ToString().Substring(4, 2), parution.ToString().Substring(0, 4))
                            , (long)parution));
                        break;
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly:
                        headers.Root.Add(new Header(true, AdExpress.Web.Core.Utilities.Dates.DateToString(Dates.YYYYMMDDToDD_MM_YYYY(parution.ToString()).Value, _webSession.SiteLanguage), (long)parution));
                        break;
                    default:
                        break;
                }

            }
            if (!_webSession.Percentage)
            {
                //cellFactory = _webSession.GetCellUnitFactory();
                UnitInformation selectedUnit = UnitsInformation.Get(CustomerSessions.Unit.versionNb);
                var cellIdsNumber =   new CellIdsNumber();
                cellIdsNumber.StringFormat = selectedUnit.StringFormat;
                cellFactory = new CellUnitFactory(cellIdsNumber);

            }
            else
            {
                cellPDM = new CellPDM(0.0);
                cellPDM.StringFormat = "{0:percentWOSign}";
                cellFactory = new CellUnitFactory(cellPDM);
            }
        }
        #endregion

        public override GridResult GetGridResult()
        {
           
            long nbRows = CountData();
            if (nbRows == 0)
            {
                GridResult gridResult = new GridResult();
                gridResult.HasData = false;
                return gridResult;
            }
            if (nbRows > AdExpress.Constantes.Web.Core.MAX_ALLOWED_DATA_ROWS)
            {
                GridResult gridResult = new GridResult();
                gridResult.HasData = true;
                gridResult.HasMoreThanMaxRowsAllowed = true;
                return (gridResult);
            }

            return base.GetGridResult();
        }

        protected override void SetListLine(ResultTable oTab, Int32 cLine, DataRow row)
        {
            if (row != null)
            {
                Int32 lCol = oTab.GetHeadersIndexInResultTable(row["date_creation"].ToString());
                //Get values
                string[] tIds = row[CustomerSessions.Unit.versionNb.ToString()].ToString().Split(',');
                //Affect value
                for (int i = 0; i < tIds.Length; i++)
                {
                    oTab.AffectValueAndAddToHierarchy(1, cLine, lCol, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, 3, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, 2, Convert.ToInt64(tIds[i]));
                }
            }
        }
    }
}
