using System;
using System.Collections.Generic;

namespace ShoeStoreData.Models;

public partial class ProductItem
{
    public int ProductItemId { get; set; }

    public int ProductId { get; set; }

    public decimal Size { get; set; }

    public int Quantity { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;
}
