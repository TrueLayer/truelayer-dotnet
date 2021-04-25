using System;
using Microsoft.EntityFrameworkCore;

namespace TrueLayerSdk.SampleApp.Data
{
    public class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions options) : base(options) { }
        
        public DbSet<PaymentEntity> Payments { get; set; }
    }

    public class PaymentEntity
    {
        public string PaymentEntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
