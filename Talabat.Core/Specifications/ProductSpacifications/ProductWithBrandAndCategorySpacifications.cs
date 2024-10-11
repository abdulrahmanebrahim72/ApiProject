﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;


namespace Talabat.Core.Specifications.ProductSpacifications
{
    public class ProductWithBrandAndCategorySpacifications : BaseSpecifications<Product>
    {
        public ProductWithBrandAndCategorySpacifications(ProductSpecParams specParams) 
            : base(p =>
                       (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search.ToLower()))&&
                       (!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId.Value) &&
                       (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value)  
                  )
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc" : OrderBy = p => p.Price; break;
                    case "priceDesc": OrderByDesc = p => p.Price; break;
                    default : OrderBy = p => p.Name; break;
                }
            }
            else
            {
                OrderBy = p => p.Name;
            }

            ApplyPagination((specParams.PageIndex - 1) * (specParams.PageSize), specParams.PageSize);

        }
        public ProductWithBrandAndCategorySpacifications(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }
    }
}
