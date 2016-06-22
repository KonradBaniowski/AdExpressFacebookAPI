using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Repository
{
    public class DataFacebookRepository : GenericRepository<DataFacebook>, IDataFacebookRepository
    {
        public DataFacebookRepository(FacebookContext context) : base(context)
        {
        }

        public List<DataFacebook> GetDataFacebook()
        {

            var query = (from d in context.DataFacebook
                         select d);

            return new List<DataFacebook>();
        }

        //public List<DataFacebook> GetDataFacebook(Dictionary<int, List<RightDomain>> ProductRights,
        //    Dictionary<int, List<RightDomain>> MediasRights,
        //    DateTime Begin, DateTime End, List<int> Advertiser, List<int> Brand)
        //{
        //    var query = (from d in context.DataFacebook
        //                 select d);

        //    //query.ApplyRight(right)
        //    foreach (var item in ProductRights)
        //    {
        //        if (item.Key == 61)
        //        {
        //            query = query.Where(e => string.Join(",", ProductRights[61].Where(_ => _.Exception == 0).Select(_ => _.Rights)).Split(',').AsEnumerable<string>().Cast<long>().ToList().Contains(e.IdSubSector));
        //        }
        //    }
            
        //    return new List<DataFacebook>();
        //}
    }
}
