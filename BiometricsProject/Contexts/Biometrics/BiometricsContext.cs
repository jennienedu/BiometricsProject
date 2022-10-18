using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BiometricsProject.Entities.Biometrics;

#nullable disable

namespace BiometricsProject.Contexts.Biometrics
{
    public partial class BiometricsContext : DbContext
    {
        public BiometricsContext()
        {
        }

        public BiometricsContext(DbContextOptions<BiometricsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BiometricsLog> BiometricsLogs { get; set; }
        public virtual DbSet<StaffBiometricRegistration> StaffBiometricRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BiometricsLog>(entity =>
            {
                entity.ToTable("BiometricsLog");

                entity.Property(e => e.LOCATION).HasMaxLength(100);

                entity.Property(e => e.TEMPERATURE).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.TIMEIN).HasColumnType("datetime");

                entity.Property(e => e.TIMEOUT).HasColumnType("datetime");

                entity.Property(e => e.USERNAME)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<StaffBiometricRegistration>(entity =>
            {
                entity.HasKey(e => e.USERNAME);

                entity.ToTable("StaffBiometricRegistration");

                entity.Property(e => e.USERNAME).HasMaxLength(50);

                entity.Property(e => e.DATEINSERTED).HasColumnType("datetime");

                entity.Property(e => e.EMAIL).HasMaxLength(100);

                entity.Property(e => e.FIRSTNAME).HasMaxLength(50);

                entity.Property(e => e.GROUPNAME).HasMaxLength(50);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();

                entity.Property(e => e.LASTNAME).HasMaxLength(50);

                entity.Property(e => e.LOCATION).HasMaxLength(100);

                entity.Property(e => e.OFFICEALLOCATED).HasMaxLength(100);

                entity.Property(e => e.PHONENUMBER).HasMaxLength(30);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
