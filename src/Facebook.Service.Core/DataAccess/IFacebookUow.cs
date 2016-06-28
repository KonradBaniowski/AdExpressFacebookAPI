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
        IOrderClientProductRepository OrderClientProductRepository { get; }
        IOrderTemplateProductRepository OrderTemplateProductRepository { get; }
        ITemplateAssignmentRepository TemplateAssignmentRepository { get; }
        IDataFacebookRepository DataFacebookRepository { get; }
        IDataPostFacebookRepository DataPostFacebookRepository { get; }
        IProductRepository ProductRepository { get; }
        void Dispose();
        void Save();
    }
}
