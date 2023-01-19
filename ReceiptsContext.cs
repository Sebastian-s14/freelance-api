using Microsoft.EntityFrameworkCore;
using FreelanceApi2.Models;

namespace FreelanceApi2;

public class ReceiptsContext : DbContext
{
    public DbSet<Receipt>? Receipts { get; set; }
    // public DbSet<User>? Users { get; set; }

    public ReceiptsContext(DbContextOptions<ReceiptsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        List<Receipt> receiptsInit = new List<Receipt>();
        receiptsInit.Add(new Receipt()
        {
            ReceiptId = Guid.Parse("fe2de405-c38e-4c90-ac52-da0540dfb4ea"),
            // UserId = Guid.Parse("fe2de405-c38e-4c90-ac52-da0540dfb410"),
            Name = "User 1",
            LastName = "dev 1",
            Address = "Lima",
            Title = "Receipt 1",
            Description = "some description",
            currency = "USD",
            payment = 500,
            Logo = "image url",
            TypeDocument = "DNI",
            NumberDocument = "77777777",
            CreateAt = DateTime.Now
        });
        receiptsInit.Add(new Receipt()
        {
            ReceiptId = Guid.Parse("fe2de405-c38e-4c90-ac52-da0540dfb4ef"),
            // UserId = Guid.Parse("fe2de405-c38e-4c90-ac52-da0540dfb411"),
            Name = "user 2",
            LastName = "dev 2",
            Address = "La Molina",
            Title = "Receipt 2",
            Description = "random description",
            currency = "SOL",
            payment = 200,
            Logo = "image url",
            TypeDocument = "RUC",
            NumberDocument = "10777777771",
            CreateAt = DateTime.Now
        });

        modelBuilder.Entity<Receipt>(receipt =>
        {
            receipt.ToTable("Receipt");
            receipt.HasKey(r => r.ReceiptId);
            // receipt.HasOne(r => r.User).WithMany(u => u.Receipts).HasForeignKey(r => r.UserId);
            receipt.Property(r => r.Title);
            receipt.Property(r => r.Description);
            receipt.Property(r => r.Address);
            receipt.Property(r => r.Name);
            receipt.Property(r => r.LastName);
            receipt.Property(r => r.Logo);
            receipt.Property(r => r.TypeDocument);
            receipt.Property(r => r.NumberDocument);
            receipt.Property(r => r.currency);
            receipt.Property(r => r.payment);

            receipt.HasData(receiptsInit);
        });


    }

}