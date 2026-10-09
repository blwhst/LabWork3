using System;
using System.Collections.Generic;

namespace ShoeStoreData.Models;

public partial class Material
{
    public int MaterialId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
