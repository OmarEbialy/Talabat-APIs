using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data.Config
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Order> builder)
        {
            // 3awz a5le el values lly fe address yt7wlo l columns fe Order Database
            builder.OwnsOne(O => O.ShippingAddress, Address => Address.WithOwner());

            // Configure Enums to Saved As String and Return As Enums to Me
            builder.Property(O => O.Status)
                   .HasConversion(OStatus => OStatus.ToString() , OStatus => (OrderStatus) Enum.Parse(typeof(OrderStatus), OStatus));

            builder.Property(O => O.SubTotal).HasColumnType("decimal(18,2)");

            builder.HasMany(O => O.Items).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
