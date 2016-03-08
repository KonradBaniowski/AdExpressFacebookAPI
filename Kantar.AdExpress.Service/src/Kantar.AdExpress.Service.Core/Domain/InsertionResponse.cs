using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Results;

namespace Kantar.AdExpress.Service.Core.Domain
{
  public  class InsertionResponse
    {

        public GridResult GridResult { get; set; }

        public string Message { get; set; }

    }
}
