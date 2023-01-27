using Microsoft.EntityFrameworkCore;

namespace Office_Management_.NET_MVC_Angular_JWT.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Asset> Assets { get; set; } = null!;
        public virtual DbSet<AssetHistory> AssetHistories { get; set; } = null!;
        public virtual DbSet<Inventory> Inventories { get; set; } = null!;
        public virtual DbSet<Requisition> Requisitions { get; set; } = null!;
        public virtual DbSet<Leave> Leaves { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NOTE: only properties which are mapped with changed name of table column is writter here
            // others are mapped by default configuration, so no need to mention explicitly
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.DateJoined).HasColumnName("date_joined");
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.IsSuperuser).HasColumnName("is_superuser");
                entity.Property(e => e.LastLogin).HasColumnName("last_login");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.CanApproveInventory).HasColumnName("can_approve_inventory");
                entity.Property(e => e.CanApproveLeave).HasColumnName("can_approve_leave");
                entity.Property(e => e.CanDistributeInventory).HasColumnName("can_distribute_inventory");
                entity.Property(e => e.CanManageAsset).HasColumnName("can_manage_asset");
                entity.Property(e => e.SupervisorId).HasColumnName("supervisor_id");

                entity.HasOne(d => d.Supervisor)
                        .WithMany(p => p.UserSupervisors)
                        .HasForeignKey(d => d.SupervisorId);
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("asset");

                entity.Property(e => e.CreationDate).HasColumnName("creation_date");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.NextUserId).HasColumnName("next_user_id");
                entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.NextUser)
                        .WithMany(p => p.AssetNextUsers)
                        .HasForeignKey(d => d.NextUserId);
                entity.HasOne(d => d.User)
                        .WithMany(p => p.AssetUsers)
                        .HasForeignKey(d => d.UserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AssetHistory>(entity =>
            {
                entity.ToTable("assethistory");

                entity.Property(e => e.AssetId).HasColumnName("asset_id");
                entity.Property(e => e.CreationDate).HasColumnName("creation_date");
                entity.Property(e => e.FromUserId).HasColumnName("from_user_id");
                entity.Property(e => e.ToUserId).HasColumnName("to_user_id");

                entity.HasOne(d => d.Asset)
                        .WithMany(p => p.AssetHistories)
                        .HasForeignKey(d => d.AssetId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.FromUser)
                        .WithMany(p => p.AssetHistoryFromUsers)
                        .HasForeignKey(d => d.FromUserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.ToUser)
                        .WithMany(p => p.AssetHistoryToUsers)
                        .HasForeignKey(d => d.ToUserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("inventory");

                entity.Property(e => e.LastModifiedDate).HasColumnName("last_modified_date");
            });

            modelBuilder.Entity<Requisition>(entity =>
            {
                entity.ToTable("requisition");

                entity.Property(e => e.ApproveDate).HasColumnName("approve_date");
                entity.Property(e => e.ApproverId).HasColumnName("approver_id");
                entity.Property(e => e.DistributionDate).HasColumnName("distribution_date");
                entity.Property(e => e.DistributorId).HasColumnName("distributor_id");
                entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
                entity.Property(e => e.RequestDate).HasColumnName("request_date");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Approver)
                        .WithMany(p => p.RequisitionApprovers)
                        .HasForeignKey(d => d.ApproverId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Distributor)
                        .WithMany(p => p.RequisitionDistributors)
                        .HasForeignKey(d => d.DistributorId);
                entity.HasOne(d => d.Inventory)
                        .WithMany(p => p.Requisitions)
                        .HasForeignKey(d => d.InventoryId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.User)
                        .WithMany(p => p.RequisitionUsers)
                        .HasForeignKey(d => d.UserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Leave>(entity =>
            {
                entity.ToTable("leave");

                entity.Property(e => e.ApproveDate).HasColumnName("approve_date");
                entity.Property(e => e.ApproverId).HasColumnName("approver_id");
                entity.Property(e => e.CreationDate).HasColumnName("creation_date");
                entity.Property(e => e.DayCount).HasColumnName("day_count");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Approver)
                        .WithMany(p => p.LeaveApprovers)
                        .HasForeignKey(d => d.ApproverId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.User)
                        .WithMany(p => p.LeaveUsers)
                        .HasForeignKey(d => d.UserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
