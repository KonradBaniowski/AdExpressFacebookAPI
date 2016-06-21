using Facebook.Service.Core.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DataAccess
{
    public interface IFacebookUow
    {
        IOrderClientMediaRepository OrderClientMediaRepository { get; }
        IOrderTemplateMediaRepository OrderTemplateMediaRepository { get; }
        ITemplateAssignmentRepository TemplateAssignmentRepository { get; }
        IDataFacebookRepository DataFacebookRepository { get; }
        IProductRepository ProductRepository { get; }
        void Dispose();
        void Save();
    }
}
