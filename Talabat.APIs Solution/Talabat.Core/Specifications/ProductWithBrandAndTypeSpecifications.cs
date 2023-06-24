using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandAndTypeSpecifications : BaseSpecifications<Product>
    {
        // This CTOR Used For Get All Products

        public ProductWithBrandAndTypeSpecifications(ProductSpecParams specParams)
            :base(P =>
                                (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search)) &&
                                (!specParams.BrandId.HasValue || P.ProductBrandId == specParams.BrandId.Value) &&
                                (!specParams.TypeId.HasValue  || P.ProductTypeId  == specParams.TypeId.Value)
                 )
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductType);

            // Sorting
            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc": AddOrderBy(P => P.Price);
                        break;
                    case "priceDesc": AddOrderByDecsending(P => P.Price); 
                        break;

                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }

            // Pagination
            ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1),specParams.PageSize);



        }

        // This CTOR Used For Get Specific Products With Id
        public ProductWithBrandAndTypeSpecifications(int id):base(P => P.Id == id)
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductType);
        }

    }
}
