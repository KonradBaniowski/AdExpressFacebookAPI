using Kantar.AdExpress.Service.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.BusinessLogic
{
    public class AddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void GetAllAddress()
        {
            _unitOfWork.AddressRepository.GetAll();
        }
    }
}
