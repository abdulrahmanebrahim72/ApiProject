using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpacifications
{
    public class ProductsWithFilterationForCountSpecification : BaseSpecifications<Product>
    {
        public ProductsWithFilterationForCountSpecification(ProductSpecParams specParams)
            : base(p => (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                        (!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId.Value) &&
                        (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value)
                  )
        {
            
        }
    }
}
