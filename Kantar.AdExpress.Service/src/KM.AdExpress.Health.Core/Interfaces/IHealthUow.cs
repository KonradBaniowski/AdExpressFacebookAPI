using KM.AdExpress.Health.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Core.Interfaces
{
    public interface IHealthUow
    {
        IDataCostRepository DataCostRepository { get; }

        void Dispose();
        void Save();
    }
}
