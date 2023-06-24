using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        // in .NET 6.00 by Default String Not Allow NULL

        // ONE Brand has Many Products
        // ONE Product has ONE Brand
        // Relation mn n7yt el product one w mn n7yt el Brand Many

        //[ForeignKey("ProductBrand")]
        public int ProductBrandId { get; set; } // ForeignKey NOT ALLOW Null

        public int ProductTypeId { get; set; } // ForeignKey NOT ALLOW Null
        public ProductBrand ProductBrand { get; set; } // Nav Property => ONE

        // ONE Type has Many Products
        // ONE Product has ONE Type

        public ProductType ProductType { get; set; } // Nav Property => ONE



    }
}
