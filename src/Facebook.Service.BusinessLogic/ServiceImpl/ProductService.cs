using AutoMapper;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.BusinessService;
using Facebook.Service.Core.DataAccess;
using Facebook.Service.Core.DomainModels.AdExprSchema;
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

        public List<LevelItemContract> GetItems(string  keyword, int level)
        {
            var products = new List<LevelItem>();
            switch (level)
            {
                case 1://Sector
                    products= _uow.ProductRepository.Find(p => p.Sector.Contains(keyword)).ToList();
                    break;
                case 2://SubSector
                    products = _uow.ProductRepository.Find(p => p.SubSector.Contains(keyword)).ToList();
                    break;
                case 3://Group_
                    products = _uow.ProductRepository.Find(p => p.Group.Contains(keyword)).ToList();
                    break;
                case 4://Segment
                    products = _uow.ProductRepository.Find(p => p.Segment.Contains(keyword)).ToList();
                    break;
                case 5://Product
                    products = _uow.ProductRepository.Find(p => p.Product.Contains(keyword)).ToList();
                    break;
                case 6://Advertiser
                    products = _uow.ProductRepository.Find(p => p.Advertiser.Contains(keyword)).ToList();
                    break;
                case 7://Holding Company
                    products = _uow.ProductRepository.Find(p => p.HoldingCompany.Contains(keyword)).ToList();
                    break;
                case 8://Brand
                    products = _uow.ProductRepository.Find(p => p.Brand.Contains(keyword)).ToList();
                    break;
                default:
                    break;
            }
            var result = _mapper.Map<List<LevelItemContract>>(products);
            return result;
        }
        public List<LevelItemContract> GetItems(int level, string selectedItemIds, int selectedLevel)
        {
            var items = new List<LevelItem>();
            List<long> selectedIds = selectedItemIds.Split(',').Select(p => long.Parse(p)).ToList();            
            var result = _mapper.Map<List<LevelItemContract>>(items);
            return result;
        }
    }
}
