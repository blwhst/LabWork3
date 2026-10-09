namespace ShoeStoreData.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public int SubcategoryId { get; set; }

    public string Manufacturer { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();

    public virtual Subcategory Subcategory { get; set; } = null!;

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
