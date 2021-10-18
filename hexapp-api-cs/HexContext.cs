using System;
using hexapp_api_cs.Models;
using hexapp_api_cs.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace hexapp_api_cs
{
    public partial class HexContext : DbContext
    {
        public HexContext() { }

        public HexContext(DbContextOptions<HexContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<HealthMetric> HealthMetrics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Startup.StaticConfiguration.GetSection("DatabaseSettings")["ConnectionString"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("User_pk")
                    .IsClustered(false);

                entity.ToTable("User");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.HashedPassword)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.TokenId)
                    .HasName("Token_pk")
                    .IsClustered(false);

                entity.ToTable("Token");

                entity.Property(e => e.CreationTimestampUtc).HasColumnType("datetime");

                entity.Property(e => e.TokenString)
                    .IsUnicode(false)
                    .HasColumnName("Token");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HealthMetric>(entity =>
            {
                entity.HasKey(e => e.HealthMetricId)
                    .HasName("HealthMetric_pk")
                    .IsClustered(false);

                entity.ToTable("HealthMetric");

                entity.Property(e => e.EntryDate).HasColumnType("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
