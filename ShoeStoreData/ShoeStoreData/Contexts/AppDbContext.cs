using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoeStoreData.Models;

namespace ShoeStoreData.Contexts;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductItem> ProductItems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=mssql;Initial Catalog=ispp3406;User ID=ispp3406;Password=3406;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.HasIndex(e => e.Name, "UQ_Category_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("Material");

            entity.HasIndex(e => e.Name, "UQ_Material_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(CONVERT([date],getdate()))", "DF_Orders_OrderDate");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");

            entity.HasIndex(e => new { e.OrderId, e.ProductItemId }, "UQ_OrderItem").IsUnique();

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Orders");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_ProductItem");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.HasIndex(e => new { e.Name, e.Manufacturer }, "UQ_Product_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Image).HasMaxLength(260);
            entity.Property(e => e.Manufacturer).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Subcategory");

            entity.HasMany(d => d.Materials).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductMaterial",
                    r => r.HasOne<Material>().WithMany()
                        .HasForeignKey("MaterialId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProductMaterial_Material"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProductMaterial_Product"),
                    j =>
                    {
                        j.HasKey("ProductId", "MaterialId");
                        j.ToTable("ProductMaterial");
                    });
        });

        modelBuilder.Entity<ProductItem>(entity =>
        {
            entity.ToTable("ProductItem");

            entity.HasIndex(e => new { e.ProductId, e.Size }, "UQ_ProductItem").IsUnique();

            entity.Property(e => e.Size).HasColumnType("decimal(3, 1)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductItem_Product");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ_Role_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.ToTable("Subcategory");

            entity.HasIndex(e => new { e.CategoryId, e.Name }, "UQ_Subcategory").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Subcategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subcategory_Category");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Login, "UQ_Users_Login").IsUnique();

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Login).HasMaxLength(30);
            entity.Property(e => e.MiddleName).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
