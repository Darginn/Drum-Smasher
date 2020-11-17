using DSServer.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

public class DB : DbContext
{
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Warning> Warnings { get; set; }

    /// <summary>
    /// Path to db connection string file
    /// </summary>
    const string _CONSTR_LOC = "dbcon";
    const bool _USE_MARIADB = true;

    public DB()
    {
    }

    public DB(DbContextOptions<DbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        CreateUserModels(builder);
    }

    void CreateUserModels(ModelBuilder builder)
    {
        builder.Entity<Account>(en =>
        {
            en.ToTable("account");

            en.Property(e => e.Id)
              .HasColumnName("id")
              .HasColumnType("bigint(20)");
            
            en.Property(e => e.AccountName)
              .HasColumnName("account_name")
              .HasColumnType("longtext");
            
            //TODO: is "blob" the right type name?
            en.Property(e => e.PasswordHash)
              .HasColumnName("password_hash")
              .HasColumnType("blob");

            en.Property(e => e.Salt)
              .HasColumnName("salt")
              .HasColumnType("blob");

            en.Property(e => e.AccessLevel)
              .HasColumnName("access_level")
              .HasColumnType("int(11)");
            
            en.Property(e => e.DisplayName)
              .HasColumnName("display_name")
              .HasColumnType("longtext");
            
            en.Property(e => e.IsBanned)
              .HasColumnName("is_banned")
              .HasColumnType("tinyint(1)");
        });

        builder.Entity<Warning>(en =>
        {
            en.ToTable("warning");

            en.Property(e => e.Id)
              .HasColumnName("id")
              .HasColumnType("bigint(20)");

            en.Property(e => e.AccountId)
              .HasColumnName("account_id")
              .HasColumnType("bigint(20)");

            en.Property(e => e.Reason)
              .HasColumnName("reason")
              .HasColumnType("longtext");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = LoadConnectionString(_CONSTR_LOC);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(connectionString, builder =>
            {
                builder.EnableRetryOnFailure(25, TimeSpan.FromSeconds(2), null);

                if (_USE_MARIADB)
                    builder.ServerVersion(new Version(10, 1, 41), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb);
            }).EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }
    }

    string LoadConnectionString(string file)
    {
        if (string.IsNullOrEmpty(file))
            throw new ArgumentNullException(nameof(file));
        else if (!System.IO.File.Exists(file))
            throw new System.IO.FileNotFoundException("File not found", file);

        return System.IO.File.ReadAllText(file);
    }
}
