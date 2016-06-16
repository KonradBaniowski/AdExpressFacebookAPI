using Facebook.Service.Core.DataAccess.Repository;
using Facebook.Service.Core.DomainModels.MauSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.DataAccess.Repository
{
    public class TemplateAssignmnetRepository : GenericRepository<TemplateAssignment>, ITemplateAssignmentRepository
    {
        public TemplateAssignmnetRepository(FacebookContext context) : base(context)
        {

        }
    }
}
