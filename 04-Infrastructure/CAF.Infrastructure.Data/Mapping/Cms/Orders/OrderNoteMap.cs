using CAF.Infrastructure.Core.Domain.Cms.Orders;
using System.Data.Entity.ModelConfiguration;
 

namespace CAF.Infrastructure.Data.Mapping.Orders
{
    public partial class OrderNoteMap : EntityTypeConfiguration<OrderNote>
    {
        public OrderNoteMap()
        {
            this.ToTable("OrderNote");
            this.HasKey(on => on.Id);
            this.Property(on => on.Note).IsRequired().IsMaxLength();

            this.HasRequired(on => on.Order)
                .WithMany(o => o.OrderNotes)
                .HasForeignKey(on => on.OrderId);
        }
    }
}