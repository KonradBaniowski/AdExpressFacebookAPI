using AutoMapper;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.BusinessService;
using Facebook.Service.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.BusinessLogic.ServiceImpl
{
    public class ProductService :IProductService
    {
        private IFacebookUow _uow;
        private readonly IMapper _mapper;
        public ProductService ( IFacebookUow uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public List<ProductContract> GetProducts(string  keyword)
        {
            var products = _uow.ProductRepository.Find(p => p.Name.Contains(keyword)).ToList();
            var result = _mapper.Map<List<ProductContract>>(products);
            return result;
        }
    }
}
