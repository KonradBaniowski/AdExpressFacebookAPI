using Facebook.Service.Core.DomainModels.AdExprSchema;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IDataFacebookRepository: IGenericRepository<DataFacebook>
    {
        //List<DataFacebook> GetDataFacebook(Dictionary<int, List<RightDomain>> ProductRights, Dictionary<int, List<RightDomain>> MediasRights, DateTime Begin, DateTime End, List<int> Advertiser, List<int> Brand);
    }
}
