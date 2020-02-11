using Microsoft.EntityFrameworkCore;

namespace Moneymo.AuthenticationService.Data.Models
{
    public partial class MoneymoDbContext : DbContext
    {
        public MoneymoDbContext()
        {
        }

        public MoneymoDbContext(DbContextOptions<MoneymoDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FwPerson> FwPersons { get; set; }
        public virtual DbSet<FwSession> FwSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<FwPerson>(entity =>
            {
                entity.ToTable("fw_person");

                entity.HasIndex(e => e.UserName)
                    .HasName("fw_person_user_name_unq")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.LastUpd).HasColumnName("last_upd");

                entity.Property(e => e.LastUpdBy)
                    .HasColumnName("last_upd_by")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("character varying");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasColumnName("salt")
                    .HasColumnType("character varying");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.UpdServiceId).HasColumnName("upd_service_id");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<FwSession>(entity =>
            {
                entity.ToTable("fw_session");

                entity.HasIndex(e => e.SessionKey)
                    .HasName("fw_session_key_unq")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.Property(e => e.SessionKey)
                    .IsRequired()
                    .HasColumnName("session_key")
                    .HasColumnType("character varying");

                entity.Property(e => e.UpdServiceId).HasColumnName("upd_service_id");

                entity.Property(e => e.Valid)
                    .HasColumnName("valid")
                    .HasDefaultValueSql("now()");
            });

            modelBuilder.HasSequence("fw_event_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("fw_person_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("fw_role_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("fw_session_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("mc_branch_id_seq");

            modelBuilder.HasSequence("mc_product_id_seq")
                .StartsAt(1001)
                .HasMax(2147483647);

            modelBuilder.HasSequence("mp_bkm_order_id_seq");

            modelBuilder.HasSequence("mp_order_id_seq");

            modelBuilder.HasSequence("wirecard_payments_id_seq");
        }
    }
}
