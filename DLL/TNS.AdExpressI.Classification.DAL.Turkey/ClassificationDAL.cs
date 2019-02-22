using System;
using System.Data;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;

namespace TNS.AdExpressI.Classification.DAL.Turkey
{
  public  class ClassificationDAL : DAL.ClassificationDAL
    {
        #region Constructor
        public ClassificationDAL(WebSession session) : base(session)
        {
        }

        public ClassificationDAL(WebSession session, Dimension dimension) : base(session, dimension)
        {
        }

        public ClassificationDAL(WebSession session, Dimension dimension, string vehicleList) : base(session, dimension, vehicleList)
        {
        }

        public ClassificationDAL(WebSession session, GenericDetailLevel genericDetailLevel, string vehicleList) : base(session, genericDetailLevel, vehicleList)
        {
        }
        #endregion

        public override DataSet GetItems(long levelId, string wordToSearch)
        {
            //string classificationLevelLabel = UniverseLevels.Get(levelId).TableName;
            string classificationLevelLabel = UniverseLevels.Get(levelId).TableName;
            //Calling the engine which compute data
            if (string.IsNullOrEmpty(_dBSchema))
                throw (new ArgumentException("Invalid dBSchema parameter"));//Excepted for france data base, can be null for other country         
            ClassificationItemsDAL engineDal = new ClassificationItemsDAL(_session, _dimension);
            engineDal.DBSchema = _dBSchema;
            engineDal.Filters = _filters;
            engineDal.FilterWithProductSelection = _filterWithProductSelection;
            engineDal.DataSource = _dataSource;
            engineDal.VehiclesId = _vehicleList;
            return engineDal.GetItems(classificationLevelLabel, wordToSearch);
        }

        public override DataSet GetItems(long levelId, string selectedClassificationItemsIds, long selectedLevelId)
        {
            string classificationLevelLabel = UniverseLevels.Get(levelId).TableName;
            string selectedClassificationLevelLabel = UniverseLevels.Get(selectedLevelId).TableName;

            //Calling the engine which compute data
            if (string.IsNullOrEmpty(_dBSchema))
                throw (new ArgumentException("Invalid dBSchema parameter"));//Excepted for france data base, can be null for other country         
            ClassificationItemsDAL engineDal = new ClassificationItemsDAL(_session, _dimension);
            engineDal.DBSchema = _dBSchema;
            engineDal.Filters = _filters;
            engineDal.FilterWithProductSelection = _filterWithProductSelection;
            engineDal.DataSource = _dataSource;
            engineDal.VehiclesId = _vehicleList;
            return engineDal.GetItems(classificationLevelLabel, selectedClassificationItemsIds, selectedClassificationLevelLabel);
        }

        public override DataSet GetSelectedItems(string classificationLevelLabel, string idList)
        {
            //Calling the engine which compute data
            if (string.IsNullOrEmpty(_dBSchema))
                throw (new ArgumentException("Invalid dBSchema parameter"));//Excepted for france data base, can be null for other country         
            ClassificationItemsDAL engineDal = new ClassificationItemsDAL(_session, _dimension);
            engineDal.DBSchema = _dBSchema;
            engineDal.FilterWithProductSelection = _filterWithProductSelection;
            engineDal.DataSource = _dataSource;
            engineDal.VehiclesId = _vehicleList;
            return engineDal.GetSelectedItems(classificationLevelLabel, idList);
        }

        public override DataSet GetHealthMediaType()
        {
            throw new NotImplementedException();
        }

        #region Get Media Type
        /// <summary>
        /// This method provides SQL queries to get the media classification level's items.
        /// The data are filtered by customer's media rights and selected working set.		
        /// </summary>
        /// <returns>Data table 
        /// with media's identifiers ("idMediaType" column) and media's labels ("mediaType" column).
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Impossible to execute query
        /// </exception>
        public override DataSet GetMediaType()
        {
            //Calling the engine which compute data
            VehiclesDAL engineDal = new VehiclesDAL(_session) {DataSource = _dataSource};
            return engineDal.GetData();
        }
        #endregion


    }
}
