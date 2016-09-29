using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CustomerConstantes = TNS.AdExpress.Constantes.Customer;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using ClassificationDB = TNS.AdExpress.DataAccess.Classification;

using TNS.FrameWork;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Utilities;
using System.Collections;
using System.Data;
using TNS.AdExpressI.CampaignAnalysis.DAL;
using System.Globalization;
using FctWeb = TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpressI.CampaignAnalysis
{
    public class CampaignAnalysisResult
    {
        #region GetData
        /// <summary>
        /// Obtient le tableau contenant l'ensemble des résultats du parrainage télé
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginningPeriod">Date de début</param>
        /// <param name="endPeriod">Date de fin</param>
        /// <returns>Tableau de résultat</returns>
        public static ResultTable GetData(WebSession webSession, string beginningPeriod, string endPeriod)
        {

            #region Tableaux d'index
            long nbCol = 0;
            Hashtable dimensionColIndex = new Hashtable();
            string dimensionListForLabelSearch = "";
            int maxIndex = 0;
            InitIndexAndValues(webSession, dimensionColIndex, ref nbCol, beginningPeriod, endPeriod, ref maxIndex, ref dimensionListForLabelSearch);
            #endregion

            #region Chargement du tableau
            long nbLineInNewTable = 0;
            object[,] tabData = GetPreformatedTable(webSession, dimensionColIndex, maxIndex, ref nbLineInNewTable, beginningPeriod, endPeriod);
            #endregion

            if (tabData == null)
            {
                return null;
            }

            return (GetResultTable(webSession, tabData, nbLineInNewTable, dimensionColIndex, dimensionListForLabelSearch));
        }
        #endregion

        public static GridResult GetGridResult(WebSession webSession, string beginningPeriod, string endPeriod)
        {
            GridResult gridResult = new GridResult();
            ResultTable resultTable = GetData(webSession, beginningPeriod, endPeriod);
            string mediaSchedulePath = "/MediaSchedulePopUp";
            string insertionPath = "/Insertions";
            string versionPath = "/Creative";
            LineStart cLineStart = null;
            int nbLines = 0;

            if (resultTable == null || resultTable.DataColumnsNumber == 0)
            {
                gridResult.HasData = false;
                return gridResult;
            }

            resultTable.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
            resultTable.CultureInfo = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo;
            object[,] gridData = null;
           
            for (int i = 0; i < resultTable.LinesNumber; i++)
            {
                cLineStart = resultTable.GetLineStart(i);
                if (!(cLineStart is LineHide))
                    nbLines++;
            }
            if (nbLines == 0)
            {
                gridResult.HasData = false;
                return gridResult;
            }
                gridData = new object[nbLines, resultTable.ColumnsNumber + 1]; //+2 car ID et PID en plus  -  //_data.LinesNumber // + 1 for gad column

            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();

            //Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            columns.Add(new { headerText = "GAD", key = "GAD", dataType = "string", width = "*", hidden = true });
            schemaFields.Add(new { name = "GAD" });

            List<object> groups = null;
            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo;
            string format = string.Empty;
            List<object> subGroups = null;

            //Headers
            if (resultTable.NewHeaders != null)
            {
                for (int j = 0; j < resultTable.NewHeaders.Root.Count; j++)
                {
                    groups = null;
                    string colKey = string.Empty;
                    if (resultTable.NewHeaders.Root[j].Count > 0)
                    {
                        groups = new List<object>();

                        int nbGroupItems = resultTable.NewHeaders.Root[j].Count;
                        for (int g = 0; g < nbGroupItems; g++)
                        {
                            //Manage sub groups items (stynthesis result)
                            if (resultTable.NewHeaders.Root[j][g].Count > 0)
                            {
                                #region Sub group synthesis

                                subGroups = new List<object>();

                                int nbSubGroupitems = resultTable.NewHeaders.Root[j][g].Count;

                                for (int sg = 0; sg < nbSubGroupitems; sg++)
                                {
                                    colKey = string.Format("sg{0}", resultTable.NewHeaders.Root[j][g][sg].IndexInResultTable);
                                    ICell cell = null;
                                    //cell format sub group
                                    if (resultTable != null && resultTable.LinesNumber > 0)
                                    {
                                        cell = resultTable[0, resultTable.NewHeaders.Root[j][g][sg].IndexInResultTable];
                                    }

                                    //subGroups.Add(new { headerText = resultTable.NewHeaders.Root[j][g][sg].Label, key = colKey, dataType = "number", format = format, columnCssClass = "colStyle", width = "*", allowSorting = true });
                                    subGroups.Add(GetColumnDef(webSession, cell, resultTable.NewHeaders.Root[j][g][sg].Label, ref colKey, "*"));
                                    schemaFields.Add(new { name = colKey });

                                }

                                colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j][g].IndexInResultTable);
                                groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = colKey, group = subGroups });
                                //schemaFields.Add(new { name = colKey });


                                #endregion
                            }
                            else
                            {
                                //No sub groups items
                                #region /No sub groups items
                                colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j][g].IndexInResultTable);
                                //cell format
                                ICell cell = null;
                                if (resultTable != null && resultTable.LinesNumber > 0)
                                {
                                    cell = resultTable[0, resultTable.NewHeaders.Root[j][g].IndexInResultTable];
                                }

                                //groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = colKey, dataType = "number", format = format, columnCssClass = "colStyle", width = "*", allowSorting = true });
                                groups.Add(GetColumnDef(webSession, cell, resultTable.NewHeaders.Root[j][g].Label, ref colKey, "*"));
                                schemaFields.Add(new { name = colKey });
                                #endregion

                            }


                        }

                        columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = "gr" + colKey, group = groups });
                        columnsFixed.Add(new { columnKey = "gr" + colKey, isFixed = false, allowFixing = false });
                    }
                    else
                    {
                        colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j].IndexInResultTable);
                        if (j == 0)
                        {
                            columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "350", allowSorting = true, template = "{{if ${GAD}.length > 0}} <span class=\"gadLink\" href=\"#gadModal\" data-toggle=\"modal\" data-gad=\"[${GAD}]\">${" + colKey + "}</span> {{else}} ${" + colKey + "} {{/if}}" });
                            columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });
                        }
                        else
                        {
                            if (resultTable.NewHeaders.Root[j].Label == GestionWeb.GetWebWord(805, webSession.SiteLanguage))
                            {
                                var cell = resultTable[0, resultTable.NewHeaders.Root[j].IndexInResultTable];

                                //columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "number", format = format, columnCssClass = "colStyle", width = "*", allowSorting = true });
                                columns.Add(GetColumnDef(webSession, cell, resultTable.NewHeaders.Root[j].Label, ref colKey, "*"));
                            }
                            else
                            {
                                var cell = resultTable[0, resultTable.NewHeaders.Root[j].IndexInResultTable];
                                //columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "*" });
                                columns.Add(GetColumnDef(webSession, cell, resultTable.NewHeaders.Root[j].Label, ref colKey, "*"));
                            }
                            
                            columnsFixed.Add(new { columnKey = colKey, isFixed = false, allowFixing = false });
                        }
                        schemaFields.Add(new { name = colKey });
                    }

                }
            }

            //table body rows
            int currentLine = 0;
            for (int i = 0; i < resultTable.LinesNumber; i++) //
            {
                cLineStart = resultTable.GetLineStart(i);
                if (cLineStart is LineHide)
                    continue;

                gridData[currentLine, 0] = i; // Pour column ID
                gridData[currentLine, 1] = resultTable.GetSortedParentIndex(i); // Pour column PID

                for (int k = 1; k < resultTable.ColumnsNumber - 1; k++)
                {
                    var cell = resultTable[i, k];
                    var link = string.Empty;
                    if (cell is CellMediaScheduleLink)
                    {

                        var c = cell as CellMediaScheduleLink;
                        if (c != null)
                        {
                            link = c.GetLink();
                            if (!string.IsNullOrEmpty(link))
                            {
                                link = string.Format("<center><a href='{0}?{1}' target='_blank'><span class='fa fa-search-plus'></span></a></center>"
                           , mediaSchedulePath
                           , link);
                            }
                        }
                        gridData[currentLine, k + 2] = link;

                    }
                    else if (cell is CellSponsorshipInsertionsLink)
                    {
                        var c = cell as CellSponsorshipInsertionsLink;

                        if (c != null)
                        {
                            link = c.GetLink();
                            if (!string.IsNullOrEmpty(link))
                            {
                                link = string.Format("<center><a href='{0}?{1}' target='_blank'><span class='fa fa-search-plus'></span></a></center>"
                         , insertionPath
                         , link);
                            }

                        }
                        gridData[currentLine, k + 2] = link;
                    }
                    else if (cell is CellSponsorshipCreativesLink)
                    {
                        var c = cell as CellSponsorshipCreativesLink;

                        if (c != null)
                        {
                            link = c.GetLink();
                            if (!string.IsNullOrEmpty(link))
                            {
                                link = string.Format("<center><a href='{0}?{1}' target='_blank'><span class='fa fa-search-plus'></span></a></center>"
                         , versionPath
                         , link);
                            }

                        }
                        gridData[currentLine, k + 2] = link;
                    }
                    else
                    {
                        if (cell is CellEvol || cell is CellPDM)
                        {
                            double value = ((CellUnit)cell).Value;

                            if (double.IsInfinity(value))
                                gridData[currentLine, k + 2] = "Infinity";
                            else if (double.IsNaN(value))
                                gridData[currentLine, k + 2] = null;
                            else
                                gridData[currentLine, k + 2] = value / 100;
                        }
                        else if (cell is CellUnit)
                        {
                            if (((LineStart)resultTable[i, 0]).LineType != LineType.nbParution)
                            {
                               gridData[currentLine, k + 2] = FctWeb.Units.ConvertUnitValue(((CellUnit)cell).Value, webSession.Unit);
                            }
                            else
                                gridData[currentLine, k + 2] = ((CellUnit)cell).Value;
                        }
                        else if (cell is AdExpressCellLevel)
                        {
                            string label = ((AdExpressCellLevel)cell).RawString();
                            string gadParams = ((AdExpressCellLevel)cell).GetGadParams();

                            if (gadParams.Length > 0)
                                gridData[currentLine, 2] = gadParams;
                            else
                                gridData[currentLine, 2] = "";

                            gridData[currentLine, k + 2] = label;
                        }
                        else
                        {
                           gridData[currentLine, k + 2] = cell.RenderString();
                        }
                    }
                }
                currentLine++;
            }
            gridResult.NeedFixedColumns = true;
            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            gridResult.ColumnsFixed = columnsFixed;
            gridResult.Data = gridData;
            gridResult.Unit = webSession.Unit.ToString();

            return gridResult;
        }

        private static object GetColumnDef(WebSession webSession, ICell cell, string headerText, ref string key, string width)
        {

            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo;

            if (cell is CellPercent)
                return new { headerText = headerText, key = key, dataType = "number", format = "percent", columnCssClass = "colStyle", width = width, allowSorting = true };
            else if (cell is CellEvol)
                return new { headerText = headerText, key = key + "-evol", dataType = "number", format = "percent", columnCssClass = "colStyle", width = width, allowSorting = true };
            else if (cell is CellDuration)
            {
                key += "-unit-duration";
                return new { headerText = headerText, key = key, dataType = "number", format = "duration", columnCssClass = "colStyle", width = width, allowSorting = true };
            }
            else if (cell is CellInsertion)
            {
                string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(CustomerSessions.Unit.insertion).StringFormat);
                key += "-unit-insertion";
                return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            }
            else if (cell is CellMMC)
            {
                string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(CustomerSessions.Unit.mmPerCol).StringFormat);
                key += "-unit-mmPerCol";
                return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            }
            else if (cell is CellPage)
            {
                string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(CustomerSessions.Unit.pages).StringFormat);
                key += "-unit-pages";
                return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            }
            else if (cell is CellEuro)
            {
                string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(CustomerSessions.Unit.euro).StringFormat);
                key += "-unit-euro";
                return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            }
            else if (cell is CellKEuro)
            {
                string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(CustomerSessions.Unit.kEuro).StringFormat);
                key += "-unit-euro";
                return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            }
            else if (cell is CellGRP)
            {
                string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(CustomerSessions.Unit.grp).StringFormat);
                key += "-unit-euro";
                return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            }
            else if (cell is CellVolume)
            {
                string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(CustomerSessions.Unit.volume).StringFormat);
                key += "-unit-euro";
                return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            }
            else if (cell is AdExpressCellLevel)
            {
                return new { headerText = headerText, key = key, dataType = "string", width = width, template = "{{if ${GAD}.length > 0}} <span class=\"gadLink\" href=\"#gadModal\" data-toggle=\"modal\" data-gad=\"[${GAD}]\">${" + key + "}</span> {{else}} ${" + key + "} {{/if}}" };
            }
            else
                return new { headerText = headerText, key = key, dataType = "string", width = width };
        }

        #region Méthodes internes

        #region Tableau Préformaté
        /// <summary>
        /// Obtient le tableau de résultat préformaté pour le parrainage télé
        /// </summary>
        /// <param name="webSession">Session du client</param>	
        /// <param name="dimensionColIndex">Liste des index des dimensions en colonnes du tableau</param>
        /// <param name="nbCol">Nombre de colonnes du tableau de résultat</param>
        /// <param name="nbLineInNewTable">(out) Nombre de ligne du tableau de résultat</param>
        /// <param name="periodBeginning">Date de début</param>
        /// <param name="periodEnd">Date de fin</param>
        /// <returns>Tableau de résultat</returns>
        private static object[,] GetPreformatedTable(WebSession webSession, Hashtable dimensionColIndex, long nbCol, ref long nbLineInNewTable, string periodBeginning, string periodEnd)
        {

            #region Variables
            DataSet ds = null;
            #endregion

            #region Chargement des données à partir de la base	
            ds = CampaignAnalysisDAL.GetData(webSession, periodBeginning, periodEnd);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            #endregion

            #region Déclaration du tableau de résultat
            long nbline = dt.Rows.Count;
            object[,] tabResult = new object[nbline, nbCol];
            #endregion

            #region Tableau de résultat
            SetMediaDetailLevelValues(webSession, dt, tabResult, ref nbLineInNewTable, dimensionColIndex, nbCol);
            #endregion

            return (tabResult);
        }
        #endregion

        #region Formattage d'un tableau de résultat
        /// <summary>
        /// Formattage d'un tableau de résultat à partir d'un tableau de données
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tabData">Table de données</param>
        /// <param name="nbLineInTabData">Nombre de ligne dans le tableau</param>
        /// <param name="dimensionColIndex">Column Index of dimension</param>
        /// <param name="dimensionListForLabelSearch">Dimension List for Label Searching</param>
        /// <returns>Tableau de résultat</returns>
        private static ResultTable GetResultTable(WebSession webSession, object[,] tabData, long nbLineInTabData, Hashtable dimensionColIndex, string dimensionListForLabelSearch)
        {

            #region Variables		
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            Int64 oldIdL4 = -1;
            int currentLine;
            int currentLineInTabResult;
            int k;
            ResultTable resultTable = null;
            CellUnitFactory cellUnitFactory = null, cellKeuroFactory = null, cellEuroFactory = null, cellDurationFactory = null, cellInsertionFactory = null;
            CellUnit cellKeuro = null, cellEuro = null, cellDuration = null, cellInsertion = null;
            #endregion

            #region Calcul des pourcentages
            bool computePercentage = false;
            if (webSession.PercentageAlignment != WebConstantes.Percentage.Alignment.none)
                computePercentage = true;
            #endregion

            #region Vahicle Informations
            string vehicleSelection = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new System.Exception("Media Selection is not valid"));
            VehicleInformation vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            Vehicles.names vehicle = vehicleInformation.Id;
            #endregion

            #region Headers
            Headers headers = new Headers();
            // Ajout de la colonne des libellés des Autres dimensions
            if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
                headers.Root.Add(new Header(true, GestionWeb.GetWebWord(1164, webSession.SiteLanguage), FrameWorkResultConstantes.TvSponsorship.LEVEL_HEADER_ID));
            else if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS)
                headers.Root.Add(new Header(true, GestionWeb.GetWebWord(804, webSession.SiteLanguage), FrameWorkResultConstantes.TvSponsorship.LEVEL_HEADER_ID));
            int startDataColIndex = 1;
            int startDataColIndexInit = 1;

            // Ajout Création 
            bool showCreative = false;
            //A vérifier Création où version
            if (vehicleInformation.ShowCreations &&
                (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
                (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES &&
                (webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)) ||
                webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES))
                )
            {
                headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(1994, webSession.SiteLanguage), FrameWorkResultConstantes.TvSponsorship.CREATIVE_HEADER_ID));
                showCreative = true;
                startDataColIndex++;
                startDataColIndexInit++;
            }
            bool showInsertions = false;
            if (vehicleInformation.ShowInsertions &&
                ((webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES &&
                (webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product))) ||
                webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES
                ))
            {
                headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(2245, webSession.SiteLanguage), FrameWorkResultConstantes.TvSponsorship.INSERTIONS_HEADER_ID));
                startDataColIndex++;
                startDataColIndexInit++;
                showInsertions = true;
            }

            //Colonne total 			
            bool showTotal = false;
            if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
            {
                startDataColIndexInit++;
                showTotal = true;
                headers.Root.Add(new Header(true, GestionWeb.GetWebWord(805, webSession.SiteLanguage), FrameWorkResultConstantes.TvSponsorship.TOTAL_HEADER_ID));
            }

            // Chargement des libellés de colonnes
            SetColumnsLabels(webSession, headers, dimensionColIndex, dimensionListForLabelSearch);
            #endregion

            #region Déclaration du tableau de résultat
            int nbLine = GetNbLineFromPreformatedTableToResultTable(tabData) + 1;
            resultTable = new ResultTable(nbLine, headers);
            int nbCol = resultTable.ColumnsNumber - 2;
            int totalColIndex = -1;
            if (showTotal) totalColIndex = resultTable.GetHeadersIndexInResultTable(FrameWorkResultConstantes.TvSponsorship.TOTAL_HEADER_ID.ToString());
            int levelLabelColIndex = resultTable.GetHeadersIndexInResultTable(FrameWorkResultConstantes.TvSponsorship.LEVEL_HEADER_ID.ToString());
            int creativeColIndex = resultTable.GetHeadersIndexInResultTable(FrameWorkResultConstantes.TvSponsorship.CREATIVE_HEADER_ID.ToString());
            int insertionsColIndex = resultTable.GetHeadersIndexInResultTable(FrameWorkResultConstantes.TvSponsorship.INSERTIONS_HEADER_ID.ToString());
            #endregion

            #region Sélection de l'unité
            switch (webSession.PreformatedTable)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media:
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period:
                    cellUnitFactory = webSession.GetCellUnitFactory();
                    break;
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units:
                    cellKeuro = new CellKEuro(0.0);
                    cellKeuro.StringFormat = UnitsInformation.Get(UnitsInformation.DefaultKCurrency).StringFormat;
                    cellEuro = new CellEuro(0.0);
                    cellEuro.StringFormat = UnitsInformation.Get(UnitsInformation.DefaultCurrency).StringFormat;
                    cellDuration = new CellDuration(0.0);
                    cellDuration.StringFormat = UnitsInformation.Get(CustomerSessions.Unit.duration).StringFormat;
                    cellInsertion = new CellInsertion(0.0);
                    cellInsertion.StringFormat = UnitsInformation.Get(CustomerSessions.Unit.insertion).StringFormat;

                    cellKeuroFactory = new CellUnitFactory(cellKeuro);
                    cellEuroFactory = new CellUnitFactory(cellEuro);
                    cellDurationFactory = new CellUnitFactory(cellDuration);
                    cellInsertionFactory = new CellUnitFactory(cellInsertion);
                    break;
            }
            #endregion

            #region Total
            TNS.AdExpress.Domain.Level.GenericDetailLevel genericDetailLevel = webSession.GenericMediaDetailLevel;

            int nbColInTabData = tabData.GetLength(1);
            startDataColIndex++;
            startDataColIndexInit++;
            currentLineInTabResult = resultTable.AddNewLine(TNS.FrameWork.WebResultUI.LineType.total);
            //Libellé du total
            resultTable[currentLineInTabResult, levelLabelColIndex] = new CellLevel(-1, GestionWeb.GetWebWord(805, webSession.SiteLanguage), 0, currentLineInTabResult);
            CellLevel currentCellLevel0 = (CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
            if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellSponsorshipCreativesLink(currentCellLevel0, webSession, genericDetailLevel);
            if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel0, webSession, genericDetailLevel);
            if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
            {//Pas de colonnes total pour le tableau Autres dimensions X unités 
                if (showTotal && !computePercentage) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
                if (showTotal && computePercentage)
                {
                    resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, null);
                    ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                }
            }

            for (k = startDataColIndexInit; k <= nbCol; k++)
            {
                if (computePercentage)
                {
                    if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                    {
                        resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                        ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                    }
                    else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                    {
                        resultTable[currentLineInTabResult, k] = new CellPercent(0.0, null);
                        ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                    }

                }
                else
                {
                    if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                        resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                    else if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    {

                        //Keuro
                        resultTable[currentLineInTabResult, k] = cellKeuroFactory.Get(0.0);

                        //Euro
                        k++;
                        resultTable[currentLineInTabResult, k] = cellEuroFactory.Get(0.0);

                        //Durée
                        k++;
                        resultTable[currentLineInTabResult, k] = cellDurationFactory.Get(0.0);

                        //Spots
                        k++;
                        resultTable[currentLineInTabResult, k] = cellInsertionFactory.Get(0.0);

                        break;
                    }
                }
            }
            #endregion

            #region Tableau de résultat
            oldIdL1 = -1;
            oldIdL2 = -1;
            oldIdL3 = -1;
            oldIdL4 = -1;
            AdExpressCellLevel currentCellLevel1 = null;
            AdExpressCellLevel currentCellLevel2 = null;
            AdExpressCellLevel currentCellLevel3 = null;
            AdExpressCellLevel currentCellLevel4 = null;
            long currentL1Index = -1;
            long currentL2Index = -1;
            long currentL3Index = -1;
            long currentL4Index = -1;

            for (currentLine = 0; currentLine < nbLineInTabData; currentLine++)
            {

                #region On change de niveau L1
                if (tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX] != null && (Int64)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX] != oldIdL1)
                {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level1);

                    #region Totaux et autres dimensions en colonnnes à 0 
                    if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    {//Pas de colonnes total pour le tableau Autres dimensions X unités 
                        if (showTotal && !computePercentage) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
                        if (showTotal && computePercentage)
                        {
                            if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            {
                                resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, null);
                                ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                            }
                            else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                            {
                                resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, (CellUnit)resultTable[currentCellLevel0.LineIndexInResultTable, totalColIndex]);
                                ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                            }

                        }
                    }
                    for (k = startDataColIndexInit; k <= nbCol; k++)
                    {
                        if (computePercentage)
                        {
                            if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            {
                                resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                                ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                            }
                            else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                            {
                                resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentCellLevel0.LineIndexInResultTable, k]);
                                ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                            }

                        }
                        else
                        {
                            if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                                resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                            else if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                            {

                                //Keuro
                                resultTable[currentLineInTabResult, k] = cellKeuroFactory.Get(0.0);

                                //Euro
                                k++;
                                resultTable[currentLineInTabResult, k] = cellEuroFactory.Get(0.0);

                                //Durée
                                k++;
                                resultTable[currentLineInTabResult, k] = cellDurationFactory.Get(0.0);

                                //Spots
                                k++;
                                resultTable[currentLineInTabResult, k] = cellInsertionFactory.Get(0.0);
                                break;
                            }
                        }
                    }
                    #endregion

                    oldIdL1 = (Int64)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel(Int64.Parse(tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX].ToString()), tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.LABELL1_INDEX].ToString(), currentCellLevel0, 1, currentLineInTabResult, webSession, webSession.GenericMediaDetailLevel);
                    currentCellLevel1 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellSponsorshipCreativesLink(currentCellLevel1, webSession, genericDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel1, webSession, genericDetailLevel);
                    currentL1Index = currentLineInTabResult;
                    oldIdL2 = oldIdL3 = oldIdL4 = -1;

                    #region GAD
                    if (webSession.GenericMediaDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 1)
                    {
                        if (tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX] != null)
                        {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = Int64.Parse(tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX].ToString());
                        }
                    }
                    #endregion


                }
                #endregion

                #region On change de niveau L2
                if (tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX] != null && (Int64)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX] != oldIdL2)
                {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level2);

                    #region Totaux et autres dimensions en colonnnes à 0 
                    if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    {//Pas de colonnes total pour le tableau Autres dimensions X unités 
                        if (showTotal && !computePercentage) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
                        if (showTotal && computePercentage)
                        {
                            if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            {
                                resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, null);
                                ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                            }
                            else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                            {
                                resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, (CellUnit)resultTable[currentCellLevel1.LineIndexInResultTable, totalColIndex]);
                                ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                            }
                        }
                    }
                    for (k = startDataColIndexInit; k <= nbCol; k++)
                    {
                        if (computePercentage)
                        {
                            if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            {
                                resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                                ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                            }
                            else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                            {
                                resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentCellLevel1.LineIndexInResultTable, k]);
                                ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                            }
                        }
                        else
                        {
                            if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                                resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                            else if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                            {

                                //Keuro
                                resultTable[currentLineInTabResult, k] = cellKeuroFactory.Get(0.0);

                                //Euro
                                k++;
                                resultTable[currentLineInTabResult, k] = cellEuroFactory.Get(0.0);

                                //Durée
                                k++;
                                resultTable[currentLineInTabResult, k] = cellDurationFactory.Get(0.0);

                                //Spots
                                k++;
                                resultTable[currentLineInTabResult, k] = cellInsertionFactory.Get(0.0);
                                break;
                            }
                        }
                    }
                    #endregion

                    oldIdL2 = (Int64)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel(Int64.Parse(tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX].ToString()), tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.LABELL2_INDEX].ToString(), currentCellLevel1, 2, currentLineInTabResult, webSession, webSession.GenericMediaDetailLevel);
                    currentCellLevel2 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellSponsorshipCreativesLink(currentCellLevel2, webSession, genericDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel2, webSession, genericDetailLevel);
                    currentL2Index = currentLineInTabResult;
                    oldIdL3 = oldIdL4 = -1;

                    #region GAD
                    if (webSession.GenericMediaDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 2)
                    {
                        if (tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX] != null)
                        {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = Int64.Parse(tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX].ToString());
                        }
                    }
                    #endregion
                }
                #endregion

                #region On change de niveau L3
                if (tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX] != null && (Int64)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX] != oldIdL3)
                {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level3);

                    #region Totaux et autres dimensions en colonnnes à 0 
                    if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    {//Pas de colonnes total pour le tableau Autres dimensions X unités 
                        if (showTotal && !computePercentage) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
                        if (showTotal && computePercentage)
                        {
                            if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            {
                                resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, null);
                                ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                            }
                            else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                            {
                                resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, (CellUnit)resultTable[currentCellLevel2.LineIndexInResultTable, totalColIndex]);
                                ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                            }
                        }
                    }
                    for (k = startDataColIndexInit; k <= nbCol; k++)
                    {
                        if (computePercentage)
                        {
                            if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            {
                                resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                                ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                            }
                            else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                            {
                                resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentCellLevel2.LineIndexInResultTable, k]);
                                ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                            }
                        }
                        else
                        {
                            if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                                resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                            else if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                            {

                                //Keuro
                                resultTable[currentLineInTabResult, k] = cellKeuroFactory.Get(0.0);

                                //Euro
                                k++;
                                resultTable[currentLineInTabResult, k] = cellEuroFactory.Get(0.0);

                                //Durée
                                k++;
                                resultTable[currentLineInTabResult, k] = cellDurationFactory.Get(0.0);

                                //Spots
                                k++;
                                resultTable[currentLineInTabResult, k] = cellInsertionFactory.Get(0.0);
                                break;
                            }
                        }
                    }
                    #endregion

                    oldIdL3 = (Int64)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel(Int64.Parse(tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX].ToString()), (string)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.LABELL3_INDEX].ToString(), currentCellLevel2, 3, currentLineInTabResult, webSession, webSession.GenericMediaDetailLevel);
                    currentCellLevel3 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellSponsorshipCreativesLink(currentCellLevel3, webSession, genericDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel3, webSession, genericDetailLevel);
                    currentL3Index = currentLineInTabResult;
                    oldIdL4 = -1;

                    #region GAD
                    if (webSession.GenericMediaDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 3)
                    {
                        if (tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX] != null)
                        {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = Int64.Parse(tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX].ToString());
                        }
                    }
                    #endregion

                }
                #endregion

                #region On change de niveau L4
                if (tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX] != null && (Int64)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX] != oldIdL4)
                {
                    currentLineInTabResult = resultTable.AddNewLine(LineType.level4);

                    #region Totaux et autres dimensions en colonnnes à 0 
                    if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    {//Pas de colonnes total pour le tableau Autres dimensions X unités 
                        if (showTotal && !computePercentage) resultTable[currentLineInTabResult, totalColIndex] = cellUnitFactory.Get(0.0);
                        if (showTotal && computePercentage)
                        {
                            if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            {
                                resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, null);
                                ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                            }
                            else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                                resultTable[currentLineInTabResult, totalColIndex] = new CellPercent(0.0, (CellUnit)resultTable[currentCellLevel3.LineIndexInResultTable, totalColIndex]);
                            ((CellPercent)resultTable[currentLineInTabResult, totalColIndex]).StringFormat = "{0:percentWOSign}";
                        }
                    }
                    for (k = startDataColIndexInit; k <= nbCol; k++)
                    {
                        if (computePercentage)
                        {
                            if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            {
                                resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentLineInTabResult, totalColIndex]);
                                ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                            }
                            else if (webSession.PercentageAlignment == WebConstantes.Percentage.Alignment.vertical)
                            {
                                resultTable[currentLineInTabResult, k] = new CellPercent(0.0, (CellUnit)resultTable[currentCellLevel3.LineIndexInResultTable, k]);
                                ((CellPercent)resultTable[currentLineInTabResult, k]).StringFormat = "{0:percentWOSign}";
                            }
                        }
                        else
                        {
                            if (webSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                                resultTable[currentLineInTabResult, k] = cellUnitFactory.Get(0.0);
                            else if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                            {

                                //Keuro
                                resultTable[currentLineInTabResult, k] = cellKeuroFactory.Get(0.0);

                                //Euro
                                k++;
                                resultTable[currentLineInTabResult, k] = cellEuroFactory.Get(0.0);

                                //Durée
                                k++;
                                resultTable[currentLineInTabResult, k] = cellDurationFactory.Get(0.0);

                                //Spots
                                k++;
                                resultTable[currentLineInTabResult, k] = cellInsertionFactory.Get(0.0);
                                break;
                            }
                        }
                    }
                    #endregion

                    oldIdL4 = (Int64)tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX];
                    resultTable[currentLineInTabResult, levelLabelColIndex] = new AdExpressCellLevel(Int64.Parse(tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX].ToString()), tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.LABELL4_INDEX].ToString(), currentCellLevel3, 4, currentLineInTabResult, webSession, webSession.GenericMediaDetailLevel);
                    currentCellLevel4 = (AdExpressCellLevel)resultTable[currentLineInTabResult, levelLabelColIndex];
                    if (showCreative) resultTable[currentLineInTabResult, creativeColIndex] = new CellSponsorshipCreativesLink(currentCellLevel4, webSession, genericDetailLevel);
                    if (showInsertions) resultTable[currentLineInTabResult, insertionsColIndex] = new CellSponsorshipInsertionsLink(currentCellLevel4, webSession, genericDetailLevel);
                    currentL4Index = currentLineInTabResult;
                    oldIdL4 = -1;

                    #region GAD
                    if (webSession.GenericMediaDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == 4)
                    {
                        if (tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX] != null)
                        {
                            ((CellLevel)resultTable[currentLineInTabResult, levelLabelColIndex]).AddressId = Int64.Parse(tabData[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX].ToString());
                        }
                    }
                    #endregion

                }
                #endregion

                // On rajoute les valeurs aux cellules de la ligne	
                int startCol;
                if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    startCol = FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX;
                else startCol = FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX;

                for (k = startCol; k < nbColInTabData; k++)
                {
                    if (webSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                        resultTable.AffectValueAndAddToHierarchy(levelLabelColIndex, currentLineInTabResult, startDataColIndex + k - FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX, (double)tabData[currentLine, k]);
                    else resultTable.AffectValueAndAddToHierarchy(levelLabelColIndex, currentLineInTabResult, startDataColIndex + k - FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX, (double)tabData[currentLine, k]);
                }

            }
            #endregion

            return resultTable;
        }
        #endregion

        #region Calcul du nombre de ligne d'un tableau préformaté
        /// <summary>
        /// Obtient le nombre de ligne du tableau de résultat à partir d'un tableau préformaté
        /// </summary>
        /// <param name="tabData">Tableau préformaté</param>
        /// <returns>Nombre de ligne du tableau de résultat</returns>
        private static int GetNbLineFromPreformatedTableToResultTable(object[,] tabData)
        {

            #region Variables
            int nbLine = 0;
            int k;
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            Int64 oldIdL4 = -1;
            #endregion

            for (k = 0; k < tabData.GetLength(0); k++)
            {
                // Somme des L1
                if (tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX] != null && Int64.Parse(tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX].ToString()) != oldIdL1)
                {
                    oldIdL1 = Int64.Parse(tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX].ToString());
                    oldIdL4 = oldIdL3 = oldIdL2 = -1;
                    nbLine++;
                }

                // Somme des L2
                if (tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX] != null && Int64.Parse(tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX].ToString()) != oldIdL2)
                {
                    oldIdL2 = Int64.Parse(tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX].ToString());
                    oldIdL3 = -1;
                    oldIdL4 = -1;
                    nbLine++;
                }

                // Somme des L3
                if (tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX] != null && Int64.Parse(tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX].ToString()) != oldIdL3)
                {
                    oldIdL3 = Int64.Parse(tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX].ToString());
                    oldIdL4 = -1;
                    nbLine++;
                }

                // Somme des L4
                if (tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX] != null && Int64.Parse(tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX].ToString()) != oldIdL4)
                {
                    oldIdL4 = Int64.Parse(tabData[k, FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX].ToString());
                    nbLine++;
                }

            }
            return (nbLine);
        }
        #endregion

        #region Initialisation des indexes
        /// <summary>
        /// Initialisation des tableaux d'indexes et valeurs sur les groupes de séléection et médias
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="beginningPeriod">Date de début</param>
        /// <param name="nbCol">Nombre de colonnes du tableau</param>
        /// <param name="endPeriod">Date de fin</param>
        /// <param name="maxIndex">Index max des colonnex du tableaux</param>
        /// <param name="dimensionColIndex">(out Tableau d'indexes des dimensions en colonne</param>
        /// <param name="dimensionListForLabelSearch">Liste des libellés de colonnes</param>	
        internal static void InitIndexAndValues(WebSession webSession, Hashtable dimensionColIndex, ref long nbCol, string beginningPeriod, string endPeriod, ref int maxIndex, ref string dimensionListForLabelSearch)
        {
            switch (webSession.PreformatedTable)
            {

                case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media:
                    InitMediaTableIndex(webSession, dimensionColIndex, ref nbCol, beginningPeriod, endPeriod, ref maxIndex, ref dimensionListForLabelSearch);
                    break;

                case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period:
                    if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.weekly
                        || webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.monthly)
                    {
                        beginningPeriod = webSession.PeriodBeginningDate;
                        endPeriod = webSession.PeriodEndDate;
                    }
                    InitDatesTableIndex(webSession, dimensionColIndex, ref nbCol, beginningPeriod, endPeriod, ref maxIndex, ref dimensionListForLabelSearch);
                    break;

                case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units:
                    InitUnitsTableIndex(webSession, dimensionColIndex, ref nbCol, ref maxIndex, ref dimensionListForLabelSearch);
                    break;
            }
        }

        /// <summary>
        /// Construit le tableau des index des unités
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dimensionColIndex">Tableau des index</param>		
        /// <param name="maxIndex">Index max des colonnex du tableaux</param>
        /// <param name="nbCol">Nombre de colonnes</param>
        /// <param name="unitsListForLabelSearch">  units list for Label search</param>
        internal static void InitUnitsTableIndex(WebSession webSession, Hashtable dimensionColIndex, ref long nbCol, ref int maxIndex, ref string unitsListForLabelSearch)
        {
            maxIndex = FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX;
            int positionUnivers = 1;

            //Keuros		
            dimensionColIndex[UnitsInformation.DefaultKCurrency.GetHashCode()] = new GroupItemForTableResult(UnitsInformation.DefaultKCurrency.GetHashCode(), positionUnivers, maxIndex);
            unitsListForLabelSearch = UnitsInformation.DefaultKCurrency.GetHashCode().ToString();
            maxIndex++;


            //euro			
            dimensionColIndex[UnitsInformation.DefaultCurrency.GetHashCode()] = new GroupItemForTableResult(UnitsInformation.DefaultCurrency.GetHashCode(), positionUnivers, maxIndex);
            unitsListForLabelSearch += "," + UnitsInformation.DefaultCurrency.GetHashCode().ToString();
            maxIndex++;


            //Durée		
            dimensionColIndex[CustomerSessions.Unit.duration.GetHashCode()] = new GroupItemForTableResult(CustomerSessions.Unit.duration.GetHashCode(), positionUnivers, maxIndex);
            unitsListForLabelSearch += "," + CustomerSessions.Unit.duration.GetHashCode().ToString();
            maxIndex++;

            //Spot ( car parrainage Tv )			
            dimensionColIndex[CustomerSessions.Unit.spot.GetHashCode()] = new GroupItemForTableResult(CustomerSessions.Unit.spot.GetHashCode(), positionUnivers, maxIndex);
            unitsListForLabelSearch += "," + CustomerSessions.Unit.spot.GetHashCode().ToString();
            maxIndex++;

            if (dimensionColIndex != null && dimensionColIndex.Count > 0) nbCol = maxIndex;
        }

        /// <summary>
        /// Construit le tableau des index des dates
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="periodItemsIndex">Tableau des index</param>
        /// <param name="beginUserDate">Date de début</param>
        /// <param name="endUserDate">Date de fin</param>
        /// <param name="nbCol">Nombre de colonnes</param>
        /// <param name="maxIndex">Index max des colonnex du tableaux</param>
        /// <param name="datesListForLabelSearch">Dates list for label search</param>
        internal static void InitDatesTableIndex(WebSession webSession, Hashtable periodItemsIndex, ref long nbCol, string beginUserDate, string endUserDate, ref int maxIndex, ref string datesListForLabelSearch)
        {

            #region Création du tableau des mois ou semaine
            string tmpDate;
            int positionUnivers = 1;
            maxIndex = FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX;

            switch (webSession.DetailPeriod)
            {

                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    AtomicPeriodWeek currentWeek = new AtomicPeriodWeek(int.Parse(beginUserDate.Substring(0, 4)), int.Parse(beginUserDate.Substring(4, 2)));
                    AtomicPeriodWeek endWeek = new AtomicPeriodWeek(int.Parse(endUserDate.Substring(0, 4)), int.Parse(endUserDate.Substring(4, 2)));
                    endWeek.Increment();
                    while (!(currentWeek.Week == endWeek.Week && currentWeek.Year == endWeek.Year))
                    {

                        tmpDate = currentWeek.Year.ToString();
                        if (currentWeek.Week.ToString().Length < 2) tmpDate += "0" + currentWeek.Week.ToString();
                        else tmpDate += currentWeek.Week.ToString();
                        datesListForLabelSearch += tmpDate + ",";
                        periodItemsIndex[Int64.Parse(tmpDate)] = new GroupItemForTableResult(Int64.Parse(tmpDate), positionUnivers, maxIndex);
                        currentWeek.Increment();
                        maxIndex++;
                    }
                    if (datesListForLabelSearch != null && datesListForLabelSearch.Length > 0) datesListForLabelSearch = datesListForLabelSearch.Substring(0, datesListForLabelSearch.Length - 1);
                    break;
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                    DateTime dateCurrent = new DateTime(int.Parse(beginUserDate.Substring(0, 4)), int.Parse(beginUserDate.Substring(4, 2)), 1);
                    DateTime dateEnd = new DateTime(int.Parse(endUserDate.Substring(0, 4)), int.Parse(endUserDate.Substring(4, 2)), 1);
                    dateEnd = dateEnd.AddMonths(1);
                    while (!(dateCurrent.Month == dateEnd.Month && dateCurrent.Year == dateEnd.Year))
                    {

                        tmpDate = dateCurrent.Year.ToString();
                        if (dateCurrent.Month.ToString().Length < 2) tmpDate += "0" + dateCurrent.Month.ToString();
                        else tmpDate += dateCurrent.Month.ToString();
                        datesListForLabelSearch += tmpDate + ",";
                        periodItemsIndex[Int64.Parse(tmpDate)] = new GroupItemForTableResult(Int64.Parse(tmpDate), positionUnivers, maxIndex);
                        dateCurrent = dateCurrent.AddMonths(1);
                        maxIndex++;
                    }
                    if (datesListForLabelSearch != null && datesListForLabelSearch.Length > 0) datesListForLabelSearch = datesListForLabelSearch.Substring(0, datesListForLabelSearch.Length - 1);
                    break;
                case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
                    DateTime currentDateTime = DateString.YYYYMMDDToDateTime(beginUserDate);
                    DateTime endDate = DateString.YYYYMMDDToDateTime(endUserDate);
                    while (currentDateTime <= endDate)
                    {
                        datesListForLabelSearch += DateString.DateTimeToYYYYMMDD(currentDateTime) + ",";
                        periodItemsIndex[Int64.Parse(DateString.DateTimeToYYYYMMDD(currentDateTime))] = new GroupItemForTableResult(Int64.Parse(DateString.DateTimeToYYYYMMDD(currentDateTime)), positionUnivers, maxIndex);
                        currentDateTime = currentDateTime.AddDays(1);
                        maxIndex++;
                    }
                    if (datesListForLabelSearch != null && datesListForLabelSearch.Length > 0) datesListForLabelSearch = datesListForLabelSearch.Substring(0, datesListForLabelSearch.Length - 1);
                    break;
                default:
                    throw (new Exception("Impossible de construire le tableau d'index des dates"));
            }
            #endregion

            if (periodItemsIndex != null && periodItemsIndex.Count > 0) nbCol = maxIndex;
        }

        /// <summary>
        /// Initialise le tableau des index supports
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="mediaIndex">Tableau des index supports</param>
        /// <param name="nbCol">Nombre de colonnes</param>
        /// <param name="beginningPeriod">Date de début</param>
        /// <param name="endPeriod">Date de fin</param>
        /// <param name="maxIndex">Index max des colonnex du tableaux</param>
        /// <param name="mediaListForLabelSearch">List des méidas pour la recherche</param>
        internal static void InitMediaTableIndex(WebSession webSession, Hashtable mediaIndex, ref long nbCol, string beginningPeriod, string endPeriod, ref int maxIndex, ref string mediaListForLabelSearch)
        {

            #region Variables
            string tmp = "";
            //			int positionUnivers=1;
            string[] mediaList;
            maxIndex = FrameWorkResultConstantes.TvSponsorship.FIRST_RESULT_ITEM_COLUMN_INDEX;
            #endregion

            //			if(webSession.CompetitorUniversMedia.Count>0){

            if (webSession.CurrentUniversMedia != null && webSession.CurrentUniversMedia.Nodes.Count > 0)
            {
                // Chargement de la liste de support (média)
                tmp = webSession.GetSelection(webSession.CurrentUniversMedia, CustomerConstantes.Right.type.mediaAccess);
                mediaListForLabelSearch += tmp + ",";
                if (mediaListForLabelSearch != null && mediaListForLabelSearch.Length > 0) mediaListForLabelSearch = mediaListForLabelSearch.Substring(0, mediaListForLabelSearch.Length - 1);
                mediaList = tmp.Split(',');
                // Indexes des média (support)
                foreach (string currentMedia in mediaList)
                {
                    mediaIndex[Int64.Parse(currentMedia)] = new GroupItemForTableResult(Int64.Parse(currentMedia), 1, maxIndex);
                    maxIndex++;
                }
            }
            //		}	
            else
            {
                if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS)
                {
                    DataSet ds = CampaignAnalysisDAL.GetMediaData(webSession, beginningPeriod, endPeriod);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            mediaListForLabelSearch += dr["id_media"].ToString() + ",";
                            mediaIndex[Int64.Parse(dr["id_media"].ToString())] = new GroupItemForTableResult(Int64.Parse(dr["id_media"].ToString()), 1, maxIndex);
                            maxIndex++;
                        }
                        if (mediaListForLabelSearch != null && mediaListForLabelSearch.Length > 0) mediaListForLabelSearch = mediaListForLabelSearch.Substring(0, mediaListForLabelSearch.Length - 1);
                    }
                }

            }
            if (mediaIndex != null && mediaIndex.Count > 0) nbCol = maxIndex;

        }
        #endregion

        #region Insert les valeurs des niveaux de détail média
        /// <summary>
        /// Calcule les résultats pour les niveaux de détail média
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dt">Table de données</param>
        /// <param name="tabResult">Table de résultats</param>
        /// <param name="dimensionColIndex">Tableau d'indexes</param>
        /// <param name="nbCol">Nombre de colonnes</param>
        /// <param name="nbLineInNewTable">Nombre de lignes dans le tableau de résultats</param>
        private static void SetMediaDetailLevelValues(WebSession webSession, DataTable dt, object[,] tabResult, ref long nbLineInNewTable, Hashtable dimensionColIndex, long nbCol)
        {

            #region Variables
            //			double unit;
            long oldIdL1 = -1;
            long oldIdL2 = -1;
            long oldIdL3 = -1;
            long oldIdL4 = -1;
            Int64 currentLine = -1;
            int k;
            bool changeLine = false;

            #endregion

            #region Tableau de résultat
            foreach (DataRow currentRow in dt.Rows)
            {

                if (webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 1) >= 0 && webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 1) != oldIdL1) changeLine = true;
                if (!changeLine && webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 2) >= 0 && webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 2) != oldIdL2) changeLine = true;
                if (!changeLine && webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 3) >= 0 && webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 3) != oldIdL3) changeLine = true;
                if (!changeLine && webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 4) >= 0 && webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 4) != oldIdL4) changeLine = true;

                #region On change de ligne
                if (changeLine)
                {
                    currentLine++;

                    // identifiants de L1 
                    if (webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 1) >= 0)
                    {
                        oldIdL1 = webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 1);
                        tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL1_INDEX] = oldIdL1;
                        tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.LABELL1_INDEX] = webSession.GenericMediaDetailLevel.GetLabelValue(currentRow, 1);
                    }

                    // identifiants de L2 
                    if (webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 2) >= 0)
                    {
                        oldIdL2 = webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 2);
                        tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL2_INDEX] = oldIdL2;
                        tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.LABELL2_INDEX] = webSession.GenericMediaDetailLevel.GetLabelValue(currentRow, 2);
                    }

                    // identifiants de L3 
                    if (webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 3) >= 0)
                    {
                        oldIdL3 = webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 3);
                        tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL3_INDEX] = oldIdL3;
                        tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.LABELL3_INDEX] = webSession.GenericMediaDetailLevel.GetLabelValue(currentRow, 3);
                    }

                    // identifiants de L4 
                    if (webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 4) >= 0)
                    {
                        oldIdL4 = webSession.GenericMediaDetailLevel.GetIdValue(currentRow, 4);
                        tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.IDL4_INDEX] = oldIdL4;
                        tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.LABELL4_INDEX] = webSession.GenericMediaDetailLevel.GetLabelValue(currentRow, 4);
                    }

                    // Totaux  et dimensions en colonnes à 0
                    for (k = FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX; k < nbCol; k++)
                    {
                        tabResult[currentLine, k] = (double)0.0;
                    }

                    if (webSession.GenericMediaDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                    {
                        try
                        {
                            if (currentRow["id_address"] != null) tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.ADDRESS_COLUMN_INDEX] = Int64.Parse(currentRow["id_address"].ToString());
                        }
                        catch (Exception)
                        {

                        }

                    }

                    changeLine = false;
                }
                #endregion


                //Insertion valeurs dans cellule
                SetTableCellValue(webSession, tabResult, currentRow, currentLine, dimensionColIndex);

            }
            #endregion

            nbLineInNewTable = currentLine + 1;
        }

        #endregion

        #region Insert des valeurs dans les cellules du tableau
        /// <summary>
        /// Insert des valeurs dans les celules du tableau de résulats
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="tabResult">Tableau de résultats</param>
        /// <param name="currentRow">Ligne courante dans table de données (DataTable)</param>
        /// <param name="currentLine">Ligne courante dans tableau de résultats </param>
        /// <param name="dimensionColIndex"></param>
        private static void SetTableCellValue(WebSession webSession, object[,] tabResult, DataRow currentRow, long currentLine, Hashtable dimensionColIndex)
        {
            double unit;

            switch (webSession.PreformatedTable)
            {

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media:

                    unit = double.Parse(currentRow[SQLGenerator.GetUnitAlias(webSession)].ToString());

                    // Ecriture du résultat des dimensions en colonnes
                    if (dimensionColIndex != null && dimensionColIndex.Count > 0)
                        tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[Int64.Parse(currentRow["id_media"].ToString())]).IndexInResultTable] = unit;

                    // Ecriture du résultat du total (somme)
                    tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX] = (double)tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX] + unit;
                    break;

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period:
                    unit = double.Parse(currentRow[SQLGenerator.GetUnitAlias(webSession)].ToString());

                    // Ecriture du résultat des dimensions en colonnes
                    if (dimensionColIndex != null && dimensionColIndex.Count > 0)
                        tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[Int64.Parse(currentRow["date_num"].ToString())]).IndexInResultTable] = unit;

                    // Ecriture du résultat du total (somme)
                    tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX] = (double)tabResult[currentLine, FrameWorkResultConstantes.TvSponsorship.TOTAL_COLUMN_INDEX] + unit;
                    break;

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units:

                    // Ecriture du résultat des keuros en colonnes
                    unit = double.Parse(currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString());
                    tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[UnitsInformation.List[UnitsInformation.DefaultKCurrency].Id.GetHashCode()]).IndexInResultTable] = unit;

                    // Ecriture du résultat des euro en colonnes
                    unit = double.Parse(currentRow[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString());
                    if (dimensionColIndex != null && dimensionColIndex.Count > 0)
                        tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.GetHashCode()]).IndexInResultTable] = unit;

                    // Ecriture du résultat des durée en colonnes
                    unit = double.Parse(currentRow[UnitsInformation.List[CustomerSessions.Unit.duration].Id.ToString()].ToString());
                    if (dimensionColIndex != null && dimensionColIndex.Count > 0)
                        tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[UnitsInformation.List[CustomerSessions.Unit.duration].Id.GetHashCode()]).IndexInResultTable] = unit;

                    // Ecriture du résultat des spots en colonnes
                    unit = double.Parse(currentRow[UnitsInformation.List[CustomerSessions.Unit.insertion].Id.ToString()].ToString());
                    if (dimensionColIndex != null && dimensionColIndex.Count > 0)
                        tabResult[currentLine, ((GroupItemForTableResult)dimensionColIndex[UnitsInformation.List[CustomerSessions.Unit.spot].Id.GetHashCode()]).IndexInResultTable] = unit;

                    break;
            }
        }
        #endregion

        #region Chargements des libellés de colonnes
        /// <summary>
        /// Définit les libéllés des entêtes de colonnes
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="headers">entêtes</param>
        /// <param name="dimensionColIndex">Tableau des indexes des dimensions en colonnes</param>
        /// <param name="dimensionListForLabelSearch">Identifiants des éléments  en colonnes pour la rechercher de libellés</param>
        private static void SetColumnsLabels(WebSession webSession, Headers headers, Hashtable dimensionColIndex, string dimensionListForLabelSearch)
        {
            string[] dimendionIdList = null;
            HeaderGroup headerGroupTmp = null;
            int m = 1;
            int i = 0;
            AtomicPeriodWeek week;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            switch (webSession.PreformatedTable)
            {

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media:

                    //Libellés supports 
                    headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(804, webSession.SiteLanguage), true, FrameWorkResultConstantes.TvSponsorship.START_ID_GROUP + m);//TODO vérifier index de départ															

                    if (dimensionListForLabelSearch != null && dimensionListForLabelSearch.Length > 0)
                    {
                        dimendionIdList = dimensionListForLabelSearch.Split(',');
                        ClassificationDB.MediaBranch.PartialMediaListDataAccess mediaLabelList = new ClassificationDB.MediaBranch.PartialMediaListDataAccess(dimensionListForLabelSearch, webSession.DataLanguage, webSession.Source);

                        foreach (string currentMedia in dimendionIdList)
                        {
                            headerGroupTmp.Add(new Header(true, mediaLabelList[Int64.Parse(currentMedia)], Int64.Parse(currentMedia)));
                        }
                        headers.Root.Add(headerGroupTmp);
                    }
                    break;

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period:
                    //Libellés périodes
                    headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(1755, webSession.SiteLanguage), true, FrameWorkResultConstantes.TvSponsorship.START_ID_GROUP + m);//TODO vérifier index de départ

                    if (dimensionListForLabelSearch != null && dimensionListForLabelSearch.Length > 0)
                    {
                        dimendionIdList = dimensionListForLabelSearch.Split(',');
                        string dateLabel = null;

                        foreach (string currentDate in dimendionIdList)
                        {

                            switch (webSession.DetailPeriod)
                            {

                                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                                    dateLabel = Dates.DateToString(Dates.GetPeriodBeginningDate(currentDate, webSession.PeriodType), webSession.SiteLanguage)
                                    + "-" + Dates.DateToString(Dates.GetPeriodEndDate(currentDate, webSession.PeriodType), webSession.SiteLanguage);
                                    break;
                                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                                    i++;
                                    if (i == 1 || i == dimendionIdList.GetLongLength(0))
                                    {//Si premier ou dernier mois de l'univers
                                        dateLabel = MonthString.GetCharacters(int.Parse(currentDate.Substring(4, 2)), cultureInfo, 9) + " " + currentDate.Substring(0, 4);
                                        if (webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.dateToDate)
                                        {
                                            dateLabel += (i == 1) ? "<br>" + DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodBeginningDate, webSession.SiteLanguage) : "<br>" + DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodEndDate, webSession.SiteLanguage);
                                            if (dimendionIdList.GetLongLength(0) == 1 && !webSession.PeriodBeginningDate.Equals(webSession.PeriodEndDate))
                                                dateLabel = dateLabel + " - " + DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodEndDate, webSession.SiteLanguage);
                                        }
                                    }
                                    else dateLabel = MonthString.GetCharacters(int.Parse(currentDate.Substring(4, 2)), cultureInfo, 9) + " " + currentDate.Substring(0, 4);
                                    break;
                                case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
                                    dateLabel = DateString.YYYYMMDDToDD_MM_YYYY(currentDate, webSession.SiteLanguage);//TODO vérifier la forme du libellé date retourné par la fonction DateString.YYYYMMDDToDD_MM_YYYY
                                    break;
                            }

                            if (dateLabel != null) headerGroupTmp.Add(new Header(true, dateLabel, Int64.Parse(currentDate)));
                            dateLabel = null;
                        }
                        headers.Root.Add(headerGroupTmp);
                    }
                    break;

                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units:
                    //Libellés unités
                    headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(2061, webSession.SiteLanguage), true, FrameWorkResultConstantes.TvSponsorship.START_ID_GROUP + m);//TODO vérifier index de départ

                    if (dimensionListForLabelSearch != null && dimensionListForLabelSearch.Length > 0)
                    {
                        dimendionIdList = dimensionListForLabelSearch.Split(',');
                        string unitLabel = null;
                        WebConstantes.CustomerSessions.Unit unit;

                        foreach (string currentUnit in dimendionIdList)
                        {
                            unit = (WebConstantes.CustomerSessions.Unit)int.Parse(currentUnit);

                            switch (unit)
                            {
                                case WebConstantes.CustomerSessions.Unit.euro:
                                    unitLabel = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CustomerSessions.Unit.euro].WebTextId, webSession.SiteLanguage));
                                    break;
                                case WebConstantes.CustomerSessions.Unit.kEuro:
                                    unitLabel = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CustomerSessions.Unit.kEuro].WebTextId, webSession.SiteLanguage));
                                    break;
                                case WebConstantes.CustomerSessions.Unit.duration:
                                    unitLabel = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CustomerSessions.Unit.duration].WebTextId, webSession.SiteLanguage));
                                    break;
                                case WebConstantes.CustomerSessions.Unit.spot:
                                    unitLabel = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CustomerSessions.Unit.spot].WebTextId, webSession.SiteLanguage));
                                    break;
                            }

                            if (unitLabel != null) headerGroupTmp.Add(new Header(true, unitLabel, Int64.Parse(currentUnit)));
                            unitLabel = null;
                        }
                        headers.Root.Add(headerGroupTmp);
                    }
                    break;
            }
        }
        #endregion

        #endregion
    }
}
