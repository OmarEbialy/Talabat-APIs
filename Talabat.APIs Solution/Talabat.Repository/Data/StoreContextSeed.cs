using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync (StoreContext dbContext) // hnady el func de fe program fe el try bta3t el Update b3d ma y3ml Migrations
        {
            #region ProductBrand Insertion
            if (!dbContext.ProductBrands.Any()) // lw feh brand mt3ml4 insert lw mfi4 brand e3ml insert
            {
                var brandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json"); // Read File
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData); // h7wl Json File to List<> mn Brand

                if (brands is not null && brands.Count > 0)
                {
                    foreach (var brand in brands)
                        await dbContext.Set<ProductBrand>().AddAsync(brand); // add brand in Db

                    await dbContext.SaveChangesAsync();
                }
            }

            #endregion

            #region ProductTypes Insertion
            if (!dbContext.ProductTypes.Any()) // lw feh brand mt3ml4 insert lw mfi4 brand e3ml insert
            {
                var typesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/types.json"); // Read File
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData); // h7wl Json File to List<> mn Brand

                if (types is not null && types.Count > 0)
                {
                    foreach (var type in types)
                        await dbContext.Set<ProductType>().AddAsync(type); // add brand in Db

                    await dbContext.SaveChangesAsync();
                }
            }

            #endregion

            #region Product Insertion
            if (!dbContext.Products.Any()) // lw feh brand mt3ml4 insert lw mfi4 brand e3ml insert
            {
                var productsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json"); // Read File
                var products = JsonSerializer.Deserialize<List<Product>>(productsData); // h7wl Json File to List<> mn Brand

                if (products is not null && products.Count > 0)
                {
                    foreach (var product in products)
                        await dbContext.Set<Product>().AddAsync(product); // add brand in Db

                    await dbContext.SaveChangesAsync();
                }
            }

            #endregion

            #region Delivery Seeding
            if (!dbContext.DeliveryMethods.Any()) // lw feh brand mt3ml4 insert lw mfi4 brand e3ml insert
            {
                var deliveryMethodData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json"); // Read File
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodData); // h7wl Json File to List<> mn Brand

                if (deliveryMethods?.Count > 0)
                {
                    foreach (var deliveryMethod in deliveryMethods)
                        await dbContext.Set<DeliveryMethod>().AddAsync(deliveryMethod); // add brand in Db

                    await dbContext.SaveChangesAsync();
                }
            }

            #endregion


        }
    }
}
