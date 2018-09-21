using System;
using System.Data;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;

namespace TNS.AdExpressI.Classification.DAL.Turkey
{
  public  class ClassificationDAL : DAL.ClassificationDAL
    {
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
