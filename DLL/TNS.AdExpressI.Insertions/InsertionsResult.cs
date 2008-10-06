using System;
using System.Collections.Generic;
using System.Text;

using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using System.Windows.Forms;
using TNS.AdExpressI.Insertions.Exceptions;
using TNS.AdExpressI.Insertions.DAL;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork.WebResultUI;
using System.Data;
using System.Collections;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Insertions.Cells;


namespace TNS.AdExpressI.Insertions
{
    public abstract class InsertionsResult : IInsertionsResult
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current Module Id
        /// </summary>
        protected Module _module;
        /// <summary>
        /// Data Access Layer
        /// </summary>
        protected IInsertionsDAL _dalLayer;
        #endregion

        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId)
        {
            _session = session;
            _module = ModulesList.GetModule(moduleId);
            object[] param = new object[1];
            param[0] = session;
            param[1] = moduleId;
            _dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + "TNS.AdExpressI.Insertions.DAL.Default", "TNS.AdExpressI.Insertions.Default.DAL.InsertionsDAL", false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);
        }
        #endregion

        #region GetVehicles
        /// <summary>
        /// Get vehicles matching filters and which has data
        /// </summary>
        /// <param name="filters">Filters to apply (id1,id2,id3,id4</param>
        /// <returns>List of vehicles with data</returns>
        public List<VehicleInformation> GetPresentVehicles(string filters, int universId)
        {
            List<VehicleInformation> vehicles = new List<VehicleInformation>();

            DateTime dateBegin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            DateTime dateEnd = FctUtilities.Dates.getPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
            int iDateBegin = Convert.ToInt32(dateBegin.ToString("yyyyMMdd"));
            int iDateEnd = Convert.ToInt32(dateEnd.ToString("yyyyMMdd"));

            switch (_module.Id)
            {
                case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                case CstWeb.Module.Name.ANALYSE_POTENTIELS:
                    Int64 id = ((LevelInformation)_session.SelectionUniversMedia.Nodes[0].Tag).ID;
                    vehicles.Add(VehiclesInformation.Get(id));
                    break;
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                    string[] ids = filters.Split(',');
                    vehicles = GetVehicles(Convert.ToInt64(ids[0]), Convert.ToInt64(ids[1]), Convert.ToInt64(ids[2]), Convert.ToInt64(ids[3]));
                    break;
                case CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS:
                case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                    vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.tv));
                    break;
            }

            if (vehicles.Count <= 0)
            {
                vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.others));
                vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.directMarketing));
                vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.internet));
                vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack));
                vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.press));
                vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.outdoor));
                vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.radio));
                vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.tv));

            }

            return _dalLayer.GetPresentVehicles(vehicles, filters, iDateBegin, iDateEnd, universId, _module);

        }
        #endregion

        #region Liste of vehicles matching filters
        /// <summary>
        /// Get List of vehicles matching filters
        /// </summary>
        /// <param name="idLevel1">Level 1 filter</param>
        /// <param name="idLevel2">Level 2 filter</param>
        /// <param name="idLevel3">Level 3 filter</param>
        /// <param name="idLevel4">Level 4 filter</param>
        /// <returns>List of vehicles matching filters</returns>
        protected List<VehicleInformation> GetVehicles(Int64 idLevel1, Int64 idLevel2, Int64 idLevel3, Int64 idLevel4)
        {

            Dictionary<DetailLevelItemInformation, Int64> filters;
            List<VehicleInformation> vehicles = new List<VehicleInformation>();
            Int64[] vIds = null;

            try
            {
                //Get media classification filters
                filters = FctUtilities.MediaDetailLevel.GetFilters(_session, idLevel1, idLevel2, idLevel3, idLevel4);
                vIds = _dalLayer.GetVehiclesIds(filters);
                foreach (Int64 id in vIds)
                {
                    vehicles.Add(VehiclesInformation.Get(id));
                }
            }
            catch (System.Exception ex)
            {
                throw (new InsertionsException("Unable to get media classifications level matching the filters.", ex));
            }
            return vehicles;
        }
        #endregion

        #region GetInsertions
        public virtual ResultTable GetInsertions(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId)
        {
            ResultTable data = null;

            #region Data Access
            if (vehicle == null)
                return null;
            DataSet ds = _dalLayer.GetInsertionsData(vehicle, fromDate, toDate, universId, filters);

            if (ds == null || ds.Equals(DBNull.Value) || ds.Tables[0] == null || ds.Tables[0].Rows.Count ==0)
                return null;
            #endregion

            DataTable dt = ds.Tables[0];

            #region Init ResultTable
            List<DetailLevelItemInformation> levels = new List<DetailLevelItemInformation>();
            foreach (DetailLevelItemInformation d in _session.DetailLevel.Levels) {
                levels.Add((DetailLevelItemInformation)d);
            }
            Int64 idColumnsSet = WebApplicationParameters.InsertionsDetail.GetDetailColumnsId(vehicle.DatabaseId, _module.Id);
            //Data Keys
            List<GenericColumnItemInformation> keys = WebApplicationParameters.GenericColumnsInformation.GetKeys(idColumnsSet);
            List<string> keyIdName = new List<string>();
            List<string> keyLabelName = new List<string>();
            GetKeysColumnNames(dt, keys, keyIdName, keyLabelName);
            //Line Number
            Int64 nbLine = GetLineNumber(dt, levels, keyIdName);
            //Data Columns
            List<GenericColumnItemInformation> columns = _session.GenericInsertionColumns.Columns;
            List<Cell> cells = new List<Cell>();
            List<string> columnsName = GetColumnsName(dt, columns, cells);
            //Result Table init
            data = new ResultTable(nbLine, GetHeaders(vehicle, columns));
            SetLine setLine = null;
            switch (vehicle.Id) {
                case CstDBClassif.Vehicles.names.directMarketing:
                case CstDBClassif.Vehicles.names.internationalPress:
                case CstDBClassif.Vehicles.names.press:
                case CstDBClassif.Vehicles.names.outdoor:
                    setLine = new SetLine(SetAggregLine);
                    break;
                default:
                    setLine = new SetLine(SetRawLine);
                    break;
            }
            #endregion

            #region Table fill
            LineType[] lineTypes = new LineType[4]{LineType.level1, LineType.level2, LineType.level3, LineType.level4};
            Int64[] oldIds = new Int64[levels.Count];
            for (int i = 0; i < oldIds.Length; i++) { oldIds[i] = -1; }
            Int64[] cIds = new Int64[levels.Count];

            Int64[] oldKeyIds = new Int64[keys.Count];
            for (int i = 0; i < oldKeyIds.Length; i++) { oldKeyIds[i] = -1; }
            Int64[] cKeyIds = new Int64[keys.Count];

            string label = string.Empty;
            bool isNewInsertion = true;
            Int64 cLine = 0;

            foreach (DataRow row in dt.Rows) {

                //Detail levels
                for (int i = 0; i < oldIds.Length; i++) {
                    cIds[i] = Convert.ToInt64(row[levels[i].DataBaseAliasIdField]);
                    if (cIds[i] != oldIds[i]) {
                        oldIds[i] = cIds[i];                        
                        if (i < oldIds.Length - 1) {
                            oldIds[i + 1] = -1;
                            for (int j = 0; j < oldKeyIds.Length; j++) { oldKeyIds[j] = -1; }
                        }

                        //Set current level
                        cLine = data.AddNewLine(lineTypes[i]);
                        data[cLine, 1] = new AdExpressCellLevel(cIds[i], row[keyLabelName[i]].ToString(), null, i + 1, cLine, _session, _session.DetailLevel);

                        for (int j = 2; j <= data.DataColumnsNumber; j++) {
                            data[cLine, j] = new CellEmpty();
                        }

                    }
                }

                //Insertion Keys
                for (int i = 0; i < oldKeyIds.Length; i++) {
                    cKeyIds[i] = Convert.ToInt64(row[keyIdName[i]]);
                    if (cKeyIds[i] != oldKeyIds[i]) {
                        oldKeyIds[i] = cKeyIds[i];
                        if (i < oldKeyIds.Length - 1) {
                            oldKeyIds[i + 1] = -1;
                        }
                        cLine = data.AddNewLine(lineTypes[levels.Count]);
                    }
                }


                setLine(data, row, cLine, columns, columnsName, cells);

            }

            #endregion

            return data;
        }

        protected delegate void SetLine(ResultTable tab, DataRow row, Int64 cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells);
        protected void SetRawLine(ResultTable tab, DataRow row, Int64 cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells)
        {
            int i = -1;
            int j = 0;
            foreach (GenericColumnItemInformation g in columns) {

                i++;
                j++;
                if (cells[i] is CellUnit) {
                    if (tab[cLine, j] == null){
                        tab[cLine, j] = ((CellUnit)cells[i]).Clone(Convert.ToDouble(row[columnsName[i]]));
                    }
                    else{
                        ((CellUnit)tab[cLine, j]).Add(Convert.ToDouble(row[columnsName[i]]));
                    }
                }
                else {
                    if (tab[cLine, j] == null) {
                        tab[cLine, j] = new CellLabel(row[columnsName[i]].ToString());
                    }
                    else {
                        ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}",((CellLabel)tab[cLine, j]).Label, row[columnsName[i]].ToString());
                    }
                }

            }
        }
        protected void SetAggregLine(ResultTable tab, DataRow row, Int64 cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells) {
            CellInsertionInformation c;
            if (tab[cLine, 1] == null) {
                tab[cLine, 1] = c = new CellInsertionInformation(columns, columnsName, cells);
            }
            else {
                c = (CellInsertionInformation)tab[cLine, 1];
            }
            c.Add(row);

        }
        #endregion

        #region GetLineNumber
        /// <summary>
        /// Get table line numbers
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <param name="levels">Detail levels</param>
        /// <param name="keys">Data Key</param>
        /// <returns>Number of line in final table</returns>
        protected Int64 GetLineNumber(DataTable dt, List<DetailLevelItemInformation> levels, List<string> keys) {

            Int64 nbLine = 0;

            Int64[] oldIds = new Int64[levels.Count];
            for (int i = 0; i < oldIds.Length; i++) { oldIds[i] = -1; }
            Int64[] cIds = new Int64[levels.Count];

            Int64[] oldKeyIds = new Int64[keys.Count];
            for (int i = 0; i < oldKeyIds.Length; i++) { oldKeyIds[i] = -1; }
            Int64[] cKeyIds = new Int64[keys.Count];

            foreach (DataRow row in dt.Rows) {

                //Detail levels
                for (int i = 0; i < oldIds.Length; i++) {
                    cIds[i] = Convert.ToInt64(row[levels[i].DataBaseAliasIdField]);
                    if (cIds[i] != oldIds[i]) {
                        oldIds[i] = cIds[i];
                        nbLine++;
                        if (i < oldIds.Length - 1) {
                            oldIds[i + 1] = -1;
                            for (int j = 0; j < oldKeyIds.Length; j++) { oldKeyIds[j] = -1; }
                        }
                    }
                }

                //Insertion Keys
                for (int i = 0; i < oldKeyIds.Length; i++) {
                    cKeyIds[i] = Convert.ToInt64(row[keys[i]]);
                    if (cKeyIds[i] != oldKeyIds[i]) {
                        oldKeyIds[i] = cKeyIds[i];
                        nbLine++;
                        if (i < oldKeyIds.Length - 1) {
                            oldKeyIds[i + 1] = -1;
                        }
                    }
                }


            }

            return nbLine;

        }
        #endregion

        #region GetColumnsName
        /// <summary>
        /// Get Data Column Names for data to display
        /// </summary>
        /// <param name="columns">List of columns</param>
        /// <returns>List of data column names</returns>
        protected List<string> GetColumnsName(DataTable dt, List<GenericColumnItemInformation> columns, List<Cell> cells) {

            List<string> names = new List<string>();
            string name = string.Empty;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");

            foreach (GenericColumnItemInformation g in columns) {

                if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0) {
                    name = g.DataBaseAliasField.ToUpper();
                }
                else if (g.DataBaseField != null && g.DataBaseField.Length > 0) {
                    name = g.DataBaseField.ToUpper();
                }

                if (dt.Columns.Contains(name) && !names.Contains(name))
                    names.Add(name);

                switch (g.CellType) {
                    case "TNS.FrameWork.WebResultUI.CellLabel":
                        cells.Add(new CellLabel(string.Empty));
                        break;
                    default:
                        
                        Type type = assembly.GetType(g.CellType);
                        Cell cellUnit = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                        cellUnit.StringFormat = g.StringFormat;
                        cells.Add(cellUnit);
                        break;
                }
            }

            return names;
        }
        #endregion

        #region GetKeysName
        /// <summary>
        /// Get Data Column Names for data keys
        /// </summary>
        /// <param name="columns">List of key columns</param>
        /// <returns>List of key column names</returns>
        protected void GetKeysColumnNames(DataTable dt, List<GenericColumnItemInformation> columns, List<string> idsColumn, List<string> labelsColumn) {

            string idName = string.Empty;
            string labelName = string.Empty;

            foreach (GenericColumnItemInformation g in columns) {
                //Init stirngs
                idName = string.Empty;
                labelName = string.Empty;

                //Data Base ID
                if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0) {
                    labelName = idName = g.DataBaseAliasIdField.ToUpper();
                }
                else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0) {
                    labelName = idName = g.DataBaseIdField.ToUpper();
                }
                //Database Label
                if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0 ) {
                    labelName = g.DataBaseAliasField.ToUpper();
                }
                else if (g.DataBaseField != null && g.DataBaseField.Length > 0) {
                    labelName = g.DataBaseField.ToUpper();
                }

                if (idName.Length < 1) {
                    idName = labelName;
                }

                if (dt.Columns.Contains(idName) && !idsColumn.Contains(idName))
                    idsColumn.Add(idName);
                if (dt.Columns.Contains(labelName) && !labelsColumn.Contains(labelName))
                    labelsColumn.Add(labelName);
            }

        }
        #endregion

        #region GetHeaders
        /// <summary>
        /// Get Table headers
        /// </summary>
        /// <param name="vehicle">Current vehicle</param>
        /// <param name="columns">Data columns to display</param>
        /// <returns>Table headers</returns>
        protected Headers GetHeaders(VehicleInformation vehicle, List<GenericColumnItemInformation> columns) {

            Headers root = new Headers();

            switch (vehicle.Id) {
                case CstDBClassif.Vehicles.names.directMarketing:
                case CstDBClassif.Vehicles.names.adnettrack:
                case CstDBClassif.Vehicles.names.internationalPress:
                case CstDBClassif.Vehicles.names.outdoor:
                case CstDBClassif.Vehicles.names.press:
                    root.Root.Add(new Header(string.Empty));
                    break;
                case CstDBClassif.Vehicles.names.others:
                case CstDBClassif.Vehicles.names.radio:
                case CstDBClassif.Vehicles.names.tv:
                    for (int i = 0; i < columns.Count; i++) {
                        root.Root.Add(new Header(GestionWeb.GetWebWord(columns[i].WebTextId, _session.SiteLanguage), columns[i].Id.GetHashCode()));
                    }
                    break;
            }

            return root;

        }
        #endregion

        #region Init cells
        protected List<Cell> GetCells(List<GenericColumnItemInformation> columns) {

            List<Cell> cells = new List<Cell>();
            Cell cell = null;
            int i = -1;

            foreach (GenericColumnItemInformation g in columns) {

                i++;
                try {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");
                    Type type = assembly.GetType(g.CellType);
                    cell = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                    cell.StringFormat = g.StringFormat;
                }
                catch (Exception e) {
                    cells[i] = new CellLabel(string.Empty);
                }

            }

            return cells;
        }
        #endregion
    }
}