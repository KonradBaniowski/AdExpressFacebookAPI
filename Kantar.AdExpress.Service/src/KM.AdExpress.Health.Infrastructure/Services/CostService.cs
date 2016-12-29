using KM.AdExpress.Health.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KM.AdExpress.Health.Infrastructure.Contract;
using KM.AdExpress.Health.Core.Interfaces;
using AutoMapper;

namespace KM.AdExpress.Health.Infrastructure.Services
{
    public class CostService : ICostService
    {

        private IHealthUow _uow;
        private readonly IMapper _mapper;

        public CostService(IHealthUow uow, IMapper mapper)
        {

            _mapper = mapper;
            _uow = uow;
        }

        public List<DataCostContract> GetDataCost(UserCriteriaContract userCriteria)
        {
            List<DataCostContract> contract = new List<DataCostContract>();


            contract = _uow.DataCostRepository.GetData(userCriteria);


            return contract;

        }
    }
}
