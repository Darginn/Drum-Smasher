using DSServer.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Database
{
    public partial class DSContext : DbContext
    {
        public DSContext()
        {
        }

        public DSContext(DbContextOptions<DbContext> options) : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<ChatChannel> ChatChannel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("DBConnectionString", EnvironmentVariableTarget.Process), builder =>
                {
                    builder.EnableRetryOnFailure(25, TimeSpan.FromSeconds(2), null);
                }).EnableSensitiveDataLogging();

                base.OnConfiguring(optionsBuilder);
                //optionsBuilder.UseMySql(Program.DBConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AccountName)
                    .HasColumnName("account_name")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasColumnType("text");

                entity.Property(e => e.PasswordSalt)
                    .HasColumnName("password_salt")
                    .HasColumnType("text");

                entity.Property(e => e.LastLogin)
                    .HasColumnName("last_login")
                    .HasColumnType("datetime");

                entity.Property(e => e.ResetPasswordNextLogin)
                    .HasColumnName("reset_password_next_login")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.DiscordId)
                    .HasColumnName("discord_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.IsBanned)
                    .HasColumnName("is_banned")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.PermissionLevel)
                    .HasColumnName("permission_level")
                    .HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<ChatChannel>(entity =>
            {
                entity.ToTable("chat_channel");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.ChatType)
                    .HasColumnName("chat_type")
                    .HasColumnType("smallint(1)");

                entity.Property(e => e.Owner)
                    .HasColumnName("owner")
                    .HasColumnType("bigint(20)");
            });

        }
    }
}