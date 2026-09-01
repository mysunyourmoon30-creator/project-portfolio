using Innovation.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Innovation.Data;

// One consolidated SQLite database for the whole demo slice - see
// ConsolidatedTables.cs for why this deliberately differs from the real
// system's 16-database split.
public class SiloDbContext : DbContext
{
    public SiloDbContext(DbContextOptions<SiloDbContext> options) : base(options)
    {
    }

    public DbSet<KbTogether> KbTogether => Set<KbTogether>();
    public DbSet<Weighting> Weighting => Set<Weighting>();
    public DbSet<TotalWeight> TotalWeight => Set<TotalWeight>();
    public DbSet<TwAcceptWeightHis> TwAcceptWeightHis => Set<TwAcceptWeightHis>();
    public DbSet<SendStepParameter> SendStepParameter => Set<SendStepParameter>();
    public DbSet<Station> Station => Set<Station>();
    public DbSet<UsrWt> UsrWt => Set<UsrWt>();
    public DbSet<TrayPlan> TrayPlan => Set<TrayPlan>();
    public DbSet<TrayWeight> TrayWeight => Set<TrayWeight>();
    public DbSet<TrayBarcode> TrayBarcode => Set<TrayBarcode>();
    public DbSet<TypeTray> TypeTray => Set<TypeTray>();
    public DbSet<RmBal> RmBal => Set<RmBal>();
    public DbSet<SiloApprove> SiloApprove => Set<SiloApprove>();
    public DbSet<OnHand> OnHand => Set<OnHand>();
    public DbSet<ProdstdMixtemp> ProdstdMixtemp => Set<ProdstdMixtemp>();
    public DbSet<ApplicationSetting> ApplicationSetting => Set<ApplicationSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        const int decimalPrecision = 18;
        const int decimalScale = 4;

        modelBuilder.Entity<KbTogether>(e => e.HasIndex(x => x.Barcode));

        modelBuilder.Entity<Weighting>(e =>
        {
            e.Property(x => x.TargetWeight).HasPrecision(decimalPrecision, decimalScale);
            e.Property(x => x.ActualWeight).HasPrecision(decimalPrecision, decimalScale);
        });

        modelBuilder.Entity<TotalWeight>(e =>
        {
            e.HasIndex(x => x.KbTogetherId).IsUnique();
            e.Property(x => x.TotalActualWeight).HasPrecision(decimalPrecision, decimalScale);
        });

        modelBuilder.Entity<TwAcceptWeightHis>(e =>
            e.Property(x => x.AcceptedWeight).HasPrecision(decimalPrecision, decimalScale));

        modelBuilder.Entity<UsrWt>(e => e.HasIndex(x => x.LoginName).IsUnique());

        modelBuilder.Entity<TrayPlan>(e =>
            e.Property(x => x.PlannedWeight).HasPrecision(decimalPrecision, decimalScale));

        modelBuilder.Entity<TrayWeight>(e =>
            e.Property(x => x.ActualWeight).HasPrecision(decimalPrecision, decimalScale));

        modelBuilder.Entity<TrayBarcode>(e => e.HasIndex(x => x.Barcode).IsUnique());

        modelBuilder.Entity<TypeTray>(e =>
            e.Property(x => x.MaxCapacity).HasPrecision(decimalPrecision, decimalScale));

        modelBuilder.Entity<RmBal>(e =>
        {
            e.HasIndex(x => x.RawMaterialBarcode).IsUnique();
            e.Property(x => x.Balance).HasPrecision(decimalPrecision, decimalScale);
        });

        modelBuilder.Entity<SiloApprove>(e => e.HasIndex(x => x.Barcode));

        modelBuilder.Entity<OnHand>(e =>
            e.Property(x => x.Quantity).HasPrecision(decimalPrecision, decimalScale));

        modelBuilder.Entity<ProdstdMixtemp>(e =>
        {
            e.HasIndex(x => x.PlanId);
            e.Property(x => x.Temperature).HasPrecision(decimalPrecision, decimalScale);
        });
    }
}
