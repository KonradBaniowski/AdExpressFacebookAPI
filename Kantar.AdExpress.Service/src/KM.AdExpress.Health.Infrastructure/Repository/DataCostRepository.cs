using KM.AdExpress.Health.Core.Interfaces.Repository;
using KM.AdExpress.Health.Core.Model;
using KM.AdExpress.Health.Infrastructure.Contract;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Repository
{
    public class DataCostRepository : GenericRepository<DataCost>, IDataCostRepository
    {
        public DataCostRepository(HealthContext context) : base(context)
        {

        }

        public List<DataCostContract> GetData(UserCriteriaContract userContract)
        {
            var predicateBuilder = PredicateBuilder.New<DataCost>();

            var query = (from d in context.DataCost
                         join jCanal in context.Canal on new { id = d.IdCanal } equals new { id = jCanal.IdCanal }
                         join jCategory in context.Category on new { id = d.IdCanal } equals new { id = jCategory.IdCategory }
                         join jSpecialist in context.Specialist on new { id = d.IdCanal } equals new { id = jSpecialist.IdSpecialist }
                         join jGrpPharma in context.GrpPharma on new { id = d.IdCanal } equals new { id = jGrpPharma.IdGrpPharma }
                         join jLaboratory in context.Laboratory on new { id = d.IdCanal } equals new { id = jLaboratory.IdLaboratory }
                         join jProduct in context.Product on new { id = d.IdCanal } equals new { id = jProduct.IdProduct }
                         join jFormat in context.Format on new { id = d.IdCanal } equals new { id = jFormat.IdFormat }
                         where d.Date >= userContract.StartDate && d.Date <= userContract.EndDate
                         && userContract.CanalIds.Contains(d.IdCanal)
                         select d);


            //Include
            if (userContract.CategoryIncludeIds != null && userContract.CategoryIncludeIds.Any())
                query = query.Include(t => t.Category).Where(e => userContract.CategoryIncludeIds.Contains(e.IdCategory));

            if (userContract.ProductIncludeIds != null && userContract.ProductIncludeIds.Any())
                query = query.Include(t => t.Product).Where(e => userContract.ProductIncludeIds.Contains(e.IdProduct));

            if (userContract.GrpPharmaIncludeIds != null && userContract.GrpPharmaIncludeIds.Any())
                query = query.Include(t => t.GrpPharma).Where(e => userContract.GrpPharmaIncludeIds.Contains(e.IdGrpPharma));


            //Exclude
            if (userContract.CategoryExcludeIds != null && userContract.CategoryExcludeIds.Any())
                query = query.Include(t => t.Category).Where(e => !userContract.CategoryIncludeIds.Contains(e.IdCategory));

            if (userContract.ProductExcludeIds != null && userContract.ProductExcludeIds.Any())
                query = query.Include(t => t.Product).Where(e => !userContract.ProductIncludeIds.Contains(e.IdProduct));

            if (userContract.GrpPharmaExcludeIds != null && userContract.GrpPharmaExcludeIds.Any())
                query = query.Include(t => t.GrpPharma).Where(e => !userContract.GrpPharmaIncludeIds.Contains(e.IdGrpPharma));

            var res = (from p in query
                       select new DataCostContract
                       {
                           IdCanal = p.IdCanal,
                           Canal = p.Canal.CanalLabel,
                           IdCategory = p.IdCategory,
                           Category = p.Category.CategoryLabel,
                           IdSpecialist = p.IdSpecialist,
                           Specialist = p.Specialist.SpecialistLabel,
                           IdGrpPharma = p.IdGrpPharma,
                           GrpPharma = p.GrpPharma.GrpPharmaLabel,
                           IdLabratory = p.IdLaboratory,
                           Laboratory = p.Laboratory.LaboratoryLabel,
                           IdProduct = p.IdProduct,
                           Product = p.Product.ProductLabel,
                           IdFormat = p.IdFormat,
                           Format = p.Format.FormatLabel,
                           Date = p.Date,
                           Euro = p.Euro,

                       }).ToList();


            return res;
        }
    }
}
